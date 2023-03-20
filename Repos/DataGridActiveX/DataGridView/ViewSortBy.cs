/*
* @(#)ViewSortBy.cs
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
	/// Represents a ViewSortBy in a query.
	/// </summary>
	/// <version>1.0.1 26 Sep 2006</version>
	/// <author>Yong Zhang</author>
	public class ViewSortBy : DataGridViewElementBase
	{
		private ViewSortDirection _sortDirection = ViewSortDirection.Ascending;
		private ViewAttributeCollection _sortAttributes;

		/// <summary>
		/// Initiating an instance of ViewSortBy class
		/// </summary>
		public ViewSortBy() : base()
		{
			_sortAttributes = new ViewAttributeCollection();
		}

		/// <summary>
		/// Initiating an instance of ViewSortBy class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewSortBy(XmlElement xmlElement) : base()
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
				_sortAttributes.DataGridView = value;
			}
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.SortBy;
			}
		}

		/// <summary>
		/// Gets or sets the sort direction
		/// </summary>
		/// <value>One of ViewSortDirection enum</value>
		public ViewSortDirection SortDirection
		{
			get
			{
				return _sortDirection;
			}
			set
			{
				_sortDirection = value;
			}
		}

		/// <summary>
		/// Gets sort attributes
		/// </summary>
		/// <value>A collection of the attributes</value>
		public ViewAttributeCollection SortAttributes
		{
			get
			{
				return _sortAttributes;
			}
		}

		/// <summary>
		/// Gets the information indicating whether there are sort by attributes
		/// </summary>
		public bool HasSortBy
		{
			get
			{
				if (_sortAttributes.Count > 0)
				{
					return true;
				}
				else
				{
					return false;
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
			if (visitor.VisitSortBy(this))
			{
				_sortAttributes.Accept(visitor);
			}
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of sort direction member
			string directionStr = parent.GetAttribute("Direction");

			_sortDirection = (ViewSortDirection) Enum.Parse(typeof(ViewSortDirection), directionStr);

			// the first node represents a collection of sort attributes
			_sortAttributes = (ViewAttributeCollection) ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the sort direction member
			string directionStr = Enum.GetName(typeof(ViewSortDirection), _sortDirection);
			parent.SetAttribute("Direction", directionStr);

			// write the _sortAttributes
			XmlElement child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_sortAttributes.ElementType));
			_sortAttributes.Marshal(child);
			parent.AppendChild(child);
		}
	}

	/// <summary>
	/// Sort Direction enum
	/// </summary>
	public enum ViewSortDirection
	{
		/// <summary>
		/// Ascending order
		/// </summary>
		Ascending,
		/// <summary>
		/// Descending order
		/// </summary>
		Descending
	}
}