/*
* @(#)AncestorAxisEnumerator.cs
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
	/// This is an implementation of enumerator for Descendant Axis of a node.
	/// </summary>
	/// <version>  	1.0.1 19 Nov 2003
	/// </version>
	/// <author>Yong Zhang</author>
	public class DescendantAxisEnumerator : StackedEnumerator
	{
		/// <summary>
		/// Initializing a DescendantAxisEnumerator object
		/// </summary>
		/// <param name="node">the context node.</param>
		/// <param name="document">the document the context node belongs to.</param>
		public DescendantAxisEnumerator(XmlNode contextNode, VDocument document) : base(contextNode, document)
		{
		}
		
		/// <summary>
		/// Initializing a DescendantAxisEnumerator object
		/// </summary>
		/// <param name="node">the context node.</param>
		/// <param name="document">the document the context node belongs to.</param>
		/// <param name="">The selfEnumerator for the context node.</param>
		public DescendantAxisEnumerator(XmlNode contextNode, VDocument document, IEnumerator selfEnumerator):base(contextNode, document, selfEnumerator)
		{
		}

		/// <summary>
		/// Create a ChildAxisEnumerator given a context node.
		/// </summary>
		/// <param name="node">the context node</param>
		/// <returns> the ChildAxisEnumerator </returns>
		protected override IEnumerator CreateEnumerator(XmlNode contextNode)
		{
			IEnumerator enumerator = null;
			
			try
			{
				enumerator = Document.GetChildAxisEnumerator(contextNode);
			}
			catch (UnsupportedAxisException)
			{
				// Return a NULL is ok.
			}
			
			return enumerator;
		}
	}
}