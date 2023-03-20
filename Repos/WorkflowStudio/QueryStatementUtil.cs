/*
* @(#)QueryStatementUtil.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.Xml;
	using System.Text;
    using System.IO;
	using System.Threading;
    using System.Windows.Forms;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// A class that provides utilities for parsing XQuery statement.
	/// </summary>
	/// <version>1.0.0 25 Oct 2008 </version>
	public class QueryStatementUtil
	{
        /// <summary>
        /// Gets the xml documment for the data embedded in the xquery
        /// </summary>
        /// <returns>The XmlDocument object</returns>
        private static XmlDocument GetXmlDocument(string xquery)
        {
            XmlDocument doc = null;

            // xml data is enclosed with [[ and ]] in an xquery
            int start = xquery.IndexOf("[[");
            int end = xquery.IndexOf("]]");

            if (start > 0 && end > 0 && start + 2 < end)
            {
                try
                {
                    start = start + 2; // skip the [[
                    int length = end - start;
                    string xml = xquery.Substring(start, length);
                    doc = new XmlDocument();
                    doc.LoadXml(xml);
                }
                catch (Exception)
                {
                    doc = null;
                }
            }

            return doc;
        }

        private static string GetPrimaryKeyValues(XmlElement parent)
        {
            string val = null;

            // primary keys are child nodes under the parent
            foreach (XmlElement child in parent.ChildNodes)
            {
                if (val == null)
                {
                    val = child.InnerText;
                }
                else
                {
                    // multiple primary key values are separated with &
                    val = "&" + child.InnerText;
                }   
            }

            return val;
        }

        /// <summary>
        /// Extract the attribute values from an insert or update xquery and set them to an InstanceView object
        /// </summary>
        /// <param name="instanceView">The instance view to set values to</param>
        /// <param name="xquery">The xquery to be parsed</param>
        public static void SetDataInstanceAttributeValues(InstanceView instanceView, string xquery)
        {
            XmlDocument doc = QueryStatementUtil.GetXmlDocument(xquery);

            if (doc != null)
            {
                XmlElement child;
                string val;
                foreach (InstanceAttributePropertyDescriptor pd in instanceView.GetProperties(null))
                {
                    child = doc.DocumentElement[pd.Name];

                    if (child != null)
                    {
                        val = child.InnerText;
                        if (pd.IsRelationship)
                        {
                            // get the primary key values
                            val = GetPrimaryKeyValues(child);
                        }
                        // do not set the value enclosed with { } to the instance since
                        // they are input parameters
                        if (!string.IsNullOrEmpty(val) && !val.StartsWith("{"))
                        {
                            instanceView.InstanceData.SetAttributeStringValue(pd.Name, val);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extract the attribute values from an insert or update xquery and set them to corresponding ListViewItem
        /// </summary>
        /// <param name="listView">The list view that shows a list of input patameters</param>
        /// <param name="instanceView">The instance view represents the binding data class</param>
        /// <param name="xquery">The xuqery to be parsed</param>
        public static void SetInputParameterBindingValues(ListView listView, string[] boundAttributeNames, InstanceView instanceView, string xquery)
        {
            XmlDocument doc = QueryStatementUtil.GetXmlDocument(xquery);

            if (doc != null)
            {
                XmlElement child;
                string val;
                string parameterName;
                foreach (InstanceAttributePropertyDescriptor pd in instanceView.GetProperties(null))
                {
                    child = doc.DocumentElement[pd.Name];

                    if (child != null)
                    {
                        val = child.InnerText;
                        if (pd.IsRelationship)
                        {
                            // get the primary key values
                            val = GetPrimaryKeyValues(child);
                        }
                        // parameter value is enclosed with {}
                        if (!string.IsNullOrEmpty(val) && val.StartsWith("{"))
                        {
                            parameterName = val.Substring(1, val.Length - 2);
                            // find list view item
                            int index = 0;
                            foreach (ListViewItem listViewItem in listView.Items)
                            {
                                if (listViewItem.Text == parameterName)
                                {
                                    listViewItem.SubItems[1].Text = pd.DisplayName;
                                    boundAttributeNames[index] = pd.Name;
                                }

                                index++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a search expression definition to xml
        /// </summary>
        /// <returns></returns>
        public static string GenerateSearchExprXml(IDataViewElement element)
        {
            // use Marshal and Unmarshal to clone a DataViewModel
            XmlDocument doc = new XmlDocument();
            XmlElement child = doc.CreateElement(ElementFactory.ConvertTypeToString(element.ElementType));
            element.Marshal(child);
            doc.AppendChild(child);

            StringBuilder builder = new StringBuilder();
            StringWriter strWriter = new StringWriter(builder);
            XmlTextWriter writer = new XmlTextWriter(strWriter);
            doc.WriteTo(writer);

            return builder.ToString();
        }


        /// <summary>
        /// Create a search expression object from a xml.
        /// </summary>
        /// <param name="xml"></param>
        public static IDataViewElement CreateSearchExprFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            // create a Filter object and unmarshal from the xml element as source
            IDataViewElement element = ElementFactory.Instance.Create(doc.DocumentElement);

            return element;
        }
	}
}