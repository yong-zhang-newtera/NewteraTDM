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
    using System.IO;
    using System.Threading;
    using System.Security.Principal;
	using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Newtera.Common.Core;
    using Newtera.Common.Config;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.FullText;
    using Newtera.Data;
    using Newtera.WebForm;

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
        public void Execute(IndexingContext context)
        {
            _context = context;

            try
            {
                switch (context.OperationType)
                {
                    case Common.MetaData.Events.OperationType.Insert:

                        DeleteIndex();

                        CreateIndex();

                        CreateDocuments();

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

        private void DeleteIndex()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            if (ElasticSearchWrapper.IsIndexExist(schemaName, className))
            {
                ElasticSearchWrapper.DeleteIndex(schemaName, className);
            }
        }

        private void CreateIndex()
        {
            string schemaName = _context.MetaData.SchemaInfo.Name;
            string className = _context.ClassElement.Name;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);
                InstanceView instanceView = new InstanceView(dataView);

                JObject properties = new JObject();
                JObject property;

                foreach (InstanceAttributePropertyDescriptor pd in instanceView.GetProperties(null))
                {
                    // add a property to the properties
                    property = new JObject();
                    string dataType = getFieldType(pd);
                    property.Add("type", dataType);
                    if (dataType == "date")
                    {
                        property.Add("format", "yyyy-MM-dd HH:mm:ss || yyyy-MM-dd || yyyy/MM/dd || MM/dd/yyyy");
                    }

                    if (pd.IsGoodForFullTextSearch || pd.IsGoodForSearchSuggester)
                    {
                        // add the catch_all field for full-text search purpose
                        property.Add("copy_to", COPY_TO_PROPERTY);
                    }

                    properties.Add(pd.Name, property);
                }

                // add full_text property
                property = new JObject();
                property.Add("type", "text");
                string analyzer = ElasticsearchConfig.Instance.Analyzer;
                if (!string.IsNullOrEmpty(analyzer))
                    property.Add("analyzer", analyzer);
                string searchAnalyzer = ElasticsearchConfig.Instance.SearchAnalyzer;
                if (!string.IsNullOrEmpty(searchAnalyzer))
                    property.Add("search_analyzer", searchAnalyzer);
                properties.Add(COPY_TO_PROPERTY, property);

                // add completion property
                property = new JObject();
                property.Add("type", "completion");
                if (!string.IsNullOrEmpty(analyzer))
                    property.Add("analyzer", analyzer);
                if (!string.IsNullOrEmpty(searchAnalyzer))
                    property.Add("search_analyzer", searchAnalyzer);
                properties.Add(SUGGEST_PROPERTY, property);

                JObject mappingObj = CreateMappingObj(properties);

                var jsonString = JsonConvert.SerializeObject(mappingObj, Newtonsoft.Json.Formatting.Indented);
                //ErrorLog.Instance.WriteLine("mapping=" + jsonString);

                ElasticSearchWrapper.CreateDocumentMapping(schemaName, className, mappingObj);
            }
        }

        private string getFieldType(InstanceAttributePropertyDescriptor pd)
        {
            string type = "text";

            switch (pd.DataType)
            {
                case DataType.BigInteger:

                    type = "long";
                    break;

                case DataType.Boolean:

                    type = "boolean";
                    break;

                case DataType.Byte:

                    type = "byte";
                    break;

                case DataType.Date:

                    type = "date";
                    break;

                case DataType.DateTime:

                    type = "date";
                    break;

                case DataType.Decimal:

                    type = "float";
                    break;

                case DataType.Double:

                    type = "double";
                    break;

                case DataType.Float:

                    type = "float";
                    break;

                case DataType.Integer:

                    type = "integer";
                    break;

                case DataType.String:

                    type = "text";
                    break;

                case DataType.Text:

                    type = "text";
                    break;

            }

            return type;
        }

        // create a document
        private void CreateDocuments()
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
                                    //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                                    //ErrorLog.Instance.WriteLine("doc=" + jsonString);
                                    documents.Add(instance);
                                }
                            }

                            ElasticSearchWrapper.CreateDocumentIndexes(schemaName, className, documents);
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

            JObject docObj = new JObject();

            mappings.Add("_doc", docObj);

            docObj.Add("properties", properties);

            return mappingsObj;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}