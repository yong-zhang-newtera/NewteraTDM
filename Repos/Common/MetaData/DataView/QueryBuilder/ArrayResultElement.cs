/*
* @(#)ArrayResultElement.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 11 Aug 2004</version>
	/// <author>  Yong Zhang </author>
	internal class ArrayResultElement : QueryElementBase
	{
		private DataArrayAttribute _arrayAttribute;

		/// <summary>
		/// Initiating an instance of ArrayResultElement class
		/// </summary>
		/// <param name="arrayAttribute">The array attribute</param>
		public ArrayResultElement(DataArrayAttribute arrayAttribute) : base()
		{
			_arrayAttribute = arrayAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("{").Append(((IQueryElement) _arrayAttribute).ToXQuery()).Append("}");

			query.Append("\n");

			return query.ToString();
		}
	}
}