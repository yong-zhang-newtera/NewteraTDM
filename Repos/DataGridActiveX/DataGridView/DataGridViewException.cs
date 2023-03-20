/*
* @(#)DataGridViewException.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;

	/// <summary>
	/// The DataGridViewException class is the default exception type for ClassView
	/// namespace.
	/// </summary>
	/// <version>  	1.0.0 28 May 2006</version>
	public class DataGridViewException : Exception
	{
		/// <summary>
		/// Initializing a DataGridViewException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public DataGridViewException(string reason):base(reason)
		{
		}
	}
}