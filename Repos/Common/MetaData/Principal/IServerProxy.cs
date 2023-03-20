/*
* @(#)IServerProxy.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Principal
{
	using System;

	using Newtera.Common.Core;

	/// <summary>
	/// Represents an interface for getting server-side information.
    /// This interface will have different implementation on
	/// the window client side and server side. Client side implementation will access server information via web service
	/// </summary>
	/// <version>  	1.0.0 26 Sept. 2012</version>
	public interface IServerProxy
	{
        /// <summary>
        /// Gets the information indicating whether the proxy is for server side
        /// </summary>
        bool IsServerSide { get; }

        /// <summary>
        /// Gets file names representing form templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the form templates belong</param>
        /// <param name="className">Indicate the class to which the form templates belong</param>
        /// <returns>An array of form template file names</returns>
        string[] GetFormTemplatesFileNames(string schemaId, string className);

        /// <summary>
        /// Gets file names representing Word templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the Word templates belong</param>
        /// <param name="className">Indicate the class to which the Word templates belong</param>
        /// <returns>An array of Word template file names</returns>
        string[] GetReportTemplatesFileNames(string schemaId, string className);

        /// <summary>
        /// Gets workflow names.
        /// </summary>
        /// <param name="projectName">Indicate the project name to which the Workflows belong</param>
        /// <param name="projectVersion">Indicate the project version to which the Workflows belong. Null indicates the latest version</param>
        /// <param name="schemaId">Indicate the schema to which the workflows are bound to</param>
        /// <param name="className">Indicate the class to which the workflows are bound to</param>
        /// <returns>An array of workflow names</returns>
        string[] GetWorkflowNames(string projectName, string projectVersion, string schemaId, string className);
	}
}