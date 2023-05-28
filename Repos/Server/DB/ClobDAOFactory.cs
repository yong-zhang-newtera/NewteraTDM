/*
* @(#) ClobDAOFactory.cs
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
	/// Creates a ClobDAO based on the type of database because accessing to
	/// Clob data is differently handled.
	/// </summary>
	/// <version> 	1.0.0	23 Aug 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class ClobDAOFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static ClobDAOFactory theFactory;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private ClobDAOFactory()
		{
		}

		/// <summary>
		/// Gets the DataProviderFactory instance.
		/// </summary>
		/// <returns> The DataProviderFactory instance.</returns>
		static public ClobDAOFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a specific ClobDAO for a database.
		/// </summary>
		/// <param name="dataProvider">data provider.</param>
		/// <returns>A ClobDAO object.</returns>
		public IClobDAO Create(IDataProvider dataProvider)
		{
			IClobDAO clobDAO = null;

			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.SQLServer:
					clobDAO = new SQLServerClobDAO(dataProvider);
					break;
                case DatabaseType.SQLServerCE:
                    clobDAO = new SQLServerCEClobDAO(dataProvider);
                    break;
                case DatabaseType.Oracle:
					clobDAO = new OracleClobDAO(dataProvider);
					break;
				case DatabaseType.MySql:
					clobDAO = new MySqlClobDAO(dataProvider);
					break;
				default:
					throw new DBException("Not supported database type " + dataProvider.DatabaseType);
			}

			return clobDAO;
		}

		static ClobDAOFactory()
		{
			// Initializing the factory.
			{
				theFactory = new ClobDAOFactory();
			}
		}
	}
}