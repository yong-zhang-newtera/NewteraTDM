/*
* @(#)CMCommandBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Cache;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// For cases where the SelectCommand is dynamically specified at runtime, you may not be
	/// able to specify the appropriate InsertCommand, UpdateCommand, or DeleteCommand at
	/// design time. If your DataTable maps to or is generated from a single class, you can
	/// take advantage of the CommandBuilder object to automatically generate the DeleteCommand,
	/// InsertCommand, and UpdateCommand of the DataAdapter.
	/// 
	/// As a minimum requirement, you must set the SelectCommand property in order for automatic
	/// command generation to work. The class schema retrieved by the SelectCommand determines the
	/// syntax of the automatically generated INSERT, UPDATE, and DELETE statements.
	/// 
	/// The CommandBuilder must execute the SelectCommand in order to return the class schema
	/// necessary to construct the insert, update, and delete commands. As a result, an extra
	/// trip to the data source is necessary which can hinder performance. To achieve optimal
	/// performance, specify your commands explicitly rather than using the CommandBuilder.
	/// 
	/// The SelectCommand must also return at least one primary key or unique column. If none
	/// are present, an InvalidOperation exception is generated, and the commands are not
	/// generated.
	/// 
	/// When associated with a DataAdapter, the CommandBuilder automatically generates the
	/// InsertCommand, UpdateCommand, and DeleteCommand properties of the DataAdapter if they
	/// are null references. If a Command already exists for a property, the existing Command
	/// is used.
	/// 
	/// Database views that are created by joining two or more tables together are not
	/// considered a single database table. In this instance you will not be able to use the
	/// CommandBuilder to automatically generate commands and will need to specify your
	/// commands explicitly. 
	/// 
	/// </summary>
	/// <version>  	1.0.0 26 April 2003 </version>
	/// <author>  		Yong Zhang </author>
	internal class CMCommandBuilder
	{
		//UPGRADE_NOTE: The initialization of  '_builder' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private QueryBuilder _builder;
		
		/// <summary> The constructor that CMDataAdapter as a parameter
		/// </summary>
		public CMCommandBuilder(CMDataAdapter adapter)
		{
			_builder = new QueryBuilder();
			
			bool missingCmd = false;
			
			CMCommand selectCmd = adapter.SelectCommand;
			
			// Check if the select cmd is available
			if (selectCmd == null)
			{
				throw new InvalidOperationException("A SelectCommand must be set");
			}
			
			// Check if there is a valid and open connection
			if (selectCmd.Connection != null || selectCmd.Connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
			
			if (adapter.DeleteCommand == null || adapter.InsertCommand == null || adapter.UpdateCommand == null)
			{
				
				missingCmd = true;
			}
			
			/*
			* if any one of delete, insert, and update commands are missing, we need to get
			* the meta data of the single class for generating those commands.
			*/
			if (missingCmd)
			{
				
				ClassEntity classEntity = GetClassEntity(selectCmd);
				
				if (adapter.DeleteCommand == null)
				{
					adapter.DeleteCommand = BuildDeleteCommand(classEntity, selectCmd.Connection);
				}
				
				if (adapter.InsertCommand == null)
				{
					adapter.InsertCommand = BuildInsertCommand(classEntity, selectCmd.Connection);
				}
				
				if (adapter.UpdateCommand == null)
				{
					adapter.UpdateCommand = BuildUpdateCommand(classEntity, selectCmd.Connection);
				}
			}
		}
		
		/// <summary>
		/// Build a CMCommand for delete instances from a class.
		/// </summary>
		/// <param name="classEntity">the class entity describing the class for which to generate
		/// delete command.
		/// </param>
		/// <param name="connection">the CM server connection.</param>
		/// <returns> the created delete command object </returns>
		public CMCommand BuildDeleteCommand(ClassEntity classEntity, CMConnection connection)
		{
			string cmdText = _builder.BuildDeleteStatement(classEntity);
			
			CMCommand cmd = new CMCommand(cmdText, connection);
			cmd.CommandType = CommandType.Text;
			
			return cmd;
		}
		
		/// <summary>
		/// Build a CMCommand for insert instance to a class.
		/// </summary>
		/// <param name="classEntity">the class entity describing the class for which to generate
		/// insert command.</param>
		/// <param name="connection">the CM server connection.</param>
		/// <returns>The created insert command object.</returns>
		public CMCommand BuildInsertCommand(ClassEntity classEntity, CMConnection connection)
		{
			string cmdText = _builder.BuildInsertStatement(classEntity);
			
			CMCommand cmd = new CMCommand(cmdText, connection);
			cmd.CommandType = CommandType.Text;
			
			return cmd;
		}
		
		/// <summary>
		/// Build a CMCommand for update operation
		/// </summary>
		/// <param name="classEntity">the class entity describing the class for which to generate
		/// update command.</param>
		/// <param name="connection">the CM server connection </param>
		/// <returns> the created update command object.</returns>
		public CMCommand BuildUpdateCommand(ClassEntity classEntity, CMConnection connection)
		{
			string cmdText = _builder.BuildUpdateStatement(classEntity);
			
			CMCommand cmd = new CMCommand(cmdText, connection);
			cmd.CommandType = CommandType.Text;
			
			return cmd;
		}
		
		/// <summary>
		/// Get the class entity for the class implied by the select statement. If the select
		/// statement involves joining of more than one classes, an exception is thrown.
		/// </summary>
		/// <param name="cmd">the select command</param>
		/// <returns> the class entity implied by the select cmd.</returns>
		private ClassEntity GetClassEntity(CMCommand cmd)
		{
			ClassEntity classEntity = null;
			CMConnection connection = cmd.Connection;
			
			if (cmd.CommandType == CommandType.Text)
			{
				// The command text is an XQuery for selecting
				// TODO
				//schema = connection.getEJBRemote().executeForSchema(selectCmd.getCommandText());
			}
			else if (cmd.CommandType == CommandType.TableDirect)
			{
				// The command text is a class name, find corresponding class element for the class
				SchemaInfo schemaInfo = connection.SchemaInfo;
				ClassElement classElement = MetaDataCache.Instance.GetMetaData(connection.SchemaInfo, connection.DataProvider).SchemaModel.FindClass(cmd.CommandText);
				
				// create an class entity for it
				classEntity = new ClassEntity(classElement);
			}
			else
			{
				// The command text is a stored query path
				string storedQuery = cmd.StoredQuery;
				// TODO
				//schema = connection.getEJBRemote().executeForSchema(selectCmd.getCommandText());
			}
			
			return classEntity;
		}
	}

	/// <summary>
	/// Build xqueries automatically for delete, insert, update using a class entity
	/// </summary>
	internal class QueryBuilder
	{
		/// <summary>
		/// Initiate an new instance of QueryBuilder class
		/// </summary>
		public QueryBuilder()
		{
		}

		/// <summary>
		/// Build a delete query.
		/// </summary>
		/// <param name="classEntity">The class entity describing the class</param>
		/// <returns>the built xquery</returns>
		public string BuildDeleteStatement(ClassEntity classEntity)
		{
			return null;
		}

		/// <summary>
		/// Build an insert xquery.
		/// </summary>
		/// <param name="classEntity">The class entity describing the class</param>
		/// <returns>The built xquery</returns>
		public string BuildInsertStatement(ClassEntity classEntity)
		{
			return null;
		}

		/// <summary>
		/// Build an select xquery.
		/// </summary>
		/// <param name="classEntity">The class entity describing the class</param>
		/// <returns>The built xquery</returns>
		public string BuildSelectStatement(ClassEntity classEntity)
		{
			return null;
		}

		/// <summary>
		/// Build an update xqury.
		/// </summary>
		/// <param name="classEntity">The class entity describing the class</param>
		/// <returns>The built xquery.</returns>
		public string BuildUpdateStatement(ClassEntity classEntity)
		{
			return null;
		}
	}
}