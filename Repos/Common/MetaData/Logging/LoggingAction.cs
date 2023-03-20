/*
* @(#)LoggingAction.cs
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
	/// The class represents an action in an LoggingRule object.
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	public class LoggingAction : LoggingNodeBase
	{
		private LoggingActionType _actionType = LoggingActionType.Unknown;
		
		private LoggingStatus _status = LoggingStatus.Unknown;
		
		/// <summary>
		/// Constructor  with parameters.
		/// </summary>
		/// <param name="actionType">action type</param>
		/// <param name="loggingStatus"> LoggingStatus </param>
		public LoggingAction(LoggingActionType actionType, LoggingStatus loggingStatus): base()
		{
			_actionType = actionType;
			_status = loggingStatus;
		}

		/// <summary>
		/// Initiating an instance of LoggingAction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingAction(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the value of logging status of the action element.
		/// </summary>
		public LoggingStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the action type.
		/// </summary>
		/// <value>One of the LoggingActionType values.</value>
		public virtual LoggingActionType ActionType
		{
			get
			{
				return _actionType;
			}
			set
			{
				_actionType = value;
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
				return NodeType.Action;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            visitor.VisitLoggingAction(this);
        }

		/// <summary>
		/// create an LoggingAction from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_actionType = (LoggingActionType) Enum.Parse(typeof(LoggingActionType), parent.GetAttribute("type"));
			_status = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("status"));
		}

		/// <summary>
		/// write LoggingAction to xml document
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			parent.SetAttribute("type", Enum.GetName(typeof(LoggingActionType), _actionType));
			parent.SetAttribute("status", Enum.GetName(typeof(LoggingStatus), _status));
		}
	}
}