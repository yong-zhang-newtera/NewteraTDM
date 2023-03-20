/*
* @(#)MissingAttributesException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// The exception is thrown for missing attributes in a class entity.
	/// </summary>
	/// <version>  	1.0.0 13 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class MissingAttributesException : SQLBuilderException
	{
		
		/// <summary>
		/// Initiating an instance of MissingAttributesException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public MissingAttributesException(string reason) : base(reason)
		{
		}
	}
}