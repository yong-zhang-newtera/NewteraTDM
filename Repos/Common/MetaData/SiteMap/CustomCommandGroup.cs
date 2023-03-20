/*
* @(#)CustomCommandGroup.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.IO;
	using System.Xml;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;

	/// <summary> 
	/// Represent a command group node
	/// </summary>
	/// <version> 1.0.0 27 Nov 2010</version>
	public class CustomCommandGroup : SiteMapNodeBase
	{
        private string _database;
        private string _className;
        private string _xpath;

		/// <summary>
		/// Initiate an instance of CustomCommandGroup class
		/// </summary>
		public CustomCommandGroup()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of CustomCommandGroup class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal CustomCommandGroup(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify the database associated with command group."),
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
        /// Gets or sets the class name associated with the node.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify name of the class associated with command group."),
        DefaultValueAttribute(null),
        TypeConverterAttribute("Newtera.SiteMapStudio.ClassNameConverter, SiteMapStudio"),
        EditorAttribute("Newtera.SiteMapStudio.ClassNamePropertyEditor, SiteMapStudio", typeof(UITypeEditor)),
        ]
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

		/// <summary>
		/// Initialize the instance
		/// </summary>
        private void Init()
        {
            _database = null;
            _className = null;
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
                return NodeType.CustomCommandGroup;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitCustomCommandGroup(this))
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

            if (parent.GetAttribute("className") != null)
            {
                _className = parent.GetAttribute("className");
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

            if (!string.IsNullOrEmpty(_className))
            {
                parent.SetAttribute("className", _className);
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
                _xpath = this.Parent.ToXPath() + "/CustomCommandGroup[@Name='" + this.Name + "']";
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
                _dpath = this.ParentNode.ToDisplayPath() + "/CustomCommandGroup[@Text='" + this.Title + "']";
            }

            return _dpath;
        }

        public string ToPath()
        {
            return @"/SideMenu/CustomCommandGroup[@Name='" + this.Name + "']/*";
        }
	}
}