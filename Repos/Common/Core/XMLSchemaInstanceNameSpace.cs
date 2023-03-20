/*
* @(#)XMLSchemaInstanceNameSpace.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;

	/// <summary>
	/// The XMLSchemaInstanceNameSpace class is a centralized place where keywords belong 
	/// to XML Schema instance name space are defined as constants. Application will refer 
	/// to the constants rather than directly to the keywords. Therefore, if any 
	/// changes of keywords in XML Schema name space won't affect the application 
	/// code.
	/// </summary>
	/// <version>  	1.0.0 29 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	public class XMLSchemaInstanceNameSpace
	{	
		/// <summary>
		/// XMLSchemaInstanceNameSpace.TYPE
		/// </summary>
		public const string TYPE = "type";
		/// <summary>
		/// XMLSchemaInstanceNameSpace.NIL
		/// </summary>
		public const string NIL = "nil";
		/// <summary>
		/// XMLSchemaInstanceNameSpace.PREFIX
		/// </summary>
		public const string PREFIX = "xsi";
		/// <summary>
		/// XMLSchemaInstanceNameSpace.URI
		/// </summary>
		public const string URI = "http://www.w3.org/2003/XMLSchema-instance";
	}
}