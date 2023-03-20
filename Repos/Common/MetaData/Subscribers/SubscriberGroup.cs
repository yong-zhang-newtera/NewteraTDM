/*
* @(#)SubscriberGroup.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represent a group of subscribers that are associated with a certain class.
	/// </summary>
	/// <version>  1.0.0 16 Sept 2013</version>
	public class SubscriberGroup : SubscriberNodeBase
	{
		private string _className;
		
		private SubscriberCollection _subscribers;
		
		/// <summary>
		/// Initiate an instance of a SubscriberGroup class.
		/// </summary>
        public SubscriberGroup(string className) : base(className)
		{
			_className = className;
			_subscribers = new SubscriberCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subscribers.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of SubscriberGroup class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SubscriberGroup(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets name of the class associated with a SubscriberGroup
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// Gets the subscribers contained in a SubscriberGroup
		/// </summary>
		public SubscriberCollection Subscribers
		{
			get
			{
				return _subscribers;
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
				return SubscriberNodeType.SubscriberGroup;
			}
		}

        /// <summary>
        /// Accept a visitor of ISubscriberNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISubscriberNodeVisitor visitor)
        {
            if (visitor.VisitSubscriberGroup(this))
            {
                this._subscribers.Accept(visitor);
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
            _subscribers = (SubscriberCollection)SubscriberNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subscribers.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write SubscriberGroup instance to xml document
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
            XmlElement child = parent.OwnerDocument.CreateElement(SubscriberNodeFactory.ConvertTypeToString(_subscribers.NodeType));
			_subscribers.Marshal(child);
			parent.AppendChild(child);
		}
	}
}