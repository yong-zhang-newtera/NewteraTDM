/*
* @(#)FunctionImpBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;

	/// <summary>
	/// This is a base class for function implementation classes, including
	/// builtin and user-defined functions
	/// </summary>
	/// <version> 1.0.0 23 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	abstract public class FunctionImpBase : ExprBase, IFunctionImp
	{
		protected ExprCollection _arguments;
		protected string _name;
			
		/// <summary>
		/// Initiate an instance of FunctionImpBase object.
		/// </summary>
		public FunctionImpBase() : base()
		{
			_arguments = null;
			_name = null;
		}

		#region IFunctionImp Members

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public abstract void CheckArgs(ExprCollection arguments);

		/// <summary>
		/// Initialize the function implementation instance
		/// </summary>
		/// <param name="interpreter">The interpreter</param>
		/// <param name="name">The function name</param>
		/// <param name="arguments">The invoking arguments</param>
		public void Initialize(Interpreter interpreter, string name, ExprCollection arguments)
		{
			this.Interpreter = interpreter;
			_name = name;
			_arguments = arguments;
		}

		/// <summary>
		/// Gets the information indicating whether the value of a function is cacheable.
		/// </summary>
		/// <value>true if the value is cacheable, false otherwise. Default is false</value>
		public virtual bool IsValueCacheable
		{
			get
			{
				return false;
			}
		}

		#endregion


		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.FUNCTIONIMP;
			}
		}

		/// <summary>
		/// Prepare the expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_arguments != null)
			{
				_arguments.Prepare();
			}
		}

		/// <summary>
		/// Restrict the expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_arguments != null)
			{
				_arguments.Restrict();
			}
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Function Implementation for " + _name);
		}
	}
}