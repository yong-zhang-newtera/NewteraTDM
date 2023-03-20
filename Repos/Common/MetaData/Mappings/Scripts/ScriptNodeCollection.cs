/*
* @(#)ScriptNodeCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IScriptNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 23 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	public class ScriptNodeCollection : CollectionBase, IScriptNode
	{
		/// <summary>
		///  Initializes a new instance of the ScriptNodeCollection class.
		/// </summary>
		public ScriptNodeCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of ScriptNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ScriptNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IScriptNode this[int index]  
		{
			get  
			{
				return ((IScriptNode) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an IScriptNode to the ScriptNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IScriptNode value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// Adds the elements of a ScriptNodeCollection to the end of the ScriptNodeCollection.
		/// </summary>
		/// <param name="collection">The ScriptNodeCollection whose elements should be added to the end of the ScriptNodeCollection</param>
		public void AddRange(ScriptNodeCollection collection )  
		{
			InnerList.AddRange(collection);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IScriptNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IScriptNode value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IScriptNode value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IScriptNode value )  
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
			if (!(value is IScriptNode))
			{
				throw new ArgumentException( "value must be of type IScriptNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IScriptNode))
			{
				throw new ArgumentException( "value must be of type IScriptNode.", "value" );
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
			if (!(newValue is IScriptNode))
			{
				throw new ArgumentException( "newValue must be of type IScriptNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IScriptNode))
			{
				throw new ArgumentException( "value must be of type IScriptNode." );
			}
		}

		#region IScriptNode Members

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
				IScriptNode node = ScriptNodeFactory.Instance.Create(xmlElement);

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

			foreach (IScriptNode node in List)
			{
				child = parent.OwnerDocument.CreateElement(ScriptNodeFactory.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
				parent.AppendChild(child);
			}		
		}

		#endregion

	}
}