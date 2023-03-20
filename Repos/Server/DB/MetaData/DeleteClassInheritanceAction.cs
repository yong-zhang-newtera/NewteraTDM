/*
* @(#)DeleteClassInheritanceAction.cs
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
	/// Delete a class inheritance in the database. 
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteClassInheritanceAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteClassInheritanceAction.
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteClassInheritanceAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteClassInheritance;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a class inheritance in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			ClassElement classElement = (ClassElement) SchemaModelElement;

			if (!classElement.IsRoot)
			{
				IDbCommand cmd = con.CreateCommand();
				cmd.Transaction = trans;
				
				string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelParentChildRelationDML");
                if (!string.IsNullOrEmpty(sql))
                {
                    DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
				    sql = dmlGenerator.GetDelParentChildRelationDML(sql,
					    classElement.ID, classElement.ParentClass.ID);

                    cmd.CommandText = sql;

                    if (_log != null)
                    {
                        _log.Append(sql, LogType.DML);
                    }

                    cmd.ExecuteNonQuery();
                }
			}
		}

		/// <summary>
		/// Peform the action of deleting a class inheritance in database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			ClassElement classElement = (ClassElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string constraintName = DBNameComposer.GetFKConstraintName(classElement, true);
			string ddl = generator.GetDelFKConstraintDDL(constraintName, classElement.TableName);

            if (!string.IsNullOrEmpty(ddl))
            {
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