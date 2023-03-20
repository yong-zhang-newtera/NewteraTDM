/*
* @(#)Conclusion.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;
	using System.Collections;

	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The class represents the result for evaluation of an access request
	/// </summary>
	/// <version> 1.0.0 10 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class Conclusion
	{
		private XaclPermissionType _permission;
		private ArrayList _decisionList;
		
		/// <summary> 
		/// Initiate an instance of Conclusion class
		/// </summary>
		public Conclusion()
		{
			_permission = XaclPermissionType.Unknown;
			_decisionList = null;
		}
		
		/// <summary>
		/// Gets or sets the resulting permission
		/// </summary>
		/// <value> One of the XaclPermissionType</value>
		public XaclPermissionType Permission
		{
			get
			{
				return _permission;
			}
			set
			{
				_permission = value;
			}
		}

		/// <summary>
		/// Gets or sets the decision list.
		/// </summary>
		/// <value> A list of Decision objects.</value>
		public ArrayList DecisionList
		{
			get
			{
				return _decisionList;
			}
			set
			{
				_decisionList = value;
			}
		}
	}
}