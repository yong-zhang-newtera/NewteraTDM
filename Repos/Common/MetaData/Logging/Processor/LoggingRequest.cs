/*
* @(#)LoggingRequest.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Logging;

	/// <summary>
	/// The class represents an logging request by a client. An LoggingRequest includes
	/// a LoggingObject, LoggingSubject. and a LoggingActionType value.
	/// </summary>
	/// <version>  	1.0.0 04 Jan 2009 </version>
	internal class LoggingRequest
	{
		// the logging object
		private LoggingObject _object;
		
		// the logging subject
		private LoggingSubject _subject;
		
		// the logging action type
		private LoggingActionType _actionType;
		
		/// <summary>
		/// Initiating an instance of LoggingRequest class.
		/// </summary>
		public LoggingRequest()
		{
			_object = null;
			_subject = null;
			_actionType = LoggingActionType.Unknown;
		}
		
		/// <summary>
		/// Initiating an instance of LoggingRequest class.
		/// </summary>
		/// <param name="subject"> The subject of the access request. </param>
		/// <param name="obj">  The object of the access request. </param>
		/// <param name="actionType"> The type of action. </param>
		public LoggingRequest(LoggingSubject subject, LoggingObject obj, LoggingActionType actionType)
		{
			_subject = subject;
			_object = obj;
			_actionType = actionType;
		}
		
		/// <summary>
		/// Gets or sets the LoggingObject of the request.
		/// </summary>
		/// <value>An LoggingObject.</value>
		public LoggingObject Object
		{
			get
			{
				return _object;
			}
			
			set
			{
				_object = value;
			}
		}

		/// <summary>
		/// Gets or sets the LoggingSubject object of the request.
		/// </summary>
		/// <value>LoggingSubject object.</value>
		public LoggingSubject Subject
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

		/// <summary>
		/// Gets or sets logging action type of the request.
		/// </summary>
		/// <value> One of LoggingActionType enum values.</value>
		public LoggingActionType ActionType
		{
			get
			{
				return _actionType;
			}
			set
			{
				_actionType = value;
			}
		}
	}
}