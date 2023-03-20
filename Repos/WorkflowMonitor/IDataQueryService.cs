/*
* @(#)IDataQueryService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

using Newtera.WFModel;

namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// Provides Data Instance related query services
    /// </summary>
    /// <version> 1.0.0 08 Aug 2007</version>
    public interface IDataQueryService
    {
        /// <summary>
        /// Execute a query and return the result in XmlDocument
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The query result in XmlNode</returns>
        XmlNode ExecuteQuery(string connectionStr, string query);
    }
}
