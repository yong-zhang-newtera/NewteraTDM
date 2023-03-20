/*
* @(#)ValidateResult.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Validate
{
	using System;
	using System.Collections;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Keeps the result of validating a schema model.
	/// </summary>
	/// <version>  	1.0.0 23 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class ValidateResult
	{
		private ValidateResultEntryCollection _errors;
		private ValidateResultEntryCollection _warnings;
		private ValidateResultEntryCollection _doubts;

		/// <summary>
		/// Initializes a new instance of the ValidateResult class
		/// </summary>
		public ValidateResult()
		{
			_errors = new ValidateResultEntryCollection();
			_warnings = new ValidateResultEntryCollection();
			_doubts = new ValidateResultEntryCollection();
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
		/// warnings.
		/// </summary>
		/// <value>true if it has waring, false otherwise</value>
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
		/// Gets the information indicating whether there are validating
		/// doubts that need to be verified.
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
		public ValidateResultEntryCollection Errors
		{
			get
			{
				return _errors;
			}
		}

		/// <summary>
		/// Add a validating error entry
		/// </summary>
		public void AddError(ValidateResultEntry entry)
		{
			_errors.Add(entry);
		}

		/// <summary>
		/// Gets the validating warnings
		/// </summary>
		public ValidateResultEntryCollection Warnings
		{
			get
			{
				return _warnings;
			}
		}

		/// <summary>
		/// Add a validating warning entry
		/// </summary>
		public void AddWarning(ValidateResultEntry entry)
		{
			_warnings.Add(entry);
		}

		/// <summary>
		/// Gets the validating doubts
		/// </summary>
		public ValidateResultEntryCollection Doubts
		{
			get
			{
				return _doubts;
			}
		}

		/// <summary>
		/// Add a validating warning doubt
		/// </summary>
		public void AddDoubt(ValidateResultEntry entry)
		{
			_doubts.Add(entry);
		}

		/// <summary>
		/// Gets the all entries
		/// </summary>
		public ValidateResultEntryCollection AllEntries
		{
			get
			{
				ValidateResultEntryCollection entries = new ValidateResultEntryCollection();
				entries.AddRange(_errors);
				entries.AddRange(_warnings);

				return entries;
			}
		}
	}
}