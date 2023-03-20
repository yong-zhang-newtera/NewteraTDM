/*
* @(#)AddWorkflowAction.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;
	using System.Text;
	using System.Data;

	using Newtera.WFModel;
	using Newtera.Server.DB;

	/// <summary>
	/// Add a new workflow model to the database.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class AddWorkflowAction : WorkflowModelActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddWorkflowAction
		/// </summary>
		/// <param name="projectModel">The meta data model of the action</param>
		/// <param name="element">The class element</param>
		/// <param name="dataProvider">The data provider</param>
		public AddWorkflowAction(ProjectModel projectModel,
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
				return WorkflowModelActionType.AddWorkflow;
			}
		}

		/// <summary>
		/// Prepare the action for adding a workflow model to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans)
		{
            WorkflowModel workflowModel = (WorkflowModel)Element;

            // assign an unique id to the workflow model
            KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.WorkflowId, null);
            workflowModel.ID = generator.NextKey().ToString();

            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = trans;

            // add a record in wf_workflow for an added workflow
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddWorkflowDML");
            DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
            sql = dmlGenerator.GetAddWorkflowDML(sql, workflowModel.ID,
                workflowModel.Name.ToUpper(),
                Enum.GetName(typeof(WorkflowType), workflowModel.WorkflowType),
                workflowModel.WorkflowClass,
                this.ProjectModel.ID);

            cmd.CommandText = sql;

            if (_log != null)
            {
                _log.Append(sql, LogType.DML);
            }

            cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of adding a workflow model to database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
		}
	}
}