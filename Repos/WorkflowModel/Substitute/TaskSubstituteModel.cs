/*
* @(#) TaskSubstituteModel.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Newtera.WFModel
{
    /// <summary>
    /// Define a model for the information describing the substitutes of user's tasks assigned
    /// by all workflows.
    /// </summary>
    /// <version>1.0.0 25 Dec 2008 </version>
    public class TaskSubstituteModel : WFModelElementBase
    {
        private SubjectEntryCollection _subjects;
        private bool _isLockObtained = false;

        /// <summary>
		/// Initiating a TaskSubstituteModel object
		/// </summary>
        public TaskSubstituteModel() : base()
		{
            _subjects = new SubjectEntryCollection();
		}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.TaskSubstituteModel;
            }
        }

        /// <summary>
        /// Gets the subject entry collection
        /// </summary>
        public SubjectEntryCollection SubjectEntries
        {
            get
            {
                return _subjects;
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
        /// Accept a visitor of IWFModelElementVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        public override void Accept(IWFModelElementVisitor visitor)
        {
            if (visitor.VisitTaskSubstituteModel(this))
            {
                this._subjects.Accept(visitor);
            }
        }

        public void AddSubjectEntry(SubjectEntry subjectEntry)
        {
            _subjects.Add(subjectEntry);
        }

        public void RemoveSubjectEntry(SubjectEntry subjectEntry)
        {
            _subjects.Remove(subjectEntry);
        }

        /// <summary>
        /// Gets a list of substitute users to perform a task owned by an user
        /// </summary>
        /// <param name="user">The original task owner</param>
        /// <param name="projectName">The name of project in question</param>
        /// <param name="projectVersion">The version of the project in question</param>
        /// <returns>A list of substitute users, null if there are no substitute user defined</returns>
        public StringCollection GetSubstituteUsers(string user, string projectName, string projectVersion)
        {
            StringCollection substituteUsers = null;

            SubjectEntry subjectEntry = (SubjectEntry)_subjects[user];
            if (subjectEntry != null)
            {
                // get substitute users defined by the rules under the found subject entry
                substituteUsers = subjectEntry.GetSubstituteUsers(projectName, projectVersion);
            }

            return substituteUsers;
        }

        /// <summary>
        /// Constrauct a project collection from an XML file.
        /// </summary>
        /// <param name="fileName">the name of the XML file</param>
        public void Read(string fileName)
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

        /// <summary>
        /// Constrauct a project from an stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        public void Read(Stream stream)
        {
            if (stream != null && stream.Length > 0)
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(stream);

                // Initializing the objects from the xml document
                Unmarshal(doc.DocumentElement);
            }
        }

        /// <summary>
        /// Constrauct a project from a text reader.
        /// </summary>
        /// <param name="reader">the text reader</param>
        public void Read(TextReader reader)
        {
            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(reader);

                // Initializing the objects from the xml document
                Unmarshal(doc.DocumentElement);
            }
        }

        /// <summary>
        /// Write the project to an XML file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        public void Write(string fileName)
        {
            //Open the stream and read XSD from it.
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                Write(fs);
                fs.Flush();
            }
        }

        /// <summary>
        /// Write the data view as a XML data to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML data</param>
        public void Write(Stream stream)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(stream);
        }

        /// <summary>
        /// Write the data view as a XML data to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        public void Write(TextWriter writer)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(writer);
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            // then a collection of  user entries
            _subjects = (SubjectEntryCollection)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

            // write the _subjects
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_subjects.ElementType));
            _subjects.Marshal(child);
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

            XmlElement element = doc.CreateElement("TaskSubstituteModel");

            doc.AppendChild(element);

            Marshal(element);

            return doc;
        }
    }
}
