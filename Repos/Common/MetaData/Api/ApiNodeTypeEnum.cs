/*
* @(#)SubscriberNodeTypeEnum.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	/// <summary>
	/// Specify the types of nodes in Api package.
	/// </summary>
	public enum ApiNodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// ApiManager
		/// </summary>
		ApiManager,

		/// <summary>
		/// ApiGroup
		/// </summary>
		ApiGroup,

		/// <summary>
		/// Api
		/// </summary>
		Api,

		/// <summary>
		/// Collection
		/// </summary>
		Collection,

		/// <summary>
        /// ApiGroupCollection
		/// </summary>
		ApiGroupCollection,

		/// <summary>
		/// ApiCollection
		/// </summary>
		ApiCollection
	}
}