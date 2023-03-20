/*
* @(#)CountField.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	
	/// <summary>
	/// A CountField is dedicted to Count(*) in a SELECT clause
	/// </summary>
	/// <version>  	1.0.1 03 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class CountField:SQLElement
	{
		
		// private instance members
		private string _function;
		
		/// <summary>
		/// Initiating a CountField object.
		/// </summary>
		/// <param name="function">the name of count function</param>
		public CountField(string function) : base()
		{
			_function = function;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			return _function + "(*)";
		}
	}
}