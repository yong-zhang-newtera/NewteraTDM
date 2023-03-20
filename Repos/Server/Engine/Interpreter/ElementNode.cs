/*
* @(#)ElementNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a element node expression.
	/// </summary>
	/// <version> 1.0.0 15 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class ElementNode : ExprBase
	{
		private string _name;
		private string _prefix;
		private IExpr _content;
		private ExprCollection _attributes;

		/// <summary>
		/// Initiate an instance of ElementNode object.
		/// </summary>
		public ElementNode(Interpreter interpreter, string name, IExpr content) : this(interpreter, name, null, null, content)
		{
		}

		/// <summary>
		/// Initiate an instance of ElementNode object.
		/// </summary>
		public ElementNode(Interpreter interpreter, string name, ExprCollection attributes, IExpr content) : this(interpreter, name, null, attributes, content)
		{
		}

		/// <summary>
		/// Initiate an instance of ElementNode object.
		/// </summary>
		/// <param name="name">name of element</param>
		/// <param name="prefix">Prefix of element</param>
		/// <param name="attributes">Element attributes</param>
		/// <param name="content">The element content expr</param>
		public ElementNode(Interpreter interpreter, string name, string prefix, ExprCollection attributes, IExpr content) : base(interpreter)
		{
			_name = name;
			_prefix = prefix;
			_content = content;
			_attributes = attributes;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.ELEMENT;
			}
		}

		/// <summary>
		/// Prepare the ElementNode expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_content != null)
			{
				_content.Prepare();
			}

			if (_attributes != null)
			{
				_attributes.Prepare();
			}
		}

		/// <summary>
		/// Restrict the ElementNode expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_content != null)
			{
				_content.Restrict();
			}

			if (_attributes != null)
			{
				_attributes.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the ElementNode expression in the phase three of interpreting
		/// </summary>
		/// <returns>A XNode object</returns>
		public override Value Eval()
		{
			XmlElement element = Interpreter.Document.CreateElement(_name);
	
			if (_attributes != null)
			{
				Value attributeValues = _attributes.Eval();
				// add element attributes
				AddElementAttributes(element, attributeValues);
			}

			Value val = _content.Eval();

			AddElementChildren(element, val);
						
			return new XNode(element);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("<" + _name + ">"+ _content.Eval().ToString() + "</" + _name + ">");
		}

		/// <summary>
		/// Add children of element
		/// </summary>
		/// <param name="element">the element</param>
		/// <param name="val">the content value</param>
		private void AddElementChildren(XmlElement element, Value val)
		{
			if (val.DataType.IsXmlNode)
			{
				// make sure that child and parent elements belong to
				// the same document
				XmlNode child;
				if (element.OwnerDocument != val.ToNode().OwnerDocument)
				{
					child = element.OwnerDocument.ImportNode(val.ToNode(), true);
				}
				else
				{
					child = val.ToNode();
				}

				// make it a child element
				element.AppendChild(child);
			}
			else if (val.DataType.IsCollection)
			{
				ValueCollection values = val.ToCollection();
				foreach (Value currentVal in values)
				{
					AddElementChildren(element, currentVal);
				}
			}
			else
			{
				// make it as an inner text
				element.InnerText = val.ToString();
			}
		}

		/// <summary>
		/// Add attributes of element
		/// </summary>
		/// <param name="element">the element</param>
		/// <param name="val">the attribute value</param>
		private void AddElementAttributes(XmlElement element, Value val)
		{
			if (val.DataType.IsXmlNode && val.ToNode() is XmlAttribute)
			{
				// make sure that child and parent elements belong to
				// the same document
				XmlNode attribute;
				if (element.OwnerDocument != val.ToNode().OwnerDocument)
				{
					attribute = element.OwnerDocument.ImportNode(val.ToNode(), true);
				}
				else
				{
					attribute = val.ToNode();
				}

				// Add it as an attribute
				element.SetAttributeNode((XmlAttribute) attribute);
			}
			else if (val.DataType.IsCollection)
			{
				ValueCollection values = val.ToCollection();
				foreach (Value currentVal in values)
				{
					AddElementAttributes(element, currentVal);
				}
			}
		}
	}
}