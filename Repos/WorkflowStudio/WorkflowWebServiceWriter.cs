/*
* @(#) WorkflowWebServiceWriter.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
    using System.Xml;
    using System.IO;
	using System.Text;

    using Newtera.WinClientCommon;
    using Newtera.WFModel;

	/// <summary>
	/// Implement a TextWriter that writes data to the WorkflowModelWebService
	/// </summary>
	/// <version> 	1.0.0 16 Dec 2006 </version>
	public class WorkflowWebServiceWriter : TextWriter
	{
        private WorkflowModelServiceStub _webService;
        private ProjectModel _projectModel;
        private WorkflowModel _workflowModel = null;
        private WorkflowDataType _dataType = WorkflowDataType.Unknown;

        /// <summary>
        /// Initiating an instance of WorkflowWebServiceWriter class
		/// </summary>
        /// <param name="webService">The web service to access to database</param>
        /// <param name="projectModel">The project model</param>
        public WorkflowWebServiceWriter(WorkflowModelServiceStub webService, ProjectModel projectModel,
            WorkflowModel workflowModel, WorkflowDataType dataType)
		{
            _webService = webService;
            _projectModel = projectModel;
            _workflowModel = workflowModel;
            _dataType = dataType;
		}

        /// <summary>
        /// override one of Write method that takes a string and write it to web service
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            _webService.SetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                _projectModel.Name,
                _projectModel.Version,
                _workflowModel.Name,
                Enum.GetName(typeof(WorkflowDataType), _dataType),
                value);
        }

        /// <summary>
        /// Gets the encoding
        /// </summary>
        public override Encoding Encoding
        {
            get
            { 
                return Encoding.Default; 
            }
        }
    }
}