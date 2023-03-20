/*
* @(#)AddArrayAttributeAction.cs
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
	/// Add a new array attribute to the database.
	/// </summary>
	/// <version> 1.0.0 10 Aug 2004 </version>
	/// <author> Yong Zhang</author>
	public class AddArrayAttributeAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddArrayAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddArrayAttributeAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddArrayAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for adding an array attribute to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			ArrayAttributeElement attribute = (ArrayAttributeElement) SchemaModelElement;

			// assign an unique id to the array attribute
			KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.AttributeId, MetaDataModel.SchemaInfo);
			attribute.ID = generator.NextKey().ToString();
			attribute.ColumnName = DBNameComposer.GetColumnName(attribute);

			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			// add a record in mm_attribute table
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddAttributeDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetAddAttributeDML(sql, attribute.ID,
				attribute.Name.ToUpper(), attribute.Caption, "1", /* 1 for simple attribute */
				attribute.ColumnName, attribute.OwnerClass.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();

			// Add a record in mm_simple_attribute table
			sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddSimpleAttributeDML");
			sql = dmlGenerator.GetAddSimpleAttributeDML(sql, attribute.ID,
				attribute.DataType);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of adding a column to a table
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			if (!SkipDDLExecution)
			{
				IDbCommand cmd = con.CreateCommand();
				ArrayAttributeElement attribute = (ArrayAttributeElement) SchemaModelElement;
				IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

				StringBuilder builder = new StringBuilder();

				builder.Append(generator.GetAddColumnHeaderDDL(attribute.OwnerClass.TableName));

				builder.Append(generator.GetAddColumnDDL(attribute));

				builder.Append(generator.GetAddColumnFooterDDL());

				string ddl = builder.ToString();
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