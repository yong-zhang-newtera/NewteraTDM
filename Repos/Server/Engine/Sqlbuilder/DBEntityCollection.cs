/*
* @(#)DBEntityCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle DBEntity when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	public class DBEntityCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the DBEntityCollection class.
		/// </summary>
		public DBEntityCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public DBEntity this[int index]  
		{
			get  
			{
				return( (DBEntity) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an DBEntity to the DBEntityCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(DBEntity value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(DBEntity value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, DBEntity value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(DBEntity value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// Add a collection of DBEntity object to the end of this collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void AddCollection(DBEntityCollection entities )  
		{
			foreach (DBEntity entity in entities)
			{
				this.Add(entity);
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the IList; otherwise, false.</returns>
		public bool Contains(DBEntity value )  
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
			if (!(value is DBEntity))
			{
				throw new ArgumentException( "value must be of type DBEntity.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is DBEntity))
			{
				throw new ArgumentException( "value must be of type DBEntity.", "value" );
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
			if (!(newValue is DBEntity))
			{
				throw new ArgumentException( "newValue must be of type DBEntity.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is DBEntity))
			{
				throw new ArgumentException( "value must be of type DBEntity." );
			}
		}
	}
}