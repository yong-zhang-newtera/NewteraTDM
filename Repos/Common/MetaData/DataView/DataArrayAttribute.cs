/*
* @(#)DataArrayAttribute.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Data;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A DataArrayAttribute instance represents an array attribute in a data view.
	/// It can appears in the result attribute collection.
	/// </summary>
	/// <version>1.0.1 10 Aug 2004</version>
	/// <author>Yong Zhang</author>
	public class DataArrayAttribute : DataViewElementBase, IQueryElement
	{
		/// <summary>
		/// Array element delimiter definition
		/// </summary>
		public const string DELIMETER = ";";
		/// <summary>
		/// Array cell default value, 0
		/// </summary>
		public const string DEFAULT_VALUE = "";

		private string _ownerClassAlias;
		private string _value;
		private ArrayList _cellValues;

		/// <summary>
		/// Initiating an instance of DataArrayAttribute class
		/// </summary>
		/// <param name="name">Name of the array attribute</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns this attribute</param>
		public DataArrayAttribute(string name, string ownerClassAlias) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_value = null; // run-time use only
			_cellValues = null; // run-time use only
		}

		/// <summary>
		/// Initiating an instance of DataArrayAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataArrayAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_value = null; // run-time use only
			_cellValues = null; // run-time use only
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.ArrayAttribute;
			}
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
		/// Gets the information indicating whether the data type of array element is
		/// numeric, including integer, decimal, float, double, etc.
		/// </summary>
		/// <value>true if it is a numeric attribute, false otherwise.</value>
		public bool IsNumeric
		{
			get
			{
				bool status = false;
				ArrayAttributeElement arrayAttrElement = (ArrayAttributeElement) this.GetSchemaModelElement();
				DataType dataType = arrayAttrElement.ElementDataType;
				
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
		/// Gets or sets the value of a array attribute.
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
		/// Gets the array of values of an array attribute.
		/// </summary>
		/// <value>The attribute value</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		public string[] AttributeValues
		{
			get
			{
				if (_value != null && _value.Length > 0)
				{
					return ParseArrayValues(_value);
				}
				else
				{
                    string[] values = new string[1];
                    values[0] = "";
                    return values;
				}
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
		/// Add an empty row to the array
		/// </summary>
		public void AddEmptyRow()
		{
			StringBuilder builder = new StringBuilder();

			// append an empty row
			ArrayAttributeElement arrayAttrElement = (ArrayAttributeElement) this.GetSchemaModelElement();
			for (int i = 0; i < arrayAttrElement.ColumnCount; i++)
			{
				if (i > 0)
				{
					builder.Append(DELIMETER);
				}

				builder.Append(DEFAULT_VALUE);
			}

			if (this.HasValue)
			{
				this._value += DELIMETER + builder.ToString();
			}
			else
			{
				this._value = builder.ToString();
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitArrayAttribute(this);
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
                    _schemaModelElement = classElement.FindInheritedArrayAttribute(Name);
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
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

		/// <summary>
		/// Get an array cell value.
		/// </summary>
		/// <param name="arrayData">The array data string</param>
		/// <param name="rowIndex">Array row index</param>
		/// <param name="colIndex">Array col index</param>
		internal string GetCellValue(string arrayData, int rowIndex, int colIndex)
		{
			string cellValue = null;

			if (arrayData != null && arrayData.Length > 0)
			{
				string[] arrayValues = ParseArrayValues(arrayData);
			
				ArrayAttributeElement arrayAttrElement = (ArrayAttributeElement) this.GetSchemaModelElement();

				int pos = rowIndex * arrayAttrElement.ColumnCount + colIndex;

				if (pos < arrayValues.Length)
				{
					cellValue = arrayValues[pos];
				}
			}

			return cellValue;
		}

		/// <summary>
		/// Keep an array cell value temporarily. It will not update
		/// the array value. It is mainly used during importing process.
		/// </summary>
		/// <param name="rowIndex">Array row index</param>
		/// <param name="colIndex">Array col index</param>
		/// <param name="cellValue">Cell value</param>
		internal void KeepCellValue(int rowIndex, int colIndex, string cellValue)
		{
			if (_cellValues == null)
			{
				_cellValues = new ArrayList();
			}

			ArrayCellValue arrayCellValue = new ArrayCellValue();
			arrayCellValue._rowIndex = rowIndex;
			arrayCellValue._colIndex = colIndex;
			arrayCellValue._value = cellValue;

			_cellValues.Add(arrayCellValue);
		}

		/// <summary>
		/// Convert the kept cell values into a delimiter separated string and
		/// return it.
		/// </summary>
		internal string ConvertCellValuesToString()
		{
			string arrayValue = null;
			if (_cellValues != null)
			{
				// create an DataTable to hold the cell value for easy
				// manipulation
				DataTable dataTable = new DataTable();
				// add columns to datatable
				ArrayAttributeElement arrayAttrElement = (ArrayAttributeElement) this.GetSchemaModelElement();
				for (int i = 0; i < arrayAttrElement.ColumnCount; i++)
				{
					dataTable.Columns.Add("Col" + i); // column name is insignificant
				}
				// get the max row count so that we can add rows to the datatable
				int rowCount = 0;
				foreach (ArrayCellValue cellValue in _cellValues)
				{
					if ((cellValue._rowIndex + 1) > rowCount)
					{
						rowCount = cellValue._rowIndex + 1;
					}
				}

				// add rows to datatable
				for (int i = 0; i < rowCount; i++)
				{
					DataRow dataRow = dataTable.NewRow();

					dataTable.Rows.Add(dataRow);
				}

				// set the cell values to the DataTable
				foreach (ArrayCellValue cellValue in _cellValues)
				{
					dataTable.Rows[cellValue._rowIndex][cellValue._colIndex] = cellValue._value;
				}

				// concatenate the cell values together with DataArrayAttribute.Delimiter
				StringBuilder builder = new StringBuilder();

				for (int row = 0; row < dataTable.Rows.Count; row ++)
				{
					for (int col = 0; col < dataTable.Columns.Count; col ++)
					{
						if (row > 0 || col > 0)
						{
							builder.Append(DataArrayAttribute.DELIMETER);
						}

						if (!dataTable.Rows[row].IsNull(col))
						{
							builder.Append(dataTable.Rows[row][col].ToString());
						}
						else
						{
							builder.Append(DEFAULT_VALUE); // zero as default
						}
					}
				}

				arrayValue = builder.ToString();
			}

			return arrayValue;
		}

		/// <summary>
		/// Parse a string representing array values separated by delimiter
		/// </summary>
		/// <param name="arrayDataString">An array value string</param>
		/// <returns>A string array contains parsed array values</returns>
		internal string[] ParseArrayValues(string arrayDataString)
		{
			Regex regex = new Regex(DELIMETER);

			string[] arrayValues = regex.Split(arrayDataString);

			// fix the values by assigning default values for those empty cells
			for (int i = 0; i < arrayValues.Length; i++)
			{
				// set the default values to empty cell
				if (arrayValues[i].Length == 0)
				{
					arrayValues[i] = DataArrayAttribute.DEFAULT_VALUE;
				}
			}

			return arrayValues;
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

			query.Append("$").Append(_ownerClassAlias).Append("/").Append(Name);

			return query.ToString();
		}

		#endregion
	}

	internal class ArrayCellValue
	{
		internal int _rowIndex;
		internal int _colIndex;
		internal string _value;
	}
}