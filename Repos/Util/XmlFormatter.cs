/*
* @(#)XmlFormatter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Data;
	using System.Text;
	using System.ComponentModel;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The class converts an instance to a text format
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	internal class XmlFormatter : FormatterBase
	{
		private const string XML_TEMPLATE_FILE_NAME = "template.xml";

		/// <summary>
		/// Initiate an instance of XmlFormatter class
		/// </summary>
		public XmlFormatter() : base()
		{
		}

		#region IInstanceFormatter interface implementation

		/// <summary>
		/// Convert an instance data to a XML format and save it to a file.
		/// </summary>
		/// <param name="instanceView">The InstanceView that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <returns>A formatted data in byte array</returns>
		public override void Save(InstanceView instanceView, string filePath)
		{
			XmlNode refNode = null;

			// create an xml document from a template if it exists
			XmlDocument doc = new XmlDocument();
		
			// try to find a template which the same as class name
			string templatePath = NewteraNameSpace.GetAppHomeDir() + @"\Config\" + instanceView.DataView.BaseClass.Name + ".xml";
		
			if (!File.Exists(templatePath))
			{
				// use default template
				templatePath = NewteraNameSpace.GetAppHomeDir() + @"\Config\" + XmlFormatter.XML_TEMPLATE_FILE_NAME;
			}

			if (File.Exists(templatePath))
			{
				doc.Load(templatePath);

				// find the insert location by locating the reference node
				foreach (XmlNode node in doc.DocumentElement.ChildNodes)
				{
					if (node.Name.ToUpper() == "OUTPUT")
					{
						refNode = node;
						break;
					}
				}
			}
			else
			{
				//Create an XML declaration with default encoding. 
				XmlDeclaration xmldecl;
				xmldecl = doc.CreateXmlDeclaration("1.0", null, null);

				XmlElement root = doc.CreateElement("newtera", instanceView.DataView.BaseClass.Name, "http://www.newtera.com");
				root.SetAttribute("Name", instanceView.DataView.BaseClass.Caption);
				doc.AppendChild(root);

				//Add the declaration node to the document.
				root = doc.DocumentElement;
				doc.InsertBefore(xmldecl, root);
			}
			
			PropertyDescriptorCollection properties = instanceView.GetProperties(null);

			// convert each attribute of an instance into corresponding xml elements
			foreach(InstanceAttributePropertyDescriptor pd in properties)
			{
				if (pd.IsArray)
				{
					FormatArray(pd, doc, refNode);
				}
				else if (pd.IsRelationship)
				{
					FormatRelationship(pd, doc, refNode);
				}
				else
				{
					FormatAttribute(pd, doc, refNode);
				}
			}

			if (refNode != null)
			{
				// remove the reference node
				doc.DocumentElement.RemoveChild(refNode);
			}

			// convert xml to a file
			doc.Save(filePath);
		}

		/// <summary>
		/// Convert a DataTable to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a xml formatter.
		public override void Save(DataTable dataTable, string filePath, params object[] args)
		{
			Save(dataTable, null, filePath, args);
		}

		/// <summary>
		/// Convert two DataTable instances as comparison to a corresponding XML format
		/// and save it to a file.
		/// </summary>
		/// <param name="beforeDataTable">The DataTable that stores before data.</param>
		/// <param name="afterDataTable">The DataTable that stores after data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		public override void Save(DataTable beforeDataTable, DataTable afterDataTable, string filePath, params object[] args)
		{
			// create an xml document that has a root element with the class caption
			// of the given instance
			XmlDocument doc = new XmlDocument();

			//Create an XML declaration with default encoding. 
			XmlDeclaration xmldecl;
			xmldecl = doc.CreateXmlDeclaration("1.0", null, null);

			XmlElement root = doc.CreateElement("ArrayData");
			doc.AppendChild(root);

			//Add the declaration node to the document.
			root = doc.DocumentElement;
			doc.InsertBefore(xmldecl, root);
			
			FormatDataTable(beforeDataTable, doc, root);

			// convert xml to a file
			doc.Save(filePath);
		}

		#endregion

		/// <summary>
		/// Convert a value of simple attribute to a xml representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="doc">The xml document</param>
		private void FormatAttribute(InstanceAttributePropertyDescriptor pd,
			XmlDocument doc, XmlNode refNode)
		{
			string namespaceUrl = doc.DocumentElement.NamespaceURI;
			string prefix = doc.DocumentElement.Prefix;

			XmlElement element = doc.CreateElement(prefix, pd.Name, namespaceUrl);
			element.SetAttribute("Name", pd.DisplayName);
			if (pd.GetValue() != null)
			{
				element.InnerText = pd.GetValue().ToString();
			}
			else
			{
				element.InnerText = "";
			}

			if (refNode == null)
			{
				doc.DocumentElement.AppendChild(element);
			}
			else
			{
				doc.DocumentElement.InsertBefore(element, refNode);
			}
		}

		/// <summary>
		/// Convert a value of relationship attribute to a xml representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="doc">The xml document</param>
		/// <param name="refNode">The reference node</param>
		private void FormatRelationship(InstanceAttributePropertyDescriptor pd,
			XmlDocument doc, XmlNode refNode)
		{
			string namespaceUrl = doc.DocumentElement.NamespaceURI;
			string prefix = doc.DocumentElement.Prefix;

			XmlElement element = doc.CreateElement(prefix, pd.Name, namespaceUrl);
			element.SetAttribute("Name", pd.DisplayName);
			if (pd.GetValue() != null)
			{
				element.InnerText = pd.GetValue().ToString();
			}
			else
			{
				element.InnerText = "";
			}

			if (refNode == null)
			{
				doc.DocumentElement.AppendChild(element);
			}
			else
			{
				doc.DocumentElement.InsertBefore(element, refNode);
			}
		}

		/// <summary>
		/// Convert a value of array attribute to a text representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="doc">The xml document</param>
		/// <param name="refNode">Reference node</param>
		private void FormatArray(InstanceAttributePropertyDescriptor pd,
			XmlDocument doc, XmlNode refNode)
		{
			string namespaceUrl = doc.DocumentElement.NamespaceURI;
			string prefix = doc.DocumentElement.Prefix;
			XmlElement sectionElement = null;

			// if there is a section, create an element
			if (pd.Section != null && pd.Section.Length > 0)
			{
				// create a element representing a section
				sectionElement = doc.CreateElement(prefix, pd.Section, namespaceUrl);
			
				if (refNode == null)
				{
					doc.DocumentElement.AppendChild(sectionElement);
				}
				else
				{
					doc.DocumentElement.InsertBefore(sectionElement, refNode);
				}
			}

			ArrayDataTableView arrayView = (ArrayDataTableView) pd.GetValue();
			DataTable arrayTable = arrayView.ArrayAttributeValue;
			XmlElement rowElement;

			if (arrayTable.Columns.Count > 1)
			{
				for (int rowIndex = 0; rowIndex < arrayTable.Rows.Count; rowIndex++)
				{
					// create a element representing a row
					rowElement = doc.CreateElement(prefix, pd.Name, namespaceUrl);
					rowElement.SetAttribute("Name", pd.DisplayName);

					if (sectionElement != null)
					{
						sectionElement.AppendChild(rowElement);
					}
					else if (refNode == null)
					{
						doc.DocumentElement.AppendChild(rowElement);
					}
					else
					{
						doc.DocumentElement.InsertBefore(rowElement, refNode);
					}

					FormatDataRow(arrayTable, rowIndex, doc, rowElement);
				}
			}
			else if (arrayTable.Columns.Count == 1)
			{
				// create a element representing a row
				rowElement = doc.CreateElement(prefix, pd.Name, namespaceUrl);
				if (sectionElement != null)
				{
					sectionElement.AppendChild(rowElement);
				}
				else if (refNode == null)
				{
					doc.DocumentElement.AppendChild(rowElement);
				}
				else
				{
					doc.DocumentElement.InsertBefore(rowElement, refNode);
				}

				for (int rowIndex = 0; rowIndex < arrayTable.Rows.Count; rowIndex++)
				{
					FormatDataRow(arrayTable, rowIndex, doc, rowElement);
				}
			}
		}

		/// <summary>
		/// Convert a DataTable to a xml representation
		/// </summary>
		/// <param name="dataTable">A DataTable instance</param>
		/// <param name="doc">The xml document</param>
		/// <param name="arrayRoot">Array element root</param>
		private void FormatDataTable(DataTable dataTable,
			XmlDocument doc, XmlElement arrayRoot)
		{
			// output array data
			XmlElement rowElement, cellElement;
			int rowCount = 0;
			foreach (DataRow row in dataTable.Rows)
			{
				// create a element representing a row
				rowElement = doc.CreateElement("Row_" + rowCount);
				arrayRoot.AppendChild(rowElement);

				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					cellElement = doc.CreateElement("Col_" + i);
					cellElement.SetAttribute("Name", dataTable.Columns[i].ColumnName);
					cellElement.InnerText = row[i].ToString();
					rowElement.AppendChild(cellElement);
				}

				rowCount++;
			}
		}

		/// <summary>
		/// Convert a DataRow to a xml representation
		/// </summary>
		/// <param name="arrayData">A DataTable instance</param>
		/// <param name="rowIndex">The row index</param>
		/// <param name="doc">The xml document</param>
		/// <param name="rowElement">DataRow xml element</param>
		private void FormatDataRow(DataTable arrayData, int rowIndex,
			XmlDocument doc, XmlElement rowElement)
		{
			// output array row data
			XmlElement cellElement;

			string namespaceUrl = doc.DocumentElement.NamespaceURI;
			string prefix = doc.DocumentElement.Prefix;

			for (int i = 0; i < arrayData.Columns.Count; i++)
			{
				cellElement = doc.CreateElement(prefix, "Col" + i, namespaceUrl);
				cellElement.SetAttribute("Name", arrayData.Columns[i].ColumnName);
				cellElement.InnerText = arrayData.Rows[rowIndex][i].ToString();
				rowElement.AppendChild(cellElement);
			}
		}
	}
}