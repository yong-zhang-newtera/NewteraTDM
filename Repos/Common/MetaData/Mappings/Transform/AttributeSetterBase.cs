/*
* @(#)AttributeSetterBase.cs
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

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The base class for all attribute setters.
	/// </summary>
	/// <version> 1.0.0 17 Nov 2004</version>
	/// <author>Yong Zhang</author>
	internal abstract class AttributeSetterBase : IAttributeSetter
	{
		protected string _srcValue;
		protected DataSet _dstDataSet;
		protected DataViewModel _dstDataView;
		protected int _rowIndex;

		/// <summary>
		/// Initiate an instance of AttributeSetterBase class
		/// </summary>
		/// <param name="srcValue">The value from source</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param> 
		public AttributeSetterBase(string srcValue, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex)
		{
			_srcValue = srcValue;
			_dstDataSet = dstDataSet;
			_dstDataView = dstDataView;
			_rowIndex = rowIndex;
		}


		#region IAttributeSetter interface implementation
		
		/// <summary>
		/// Gets the type of setter
		/// </summary>
		/// <value>One of SetterType enum values</value>
		public abstract SetterType Type { get; }

		/// <summary>
		/// Assign a value to an attribute.
		/// </summary>
		public abstract void AssignValue();

		#endregion

		/// <summary>
		/// Make necessary data type conversion from original value
		/// </summary>
		/// <param name="element">The destination IDataViewElement</param>
		/// <param name="original">Original value</param>
		/// <returns>The converted value</returns>
		protected string ConvertDataType(IDataViewElement element, string original)
		{
			string converted = original;
			DataType dataType = DataType.Unknown;

			SchemaModelElement schemaModelElement = element.GetSchemaModelElement();

			SimpleAttributeElement simpleAttribute = schemaModelElement as SimpleAttributeElement;

			if (simpleAttribute != null)
			{
				dataType = simpleAttribute.DataType;
			}

			switch (dataType)
			{
				case DataType.Integer:
					// take the integer portion
					int pos = original.IndexOf('.');

					if (pos > 0)
					{
						converted = original.Substring(0, pos);
					}
					else if (pos == 0)
					{
						converted = "0";
					}

					break;
				default:
					break;
			}

			return converted;
		}
	}
}