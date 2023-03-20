/*
* @(#)IAttributeValueGenerator.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;

    using Newtera.Common.MetaData;
    using Newtera.Common.Wrapper;

	/// <summary>
	/// Represents an interface for a custom attribute value generator
	/// </summary>
	/// <version> 1.0.0 13 Nov 2007 </version>
	public interface IAttributeValueGenerator
	{
		/// <summary>
		/// Generate a value for a simple attribute
		/// </summary>
        /// <param name="id">An unique id provided by the system that may be used as part of the generated value.</param>
        /// <param name="instance">The data instance to be inserted</param>
        /// <param name="metaData">The meta-data of the database</param>
		/// <returns>A generated value</returns>
		string GetValue(string id, IInstanceWrapper instance, MetaDataModel metaData);
	}
}