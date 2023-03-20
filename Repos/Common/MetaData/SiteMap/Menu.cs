/*
* @(#)Menu.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.IO;
	using System.Xml;
    using System.ComponentModel;
    using System.Drawing.Design;

	/// <summary> 
	/// Represent a menu node
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009</version>
	public class Menu : SiteMapNodeBase
	{
        private MenuType _type;
        private string _hierarchyName;
        private string _xpath;

		/// <summary>
		/// Initiate an instance of Menu class
		/// </summary>
		public Menu()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of Menu class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Menu(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the type of the menu.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Type of the menu"),
        DefaultValueAttribute(MenuType.Unknown)
        ]	
        public MenuType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the metadata hierarchy associated with the menu.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify the name of the metadata hierarchy associated with the menu when the type is Dashboard or Tree."),
        DefaultValueAttribute(null),
        EditorAttribute("Newtera.SiteMapStudio.HierarchyNamePropertyEditor, SiteMapStudio", typeof(UITypeEditor))
        ]
        public string HierarchyName
        {
            get
            {
                return _hierarchyName;
            }
            set
            {
                _hierarchyName = value;
            }
        }


		/// <summary>
		/// Initialize the instance
		/// </summary>
		private void Init()
		{
            _type = MenuType.Unknown;
            _xpath = null;
		}

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType	
		{
			get
			{
                return NodeType.Menu;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitMenu(this))
            {
                foreach (ISiteMapNode child in ChildNodes)
                {
                    child.Accept(visitor);
                }
            }
        }
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
            base.Unmarshal(parent);

            if (parent.GetAttribute("Type") != null)
            {
                _type = (MenuType)Enum.Parse(typeof(MenuType), parent.GetAttribute("Type"));
            }

            if (parent.GetAttribute("Name") != null)
            {
                _hierarchyName = parent.GetAttribute("Name");
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
		{
            base.Marshal(parent);

            if (_type != MenuType.Unknown)
            {
                parent.SetAttribute("Type", Enum.GetName(typeof(MenuType), _type));
            }

            if (!string.IsNullOrEmpty(_hierarchyName))
            {
                parent.SetAttribute("Name", _hierarchyName);
            }
		}

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Parent.ToXPath() + "/Menu[@Name='" + this.Name + "']";
            }

            return _xpath;
        }

        /// <summary>
        /// Return a displayed title path representation of the ISiteMapNode
        /// </summary>
        /// <returns>a title path representation</returns>
        public override string ToDisplayPath()
        {
            if (_dpath == null)
            {
                _dpath = this.ParentNode.ToDisplayPath() + "/Menu[@Text='" + this.Title + "']";
            }

            return _dpath;
        }
	}

    /// <summary>
	/// Specify the types of menu
	/// </summary>
    public enum MenuType
    {
        /// <summary>
        /// Unknown menu type
        /// </summary>
        Unknown,
        /// <summary>
        /// Keywords type menu
        /// </summary>
        Keywords,
        /// <summary>
        /// Trees
        /// </summary>
        Trees,
        /// <summary>
        /// Actions
        /// </summary>
        Actions,
        /// <summary>
        /// Classification
        /// </summary>
        Tree,
        /// <summary>
        /// Dashboard type menu
        /// </summary>
        Dashboard
    }
}