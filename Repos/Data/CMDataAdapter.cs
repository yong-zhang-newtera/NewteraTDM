/*
* @(#)CMDataAdapter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
	using System.Data.Common;
	
	/// <summary> Aids implementation of the IDbDataAdapter interface. Inheritors of DbDataAdapter
	/// implement a set of functions to provide strong typing, but inherit most of the
	/// functionality needed to fully implement a DataAdapter.
	/// 
	/// </summary>
	/// <version>  	1.0.0 30 April 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class CMDataAdapter : DbDataAdapter, IDbDataAdapter
	{
		private CMCommand _deleteCommand = null;
		private CMCommand _insertCommand = null;
		private CMCommand _selectCommand = null;
		private CMCommand _updateCommand = null;

		/*
		 * Inherit from Component through DbDataAdapter. The event
		 * mechanism is designed to work with the Component.Events
		 * property. These variables are the keys used to find the
		 * events in the components list of events.
		 */
		static private readonly object EventRowUpdated = new object(); 
		static private readonly object EventRowUpdating = new object();
		
		/// <summary>
		/// The default constructor
		/// </summary>
		public CMDataAdapter():base()
		{
		}
		
		/// <summary> Gets the parameters set by the user when executing an SQL SELECT statement.
		/// 
		/// </summary>
		/// <returns> An array of IDataParameter objects that contains the parameters set by the user. 
		/// 
		/// </returns>
		public IDataParameterCollection FillParameters
		{
			get
			{
				if (_selectCommand != null)
				{
					return _selectCommand.Parameters;
				}
				else
				{
					return null;
				}
			}
			
		}
		/// <summary>
		/// Gets a Command for deleting records from the data set.
		/// </summary>
		/// <value> A CMCommand used during Update to delete records in the data source
		/// for deleted rows in the data set.
		/// </value>
		public new CMCommand DeleteCommand
		{
			get
			{
				return _deleteCommand;
			}
			set
			{
				_deleteCommand = value;
			}
		}

		/// <summary>
		/// Gets an command for deleting records from the data set.
		/// </summary>
		/// <value> A IDbCommand used during Update to delete records in the data source
		/// for deleted rows in the data set.
		/// </value>
		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return _deleteCommand;
			}
			set
			{
				_deleteCommand = (CMCommand) value;
			}
		}

		/// <summary>
		/// Gets a command used to insert records into the data set.
		/// </summary>
		/// <value> A CMCommand used during Update to insert records in the
		/// data source for new rows in the data set.
		/// </value>
		public new CMCommand InsertCommand
		{
			get
			{
				return _insertCommand;
			}
			set
			{
				_insertCommand = value;
			}
		}

		/// <summary>
		/// Gets a command used to insert records into the data set.
		/// </summary>
		/// <value> A IDbCommand used during Update to insert records in the
		/// data source for new rows in the data set.
		/// </value>
		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return _insertCommand;
			}
			set
			{
				_insertCommand = (CMCommand) value;
			}
		}

		/// <summary>
		/// Gets a command used to select records in the data source.
		/// </summary>
		/// <value> An CMCommand that is used to select records from data source
		/// for placement in the data set.
		/// </value>
		public new CMCommand SelectCommand
		{
			get
			{
				return _selectCommand;
			}
			set
			{
				_selectCommand = value;
			}
		}

		/// <summary>
		/// Gets a command used to select records in the data source.
		/// </summary>
		/// <value> An IDbCommand that is used to select records from data source
		/// for placement in the data set.
		/// </value>
		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return _selectCommand;
			}
			set
			{
				_selectCommand = (CMCommand) value;
			}
		}

		/// <summary>
		/// Gets a command used to update records in the data source.
		/// </summary>
		/// <value> An CMCommand used during Update to update records in the data
		/// source for modified rows in the data set.
		/// </value>
		public new CMCommand UpdateCommand
		{
			get
			{
				return _updateCommand;
			}
			set
			{
				_updateCommand = value;
			}
		}

		/// <summary>
		/// Gets a command used to update records in the data source.
		/// </summary>
		/// <value> An IDbCommand used during Update to update records in the data
		/// source for modified rows in the data set.
		/// </value>
		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return _updateCommand;
			}
			set
			{
				_updateCommand = (CMCommand) value;
			}
		}
	
		/*
		 * Implement abstract methods inherited from DbDataAdapter.
		 */
		/// <summary>
		/// Create a row updated event
		/// </summary>
		/// <param name="dataRow"></param>
		/// <param name="command"></param>
		/// <param name="statementType"></param>
		/// <param name="tableMapping"></param>
		/// <returns></returns>
		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new CMRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		/// <summary>
		/// Create a row updating event
		/// </summary>
		/// <param name="dataRow"></param>
		/// <param name="command"></param>
		/// <param name="statementType"></param>
		/// <param name="tableMapping"></param>
		/// <returns></returns>
		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new CMRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		/// <summary>
		/// called opon OnRowUpdating event
		/// </summary>
		/// <param name="value"></param>
		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			CMRowUpdatingEventHandler handler = (CMRowUpdatingEventHandler) Events[EventRowUpdating];
			if ((null != handler) && (value is CMRowUpdatingEventArgs)) 
			{
				handler(this, (CMRowUpdatingEventArgs) value);
			}
		}

		/// <summary>
		/// called upon OnRowUpdated event
		/// </summary>
		/// <param name="value"></param>
		override protected void OnRowUpdated(RowUpdatedEventArgs value)
		{
			CMRowUpdatedEventHandler handler = (CMRowUpdatedEventHandler) Events[EventRowUpdated];
			if ((null != handler) && (value is CMRowUpdatedEventArgs)) 
			{
				handler(this, (CMRowUpdatedEventArgs) value);
			}
		}

		/// <summary>
		/// RowUpdating handler
		/// </summary>
		public event CMRowUpdatingEventHandler RowUpdating
		{
			add { Events.AddHandler(EventRowUpdating, value); }
			remove { Events.RemoveHandler(EventRowUpdating, value); }
		}

		/// <summary>
		/// RowUpdated handler
		/// </summary>
		public event CMRowUpdatedEventHandler RowUpdated
		{
			add { Events.AddHandler(EventRowUpdated, value); }
			remove { Events.RemoveHandler(EventRowUpdated, value); }
		}
	}

	/// <summary>
	/// Definition of delegate for row updating event handler
	/// </summary>
	public delegate void CMRowUpdatingEventHandler(object sender, CMRowUpdatingEventArgs e);
	
	/// <summary>
	/// Definition of delegate for row update event handler
	/// </summary>
	public delegate void CMRowUpdatedEventHandler(object sender, CMRowUpdatedEventArgs e);

	/// <summary>
	/// Rowupdating Event args
	/// </summary>
	public class CMRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		/// <summary>
		/// Instantiate a CMRowUpdatingEventArgs instance
		/// </summary>
		/// <param name="row">The row being updated</param>
		/// <param name="command">The update command</param>
		/// <param name="statementType">One of StatementType enum values</param>
		/// <param name="tableMapping">One of the DataTableMapping enum values</param>
		public CMRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) 
			: base(row, command, statementType, tableMapping) 
		{
		}

		/// <summary>
		/// Get the CMCommand instance
		/// </summary>
		/// <remarks>Hide the inherited implementation of the command property.</remarks>
		new public CMCommand Command
		{
			get  { return (CMCommand) base.Command; }
			set  { base.Command = value; }
		}
	}

	/// <summary>
	/// Rowupdated Event args
	/// </summary>
	public class CMRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		/// <summary>
		/// Instantiate a CMRowUpdatedEventArgs instance
		/// </summary>
		/// <param name="row">The row being updated</param>
		/// <param name="command">The update command</param>
		/// <param name="statementType">One of StatementType enum values</param>
		/// <param name="tableMapping">One of the DataTableMapping enum values</param>
		public CMRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping) 
		{
		}

		/// <summary>
		/// Get the CMCommand instance
		/// </summary>
		/// <remarks>Hide the inherited implementation of the command property.</remarks>
		new public CMCommand Command
		{
			get  { return (CMCommand) base.Command; }
		}
	}
}