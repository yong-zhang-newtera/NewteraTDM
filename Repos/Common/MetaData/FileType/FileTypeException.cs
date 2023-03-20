/*
* @(#)FileTypeException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	using System;
	using Newtera.Common.Core;

	/// <summary> 
	/// The FileTypeException class is the default exception type for FileType
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 14 Jan 2004</version>
	/// <author> Yong Zhang </author>
    public class FileTypeException : NewteraException
	{
		/// <summary> 
		/// Constructor of a FileTypeException without an object
		/// </summary>
		/// <param name="reason">a description of the exception </param>
		public FileTypeException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Use this constructor when you wish to wrap an Exception.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">The exception to translate; is stored as
		/// next exception in chain.  Since Throwables have no
		/// chain, ex will be the last exception in the chain.
		/// </param>
		public FileTypeException(string reason, System.Exception ex) : base(reason, ex)
		{
		}
	}
}