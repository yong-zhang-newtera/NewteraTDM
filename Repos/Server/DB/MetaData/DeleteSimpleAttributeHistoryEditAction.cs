/*
* @(#)DeleteSimpleAttributeHistoryEditAction.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Delete the history edit index for an existing simple attribute.
	/// </summary>
	/// <version> 1.0.0 23 Feb 2010 </version>
	/// <author> Yong Zhang</author>
	public class DeleteSimpleAttributeHistoryEditAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteSimpleAttributeHistoryEditAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteSimpleAttributeHistoryEditAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider) : base(metaDataModel, element, dataProvider)
		{
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.DeleteSimpleAttributeHistoryEdit;
			}
		}

		/// <summary>
		/// Prepare the action for deleting full text search index for a simple attribute.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting a full text index
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}