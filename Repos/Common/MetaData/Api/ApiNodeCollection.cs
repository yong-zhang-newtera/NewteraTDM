/*
* @(#)ApiNodeCollection.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IApiNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 16 Oct 2015 </version>
	public class ApiNodeCollection : CollectionBase, IApiNode
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		///  Initializes a new instance of the ApiNodeCollection class.
		/// </summary>
		public ApiNodeCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of ApiNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ApiNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IApiNode this[int index]  
		{
			get  
			{
				return ((IApiNode) List[index] );
			}
			set  
			{
				List[index] = value;
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("ApiNodeCollection", value));
                    }

                    value.ValueChanged += new EventHandler(ValueChangedHandler);
                }
			}
		}

        /// <summary>
        /// Implemention of Indexer member using attribute name
        /// </summary>
        public IApiNode this[string name]
        {
            get
            {
                IApiNode found = null;

                foreach (IApiNode element in List)
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
		/// Adds an IApiNode to the ApiNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IApiNode value )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("ApiNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a ApiNodeCollection to the end of the ApiNodeCollection.
		/// </summary>
		/// <param name="collection">The ApiNodeCollection whose elements should be added to the end of the ApiNodeCollection</param>
		public void AddRange(ApiNodeCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IApiNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IApiNode value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("ApiNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IApiNode value )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("ApiNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IApiNode value )  
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
			if (!(value is IApiNode))
			{
				throw new ArgumentException( "value must be of type IApiNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IApiNode))
			{
				throw new ArgumentException( "value must be of type IApiNode.", "value" );
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
			if (!(newValue is IApiNode))
			{
				throw new ArgumentException( "newValue must be of type IApiNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IApiNode))
			{
				throw new ArgumentException( "value must be of type IApiNode." );
			}
		}

		#region IApiNode Members

        /// <summary>
        /// Gets or sets of the name
        /// </summary>
        public string Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of ApiNodeType values</value>
        public virtual ApiNodeType NodeType
		{
			get
			{
				return ApiNodeType.Collection;
			}
		}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public virtual void Accept(IApiNodeVisitor visitor)
        {
        }

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IApiNode node = ApiNodeFactory.Instance.Create(xmlElement);

				this.Add(node);
			}		
		}

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (IApiNode node in List)
			{
                child = parent.OwnerDocument.CreateElement(ApiNodeFactory.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
				parent.AppendChild(child);
			}		
		}

		#endregion

		/// <summary>
		/// Handler for Value Changed event fired by members of a xacl model
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		private void ValueChangedHandler(object sender, EventArgs e)
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                // propagate the event
                if (ValueChanged != null)
                {
                    ValueChanged(sender, e);
                }
            }
		}
	}
}