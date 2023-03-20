/*
* @(#)SQLServerCEDDLGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using System.Data;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Represents a DDL generator for SQL Server database.
	/// </summary>
	/// <version> 1.0.0 17 Apr 2016 </version>
	public class SQLServerCEDDLGenerator : IDDLGenerator
	{
		/// <summary>
		/// Instantiate an instance of SQLServerCEDDLGenerator class.
		/// </summary>
		public SQLServerCEDDLGenerator()
		{
		}

		/// <summary>
		/// Convert data type from application-specific to SQLServer-specific.
		/// </summary>
		/// <param name="type">One of DataType values</param>
		/// <returns>A string representation of oracle database type</returns>
		public string GetDatabaseType(DataType type)
		{
			string dbType = null;

			switch (type)
			{
				case DataType.Boolean:
					dbType = "bit";
					break;
				case DataType.Byte:
					dbType = "smallint";
					break;
				case DataType.Date:
					dbType = "datetime";
					break;
				case DataType.DateTime:
					dbType = "datetime";
					break;
				case DataType.Decimal:
					dbType = "decimal";
					break;
				case DataType.Double:
					dbType = "real";
					break;
				case DataType.Float:
					dbType = "float";
					break;
				case DataType.Integer:
					dbType = "int";
					break;
				case DataType.BigInteger:
					dbType = "bigint";
					break;
				case DataType.String:
					dbType = "nvarchar";
					break;
				case DataType.Text:
					dbType = "ntext";
					break;
				default:
					dbType = "nvarchar";
					break;
			}

			return dbType;
		}

		/// <summary>
		/// Get the DDL for creating header part of Create Table statement
		/// </summary>
		/// <param name="tableName">The table name</param>
		public string GetAddTableHeaderDDL(string tableName)
		{
			return "create table " + tableName + "(";
		}

		/// <summary>
		/// Get the DDL for creating footer part of Create Table statement
		/// </summary>
		/// <returns>DDL for creating table footer</returns>
		public string GetAddTableFooterDDL()
		{
			return ")";
		}

		/// <summary>
		/// Get the DDL for creating header of adding a column to a existing table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>The first part of DDL for adding a column</returns>
		public string GetAddColumnHeaderDDL(string tableName)
		{
			return "alter table " + tableName + " add ";
		}

		/// <summary>
		/// Get the DDL for creating footer of adding a column to a existing table
		/// </summary>
		/// <returns>The last part of DDL for adding a column</returns>
		public string GetAddColumnFooterDDL()
		{
			return "";
		}

		/// <summary>
		/// Get the DDL for creating header of modifying a column to a existing table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>The first part of DDL for modifying a column</returns>
		public string GetModifyColumnHeaderDDL(string tableName)
		{
			return "alter table " + tableName + " alter column ";
		}

		/// <summary>
		/// Get the DDL for creating footer of modifying a column to a existing table
		/// </summary>
		/// <returns>The last part of DDL for modifying a column</returns>
		public string GetModifyColumnFooterDDL()
		{
			return "";
		}

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="args">The vary-lengthed arguments necessary for creating a column</param>
		/// <returns>DDL for creating a table column</returns>
		public string GetAddColumnDDL(params object[] args)
		{
			string classId = (string) args[0];
			string columnName = (string) args[1];
			DataType dataType = (DataType) args[2];

			bool isPrimaryKey = false;
			if (args.Length > 3)
			{
				isPrimaryKey = (bool) args[3];
			}

			string defaultValue = null;
			if (args.Length > 4)
			{
				defaultValue = (string) args[4];
			}

			int length = -1;
			if (args.Length > 5)
			{
				length = (int) args[5];
			}

			StringBuilder builder = new StringBuilder();
			
			builder.Append(columnName).Append(" ");
			string dbType = GetDatabaseType(dataType);
			if (length > 0)
			{
				// in Oracle each unicode uses too bytes, so double the size
				builder.Append(dbType).Append("(").Append(length*2).Append(") ");
			}
			else
			{
				builder.Append(dbType).Append(" ");
			}
			
			if (defaultValue != null)
			{
				builder.Append("default '").Append(defaultValue).Append("' ");
			}

			if (isPrimaryKey)
			{
				builder.Append("constraint ").Append(DBNameComposer.GetPrimaryKeyName(classId));
				builder.Append(" primary key");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="attribute">A SimpleAttributeElement</param>
		/// <returns>DDL for creating a table column</returns>
		public string GetAddColumnDDL(SimpleAttributeElement attribute)
		{
			return GenerateAddColumnDDL(attribute, ModifyFlag.All);
		}

        /// <summary>
        /// Get the DDL for specifying a table column
        /// </summary>
        /// <param name="attribute">A ImageAttributeElement</param>
        /// <returns>DDL for creating a table column</returns>
        public string GetAddColumnDDL(ImageAttributeElement attribute)
        {
            return GenerateAddColumnDDL(attribute, ModifyFlag.All);
        }

		/// <summary>
		/// Get the DDL for modifying a table column
		/// </summary>
		/// <param name="attribute">A SimpleAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column is modified</param>
		/// <returns>DDL for creating a table column</returns>
		public string GetAddColumnDDL(SimpleAttributeElement attribute, ModifyFlag flag)
		{
			return GenerateAddColumnDDL(attribute, flag);
		}

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="attribute">A ArrayAttributeElement</param>
		/// <returns>DDL for creating a table column</returns>
		public string GetAddColumnDDL(ArrayAttributeElement attribute)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(attribute.ColumnName).Append(" ");
			if (attribute.ArraySize == ArraySizeType.NormalSize)
			{
				string dbType = GetDatabaseType(attribute.DataType);
				builder.Append(dbType).Append("(").Append(attribute.ColumnLength).Append(") ");
			}
			else
			{
				// use text type to store overiszed array
				builder.Append("text ");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for specifying a foreign key column
		/// </summary>
		/// <param name="attribute">A RelationshipAttributeElement</param>
		/// <returns>DDL for creating a foreign key column</returns>
		public string GetAddFKColumnDDL(RelationshipAttributeElement attribute)
		{
			return GenerateAddFKColumnDDL(attribute, ModifyFlag.All);
		}

		/// <summary>
		/// Get the DDL for modifying a foreign key column
		/// </summary>
		/// <param name="attribute">A RelationshipAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column needs to be modified</param>
		/// <returns>DDL for creating a foreign key column</returns>
		public string GetAddFKColumnDDL(RelationshipAttributeElement attribute, ModifyFlag flag)
		{
			return GenerateAddFKColumnDDL(attribute, flag);
		}

		/// <summary>
		/// Get the DDL for the header part of adding an unique constraint to a table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <param name="constraintName">The name of constraint</param>
		/// <returns>The first part of DDL for adding unique constraint</returns>
		public string GetAddUniqueConstraintHeaderDDL(string tableName, string constraintName)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("alter table ").Append(tableName).Append(" add constraint ");
			builder.Append(constraintName).Append(" unique(");
			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for footer part of adding an unique constraint to a table.
		/// </summary>
		/// <returns>The last part of DDL for adding unique constraint</returns>
		public string GetAddUniqueConstraintFooterDDL()
		{
			return ")";
		}

        /// <summary>
        /// Get a DLL for adding a default value constraint to a column
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <param name="constraintName">The unique name for the default value constraint</param>
        /// <param name="attribute">The simple attribute</param>
        /// <returns></returns>
        public string GetAddDefaultValueConstraintDDL(string tableName, string constraintName, SimpleAttributeElement attribute)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("alter table ").Append(tableName).Append(" add constraint ");
            builder.Append(constraintName).Append(" default ");

            if (attribute.DataType == DataType.Date ||
                attribute.DataType == DataType.DateTime)
            {
                if (attribute.IsSystemTimeDefault)
                {
                    // use the system time as default
                    builder.Append("getdate()");
                }
                else
                {
                    builder.Append("convert(datetime, '").Append(attribute.DefaultValue).Append("')");
                }
            }
            else if (attribute.DataType == DataType.Boolean)
            {
                string val = "0";
                if (LocaleInfo.Instance.IsTrue(attribute.DefaultValue))
                {
                    val = "1";
                }

                builder.Append(" ").Append(val).Append(" ");
            }
            else
            {
                builder.Append("'").Append(attribute.DefaultValue).Append("'");
            }

            builder.Append(" for ").Append(attribute.ColumnName);
            return builder.ToString();
        }

        /// <summary>
        /// Get a DLL for deleting a default value constraint from a column
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <param name="constraintName">The unique name for the default value constraint</param>
        /// <returns></returns>
        public string GetDelDefaultValueConstraintDDL(string tableName, string constraintName)
        {
            /*
            StringBuilder builder = new StringBuilder();
            builder.Append("alter table ").Append(tableName).Append(" drop constraint ");
            builder.Append(constraintName);
            return builder.ToString();
            */
            return null;
        }

		/// <summary>
		/// Gets the DDL of creating a foreign key constraint for a relationship.
		/// </summary>
		/// <param name="constraintName">The FK constraint name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="linkedTableName">The linked table name</param>
		/// <param name="fkColumnName">The foreign key column name</param>
		/// <param name="ownership">The relationship ownership</param>
		/// <returns></returns>
		public string GetAddFKConstraintDDL(string constraintName, string tableName, string linkedTableName,
			string fkColumnName, RelationshipOwnership ownership)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("alter table ").Append(tableName).Append(" ");
			builder.Append("add constraint ").Append(constraintName).Append(" ");
			builder.Append("foreign key (").Append(fkColumnName).Append(") ");
			builder.Append("references ").Append(linkedTableName).Append("(OID) ");
			if (ownership == RelationshipOwnership.Owned)
			{
				builder.Append("on delete cascade ");
			}
			else if (ownership == RelationshipOwnership.LooselyReferenced)
			{
				builder.Append("on delete no action "); // SQL Server 2000
                //builder.Append("on delete set null "); // require SQL Server 2005 or up
			}

			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for creating an index for a column.
		/// </summary>
		/// <param name="indexName">The unique index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <returns>The first part of DDL for creating an index</returns>
		public string GetAddIndexDDL(string indexName, string tableName, string columnName)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append("create index ").Append(indexName).Append(" on ");
			builder.Append(tableName).Append("(").Append(columnName).Append(")");

			return builder.ToString();
		}

		/// <summary>
		/// Get the DDLs for creating an sequnce for an auto-increment column.
		/// </summary>
		/// <param name="start">Sequence starting number</param>
		/// <param name="sequenceName">The sequence name</param>
		/// <param name="triggerName">The trigger name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <returns>The DDLs for sequnece</returns>
		public string[] GetAddSequenceDDLs(int start, string sequenceName, string triggerName, string tableName, string columnName)
		{
			// SQL Server does not use a sequence and trigger to implement a
			// an auto-increment column, like the Oracle does.
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Get the DDL for deleting a column.
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting table DDL</returns>
		public string GetDelTableDDL(string tableName)
		{
			return "drop table " + tableName;
		}

		/// <summary>
		/// Get the DDL for deleting a column.
		/// </summary>
		/// <param name="tableName">The name of table</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Deleting column DDL</returns>
		public string GetDelColumnDDL(string tableName, string columnName)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("alter table ").Append(tableName).Append("   drop column  ").Append(columnName);
			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for deleting a foreign key constraint.
		/// </summary>
		/// <param name="constraintName">The name of constraint</param>
		/// <param name="tableName">The table name</param>
		/// <returns></returns>
		public string GetDelFKConstraintDDL(string constraintName, string tableName)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("alter table ").Append(tableName).Append("  drop constraint ").Append(constraintName);
			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for deleting a unique constraint.
		/// </summary>
		/// <param name="constraintName">The name of constraint</param>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting unique constraint DDL</returns>
		public string GetDelUniqueConstraintDDL(string constraintName, string tableName)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("alter table ").Append(tableName).Append("  drop constraint ").Append(constraintName);
			return builder.ToString();
		}

		/// <summary>
		/// Get the DDL for deleting an index.
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The table name</param>		
		/// <returns>Deleting index DDL</returns>
		public string GetDelIndexDDL(string indexName, string tableName)
		{
			return "drop index " + tableName + "." + indexName;
		}

		/// <summary>
		/// Get the DDL for deleting a full text index.
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column that is full-text indexed.</param>	
		/// <returns>Deleting full text index DDL</returns>
		public string[] GetDelFullTextIndexDDLs(string indexName, string tableName, string columnName)
		{
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Gets DDLs for deleting a sequnence for an auto-increment column
		/// </summary>
		/// <param name="sequenceName">The sequnence name</param>
		/// <param name="triggerName">The trigger name</param>
		/// <returns>The DDLs for deleting a sequnence</returns>
		public string[] GetDelSequenceDDLs(string sequenceName, string triggerName)
		{
			// SQL Server does not use a sequence and trigger to implement a
			// an auto-increment column, like the Oracle does.
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Get DDL of creating a full text index for a column
		/// </summary>
		/// <param name="indexName">The full text index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <param name="dataStore">The data store type</param>
		/// <param name="isFilter">True if it has filter, false otherwise.</param>
		/// <returns>An array of ddls</returns>
		public string[] GetAddFullTextIndexDDLs(string indexName, string tableName, string columnName, string dataStore, bool isFilter)
		{
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Get DDL for crating a full-text search index
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The name of table that owns the full-text search index</param>
		/// <param name="columnName">The name of column that is used in full-text search.</param>
		/// <returns></returns>
		public string GetCreateFullTextIndexDDL(string indexName, string tableName, string columnName)
		{
            return null;
		}

		/// <summary>
		/// Gets the DDLs for clear up full text config
		/// </summary>
		/// <returns>The DDLs for clear full text config</returns>
		public string[] GetClearFullTextDDLs()
		{
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Gets the DDLs for setting up full text config
		/// </summary>
		/// <returns>The DDLs for setting up full text config</returns>
		public string[] GetFullTextSetupDDLs()
		{
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="attribute">A SimpleAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column needs to be modified</param>
		/// <returns>DDL for creating a table column</returns>
		private string GenerateAddColumnDDL(SimpleAttributeElement attribute, ModifyFlag flag)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(attribute.ColumnName).Append(" ");
			string dbType = GetDatabaseType(attribute.DataType);
			int length = attribute.ColumnLength;
			if (length > 0)
			{
				// each unicode uses too bytes, so double the size
				builder.Append(dbType).Append("(").Append(length*2).Append(") ");
			}
			else
			{
				builder.Append(dbType).Append(" ");
			}

            if (attribute.IsAutoIncrement && !attribute.HasCustomValueGenerator)
			{
				// auto increment
				builder.Append("IDENTITY (1, 1) ");
			}

			return builder.ToString();
		}

        /// <summary>
        /// Get the DDL for specifying a table column
        /// </summary>
        /// <param name="attribute">A ImageAttributeElement</param>
        /// <param name="flag">The flag indicating which parts of column needs to be modified</param>
        /// <returns>DDL for creating a table column</returns>
        private string GenerateAddColumnDDL(ImageAttributeElement attribute, ModifyFlag flag)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(attribute.ColumnName).Append(" ");
            string dbType = GetDatabaseType(attribute.DataType);
            int length = attribute.ColumnLength;

            // each unicode uses too bytes, so double the size
            builder.Append(dbType).Append("(").Append(length * 2).Append(") ");

            return builder.ToString();
        }

		/// <summary>
		/// Get the DDL for specifying a foreign key column
		/// </summary>
		/// <param name="attribute">A RelationshipAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column needs to be modified</param>
		/// <returns>DDL for creating a foreign key column</returns>
		public string GenerateAddFKColumnDDL(RelationshipAttributeElement attribute, ModifyFlag flag)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(attribute.ColumnName).Append(" ");
			string dbType = GetDatabaseType(DataType.BigInteger); // The relationship type is Integer
			builder.Append(dbType).Append(" ");

			return builder.ToString();
		}

		/// <summary>
		/// Get DDL of creating a tablespace
		/// </summary>
		/// <param name="tableSpaceName">The tablespace name</param>
		/// <param name="dataFileDir">The directory of data file</param>
		/// <returns>A DDL of creating tablespace</returns>
		public string GetCreateTablespaceDDL(string tableSpaceName, string dataFileDir)
		{
			StringBuilder builder = new StringBuilder();

            string dbFile = dataFileDir + tableSpaceName;

			builder.Append("CREATE DATABASE \"" + dbFile + "\" DATABASEPASSWORD 'newtera' ");
           
			return builder.ToString();
		}

		/// <summary>
		/// Get DDL of creating an user account.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="password">User password</param>
		/// <param name="tablespace">Tablespace name</param>
		/// <returns>A set of DDLs of creating user account</returns>
		public string[] GetCreateUserDDLs(string userName, string password, string tablespaceName)
		{
			string[] ddls = new string[0];

			return ddls;
		}

		/// <summary>
		/// Get SQL of searching a sequence.
		/// </summary>
		/// <param name="sequenceName">A sequence name</param>
		/// <returns>A SQL of searching a sequence.</returns>
		public string GetFindSequenceSQL(string sequenceName)
		{
			return null;
		}

		/// <summary>
		/// Get SQL of searching a trigger.
		/// </summary>
		/// <param name="triggerName">A trigger name</param>
		/// <returns>A SQL of searching a trigger.</returns>
		/// <remarks>This method is for Oracle only</remarks>
		public string GetFindTriggerSQL(string triggerName)
		{
			return null;
		}
	}
}