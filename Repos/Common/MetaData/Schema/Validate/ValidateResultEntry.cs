/*
* @(#)ValidateResultEntry.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Validate
{
	using System;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.Api;

	/// <summary>
	/// Represent an entry of validating error or warning.
	/// </summary>
	/// <version>  	1.0.0 23 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class ValidateResultEntry
	{
		private string _message;
		private string _source;
		private EntryType _type;
		private IMetaDataElement _element;
        private EventDef _eventDef;
        private Subscriber _subscriber;
        private Api _api;

        /// <summary>
        /// Initializes a new instance of the ValidateResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="type">The type of entry</param>
        public ValidateResultEntry(string msg, string source, EntryType type) :
            this(msg, source, type, null)
        {
        }

		/// <summary>
		/// Initializes a new instance of the ValidateResultEntry class
		/// </summary>
		/// <param name="msg">The entry message</param>
		/// <param name="source">The source string</param>
		/// <param name="type">The type of entry</param>
		/// <param name="element">The meta data element associated with the entry</param>
		public ValidateResultEntry(string msg, string source, EntryType type, IMetaDataElement element)
		{
			_message = msg;
			_source = source;
			_type = type;
			_element = element;
            _eventDef = null;
            _subscriber = null;
		}

        /// <summary>
        /// Initializes a new instance of the ValidateResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="type">The type of entry</param>
        /// <param name="element">The meta data element associated with the entry</param>
        public ValidateResultEntry(string msg, string source, EntryType type, IMetaDataElement element, EventDef eventDef)
        {
            _message = msg;
            _source = source;
            _type = type;
            _element = element;
            _eventDef = eventDef;
            _subscriber = null;
        }

        /// <summary>
        /// Initializes a new instance of the ValidateResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="type">The type of entry</param>
        /// <param name="element">The meta data element associated with the entry</param>
        public ValidateResultEntry(string msg, string source, EntryType type, IMetaDataElement element, Subscriber subscriber)
        {
            _message = msg;
            _source = source;
            _type = type;
            _element = element;
            _eventDef = null;
            _subscriber = subscriber;
        }

        /// <summary>
        /// Initializes a new instance of the ValidateResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="type">The type of entry</param>
        /// <param name="element">The meta data element associated with the entry</param>
        public ValidateResultEntry(string msg, string source, EntryType type, IMetaDataElement element, Api api)
        {
            _message = msg;
            _source = source;
            _type = type;
            _element = element;
            _eventDef = null;
            _api = api;
        }

        /// <summary>
        /// Gets or sets the message that describes the validating problem.
        /// </summary>
        /// <value>A string</value>
        public string Message
		{
			get
			{
				return _message;
			}
            set
            {
                _message = value;
            }
		}

		/// <summary>
		/// Gets the source of the validating problem.
		/// </summary>
		/// <value>A string</value>
		public string Source
		{
			get
			{
				return _source;
			}
		}

		/// <summary>
		/// Gets or sets the type of entry
		/// </summary>
		/// <value>One of the EntryType enum values</value>
		public EntryType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets the meta data element that causes the problem.
		/// </summary>
		public IMetaDataElement MetaDataElement
		{
			get
			{
				return _element;
			}
		}

        /// <summary>
        /// Gets the EventDef object that causes the problem.
        /// </summary>
        public EventDef EventDef
        {
            get
            {
                return _eventDef;
            }
        }

        /// <summary>
        /// Gets the Subscriber object that causes the problem.
        /// </summary>
        public Subscriber Subscriber
        {
            get
            {
                return _subscriber;
            }
        }

        /// <summary>
        /// Gets the Api object that causes the problem.
        /// </summary>
        public Api Api
        {
            get
            {
                return _api;
            }
        }
    }

	/// <summary>
	/// Specify the types of validating result entry.
	/// </summary>
	public enum EntryType
	{
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
	}
}