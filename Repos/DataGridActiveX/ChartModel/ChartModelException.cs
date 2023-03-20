/*
* @(#)ChartModelException.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;

	/// <summary> 
	/// The ChartModelException class is the default exception type for ChartModel
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 24 Apr 2006</version>
	public class ChartModelException : Exception
	{
		/// <summary>
		/// Constructor of a ChartModelException without an object
		/// </summary>
		/// <param name="reason">a description of the exception </param>
		public ChartModelException(string reason):base(reason)
		{
		}
	}
}