/*
* @(#)SymbolLookupFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.DB;

	/// <summary>
	/// This a singleton class that creates a concrete SymbolLookup object according to a database type. 
	/// </summary>
	/// <version>  	1.0.1 23 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class SymbolLookupFactory
	{
		/* private static variables*/
		private OracleSymbolLookup _oracleLookup;
		private SQLServerSymbolLookup _sqlServerLookup;
        private SQLServerCESymbolLookup _sqlServerCELookup;
        private DB2SymbolLookup _db2Lookup;
		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static SymbolLookupFactory theSymbolLookupFactory;
		
		/// <summary>
		/// The private constructor.
		/// </summary>
		private SymbolLookupFactory()
		{
			_oracleLookup = new OracleSymbolLookup();
			_sqlServerLookup = new SQLServerSymbolLookup();
			_db2Lookup = new DB2SymbolLookup();
            _sqlServerCELookup = new SQLServerCESymbolLookup();
        }

		/// <summary>
		/// Gets the SymbolLookupFactory instance.
		/// </summary>
		/// <returns> The SymbolLookupFactory instance.</returns>
		static public SymbolLookupFactory Instance
		{
			get
			{
				return theSymbolLookupFactory;
			}
		}		
		
		/// <summary>
		/// This creates a concrete SymbolLooup object for a database. Currently, only
		/// oracle and DB2 database is supported.
		/// </summary>
		/// <param name="source">type of database.</param>
		/// <returns> a SymbolLookup object</returns>
		public virtual SymbolLookup Create(DatabaseType source)
		{
			switch (source)
			{
				case DatabaseType.Oracle:
					return _oracleLookup;
				case DatabaseType.SQLServer:
					return _sqlServerLookup;
                case DatabaseType.SQLServerCE:
                    return _sqlServerCELookup;
				case DatabaseType.DB2:
					return _db2Lookup;
				default:
					// Default lookup table
					return _oracleLookup;
			}
		}

		static SymbolLookupFactory()
		{
			// Initializing the document factory.
			{
				theSymbolLookupFactory = new SymbolLookupFactory();
			}
		}
	}
}