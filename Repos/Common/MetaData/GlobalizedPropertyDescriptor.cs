/*
* @(#)GlobalizedPropertyDescriptor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Resources;
	using System.Reflection;

	/// <summary>
	/// GlobalizedPropertyDescriptor enhances the base class by obtaining the display name for a property
	/// from the resource.
	/// </summary>
	/// <version> 1.0.0 15 Oct 2004 </version>
	/// <author> Yong Zhang</author> 
	public class GlobalizedPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor _basePropertyDescriptor;
		private string _localizedCategory = null;
		private string _localizedName = null;
		private string _localizedDescription = null;

		/// <summary>
		/// Instantiate an instance of GlobalizedPropertyDescriptor
		/// </summary>
		/// <param name="basePropertyDescriptor"></param>
		public GlobalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor) : base(basePropertyDescriptor)
		{
			this._basePropertyDescriptor = basePropertyDescriptor;
		}

		/// <summary>
		/// Indicate whether the property value can be reset
		/// </summary>
		/// <param name="component">The component</param>
		/// <returns>true if it can be reset, false otherwise</returns>
		public override bool CanResetValue(object component)
		{
			return _basePropertyDescriptor.CanResetValue(component);
		}

		/// <summary>
		/// Gets the component type
		/// </summary>
		public override Type ComponentType
		{
			get {
				return _basePropertyDescriptor.ComponentType; 
			}
		}

		/// <summary>
		/// Get the globalized category name of the property from a resource file
		/// </summary>
		public override string Category
		{
			get
			{
				if (this._localizedCategory == null)
				{
					// Build the resource string table name. This sample uses the class name 
					// prefixed by the namespace. 
					string tableName = _basePropertyDescriptor.ComponentType.Namespace + "." + 
						_basePropertyDescriptor.ComponentType.Name;

					// Build the resource identifier. This sample uses the default category name   
					string category = _basePropertyDescriptor.Category;
	                  
					string s = null;
  
					try
					{
						// Now use resource table name and string id to access the resources.  
						ResourceManager rm = new ResourceManager(
							tableName, _basePropertyDescriptor.ComponentType.Assembly);

						// Get the string from the resources. 
						// append the "Category" to resource id to avoid clash with
						// property name
						s = rm.GetString(category + "Category");
					}
					catch (Exception)
					{
					}
	                    
					// Store the localized category
					_localizedCategory = (s != null)? s : category;
				}

				return _localizedCategory;
			}
		}


		/// <summary>
		/// Get the globalized display name of the property from a resource file
		/// </summary>
		public override string DisplayName
		{
			get 
			{
				if (this._localizedName == null)
				{
					// Build the resource string table name. This sample uses the class name 
					// prefixed by the namespace. 
					string tableName = _basePropertyDescriptor.ComponentType.Namespace + "." + 
						_basePropertyDescriptor.ComponentType.Name;

					// Build the resource identifier. This sample uses the default property name   
					string displayName = _basePropertyDescriptor.DisplayName;
	                  
					string s = null;
  
					try
					{
						// Now use resource table name and string id to access the resources.  
						ResourceManager rm = new ResourceManager(
							tableName, _basePropertyDescriptor.ComponentType.Assembly);

						// Get the string from the resources. 
						// If this fails, then use default display name (usually the property name) 
						s = rm.GetString(displayName);
					}
					catch (Exception)
					{
					}
	                    
					// Store the localized display name 
					_localizedName = (s!=null)? s : displayName;
				}

				return _localizedName;
			}
		}

		/// <summary>
		/// Get the globalized description of the property from a resource file
		/// </summary>
		public override string Description
		{
			get
			{
				if (this._localizedDescription == null)
				{
					string displayName;
					string tableName = _basePropertyDescriptor.ComponentType.Namespace + "." + _basePropertyDescriptor.ComponentType.Name;

					// If no display name id is specified by attribute, then construct it by using default description 
					displayName = this._basePropertyDescriptor.DisplayName + "Description";
				
					string s = null;
  
					try
					{
						// Now use table name and display name id to access the resources.  
						ResourceManager rm = new ResourceManager(tableName,_basePropertyDescriptor.ComponentType.Assembly);

						// Get the string from the resources. 
						// If this fails, then use default empty string indictating 'no description' 
						s = rm.GetString(displayName);
					}
					catch (Exception)
					{
					}

					_localizedDescription = (s!=null)? s : ""; 
				}

				return _localizedDescription;
			}
		}

		/// <summary>
		/// Gets the property value
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object GetValue(object component)
		{
			return this._basePropertyDescriptor.GetValue(component);
		}

		/// <summary>
		/// Is it read only property
		/// </summary>
		public override bool IsReadOnly
		{
			get { return this._basePropertyDescriptor.IsReadOnly; }
		}

		/// <summary>
		/// Get property name
		/// </summary>
		public override string Name
		{
			get { return this._basePropertyDescriptor.Name; }
		}

		/// <summary>
		/// Gets the property type
		/// </summary>
		public override Type PropertyType
		{
			get { return this._basePropertyDescriptor.PropertyType; }
		}

		/// <summary>
		/// Resets the property value
		/// </summary>
		/// <param name="component"></param>
		public override void ResetValue(object component)
		{
			this._basePropertyDescriptor.ResetValue(component);
		}

		/// <summary>
		/// Inidicate whether the property should be serialized
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool ShouldSerializeValue(object component)
		{
			return this._basePropertyDescriptor.ShouldSerializeValue(component);
		}

		/// <summary>
		/// Sets the property value
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public override void SetValue(object component, object value)
		{
			this._basePropertyDescriptor.SetValue(component, value);
		}
	}
}