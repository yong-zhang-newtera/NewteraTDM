/*
* @(#) DatabaseConfig.cs	1.0.1		2003-01-27
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Collections.Specialized;
	using System.Configuration;
	using Newtera.Common.Core;
	
	/// <summary>
	/// This is a singleton that managing the database configuration.
	/// It loads database configuration from Application Configuration file.
	/// </summary>
	/// <version> 	1.0.1	24 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	/// <remarks>The name of section is Schema_XXXXX, where XXXXX is the name of schema.</remarks>
	public class DatabaseConfig
	{
		private const string DATABASE_TYPE = "DataBaseType";
		private const string DATABASE_STRING = "DataBaseString";
		private const string IMAGE_BASE_URL = "ImageBaseURL";
		private const string IMAGE_BASE_PATH = "ImageBasePath";
		private const string IMAGE_RELATIVE_PATH = @"images\items\";
        private const string TRACE_LOG = "TraceLog";
		private const string WEB_SERVICE_BASE_URL = "WebServiceBaseURL";
		private const string USER_ID = "newtera";
		private const string USER_PASSWORD = "newtera";
		private const string TABLE_SPACE = "Newtera";
		private const string INDEX_TABLE_SPACE = "Newtera";
		private const string LEXER = "CHINESE_LEXER"; // default lexer

		// Static object, all invokers will use this factory object.
		private static DatabaseConfig theDatabaseConfig;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private DatabaseConfig()
		{
            string traceLogStatus = ConfigurationManager.AppSettings[TRACE_LOG];

            if (traceLogStatus != null && traceLogStatus.ToUpper() == "ON")
            {
                // turn on the trace log
                TraceLog.Instance.Enabled = true;
            }
            else
            {
                // turn off the trace log
                TraceLog.Instance.Enabled = false;
            }
		}

		/// <summary>
		/// Gets the DatabaseConfig instance.
		/// </summary>
		/// <returns> The DatabaseConfig instance.</returns>
		static public DatabaseConfig Instance
		{
			get
			{
				return theDatabaseConfig;
			}
		}
		
		/// <summary>
		/// Gets the type of database.
		/// </summary>
		/// <returns>One of database type enum values</returns>
		public DatabaseType GetDatabaseType()
		{
			if (ConfigurationManager.AppSettings == null)
			{
				throw new InvalidConfigurationException("No database settings available");
			}

			DatabaseType type = DatabaseType.SQLServer; // default DB type

            string dbType = ConfigurationManager.AppSettings[DATABASE_TYPE];
			
			if (dbType != null)
			{
				switch (dbType.ToUpper())
				{
					case "ORACLE":
						type = DatabaseType.Oracle;
						break;
					case "SQLSERVER":
						type = DatabaseType.SQLServer;
						break;
                    case "SQLSERVERCE":
                        type = DatabaseType.SQLServerCE;
                        break;
                    case "DB2":
						type = DatabaseType.DB2;
						break;
					default:
						break;
				}
			}

			return type;
		}

		/// <summary>
		/// Gets the type of database for a given string.
		/// </summary>
		/// <param name="databaseStr">The database type string</param>
		/// <returns>One of database type enum values</returns>
		public DatabaseType GetDatabaseType(string databaseStr)
		{			
			DatabaseType type = DatabaseType.Unknown;
			switch (databaseStr.ToUpper())
			{
				case "ORACLE":
					type = DatabaseType.Oracle;
					break;
				case "SQLSERVER":
					type = DatabaseType.SQLServer;
					break;
                case "SQLSERVERCE":
                    type = DatabaseType.SQLServerCE;
                    break;
                case "DB2":
					type = DatabaseType.DB2;
					break;
				default:
					break;
			}

			return type;
		}

		/// <summary>
		/// Gets the connection string of database
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>One of database type enum values</returns>
		public string GetConnectionString()
		{
            string connectionString = ConfigurationManager.AppSettings[DATABASE_STRING];
			
			return connectionString;
		}

		/// <summary>
		/// Get the table space name that store data
		/// </summary>
		/// <value> A table space name</value>
		public string TableSpace
		{
			get
			{
				return TABLE_SPACE;
			}
		}

		/// <summary>
		/// Get the key name for Database type in app config file
		/// </summary>
		/// <value> A key name for DataBase Type in config file</value>
		public string DatabaseTypeKey
		{
			get
			{
				return DATABASE_TYPE;
			}
		}

		/// <summary>
		/// Get the key name for Database string in app config file
		/// </summary>
		/// <value> A key name for DataBase string in config file</value>
		public string DatabaseStringKey
		{
			get
			{
				return DATABASE_STRING;
			}
		}

		/// <summary>
		/// Get the key name for Image Base URL in app config file
		/// </summary>
		/// <value> A key name for image base url in config file</value>
		public string ImageBaseURLKey
		{
			get
			{
				return IMAGE_BASE_URL;
			}
		}

		/// <summary>
		/// Get the key name for Image Base Path in app config file
		/// </summary>
		/// <value> A key name for image base path in config file</value>
		public string ImageBasePathKey
		{
			get
			{
				return IMAGE_BASE_PATH;
			}
		}

		/// <summary>
		/// Get the string for Image Relative Path
		/// </summary>
		/// <value> A string image relative path</value>
		public string ImageRelativePath
		{
			get
			{
				return IMAGE_RELATIVE_PATH;
			}
		}

		/// <summary>
		/// Get the base URL of the Web Services from the app config file
		/// </summary>
		public string WebServiceBaseURL
		{
			get
			{
                return ConfigurationManager.AppSettings[WEB_SERVICE_BASE_URL];
			}
		}


		/// <summary>
		/// Get the table space name that store indecies
		/// </summary>
		/// <value> A table space name</value>
		public string IndexTableSpace
		{
			get
			{
				return INDEX_TABLE_SPACE;
			}
		}

		/// <summary>
		/// Gets the DB user id for the specified Newtera tablespace
		/// </summary>
		public string DBUserID
		{
			get
			{
				return USER_ID;
			}
		}

		/// <summary>
		/// Gets the DB user password for the specified Newtera tablespace
		/// </summary>
		public string DBUserPassword
		{
			get
			{
				return USER_PASSWORD;
			}
		}

		/// <summary>
		/// Get the lexer for full text search
		/// </summary>
		/// <value> Lexer name</value>
		public string Lexer
		{
			get
			{
				return LEXER;
			}
		}

		static DatabaseConfig()
		{
			// Initializing the singleton.
			{
				theDatabaseConfig = new DatabaseConfig();
			}
		}
	}

	/// <summary>
	/// Describe the options of database types
	/// </summary>
	public enum DatabaseType
	{
		Unknown,
		SQLServer,
		Oracle,
        SQLServerCE,
		DB2
	}
}