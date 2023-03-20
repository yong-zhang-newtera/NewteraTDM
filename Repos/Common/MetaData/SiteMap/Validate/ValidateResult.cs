/*
* @(#)ValidateResult.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap.Validate
{
	using System;
	using System.Collections;
	using Newtera.Common.MetaData.SiteMap;

	/// <summary>
	/// Keeps the result of validating a site map model.
	/// </summary>
	/// <version>  	1.0.0 24 Jun 2009 </version>
	public class ValidateResult
	{
		private ValidateResultEntryCollection _errors;

		/// <summary>
		/// Initializes a new instance of the ValidateResult class
		/// </summary>
		public ValidateResult()
		{
			_errors = new ValidateResultEntryCollection();
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
	}
}