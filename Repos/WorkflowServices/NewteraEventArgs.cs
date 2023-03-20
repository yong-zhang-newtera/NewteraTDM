/*
* @(#)NewteraEventArgs.cs
*
* Copyright (c) 2006-2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowServices
{
	using System;

	/// <summary> 
	/// Representing a special event argument.
	/// </summary>
	/// <version>  	1.0.0 27 Dec 2006</version>
    [Serializable]
    public class NewteraEventArgs : EventArgs
    {
        private string _schemaId;
        private string _className;
        private string _eventName;
        private string _objId;
        private string _wfInstanceId;
        private string _taskId;
        private string _userId;

        public NewteraEventArgs() : base()
        {
            _schemaId = null;
            _className = null;
            _eventName = null;
            _objId = null;
            _wfInstanceId = null;
            _taskId = null;
            _userId = null;
        }

        public NewteraEventArgs(string schemaId, string className, string eventName, string objId)
            : base()
        {
            _schemaId = schemaId;
            _className = className;
            _eventName = eventName;
            _objId = objId;
            _wfInstanceId = null;
            _taskId = null;
        }

        public NewteraEventArgs(string schemaId, string className, string eventName, string objId, string taskId)
            : base()
        {
            _schemaId = schemaId;
            _className = className;
            _eventName = eventName;
            _objId = objId;
            _wfInstanceId = null;
            _taskId = taskId;
        }

        public NewteraEventArgs(string schemaId, string className, string eventName, string objId, string taskId, string userId)
       : base()
        {
            _schemaId = schemaId;
            _className = className;
            _eventName = eventName;
            _objId = objId;
            _wfInstanceId = null;
            _taskId = taskId;
            _userId = userId;
        }

        /// <summary>
        /// Gets the id of schema which consists of a name and version
        /// </summary>
        public string SchemaId
        {
            get
            {
                return _schemaId; 
            }
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string EventName
        {
            get
            {
                return _eventName;
            }
        }

        /// <summary>
        /// Gets the id of the data instance that causes the event.
        /// </summary>
        public string ObjId
        {
            get
            {
                return _objId;
            }
        }

        /// <summary>
        /// Gets the id of the workflow instance that raises the event.
        /// </summary>
        public string WorkflowInstanceId
        {
            get
            {
                return _wfInstanceId;
            }
            set
            {
                _wfInstanceId = value;
            }
        }

        /// <summary>
        /// Gets the id of a workflow task that raises the event.
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
        /// Get or set the id of the user whose action generates the event
        /// </summary>
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
    }
}