/*
* @(#)SetTextFunction.cs
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
	/// Sets a text to an Xml Node.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class SetTextFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public SetTextFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 2)
			{
				throw new InterpreterException("SetTextFunction expectes two argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			Value val = ((IExpr) _arguments[0]).Eval();
			string text = ((IExpr) _arguments[1]).Eval().ToString();

			if (val.DataType.IsCollection)
			{
				ValueCollection values = val.ToCollection();
				if (values.Count != 1)
				{
					throw new InterpreterException("The first argument of setText Function must represent a single node.");
				}

				XmlNode node = values[0].ToNode();

				if (node.NodeType == XmlNodeType.Element)
				{
					((XmlElement) node).InnerText = text;
				}
				else if (node.NodeType == XmlNodeType.Attribute)
				{
					((XmlAttribute) node).Value = text;
				}
				else
				{
					throw new InterpreterException("Unhandled xml node type");
				}			
			}
			else
			{
				throw new InterpreterException("The first argument of setText Function must be a path");
			}

			return new XString(text);
		}
	}
}