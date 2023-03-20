/*
* @(#)DeleteProjectAction.cs
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
	/// Delete an existing project model from the database. It is responsible to create actions that
	/// perform individual task, such as delete workflow models, etc.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class DeleteProjectAction : WorkflowModelActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteProjectAction
		/// </summary>
		/// <param name="projectModel">The project model of the action</param>
		public DeleteProjectAction(ProjectModel projectModel,
			IWFModelElement element,
			IDataProvider dataProvider) : base(projectModel, element, dataProvider)
		{
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of WorkflowModelActionType values</value>
		public override WorkflowModelActionType ActionType
		{
			get
			{
				return WorkflowModelActionType.DeleteProject;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a project model from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelProjectDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetDelProjectDML(sql, Element.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of deleting a project model from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}