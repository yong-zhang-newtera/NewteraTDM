/*
* @(#)Path.cs
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
	/// Represents a Path expression.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	/// <remarks>Also see FLClause</remarks>
	public class Path : ExprBase, ITraceable
	{
		private IExpr _context;
		private ExprCollection _steps;
		private IExpr _sortBy;
		private VDocument _ownerDocument;

		/// <summary>
		/// Initiate an instance of Path object.
		/// </summary>
		/// <param name="steps">The steps that comprise the path.</param>
		/// <param name="conext"> The context of the path</param> 
		public Path(Interpreter interpreter, ExprCollection steps, IExpr context) : base(interpreter)
		{
			_context = context;
			_steps = steps;
			_sortBy = null;
			_ownerDocument = null;
			SetStepToPathReference(); // so that each step has a reference to its path
		}

		/// <summary>
		/// Initiate an instance of Path object.
		/// </summary>
		/// <param name="steps">The steps that comprise the path.</param>
		/// <param name="conext"> The context of the path</param> 
		public Path(Interpreter interpreter, ExprCollection steps, IExpr context, IExpr sortBy) : base(interpreter)
		{
			_context = context;
			_steps = steps;
			_sortBy = sortBy;
			_ownerDocument = null;
			SetStepToPathReference(); // so that each step has a reference to its path
		}

		/// <summary>
		/// Initiate an instance of Path object.
		/// </summary>
		/// <param name="step">A single step.</param>
		/// <param name="conext"> The context of the path</param> 
		public Path(Interpreter interpreter, Step step, IExpr context) : base(interpreter)
		{
			_context = context;
			_steps = new ExprCollection();
			_steps.Add(step);
			_sortBy = null;
			_ownerDocument = null;
			SetStepToPathReference(); // so that each step has a reference to its path
		}

		/// <summary>
		/// Gets or sets the context of the path
		/// </summary>
		/// <value>The IExpr object</value>
		public IExpr Context
		{
			get
			{
				return _context;
			}
			set
			{
				_context = value;
			}
		}

		/// <summary>
		/// Gets the step collection
		/// </summary>
		public ExprCollection Steps  
		{
			get  
			{
				return _steps;
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
				return ExprType.PATH;
			}
		}

		/// <summary>
		/// Prepare the Path expression in the phase one of interpreting
		/// </summary>
		/// <remarks>
		/// For the DocumentDB, we need to prepare the document with the path info.
		/// An absolute path is required to pass to the document for preparation.
		/// </remarks>
		public override void Prepare()
		{
			if (_context == null || !(_context is ITraceable))
			{
				throw new InterpreterException("The path do not have a context or have a wrong type of context");
			}
			
			// find the owner document first
			if (_ownerDocument == null)
			{
                Value doc = TraceDocument();
                if (doc != null)
                {
                    _ownerDocument = doc.ToNode() as VDocument;
                }
			}

			// prepare document with an absolute path
			if (_ownerDocument != null)
			{
				_ownerDocument.PrepareNodes(GetAbsolutePathEnumerator());
			}
			
			_steps.Prepare(); // This will prepare the qualifiers that may associated with the steps
		
			if (_sortBy != null)
			{
				_sortBy.Prepare();
			}
		}

		/// <summary>
		/// Restrict the Path expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			_steps.Restrict();

			if (_sortBy != null)
			{
				_sortBy.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the path expression in the phase three of interpreting to
		/// select the nodes specified by the path
		/// </summary>
		/// <returns>A XCollection contains xml nodes selected by the path. </returns>
		public override Value Eval()
		{
			// Get base node(s) by calling Eval on the context
			Value val = _context.Eval();

			if (!(val is ISelectable))
			{
				throw new InterpreterException("The context node is a wrong type");
			}

			// select nodes on the base node(s) using locale path
			return ((ISelectable) val).SelectNodes(GetLocalPathEnumerator());
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			//System.Console.WriteLine("let $" + ((Ident) _ident).Name + " in " + _expr.Eval().ToString());
		}

		/// <summary>
		/// Get an enumerator of the local path, not including the base path referenced
		/// by the context.
		/// </summary>
		/// <returns>A PathEnumerator</returns>
		public PathEnumerator GetLocalPathEnumerator()
		{
			PathEnumerator enumerator = new PathEnumerator();

			// Add the IEnumerator of local path to the path enumerator
			enumerator.Append(_steps.GetEnumerator());

			return enumerator;
		}

		/// <summary>
		/// Get an enumerator of the base path only
		/// </summary>
		/// <returns>A PathEnumerator, null if it does not exist</returns>
		public PathEnumerator GetBasePathEnumerator()
		{
			if (_context == null)
			{
				return null;
			}

			if (!(_context is ITraceable))
			{
				throw new InterpreterException("Context of a path isn't traceable. Path or some of function expressions are traceable.");
			}
			
			PathEnumerator enumerator = ((ITraceable) _context).GetAbsolutePathEnumerator();
			
			return enumerator;
		}

		/// <summary>
		/// Establish the reversed relationship from steps to the path
		/// </summary>
		private void SetStepToPathReference()
		{
			foreach (Step step in _steps)
			{
				step.OwnerPath = this;
			}
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the expression.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			if (!(_context is ITraceable))
			{
				throw new InterpreterException("Context of a path isn't traceable. Path or some of function expressions are traceable.");
			}

			return ((ITraceable) _context).TraceDocument();
		}

		/// <summary>
		/// Get an enumerator of the absolute path, including the base path referenced
		/// by the context.
		/// </summary>
		/// <returns>The PathEnumerator, null if it does not exist</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			PathEnumerator pathEnumerator;

			if (_context == null)
			{
				return null;
			}

			if (!(_context is ITraceable))
			{
				throw new InterpreterException("Context of a path isn't traceable. Path or some of function expressions are traceable.");
			}
			
			pathEnumerator = ((ITraceable) _context).GetAbsolutePathEnumerator();

			// append the local path IEnumerator to it
			pathEnumerator.Append(_steps.GetEnumerator());

			//System.Console.WriteLine(pathEnumerator.ToString());

			return pathEnumerator;
		}

		#endregion
	}
}