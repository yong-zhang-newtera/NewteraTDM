/*
* @(#)SubscriberListViewItem.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Represents a ListView item for a class subscriber
	/// </summary>
	/// <version>  1.0.1 16 Sep 2013</version>
	public class SubscriberListViewItem : ListViewItem
	{
		private ClassElement _clsElement;
		private Subscriber _subscriber;

		/// <summary>
		/// Initializes a new instance of the SubscriberListViewItem class.
		/// </summary>
		/// <param name="clsElement">The ClassElement that the event is defined for.</param>
		/// <param name="subscriber">The Subscriber instance</param>
		public SubscriberListViewItem(ClassElement clsElement, Subscriber subscriber) : base(subscriber.Name)
		{
            _clsElement = clsElement;
			_subscriber = subscriber;
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
		/// Gets the Subscriber instance
		/// </summary>
		/// <value> The Subscriber instance</value>
        public Subscriber Subscriber
		{
			get
			{
				return _subscriber;
			}
		}
	}
}