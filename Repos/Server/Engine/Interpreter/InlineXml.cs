/*
* @(#)InlineXml.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents an inline xml in a query.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class InlineXml : ExprBase
	{
		/// <summary>
		/// Initiate an instance of InlineXml object.
		/// </summary>
		/// <param name="inlineXml">The inlineXml</param>
		public InlineXml(Interpreter interpreter, string inlineXml) : base(interpreter)
		{
			XmlDocument doc = DocumentFactory.Instance.CreateFromXMLString(inlineXml);
			this.DataType = new NodeType();
			this.Value = new XNode(doc.DocumentElement);
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.INLINE;
			}
		}

		/// <summary>
		/// Prepare the inline xml in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
		}

		/// <summary>
		/// Restrict the inline xml in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
		}

		/// <summary>
		/// Evaluate the inline xml in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			return this.Value;
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Inline Xml");
		}
	}
}