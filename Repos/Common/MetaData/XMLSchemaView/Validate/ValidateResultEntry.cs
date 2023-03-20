/*
* @(#)DataValidateResultEntry.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Validate
{
	using System;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Represent an entry of validating error
	/// </summary>
	/// <version>  	1.0.0 16 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class DataValidateResultEntry
	{
		private string _message;
		private string _source;
		private IDataViewElement _offendingElement;
        private EntryType _type;
        private string _className;

		/// <summary>
		/// Initializes a new instance of the DataValidateResultEntry class
		/// </summary>
		/// <param name="msg">The entry message</param>
		/// <param name="source">The source string</param>
		/// <param name="attribute">The data view element associated with the entry</param>
		public DataValidateResultEntry(string msg, string source, IDataViewElement attribute)
		{
			_message = msg;
			_source = source;
			_offendingElement = attribute;
            _type = EntryType.Error;
            _className = null;
		}

        /// <summary>
        /// Initializes a new instance of the DataValidateResultEntry class
        /// </summary>
        /// <param name="msg">The entry message</param>
        /// <param name="source">The source string</param>
        /// <param name="attribute">The data view element associated with the entry</param>
        /// <param name="type">One of the EntryType enum values</param>
        public DataValidateResultEntry(string msg, string source, IDataViewElement attribute, EntryType type)
        {
            _message = msg;
            _source = source;
            _offendingElement = attribute;
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
		/// Gets the offending dataview element
		/// </summary>
		public IDataViewElement DataViewElement
		{
			get
			{
				return _offendingElement;
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

        /// <summary>
        /// Gets or sets the class name of validating entry
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
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
        /// <summary>
        /// Primary key constraint, further check the primary key constraint
        /// </summary>
        PrimaryKey,
        /// <summary>
        /// Unique value, further check the unique value
        /// </summary>
        UniqueValue,
        /// <summary>
        /// Unique reference, further check the unique reference
        /// </summary>
        UniqueReference,
        /// <summary>
        /// Combind Unique values, further check if the combination of the values are unique
        /// </summary>
        UniqueValues,
    }
}