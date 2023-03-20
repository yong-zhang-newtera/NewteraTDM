/*
* @(#)MetaDataElementSortedList.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.Collections;

	/// <summary>
	/// A collection class that keeps the a list of objects in a sorted
	/// order based on display positions.
	/// </summary>
	/// <version> 1.0.1 8 Jun 2005 </version>
	/// <author> Yong Zhang</author>
	public class MetaDataElementSortedList
	{
		private IList _positions;
		private IList _values;

		/// <summary>
		///  Initializes a new instance of the MetaDataElementSortedList class.
		/// </summary>
		public MetaDataElementSortedList() : base()
		{
			_positions = new ArrayList();
			_values = new ArrayList();
		}

		/// <summary>
		/// Get the sorted list of objects based on their display positions.
		/// </summary>
		/// <value>A sorted list of objects.</value>
		public IList Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Adds an object to the list at the position based on its
		/// display position.
		/// </summary>
		/// <param name="position">The integer represents display position.</param>
		/// <param name="val">the object to be added</param>
		/// <returns>The actual position into which the new element was inserted</returns>
		public int Add(int position, object val)  
		{
			int index = 0;

			foreach (int pos in _positions)
			{
				// the value is added in acending order
				if (position < pos)
				{
					// found a place in the list
					break;
				}
				else
				{
					index++;
				}
			}

			if (index < _positions.Count)
			{
				_positions.Insert(index, position);
				_values.Insert(index, val);
			}
			else
			{
				_positions.Add(position);
				_values.Add(val);
			}

			return index;
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(object value )  
		{
			return (_values.IndexOf( value ));
		}


		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(object value )  
		{
			int index = _values.IndexOf(value);
			if (index >= 0)
			{
				_positions.RemoveAt(index);
				_values.RemoveAt(index);
			}
		}
	}
}