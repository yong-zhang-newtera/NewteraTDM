/*
* @(#)Executor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Data;

	using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom.Common;
    using Newtera.Server.Engine.Cache;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Workflow;
    using Newtera.Server.FullText;
	using Newtera.Server.DB;

	/// <summary> 
	/// Provides common functions for all executor subclasses
	/// </summary>
	/// <version>  	1.0.1 18 Jul 2003</version>
	/// <author> Yong Zhang</author>
	abstract public class Executor
	{
        private static string VARIABLE_NAME = "$this/"; // The variable name at beginning of a xpath	
        
		protected SQLBuilder _builder; // the sqlbuilder
		protected IDataProvider _dataProvider; // The DB provider
		protected MetaDataModel _metaData;
        protected List<EventContext> _eventContexts;
        protected List<IndexingContext> _indexingContexts;
        protected bool _needToRaiseEvents;
        protected bool _checkReadPermissionOnly = false;
        protected bool _showEncryptedData = false;
        protected bool _obtainCachedObjId = false;
        protected string _cachedSQL = null;
		
		/// <summary>
		/// Initiating an Executor
		/// </summary>
		/// <param name="schemaModel">The schema model</param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder</param>
		public Executor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder)
		{
			_metaData = metaData;
			_dataProvider = dataProvider;
			_builder = builder;
            _eventContexts = new List<EventContext>();
            _indexingContexts = new List<IndexingContext>(); // db events for modifying a full-text search index
            _needToRaiseEvents = true;
            _cachedSQL = null;
        }

        /// <summary>
        /// Gets or sets the information indicates whether to raise events when insert,
        /// update, or delete instances
        /// </summary>
        public bool NeedToRaiseEvents
        {
            get
            {
                return _needToRaiseEvents;
            }
            set
            {
                _needToRaiseEvents = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to check permissions other than read
        /// </summary>
        /// <value>True to check read permission only. false to check all permissions. default is false</value>
        public bool CheckReadPermissionOnly
        {
            get
            {
                return _checkReadPermissionOnly;
            }
            set
            {
                _checkReadPermissionOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to show encrypted data in the query result
        /// </summary>
        /// <value>True to show encrypted data in the result. false otherwise, default is false</value>
        public bool ShowEncryptedData
        {
            get
            {
                return _showEncryptedData;
            }
            set
            {
                _showEncryptedData = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to obtain cached objId for a primary key
        /// </summary>
        /// <value>True to obtain a cached objId, false to get objId from database. default is false</value>
        public bool ObtainCachedObjId
        {
            get
            {
                return _obtainCachedObjId;
            }
            set
            {
                _obtainCachedObjId = value;
            }
        }

        /// <summary> 
        /// Gets or sets a cached sql for the xquery being processed
        /// </summary>
        /// <returns>a sql statement or null</returns>
        public string CachedSQL
        {
            get
            {
                return _cachedSQL;
            }
            set
            {
                _cachedSQL = value;
            }
        }

        /// <summary>
        /// Add the database events to the event processing queue and post an event
        /// </summary>
        public void RaiseEvents()
        {
            foreach (EventContext eventContext in _eventContexts)
            {
                EventContext ec = eventContext;
                DBEventQueueManager.Instance.PostEvent(ec);
            }

            foreach (IndexingContext indexingContext in _indexingContexts)
            {
                IndexEventManager.Instance.PostEvent(indexingContext);
            }
        }

        /// <summary>
        /// This is a hack to clear the server cache when UserInfo's UserRole class has changes,
        /// such as insert, update, or delete
        /// </summary>
        protected void ClearServerCache(string className)
        {
            if (className == "UserRole")
            {
                UserDataCache.Instance.ClearUserAndRoleCaches();
            }
            else if (className == "User")
            {
                UserDataCache.Instance.ClearUserDataCaches();
            }
            else if (className == "Role")
            {
                UserDataCache.Instance.ClearRoleDataCaches();
            }
        }

		/// <summary>
		/// Create a full-brown class entity for the given parent element.
		/// </summary>
		/// <param name="instance">the instance data that provides class name in its type attribute.</param>
		/// <returns> the base class entity</returns>
		protected ClassEntity CreateFullBlownClass(Instance instance)
		{
			ClassEntity baseClass;
			string className = instance.ClsName;
			
			if (className == null)
			{
				throw new MissingClassTypeException("Missing type attribute in an instance");
			}
						
			ClassElement element = (ClassElement) _metaData.SchemaModel.FindClass(className);
			if (element == null)
			{
				// something wrong
				throw new UnknownClassNameException("Unknow class " + className);
			}
			
			baseClass = new ClassEntity(element);
			baseClass.MakeFullBlown();
			
			return baseClass;
		}
		
		/// <summary>
		/// Retrieve the id(s) of referenced object(s) based on the key data in
		/// the instance.
		/// </summary>
		/// <param name="baseClass">the base class to which to insert the instance.</param>
		/// <param name="instance">the instance data.</param>
		/// <param name="isForInsert">whether it is called by InsertExecutor.</param>
		protected void SetReferencedIds(ClassEntity baseClass, Instance instance, bool isForInsert, VDocument doc)
		{
			/*
			* First to check if the id(s) of referenced object exist. It they don't
			* exist, retrieve them using the values of primary key(s) in the instance
			*/
			DBEntityCollection relationships = baseClass.InheritedRelationships;
			if (relationships != null)
			{
				for (int i = 0; i < relationships.Count; i++)
				{
					RelationshipEntity relationship = (RelationshipEntity) relationships[i];
					
					// do this only for the forward relationship and the object id doesn't exist
					if (relationship.Direction == RelationshipDirection.Forward)
					{
						
						string objId = instance.GetReferencedObjId(relationship.Name);
                        if (objId == null)
						{
							objId = RetrieveReferencedId(relationship, instance, doc);

                            if (objId != null)
                            {
                                instance.SetReferencedObjId(relationship.Name, objId);
                            }
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Retrieve the id of a referenced object.
		/// </summary>
		/// <param name="relationship">the relationship attribute for the referenced object.</param>
		/// <param name="instance">the instance data to get primary key value(s).</param>
		private string RetrieveReferencedId(RelationshipEntity relationship, Instance instance, VDocument doc)
		{
			string referencedObjId = null;
			
			// first check if the relationship value is null
			if (instance.IsNullValue(relationship.Name))
			{
				referencedObjId = SQLElement.VALUE_NULL;
			}
            else if (instance.HasPrimaryKeyValues(relationship.Name))
            {
                // get the primary key value from instance and use them to retrieve the object id 		

                // build a SQL for retrieving the id of referenced object
                ClassEntity referencedClass = new ClassEntity(relationship.LinkedClass.SchemaElement);
                // the referenced class may have inherited classes, create them
                referencedClass.CreateEmptyAncestorClasses();

                DBEntityCollection keys = referencedClass.CreatePrimaryKeys();
                if (keys.Count == 0)
                {
                    throw new NoPrimaryKeyException("Class " + referencedClass.Name + " dose not have any primary keys.");
                }

                if (keys.Count == 1 && ((AttributeEntity)keys[0]).IsAutoIncrement)
                {
                    string keyValue = instance.GetPrimaryKeyValue(relationship.Name, keys[0].Name);

                    if (ObtainCachedObjId && keyValue != null)
                    {
                        // see if there is an obj id of the referenced instance existed. this happens
                        // when the application (import data converter) generats a id for key of the instance but the database
                        // generates a different id to replace it, we have to use the app generated
                        // id to get the referenced instance obj id
                        referencedObjId = (string)QueryDataCache.Instance.GetSessionObject(keyValue);
                    }
                }

                if (referencedObjId == null)
                {
                    foreach (AttributeEntity key in keys)
                    {
                        string keyValue = instance.GetPrimaryKeyValue(relationship.Name, key.Name);
                        if (keyValue == null)
                        {
                            throw new MissingPrimaryKeyValueException("Missing primary key value for key " + key.Name);
                        }

                        key.SearchValue = keyValue;
                    }

                    IDbConnection con;
                    if (doc.Interpreter.HasGlobalTransaction)
                    {
                        con = doc.Interpreter.IDbConnection;
                    }
                    else
                    {
                        con = _dataProvider.Connection;
                    }
                    IDataReader dataReader = null;
                    string sql = "";
                    try
                    {
                        sql = _builder.GenerateSelect(referencedClass);

                        IDbCommand cmd = con.CreateCommand();
                        if (doc.Interpreter.HasGlobalTransaction)
                        {
                            cmd.Transaction = doc.Interpreter.IDbTransaction;
                        }
                        cmd.CommandText = sql;
                        dataReader = cmd.ExecuteReader();

                        if (!dataReader.Read())
                        {
                            throw new NonexistReferencedObjException("Unable to find the referenced instances for " + relationship.Name);
                        }

                        referencedObjId = System.Convert.ToString(dataReader.GetValue(referencedClass.ObjIdEntity.ColumnIndex - 1));

                        // only one row is expected
                        if (dataReader.Read())
                        {
                            throw new MultiReferencedObjException("More than one instances have the same primary key value(s) in the class " + relationship.LinkedClass.SchemaElement.Caption);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + "; The SQL:" + sql);
                    }
                    finally
                    {
                        if (dataReader != null && !dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }

                        if (!doc.Interpreter.HasGlobalTransaction)
                        {
                            con.Close();
                        }
                    }
                }
            }
			
			return referencedObjId;
		}
		
		/// <summary>
		/// Find the instance with the given obj_id as the current instance from a document.
		/// </summary>
		/// <param name="doc">the document that may contains the instance.</param>
		/// <param name="the">obj_id of the instance</param>
		/// <returns> the found instance element</returns>
		protected XmlElement FindInstance(XmlDocument doc, string objId)
		{
			XmlElement theInstance = null;
            XmlNodeList classLists = doc.DocumentElement.ChildNodes;
            XmlNodeList instances;
            foreach (XmlElement classList in classLists)
            {
                instances = classList.ChildNodes;
                foreach (XmlElement instance in instances)
                {
                    if (instance.GetAttribute(SQLElement.OBJ_ID) == objId)
                    {
                        theInstance = instance;
                        break;
                    }
                }

                if (theInstance != null)
                {
                    break;
                }
            }
			
			return theInstance;
		}

        /// <summary>
        /// Build an xquery to retrieve the instance given obj_id and leaf class name.
        /// </summary>
        /// <param name="objId">The instance id</param>
        /// <param name="baseClassName">the base class name</param>
        /// <param name="doc">The document that owns the instance element</param>
        /// <param name="conclusion">The conclusion of the permission check on the instance</param>
        /// <returns> the xquery</returns>
        protected string BuildInstanceQuery(string objId, string baseClassName, VDocument doc, Conclusion conclusion)
        {
            DataViewModel dataView = _metaData.GetDetailedDataView(baseClassName);

            return dataView.GetInstanceQuery(objId);

            /*
            System.Text.StringBuilder query = new System.Text.StringBuilder();

            // Constructing a XQuery for retrieving the instance
            query.Append("document(\"").Append(doc.URL).Append("\")//");

            // Constructing part of XPath, e.g. CustomerList/Customer
            query.Append(baseClassName).Append(SQLElement.ELEMENT_CLASS_NAME_SUFFIX).Append("/");
            query.Append(baseClassName).Append("[");

            string objIdExpression = "@obj_id=\"" + objId + "\"";
            query.Append(objIdExpression);

            string condition;
            System.Text.StringBuilder queryCondition = new System.Text.StringBuilder();

            // add rule condition as part of the instance query so that the related instances that could
            // be referred by rule's condition will be loaded as part of return document, which is
            // required by running the condition checker of access control processor.
            foreach (Decision decision in conclusion.DecisionList)
            {
                if (decision.Rule.HasCondition)
                {
                    condition = decision.Rule.Condition.Condition;

                    // some condition may not have $this, do not add it as part of sql condition
                    if (ContainVariable(condition))
                    {
                        // combined objId expression with the rule condition using and operator
                        // so that the query will return an instance if the instance has the specified
                        // obj id and meets the rule's condition
                        queryCondition.Append(" or ").Append(objIdExpression).Append(" and ").Append(RemoveVariable(condition));
                    }
                }
            }

            if (queryCondition.Length > 0)
            {
                // rules have condition, append the additional condition to the instance query
                query.Append(queryCondition.ToString());
            }
            
            query.Append("]");

            return query.ToString();
             */

        }

        /// <summary>
        /// Gets the information indicating whether a condition contains a variable $this
        /// </summary>
        /// <param name="condition">The condition expression</param>
        /// <returns>true if it contains, false otherwise</returns>
        private bool ContainVariable(string condition)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(condition))
            {
                int pos = condition.IndexOf(VARIABLE_NAME);
                if (pos >= 0)
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Remove the variable ($this) from xpath strings appears in the condition.
        /// </summary>
        /// <param name="condition">the condition string. </param>
        /// <returns> the modified condition.</returns>
        private string RemoveVariable(string condition)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            int start = 0;
            int end = condition.IndexOf(VARIABLE_NAME, 0);
            while (end >= 0)
            {
                buf.Append(condition.Substring(start, (end) - (start)));
                start = end + VARIABLE_NAME.Length;
                end = condition.IndexOf(VARIABLE_NAME, start);
            }

            buf.Append(condition.Substring(start));

            return buf.ToString();
        }
	}
}