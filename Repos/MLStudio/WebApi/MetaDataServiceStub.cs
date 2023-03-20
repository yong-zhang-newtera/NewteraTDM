using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.MLStudio.ViewModel;

namespace Newtera.MLStudio.WebApi
{
    public class MetaDataServiceStub : WebApiServiceBase
    {
        public MetaDataServiceStub()
        {
        }

        public SchemaInfo[] GetSchemaInfos()
        {
            string result = GetAPICall("api/metaDataService/GetSchemaInfos");

            SchemaInfo[] array = new SchemaInfo[] { };

            try
            {
                array = JsonConvert.DeserializeObject<SchemaInfo[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }


        public string GetClassTree(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetClassTree?connectionStr=" + connectionStr);

            return result;
        }

        public string GetXMLSchemas(string connectionStr, string className)
        {
            string result = GetAPICall("api/metaDataService/GetXMLSchemas/?connectionStr=" + connectionStr + "&className=" + className);

            return result;
        }
    }
}
