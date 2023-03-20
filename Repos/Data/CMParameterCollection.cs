/*
* @(#)CMParameterCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
	using System.Collections;

	/// <summary>
	/// Collects all parameters relevant to a Command object and their mappings to DataSet
	/// columns.
	/// 
	/// Because CMDataParameterCollection is primarily a List,
	/// the CMParameterCollection can use an existing class for most of the implementation.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003</version>
	/// <author>  		Yong Zhang </author>
	public class CMParameterCollection : CollectionBase, IDataParameterCollection
	{
		/// <summary>
		///  Initializes a new instance of the CMParameterCollection class.
		/// </summary>
		public CMParameterCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public CMParameter this[int index]  
		{
			get  
			{
				return ((CMParameter) List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public CMParameter this[string name]  
		{
			get  
			{
				CMParameter found = null;
				foreach (CMParameter parameter in List)
				{
					if (parameter.ParameterName == name)
					{
						found = parameter;
						break;
					}
				}

				return found;
			}
			set  
			{
				List.Add(new CMParameter(name, null));
			}
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		object IDataParameterCollection.this[string name]  
		{
			get  
			{
				return this[name];
			}
			set
			{
				// TODO
			}
		}

		/// <summary>
		/// Adds a CMParameter to the CMParameterCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(CMParameter value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(CMParameter value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// determines the index of a specific item of a given name in the collection
		/// </summary>
		/// <param name="parameterName">The name of the parameter to locate.</param>
		/// <returns> The zero-based location of the CMDataParameter within the collection.
		/// otherwise, -1 if not found</returns>
		public int IndexOf(string parameterName)
		{
			int index = 0;
			foreach (CMParameter parameter in List)
			{
				if (parameter.ParameterName == parameterName)
				{
					return index;
				}
				
				index++;
			}
			
			return - 1;
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, CMParameter value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(CMParameter value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// Removes the specified CMParameter from the collection using the parameter name.
		/// </summary>
		/// <param name="parameterName">The name of the SqlParameter object to retrieve. </param>
		public void RemoveAt(string parameterName)  
		{
			int index = this.IndexOf(parameterName);
			if (index != -1)
			{
				List.RemoveAt(index);
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(CMParameter value )  
		{
			// If value is not of type CMParameter, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// determines whether the collection contains a specific parameter of give name.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to retrieve.</param>
		/// <returns> true if the collection contains the parameter; otherwise, false.
		/// </returns>
		public bool Contains(string parameterName)
		{
			return (- 1 != IndexOf(parameterName));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is CMParameter))
			{
				throw new ArgumentException( "value must be of type CMParameter.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is CMParameter))
			{
				throw new ArgumentException( "value must be of type CMParameter.", "value" );
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
			if (!(newValue is CMParameter))
			{
				throw new ArgumentException( "newValue must be of type CMParameter.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is CMParameter))
			{
				throw new ArgumentException( "value must be of type CMParameter." );
			}
		}
	}
}