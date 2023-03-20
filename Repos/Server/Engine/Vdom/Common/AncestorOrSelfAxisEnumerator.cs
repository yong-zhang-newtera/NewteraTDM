/*
* @(#)AncestorOrSelfAxisEnumerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Common
{
	using System;
	using System.Xml;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// This is an implementation of enumerator for Ancestor or self Axis of a node.
	/// </summary>
	/// <version>  	1.0.1 19 Nov 2003
	/// </version>
	/// <author> Yong Zhang </author>
	public class AncestorOrSelfAxisEnumerator : AncestorAxisEnumerator
	{
		/// <summary>
		/// Initializing a AncestorOrSelfAxisEnumerator
		/// </summary>
		/// <param name="node">the context node.</param>
		/// <param name="document">the document.</param>
		public AncestorOrSelfAxisEnumerator(XmlNode contextNode, VDocument document) : base(contextNode, document, document.GetSelfAxisEnumerator(contextNode))
		{
		}
	}
}