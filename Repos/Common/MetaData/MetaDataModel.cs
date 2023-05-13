/*
* @(#)MetaDataModel.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.IO;
    using System.Data;
	using System.Collections;
	using System.Threading;
	using System.Text.RegularExpressions;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.DataView.Taxonomy;
	using Newtera.Common.MetaData.Rules;
	using Newtera.Common.MetaData.Mappings;
	using Newtera.Common.MetaData.FileType;
	using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.Logging;
    using Newtera.Common.MetaData.Api;
    using Newtera.Common.MetaData.XMLSchemaView;

	/// <summary>
	/// Represents meta data of a database schema
	/// </summary>
	/// 
	/// <version>1.0.1 25 Jul 2003</version>
	/// <author>Yong Zhang</author>
	public class MetaDataModel : IXaclObject
	{
        public const string OPT_EQUALS = "=";
        public const string OPT_NOT_EQUALS = "!=";
        public const string OPT_LESS_THAN = "<";
        public const string OPT_GREATER_THAN = ">";
        public const string OPT_LESS_THAN_EQUALS = "<=";
        public const string OPT_GREATER_THAN_EQUALS = ">=";
        public const string OPT_LIKE = "like";

		private bool _unknownSchema;
		private SchemaInfo _schemaInfo;
		private SchemaModel _schemaModel;
		private bool _needReloadSchema;
		private DataViewModelCollection _dataViews;
		private bool _needReloadDataViews;
		private XaclPolicy _xaclPolicy;
		private bool _needReloadXaclPolicy;
		private TaxonomyModelCollection _taxonomies;
		private bool _needReloadTaxonomies;
		private RuleManager _ruleManager;
		private bool _needReloadRules;
		private MappingManager _mappingManager;
		private bool _needReloadMappings;
		private SelectorManager _selectorManager;
		private bool _needReloadSelectors;
        private EventManager _eventManager;
        private bool _needReloadEvents;
        private SubscriberManager _subscriberManager;
        private bool _needReloadSubscribers;
        private LoggingPolicy _loggingPolicy;
        private bool _needReloadLoggingPolicy;
        private XMLSchemaModelCollection _xmlSchemaViews;
        private bool _needReloadXMLSchemaViews;
        private ApiManager _apiManager;
        private bool _needReloadApis;

        // indicate whether the meta data model needs to be saved to files
        private bool _needToSave; // run-time use

		// indicate whether a lock to meta data model at server side is obtained
		private bool _isLockObtained; // run-time use

        // DataView cache by class and user
        private Hashtable _defaultDataViews;
        private Hashtable _detailedDataViews;


		/// <summary>
		/// Initiating an instance of MetaDataModel class
		/// </summary>
		public MetaDataModel()
		{
			_unknownSchema = true;
			_schemaModel = new SchemaModel();
			_schemaModel.MetaData = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _schemaModel.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_dataViews = new DataViewModelCollection();
			_dataViews.MetaData = this;
			_dataViews.SchemaModel = _schemaModel;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _dataViews.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_xaclPolicy = new XaclPolicy();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _xaclPolicy.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_taxonomies = new TaxonomyModelCollection(this);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _taxonomies.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_ruleManager = new RuleManager();
            _ruleManager.MetaData = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _ruleManager.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_mappingManager = new MappingManager();
			_selectorManager = new SelectorManager();
			_selectorManager.MetaData = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _selectorManager.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _eventManager = new EventManager();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _eventManager.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _subscriberManager = new SubscriberManager();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subscriberManager.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _loggingPolicy = new LoggingPolicy();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _loggingPolicy.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _xmlSchemaViews = new XMLSchemaModelCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _xmlSchemaViews.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _apiManager = new ApiManager();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _apiManager.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _needReloadSchema = true;
			_needReloadDataViews = true;
			_needReloadXaclPolicy = true;
			_needReloadTaxonomies = true;
			_needReloadRules = true;
			_needReloadMappings = true;
			_needReloadSelectors = true;
            _needReloadEvents = true;
            _needReloadSubscribers = true;
            _needReloadLoggingPolicy = true;
            _needReloadXMLSchemaViews = true;
            _needReloadApis = true;
			_needToSave = false;
			_isLockObtained = false;

            // initialize the cache
            _detailedDataViews = new Hashtable();
            _defaultDataViews = new Hashtable();
		}
		
		/// <summary>
		/// Initiating an instance of MetaDataModel class
		/// </summary>
		public MetaDataModel(SchemaInfo schemaInfo)
		{
			_unknownSchema = false;
			_schemaInfo = schemaInfo;
			_schemaModel = new SchemaModel(schemaInfo);
			_schemaModel.MetaData = this;
			_dataViews = new DataViewModelCollection();
			_dataViews.MetaData = this;
			_dataViews.SchemaInfo = schemaInfo;
			_dataViews.SchemaModel = _schemaModel;
			_xaclPolicy = new XaclPolicy();
			_taxonomies = new TaxonomyModelCollection(this);
			_ruleManager = new RuleManager();
            _ruleManager.MetaData = this;
			_mappingManager = new MappingManager();
			_selectorManager = new SelectorManager();
			_selectorManager.MetaData = this;
            _eventManager = new EventManager();
            _subscriberManager = new SubscriberManager();
            _loggingPolicy = new LoggingPolicy();
            _xmlSchemaViews = new XMLSchemaModelCollection();
            _apiManager = new ApiManager();

			_needReloadSchema = true;
			_needReloadDataViews = true;
			_needReloadXaclPolicy = true;
			_needReloadTaxonomies = true;
			_needReloadRules = true;
			_needReloadMappings = true;
			_needReloadSelectors = true;
            _needReloadEvents = true;
            _needReloadSubscribers = true;
            _needReloadLoggingPolicy = true;
            _needReloadXMLSchemaViews = true;
            _needReloadApis = true;
			_needToSave = false;
			_isLockObtained = false;

            // initialize the cache
            _detailedDataViews = new Hashtable();
            _defaultDataViews = new Hashtable();
		}

		/// <summary>
		/// Gets Schema information
		/// </summary>
		/// <value> A SchemaInfo object</value>
		public SchemaInfo SchemaInfo
		{
			get
			{
				if (_unknownSchema && _schemaModel.SchemaInfo != null)
				{
					_schemaInfo = new SchemaInfo();
					_schemaInfo.ID = _schemaModel.SchemaInfo.ID;
					_schemaInfo.Name = _schemaModel.SchemaInfo.Name;
					_schemaInfo.Version = _schemaModel.SchemaInfo.Version;
                    _schemaInfo.ModifiedTime = _schemaModel.SchemaInfo.ModifiedTime;
					_unknownSchema = false;
				}

				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
				_schemaModel.SchemaInfo.ID = _schemaInfo.ID;
				_schemaModel.SchemaInfo.Name = _schemaInfo.Name;
				_schemaModel.SchemaInfo.Version = _schemaInfo.Version;
                _schemaModel.SchemaInfo.ModifiedTime = _schemaInfo.ModifiedTime;
				_dataViews.SchemaInfo = value;
			}
		}

		/// <summary>
		/// Gets Schema model
		/// </summary>
		/// <value> A SchemaInfo object</value>
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
		}

		/// <summary>
		/// Gets data view collection
		/// </summary>
		/// <value> A DataViewModelCollection</value>
		public DataViewModelCollection DataViews
		{
			get
			{
				return _dataViews;
			}
		}

		/// <summary>
		/// Gets xacl policy model
		/// </summary>
		/// <value> A xacl policy model</value>
		public XaclPolicy XaclPolicy
		{
			get
			{
				return _xaclPolicy;
			}
		}

		/// <summary>
		/// Gets a collection of TaxonomyModel objects
		/// </summary>
		/// <value>A TaxonomyModelCollection</value>
		public TaxonomyModelCollection Taxonomies
		{
			get
			{
				return _taxonomies;
			}
		}

		/// <summary>
		/// Gets the RuleManager that manages the rules
		/// </summary>
		/// <value>A RuleManager</value>
		public RuleManager RuleManager
		{
			get
			{
				return _ruleManager;
			}
		}

		/// <summary>
		/// Gets the MappingManager that manages the import/export mappings
		/// </summary>
		/// <value>A MappingManager</value>
		public MappingManager MappingManager
		{
			get
			{
				return _mappingManager;
			}
		}

		/// <summary>
		/// Gets the SelectorManager that manages all selectors
		/// </summary>
		/// <value>A SelectorManager</value>
		public SelectorManager SelectorManager
		{
			get
			{
				return _selectorManager;
			}
		}

        /// <summary>
        /// Gets the EventManager that manages the events
        /// </summary>
        /// <value>A EventManager</value>
        public EventManager EventManager
        {
            get
            {
                return _eventManager;
            }
        }

        /// <summary>
        /// Gets the SubscriberManager that manages the subscribers
        /// </summary>
        /// <value>A SubscriberManager</value>
        public SubscriberManager SubscriberManager
        {
            get
            {
                return _subscriberManager;
            }
        }

        /// <summary>
        /// Gets logging policy model
        /// </summary>
        /// <value> A logging policy model</value>
        public LoggingPolicy LoggingPolicy
        {
            get
            {
                return _loggingPolicy;
            }
        }

        /// <summary>
        /// Gets a collection of XMLSchemaViewModel objects
        /// </summary>
        /// <value>A TaxonomyModelCollection</value>
        public XMLSchemaModelCollection XMLSchemaViews
        {
            get
            {
                return _xmlSchemaViews;
            }
        }

        /// <summary>
        /// Gets the ApiManager that manages the apis
        /// </summary>
        /// <value>An ApiManager</value>
        public ApiManager ApiManager
        {
            get
            {
                return _apiManager;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the meta data model needs to save to the files.
        /// </summary>
        public bool NeedToSave
		{
			get
			{
				return _needToSave;
			}
			set
			{
				_needToSave = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a lock to the meta data model
		/// at server side has been obtained.
		/// </summary>
		public bool IsLockObtained
		{
			get
			{
				return _isLockObtained;
			}
			set
			{
				_isLockObtained = value;
			}
		}

		/// <summary>
		/// Gets the value indicating whether any part of the meta data has been altered.
		/// </summary>
		/// <value>true if any part of meta data is altered, false otherwise</value>
		public bool IsAltered
		{
			get
			{
				return SchemaModel.IsAltered || DataViews.IsAltered || XaclPolicy.IsAltered || Taxonomies.IsAltered
					|| RuleManager.IsAltered || MappingManager.IsAltered || SelectorManager.IsAltered
                    || EventManager.IsAltered || SubscriberManager.IsAltered || LoggingPolicy.IsAltered || XMLSchemaViews.IsAltered
                    || ApiManager.IsAltered;
			}
			set
			{
				SchemaModel.IsAltered = value;
				DataViews.IsAltered = value;
				XaclPolicy.IsAltered = value;
				Taxonomies.IsAltered = value;
				RuleManager.IsAltered = value;
				MappingManager.IsAltered = value;
				SelectorManager.IsAltered = value;
                EventManager.IsAltered = value;
                SubscriberManager.IsAltered = value;
                LoggingPolicy.IsAltered = value;
                XMLSchemaViews.IsAltered = value;
                ApiManager.IsAltered = value;
			}
		}

		/// <summary>
		/// Gets the value indicating whether any part of the meta data need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReload
		{
			get
			{
				return _needReloadSchema || _needReloadDataViews || _needReloadXaclPolicy || _needReloadTaxonomies
					|| _needReloadRules || _needReloadMappings || _needReloadSelectors || _needReloadEvents
                    || _needReloadSubscribers || _needReloadLoggingPolicy || _needReloadXMLSchemaViews || _needReloadApis;
			}
			set
			{
				NeedReloadSchema = value;
				NeedReloadDataViews = value;
				NeedReloadXaclPolicy = value;
				NeedReloadTaxonomies = value;
				NeedReloadRules = value;
				NeedReloadMappings = value;
				NeedReloadSelectors = value;
                NeedReloadEvents = value;
                NeedReloadSubscribers = value;
                NeedReloadLoggingPolicy = value;
                NeedReloadXMLSchemaViews = value;
                NeedReloadApis = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the schema model need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadSchema
		{
			get
			{
				return _needReloadSchema;
			}
			set
			{
				_needReloadSchema = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the data views need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadDataViews
		{
			get
			{
				return _needReloadDataViews;
			}
			set
			{
				_needReloadDataViews = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the xacl policy need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadXaclPolicy
		{
			get
			{
				return _needReloadXaclPolicy;
			}
			set
			{
				_needReloadXaclPolicy = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the taxonomies need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadTaxonomies
		{
			get
			{
				return _needReloadTaxonomies;
			}
			set
			{
				_needReloadTaxonomies = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the rules need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadRules
		{
			get
			{
				return _needReloadRules;
			}
			set
			{
				_needReloadRules = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the mappings need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadMappings
		{
			get
			{
				return _needReloadMappings;
			}
			set
			{
				_needReloadMappings = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether the selectors need to be reloaded.
		/// </summary>
		/// <value>true if it needs reload, false otherwise</value>
		public bool NeedReloadSelectors
		{
			get
			{
				return _needReloadSelectors;
			}
			set
			{
				_needReloadSelectors = value;
			}
		}

        /// <summary>
        /// Gets or sets the value indicating whether the events need to be reloaded.
        /// </summary>
        /// <value>true if it needs reload, false otherwise</value>
        public bool NeedReloadEvents
        {
            get
            {
                return _needReloadEvents;
            }
            set
            {
                _needReloadEvents = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the subscribers need to be reloaded.
        /// </summary>
        /// <value>true if it needs reload, false otherwise</value>
        public bool NeedReloadSubscribers
        {
            get
            {
                return _needReloadSubscribers;
            }
            set
            {
                _needReloadSubscribers = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the logging policy need to be reloaded.
        /// </summary>
        /// <value>true if it needs reload, false otherwise</value>
        public bool NeedReloadLoggingPolicy
        {
            get
            {
                return _needReloadLoggingPolicy;
            }
            set
            {
                _needReloadLoggingPolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the xml schema views need to be reloaded.
        /// </summary>
        /// <value>true if it needs reload, false otherwise</value>
        public bool NeedReloadXMLSchemaViews
        {
            get
            {
                return _needReloadXMLSchemaViews;
            }
            set
            {
                _needReloadXMLSchemaViews = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the apis need to be reloaded.
        /// </summary>
        /// <value>true if it needs reload, false otherwise</value>
        public bool NeedReloadApis
        {
            get
            {
                return _needReloadApis;
            }
            set
            {
                _needReloadApis = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether it must check permissions
        /// before adding an item to the tree
        /// </summary>
        /// <value>true if it must check permissions, false otherwise.</value>
        public bool CheckPermission
		{
			get
			{
				// if the CustomerPrincipal is null, it means an unauthenticated user
				// do not check permission
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

				if (principal != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        public void ClearCache()
        {
            _detailedDataViews.Clear();
            _defaultDataViews.Clear();
        }

        /// <summary>
        /// Load the meta data model with xml strings representing various kinds of meta data
        /// </summary>
        /// <param name="xmlStrings"></param>
        public void Load(string[] xmlStrings)
        {
            // read the xml string for schema
            StringReader reader = new StringReader(xmlStrings[0]);
            this.SchemaModel.Read(reader);
            this.SchemaModel.IsAltered = false;

            // read the xml string for data views
            reader = new StringReader(xmlStrings[1]);
            this.DataViews.Read(reader);
            this.DataViews.IsAltered = false;

            // read the xml string for xacl policy
            reader = new StringReader(xmlStrings[2]);
            this.XaclPolicy.Read(reader);
            this.XaclPolicy.IsAltered = false;

            // read the xml string for taxonomies
            reader = new StringReader(xmlStrings[3]);
            this.Taxonomies.Read(reader);
            this.Taxonomies.IsAltered = false;

            // read the xml string for rules
            reader = new StringReader(xmlStrings[4]);
            this.RuleManager.Read(reader);
            this.RuleManager.IsAltered = false;

            // read the xml string for mappings
            reader = new StringReader(xmlStrings[5]);
            this.MappingManager.Read(reader);
            this.MappingManager.IsAltered = false;

            // read the xml string for selectors
            reader = new StringReader(xmlStrings[6]);
            this.SelectorManager.Read(reader);
            this.SelectorManager.IsAltered = false;

            // read the xml string for events
            reader = new StringReader(xmlStrings[7]);
            this.EventManager.Read(reader);
            this.EventManager.IsAltered = false;

            // read the xml string for logging policy
            reader = new StringReader(xmlStrings[8]);
            this.LoggingPolicy.Read(reader);
            this.LoggingPolicy.IsAltered = false;

            // read the xml string for subscribers
            reader = new StringReader(xmlStrings[9]);
            this.SubscriberManager.Read(reader);
            this.SubscriberManager.IsAltered = false;

            // read the xml string for XMLSchemaViews
            reader = new StringReader(xmlStrings[10]);
            this.XMLSchemaViews.Read(reader);
            this.XMLSchemaViews.IsAltered = false;

            // read the xml string for apis
            reader = new StringReader(xmlStrings[11]);
            this.ApiManager.Read(reader);
            this.ApiManager.IsAltered = false;
        }

        /// <summary>
        /// Find a object of IMetaDataElement that is indicated by a xpath.
        /// </summary>
        /// <param name="xpath">The xpath</param>
        /// <returns>The found IMetaDataElement, null if not found.</returns>
        public IMetaDataElement FindMetaModelElementByXPath(string xpath)
        {
            IMetaDataElement found = null;

            // try the schema model first
            found = this.SchemaModel.FindSchemaModelElementByXPath(xpath);
            if (found != null)
            {
                return found;
            }

            found = this.Taxonomies.FindNodeByXPath(xpath);
            if (found != null)
            {
                return found;
            }

            found = this.DataViews.FindDataViewByXPath(xpath);
            if (found != null)
            {
                return found;
            }

            return found;
        }

		/// <summary>
		/// Gets all bottom classes of the schema
		/// </summary>
		/// <returns>A collection of ClassElement instances representing bottom classes.</returns>
		/// <remarks>
		/// Only the bottom classes that the user has permission to read will be returned.
		/// </remarks>
		public SchemaModelElementCollection GetBottomClasses()
		{
			return GetBottomClasses(null);
		}

		/// <summary>
		/// Gets all bottom classes of the given root class.
		/// </summary>
		/// <param name="rootClassName">The root class name, null for all bottom classes in the schema.</param>
		/// <returns>A collection of ClassElement instances representing bottom classes of the given root.</returns>
		/// <remarks>
		/// Only the bottom classes that the user has permission to read will be returned.
		/// </remarks>
		public SchemaModelElementCollection GetBottomClasses(string rootClassName)
		{
			SchemaModelElementCollection bottomClasses = new SchemaModelElementCollection();
			SchemaModelElementCollection childClasses;

			// find the first level of child classes
			if (rootClassName == null)
			{
				childClasses = this.SchemaModel.RootClasses;
			}
			else
			{
				ClassElement rootClass = this.SchemaModel.FindClass(rootClassName);

				if (rootClass != null)
				{
					childClasses = rootClass.Subclasses;
				}
				else
				{
					// root class name is incorrect, get all root classes from schema
					childClasses = this.SchemaModel.RootClasses;
				}
			}

			AddBottomClasses(bottomClasses, childClasses);

			return bottomClasses;
		}

        /// <summary>
        /// Get a copy of a dataview with given name
        /// </summary>
        /// <param name="dataViewName">The dataview name</param>
        /// <returns>The DataViewModel</returns>
        public DataViewModel GetDataView(string dataViewName)
        {
            DataViewModel dataView;

            if (DataViews[dataViewName] != null)
            {
                // get the specified data view
                dataView = ((DataViewModel)DataViews[dataViewName]).Clone();

                dataView.SchemaModel = SchemaModel;
                dataView.SchemaInfo = SchemaInfo;

                // add related classes to the dataview
                dataView.BaseClass.RelatedClasses = GetRelatedClasses(dataView.BaseClass);
            }
            else
            {
                throw new Exception("Unable to find a dataview in metadata with name " + dataViewName);
            }

            return dataView;
        }

		/// <summary>
		/// Gets the default data view of a class, including only attributes whose
		/// usage is defined as "either Result, Serach, or Both"
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of all the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default.
		/// </remarks>
		public DataViewModel GetDefaultDataView(string className)
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                return this.GetDefaultDataView(className, null, false, false, false);
            }
            else
            {
                // TODO, Get the dataview from cache
                return this.GetDefaultDataView(className, null, false, false, false);
            }
		}

		/// <summary>
		/// Gets the default data view of a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="sectionString">A string of sections separated by ;</param>
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default. If a section string is provided, only those attributes
		/// that matches any of sections will be added to the result list.
		/// </remarks>
		public DataViewModel GetDefaultDataView(string className, string sectionString)
		{
			return this.GetDefaultDataView(className, sectionString, true, true, false);
		}

		/// <summary>
		/// Gets the default data view of a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="includeArrays">true to include array attributes, false otherwise.</param>
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default. If a section string is provided, only those attributes
		/// that matches any of sections will be added to the result list.
		/// </remarks>
		public DataViewModel GetDefaultDataView(string className, bool includeArrays)
		{
			return this.GetDefaultDataView(className, null, includeArrays, false, false);
		}


		/// <summary>
		/// Gets the default data view of a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="sectionString">A string of sections separated by ;</param>
		/// <param name="includeArrays">true to include array attributes, false otherwise.</param>
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default. If a section string is provided, only those attributes
		/// that matches any of sections will be added to the result list.
		/// </remarks>
		public DataViewModel GetDefaultDataView(string className, string sectionString, bool includeArrays)
		{
			return this.GetDefaultDataView(className, sectionString, includeArrays, true, false);
		}

		/// <summary>
		/// Gets the detailed data view of a class, ignore the usage definitions of attributes.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>The DataViewModel for the detailed data view</returns>
		/// <remarks>
		/// The detailed data view consists of all the attributes that are browsable
		/// </remarks>
		public DataViewModel GetDetailedDataView(string className)
		{
            if (GlobalSettings.Instance.IsWindowClient)
            {
                return this.GetDefaultDataView(className, null, true, true, false);
            }
            else
            {
                // TODO, Get the dataview from the cache
                //string key = CreateKey(className);
                //DataViewModel dataView = (DataViewModel)_detailedDataViews[key];

                return this.GetDefaultDataView(className, null, true, true, false);
            }
		}

		/// <summary>
		/// Gets the complete data view of a class, including non-browsable attribute
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of all the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default.
		/// </remarks>
		public DataViewModel GetCompleteDataView(string className)
		{
			return this.GetDefaultDataView(className, null, true, true, true);
		}

        /// <summary>
        /// Gets the default data view of a class related to the master instance via a relationship
        /// </summary>
        /// <param name="relatedClassName">The related class name</param>
        /// <returns>The default DataViewModel of the related class</returns>
        /// <remarks>
        /// The search query generated by the returned DataViewModel will only retrieve the instances that related to the master instance
        /// </remarks>
        public DataViewModel GetRelatedDefaultDataView(InstanceView masterInstance, string relatedClassName)
        {
            // get the related data view
            DataViewModel relatedDataView = GetDefaultDataView(relatedClassName);

            AddConstraintForRelatedClass(masterInstance, relatedDataView);

            return relatedDataView;
        }

        /// <summary>
        /// Gets the detailed data view of a class related to the master instance via a relationship
        /// </summary>
        /// <param name="relatedClassName">The related class name</param>
        /// <returns>The default DataViewModel of the related class</returns>
        /// <remarks>
        /// The search query generated by the returned DataViewModel will only retrieve the instances that related to the master instance
        /// </remarks>
        public DataViewModel GetRelatedDetailedDataView(InstanceView masterInstance, string relatedClassName)
        {
            // get the related data view
            DataViewModel relatedDataView = GetDetailedDataView(relatedClassName);

            AddConstraintForRelatedClass(masterInstance, relatedDataView);

            return relatedDataView;
        }

        /// <summary>
        /// Gets a specified data view of a class related to the master instance via a relationship
        /// </summary>
        /// <param name="relatedClassName">The related class name</param>
        /// <returns>The default DataViewModel of the related class</returns>
        /// <remarks>
        /// The search query generated by the returned DataViewModel will only retrieve the instances that related to the master instance
        /// </remarks>
        public DataViewModel GetRelatedDataView(string dataViewName, InstanceView masterInstance, string relatedClassName)
        {
            // get the related data view
            DataViewModel originalDataView = DataViews[dataViewName] as DataViewModel;

            if (originalDataView == null)
            {
                throw new Exception("Unable to find the dataview with name " + dataViewName);
            }
            else if (originalDataView.BaseClass.ClassName != relatedClassName &&
                !IsParentOf(originalDataView.BaseClass.ClassName, relatedClassName))
            {
                throw new Exception("The dataview with name " + dataViewName + " is defined for class " + originalDataView.BaseClass.ClassName + " which isn't the same class as or parent class for the related class " + relatedClassName);
            }

            DataViewModel relatedDataView = GetClonedDataView(originalDataView); // clone the dataview so the changes made to the dataview will not affect the original one

            // The referrence classes statically defined in a customized data view may not have run-time info,
            // we need to add referenced classes here
            relatedDataView.ReferencedClasses.Clear();
            DataRelationshipAttribute result;
            IDataViewElement expr, left, right;

            ClassElement currentRelatedClassElement = relatedDataView.BaseClass.GetSchemaModelElement() as ClassElement;
            while (currentRelatedClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentRelatedClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable)
                    {
                        if (relationshipAttribute.IsForeignKeyRequired)
                        {
                            // add relationship as a search filter
                            // add as a search attribute
                            DataRelationshipAttribute relationshipExpr = new DataRelationshipAttribute(relationshipAttribute.Name, relatedDataView.BaseClass.Alias, relationshipAttribute.LinkedClassName);
                            relationshipExpr.Caption = relationshipAttribute.Caption;
                            relationshipExpr.Description = relationshipAttribute.Description;
                            right = new Parameter(relationshipAttribute.Name, relationshipExpr.OwnerClassAlias, relationshipAttribute.DataType);

                            expr = new RelationalExpr(ElementType.Equals, relationshipExpr, right);
                            relatedDataView.AddSearchExpr(expr, ElementType.And);

                            // Add the linked class as a referenced class to data view
                            DataClass referencedClass = relationshipExpr.ReferencedDataClass;
                            relatedDataView.ReferencedClasses.Add(referencedClass);
                        }
                        else if (relationshipAttribute.LinkedClass.IsJunction)
                        {
                            // lined class is an junction class for a many-to-many relationship
                            // add the junction class and the class on the other side of relationship
                            // as referenced class

                            // First add the junction class as a referenced class to data view
                            DataClass junctionClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                                    relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            junctionClass.ReferringClassAlias = relatedDataView.BaseClass.Alias;
                            junctionClass.ReferringRelationshipName = relationshipAttribute.Name;
                            junctionClass.Caption = relationshipAttribute.LinkedClass.Caption;
                            junctionClass.ReferringRelationship = relationshipAttribute;
                            relatedDataView.ReferencedClasses.Add(junctionClass);

                            // find the many-to-one relationship attribute in the junction class that connects to
                            // the referenced class on the otherside of relationship
                            RelationshipAttributeElement toReferencedClsRelationshipAttribute = relationshipAttribute.LinkedClass.FindPairedRelationshipAttribute(relationshipAttribute);

                            // add the other end class of many-to-many relationship as a referenced class in the dataview
                            DataClass referencedClass = new DataClass(toReferencedClsRelationshipAttribute.LinkedClassAlias,
                                toReferencedClsRelationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            referencedClass.ReferringClassAlias = junctionClass.Alias;
                            referencedClass.ReferringRelationshipName = toReferencedClsRelationshipAttribute.Name;
                            referencedClass.Caption = toReferencedClsRelationshipAttribute.LinkedClass.Caption;
                            referencedClass.ReferringRelationship = toReferencedClsRelationshipAttribute;
                            relatedDataView.ReferencedClasses.Add(referencedClass);
                        }
                        else
                        {
                            // one-to-many or one-to-one
                            // Add the related class as a referenced class to data view
                            DataClass relatedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                                    relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            relatedClass.ReferringClassAlias = relatedDataView.BaseClass.Alias;
                            relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                            relatedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                            relatedClass.ReferringRelationship = relationshipAttribute;
                            relatedDataView.ReferencedClasses.Add(relatedClass);
                        }
                    }
                }

                currentRelatedClassElement = currentRelatedClassElement.ParentClass;
            }

            AddConstraintForRelatedClass(masterInstance, relatedDataView);

            return relatedDataView;
        }

        /// <summary>
        /// Add constraint to the DataModelView so that it generate the query that retrieves the
        /// data instances related to the master instance
        /// </summary>
        protected void AddConstraintForRelatedClass(InstanceView masterInstance, DataViewModel relatedDataView)
        {
            DataClass relatedDataClass = null;
            foreach (DataClass relatedClass in masterInstance.DataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedDataView.BaseClass.Name ||
                    IsParentOf(relatedClass.ClassName, relatedDataView.BaseClass.Name))
                {
                    relatedDataClass = relatedClass;
                    break;
                }
            }

            if (relatedDataClass == null)
            {
                // try to get from referenced Classes
                foreach (DataClass relatedClass in masterInstance.DataView.ReferencedClasses)
                {
                    if (relatedClass.ClassName == relatedDataView.BaseClass.Name ||
                        IsParentOf(relatedClass.ClassName, relatedDataView.BaseClass.Name))
                    {
                        relatedDataClass = relatedClass;
                        break;
                    }
                }

                if (relatedDataClass == null)
                {
                    throw new Exception("Unable to find related data class " + relatedDataView.BaseClass.Name  + " for " + masterInstance.DataView.BaseClass.Name);
                }
            }

            // build a search expression that retrieve the data instances of the related class that
            // are asspciated with the selected data instance
            string searchValue = null;
            if (relatedDataClass.ReferringRelationship.IsForeignKeyRequired)
            {
                // it is a many-to-one relationship between the base class and related class,
                // gets the obj_id of the instance for the related class from the master instance view

                if (masterInstance.DataSet != null)
                {
                    int rowIndex = masterInstance.SelectedIndex;

                    DataTable relationshipTable = masterInstance.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(masterInstance.DataView.BaseClass.ClassName, relatedDataClass.ReferringRelationship.Name)];
                    if (relationshipTable != null)
                    {
                        if (rowIndex < relationshipTable.Rows.Count &&
                            relationshipTable.Columns[NewteraNameSpace.OBJ_ID] != null &&
                            relationshipTable.Rows[rowIndex].IsNull(NewteraNameSpace.OBJ_ID) == false)
                        {
                            searchValue = relationshipTable.Rows[rowIndex][NewteraNameSpace.OBJ_ID].ToString();
                        }

                        if (string.IsNullOrEmpty(searchValue))
                        {
                            searchValue = "0"; // default
                        }
                    }
                }
                else
                {
                    searchValue = "0"; // default
                }
            }
            else
            {
                // it is a one-to-many relationship bewtween the base class and related class
                // use the obj_id of the master instance
                if (masterInstance.InstanceData != null &&
                    !string.IsNullOrEmpty(masterInstance.InstanceData.ObjId))
                {
                    searchValue = masterInstance.InstanceData.ObjId;
                }
                else
                {
                    searchValue = "0"; // default
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                SearchExpressionBuilder builder = new SearchExpressionBuilder();
                IDataViewElement expr = builder.BuildSearchExprForRelationship(relatedDataView, relatedDataClass, searchValue);

                // add search expression to the dataview
                relatedDataView.AddSearchExpr(expr, Newtera.Common.MetaData.DataView.ElementType.And);
            }
        }

        private bool IsParentOf(string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = SchemaModel.FindClass(childClassName);
            if (childClassElement == null)
            {
                throw new Exception("Unable to find class element for " + childClassName);
            }

            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        private string CreateKey(string className)
        {
            string key = className;

            if (Thread.CurrentPrincipal != null &&
                Thread.CurrentPrincipal.Identity != null)
            {
                string userName = Thread.CurrentPrincipal.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    key += "-" + userName;
                }
                else
                {
                    key += "-system";
                }
            }   

            return key;
        }

		/// <summary>
		/// Gets the default data view of a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="sectionString">A string of sections separated by ;</param>
		/// <param name="includeArrays">true to include array attributes.</param>
		/// <param name="ignoreUsage">ignore the attribute usage definitions</param>
		/// <param name="includeNonBrowsables">true to include non-browsable attributes in result attributes, false, otherwise</param>		
		/// <returns>The DataViewModel for the default view</returns>
		/// <remarks>
		/// The default data view consists of the attributes in search
		/// and result fields, including simple and relationship ones, that an user
		/// has permission to access. The search fields are combinded with AND
		/// operator by default. If a section string is provided, only those attributes
		/// that matches any of sections will be added to the result list.
		/// </remarks>
		public DataViewModel GetDefaultDataView(string className, string sectionString,
			bool includeArrays, bool ignoreUsage, bool includeNonBrowsables)
		{
			string[] sections;
			DataViewModel dataView = null;
			ClassElement classElement = SchemaModel.FindClass(className);
			if (classElement != null)
			{
				if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, classElement, XaclActionType.Read))
				{
					throw new PermissionViolationException("Do not have permission to read class " + classElement.Name);
				}

				if (sectionString != null)
				{
					Regex regex = new Regex(";");
					sections = regex.Split(sectionString);
				}
				else
				{
					sections = new string[0];
				}

				dataView = new DataViewModel(classElement.Caption + " (Default)", SchemaInfo, _schemaModel);

				dataView.BaseClass = new DataClass(classElement.Name, classElement.Name, DataClassType.BaseClass);
				dataView.BaseClass.Caption = classElement.Caption;

                // obtain the related classes to the base class that the current user has permission to read
                dataView.BaseClass.RelatedClasses = GetRelatedClasses(dataView.BaseClass);

                // if the sort attribute is obj_id, add obj_id as a sort attribute
                if (classElement.SortAttribute == NewteraNameSpace.OBJ_ID)
                {
                    // add @obj_id as a sort attribute
                    SortAttribute sortAttribute = new SortAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, dataView.BaseClass.Alias);
                    sortAttribute.SortDirection = classElement.SortDirection;
                    dataView.SortBy.SortAttributes.Add(sortAttribute);
                }

				IList attributes = GetReadableAttributes(classElement);

				foreach (SchemaModelElement attribute in attributes)
				{
					IDataViewElement expr, left, right, result;
					SimpleAttributeElement simpleAttribute = attribute as SimpleAttributeElement;
					ArrayAttributeElement arrayAttribute = attribute as ArrayAttributeElement;
                    VirtualAttributeElement virtualAttribute = attribute as VirtualAttributeElement;
                    ImageAttributeElement imageAttribute = attribute as ImageAttributeElement;
                    RelationshipAttributeElement relationshipAttribute = attribute as RelationshipAttributeElement;

					if (simpleAttribute != null)
					{
						if (ignoreUsage || simpleAttribute.Usage == DefaultViewUsage.Included)
						{
                            // add as a search attribute
							left = new DataSimpleAttribute(simpleAttribute.Name, dataView.BaseClass.Alias);
							left.Caption = simpleAttribute.Caption;
							left.Description = simpleAttribute.Description;
                            
							if (simpleAttribute.IsMultipleChoice)
							{
								// Use String type for the attribute with multiple choices
								right = new Parameter(simpleAttribute.Name, dataView.BaseClass.Alias, DataType.String);
							}
							else
							{
								right = new Parameter(simpleAttribute.Name, dataView.BaseClass.Alias, simpleAttribute.DataType);
							}
							if (simpleAttribute.IsHistoryEdit || simpleAttribute.IsRichText)
							{
								expr = new ContainsFunc(left, right);
							}
							else
							{
								expr = new RelationalExpr(GetOperatorType(simpleAttribute), left, right);
							}
							dataView.AddSearchExpr(expr, ElementType.And);
						}

						if ((includeNonBrowsables || simpleAttribute.IsBrowsable) &&
							((sections.Length == 0 &&
							(ignoreUsage || simpleAttribute.Usage == DefaultViewUsage.Included))
							|| IsInSections(simpleAttribute.Section, sections)))
						{
                            // add as result attribute
							result = new DataSimpleAttribute(simpleAttribute.Name, dataView.BaseClass.Alias);
							result.Caption = simpleAttribute.Caption;
							result.Description = simpleAttribute.Description;
							dataView.ResultAttributes.Add(result);

                            // If the simple attribute has rules defined localy,
                            // check write permission using the SimpleAttributeElement rather than
                            // DataSimpleAttribute because DataSimpleAttribute is associated with
                            // the base class, while SimpleAttributeElement is associated with the
                            // its owner class which may not be the base class. The xacl rules
                            // defined for an attribute are not passed down to the base class.
                            // Otherwise, check the write permission of the attribute using DataSimpleAttribute
                            // in case that the base class overrides the rules defined in the owner class.
                            if (this.XaclPolicy.HasLocalRules(simpleAttribute))
                            {
                                // check using the rules defined localy for the attribute
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, simpleAttribute, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
                            else
                            {
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, result, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
						}

                        SortDirection sortDirection;
                        if (simpleAttribute.IsBrowsable && IsSortAttribute(classElement, simpleAttribute, out sortDirection))
                        {
                            // add as a sort attribute
                            SortAttribute sortAttribute = new SortAttribute(simpleAttribute.Name, dataView.BaseClass.Alias);
                            sortAttribute.SortDirection = sortDirection;
                            dataView.SortBy.SortAttributes.Add(sortAttribute);
                        }
					}
					else if (arrayAttribute != null)
					{
						// add the array attribute as a result field only
						if ((includeNonBrowsables || arrayAttribute.IsBrowsable) &&
							(sections.Length == 0 || IsInSections(arrayAttribute.Section, sections)) &&
							includeArrays)
						{
							result = new DataArrayAttribute(arrayAttribute.Name, dataView.BaseClass.Alias);
							result.Caption = arrayAttribute.Caption;
							result.Description = arrayAttribute.Description;
							dataView.ResultAttributes.Add(result);

                            // If the simple attribute has rules defined localy,
                            // check write permission using the ArrayAttributeElement rather than
                            // DataArrayAttribute because it is associated with
                            // the base class, while ArrayAttributeElement is associated with the
                            // its owner class which may not be the base class. The xacl rules
                            // defined for an attribute are not passed down to the base class.
                            // Otherwise, check the write permission of the attribute using DataArrayAttribute
                            // in case that the base class overrides the rules defined in the owner class.
                            if (this.XaclPolicy.HasLocalRules(arrayAttribute))
                            {
                                // check using the rules defined localy for the attribute
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, arrayAttribute, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
                            else
                            {
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, result, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
						}
					}
                    else if (virtualAttribute != null)
                    {
                        // add the virtual attribute as a result field only
                        if ((includeNonBrowsables || virtualAttribute.IsBrowsable) &&
                             ((sections.Length == 0 &&
                                (ignoreUsage || virtualAttribute.Usage == DefaultViewUsage.Included))
                                || IsInSections(virtualAttribute.Section, sections)))
                        {
                            result = new DataVirtualAttribute(virtualAttribute.Name, dataView.BaseClass.Alias);
                            result.Caption = virtualAttribute.Caption;
                            result.Description = virtualAttribute.Description;
                            result.IsReadOnly = true;
                            dataView.ResultAttributes.Add(result);
                        }
                    }
                    else if (imageAttribute != null)
                    {
                        // add the image attribute as a result field only
                        if ((includeNonBrowsables || imageAttribute.IsBrowsable) &&
                             ((sections.Length == 0 &&
                              (ignoreUsage || imageAttribute.Usage == DefaultViewUsage.Included))
                              || IsInSections(imageAttribute.Section, sections)))
                        {
                            result = new DataImageAttribute(imageAttribute.Name, dataView.BaseClass.Alias);
                            result.Caption = imageAttribute.Caption;
                            result.Description = imageAttribute.Description;
                            dataView.ResultAttributes.Add(result);

                            // If the image attribute has rules defined localy,
                            // check write permission using the ImageAttributeElement rather than
                            // DataImageAttribute because it is associated with
                            // the base class, while ImageAttributeElement is associated with the
                            // its owner class which may not be the base class. The xacl rules
                            // defined for an attribute are not passed down to the base class.
                            // Otherwise, check the write permission of the attribute using DataImageAttribute
                            // in case that the base class overrides the rules defined in the owner class.
                            if (this.XaclPolicy.HasLocalRules(imageAttribute))
                            {
                                // check using the rules defined localy for the attribute
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, imageAttribute, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
                            else
                            {
                                if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, result, XaclActionType.Write))
                                {
                                    result.IsReadOnly = true;
                                }
                            }
                        }
                    }
					else if (relationshipAttribute != null &&
						(includeNonBrowsables || relationshipAttribute.IsBrowsable))
					{
                        result = new DataRelationshipAttribute(relationshipAttribute.Name, dataView.BaseClass.Alias, relationshipAttribute.LinkedClassName);
						result.Caption = relationshipAttribute.Caption;
						result.Description = relationshipAttribute.Description;

                        if (relationshipAttribute.IsForeignKeyRequired)
                        {
                            // many-to-one relationship or one-to-one with joined manager
                            if (relationshipAttribute.Usage == DefaultViewUsage.Included)
                            {
                                // display relationship as primary keys in web client
                                ((DataRelationshipAttribute)result).ShowPrimaryKeys = true;
                            }

                            // add relationship as a search filter
                            // add as a search attribute
                            DataRelationshipAttribute relationshipExpr = new DataRelationshipAttribute(relationshipAttribute.Name, dataView.BaseClass.Alias, relationshipAttribute.LinkedClassName);
                            relationshipExpr.Caption = relationshipAttribute.Caption;
                            relationshipExpr.Description = relationshipAttribute.Description;
                            right = new Parameter(relationshipAttribute.Name, relationshipExpr.OwnerClassAlias, relationshipAttribute.DataType);

                            expr = new RelationalExpr(ElementType.Equals, relationshipExpr, right);
                            dataView.AddSearchExpr(expr, ElementType.And);

                            // Add the linked class as a referenced class to data view
                            DataClass referencedClass = relationshipExpr.ReferencedDataClass;
                            dataView.ReferencedClasses.Add(referencedClass);
                        }
                        else if (relationshipAttribute.LinkedClass.IsJunction)
                        {
                            // lined class is an junction class for a many-to-many relationship
                            // add the junction class and the class on the other side of relationship
                            // as referenced class

                            // First add the junction class as a referenced class to data view
                            DataClass junctionClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                                    relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            junctionClass.ReferringClassAlias = dataView.BaseClass.Alias;
                            junctionClass.ReferringRelationshipName = relationshipAttribute.Name;
                            junctionClass.Caption = relationshipAttribute.LinkedClass.Caption;
                            junctionClass.ReferringRelationship = relationshipAttribute;
                            dataView.ReferencedClasses.Add(junctionClass);

                            // find the many-to-one relationship attribute in the junction class that connects to
                            // the referenced class on the otherside of relationship
                            RelationshipAttributeElement toReferencedClsRelationshipAttribute = relationshipAttribute.LinkedClass.FindPairedRelationshipAttribute(relationshipAttribute);

                            // add the other end class of many-to-many relationship as a referenced class in the dataview
                            DataClass referencedClass = new DataClass(toReferencedClsRelationshipAttribute.LinkedClassAlias,
                                toReferencedClsRelationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            referencedClass.ReferringClassAlias = junctionClass.Alias;
                            referencedClass.ReferringRelationshipName = toReferencedClsRelationshipAttribute.Name;
                            referencedClass.Caption = toReferencedClsRelationshipAttribute.LinkedClass.Caption;
                            referencedClass.ReferringRelationship = toReferencedClsRelationshipAttribute;
                            dataView.ReferencedClasses.Add(referencedClass);
                        }
                        else
                        {
                            // one-to-many or one-to-one
                            // Add the related class as a referenced class to data view
                            DataClass relatedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                                    relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            relatedClass.ReferringClassAlias = dataView.BaseClass.Alias;
                            relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                            relatedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                            relatedClass.ReferringRelationship = relationshipAttribute;
                            dataView.ReferencedClasses.Add(relatedClass);
                        }

                        // add relationship attribute as a result attribute regardless of its
                        // cardinal type so that dataset for the result will contains the relations
                        // for navigating purpose
                        dataView.ResultAttributes.Add(result);

                        // If the relationship attribute has rules defined localy,
                        // check write permission using the RelationshipAttributeElement rather than
                        // DataRelationshipAttribute because it is associated with
                        // the base class, while RelationshipAttributeElement is associated with the
                        // its owner class which may not be the base class. The xacl rules
                        // defined for an attribute are not passed down to the base class.
                        // Otherwise, check the write permission of the attribute using DataRelationshipAttribute
                        // in case that the base class overrides the rules defined in the owner class.
                        if (this.XaclPolicy.HasLocalRules(relationshipAttribute))
                        {
                            // check using the rules defined localy for the attribute
                            if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, relationshipAttribute, XaclActionType.Write))
                            {
                                result.IsReadOnly = true;
                            }
                        }
                        else
                        {
                            if (CheckPermission && !PermissionChecker.Instance.HasPermission(this.XaclPolicy, result, XaclActionType.Write))
                            {
                                result.IsReadOnly = true;
                            }
                        }

					}
				}
			}

			return dataView;
		}

        private DataViewModel GetClonedDataView(DataViewModel originalDataView)
        {
            DataViewModel clonedDataView = originalDataView.Clone();

            // obtain the related classes to the base class that the current user has permission to read
            clonedDataView.BaseClass.RelatedClasses = GetRelatedClasses(clonedDataView.BaseClass);

            return clonedDataView;
        }

		/// <summary>
		/// Add bottom classes to the collection.
		/// </summary>
		/// <param name="bottomClasses">A collection of bottom classes.</param>
		/// <param name="childClasses">The collection of child classes</param>
		private void AddBottomClasses(SchemaModelElementCollection bottomClasses,
			SchemaModelElementCollection childClasses)
		{
			if (childClasses == null)
			{
				return;
			}

			foreach (ClassElement childClass in childClasses)
			{
				// make sure that only show the bottom classes that current principal
				// has permission to see
				if (PermissionChecker.Instance.HasPermission(this.XaclPolicy, childClass, XaclActionType.Read))
				{
					if (childClass.IsLeaf)
					{
						bottomClasses.Add(childClass);
					}

					// append the other bottom classes reursively
					AddBottomClasses(bottomClasses, childClass.Subclasses);
				}
			}
		}

		/// <summary>
		/// Gets the inherited and local attributes (Simple, virtual, and relationship) of a class
		/// that the principal has permission to read.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of attributes</returns>
		private IList GetReadableAttributes(ClassElement classElement)
		{
			MetaDataElementSortedList sortedList = new MetaDataElementSortedList();
				
			ClassElement currentClass = classElement;
			int level = 10; // estimate of maximum levels of inheritance
			while (currentClass != null)
			{
				level--;
				if (level < 0)
				{
					level = 0;
				}

				SchemaModelElementCollection localSimpleAttributes = currentClass.SimpleAttributes;
				
				if (localSimpleAttributes != null)
				{	
					foreach (SchemaModelElement att in localSimpleAttributes)
					{
						// exclude the attributes that the principal does not have read
						// permission
						if (!CheckPermission || PermissionChecker.Instance.HasPermission(this.XaclPolicy, att, XaclActionType.Read))
						{
							// attributes of parent appears first
							sortedList.Add(level * 1000 + att.Position, att);
						}
					}
				}

				SchemaModelElementCollection localArrayAttributes = currentClass.ArrayAttributes;
				
				if (localArrayAttributes != null)
				{	
					foreach (SchemaModelElement att in localArrayAttributes)
					{
						// exclude the attributes that the principal does not have read
						// permission
						if (!CheckPermission || PermissionChecker.Instance.HasPermission(this.XaclPolicy, att, XaclActionType.Read))
						{	
							// attributes of parent appears first
							sortedList.Add(level * 1000 + att.Position, att);
						}
					}
				}

                SchemaModelElementCollection localVirtualAttributes = currentClass.VirtualAttributes;

                if (localVirtualAttributes != null)
                {
                    foreach (SchemaModelElement att in localVirtualAttributes)
                    {
                        // exclude the attributes that the principal does not have read
                        // permission
                        if (!CheckPermission || PermissionChecker.Instance.HasPermission(this.XaclPolicy, att, XaclActionType.Read))
                        {
                            // attributes of parent appears first
                            sortedList.Add(level * 1000 + att.Position, att);
                        }
                    }
                }

                SchemaModelElementCollection localImageAttributes = currentClass.ImageAttributes;

                if (localImageAttributes != null)
                {
                    foreach (SchemaModelElement att in localImageAttributes)
                    {
                        // exclude the attributes that the principal does not have read
                        // permission
                        if (!CheckPermission || PermissionChecker.Instance.HasPermission(this.XaclPolicy, att, XaclActionType.Read))
                        {
                            // attributes of parent appears first
                            sortedList.Add(level * 1000 + att.Position, att);
                        }
                    }
                }
					
				SchemaModelElementCollection localRelationshipAttributes = currentClass.RelationshipAttributes;
				
				if (localRelationshipAttributes != null)
				{	
					foreach (RelationshipAttributeElement relationship in localRelationshipAttributes)
					{
						// exclude the relationships if the principal does not have read
						// permission or if the related class the principal does not have read permission
						if (!CheckPermission ||
                            (PermissionChecker.Instance.HasPermission(this.XaclPolicy, relationship, XaclActionType.Read) &&
                             PermissionChecker.Instance.HasPermission(this.XaclPolicy, relationship.LinkedClass, XaclActionType.Read)))
						{	
							// attributes of parent appears first
                            sortedList.Add(level * 1000 + relationship.Position, relationship);
						}
					}
				}

				currentClass = currentClass.ParentClass;
			}
				
			return sortedList.Values;
		}

		/// <summary>
		/// Gets the information indicating whether a SimpleAttributeElement is in one of
		/// the provided sections
		/// </summary>
		/// <param name="attributeSection">The attribute section</param>
		/// <param name="sections">The sections array</param>
		/// <returns>true if the attribute is in one of the sections, false otherwise.</returns>
		private bool IsInSections(string attributeSection, string[] sections)
		{
			foreach (string section in sections)
			{
				if (attributeSection != null &&
					attributeSection.ToUpper() == section.ToUpper())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// A handler to call when a value of any of the meta data changed
		/// </summary>
		/// <param name="sender">the IMetaDataElement that cause the event</param>
		/// <param name="e">the arguments</param>
		private void ValueChangedHandler(object sender, EventArgs e)
		{
			// when any part of data model changed, mark the meta data as needing to save
			NeedToSave = true;
		}

        /// <summary>
        /// Gets the information indicating whether the simple attribute is specified for sorting.
        /// </summary>
        /// <param name="baseClass">The base class element.</param>
        /// <param name="simpleAttribute">The simple attribute</param>
        /// <returns>true if it is used for sorting, false otherwise.</returns>
        private bool IsSortAttribute(ClassElement baseClass, SimpleAttributeElement simpleAttribute, out SortDirection sortDirection)
        {
            bool status = false;
            sortDirection = SortDirection.Ascending;

            ClassElement currentClass = baseClass;
            while (currentClass != null)
            {
                if (simpleAttribute.Name == currentClass.SortAttribute)
                {
                    sortDirection = currentClass.SortDirection;
                    status = true;
                    break;
                }

                currentClass = currentClass.ParentClass;
            }

            return status;
        }

        /// <summary>
        /// Get a collection of the DataClass objects that represents the data classes that are
        /// linked to a base data class through the relationship attributes
        /// </summary>
        public ReferencedClassCollection GetRelatedClasses(DataClass baseClass)
        {
            ReferencedClassCollection referencedClasses = new ReferencedClassCollection();

            ClassElement currentClassElement = baseClass.GetSchemaModelElement() as ClassElement;
            DataClass relatedClass;
            ClassElement relatedClassElement;
            while (currentClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable &&
                        (!CheckPermission ||
                          PermissionChecker.Instance.HasPermission(this.XaclPolicy, relationshipAttribute, XaclActionType.Read) &&
                          PermissionChecker.Instance.HasPermission(this.XaclPolicy, relationshipAttribute.LinkedClass, XaclActionType.Read)))
                    {
                        if (relationshipAttribute.LinkedClass.IsJunction &&
                            relationshipAttribute.Type == RelationshipType.OneToMany)
                        {
                            // the linked class is a junction class for many-to-many relationship
                            ClassElement junctionClass = relationshipAttribute.LinkedClass;

                            // first find the many-to-one relationship that connects to the related class
                            // on the other side of many-to-many relationship
                            RelationshipAttributeElement toReferencedClsRelationshipAttribute = junctionClass.FindPairedRelationshipAttribute(relationshipAttribute);

                            // create a related class representing the class on the other side of
                            // the junction class
                            relatedClass = new DataClass(toReferencedClsRelationshipAttribute.LinkedClassAlias,
                                toReferencedClsRelationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            relatedClass.ReferringClassAlias = baseClass.Alias; // use the alias of the base class as referring class alias
                            relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                            relatedClass.Caption = toReferencedClsRelationshipAttribute.LinkedClass.Caption;
                            relatedClass.ReferringRelationship = relationshipAttribute; // relationhsip atribute from the referring class as the referring relationship
                            relatedClassElement = SchemaModel.FindClass(toReferencedClsRelationshipAttribute.LinkedClassName);
                            relatedClass.IsLeafClass = relatedClassElement.IsLeaf;
                        }
                        else
                        {
                            relatedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                                relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                            relatedClass.ReferringClassAlias = baseClass.Alias;
                            relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                            relatedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                            relatedClass.ReferringRelationship = relationshipAttribute;
                            relatedClassElement = SchemaModel.FindClass(relationshipAttribute.LinkedClassName);
                            relatedClass.IsLeafClass = relatedClassElement.IsLeaf;
                        }

                        referencedClasses.Add(relatedClass);
                    }
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            return referencedClasses;
        }

        /// <summary>
        /// Gets the corresponding element type representing the default operator of a
        /// simple attribute.
        /// </summary>
        /// <param name="simpleAttribute">The simple attribute</param>
        /// <returns>The Element Type representing an operator</returns>
        private ElementType GetOperatorType(SimpleAttributeElement simpleAttribute)
        {
            ElementType operatorType = ElementType.Equals;
            if (!string.IsNullOrEmpty(simpleAttribute.Operator))
            {
                switch (simpleAttribute.Operator)
                {
                    case OPT_EQUALS:
                        operatorType = ElementType.Equals;
                        break;
                    case OPT_NOT_EQUALS:
                        operatorType = ElementType.NotEquals;
                        break;
                    case OPT_GREATER_THAN:
                        operatorType = ElementType.GreaterThan;
                        break;
                    case OPT_LESS_THAN:
                        operatorType = ElementType.LessThan;
                        break;
                    case OPT_GREATER_THAN_EQUALS:
                        operatorType = ElementType.GreaterThanEquals;
                        break;
                    case OPT_LESS_THAN_EQUALS:
                        operatorType = ElementType.LessThanEquals;
                        break;
                    case OPT_LIKE:
                        operatorType = ElementType.Like;
                        break;
                }
            }

            return operatorType;
        }

		#region IXaclObject

		/// <summary>
		/// Return a xpath for the SchemaModel
		/// </summary>
		/// <returns>a xapth string</returns>
		public string ToXPath()
		{
			return _schemaModel.SchemaInfo.ToXPath();
		}

		/// <summary>
		/// Gets parent of IXaclObject
		/// </summary>
		/// <returns>null since the schema model is a root.</returns>
		public IXaclObject Parent
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets children of the SchemaModel
		/// </summary>
		/// <returns>The collection of IXaclObject nodes for root classes</returns>
		public IEnumerator GetChildren()
		{
			ArrayList children = new ArrayList();
			children.Add(_schemaModel);
			children.Add(_dataViews);
			children.Add(_taxonomies);
            children.Add(_xmlSchemaViews);

			return children.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Specify the types of meta data model
	/// </summary>
	public enum MetaDataType
	{
		/// <summary>
		/// Schema
		/// </summary>
		Schema = 0,
		/// <summary>
		/// DataViews
		/// </summary>
		DataViews,
		/// <summary>
		/// XaclPolicy
		/// </summary>
		XaclPolicy,
		/// <summary>
		/// Taxonomies
		/// </summary>
		Taxonomies,
		/// <summary>
		/// Rules
		/// </summary>
		Rules,
		/// <summary>
		/// Mappings
		/// </summary>
		Mappings,
		/// <summary>
		/// Selectors
		/// </summary>
		Selectors,
        /// <summary>
        /// 
        /// </summary>
        Events,
        /// <summary>
        /// LoggingPolicy
        /// </summary>
        LoggingPolicy,
		/// <summary>
		/// FileTypeInfo
		/// </summary>
		FileTypeInfo,
        /// <summary>
        /// Subscribers
        /// </summary>
        Subscribers,
        /// <summary>
        /// XML Schema Views
        /// </summary>
        XMLSchemaViews,
        /// <summary>
        /// Apis
        /// </summary>
        Apis
	}
}