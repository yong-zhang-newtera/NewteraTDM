/*
* @(#) DataProviderFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Data.OracleClient;
	using Newtera.Common.Core;

	/// <summary>
	/// Creates a DataProvider based on settings in the application configuration file.
	/// </summary>
	/// <version> 	1.0.0	23 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class DataProviderFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static DataProviderFactory theFactory;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private DataProviderFactory()
		{
		}

		/// <summary>
		/// Gets the DataProviderFactory instance.
		/// </summary>
		/// <returns> The DataProviderFactory instance.</returns>
		static public DataProviderFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a specific DataProvider for a schema.
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>A DataProvider object for the schema.</returns>
		public IDataProvider Create()
		{
            IDataProvider provider = null;

            DatabaseConfig dbConfig = DatabaseConfig.Instance;

            switch (dbConfig.GetDatabaseType())
            {
                case DatabaseType.SQLServer:
                    provider = new SQLServerProvider(dbConfig.GetConnectionString());
                    break;
                case DatabaseType.Oracle:
                    provider = new OracleProvider(dbConfig.GetConnectionString());
                    break;
                case DatabaseType.SQLServerCE:
                    provider = new SQLServerCEProvider(dbConfig.GetConnectionString());
                    break;
            }

            return provider;
		}

		/// <summary>
		/// Creates a specific DataProvider, given a database type and connection string.
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>A DataProvider object for the schema.</returns>
		public IDataProvider Create(DatabaseType type, string connectionString)
		{
			IDataProvider provider = null;
			
			switch (type)
			{
				case DatabaseType.SQLServer:
					provider = new SQLServerProvider(connectionString);
					break;
				case DatabaseType.Oracle:
					provider = new OracleProvider(connectionString);
					break;
                case DatabaseType.SQLServerCE:
                    provider = new SQLServerCEProvider(connectionString);
                    break;
            }

            if (type != DatabaseType.SQLServerCE)
            {
                IDbConnection con = provider.Connection;
            }

            return provider;
        }

		static DataProviderFactory()
		{
			// Initializing the factory.
			{
				theFactory = new DataProviderFactory();
			}
		}
	}
}