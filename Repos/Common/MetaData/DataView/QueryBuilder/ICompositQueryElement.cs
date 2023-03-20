/*
* @(#)ICompositQueryElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an interface for all composit elements in XQuery,
	/// such as for clause, let clause, etc.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal interface ICompositQueryElement
	{
		/// <summary>
		/// Gets the child elements of the element
		/// </summary>
		/// <value>A Parent element</value>
		QueryElementCollection Children {get;}
	}
}