/*
* @(#)WorkflowInstanceStateInfo.cs
*
* Copyright (c) 2003-2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;
    using System.Text;

	/// <summary>
	/// A class contains information about the current state of a workflow instance.
	/// </summary>
	/// <version>1.0.0 28 Dec 2008 </version>
	public class WorkflowInstanceStateInfo
	{
        private string _workflowInstanceId;
        private bool _unlocked;
        private DateTime _modifiedTime;
        private byte[] _state;

        public WorkflowInstanceStateInfo()
        {
            _workflowInstanceId = null;
            _unlocked = true;
            _modifiedTime = new DateTime();
            _state = null;
        }

        /// <summary>
        /// Initiating an instance of WorkflowInstanceStateInfo class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public WorkflowInstanceStateInfo(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
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
        /// Gets or sets state modified time
        /// </summary>
        public DateTime ModifiedTime
        {
            get
            {
                return _modifiedTime;
            }
            set
            {
                _modifiedTime = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether the state is unlocked or not,
        /// default is true
        /// </summary>
        public bool Unlocked
        {
            get
            {
                return _unlocked;
            }
            set
            {
                _unlocked = value;
            }
        }

        /// <summary>
        /// Gets or sets state binary data
        /// </summary>
        public byte[] State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
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

            str = parent.GetAttribute("modifiedTime");
            _modifiedTime = DateTime.Parse(str);

            str = parent.GetAttribute("unlocked");
            if (!String.IsNullOrEmpty(str) && str == "true")
            {
                _unlocked = true;
            }
            else
            {
                _unlocked = false;
            }

            // do not write _state which is a binary data
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public void Marshal(XmlElement parent)
        {
            parent.SetAttribute("wfInstanceId", _workflowInstanceId);

            parent.SetAttribute("modifiedTime", _modifiedTime.ToString());

            if (_unlocked)
            {
                parent.SetAttribute("unlocked", "true");
            }

            // do not write _state which is a binary data
        }
	}
}