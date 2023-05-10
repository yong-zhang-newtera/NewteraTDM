/*
* @(#)BatchDocumentsRunner.cs
*
* Copyright (c) 2003-2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ElasticSearchIndexer
{
	using System;
	using System.Xml;
    using System.Data;
    using System.Threading.Tasks;
    using System.Collections.Generic;
 
    using Newtonsoft.Json.Linq;

    using Newtera.Common.Core;
    using Newtera.Common.Config;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.FullText;
    using Newtera.Data;
    using Newtera.WebForm;
    using Newtonsoft.Json;

    /// <summary> 
    /// Create a batch document indexes
    /// </summary>
    /// <version>  	1.0.0 22 Nov 2017</version>
    public class BatchDocumentsRunner : IIndexingRunner
	{
        private const int PAGE_SIZE = 100;
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string COPY_TO_PROPERTY = "catch_all";
        private const string SUGGEST_PROPERTY = "suggest";
        private IndexingContext _context;


        /// <summary>
        /// Execute the runner
        /// </summary>
        public async Task Execute(IndexingContext context)
        {
            _context = context;

            try
            {
                switch (context.OperationType)
                {
                    case Common.MetaData.Events.OperationType.Insert:

                        await DeleteIndex();

                        await CreateDocuments ();

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
        }

        private async Task DeleteIndex()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            if (ElasticSearchWrapper.IsIndexExist(schemaName, className))
            {
                await ElasticSearchWrapper.DeleteIndex(schemaName, className);
            }
        }

        // create a document
        private async Task CreateDocuments()
        {
            List<JObject> documents = null;
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);
                string query = dataView.SearchQuery;

                CMDataReader dataReader = null;
                XmlReader xmlReader;
                XmlDocument doc;
                DataSet ds;
                int count;

                try
                {
                    CMCommand cmd = con.CreateCommand();
                    cmd.PageSize = PAGE_SIZE;
                    cmd.CommandText = query;

                    // use Default behavior so that when closing CMDataReader, the
                    // connection won't be closed
                    dataReader = cmd.ExecuteReader();

                    // export data to the file in pages
                    while (dataReader.Read())
                    {
                        doc = dataReader.GetXmlDocument();

                        xmlReader = new XmlNodeReader(doc);
                        ds = new DataSet();
                        ds.ReadXml(xmlReader);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            InstanceView instanceView = new InstanceView(dataView, ds);

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;

                            count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                            documents = new List<JObject>();
                            JObject instance;
                            for (int row = 0; row < count; row++)
                            {
                                instanceEditor.EditInstance.SelectedIndex = row; // set the cursor

                                instance = instanceEditor.ConvertToIndexingDocument(); // returned instances for elastic search indexing

                                if (instance != null)
                                {
                                    var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                                    ErrorLog.Instance.WriteLine("doc=" + jsonString);
                                    documents.Add(instance);
                                }
                            }

                            await ElasticSearchWrapper.CreateDocumentIndexes(schemaName, className, documents);
                        }
                        else
                        {
                            // got an empty result
                            break;
                        }
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }
                }
            }
        }

        private JObject CreateMappingObj(JObject properties)
        {
            JObject mappingsObj = new JObject();

            JObject mappings = new JObject();

            mappingsObj.Add("mappings", mappings);

            mappings.Add("properties", properties);

            return mappingsObj;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}