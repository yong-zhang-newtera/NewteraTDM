using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;

namespace Newtera.SmartWordUtil
{
    public class WordPopulator
    {
        private IDocDataSource _dataSource;

        private const int LevelLimit = 5; // limit level of finding a related class
        private object objMissing = System.Type.Missing;

        private Hashtable _propertyRowIndexTable;

        public const string NameSpaceUri = "http://www.newtera.com/SmartWord.xsd";
        public const string DatabaseAttribute = "dname";
        public const string TypeAttribute = "etype"; // meta data element type
        public const string ElementAttribute = "ename";
        public const string TableAttribute = "tname";
        public const string TaxonomyAttribute = "xname";
        public const string PropertyNameAttribute = "pname";
        public const string ColumnNameAttribute = "cname";
        public const string PathAttribute = "path";
        public const string PropertyNodeName = "Property";
        public const string ArrayNodeName = "Array";
        public const string ColumnNodeName = "Column";
        public const string ViewNodeName = "View";
        public const string FamilyNodeName = "Family";
        public const string DatabaseNodeName = "Database";
        public const string ClassType = "c";
        public const string DataViewType = "d";
        public const string TaxonType = "t";

        public WordPopulator(IDocDataSource dataSource)
        {
            _dataSource = dataSource;
            _propertyRowIndexTable = new Hashtable();
        }

        /// <summary>
        /// Populate the selected instances in the datagrid to the selected xml node in the document
        /// </summary>
        public void PopulateToSelectedNode(Word.XMLNode selectedNode)
        {
            if (selectedNode != null)
            {
                if (selectedNode.BaseName == WordPopulator.DatabaseNodeName)
                {
                    PopulateDatabaseNode(selectedNode);
                }
                else if (selectedNode.BaseName == WordPopulator.FamilyNodeName)
                {
                    PopulateFamilyNode(selectedNode);
                }
            }
        }

        // To see if the view node and familiy node have the same attribute values
        private bool IsFamilyBaseView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            bool status = true;

