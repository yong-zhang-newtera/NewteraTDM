/*
* @(#)Variable.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A Variable object represents a variable in a VALUE clause of Insert and Update statement.
	/// </summary>
	/// <version>  	1.0.0 04 Aug 2008 </version>
	public class Variable : SQLElement
	{
		// private instance members
		private string _name;
		
		/// <summary>
		/// Initiating a Variable object
		/// </summary>
		/// <param name="name">the variable name.</param>
		public Variable(string name) : base()
		{
            _name = name;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{			
			return "{" + _name + "}";
		}
	}
}