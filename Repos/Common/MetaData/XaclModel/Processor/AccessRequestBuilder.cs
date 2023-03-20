/*
* @(#)AccessRequestBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// This singleton class provide some convenient methods to build a access request.
	/// </summary>
	/// <version>  	1.0.0 26 Nov 2003 </version>
	/// <author>  		Yong Zhang </author>
	internal class AccessRequestBuilder
	{		
		/// <summary> Singleton's private instance.
		/// </summary>
		private static AccessRequestBuilder theAccessRequestBuilder;
		
		static AccessRequestBuilder()
		{
			// Initializing the access request builder.
		{
			theAccessRequestBuilder = new AccessRequestBuilder();
		}
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private AccessRequestBuilder()
		{
		}
		
		/// <summary>
		/// Gets the AccessRequestBuilder instance.
		/// </summary>
		/// <returns> The AccessRequestBuilder instance.</returns>
		static public AccessRequestBuilder Instance
		{
			get
			{
				return theAccessRequestBuilder;
			}
		}

		/// <summary>
		/// Build an access request for accessing an object of IXaclObject type.
		/// </summary>
		/// <param name="xaclObject">the object be accessed.</param>
		/// <param name="actionType">One of the XaclActionType enum values</param>
		/// <returns>A created AccessRequest.</returns>
		public AccessRequest BuildAccessRequest(IXaclObject xaclObject, XaclActionType actionType)
		{
			AccessRequest request = new AccessRequest();
			
			// Build the object of Access Request
			request.Object = BuildObject(xaclObject);
			
			request.ActionType = actionType;
			
			// Build subject part of Access Request
			request.Subject = BuildSubject();
			
			return request;
		}
		
		/// <summary>
		/// Build an XaclObject based on an IXaclObject.
		/// </summary>
		/// <param name="xaclObject">the object be accessed.</param>
		private XaclObject BuildObject(IXaclObject xaclObject)
		{
			string href = xaclObject.ToXPath();
			
			XaclObject obj = new XaclObject(href);
			
			return obj;
		}
		
		/// <summary>
		/// Gets the XaclSubject instance for current user.
		/// </summary>
		/// <returns> the XaclSubject instance for current user.
		/// 
		/// </returns>
		private XaclSubject BuildSubject()
		{
			// get the CustomPrincipal object from the thread
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

			// create the XaclSubject instance for current user.
			XaclSubject subject = new XaclSubject();

			if (principal != null)
			{
				subject.Uid = principal.Identity.Name;
				subject.Roles = principal.Roles;
			}
			else
			{
				throw new XaclException("Unauthorized access.");
			}
			
			return subject;
		}
	}
}