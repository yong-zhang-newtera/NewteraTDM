/*
* @(#)NewteraTrackingWorkflowInstance.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
    using System.IO;
    using System.Text;
    using System.Workflow.Runtime;
    using System.Workflow.ComponentModel;

	/// <summary>
    /// Provides access to tracking data maintained in a Newtera database by
    /// the NewteraTrackingService for a workflow instance 
	/// </summary>
	/// <version>1.0.6 3 Jan 2007</version>
	public class NewteraTrackingWorkflowInstance : WFModelElementBase
	{
        private NewteraActivityTrackingRecordCollection _activityEvents;
        private DateTime _initialized = new DateTime();
        private WorkflowStatus _status = WorkflowStatus.Created;
        private Guid _workflowInstanceId = new Guid();
        private string _trackingEvent = null;

		/// <summary>
		/// Initiating an instance of NewteraTrackingWorkflowInstance class
		/// </summary>
        /// <param name="name">Name of the workflow.</param>
		public NewteraTrackingWorkflowInstance(string name) : base(name)
		{
            _activityEvents = new NewteraActivityTrackingRecordCollection();
		}

		/// <summary>
		/// Initiating an instance of NewteraTrackingWorkflowInstance class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal NewteraTrackingWorkflowInstance(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets the list of activity tracking records that have been sent for this workflow
        /// instance to a NewteraTrackingService database by the runtime tracking infrastructure
        /// </summary>
        public NewteraActivityTrackingRecordCollection ActivityEvents
        {
            get
            {
                return this._activityEvents;
            }
        }

        /// <summary>
        /// Gets or sets a DateTime that indicates the time at which the first TrackingChannel
        /// for this workflow instance was requested by the workflow runtime engine. 
        /// </summary>
        public DateTime Initialized
        {
            get
            {
                return _initialized;
            }
            set
            {
                this._initialized = value;
            }
        }

        /// <summary>
        /// Gets or sets the status of the workflow instance
        /// </summary>
        public WorkflowStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        /// The Guid of the workflow instance for which this NewteraTrackingWorkflowInstance applies. 
        /// </summary>
        public Guid WorkflowInstanceId
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
        /// Gets or sets the event name for the tracking record. 
        /// </summary>
        public string TrackingEvent
        {
            get
            {
                return _trackingEvent;
            }
            set
            {
                _trackingEvent = value;
            }
        }

        /// <summary>
        /// Gets an Activity that represents the current workflow definition for the workflow instance.
        /// </summary>
        public Activity WorkflowDefinition
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.TrackingInstance;
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
            if (visitor.VisitTrackingWorkflowInstance(this))
            {
                foreach (IWFModelElement element in _activityEvents)
                {
                    element.Accept(visitor);
                }
            }
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            string str = parent.GetAttribute("initialized");
            _initialized = DateTime.Parse(str);

            str = parent.GetAttribute("instanceid");
            _workflowInstanceId = new Guid(str);

            str = parent.GetAttribute("status");
            if (str != null && str.Length > 0)
            {
                _status = (WorkflowStatus)Enum.Parse(typeof(WorkflowStatus), str);
            }

            str = parent.GetAttribute("eventName");
            if (!string.IsNullOrEmpty(str))
            {
                _trackingEvent = str;
            }

            // then a collection of NewteraActivityTrackingRecord instances
            _activityEvents = (NewteraActivityTrackingRecordCollection) ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("initialized", _initialized.ToString());

            parent.SetAttribute("status", Enum.GetName(typeof(WorkflowStatus), _status));

            parent.SetAttribute("instanceid", _workflowInstanceId.ToString());

            parent.SetAttribute("eventName", _trackingEvent);

            // write the activity events
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(this._activityEvents.ElementType));
            this._activityEvents.Marshal(child);
            parent.AppendChild(child);
		}
	}
}