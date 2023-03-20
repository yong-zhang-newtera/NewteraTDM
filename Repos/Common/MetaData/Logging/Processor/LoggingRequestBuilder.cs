/*
* @(#)LoggingRequestBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.Logging;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// This singleton class provide some convenient methods to build a logging request.
	/// </summary>
	/// <version>  	1.0.0 05 Jan 2009 </version>
	internal class LoggingRequestBuilder
	{		
		/// <summary> Singleton's private instance.
		/// </summary>
		private static LoggingRequestBuilder theLoggingRequestBuilder;
		
		static LoggingRequestBuilder()
		{
			// Initializing the access request builder.
		    {
			    theLoggingRequestBuilder = new LoggingRequestBuilder();
		    }
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private LoggingRequestBuilder()
		{
		}
		
		/// <summary>
		/// Gets the LoggingRequestBuilder instance.
		/// </summary>
		/// <returns> The LoggingRequestBuilder instance.</returns>
		static public LoggingRequestBuilder Instance
		{
			get
			{
				return theLoggingRequestBuilder;
			}
		}

		/// <summary>
		/// Build a logging request to an object of IXaclObject type.
		/// </summary>
		/// <param name="loggingObject">the object to which a log is requested.</param>
		/// <param name="actionType">One of the LoggingActionType enum values</param>
		/// <returns>A created LoggingRequest.</returns>
		public LoggingRequest BuildLoggingRequest(IXaclObject loggingObject, LoggingActionType actionType)
		{
			LoggingRequest request = new LoggingRequest();
			
			// Build the object of Access Request
			request.Object = BuildObject(loggingObject);
			
			request.ActionType = actionType;
			
			// Build subject part of Access Request
			request.Subject = BuildSubject();
			
			return request;
		}
		
		/// <summary>
		/// Build an LoggingObject based on an IXaclObject.
		/// </summary>
		/// <param name="loggingObject">the object be accessed.</param>
		private LoggingObject BuildObject(IXaclObject loggingObject)
		{
			string href = loggingObject.ToXPath();
			
			LoggingObject obj = new LoggingObject(href);
			
			return obj;
		}
		
		/// <summary>
		/// Gets the LoggingSubject instance for current user.
		/// </summary>
		/// <returns> the LoggingSubject instance for current user
		/// </returns>
		private LoggingSubject BuildSubject()
		{
			// get the CustomPrincipal object from the thread
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

			// create the LoggingSubject instance for current user.
			LoggingSubject subject = new LoggingSubject();

			if (principal != null)
			{
				subject.Uid = principal.Identity.Name;
				subject.Roles = principal.Roles;
			}
			else
			{
				throw new LoggingException("Unauthorized access.");
			}
			
			return subject;
		}
	}
}