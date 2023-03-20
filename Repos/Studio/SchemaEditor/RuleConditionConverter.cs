/*
* @(#)RuleConditionConverter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Conversion between a IDataViewElement object representing a search
	/// expression and string
	/// </summary>
	/// <version>  1.0.0 13 Oct 2007</version>
	public class RuleConditionConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert a IDataViewElement to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a IDataViewElement object to a string type
			if (sourceType == typeof(IDataViewElement))
			{
				return true;
			}
			else if (sourceType == typeof(string))
			{
				return false;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Convert from a IDataViewElement to string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			// if the source value is a constraint type
			if (value != null && value is IDataViewElement)
			{
				string converted = "";
				
				FlattenSearchFiltersVisitor visitor = new FlattenSearchFiltersVisitor();

			
				((IDataViewElement) value).Accept(visitor);

				DataViewElementCollection flattenExprs = visitor.FlattenedSearchFilters;

				foreach (IDataViewElement element in flattenExprs)
				{
					converted += element.ToString();
				}

				return converted;
			}

			return "(None)";
			//return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Convert from string to a IDataViewElement
		/// </summary>
		/// <param name="context"></param>
		/// <param name="info"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo info, object value)
		{
			return base.ConvertFrom(context, info, value);
		}
	}
}