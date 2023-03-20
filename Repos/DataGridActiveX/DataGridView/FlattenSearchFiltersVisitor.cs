/*
* @(#)FlattenSearchFiltersVisitor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;

	/// <summary>
	/// Represents a visitor that builds a flatten search filter collection. 
	/// </summary>
	/// <version> 1.0.0 04 May 2006 </version>
	///
	public class FlattenSearchFiltersVisitor : IDataGridViewElementVisitor
	{
		private bool _isInParameters = false;
		private DataGridViewElementCollection _flattens;

		/// <summary>
		/// Instantiate a FlattenSearchFiltersVisitor instance
		/// </summary>
		public FlattenSearchFiltersVisitor()
		{
			_flattens = new DataGridViewElementCollection();
		}

		/// <summary>
		/// Gets the flattened search filters
		/// </summary>
		/// <value>A DataGridViewElementCollection object</value>
		public DataGridViewElementCollection FlattenedSearchFilters
		{
			get
			{
				return _flattens;
			}
		}

		/// <summary>
		/// Viste a class view element.
		/// </summary>
		/// <param name="element">A DataGridViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitClassView(DataGridViewModel element)
		{
			return true;
		}

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A ViewClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitClassElement(ViewClass element)
		{
			return true;
		}

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A ViewFilter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitFilter(ViewFilter element)
		{
			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A ViewAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(ViewSimpleAttribute element)
		{
			_flattens.Add(element);
			return true;
		}

        /// <summary>
        /// Viste an array attribute element.
        /// </summary>
        /// <param name="element">A ViewArrayAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitArrayAttribute(ViewArrayAttribute element)
        {
            return true;
        }

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A ViewVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitVirtualAttribute(ViewVirtualAttribute element)
        {
            return false;
        }

        /// <summary>
        /// Viste a relationship attribute element.
        /// </summary>
        /// <param name="element">A ViewRelationshipAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitRelationshipAttribute(ViewRelationshipAttribute element)
        {
            _flattens.Add(element);
            return false; // stop visiting the children of relationship since we do not want to show the primary keys
        }

        /// <summary>
        /// Viste an enum element.
        /// </summary>
        /// <param name="element">A ViewEnumElement instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitEnumElement(ViewEnumElement element)
        {
            return false;
        }

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ViewAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttributes(ViewAttributeCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A ViewBinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(ViewBinaryExpr element)
		{
			if (element.Left == null)
			{
				// create an empty element so that expression builder can show it
				// appropriately
				ViewLeftEmptyOperand left = new ViewLeftEmptyOperand();
				left.Parent = element;
				_flattens.Add(left);
			}

			_flattens.Add(element);

            if (element.Right == null &&
                element.ElementType != ViewElementType.IsNull &&
                element.ElementType != ViewElementType.IsNotNull)
			{
				ViewRightEmptyOperand right = new ViewRightEmptyOperand();
				right.Parent = element;
				_flattens.Add(right);
			}

			return true;
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ViewParenthesizedExpr element)
		{
			ViewLeftParenthesis expr = new ViewLeftParenthesis();
			expr.Parent = element;
			_flattens.Add(expr);
			return true;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ViewParenthesizedExpr element)
		{
			ViewRightParenthesis expr = new ViewRightParenthesis();
			expr.Parent = element;
			_flattens.Add(expr);
			return true;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A ViewParameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(ViewParameter element)
		{
			_flattens.Add(element);

			if (_isInParameters)
			{
				_flattens.Add(new ViewComma());
			}

			return true;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ViewParameterCollection element)
		{
			ViewLeftParenthesis expr = new ViewLeftParenthesis();
			expr.Parent = element;
			_flattens.Add(expr);
			_isInParameters = true;
			return true;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ViewParameterCollection element)
		{
			ViewRightParenthesis expr = new ViewRightParenthesis();
			expr.Parent = element;
			_flattens.Add(expr);
			_isInParameters = false;
			return true;
		}

		/// <summary>
		/// Visit ViewSortBy.
		/// </summary>
		/// <param name="element">A ViewSortBy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitSortBy(ViewSortBy element)
		{
			return false;
		}

        /// <summary>
        /// Visit ViewEnumValueCollection.
        /// </summary>
        /// <param name="element">A ViewEnumValueCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitEnumValues(ViewEnumValueCollection element)
        {
            return false;
        }

        /// <summary>
        /// Visit ViewEnumValue.
        /// </summary>
        /// <param name="element">A ViewEnumValue instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitEnumValue(ViewEnumValue element)
        {
            return false;
        }

        /// <summary>
        /// Viste a locale info.
        /// </summary>
        /// <param name="element">A ViewLocaleInfo instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>	
        public bool VisitLocaleInfo(ViewLocaleInfo element)
        {
            return false;
        }
	}
}