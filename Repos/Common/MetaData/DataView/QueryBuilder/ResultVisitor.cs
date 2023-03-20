/*
* @(#)ResultVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a DataView visitor that generates a result part of a XQuery. 
	/// </summary>
	/// <version> 1.0.0 30 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	internal class ResultVisitor : IDataViewElementVisitor
	{
		private DataViewModel _dataView;
		private ICompositQueryElement _returnClause;
		private ICompositQueryElement _baseClassElement;
		private bool _includePrimaryKeys;
        private bool _ignoreUsage;

		/// <summary>
		/// Initiate an instance of ResultVisitor class
		/// </summary>
		/// <param name="dataView">The data view</param>
		/// <param name="includePrimaryKeys">Value to indicate whether to include
		/// primary key values as part of search result for relationship attributes</param> 
		public ResultVisitor(DataViewModel dataView, bool includePrimaryKeys)
		{
			_dataView = dataView;
			_returnClause = null;
			_baseClassElement = null;
			_includePrimaryKeys = includePrimaryKeys;
            _ignoreUsage = true;
		}

        /// <summary>
        /// Initiate an instance of ResultVisitor class
        /// </summary>
        /// <param name="dataView">The data view</param>
        /// <param name="includePrimaryKeys">Value to indicate whether to include
        /// primary key values as part of search result for relationship attributes</param> 
        public ResultVisitor(DataViewModel dataView, bool includePrimaryKeys, bool ignoreUsage)
        {
            _dataView = dataView;
            _returnClause = null;
            _baseClassElement = null;
            _includePrimaryKeys = includePrimaryKeys;
            _ignoreUsage = ignoreUsage;
        }

		/// <summary>
		/// Gets the return clause built by the visitor
		/// </summary>
		/// <value>A return clause</value>
		public IQueryElement ReturnClause
		{
			get
			{
				// make sure there is at least one result attribute
				if (this._baseClassElement == null ||
					this._baseClassElement.Children.Count <= 0)
				{
					throw new DataViewException("There isn't any attributes defined for class " + _dataView.BaseClass.Caption);
				}

				return (IQueryElement) _returnClause;
			}
		}

		/// <summary>
		/// Viste a data view element.
		/// </summary>
		/// <param name="element">A DataViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitDataView(DataViewModel element)
		{
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
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitFilter(Filter element)
		{
			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
			if (_baseClassElement != null)
			{
				IQueryElement simpleResultElement = new SimpleResultElement(element);

				_baseClassElement.Children.Add(simpleResultElement);
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
			if (_baseClassElement != null)
			{
				IQueryElement arrayResultElement = new ArrayResultElement(element);

				_baseClassElement.Children.Add(arrayResultElement);
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
            if (_baseClassElement != null)
            {
                IQueryElement virtualResultElement = new VirtualResultElement(element);

                _baseClassElement.Children.Add(virtualResultElement);
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
            if (_baseClassElement != null)
            {
                IQueryElement imageResultElement = new ImageResultElement(element);

                _baseClassElement.Children.Add(imageResultElement);
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
			if (_baseClassElement != null)
			{
				DataClass ownerClass = _dataView.FindClass(element.OwnerClassAlias);
				ClassElement classElement = _dataView.SchemaModel.FindClass(ownerClass.ClassName);
				RelationshipAttributeElement relationshipAttribute = classElement.FindInheritedRelationshipAttribute(element.Name);
                RelationshipResultElement relationshipResultElement;
                if (_ignoreUsage || relationshipAttribute.Usage == DefaultViewUsage.Included)
                {
                    relationshipResultElement = new RelationshipResultElement(element, _includePrimaryKeys, _dataView.BaseClass.Name);
                }
                else
                {
                    // do not include PK key values
                    relationshipResultElement = new RelationshipResultElement(element, false, _dataView.BaseClass.Name);
                }
				relationshipResultElement.IsForeignKeyRequired = relationshipAttribute.IsForeignKeyRequired;

				_baseClassElement.Children.Add(relationshipResultElement);
			}

			return false; // stop visting its sub elements
		}

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			_returnClause = new ReturnClause();

			_baseClassElement = new BaseClassElement(_dataView.BaseClass);

			_returnClause.Children.Add((IQueryElement) _baseClassElement);
 
			return true;
		}

		/// <summary>
		/// Viste a referenced class Collection.
		/// </summary>
		/// <param name="element">A ReferencedClassCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitReferencedClasses(ReferencedClassCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
			return true;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// Visit a TaxonomyModel.
		/// </summary>
		/// <param name="element">A TaxonomyModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonomyModel(TaxonomyModel element)
		{
			return true;
		}

		/// <summary>
		/// Visit a TaxonNode.
		/// </summary>
		/// <param name="element">A TaxonNode instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonNode(TaxonNode element)
		{
			return true;
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