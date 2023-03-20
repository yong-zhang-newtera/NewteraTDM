/*
* @(#)ParseTreeRuleNode.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a rule tree node
	/// interface.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005</version>
	/// <author> Yong Zhang </author>
	[Serializable]
	public class ParseTreeRuleNode : ParseTreeNodeBase
	{
		/// <summary>
		/// Instantiate an instance of ParseTreeRuleNode
		/// </summary>
		public ParseTreeRuleNode() : base()
		{
		}

		/// <summary>
		/// Gets or sets the text of the rule node
		/// </summary>
		public override string Text
		{
			get
			{
				if (this._text == null)
				{
					// not initialized, concatenate the text of its children
					this._text = "";
					int index  = 0;
					foreach (IParseTreeNode child in this.Children)
					{
						if (child is ParseTreeTokenNode && index > 0)
						{
							// add a space
							this._text += " ";
						}

						this._text += child.Text;
						index++;
					}
				}

				return _text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Gets the start postion from its first child
		/// </summary>
		public override int TextStart
		{
			get
			{ 
				if (this._textStart < 0)
				{
					// not initialized
					if (Children.Count > 0)
					{
						// get the value from the first child
						this._textStart = Children[0].TextStart;
					}
					else
					{
						this._textStart = 0;
					}
				}

				return this._textStart;
			}
			set
			{
				base.TextStart = value;
			}
		}

		/// <summary>
		/// Gets the length of the text portion represented by the node
		/// </summary>
		public override int TextLength
		{
			get
			{ 
				if (this._textLength < 0)
				{
					// not initialized
					
					if (Children.Count > 0)
					{
						IParseTreeNode lastChild = Children[Children.Count - 1];

						// the last node may be an EOF node which has TextStart
						// as zero. In this case, we will use the node before
						// EOF to get TextStart value
						if (lastChild.TextStart <= 0 && Children.Count > 1)
						{
							IParseTreeNode secondLastChild = Children[Children.Count - 2];
							lastChild.TextStart = secondLastChild.TextStart + secondLastChild.TextLength;
						}

						this._textLength = lastChild.TextStart + lastChild.TextLength - this.TextStart;
					}
					else
					{
						this._textLength = this.Name.Length;
					}
				}

				return _textLength;
			}
			set
			{
				base.TextLength = value;
			}
		}

		/// <summary>
		/// Accept a IParseTreeNodeVisitor visitor to visit this rule node
		/// </summary>
		/// <param name="visitor">The visitor of IParseTreeNodeVisitor type</param>
		public override void Accept(IParseTreeNodeVisitor visitor)
		{
			if (visitor.VisitRule(this))
			{
				foreach (IParseTreeNode child in this.Children)
				{
					child.Accept(visitor);
				}
			}
		}
	}
}