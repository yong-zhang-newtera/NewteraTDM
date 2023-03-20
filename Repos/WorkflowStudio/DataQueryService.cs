/*
* @(#)DataQueryService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;
    using Newtera.WinClientCommon;
    using Newtera.WorkflowMonitor;

	/// <summary>
	/// providing service for validating properties of custom activities.
	/// </summary>
	/// <version>1.0.0 08 Aug 2007</version>
	public class DataQueryService : IDataQueryService
	{
        public DataQueryService()
        {
        }

        /// <summary>
        /// Execute a query and return the result in XmlDocument
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The query result in XmlNode</returns>
        public XmlNode ExecuteQuery(string connectionStr, string query)
        {
            CMDataServiceStub service = new CMDataServiceStub();

            return service.ExecuteQuery(connectionStr, query);
        }
	}
}