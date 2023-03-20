/*
* @(#)ReassignedTaskInfo.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;
    using System.Security;

	/// <summary>
	/// A class contains information that a task that has been reassigned to a new user.
	/// </summary>
	/// <version>1.0.0 24 Apr 2011 </version>
	public class ReassignedTaskInfo
	{
        private string _taskId;
        private string _workflowInstanceId;
        private string _originalOwner;
        private string _currentOwner;

        public ReassignedTaskInfo()
        {
            _taskId = null;
            _workflowInstanceId = null;
            _originalOwner = null;
            _currentOwner = null;
        }


        /// <summary>
        /// Gets or sets the id of a task
        /// </summary>
        public string TaskId
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
            }
        }

        /// <summary>
        /// Gets or sets id of workflow instance
        /// </summary>
        public string WorkflowInstanceId
        {
            get
            {
                return _workflowInstanceId;
            }
            set
            {
                _workflowInstanceId = value;
            }
        }

        /// <summary>
        /// Gets or sets original owner of the task
        /// </summary>
        public string OriginalOwner
        {
            get
            {
                return _originalOwner;
            }
            set
            {
                _originalOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets current owner of the task
        /// </summary>
        public string CurrentOwner
        {
            get
            {
                return _currentOwner;
            }
            set
            {
                _currentOwner = value;
            }
        }
	}
}