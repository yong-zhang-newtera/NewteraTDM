/*
* @(#)FindSearchParameterVisitor.cs
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
	public class FindSearchParameterVisitor : IDataGridViewElementVisitor
	{
		private string _classAlias = null;
		private string _attributeName = null;
		private ViewParameter _parameter = null;

		/// <summary>
		/// Instantiate an instance of FindSearchParameterVisitor class
		/// </summary>
		/// <param name="classAlias">The alias of the class that owns the attribute</param>
		/// <param name="attributeName">The attribute name</param>
		public FindSearchParameterVisitor(string classAlias, string attributeName)
		{
			_classAlias = classAlias;
			_attributeName = attributeName;
		}

		/// <summary>
		/// Gets the ViewParameter object found
		/// </summary>
		/// <value>A ViewParameter object</value>
		public ViewParameter SearchParameter
		{
			get
			{
				return _parameter;
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
		/// <param name="element">A ViewSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(ViewSimpleAttribute element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}
		}

        /// <summary>
        /// Viste an array attribute element.
        /// </summary>
        /// <param name="element">A ViewArrayAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitArrayAttribute(ViewArrayAttribute element)
        {
            return false;
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
            return false;
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
			return false;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A ViewBinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(ViewBinaryExpr element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}		
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ViewParenthesizedExpr element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ViewParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ViewParenthesizedExpr element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A ViewParameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(ViewParameter element)
		{
			if (element.Name == _attributeName)
			{
				_parameter = element;

				return false; // return false to stop iteration
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ViewParameterCollection element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ViewParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ViewParameterCollection element)
		{
			if (_parameter != null)
			{
				// parameter is found, return false to stop the iteration
				return false;
			}
			else
			{
				return true;
			}
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