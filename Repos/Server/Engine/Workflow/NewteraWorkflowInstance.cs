/*
* @(#)NewteraWorkflowInstance.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Workflow.Runtime;

using Newtera.Server.DB;

namespace Newtera.Server.Engine.Workflow
{
    [Serializable]
    public class NewteraWorkflowInstance
    {
        private Guid _workflowInstanceId;
        private WorkflowInstance _workflowInstance; //  run-time
        private string _projectName;
        private string _projectVersion;
        private string _workflowName;
        private string _workflowTypeId;
        private string _schemaId;
        private string _className;
        private string _objId;
        [NonSerialized]
        private Exception _exception; // run-time

        public NewteraWorkflowInstance()
        {
            this._workflowInstanceId = Guid.Empty;
            this._workflowInstance = null;
            this._projectName = string.Empty;
            this._projectVersion = string.Empty;
            this._workflowName = string.Empty;
            this._workflowTypeId = string.Empty;
            this._schemaId = string.Empty;
            this._className = string.Empty;
            this._objId = string.Empty;
            this._exception = null;
        }

        public NewteraWorkflowInstance(Guid workflowInstanceId, string projectName, string projectVersion,
                    string workflowName, string workflowTypeId)
        {
            this._workflowInstanceId = workflowInstanceId;
            this._workflowInstance = null;
            this._projectName = projectName;
            this._projectVersion = projectVersion;
            this._workflowName = workflowName;
            this._workflowTypeId = workflowTypeId;
            this._schemaId = string.Empty;
            this._className = string.Empty;
            this._objId = string.Empty;
            this._exception = null;
        }

        /// <summary>
        /// Gets the workflow instance
        /// </summary>
        public WorkflowInstance WorkflowInstance
        {
            get
            {
                return _workflowInstance;
            }
            set
            {
                _workflowInstance = value;
            }
        }

        /// <summary>
        /// Gets the workflow instance id
        /// </summary>
        public Guid WorkflowInstanceId
        {
            get
            {
                return _workflowInstanceId;
            }
        }

        /// <summary>
        /// gets or sets the name of project that owns the workflow
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        /// <summary>
        /// gets or sets the version of project that owns the workflow
        /// </summary>
        public string ProjectVersion
        {
            get { return _projectVersion; }
            set { _projectVersion = value; }
        }

        /// <summary>
        /// Gets or sets the workflow name of the workflow definition
        /// </summary>
        public string WorkflowName
        {
            get { return _workflowName; }
            set { _workflowName = value; }
        }

        /// <summary>
        /// Gets or sets the unique id of the workflow definition of the workflow instance
        /// </summary>
        public string WorkflowTypeId
        {
            get { return _workflowTypeId; }
            set { _workflowTypeId = value; }
        }

        /// <summary>
        /// Gets or sets the schema id of the data instance that is bound to the worklfow instance
        /// </summary>
        public string SchemaId
        {
            get { return _schemaId; }
            set { _schemaId = value; }
        }

        /// <summary>
        /// Gets or sets the class name of the data instance that is bound to the workflow instance.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        /// <summary>
        /// Gets or sets the id of the data instance that is bound to the workflow instance
        /// </summary>
        public string ObjId
        {
            get { return _objId; }
            set { _objId = value; }
        }

        /// <summary>
        /// Gets or sets the exception that happened to the workflow instance, used by the application
        /// at run-time
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        /// <summary>
        /// Save the changes to the properties to the database
        /// </summary>
        public void Save()
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            adapter.UpdateWorkflowInstanceBinding(this.ObjId, this.ClassName, this.SchemaId, this.WorkflowInstanceId.ToString());
        }
    }
}
