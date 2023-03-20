/*
* @(#)DeleteClassAction.cs
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
	/// Add a new class from the database. It is responsible to create actions that
	/// perform individual task, such as deleting simple or relationship attributes, etc.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteClassAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteClassAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteClassAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteClass;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a class from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelClassDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetDelClassDML(sql, SchemaModelElement.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of deleting a class from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			ClassElement classElement = (ClassElement) SchemaModelElement;

			IDbCommand cmd = con.CreateCommand();

			// then delete the class
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);
			string ddl;

			// delete primary key constraint
			if (classElement.PrimaryKeys.Count > 0)
			{
				string constraintName = DBNameComposer.GetUniqueKeyName(classElement, null);

				ddl = generator.GetDelUniqueConstraintDDL(constraintName, classElement.TableName);
				cmd.CommandText = ddl;

				if (_log != null)
				{
					_log.Append(ddl, LogType.DDL);
				}

				cmd.ExecuteNonQuery();
			}

			ddl = generator.GetDelTableDDL(classElement.TableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();
		}
	}
}