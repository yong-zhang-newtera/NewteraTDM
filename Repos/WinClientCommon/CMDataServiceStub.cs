using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WinClientCommon
{
    public class CMDataServiceStub : WebApiServiceBase
    {
        public CMDataServiceStub()
        {
        }

        public System.Xml.XmlNode ExecuteQuery(string connectionStr, string query)
        {
            string result = PostAPICall("api/appDataService/ExecuteQuery?connectionStr=" + connectionStr, query, "text/plain");

            if (!string.IsNullOrEmpty(result))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(result);
                return doc;
            }
            else
            {
                return null;
            }
        }

        public string BeginQuery(string connectionStr, string query, int pageSize)
        {
            return PostAPICall("api/appDataService/BeginQuery/" + pageSize + "?connectionStr=" + connectionStr, query, "text/plain");
        }

        public System.Xml.XmlNode GetNextResult(string connectionStr, string queryId)
        {
            string result = GetAPICall("api/appDataService/GetNextResult/" + queryId + "?connectionStr=" + connectionStr);

            if (!string.IsNullOrEmpty(result))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(result);
                return doc;
            }
            else
            {
                return null;
            }
        }

        public void EndQuery(string queryId)
        {
            GetAPICall("api/appDataService/EndQuery/" + queryId);
        }

        public string ExecuteUpdateQuery(string connectionStr, string query, bool needToRaiseEvents)
        {
            return PostAPICall("api/appDataService/ExecuteUpdateQuery/" + needToRaiseEvents + "?connectionStr=" + connectionStr, query, "text/plain");
        }

        public string ExecuteValidatingQuery(string connectionStr, string query)
        {
            return PostAPICall("api/appDataService/ExecuteValidatingQuery?connectionStr=" + connectionStr, query, "text/plain");
        }

        public int DeleteAllInstances(string connectionStr, string className)
        {
            string result = PostAPICall("api/appDataService/DeleteAllInstances/" + className + "?connectionStr=" + connectionStr, null, null);
            try
            {
                int count = int.Parse(result);
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public System.Xml.XmlNode GenerateXmlDoc(string connectionStr, string xmlSchemaName, string query, string baseClassName)
        {
            string result = PostAPICall("api/appDataService/GenerateXmlDoc/" + xmlSchemaName + "/" + baseClassName + "?connectionStr=" + connectionStr, query, "text/plain");

            if (!string.IsNullOrEmpty(result))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(result);
                return doc;
            }
            else
            {
                return null;
            }
        }

        public void BuildFullTextIndex(string connectionStr, string className)
        {
            PostAPICall("api/appDataService/BuildFullTextIndex/" + className + "?connectionStr=" + connectionStr, null, null);
        }

        public string ExecuteUpdateQueries(string connectionStr, string[] queries)
        {
            string content = JsonConvert.SerializeObject(queries);
            return PostAPICall("api/appDataService/ExecuteUpdateQueries?connectionStr=" + connectionStr, content, "application/json");
        }

        public int ExecuteCount(string connectionStr, string query)
        {
            string result = PostAPICall("api/appDataService/ExecuteCount?connectionStr=" + connectionStr, query, "text/plain");
            try
            {
                int count = int.Parse(result);
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetInstanceCount(string connectionStr, string className)
        {
            string result = GetAPICall("api/appDataService/GetInstanceCount/" + className + "?connectionStr=" + connectionStr);
            try
            {
                int count = int.Parse(result);
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void ResetDataCache()
        {
            PostAPICall("api/appDataService/ResetDataCache", null, null);
        }

        public string ExecuteImportScripts(string connectionStr, string xmlString, int chunkIndex)
        {
            return PostAPICall("api/appDataService/ExecuteImportScripts/" + chunkIndex + "?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public string BeginImport(string connectionStr, string xquery)
        {
            return PostAPICall("api/appDataService/BeginImport?connectionStr=" + connectionStr, xquery, "text/plain");
        }

        public string ImportNext(string connectionStr, string importId, string className, string xmlString, int chunkIndex)
        {
            return PostAPICall("api/appDataService/ImportNext/" + importId  + "/" + className + "/" + chunkIndex + "?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public void EndImport(string importId)
        {
            PostAPICall("api/appDataService/EndImport/" + importId, null, null);
        }

        public bool IsExternalFullTextSearch()
        {
            string result = GetAPICall("api/appDataService/IsExternalFullTextSearch");

            return bool.Parse(result);
        }
    }
}
