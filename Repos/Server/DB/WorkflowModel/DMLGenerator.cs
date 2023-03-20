/*
* @(#)DMLGenerator.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;
	using System.Text;

    using Newtera.Common.Core;
	using Newtera.Server.DB;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// Generating standard DMLs that manipulate the workflow model tables in database
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class DMLGenerator
	{
		IDataProvider _dataProvider;

		/// <summary>
		/// Instantiate an instance of DMLGenerator class.
		/// </summary>
		public DMLGenerator(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Get the DML for creating a record in wf_project table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddProjectDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("name"), "'" + (string) args[1] + "'");
            sql = sql.Replace(GetParamExpr("version"), "'" + (string)args[2] + "'");

            // timestamp parameter
            string modifiedTimeStr = (string)args[3];
            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedTimeSql = lookup.GetTimestampFunc(modifiedTimeStr, LocaleInfo.Instance.DateTimeFormat);
            sql = sql.Replace(GetParamExpr("modified_time"), modifiedTimeSql);

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in wf_workflow table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddWorkflowDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("name"), "'" + (string) args[1] + "'");
			sql = sql.Replace(GetParamExpr("type"), "'" + (string) args[2] + "'");
			sql = sql.Replace(GetParamExpr("class_name"), "'" + (string) args[3] + "'");
			sql = sql.Replace(GetParamExpr("project_id"), (string) args[4]);

			return sql;
		}

        /// <summary>
        /// Get the DML for deleting a record in wf_project table
        /// </summary>
        /// <param name="template">The sql template</param>
        /// <param name="args">The arguments of the sql</param>
        public string GetDelProjectDML(string template, params object[] args)
        {
            string sql = template;

            sql = sql.Replace(GetParamExpr("id"), (string)args[0]);

            return sql;
        }

        /// <summary>
        /// Get the DML for deleting a record in wf_worklfow table
        /// </summary>
        /// <param name="template">The sql template</param>
        /// <param name="args">The arguments of the sql</param>
        public string GetDelWorkflowDML(string template, params object[] args)
        {
            string sql = template;

            sql = sql.Replace(GetParamExpr("id"), (string)args[0]);

            return sql;
        }

		/// <summary>
		/// Get the parameter expression based on the type of database
		/// </summary>
		/// <param name="paramName">the parameter name</param>
		/// <returns>The parameter expression</returns>
		private string GetParamExpr(string paramName)
		{
			switch (_dataProvider.DatabaseType)
			{
				case DatabaseType.Oracle:
					return ":" + paramName;
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    return "@" + paramName;
				default:
					return ":" + paramName;
			}
		}
	}
}