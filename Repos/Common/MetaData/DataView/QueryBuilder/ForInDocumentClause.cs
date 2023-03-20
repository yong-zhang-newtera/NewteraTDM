/*
* @(#)ForInDocumentClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents a For clause element in XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class ForInDocumentClause : QueryElementBase
	{
		private string _alias;
		private string _className;
		private string _schemaName;
		private string _schemaVersion;
		private int _from;
		private int _to;

		/// <summary>
		/// Initiating an instance of ForInDocumentClause class
		/// </summary>
		/// <param name="alias">The unique alias of a class</param>
		/// <param name="className">The class name</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		/// <param name="from">Start from</param>
		/// <param name="to">To</param>
		public ForInDocumentClause(string alias, string className, string schemaName,
			string schemaVersion, int from, int to) : base()
		{
			_alias = alias;
			_className = className;
			_schemaName = schemaName;
			_schemaVersion = schemaVersion;
			_from = from;
			_to = to;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("for $").Append(_alias).Append(" in document(\"db://");
			query.Append(_schemaName).Append(".xml");
			if (_schemaVersion != "1.0")
			{
				query.Append("?Version=").Append(_schemaVersion);
			}
			// NOTE: do not put "//" which represents a Decendant step.
			// a Decendant step will cause the path navigator to travel all
			// the nodes in a xml document, which has big impact to the performance.
			// therefore, we must construct the path with child step, denoted by "/"
			query.Append("\")/");
			query.Append(_className).Append("List/").Append(_className);
			query.Append("[").Append(_from).Append(" to ").Append(_to).Append("]").Append("\n");

			return query.ToString();
		}
	}
}