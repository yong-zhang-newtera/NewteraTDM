/*
* @(#)ForClause.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
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
	/// <version>  	1.0.0 04 Dec 2007</version>
	internal class ForClause : QueryElementBase
	{
		private string _alias;
		private string _className;
		private string _referringClassAlias;
		private string _referringRelationshipName;
        private bool _isInlineXml;

		/// <summary>
		/// Initiating an instance of ForClause class
		/// </summary>
		/// <param name="alias">The unique alias of a referenced class</param>
		/// <param name="className">The refenced class name</param>
        /// <param name="referringClassAlias">The referring class alias</param>
		/// <param name="referringRelationship">The name of parent relationship attribute</param>
		public ForClause(string alias, string className, string referringClassAlias, string referringRelationship) :
            this(alias, className, referringClassAlias, referringRelationship, false)
		{
		}

        /// <summary>
        /// Initiating an instance of ForClause class
        /// </summary>
        /// <param name="alias">The unique alias of a referenced class</param>
        /// <param name="className">The refenced class name</param>
        /// <param name="referringClassAlias">The referring class alias</param>
        /// <param name="referringRelationship">The name of parent relationship attribute</param>
        /// <param name="isInlineXml">true if this let clause is used for an inline xml, false if it is used for a virtual xml doc</param>
        public ForClause(string alias, string className, string referringClassAlias, string referringRelationship, bool isInlineXml)
            : base()
        {
            _alias = alias;
            _className = className;
            _referringClassAlias = referringClassAlias;
            _referringRelationshipName = referringRelationship;
            _isInlineXml = isInlineXml;
        }

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("for $").Append(_alias).Append(" in ");
			query.Append("$").Append(_referringClassAlias);
            if (!_isInlineXml)
            {
                // use @ to indicate the following symbol is a relastionship name
                query.Append("/@");
            }
            else
            {
                // treat a relationship in in-line xml as a regular xml element
                query.Append("/");
            }
			query.Append(_referringRelationshipName);
            if (!_isInlineXml)
            {
                // use => to navigate to the referenced class
                query.Append("=>").Append(_className);
            }
            query.Append("\n");

			return query.ToString();
		}
	}
}