/*
* @(#)BehindHoursFunction.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
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
    /// Gets the number of hours in advance of a time specified by an xml element or attribute.
    /// </summary>
	/// <version>  1.0 20 Sep 2013</version>
	public class BehindHoursFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public BehindHoursFunction() : base()
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
				throw new InterpreterException("BehindHoursFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			XmlNode node = ((IExpr) _arguments[0]).Eval().ToNode();

            double hours = 0;

            if (node == null)
            {
                hours = int.MaxValue;
            }
            else if (node.NodeType == XmlNodeType.Element)
            {
                try
                {
                    DateTime baseDate = DateTime.Parse(((XmlElement)node).InnerText);

                    TimeSpan timeSpan = DateTime.Now.Subtract(baseDate);

                    hours = timeSpan.TotalHours;
                }
                catch (Exception)
                {
                    hours = int.MaxValue;
                }
            }
            else if (node.NodeType == XmlNodeType.Attribute)
            {
                DateTime baseDate = DateTime.Parse(((XmlAttribute)node).Value);

                TimeSpan timeSpan = DateTime.Now.Subtract(baseDate);

                hours = timeSpan.TotalHours;
            }
            else
            {
                throw new InterpreterException("Unhandled xml node type");
            }

            return new XDouble(hours);
		}
	}
}