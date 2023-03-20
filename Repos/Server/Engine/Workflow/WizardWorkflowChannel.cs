/*
* @(#)WizardWorkflowChannel.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using System.Workflow.Runtime;

using Newtera.WorkflowServices;
using Newtera.WFModel;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// This class represents a channel into the workflow that is based on the state machine service
    /// </summary>
    public static class WizardWorkflowChannel
    {
        #region Static Methods
        
        /// <summary>
        /// This method returns a reference to the spoke wizard service
        /// </summary>
        /// <returns></returns>
        private static WizardWorkflowService GetService()
        {
            return (WizardWorkflowService)NewteraWorkflowRuntime.Instance.GetWorkflowRunTime().GetService(typeof(WizardWorkflowService));
        }

        /// <summary>
        /// Processes the Previous request and sends it to the workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the current workflow</param>
        /// <param name="pageComposition">The current page data to send to the workflow</param>
        /// <returns>New Page Datat Information</returns>
        public static WizardPageData ProcessPreviousRequest(Guid instanceId, WizardPageData data)
        {
            WizardWorkflowService WizardWorkflowService = GetService();
            
            //send next event to workflow moving to the next state
            WizardWorkflowService.SendPreviousEventToWorkflow(instanceId, data);

            //have scheduler run event
            NewteraWorkflowRuntime.Instance.RunWorkflowInstance(instanceId);

            //return data which is the next page
            return WizardWorkflowService.RetrieveDataFromWorkflow(instanceId);

        }

        /// <summary>
        /// Processes the Next request and sends it to the workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the current workflow</param>
        /// <param name="pageComposition">The current page data to send to the workflow</param>
        /// <returns>New Page Datat Information</returns>
        public static WizardPageData ProcessNextRequest(Guid instanceId, WizardPageData data)
        {
            WizardWorkflowService WizardWorkflowService = GetService();

            //send next event to workflow moving to the next state
            WizardWorkflowService.SendNextEventToWorkflow(instanceId, data);

            //have scheduler run event
            NewteraWorkflowRuntime.Instance.RunWorkflowInstance(instanceId);

            //return data which is the next page
            return WizardWorkflowService.RetrieveDataFromWorkflow(instanceId);

        }

        /// <summary>
        /// Processes the Canel request and sends it to the workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the current workflow</param>
        /// <param name="pageComposition">The current page data to send to the workflow</param>
        /// <returns>New Page Datat Information</returns>
        public static WizardPageData ProcessCancelRequest(Guid instanceId, WizardPageData data)
        {
            WizardWorkflowService WizardWorkflowService = GetService();

            //send next event to workflow moving to the next state
            WizardWorkflowService.SendCancelEventToWorkflow(instanceId, data);

            //have scheduler run event
            NewteraWorkflowRuntime.Instance.RunWorkflowInstance(instanceId);

            //return data which is the next page
            return WizardWorkflowService.RetrieveDataFromWorkflow(instanceId);

        }

        /// <summary>
        /// Processes the Finish request and sends it to the workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the current workflow</param>
        /// <param name="pageComposition">The current page data to send to the workflow</param>
        /// <returns>New Page Datat Information</returns>
        public static WizardPageData ProcessFinishRequest(Guid instanceId, WizardPageData data)
        {

            WizardWorkflowService WizardWorkflowService = GetService();
            
            //send next event to workflow moving to the next state
            WizardWorkflowService.SendFinishEventToWorkflow(instanceId, data);

            //have scheduler run event
            NewteraWorkflowRuntime.Instance.RunWorkflowInstance(instanceId);

            //return data which is the next page
            return WizardWorkflowService.RetrieveDataFromWorkflow(instanceId);

        }

        /// <summary>
        /// Get data from workflow
        /// </summary>
        /// <param name="instanceId">The instanceId of the current workflow</param>
        /// <returns>New Page Datat Information</returns>
        public static WizardPageData GetWorkflowInformation(Guid instanceId)
        {

            WizardWorkflowService WizardWorkflowService = GetService();

            //return data which is the next page
            return WizardWorkflowService.RetrieveDataFromWorkflow(instanceId);
        }

        #endregion
    }
}
