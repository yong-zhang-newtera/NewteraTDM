/*
* @(#)ArrayDataCellMapping.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;
	using System.Reflection;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;

	/// <summary>
	/// The class represents mapping definition of a source attribute and
	/// an array data cell of a destination array attribute.
	/// </summary>
	/// <version>1.0.0 31 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class ArrayDataCellMapping : AttributeMapping
	{
		private string _arrayAttributeName = null; // obtained from destination attribute name
		private int _rowIndex = -1; // obtained from destination attribute name
		private int _colIndex = -1; // obtained from destination attribute name

		/// <summary>
		/// Initiate an instance of ArrayDataCellMapping class.
		/// </summary>
		public ArrayDataCellMapping(string sourceAttribute, string destinationAttribute) : base(sourceAttribute, destinationAttribute)
		{
		}

		/// <summary>
		/// Initiating an instance of ArrayDataCellMapping class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ArrayDataCellMapping(XmlElement xmlElement) : base(xmlElement)
		{
		}


		/// <summary>
		/// Gets name of the array attribute associated with a ArrayDataCellMapping
		/// </summary>
		public string ArrayAttributeName
		{
			get
			{
				if (_arrayAttributeName == null)
				{
					int pos = base.DestinationAttributeName.IndexOf("_");
					if (pos > 0)
					{
						_arrayAttributeName = base.DestinationAttributeName.Substring(0, pos);
					}
					else
					{
						_arrayAttributeName = base.DestinationAttributeName;
					}
				}

				return _arrayAttributeName;
			}
		}

		/// <summary>
		/// Gets the row index of the array data cell
		/// </summary>
		public int RowIndex
		{
			get
			{
				if (_rowIndex < 0)
				{
					GetIndex();
				}

				return _rowIndex;
			}
		}

		/// <summary>
		/// Gets the column index of the array data cell
		/// </summary>
		public int ColIndex
		{
			get
			{
				if (_colIndex < 0)
				{
					GetIndex();
				}

				return _colIndex;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ArrayDataCellMapping;
			}
		}

		/// <summary>
		/// create an ArrayDataCellMapping from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write ArrayDataCellMapping to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}

		/// <summary>
		/// Perform transformation and return a collection of IAttributeSetter instances that set a source
		/// value to destination attribute.
		/// </summary>
		/// <param name="srcDataRow">The DataRow from source</param>
		/// <param name="srcDataView">The DataViewModel for source class, can be null.</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="assembly">The assembly contains transformer classes</param>
		/// <returns>An AttributeSetterCollection instance.</returns>
		public override AttributeSetterCollection DoTransform(DataRow srcDataRow, 
			DataViewModel srcDataView, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex, Assembly assembly)
		{
			AttributeSetterCollection setters = new AttributeSetterCollection();

			IAttributeGetter getter = GetterFactory.Instance.Create(this.GetterType,
				srcDataRow, SourceAttributeName, srcDataView);

			string srcValue = getter.GetValue();

			IAttributeSetter setter = new ArrayDataCellSetter(srcValue,
				dstDataSet, dstDataView,
				rowIndex, ArrayAttributeName,
				RowIndex, ColIndex);
			
			setters.Add(setter);

			return setters;
		}

		/// <summary>
		/// Parse the row and column index which is part of the destination attribute
		/// name.
		/// </summary>
		private void GetIndex()
		{
			int pos = base.DestinationAttributeName.IndexOf("_");
			if (pos > 0)
			{
				// the index str is rowIndex_colIndex, for example 9_12
				string indexStr = base.DestinationAttributeName.Substring(pos + 1);
				pos = indexStr.IndexOf("_");
				if (pos > 0 && (pos + 1) < indexStr.Length)
				{
					_rowIndex = int.Parse(indexStr.Substring(0, pos));
					_colIndex = int.Parse(indexStr.Substring(pos + 1));
				}
			}
		}
	}
}