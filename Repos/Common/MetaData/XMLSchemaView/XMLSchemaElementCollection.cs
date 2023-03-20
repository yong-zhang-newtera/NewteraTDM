/*
* @(#)XMLSchemaElementCollection.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a collection of XMLSchemaElement objects.
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaElementCollection : XMLSchemaNodeCollection
	{
		private bool _isAltered;
        private IXMLSchemaNode _parentNode;

		/// <summary>
		/// Initiating an instance of XMLSchemaElementCollection class
		/// </summary>
		public XMLSchemaElementCollection() : base()
		{
			_isAltered = false;
			_xpath = null;
            _parentNode = null;

            if (GlobalSettings.Instance.IsWindowClient)
            {
                // listen to the value changed event from the data views
                this.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

        /// <summary>
		/// Initiating an instance of XMLSchemaElementCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal XMLSchemaElementCollection(XmlElement xmlElement)
            : base()
        {
            Unmarshal(xmlElement);

            _isAltered = false;
            _xpath = null;
            _parentNode = null;
        }

        /// <summary>
        /// Gets or sets the parent node of this node
        /// </summary>
        /// <value>A IXMLSchemaNode object.</value>
        public IXMLSchemaNode ParentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                _parentNode = value;
            }
        }

		/// <summary>
		/// Gets or sets the information indicating whether the data views has been altered
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
        public override XMLSchemaNodeType NodeType 
		{
			get
			{
                return XMLSchemaNodeType.XMLSchemaElements;
			}
		}

        /// <summary>
		/// Adds a XMLSchemaElement to the XMLSchemaElementCollection.
		/// </summary>
		/// <param name="value">the XMLSchemaElement to be added</param>
		/// <returns>The position into which the new element was added</returns>
        public override int Add(IXMLSchemaNode value)
        {
            int pos = 0;
            if (value is XMLSchemaElement)
            {
                pos = base.Add(value);
                ((XMLSchemaElement)value).ParentNode = this;
            }
            else
            {
                throw new ArgumentException("value must be of type XMLSchemaElement.", "value");
            }

            return pos;
        }

        /// <summary>
		/// inserts a XMLSchemaElement to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The XMLSchemaElement to insert into collection</param>
        public override void Insert(int index, IXMLSchemaNode value)
        {
            if (value is XMLSchemaElement)
            {
                base.Insert(index, value);
                ((XMLSchemaElement)value).ParentNode = this;
            }
            else
            {
                throw new ArgumentException("value must be of type XMLSchemaElement.", "value");
            }
        }

		/// <summary>
		/// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IXMLSchemaNodeVisitor visitor)
		{
			foreach (IXMLSchemaNode element in List)
			{
				element.Accept(visitor);
			}
		}

		/// <summary>
		/// Unmarshal an element representing data view collection
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			this.List.Clear();

			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				XMLSchemaElement xmlSchemaView = (XMLSchemaElement) XMLSchemaNodeFactory.Instance.Create(xmlElement);

				this.Add(xmlSchemaView);
			}
		}

        /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel)
        {
            return null;
        }

        /// <summary>
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel)
        {
            return null;
        }

		/// <summary>
		/// A handler to call when a value of the data views changed
		/// </summary>
		/// <param name="sender">the IXMLSchemaNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
            if (_xpath == null && this.Parent != null)
			{
				_xpath = this.Parent.ToXPath() + "/elements";
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
        public override IXaclObject Parent
		{
			get
			{
				return _parentNode;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}