/*
* @(#)IEventNodeVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in an event name space.
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
	public interface IEventNodeVisitor
	{
		/// <summary>
        /// Viste a EventManager element.
		/// </summary>
        /// <param name="element">A EventManager instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitEventManager(EventManager element);

        /// <summary>
		/// Viste an EventGroupCollection element.
		/// </summary>
        /// <param name="element">A EventGroupCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitEventGroupCollection(EventGroupCollection element);
        
		/// <summary>
		/// Viste an EventGroup element.
		/// </summary>
        /// <param name="element">A EventGroup instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitEventGroup(EventGroup element);

        /// <summary>
        /// Viste an EventCollection element.
        /// </summary>
        /// <param name="element">A EventCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitEventCollection(EventCollection element);

		/// <summary>
		/// Viste a EventDef element.
		/// </summary>
        /// <param name="element">A EventDef instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitEventDef(EventDef element);
	}
}