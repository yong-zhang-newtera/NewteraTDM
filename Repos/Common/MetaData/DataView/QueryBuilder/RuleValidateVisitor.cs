/*
* @(#)RuleValidateVisitor.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Rules;
    using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a DataView visitor that generates a XQuery for validating a data instance.
	/// </summary>
	/// <version> 1.0.0 18 Oct 2007 </version>
	internal class RuleValidateVisitor : EditInstanceVisitorBase, IDataViewElementVisitor
	{
		private ICompositQueryElement _stmt;
		private bool _visitingResultAttributes;
		private ICompositQueryElement _inlinedXmlClause;
        private RuleDef _ruleDef;

		/// <summary>
		/// Initiate an instance of RuleValidateVisitor class
		/// </summary>
		/// <param name="dataView">The data view</param>
		/// <param name="objId">The id of an instance to be updated.</param>
		public RuleValidateVisitor(DataViewModel dataView, string objId, RuleDef ruleDef) : base(dataView)
		{
            _ruleDef = ruleDef;

			_visitingResultAttributes = false;

			// create the static part of query first
			_stmt = new XQueryStatement();

			_inlinedXmlClause = BuildInlinedXmlClause(objId);
			_stmt.Children.Add((IQueryElement) _inlinedXmlClause);

            IQueryElement returnClause = BuildReturnClauseForRule(objId);
			_stmt.Children.Add(returnClause);
		}

		/// <summary>
		/// Gets the xquery built by the visitor
		/// </summary>
		/// <value>A XQuery for update</value>
		public IQueryElement XQueryStatement
		{
			get
			{
				return (IQueryElement) _stmt;
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
            // create a let statement that assigns a relatiosnhip to a variable
            // this let statement is different to what used in the search statement, since it
            // needs to be modified to reference an element in an in-line xml document
            if (element.Type == DataClassType.ReferencedClass && element.IsReferenced)
			{
				ICompositQueryElement letClause = new LetClause(element.Alias, element.ClassName,
                    element.ReferringClassAlias, element.ReferringRelationshipName, true);
                _stmt.Children.Insert(1, (IQueryElement)letClause); // insert after the first let statement
			}
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
			// do not add an XQuery element if the value of an element is unchanged
			if (_visitingResultAttributes)
			{
				IQueryElement simpleEditElement = new SimpleEditElement(element);

				this._inlinedXmlClause.Children.Add(simpleEditElement);
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
			if (_visitingResultAttributes)
			{
				IQueryElement arrayEditElement = new ArrayEditElement(element);

				this._inlinedXmlClause.Children.Add(arrayEditElement);
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
            return true; // no need to update the virtual attribute
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            return true; // no need to update the virtual attribute
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
			if (_visitingResultAttributes && element.PrimaryKeys != null)
			{
				// do not add an XQuery element if the primary keys have an empty value
				RelationshipEditElement relationshipEditElement = new RelationshipEditElement(element);

				DataViewElementCollection primaryKeys = element.PrimaryKeys;
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					IQueryElement pkElement = new SimpleEditElement(pk);
					relationshipEditElement.Children.Add(pkElement);
				}

				_inlinedXmlClause.Children.Add(relationshipEditElement);
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
			_visitingResultAttributes = true;
 
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

        /// <summary>
        /// Build a return clause that contains rule statement
        /// </summary>
        /// <param name="objId">The data instance id</param>
        /// <returns>An IQueryElement</returns>
        protected IQueryElement BuildReturnClauseForRule(string objId)
        {
            ICompositQueryElement returnClause = new ReturnClause();

            // assign the objId to rule in case that some of functions in the rule
            // need to know the data instance id to operate
            _ruleDef.ObjId = objId;

            IQueryElement ruleStatement = new RuleStatement(_ruleDef,
                _dataView.BaseClass.Alias, _dataView.SchemaInfo.Name,
                _dataView.SchemaInfo.Version);

            returnClause.Children.Add(ruleStatement);

            return (IQueryElement)returnClause;
        }
	}
}