/*
* @(#)LoggingRule.cs
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
	/// The class represents a logging rule in an LoggingDef that includes
	/// an LoggingSubject, A set of LoggingAction.
	/// </summary>
	/// <version>1.0.0 04 Jan 2009</version>
	public class LoggingRule : LoggingNodeBase
	{
		private LoggingSubject _subject;
		
		private LoggingActionCollection _actions;
		
		private bool _allowPropagation;

		private bool _isOverrided; // whether to override inherited rule

		private string _href; // Run-time use only, do not write to xml
		
		/// <summary>
		/// Initiate an instance of LoggingRule class.
		/// </summary>
		/// <param name="subject"> LoggingSubject object </param>
		public LoggingRule(LoggingSubject subject) : this(subject, null)
		{
		}

		/// <summary>
		/// Initiate an instance of LoggingRule class.
		/// </summary>
		/// <param name="subject"> LoggingSubject object </param>
		/// <param name="href">The href of the rule</param>
		public LoggingRule(LoggingSubject subject, string href) : base()
		{
			_subject = subject;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subject.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_actions = CreateDefaultActions();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _actions.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_allowPropagation = true;
			_isOverrided = false;
			_href = href;
		}

		/// <summary>
		/// Initiating an instance of LoggingRule class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingRule(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>An LoggingActionCollection instance.</value>
		public LoggingActionCollection Actions
		{
			get
			{
				return _actions;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a read log is turn on.
		/// </summary>
		/// <value>true if the read log turned on, false if off</value>
		public bool IsReadLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Read) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Read, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Read, LoggingStatus.Off);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a read log is off.
		/// </summary>
		/// <value>true if the read log is off, false if on</value>
		public bool IsReadLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Read) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
                    SetLoggingStatus(LoggingActionType.Read, LoggingStatus.Off);
				}
				else
				{
					SetLoggingStatus(LoggingActionType.Read, LoggingStatus.On);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a write log is on.
		/// </summary>
		/// <value>true if the write log is on, false if off</value>
		public bool IsWriteLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Write) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Write, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Write, LoggingStatus.Off);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a write log is off.
		/// </summary>
		/// <value>true if the write log is off, false if on</value>
		public bool IsWriteLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Write) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Write, LoggingStatus.Off);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Write, LoggingStatus.On);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a create log is on</summary>
		/// <value>true if the create log is on, false if off</value>
		public bool IsCreateLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Create) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Create, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Create, LoggingStatus.Off);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a create log is off.
		/// </summary>
		/// <value>true if the create log is off, false if on</value>
		public bool IsCreateLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Create) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Create, LoggingStatus.Off);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Create, LoggingStatus.On);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a delete log is on.
		/// </summary>
		/// <value>true if the delete log is on, false if off</value>
		public bool IsDeleteLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Delete) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Delete, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Delete, LoggingStatus.Off);
                }
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a delete log is off.
		/// </summary>
		/// <value>true if the delete log is off, false if on</value>
		public bool IsDeleteLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Delete) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Delete, LoggingStatus.Off);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Delete, LoggingStatus.On);
				}
			}
		}


		/// <summary>
		/// Gets or sets the information indicating whether a upload log is on.
		/// </summary>
		/// <value>true if the upload log is on, false if off</value>
		public bool IsUploadLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Upload) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Upload, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Upload, LoggingStatus.Off);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a upload log is off.
		/// </summary>
		/// <value>true if the upload log is off, false if on</value>
		public bool IsUploadLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Upload) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Upload, LoggingStatus.Off);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Upload, LoggingStatus.On);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a download log is on.
		/// </summary>
		/// <value>true if the download log is on, false if off</value>
		public bool IsDownloadLogOn
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Download) == LoggingStatus.On)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Download, LoggingStatus.On);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Download, LoggingStatus.Off);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a download log is off.
		/// </summary>
		/// <value>true if the download log is off, false if on</value>
		public bool IsDownloadLogOff
		{
			get
			{
				if (GetLoggingStatus(LoggingActionType.Download) == LoggingStatus.Off)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetLoggingStatus(LoggingActionType.Download, LoggingStatus.Off);
				}
				else
				{
                    SetLoggingStatus(LoggingActionType.Download, LoggingStatus.On);
				}
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether an import log is on.
        /// </summary>
        /// <value>true if the import log is on, false if off</value>
        public bool IsImportLogOn
        {
            get
            {
                if (GetLoggingStatus(LoggingActionType.Import) == LoggingStatus.On)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    SetLoggingStatus(LoggingActionType.Import, LoggingStatus.On);
                }
                else
                {
                    SetLoggingStatus(LoggingActionType.Import, LoggingStatus.Off);
                }
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether an import log is off.
        /// </summary>
        /// <value>true if the import log is off, false if on</value>
        public bool IsImportLogOff
        {
            get
            {
                if (GetLoggingStatus(LoggingActionType.Import) == LoggingStatus.Off)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    SetLoggingStatus(LoggingActionType.Import, LoggingStatus.Off);
                }
                else
                {
                    SetLoggingStatus(LoggingActionType.Import, LoggingStatus.On);
                }
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether an export log is on.
        /// </summary>
        /// <value>true if the export log is on, false if off</value>
        public bool IsExportLogOn
        {
            get
            {
                if (GetLoggingStatus(LoggingActionType.Export) == LoggingStatus.On)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    SetLoggingStatus(LoggingActionType.Export, LoggingStatus.On);
                }
                else
                {
                    SetLoggingStatus(LoggingActionType.Export, LoggingStatus.Off);
                }
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether an export log is off.
        /// </summary>
        /// <value>true if the export log is off, false if on</value>
        public bool IsExportLogOff
        {
            get
            {
                if (GetLoggingStatus(LoggingActionType.Export) == LoggingStatus.Off)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    SetLoggingStatus(LoggingActionType.Export, LoggingStatus.Off);
                }
                else
                {
                    SetLoggingStatus(LoggingActionType.Export, LoggingStatus.On);
                }
            }
        }

		/// <summary>
		/// Gets the LoggingSubject of The LoggingRule. 
		/// </summary>
		public LoggingSubject Subject
		{
			get
			{
				return _subject;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the rule is allowed to
		/// propagate
		/// </summary>
		/// <value>true if it is allowed to propagate, false otherwise</value>
		public bool AllowPropagation
		{
			get
			{
				return _allowPropagation;
			}
			set
			{
				_allowPropagation = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the rule overrides the
		/// inherited rule with the same subject
		/// </summary>
		/// <value>true if it overrides the inherited rule, false otherwise. Default is false.</value>
		public bool IsOverrided
		{
			get
			{
				return _isOverrided;
			}
			set
			{
				_isOverrided = value;
				FireValueChangedEvent(value);
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
				return NodeType.Rule;
			}
		}

		/// <summary>
		/// Gets the logging status of the rule for an action type
		/// </summary>
		/// <param name="actionType">One of LoggingActionType values</param>
		/// <returns>One of LoggingStatus values</returns>
		public LoggingStatus GetLoggingStatus(LoggingActionType actionType)
		{
			LoggingStatus status = LoggingStatus.Unknown;

			foreach (LoggingAction action in _actions)
			{
				if (action.ActionType == actionType)
				{
					status = action.Status;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Sets the status of the rule for an action type
		/// </summary>
		/// <param name="actionType">One of LoggingActionType values</param>
		/// <param name="status">One of LoggingStatus</param>
		public void SetLoggingStatus(LoggingActionType actionType, LoggingStatus status)
		{
			foreach (LoggingAction action in _actions)
			{
				if (action.ActionType == actionType)
				{
					action.Status = status;
					break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the href of the LoggingObject to which the rule is
		/// associated with.
		/// </summary>
		/// <value>A href string</value>
		public string ObjectHref
		{
			get
			{
				return _href;
			}
			set
			{
				_href = value;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            if (visitor.VisitLoggingRule(this))
            {
                _subject.Accept(visitor);
                _actions.Accept(visitor);
            }
        }

		/// <summary>
		/// create an LoggingRule from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("propagate");
			if (str != null && str == "false")
			{
				_allowPropagation = false;
			}
			else
			{
				_allowPropagation = true;
			}

			str = parent.GetAttribute("override");
			if (str != null && str == "true")
			{
				_isOverrided = true;
			}
			else
			{
				_isOverrided = false; // default
			}

			// the first child is LoggingSubject
			_subject = (LoggingSubject) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subject.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of LoggingAction instances
			_actions = (LoggingActionCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _actions.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write LoggingRule to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (!_allowPropagation)
			{
				parent.SetAttribute("propagate", "false");
			}

			if (_isOverrided)
			{
				parent.SetAttribute("override", "true");
			}

			// write the _subject
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_subject.NodeType));
			_subject.Marshal(child);
			parent.AppendChild(child);

			// write the actions
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_actions.NodeType));
			_actions.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Create default actions for read, write, create, delete, upload, and download.
		/// </summary>
		/// <returns>A collection of LoggingAction</returns>
		private LoggingActionCollection CreateDefaultActions()
		{
			LoggingActionCollection actions = new LoggingActionCollection();
			LoggingAction action;

			action = new LoggingAction(LoggingActionType.Read, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Write, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Create, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Delete, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Upload, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Download, LoggingStatus.Off);
			actions.Add(action);

            action = new LoggingAction(LoggingActionType.Import, LoggingStatus.Off);
            actions.Add(action);

            action = new LoggingAction(LoggingActionType.Export, LoggingStatus.Off);
            actions.Add(action);

			return actions;
		}
	}
}