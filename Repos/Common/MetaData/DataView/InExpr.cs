/*
* @(#)InExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a In expression (In/NotIn)
	/// </summary>
	/// <version> 1.0.0 14 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class InExpr : BinaryExpr
	{
		/// <summary>
		/// Operators of In expression
		/// </summary>
		public static string[] Operators = new string[] {"in", "not in"};

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
				case "in":
					type = ElementType.In;
					break;
				case "not in":
					type = ElementType.NotIn;
					break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of InExpr object.
		/// </summary>
		public InExpr(ElementType type) : base()
		{
			if (type == ElementType.In || type == ElementType.NotIn)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of InExpr object.
		/// </summary>
		public InExpr(ElementType type, IDataViewElement left, IDataViewElement right) : base(left, right)
		{
			if (type == ElementType.In || type == ElementType.NotIn)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of InExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal InExpr(XmlElement parent) : base(parent)
		{
		}

        /// <summary>
        /// Gets the information indicating whether the binary expression has a value(s) as
        /// the right expression of the operator.
        /// </summary>
        public override bool HasValue
        {
            get
            {
                bool status = false;

                switch (ElementType)
                {
                    case ElementType.In:
                    case ElementType.NotIn:
                        IParameter parameter = this.Right as IParameter;
                        if (parameter != null && parameter.HasValue)
                        {
                            status = true;
                        }
                        break;
                }

                return status;
            }
        }

		/// <summary>
		/// Gets the XQuery operator.
		/// </summary>
		/// <value>An XQuery operator</value>
		public override string Operator
		{
			get
			{
				string op = "";

				switch (_type)
				{
					case ElementType.In:
						op = InExpr.Operators[0];
						break;
					case ElementType.NotIn:
						op = InExpr.Operators[1];
						break;
				}

				return op;
			}
			set
			{
				_type = InExpr.ConvertToElementType(value);

				// fire an event for operator change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			string xquery = null;

            if (SubstituteExpression == null)
            {
                string leftExpr = null;
                string rightExpr = null;
                IQueryElement leftElement = _left as IQueryElement;
                IQueryElement rightElement = _right as IQueryElement;

                if (leftElement != null)
                {
                    leftExpr = leftElement.ToXQuery();
                }

                if (rightElement != null)
                {
                    rightExpr = rightElement.ToXQuery();
                }

                if (leftExpr != null && rightExpr != null)
                {
                    StringBuilder query = new StringBuilder();

                    string op = "in";
                    switch (this.ElementType)
                    {
                        case ElementType.In:
                            op = "in"; // in
                            break;
                        case ElementType.NotIn:
                            op = "ni"; // not in
                            break;
                    }

                    query.Append(leftExpr).Append(" ").Append(op).Append(" ").Append(rightExpr);

                    xquery = query.ToString();
                }
            }
            else
            {
                IQueryElement queryElement = SubstituteExpression as IQueryElement;
                if (queryElement != null)
                {
                    xquery = queryElement.ToXQuery();
                }
            }

			return xquery;
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return " " + this.Operator + " ";
		}
	}
}