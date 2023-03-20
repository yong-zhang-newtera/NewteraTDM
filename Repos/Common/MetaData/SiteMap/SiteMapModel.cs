/*
* @(#)SiteMapModel.cs
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
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.SiteMap.Validate;

	/// <summary>
	/// Represents a sitemap model
	/// </summary>
	/// <version> 1.0.0 21 Jun 2009 </version>
    public class SiteMapModel : SiteMapNodeBase
    {
        public const string DEFAULT_MODEL_NAME = "Default";
        private const string SITE_MAP_FILE = @"web_{0}.sitemap";
        private const string SIDE_MENU_FILE = @"side_menu_defs_{0}.xml";
        private const string CUSTOM_COMMAND_FILE = @"custom_command_defs_{0}.xml";
        private const string XACL_POLICY_FILE = @"xacl_{0}.xml";

        private string _siteMapFile = null;
        private string _sideMenuFile = null;
        private string _customCommandSetFile = null;
        private string _policyFile = null;
        private SiteMap _siteMap;
        private SideMenu _sideMenu;
        private CustomCommandSet _customCommandSet;
        private XaclPolicy _policy;
        private string _siteMapXml = null;
        private string _sideMenuXml = null;
        private string _customCommandSetXml = null;
        private string _policyXml = null;
        private string _siteDatabase = null;
        private string _siteLanguage = null;

        private Hashtable _customCommandGroupTable;

		/// <summary>
		///  Initializes a new instance of the SiteMapModel class.
		/// </summary>
		public SiteMapModel() : base()
		{
            _siteMap = new SiteMap();
            _siteMap.ParentNode = this;
            _sideMenu = new SideMenu();
            _sideMenu.ParentNode = this;
            _customCommandSet = new CustomCommandSet();
            _customCommandSet.ParentNode = this;
            _policy = new XaclPolicy();
		}

        /// <summary>
		/// Initiating an instance of SiteMapModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal SiteMapModel(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

        [BrowsableAttribute(false)]	
        public SiteMap SiteMap
        {
            get
            {
                return _siteMap;
            }
            set
            {
                _siteMap = value;
                if (_siteMap != null)
                {
                    _siteMap.ParentNode = this;
                }
            }
        }

        [BrowsableAttribute(false)]
        public string SiteMapXml
        {
            get
            {
                return _siteMapXml;
            }
            set
            {
                _siteMapXml = value;
            }
        }

        [BrowsableAttribute(false)]	
        public SideMenu SideMenu
        {
            get
            {
                return _sideMenu;
            }
            set
            {
                _sideMenu = value;
                if (_sideMenu != null)
                {
                    _sideMenu.ParentNode = this;
                }
            }
        }

        [BrowsableAttribute(false)]
        public string SideMenuXml
        {
            get
            {
                return _sideMenuXml;
            }
            set
            {
                _sideMenuXml = value;
            }
        }

        [BrowsableAttribute(false)]
        public CustomCommandSet CustomCommandSet
        {
            get
            {
                return _customCommandSet;
            }
            set
            {
                _customCommandSet = value;
                if (_customCommandSet != null)
                {
                    _customCommandSet.ParentNode = this;
                }
            }
        }

        [BrowsableAttribute(false)]
        public string CustomCommandSetXml
        {
            get
            {
                return _customCommandSetXml;
            }
            set
            {
                _customCommandSetXml = value;
            }
        }

        [BrowsableAttribute(false)]	
        public XaclPolicy Policy
        {
            get
            {
                return _policy;
            }
            set
            {
                _policy = value;
            }
        }

        [BrowsableAttribute(false)]
        public string PolicyXml
        {
            get
            {
                return _policyXml;
            }
            set
            {
                _policyXml = value;
            }
        }

        [
             CategoryAttribute("System"),
             DescriptionAttribute("Name of the sitemap file"),
        ]
        public string SiteMapFile
        {
            get
            {
                if (string.IsNullOrEmpty(_siteMapFile))
                {
                    return SITE_MAP_FILE.Replace("{0}", this.Name);
                }
                else
                {
                    return _siteMapFile;
                }
            }
            set
            {
                _siteMapFile = value;
            }
        }

        [
             BrowsableAttribute(false),
             CategoryAttribute("System"),
             DescriptionAttribute("Name of the side menu file"),
        ]
        public string SideMenuFile
        {
            get
            {
                if (string.IsNullOrEmpty(_sideMenuFile))
                {
                    return SIDE_MENU_FILE.Replace("{0}", this.Name);
                }
                else
                {
                    return _sideMenuFile;
                }
            }
            set
            {
                _sideMenuFile = value;
            }
        }

        [
             CategoryAttribute("System"),
             DescriptionAttribute("Name of the Custom command set file"),
        ]
        public string CustomCommandSetFile
        {
            get
            {
                if (string.IsNullOrEmpty(_customCommandSetFile))
                {
                    return CUSTOM_COMMAND_FILE.Replace("{0}", this.Name);
                }
                else
                {
                    return _customCommandSetFile;
                }
            }
            set
            {
                _customCommandSetFile = value;
            }
        }

        [
             CategoryAttribute("System"),
             DescriptionAttribute("Name of the xacl policy file"),
        ]
        public string PolicyFile
        {
            get
            {
                if (string.IsNullOrEmpty(_policyFile))
                {
                    return XACL_POLICY_FILE.Replace("{0}", this.Name);
                }
                else
                {
                    return _policyFile;
                }
            }
            set
            {
                _policyFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the database associated with the node.
        /// </summary>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Specify the database associated with site map."),
        DefaultValueAttribute(null),
        EditorAttribute("Newtera.SiteMapStudio.DatabasePropertyEditor, SiteMapStudio", typeof(UITypeEditor))
        ]
        public string Database
        {
            get
            {
                return _siteDatabase;
            }
            set
            {
                _siteDatabase = value;
            }
        }

        [
          CategoryAttribute("System"),
          DescriptionAttribute("Deafult languange of the site"),
        ]
        public string Language
        {
            get
            {
                return _siteLanguage;
            }
            set
            {
                _siteLanguage = value;
            }
        }

        /// <summary>
        /// For fast access of custom command groups
        /// </summary>
        [BrowsableAttribute(false)]	
        public Hashtable CustomCommandGroupTable
        {
            get
            {
                return _customCommandGroupTable;
            }
            set
            {
                _customCommandGroupTable = value;
            }
        }

        [BrowsableAttribute(false)]
        public bool AllowDelete
        {
            get
            {
                if (this.Name == DEFAULT_MODEL_NAME)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #region ISiteMapNode members

        /// <summary>
        /// Gets the type of Node
        /// </summary>
        /// <value>One of NodeType values</value>
        public override NodeType NodeType
        {
            get
            {
                return NodeType.Model;
            }
        }

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitSiteMapModel(this))
            {
                this._siteMap.Accept(visitor);

                this._sideMenu.Accept(visitor);

                this._customCommandSet.Accept(visitor);
            }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            if (parent.GetAttribute("sitemapfile") != null)
            {
                _siteMapFile = parent.GetAttribute("sitemapfile");
            }

            if (parent.GetAttribute("sidemenufile") != null)
            {
                _sideMenuFile = parent.GetAttribute("sidemenufile");
            }

            if (parent.GetAttribute("customcommandfile") != null)
            {
                _customCommandSetFile = parent.GetAttribute("customcommandfile");
            }

            if (parent.GetAttribute("policyfile") != null)
            {
                _policyFile = parent.GetAttribute("policyfile");
            }

            if (parent.GetAttribute("database") != null)
            {
                _siteDatabase = parent.GetAttribute("database");
            }

            if (parent.GetAttribute("language") != null)
            {
                _siteLanguage = parent.GetAttribute("language");
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

            if (!string.IsNullOrEmpty(SiteMapFile))
            {
                parent.SetAttribute("sitemapfile", SiteMapFile);
            }

            if (!string.IsNullOrEmpty(SideMenuFile))
            {
                parent.SetAttribute("sidemenufile", SideMenuFile);
            }

            if (!string.IsNullOrEmpty(CustomCommandSetFile))
            {
                parent.SetAttribute("customcommandfile", CustomCommandSetFile);
            }

            if (!string.IsNullOrEmpty(PolicyFile))
            {
                parent.SetAttribute("policyfile", PolicyFile);
            }

            if (!string.IsNullOrEmpty(Database))
            {
                parent.SetAttribute("database", Database);
            }

            if (!string.IsNullOrEmpty(Language))
            {
                parent.SetAttribute("language", Language);
            }
        }

        #endregion

        /// <summary>
        /// Validate the schema model to see if it confirm to schema model integrity
        /// rules.
        /// </summary>
        /// <returns>The result in ValidateResult object</returns>
        public ValidateResult Validate()
        {
            SiteMapModelValidateVisitor visitor = new SiteMapModelValidateVisitor(this);

            Accept(visitor); // start validating

            return visitor.ValidateResult;
        }

        /// <summary>
        /// Return a displayed title path representation of the object
        /// </summary>
        /// <returns>a displayed path representation</returns>
        public override string ToDisplayPath()
        {
            return "/Model";
        }

        #region IXaclObject

        /// <summary>
        /// Return a xpath representation of the object
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            return "/Model";
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
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the object
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            children.Add(this.SiteMap);
            children.Add(this.SideMenu);
            children.Add(this.CustomCommandSet);
            return children.GetEnumerator();
        }

        #endregion
	}
}