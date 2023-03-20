/*
* @(#)NewteraTrackingQuery.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Xml;
using System.Data;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Workflow.ComponentModel;
using System.Threading;
using System.Security.Principal;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.Server.DB;
using Newtera.WFModel;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary> 
    /// Implements the queries to get workflow tracking data from the Newtera Data Store
    /// </summary>
    /// <version> 1.0.0 04 Jan 2007 </version>
    public class NewteraTrackingQuery
    {
        public NewteraTrackingQuery()
        {
        }

        /// <summary>
        /// Gets a count of workflow tracking instances from Newtera Tracking store
        /// </summary>
        /// <param name="workflowTypeId">The unique id of workflow type to be matched.</param>
        /// <param name="workflowEvent">The workflow event to be matched</param>
        /// <param name="from">The from time to be matched</param>
        /// <param name="until">The until time to be matched</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <returns>Total count</returns>
        public int GetTrackingWorkflowInstanceCount(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until, bool useCondition)
        {
            int count = 0;

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            if (!useCondition)
            {
                count = adapter.GetTrackingWorkflowInstanceCount(workflowTypeId);
            }
            else
            {
                string statusCondition = GetStatusCondition(workflowEvent);

                count = adapter.GetTrackingWorkflowInstanceCountByCondition(workflowTypeId,
                    statusCondition,
                    from.ToString(),
                    until.ToString());
            }

            return count;
        }

        /// <summary>
        /// Gets a collection of NewteraTrackingWorkflowInstance from Newtera Tracking store
        /// </summary>
        /// <param name="workflowTypeId">The unique id of workflow type to be matched.</param>
        /// <param name="workflowEvent">The workflow event to be matched</param>
        /// <param name="from">The from time to be matched</param>
        /// <param name="until">The until time to be matched</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>NewteraTrackingWorkflowInstanceCollection object</returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstances(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until, bool useCondition, int pageIndex, int pageSize)
        {
            NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = null;

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            if (!useCondition)
            {
                trackingWorkflowInstances = adapter.GetTrackingWorkflowInstances(workflowTypeId, pageIndex, pageSize);
            }
            else
            {
                string statusCondition = GetStatusCondition(workflowEvent);

                trackingWorkflowInstances = adapter.GetTrackingWorkflowInstancesByCondition(workflowTypeId,
                    statusCondition,
                    from.ToString(),
                    until.ToString(),
                    pageIndex,
                    pageSize);
            }

            return trackingWorkflowInstances;
        }

        /// <summary>
        /// Gets a collection of NewteraTrackingWorkflowInstance from Newtera Tracking store
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id to be matched</param>
        /// <returns>NewteraTrackingWorkflowInstanceCollection instance</returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstances(string workflowInstanceId)
        {
            NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = null;

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            trackingWorkflowInstances = adapter.GetTrackingWorkflowInstancesByWorkflowInstanceId(workflowInstanceId);

            return trackingWorkflowInstances;
        }

        /// <summary>
        /// Delete tracking data from the database of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        public void DeleteTrackingWorkflowInstance(string workflowInstanceId)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            // delete the activity tracking instances first, and then workflow instance next
            adapter.DeleteActivityTrackingRecords(workflowInstanceId);

            adapter.DeleteWorkflowTrackingRecords(workflowInstanceId);
        }

        /// <summary>
        /// Sets a collection of NewteraTrackingWorkflowInstance
        /// for a given workflow type to database. Used by the project restore tool
        /// </summary>
        /// <param name="workflowTypeId">The unique workflow internal type id</param>
        /// <param name="workflowTrackingInfos">A collection of NewteraTrackingWorkflowInstance.</param>
        public void SetTrackingWorkflowInstances(string workflowTypeId,
            NewteraTrackingWorkflowInstanceCollection workflowTrackingInfos)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            foreach (NewteraTrackingWorkflowInstance workflowTrackingInfo in workflowTrackingInfos)
            {
                // write a workflow instance tracking info
                adapter.WriteWorkflowTrackingRecord(workflowTrackingInfo.WorkflowInstanceId.ToString(),
                    workflowTypeId,
                    workflowTrackingInfo.Initialized.ToString("s"),
                    workflowTrackingInfo.TrackingEvent);

                // write the workflow instance's activity tracking info
                foreach (NewteraActivityTrackingRecord activityTrackingInfo in workflowTrackingInfo.ActivityEvents)
                {
                    adapter.WriteActivityTrackingRecord(activityTrackingInfo.ID,
                        activityTrackingInfo.TypeName,
                        activityTrackingInfo.QualifiedName,
                        activityTrackingInfo.Initialized.ToString("s"),
                        Enum.GetName(typeof(ActivityExecutionStatus), activityTrackingInfo.ExecutionStatus),
                        workflowTrackingInfo.WorkflowInstanceId.ToString());
                }
            }
        }

        private string GetStatusCondition(string status)
        {
            string conditions = null;
            switch (status)
            {
                case "Completed":
                    conditions = "('Completed')";
                    break;

                case "Running":
                    conditions = "('Idle', 'Loaded', 'Persisted', 'Resumed', 'Started', 'Unloaded')";
                    break;

                case "Suspended":
                    conditions = "('Suspended')";

                    break;

                case "Terminated":
                    conditions = "('Aborted', 'Terminated', 'Exception')";

                    break;
            }

            return conditions;
        }
    }
}
