/*
* @(#)ArrayDataCellPropertyDescriptor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
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

	/// <summary>
	/// Represents a class that provides properties for DataRow instances in a
	/// DataTable. This is a supporting class for ArrayAttributeView class.
	/// </summary>
	/// <version>1.0.1 29 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class ArrayDataCellPropertyDescriptor : PropertyDescriptor
	{
		private DataTable _dataTable;
		private int _rowIndex;
		private int _colIndex;

		/// <summary>
		/// Initiating an instance of ArrayDataCellPropertyDescriptor class
		/// </summary>
		public ArrayDataCellPropertyDescriptor(string name, Attribute[] attributes,
			DataTable dataTable, int rowIndex, int colIndex) : base(name, attributes)
		{
			_dataTable = dataTable;
			_rowIndex = rowIndex;
			_colIndex = colIndex;
		}

		/// <summary>
		/// Gets the type of the component this property is bound to.
		/// </summary>
		/// <value>Type of an InstanceData</value>
		public override Type ComponentType
		{
			get
			{
				return typeof(DataColumn);
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
				return base.Converter;
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
				return true;
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
		/// Gets the display name of the property
		/// </summary>
		public override string DisplayName
		{
			get
			{
				return _dataTable.Columns[_colIndex].ColumnName;
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
				return true;
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
				
				return type;
			}
		}

		/// <summary>
		/// Returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>true if resetting the component changes its value; otherwise, false.</returns>
		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Gets an editor of the specified type.
		/// </summary>
		/// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
		/// <returns>An instance of the requested editor type, or a null reference </returns>
		public override object GetEditor(Type editorBaseType)
		{			
			return base.GetEditor (editorBaseType);
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>The value of a property for a given component. return an empty string if the value is null</returns>
		public override object GetValue(object component)
		{
			if (!_dataTable.Rows[_rowIndex].IsNull(_colIndex))
			{
				return _dataTable.Rows[_rowIndex][_colIndex].ToString();
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
		/// Resets the value for this property of the component to the default value
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component)
		{
		}

		/// <summary>
		/// Sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value. </param>
		public override void SetValue(object component, object value)
		{
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
	}
}