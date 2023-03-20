/*
* @(#)GetWizardPageDataActivity.cs
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

namespace Newtera.Activities
{
    public partial class GetWizardPageDataActivity : System.Workflow.ComponentModel.Activity
	{
		public GetWizardPageDataActivity()
		{
			InitializeComponent();
		}

        public static DependencyProperty SessionDataProperty = System.Workflow.ComponentModel.DependencyProperty.Register("SessionData", typeof(WorkflowSession), typeof(GetWizardPageDataActivity));

        [Description("This is the workflow session information")]
        [Category("Input Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowSession SessionData
        {
            get
            {
                return ((WorkflowSession)(base.GetValue(GetWizardPageDataActivity.SessionDataProperty)));
            }
            set
            {
                base.SetValue(GetWizardPageDataActivity.SessionDataProperty, value);
            }
        }

        public static DependencyProperty RetrievedPageDataProperty = System.Workflow.ComponentModel.DependencyProperty.Register("RetrievedPageData", typeof(WizardPageData), typeof(GetWizardPageDataActivity));

        [Description("This is the page data")]
        [Category("Output Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WizardPageData RetrievedPageData
        {
            get
            {
                return ((WizardPageData)(base.GetValue(GetWizardPageDataActivity.RetrievedPageDataProperty)));
            }
            set
            {
                base.SetValue(GetWizardPageDataActivity.RetrievedPageDataProperty, value);
            }
        }

        private string dataKeyValue;

        public string DataKeyValue
        {
            get { return dataKeyValue; }
            set { dataKeyValue = value; }
        }

        private string controlName;

        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }
	

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            if (this.SessionData != null && this.SessionData.PageDataCollection.Count > 0)
            {
                if (this.SessionData.PageDataCollection.ContainsKey(this.dataKeyValue))
                {
                    this.RetrievedPageData = this.SessionData.PageDataCollection[this.dataKeyValue];
                }
            }
            
            if (this.RetrievedPageData == null)
            {
                this.RetrievedPageData = new WizardPageData();
                this.RetrievedPageData.ControlName = this.controlName;
            }
            
            return ActivityExecutionStatus.Closed;
        }
	}
}
