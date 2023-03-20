/*
* @(#)TaskLogInfo.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// A class contains information about a task execution log
	/// </summary>
	/// <version>1.0.0 30 Dec 2014 </version>
	public class TaskLogInfo
	{
        private string _logId;
        private string _bindingInstanceKey;
        private string _bindingInstanceDesc;
        private string _workflowInstanceId;
        private string _taskName;
        private string _projectName;
        private string _projectVersion;
        private string _workflowName;
        private string _startTime;
        private string _finishTime;
        private string _expectedFinishTime;
        private string _taskTakers;
        private string _taskID;
        private string _year;
        private string _month;
        private string _duration;

        public TaskLogInfo()
        {
            _logId = null;
            _bindingInstanceKey = null;
            _bindingInstanceDesc = null;
            _workflowInstanceId = null;
            _taskName = null;
            _projectName = null;
            _projectVersion = null;
            _workflowName = null;
            _startTime = null;
            _finishTime = null;
            _expectedFinishTime = null;
            _taskTakers = null;
            _taskID = null;
            _year = null;
            _month = null;
            _duration = null;
        }

        /// <summary>
        /// Gets or sets the log id
        /// </summary>
        public string LogID
        {
            get
            {
                return _logId;
            }
            set
            {
                _logId = value;
            }
        }

        /// <summary>
        /// Gets or sets the binding instance key
        /// </summary>
        public string BindingInstanceKey
        {
            get
            {
                return _bindingInstanceKey;
            }
            set
            {
                _bindingInstanceKey = value;
            }
        }

        /// <summary>
        /// Gets or sets description of the log
        /// </summary>
        public string BindingInstanceDesc
        {
            get
            {
                return _bindingInstanceDesc;
            }
            set
            {
                _bindingInstanceDesc = value;
            }
        }

        /// <summary>
        /// Gets or sets WorkflowInstanceId
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
        /// Gets or sets TaskName
        /// </summary>
        public string TaskName
        {
            get
            {
                return _taskName;
            }
            set
            {
                _taskName = value;
            }
        }

        /// <summary>
        /// Gets or sets ProjectName
        /// </summary>
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }

        /// <summary>
        /// Gets or sets ProjectVersion
        /// </summary>
        public string ProjectVersion
        {
            get
            {
                return _projectVersion;
            }
            set
            {
                _projectVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets WorkflowName
        /// </summary>
        public string WorkflowName
        {
            get
            {
                return _workflowName;
            }
            set
            {
                _workflowName = value;
            }
        }

        /// <summary>
        /// Gets or sets StartTime
        /// </summary>
        public string StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }

        /// <summary>
        /// Gets or sets FinishTime
        /// </summary>
        public string FinishTime
        {
            get
            {
                return _finishTime;
            }
            set
            {
                _finishTime = value;
            }
        }

        /// <summary>
        /// Gets or sets ExpectedFinishTime
        /// </summary>
        public string ExpectedFinishTime
        {
            get
            {
                return _expectedFinishTime;
            }
            set
            {
                _expectedFinishTime = value;
            }
        }

        /// <summary>
        /// Gets or sets TaskTakers
        /// </summary>
        public string TaskTakers
        {
            get
            {
                return _taskTakers;
            }
            set
            {
                _taskTakers = value;
            }
        }

        /// <summary>
        /// Gets or sets TaskID
        /// </summary>
        public string TaskID
        {
            get
            {
                return _taskID;
            }
            set
            {
                _taskID = value;
            }
        }

        /// <summary>
        /// Gets or sets Year
        /// </summary>
        public string Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
            }
        }

        /// <summary>
        /// Gets or sets Month
        /// </summary>
        public string Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
            }
        }

        /// <summary>
        /// Gets or sets Duration
        /// </summary>
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }
	}
}