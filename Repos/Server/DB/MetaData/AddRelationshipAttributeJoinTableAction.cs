/*
* @(#)AddRelationshipAttributeJoinTableAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Add a join table for an many-to-many relationship attribute.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddRelationshipAttributeJoinTableAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddRelationshipAttributeJoinTableAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddRelationshipAttributeJoinTableAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddRelationshipAttributeJoinTable;
			}
		}

		/// <summary>
		/// Prepare the action for adding a join table for a many-to-many relationship attribute.
		/// </summary>
		/// <param name="dataProvider">The data provider for preparing the action</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			// DML for adding relationship attribute is done in AddRelationshipAttributeAction
		}

		/// <summary>
		/// Peform the action of adding a join table for a many-to-many relationship attribute.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			RelationshipAttributeElement relationship = (RelationshipAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);
			StringBuilder builder = new StringBuilder();

			// Create a DDL for creating a join table
			string joinTableName = DBNameComposer.GetJoinedTableName(relationship, relationship.BackwardRelationship);
			builder.Append(generator.GetAddTableHeaderDDL(joinTableName)).Append("\n");

			// add foreign key column pointing to the join manager class
			string fkName1 = DBNameComposer.GetForeignKeyName(relationship);
			builder.Append(generator.GetAddColumnDDL(null, fkName1, DataType.BigInteger)).Append(",\n");

			// add foreign key column pointing to the linked class
			string fkName2 = DBNameComposer.GetForeignKeyName(relationship.BackwardRelationship);
			builder.Append(generator.GetAddColumnDDL(null, fkName2, DataType.BigInteger)).Append("\n");			

			builder.Append(generator.GetAddTableFooterDDL()).Append("\n");

			string ddl = builder.ToString();
			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			// Add foreign key constraints to the join table
			string constraintName = DBNameComposer.GetFKConstraintName(relationship, false);
			ddl = generator.GetAddFKConstraintDDL(constraintName, joinTableName,
				relationship.OwnerClass.TableName, fkName1, RelationshipOwnership.Owned);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			constraintName = DBNameComposer.GetFKConstraintName(relationship.BackwardRelationship, false);
			ddl = generator.GetAddFKConstraintDDL(constraintName, joinTableName,
				relationship.BackwardRelationship.OwnerClass.TableName, fkName2,
				RelationshipOwnership.Owned);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();
		}
	}
}