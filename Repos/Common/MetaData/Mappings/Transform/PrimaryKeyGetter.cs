/*
* @(#)PrimaryKeyGetter.cs
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
	internal class PrimaryKeyGetter : AttributeGetterBase
	{
		private string _srcAttributeName;
		private string _relationshipName;

		/// <summary>
		/// Initiate an instance of PrimaryKeyGetter class
		/// </summary>
		/// <param name="srcDataRow">The source DataRow</param>
		/// <param name="srcDataView">The DataViewModel for source class</param>
		/// <param name="srcAttributeName"> The source attribute name</param>
		/// <param name="relationshipName">The name of the relationship that owns the primary key</param>		
		public PrimaryKeyGetter(DataRow srcDataRow,
			DataViewModel srcDataView, string srcAttributeName,
			string relationshipName)
			: base(srcDataRow, srcDataView)
		{
			_srcAttributeName = srcAttributeName;
			_relationshipName = relationshipName;
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
				return GetterType.PrimaryKeyGetter;
			}
		}

		/// <summary>
		/// Get a value from an attribute.
		/// </summary>
		public override string GetValue()
		{
			string srcValue = null;

			srcValue = _srcDataRow[_srcAttributeName].ToString();

			return srcValue;
		}

		#endregion

	}
}