/*
* @(#)ArrayDataRowView.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Data;
	using System.ComponentModel;


	/// <summary>
	/// Represents a view for Array Data Row of an Array Attribute.
	/// This is mainly for rendering a sub properties of an array data row
	/// in an editing UI form.
	/// </summary>
	/// <version>1.0.1 29 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class ArrayDataRowView : ICustomTypeDescriptor
	{
		private PropertyDescriptorCollection _propertyDescriptors;
		private string _rowName;
		private DataTable _dataTable;
		private int _rowIndex;

		/// <summary>
		/// Initiating an instance of ArrayDataRowView class
		/// </summary>
		/// <param name="rowName">The unique name of the row.</param>
		/// <param name="dataTable">The DataTable contains the row.</param>
		/// <param name="rowIndex">The index of row in the DataTable.</param>
		public ArrayDataRowView(string rowName, DataTable dataTable, int rowIndex)
		{
			_propertyDescriptors = null;
			_rowName = rowName;
			_dataTable = dataTable;
			_rowIndex = rowIndex;
		}

		/// <summary>
		/// Create a PropertyDescriptorCollection base on the data cells in the data row.
		/// </summary>
		/// <returns>a PropertyDescriptorCollection</returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(null);
			ArrayDataCellPropertyDescriptor pd;

			for (int i = 0; i < _dataTable.Columns.Count; i++)
			{
				// the name of the property is combination of row name plus column index
				// to make it unique.
				pd = new ArrayDataCellPropertyDescriptor(_rowName + "_" + i, null, _dataTable, _rowIndex, i);

				propertyDescriptors.Add(pd);
			}

			return propertyDescriptors;
		}

		/// <summary>
		/// Gets a string representing the object
		/// </summary>
		/// <returns>A string</returns>
		public override string ToString()
		{
			string val = "";

			for (int i = 0; i < _dataTable.Columns.Count; i++)
			{
				if (i > 0)
				{
					val += ";";
				}

				val += _dataTable.Rows[_rowIndex][i].ToString();
			}

			return val;
		}

		#region ICustomTypeDescriptor Members

		/// <summary>
		/// Get converter of this object
		/// </summary>
		/// <returns>The converter of TypeConverter, or null </returns>
		public TypeConverter GetConverter()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Gets the events.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Gets the events
		/// </summary>
		/// <returns></returns>
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Gets the component name.
		/// </summary>
		/// <returns></returns>
		public string GetComponentName()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Gets property owner
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _dataTable;
		}

		/// <summary>
		/// Gets the attributes of the objects
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			// return an empty attribute collection
			return new AttributeCollection(null);
		}

		/// <summary>
		/// Gets properties of the object
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

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			if (_propertyDescriptors == null)
			{
				_propertyDescriptors = GetPropertyDescriptors();
			}

			return _propertyDescriptors;
		}

		/// <summary>
		/// Gets the Editor of the object
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns></returns>
		public object GetEditor(Type editorBaseType)
		{
			// no editor for this object, return a null reference
			return null;
		}

		/// <summary>
		/// Gets the default property
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetDefaultProperty()
		{
			// no default property, return a null reference
			return null;
		}

		/// <summary>
		/// Get default event
		/// </summary>
		/// <returns></returns>
		public EventDescriptor GetDefaultEvent()
		{
			// no default event, return null reference
			return null;
		}

		/// <summary>
		/// Get class name of the object
		/// </summary>
		/// <returns></returns>
		public string GetClassName()
		{
			return "ArrayDataRowView";
		}

		#endregion
	}
}