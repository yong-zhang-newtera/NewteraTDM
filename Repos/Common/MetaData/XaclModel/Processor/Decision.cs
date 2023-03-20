/*
* @(#)Decision.cs
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
	/// The class represents a decision as result of evaluating a single XaclRule
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class Decision
	{
		// the rule that results the decision
		private XaclRule _rule;
		
		// the permission
		private XaclPermissionType _permission;
		
		/// <summary>
		/// Initiate an instance of Decision class.
		/// </summary>
		/// <param name="rule"> The rule of a decision.</param>
		/// <param name="permission">One of XaclPermissionType</param>
		public Decision(XaclRule rule, XaclPermissionType permission)
		{
			_rule = rule;
			_permission = permission;
		}

		/// <summary>
		/// Gets permission of a decision
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType Permission
		{
			get
			{
				return _permission;
			}
		}

		/// <summary>
		/// Gets the rule of a decision.
		/// </summary>
		/// <value> An XaclRule object</value>
		public XaclRule Rule
		{
			get
			{
				return _rule;
			}
		}
	}
}