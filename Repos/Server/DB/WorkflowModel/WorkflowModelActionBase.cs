/*
* @(#)WorkflowModelActionBase.cs
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
	/// Represents a base class for all classes that implements IWorkflowModelAction interface.
	/// It provides common functionality
	/// </summary>
	/// <version> 1.0.0 15 Dec 2005 </version>
	public abstract class WorkflowModelActionBase : IWorkflowModelAction
	{
		private ProjectModel _projectModel;
		private IWFModelElement _element;
		protected IDataProvider _dataProvider;
		protected ProjectModelUpdateLog _log;

		/// <summary>
		/// Instantiate an instance of WorkflowModelActionBase
		/// </summary>
		/// <param name="projectModel">The meta data model of the action</param>
		public WorkflowModelActionBase(ProjectModel projectModel, IWFModelElement element,
			IDataProvider dataProvider)
		{
			_projectModel = projectModel;
			_element = element;
			_dataProvider = dataProvider;
			_log = null;
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of WorkflowModelActionType values</value>
		public abstract WorkflowModelActionType ActionType
		{
			get;
		}

		/// <summary>
		/// Gets or sets the ProjectModel instance representing the meta data
		/// that the action is applying for.
		/// </summary>
		/// <value>A ProjectModel instance</value>
		public ProjectModel ProjectModel
		{
			get
			{
				return _projectModel;
			}
		}

		/// <summary>
		/// Gets the Schema Model Element that is involved when
		/// performing the action.
		/// </summary>
		/// <value>A IWFModelElement</value>
		public IWFModelElement Element
		{
			get
			{
				return _element;
			}
		}

		/// <summary>
		/// Gets or sets the log to record DML, DDL, or error messages
		/// </summary>
		/// <value>A ProjectModelUpdateLog</value>
		public ProjectModelUpdateLog Log
		{
			get
			{
				return _log;
			}
			set
			{
				_log = value;
			}
		}

		/// <summary>
		/// Prepare the action. This give the action a chance to prepare the action
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public abstract void Prepare(IDbConnection con, IDbTransaction trans);

		/// <summary>
		/// Perform the action This is where DDL(s) is executed against the database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public abstract void Do(IDbConnection con);

		/// <summary>
		/// Undo the action in case an error occures. It should is overrided by subclasses
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public virtual void UnDo(IDbConnection con)
		{
		}

		/// <summary>
		/// Commit the action. This is called after all actions are performed without errors.
		/// It should be overried by subclasses.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public virtual void Commit(IDbConnection con)
		{
		}
	}
}