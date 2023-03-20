/*
* @(#)ValidateResultEntry.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap.Validate
{
	using System;
	using Newtera.Common.MetaData.SiteMap;

	/// <summary>
	/// Represent an entry of validating error.
	/// </summary>
	/// <version>  	1.0.0 24 Jun 2009 </version>
	public class ValidateResultEntry
	{
		private string _message;
		private string _source;
		private EntryType _type;
		private ISiteMapNode _node;

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
		public ValidateResultEntry(string msg, string source, EntryType type, ISiteMapNode element)
		{
			_message = msg;
			_source = source;
			_type = type;
			_node = element;
		}


		/// <summary>
		/// Gets the message that describes the validating problem.
		/// </summary>
		/// <value>A string</value>
		public string Message
		{
			get
			{
				return _message;
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
		public ISiteMapNode SiteMapNode
		{
			get
			{
				return _node;
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
		Error
	}
}