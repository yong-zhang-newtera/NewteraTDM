/*
* @(#)AddSimpleAttributeHistoryEditAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
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
	/// Add the history edit status of an existing simple attribute in the database.
	/// </summary>
	/// <version> 1.0.0 27 Feb 2010 </version>
	/// <author> Yong Zhang</author>
	public class AddSimpleAttributeHistoryEditAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddSimpleAttributeHistoryEditAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSimpleAttributeHistoryEditAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddSimpleAttributeHistoryEdit;
			}
		}

		/// <summary>
		/// Prepare the action for creating full text index for a simple attribute in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Do nothing for now
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}