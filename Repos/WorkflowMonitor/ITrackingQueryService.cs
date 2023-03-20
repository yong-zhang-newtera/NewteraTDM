/*
* @(#)ITrackingQueryService.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Collections.Generic;
using System.Workflow.Runtime.Tracking;
using System.Workflow.Runtime;
using System.Globalization;

using Newtera.WFModel;

namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// Defines NewteraTrackingQuery APIs
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    public interface ITrackingQueryService
    {
        /// <summary>
        /// Gets count of for NewteraTrackingWorkflowInstance a given workflow type from tracking service located
        /// at the server
        /// </summary>
        /// <param name="workflowTypeId">An unque id of the workflow type</param>
        /// <param name="workflowEvent"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <returns>The workflow instance count</returns>
        int GetWorkflowCount(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until,
            bool useCondition);

        /// <summary>
        /// Gets a collection of NewteraTrackingWorkflowInstance for a given workflow type from tracking service located
        /// at the server, fetched in paging mode
        /// </summary>
        /// <param name="workflowTypeId">An unque id of the workflow type</param>
        /// <param name="workflowEvent"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>A collection of workflow tracking instances</returns>
        NewteraTrackingWorkflowInstanceCollection GetWorkflows(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until,
            bool useCondition, int pageIndex, int pageSize);

        /// <summary>
        /// Gets a NewteraTrackingWorkflowInstance for a given workflow instance from the
        /// tracking service located at the server
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <returns></returns>
        NewteraTrackingWorkflowInstance GetWorkflow(Guid workflowInstanceId);

        /// <summary>
        /// Cancel a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        void CancelWorkflow(Guid workflowInstanceId);

        /// <summary>
        /// Cancel a workflow activity that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        /// <param name="activityName">the name of activity</param>
        void CancelActivity(Guid workflowInstanceId, string activityName);

        /// <summary>
        /// Delete tracking data from the database of a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        void DeleteTrackingWorkflowInstance(Guid workflowInstanceId);

        /// <summary>
        /// Get a list of TaskInfo objects that are associated with a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        TaskInfoCollection GetTaskInfos(Guid workflowInstanceId);

        /// <summary>
        /// Get information of the data instance that has been bound to a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The binding info, null if no binding data instance exists.</returns>
        WorkflowInstanceBindingInfo GetBindingDataInstanceInfo(Guid workflowInstanceId);
    }
}
