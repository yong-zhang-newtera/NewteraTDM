/*
* @(#)ArrayDataCellGetter.cs
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
	internal class ArrayDataCellGetter : AttributeGetterBase
	{
		private string _srcAttributeName;
		private int _cellRow;
		private int _cellCol;

		/// <summary>
		/// Initiate an instance of ArrayDataCellGetter class
		/// </summary>
		/// <param name="srcDataRow">The source DataRow</param>
		/// <param name="srcDataView">The DataViewModel for source class</param>
		/// <param name="srcAttributeName"> The source attribute name</param>
		/// <param name="cellRow">The array cell row index</param>
		/// <param name="cellCol">The array cell col index</param>
		public ArrayDataCellGetter(DataRow srcDataRow,
			DataViewModel srcDataView, string srcAttributeName,
			int cellRow, int cellCol)
			: base(srcDataRow, srcDataView)
		{
			_srcAttributeName = srcAttributeName;
			_cellRow = cellRow;
			_cellCol = cellCol;
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
				return GetterType.ArrayDataCellGetter;
			}
		}

		/// <summary>
		/// Get a value from an attribute.
		/// </summary>
		public override string GetValue()
		{
			string srcValue = null;

			if (_srcDataView != null)
			{
				string arrayData = _srcDataRow[this._srcAttributeName].ToString();
				IDataViewElement element = _srcDataView.ResultAttributes[_srcAttributeName];
				if (element == null && !(element is DataArrayAttribute))
				{
					throw new MappingException("Unable to find DataArrayAttribute element for " + _srcAttributeName + " during transformation.");
				}

				DataArrayAttribute arrayAttribute = (DataArrayAttribute) element;

				// get array cell data
				srcValue = arrayAttribute.GetCellValue(arrayData, _cellRow, _cellCol);
			}

			return srcValue;
		}

		#endregion

	}
}