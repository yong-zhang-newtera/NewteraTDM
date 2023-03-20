/*
* @(#)VirtualResultElement.cs
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
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 26 May 2007</version>
	internal class VirtualResultElement : QueryElementBase
	{
		private DataVirtualAttribute _virtualAttribute;

		/// <summary>
		/// Initiating an instance of VirtualResultElement class
		/// </summary>
		/// <param name="virtualAttribute">The attribute</param>
		public VirtualResultElement(DataVirtualAttribute virtualAttribute) : base()
		{
			_virtualAttribute = virtualAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("{").Append(((IQueryElement) _virtualAttribute).ToXQuery()).Append("}");

			query.Append("\n");

			return query.ToString();
		}
	}
}