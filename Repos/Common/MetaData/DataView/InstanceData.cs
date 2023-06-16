/*
* @(#)InstanceData.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Data;
    using System.Xml;
    using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Represents a data of an instance of a data view.
	/// </summary>
	/// <version>1.0.1 09 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class InstanceData
	{
        private const char PRIMARY_KEY_SEPARATOR = '&';

		private DataViewModel _dataView;
		private DataSet _dataSet;
		private int _rowIndex;
		private bool _lazyCopy;
        private bool _copyValues;
        private string[] _readOnlyAttributes;
        private string _instanceId;

        /// <summary>
        /// Initiating an instance of InstanceData class
        /// </summary>
        /// <param name="dataView">The data view</param>
        /// <param name="dataSet">The data set that contains instance data</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        /// <remarks>If the dataset is null, then values of an instance data are all empty</remarks>
        public InstanceData(DataViewModel dataView, DataSet dataSet, bool lazyCopy)
            : this(dataView, dataSet, 0, lazyCopy, true)
        {
        }

        /// <summary>
        /// Initiating an instance of InstanceData class
        /// </summary>
        /// <param name="dataView">The data view</param>
        /// <param name="dataSet">The data set that contains instance data</param>
        /// <param name="rowIndex">The index of dataset row whose values are to be copied</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        /// <remarks>If the dataset is null, then values of an instance data are all empty</remarks>
        public InstanceData(DataViewModel dataView, DataSet dataSet, int rowIndex, bool lazyCopy)
            : this(dataView, dataSet, rowIndex, lazyCopy, true)
        {
        }

		/// <summary>
		/// Initiating an instance of InstanceData class
		/// </summary>
		/// <param name="dataView">The data view</param>
		/// <param name="dataSet">The data set that contains instance data</param>
        /// <param name="rowIndex">The index of the DataSet row whose values are to be copied</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        /// <param name="copyValues">Indicate whether to copy the values from dataset</param>
		/// <remarks>If the dataset is null, then values of an instance data are all empty</remarks>
		public InstanceData(DataViewModel dataView, DataSet dataSet, int rowIndex, bool lazyCopy, bool copyValues)
		{
			_dataView = dataView;
			_dataSet = dataSet;
            _rowIndex = rowIndex;
			_lazyCopy = lazyCopy;
            _copyValues = copyValues;
            _readOnlyAttributes = null;
            _instanceId = null;
            if (_copyValues)
            {
                CopyInstanceValues(_dataView, _dataSet, _rowIndex, _lazyCopy);
            }
		}

		/// <summary>
		/// Gets or sets the row index for the current instance in data set
		/// </summary>
		/// <value>The row index</value>
		public int RowIndex
		{
			get
			{
				return _rowIndex;
			}
			set
			{
				_rowIndex = value;
                _readOnlyAttributes = null;
                if (_copyValues)
                {
                    CopyInstanceValues(_dataView, _dataSet, _rowIndex, _lazyCopy);
                }
			}
		}

		/// <summary>
		/// Gets or sets the data set that contains data for an instance.
		/// </summary>
		public DataSet DataSet
		{
			get
			{
				return _dataSet;
			}
			set
			{
				_dataSet = value;
				RowIndex = 0;
			}
		}

		/// <summary>
		/// Gets the value indicating whether any values of instance attributes have been
		/// changed comparing to original values.
		/// </summary>
		/// <value>true if there are changed value(s), false otherwise</value>
		/// <remarks>This method call will also set a true flag to those attributes whose
		/// values have been changed</remarks>
		public bool IsChanged
		{
			get
			{
				bool isChanged = false;

				foreach (IDataViewElement attribute in _dataView.ResultAttributes)
				{
					if (IsAttributeValueChanged(attribute, _dataView, _dataSet, _rowIndex))
					{
						isChanged = true;

						// do not break out, we want to check each of attributes
						// so that its IsValueChanged flag gets set properly
					}
				}

				return isChanged;
			}
		}

        /// <summary>
        /// Gets the value indicating whether value of an attribute has been
        /// changed comparing to original values.
        /// </summary>
        /// <value>true if the value has been changed, false otherwise</value>
        /// <remarks>This method call will also set a true flag to the attribute whose
        /// value has been changed</remarks>
        public bool IsValueChanged(string attributeName)
        {
            bool isChanged = false;

            if (_dataSet != null)
            {
                IDataViewElement attribute = _dataView.ResultAttributes[attributeName];

                if (attribute != null)
                {
                    if (IsAttributeValueChanged(attribute, _dataView, _dataSet, _rowIndex))
                    {
                        isChanged = true;
                    }
                }
            }

            return isChanged;
        }

		/// <summary>
		/// Gets or sets the obj_id of the instance.
		/// </summary>
		/// <returns>A string representing obj_id</returns>
		public string ObjId
		{
			get
			{
                if (_dataSet == null)
                {
                    return _instanceId;
                }
                else
                {
                    return DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
                        NewteraNameSpace.OBJ_ID, _rowIndex);
                }
			}
			set
			{
                _instanceId = value; // keep id in case _dataSet is null which is true for a new instance

				DataSetHelper.SetCellValue(_dataSet, _dataView.BaseClass.ClassName,
					NewteraNameSpace.OBJ_ID, _rowIndex, value);
			}
		}

        /// <summary>
        /// Gets primary key values of the instance.
        /// </summary>
        /// <returns>A primary key values, separated by '&', null if the instance deosn't have primary keys</returns>
        public string PrimaryKeyValues
        {
            get
            {
                string pkValues = null;

                ClassElement classElement = _dataView.BaseClass.GetSchemaModelElement() as ClassElement;

                while (classElement != null)
                {
                    if (classElement.PrimaryKeys != null && classElement.PrimaryKeys.Count > 0)
                    {
                        foreach (SimpleAttributeElement pk in classElement.PrimaryKeys)
                        {
                            string pkValue = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
                                pk.Name, _rowIndex);
                            if (!string.IsNullOrEmpty(pkValue))
                            {
                                if (pkValues == null)
                                {
                                    pkValues = pkValue;
                                }
                                else
                                {
                                    pkValues = PRIMARY_KEY_SEPARATOR + pkValue;
                                }
                            }
                        }

                        break;
                    }
                    else
                    {
                        classElement = classElement.ParentClass;
                    }
                }

                return pkValues;
            }
        }

		/// <summary>
		/// Gets the owner class name of the instance.
		/// </summary>
		/// <returns>A string representing owner class name</returns>
		public string OwnerClassName
		{
			get
			{
				return DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
					NewteraNameSpace.TYPE, _rowIndex);
			}
		}

        /// <summary>
        /// Gets the information indicating whether the InstanceData is the result of duplicating
        /// another instance
        /// </summary>
        public bool IsDuplicated
        {
            get
            {
                return !_lazyCopy;
            }
        }

        /// <summary>
        /// Copy the values of an InstanceData object of the same class
        /// </summary>
        /// <param name="otherInstance">The instance to copy values from.</param>
        public void Copy(InstanceData otherInstance)
        {
            foreach (IDataViewElement resultAttribute in _dataView.ResultAttributes)
            {
                // copy the value
                if (resultAttribute is DataSimpleAttribute)
                {
					var simpleAttribute = (DataSimpleAttribute)resultAttribute;

					if (!simpleAttribute.IsHistoryEdit &&
						simpleAttribute.AllowManualUpdate)
                    {
						simpleAttribute.AttributeValue = otherInstance.GetAttributeStringValue(resultAttribute.Name);
                    }
                }
                else if (resultAttribute is DataVirtualAttribute)
                {
                }
                else if (resultAttribute is DataImageAttribute)
                {
                    ((DataImageAttribute)resultAttribute).AttributeValue = otherInstance.GetAttributeStringValue(resultAttribute.Name);
                }
                else if (resultAttribute is DataRelationshipAttribute)
                {
                    DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute)resultAttribute;

                    SetRelationshipAttributeValue(relationshipAttr, otherInstance.GetAttributeStringValue(resultAttribute.Name));
                }
                else if (resultAttribute is DataArrayAttribute)
                {
                    ((DataArrayAttribute)resultAttribute).AttributeValue = otherInstance.GetAttributeStringValue(resultAttribute.Name);
                }
            }
        }

		/// <summary>
		/// Gets the information indicating whether the user has permission to
		/// perform an action on this instance data.
		/// </summary>
		/// <param name="action">One of XaclActionType values</param>
		/// <returns>true if the user has permission to perform the action, false otherwise.</returns>
		public bool HasPermission(XaclActionType action)
		{	
			bool hasPermission = true;
			string permissionStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
				NewteraNameSpace.PERMISSION, _rowIndex);
			if (!string.IsNullOrEmpty(permissionStr))
			{
				XaclPermissionFlag flags = (XaclPermissionFlag) Enum.Parse(typeof(XaclPermissionFlag), permissionStr);
				switch (action)
				{
					case XaclActionType.Read:
						if ((flags & XaclPermissionFlag.GrantRead) == 0)
						{
							hasPermission = false;
						}
						break;
					case XaclActionType.Write:
						if ((flags & XaclPermissionFlag.GrantWrite) == 0)
						{
							hasPermission = false;
						}
						break;
					case XaclActionType.Create:
						if ((flags & XaclPermissionFlag.GrantCreate) == 0)
						{
							hasPermission = false;
						}
						break;
					case XaclActionType.Delete:
						if ((flags & XaclPermissionFlag.GrantDelete) == 0)
						{
							hasPermission = false;
						}
						break;
					case XaclActionType.Upload:
						if ((flags & XaclPermissionFlag.GrantUpload) == 0)
						{
							hasPermission = false;
						}
						break;
					case XaclActionType.Download:
						if ((flags & XaclPermissionFlag.GrantDownload) == 0)
						{
							hasPermission = false;
						}
						break;
				}
			}
			
			return hasPermission;
		}

        /// <summary>
        /// Gets the instance's permission string value
        /// </summary>
        /// <returns>Permission string</returns>
        public string GetPermissionValue()
        {
            string permissionStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
                NewteraNameSpace.PERMISSION, _rowIndex);

            return permissionStr;
        }

        /// <summary>
        /// Gets the information indicating whether the attribute is read-only
        /// </summary>
        /// <param name="attributeName">The name of attribute</param>
        /// <returns>true if it is read-only, false otherwise</returns>
        public bool IsAttributeReadOnly(string attributeName)
        {
            bool status = false;

            if (_readOnlyAttributes == null)
            {
                _readOnlyAttributes = GetReadOnlyAttributes();
            }

            foreach (string readOnlyAttribute in _readOnlyAttributes)
            {
                if (readOnlyAttribute == attributeName)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Gets the number of attachments associated with the instance.
		/// </summary>
		/// <returns></returns>
		public int GetANUM()
		{
			string anumStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
				NewteraNameSpace.ATTACHMENTS, _rowIndex);

			if (anumStr != null && anumStr.Length > 0)
			{
				return System.Convert.ToInt32(anumStr);
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Increment attachment number of the instance by one.
		/// </summary>
		public void IncreamentANUM()
		{	
			string anumStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
				NewteraNameSpace.ATTACHMENTS, _rowIndex);

			if (anumStr != null)
			{
				int anum = System.Convert.ToInt32(anumStr);
				anum++;
				DataSetHelper.SetCellValue(_dataSet,
					_dataView.BaseClass.ClassName, NewteraNameSpace.ATTACHMENTS,
					_rowIndex, anum.ToString());
			}
		}

		/// <summary>
		/// Decrement attachment number of the instance by one.
		/// </summary>
		public void DecreamentANUM()
		{	
			string anumStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName,
				NewteraNameSpace.ATTACHMENTS, _rowIndex);

			if (anumStr != null)
			{
				int anum = System.Convert.ToInt32(anumStr);
				if (anum > 0)
				{
					anum--;
					DataSetHelper.SetCellValue(_dataSet,
						_dataView.BaseClass.ClassName, NewteraNameSpace.ATTACHMENTS,
						_rowIndex, anum.ToString());
				}
			}
		}

		/// <summary>
		/// Gets the value of an attribute of an instance.
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <returns>Attribute value</returns>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>
		public object GetAttributeValue(string name)
		{
			object attributeValue = null;
			IDataViewElement instanceAttribute = GetInstanceAttribute(name);

			if (instanceAttribute != null)
			{
				switch (instanceAttribute.ElementType)
				{
					case ElementType.SimpleAttribute:
						SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) instanceAttribute.GetSchemaModelElement();
						if (schemaModelElement.DataType == DataType.Boolean)
						{
							string boolVal = ((DataSimpleAttribute) instanceAttribute).AttributeValue;
							// if boolVal is empty, replace it with localized "None" value
							if (boolVal == null || boolVal.Length == 0)
							{
								boolVal = LocaleInfo.Instance.None;
							}

							// get the corresponding Dynamic Generated BooleanEnum value
							Type booleanEnumType = EnumTypeFactory.Instance.Create(schemaModelElement);
							try
							{
								attributeValue = Enum.Parse(booleanEnumType, boolVal);
							}
							catch (Exception)
							{
								attributeValue = 0; // treat it as an unknow boolean value
							}
						}
						else if (schemaModelElement.DataType == DataType.Date ||
							schemaModelElement.DataType == DataType.DateTime)
						{
                            if (((DataSimpleAttribute)instanceAttribute).AttributeValue != null &&
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue.Length > 0)
							{
								attributeValue = DateTime.Parse(((DataSimpleAttribute) instanceAttribute).AttributeValue);
							}
							else
							{
								attributeValue = null;
							}
						}
						else if (schemaModelElement.Constraint != null &&
							schemaModelElement.Constraint is IEnumConstraint &&
							schemaModelElement.ConstraintUsage == ConstraintUsage.Restriction)
						{
							string val = ((DataSimpleAttribute) instanceAttribute).AttributeValue;
	
							// get the corresponding Dynamic Generated Enum Type value(s)
							if (schemaModelElement.IsMultipleChoice)
							{
                                object[] enumValues;

                                if (val != null && val.Length > 0)
                                {
                                    string[] values = ((EnumElement)schemaModelElement.Constraint).GetStringArray(val);
                                    enumValues = new object[values.Length];
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        enumValues[i] = Enum.Parse(EnumTypeFactory.Instance.Create(schemaModelElement), values[i]);
                                    }
                                }
                                else
                                {
                                    enumValues = new object[0];
                                }

								attributeValue = enumValues;
							}
							else
							{
                                // if value is empty, replace it with localized "None" value
                                if (val == null || val.Length == 0)
                                {
                                    val = LocaleInfo.Instance.None;
                                }

								try
								{
									attributeValue = Enum.Parse(EnumTypeFactory.Instance.Create(schemaModelElement), val);
								}
								catch (Exception)
								{
									// default is unknown
									//attributeValue = Enum.Parse(EnumTypeFactory.Instance.Create(schemaModelElement), LocaleInfo.Instance.None);
                                    attributeValue = val;
								}
							}
						}
						else
						{
							attributeValue = ((DataSimpleAttribute) instanceAttribute).AttributeValue;
						}

						break;
					case ElementType.ArrayAttribute:
						ArrayAttributeElement arrayAttributeElement = (ArrayAttributeElement) instanceAttribute.GetSchemaModelElement();
						string[] strValues = ((DataArrayAttribute) instanceAttribute).AttributeValues;
						DataTable dataTable = new DataTable(name);
						DataColumn dataColumn;
						DataRow dataRow;

						if (arrayAttributeElement.Dimension == 1 || arrayAttributeElement.Dimension == 2)
						{
							int cols = arrayAttributeElement.ColumnCount;
							int rows = strValues.Length / cols;
							if ((strValues.Length % cols) > 0)
							{
								rows += 1; // add an extra row for remaining elements
							}

							Type elementType = DataTypeConverter.ConvertToSystemType(arrayAttributeElement.ElementDataType);

							for (int i = 0; i < cols; i++)
							{
								// Create new DataColumn for each of columns in the array.    
								dataColumn = new DataColumn();
								dataColumn.DataType = elementType;
								dataColumn.ColumnName = arrayAttributeElement.GetColumnTitle(i);
								dataColumn.ReadOnly = false;
								// Add the Column to the DataColumnCollection.
								dataTable.Columns.Add(dataColumn);
							}
							
							int pos;

							// add data rows to the data table
							for (int row = 0; row < rows; row++)
							{
								dataRow = dataTable.NewRow();
		
								for (int col = 0; col < cols; col++)
								{
									pos = row * cols + col;
									if (pos < strValues.Length)
									{
										if (strValues[pos].Length > 0)
										{
											dataRow[col] = strValues[pos];
										}
									}
								}

								dataTable.Rows.Add(dataRow);
							}
						}
						else
						{
							throw new DataViewException("Unsupported array dimension " + arrayAttributeElement.Dimension);
						}
						
						attributeValue = dataTable;

						break;

                    case ElementType.VirtualAttribute:
                        VirtualAttributeElement virtualAttribute = (VirtualAttributeElement)instanceAttribute.GetSchemaModelElement();
                        if (virtualAttribute.DataType == DataType.Boolean)
                        {
                            string boolVal = ((DataVirtualAttribute)instanceAttribute).AttributeValue;
                            // if boolVal is empty, replace it with localized "None" value
                            if (boolVal == null || boolVal.Length == 0)
                            {
                                boolVal = LocaleInfo.Instance.None;
                            }

                            // get the corresponding Dynamic Generated BooleanEnum value
                            Type booleanEnumType = EnumTypeFactory.Instance.Create(virtualAttribute);
                            try
                            {
                                attributeValue = Enum.Parse(booleanEnumType, boolVal);
                            }
                            catch (Exception)
                            {
                                attributeValue = 0; // treat it as an unknow boolean value
                            }
                        }
                        else if (virtualAttribute.DataType == DataType.Date ||
                            virtualAttribute.DataType == DataType.DateTime)
                        {
                            if (!string.IsNullOrEmpty(((DataVirtualAttribute)instanceAttribute).AttributeValue))
                            {
                                attributeValue = DateTime.Parse(((DataVirtualAttribute)instanceAttribute).AttributeValue);
                            }
                            else
                            {
                                attributeValue = null;
                            }
                        }
                        else if (virtualAttribute.Constraint != null &&
                                virtualAttribute.Constraint is IEnumConstraint)
                        {
                            string val = ((DataVirtualAttribute)instanceAttribute).AttributeValue;

                            // get the corresponding Dynamic Generated Enum Type value(s)
                            if (virtualAttribute.IsMultipleChoice)
                            {
                                object[] enumValues;

                                if (val != null && val.Length > 0)
                                {
                                    string[] values = ((EnumElement)virtualAttribute.Constraint).GetStringArray(val);
                                    enumValues = new object[values.Length];
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        enumValues[i] = Enum.Parse(EnumTypeFactory.Instance.Create(virtualAttribute), values[i]);
                                    }
                                }
                                else
                                {
                                    enumValues = new object[0];
                                }

                                attributeValue = enumValues;
                            }
                            else
                            {
                                // if value is empty, replace it with localized "None" value
                                if (val == null || val.Length == 0)
                                {
                                    val = LocaleInfo.Instance.None;
                                }

                                try
                                {
                                    attributeValue = Enum.Parse(EnumTypeFactory.Instance.Create(virtualAttribute), val);
                                }
                                catch (Exception)
                                {
                                    // default is unknown
                                    attributeValue = Enum.Parse(EnumTypeFactory.Instance.Create(virtualAttribute), LocaleInfo.Instance.None);
                                }
                            }
                        }
                        else
                        {
                            attributeValue = ((DataVirtualAttribute)instanceAttribute).AttributeValue;
                        }

                        break;

                    case ElementType.ImageAttribute:
                        ImageAttributeElement imageAttributeElement = (ImageAttributeElement)instanceAttribute.GetSchemaModelElement();

                        attributeValue = ((DataImageAttribute)instanceAttribute).AttributeValue;

                        break;

					case ElementType.RelationshipAttribute:
						DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute) instanceAttribute;
						attributeValue = GetRelationshipAttributeValue(relationshipAttr);

						break;
				}
			}

			return attributeValue;
		}

        /// <summary>
        /// Gets the storage value of an attribute of an instance. A storage value is different
        /// from the display value when the attribute is bound to an enum or list constrait
        /// </summary>
        /// <param name="name">The attribute name</param>
        /// <returns>The storage value</returns>
        public string GetAttributeStorageValue(string name)
        {
            string storageValue = null;
            IDataViewElement instanceAttribute = GetInstanceAttribute(name);

            if (instanceAttribute != null)
            {
                switch (instanceAttribute.ElementType)
                {
                    case ElementType.SimpleAttribute:
                        SimpleAttributeElement schemaModelElement = (SimpleAttributeElement)instanceAttribute.GetSchemaModelElement();
                        if (schemaModelElement.Constraint != null &&
                            schemaModelElement.Constraint is IEnumConstraint &&
							schemaModelElement.ConstraintUsage == ConstraintUsage.Restriction)
                        {
                            string val = ((DataSimpleAttribute)instanceAttribute).AttributeValue;
                            if (!string.IsNullOrEmpty(val))
                            {
                                EnumValueCollection enumValues;
                                // get the corresponding Dynamic Generated Enum Type value(s)
                                if (!schemaModelElement.IsMultipleChoice)
                                {
                                    if (schemaModelElement.Constraint is ListElement)
                                    {
                                        // try to get the enum values from a cache since it may be very
                                        // expensive to get enum values from list hangdler
                                        enumValues = EnumTypeFactory.Instance.GetEnumValues(schemaModelElement, (ListElement)schemaModelElement.Constraint);
                                    }
                                    else
                                    {
                                        enumValues = ((IEnumConstraint)schemaModelElement.Constraint).Values;
                                    }

                                    storageValue = enumValues.ConvertToEnumValue(val);
                                }
                            }
                        }

                        break;
                }
            }

            return storageValue;
        }

		/// <summary>
		/// Sets the value of an attribute of an instance.
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <param name="value">The attribute value to be set</param>
		/// <returns>Attribute value</returns>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>		
		public void SetAttributeValue(string name, object value)
		{
			IDataViewElement instanceAttribute = GetInstanceAttribute(name);

			if (instanceAttribute != null)
			{
				switch (instanceAttribute.ElementType)
				{
					case ElementType.SimpleAttribute:
						SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) instanceAttribute.GetSchemaModelElement();
						if (schemaModelElement.DataType == DataType.Boolean)
						{
							// the value is one of Dynamic Generated BooleanEnum values
							// convert it to its string representation or empty string
							// if the value is BooleanEnum.None
							string localizedVal = "";
                            if (value is Boolean)
                            {
                                switch ((bool)value)
                                {
                                    case true:
                                        localizedVal = LocaleInfo.Instance.True;
                                        break;
                                    case false:
                                        localizedVal = LocaleInfo.Instance.False;
                                        break;
                                }
                            }
                            else
                            {
                                // integer is default
                                switch ((int)value)
                                {
                                    case 1: // True, see EnumTypeFactory
                                        localizedVal = LocaleInfo.Instance.True;
                                        break;
                                    case 2: // False, see EnumTypeFactory
                                        localizedVal = LocaleInfo.Instance.False;
                                        break;
                                }
                            }
							
							((DataSimpleAttribute) instanceAttribute).AttributeValue = localizedVal;
						}
						else if (schemaModelElement.DataType == DataType.Date)
						{
                            if (value != null && (DateTime)value > DateTime.MinValue)
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = ((DateTime)value).ToString("d");
                            }
                            else
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = null;
                            }
						}
						else if (schemaModelElement.DataType == DataType.DateTime)
						{
                            if (value != null)
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = ((DateTime)value).ToString("s");
                            }
                            else
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = null;
                            }
						}
						else if (schemaModelElement.Constraint != null &&
							schemaModelElement.Constraint is IEnumConstraint  &&
							schemaModelElement.ConstraintUsage == ConstraintUsage.Restriction)
						{
							// the value is one of Dynamic Generated Enum Type values
							// convert it to its string representation or empty string
							// if the value is None
							string converted = "";

							if (schemaModelElement.IsMultipleChoice)
							{
								// concatenate multiple enum values into a single string
								object[] enumValues = (object[]) value;
								for (int i = 0; i < enumValues.Length; i++)
								{
									if (((int) enumValues[i]) > 0)
									{
										// it is not None value, converted to its string representation
										string enumName = Enum.GetName(EnumTypeFactory.Instance.Create(schemaModelElement), enumValues[i]);
                                        if (converted.Length > 0)
										{
											converted = converted + EnumElement.SEPARATOR + enumName;
										}
										else
										{
											converted = enumName;
										}
									}
								}
							}
							else
							{
								// it is a single value
								if (((int) value) > 0)
								{
									// it is not None value, converted to its string representation
									converted = Enum.GetName(EnumTypeFactory.Instance.Create(schemaModelElement), value);
								}
							}
							
							((DataSimpleAttribute) instanceAttribute).AttributeValue = converted;
						}
                        else if (schemaModelElement.IsHistoryEdit)
                        {
                            if (value != null && ((string)value) != "")
                            {
                                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                                string userName = principal.DisplayText;

                                // build a log message with user name and timestamp
                                string log = userName + " (" + DateTime.Now.ToString("s") + ") : " + value.ToString();

                                // append the text to the existing value
                                string existingLog = ((DataSimpleAttribute)instanceAttribute).AttributeValue;
                                if (!string.IsNullOrEmpty(existingLog))
                                {
                                    log = existingLog + "\n\n" + log;
                                }

                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = log;
                            }
                        }
						else
						{
                            // trim the spaces at beginning and end
                            string val = "";
                            if (value != null)
                            {
                                val = value.ToString();
                                if (!string.IsNullOrEmpty(val))
                                {
                                    val = val.Trim();
                                }
                            }
							((DataSimpleAttribute) instanceAttribute).AttributeValue = val;
						}

						break;
					case ElementType.ArrayAttribute:

						ArrayAttributeElement arrayAttributeElement = (ArrayAttributeElement) instanceAttribute.GetSchemaModelElement();
						DataTable dataTable = (DataTable) value;
						StringBuilder builder = new StringBuilder();

						if (arrayAttributeElement.Dimension == 1 || arrayAttributeElement.Dimension == 2)
						{
							// build a comma separated string
							int cols = dataTable.Columns.Count;
							int rows = dataTable.Rows.Count;
                            if (rows > 0)
                            {
                                for (int row = 0; row < rows; row++)
                                {
                                    for (int col = 0; col < cols; col++)
                                    {
                                        if (row > 0 || col > 0)
                                        {
                                            builder.Append(DataArrayAttribute.DELIMETER);
                                        }

                                        if (!dataTable.Rows[row].IsNull(col))
                                        {
                                            builder.Append(dataTable.Rows[row][col].ToString());
                                        }
                                    }
                                }

                                ((DataArrayAttribute)instanceAttribute).AttributeValue = builder.ToString();
                            }
                            else
                            {
                                ((DataArrayAttribute)instanceAttribute).AttributeValue = "";
                            }

						}
						else
						{
							throw new DataViewException("Unsupported array dimension " + arrayAttributeElement.Dimension);
						}

						break;
                    case ElementType.VirtualAttribute:
                        break;
                    case ElementType.ImageAttribute:
                        ((DataImageAttribute)instanceAttribute).AttributeValue = (string)value;
                        break;
					case ElementType.RelationshipAttribute:
						// The value of a relationship can be assigned manually with a obj_id
						// of the linked instance. It can be called by PropertyGrid UI with
						// value of RelationshipPrimaryKeyView.
						// If the value is a obj_id, set it as attribute value of the
						// relationship, otherwise, ignore it.
						if (value is String)
						{
							((DataRelationshipAttribute) instanceAttribute).AttributeValue = (string) value;
						}

						break;
				}
			}
		}

        /// <summary>
		/// Sets the value of an attribute of an instance in string type.
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <param name="attributeValue">The attribute value in string to be set</param>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>		
        public void SetAttributeStringValue(string name, string attributeValue)
        {
            SetAttributeStringValue(name, attributeValue, true);
        }

		/// <summary>
		/// Sets the value of an attribute of an instance in string type.
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <param name="attributeValue">The attribute value in string to be set</param>
        /// <param name="verifyValue">true to verify the value, false not to verify</param>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>		
		public void SetAttributeStringValue(string name, string attributeValue, bool verifyValue)
		{
			IDataViewElement instanceAttribute = GetInstanceAttribute(name);

			if (instanceAttribute != null)
			{
				switch (instanceAttribute.ElementType)
				{
					case ElementType.SimpleAttribute:
                        // verify the string format
                        SimpleAttributeElement schemaModelElement = (SimpleAttributeElement)instanceAttribute.GetSchemaModelElement();
                        if (schemaModelElement.DataType == DataType.Boolean)
                        {
                            string localizedVal = attributeValue;
                            if (verifyValue &&
                                !string.IsNullOrEmpty(attributeValue) &&
                                attributeValue != LocaleInfo.Instance.None &&
                                attributeValue != LocaleInfo.Instance.True &&
                                attributeValue != LocaleInfo.Instance.False)
                            {
                                throw new Exception(attributeValue + " is not a valid boolean value. The valid boolean value is " + LocaleInfo.Instance.True + " or " + LocaleInfo.Instance.False);
                            }

                            if (attributeValue == LocaleInfo.Instance.None)
                            {
                                attributeValue = "";
                            }

                            ((DataSimpleAttribute)instanceAttribute).AttributeValue = attributeValue;
                        }
                        else if (schemaModelElement.DataType == DataType.Date)
                        {
                            if (!string.IsNullOrEmpty(attributeValue))
                            {
                                // verify the date format
                                if (verifyValue)
                                {
                                    try
                                    {
                                        DateTime.Parse(attributeValue);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(attributeValue + " is not a valid date format.");
                                    }
                                }

                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = attributeValue;
                            }
                            else
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = null;
                            }
                        }
                        else if (schemaModelElement.DataType == DataType.DateTime)
                        {
                            if (!string.IsNullOrEmpty(attributeValue))
                            {
                                if (verifyValue)
                                {
                                    try
                                    {
                                        DateTime.Parse(attributeValue);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception(attributeValue + " is not a valid datetime format.");
                                    }
                                }

                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = attributeValue;
                            }
                            else
                            {
                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = null;
                            }
                        }
                        else if (schemaModelElement.Constraint != null &&
                            schemaModelElement.Constraint is IEnumConstraint &&
							schemaModelElement.ConstraintUsage == ConstraintUsage.Restriction)
                        {
                            if (!string.IsNullOrEmpty(attributeValue) && verifyValue)
                            {
                                if (schemaModelElement.IsMultipleChoice)
                                {
                                    string[] enumValues = attributeValue.Split(';');

                                    // verify each enum value
                                    for (int i = 0; i < enumValues.Length; i++)
                                    {
                                        try
                                        {
                                            // it is a single value, verify
                                            Enum.Parse(EnumTypeFactory.Instance.Create(schemaModelElement), enumValues[i]);
                                        }
                                        catch (Exception)
                                        {
                                            throw new Exception(enumValues[i] + " is not defined in the enum constraint " + schemaModelElement.Constraint.Caption);
                                        }
                                    }
                                }
                                else
                                {
                                    // do not validate the value for the Conditional List Constraint
                                    if (!(schemaModelElement.Constraint is ListElement && ((ListElement)schemaModelElement.Constraint).IsConditionBased))
                                    {
                                        try
                                        {
                                            // it is a single value, verify
                                            Enum.Parse(EnumTypeFactory.Instance.Create(schemaModelElement), attributeValue);
                                        }
                                        catch (Exception)
                                        {
                                            throw new Exception(attributeValue + " is not defined in the enum constraint " + schemaModelElement.Constraint.Caption);
                                        }
                                    }
                                }
                            }

                            ((DataSimpleAttribute)instanceAttribute).AttributeValue = attributeValue;
                        }
                        else if (schemaModelElement.IsHistoryEdit)
                        {
                            if (!string.IsNullOrEmpty(attributeValue))
                            {
                                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                                string userName = principal.DisplayText;

                                // build a log message with user name and timestamp
                                string log = userName + " (" + DateTime.Now.ToString("s") + ") : " + attributeValue;

                                // append the text to the existing value
                                string existingLog = ((DataSimpleAttribute)instanceAttribute).AttributeValue;
 
                                if (!string.IsNullOrEmpty(existingLog))
                                {
                                    log = existingLog + "\n\n" + log;
                                }

                                ((DataSimpleAttribute)instanceAttribute).AttributeValue = log;
                            }
                        }
                        else
                        {
                            ((DataSimpleAttribute)instanceAttribute).AttributeValue = attributeValue;
                        }

						break;
					case ElementType.ArrayAttribute:

						((DataArrayAttribute) instanceAttribute).AttributeValue = attributeValue;

						break;
                    case ElementType.VirtualAttribute:

                        break;
                    case ElementType.ImageAttribute:

                        ((DataImageAttribute)instanceAttribute).AttributeValue = attributeValue;

                        break;
					case ElementType.RelationshipAttribute:

                        DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute)instanceAttribute;

                        SetRelationshipAttributeValue(relationshipAttr, attributeValue);

						break;
				}
			}
		}

		/// <summary>
		/// Reset the value of an attribute to its original value
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>		
		public void ResetAttributeValue(string name)
		{
			String attributeName = name;
			int pos = attributeName.IndexOf("_");
			if (pos > 0)
			{
				// extract the name of a relationship
				attributeName = attributeName.Substring(0, pos);
			}

			IDataViewElement instanceAttribute = _dataView.ResultAttributes[attributeName];

			if (instanceAttribute != null)
			{
				CopyInstanceValue(instanceAttribute, _dataView, _dataSet, _rowIndex, _lazyCopy);
			}
		}

		/// <summary>
		/// Gets the value of an attribute of an instance in string type.
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <returns>Attribute value in string</returns>
		/// <remarks>name can be that of a primary key of a relationship. In this case,
		/// name consists of relationship name and primary key name in format of
		/// RelationshipName_PrimaryKeyName</remarks>
		public string GetAttributeStringValue(string name)
		{
			string attributeValue = null;
			IDataViewElement instanceAttribute = GetInstanceAttribute(name);

			if (instanceAttribute != null)
			{
				switch (instanceAttribute.ElementType)
				{
					case ElementType.SimpleAttribute:
						attributeValue = ((DataSimpleAttribute) instanceAttribute).AttributeValue;

						break;
					case ElementType.ArrayAttribute:
						attributeValue = ((DataArrayAttribute) instanceAttribute).AttributeValue;

						break;
                    case ElementType.VirtualAttribute:
                        attributeValue = ((DataVirtualAttribute)instanceAttribute).AttributeValue;

                        break;
                    case ElementType.ImageAttribute:
                        attributeValue = ((DataImageAttribute)instanceAttribute).AttributeValue;

                        break;
					case ElementType.RelationshipAttribute:
						DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute) instanceAttribute;

						attributeValue = GetRelationshipAttributeValue(relationshipAttr);

						break;
				}
			}

			return attributeValue;
		}

		/// <summary>
		/// Gets the objId of referenced instance, given a relationship name
		/// </summary>
		/// <param name="relationshipName">The name of relationship attribute.</param>
		/// <returns>The objId of referenced instance, it could be null.</returns>
		public string GetReferencedObjID(string relationshipName)
		{
			string objId = null;
			
			if (_dataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_dataView.BaseClass.Name, relationshipName)] != null)
			{
                // starting from 3.7.0, the relationship element saved in backup file is using
                // className_relationshipName as element name
                objId = _dataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_dataView.BaseClass.Name, relationshipName)].Rows[_rowIndex][NewteraNameSpace.OBJ_ID].ToString();
			}
            else if (_dataSet.Tables[relationshipName] != null)
            {
                // from 3.6.2 and older version, the relationship element saved in backup file is using
                // relationship name as element name, therefore, in order to be able to restore the
                // backup file of 3.6.2 and older version, we need to handle the situation here
                objId = _dataSet.Tables[relationshipName].Rows[_rowIndex][NewteraNameSpace.OBJ_ID].ToString();
            }

			return objId;
		}

		/// <summary>
		/// Sets the objId of referenced instance, given a relationship name
		/// </summary>
		/// <param name="relationshipName">The name of relationship attribute.</param>
		/// <param name="objId">The objId of referenced instance.</param>
		public void SetReferencedObjID(string relationshipName, string objId)
		{
            if (_dataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_dataView.BaseClass.Name, relationshipName)] != null)
			{
                _dataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_dataView.BaseClass.Name, relationshipName)].Rows[_rowIndex][NewteraNameSpace.OBJ_ID] = objId;
			}
            else if (_dataSet.Tables[relationshipName] != null)
            {
                // backward compatibility purpose
                _dataSet.Tables[relationshipName].Rows[_rowIndex][NewteraNameSpace.OBJ_ID] = objId;
            }
		}

		/// <summary>
		/// Save the changed values in InstanceData instance to the corresponding
		/// row in the DataSet.
		/// </summary>
		/// <remarks>If there is no DataSet associated with the InstanceData instance,
		/// no operation will be performed by this call.</remarks>
		public void SaveValuesToDataSet()
		{
			if (_dataSet != null)
			{
				foreach (IDataViewElement attribute in _dataView.ResultAttributes)
				{
					SaveValueToDataSet(attribute, _dataView, _dataSet, _rowIndex);
				}
			}
		}

        /// <summary>
        /// Copy the values of an instance from DataSet to Instance Attributes
        /// </summary>
        /// <param name="dataView">The dataview that is copy target</param>
        /// <param name="dataSet">The data set that is copy source</param>
        /// <param name="rowIndex">The row index to the data set</param>
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        /// <remarks>If dataSet is null, it will set values of all attributes to empty string</remarks>
        private void CopyInstanceValues(DataViewModel dataView, DataSet dataSet, int rowIndex, bool lazyCopy)
		{
			// set the id of an instance whose values will be kept in result attributes of a data view
			if (_dataSet != null && lazyCopy)
			{
				dataView.CurrentObjId = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, NewteraNameSpace.OBJ_ID, rowIndex);
			}
			else
			{
				dataView.CurrentObjId = null;
			}

            foreach (IDataViewElement attribute in dataView.ResultAttributes)
            {
                CopyInstanceValue(attribute, dataView, dataSet, rowIndex, lazyCopy);
            }
		}

		/// <summary>
		/// Copy a value of an instance attribute from DataSet to an Instance Attribute
		/// </summary>
		/// <param name="instanceAttribute">The instance attribure that store copied value</param>
		/// <param name="dataView">The dataview that is copy target</param>
		/// <param name="dataSet">The data set that is copy source</param>
		/// <param name="rowIndex">The row index to the data set</param> 
        /// <param name="lazyCopy">True to copy all attribute values without checkings, false to exclude copying some of the attribute values </param>
        private void CopyInstanceValue(IDataViewElement instanceAttribute,
			DataViewModel dataView, DataSet dataSet, int rowIndex, bool lazyCopy)
		{
			switch (instanceAttribute.ElementType)
			{
				case ElementType.SimpleAttribute:
					DataSimpleAttribute simpleAttribute = (DataSimpleAttribute) instanceAttribute;
					bool isUnique = IsUnique(simpleAttribute);

					if (dataSet != null)
					{
						if (!lazyCopy)
						{
                            if (isUnique)
                            {
                                // do not copy unique values
                                simpleAttribute.AttributeValue = "";
                            }
                            else if (!simpleAttribute.AllowManualUpdate)
                            {
                                // do not copy values that cannot be updated by users, use default value if exists
                                if (!string.IsNullOrEmpty(simpleAttribute.DefaultValue))
                                {
                                    simpleAttribute.AttributeValue = simpleAttribute.DefaultValue;
                                }
                                else
                                {
                                    simpleAttribute.AttributeValue = "";
                                }
                            }
                            else if (simpleAttribute.IsHistoryEdit)
                            {
                                // do not copy history
                                simpleAttribute.AttributeValue = "";
                            }
                            else
                            {
                                simpleAttribute.AttributeValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, simpleAttribute.Name, rowIndex);
                            }
						}
						else
						{
                            simpleAttribute.AttributeValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, simpleAttribute.Name, rowIndex);
						}
					}
					else
					{
						// clear the attribute value
						simpleAttribute.AttributeValue = "";
					}

					break;
				case ElementType.ArrayAttribute:
					DataArrayAttribute arrayAttribute = (DataArrayAttribute) instanceAttribute;

					if (dataSet != null)
					{
					    arrayAttribute.AttributeValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, arrayAttribute.Name, rowIndex);
					}
					else
					{
						// clear the attribute value
						arrayAttribute.AttributeValue = "";
					}

					break;
                case ElementType.VirtualAttribute:
                    DataVirtualAttribute virtualAttribute = (DataVirtualAttribute)instanceAttribute;

                    if (dataSet != null)
                    {
                        virtualAttribute.AttributeValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, virtualAttribute.Name, rowIndex);
                    }
                    else
                    {
                        // clear the attribute value
                        virtualAttribute.AttributeValue = "";
                    }
                    break;
                case ElementType.ImageAttribute:
                    DataImageAttribute imageAttribute = (DataImageAttribute)instanceAttribute;

                    if (dataSet != null)
                    {
                        if (_lazyCopy)
                        {
                            imageAttribute.AttributeValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, imageAttribute.Name, rowIndex);
                        }
                        else
                        {
                            // do not copy image id since it is unique among the instances
                            // clear the attribute value
                            imageAttribute.AttributeValue = "";
                        }
                    }
                    else
                    {
                        // clear the attribute value
                        imageAttribute.AttributeValue = "";
                    }
                    break;
				case ElementType.RelationshipAttribute:
					DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute) instanceAttribute;
                    RelationshipAttributeElement relationshipElement = (RelationshipAttributeElement)relationshipAttribute.GetSchemaModelElement();

                    // copy the values of primary keys if the relationship is not one-to-one
					DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
                    if (primaryKeys != null)
					{
						if (dataSet != null &&
                            (relationshipElement.Type != RelationshipType.OneToOne || lazyCopy))
						{
							// The table in data set for a relationship attribute consists of the base class name
							// and relationship attribute name so that when more than
							// one relationship attributes refer to a same class, there won't be
							// name conflicts, since the relationship attribute names are
							// unique among a class.

							foreach (DataSimpleAttribute pk in primaryKeys)
							{
                                string pkValue = DataSetHelper.GetCellValue(dataSet,
                                    DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipAttribute.Name),
									pk.Name, rowIndex);
								// PK values may not present in the data set
								if (pkValue != null)
								{
									pk.AttributeValue = pkValue;
								}
								else
								{
                                    // backward compatibility
                                    pkValue = DataSetHelper.GetCellValue(dataSet, relationshipAttribute.Name,
                                                                         pk.Name, rowIndex);
                                    if (pkValue != null)
                                    {
                                        pk.AttributeValue = pkValue;
                                    }
                                    else
                                    {
                                        pk.AttributeValue = "";
                                    }
								}
							}
						}
						else
						{
							foreach (DataSimpleAttribute pk in primaryKeys)
							{
								pk.AttributeValue = "";
							}
						}
					}

					break;
			}
		}

		/// <summary>
		/// Gets the value of a relationship attribute that combines values of
		/// its primary keys.
		/// </summary>
		/// <param name="relationshipAttr">The DataRelationshipAttribute instance</param>
		/// <returns>The value of relationship attribute</returns>
		private string GetRelationshipAttributeValue(DataRelationshipAttribute relationshipAttr)
		{
			string attributeValue = "";
			if (relationshipAttr.PrimaryKeys != null)
			{
				int index = 0;
				foreach (DataSimpleAttribute pk in relationshipAttr.PrimaryKeys)
				{
					attributeValue += pk.AttributeValue;
					if (index < relationshipAttr.PrimaryKeys.Count - 1)
					{
						attributeValue += "&";
					}
					index ++;
				}
			}

			return attributeValue;
		}

        /// <summary>
        /// Sets the value of a relationship attribute that combines values of
        /// its primary keys.
        /// </summary>
        /// <param name="relationshipAttr">The DataRelationshipAttribute instance</param>
        /// <param name="attributeValue">Attribute consists of primary key values of relationship</param>
        private void SetRelationshipAttributeValue(DataRelationshipAttribute relationshipAttr, string attributeValue)
        {
            if (relationshipAttr.PrimaryKeys != null && !string.IsNullOrEmpty(attributeValue))
            {
                string[] pkValues = attributeValue.Split('&');
                int index = 0;
                foreach (DataSimpleAttribute pk in relationshipAttr.PrimaryKeys)
                {
                    if (index < pkValues.Length)
                    {
                        pk.AttributeValue = pkValues[index].Trim();
                    }
                    else
                    {
                        pk.AttributeValue = "";
                    }

                    index++;
                }
            }
        }

		/// <summary>
		/// Save a value of an instance attribute to the corresponding cell in the DataSet.
		/// </summary>
		/// <param name="instanceAttribute">The instance attribure that store copied value</param>
		/// <param name="dataView">The dataview from which to get an attribute value</param>
		/// <param name="dataSet">The data set to which the value is saved</param>
		/// <param name="rowIndex">The row index indicating the row in the data set</param> 
		private void SaveValueToDataSet(IDataViewElement instanceAttribute,
			DataViewModel dataView, DataSet dataSet, int rowIndex)
		{
			switch (instanceAttribute.ElementType)
			{
				case ElementType.SimpleAttribute:
					DataSimpleAttribute simpleAttribute = (DataSimpleAttribute) instanceAttribute;

					DataSetHelper.SetCellValue(dataSet,
						dataView.BaseClass.ClassName, simpleAttribute.Name,
						_rowIndex, simpleAttribute.AttributeValue);

					break;
				case ElementType.ArrayAttribute:
					DataArrayAttribute arrayAttribute = (DataArrayAttribute) instanceAttribute;

					DataSetHelper.SetCellValue(dataSet,
						dataView.BaseClass.ClassName, arrayAttribute.Name,
						_rowIndex, arrayAttribute.AttributeValue);

					break;

                case ElementType.VirtualAttribute:

                case ElementType.ImageAttribute:
                    DataImageAttribute imageAttribute = (DataImageAttribute)instanceAttribute;

                    DataSetHelper.SetCellValue(dataSet,
                        dataView.BaseClass.ClassName, imageAttribute.Name,
                        _rowIndex, imageAttribute.AttributeValue);

                    break;

				case ElementType.RelationshipAttribute:
					DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute) instanceAttribute;
					
					// Save the values of primary keys
					DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
					if (primaryKeys != null)
					{
                        foreach (DataSimpleAttribute pk in primaryKeys)
                        {
                            DataSetHelper.SetCellValue(dataSet,
                                dataView.BaseClass.ClassName, pk.Name,
                                _rowIndex, pk.AttributeValue);
                        }
					}

					break;
			}
		}

		/// <summary>
		/// Gets the value indicating whether the value of an instance attribute has
		/// been changed or not.
		/// </summary>
		/// <param name="instanceAttribute">The instance attribure</param>
		/// <param name="dataView">The dataview</param>
		/// <param name="dataSet">The data set that contains original values</param>
		/// <param name="rowIndex">The row index to the data set</param> 
		/// <remarks>This method call set IsValueChange property of the attribute being chekced</remarks>
		private bool IsAttributeValueChanged(IDataViewElement instanceAttribute,
			DataViewModel dataView, DataSet dataSet, int rowIndex)
		{
			bool isChanged = false;
			string originalValue;

			switch (instanceAttribute.ElementType)
			{
				case ElementType.SimpleAttribute:
					DataSimpleAttribute simpleAttribute = (DataSimpleAttribute) instanceAttribute;

					if (dataSet != null)
					{
						originalValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, simpleAttribute.Name, rowIndex);
						if (simpleAttribute.IsValueDifferent(originalValue))
						{
							simpleAttribute.IsValueChanged = true;
						}
						else
						{
							simpleAttribute.IsValueChanged = false;
						}

						isChanged = simpleAttribute.IsValueChanged;
					}

					break;

				case ElementType.ArrayAttribute:
					DataArrayAttribute arrayAttribute = (DataArrayAttribute) instanceAttribute;

					if (dataSet != null)
					{
						originalValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, arrayAttribute.Name, rowIndex);
						if (arrayAttribute.IsValueDifferent(originalValue))
						{
							arrayAttribute.IsValueChanged = true;
						}
						else
						{
							arrayAttribute.IsValueChanged = false;
						}

						isChanged = arrayAttribute.IsValueChanged;
					}

					break;

                case ElementType.VirtualAttribute:

                    break;

                case ElementType.ImageAttribute:
                    DataImageAttribute imageAttribute = (DataImageAttribute)instanceAttribute;

                    if (dataSet != null)
                    {
                        originalValue = DataSetHelper.GetCellValue(dataSet, dataView.BaseClass.ClassName, imageAttribute.Name, rowIndex);
                        if (imageAttribute.IsValueDifferent(originalValue))
                        {
                            imageAttribute.IsValueChanged = true;
                        }
                        else
                        {
                            imageAttribute.IsValueChanged = false;
                        }

                        isChanged = imageAttribute.IsValueChanged;
                    }

                    break;

				case ElementType.RelationshipAttribute:
					bool isPkValueChanged = false;
					DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute) instanceAttribute;
					DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
					if (primaryKeys != null && dataSet != null)
					{
						foreach (DataSimpleAttribute pk in primaryKeys)
						{
                            string pkValue = DataSetHelper.GetCellValue(dataSet,
                                DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipAttribute.Name),
								pk.Name, rowIndex);
							if (pk.IsValueDifferent(pkValue))
							{
								pk.IsValueChanged = true;
								isPkValueChanged = true;
							}
							else
							{
								pk.IsValueChanged = false;
							}
						}
					}

					if (isPkValueChanged)
					{
						// primary key value(s) are changed, mark the relationship as changed
						relationshipAttribute.IsValueChanged = true;
					}
					else
					{
						relationshipAttribute.IsValueChanged = false;
					}

					isChanged = relationshipAttribute.IsValueChanged;

					break;
			}

			return isChanged;
		}

		/// <summary>
		/// Find the instance attribute of given name from the data view
		/// </summary>
		/// <param name="name">The attribute name</param>
		/// <returns>The IDataViewElement</returns>
		private IDataViewElement GetInstanceAttribute(string name)
		{
			IDataViewElement instanceAttribute = null;
			int pos;
			pos = name.IndexOf("_");

			if (pos < 0)
			{
				// This is a name of a result attributes
				instanceAttribute = _dataView.ResultAttributes[name];
			}
			else
			{
				// This is a name of a primary key of a relationship attribute
				string relationshipName, pkName;
				relationshipName = name.Substring(0, pos);
				pkName = name.Substring(pos + 1);
				
				instanceAttribute = _dataView.ResultAttributes[relationshipName];
				if (instanceAttribute != null &&
					instanceAttribute.ElementType == ElementType.RelationshipAttribute)
				{
					DataViewElementCollection pks = ((DataRelationshipAttribute) instanceAttribute).PrimaryKeys;
					if (pks != null)
					{
						instanceAttribute = pks[pkName];
					}
				}
				else
				{
					instanceAttribute = null;
				}
			}

			return instanceAttribute;
		}

		/// <summary>
		/// Gets the value indicating whether the value of a simple attribute is unique
		/// </summary>
		/// <param name="simpleAttribute">A simple attribute</param>
		/// <returns>true if it is unique, false otherwise.</returns>
		private bool IsUnique(DataSimpleAttribute simpleAttribute)
		{
			DataClass dataClass = _dataView.FindClass(simpleAttribute.OwnerClassAlias);
			ClassElement classElement = _dataView.SchemaModel.FindClass(dataClass.ClassName);
			SimpleAttributeElement attributeElement = classElement.FindInheritedSimpleAttribute(simpleAttribute.Name);

			if (attributeElement != null && attributeElement.IsUnique)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// Gets an array of read-only attributes from the instance data
        /// </summary>
        /// <returns>An array of strings</returns>
        private string[] GetReadOnlyAttributes()
        {
            string[] readOnlyAttributes = new string[0];

            if (_dataSet != null)
            {
                string readOnlyAttributeStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName, NewteraNameSpace.READ_ONLY, _rowIndex);

                if (!string.IsNullOrEmpty(readOnlyAttributeStr))
                {
                    readOnlyAttributes = readOnlyAttributeStr.Split(';');
                }
            }

            return readOnlyAttributes;
        }
	}
}