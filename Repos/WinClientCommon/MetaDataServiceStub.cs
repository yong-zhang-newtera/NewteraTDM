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

        public string[] GetMetaData(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetMetaData?connectionStr=" + connectionStr);

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


        public string GetSchemaModel(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetSchemaModel?connectionStr=" + connectionStr);

            return result;
        }

        public System.DateTime SetMetaData(string connectionStr, string[] xmlStrings)
        {
            string content = JsonConvert.SerializeObject(xmlStrings);


            string result = PostAPICall("api/metaDataService/SetMetaData?connectionStr=" + connectionStr, content, "application/json");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                throw new Exception(result);
            }

            return modifiedTime;
        }

        public System.DateTime SetSchemaModel(string connectionStr, string xmlSchema)
        {
            string result = PostAPICall("api/metaDataService/SetSchemaModel?connectionStr=" + connectionStr, xmlSchema, "application/xml");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                modifiedTime = DateTime.Now;
            }
            return modifiedTime;
        }

        public void FixSchemaModel(string connectionStr)
        {
            GetAPICall("api/metaDataService/FixSchemaModel?connectionStr=" + connectionStr);
        }

        public string GetDataViews(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetDataViews?connectionStr=" + connectionStr);

            return result;
        }

        public System.DateTime SetDataViews(string connectionStr, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetDataViews?connectionStr=" + connectionStr, xmlString, "application/xml");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                modifiedTime = DateTime.Now;
            }
            return modifiedTime;
        }

        public System.DateTime SetXaclPolicy(string connectionStr, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetXaclPolicy?connectionStr=" + connectionStr, xmlString, "application/xml");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                modifiedTime = DateTime.Now;
            }
            return modifiedTime;
        }

        public System.DateTime SetTaxonomies(string connectionStr, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetTaxonomies?connectionStr=" + connectionStr, xmlString, "application/xml");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                modifiedTime = DateTime.Now;
            }
            return modifiedTime;
        }

        public System.DateTime SetRules(string connectionStr, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetRules?connectionStr=" + connectionStr, xmlString, "application/xml");

            DateTime modifiedTime = DateTime.Now;
            try
            {
                modifiedTime = DateTime.Parse(result);
            }
            catch (Exception)
            {
                modifiedTime = DateTime.Now;
            }
            return modifiedTime;
        }

        public void DeleteMetaData(string connectionStr)
        {
            GetAPICall("api/metaDataService/DeleteMetaData?connectionStr=" + connectionStr);
        }

        public string GetMetaDataUpdateLog(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetMetaDataUpdateLog?connectionStr=" + connectionStr);

            return result;
        }

        public string GetTransformerId(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetTransformerId?connectionStr=" + connectionStr);

            return result;
        }

        public bool IsValueGeneratorExist(string valueGeneratorDef)
        {
            bool status = false;

            string result = PostAPICall("api/metaDataService/IsValueGeneratorExist", valueGeneratorDef, "text/plain");

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public string ValidateXQueryCondition(string connectionStr, string className, string condition)
        {
            string result = PostAPICall("api/metaDataService/ValidateXQueryCondition/" + className + "?connectionStr=" + connectionStr, condition, "text/plain");

            return result;
        }

        public bool IsValidCustomFunctionDefinition(string connectionStr, string functionDefinition)
        {
            bool status = false;

            string result = PostAPICall("api/metaDataService/IsValidCustomFunctionDefinition?connectionStr=" + connectionStr, functionDefinition, "text/plain");

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public System.Xml.XmlNode SearchXmlDataSource(string xquery)
        {
            string result = PostAPICall("api/metaDataService/SearchXmlDataSource", xquery, "text/plain");

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

        public string ValidateMethodCode(string connectionStr, string code, string schemaId, string instanceClassName)
        {
            string result = PostAPICall("api/metaDataService/ValidateMethodCode/" + schemaId + "/" + instanceClassName + "?connectionStr=" + connectionStr, code, "text/plain");

            return result;
        }

        public string GetDBARole(string connectionStr)
        {
            string result = GetAPICall("api/metaDataService/GetDBARole?connectionStr=" + connectionStr);

            return result;
        }

        public void SetDBARole(string connectionStr, string role)
        {
            GetAPICall("api/metaDataService/SetDBARole/" + role + "?connectionStr=" + connectionStr);
        }

        public string LockMetaData(string connectionStr)
        {
            return GetAPICall("api/metaDataService/LockMetaData?connectionStr=" + connectionStr);
        }

        public void UnlockMetaData(string connectionStr, bool forceUnlock)
        {
            GetAPICall("api/metaDataService/UnlockMetaData/" + forceUnlock + "?connectionStr=" + connectionStr);
        }

        public string GetSiteMapModels()
        {
            string result = GetAPICall("api/metaDataService/GetSiteMapModels");

            return result;
        }

        public void SetSiteMapModels(string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetSiteMapModels", xmlString, "application/xml");
        }

        public string GetSiteMap(string modelName)
        {
            string result = GetAPICall("api/metaDataService/GetSiteMap/" + modelName);

            return result;
        }

        public string GetSiteMapFromFile(string fileName)
        {
            string result = GetAPICall("api/metaDataService/GetSiteMapFromFile/" + fileName);

            return result;
        }

        public void SetSiteMap(string modelName, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetSiteMap/" + modelName, xmlString, "application/xml");
        }

        public string GetSideMenu(string modelName)
        {
            string result = GetAPICall("api/metaDataService/GetSideMenu/" + modelName);

            return result;
        }

        public string GetSideMenuFromFile(string fileName)
        {
            string result = GetAPICall("api/metaDataService/GetSideMenuFromFile/" + fileName);

            return result;
        }

        public void SetSideMenu(string modelName, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetSideMenu/" + modelName, xmlString, "application/xml");
        }

        public string GetCustomCommandSet(string modelName)
        {
            string result = GetAPICall("api/metaDataService/GetCustomCommandSet/" + modelName);

            return result;
        }

        public string GetCustomCommandSetFromFile(string fileName)
        {
            string result = GetAPICall("api/metaDataService/GetCustomCommandSetFromFile/" + fileName);

            return result;
        }

        public void SetCustomCommandSet(string modelName, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetCustomCommandSet/" + modelName, xmlString, "application/xml");
        }

        public string GetSiteMapAccessPolicy(string modelName)
        {
            string result = GetAPICall("api/metaDataService/GetSiteMapAccessPolicy/" + modelName);

            return result;
        }

        public string GetSiteMapAccessPolicyFromFile(string fileName)
        {
            string result = GetAPICall("api/metaDataService/GetSiteMapAccessPolicyFromFile/" + fileName);

            return result;
        }


        public void SetSiteMapAccessPolicy(string modelName, string xmlString)
        {
            string result = PostAPICall("api/metaDataService/SetSiteMapAccessPolicy/" + modelName, xmlString, "application/xml");
        }

        public string[] GetFormTemplatesFileNames(string schemaId, string className)
        {
            string result = GetAPICall("api/metaDataService/GetFormTemplatesFileNames/" + schemaId + "/" + className);

            string[] array = null;

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception)
            {
                array = new string[] { };
            }

            return array;
        }

        public string[] GetReportTemplatesFileNames(string schemaId, string className)
        {
            string result = GetAPICall("api/metaDataService/GetReportTemplatesFileNames/" + schemaId + "/" + className);

            string[] array = null;

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception)
            {
                array = new string[] { };
            }

            return array;
        }

        public string[] GetWorklowNames(string projectName, string projectVersion, string schemaId, string className)
        {
            string result = GetAPICall("api/metaDataService/GetWorklowNames/" + projectName + "/" + projectVersion + "/" + schemaId + "/" + className);

            string[] array = null;

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception)
            {
                array = new string[] { };
            }

            return array;
        }

        public string[] GetImageNames()
        {
            string result = GetAPICall("api/metaDataService/GetImageNames");

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

        public byte[] GetImageBytes(string imageName)
        {
            string result = GetAPICall("api/metaDataService/GetImageBytes/" + imageName);

            byte[] array = new byte[] { };

            try
            {
                array = JsonConvert.DeserializeObject<byte[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }
    }
}
