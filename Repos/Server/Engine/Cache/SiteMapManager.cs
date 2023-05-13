/*
* @(#)SiteMapManager.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.Xml;
    using System.IO;
    using System.Text;
	using System.Collections;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.SiteMap;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary> 
	/// Single point of keeping sitemap and sidemenu info
	/// </summary>
	/// <version>  	1.0.0 15 Jun 2009</version>
	public class SiteMapManager
	{
        private const string SiteMapModelsFile = @"sitemap_defs.xml";
        private const string SiteMapFile = @"web.sitemap";
        private const string ConfigDir = @"\Config\";
        private const string SiteMapDir = @"\Config\Sitemaps\";
        private const string SideMenuFile = @"side_menu_defs.xml";
        private const string CustomCommandFile = @"custom_command_defs.xml";
        private const string XaclPolicyFile = @"xacl.xml";

        private SiteMapModelSet _siteMapModelSet = null;
        private string _siteMapModelsXml = null;

        // Static cache object, all invokers will use this cache object.
        private static SiteMapManager theManager;

		/// <summary>
		/// Private constructor.
		/// </summary>
        private SiteMapManager()
		{
            Initialize();
		}

		/// <summary>
        /// Gets the SiteMapManager instance.
		/// </summary>
		/// <returns> The MetaDataCache instance.</returns>
        static public SiteMapManager Instance
		{
			get
			{
                return theManager;
			}
		}

        /// <summary>
        /// Gets site map model set
        /// </summary>
        public SiteMapModelSet SiteMapModelSet
        {
            get
            {
                return _siteMapModelSet;
            }
        }

        /// <summary>
        /// Gets the current sitemap model
        /// </summary>
        public SiteMapModel SiteMapModel
        {
            get
            {
                return SiteMapModelSet.CurrentSiteMapModel;
            }
        }

        /// <summary>
        /// Gets a model of the current selected site map
        /// </summary>
        public SiteMap SiteMap
        {
            get
            {
                return SiteMapModelSet.CurrentSiteMapModel.SiteMap;
            }
        }

        /// <summary>
        /// Gets a model of the current side menu
        /// </summary>
        public SideMenu SideMenu
        {
            get
            {
                return SiteMapModelSet.CurrentSiteMapModel.SideMenu;
            }
        }

        /// <summary>
        /// Gets a xml string of the current selected side menu
        /// </summary>
        public string SideMenuXml
        {
            get
            {
                return SiteMapModelSet.CurrentSiteMapModel.SideMenuXml;
            }
        }

        /// <summary>
        /// Gets a Custom Command Group given a schema id and class name from the current sitemap model object
        /// </summary>
        /// <param name="schemaId">schema id</param>
        /// <param name="className">class name</param>
        /// <returns></returns>
        public CustomCommandGroup GetCustomCommandGroup(string schemaId, string className)
        {
            CustomCommandGroup found = (CustomCommandGroup)SiteMapModelSet.CurrentSiteMapModel.CustomCommandGroupTable[schemaId + className];

            return found;
        }

        /// <summary>
        /// Gets a model of the access policy
        /// </summary>
        public XaclPolicy Policy
        {
            get
            {
                return SiteMapModelSet.CurrentSiteMapModel.Policy;
            }
        }

        /// <summary>
        /// Get sitemap models in xml string
        /// </summary>
        public string GetSiteMapModels()
        {
            return _siteMapModelsXml;
        }

        /// <summary>
        /// Write sitemap models xml to a file
        /// </summary>
        /// <param name="xmlString">sitemap xml</param>
        public void SetSiteMapModels(string xmlString)
        {
            lock (this)
            {
                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + SiteMapModelsFile;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                doc.Save(filePath);

                // initialize the cache
                InitializeSiteMapModels();
            }
        }

        /// <summary>
        /// Get server sitemap in xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        public string GetSiteMap(string modelName)
        {
            SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
            if (siteMapModel == null)
            {
                throw new Exception("Unable to find a site map model with name " + modelName);
            }
            return siteMapModel.SiteMapXml;
        }

        /// <summary>
        /// Get server sitemap in xml string from a file
        /// </summary>
        /// <param name="fileName">The name of a sitemap file</param>
        public string GetSiteMapFromFile(string fileName)
        {
            string xml = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + fileName;

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// Write sitemap xml to a file
        /// </summary>
        /// <param name="modelName">The model name, null for default model</param>
        /// <param name="xmlString">sitemap xml</param>
        public void SetSiteMap(string modelName, string xmlString)
        {
            lock (this)
            {
                SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
                if (siteMapModel == null)
                {
                    throw new Exception("Unable to find a site map model with name " + modelName);
                }

                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + siteMapModel.SiteMapFile;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                doc.Save(filePath);

                // initialize the cache
                InitializeSiteMap(siteMapModel, filePath);
            }
        }

        /// <summary>
        /// Get side menu definition in xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        public string GetSideMenu(string modelName)
        {
            SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
            if (siteMapModel == null)
            {
                throw new Exception("Unable to find a site map model with name " + modelName);
            }
            return siteMapModel.SideMenuXml;
        }

        /// <summary>
        /// Get server side menu in xml string from a file
        /// </summary>
        /// <param name="fileName">The name of a side menu file</param>
        public string GetSideMenuFromFile(string fileName)
        {
            string xml = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + fileName;

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// Write side menu xml to a file
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString"></param>
        public void SetSideMenu(string modelName, string xmlString)
        {
            lock (this)
            {
                SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
                if (siteMapModel == null)
                {
                    throw new Exception("Unable to find a site map model with name " + modelName);
                }

                string theSiteMenuFile = siteMapModel.SideMenuFile;
                if (string.IsNullOrEmpty(theSiteMenuFile))
                {
                    // use the deafult side menu file name
                    theSiteMenuFile = SideMenuFile;
                }

                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + theSiteMenuFile;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                doc.Save(filePath);

                // Initialize side menu
                InitializeSideMenu(siteMapModel, filePath);
            }
        }

        /// <summary>
        /// Get custom command definition in xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        public string GetCustomCommandSet(string modelName)
        {
            SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
            if (siteMapModel == null)
            {
                throw new Exception("Unable to find a site map model with name " + modelName);
            }
            return siteMapModel.CustomCommandSetXml;
        }

        /// <summary>
        /// Get custom command set in xml string from a file
        /// </summary>
        /// <param name="fileName">The name of a custom command set file</param>
        public string GetCustomCommandSetFromFile(string fileName)
        {
            string xml = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + fileName;

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// Write a custom command set xml string to a file
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString"></param>
        public void SetCustomCommandSet(string modelName, string xmlString)
        {
            lock (this)
            {
                SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
                if (siteMapModel == null)
                {
                    throw new Exception("Unable to find a site map model with name " + modelName);
                }

                string theCustomCommandFile = siteMapModel.CustomCommandSetFile;
                if (string.IsNullOrEmpty(theCustomCommandFile))
                {
                    // use the deafult custom command file name
                    theCustomCommandFile = CustomCommandFile;
                }

                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + theCustomCommandFile;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                doc.Save(filePath);

                // clear the cached one
                InitializeCustomCommandSets(siteMapModel, filePath);
            }
        }

        /// <summary>
        /// Get xacl policy in xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        public string GetSiteMapPolicy(string modelName)
        {
            SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
            if (siteMapModel == null)
            {
                throw new Exception("Unable to find a site map model with name " + modelName);
            }
            return siteMapModel.PolicyXml;
        }

        /// <summary>
        /// Get xacl policy in xml string from a file
        /// </summary>
        /// <param name="fileName">The name of a policy file</param>
        public string GetSiteMapPolicyFromFile(string fileName)
        {
            string xml = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + fileName;

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// Write a sitemap access control policy xml string to a file
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString">The policy xml string</param>
        public void SetSiteMapPolicy(string modelName, string xmlString)
        {
            lock (this)
            {
                SiteMapModel siteMapModel = SiteMapModelSet.FindSiteMapModel(modelName);
                if (siteMapModel == null)
                {
                    throw new Exception("Unable to find a site map model with name " + modelName);
                }

                string theXaclPolicyFile = siteMapModel.PolicyFile;
                if (string.IsNullOrEmpty(theXaclPolicyFile))
                {
                    // use the deafult xacl policy file name
                    theXaclPolicyFile = XaclPolicyFile;
                }

                string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + theXaclPolicyFile;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                doc.Save(filePath);

                // clear the cached one
                InitializePolicy(siteMapModel, theXaclPolicyFile);
            }
        }

        private void Initialize()
        {
            InitializeSiteMapModels();
            InitializeSiteMaps();
            InitializeSideMenus();
            InitializeCustomCommandSets();
            InitializePolicies();
        }

        private void InitializeSiteMapModels()
        {
            // get sidemenu xml string
            string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + SiteMapModelsFile;

            XmlDocument doc;
            if (File.Exists(filePath))
            {
                doc = new XmlDocument();
                doc.Load(filePath);

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);
                _siteMapModelsXml = sw.ToString();
            }
            else
            {
                _siteMapModelsXml = null;
            }

            string xml = GetSiteMapModels();
            if (!string.IsNullOrEmpty(xml))
            {
                _siteMapModelSet = new SiteMapModelSet();
                StringReader reader = new StringReader(xml);
                _siteMapModelSet.Read(reader);
            }
            else
            {
                _siteMapModelSet = null;
            }
        }

        // load site map models in the memory to improve the performance
        private void InitializeSiteMaps()
        {
            if (SiteMapModelSet != null)
            {
                foreach (SiteMapModel siteMapModel in SiteMapModelSet.ChildNodes)
                {
                    string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + siteMapModel.SiteMapFile;

                    InitializeSiteMap(siteMapModel, filePath);
                }
            }
        }

        private void InitializeSiteMap(SiteMapModel siteMapModel, string siteMapFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(siteMapFilePath);

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            doc.WriteTo(tx);
            string siteMapXml = sw.ToString();

            SiteMap siteMap = new SiteMap();
            StringReader reader = new StringReader(siteMapXml);
            siteMap.Read(reader);

            siteMapModel.SiteMap = siteMap;
            siteMapModel.SiteMapXml = siteMapXml;
        }

        // load current side menu model in the memory to improve the performance
        private void InitializeSideMenus()
        {
            if (SiteMapModelSet != null)
            {
                foreach (SiteMapModel siteMapModel in SiteMapModelSet.ChildNodes)
                {
                    string theSiteMenuFile = siteMapModel.SideMenuFile;
                    if (string.IsNullOrEmpty(theSiteMenuFile))
                    {
                        // use the deafult side menu file name
                        theSiteMenuFile = SideMenuFile;
                    }

                    string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + theSiteMenuFile;

                    InitializeSideMenu(siteMapModel, filePath);
                }
            }
        }

        private void InitializeSideMenu(SiteMapModel siteMapModel, string siteMenuFilePath)
        {
            // get sidemenu xml string
            XmlDocument doc;
            if (File.Exists(siteMenuFilePath))
            {
                doc = new XmlDocument();
                doc.Load(siteMenuFilePath);

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);
                siteMapModel.SideMenuXml = sw.ToString();
            }
            else
            {
                siteMapModel.SideMenuXml = null;
            }

            string xml = siteMapModel.SideMenuXml;
            if (!string.IsNullOrEmpty(xml))
            {
                siteMapModel.SideMenu = new SideMenu();
                StringReader reader = new StringReader(xml);
                siteMapModel.SideMenu.Read(reader);
            }
            else
            {
                siteMapModel.SideMenu = null;
            }
        }

        // load current custom command set model in the memory to improve the performance
        private void InitializeCustomCommandSets()
        {
            if (SiteMapModelSet != null)
            {
                foreach (SiteMapModel siteMapModel in SiteMapModelSet.ChildNodes)
                {
                    string theCustomCommandFile = siteMapModel.CustomCommandSetFile;
                    if (string.IsNullOrEmpty(theCustomCommandFile))
                    {
                        // use the deafult custom command file name
                        theCustomCommandFile = CustomCommandFile;
                    }

                    string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + theCustomCommandFile;

                    InitializeCustomCommandSets(siteMapModel, filePath);
                }
            }
        }

        private void InitializeCustomCommandSets(SiteMapModel siteMapModel, string customCommandFilePath)
        {
            XmlDocument doc;

            if (File.Exists(customCommandFilePath))
            {
                doc = new XmlDocument();
                doc.Load(customCommandFilePath);

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);
                siteMapModel.CustomCommandSetXml = sw.ToString();
            }
            else
            {
                siteMapModel.CustomCommandSetXml = null;
            }

            siteMapModel.CustomCommandGroupTable = new Hashtable();

            string xml = siteMapModel.CustomCommandSetXml;
            if (!string.IsNullOrEmpty(xml))
            {
                siteMapModel.CustomCommandSet = new CustomCommandSet();
                StringReader reader = new StringReader(xml);
                siteMapModel.CustomCommandSet.Read(reader);

                foreach (CustomCommandGroup customCommandGroup in siteMapModel.CustomCommandSet.ChildNodes)
                {
                    siteMapModel.CustomCommandGroupTable[customCommandGroup.Database + customCommandGroup.ClassName] = customCommandGroup;
                }
            }
        }

        // load current xacl policy model in the memory to improve the performance
        private void InitializePolicies()
        {
            if (SiteMapModelSet != null)
            {
                foreach (SiteMapModel siteMapModel in SiteMapModelSet.ChildNodes)
                {
                    string thePolicyFile = siteMapModel.PolicyFile;
                    if (string.IsNullOrEmpty(thePolicyFile))
                    {
                        // use the deafult policy file name
                        thePolicyFile = XaclPolicyFile;
                    }

                    string filePath = NewteraNameSpace.GetAppHomeDir() + SiteMapDir + thePolicyFile;

                    InitializePolicy(siteMapModel, filePath);
                }
            }
        }

        private void InitializePolicy(SiteMapModel siteMapModel, string xaclPolicyFilePath)
        {
            XmlDocument doc;

            if (File.Exists(xaclPolicyFilePath))
            {
                doc = new XmlDocument();
                doc.Load(xaclPolicyFilePath);

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);
                siteMapModel.PolicyXml = sw.ToString();
            }
            else
            {
                siteMapModel.PolicyXml = null;
            }

            string xml = siteMapModel.PolicyXml;
            if (!string.IsNullOrEmpty(xml))
            {
                siteMapModel.Policy = new XaclPolicy();
                StringReader reader = new StringReader(xml);
                siteMapModel.Policy.Read(reader);
            }
            else
            {
                siteMapModel.Policy = null;
            }
        }

        static SiteMapManager()
		{
			// Initializing the cache.
			{
                theManager = new SiteMapManager();
			}
		}
	}
}