/*
* @(#) WFProject.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;

using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData;

namespace Newtera.WFModel
{
    /// <summary>
    /// Representing a workflow project that contains workflows of a specific
    /// project. 
    /// </summary>
    /// <version>1.0.0 8 Dec 2006 </version>
    public class ProjectModel : WFModelElementBase, IXaclObject
    {
        private const string DefaultVersion = "1.0";

        private WorkflowModelCollection _workflows;
        private bool _isLockObtained = false;
        private bool _needToSave = false;
        private DateTime _modifiedTime;
        private bool _isLoadedFromDB = false;
        private XaclPolicy _xaclPolicy;
        private string _version;

        private string _xpath; // run-time use only

        /// <summary>
		/// Initiating a ProjectModel object
		/// </summary>
        public ProjectModel(string name) : base(name)
		{
            _workflows = new WorkflowModelCollection();

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _workflows.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

            _xaclPolicy = new XaclPolicy();

            _version = null;
		}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.Project;
            }
        }

        /// <summary>
        /// Gets or sets the xacl policy of the project
        /// </summary>
        public XaclPolicy Policy
        {
            get
            {
                return _xaclPolicy;
            }
            set
            {
                _xaclPolicy = value;
            }
        }

        /// <summary>
        /// Gets the workflow collection managed by the project.
        /// </summary>
        public WorkflowModelCollection Workflows
        {
            get
            {
                return _workflows;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a lock to the project
        /// at server side has been obtained.
        /// </summary>
        public bool IsLockObtained
        {
            get
            {
                return _isLockObtained;
            }
            set
            {
                _isLockObtained = value;
            }
        }

        /// <summary>
        /// Gets or sets the modified time of the project model.
        /// </summary>
        public DateTime ModifiedTime
        {
            get
            {
                return _modifiedTime;
            }
            set
            {
                _modifiedTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the project model needs to be saved
        /// in files.
        /// </summary>
        public bool NeedToSave
        {
            get
            {
                return _needToSave;
            }
            set
            {
                _needToSave = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of the project .
        /// </summary>
        public string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    return DefaultVersion;
                }
                else
                {
                    return _version;
                }
            }
            set
            {
                _version = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the project model is loaded from database.
        /// </summary>
        public bool IsLoadedFromDB
        {
            get
            {
                return _isLoadedFromDB;
            }
            set
            {
                _isLoadedFromDB = value;
            }
        }

        /// <summary>
        /// Clear the project id and workflow ids
        /// </summary>
        public void ClearIds()
        {
            this.ID = null;
            foreach (WorkflowModel workflow in this.Workflows)
            {
                workflow.ID = null;
            }
        }

        /// <summary>
        /// Accept a visitor of IWFModelElementVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public override void Accept(IWFModelElementVisitor visitor)
        {
            if (visitor.VisitProjectModel(this))
            {
                this._workflows.Accept(visitor);
            }
        }

        /// <summary>
        /// Load workflow data into memory for all WorkflowModel instances in the project
        /// </summary>
        public void LoadAll()
        {
            foreach (WorkflowModel workflowModel in this._workflows)
            {
                workflowModel.LoadData();
            }
        }

        public void AddWorkflowModel(WorkflowModel model)
        {
            _workflows.Add(model);
        }

        /// <summary>
        /// Constrauct a project collection from an XML file.
        /// </summary>
        /// <param name="fileName">the name of the XML file</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// read the XML file
        /// </exception>
        public void Read(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    //Open the stream and read XSD from it.
                    using (FileStream fs = File.OpenRead(fileName))
                    {
                        Read(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new WFModelException("Failed to read the file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Constrauct a project from an stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// read the stream
        /// </exception>
        public void Read(Stream stream)
        {
            if (stream != null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    doc.Load(stream);

                    // Initializing the objects from the xml document
                    Unmarshal(doc.DocumentElement);
                }
                catch (Exception e)
                {
                    throw new WFModelException(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Constrauct a project from a text reader.
        /// </summary>
        /// <param name="reader">the text reader</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// read the text reader
        /// </exception>
        public void Read(TextReader reader)
        {
            if (reader != null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    doc.Load(reader);

                    // Initializing the objects from the xml document
                    Unmarshal(doc.DocumentElement);
                }
                catch (Exception e)
                {
                    throw new WFModelException(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Write the project to an XML file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// write to the file.
        /// </exception> 
        public void Write(string fileName)
        {
            try
            {
                //Open the stream and read XSD from it.
                using (FileStream fs = File.Open(fileName, FileMode.Create))
                {
                    Write(fs);
                    fs.Flush();
                }
            }
            catch (System.IO.IOException ex)
            {
                throw new WFModelException("Failed to write to file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Write the data view as a XML data to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML data</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// write to the stream.
        /// </exception>
        public void Write(Stream stream)
        {
            try
            {
                XmlDocument doc = GetXmlDocument();

                doc.Save(stream);
            }
            catch (System.IO.IOException ex)
            {
                throw new WFModelException("Failed to write the SchemaModel object", ex);
            }
        }

        /// <summary>
        /// Write the data view as a XML data to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
        /// write to the stream.
        /// </exception>
        public void Write(TextWriter writer)
        {
            try
            {
                XmlDocument doc = GetXmlDocument();

                doc.Save(writer);
            }
            catch (System.IO.IOException ex)
            {
                throw new WFModelException("Failed to write the SchemaModel object", ex);
            }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            string val = parent.GetAttribute("modified_time");
            if (!string.IsNullOrEmpty(val))
            {
                _modifiedTime = DateTime.Parse(val);
            }

            val = parent.GetAttribute("version");
            if (!string.IsNullOrEmpty(val))
            {
                _version = val;
            }

            // then a collection of  workflow models
            _workflows = (WorkflowModelCollection)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

            if (_modifiedTime != null)
            {
                parent.SetAttribute("modified_time", _modifiedTime.ToString("s"));
            }

            if (!string.IsNullOrEmpty(_version))
            {
                parent.SetAttribute("version", _version);
            }

            // write the _workflows
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_workflows.ElementType));
            _workflows.Marshal(child);
            parent.AppendChild(child);
        }

        /// <summary>
        /// Gets the xml document that represents the project
        /// </summary>
        /// <returns>A XmlDocument instance</returns>
        private XmlDocument GetXmlDocument()
        {
            // Marshal the objects to xml document
            XmlDocument doc = new XmlDocument();

            XmlElement element = doc.CreateElement("WFProject");

            doc.AppendChild(element);

            Marshal(element);

            return doc;
        }

        /// <summary>
        /// Handler for Value Changed event fired by members of a schema model element
        /// </summary>
        /// <param name="sender">The element that fires the event</param>
        /// <param name="e">The event arguments</param>
        protected override void ValueChangedHandler(object sender, EventArgs e)
        {
            this._needToSave = true;
            this.IsAltered = true;
        }

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the Project node
        /// </summary>
        /// <returns>a xapth representation</returns>
        public string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Name;
            }

            return _xpath;
        }

        /// <summary>
        /// Return a  parent of the Project node
        /// </summary>
        /// <returns>Null</returns>
        public IXaclObject Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the Taxonomy node
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public IEnumerator GetChildren()
        {
            return this._workflows.GetEnumerator();
        }

        #endregion
    }
}