            if (familyNode.BaseName == WordPopulator.FamilyNodeName &&
                viewNode.BaseName == WordPopulator.ViewNodeName)
            {
                if (GetAttributeValue(familyNode, WordPopulator.ElementAttribute, false) !=
                    GetAttributeValue(viewNode, WordPopulator.ElementAttribute, false))
                {
                    status = false;
                }
                else if (GetAttributeValue(familyNode, WordPopulator.TypeAttribute, false) !=
                    GetAttributeValue(viewNode, WordPopulator.TypeAttribute, false))
                {
                    status = false;
                }
                else if (GetAttributeValue(familyNode, WordPopulator.TaxonomyAttribute, false) !=
                    GetAttributeValue(viewNode, WordPopulator.TaxonomyAttribute, false))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Populate the selected data instances in result grid to all xml nodes in the document that belong to the selected class.
        /// </summary>
        /// <param name="databaseNode">The database node</param>
        public void PopulateDatabaseNode(Word.XMLNode databaseNode)
        {
            string database = GetAttributeValue(databaseNode, WordPopulator.DatabaseAttribute, true);
            if (database == _dataSource.MetaData.SchemaInfo.NameAndVersion)
            {
                foreach (Word.XMLNode viewNode in databaseNode.ChildNodes)
                {
                    string elementName = GetAttributeValue(viewNode, WordPopulator.ElementAttribute, true);
                    // make sure to populate the matched node
                    if (elementName == _dataSource.BaseClassName || _dataSource.IsInheitedClass(elementName))
                    {
                        if (viewNode.BaseName == WordPopulator.FamilyNodeName)
                        {
                            PopulateFamilyNode(viewNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the class to be inserted is related to
        /// the base class of the family node through relationship chain.
        /// </summary>
        /// <param name="familyNode">The family xml node</param>
        /// <param name="selectedMetaDataElement">A meta data element representing the class to be inserted</param>
        /// <returns>true if it is related, false otherwise.</returns>
        public bool IsFamilyRelated(Word.XMLNode familyNode, IMetaDataElement selectedMetaDataElement, out string baseClassCaption, Stack<string> path)
        {
            bool status = false;
            string insertingClassName = null;
            string familyBaseClassName = _dataSource.GetViewClassName(familyNode, out baseClassCaption);

            if (selectedMetaDataElement != null)
            {
                if (selectedMetaDataElement is ClassElement)
                {
                    insertingClassName = ((ClassElement)selectedMetaDataElement).Name;
                }
                else if (selectedMetaDataElement is DataViewModel)
                {
                    insertingClassName = ((DataViewModel)selectedMetaDataElement).BaseClass.Name;
                }
                else if (selectedMetaDataElement is TaxonNode)
                {
                    DataViewModel dataView = ((TaxonNode)selectedMetaDataElement).GetDataView(null);
                    if (dataView != null)
                    {
                        insertingClassName = dataView.BaseClass.Name;
                    }
                }
            }

            if (familyBaseClassName != null && insertingClassName != null)
            {
                if (familyBaseClassName != insertingClassName)
                {
                    // check to see if the inserting class is a related class through relationship attributes
                    // this method is called recursively to travel down the relationship chain
                    status = IsRelatedClass(familyBaseClassName, insertingClassName, null, 0, path);
                }
                else
                {
                    // inserting class is the same as the base class of the family node
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Populate the selected data instance and its related data instances to view nodes
        /// within a family node in the document.
        /// </summary>
        /// <param name="familyNode">The family node</param>
        public void PopulateFamilyNode(Word.XMLNode familyNode)
        {
            foreach (Word.XMLNode viewNode in familyNode.ChildNodes)
            {
                if (IsFamilyBaseView(familyNode, viewNode))
                {
                    // populate the base view node
                    PopulateViewNode(viewNode);
                }
                else if (IsFamilyRelated(familyNode, viewNode))
                {
                    // populate the view node within a family node
                    PopulateFamilyViewNode(familyNode, viewNode);
                }
            }
        }

        /// <summary>
        /// Populate all the property nodes under a view node
        /// </summary>
        /// <param name="viewNode">The view node</param>
        public void PopulateViewNode(Word.XMLNode viewNode)
        {
            string tableName = GetAttributeValue(viewNode, WordPopulator.TableAttribute, false);
            //if (tableName == null)
            //{
                // populate each individual property nodes under the class node with the currently
                // selected data instance
                InstanceView instanceView = _dataSource.GetInstanceView();
                foreach (Word.XMLNode childNode in viewNode.ChildNodes)
                {
                    string propertyName = GetAttributeValue(childNode, WordPopulator.PropertyNameAttribute, true);
                    if (childNode.BaseName == WordPopulator.PropertyNodeName)
                    {
                        PopulatePropertyNode(childNode, propertyName, instanceView);
                    }
                    else if (childNode.BaseName == WordPopulator.ArrayNodeName)
                    {
                        PopulateArrayNode(childNode, propertyName, instanceView);
                    }
                }
            //}
        }

        /// <summary>
        /// Populate all the property nodes under a view node which is related to a family node
        /// </summary>
        /// <param name="familyNode">The family node</param>
        /// <param name="viewNode">The view node</param>
        private void PopulateFamilyViewNode(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            // populate each individual property nodes under the view node with the data instance
            // that is related to the data instances selected in the result grid
            InstanceView instanceView = _dataSource.GetInstanceView(familyNode, viewNode);
 
            string tableName = GetAttributeValue(viewNode, WordPopulator.TableAttribute, false);
            if (tableName == null)
            {
                foreach (Word.XMLNode childNode in viewNode.ChildNodes)
                {
                    if (instanceView != null)
                    {
                        // since a property of a related class can appear more than once in the doc,
                        // we assume that each subsequent appearance represents a data in a different
                        // row in the related class, therefore, we need to set current index of the InstanceView
                        // to the corresponsing row index, so that it will return the data in the corresponding row
                        string propertyName = GetAttributeValue(childNode, WordPopulator.PropertyNameAttribute, true);

                        int nextRowIndex = GetNextRowIndex(familyNode, viewNode, propertyName);
                        int count = 0;
                        try
                        {
                            count = instanceView.InstanceData.DataSet.Tables[instanceView.DataView.BaseClass.Name].Rows.Count;
                        }
                        catch (Exception)
                        {
                            count = 0;
                        }

                        if (nextRowIndex < count)
                        {
                            if (instanceView.SelectedIndex != nextRowIndex)
                            {
                                instanceView.SelectedIndex = nextRowIndex;
                            }

                            if (childNode.BaseName == WordPopulator.PropertyNodeName)
                            {
                                PopulatePropertyNode(childNode, propertyName, instanceView);
                            }
                            else if (childNode.BaseName == WordPopulator.ArrayNodeName)
                            {
                                PopulateArrayNode(childNode, propertyName, instanceView);
                            }
                        }
                        else
                        {
                            // current row index does'nt have corresponding row in the database
                            childNode.Text = "";
                        }
                    }
                    else
                    {
                        childNode.Text = "";
                    }
                }
            }
            else
            {
                // populate the table rows with with the data instance
                // that are related to the data instances selected in the data grid
                PopulateTable(familyNode, viewNode, instanceView);
            }
        }

        /// <summary>
        /// Populate a property node
        /// </summary>
        /// <param name="propertyNode">The property node</param>
        /// <param name="propertyName"></param>
        /// <param name="instanceView"></param>
        private void PopulatePropertyNode(Word.XMLNode propertyNode,
            string propertyName, InstanceView instanceView)
        {
            PropertyDescriptorCollection properties = instanceView.GetProperties(null);
            InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor) properties[propertyName];
            if (ipd != null)
            {
                if (ipd.PropertyType.IsEnum && ipd.IsMultipleChoice)
                {
                    object[] vEnums = (object[])ipd.GetValue();
                    if (vEnums != null && vEnums.Length > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < vEnums.Length; i++)
                        {
                            if (i > 0)
                            {
                                builder.Append(",");
                            }

                            builder.Append(vEnums[i].ToString());
                        }

                        propertyNode.Text = builder.ToString();
                    }
                    else
                    {
                        propertyNode.Text = "";
                    }
                }
                else if (ipd.IsImage)
                {
                    // insert image
                    object val = ipd.GetValue();
                    string imageId = null;
                    if (val != null)
                    {
                        imageId = val.ToString();
                    }

                    if (!string.IsNullOrEmpty(imageId))
                    {
                        string imageFilePath = GetImageFilePath(imageId);

                        if (System.IO.File.Exists(imageFilePath))
                        {
                            Object oMissed = propertyNode.Range; //the position you want to insert
                            Object oLinkToFile = false;  //default
                            Object oSaveWithDocument = true;//default
                            propertyNode.OwnerDocument.InlineShapes.AddPicture(imageFilePath, ref  oLinkToFile, ref  oSaveWithDocument, ref  oMissed);
                        }
                        else
                        {
                            propertyNode.Text = "";
                        }
                    }
                }
                else
                {
                    object val = ipd.GetValue();
                    if (val != null)
                    {
                        propertyNode.Text = val.ToString();
                    }
                    else
                    {
                        propertyNode.Text = "";
                    }
                }
            }
            else
            {
                // unable to find the corresponding property from the instance view, clear the propertyNode value
                propertyNode.Text = "";
            }
        }


        /// <summary>
        /// Gets image file path at server
        /// </summary>
        private string GetImageFilePath(string imageId)
        {
            // Get user image dir
            string actualImageDir = NewteraNameSpace.GetUserImageDir();

            return actualImageDir + imageId;
        }

        /// <summary>
        /// Populate a column node
        /// </summary>
        /// <param name="columnNode">The column node</param>
        /// <param name="columnName">The column name</param>
        /// <param name="dataRow">The row data</param>
        private void PopulateColumnNode(Word.XMLNode columnNode,
            string columnName, DataRow dataRow)
        {
            string val = "";
            if (columnName != null)
            {
                try
                {
                    val = dataRow[columnName].ToString();
                }
                catch (Exception)
                {
                    // for one-dimension array, the column name is Unknown which does not exist in data row
                    // get the value from the column zero
                    val = dataRow[0].ToString();
                }

                columnNode.Text = val;
            }
            else
            {
                columnNode.Text = "";
            }
        }

        /// <summary>
        /// Populate an array node
        /// </summary>
        /// <param name="arrayNode">The array node</param>
        /// <param name="arrayName">The array name</param>
        /// <param name="instanceView"></param>
        private void PopulateArrayNode(Word.XMLNode arrayNode,
            string arrayName, InstanceView instanceView)
        {
            PropertyDescriptorCollection properties = instanceView.GetProperties(null);
            InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor)properties[arrayName];
            if (ipd != null)
            {
                if (ipd.IsArray)
                {
                    DataTable arrayData = null;
                    ArrayDataTableView arrayDataTableView = (ArrayDataTableView)ipd.GetValue();
                    if (arrayDataTableView != null)
                    {
                        arrayData = arrayDataTableView.ArrayAttributeValue;
                    }

                    if (arrayData != null && arrayData.Columns.Count > 0)
                    {
                        // find the table contained by the array node
                        Word.Table arraytable = arrayNode.Range.Tables[1];
                        if (!IsArrayTable(arraytable, arrayData))
                        {
                            // for some reason, when the array table is nested table
                            // arrayNode.Range.Tables[1] return the root table,
                            // therefore, we have to search for the nested table
                            arraytable = FindNestedTable(arraytable, arrayData);
                            if (arraytable == null)
                            {
                                throw new Exception("Unable to find table for array " + arrayName);
                            }
                        }

                        // Remove any data in the table.
                        if (arraytable.Rows.Count > 2 && NeedDel(arrayNode, arraytable))
                        {
                            RemoveDataRows(arraytable);
                        }

                        // clear the old content
                        foreach (Word.XMLNode columnNode in arrayNode.ChildNodes)
                        {
                            columnNode.Text = "";
                        }

                        // populate the array table with the data in DataTable
                        PopulateArrayTableRows(arrayNode, arraytable, arrayData);
                    }
                }
            }
        }

        /// <summary>
        /// Populate the rows in a table with the provided InstanceView
        /// </summary>
        /// <param name="familyNode">The family node that contains the view node, can be null</param>
        /// <param name="viewNode">Word XmlNode representing a view</param>
        private void PopulateTable(Word.XMLNode familyNode, Word.XMLNode viewNode, InstanceView instanceView)
        {
            if (viewNode.Range.Tables.Count == 1)
            {
                // find the table contained by the view node
                //Remember: arrays through interop in Word are 1 based.
                Word.Table table = viewNode.Range.Tables[1];
                // Remove any data in the table.
                if (table.Rows.Count > 2 && NeedDel(viewNode, table))
                {
                    RemoveDataRows(table);
                }

                // clear the old content
                foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
                {
                    propertyNode.Text = "";
                }

                // populate the table with the data instances with data in the instanceView
                PopulateTableRows(familyNode, viewNode, table, instanceView);
            }
        }

        /// <summary>
        /// Deletes all except the two template rows in the table.
        /// </summary>
        /// <param name="table"></param>
        private void RemoveDataRows(Word.Table table)
        {
            if (table.Rows.Count < 2)
            {
                throw new ApplicationException(Newtera.SmartWordUtil.MessageResourceManager.GetString("SmartWord.InvalidTable"));
            }
            try
            {
                for (int i = table.Rows.Count; i > 2; i--)
                {
                    table.Rows[i].Delete();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Populates the rows of the table in the document with the data instances retrived from database.
        /// </summary>
        /// <param name="familyNode">The family node that contains the view node.</param>
        /// <param name="viewNode">The view node</param>
        /// <param name="table">Word table</param>
        /// <param name="instanceView">The instanceview</param>
        private void PopulateTableRows(Word.XMLNode familyNode, Word.XMLNode viewNode, Word.Table table, InstanceView instanceView)
        {
            int rowId = 0;
            int propertiesPerRow = viewNode.ChildNodes.Count;

            if (instanceView != null)
            {
                int count = 0;
                try
                {
                    count = instanceView.InstanceData.DataSet.Tables[instanceView.DataView.BaseClass.Name].Rows.Count;
                }
                catch (Exception)
                {
                    // no result
                    count = 0;
                }

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        // Add item rows only after the first template row.
                        if (rowId > 0)
                        {
                            table.Rows.Add(ref objMissing);
                        }

                        instanceView.SelectedIndex = i; ;

                        // populate a table row
                        PopulateTableRow(viewNode, rowId, instanceView, propertiesPerRow);

                        // Move to the next item row in the table.
                        rowId++;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the rows of the table representing an array with the data in a DataTable
        /// </summary>
        /// <param name="arrayNode">The array node</param>
        /// <param name="table">Word table</param>
        /// <param name="arrayData">The array data</param>
        private void PopulateArrayTableRows(Word.XMLNode arrayNode, Word.Table table, DataTable arrayData)
        {
            int rowId = 0;
            int columnsPerRow = arrayNode.ChildNodes.Count;

            if (arrayData.Rows.Count > 0)
            {
                for (int i = 0; i < arrayData.Rows.Count; i++)
                {
                    // Add item rows only after the first template row.
                    if (rowId > 0)
                    {
                        table.Rows.Add(ref objMissing);
                    }

                    DataRow dataRow = arrayData.Rows[i];

                    // populate an array table row
                    PopulateArrayTableRow(arrayNode, rowId, dataRow, columnsPerRow);

                    // Move to the next item row in the table.
                    rowId++;
                }
            }
        }

        /// <summary>
        /// Gets the row index of an instance in a related class for the given property node.
        /// The rule is that if the property node appear first tim in the doc, the row index is 0;
        /// the second time, row index is 1, and so on.
        /// </summary>
        /// <param name="familyNode">The family node that contains the view node</param>
        /// <param name="viewNode">The view node that contains the property node</param>
        /// <param name="propertyName">The property name</param>
        /// <returns></returns>
        private int GetNextRowIndex(Word.XMLNode familyNode, Word.XMLNode viewNode, string propertyName)
        {
            int rowIndex = 0;
            string path = GetAttributeValue(viewNode, WordPopulator.PathAttribute, false);
            string key = path + ":" + propertyName;
            if (_propertyRowIndexTable[key] != null)
            {
                rowIndex = (int)_propertyRowIndexTable[key];
                // increase the row index for the property
                _propertyRowIndexTable[key] = rowIndex + 1;
            }
            else
            {
                _propertyRowIndexTable[key] = 1; // next row index is 1
            }

            return rowIndex;
        }

        /// <summary>
        /// Gets the information indicating whether a view node represents a class that is related to the
        /// class represented by a family node
        /// </summary>
        /// <param name="familyNode">The family xml node</param>
        /// <param name="viewNode">A view node</param>
        /// <returns>true if it is related, false otherwise.</returns>
        private bool IsFamilyRelated(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            bool status = false;
            string caption = null;

            string elementName = GetAttributeValue(viewNode, WordPopulator.ElementAttribute, false);
            string elementType = GetAttributeValue(viewNode, WordPopulator.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(viewNode, WordPopulator.TaxonomyAttribute, false);

            IMetaDataElement metaDataElement = _dataSource.GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

            Stack<string> path = new Stack<string>();
            status = IsFamilyRelated(familyNode, metaDataElement, out caption, path);

            return status;
        }

        /// <summary>
        /// Gets information indicating whether two classes (base and related classes) are related through relationships.
        /// This method is called recursively to travel down the relationship chain until the related class in question
        /// is reached, or exhausted all the possible paths, or max length of travelling is reached
        /// </summary>
        /// <param name="currentClassName">The class name which is currently used as a base class</param>
        /// <param name="relatedClassName">The class name to see if it is related to the current base class</param>
        /// <param name="parentClassName">The name of class where it comes from</param>
        /// <param name="level">The level of recursive method call</param>
        /// <returns></returns>
        private bool IsRelatedClass(string baseClassName, string theClassName, string parentClassName, int level, Stack<string> path)
        {
            bool status = false;

            ClassElement classElement = _dataSource.MetaData.SchemaModel.FindClass(baseClassName);

            while (classElement != null)
            {
                foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
                {
                    // do not trace the relationship that linked back to class where we come from
                    if (parentClassName == null ||
                        parentClassName != relationship.LinkedClass.Name)
                    {
                        path.Push(classElement.Name + ":" + relationship.Name);
                        ClassElement linkedClass = relationship.LinkedClass;
                        if (linkedClass.Name == theClassName || IsParentOf(linkedClass.Name, theClassName))
                        {
                            status = true;
                            break;
                        }
                        else if (level < LevelLimit) // to prevent dead loop in case there is a circular relationship
                        {
                            // travel down the relaionship chain
                            status = IsRelatedClass(linkedClass.Name, theClassName, baseClassName, level + 1, path);
                            if (status)
                            {
                                break;
                            }
                        }
                    }
                }

                if (status)
                {
                    break;
                }

                // go up class hierarchy
                classElement = classElement.ParentClass;
            }

            if (!status && path.Count > 0)
            {
                // it's a wrong path, pop the relationhsip
                path.Pop();
            }

            return status;
        }

        private bool IsParentOf(string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = _dataSource.MetaData.SchemaModel.FindClass(childClassName);
            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        private Word.Table FindNestedTable(Word.Table parentTable, DataTable arrayData)
        {
            Word.Table foundTable = null;
            for (int col = 0; col < parentTable.Columns.Count; col++)
            {
                for (int row = 0; row < parentTable.Rows.Count; row++)
                {
                    // rows in the parent table may not have same amount of columns, ignore the
                    try
                    {
                        if (row < parentTable.Rows.Count &&
                            col < parentTable.Columns.Count &&
                            parentTable.Cell(row + 1, col + 1).Tables != null &&
                            parentTable.Cell(row + 1, col + 1).Tables.Count == 1)
                        {
                            Word.Table nestedTable = parentTable.Cell(row + 1, col + 1).Tables[1];
                            if (IsArrayTable(nestedTable, arrayData))
                            {
                                foundTable = nestedTable;
                                break;
                            }
                            else
                            {
                                foundTable = FindNestedTable(nestedTable, arrayData);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    

                    if (foundTable != null)
                    {
                        break;
                    }
                }
            }

            return foundTable;
        }

        private bool IsArrayTable(Word.Table table, DataTable arrayData)
        {
            bool status = true;

            for (int col = 0; col < table.Columns.Count; col++)
            {
                string colName = table.Cell(1, col + 1).Range.Text;
                // colname may end with \r\a, trim it
                int pos = colName.IndexOf("\r");
                if (pos > 0)
                {
                    colName = colName.Substring(0, pos);
                }

                // if a table column doesn't exist in array data's column, then it is not table for the array attribute
                if (arrayData.Columns[colName] == null)
                {
                    status = false;
                    break;
                }
            }

            return status;
        }

        //by zhang.jingyuan
        private bool NeedDel(Word.XMLNode viewNode, Word.Table table)
        {
            bool _isNeedDel=true;

            try
            {
                int colCount = table.Rows[1].Cells.Count;
                int rowsCount = table.Rows.Count;
                int nodesCount = 0;

                for (int row = 2; row <= rowsCount; row++)
                {
                    if (table.Rows[row].Cells.Count != table.Rows[1].Cells.Count)
                    {
                        _isNeedDel = false;
                    }
                }

                foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
                {
                    nodesCount++;

                }

                if (nodesCount == (rowsCount - 1) * colCount)
                {
                    _isNeedDel = true;
                }
                else
                {
                    _isNeedDel = false;
                }
            }
            catch
            {
                _isNeedDel = false;
            }

            return _isNeedDel;
        }

        /// <summary>
        /// Populates a row in the table.
        /// </summary>
        /// <param name="viewNode">The view node that contains the table</param>
        /// <param name="rowId">row id</param>
        /// <param name="instanceView"></param>
        /// <param name="propertiesPerRow">Number of properties per row</param>
        private void PopulateTableRow(Word.XMLNode viewNode, int rowId, InstanceView instanceView, int propertiesPerRow)
        {
            int startPos = rowId * propertiesPerRow;
            int endPos = startPos + propertiesPerRow;
            int position = 0;
            string propertyName = null;
            StringCollection propertyNames = new StringCollection();

            // get the property names for XML node in the first row, since
            // the xml nodes in the new rows are created automatically, hence
            // do not have property name attribute
            int colIndex = 0;
            foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
            {
                propertyName = GetAttributeValue(propertyNode, WordPopulator.PropertyNameAttribute, true);
                propertyNames.Add(propertyName);
                colIndex++;
                if (colIndex >= propertiesPerRow)
                {
                    break;
                }
            }

            // populate each individual property
            colIndex = 0;
            Word.XMLNode attributeNode;
            foreach (Word.XMLNode childNode in viewNode.ChildNodes)
            {
                if (position >= startPos && position < endPos)
                {
                    propertyName = null;
                    try
                    {
                        propertyName = GetAttributeValue(childNode, WordPopulator.PropertyNameAttribute, true);
                    }
                    catch
                    {
                        // add property name attribute to the property node
                        propertyName = propertyNames[colIndex];
                        attributeNode = childNode.Attributes.Add(WordPopulator.PropertyNameAttribute, WordPopulator.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = propertyName;
                    }

                    if (childNode.BaseName == WordPopulator.PropertyNodeName)
                    {
                        PopulatePropertyNode(childNode, propertyName, instanceView);
                    }
                    else if (childNode.BaseName == WordPopulator.ArrayNodeName)
                    {
                        PopulateArrayNode(childNode, propertyName, instanceView);
                    }

                    colIndex++;
                }

                position++;
            }
        }

        /// <summary>
        /// Populates a row in the array table.
        /// </summary>
        /// <param name="arrayNode">The array node that contains the table</param>
        /// <param name="rowId">row id</param>
        /// <param name="dataRow">An array data row</param>
        /// <param name="columnsPerRow">Number of columns per row</param>
        private void PopulateArrayTableRow(Word.XMLNode arrayNode, int rowId, DataRow dataRow, int columnsPerRow)
        {
            int startPos = rowId * columnsPerRow;
            int endPos = startPos + columnsPerRow;
            int position = 0;
            string columnName = null;
            StringCollection columnNames = new StringCollection();

            // get the column names for XML node in the first row, since
            // the xml nodes in the new rows are created automatically, hence
            // do not have column name attribute
            int colIndex = 0;
            foreach (Word.XMLNode columnNode in arrayNode.ChildNodes)
            {
                columnName = GetAttributeValue(columnNode, WordPopulator.ColumnNameAttribute, true);
                columnNames.Add(columnName);
                colIndex++;
                if (colIndex >= columnsPerRow)
                {
                    break;
                }
            }

            // populate each individual property
            colIndex = 0;
            Word.XMLNode attributeNode;
            foreach (Word.XMLNode childNode in arrayNode.ChildNodes)
            {
                if (position >= startPos && position < endPos)
                {
                    columnName = null;
                    try
                    {
                        columnName = GetAttributeValue(childNode, WordPopulator.ColumnNameAttribute, true);
                    }
                    catch
                    {
                        // add column name attribute to the property node
                        columnName = columnNames[colIndex];
                        attributeNode = childNode.Attributes.Add(WordPopulator.ColumnNameAttribute, WordPopulator.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = columnName;
                    }

                    if (childNode.BaseName == WordPopulator.ColumnNodeName)
                    {
                        PopulateColumnNode(childNode, columnName, dataRow);
                    }

                    colIndex++;
                }

                position++;
            }
        }

        /// <summary>
        /// Get value of the specified attribute from a XMLNode
        /// </summary>
        /// <param name="xmlNode">The XMLNode</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="isRequired">indicating whether the attribute is a required attribute</param>
        /// <returns>attribute value</returns>
        /// <exception cref="Exception">Thrown if it is unable to find a required attribute</exception>
        private string GetAttributeValue(Word.XMLNode xmlNode, string attributeName, bool isRequired)
        {
            string val = null;
            foreach (Word.XMLNode attr in xmlNode.Attributes)
            {
                if (attr.BaseName == attributeName)
                {
                    val = attr.NodeValue;
                }
            }

            if (val == null && isRequired)
            {
                throw new Exception("Unable to find attribute " + attributeName + " in xml node " + xmlNode.BaseName);
            }

            return val;
        }
    }
}