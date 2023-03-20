/*
* @(#)DeleteWorkflowAction.cs
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
    using Newtera.Server.Engine.Workflow;

	/// <summary>
	/// Delete a workflow model from the database.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class DeleteWorkflowAction : WorkflowModelActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteWorkflowAction
		/// </summary>
		/// <param name="projectModel">The project model of the action</param>
		public DeleteWorkflowAction(ProjectModel projectModel,
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
				return WorkflowModelActionType.DeleteWorkflow;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a workflow model from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelWorkflowDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetDelWorkflowDML(sql, Element.ID);
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of deleting a class from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
            // delete workflow related data saved in the database
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            
            int pageIndex = 0;
            int pageSize = 100;
            while (true)
            {
                NewteraTrackingWorkflowInstanceCollection trackingInstances = adapter.GetTrackingWorkflowInstances(Element.ID, pageIndex, pageSize);

                if (trackingInstances == null || trackingInstances.Count == 0)
                {
                    break;
                }

                // delete workflow instance related data
                foreach (NewteraTrackingWorkflowInstance wokflowTrackingInstance in trackingInstances)
                {
                    string workflowInstanceId = wokflowTrackingInstance.WorkflowInstanceId.ToString();
                    try
                    {
                        adapter.DeleteActivityTrackingRecords(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteWorkflowTrackingRecords(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteSubscriptionByWFInstanceId(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteWorkflowEventSubscriptionByWFId(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteWorkflowInstanceBindings(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteInstanceState(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        adapter.DeleteReassignedTaskInfosByWFInstanceId(workflowInstanceId);
                    }
                    catch
                    {
                    }

                    try
                    {
                        NewteraTaskService taskService = new NewteraTaskService();
                        taskService.DeleteTasks(workflowInstanceId);
                    }
                    catch
                    {
                    }
                }

                pageIndex++;
            }
		}
	}
}