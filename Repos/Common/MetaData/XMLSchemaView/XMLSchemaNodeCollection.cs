/*
* @(#)XMLSchemaNodeCollection.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;
    using System.ComponentModel;

	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// An object collection class to handle IXMLSchemaNode when collections are
	/// returned from method calls.
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
	public abstract class XMLSchemaNodeCollection : CollectionBase, IXMLSchemaNode
	{
		private bool _isChanged; // run-time use only
        internal string _xpath = null; // run-time member
        private IXMLSchemaNode _parentNode; // run-time use

		private XMLSchemaModel _xmlSchemaView;

		/// <summary>
		/// Value changed event habdler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption changed event handler
		/// </summary>
		public event EventHandler CaptionChanged;

		/// <summary>
		///  Initializes a new instance of the XMLSchemaNodeCollection class.
		/// </summary>
		public XMLSchemaNodeCollection() : base()
		{
            _parentNode = null;
		}

		/// <summary>
		/// Initiating an instance of XMLSchemaNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XMLSchemaNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
            _parentNode = null;
		}

		/// <summary>
		/// Gets or sets the XMLSchemaModel that owns this element
		/// </summary>
		/// <value>XMLSchemaModel object</value>
		public virtual XMLSchemaModel XMLSchemaView
		{
			get
			{
				return _xmlSchemaView;
			}
			set
			{
				_xmlSchemaView = value;
				if (value != null)
				{
					foreach (IXMLSchemaNode element in this.List)
					{
						element.XMLSchemaView = value;
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets the data view element that is the parent element in expression tree
        /// </summary>
        /// <value>The data view element</value>
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
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public IXMLSchemaNode this[int index]  
		{
			get  
			{
				return( (IXMLSchemaNode) List[index] );
			}
			set  
			{
				// Raise the event for value change
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("XMLSchemaNodeCollection", value));
                    }
                }

				if (XMLSchemaView != null)
				{
					value.XMLSchemaView = XMLSchemaView;
				}
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    value.ValueChanged += new EventHandler(ValueChangedHandler);
                }

				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public virtual IXMLSchemaNode this[string name]  
		{
			get  
			{
				IXMLSchemaNode found = null;

				foreach (IXMLSchemaNode element in List)
				{
					if (element.Name == name)
					{
						found = element;
						break;
					}
				}

				return found;
			}
			set  
			{
			}
		}

		/// <summary>
		/// Adds an IXMLSchemaNode to the XMLSchemaNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public virtual int Add(IXMLSchemaNode value )  
		{
			// Raise the event for value change
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("XMLSchemaNodeCollection", value));
                }
            }

			if (XMLSchemaView != null)
			{
				value.XMLSchemaView = XMLSchemaView;
			}
            if (GlobalSettings.Instance.IsWindowClient)
            {
                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			// add the element based on its position
			int index = 0;
			foreach (IXMLSchemaNode element in List)
			{
				// the collection is sorted in acsending order
				if (value.Position < element.Position)
				{
					// found a place in the list
					break;
				}
				else
				{
					index++;
				}
			}

			if (index < List.Count)
			{
				List.Insert(index, value);
			}
			else
			{
				// append at the end
				List.Add(value);
			}

			return index;
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IXMLSchemaNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public virtual void Insert(int index, IXMLSchemaNode value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                // Raise the event for value change
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("XMLSchemaNodeCollection", value));
                }
            }

			if (XMLSchemaView != null)
			{
				value.XMLSchemaView = XMLSchemaView;
			}
            if (GlobalSettings.Instance.IsWindowClient)
            {
                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IXMLSchemaNode value )  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("XMLSchemaNodeCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IXMLSchemaNode value )  
		{
			// If value is not of type IXMLSchemaNode, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Sort the elements in the collection in acsending order based on
		/// their position values
		/// </summary>
		public void Sort()
		{
			// using the simple bubble sort given that the collection size
			// is usually very small
			int i;
			int j;
			int size = List.Count;
			IXMLSchemaNode first, second;

			if (size > 1)
			{
				for ( i = (size - 1); i >= 0; i-- )
				{
					for ( j = 1; j <= i; j++ )
					{
						first = (IXMLSchemaNode) List[j-1];
						second = (IXMLSchemaNode) List[j];
						if (first.Position > first.Position)
						{
							// swap two element
							List.RemoveAt(j-1);
							List.RemoveAt(j);
							List.Insert(j-1, second);
							List.Insert(j, first);
						}
					}
				}
			}
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is IXMLSchemaNode))
			{
				throw new ArgumentException( "value must be of type IXMLSchemaNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IXMLSchemaNode))
			{
				throw new ArgumentException( "value must be of type IXMLSchemaNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (!(newValue is IXMLSchemaNode))
			{
				throw new ArgumentException( "newValue must be of type IXMLSchemaNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IXMLSchemaNode))
			{
				throw new ArgumentException( "value must be of type IXMLSchemaNode." );
			}
		}

		/// <summary>
		/// Handler for Value Changed event fired by elements of a collection
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected virtual void ValueChangedHandler(object sender, EventArgs e)
		{
			// propagate the event
			if (ValueChanged != null)
			{
				ValueChanged(sender, e);
			}
		}

               /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public abstract System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel);

        /// <summary>
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public abstract System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel);

		#region IXMLSchemaNode members

		/// <summary>
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		public bool IsValueChanged
		{
			get
			{
				return _isChanged;
			}
			set
			{
				_isChanged = value;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of ElementType values</value>
        public virtual XMLSchemaNodeType NodeType
        {
			get
			{
                return XMLSchemaNodeType.Collection;
			}
		}

		/// <summary>
		/// Gets the name of element
		/// </summary>
		/// <value>The collection name, default is null</value>
		public string Name
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the caption of element
		/// </summary>
		/// <value>The element caption.</value>
		public string Caption
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets position of this element among its sibling.
		/// </summary>
		/// <value>A zero-based integer representing the position.</value>
		public int Position
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		
		/// <summary>
		/// Gets or sets the description of element
		/// </summary>
		/// <value>The element description.</value>
		public string Description
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public virtual void Accept(IXMLSchemaNodeVisitor visitor)
		{
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IXMLSchemaNode element = XMLSchemaNodeFactory.Instance.Create(xmlElement);

				this.Add(element);
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (IXMLSchemaNode element in List)
			{
                child = parent.OwnerDocument.CreateElement(XMLSchemaNodeFactory.ConvertTypeToString(element.NodeType));
				element.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#endregion

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public virtual string ToXPath()
        {
            return "";
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        [BrowsableAttribute(false)]
        public virtual IXaclObject Parent
        {
            get
            {
                // TODO
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public virtual IEnumerator GetChildren()
        {
            // return an enumerator
            return List.GetEnumerator();
        }

        #endregion
	}
}