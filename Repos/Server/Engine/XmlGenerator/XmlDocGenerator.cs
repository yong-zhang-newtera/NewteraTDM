/*
* @(#)XmlDocGenerator.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.XmlGenerator
{
	using System;
	using System.Xml;
    using System.Data;
    using System.Text;
    using System.IO;
    using System.Threading;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Runtime.Remoting;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Mappings;
	using Newtera.Common.MetaData.XMLSchemaView;
    using Newtera.Common.MetaData.DataView;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Vdom;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;

	/// <summary>
	/// Generate a XmlDocument for the data instance(s) based on a given XMLSchemaModel object using the mappings defined for the classes involved
	/// </summary>
	/// <version>  	1.0.0 27 Aug 2014 </version>
	public class XmlDocGenerator
	{
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";
        private const string TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE = "fileBaseDir";
        private const int MAX_SORT_NUMBER = 1000;
        private const int MAX_PAGE_SIZE = 2000;
        private const string TIME_SERIES_FILE = "timeseries.csv";
        private const string TIME_SERIES_LOG_FILE = "log.txt";

        private XMLSchemaModel _xmlSchemaModel;
        private MetaDataModel _metaData;
        private string _connectionStr;
        private InstanceView _baseInstanceView;
        private int _baseInstanceCount;
        private string _fileBaseDirPath;
        private XmlDocument _baseDoc;
        private int _pageIndex;
        private Hashtable _pagingInfoTable;
        private XMLSchemaElement _xAxisElement;
        private XMLSchemaElement _categoryAxisElement;
        private Hashtable _uniqueKeys;

        /// <summary>
        /// Initiating an instance of XmlDocGenerator
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="connectionStr">The connection string for the db</param>
        /// <param name="xmlSchemaModel">The xml schema model</param>
        /// <param name="baseInstanceView">The root instance data view</param>
        /// <param name="baseInstanceCount">The root instance count</param>
        public XmlDocGenerator(MetaDataModel metaData, string connectionStr, XMLSchemaModel xmlSchemaModel, InstanceView baseInstanceView, int baseInstanceCount)
		{
            _metaData = metaData;
            _connectionStr = connectionStr;
            _xmlSchemaModel = xmlSchemaModel;
            _baseInstanceView = baseInstanceView;
            _baseInstanceCount = baseInstanceCount;
            _baseDoc = null;
            _pageIndex = 0;
            _pagingInfoTable = new Hashtable();
            _xAxisElement = _xmlSchemaModel.GetXAxisElement();
            _categoryAxisElement = _xmlSchemaModel.GetCategoryAxisElement();
            _uniqueKeys = new Hashtable();
        }

        /// <summary>
        /// Create a xml document
        /// </summary>
        /// <returns>The created xml document</returns>
        public XmlDocument Create()
		{
            XmlDocument doc = new XmlDocument();

            // create a hierarchical strcuture
            // create root element
            if (_xmlSchemaModel.RootElement.MaxOccurs != "unbounded")
            {
                // HACK, get the directory where the import files reside
                _fileBaseDirPath = GetUserFilePath(_baseInstanceView, TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE);

                XmlElement root = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.RootElement.Caption));

                AddChildElements(doc, root, _baseInstanceView, (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[_xmlSchemaModel.RootElement.Caption], false);

                SortChildElement();

                doc.AppendChild(root);
            }
            else
            {
                // root element repeats, create a pseudo root element
                XmlElement pseudoRoot = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.Caption));

                doc.AppendChild(pseudoRoot);

                for (int i = 0; i < _baseInstanceCount; i++)
                {
                    _baseInstanceView.SelectedIndex = i;

                    // HACK, get the directory where the import files reside
                    _fileBaseDirPath = GetUserFilePath(_baseInstanceView, TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE);

                    XmlElement root = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.RootElement.Caption));

                    AddChildElements(doc, root, _baseInstanceView, (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[_xmlSchemaModel.RootElement.Caption], false);

                    pseudoRoot.AppendChild(root);
                }

                SortChildElement();
            }

            return doc;
		}

        /// <summary>
        /// Create the first Xml Document in paging mode.
        /// </summary>
        /// <returns>An Xml Documents</returns>
        public XmlDocument BeginCreateDoc()
        {
            XmlDocument doc = new XmlDocument();

            // create root element
            if (_xmlSchemaModel.RootElement.MaxOccurs != "unbounded")
            {
                // HACK, get the directory where the import files reside
                _fileBaseDirPath = GetUserFilePath(_baseInstanceView, TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE);

                // create root element
                XmlElement root = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.RootElement.Caption));

                // Create xml document in paging mode
                AddChildElements(doc, root, _baseInstanceView, (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[_xmlSchemaModel.RootElement.Caption], true);

                // child elements were added to the root element
                doc.AppendChild(root);
            }
            else
            {
                // root element repeats, create a pseudo root element
                XmlElement pseudoRoot = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.Caption));

                doc.AppendChild(pseudoRoot);

                for (int i = 0; i < _baseInstanceCount; i++)
                {
                    _baseInstanceView.SelectedIndex = i;

                    // update the fileBaseDirPath using the value of the virtual attribute of the current instance
                    _fileBaseDirPath = GetUserFilePath(_baseInstanceView, TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE);

                    XmlElement root = doc.CreateElement(XmlConvert.EncodeName(_xmlSchemaModel.RootElement.Caption));

                    AddChildElements(doc, root, _baseInstanceView, (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[_xmlSchemaModel.RootElement.Caption], true);

                    pseudoRoot.AppendChild(root);
                }
            }

            // keep the first doc as the base doc
            _baseDoc = doc;

            return doc;
        }

        /// <summary>
        /// Create a next Xml Document in paging mode.
        /// </summary>
        /// <returns>An Xml Documents or null if the end is reached</returns>
        public XmlDocument CreateNextDoc()
        {
            if (_pagingInfoTable.Keys.Count > 0)
            {
                XmlElement parentElement = null;

                foreach (XmlElement key in _pagingInfoTable.Keys)
                {
                    parentElement = key;
                    break;
                }

                // clear child nodes first
                parentElement.RemoveAll();

                List<XmlElement> childElements = (List<XmlElement>)_pagingInfoTable[parentElement];

                int count = 0;
                foreach (XmlElement childElement in childElements)
                {
                    parentElement.AppendChild(childElement);

                    count++;
                    if (count >= MAX_PAGE_SIZE)
                    {
                        break;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    // remove xml child element from the list
                    childElements.RemoveAt(0);
                }

                if (childElements.Count == 0)
                {
                    // Reaching the end of child element list, remove it from hash table
                    _pagingInfoTable[parentElement] = null;
                    _pagingInfoTable.Remove(parentElement);
                }

                _pageIndex++;

                return _baseDoc;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Call this method with BeginCreateDoc method to release the resource
        /// </summary>
        /// <returns>A list of the Xml Documents</returns>
        public void EndCreateDoc()
        {
            _baseDoc = null;
        }

        private void AddChildElements(XmlDocument doc, XmlElement parentElement, InstanceView instanceView, XMLSchemaComplexType complexType, bool isPaging)
        {
            DataViewModel relatedDataView;
            XMLSchemaComplexType childComplexType;
            XmlElement childElement;

            foreach (XMLSchemaElement schemaElement in complexType.Elements)
            {
                if (!IsComplexType(schemaElement))
                {
                    childElement = doc.CreateElement(XmlConvert.EncodeName(schemaElement.Caption));
                    childElement.InnerText = instanceView.InstanceData.GetAttributeStringValue(schemaElement.Name); // the schema element name is a name of instance attributes
                    parentElement.AppendChild(childElement);
                }
                else
                {
                    childComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[schemaElement.Caption];

                    // instanceView represents a master instance, childComplexType.Name represents a related class name
                    relatedDataView = _metaData.GetRelatedDetailedDataView(instanceView, childComplexType.Name);

                    if (relatedDataView != null)
                    {
                        switch (schemaElement.DataSourceOption)
                        {
                            case XMLElementDataSourceOption.Database:

                                relatedDataView = AddSortCriretia(relatedDataView, childComplexType);

                                // create the xml child elements using database data
                                AddChildElementsFromDatabase(doc, parentElement, schemaElement, relatedDataView, childComplexType, isPaging);
                                break;

                            case XMLElementDataSourceOption.File:

                                if (schemaElement.ImportScriptNames != null &&
                                    schemaElement.ImportScriptNames.Count > 0)
                                {
                                    // create the xml child elements using imported data
                                    AddChildElementsFromImports(doc, parentElement, schemaElement, relatedDataView, childComplexType, isPaging);
                                }

                                break;

                            case XMLElementDataSourceOption.DatabaseAndFile:

                                relatedDataView = AddSortCriretia(relatedDataView, childComplexType);

                                // create the xml child elements using database data first
                                AddChildElementsFromDatabase(doc, parentElement, schemaElement, relatedDataView, childComplexType, isPaging);

                                // create the xml child elements using imported data secondly
                                if (schemaElement.ImportScriptNames != null &&
                                    schemaElement.ImportScriptNames.Count > 0)
                                {
                                    // create the xml child elements using imported data
                                    AddChildElementsFromImports(doc, parentElement, schemaElement, relatedDataView, childComplexType, isPaging);
                                }

                                break;
                        }
                    }
                }
            }
        }

        private void MergeChildElements(XmlDocument doc, XmlElement parentElement, InstanceView instanceView, XMLSchemaComplexType complexType)
        {
            XmlElement childElement;

            foreach (XMLSchemaElement schemaElement in complexType.Elements)
            {
                // merging values for simple child elements
                if (!IsComplexType(schemaElement))
                {
                    childElement = parentElement[XmlConvert.EncodeName(schemaElement.Caption)];
                    if (childElement != null && string.IsNullOrEmpty(childElement.InnerText))
                    {
                        childElement.InnerText = instanceView.InstanceData.GetAttributeStringValue(schemaElement.Name); // the schema element name is a name of instance attributes
                    }
                }
            }
        }

        private void SortChildElement()
        {
            // get a parent element to add child elements in a sorted order
            foreach (XmlElement parentElement in _pagingInfoTable.Keys)
            {
                List<XmlElement> childElements = (List<XmlElement>)_pagingInfoTable[parentElement];

                if (childElements.Count > 0)
                {
                    XMLSchemaComplexType childComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[childElements[0].Name];

                    if (childComplexType != null)
                    {
                        XMLSchemaElementCollection sortAttributes = GetSortAttributes(childComplexType);
                        bool hasSortAttributes = (sortAttributes.Count > 0 ? true : false);

                        List<XmlElement> sortedChildElements = new List<XmlElement>();

                        int pos;
                        foreach (XmlElement childElement in childElements)
                        {
                            pos = GetInsertPosition(sortedChildElements, childElement, sortAttributes);

                            if (pos >= 0)
                            {
                                sortedChildElements.Insert(pos, childElement);
                            }
                            else
                            {
                                sortedChildElements.Add(childElement);
                            }
                        }

                        childElements = sortedChildElements;
                    }
                }

                foreach (XmlElement childElement in childElements)
                {
                    parentElement.AppendChild(childElement);
                }
            }
        }

        private int GetInsertPosition(List<XmlElement> sortedChildElements, XmlElement childElement, XMLSchemaElementCollection sortAttributes)
        {
            int insertPos = -1;

            // HACK, obly use one sort attribute to sort
            if (sortAttributes.Count > 0)
            {
                XMLSchemaElement sortAttribute = (XMLSchemaElement)sortAttributes[0];

                int pos = 0;
                string val1, val2;
                val2 = null;
                if (childElement[sortAttribute.Caption] != null)
                {
                    val2 = childElement[sortAttribute.Caption].InnerText;
                }

                foreach (XmlElement sortedChildElement in sortedChildElements)
                {
                    val1 = null;
                    if (sortedChildElement[sortAttribute.Caption] != null)
                    {
                        val1 = sortedChildElement[sortAttribute.Caption].InnerText;
                    }

                    int result = CompareValues(val1, val2, sortAttribute); // 1 if val1 > val2, 0 if val1 = val2, -1 if val1 < val2
                    if (sortAttribute.IsSortAscending)
                    {
                        if (result > 0)
                        {
                            insertPos = pos;
                            break;
                        }
                    }
                    else
                    {
                        if (result < 0)
                        {
                            insertPos = pos;
                            break;
                        }
                    }

                    pos++;
                }
            }

            return insertPos;
        }

        private int CompareValues(string val1, string val2, XMLSchemaElement sortAttribute)
        {
            int result = 0;

            switch (sortAttribute.ElementType)
            {
                case "integer":

                    int intVal1, intVal2;

                    try
                    {
                        intVal1 = int.Parse(val1);
                        intVal2 = int.Parse(val2);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    if (intVal1 > intVal2)
                    {
                        result = 1;
                    }
                    else if (intVal1 < intVal2)
                    {
                        result = -1;
                    }

                    break;

                case "float":

                    float fVal1, fVal2;

                    try
                    {
                        fVal1 = float.Parse(val1);
                        fVal2 = float.Parse(val2);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    if (fVal1 > fVal2)
                    {
                        result = 1;
                    }
                    else if (fVal1 < fVal2)
                    {
                        result = -1;
                    }

                    break;

                case "double":

                    double dVal1, dVal2;

                    try
                    {
                        dVal1 = double.Parse(val1);
                        dVal2 = double.Parse(val2);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    if (dVal1 > dVal2)
                    {
                        result = 1;
                    }
                    else if (dVal1 < dVal2)
                    {
                        result = -1;
                    }

                    break;

                default:

                    result = String.Compare(val1, val2);

                    break;
            }

            return result;
        }

        // gets information indicating whether a xml element refers to a complex type
        private bool IsComplexType(XMLSchemaElement schemaElement)
        {
            bool status = false;
            XMLSchemaComplexType childComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[schemaElement.Caption];

            if (childComplexType != null &&
                childComplexType.Name == schemaElement.Name)
            {
                status = true;
            }

            return status;
        }

        // add database instances as child xml elements
        private void AddChildElementsFromDatabase(XmlDocument doc, XmlElement parentElement, XMLSchemaElement currentSchemaElement, DataViewModel relatedDataView, XMLSchemaComplexType childComplexType, bool isPaging)
        {
            XmlReader xmlReader;
            QueryReader reader = null;
            XmlDocument resultDoc;
            DataSet ds;
            InstanceView relatedInstanceView = null;
            int numOfInstances;
            XmlElement childElement;

            // create the xml child elements using database instances
            string query = relatedDataView.SearchQuery;

            Interpreter interpreter = new Interpreter();

            interpreter.IsPaging = true;
            interpreter.PageSize = 500;
            interpreter.OmitArrayData = false;
            reader = interpreter.GetQueryReader(query);

            try
            {
                int pageIndex = 1;
                resultDoc = reader.GetNextPage(); // get the first page
                while (resultDoc != null && resultDoc.DocumentElement.ChildNodes.Count > 0)
                {
                    // convert xml data into instance view
                    xmlReader = new XmlNodeReader(resultDoc);
                    ds = new DataSet();
                    ds.ReadXml(xmlReader);
                    relatedInstanceView = new InstanceView(relatedDataView, ds);
                    if (DataSetHelper.IsEmptyDataSet(ds, relatedDataView.BaseClass.ClassName))
                    {
                        break;
                    }

                    numOfInstances = relatedInstanceView.InstanceCount;

                    // Create child xml elements for each of related data instances
                    
                    for (int i = 0; i < numOfInstances; i++)
                    {
                        relatedInstanceView.SelectedIndex = i;

                        if (MeetFilterCondition(resultDoc, i, currentSchemaElement))
                        {
                            if (!currentSchemaElement.IsMergingInstances || string.IsNullOrEmpty(currentSchemaElement.InstanceIdentifyAttribute))
                            {
                                // new child element is appended to parent xml element
                                childElement = doc.CreateElement(XmlConvert.EncodeName(currentSchemaElement.Caption));

                                parentElement.AppendChild(childElement);

                                AddChildElements(doc, childElement, relatedInstanceView, childComplexType, isPaging);
                            }
                            else
                            {
                                childElement = FindMatchedElement(parentElement, currentSchemaElement, relatedInstanceView);
                                if (childElement != null)
                                {
                                    // found an existing element, merge the values to the existing child element
                                    MergeChildElements(doc, childElement, relatedInstanceView, childComplexType);
                                }
                                else
                                {
                                    // unable to fing a matched child element, append the new child element to the parent
                                    childElement = doc.CreateElement(XmlConvert.EncodeName(currentSchemaElement.Caption));

                                    parentElement.AppendChild(childElement);

                                    AddChildElements(doc, childElement, relatedInstanceView, childComplexType, isPaging);
                                }
                            }
                        }
                    }

                    resultDoc = reader.GetNextPage();

                    pageIndex++;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private DataViewModel AddSortCriretia(DataViewModel orignalDataView, XMLSchemaComplexType complexType)
        {
            XMLSchemaElementCollection sortAttributes = GetSortAttributes(complexType);
            bool hasSortAttributes = (sortAttributes.Count > 0 ? true : false);

            DataViewModel dataView = orignalDataView;

            if (hasSortAttributes)
            {
                dataView.ClearSortBy(); // clear the sort expresssion that may exist

                foreach (XMLSchemaElement sortAttribute in sortAttributes)
                {
                    // add as a sort attribute
                    SortAttribute sortElement = new SortAttribute(sortAttribute.Name, dataView.BaseClass.Alias);
                    if (sortAttribute.IsSortAscending)
                    {
                        sortElement.SortDirection = Newtera.Common.MetaData.Schema.SortDirection.Ascending;
                    }
                    else
                    {
                        sortElement.SortDirection = Newtera.Common.MetaData.Schema.SortDirection.Descending;
                    }

                    dataView.SortBy.SortAttributes.Add(sortElement);
                }
            }

            return dataView;
        }

        private XMLSchemaElementCollection GetSortAttributes(XMLSchemaComplexType complexType)
        {
            XMLSchemaElementCollection sortAttributes = new XMLSchemaElementCollection();

            foreach (XMLSchemaElement childXmlSchemaElement in complexType.Elements)
            {
                if (childXmlSchemaElement.IsSortEnabled)
                {
                    int index = 0;
                    foreach (XMLSchemaElement sortAttribute in sortAttributes)
                    {
                        if (sortAttribute.SortOrder > childXmlSchemaElement.SortOrder)
                        {
                            break;
                        }

                        index++;
                    }

                    // add to the list according to the sort order
                    sortAttributes.Insert(index, childXmlSchemaElement);
                }
            }

            return sortAttributes;
        }

        // find a xml element which matches the identifying criteria
        private XmlElement FindMatchedElement(XmlElement parentElement, XMLSchemaElement currentSchemaElement, InstanceView relatedInstanceView)
        {
            XmlElement matchedElement = null;

            string identifyingValue = relatedInstanceView.InstanceData.GetAttributeStringValue(currentSchemaElement.InstanceIdentifyAttribute);
            if (!string.IsNullOrEmpty(identifyingValue))
            {
                InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor)relatedInstanceView.GetProperties(null)[currentSchemaElement.InstanceIdentifyAttribute];
                if (ipd != null)
                {
                    // xml element uses the display name of the identifying attribute as element name
                    string elementName = XmlConvert.EncodeName(ipd.DisplayName);

                    // try to find an existing xml element has the same identifier
                    foreach (XmlElement childElement in parentElement.ChildNodes)
                    {
                        if (childElement[elementName] != null && childElement[elementName].InnerText == identifyingValue)
                        {
                            matchedElement = childElement;
                            break;
                        }
                    }
                }
            }

            return matchedElement;
        }

        // Add imported instances as child xml elements
        // return true if there are new elements are added to the parent elements as children, false if no more elements are added
        private void AddChildElementsFromImports(XmlDocument doc, XmlElement parentElement, XMLSchemaElement currentSchemaElement, DataViewModel relatedDataView, XMLSchemaComplexType childComplexType, bool isPaging)
        {
            //ErrorLog.Instance.WriteLine("AddChildElementsFromImports called for " + currentSchemaElement.Caption + " with fileBaseDirPath = " + _fileBaseDirPath);

            MappingPackage mappingPackage;
            XmlDocument resultDoc;
            XmlElement childElement;

            if (!string.IsNullOrEmpty(_fileBaseDirPath))
            {
                if (!Directory.Exists(_fileBaseDirPath))
                {
                    Directory.CreateDirectory(_fileBaseDirPath);
                }

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(_fileBaseDirPath);

                FileIterator fileIterator = new FileIterator(dirInfo);

                MappingPackageCollection mappingPackages = _metaData.MappingManager.GetMappingPackagesByClass(DataSourceType.Unknown, relatedDataView.BaseClass.ClassName);

                IImportManager importManager = CreateImportManager();

                string filePath;
                while ((filePath = fileIterator.GetNextFile()) != null)
                {
                    // skip the system files
                    if (IsSystemFile(filePath))
                    {
                        continue;
                    }

                    foreach (string scriptName in currentSchemaElement.ImportScriptNames)
                    {
                        // show the import script
                        mappingPackage = GetMappingPackage(mappingPackages, scriptName);
                        if (mappingPackage != null && IsFileTypeMatched(mappingPackage, filePath))
                        {
                            // Get the dataset converted from the file
                            DataSet ds = null;
                            bool status = mappingPackage.IsPaging;
                            try
                            {
                                // disable the paging
                                mappingPackage.IsPaging = false;
                                ds = importManager.ConvertToDestinationDataSet(_connectionStr, filePath, mappingPackage);
                            }
                            catch (Exception ex)
                            {
                                ds = null;
                                ErrorLog.Instance.WriteLine("Failed to convert data from file " + filePath + " with import script " + scriptName + "\n with error " + ex.Message + "\n" + ex.StackTrace);
                            }
                            finally
                            {
                                mappingPackage.IsPaging = status;
                            }

                            if (ds != null)
                            {
                                InstanceView relatedInstanceView = new InstanceView(relatedDataView, ds);

                                resultDoc = ConvertDataTableToXmlDoc(ds.Tables[relatedDataView.BaseClass.ClassName]);
                                // check to see if the file and mapping package match by using validate condition
                                // specified in schemaElement
                                if (!IsValidResult(resultDoc, currentSchemaElement))
                                {
                                    //ErrorLog.Instance.WriteLine("Invalid result");

                                    // skip to the next import script or next file
                                    break;
                                }

                                int numOfInstances = DataSetHelper.GetRowCount(ds, relatedDataView.BaseClass.ClassName);

                                // create child xml elements for the related data instances
                                for (int i = 0; i < numOfInstances; i++)
                                {
                                    relatedInstanceView.SelectedIndex = i;

                                    if (!ContainDuplicateKey(resultDoc, i, currentSchemaElement) &&
                                        MeetFilterCondition(resultDoc, i, currentSchemaElement))
                                    {
                                        if (!currentSchemaElement.IsMergingInstances || string.IsNullOrEmpty(currentSchemaElement.InstanceIdentifyAttribute))
                                        {
                                            childElement = doc.CreateElement(XmlConvert.EncodeName(currentSchemaElement.Caption));
  
                                            List<XmlElement> childElements;
                                            // add the child element in a list instead of adding to the parent element
                                            if (!_pagingInfoTable.ContainsKey(parentElement))
                                            {
                                                // create a list to add child elements
                                                childElements = new List<XmlElement>();
                                                _pagingInfoTable[parentElement] = childElements;
                                            }
                                            else
                                            {
                                                childElements = (List<XmlElement>)_pagingInfoTable[parentElement];
                                            }

                                            childElements.Add(childElement);


                                            AddChildElements(doc, childElement, relatedInstanceView, childComplexType, isPaging);
                                        }
                                        else
                                        {
                                            childElement = FindMatchedElement(parentElement, currentSchemaElement, relatedInstanceView);
                                            if (childElement != null)
                                            {
                                                // found an existing element, merge the values to the existing child element
                                                MergeChildElements(doc, childElement, relatedInstanceView, childComplexType);
                                            }
                                            else
                                            {
                                                // unable to fing a matched child element, append the new child element to the parent
                                                childElement = doc.CreateElement(XmlConvert.EncodeName(currentSchemaElement.Caption));

                                                List<XmlElement> childElements;
                                                // add the child element in a list instead of adding to the parent element
                                                if (!_pagingInfoTable.ContainsKey(parentElement))
                                                {
                                                    // create a list to add child elements
                                                    childElements = new List<XmlElement>();
                                                    _pagingInfoTable[parentElement] = childElements;
                                                }
                                                else
                                                {
                                                    childElements = (List<XmlElement>)_pagingInfoTable[parentElement];
                                                }

                                                childElements.Add(childElement);

                                                AddChildElements(doc, childElement, relatedInstanceView, childComplexType, isPaging);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorLog.Instance.WriteLine("Type of the file " + filePath + " doesn't match type of the script " + scriptName);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Base directory for the import files isn't defined");
            }
        }

        // check to see if the file and mapping package match by using validate condition
        // specified in schemaElement
        private bool IsValidResult(XmlDocument xmlDoc, XMLSchemaElement xmlSchemaElement)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(xmlSchemaElement.ValidateCondition))
            {
                CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
                XmlElement originalInstance = principal.CurrentInstance;
                try
                {
                    if (principal != null)
                    {
                        // only check the first element
                        XmlElement currentInstance = GetXmlInstance(xmlDoc, 0);

                        if (currentInstance != null)
                        {
                            principal.CurrentInstance = currentInstance;

                            // build a complete xquery
                            string finalCondition = TEMPLATE_XQUERY.Replace("$$", xmlSchemaElement.ValidateCondition);

                            IConditionRunner conditionRunner = PermissionChecker.Instance.ConditionRunner;

                            status = conditionRunner.IsConditionMet(finalCondition);
                        }
                    }

                    return status;
                }
                finally
                {
                    // unset the current instance as a context for condition evaluation
                    principal.CurrentInstance = originalInstance;
                }
            }

            return status;
        }

        // check if the data instance contains a duplicate key for the time axis
        private bool ContainDuplicateKey(XmlDocument xmlDoc, int rowIndex, XMLSchemaElement xmlSchemaElement)
        {
            bool status = false;

            // duplicate key is allowed if there is a category axis which divides the data into groups
            if (_categoryAxisElement == null && _xAxisElement != null)
            {
                XmlElement currentInstance = GetXmlInstance(xmlDoc, rowIndex);

                if (currentInstance != null)
                {
                    foreach (XmlElement child in currentInstance.ChildNodes)
                    {
                        if (child.Name == _xAxisElement.Name)
                        {
                            string keyVal = child.InnerText;
                            if (_uniqueKeys[child.InnerText] != null)
                            {
                                status = true;
                            }
                            else
                            {
                                _uniqueKeys[keyVal] = "1";
                            }

                            break;
                        }
                    }
                }
            }

            return status;
        }

        // check if the data instance from a data source meets the filter condition
        private bool MeetFilterCondition(XmlDocument xmlDoc, int rowIndex, XMLSchemaElement xmlSchemaElement)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(xmlSchemaElement.FilterCondition))
            {
                CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
                XmlElement originalInstance = principal.CurrentInstance;
                try
                {
                    if (principal != null)
                    {
                        XmlElement currentInstance = GetXmlInstance(xmlDoc, rowIndex);

                        if (currentInstance != null)
                        {
                            principal.CurrentInstance = currentInstance;

                            // build a complete xquery
                            string finalCondition = TEMPLATE_XQUERY.Replace("$$", xmlSchemaElement.FilterCondition);

                            IConditionRunner conditionRunner = PermissionChecker.Instance.ConditionRunner;

                            status = conditionRunner.IsConditionMet(finalCondition);
                        }
                    }

                    return status;
                }
                finally
                {
                    // unset the current instance as a context for condition evaluation
                    principal.CurrentInstance = originalInstance;
                }
            }

            return status;
        }

        private XmlElement GetXmlInstance(XmlDocument xmlDoc, int rowIndex)
        {
            XmlElement xmlInstance = null;

            if (xmlDoc != null &&
                rowIndex < xmlDoc.DocumentElement.ChildNodes.Count)
            {
                xmlInstance = xmlDoc.DocumentElement.ChildNodes[rowIndex] as XmlElement;
            }

            return xmlInstance;
        }

        private VDocument ConvertDataTableToXmlDoc(DataTable dataTable)
        {
            VDocument doc = null;
            string xml;
            using (StringWriter sw = new StringWriter())
            {
                dataTable.WriteXml(sw);
                xml = sw.ToString();
                doc = DocumentFactory.Instance.Create();
                doc.LoadXml(xml);
            }

            return doc;
        }

        private MappingPackage GetMappingPackage(MappingPackageCollection mappingPackages, string scriptName)
        {
            MappingPackage mappingPackage = null;

            foreach (MappingPackage mp in mappingPackages)
            {
                if (mp.Name == scriptName)
                {
                    mappingPackage = mp;
                    break;
                }
            }

            return mappingPackage;
        }

        // return true if the file matches the import package's data source type
        private bool IsFileTypeMatched(MappingPackage mappingPackage, string filePath)
        {
            bool status = false;

            string fileSuffix = System.IO.Path.GetExtension(filePath).Trim();
            DataSourceType fileType = DataSourceType.Unknown;

            if (!string.IsNullOrEmpty(fileSuffix))
            {
                switch (fileSuffix)
                {
                    case ".xls":
                    case ".xlsx":
                    case ".XLS":
                    case ".XLSX":
                        fileType = DataSourceType.Excel;
                        break;
                    case ".txt":
                    case ".cvs":
                    case ".csv":
                    case ".bat":
                    case ".TXT":
                    case ".CVS":
                    case ".CSV":
                    case ".BAT":
                        fileType = DataSourceType.Text;
                        break;
                    default:
                        fileType = DataSourceType.Other;
                        break;
                }
            }

            if (mappingPackage.DataSourceType == fileType)
            {
                status = true;
            }
        
            return status;
        }

        /// <summary>
        /// Create an IImportManager so that the reference to Newtera.Import is avoid
        /// </summary>
        /// <returns>An IImportManager object</returns>
        private IImportManager CreateImportManager()
        {
            IImportManager importManager = null;
            string typeName = "Newtera.Import.ImportManager,Newtera.Import";

            int index = typeName.IndexOf(",");
            string assemblyName = null;
            string className;

            if (index > 0)
            {
                className = typeName.Substring(0, index).Trim();
                assemblyName = typeName.Substring(index + 1).Trim();
            }
            else
            {
                className = typeName.Trim();
            }
            ObjectHandle obj;
            try
            {
                // try to create a converter from loaded assembly first
                obj = Activator.CreateInstance(assemblyName, className);
                importManager = (IImportManager)obj.Unwrap();
            }
            catch (Exception)
            {
                obj = null;
            }

            if (obj == null)
            {
                // try to create it from a dll
                string dllPath = AppDomain.CurrentDomain.BaseDirectory + assemblyName + ".dll";
                obj = Activator.CreateInstanceFrom(dllPath, className);
                importManager = (IImportManager)obj.Unwrap();
            }

            return importManager;
        }

        private string GetUserFilePath(InstanceView baseInstanceView, string baseDirAttribute)
        {
            string path = NewteraNameSpace.GetUserFilesDir(); ;

            string itemFilePath = baseInstanceView.InstanceData.GetAttributeStringValue(baseDirAttribute);
            if (!string.IsNullOrEmpty(itemFilePath) && itemFilePath.Length > 0)
            {
                // fix the path
                if (itemFilePath.StartsWith(@"\"))
                {
                    itemFilePath = itemFilePath.Substring(1); // remove the \ at beginning since the base path ends with one
                }

                if (!itemFilePath.EndsWith(@"\"))
                {
                    itemFilePath += @"\";
                }

                path = path + itemFilePath;
            }
            else
            {
                path = null;
            }

            return path;
        }

        private bool IsSystemFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            if (fi.Name == TIME_SERIES_FILE ||
                fi.Name == TIME_SERIES_LOG_FILE)
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
    /// An utility class that traverses a root directory and its all subdirs to get each file directly or indirectly under the root dir.
    /// </summary>
    public class FileIterator
    {
        private StringCollection _flatFilelist = null;
        private int _currentIndex;

        public FileIterator(System.IO.DirectoryInfo root)
        {
            _currentIndex = 0;
            _flatFilelist = new StringCollection();
            FlatternFileTree(_flatFilelist, root);
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        public string GetNextFile()
        {
            string currentFileName = null;
            if (_currentIndex < _flatFilelist.Count)
            {
                currentFileName = _flatFilelist[_currentIndex++];
            }

            return currentFileName;
        }

        private void FlatternFileTree(StringCollection fileList, System.IO.DirectoryInfo parentDirectory)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder 
            try
            {
                files = parentDirectory.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException ex)
            {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
                ErrorLog.Instance.WriteLine(ex.Message);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo file in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    fileList.Add(file.FullName);
                }

                // Now find all the subdirectories under this directory.
                subDirs = parentDirectory.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    FlatternFileTree(fileList, dirInfo);
                }
            }
        }
    }
}