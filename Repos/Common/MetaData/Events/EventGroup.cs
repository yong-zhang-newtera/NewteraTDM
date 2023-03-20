/*
* @(#)EventGroup.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represent a group of events that are associated with a certain class.
	/// </summary>
	/// <version>  1.0.0 22 Dec 2006</version>
	public class EventGroup : EventNodeBase
	{
		private string _className;
		
		private EventCollection _events;
		
		/// <summary>
		/// Initiate an instance of a EventGroup class.
		/// </summary>
        public EventGroup(string className) : base(className)
		{
			_className = className;
			_events = new EventCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _events.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of EventGroup class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal EventGroup(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets name of the class associated with a EventGroup
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// Gets the events contained in a EventGroup
		/// </summary>
		public EventCollection Events
		{
			get
			{
				return _events;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of EventNodeType values</value>
        public override EventNodeType NodeType 
		{
			get
			{
				return EventNodeType.EventGroup;
			}
		}

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IEventNodeVisitor visitor)
        {
            if (visitor.VisitEventGroup(this))
            {
                this._events.Accept(visitor);
            }
        }

		/// <summary>
		/// create Ruleset instance from a xml document.
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

			// then a collection of  acl rules
            _events = (EventCollection)EventNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _events.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write EventGroup instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _className
			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}			

			// write the rules
            XmlElement child = parent.OwnerDocument.CreateElement(EventNodeFactory.ConvertTypeToString(_events.NodeType));
			_events.Marshal(child);
			parent.AppendChild(child);
		}
	}
}