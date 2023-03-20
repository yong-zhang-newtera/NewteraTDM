/*
* @(#)DeleteRelationshipAttributeJoinTableAction.cs
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
	/// Delete a join table for an many-to-many relationship attribute.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteRelationshipAttributeJoinTableAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteRelationshipAttributeJoinTableAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteRelationshipAttributeJoinTableAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteRelationshipAttributeJoinTable;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a join table for a relationship attribute.
		/// </summary>
		/// <param name="dataProvider">The data provider for preparing the action</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting a join table a relationship attribute.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			RelationshipAttributeElement relationship = (RelationshipAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);
			string ddl;

			// get join table name
			string joinTableName = DBNameComposer.GetJoinedTableName(relationship, relationship.BackwardRelationship);

			// delete foreign key constraints of the join table
			string constraintName = DBNameComposer.GetFKConstraintName(relationship, false);
			ddl = generator.GetDelFKConstraintDDL(constraintName, joinTableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			constraintName = DBNameComposer.GetFKConstraintName(relationship.BackwardRelationship, false);
			ddl = generator.GetDelFKConstraintDDL(constraintName, joinTableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			ddl = generator.GetDelTableDDL(joinTableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();
		}
	}
}