/*
* @(#)DataRowPropertyDescriptor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Threading;
	using System.Reflection;
	using System.Runtime.Remoting;
	using System.Data;
	using System.ComponentModel;
	using System.Drawing.Design;

	/// <summary>
	/// Represents a property descriptor for a column of DataRow instance.
	/// </summary>
	/// <version>1.0.1 15 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class DataRowPropertyDescriptor : PropertyDescriptor
	{
		private DataTable _dataTable;
		private int _selectedIndex;

		/// <summary>
		/// Initiating an instance of DataRowPropertyDescriptor class
		/// </summary>
		public DataRowPropertyDescriptor(string name, Attribute[] attributes,
			DataTable dataTable, int selectedIndex) : base(name, attributes)
		{
			_dataTable = dataTable;
			_selectedIndex = selectedIndex;
		}

		/// <summary>
		/// Gets or sets the selected row index of the DataTable.
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				_selectedIndex = value;
			}
		}

		/// <summary>
		/// Gets the category which the simple attribute belongs,
		/// </summary>
		/// <value>The category of the attribute, it can be null</value>
		public override string Category
		{
			get
			{				
				return null;
			}
		}

		/// <summary>
		/// Gets the type of the component this property is bound to.
		/// </summary>
		/// <value>Type of a DataTable</value>
		public override Type ComponentType
		{
			get
			{
				return _dataTable.GetType();
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
		/// Gets the description of the property
		/// </summary>
		/// <value>The description of an instance attribute</value>
		public override string Description
		{
			get
			{
				return "";
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
				return this.Name;
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
		/// Gets a value indicating whether this property is read-only
		/// </summary>
		/// <value>false</value>
		public override bool IsReadOnly
		{
			get
			{	
				return false;
			}
		}

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		/// <value>A Type of string</value>
		public override Type PropertyType
		{
			get
			{				
				return typeof(string);
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
			object obj = _dataTable.Rows[_selectedIndex][this.Name];

			if (obj != null)
			{
				return obj;
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
			_dataTable.Rows[_selectedIndex][this.Name] = value;
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