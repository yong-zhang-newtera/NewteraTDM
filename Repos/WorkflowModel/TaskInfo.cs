/*
* @(#)TaskInfo.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;
    using System.Security;

	/// <summary>
	/// A class contains information that a task generated by workflow instance.
	/// </summary>
	/// <version>1.0.0 28 Dec 2008 </version>
	public class TaskInfo
	{
        private string _taskId;
        private string _workflowInstanceId;
        private string _createTime;
        private string _finishTime;
        private string _subject;
        private string _description;
        private string _instruction;
        private string _bindingSchemaId;
        private string _bindingClassName;
        private string _bingdingObjId;
        private string _activityName;
        private string _users;
        private string _roles;
        private string _customActionsXml;
        private string _customFormUrl;
        private string _formProperties;
        private bool _isVisible;
        private string _userId;

        public TaskInfo()
        {
            _taskId = null;
            _workflowInstanceId = null;
            _createTime = null;
            _finishTime = null;
            _subject = null;
            _description = null;
            _instruction = null;
            _bindingSchemaId = null;
            _bindingClassName = null;
            _bingdingObjId = null;
            _activityName = null;
            _users = null;
            _roles = null;
            _customActionsXml = null;
            _customFormUrl = null;
            _formProperties = null;
            _isVisible = true;
            _userId = null;
        }

        /// <summary>
        /// Initiating an instance of TaskInfo class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public TaskInfo(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
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
        /// Gets or sets task create time
        /// </summary>
        public string CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                _createTime = value;
            }
        }

        /// <summary>
        /// Gets or sets task finish time
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
        /// Gets or sets task subject
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        /// <summary>
        /// Gets or sets task description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets or sets task instruction
        /// </summary>
        public string Instruction
        {
            get
            {
                return _instruction;
            }
            set
            {
                _instruction = value;
            }
        }

        /// <summary>
        /// Gets or sets activity name
        /// </summary>
        public string ActivityName
        {
            get
            {
                return _activityName;
            }
            set
            {
                _activityName = value;
            }
        }

        /// <summary>
        /// Gets or sets xml string represents a collection of custom actions
        /// </summary>
        public string CustomActionsXml
        {
            get
            {
                return _customActionsXml;
            }
            set
            {
                _customActionsXml = value;
            }
        }

        /// <summary>
        /// Gets or sets url of a custom form
        /// </summary>
        public string CustomFormUrl
        {
            get
            {
                return _customFormUrl;
            }
            set
            {
                _customFormUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets properties of a form
        /// </summary>
        public string FormProperties
        {
            get
            {
                return _formProperties;
            }
            set
            {
                _formProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets data binding schema id
        /// </summary>
        public string BindingSchemaId
        {
            get
            {
                return _bindingSchemaId;
            }
            set
            {
                if (value.Length > 0)
                {
                    _bindingSchemaId = value;
                }
                else
                {
                    _bindingSchemaId = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets data binding class name
        /// </summary>
        public string BindingClassName
        {
            get
            {
                return _bindingClassName;
            }
            set
            {
                if (value.Length > 0)
                {
                    _bindingClassName = value;
                }
                else
                {
                    _bindingClassName = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets data binding i nstanceid
        /// </summary>
        public string BindingObjId
        {
            get
            {
                return _bingdingObjId;
            }
            set
            {
                _bingdingObjId = value;
            }
        }

        /// <summary>
        /// Gets or sets the users of the task
        /// </summary>
        public string Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
            }
        }

        /// <summary>
        /// Gets or sets the roles of the task
        /// </summary>
        public string Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the task is visible in my task list
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }


        /// <summary>
        /// Gets the information indicates whether the task has any custom actions
        /// </summary>
        public bool HasCustomActions
        {
            get
            {
                if (string.IsNullOrEmpty(CustomActionsXml))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the user who executed the task.
        /// </summary>
        /// <remarks>Temp value. No need to reserialize/deserialize </remarks>
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

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            string str = parent.GetAttribute("taskId");
            _taskId = str;

            str = parent.GetAttribute("wfInstanceId");
            _workflowInstanceId = str;

            str = parent.GetAttribute("createTime");
            _createTime = str;

            if (parent["Subject"] != null)
            {
                _subject = parent["Subject"].InnerText;
            }

            if (parent["Description"] != null)
            {
                _description = parent["Description"].InnerText;
            }

            if (parent["Instruction"] != null)
            {
                _instruction = parent["Instruction"].InnerText;
            }

            str = parent.GetAttribute("schemaId");
            _bindingSchemaId = str;

            str = parent.GetAttribute("className");
            _bindingClassName = str;

            str = parent.GetAttribute("activityName");
            _activityName = str;

            str = parent.GetAttribute("users");
            _users = str;

            str = parent.GetAttribute("roles");
            _roles = str;

            str = parent.GetAttribute("visible");
            if (!string.IsNullOrEmpty(str) && str == "false")
            {
                _isVisible = false;
            }
            else
            {
                _isVisible = true;
            }

            if (parent["CustomActionsXml"] != null)
            {
                _customActionsXml = UnescapeXml(parent["CustomActionsXml"].InnerText);
            }

            str = parent.GetAttribute("customFormUrl");
            _customFormUrl = str;

            str = parent.GetAttribute("formProperties");
            _formProperties = str;
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public void Marshal(XmlElement parent)
        {
            parent.SetAttribute("taskId", _taskId);

            parent.SetAttribute("wfInstanceId", _workflowInstanceId);

            parent.SetAttribute("createTime", _createTime);

            XmlElement child;
            if (!string.IsNullOrEmpty(_subject))
            {
                child = parent.OwnerDocument.CreateElement("Subject");
                child.InnerText = _subject;
                parent.AppendChild(child);
            }

            if (!string.IsNullOrEmpty(_description))
            {
                child = parent.OwnerDocument.CreateElement("Description");
                child.InnerText = _description;
                parent.AppendChild(child);
            }

            if (!string.IsNullOrEmpty(_instruction))
            {
                child = parent.OwnerDocument.CreateElement("Instruction");
                child.InnerText = _instruction;
                parent.AppendChild(child);
            }

            if (!string.IsNullOrEmpty(_bindingSchemaId))
            {
                parent.SetAttribute("schemaId", _bindingSchemaId);
            }

            if (!string.IsNullOrEmpty(_bindingClassName))
            {
                parent.SetAttribute("className", _bindingClassName);
            }

            if (!string.IsNullOrEmpty(_activityName))
            {
                parent.SetAttribute("activityName", _activityName);
            }

            if (!string.IsNullOrEmpty(_users))
            {
                parent.SetAttribute("users", _users);
            }

            if (!string.IsNullOrEmpty(_roles))
            {
                parent.SetAttribute("roles", _roles);
            }

            if (!_isVisible)
            {
                // default is true
                parent.SetAttribute("visible", "false");
            }

            if (!string.IsNullOrEmpty(_customActionsXml))
            {
                child = parent.OwnerDocument.CreateElement("CustomActionsXml");
                // esacpe the xml characters since it will be inserted into a xquery
                child.InnerText = EscapeXml(_customActionsXml);
                parent.AppendChild(child);
            }

            if (!string.IsNullOrEmpty(_customFormUrl))
            {
                parent.SetAttribute("customFormUrl", _customFormUrl);
            }

            if (!string.IsNullOrEmpty(_formProperties))
            {
                parent.SetAttribute("formProperties", _formProperties);
            }
        }

        /// <summary>
        /// Create a new TaskInfo by cloning this TaskInfo
        /// </summary>
        /// <returns>A cloned TaskInfo</returns>
        public TaskInfo Clone()
        {
            // use Marshal and Unmarshal to clone a TaskInfo
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("TaskInfos");
            doc.AppendChild(root);
            XmlElement child = doc.CreateElement("TaskInfo");
            this.Marshal(child);
            root.AppendChild(child);

            // create a new TaskInfo and unmarshal from the xml element as source
            TaskInfo newTaskInfo = new TaskInfo(child);

            return newTaskInfo;
        }

        private string EscapeXml(string xml)
        {
            string s = xml;
            s.Replace("&", "&amp;");
            s.Replace("<", "&lt;");
            s.Replace(">", "&gt;");
            s.Replace("\"", "&quot;");
            s.Replace("'", "&apos;");

            return s;
        }

        private string UnescapeXml(string s)
        {
            string xml = s;
            xml.Replace("&amp;", "&");
            s.Replace("&lt;", "<");
            s.Replace("&gt;", ">");
            s.Replace("&quot;", "\"");
            s.Replace("&apos;", "'");

            return xml;
        }
	}
}