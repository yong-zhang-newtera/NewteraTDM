/*
* @(#)RuleMatcher.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The class implements the matching algorithm. The 
	/// algorithm is to find applicable rules that match a given access request. 
	/// And it doesn't concern about conflict rules, but simply
	/// finds all rules that are matched.
	/// </summary>
	/// <version>1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	internal class RuleMatcher
	{		
		private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";
		
		private IConditionRunner _conditionRunner;

		/// <summary>
		/// Initiate an instance of RuleMatcher class.
		/// </summary>
		internal RuleMatcher()
		{
			_conditionRunner = null;
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
				return _conditionRunner;
			}
			set
			{
				_conditionRunner = value;
			}
		}

		/// <summary>
		/// Match an access request with rules and return a list of decision of the
		/// matched rules.
		/// </summary>
		/// <param name="accessReq">The access request.</param>
		/// <param name="policy">The xacl policy.</param>
		/// <param name="isCheckCondition">If true, executing condition; 
		/// otherwise, condition always is true.</param>
		/// <returns>
		/// A list of decisions of the rules that match the access request.
		/// </returns>
		public ArrayList Match(AccessRequest accessReq, XaclPolicy policy, bool isCheckCondition)
		{	
			ArrayList decisions = new ArrayList();

			// Get all the rules, including propogated ones for the XaclObject of
			// the access request
			XaclRuleCollection rules = policy.GetRules(accessReq.Object);
			
			if (rules != null)
			{
				// Get the matched rules.
                XaclRuleCollection matchedRules = GetMatchedRules(policy, rules, accessReq, isCheckCondition);

				foreach (XaclRule rule in matchedRules)
				{
					XaclPermissionType permission = rule.GetPermission(accessReq.ActionType);
					Decision decision = new Decision(rule, permission);
					decisions.Add(decision);
				}
			}
			
			return decisions;
		}
		
		/// <summary>
		/// Gets the rules that match the criteria specified in an access request.
		/// </summary>
        /// <param name="policy">The xacl policy</param>
		/// <param name="rules">A collection of rules to match against</param>
		/// <param name="accessReq">An access request.</param>
		/// <param name="isCheckCondition">true to check the condition, false otherwise.</param>
		/// <returns>A collection of matched rules</returns>
		private XaclRuleCollection GetMatchedRules(XaclPolicy policy, XaclRuleCollection rules, AccessRequest accessReq, bool isCheckCondition)
		{
			XaclRuleCollection matchedRules = new XaclRuleCollection();
			
			foreach (XaclRule rule in rules)
			{
				if (IsMatched(policy, rule, accessReq, isCheckCondition))
				{
					matchedRules.Add(rule);
				}
			}

			return matchedRules;
		}
		
		/// <summary>
		/// Gets the information indicating whether a rule matchs the criteria specified
		/// in an access request. A rule is considered to be matched if rule's subject and
		/// condition (if required) match that of an access request.
		/// </summary>
        /// <param name="policy">The xacl policy</param>
		/// <param name="rule"> The rule to be matched.</param>
		/// <param name="accessReq"> The access request.</param>
		/// <param name="isCheckCondition">true to check the condition, false otherwise.</param>
		/// <returns>true if the rule is matched, false otherwise.</returns>
		private bool IsMatched(XaclPolicy policy, XaclRule rule, AccessRequest accessReq, bool isCheckCondition)
		{
			bool isMatched = false;

			// match subject
			if (IsSubjectMatched(rule.Subject, accessReq.Subject))
			{
                isMatched = true;

                // match condition
                if (isCheckCondition && !IsConditionMatched(policy, rule, accessReq.ActionType))
                {
                    isMatched = false;
                }
			}

			return isMatched;
		}
				
		/// <summary>
		/// Gets the information indicating whether the subject of an access request
		/// matches that of a rule.
		/// </summary>
		/// <param name="ruleSubject">The subject of a rule.</param>
		/// <param name="reqSubject">The subject of an access request.</param>
		/// <returns> True if subjects are matched, false otherwise.</returns>
		private bool IsSubjectMatched(XaclSubject ruleSubject, XaclSubject reqSubject)
		{
			bool isMatched = false;
			if (ruleSubject.Roles.Length == 1 && ruleSubject.Roles[0] == XaclSubject.EveryOne)
			{
				isMatched = true;
			}
			else if (IsUidMatched(ruleSubject.Uid, reqSubject.Uid))
			{
				isMatched = true;
			}
			else if (IsRoleMatched(ruleSubject.Roles, reqSubject.Roles))
			{
				isMatched = true;
			}
			else if (IsGroupMatched(ruleSubject.Groups, reqSubject.Groups))
			{
				isMatched = true;
			}

			return isMatched;
		}

        /// <summary>
        /// Gets the information indicating whether the action of an access request
        /// matches that of a rule.
        /// </summary>
        /// <param name="policy">The xacl policy</param>
        /// <param name="rule">A rule.</param>
        /// <param name="reqAction">The action of an access request.</param>
        /// <returns> True if rule are matched, false otherwise.</returns>
        private bool IsActionMatched(XaclPolicy policy, XaclRule rule, XaclActionType reqAction)
        {
            bool isMatched = false;

            switch (reqAction)
            {
                case XaclActionType.Read:
                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsReadDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsReadGranted)
                    {
                        isMatched = true;
                    }

                    break;

                case XaclActionType.Download:

                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsDownloadDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsDownloadGranted)
                    {
                        isMatched = true;
                    }

                    break;

                case XaclActionType.Write:

                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsWriteDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsWriteGranted)
                    {
                        isMatched = true;
                    }

                    break;

                case XaclActionType.Upload:

                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsUploadDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsUploadGranted)
                    {
                        isMatched = true;
                    }

                    break;

                case XaclActionType.Create:

                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsCreateDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsCreateGranted)
                    {
                        isMatched = true;
                    }

                    break;

                case XaclActionType.Delete:

                    if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Deny &&
                        rule.IsDeleteDenied)
                    {
                        isMatched = true;
                    }
                    else if (policy.GetResolutionPermission(reqAction) == XaclPermissionType.Grant &&
                        rule.IsDeleteGranted)
                    {
                        isMatched = true;
                    }

                    break;

                default:
                    break;
            }

            return isMatched;
        }
		
		/// <summary>
		/// Gets the information indicating whether the Uid of an access request subject
		/// matches that of a rule subject
		/// </summary>
		/// <param name="ruleUid">The uid of a rule</param>
		/// <param name="reqUid">The uid of an access request.</param>
		/// <returns>true if the uid is matched, false otherwise.</returns>
		/// <remarks>If the rule does not have a UID, it is considered to be matched</remarks>
		private bool IsUidMatched(string ruleUid, string reqUid)
		{
			bool isMatched = false;

			if (ruleUid != null && reqUid != null && reqUid == ruleUid)
			{
				isMatched = true;
			}
			
			return isMatched;
		}
		
		/// <summary>
		/// Gets the information indicating whether groups of a rule and an access
		/// request are matched.
		/// </summary>
		/// <param name="ruleGroups">The rule's groups.</param>
		/// <param name="reqGroups">The request's groups.</param>
		/// <returns> true if groups are matched; false, otherwise.</returns>
		/// <remarks>
		/// If the rule's groups are subset of access request, it is considered as being
		/// matched.  If the rule doesn't have a group, it is considered being matched
		/// </remarks>
		private bool IsGroupMatched(string[] ruleGroups, string[] reqGroups)
		{
			return Contains(reqGroups, ruleGroups);
		}

		/// <summary>
		/// Gets the information indicating whether roles of a rule and an access
		/// request are matched.
		/// </summary>
		/// <param name="ruleRoles">The rule's roles.</param>
		/// <param name="reqRoles">The request's roles.</param>
		/// <returns> true if roles are matched; false, otherwise.</returns>
		/// <remarks>
		/// If rule's roles is a subset of that of the access request,
		/// it is considered to be matched.
		/// </remarks>
		private bool IsRoleMatched(string[] ruleRoles, string[] reqRoles)
		{
            return Contains(reqRoles, ruleRoles);
		}
		
		/// <summary>
		/// Gets the information indicating whether a string array contains another
		/// string array
		/// </summary>
		/// <param name="firstArray">The first string array</param>
		/// <param name="secondArray">The second string array</param>
		/// <returns> 
		/// true if the first string array contains the second string array
		/// </returns>
		private bool Contains(string[] firstArray, string[] secondArray)
		{
			bool isMatched = true;
			if (firstArray != null && firstArray.Length >0 &&
				secondArray != null && secondArray.Length > 0)
			{
				foreach (string secondValue in secondArray)
				{
					bool isFound = false;

					foreach (string firstValue in firstArray)
					{
						if (firstValue == secondValue)
						{
							isFound = true;
							break;
						}
					}

					if (!isFound)
					{
						isMatched = false;
						break;
					}
				}
			}
			else
			{
				isMatched = false;
			}

			return isMatched;
		}
	
		/// <summary>
		/// Gets the information indicating whether the condition of a rule meets
		/// the current context.
		/// </summary>
        /// <param name="policy">The xacl policy</param>
		/// <param name="rule">The access rule.</param>
		/// <returns> if matching, return true; otherwise return false.</returns>
        private bool IsConditionMatched(XaclPolicy policy, XaclRule rule, XaclActionType reqAction)
		{
			// get xquery string for the condition
			XaclCondition condition = rule.Condition;
			if (condition.Condition == null || condition.Condition.Length == 0)
			{
				return true;
			}

            if (IsActionMatched(policy, rule, reqAction))
            {
                // build a complete xquery
                string finalCondition = TEMPLATE_XQUERY.Replace("$$", condition.Condition);

                if (_conditionRunner != null)
                {
                    return _conditionRunner.IsConditionMet(finalCondition);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
		}
	}
}