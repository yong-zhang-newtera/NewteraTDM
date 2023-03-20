/*
* @(#)AddProjectAction.cs
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
	/// Add a new project to the database. It is responsible to create actions that
	/// perform individual task, such as add workflow, etc.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class AddProjectAction : WorkflowModelActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddProjectAction
		/// </summary>
		/// <param name="projectModel">The meta data model of the action</param>
		public AddProjectAction(ProjectModel projectModel,
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
				return WorkflowModelActionType.AddProject;
			}
		}

		/// <summary>
		/// Prepare the action for adding a project to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
            // assign an unique id to the project
            KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ProjectId, null);
            Element.ID = generator.NextKey().ToString();
            ProjectModel projectModel = (ProjectModel)Element;

            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = trans;

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddProjectDML");
            DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
            sql = dmlGenerator.GetAddProjectDML(sql, projectModel.ID, projectModel.Name, projectModel.Version, DateTime.Now.ToString("s"));

            cmd.CommandText = sql;

            if (_log != null)
            {
                _log.Append(sql, LogType.DML);
            }

            cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of creating an entry for a project.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}