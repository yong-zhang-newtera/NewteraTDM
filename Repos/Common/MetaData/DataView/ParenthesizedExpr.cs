/*
* @(#)ParenthesizedExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a parenthesized expression appeared in search filters.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class ParenthesizedExpr : DataViewElementBase, IQueryElement
	{
		/// <summary>
		/// Definitions of parenthesied expressions
		/// </summary>
		public static string[] Operators = new string[] {"( )"};

		/// <summary>
		/// Convert an operator string to a ElementType
		/// </summary>
		/// <param name="op">An operator string</param>
		/// <returns>A ElementType</returns>
		public static ElementType ConvertToElementType(string op)
		{
			ElementType type = ElementType.Unknown;

			switch (op)
			{
				case "( )":
					type = ElementType.ParenthesizedExpr;
					break;
			}

			return type;
		}

		private IDataViewElement _expr;

		/// <summary>
		/// Initiating an instance of ParenthesizedExpr class
		/// </summary>
		public ParenthesizedExpr() : base()
		{
			_expr = null;
		}

		/// <summary>
		/// Initiating an instance of ParenthesizedExpr class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ParenthesizedExpr(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		public override DataViewModel DataView
		{
			get
			{
				return base.DataView;
			}
			set
			{
				base.DataView = value;
				if (_expr != null && value != null)
				{
					_expr.DataView = value;
				}
			}
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.ParenthesizedExpr;
			}
		}

		/// <summary>
		/// Gets or sets the internal expression
		/// </summary>
		/// <value>An IDataViewElement instance.</value>
		public IDataViewElement Expression
		{
			get
			{
				return _expr;
			}
			set
			{
				_expr = value;
				if (_expr != null && DataView != null)
				{
					_expr.DataView = DataView;
				}
			}
		}

		/// <summary>
		/// Add a search expression to the expression inside a parenthesis with a logical operator.
		/// If there exists an expression, the new expression is appended to the end.
		/// </summary>
		/// <param name="expr">The expression to be appended.</param>
		/// <param name="type">The logical operator type, either And or Or</param>
		public void AddSearchExpr(IDataViewElement expr, ElementType type)
		{
			IDataViewElement existing = _expr;

			if (existing != null)
			{
				LogicalExpr logicalExpr;
				switch (type)
				{
					case ElementType.And:
						logicalExpr = new LogicalExpr(ElementType.And, existing, expr);
						break;
					case ElementType.Or:
						logicalExpr = new LogicalExpr(ElementType.Or, existing, expr);
						break;
					default:
						// default is And operator
						logicalExpr = new LogicalExpr(ElementType.And, existing, expr);
						break;
				}

				_expr = logicalExpr;
			}
			else
			{
				// the first expression, set it as root
				_expr = expr;
			}

            expr.DataView = this.DataView;

			// fire an event for adding a new expression
			FireValueChangedEvent(expr);
		}

		/// <summary>
		/// Remove the last relational expression from the search expression.
		/// </summary>
		/// <remarks>If there doesn't exist a search expression, this method does nothing.</remarks>
		public void RemoveLastSearchExpr()
		{
			IDataViewElement existing = _expr;

			if (existing != null)
			{
				if (existing is LogicalExpr)
				{
					// there are more than one relational expressions in the search
					// expression, remove the last one
					_expr = ((BinaryExpr) existing).Left;
					((BinaryExpr) existing).Left = null;
				}
				else if (existing is RelationalExpr || existing is InExpr ||
					existing is ParenthesizedExpr)
				{
					// this is only one relational expression, remove it
					_expr = null;
				}

				// fire an event for operator change
				FireValueChangedEvent(existing);
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
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
				_expr = ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
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
				XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_expr.ElementType));
				_expr.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			string xquery = null;
			IQueryElement expr = _expr as IQueryElement;

			if (expr != null)
			{
				xquery = expr.ToXQuery();
			}

			if (xquery != null)
			{
				StringBuilder query = new StringBuilder();

				query.Append("(").Append(xquery).Append(")");

				xquery = query.ToString();
			}

			return xquery;
		}

		#endregion
	}
}