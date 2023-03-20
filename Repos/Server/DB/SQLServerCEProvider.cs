/*
* @(#) SQLServerCEProvider.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
    using System.Data.SqlServerCe;
    using System.Diagnostics;

    using Newtera.Common.Core;

	/// <summary>
	/// Data provider for SQL server.
	/// </summary>
	/// <version> 	1.0.0	15 Apr 2016 </version>
	public class SQLServerCEProvider : IDataProvider
	{
		private string _connectionString;

		/// <summary>
		/// Intiating an instance of SQLServerCEProvider class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public SQLServerCEProvider(string connectionString)
		{
			_connectionString = connectionString;
		}

		#region IDataProvider Members

		/// <summary>
		/// Gets a connection to the sql server database.
		/// </summary>
		public IDbConnection Connection
		{
			get
			{
                try
                {
                    IDbConnection con = new SqlCeConnection(_connectionString);
                    con.Open();

                    return con;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    /*
                    StackTrace stackTrace = new StackTrace();           // get call stack
                    StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                    String output = "";
                    // write call stack method names
                    foreach (StackFrame stackFrame in stackFrames)
                    {
                        output += stackFrame.GetMethod().Name + "\n";
                    }

                    ErrorLog.Instance.WriteLine("Call stack : " + output);
                    */

                    throw new InvalidDataSourceException(ex.Message);
                }
			}
		}

        /// <summary>
        /// Create a database
        /// </summary>
        public void CreateDataBase()
        {
            try
            {
                SqlCeEngine engine = new SqlCeEngine(_connectionString);
                engine.CreateDatabase();
                engine.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Get a DataAdapter for sql server database
        /// </summary>
        /// <param name="cmd">The command to be used by the adapter.</param>
        /// <returns>An adapter of IDataAdapter type</returns>
        public IDataAdapter GetDataAdapter(IDbCommand cmd)
		{
			return new SqlCeDataAdapter((SqlCeCommand) cmd);
		}

		/// <summary>
		/// Gets the database type 
		/// </summary>
		public DatabaseType DatabaseType
		{
			get
			{
				return DatabaseType.SQLServerCE;
			}
		}

		#endregion
	}
}