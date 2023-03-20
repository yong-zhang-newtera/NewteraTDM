/*
* @(#)ParseTreeLiteralNode.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a literal tree node
	/// interface.
	/// </summary>
	/// <version> 1.0.0 03 Dec 2005</version>
	/// <author> Yong Zhang </author>
	[Serializable]
	public class ParseTreeLiteralNode : ParseTreeNodeBase
	{
		/// <summary>
		/// Inistantiate an ParseTreeLiteralNode instance
		/// </summary>
		public ParseTreeLiteralNode() : base()
		{
		}

		/// <summary>
		/// Gets or sets the text of the node
		/// </summary>
		public override string Text
		{
			get
			{
				// fix char escapes
				return UnescapeChars(base.Text);
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Accept a IParseTreeNodeVisitor visitor to visit this literal node
		/// </summary>
		/// <param name="visitor">The visitor of IParseTreeNodeVisitor type</param>
		public override void Accept(IParseTreeNodeVisitor visitor)
		{
			if (visitor.VisitLiteral(this))
			{
				foreach (IParseTreeNode child in this.Children)
				{
					child.Accept(visitor);
				}
			}
		}

		/// <summary>
		/// Return a string representing a path from root to this lieral node
		/// </summary>
		/// <returns></returns>
		public override string ToPath()
		{
			if (_path == null)
			{
				if (this.Parent != null)
				{
					_path = this.Parent.ToPath() + "->" + this.Text;
				}
				else
				{
					_path = this.Text;
				}
			}

			return _path;
		}

		/// <summary>
		/// Unescape special chars in the text
		/// </summary>
		/// <param name="s"></param>
		/// <returns>The unescaped string</returns>
		private string UnescapeChars(string s)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < s.Length; i++)
			{
				switch (s[i])
				{
					case '\t':
						sb.Append(@"\t");
						break;
					case '\f':
						sb.Append(@"\f");
						break;
					case '\r':
						sb.Append(@"\r");
						break;
					case '\n':
						sb.Append(@"\n");
						break;
					case '\\':
						sb.Append(@"\");
						break;
					default:
						sb.Append(s[i]);
						break;
				}
			}

			return sb.ToString();
		}
	}
}