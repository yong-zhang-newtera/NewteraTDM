/*
* @(#)DataGridViewElementCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IDataGridViewElement when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 28 May 2006 </version>
	///
	public class DataGridViewElementCollection : CollectionBase, IDataGridViewElement
	{
		private DataGridViewModel _dataGridView;

		/// <summary>
		///  Initializes a new instance of the DataGridViewElementCollection class.
		/// </summary>
		public DataGridViewElementCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of DataGridViewElementCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataGridViewElementCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the DataGridViewModel that owns this element
		/// </summary>
		/// <value>DataGridViewModel object</value>
		public virtual DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
				if (value != null)
				{
					foreach (IDataGridViewElement element in this.List)
					{
						element.DataGridView = value;
					}
				}
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public IDataGridViewElement this[int index]  
		{
			get  
			{
				return( (IDataGridViewElement) List[index] );
			}
			set  
			{
				if (DataGridView != null)
				{
					value.DataGridView = DataGridView;
				}

				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public IDataGridViewElement this[string name]  
		{
			get  
			{
				IDataGridViewElement found = null;

				foreach (IDataGridViewElement element in List)
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
		/// Adds an IDataGridViewElement to the DataGridViewElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IDataGridViewElement value )  
		{
			if (DataGridView != null)
			{
				value.DataGridView = DataGridView;
			}

			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IDataGridViewElement value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IDataGridViewElement value)  
		{
			if (DataGridView != null)
			{
				value.DataGridView = DataGridView;
			}

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IDataGridViewElement value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IDataGridViewElement value )  
		{
			// If value is not of type IDataGridViewElement, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is IDataGridViewElement))
			{
				throw new ArgumentException( "value must be of type IDataGridViewElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IDataGridViewElement))
			{
				throw new ArgumentException( "value must be of type IDataGridViewElement.", "value" );
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
			if (!(newValue is IDataGridViewElement))
			{
				throw new ArgumentException( "newValue must be of type IDataGridViewElement.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IDataGridViewElement))
			{
				throw new ArgumentException( "value must be of type IDataGridViewElement." );
			}
		}


		#region IDataGridViewElement members


		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public virtual ViewElementType ElementType	{
			get
			{
				return ViewElementType.Collection;
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
			set
			{
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
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public virtual void Accept(IDataGridViewElementVisitor visitor)
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
				IDataGridViewElement element = ViewElementFactory.Instance.Create(xmlElement);

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

			foreach (IDataGridViewElement element in List)
			{
				child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(element.ElementType));
				element.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#endregion
	}
}