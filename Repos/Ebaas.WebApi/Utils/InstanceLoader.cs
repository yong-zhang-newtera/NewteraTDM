/*
* @(#)InstanceLoader.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Utils
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
    using Newtera.Data;

	/// <summary>
	/// Load instances from class data files into the database.
	/// It takes two phases to load instances. The phase one, insert
	/// instances without worrying about relationships between instances.
	/// Phase two, updating the inserted instances with relationships.
	/// </summary>
	/// <version> 1.0.0 11 Jan 2013 </version>
	public class InstanceLoader
	{
        CMConnection _connection;
		private MetaDataModel _metaData;
		private string _classDataDir;
		private Hashtable _objIdMappings;
        private bool _isOverride;

		/// <summary>
		/// Instantiate an instance of InstanceLoader class
		/// </summary>
		/// <param name="metaData">The Meta Data Model</param>
		/// <param name="classDataDir">The directory where the class data files reside.</param>
		public InstanceLoader(CMConnection connection, MetaDataModel metaData, string classDataDir, bool isOverride)
		{
            _connection = connection;
			_classDataDir = classDataDir;
			_metaData = metaData;
			_objIdMappings = new Hashtable();
            //_isOverride = isOverride;
            _isOverride = true;
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
		/// Load instances from class data files into the database
		/// </summary>
		/// <remarks>It takes two phases to load instances. The phase one, insert
		/// instances without worrying about relationships between instances.
		/// Loading instances is the phase one.</remarks>
		public void LoadInstances()
		{
            if (Directory.Exists(_classDataDir))
            {
                string[] filenames = Directory.GetFiles(_classDataDir);
                string className;
                string query;
                string oldObjId, newObjId;
                CMCommand cmd;
                XmlDocument doc;

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

                    ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(className);
                    if (classElement == null)
                    {
                        throw new Exception("Class " + className + " does not exist in the meta data " + _connection.MetaDataModel.SchemaInfo.Name);
                    }

                    DataViewModel dataView = _connection.MetaDataModel.GetDetailedDataView(className);

                    // convert xml data into a DataSet
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);

                    InstanceView instanceView = new InstanceView(dataView, ds);

                    int numberOfInstances = 0;

                    if (ds.Tables[className] != null)
                    {
                        numberOfInstances = ds.Tables[className].Rows.Count;
                    }

                    InstanceView duplicatedInstanceView;
                    int count;

                    for (int i = 0; i < numberOfInstances; i++)
                    {
                        instanceView.SelectedIndex = i;

                        oldObjId = instanceView.InstanceData.ObjId;
                        query = null;
                        newObjId = null;

                        string matchCondition = GetMatchConditionExpression(classElement);

                        if (!string.IsNullOrEmpty(matchCondition))
                        {
                            duplicatedInstanceView = GetDuplicatedInstances(instanceView, matchCondition, out count);
                            if (count >= 1)
                            {
                                if (_isOverride)
                                {
                                    // copy the value from the new instance to the duplicated instance
                                    duplicatedInstanceView.InstanceData.Copy(instanceView.InstanceData);
                                    if (duplicatedInstanceView.InstanceData.IsChanged)
                                    {
                                        query = duplicatedInstanceView.DataView.UpdateQuery;
                                    }
                                }
                                else
                                {
                                    // do not modify the duplicated instance
                                    query = null;
                                }

                                // reference to the existing obj id
                                newObjId = duplicatedInstanceView.InstanceData.ObjId;
                            }
                            else
                            {
                                // no duplicate instance exist, insert as a new instance
                                query = instanceView.DataView.InsertQuery;
                            }
                        }
                        else
                        {
                            // no duplicate instance exist, insert as a new instance
                            query = instanceView.DataView.InsertQuery;
                        }

                        if (query != null)
                        {
                            try
                            {
                                cmd = _connection.CreateCommand();
                                cmd.CommandText = query;

                                // execute insert or update query
                                doc = cmd.ExecuteXMLDoc();
                                newObjId = doc.DocumentElement.InnerText;

                                ObjIdMappings[oldObjId] = newObjId;
                            }
                            catch (Exception ex)
                            {
                                /*
                                if (_errorLog != null)
                                {
                                    _errorLog.WriteLine("************ Message Begin ***********");
                                    _errorLog.WriteLine(ex.Message);
                                    _errorLog.WriteLine("************ Message End *************");
                                }
                                 */
                            }
                        }
                        else if (newObjId != null)
                        {
                            // keep the objId for later reference
                            ObjIdMappings[oldObjId] = newObjId;
                        }
                    }
                }
            }
		}

        private string GetMatchConditionExpression(ClassElement classElement)
        {
            string matchCondition = null;
            ClassElement currentClassElement = classElement;

            while (currentClassElement != null)
            {
                if (!string.IsNullOrEmpty(currentClassElement.MatchCondition))
                {
                    matchCondition = currentClassElement.MatchCondition;
                    break;
                }

                // go up to the parent class
                currentClassElement = currentClassElement.ParentClass;
            }

            return matchCondition;
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
                CMCommand cmd;

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

                    DataViewModel dataView = _connection.MetaDataModel.GetDetailedDataView(className);

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
                                cmd = _connection.CreateCommand();
                                cmd.CommandText = query;

                                // execute update query
                                cmd.ExecuteXMLDoc();
                            }
                            catch (Exception ex)
                            {
                                /*
                                if (_errorLog != null)
                                {
                                    _errorLog.WriteLine("************ Message Begin ***********");
                                    _errorLog.WriteLine(ex.Message);
                                    _errorLog.WriteLine("************ Message End *************");
                                }
                                 */
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

        private InstanceView GetDuplicatedInstances(InstanceView instanceView, string matchCondition, out int count)
        {
            InstanceView duplicatedInstanceView = null;
            count = 0;

            // first substitude the variables in the condition with attribute values of the instance
            string substitudedCondition = matchCondition;

            string variable;
            foreach (InstanceAttributePropertyDescriptor ipd in instanceView.GetProperties(null))
            {
                variable = "{" + ipd.Name + "}";
                if (substitudedCondition.Contains(variable))
                {
                    string value = instanceView.InstanceData.GetAttributeStringValue(ipd.Name);
                    if (!string.IsNullOrEmpty(value))
                    {
                        // substitude the variable with value
                        substitudedCondition = substitudedCondition.Replace(variable, value);
                    }
                    else
                    {
                        // no value, replace the variable with zero
                        substitudedCondition = substitudedCondition.Replace(variable, "0");
                    }
                }
            }

            // Need a new dataview model for a brand new instance
            DataViewModel dataView = _connection.MetaDataModel.GetDetailedDataView(instanceView.DataView.BaseClass.Name);

            string query = dataView.SearchQuery;

            CMCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;
            // provide the matching condition as an extra condition to the command, it will be added to the regular query
            cmd.ExtraCondition = substitudedCondition;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);

            duplicatedInstanceView = new InstanceView(dataView, ds);

            return duplicatedInstanceView;
        }
	}
}