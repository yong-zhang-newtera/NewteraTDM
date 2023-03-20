/*
* @(#)IDBFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// An interface for the builtin functions that need to be executed by the corresponding
	/// database functions, such  avg, min, max, count, sum, contains, and like.
	/// </summary>
	/// <version>  1.0 2003-9-22 </version>
	/// <author>    Yong Zhang </author>
	public interface IDBFunction
	{
		/// <summary>
		/// Sets the tree manager for retrieving an entity using a path
		/// </summary>
		/// <value>the tree manager.</value>
		TreeManager TreeManager
		{
			set;
		}

		/// <summary>
		/// Set the data provider.
		/// </summary>
		/// <value>the data provider</value>
		IDataProvider DataProvider
		{
			set;
		}

		/// <summary>
		/// Get an SQL equivalent element created for the function
		/// </summary>
		/// <value> an SQLElement.</value>
		SQLElement SQLElement
		{
			get;
		}

		/// <summary>
		/// Get the DBEntity for the function.
		/// </summary>
		/// <value> a DBEntity</value>
		DBEntity FunctionEntity
		{
			get;
		}

		/// <summary>
		/// Get the function type, defined in SQLElement.
		/// </summary>
		/// <value> one of SQLFunction enum values.</value>
		SQLFunction Type
		{
			get;
		}
	}
}