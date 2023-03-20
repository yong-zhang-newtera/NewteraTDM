/*
* @(#)HierarchyGenerationUtil.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.Data;
    using System.Xml;
	using System.ComponentModel;
	using System.Globalization;
    using System.Collections.Specialized;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.DataView.Taxonomy;
    using Newtera.WinClientCommon;

	/// <summary>
	/// Provide untilities for generating classifying hierarchy
	/// </summary>
	/// <version>  1.0.1 22 June 2008</version>
	public class HierarchyGenerationUtil
	{
        public const int MaxChildNumber = 500;
        public const int MaxInstanceNumber = 5000;

        private CMDataServiceStub _dataService;
        private AutoClassifyDef _autoClassifyDef;
        private bool _isCancelled;
        private DataViewModel _dataView;
        private int _pageSize = 100;

        /// <summary>
        /// Initializes a new instance of the HierarchyGenerationUtil class.
		/// </summary>
        public HierarchyGenerationUtil(AutoClassifyDef autoClassifyDef, 
            CMDataServiceStub dataService, DataViewModel dataView)
		{
            _autoClassifyDef = autoClassifyDef;
            _dataService = dataService;
            _dataView = dataView;
            AddSearchFields(_dataView);
            _isCancelled = false;
		}

        /// <summary>
        /// Gets or sets the information indicating whether the operation has been cancelled
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                return _isCancelled;
            }
            set
            {
                _isCancelled = value;
            }
        }

        /// <summary>
        /// Create an unique name for a child taxon node in a taxonomy tree.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>A string representing an unique name.</returns>
        internal string CreateUniqueTaxonName(ITaxonomy parentNode)
        {
            string nodeName = "Node";

            nodeName = ((IDataViewElement)parentNode).Name + (parentNode.ChildrenNodes.Count + 1);

            return nodeName;
        }

        /// <summary>
        /// Create a search filter for a classifying node.
        /// </summary>
        /// <param name="autoLevel">The classifying defintion for a level</param>
        /// <returns>A string representing an unique name.</returns>
        internal IDataViewElement CreateSearchFilter(AutoClassifyLevel autoLevel, string nodeValue)
        {
            IDataViewElement expr = null;
            DataSimpleAttribute left;
            Parameter right;

            SimpleAttributeElement simpleAttribute = GetSimpleAttributeElement(autoLevel.OwnerClassAlias, autoLevel.ClassifyingAttribute);

            if (simpleAttribute != null)
            {
                // add as a search attribute
                left = new DataSimpleAttribute(autoLevel.ClassifyingAttribute, autoLevel.OwnerClassAlias);
                left.Caption = simpleAttribute.Caption;
                right = new Parameter(autoLevel.ClassifyingAttribute, autoLevel.OwnerClassAlias, simpleAttribute.DataType);
                right.ParameterValue = nodeValue;

                expr = new RelationalExpr(ElementType.Equals, left, right);
            }
            else
            {
                throw new Exception("Unable to find the simple attribute with name " + autoLevel.ClassifyingAttribute + " in a class with alias " + autoLevel.OwnerClassAlias);
            }

            return expr;
        }

        /// <summary>
        /// Get the distinct values representing the nodes for the given level
        /// </summary>
        /// <param name="currentAutoLevelDef">The current auto level</param>
        /// <param name="levelIndex"></param>
        /// <returns>A collection of distinct values</returns>
        internal StringCollection GetDistinctLevelValue(AutoClassifyLevel currentAutoLevelDef, int childLevelIndex)
        {
            StringCollection distinctNodeValues;

            // get the data view that contains search values that restrict search results according
            // to the node values of current parent nodes alone the path
            DataViewModel dataView = GetDataView(childLevelIndex);
            string query = dataView.GetAttributeValueQuery(currentAutoLevelDef.ClassifyingAttribute, currentAutoLevelDef.OwnerClassAlias);

            // get total count of return results
            int totalCount = _dataService.ExecuteCount(ConnectionStringBuilder.Instance.Create(dataView.SchemaModel.SchemaInfo), query);

            // get the result in page mode
            string queryId = _dataService.BeginQuery(ConnectionStringBuilder.Instance.Create(dataView.SchemaModel.SchemaInfo), query, _pageSize);

            try
            {
                DataSet masterDataSet = null;

                DataSet slaveDataSet;
                int currentPageIndex = 0;
                int instanceCount = 0;
                int start, end;
                XmlReader xmlReader;
                XmlNode xmlNode;
                while (instanceCount < totalCount && !_isCancelled)
                {
                    start = currentPageIndex * _pageSize + 1;
                    end = start + this._pageSize - 1;
                    if (end > totalCount)
                    {
                        end = totalCount;
                    }

                    // invoke the web service synchronously to get data in pages
                    xmlNode = _dataService.GetNextResult(ConnectionStringBuilder.Instance.Create(dataView.SchemaModel.SchemaInfo),
                        queryId);

                    if (xmlNode == null)
                    {
                        // end of result
                        break;
                    }

                    slaveDataSet = new DataSet();

                    xmlReader = new XmlNodeReader(xmlNode);
                    slaveDataSet.ReadXml(xmlReader);

                    instanceCount += slaveDataSet.Tables[dataView.BaseClass.Name].Rows.Count;

                    if (masterDataSet == null)
                    {
                        // first page
                        masterDataSet = slaveDataSet;
                        masterDataSet.EnforceConstraints = false;
                    }
                    else
                    {
                        // append to the master dataset
                        AppendDataSet(dataView.BaseClass.Name, masterDataSet, slaveDataSet);
                    }

                    currentPageIndex++;
                }

                if (!this._isCancelled &&
                    masterDataSet != null &&
                    !DataSetHelper.IsEmptyDataSet(masterDataSet, dataView.BaseClass.ClassName))
                {
                    distinctNodeValues = GetDistinctValues(masterDataSet, currentAutoLevelDef.ClassifyingAttribute);
                }
                else
                {
                    distinctNodeValues = new StringCollection();
                }

                return distinctNodeValues;
            }
            finally
            {
                if (queryId != null)
                {
                    _dataService.EndQuery(queryId);
                }
            }
        }

        /// <summary>
        /// Gets distinct values of a classifying attributes
        /// </summary>
        /// <param name="ds"></param>
        /// <returns>A StringCollection object</returns>
        private StringCollection GetDistinctValues(DataSet ds, string attributeName)
        {
            StringCollection distinctValues = new StringCollection();

            if (ds.Tables[_dataView.BaseClass.Name].Columns[attributeName] != null)
            {
                foreach (DataRow dataRow in ds.Tables[_dataView.BaseClass.Name].Rows)
                {
                    if (!dataRow.IsNull(attributeName))
                    {
                        string attributeVal = dataRow[attributeName].ToString();
                        if (!string.IsNullOrEmpty(attributeVal) &&
                            !distinctValues.Contains(attributeVal))
                        {
                            distinctValues.Add(attributeVal);
                        }
                    }
                }
            }

            return distinctValues;
        }

        /// <summary>
        /// Append a slave DataSet to a master dataset
        /// </summary>
        /// <param name="master">The master DataSet</param>
        /// <param name="slave">The slave DataSet</param>
        private void AppendDataSet(string tableName, DataSet master, DataSet slave)
        {
            DataTable masterTable = master.Tables[tableName];
            DataTable slaveTable = slave.Tables[tableName];

            foreach (DataRow row in slaveTable.Rows)
            {
                masterTable.ImportRow(row);
            }
        }

        /// <summary>
        /// Gets the data view that contains search values that restrict search results according
        /// to the node values of current parent nodes alone the path.
        /// </summary>
        /// <param name="levelIndex">level index, starting from 0.</param>
        /// <returns>The data view model</returns>
        private DataViewModel GetDataView(int levelIndex)
        {
            // Update the search values of the classifying attributes to the data view
            int index = 0;
            foreach (AutoClassifyLevel autoLevel in _autoClassifyDef.AutoClassifyLevels)
            {
                if (index < levelIndex)
                {
                    // include the parent node values in the search
                    _dataView.SetSearchValue(autoLevel.OwnerClassAlias, autoLevel.ClassifyingAttribute, autoLevel.NodeValue);
                }
                else
                {
                    _dataView.SetSearchValue(autoLevel.OwnerClassAlias, autoLevel.ClassifyingAttribute, null);
                }

                index++;
            }

            return _dataView;
        }

        /// <summary>
        /// Gets the information indicating whether a given attribute is part of search expression
        /// of the data view
        /// </summary>
        /// <param name="classAlias">Alias of owner class</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>true if the search expression exist, false otherwise</returns>
        private bool IsSearchAttributeExist(string classAlias, string attributeName)
        {
            bool status = false;

            FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(classAlias, attributeName);

            if (_dataView.FilterExpr != null)
            {
                _dataView.FilterExpr.Accept(visitor);
                // TODO, also make sure that the operator of the binary expression for search attribute
                // is "="
                if (visitor.SearchParameter != null &&
                    !visitor.SearchParameter.HasValue)
                {
                    status = true;
                }
            }

            return status;
        }

        private void AddSearchFields(DataViewModel dataView)
        {
            // make sure that the classifying attributes of all levels have been part
            // of search expression of the data view
            foreach (AutoClassifyLevel autoLevel in _autoClassifyDef.AutoClassifyLevels)
            {
                if (!IsSearchAttributeExist(autoLevel.OwnerClassAlias, autoLevel.ClassifyingAttribute))
                {
                    IDataViewElement expr, left, right;

                    SimpleAttributeElement simpleAttribute = GetSimpleAttributeElement(autoLevel.OwnerClassAlias, autoLevel.ClassifyingAttribute);

                    if (simpleAttribute != null)
                    {
                        // add as a search attribute
                        left = new DataSimpleAttribute(autoLevel.ClassifyingAttribute, autoLevel.OwnerClassAlias);
                        right = new Parameter(autoLevel.ClassifyingAttribute, autoLevel.OwnerClassAlias, simpleAttribute.DataType);

                        expr = new RelationalExpr(ElementType.Equals, left, right);

                        _dataView.AddSearchExpr(expr, ElementType.And);
                    }
                    else
                    {
                        throw new Exception("Unable to find the simple attribute with name " + autoLevel.ClassifyingAttribute + " in a class with alias " + autoLevel.OwnerClassAlias);
                    }
                }
            }
        }

        /// <summary>
        /// Get the SimpleAttributeElement object corresponds to the result attribute in the data view
        /// </summary>
        /// <param name="classAlias"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private SimpleAttributeElement GetSimpleAttributeElement(string classAlias, string attributeName)
        {
            SimpleAttributeElement simpleAttribute = null;

            /*
            DataClass dataClass = _dataView.FindClass(classAlias);
            if (dataClass != null)
            {
                ClassElement classElement = (ClassElement)dataClass.GetSchemaModelElement();
                simpleAttribute = classElement.FindInheritedSimpleAttribute(attributeName);
            }
             */
            DataSimpleAttribute simpleResult;
            foreach (IDataViewElement result in _dataView.ResultAttributes)
            {
                if (result is DataSimpleAttribute)
                {
                    simpleResult = (DataSimpleAttribute)result;

                    if (simpleResult.OwnerClassAlias == classAlias &&
                        simpleResult.Name == attributeName)
                    {
                        simpleAttribute = (SimpleAttributeElement)simpleResult.GetSchemaModelElement();
                    }
                }
            }

            return simpleAttribute;
        }
	}
}