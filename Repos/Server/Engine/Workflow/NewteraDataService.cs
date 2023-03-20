/*
* @(#)NewteraDataService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Workflow.Runtime;
using System.Threading;
using System.Security.Principal;
using System.Net.Mail;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.Server.DB;
using Newtera.Server.Engine.Cache;
using Newtera.WFModel;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// Provide the service for workflow task management based on Newtera Database.
    /// </summary>
    public class NewteraDataService : IDataService, IBindingInstanceService
    {       
        private IPrincipal _superUser = null;
        private Dictionary<Guid, IInstanceWrapper> _wappers;

        public NewteraDataService()
        {
            CMUserManager userMgr = new CMUserManager();
            _superUser = userMgr.SuperUser;
            _wappers = new Dictionary<Guid, IInstanceWrapper>();
        }

        #region IDataService

        /// <summary>
        /// Execute a search query.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="statement">The query statement</param>
        /// <returns>The search result in dataset</returns>
        public DataSet ExecuteQuery(string schemaId, string className, string statement)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(statement);

                XmlReader xmlReader = new XmlNodeReader(doc);
                DataSet ds = new DataSet();
                ds.ReadXml(xmlReader);

                return ds;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="statement">The query statement</param>
        /// <returns>The obj_id</returns>
        public string ExecuteNonQuery(string schemaId, string className, string statement)
        {
            return ExecuteNonQuery(schemaId, className, statement, true);
        }

        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="statement">The query statement</param>
        /// <param name="needToRaiseEvents">true to raise db events, false otherwise</param>
        /// <returns>The obj_id</returns>
        public string ExecuteNonQuery(string schemaId, string className, string statement, bool needToRaiseEvents)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                interpreter.NeedToRaiseEvents = needToRaiseEvents;
                XmlDocument doc = interpreter.Query(statement);

                if (doc.DocumentElement.InnerText != null)
                {
                    return doc.DocumentElement.InnerText;
                }
                else
                {
                    return "";
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Get an InstanceView from DB given a primary key
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="pk">The primary key of the instance</param>
        /// <returns>The obj_id of the affected data instance</returns>
        public InstanceView GetInstanceView(string schemaId, string className, string pk)
        {
            InstanceView instanceView = null;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                IDataProvider dataProvider = DataProviderFactory.Instance.Create();

                SchemaInfo[] schemaInfos = MetaDataCache.Instance.GetSchemaInfos(dataProvider);
                SchemaInfo theSchemaInfo = null;
                foreach (SchemaInfo schemaInfo in schemaInfos)
                {
                    if (schemaInfo.NameAndVersion == schemaId)
                    {
                        theSchemaInfo = schemaInfo;
                        break;
                    }
                }

                if (theSchemaInfo == null)
                {
                    throw new InvalidDataException("The schema " + schemaId + " doesn't exist in the database anymore.");
                }

                // build a query for getting the binding instance
                MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(theSchemaInfo, dataProvider);
                DataViewModel dataView = metaData.GetDetailedDataView(className);
                if (dataView == null)
                {
                    throw new InvalidDataException("The class " + className + " doesn't exist in the schema " + schemaId + " anymore.");
                }

                if (!string.IsNullOrEmpty(pk))
                {
                    // get the instance by primary key
                    foreach (IDataViewElement element in dataView.ResultAttributes)
                    {
                        if (element is DataSimpleAttribute)
                        {
                            DataSimpleAttribute attribute = (DataSimpleAttribute)element;
                            SimpleAttributeElement schemaModelElement = (SimpleAttributeElement)attribute.GetSchemaModelElement();
                            if (schemaModelElement.IsPrimaryKey)
                            {
                                // Assume the class has only one primary key attribute
                                attribute.AttributeValue = pk; // TODO, deal with multiple pk keys
                                break;
                            }
                        }
                    }

                    string query = dataView.GetInstanceByPKQuery();
                    Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                    XmlDocument doc = interpreter.Query(query);
                    XmlReader reader = new XmlNodeReader(doc);
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (!DataSetHelper.IsEmptyDataSet(ds, className))
                    {
                        // Create an instance view
                        instanceView = new InstanceView(dataView, ds);
                    }
                }
                else
                {
                    instanceView = new InstanceView(dataView);
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            return instanceView;
        }

        #endregion

        #region IBindingInstanceService

        /// <summary>
        /// Gets the binding data instance of a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        /// <returns>IInstanceWrapper object</returns>
        public IInstanceWrapper GetBindingInstance(Guid workflowInstanceId)
        {
            IInstanceWrapper wrapped = null;

            if (!_wappers.ContainsKey(workflowInstanceId))
            {

                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();

                    WorkflowInstanceBindingInfo binding = workflowModelAdapter.GetBindingInfoByWorkflowInstanceId(workflowInstanceId.ToString());

                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    IDataProvider dataProvider = DataProviderFactory.Instance.Create();

                    SchemaInfo[] schemaInfos = MetaDataCache.Instance.GetSchemaInfos(dataProvider);
                    SchemaInfo theSchemaInfo = null;
                    foreach (SchemaInfo schemaInfo in schemaInfos)
                    {
                        if (schemaInfo.NameAndVersion == binding.SchemaId)
                        {
                            theSchemaInfo = schemaInfo;
                            break;
                        }
                    }

                    if (theSchemaInfo == null)
                    {
                        throw new InvalidDataException("The schema " + binding.SchemaId + " doesn't exist in the database anymore.");
                    }

                    // build a query for getting the binding instance
                    MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(theSchemaInfo, dataProvider);
                    DataViewModel dataView = metaData.GetDetailedDataView(binding.DataClassName);
                    if (dataView == null)
                    {
                        throw new InvalidDataException("The class " + binding.DataClassName + " doesn't exist in the schema " + binding.SchemaId + " anymore.");
                    }

                    // get the instance
                    string query = dataView.GetInstanceQuery(binding.DataInstanceId);
                    Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                    XmlDocument doc = interpreter.Query(query);
                    XmlReader reader = new XmlNodeReader(doc);
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (DataSetHelper.IsEmptyDataSet(ds, binding.DataClassName))
                    {
                        throw new InvalidDataException("The data instance with id " + binding.DataInstanceId + " doesn't exist. It may be deleted.");
                    }

                    // Create an instance view
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    wrapped = new InstanceWrapper(instanceView);

                    // keep the instance in a cache
                    _wappers.Add(workflowInstanceId, wrapped);
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
            else
            {
                wrapped = _wappers[workflowInstanceId];
            }

            return wrapped;
        }

        /// <summary>
        /// Save the channges of the wrapped binding data instance to database.
        /// </summary>
        public void SaveBindingInstance(Guid workflowInstanceId)
        {
            if (this._wappers.ContainsKey(workflowInstanceId))
            {
                IInstanceWrapper wrapper = this._wappers[workflowInstanceId];
                wrapper.Save();
            }
        }

        /// <summary>
        /// Clear the cached binding instance for a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void ClearBindingInstance(Guid workflowInstanceId)
        {
            if (this._wappers.ContainsKey(workflowInstanceId))
            {
                this._wappers.Remove(workflowInstanceId);
            }
        }

        #endregion
    }
}
