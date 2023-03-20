/*
* @(#)Spider.cs
*
* Copyright (c) 2003-2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.Xml;
	using System.Text;
    using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Data;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.DB.MetaData;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;

	/// <summary> 
	/// A spider that travels data instances that are related to collect the keywords to be full-text indexed.
    /// It first finds the classes in all schemas that have an attributes to store the keywords as CLob, and then
    /// retrieves the data instances from the bottom classses, collect the keywords from the instances, follows the
    /// relationships of the instances to collect keywords from the related instances until reaching the ends.
	/// <version>  	1.0.0 14 Apr 2104 </version>
    public class Spider
    {
        private int _depth;

        /// <summary>
        /// Initiate an instance of Spider class
        /// </summary>
        public Spider(int depth)
        {
            _depth = depth;
        }

        /// <summary>
        /// Start crawling to collect index values from the data instances in the base class and its related classes, and save the clob of the indexing attribute.
        /// </summary>
        /// <param name="metaData">The meta data model for the schema</param>
        /// <param name="classElement">The class element represents the class for the full text index</param>
        /// <param name="fullTextAttributeName">The attribute for keeping the ful-text</param>
        /// <param name="lastIndexedTime">The last time that index was built</param>
        /// <returns>True if the index need to be rebuilt, False, otherwise.</returns>
        /// <remarks>This is a Spider specific method</remarks>
        public bool StartCrawling(MetaDataModel metaData, ClassElement baseClassElement, string fullTextAttributeName)
        {
            return StartCrawling(metaData, baseClassElement,fullTextAttributeName, DateTime.MinValue, false);
        }

        /// <summary>
        /// Start crawling to collect index values from the data instances in the base class and its related classes, and save the clob of the indexing attribute.
        /// </summary>
        /// <param name="metaData">The meta data model for the schema</param>
        /// <param name="classElement">The class element represents the class for the full text index</param>
        /// <param name="fullTextAttributeName">The attribute for keeping the ful-text</param>
        /// <param name="lastIndexedTime">The last time that index was built</param>
        /// <param name="isIncremental">Indicate if building the index in an incremental mode</param>
        /// <returns>True if the index need to be rebuilt, False, otherwise.</returns>
        /// <remarks>This is a Spider specific method</remarks>
        public bool StartCrawling(MetaDataModel metaData, ClassElement baseClassElement, string fullTextAttributeName, DateTime lastIndexedTime, bool isIncremental)
        {
            bool needRebuild = false;
            DataViewModel dataView;
            string query;
            XmlReader xmlReader;
            DataSet ds;
            InstanceView instanceView;
            StringBuilder builder;
            int rows;
            Hashtable visitedInstancesTable;

            // First walk through all leaf classes of the given class, collect the values of
            // those simple attributes whose IsGoodForFullTextSearch is set to true,
            SchemaModelElementCollection leafClasses = baseClassElement.GetLeafClasses();
            XmlDocument doc;
            QueryReader reader = null;
            int currentDepth = 0;
            foreach (ClassElement leafClass in leafClasses)
            {
                dataView = metaData.GetCompleteDataView(leafClass.Name); // Complete DataView includes the keyword attributes

                // execute the query to get the xml result
                query = dataView.SearchQuery;

                Interpreter interpreter = new Interpreter();
                // get result in pages
                interpreter.IsPaging = true;
                interpreter.PageSize = 500;
                reader = interpreter.GetQueryReader(query);
                try
                {

                    doc = reader.GetNextPage(); // get the first page
                    while (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                    {
                        // convert xml data into instance view
                        xmlReader = new XmlNodeReader(doc);
                        ds = new DataSet();
                        ds.ReadXml(xmlReader);
                        instanceView = new InstanceView(dataView, ds);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            // iterate through each instance and update the full-text value
                            rows = ds.Tables[leafClass.Name].Rows.Count;

                            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
                            {
                                instanceView.SelectedIndex = rowIndex;

                                // if isIncremental is true, build keyword content only for the newly created data instances for performance sake
                                if (!isIncremental || !IsKeywordsExist(instanceView, fullTextAttributeName))
                                {
                                    builder = new StringBuilder();

                                    visitedInstancesTable = new Hashtable(); // keep the visited instance ids to prevent travel loop

                                    visitedInstancesTable[instanceView.InstanceData.ObjId] = "1";

                                    TravelInstancePath(metaData, instanceView, builder, visitedInstancesTable, currentDepth); // travel the relationships of the instance to collect full-text searchable values

                                    bool status = UpdateInstanceContent(instanceView, builder,
                                        (InstanceAttributePropertyDescriptor)instanceView.GetProperties(null)[fullTextAttributeName]);

                                    if (!needRebuild && status)
                                    {
                                        // some full-text values have been changed, need to rebuild the index
                                        needRebuild = true;
                                    }
                                }
                            }
                        }

                        doc = reader.GetNextPage();                    
                    }
                }
                finally
                {
                    // close the database connection
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
            }

            return needRebuild;
        }

        private bool IsKeywordsExist(InstanceView instanceView, string fullTextAttributeName)
        {
            bool status = false;

            string keywords = instanceView.InstanceData.GetAttributeStringValue(fullTextAttributeName);
            if (!string.IsNullOrEmpty(keywords))
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// travel the relationships of the instance to collect full-text searchable values
        /// </summary>
        /// <param name="instanceView">A instance view</param>
        /// <param name="builder">Holds the collected values</param>
        private void TravelInstancePath(MetaDataModel metaData, InstanceView instanceView, StringBuilder builder, Hashtable visitedInstancesTable, int currentDepth)
        {
            // collect values from the base instance
            CollectInstanceValues(instanceView, builder);

            if (currentDepth < _depth)
            {
                currentDepth++;
                foreach (DataClass relatedClass in instanceView.DataView.BaseClass.RelatedClasses)
                {
                    if (relatedClass.ReferringRelationship.IsUsedForFullTextIndex)
                    {
                        if (relatedClass.IsLeafClass)
                        {
                            TravelRelatedClass(metaData, relatedClass, instanceView, builder, visitedInstancesTable, currentDepth);
                        }
                        else
                        {
                            // travel to the leaf classes of each related class
                            ReferencedClassCollection relatedLeafClasses = GetLeafClasses(metaData, relatedClass, instanceView);
                            foreach (DataClass relatedLeafClass in relatedLeafClasses)
                            {
                                TravelRelatedClass(metaData, relatedLeafClass, instanceView, builder, visitedInstancesTable, currentDepth);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Travel to a related class to collect instance values
        /// </summary>
        /// <param name="relatedClass"></param>
        /// <param name="instanceView"></param>
        /// <param name="builder"></param>
        private void TravelRelatedClass(MetaDataModel metaData, DataClass relatedClass, InstanceView masterInstanceView, StringBuilder builder, Hashtable visitedInstancesTable, int currentDepth)
        {
            DataViewModel relatedDataView = GetDataViewForRelatedClass(metaData, relatedClass, masterInstanceView);

            if (relatedDataView != null)
            {
                // execute the query to get the xml result
                string query = relatedDataView.SearchQuery;

                Interpreter interpreter = new Interpreter();
                // get result in pages
                interpreter.IsPaging = true;
                interpreter.PageSize = 500;

                QueryReader reader = interpreter.GetQueryReader(query);
                XmlDocument doc;
                XmlReader xmlReader;
                DataSet ds;
                InstanceView instanceView;
                int rows;
                try
                {
                    doc = reader.GetNextPage(); // get the first page
                    while (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                    {
                        // convert xml data into instance view
                        xmlReader = new XmlNodeReader(doc);
                        ds = new DataSet();
                        ds.ReadXml(xmlReader);
                        instanceView = new InstanceView(relatedDataView, ds);

                        if (!DataSetHelper.IsEmptyDataSet(ds, relatedDataView.BaseClass.ClassName))
                        {
                            // iterate through each instance and update the full-text value
                            rows = ds.Tables[relatedClass.ClassName].Rows.Count;
                            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
                            {
                                instanceView.SelectedIndex = rowIndex;

                                // check if the instance has been visited by the spider in case of looping relationship
                                if (visitedInstancesTable[instanceView.InstanceData.ObjId] == null)
                                {
                                    visitedInstancesTable[instanceView.InstanceData.ObjId] = "1"; // remember the instance
                                    TravelInstancePath(metaData, instanceView, builder, visitedInstancesTable, currentDepth); // travel the relationships of the instance to collect full-text searchable values
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                        doc = reader.GetNextPage();
                    }
                }
                finally
                {
                    // close the database connection
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Get the data view to build a query
        /// </summary>
        /// <param name="connection">A CMConnection</param>
        /// <returns>A DataViewModel</returns>
        private DataViewModel GetDataViewForRelatedClass(MetaDataModel metaData, DataClass relatedClass, InstanceView masterInstanceView)
        {
            // get the complete data view of the class
             DataViewModel dataView = metaData.GetDetailedDataView(relatedClass.ClassName);

            // build a search expression that retrieve the data instances of the related class that
            // are asspciated with the selected data instance
            string searchValue = null;
            if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
            {
                // many-to-one relationship, gets the obj_id of the referenced instance from the table
                DataTable relationshipTable = masterInstanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(masterInstanceView.DataView.BaseClass.ClassName, relatedClass.ReferringRelationship.Name)];
                if (relationshipTable != null)
                {
                    if (relationshipTable.Rows[masterInstanceView.SelectedIndex].IsNull(NewteraNameSpace.OBJ_ID) == false)
                    {
                        searchValue = relationshipTable.Rows[masterInstanceView.SelectedIndex][NewteraNameSpace.OBJ_ID].ToString();
                    }

                    if (string.IsNullOrEmpty(searchValue))
                    {
                        return null;  // related instance doesn't exist
                    }
                }
            }
            else
            {
                // one-to-many relationship
                searchValue = masterInstanceView.InstanceData.ObjId;
            }

            SearchExpressionBuilder builder = new SearchExpressionBuilder();
            IDataViewElement expr = builder.BuildSearchExprForRelationship(dataView, relatedClass, searchValue);

            // add search expression to the dataview
            dataView.AddSearchExpr(expr, ElementType.And);

            return dataView;
        }

        /// <summary>
        /// Collect full-text indexable values for an instance
        /// </summary>
        /// <param name="instanceView">A instance view</param>
        /// <param name="builder">Holds the collected values</param>
        private void CollectInstanceValues(InstanceView instanceView, StringBuilder builder)
        {
            // concate values of selected instance propertties together
            foreach (InstanceAttributePropertyDescriptor property in instanceView.GetProperties(null))
            {
                if (property.IsBrowsable &&
                    property.IsGoodForFullTextSearch &&
                    property.GetValue() != null)
                {
                    object propertyValue = property.GetValue();

                    if (property.IsArray)
                    {
                        DataTable arrayDataTable = ((ArrayDataTableView)propertyValue).ArrayAttributeValue;

                        string keyword;
                        foreach (DataColumn dataColumn in arrayDataTable.Columns)
                        {
                            for (int row = 0; row < arrayDataTable.Rows.Count; row++)
                            {
                                if (!arrayDataTable.Rows[row].IsNull(dataColumn))
                                {
                                    if (!string.IsNullOrEmpty(property.KeywordFormat))
                                    {
                                        keyword = property.KeywordFormat.Replace("{Column}", dataColumn.ColumnName);
                                        keyword = keyword.Replace("{Value}", arrayDataTable.Rows[row][dataColumn].ToString());
                                    }
                                    else
                                    {
                                        keyword = arrayDataTable.Rows[row][dataColumn].ToString();
                                    }

                                    builder.Append(keyword).Append(" ");
                                }
                            }
                        }
                    }
                    else if (property.IsMultipleChoice)
                    {
                        object[] vEnums = (object[])propertyValue;
                        for (int i = 0; i < vEnums.Length; i++)
                        {
                            builder.Append(vEnums[i].ToString()).Append(" ");
                        }
                    }
                    else
                    {
                        string keyword = property.GetValue().ToString();
                        if (!string.IsNullOrEmpty(property.KeywordFormat))
                        {
                            keyword = property.KeywordFormat.Replace("{Value}", keyword);
                        }

                        builder.Append(keyword).Append(" ");
                    }
                }
            }
        }


        /// <summary>
        /// Update value of the full-text attribute for the specified instance
        /// </summary>
        /// <param name="instanceView">A instance view</param>
        /// <param name="fullTextPropertyDecriptor">The full-text serach property descriptor</param>
        /// <returns>true if full-text values has been changed</returns>
        private bool UpdateInstanceContent(InstanceView instanceView, StringBuilder builder, InstanceAttributePropertyDescriptor fullTextPropertyDecriptor)
        {
            bool isValueChanged = false;
            string query;

            // set the new content
            string contentVal = builder.ToString();
            fullTextPropertyDecriptor.SetValue(null, contentVal);

            // do not update if the content does not change
            if (instanceView.IsDataChanged)
            {
                isValueChanged = true;

                query = instanceView.DataView.UpdateQuery;

                Interpreter interpreter = new Interpreter();

                try
                {
                    interpreter.Query(query);
                }
                catch (Exception)
                {
                    // do not stop if there is error
                }
            }

            return isValueChanged;
        }

        private ReferencedClassCollection GetLeafClasses(MetaDataModel metaData, DataClass relatedClass, InstanceView masterInstanceView)
        {
            ReferencedClassCollection relatedLeafClasses = new ReferencedClassCollection();
            DataClass relatedLeafClass;
            SchemaModelElementCollection leafClassElements;

            // Gets leaf classes that contains at least one instance
            leafClassElements = GetLeafClassElements(metaData, relatedClass, masterInstanceView);

            foreach (ClassElement leafClassElement in leafClassElements)
            {
                // create a DataClass using leaf class name
                relatedLeafClass = new DataClass(relatedClass.Name, leafClassElement.Name, DataClassType.ReferencedClass);
                relatedLeafClass.ReferringClassAlias = relatedClass.ReferringClassAlias;
                relatedLeafClass.ReferringRelationshipName = relatedClass.ReferringRelationshipName;
                relatedLeafClass.Caption = leafClassElement.Caption;
                relatedLeafClass.ReferringRelationship = relatedClass.ReferringRelationship;

                relatedLeafClasses.Add(relatedLeafClass);
            }

            return relatedLeafClasses;
        }

        /// <summary>
        /// Gets a collection of leaf classes that contains at least one search result instance
        /// </summary>
        /// <returns></returns>
        public SchemaModelElementCollection GetLeafClassElements(MetaDataModel metaData, DataClass relatedClass, InstanceView masterInstanceView)
        {
            SchemaModelElementCollection leafClassElements = new SchemaModelElementCollection();
            DataViewModel dataView;

            // get dataview model for the related data  query
            dataView = GetDataViewForRelatedClass(metaData, relatedClass, masterInstanceView);

            if (dataView != null)
            {
                string query = dataView.SearchQuery;

                Interpreter interpreter = new Interpreter();

                XmlDocument doc = interpreter.GetClassNames(query);

                // the result has been saved in interpreter, so we do not need to get them from
                // XmlDocument
                StringCollection leafClasses = interpreter.ClassNames;

                ClassElement classElement;
                foreach (string leafClassName in leafClasses)
                {
                    classElement = metaData.SchemaModel.FindClass(leafClassName);
                    leafClassElements.Add(classElement);
                }
            }

            return leafClassElements;
        }
    }
}