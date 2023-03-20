/*
* @(#)CountExecutor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Data;
	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.DB;

	/// <summary>
	/// Execute a count for a query. This is done
	/// to tempararily provide a solution to get count info for a FLWR query.
	/// </summary>
	/// <version>  	1.0.1 3 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class CountExecutor : Executor
	{		
		private Interpreter _interpreter;
		
		/// <summary>
		/// Initiating CountExecutor
		/// </summary>
		/// <param name="metaData">the schema model </param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder</param>
		/// <param name="interpreter">the interpreter</param>
		public CountExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, Interpreter interpreter) : base(metaData, dataProvider, builder)
		{
			_interpreter = interpreter;
		}
		
		/// <summary>
		/// This method handles all the details of building a SQL statement for count,
		/// executing the SQL, convert the query result into count value.
		/// </summary>
		/// <param name="queryInfo">the information about a query</param>
		public virtual void Execute(QueryInfo queryInfo)
		{
			IDbConnection con = _dataProvider.Connection;

			try
			{
				// generate a SELECT SQL statement				
				string sql = _builder.GenerateCountSQL(queryInfo);
				
				SQLPrettyPrint.printSql(sql);

                IDbCommand cmd = con.CreateCommand();
				cmd.CommandText = sql;
				
				object count = cmd.ExecuteScalar();

                // set the count value to the interpreter
                this._interpreter.CountValue = System.Convert.ToInt32(count.ToString()); // set the count to interpreter
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				con.Close();
			}
		}
	}
}