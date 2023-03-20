/*
* @(#)DataViewValidateResult.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Validate
{
	using System;
	using System.Collections;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Keeps the result of validating an instance data.
	/// </summary>
	/// <version>  	1.0.0 16 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class DataViewValidateResult
	{
		private DataValidateResultEntryCollection _errors;
        private DataValidateResultEntryCollection _doubts;

		/// <summary>
		/// Initializes a new instance of the DataViewValidateResult class
		/// </summary>
		public DataViewValidateResult()
		{
			_errors = new DataValidateResultEntryCollection();
            _doubts = new DataValidateResultEntryCollection();
		}

		/// <summary>
		/// Gets the information indicating whether there are validating
		/// errors.
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
        /// Gets the information indicating whether there are validating
        /// doubts.
        /// </summary>
        /// <value>true if it has doubts, false otherwise</value>
        public bool HasDoubt
        {
            get
            {
                if (_doubts.Count > 0)
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
		public DataValidateResultEntryCollection Errors
		{
			get
			{
				return _errors;
			}
		}

        /// <summary>
        /// Gets the validating doubts
        /// </summary>
        public DataValidateResultEntryCollection Doubts
        {
            get
            {
                return _doubts;
            }
        }

		/// <summary>
		/// Add a validating error entry
		/// </summary>
		public void AddError(DataValidateResultEntry entry)
		{
			_errors.Add(entry);
		}

        /// <summary>
        /// Add a validating doubt entry
        /// </summary>
        public void AddDoubt(DataValidateResultEntry entry)
        {
            bool found = false;

            if (entry.EntryType == EntryType.PrimaryKey)
            {
                // there may have multiple primary keys, we just need one doubt entry
                foreach (DataValidateResultEntry et in _doubts)
                {
                    if (et.EntryType == EntryType.PrimaryKey)
                    {
                        found = true;
                    }
                }

            }

            if (!found)
            {
                _doubts.Add(entry);
            }
        }
	}
}