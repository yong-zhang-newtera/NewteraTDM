/*
* @(#)Step.cs
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
	/// Holds an expression referring to a single segment in a path. This
	/// includes an axis type, a name, and optionally a qualifier.
	/// </summary>
	/// <version>  1.0 17 Aug 2003</version>
	/// <author> Yong Zhang</author>
	public class Step :  ExprBase, ITraceable
	{
		private string _name;
		private string _prefix;
		private int _code; // code for axis type, see Axis.cs
		private IExpr _qualifier;
		private PathEnumerator _absPathEnumerator; // contains steps from root to this step
		private Path _ownerPath; // The path to which the step belongs

		/// <summary>
		/// Initiating an instance of Step class.
		/// </summary>
		/// <param name="axis">Axis string </param>
		/// <param name="name">Step name </param>
		/// <param name="prefix">The prefix of the step name</param>
		public Step(Interpreter interpreter, string name, string prefix, string axis) : this(interpreter, name, prefix, axis, null)
		{
		}
		
		/// <summary>
		/// Initiating an instance of Step class.
		/// </summary>
		/// <param name="axis">Axis string </param>
		/// <param name="name">Step name </param>
		/// <param name="prefix">The prefix of the step name</param>
		public Step(Interpreter interpreter, string name, string prefix, string axis, IExpr qualifier) : base(interpreter)
		{
			_name = name;
			_prefix = prefix;
			_code = Axis.GetCode(axis);
			_qualifier = qualifier;
			_absPathEnumerator = null;
			_ownerPath = null;
		}

		/// <summary>
		/// Initiating an instance of Step class.
		/// </summary>
		/// <param name="name">Step name </param>
		/// <param name="prefix">The prefix of the step name</param> 
		/// <param name="axisCode">Axis code </param>
		public Step(Interpreter interpreter, string name, string prefix, int axisCode) : this(interpreter, name, prefix, axisCode, null)
		{
		}

		/// <summary>
		/// Initiating an instance of Step class.
		/// </summary>
		/// <param name="name">Step name </param>
		/// <param name="axisCode">Axis code </param>
		/// <param name="prefix">The prefix of the step name</param>
		/// <param name="qualifier">The step qualifier</param>
		public Step(Interpreter interpreter, string name, string prefix, int axisCode, IExpr qualifier) : base(interpreter)
		{
			_name = name;
			_prefix = prefix;
			_code = axisCode;
			_qualifier = qualifier;
			_absPathEnumerator = null;
			_ownerPath = null;
		}

		/// <summary>
		/// Gets the axis code.
		/// </summary>
		/// <returns> int code for the axis type (see Axis.cs)</returns>
		public int AxisCode
		{
			get
			{
				return _code;
			}
		}

		/// <summary>
		/// Gets the step name.
		/// </summary>
		/// <returns> step name.</returns>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		/// <summary>
		/// Gets the step prefix.
		/// </summary>
		/// <returns> step prefix.</returns>
		public string Prefix
		{
			get
			{
				return _prefix;
			}
		}

		/// <summary>
		/// Gets or sets the qualifier.
		/// </summary>
		/// <returns> The qualifier expr</returns>
		public IExpr Qualifier
		{
			get
			{
				return _qualifier;
			}
			set
			{
				_qualifier = value;
			}
		}

		/// <summary>
		/// Gets or sets the owner path.
		/// </summary>
		/// <returns> The path expr</returns>
		public Path OwnerPath
		{
			get
			{
				return _ownerPath;
			}
			set
			{
				_ownerPath = value;
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
				return ExprType.STEP;
			}
		}

		/// <summary>
		/// Gets or sets the base path enumerator that contains steps from root to this step.
		/// </summary>
		/// <returns> A base path enumerator</returns>
		public PathEnumerator BasePathEnumerator
		{
			get
			{
				return _absPathEnumerator;
			}
			set
			{
				_absPathEnumerator = value;
			}
		}

		/// <summary>
		/// Prepare the step expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_qualifier != null)
			{
				// TODO, set the base path enumerator as an context
				_qualifier.Prepare();
			}
		}

		/// <summary>
		/// If the step has a qualifier, invoke the PrepareQualifier method on the
		/// owner document so that DocumentDB instance has a chance to create corresponding
		/// SQL conditions in the generated SQL.
		/// </summary>
		public override void Restrict()
		{
			if (_qualifier != null)
			{
                Value val = this.TraceDocument();
                if (val != null)
                {
                    VDocument doc = val.ToNode() as VDocument;
                    if (doc != null)
                    {
                        doc.PrepareQualifier(_qualifier);
                    }
                }
			}
		}

		/// <summary>
		/// Evaluate the step expression in the phase three of interpreting.
		/// The step expression is not supposed to be evaluated.
		/// </summary>
		/// <returns>A null</returns>
		public override Value Eval()
		{
			throw new InvalidOperationException("Eval is invalid on Step");
		}

		/// <summary>
		/// This is used to let VDOM insert access control conditions into
		/// a query. It is called during the PREPARE phase so the
		/// qualifiers are set properly when they are prepared during
		/// RESTRICT.
		///
		/// It parses the condition String by putting it into a fake path
		/// (to fool the parser into thinking it has a context), and
		/// retrieves the qualifier from that path. If the Step has no
		/// qualifier it just inserts it, otherwise it replaces the current
		/// qualifier with an AND node combining the old and new
		/// qualifiers.
		/// </summary>
		/// <param name="condition">String containing a condition to add to the step qualifier.</param>
		/// <param name="option">the option tells how the qualifier should be inserted</param>
		public void InsertQualifier(string condition, ConditionInsertOption option)
		{
			QueryExpr queryExpr = (QueryExpr) Interpreter.Parse(condition, this);

			// The inner expression is the parsed expression for the condition
			IExpr parsed = queryExpr.InnerExpr;

			switch (option)
			{
				case ConditionInsertOption.AlwaysTrue: 
					parsed = new UTrue(Interpreter, parsed);
					_qualifier = (_qualifier == null)? parsed : new And(Interpreter, _qualifier, parsed);
					break;
				
				case ConditionInsertOption.Excluding: 
					parsed = new Not(Interpreter, parsed);
					_qualifier = (_qualifier == null)? parsed : new And(Interpreter, _qualifier, parsed);
					break;
				
				case ConditionInsertOption.Including:
                    // add parenthesis around the parsed expression since it may contains OR operator
                    parsed = new ParenthesizedExpr(Interpreter, parsed);
					_qualifier = (_qualifier == null)? parsed : new And(Interpreter, _qualifier, parsed);
					break;
			}
		}
		
		/// <summary>
		/// Print the information about the step for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine(Axis.GetType(_code) + _name + (_qualifier != null? _qualifier.ToString() : ""));
		}

		/// <summary>
		/// Gets string representation of the step
		/// </summary>
		/// <returns>a string representing step</returns>
		public override string ToString()
		{
			return Axis.GetType(_code) + _name + (_qualifier != null? "[" + _qualifier.ToString() + "]" : "");
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the step.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			if (_ownerPath != null)
			{
				return _ownerPath.TraceDocument();
			}
			else
			{
				throw new InterpreterException("The step " + this._name + " is not traceable because the owner path is missing..");
			}
		}

		/// <summary>
		/// Get an enumerator of the absolute path, from the root to this step
		/// </summary>
		/// <returns>The PathEnumerator, null if it does not exist</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			PathEnumerator pathEnumerator = null;

			if (_ownerPath != null)
			{
				pathEnumerator = _ownerPath.GetBasePathEnumerator();
				IList steps = new ArrayList();
				foreach (Step step in _ownerPath.Steps)
				{
					// add steps up to this step
					steps.Add(step);
					if (step == this)
					{
						break;
					}
				}

				pathEnumerator.Append(steps.GetEnumerator());
			}
			else
			{
				throw new InterpreterException("Unable to get the absolute path for the step because the owner path is missing.");
			}

			return pathEnumerator;
		}

		#endregion
	}

	/// <summary>
	/// Option used when inserting a condition into a step
	/// </summary>
	public enum ConditionInsertOption
	{
		AlwaysTrue,
		Excluding,
		Including
	}
}