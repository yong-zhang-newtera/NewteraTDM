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
using Newtera.WFModel;

namespace Newtera.WinClientCommon
{
    public class WorkflowModelServiceStub : WebApiServiceBase
    {
        public WorkflowModelServiceStub()
        {
        }

        public ProjectInfo[] GetExistingProjectInfos()
        {
            string result = GetAPICall("api/workflowModelService/GetExistingProjectInfos");

            ProjectInfo[] array = new ProjectInfo[] { };

            try
            {
                array = JsonConvert.DeserializeObject<ProjectInfo[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string GetProject(string connectionStr, string projectName, string projectVersion)
        {
            return GetAPICall("api/workflowModelService/GetProject/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr);
        }

        public string GetProjectPolicy(string connectionStr, string projectName, string projectVersion)
        {
            return GetAPICall("api/workflowModelService/GetProjectPolicy/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr);
        }

        public System.DateTime SaveProject(string connectionStr, string projectName, string projectVersion, string projectXml, string policyXml)
        {
            string[] apiParams = new string[] { projectXml, policyXml };

            string content = JsonConvert.SerializeObject(apiParams);

            string result = PostAPICall("api/workflowModelService/SaveProject/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr, content, "application/json");

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

        public void DeleteProject(string connectionStr, string projectName, string projectVersion)
        {
            PostAPICall("api/workflowModelService/DeleteProject/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr, null, null);
        }

        public bool HasRules(string connectionStr, string projectName, string projectVersion, string workflowName)
        {
            string result = GetAPICall("api/workflowModelService/HasRules/" + projectName + "/" + projectVersion + "/" + workflowName + "?connectionStr=" + connectionStr);

            bool status = false;
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

        public bool HasLayout(string connectionStr, string projectName, string projectVersion, string workflowName)
        {
            string result = GetAPICall("api/workflowModelService/HasLayout/" + projectName + "/" + projectVersion + "/" + workflowName + "?connectionStr=" + connectionStr);

            bool status = false;
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

        public string GetWorkflowData(string connectionStr, string projectName, string projectVersion, string workflowName, string dataTypestr)
        {
            return GetAPICall("api/workflowModelService/GetWorkflowData/" + projectName + "/" + projectVersion + "/" + workflowName + "/" + dataTypestr + "?connectionStr=" + connectionStr);
        }

        public void SetWorkflowData(string connectionStr, string projectName, string projectVersion, string workflowName, string dataTypestr, string dataString)
        {
            PostAPICall("api/workflowModelService/SetWorkflowData/" + projectName + "/" + projectVersion + "/" + workflowName + "/" + dataTypestr + "?connectionStr=" + connectionStr, dataString, "text/plain");
        }

        public string StartWorkflow(string connectionStr, string projectName, string projectVersion, string workflowName)
        {
            return PostAPICall("api/workflowModelService/StartWorkflow/" + projectName + "/" + projectVersion + "/" + workflowName + "?connectionStr=" + connectionStr, null, null);
        }

        public void LockProject(string projectName, string projectVersion, string connectionStr)
        {
            PostAPICall("api/workflowModelService/LockProject/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr, null, null);
        }

        public void UnlockProject(string projectName, string projectVersion, string connectionStr, bool forceUnlock)
        {
            PostAPICall("api/workflowModelService/UnlockProject/" + projectName + "/" + projectVersion + "/" + forceUnlock + "?connectionStr=" + connectionStr, null, null);
        }

        public SchemaInfo[] GetAuthorizedSchemaInfos(string connectionStr)
        {
            string result = GetAPICall("api/workflowModelService/GetAuthorizedSchemaInfos?connectionStr=" + connectionStr);

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

        public string GetDBARole(string connectionStr, string projectName, string projectVersion)
        {
            return GetAPICall("api/workflowModelService/GetDBARole/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr);
        }

        public void SetDBARole(string connectionStr, string projectName, string projectVersion, string role)
        {
            PostAPICall("api/workflowModelService/SetDBARole/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr, role, "text/plain");
        }

        public bool HasRunningInstances(string connectionStr, string projectName, string projectVersion, string workflowId)
        {
            string result = GetAPICall("api/workflowModelService/HasRunningInstances/" + projectName + "/" + projectVersion + "/" + workflowId + "?connectionStr=" + connectionStr);

            bool status = false;
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

        public string GetWorkflowModelID(string connectionStr, string projectName, string projectVersion, string workflowName)
        {
            return GetAPICall("api/workflowModelService/GetWorkflowModelID/" + projectName + "/" + projectVersion + "/" + workflowName + "?connectionStr=" + connectionStr);
        }

        public bool IsLatestVersion(string connectionStr, string projectName, string projectVersion)
        {
            string result = GetAPICall("api/workflowModelService/IsLatestVersion/" + projectName + "/" + projectVersion + "?connectionStr=" + connectionStr);

            bool status = false;
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

        public bool IsQueryValid(string connectionStr, string query)
        {
            string result = PostAPICall("api/workflowModelService/IsQueryValid?connectionStr=" + connectionStr, query, "text/plain");

            bool status = false;
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

        public bool IsValidCustomFunctionDefinition(string connectionStr, string functionDefinition)
        {
            string result = PostAPICall("api/workflowModelService/IsValidCustomFunctionDefinition?connectionStr=" + connectionStr, functionDefinition, "text/plain");

            bool status = false;
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

        public string ValidateActionCode(string connectionStr, string actionCode, string schemaId, string instanceClassName)
        {
            return PostAPICall("api/workflowModelService/ValidateActionCode/" + schemaId + "/" + instanceClassName + "?connectionStr=" + connectionStr, actionCode, "text/plain");
        }

        public string GetTaskSubstituteModel(string connectionStr)
        {
            return GetAPICall("api/workflowModelService/GetTaskSubstituteModel?connectionStr=" + connectionStr);
        }

        public void UpdateTaskSubstituteModel(string connectionStr, string xml)
        {
            PostAPICall("api/workflowModelService/UpdateTaskSubstituteModel?connectionStr=" + connectionStr, xml, "application/xml");
        }

        public void LockTaskSubstituteModel(string connectionStr)
        {
            PostAPICall("api/workflowModelService/LockTaskSubstituteModel?connectionStr=" + connectionStr, null, null);
        }

        public void UnlockTaskSubstituteModel(string connectionStr, bool forceUnlock)
        {
            PostAPICall("api/workflowModelService/UnlockTaskSubstituteModel/" + forceUnlock + "?connectionStr=" + connectionStr, null, null);
        }

    }
}
