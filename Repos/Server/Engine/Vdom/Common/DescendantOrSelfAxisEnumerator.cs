/*
* @(#)DescendantOrSelfAxisEnumerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Common
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// This is an implementation of enumerator for Descendant or self Axis of a node.
	/// </summary>
	/// <version>  	1.0.1 19 Nov 2003
	/// </version>
	/// <author> Yong Zhang</author>
	public class DescendantOrSelfAxisEnumerator : DescendantAxisEnumerator
	{
		/// <summary>
		/// Initializing a DescendantOrSelfAxisEnumerator
		/// </summary>
		/// <param name="node">the context node</param>
		/// <param name="document">the document the context node belongs to </param>
		public DescendantOrSelfAxisEnumerator(XmlNode contextNode, VDocument document) : base(contextNode, document, document.GetSelfAxisEnumerator(contextNode))
		{
		}
	}
}