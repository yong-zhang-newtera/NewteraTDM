/*
* @(#)AddRelationshipAttributeAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using System.Data;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Add a new relationship attribute to the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddRelationshipAttributeAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddRelationshipAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddRelationshipAttributeAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddRelationshipAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for adding a relationship attribute to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans)
		{
			RelationshipAttributeElement attribute = (RelationshipAttributeElement) SchemaModelElement;

			// assign an unique id to the relationship attribute
			KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.AttributeId, MetaDataModel.SchemaInfo);
			attribute.ID = generator.NextKey().ToString();
			attribute.ColumnName = DBNameComposer.GetForeignKeyName(attribute);

			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			// add a record in mm_attribute table
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddAttributeDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetAddAttributeDML(sql, attribute.ID,
				attribute.Name.ToUpper(), attribute.Caption, "2", /* 2 for relationship attribute */
				attribute.ColumnName, attribute.OwnerClass.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

            try
            {
                cmd.ExecuteNonQuery();

                // add a record in mm_relation_attribute table
                sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddRelationshipAttributeDML");
                sql = dmlGenerator.GetAddRelationshipAttributeDML(sql, attribute.ID,
                    DataType.Integer, attribute.Ownership,
                    attribute.LinkedClass.ID,
                    (attribute.IsForeignKeyRequired ? "1" : "0"),
                    "0");

                cmd.CommandText = sql;

                if (_log != null)
                {
                    _log.Append(sql, LogType.DML);
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

		/// <summary>
		/// Peform the action of adding a relationship attribute to database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}