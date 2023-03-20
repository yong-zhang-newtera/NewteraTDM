/*
* @(#)SchemaInfoCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle SchemaInfo when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 17 Mar 2005 </version>
	/// <author> Yong Zhang</author>
	public class SchemaInfoCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the SchemaInfoCollection class.
		/// </summary>
		public SchemaInfoCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public SchemaInfo this[int index]  
		{
			get  
			{
				return ((SchemaInfo) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an SchemaInfo to the SchemaInfoCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(SchemaInfo value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a SchemaInfoCollection to the end of the SchemaInfoCollection.
		/// </summary>
		/// <param name="collection">The SchemaInfoCollection whose elements should be added to the end of the SchemaInfoCollection</param>
		public void AddRange(SchemaInfoCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(SchemaInfo value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, SchemaInfo value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(SchemaInfo value )  
		{
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
				base.RemoveAt(index);
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(SchemaInfo value )  
		{
			// If value is not of type SchemaModelElement, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is SchemaInfo))
			{
				throw new ArgumentException( "value must be of type SchemaInfo.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is SchemaInfo))
			{
				throw new ArgumentException( "value must be of type SchemaInfo.", "value" );
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
			if (!(newValue is SchemaInfo))
			{
				throw new ArgumentException( "newValue must be of type SchemaInfo.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is SchemaInfo))
			{
				throw new ArgumentException( "value must be of type SchemaInfo." );
			}
		}
	}
}