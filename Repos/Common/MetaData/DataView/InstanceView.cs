/*
* @(#)InstanceView.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Data;
	using System.ComponentModel;
    using System.Collections;
    using System.Text.RegularExpressions;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a view for a single class instance. This is mainly for
	/// rendering an instance data in an editing UI form.
	/// </summary>
	/// <version>1.0.1 09 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class InstanceView : ICustomTypeDescriptor
	{
		private PropertyDescriptorCollection _propertyDescriptors;
		private DataViewModel _dataView;
		private DataSet _dataSet;
		private int _selectedIndex;
		private InstanceData _instanceData;
		private bool _showRelationshipAttributes;

		/// <summary>
		/// Initiating an instance of InstanceView class
		/// </summary>
		public InstanceView(DataViewModel dataView) : this(dataView, null, true, true)
		{
		}

		/// <summary>
		/// Initiating an instance of InstanceView class
		/// </summary>
		public InstanceView(DataViewModel dataView, DataSet dataSet) : this(dataView, dataSet, true, true)
		{
		}

        /// <summary>
        /// Initiating an instance of InstanceView class
        /// </summary>
        /// <param name="dataView">The data view model for the instance view</param>
        /// <param name="dataSet">The data set contains instance data</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        public InstanceView(DataViewModel dataView, DataSet dataSet, bool lazyCopy)
            : this(dataView, dataSet, lazyCopy, true)
        {
        }

		/// <summary>
		/// Initiating an instance of InstanceView class
		/// </summary>
		/// <param name="dataView">The data view for the instance</param>
		/// <param name="dataSet">The data set contains data for the instance</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        /// <param name="copyValue">Indicate whether to copy value from dataset</param>
		public InstanceView(DataViewModel dataView, DataSet dataSet, bool lazyCopy, bool copyValue)
		{
			_dataView = dataView;
			_dataSet = dataSet;
			_selectedIndex = 0;
            _instanceData = new InstanceData(_dataView, _dataSet, _selectedIndex, lazyCopy, copyValue);
			_showRelationshipAttributes = true;
			_propertyDescriptors = null;
		}

        /// <summary>
        /// Parse a setting string which is form of name1=value1;name2=value2
        /// </summary>
        /// <param name="settingString">A setting string</param>
        /// <returns>A hashtable with name and value entries</returns>
        public static Hashtable ParseSettingString(string settingString)
        {
            Hashtable properties = new Hashtable();

            if (!string.IsNullOrEmpty(settingString))
            {
                // Compile regular expression to find "name = value" pairs
                Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

                MatchCollection matches = regex.Matches(settingString);
                foreach (Match match in matches)
                {
                    int pos = match.Value.IndexOf("=");
                    string key = match.Value.Substring(0, pos).TrimEnd();
                    string val = match.Value.Substring(pos + 1).TrimStart();
                    properties[key] = val;
                }
            }

            return properties;
        }

		/// <summary>
		/// Gets the data view.
		/// </summary>
		/// <value>A DataViewModel object</value>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
		}

		/// <summary>
		/// Gets or sets the data set that contains the instance data.
		/// </summary>
		/// <value>The DataSet</value>
		public DataSet DataSet
		{
			get
			{
				return _dataSet;
			}
			set
			{
				_dataSet = value;
				// Reset the SelectedIndex
				SelectedIndex = 0;
				_instanceData.DataSet = value;
			}
		}

        public int InstanceCount
        {
            get
            {
                int count = 0;

                if (_dataSet != null &&
                    !DataSetHelper.IsEmptyDataSet(_dataSet, _dataView.BaseClass.Name))
                {
                    count = DataSetHelper.GetRowCount(_dataSet, _dataView.BaseClass.Name);
                }

                return count;
            }
        }

		/// <summary>
		/// Gets the InstanceData of the instance view.
		/// </summary>
		public InstanceData InstanceData
		{
			get
			{
				return _instanceData;
			}
		}

		/// <summary>
		/// Gets or sets the index of a selected instance in a data set.
		/// </summary>
		/// <value>Index integer value</value>
		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				// validate the index
				if (value >= 0 && value < DataSetHelper.GetRowCount(_dataSet, _dataView.BaseClass.ClassName))
				{

					_selectedIndex = value;
					_instanceData.RowIndex = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether to show relationship attributes
		/// in the view.
		/// </summary>
		public bool ShowRelationshipAttributes
		{
			get
			{
				return 	_showRelationshipAttributes;
			}
			set
			{
				_showRelationshipAttributes = value;
			}
		}

		/// <summary>
		/// Gets the value indicating whether the data of an instance has been changed.
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		public bool IsDataChanged
		{
			get
			{
				return _instanceData.IsChanged;
			}
		}

		/// <summary>
		/// Create a PropertyDescriptorCollection base on the result attributes
		/// in the data view.
		/// </summary>
		/// <returns>a PropertyDescriptorCollection</returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			AttributeElementBase schemaModelElement = null;
			PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(null);
			ClassElement classElement = _dataView.SchemaModel.FindClass(_dataView.BaseClass.ClassName);
			InstanceAttributePropertyDescriptor pd;
			bool isAdd = true;

			foreach (IDataViewElement resultAttribute in _dataView.ResultAttributes)
			{
                switch (resultAttribute.ElementType)
				{
					case ElementType.SimpleAttribute:
						isAdd = true;
						schemaModelElement = classElement.FindInheritedSimpleAttribute(resultAttribute.Name);
						break;
					case ElementType.ArrayAttribute:
						isAdd = true;
						schemaModelElement = classElement.FindInheritedArrayAttribute(resultAttribute.Name);
						break;
                    case ElementType.VirtualAttribute:
                        isAdd = true;
                        schemaModelElement = classElement.FindInheritedVirtualAttribute(resultAttribute.Name);
                        break;
                    case ElementType.ImageAttribute:
                        isAdd = true;
                        schemaModelElement = classElement.FindInheritedImageAttribute(resultAttribute.Name);
                        break;
					case ElementType.RelationshipAttribute:
                        isAdd = false;
						if (_showRelationshipAttributes)
						{
							// instance view only show those relationship attributes
							// that have foreign keys associated with them in database.
                            schemaModelElement = classElement.FindInheritedRelationshipAttribute(resultAttribute.Name) as RelationshipAttributeElement;
                            RelationshipAttributeElement relationshipAttribute = schemaModelElement as RelationshipAttributeElement;
                            if (relationshipAttribute != null)
                            {
                                if (relationshipAttribute.IsForeignKeyRequired)
                                {
                                    isAdd = true;
                                }
                                else if (relationshipAttribute.Usage == DefaultViewUsage.Included)
                                {
                                    // lined class is an junction class for a many-to-many relationship
                                    // show the relationship as one of the instance attributes
                                    isAdd = true;
                                }
                            }
						}

						break;
				}

				if (isAdd)
				{
                    if (schemaModelElement == null)
                    {
                        // 2017-11-26
                        // the result attribute may belong to a related class, get it from calling GetSchemaModelElement
                        schemaModelElement = resultAttribute.GetSchemaModelElement() as AttributeElementBase;
                    }

                    pd = new InstanceAttributePropertyDescriptor(resultAttribute.Name,
						null, resultAttribute, schemaModelElement, _instanceData, resultAttribute.IsReadOnly);

                    // Hack dataview use description to specify the appearance of column such as size=10;read=true
                    try
                    {
                        if (!string.IsNullOrEmpty(resultAttribute.Description))
                        {
                            Hashtable settings = InstanceView.ParseSettingString(resultAttribute.Description);
                            string str = (string) settings["size"];
                            if (str != null)
                            {
                                int size = int.Parse(str);
                                pd.Size = size + "";
                            }

                            str = (string)settings["readonly"];
                            if (str != null && str == "true")
                            {
                                pd.SetReadOnlyStatus(true);
                            }
                            else
                            {
                                pd.SetReadOnlyStatus(false);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

					propertyDescriptors.Add(pd);
				}
			}

			return propertyDescriptors;
		}

		#region ICustomTypeDescriptor Members

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public TypeConverter GetConverter()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public string GetComponentName()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _instanceData;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			// return an empty attribute collection
			return new AttributeCollection(null);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (_propertyDescriptors == null)
			{
				_propertyDescriptors = GetPropertyDescriptors();
			}

			return _propertyDescriptors;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			if (_propertyDescriptors == null)
			{
				_propertyDescriptors = GetPropertyDescriptors();
			}

			return _propertyDescriptors;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns></returns>
		public object GetEditor(Type editorBaseType)
		{
			// no editor for this object, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetDefaultProperty()
		{
			// no default property, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public EventDescriptor GetDefaultEvent()
		{
			// no default event, return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public string GetClassName()
		{
			return "InstanceView";
		}

		#endregion
	}
}