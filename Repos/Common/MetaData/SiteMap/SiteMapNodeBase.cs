/*
* @(#)SiteMapNodeBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.IO;
	using System.Xml;
    using System.ComponentModel;
    using System.Collections;

    using Newtera.Common.MetaData.XaclModel;

	/// <summary> 
	/// Represent a base node for all nodes in the package
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009</version>
    public class SiteMapNodeBase : ISiteMapNode, ICustomTypeDescriptor
	{
		private ISiteMapNode _parent;
        private SiteMapNodeCollection _children;
        private string _name;
        private string _text;
		private string _desc;
        protected string _dpath;

        /// <summary>
        /// Title changed handler
        /// </summary>
        public event EventHandler TitleChanged;

        private PropertyDescriptorCollection _globalizedProperties = null;

		/// <summary>
		/// Initiate an instance of SiteMapNodeBase class
		/// </summary>
		public SiteMapNodeBase()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of SiteMapNodeBase class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SiteMapNodeBase(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Initialize the instance
		/// </summary>
		private void Init()
		{
		    _parent = null;
            _children = new SiteMapNodeCollection();
            _name = null;
            _text = null;
		    _desc = null;
            _dpath = null;
		}

		#region ISiteMapNode interface implementation

        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The unique name of the item"),
            ReadOnlyAttribute(true)
        ]
        public virtual string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    return _name;
                }
                else
                {
                    // for backward compatibility purpose. Name is added at 4.3.3 version
                    if (!string.IsNullOrEmpty(_text))
                    {
                        _name = _text;

                        return _name;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the node title
        /// </summary>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The display name of the item"),
        ]
        public virtual string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(_text))
                {
                    return _text;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                _text = value;

                // Raise the event for Title change
                if (TitleChanged != null)
                {
                    TitleChanged(this, new ValueChangedEventArgs("Title", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the node description.
        /// </summary>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The description of the item"),
        ]
        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent node
        /// </summary>
        [BrowsableAttribute(false)]		
        public ISiteMapNode ParentNode
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>A SiteMapNodeCollection</value>
        [BrowsableAttribute(false)]		
        public SiteMapNodeCollection ChildNodes
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
            }
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
        [BrowsableAttribute(false)]		
        public virtual NodeType NodeType	
		{
			get
			{
                return NodeType.Unknown;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public virtual void Accept(ISiteMapNodeVisitor visitor)
        {
        }

        /// <summary>
        /// Add a node as a child
        /// </summary>
        /// <param name="child"></param>
        public void AddChildNode(ISiteMapNode child)
        {
            if (this.ChildNodes != null)
            {
                this.ChildNodes.Add(child);
                child.ParentNode = this;
            }
        }

        /// <summary>
        /// Delete a child node
        /// </summary>
        /// <param name="child"></param>
        public void DeleteChildNode(ISiteMapNode child)
        {
            if (this.ChildNodes != null)
            {
                this.ChildNodes.Remove(child);
                child.ParentNode = null;
            }
        }
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			_name = parent.GetAttribute("Name");
            _text = parent.GetAttribute("title");
            if (string.IsNullOrEmpty(_text))
            {
                _text = parent.GetAttribute("Text");
            }
            
            if (parent.GetAttribute("description") != null)
            {
                _desc = parent.GetAttribute("description");
            }

			foreach (XmlElement child in parent.ChildNodes)
			{
				ISiteMapNode node = NodeFactory.Instance.Create(child);

                node.ParentNode = this;
				this._children.Add(node); 
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			parent.SetAttribute("title", Title); // title is used by the web sitemap
            parent.SetAttribute("Text", Title); // Text is used by sitemap studio
            parent.SetAttribute("Name", Name); // Name is an unique name
            if (!string.IsNullOrEmpty(_desc))
            {
                parent.SetAttribute("description", _desc);
            }

			XmlElement child;

			foreach (ISiteMapNode node in _children)
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
            return "";
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
        public virtual string ToXPath()
        {
            return "";
        }

        /// <summary>
        /// Return a  parent of the object
        /// </summary>
        /// <returns>The parent of the object</returns>
        [BrowsableAttribute(false)]	
        public IXaclObject Parent
        {
            get
            {
                return (IXaclObject)this.ParentNode;
            }
        }

        /// <summary>
        /// Return a  of children of the object
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public IEnumerator GetChildren()
        {
            return this.ChildNodes.GetEnumerator();
        }

        #endregion

        #region ICustomTypeDescriptor

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Called to get the properties of a type.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to be globalized
                foreach (PropertyDescriptor property in baseProps)
                {
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Our implementation overrides GetProperties() only and creates a
        /// collection of custom property descriptors of type GlobalizedPropertyDescriptor
        /// and returns them to the caller instead of the default ones.
        /// </summary>
        /// <returns>A collection of Property Descriptors.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            // Only do once
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProperties = TypeDescriptor.GetProperties(this, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to 
                // be globalized
                foreach (PropertyDescriptor property in baseProperties)
                {
                    // create our custom property descriptor and add it to the collection
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}