/*
* @(#)NewteraWorkflowService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Workflow.Runtime;
using System.Threading;
using System.Security.Principal;
using System.Net.Mail;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Server.DB;
using Newtera.WFModel;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// Provide the service for newtera workflow runtime.
    /// </summary>
    public class NewteraWorkflowService : IWorkflowService
    {
        private Dictionary<string, WorkflowEventSubscription> subscriptions;
        private Dictionary<string, object> _workflowParameterTable;

        public NewteraWorkflowService()
        {
            // get the subscriptions from the database
            WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();

            this.subscriptions = workflowAdaptor.GetWorkflowEventSubscriptions();

            _workflowParameterTable = new Dictionary<string, object>();
        }

        /// <summary>
        /// Start a workflow instance of given workflow name
        /// </summary>
        /// <param name="workflowName">The name of workflow to be started.</param>
        /// <param name="inputParameters">The input parameters to pass to the new workflow.</param>
        /// <returns>The workflow instance id</returns>
        public Guid StartWorkflowInstance(string workflowName, IList inputParameters)
        {
            Guid wfInstanceId = WorkflowEnvironment.WorkflowInstanceId;

            NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(wfInstanceId);
            if (wfInstance == null)
            {
                throw new WorkflowServerException("Unable to find the workflow instance with id " + wfInstanceId.ToString());
            }
            return NewteraWorkflowRuntime.Instance.StartWorkflowInstance(wfInstance.ProjectName, wfInstance.ProjectVersion, workflowName, inputParameters);
        }

        /// <summary>
        /// Cancel the given workflow instance
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id</param>
        public void CancelWorkflow(Guid wfInstanceId)
        {
            NewteraWorkflowRuntime.Instance.CancelWorkflow(wfInstanceId);
        }

        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="queueName">The queue name</param>
        /// <param name="childWorkflowInstanceId">The child workflow instance id</param>
        /// <returns>The subscription id</returns>
        public Guid RegisterListener(IComparable queueName, Guid childWorkflowInstanceId)
        {
            try
            {
                WorkflowEventSubscription found = null;
                bool persistToDB = false;

                WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();

                lock (this.subscriptions)
                {
                    WorkflowEventSubscriptionCollection workflowEventSubscriptions = workflowAdaptor.GetWorkflowEventSubscriptionsByWFInstanceId(WorkflowEnvironment.WorkflowInstanceId.ToString());
                    foreach (WorkflowEventSubscription subscription in workflowEventSubscriptions)
                    {
                        if (subscription.ChildWorkflowInstanceId == childWorkflowInstanceId)
                        {
                            found = subscription;

                            if (string.IsNullOrEmpty(subscription.QueueName) || subscription.QueueName == "0")
                            {
                                // the listener has not been registered
                                subscription.QueueName = (string)queueName;

                                this.subscriptions.Add(subscription.SubscriptionId, subscription);

                                persistToDB = true; // persist the subscription to db outside of lock scope
                            }

                            break;
                        }
                    }
                }

                if (found != null)
                {
                    if (persistToDB)
                    {
                        // update the subscription's queueName
                        workflowAdaptor.UpdateWFEventQueueName(found.SubscriptionId, (string)queueName);
                    }
                }
                else
                {
                    throw new Exception("Unable to find a workflow subscription for the child workflow with id " + childWorkflowInstanceId.ToString());
                }

                return new Guid(found.SubscriptionId);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Unregister an event listener
        /// </summary>
        /// <param name="subscriptionId"></param>
        public void UnregisterListener(Guid subscriptionId)
        {
            string key = subscriptionId.ToString();
            lock (this.subscriptions)
            {
                if (this.subscriptions.ContainsKey(key))
                {
                    this.subscriptions.Remove(key);

                    // do not delete the subscription from database, since
                    // they will be cleared up when a workflow is completed or terminated
                }
            }
        }

        /// <summary>
        /// Add a subscription to db that associates a child workflow instance with the parent workflow instance
        /// </summary>
        /// <param name="childWorkflowInstanceId">The child workflow instance id</param>
        /// <returns>The subscription id</returns>
        public Guid AddChildWorkflowEventSubscription(Guid childWorkflowInstanceId)
        {
            try
            {
                // WorkflowEnvironment.WorkflowInstanceId is the invoking workflow instance id
                // childWorkflowInstanceId is the invoked workflow instance id
                WorkflowEventSubscription subscription =
                    new WorkflowEventSubscription(WorkflowEnvironment.WorkflowInstanceId, childWorkflowInstanceId, "0"); // oracle takes empty as null

                Guid subscriptionId = Guid.NewGuid();

                WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();

                // persist the subscription to database for recovery
                workflowAdaptor.AddWorkflowEventSubscription(subscriptionId, subscription);

                return subscriptionId;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Raise an workflow complete or terminate event
        /// </summary>
        /// <param name="instanceId">Workflow instance id</param>
        /// <param name="runtime">Workflow runtime</param>
        /// <param name="e"></param>
        public void RaiseWorkflowEvent(Guid wfInstanceId, WorkflowRuntime runtime, NewteraEventArgs e)
        {
            WorkflowEventSubscription found = null;

            lock (this.subscriptions)
            {  
                foreach (WorkflowEventSubscription subscription in this.subscriptions.Values)
                {
                    if (subscription.ChildWorkflowInstanceId == wfInstanceId)
                    {
                        found = subscription;
                    }
                }
            }

            try
            {
                if (found != null)
                {
                    this.DeliverToParentWorkflow(found, e, runtime);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Remove the subscriptions associated with a workflow instance.
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id.</param>
        public void RemoveSubscriptions(Guid wfInstanceId)
        {
            lock (this.subscriptions)
            {
                StringCollection removedSubscriptionIds = new StringCollection();
                foreach (string key in this.subscriptions.Keys)
                {
                    WorkflowEventSubscription subscription = this.subscriptions[key];
                    // parent workflow instance is the one that register the subscription
                    if (subscription.ParentWorkflowInstanceId == wfInstanceId)
                    {
                        removedSubscriptionIds.Add(key);
                    }
                }

                foreach (string key in removedSubscriptionIds)
                {
                    if (this.subscriptions.ContainsKey(key))
                    {
                        this.subscriptions.Remove(key);
                    }
                }
            }

            // remove the subscription from the database
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            workflowModelAdapter.DeleteWorkflowEventSubscriptionByWFId(wfInstanceId.ToString());
        }

        /// <summary>
        /// Get the id of binding data instance to a workflow instance
        /// </summary>
        /// <param name="dataClassName">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>The id of the binding data instance, null if no binding exists.</returns>
        public string GetBindingDataInstanceId(string dataClassName, string schemaId, Guid workflowInstanceId)
        {
            NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(workflowInstanceId);
            if (wfInstance == null)
            {
                throw new WorkflowServerException("Unable to find the workflow instance with id " + workflowInstanceId.ToString());
            }

            return wfInstance.ObjId;
        }

        /// <summary>
        /// Set a binding between a data instance and a workflow instance
        /// </summary>
        /// <param name="dataInstanceId">The data instance id</param>
        /// <param name="className">The data class name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void SetWorkflowInstanceBinding(string dataInstanceId, string className, string schemaId, Guid workflowInstanceId)
        {
            NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(workflowInstanceId);
            if (wfInstance == null)
            {
                throw new WorkflowServerException("Unable to find the workflow instance with id " + workflowInstanceId.ToString());
            }

            // only if the workflow instance has not had a binding instance yet
            if (string.IsNullOrEmpty(wfInstance.ObjId))
            {
                wfInstance.ObjId = dataInstanceId;
                wfInstance.SchemaId = schemaId;
                wfInstance.ClassName = className;
                wfInstance.Save(); // save the updates to database
            }
        }

        /// <summary>
        /// Sets a parameter value of a workflow instance in the cache
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance id</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">parameter value</param>
        public void SetInputParameterValue(string workflowInstanceId, string parameterName, object parameterValue)
        {
            _workflowParameterTable.Add(workflowInstanceId + parameterName, parameterValue);
        }

        /// <summary>
        /// Gets a value of an input parameter of a workflow instance from the cache
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter value</returns>
        public object GetInputParameterValue(string workflowInstanceId, string parameterName)
        {
            object val = null;
            string key = workflowInstanceId + parameterName;
            if (_workflowParameterTable.ContainsKey(key))
            {
                val = _workflowParameterTable[key];
            }

            return val;
        }

        /// <summary>
        /// Remove a value of an input parameter of a workflow instance from the cache
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <param name="parameterName">The parameter name</param>
        public void RemoveInputParameterValue(string workflowInstanceId, string parameterName)
        {
            string key = workflowInstanceId + parameterName;
            if (_workflowParameterTable.ContainsKey(key))
            {
                _workflowParameterTable.Remove(key);
            }
        }

        /// <summary>
        /// Gets a collection of the on-going child workflow instance ids
        /// </summary>
        /// <param name="workflowInstanceId">The parent workflow instance id</param>
        /// <param name="activityName">The name of InvokeAsyncWorkflowActivity</param>
        /// <returns>A list of workflow instance ids.</returns>
        public List<Guid> GetChildWorkflowInstanceIds(string workflowInstanceId, string activityName)
        {
            List<Guid> childWorkflowInstanceIds =  new List<Guid>();

            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            WorkflowEventSubscriptionCollection workflowEventSubscriptions = workflowModelAdapter.GetWorkflowEventSubscriptionsByWFInstanceId(workflowInstanceId);
            foreach (WorkflowEventSubscription sub in workflowEventSubscriptions)
            {
                try
                {
                    WorkflowInstance childWFInstance = NewteraWorkflowRuntime.Instance.GetWorkflow(sub.ChildWorkflowInstanceId);
                    if (childWFInstance != null)
                    {
                        childWorkflowInstanceIds.Add(sub.ChildWorkflowInstanceId);
                    }
                }
                catch (Exception)
                {
                    // catch the exception of GetWorkflow for non-exist workflow instance
                }
            }

            return childWorkflowInstanceIds;
        }

        /// <summary>
        /// deliver the event to the workflow activity
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="args"></param>
        private void DeliverToParentWorkflow(WorkflowEventSubscription subscription, NewteraEventArgs args, WorkflowRuntime runtime)
        {
            try
            {
                WorkflowInstance workflowInstance = runtime.GetWorkflow(subscription.ParentWorkflowInstanceId);

                workflowInstance.EnqueueItem(subscription.QueueName, args, null, null);
            }
            catch (Exception e)
            {
                // Write the exception out to the Debug console and throw the exception
                //System.Diagnostics.Debug.WriteLine(e);
                throw e;
            }
        }
    }
}
