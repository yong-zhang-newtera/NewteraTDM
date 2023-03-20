/*
* @(#)SearchClassNameExecutor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Data;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Common.MetaData.XaclModel;
	using Newtera.Server.DB;

	/// <summary>
	/// Execute a query that returns distinct class names.
	/// </summary>
	/// <version>  	1.0.1 1 Jun 2007 </version>
	public class SearchClassNameExecutor : Executor
	{		
		private Interpreter _interpreter;
		
		/// <summary>
		/// Initiating SearchClassNameExecutor
		/// </summary>
		/// <param name="metaData">the schema model </param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder</param>
		/// <param name="interpreter">the interpreter</param>
		public SearchClassNameExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, Interpreter interpreter) : base(metaData, dataProvider, builder)
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
            IDataReader dataReader = null;
            string sql = "";

			try
			{
				// generate a SELECT SQL statement that returns distinct class ids			
				sql = _builder.GenerateDistinctClassIdSQL(queryInfo);
				
				SQLPrettyPrint.printSql(sql);
				
				IDbCommand cmd = con.CreateCommand();
				cmd.CommandText = sql;
                dataReader = cmd.ExecuteReader();

                StringCollection classNames = new StringCollection();
                string classId;
                ClassElement classElement;
                while (dataReader.Read())
                {
                    classId = dataReader.GetValue(0).ToString();
                    classElement = _metaData.SchemaModel.FindClassById(classId);
                    if (classElement != null && classElement.IsBrowsable &&
                        PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, classElement, XaclActionType.Read))
                    {
                        classNames.Add(classElement.Name);
                    }
                }
				
				// set the class name collection to the interpreter
                this._interpreter.ClassNames = classNames;
			}
			catch (Exception e)
			{
                if (e is System.Data.SqlClient.SqlException || e is System.Data.OracleClient.OracleException)
                {
                    throw new Exception(e.Message + ";\n The SQL is " + sql + "\n");
                }
                else
                {
                    throw e;
                }
			}
			finally
			{
                if (dataReader != null && !dataReader.IsClosed)
                {
                    dataReader.Close();
                }

				con.Close();
			}
		}
	}
}