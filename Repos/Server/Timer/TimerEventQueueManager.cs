/*
* @(#) TimerEventQueueManager.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Timer
{
	using System;
	using System.IO;
	using System.Collections;
    using System.Collections.Generic;
	using System.Threading;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Events;

	/// <summary>
	/// This is the single object that manages a queue for timer events and schedules
    /// raising events to the event subscribers in a sequential manner.
	/// </summary>
	/// <version> 1.0.0 19 Sep 2013 </version>
	public class TimerEventQueueManager
	{		
		// Static cache object, all invokers will use this cache object.
		private static TimerEventQueueManager theManager;

        private List<EventDef> _events; // event queue

		/// <summary>
		/// Private constructor.
		/// </summary>
		private TimerEventQueueManager()
		{
            _events = new List<EventDef>();
		}

		/// <summary>
		/// Gets the TimerEventQueueManager instance.
		/// </summary>
		/// <returns> The TimerEventQueueManager instance.</returns>
		static public TimerEventQueueManager Instance
		{
			get
			{
				return theManager;
			}
		}

        /// <summary>
        /// Post the next event in the queue to the subscribers
        /// </summary>
        public void PostNextEvent()
        {
            // lock the while processing the events
            lock (this)
            {
                bool status = true;
                // process the events in thr queue
                while (status)
                {
                    status = ProcessEvent();
                }
            }
        }

		/// <summary>
		/// Add an event to the queue
		/// </summary>
		/// <param name="eventDef">The event context</param>
        public void AddEvent(EventDef eventDef)
		{
			lock (this)
			{
                _events.Add(eventDef);
			}
		}

        /// <summary>
        /// Process an event at beginning of the queue using a worker thread from the thread pool
        /// </summary>
        /// <returns>true if an event is posted, false if there is no more events to be posted</returns>
        private bool ProcessEvent()
        {
            bool status = false;

            try
            {
                if (_events.Count > 0)
                {
                    EventDef eventDef = _events[0];

                    _events.RemoveAt(0);

                    TimerEventRaiser eventRaiser = new TimerEventRaiser(eventDef);

     
                    // run the event raiser in a worker thread from the thread pool
                    // when the eventRaiser completes the event processing, it will notify the
                    // the event queue manager
                    ThreadPool.QueueUserWorkItem(eventRaiser.Run);

                    status = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            return status;
        }

		static TimerEventQueueManager()
		{
			// Initializing the manager.
			{
				theManager = new TimerEventQueueManager();
			}
		}
	}
}