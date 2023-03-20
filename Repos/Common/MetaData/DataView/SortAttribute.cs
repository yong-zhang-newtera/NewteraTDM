/*
* @(#)SortAttribute.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a SortAttribute in a SortBy clause.
	/// </summary>
	/// <version>1.0.1 10 Aug 2007</version>
	public class SortAttribute : DataViewElementBase, IQueryElement
	{
		private SortDirection _sortDirection = SortDirection.Ascending;
        private string _ownerClassAlias;

		/// <summary>
		/// Initiating an instance of SortAttribute class
		/// </summary>
        public SortAttribute(string name, string ownerClassAlias) : base(name)
		{
            _ownerClassAlias = ownerClassAlias;
		}

		/// <summary>
		/// Initiating an instance of SortAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SortAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.SortAttribute;
			}
		}

        /// <summary>
        /// Gets alias of class that owns this attribute
        /// </summary>
        /// <value>Owner class alias</value>
        public string OwnerClassAlias
        {
            get
            {
                return _ownerClassAlias;
            }
        }

		/// <summary>
		/// Gets or sets the sort direction
		/// </summary>
		/// <value>One of SortDirection enum</value>
		public SortDirection SortDirection
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
            visitor.VisitSortAttribute(this);
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

			_sortDirection = (SortDirection) Enum.Parse(typeof(SortDirection), directionStr);

            // set value of _ownerClassAlias member
            _ownerClassAlias = parent.GetAttribute("OwnerAlias");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the sort direction member
			string directionStr = Enum.GetName(typeof(SortDirection), _sortDirection);
			parent.SetAttribute("Direction", directionStr);

            // write the _ownerName member
            parent.SetAttribute("OwnerAlias", _ownerClassAlias);
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

            query.Append("$").Append(_ownerClassAlias).Append("/").Append(Name);

			if (_sortDirection == SortDirection.Descending)
			{
				// ascending is the default behavious
				query.Append(" descending");
			}

			return query.ToString();
		}
	}
}