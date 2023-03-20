/*
* @(#)AccessRequest.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The class represents an access request by a client. An AccessRequest includes
	/// an XaclObject, XaclSubject. and an XaclActionType value.
	/// </summary>
	/// <version>  	1.0.0 11 Dec 2003 </version>
	/// <author>  Yong Zhang </author>
	internal class AccessRequest
	{
		// the object of the access request
		private XaclObject _object;
		
		// the subject element of the access request
		private XaclSubject _subject;
		
		// the action type of the access request
		private XaclActionType _actionType;
		
		/// <summary>
		/// Initiating an instance of AccessRequest class.
		/// </summary>
		public AccessRequest()
		{
			_object = null;
			_subject = null;
			_actionType = XaclActionType.Unknown;
		}
		
		/// <summary>
		/// Initiating an instance of AccessRequest class.
		/// </summary>
		/// <param name="subject"> The subject of the access request. </param>
		/// <param name="obj">  The object of the access request. </param>
		/// <param name="actionType"> The type of action. </param>
		public AccessRequest(XaclSubject subject, XaclObject obj, XaclActionType actionType)
		{
			_subject = subject;
			_object = obj;
			_actionType = actionType;
		}
		
		/// <summary>
		/// Gets or sets the XaclObject of the access request.
		/// </summary>
		/// <value>An XaclObject.</value>
		public XaclObject Object
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
		/// Gets or sets the XaclSubject of the access request.
		/// </summary>
		/// <value>XaclSubject object.</value>
		public XaclSubject Subject
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
		/// Gets or sets action type of the access request.
		/// </summary>
		/// <value> One of XaclActionType enum values.</value>
		public XaclActionType ActionType
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