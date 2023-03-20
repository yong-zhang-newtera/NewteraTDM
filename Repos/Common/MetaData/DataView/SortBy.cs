/*
* @(#)SortBy.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a SortBy in a query.
	/// </summary>
	/// <version>1.0.1 26 Sep 2006</version>
	/// <author>Yong Zhang</author>
	public class SortBy : DataViewElementBase, IQueryElement
	{
		private ResultAttributeCollection _sortAttributes;

		/// <summary>
		/// Initiating an instance of SortBy class
		/// </summary>
		public SortBy() : base()
		{
			_sortAttributes = new ResultAttributeCollection();
		}

		/// <summary>
		/// Initiating an instance of SortBy class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SortBy(XmlElement xmlElement) : base()
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
				_sortAttributes.DataView = value;
			}
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.SortBy;
			}
		}

		/// <summary>
		/// Gets sort attributes
		/// </summary>
		/// <value>A collection of the attributes</value>
		public ResultAttributeCollection SortAttributes
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
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

			// the first node represents a collection of sort attributes
			_sortAttributes = (ResultAttributeCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _sortAttributes
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_sortAttributes.ElementType));
			_sortAttributes.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("sortby (");

			int index = 0;
			foreach (IQueryElement child in _sortAttributes)
			{
				if (index > 0)
				{
					query.Append(", ");	
				}

				query.Append(child.ToXQuery());

				index++;
			}

			query.Append(")");

			return query.ToString();
		}
	}
}