/*
* @(#)BaseClassElement.cs
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
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class BaseClassElement : QueryElementBase
	{
		private DataClass _baseClass;

		/// <summary>
		/// Initiating an instance of BaseClassElement class
		/// </summary>
		/// <param name="baseClass">The base class of a data view</param>
		public BaseClassElement(DataClass baseClass) : base()
		{
			_baseClass = baseClass;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("<").Append(_baseClass.ClassName).Append(" {");
			query.Append("$").Append(_baseClass.Alias).Append("/@").Append(NewteraNameSpace.OBJ_ID).Append(", ");
			query.Append("$").Append(_baseClass.Alias).Append("/@").Append(XMLSchemaInstanceNameSpace.PREFIX).Append(":").Append(NewteraNameSpace.TYPE).Append(", ");
			query.Append("$").Append(_baseClass.Alias).Append("/@").Append(NewteraNameSpace.ATTACHMENTS).Append(", ");
			query.Append("$").Append(_baseClass.Alias).Append("/@").Append(NewteraNameSpace.PERMISSION).Append(", ");
            query.Append("$").Append(_baseClass.Alias).Append("/@").Append(NewteraNameSpace.READ_ONLY);
			query.Append("}>\n");

			foreach (IQueryElement child in Children)
			{
				query.Append(child.ToXQuery()).Append(" ");
			}

			query.Append("</").Append(_baseClass.ClassName).Append(">\n");

			return query.ToString();
		}
	}
}