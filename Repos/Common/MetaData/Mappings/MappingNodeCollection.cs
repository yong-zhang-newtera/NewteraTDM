/*
* @(#)MappingNodeCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IMappingNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 03 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	public class MappingNodeCollection : CollectionBase, IMappingNode
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption changed event handler
		/// </summary>
		public event EventHandler CaptionChanged;

		private bool _isAltered;

		/// <summary>
		///  Initializes a new instance of the MappingNodeCollection class.
		/// </summary>
		public MappingNodeCollection() : base()
		{
			_isAltered = false;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the collection
		/// has been altered.
		/// </summary>
		/// <value>true if it has been altered, false otherwise.</value>
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
		/// Initiating an instance of MappingNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal MappingNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IMappingNode this[int index]  
		{
			get  
			{
				return ((IMappingNode) List[index] );
			}
			set  
			{
				List[index] = value;
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", value));
                    }

                    value.ValueChanged += new EventHandler(ValueChangedHandler);
                }
			}
		}

		/// <summary>
		/// Adds an IMappingNode to the MappingNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IMappingNode value )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			// add the element based on its position
			int index = 0;
			foreach (IMappingNode element in List)
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
		/// Adds the elements of a MappingNodeCollection to the end of the MappingNodeCollection.
		/// </summary>
		/// <param name="collection">The MappingNodeCollection whose elements should be added to the end of the MappingNodeCollection</param>
		public void AddRange(MappingNodeCollection collection )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", collection));
                }

                foreach (IMappingNode node in collection)
                {
                    node.ValueChanged += new EventHandler(ValueChangedHandler);
                }
            }

			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IMappingNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IMappingNode value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IMappingNode value )  
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// removes the a specific object at the given index from the collection
		/// </summary>
		/// <param name="index">The index of the object to be removed.</param>
		public void RemoveNodeAt(int index )  
		{
			if (index >= 0 && index < List.Count)
			{
				if (ValueChanged != null)
				{
					ValueChanged(this, new ValueChangedEventArgs("MappingNodeCollection", List[index]));
				}

				base.RemoveAt(index);
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IMappingNode value )  
		{
			// If value is not of type SchemaModelElement, this will return false.
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
			IMappingNode first, second;

			if (size > 1)
			{
				for ( i = (size - 1); i >= 0; i-- )
				{
					for ( j = 1; j <= i; j++ )
					{
						first = (IMappingNode) List[j-1];
						second = (IMappingNode) List[j];
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
			if (!(value is IMappingNode))
			{
				throw new ArgumentException( "value must be of type IMappingNode.", "value" );
			}

			_isAltered = true;
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IMappingNode))
			{
				throw new ArgumentException( "value must be of type IMappingNode.", "value" );
			}

			_isAltered = true;
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (!(newValue is IMappingNode))
			{
				throw new ArgumentException( "newValue must be of type IMappingNode.", "newValue" );
			}

			_isAltered = true;
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IMappingNode))
			{
				throw new ArgumentException( "value must be of type IMappingNode." );
			}
		}

		#region IMappingNode Members

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
		/// Gets or sets the display position of element
		/// </summary>
		/// <value>The display position.</value>
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
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType
		{
			get
			{
				return NodeType.Collection;
			}
		}

		/// <summary>
		/// Make a copy of the IMappingNode instance
		/// </summary>
		/// <returns>A copy of IMappingNode instance</returns>
		public virtual IMappingNode Copy()
		{
			// convert this AttributeMapping instance to xml
			XmlDocument doc = new XmlDocument();
			XmlElement xmlElement = doc.CreateElement(MappingNodeFactory.ConvertTypeToString(this.NodeType));
			IMappingNode copy = MappingNodeFactory.Instance.Create(xmlElement);

			return copy;
		}

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IMappingNode node = MappingNodeFactory.Instance.Create(xmlElement);

				this.Add(node);
			}		
		}

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (IMappingNode node in List)
			{
				child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
				parent.AppendChild(child);
			}		
		}

		#endregion

		/// <summary>
		/// Handler for Value Changed event fired by members of a xacl model
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		private void ValueChangedHandler(object sender, EventArgs e)
		{
			// propagate the event
			if (ValueChanged != null)
			{
				ValueChanged(sender, e);
			}

			// set the flag
			IsAltered = true;
		}
	}
}