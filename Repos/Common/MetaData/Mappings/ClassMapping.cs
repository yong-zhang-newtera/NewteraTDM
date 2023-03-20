/*
* @(#)ClassMapping.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Text;
	using System.Collections;
    using System.Collections.Specialized;
	using System.Reflection;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;

	/// <summary>
	/// The class represents mapping definition of two classes between source and
	/// destination. A ClassMapping object contains defintions of attribute mappings
	/// between a source class and a destination class, and/or default values
	/// for some of destination attributes.
	/// </summary>
	/// <version>  1.0.0 03 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class ClassMapping : MappingNodeBase
	{
		private string _sourceClassName;
		private int _sourceClassIndex;
		private string _destinationClassName;
        private bool _isChecked;
        private string _tableName;

		private DataViewModel _dstDataView; // run-time use only
		private DataViewModel _srcDataView; // run-time use only
		
		private AttributeMappingCollection _attributeMappings;
		private DefaultValueCollection _defaultValues;
		private TransformCardinal _transformCardinal;
        private SelectRowScript _selectSrcRowScript;
        private IdentifyRowScript _identifyRowScript;
        private DefaultValueCollection _overridingDefaultValues; // run-time use
        private TransformerBase _rowSelector = null; // run-time use only
        private TransformerBase _rowIdentifier = null; // run-time use only

		/// <summary>
		/// Initiate an instance of a ClassMapping class.
		/// </summary>
		public ClassMapping(string sourceClassName, string destinationClassName) : base()
		{
			_sourceClassName = sourceClassName;
			_sourceClassIndex = 0;
			_destinationClassName = destinationClassName;
			_dstDataView = null;
			_srcDataView = null;
            _tableName = null;
			_isChecked = true;

			_attributeMappings = new AttributeMappingCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _attributeMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			_defaultValues = new DefaultValueCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _defaultValues.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			_transformCardinal = TransformCardinal.OneToOne; // default

            _selectSrcRowScript = new SelectRowScript();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _selectSrcRowScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

            _identifyRowScript = new IdentifyRowScript();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _identifyRowScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

            _overridingDefaultValues = null;
		}
		
		/// <summary>
		/// Initiating an instance of ClassMapping class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ClassMapping(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the information indicating whether the class mapping is checked or not
		/// </summary>
		/// <value>The default is false</value>
		public bool IsChecked
		{
			get
			{
				return _isChecked;
			}
			set
			{
				_isChecked = value;
			}
		}
		
		/// <summary>
		/// Gets or sets name of the source class associated with a ClassMapping
		/// </summary>
		public string SourceClassName
		{
			get
			{
				return _sourceClassName;
			}
			set
			{
				_sourceClassName = value;
			}
		}

		/// <summary>
		/// Gets or sets index of the source class that matches to the index
		/// of the DataTable in the Source DataSet.
		/// </summary>
		public int SourceClassIndex
		{
			get
			{
				return _sourceClassIndex;
			}
			set
			{
				_sourceClassIndex = value;
			}
		}

		/// <summary>
		/// Gets name of the destination class associated with a ClassMapping
		/// </summary>
		public string DestinationClassName
		{
			get
			{
				return _destinationClassName;
			}
			set
			{
				_destinationClassName = value;
			}
		}

        /// <summary>
        /// Gets name of the destination table name associated with a ClassMapping
        /// </summary>
        public string DestinationTableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

		/// <summary>
		/// Gets or sets the default DataView instance for the source class
		/// </summary>
		/// <value>A DataViewModel instance</value>
		public DataViewModel SourceDataView
		{
			get
			{
				return _srcDataView;
			}
			set
			{
				_srcDataView = value;
			}
		}

		/// <summary>
		/// Gets or sets the default DataView instance for the destination class
		/// </summary>
		/// <value>A DataViewModel instance</value>
		public DataViewModel DestinationDataView
		{
			get
			{
				return _dstDataView;
			}
			set
			{
				_dstDataView = value;
			}
		}

		/// <summary>
		/// Gets or sets the collection of the attribute mappings contained in a ClassMapping
		/// </summary>
		public AttributeMappingCollection AttributeMappings
		{
			get
			{
				return _attributeMappings;
			}
			set
			{
				_attributeMappings = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the collection of DefaultValue instances
		/// </summary>
		public DefaultValueCollection DefaultValues
		{
			get
			{
				return _defaultValues;
			}
			set
			{
				_defaultValues = value;

				FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets or sets the collection of overriding DefaultValue instances
        /// </summary>
        public DefaultValueCollection OverridingDefaultValues
        {
            get
            {
                return _overridingDefaultValues;
            }
            set
            {
                _overridingDefaultValues = value;
            }
        }

        /// <summary>
        /// Gets the script object that selects the rows from source data
        /// </summary>
        public SelectRowScript SelectSrcRowScript
        {
            get
            {
                return _selectSrcRowScript;
            }
        }

        /// <summary>
        /// Gets the script object that identify a row in the destination data table
        /// </summary>
        public IdentifyRowScript IdentifyDstRowScript
        {
            get
            {
                return _identifyRowScript;
            }
        }

        /// <summary>
        /// Gets or sets the row selector associated with the mapping class
        /// </summary>
        internal TransformerBase RowSelector
        {
            get
            {
                return _rowSelector;
            }
            set
            {
                _rowSelector = value;
            }
        }

        /// <summary>
        /// Gets or sets the row identifier associated with the mapping class
        /// </summary>
        internal TransformerBase RowIdentifier
        {
            get
            {
                return _rowIdentifier;
            }
            set
            {
                _rowIdentifier = value;
            }
        }

		/// <summary>
		/// gets or sets the transform cardinal type
		/// </summary>
		/// <value>One of the TransformCardinal enum values.</value>
		public TransformCardinal TransformCardinalType
		{
			get
			{
				return _transformCardinal;
			}
			set
			{
				_transformCardinal = value;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.ClassMapping;
			}
		}

		/// <summary>
		/// Clear the attribute mappings and default values.
		/// </summary>
		public void Clear()
		{
			_attributeMappings.Clear();
			_defaultValues.Clear();
            if (_overridingDefaultValues != null)
            {
                _overridingDefaultValues = null;
            }
		}

        /// <summary>
        /// Override the default value of a relationship attribute
        /// </summary>
        public void OverrideRelationshipDefaultValue(string attributeName, string defaultValueString)
        {
            OverrideRelationshipDefaultValue(attributeName, defaultValueString, -1);
        }

        /// <summary>
        /// Override the default value of a relationship attribute
        /// </summary>
        public void OverrideRelationshipDefaultValue(string attributeName, string defaultValueString, int overridingRowIndex)
        {
            if (_dstDataView != null && !string.IsNullOrEmpty(defaultValueString))
            {
                DefaultValue defaultValue;
                DataRelationshipAttribute relationship = _dstDataView.ResultAttributes[attributeName] as DataRelationshipAttribute;
                if (relationship != null && relationship.PrimaryKeyCount > 0)
                {
                    string[] pkValues = defaultValueString.Split('&');
                    int index = 0;
                    string pkFullName, pkValue;
                    foreach (DataSimpleAttribute pk in relationship.PrimaryKeys)
                    {
                        // name of mapping for primary key in format of
                        // relationshipName_primaryKeyName
                        pkFullName = relationship.GetUniquePKName(pk.Name);
                        if (index < pkValues.Length)
                        {
                            pkValue = pkValues[index];
                        }
                        else
                        {
                            pkValue = null;
                        }

                        defaultValue = new DefaultValue(pkFullName, pkValue);
                        defaultValue.AppliedRowIndex = overridingRowIndex;

                        if (_overridingDefaultValues == null)
                        {
                            _overridingDefaultValues = new DefaultValueCollection();
                        }

                        _overridingDefaultValues.Add(defaultValue);

                        index++;
                    }
                }
            }
        }

		/// <summary>
		/// Transform data from source format to destination format according to
		/// the class mappings.
		/// </summary>
		/// <param name="sourceDataSet">The DataSet for the source data</param>
		/// <param name="dstDataSet">The DataSet containing the transformed data.</param>
		/// <param name="assembly">An assembly containing transformer classes.</param>
		/// <remarks>The method will write the transformed data into
		/// the corresponding DataTable instance(s) in destinationDataSet.</remarks>
		public void Transform(DataSet sourceDataSet, DataSet dstDataSet, Assembly assembly)
		{
			DataTable srcDataTable = sourceDataSet.Tables[SourceClassIndex];

			if (srcDataTable != null)
			{
				BuildDataSetVisitor buildDataSetVisitor = new BuildDataSetVisitor(this,
					dstDataSet);
				// The visitor will add DataTable instance(s) for storing transformed data
				// to the destinationDataSet
				_dstDataView.Accept(buildDataSetVisitor);
                DataTable dstDataTable = dstDataSet.Tables[_dstDataView.BaseClass.Name];

				int srcRowIndex = 0;
                int dstRowIndex = 0;
                int identifiedRowIndex;
				// transform the data one source row at a time
				foreach (DataRow srcDataRow in srcDataTable.Rows)
				{
                    // skip the rows that are not selected by the script
                    if (IsRowSelected(srcDataTable, srcRowIndex,assembly))
                    {
                        identifiedRowIndex = IdentifyDstRow(srcDataTable, srcRowIndex, dstDataTable, assembly);
                        if (identifiedRowIndex < 0)
                        {
                            // no existing row has been identified in the destination dataset, add a new row to the destination dataset
                            identifiedRowIndex = dstRowIndex;
                            dstRowIndex++;
                        }

                        switch (_transformCardinal)
                        {
                            case TransformCardinal.OneToOne:
                                TransformRow(srcDataRow, dstDataSet, identifiedRowIndex, assembly);
                                break;

                            case TransformCardinal.AllToOne:
                                MergeRow(srcDataRow, dstDataSet, identifiedRowIndex, assembly);
                                break;
                        }
                    }

                    srcRowIndex++;
				}
			}
		}

		/// <summary>
		/// Select data from source instances and copy them to an destination instance
		/// according to the class mappings.
		/// </summary>
		/// <param name="sourceDataSet">The DataSet for the source data</param>
		/// <param name="dstDataSet">The DataSet containing the transformed data.</param>
		/// <param name="assembly">An assembly containing transformer classes.</param>
		/// <remarks>This method is used by Selector. It will create an new row to
		/// destination DataTable only once to store the selected data from multiple
		/// instances in the source DataSet for a certain class.
		/// For simple attribute in destination row, the value is override by each
		/// iteration; for array attribute in destination row, a new row is added
		/// to the array value by each iteration.</remarks>
		public void Select(DataSet sourceDataSet, DataSet dstDataSet, Assembly assembly)
		{
			DataTable srcDataTable = sourceDataSet.Tables[SourceClassIndex];

			if (srcDataTable != null)
			{
				// create a DataTable in destination DataSet to store the selected
				// data. This is done once
				if (dstDataSet.Tables[_destinationClassName] == null)
				{
					BuildDataSetVisitor buildDataSetVisitor = new BuildDataSetVisitor(this,
						dstDataSet);
					// The visitor will add a DataTable instance for storing selected data
					// to the destinationDataSet
					_dstDataView.Accept(buildDataSetVisitor);

					// create an empty row to store data for destination instance
					CreateEmptyRow(dstDataSet, _dstDataView);
				}

				// transform the data one row at a time
				foreach (DataRow srcDataRow in srcDataTable.Rows)
				{
					SelectRow(srcDataRow, dstDataSet, assembly);
				}
			}
		}

        /// <summary>
        /// Gets a DefaultValue instance for a given attribute name from existing default value collection.
        /// </summary>
        /// <param name="dstAttributeName">The given attribute name</param>
        /// <returns>A DefaultValue instance, null if no DefaultValue instance with the given name.</returns>
        public DefaultValue GetDefaultValue(string dstAttributeName)
        {
            DefaultValue defaultValue = null;

            foreach (DefaultValue item in _defaultValues)
            {
                if (item.DestinationAttributeName == dstAttributeName)
                {
                    defaultValue = item;
                    break;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a DefaultValue for a relationship attribute name defined in the class mapping.
        /// </summary>
        /// <param name="dstAttributeName">The relationship attribute name</param>
        /// <returns>A DefaultValue instance, null if no DefaultValue instance for the relationship.</returns>
        /// <remarks>The default value string consists of multiple primary key values</remarks>
        public DefaultValue GetRelationshipDefaultValue(string dstAttributeName)
        {
            DefaultValue defaultValue = null;
            DefaultValue pkDefaultValue;
            if (_dstDataView != null)
            {
                DataRelationshipAttribute relationship = _dstDataView.ResultAttributes[dstAttributeName] as DataRelationshipAttribute;
                if (relationship != null && relationship.PrimaryKeyCount > 0)
                {
                    foreach (DataSimpleAttribute pk in relationship.PrimaryKeys)
                    {
                        // name of mapping for primary key in format of
                        // relationshipName_primaryKeyName
                        string pkFullName = relationship.GetUniquePKName(pk.Name);

                        pkDefaultValue = this.GetDefaultValue(_defaultValues, pkFullName);

                        if (pkDefaultValue != null)
                        {
                            if (defaultValue == null)
                            {
                                defaultValue = new DefaultValue(dstAttributeName, pkDefaultValue.Value);
                            }
                            else
                            {
                                // append the pk value
                                defaultValue.Value += "&" + pkDefaultValue.Value;
                            }
                        }
                    }
                }
            }

            return defaultValue;
        }

		/// <summary>
		/// Gets a DefaultValue instance for a given attribute name from the given collection.
		/// </summary>
        /// <param name="defaultValues">The default value collection from which to find a default value</param>
		/// <param name="dstAttributeName">The given attribute name</param>
		/// <returns>A DefaultValue instance, null if no DefaultValue instance with the given name.</returns>
		public DefaultValue GetDefaultValue(DefaultValueCollection defaultValues, string dstAttributeName)
		{
			DefaultValue defaultValue = null;

            foreach (DefaultValue item in defaultValues)
			{
				if (item.DestinationAttributeName == dstAttributeName)
				{
                    // Compare the destination class name if it is available
                    if (string.IsNullOrEmpty(item.DestinationClassName) ||
                        item.DestinationClassName == this.DestinationClassName)
                    {
                        defaultValue = item;
                        break;
                    }
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the information indicating whether a DefaultValue instance for a given
		/// attribute name has already existed.
		/// </summary>
		/// <param name="dstAttributeName">The given attribute name</param>
		/// <returns>true if the default value has existed, false otherwise.</returns>
		public bool IsDefaultValueExist(string dstAttributeName)
		{
			bool found = false;

			foreach (DefaultValue item in _defaultValues)
			{
				if (item.DestinationAttributeName == dstAttributeName)
				{
					found = true;
					break;
				}
			}

			return found;
		}

        /// <summary>
        /// Create, modify or clear a DefaultValue instance in the ClassMapping.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="value">The default value.</param>
        public void SetDefaultValue(string attributeName, string attributeValue)
        {
            DefaultValue defaultValue = null;
            foreach (DefaultValue dv in this._defaultValues)
            {
                if (dv.DestinationAttributeName == attributeName)
                {
                    defaultValue = dv;
                    break;
                }
            }

            if (defaultValue == null)
            {
                // create a Default Value instance
                defaultValue = new DefaultValue(attributeName, attributeValue);
                _defaultValues.Add(defaultValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    // change the default value
                    defaultValue.Value = attributeValue;
                }
                else
                {
                    // remove the default value
                    _defaultValues.Remove(defaultValue);
                }
            }
        }

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Source");
			if (str != null && str.Length > 0)
			{
				_sourceClassName = str;
			}
			else
			{
				_sourceClassName = null;
			}

			str = parent.GetAttribute("SourceIdx");
			if (str != null && str.Length > 0)
			{
				_sourceClassIndex = int.Parse(str);
			}
			else
			{
				_sourceClassIndex = 0;
			}

			str = parent.GetAttribute("Destination");
			if (str != null && str.Length > 0)
			{
				_destinationClassName = str;
			}
			else
			{
				_destinationClassName = null;
			}

            str = parent.GetAttribute("Checked");
            if (str != null && str == "false")
            {
                _isChecked = false;
            }
            else
            {
                _isChecked = true; // default
            }

			str = parent.GetAttribute("Cardinal");
			if (str != null && str.Length > 0)
			{
				_transformCardinal = (TransformCardinal) Enum.Parse(typeof(TransformCardinal), str);
			}
			else
			{
				_transformCardinal = TransformCardinal.OneToOne;
			}

			// then a collection of  attribute mappings
			_attributeMappings = (AttributeMappingCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _attributeMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
	
			// then a collection of default values
			_defaultValues = (DefaultValueCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _defaultValues.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

            // then script for select source rows
            if (parent.ChildNodes.Count > 2)
            {
                _selectSrcRowScript = (SelectRowScript)MappingNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[2]);
            }
            else
            {
                _selectSrcRowScript = new SelectRowScript();
            }

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _selectSrcRowScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

            // then script for identify a destination row
            if (parent.ChildNodes.Count > 3)
            {
                _identifyRowScript = (IdentifyRowScript)MappingNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[3]);
            }
            else
            {
                _identifyRowScript = new IdentifyRowScript();
            }

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _identifyRowScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write ClassMapping instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _sourceClassName
			if (_sourceClassName != null && _sourceClassName.Length > 0)
			{
				parent.SetAttribute("Source", _sourceClassName);
			}

			// write the _sourceClassIndex
			if (_sourceClassIndex > 0)
			{
				parent.SetAttribute("SourceIdx", _sourceClassIndex + "");
			}
		
			// write the _destinationClassName
			if (_destinationClassName != null && _destinationClassName.Length > 0)
			{
				parent.SetAttribute("Destination", _destinationClassName);
			}

            // write the _isChecked
            if (!_isChecked)
            {
                parent.SetAttribute("Checked", "false"); // default is true
            }

			// write the _transformCardinal, default is OneToOne
			if (_transformCardinal != TransformCardinal.OneToOne)
			{
				parent.SetAttribute("Cardinal", Enum.GetName(typeof(TransformCardinal), _transformCardinal));
			}

			// write the attribute mappings
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_attributeMappings.NodeType));
			_attributeMappings.Marshal(child);
			parent.AppendChild(child);

			// write default values
			child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_defaultValues.NodeType));
			_defaultValues.Marshal(child);
			parent.AppendChild(child);

            // write the selectSrcRowScript
            if (_selectSrcRowScript != null)
            {
                child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_selectSrcRowScript.NodeType));
                _selectSrcRowScript.Marshal(child);
                parent.AppendChild(child);
            }

            // write the identifyDstRowScript
            if (_identifyRowScript != null)
            {
                child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_identifyRowScript.NodeType));
                _identifyRowScript.Marshal(child);
                parent.AppendChild(child);
            }
		}

		/// <summary>
		/// Clear the transformer cached in the mappings in the previouse action
		/// </summary>
		internal void UnsetTransformers()
		{
			foreach (MappingNodeBase mapping in _attributeMappings)
			{
				mapping.Transformer = null;
			}

            RowSelector = null;
            RowIdentifier = null;
		}

		/// <summary>
		/// Append the transform scripts defined in the class mapping to the given
		/// StringBuilder
		/// </summary>
		/// <param name="builder">The StringBuilder instance</param>
		internal void AppendScripts(StringBuilder builder)
		{
			// append the scripts defined in attribute mappings to the
			// StringBuilder
			foreach (ITransformable mapping in _attributeMappings)
			{
				if (mapping.Script != null && mapping.ScriptEnabled)
				{
					builder.Append(mapping.Script);
				}
			}

            if (_selectSrcRowScript.Enabled && !string.IsNullOrEmpty(_selectSrcRowScript.Script))
            {
                builder.Append(_selectSrcRowScript.Script);
            }

            if (_identifyRowScript.Enabled && !string.IsNullOrEmpty(_identifyRowScript.Script))
            {
                builder.Append(_identifyRowScript.Script);
            }
		}

        /// <summary>
        /// Gets the information indicating whether a given row in a DataTable is selected for trandformation
        /// </summary>
        /// <returns>true if the row is selected, false otherwise.</returns>
        private bool IsRowSelected(DataTable dt, int rowIndex, Assembly assembly)
        {
            bool isSelected = true;

            if (_selectSrcRowScript != null &&
                _selectSrcRowScript.Enabled)
            {
                if (RowSelector == null)
                {
                    string typeName = NewteraNameSpace.TRANSFORMER_NAME_SPACE + "." + _selectSrcRowScript.ClassType;
                    RowSelector = (TransformerBase)assembly.CreateInstance(typeName);
                }

                try
                {
                    // execute the script
                    isSelected = RowSelector.IsRowSelected(dt, rowIndex);
                }
                catch (Exception ex)
                {
                    throw new MappingException("Encounter erros while executing " + _selectSrcRowScript.ClassType + ":" + ex.Message);
                }
            }

            return isSelected;
        }

        /// <summary>
        /// Indentify a row in the destination table in which to set the values of transformed data record
        /// </summary>
        /// <returns>0 or positive row index indicating an existing row, -1 indicating that no row is identified.</returns>
        private int IdentifyDstRow(DataTable srcDataTable, int srcRowIndex, DataTable dstDataTable, Assembly assembly)
        {
            int identifiedRowIndex = -1;

            if (_identifyRowScript != null &&
                _identifyRowScript.Enabled)
            {
                if (RowIdentifier == null)
                {
                    string typeName = NewteraNameSpace.TRANSFORMER_NAME_SPACE + "." + this._identifyRowScript.ClassType;
                    RowIdentifier = (TransformerBase)assembly.CreateInstance(typeName);
                }

                try
                {
                    // execute the script
                    identifiedRowIndex = RowIdentifier.IdentifyRow(srcDataTable, srcRowIndex, dstDataTable);
                }
                catch (Exception ex)
                {
                    throw new MappingException("Encounter erros while executing " + _identifyRowScript.ClassType + ":" + ex.Message);
                }
            }

            return identifiedRowIndex;
        }

		/// <summary>
		/// Transform one source row to one destination row using the transformations
		/// defined in Attribute Mapping list.
		/// </summary>
		/// <param name="srcDataRow">The DataRow instance</param>
		/// <param name="dstDataSet">The destination DataSet instance</param>
		/// <param name="rowIndex">The zero-based row index</param>
		/// <param name="assembly">An assembly containing transformer classes.</param>
		private void TransformRow(DataRow srcDataRow, DataSet dstDataSet, int rowIndex,
			Assembly assembly)
		{
            bool isExistingRow = IsExistingRow(dstDataSet, rowIndex);

            if (!isExistingRow)
            {
                // create empty data row(s) in destination DataSet
                CreateEmptyRow(dstDataSet, _dstDataView);

                // set default values to the data row first
                SetDefaultValues(_defaultValues, dstDataSet, _dstDataView, rowIndex);

                // if there exists overriding default values, override them
                if (_overridingDefaultValues != null &&
                    _overridingDefaultValues.Count > 0)
                {
                    SetDefaultValues(_overridingDefaultValues, dstDataSet, _dstDataView, rowIndex);
                }
            }

			// perform data transforms defined in the mappings
			foreach (ITransformable mapping in _attributeMappings)
			{
				AttributeSetterCollection setters = mapping.DoTransform(srcDataRow,
					_srcDataView, dstDataSet, _dstDataView, rowIndex, assembly);

				foreach (IAttributeSetter setter in setters)
				{
					setter.AssignValue();
				}
			}

			// For array attributes, we need to convert a collection of
			// transformed cell values to a combined string value
			foreach (IDataViewElement element in _dstDataView.ResultAttributes)
			{
				if (element is DataArrayAttribute)
				{
					DataRow dstDataRow = dstDataSet.Tables[_dstDataView.BaseClass.Name].Rows[rowIndex];
					string arrayValue = ((DataArrayAttribute) element).ConvertCellValuesToString();
					if (arrayValue != null)
					{
						dstDataRow[element.Name] = arrayValue;

						// set this flag so that the update query can include this value
						element.IsValueChanged = true;
					}
				}
			}
		}

        private bool IsExistingRow(DataSet ds, int rowIndex)
        {
            bool isExisting = false;

            if (rowIndex < ds.Tables[_dstDataView.BaseClass.Name].Rows.Count)
            {
                isExisting = true;
            }

            return isExisting;
        }

		/// <summary>
		/// Transform and merge the source row to the single destination row using
		/// the transformations defined in Attribute Mapping list.
		/// </summary>
		/// <param name="srcDataRow">The DataRow instance</param>
		/// <param name="dstDataSet">The destination DataSet instance</param>
		/// <param name="rowIndex">The zero-based row index</param>
		/// <param name="assembly">An assembly containing transformer classes.</param>
		private void MergeRow(DataRow srcDataRow, DataSet dstDataSet, int rowIndex,
			Assembly assembly)
		{
			DataTable dstDataTable = dstDataSet.Tables[_dstDataView.BaseClass.Name];

			// create an empty data row(s) in destination DataTable if the table is empty
			if (dstDataTable.Rows.Count == 0)
			{
				CreateEmptyRow(dstDataSet, _dstDataView);
			}

			// set default values to the destination row at the first time
			if (rowIndex == 0)
			{
				SetDefaultValues(_defaultValues, dstDataSet, _dstDataView, rowIndex);
			}

			// perform data transforms defined in the mappings
			foreach (ITransformable mapping in _attributeMappings)
			{
				AttributeSetterCollection setters = mapping.DoTransform(srcDataRow,
					_srcDataView, dstDataSet, _dstDataView, 0, assembly);

				foreach (IAttributeSetter setter in setters)
				{
					setter.AssignValue();
				}
			}

			// For array attributes, add values of each source row as new array row in the
			// destination row.
			foreach (IDataViewElement element in _dstDataView.ResultAttributes)
			{
				if (element is DataArrayAttribute)
				{
					DataRow dstDataRow = dstDataSet.Tables[_dstDataView.BaseClass.Name].Rows[0];
					string arrayValue = ((DataArrayAttribute) element).ConvertCellValuesToString();
					if (arrayValue != null)
					{
						string val = dstDataRow[element.Name].ToString();
						if (val != null && val.Length > 0)
						{
							// append an array row
							dstDataRow[element.Name] += DataArrayAttribute.DELIMETER + arrayValue;
						}
						else
						{
							dstDataRow[element.Name] = arrayValue;
						}

						// set this flag so that the update query can include this value
						element.IsValueChanged = true;
					}
				}
			}
		}

		/// <summary>
		/// Perform the data selection defined in Attribute Mapping list
		/// using a data row from the source DataSet.
		/// </summary>
		/// <param name="srcDataRow">The DataRow instance</param>
		/// <param name="dstDataSet">The destination DataSet instance</param>
		/// <param name="assembly">An assembly containing transformer classes.</param>
		private void SelectRow(DataRow srcDataRow, DataSet dstDataSet, Assembly assembly)
		{
			// perform data transforms defined in the mappings
			foreach (ITransformable mapping in _attributeMappings)
			{
				AttributeSetterCollection setters = mapping.DoTransform(srcDataRow,
					_srcDataView, dstDataSet, _dstDataView, 0, assembly);

				foreach (IAttributeSetter setter in setters)
				{
					setter.AssignValue();
				}
			}

			// For array attributes, we will first convert a set of array cell values
			// representing a array row to a combined string value, append the new row
			// to the existing value of DataArrayAttribute object
			foreach (IDataViewElement element in _dstDataView.ResultAttributes)
			{
				if (element is DataArrayAttribute)
				{
					DataRow dstDataRow = dstDataSet.Tables[_dstDataView.BaseClass.Name].Rows[0];
					string arrayValue = ((DataArrayAttribute) element).ConvertCellValuesToString();
					if (arrayValue != null)
					{
						string val = dstDataRow[element.Name].ToString();
						if (val != null && val.Length > 0)
						{
							// append an array row
							dstDataRow[element.Name] += DataArrayAttribute.DELIMETER + arrayValue;
						}
						else
						{
							dstDataRow[element.Name] = arrayValue;
						}

						// set this flag so that the update query can include this value
						element.IsValueChanged = true;
					}
				}
			}
		}

		/// <summary>
		/// Create an empty row in corresponding destination DataTable to store the
		/// transformed data. For each relationship in the destination DataViewModel
		/// we also need to create a new DataRow in a related DataTable to store
		/// primary key values.
		/// </summary>
		/// <param name="dstDataSet">The destination DataSet instance</param>
		/// <param name="dstDataView">The destination DataViewModel instance</param>
		private void CreateEmptyRow(DataSet dstDataSet, DataViewModel dstDataView)
		{
			DataTable dstDataTable = dstDataSet.Tables[dstDataView.BaseClass.Name];

			// create a DataRow instance for destination DataTable
			DataRow dstDataRow = dstDataTable.NewRow();

			// set the attachment count to zero
			dstDataRow[NewteraNameSpace.ATTACHMENTS] = "0";

			dstDataTable.Rows.Add(dstDataRow);

			// create a data row in the DataTable representing the related class
			// pointed by each relationship attribute in the destination DataViewModel
			foreach (IDataViewElement element in dstDataView.ResultAttributes)
			{
				if (element.ElementType == ElementType.RelationshipAttribute)
				{
					DataRelationshipAttribute relationship = (DataRelationshipAttribute) element;

					// The mapping from source attribute to destination relationship attribute
					// is established thorugh the primary key(s) of the relationship attribute.
					if (relationship.PrimaryKeyCount > 0)
					{
						// create a new row in the specific DataTable instance for storing
						// the primary key values. The name of the DataTable consists of the name of class and relationship attribute which is created by BuildDataSetVisitor.
						DataTable dataTable = dstDataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(this.DestinationClassName, relationship.Name)];
						DataRow pkRow = dataTable.NewRow();
						dataTable.Rows.Add(pkRow);
					}
				}
			}
		}

		/// <summary>
		/// Set a default values to a destination attributes in destination data row
		/// </summary>
		/// <param name="defaultValues">The DefaultValueCollection instance</param>
		/// <param name="dstDataSet">The destination DataSet instance</param>
		/// <param name="dstDataView">The destination DataViewModel instance</param>
		/// <param name="rowIndex">The current row index</param>
		private void SetDefaultValues(DefaultValueCollection defaultValues,
			DataSet dstDataSet, DataViewModel dstDataView, int rowIndex)
		{
			DataRow dstDataRow = dstDataSet.Tables[dstDataView.BaseClass.Name].Rows[rowIndex];
			DefaultValue defaultValue;

			foreach (IDataViewElement element in dstDataView.ResultAttributes)
			{
				switch (element.ElementType)
				{
					case ElementType.SimpleAttribute:
                        defaultValue = this.GetDefaultValue(defaultValues, element.Name);
						if (defaultValue != null && (defaultValue.AppliedRowIndex < 0 || defaultValue.AppliedRowIndex == rowIndex))
						{
							dstDataRow[element.Name] = defaultValue.Value;
							element.IsValueChanged = true;
						}

						break;

					case ElementType.ArrayAttribute:
                        defaultValue = this.GetDefaultValue(defaultValues, element.Name);
                        if (defaultValue != null && (defaultValue.AppliedRowIndex < 0 || defaultValue.AppliedRowIndex == rowIndex))
						{
							dstDataRow[element.Name] = defaultValue.Value;
							element.IsValueChanged = true;
						}

						break;

                    case ElementType.VirtualAttribute:

                        break;

                    case ElementType.ImageAttribute:

                        break;

					case ElementType.RelationshipAttribute:

						DataRelationshipAttribute relationship = (DataRelationshipAttribute) element;

						// The mapping from source attribute to destination relationship attribute
						// is established thorugh the primary key(s) of the relationship attribute.
						if (relationship.PrimaryKeyCount > 0)
						{
							// The name of the DataTable is the name
							// of the relationship attribute which is created by BuildDataSetVisitor.
							DataRow pkRow = dstDataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(this.DestinationClassName, relationship.Name)].Rows[rowIndex];

							foreach (DataSimpleAttribute pk in relationship.PrimaryKeys)
							{
								// name of mapping for primary key in format of
								// relationshipName_primaryKeyName
								string pkFullName = relationship.GetUniquePKName(pk.Name);

                                defaultValue = this.GetDefaultValue(defaultValues, pkFullName);

                                if (defaultValue != null && (defaultValue.AppliedRowIndex < 0 || defaultValue.AppliedRowIndex == rowIndex))
								{
									// set the default value
									pkRow[pk.Name] = defaultValue.Value;

									// set this flag so that the update query can include this value
									pk.IsValueChanged = true;

									element.IsValueChanged = true;
								}
							}
						}

						break;
				}
			}
		}
	}
}