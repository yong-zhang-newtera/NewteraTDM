/*
* @(#)PrepareVisitor.cs
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
	/// Represents a DataView visitor that prepare the data view for query
	/// generation. For example, determine which referenced classes will be required
	/// in a query. Including referenced classes that are not used in a query will
	/// impact query performance. 
	/// </summary>
	/// <version> 1.0.0 06 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	internal class PrepareVisitor : IDataViewElementVisitor
	{
		private DataViewModel _dataView;
		private bool _isVisitingResults;
		private bool _includePrimaryKeys;
        private bool _ignoreUsage;

		/// <summary>
		/// Initiate an instance of PrepareVisitor class
		/// </summary>
		/// <param name="dataView"></param>
		/// <param name="includePrimaryKeys">Value to indicate whether to include
		/// primary key values as part of search result for relationship attributes</param>
		public PrepareVisitor(DataViewModel dataView, bool includePrimaryKeys)
		{
			_dataView = dataView;
			_isVisitingResults = false;
			_includePrimaryKeys = includePrimaryKeys;
            _ignoreUsage = true;
		}

        /// <summary>
        /// Initiate an instance of PrepareVisitor class
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="includePrimaryKeys">Value to indicate whether to include
        /// primary key values as part of search result for relationship attributes</param>
        public PrepareVisitor(DataViewModel dataView, bool includePrimaryKeys, bool ignoreUsage)
        {
            _dataView = dataView;
            _isVisitingResults = false;
            _includePrimaryKeys = includePrimaryKeys;
            _ignoreUsage = ignoreUsage;
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
			_isVisitingResults = false;
			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
            if (_isVisitingResults)
			{
				// mark the referenced classes that are in the path leading to
				// this simple attribute.
				DataClass referencedClass = _dataView.FindClass(element.OwnerClassAlias);
				while (referencedClass != null)
				{
					referencedClass.IsReferenced = true;

					// get the referring class of this referenced class if it exists
                    string referringClassAlias = referencedClass.ReferringClassAlias;
					if (referringClassAlias != null)
					{
						referencedClass = _dataView.FindClass(referringClassAlias);
					}
					else
					{
						referencedClass = null;
					}
				}
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
			if (_isVisitingResults)
			{
				// mark the referenced classes that are in the path leading to
				// this array attribute.
				DataClass referencedClass = _dataView.FindClass(element.OwnerClassAlias);
				while (referencedClass != null)
				{
					referencedClass.IsReferenced = true;

					// get the referring class of this referenced class if it exists
                    string referringClassAlias = referencedClass.ReferringClassAlias;
                    if (referringClassAlias != null)
					{
                        referencedClass = _dataView.FindClass(referringClassAlias);
					}
					else
					{
						referencedClass = null;
					}
				}
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
            if (_isVisitingResults)
            {
                // mark the referenced classes that are in the path leading to
                // this virtual attribute.
                DataClass referencedClass = _dataView.FindClass(element.OwnerClassAlias);
                while (referencedClass != null)
                {
                    referencedClass.IsReferenced = true;

                    // get the referring class of this referenced class if it exists
                    string referringClassAlias = referencedClass.ReferringClassAlias;
                    if (referringClassAlias != null)
                    {
                        referencedClass = _dataView.FindClass(referringClassAlias);
                    }
                    else
                    {
                        referencedClass = null;
                    }
                }
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
            if (_isVisitingResults)
            {
                // mark the referenced classes that are in the path leading to
                // this virtual attribute.
                DataClass referencedClass = _dataView.FindClass(element.OwnerClassAlias);
                while (referencedClass != null)
                {
                    referencedClass.IsReferenced = true;

                    // get the referring class of this referenced class if it exists
                    string referringClassAlias = referencedClass.ReferringClassAlias;
                    if (referringClassAlias != null)
                    {
                        referencedClass = _dataView.FindClass(referringClassAlias);
                    }
                    else
                    {
                        referencedClass = null;
                    }
                }
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
			if (_isVisitingResults)
			{
				// mark the referenced classes that are in the path leading to
				// this relationship attribute.
				DataClass referencedClass = _dataView.FindClass(element.OwnerClassAlias);
				while (referencedClass != null)
				{
					referencedClass.IsReferenced = true;

					// get the referring class of this referenced class if it exists
                    string referringClassAlias = referencedClass.ReferringClassAlias;
                    if (referringClassAlias != null)
					{
                        referencedClass = _dataView.FindClass(referringClassAlias);
					}
					else
					{
						referencedClass = null;
					}
				}				

				if (_includePrimaryKeys)
				{
                    RelationshipAttributeElement relationshipAttribute = element.GetSchemaModelElement() as RelationshipAttributeElement;
                    if (_ignoreUsage || relationshipAttribute.Usage == DefaultViewUsage.Included)
                    {
                        // mark the referenced class for the primary keys
                        DataViewElementCollection primaryKeys = element.PrimaryKeys;
                        if (primaryKeys != null)
                        {
                            foreach (DataSimpleAttribute pk in primaryKeys)
                            {
                                referencedClass = _dataView.FindClass(pk.OwnerClassAlias);
                                if (referencedClass != null)
                                {
                                    referencedClass.IsReferenced = true;
                                }
                            }
                        }
                    }
				}

				// tell visitor not to visit the primary keys of this relationship
				// attribute since primary keys have been handled in this block
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
			_isVisitingResults = true;
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
			if (element is RelationalExpr || element is InExpr)
			{
				DataSimpleAttribute simpleAttribute = element.Left as DataSimpleAttribute;
				DataRelationshipAttribute relationshipAttribute = element.Left as DataRelationshipAttribute;
                IParameter parameter = element.Right as IParameter;

				// find the referenced class
				DataClass referencedClass = null;
				if (simpleAttribute != null)
				{
					referencedClass = _dataView.FindClass(simpleAttribute.OwnerClassAlias);
				}
				else if (relationshipAttribute != null)
				{
					referencedClass = _dataView.FindClass(relationshipAttribute.OwnerClassAlias);
				}

				// mark the referenced classes that it is being used by a search expression
				// whose parameter value isn't empty
				if (referencedClass != null && !referencedClass.IsReferenced)
				{
                    if (element.HasValue)
                    {
                        referencedClass.IsReferenced = true;

                        DataClass referringClass = _dataView.FindClass(referencedClass.ReferringClassAlias);
                        while (referringClass != null)
                        {
                            referringClass.IsReferenced = true;

                            if (referringClass.ReferringClassAlias != null)
                            {
                                referringClass = _dataView.FindClass(referringClass.ReferringClassAlias);
                            }
                            else
                            {
                                referringClass = null;
                            }
                        }			
                    }
				}

                // When the right side element is a DataRelationshipAttribute, replace the
                // binary expression that consists of the relationship attribute with the
                // expression that consists of primary key(s) of the referenced class
                if (relationshipAttribute != null)
                {
                    IDataViewElement primaryKeyExpr = relationshipAttribute.GetPrimaryKeyFilter(element, parameter);
                    element.SubstituteExpression = primaryKeyExpr;
                    element.SubstituteExpression.Accept(this); // prepare the primary key expression
                }
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