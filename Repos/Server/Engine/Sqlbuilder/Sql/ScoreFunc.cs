/*
* @(#)ScoreFunc.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	
	/// <summary>
	/// A ScoreFunc class is responsible for outputting the SQL text used for expressing
	/// a score of full-text search result.
	/// </summary>
	/// <version>  	1.0.0 21 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class ScoreFunc : SQLElement
	{
		
		// private declarations
		private string _funcName;
		
		/// <summary>
		/// Initiating a Score function object
		/// </summary>
		/// <param name="funcName">the score function name.</param>
		public ScoreFunc(string funcName) : base()
		{
			_funcName = funcName;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			return _funcName;
		}
	}
}