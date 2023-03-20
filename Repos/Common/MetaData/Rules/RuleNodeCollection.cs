/*
* @(#)RuleNodeCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IRuleNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 16 Jun 2004 </version>
	/// <author> Yong Zhang</author>
	public class RuleNodeCollection : CollectionBase, IRuleNode
	{
        private IRuleNode _owner = null; // run-time use

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		///  Initializes a new instance of the RuleNodeCollection class.
		/// </summary>
		public RuleNodeCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of RuleNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal RuleNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets the owner that owns the node
        /// </summary>
        /// <value></value>
        public IRuleNode Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IRuleNode this[int index]  
		{
			get  
			{
				return ((IRuleNode) List[index] );
			}
			set  
			{
				List[index] = value;
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgs("RuleNodeCollection", value));
                    }

                    value.ValueChanged += new EventHandler(ValueChangedHandler);
                }
			}
		}

		/// <summary>
		/// Adds an IRuleNode to the RuleNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IRuleNode value )  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("RuleNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            value.Owner = this;

			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a RuleNodeCollection to the end of the RuleNodeCollection.
		/// </summary>
		/// <param name="collection">The RuleNodeCollection whose elements should be added to the end of the RuleNodeCollection</param>
		public void AddRange(RuleNodeCollection collection )  
		{
            foreach (IRuleNode rule in collection)
            {
                rule.Owner = this;
            }

			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IRuleNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IRuleNode value)  
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new ValueChangedEventArgs("RuleNodeCollection", value));
                }

                value.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            value.Owner = this;

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		/// <returns>The position where the rule is removed, -1 if nothing is removed.</returns>
        public int Remove(IRuleNode value )  
		{
            int pos;
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("RuleNodeCollection", value));
			}

            pos = List.IndexOf(value);
            if (pos >= 0)
            {
                List.Remove(value);
            }

            return pos;
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IRuleNode value )  
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
			if (!(value is IRuleNode))
			{
				throw new ArgumentException( "value must be of type IRuleNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IRuleNode))
			{
				throw new ArgumentException( "value must be of type IRuleNode.", "value" );
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
			if (!(newValue is IRuleNode))
			{
				throw new ArgumentException( "newValue must be of type IRuleNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IRuleNode))
			{
				throw new ArgumentException( "value must be of type IRuleNode." );
			}
		}

		#region IRuleNode Members

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType
		{
			get
			{
				return NodeType.Collection;
			}
		}

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IRuleNode node = NodeFactory.Instance.Create(xmlElement);

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

			foreach (IRuleNode node in List)
			{
				child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(node.NodeType));
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
			// propagate the event
			if (ValueChanged != null)
			{
				ValueChanged(sender, e);
			}
		}
	}
}