/*
* @(#) MemoryStorageProvider.cs
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
	/// A implementation of IStorageProvider for storing workflow data in memory.
	/// </summary>
	/// <version>1.0.0 12 Dec 2006 </version>
	public class MemoryStorageProvider : IStorageProvider
	{
        private StringBuilder _xoml = null;
        private StringBuilder _layout = null;
        private StringBuilder _rules = null;
        private MemoryStream _codes = null;

        /// <summary>
        /// Initiating an instance of MemoryStorageProvider class
		/// </summary>
        public MemoryStorageProvider()
		{
		}

        /// <summary>
        /// Gets the information indicating whether the workflow has layout data
        /// </summary>
        public bool HasLayout
        { 
            get
            {
                if (_layout != null)
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
                if (_rules != null)
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
            if (_xoml != null)
            {
                return new XmlTextReader(new StringReader(_xoml.ToString()));
            }
            else
            {
                throw new InvalidOperationException("no xoml string exists.");
            }
        }

        /// <summary>
        /// Create a TextReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForXoml()
        {
            if (_xoml != null)
            {
                return new StringReader(_xoml.ToString());
            }
            else
            {
                throw new InvalidOperationException("no xoml string exists.");
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForXoml()
        {
            _xoml = new StringBuilder();
            return new XmlTextWriter(new StringWriter(_xoml));
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForXoml()
        {
            _xoml = new StringBuilder();
            return new StringWriter(_xoml);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow layout
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForLayout()
        {
            if (_layout != null)
            {
                return new XmlTextReader(new StringReader(_layout.ToString()));
            }
            else
            {
                throw new InvalidOperationException("no layout string exists.");
            }
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow layout
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForLayout()
        {
            if (_layout != null)
            {
                return new StringReader(_layout.ToString());
            }
            else
            {
                throw new InvalidOperationException("no layout string exists.");
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForLayout()
        {
            _layout = new StringBuilder();
            return new XmlTextWriter(new StringWriter(_layout));
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForLayout()
        {
            _layout = new StringBuilder();
            return new StringWriter(_layout);
        }

        /// <summary>
        /// Create a XmlReader from which to read the workflow rules
        /// </summary>
        /// <returns>XmlReader instance</returns>
        public XmlReader CreateXmlReaderForRules()
        {
            if (_rules != null)
            {
                return new XmlTextReader(new StringReader(_rules.ToString()));
            }
            else
            {
                throw new InvalidOperationException("no rules exists.");
            }
        }

        /// <summary>
        /// Create a TextReader from which to read the workflow's rules
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForRules()
        {
            if (_rules != null)
            {
                return new StringReader(_rules.ToString());
            }
            else
            {
                throw new InvalidOperationException("no rules string exists.");
            }
        }

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        public XmlWriter CreateXmlWriterForRules()
        {
            _rules = new StringBuilder();
            return new XmlTextWriter(new StringWriter(_rules));
        }

        /// <summary>
        /// Create a TextWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForRules()
        {
            _rules = new StringBuilder();
            return new StringWriter(_rules);
        }

        /// <summary>
        /// Create a TextReader from which to read generated code
        /// </summary>
        /// <returns>TextReader instance</returns>
        public TextReader CreateTextReaderForCode()
        {
            if (_codes != null)
            {
                return new StreamReader(new MemoryStream(_codes.ToArray()));
            }
            else
            {
                throw new InvalidOperationException("no code string exists.");
            }
        }

        /// <summary>
        /// Create a TextWriter to which to write generated code
        /// </summary>
        /// <returns>TextWriter instance</returns>
        public TextWriter CreateTextWriterForCode()
        {
            _codes = new MemoryStream();
            return new StreamWriter(_codes);
        }
	}
}