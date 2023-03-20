/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	/// <summary>
	/// Specify the types of nodes in Rules package.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// RuleManager
		/// </summary>
		RuleManager,
		/// <summary>
		/// RuleSet
		/// </summary>
		RuleSet,
		/// <summary>
		/// RuleDef
		/// </summary>
		RuleDef,
		/// <summary>
		/// Collection
		/// </summary>
		Collection,
		/// <summary>
		/// RuleSetCollection
		/// </summary>
		RuleSetCollection,
		/// <summary>
		/// RuleCollection
		/// </summary>
		RuleCollection,
        /// <summary>
        /// PrioritizedRuleList
        /// </summary>
        PrioritizedRuleList
	}
}