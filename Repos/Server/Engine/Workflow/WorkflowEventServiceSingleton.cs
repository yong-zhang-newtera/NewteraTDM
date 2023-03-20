/*
* @(#) WorkflowEventServiceSingleton.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;

    using Newtera.WorkflowServices;

	/// <summary>
	/// Represent the single NewteraWorkflowEventService instance in the server.
	/// </summary>
	/// <version> 	1.0.0 21 Mar 2007 </version>
	public class WorkflowEventServiceSingleton
	{		
		private static NewteraWorkflowService theEventService;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private WorkflowEventServiceSingleton()
		{
		}

		/// <summary>
        /// Gets the NewteraWorkflowService instance.
		/// </summary>
        /// <returns> The NewteraWorkflowService instance.</returns>
        static public IWorkflowService Instance
		{
			get
			{
                return theEventService;
			}
		}

		static WorkflowEventServiceSingleton()
		{
			// Initializing the instance.
			{
                theEventService = new NewteraWorkflowService();
			}
		}
	}
}