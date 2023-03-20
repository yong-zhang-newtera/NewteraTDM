/*
* @(#)XaclProcessor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// The XaclProcessor that evaluates a access request against a xacl policy and
	/// return a conclusion to the client
	/// </summary>
	/// <version>1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	internal sealed class XaclProcessor
	{
		private RuleMatcher _ruleMatcher;

		/// <summary>
		/// Initiatiate an instance of XaclProcessor class.
		/// </summary>
		public XaclProcessor()
		{
			_ruleMatcher = new RuleMatcher();
		}
		
		/// <summary>
		/// Gets or sets the runner object that is responsible for running the 
		/// rules' conditions expressed in xquery
		/// </summary>
		/// <value>An IConditionRunner</value>
		public IConditionRunner ConditionRunner
		{
			get
			{
				return _ruleMatcher.ConditionRunner;
			}
			set
			{
				_ruleMatcher.ConditionRunner = value;
			}
		}
		
		/// <summary>
		/// Evaluate an access request against a xacl policy and return a
		/// Conclusion.
		/// </summary>
		/// <param name="accessReq">The access request.</param>
		/// <param name="policy">The access control policy.</param>
		/// <returns>A Conclusion object.</returns>
		public Conclusion Evaluate(AccessRequest accessReq, XaclPolicy policy)
		{
			return Evaluate(accessReq, policy, null);
		}

		/// <summary>
		/// Evaluate an access request against a xacl policy and return a
		/// Conclusion.
		/// </summary>
		/// <param name="accessReq">The access request.</param>
		/// <param name="policy">The access control policy.</param>
		/// <param name="currentInstance">the current xml instance as a context for condition evaluation.</param>
		/// <returns>A Conclusion object.</returns>
		public Conclusion Evaluate(AccessRequest accessReq, XaclPolicy policy, XmlElement currentInstance)
		{
			CustomPrincipal principal = null;
			bool isCheckCondition = false;

			try 
			{
				Conclusion conclusion = new Conclusion();

				if (IsSuperUser(accessReq))
				{
                    // Grant permission to the Super User anyway
					conclusion.Permission = XaclPermissionType.Grant;
					return conclusion;
				}
				
				// set the current instance as a context for condition evaluation
				if (currentInstance != null)
				{
					principal = (CustomPrincipal) Thread.CurrentPrincipal;
					principal.CurrentInstance = currentInstance;
					isCheckCondition = true;
				}

				// Get the decisions from the rules that matche the access request.
				ArrayList decisions = _ruleMatcher.Match(accessReq, policy, isCheckCondition);
				conclusion.DecisionList = decisions;
				
				if (decisions.Count > 0)
				{
					conclusion = GetConclusion(decisions, accessReq.ActionType, policy, isCheckCondition);
				}
				else
				{
					// when there isn't any rules matched, take the default settings
					switch (accessReq.ActionType)
					{
						case XaclActionType.Read:
							conclusion.Permission = policy.Setting.DefaultReadPermission;
							break;
						case XaclActionType.Write:
							conclusion.Permission = policy.Setting.DefaultWritePermission;
							break;
						case XaclActionType.Create:
							conclusion.Permission = policy.Setting.DefaultCreatePermission;
							break;
						case XaclActionType.Delete:
							conclusion.Permission = policy.Setting.DefaultDeletePermission;
							break;
						case XaclActionType.Upload:
							conclusion.Permission = policy.Setting.DefaultUploadPermission;
							break;
						case XaclActionType.Download:
							conclusion.Permission = policy.Setting.DefaultDownloadPermission;
							break;
					}
				}

				return conclusion;
			}
			finally
			{
				// unset the current instance as a context for condition evaluation
				if (currentInstance != null && principal != null)
				{
					principal.CurrentInstance = null;
				}
			}
		}

		/// <summary>
		/// Gets a conclusion of the evaluating an access request.
		/// </summary>
		/// <param name="decisions">The list of decisions</param>
		/// <param name="actionType">The action type</param>
		/// <param name="policy">The xacl policy</param>
		/// <param name="isCheckCondition">Indicate whether to check the condition of a rule</param>
		/// <returns>A Conclusion object</returns>
		private Conclusion GetConclusion(ArrayList decisions, XaclActionType actionType,
			XaclPolicy policy, bool isCheckCondition)
		{
			Conclusion conclusion = new Conclusion();
			conclusion.DecisionList = decisions;
			XaclConflictResolutionType resolutionType = policy.Setting.ConflictResolutionType;

			if (HasConflict(decisions))
			{
                switch (resolutionType)
				{
					case XaclConflictResolutionType.Dtp:
						conclusion.Permission = XaclPermissionType.Deny;
						break;
					case XaclConflictResolutionType.Gtp:
						conclusion.Permission = XaclPermissionType.Grant;
						break;
					case XaclConflictResolutionType.Ntp:
						// take the default permission
						switch (actionType)
						{
							case XaclActionType.Read:
								conclusion.Permission = policy.Setting.DefaultReadPermission;
								break;
							case XaclActionType.Write:
								conclusion.Permission = policy.Setting.DefaultWritePermission;
								break;
							case XaclActionType.Create:
								conclusion.Permission = policy.Setting.DefaultCreatePermission;
								break;
							case XaclActionType.Delete:
								conclusion.Permission = policy.Setting.DefaultDeletePermission;
								break;
							case XaclActionType.Upload:
								conclusion.Permission = policy.Setting.DefaultUploadPermission;
								break;
							case XaclActionType.Download:
								conclusion.Permission = policy.Setting.DefaultDownloadPermission;
								break;
						}
						break;
				}
			}
			else
			{
				conclusion.Permission = ((Decision) decisions[0]).Permission;
			}


			if (!isCheckCondition && IsConclusionConditional(conclusion))
			{
				if (conclusion.Permission == XaclPermissionType.Grant)
				{
					conclusion.Permission = XaclPermissionType.ConditionalGrant;
				}
				else
				{
					conclusion.Permission = XaclPermissionType.ConditionalDeny;
				}
			}

			return conclusion;
		}

		/// <summary>
		/// Gets the information indicating whether there is a conflict in the
		/// evaluating decisions
		/// </summary>
		/// <param name="decisions">The decisions</param>
		/// <returns>true if there is a conflict, false otherwise</returns>
		private bool HasConflict(ArrayList decisions)
		{
			bool hasConflict = false;

			if (decisions.Count > 1)
			{
				XaclPermissionType permission = ((Decision) decisions[0]).Permission;

				for (int i = 1; i < decisions.Count; i++)
				{
                    if (((Decision) decisions[i]).Permission != permission)
					{
						hasConflict = true;
						break;
					}
				}
			}

			return hasConflict;
		}

		/// <summary>
		/// Gets the information indicating whether a conclusion is conditional,
		/// meaning all the decisions contributing the conclusion are conditional.
		/// </summary>
		/// <param name="conclusion">The conclusion</param>
		/// <returns>true if the conclusion are conditional, false otherwise.</returns>
		private bool IsConclusionConditional(Conclusion conclusion)
		{
			bool status = true;

			foreach (Decision decision in conclusion.DecisionList)
			{
				if (decision.Permission == conclusion.Permission &&
					(decision.Rule.Condition.Condition == null ||
					decision.Rule.Condition.Condition.Length == 0))
				{
					// one of the decision is unconditional, therefore,
					// the conclusion is unconditional
					status = false;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether the access request is submmited
		/// by the super user
		/// </summary>
		/// <param name="accessReq">The access request</param>
		/// <returns>true if it is submitted by the super user, false otherwise.</returns>
		private bool IsSuperUser(AccessRequest accessReq)
		{
			bool isSuperUser = false;
			string[] roles = accessReq.Subject.Roles;

			if (roles != null && roles.Length == 1 &&
				roles[0] == NewteraNameSpace.CM_SUPER_USER_ROLE)
			{
				isSuperUser = true;
			}

			return isSuperUser;
		}
	}
}