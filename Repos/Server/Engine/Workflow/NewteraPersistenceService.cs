/*
* @(#)NewteraPersistenceService.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Collections.Specialized;
using System.IO;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Threading;

using Newtera.Common.Core;
using Newtera.Server.DB;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary> 
	/// Implements a workflow persistence service using Newtera Data Store
	/// </summary>
	/// <version> 1.0.0 18 Dec 2006 </version>
    public class NewteraPersistenceService : WorkflowPersistenceService
    {
        private bool unloadOnIdle = false;

        /// <summary>
        /// Create an instance of NewteraPersistenceService
        /// </summary>
        /// <param name="unloadOnIdle"></param>
        public NewteraPersistenceService(bool unloadOnIdle)
        {
            this.unloadOnIdle = unloadOnIdle;
        }

        /// <summary>
        /// Save the workflow instance state at the point of persistence with option of locking
        /// the instance state if it is shared
        /// across multiple runtimes or multiple phase instance updates
        /// </summary>
        /// <param name="rootActivity"></param>
        /// <param name="unlock"></param>
        protected override void SaveWorkflowInstanceState(Activity rootActivity, bool unlock)
        {
            try
            {
                // Save the workflow
                Guid contextGuid = (Guid)rootActivity.GetValue(Activity.ActivityContextGuidProperty);

                SerializeInstanceStateToDatabase(
                    WorkflowPersistenceService.GetDefaultSerializedForm(rootActivity), contextGuid, unlock);

                // See when the next timer (Delay activity) for this workflow will expire
                TimerEventSubscriptionCollection timers = (TimerEventSubscriptionCollection)rootActivity.GetValue(TimerEventSubscriptionCollection.TimerCollectionProperty);
                TimerEventSubscription subscription = timers.Peek();
                if (subscription != null)
                {
                    // Set a system timer to automatically reload this workflow when its next timer expires
                    TimerCallback callback = new TimerCallback(ReloadWorkflow);
                    TimeSpan timeDifference = subscription.ExpiresAt - DateTime.UtcNow;
                    System.Threading.Timer timer = new System.Threading.Timer(
                        callback,
                        subscription.WorkflowInstanceId,
                        timeDifference < TimeSpan.Zero ? TimeSpan.Zero : timeDifference,
                        new TimeSpan(-1));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Reload the workflow instance
        /// </summary>
        /// <param name="instanceId">instance id </param>
        private void ReloadWorkflow(object instanceId)
        {
            // Reload the workflow so that it will continue processing
            this.Runtime.GetWorkflow((Guid) instanceId).Load();
        }

        /// <summary>
        /// Load workflow instance state.
        /// </summary>
        /// <param name="instanceId">Workflow instance if</param>
        /// <returns>Activity</returns>
        protected override Activity LoadWorkflowInstanceState(Guid instanceId)
        {
            try
            {
                byte[] workflowBytes = DeserializeInstanceStateFromDatabase(instanceId);

                return WorkflowPersistenceService.RestoreFromDefaultSerializedForm(workflowBytes, null);
            }
            catch (Exception ex)
            {
                //ErrorLog.Instance.WriteLine("LoadWorkflowInstanceState RestoreFromDefaultSerializedForm error: " + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Unlock the workflow instance state. 
        /// Instance state locking is necessary when multiple runtimes share instance persistence store
        /// </summary>
        /// <param name="state"></param>
        protected override void UnlockWorkflowInstanceState(Activity state)
        {
            //locking is not supported at this time
        }

        /// <summary>
        /// Save the completed activity state.
        /// </summary>
        /// <param name="activity"></param>
        protected override void SaveCompletedContextActivity(Activity activity)
        {
            Guid contextGuid = (Guid)activity.GetValue(Activity.ActivityContextGuidProperty);
            
            SerializeCompletedScopeToDatabase(
                WorkflowPersistenceService.GetDefaultSerializedForm(activity), contextGuid);
        }

        /// <summary>
        /// Load the completed activity state.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="outerActivity"></param>
        /// <returns></returns>
        protected override Activity LoadCompletedContextActivity(Guid activityId, Activity outerActivity)
        {
            try
            {
                byte[] workflowBytes = DeserializeCompletedScopeFromDatabase(activityId);
                Activity deserializedActivities = WorkflowPersistenceService.RestoreFromDefaultSerializedForm(workflowBytes, outerActivity);
                return deserializedActivities;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the information indicating whether to unload workflow on idle
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        protected override bool UnloadOnIdle(Activity activity)
        {
            return unloadOnIdle;
        }

        // Serialize the workflow instance state to the database
        private void SerializeInstanceStateToDatabase(byte[] workflowBytes, Guid id, bool unlock)
        {
            String instanceId = id.ToString();

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            adapter.WriteInstanceState(instanceId, workflowBytes, unlock);
        }

        // Deserialize the instance state from the database given the instance id 
        private byte[] DeserializeInstanceStateFromDatabase(Guid id)
        {
            String instanceId = id.ToString();

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.ReadInstanceState(instanceId);
        }

        // Serialize the completed scope to the database
        private void SerializeCompletedScopeToDatabase(byte[] workflowBytes, Guid id)
        {
            String instanceId = id.ToString();

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            adapter.WriteCompletedScope(instanceId, workflowBytes);
        }

        // Deserialize the completed scope from the database given the activity id 
        private byte[] DeserializeCompletedScopeFromDatabase(Guid id)
        {
            String instanceId = id.ToString();

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.ReadCompletedScope(instanceId);
        }
    }
}