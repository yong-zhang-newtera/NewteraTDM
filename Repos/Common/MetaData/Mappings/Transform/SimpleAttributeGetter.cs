/*
* @(#)SimpleAttributeGetter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings;

	/// <summary> 
	/// The class for getting a value to a SimpleAttribute
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	/// <author>Yong Zhang</author>
	internal class SimpleAttributeGetter : AttributeGetterBase
	{
		private string _srcAttributeName;

		/// <summary>
		/// Initiate an instance of SimpleAttributeGetter class
		/// </summary>
		/// <param name="srcDataRow">The source DataRow</param>
		/// <param name="srcDataView">The DataViewModel for source class</param>
		/// <param name="srcAttributeName"> The source attribute name</param>
		public SimpleAttributeGetter(DataRow srcDataRow,
			DataViewModel srcDataView, string srcAttributeName)
			: base(srcDataRow, srcDataView)
		{
			_srcAttributeName = srcAttributeName;
		}

		#region IAttributeGetter interface implementation
		
		/// <summary>
		/// Gets the type of setter
		/// </summary>
		/// <value>One of GetterType enum values</value>
		public override GetterType Type 
		{
			get
			{
				return GetterType.SimpleAttributeGetter;
			}
		}

		/// <summary>
		/// Get a value from an attribute.
		/// </summary>
		public override string GetValue()
		{
			string srcValue = null;

			if (_srcDataRow[_srcAttributeName] != null)
			{
				srcValue = _srcDataRow[_srcAttributeName].ToString();
			}

			return srcValue;
		}

		#endregion

	}
}