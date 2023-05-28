/*
* @(#) MySqlProvider.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
	using MySqlConnector;
    using System.Diagnostics;

    using Newtera.Common.Core;

    /// <summary>
    /// Data provider for oracle.
    /// </summary>
    public class MySqlProvider : IDataProvider
	{
		private string _connectionString;

		/// <summary>
		/// Intiating an instance of MySqlProvider class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public MySqlProvider(string connectionString)
		{
			_connectionString = connectionString;
		}

		#region IDataProvider Members

		/// <summary>
		/// Gets a connection to the oracle database.
		/// </summary>
		public IDbConnection Connection
		{
			get
			{
				try
				{
					IDbConnection con = new MySqlConnection(_connectionString);
					con.Open();

					return con;
				}
				catch (MySqlException e)
				{
                    ErrorLog.Instance.WriteLine(e.Message + "\n" + e.StackTrace);

					throw new DBException(e.Message, e);
				}
                catch (Exception ex)
                {
                    throw ex;
                }
			}
		}

        /// <summary>
        /// Create a database
        /// </summary>
        public void CreateDataBase()
        {
            // do nothing
        }

        /// <summary>
        /// Get a DataAdapter for mysql database
        /// </summary>
        /// <param name="cmd">The command to be used by the adapter.</param>
        /// <returns>An adapter of IDataAdapter type</returns>
        public IDataAdapter GetDataAdapter(IDbCommand cmd)
		{
			return new MySqlDataAdapter((MySqlCommand) cmd);
		}

		/// <summary>
		/// Gets the database type 
		/// </summary>
		public DatabaseType DatabaseType
		{
			get
			{
				return DatabaseType.MySql;
			}
		}
		#endregion
	}
}