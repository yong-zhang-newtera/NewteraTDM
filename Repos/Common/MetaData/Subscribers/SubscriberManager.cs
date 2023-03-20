/*
* @(#)SubscriberManager.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
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
	/// This is the top level class that manages subscribers associated with
	/// all classes in a schema and provides methods to allow easy accesses, addition, and 
	/// deletion of the subscribers.
	/// </summary>
	/// <version> 1.0.0 16 Sept 2013 </version>
	public class SubscriberManager : SubscriberNodeBase
	{
		private bool _isAltered;
		private SubscriberGroupCollection _subscriberGroups;
        private Hashtable _hasSubscribersTable;

		/// <summary>
		/// Initiate an instance of SubscriberManager class
		/// </summary>
		public SubscriberManager(): base()
		{
			_isAltered = false;
			_subscriberGroups = new SubscriberGroupCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subscriberGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
            _hasSubscribersTable = new Hashtable();
		}

		/// <summary>
		/// Gets or sets the information indicating whether subscriber information has been
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
		/// Gets the information indicating whether it is an empty subscriber set
		/// </summary>
		/// <value>true if it is an empty subscriber set, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._subscriberGroups.Count == 0)
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
        /// Gets the information indicating whether a class has the subscribers defined
        /// </summary>
        /// <param name="classElement">The class element</param>
        /// <returns>true if it has subscribers defined, false otherwise</returns>
        public bool HasSubscribers(ClassElement classElement)
        {
            bool status = false;

            if (this._hasSubscribersTable[classElement.Name] == null)
            {
                SubscriberCollection subscribers = this.GetClassSubscribers(classElement);
               
                if (subscribers != null && subscribers.Count > 0)
                {
                    status = true;
                }

                this._hasSubscribersTable[classElement.Name] = status;
            }
            else
            {
                status = (bool)this._hasSubscribersTable[classElement.Name];
            }

            return status;
        }

		/// <summary>
		/// Gets the subscribers, including the inherited ones, for a class
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of Subscribers</returns>
		public SubscriberCollection GetClassSubscribers(ClassElement classElement)
		{
            SubscriberCollection subscribers = new SubscriberCollection();
            SubscriberGroup subscriberGroup;
            ClassElement currentClass = classElement;

            while (currentClass != null)
            {
                subscriberGroup = (SubscriberGroup)this._subscriberGroups[currentClass.Name];
                if (subscriberGroup != null)
                {
                    foreach (Subscriber subscriber in subscriberGroup.Subscribers)
                    {
                        subscribers.Add(subscriber);
                    }
                }

                // get the subscribers inherited from the parent class
                currentClass = currentClass.ParentClass;
            }

			return subscribers;
		}

		/// <summary>
		/// Gets the subscribers (not including the inherited ones) for a class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of Subscribers</returns>
		public SubscriberCollection GetLocalSubscribers(ClassElement classElement)
		{
            SubscriberGroup subscriberGroup = (SubscriberGroup)this._subscriberGroups[classElement.Name];

            if (subscriberGroup != null)
            {
                return subscriberGroup.Subscribers;
            }
            else
            {
                return null;
            }
		}

		/// <summary>
		/// Gets the information indicating whether a subscriber has already existed for a
		/// class or for its parent classes.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="subscriber">The subscriber</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsSubscriberExist(ClassElement classElement, Subscriber subscriber)
		{
			bool isExist = false;

			SubscriberCollection subscribers = GetClassSubscribers(classElement);

			foreach (Subscriber tmpSubscriber in subscribers)
			{
				if (subscriber.Name == tmpSubscriber.Name)
				{
					isExist = true;
					break;
				}
			}

			return isExist;
		}

		/// <summary>
		/// Gets the information indicating whether a local subscriber has already existed for a
		/// class or for its parent classes.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="subscriber">The subscriber</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsLocalSubscriberExist(ClassElement classElement, Subscriber subscriber)
		{
			bool isExist = false;

			SubscriberCollection subscribers = GetLocalSubscribers(classElement);

            if (subscribers != null)
            {
                foreach (Subscriber tmpSubscriber in subscribers)
                {
                    if (subscriber.Name == tmpSubscriber.Name)
                    {
                        isExist = true;
                        break;
                    }
                }
            }

			return isExist;
		}

		/// <summary>
		/// Add a subscriber for a class
		/// </summary>
		/// <param name="classElement">The class Element</param>
		/// <param name="subscriber">The Subscriber</param>
		public void AddSubscriber(ClassElement classElement, Subscriber subscriber)
		{
			SubscriberGroup subscriberGroup = (SubscriberGroup) this._subscriberGroups[classElement.Name];

            // Create a SubscriberGroup instance for the class element if it doesn't exist.
			if (subscriberGroup == null)
			{
				// class name is unique among the classes in a schema
				subscriberGroup = new SubscriberGroup(classElement.Name);
				_subscriberGroups.Add(subscriberGroup);
			}
			
			if (!IsSubscriberExist(classElement, subscriber))
			{
				subscriberGroup.Subscribers.Add(subscriber);

                _hasSubscribersTable.Remove(classElement.Name);
			}
		}

		/// <summary>
		/// Remove a subscriber from a class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="subscriber">The Subscriber to be removed</param>
		public void RemoveSubscriber(ClassElement classElement, Subscriber subscriber)
		{
            SubscriberGroup subscriberGroup = (SubscriberGroup)this._subscriberGroups[classElement.Name];

			if (subscriberGroup != null)
			{
				subscriberGroup.Subscribers.Remove(subscriber);

                _hasSubscribersTable.Remove(classElement.Name);
			}
		}

        /// <summary>
        /// Remove subscribers belongs to a class.
        /// </summary>
        /// <param name="classElement">The class element</param>
        public void RemoveSubscribers(ClassElement classElement)
        {
            SubscriberGroup subscriberGroup = (SubscriberGroup)this._subscriberGroups[classElement.Name];

            if (subscriberGroup != null)
            {
                this._subscriberGroups.Remove(subscriberGroup);

                _hasSubscribersTable.Remove(classElement.Name);
            }
        }

        /// <summary>
        /// Get information indicating whether an event is referenced by subscribers in any of
        /// in the collection.
        /// </summary>
        /// <param name="className">The name of attribute owner class</param>
        /// <param name="eventName">The name of the event to be found</param>
        /// <returns>true if the event is referenced, false otherwise.</returns>
        public bool IsEventReferenced(string className, string eventName, out string subscriberCaption)
        {
            bool status = false;
            subscriberCaption = null;

            SubscriberGroup subscriberGroup = (SubscriberGroup)this._subscriberGroups[className];

            if (subscriberGroup != null)
            {
                foreach (Subscriber subscriber in subscriberGroup.Subscribers)
                {
                    if (subscriber.IsEventReferenced(className, eventName))
                    {
                        subscriberCaption = subscriber.Name;
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
            SubscriberModelValidateVisitor visitor = new SubscriberModelValidateVisitor(metaData, userManager, result);

            Accept(visitor); // start validating

            return visitor.ValidateResult;
        }

        /// <summary>
        /// Accept a visitor of ISubscriberNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISubscriberNodeVisitor visitor)
        {
            if (visitor.VisitSubscriberManager(this))
            {
                this._subscriberGroups.Accept(visitor);
            }
        }

		/// <summary>
		/// Read subscribers from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
                throw new SubscriberException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read subscribers from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
					throw new SubscriberException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read subscribers from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
					throw new SubscriberException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write subscribers to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
				throw new SubscriberException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write subscribers as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
				throw new SubscriberException("Failed to write the subscribers", ex);
			}
		}

		/// <summary>
		/// Write subscribers as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="SubscriberException">SubscriberException is thrown when it fails to
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
				throw new SubscriberException("Failed to write the subscribers", ex);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of SubscriberNodeType values</value>
        public override SubscriberNodeType NodeType 
		{
			get
			{
				return SubscriberNodeType.SubscriberManager;
			}
		}

		/// <summary>
		/// create subscribers from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of SubscriberGroup instances
            _subscriberGroups = (SubscriberGroupCollection)SubscriberNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subscriberGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write subscribers to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the subscriber defs
            XmlElement child = parent.OwnerDocument.CreateElement(SubscriberNodeFactory.ConvertTypeToString(_subscriberGroups.NodeType));
			_subscriberGroups.Marshal(child);
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

			XmlElement element = doc.CreateElement("SubscriberManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the SubscriberManager changed
		/// </summary>
		/// <param name="sender">the IEventNode that cause the subscriber</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}
	}
}