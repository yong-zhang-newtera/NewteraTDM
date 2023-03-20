/*
* @(#)ProjectInfoCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle ProjectInfo when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 8 Dec 2006 </version>
	public class ProjectInfoCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ProjectInfoCollection class.
		/// </summary>
		public ProjectInfoCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public ProjectInfo this[int index]  
		{
			get  
			{
				return( (ProjectInfo) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using project name and version
		/// </summary>
		public ProjectInfo this[string nameAndVersion]  
		{
			get  
			{
				ProjectInfo found = null;

				foreach (ProjectInfo element in List)
				{
                    if (element.NameAndVersion == nameAndVersion)
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
		/// Adds an ProjectInfo to the ProjectInfoCollection in a sorted order.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public virtual int Add(ProjectInfo value )  
		{
            int index = 0;
            foreach (ProjectInfo projectInfo in List)
            {
                if (string.Compare(projectInfo.NameAndVersion, value.NameAndVersion) > 0)
                {
                    break;
                }

                index++;
            }

            List.Insert(index, value);
			return index;
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ProjectInfo value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ProjectInfo value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ProjectInfo value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ProjectInfo value )  
		{
			// If value is not of type ProjectInfo, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ProjectInfo))
			{
				throw new ArgumentException( "value must be of type ProjectInfo.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ProjectInfo))
			{
				throw new ArgumentException( "value must be of type ProjectInfo.", "value" );
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
			if (!(newValue is ProjectInfo))
			{
				throw new ArgumentException( "newValue must be of type ProjectInfo.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ProjectInfo))
			{
				throw new ArgumentException( "value must be of type ProjectInfo." );
			}
		}
	}
}