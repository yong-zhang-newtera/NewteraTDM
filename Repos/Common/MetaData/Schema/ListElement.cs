/*
* @(#)ListElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
    using System.Web;
	using System.Runtime.Remoting;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Text;
	using System.Collections;
    using System.Threading;
	using System.Collections.Specialized;
	using System.Xml.Schema;
    using System.Security;

    using Newtera.Common.MetaData.Principal;
	using Newtera.Common.Core;
	
	/// <summary>
	/// The ListElement represents a list of values obtained from a customized
	/// handler. 
	/// </summary>
	/// <version>  	1.0.1 05 Apr 2004
	/// </version>
	/// <author>  		Yong Zhang</author>
    public class ListElement : ConstraintElementBase, IEnumConstraint
	{
        static public Hashtable ListFilterTable = new Hashtable();

        /// <summary>
        /// Constant definition for enum value SEPARATOR
        /// </summary>
        public const string SEPARATOR = ";";

		private string _handlerName = null;
        private string _parameter = null;
        private string _valueField = null;
        private string _textField = null;
        private ListStyle _listStyle = ListStyle.Static;
        private string _conditionalQuery = null;
        private string _nonconditionalQuery = null;
		private IListHandler _handler = null; // created for run-time, do not save to schema
       
		/// <summary>
		/// Get info if the xmlSchemaElemet represents a List constraint
		/// </summary>
		/// <param name="xmlSchemaElement">the XmlSchemaAnnotated object
		/// </param>
		/// <returns>
		/// return true if the element represents a List constraint, 
		/// otherwise, false.
		/// </returns>
		static public bool isList(XmlSchemaAnnotated xmlSchemaElement)
		{
			bool status = false;
			
			if (xmlSchemaElement is XmlSchemaSimpleType)
			{
				XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) xmlSchemaElement).Content;
				
				if (restriction != null)
				{
					if (restriction.Facets.Count == 1 &&
						restriction.Facets[0] is XmlSchemaEnumerationFacet &&
						((XmlSchemaEnumerationFacet) restriction.Facets[0]).Value == "DynamicValues")
					{
						status = true;
					}
				}
			}
			
			return status;
		}

        /// <summary>
        /// Compare the values of two lists
        /// </summary>
        /// <param name="firstList"></param>
        /// <param name="secondList"></param>
        /// <returns>return true if the two lists are the same, false if different</returns>
        static public bool Compare(ListElement firstList, ListElement secondList)
        {
            bool result = true;

            if (firstList.ListHandlerName != secondList.ListHandlerName ||
                firstList.Parameter != secondList.Parameter ||
                firstList.ValueField != secondList.ValueField ||
                firstList.TextField != secondList.TextField ||
                firstList.NonConditionalQuery != secondList.NonConditionalQuery ||
                firstList.ConditionalQuery != secondList.ConditionalQuery)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Create a key to identify a filter value associated with a list constraint bound
        /// attribute
        /// </summary>
        /// <param name="schemaId">Schema Id</param>
        /// <param name="className">The name of owner class</param>
        /// <param name="attributeName">The name of attribute bound to a list constraint</param>
        /// <returns>A key</returns>
        public static string CreateListFilterKey(string schemaId, string className, string attributeName)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Filder_").Append(schemaId).Append("_").Append(className).Append("_").Append(attributeName);

            return builder.ToString();
        }

		/// <summary>
		/// Initializing an ListElement object
		/// </summary>
		/// <param name="name">Name of element</param>
		public ListElement(string name): base(name)
		{
			DataType = DataType.String; // default type for List
		}

		/// <summary>
		/// Initializing an ListElement object
		/// </summary>
		/// <param name="xmlSchemaElement">The XmlSchemaAnnotated object</param>
		internal ListElement(XmlSchemaAnnotated xmlSchemaElement): base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets the list values.
		/// </summary>
		/// <value>
		/// An collection of EnumValue object
		/// </value>
		[BrowsableAttribute(false)]		
		public EnumValueCollection Values
		{
			get
			{
                throw new Exception("Values property is not supported for ListElement");
			}
		}

		/// <summary>
		/// Gets or sets the handler that retrieves a list of values dynamically.
		/// </summary>
		/// <value>
		/// A fully-qualified handler class name, including namespace and class name.
		/// for example, Newtera.Common.MetaData.DataView.GetDataViewNamesHandler
		/// </value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Gets or sets the name of a handler that retrieves a list of values. an example of a handler spec is: Newtera.Common.MetaData.DataViewListHandler, Newtera.Common"),
		DefaultValueAttribute(null)
		]		
		public string ListHandlerName
		{
			get
			{
				return _handlerName;
			}
			set
			{
				_handlerName = value;
			}
		}

        /// <summary>
        /// Gets or sets the parameter that is passed to the handler at the runtime.
        /// </summary>
        /// <value>
        /// A string value that is up to the hanlder to interpret
        /// </value>
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Gets or sets the parameter that is passed to the handler at the runtime"),
        DefaultValueAttribute(null)
        ]
        public string Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }

        /// <summary>
        /// Gets or sets a conditional query when the list style is set to Conditional.
        /// </summary>
        [
        CategoryAttribute("DataBinding"),
        DescriptionAttribute("Gets or sets a conditional query when the list style is set to Conditional."),
        DefaultValueAttribute(null)
        ]
        public string ConditionalQuery
        {
            get
            {
                return _conditionalQuery;
            }
            set
            {
                _conditionalQuery = value;
            }
        }

        /// <summary>
        /// Gets or sets a non-conditional query when the list style is set to Conditional or static.
        /// </summary>
        [
        CategoryAttribute("DataBinding"),
        DescriptionAttribute("Gets or sets a non-conditional query when the list style is set to Conditional or static."),
        DefaultValueAttribute(null)
        ]
        public string NonConditionalQuery
        {
            get
            {
                return _nonconditionalQuery;
            }
            set
            {
                _nonconditionalQuery = value;
            }
        }

        /// <summary>
        /// Gets or sets the name indicates which field in the retrieved data is used as display text of the enum.
        /// </summary>
        [
        CategoryAttribute("DataBinding"),
        DescriptionAttribute("Gets or sets the name indicates which field in the retrieved data is used as display text of the enum"),
        DefaultValueAttribute(null)
        ]
        public string TextField
        {
            get
            {
                return _textField;
            }
            set
            {
                _textField = value;
            }
        }

        /// <summary>
        /// Gets or sets the name indicates which field in the retrieved data is used as value of the enum.
        /// </summary>
        [
        CategoryAttribute("DataBinding"),
        DescriptionAttribute("Gets or sets the name indicates which field in the retrieved data is used as value of the enum"),
        DefaultValueAttribute(null)
        ]
        public string ValueField
        {
            get
            {
                return _valueField;
            }
            set
            {
                _valueField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicates how list values are generated
        /// </summary>
        [
         CategoryAttribute("DataBinding"),
         DescriptionAttribute("Gets or sets a value indicates how list values are generated"),
         DefaultValueAttribute(ListStyle.Static)
        ]
        public ListStyle ListStyle
        {
            get
            {
                return _listStyle;
            }
            set
            {
                _listStyle = value;
            }
        }

        /// <summary>
        /// Gets the information indicates whether the enum values of the constraint are conditional
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsConditionBased
        {
            get
            {
                return _listStyle == ListStyle.Conditional;
            }
        }

        /// <summary>
        /// Gets the information indicates whether the values of the constraint are user-based
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsUserBased
        {
            get
            {
                return _listStyle == ListStyle.UserBased;
            }
        }

        /// <summary>
        /// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitListElement(this);
		}

		/// <summary>
		/// Gets the information indicating whether the given value is valid
		/// based on the constraint.
		/// </summary>
		/// <param name="value">The given value</param>
		/// <returns>true if it is valid, false otherwise</returns>
		public override bool IsValueValid(string value)
		{
			return true; // do not perform validation for list
		}

        /// <summary>
        /// Create a list handler described by the handler name
        /// </summary>
        /// <returns></returns>
        public IListHandler CreateHandler()
        {
            IListHandler handler = null;

            if (_handlerName != null)
            {
                int index = _handlerName.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = _handlerName.Substring(0, index).Trim();
                    assemblyName = _handlerName.Substring(index + 1).Trim();
                }
                else
                {
                    className = _handlerName.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    handler = (IListHandler)obj.Unwrap();
                }
                catch
                {
                    handler = null;
                }
            }

            return handler;
        }

        /// <summary>
        /// Gets a conditional list of enum values
        /// </summary>
        /// <returns>A list of enum values</returns>
        public EnumValueCollection GetEnumValues(SchemaModelElement context)
        {
            if (_handler == null)
            {
                _handler = CreateHandler();
            }

            if (_handler != null)
            {
                string query = _nonconditionalQuery;
                string filterValue = null;

                Newtera.Common.MetaData.Principal.CustomPrincipal customPrincipal = Thread.CurrentPrincipal as Newtera.Common.MetaData.Principal.CustomPrincipal;

                // Currently conditional enum values are only available for web app
                // not valid for WinForm app
                if (context != null &&
                customPrincipal != null &&
                customPrincipal.IsServerSide &&
                context is SimpleAttributeElement)
                {
                    filterValue = ((SimpleAttributeElement)context).GetListFilterValue();

                    if (this.IsConditionBased && !string.IsNullOrEmpty(_conditionalQuery))
                    {
                        query = _conditionalQuery;
                    }
                }

                return _handler.GetValues(context, query, _parameter, filterValue, TextField, ValueField);
            }
            else
            {
                return new EnumValueCollection();
            }
        }


        /// <summary>
        /// Gets a non-conditional list of enum values
        /// </summary>
        /// <returns>a complete list of enum values</returns>
        public EnumValueCollection GetAllEnumValues(SchemaModelElement context)
        {
            if (_handler == null)
            {
                _handler = CreateHandler();
            }

            if (_handler != null &&
                context != null &&
                context is SimpleAttributeElement)
            {
                string query = _nonconditionalQuery;

                return _handler.GetValues(context, query, _parameter, null, TextField, ValueField);
            }
            else
            {
                return new EnumValueCollection();
            }
        }

        #region IEnumConstraint

        /// <summary>
        /// Convert an enum display text to its value.
        /// </summary>
        /// <param name="text">An enum display text.</param>
        /// <returns>The corresponding enum value</returns>
        public string GetValue(string text)
        {
            string val = "";
            EnumValueCollection enumValues = Values;
            if (enumValues != null)
            {
                foreach (EnumValue enumValue in enumValues)
                {
                    if (enumValue.DisplayText == text)
                    {
                        val = enumValue.Value;
                        break;
                    }
                }
            }

            return val;
        }

        /// <summary>
        /// Gets the corresponsing image name of an enum value
        /// </summary>
        /// <param name="enumValue">enum value</param>
        /// <returns>The display text, could be null</returns>
        public string GetImageName(string val)
        {
            string imageName = null;
            EnumValueCollection enumValues = Values;
            if (enumValues != null)
            {
                foreach (EnumValue enumValue in enumValues)
                {
                    if (enumValue.Value == val)
                    {
                        imageName = enumValue.ImageName;
                        break;
                    }
                }
            }

            return imageName;
        }

        #endregion

        /// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
			
			_handlerName = GetNewteraAttributeValue(NewteraNameSpace.LIST_HANDLER);

            string str = GetNewteraAttributeValue(NewteraNameSpace.LIST_PARAMETER);
            if (!string.IsNullOrEmpty(str))
            {
                _parameter = str;
            }

            str = GetNewteraAttributeValue(NewteraNameSpace.LIST_CONDITIONAL_QUERY);
            if (!string.IsNullOrEmpty(str))
            {
                _conditionalQuery = str;
            }

            str = GetNewteraAttributeValue(NewteraNameSpace.LIST_NONCONDITIONAL_QUERY);
            if (!string.IsNullOrEmpty(str))
            {
                _nonconditionalQuery = str;
            }

            str = GetNewteraAttributeValue(NewteraNameSpace.LIST_TEXT);
            if (!string.IsNullOrEmpty(str))
            {
                _textField = str;
            }

            str = GetNewteraAttributeValue(NewteraNameSpace.LIST_VALUE);
            if (!string.IsNullOrEmpty(str))
            {
                _valueField = str;
            }

            // list style member
            str = GetNewteraAttributeValue(NewteraNameSpace.LIST_STYLE);
            if (!string.IsNullOrEmpty(str))
            {
                _listStyle = (ListStyle)Enum.Parse(typeof(ListStyle), str);
            }
            else
            {
                _listStyle = ListStyle.Static;
            }
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <!--
		/// List constraint expressed in xml schema:
		/// 
		/// <xsd:simpleType name="PartNumber">
		/// <xsd:restriction base="xsd:string">
		/// <xsd:maxLength value="2"/>
		/// <xsd:enumeration value="DynamicValues"/>
		/// </xsd:restriction>
		/// </xsd:simpleType>
		/// -->
		internal override void Marshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) XmlSchemaElement).Content;

			XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "DynamicValues";
			restriction.Facets.Add(enumeration);

			// Write IsMultipleSelection member
			SetNewteraAttributeValue(NewteraNameSpace.LIST_HANDLER, _handlerName);

            if (!string.IsNullOrEmpty(_parameter))
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_PARAMETER, SecurityElement.Escape(_parameter));
            }

            if (!string.IsNullOrEmpty(_conditionalQuery))
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_CONDITIONAL_QUERY, SecurityElement.Escape(_conditionalQuery));
            }

            if (!string.IsNullOrEmpty(_nonconditionalQuery))
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_NONCONDITIONAL_QUERY, SecurityElement.Escape(_nonconditionalQuery));
            }

            if (!string.IsNullOrEmpty(_textField))
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_TEXT, _textField);
            }

            if (!string.IsNullOrEmpty(_valueField))
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_VALUE, _valueField);
            }

            // write list style enum value, Static is default value
            if (_listStyle != ListStyle.Static)
            {
                SetNewteraAttributeValue(NewteraNameSpace.LIST_STYLE, Enum.GetName(typeof(ListStyle), _listStyle));
            }

			base.Marshal();
		}
	}

    public enum ListStyle
    {
        Static, // List values are statis
        Conditional, // List values vary on condition
        UserBased // List values vary on user
    }
}