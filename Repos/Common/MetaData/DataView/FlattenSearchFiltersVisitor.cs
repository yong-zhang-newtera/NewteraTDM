/*
* @(#)FlattenSearchFiltersVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
    using System.Collections;

    using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a visitor that builds a flatten search filter collection and also assign unique
    /// aliases to the attributes appeared in the search expression so that each attribute is the
    /// expression can be uniquely identified using an alias. 
	/// </summary>
	/// <version> 1.0.0 04 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class FlattenSearchFiltersVisitor : IDataViewElementVisitor
	{
		private bool _isInParameters = false;
		private DataViewElementCollection _flattens;
        private Hashtable _usedAliases;
        private DataViewElementBase _currentAttribute;

		/// <summary>
		/// Instantiate a FlattenSearchFiltersVisitor instance
		/// </summary>
		public FlattenSearchFiltersVisitor()
		{
			_flattens = new DataViewElementCollection();
            _usedAliases = new Hashtable();
            _currentAttribute = null;
		}

		/// <summary>
		/// Gets the flattened search filters
		/// </summary>
		/// <value>A DataViewElementCollection object</value>
		public DataViewElementCollection FlattenedSearchFilters
		{
			get
			{
				return _flattens;
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
			_flattens.Add(element);
            element.Alias = GetAlias(element.OwnerClassAlias, element.Name);
            _currentAttribute = element;
			return true;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
		{
            _flattens.Add(element);
            element.Alias = GetAlias(element.OwnerClassAlias, element.Name);
            _currentAttribute = element;
			return true;
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A DataVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitVirtualAttribute(DataVirtualAttribute element)
        {
            return true;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            return true;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
            _flattens.Add(element);
            element.Alias = GetAlias(element.OwnerClassAlias, element.Name);
            _currentAttribute = element;

            return false; // stop visiting the children of relationship since we do not want to show the primary keys
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
			return true;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
		{
            if (element.Left == null)
            {
                // create an empty element so that expression builder can show it
                // appropriately
                LeftEmptyOperand left = new LeftEmptyOperand();
                left.ParentElement = element;
                _flattens.Add(left);
            }
            else
            {
                element.Left.ParentElement = element;
            }

			_flattens.Add(element);

            if (element.Right == null &&
                element.ElementType != ElementType.IsNull &&
                element.ElementType != ElementType.IsNotNull)
            {
                RightEmptyOperand right = new RightEmptyOperand();
                right.ParentElement = element;
                _flattens.Add(right);
            }
            else if (element.Right != null)
            {
                element.Right.ParentElement = element;
            }

			return true;
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
		{
			LeftParenthesis expr = new LeftParenthesis();
			expr.ParentElement = element;
			_flattens.Add(expr);
			return true;
		}

		/// <summary>
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
		{
			RelationshipBegin expr = new RelationshipBegin(element.Name);
			expr.Caption = element.Caption;
			expr.Description = element.Description;
			expr.LinkedClassAlias = element.LinkedClassAlias;
			_flattens.Add(expr);
			return true;
		}

		/// <summary>
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
		{
			_flattens.Add(new RelationshipEnd());
			return true;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
		{
			RightParenthesis expr = new RightParenthesis();
			expr.ParentElement = element;
			_flattens.Add(expr);
			return true;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
			_flattens.Add(element);

            if (_currentAttribute != null)
            {
                // get the alias of the current attribute which appears on the left side of a
                // binary expression, while the parameter is on right side of it.
                // we are doing this because we will be able to find the parameter given
                // an alias when using FindSearchParameterVisitor
                element.Alias = _currentAttribute.Alias;
            }

			if (_isInParameters)
			{
				_flattens.Add(new Comma());
			}

			return true;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
		{
			LeftParenthesis expr = new LeftParenthesis();
			expr.ParentElement = element;
			_flattens.Add(expr);
			_isInParameters = true;
			return true;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
		{
			RightParenthesis expr = new RightParenthesis();
			expr.ParentElement = element;
			_flattens.Add(expr);
			_isInParameters = false;
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
            _flattens.Add((IDataViewElement)element);

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

        /// <summary>
        /// Gets an unque alias for an attribute
        /// </summary>
        /// <param name="ownerClassAlias">The alias of the class that owns the attribute</param>
        /// <param name="attributeName">The attribute name</param>
        private string GetAlias(string ownerClassAlias, string attributeName)
        {
            string basicAlias = ownerClassAlias + "_" + attributeName;
            string alias = basicAlias;
            int sequenceNo = 1;
            while (_usedAliases.Contains(alias))
            {
                alias = basicAlias + sequenceNo;
                sequenceNo++;
            }

            _usedAliases.Add(alias, alias); // remember the alias

            return alias;
        }
	}
}