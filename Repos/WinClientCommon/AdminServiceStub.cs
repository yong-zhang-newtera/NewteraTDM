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
    public class AdminServiceStub : WebApiServiceBase
    {
        public AdminServiceStub()
        {
        }

        public string GetServerVersion()
        {
            return GetAPICall("api/adminService/GetServerVersion");
        }

        public bool IsDataSourceValid(string databaseType, string dataSourceName)
        {
            bool status = false;

            string result = GetAPICall("api/adminService/IsDataSourceValid/" + databaseType + "/" + dataSourceName);

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

        public bool NeedCreateDatabase(string databaseType, string dataSourceName)
        {
            bool status = false;

            string result = GetAPICall("api/adminService/NeedCreateDatabase/" + databaseType + "/" + dataSourceName);

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

        public void CreateDatabase(string databaseType, string dataSourceName)
        {
            GetAPICall("api/adminService/CreateDatabase/" + databaseType + "/" + dataSourceName);
        }

        public bool NeedCreateTablespace(string databaseType, string dataSourceName)
        {
            bool status = false;

            string result = GetAPICall("api/adminService/NeedCreateTablespace/" + databaseType + "/" + dataSourceName);

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

        public void CreateTablespace(string databaseType, string dataSourceName, string userName, string password, string dataFileDir)
        {
            string[] apiParams = new string[] { userName, password, dataFileDir};

            string content = JsonConvert.SerializeObject(apiParams);

            PostAPICall("api/adminService/CreateTablespace/" + databaseType + "/" + dataSourceName + "/" + userName + "/" + password, 
                content, "application/json");
        }

        public void UpdateSchema(string databaseType, string dataSourceName)
        {
            PostAPICall("api/adminService/UpdateSchema/" + databaseType + "/" + dataSourceName, null, null);
        }

        public void UpdateServerConfig(string databaseType, string dataSourceName, string imageBaseURL, string imageBasePath)
        {
            string[] apiParams = new string[] { imageBaseURL, imageBasePath };

            string content = JsonConvert.SerializeObject(apiParams);

            PostAPICall("api/adminService/UpdateServerConfig/" + databaseType + "/" + dataSourceName, content, "application/json");
        }

        public string GetAppSchemaList()
        {
            return GetAPICall("api/adminService/GetAppSchemaList");
        }

        public string GetAppHomeDir()
        {
            return GetAPICall("api/adminService/GetAppHomeDir");
        }

        public string GetServerId()
        {
            return GetAPICall("api/adminService/GetServerId");
        }

        public string GetServerLicenseMsg()
        {
            return GetAPICall("api/adminService/GetServerLicenseMsg");
        }

        public string GetServerLicenseDetails()
        {
            return GetAPICall("api/adminService/GetServerLicenseDetails");
        }

        public string DisableLicense(string adminName, string adminPassword)
        {
            return GetAPICall("api/adminService/DisableLicense/" + adminName + "/" + adminPassword);
        }

        public string GetClientLicenseMsg(string clientName, string clientId)
        {
            return GetAPICall("api/adminService/GetClientLicenseMsg/" + clientName + "/" + clientId);
        }

        public void SetLicenseKey(string licenseKey)
        {
            PostAPICall("api/adminService/SetLicenseKey", licenseKey, "text/plain");
        }

        public bool IsEvaluationLicense()
        {
            bool status = false;

            string result = GetAPICall("api/adminService/IsEvaluationLicense");

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

        public bool IsStandardLicense()
        {
            bool status = false;

            string result = GetAPICall("api/adminService/IsStandardLicense");

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

        public int GetRemainingEvaluationDays()
        {
            int days = 0;

            string result = GetAPICall("api/adminService/GetRemainingEvaluationDays");

            try
            {
                days = int.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw ex;
            }

            return days;
        }

        public string[] GetRegisteredClients(string clientName)
        {
            string result = GetAPICall("api/adminService/GetRegisteredClients/" + clientName);

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

        public void RegisterClient(string clientName, string clientId, string machineName)
        {
            PostAPICall("api/adminService/RegisterClient/" + clientName + "/" + clientId + "/" + machineName, null, null);
        }

        public void CheckInClient(string clientName, string clientId)
        {
            PostAPICall("api/adminService/CheckInClient/" + clientName + "/" + clientId, null, null);
        }

        public void UnregisterClient(string clientName, string clientId)
        {
            PostAPICall("api/adminService/UnregisterClient/" + clientName + "/" + clientId, null, null);
        }

        public void SetupAppSchemas(string[] schemaNames)
        {
            string content = JsonConvert.SerializeObject(schemaNames);

            PostAPICall("api/adminService/SetupAppSchemas", content, "application/json");
        }

        public string GetServerLogText()
        {
            return GetAPICall("api/adminService/GetServerLogText");
        }

        public void ClearServerLog()
        {
            PostAPICall("api/adminService/ClearServerLog", null, null);
        }

        public string GetServerTraceLog()
        {
            return GetAPICall("api/adminService/GetServerTraceLog");
        }

        public void ClearTraceLog()
        {
            PostAPICall("api/adminService/ClearTraceLog", null, null);
        }

        public string GetServerUrl()
        {
            return GetAPICall("api/adminService/GetServerUrl");
        }

        public void SetServerUrl(string url)
        {
            PostAPICall("api/adminService/SetServerUrl", url, "text/plain");
        }
    }
}
