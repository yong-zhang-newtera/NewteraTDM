/*
* @(#) VariableQuery.cs    1.0.0        2001-11-19
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
	using System.Text.RegularExpressions;

	/// <summary>
	/// The class represents a query that may contain variables. It provides convinient
	/// methods to deal with these variables, such as replacing variables with values.
	/// </summary>
	/// <version>     1.0.0    08 Aug 2003</version>
	/// <author>     Yong Zhang </author>
	/// <example>
	/// </example>
	internal class VariableQuery
	{
		
		private const string Delimiters = "'\"<>=\n\r; [],()";
		
		private string _query;
		private CMParameterCollection _parameters;
		private Regex _regex;
		
		/// <summary> The Constructor.
		/// 
		/// </summary>
		/// <param name="query">the query that contains variables
		/// </param>
		/// <param name="parameters">the parameters to substitute variables
		/// 
		/// </param>
		public VariableQuery(string query, CMParameterCollection parameters)
		{
			_query = query;
			_parameters = parameters;
			_regex = new Regex(@"@\w+");  // looking for @xxxx
		}
		
		/// <summary>
		/// Scan the query to find each of variables and replace the variable with the value
		/// of corresponding parameter in the parameter collections.
		/// </summary>
		/// <returns>  a new query with its variables replaced by the parameter values.
		/// 
		/// </returns>
		public string Substitute()
		{
			return _regex.Replace(_query, new MatchEvaluator(this.Replace));
		}

		/// <summary>
		/// The delegate to be called by the regular expression parser.
		/// </summary>
		/// <param name="match">The Match</param>
		/// <returns>the replaced string</returns>
		internal string Replace(Match match)
		{
			string parameterName = match.Value.Substring(1);

			CMParameter parameter = _parameters[parameterName];

			if (parameter != null)
			{
				// found the parameter for the variable, return the value for it
				return GetStringValue(parameter);
			}

			// otherwise, return the original value
			return match.Value;
		}
		
		/// <summary>
		/// Gets the string value in order to be concatenated in the new query
		/// 
		/// If the value is a String type, a double quote added.
		/// </summary>
		/// <param name="parameter">the value object.</param>
		/// <returns> the value's string representation.</returns>
		private string GetStringValue(CMParameter parameter)
		{
			string strValue;
			
			if (parameter.DbType == DbType.String)
			{
				// esacpe the double quote
				Regex regex = new Regex("\"");
				strValue = regex.Replace((string) parameter.Value, "\\\"");
				strValue = "\"" + strValue + "\"";
			}
			else
			{
				strValue = parameter.Value + "";
			}
			return strValue;
		}
	}
}