/*
* @(#)RelationshipPrimaryKeyView.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Data;
	using System.ComponentModel;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a view for primary keys of a relationship attribute.
	/// This is mainly for rendering a sub properties of a relationship attribute
	/// in an editing UI form.
	/// </summary>
	/// <version>1.0.1 11 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class RelationshipPrimaryKeyView : ICustomTypeDescriptor
	{
		private PropertyDescriptorCollection _propertyDescriptors;
		private RelationshipAttributeElement _schemaModelElement;
		private DataRelationshipAttribute _relationshipAttribute;
		private InstanceData _instanceData;

		/// <summary>
		/// Initiating an instance of RelationshipPrimaryKeyView class
		/// </summary>
		public RelationshipPrimaryKeyView(DataRelationshipAttribute relationshipAttribute,
			RelationshipAttributeElement schemaModelElement,
			InstanceData instanceData)
		{
			_propertyDescriptors = null;
			_relationshipAttribute = relationshipAttribute;
			_schemaModelElement = schemaModelElement;
			_instanceData = instanceData;
		}

		/// <summary>
		/// Gets the relationship attribute that provides data for this view.
		/// </summary>
		/// <value>The DataSet</value>
		public DataRelationshipAttribute RelationshipAttribute
		{
			get
			{
				return _relationshipAttribute;
			}
		}

		/// <summary>
		/// Create a PropertyDescriptorCollection base on the result attributes
		/// in the data view.
		/// </summary>
		/// <returns>a PropertyDescriptorCollection</returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			ClassElement classElement = _schemaModelElement.LinkedClass;
			PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(null);
			InstanceAttributePropertyDescriptor pd;

			DataViewElementCollection primaryKeys = _relationshipAttribute.PrimaryKeys;
			if (primaryKeys != null)
			{
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					AttributeElementBase schemaModelElement = classElement.FindInheritedSimpleAttribute(pk.Name);

					// the name of the property is combination of names of relationship and primary key
					pd = new InstanceAttributePropertyDescriptor(_relationshipAttribute.Name + "_" + pk.Name, null, pk,
						schemaModelElement, _instanceData, _relationshipAttribute.IsReadOnly);

					pd.IsPrimaryKey = true;

					propertyDescriptors.Add(pd);
				}
			}

			return propertyDescriptors;
		}

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string val = "";

			DataViewElementCollection primaryKeys = _relationshipAttribute.PrimaryKeys;
			if (primaryKeys != null)
			{
				int index = 0;
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					val += pk.AttributeValue;

					if (index < primaryKeys.Count - 1 && !string.IsNullOrEmpty(val))
					{
						val += "&";
					}
					index++;
				}
			}

			return val;
		}

		#region ICustomTypeDescriptor Members

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public TypeConverter GetConverter()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// return an empty event collection
			return EventDescriptorCollection.Empty;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public string GetComponentName()
		{
			// return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _instanceData;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			// return an empty attribute collection
			return new AttributeCollection(null);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
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
		/// Refer to ICustomTypeDescriptor specification.
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
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns></returns>
		public object GetEditor(Type editorBaseType)
		{
			// no editor for this object, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetDefaultProperty()
		{
			// no default property, return a null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public EventDescriptor GetDefaultEvent()
		{
			// no default event, return null reference
			return null;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification.
		/// </summary>
		/// <returns></returns>
		public string GetClassName()
		{
			return "RelationshipPrimaryKeyView";
		}

		#endregion
	}
}