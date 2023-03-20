/*
* @(#)ColumnInfoCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;


	/// <summary>
	/// An object collection class to handle ColumnInfo when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 14 Apr 2006 </version>
	public class ColumnInfoCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ColumnInfoCollection class.
		/// </summary>
		public ColumnInfoCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public ColumnInfo this[int index]  
		{
			get  
			{
				return( (ColumnInfo) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public ColumnInfo this[string name]  
		{
			get  
			{
				ColumnInfo found = null;

				foreach (ColumnInfo element in List)
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
		/// Clone the collection
		/// </summary>
		/// <returns>A cloned collection</returns>
		public ColumnInfoCollection Clone()
		{
			ColumnInfoCollection clonedCollection = new ColumnInfoCollection();

			ColumnInfo clonedColumnInfo;
			foreach (ColumnInfo columnInfo in this.List)
			{
				clonedColumnInfo = new ColumnInfo();
				clonedColumnInfo.Name = columnInfo.Name;
				clonedColumnInfo.Caption = columnInfo.Caption;
				clonedColumnInfo.IsChecked = columnInfo.IsChecked;
                clonedColumnInfo.IsArray = columnInfo.IsArray;

				clonedCollection.Add(clonedColumnInfo);
			}

			return clonedCollection;
		}

		/// <summary>
		/// Adds an ColumnInfo to the ColumnInfoCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(ColumnInfo value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ColumnInfo value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ColumnInfo value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ColumnInfo value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ColumnInfo value )  
		{
			// If value is not of type ColumnInfo, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ColumnInfo))
			{
				throw new ArgumentException( "value must be of type ColumnInfo.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ColumnInfo))
			{
				throw new ArgumentException( "value must be of type ColumnInfo.", "value" );
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
			if (!(newValue is ColumnInfo))
			{
				throw new ArgumentException( "newValue must be of type ColumnInfo.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ColumnInfo))
			{
				throw new ArgumentException( "value must be of type ColumnInfo." );
			}
		}
	}
}