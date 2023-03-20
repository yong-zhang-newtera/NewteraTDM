/*
* @(#)EventNodeTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	/// <summary>
	/// Specify the types of nodes in Events package.
	/// </summary>
	public enum EventNodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// EventManager
		/// </summary>
		EventManager,

		/// <summary>
		/// EventGroup
		/// </summary>
		EventGroup,

		/// <summary>
		/// EventDef
		/// </summary>
		EventDef,

		/// <summary>
		/// Collection
		/// </summary>
		Collection,

		/// <summary>
        /// EventGroupCollection
		/// </summary>
		EventGroupCollection,

		/// <summary>
		/// EventCollection
		/// </summary>
		EventCollection
	}
}