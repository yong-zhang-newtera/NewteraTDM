/*
* @(#)MetaDataUpdateExecutor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;
	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Execute the actions as result of comparing two meta data model. The actions
	/// are executed in an order that will ensure the successful updates to the 
	/// database. Therefore, the care must be taken when changing the order
	/// of executing actions.
	/// </summary>
	/// <version>  	1.0.0 16 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class MetaDataUpdateExecutor
	{
		private MetaDataCompareResult _result;
		private IDataProvider _dataProvider;
		private MetaDataActionCollection _actions;
		private MetaDataUpdateLog _log;
		private MetaDataAdapter _adapter;

		/// <summary>
		/// Initializes a new instance of the MetaDataUpdateExecutor class
		/// </summary>
		public MetaDataUpdateExecutor(MetaDataCompareResult result, IDataProvider dataProvider)
		{
			_result = result;
			_dataProvider = dataProvider;
			_actions = new MetaDataActionCollection();
			_log = new MetaDataUpdateLog();
			_adapter = new MetaDataAdapter(dataProvider);
		}

		/// <summary>
		/// Gets the update log
		/// </summary>
		public MetaDataUpdateLog UpdateLog
		{
			get
			{
				return _log;
			}
		}

		/// <summary>
		/// Execute actions in the compare result to perform updates to
		/// according to the meta data.
		/// </summary>
		public void Execute()
		{
			// Set the action execution orders
			SetActionOrders();

			// Phase one: prepare the actions
			PrepareActions();

            // Phase two: update the clobs with updated meta data in xml format
            WriteClobs();

			// Phase three: execute the actions
			ExecuteActions();

			// Phase four: save summary of execution to the database
			WriteSummary();

			if (_log.HasError)
			{
                ErrorLog.Instance.WriteLine(_log.Summary);

				throw new MetaDataUpdateException("Error occured during updating meta data, please check the update log");
			}
		}

		/// <summary>
		/// Fix the discrepancies between the meta data and its corresponding database
		/// </summary>
		public void FixDatabase()
		{
			// Set the action execution orders
			SetActionOrders();

			// set the log
			foreach (IMetaDataAction action in _actions)
			{
				action.Log = this._log;
			}

			// Phase one: execute the actions
			ExecuteActions();

			// save summary of execution to the database
			WriteSummary();

			if (_log.HasError)
			{
                ErrorLog.Instance.WriteLine(_log.Summary);
                throw new MetaDataUpdateException("Error occured during updating meta data, please check the update log");
			}
		}

		/// <summary>
		/// Set the action's executing order depending on whether it is adding,
		/// deleting, or altering a schema
		/// </summary>
		private void SetActionOrders()
		{
			if (_result.IsAddSchema)
			{
				SetTopDownOrder();
			}
			else if (_result.IsDeleteSchema)
			{
				SetBottomUpOrder();
			}
			else
			{
				// altering
				SetTopDownOrder();
			}
		}

		/// <summary>
		/// Prepare all action in the meta data compare result.
		/// </summary>
		private void PrepareActions()
		{
			// The first step is to prepare the actions. During the preparation,
			// some actions may update the data in MM Tables (System tables).
			// We use a transaction to commit or rollback the MM table updates
			// if anything goes wrong.
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
            IMetaDataAction currentAction = null;
            try
			{
				foreach (IMetaDataAction action in _actions)
				{
                    currentAction = action;
                    action.Log = _log;
					action.Prepare(con, tran);
				}

				// successfully prepare all action, commit the updates to MM Tables
				tran.Commit();
			}
			catch (Exception ex)
			{
				tran.Rollback(); // Rollback the updates to MM Tables

                string actionName = Enum.GetName(typeof(MetaDataActionType), currentAction.ActionType);
                _log.Append("Action " + actionName + " got error: " + ex.Message, LogType.Error);

                //throw new MetaDataUpdateException(e.Message, e);
            }
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// If the compare result is adding or altering a schema,
		/// write the meta data in xml formated string to the clobs in database.
		/// </summary>
		private void WriteClobs()
		{
			if (!_result.IsDeleteSchema)
			{
				_adapter.WriteMetaData(_result.NewMetaDataModel);
			}
		}

		/// <summary>
		/// Execute all actions in the meta data compare result.
		/// </summary>
		private void ExecuteActions()
		{
			// The second step is to execute the actions. During this phase,
			// some of actions may execute DDLs against database. Since transaction
			// is not able to rollback the DDLs, actions will have to implement
			// Undo and Commit methods in the future, currently they don't do nothing
			IDbConnection con = _dataProvider.Connection;

			try
			{
				foreach (IMetaDataAction action in _actions)
				{
					try
					{
						action.Do(con);
					}
					catch (Exception ex)
					{
                        // Currently ignore the ddl execution exceptions
                        // TODO, implement undo functions
                        string actionName = Enum.GetName(typeof(MetaDataActionType), action.ActionType);
						_log.Append("Action " + actionName + " got error: " + ex.Message, LogType.Error);
					}
				}
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Write the summary of update execution to the database.
		/// </summary>
		private void WriteSummary()
		{
			if (_result.NewMetaDataModel != null)
			{
				_adapter.WriteLog(_result.NewMetaDataModel.SchemaInfo, _log.Summary);
			}
		}

		/// <summary>
		/// Set the action executing order from top down
		/// </summary>
		private void SetTopDownOrder()
		{
			// NOTE: DO NOT CHANGE THE ORDER OF COMPILING THE ACTIONS. OTHERWISE,
			// EXECUTION OF THE ACTION MAY FAIL.
			if (_result.AddSchema != null)
			{
				_actions.Add(_result.AddSchema);
			}

			foreach (IMetaDataAction action in _result.AddClasses)
			{
				_actions.Add(action);
			}
			
			foreach (IMetaDataAction action in _result.AddSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AlterSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AddArrayAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AlterArrayAttributes)
			{
				_actions.Add(action);
			}

            foreach (IMetaDataAction action in _result.AddImageAttributes)
            {
                _actions.Add(action);
            }

			foreach (IMetaDataAction action in _result.AddRelationshipAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AlterRelationshipAttributes)
			{
				_actions.Add(action);
			}

            foreach (IMetaDataAction action in _result.AlterClasses)
            {
                _actions.Add(action);
            }

			foreach (IMetaDataAction action in _result.DeleteSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteArrayAttributes)
			{
				_actions.Add(action);
			}

            foreach (IMetaDataAction action in _result.DeleteImageAttributes)
            {
                _actions.Add(action);
            }

			foreach (IMetaDataAction action in _result.DeleteRelationshipAttributes)
			{
                _actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteClasses)
			{
				_actions.Add(action);
			}

			if (_result.DeleteSchema != null)
			{
				_actions.Add(_result.DeleteSchema);
			}
		}

		/// <summary>
		/// Set the action executing order from bottom up
		/// </summary>
		private void SetBottomUpOrder()
		{
			// NOTE: DO NOT CHANGE THE ORDER OF COMPILING THE ACTIONS. OTHERWISE,
			// EXECUTION OF THE ACTION MAY FAIL.
			foreach (IMetaDataAction action in _result.AlterSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AddSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteSimpleAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AlterArrayAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AddArrayAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteArrayAttributes)
			{
				_actions.Add(action);
			}

            foreach (IMetaDataAction action in _result.AddImageAttributes)
            {
                _actions.Add(action);
            }

            foreach (IMetaDataAction action in _result.DeleteImageAttributes)
            {
                _actions.Add(action);
            }

			foreach (IMetaDataAction action in _result.AlterRelationshipAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AddRelationshipAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteRelationshipAttributes)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AlterClasses)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.AddClasses)
			{
				_actions.Add(action);
			}

			foreach (IMetaDataAction action in _result.DeleteClasses)
			{
				_actions.Add(action);
			}

			if (_result.AddSchema != null)
			{
				_actions.Add(_result.AddSchema);
			}

			if (_result.DeleteSchema != null)
			{
				_actions.Add(_result.DeleteSchema);
			}
		}
	}
}