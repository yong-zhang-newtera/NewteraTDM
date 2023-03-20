/*
* @(#)RuleStatement.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

    using Newtera.Common.MetaData.Rules;

	/// <summary>
	/// Represents a rule statement which is consist of if-then-else syntax.
	/// </summary>
	/// <version>  	1.0.0 18 Oct 2007</version>
	internal class RuleStatement : QueryElementBase
	{
        private RuleDef _ruleDef;
		private string _alias;
		private string _schemaName;
		private string _schemaVersion;

		/// <summary>
		/// Initiating an instance of RuleStatement class
		/// </summary>
        /// <param name="ruleDef">The rule definition instance</param>
		/// <param name="alias">The unique alias of a class</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		public RuleStatement(RuleDef ruleDef, string alias, string schemaName, string schemaVersion) : base()
		{
            _ruleDef = ruleDef;
			_alias = alias;
			_schemaName = schemaName;
			_schemaVersion = schemaVersion;
		}

		/// <summary>
		/// Gets the XQuery representation of the rule.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
            // propagated the runtime values as parameters to the
            // functions that are part of the rule.
            _ruleDef.PropagateParameters();

			StringBuilder query = new StringBuilder();

            if (_ruleDef.Condition != null && _ruleDef.ThenAction != null)
            {
                string condition = ((IQueryElement)_ruleDef.Condition).ToXQuery();
                // replace the alias of the class that owns the rule with the one of the current base class
                if (_ruleDef.ClassAlias != _alias)
                {
                    condition = condition.Replace("$" + _ruleDef.ClassAlias, "$" + _alias);
                }

                query.Append("<Result>{if (").Append(condition).Append(")");
                query.Append(" then ").Append(((IQueryElement)_ruleDef.ThenAction).ToXQuery());
                query.Append(" else ");
                if (_ruleDef.ElseAction != null)
                {
                    query.Append(((IQueryElement)_ruleDef.ElseAction).ToXQuery());
                }
                else
                {
                    query.Append("noop()");
                }

                query.Append("}</Result>");
            }

			return query.ToString();
		}
	}
}