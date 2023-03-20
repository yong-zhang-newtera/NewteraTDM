/*
* @(#)EventDef.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Xml;
    using System.Text;
    using System.Collections.Specialized;
	using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// The class represents definition of a event for a class.
	/// </summary>
	/// <version>1.0.0 22 Dec 2006</version>
	public partial class EventDef : EventNodeBase
	{
		private string _className;
        private OperationType _opType;
        private IDataViewElement _afterCondition;
        private StringCollection _attributesUpdated;
        private TimerInterval _interval;
        private string _timerCondition;
        private DateTime _lastCheckedTime;
        private string _afterConditionInXQuery;

        private MetaDataModel _metaData = null; // run-time value
		
		/// <summary>
		/// Initiate an instance of EventDef class.
		/// </summary>
		public EventDef() : base()
		{
			_className = null;
            _opType = OperationType.Unknown;
            _attributesUpdated = new StringCollection();
            _afterCondition = null;
            _interval = TimerInterval.EveryDay;
            _timerCondition = null;
            _lastCheckedTime = DateTime.Now;
            _afterConditionInXQuery = null;
           
            // set default name
            this.Name = "Event1";
		}

		/// <summary>
		/// Initiating an instance of EventDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal EventDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets type of operation that trigger the event.
        /// </summary>
        /// <value>One of the OperationType enum values</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Type of data operation that raises the event."),
            DefaultValueAttribute(OperationType.Unknown)
        ]	
        public OperationType OperationType
        {
            get
            {
                return _opType;
            }
            set
            {
                _opType = value;
            }
        }

        /// <summary>
        /// Gets a collection of attribute names whose values have to be updated for the event can be raised.
        /// </summary>
        /// <value>A StringCollection of attribute names.</value>
        /// <remarks>This property is valid for instance update event. If no attributes specified, any changes of instance values will raise a event.</remarks>
        [
            CategoryAttribute("Database"),
            DescriptionAttribute("Specify the attributes whose values have to be updated by the update before the event to be raised."),
            EditorAttribute("Newtera.Studio.AttributesUpdatedPropertyEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.AttributesUpdatedPropertyConverter, Studio")
        ]
        public StringCollection AttributesUpdated
        {
            get
            {
                return _attributesUpdated;
            }
            set
            {
                _attributesUpdated = value;
            }
        }

        /// <summary>
        /// Gets the after-operation condition that must be met for the event can be raised.
        /// </summary>
        /// <value>A IDataViewElement instance represents the condition.</value>
        /// <remarks>If there is no after condition, the result of evaluation will be true.</remarks>
        [
            CategoryAttribute("Database"),
            DescriptionAttribute("An after-condition expression which must be evaluated to be true for the event to be raised."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.EventConditionPropertyEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.EventConditionPropertyConverter, Studio")
        ]
        public IDataViewElement AfterCondition
        {
            get
            {
                return _afterCondition;
            }
            set
            {
                _afterCondition = value;
            }
        }

        /// <summary>
        /// Gets or sets interval of the timer that triggers
        /// </summary>
        /// <value>One of the TimerIntervals enum values</value>
        [
            CategoryAttribute("Timer"),
            DescriptionAttribute("Interval of timer that raises the event."),
            DefaultValueAttribute(TimerInterval.EveryDay)
        ]
        public TimerInterval TimerInterval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
            }
        }

        /// <summary>
        /// Gets or sets condition when timer event is raised.
        /// </summary>
        /// <value>An XQuery condition</value>
        [
            CategoryAttribute("Timer"),
            DescriptionAttribute("The condition when timer event is raised."),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string TimerCondition
        {
            get
            {
                return _timerCondition;
            }
            set
            {
                _timerCondition = value;
            }
        }

        /// <summary>
        /// Gets or sets datetime when the event is last checked by the timer
        /// </summary>
        /// <value>The datetime object</value>
        [BrowsableAttribute(false)]
        public DateTime LastCheckedTime
        {
            get
            {
                return _lastCheckedTime;
            }
            set
            {
                _lastCheckedTime = value;
            }
        }

		/// <summary>
		/// Gets or sets name of the class that this rule is associated with.
		/// </summary>
		/// <value> The unique class name.</value>
        [BrowsableAttribute(false)]
        public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

        /// <summary>
        /// Gets or sets MetaDataModel instance.
        /// </summary>
        [BrowsableAttribute(false)]
        public MetaDataModel MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                _metaData = value;
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of EventNodeType values</value>
        [BrowsableAttribute(false)]		
        public override EventNodeType NodeType
		{
			get
			{
				return EventNodeType.EventDef;
			}
		}

        /// <summary>
        /// Get the event condition in xquey format
        /// </summary>
        [BrowsableAttribute(false)]	
        public string AfterConditionInXQuery
        {
            get
            {
                if (_afterConditionInXQuery == null && AfterCondition != null)
                {
                    _afterConditionInXQuery = ((IQueryElement)AfterCondition).ToXQuery();

                    // replace the variable name from the class name to "this"
                    _afterConditionInXQuery = _afterConditionInXQuery.Replace(ClassName.ToLower(), "this");
                }

                return _afterConditionInXQuery;
            }
        }

        /// <summary>
        /// Get information indicating whether an attribute is referenced by the event definition.
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced by the event, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;

            // check in the _afterCondition first
            if (_afterCondition != null)
            {
                FindSearchAttributeVisitor visitor = new FindSearchAttributeVisitor(className, attributeName);
                _afterCondition.Accept(visitor);
                if (visitor.IsFound)
                {
                    status = true;
                }
            }

            if (!status)
            {
                if (_attributesUpdated != null)
                {
                    // checking the _attributesUpdated
                    foreach (string attrName in _attributesUpdated)
                    {
                        if (attrName == attributeName)
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IEventNodeVisitor visitor)
        {
            visitor.VisitEventDef(this);
        }

		/// <summary>
		/// create an EventDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("class");
			if (str != null && str.Length > 0)
			{
				_className = str;
			}
			else
			{
				_className = null;
			}

            str = parent.GetAttribute("optype");
            if (str != null && str.Length > 0)
            {
                _opType = (OperationType)Enum.Parse(typeof(OperationType), str);
            }
            else
            {
                _opType = OperationType.Unknown;
            }

            str = parent.GetAttribute("AttributesUpdated");
            this._attributesUpdated = new StringCollection();
            if (str != null && str.Length > 0)
            {
                string[] attrNames = str.Split(' ');
                for (int i = 0; i < attrNames.Length; i++)
                {
                    this._attributesUpdated.Add(attrNames[i]);
                }
            }

            try
            {
                if (((XmlElement)parent.ChildNodes[0]).Name != "Empty")
                {
                    _afterCondition = ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
                }
                else
                {
                    _afterCondition = null;
                }
            }
            catch (Exception)
            {
            }

            str = parent.GetAttribute("interval");
            if (!string.IsNullOrEmpty(str))
            {
                _interval = (TimerInterval)Enum.Parse(typeof(TimerInterval), str);
            }
            else
            {
                _interval = TimerInterval.EveryDay;
            }

            str = parent.GetAttribute("timerCondition");
            if (!string.IsNullOrEmpty(str))
            {
                _timerCondition = str;
            }
            else
            {
                _timerCondition = null;
            }

            // set the last checked time to the current machine time
            _lastCheckedTime = DateTime.Now;
		}

		/// <summary>
		/// write EventDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}

            if (_opType != OperationType.Unknown)
            {
                parent.SetAttribute("optype", Enum.GetName(typeof(OperationType), _opType));
            }

            if (_attributesUpdated.Count > 0)
            {
                StringBuilder buffer = new StringBuilder();
                int index = 0;
                foreach (string str in _attributesUpdated)
                {
                    if (index == 0)
                    {
                        buffer.Append(str);
                    }
                    else
                    {
                        buffer.Append(" ").Append(str);
                    }

                    index++;
                }

                parent.SetAttribute("AttributesUpdated", buffer.ToString());
            }

            XmlElement child;
            if (_afterCondition != null)
            {
                child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_afterCondition.ElementType));
                _afterCondition.Marshal(child);
                parent.AppendChild(child);
            }
            else
            {
                child = parent.OwnerDocument.CreateElement("Empty");
                parent.AppendChild(child);
            }

            if (_interval != TimerInterval.EveryDay)
            {
                parent.SetAttribute("interval", Enum.GetName(typeof(TimerInterval), _interval));
            }

            if (!string.IsNullOrEmpty(_timerCondition))
            {
                parent.SetAttribute("timerCondition", _timerCondition);
            }
		}

        /// <summary>
        /// Clone an event
        /// </summary>
        /// <returns>The cloned event.</returns>
        public EventDef Clone()
        {
            // use Marshal and Unmarshal to clone a EventDef
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Event");
            doc.AppendChild(root);
            XmlElement child = doc.CreateElement(EventNodeFactory.ConvertTypeToString(this.NodeType));
            this.Marshal(child);
            root.AppendChild(child);

            // create a new EventDef and unmarshal from the xml element as source
            EventDef eventDef = new EventDef(child);
            eventDef.MetaData = this.MetaData;
            eventDef.LastCheckedTime = this.LastCheckedTime;

            return eventDef;
        }
	}
}