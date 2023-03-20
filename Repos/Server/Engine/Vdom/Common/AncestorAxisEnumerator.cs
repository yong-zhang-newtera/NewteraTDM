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
	/// This is an implementation of enumerator for Ancestor Axis of a node.
	/// </summary>
	/// <version>  	1.0.1 19 Nov 2003
	/// </version>
	/// <author>Yong Zhang</author>
	public class AncestorAxisEnumerator : StackedEnumerator
	{
		/// <summary>
		/// Initializing a AncestorAxisEnumerator
		/// </summary>
		/// <param name="node">the context node</param>
		/// <param name="the"> the document that the context node belongs.</param>
		public AncestorAxisEnumerator(XmlNode contextNode, VDocument document) : base(contextNode, document)
		{
		}
		
		/// <summary>
		/// Initializing a AncestorAxisEnumerator
		/// </summary>
		/// <param name="node">the context node</param>
		/// <param name="the"> the document that the context node belongs.</param>
		/// <param name="the">self enumerator of the context node</param>
		public AncestorAxisEnumerator(XmlNode contextNode, VDocument document, IEnumerator selfEnumerator) : base(contextNode, document, selfEnumerator)
		{
		}

		/// <summary>
		/// Create a ParentAxisEnumerator given a context node.
		/// </summary>
		/// <param name="node">the context node</param>
		/// <returns> the ParentAxisEnumerator</returns>
		protected override IEnumerator CreateEnumerator(XmlNode contextNode)
		{
			IEnumerator enumerator = null;
			
			try
			{
				enumerator = Document.GetParentAxisEnumerator(contextNode);
			}
			catch (UnsupportedAxisException)
			{
				// Return a NULL is ok.
			}
			
			return enumerator;
		}
	}
}