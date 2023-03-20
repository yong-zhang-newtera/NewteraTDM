/*
* @(#)Parameter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;
    using System.Text;
    using System.Text.RegularExpressions;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a parameter appeared in search filters.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
    public class Parameter : DataViewElementBase, IQueryElement, IParameter
	{
		private string _ownerClassAlias;
		private DataType _dataType;
		private string _value;
        private bool _isPattern;
        private string _function;

        /// <summary>
        /// Split values separated by "&"
        /// </summary>
        /// <param name="valueString">Multiple value string</param>
        /// <returns>The split values in a string array, could be null</returns>
        public static string[] SplitValues(string valueString)
        {
            if (valueString != null && valueString.Length > 0)
            {
                Regex exp = new Regex("&");

                return exp.Split(valueString);
            }
            else
            {
                return null;
            }
        }
		/// <summary>
		/// Initiating an instance of Parameter class
		/// </summary>
		/// <param name="name">Name of an attribute that parameter is associated with</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns the attribute</param>
		/// <param name="dataType">Parameter's data type. One of DataType values</param>
		public Parameter(string name, string ownerClassAlias, DataType dataType) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_dataType = dataType;
			_value = null;
            _function = null;
		}

		/// <summary>
		/// Initiating an instance of Parameter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Parameter(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.Parameter;
			}
		}

		/// <summary>
		/// Gets an alias of the class that owns this parameter
		/// </summary>
		public string ClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}

		/// <summary>
		/// Gets or sets the value of Parameter
		/// </summary>
		/// <value>A value string.</value>
		public string ParameterValue
		{
			get
			{
				if (_value != null)
				{
					return _value;
				}
				else
				{
					return "";
				}
			}
			set
			{
				_value = value;

				// fire an event for parameter value change
				FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets or sets the function for the parameter value, such as upperCase
        /// </summary>
        /// <value>The function name</value>
        public string Function
        {
            get
            {
                if (!string.IsNullOrEmpty(_function))
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
		/// Gets the data type of a parameter
		/// </summary>
		/// <value>One of DataType values</value>
		public DataType DataType
		{
			get
			{
				return _dataType;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitParameter(this);
		}
		
		/// <summary>
		/// Gets or sets the schema model element that a parameter is associated with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public override SchemaModelElement GetSchemaModelElement()
		{
            if (_schemaModelElement == null &&
                DataView != null &&
                !string.IsNullOrEmpty(_ownerClassAlias))
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

			// set value of _value member
			_value = parent.GetAttribute("Value");

			// set the data type
			_dataType = (DataType) Enum.Parse(typeof(DataType), parent.GetAttribute("Type"));
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _ownerName member
            if (!string.IsNullOrEmpty(_ownerClassAlias))
            {
                parent.SetAttribute("OwnerAlias", _ownerClassAlias);
            }

			// write the value member
			if (_value != null && _value.Length > 0)
			{
				parent.SetAttribute("Value", _value);
			}

			parent.SetAttribute("Type", Enum.GetName(typeof(DataType), _dataType));
		}

		/// <summary>
		/// Clone a paramter with a empty value
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Parameter parameter = new Parameter(this.Name, this._ownerClassAlias, this._dataType);
			parameter.Caption = this.Caption;
			parameter.Description = this.Description;

			return parameter;
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			if (_value != null)
			{
				return _value;
			}
			else
			{
				return "";
			}
        }

        #region IParameter members

        /// <summary>
        /// Gets information indicating whether the parameter has value.
        /// </summary>
        /// <returns>true if the parameter has value, false otherwise.</returns>
        public bool HasValue
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// If the paramter's value is composed of multiple values separated by "&",
        /// this method return the same type of IParameter that contains a single value
        /// whose position is indicated by the index.
        /// </summary>
        /// <param name="index">The value index</param>
        /// <param name="name">Parameter name</param>
        /// <param name="dataType">The data type of parameter</param>
        /// <returns>An IParameter whose value is standalone, null if there isn't a value at the given index.</returns>
        public IParameter GetParameterByIndex(int index, string name, DataType dataType)
        {
            Parameter parameter = null;

            string[] values = Parameter.SplitValues(ParameterValue);
            if (values != null && index < values.Length)
            {
                parameter = new Parameter(name, this.ClassAlias, dataType);
                parameter.ParameterValue = values[index];
            }

            return parameter;
        }

        /// <summary>
        /// Escape the special characters contained in the string value
        /// </summary>
        /// <param name="oldValue">The unescaped value</param>
        /// <returns>The escaped value</returns>
        private string EscapeSpecialChars(string oldValue)
        {
            string escaped = oldValue;
            if (!string.IsNullOrEmpty(escaped))
            {
                // escape \ with \\
                int pos = escaped.IndexOf(@"\");
                if (pos >= 0)
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    for (int i = 0; i < escaped.Length; i++)
                    {
                        if (escaped[i] == '\\')
                        {
                            builder.Append(@"\\");
                        }
                        else
                        {
                            builder.Append(escaped[i]);
                        }
                    }

                    escaped = builder.ToString();
                }
            }

            return escaped;
        }

        #endregion

        #region IQueryElement Members

        /// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			if (_value != null && _value.Length > 0)
			{
                // if the avlue is one of the xquery built-in function, do not have quotes around it.
                if (!IsBuildinFunc(_value))
                {
                    switch (_dataType)
                    {
                        case DataType.String:
                        case DataType.Text:

                            StringBuilder query = new StringBuilder();
                            if (!string.IsNullOrEmpty(_function))
                            {
                                query.Append(_function).Append("(");
                            }

                            string val = EscapeSpecialChars(_value);
                            query.Append("\"").Append(val).Append("\"");

                            if (!string.IsNullOrEmpty(_function))
                            {
                                query.Append(")");
                            }

                            return query.ToString();
                            break;
                        case DataType.Date:
                        case DataType.DateTime:
                        case DataType.Boolean:
                            return "\"" + _value + "\"";
                        default:
                            return _value;
                    }
                }
                else
                {
                    return _value;
                }
			}
			else
			{
				return null;
			}
		}

        private bool IsBuildinFunc(string val)
        {
            bool status = false;

            if (val.StartsWith("currentUser"))
            {
                status = true;
            }
            else if (val.StartsWith("currentRoles"))
            {
                status = true;
            }

            return status;
        }

		#endregion
	}
}