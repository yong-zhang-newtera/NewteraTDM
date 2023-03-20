/*
* @(#)XQueryBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Xml;

	using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Rules;

	/// <summary>
	/// An utility class that builds XQuery for various purposes
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	internal class XQueryBuilder
	{
		private DataViewModel _dataView;

		/// <summary>
		/// Initiating an instance of XQueryBuilder class
		/// </summary>
		/// <param name="dataView">The DataView instance</param>
		public XQueryBuilder(DataViewModel dataView)
		{
			_dataView = dataView;
		}

        /// <summary>
		/// Generate an XQuery for searching based on the data view spec
		/// </summary>
        /// <param name="includePrimaryKeys">Information indicating whether to include
        /// primary key values as part of search result for relationship attributes</param>
        /// <returns>An XQuery</returns>
        public string GenerateSearchQuery(bool includePrimaryKeys)
        {
            return GenerateSearchQuery(includePrimaryKeys, false);
        }

		/// <summary>
		/// Generate an XQuery for searching based on the data view spec
		/// </summary>
        /// <param name="includePrimaryKeys">Information indicating whether to include
        /// primary key values as part of search result for relationship attributes</param>
        /// <param name="ignoreUsage">True to include primary keys of all relatioships without regard to the Usage Setting, false, otherwise</param>
        /// <returns>An XQuery</returns>
		public string GenerateSearchQuery(bool includePrimaryKeys, bool ignoreUsage)
		{
			ICompositQueryElement stmt;

			// Prepare the data view for query generation
            PrepareVisitor prepareVisitor = new PrepareVisitor(_dataView, includePrimaryKeys, ignoreUsage);
			_dataView.Accept(prepareVisitor);

			// generate the search part of the query
			SearchVisitor searchVisitor = new SearchVisitor(_dataView);

			_dataView.Accept(searchVisitor);

			stmt = searchVisitor.XQueryStatement;

			// then generate the result part of the query
            ResultVisitor resultVisitor = new ResultVisitor(_dataView, includePrimaryKeys, ignoreUsage);

			_dataView.Accept(resultVisitor);

			IQueryElement returnClause = resultVisitor.ReturnClause;

			stmt.Children.Add(returnClause);

			if (_dataView.SortBy.HasSortBy)
			{
				stmt.Children.Add(_dataView.SortBy);
			}

			return ((IQueryElement) stmt).ToXQuery();
		}

		/// <summary>
		/// Generate an XQuery for searching for a particular instance of a given id.
		/// </summary>
		/// <returns>An XQuery</returns>
		public string GenerateInstanceQuery()
		{
			ICompositQueryElement stmt;

			// Prepare the data view for query generation
			PrepareVisitor prepareVisitor = new PrepareVisitor(_dataView, true);
			_dataView.Accept(prepareVisitor);

			// generate the search part of the query
			SearchVisitor searchVisitor = new SearchVisitor(_dataView);

			_dataView.Accept(searchVisitor);

			stmt = searchVisitor.XQueryStatement;

			// then generate the result part of the query
			ResultVisitor resultVisitor = new ResultVisitor(_dataView, true);

			_dataView.Accept(resultVisitor);

			IQueryElement returnClause = resultVisitor.ReturnClause;

			stmt.Children.Add(returnClause);

            if (_dataView.SortBy.HasSortBy)
            {
                stmt.Children.Add(_dataView.SortBy);
            }

            return ((IQueryElement) stmt).ToXQuery();
		}

		/// <summary>
		/// Generate an XQuery for insert an instance
		/// </summary>
		/// <returns>An XQuery</returns>
		public string GenerateInsertQuery()
		{
			IQueryElement stmt;

			// generate a xquery for insert
			InsertVisitor insertVisitor = new InsertVisitor(_dataView);

			_dataView.Accept(insertVisitor);

			stmt = insertVisitor.XQueryStatement;

			return stmt.ToXQuery();
		}

		/// <summary>
		/// Generate an XQuery for update an instance
		/// </summary>
		/// <returns>An XQuery</returns>
		public string GenerateUpdateQuery(bool verifyChanges)
		{
			IQueryElement stmt;

			// generate a xquery for update
            UpdateVisitor updateVisitor = new UpdateVisitor(_dataView, _dataView.CurrentObjId, verifyChanges);

			_dataView.Accept(updateVisitor);

			stmt = updateVisitor.XQueryStatement;

			return stmt.ToXQuery();
		}

		/// <summary>
		/// Generate an XQuery for deleting an instance
		/// </summary>
		/// <returns>An XQuery</returns>
		public string GenerateDeleteQuery()
		{
			IQueryElement stmt;

			// generate a xquery for delete
			DeleteVisitor deleteVisitor = new DeleteVisitor(_dataView, _dataView.CurrentObjId);

			_dataView.Accept(deleteVisitor);

			stmt = deleteVisitor.XQueryStatement;

			return stmt.ToXQuery();
		}

		/// <summary>
		/// Get a query that updates the obj_id(s) of referenced instance(s) by an
		/// instance
		/// </summary>
		/// <param name="instanceData">The instance data</param>
		/// <returns></returns>
		public string GenerateReferenceUpdateQuery(InstanceData instanceData)
		{
			IQueryElement stmt;

			// generate a xquery
			UpdateReferencesVisitor updateReferencesVisitor = new UpdateReferencesVisitor(_dataView, instanceData);

			_dataView.Accept(updateReferencesVisitor);

			stmt = updateReferencesVisitor.XQueryStatement;

			return stmt.ToXQuery();
		}

        /// <summary>
        /// Gets a query that validates the data instance based on the given rule.
        /// </summary>
        /// <param name="ruleDef">The rule contains definition.</param>
        /// <returns>The query that validates a data instance.</returns>
        public string GenerateRuleValidatingQuery(RuleDef ruleDef)
        {
            IQueryElement stmt;

            // Prepare the condition expression contains in the rule for query generation
            if (ruleDef.Condition != null)
            {
                PrepareVisitor prepareVisitor = new PrepareVisitor(_dataView, false);
                ruleDef.Condition.Accept(prepareVisitor);
            }

            // generate a xquery for validate
            RuleValidateVisitor validateVisitor = new RuleValidateVisitor(_dataView, _dataView.CurrentObjId, ruleDef);

            _dataView.Accept(validateVisitor);

            stmt = validateVisitor.XQueryStatement;

            string query = stmt.ToXQuery();

            ruleDef.ObjId = null; // clear the obj id in the rule def

            return query;
        }
	}
}