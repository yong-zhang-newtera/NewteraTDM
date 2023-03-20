/*
* @(#)InlinedXmlAttribute.cs
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
	/// Represents an attribute inside an Inlined xml tag.
	/// </summary>
	/// <version>  	1.0.0 10 Mar 2004</version>
	/// <author>  Yong Zhang </author>
	internal class InlinedXmlAttribute : QueryElementBase
	{
		private string _name;
		private string _value;

		/// <summary>
		/// Initiating an instance of InlinedXmlAttribute class
		/// </summary>
		/// <param name="name">The name of attribute</param>
		/// <param name="value">The attribute value</param>
		public InlinedXmlAttribute(string name, string value) : base()
		{
			_name = name;
			_value = value;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			return _name + "=\"" + _value + "\"";
		}
	}
}