/*
* @(#)WorkflowEventArguments.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowServices
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Workflow.Activities;

    using Newtera.WFModel;

    /// <summary>
    /// This class is the event argument for the workflow events
    /// </summary>
    [Serializable]
    public class WorkflowEventArguments : ExternalDataEventArgs
    {
        #region Constructors

        public WorkflowEventArguments(Guid instanceId, WizardPageData data)
            : base(instanceId)
        {
            this.data = data;
        }

        #endregion

        #region Fields

        private WizardPageData data;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the page information for the event
        /// </summary>
        public WizardPageData Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion
    }
}
