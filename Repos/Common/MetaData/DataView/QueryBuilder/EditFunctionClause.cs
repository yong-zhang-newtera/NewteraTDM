/*
* @(#)EditFunctionClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents a clause of edit-related builtin function calls.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class EditFunctionClause : QueryElementBase
	{
		private EditFunctionType _functionType;
		private string _alias;
		private string _schemaName;
		private string _schemaVersion;

		/// <summary>
		/// Initiating an instance of EditFunctionClause class
		/// </summary>
		/// <param name="functionType">One of the EditFunctionType enum values</param>
		/// <param name="alias">The unique alias of a class</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		public EditFunctionClause(EditFunctionType functionType, string alias, string schemaName, string schemaVersion) : base()
		{
			_functionType = functionType;
			_alias = alias;
			_schemaName = schemaName;
			_schemaVersion = schemaVersion;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			switch (_functionType)
			{
				case EditFunctionType.AddInstance:
					query.Append("addInstance(");
					break;
				case EditFunctionType.DeleteInstance:
					query.Append("deleteInstance(");
					break;
				case EditFunctionType.UpdateInstance:
					query.Append("updateInstance(");
					break;
			}

			query.Append("document(\"db://");
			query.Append(_schemaName).Append(".xml");
			if (_schemaVersion != "1.0")
			{
				query.Append("?Version=").Append(_schemaVersion);
			}
			query.Append("\"), $").Append(_alias).Append(")\n");

			return query.ToString();
		}
	}

	/// <summary>
	/// Specify the types of edit-related builtin functions
	/// </summary>
	internal enum EditFunctionType
	{
		AddInstance,
		UpdateInstance,
		DeleteInstance
	}
}