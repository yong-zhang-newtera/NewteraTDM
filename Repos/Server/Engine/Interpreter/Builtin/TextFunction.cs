/*
* @(#)TextFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;

	/// 
	/// <summary>
	/// Gets the text of an xml element or attribute.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class TextFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public TextFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 1)
			{
				throw new InterpreterException("TextFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			XmlNode node = ((IExpr) _arguments[0]).Eval().ToNode();

			string text = null;

			if (node == null)
			{
				text = "";
			}
			else if (node.NodeType == XmlNodeType.Element)
			{
				text = ((XmlElement) node).InnerText;
			}
			else if (node.NodeType == XmlNodeType.Attribute)
			{
				text = ((XmlAttribute) node).Value;
			}
			else
			{
				throw new InterpreterException("Unhandled xml node type");
			}

			return new XString(text);
		}
	}
}