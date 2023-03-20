/*
* @(#)ParseTreeNodeBase.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;

	/// <summary>
	/// Represents a base class for all concrete implementation of IParseTreeNode
	/// interface.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005</version>
	/// <author> Yong Zhang </author>
	[Serializable]
	public abstract class ParseTreeNodeBase : IParseTreeNode
	{
		private string _name = null;
		private string _caption = null;
		protected string _text = null;
		private IParseTreeNode _parent = null;
		private ParseTreeNodeCollection _children = new ParseTreeNodeCollection();
		protected int _textStart = -1;
		protected int _textLength = -1;
		private bool _isTerminal = false;
		private bool _isHidden = false;


		[System.Xml.Serialization.XmlIgnoreAttribute]
		protected string _path = null; // run-time use

		#region IParseTreeNode

		/// <summary>
		/// Gets or sets the text of the node
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the caption of the node
		/// </summary>
		public string Caption
		{
			get
			{
				if (_caption == null)
				{
					return _name;
				}
				else
				{
					return _caption;
				}
			}
			set
			{
				_caption = value;
			}
		}

		/// <summary>
		/// Gets or sets the text of the node
		/// </summary>
		public virtual string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Gets or sets the parent of the node.
		/// </summary>
		public IParseTreeNode Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Gets the children of the node
		/// </summary>
		public ParseTreeNodeCollection Children
		{
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// Gets or sets the start position of the text portion represented by the node
		/// </summary>
		public virtual int TextStart
		{
			get
			{ 
				return _textStart;
			}
			set
			{
				_textStart = value;
			}
		}

		/// <summary>
		/// Gets the length of the text portion represented by the node
		/// </summary>
		public virtual int TextLength
		{
			get
			{ 
				return _textLength;
			}
			set
			{
				_textLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the node is
		/// terminal. If true, the node is shown as a terminal node in the
		/// tree.
		/// </summary>
		public bool IsTerminal
		{
			get
			{
				return _isTerminal;
			}
			set
			{
				_isTerminal = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the node is
		/// hidden in the parse tree. If true, the node is not showing in
		/// the parse tree, false otherwise.
		/// </summary>
		public bool IsHidden
		{
			get
			{
				return _isHidden;
			}
			set
			{
				_isHidden = value;
			}
		}

		/// <summary>
		/// Accept a IParseTreeNodeVisitor visitor to visit this node
		/// </summary>
		/// <param name="visitor"></param>
		public abstract void Accept(IParseTreeNodeVisitor visitor);

		/// <summary>
		/// Return a string representing a path from root to this node
		/// </summary>
		/// <returns></returns>
		public virtual string ToPath()
		{
			if (_path == null)
			{
				if (this.Parent != null)
				{
					_path = this.Parent.ToPath() + "->" + (this.Name != null? this.Name : this.Text);
				}
				else
				{
					_path = this.Name;
				}
			}

			return _path;
		}

		#endregion
	}
}