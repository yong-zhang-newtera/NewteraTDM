/*
* @(#)ViewRelationshipAttribute.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Collections.Specialized;
	using System.Xml;

	/// <summary>
	/// A ViewRelationshipAttribute instance represents a relationship attribute in a class view.
	/// It can appears in the result attribute collection or search expression.
	/// </summary>
	/// <version>1.0.0 29 Oct 2007</version>
    public class ViewRelationshipAttribute : ViewAttribute
	{
        private bool _isForeignKeyRequired;

		/// <summary>
		/// Initiating an instance of ViewRelationshipAttribute class
		/// </summary>
		/// <param name="name">Name of the simple attribute</param>
		/// <param name="ownerClassAlias">Owner class alias</param>
		public ViewRelationshipAttribute(string name, string ownerClassAlias) : base(name, ViewDataType.String, ownerClassAlias)
		{
            _isForeignKeyRequired = false;
		}

		/// <summary>
		/// Initiating an instance of ViewRelationshipAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewRelationshipAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType 
		{
			get
			{
				return ViewElementType.RelationshipAttribute;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a foreign key is required for the 
        /// relationship attribute.
		/// </summary>
		/// <value>true if a foreign key is required, false otherwise.</value>
		public bool IsForeignKeyRequired
		{
			get
			{				
				return _isForeignKeyRequired;
			}
            set
            {
                _isForeignKeyRequired = value;
            }
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			visitor.VisitRelationshipAttribute(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // set value of _value member
            string val = parent.GetAttribute("fk");
            if (!string.IsNullOrEmpty(val))
            {
                _isForeignKeyRequired = bool.Parse(val);
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("fk", _isForeignKeyRequired.ToString());
		}

		/// <summary>
		/// Clone the ViewRelationshipAttribute object
		/// </summary>
		/// <returns>The cloned object</returns>
		public object Clone()
		{
			ViewRelationshipAttribute cloned = null;

			// first convert the expr into xml
			XmlDocument doc = new XmlDocument();

			string elementName = Enum.GetName(typeof(ViewElementType), this.ElementType);

			XmlElement xmlElement = doc.CreateElement(elementName);

			doc.AppendChild(xmlElement);

			this.Marshal(xmlElement); // created a xml element tree

			// convert xml to a new ViewRelationshipAttribute
			cloned = (ViewRelationshipAttribute) ViewElementFactory.Instance.Create(xmlElement);

			return cloned;
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

	}
}