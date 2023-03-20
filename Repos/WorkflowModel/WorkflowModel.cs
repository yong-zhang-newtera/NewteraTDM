/*
* @(#)WorkflowModel.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
    using System.IO;
    using System.Text;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Serialization;

	/// <summary>
	/// Describing a workflow basic information.
	/// </summary>
	/// <version>1.0.6 8 Dec 2006</version>
	public class WorkflowModel : WFModelElementBase
	{
        private EventInfo _startEvent;
        private WorkflowType _workflowType;
        private string _workflowClass;
        private Activity _rootActivity; // run-time value
        private string _xomlFileBaseDir = null; // run-time value
        private IStorageProvider _storageProvider; // run-time value
        private IStorageProvider _databaseStorageProvider = null; // run-time value
        private StorageType _sourceStorageType = StorageType.Unknown; // run-time value
        private bool _isLoaded; // run-time, true if the workflow data has been loaded in the memory

		/// <summary>
		/// Initiating an instance of WorkflowModel class
		/// </summary>
        /// <param name="name">Name of the workflow.</param>
		public WorkflowModel(string name) : base(name)
		{
            _startEvent = new EventInfo();
            _workflowType = WorkflowType.Sequential;
            _workflowClass = GetWorkflowClassName();
            _storageProvider = StorageProviderFactory.Instance.CreateMemoryProvider();
            _isLoaded = true;
		}

        /// <summary>
        /// Initiating an instance of WorkflowModel class
        /// </summary>
        /// <param name="name">Name of the workflow.</param>
        /// <param name="workflowType">One of the WorkflowType enum.</param>
        public WorkflowModel(string name, WorkflowType workflowType)
            : base(name)
        {
            _startEvent = new EventInfo();
            _workflowType = workflowType;
            _workflowClass = GetWorkflowClassName();
            _storageProvider = StorageProviderFactory.Instance.CreateMemoryProvider();
            _isLoaded = true;
        }

		/// <summary>
		/// Initiating an instance of WorkflowModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal WorkflowModel(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

            _storageProvider = StorageProviderFactory.Instance.CreateMemoryProvider();
            _isLoaded = false;
		}

        /// <summary>
        /// Gets the event that starts the workflow
        /// </summary>
        public EventInfo StartEvent
        {
            get
            {
                return _startEvent;
            }
        }

        /// <summary>
        /// Gets type of the workflow
        /// </summary>
        /// <value>One of WorkflowType values</value>
        public WorkflowType WorkflowType
        {
            get
            {
                return _workflowType;
            }
        }

        /// <summary>
        /// Gets unique class name of the workflow
        /// </summary>
        public string WorkflowClass
        {
            get
            {
                return _workflowClass;
            }
        }

        /// <summary>
        /// Gets a qualified class name
        /// </summary>
        public string QualifiedWorkflowClass
        {
            get
            {
                return @"Newtera.Workflow." + _workflowClass;
            }
        }

        /// <summary>
        /// Gets or sets the root activity of the workflow
        /// </summary>
        public Activity RootActivity
        {
            get
            {
                return _rootActivity;
            }
            set
            {
                _rootActivity = value;
            }
        }

        /// <summary>
        /// Gets an unique name for saving xoml file.
        /// </summary>
        public string XomlFileName
        {
            get
            {
                return _workflowClass + @".xoml";
            }
        }

        /// <summary>
        /// Gets or sets base directory for xoml file.
        /// </summary>
        public string XomlFileBaseDir
        {
            get
            {
                return _xomlFileBaseDir;
            }
            set
            {
                _xomlFileBaseDir = value;
            }
        }

        /// <summary>
        /// Gets a full file path for saving xoml file.
        /// </summary>
        public string XomlFilePath
        {
            get
            {
                if (_xomlFileBaseDir == null)
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.XomlFileName);
                }
                else
                {
                    return Path.Combine(_xomlFileBaseDir, this.XomlFileName);
                }
            }
        }

        /// <summary>
        /// Gets or sets type of workflow data source.
        /// </summary>
        public StorageType SourceStorageType
        {
            get
            {
                return _sourceStorageType;
            }
            set
            {
                _sourceStorageType = value;
            }
        }

        /// <summary>
        /// Gets or sets the database storage provider
        /// </summary>
        public IStorageProvider DatabaseStorageProvider
        {
            get
            {
                return _databaseStorageProvider;
            }
            set
            {
                _databaseStorageProvider = value;
            }
        }

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.Workflow;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the workflow has layout data
        /// </summary>
        public bool HasLayout
        {
            get
            {
                if (!_isLoaded)
                {
                    LoadWorkflowData();
                    _isLoaded = true;
                }

                return _storageProvider.HasLayout;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the workflow has rules defined
        /// </summary>
        public bool HasRules
        {
            get
            {
                if (!_isLoaded)
                {
                    LoadWorkflowData();
                    _isLoaded = true;
                }

                return _storageProvider.HasRules;
            }
        }

        /// <summary>
        /// Load workflow data into memory
        /// </summary>
        public void LoadData()
        {
            if (!this._isLoaded)
            {
                LoadWorkflowData();
                _isLoaded = true;
            }
        }

        /// <summary>
        /// Create an XmlReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXomlReader()
        {
            if (!_isLoaded)
            {
                LoadWorkflowData();
                _isLoaded = true;
            }

            return _storageProvider.CreateXmlReaderForXoml();
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXomlWriter()
        {
            return _storageProvider.CreateXmlWriterForXoml();
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow layout
        /// </summary>
        /// <returns></returns>
        public XmlReader CreateLayoutReader()
        {
            if (!_isLoaded)
            {
                LoadWorkflowData();
                _isLoaded = true;
            }

            return _storageProvider.CreateXmlReaderForLayout();
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's layout
        /// </summary>
        /// <returns></returns>
        public XmlWriter CreateLayoutWriter()
        {
            return _storageProvider.CreateXmlWriterForLayout();
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow's rules
        /// </summary>
        /// <returns></returns>
        public XmlReader CreateRulesXmlReader()
        {
            if (!_isLoaded)
            {
                LoadWorkflowData();
                _isLoaded = true;
            }

            return _storageProvider.CreateXmlReaderForRules();
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow's rules
        /// </summary>
        /// <returns></returns>
        public TextReader CreateRulesTextReader()
        {
            if (!_isLoaded)
            {
                LoadWorkflowData();
                _isLoaded = true;
            }

            return _storageProvider.CreateTextReaderForRules();
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's rules
        /// </summary>
        /// <returns></returns>
        public TextWriter CreateRulesTextWriter()
        {
            return _storageProvider.CreateTextWriterForRules();
        }

        /// <summary>
        /// Create a TextWriter to which to write generated code
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateCodeWriter()
        {
            return _storageProvider.CreateTextWriterForCode();
        }

        /// <summary>
        /// Create am activity tree from the xoml and return the root activity
        /// </summary>
        /// <returns>The root activity of the tree.</returns>
        public Activity CreateRootActivity()
        {
            XmlReader reader = CreateXomlReader();
            Activity rootActivity = null;
            try
            {
                WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
                object obj = xomlSerializer.Deserialize(reader);
                rootActivity = obj as Activity;
                //rootActivity = xomlSerializer.Deserialize(reader) as Activity;
            }
            catch (Exception)
            {
                // do nothing
            }
            finally
            {
                reader.Close();
            }
            return rootActivity;
        }

        /// <summary>
        /// Save the workflow's xoml, rules, and layout as files under the given directory
        /// </summary>
        /// <param name="baseDirPath">The base directory</param>
        public void Save(string baseDirPath)
        {
            string xomlFilePath = Path.Combine(baseDirPath, this.XomlFileName);
            IStorageProvider fileProvider = StorageProviderFactory.Instance.CreateFileProvider(xomlFilePath);

            DumpWorkflowData(fileProvider);
        }

        /// <summary>
        /// Save the workflow's xoml, rules, and layout as files to the given storage provider
        /// </summary>
        /// <param name="storageProvider">A IStorageProvider</param>
        public void Save(IStorageProvider storageProvider)
        {
            DumpWorkflowData(storageProvider);
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			visitor.VisitWorkflowModel(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            string str = parent.GetAttribute("Type");
            if (str != null)
            {
                this._workflowType = (WorkflowType)Enum.Parse(typeof(WorkflowType), str);
            }
            else
            {
                this._workflowType = WorkflowType.Sequential; // Default type
            }

            _workflowClass = parent.GetAttribute("Cls");

            // start event
            _startEvent = new EventInfo();
            str = parent.GetAttribute("schemaId");
            if (str != null)
            {
                _startEvent.SchemaID = str;
            }

            str = parent.GetAttribute("clsName");
            if (str != null)
            {
                _startEvent.ClassName = str;
            }

            str = parent.GetAttribute("clsCaption");
            if (str != null)
            {
                _startEvent.ClassCaption = str;
            }

            str = parent.GetAttribute("eventName");
            if (str != null)
            {
                _startEvent.EventName = str;
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("Type", Enum.GetName(typeof(WorkflowType), this._workflowType));

            parent.SetAttribute("Cls", _workflowClass);

            if (_startEvent.SchemaID != null)
            {
                parent.SetAttribute("schemaId", _startEvent.SchemaID);
            }

            if (_startEvent.ClassName != null)
            {
                parent.SetAttribute("clsName", _startEvent.ClassName);
            }

            if (_startEvent.ClassCaption != null)
            {
                parent.SetAttribute("clsCaption", _startEvent.ClassCaption);
            }

            if (_startEvent.EventName != null)
            {
                parent.SetAttribute("eventName", _startEvent.EventName);
            }
		}

        /// <summary>
        /// Gets a workflow class that is unique among all workflows
        /// </summary>
        /// <returns>An unique class name</returns>
        private string GetWorkflowClassName()
        {
            int hashCode = Guid.NewGuid().GetHashCode();

            if (hashCode < 0)
            {
                hashCode = hashCode * -1; // make it positive
            }

            return "WF" + hashCode;
        }

        /// <summary>
        /// Dump the workflow data to a target storage, whether it is a file system or
        /// database.
        /// </summary>
        /// <param name="targetStorageProvider">The target storage provider.</param>
        private void DumpWorkflowData(IStorageProvider targetStorageProvider)
        {
            if (_isLoaded)
            {
                // save the xoml file
                TextReader xomlReader = _storageProvider.CreateTextReaderForXoml(); // read from memory
                TextWriter xomlWriter = targetStorageProvider.CreateTextWriterForXoml(); // write to target
                try
                {
                    xomlWriter.Write(xomlReader.ReadToEnd());
                }
                finally
                {
                    xomlReader.Close();
                    xomlWriter.Close();
                }

                // save the rules file
                if (HasRules)
                {
                    TextReader rulesReader = _storageProvider.CreateTextReaderForRules(); // read from memory
                    TextWriter rulesWriter = targetStorageProvider.CreateTextWriterForRules(); // write to file
                    try
                    {
                        rulesWriter.Write(rulesReader.ReadToEnd());
                    }
                    finally
                    {
                        rulesReader.Close();
                        rulesWriter.Close();
                    }
                }

                // save the code file
                TextReader codeReader = _storageProvider.CreateTextReaderForCode(); // read from memory
                TextWriter codeWriter = targetStorageProvider.CreateTextWriterForCode(); // write to file
                try
                {
                    codeWriter.Write(codeReader.ReadToEnd());
                }
                finally
                {
                    codeReader.Close();
                    codeWriter.Close();
                }

                // Need to save the layout in case of State Machine Workflow
                if (this.HasLayout)
                {
                    TextReader layoutReader = _storageProvider.CreateTextReaderForLayout(); // read from memory
                    TextWriter layoutWriter = targetStorageProvider.CreateTextWriterForLayout(); // write to file
                    try
                    {
                        layoutWriter.Write(layoutReader.ReadToEnd());
                    }
                    finally
                    {
                        layoutReader.Close();
                        layoutWriter.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Load the workflow data into the memory
        /// </summary>
        private void LoadWorkflowData()
        {
            IStorageProvider sourceProvider = null;

            if (_sourceStorageType == StorageType.File && _xomlFileBaseDir != null)
            {
                // load the data from file
                sourceProvider = StorageProviderFactory.Instance.CreateFileProvider(this.XomlFilePath);
            }
            else if (_sourceStorageType == StorageType.Database && _databaseStorageProvider != null)
            {
                sourceProvider = _databaseStorageProvider;
            }

            if (sourceProvider != null)
            {
                // load xoml data
                TextReader xomlReader = sourceProvider.CreateTextReaderForXoml(); // read from memory
                TextWriter xomlWriter = _storageProvider.CreateTextWriterForXoml(); // write to file
                try
                {
                    string xoml = xomlReader.ReadToEnd();

                    if (xoml != null && xoml.Length > 0)
                    {
                        xomlWriter.Write(xoml);
                    }
                    else
                    {
                        throw new Exception("empty xoml string.");
                    }
                }
                finally
                {
                    xomlReader.Close();
                    xomlWriter.Close();
                }

                // load the rules file
                if (sourceProvider.HasRules)
                {
                    TextReader rulesReader = sourceProvider.CreateTextReaderForRules(); // read from memory
                    TextWriter rulesWriter = _storageProvider.CreateTextWriterForRules(); // write to file
                    try
                    {
                        rulesWriter.Write(rulesReader.ReadToEnd());
                    }
                    finally
                    {
                        rulesReader.Close();
                        rulesWriter.Close();
                    }
                }

                // save the code file
                TextReader codeReader = sourceProvider.CreateTextReaderForCode(); // read from memory
                TextWriter codeWriter = _storageProvider.CreateTextWriterForCode(); // write to file
                try
                {
                    codeWriter.Write(codeReader.ReadToEnd());
                }
                finally
                {
                    codeReader.Close();
                    codeWriter.Close();
                }

                // Need to save the layout in case of State Machine Workflow
                if (sourceProvider.HasLayout)
                {
                    TextReader layoutReader = sourceProvider.CreateTextReaderForLayout(); // read from memory
                    TextWriter layoutWriter = _storageProvider.CreateTextWriterForLayout(); // write to file
                    try
                    {
                        layoutWriter.Write(layoutReader.ReadToEnd());

                    }
                    finally
                    {
                        layoutReader.Close();
                        layoutWriter.Close();
                    }
                }
            }
        }
	}
}