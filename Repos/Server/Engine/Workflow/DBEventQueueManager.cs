/*
* @(#) DBEventQueueManager.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.IO;
    using System.Xml;
    using System.Data;
	using System.Collections;
    using System.Collections.Generic;
	using System.Threading;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.DB;

	/// <summary>
	/// This is the single object that manages a queue for database events and schedules
    /// raising events to the workflow in a sequential manner.
	/// </summary>
	/// <version> 1.0.0 10 April 2015 </version>
	public class DBEventQueueManager
	{		
		// Static cache object, all invokers will use this cache object.
		private static DBEventQueueManager theManager;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private DBEventQueueManager()
		{
		}

		/// <summary>
		/// Gets the DBEventQueueManager instance.
		/// </summary>
		/// <returns> The DBEventQueueManager instance.</returns>
		static public DBEventQueueManager Instance
		{
			get
			{
				return theManager;
			}
		}

        public void RunSavedEvents()
        {
            List<EventContext> eventContexts = null;

            // get the list from the database
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            eventContexts = adapter.GetDBEventConexts();
            ErrorLog.Instance.WriteLine("DBEventQueueManager started with " + eventContexts.Count + " eventContext objects from DB.");

            // Remove the events from the DB
            foreach (EventContext eContext in eventContexts)
            {
                adapter.DeleteDBEventContext(eContext.EventContextId);
            }

            // Run the events
            foreach (EventContext eContext in eventContexts)
            {
                PostEvent(eContext);
            }
        }

        /// <summary>
        /// Post an event in the queue to the workflow
        /// </summary>
        public void PostEvent(EventContext eventContext)
        {
            eventContext.XmlInstance = GetXmlInstance(eventContext); // get the xml instance from the database that may raise the event

            if (eventContext.XmlInstance != null)
            {
                EventRaiser eventRaiser = new EventRaiser(eventContext);

                try
                {
                    // run the event raiser asynchronousely using the worker thread from a threadpool;
                    eventRaiser.Run();
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine("Failed in running an event due to " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private XmlElement GetXmlInstance(EventContext eventContext)
        {
            XmlElement xmlInstance = null;

            try
            {
                DataViewModel instanceDataView = eventContext.MetaData.GetDetailedDataView(eventContext.ClassElement.Name);
                string query = instanceDataView.GetInstanceQuery(eventContext.ObjId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);

                if (doc != null && 
                    doc.DocumentElement != null)
                {
                    if (doc.DocumentElement.ChildNodes.Count > 0)
                        xmlInstance = doc.DocumentElement.ChildNodes[0] as XmlElement;
                    else
                        // the instance has been deleted, make an empty instance
                        xmlInstance = doc.CreateElement(eventContext.ClassElement.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            return xmlInstance;
        }

		static DBEventQueueManager()
		{
			// Initializing the manager.
			{
				theManager = new DBEventQueueManager();

                // run the events saved in DB due to previouse shut down of the server
                theManager.RunSavedEvents();
			}
		}
	}
}