/*
* @(#) FileStorageProvider.cs
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

	/// <summary>
	/// A implementation of IStorageProvider for file system.
	/// </summary>
	/// <version> 	1.0.0	12 Dec 2006 </version>
	public class FileStorageProvider : IStorageProvider
	{
        private string _xomlFilePath;

        /// <summary>
        /// Initiating an instance of FileStorageProvider class
		/// </summary>
        /// <param name="xomlfilePath">The xoml file path</param>
        public FileStorageProvider(string xomlfilePath)
		{
            _xomlFilePath = xomlfilePath;
		}

        /// <summary>
        /// Gets the information indicating whether the workflow has layout data
        /// </summary>
        public bool HasLayout
        { 
            get
            {
                string layoutFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".layout");
                if (File.Exists(layoutFile))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

       /// <summary>
        /// Gets the information indicating whether the workflow has rules defined
        /// </summary>
        public bool HasRules
        {
            get
            {
                string rulesFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".rules");
                if (File.Exists(rulesFile))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Create an XmlReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForXoml()
        {
            return new XmlTextReader(_xomlFilePath);
        }

        /// <summary>
        /// Create a TextReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForXoml()
        {
            return new StreamReader(_xomlFilePath);
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForXoml()
        {
            return new XmlTextWriter(_xomlFilePath, Encoding.Default);
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForXoml()
        {
            return new StreamWriter(_xomlFilePath);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow layout
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForLayout()
        {
            string layoutFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".layout");

            return XmlReader.Create(layoutFile);
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow layout
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForLayout()
        {
            string layoutFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".layout");

            return new StreamReader(layoutFile);
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForLayout()
        {
            string layoutFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".layout");
            return XmlWriter.Create(layoutFile);
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForLayout()
        {
            string layoutFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".layout");
            return new StreamWriter(layoutFile);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow rules
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForRules()
        {
            string rulesFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".rules");

            return XmlReader.Create(rulesFile);
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow's rules
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForRules()
        {
            string rulesFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".rules");

            return new StreamReader(rulesFile);
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForRules()
        {
            string rulesFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".rules");
            return XmlWriter.Create(rulesFile);
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForRules()
        {
            string rulesFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".rules");
            return new StreamWriter(rulesFile);
        }

        /// <summary>
        /// Create a TextReader from which to read generated code
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForCode()
        {
            string codeBesideFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".cs");
            return new StreamReader(codeBesideFile);
        }

        /// <summary>
        /// Create a TextWriter to which to write generated code
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForCode()
        {
            string codeBesideFile = Path.Combine(Path.GetDirectoryName(_xomlFilePath), Path.GetFileNameWithoutExtension(_xomlFilePath) + ".cs");
            return new StreamWriter(codeBesideFile);
        }
    }
}