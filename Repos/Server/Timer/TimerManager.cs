/*
* @(#) TimerManager.cs
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
    using System.Xml;
    using System.Data;
    using System.Timers;
    using System.Security.Principal;
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Events;
    using Newtera.Server.Engine.Cache;

	/// <summary>
	/// This is the single object that manages a queue for timer events and the queue
    /// uses a single worker thread to process all timer events..
	/// </summary>
	/// <version> 1.0.0 18 Sept 2013 </version>
	public class TimerManager
	{
		// Static cache object, all invokers will use this cache object.
		private static TimerManager theManager;

        private System.Timers.Timer _theTimer;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private TimerManager()
		{
            // start the timer
            _theTimer = new System.Timers.Timer(3600000); // one hour interval         3600000
            _theTimer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _theTimer.Interval = 3600000;
            _theTimer.Enabled = true;
            _theTimer.AutoReset = true; // repeat timer
		}

		/// <summary>
		/// Gets the TimerManager instance.
		/// </summary>
		/// <returns> The TimerManager instance.</returns>
		static public TimerManager Instance
		{
			get
			{
				return theManager;
			}
		}

        public void Start()
        {
            _theTimer.Start();
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                //ErrorLog.Instance.WriteLine("OnTimer event: " + DateTime.Now.ToString("s"));

                EventCollection timerEvents = MetaDataCache.Instance.GetAllTimerEvents(); // get a list of timer events (cloned)

                int count = 0;
                foreach (EventDef eventDef in timerEvents)
                {
                    if (IsElapsedEvent(eventDef))
                    {
                        // add a clone of the event to the queue, and worker thread will pick it up and process it
                        TimerEventQueueManager.Instance.AddEvent(eventDef.Clone());

                        // set the checked time of the event
                        MetaDataCache.Instance.SetEventLastCheckedTime(eventDef, DateTime.Now);

                        count++;
                    }
                }

                // posts an event in the queue 
                if (count > 0)
                {
                    TimerEventQueueManager.Instance.PostNextEvent();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get the information indicating whether interval of the given event has elapsed
        /// </summary>
        /// <param name="eventDef">The given event</param>
        /// <returns>true if it is elapsed, false otherwise.</returns>
        private bool IsElapsedEvent(EventDef eventDef)
        {
            bool status = false;

            TimeSpan timeSpan = DateTime.Now.Subtract(eventDef.LastCheckedTime);

            switch (eventDef.TimerInterval)
            {
                case TimerInterval.EveryHour:
                    // timer time out every hour, so we don't need to check here
                    //if (timeSpan.TotalHours >= 1)
                    {
                        status = true;
                    }

                    break;

                case TimerInterval.EveryDay:

                    if (timeSpan.TotalDays >= 1)
                    {
                        status = true;
                    }

                    break;

                case TimerInterval.EveryWeek:

                    if (timeSpan.TotalDays >= 7)
                    {
                        status = true;
                    }

                    break;

                case TimerInterval.EveryMonth:

                    int monthDifference = MonthDifference(DateTime.Now, eventDef.LastCheckedTime);

                    if (monthDifference >= 1)
                    {
                        status = true;
                    }

                    break;

                case TimerInterval.EveryYear:

                    if (timeSpan.TotalDays >= 365)
                    {
                        status = true;
                    }

                    break;
            }

            return status;
        }

        private int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

		static TimerManager()
		{
			// Initializing the manager.
			{
				theManager = new TimerManager();
			}
		}
	}
}