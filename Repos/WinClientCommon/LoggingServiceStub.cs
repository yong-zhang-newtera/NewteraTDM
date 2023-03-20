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
    public class LoggingServiceStub : WebApiServiceBase
    {
        public LoggingServiceStub()
        {
        }

        public string[] GetLoggingMetaData()
        {
            string result = GetAPICall("api/loggingService/GetLoggingMetaData");

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public System.Xml.XmlNode ExecuteLoggingQuery(string query)
        {
            string result = PostAPICall("api/loggingService/ExecuteLoggingQuery", query, "text/plain");

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

        public int ExecuteLoggingCount(string query)
        {
            string result = PostAPICall("api/loggingService/ExecuteLoggingCount", query, "text/plain");
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


    }
}
