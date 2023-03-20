/*
* @(#)DataViewSlideCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle DataViewSlide when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 28 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DataViewSlideCollection : CollectionBase
	{
		// Event to fire when count of collection is changed
		public event EventHandler CountChanged;

		/// <summary>
		///  Initializes a new instance of the DataViewSlideCollection class.
		/// </summary>
		public DataViewSlideCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public DataViewSlide this[int index]  
		{
			get  
			{
				return( (DataViewSlide) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an DataViewSlide to the DataViewSlideCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(DataViewSlide value )  
		{
			int pos = List.Add( value );

			// fire the event when adding a value
			if (CountChanged != null)
			{
				CountChanged(this, EventArgs.Empty);
			}

			return pos;
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(DataViewSlide value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, DataViewSlide value)  
		{
			List.Insert(index, value );

			// fire the event after inserting a value
			if (CountChanged != null)
			{
				CountChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(DataViewSlide value )  
		{
			List.Remove(value);

			// fire the event after removing a value
			if (CountChanged != null)
			{
				CountChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(DataViewSlide value )  
		{
			// If value is not of type DataViewSlide, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is DataViewSlide))
			{
				throw new ArgumentException( "value must be of type DataViewSlide.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is DataViewSlide))
			{
				throw new ArgumentException( "value must be of type DataViewSlide.", "value" );
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
			if (!(newValue is DataViewSlide))
			{
				throw new ArgumentException( "newValue must be of type DataViewSlide.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is DataViewSlide))
			{
				throw new ArgumentException( "value must be of type DataViewSlide." );
			}
		}
	}
}