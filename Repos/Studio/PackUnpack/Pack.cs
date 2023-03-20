/*
* @(#)PackData.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.PackUnpack
{
	using System;
	using System.IO;
	using System.Xml;
    using System.Data;
	using System.Text;
    using System.ComponentModel;
    using System.Collections.Specialized;
	using Microsoft.Web.Services.Dime;

    using ICSharpCode.SharpZipLib.Checksums;
	using ICSharpCode.SharpZipLib.Zip;
	using ICSharpCode.SharpZipLib.GZip;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.Attachment;
    using Newtera.DataGridActiveX.Pivot;
	using Newtera.WinClientCommon;
    using Newtera.WindowsControl;

	/// <summary>
	/// Pack the database data, including metadata, instance data, attachments, and
	/// log data into a zip file
	/// </summary>
	/// <version> 1.0.0 20 Apr 2005 </version>
	/// <author> Yong Zhang</author>
	public class PackData
	{
		private bool _isCancelled;
		private MetaDataModel _metaData;
		private string _packFileName;
		private Newtera.Common.Core.SchemaInfo _schemaInfo;
		private string _dataDirPath;
		private string _metaDataDir;
		private string _classDataDir;
		private string _attachmentDir;
        private string _pivotLayoutDir;
		private bool _backupAttachments;
        private StringCollection _imageFileNames;
		private WorkInProgressDialog _workingDialog;

		private const string META_DATA_DIR = "metaData";
		private const string CLASS_DATA_DIR = "classData";
		private const string ATTACHEMENTS_DIR = "attachments";
        private const string PIVOT_LAYOUTS_DIR = "pivots";

		private const int PAGE_SIZE = 50;

		/// <summary>
		/// Instantiate an instance of PackData class
		/// </summary>
		/// <param name="packFileName">The zipped file name.</param>
		/// <param name="schemaInfo">The schema information.</param>
		/// <param name="dataDirPath">The directory path where to put data files for packing.</param>
		/// <param name="backupAttachments">Information indicating whether to backup the attachments.</param>
		public PackData(string packFileName, Newtera.Common.Core.SchemaInfo schemaInfo, string dataDirPath,
			bool backupAttachments)
		{
			_packFileName = packFileName;
			_schemaInfo = schemaInfo;
			_dataDirPath = dataDirPath;
			_backupAttachments = backupAttachments;
			_metaDataDir = @".\" + PackData.META_DATA_DIR;
			_classDataDir = @".\" + PackData.CLASS_DATA_DIR;
			_attachmentDir = @".\" + PackData.ATTACHEMENTS_DIR;
            _pivotLayoutDir = @".\" + PackData.PIVOT_LAYOUTS_DIR;
			_metaData = null;
			_isCancelled = false;
			_workingDialog = null;
            _imageFileNames = new StringCollection();
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
		/// Pack data of the given schema into a zipped file.
		/// </summary>
		public void Pack()
		{
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

				// set _dataDirPath as the current working dir
				Directory.SetCurrentDirectory(_dataDirPath);

				if (!Directory.Exists(_metaDataDir))
				{
					Directory.CreateDirectory(_metaDataDir);
				}

				if (!Directory.Exists(_classDataDir))
				{
					Directory.CreateDirectory(_classDataDir);
				}

				if (!Directory.Exists(_attachmentDir))
				{
					Directory.CreateDirectory(_attachmentDir);
				}

                if (!Directory.Exists(_pivotLayoutDir))
                {
                    Directory.CreateDirectory(_pivotLayoutDir);
                }

				// output the meta data
				WriteMetaData();

				if (IsCancelled)
				{
					return;
				}

				// output class data
				WriteClassData();

				if (IsCancelled)
				{
					return;
				}

                // output image files
                WriteImageFiles();

                if (IsCancelled)
                {
                    return;
                }

				// output instance attachments
				WriteInstancesAttachments();

				if (IsCancelled)
				{
					return;
				}

				// output class attachments
				WriteClassAttachments();

				if (IsCancelled)
				{
					return;
				}

				// create the zipped file
				PackFiles();
			}
			finally
			{
				Directory.SetCurrentDirectory(workingDir);

				// cleanup data dir
				if (Directory.Exists(_dataDirPath))
				{
					Directory.Delete(_dataDirPath, true);
				}

				_workingDialog.DisplayText = msg;
			}
		}

		/// <summary>
		/// Write the meta data files to a directory
		/// </summary>
		private void WriteMetaData()
		{
			WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.BackupMetaData");

			MetaDataServiceStub metaDataService;
			metaDataService = new MetaDataServiceStub();

			// Create an meta data object to hold the meta data model
			_metaData = new MetaDataModel();
			_metaData.SchemaInfo = _schemaInfo;

			// create a MetaDataModel instance from the xml strings retrieved from the database
			string[] xmlStrings = metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(_schemaInfo));

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

            // read the xml string for logging policy
            reader = new StringReader(xmlStrings[8]);
            _metaData.LoggingPolicy.Read(reader);

            // read the xml string for subscribers
            reader = new StringReader(xmlStrings[9]);
            _metaData.SubscriberManager.Read(reader);

            // read the xml string for xml schema views
            reader = new StringReader(xmlStrings[10]);
            _metaData.XMLSchemaViews.Read(reader);

            // read the xml string for apis
            reader = new StringReader(xmlStrings[11]);
            _metaData.ApiManager.Read(reader);

            if (IsCancelled)
			{
				return;
			}

			// write the meta data into files
			string schemaFileName = _metaDataDir + @"\" + _schemaInfo.Name + ".schema";

			_metaData.SchemaModel.Write(schemaFileName);
			string xaclPolicyFileName = schemaFileName.Replace(".schema", ".xacl");
			_metaData.XaclPolicy.Write(xaclPolicyFileName);
			string taxonomyFileName = schemaFileName.Replace(".schema", ".taxonomy");
			_metaData.Taxonomies.Write(taxonomyFileName);
			string dataViewFileName = schemaFileName.Replace(".schema", ".dataview");
			_metaData.DataViews.Write(dataViewFileName);
			string ruleFileName = schemaFileName.Replace(".schema", ".rules");
			_metaData.RuleManager.Write(ruleFileName);
			string mappingFileName = schemaFileName.Replace(".schema", ".mappings");
			_metaData.MappingManager.Write(mappingFileName);
			string selectorFileName = schemaFileName.Replace(".schema", ".selectors");
			_metaData.SelectorManager.Write(selectorFileName);
            string eventFileName = schemaFileName.Replace(".schema", ".events");
            _metaData.EventManager.Write(eventFileName);
            string loggingFileName = schemaFileName.Replace(".schema", ".logging");
            _metaData.LoggingPolicy.Write(loggingFileName);
            string subscriberFileName = schemaFileName.Replace(".schema", ".subscribers");
            _metaData.SubscriberManager.Write(subscriberFileName);
            string xmlSchemaViewFileName = schemaFileName.Replace(".schema", ".schemaviews");
            _metaData.XMLSchemaViews.Write(xmlSchemaViewFileName);
            string apiFileName = schemaFileName.Replace(".schema", ".apis");
            _metaData.ApiManager.Write(apiFileName);
        }

		/// <summary>
		/// Write instance data of all classes into files
		/// </summary>
		private void WriteClassData()
		{
			SchemaModelElementCollection classes = _metaData.GetBottomClasses();
			CMDataServiceStub dataService = new CMDataServiceStub();
			
			XmlDocument doc;

			foreach (ClassElement classElement in classes)
			{
				if (IsCancelled)
				{
					break;
				}

				WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.BackupClassData") + " " + classElement.Caption;
				
				// one class per file
				string fileName = _classDataDir + @"\" + classElement.Name + ".";

				// get query
				DataViewModel dataView = _metaData.GetDetailedDataView(classElement.Name);

				dataView.PageSize = PackData.PAGE_SIZE;

                // clear any sort clauses, since we want to backup the data instances in the order
                // of being created. This way is faster too.
                dataView.ClearSortBy(); 

				string query = dataView.SearchQuery;

				string queryId = dataService.BeginQuery(ConnectionStringBuilder.Instance.Create(_schemaInfo), query, PackData.PAGE_SIZE);
				int pageIndex = 0;
				XmlDocument xmlDoc;
				try
				{
					while (!IsCancelled)
					{
                        // invoke the web service synchronously
                        xmlDoc = (XmlDocument)dataService.GetNextResult(ConnectionStringBuilder.Instance.Create(_schemaInfo), queryId);

						if (xmlDoc == null)
						{
							break;
						}

						// create an XML document
						doc = new XmlDocument();
						doc.AppendChild(doc.ImportNode(xmlDoc.DocumentElement, true));
						doc.Save(fileName + pageIndex); // save the xml to a file
						pageIndex++;

                        // remember the image names so that we can back them up later
                        if (HasImageAttributes(classElement))
                        {
                            KeepImageNames(dataView, xmlDoc);
                        }
					}
				}
				finally
				{
					if (queryId != null)
					{
						dataService.EndQuery(queryId);
					}
				}
			}
		}

        /// <summary>
        /// Write image files
        /// </summary>
        private void WriteImageFiles()
        {
        }

		/// <summary>
		/// Write instances attachments to files in a directory
		/// </summary>
		private void WriteInstancesAttachments()
		{
		}

		/// <summary>
		/// Write class attachments to files in a directory
		/// </summary>
		private void WriteClassAttachments()
		{
		}

		/// <summary>
		/// Pack the files into a zipped file
		/// </summary>
		private void PackFiles()
		{
			ZipEntry entry;
			FileStream fs = null;
			ZipOutputStream s = null;

			try
			{
				WorkingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.PackFiles");

				s = new ZipOutputStream(File.Create(_packFileName));

				s.SetLevel(6); // 0 - store only to 9 - means best compression

				string[] subDirs = Directory.GetDirectories(".");

				for (int i = 0; i < subDirs.Length; i++)
				{
					if (IsCancelled)
					{
						break;
					}

					string[] filenames = Directory.GetFiles(subDirs[i]);
				
					foreach (string file in filenames) 
					{
						if (IsCancelled)
						{
							break;
						}

						fs = File.OpenRead(file);
					
						byte[] buffer = new byte[fs.Length];
						fs.Read(buffer, 0, buffer.Length);
						entry = new ZipEntry(file);
					
						entry.DateTime = DateTime.Now;
						entry.Size = fs.Length;
						fs.Close();
						fs = null;
					
						s.PutNextEntry(entry);
					
						s.Write(buffer, 0, buffer.Length);
					}
				}
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
				}

				if (s != null)
				{
					s.Finish();
					s.Close();
				}
			}
		}

        // return true if the class contains image attributes
        private bool HasImageAttributes(ClassElement classElement)
        {
            bool status = false;
            ClassElement currentClassElement = classElement;

            while (currentClassElement != null)
            {
                if (currentClassElement.ImageAttributes != null &&
                    currentClassElement.ImageAttributes.Count > 0)
                {
                    status = true;
                    break;
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            return status;
        }

        // Keep the images names in a collection
        private void KeepImageNames(DataViewModel dataView, XmlNode xmlNode)
        {
            DataSet ds = new DataSet();

            XmlReader xmlReader = new XmlNodeReader(xmlNode);
            ds.ReadXml(xmlReader);

            if (ds.Tables[dataView.BaseClass.Name] != null)
            {
                InstanceView instanceView = new InstanceView(dataView, ds);
                int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;
                PropertyDescriptorCollection propertyDescriptors = instanceView.GetProperties(null);
                for (int i = 0; i < count; i++)
                {
                    instanceView.SelectedIndex = i;
                    foreach (InstanceAttributePropertyDescriptor pd in propertyDescriptors)
                    {
                        if (pd.IsImage)
                        {
                            object val = pd.GetValue();
                            if (val != null && ((string)val).Length > 0)
                            {
                                // keep the image id
                                _imageFileNames.Add((string)val);
                            }
                        }
                    }
                }
            }
        }
    }
}