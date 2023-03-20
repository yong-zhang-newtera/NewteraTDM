/*
* @(#)AttachmentException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Attachment
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The AttachmentException class is the base exception type for Data.Attachment 
	/// package. It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 09 Jan 2004 </version>
	/// <author> Yong Zhang</author>
    public class AttachmentException : NewteraException
	{
		/// <summary>
		/// Initiating a AttachmentException object
		/// </summary>
		/// <param name="reason">
		/// a description of the error
		/// </param>
		public AttachmentException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating a AttachmentException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		/// <param name="ex">the root exception</param>
		public AttachmentException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}