/*
* @(#)ProjectModelUpdateExecutor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;
	using System.Data;

	using Newtera.Common.Core;
	using Newtera.WFModel;
	using Newtera.Server.DB;

	/// <summary>
	/// Execute the actions as result of comparing two project models. The actions
	/// are executed in an order that will ensure the successful updates to the 
	/// database. Therefore, the care must be taken when changing the order
	/// of executing actions.
	/// </summary>
	/// <version>1.0.0 15 Dec 2006 </version>
	public class ProjectModelUpdateExecutor
	{
		private ProjectModelCompareResult _result;
		private IDataProvider _dataProvider;
		private WorkflowModelActionCollection _actions;
		private ProjectModelUpdateLog _log;
		private WorkflowModelAdapter _adapter;

		/// <summary>
		/// Initializes a new instance of the ProjectModelUpdateExecutor class
		/// </summary>
		public ProjectModelUpdateExecutor(ProjectModelCompareResult result, IDataProvider dataProvider)
		{
			_result = result;
			_dataProvider = dataProvider;
			_actions = new WorkflowModelActionCollection();
			_log = new ProjectModelUpdateLog();
			_adapter = new WorkflowModelAdapter(dataProvider);
		}

		/// <summary>
		/// Gets the update log
		/// </summary>
		public ProjectModelUpdateLog UpdateLog
		{
			get
			{
				return _log;
			}
		}

		/// <summary>
		/// Execute actions in the compare result to perform updates to
		/// according to the project model.
		/// </summary>
		public void Execute()
		{
			// Set the action execution orders
			SetActionOrders();

			// Phase one: prepare the actions
			PrepareActions();

			// Phase two: update the clobs with updated project model in xml format
			WriteClobs();

			// Phase three: execute the actions
			ExecuteActions();

			// Phase foure: save summary of execution to the database
			WriteSummary();

			if (_log.HasError)
			{
                ErrorLog.Instance.WriteLine(_log.Summary);
                throw new ProjectModelUpdateException("Error occured during updating project model, please view the error log");
			}
		}

		/// <summary>
		/// Set the action's executing order depending on whether it is adding,
		/// deleting, or altering a schema
		/// </summary>
		private void SetActionOrders()
		{
			if (_result.IsAddProject)
			{
				SetTopDownOrder();
			}
			else if (_result.IsDeleteProject)
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
		/// Prepare all action in the project model compare result.
		/// </summary>
		private void PrepareActions()
		{
			// The first step is to prepare the actions. During the preparation,
			// some actions may update the data in MM Tables (System tables).
			// We use a transaction to commit or rollback the MM table updates
			// if anything goes wrong.
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			
			try
			{
				foreach (IWorkflowModelAction action in _actions)
				{
					action.Log = _log;
					action.Prepare(con, tran);
				}

				// successfully prepare all action, commit the updates to MM Tables
				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback(); // Rollback the updates to MM Tables
				
				_log.Append(e.Message, LogType.Error);
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// If the compare result is adding or altering a schema,
		/// write the project model in xml formated string to the clobs in database.
		/// </summary>
		private void WriteClobs()
		{
			if (!_result.IsDeleteProject)
			{
				_adapter.WriteProjectModel(_result.NewProjectModel);
			}
		}

		/// <summary>
		/// Execute all actions in the project model compare result.
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
				foreach (IWorkflowModelAction action in _actions)
				{
					try
					{
						action.Do(con);
					}
					catch (Exception ex)
					{
						// Currently ignore the ddl execution exceptions
						// TODO, implement undo functions
						_log.Append(ex.Message, LogType.Error);
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
			if (_result.NewProjectModel != null)
			{
				_adapter.WriteLog(_result.NewProjectModel.ID, _log.Summary);
			}
		}

		/// <summary>
		/// Set the action executing order from top down
		/// </summary>
		private void SetTopDownOrder()
		{
			// NOTE: DO NOT CHANGE THE ORDER OF COMPILING THE ACTIONS. OTHERWISE,
			// EXECUTION OF THE ACTION MAY FAIL.
			if (_result.AddProject != null)
			{
                _actions.Add(_result.AddProject);
			}

			foreach (IWorkflowModelAction action in _result.AddWorkflows)
			{
				_actions.Add(action);
			}

			foreach (IWorkflowModelAction action in _result.AlterWorkflows)
			{
				_actions.Add(action);
			}

			foreach (IWorkflowModelAction action in _result.DeleteWorkflows)
			{
				_actions.Add(action);
			}

			if (_result.DeleteProject != null)
			{
                _actions.Add(_result.DeleteProject);
			}
		}

		/// <summary>
		/// Set the action executing order from bottom up
		/// </summary>
		private void SetBottomUpOrder()
		{
			// NOTE: DO NOT CHANGE THE ORDER OF COMPILING THE ACTIONS. OTHERWISE,
			// EXECUTION OF THE ACTION MAY FAIL.

			foreach (IWorkflowModelAction action in _result.AlterWorkflows)
			{
				_actions.Add(action);
			}

			foreach (IWorkflowModelAction action in _result.AddWorkflows)
			{
				_actions.Add(action);
			}

			foreach (IWorkflowModelAction action in _result.DeleteWorkflows)
			{
				_actions.Add(action);
			}

			if (_result.AddProject != null)
			{
                _actions.Add(_result.AddProject);
			}

			if (_result.DeleteProject != null)
			{
                _actions.Add(_result.DeleteProject);
			}
		}
	}
}