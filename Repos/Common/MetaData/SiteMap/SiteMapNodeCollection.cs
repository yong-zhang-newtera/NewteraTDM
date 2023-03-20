/*
* @(#)SiteMapNodeCollection.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// An object collection class to handle ISiteMapNode when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009 </version>
	public class SiteMapNodeCollection : CollectionBase, ISiteMapNode
	{
        /// Title changed handler
        /// </summary>
        public event EventHandler TitleChanged;

		/// <summary>
		///  Initializes a new instance of the SiteMapNodeCollection class.
		/// </summary>
		public SiteMapNodeCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of SiteMapNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SiteMapNodeCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public ISiteMapNode this[int index]  
		{
			get  
			{
				return( (ISiteMapNode) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an ISiteMapNode to the SiteMapNodeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(ISiteMapNode value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ISiteMapNode value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ISiteMapNode value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ISiteMapNode value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ISiteMapNode value )  
		{
			// If value is not of type ISiteMapNode, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ISiteMapNode))
			{
				throw new ArgumentException( "value must be of type ISiteMapNode.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ISiteMapNode))
			{
				throw new ArgumentException( "value must be of type ISiteMapNode.", "value" );
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
			if (!(newValue is ISiteMapNode))
			{
				throw new ArgumentException( "newValue must be of type ISiteMapNode.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ISiteMapNode))
			{
				throw new ArgumentException( "value must be of type ISiteMapNode." );
			}
		}

		#region ISiteMapNode members

        public string Name
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string Title
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string Description
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the parent node
        /// </summary>
        public ISiteMapNode ParentNode
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
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>A SiteMapNodeCollection</value>
        public SiteMapNodeCollection ChildNodes
        {
            get
            {
                return null;
            }
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType	{
			get
			{
				return NodeType.SiteMapNodeCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public void Accept(ISiteMapNodeVisitor visitor)
        {
        }

        /// <summary>
        /// Add a node as a child
        /// </summary>
        /// <param name="child"></param>
        public void AddChildNode(ISiteMapNode child)
        {
        }

        /// <summary>
        /// Delete a child node
        /// </summary>
        /// <param name="child"></param>
        public void DeleteChildNode(ISiteMapNode child)
        {
        }

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				ISiteMapNode element = NodeFactory.Instance.Create(xmlElement);

				this.Add(element);
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (ISiteMapNode node in List)
			{
				child = parent.OwnerDocument.CreateElement(NodeFactory.Instance.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
				parent.AppendChild(child);
			}
		}

        /// <summary>
        /// Gets the node path that consists of node's displayed titles
        /// </summary>
        /// <returns>A title path</returns>
        public virtual string ToDisplayPath()
        {
            return "siteMapNodes";
        }

        /// <summary>
        /// Gets the node 's unique hash code
        /// </summary>
        /// <returns>A hashcode</returns>
        public virtual int GetNodeHashCode()
        {
            return ToXPath().GetHashCode();
        }

        #endregion

        #region IXaclObject

        /// <summary>
        /// Return a xpath representation of the object
        /// </summary>
        /// <returns>a xapth representation</returns>
        public string ToXPath()
        {
            return "siteMapNodes";
        }

        /// <summary>
        /// Return a  parent of the object
        /// </summary>
        /// <returns>The parent of the object</returns>
        public IXaclObject Parent
        {
            get
            {
                return (IXaclObject) this.ParentNode;
            }
        }

        /// <summary>
        /// Return a  of children of the object
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public IEnumerator GetChildren()
        {
            // return an empty enumerator
            return this.GetEnumerator();
        }

        #endregion
	}
}