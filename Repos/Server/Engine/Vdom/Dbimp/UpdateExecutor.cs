/*
* @(#)UpdateExecutor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Xml;
    using System.Threading;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
    using System.Resources;
    using System.Text;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Workflow;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;
    using Newtera.Common.MetaData.Logging;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Principal;
	using Newtera.Server.DB;
    using Newtera.Server.FullText;
    using Newtera.Server.Logging;

	/// <summary>
	/// This class perform update-related actions, such as, generating UPDATE SQL statement(s),
	/// and execute the SQL statements.
	/// </summary>
	/// <version>  	1.0.0 18 Jul 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class UpdateExecutor : Executor
	{
        private Interpreter _interpreter;
        private ResourceManager _resources;
        private VDocument _doc;
		
		/// <summary>
		/// Initiating an instance of UpdateExecutor class.
		/// </summary>
		/// <param name="metaData">the meta data model</param>
		/// <param name="dataProvider">the database provider.</param>
		/// <param name="builder">the sql builder </param>
		/// <param name="documents">that contains original instances to the updating instances.</param>
        public UpdateExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Interpreter interpreter)
            : base(metaData, dataProvider, builder)
		{
            _doc = doc;
            _interpreter = interpreter;
            _resources = new ResourceManager(this.GetType());
		}
		
		/// <summary>
		/// Handles all the details of building SQL statements,
		/// and executing the SQLs.
		/// </summary>
		/// <param name="instanceNodes">the instances to be updated.</param>
		/// <returns> a string consisting of obj_ids of updated instances.</returns>
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
					
					if (objIdStr == null)
					{
						objIdStr = instance.ObjId;
					}
					else
					{
						objIdStr = objIdStr + " " + instance.ObjId;
					}
					
					baseClass = CreateFullBlownClass(instance);

                    // create an event context for the update instance
                    if (!_doc.Interpreter.HasGlobalTransaction &&
                        NeedToRaiseEvents &&
                        _metaData.EventManager.HasEvents(baseClass.SchemaElement))
                    {
                        EventContext eventContext = new EventContext(Guid.NewGuid().ToString(), this._metaData, baseClass.SchemaElement,
                            objIdStr, OperationType.Update, instance.GetElementNames(), Thread.CurrentPrincipal.Identity.Name);
                        this._eventContexts.Add(eventContext);
                    }

                    if (Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled)
                    {
                        // add the context to update the external full text search index
                        IndexingContext indexingContext = new IndexingContext(this._metaData, baseClass.SchemaElement, objIdStr, OperationType.Update);
                        this._indexingContexts.Add(indexingContext);
                    }

                    // set id(s) of referenced object(s) by the instance
                    SetReferencedIds(baseClass, instance, false, _doc);

                    StringBuilder actionData = new StringBuilder();
					
					SQLActionCollection actions = _builder.GenerateUpdates(baseClass, instance, currentInstance, actionData);
					
                    builder = new StringBuilder();
					foreach (SQLAction action in actions)
					{
						if (action.Type == SQLActionType.Update)
						{
							sql = action.Statement.ToSQL();
							SQLPrettyPrint.printSql(sql);
							cmd.CommandText = sql;
							cmd.ExecuteNonQuery();
                            builder.Append(sql).Append("; \n"); // for log purpose
						}
					}

					// write over-size array data to the corresponding CLOB columns
					IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
					foreach (SQLAction action in actions)
					{
						if (action.Type == SQLActionType.WriteClob)
						{
							action.ObjId = instance.ObjId;

							// Use the database-specific ClobDAO to write to a clob
							clobDAO.WriteClob(cmd, action.Data, action.TableName,
								action.ColumnName, action.ObjId);
						}
					}

                    if (!_interpreter.HasGlobalTransaction)
                    {
                        /*
                         * Check if the current principal has permission to write this instance. If the matched rules have
                         * conditions defined, checking result may depend on values of the
                         * instance and its related instances, therefore, we need to ask
                         * interpreter to retrieve the document that contains the instance and
                         * its related instances.
                         */
                        Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, baseClass.SchemaElement, XaclActionType.Write);

                        /*
                        * The conclusion may have three answers, unconditional deny,
                        * unconditional grant, conditional deny, and conditional grant.
                        * Later two depend on the condition of the instances, we need to
                        * get it.
                        */
                        if (conclusion.Permission == XaclPermissionType.ConditionalDeny ||
                            conclusion.Permission == XaclPermissionType.ConditionalGrant)
                        {
                            principal = (CustomPrincipal)Thread.CurrentPrincipal;
                            // tell the SearchExecutor to keep a copy of VDocument at the principal
                            principal.NeedCurrentDocumentStatus = true;
                            // SearchExecutor has to use the same conection to find the 
                            // instance just modified since it has not been committed
                            principal.CurrentConnection = con;
                            principal.CurrentTransaction = tran;
                            string query = BuildInstanceQuery(instance.ObjId, baseClass.Name, _doc, conclusion); ;
                            _interpreter.Reset();
                            _interpreter.Query(query);

                            // get the current instance from principal
                            currentInstance = FindInstance((VDocument)principal.CurrentDocument, instance.ObjId);
                            // unset the principal data
                            principal.NeedCurrentDocumentStatus = false;
                            principal.CurrentDocument = null;
                            principal.CurrentConnection = null;
                            principal.CurrentTransaction = null;

                            // check the permission with retrieved instance
                            if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, baseClass, XaclActionType.Write, currentInstance))
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
                    }

					index++;

                    // log the event if the particular log is on 
                    if (LoggingChecker.Instance.IsLoggingOn(_metaData, baseClass.SchemaElement, LoggingActionType.Write))
                    {
                        LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Write, _metaData.SchemaInfo.NameAndVersion,
                            baseClass.SchemaElement.Name, baseClass.SchemaElement.Caption, instance.ObjId, actionData.ToString());

                        LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                    }

                    ClearServerCache(baseClass.Name); // A HACK, should be removed when AfterEdit action is implemented for each class
				}
				
                // commit the local transaction
                if (!_interpreter.HasGlobalTransaction)
                {
                    tran.Commit();
                }
			}
			catch (Exception e)
			{
                ErrorLog.Instance.WriteLine(e.Message + "\n" + e.StackTrace);

                if (!_doc.Interpreter.HasGlobalTransaction && tran != null)
                {
                    // rollback a local transaction
                    tran.Rollback();
                }

                if (e is System.Data.SqlClient.SqlException || e is System.Data.OracleClient.OracleException)
                {
                    if (!string.IsNullOrEmpty(sql))
                    {
                        throw new Exception(e.Message + ";\n The SQL is " + sql + "\n" + e.StackTrace);
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
	}
}