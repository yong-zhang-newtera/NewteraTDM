/*
* @(#)IDDLGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Represents an interface for DDL generator of various database types.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public interface IDDLGenerator
	{
		/// <summary>
		/// Convert data type from application-specific to database-specific.
		/// </summary>
		/// <param name="type">One of DataType values</param>
		/// <returns>A string representation of database type</returns>
		string GetDatabaseType(DataType type);

		/// <summary>
		/// Get the DDL for creating header part of Create Table statement
		/// </summary>
		/// <param name="tableName">The table name</param>
		string GetAddTableHeaderDDL(string tableName);

		/// <summary>
		/// Get the DDL for creating footer part of Create Table statement
		/// </summary>
		/// <returns>DDL for creating table footer</returns>
		string GetAddTableFooterDDL();

		/// <summary>
		/// Get the DDL for creating header of adding a column to a existing table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>The first part of DDL for adding a column</returns>
		string GetAddColumnHeaderDDL(string tableName);

		/// <summary>
		/// Get the DDL for creating footer of adding a column to a existing table
		/// </summary>
		/// <returns>The last part of DDL for adding a column</returns>
		string GetAddColumnFooterDDL();

		/// <summary>
		/// Get the DDL for creating header of modifying a column to a existing table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>The first part of DDL for modifying a column</returns>
		string GetModifyColumnHeaderDDL(string tableName);

		/// <summary>
		/// Get the DDL for creating footer of modifying a column to a existing table
		/// </summary>
		/// <returns>The last part of DDL for modifying a column</returns>
		string GetModifyColumnFooterDDL();

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="args">The vary-lengthed arguments necessary for creating a column</param>
		/// <returns>DDL for creating a table column</returns>
		string GetAddColumnDDL(params object[] args);

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="attribute">A SimpleAttributeElement</param>
		/// <returns>DDL for creating a table column</returns>
		string GetAddColumnDDL(SimpleAttributeElement attribute);

        /// <summary>
        /// Get the DDL for specifying a table column
        /// </summary>
        /// <param name="attribute">A ImageAttributeElement</param>
        /// <returns>DDL for creating a table column</returns>
        string GetAddColumnDDL(ImageAttributeElement attribute);

		/// <summary>
		/// Get the DDL for modifying a table column
		/// </summary>
		/// <param name="attribute">A SimpleAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column is modified</param>
		/// <returns>DDL for creating a table column</returns>
		string GetAddColumnDDL(SimpleAttributeElement attribute, ModifyFlag flag);

		/// <summary>
		/// Get the DDL for specifying a table column
		/// </summary>
		/// <param name="attribute">A ArrayAttributeElement</param>
		/// <returns>DDL for creating a table column</returns>
		string GetAddColumnDDL(ArrayAttributeElement attribute);

		/// <summary>
		/// Get the DDL for specifying a foreign key column
		/// </summary>
		/// <param name="attribute">A RelationshipAttributeElement</param>
		/// <returns>DDL for creating a foreign key column</returns>
		string GetAddFKColumnDDL(RelationshipAttributeElement attribute);

		/// <summary>
		/// Get the DDL for specifying a foreign key column
		/// </summary>
		/// <param name="attribute">A RelationshipAttributeElement</param>
		/// <param name="flag">The flag indicating which parts of column is modified</param>
		/// <returns>DDL for creating a foreign key column</returns>
		string GetAddFKColumnDDL(RelationshipAttributeElement attribute, ModifyFlag flag);

		/// <summary>
		/// Get the DDL for the header part of adding an unique constraint to a table
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <param name="constraintName">The unique constraint name</param>
		/// <returns>The first part of DDL for adding unique constraint</returns>
		string GetAddUniqueConstraintHeaderDDL(string tableName, string constraintName);

		/// <summary>
		/// Get the DDL for footer part of adding an unique constraint to a table.
		/// </summary>
		/// <returns>The last part of DDL for adding unique constraint</returns>
		string GetAddUniqueConstraintFooterDDL();

        /// <summary>
        /// Get a DLL for adding a default value constraint to a column
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <param name="constraintName">The unique name for the default value constraint</param>
        /// <param name="attribute">The simple attribute</param>
        /// <returns></returns>
        string GetAddDefaultValueConstraintDDL(string tableName, string constraintName, SimpleAttributeElement attribute);

        /// <summary>
        /// Get a DLL for deleting a default value constraint from a column
        /// </summary>
        /// <param name="tableName">The database table name</param>
        /// <param name="constraintName">The unique name for the default value constraint</param>
        /// <returns></returns>
        string GetDelDefaultValueConstraintDDL(string tableName, string constraintName);

		/// <summary>
		/// Gets the DDL of creating a foreign key constraint for a relationship.
		/// </summary>
		/// <param name="constraintName">The FK constraint name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="linkedTableName">The linked table name</param>
		/// <param name="fkColumnName">The foreign key column name</param>
		/// <param name="ownership">The relationship ownership</param>
		/// <returns></returns>
		string GetAddFKConstraintDDL(string constraintName, string tableName, string linkedTableName,
			string fkColumnName, RelationshipOwnership ownership);

		/// <summary>
		/// Get the DDL for creating an index for a column.
		/// </summary>
		/// <param name="indexName">The unique index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <returns>The DDL for creating an index</returns>
		string GetAddIndexDDL(string indexName, string tableName, string columnName);

		/// <summary>
		/// Get the DDLs for creating an sequnce for an auto-increment column.
		/// </summary>
		/// <param name="start">Sequence starting number</param>
		/// <param name="sequenceName">The sequence name</param>
		/// <param name="triggerName">The trigger name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <returns>The DDLs for sequnece</returns>
		string[] GetAddSequenceDDLs(int start, string sequenceName, string triggerName, string tableName, string columnName);

		/// <summary>
		/// Get the DDL for deleting a column.
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting table DDL</returns>
		string GetDelTableDDL(string tableName);

		/// <summary>
		/// Get the DDL for deleting a column.
		/// </summary>
		/// <param name="tableName">The name of table</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Deleting column DDL</returns>
		string GetDelColumnDDL(string tableName, string columnName);

		/// <summary>
		/// Get the DDL for deleting a foreign key constraint.
		/// </summary>
		/// <param name="constraintName">The name of constraint</param>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting fk constraint DDL</returns>
		string GetDelFKConstraintDDL(string constraintName, string tableName);

		/// <summary>
		/// Get the DDL for deleting a unique constraint.
		/// </summary>
		/// <param name="constraintName">The name of constraint</param>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting unique constraint DDL</returns>
		string GetDelUniqueConstraintDDL(string constraintName, string tableName);

		/// <summary>
		/// Get the DDL for deleting an index.
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The table name</param>
		/// <returns>Deleting index DDL</returns>
		string GetDelIndexDDL(string indexName, string tableName);

		/// <summary>
		/// Get the DDL for deleting a full text index.
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column that is full-text index</param>
		/// <returns>An array of deleting full text index DDLs</returns>
		string[] GetDelFullTextIndexDDLs(string indexName, string tableName, string columnName);

		/// <summary>
		/// Gets DDLs for deleting a sequnence for an auto-increment column
		/// </summary>
		/// <param name="sequenceName">The sequnence name</param>
		/// <param name="triggerName">The trigger name</param>
		/// <returns>The DDLs for deleting a sequnence</returns>
		string[] GetDelSequenceDDLs(string sequenceName, string triggerName);

		/// <summary>
		/// Get DDLs of creating a full text index for a column
		/// </summary>
		/// <param name="indexName">The full text index name</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <param name="dataStore">The data store type</param>
		/// <param name="isFilter">True if it has filter, false otherwise.</param>
		/// <returns>A array of DDL strings.</returns>
		string[] GetAddFullTextIndexDDLs(string indexName, string tableName, string columnName, string dataStore, bool isFilter);

		/// <summary>
		/// Get DDL for crating a full-text search index
		/// </summary>
		/// <param name="indexName">The index name</param>
		/// <param name="tableName">The name of table that owns the full-text search index</param>
		/// <param name="columnName">The name of column that is used in full-text search.</param>
		/// <returns></returns>
		string GetCreateFullTextIndexDDL(string indexName, string tableName, string columnName);

		/// <summary>
		/// Gets the DDLs for clear up full text config
		/// </summary>
		/// <returns>The DDLs for clear full text config</returns>
		string[] GetClearFullTextDDLs();

		/// <summary>
		/// Gets the DDLs for setting up full text config
		/// </summary>
		/// <returns>The DDLs for setting up full text config</returns>
		string[] GetFullTextSetupDDLs();

		/// <summary>
		/// Get DDL of creating a tablespace
		/// </summary>
		/// <param name="tableSpaceName">The tablespace name</param>
		/// <param name="dataFileDir">The directory of data file</param>
		/// <returns>A DDL of creating tablespace</returns>
		string GetCreateTablespaceDDL(string tableSpaceName, string dataFileDir);

		/// <summary>
		/// Get DDL of creating an user account.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="password">User password</param>
		/// <param name="tablespace">Tablespace name</param>
		/// <returns>A set of DDLs of creating user account</returns>
		string[] GetCreateUserDDLs(string userName, string password, string tablespace);

		/// <summary>
		/// Get SQL of searching a sequence.
		/// </summary>
		/// <param name="sequenceName">A sequence name</param>
		/// <returns>A SQL of searching a sequence.</returns>
		string GetFindSequenceSQL(string sequenceName);

		/// <summary>
		/// Get SQL of searching a trigger.
		/// </summary>
		/// <param name="triggerName">A trigger name</param>
		/// <returns>A SQL of searching a trigger.</returns>
		string GetFindTriggerSQL(string triggerName);
	}
}