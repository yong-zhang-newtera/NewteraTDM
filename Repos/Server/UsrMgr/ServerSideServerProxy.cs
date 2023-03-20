/*
* @(#)ServerSideServerProxy.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.UsrMgr
{
	using System;
	using System.Data;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Security.Principal;
    using System.Collections.Specialized;
	
	using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Engine.Cache;
    using Newtera.WFModel;
    using Newtera.Server.DB;

	/// <summary> 
	/// An implementation of IServerProxy at server side
	/// <version> 1.0.0 25 Sept 2012 </version>
	public class ServerSideServerProxy : IServerProxy
	{
		/// <summary>
		/// Initiate an instance of ServerSideServerProxy class
		/// </summary>
		public ServerSideServerProxy()
		{
			
		}

        #region IServerProxy Members

        /// <summary>
        /// Gets the information indicating whether the proxy is for server side
        /// </summary>
        public bool IsServerSide
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets file names representing form templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the form templates belong</param>
        /// <param name="className">Indicate the class to which the form templates belong</param>
        /// <returns>An array of form template file names</returns>
        public string[] GetFormTemplatesFileNames(string schemaId, string className)
        {
            string[] files = new string[0];
            string dir = NewteraNameSpace.GetFormTemplateDir(schemaId, className);
            // check if the directry exists
            if (Directory.Exists(dir))
            {
                files = Directory.GetFiles(dir);
            }

            return files;
        }

        /// <summary>
        /// Gets file names representing Word templates.
        /// </summary>
        /// <param name="schemaId">Indicate the schema to which the Word templates belong</param>
        /// <param name="className">Indicate the class to which the Word templates belong</param>
        /// <returns>An array of Word template file names</returns>
        public string[] GetReportTemplatesFileNames(string schemaId, string className)
        {
            string[] files = new string[0];
            string dir = NewteraNameSpace.GetReportTemplateDir(schemaId, className);
            // check if the directry exists
            if (Directory.Exists(dir))
            {
                files = Directory.GetFiles(dir);
            }

            return files;
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
            string[] workflowNames;
            StringCollection workflowNameCollection = new StringCollection();
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            ProjectModel projectModel = WorkflowModelCache.Instance.GetProjectModel(projectName, projectVersion, dataProvider);
            if (projectModel != null)
            {
                foreach (WorkflowModel workflowModel in projectModel.Workflows)
                {
                    if (workflowModel.StartEvent.SchemaID == schemaId &&
                        workflowModel.StartEvent.ClassName == className)
                    {
                        workflowNameCollection.Add(workflowModel.Name);
                    }
                }

                workflowNames = new string[workflowNameCollection.Count];
                for (int i = 0; i < workflowNames.Length; i++)
                {
                    workflowNames[i] = workflowNameCollection[i];
                }
            }
            else
            {
                workflowNames = new String[0];
            }

            return workflowNames;
        }

		#endregion
	}
}