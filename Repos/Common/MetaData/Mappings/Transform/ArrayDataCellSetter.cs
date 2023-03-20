/*
* @(#)ArrayDataCellSetter.cs
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
	/// The class for setting a value to a cell of an ArrayAttribute.
	/// </summary>
	/// <version> 1.0.0 17 Nov 2004</version>
	/// <author>Yong Zhang</author>
	internal class ArrayDataCellSetter : AttributeSetterBase
	{
		private string _dstAttributeName;
		private int _cellRow;
		private int _cellCol;

		/// <summary>
		/// Initiate an instance of ArrayDataCellSetter class
		/// </summary>
		/// <param name="srcValue">The value from source</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="dstAttributeName"> The destination array attribute name</param>
		/// <param name="cellRow">The array cell row index</param>
		/// <param name="cellCol">The array cell col index</param>
		public ArrayDataCellSetter(string srcValue, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex, string dstAttributeName,
			int cellRow, int cellCol)
			: base(srcValue, dstDataSet, dstDataView, rowIndex)
		{
			_dstAttributeName = dstAttributeName;
			_cellRow = cellRow;
			_cellCol = cellCol;
		}

		#region IAttributeSetter interface implementation
		
		/// <summary>
		/// Gets the type of setter
		/// </summary>
		/// <value>One of SetterType enum values</value>
		public override SetterType Type 
		{
			get
			{
				return SetterType.ArrayDataCellSetter;
			}
		}

		/// <summary>
		/// Assign a value to an attribute.
		/// </summary>
		public override void AssignValue()
		{
			IDataViewElement element = _dstDataView.ResultAttributes[_dstAttributeName];
			if (element == null && !(element is DataArrayAttribute))
			{
				throw new MappingException("Unable to find DataArrayAttribute element for " + _dstAttributeName + " during transformation.");
			}

			DataArrayAttribute arrayAttribute = (DataArrayAttribute) element;

			// copy the source value as an array cell value
			arrayAttribute.KeepCellValue(_cellRow, _cellCol, _srcValue);
		}

		#endregion

	}
}