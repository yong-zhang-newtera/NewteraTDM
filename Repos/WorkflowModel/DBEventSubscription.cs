/*
* @(#)DBEventSubscription.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Xml;

namespace Newtera.WFModel
{
    [Serializable]
    public class DBEventSubscription
    {
        private string _subscriptionId;
        private string _schemaId;
        private string _className;
        private string _eventName;
        private Guid _workflowInstanceId;
        private string _queueName;
        private bool _createDataBinding;

        public DBEventSubscription()
        {
            this._subscriptionId = string.Empty;
            this._schemaId = string.Empty;
            this._className = string.Empty;
            this._eventName = string.Empty;
            this._workflowInstanceId = Guid.Empty;
            this._queueName = string.Empty;
            this._createDataBinding = false; // default
        }

        public DBEventSubscription(string schemaId,
                    string className, string eventName,
                    Guid workflowInstanceId, string queueName,
                    bool createDataBinding)
        {
            this._subscriptionId = string.Empty;
            this._schemaId = schemaId;
            this._className = className;
            this._eventName = eventName;
            this._workflowInstanceId = workflowInstanceId;
            this._queueName = queueName;
            this._createDataBinding = createDataBinding;
        }

        /// <summary>
        /// Initiating an instance of DBEventSubscription class
        /// </summary>
        /// <param name="xmlElement">The xml element conatins data of the instance</param>
        public DBEventSubscription(XmlElement xmlElement)
        {
            Unmarshal(xmlElement);
        }

        public string SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = value; }
        }

        public string SchemaId
        {
            get { return _schemaId; }
            set { _schemaId = value; }
        }

        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
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

        public bool CreateDataBinding
        {
            get { return _createDataBinding; }
            set { _createDataBinding = value; }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            string str = parent.GetAttribute("subscriptionId");
            _subscriptionId = str;

            str = parent.GetAttribute("schemaId");
            _schemaId = str;

            str = parent.GetAttribute("className");
            _className = str;

            str = parent.GetAttribute("eventName");
            _eventName = str;

            str = parent.GetAttribute("wfInstanceId");
            _workflowInstanceId = new Guid(str);

            str = parent.GetAttribute("queueName");
            _queueName = str;

            str = parent.GetAttribute("dataBinding");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _createDataBinding = true;
            }
            else
            {
                _createDataBinding = false;
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

            parent.SetAttribute("schemaId", _schemaId);

            parent.SetAttribute("className", _className);

            parent.SetAttribute("eventName", _eventName);

            parent.SetAttribute("wfInstanceId", _workflowInstanceId.ToString());

            parent.SetAttribute("queueName", (string)_queueName);

            if (_createDataBinding)
            {
                parent.SetAttribute("dataBinding", "true");
            }
        }
    }
}
