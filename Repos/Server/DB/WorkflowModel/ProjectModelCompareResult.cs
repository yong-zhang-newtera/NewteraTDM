/*
* @(#)ProjectModelCompareResult.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;

    using Newtera.WFModel;

	/// <summary>
	/// Keeps the result of comparing two different ProjectModel.
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	public class ProjectModelCompareResult
	{
		private ProjectModel _newProjectModel;
		private ProjectModel _oldProjectModel;
		private IWorkflowModelAction _addProject = null;
		private IWorkflowModelAction _deleteProject = null;
		private WorkflowModelActionCollection _addWorkflows = null;
		private WorkflowModelActionCollection _deleteWorkflows = null;
		private WorkflowModelActionCollection _alterWorflows = null;

		/// <summary>
		/// Initializes a new instance of the ProjectModelCompareResult class
		/// </summary>
		public ProjectModelCompareResult(ProjectModel newProjectModel,
			ProjectModel oldProjectModel)
		{
			_newProjectModel = newProjectModel;
			_oldProjectModel = oldProjectModel;
			_addWorkflows = new WorkflowModelActionCollection();
			_deleteWorkflows = new WorkflowModelActionCollection();
			_alterWorflows = new WorkflowModelActionCollection();
		}

		/// <summary>
		/// Gets the new project model in the comparison
		/// </summary>
		public ProjectModel NewProjectModel
		{
			get
			{
				return _newProjectModel;
			}
		}

		/// <summary>
		/// Gets the old project model in the comparison
		/// </summary>
		public ProjectModel OldProjectModel
		{
			get
			{
				return _oldProjectModel;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the result is for adding a project model
		/// </summary>
		/// <value>true if it is adding a project model, false otherwise</value>
		public bool IsAddProject
		{
			get
			{
				if (_addProject != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the result is for deleting a project model
		/// </summary>
		/// <value>true if it is deleting a project model, false otherwise</value>
		public bool IsDeleteProject
		{
			get
			{
				if (_deleteProject != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the action that addes a new project model
		/// </summary>
		public IWorkflowModelAction AddProject
		{
			get
			{
				return _addProject;
			}
			set
			{
				_addProject = value;
			}
		}

		/// <summary>
		/// Gets or sets the action that deletes an existing project model
		/// </summary>
		public IWorkflowModelAction DeleteProject
		{
			get
			{
				return _deleteProject;
			}
			set
			{
				_deleteProject = value;
			}
		}

		/// <summary>
		/// Gets actions that add worflows to the project model in database
		/// </summary>
		public WorkflowModelActionCollection AddWorkflows
		{
			get
			{
				return _addWorkflows;
			}
		}

		/// <summary>
		/// Gets the actions that deletes worflows from the project model database
		/// </summary>
		public WorkflowModelActionCollection DeleteWorkflows
		{
			get
			{
				return _deleteWorkflows;
			}
		}

		/// <summary>
		/// Gets the actions that alters worflows in the project model database
		/// </summary>
		public WorkflowModelActionCollection AlterWorkflows
		{
			get
			{
				return _alterWorflows;
			}
		}

		/// <summary>
		/// Add an AddWorkflow action
		/// </summary>
		public void AddAddWorkflowAction(IWorkflowModelAction action)
		{
			_addWorkflows.Add(action);
		}

		/// <summary>
		/// Add an DeleteWorflow action
		/// </summary>
		public void AddDeleteWorkflowAction(IWorkflowModelAction action)
		{
			_deleteWorkflows.Add(action);
		}

		/// <summary>
		/// Add an AlterWorkflow action
		/// </summary>
		public void AddAlterWorkflowAction(IWorkflowModelAction action)
		{
			_alterWorflows.Add(action);
		}
	}
}