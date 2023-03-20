/*
* @(#)AttributeUsageEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;

	/// <summary>
	/// Describes the usages of an attribute, including simple and array attributes.
	/// </summary>
	public enum AttributeUsage
	{
		/// <summary>
		/// The attribute is not used for search or result
		/// </summary>
		None = 0,

		/// <summary>
		/// The attribute is used as a result attribute
		/// </summary>
		Result = 1,
		/// <summary>
		/// The attribute is used as a search attribute
		/// </summary>
		Search = 2,

		/// <summary>
		/// The attribute is used as both a search and result attribute
		/// </summary>
		Both = 4
	}

    public enum DefaultViewUsage
    {
        /// <summary>
        /// The attribute is part of defaulr view
        /// </summary>
        Included = 0,

        /// <summary>
        /// The attribute is not part of defaulr view
        /// </summary>
        Excluded = 1,
    }
}