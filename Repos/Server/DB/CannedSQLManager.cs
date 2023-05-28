/*
* @(#) CannedSQLManager.cs	1.0.1		2001-09-07
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
	using System.Text.RegularExpressions;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml;

	using Newtera.Common.Core;

	/// <summary>
	/// The class CannedSQLManager is for fetching predefined SQL strings
	/// used in the program.  The SQL strings are stored name/value pairs in
	/// the canned sql file. There is a separate SQL statement for each
	/// version of database supported by the catalog. One can get a
	/// a canned SQL by first getting a specific CannedSQLManager and an unique
	/// name that identifies the SQL string.
	/// 
	/// The following code sample demonstrats how Canned SQLs are stored in
	/// a canned sql file
	/// 
	/// <sqls>
	///		<sql key="GetClobById" value="SELECT {COLUMN_NAME} FROM {TABLE_NAME} WHERE ID = @id"/>
	///		<sql key="GetTableName" value="SELECT NAME, TABLE_NAME FROM MM_CLASS WHERE SCHEMA_ID = @id"/>
	///		...
	/// </sqls>
	/// 
	/// </summary>
	/// <version> 	1.0.1	12 Aug 2003</version>
	/// <author> 	Yong Zhang </author>
	public class CannedSQLManager
	{
		// File names for specific sqls of databases
		public const string CONFIG_DIR = @"\Config\";
		public const string OracleCannedSQLFile = "catalog_sqls_oracle.xml";
		public const string SQLServerCannedSQLFile = "catalog_sqls_sqlserver.xml";
        public const string SQLServerCECannedSQLFile = "catalog_sqls_sqlserverce.xml";
		public const string MySqlCannedSQLFile = "catalog_sqls_mysql.xml";

		// A table contains SQL strings that are indexed by keys.
		private NameValueCollection _sqlTable;
		
		// A hash table conatins CannedSQLManager objects
		private static Hashtable _cannedSqlManagers;
		
		/// <summary> Private constructor.  User can not construct a
		/// <code>CannedSQLManager</code>.
		/// </summary>
		private CannedSQLManager()
		{
			_sqlTable = null;
		}

		/// <summary>
		/// Gets a CannedSQLManager instance for the specific database type.
		/// </summary>
		/// <param name="dataProvider">The data provider for the specific database.</param>
		/// <returns>
		/// A CannedSQLManager instance that is loaded with 
		/// the canned sqls defined for the specific database.
		/// </returns>
		public static CannedSQLManager GetCannedSQLManager(IDataProvider dataProvider)
		{
			string fileName = GetFileName(dataProvider);

			/* 
			* If the CannedSQLManager of the provider is not in the
			* hash table, the created one and store it in the
			* hash table.
			*/
			if (!_cannedSqlManagers.ContainsKey(fileName))
			{
				CannedSQLManager cannedSqlMgr = new CannedSQLManager();
				cannedSqlMgr.Load(fileName);
				_cannedSqlManagers[fileName] = cannedSqlMgr;
			}

			/*
			* Gets the CannedSQLManager from the hash map by the tag
			* provider, and returns it.
			*/
			return (CannedSQLManager) _cannedSqlManagers[fileName];
		}
		
		/// <summary>
		/// Gets a SQL string indexed by a string key.
		/// The method converts a string in the canned sql file
		/// indexed by the parameter to an executable SQL.
		/// The variable in the raw string indexd by the parameter 
		/// does not contain any variable.  The result string only
		/// convert RT or LF or TAB to a SPACE charactor.
		/// </summary>
		/// <param name="sqlName">defined by user, indexes a special SQL.</param>
		/// <returns>a SQL string</returns>
		/// <exception cref="">	SQLNotFoundException
		/// thrown if cannot find the SQL string using
		/// parameter <code>sqlName</code>.
		/// </exception>
		public string GetSql(string sqlName)
		{
			string sql = _sqlTable[sqlName];

			if (sql == null)
			{
				throw new SQLNotFoundException("The sql " + sqlName + " does not exist.");
			}
						
			return sql;
		}
		
		/// <summary>
		/// Gets a SQL string indexed by a string key, and replaces the
		/// varaible in the orginal string by the parameter
		/// <code>param</code> in order.
		/// The method converts a string in the SQL configuraion file
		/// indexed by the parameter to an executable SQL.
		/// The raw string containes variables needed to be replace
		/// by fact values.  The variable in the raw
		/// string will be placed by the parameter <code>params</code>.
		/// The paramter <code>params</code> should contains values that
		/// will replace variable in orignal string in order.
		/// The variables start with charactor '$'
		/// For example, the orignal string indexed by the key 'sql1' in the
		/// canned sql file is:
		/// 
		/// SELECT $COL1, $COL2 FROM $TABLE WHERE $COL1 = ?
		/// 
		/// The arrary of string contains fact values in order.
		/// 
		/// CannedSQLManager cannedSql = CannedSql.getCannedSQL();
		/// String[] params = {"product_num", "ship_to", "orders", "product_num"};
		/// String sql = cannedSql.getSql("sql1", param);
		/// 
		/// The result string is:
		/// 
		/// SELECT product_num, ship_to FROM orders WHERE product_num = ?
		/// 
		/// </summary>
		/// <param name="sqlName">defined by user, indexes a special SQL.</param>
		/// <param name="params	contains">values that is used to replace
		/// variables in the orginal string in order.</param>
		/// <returns>the result string, all variables in the orginal string
		/// has been replaced.</returns>
		/// <exception cref="">	SQLNotFoundException
		/// thrown if cannot find the SQL string using
		/// parameter <code>sqlName</code>.
		/// </exception>
		/// <exception cref="">	MissingParameterException
		/// if the parameters does not contain enough values of
		/// variable.
		/// </exception>
		public string GetSql(string sqlName, string[] parameters)
		{
			string rawSql = _sqlTable[sqlName];
			
			if (rawSql == null)
			{
				throw new SQLNotFoundException("Failed to find a canned sql for " + sqlName);
			}
			
			// Converts raw SQL string using parameters of array in order.
			string sql = GetPackedSql(rawSql, parameters);
			return sql;
		}
		
		/// <summary>
		/// Gets a SQL string indexed by a string key, and replaces the
		/// varaible in the orginal string by the parameter that contains
		/// tags/values that is used to replace	variables in the
		/// orginal string.
		/// The method converts a string in the canned sql file
		/// indexed by the parameter to an executable SQL.
		/// The raw SQL indexed by the parameter containes variables
		/// needed to be replace by fact values.  The variable in the raw
		/// string will be placed by the parameter <code>params</code>.
		/// The paramter <code>params</code> should contains tags that
		/// is the same as those defined in the orginal string using
		/// variable starting with charactor '$'
		/// For example, the orignal string in the canned sql file
		/// indexed by the key 'sql1' is:
		/// <pre>
		/// SELECT $COL1, $COL2 FROM $TABLE WHERE $COL1 = ?
		/// </pre>
		/// The <code>java.util.Properties</code> object <code>params</code>
		/// should be initialized with tags COL1, COL2 and TABLE and their
		/// values:
		/// <pre>
		/// CannedSQLManager cannedSql = CannedSql.getCannedSQL();
		/// Properties params = new Hashtable();
		/// 
		/// params["COL1"] = "product_num";
		/// params["COL2"] = "ship_to";
		/// params["TABLE"] = "orders";
		/// 
		/// String sql = cannedSql.getSQL('sql1', params);
		/// 
		/// The result string is:
		/// 
		/// SELECT product_num, ship_to FROM orders WHERE product_num = ?
		/// </summary>
		/// <param name="sqlName">defined by user, indexes a special SQL.</param>
		/// <param name="params	contains">tags/values that is used to replace
		/// variables in the orginal string.</param>
		/// <returns>the result string, all variables in the orginal string
		/// has been replaced.</returns>
		/// <exception cref="">	SQLNotFoundException
		/// thrown if cannot find the SQL string using
		/// parameter <code>sqlName</code>.
		/// </exception>
		/// <exception cref="">	MissingParameterException
		/// if the parameters does not contain enough values of variable.</exception>
		public string GetSql(string sqlName, Hashtable parameters)
		{
			string rawSql = _sqlTable[sqlName];

			if (rawSql == null)
			{
				throw new SQLNotFoundException("Failed to find a canned sql for " + sqlName);
			}

			// Converts raw SQL string using parameters of Properties.
			string sql = GetPackedSql(rawSql, parameters);
			return sql;
		}
		
		/// <summary>
		/// Loads SQL strings from the canned sql file, indicated by the file name.
		/// </summary>
		/// <param name="fileName">Name of the canned sql file.</param>
		private void Load(string fileName)
		{
			string fullPath = NewteraNameSpace.GetAppHomeDir() + CannedSQLManager.CONFIG_DIR + fileName;

			try
			{
				_sqlTable = new NameValueCollection();

				XmlDocument doc = new XmlDocument();

				doc.Load(fullPath);

				XmlNodeList sqlNodes = doc.DocumentElement.ChildNodes;

				foreach (XmlElement sqlElement in sqlNodes)
				{
					string key = sqlElement.GetAttribute("key");
					string val = sqlElement.GetAttribute("value");

					_sqlTable.Add(key, val);
				}
			}
			catch (Exception)
			{
				throw new Exception(fullPath + " is an invalid xml file.");
			}
		}
		
		/// <summary>
		/// Converts a string that containes variables needed to be replace
		/// by fact values to an executable SQL.  The variable in the raw
		/// string will be placed by the parameter <code>params</code>.
		/// The paramter <code>params</code> should contains values that
		/// will replace variable in orignal string in order.
		/// The variables start with charactor '$'.
		/// </summary>
		/// <param name="rawSql	the">orginal string that is need to be converted.</param>
		/// <param name="params	contains">values that is used to replace
		/// variables in the orginal string in order.</param>
		/// <returns>	the result string, all variables in the orginal string
		/// has been replaced.</returns>
		/// <exception cref="MissingParameterException">	
		/// if the parameters does not contain enough values of variable. 
		/// </exception>
		private string GetPackedSql(string rawSql, string[] parameters)
		{
			Regex regex = new Regex(@"$\w+");
			ReplaceHandler handler = new ReplaceHandler(parameters);

			return regex.Replace(rawSql, new MatchEvaluator(handler.ReplaceWithArray));
		}
		
		/// <summary>
		/// Converts a string that containes variables needed to be replace
		/// by fact values to an executable SQL.  The variable in the raw
		/// string will be placed by the parameter <code>params</code>.
		/// The paramter <code>params</code> should contains tags that
		/// is the same as those defined in the orginal string using
		/// variable starting with charactor '$'.
		/// </summary>
		/// <param name="rawSql	the">orginal string that is need to be converted.</param>
		/// <param name="params	contains">tags/values that is used to replace
		/// variables in the orginal string.</param>
		/// <returns>	the result string, all variables in the orginal string
		/// has been replaced.</returns>
		/// <exception cref="">	MissingParameterException
		/// if the parameters does not contain enough values of
		/// variable. 
		/// </exception>
		private string GetPackedSql(string rawSql, Hashtable parameters)
		{
			Regex regex = new Regex(@"{\w+}");
			ReplaceHandler handler = new ReplaceHandler(parameters);

			return regex.Replace(rawSql, new MatchEvaluator(handler.ReplaceWithHashtable));
		}

		/// <summary>
		/// Gets the section name in the canned sql file that represents
		/// a section of canned sqls for a specific database.
		/// </summary>
		/// <param name="dataProvider">The data provider for a specific database</param>
		/// <returns>The section name</returns>
		private static string GetFileName(IDataProvider dataProvider)
		{
			string fileName = null;

			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.Oracle:
					fileName = CannedSQLManager.OracleCannedSQLFile;
					break;
				case DatabaseType.SQLServer:
					fileName = CannedSQLManager.SQLServerCannedSQLFile;
					break;
                case DatabaseType.SQLServerCE:
                    fileName = CannedSQLManager.SQLServerCECannedSQLFile;
                    break;
				case DatabaseType.MySql:
					fileName = CannedSQLManager.MySqlCannedSQLFile;
					break;
			}

			return fileName;
		}

		static CannedSQLManager()
		{
			_cannedSqlManagers = new Hashtable();
		}
	}

	/// <summary>
	/// Sql variable replace handler
	/// </summary>
	internal class ReplaceHandler
	{
		private string[] _parameterArray;
		private int _index;
		private Hashtable _parameterTable;

		public ReplaceHandler(string[] parameters)
		{
			_parameterArray = parameters;
			_index = 0;
		}

		public ReplaceHandler(Hashtable parameters)
		{
			_parameterTable = parameters;
		}

		/// <summary>
		/// Regex replace delegate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public string ReplaceWithArray(Match match)
		{
			if (_index < _parameterArray.Length)
			{
				return _parameterArray[_index++];
			}
			else
			{
				return match.Value;
			}
		}

		/// <summary>
		/// Regex replace delegate
		/// </summary>
		public string ReplaceWithHashtable(Match match)
		{
			// get rid of {} characters at begining and end
			string name = match.Value.Substring(1, match.Value.Length - 2);
			string val = (string) _parameterTable[name];

			if (val != null)
			{
				return val;
			}
			else
			{
				throw new MissingParameterException("Parameter " + name + " is missing from the sql");
			}
		}
	}
}