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
    public class WorkflowTrackingServiceStub : WebApiServiceBase
    {
        public WorkflowTrackingServiceStub()
        {
        }

        public int GetTrackingWorkflowInstanceCount(string connectionStr, string workflowTypeId, string workflowEvent, System.DateTime from, System.DateTime until, bool useCondition)
        {
            string[] apiParams = new string[] { workflowEvent, from.ToString("s"), until.ToString("s"), useCondition.ToString() };

            string content = JsonConvert.SerializeObject(apiParams);

            string result = PostAPICall("api/workflowTrackingService/GetTrackingWorkflowInstanceCount/" + workflowTypeId + "?connectionStr=" + connectionStr, content, "application/json");

            int count;
            try
            {
                count = int.Parse(result);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count;
        }

        public string GetTrackingWorkflowInstances(string connectionStr, string workflowTypeId, string workflowEvent, System.DateTime from, System.DateTime until, bool useCondition, int pageIndex, int pageSize)
        {
            string[] apiParams = new string[] { workflowEvent, from.ToString("s"), until.ToString("s"), useCondition.ToString(),pageIndex.ToString(), pageSize.ToString() };

            string content = JsonConvert.SerializeObject(apiParams);

            return PostAPICall("api/workflowTrackingService/GetTrackingWorkflowInstances/" + workflowTypeId + "?connectionStr=" + connectionStr, content, "application/json");
        }

        public string GetTrackingWorkflowInstance(string connectionStr, string workflowInstanceId)
        {
            return GetAPICall("api/workflowTrackingService/GetTrackingWorkflowInstance/" + workflowInstanceId + "?connectionStr=" + connectionStr);
        }

        public void CancelWorkflowInstance(string connectionStr, string workflowInstanceId)
        {
            PostAPICall("api/workflowTrackingService/CancelWorkflowInstance/" + workflowInstanceId + "?connectionStr=" + connectionStr, null, null);
        }

        public void CancelActivity(string connectionStr, string workflowInstanceId, string activityName)
        {
            PostAPICall("api/workflowTrackingService/CancelActivity/" + workflowInstanceId + "/" + activityName + "?connectionStr=" + connectionStr, null, null);
        }

        public void DeleteTrackingWorkflowInstance(string connectionStr, string workflowInstanceId)
        {
            PostAPICall("api/workflowTrackingService/DeleteTrackingWorkflowInstance/" + workflowInstanceId + "?connectionStr=" + connectionStr, null, null);
        }

        public TaskInfo[] GetTaskInfos(string connectionStr, string workflowInstanceId)
        {
            string result = GetAPICall("api/workflowTrackingService/GetTaskInfos/" + workflowInstanceId + "?connectionStr=" + connectionStr);

            TaskInfo[] array = new TaskInfo[] { };

            try
            {
                array = JsonConvert.DeserializeObject<TaskInfo[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public WorkflowInstanceBindingInfo GetBindingDataInstanceInfo(string connectionStr, string workflowInstanceId)
        {
            string result = GetAPICall("api/workflowTrackingService/GetBindingDataInstanceInfo/" + workflowInstanceId + "?connectionStr=" + connectionStr);

            WorkflowInstanceBindingInfo obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject<WorkflowInstanceBindingInfo>(result);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message);
                throw ex;
            }

            return obj;
        }

        public string[] GetBindingDataInstanceIds(string connectionStr, string schemaId, int pageSize, int pageIndex)
        {
            string result = GetAPICall("api/workflowTrackingService/GetBindingDataInstanceIds/" + schemaId + "/" + pageSize + "/" + pageIndex + "?connectionStr=" + connectionStr);

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

        public void ReplaceBindingDataInstanceId(string connectionStr, string oldInstanceId, string newInstanceId)
        {
            PostAPICall("api/workflowTrackingService/ReplaceBindingDataInstanceId/" + oldInstanceId + "/" + newInstanceId + "?connectionStr=" + connectionStr, null, null);
        }

        public WorkflowInstanceStateInfo GetWorkflowInstanceStateInfo(string connectionStr, string workflowInstanceId)
        {
            string result = GetAPICall("api/workflowTrackingService/GetWorkflowInstanceStateInfo/" + workflowInstanceId + "?connectionStr=" + connectionStr);

            WorkflowInstanceStateInfo obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject<WorkflowInstanceStateInfo>(result);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message);
                throw ex;
            }

            return obj;
        }

        public DBEventSubscription[] GetDBEventSubscriptions(string connectionStr, string workflowInstanceId)
        {
            string result = GetAPICall("api/workflowTrackingService/GetDBEventSubscriptions/" + workflowInstanceId + "?connectionStr=" + connectionStr);

            DBEventSubscription[] array = new DBEventSubscription[] { };

            try
            {
                array = JsonConvert.DeserializeObject<DBEventSubscription[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public WorkflowEventSubscription[] GetWorkflowEventSubscriptions(string connectionStr, string workflowInstanceId)
        {
            string result = GetAPICall("api/workflowTrackingService/GetWorkflowEventSubscriptions/" + workflowInstanceId + "?connectionStr=" + connectionStr);

            WorkflowEventSubscription[] array = new WorkflowEventSubscription[] { };

            try
            {
                array = JsonConvert.DeserializeObject<WorkflowEventSubscription[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public void SetTrackingWorkflowInstances(string connectionStr, string workflowTypeId, string xmlString)
        {
            PostAPICall("api/workflowTrackingService/SetTrackingWorkflowInstances/" + workflowTypeId + "?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public void SetWorkflowInstanceStateInfo(string connectionStr, WorkflowInstanceStateInfo stateInfo)
        {
            string content = JsonConvert.SerializeObject(stateInfo);

            PostAPICall("api/workflowTrackingService/SetWorkflowInstanceStateInfo?connectionStr=" + connectionStr, content, "application/json");
        }

        public void SetWorkflowEventSubscriptions(string connectionStr, string xmlString)
        {
            PostAPICall("api/workflowTrackingService/SetWorkflowEventSubscriptions?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public void SetDBEventSubscriptions(string connectionStr, string xmlString)
        {
            PostAPICall("api/workflowTrackingService/SetDBEventSubscriptions?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public void SetBindingDataInstanceInfos(string connectionStr, string workflowTypeId, string xmlString)
        {
            PostAPICall("api/workflowTrackingService/SetBindingDataInstanceInfos/" + workflowTypeId + "?connectionStr=" + connectionStr, xmlString, "application/xml");
        }

        public void SetTaskInfos(string connectionStr, string xmlString)
        {
            PostAPICall("api/workflowTrackingService/SetTaskInfos?connectionStr=" + connectionStr, xmlString, "application/xml");
        }
    }
}
