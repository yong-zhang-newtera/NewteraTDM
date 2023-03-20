/*
* @(#)ImportResultEntryCollection.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Import
{
	using System;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle ImportResultEntry when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 21 Apr 2007 </version>
	public class ImportResultEntryCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ImportResultEntryCollection class.
		/// </summary>
		public ImportResultEntryCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public ImportResultEntry this[int index]  
		{
			get  
			{
				return( (ImportResultEntry) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an ImportResultEntry to the SchemaModelElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(ImportResultEntry value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// Adds an ImportResultEntry to the SchemaModelElementCollection.
		/// </summary>
		/// <param name="collection">Another collection</param>
		/// <returns>The position into which the new element was inserted</returns>
		public void AddRange(ImportResultEntryCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ImportResultEntry value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ImportResultEntry value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ImportResultEntry value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ImportResultEntry value )  
		{
			// If value is not of type ImportResultEntry, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ImportResultEntry))
			{
				throw new ArgumentException( "value must be of type ImportResultEntry.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ImportResultEntry))
			{
				throw new ArgumentException( "value must be of type ImportResultEntry.", "value" );
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
			if (!(newValue is ImportResultEntry))
			{
				throw new ArgumentException( "newValue must be of type ImportResultEntry.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ImportResultEntry))
			{
				throw new ArgumentException( "value must be of type ImportResultEntry." );
			}
		}
	}
}