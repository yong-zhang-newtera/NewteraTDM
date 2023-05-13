/*
* @(#)InstanceAttributePropertyDescriptor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Threading;
	using System.Reflection;
	using System.Runtime.Remoting;
	using System.Data;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Core;

	/// <summary>
	/// Represents a class that provides properties for data of result attributes
	/// in a data view results.
	/// </summary>
	/// <version>1.0.1 09 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class InstanceAttributePropertyDescriptor : PropertyDescriptor
	{
		private IDataViewElement _instanceAttribute;
		private AttributeElementBase _schemaModelElement;
		private InstanceData _instanceData;
		private bool _isReadOnly;
		private bool _isPrimaryKey;
        private bool _allowManualUpdate;
        private string _size;
        private string _settings;

		/// <summary>
		/// Initiating an instance of InstanceAttributePropertyDescriptor class
		/// </summary>
		public InstanceAttributePropertyDescriptor(string name, Attribute[] attributes,
			IDataViewElement instanceAttribute, AttributeElementBase schemaModelElement,
			InstanceData instanceData, bool isReadOnly) : base(name, attributes)
		{
			_instanceAttribute = instanceAttribute;
			_schemaModelElement = schemaModelElement;
			_instanceData = instanceData;
			_isReadOnly = isReadOnly;
			_isPrimaryKey = false;
            _allowManualUpdate = true;
            _size = null;
            _settings = null;
		}

		/// <summary>
		/// Gets the category which the simple attribute belongs,
		/// </summary>
		/// <value>The category of the attribute, it can be null</value>
		public override string Category
		{
			get
			{
				string category = null;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					ArrayAttributeElement arrayAttribute = _schemaModelElement as ArrayAttributeElement;
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

					if (simpleAttribute != null)
					{
						if (simpleAttribute.Category != null && simpleAttribute.Category.Length > 0)
						{
							category = simpleAttribute.Category;
						}
					}
					else if (arrayAttribute != null)
					{
						if (arrayAttribute.Category != null && arrayAttribute.Category.Length > 0)
						{
							category = arrayAttribute.Category;
						}
					}
                    else if (imageAttribute != null)
                    {
                        if (imageAttribute.Category != null && imageAttribute.Category.Length > 0)
                        {
                            category = imageAttribute.Category;
                        }
                    }
				}
				
				return category;
			}
		}

		/// <summary>
		/// Gets the section which the simple attribute belongs,
		/// </summary>
		/// <value>The section of the attribute, it can be null</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public string Section
		{
			get
			{
				string section = null;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					ArrayAttributeElement arrayAttribute = _schemaModelElement as ArrayAttributeElement;
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

					if (simpleAttribute != null)
					{
						if (simpleAttribute.Section != null && simpleAttribute.Section.Length > 0)
						{
							section = simpleAttribute.Section;
						}
					}
					else if (arrayAttribute != null)
					{
						if (arrayAttribute.Section != null && arrayAttribute.Section.Length > 0)
						{
							section = arrayAttribute.Section;
						}
					}
                    else if (imageAttribute != null)
                    {
                        if (imageAttribute.Category != null && imageAttribute.Category.Length > 0)
                        {
                            section = imageAttribute.Section;
                        }
                    }
				}
				
				return section;
			}
		}

        /// <summary>
        /// Gets the database column name of the property
        /// </summary>
        /// <value>The db column name of the attribute, it can be null</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string ColumnName
        {
            get
            {
                string colName = null;

                if (_schemaModelElement != null)
                {
                    colName = _schemaModelElement.ColumnName;
                }

                return colName;
            }
        }

        /// <summary>
        /// Gets information indicating whether the property should be hidden from display
        /// </summary>
        /// <value>True to be hidden, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsHidden
        {
            get
            {
                bool status = false;

                if (_instanceAttribute.ElementType == ElementType.RelationshipAttribute &&
                    !((DataRelationshipAttribute) _instanceAttribute).ShowPrimaryKeys)
                {
                    status = true;
                }

                return status;
            }
        }


        /// <summary>
        /// Gets the information indicating whether the property value is required
        /// </summary>
        /// <value>true if it is required, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsRequired
		{
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					ArrayAttributeElement arrayAttribute = _schemaModelElement as ArrayAttributeElement;					
					RelationshipAttributeElement relationshipAttribute = _schemaModelElement as RelationshipAttributeElement;
					
                    if (simpleAttribute != null)
					{
						status = simpleAttribute.IsRequired;
					}
					else if (arrayAttribute != null)
					{
						status = arrayAttribute.IsRequired;
					}
					else if (relationshipAttribute != null)
					{
						status = relationshipAttribute.IsRequired;
					}
				}
				
				return status;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the property value has to be unique
		/// </summary>
		/// <value>true if it is unique, false otherwise.</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool IsUnique
		{
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					if (simpleAttribute != null)
					{
						status = simpleAttribute.IsUnique;
					}
				}
				
				return status;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the property is multipled lined,
		/// </summary>
		/// <value>True if it is multipled line, false otherwise</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool IsMultipleLined
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.IsMultipleLined;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property value is auto-incremental,
        /// </summary>
        /// <value>True if it is auto-incremental, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsAutoIncremental
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.IsAutoIncrement;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the constraint is used for restriction or suggestion
        /// </summary>
        /// <value>One of the ConstraintUsage enum values</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public ConstraintUsage ConstraintUsage
        {
            get
            {
                ConstraintUsage usage = ConstraintUsage.Restriction;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        usage = simpleAttribute.ConstraintUsage;
                    }
                }

                return usage;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute's value is rich text.
        /// </summary>
        /// <value>True if it is rich text, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsRichText
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.IsRichText;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property is for history edit.
        /// </summary>
        /// <value>True if it is for history edit, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsHistoryEdit
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.IsHistoryEdit;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property value is encrypted.
        /// </summary>
        /// <value>True if it is encrypted, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsEncrypted
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.IsEncrypted;
                    }
                }

                return status;
            }
        }

		/// <summary>
		/// Gets the number of rows to display of a multipled lined property
		/// </summary>
		/// <value>Number of rows</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public int Rows
		{
			get
			{
				int rows = 1; // default

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

					if (simpleAttribute != null)
					{
						rows = simpleAttribute.Rows;
					}
				}
				
				return rows;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the property value is a multiple choice.
		/// </summary>
		/// <value>true if it is a multiple choice, false otherwise.</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool IsMultipleChoice
		{
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					if (simpleAttribute != null &&
						simpleAttribute.IsMultipleChoice)
					{
						status = true;
					}
				}
				
				return status;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the property is for an array.
		/// </summary>
		/// <value>true if it is for an array, false otherwise.</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool IsArray
		{
			get
			{
				if (_instanceAttribute.ElementType == ElementType.ArrayAttribute)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether the property is virtual.
        /// </summary>
        /// <value>true if it is virtual, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsVirtual
        {
            get
            {
                if (_instanceAttribute.ElementType == ElementType.VirtualAttribute)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property is for an image.
        /// </summary>
        /// <value>true if it is for an image, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsImage
        {
            get
            {
                if (_instanceAttribute.ElementType == ElementType.ImageAttribute)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property value is html code
        /// </summary>
        /// <value>true if it is html, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsHtmlValue
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    VirtualAttributeElement virualAttribute = _schemaModelElement as VirtualAttributeElement;
                    if (virualAttribute != null)
                    {
                        status = virualAttribute.IsHtml;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the property is for a primary key,
        /// </summary>
        /// <value>True if it is for a primary key, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsPrimaryKey
		{
			get
			{	
				return _isPrimaryKey;
			}
			set
			{
				_isPrimaryKey = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the property can be updated manually via Web UI.
        /// </summary>
        /// <value>True if it can be updated manually, 
        /// false indicate the property can only be updated via programs,
        /// default to true.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool AllowManualUpdate
        {
            get
            {
                bool status = _allowManualUpdate;

                // If _allowManualUpdate has been set to false, do not check the status from
                // _schemaModelElement
                if (status && _schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        status = simpleAttribute.AllowManualUpdate;
                    }
                }

                return status;
            }
            set
            {
                _allowManualUpdate = value;
            }
        }

        /// <summary>
        /// Gets or sets the size which can be used to determine the width of Web UI control
        /// for the property.
        /// </summary>
        /// <value>integer string</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        /// <summary>
        /// Gets or sets the settings which can be used to change the behavious of Web UI control
        /// for the property.
        /// </summary>
        /// <value>setting string in form of "param1=value1&param2=value2&param3"</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property has an inupt mask.
        /// </summary>
        /// <value>True if it has an input mask, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool HasInputMask
        {
            get
            {
                bool status = false;

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && !string.IsNullOrEmpty(simpleAttribute.InputMask))
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the attribute's input mask.
        /// </summary>
        /// <value>A string representing an input mask, could be null.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string InputMask
        {
            get
            {
                string inputMask = "";

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && !string.IsNullOrEmpty(simpleAttribute.InputMask))
                {
                    inputMask = simpleAttribute.InputMask;
                }

                return inputMask;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property has a display format.
        /// </summary>
        /// <value>True if it has a display format, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool HasDisplayFormat
        {
            get
            {
                bool status = false;

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && !string.IsNullOrEmpty(simpleAttribute.DisplayFormatString))
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the attribute's display format
        /// </summary>
        /// <value>A string representing a display format, could be null.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string DisplayFormatString
        {
            get
            {
                string displayFormat = "";

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && !string.IsNullOrEmpty(simpleAttribute.DisplayFormatString))
                {
                    displayFormat = simpleAttribute.DisplayFormatString;
                }

                return displayFormat;
            }
        }

        /// <summary>
        /// Gets the information indicating whether to show update history of the property.
        /// </summary>
        /// <value>True if showing update history, false otherwise.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool ShowUpdateHistory
        {
            get
            {
                bool status = false;

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && simpleAttribute.ShowUpdateHistory)
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the attribute's valid value data source name.
        /// </summary>
        /// <value>A string representing a class name, could be null.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string DataSourceName
        {
            get
            {
                string dataSource = "";

                SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                if (simpleAttribute != null && !string.IsNullOrEmpty(simpleAttribute.DataSourceName))
                {
                    dataSource = simpleAttribute.DataSourceName;
                }

                return dataSource;
            }
        }

		/// <summary>
		/// Gets the min length of the property,
		/// </summary>
		/// <value>An integer, default is 0</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public int MinLength
		{
			get
			{
				int length = 0;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

					if (simpleAttribute != null)
					{
						length = simpleAttribute.MinLength;
					}
				}
				
				return length;
			}
		}

		/// <summary>
		/// Gets the max length of the property,
		/// </summary>
		/// <value>An integer, default is 100</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public int MaxLength
		{
			get
			{
				int length = 100;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

					if (simpleAttribute != null)
					{
						length = simpleAttribute.MaxLength;
					}
				}
				
				return length;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the property has a constraint, such
		/// EnumConstraint, RangeConstraint, or PatternConstraint,
		/// </summary>
		/// <value>True if it has a constraint, false otherwise</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool HasConstraint
		{
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
                    VirtualAttributeElement virtualAttribute = _schemaModelElement as VirtualAttributeElement;

					if (simpleAttribute != null && simpleAttribute.Constraint != null)
					{
						status = true;
					}
                    else if (virtualAttribute != null && virtualAttribute.Constraint != null)
                    {
                        status = true;
                    }
				}
				
				return status;
			}
		}

		/// <summary>
		/// Get the constraint associated with the property
		/// </summary>
		/// <value>An ConstraintElementBase object, possible types are EnumConstraint,
		/// RangeContraint, and PatternConstraint. null if the property doesn't have a constraint</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>		
		public ConstraintElementBase Constraint
		{
			get
			{
				ConstraintElementBase constraint = null;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
                    VirtualAttributeElement virtualAttribute = _schemaModelElement as VirtualAttributeElement;

                    if (simpleAttribute != null)
					{
						constraint = simpleAttribute.Constraint;
					}
                    else if (virtualAttribute != null)
                    {
                        constraint = virtualAttribute.Constraint;
                    }
                }
						
				return constraint;
			}
		}

		/// <summary>
		/// Get the default value of the property if it exists.
		/// </summary>
		/// <value>A string object, null if it doesn't have a default value.</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>		
		public string DefaultValue
		{
			get
			{
				string defaultValue = null;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

					if (simpleAttribute != null)
					{
						defaultValue = simpleAttribute.DefaultValue;
					}
				}
						
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether the attribute is shown as a progress bar
        /// </summary>
        /// <value> true if the attribute is shown as progress bar, false, otherwise. Default is false.
        /// </value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool ShowAsProgressBar
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null && simpleAttribute.ShowAsProgressBar)
                    {
                        status = true;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property is a relationship,
        /// </summary>
        /// <value>True if it is a relationship, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsRelationship
		{
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					RelationshipAttributeElement relationshipAttribute = _schemaModelElement as RelationshipAttributeElement;

					if (relationshipAttribute != null)
					{
						status = true;
					}
				}
				
				return status;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the relationship has a foreign key
        /// </summary>
        /// <value>True if it is a relationship and has a foreign key, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool IsForeignKeyRequired
        {
            get
            {
                bool status = false;

                if (_schemaModelElement != null)
                {
                    RelationshipAttributeElement relationshipAttribute = _schemaModelElement as RelationshipAttributeElement;

                    if (relationshipAttribute != null &&
                        relationshipAttribute.IsForeignKeyRequired)
                    {
                        status = true;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the referenced class name of a relationship attribute.
        /// </summary>
        /// <value>The name of the referenced class</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string ReferencedClassName
        {
            get
            {
                string className = null;

                if (_schemaModelElement != null)
                {
                    RelationshipAttributeElement relationshipAttribute = _schemaModelElement as RelationshipAttributeElement;

                    if (relationshipAttribute != null)
                    {
                        className = relationshipAttribute.LinkedClassName;
                    }
                }

                return className;
            }
        }

		/// <summary>
		/// Gets the information indicating whether the property's value is used
		/// for full-text search.
		/// </summary>
		/// <value>True if it is used for full-text search, false otherwise</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public bool IsFullTextSearchAttribute
        {
			get
			{
				bool status = false;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
                    ArrayAttributeElement arrayAttribute = _schemaModelElement as ArrayAttributeElement;

					if (simpleAttribute != null && simpleAttribute.IsFullTextSearchAttribute)
					{
						status = true;
					}
                    else if (arrayAttribute != null && arrayAttribute.IsFullTextSearchAttribute)
                    {
                        status = true;
                    }
				}
				
				return status;
			}
		}

        /// <summary>
        /// Gets the information indicating the UI type for selecting a related instance,
        /// </summary>
        /// <value>One of SelectionUIType enum, default is popup</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string UIControlCreator
        {
            get
            {
                string uiControlCreator = null;

                if (_schemaModelElement != null)
                {
                    AttributeElementBase attributeElement = _schemaModelElement as AttributeElementBase;

                    if (attributeElement != null)
                    {
                        uiControlCreator = attributeElement.UIControlCreator;
                    }
                }

                return uiControlCreator;
            }
        }

        /// <summary>
        /// Gets the information indicating wether to invoke callback function when the attribute's value is changed on the user interface,
        /// </summary>
        /// <value>True to invoke callback function, false otherwise</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public bool InvokeCallback
        {
            get
            {
                bool invokeCallback = false;

                if (_schemaModelElement != null)
                {
                    AttributeElementBase attributeElement = _schemaModelElement as AttributeElementBase;

                    if (attributeElement != null)
                    {
                        invokeCallback = attributeElement.InvokeCallback;
                    }
                }

                return invokeCallback;
            }
        }

		/// <summary>
		/// Gets the IDataViewElement instance associated with the property descriptor.
		/// </summary>
		/// <value>An IDataViewElement instance.</value>
		/// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
		public IDataViewElement DataViewElement
		{
			get
			{
				return _instanceAttribute;
			}
		}

        /// <summary>
        /// Gets a names of the cascaded properties if exists. Values of a cascaded property are
        /// dependent on the current value of this property
        /// </summary>
        /// <value>A string name, or null.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string CascadedPropertyNames
        {
            get
            {
                string propertyNames = null;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        if (!string.IsNullOrEmpty(simpleAttribute.CascadedAttributes))
                        {
                            propertyNames = simpleAttribute.CascadedAttributes;
                        }
                    }
                    else if (_schemaModelElement is RelationshipAttributeElement)
                    {
                        RelationshipAttributeElement relationshipElement = _schemaModelElement as RelationshipAttributeElement;

                        if (!string.IsNullOrEmpty(relationshipElement.CascadedAttributes))
                        {
                            propertyNames = relationshipElement.CascadedAttributes;
                        }
                    }
                }

                return propertyNames;
            }
        }

        /// <summary>
        /// Gets a name of the parent property on which the alues of a property are
        /// dependent
        /// </summary>
        /// <value>A string name, or null.</value>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string ParentPropertyName
        {
            get
            {
                string propertyName = null;

                if (_schemaModelElement != null)
                {
                    SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

                    if (simpleAttribute != null)
                    {
                        if (!string.IsNullOrEmpty(simpleAttribute.ParentAttribute))
                        {
                            propertyName = simpleAttribute.ParentAttribute;
                        }
                    }
                }

                return propertyName;
            }
        }

        /// <summary>
        /// Gets maximum height of displayed image.
        /// </summary>
        public int ImageMaximumHeight
        {
            get
            {
                int val = 0;

                if (_schemaModelElement != null)
                {
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

                    if (imageAttribute != null)
                    {
                        val = imageAttribute.MaximumHeight;
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Gets maximum width of displayed image.
        /// </summary>
        public int ImageMaximumWidth
        {
            get
            {
                int val = 0;

                if (_schemaModelElement != null)
                {
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

                    if (imageAttribute != null)
                    {
                        val = imageAttribute.MaximumWidth;
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Gets height of thumbnail image.
        /// </summary>
        public int ImageThumbnailHeight
        {
            get
            {
                int val = 0;

                if (_schemaModelElement != null)
                {
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

                    if (imageAttribute != null)
                    {
                        val = imageAttribute.ThumbnailHeight;
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Gets width of thumbnail image.
        /// </summary>
        public int ImageThumbnailWidth
        {
            get
            {
                int val = 0;

                if (_schemaModelElement != null)
                {
                    ImageAttributeElement imageAttribute = _schemaModelElement as ImageAttributeElement;

                    if (imageAttribute != null)
                    {
                        val = imageAttribute.ThumbnailWidth;
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Gets the type of the component this property is bound to.
        /// </summary>
        /// <value>Type of an InstanceData</value>
        public override Type ComponentType
		{
			get
			{
				return _instanceData.GetType();
			}
		}

		/// <summary>
		/// Gets the type converter for this property.
		/// </summary>
		/// <value>A TypeConverter that is used to convert the Type of this property.</value>
		public override TypeConverter Converter
		{
			get
			{
				if (_instanceAttribute.ElementType == ElementType.RelationshipAttribute)
				{
					return new RelationshipPrimaryKeyViewConverter((DataRelationshipAttribute) _instanceAttribute,
						(RelationshipAttributeElement) _schemaModelElement,
						_instanceData);
				}
				else if (_instanceAttribute.ElementType == ElementType.SimpleAttribute)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
					if (simpleAttribute != null && simpleAttribute.IsMultipleChoice)
					{
						return new MultipleChoiceValueConverter(this.PropertyType);
					}

					return base.Converter;
				}
				else if (_instanceAttribute.ElementType == ElementType.ArrayAttribute)
				{
					return new ArrayDataTableViewConverter();
				}
				else
				{
					return base.Converter;
				}
			}
		}

		/// <summary>
		/// Gets the description of the property
		/// </summary>
		/// <value>The description of an instance attribute</value>
		public override string Description
		{
			get
			{
				if (_instanceAttribute.Description != null)
				{
					return _instanceAttribute.Description;
				}
				else
				{
					return "";
				}
			}
		}

		/// <summary>
		/// Gets the name that can be displayed in a UI
		/// </summary>
		/// <value>The caption of an instance attribute</value>
		public override string DisplayName
		{
			get
			{
				return _instanceAttribute.Caption;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the member is browsable
		/// </summary>
		/// <value>Always true</value>
		public override bool IsBrowsable
		{
			get
			{
				bool status = true;

				if (_schemaModelElement != null)
				{
					AttributeElementBase attribute = _schemaModelElement as AttributeElementBase;

					if (attribute != null)
					{
						status = attribute.IsBrowsable;
					}
				}
				
				return status;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this property should be localized
		/// </summary>
		/// <value>The default value</value>
		public override bool IsLocalizable
		{
			get
			{
				return base.IsLocalizable;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this property is read-only
		/// </summary>
		/// <value>true if the property is read-only; otherwise, false</value>
		public override bool IsReadOnly
		{
			get
			{
				bool status = _isReadOnly;

				if (_schemaModelElement != null)
				{
					SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;

					if (simpleAttribute != null &&
                        (simpleAttribute.IsAutoIncrement || simpleAttribute.IsReadOnly))
					{
						status = true;
					}
				}

                if (!status && _instanceData != null)
                {
                    // check against the instance data to see if the attribute is set to read-only
                    // conditionally by access control rules
                    status = _instanceData.IsAttributeReadOnly(this.Name);
                }
				
				return status;
			}
		}

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		/// <value>A Type that represents the type of the property. Default is type of string</value>
		public override Type PropertyType
		{
			get
			{
				Type type = typeof(string);
				SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
                VirtualAttributeElement virtualAttribute = _schemaModelElement as VirtualAttributeElement;
                ArrayAttributeElement arrayAttribute = _schemaModelElement as ArrayAttributeElement;

				if (_schemaModelElement != null)
				{
					if (arrayAttribute != null)
					{
						type = typeof(DataTable);
					}
					else if (simpleAttribute != null && simpleAttribute.Constraint != null &&
						simpleAttribute.Constraint is IEnumConstraint)
					{
                        // create a dynamic enum type for this attribute
                        type = EnumTypeFactory.Instance.Create(_schemaModelElement);
					}
                    else if (virtualAttribute != null && virtualAttribute.Constraint != null &&
                            virtualAttribute.Constraint is IEnumConstraint)
                    {
                        // create a dynamic enum type for this attribute
                        type = EnumTypeFactory.Instance.Create(_schemaModelElement);
                    }
                    else
					{
						switch (_schemaModelElement.DataType)
						{
							case DataType.Boolean:
                                type = EnumTypeFactory.Instance.Create(_schemaModelElement);
								break;
							case DataType.Byte:
								type = typeof(byte);
								break;
							case DataType.Date:
								type = typeof(DateTime);
								break;
							case DataType.DateTime:
								type = typeof(DateTime);
								break;						
							case DataType.Decimal:
								type = typeof(decimal);
								break;
							case DataType.Double:
								type = typeof(double);
								break;
							case DataType.Float:
								type = typeof(float);
								break;
							case DataType.Integer:
								type = typeof(int);
								break;
							case DataType.BigInteger:
								type = typeof(Int64);
								break;
							case DataType.String:
								type = typeof(string);
								break;
							case DataType.Text:
								type = typeof(string);
								break;
						}
					}
				}

				return type;
			}
		}

        /// <summary>
        /// Gets the data type of the property.
        /// </summary>
        /// <value>One of the DataType enum values</value>
        public DataType DataType
        {
            get
            {
                DataType dataType = DataType.String;

                if (_schemaModelElement != null)
                {
                    dataType = _schemaModelElement.DataType;
                }

                return dataType;
            }
        }

        /// <summary>
        /// Returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>true if resetting the component changes its value; otherwise, false.</returns>
        public override bool CanResetValue(object component)
		{
			return true;
		}

		/// <summary>
		/// Gets an editor of the specified type.
		/// </summary>
		/// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
		/// <returns>An instance of the requested editor type, or a null reference </returns>
		public override object GetEditor(Type editorBaseType)
		{
			if (_instanceAttribute.ElementType == ElementType.RelationshipAttribute)
			{
				if (editorBaseType == typeof(UITypeEditor))
				{
					// Create an instance of Newtera.Studio.RelationshipValueEditor class using
					// reflection so that there is no compilation dependency between
					// these two assemblies. But if the names of either Studio assembly
					// or class RelationshipValueEditor changes, this will break.
					AppDomain appDomain = Thread.GetDomain();
                    try
                    {
                        ObjectHandle objHandle = appDomain.CreateInstance("Studio", "Newtera.Studio.RelationshipValueEditor");
                        return objHandle.Unwrap();
                    }
                    catch
                    {
                        // when showing instance in the application other than DesignStudio,
                        // we will not show a special editor for the relationship attribute
                        base.GetEditor(editorBaseType);
                    }
				}
			}
			else if (_instanceAttribute.ElementType == ElementType.SimpleAttribute)
			{
				SimpleAttributeElement simpleAttribute = _schemaModelElement as SimpleAttributeElement;
				if (simpleAttribute != null)
				{
					if (editorBaseType == typeof(UITypeEditor))
					{
						if (simpleAttribute.IsMultipleChoice)
						{
							// create a drop-down UI Type Editor that display enum values
							// as checkboxes
							AppDomain appDomain = Thread.GetDomain();
                            ObjectHandle objHandle = appDomain.CreateInstance("Newtera.WindowsControl", "Newtera.WindowsControl.MultipleChoiceEditor");
							return objHandle.Unwrap();
						}
						else if (simpleAttribute.IsMultipleLined)
						{
							// create a drop-down UI Type Editor that display a text area with
							// multiple row.
							AppDomain appDomain = Thread.GetDomain();
                            ObjectHandle objHandle = appDomain.CreateInstance("Newtera.WindowsControl", "Newtera.WindowsControl.MultipleLineTextEditor");
							return objHandle.Unwrap();
						}
					}
				}
			}
			else if (_instanceAttribute.ElementType == ElementType.ArrayAttribute)
			{
				if (editorBaseType == typeof(UITypeEditor))
				{
                    // Create an instance of Newtera.WindowsControl.ArrayValueEditor class using
					// reflection so that there is no compilation dependency between
                    // these two assemblies. But if the names of either Newtera.WindowsControl assembly
					// or class ArrayValueEditor changes, this will break.
					AppDomain appDomain = Thread.GetDomain();
                    ObjectHandle objHandle = appDomain.CreateInstance("Newtera.WindowsControl", "Newtera.WindowsControl.ArrayValueEditor");
					return objHandle.Unwrap();
				}
			}
            /*
            else if (_instanceAttribute.ElementType == ElementType.ImageAttribute)
            {
                if (editorBaseType == typeof(UITypeEditor))
                {
                    // Create an instance of Newtera.WindowsControl.ImageAttributeEditor class using
                    // reflection so that there is no compilation dependency between
                    // these two assemblies. But if the names of either Newtera.WindowsControl assembly
                    // or class ImageAttributeEditor changes, this will break.
                    //Note: do not reference to Studio.exe application, WorkflowStudio or SmartWord
                    // clients will not work since its bin dir doesn't contain Studio.ex
                    AppDomain appDomain = Thread.GetDomain();
                    ObjectHandle objHandle = appDomain.CreateInstance("Newtera.WindowsControl", "Newtera.WindowsControl.ImageAttributeEditor");
                    return objHandle.Unwrap();
                }
            }
            */
			
			return base.GetEditor (editorBaseType);
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>The value of a property for a given component. return an empty string if the value is null</returns>
		public override object GetValue(object component)
		{
			if (_instanceAttribute.ElementType == ElementType.SimpleAttribute)
			{
				return _instanceData.GetAttributeValue(this.Name);
			}
            if (_instanceAttribute.ElementType == ElementType.VirtualAttribute)
            {
                return _instanceData.GetAttributeValue(this.Name);
            }
			else if (_instanceAttribute.ElementType == ElementType.ArrayAttribute)
			{
				return new ArrayDataTableView(_instanceAttribute.Name,
					_instanceData);
			}
            if (_instanceAttribute.ElementType == ElementType.ImageAttribute)
            {
                return _instanceData.GetAttributeValue(this.Name);
            }
			else if (_instanceAttribute.ElementType == ElementType.RelationshipAttribute)
			{
				return new RelationshipPrimaryKeyView((DataRelationshipAttribute) _instanceAttribute,
					(RelationshipAttributeElement) _schemaModelElement,
					_instanceData);
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <returns>The value of a property.</returns>
		public object GetValue()
		{
			return GetValue(null);
		}

        /// <summary>
        /// Gets the current value of the property on a component.
        /// </summary>
        /// <returns>The value of a property.</returns>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string GetStringValue()
        {
            return _instanceData.GetAttributeStringValue(this.Name);
        }

		/// <summary>
		/// Resets the value for this property of the component to the default value
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component)
		{
			_instanceData.ResetAttributeValue(this.Name);
		}

		/// <summary>
		/// Sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value. </param>
		public override void SetValue(object component, object value)
		{
			// SetValue for ArrayAttribute is taken care of by ArrayDataTableView
			if (_instanceAttribute.ElementType != ElementType.ArrayAttribute &&
                _instanceAttribute.ElementType != ElementType.VirtualAttribute)
			{
				_instanceData.SetAttributeValue(this.Name, value);
			}
		}

        /// <summary>
        /// Sets the value of the component to a string value.
        /// </summary>
        /// <param name="value">The new string value. </param>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public void SetStringValue(string value)
        {
            // SetValue for ArrayAttribute is taken care of by ArrayDataTableView
            if (_instanceAttribute.ElementType != ElementType.ArrayAttribute &&
                _instanceAttribute.ElementType != ElementType.VirtualAttribute)
            {
                _instanceData.SetAttributeStringValue(this.Name, value);
            }
        }

		/// <summary>
		/// determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <param name="component">The component with the property to be examined for persistence.</param>
		/// <returns>Return false</returns>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

        /// <summary>
        /// Set the status of readonly, this method is here because IsReadOnly Property doesn't
        /// allow set
        /// </summary>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public void SetReadOnlyStatus(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Get a list filter value for the property
        /// </summary>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public string GetListFilterValue()
        {
            string filterValue = null;
            if (_schemaModelElement != null && _schemaModelElement is SimpleAttributeElement)
            {
                filterValue = ((SimpleAttributeElement)_schemaModelElement).GetListFilterValue();
            }

            return filterValue;
        }

        /// <summary>
        /// Set a list filter value for the property
        /// </summary>
        /// <remarks>This is a InstanceAttributePropertyDescriptor specific method</remarks>
        public void SetListFilterValue(string filterValue)
        {
            if (_schemaModelElement != null && _schemaModelElement is SimpleAttributeElement)
            {
                ((SimpleAttributeElement)_schemaModelElement).SetListFilterValue(filterValue);
            }
        }
	}
}