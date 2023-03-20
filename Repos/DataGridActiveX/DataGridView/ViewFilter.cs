/*
* @(#)ViewFilter.cs
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
	/// Represents a filter in query.
	/// </summary>
	/// <version>1.0.1 29 May 2006</version>
	public class ViewFilter : DataGridViewElementBase
	{
		private IDataGridViewElement _expr;

		/// <summary>
		/// Initiating an instance of ViewFilter class
		/// </summary>
		public ViewFilter() : base()
		{
			_expr = null;
		}

		/// <summary>
		/// Initiating an instance of ViewFilter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewFilter(XmlElement xmlElement) : base()
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
				return ViewElementType.Filter;
			}
		}

		/// <summary>
		/// Gets or sets the internal expression of the filter
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
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			if (visitor.VisitFilter(this))
			{
				if (_expr != null)
				{
					_expr.Accept(visitor);
				}
			}
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