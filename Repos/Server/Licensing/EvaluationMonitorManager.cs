/*
* @(#) EvaluationMonitorManager.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Licensing
{
	using System;
	using System.Data;
	using System.Threading;

    using Infralution.Licensing;

	/// <summary>
	/// Use one copy of the Evaluation Monitor for better performance
	/// </summary>
	/// <version>1.0.1 22 Dec 2007 </version>
	public class EvaluationMonitorManager
	{
        internal const int DefaultSessionNumber = 1;

        internal const int DefaultAdvancedSessionNumber = 1;

		// Static factory object, all invokers will use this factory object.
		private static EvaluationMonitorManager theManager;

        internal static string PRODUCT_ID = "Newtera";

        private Infralution.Licensing.EvaluationMonitor _monitor;

        private int _availableSessions;

        private int _maxSessionNumber;

        private int _availableAdvancedSessions;

        private int _maxAdvancedSessionNumber;

        private bool _initialized;

        private bool _advancedInitialized;

		/// <summary>
		/// Instantiate an instance of EvaluationMonitorManager class.
		/// </summary>
		public EvaluationMonitorManager()
		{
            _monitor = new Infralution.Licensing.EvaluationMonitor(EvaluationMonitorManager.PRODUCT_ID);
            _availableSessions = DefaultSessionNumber;
            _maxSessionNumber = DefaultSessionNumber;
            _availableAdvancedSessions = DefaultAdvancedSessionNumber;
            _maxAdvancedSessionNumber = DefaultAdvancedSessionNumber;
            _initialized = false;
            _advancedInitialized = false;
		}

        /// <summary>
        /// Gets the evaluation monitor
        /// </summary>
        public Infralution.Licensing.EvaluationMonitor Monitor
        {
            get
            {
                return _monitor;
            }
        }

        /// <summary>
        /// Gets or sets maximum number of sessions available for use
        /// </summary>
        public int MaxSessionNumber
        {
            get
            {
                return _maxSessionNumber;
            }
            set
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        // initialize 
                        _maxSessionNumber = value;
                        _availableSessions = value;
                        _initialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum number of advanced sessions available for use
        /// </summary>
        public int MaxAdvancedSessionNumber
        {
            get
            {
                return _maxAdvancedSessionNumber;
            }
            set
            {
                lock (this)
                {
                    if (!_advancedInitialized)
                    {
                        // initialize 
                        _maxAdvancedSessionNumber = value;
                        _availableAdvancedSessions = value;
                        _advancedInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of sessions available for use
        /// </summary>
        public int AvailableSessionNumber
        {
            get
            {
                lock (this)
                {
                    return _availableSessions;
                }
            }
        }

        /// <summary>
        /// Gets the number of advanced sessions available for use
        /// </summary>
        public int AvailableAdvancedSessionNumber
        {
            get
            {
                lock (this)
                {
                    return _availableAdvancedSessions;
                }
            }
        }

        /// <summary>
        /// Decrement available session number by one
        /// </summary>
        public void DecrementSessionNumber()
        {
            lock (this)
            {
                if (_availableSessions > 0)
                {
                    _availableSessions--;
                }
                else
                {
                    throw new InvalidOperationException("No sessions available");
                }
            }
        }

        /// <summary>
        /// Decrement available advanced session number by one
        /// </summary>
        public void DecrementAdvancedSessionNumber()
        {
            lock (this)
            {
                if (_availableAdvancedSessions > 0)
                {
                    _availableAdvancedSessions--;
                }
                else
                {
                    throw new Exception("No advanced sessions available");
                }
            }
        }

        /// <summary>
        /// Increment available session number by one
        /// </summary>
        public void IncrementSessionNumber()
        {
            lock (this)
            {
                if (_availableSessions < _maxSessionNumber)
                {
                    _availableSessions++;
                }
            }
        }

        /// <summary>
        /// Increment available advanced session number by one
        /// </summary>
        public void IncrementAdvancedSessionNumber()
        {
            lock (this)
            {
                if (_availableAdvancedSessions < _maxAdvancedSessionNumber)
                {
                    _availableAdvancedSessions++;
                }
            }
        }

        /// <summary>
        /// Re-create the monitor
        /// </summary>
        public void Reset()
        {
            lock (this)
            {
                _monitor = new Infralution.Licensing.EvaluationMonitor(EvaluationMonitorManager.PRODUCT_ID);
                _initialized = false;
                _advancedInitialized = false;
            }
        }

		/// <summary>
		/// Gets the EvaluationMonitorManager instance.
		/// </summary>
		/// <returns> The EvaluationMonitorManager instance.</returns>
		static public EvaluationMonitorManager Instance
		{
			get
			{
                return theManager;
            }
		}

		/// <summary>
		/// Initializing the singleton instance
		/// </summary>
		static EvaluationMonitorManager()
		{
			// Initializing the instance
			{
				theManager = new EvaluationMonitorManager();
			}
		}
	}
}
