/*
* @(#)DeleteRelationshipAttributeIndexAction.cs
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
	/// Delete the index status of a simple attribute in the database.
	/// </summary>
	/// <version> 1.0.0 17 Nov 2010 </version>
	/// <author> Yong Zhang</author>
	public class DeleteRelationshipAttributeIndexAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteRelationshipAttributeIndexAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteRelationshipAttributeIndexAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteRelationshipAttributeIndex;
			}
		}

		/// <summary>
		/// Prepare the action for deleting indexing status of a simple attribute in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting indexing status of a simple attribute in database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();

			RelationshipAttributeElement attribute = (RelationshipAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string indexName = DBNameComposer.GetIndexName(attribute.OwnerClass, attribute);
			string ddl = generator.GetDelIndexDDL(indexName, attribute.OwnerClass.TableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();
		}
	}
}