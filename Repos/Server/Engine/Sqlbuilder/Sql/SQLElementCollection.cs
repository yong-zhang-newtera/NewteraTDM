/*
* @(#)SQLElementCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle SQLElement when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	public class SQLElementCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the SQLElementCollection class.
		/// </summary>
		public SQLElementCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public SQLElement this[int index]  
		{
			get  
			{
				return ((SQLElement) List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds a SQLElement to the SQLElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(SQLElement value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(SQLElement value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, SQLElement value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(SQLElement value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// Sort elements in the collection based on their position
		/// </summary>
		/// <remarks>SQLElement implements IComparable interface that uses position as comparator</remarks>
		public void Sort()  
		{
			this.InnerList.Sort();
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(SQLElement value )  
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
			if (!(value is SQLElement))
			{
				throw new ArgumentException( "value must be of type SQLElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is SQLElement))
			{
				throw new ArgumentException( "value must be of type SQLElement.", "value" );
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
			if (!(newValue is SQLElement))
			{
				throw new ArgumentException( "newValue must be of type SQLElement.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is SQLElement))
			{
				throw new ArgumentException( "value must be of type SQLElement." );
			}
		}
	}
}