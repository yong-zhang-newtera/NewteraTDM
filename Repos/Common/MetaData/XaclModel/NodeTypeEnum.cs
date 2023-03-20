/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	/// <summary>
	/// Specify the types of nodes in xacl policy.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Policy
		/// </summary>
		Policy,
		/// <summary>
		/// Collection
		/// </summary>
		Collection,
		/// <summary>
		/// Definition
		/// </summary>
		Definition,
		/// <summary>
		/// Object
		/// </summary>
		Object,
		/// <summary>
		/// Subject
		/// </summary>
		Subject,
		/// <summary>
		/// Rule
		/// </summary>
		Rule,
		/// <summary>
		/// Action
		/// </summary>
		Action,
		/// <summary>
		/// Condition
		/// </summary>
		Condition,
		/// <summary>
		/// Rules
		/// </summary>
		Rules,
		/// <summary>
		/// Actions
		/// </summary>
		Actions,
		/// <summary>
		/// Definitions
		/// </summary>
		Definitions,
		/// <summary>
		/// Setting
		/// </summary>
		Setting
	}
}