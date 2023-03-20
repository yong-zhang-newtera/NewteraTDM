/*
* @(#)IWizardWorkflowService.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowServices
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Workflow.ComponentModel;
    using System.Workflow.Activities;

    using Newtera.WFModel;

    /// <summary>
    /// This interface is the main interface used by spoke wizard services
    /// </summary>
    [ExternalDataExchange]
    public interface IWizardWorkflowService
    {
        #region Methods
        /// <summary>
        /// Method used to send data from the workflow to the host
        /// </summary>
        /// <param name="data">Data the page will need to display itself</param>
        void SendWorkflowInformationToHost(WizardPageData data);

        #endregion


        #region Events
        /// <summary>
        /// The event that moves the order to the next step
        /// </summary>
        event EventHandler<WorkflowEventArguments> NextEventHandler;
        /// <summary>
        /// The event that moves the order to the previous step
        /// </summary>
        event EventHandler<WorkflowEventArguments> PreviousEventHandler;
        /// <summary>
        /// The event that cancels the order
        /// </summary>
        event EventHandler<WorkflowEventArguments> CancelEventHandler;
        /// <summary>
        /// The event that completes the order
        /// </summary>
        event EventHandler<WorkflowEventArguments> FinishEventHandler;

        #endregion

    }
}
