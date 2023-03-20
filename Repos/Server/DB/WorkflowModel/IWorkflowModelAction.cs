/*
* @(#)IWorkflowModelAction.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;
	using System.Data;

	using Newtera.WFModel;
	using Newtera.Server.DB;

	/// <summary>
	/// Represents an interface for actions applied to the project model stored in database
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public interface IWorkflowModelAction
	{
		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of WorkflowModelActionType values</value>
		WorkflowModelActionType ActionType
		{
			get;
		}

		/// <summary>
		/// Gets the ProjectModel instance representing the project model
		/// that the action is applying for.
		/// </summary>
		/// <value>A ProjectModel instance</value>
		ProjectModel ProjectModel
		{
			get;
		}

		/// <summary>
		/// Gets or sets the Workflow Model Element that is involved when
		/// performing the action.
		/// </summary>
		/// <value>A WorkflowModelElement</value>
		IWFModelElement Element
		{
			get;
		}

		/// <summary>
		/// Gets or sets the log to record results of executing DDL, or error messages
		/// </summary>
		/// <value>A ProjectModelUpdateLog</value>
		ProjectModelUpdateLog Log
		{
			get;
			set;
		}

		/// <summary>
		/// Prepare the action. This give the action a chance to prepare the action
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		void Prepare(IDbConnection con, IDbTransaction trans);

		/// <summary>
		/// Perform the action. This is where DDL(s) is executed against the database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void Do(IDbConnection con);

		/// <summary>
		/// Undo the action in case an error occures
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void UnDo(IDbConnection con);

		/// <summary>
		/// Commit the action. This is called after all actions are executed without errors
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void Commit(IDbConnection con);
	}

	/// <summary>
	/// Specify the types of available actions that can be applied to project model
	/// </summary>
	public enum WorkflowModelActionType
	{
		Unknown,
		AddProject,
		DeleteProject,
		AddWorkflow,
		DeleteWorkflow,
	}
}