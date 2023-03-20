/*
* @(#)DataViewException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The DataViewException class is the default exception type for DataView
	/// namespace.
	/// </summary>
	/// <version>  	1.0.0 28 Oct 2003</version>
	/// <author>  Yong Zhang</author>
    public class DataViewException : NewteraException
	{
		/// <summary>
		/// Initializing a DataViewException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public DataViewException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a DataViewException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		public DataViewException(string reason, Exception ex):base(reason, ex)
		{
		}
	}
}