/*
* @(#)NewteraLicenseException.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Licensing
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The NewteraLicenseException class is the exception type for Newtera License
	/// related exceptions
	/// </summary>
	/// <version>1.0.0 23 Sep 2005 </version>
	/// <author> Yong Zhang</author>
    public class NewteraLicenseException : NewteraException
	{
		/// <summary>
		/// Initializing a NewteraLicenseException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public NewteraLicenseException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a NewteraLicenseException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public NewteraLicenseException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}