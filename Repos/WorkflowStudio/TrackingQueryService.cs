/*
* @(#)TrackingQueryService.cs
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
using Newtera.WorkflowMonitor;
using Newtera.WinClientCommon;

namespace WorkflowStudio
{
    /// <summary>
    /// This class provides database query services against the TrackingService using NewteraTrackingQuery APIs
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    public sealed class TrackingQueryService : ITrackingQueryService
    {
        public TrackingQueryService()
        {
        }

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
        public int GetWorkflowCount(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until,
            bool useCondition)
        {
            int count;

            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            count = service.GetTrackingWorkflowInstanceCount(connectionStr,
                workflowTypeId,
                workflowEvent,
                from,
                until,
                useCondition);

            return count;
        }

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
        public NewteraTrackingWorkflowInstanceCollection GetWorkflows(string workflowTypeId, string workflowEvent,
            DateTime from, DateTime until, bool useCondition, int pageIndex, int pageSize)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            string xmlString = service.GetTrackingWorkflowInstances(connectionStr,
                workflowTypeId,
                workflowEvent,
                from,
                until,
                useCondition,
                pageIndex,
                pageSize);
            NewteraTrackingWorkflowInstanceCollection queriedWorkflows = new NewteraTrackingWorkflowInstanceCollection();
            queriedWorkflows.Load(xmlString);
            
            return queriedWorkflows;
        }

        /// <summary>
        /// Gets a NewteraTrackingWorkflowInstance for a given workflow instance from the
        /// tracking service located at the server
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <returns></returns>
        public NewteraTrackingWorkflowInstance GetWorkflow(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            string xmlString = service.GetTrackingWorkflowInstance(connectionStr, workflowInstanceId.ToString());
            NewteraTrackingWorkflowInstanceCollection queriedWorkflows = new NewteraTrackingWorkflowInstanceCollection();
            queriedWorkflows.Load(xmlString);
            if (queriedWorkflows.Count == 1)
            {
                return (NewteraTrackingWorkflowInstance)queriedWorkflows[0];
            }
            else
            {
                throw new Exception("Unable to get tracking data for workflow instance with id " + workflowInstanceId.ToString());
            }
        }

        /// <summary>
        /// Cancel a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public void CancelWorkflow(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            service.CancelWorkflowInstance(connectionStr, workflowInstanceId.ToString());
        }

        /// <summary>
        /// Cancel a workflow activity that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        /// <param name="activityName">The activity name</param>
        public void CancelActivity(Guid workflowInstanceId, string activityName)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            service.CancelActivity(connectionStr, workflowInstanceId.ToString(), activityName);
        }


        /// <summary>
        /// Delete tracking data from the database of a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public void DeleteTrackingWorkflowInstance(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            service.DeleteTrackingWorkflowInstance(connectionStr, workflowInstanceId.ToString());
        }

        /// <summary>
        /// Get a list of TaskInfo objects that are associated with a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public TaskInfoCollection GetTaskInfos(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            TaskInfo[] taskInfos = service.GetTaskInfos(connectionStr, workflowInstanceId.ToString());

            // convert to WFModel.TaskInfo
            TaskInfoCollection newTaskInfos = new TaskInfoCollection();
            for (int i = 0; i < taskInfos.Length; i++)
            {
                TaskInfo taskInfo = taskInfos[i];
                TaskInfo newTaskInfo = new TaskInfo();
                newTaskInfo.TaskId = taskInfo.TaskId;
                newTaskInfo.Subject = taskInfo.Subject;
                newTaskInfo.Description = taskInfo.Description;
                newTaskInfo.Instruction = taskInfo.Instruction;
                newTaskInfo.CreateTime = taskInfo.CreateTime;
                newTaskInfo.Users = taskInfo.Users;
                newTaskInfo.Roles = taskInfo.Roles;
                newTaskInfo.WorkflowInstanceId = taskInfo.WorkflowInstanceId;
                newTaskInfo.BindingClassName = taskInfo.BindingClassName;
                newTaskInfo.BindingSchemaId = taskInfo.BindingSchemaId;
                newTaskInfo.ActivityName = taskInfo.ActivityName;
                newTaskInfo.CustomActionsXml = taskInfo.CustomActionsXml;

                newTaskInfos.Add(newTaskInfo);
            }

            return newTaskInfos;
        }

        /// <summary>
        /// Get information of the data instance that has been bound to a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The binding info, null if no binding data instance exists.</returns>
        public Newtera.WFModel.WorkflowInstanceBindingInfo GetBindingDataInstanceInfo(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            WorkflowInstanceBindingInfo bindingInfo = service.GetBindingDataInstanceInfo(connectionStr, workflowInstanceId.ToString());
            Newtera.WFModel.WorkflowInstanceBindingInfo newBindingInfo = null;

            if (bindingInfo != null)
            {
                // convert to WFModel.WorkflowInstanceBindingInfo
                newBindingInfo = new WorkflowInstanceBindingInfo();
                newBindingInfo.WorkflowInstanceId = bindingInfo.WorkflowInstanceId;
                newBindingInfo.WorkflowTypeId = bindingInfo.WorkflowTypeId;
                newBindingInfo.DataInstanceId = bindingInfo.DataInstanceId;
                newBindingInfo.DataClassName = bindingInfo.DataClassName;
                newBindingInfo.SchemaId = bindingInfo.SchemaId;
                newBindingInfo.ProjectName = bindingInfo.ProjectName;
                newBindingInfo.WorkflowName = bindingInfo.WorkflowName;
            }

            return newBindingInfo;
        }

        /// <summary>
        /// Gets a WorkflowInstanceStateInfo for a given workflow instance from the server
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>Newtera.WFModel.WorkflowInstanceStateInfo object</returns>
        public Newtera.WFModel.WorkflowInstanceStateInfo GetWorkflowInstanceStateInfo(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            WorkflowInstanceStateInfo instanceState = service.GetWorkflowInstanceStateInfo(connectionStr, workflowInstanceId.ToString());

            return instanceState;
        }

        /// <summary>
        /// Get a collection of DBEventSubscription objects that are associated with a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public Newtera.WFModel.DBEventSubscriptionCollection GetDBEventSubscriptions(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            DBEventSubscription[] subscriptions = service.GetDBEventSubscriptions(connectionStr, workflowInstanceId.ToString());

            DBEventSubscriptionCollection newSubscriptions = new DBEventSubscriptionCollection();
            for (int i = 0; i < subscriptions.Length; i++)
            {
                DBEventSubscription subscription = subscriptions[i];
                Newtera.WFModel.DBEventSubscription newSubscription = new Newtera.WFModel.DBEventSubscription();
                newSubscription.SubscriptionId = subscription.SubscriptionId;
                newSubscription.SchemaId = subscription.SchemaId;
                newSubscription.ClassName = subscription.ClassName;
                newSubscription.EventName = subscription.EventName;
                newSubscription.WorkflowInstanceId = subscription.WorkflowInstanceId;
                newSubscription.QueueName = subscription.QueueName;
                newSubscription.CreateDataBinding = subscription.CreateDataBinding;

                newSubscriptions.Add(newSubscription);
            }

            return newSubscriptions;
        }

        /// <summary>
        /// Get a collection of WorkflowEventSubscription objects that are associated with a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public Newtera.WFModel.WorkflowEventSubscriptionCollection GetWorkflowEventSubscriptions(Guid workflowInstanceId)
        {
            WorkflowTrackingServiceStub service = new WorkflowTrackingServiceStub();
            string connectionStr = ConnectionStringBuilder.Instance.Create();
            WorkflowEventSubscription[] subscriptions = service.GetWorkflowEventSubscriptions(connectionStr, workflowInstanceId.ToString());

            WorkflowEventSubscriptionCollection newSubscriptions = new WorkflowEventSubscriptionCollection();
            for (int i = 0; i < subscriptions.Length; i++)
            {
                WorkflowEventSubscription subscription = subscriptions[i];
                Newtera.WFModel.WorkflowEventSubscription newSubscription = new Newtera.WFModel.WorkflowEventSubscription();
                newSubscription.SubscriptionId = subscription.SubscriptionId;
                newSubscription.ParentWorkflowInstanceId = subscription.ParentWorkflowInstanceId;
                newSubscription.ChildWorkflowInstanceId = subscription.ChildWorkflowInstanceId;
                newSubscription.QueueName = subscription.QueueName;
                newSubscription.EventType = (WorkflowEventType) Enum.Parse(typeof(WorkflowEventType),
                    Enum.GetName(typeof(WorkflowEventType), subscription.EventType));
                
                newSubscriptions.Add(newSubscription);
            }

            return newSubscriptions;
        }
    }
}
