/*
* @(#)DataViewElementCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;
    using System.ComponentModel;

	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// An object collection class to handle IDataViewElement when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 28 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DataViewElementCollection : CollectionBase, IDataViewElement
	{
		private bool _isChanged; // run-time use only
        internal string _xpath = null; // run-time member
        private IDataViewElement _parentElement; // run-time use

		private DataViewModel _dataView;

		/// <summary>
		/// Value changed event habdler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption changed event handler
		/// </summary>
		public event EventHandler CaptionChanged;

		/// <summary>
		///  Initializes a new instance of the DataViewElementCollection class.
		/// </summary>
		public DataViewElementCollection() : base()
		{
            _parentElement = null;
		}

		/// <summary>
		/// Initiating an instance of DataViewElementCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataViewElementCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
            _parentElement = null;
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		public virtual DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
				if (value != null)
				{
					foreach (IDataViewElement element in this.List)
					{
						element.DataView = value;
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets the data view element that is the parent element in expression tree
        /// </summary>
        /// <value>The data view element</value>
        public IDataViewElement ParentElement
        {
            get
            {
                return _parentElement;
            }
            set
            {
                _parentElement = value;
            }
        }

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public IDataViewElement this[int index]  
		{
			get  
			{
				return( (IDataViewElement) List[index] );
			}
			set  
			{
				// Raise the event for value change
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("DataViewElementCollection", value));
                    }
                }

				if (DataView != null)
				{
					value.DataView = DataView;
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
		public IDataViewElement this[string name]  
		{
			get  
			{
				IDataViewElement found = null;

				foreach (IDataViewElement element in List)
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
		/// Adds an IDataViewElement to the DataViewElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public virtual int Add(IDataViewElement value )  
		{
			// Raise the event for value change
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("DataViewElementCollection", value));
                }
            }

			if (DataView != null)
			{
				value.DataView = DataView;
			}
            if (GlobalSettings.Instance.IsWindowClient)
            {
                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			// add the element based on its position
			int index = 0;
			foreach (IDataViewElement element in List)
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
		public int IndexOf(IDataViewElement value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public virtual void Insert(int index, IDataViewElement value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                // Raise the event for value change
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("DataViewElementCollection", value));
                }
            }

			if (DataView != null)
			{
				value.DataView = DataView;
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
		public void Remove(IDataViewElement value )  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("DataViewElementCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IDataViewElement value )  
		{
			// If value is not of type IDataViewElement, this will return false.
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
			IDataViewElement first, second;

			if (size > 1)
			{
				for ( i = (size - 1); i >= 0; i-- )
				{
					for ( j = 1; j <= i; j++ )
					{
						first = (IDataViewElement) List[j-1];
						second = (IDataViewElement) List[j];
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
			if (!(value is IDataViewElement))
			{
				throw new ArgumentException( "value must be of type IDataViewElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IDataViewElement))
			{
				throw new ArgumentException( "value must be of type IDataViewElement.", "value" );
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
			if (!(newValue is IDataViewElement))
			{
				throw new ArgumentException( "newValue must be of type IDataViewElement.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IDataViewElement))
			{
				throw new ArgumentException( "value must be of type IDataViewElement." );
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

		#region IDataViewElement members

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
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public virtual ElementType ElementType	{
			get
			{
				return ElementType.Collection;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the DataViewElement is
		/// readonly
		/// </summary>
		/// <value>true if it is read-only, false otherwise</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
			set
			{
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public virtual void Accept(IDataViewElementVisitor visitor)
		{
		}

		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>Null reference</value>
		public SchemaModelElement GetSchemaModelElement()
		{
			return null;
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IDataViewElement element = ElementFactory.Instance.Create(xmlElement);

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

			foreach (IDataViewElement element in List)
			{
				child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(element.ElementType));
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