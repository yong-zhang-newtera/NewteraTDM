/*
* @(#)ViewVirtualAttribute.cs
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
	/// A ViewVirtualAttribute instance represents a virtual attribute in a class view.
	/// It can appears in the result attribute collection only.
	/// </summary>
	/// <version>1.0.0 29 May 2007</version>
    public class ViewVirtualAttribute : ViewAttribute
	{
        private ViewEnumElement _enumElement;

		/// <summary>
		/// Initiating an instance of ViewVirtualAttribute class
		/// </summary>
		/// <param name="name">Name of the simple attribute</param>
		/// <param name="ownerClassAlias">Owner class alias</param>
		public ViewVirtualAttribute(string name, string ownerClassAlias) : this(name, ViewDataType.Unknown, ownerClassAlias)
		{
            _enumElement = null;
		}

		/// <summary>
		/// Initiating an instance of ViewVirtualAttribute class
		/// </summary>
		/// <param name="name">Name of the simple attribute</param>
		/// <param name="dataType">Data Type</param>
		/// <param name="ownerClassAlias">The class alias</param>
        public ViewVirtualAttribute(string name, ViewDataType dataType, string ownerClassAlias)
            : base(name, dataType, ownerClassAlias)
		{
            _enumElement = null;
		}

		/// <summary>
		/// Initiating an instance of ViewVirtualAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewVirtualAttribute(XmlElement xmlElement) : base()
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
				return ViewElementType.VirtualAttribute;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the attribute value is
        /// constrainted by a enum.
        /// </summary>
        public bool HasEnumConstraint
        {
            get
            {
                if (this._enumElement != null)
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
        /// Gets or sets the enum values of the attribute.
        /// </summary>
        public ViewEnumElement EnumConstraint
        {
            get
            {
                return _enumElement;
            }
            set
            {
                _enumElement = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the attribute value is
        /// a multiple choices.
        /// </summary>
        public bool IsMultipleChoice
        {
            get
            {
                if (_enumElement != null)
                {
                    return _enumElement.IsMultipleChoice;
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
			visitor.VisitVirtualAttribute(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            base.Unmarshal(parent);

            // first node may be enum element
            if (parent.ChildNodes.Count > 0)
            {
                _enumElement = (ViewEnumElement)ViewElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            }
            else
            {
                _enumElement = null;
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            if (_enumElement != null)
            {
                XmlElement child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_enumElement.ElementType));
                _enumElement.Marshal(child);
                parent.AppendChild(child);
            }
		}

		/// <summary>
		/// Clone the ViewVirtualAttribute object
		/// </summary>
		/// <returns>The cloned object</returns>
		public object Clone()
		{
			ViewVirtualAttribute cloned = null;

			// first convert the expr into xml
			XmlDocument doc = new XmlDocument();

			string elementName = Enum.GetName(typeof(ViewElementType), this.ElementType);

			XmlElement xmlElement = doc.CreateElement(elementName);

			doc.AppendChild(xmlElement);

			this.Marshal(xmlElement); // created a xml element tree

			// convert xml to a new ViewVirtualAttribute
			cloned = (ViewVirtualAttribute) ViewElementFactory.Instance.Create(xmlElement);

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