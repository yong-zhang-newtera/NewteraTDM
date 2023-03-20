/*
* @(#)RelationshipPrimaryKeyViewConverter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text.RegularExpressions;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Conversion between a RelationshipPrimaryKeyView object and string
	/// </summary>
	/// <version>  1.0.1 10 Nov. 2003</version>
	/// <author> Yong Zhang</author>
	public class RelationshipPrimaryKeyViewConverter : ExpandableObjectConverter
	{
		private RelationshipAttributeElement _schemaModelElement;
		private DataRelationshipAttribute _relationshipAttribute;
		private InstanceData _instanceData;

		/// <summary>
		/// Initiating an instance of RelationshipPrimaryKeyViewConverter class
		/// </summary>
		public RelationshipPrimaryKeyViewConverter(DataRelationshipAttribute relationshipAttribute,
			RelationshipAttributeElement schemaModelElement,
			InstanceData instanceData) : base()
		{
			_relationshipAttribute = relationshipAttribute;
			_schemaModelElement = schemaModelElement;
			_instanceData = instanceData;
		}

		/// <summary> 
		/// Let clients know it can convert a RelationshipPrimaryKeyView to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a RelationshipPrimaryKeyView object to a string type
			// but not from string to a RelationshipPrimaryKeyView
			if (sourceType == typeof(RelationshipPrimaryKeyView))
			{
				return true;
			}
			else if (sourceType == typeof(string))
			{
				return false;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		/// <summary>
		/// Convert from a RelationshipPrimaryKeyView object to string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			object converted = "";

			// if the source value is a RelationshipPrimaryKeyView type
			if (value is RelationshipPrimaryKeyView)
			{
				// Convert to string
				if (destinationType == typeof(string))
				{
					converted = ((RelationshipPrimaryKeyView) value).ToString();
				}
			}

			return converted;
		}

		/// <summary>
		/// Convert from string to a RelationshipPrimaryKeyView
		/// </summary>
		/// <param name="context"></param>
		/// <param name="info"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo info, object value)
		{
			object converted = null;

			if (value is String)
			{
				// if there are multiple primary keys, the values of primary keys
				// are separated by "&"
				SetPrimaryKeyValues(_instanceData, (string) value);

				// create a RelationshipPrimaryKeyView
				converted = new RelationshipPrimaryKeyView(_relationshipAttribute,
					_schemaModelElement, _instanceData);
			}

			return converted;
		}

		/// <summary>
		/// Tricks to allow changing values of child properties trigger the update
		/// on value of the parent property
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			// always force a new instance
			return true;
		}

		/// <summary>
		/// Retrun an instance of RelationshipPrimaryKeyView
		/// </summary>
		/// <param name="context"></param>
		/// <param name="propertyValues"></param>
		/// <returns></returns>
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			// propertyValues contains name/value pairs for the subproperties of
			// RelationshipPrimaryKeyView. Get the new values from propertyValues
			// and set them to the primary keys of a relationship attribute
			DataViewElementCollection primaryKeys = _relationshipAttribute.PrimaryKeys;
			if (primaryKeys != null)
			{
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					// property name consists of names of relationship and primary key
					pk.AttributeValue = System.Convert.ToString(propertyValues[_relationshipAttribute.Name + "_" + pk.Name]);
				}
			}

			return new RelationshipPrimaryKeyView(_relationshipAttribute,
				_schemaModelElement, _instanceData);
		}

		/// <summary>
		/// Parse the string containing primary key values separated by char
		/// and set the new values to the instance data
		/// </summary>
		/// <param name="instanceData">The instance data</param>
		/// <param name="primaryKeyValues">The primary key values</param>
		private void SetPrimaryKeyValues(InstanceData instanceData, string primaryKeyValues)
		{
			// TODO: Compile a regular expression to find objId
			string[] pkValues = _relationshipAttribute.SplitPKValues(primaryKeyValues);

			DataViewElementCollection primaryKeys = _relationshipAttribute.PrimaryKeys;
			if (primaryKeys != null && pkValues != null)
			{
				int i = 0;
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					string pkValue = "";

					if (i < pkValues.Length)
					{
						pkValue = pkValues[i].Trim();
					}

					// to set a pk value, the name combines that of relationship attribute and primary key
					instanceData.SetAttributeValue(_relationshipAttribute.GetUniquePKName(pk.Name), pkValue);

					i++;
				}
			}
		}
	}
}