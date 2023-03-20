/*
* @(#) IDataInstanceGetter.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Data;
    using System.Xml;
    using System.Threading;
    using System.Security.Principal;

    using Newtera.Common.Core;
    using Newtera.Common.Wrapper;

	/// <summary>
    /// Represents an interface for a getter that retrieve a data instance from a specific data source
	/// <version> 	1.0.0	15 June 2014 </version>
    public interface IDataInstanceGetter
	{
        /// <summary>
        /// Get a data instance given the data type and attribute value
        /// </summary>
        /// <param name="schemaId">Id of the database schema</param>
        /// <param name="className">Class name to which the instance belongs</param>
        /// <param name="attributeName">A name of the attribute whose value uniquely identifies an instance</param>
        /// <param name="attributeValue"> The attribute's value uniquely indentify the data instance</param>
        /// <returns>A data instance in IInstanceWrapper type, null if not found</returns>
        IInstanceWrapper GetInstance(string schemaId, string className, string attributeName, string attributeValue);
	}
}
