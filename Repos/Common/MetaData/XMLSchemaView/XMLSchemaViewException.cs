/*
* @(#)XMLSchemaViewException.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The XMLSchemaViewException class is the default exception type for XMLSchemaView
	/// namespace.
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
    public class XMLSchemaViewException : NewteraException
	{
		/// <summary>
		/// Initializing a XMLSchemaViewException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public XMLSchemaViewException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a XMLSchemaViewException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		public XMLSchemaViewException(string reason, Exception ex):base(reason, ex)
		{
		}
	}
}