/*
* @(#)ChartNodeCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IChartNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 24 Apr 2006 </version>
	public abstract class ChartNodeCollection : CollectionBase, IChartNode
	{
		private bool _isAltered; // run-time use

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		///  Initializes a new instance of the ChartNodeCollection class.
		/// </summary>
		public ChartNodeCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of ChartNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ChartNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IChartNode this[int index]  
		{
			get  
			{
				return ((IChartNode) List[index] );
			}
			set  
			{
				List[index] = value;
				if (ValueChanged != null)
				{
					ValueChanged(this, new ValueChangedEventArgs("ChartNodeCollection", value));
				}

				value.ValueChanged += new EventHandler(ValueChangedHandler);
			}
		}

		/// <summary>
		/// Adds an IChartNode to the ChartNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IChartNode value )  
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ChartNodeCollection", value));
			}

			value.ValueChanged += new EventHandler(ValueChangedHandler);

			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a ChartNodeCollection to the end of the ChartNodeCollection.
		/// </summary>
		/// <param name="collection">The ChartNodeCollection whose elements should be added to the end of the ChartNodeCollection</param>
		public void AddRange(ChartNodeCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IChartNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IChartNode value)  
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ChartNodeCollection", value));
			}

			value.ValueChanged += new EventHandler(ValueChangedHandler);

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IChartNode value )  
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ChartNodeCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IChartNode value )  
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
			if (!(value is IChartNode))
			{
				throw new ArgumentException( "value must be of type IChartNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IChartNode))
			{
				throw new ArgumentException( "value must be of type IChartNode.", "value" );
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
			if (!(newValue is IChartNode))
			{
				throw new ArgumentException( "newValue must be of type IChartNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IChartNode))
			{
				throw new ArgumentException( "value must be of type IChartNode." );
			}
		}

		#region IChartNode Members

		/// <summary>
		/// Gets or sets the information indicating whether the content of the Node
		/// has been altered or not
		/// </summary>
		/// <value>True when it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract NodeType NodeType
		{
			get;
		}

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IChartNode node = NodeFactory.Instance.Create(xmlElement);

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

			foreach (IChartNode node in List)
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

			this._isAltered = true;
		}
	}
}