/*
* @(#)SideMenuGroup.cs
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
	/// Represent a menu group node
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009</version>
	public class SideMenuGroup : SiteMapNodeBase
	{
        private string _database;
        private string _xpath;

		/// <summary>
		/// Initiate an instance of SideMenuGroup class
		/// </summary>
		public SideMenuGroup()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of SideMenuGroup class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SideMenuGroup(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify the database associated with menu group."),
        DefaultValueAttribute(null),
        EditorAttribute("Newtera.SiteMapStudio.DatabasePropertyEditor, SiteMapStudio", typeof(UITypeEditor))
        ]
        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                _database = value;
            }
        }

		/// <summary>
		/// Initialize the instance
		/// </summary>
        private void Init()
        {
            _database = null;
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
                return NodeType.SideMenuGroup;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitSideMenuGroup(this))
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

            if (parent.GetAttribute("database") != null)
            {
                _database = parent.GetAttribute("database");
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
		{
            base.Marshal(parent);

            if (!string.IsNullOrEmpty(_database))
            {
                parent.SetAttribute("database", _database);
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
                _xpath = this.Parent.ToXPath() + "/SideMenuGroup[@Name='" + this.Name + "']";
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
                _dpath = this.ParentNode.ToDisplayPath() + "/SideMenuGroup[@Text='" + this.Title + "']";
            }

            return _dpath;
        }

        public string ToPath()
        {
            return @"/SideMenu/SideMenuGroup[@Name='" + this.Name + "']/*";
        }
	}
}