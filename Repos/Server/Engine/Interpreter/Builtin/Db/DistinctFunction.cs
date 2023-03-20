/*
* @(#)DistinctFunction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	
	/// <summary>
	/// DB implementation of finding the distinct values of the given collection values.
	/// </summary>
	/// <version>  1.0 19 June 2008 </version>
	public class DistinctFunction : AggregateFunction
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public DistinctFunction()
		{
		}

		/// <summary>
		/// Get the function type, defined in SQLElement.
		/// </summary>
		/// <value> one of SQLFunction enum values</value>
		public override SQLFunction Type
		{
			get
			{
				return SQLFunction.Distinct;
			}
		}
	}
}