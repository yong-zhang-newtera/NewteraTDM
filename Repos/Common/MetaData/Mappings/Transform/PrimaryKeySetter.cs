/*
* @(#)PrimaryKeySetter.cs
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
    /// The transformer setting a value to a cell of an PrimaryKeySetter.
	/// </summary>
	/// <version> 1.0.0 17 Nov 2004</version>
	/// <author>Yong Zhang</author>
	internal class PrimaryKeySetter : AttributeSetterBase
	{
		private string _dstAttributeName;
		private string _relationshipName;

		/// <summary>
		/// Initiate an instance of PrimaryKeySetter class
		/// </summary>
		/// <param name="srcValue">The value from source</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="dstAttributeName"> The destination array attribute name</param>
		/// <param name="relationshipName">The name of the relationship that owns the primary key</param>
		public PrimaryKeySetter(string srcValue, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex,
			string dstAttributeName, string relationshipName)
			: base(srcValue, dstDataSet, dstDataView, rowIndex)
		{
			_dstAttributeName = dstAttributeName;
			_relationshipName = relationshipName;
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
			// The name of the DataTable consits of the name
			// of the class and relationship attribute which is created by BuildDataSetVisitor.
            DataRow dstDataRow = _dstDataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_dstDataView.BaseClass.Name, _relationshipName)].Rows[_rowIndex];
			IDataViewElement element = _dstDataView.ResultAttributes[_relationshipName];
			
			if (element != null && element is DataRelationshipAttribute)
			{
				DataRelationshipAttribute relationshipElement = (DataRelationshipAttribute) element;
				DataSimpleAttribute pkElement = null;
				foreach (DataSimpleAttribute pk in relationshipElement.PrimaryKeys)
				{
                    if ((relationshipElement.Name + "_" + pk.Name) == _dstAttributeName)
					{
						pkElement = pk;
						break;
					}
				}

				if (pkElement == null)
				{
					throw new MappingException("Unable to find Data View element for primary key " + _dstAttributeName);
				}

				// copy the value from source to destination
				//dstDataRow[_dstAttributeName] = _srcValue;
                dstDataRow[pkElement.Name] = _srcValue;

				// set this flag so that the update query can include this value
				pkElement.IsValueChanged = true;
			}
			else
			{
				throw new MappingException("Unable to find the Data View element for relationship " + _relationshipName + " during transformation.");
			}
		}

		#endregion

	}
}