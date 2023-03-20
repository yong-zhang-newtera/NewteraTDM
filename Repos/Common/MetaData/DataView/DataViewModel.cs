/*
* @(#)DataViewModel.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml;
	using System.Data;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.DataView.QueryBuilder;
	using Newtera.Common.MetaData.DataView.Validate;
    using Newtera.Common.MetaData.Rules;

	/// <summary>
	/// A DataViewModel is an object-representation of a specific data view on the
	/// instance data of a class. It includes a base class, a set of referenced classes,
	/// search filters, and a set of attributes to be shown in the view.
	/// 
	/// A DataViewModel is used in two areas: to generate an XQuery to fetch the data
	/// from database, and to provide information for user interface creation.
	/// 
	/// A DataViewModel can be constructed programatically or from an XML data. It can be
	/// saved as an XML data too.
	/// </summary>
	/// 
	/// <version>1.0.1 28 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class DataViewModel : DataViewElementBase, IXaclObject
	{
		/// <summary>
		/// The default page size
		/// </summary>
		public const int DEFAULT_PAGE_SIZE = 15;
		/// <summary>
		/// The default count of a page
		/// </summary>
		public const int DEFAULT_PAGE_COUNT = 10000;

		private SchemaInfo _schemaInfo;
		private SchemaModel _schemaModel;
		private DataClass _baseClass;
		private ReferencedClassCollection _referencedClasses;
		private Filter _filter;
		private ResultAttributeCollection _resultAttributes;
		private SortBy _sortBy;
		private XQueryBuilder _queryBuilder;
		private int _pageIndex; // run-time use only
		private int _pageSize;  // run-time use only
		private string _objId; // run-time use only
		private DataViewModelCollection _container; // run-time use only
		private string _xpath; // run-time use only
        private bool _verifyChanges = true; // run-time use only

		/// <summary>
		/// Initiating an instance of DataViewModel class
		/// </summary>
		/// <param name="name">Name of the data view</param>
		/// <param name="schemaInfo">The SchemaInfo object</param>
		/// <param name="schemaModel">The SchemaModel object</param>
		public DataViewModel(string name, SchemaInfo schemaInfo, SchemaModel schemaModel) : base(name)
		{
			_schemaInfo = schemaInfo;
			_schemaModel = schemaModel;
			_baseClass = null;
			_referencedClasses = new ReferencedClassCollection();
			_referencedClasses.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _referencedClasses.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_resultAttributes = new ResultAttributeCollection();
			_resultAttributes.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _resultAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_filter = new Filter();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _filter.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_sortBy = new SortBy();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _sortBy.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			_pageIndex = 0;
			_pageSize = DEFAULT_PAGE_SIZE;
			_objId = null;
			_queryBuilder = new XQueryBuilder(this);
			_container = null;
			_xpath = null;
		}

		/// <summary>
		/// Initiating an instance of DataViewModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataViewModel(XmlElement xmlElement) : base()
        {
            _schemaInfo = null;
            _schemaModel = null;
            Unmarshal(xmlElement);

            // register the value change handlers
            _baseClass.DataView = this;
            _referencedClasses.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _referencedClasses.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _resultAttributes.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _resultAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _filter.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _filter.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _sortBy.DataView = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _sortBy.ValueChanged += new EventHandler(ValueChangedHandler);
            }

            _pageIndex = 0;
            _pageSize = DEFAULT_PAGE_SIZE;
            _objId = null;
            _queryBuilder = new XQueryBuilder(this);
            _container = null;
            _xpath = null;
        }

		/// <summary>
		/// Gets or sets the schema info of the data view.
		/// </summary>
		/// <value>The SchemaInfo instance</value>
		[BrowsableAttribute(false)]	 
		public SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the schema model of the data view.
		/// </summary>
		/// <value>The SchemaModel instance</value>
		[BrowsableAttribute(false)]			
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
			set
			{
				_schemaModel = value;
			}
		}

		/// <summary>
		/// Gets or sets the container of the data view model object
		/// </summary>
		/// <value>A DataViewModelCollection object.</value>
		[BrowsableAttribute(false)]		
		public DataViewModelCollection Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		/// <summary>
		/// Gets the base class of the data view.
		/// </summary>
		/// <value>The base class</value>
		[BrowsableAttribute(false)]		
		public DataClass BaseClass
		{
			get
			{
				return _baseClass;
			}
			set
			{
				_baseClass = value;
				value.DataView = this;
                if (GlobalSettings.Instance.IsWindowClient)
                {
                    _baseClass.ValueChanged += new EventHandler(ValueChangedHandler);

                    FireValueChangedEvent(value);
                }
			}
		}

		/// <summary>
		/// Gets the referenced classes
		/// </summary>
		/// <value>A collection of DataClass instances</value>
		[BrowsableAttribute(false)]		
		public ReferencedClassCollection ReferencedClasses
		{
			get
			{
				return _referencedClasses;
			}
		}

		/// <summary>
		/// Gets the referenced classes
		/// </summary>
		/// <value>A collection of DataClass instances</value>
		[BrowsableAttribute(false)]		
		public ResultAttributeCollection ResultAttributes
		{
			get
			{
				return _resultAttributes;
			}
		}
		
		/// <summary>
		/// Gets filter expression of the DataViewModel
		/// </summary>
		/// <value>A Filter object</value>
		[BrowsableAttribute(false)]
        public IDataViewElement FilterExpr
		{
			get
			{
				return _filter.Expression;
			}
            set
            {
                _filter.Expression = value;
                FireValueChangedEvent(value);
            }
		}

		/// <summary>
		/// Gets SortBy of the DataViewModel
		/// </summary>
		/// <value>A SortBy object</value>
		[BrowsableAttribute(false)]		
		public SortBy SortBy
		{
			get
			{
				return _sortBy;
			}
            set
            {
                _sortBy = value;
                FireValueChangedEvent(value);
            }
		}

		/// <summary>
		/// Gets the flattened search filter expressions
		/// </summary>
		/// <value>An DataViewElementCollection</value>
		[BrowsableAttribute(false)]		
		public DataViewElementCollection FlattenedSearchFilters
		{
			get
			{
                FlattenSearchFiltersVisitor visitor = new FlattenSearchFiltersVisitor();

                if (_filter.Expression != null)
                {
                    _filter.Expression.Accept(visitor);
                }

                return visitor.FlattenedSearchFilters;
			}
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		[BrowsableAttribute(false)]		
		public override DataViewModel DataView
		{
			get
			{
				return this;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		[BrowsableAttribute(false)]		
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.View;
			}
		}

		/// <summary>
		/// Gets or sets the id of an instance whose values are currently being
		/// kept in result attributes. This is to serve the edit query building.
		/// </summary>
		/// <value>The id of an instance</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		[BrowsableAttribute(false)]			
		public string CurrentObjId
		{
			get
			{
				return _objId;
			}
			set
			{
				_objId = value;
			}
		}

		/// <summary>
		/// Gets or sets the index of current page.
		/// </summary>
		/// <value>The page index</value>
		[BrowsableAttribute(false)]		
		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of a page
		/// </summary>
		/// <value>Page size</value>
		[BrowsableAttribute(false)]		
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

        /// <summary>
        /// Gets or sets information indicate whether to verify if the attribute values have been
        /// changed when generating update query.
        /// </summary>
        /// <value>true to verify when generating update query, false otherwise. true is default</value>
        [BrowsableAttribute(false)]
        public bool VerifyChanges
        {
            get
            {
                return _verifyChanges;
            }
            set
            {
                _verifyChanges = value;
            }
        }

		/// <summary>
		/// Gets the search XQuery of the data view
		/// </summary>
		/// <value>An XQuery for search</value>
		[BrowsableAttribute(false)]		
		public string SearchQuery
		{
			get
			{
				return _queryBuilder.GenerateSearchQuery(false);
			}
		}

        /// <summary>
        /// Gets the search XQuery of the data view that includes primary keys of referenced
        /// classes as results
        /// </summary>
        /// <value>An XQuery for search</value>
        [BrowsableAttribute(false)]
        public string SearchQueryWithPKValues
        {
            get
            {
                return _queryBuilder.GenerateSearchQuery(true);
            }
        }

		/// <summary>
		/// Gets the insert XQuery of the data view
		/// </summary>
		/// <value>An XQuery for insert</value>
		[BrowsableAttribute(false)]		
		public string InsertQuery
		{
			get
			{
				return _queryBuilder.GenerateInsertQuery();
			}
		}

		/// <summary>
		/// Gets the update XQuery of the data view
		/// </summary>
		/// <value>An XQuery for update</value>
		[BrowsableAttribute(false)]		
		public string UpdateQuery
		{
			get
			{
				return _queryBuilder.GenerateUpdateQuery(_verifyChanges);
			}
		}

		/// <summary>
		/// Gets the delete XQuery of the data view
		/// </summary>
		/// <value>An XQuery for delete</value>
		[BrowsableAttribute(false)]		
		public string DeleteQuery
		{
			get
			{
				return _queryBuilder.GenerateDeleteQuery();
			}
		}

		/// <summary>
		/// Gets a query that search for a particular instance of given id.
		/// </summary>
		/// <param name="instanceId">The instance id</param>
		/// <returns>The instance query</returns>
		public string GetInstanceQuery(string instanceId)
		{
            return GetInstanceQuery(instanceId, true);
		}

        /// <summary>
        /// Gets a query that search for a particular instance of given id.
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <param name="ignoreFilter">true to ignore the search filter, false to use the search filter.</param>
        /// <returns>The instance query</returns>
        public string GetInstanceQuery(string instanceId, bool ignoreFilter)
        {
            IDataViewElement expr = _filter.Expression;

            try
            {
                // A query for searching an instance just need instance id as condition
                // replace the search expression with special condition
                IDataViewElement left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, _baseClass.Alias);
                Parameter right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, _baseClass.Alias, DataType.String);
                right.ParameterValue = instanceId;
                RelationalExpr objIdExpr = new RelationalExpr(ElementType.Equals, left, right);

                if (ignoreFilter)
                {
                    _filter.Expression = objIdExpr;
                }
                else
                {
                    _filter.Expression = new LogicalExpr(ElementType.And, expr, objIdExpr);
                }

                return _queryBuilder.GenerateInstanceQuery();
            }
            finally
            {
                // restore the original search condition
                _filter.Expression = expr;
            }
        }

		/// <summary>
		/// Gets a query that update obj_id(s) of referenced instances by
		/// an instance.
		/// </summary>
		/// <param name="instanceData">The instance data.</param>
		/// <returns>The query that updates referenced obj_id(s)</returns>
		public string GetReferenceUpdateQuery(InstanceData instanceData)
		{
			return _queryBuilder.GenerateReferenceUpdateQuery(instanceData);
		}

		/// <summary>
		/// Gets a query that search for a set of instances of a class given a
		/// collection of instance ids.
		/// </summary>
		/// <param name="instanceIds">A collection of instance ids</param>
		/// <returns>The query that retrives a set of instances</returns>
		public string GetInstancesQuery(StringCollection instanceIds)
		{
			IDataViewElement expr = _filter.Expression;

			try
			{
				// A query for searching a set of instances
				// replace the search expression with special condition
				IDataViewElement left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, _baseClass.Alias);
				
				ParameterCollection parameters = new ParameterCollection();
				foreach (string instanceId in instanceIds)
				{
					Parameter parameter = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, _baseClass.Alias, DataType.String);
					parameter.ParameterValue = instanceId;

					parameters.Add(parameter);
				}

				InExpr inExpr = new InExpr(ElementType.In, left, parameters);
				_filter.Expression = inExpr;

				return _queryBuilder.GenerateInstanceQuery();
			}
			finally
			{
				// restore the original search condition
				_filter.Expression = expr;
			}
		}

        /// <summary>
        /// Gets a query that search for a set of instances of a class given an
        /// attribute value, the attribute can be a simple attribute or a relationship attribute.
        /// </summary>
        /// <param name="attributeElement">The attribute element.</param>
        /// <returns>The query that retrives instances that satisfied the search condition.</returns>
        public string GetInstancesQuery(IDataViewElement attributeElement)
        {
            // the attribute element contains the value
            return GetInstancesQuery(attributeElement, null);
        }

        /// <summary>
        /// Gets a query that search for a set of instances of a class given an
        /// attribute value, the attribute can be a simple attribute or a relationship attribute.
        /// </summary>
        /// <param name="attributeElement">The attribute element.</param>
        /// <param name="searchValue">The search value.</param>
        /// <returns>The query that retrives instances that satisfied the search condition.</returns>
        public string GetInstancesQuery(IDataViewElement attributeElement, string searchValue)
        {
            string query = null;

            // clear the search values in the data model
            ClearSearchValuesVisitor visitor = new ClearSearchValuesVisitor();
            if (_filter.Expression != null)
            {
                _filter.Expression.Accept(visitor); // clear the search values
            }

            // A query for searching a set of instances
            // replace the search expression with special condition
            if (attributeElement is DataSimpleAttribute)
            {
                DataSimpleAttribute simpleAttribute = attributeElement as DataSimpleAttribute;

                if (string.IsNullOrEmpty(searchValue))
                {
                    SetSearchValue(simpleAttribute.OwnerClassAlias,
                        simpleAttribute.Name, simpleAttribute.AttributeValue);
                }
                else
                {
                    SetSearchValue(simpleAttribute.OwnerClassAlias,
                        simpleAttribute.Name, searchValue);
                }

                query = _queryBuilder.GenerateSearchQuery(false);
            }
            else if (attributeElement is DataRelationshipAttribute)
            {
                DataRelationshipAttribute relationshipAttribute = attributeElement as DataRelationshipAttribute;
                string val = searchValue;
                if (string.IsNullOrEmpty(val))
                {
                    foreach (DataSimpleAttribute pk in relationshipAttribute.PrimaryKeys)
                    {
                        if (string.IsNullOrEmpty(val))
                        {
                            val = pk.AttributeValue;
                        }
                        else
                        {
                            val += "&" + pk.AttributeValue;
                        }
                    }
                }

                SetSearchValue(relationshipAttribute.OwnerClassAlias, relationshipAttribute.Name, val);

                query = _queryBuilder.GenerateSearchQuery(false);
            }

            return query;
        }

		/// <summary>
		/// Gets a query that search for a particular instance based on primary
		/// key(s).
		/// </summary>
		/// <returns>A search query that retrieves an instance by primary key(s),
		/// null if the data view model does not have primary key attribute(s), or
		/// the values(s) of primary key(s) do not exist.</returns>
		public string GetInstanceByPKQuery()
		{
			bool hasPKValues = false;

            // clear the search values in the data model
            ClearSearchValuesVisitor visitor = new ClearSearchValuesVisitor();
            if (_filter.Expression != null)
            {
                _filter.Expression.Accept(visitor); // clear the search value
            }

			// set the value(s) of primary key(s) as search values
			foreach (IDataViewElement element in _resultAttributes)
			{
				if (element is DataSimpleAttribute)
				{
					DataSimpleAttribute simpleAttribute = (DataSimpleAttribute) element;
					SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) simpleAttribute.GetSchemaModelElement();
					if (schemaModelElement.IsPrimaryKey &&
						simpleAttribute.AttributeValue != null &&
						simpleAttribute.AttributeValue.Length > 0)
					{
						// set the primary key value as a search value
						SetSearchValue(simpleAttribute.OwnerClassAlias,
							simpleAttribute.Name, simpleAttribute.AttributeValue);
						hasPKValues = true;
					}
				}
			}

			if (hasPKValues)
			{
				return _queryBuilder.GenerateSearchQuery(true, true);
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Gets a query that search for a particular instance based on primary
        /// key(s).
        /// </summary>
        /// <param name="primaryKeys">The primary key values separated by &</param>
        /// <returns>A search query that retrieves an instance by primary key(s),
        /// null if the data view model does not have primary key attribute(s), or
        /// the values(s) of primary key(s) do not exist.</returns>
        public string GetInstanceByPKQuery(string primaryKeyValues)
        {
            bool hasPKValues = false;
            string[] pkArray = null;

            if (!string.IsNullOrEmpty(primaryKeyValues))
            {
                pkArray = primaryKeyValues.Split('&');

                // clear the search values in the data model
                ClearSearchValuesVisitor visitor = new ClearSearchValuesVisitor();
                if (_filter.Expression != null)
                {
                    _filter.Expression.Accept(visitor); // clear the search value
                }

                int index = 0;
                // set the value(s) of primary key(s) as search values
                foreach (IDataViewElement element in _resultAttributes)
                {
                    if (element is DataSimpleAttribute)
                    {
                        DataSimpleAttribute simpleAttribute = (DataSimpleAttribute)element;
                        SimpleAttributeElement schemaModelElement = (SimpleAttributeElement)simpleAttribute.GetSchemaModelElement();
                        if (schemaModelElement.IsPrimaryKey &&
                            index < pkArray.Length)
                        {
                            // set the primary key value as a search value
                            SetSearchValue(simpleAttribute.OwnerClassAlias,
                                simpleAttribute.Name, pkArray[index]);
                            hasPKValues = true;
                            index++;
                        }
                    }
                }
            }

            if (hasPKValues)
            {
                return _queryBuilder.GenerateSearchQuery(true, true);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a query that search for a particular instance based on value of attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute whose value is used in query.</param>
        /// <param name="attributeValue">The value of attribute</param>
        /// <param name="dataType">The data type of the attribute</param>
        /// <returns>A search query that retrieves an instance by attribute value,
        /// null if the value of attribute does not exist.</returns>
        public string GetInstanceByAttributeValueQuery(string attributeName, string attributeValue)
        {
            return GetInstanceByAttributeValueQuery(attributeName, attributeValue, DataType.Unknown);
        }

        /// <summary>
        /// Gets a query that search for a particular instance based on value of attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute whose value is used in query.</param>
        /// <param name="attributeValue">The value of attribute</param>
        /// <param name="dataType">The data type of the attribute</param>
        /// <returns>A search query that retrieves an instance by attribute value,
        /// null if the value of attribute does not exist.</returns>
        public string GetInstanceByAttributeValueQuery(string attributeName, string attributeValue, DataType attributeDataType)
        {
            IDataViewElement expr = _filter.Expression;
            DataType dataType = attributeDataType;
            try
            {
                if (dataType == DataType.Unknown)
                {
                    FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(BaseClass.Alias, attributeName);

                    if (_filter.Expression != null)
                    {
                        _filter.Expression.Accept(visitor);
                        if (visitor.SearchParameter != null)
                        {
                            dataType = visitor.SearchParameter.DataType;
                        }
                    }
                }

                ClearSearchExpression();

                DataSimpleAttribute left = new DataSimpleAttribute(attributeName, BaseClass.Alias);
                Parameter right = new Parameter(attributeName, BaseClass.Alias, dataType);
                right.ParameterValue = attributeValue;
                RelationalExpr bExpr = new RelationalExpr(ElementType.Equals, left, right);
                AddSearchExpr(bExpr, ElementType.And);

                string query = _queryBuilder.GenerateSearchQuery(true,true); // get an instance query that includes the parimary key values of all forward relatiosnhip attributes

                return query;
            }
            finally
            {
                // restore the original search condition
                _filter.Expression = expr;
            }
        }

        /// <summary>
        /// Gets a query that returns values of an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute whose value is used in query.</param>
        /// <returns>A search query that retrieves values of an attribute.</returns>
        public string GetAttributeValueQuery(string attributeName, string ownerClassAlias)
        {
            ResultAttributeCollection oldResults = new ResultAttributeCollection();

            try
            {
                // Remove original result attributes from data view
                foreach (IDataViewElement result in ResultAttributes)
                {
                    oldResults.Add(result);
                }
                ResultAttributes.Clear();

                ResultAttributes.Add(new DataSimpleAttribute(attributeName, ownerClassAlias));
                
                string query = _queryBuilder.GenerateSearchQuery(false);

                return query;
            }
            finally
            {
                // restore the original result attribute
                ResultAttributes.Clear();
                foreach (IDataViewElement result in oldResults)
                {
                    ResultAttributes.Add(result);
                }
            }
        }

        /// <summary>
        /// Gets a query that validates the data instance based on the given rule.
        /// </summary>
        /// <param name="ruleDef">The rule contains definition.</param>
        /// <returns>The query that validates a data instance.</returns>
        public string GetRuleValidatingQuery(RuleDef ruleDef)
        {
            return _queryBuilder.GenerateRuleValidatingQuery(ruleDef);
        }

        /// <summary>
        /// Gets the information indicating whether any values in an unique keys of a class
        /// have been changed or not.
        /// </summary>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <returns>True if it is changed, false otherwise</returns>
        public bool IsUniqueKeyValuesChanged(string className)
        {
            bool status = false;

            ClassElement currentClass = (ClassElement)this.BaseClass.GetSchemaModelElement();
            while (currentClass != null)
            {
                if (currentClass.Name == className &&
                    currentClass.UniqueKeys.Count > 0)
                {
                    foreach (SchemaModelElement key in currentClass.UniqueKeys)
                    {
                        if (_resultAttributes[key.Name].IsValueChanged)
                        {
                            status = true;
                            break;
                        }
                    }

                    break;
                }

                currentClass = currentClass.ParentClass;
            }

            return status;
        }

        /// <summary>
        /// Gets a query that search for a particular instance based on unique keys defined for
        /// the class.
        /// </summary>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <returns>A search query that retrieves an instance by unique keys,
        /// null if the data view model does not have unique keys, or
        /// the values(s) of unique key(s) do not exist.</returns>
        public string GetInstanceByUniqueKeysQuery(string className)
        {
            bool hasUQValues = false;

            ClassElement currentClass = (ClassElement)this.BaseClass.GetSchemaModelElement();
            while (currentClass != null)
            {
                if (currentClass.Name == className &&
                    currentClass.UniqueKeys.Count > 0)
                {
                    // clear the search values in the data model
                    ClearSearchValuesVisitor visitor = new ClearSearchValuesVisitor();
                    if (_filter.Expression != null)
                    {
                        _filter.Expression.Accept(visitor); // clear the search value
                    }

                    // set the value(s) of unique key(s) as search values
                    foreach (IDataViewElement element in _resultAttributes)
                    {
                        if (element is DataSimpleAttribute)
                        {
                            DataSimpleAttribute simpleAttribute = (DataSimpleAttribute)element;

                            if (IsUniqueKey(simpleAttribute, currentClass) &&
                                simpleAttribute.AttributeValue != null &&
                                simpleAttribute.AttributeValue.Length > 0)
                            {
                                // set the attribute value as a search value
                                SetSearchValue(simpleAttribute.OwnerClassAlias,
                                    simpleAttribute.Name, simpleAttribute.AttributeValue);
                                hasUQValues = true;
                            }
                        }
                        else if (element is DataRelationshipAttribute)
                        {
                            DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute)element;
                            string val = "";
                            // get search value from the primary keys
                            foreach (DataSimpleAttribute pk in relationshipAttribute.PrimaryKeys)
                            {
                                if (string.IsNullOrEmpty(val))
                                {
                                    val = pk.AttributeValue;
                                }
                                else
                                {
                                    val += "&" + pk.AttributeValue;
                                }
                            }

                            if (IsUniqueKey(relationshipAttribute, currentClass) &&
                                !string.IsNullOrEmpty(val))
                            {
                                // set the attribute value as a search value
                                SetSearchValue(relationshipAttribute.OwnerClassAlias,
                                    relationshipAttribute.Name, val);
                                hasUQValues = true;
                            }
                        }
                    }

                    break;
                }

                currentClass = currentClass.ParentClass;
            }

            if (hasUQValues)
            {
                return _queryBuilder.GenerateSearchQuery(false);
            }
            else
            {
                return null;
            }
        }

		/// <summary>
		/// Validate an instance data kept in the result attributes based on the rules
		/// of the schema model.
		/// </summary>
		/// <returns>A DataViewValidateResult</returns>
		public DataViewValidateResult ValidateData()
		{
			InstanceDataValidateVisitor visitor = new InstanceDataValidateVisitor(this);

            visitor.VisitDataClass(_baseClass);

			_resultAttributes.Accept(visitor);

			return visitor.ValidateResult;
		}

		/// <summary>
		/// Validate the data view to see whether it is in sync with schema model.
		/// </summary>
		/// <returns>A DataViewValidateResult</returns>
		public DataViewValidateResult ValidateDataView()
		{
			DataViewValidateVisitor visitor = new DataViewValidateVisitor();

			this.Accept(visitor);

			return visitor.ValidateResult;
		}

        /// <summary>
        /// Gets the information indicating whether a given attribute is part of search expression
        /// of the data view
        /// </summary>
        /// <param name="classAlias">Alias of owner class</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>true if the search expression exist, false otherwise</returns>
        public bool IsSearchAttributeExist(string classAlias, string attributeName)
        {
            bool status = false;

            FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(classAlias, attributeName);

            if (_filter.Expression != null)
            {
                _filter.Expression.Accept(visitor);
  
                if (visitor.SearchParameter != null)
                {
                    status = true;
                }
            }

            return status;
        }

		/// <summary>
		/// Set a search value to a attribute identified with an owner class alias and a name. This
        /// method does not function correctly if the search expression of the data view contains
        /// the same attribute more than once (e.g. A < 10 and A > 2)
		/// </summary>
		/// <param name="classAlias">The alias of the attribute owner class.</param>
		/// <param name="attributeName">The name of an attribute</param>
		/// <param name="searchValue">A search value</param>
		public void SetSearchValue(string classAlias, string attributeName, string searchValue)
		{
			FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(classAlias, attributeName);

			if (_filter.Expression != null)
			{
				_filter.Expression.Accept(visitor);
				if (visitor.SearchParameter != null)
				{
					visitor.SearchParameter.ParameterValue = searchValue;
				}
			}
		}

        /// <summary>
        /// Set a search value to a attribute identified with an alias. This
        /// method can function correctly even when the search expression of the data view contains
        /// the same attribute more than once (e.g. A < 10 and A > 2)
        /// </summary>
        /// <param name="attributeAlias">The alias of the attribute which is unique among the search expression.</param>
        /// <param name="searchValue">A search value</param>
        public void SetSearchValue(string attributeAlias, string searchValue)
        {
            FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(attributeAlias);

            if (_filter.Expression != null)
            {
                _filter.Expression.Accept(visitor);
                if (visitor.SearchParameter != null)
                {
                    visitor.SearchParameter.ParameterValue = searchValue;
                }
            }
        }

		/// <summary>
		/// Clear the search expression
		/// </summary>
		public void ClearSearchExpression()
		{
			FilterExpr = null;
		}

		/// <summary>
		/// Clear the sortBy expression
		/// </summary>
		public void ClearSortBy()
		{
			_sortBy.SortAttributes.Clear();

			// fire an event for clear the sortby
			FireValueChangedEvent(null);
		}

		/// <summary>
		/// Add a search expression to the data view with a logical operator.
		/// If there exists an expression, the new expression is appended to the
		/// end.
		/// </summary>
		/// <param name="expr">The expression to be appended.</param>
		/// <param name="type">The logical operator type, either And or Or</param>
		public void AddSearchExpr(IDataViewElement expr, ElementType type)
		{
			IDataViewElement existing = _filter.Expression;

			if (existing != null)
			{
				LogicalExpr logicalExpr;
				switch (type)
				{
					case ElementType.And:
						logicalExpr = new LogicalExpr(ElementType.And, existing, expr);
						break;
					case ElementType.Or:
						logicalExpr = new LogicalExpr(ElementType.Or, existing, expr);
						break;
					default:
						// default is And operator
						logicalExpr = new LogicalExpr(ElementType.And, existing, expr);
						break;
				}
				this.FilterExpr = logicalExpr;
			}
			else
			{
				// the first expression, set it as root
                this.FilterExpr = expr;
			}

			expr.DataView = this;
		}

		/// <summary>
		/// Remove the last relational expression from the search expression.
		/// </summary>
		/// <remarks>If there doesn't exist a search expression, this method does nothing.</remarks>
		public void RemoveLastSearchExpr()
		{
			IDataViewElement existing = _filter.Expression;

			if (existing != null)
			{
				if (existing is LogicalExpr)
				{
					// there are more than one relational expressions in the search
					// expression, remove the last one
					FilterExpr = ((BinaryExpr) existing).Left;
					((BinaryExpr) existing).Left = null;
				}
				else if (existing is RelationalExpr || existing is InExpr ||
						 existing is ParenthesizedExpr)
				{
					// this is only one relational expression, remove it
                    FilterExpr = null;
				}
			}
		}

		/// <summary>
		/// Create a new DataViewModel by cloning this DataViewModel
		/// </summary>
		/// <returns>A cloned DataViewModel</returns>
		public DataViewModel Clone()
		{
			// use Marshal and Unmarshal to clone a DataViewModel
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("DataViews");
			doc.AppendChild(root);
			XmlElement child = doc.CreateElement(ElementFactory.ConvertTypeToString(this.ElementType));
			this.Marshal(child);
			root.AppendChild(child);

			// create a new DataViewModel and unmarshal from the xml element as source
			DataViewModel newDataView = new DataViewModel(child);
            newDataView.SchemaModel = this.SchemaModel;
            newDataView.SchemaInfo = this.SchemaInfo;

			return newDataView;
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitDataView(this);
			
			_baseClass.Accept(visitor);

			_referencedClasses.Accept(visitor);

			_filter.Accept(visitor);

			_resultAttributes.Accept(visitor);

			_sortBy.Accept(visitor);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// the first child is for base class spec
			_baseClass = (DataClass) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);

			// then a collection of  referenced classes
			_referencedClasses = (ReferencedClassCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);

			// then comes filter
			_filter = (Filter) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);

			// then comes a collection of result attributes
			_resultAttributes = (ResultAttributeCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[3]);

			// then may comes SortBy
			if (parent.ChildNodes.Count > 4)
			{
				_sortBy = (SortBy) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[4]);
			}
			else
			{
				_sortBy = new SortBy();
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _baseClass
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_baseClass.ElementType));
			_baseClass.Marshal(child);
			parent.AppendChild(child);

			// write the _referencedClasses
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_referencedClasses.ElementType));
			_referencedClasses.Marshal(child);
			parent.AppendChild(child);

			// write the _filter
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_filter.ElementType));
			_filter.Marshal(child);
			parent.AppendChild(child);

			// write the _resultAttributes
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_resultAttributes.ElementType));
			_resultAttributes.Marshal(child);
			parent.AppendChild(child);

			// write the _sortBy
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_sortBy.ElementType));
			_sortBy.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Find the class element in the data view given a class alias
		/// </summary>
		/// <param name="alias">The class alias</param>
		/// <returns>A DataClass element</returns>
		public DataClass FindClass(string alias)
		{
			DataClass found = null;

            if (!string.IsNullOrEmpty(alias))
            {
                if (_baseClass.Alias == alias)
                {
                    found = _baseClass;
                }
                else
                {
                    foreach (DataClass dataClass in _referencedClasses)
                    {
                        if (dataClass.Alias == alias)
                        {
                            found = dataClass;
                            break;
                        }
                    }
                }
            }

			return found;
		}

		/// <summary>
		/// Get information indicating whether a class is the base class or
		/// one of the referenced classes in the dataview.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>true if the class is referenced by the data view, false otherwise.</returns>
		public bool IsClassReferenced(string className)
		{
			bool status = false;

			if (_baseClass.Name == className)
			{
				status = true;
			}
			else
			{
				foreach (DataClass dataClass in _referencedClasses)
				{
					if (dataClass.Name == className)
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

        /// <summary>
        /// Get information indicating whether an attribute is used in search expression or
        /// result list.
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>true if the attribute is referenced by the data view, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;

            // check in the result attributes first
            foreach (IDataViewElement result in this._resultAttributes)
            {
                if (result.Name == attributeName)
                {
                    string ownerName = "";
                    if (result.ElementType == ElementType.SimpleAttribute)
                    {
                        SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)result.GetSchemaModelElement();
                        if (simpleAttribute != null)
                        {
                            ownerName = simpleAttribute.OwnerClass.Name;
                        }
                    }
                    else if (result.ElementType == ElementType.ArrayAttribute)
                    {
                        ArrayAttributeElement arrayAttribute = (ArrayAttributeElement)result.GetSchemaModelElement();
                        if (arrayAttribute != null)
                        {
                            ownerName = arrayAttribute.OwnerClass.Name;
                        }
                    }
                    else if (result.ElementType == ElementType.VirtualAttribute)
                    {
                        VirtualAttributeElement virtualAttribute = (VirtualAttributeElement)result.GetSchemaModelElement();
                        if (virtualAttribute != null)
                        {
                            ownerName = virtualAttribute.OwnerClass.Name;
                        }
                    }
                    else if (result.ElementType == ElementType.ImageAttribute)
                    {
                        ImageAttributeElement imageAttribute = (ImageAttributeElement)result.GetSchemaModelElement();
                        if (imageAttribute != null)
                        {
                            ownerName = imageAttribute.OwnerClass.Name;
                        }
                    }
                    else if (result.ElementType == ElementType.RelationshipAttribute)
                    {
                        RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)result.GetSchemaModelElement();
                        if (relationshipAttribute != null)
                        {
                            ownerName = relationshipAttribute.OwnerClass.Name;
                        }
                    }

                    if (ownerName == className)
                    {
                        status = true;
                        break;
                    }
                }
            }

            if (!status)
            {
                // check in the search expression
                FindSearchAttributeVisitor visitor = new FindSearchAttributeVisitor(className, attributeName);

                _filter.Accept(visitor);

                if (visitor.IsFound)
                {
                    status = true;
                }
            }

            if (!status)
            {
                // checking the referenced class for relationships
                foreach (DataClass refClass in _referencedClasses)
                {
                    if (refClass.ReferringRelationshipName == attributeName)
                    {
                        DataClass referringClass = this.FindClass(refClass.ReferringClassAlias);
                        if (referringClass != null)
                        {
                            ClassElement schemaModelElement = referringClass.GetSchemaModelElement() as ClassElement;
                            if (schemaModelElement != null &&
                                schemaModelElement.Name == className)
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether an attribute is part of the class' unique keys
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <param name="classElement">The class element</param>
        /// <returns>true if the attribute is part of class' unique keys, false otherwise.</returns>
        private bool IsUniqueKey(DataViewElementBase attribute, ClassElement classElement)
        {
            bool status = false;

            foreach (AttributeElementBase uk in classElement.UniqueKeys)
            {
                if (uk.Name == attribute.Name)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/" + this.Name;
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		[BrowsableAttribute(false)]		
		public IXaclObject Parent
		{
			get
			{
				return _container;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
		public IEnumerator GetChildren()
		{
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

		#endregion
	}
}