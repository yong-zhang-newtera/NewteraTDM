/*
* @(#)DataRowView.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Data;
	using System.ComponentModel;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a view for a DataRow in a DataTable. This is mainly for
	/// rendering an DataRow data in a PropertyGrid.
	/// </summary>
	/// <version>1.0.1 15 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class DataRowView : ICustomTypeDescriptor
	{
		private PropertyDescriptorCollection _propertyDescriptors;
		private DataTable _dataTable;
		private int _selectedIndex;

		/// <summary>
		/// Initiating an instance of DataRowView class
		/// </summary>
		/// <param name="dataTable">The DataTable containing the DataRow to view</param>
		/// <remarks>The first DataRow is selected by default</remarks>
		public DataRowView(DataTable dataTable)
		{
			_dataTable = dataTable;
			_selectedIndex = 0;
			_propertyDescriptors = null;
		}

		/// <summary>
		/// Gets or sets the data table that contains the data rows.
		/// </summary>
		/// <value>The DataSet</value>
		public DataTable DataTable
		{
			get
			{
				return _dataTable;
			}
			set
			{
				_dataTable = value;
				// Reset the SelectedIndex
				SelectedIndex = 0;
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
				if (value >= 0 && value < _dataTable.Rows.Count)
				{

					_selectedIndex = value;

					if (_propertyDescriptors != null)
					{
						foreach (DataRowPropertyDescriptor descriptor in _propertyDescriptors)
						{
							descriptor.SelectedIndex = value;
						}
					}
				}
			}
		}

		/// <summary>
		/// Create a PropertyDescriptorCollection base on the columns
		/// in the data table.
		/// </summary>
		/// <returns>a PropertyDescriptorCollection</returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(null);
			DataRowPropertyDescriptor pd;

			foreach (DataColumn column in _dataTable.Columns)
			{
				pd = new DataRowPropertyDescriptor(column.ColumnName,
					null, _dataTable, _selectedIndex);

				propertyDescriptors.Add(pd);
			}

			return propertyDescriptors;
		}

		#region ICustomTypeDescriptor Members

		public TypeConverter GetConverter()
		{
			// return null reference
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		public string GetComponentName()
		{
			// return null reference
			return null;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _dataTable;
		}

		public AttributeCollection GetAttributes()
		{
			// return an empty attribute collection
			return new AttributeCollection(null);
		}

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

		public object GetEditor(Type editorBaseType)
		{
			// no editor for this object, return a null reference
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			// no default property, return a null reference
			return null;
		}

		public EventDescriptor GetDefaultEvent()
		{
			// no default event, return null reference
			return null;
		}

		public string GetClassName()
		{
			return "DataRowView";
		}

		#endregion
	}
}