/*
* @(#)Document.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	
	/// <summary> 
	/// Create an VDocumnet based on the url speficified in the argument.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	public class Document : FunctionImpBase, ITraceable
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public Document() : base()
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
				throw new InterpreterException("DocumentFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary>
		/// Gets the information indicating whether the value of a function is cacheable.
		/// </summary>
		/// <value>true for document function</value>
		public override bool IsValueCacheable
		{
			get
			{
				return true;
			}
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			string url = ((IExpr) _arguments[0]).Eval().ToString();

			VDocument doc = DocumentFactory.Instance.Create(url);
			
			// we have to do this here
			doc.Interpreter = this.Interpreter;

			return new XNode(doc);
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the expression.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			// return the VDocument node
			return this.Eval();
		}

		public PathEnumerator GetAbsolutePathEnumerator()
		{
			PathEnumerator enumerator = new PathEnumerator();
			return enumerator;
		}

		#endregion
	}
}