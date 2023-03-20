/*
* @(#)ProjectUpdateException.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;

	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for error during the update of project model.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006</version>
    public class ProjectModelUpdateException : NewteraException
	{
		/// <summary>
		/// Initiating an instance of ProjectModelUpdateException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public ProjectModelUpdateException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating an instance of ProjectUpdateException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">An inner exception</param>
        public ProjectModelUpdateException(string reason, Exception ex)
            : base(reason, ex)
		{
		}
	}
}