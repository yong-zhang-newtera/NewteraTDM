/*
* @(#) DynamicArrayTableViewControl.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using HtmlAgilityPack;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel;
using Newtera.WebForm;

namespace CustomWebFormControls
{
    /// <summary>
    /// An IPropertyControl control that displays enum values of a property in radio button group
    /// </summary>
    public class DynamicArrayTableViewControl : PropertyControlBase
    {
        private const string COLUMN_PREFIX = "C_";

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        public override bool HasValue(JProperty jProperty)
        {
            if (jProperty.Value != null)
            {
                bool hasValue = false;

                JArray rows = jProperty.Value["rows"] as JArray; // The values of rows property is a JArray
                DataTable existingDataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;

                for (int row = 0; row < rows.Count; row++)
                {
                    for (int col = 0; col < existingDataTable.Columns.Count; col++)
                    {
                        if (rows[row]["col" + col] != null &&
                            !string.IsNullOrEmpty(rows[row]["col" + col].ToString()))
                        {
                            hasValue = true;
                            break;
                        }
                    }

                    if (hasValue)
                    {
                        break;
                    }
                }

                return hasValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool HasValue(XmlElement element)
        {
            if (element != null)
            {
                bool hasValue = false;

                XmlNodeList rows = element.ChildNodes; // The child elements are data of array rows
                DataTable existingDataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;

                for (int row = 0; row < rows.Count; row++)
                {
                    XmlElement rowElement = rows[row] as XmlElement;
                    for (int col = 0; col < existingDataTable.Columns.Count; col++)
                    {
                        XmlElement colElement = rowElement[existingDataTable.Columns[col].ColumnName];
                        if (colElement != null &&
                            !string.IsNullOrEmpty(colElement.InnerText))
                        {
                            hasValue = true;
                            break;
                        }
                    }

                    if (hasValue)
                    {
                        break;
                    }
                }

                return hasValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (!IsReadOnly)
            {
                DataTable dataTable = GetDataTable(jProperty);

                ArrayDataTableView arrayDataTableView = (ArrayDataTableView)PropertyInfo.GetValue();

                arrayDataTableView.ArrayAttributeValue = dataTable;
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (!IsReadOnly)
            {
                DataTable dataTable = GetDataTable(element);

                ArrayDataTableView arrayDataTableView = (ArrayDataTableView)PropertyInfo.GetValue();

                arrayDataTableView.ArrayAttributeValue = dataTable;
            }
        }

        /// <summary>
        /// Get a DataTable from the ViewModel value
        /// </summary>
        /// <returns>A DataTable object</returns>
        private DataTable GetDataTable(JProperty jProperty)
        {
            DataTable dataTable = new DataTable(PropertyInfo.Name);
            DataColumn dataColumn;
            DataRow dataRow;

            JArray rows = jProperty.Value["rows"] as JArray; // The values of rows property is a JArray
            DataTable existingDataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;

            for (int col = 0; col < existingDataTable.Columns.Count; col++)
            {
                // Create new DataColumn for each of columns in the array.    
                dataColumn = new DataColumn();
                dataColumn.ColumnName = existingDataTable.Columns[col].ColumnName;
                // Add the Column to the DataColumnCollection.
                dataTable.Columns.Add(dataColumn);
            }

            bool hasValues;
            for (int row = 0; row < rows.Count; row++)
            {
                hasValues = false;

                dataRow = dataTable.NewRow();

                for (int col = 0; col < existingDataTable.Columns.Count; col++)
                {
                    if (rows[row]["col" + col] != null &&
                        !string.IsNullOrEmpty(rows[row]["col" + col].ToString()))
                    {
                        hasValues = true;
                        dataRow[col] = rows[row]["col" + col];
                    }
                }

                if (hasValues)
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Get a DataTable from the XmlElememnt object
        /// </summary>
        /// <returns>A DataTable object</returns>
        private DataTable GetDataTable(XmlElement element)
        {
            DataTable dataTable = new DataTable(PropertyInfo.Name);
            DataColumn dataColumn;
            DataRow dataRow;

            XmlNodeList rows = element.ChildNodes; // Child elements are array rows
            DataTable existingDataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;

            for (int col = 0; col < existingDataTable.Columns.Count; col++)
            {
                // Create new DataColumn for each of columns in the array.    
                dataColumn = new DataColumn();

                dataColumn.ColumnName = existingDataTable.Columns[col].ColumnName;
                // Add the Column to the DataColumnCollection.
                dataTable.Columns.Add(dataColumn);
            }

            bool hasValues;
            XmlElement rowElement;
            XmlElement colElement;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                hasValues = false;

                rowElement = rows[rowIndex] as XmlElement;

                dataRow = dataTable.NewRow();

                for (int col = 0; col < existingDataTable.Columns.Count; col++)
                {
                    colElement = rowElement[existingDataTable.Columns[col].ColumnName];
                    if (colElement != null && !string.IsNullOrEmpty(colElement.InnerText))
                    {
                        hasValues = true;
                        dataRow[col] = GetCellValue(colElement);
                    }
                }

                if (hasValues)
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private string GetCellValue(XmlElement element)
        {
            if (element.ChildNodes == null || element.ChildNodes.Count == 0)
                return element.InnerText;
            else
            {
                string val = "";
                foreach (XmlNode child in element.ChildNodes)
                {
                    if (!string.IsNullOrEmpty(val))
                        val += ",";

                    val += child.InnerText;
                }

                return val;
            }
        }


        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            DataTable dataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;
            JObject arrayObj = new JObject();
            JArray rows = new JArray();

            arrayObj.Add("rows", rows);

            JObject rowObj;
            string val;

            if (dataTable.Rows.Count > 0)
            {
                // count number of columns, the column names start with "C_";
                // It must be an one-dimension array
                int colCount = 0;
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    if (!dataTable.Rows[row].IsNull(0) &&
                        dataTable.Rows[row][0].ToString().Length > 0)
                    {
                        val = dataTable.Rows[row][0].ToString();
                        if (val.StartsWith(COLUMN_PREFIX))
                            colCount++;
                        else
                            break;
                    }
                    else
                    {
                        break;
                    }
                }

                if (colCount > 0)
                {
                    // rows after the column names are data
                    int row = colCount;

                    while (row < dataTable.Rows.Count)
                    {
                        rowObj = new JObject();
                        rows.Add(rowObj);

                        for (int col = 0; col < colCount; col++)
                        {
                            if (row < dataTable.Rows.Count && !dataTable.Rows[row].IsNull(0) &&
                                dataTable.Rows[row][0].ToString().Length > 0)
                            {
                                val = dataTable.Rows[row][0].ToString();
                            }
                            else
                            {
                                val = "";
                            }
                            rowObj.Add("col" + col, val); // name of a cell is "col0", "col1", etc.

                            row++;
                        }
                    }
                }
            }

            //var jsonString = JsonConvert.SerializeObject(arrayObj, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            return arrayObj;
        }


        /// Create a table of text boxes for the array data	
		public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            HtmlNode divNode = this.InstanceEditor.Document.CreateElement("div");
            divNode.SetAttributeValue("class", "table-responsive");

            container.AppendChild(divNode);

            HtmlNode tableNode = this.InstanceEditor.Document.CreateElement("table");
            tableNode.SetAttributeValue("class", "table table-striped");
            divNode.AppendChild(tableNode);

            // table header
            HtmlNode theadNode = this.InstanceEditor.Document.CreateElement("thead");
            tableNode.AppendChild(theadNode);

            HtmlNode trNode = this.InstanceEditor.Document.CreateElement("tr");
            theadNode.AppendChild(trNode);

            DataTable dataTable = ((ArrayDataTableView)PropertyInfo.GetValue()).ArrayAttributeValue;
            // create a table of text boxes for each value in the datatable
            HtmlNode thNode;

            // count number of columns, the column names start with "C_";
            // It must be an one-dimension array
            int colCount = 0;
            string val;
            if (dataTable.Rows.Count > 0)
            {
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    if (!dataTable.Rows[row].IsNull(0) &&
                        dataTable.Rows[row][0].ToString().Length > 0)
                    {

                        val = dataTable.Rows[row][0].ToString();
                        if (val.StartsWith(COLUMN_PREFIX))
                            colCount++;
                        else
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // create columns of a html table
            for (int row = 0; row < colCount; row++)
            {
                thNode = this.InstanceEditor.Document.CreateElement("td");
                val = dataTable.Rows[row][0].ToString().Substring(COLUMN_PREFIX.Length); // Remove the prifix
                if (!string.IsNullOrEmpty(val))
                {
                    thNode.InnerHtml = HtmlDocument.HtmlEncode(val);
                }
                else
                {
                    thNode.InnerHtml = "Col_" + row;
                }
                trNode.AppendChild(thNode);
            }

            HtmlNode tbodyNode = this.InstanceEditor.Document.CreateElement("tbody");
            tbodyNode.SetAttributeValue("ng-repeat", "row in " + this.InstanceEditor.ViewModelPath + PropertyInfo.Name + ".rows" + " track by $index"); // create tr with imput elements for array
            tableNode.AppendChild(tbodyNode);

            HtmlNode tdNode;
            trNode = this.InstanceEditor.Document.CreateElement("tr");
            tbodyNode.AppendChild(trNode);
            HtmlNode inputNode;
            for (int col = 0; col < colCount; col++)
            {
                tdNode = this.InstanceEditor.Document.CreateElement("td");
                trNode.AppendChild(tdNode);

                inputNode = CreateInputElement(col);

                tdNode.AppendChild(inputNode);
            }

            //string arrayName = this.InstanceEditor.ViewModelPath + PropertyInfo.Name + ".rows";
            string arrayPath = PropertyInfo.Name + ".rows";

            return container;
        }  // CreatePropertyNode()

        private HtmlNode CreateInputElement(int colIndex)
        {
            HtmlNode container = this.InstanceEditor.Document.CreateElement("label");
            container.SetAttributeValue("class", "input");

            HtmlNode inputNode = this.InstanceEditor.Document.CreateElement("input");
            inputNode.SetAttributeValue("class", "form-control");
            inputNode.SetAttributeValue("type", "text");

            inputNode.SetAttributeValue("readonly", "readonly");
            inputNode.SetAttributeValue("style", "background-color:#f7f9f9;");

            // set the angularjs  model binding
            inputNode.SetAttributeValue("ng-model", "row.col" + colIndex);

            container.AppendChild(inputNode);

            return container;
        }
    }
}