/*
* @(#) WorkflowModelAdapter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Data;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Workflow.Runtime.Tracking;
    using System.Workflow.Runtime;
    using System.Workflow.ComponentModel;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Server.Engine.Cache;
    using Newtera.WFModel;
    using Newtera.Server.Engine.Workflow;
    using Newtera.Common.MetaData.Events;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// Purpose of this adapter is to isoloate a specific data source from the rest of code
    /// where the workflow model data is stored.
	/// </summary>
	/// <version> 	1.0.0	14 Dec 2006 </version>
	public class WorkflowModelAdapter
	{
        /* TABLE and COLUMN Names */
        public const string WF_PROJECT = "WF_PROJECT";
        public const string XML_DATA = "XML";
        public const string XACL_DATA = "XACL";
        public const string LOG = "UPDATELOG";
        public const string WF_WORKFLOW = "WF_WORKFLOW";
        public const string XOML = "XOML";
        public const string RULES = "RULES";
        public const string LAYOUT = "LAYOUT";
        public const string CODE = "CODE";
        public const string WF_INSTANCE_STATE = "WF_INSTANCE_STATE";
        public const string WF_INSTANCE_STATE_COLUMN = "STATE";
        public const string WF_INSTANCE_ID = "INSTANCEID";
        public const string WF_COMPLETED_SCOPE = "WF_COMPLETED_SCOPE";
        public const string WF_SCOPE_STATE_COLUMN = "STATE";
        public const string WF_SCOPE_ID = "CompletedScopeID";
        public const string WF_TASK_SUBSTITUTE = "WF_TASK_SUBSTITUTE";
        public const string WF_TASK_SUBSTITUTE_ID = "1";// only one record in WF_TASK_SUBSTITUTE table

		private IDataProvider _dataProvider;

		/// <summary>
		/// Intiating an instance of WorkflowModelAdapter class.
		/// </summary>
		public WorkflowModelAdapter()
		{
            _dataProvider = DataProviderFactory.Instance.Create();
		}

		/// <summary>
		/// Intiating an instance of WorkflowModelAdapter class.
		/// </summary>
		/// <param name="dataProvider">The data provider.</param>
		public WorkflowModelAdapter(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets project infos of the workflow projects stored in database.
		/// </summary>
		/// <returns>
		/// An ProjectInfoCollection instance
		/// </returns>
        public ProjectInfoCollection GetProjectInfos()
		{
            ProjectInfoCollection projectInfoCollection = new ProjectInfoCollection();
            ProjectInfoCollection missingTimestampProjectInfos = new ProjectInfoCollection();

			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetProjectInfos");
			
			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

                ProjectInfo projectInfo;
				while (reader.Read())
				{
                    projectInfo = new ProjectInfo();
                    projectInfo.ID = System.Convert.ToString(reader[("ID")]);
                    projectInfo.Name = System.Convert.ToString(reader[("NAME")]);
                    if (!reader.IsDBNull(2))
                    {
                        // default version is 1.0
                        projectInfo.Version = System.Convert.ToString(reader[("VERSION")]);
                    }
                    projectInfo.Description = System.Convert.ToString(reader[("DESCRIPTION")]);
                    if (!reader.IsDBNull(4))
                    {
                        projectInfo.ModifiedTime = DateTime.Parse(reader["MODIFIED_TIME"].ToString());
                    }
                    else
                    {
                        // this is the project info saved using 3.1.0 or earlier,
                        // write the current time as the latest modified time later
                        projectInfo.ModifiedTime = DateTime.Now; // default
                        missingTimestampProjectInfos.Add(projectInfo);
                    }

                    projectInfoCollection.Add(projectInfo);
				}

                // write the timestamp back to the database for those that do not have
                // timestamps
                foreach (ProjectInfo pi in missingTimestampProjectInfos)
                {
                    SetModifiedTime(pi.Name, pi.Version, pi.ModifiedTime);

                    // for some reason, the value of DateTime.Now is less than what
                    // stored in the database. Therefore, we need to retrieave the modified time
                    // from database
                    GetModifiedTime(pi);
                }

                return projectInfoCollection;
			}
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

        /// <summary>
        /// Fill the project model with data stored in database
        /// </summary>
        /// <param name="projectId">The unique id of the project.</param>
        /// <param name="projectModel">A ProjectModel to fill with data</param>
        public void Fill(string projectId, ProjectModel projectModel)
        {
            // get a stream to read schema model from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                projectModel.Read(clobDAO.ReadClob(WF_PROJECT, XML_DATA, projectId));
            }

            clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                projectModel.Policy.Read(clobDAO.ReadClob(WF_PROJECT, XACL_DATA, projectId));
            }
        }

        /// <summary>
        /// Write a project model as xml string to the corresponding clobs.
        /// </summary>
        /// <param name="projectModel">The project model to be written</param>
        public void WriteProjectModel(ProjectModel projectModel)
        {
            // write the project model
            string projectName = projectModel.Name;
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            projectModel.Write(writer);

            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(builder.ToString(), WF_PROJECT, XML_DATA, projectModel.ID);
            }

            // write the project access policy
            builder = new StringBuilder();
            writer = new StringWriter(builder);
            projectModel.Policy.Write(writer);

            // get the clob object
            clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(builder.ToString(), WF_PROJECT, XACL_DATA, projectModel.ID);
            }
        }

        /// <summary>
        /// Write a project update log to the database.
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <param name="log">The project update log text</param>
        public void WriteLog(string projectId, string log)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(log, WF_PROJECT, LOG, projectId);
            }
        }

        /// <summary>
        /// Fill the WorkflowDataCacheEntry with data stored in database
        /// </summary>
        /// <param name="workflowId">The unique id of workflow</param>
        /// <param name="entry">The cache entry of workflow data</param>
        public void FillWorkflowData(string workflowId, WorkflowDataCacheEntry entry)
        {
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            // use clob for one call only because it can not be reused
            using (clobDAO)
            {
                entry.Xoml = clobDAO.ReadClobAsText(WF_WORKFLOW, XOML, workflowId);
            }

            clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
            using (clobDAO)
            {
                entry.Rules = clobDAO.ReadClobAsText(WF_WORKFLOW, RULES, workflowId);
            }

            clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
            using (clobDAO)
            {
                entry.Layout = clobDAO.ReadClobAsText(WF_WORKFLOW, LAYOUT, workflowId);
            }

            clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
            using (clobDAO)
            {
                entry.Code = clobDAO.ReadClobAsText(WF_WORKFLOW, CODE, workflowId);
            }
        }

        /// <summary>
        /// Write a data string representing a workflow data into a clob of the WF_WORKFLOW table
        /// </summary>
        /// <param name="workflowId">The unique id of the workflow</param>
        /// <param name="dataType">One of WorkflowDataType enum values</param>
        /// <param name="dataString">The data string.</param>
        public void WriteWorkflowData(string workflowId, WorkflowDataType dataType,
            string dataString)
        {
            string columnName = null;

            switch (dataType)
            {
                case WorkflowDataType.Xoml:
                    columnName = XOML;
                    break;

                case WorkflowDataType.Rules:
                    columnName = RULES;
                    break;

                case WorkflowDataType.Layout:
                    columnName = LAYOUT;
                    break;

                case WorkflowDataType.Code:
                    columnName = CODE;
                    break;
            }

            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(dataString, WF_WORKFLOW, columnName, workflowId);
            }
        }

        /// <summary>
        /// Write a workflow instance state to the underlying database
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <param name="workflowBytes">Data representing the instance state</param>
        /// <param name="unlock">true for saving state without lock, false otherwise.</param>
        public void WriteInstanceState(string instanceId, byte[] workflowBytes, bool unlock)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            if (!IsInstanceStateExist(instanceId, dataProvider))
            {
                // the first time, create a record in database
                CreateInstanceStateInDB(instanceId, unlock, dataProvider);
            }

            // get the clob object to write the state data
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteBlob(WF_INSTANCE_STATE, WF_INSTANCE_STATE_COLUMN, WF_INSTANCE_ID,
                    instanceId, workflowBytes);
            }
        }

        /// <summary>
        /// Read a workflow instance state from the underlying database
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns>byte data representing the instance state.</returns>
        public byte[] ReadInstanceState(string instanceId)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(dataProvider);

            using (clobDAO)
            {
                return clobDAO.ReadBlob(WF_INSTANCE_STATE, WF_INSTANCE_STATE_COLUMN, WF_INSTANCE_ID,
                    instanceId);
            }
        }

        /// <summary>
        /// Write a completed scope to the underlying database
        /// </summary>
        /// <param name="scopeId">The scope id</param>
        /// <param name="workflowBytes">Data representing the scope activity</param>
        public void WriteCompletedScope(string scopeId, byte[] workflowBytes)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            if (!IsCompletedScopeExist(scopeId, dataProvider))
            {
                // the first time, create a record in database
                CreateCompletedScopeInDB(scopeId, dataProvider);
            }

            // get the clob object to write the state data
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteBlob(WF_COMPLETED_SCOPE, WF_SCOPE_STATE_COLUMN, WF_SCOPE_ID,
                    scopeId, workflowBytes);
            }
        }

        /// <summary>
        /// Read a completed scope from the underlying database
        /// </summary>
        /// <param name="scopeId">The scope id</param>
        /// <returns>byte data representing the completed scope.</returns>
        public byte[] ReadCompletedScope(string scopeId)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(dataProvider);

            using (clobDAO)
            {
                return clobDAO.ReadBlob(WF_COMPLETED_SCOPE, WF_SCOPE_STATE_COLUMN, WF_SCOPE_ID,
                    scopeId);
            }
        }

        /// <summary>
        /// Gets the binding info associated with a data instance of the given id.
        /// </summary>
        /// <param name="objId">The id of data instance</param>
        /// <returns>The BindingInfo object, null if the binding deosn't exist</returns>
        /// <remarks>An instance can be bound to only one workflow instance</remarks>
        public WorkflowInstanceBindingInfo GetBindingInfoByObjId(string objId)
        {
            WorkflowInstanceBindingInfo binding = null;

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetBindingInfoByObjId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@datainstanceid", "'" + objId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":datainstanceid", "'" + objId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    binding = new WorkflowInstanceBindingInfo();

                    binding.WorkflowInstanceId = reader.GetString(0);
                    binding.WorkflowTypeId = reader.GetValue(1).ToString();
                    if (!reader.IsDBNull(2))
                    {
                        binding.DataInstanceId = reader.GetValue(2).ToString();
                    }

                    if (!reader.IsDBNull(3))
                    {
                        binding.DataClassName = reader.GetString(3);
                    }

                    if (!reader.IsDBNull(4))
                    {
                        binding.SchemaId = reader.GetString(4);
                    }
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return binding;
        }

        /// <summary>
        /// Set a binding between a data instance and a workflow instance
        /// </summary>
        /// <param name="dataInstanceId">The data instance id</param>
        /// <param name="dataClassName">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <param name="workflowTypeId">The workflow unique type id</param>
        public void SetWorkflowInstanceBinding(string dataInstanceId, string dataClassName, string schemaId, string workflowInstanceId, string workflowTypeId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("SetWFInstanceBinding");

            if (dataInstanceId == null)
            {
                dataInstanceId = "";
            }

            if (dataClassName == null)
            {
                dataClassName = "";
            }

            if (schemaId == null)
            {
                schemaId = "";
            }

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                        sql = sql.Replace("@datainstanceid", "'" + dataInstanceId + "'");
                        sql = sql.Replace("@dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace("@schemaid", "'" + schemaId + "'");
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");
                        sql = sql.Replace("@Workflowtypeid", "'" + workflowTypeId + "'");
                        break;

                    case DatabaseType.SQLServerCE:
                        if (string.IsNullOrEmpty(dataInstanceId))
                        {
                            dataInstanceId = "0";
                        }
                        sql = sql.Replace("@datainstanceid", dataInstanceId);
                        sql = sql.Replace("@dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace("@schemaid", "'" + schemaId + "'");
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");
                        sql = sql.Replace("@Workflowtypeid", workflowTypeId);
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":datainstanceid", "'" + dataInstanceId + "'");
                        sql = sql.Replace(":dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace(":schemaid", "'" + schemaId + "'");
                        sql = sql.Replace(":wfinstanceid", "'" + workflowInstanceId + "'");
                        sql = sql.Replace(":Workflowtypeid", "'" + workflowTypeId + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.WriteLine(e.Message + "\n" + e.StackTrace);

                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// update a binding between a data instance and a workflow instance
        /// </summary>
        /// <param name="dataInstanceId">The data instance id</param>
        /// <param name="dataClassName">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void UpdateWorkflowInstanceBinding(string dataInstanceId, string dataClassName, string schemaId, string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateWFInstanceBinding");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                        sql = sql.Replace("@datainstanceid", "'" + dataInstanceId + "'");
                        sql = sql.Replace("@dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace("@schemaid", "'" + schemaId + "'");
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");
                        break;

                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@datainstanceid", dataInstanceId);
                        sql = sql.Replace("@dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace("@schemaid", "'" + schemaId + "'");
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":datainstanceid", "'" + dataInstanceId + "'");
                        sql = sql.Replace(":dataclassname", "'" + dataClassName + "'");
                        sql = sql.Replace(":schemaid", "'" + schemaId + "'");
                        sql = sql.Replace(":wfinstanceid", "'" + workflowInstanceId + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets binding information of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>The bidning info, null if no binding exists</returns>
        public WorkflowInstanceBindingInfo GetBindingInfoByWorkflowInstanceId(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDataReader reader = null;
            IDbCommand cmd = con.CreateCommand();
            WorkflowInstanceBindingInfo binding = null;

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWFInstanceBinding");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":wfinstanceid", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    binding = new WorkflowInstanceBindingInfo();

                    binding.WorkflowInstanceId = reader.GetString(0);
                    binding.WorkflowTypeId = reader.GetValue(1).ToString();
                    if (!reader.IsDBNull(2))
                    {
                        binding.DataInstanceId = reader.GetValue(2).ToString();
                    }

                    if (!reader.IsDBNull(3))
                    {
                        binding.DataClassName = reader.GetString(3);
                    }

                    if (!reader.IsDBNull(4))
                    {
                        binding.SchemaId = reader.GetString(4);
                    }

                    binding.WorkflowName = reader.GetString(5);

                    binding.ProjectName = reader.GetString(6);

                    if (!reader.IsDBNull(7))
                    {
                        binding.ProjectVersion = reader.GetString(7);
                    }
                }

                return binding;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Gets state information of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>The state info, null if no state exists</returns>
        public WorkflowInstanceStateInfo GetStateInfoByWorkflowInstanceId(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDataReader reader = null;
            IDbCommand cmd = con.CreateCommand();
            WorkflowInstanceStateInfo stateInfo = null;

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetInstanceState");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@instanceid", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":instanceid", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    stateInfo = new WorkflowInstanceStateInfo();

                    stateInfo.WorkflowInstanceId = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                    {
                        if (reader.GetValue(1).ToString() == "1")
                        {
                            stateInfo.Unlocked = true;
                        }
                        else
                        {
                            stateInfo.Unlocked = false;
                        }
                    }

                    if (!reader.IsDBNull(2))
                    {
                        stateInfo.ModifiedTime = DateTime.Parse(reader.GetValue(2).ToString());
                    }

                    stateInfo.State = ReadInstanceState(workflowInstanceId);
                }

                return stateInfo;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Get ids of the data instances of a schema that has been bound to workflow instances.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="pageSize"> The number of ids returned each call</param>
        /// <param name="pageIndex"> The index of current page</param>
        /// <returns>A string array of data instance ids, null if it reaches the end of result.</returns>
        public string[] GetBindingDataInstanceIds(string schemaId, int pageSize, int pageIndex)
        {
            IDbConnection con = _dataProvider.Connection;
            IDataReader reader = null;
            IDbCommand cmd = con.CreateCommand();
            StringCollection ids = new StringCollection();

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWFDataInstanceIds");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@schemaId", "'" + schemaId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":schemaId", "'" + schemaId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                /*
                 * move the cursor of result set to the position indicated by the from
                 * of range
                 */
                int row = 0;
                int from = pageSize * pageIndex;
                int to = from + pageSize;

                while (row < from && reader.Read())
                {
                    row++;
                }

                // Now process the rows fall in the range
                while (row < to && reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        ids.Add(reader.GetValue(0).ToString());
                    }

                    row++;
                }

                if (ids.Count > 0)
                {
                    string[] idArray = new string[ids.Count];
                    int index = 0;
                    foreach (string id in ids)
                    {
                        idArray[index++] = id;
                    }

                    return idArray;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Replace the old id of a binding data instance with a new id. This method is used when
        /// restore a database from a backup file in which new ids are created for each data instance.
        /// </summary>
        /// <param name="oldInstanceId"> The old instance id</param>
        /// <param name="newInstanceId"> The new instance id</param>        
        public void ReplaceBindingDataInstanceId(string oldInstanceId, string newInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateDataInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@oldinstanceid", "'" + oldInstanceId + "'");
                        sql = sql.Replace("@newinstanceid", "'" + newInstanceId + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":oldinstanceid", "'" + oldInstanceId + "'");
                        sql = sql.Replace(":newinstanceid", "'" + newInstanceId + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets current workflow instances from database.
        /// </summary>
        public List<WorkflowInstanceBindingInfo> GetWorkflowInstances()
        {
            IDbConnection con = _dataProvider.Connection;
            IDataReader reader = null;
            IDbCommand cmd = con.CreateCommand();
            WorkflowInstanceBindingInfo binding = null;

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstances");

            try
            {
                List<WorkflowInstanceBindingInfo> workflowInstances = new List<WorkflowInstanceBindingInfo>();

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    binding = new WorkflowInstanceBindingInfo();

                    binding.WorkflowInstanceId = reader.GetString(0);
                    binding.WorkflowTypeId = reader.GetValue(1).ToString();
                    if (!reader.IsDBNull(2))
                    {
                        binding.DataInstanceId = reader.GetValue(2).ToString();
                    }

                    if (!reader.IsDBNull(3))
                    {
                        binding.DataClassName = reader.GetString(3);
                    }

                    if (!reader.IsDBNull(4))
                    {
                        binding.SchemaId = reader.GetString(4);
                    }

                    binding.WorkflowName = reader.GetString(5);

                    binding.ProjectName = reader.GetString(6);

                    if (!reader.IsDBNull(7))
                    {
                        binding.ProjectVersion = reader.GetString(7);
                    }

                    workflowInstances.Add(binding);
                }

                return workflowInstances;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Delete the workflow instance's bindings to data instances
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void DeleteWorkflowInstanceBindings(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@wfinstanceid", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":wfinstanceid", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete the workflow instance state of given instance id
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void DeleteInstanceState(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteWFInstanceState");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@instanceid", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":instanceid", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets event subscriptions saved in database
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DBEventSubscription> GetEventSubscriptions()
        {
            Dictionary<string, DBEventSubscription> subscriptions = new Dictionary<string, DBEventSubscription>();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetEventSubscriptions");

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                DBEventSubscription subscription;
                string subscriptionId;
                while (reader.Read())
                {
                    subscription = new DBEventSubscription();
                    subscriptionId = reader.GetString(0);
                    subscription.SubscriptionId = subscriptionId;
                    subscription.WorkflowInstanceId = new Guid(reader.GetString(1));
                    subscription.QueueName = reader.GetString(2);
                    subscription.SchemaId = reader.GetString(3);
                    subscription.ClassName = reader.GetString(4);
                    subscription.EventName = reader.GetString(5);
                    subscription.CreateDataBinding = (reader.GetString(6).Trim() == "true" ? true : false);

                    subscriptions.Add(subscriptionId, subscription);
                }

                return subscriptions;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Gets event subscriptions saved in database for a workflow instance
        /// </summary>
        /// <returns>A collection of DBEventSubscription</returns>
        public DBEventSubscriptionCollection GetEventSubscriptionsByWorkflowInstanceId(string workflowInstanceId)
        {
            DBEventSubscriptionCollection subscriptions = new DBEventSubscriptionCollection();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetEventSubscriptionsByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WFInstanceID", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WFInstanceID", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                DBEventSubscription subscription;
                while (reader.Read())
                {
                    subscription = new DBEventSubscription();
                    subscription.SubscriptionId = reader.GetString(0);
                    subscription.WorkflowInstanceId = new Guid(reader.GetString(1));
                    subscription.QueueName = reader.GetString(2);
                    subscription.SchemaId = reader.GetString(3);
                    subscription.ClassName = reader.GetString(4);
                    subscription.EventName = reader.GetString(5);
                    subscription.CreateDataBinding = (reader.GetString(6).Trim() == "true" ? true : false);

                    subscriptions.Add(subscription);
                }

                return subscriptions;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Add an event subscription to the database
        /// </summary>
        /// <param name="subscriptionId">Sunbscription id</param>
        /// <param name="subscription">The subscription data</param>
        public void AddEventSubscription(Guid subscriptionId, DBEventSubscription subscription)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddEventSubscription");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@SubscriptionId", "'" + subscriptionId.ToString() + "'");
                        sql = sql.Replace("@WFInstanceID", "'" + subscription.WorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace("@QueueName", "'" + subscription.QueueName.ToString() + "'");
                        sql = sql.Replace("@SchemaId", "'" + subscription.SchemaId + "'");
                        sql = sql.Replace("@ClassName", "'" + subscription.ClassName + "'");
                        sql = sql.Replace("@EventName", "'" + subscription.EventName + "'");
                        sql = sql.Replace("@CreateBinding", "'" + (subscription.CreateDataBinding? "true" : "false") + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":SubscriptionId", "'" + subscriptionId.ToString() + "'");
                        sql = sql.Replace(":WFInstanceID", "'" + subscription.WorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace(":QueueName", "'" + subscription.QueueName.ToString() + "'");
                        sql = sql.Replace(":SchemaId", "'" + subscription.SchemaId + "'");
                        sql = sql.Replace(":ClassName", "'" + subscription.ClassName + "'");
                        sql = sql.Replace(":EventName", "'" + subscription.EventName + "'");
                        sql = sql.Replace(":CreateBinding", "'" + (subscription.CreateDataBinding ? "true" : "false") + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete an event subscription from the database
        /// </summary>
        /// <param name="subscriptionId">Sunbscription id</param>
        public void DeleteEventSubscription(string subscriptionId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelEventSubscription");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@SubscriptionId", "'" + subscriptionId + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":SubscriptionId", "'" + subscriptionId + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete all subscriptions associated with a workflow instance, called affer
        /// the workflow is completed or terminated.
        /// </summary>
        /// <param name="worrkflowInstanceId"></param>
        public void DeleteSubscriptionByWFInstanceId(string worrkflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteSubscriptionsByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets workflow event subscriptions saved in database
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, WorkflowEventSubscription> GetWorkflowEventSubscriptions()
        {
            Dictionary<string, WorkflowEventSubscription> subscriptions = new Dictionary<string, WorkflowEventSubscription>();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowEventSubscriptions");

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                WorkflowEventSubscription subscription;
                string subscriptionId;
                while (reader.Read())
                {
                    subscription = new WorkflowEventSubscription();
                    subscriptionId = reader.GetString(0);
                    subscription.SubscriptionId = subscriptionId;
                    subscription.ParentWorkflowInstanceId = new Guid(reader.GetString(1));
                    subscription.ChildWorkflowInstanceId = new Guid(reader.GetString(2));
                    subscription.QueueName = reader.GetString(3);

                    subscriptions.Add(subscriptionId, subscription);
                }

                return subscriptions;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Gets workflow event subscriptions saved in database for a workflow instance
        /// </summary>
        /// <returns></returns>
        public WorkflowEventSubscriptionCollection GetWorkflowEventSubscriptionsByWFInstanceId(string worrkflowInstanceId)
        {
            WorkflowEventSubscriptionCollection subscriptions = new WorkflowEventSubscriptionCollection();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowEventSubscriptionsByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ParentWFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ParentWFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                WorkflowEventSubscription subscription;
                while (reader.Read())
                {
                    subscription = new WorkflowEventSubscription();
                    subscription.SubscriptionId = reader.GetString(0);
                    subscription.ParentWorkflowInstanceId = new Guid(reader.GetString(1));
                    subscription.ChildWorkflowInstanceId = new Guid(reader.GetString(2));
                    subscription.QueueName = reader.GetString(3);

                    subscriptions.Add(subscription);
                }

                return subscriptions;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Add a workflow event subscription to the database
        /// </summary>
        /// <param name="subscriptionId">Sunbscription id</param>
        /// <param name="subscription">The subscription data</param>
        public void AddWorkflowEventSubscription(Guid subscriptionId, WorkflowEventSubscription subscription)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddWorkflowEventSubscription");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@SubscriptionId", "'" + subscriptionId.ToString() + "'");
                        sql = sql.Replace("@ParentWFInstanceID", "'" + subscription.ParentWorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace("@ChildWFInstanceID", "'" + subscription.ChildWorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace("@QueueName", "'" + subscription.QueueName.ToString() + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":SubscriptionId", "'" + subscriptionId.ToString() + "'");
                        sql = sql.Replace(":ParentWFInstanceID", "'" + subscription.ParentWorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace(":ChildWFInstanceID", "'" + subscription.ChildWorkflowInstanceId.ToString() + "'");
                        sql = sql.Replace(":QueueName", "'" + subscription.QueueName.ToString() + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Update the queue name of a subscription
        /// </summary>
        /// <param name="subscriptionId">Sunbscription id</param>
        /// <param name="queueName">The subscription's queueName</param>
        public void UpdateWFEventQueueName(string subscriptionId, string queueName)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateWorkflowEventSubscription");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@SubscriptionId", "'" + subscriptionId + "'");
                        sql = sql.Replace("@QueueName", "'" + queueName + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":SubscriptionId", "'" + subscriptionId + "'");
                        sql = sql.Replace(":QueueName", "'" + queueName + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete all workflow event subscriptions associated with a workflow instance, called affer
        /// the workflow is completed or terminated.
        /// </summary>
        /// <param name="worrkflowInstanceId"></param>
        public void DeleteWorkflowEventSubscriptionByWFId(string worrkflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteWorkflowEventSubscriptionsByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ParentWFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ParentWFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets database event contexts saved in database
        /// </summary>
        /// <returns></returns>
        public List<EventContext> GetDBEventConexts()
        {
            List<EventContext> eventContexts = new List<EventContext>();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetDBEventContexts");

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                EventContext eventContext;
                string value;
                MetaDataModel metaData;
                SchemaInfo schemaInfo;
                ClassElement classElement;
                string[] attributeArray;
                StringCollection attributesUpdated;
                while (reader.Read())
                {
                    eventContext = new EventContext();
                    eventContext.EventContextId = reader.GetString(0);
                    value = reader.GetString(1); // Get schema id of the context
                    schemaInfo = ParseSchemaId(value);
                    metaData = MetaDataCache.Instance.GetMetaData(schemaInfo, dataProvider);
                    if (metaData == null)
                    {
                        continue;
                    }
                    else
                    {
                        eventContext.MetaData = metaData;
                    }
                    value = reader.GetString(2); // Get class name of the context
                    classElement = metaData.SchemaModel.FindClass(value);
                    if (classElement == null)
                    {
                        continue;
                    }
                    else
                    {
                        eventContext.ClassElement = classElement;
                    }
                    eventContext.ObjId = reader.GetString(3); // get instance Id
                    value = reader.GetString(4); // Get operation type
                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            eventContext.OperationType = (OperationType)Enum.Parse(typeof(OperationType), value);
                        }
                        catch (Exception)
                        {
                            eventContext.OperationType = OperationType.Unknown;
                        }
                    }
                    if (!reader.IsDBNull(5))
                    {
                        value = reader.GetString(5); // Get attributes updated array
                        if (!string.IsNullOrEmpty(value))
                        {
                            attributeArray = value.Split(';');
                            attributesUpdated = new StringCollection();
                            foreach (string attr in attributeArray)
                            {
                                attributesUpdated.Add(attr);
                            }

                            eventContext.AttributesUpdated = attributesUpdated;
                        }
                    }
                    else
                    {
                        eventContext.AttributesUpdated = null;
                    }

                    if (!string.IsNullOrEmpty(eventContext.EventContextId))
                    {
                        eventContexts.Add(eventContext);
                    }
                }

                return eventContexts;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Add an DB event context to the database
        /// </summary>
        /// <param name="eventContext">The event context instance</param>
        public void AddDBEventContext(EventContext eventContext)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddDBEventContext");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ContextId", "'" + eventContext.EventContextId + "'");
                        sql = sql.Replace("@SchemaId", "'" + eventContext.MetaData.SchemaInfo.NameAndVersion + "'");
                        sql = sql.Replace("@ClassName", "'" + eventContext.ClassElement.Name + "'");
                        if (eventContext.ObjId != null)
                        {
                            sql = sql.Replace("@InstanceId", "'" + eventContext.ObjId + "'");
                        }
                        else
                        {
                            sql = sql.Replace("@InstanceId", "''");
                        }
                        if (eventContext.OperationType != OperationType.Unknown)
                        {
                            sql = sql.Replace("@OperationType", "'" + Enum.GetName(typeof(OperationType), eventContext.OperationType) +"'");
                        }
                        else
                        {
                            sql = sql.Replace("@OperationType", "''");
                        } 
                        if (eventContext.AttributesUpdated != null && eventContext.AttributesUpdated.Count > 0)
                        {
                            StringBuilder builder = new StringBuilder();
                            foreach (string attr in eventContext.AttributesUpdated)
                            {
                                if (builder.Length > 0)
                                {
                                    builder.Append(";");
                                }
                                builder.Append(attr);
                            }
                            sql = sql.Replace("@Attributes", "'" + builder.ToString() +"'");
                        }
                        else
                        {
                            sql = sql.Replace("@Attributes", "''");
                        } 
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ContextId", "'" + eventContext.EventContextId + "'");
                        sql = sql.Replace(":SchemaId", "'" + eventContext.MetaData.SchemaInfo.NameAndVersion + "'");
                        sql = sql.Replace(":ClassName", "'" + eventContext.ClassElement.Name + "'");
                        if (eventContext.ObjId != null)
                        {
                            sql = sql.Replace(":InstanceId", "'" + eventContext.ObjId + "'");
                        }
                        else
                        {
                            sql = sql.Replace(":InstanceId", "''");
                        }
                        if (eventContext.OperationType != OperationType.Unknown)
                        {
                            sql = sql.Replace(":OperationType", "'" + Enum.GetName(typeof(OperationType), eventContext.OperationType) + "'");
                        }
                        else
                        {
                            sql = sql.Replace(":OperationType", "''");
                        }
                        if (eventContext.AttributesUpdated != null && eventContext.AttributesUpdated.Count > 0)
                        {
                            StringBuilder builder = new StringBuilder();
                            foreach (string attr in eventContext.AttributesUpdated)
                            {
                                if (builder.Length > 0)
                                {
                                    builder.Append(";");
                                }
                                builder.Append(attr);
                            }
                            sql = sql.Replace(":Attributes", "'" + builder.ToString() + "'");
                        }
                        else
                        {
                            sql = sql.Replace(":Attributes", "''");
                        }

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete an db event context indicated by the context id
        /// </summary>
        /// <param name="eventContextId">context id</param>
        public void DeleteDBEventContext(string eventContextId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteDBEventContext");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ContextId", "'" + eventContextId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ContextId", "'" + eventContextId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets the name of role that has permission to modify the project
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <returns>The name of role, null for non-protected mode.</returns>
        /// <exception cref="DBException">If a database access error occurs.</exception>
        public string GetDBARole(string projectName, string projectVersion)
        {
            string role = null;
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetProjectRole");
            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + projectName + "'");
                    sql = sql.Replace("@version", "'" + projectVersion + "'");
                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":name", "'" + projectName + "'");
                    sql = sql.Replace(":version", "'" + projectVersion + "'");
                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        role = reader.GetString(0);
                        if (role.Length == 0)
                        {
                            role = null;
                        }
                    }
                }

                return role;
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to get dba role", ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Sets the name of role that has permission to modify the meta data
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="role">The name of role, null to set non-protected mode.</param>
        /// <exception cref="DBException">If a database access error occurs.</exception>		
        public void SetDBARole(string projectName, string projectVersion, string role)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetProjectRole");

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + projectName + "'");
                    sql = sql.Replace("@version", "'" + projectVersion + "'");
                    if (role != null)
                    {
                        sql = sql.Replace("@dba_role", "'" + role + "'");
                    }
                    else
                    {
                        sql = sql.Replace("@dba_role", "''");
                    }
                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":name", "'" + projectName + "'");
                    sql = sql.Replace(":version", "'" + projectVersion + "'");
                    if (role != null)
                    {
                        sql = sql.Replace(":dba_role", "'" + role + "'");
                    }
                    else
                    {
                        sql = sql.Replace(":dba_role", "''");
                    }

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to set dba role", ex);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Create a new workflow instance tracking record
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <param name="workflowTypeId"></param>
        /// <param name="initializedDateTime"></param>
        /// <param name="status"></param>
        public void WriteWorkflowTrackingRecord(string workflowInstanceId, string workflowTypeId,
            string initializedDateTime, string status)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddWorkflowTrackingRecord");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            initializedDateTime = lookup.GetTimestampFunc(initializedDateTime, LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");
                        sql = sql.Replace("@InitializedDateTime", initializedDateTime);
                        sql = sql.Replace("@CurrentStatus", "'" + status + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");
                        sql = sql.Replace(":InitializedDateTime", initializedDateTime);
                        sql = sql.Replace(":CurrentStatus", "'" + status + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Update a workflow instance tracking record.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <param name="eventDateTime"></param>
        /// <param name="status"></param>
        public void UpdateWorkflowTrackingRecord(string workflowInstanceId,
            string eventDateTime, string status)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateWorkflowTrackingRecord");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            eventDateTime = lookup.GetTimestampFunc(eventDateTime, LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace("@EndDateTime", eventDateTime);
                        sql = sql.Replace("@CurrentStatus", "'" + status + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace(":EndDateTime", eventDateTime);
                        sql = sql.Replace(":CurrentStatus", "'" + status + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Create a new activity instance tracking record
        /// </summary>
        /// <param name="activityInstanceId"></param>
        /// <param name="activityTypeName">Activity type name</param>
        /// <param name="qualifiedName"></param>
        /// <param name="initializedDateTime"></param>
        /// <param name="status"></param>
        /// <param name="workflowInstanceId"></param>
        public void WriteActivityTrackingRecord(string activityInstanceId, 
            string activityTypeName,
            string qualifiedName,
            string initializedDateTime, string status, string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddActivityTrackingRecord");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            initializedDateTime = lookup.GetTimestampFunc(initializedDateTime, LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ActivityInstanceId", "'" + activityInstanceId + "'");
                        sql = sql.Replace("@QualifiedName", "'" + qualifiedName + "'");
                        sql = sql.Replace("@InitializedDateTime", initializedDateTime);
                        sql = sql.Replace("@CurrentStatus", "'" + status + "'");
                        sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace("@TypeName", "'" + activityTypeName + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ActivityInstanceId", "'" + activityInstanceId + "'");
                        sql = sql.Replace(":QualifiedName", "'" + qualifiedName + "'");
                        sql = sql.Replace(":InitializedDateTime", initializedDateTime);
                        sql = sql.Replace(":CurrentStatus", "'" + status + "'");
                        sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");
                        sql = sql.Replace(":TypeName", "'" + activityTypeName + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Update an activity instance tracking record.
        /// </summary>
        /// <param name="activityInstanceId"></param>
        /// <param name="eventDateTime"></param>
        /// <param name="status"></param>
        public void UpdateActivityTrackingRecord(string activityInstanceId,
            string eventDateTime, string status)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateActivityTrackingRecord");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            eventDateTime = lookup.GetTimestampFunc(eventDateTime, LocaleInfo.Instance.DateTimeFormat);
            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@ActivityInstanceId", "'" + activityInstanceId + "'");
                        sql = sql.Replace("@EndDateTime", eventDateTime);
                        sql = sql.Replace("@CurrentStatus", "'" + status + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":ActivityInstanceId", "'" + activityInstanceId + "'");
                        sql = sql.Replace(":EndDateTime", eventDateTime);
                        sql = sql.Replace(":CurrentStatus", "'" + status + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets id of an activity instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <param name="qualifiedName">The activity qualified name</param>
        /// <returns>return null if the activity instance doesn't exist.</returns>
        public string GetActivityInstanceId(string workflowInstanceId, string qualifiedName)
        {
            string activityInstanceId = null;

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetActivityInstanceId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");
                    sql = sql.Replace("@QualifiedName", "'" + qualifiedName + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");
                    sql = sql.Replace(":QualifiedName", "'" + qualifiedName + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    activityInstanceId = reader.GetValue(0).ToString();
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return activityInstanceId;
        }

        /// <summary>
        /// Gets all workflow instance tracking records of a workflow type
        /// </summary>
        /// <param name="workflowTypeId"></param>
        /// <returns>Instance count</returns>
        public int GetTrackingWorkflowInstanceCount(string workflowTypeId)
        {
            int count = 0;
            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstanceCountByTypeId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                count = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        /// <summary>
        /// Gets all workflow instance tracking records of a workflow type
        /// </summary>
        /// <param name="workflowTypeId"></param>
        /// <param name="pageIndex">Page index, start from 0</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstances(string workflowTypeId, int pageIndex, int pageSize)
        {
            NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = new NewteraTrackingWorkflowInstanceCollection();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstancesByTypeId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                NewteraTrackingWorkflowInstance trackingWorkflowInstance;
                string wfInstanceId;

                // move the cursor of result set to the position indicated by pageindex
                int count = 0;
                int from = pageIndex * pageSize;
                int to = from + pageSize;

                // skip the rows outside the range
                while (count < from && reader.Read())
                {
                    count++;
                }

                while (count < to && reader.Read())
                {
                    wfInstanceId = reader["WorkflowInstanceId"].ToString();
                    trackingWorkflowInstance = new NewteraTrackingWorkflowInstance(wfInstanceId);
                    trackingWorkflowInstance.WorkflowInstanceId = new Guid(wfInstanceId);
                    trackingWorkflowInstance.TrackingEvent = reader["CurrentStatus"].ToString();
                    TrackingWorkflowEvent trackingEvent = (TrackingWorkflowEvent)Enum.Parse(typeof(TrackingWorkflowEvent), trackingWorkflowInstance.TrackingEvent);
                    trackingWorkflowInstance.Status = ConvertWorkflowStatus(trackingEvent);
                    trackingWorkflowInstance.Initialized = DateTime.Parse(reader["InitializedDateTime"].ToString());

                    trackingWorkflowInstances.Add(trackingWorkflowInstance);

                    count++;
                }

                foreach (NewteraTrackingWorkflowInstance trackingRecord in trackingWorkflowInstances)
                {
                    // add activity events
                    AddActivityEvents(trackingRecord);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return trackingWorkflowInstances;
        }

        /// <summary>
        /// Gets count of all workflow instance tracking records of a workflow type filtered by conditions
        /// </summary>
        /// <param name="workflowTypeId">Workflow type id</param>
        /// <param name="status">The workflow instance status</param>
        /// <param name="fromDateTime"></param>
        /// <param name="untilDateTime"></param>
        /// <returns>The count</returns>
        public int GetTrackingWorkflowInstanceCountByCondition(string workflowTypeId,
            string status, string fromDateTime, string untilDateTime)
        {
            int count = 0;
            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstanceCountByCondition");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string fromDataTimeFunc = lookup.GetTimestampFunc(fromDateTime, LocaleInfo.Instance.DateTimeFormat);
            string untilDataTimeFunc = lookup.GetTimestampFunc(untilDateTime, LocaleInfo.Instance.DateTimeFormat);

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");
                    sql = sql.Replace("@CurrentStatus", status);
                    sql = sql.Replace("@FromDateTime", fromDataTimeFunc);
                    sql = sql.Replace("@UntilDateTime", untilDataTimeFunc);

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");
                    sql = sql.Replace(":CurrentStatus", status);
                    sql = sql.Replace(":FromDateTime", fromDataTimeFunc);
                    sql = sql.Replace(":UntilDateTime", untilDataTimeFunc);

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                count = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        /// <summary>
        /// Gets all workflow instance tracking records of a workflow type filtered by conditions
        /// </summary>
        /// <param name="workflowTypeId">Workflow type id</param>
        /// <param name="status">The workflow instance status</param>
        /// <param name="fromDateTime"></param>
        /// <param name="untilDateTime"></param>
        /// <returns></returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstancesByCondition(string workflowTypeId,
            string status, string fromDateTime, string untilDateTime, int pageIndex, int pageSize)
        {
            NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = new NewteraTrackingWorkflowInstanceCollection();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstancesByCondition");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string fromDataTimeFunc = lookup.GetTimestampFunc(fromDateTime, LocaleInfo.Instance.DateTimeFormat);
            string untilDataTimeFunc = lookup.GetTimestampFunc(untilDateTime, LocaleInfo.Instance.DateTimeFormat);

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");
                    sql = sql.Replace("@CurrentStatus", status);
                    sql = sql.Replace("@FromDateTime", fromDataTimeFunc);
                    sql = sql.Replace("@UntilDateTime", untilDataTimeFunc);

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");
                    sql = sql.Replace(":CurrentStatus", status);
                    sql = sql.Replace(":FromDateTime", fromDataTimeFunc);
                    sql = sql.Replace(":UntilDateTime", untilDataTimeFunc);

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                NewteraTrackingWorkflowInstance trackingWorkflowInstance;
                string wfInstanceId;

                // move the cursor of result set to the position indicated by pageindex
                int count = 0;
                int from = pageIndex * pageSize;
                int to = from + pageSize;

                // skip the rows outside the range
                while (count < from && reader.Read())
                {
                    count++;
                }

                while (count < to && reader.Read())
                {
                    wfInstanceId = reader["WorkflowInstanceId"].ToString();
                    trackingWorkflowInstance = new NewteraTrackingWorkflowInstance(wfInstanceId);
                    trackingWorkflowInstance.WorkflowInstanceId = new Guid(wfInstanceId);
                    trackingWorkflowInstance.TrackingEvent = reader["CurrentStatus"].ToString();
                    TrackingWorkflowEvent trackingEvent = (TrackingWorkflowEvent)Enum.Parse(typeof(TrackingWorkflowEvent), trackingWorkflowInstance.TrackingEvent);
                    trackingWorkflowInstance.Status = ConvertWorkflowStatus(trackingEvent);
                    trackingWorkflowInstance.Initialized = DateTime.Parse(reader["InitializedDateTime"].ToString());

                    trackingWorkflowInstances.Add(trackingWorkflowInstance);
                }

                foreach (NewteraTrackingWorkflowInstance trackingRecord in trackingWorkflowInstances)
                {
                    // add activity events
                    AddActivityEvents(trackingRecord);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return trackingWorkflowInstances;
        }

        /// <summary>
        /// Gets all workflow instance tracking records of a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance id</param>
        /// <returns></returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstancesByWorkflowInstanceId(string workflowInstanceId)
        {
            NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = new NewteraTrackingWorkflowInstanceCollection();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstancesByWFInstanceId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                NewteraTrackingWorkflowInstance trackingWorkflowInstance;

                while (reader.Read())
                {
                    trackingWorkflowInstance = new NewteraTrackingWorkflowInstance(workflowInstanceId);
                    trackingWorkflowInstance.WorkflowInstanceId = new Guid(workflowInstanceId);
                    trackingWorkflowInstance.TrackingEvent = reader["CurrentStatus"].ToString();
                    TrackingWorkflowEvent trackingEvent = (TrackingWorkflowEvent)Enum.Parse(typeof(TrackingWorkflowEvent), trackingWorkflowInstance.TrackingEvent);
                    trackingWorkflowInstance.Status = ConvertWorkflowStatus(trackingEvent);
                    trackingWorkflowInstance.Initialized = DateTime.Parse(reader["InitializedDateTime"].ToString());

                    trackingWorkflowInstances.Add(trackingWorkflowInstance);
                }

                foreach (NewteraTrackingWorkflowInstance trackingRecord in trackingWorkflowInstances)
                {
                    // add activity events
                    AddActivityEvents(trackingRecord);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return trackingWorkflowInstances;
        }

        /// <summary>
        /// Delete all activity tracking records associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        public void DeleteActivityTrackingRecords(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelActivityInstancesByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete the workflow instance tracking record associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        public void DeleteWorkflowTrackingRecords(string workflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelWorkflowInstanceByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WorkflowInstanceId", "'" + workflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WorkflowInstanceId", "'" + workflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets the information indicating whether a workflow model has running instances.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowTypeId">The workflow id</param>
        /// <returns>true if it has running instances, false otherwise.</returns>
        public bool HasRunningInstances(string projectName, string projectVersion, string workflowTypeId)
        {
            bool status = false;

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowInstancesByTypeId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowTypeId", "'" + workflowTypeId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    TrackingWorkflowEvent trackingEvent = (TrackingWorkflowEvent)Enum.Parse(typeof(TrackingWorkflowEvent), reader["CurrentStatus"].ToString());
                    WorkflowStatus workflowStatus = ConvertWorkflowStatus(trackingEvent);
                    if (workflowStatus == WorkflowStatus.Created ||
                        workflowStatus == WorkflowStatus.Running ||
                        workflowStatus == WorkflowStatus.Suspended)
                    {
                        status = true;
                        break;
                    }
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return status;
        }

        /// <summary>
        /// Gets the id of a workflow model given the name.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow Name</param>
        /// <returns>The id of the found workflow model, null if the workflow model does not exist.</returns>
        public string GetWorkflowModelID(string projectName, string projectVersion, string workflowName)
        {
            string workflowModelId = null;

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetWorkflowModelId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@ProjectName", "'" + projectName + "'");
                    sql = sql.Replace("@ProjectVersion", "'" + projectVersion + "'");
                    sql = sql.Replace("@WorkflowName", "'" + workflowName + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":ProjectName", "'" + projectName + "'");
                    sql = sql.Replace(":ProjectVersion", "'" + projectVersion + "'");
                    sql = sql.Replace(":WorkflowName", "'" + workflowName + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    workflowModelId = reader.GetValue(0).ToString();
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return workflowModelId;
        }

        /// <summary>
        /// Sets the time as the latest modified time of the given workflow project
        /// </summary>
        /// <param name="projectName">The name of the project to set time stamp</param>
        /// <param name="projectVersion">The version of project</param>
        /// <param name="modifiedTime">The modified time.</param>
        /// <exception cref="DBException">If a database access error occurs.</exception>		
        public void SetModifiedTime(string projectName, string projectVersion, DateTime modifiedTime)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetWFProjectModifiedTime");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedDateTime = lookup.GetTimestampFunc(modifiedTime.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + projectName + "'");
                    sql = sql.Replace("@version", "'" + projectVersion + "'");
                    sql = sql.Replace("@modified_time", modifiedDateTime);

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":name", "'" + projectName + "'");
                    sql = sql.Replace(":version", "'" + projectVersion + "'");
                    sql = sql.Replace(":modified_time", modifiedDateTime);

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new DBException("Failed to set modified time of workflow project.", ex);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Fill the task substitute model with data stored in database
        /// </summary>
        /// <param name="taskSubstituteModel">A TaskSubstituteModel to fill with data</param>
        public void FillTaskSubstituteModel(TaskSubstituteModel taskSubstituteModel)
        {
            // get a stream to read schema model from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                taskSubstituteModel.Read(clobDAO.ReadClob(WF_TASK_SUBSTITUTE, XML_DATA, WF_TASK_SUBSTITUTE_ID));
            }
        }

        /// <summary>
        /// Write a xml that represents for the task substitute model to the database.
        /// </summary>
        /// <param name="xml">The task substitute model xml string</param>
        public void WriteTaskSubstituteModel(string xml)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xml, WF_TASK_SUBSTITUTE, XML_DATA, WF_TASK_SUBSTITUTE_ID);
            }
        }

        /// <summary>
        /// Gets reassigned task infos for a task saved in database
        /// </summary>
        /// <returns>A List of ReassignedTaskInfo</returns>
        public List<ReassignedTaskInfo> GetReassignedTaskInfosByTaskId(string taskId)
        {
            List<ReassignedTaskInfo> taskInfos = new List<ReassignedTaskInfo>();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetReassignedTaskInfos");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@TaskId", "'" + taskId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":TaskId", "'" + taskId + "'");

                        break;
                }

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                ReassignedTaskInfo taskInfo;
                while (reader.Read())
                {
                    taskInfo = new ReassignedTaskInfo();
                    taskInfo.TaskId = reader.GetValue(0).ToString();;
                    taskInfo.WorkflowInstanceId = reader.GetString(1);
                    taskInfo.OriginalOwner = reader.GetString(2);
                    taskInfo.CurrentOwner = reader.GetString(3);

                    taskInfos.Add(taskInfo);
                }

                return taskInfos;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Gets all reassigned task infos saved in database
        /// </summary>
        /// <returns>A List of ReassignedTaskInfo</returns>
        public List<ReassignedTaskInfo> GetReassignedTaskInfos()
        {
            List<ReassignedTaskInfo> taskInfos = new List<ReassignedTaskInfo>();

            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = "select TaskId, WorkflowInstanceId, OriginalOwner, CurrentOwner from WF_REASSIGNED_TASK";

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                ReassignedTaskInfo taskInfo;
                while (reader.Read())
                {
                    taskInfo = new ReassignedTaskInfo();
                    taskInfo.TaskId = reader.GetValue(0).ToString(); ;
                    taskInfo.WorkflowInstanceId = reader.GetString(1);
                    taskInfo.OriginalOwner = reader.GetString(2);
                    taskInfo.CurrentOwner = reader.GetString(3);

                    taskInfos.Add(taskInfo);
                }

                return taskInfos;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Add a reassigned task info to database
        /// </summary>
        /// <param name="taskInfo"></param>
        public void AddReassignedTaskInfo(ReassignedTaskInfo taskInfo)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddReassignedTaskInfo");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@TaskId", "'" + taskInfo.TaskId.ToString() + "'");
                        sql = sql.Replace("@WFInstanceID", "'" + taskInfo.WorkflowInstanceId + "'");
                        sql = sql.Replace("@OriginalOwner", "'" + taskInfo.OriginalOwner + "'");
                        sql = sql.Replace("@CurrentOwner", "'" + taskInfo.CurrentOwner + "'");
                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":TaskId", "'" + taskInfo.TaskId.ToString() + "'");
                        sql = sql.Replace(":WFInstanceID", "'" + taskInfo.WorkflowInstanceId + "'");
                        sql = sql.Replace(":OriginalOwner", "'" + taskInfo.OriginalOwner + "'");
                        sql = sql.Replace(":CurrentOwner", "'" + taskInfo.CurrentOwner + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Remove all reassigned task infos associated with a task id
        /// </summary>
        /// <param name="taskId"></param>
        public void DeleteReassignedTaskInfoByTaskId(string taskId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteReassignedTaskInfoByTaskId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@TaskId", "'" + taskId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":TaskId", "'" + taskId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete all reassigned task infos associated with a workflow instance, called affer
        /// the workflow is completed or terminated.
        /// </summary>
        /// <param name="worrkflowInstanceId"></param>
        public void DeleteReassignedTaskInfosByWFInstanceId(string worrkflowInstanceId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteReassignedTaskInfosByWFInstanceId");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@WFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":WFInstanceID", "'" + worrkflowInstanceId + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Replace a current owner of a reassigned task info with a new current owner
        /// </summary>
        /// <param name="oldCurrentOwner"></param>
        /// <param name="newCurrentOwner"></param>
        public void ReplaceReassignedTaskCurrentOwner(string taskId, string oldCurrentOwner, string newCurrentOwner)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("ReplaceReassignedTaskCurrentOwner");

            try
            {
                switch (_dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@TaskId", "'" + taskId + "'");
                        sql = sql.Replace("@NewOwner", "'" + newCurrentOwner + "'");
                        sql = sql.Replace("@OldOwner", "'" + oldCurrentOwner + "'");

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":TaskId", "'" + taskId + "'");
                        sql = sql.Replace(":NewOwner", "'" + newCurrentOwner + "'");
                        sql = sql.Replace(":OldOwner", "'" + oldCurrentOwner + "'");

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets the modified time of the given project info
        /// </summary>
        /// <param name="projectInfo">The project info</param>
        /// <exception cref="DBException">If a database access error occurs.</exception>		
        private void GetModifiedTime(ProjectInfo projectInfo)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetWFProjectModifiedTime");

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + projectInfo.Name + "'");
                    sql = sql.Replace("@version", "'" + projectInfo.Version + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":name", "'" + projectInfo.Name + "'");
                    sql = sql.Replace(":version", "'" + projectInfo.Version + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        projectInfo.ModifiedTime = DateTime.Parse(reader["MODIFIED_TIME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to get modified time of project info.", ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        private void AddActivityEvents(NewteraTrackingWorkflowInstance trackingWorkflowInstance)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetActivityInstancesByWFInstanceId");

            switch (_dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@WorkflowInstanceId", "'" + trackingWorkflowInstance.WorkflowInstanceId.ToString() + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":WorkflowInstanceId", "'" + trackingWorkflowInstance.WorkflowInstanceId.ToString() + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                NewteraActivityTrackingRecord activityTrackingRecord;
                string activityInstanceId;
                int index = 0;
                while (reader.Read())
                {
                    activityInstanceId = reader["ActivityInstanceId"].ToString();
                    activityTrackingRecord = new NewteraActivityTrackingRecord(activityInstanceId);
                    activityTrackingRecord.ID = activityInstanceId;
                    activityTrackingRecord.EventOrder = index; // TODO, get event order from db
                    activityTrackingRecord.ExecutionStatus = (ActivityExecutionStatus)Enum.Parse(typeof(ActivityExecutionStatus), reader["CurrentStatus"].ToString());
                    activityTrackingRecord.QualifiedName = reader["QualifiedName"].ToString();
                    activityTrackingRecord.Initialized = DateTime.Parse(reader["InitializedDateTime"].ToString());
                    activityTrackingRecord.TypeName = reader["TypeName"].ToString();

                    trackingWorkflowInstance.ActivityEvents.Add(activityTrackingRecord);

                    index++;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Gets the information indicating whether a given instance has already existed
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns>true if it exists, false otherwise</returns>
        private bool IsInstanceStateExist(string instanceId, IDataProvider dataProvider)
        {
            bool status = false;

            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetInstanceState");

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@instanceid", "'" + instanceId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":instanceid", "'" + instanceId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    status = true;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return status;
        }

        /// <summary>
        /// Create a record in WF_INSTANCE_STATE table for a workflow instance
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="unlock"></param>
        /// <param name="dataProvider"></param>
        private void CreateInstanceStateInDB(string instanceId, bool unlock, IDataProvider dataProvider)
        {
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AddInstanceState");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedDateTime = lookup.GetTimestampFunc(DateTime.Now.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@instanceid", "'" + instanceId + "'");
                        sql = sql.Replace("@unlocked", "'" + (unlock ? "1" : "0") + "'");
                        sql = sql.Replace("@modified", modifiedDateTime);

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":instanceid", "'" + instanceId + "'");
                        sql = sql.Replace(":unlocked", "'" + (unlock ? "1" : "0") + "'");
                        sql = sql.Replace(":modified", modifiedDateTime);

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Gets the information indicating whether a given completed scope has already existed
        /// </summary>
        /// <param name="scopeId">The scope id</param>
        /// <returns>true if it exists, false otherwise</returns>
        private bool IsCompletedScopeExist(string scopeId, IDataProvider dataProvider)
        {
            bool status = false;

            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetCompletedScope");

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@completedscopeid", "'" + scopeId + "'");

                    break;

                case DatabaseType.Oracle:
                    sql = sql.Replace(":completedscopeid", "'" + scopeId + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    status = true;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return status;
        }

        /// <summary>
        /// Create a record in WF_COMPLETED_SCOPE table for a completed scope
        /// </summary>
        /// <param name="scopeId"></param>
        /// <param name="dataProvider"></param>
        private void CreateCompletedScopeInDB(string scopeId, IDataProvider dataProvider)
        {
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AddCompletedScope");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedDateTime = lookup.GetTimestampFunc(DateTime.Now.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (dataProvider.DatabaseType)
                {
                    case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@completedscopeid", "'" + scopeId + "'");
                        sql = sql.Replace("@modified", modifiedDateTime);

                        break;

                    case DatabaseType.Oracle:
                        sql = sql.Replace(":completedscopeid", "'" + scopeId + "'");
                        sql = sql.Replace(":modified", modifiedDateTime);

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        private SchemaInfo ParseSchemaId(string schemaId)
        {
            string[] strings = schemaId.Split(' ');
            string schemaName = strings[0].Trim();
            string schemaVersion = strings[1].Trim();

            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;
            schemaInfo.Version = schemaVersion;

            return schemaInfo;
        }

        private WorkflowStatus ConvertWorkflowStatus(TrackingWorkflowEvent workflowEvent)
        {
            WorkflowStatus status;

            switch (workflowEvent)
            {
                case TrackingWorkflowEvent.Aborted:
                case TrackingWorkflowEvent.Terminated:
                case TrackingWorkflowEvent.Exception:
                    status = WorkflowStatus.Terminated;
                    break;

                case TrackingWorkflowEvent.Completed:
                    status = WorkflowStatus.Completed;
                    break;

                case TrackingWorkflowEvent.Created:
                    status = WorkflowStatus.Created;
                    break;

                case TrackingWorkflowEvent.Idle:
                case TrackingWorkflowEvent.Loaded:
                case TrackingWorkflowEvent.Persisted:
                case TrackingWorkflowEvent.Resumed:
                case TrackingWorkflowEvent.Started:
                case TrackingWorkflowEvent.Unloaded:
                    status = WorkflowStatus.Running;
                    break;

                case TrackingWorkflowEvent.Suspended:
                    status = WorkflowStatus.Suspended;
                    break;

                default:
                    status = WorkflowStatus.Completed;
                    break;
            }

            return status;
        }
	}
}