/*
* @(#)TimerEventRaiser.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Timer
{
	using System;
	using System.Xml;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Security.Principal;
	using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net.Mail;
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.Wrapper;
    using Newtera.Server.DB;
    using Newtera.Server.Engine.Cache;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Engine.Vdom;
    using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Workflow;
    using Newtera.WorkflowServices;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;
    using Newtera.WFModel;

    /// <summary> 
    /// Responsible for evaluating the data instances against the condition indicated by the event definition and raise the event to the subscribers if the
    /// condition are met.
    /// </summary>
    /// <version>  	1.0.0 19 Sep 2013</version>
    public class TimerEventRaiser
	{
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";

        private EventDef _eventDef;
 
		/// <summary>
        /// Initiating an TimerEventRaiser instance
		/// </summary>
        /// <param name="_eventDef">The event _eventDef</param>
        public TimerEventRaiser(EventDef eventDef)
		{
            _eventDef = eventDef;
		}

        /// <summary>
        /// run the event raiser
        /// </summary>
        public void Run(Object threadContext)
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            XmlReader xmlReader;
            XmlDocument doc;
            DataSet ds;
            DataTable dt;
            QueryReader reader = null;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                DataViewModel dataView = _eventDef.MetaData.GetDetailedDataView(_eventDef.ClassName);

                // Get the search query that retrieve the data instances from the class indicated by the event
                string query = GetSearchQuery(dataView, _eventDef);

                Interpreter interpreter = new Interpreter();

                // get result in pages
                interpreter.IsPaging = true;
                interpreter.PageSize = 100;
                interpreter.OmitArrayData = true;
                reader = interpreter.GetQueryReader(query);

                InstanceView instanceView;
                VDocument vdoc;
                IInstanceWrapper instanceWrapper;

                doc = reader.GetNextPage(); // get the first page
                while (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                {
                    // convert xml data into instance view
                    xmlReader = new XmlNodeReader(doc);
                    ds = new DataSet();
                    ds.ReadXml(xmlReader);
                    instanceView = new InstanceView(dataView, ds);

                    // get a document that only contains base class data and convert into xml doc
                    string xml;
                    dt = ds.Tables[dataView.BaseClass.ClassName];
                    using (StringWriter sw = new StringWriter())
                    {
                        dt.WriteXml(sw);
                        xml = sw.ToString();
                    }
                    vdoc = DocumentFactory.Instance.Create();
                    vdoc.LoadXml(xml);

                    // walk through the base class instances to check which ones meet the timer condition indicated by the event
                    int index = 0;
                    foreach (XmlElement instanceElement in vdoc.DocumentElement.ChildNodes)
                    {
                        instanceView.SelectedIndex = index++; // sync the row

                        // check if the instance meets the condition indicated by the event
                        if (IsConditionMet(instanceElement, _eventDef))
                        {
                            // trace the event that took place
                            if (TraceLog.Instance.Enabled)
                            {
                                string[] messages = { _eventDef.Name + " event is raised.",
                                    "Event Class: " + _eventDef.ClassName,
                                    "Event Operation: " + _eventDef.OperationType.ToString(),
                                    "Event Condition: " + _eventDef.TimerCondition};
                                TraceLog.Instance.WriteLines(messages);
                            }

                            // first check if event is used to start a workflow. if yes, the data instance
                            // is bound to the started workflow instance
                            if (RaiseEventToStartWorkflow(_eventDef, instanceView.InstanceData.ObjId))
                            {
                                if (TraceLog.Instance.Enabled)
                                {
                                    string[] messages = { "Event " + _eventDef.Name + " is used to start a workflow instance." };
                                    TraceLog.Instance.WriteLines(messages);
                                }
                            }
                            else
                            {
                                // no workflow has been started by the event, then try to send
                                // the event to the running workflows in case there is a workflow
                                // waiting for the event
                                RaiseEventToWorkflowActivity(_eventDef, instanceView);
                            }

                            // then check if event is subscribed.
                            instanceWrapper = GetInstanceWrapper(instanceView);
                            RaiseEventToSubscribers(_eventDef, instanceWrapper);
                        }
                    }

                    doc = reader.GetNextPage();
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

                // close the database connection
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether an instance meets the condition indicated by an event.
        /// </summary>
        /// <param name="instanceView">An instance view</param>
        /// <param name="eventDef">The event definition.</param>
        /// <returns>true if the condition is met, false otherwise.</returns>
        private bool IsConditionMet(XmlElement currentInstance, EventDef eventDef)
        {
            bool status = false;
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            XmlElement originalInstance = principal.CurrentInstance;
            try
            {
                if (principal != null)
                {
                    if (currentInstance != null)
                    {
                        principal.CurrentInstance = currentInstance;

                        // build a complete xquery
                        string finalCondition = TEMPLATE_XQUERY.Replace("$$", eventDef.TimerCondition);

                        Interpreter interpreter = new Interpreter();

                        // execute the xquery using interpreter
                        XmlDocument doc = interpreter.Query(finalCondition);

                        string result = doc.DocumentElement["flag"].InnerText;
                        if (result == "1")
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                        }
                    }
                }

                return status;
            }
            finally
            {
                // unset the current instance as a context for condition evaluation
                principal.CurrentInstance = originalInstance;
            }
        }

        /// <summary>
        /// Raise the event to start a workflow whose start event matches the given event
        /// </summary>
        /// <param name="eventDef">The event</param>
        /// <param name="objId">The object id</param>
        /// <returns>true if there is a workflow started by the event, false, otherwise.</returns>
        private bool RaiseEventToStartWorkflow(EventDef eventDef, string objId)
        {
            NewteraEventArgs eventArgs = new NewteraEventArgs(eventDef.MetaData.SchemaInfo.NameAndVersion,
                    eventDef.ClassName,
                    eventDef.Name,
                    objId);

            return NewteraWorkflowRuntime.Instance.StartWorkflowEventHandler(this, eventArgs);
        }

        /// <summary>
        /// Raise the event to the workflow activity through NewteraEventService
        /// </summary>
        /// <param name="eventDef">The event</param>
        private void RaiseEventToWorkflowActivity(EventDef eventDef, InstanceView instanceView)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(instanceView.InstanceData.ObjId);
            Guid workflowInstaceId = Guid.Empty;
            if (bindingInfo != null)
            {
                // there exists a binding between the data instance and a workflow instance
                // raise the event to the workflow instance
                workflowInstaceId = new Guid(bindingInfo.WorkflowInstanceId);

                NewteraEventArgs eventArgs = new NewteraEventArgs(instanceView.DataView.SchemaInfo.NameAndVersion,
                     instanceView.InstanceData.OwnerClassName,
                     eventDef.Name,
                     instanceView.InstanceData.ObjId,
                     null, // task id is unknown at this moment
                     null);

                DBEventServiceSingleton.Instance.RaiseDataChangedEvent(workflowInstaceId,
                    NewteraWorkflowRuntime.Instance.GetWorkflowRunTime(),
                    eventArgs);
            }
        }

        /// <summary>
        /// Get a query that retrieve the data instance from a class that meets a condition specified by the event
        /// </summary>
        /// <returns>The search query</returns>
        private string GetSearchQuery(DataViewModel dataView, EventDef eventDef)
        {
            // add condition expression
            if (eventDef.AfterCondition != null)
            {
                dataView.AddSearchExpr(eventDef.AfterCondition, Newtera.Common.MetaData.DataView.ElementType.And);
            }

            return dataView.SearchQuery;
        }

        /// <summary>
        /// Create an instance wrapper that represents an instance with detailed data view model
        /// </summary>
        /// <param name="instanceView"></param>
        /// <returns></returns>
        private IInstanceWrapper GetInstanceWrapper(InstanceView instanceView)
        {
            DataViewModel instanceDataView = _eventDef.MetaData.GetDetailedDataView(_eventDef.ClassName);
            string query = instanceDataView.GetInstanceQuery(instanceView.InstanceData.ObjId);

            Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
            XmlDocument doc = interpreter.Query(query);
            XmlReader reader = new XmlNodeReader(doc);
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            return new InstanceWrapper(new InstanceView(instanceDataView, ds));
        }

        /// <summary>
        /// Find the subscribers that subscribe to the event, run actions of the found subscribers.
        /// </summary>
        /// <param name="eventDef">The event to be raised</param>
        /// <param name="instanceView">The data instance that cause the event</param>
        private void RaiseEventToSubscribers(EventDef eventDef, IInstanceWrapper instanceWrapper)
        {
            ClassElement classElement = eventDef.MetaData.SchemaModel.FindClass(eventDef.ClassName);
            SubscriberCollection subscribers = eventDef.MetaData.SubscriberManager.GetClassSubscribers(classElement);
            NewteraSubscriberService subscriberService = new NewteraSubscriberService();

            foreach (Subscriber subscriber in subscribers)
            {
                try
                {
                    if (subscriber.EventName == eventDef.Name)
                    {
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

	}
}