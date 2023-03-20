/*
* @(#)NewteraCustomTrackingChannel.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Xml;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Workflow.ComponentModel;
using System.Threading;
using System.Security.Principal;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.Server.DB;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary> 
    /// Implements a custom workflow tracking channel using Newtera Data Store
    /// </summary>
    /// <version> 1.0.0 12 Feb 2007 </version>
    public class NewteraCustomTrackingChannel : TrackingChannel
    {        
        private TrackingParameters _trackingParameters = null;

        protected NewteraCustomTrackingChannel()
        {
            CMUserManager userMgr = new CMUserManager();
        }

        public NewteraCustomTrackingChannel(TrackingParameters parameters)
        {
            _trackingParameters = parameters;

            CMUserManager userMgr = new CMUserManager();
        }

        // Send() is called by Tracking runtime to send various tracking records
        protected override void Send(TrackingRecord record)
        {
            try
            {
                //filter on record type
                if (record is WorkflowTrackingRecord)
                {
                    WriteWorkflowTrackingRecord((WorkflowTrackingRecord)record);
                }
                if (record is ActivityTrackingRecord)
                {
                    WriteActivityTrackingRecord((ActivityTrackingRecord)record);
                }

                if (record is UserTrackingRecord)
                {
                    WriteUserTrackingRecord((UserTrackingRecord)record);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                //throw ex;
            }
        }

        // InstanceCompletedOrTerminated() is called by Tracking runtime to indicate that the Workflow instance finished running
        protected override void InstanceCompletedOrTerminated()
        {
        }

        private void WriteWorkflowTrackingRecord(WorkflowTrackingRecord workflowTrackingRecord)
        {
            Guid wfInstanceId = WorkflowEnvironment.WorkflowInstanceId;
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            if (workflowTrackingRecord.TrackingWorkflowEvent == TrackingWorkflowEvent.Started)
            {
                NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(wfInstanceId);
                if (wfInstance == null)
                {
                    throw new WorkflowServerException("Unable to find the workflow instance with id " + wfInstanceId.ToString());
                }

                string workflowTypeId = wfInstance.WorkflowTypeId;

                adapter.WriteWorkflowTrackingRecord(wfInstanceId.ToString(),
                    workflowTypeId,
                    workflowTrackingRecord.EventDateTime.ToString("s"),
                    Enum.GetName(typeof(TrackingWorkflowEvent), workflowTrackingRecord.TrackingWorkflowEvent));

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = { "An new worklfow record has been created.",
                                "Workflow Instance Id: " + wfInstanceId.ToString(),
                                "Event DateTime: " + workflowTrackingRecord.EventDateTime.ToString("s"),
                                "Event: " + Enum.GetName(typeof(TrackingWorkflowEvent), workflowTrackingRecord.TrackingWorkflowEvent)};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
            else
            {
                adapter.UpdateWorkflowTrackingRecord(wfInstanceId.ToString(),
                    workflowTrackingRecord.EventDateTime.ToString("s"),
                    Enum.GetName(typeof(TrackingWorkflowEvent), workflowTrackingRecord.TrackingWorkflowEvent));

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = { "An worklfow record has been updated.",
                                "Workflow Instance Id: " + wfInstanceId.ToString(),
                                "Event DateTime: " + workflowTrackingRecord.EventDateTime.ToString("s"),
                                "Event: " + Enum.GetName(typeof(TrackingWorkflowEvent), workflowTrackingRecord.TrackingWorkflowEvent)};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
        }

        private void WriteActivityTrackingRecord(ActivityTrackingRecord activityTrackingRecord)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            string wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
            string activityInstanceId;
            if (!TryGetActivityInstanceId(wfInstanceId, activityTrackingRecord.QualifiedName, out activityInstanceId))
            {
                activityInstanceId = Guid.NewGuid().ToString();
                adapter.WriteActivityTrackingRecord(activityInstanceId,
                    activityTrackingRecord.ActivityType.Name,
                    activityTrackingRecord.QualifiedName,
                    activityTrackingRecord.EventDateTime.ToString("s"),
                    Enum.GetName(typeof(ActivityExecutionStatus), activityTrackingRecord.ExecutionStatus),
                    wfInstanceId);

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = { "An new activity record has been created.",
                                "Activity Id: " + activityInstanceId,
                                "Activity Name: " + activityTrackingRecord.ActivityType.Name,
                                "Activity Qualified Name: " + activityTrackingRecord.QualifiedName,
                                "Event DateTime: " + activityTrackingRecord.EventDateTime.ToString("s"),
                                "Execution Status: " + Enum.GetName(typeof(ActivityExecutionStatus), activityTrackingRecord.ExecutionStatus),
                                "Workflow Instance Id: " + wfInstanceId};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
            else
            {
                adapter.UpdateActivityTrackingRecord(activityInstanceId,
                    activityTrackingRecord.EventDateTime.ToString("s"),
                    Enum.GetName(typeof(ActivityExecutionStatus), activityTrackingRecord.ExecutionStatus));

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = { "An activity record has been updated.",
                                "Activity Id: " + activityInstanceId,
                                "Activity Name: " + activityTrackingRecord.ActivityType.Name,
                                "Activity Qualified Name: " + activityTrackingRecord.QualifiedName,
                                "Event DateTime: " + activityTrackingRecord.EventDateTime.ToString("s"),
                                "Execution Status: " + Enum.GetName(typeof(ActivityExecutionStatus), activityTrackingRecord.ExecutionStatus),
                                "Workflow Instance Id: " + wfInstanceId};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
        }

        private void WriteUserTrackingRecord(UserTrackingRecord userTrackingRecord)
        {
        }

        private bool TryGetActivityInstanceId(string wfInstanceId, string activityQualifiedName, out string activityInstanceId)
        {
            bool status = false;

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            activityInstanceId = adapter.GetActivityInstanceId(wfInstanceId, activityQualifiedName);

            if (activityInstanceId != null)
            {
                status = true;
            }

            return status;
        }
    }
}
