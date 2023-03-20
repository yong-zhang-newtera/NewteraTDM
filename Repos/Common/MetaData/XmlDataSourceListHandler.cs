/*
* @(#)XmlDataSourceListHandler.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.Xml;
    using System.Threading;
	using System.Collections.Specialized;
    using System.Security.Principal;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Represents an handler that retrieves a list of items from a xml data source
	/// </summary>
	/// <version> 1.0.0 13 Nov 2009 </version>
    public class XmlDataSourceListHandler : IListHandler
	{
        public static IXMLDataSourceService XmlDataSourceService = null;

        private const string DefaultTextField = "Text";
        private const string DefaultValueField = "Value";

		/// <summary>
		/// Get a list of data view names
		/// </summary>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
        /// <param name="parameter">The parameter value that is a xquery</param>
        /// <returns>A collection of EnumValue object</returns>
		public EnumValueCollection GetValues(SchemaModelElement context, string xquery, string parameter, string filterValue, string textField, string valueField)
		{
			EnumValueCollection values = new EnumValueCollection();

            if (context != null && XmlDataSourceListHandler.XmlDataSourceService != null &&
                !string.IsNullOrEmpty(xquery))
            {
                string query = xquery;

                if (query.Contains("?"))
                {
                    if (!string.IsNullOrEmpty(filterValue))
                    {
                        query = query.Replace("?", filterValue); // set filter value to the query
                    }
                    else
                    {
                        query = query.Replace("?", ""); // replace the ?
                    }
                }

                XmlDocument doc = XmlDataSourceListHandler.XmlDataSourceService.Execute(query);

                if (doc.DocumentElement.ChildNodes.Count > 0)
                {
                    EnumValue enumValue;
                    int index = 0;
                    string val;
                    foreach (XmlElement xmlElement in doc.DocumentElement.ChildNodes)
                    {
                        enumValue = new EnumValue();
                        val = index.ToString();
                        if (valueField != null && (xmlElement[valueField] != null || xmlElement.Attributes[valueField] != null))
                        {
                            if (xmlElement[valueField] != null)
                            {
                                val = xmlElement[valueField].InnerText;
                            }
                            else
                            {
                                val = xmlElement.Attributes[valueField].Value;
                            }
                        }
                        else if (xmlElement[DefaultValueField] != null || xmlElement.Attributes[DefaultValueField] != null)
                        {
                            if (xmlElement[DefaultValueField] != null)
                            {
                                val = xmlElement[DefaultValueField].InnerText;
                            }
                            else
                            {
                                val = xmlElement.Attributes[DefaultValueField].Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(val))
                        {
                            // trim the spaces
                            val = val.Trim();
                        }

                        enumValue.Value = val;

                        if (textField != null && (xmlElement[textField] != null || xmlElement.Attributes[textField] != null))
                        {
                            if (xmlElement[textField] != null)
                            {
                                val = xmlElement[textField].InnerText;
                            }
                            else
                            {
                                val = xmlElement.Attributes[textField].Value;
                            }
                        }
                        else if (xmlElement[DefaultTextField] != null || xmlElement.Attributes[DefaultTextField] != null)
                        {
                            if (xmlElement[DefaultTextField] != null)
                            {
                                val = xmlElement[DefaultTextField].InnerText;
                            }
                            else
                            {
                                val = xmlElement.Attributes[DefaultTextField].Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(val))
                        {
                            // trim the spaces
                            val = val.Trim();
                        }

                        enumValue.DisplayText = val;

                        // Check if there is an enum value in the collection that has the same
                        // value or DisplayText as the enumValue to be added, if so, do not add the
                        // enum value
                        if (!IsEnumValueExists(values, enumValue))
                        {
                            values.Add(enumValue);
                            index++;
                        }
                    }
                }
            }

			return values;
		}

		/// <summary>
		/// Gets information indicating whether a given data is valid
		/// </summary>
		/// <param name="val">The given data view name</param>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
		/// <returns>true if the value is valid, false, otherwise.</returns>
		public bool IsValueValid(string val, SchemaModelElement context)
		{
			return true;
		}

        private bool IsEnumValueExists(EnumValueCollection values, EnumValue enumValue)
        {
            bool status = false;

            foreach (EnumValue existEnumValue in values)
            {
                if (existEnumValue.Value == enumValue.Value ||
                    existEnumValue.DisplayText == enumValue.DisplayText)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}