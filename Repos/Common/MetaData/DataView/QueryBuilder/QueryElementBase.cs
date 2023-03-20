/*
* @(#)QueryElementBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;
	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Represents a base class for all other IQueryElement subclasses.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal abstract class QueryElementBase : IQueryElement, ICompositQueryElement
	{
		private QueryElementCollection _children;

		/// <summary>
		/// Initiating an instance of QueryElementBase class
		/// </summary>
		public QueryElementBase()
		{
			_children = new QueryElementCollection();
		}

		/// <summary>
		/// Gets or sets the child elements of the element
		/// </summary>
		/// <value>A Parent element</value>
		public QueryElementCollection Children {
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns> 
		public abstract string ToXQuery();

		/// <summary>
		/// Escape special chars that are contained in the edit value and disallowed
		/// in an xml
		/// </summary>
		/// <param name="orig">Original</param>
		/// <returns>Escaped string</returns>
		protected string EscapeChars(string orig)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < orig.Length; i++)
			{
				if (orig[i] == '<')
				{
					builder.Append("&lt;");
				}
				else if (orig[i] == '>')
				{
					builder.Append("&gt;");
				}
				else if (orig[i] == '&')
				{
					builder.Append("&amp;");
				}
				else
				{
					builder.Append(orig[i]);
				}
			}
			
			return builder.ToString();
		}
	}
}