/*
* @(#)InlinedXmlClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;

	/// <summary>
	/// Represents a clause of edit-related builtin function calls.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class InlinedXmlClause : QueryElementBase
	{
		private string _alias;
		private string _classType;
		private string _rootClassType;
		private string _objId;
		private QueryElementCollection _attributes;

		/// <summary>
		/// Initiating an instance of InlinedXmlClause class
		/// </summary>
		/// <param name="alias">The unique alias of a class</param>
		/// <param name="classType">The class type</param>
		/// <param name="rootClassType">The root class type</param>
		public InlinedXmlClause(string alias, string classType, string rootClassType) : this(alias, classType, rootClassType, null)
		{
		}

		/// <summary>
		/// Initiating an instance of InlinedXmlClause class
		/// </summary>
		/// <param name="alias">The unique alias of a class</param>
		/// <param name="classType">The class type</param>
		/// <param name="rootClassType">The root class type</param>
		/// <param name="objId">The id of an instance</param>
		public InlinedXmlClause(string alias, string rootClassType, string classType, string objId) : base()
		{
			_alias = alias;
			_classType = classType;
			_rootClassType = rootClassType;
			_objId = objId;
			_attributes = null;
		}

		/// <summary>
		/// Add an attribute to the root tag of the inlined xml instance
		/// </summary>
		/// <param name="attribute">An InlinedXmlAttribute object</param>
		public void AddAttribute(InlinedXmlAttribute attribute)
		{
			if (_attributes == null)
			{
				_attributes = new QueryElementCollection();
			}

			_attributes.Add(attribute);
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("let $").Append(_alias).Append(" := ");

			query.Append("[[<").Append(_rootClassType).Append(" ");
			if (!string.IsNullOrEmpty(_objId))
			{
				query.Append("obj_id=\"").Append(_objId).Append("\" ");
			}

			if (_attributes != null)
			{
				foreach (IQueryElement attribute in _attributes)
				{
					query.Append(attribute.ToXQuery()).Append(" ");
				}
			}

			query.Append("xmlns:xsi=\"").Append(XMLSchemaInstanceNameSpace.URI).Append("\" ");
			query.Append(" xsi:type=\"").Append(_classType).Append("\">\n");

			// append individual attributes
			foreach (IQueryElement child in Children)
			{
				query.Append(child.ToXQuery());
			}

			query.Append("</").Append(_rootClassType).Append(">]] \n");

			return query.ToString();
		}
	}
}