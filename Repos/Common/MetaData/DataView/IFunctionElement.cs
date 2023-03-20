/*
* @(#)IFunctionElement.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Collections;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents an interface for external functions used in XQuery
	/// </summary>
	/// <version>  	1.0.0 15 Oct 2007</version>
	public interface IFunctionElement
	{
		/// <summary>
		/// Gets returned data type of the function.
		/// </summary>
		/// <returns>One of the DataType enum</returns>
        DataType DataType { get; set;}

        /// <summary>
        /// Gets or sets schema name of a data instance as function parameter, can be null
        /// </summary>
        string SchemaName { get; set;}

        /// <summary>
        /// Gets or sets schema version of a data instance as function parameter, can be null
        /// </summary>
        string SchemaVersion { get; set;}

        /// <summary>
        /// Gets or sets class name of a data instance as function parameter, can be null
        /// </summary>
        string ClassName { get; set;}

        /// <summary>
        /// Gets or sets attribute name of a data instance as function parameter, can be null
        /// </summary>
        string AttributeName { get; set;}

        /// <summary>
        /// Gets or sets attribute caption of a data instance as function parameter, can be null
        /// </summary>
        string AttributeCaption { get; set;}

        /// <summary>
        /// Gets or sets a data instance id as function parameter, can be null
        /// </summary>
        string ObjId { get; set;}
	}
}