/*
* @(#)CustomCommand.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
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
	/// Represent a CustomCommand node
	/// </summary>
	/// <version> 1.0.0 27 Nov 2010</version>
	public class CustomCommand : SiteMapNodeBase
	{
        private string _imageUrl;
        private string _navigationUrl;
        private string _xpath;
        private string _eventHandler;
        private string _visibleCondition;
        private bool _showInPopup;
        private string _popupSettings;
        private CommandScope _commandScope;
        private SiteMapNodeCollection _parameters;
        private string _baseUrl;

        /// <summary>
        /// Initiate an instance of CustomCommand class
        /// </summary>
        public CustomCommand()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of CustomCommand class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal CustomCommand(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the navigation url of the custom command.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("The navigation url of the command"),
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
        /// Gets or sets the image url of the custom command.
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
        /// Gets or sets the event handler for the custom command.
        /// </summary>
        [
        BrowsableAttribute(false),
        CategoryAttribute("System"),
        DescriptionAttribute("The event handler for the custom command"),
        ]
        public string EventHanlder
        {
            get
            {
                return _eventHandler;
            }
            set
            {
                _eventHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the base url of app server.
        /// </summary>
        [
        BrowsableAttribute(false),
        CategoryAttribute("System"),
        DescriptionAttribute("The base url of an app server"),
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
        [
        CategoryAttribute("System"),
        DescriptionAttribute("The visible condition for the command"),
        EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string VisibleCondition
        {
            get
            {
                return _visibleCondition;
            }
            set
            {
                _visibleCondition = value;
            }
        }

        /// <summary>
        /// Gets or sets the navigation url of the custom command.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("The applicable scope of the command"),
        ]
        public CommandScope CommandScope
        {
            get
            {
                return _commandScope;
            }
            set
            {
                _commandScope = value;
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
        [BrowsableAttribute(false)]
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
            _eventHandler = null;
            _visibleCondition = null;
            _commandScope = CommandScope.Instance;
            _showInPopup = true;
            _popupSettings = null;
            _parameters = new SiteMapNodeCollection();
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
                return NodeType.CustomCommand;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitCustomCommand(this))
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

            if (parent.GetAttribute("EventHandler") != null)
            {
                _eventHandler = parent.GetAttribute("EventHandler");
            }

            if (parent.GetAttribute("VisibleCondition") != null)
            {
                _visibleCondition = parent.GetAttribute("VisibleCondition");
            }
            string scope = parent.GetAttribute("Scope");
            if (!string.IsNullOrEmpty(scope))
            {
                _commandScope = (CommandScope)Enum.Parse(typeof(CommandScope), scope);
            }
            else
            {
                _commandScope = CommandScope.Instance;
            }

            string showInPopup = parent.GetAttribute("ShowInPopup");
            if (!string.IsNullOrEmpty(showInPopup) && showInPopup == "false")
            {
                _showInPopup = false;
            }
            else
            {
                _showInPopup = true; // default is true
            }

            if (parent.GetAttribute("PopupSettings") != null)
            {
                _popupSettings = parent.GetAttribute("PopupSettings");
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

            if (!string.IsNullOrEmpty(_navigationUrl))
            {
                parent.SetAttribute("NavigateUrl", _navigationUrl);
            }

            if (!string.IsNullOrEmpty(_imageUrl))
            {
                parent.SetAttribute("ImageUrl", _imageUrl);
            }

            if (!string.IsNullOrEmpty(_eventHandler))
            {
                parent.SetAttribute("EventHandler", _eventHandler);
            }

            if (!string.IsNullOrEmpty(_visibleCondition))
            {
                parent.SetAttribute("VisibleCondition", _visibleCondition);
            }

            if (_commandScope != CommandScope.Instance)
            {
                parent.SetAttribute("Scope", Enum.GetName(typeof(CommandScope), _commandScope));
            }

            if (!_showInPopup)
            {
                parent.SetAttribute("ShowInPopup", "false");
            }

            if (!string.IsNullOrEmpty(_popupSettings))
            {
                parent.SetAttribute("PopupSettings", _popupSettings);
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
                _xpath = this.Parent.ToXPath() + "/CustomCommand[@Name='" + this.Name + "']";
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
                _dpath = this.ParentNode.ToDisplayPath() + "/CustomCommand[@Text='" + this.Title + "']";
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

    // define the scope of the command
    public enum CommandScope
    {
        Instance,
        Class
    }
}