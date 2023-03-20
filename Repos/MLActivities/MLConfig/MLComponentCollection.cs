/*
* @(#)SchemaModelElementCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLActivities.MLConfig
{
	using System;
	using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// An object collection class of IMLComponents.
    /// </summary>
    [Serializable]
    public class MLComponentCollection : CollectionBase, IMLComponnet
	{
        /// <summary>
        ///  Initializes a new instance of the MLComponentCollection class.
        /// </summary>
        public MLComponentCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IMLComponnet this[int index]  
		{
			get  
			{
				return( (IMLComponnet) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

        /// <summary>
        /// Adds an IMLComponnet to the Collection.
        /// </summary>
        /// <param name="value">the object to be added</param>
        /// <returns>The position into which the new element was inserted</returns>
        public int Add(IMLComponnet value )  
		{
			// append at the end
			return List.Add(value);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IMLComponnet value )  
		{
			return (List.IndexOf(value));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IMLComponnet value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IMLComponnet value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IMLComponnet value )  
		{
            // If value is not of type IMLComponnet, this will return false.
            return (List.Contains( value ));
		}

        /// <summary>
        /// Accept a visitor of IMLComponnetVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        /// <returns>true to continue travers, false to stop</returns>
        public bool Accept(IMLComponnetVisitor visitor)
        {
            bool status = visitor.VisitComponentCollection(this);
            // top down traverse the tree
            if (status)
            {
                foreach (IMLComponnet component in List)
                {
                    status = component.Accept(visitor);
                    if (!status)
                    {
                        break;
                    }
                }
            }

            return status;
        }

        #region IMLComponnet Members

        public string Name { get; set; }

        public IList Children {
            get
            {
                return List;
            }
            set
            {
 
            }
        }

        /// <summary>
        /// Write Machine Learning Configuration code to a writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="configType"></param>
        public void WriteTo(System.IO.StreamWriter writer, int indentLevel, MLConfigurationType configType)
        {

        }

        #endregion
    }
}