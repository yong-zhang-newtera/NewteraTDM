/*
* @(#)InsertExecutor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
    using System.Text;
    using System.Resources;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Workflow;
	using Newtera.Common.MetaData.XaclModel.Processor;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.Logging;
	using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Events;
	using Newtera.Server.DB;
    using Newtera.Server.FullText;
    using Newtera.Server.Logging;

	/// <summary>
	/// This class perform insertion-related actions, such as, assign an unique object id to
	/// an instance, get ids of referenced objects by the instance, generating SQL statement(s)
	/// for insertion, and execute the SQL statements.
	/// </summary>
	/// <version>  	1.0.0 18 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class InsertExecutor : Executor
	{
		private KeyGenerator _idGenerator; // the obj id generator
		private Interpreter _interpreter;
        private VDocument _doc;
        private ResourceManager _resources;

		/// <summary>
		/// Initiating an instance of InsertExecutor class
		/// </summary>
		/// <param name="metaData">the meta data model </param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder </param>
		/// <param name="doc">the document to which to insert the instance</param>
		/// <param name="interpreter">the interpreter to run the xqueries. </param>
		public InsertExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Interpreter interpreter):base(metaData, dataProvider, builder)
		{
			_idGenerator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ObjId, metaData.SchemaInfo);
			_interpreter = interpreter;
            _doc = doc;
            _resources = new ResourceManager(this.GetType());
		}
		
		/// <summary>
		/// Handles all the details of getting object ids, building SQL statements,
		/// executing the SQLs.
		/// </summary>
		/// <param name="instanceNodes">the instances to be inserted.</param>
		/// <returns>
		/// a string that consists of obj_id(s) for the inserted instance,
		/// separated by space.
		/// </returns>
		public string Execute(ICollection instanceNodes)
		{
			string objIdStr = null;
			ClassEntity baseClass;
			CustomPrincipal principal = null;
			XmlElement currentInstance = null;
			IDbConnection con = null;
			IDbTransaction tran = null;
			IDbCommand cmd = null;
            StringBuilder builder;
            string sql = "";
			
			try
			{
                if (_doc.Interpreter.HasGlobalTransaction)
                {
                    // global transaction
                    con = _doc.Interpreter.IDbConnection;
                    tran = _doc.Interpreter.IDbTransaction;
                }
                else
                {
                    // local transaction
                    con = _dataProvider.Connection;
                    tran = con.BeginTransaction();
                }

                cmd = con.CreateCommand();
                cmd.Transaction = tran;

				int index = 0;
				foreach (XNode node in instanceNodes)
				{
					Instance instance = new Instance((XmlElement) node.ToNode());
					
					baseClass = CreateFullBlownClass(instance);
					
					// assign an unique object id to the instance
                    string assignedId = AssignObjID(instance, baseClass);
					if (objIdStr == null)
					{
						objIdStr = assignedId;
					}
					else
					{
						objIdStr = objIdStr + " " + assignedId;
					}

                    // create an event context for the inserted instance
                    if (!_doc.Interpreter.HasGlobalTransaction &&
                        NeedToRaiseEvents &&
                        _metaData.EventManager.HasEvents(baseClass.SchemaElement))
                    {
                        EventContext eventContext = new EventContext(Guid.NewGuid().ToString(), this._metaData, baseClass.SchemaElement,
                            objIdStr, OperationType.Insert);
                        this._eventContexts.Add(eventContext);
                    }

                    if (Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled)
                    {
                        // add the context to update the external full text search index
                        IndexingContext indexingContext = new IndexingContext(this._metaData, baseClass.SchemaElement, objIdStr, OperationType.Insert);
                        this._indexingContexts.Add(indexingContext);
                    }

                    // set id(s) of referenced object(s) by the instance
                    SetReferencedIds(baseClass, instance, true, _doc);

                    StringBuilder actionData = new StringBuilder();
					
					SQLActionCollection actions = _builder.GenerateInserts(baseClass, instance, actionData);
					
					/*
					* Execute the insert starting from root. Database will throw
					* an error if trying to insert from bottom
					*/	
                    builder = new StringBuilder();
					foreach (SQLAction action in actions)
					{
						if (action.Type == SQLActionType.Insert)
						{
							sql = action.Statement.ToSQL();
							SQLPrettyPrint.printSql(sql);
                            cmd.CommandText = sql;

							cmd.ExecuteNonQuery();
                            builder.Append(sql).Append(";"); // for log purpose
						}
					}

					// write large sized data to the corresponding CLOB columns
					// this has to be executed after the record(s) have been created
					IClobDAO clobDAO = ClobDAOFactory.Instance.Create(this._dataProvider);
					foreach (SQLAction action in actions)
					{
						if (action.Type == SQLActionType.WriteClob)
						{
							action.ObjId = assignedId;

							// Use the database-specific ClobDAO to write to a clob
							clobDAO.WriteClob(cmd, action.Data, action.TableName,
								action.ColumnName, action.ObjId);
						}
					}
					
					/*
					* Check if the current principal has permission to create this instance. If the matched rules have
					* conditions defined, checking result may depend on values of the
					* instance and its related instances, therefore, we need to ask
					* interpreter to retrieve the document that contains the instance and
					* its related instances.
					*/
					Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, baseClass.SchemaElement, XaclActionType.Create);
					
					/*
					* The conclusion may have three answers, unconditional deny,
					* unconditional grant, conditional deny, and conditional grant.
					* Later two depend on the condition of the instances, we need to
					* get it.
					*/
					if (conclusion.Permission == XaclPermissionType.ConditionalDeny ||
						conclusion.Permission == XaclPermissionType.ConditionalGrant)
					{
						principal = (CustomPrincipal) Thread.CurrentPrincipal;
						// tell the SearchExecutor to keep a copy of VDocument at the principal
						principal.NeedCurrentDocumentStatus = true;
                        // SearchExecutor has to use the same conection to find the 
                        // instance just added since it has not been committed
						principal.CurrentConnection = con;
                        principal.CurrentTransaction = tran;
                        string query = BuildInstanceQuery(instance.ObjId, baseClass.Name, _doc, conclusion); ;
						_interpreter.Reset();
						_interpreter.Query(query);
						
						// get the current instance from principal
						currentInstance = FindInstance((VDocument) principal.CurrentDocument, instance.ObjId);
						// unset the principal data
						principal.NeedCurrentDocumentStatus = false;
						principal.CurrentDocument = null;
						principal.CurrentConnection = null;
                        principal.CurrentTransaction = null;
						
						// check the permission with instance which could be null
						if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, baseClass, XaclActionType.Create, currentInstance))
						{
                            // throw an exception will rollback the inserted record
                            throw new PermissionViolationException(_resources.GetString("NoPermissions"));
						}
					}
					else if (conclusion.Permission == XaclPermissionType.Deny)
					{
                        // throw an exception will rollback the inserted record
                        throw new PermissionViolationException(_resources.GetString("NoPermissions"));
					}

					index++;

                    // log the event if the particular log is on 
                    if (LoggingChecker.Instance.IsLoggingOn(_metaData, baseClass.SchemaElement, LoggingActionType.Create))
                    {
                        LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Create, _metaData.SchemaInfo.NameAndVersion,
                            baseClass.SchemaElement.Name, baseClass.SchemaElement.Caption, assignedId, actionData.ToString());

                        LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                    }

                    ClearServerCache(baseClass.Name); // A HACK, should be removed when AfterEdit action is implemented for each class
				}
				
				// commit the local transaction
                if (!_doc.Interpreter.HasGlobalTransaction)
                {
                    tran.Commit();
                }
			}
			catch (Exception e)
			{
                ErrorLog.Instance.WriteLine(e.Message + "\n" + e.StackTrace);
                try
                {
                    if (!_doc.Interpreter.HasGlobalTransaction && tran != null)
                    {
                        // rollback a local transaction
                        tran.Rollback();
                    }
                }
                catch (Exception)
                {
                    // ignore the rollback exception
                }

                if (e is System.Data.SqlClient.SqlException || e is System.Data.OracleClient.OracleException)
                {
                    if (!string.IsNullOrEmpty(sql))
                    {
                        throw new Exception(e.Message +  ";\n The SQL is " + sql + "\n" + e.StackTrace);
                    }
                    else
                    {
                        throw new Exception(e.Message + ";" + e.StackTrace);
                    }
                }
                else
                {
                    throw e;
                }
			}
			finally
			{
                if (!_doc.Interpreter.HasGlobalTransaction && con != null)
                {
                    con.Close();
                }
			}
			
			return objIdStr;
		}
		
		/// <summary>
		/// Assign an unique object id to the instance.
		/// </summary>
		/// <param name="instance">the instance element.</param>
		/// <returns> the assigned obj_id.</returns>
		protected string AssignObjID(Instance instance, ClassEntity baseClass)
		{
			string objId = _idGenerator.NextKey().ToString();
			
			instance.ObjId = objId;
			
			return objId;
		}
		
		/// <summary>
		/// Set an assigned obj_id to the query as a parameter.
		/// </summary>
		/// <param name="template">the query template.</param>
		/// <param name="obj_id">the assigned object id.</param>
		/// <returns> the instance query.</returns>
		private string GetInstanceQuery(string template, string obj_id)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			string wildcard = "??"; // Hack use of ?? as wildcard
			
			int pos = template.IndexOf(wildcard);
			if (pos >= 0)
			{
				buf.Append(template.Substring(0, (pos) - (0))).Append(obj_id);
				buf.Append(template.Substring(pos + wildcard.Length));
			}
			else
			{
				buf.Append(template);
			}
			return buf.ToString();
		}
	}
}