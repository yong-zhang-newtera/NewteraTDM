/*
* @(#)SchemaModelElementCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Collections;
    using System.ComponentModel;

    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// An object collection class to handle SchemaModelElement when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
    public class SchemaModelElementCollection : CollectionBase, IXaclObject
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		///  Initializes a new instance of the SchemaModelElementCollection class.
		/// </summary>
		public SchemaModelElementCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public SchemaModelElement this[int index]  
		{
			get  
			{
				return( (SchemaModelElement) List[index] );
			}
			set  
			{
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    // Raise the event for value change
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("SchemaModelElementCollection", value));
                    }

                    value.ValueChanged += new EventHandler(ValueChangedHandler);
                }

				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an SchemaModelElement to the SchemaModelElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(SchemaModelElement value )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                // Raise the event for value change
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("SchemaModelElementCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			// add the element based on its position
			int index = 0;
			foreach (SchemaModelElement element in List)
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
		public int IndexOf(SchemaModelElement value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, SchemaModelElement value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                // Raise the event for value change
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("SchemaModelElementCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(SchemaModelElement value )  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("SchemaModelElementCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(SchemaModelElement value )  
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
			SchemaModelElement first, second;

			if (size > 1)
			{
				for ( i = (size - 1); i >= 0; i-- )
				{
					for ( j = 1; j <= i; j++ )
					{
						first = (SchemaModelElement) List[j-1];
						second = (SchemaModelElement) List[j];
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
			if (!(value is SchemaModelElement))
			{
				throw new ArgumentException( "value must be of type SchemaModelElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is SchemaModelElement))
			{
				throw new ArgumentException( "value must be of type SchemaModelElement.", "value" );
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
			if (!(newValue is SchemaModelElement))
			{
				throw new ArgumentException( "newValue must be of type SchemaModelElement.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is SchemaModelElement))
			{
				throw new ArgumentException( "value must be of type SchemaModelElement." );
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

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public virtual string ToXPath()
        {
            return "SchemaModelELementCollection";
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
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public virtual IEnumerator GetChildren()
        {
            return this.GetEnumerator();
        }

        #endregion
	}
}