/*
* @(#)LoggingDef.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represent an logging definition for an LoggingObject. It contains an LoggingObject
	/// and a set of logging rules applied to the object.
	/// </summary>
	/// <version>  1.0.0 04 Jan 2009</version>
	public class LoggingDef : LoggingNodeBase
	{
		// the logging object
		private LoggingObject _object;
		
		private LoggingRuleCollection _rules;

        private bool _obsolete = false; // run-time use only
		
		/// <summary>
		/// Initiate an instance of a LoggingDef class.
		/// </summary>
		public LoggingDef(LoggingObject obj) : base()
		{
			_object = obj;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _object.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_rules = new LoggingRuleCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _rules.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of DataClass class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets the LoggingObject associated with a LoggingDef
		/// </summary>
		public LoggingObject Object
		{
			get
			{
				return _object;
			}
		}

		/// <summary>
		/// Gets the rules associated with a LoggingDef
		/// </summary>
		public LoggingRuleCollection Rules
		{
			get
			{
				return _rules;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.Definition;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the def is obsolete or not
        /// </summary>
        public bool IsObsolete
        {
            get
            {
                return _obsolete;
            }
            set
            {
                _obsolete = value;
            }
        }

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            if (visitor.VisitLoggingDef(this))
            {
                _object.Accept(visitor);
                _rules.Accept(visitor);
            }
        }

		/// <summary>
		/// create an xacl definition from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// the first child is for xacl object
			_object = (LoggingObject) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _object.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of  acl rules
			_rules = (LoggingRuleCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _rules.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write an xacl definition to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _baseClass
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_object.NodeType));
			_object.Marshal(child);
			parent.AppendChild(child);

			// write the rules
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_rules.NodeType));
			_rules.Marshal(child);
			parent.AppendChild(child);
		}
	}
}