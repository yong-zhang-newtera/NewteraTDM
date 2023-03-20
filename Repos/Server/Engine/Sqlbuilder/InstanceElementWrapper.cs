/*
* @(#)InstanceElementWrapper.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Engine.Sqlbuilder
{
    using System;
    using System.Xml;
    using System.Collections;
    using System.ComponentModel;
    using System.Text;

    using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

    /// <summary>
    /// A implementation of IIstanceWrapper for an XML element that represents an instance in database.
    /// </summary>
    public class InstanceElementWrapper : IInstanceWrapper
    {
        MetaDataModel _metaData;
        private XmlElement _instanceElement;
        private InstanceView _instanceView;
        private XmlDocument _doc;
        private Hashtable _instanceViewTable;

        /// <summary> 
        /// Creates a new instance of the InstanceElementWrapper class.
        /// </summary>
        /// <param name="instanceElement">
        /// The xml element that represents an instance.
        /// </param>
        /// <param name="instanceView">The View for the instance</param>
        public InstanceElementWrapper(MetaDataModel metaData, XmlDocument doc, XmlElement instanceElement, InstanceView instanceView)
        {
            _metaData = metaData;
            _doc = doc;
            _instanceElement = instanceElement;
            _instanceView = instanceView;
            _instanceViewTable = new Hashtable();
        }

        /// <summary>
        /// The object wrapped by this <see cref="Newtera.Common.Wrapper.InstanceElementWrapper"/>.
        /// </summary>
        public InstanceView WrappedInstance
        {
            get 
            {
                return _instanceView;
            }
        }

        /// <summary>
        /// Return the collection of property descriptors.
        /// </summary>
        /// <returns>A collection of InstanceAttributePropertyDescriptor </returns>
        public PropertyDescriptorCollection PropertyDescriptors
        {
            get
            {
                return _instanceView.GetProperties(null);
            }
        }

        /// <summary>
        /// Get system generated obj id of the instance
        /// </summary>
        public string ObjId
        {
            get
            {
                return _instanceElement.GetAttribute(SQLElement.OBJ_ID);
            }
        }

        /// <summary>
        /// Get the name of the bottom class that owns the instance
        /// </summary>
        public string OwnerClassName
        {
            get
            {
                return _instanceElement.GetAttribute(XMLSchemaInstanceNameSpace.TYPE, XMLSchemaInstanceNameSpace.URI);
            }
        }

        /// <summary>
        /// Get the id of the database schema that owns the instance
        /// </summary>
        public string SchemaId
        {
            get
            {
                return _instanceView.DataView.SchemaModel.MetaData.SchemaInfo.NameAndVersion;
            }
        }

        /// <summary>Get boolean value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The boolean value of the property.</returns>
        public bool GetBoolean(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                string val = attributeElement.InnerText;
                if (LocaleInfo.Instance.IsTrue(val))
                {
                    return true;
                }
                else if (LocaleInfo.Instance.IsFalse(val))
                {
                    return false;
                }
                else
                {
                    throw new WrapperException("GetBoolean method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is an boolean value");
                }
            }
            else
            {
                throw new WrapperException("GetBoolean method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Get date and time value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The date and time value of the property.</returns>
        public DateTime GetDateTime(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return DateTime.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetDateTime method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetDateTime method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Get decimal value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The decimal value of the property.</returns>
        public decimal GetDecimal(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return decimal.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetDecimal method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetDecimal method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Get double value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The double value of the property.</returns>
        public double GetDouble(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return double.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetDouble method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetDouble method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Get float value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The float value of the property.</returns>
        public float GetFloat(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return float.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetFloat method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetFloat method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Gets the 16-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The the 16-bit signed integer value of the property.</returns>
        public short GetInt16(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return short.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetInt16 method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetInt16 method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Gets the 32-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 32-bit signed integer value of the property.</returns>
        public int GetInt32(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return int.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetInt32 method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetInt32 method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Gets the 64-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 64-bit signed integer value of the property.</returns>
        public long GetInt64(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            if (!IsAttributeNull(attributeElement))
            {
                try
                {
                    string val = attributeElement.InnerText;
                    return long.Parse(val);
                }
                catch (Exception ex)
                {
                    throw new WrapperException("GetInt64 method failed for value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " because " + ex.Message);
                }
            }
            else
            {
                throw new WrapperException("GetInt64 method requires that value of the property " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + " is not null");
            }
        }

        /// <summary>Gets the string value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The string value of the property.</returns>
        public string GetString(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);
            if (!IsAttributeNull(attributeElement))
            {
                return attributeElement.InnerText;
            }
            else
            {
                return "";
            }
        }

        /// <summary>Get the object value of a property.</summary>
        /// <param name="theProperty">
        /// The name of the property to get the value of. May be nested.
        /// </param>
        /// <returns>The object value of the property.</returns>
        public object GetValue(string theProperty)
        {
            object val = null;
            XmlElement attributeElement = null;
            string attributeName = null;
            bool isRelationship = false;
            foreach (InstanceAttributePropertyDescriptor pd in this.PropertyDescriptors)
            {
                if (pd.DisplayName == theProperty ||
                    pd.Name == theProperty)
                {
                    attributeName = pd.Name;

                    if (pd.IsRelationship)
                    {
                        isRelationship = true;
                    }

                    break;
                }
            }

            if (attributeName == null)
            {
                throw new WarningException("Unable to find the property with a name or display name as " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + ". Please check your code.");
            }

            if (!isRelationship)
            {
                attributeElement = _instanceElement[attributeName];
                if (attributeElement != null)
                {
                    val = attributeElement.InnerText;
                }
            }
            else
            {
                // return an IInstanceWrapper object represents the referenced instance
                XmlAttribute xmlAttribute = _instanceElement.Attributes[attributeName];
                if (xmlAttribute != null)
                {
                    string objId = xmlAttribute.Value;
                    XmlElement foundElement = null;
                    
                    // root element consists of class elements, and each class element consists of
                    // instance element
                    foreach (XmlElement classElement in _doc.DocumentElement.ChildNodes)
                    {
                        foreach (XmlElement instanceElement in classElement.ChildNodes)
                        {
                            xmlAttribute = instanceElement.Attributes[NewteraNameSpace.OBJ_ID];
                            if (xmlAttribute != null && xmlAttribute.Value == objId)
                            {
                                foundElement = instanceElement;
                                break;
                            }
                        }

                        if (foundElement != null)
                        {
                            break;
                        }
                    }

                    if (foundElement != null)
                    {
                        InstanceView instanceView = (InstanceView)_instanceViewTable[foundElement.Name];
                        if (instanceView == null)
                        {
                            DataViewModel dataView = _metaData.GetDetailedDataView(foundElement.Name);
                            instanceView = new InstanceView(dataView);
                            // keep the instanceView in a hashtable for sake of performance
                            _instanceViewTable[foundElement.Name] = instanceView;
                        }

                        val = new InstanceElementWrapper(_metaData, _doc, foundElement, instanceView);
                    }
                }
            }

            return val;
        }

        /// <summary>Gets the information indicating whether the value of the specified property is null.</summary>
        /// <param name="theProperty">
        /// The name of the property to get the value of. May be nested.
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        public bool IsNull(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            return IsAttributeNull(attributeElement);
        }

        /// <summary>
        /// Gets the information indicating whether the value of the specified property is changed.</summary>
        /// <param name="theProperty">
        /// The name of the property to check
        /// </param>
        /// <returns>true if it is changed, false otherwise.</returns>
        public bool IsValueChanged(string theProperty)
        {
            return false; // no old value to compare
        }

        /// <summary>Gets the information indicating whether the value of the specified property contains a value.</summary>
        /// <param name="theProperty">The name of the property to get the value of. May be nested.</param>
        /// <param name="propertyValue">The specified value</param>
        /// <returns>true if it contains the value, false otherwise.</returns>
        /// <remarks>This method is valid for the property whose value is a collection, such as array or multiple choice enum</remarks>
        public bool Contains(string theProperty, object propertyValue)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            return DoesAttributeContains(attributeElement, propertyValue);
        }

        /// <summary>
        /// Get the <see cref="System.Reflection.PropertyInfo"/> for a particular
        /// property.
        /// </summary>
        /// <param name="theProperty">
        /// The property to be retrieved.
        /// </param>
        /// <returns>
        /// The <see cref="Newtera.Common.MetaData.DataView.InstanceAttributePropertyDescriptor"/> for the particular
        /// property.
        /// </returns>
        public InstanceAttributePropertyDescriptor GetPropertyDescriptor(string theProperty)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);

            return (InstanceAttributePropertyDescriptor)_instanceView.GetProperties(null)[attributeElement.Name];
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="theProperty">
        /// The name of the property to set value of.
        /// </param>
        /// <param name="propertyValue">The new property value.</param>
        public void SetValue(string theProperty, object propertyValue)
        {
            XmlElement attributeElement = GetAttributeElement(theProperty);
            if (propertyValue != null)
            {
                attributeElement.InnerText = propertyValue.ToString();
            }
            else
            {
                attributeElement.InnerText = "";
            }
        }

        /// <summary>
        /// Save the instance to the database
        /// </summary>
        public void Save()
        {
            // do nothing here
        }

        /// <summary>
        /// Copy the instance values from a source instance
        /// </summary>
        public void Copy(IInstanceWrapper srcInstance)
        {
            if (this.WrappedInstance != null &&
                srcInstance.WrappedInstance != null)
            {
                this.WrappedInstance.InstanceData.Copy(srcInstance.WrappedInstance.InstanceData);
            }
        }

        /// <summary>
        /// Gets the corresponding xml element from the xml instance element for the property name which is
        /// the attribute display name
        /// </summary>
        /// <param name="theProperty">The display name of the property</param>
        /// <returns>The xml element of the property.</returns>
        /// <exception cref="Newtera.Common.Wrapper.WrapperException">thrown when unable to find the attriute element of the specified property.</exception>
        private XmlElement GetAttributeElement(string theProperty)
        {
            XmlElement attributeElement = null;
            string attributeName = null;
            foreach (InstanceAttributePropertyDescriptor pd in this.PropertyDescriptors)
            {
                if (pd.DisplayName == theProperty)
                {
                    attributeName = pd.Name;
                    break;
                }
            }

            if (attributeName == null)
            {
                if (this.PropertyDescriptors[theProperty] == null)
                {
                    throw new WarningException("Unable to find the property with a name or display name as " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption + ". Please check your code.");
                }
                else
                {
                    attributeName = theProperty;
                }
            }

            attributeElement = _instanceElement[attributeName];

            if (attributeElement == null)
            {
                throw new WarningException("Unable to find a xml element representing the attribute with display name as " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption);
            }

            return attributeElement;
        }

        /// <summary>Gets the information indicating whether the value of an attribute element is null.</summary>
        /// <param name="attributeName">
        /// The xml attribute element.
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        public bool IsAttributeNull(XmlElement attributeElement)
        {
            if (string.IsNullOrEmpty(attributeElement.InnerText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets the information indicating whether the value of the specified attribute is null.</summary>
        /// <param name="attributeElement">
        /// The attribute element to get the value of..
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        private bool DoesAttributeContains(XmlElement attributeElement, object attributeValue)
        {
            bool status = false;
            InstanceAttributePropertyDescriptor ipd = this.PropertyDescriptors[attributeElement.Name] as InstanceAttributePropertyDescriptor;
            if (ipd != null && (ipd.IsMultipleChoice || ipd.IsArray))
            {
                if (ipd.IsMultipleChoice)
                {
                    string stringVal = attributeElement.InnerText;
                    if (!string.IsNullOrEmpty(stringVal))
                    {
                        string[] stringArray = stringVal.Split(';');
                        for (int i = 0; i < stringArray.Length; i++)
                        {
                            if (stringArray[i] == (string)attributeValue)
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
                else if (ipd.IsArray)
                {
                    string stringVal = attributeElement.InnerText;
                    if (!string.IsNullOrEmpty(stringVal))
                    {
                        string[] stringArray = stringVal.Split(';');
                        for (int i = 0; i < stringArray.Length; i++)
                        {
                            if (stringArray[i] == (string)attributeValue)
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
    }
}