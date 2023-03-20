/*
* @(#)ViewBinaryExpr.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a base class for all binary expressions in search filter 
	/// which has left and right operands.
	/// </summary>
	/// <version> 1.0.0 28 May 2006 </version>
	///
	public abstract class ViewBinaryExpr : DataGridViewElementBase
	{
		internal IDataGridViewElement _left = null;
		internal IDataGridViewElement _right = null;
		internal ViewElementType _type;

		/// <summary>
		/// Initiate an instance of ViewBinaryExpr class.
		/// </summary>
		public ViewBinaryExpr() : base()
		{
			_left = null;
			_right = null;
		}

		/// <summary>
		/// Initiate an instance of ViewBinaryExpr class.
		/// </summary>
		public ViewBinaryExpr(IDataGridViewElement left, IDataGridViewElement right) : base()
		{
			_left = left;
			_right = right;
		}

		/// <summary>
		/// Initiate an instance of ViewBinaryExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ViewBinaryExpr(XmlElement parent) : base()
		{
			Unmarshal(parent);
		}

		/// <summary>
		/// Gets or sets the ClassView that owns this element
		/// </summary>
		/// <value>ClassView object</value>
		public override DataGridViewModel DataGridView
		{
			get
			{
				return base.DataGridView;
			}
			set
			{
				base.DataGridView = value;
				if (_left != null && value != null)
				{
					_left.DataGridView = value;
				}

				if (_right != null && value != null)
				{
					_right.DataGridView = value;
				}
			}
		}

		/// <summary>
		/// Gets the element type
		/// </summary>
		/// <value>One of the ViewElementType values</value>
		public override ViewElementType ElementType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the left operand of the binary expression
		/// </summary>
		public IDataGridViewElement Left
		{
			get
			{				
				return _left;
			}
			set
			{
				_left = value;

				if (_left != null && DataGridView != null)
				{
					_left.DataGridView = DataGridView;
				}
			}
		}

		/// <summary>
		/// Gets the right operand of the binary expression
		/// </summary>
		public IDataGridViewElement Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;

				if (_right != null && DataGridView != null)
				{
					_right.DataGridView = DataGridView;
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
			if (_left != null)
			{
				_left.Accept(visitor);
			}

			visitor.VisitBinaryExpr(this);

			if (_right != null)
			{
				_right.Accept(visitor);
			}
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set the type memember
            string typeStr = parent.GetAttribute("eType");
            if (string.IsNullOrEmpty(typeStr))
            {
                // XmlElement is generated by DataViewModel where Name is the type
                typeStr = parent.Name;
            }
            _type = (ViewElementType)Enum.Parse(typeof(ViewElementType), typeStr);

			// the first child element is left operand element
			_left = ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);

			// The second child element is right operand element
			_right = ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			XmlElement child;
			base.Marshal(parent);

			parent.SetAttribute("eType", Enum.GetName(typeof(ViewElementType), _type));

			if (_left != null)
			{
				// write the _left operand member
				child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_left.ElementType));
				_left.Marshal(child);
				parent.AppendChild(child);
			}

			if (_right != null)
			{
				// write the _right operand member
				child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_right.ElementType));
				_right.Marshal(child);
				parent.AppendChild(child);
			}
		}

		/// <summary>
		/// Gets or sets the binary expression's operator.
		/// </summary>
		/// <value>An operator symbol</value>
		public abstract string Operator {get; set;}
	}
}