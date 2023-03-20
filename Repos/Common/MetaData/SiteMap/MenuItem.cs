/*
* @(#)MenuItem.cs
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
	/// Represent a menuitem node
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009</version>
	public class MenuItem : SiteMapNodeBase
	{
        private string _imageUrl;
        private string _navigationUrl;
        private string _xpath;
        private bool _showInPopup;
        private string _popupSettings;

		/// <summary>
		/// Initiate an instance of MenuItem class
		/// </summary>
		public MenuItem()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of MenuItem class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal MenuItem(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the navigation url of the menu item.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The navigation url of the menu"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string NavigationUrl
        {
            get
            {
                return _navigationUrl;
            }
            set
            {
                _navigationUrl = value;
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
        [BrowsableAttribute(false)]
        public bool ShowInPopup
        {
            get
            {
                return _showInPopup;
            }
            set
            {
                _showInPopup = value;
            }
        }

        /// <summary>
        /// Gets or sets the event handler for the custom command.
        /// </summary>
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
		/// Initialize the instance
		/// </summary>
		private void Init()
		{
            _navigationUrl = null;
            _imageUrl = null;
            _xpath = null;
            _showInPopup = false;
            _popupSettings = null;
		}

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType	
		{
			get
			{
                return NodeType.MenuItem;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitMenuItem(this))
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

            if (parent.GetAttribute("NavigateUrl") != null)
            {
                _navigationUrl = parent.GetAttribute("NavigateUrl");
            }

            if (parent.GetAttribute("ImageUrl") != null)
            {
                _imageUrl = parent.GetAttribute("ImageUrl");
            }

            string showInPopup = parent.GetAttribute("ShowInPopup");
            if (!string.IsNullOrEmpty(showInPopup) && showInPopup == "true")
            {
                _showInPopup = true;
            }
            else
            {
                _showInPopup = false; // default is false
            }

            if (parent.GetAttribute("PopupSettings") != null)
            {
                _popupSettings = parent.GetAttribute("PopupSettings");
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
		{
            base.Marshal(parent);

            if (!string.IsNullOrEmpty(_navigationUrl))
            {
                parent.SetAttribute("NavigateUrl", _navigationUrl);
            }

            if (!string.IsNullOrEmpty(_imageUrl))
            {
                parent.SetAttribute("ImageUrl", _imageUrl);
            }

            if (_showInPopup)
            {
                parent.SetAttribute("ShowInPopup", "true");
            }

            if (!string.IsNullOrEmpty(_popupSettings))
            {
                parent.SetAttribute("PopupSettings", _popupSettings);
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
                _xpath = this.Parent.ToXPath() + "/MenuItem[@Name='" + this.Name + "']";
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
                _dpath = this.ParentNode.ToDisplayPath() + "/MenuItem[@Text='" + this.Title + "']";
            }

            return _dpath;
        }
	}
}