/*
* @(#)WizardStateActivityToolboxItem.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Workflow.Activities;
using System.Workflow.ComponentModel.Design;

using Newtera.WFModel;
using Newtera.WorkflowServices;

namespace Newtera.Activities
{
    public partial class WizardStateActivityToolboxItem : ActivityToolboxItem
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        public WizardStateActivityToolboxItem() : base()
        {
        }
 
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public WizardStateActivityToolboxItem(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
 
        protected override IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            //create the primary state activity
            StateActivity activity = new StateActivity();

            //add an StateInitialization activity
            StateInitializationActivity initialization = new StateInitializationActivity("WizardStateInitialization");
            // add GetWizardPageDataActivity
            GetWizardPageDataActivity getWizardPage = new GetWizardPageDataActivity();
            getWizardPage.Name = "GetWizardPageData";
            initialization.Activities.Add(getWizardPage);
            // Add CallExternalMethodActivity
            CallExternalMethodActivity callExternalMethodActivity = new CallExternalMethodActivity();
            callExternalMethodActivity.Name = "CallExternalMethod";
            callExternalMethodActivity.InterfaceType = typeof(IWizardWorkflowService);
            callExternalMethodActivity.MethodName = "SendWorkflowInformationToHost";
            initialization.Activities.Add(callExternalMethodActivity);
            activity.Activities.Add(initialization);

            // add an EventDrivenActivity for next event
            EventDrivenActivity eventDriven = new EventDrivenActivity("NextEvent");
            HandleExternalEventActivity handleExternalActivity = new HandleExternalEventActivity("HandleNextEvent");
            handleExternalActivity.InterfaceType = typeof(IWizardWorkflowService);
            handleExternalActivity.EventName = "NextEventHandler";
            eventDriven.Activities.Add(handleExternalActivity);
            activity.Activities.Add(eventDriven);

            // add an EventDrivenActivity for previous event
            eventDriven = new EventDrivenActivity("PreviousEvent");
            handleExternalActivity = new HandleExternalEventActivity("HandlePreviousEvent");
            handleExternalActivity.InterfaceType = typeof(IWizardWorkflowService);
            handleExternalActivity.EventName = "PreviousEventHandler";
            eventDriven.Activities.Add(handleExternalActivity);
            activity.Activities.Add(eventDriven);

            // add an EventDrivenActivity for cancel event
            eventDriven = new EventDrivenActivity("CancelEvent");
            handleExternalActivity = new HandleExternalEventActivity("HandleCancelEvent");
            handleExternalActivity.InterfaceType = typeof(IWizardWorkflowService);
            handleExternalActivity.EventName = "CancelEventHandler";
            eventDriven.Activities.Add(handleExternalActivity);
            activity.Activities.Add(eventDriven);

            // add an EventDrivenActivity for finish event
            eventDriven = new EventDrivenActivity("FinishEvent");
            handleExternalActivity = new HandleExternalEventActivity("HandleFinishEvent");
            handleExternalActivity.InterfaceType = typeof(IWizardWorkflowService);
            handleExternalActivity.EventName = "FinishEventHandler";
            eventDriven.Activities.Add(handleExternalActivity);
            activity.Activities.Add(eventDriven);

            //add a StateFinalization
            StateFinalizationActivity finalization = new StateFinalizationActivity("WizardStateFinalization");
            // add SetWizardPageDataActivity
            SetWizardPageDataActivity setWizardPage = new SetWizardPageDataActivity();
            setWizardPage.Name = "SetWizardPageData";
            finalization.Activities.Add(setWizardPage);
            activity.Activities.Add(finalization);
 
            return new IComponent[] { activity };
        }
	}
}
