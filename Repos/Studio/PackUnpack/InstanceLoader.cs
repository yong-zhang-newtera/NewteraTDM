/*
* @(#)InstanceLoader.cs
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
	using System.Data;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.QueryBuilder;
    using Newtera.WindowsControl;
    using Newtera.WinClientCommon;

	/// <summary>
	/// Load instances from class data files into the database.
	/// It takes two phases to load instances. The phase one, insert
	/// instances without worrying about relationships between instances.
	/// Phase two, updating the inserted instances with relationships.
	/// </summary>
	/// <version> 1.0.0 24 Apr 2005 </version>
	/// <author> Yong Zhang</author>
	public class InstanceLoader
	{
		private MetaDataModel _metaData;
		private string _classDataDir;
		private CMDataServiceStub _dataService;
		private Hashtable _objIdMappings;
        private StreamWriter _errorLog;
		private WorkInProgressDialog _workingDialog;

		/// <summary>
		/// Instantiate an instance of InstanceLoader class
		/// </summary>
		/// <param name="metaData">The Meta Data Model</param>
		/// <param name="classDataDir">The directory where the class data files reside.</param>
		public InstanceLoader(MetaDataModel metaData, string classDataDir,
			WorkInProgressDialog workingDialog)
		{
			_classDataDir = classDataDir;
			_metaData = metaData;
			_objIdMappings = new Hashtable();
			_workingDialog = workingDialog;
			_dataService = new CMDataServiceStub();
            _errorLog = null;
		}

		/// <summary>
		/// Gets the mapping table that maps an old obj_id to a new obj_id
		/// </summary>
		public Hashtable ObjIdMappings
		{
			get
			{
				return _objIdMappings;
			}
		}

        /// <summary>
        /// Gets or sets the error log stream
        /// </summary>
        public StreamWriter ErrorLog
        {
            get
            {
                return _errorLog;
            }
            set
            {
                _errorLog = value;
            }
        }

		/// <summary>
		/// Load instances from class data files into the database
		/// </summary>
		/// <remarks>It takes two phases to load instances. The phase one, insert
		/// instances without worrying about relationships between instances.
		/// Loading instances is the phase one.</remarks>
		public void LoadInstances()
		{
			// phase one, insert instances into their corresponding classes.
			InsertInstances();
		}

		/// <summary>
		/// Insert instances into their corresponding classes.
		/// </summary>
		private void InsertInstances()
		{
            if (Directory.Exists(_classDataDir))
            {
                string[] filenames = Directory.GetFiles(_classDataDir);
                string className;
                string query;
                string oldObjId, newObjId;

                foreach (string file in filenames)
                {
                    className = file;
                    int pos = className.LastIndexOf(@"\");
                    if (pos > 0 && pos < className.Length)
                    {
                        className = file.Substring(pos + 1);
                    }

                    pos = className.IndexOf(".");
                    if (pos > 0)
                    {
                        className = className.Substring(0, pos);
                    }

                    ClassElement classElement = this._metaData.SchemaModel.FindClass(className);
                    if (classElement == null)
                    {
                        throw new Exception("Class " + className + " does not exist in the meta data " + this._metaData.SchemaInfo.Name);
                    }

                    _workingDialog.DisplayText = MessageResourceManager.GetString("DesignStudio.LoadInstances") + " " + classElement.Caption;

                    DataViewModel dataView = _metaData.GetDetailedDataView(className);

                    // convert xml data into a DataSet
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);

                    InstanceView instanceView = new InstanceView(dataView, ds);

                    int numberOfInstances = 0;

                    if (ds.Tables[className] != null)
                    {
                        numberOfInstances = ds.Tables[className].Rows.Count;
                    }

                    for (int i = 0; i < numberOfInstances; i++)
                    {
                        instanceView.SelectedIndex = i;

                        oldObjId = instanceView.InstanceData.ObjId;

                        query = instanceView.DataView.InsertQuery;

                        try
                        {
                            // execute insert query
                            newObjId = _dataService.ExecuteUpdateQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                                query, false);

                            ObjIdMappings[oldObjId] = newObjId;
                        }
                        catch (Exception ex)
                        {
                            if (_errorLog != null)
                            {
                                _errorLog.WriteLine("************ Message Begin ***********");
                                _errorLog.WriteLine(ex.Message);
                                _errorLog.WriteLine("************ Message End *************");
                            }
                        }
                    }
                }
            }
		}

		/// <summary>
		/// Updating the relationships among the previously loaded instances in the
		/// database.
		/// </summary>
		public void UpdateRelationships()
		{
            if (Directory.Exists(_classDataDir))
            {
                string[] filenames = Directory.GetFiles(_classDataDir);
                string className;
                string query;
                string objId;

                foreach (string file in filenames)
                {
                    className = file;
                    int pos = className.LastIndexOf(@"\");
                    if (pos > 0 && pos < className.Length)
                    {
                        className = file.Substring(pos + 1);
                    }

                    pos = className.IndexOf(".");
                    if (pos > 0)
                    {
                        className = className.Substring(0, pos);
                    }

                    DataViewModel dataView = _metaData.GetDetailedDataView(className);

                    // convert xml data into a DataSet
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);

                    InstanceView instanceView = new InstanceView(dataView, ds);

                    int numberOfInstances = 0;

                    if (ds.Tables[className] != null)
                    {
                        numberOfInstances = ds.Tables[className].Rows.Count;
                    }

                    for (int i = 0; i < numberOfInstances; i++)
                    {
                        instanceView.SelectedIndex = i;

                        query = GetUpdateRelationshipsQuery(instanceView);

                        // execute update query
                        if (query != null)
                        {
                            try
                            {
                                objId = _dataService.ExecuteUpdateQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                                    query, false);
                            }
                            catch (Exception ex)
                            {
                                if (_errorLog != null)
                                {
                                    _errorLog.WriteLine("************ Message Begin ***********");
                                    _errorLog.WriteLine(ex.Message);
                                    _errorLog.WriteLine("************ Message End *************");
                                }
                            }
                        }
                    }
                }
            }
		}

		/// <summary>
		/// Generate a query that updates values of relationship attributes of
		/// an instance.
		/// </summary>
		/// <param name="instanceView">The InstanceView object</param>
		/// <returns>The update query string.</returns>
		private string GetUpdateRelationshipsQuery(InstanceView instanceView)
		{
			string query = null;
			string oldObjId, newObjId;
			bool needUpdate = false;

			// replace the old obj_id of the instance with the new obj_id assigned
			// after the instance is loaded
			oldObjId = instanceView.InstanceData.ObjId;
			newObjId = (string) _objIdMappings[oldObjId];
			instanceView.InstanceData.ObjId = newObjId;

			// for each forward relationship attribute of the instance, replace the
			// old obj_id of referenced instance with the new obj_id of referenced
			// instance.
			foreach (InstanceAttributePropertyDescriptor pd in instanceView.GetProperties(null))
			{
				if (pd.IsRelationship)
				{
					DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute) pd.DataViewElement;
					
					if (relationshipAttr.PrimaryKeyCount > 0)
					{
						// this is a many-to-one or one-to-one relationship, therefore,
						// it has a foreign key column that needs to be updated
						// replace the referenced obj_id
						oldObjId = instanceView.InstanceData.GetReferencedObjID(relationshipAttr.Name);
						if (oldObjId != null && oldObjId.Length > 0)
						{
							newObjId = (string) _objIdMappings[oldObjId];
							instanceView.InstanceData.SetReferencedObjID(relationshipAttr.Name, newObjId);
							needUpdate = true;
						}
					}
				}
			}

			if (needUpdate)
			{
				query = instanceView.DataView.GetReferenceUpdateQuery(instanceView.InstanceData);
			}
			
			return query;
		}

        /// <summary>
        /// Update the data instance ids stored in WF_INSTANCE_MAP
        /// </summary>
        public void UpdateWFBindingInstanceIds()
        {
            WorkflowTrackingServiceStub wfTrackingService = new WorkflowTrackingServiceStub();
            int pageSize = 50;
            int pageIndex = 0;
            string newInstanceId;
            string[] dataInstanceIds;

            dataInstanceIds = wfTrackingService.GetBindingDataInstanceIds(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                _metaData.SchemaInfo.NameAndVersion,               
                pageSize, pageIndex);

            // run unill it reaches the end
            while (dataInstanceIds != null)
            {
                // update the data instance id
                foreach (string oldInstanceId in dataInstanceIds)
                {
                    newInstanceId = (string)_objIdMappings[oldInstanceId];

                    if (!string.IsNullOrEmpty(newInstanceId))
                    {
                        wfTrackingService.ReplaceBindingDataInstanceId(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                            oldInstanceId, newInstanceId);
                    }
                }

                pageIndex++;

                dataInstanceIds = wfTrackingService.GetBindingDataInstanceIds(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    _metaData.SchemaInfo.NameAndVersion,
                    pageSize, pageIndex);
            }
        }
	}
}