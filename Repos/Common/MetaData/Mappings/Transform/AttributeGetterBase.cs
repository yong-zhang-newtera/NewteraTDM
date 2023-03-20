/*
* @(#)AttributeGetterBase.cs
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
	/// The base class for all attribute getters.
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	/// <author>Yong Zhang</author>
	internal abstract class AttributeGetterBase : IAttributeGetter
	{
		protected DataRow _srcDataRow;
		protected DataViewModel _srcDataView;

		/// <summary>
		/// Initiate an instance of AttributeGetterBase class
		/// </summary>
		/// <param name="srcDataRow">The source DataRow</param>
		/// <param name="srcDataView">The DataViewModel for source class</param>
		public AttributeGetterBase(DataRow srcDataRow, DataViewModel srcDataView)
		{
			_srcDataRow = srcDataRow;
			_srcDataView = srcDataView;
		}


		#region IAttributeGetter interface implementation
		
		/// <summary>
		/// Gets the type of setter
		/// </summary>
		/// <value>One of GetterType enum values</value>
		public abstract GetterType Type { get; }

		/// <summary>
		/// Get a value from an attribute.
		/// </summary>
		public abstract string GetValue();

		#endregion
	}
}