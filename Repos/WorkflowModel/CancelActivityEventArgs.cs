/*
* @(#)CancelActivityEventArgs.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary> 
	/// Representing an event argument that cancels execution of an activity.
	/// </summary>
	/// <version>  	1.0.0 17 Nov 2008</version>
    [Serializable]
    public class CancelActivityEventArgs : EventArgs
    {
        private string _workflowInstanceId;
        private string _activityName;

        public CancelActivityEventArgs() : base()
        {
            _workflowInstanceId = null;
            _activityName = null;
        }

        public CancelActivityEventArgs(string workflowInstanceId, string activityName)           : base()
        {
            _workflowInstanceId = workflowInstanceId;
            _activityName = activityName;
        }

        /// <summary>
        /// Gets the id of the workflow instance
        /// </summary>
        public string WorkflowInstanceId
        {
            get
            {
                return _workflowInstanceId; 
            }
        }

        /// <summary>
        /// Gets the name of the activity.
        /// </summary>
        public string ActivityName
        {
            get
            {
                return _activityName;
            }
        }
    }
}