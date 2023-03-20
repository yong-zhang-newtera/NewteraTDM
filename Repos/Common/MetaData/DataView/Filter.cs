/*
* @(#)Filter.cs
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
	/// Represents a filter in query.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class Filter : DataViewElementBase
	{
		private IDataViewElement _expr;

		/// <summary>
		/// Initiating an instance of Filter class
		/// </summary>
		public Filter() : base()
		{
			_expr = null;
		}

		/// <summary>
		/// Initiating an instance of Filter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Filter(XmlElement xmlElement) : base()
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
				return ElementType.Filter;
			}
		}

		/// <summary>
		/// Gets or sets the internal expression of the filter
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
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
				_expr = ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (_expr != null)
                    {
                        _expr.ValueChanged += new EventHandler(this.ValueChangedHandler);
                    }
                }
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
	}
}