/*
* @(#)WorkflowInstanceBindingInfo.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;

	/// <summary>
	/// A class contains information about binding between a workflow instance and a data instance.
	/// </summary>
	/// <version>1.0.0 12 Jan 2007 </version>
    [Serializable]
	public class WorkflowInstanceBindingInfo
	{
        private string _workflowInstanceId;
        private string _workflowTypeId;
        private string _dataInstanceId;
        private string _dataClassName;
        private string _schemaId;
        private string _projectName;
        private string _projectVersion;
        private string _workflowName;

        public WorkflowInstanceBindingInfo()
        {
            _workflowInstanceId = null;
            _workflowTypeId = null;
            _dataInstanceId = null;
            _dataClassName = null;
            _schemaId = null;
            _projectName = null;
            _projectVersion = null;
            _workflowName = null;
        }

        /// <summary>
        /// Initiating an instance of NewteraTrackingWorkflowInstance class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public WorkflowInstanceBindingInfo(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
        }

        /// <summary>
        /// Gets or sets the id of the workflow instance
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
        /// Gets or sets id of workflow type to which the workflow instance belongs
        /// </summary>
        public string WorkflowTypeId
        {
            get
            {
                return _workflowTypeId;
            }
            set
            {
                _workflowTypeId = value;
            }
        }

        /// <summary>
        /// Gets or sets id of the data instance that binds to the workflow instance
        /// </summary>
        public string DataInstanceId
        {
            get
            {
                return _dataInstanceId;
            }
            set
            {
                _dataInstanceId = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the Data Class that the data instance is part of
        /// </summary>
        public string DataClassName
        {
            get
            {
                return _dataClassName;
            }
            set
            {
                _dataClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets id of the schema that the data instance is part of
        /// </summary>
        public string SchemaId
        {
            get
            {
                return _schemaId;
            }
            set
            {
                _schemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the project that owns the workflow defiition
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
        /// Gets or sets version of the project that owns the workflow defiition
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
        /// Gets or sets the name of the workflow defiition
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
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            string str = parent.GetAttribute("wfInstanceId");
            _workflowInstanceId = str;

            str = parent.GetAttribute("wfTypeId");
            _workflowTypeId = str;

            str = parent.GetAttribute("dataInstanceId");
            _dataInstanceId = str;

            str = parent.GetAttribute("dataClassName");
            _dataClassName = str;

            str = parent.GetAttribute("schemaId");
            _schemaId = str;

            str = parent.GetAttribute("projectName");
            _projectName = str;

            str = parent.GetAttribute("projectVersion");
            _projectVersion = str;

            str = parent.GetAttribute("wfName");
            _workflowName = str;
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public void Marshal(XmlElement parent)
        {
            parent.SetAttribute("wfInstanceId", _workflowInstanceId);

            parent.SetAttribute("wfTypeId", _workflowTypeId);

            parent.SetAttribute("dataInstanceId", _dataInstanceId);

            parent.SetAttribute("dataClassName", _dataClassName);

            parent.SetAttribute("schemaId", _schemaId);

            if (!string.IsNullOrEmpty(_projectName))
            {
                parent.SetAttribute("projectName", _projectName);
            }

            if (!string.IsNullOrEmpty(_projectVersion))
            {
                parent.SetAttribute("projectVersion", _projectVersion);
            }

            if (!string.IsNullOrEmpty(_workflowName))
            {
                parent.SetAttribute("wfName", _workflowName);
            }
        }
	}
}