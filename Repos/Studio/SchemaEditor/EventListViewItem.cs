/*
* @(#)EventListViewItem.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Represents a ListView item for a class event
	/// </summary>
	/// <version>  1.0.1 25 Dec 2006</version>
	public class EventListViewItem : ListViewItem
	{
		private ClassElement _clsElement;
		private EventDef _event;

		/// <summary>
		/// Initializes a new instance of the EventListViewItem class.
		/// </summary>
		/// <param name="clsElement">The ClassElement that the event is defined for.</param>
		/// <param name="eventDef">The EventDef instance</param>
		public EventListViewItem(ClassElement clsElement, EventDef eventDef) : base(eventDef.Name)
		{
            _clsElement = clsElement;
			_event = eventDef;
		}

		/// <summary> 
		/// Gets the ClassElement instance
		/// </summary>
		/// <value> The ClassElement instance</value>
		public ClassElement ClassElement
		{
			get
			{
				return _clsElement;
			}
		}

		/// <summary> 
		/// Gets the EventDef instance
		/// </summary>
		/// <value> The EventDef instance</value>
		public EventDef Event
		{
			get
			{
				return _event;
			}
		}
	}
}