/*
* @(#)ArrayDataTableView.cs
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
	/// Represents a view for value of Array Data Table.
	/// This is mainly for rendering a sub properties of an array attribute
	/// in an editing UI form.
	/// </summary>
	/// <version>1.0.1 29 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class ArrayDataTableView : ICustomTypeDescriptor
	{
		private PropertyDescriptorCollection _propertyDescriptors;
		private string _attributeName;
		private InstanceData _instanceData;

		/// <summary>
		/// Initiating an instance of ArrayDataTableView class
		/// </summary>
		public ArrayDataTableView(string attributeName,
			InstanceData instanceData)
		{
			_propertyDescriptors = null;
			_attributeName = attributeName;
			_instanceData = instanceData;
		}

		/// <summary>
		/// Gets or sets a DataTable instance storing array values
		/// </summary>
		/// <value>A DataTable instance.</value>
		public DataTable ArrayAttributeValue
		{
			get
			{
				return (DataTable) _instanceData.GetAttributeValue(_attributeName);
			}
			set
			{
				_instanceData.SetAttributeValue(_attributeName, value);
			}
		}

		/// <summary>
		/// Create a PropertyDescriptorCollection base on the data rows in the data table.
		/// </summary>
		/// <returns>a PropertyDescriptorCollection</returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(null);
			ArrayDataRowPropertyDescriptor pd;

			DataTable dataTable = (DataTable) _instanceData.GetAttributeValue(_attributeName);
			if (dataTable != null)
			{
				for (int index = 0; index < dataTable.Rows.Count; index ++)
				{
					// the name of the property is combination of names of array attribute and row index
					// to make it unique
					pd = new ArrayDataRowPropertyDescriptor(_attributeName + "_" + index, null, dataTable, index);

					propertyDescriptors.Add(pd);
				}
			}

			return propertyDescriptors;
		}

		/// <summary>
		/// Gets string representing the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _instanceData.GetAttributeStringValue(_attributeName);
		}

		#region ICustomTypeDescriptor Members

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public TypeConverter GetConverter()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public string GetComponentName()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _instanceData;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			// return an empty attribute collection
			return new AttributeCollection(null);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
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
		/// Refer to ICustomTypeDescriptor
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
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns></returns>
		public object GetEditor(Type editorBaseType)
		{
			// no editor for this object, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetDefaultProperty()
		{
			// no default property, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public EventDescriptor GetDefaultEvent()
		{
			// no default event, return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public string GetClassName()
		{
			return "ArrayDataTableView";
		}

		#endregion
	}
}