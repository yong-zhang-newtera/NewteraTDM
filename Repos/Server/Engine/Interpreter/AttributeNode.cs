/*
* @(#)AttributeNode.cs
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
	/// Represents a xml attribute node expression.
	/// </summary>
	/// <version> 1.0.0 06 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class AttributeNode : ExprBase
	{
		private string _name;
		private IExpr _valueExpr;

		/// <summary>
		/// Initiate an instance of AttributeNode object.
		/// </summary>
		/// <param name="name">name of attribute</param>
		/// <param name="valueExpr">Attribute value expr</param>
		public AttributeNode(Interpreter interpreter, string name, IExpr valueExpr) : base(interpreter)
		{
			_name = name;
			_valueExpr = valueExpr;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.ATTRIBUTE;
			}
		}

		/// <summary>
		/// Prepare the AttributeNode expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_valueExpr != null)
			{
				_valueExpr.Prepare();
			}
		}

		/// <summary>
		/// Restrict the AttributeNode expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_valueExpr != null)
			{
				_valueExpr.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the AttributeNode expression in the phase three of interpreting
		/// </summary>
		/// <returns>A XNode object</returns>
		public override Value Eval()
		{
			XmlAttribute xmlAttribute = Interpreter.Document.CreateAttribute(_name);

			Value val = _valueExpr.Eval();

			xmlAttribute.Value = val.ToString();
						
			return new XNode(xmlAttribute);
		}

		/// <summary>
		/// Print the information about the expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine(_name + "="+ _valueExpr.Eval().ToString());
		}
	}
}