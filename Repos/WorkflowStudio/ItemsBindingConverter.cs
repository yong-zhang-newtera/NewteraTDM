/*
* @(#)ItemsBindingConverter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
    using System.Text;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.WFModel;
	
	/// <summary>
    /// Conversion between a ParameterBindingInfo and string
	/// </summary>
	/// <version>  1.0.0 16 Oct 2007</version>
    public class ItemsBindingConverter : ExpandableObjectConverter
	{
		/// <summary> 
		/// Let clients know it can convert a ParameterBindingInfo to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a ParameterBindingInfo object to a string type
			if (sourceType == typeof(ParameterBindingInfo))
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
		/// Convert from a ParameterBindingInfo to string
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
			if (value != null && value is ParameterBindingInfo)
			{
				StringBuilder builder = new StringBuilder();

                ParameterBindingInfo parameterBinding = (ParameterBindingInfo)value;

                builder.Append("Source=").Append(parameterBinding.SourceType);
                builder.Append(";Path=").Append(parameterBinding.Path);
                return builder.ToString();
			}

			return "(None)";
			//return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Convert from string to a ParameterBindingInfo
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