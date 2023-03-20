/*
* @(#)SiteMapNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.IO;
	using System.Xml;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;

	/// <summary> 
	/// Represent a sitemap node
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009</version>
	public class SiteMapNode : SiteMapNodeBase
	{
        private string _imageUrl;
        private string _url;
        private string _database;
        private string _className;
        private string _sideMenuPath;
        private string _xpath;
        private bool _isVisible;
        private string _popupSettings;
        private bool _showNotification;
        private string _notificationHandler;
        private SiteMapNodeCollection _parameters;
        private string _helpDoc;
        private string _baseUrl;

		/// <summary>
		/// Initiate an instance of SiteMapNode class
		/// </summary>
		public SiteMapNode()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of SiteMapNode class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SiteMapNode(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the url associated with the node.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The url of the site map node"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Gets or sets the image url of the menu item.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("The image url of the menu"),
        ]
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify the database associated with site map node."),
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
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Specify the class associated with site map node."),
            DefaultValueAttribute(null),
            TypeConverterAttribute("Newtera.SiteMapStudio.ClassNameConverter, SiteMapStudio"),
            EditorAttribute("Newtera.SiteMapStudio.ClassNamePropertyEditor, SiteMapStudio", typeof(UITypeEditor))
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
        /// Gets or sets the paramters for passing to the node's module
        /// </summary>
        /// <value>
        /// An collection of StateParameters objects
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("A collection of parameters passing to the node's module"),
            EditorAttribute("Newtera.SiteMapStudio.StateParameterCollectionEditor, SiteMapStudio", typeof(UITypeEditor)),
        ]
        public SiteMapNodeCollection StateParameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the path of a side menu associated with the node.
        /// </summary>
        /// <remarks>Obsolete</remarks>
        [
            BrowsableAttribute(false),
            CategoryAttribute("System"),
            DescriptionAttribute("Specify the side menu associated with site map node."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.SiteMapStudio.SideMenuPathPropertyEditor, SiteMapStudio", typeof(UITypeEditor))
        ]
        public string SideMenuPath
        {
            get
            {
                return _sideMenuPath;
            }
            set
            {
                _sideMenuPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the node is visible
        /// </summary>
        /// <remarks>Obsolete</remarks>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Indicate whether to show the custom page in a popup?"),
            DefaultValueAttribute(true)
        ]
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the image url of the menu item.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("The online help document"),
        ]
        public string HelpDoc
        {
            get
            {
                return _helpDoc;
            }
            set
            {
                _helpDoc = value;
            }
        }

        /// <summary>
        /// Gets or sets the image url of the menu item.
        /// </summary>
        [
        BrowsableAttribute(false),
        CategoryAttribute("System"),
        DescriptionAttribute("The base URL of the app server"),
        ]
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the event handler for the custom command.
        /// </summary>
        /// <remarks>Obsolete</remarks>
        [
            BrowsableAttribute(false),
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The custom page popup settings, for eample, width,height,top,left"),
            DefaultValueAttribute(null)
        ]
        public string PopupSettings
        {
            get
            {
                return _popupSettings;
            }
            set
            {
                _popupSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether to show the notification on the dashboard icon
        /// </summary>
        /// <remarks>Obsolete</remarks>
        [
            BrowsableAttribute(false),
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Indicate whether to show a red circle notification on the dashboard icon?"),
            DefaultValueAttribute(false)
        ]
        public bool ShowNotification
        {
            get
            {
                return _showNotification;
            }
            set
            {
                _showNotification = value;
            }
        }

        /// <summary>
        /// Gets or sets the url of the handler that gets the number to display in the notification circle
        /// </summary>
        /// <remarks>Obsolete</remarks>
        [
            BrowsableAttribute(false),
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The url of the handler that gets the number to display in the notification circle"),
            DefaultValueAttribute(null)
        ]
        public string NotificationHandler
        {
            get
            {
                return _notificationHandler;
            }
            set
            {
                _notificationHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
            CategoryAttribute("System"),
            ReadOnly(true),
            DescriptionAttribute("The menu item hash code"),
        ]
        public string HashCode
        {
            get
            {
                return this.GetNodeHashCode().ToString();
            }
        }

        /// <summary>
        /// Gets the information indicating whether site map node is one of the main menu item. A
        /// main menu item shown on web as a tab
        /// </summary>
        [BrowsableAttribute(false)]	
        public bool IsMainMenuItem
        {
            get
            {
                if (this.ParentNode != null &&
                    this.ParentNode.ParentNode != null &&
                    (this.ParentNode.ParentNode is SiteMap ||
                     this.ParentNode.ParentNode is SiteMapModel))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// Initialize the instance
		/// </summary>
		private void Init()
		{
            _url = null;
            _database = null;
            _className = null;
            _sideMenuPath = null;
            _imageUrl = null;
            _xpath = null;
            _isVisible = true;
            _popupSettings = null;
            _showNotification = false;
            _notificationHandler = null;
            _parameters = new SiteMapNodeCollection();
            _helpDoc = null;
            _baseUrl = null;
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType	
		{
			get
			{
                return NodeType.siteMapNode;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitSiteMapNode(this))
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

            if (parent.GetAttribute("url") != null)
            {
                _url = parent.GetAttribute("url");
            }

            if (parent.GetAttribute("database") != null)
            {
                _database = parent.GetAttribute("database");
            }

            if (parent.GetAttribute("cls") != null)
            {
                _className = parent.GetAttribute("cls");
            }

            if (parent.GetAttribute("sidemenu") != null)
            {
                _sideMenuPath = parent.GetAttribute("sidemenu");
            }

            if (parent.GetAttribute("ImageUrl") != null)
            {
                _imageUrl = parent.GetAttribute("ImageUrl");
            }

            string isVisible = parent.GetAttribute("IsVisible");
            if (!string.IsNullOrEmpty(isVisible) && isVisible == "false")
            {
                _isVisible = false;
            }
            else
            {
                _isVisible = true; // default is true
            }

            if (parent.GetAttribute("PopupSettings") != null)
            {
                _popupSettings = parent.GetAttribute("PopupSettings");
            }

            string str = parent.GetAttribute("ShowNotification");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _showNotification = true;
            }
            else
            {
                _showNotification = false; // default is false
            }

            if (parent.GetAttribute("NotificationHandler") != null)
            {
                _notificationHandler = parent.GetAttribute("NotificationHandler");
            }

            if (parent.GetAttribute("HelpDoc") != null)
            {
                _helpDoc = parent.GetAttribute("HelpDoc");
            }

            if (parent.GetAttribute("BaseUrl") != null)
            {
                _baseUrl = parent.GetAttribute("BaseUrl");
            }

            SetStateParameters(parent);
        }

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
		{
            base.Marshal(parent);

            if (!string.IsNullOrEmpty(_url))
            {
                parent.SetAttribute("url", _url);
            }
            if (!string.IsNullOrEmpty(_database))
            {
                parent.SetAttribute("database", _database);
            }

            if (!string.IsNullOrEmpty(_className))
            {
                parent.SetAttribute("cls", _className);
            }

            if (!string.IsNullOrEmpty(_sideMenuPath))
            {
                parent.SetAttribute("sidemenu", _sideMenuPath);
            }

            if (!string.IsNullOrEmpty(_imageUrl))
            {
                parent.SetAttribute("ImageUrl", _imageUrl);
            }

            if (!_isVisible)
            {
                parent.SetAttribute("IsVisible", "false");
            }

            if (!string.IsNullOrEmpty(_popupSettings))
            {
                parent.SetAttribute("PopupSettings", _popupSettings);
            }

            if (_showNotification)
            {
                parent.SetAttribute("ShowNotification", "true");
            }

            if (!string.IsNullOrEmpty(_notificationHandler))
            {
                parent.SetAttribute("NotificationHandler", _notificationHandler);
            }

            if (!string.IsNullOrEmpty(_helpDoc))
            {
                parent.SetAttribute("HelpDoc", _helpDoc);
            }

            if (!string.IsNullOrEmpty(_baseUrl))
            {
                parent.SetAttribute("BaseUrl", _baseUrl);
            }

            WriteStateParameters(parent);
        }

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Parent.ToXPath() + "/siteMapNode[@Name='" + this.Name + "']";
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
                _dpath = this.ParentNode.ToDisplayPath() + "/siteMapNode[@title='" + this.Title + "']";
            }

            return _dpath;
        }

        /// <summary>
        /// Read and create the state parameters
        /// </summary>
        /// <param name="parent">The xml element</param>
        private void SetStateParameters(XmlElement parent)
        {
            _parameters.Clear();

            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                NodeType type = (NodeType)Enum.Parse(typeof(NodeType), xmlElement.Name);

                if (type == NodeType.Parameter)
                {
                    ISiteMapNode element = NodeFactory.Instance.Create(xmlElement);
                    element.ParentNode = this;

                    _parameters.Add(element);
                }
            }
        }

        private void WriteStateParameters(XmlElement parent)
        {
            XmlElement child;
            List<XmlElement> parameterElements = new List<XmlElement>();

            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                NodeType type = (NodeType)Enum.Parse(typeof(NodeType), xmlElement.Name);

                if (type == NodeType.Parameter)
                {
                    parameterElements.Add(xmlElement);
                }
            }

            // remove existing parameters from children nodes, otherwise, they will be duplicated
            foreach (XmlElement parameter in parameterElements)
            {
                parent.RemoveChild(parameter);
            }

            foreach (ISiteMapNode node in _parameters)
            {
                child = parent.OwnerDocument.CreateElement(NodeFactory.Instance.ConvertTypeToString(node.NodeType));
                node.Marshal(child);
                parent.AppendChild(child);
            }
        }
    }
}