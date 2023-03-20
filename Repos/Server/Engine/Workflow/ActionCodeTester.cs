/*
* @(#)ActionCodeTester.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Engine.Workflow
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Text;
    using System.Workflow.Runtime;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.Engine.Cache;
    using Newtera.Server.DB;

    /// <summary>
    /// Implementation of the <see cref="Newtera.Common.Wrapper.IInstanceWrapper"/>
    /// interface for test run of an action code
    /// </summary>
    public class ActionCodeTester : IInstanceWrapper
    {
        #region Fields

        /// <summary>The wrapped object.</summary>
        private InstanceView _instanceView;

        #endregion

        /// <summary> 
        /// Creates a new instance of the <see cref="Newtera.Common.Wrapper.ActionCodeTester"/> class.
        /// </summary>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The instance class name</param>
        public ActionCodeTester(string schemaId, string instanceClassName)
        {
            string[] strings = schemaId.Split(' ');
            string schemaName = strings[0].Trim();
            string schemaVersion = strings[1].Trim();

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;
            schemaInfo.Version = schemaVersion;

            MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(schemaInfo, dataProvider);
            DataViewModel dataView = metaData.GetDetailedDataView(instanceClassName);

            _instanceView = new InstanceView(dataView);
        }

        /// <summary>
        /// The object wrapped by this <see cref="Newtera.Common.Wrapper.ActionCodeTester"/>.
        /// </summary>
        public InstanceView WrappedInstance
        {
            get { return _instanceView; }
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
                return _instanceView.InstanceData.ObjId;
            }
        }

        /// <summary>
        /// Get the name of the bottom class that owns the instance
        /// </summary>
        public string OwnerClassName
        {
            get
            {
                return _instanceView.InstanceData.OwnerClassName;
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
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
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
                    return false; // default
                }
            }
            else
            {
                return false; // default
            }
        }

        /// <summary>Get date and time value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The date and time value of the property.</returns>
        public DateTime GetDateTime(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return DateTime.Parse(val);
                }
                catch (Exception )
                {
                    return DateTime.MinValue; // default
                }
            }
            else
            {
                return DateTime.MinValue; // default
            }
        }

        /// <summary>Get decimal value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The decimal value of the property.</returns>
        public decimal GetDecimal(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return decimal.Parse(val);
                }
                catch (Exception )
                {
                    return decimal.Zero; // default
                }
            }
            else
            {
                return decimal.Zero; // default
            }
        }

        /// <summary>Get double value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The double value of the property.</returns>
        public double GetDouble(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return double.Parse(val);
                }
                catch (Exception)
                {
                    return 0; // deafult
                }
            }
            else
            {
                return 0; // deafult
            }
        }

        /// <summary>Get float value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The float value of the property.</returns>
        public float GetFloat(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return float.Parse(val);
                }
                catch (Exception)
                {
                    return 0; // deafult
                }
            }
            else
            {
                return 0; // deafult
            }
        }

        /// <summary>Gets the 16-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The the 16-bit signed integer value of the property.</returns>
        public short GetInt16(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return short.Parse(val);
                }
                catch (Exception )
                {
                    return 0; // deafult
                }
            }
            else
            {
                return 0; // deafult
            }
        }

        /// <summary>Gets the 32-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 32-bit signed integer value of the property.</returns>
        public int GetInt32(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return int.Parse(val);
                }
                catch (Exception )
                {
                    return 0; // deafult
                }
            }
            else
            {
                return 0; // deafult
            }
        }

        /// <summary>Gets the 64-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 64-bit signed integer value of the property.</returns>
        public long GetInt64(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                try
                {
                    string val = _instanceView.InstanceData.GetAttributeValue(attributeName).ToString();
                    return long.Parse(val);
                }
                catch (Exception)
                {
                    return 0; // deafult
                }
            }
            else
            {
                return 0; // deafult
            }
        }

        /// <summary>Gets the string value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The string value of the property.</returns>
        public string GetString(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);
            if (!IsAttributeNull(attributeName))
            {
                string val = _instanceView.InstanceData.GetAttributeStringValue(attributeName);
                return val;
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
            string attributeName = GetAttributeName(theProperty);

            InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor)this.PropertyDescriptors[attributeName];
            object val = null;
            if (_instanceView.InstanceData != null)
            {
                val = _instanceView.InstanceData.GetAttributeValue(attributeName); 

                if (val != null && ipd.PropertyType.IsEnum)
                {
                    // we need to convert enum value into string values so that they do not depend on
                    // the enum types that are dynamically generated.
                    // dynamically generated enum types can not be deserailized when used in workflow
                    // instance loading
                    if (ipd.IsMultipleChoice)
                    {
                        // it's an array of enum values, convert it to an array of string
                        object[] enumArray = (object[])val;
                        string[] stringArray = new string[enumArray.Length];
                        for (int i = 0; i < enumArray.Length; i++)
                        {
                            if (enumArray[i] != null)
                            {
                                stringArray[i] = enumArray[i].ToString();
                            }
                            else
                            {
                                stringArray[i] = null;
                            }
                        }

                        val = stringArray;
                    }
                    else
                    {
                        val = val.ToString(); // it's a signle enum value, simply convert it to string
                    }
                }
            }

            return val;
        }

        /// <summary>Gets the information indicating whether the value of the specified property is null.</summary>
        /// <param name="attributeName">
        /// The name of the property to get the value of..
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        public bool IsNull(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);

            return IsAttributeNull(attributeName);
        }

        /// <summary>
        /// Gets the information indicating whether the value of the specified property is changed.</summary>
        /// <param name="theProperty">
        /// The name of the property to check
        /// </param>
        /// <returns>true if it is changed, false otherwise.</returns>
        public bool IsValueChanged(string theProperty)
        {
            string attributeName = GetAttributeName(theProperty);

            return _instanceView.InstanceData.IsValueChanged(attributeName);
        }

        /// <summary>Gets the information indicating whether the value of the specified property contains a value.</summary>
        /// <param name="theProperty">The name of the property to get the value of. May be nested.</param>
        /// <param name="propertyValue">The specified value</param>
        /// <returns>true if it contains the value, false otherwise.</returns>
        /// <remarks>This method is valid for the property whose value is a collection, such as array or multiple choice enum</remarks>
        public bool Contains(string theProperty, object propertyValue)
        {
            string attributeName = GetAttributeName(theProperty);

            return DoesAttributeContains(attributeName, propertyValue);
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
            string attributeName = GetAttributeName(theProperty);

            return (InstanceAttributePropertyDescriptor) _instanceView.GetProperties(null)[attributeName];
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
            try
            {
                string attributeName = GetAttributeName(theProperty);

                string stringVal = null;
                if (propertyValue != null)
                {
                    stringVal = propertyValue.ToString();
                }

                _instanceView.InstanceData.SetAttributeStringValue(attributeName, stringVal);

            }
            catch (Exception ex)
            {
                Exception ex1 = GetException("SetValue method failed with property '" + theProperty + "' and value '" + propertyValue.ToString() + "' because of " + ex.Message);
                //throw ex1;
            }
        }

        /// <summary>
        /// Save the instance to the database
        /// </summary>
        public void Save()
        {
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
        /// Gets the corresponding attribute name to the property name which is
        /// the attribute display name
        /// </summary>
        /// <param name="theProperty">The display name of the property</param>
        /// <returns>The attribute name</returns>
        /// <exception cref="Newtera.Common.Wrapper.WrapperException">thrown when unable to find the attriute of the specified property.</exception>
        private string GetAttributeName(string theProperty)
        {
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
                    Exception ex = GetException("Unable to find the property with a name or display name as " + theProperty + " in class " + _instanceView.DataView.BaseClass.Caption);
                    throw ex;
                }
                else
                {
                    attributeName = theProperty;
                }
            }

            return attributeName;
        }

        /// <summary>Gets the information indicating whether the value of the specified attribute is null.</summary>
        /// <param name="attributeName">
        /// The name of the attribute to get the value of..
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        public bool IsAttributeNull(string attributeName)
        {
            if (_instanceView.InstanceData.GetAttributeValue(attributeName) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets the information indicating whether the value of the specified attribute is null.</summary>
        /// <param name="attributeName">
        /// The name of the attribute to get the value of..
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        private bool DoesAttributeContains(string attributeName, object attributeValue)
        {
            bool status = false;
            InstanceAttributePropertyDescriptor ipd = this.PropertyDescriptors[attributeName] as InstanceAttributePropertyDescriptor;
            if (ipd != null && (ipd.IsMultipleChoice || ipd.IsArray))
            {
                if (ipd.IsMultipleChoice)
                {
                    string stringVal = _instanceView.InstanceData.GetAttributeStringValue(attributeName); 
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
                    string stringVal = _instanceView.InstanceData.GetAttributeStringValue(attributeName);
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

        /// <summary>
        /// Set the exception to the workflow instance before throwing, so that
        /// we can display the inner error message at error log
        /// </summary>
        /// <param name="reason">error message</param>
        /// <returns>Exception object</returns>
        private Exception GetException(string reason)
        {
            Exception ex = new Exception(reason);

            return ex;
        }
    }
}