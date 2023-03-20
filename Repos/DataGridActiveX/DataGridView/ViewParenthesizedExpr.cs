/*
* @(#)ViewParenthesizedExpr.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a parenthesized expression appeared in search filters.
	/// </summary>
	/// <version>1.0.1 29 May 2006</version>
	public class ViewParenthesizedExpr : DataGridViewElementBase
	{
		/// <summary>
		/// Definitions of parenthesied expressions
		/// </summary>
		public static string[] Operators = new string[] {"( )"};

		/// <summary>
		/// Convert an operator string to a ViewElementType
		/// </summary>
		/// <param name="op">An operator string</param>
		/// <returns>A ViewElementType</returns>
		public static ViewElementType ConvertToElementType(string op)
		{
			ViewElementType type = ViewElementType.Unknown;

			switch (op)
			{
				case "( )":
					type = ViewElementType.ParenthesizedExpr;
					break;
			}

			return type;
		}

		private IDataGridViewElement _expr;

		/// <summary>
		/// Initiating an instance of ViewParenthesizedExpr class
		/// </summary>
		public ViewParenthesizedExpr() : base()
		{
			_expr = null;
		}

		/// <summary>
		/// Initiating an instance of ViewParenthesizedExpr class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewParenthesizedExpr(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the DataGridViewModel that owns this element
		/// </summary>
		/// <value>DataGridViewModel object</value>
		public override DataGridViewModel DataGridView
		{
			get
			{
				return base.DataGridView;
			}
			set
			{
				base.DataGridView = value;
				if (_expr != null && value != null)
				{
					_expr.DataGridView = value;
				}
			}
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.ParenthesizedExpr;
			}
		}

		/// <summary>
		/// Gets or sets the internal expression
		/// </summary>
		/// <value>An IDataGridViewElement instance.</value>
		public IDataGridViewElement Expression
		{
			get
			{
				return _expr;
			}
			set
			{
				_expr = value;
				if (_expr != null && DataGridView != null)
				{
					_expr.DataGridView = DataGridView;
				}
			}
		}

		/// <summary>
		/// Add a search expression to the expression inside a parenthesis with a logical operator.
		/// If there exists an expression, the new expression is appended to the end.
		/// </summary>
		/// <param name="expr">The expression to be appended.</param>
		/// <param name="type">The logical operator type, either And or Or</param>
		public void AddSearchExpr(IDataGridViewElement expr, ViewElementType type)
		{
			IDataGridViewElement existing = _expr;

			if (existing != null)
			{
				ViewLogicalExpr logicalExpr;
				switch (type)
				{
					case ViewElementType.And:
						logicalExpr = new ViewLogicalExpr(ViewElementType.And, existing, expr);
						break;
					case ViewElementType.Or:
						logicalExpr = new ViewLogicalExpr(ViewElementType.Or, existing, expr);
						break;
					default:
						// default is And operator
						logicalExpr = new ViewLogicalExpr(ViewElementType.And, existing, expr);
						break;
				}

				_expr = logicalExpr;
			}
			else
			{
				// the first expression, set it as root
				_expr = expr;
			}

            expr.DataGridView = this.DataGridView; // pass down the dataview
		}

		/// <summary>
		/// Remove the last relational expression from the search expression.
		/// </summary>
		/// <remarks>If there doesn't exist a search expression, this method does nothing.</remarks>
		public void RemoveLastSearchExpr()
		{
			IDataGridViewElement existing = _expr;

			if (existing != null)
			{
				if (existing is ViewLogicalExpr)
				{
					// there are more than one relational expressions in the search
					// expression, remove the last one
					_expr = ((ViewBinaryExpr) existing).Left;
					((ViewBinaryExpr) existing).Left = null;
				}
				else if (existing is ViewRelationalExpr || existing is ViewInExpr ||
					existing is ViewParenthesizedExpr)
				{
					// this is only one relational expression, remove it
					_expr = null;
				}
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			if (visitor.VisitLeftParenthesis(this))
			{
				if (_expr != null)
				{
					_expr.Accept(visitor);
				}
			}

			visitor.VisitRightParenthesis(this);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of _expr member
			if (parent.ChildNodes.Count > 0)
			{
				_expr = ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_expr != null)
			{
				XmlElement child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_expr.ElementType));
				_expr.Marshal(child);
				parent.AppendChild(child);
			}
		}
	}
}