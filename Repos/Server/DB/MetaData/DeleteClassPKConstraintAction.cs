/*
* @(#)DeleteClassPKConstraintAction.cs
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
	/// Delete the primary key constraint from an existing class.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteClassPKConstraintAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteClassPKConstraintAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteClassPKConstraintAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteClassPKConstraint;
			}
		}

		/// <summary>
		/// Prepare the action for deleting primary key constraint from a class.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting primary key constraint from a class.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			ClassElement classElement = (ClassElement) SchemaModelElement;

			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string constraintName = DBNameComposer.GetUniqueKeyName(classElement, null);

			string ddl = generator.GetDelUniqueConstraintDDL(constraintName, classElement.TableName);
			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			// drop index on the foreign key column
			/*
			ddl = generator.GetDelIndexDDL(constraintName, classElement.TableName);

			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();
			*/
		}
	}
}