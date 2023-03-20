/*
* @(#)QueryExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents an identifier for variable.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class QueryExpr : ExprBase, ITraceable
	{
		private IExpr _expr;

		/// <summary>
		/// Initiate an instance of QueryExpr object.
		/// </summary>
		/// <param name="interpreter">The interpreter for getting symbol table</param>
		/// <param name="name">The sub expression</param>
		public QueryExpr(Interpreter interpreter, IExpr expr) : base(interpreter)
		{
			_expr = expr;
		}

		/// <summary>
		/// Gets an inner expression of the QueryExpr
		/// </summary>
		/// <value>An IExpr instance</value>
		public IExpr InnerExpr
		{
			get
			{
				return _expr;
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
				return ExprType.QUERY;
			}
		}

		/// <summary>
		/// Prepare the identifier in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			_expr.Prepare();
		}

		/// <summary>
		/// Restrict the identifier in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			_expr.Restrict();
		}

		/// <summary>
		/// Evaluate the identifier in the phase three of interpreting
		/// </summary>
		/// <returns>Always return XNode object referencing to a XmlDocument</returns>
		public override Value Eval()
		{
			VDocument doc = DocumentFactory.Instance.Create();
			XmlElement root = doc.CreateElement("QueryResult");

			doc.AppendChild(root);

			// set the xml document to the interpreter so that it is available to
			// other expressions in the parse tree.
			Interpreter.Document = doc;

			doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "Unicode", null), root);

			Value result = _expr.Eval();

			if (Interpreter.IsForCount)
			{
				// Hack, added by Yong Zhang to deal with count specifically
				// get the count value from member, ignore the Value.
				root.InnerText = System.Convert.ToString(Interpreter.CountValue);
			}
            else if (Interpreter.IsForClassNames)
            {
                // add class names as xml elements to the document
                XmlElement child;
                foreach (string className in Interpreter.ClassNames)
                {
                    child = doc.CreateElement("ClassName");
                    child.InnerText = className;
                    root.AppendChild(child);
                }
            }
            else
            {
                AddResultToDocRoot(doc, root, result);
            }

			return new XNode(doc);
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Query : " + _expr.ToString());
		}

		/// <summary>
		/// Add Value result as children to the document root
		/// </summary>
		/// <param name="doc">The owner document</param>
		/// <param name="root">The document root</param>
		/// <param name="val">The result value</param>
		private void AddResultToDocRoot(VDocument doc, XmlElement root, Value result)
		{
			if (result.DataType.IsXmlNode)
			{
				XmlNode child;

				if (result.ToNode().OwnerDocument != doc)
				{
					child = doc.ImportNode(result.ToNode(), true);
				}
				else
				{
					child = result.ToNode();
				}

				root.AppendChild(child);
			}
			else if (result.DataType.IsCollection)
			{
				foreach (Value val in result.ToCollection())
				{
					AddResultToDocRoot(doc, root, val);
				}
			}
			else
			{
				// it is a Value type other than XNode and XCollection
				root.InnerText = result.ToString();
			}
		}

		#region ITraceable ≥…‘±

		public Value TraceDocument()
		{
			if (this._expr is ITraceable)
			{
				return ((ITraceable) _expr).TraceDocument();
			}
			else
			{
				return null;
			}
		}

		public PathEnumerator GetAbsolutePathEnumerator()
		{
			throw new InterpreterException("Invalid method call");
		}

		#endregion
	}
}