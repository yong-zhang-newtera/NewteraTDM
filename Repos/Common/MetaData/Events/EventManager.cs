/*
* @(#)EventManager.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema.Validate;

	/// <summary>
	/// This is the top level class that manages events associated with
	/// all classes in a schema and provides methods to allow easy accesses, addition, and 
	/// deletion of the events.
	/// </summary>
	/// <version> 1.0.0 22 Dec 2006 </version>
	/// <author> Yong Zhang </author>
	public class EventManager : EventNodeBase
	{
		private bool _isAltered;
		private EventGroupCollection _eventGroups;
        private Hashtable _hasEventsTable;

		/// <summary>
		/// Initiate an instance of EventManager class
		/// </summary>
		public EventManager(): base()
		{
			_isAltered = false;
			_eventGroups = new EventGroupCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _eventGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
            _hasEventsTable = new Hashtable();
		}

		/// <summary>
		/// Gets or sets the information indicating whether eventDef information has been
		/// altered.
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;

                if (value)
                {
                    this.FireValueChangedEvent(value);
                }
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty event set
		/// </summary>
		/// <value>true if it is an empty event set, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._eventGroups.Count == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets all timer events in a schema
        /// </summary>
        /// <returns>event collectionEv</returns>
        public EventCollection GetTimerEvents()
        {
            EventCollection timerEvents = new EventCollection();

            foreach (EventGroup eventGroup in _eventGroups)
            {
                foreach (EventDef eventDef in eventGroup.Events)
                {
                    if (eventDef.OperationType == OperationType.Timer)
                    {
                        timerEvents.Add(eventDef);
                    }
                }
            }

            return timerEvents;
        }

        /// <summary>
        /// Gets the information indicating whether a class has the events defined
        /// </summary>
        /// <param name="classElement">The class element</param>
        /// <returns>true if it has events defined, false otherwise</returns>
        public bool HasEvents(ClassElement classElement)
        {
            bool status = false;

            if (this._hasEventsTable[classElement.Name] == null)
            {
                EventCollection events = this.GetClassEvents(classElement);
               
                if (events != null && events.Count > 0)
                {
                    status = true;
                }

                this._hasEventsTable[classElement.Name] = status;
            }
            else
            {
                status = (bool)this._hasEventsTable[classElement.Name];
            }

            return status;
        }

		/// <summary>
		/// Gets the events, including the inherited ones, for a class
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of Events</returns>
		public EventCollection GetClassEvents(ClassElement classElement)
		{
            EventCollection events = new EventCollection();
            EventGroup eventGroup;
            ClassElement currentClass = classElement;

            while (currentClass != null)
            {
                eventGroup = (EventGroup)this._eventGroups[currentClass.Name];
                if (eventGroup != null)
                {
                    foreach (EventDef eventDef in eventGroup.Events)
                    {
                        events.Add(eventDef);
                    }
                }

                // get the events inherited from the parent class
                currentClass = currentClass.ParentClass;
            }

			return events;
		}

		/// <summary>
		/// Gets the events (not including the inherited ones) for a class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of Events</returns>
		public EventCollection GetLocalEvents(ClassElement classElement)
		{
            EventGroup eventGroup = (EventGroup)this._eventGroups[classElement.Name];

            if (eventGroup != null)
            {
                return eventGroup.Events;
            }
            else
            {
                return null;
            }
		}

		/// <summary>
		/// Gets the information indicating whether a event has already existed for a
		/// class or for its parent classes.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="eventDef">The eventDef</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsEventExist(ClassElement classElement, EventDef eventDef)
		{
			bool isExist = false;

			EventCollection events = GetClassEvents(classElement);

			foreach (EventDef tmpEvent in events)
			{
				if (eventDef.Name == tmpEvent.Name)
				{
					isExist = true;
					break;
				}
			}

			return isExist;
		}

		/// <summary>
		/// Gets the information indicating whether a local event has already existed for a
		/// class or for its parent classes.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="eventDef">The event</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsLocalEventExist(ClassElement classElement, EventDef eventDef)
		{
			bool isExist = false;

			EventCollection events = GetLocalEvents(classElement);

            if (events != null)
            {
                foreach (EventDef tmpEvent in events)
                {
                    if (eventDef.Name == tmpEvent.Name)
                    {
                        isExist = true;
                        break;
                    }
                }
            }

			return isExist;
		}

		/// <summary>
		/// Add a event for a class
		/// </summary>
		/// <param name="classElement">The class Element</param>
		/// <param name="eventDef">The EventDef</param>
		public void AddEvent(ClassElement classElement, EventDef eventDef)
		{
			EventGroup eventGroup = (EventGroup) this._eventGroups[classElement.Name];

            // Create a EventGroup instance for the class element if it doesn't exist.
			if (eventGroup == null)
			{
				// class name is unique among the classes in a schema
				eventGroup = new EventGroup(classElement.Name);
				_eventGroups.Add(eventGroup);
			}
			
			if (!IsEventExist(classElement, eventDef))
			{
				eventGroup.Events.Add(eventDef);

                _hasEventsTable.Remove(classElement.Name);
			}
		}

		/// <summary>
		/// Remove a event from a class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="eventDef">The EventDef to be removed</param>
		public void RemoveEvent(ClassElement classElement, EventDef eventDef)
		{
            EventGroup eventGroup = (EventGroup)this._eventGroups[classElement.Name];

			if (eventGroup != null)
			{
				eventGroup.Events.Remove(eventDef);

                _hasEventsTable.Remove(classElement.Name);
			}
		}

        /// <summary>
        /// Remove events belongs to a class.
        /// </summary>
        /// <param name="classElement">The class element</param>
        public void RemoveEvents(ClassElement classElement)
        {
            EventGroup eventGroup = (EventGroup)this._eventGroups[classElement.Name];

            if (eventGroup != null)
            {
                this._eventGroups.Remove(eventGroup);

                _hasEventsTable.Remove(classElement.Name);
            }
        }

        /// <summary>
        /// Get information indicating whether an attribute is referenced by events in any of
        /// in the collection.
        /// </summary>
        /// <param name="className">The name of attribute owner class</param>
        /// <param name="attributeName">The name of the attribute to be found</param>
        /// <returns>true if the attribute is referenced, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName, out string eventCaption)
        {
            bool status = false;
            eventCaption = null;

            EventGroup eventGroup = (EventGroup)this._eventGroups[className];

            if (eventGroup != null)
            {
                foreach (EventDef eventDef in eventGroup.Events)
                {
                    if (eventDef.IsAttributeReferenced(className, attributeName))
                    {
                        eventCaption = eventDef.Name;
                        status = true;
                        break;
                    }
                }
            }



            return status;
        }

        /// <summary>
        /// Validate the event definitions to see if they are valid or not.
        /// </summary>
        /// <param name="metaData">The meta data model to locate the schema model element associated with offending logging rules</param>
        /// <param name="userManager">The User Manager to get users or roles data</param>
        /// <param name="result">The validate result to which to append logging validating errors.</param>
        /// <returns>The result in ValidateResult object</returns>
        public ValidateResult Validate(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
        {
            EventModelValidateVisitor visitor = new EventModelValidateVisitor(metaData, userManager, result);

            Accept(visitor); // start validating

            return visitor.ValidateResult;
        }

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IEventNodeVisitor visitor)
        {
            if (visitor.VisitEventManager(this))
            {
                this._eventGroups.Accept(visitor);
            }
        }

		/// <summary>
		/// Read events from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					//Open the stream and read XSD from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new EventException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read events from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// read the stream.</exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(stream);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new EventException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read events from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// read the text reader</exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(reader);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new EventException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write events to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// write to the file.</exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new EventException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write events as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new EventException("Failed to write the events", ex);
			}
		}

		/// <summary>
		/// Write events as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="EventException">EventException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new EventException("Failed to write the events", ex);
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
				return EventNodeType.EventManager;
			}
		}

		/// <summary>
		/// create events from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of EventGroup instances
            _eventGroups = (EventGroupCollection)EventNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _eventGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write events to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the event defs
            XmlElement child = parent.OwnerDocument.CreateElement(EventNodeFactory.ConvertTypeToString(_eventGroups.NodeType));
			_eventGroups.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("EventManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the EventManager changed
		/// </summary>
		/// <param name="sender">the IEventNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}
	}
}