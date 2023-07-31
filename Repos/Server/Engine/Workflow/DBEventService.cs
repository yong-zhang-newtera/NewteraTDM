/*
* @(#)DBEventService.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Collections.Specialized;

using Newtera.WorkflowServices;
using Newtera.Server.DB;
using Newtera.Server.Util;
using Newtera.Common.Core;
using Newtera.WFModel;
using Newtera.Common.Wrapper;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// This is a single DBEventService in the server process. It is created by the DBEventServiceSingleton
    /// </summary>
    public class DBEventService : IDBEventService
    {
        private IKeyValueStore _subscriptions;

        /// <summary>
        /// Create a DBEventService
        /// </summary>
        public DBEventService()
        {
            this._subscriptions = KeyValueStoreFactory.TheInstance.Create("DBEventService.Subscriptions");

            // get the subscriptions from the database
            WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();

            this._subscriptions.Initialize(workflowAdaptor.GetEventSubscriptions());
        }

        /// <summary>
        /// Register an event listener
        /// </summary>
        /// <param name="queueName">The queue name</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="eventName">The event name</param>
        /// <param name="createDataBinding">Information indicating whether to create a binding between the workflow instance and the data instance that raises the event</param>
        /// <returns>The subscription id</returns>
        public Guid RegisterListener(IComparable queueName, string schemaId, string className, string eventName, bool createDataBinding)
        {
            try
            {
                DBEventSubscription subscription =
                    new DBEventSubscription(schemaId, className, eventName, WorkflowEnvironment.WorkflowInstanceId, (string)queueName, createDataBinding);

                Guid subscriptionId = Guid.NewGuid();

                WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();

                this._subscriptions.Add(subscriptionId.ToString(), subscription);

                // persist the subscription to database for recovery
                workflowAdaptor.AddEventSubscription(subscriptionId, subscription);

                return subscriptionId;
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
            if (this._subscriptions.Contains(key))
            {
                this._subscriptions.Remove(key);
            }

            RemoveSubscription(key); // remove subscription from db
        }

        /// <summary>
        /// Raise an data changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RaiseDataChangedEvent(Guid wfInstanceId, WorkflowRuntime runtime, NewteraEventArgs e)
        {
            DBEventSubscription theSubscription = null;

            var keys = this._subscriptions.GetKeys();
            foreach (var key in keys)
            {
                var subscription = this._subscriptions.Get<DBEventSubscription>(key);
                if (subscription != null &&
                    subscription.SchemaId == e.SchemaId &&
                    subscription.ClassName == e.ClassName &&
                    subscription.EventName == e.EventName)
                {
                    if (wfInstanceId == Guid.Empty)
                    {
                        // If a workflow isn't started with an Newtera event,
                        // it doesn't have a binding data instance. Therefore, we
                        // need to bind the data instance to the workflow instace which first register
                        // the subscription for the event.
                        NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(subscription.WorkflowInstanceId);
                        if (wfInstance == null)
                        {
                            // the workflow instance may have been terminated, ignore the subscription, stop the searching
                            break;

                            //throw new WorkflowServerException("Unable to find the binding instance for the workflow instance with id " + subscription.WorkflowInstanceId.ToString());
                        }

                        if (string.IsNullOrEmpty(wfInstance.ObjId) && subscription.CreateDataBinding)
                        {
                            // update a binding between the workflow instance and data instance that raised the event
                            wfInstance.ObjId = e.ObjId;
                            wfInstance.ClassName = e.ClassName;
                            wfInstance.SchemaId = e.SchemaId;
                            wfInstance.Save(); // save the changes to the database

                            // After establish the binding, now we can deliver
                            // the event to the workflow instance that matches the event spec.
                            theSubscription = subscription;
                            break;
                        }
                    }
                    else if (subscription.WorkflowInstanceId == wfInstanceId)
                    {
                        // the binding exists between the workflow instance and data instance
                        // deliver the event to the bound workflow instance
                        theSubscription = subscription;

                        // clear the data instance which may has been saved in the cache
                        InstanceWrapperFactory.Instance.BindingInstanceService.ClearBindingInstance(subscription.WorkflowInstanceId);
                        break;
                    }
                }
            }

            try
            {
                if (theSubscription != null)
                {
                    if (TraceLog.Instance.Enabled)
                    {
                        string[] messages = { "Event " + e.EventName + " matches a subscription",
                                "Schema Name: " + theSubscription.SchemaId,
                                "Class Name: " + theSubscription.ClassName,
                                "Queue Name: " + theSubscription.QueueName,
                                "Workflow Instance: " + wfInstanceId.ToString()};
                        TraceLog.Instance.WriteLines(messages);
                    }

                    // if there is an user id in the event context, we want to it to the task info in the cache
                    // so that we will know who executed the task of the workflow
                    if (!string.IsNullOrEmpty(e.UserId))
                    {
                        Newtera.Server.Engine.Cache.UserTaskCache.Instance.SetUserIdToTask(theSubscription.SchemaId, theSubscription.WorkflowInstanceId, e.UserId);
                    }

                    this.DeliverToWorkflow(theSubscription, e, runtime);
                }
                else
                {
                    if (TraceLog.Instance.Enabled)
                    {
                        string[] messages = { "Event " + e.EventName + " does not matche any _subscriptions.",
                                "Schema Name: " + e.SchemaId,
                                "Class Name: " + e.ClassName,
                                "ObjId: " + e.ObjId};
                        TraceLog.Instance.WriteLines(messages);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Remove the _subscriptions associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public void RemoveSubscriptions(Guid workflowInstanceId)
        {
            StringCollection removedSubscriptionIds = new StringCollection();
            var keys = this._subscriptions.GetKeys();
            foreach (string key in keys)
            {
                DBEventSubscription subscription = this._subscriptions.Get<DBEventSubscription>(key);
                if (subscription.WorkflowInstanceId == workflowInstanceId)
                {
                    removedSubscriptionIds.Add(key);
                }
            }

            foreach (string key in removedSubscriptionIds)
            {
                if (this._subscriptions.Contains(key))
                {
                    this._subscriptions.Remove(key);
                }
            }

            // remove the subscription from the database
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            workflowModelAdapter.DeleteSubscriptionByWFInstanceId(workflowInstanceId.ToString());
        }

        /// <summary>
        /// Remove the subscription of given id.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        public void RemoveSubscription(string subscriptionId)
        {
            try
            {
                // remove the subscription from the database
                WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
                workflowModelAdapter.DeleteEventSubscription(subscriptionId);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("Failed to delete an subscription " + ex.Message);
            }
        }

        /// <summary>
        /// deliver the event to the workflow activity
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="args"></param>
        private void DeliverToWorkflow(DBEventSubscription subscription, NewteraEventArgs args, WorkflowRuntime runtime)
        {
            WorkflowInstance workflowInstance = runtime.GetWorkflow(subscription.WorkflowInstanceId);

            workflowInstance.Load();

            // Deliver the event when the workflow instance is idled to make sure that
            // the subscriptions to the events have been established
            workflowInstance.EnqueueItemOnIdle(subscription.QueueName, args, null, null);
        }
    }
}
