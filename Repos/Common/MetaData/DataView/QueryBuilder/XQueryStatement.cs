/*
* @(#)XQueryStatement.cs
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
	/// Represents a XQuery statement element.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class XQueryStatement : QueryElementBase
	{
		/// <summary>
		/// Initiating an instance of XQueryStatement class
		/// </summary>
		public XQueryStatement() : base()
		{
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder builder = new StringBuilder();

			QueryElementCollection children = Children;
			foreach (IQueryElement child in children)
			{
				builder.Append(child.ToXQuery());
			}

			return builder.ToString();
		}
	}
}