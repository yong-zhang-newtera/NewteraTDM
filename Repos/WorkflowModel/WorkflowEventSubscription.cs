/*
* @(#)WorkflowEventSubscription.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Xml;

namespace Newtera.WFModel
{
    [Serializable]
    public class WorkflowEventSubscription
    {
        private string _subscriptionId;
        private Guid _parentWorkflowInstanceId;
        private Guid _childWorkflowInstanceId;
        private string _queueName;
        private WorkflowEventType _type;

        public WorkflowEventSubscription()
        {
            this._subscriptionId = string.Empty;
            this._parentWorkflowInstanceId = Guid.Empty;
            this._childWorkflowInstanceId = Guid.Empty;
            this._queueName = string.Empty;
            this._type = WorkflowEventType.All;
        }

        public WorkflowEventSubscription(Guid parentWorkflowInstanceId, Guid childWorkflowInstanceId, string queueName)
        {
            this._subscriptionId = string.Empty;
            this._parentWorkflowInstanceId = parentWorkflowInstanceId;
            this._childWorkflowInstanceId = childWorkflowInstanceId;
            this._queueName = queueName;
        }

        /// <summary>
        /// Initiating an instance of WorkflowEventSubscription class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public WorkflowEventSubscription(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
        }

        /// <summary>
        /// Gets or sets the id of the subscription
        /// </summary>
        public string SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = value; }
        }

        /// <summary>
        /// Gets or sets the id of the invoking workflow instance
        /// </summary>
        public Guid ParentWorkflowInstanceId
        {
            get { return _parentWorkflowInstanceId; }
            set { _parentWorkflowInstanceId = value; }
        }

        /// <summary>
        /// Gets or sets the id of the invoked workflow instance
        /// </summary>
        public Guid ChildWorkflowInstanceId
        {
            get { return _childWorkflowInstanceId; }
            set { _childWorkflowInstanceId = value; }
        }

        public string QueueName
        {
            get { return _queueName; }
            set { _queueName = value; }
        }

        public WorkflowEventType EventType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            string str = parent.GetAttribute("subscriptionId");
            _subscriptionId = str;

            str = parent.GetAttribute("parentWFInstanceId");
            _parentWorkflowInstanceId = new Guid(str);

            str = parent.GetAttribute("childWFInstanceId");
            _childWorkflowInstanceId = new Guid(str);

            str = parent.GetAttribute("queueName");
            _queueName = str;

            str = parent.GetAttribute("eventType");
            if (!string.IsNullOrEmpty(str))
            {
                _type = (WorkflowEventType) Enum.Parse(typeof(WorkflowEventType), str);
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public void Marshal(XmlElement parent)
        {
            if (!string.IsNullOrEmpty(_subscriptionId))
            {
                parent.SetAttribute("subscriptionId", _subscriptionId);
            }

            parent.SetAttribute("parentWFInstanceId", _parentWorkflowInstanceId.ToString());

            parent.SetAttribute("childWFInstanceId", _childWorkflowInstanceId.ToString());

            parent.SetAttribute("queueName", (string)_queueName);

            parent.SetAttribute("eventType", Enum.GetName(typeof(WorkflowEventType), _type));
        }
    }

    /// <summary>
    /// Event type enum
    /// </summary>
    public enum WorkflowEventType
    {
        All,
        Completed,
        Terminated,
        Aborted
    }
}
