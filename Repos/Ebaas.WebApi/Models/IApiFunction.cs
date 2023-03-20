/*
* @(#)IApiFunction.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Models
{
	using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Newtonsoft.Json.Linq;
    using System.Collections.Specialized;

    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Data;

    /// <summary>
    /// Represents an interface for an api
    /// </summary>
    /// <version> 1.0.0 4 Nov 2015 </version>
    public interface IApiFunction
	{
		/// <summary>
		/// Execute an api function
		/// </summary>
        /// <param name="context">The api execution context</param>
		/// <returns>A string return from api function</returns>
		JObject Execute(ApiExecutionContext context);
	}

    /// <summary>
    /// Execution context for external api execute method
    /// </summary>
    public class ApiExecutionContext
    {
        /// <summary>
        /// Get or set the dataview model
        /// </summary>
        public DataViewModel DataView { get; set; }

        /// <summary>
        /// Database schema id
        /// </summary>
        public string SchemaId { get; set; }

        /// <summary>
        /// Database class name
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Obj_id of the instance, optional
        /// </summary>
        public string ObjID { get; set; }
        /// <summary>
        /// The instance data for update or create method, optional
        /// </summary>
        public dynamic InstanceData { get; set; }

        /// <summary>
        /// The parameters of the method
        /// </summary>
        public NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Database connection
        /// </summary>
        public CMConnection Connection { get; set; }

        /// <summary>
        /// The type of the instanceData, for example, application/json (default) or application/xml
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The type of the returned data, for example, application/json (default) or application/xml
        /// </summary>
        public string AcceptType { get; set; }
    }
}