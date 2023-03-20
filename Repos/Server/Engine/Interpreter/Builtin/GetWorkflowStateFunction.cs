/*
* @(#)GetWorkflowStateFunction.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.DB;
    using Newtera.WFModel;

	/// 
	/// <summary>
	/// Gets name of the current state of a state machine workflow that has been bound to
    /// a data instance, empty string if such a state machine workflow does not exist.
	/// </summary>
	/// <version>  1.0.0 19 Oct 2007</version>
	public class GetWorkflowStateFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public GetWorkflowStateFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 1)
			{
				throw new InterpreterException("GetWorkflowStateFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            string currentStateName = "";

			string objId = ((IExpr) _arguments[0]).Eval().ToString();
            if (!string.IsNullOrEmpty(objId))
            {
                // get the binding workflow instance info if there exists one
                WorkflowModelAdapter adapter = new WorkflowModelAdapter();
                WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(objId);

                if (bindingInfo != null)
                {
                    // Get binding workflow tracking state infos
                    NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = adapter.GetTrackingWorkflowInstancesByWorkflowInstanceId(bindingInfo.WorkflowInstanceId);
                    Guid workflowInstanceId = new Guid(bindingInfo.WorkflowInstanceId);
                    foreach (NewteraTrackingWorkflowInstance trackingWorkflowInstance in trackingWorkflowInstances)
                    {
                        foreach (NewteraActivityTrackingRecord activityRecord in trackingWorkflowInstance.ActivityEvents)
                        {
                            // make sure the activity is a StateActivity and is in Executing Status
                            if (activityRecord.ExecutionStatus == System.Workflow.ComponentModel.ActivityExecutionStatus.Executing &&
                                activityRecord.TypeName == "StateActivity")
                            {
                                currentStateName = activityRecord.QualifiedName;
                                break;
                            }
                        }
                    }
                }
            }

            return new XString(currentStateName);
		}
	}
}