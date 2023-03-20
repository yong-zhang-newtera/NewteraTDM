/*
* @(#)FindSearchParameterVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;

	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a visitor that find a parameter corresponding to a given search attribute 
	/// </summary>
	/// <version> 1.0.0 04 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class FindSearchParameterVisitor : IDataViewElementVisitor
	{
		private string _classAlias = null;
		private string _attributeName = null;
        private string _attributeAlias = null;
		private Parameter _parameter = null;

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
        /// Instantiate an instance of FindSearchParameterVisitor class
        /// </summary>
        /// <param name="attributeAlias">The alias of the search attribute</param>
        public FindSearchParameterVisitor(string attributeAlias)
        {
            _attributeAlias = attributeAlias;
        }

		/// <summary>
		/// Gets the Parameter object found
		/// </summary>
		/// <value>A Parameter object</value>
		public Parameter SearchParameter
		{
			get
			{
				return _parameter;
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
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
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
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A DataVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitVirtualAttribute(DataVirtualAttribute element)
        {
            return false;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            return false;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
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
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			return false;
		}

		/// <summary>
		/// Viste a referenced class Collection.
		/// </summary>
		/// <param name="element">A ReferencedClassCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitReferencedClasses(ReferencedClassCollection element)
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
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
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
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
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
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
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
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
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
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
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
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
            if (_attributeAlias != null && element.Alias == _attributeAlias)
            {
                // match the parameter using attribute alias
                _parameter = element;

                return false; // return false to stop iteration
            }
			else if (element.Name == _attributeName &&
				element.ClassAlias == _classAlias)
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
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
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
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
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