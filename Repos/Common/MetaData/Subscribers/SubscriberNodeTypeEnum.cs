/*
* @(#)SubscriberNodeTypeEnum.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	/// <summary>
	/// Specify the types of nodes in Subscriber package.
	/// </summary>
	public enum SubscriberNodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// SubscriberManager
		/// </summary>
		SubscriberManager,

		/// <summary>
		/// SubscriberGroup
		/// </summary>
		SubscriberGroup,

		/// <summary>
		/// Subscriber
		/// </summary>
		Subscriber,

		/// <summary>
		/// Collection
		/// </summary>
		Collection,

		/// <summary>
        /// SubscriberGroupCollection
		/// </summary>
		SubscriberGroupCollection,

		/// <summary>
		/// SubscriberCollection
		/// </summary>
		SubscriberCollection
	}
}