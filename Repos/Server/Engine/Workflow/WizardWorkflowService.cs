/*
* @(#)WizardWorkflowService.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Workflow.Activities;
using System.Workflow.Runtime;

using Newtera.WorkflowServices;
using Newtera.WFModel;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// This class is the host implementation of the wizard workflow service interface
    /// </summary>
    class WizardWorkflowService : IWizardWorkflowService, IDisposable
    {
        #region Members

        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private Dictionary<string, WizardPageData> dataValues;

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public WizardWorkflowService()
        {
            this.dataValues = new Dictionary<string, WizardPageData>();
        }
        #endregion

        #region Methods
  
        /// <summary>
        /// Gets the workflow information from the workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the workflow</param>
        /// <returns></returns>
        public WizardPageData RetrieveDataFromWorkflow(Guid instanceId)
        {
            WizardPageData WizardPageData = null;

            //get the data from the internal list
            string key = instanceId.ToString();
            this.dataValues.TryGetValue(key, out WizardPageData);
           
            //if the value doesn't exist wait till it does
            while (WizardPageData == null)
            {
                this.waitHandle.WaitOne();
                this.dataValues.TryGetValue(key, out WizardPageData);
            }
 
            //removes the value
            this.dataValues.Remove(key);
            return WizardPageData;
        }

        /// <summary>
        /// Sends Previous event to workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the workflow</param>
        /// <param name="data">The page information for this current process</param>
        public void SendPreviousEventToWorkflow(Guid instanceId, WizardPageData data)
        {
            if (PreviousEventHandler != null)
                PreviousEventHandler(null, new WorkflowEventArguments(instanceId, data));
        }

        /// <summary>
        /// Sends Next event to workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the workflow</param>
        /// <param name="data">The page information for this current process</param>
        public void SendNextEventToWorkflow(Guid instanceId, WizardPageData data)
        {
            if (NextEventHandler != null)
                NextEventHandler(null, new WorkflowEventArguments(instanceId, data));
        }

        /// <summary>
        /// Sends Cancel event to workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the workflow</param>
        /// <param name="data">The page information for this current process</param>
        public void SendCancelEventToWorkflow(Guid instanceId, WizardPageData data)
        {
            if (CancelEventHandler != null)
                CancelEventHandler(null, new WorkflowEventArguments(instanceId, data));
        }

        /// <summary>
        /// Sends Finish event to workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the workflow</param>
        /// <param name="data">The page information for this current process</param>
        public void SendFinishEventToWorkflow(Guid instanceId, WizardPageData data)
        {
            if (FinishEventHandler != null)
                FinishEventHandler(null, new WorkflowEventArguments(instanceId, data));
        }

        #endregion

        #region IWizardWorkflowService Members
        /// <summary>
        /// Handles the call back into this object from the workflow
        /// </summary>
        /// <param name="pageComposition">Page Data</param>
        public void SendWorkflowInformationToHost(WizardPageData data)
        {
            String key = WorkflowEnvironment.WorkflowInstanceId.ToString();
            this.dataValues[key] = data;
            this.waitHandle.Set();
        }

        public event EventHandler<WorkflowEventArguments> NextEventHandler;

        public event EventHandler<WorkflowEventArguments> PreviousEventHandler;

        public event EventHandler<WorkflowEventArguments> CancelEventHandler;

        public event EventHandler<WorkflowEventArguments> FinishEventHandler;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.waitHandle.Close();
        }

        #endregion
    }
}
