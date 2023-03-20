/*
* @(#)RuleMatcher.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Newtera.Common.MetaData.Logging;

	/// <summary>
	/// The class implements the matching algorithm. The 
	/// algorithm is to find applicable rules that match a given logging request. 
	/// And it doesn't concern about conflict rules at this point, but simply
	/// finds all rules that are matched.
	/// </summary>
	/// <version>1.0.0 05 Jan 2009 </version>
	internal class RuleMatcher
	{		
		/// <summary>
		/// Initiate an instance of RuleMatcher class.
		/// </summary>
		internal RuleMatcher()
		{
		}

		/// <summary>
		/// Match a logging request with rules and return a list of decision of the
		/// matched rules.
		/// </summary>
		/// <param name="loggingReq">The logging request.</param>
		/// <param name="policy">The xacl policy.</param>
		/// <returns>
		/// A list of decisions of the rules that match the logging request.
		/// </returns>
		public ArrayList Match(LoggingRequest loggingReq, LoggingPolicy policy)
		{	
			ArrayList decisions = new ArrayList();

			// Get all the rules, including propogated ones for the LoggingObject of
			// the logging request
			LoggingRuleCollection rules = policy.GetRules(loggingReq.Object);
			
			if (rules != null)
			{
				// Get the matched rules.
				LoggingRuleCollection matchedRules = GetMatchedRules(rules, loggingReq);

				foreach (LoggingRule rule in matchedRules)
				{
                    LoggingStatus status = rule.GetLoggingStatus(loggingReq.ActionType);
					Decision decision = new Decision(rule, status);
					decisions.Add(decision);
				}
			}
			
			return decisions;
		}
		
		/// <summary>
		/// Gets the rules that match the criteria specified in an logging request.
		/// </summary>
		/// <param name="rules">A collection of rules to match against</param>
		/// <param name="loggingReq">An logging request.</param>
		/// <returns>A collection of matched rules</returns>
		private LoggingRuleCollection GetMatchedRules(LoggingRuleCollection rules, LoggingRequest loggingReq)
		{
			LoggingRuleCollection matchedRules = new LoggingRuleCollection();
			
			foreach (LoggingRule rule in rules)
			{
				if (IsMatched(rule, loggingReq))
				{
					matchedRules.Add(rule);
				}
			}

			return matchedRules;
		}
		
		/// <summary>
		/// Gets the information indicating whether a rule matchs the criteria specified
		/// in an logging request. A rule is considered to be matched if rule's subject matchs
        /// that of an logging request.
		/// </summary>
		/// <param name="rule"> The rule to be matched.</param>
		/// <param name="loggingReq"> The logging request.</param>
		/// <returns>true if the rule is matched, false otherwise.</returns>
		private bool IsMatched(LoggingRule rule, LoggingRequest loggingReq)
		{
			bool isMatched = false;

			// match subject
			if (IsSubjectMatched(rule.Subject, loggingReq.Subject))
			{
				isMatched = true;
			}

			return isMatched;
		}
				
		/// <summary>
		/// Gets the information indicating whether the subject of an logging request
		/// matches that of a rule.
		/// </summary>
		/// <param name="ruleSubject">The subject of a rule.</param>
		/// <param name="reqSubject">The subject of an logging request.</param>
		/// <returns> True if subjects are matched, false otherwise.</returns>
		private bool IsSubjectMatched(LoggingSubject ruleSubject, LoggingSubject reqSubject)
		{
			bool isMatched = false;
			if (ruleSubject.Roles.Length == 1 && ruleSubject.Roles[0] == LoggingSubject.EveryOne)
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
		/// Gets the information indicating whether the Uid of an logging request subject
		/// matches that of a rule subject
		/// </summary>
		/// <param name="ruleUid">The uid of a rule</param>
		/// <param name="reqUid">The uid of an logging request.</param>
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
		/// If the rule's groups are subset of logging request, it is considered as being
		/// matched.  If the rule doesn't have a group, it is considered being matched
		/// </remarks>
		private bool IsGroupMatched(string[] ruleGroups, string[] reqGroups)
		{
			return Contains(reqGroups, ruleGroups);
		}

		/// <summary>
		/// Gets the information indicating whether roles of a rule and a logging
		/// request are matched.
		/// </summary>
		/// <param name="ruleRoles">The rule's roles.</param>
		/// <param name="reqRoles">The request's roles.</param>
		/// <returns> true if roles are matched; false, otherwise.</returns>
		/// <remarks>
		/// If rule's roles is a subset of that of the logging request,
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
	}
}