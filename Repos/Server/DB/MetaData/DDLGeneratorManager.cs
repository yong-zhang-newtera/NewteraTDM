/*
* @(#) DDLGeneratorManager.cs	1.0.1
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;

	using Newtera.Server.DB;

	/// <summary>
	/// A singleton class that provides a specific DDL generator based on
	/// the database type
	/// </summary>
	/// <version> 	1.0.1	12 10 2003</version>
	/// <author> 	Yong Zhang </author>
	public class DDLGeneratorManager
	{
		// The singleton object.
		private static DDLGeneratorManager theManager;

		// Oracle DDL generator.
		private OracleDDLGenerator _oraclDDLGenerator;

		// SQL Server DDL generator.
		private SQLServerDDLGenerator _sqlServerDDLGenerator;

        // SQL Server CE DDL generator.
        private SQLServerCEDDLGenerator _sqlServerCEDDLGenerator;

		// MySql DDL generator.
		private MySqlDDLGenerator _mySqlDDLGenerator;


		/// <summary>
		/// Private constructor.  User can not construct a DDLGeneratorManager
		/// </summary>
		private DDLGeneratorManager()
		{
			_oraclDDLGenerator = null;
			_sqlServerDDLGenerator = null;
		}

		/// <summary>
		/// Gets the DDLGeneratorManager instance.
		/// </summary>
		/// <returns> The DDLGeneratorManager instance.</returns>
		static public DDLGeneratorManager Instance
		{
			get
			{
				return theManager;
			}
		}

		/// <summary>
		/// Gets a DDLGenerator for the specific database type.
		/// </summary>
		/// <param name="dataProvider">The data provider for the specific database.</param>
		/// <returns>
		/// A IDDLGenerator instance.
		/// </returns>
		public IDDLGenerator GetDDLGenerator(IDataProvider dataProvider)
		{
			IDDLGenerator generator = null;

			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.Oracle:
					if (_oraclDDLGenerator == null)
					{
						_oraclDDLGenerator = new OracleDDLGenerator();
					}

					generator = _oraclDDLGenerator;
					break;
				case DatabaseType.SQLServer:
					if (_sqlServerDDLGenerator == null)
					{
						_sqlServerDDLGenerator = new SQLServerDDLGenerator();
					}

					generator = _sqlServerDDLGenerator;
					break;
                case DatabaseType.SQLServerCE:
                    if (_sqlServerCEDDLGenerator == null)
                    {
                        _sqlServerCEDDLGenerator = new SQLServerCEDDLGenerator();
                    }

                    generator = _sqlServerCEDDLGenerator;
                    break;
				case DatabaseType.MySql:
					if (_mySqlDDLGenerator == null)
					{
						_mySqlDDLGenerator = new MySqlDDLGenerator();
					}

					generator = _mySqlDDLGenerator;
					break;
			}
			
			return generator;
		}

		static DDLGeneratorManager()
		{
			// Initializing the manager.
		{
			theManager = new DDLGeneratorManager();
		}
		}
	}
}