/*
* @(#)IWorkflowService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Workflow.Runtime;

namespace Newtera.WorkflowServices
{
    /// <summary>
    /// Define the interface for Newtera Workflow services used by workflow runtime
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// Start a workflow instance defined in the same project of the invoking workflow
        /// </summary>
        /// <param name="className">The name of the workflow</param>
        /// <param name="inputParameters">The input parameters for invoking the workflow, can be null</param>
        /// <returns>The workflow instance id</returns>
        Guid StartWorkflowInstance(string workflowName, IList inputParameters);

        /// <summary>
        /// Cancel the given workflow instance
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id</param>
        void CancelWorkflow(Guid wfInstanceId);

        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="queueName">The queue name</param>
        /// <param name="wfInstanceId">The workflow instance id</param>
        /// <returns>The subscription id</returns>
        Guid RegisterListener(IComparable queueName, Guid wfInstanceId);

        /// <summary>
        /// Unregister an event listener
        /// </summary>
        /// <param name="subscriptionId"></param>
        void UnregisterListener(Guid subscriptionId);

        /// <summary>
        /// Add a subscription to db that associates a child workflow instance with the parent workflow instance
        /// </summary>
        /// <param name="childWorkflowInstanceId">The child workflow instance id</param>
        /// <returns>The subscription id</returns>
        Guid AddChildWorkflowEventSubscription(Guid childWorkflowInstanceId);

        /// <summary>
        /// Raise an workflow complete or terminate event
        /// </summary>
        /// <param name="instanceId">Workflow instance id</param>
        /// <param name="runtime">Workflow runtime</param>
        /// <param name="e"></param>
        void RaiseWorkflowEvent(Guid wfInstanceId, WorkflowRuntime runtime, NewteraEventArgs e);

        /// <summary>
        /// Remove the subscriptions associated with a workflow instance.
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id.</param>
        void RemoveSubscriptions(Guid wfInstanceId);

        /// <summary>
        /// Get the id of binding data instance to a workflow instance
        /// </summary>
        /// <param name="dataClassName">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>The id of the binding data instance, null if no binding exists.</returns>
        string GetBindingDataInstanceId(string dataClassName, string schemaId, Guid workflowInstanceId);

        /// <summary>
        /// Set a binding between a data instance and a workflow instance
        /// </summary>
        /// <param name="dataInstanceId">The data instance id</param>
        /// <param name="dataClassName">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        void SetWorkflowInstanceBinding(string dataInstanceId, string dataClassName, string schemaId, Guid workflowInstanceId);

        /// <summary>
        /// Sets a parameter value of a workflow instance in the cache
        /// </summary>
        /// <param name="workflowName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        void SetInputParameterValue(string workflowInstanceId, string parameterName, object parameterValue);

        /// <summary>
        /// Gets a value of an input parameter of a workflow instance from the cache
        /// </summary>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter value</returns>
        object GetInputParameterValue(string workflowInstanceId, string parameterName);

        /// <summary>
        /// Remove a value of an input parameter of a workflow instance from the cache
        /// </summary>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="parameterName">The parameter name</param>
        void RemoveInputParameterValue(string workflowInstanceId, string parameterName);

        /// <summary>
        /// Gets a workflow instance's tasks
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <param name="activityName">The name of InvokeAsyncWorkflowActivity</param>
        /// <returns>A list of workflow instance ids.</returns>
        List<Guid> GetChildWorkflowInstanceIds(string workflowInstanceId, string activityName);
    }
}
