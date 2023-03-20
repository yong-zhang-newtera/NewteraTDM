/*
* @(#)ISubscriberNodeVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in an subscriber name space.
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
	public interface ISubscriberNodeVisitor
	{
		/// <summary>
        /// Viste a SubscriberManager element.
		/// </summary>
        /// <param name="element">A SubscriberManager instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubscriberManager(SubscriberManager element);

        /// <summary>
		/// Viste an SubscriberGroupCollection element.
		/// </summary>
        /// <param name="element">A SubscriberGroupCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubscriberGroupCollection(SubscriberGroupCollection element);
        
		/// <summary>
		/// Viste an SubscriberGroup element.
		/// </summary>
        /// <param name="element">A SubscriberGroup instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubscriberGroup(SubscriberGroup element);

        /// <summary>
        /// Viste an SubscriberCollection element.
        /// </summary>
        /// <param name="element">A SubscriberCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubscriberCollection(SubscriberCollection element);

		/// <summary>
		/// Viste a Subscriber element.
		/// </summary>
        /// <param name="element">A Subscriber instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitSubscriber(Subscriber element);
	}
}