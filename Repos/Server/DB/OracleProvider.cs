/*
* @(#) OracleProvider.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
	using System.Data.OracleClient;
    using System.Diagnostics;

    using Newtera.Common.Core;

    /// <summary>
    /// Data provider for oracle.
    /// </summary>
    /// <version> 	1.0.0	15 Jul 2003 </version>
    /// <author> 	Yong Zhang </author>
    public class OracleProvider : IDataProvider
	{
		private string _connectionString;

		/// <summary>
		/// Intiating an instance of OracleProvider class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public OracleProvider(string connectionString)
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
					IDbConnection con = new OracleConnection(_connectionString);
					con.Open();

					return con;
				}
				catch (OracleException e)
				{
                    ErrorLog.Instance.WriteLine(e.Message + "\n" + e.StackTrace);

                    if (e.Code == 12154)
					{
						// Data Source does not exists
						throw new InvalidDataSourceException(e.Message);
					}
					else
					{
						throw new DBException(e.Message, e);
					}
				}
                catch (Exception ex)
                {
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
        /// Get a DataAdapter for oracle database
        /// </summary>
        /// <param name="cmd">The command to be used by the adapter.</param>
        /// <returns>An adapter of IDataAdapter type</returns>
        public IDataAdapter GetDataAdapter(IDbCommand cmd)
		{
			return new OracleDataAdapter((OracleCommand) cmd);
		}

		/// <summary>
		/// Gets the database type 
		/// </summary>
		public DatabaseType DatabaseType
		{
			get
			{
				return DatabaseType.Oracle;
			}
		}
		#endregion
	}
}