/*
* @(#)IDataGridViewElementVisitor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a class view model.
	/// </summary>
	/// <version> 1.0.0 28 May 2006 </version>
	///
	public interface IDataGridViewElementVisitor
	{
		/// <summary>
		/// Viste a class view element.
		/// </summary>
		/// <param name="element">A DataGridViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitClassView(DataGridViewModel element);

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A ViewClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitClassElement(ViewClass element);

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A ViewFilter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitFilter(ViewFilter element);

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A ViewSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSimpleAttribute(ViewSimpleAttribute element);

        /// <summary>
        /// Viste an array attribute element.
        /// </summary>
        /// <param name="element">A ViewArrayAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitArrayAttribute(ViewArrayAttribute element);

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A ViewVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitVirtualAttribute(ViewVirtualAttribute element);

        /// <summary>
        /// Viste a relationship attribute element.
        /// </summary>
        /// <param name="element">A ViewRelationshipAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitRelationshipAttribute(ViewRelationshipAttribute element);

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ViewAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSimpleAttributes(ViewAttributeCollection element);

        /// <summary>
        /// Viste a ViewEnumElement.
        /// </summary>
        /// <param name="element">A ViewEnumElement instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitEnumElement(ViewEnumElement element);

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A ViewBinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitBinaryExpr(ViewBinaryExpr element);

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitLeftParenthesis(ViewParenthesizedExpr element);

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitRightParenthesis(ViewParenthesizedExpr element);

		/// <summary>
		/// Visite a search parameter.
		/// </summary>
		/// <param name="element">A ViewParameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitParameter(ViewParameter element);

		/// <summary>
		/// Begin visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitParametersBegin(ViewParameterCollection element);

		/// <summary>
		/// End Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitParametersEnd(ViewParameterCollection element);

		/// <summary>
		/// Visit ViewSortBy.
		/// </summary>
		/// <param name="element">A ViewSortBy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSortBy(ViewSortBy element);

        /// <summary>
        /// Viste an enum value Collection.
        /// </summary>
        /// <param name="element">A ViewEnumValueCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>	
        bool VisitEnumValues(ViewEnumValueCollection element);

        /// <summary>
        /// Viste an enum value.
        /// </summary>
        /// <param name="element">A ViewEnumValue instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>	
        bool VisitEnumValue(ViewEnumValue element);

        /// <summary>
        /// Viste a locale info.
        /// </summary>
        /// <param name="element">A ViewLocaleInfo instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>	
        bool VisitLocaleInfo(ViewLocaleInfo element);
	}
}