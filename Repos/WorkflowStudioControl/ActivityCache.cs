/*
* @(#)ActivityCache.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowStudioControl
{
	using System;
	using System.Collections;
    using System.Workflow.ComponentModel;

	/// <summary>
	/// A singleton class that serves as a global place to keep the activity being edited
	/// </summary>
	/// <version>  	1.0.0 19 Nov 2007 </version>
	public class ActivityCache
	{
		private Activity _currentActivity;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ActivityCache theCache;
		
		static ActivityCache()
		{
			theCache = new ActivityCache();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ActivityCache()
		{
			_currentActivity = null;
		}

		/// <summary>
		/// Gets the ActivityCache instance.
		/// </summary>
		/// <returns> The ActivityCache instance.</returns>
		static public ActivityCache Instance
		{
			get
			{
				return theCache;
			}
		}
		
		/// <summary>
		/// Gets or sets the current activity being edited
		/// </summary>
		/// <value>The Activit object.</value>
        public Activity CurrentActivity
        {
            get
            {
                return _currentActivity;
            }
            set
            {
                _currentActivity = value;
            }
        }
	}
}