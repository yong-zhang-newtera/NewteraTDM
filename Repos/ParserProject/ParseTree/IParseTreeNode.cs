/*
* @(#)IParseTreeNode.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for all nodes in a parse tree.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005</version>
	/// <author> Yong Zhang </author>
	public interface IParseTreeNode
	{
		/// <summary>
		/// Gets or sets the name of the node
		/// </summary>
		string Name {get; set;}

		/// <summary>
		/// Gets or sets the caption of the node
		/// </summary>
		string Caption {get; set;}

		/// <summary>
		/// Gets or sets the text of the node
		/// </summary>
		string Text {get; set;}

		/// <summary>
		/// Gets or sets the parent of the node.
		/// </summary>
		IParseTreeNode Parent {get; set;}

		/// <summary>
		/// Gets the children of the node
		/// </summary>
		ParseTreeNodeCollection Children {get;}

		/// <summary>
		/// Gets or sets the start position of the text portion represented by the node
		/// </summary>
		int TextStart {get; set; }

		/// <summary>
		/// Gets or sets the length of the text portion represented by the node
		/// </summary>
		int TextLength {get; set;}

		/// <summary>
		/// Gets or sets the information indicating whether the node is a terminal node.
		/// </summary>
		bool IsTerminal {get; set;}

		/// <summary>
		/// Gets or sets the information indicating whether the node is hidden node.
		/// </summary>
		bool IsHidden {get; set;}

		/// <summary>
		/// Accept a IParseTreeNodeVisitor visitor to visit this node
		/// </summary>
		/// <param name="visitor">The visitor of IParseTreeNodeVisitor type.</param>
		void Accept(IParseTreeNodeVisitor visitor);

		/// <summary>
		/// Return a string representing a path from root to this node
		/// </summary>
		/// <returns></returns>
		string ToPath();
	}
}