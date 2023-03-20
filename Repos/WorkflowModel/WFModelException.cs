/*
* @(#)WFModelException.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// The WFModelException class is the default exception type for WFModel namespace.
	/// </summary>
	/// <version>  	1.0.0 8 Dec 2006</version>
	public class WFModelException : Exception
	{
		/// <summary>
		/// Initializing a WFModelException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public WFModelException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a WFModelException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		public WFModelException(string reason, Exception ex):base(reason, ex)
		{
		}
	}
}