/*
* @(#)Ident.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an identifier for variable.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class Ident : ExprBase, ITraceable
	{
		private string _name;
		private bool _isDef;

		/// <summary>
		/// Initiate an instance of Ident object.
		/// </summary>
		/// <param name="name">Name of identifier</param>
		/// <param name="isDef">Whether it is to define a variable</param>
		public Ident(Interpreter interpreter, string name, bool isDef) : base(interpreter)
		{
			_name = name;
			_isDef = isDef;

			SymbolTable symbolTab = interpreter.SymbolTable;
			if (_isDef)
			{
				if (symbolTab.Lookup(name) != null)
				{
					throw new InterpreterException("The variable name " + name + " already exists.");
				}

				Variable variable = new Variable(name);

				symbolTab.Add(variable);
			}
			else
			{
				// the identifier is a reference, the symbol table entry must have existed
				if (symbolTab.Lookup(name) == null)
				{
					throw new InterpreterException("The variable name " + name + " has not been defined.");
				}
			}
		}

		/// <summary>
		/// Get name of identifier
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets or sets the expression defined for the variable.
		/// </summary>
		/// <value>The expression</value>
		public IExpr Expression
		{
			get
			{
				SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);
				return variable.Expression;
			}
			set
			{
				SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);
				variable.Expression = value;
			}
		}

		/// <summary>
		/// Gets the variable associated with the identifier
		/// </summary>
		public void AddValueChangeHandler(ValueChangeEventHandler handler)
		{
			Variable var = (Variable) Interpreter.SymbolTable.Lookup(_name);
			var.ValueChange += handler;
		}

		/// <summary>
		/// Gets or sets the data value for the associated variable in the symbol table.
		/// </summary>
		/// <value>The Value object</value>
		public override Value Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;

				// set the Value object to the corresponding symbol table entry
				SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);
				variable.Value = value;
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
				return ExprType.IDENT;
			}
		}

		/// <summary>
		/// Prepare the identifier in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
		}

		/// <summary>
		/// Restrict the identifier in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
		}

		/// <summary>
		/// Evaluate the identifier in the phase three of interpreting
		/// </summary>
		/// <returns>The Value object assigned to this identifier.</returns>
		public override Value Eval()
		{
			SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);

			if (variable.Value != null)
			{
				return variable.Value;
			}
			else
			{
				return variable.Expression.Eval();
			}
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Identifier with name " + _name);
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the identifier.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);
			IExpr expr = variable.Expression;

			if (expr is ITraceable)
			{
				return ((ITraceable) expr).TraceDocument();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get an enumerator of the absolute path, including the base path referenced
		/// by the context.
		/// </summary>
		/// <returns>The PathEnumerator</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			SymbolTableEntry variable = Interpreter.SymbolTable.Lookup(_name);
			IExpr expr = variable.Expression;

			if (!(expr is ITraceable))
			{
				throw new InterpreterException("The expression associated the variable is not traceable");
			}
			
			PathEnumerator enumerator = ((ITraceable) expr).GetAbsolutePathEnumerator();

			return enumerator;
		}

		#endregion
	}
}