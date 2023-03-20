/*
* @(#)NewteraStateActivityToolboxItem.cs
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
    public partial class CompositeStateActivity : ActivityToolboxItem
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        public CompositeStateActivity() : base()
        {
        }
 
        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public CompositeStateActivity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
 
        protected override IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            //create the primary state activity
            StateActivity activity = new StateActivity();

            //add an StateInitialization activity
            StateInitializationActivity initialization = new StateInitializationActivity("StateInitialization");
            // add CreateTaskActivity
            CreateTaskActivity createTask = new CreateTaskActivity();
            createTask.Name = "CreateTask1";
            initialization.Activities.Add(createTask);
            activity.Activities.Add(initialization);

            // add an EventDrivenActivity for next event
            EventDrivenActivity eventDriven = new EventDrivenActivity("NextEvent");
            HandleNewteraEventActivity handleEventActivity = new HandleNewteraEventActivity();
            handleEventActivity.Name = "HandleEvent";
            eventDriven.Activities.Add(handleEventActivity);
            activity.Activities.Add(eventDriven);

            //add a StateFinalization
            StateFinalizationActivity finalization = new StateFinalizationActivity("StateFinalization");
            // add SetWizardPageDataActivity
            CloseTaskActivity closeTask = new CloseTaskActivity();
            closeTask.Name = "CloseTask";
            finalization.Activities.Add(closeTask);
            activity.Activities.Add(finalization);
 
            return new IComponent[] { activity };
        }
	}
}
