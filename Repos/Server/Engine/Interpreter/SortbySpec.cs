/*
* @(#)SortbySpec.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents a SortbySpec expression which is part of a Sortby clause.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class SortbySpec : ExprBase
	{
		private Path _path;
		private bool _isAscending; // gives the direction of the sort

		/// <summary>
		/// Initiate an instance of SortbySpec object.
		/// </summary>
		/// <param name="path">The path specifying a sortby field</param>
		/// <param name="isAscending">true if sort direction is ascending, false otherwise.</param> 
		public SortbySpec(Interpreter interpreter, IExpr path, bool isAscending) : base(interpreter)
		{
			_path = (Path) path;
			_isAscending = isAscending;
		}

		/// <summary>
		/// Gets the information indicating whether the sort direction is ascending
		/// </summary>
		/// <value>true if the sort direction is ascending, false for the descending direction.</value>
		public bool IsAscending
		{
			get
			{
				return _isAscending;
			}
		}

		public Path SortPath
		{
			get
			{
				return _path;
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
				return ExprType.SORTBYFIELD;
			}
		}

		/// <summary>
		/// Prepare the For expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_path != null)
			{
				_path.Prepare();
			}

			// prepare the sortby spec
            Value val = _path.TraceDocument();
            if (val != null)
            {
                VDocument doc = val.ToNode() as VDocument;
                if (doc != null)
                {
                    doc.PrepareSortBy(this);
                }
            }
		}

		/// <summary>
		/// Restrict the For expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_path != null)
			{
				_path.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the sortbyField expression in the phase three of interpretin
		/// </summary>
		/// <returns></returns>
		public override Value Eval()
		{
			// Not supposed to be called, throw an exception
			throw new InvalidOperationException("Eval on SortbySpec expression is invalid");
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine(_path.ToString() + (_isAscending? "ASCENDING" : "DESCENDING"));
		}
	}
}