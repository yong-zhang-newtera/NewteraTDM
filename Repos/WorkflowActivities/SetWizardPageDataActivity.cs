/*
* @(#)SetWizardPageDataActivity.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

using System.Collections.Generic;

using Newtera.WFModel;
using Newtera.WorkflowServices;

namespace Newtera.Activities
{
    public partial class SetWizardPageDataActivity : System.Workflow.ComponentModel.Activity
	{
		public SetWizardPageDataActivity()
		{
			//InitializeComponent();
		}

        public static DependencyProperty SessionDataProperty = System.Workflow.ComponentModel.DependencyProperty.Register("SessionData", typeof(WorkflowSession), typeof(SetWizardPageDataActivity));

        [Description("This is the sesion information")]
        [Category("Input Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowSession SessionData
        {
            get
            {
                return ((WorkflowSession)(base.GetValue(SetWizardPageDataActivity.SessionDataProperty)));
            }
            set
            {
                base.SetValue(SetWizardPageDataActivity.SessionDataProperty, value);
            }
        }

        public static DependencyProperty EventArgsProperty = System.Workflow.ComponentModel.DependencyProperty.Register("EventArgs", typeof(WorkflowEventArguments), typeof(SetWizardPageDataActivity));

        [Description("The event arguments passed in the the external event handler")]
        [Category("Input Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowEventArguments EventArgs
        {
            get
            {
                return ((WorkflowEventArguments)(base.GetValue(SetWizardPageDataActivity.EventArgsProperty)));
            }
            set
            {
                base.SetValue(SetWizardPageDataActivity.EventArgsProperty, value);
            }
        }
	
        private string dataKeyValue;

        public string DataKeyValue
        {
            get { return dataKeyValue; }
            set { dataKeyValue = value; }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            if (this.SessionData != null && this.EventArgs != null)
            {
                this.SessionData.PageDataCollection[this.dataKeyValue] = this.EventArgs.Data;
            }
            return ActivityExecutionStatus.Closed;
        }
	}
}
