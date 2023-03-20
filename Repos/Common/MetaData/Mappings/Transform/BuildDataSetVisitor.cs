/*
* @(#)BuildDataSetVisitor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Data;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Taxonomy;
	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// Build an empty DataTable instance to reflect the structure of a destination class.
	/// </summary>
	/// <version> 1.0.0 24 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	internal class BuildDataSetVisitor : IDataViewElementVisitor
	{
		private DataSet _dstDataSet = null;
		private DataTable _dstDataTable = null;
		private ClassMapping _classMapping = null;

		/// <summary>
		/// Instantiate an instance of BuildDataSetVisitor class
		/// </summary>
		/// <param name="classMapping">The ClassMapping instance used by the visitor</param>
		/// <param name="dstDataSet">Destination data set</param>
		internal BuildDataSetVisitor(ClassMapping classMapping, DataSet dstDataSet)
		{
			_classMapping = classMapping;
			_dstDataSet = dstDataSet;
		}

		/// <summary>
		/// Viste a data view element.
		/// </summary>
		/// <param name="element">A DataViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataView(DataViewModel element)
		{
            string tableName = element.BaseClass.Name;
            int index = 1;

            // mutiple tables can map to the same destination class, therefore, we need to give a different name to each destination table
            while (_dstDataSet.Tables[tableName] != null)
            {
                tableName = element.BaseClass.Name + "_" + index;
                index++;
            }

			// add an DataTable to the destination DataSet for the base class
            _dstDataTable = new DataTable(tableName);
            _classMapping.DestinationTableName = tableName;

			// add an DataColumn representing the Attachment column to the DataTable
			_dstDataTable.Columns.Add(NewteraNameSpace.ATTACHMENTS);

			_dstDataSet.Tables.Add(_dstDataTable);

			return true;
		}

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A DataClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataClass(DataClass element)
		{
			return true;
		}

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A Filter instance</param>
		/// <returns>false to stop visiting filters</returns>		
		public bool VisitFilter(Filter element)
		{
			return false;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
			if (!_dstDataTable.Columns.Contains(element.Name))
			{
				// add an DataColumn to the DataTable
				_dstDataTable.Columns.Add(element.Name);
			}

			return true;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
		{
			if (!_dstDataTable.Columns.Contains(element.Name))
			{
				// add an DataColumn to the DataTable
				_dstDataTable.Columns.Add(element.Name);
			}

			return true;
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A DataVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitVirtualAttribute(DataVirtualAttribute element)
        {
            if (!_dstDataTable.Columns.Contains(element.Name))
            {
                // add an DataColumn to the DataTable
                _dstDataTable.Columns.Add(element.Name);
            }

            return true;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            if (!_dstDataTable.Columns.Contains(element.Name))
            {
                // add an DataColumn to the DataTable
                _dstDataTable.Columns.Add(element.Name);
            }

            return true;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
			DataColumn parentColumn;
			DataColumn childColumn;

			if (!_dstDataTable.Columns.Contains(element.Name))
			{
				// add an DataColumn to the DataTable
				parentColumn = _dstDataTable.Columns.Add(element.Name);
			}
			else
			{
				parentColumn = _dstDataTable.Columns[element.Name];
			}

			if (element.PrimaryKeyCount > 0)
			{
				// this is a forward relationship, add a DataRelation instance
				// for the relationship and a DataTable for storing primary key
				// value(s).
				// Create a DataTable using the relationship name as table name
                if (_dstDataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(this._classMapping.DestinationClassName, element.Name)] == null)
				{
                    DataTable dataTable = new DataTable(DataRelationshipAttribute.GetRelationshipDataTableName(this._classMapping.DestinationClassName, element.Name));
					childColumn = dataTable.Columns.Add(NewteraNameSpace.OBJ_ID);
					foreach (DataSimpleAttribute pk in element.PrimaryKeys)
					{
						dataTable.Columns.Add(pk.Name);
					}
					_dstDataSet.Tables.Add(dataTable);

					/*
					string relationName = element.Name + element.LinkedClassName;
					if (_dstDataSet.Relations[relationName] == null)
					{
						// add an relation
						DataRelation dataRelation = new DataRelation(element.Name + element.LinkedClassName,
							parentColumn, childColumn);
						_dstDataSet.Relations.Add(dataRelation);
					}
					*/
				}
			}

			return true;	
		}

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a referenced class Collection.
		/// </summary>
		/// <param name="element">A ReferencedClassCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitReferencedClasses(ReferencedClassCollection element)
		{
			return false;	
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
		{
			return false;	
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
		{
			return false;
		}

		/// <summary>
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
		{
			return false;
		}

		/// <summary>
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
		{
			return false;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
		{
			return false;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
			return false;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
		{
			return false;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
		{
			return false;
		}

		/// <summary>
		/// Visit a TaxonomyModel.
		/// </summary>
		/// <param name="element">A TaxonomyModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonomyModel(TaxonomyModel element)
		{
			return false;
		}

		/// <summary>
		/// Visit a TaxonNode.
		/// </summary>
		/// <param name="element">A TaxonNode instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonNode(TaxonNode element)
		{
			return false;
		}

		/// <summary>
		/// Visit a SortBy.
		/// </summary>
		/// <param name="element">A SortBy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitSortBy(SortBy element)
		{
			return false;
		}

        /// <summary>
        /// Visit a SortAttribute.
        /// </summary>
        /// <param name="element">A SortAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSortAttribute(SortAttribute element)
        {
            return false;
        }

        /// <summary>
        /// Visit a function element.
        /// </summary>
        /// <param name="element">A function instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitFunction(IFunctionElement element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyDef element.
        /// </summary>
        /// <param name="element">A AutoClassifyDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyDef(AutoClassifyDef element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyLevel element.
        /// </summary>
        /// <param name="element">A AutoClassifyLevel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyLevel(AutoClassifyLevel element)
        {
            return false;
        }
	}
}