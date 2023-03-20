/*
* @(#)DeleteSimpleAttributeAction.cs
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
	/// Delete a simple attribute from the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteSimpleAttributeAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteSimpleAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteSimpleAttributeAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteSimpleAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a simple attribute from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelSimpleAttributeDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetDelSimpleAttributeDML(sql, SchemaModelElement.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();

			sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelAttributeDML");
			sql = dmlGenerator.GetDelAttributeDML(sql, SchemaModelElement.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of deleting a simple attribute from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			if (!SkipDDLExecution) 
			{
				IDbCommand cmd = con.CreateCommand();
				SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
				IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

				string ddl = generator.GetDelColumnDDL(attribute.OwnerClass.TableName,
					attribute.ColumnName);

				cmd.CommandText = ddl;

				if (_log != null)
				{
					_log.Append(ddl, LogType.DDL);
				}

				cmd.ExecuteNonQuery();
			}
		}
	}
}