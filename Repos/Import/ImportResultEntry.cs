/*
* @(#)ImportResultEntry.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Import
{
	using System;

	/// <summary>
	/// Represent an entry of import error
	/// </summary>
	/// <version>  	1.0.0 21 Apr 2007 </version>
	public class ImportResultEntry
	{
		private string _message;
		private string _source;
        private EntryType _type;

		/// <summary>
		/// Initializes a new instance of the ImportResultEntry class
		/// </summary>
		/// <param name="msg">The entry message</param>
		/// <param name="source">The source string</param>
		public ImportResultEntry(string msg, string source) : this(msg, source, EntryType.Error)
		{
		}

        /// <summary>
        /// Initializes a new instance of the ImportResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="type">One of the EntryType enum values</param>
        public ImportResultEntry(string msg, string source, EntryType type)
        {
            _message = msg;
            _source = source;
            _type = type;
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
        /// Gets or sets the type of validating entry
        /// </summary>
        public EntryType EntryType
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
	}

    /// <summary>
    /// Specify the types of import result error.
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