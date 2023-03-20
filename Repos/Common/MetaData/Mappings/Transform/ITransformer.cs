/*
* @(#)ITransformer.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Collections.Specialized;
    using System.Data;

	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// Represents a interface for a transformer that transform
	/// data from source to destination
	/// </summary>
	/// <version> 1.0.0 22 Nov 2004</version>
	/// <author>  Yong Zhang </author>
	public interface ITransformer
	{
		/// <summary>
		/// Gets the type of transformer.
		/// </summary>
		/// <value>One of the NodeType enum values</value>
		NodeType TransformType { get;}

        /// <summary>
        /// Gets or sets the DataTable object that represents the source data to be transformed
        /// </summary>
        DataTable SourceDataTable { get; set;}

		/// <summary>
		/// Transform a collection of source values to a collection of destination
		/// values.
		/// </summary>
		/// <param name="srcValues">A collection of name/value pairs representing source values.</param>
		/// <param name="dstValues">A collection of name/value pairs that contains transformed destination values.</param>
		void Transform(NameValueCollection srcValues, NameValueCollection dstValues);
	}
}