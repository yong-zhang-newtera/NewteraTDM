/*
* @(#)NewteraActivityTrackingRecord.cs
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
    using System.Workflow.ComponentModel;

	/// <summary>
    /// Contains the data sent to a tracking service by the runtime tracking infrastructure
    /// when an ActivityTrackPoint is matched. It is also used in the return list of the
    /// ActivityEvents property. 
	/// </summary>
	/// <version>1.0.6 3 Jan 2007</version>
	public class NewteraActivityTrackingRecord : WFModelElementBase
	{
        private int _eventOrder = 0;
        private ActivityExecutionStatus _executionStatus = ActivityExecutionStatus.Initialized;
        private string _typeName = null;
        private string _qualifiedName = null;
        private DateTime _initialized = new DateTime();

		/// <summary>
		/// Initiating an instance of NewteraActivityTrackingRecord class
		/// </summary>
        /// <param name="name">Name of the workflow.</param>
		public NewteraActivityTrackingRecord(string name) : base(name)
		{
		}

		/// <summary>
		/// Initiating an instance of NewteraActivityTrackingRecord class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal NewteraActivityTrackingRecord(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets a value that indicates the order in the workflow instance
        /// of the activity status event that matched the ActivityTrackPoint. 
        /// </summary>
        public int EventOrder
        {
            get
            {
                return _eventOrder;
            }
            set
            {
                _eventOrder = value;
            }
        }

        /// <summary>
        /// Gets or sets a DateTime that indicates the time at which the first TrackingChannel
        /// for this activity instance was requested by the workflow runtime engine. 
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
        /// Gets or sets the execution status of the activity associated with this ActivityTrackingRecord. 
        /// </summary>
        public ActivityExecutionStatus ExecutionStatus
        {
            get
            {
                return _executionStatus;
            }
            set
            {
                _executionStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the type name of the activity associated with this ActivityTrackingRecord.
        /// </summary>
        public string TypeName
        {
            get
            {
                return _typeName;
            }
            set
            {
                _typeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier of the activity associated with this ActivityTrackingRecord.
        /// </summary>
        public string QualifiedName
        {
            get
            {
                return _qualifiedName;
            }
            set
            {
                _qualifiedName = value;
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
                return ElementType.ActivityTrackingRecord;
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			visitor.VisitActivityTrackingRecord(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            string str = parent.GetAttribute("eventorder");
            _eventOrder = Int32.Parse(str);

            str = parent.GetAttribute("executionstatus");
            _executionStatus = (ActivityExecutionStatus)Enum.Parse(typeof(ActivityExecutionStatus), str);

            str = parent.GetAttribute("qname");
            if (str != null && str.Length > 0)
            {
                _qualifiedName = str;
            }

            str = parent.GetAttribute("tname");
            if (str != null && str.Length > 0)
            {
                _typeName = str;
            }

            str = parent.GetAttribute("initialized");
            _initialized = DateTime.Parse(str);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("eventorder", _eventOrder.ToString());

            parent.SetAttribute("executionstatus", _executionStatus.ToString());

            parent.SetAttribute("initialized", _initialized.ToString());

            if (_qualifiedName != null)
            {
                parent.SetAttribute("qname", _qualifiedName);
            }

            if (_typeName != null)
            {
                parent.SetAttribute("tname", _typeName);
            }
		}
	}
}