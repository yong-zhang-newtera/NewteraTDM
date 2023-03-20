/*
* @(#)AddSchemaAction.cs
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
	/// Add a new schema to the database. It is responsible to create actions that
	/// perform individual task, such as add class, etc.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddSchemaAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddSchemaAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSchemaAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddSchema;
			}
		}

		/// <summary>
		/// Prepare the action for adding a schema to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
            try {
                // assign an unique id to the schema
                KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.SchemaId, MetaDataModel.SchemaInfo);
                SchemaModelElement.ID = generator.NextKey().ToString();
                SchemaInfoElement schemaInfo = (SchemaInfoElement)SchemaModelElement;

                IDbCommand cmd = con.CreateCommand();
                cmd.Transaction = trans;

                string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddSchemaDML");
                DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
                // save the current system time as the modified time
                sql = dmlGenerator.GetAddSchemaDML(sql, schemaInfo.ID, schemaInfo.Name.ToUpper(), schemaInfo.Version, "1", DateTime.Now.ToString("s"));

                cmd.CommandText = sql;

                if (_log != null)
                {
                    _log.Append(sql, LogType.DML);
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Newtera.Common.Core.ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
		}

		/// <summary>
		/// Peform the action of global setup for a schema.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			// enable the full text to database if it has not been enabled
			if (!IsDBFullTextEnabled())
			{
				EnableDBFullText(con); // enable the fulltext
			}
		}

		/// <summary>
		/// Gets the information indicating whether the database has been enabled for full-text search or not
		/// </summary>
		/// <returns>True if it has been enabled, false otherwise</returns>
		private bool IsDBFullTextEnabled()
		{
			// get enabling flag from the database
			MetaDataAdapter adapter = new MetaDataAdapter(_dataProvider);

			return adapter.IsFullTextEnabled();
		}

		/// <summary>
		/// Enable the database for the full-text search
		/// </summary>
		private void EnableDBFullText(IDbConnection con)
		{
			// get a database connection
			IDbCommand cmd = con.CreateCommand();
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			// Clear the full text setup, it is a hack for fixing the problem in Oracle.
			string[] ddls = generator.GetClearFullTextDDLs();
			for (int i = 0; i < ddls.Length; i++)
			{
				cmd.CommandText = ddls[i];

				if (_log != null)
				{
					_log.Append(ddls[i], LogType.DML);
				}

				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					// ignore the full-text cleanup errors
				}
			}

			ddls = generator.GetFullTextSetupDDLs();

			for (int i = 0; i < ddls.Length; i++)
			{
				cmd.CommandText = ddls[i];

				if (_log != null)
				{
					_log.Append(ddls[i], LogType.DML);
				}

				cmd.ExecuteNonQuery();
			}
		}
	}
}