/*
* @(#)SingleDocumentRunner.cs
*
* Copyright (c) 2003-2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ElasticSearchIndexer
{
	using System;
	using System.Xml;
    using System.Data;
    using System.Threading;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Data;
    using Newtera.WebForm;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.FullText;

    /// <summary> 
    /// Create or update a single document index
    /// </summary>
    /// <version>  	1.0.0 22 Nov 2017</version>
    public class SingleDocumentRunner : IIndexingRunner
	{
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private bool _isInProcess;
        private IndexingContext _context;

        /// <summary>
        /// Gets or sets the information indicating whether the index builder is in process
        /// </summary>
        public bool IsInProcess
        {
            get
            {
                return _isInProcess;
            }
            set
            {
                _isInProcess = value;
            }
        }

        /// <summary>
        /// Execute the runner
        /// </summary>
        public async Task Execute(IndexingContext context)
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            _isInProcess = true;

            _context = context;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                switch (context.OperationType)
                {
                    case Common.MetaData.Events.OperationType.Insert:

                        await CreateDocument();

                        break;

                    case Common.MetaData.Events.OperationType.Update:

                        await UpdateDocument();

                        break;

                    case Common.MetaData.Events.OperationType.Delete:

                        await DeleteDocument ();

                        break;

                    default:

                        break;
                }
            }
            catch (Exception ex)
            {
                // write the error message to the log
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;

                _isInProcess = false;
            }
        }

        // create a document in the index
        private async Task CreateDocument()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            if (ElasticSearchWrapper.IsIndexExist(schemaName, className))
            {
                // get the json document to add to the index
                JObject document = GetJsonDocument(schemaName, className, _context.ObjId);

                if (document != null)
                {
                    await ElasticSearchWrapper.CreateDocumentIndex(schemaName, className, _context.ObjId, document);
                }
                else
                {
                    throw new Exception("Unable to create a json document for the instance with objId " + _context.ObjId + " in class " + _context.ClassElement.Name);
                }
            }
        }

        // update a document in the index
        private async Task UpdateDocument()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            if (ElasticSearchWrapper.IsIndexExist(schemaName, className))
            {
                // get the json document to modify to the index
                JObject document = GetJsonDocument(schemaName, className, _context.ObjId);

                if (document != null)
                {
                    await ElasticSearchWrapper.UpdateDocumentIndex(schemaName, className, _context.ObjId, document);
                }
                else
                {
                    throw new Exception("Unable to create a json document for the instance with objId " + _context.ObjId + " in class " + _context.ClassElement.Name);
                }
            }
        }

        private async Task DeleteDocument()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            if (ElasticSearchWrapper.IsIndexExist(schemaName, className))
            {
                await ElasticSearchWrapper.DeleteDocumentIndex(schemaName, className, _context.ObjId);
            }
        }

        private JObject GetJsonDocument(string schemaName, string className, string objId)
        {
            JObject instance = null;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                // create an instance query
                string query = dataView.GetInstanceQuery(objId);

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);
                    InstanceEditor instanceEditor = new InstanceEditor();
                    instanceEditor.EditInstance = instanceView;
                    instance = instanceEditor.ConvertToIndexingDocument(); // convert to a json document for full-text search indexing
                }
            }

            return instance;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}