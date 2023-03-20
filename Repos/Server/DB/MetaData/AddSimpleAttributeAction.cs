/*
* @(#)AddSimpleAttributeAction.cs
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
	/// Add a new simple attribute to the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddSimpleAttributeAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddSimpleAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSimpleAttributeAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddSimpleAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for adding a simple attribute to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;

			// assign an unique id to the simple attribute
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

            try
            {
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
            catch (Exception ex)
            {
                throw ex;
            }
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
				SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
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