/*
* @(#)SimpleAttributeSetter.cs
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
	/// The class for setting a value to a SimpleAttribute
	/// </summary>
	/// <version> 1.0.0 17 Nov 2004</version>
	/// <author>Yong Zhang</author>
	internal class SimpleAttributeSetter : AttributeSetterBase
	{
		private string _dstAttributeName;

		/// <summary>
		/// Initiate an instance of SimpleAttributeSetter class
		/// </summary>
		/// <param name="srcValue">The value from source</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="dstAttributeName"> The destination attribute name</param>
		public SimpleAttributeSetter(string srcValue, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex, string dstAttributeName)
			: base(srcValue, dstDataSet, dstDataView, rowIndex)
		{
			_dstAttributeName = dstAttributeName;
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
				return SetterType.SimpleAttributeSetter;
			}
		}

		/// <summary>
		/// Assign a value to an attribute.
		/// </summary>
		public override void AssignValue()
		{
			DataRow dstDataRow = _dstDataSet.Tables[_dstDataView.BaseClass.Name].Rows[_rowIndex];
			IDataViewElement element = _dstDataView.ResultAttributes[_dstAttributeName];
			if (element != null)
			{
				string srcValue = ConvertDataType(element, _srcValue);

                if (!string.IsNullOrEmpty(srcValue))
                {
                    // set source value to destination
                    dstDataRow[_dstAttributeName] = srcValue;

                    // set this flag so that the update query can include this value
                    element.IsValueChanged = true;
                }
			}
			else
			{
                // Destination attribute may have been removed from the class
                //throw new MappingException("Unable to find the Data View element for " + _dstAttributeName + " during transformation.");
            }
        }

		#endregion

	}
}