/*
* @(#) DBEventServiceSingleton.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;

    using Newtera.WorkflowServices;

	/// <summary>
	/// Represent the single DBEventService instance in the server.
	/// </summary>
	/// <version> 	1.0.0 26 Dec 2006 </version>
	public class DBEventServiceSingleton
	{		
		private static DBEventService theEventService;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private DBEventServiceSingleton()
		{
		}

		/// <summary>
        /// Gets the DBEventService instance.
		/// </summary>
        /// <returns> The DBEventService instance.</returns>
        static public IDBEventService Instance
		{
			get
			{
                return theEventService;
			}
		}

		static DBEventServiceSingleton()
		{
			// Initializing the instance.
			{
                theEventService = new DBEventService();
			}
		}
	}
}