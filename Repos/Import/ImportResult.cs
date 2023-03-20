/*
* @(#)ImportResult.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Import
{
	using System;
	using System.Collections;

	/// <summary>
	/// Keeps the results of importing a data file, such as errors or warnings.
	/// </summary>
	/// <version>  	1.0.0 21 Apr 2007 </version>
	public class ImportResult
	{
        private string _mainMessage;
		private ImportResultEntryCollection _errors;
        private ImportResultEntryCollection _warnings;

		/// <summary>
		/// Initializes a new instance of the ImportResult class
		/// </summary>
		public ImportResult()
		{
            _mainMessage = "";
			_errors = new ImportResultEntryCollection();
            _warnings = new ImportResultEntryCollection();
		}

        /// <summary>
        /// Gets or sets the main message of the import result.
        /// </summary>
        public string MainMessage
        {
            get
            {
                return _mainMessage;
            }
            set
            {
                _mainMessage = value;
            }
        }

		/// <summary>
		/// Gets the information indicating whether there are errors during the import process.
		/// </summary>
		/// <value>true if it has errors, false otherwise</value>
		public bool HasError
		{
			get
			{
				if (_errors.Count > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether there are warnings during the import process.
        /// </summary>
        /// <value>true if it has warnings, false otherwise</value>
        public bool HasWarning
        {
            get
            {
                if (_warnings.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// Gets the validating errors
		/// </summary>
		public ImportResultEntryCollection Errors
		{
			get
			{
				return _errors;
			}
		}

        /// <summary>
        /// Gets the validating doubts
        /// </summary>
        public ImportResultEntryCollection Warnings
        {
            get
            {
                return _warnings;
            }
        }

		/// <summary>
		/// Add an import error entry
		/// </summary>
		public void AddError(ImportResultEntry entry)
		{
			_errors.Add(entry);
		}

        /// <summary>
        /// Add an import warning entry
        /// </summary>
        public void AddWarning(ImportResultEntry entry)
        {
            _warnings.Add(entry);
        }
	}
}