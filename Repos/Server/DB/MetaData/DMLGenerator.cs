/*
* @(#)DMLGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;

    using Newtera.Common.Core;
	using Newtera.Server.DB;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// Generating standard DMLs that manipulate the meta model tables in database
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
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
		/// Get the DML for creating a record in mm_schema table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddSchemaDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("name"), "'" + (string) args[1] + "'");
			sql = sql.Replace(GetParamExpr("version"), "'" + (string) args[2] + "'");
			sql = sql.Replace(GetParamExpr("map_method"), (string) args[3]);

            // timestamp parameter
            string modifiedTimeStr = (string)args[4];
            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedTimeSql = lookup.GetTimestampFunc(modifiedTimeStr, LocaleInfo.Instance.DateTimeFormat);
            sql = sql.Replace(GetParamExpr("modified_time"), modifiedTimeSql);

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in mm_class table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddClassDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("name"), "'" + (string) args[1] + "'");
			sql = sql.Replace(GetParamExpr("display_name"), "'" + (string) args[2] + "'");
			sql = sql.Replace(GetParamExpr("table_name"), "'" + (string) args[3] + "'");
			sql = sql.Replace(GetParamExpr("schema_id"), (string) args[4]);
			sql = sql.Replace(GetParamExpr("pk_name"), "'" + (string) args[5] + "'");

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in mm_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("name"), "'" + (string) args[1] + "'");
			sql = sql.Replace(GetParamExpr("display_name"), "'" + (string) args[2] + "'");
			sql = sql.Replace(GetParamExpr("category"), (string) args[3]);
			sql = sql.Replace(GetParamExpr("column_name"), "'" + (string) args[4] + "'");
			sql = sql.Replace(GetParamExpr("class_id"), (string) args[5]);

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in mm_simple_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddSimpleAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("type"), System.Convert.ToString((int) args[1]));

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in mm_relation_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddRelationshipAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);
			//sql = sql.Replace(":type", System.Convert.ToString((int) args[1]));
			sql = sql.Replace(GetParamExpr("type"), "1"); // A fake value because of a constraint
			//sql = sql.Replace(":ownership", System.Convert.ToString((int) args[2]));
			sql = sql.Replace(GetParamExpr("ownership"), "1"); // A fake value because of a constraint
			sql = sql.Replace(GetParamExpr("ref_class_id"), (string) args[3]);
			sql = sql.Replace(GetParamExpr("is_fk"), (string) args[4]);
			sql = sql.Replace(GetParamExpr("is_jointable"), (string) args[5]);

			return sql;
		}

		/// <summary>
		/// Get the DML for creating a record in mm_parent_child table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetAddParentChildRelationDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("parent_id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("child_id"), (string) args[1]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_schema table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelSchemaDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_class table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelClassDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_simple_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelSimpleAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_relation_attribute table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelRelationshipAttributeDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("id"), (string) args[0]);

			return sql;
		}

		/// <summary>
		/// Get the DML for deleting a record in mm_parent_child table
		/// </summary>
		/// <param name="template">The sql template</param>
		/// <param name="args">The arguments of the sql</param>
		public string GetDelParentChildRelationDML(string template, params object[] args)
		{
			string sql = template;

			sql = sql.Replace(GetParamExpr("child_id"), (string) args[0]);
			sql = sql.Replace(GetParamExpr("parent_id"), (string) args[1]);

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
					return "@" + paramName;
                case DatabaseType.SQLServerCE:
                    return "@" + paramName;
                default:
					return ":" + paramName;
			}
		}
	}
}