/*
* @(#)DataSimpleAttribute.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
    using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A DataSimpleAttribute instance represents a simple attribute in a data view.
	/// It can appears in the result attribute collection or filters.
	/// </summary>
	/// <version>1.0.1 28 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class DataSimpleAttribute : DataViewElementBase, IQueryElement
	{
		private string _ownerClassAlias;
		private string _function;
		private string _value;

		/// <summary>
		/// Initiating an instance of DataSimpleAttribute class
		/// </summary>
		/// <param name="name">Name of the simple attribute</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns this attribute</param>
		public DataSimpleAttribute(string name, string ownerClassAlias) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_function = null;
			_value = null; // run-time use only
		}

		/// <summary>
		/// Initiating an instance of DataSimpleAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataSimpleAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_value = null; // run-time use only
		}

        /// <summary>
        /// Gets or sets an alias that is used to identifies the simple attribute when used
        /// in the search expression of data view.
        /// </summary>
        /// <value>A string, can be null.</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                {
                    // default alias
                    string alias = _ownerClassAlias + "_" + Name;
                    base.Alias = alias;
                }

                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.SimpleAttribute;
			}
		}

		/// <summary>
		/// Gets alias of class that owns this attribute
		/// </summary>
		/// <value>Owner class alias</value>
		public string OwnerClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the attribute has a function associated.
		/// </summary>
		/// <value>true if it has, false otherwise</value>
		public bool HasFunction
		{
			get
			{
				if (_function != null && _function.Length > 0)
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
		/// Gets or sets the aggregate function for the simple attribute.
		/// This allows to set functions such avg, sum, max, min, and count functions
		/// </summary>
		/// <value>The function name</value>
		public string Function
		{
			get
			{
				if (_function != null  && _function.Length > 0)
				{
					return _function;
				}
				else
				{
					return "";
				}			
			}
			set
			{
				_function = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the attribute has an value
		/// </summary>
		/// <value>true if it has, false, otherwise</value>
		public bool HasValue
		{
			get
			{
				if (_value != null && _value.Length > 0)
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
		/// Gets the information indicating whether the data type of attribute is
		/// numeric, including integer, decimal, float, double, etc.
		/// </summary>
		/// <value>true if it is a numeric attribute, false otherwise.</value>
		public bool IsNumeric
		{
			get
			{
				bool status = false;
				SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement) GetSchemaModelElement();
				DataType dataType = simpleAttributeElement.DataType;
				
				if (dataType == DataType.Decimal ||
					dataType == DataType.Double ||
					dataType == DataType.Float ||
					dataType == DataType.Integer)
				{
					status = true;
				}

				return status;
			}
		}

		/// <summary>
		/// Gets the information indicating whether value of the attribute is
		/// automatically incremented.
		/// </summary>
		/// <value>true if it is an auto-increment attribute, false otherwise.</value>
		public bool IsAutoIncrement
		{
			get
			{
				SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement) GetSchemaModelElement();
				
				return simpleAttributeElement.IsAutoIncrement;
			}
		}

        /// <summary>
        /// Gets information indicating whether this attribute is for edit history.
        /// </summary>
        /// <value>
        /// true if it is editing hsitory, false otherwise. Default is false.
        /// </value>
        public bool IsHistoryEdit
        {
            get
            {
                SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement)GetSchemaModelElement();

                return simpleAttributeElement.IsHistoryEdit;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute's value is rich text.
        /// </summary>
        /// <value>
        /// true if it is rich text, false otherwise. Default is false.
        /// </value>
        public bool IsRichText
        {
            get
            {
                SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement)GetSchemaModelElement();

                return simpleAttributeElement.IsRichText;
            }
        }

        /// <summary>
        /// Gets default value of an attribute.
        /// </summary>
        /// <value> The default value.
        /// </value>
        public string DefaultValue
        {
            get
            {
                SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement)GetSchemaModelElement();

                if (!simpleAttributeElement.IsSystemTimeDefault && !simpleAttributeElement.IsUidDefault)
                {
                    return simpleAttributeElement.DefaultValue;
                }
                else
                {
                    // default value is a function
                    return null;
                }
            }
        }


        /// <summary>
        /// Gets or sets information indicating whether the attribute can be updated manually.
        /// </summary>
        /// <value> return true if attribute can be updated manually through user interface, false
        /// indicating that the attribute can be updated in programs instead of user intefaces.
        /// The default is true.</value>
        /// <remarks>
        /// This setting only be effective on Web user interface.
        /// </remarks>
        public bool AllowManualUpdate
        {
            get
            {
                SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement)GetSchemaModelElement();

                return simpleAttributeElement.AllowManualUpdate;
            }
        }

		/// <summary>
		/// Gets or sets the value of a simple attribute.
		/// </summary>
		/// <value>The attribute value</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		public string AttributeValue
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the given value is different from
		/// the attribute value
		/// </summary>
		/// <param name="val">The given value</param>
		/// <returns>true if they are different, false otherwise.</returns>
		/// <remarks>If the give value is null or empty string, and the attribute
		/// value is null or empty string, they are considered to be same</remarks>
		public bool IsValueDifferent(string val)
		{
			string attributeValue = this.AttributeValue;
			if (attributeValue != null && attributeValue.Length == 0)
			{
				attributeValue = null;
			}

			if (val != null && val.Length == 0)
			{
				val = null;
			}

			if (attributeValue != val)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitSimpleAttribute(this);
		}
		
		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public override SchemaModelElement GetSchemaModelElement()
		{
			if (_schemaModelElement == null && DataView != null)
			{
				DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
                    ClassElement classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                    _schemaModelElement = classElement.FindInheritedSimpleAttribute(Name);
                }
			}

			return _schemaModelElement;
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of _ownerClassAlias member
			_ownerClassAlias = parent.GetAttribute("OwnerAlias");

			_function = parent.GetAttribute("Function");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _ownerName member
			parent.SetAttribute("OwnerAlias", _ownerClassAlias);

			if (_function != null && _function.Length > 0)
			{
				// Set the expression member
				parent.SetAttribute("Function", _function);
			}
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Parent.ToXPath() + "/" + this.Name + NewteraNameSpace.ATTRIBUTE_SUFFIX;
            }

            return _xpath;
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        public override IXaclObject Parent
        {
            get
            {
                ClassElement classElement = null;
                DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
                    classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                    if (classElement == null)
                    {
                        throw new Exception("Unable to find the class whose caption is " + ownerClass.Caption + " in the schema " + DataView.SchemaModel.SchemaInfo.Name);
                    }
                }
                else
                {
                    throw new Exception("Unable to find the class whose alias is " + _ownerClassAlias + " in a data view for class " + DataView.BaseClass.Caption);
                }

                // the parent is the owner class element
                return classElement;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			if (_function != null && _function.Length > 0)
			{
				query.Append(_function).Append("(");
			}

			query.Append("$").Append(_ownerClassAlias).Append("/").Append(Name);

			if (_function != null && _function.Length > 0)
			{
				query.Append(")");
			}

			return query.ToString();
		}

		#endregion
	}
}