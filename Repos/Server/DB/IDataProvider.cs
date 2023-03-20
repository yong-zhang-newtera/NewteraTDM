/*
* @(#) IDataProvider.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// A common interface for data provider instances.
	/// </summary>
	/// <version> 	1.0.1	12 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	public interface IDataProvider
	{
		/// <summary>
		/// Get a connection.
		/// </summary>
		IDbConnection Connection
		{
			get;
		}

        /// <summary>
        /// Create a database
        /// </summary>
        void CreateDataBase();

		/// <summary>
		/// Get a DataAdapter for a specific database
		/// </summary>
		/// <param name="cmd">The command to be used by the adapter.</param>
		/// <returns>An adapter of IDataAdapter type</returns>
		IDataAdapter GetDataAdapter(IDbCommand cmd);
		

		DatabaseType DatabaseType
		{
			get;
		}
	}
}