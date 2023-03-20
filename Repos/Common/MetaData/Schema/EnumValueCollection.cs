/*
* @(#)EnumValueCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle EnumValue when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	public class EnumValueCollection : CollectionBase
	{
        private Hashtable _enumDisplayTextTable = new Hashtable();
        private Hashtable _enumValueTable = new Hashtable();

		/// <summary>
		///  Initializes a new instance of the EnumValueCollection class.
		/// </summary>
		public EnumValueCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public EnumValue this[int index]  
		{
			get  
			{
				return( (EnumValue) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an EnumValue to the EnumValueCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(EnumValue value )  
		{
            ClearCache();
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(EnumValue value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, EnumValue value)  
		{
            ClearCache();
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(EnumValue value )  
		{
            ClearCache();
			// compare the Value property.
			foreach (EnumValue enumValue in List)
			{
				if (enumValue.Value == value.Value)
				{
					List.Remove(enumValue);
					break;
				}
			}
		}

		/// <summary>
		/// determines whether the collection contains a specific EnumValue
		/// </summary>
		/// <param name="value">The EnumValue to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(EnumValue value)  
		{
			bool status = false;

			// compare the Value property.
			foreach (EnumValue enumValue in List)
			{
				if (enumValue.Value == value.Value)
				{
					status = true;
					break;
				}
			}
			
			return status;
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The string object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(string value)  
		{
			bool status = false;

			// compare the Value property.
			foreach (EnumValue enumValue in List)
			{
				if (enumValue.DisplayText == value)
				{
					status = true;
					break;
				}
			}
			
			return status;
		}

        /// <summary>
        /// Convert an enum value to its display text.
        /// </summary>
        /// <param name="val">An enum value.</param>
        /// <returns>The corresponding display text for the enum value</returns>
        public string ConvertToEnumDisplayText(string val)
        {
            string text = "";
            if (_enumDisplayTextTable.Count == 0)
            {
                // The collection element may return a large number of values, therefore,
                // we must build a hashtable to get a display text for a value
                // for the sake of the performance
                if (List != null)
                {
                    foreach (EnumValue enumValue in List)
                    {
                        try
                        {
                            _enumDisplayTextTable.Add(enumValue.Value, enumValue.DisplayText);
                        }
                        catch (Exception ex)
                        {
                            // found duplicated enum value, ignore it
                        }
                    }
                }
            }

            text = (string)_enumDisplayTextTable[val];
            if (text == null)
            {
                // the enum item for the value has been deleted,
                // throw an exception
                throw new MissingFieldException("Unable to find the display text for an enum value " + val); 
            }

            return text;
        }

        /// <summary>
        /// Convert an enum display text to its value.
        /// </summary>
        /// <param name="text">An enum display text.</param>
        /// <returns>The corresponding enum value</returns>
        public string ConvertToEnumValue(string text)
        {
            string val;
            if (_enumValueTable.Count == 0)
            {
                // The collection may return a large number of values, therefore,
                // we must build a hashtable to get a value for a display text
                // for the sake of the performance
                if (List != null)
                {
                    foreach (EnumValue enumValue in List)
                    {
                        try
                        {
                            _enumValueTable.Add(enumValue.DisplayText, enumValue.Value);
                        }
                        catch (Exception)
                        {
                            // found a duplicated enum value, ignore it
                        }
                    }
                }
            }

            val = (string)_enumValueTable[text];
            if (val == null)
            {
                val = "";
            }

            return val;
        }

		/// <summary>
		/// Clone a new collection of EnumValue objects
		/// </summary>
		/// <returns>A cloned collection</returns>
		public EnumValueCollection Clone()
		{
			EnumValueCollection newValues = new EnumValueCollection();
			EnumValue newValue;
			foreach (EnumValue enumValue in this.List)
			{
				newValue = new EnumValue();
				newValue.Value = enumValue.Value;
				newValue.DisplayText = enumValue.DisplayText;
                newValue.ImageName = enumValue.ImageName;
				newValues.Add(newValue);
			}

			return newValues;
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is EnumValue))
			{
				throw new ArgumentException( "value must be of type EnumValue.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is EnumValue))
			{
				throw new ArgumentException( "value must be of type EnumValue.", "value" );
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
			if (!(newValue is EnumValue))
			{
				throw new ArgumentException( "newValue must be of type EnumValue.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is EnumValue))
			{
				throw new ArgumentException( "value must be of type EnumValue." );
			}
		}

        /// <summary>
        /// Clear cached values
        /// </summary>
        private void ClearCache()
        {
            _enumDisplayTextTable.Clear();
            _enumValueTable.Clear();
        }
	}
}