/*
* @(#)EditInstanceVisitorBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Provide common utility for other editing instance related visitors
	/// </summary>
	/// <version> 1.0.0 12 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	internal class EditInstanceVisitorBase
	{
		internal DataViewModel _dataView;

		/// <summary>
		/// Initiate an instance of EditInstanceVisitorBase class
		/// </summary>
		/// <param name="dataView">The data view</param>
		public EditInstanceVisitorBase(DataViewModel dataView)
		{
			_dataView = dataView;
		}

		/// <summary>
		/// Build an inlined xml clause for a delete XQuery
		/// </summary>
		/// <returns>An IQueryElement</returns>
		/// <param name="objId">The id of an instance to be updated</param>
		protected InlinedXmlClause BuildInlinedXmlClause(string objId)
		{
			string rootClassType = _dataView.BaseClass.ClassName;

			// Get the root class of the base class
			ClassElement classElement = _dataView.SchemaModel.FindClass(rootClassType);
			rootClassType = classElement.RootClass.Name;

			InlinedXmlClause inlinedXmlClause = new InlinedXmlClause(_dataView.BaseClass.Alias,
				rootClassType, _dataView.BaseClass.ClassName, objId);

			return inlinedXmlClause;
		}

		/// <summary>
		/// Build a return clause for a delete XQuery
		/// </summary>
		/// <param name="type">The type of an edit built-in function</param>
		/// <returns>An IQueryElement</returns>
		protected IQueryElement BuildReturnClause(EditFunctionType type)
		{
			ICompositQueryElement returnClause = new ReturnClause();

			IQueryElement functionClause = new EditFunctionClause(type,
				_dataView.BaseClass.Alias, _dataView.SchemaInfo.Name,
				_dataView.SchemaInfo.Version);

			returnClause.Children.Add(functionClause);

			return (IQueryElement) returnClause;
		}
	}
}