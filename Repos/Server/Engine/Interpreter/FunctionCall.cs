/*
* @(#)FunctionCall.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Representing a function invocation.
	///
	/// At runtime this connects itself to a FunctionDef that is the
	/// function implementation
	///
	/// </summary>
	/// <version>  1.0.0 16 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	/// <remarks>
	public class FunctionCall : ExprBase, ITraceable, ITraversable
	{
		private string _name;
		private ExprCollection _arguments;
		private FunctionDef _def;
		
		/// <summary>
		/// Initiate an instance of FunctionCall class
		/// </summary>
		/// <param name="name">function name.</param>
		/// <param name="arguments">The arguments </param>
		public FunctionCall(Interpreter interpreter, string name, ExprCollection arguments) : base(interpreter)
		{
			_def = null;
			_name = name;
			if (arguments != null)
			{
				_arguments = arguments;
			}
			else
			{
				_arguments = new ExprCollection();
			}
		}
		
		/// <summary>
		/// Gets the function name.
		/// </summary>
		/// <value> function name</value>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the implementation of function.
		/// </summary>
		/// <value> function implementation</value>
		public IExpr FunctionImp
		{
			get
			{
				return FindDef().Expression;
			}
		}

		/// <summary> 
		/// Gets the arguments of the function call.
		/// </summary>
		/// <value> A collection of arguments</value>
		public ExprCollection Arguments
		{
			get
			{
				return _arguments;
			}
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.FUNCTION;
			}
		}

		/// <summary>
		/// Prepare the identifier in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			// do not call Prepare on arguments here. Arguments are prepared by
			// function implementation

			FunctionDef funcDef = FindDef();

			// exception is thrown if checkArgs failed.
			funcDef.CheckArgs(_arguments);

			funcDef.Expression.Prepare();
		}

		/// <summary>
		/// Restrict the identifier in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			// do not call Restrict on arguments here. Arguments are restricted by
			// function implementation

			FunctionDef funcDef = FindDef();

			funcDef.Expression.Restrict();
		}

		/// <summary>
		/// Evaluate the identifier in the phase three of interpreting
		/// </summary>
		/// <returns>The Value object assigned to this identifier.</returns>
		public override Value Eval()
		{
			FunctionDef funcDef = FindDef();

			if (funcDef.IsValueCacheable)
			{
				// use cached value
				if (funcDef.Value == null)
				{
					funcDef.Expression.Eval();
				}

				return funcDef.Value;
			}
			else
			{
				// do not cache value
				return funcDef.Expression.Eval();
			}
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Function call " + _name + "(" + _arguments.ToString() + ")");
		}
		
		/// <summary> Called to get the actual FunctionDef to handle the function. In
		/// most cases this will be the FunctionDef itself. For
		/// PolyFunctions though it could be a different handler.
		/// 
		/// </summary>
		/// <exception cref=""> InterpreterException
		/// </exception>
		/// <returns> FunctionDef
		/// 
		/// </returns>
		private FunctionDef FindDef()
		{
			if (_def == null)
			{
				// find the function definition from symbol table
				_def = (FunctionDef) Interpreter.SymbolTable.LookupFunction(_name, _arguments);
				
				if (_def == null)
				{
					throw new InterpreterException("Unable to find function definition for " + _name);
				}
			}

			return _def;
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the expression.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			FunctionDef funcDef = FindDef();

			// The owner document may have been cached by the previous call,
			// just return it if it exists, otherwise trace the document from
			// the function implementation.
			if (funcDef.Value == null)
			{
				IExpr imp = funcDef.Expression;

				if (!(imp is ITraceable))
				{
					throw new InterpreterException("The function " + funcDef.Name + " is not traceable.");
				}

				funcDef.Value = ((ITraceable) imp).TraceDocument();
			}

			return funcDef.Value;
		}

		/// <summary>
		/// Get an enumerator of the absolute path, including the base path referenced
		/// by the context.
		/// </summary>
		/// <returns>The PathEnumerator</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			PathEnumerator enumerator = new PathEnumerator();

			FunctionDef funcDef = FindDef();

			IExpr imp = funcDef.Expression;

			if (imp is ITraceable)
			{
				enumerator = ((ITraceable) imp).GetAbsolutePathEnumerator();
			}
			else
			{
				enumerator = new PathEnumerator();
			}

			return enumerator;
		}

		#endregion

		#region ITraversable Members

		/// <summary> 
		/// Gets child count the expression.
		/// </summary>
		/// <value> Child count </returns>
		public int ChildCount
		{
			get
			{
				return 0;
			}
		}

		/// <summary> 
		/// Accept an ExprVisitor that will traverse the expression
		/// </summary>
		/// <param name="visitor">The ExprVisitor</param>
		public void Accept(IExprVisitor visitor)
		{
			visitor.Visit(this);
		}

		#endregion
	}
}