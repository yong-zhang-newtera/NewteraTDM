/*
* @(#)Subscriber.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
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
	/// The class represents definition of a subscriber for a class.
	/// </summary>
	/// <version>1.0.0 16 Sept 2013</version>
	public partial class Subscriber : SubscriberNodeBase
	{
		private string _className;
        private string _eventName;
        private string _subject;
        private string _description;
        private StringCollection _users;
        private StringCollection _roles;
        private string _userBindingAttributeName;
        private string _senderBindingAttributeName;
        private string _url;
        private string _params;
        private bool _sendEmail;
        private bool _sendMessage;
        private string _inlineHandler;
        private string _externalHandler;

        private MetaDataModel _metaData = null; // run-time value
		
		/// <summary>
		/// Initiate an instance of Subscriber class.
		/// </summary>
		public Subscriber() : base()
		{
			_className = null;
            _eventName = null;
            _subject = null;
            _description = null;
            _users = new StringCollection();
            _roles = new StringCollection();
            _userBindingAttributeName = null;
            _sendEmail = false;
            _sendMessage = false;
            _inlineHandler = null;
            _senderBindingAttributeName = null;
            _url = null;
            _params = null;
            _externalHandler = null;
           
            // set default name
            this.Name = "Subscriber1";
		}

		/// <summary>
		/// Initiating an instance of Subscriber class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Subscriber(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets name of the class that this rule is associated with.
        /// </summary>
        /// <value> The unique class name.</value>
        [
            CategoryAttribute("EventSource"),
            DescriptionAttribute("Specify a event to subscribe to."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.EventNamePropertyEditor, Studio", typeof(UITypeEditor))
        ]
        public string EventName
        {
            get
            {
                return _eventName;
            }
            set
            {
                _eventName = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The Subject property is used to specify the subject of the notification.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The description property is used to specify the description of the notification.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Description
        {

            get
            {
                return _description;
            }
            set
            {

                _description = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The url property is used to specify url to view the detail of notification.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Url
        {

            get
            {
                return _url;
            }
            set
            {

                _url = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The params property is used to specify parameters of the url to view the detail of notification.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Params
        {

            get
            {
                return _params;
            }
            set
            {

                _params = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The users property is used to specify users who are supposed to get notification")]
        [EditorAttribute("Newtera.Studio.UsersPropertyEditor, Studio", typeof(UITypeEditor))]
        public StringCollection Users
        {

            get
            {
                return _users;
            }
            set
            {

                _users = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("The roles property is used to specify roles of the users who are getting the notification. Only those users who belong to all the specified roles will be assigned.")]
        [EditorAttribute("Newtera.Studio.RolesPropertyEditor, Studio", typeof(UITypeEditor))]
        public StringCollection Roles
        {

            get
            {
                return _roles;
            }
            set
            {

                _roles = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("Specify an attribute whose value represents the users who are getting the notification.")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("Newtera.Studio.AttributeNameConverter, Studio")]
        [EditorAttribute("Newtera.Studio.AttributeNamePropertyEditor, Studio", typeof(UITypeEditor))]
        public string UsersBindingAttribute
        {
            get
            {
                return _userBindingAttributeName;
            }
            set
            {
                _userBindingAttributeName = value;
            }
        }

        [CategoryAttribute("Notification")]
        [DescriptionAttribute("Specify an attribute whose value represents the sender of the notification.")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("Newtera.Studio.AttributeNameConverter, Studio")]
        [EditorAttribute("Newtera.Studio.AttributeNamePropertyEditor, Studio", typeof(UITypeEditor))]
        public string SenderBindingAttribute
        {
            get
            {
                return _senderBindingAttributeName;
            }
            set
            {
                _senderBindingAttributeName = value;
            }
        }

        [CategoryAttribute("Notification")]
        [Description("The SendEmail property is used to specify whether an email is sent to the assigned users")]
        [DefaultValue(false)]
        public bool SendEmail
        {
            get
            {
                return _sendEmail;
            }
            set
            {
                _sendEmail = value;
            }
        }

        [CategoryAttribute("Notification")]
        [Description("The SendMessage property is used to specify whether a message is sent to the assigned users")]
        [DefaultValue(false)]
        public bool SendMessage
        {
            get
            {
                return _sendMessage;
            }
            set
            {
                _sendMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the inline code of event handler.
        /// </summary>
        /// <value>The inline code in C#</value>
        [
            CategoryAttribute("Handler"),
            DescriptionAttribute("The inline handler that is called when the event arises"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        public string InlineHandler
        {
            get
            {
                return _inlineHandler;
            }
            set
            {
                if (value != null)
                {
                    _inlineHandler = value;
                }
                else
                {
                    _inlineHandler = "";
                }
            }
        }

        /// <summary>
        /// Gets or sets the external event handler in form of "NameSpace.ClassName,LibName"
        /// </summary>
        [
            CategoryAttribute("Handler"),
            DescriptionAttribute("The external event handler for the event"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string ExternalHanlder
        {
            get
            {
                return _externalHandler;
            }
            set
            {
                _externalHandler = value;
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
        /// <value>One of SubscriberNodeType values</value>
        [BrowsableAttribute(false)]		
        public override SubscriberNodeType NodeType
		{
			get
			{
				return SubscriberNodeType.Subscriber;
			}
		}

        /// <summary>
        /// Get information indicating whether an event is referenced by the subscriber definition.
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="eventName">The event name</param>
        /// <returns>true if the event is referenced by the subscriber, false otherwise.</returns>
        public bool IsEventReferenced(string className, string eventName)
        {
            bool status = false;

            return status;
        }

        /// <summary>
        /// Accept a visitor of ISubscriberNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISubscriberNodeVisitor visitor)
        {
            visitor.VisitSubscriber(this);
        }

		/// <summary>
		/// create an Subscriber from a xml document.
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

            str = parent.GetAttribute("eventName");
            if (!string.IsNullOrEmpty(str))
            {
                _eventName = str;
            }
            else
            {
                _eventName = null;
            }

            str = parent.GetAttribute("subject");
            if (!string.IsNullOrEmpty(str))
            {
                _subject = str;
            }
            else
            {
                _subject = null;
            }

            str = parent.GetAttribute("desc");
            if (!string.IsNullOrEmpty(str))
            {
                _description = str;
            }
            else
            {
                _description = null;
            }

            str = parent.GetAttribute("url");
            if (!string.IsNullOrEmpty(str))
            {
                _url = str;
            }
            else
            {
                _url = null;
            }

            str = parent.GetAttribute("params");
            if (!string.IsNullOrEmpty(str))
            {
                _params = str;
            }
            else
            {
                _params = null;
            }

            str = parent.GetAttribute("users");
            if (!string.IsNullOrEmpty(str))
            {
                _users = new StringCollection();

                string[] userArray = str.Split(';');
                foreach (string userName in userArray)
                {
                    _users.Add(userName);
                }
            }
            else
            {
                _users = new StringCollection();
            }

            str = parent.GetAttribute("roles");
            if (!string.IsNullOrEmpty(str))
            {
                _roles = new StringCollection();

                string[] roleArray = str.Split(';');
                foreach (string role in roleArray)
                {
                    _roles.Add(role);
                }
            }
            else
            {
                _roles = new StringCollection();
            }

            str = parent.GetAttribute("userAttribute");
            if (!string.IsNullOrEmpty(str))
            {
                _userBindingAttributeName = str;
            }
            else
            {
                _userBindingAttributeName = null;
            }

            str = parent.GetAttribute("senderAttribute");
            if (!string.IsNullOrEmpty(str))
            {
                _senderBindingAttributeName = str;
            }
            else
            {
                _senderBindingAttributeName = null;
            }

            str = parent.GetAttribute("sendEmail");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _sendEmail = true;
            }
            else
            {
                _sendEmail = false;
            }

            str = parent.GetAttribute("sendMessage");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _sendMessage = true;
            }
            else
            {
                _sendMessage = false;
            }

            str = parent.InnerText;

            if (str != null && str.Length > 0)
            {
                // hack, replace \n with \r\n. For some reason \r\n in the script
                // was changed to \n when saved to database
                if (str.IndexOf("\r\n") < 0)
                {
                    // do it once
                    _inlineHandler = str.Replace("\n", "\r\n");
                }
                else
                {
                    _inlineHandler = str;
                }
            }
            else
            {
                _inlineHandler = null;
            }

            str = parent.GetAttribute("externalHandler");
            if (!string.IsNullOrEmpty(str))
            {
                _externalHandler = str;
            }
            else
            {
                _externalHandler = null;
            }
		}

		/// <summary>
		/// write Subscriber to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}

            if (!string.IsNullOrEmpty(_eventName))
            {
                parent.SetAttribute("eventName", _eventName);
            }

            if (!string.IsNullOrEmpty(_subject))
            {
                parent.SetAttribute("subject", _subject);
            }

            if (!string.IsNullOrEmpty(_description))
            {
                parent.SetAttribute("desc", _description);
            }

            if (!string.IsNullOrEmpty(_url))
            {
                parent.SetAttribute("url", _url);
            }

            if (!string.IsNullOrEmpty(_params))
            {
                parent.SetAttribute("params", _params);
            }

            if (_users != null && _users.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (string userName in _users)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(";");
                    }

                    builder.Append(userName);
                }

                parent.SetAttribute("users", builder.ToString());
            }

            if (_roles != null && _roles.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (string role in _roles)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(";");
                    }

                    builder.Append(role);
                }

                parent.SetAttribute("roles", builder.ToString());
            }

            if (!string.IsNullOrEmpty(_userBindingAttributeName))
            {
                parent.SetAttribute("userAttribute", _userBindingAttributeName);
            }

            if (!string.IsNullOrEmpty(_senderBindingAttributeName))
            {
                parent.SetAttribute("senderAttribute", _senderBindingAttributeName);
            }

            if (_sendEmail)
            {
                parent.SetAttribute("sendEmail", "true");
            }

            if (_sendMessage)
            {
                parent.SetAttribute("sendMessage", "true");
            }

            if (!string.IsNullOrEmpty(_inlineHandler))
            {
                parent.InnerText = _inlineHandler;
            }

            if (!string.IsNullOrEmpty(_externalHandler))
            {
                parent.SetAttribute("externalHandler", _externalHandler);
            }
		}
	}
}