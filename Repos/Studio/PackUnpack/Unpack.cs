/*
* @(#)UnpackData.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.PackUnpack
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Windows.Forms;

	using ICSharpCode.SharpZipLib.Zip;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.Attachment;
	using Newtera.WinClientCommon;
    using Newtera.WindowsControl;

	/// <summary>
	/// Pack the database data, including metadata, instance data, attachments, and
	/// log data into a zip file
	/// </summary>
	/// <version> 1.0.0 20 Apr 2005 </version>
	/// <author> Yong Zhang</author>
	public class UnpackData
	{
		private bool _isCancelled;
		private bool _isOverride;
		private MetaDataModel _metaData;
		private string _packFileName;
		private string _dataDirPath;
		private string _metaDataDir;
		private string _classDataDir;
		private string _attachmentDir;
        private string _errorLog;
		private MethodInvoker _confirmCallback;
		private MetaDataServiceStub _metaDataService;
		private InstanceLoader _instanceLoader;
		private WorkInProgressDialog _workingDialog;

		private const string META_DATA_DIR = "metaData";
		private const string CLASS_DATA_DIR = "classData";
		private const string ATTACHEMENTS_DIR = "attachments";
        private const string PIVOT_LAYOUTS_DIR = "pivots";
        private const string ERROR_LOG = "RestoreLog.txt";

		/// <summary>
		/// Instantiate an instance of UnpackData class
		/// </summary>
		/// <param name="packFileName">The zipped file name.</param>
		/// <param name="dataDirPath">The directory path where to put data files for packing.</param>
		public UnpackData(string packFileName, string dataDirPath)
		{
			_packFileName = packFileName;
			_dataDirPath = dataDirPath;
			_metaDataDir = @".\" + UnpackData.META_DATA_DIR;
			_classDataDir = @".\" + UnpackData.CLASS_DATA_DIR;
			_attachmentDir = @".\" + UnpackData.ATTACHEMENTS_DIR;
            _errorLog = NewteraNameSpace.GetAppHomeDir() +  @"temp\" + UnpackData.ERROR_LOG;
			_metaData = null;
			_isCancelled = false;
			_isOverride = false;
			_confirmCallback = null;
			_workingDialog = null;
		}

		/// <summary>
		/// Gets or sets the information indicating whether to cancel a backup job.
		/// </summary>
		public bool IsCancelled
		{
			get
			{
				return _isCancelled;
			}
			set
			{
				_isCancelled = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to override an existing database.
		/// </summary>
		public bool IsOverride
		{
			get
			{
				return _isOverride;
			}
			set
			{
				_isOverride = value;
			}
		}

		/// <summary>
		/// Gets or sets the working dialog instance
		/// </summary>
		public WorkInProgressDialog WorkingDialog
		{
			get
			{
				return _workingDialog;
			}
			set
			{
				_workingDialog = value;
			}
		}

		/// <summary>
		/// Gets or sets the callback for confirming whether to override an existing database.
		/// </summary>
		public MethodInvoker ConfirmCallback
		{
			get
			{
				return _confirmCallback;
			}
			set
			{
				_confirmCallback = value;
			}
		}

		/// <summary>
		/// Restore a database from a zipped backup file.
		/// </summary>
		public void Restore()
		{
            StreamWriter sw = null;

			string msg = _workingDialog.DisplayText;

			string workingDir = Directory.GetCurrentDirectory();

            try
            {
                // delete the existing directory
                if (Directory.Exists(_dataDirPath))
                {
                    Directory.Delete(_dataDirPath, true);
                }

                // create directories for exported files
                Directory.CreateDirectory(_dataDirPath);

                Directory.SetCurrentDirectory(_dataDirPath);

                if (File.Exists(this._errorLog))
                {
                    File.Delete(this._errorLog);
                }

                sw = new StreamWriter(this._errorLog);

                // unpack database data into files
                UnpackFiles();

                _metaData = ReadMetaData();

                // check if the database of same name exists?
                _metaDataService = new MetaDataServiceStub();
                
                SchemaInfo[] schemaInfos = _metaDataService.GetSchemaInfos();
                bool isExist = false;
                for (int i = 0; i < schemaInfos.Length; i++)
                {
                    if (schemaInfos[i].Name.ToUpper() == _metaData.SchemaInfo.Name.ToUpper() &&
                        schemaInfos[i].Version == _metaData.SchemaInfo.Version)
                    {
                        isExist = true;
                        break;
                    }
                }

                // confirm with user whether to override the database
                if (isExist && _confirmCallback != null)
                {
                    _confirmCallback.DynamicInvoke(null);
                }

                if (isExist && IsOverride)
                {
                    // delete the existing schema
                    try
                    {
                        WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.DeleteDatabase");
                        _metaDataService.DeleteMetaData(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo));
                    }
                    catch (Exception ex)
                    {
                        // ignore the errors
                        sw.WriteLine("************ Message Begin ***********");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("************ Message End *************");
                        sw.WriteLine("");
                    }
                }

                if (!isExist || IsOverride)
                {
                    // Create the database according to the meta data
                    CreateDatabase();

                    // Load instances into the database
                    LoadInstances(sw);

                    // Establish relationships among the instances
                    UpdateRelationships();

                    // Update workflow binding data instance ids
                    UpdateWFBindingInstanceIds();

                    // upload the images
                    LoadImages(sw);

                    // upload instance attachments
                    LoadInstanceAttachements(sw);

                    // upload class attachments
                    LoadClassAttachements(sw);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + @"\n" + ex.StackTrace);
            }
			finally
			{
				// restore current working dir
				Directory.SetCurrentDirectory(workingDir);

                if (sw != null)
                {
                    sw.Close();
                }

				// cleanup data dir
				if (Directory.Exists(_dataDirPath))
				{
					Directory.Delete(_dataDirPath, true);
				}

				_workingDialog.DisplayText = msg;
			}
		}

		/// <summary>
		/// Unpack the files into a directory
		/// </summary>
		private void UnpackFiles()
		{
			FileStream streamWriter = null;
			ZipInputStream s = null;
			ZipEntry theEntry;

			try
			{
				WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.UnpackData");

				s = new ZipInputStream(File.OpenRead(_packFileName));
		
				while ((theEntry = s.GetNextEntry()) != null) 
				{			
					string directoryName = Path.GetDirectoryName(theEntry.Name);
					string fileName      = Path.GetFileName(theEntry.Name);
			
					// create directory
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
			
					if (fileName != String.Empty) 
					{
						streamWriter = File.Create(theEntry.Name);
				
						int size = 1024;
						byte[] data = new byte[1024];
						while (true) 
						{
							size = s.Read(data, 0, data.Length);
							if (size > 0) 
							{
								streamWriter.Write(data, 0, size);
							} 
							else 
							{
								break;
							}
						}
				
						streamWriter.Close();
						streamWriter = null;
					}
				}				
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}

				if (s != null)
				{
					s.Close();
				}
			}
		}

		/// <summary>
		/// Read meta data from the files
		/// </summary>
		/// <returns></returns>
		private MetaDataModel ReadMetaData()
		{
			string[] fileNames = Directory.GetFiles(_metaDataDir);

			MetaDataModel metaData = new MetaDataModel();

			foreach (string fileName in fileNames)
			{
				if (fileName.EndsWith(".schema"))
				{
					metaData.SchemaModel.Read(fileName);
				}
				else if (fileName.EndsWith(".dataview"))
				{
					metaData.DataViews.Read(fileName);
				}
				else if (fileName.EndsWith(".taxonomy"))
				{
					metaData.Taxonomies.Read(fileName);
				}
				else if (fileName.EndsWith(".xacl"))
				{
					metaData.XaclPolicy.Read(fileName);
				}
				else if (fileName.EndsWith(".rules"))
				{
					metaData.RuleManager.Read(fileName);
				}
				else if (fileName.EndsWith(".mappings"))
				{
					metaData.MappingManager.Read(fileName);
				}
				else if (fileName.EndsWith(".selectors"))
				{
					metaData.SelectorManager.Read(fileName);
				}
                else if (fileName.EndsWith(".events"))
                {
                    metaData.EventManager.Read(fileName);
                }
                else if (fileName.EndsWith(".logging"))
                {
                    metaData.LoggingPolicy.Read(fileName);
                }
                else if (fileName.EndsWith(".subscribers"))
                {
                    metaData.SubscriberManager.Read(fileName);
                }
                else if (fileName.EndsWith(".schemaviews"))
                {
                    metaData.XMLSchemaViews.Read(fileName);
                }
                else if (fileName.EndsWith(".apis"))
                {
                    metaData.ApiManager.Read(fileName);
                }
            }

			// clean up any post-data left in the saved schema
			CleanupSchemaVisitor visitor = new CleanupSchemaVisitor();
			metaData.SchemaModel.Accept(visitor);

			return metaData;
		}

		/// <summary>
		/// Create the database according to the meta data
		/// </summary>
		private void CreateDatabase()
		{
			WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.CreateDatabase");

			StringBuilder builder;
			StringWriter writer;
			string[] xmlStrings = new string[12];

			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.SchemaModel.Write(writer);
			// the first string is a xml string for schema model
			xmlStrings[0] = builder.ToString();
			
			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.DataViews.Write(writer);
			// the second string is a xml string for dataviews
			xmlStrings[1] = builder.ToString();
			
			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.XaclPolicy.Write(writer);
			// the third string is a xml string for xacl policy
			xmlStrings[2] = builder.ToString();
			
			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.Taxonomies.Write(writer);
			// the forth string is a xml string for taxonomies
			xmlStrings[3] = builder.ToString();
			
			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.RuleManager.Write(writer);
			// the fifth string is a xml string for rules
			xmlStrings[4] = builder.ToString();

			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.MappingManager.Write(writer);
			// the sixth string is a xml string for mappings
			xmlStrings[5] = builder.ToString();
			
			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.SelectorManager.Write(writer);
			// the seventh string is a xml string for selectors
			xmlStrings[6] = builder.ToString();

            builder = new StringBuilder();
            writer = new StringWriter(builder);
            _metaData.EventManager.Write(writer);
            // the eighth string is a xml string for events
            xmlStrings[7] = builder.ToString();

            builder = new StringBuilder();
            writer = new StringWriter(builder);
            _metaData.LoggingPolicy.Write(writer);
            // the ninth string is a xml string for logging policy
            xmlStrings[8] = builder.ToString();

            builder = new StringBuilder();
            writer = new StringWriter(builder);
            _metaData.SubscriberManager.Write(writer);
            // the tenth string is a xml string for subscribers
            xmlStrings[9] = builder.ToString();

            builder = new StringBuilder();
            writer = new StringWriter(builder);
            _metaData.XMLSchemaViews.Write(writer);
            // the eleventh string is a xml string for xml schema views
            xmlStrings[10] = builder.ToString();

            builder = new StringBuilder();
            writer = new StringWriter(builder);
            _metaData.ApiManager.Write(writer);
            // the twelveth string is a xml string for apis
            xmlStrings[11] = builder.ToString();

            // invoke the web service synchronously
            _metaDataService.SetMetaData(
				ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
				xmlStrings);

			// get the meta data from the server so that we can get class ids created
			// by the server for the classes. this is for uploading class attachments
			xmlStrings = _metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo));

			// create a new metadata
			Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
			schemaInfo.Name = _metaData.SchemaInfo.Name;
			schemaInfo.Version = _metaData.SchemaInfo.Version;
            schemaInfo.ModifiedTime = _metaData.SchemaInfo.ModifiedTime;

			_metaData = new MetaDataModel();
			_metaData.SchemaInfo = schemaInfo;

			// read the xml string for schema
			StringReader reader = new StringReader(xmlStrings[0]);
			_metaData.SchemaModel.Read(reader);

			// read the xml string for data views
			reader = new StringReader(xmlStrings[1]);
			_metaData.DataViews.Read(reader);

			// read the xml string for xacl policy
			reader = new StringReader(xmlStrings[2]);
			_metaData.XaclPolicy.Read(reader);

			// read the xml string for taxonomies
			reader = new StringReader(xmlStrings[3]);
			_metaData.Taxonomies.Read(reader);

			// read the xml string for rules
			reader = new StringReader(xmlStrings[4]);
			_metaData.RuleManager.Read(reader);

			// read the xml string for mappings
			reader = new StringReader(xmlStrings[5]);
			_metaData.MappingManager.Read(reader);

			// read the xml string for selectors
			reader = new StringReader(xmlStrings[6]);
			_metaData.SelectorManager.Read(reader);

            // read the xml string for events
            reader = new StringReader(xmlStrings[7]);
            _metaData.EventManager.Read(reader);

            // read the xml string for logging
            reader = new StringReader(xmlStrings[8]);
            _metaData.LoggingPolicy.Read(reader);

            // read the xml string for subscribers
            reader = new StringReader(xmlStrings[9]);
            _metaData.SubscriberManager.Read(reader);

            // read the xml string for xml schema views
            reader = new StringReader(xmlStrings[10]);
            _metaData.XMLSchemaViews.Read(reader);

            // read the xml string for subscribers
            reader = new StringReader(xmlStrings[11]);
            _metaData.ApiManager.Read(reader);
        }

		/// <summary>
		/// Load the instnaces into the database
		/// </summary>
        private void LoadInstances(StreamWriter sw)
		{
			_instanceLoader = new InstanceLoader(_metaData, _classDataDir, WorkingDialog);
            _instanceLoader.ErrorLog = sw;

			_instanceLoader.LoadInstances(); // load the instances
		}

		/// <summary>
		/// Establish the relationships among instances
		/// </summary>
        private void UpdateRelationships()
		{
			WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.FixRelationships");

			_instanceLoader.UpdateRelationships();
		}

        private void UpdateWFBindingInstanceIds()
        {
            WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.BindingInstanceIds");

            _instanceLoader.UpdateWFBindingInstanceIds();
        }

        /// <summary>
        /// Upload images to the database
        /// </summary>
        private void LoadImages(StreamWriter sw)
        {
            AttachmentLoader attachmentLoader = new AttachmentLoader(_metaData, _attachmentDir, _instanceLoader.ObjIdMappings, WorkingDialog);

            attachmentLoader.LoadImages(sw); // load the images
        }

		/// <summary>
		/// Upload instace attachments to the database
		/// </summary>
		private void LoadInstanceAttachements(StreamWriter sw)
		{
			AttachmentLoader attachmentLoader = new AttachmentLoader(_metaData, _attachmentDir, _instanceLoader.ObjIdMappings, WorkingDialog);

            attachmentLoader.LoadInstanceAttachments(sw); // load the instance attachments
		}

		/// <summary>
		/// Upload class attachments to the database
		/// </summary>
        private void LoadClassAttachements(StreamWriter sw)
		{
			AttachmentLoader attachmentLoader = new AttachmentLoader(_metaData, _attachmentDir, _instanceLoader.ObjIdMappings, WorkingDialog);

			attachmentLoader.LoadClassAttachments(sw); // load the instance attachments
		}
	}
}