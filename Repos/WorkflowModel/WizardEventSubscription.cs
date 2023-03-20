/*
* @(#)WizardEventSubscription.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Xml;

namespace Newtera.WFModel
{
    [Serializable]
    public class WizardEventSubscription
    {
        private string _subscriptionId;
        private string _eventName;
        private Guid _workflowInstanceId;
        private string _queueName;

        public WizardEventSubscription()
        {
            this._subscriptionId = string.Empty;
            this._eventName = string.Empty;
            this._workflowInstanceId = Guid.Empty;
            this._queueName = string.Empty;
        }

        public WizardEventSubscription(string eventName,
                    Guid workflowInstanceId, string queueName)
        {
            this._subscriptionId = string.Empty;
            this._eventName = eventName;
            this._workflowInstanceId = workflowInstanceId;
            this._queueName = queueName;
        }

        /// <summary>
        /// Initiating an instance of WizardEventSubscription class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public WizardEventSubscription(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
        }

        public string SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = value; }
        }

        public string EventName
        {
            get { return _eventName; }
            set { _eventName = value; }
        }

        public Guid WorkflowInstanceId
        {
            get { return _workflowInstanceId; }
            set { _workflowInstanceId = value; }
        }

        public string QueueName
        {
            get { return _queueName; }
            set { _queueName = value; }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            string str = parent.GetAttribute("subscriptionId");
            _subscriptionId = str;

            str = parent.GetAttribute("eventName");
            _eventName = str;

            str = parent.GetAttribute("wfInstanceId");
            _workflowInstanceId = new Guid(str);

            str = parent.GetAttribute("queueName");
            _queueName = str;
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

            parent.SetAttribute("eventName", _eventName);

            parent.SetAttribute("wfInstanceId", _workflowInstanceId.ToString());

            parent.SetAttribute("queueName", (string)_queueName);
        }
    }
}
