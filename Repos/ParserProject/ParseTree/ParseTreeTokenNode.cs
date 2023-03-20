/*
* @(#)ParseTreeTokenNode.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a token tree node
	/// interface.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005</version>
	/// <author> Yong Zhang </author>
	[Serializable]
	public class ParseTreeTokenNode : ParseTreeNodeBase
	{
		/// <summary>
		/// Inistantiate an ParseTreeTokenNode instance
		/// </summary>
		public ParseTreeTokenNode() : base()
		{
		}

		/// <summary>
		/// Gets the length of the text portion represented by the node
		/// </summary>
		public override int TextLength
		{
			get
			{ 
				if (this.Name == PredefinedTokens.EMPTY_LINE)
				{
					return 0;
				}

				return base.TextLength;
			}
			set
			{
				base.TextLength = value;
			}
		}

		/// <summary>
		/// Accept a IParseTreeNodeVisitor visitor to visit this token node
		/// </summary>
		/// <param name="visitor">The visitor of IParseTreeNodeVisitor type</param>
		public override void Accept(IParseTreeNodeVisitor visitor)
		{
			if (visitor.VisitToken(this))
			{
				foreach (IParseTreeNode child in this.Children)
				{
					child.Accept(visitor);
				}
			}
		}
	}
}