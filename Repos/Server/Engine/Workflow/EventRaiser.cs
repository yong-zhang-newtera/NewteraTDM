/*
* @(#)EventRaiser.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Xml;
    using System.Data;
    using System.IO;
	using System.Collections;
    using System.Threading;
    using System.Collections.Specialized;
    using System.Workflow.Runtime;
    using System.Security.Principal;
    using System.Runtime.Remoting.Messaging;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.Wrapper;
    using Newtera.Server.DB;
    using Newtera.Server.Engine.Cache;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;
    using Newtera.WFModel;
    using Newtera.WorkflowServices;

	/// <summary> 
	/// Holding the evnet context for the insert, update, and delete instance.
	/// </summary>
	/// <version>  	1.0.0 27 Dec 2006</version>
	public class EventRaiser
	{
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";
        private EventContext _context;
 
        /// <summary>
        /// Initiating an EventRaiser instance
        /// </summary>
        public EventRaiser()
        {
            _context = null;
        }
 
		/// <summary>
        /// Initiating an EventRaiser instance
		/// </summary>
        /// <param name="context">The event context</param>
        public EventRaiser(EventContext context)
		{
            _context = context;
		}

        /// <summary>
        /// run the event raiser async
        /// </summary>
        public void Run()
        {
            // run the event raiser using a worker thread from the thread pool
            RunEventAsyncDelegate runEventDelegate = new RunEventAsyncDelegate(RunEventAsync);

            AsyncCallback callback = new AsyncCallback(RunEventCallback);

            // save the event in DB in case of failure of raising the event
            //SaveEventContext(_context);

            runEventDelegate.BeginInvoke(callback, null);
        }

        private delegate void RunEventAsyncDelegate();

        /// <summary>
        /// run the event raiser
        /// </summary>
        private void RunEventAsync()
        {
            // attach a CustomPrincipla object to the worker thread with a fake user name in order to run xquery to evaluate the event condition against instanes
            CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy(), "EventRaiser");

            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                // check if all conditions are met for any of the events defined for the class,
                // if so, raise the event to the workflow event handler
                EventCollection events = _context.MetaData.EventManager.GetClassEvents(_context.ClassElement);
                foreach (EventDef eventDef in events)
                {
                    // check if conditions for raising the event is met
                    if (IsEventConditionMet(eventDef))
                    {
                        if (eventDef.OperationType != OperationType.Delete)
                        {
                            // trace the event that took place
                            if (TraceLog.Instance.Enabled)
                            {
                                string converted = "";

                                if (eventDef.AfterCondition != null)
                                {
                                    // convert the condition from expression to a text
                                    FlattenSearchFiltersVisitor visitor = new FlattenSearchFiltersVisitor();

                                    eventDef.AfterCondition.Accept(visitor);

                                    DataViewElementCollection flattenExprs = visitor.FlattenedSearchFilters;

                                    foreach (IDataViewElement element in flattenExprs)
                                    {
                                        converted += element.ToString();
                                    }
                                }

                                string[] messages = { eventDef.Name + " event is raised.",
                                    "Event Class: " + eventDef.ClassName,
                                    "Event Operation: " + eventDef.OperationType.ToString(),
                                    "Event Condition: " + converted};
                                TraceLog.Instance.WriteLines(messages);
                            }

                            // first check if event is used to start a workflow. if yes, the data instance
                            // is bound to the started workflow instance, stop sending the event to other
                            // workflow instance
                            if (!RaiseEventToStartWorkflow(eventDef))
                            {
                                if (TraceLog.Instance.Enabled)
                                {
                                    string[] messages = { "Event " + eventDef.Name + " is sent to a running workflow instance which may listen to the event." };
                                    TraceLog.Instance.WriteLines(messages);
                                }

                                // no workflow has been started by the event, then try to send
                                // the event to the running workflows in case there is a workflow
                                // waiting for the event
                                RaiseEventToWorkflowActivity(eventDef);
                            }
                            else
                            {
                                if (TraceLog.Instance.Enabled)
                                {
                                    string[] messages = { "Event " + eventDef.Name + " is used to start a workflow instance." };
                                    TraceLog.Instance.WriteLines(messages);
                                }
                            }
                        }

                        // raise the event to its subscribers
                        RaiseEventToSubscribers(eventDef, _context.ClassElement.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                // write the error message to the log
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private void RunEventCallback(IAsyncResult ar)
        {
            //DeleteEventContext(_context);

            // Retrieve the delegate.
            AsyncResult result = (AsyncResult)ar;
            RunEventAsyncDelegate caller = (RunEventAsyncDelegate)result.AsyncDelegate;

            // Call EndInvoke to avoid memory leak
            caller.EndInvoke(ar);
        }

        /// <summary>
        /// Save an event to database
        /// </summary>
        /// <param name="eventContext">The event context</param>
        private void SaveEventContext(EventContext eventContext)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            adapter.AddDBEventContext(eventContext);
        }

        /// <summary>
        /// Delete an event from database
        /// </summary>
        /// <param name="eventContext">The event context</param>
        private void DeleteEventContext(EventContext eventContext)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            adapter.DeleteDBEventContext(eventContext.EventContextId);
        }

        /// <summary>
        /// Raise a special event to the workflow activity through NewteraEventService when a workflow task is completed,
        /// this method is used when a task created by CreateGroupTaskActivity is completed to notify the HandleGroupTaskActivity
        /// </summary>
        /// <param name="schemaId">The id of a schema the the workflow instance is bound to</param>
        /// <param name="boundClassName">The name of a class the the workflow instance is bound to</param>
        /// <param name="boundInstanceId">The id of an data instance the the workflow instance is bound to</param>
        /// <param name="completedTaskID">The id the completed workflow task</param>
        public void RaiseTaskCompletedEventToWorkflowActivity(string schemaId, string boundClassName, string boundInstanceId, string completedTaskID)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(boundInstanceId);
            Guid workflowInstaceId = Guid.Empty;
            if (bindingInfo != null)
            {
                // there exists a binding between the data instance and a workflow instance
                // raise the event to the workflow instance
                workflowInstaceId = new Guid(bindingInfo.WorkflowInstanceId);
            }
 
            string eventName = "CloseTask_" + completedTaskID; // CloseTask is prefix of the event name, which is the ame as the one defined in HandleGroupTaskActivity 
            NewteraEventArgs eventArgs = new NewteraEventArgs(schemaId,
                boundClassName,
                eventName,
                boundInstanceId,
                completedTaskID);

            DBEventServiceSingleton.Instance.RaiseDataChangedEvent(workflowInstaceId,
                NewteraWorkflowRuntime.Instance.GetWorkflowRunTime(),
                eventArgs);
        }

        /// <summary>
        /// Gets the information indicating whether conditions of the event is met by the
        /// information provided in the context.
        /// </summary>
        /// <param name="eventDef">The event definition.</param>
        /// <returns>true if the condition is met, false otherwise.</returns>
        private bool IsEventConditionMet(EventDef eventDef)
        {
            bool status = false;

            if (eventDef.OperationType == _context.OperationType)
            {
                if (eventDef.OperationType == OperationType.Insert)
                {
                    // check if the event condition expression is evaluated to true
                    if (eventDef.AfterCondition == null ||
                        EvaluateCondition(eventDef))
                    {
                        status = true;
                    }
                }
                else if (eventDef.OperationType == OperationType.Update)
                {
                    if (!IsAttributeValuesUpdated(eventDef))
                    {
                        return false;
                    }

                    // check if the event condition expression is evaluated to true
                    if (eventDef.AfterCondition == null ||
                        EvaluateCondition(eventDef))
                    {
                        status = true;
                    }
                }
                else if (eventDef.OperationType == OperationType.Delete)
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Raise the event to start a workflow whose start event matches the given event
        /// </summary>
        /// <param name="eventDef">The event</param>
        /// <returns>true if there is a workflow started by the event, false, otherwise.</returns>
        private bool RaiseEventToStartWorkflow(EventDef eventDef)
        {
            NewteraEventArgs eventArgs = new NewteraEventArgs(_context.MetaData.SchemaInfo.NameAndVersion,
                    _context.ClassElement.Name,
                    eventDef.Name,
                    _context.ObjId);

            return NewteraWorkflowRuntime.Instance.StartWorkflowEventHandler(this, eventArgs);
        }

        /// <summary>
        /// Raise the event to the workflow activity through NewteraEventService
        /// </summary>
        /// <param name="eventDef">The event</param>
        private void RaiseEventToWorkflowActivity(EventDef eventDef)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(_context.ObjId);
            Guid workflowInstaceId = Guid.Empty;
            if (bindingInfo != null)
            {
                // there exists a binding between the data instance and a workflow instance
                // raise the event to the workflow instance
                workflowInstaceId = new Guid(bindingInfo.WorkflowInstanceId);
            }
           
            NewteraEventArgs eventArgs = new NewteraEventArgs(_context.MetaData.SchemaInfo.NameAndVersion,
                _context.ClassElement.Name,
                eventDef.Name,
                _context.ObjId,
                null, // task id is unknown at this moment
                _context.UserId);

            DBEventServiceSingleton.Instance.RaiseDataChangedEvent(workflowInstaceId,
                NewteraWorkflowRuntime.Instance.GetWorkflowRunTime(),
                eventArgs);
        }

        /// <summary>
        /// Find the subscribers that subscribe to the event, run actions of the found subscribers.
        /// </summary>
        /// <param name="eventDef">The event to be raised</param>
        /// <param name="className">The name of a class where the subscribers are defined</param>
        private void RaiseEventToSubscribers(EventDef eventDef, string className)
        {
            ClassElement classElement = _context.MetaData.SchemaModel.FindClass(className);
            SubscriberCollection subscribers = _context.MetaData.SubscriberManager.GetClassSubscribers(classElement);
            NewteraSubscriberService subscriberService = new NewteraSubscriberService();
            IInstanceWrapper instanceWrapper = null;

            foreach (Subscriber subscriber in subscribers)
            {
                try
                {
                    if (subscriber.EventName == eventDef.Name)
                    {
                        if (instanceWrapper == null)
                        {
                            XmlDocument doc = _context.XmlInstance.OwnerDocument;

                            DataViewModel instanceDataView = _context.MetaData.GetDetailedDataView(className);
                            InstanceView instanceView;
                            if (doc != null)
                            {
                                XmlReader reader = new XmlNodeReader(doc);
                                DataSet ds = new DataSet();
                                ds.ReadXml(reader);

                                instanceView = new InstanceView(instanceDataView, ds);

                                if (instanceView.InstanceData != null && string.IsNullOrWhiteSpace(instanceView.InstanceData.ObjId))
                                {
                                    // The data instance has been deleted when processing a delete event, just pass the objId of the deleted instance
                                    instanceView.InstanceData.DataSet = null;
                                    instanceView.InstanceData.ObjId = _context.ObjId;
                                }
                            }
                            else
                            {
                                instanceView = new InstanceView(instanceDataView);
                            }

                            instanceWrapper = new InstanceWrapper(instanceView);
                        }

                        subscriberService.PerformActions(classElement, subscriber, eventDef, instanceWrapper);
                    }
                }
                catch (Exception ex)
                {
                    // error, do not stop
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private bool EvaluateCondition(EventDef eventDef)
        {
            bool status = false;
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            XmlElement originalInstance = principal.CurrentInstance;
            string finalCondition = null;
            try
            {
                if (principal != null)
                {
                    // only check the first element
                    XmlElement currentInstance = _context.XmlInstance;

                    if (currentInstance != null && eventDef.AfterConditionInXQuery != null)
                    {
                        principal.CurrentInstance = currentInstance;

                        // build a complete xquery
                        finalCondition = TEMPLATE_XQUERY.Replace("$$", eventDef.AfterConditionInXQuery);

                        IConditionRunner conditionRunner = PermissionChecker.Instance.ConditionRunner;

                        status = conditionRunner.IsConditionMet(finalCondition);
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                if (finalCondition != null)
                {
                    ErrorLog.Instance.WriteLine("Got an exception with query " + finalCondition + " error is " + ex.Message + "\n" + ex.StackTrace);
                }

                throw ex;
            }
            finally
            {
                // unset the current instance as a context for condition evaluation
                principal.CurrentInstance = originalInstance;
            }
        }

        /// <summary>
        /// Gets the information indicating whether update event is caused by the updates
        /// on the attributes specified in the EventDef. If the EventDef does not specifies
        /// any attributes, return true by default.
        /// </summary>
        /// <param name="eventDef"></param>
        /// <returns></returns>
        private bool IsAttributeValuesUpdated(EventDef eventDef)
        {
            bool status = true;

            if (eventDef.AttributesUpdated != null &&
                eventDef.AttributesUpdated.Count > 0)
            {
                foreach (string attributeName in eventDef.AttributesUpdated)
                {
                    if (!_context.ContainsUpdatedAttribute(attributeName))
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }
	}
}