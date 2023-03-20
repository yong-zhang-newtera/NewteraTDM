/*
* @(#)WorkflowServerException.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using Newtera.Common.Core;
	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// The WorkflowServerException class
	/// </summary>
	/// <version>  	1.0.0 19 May 2007
	/// </version>
    public class WorkflowServerException : NewteraException
	{
		/// <summary>
		/// Initiating a WorkflowServerException object
		/// </summary>
		/// <param name="reason">
		/// a description of the error
		/// </param>
		public WorkflowServerException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating a WorkflowServerException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		/// <param name="ex">the root exception</param>
		public WorkflowServerException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}