/*
* @(#)WindowClientServerProxy.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace SmartExcel
{
	using System;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Principal;
    using SmartExcel.MetaDataWebService;

	/// <summary> 
	/// Windows client side implementation of IServerProxy that uses a web service
	/// for getting user info
	/// </summary>
	/// <version> 1.0.0 25 Sept 2012 </version>
    public class WindowClientServerProxy : IServerProxy
	{
		private MetaDataService _service;

		/// <summary>
		/// Initiate an instance of WindowClientServerProxy class
		/// </summary>
		internal WindowClientServerProxy()
		{
            _service = new MetaDataService();
		}

		#region IUserManager Members

        /// <summary>
        /// Gets file names representing form templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the form templates belong</param>
        /// <param name="className">Indicate the class to which the form templates belong</param>
        /// <returns>An array of form template file names</returns>
        public string[] GetFormTemplatesFileNames(string schemaId, string className)
        {
            return _service.GetFormTemplatesFileNames(schemaId, className);
        }

        /// <summary>
        /// Gets file names representing Word templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the Word templates belong</param>
        /// <param name="className">Indicate the class to which the Word templates belong</param>
        /// <returns>An array of Word template file names</returns>
        public string[] GetReportTemplatesFileNames(string schemaId, string className)
        {
            return _service.GetReportTemplatesFileNames(schemaId, className);
        }

        /// <summary>
        /// Gets workflow names.
        /// </summary>
        /// <param name="projectName">Indicate the project name to which the Workflows belong</param>
        /// <param name="projectVersion">Indicate the project version to which the Workflows belong. Null indicates the latest version</param>
        /// <param name="schemaId">Indicate the schema to which the workflows are bound to</param>
        /// <param name="className">Indicate the class to which the workflows are bound to</param>
        /// <returns>An array of workflow names</returns>
        public string[] GetWorkflowNames(string projectName, string projectVersion, string schemaId, string className)
        {
            return _service.GetWorklowNames(projectName, projectVersion, schemaId, className);
        }

		#endregion
	
	}
}