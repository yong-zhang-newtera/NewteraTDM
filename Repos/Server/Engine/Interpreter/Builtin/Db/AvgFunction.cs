/*
* @(#)AvgFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	
	/// <summary>
	/// DB implementation of caculate the average of the given collection values.
	/// </summary>
	/// <version>  1.0 2003-9-10 </version>
	/// <author>  Yong Zhang </author>
	public class AvgFunction : AggregateFunction
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public AvgFunction()
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
				return SQLFunction.Avg;
			}
		}
	}
}