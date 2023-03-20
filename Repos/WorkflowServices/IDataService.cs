/*
* @(#)IDataService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Workflow.Runtime;

using Newtera.Common.MetaData.DataView;

namespace Newtera.WorkflowServices
{
    /// <summary>
    /// Define the interface for Newtera Data manipulation service used by workflow runtime
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Execute a search query.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="statement">The query statement</param>
        /// <returns>The search result in dataset</returns>
        DataSet ExecuteQuery(string schemaId, string className, string statement);

        /// <summary>
        /// Execute a non-search query, such as insert, update, and delete
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="statement">The query statement</param>
        /// <returns>The obj_id of the affected data instance</returns>
        string ExecuteNonQuery(string schemaId, string className, string statement);

        /// <summary>
        /// Get an InstanceView from DB given a primary key
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="className">The class name</param>
        /// <param name="pk">The primary key of the instance</param>
        /// <returns>The obj_id of the affected data instance</returns>
        InstanceView GetInstanceView(string schemaId, string className, string pk);
    }
}
