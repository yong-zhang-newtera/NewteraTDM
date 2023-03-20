/*
* @(#)CompletedTaskSaver.cs
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Xml;
    using System.Data;
    using System.IO;
	using System.Collections;
    using System.Threading;
    using System.Collections.Specialized;
    using System.Workflow.Runtime;
    using System.Security.Principal;
    using System.Runtime.Remoting.Messaging;

	using Newtera.Common.Core;
    using Newtera.WFModel;
    using Newtera.WorkflowServices;

	/// <summary> 
	/// Run the process of saving completed tasks asynchronously
	/// </summary>
	/// <version>  	1.0.0 27 May 2017</version>
	public class CompletedTaskSaver
	{
        private TaskInfo _taskInfo;
        private StringCollection _taskOwners;

		/// <summary>
        /// Initiating an CompletedTaskSaver instance
		/// </summary>
        /// <param name="taskInfo">The completed task info</param>
        /// <param name="taskOwners">The task owners</param>
        public CompletedTaskSaver(TaskInfo taskInfo, StringCollection taskOwners)
		{
            _taskInfo = taskInfo;
            _taskOwners = taskOwners;
		}

        /// <summary>
        /// run the task saving async
        /// </summary>
        public void Run()
        {
            // run the event raiser using a worker thread from the thread pool
            RunSavingAsyncDelegate runEventDelegate = new RunSavingAsyncDelegate(RunSavingAsync);

            AsyncCallback callback = new AsyncCallback(RunSavingCallback);

            runEventDelegate.BeginInvoke(callback, null);
        }

        private delegate void RunSavingAsyncDelegate();

        /// <summary>
        /// run the event raiser
        /// </summary>
        private void RunSavingAsync()
        {
            NewteraTaskService taskService = new NewteraTaskService();

            foreach (string userName in _taskOwners)
            {
                taskService.AddUserFinishedTask(_taskInfo, userName);  
            }
        }

        private void RunSavingCallback(IAsyncResult ar)
        {
            // Retrieve the delegate.
            AsyncResult result = (AsyncResult)ar;
            RunSavingAsyncDelegate caller = (RunSavingAsyncDelegate)result.AsyncDelegate;

            // Call EndInvoke to avoid memory leak
            caller.EndInvoke(ar);
        }
	}
}