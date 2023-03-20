/*
* @(#)IEnumConstraint.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Collections.Specialized;

	/// <summary>
	/// Represents an interface for the constraints provides enum values, such as EnumElement and ListElement
	/// </summary>
	/// <version> 1.0.0 2 Apr 2008 </version>
	public interface IEnumConstraint
	{
		/// <summary>
		/// Get a collection of enum values of the enum constraint
		/// </summary>
        /// <value>A collection of enum values.</value>
        EnumValueCollection Values { get;}

        /// <summary>
		/// Convert an enum display text to its value.
		/// </summary>
		/// <param name="text">An enum display text.</param>
		/// <returns>The corresponding enum value</returns>
        string GetValue(string text);

        /// <summary>
        /// Gets the corresponsing image name of an enum value
        /// </summary>
        /// <param name="enumValue">enum value</param>
        /// <returns>The display text, could be null</returns>
        string GetImageName(string val);

        /// <summary>
        /// Gets the information indicates whether the values of the constraint are generated based on conditions
        /// </summary>
        bool IsConditionBased {get;}

        /// <summary>
        /// Gets the information indicates whether the values of the constraint are user-based
        /// </summary>
        bool IsUserBased { get;}
	}
}