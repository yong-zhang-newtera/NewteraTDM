/*
* @(#)IDBEventService.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Workflow.Runtime;

namespace Newtera.WorkflowServices
{
    /// <summary>
    /// Define the interface for Newtera Database event management service used by workflow runtime
    /// </summary>
    public interface IDBEventService
    {
        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="queueName">The queue name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="eventName">The event name</param>
        /// <param name="createDataBinding">Information indicating whether to create a binding between the workflow instance and the data instance that raises the event</param>
        /// <returns>The subscription id</returns>
        Guid RegisterListener(IComparable queueName, string schemaId, string className, string eventName, bool createDataBinding);

        /// <summary>
        /// Unregister an event listener
        /// </summary>
        /// <param name="subscriptionId"></param>
        void UnregisterListener(Guid subscriptionId);

        /// <summary>
        /// Raise an data changed event
        /// </summary>
        /// <param name="instanceId">Workflow instance id</param>
        /// <param name="runtime">Workflow runtime</param>
        /// <param name="e"></param>
        void RaiseDataChangedEvent(Guid wfInstanceId, WorkflowRuntime runtime, NewteraEventArgs e);

        /// <summary>
        /// Remove the subscriptions associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        void RemoveSubscriptions(Guid workflowInstanceId);
    }
}
