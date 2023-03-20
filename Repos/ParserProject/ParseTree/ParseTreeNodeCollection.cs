/*
* @(#)ParseTreeNodeCollection.cs
*
* Copyright (c) 2003-2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IParseTreeNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005 </version>
	/// <author> Yong Zhang</author>
	[Serializable]
	public class ParseTreeNodeCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ParseTreeNodeCollection class.
		/// </summary>
		public ParseTreeNodeCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IParseTreeNode this[int index]  
		{
			get  
			{
				return ((IParseTreeNode) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an IParseTreeNode to the ParseTreeNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IParseTreeNode value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a ParseTreeNodeCollection to the end of the ParseTreeNodeCollection.
		/// </summary>
		/// <param name="collection">The ParseTreeNodeCollection whose elements should be added to the end of the ParseTreeNodeCollection</param>
		public void AddRange(ParseTreeNodeCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IParseTreeNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IParseTreeNode value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IParseTreeNode value )  
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
		public bool Contains(IParseTreeNode value )  
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
			if (!(value is IParseTreeNode))
			{
				throw new ArgumentException( "value must be of type IParseTreeNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IParseTreeNode))
			{
				throw new ArgumentException( "value must be of type IParseTreeNode.", "value" );
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
			if (!(newValue is IParseTreeNode))
			{
				throw new ArgumentException( "newValue must be of type IParseTreeNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IParseTreeNode))
			{
				throw new ArgumentException( "value must be of type IParseTreeNode." );
			}
		}
	}
}