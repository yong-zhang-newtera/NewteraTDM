/*
* @(#)IParseTreeNodeVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse a parse tree.of IParseTreeNode type
	/// </summary>
	/// <version> 1.0.0 03 Dec 2005 </version>
	/// <author> Yong Zhang</author>
	public interface IParseTreeNodeVisitor
	{
		/// <summary>
		/// Viste a rule node.
		/// </summary>
		/// <param name="node">A ParseTreeRuleNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		bool VisitRule(ParseTreeRuleNode node);

		/// <summary>
		/// Viste a token node.
		/// </summary>
		/// <param name="node">A ParseTreeTokenNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		bool VisitToken(ParseTreeTokenNode node);

		/// <summary>
		/// Viste a literal node.
		/// </summary>
		/// <param name="node">A ParseTreeLiteralNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		bool VisitLiteral(ParseTreeLiteralNode node);
	}
}