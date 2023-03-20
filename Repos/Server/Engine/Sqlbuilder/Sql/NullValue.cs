/*
* @(#)NullValue.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	
	/// <summary>
	/// A NullValue object represents a null value.
	/// </summary>
	/// <version>  	1.0.1 19 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class NullValue : SQLElement
	{	
		/// <summary>
		/// Initiating a NullValue object.
		/// </summary>
		public NullValue()
		{
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			return "NULL";
		}
	}
}