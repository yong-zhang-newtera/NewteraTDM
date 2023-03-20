/*
* @(#)DeleteExecutor.cs
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
    using System.Resources;
    using System.IO;
    using System.Text;

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
    using Newtera.Common.Attachment;
	using Newtera.Server.DB;
    using Newtera.Server.FullText;
    using Newtera.Server.Attachment;
    using Newtera.Server.Logging;

	/// <summary>
	/// Perform deletion-related actions, such as, generating SQL statement(s)
	/// for insertion, and execute the SQL statements.
	/// </summary>
	/// <version>  	1.0.0 18 Jul 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteExecutor : Executor
	{
        private VDocument _doc;
		private Interpreter _interpreter;
        private WorkflowModelAdapter _wfAdapter;
        private ResourceManager _resources;
		
		/// <summary>
		/// Initiating a DeleteExecutor.
		/// </summary>
		/// <param name="metaData">the meta data model</param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder</param>
		/// <param name="queries">the xqueries that retrieve back the inserted nodes</param>
		/// <param name="interpreter">the interpreter to run the xqueries.</param>
		public DeleteExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Interpreter interpreter) : base(metaData, dataProvider, builder)
		{
			_interpreter = interpreter;
            _wfAdapter = null;
            _doc = doc;
            _resources = new ResourceManager(this.GetType());
		}
		
		/// <summary>
		/// This method handles all the details of building SQL statements,
		/// executing the SQLs.
		/// </summary>
		/// <param name="instanceNodes">the instances to be deleted.</param>
		/// <returns> a string consisting of obj_ids of deleted instances.</returns>
		public string Execute(ICollection instanceNodes)
		{
			string objIdStr = null;
			ClassEntity baseClass = null;
			CustomPrincipal principal = null;
			XmlElement currentInstance = null;
			IDbConnection con = null;
			IDbTransaction tran = null;
			IDbCommand cmd = null;
            AttachmentInfoCollection attachmentInfos;
            AttachmentInfoCollection allAttachmentInfos = new AttachmentInfoCollection();
            StringCollection _deletedObjIds = new StringCollection();
            StringBuilder builder;
            string deleteSql = "";

            IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();

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
                    string currentObjId = instance.ObjId;

					if (objIdStr == null)
					{
                        objIdStr = currentObjId;
					}
					else
					{
                        objIdStr = objIdStr + " " + currentObjId;
					}

                    // Make sure the instance has not been bound to any workflow instances that
                    // have are still running. If so, throw an exception
                    if (IsBoundToRunningWFInstance(currentObjId))
                    {
                        throw new BindingInstanceException(_resources.GetString("HasBinding"));
                    }
					
					baseClass = CreateFullBlownClass(instance);

                    // create an event context for the deleted instance
                    if (!_doc.Interpreter.HasGlobalTransaction &&
                        NeedToRaiseEvents &&
                        _metaData.EventManager.HasEvents(baseClass.SchemaElement))
                    {
                        EventContext eventContext = new EventContext(Guid.NewGuid().ToString(), this._metaData, baseClass.SchemaElement,
                            currentObjId, OperationType.Delete);
                        this._eventContexts.Add(eventContext);
                    }

                    if (Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled)
                    {
                        // add the context to update the external full text search index
                        IndexingContext indexingContext = new IndexingContext(this._metaData, baseClass.SchemaElement, currentObjId, OperationType.Delete);
                        this._indexingContexts.Add(indexingContext);
                    }
					
					/*
					* Check if the principal has permission to delete this instance. If the matched rules have
					* conditions defined, checking result may depend on values of the
					* instance and its related instances, therefore, we need to ask
					* interpreter to retrieve the document that contains the instance and
					* its related instances.
					*/
					Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, baseClass.SchemaElement, XaclActionType.Delete);
					
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
						// tell the SearchExecutor to keep a copy of VDocument at principla instance
						principal.NeedCurrentDocumentStatus = true;
						principal.CurrentConnection = con;
                        principal.CurrentTransaction = tran;
                        string query = BuildInstanceQuery(instance.ObjId, baseClass.Name, _doc, conclusion); ;
						_interpreter.Reset();
						_interpreter.Query(query);
						
						// get the current instance from principal instance
						currentInstance = FindInstance((VDocument) principal.CurrentDocument, instance.ObjId);
						
						// unset the principal
						principal.NeedCurrentDocumentStatus = false;
						principal.CurrentDocument = null;
						principal.CurrentConnection = null;
                        principal.CurrentTransaction = null;
						
						// check the permission with instance
						if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, baseClass, XaclActionType.Delete, currentInstance))
						{
                            throw new PermissionViolationException(_resources.GetString("NoPermissions"));
						}
					}
					else if (conclusion.Permission == XaclPermissionType.Deny)
					{
                        throw new PermissionViolationException(_resources.GetString("NoPermissions"));
					}

                    // remember the attachment info associated with the deleted instance
                    int startRow = 0;
                    int pageSize = 50;
                    int count = repository.GetAttachmentInfosCount(AttachmentType.Instance, currentObjId, _metaData.SchemaModel.SchemaInfo.ID);
                    while (startRow < count)
                    {
                        attachmentInfos = repository.GetAttachmentInfos(AttachmentType.Instance, currentObjId,
                            _metaData.SchemaModel.SchemaInfo.ID, startRow, pageSize);
 
                        foreach (AttachmentInfo aInfo in attachmentInfos)
                        {
                            allAttachmentInfos.Add(aInfo);
                        }

                        startRow += pageSize;
                    }

                    // remember the deleted objId
                    _deletedObjIds.Add(currentObjId);
					
					StringCollection sqls = _builder.GenerateDeletes(baseClass, instance);
					
					/*
					* Deletion starts from the bottom class
					*/
                    builder = new StringBuilder();
					foreach (string sql in sqls)
					{
                        deleteSql = sql; // in case of an exception to include sql in the message
						cmd.CommandText = sql;
						cmd.ExecuteNonQuery();

                        SQLPrettyPrint.printSql(sql);

                        builder.Append(sql).Append(";"); // for log purpose
					}

                    index++;

                    // delete the image files associated with the deleted instances
                    if (baseClass.HasImageAttributes)
                    {
                        // image file name has pattern of class-attribte-objId.*
                        string pattern = "*-" + currentObjId + ".*";
                        // Note, unlike attachment file, image file is stored at base dir
                        string[] fileNames = Directory.GetFiles(NewteraNameSpace.GetAttachmentDir(),
                            pattern);
                        foreach (string fileName in fileNames)
                        {
                            try
                            {
                                File.Delete(fileName);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    // log the event if the particular log is on 
                    if (LoggingChecker.Instance.IsLoggingOn(_metaData, baseClass.SchemaElement, LoggingActionType.Delete))
                    {
                        LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Delete, _metaData.SchemaInfo.NameAndVersion,
                            baseClass.SchemaElement.Name, baseClass.SchemaElement.Caption, currentObjId, builder.ToString());

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
			catch (Exception ex)
			{
                if (!_doc.Interpreter.HasGlobalTransaction && tran != null)
                {
                    tran.Rollback();
                }

                if (ex is System.Data.SqlClient.SqlException || ex is System.Data.OracleClient.OracleException)
                {
                    if (!string.IsNullOrEmpty(deleteSql))
                    {
                        throw new Exception(ex.Message + ";\n The SQL is " + deleteSql + "\n");
                    }
                    else
                    {
                        throw new Exception(ex.Message + ";" + ex.StackTrace);
                    }
                }
                else
                {
                    throw ex;
                }
			}
			finally
			{
                if (!_doc.Interpreter.HasGlobalTransaction && con != null)
                {
                    con.Close();
                }
			}

            // the instances have been deleted successfully,
            // Delete the attachment files that are associated with the instances
            // instance attachment infos associated with the instance are deleted automatically
            // when the instance is deleted through cascade deletion
            if (allAttachmentInfos.Count > 0)
            {
                DeleteAttachmens(allAttachmentInfos);
            }
			
			return objIdStr;
		}

        /// <summary>
        /// Gets the information indicating whether the data instance has been bound to
        /// any running workflow instance.
        /// </summary>
        /// <param name="objId">The obj id of data instance to be deleted.</param>
        /// <returns>true if the data instance</returns>
        private bool IsBoundToRunningWFInstance(string objId)
        {
            bool status = false;

            if (_wfAdapter == null)
            {
                _wfAdapter = new WorkflowModelAdapter(_dataProvider);
            }

            if (_wfAdapter.GetBindingInfoByObjId(objId) != null)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Delete the attachment files associated with the deleted objects
        /// </summary>
        /// <param name="attachmentInfos">The attachment infos</param>
        private void DeleteAttachmens(AttachmentInfoCollection attachmentInfos)
        {
            string attachmentDir = NewteraNameSpace.GetAttachmentDir();
            string path;

            foreach (AttachmentInfo attachmentInfo in attachmentInfos)
            {
                // delete the attachment file
                path = attachmentDir + attachmentInfo.ID;
                // the attachmen file could be saved in a sub dir (after version 5.2.0)
                if (!File.Exists(path))
                {
                    path = NewteraNameSpace.GetAttachmentSubDir(attachmentInfo.CreateTime) + attachmentInfo.ID;
                }

                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }
	}
}