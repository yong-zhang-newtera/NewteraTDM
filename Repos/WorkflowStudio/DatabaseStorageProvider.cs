/*
* @(#) DatabaseStorageProvider.cs
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
	/// A implementation of IStorageProvider for database storage.
	/// </summary>
	/// <version> 	1.0.0 15 Dec 2006 </version>
	public class DatabaseStorageProvider : IStorageProvider
	{
        private WorkflowModelServiceStub _webService;
        private ProjectModel _projectModel;
        private WorkflowModel _workflowModel = null;

        /// <summary>
        /// Initiating an instance of DatabaseStorageProvider class
		/// </summary>
        /// <param name="webService">The web service to access to database</param>
        /// <param name="projectModel">The project model</param>
        public DatabaseStorageProvider(WorkflowModelServiceStub webService, ProjectModel projectModel)
		{
            _webService = webService;
            _projectModel = projectModel;
		}

        public WorkflowModel WorkflowModel
        {
            get
            {
                return _workflowModel;
            }
            set
            {
                _workflowModel = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the workflow has layout data
        /// </summary>
        public bool HasLayout
        { 
            get
            {
                return _webService.HasLayout(ConnectionStringBuilder.Instance.Create(),
                    _projectModel.Name,
                    _projectModel.Version,
                    _workflowModel.Name);
            }
        }

       /// <summary>
        /// Gets the information indicating whether the workflow has rules defined
        /// </summary>
        public bool HasRules
        {
            get
            {
                return _webService.HasRules(ConnectionStringBuilder.Instance.Create(),
                   _projectModel.Name,
                   _projectModel.Version,
                   _workflowModel.Name);
            }
        }

        /// <summary>
        /// Create an XmlReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForXoml()
        {
            string xoml = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Xoml));

            return new XmlTextReader(new StringReader(xoml));
        }

        /// <summary>
        /// Create a TextReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForXoml()
        {
            string xoml = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Xoml));

            if (xoml != null)
            {
                return new StringReader(xoml);
            }
            else
            {
                throw new InvalidOperationException("Failed to get xoml data for project " + _projectModel.Name + " and workflow " + _workflowModel.Name);
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForXoml()
        {
            throw new InvalidOperationException("Not implemented yet");
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForXoml()
        {
            return new WorkflowWebServiceWriter(_webService, _projectModel,
                _workflowModel, WorkflowDataType.Xoml);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow layout
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForLayout()
        {
            string layout = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Layout));

            if (layout != null)
            {
                return new XmlTextReader(new StringReader(layout));
            }
            else
            {
                return new XmlTextReader(new StringReader(string.Empty));
            }
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow layout
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForLayout()
        {
            string layout = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Layout));

            if (layout != null)
            {
                return new StringReader(layout);
            }
            else
            {
                return new StringReader(string.Empty);
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForLayout()
        {
            throw new InvalidOperationException("Not implemented yet");
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForLayout()
        {
            return new WorkflowWebServiceWriter(_webService, _projectModel,
                            _workflowModel, WorkflowDataType.Layout);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow rules
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForRules()
        {
            string rules = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                           _projectModel.Name,
                           _projectModel.Version,
                           _workflowModel.Name,
                           Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Rules));

            if (rules != null)
            {
                return new XmlTextReader(new StringReader(rules));
            }
            else
            {
                return new XmlTextReader(new StringReader(string.Empty));
            }
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow's rules
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForRules()
        {
            string rules = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Rules));
            if (rules != null)
            {
                return new StringReader(rules);
            }
            else
            {
                return new StringReader(string.Empty);
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForRules()
        {
            throw new InvalidOperationException("Not implemented yet");
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForRules()
        {
            return new WorkflowWebServiceWriter(_webService, _projectModel,
                            _workflowModel, WorkflowDataType.Rules);
        }

        /// <summary>
        /// Create a TextReader from which to read generated code
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForCode()
        {
            string code = _webService.GetWorkflowData(ConnectionStringBuilder.Instance.Create(),
                            _projectModel.Name,
                            _projectModel.Version,
                            _workflowModel.Name,
                            Enum.GetName(typeof(WorkflowDataType), WorkflowDataType.Code));
            if (code != null)
            {
                return new StringReader(code);
            }
            else
            {
                return new StringReader(string.Empty);
            }
        }

        /// <summary>
        /// Create a TextWriter to which to write generated code
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForCode()
        {
            return new WorkflowWebServiceWriter(_webService, _projectModel,
                            _workflowModel, WorkflowDataType.Code);
        }
    }
}