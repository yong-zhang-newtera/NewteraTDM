/*
* @(#) IStorageProvider.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;
    using System.IO;
	using System.Data;

	/// <summary>
	/// A interface for workflow storage provider.
	/// </summary>
	/// <version> 	1.0.0	12 Dec 2006 </version>
	public interface IStorageProvider
	{
        /// <summary>
        /// Gets the information indicating whether the workflow has layout data
        /// </summary>
        bool HasLayout { get; }

       /// <summary>
        /// Gets the information indicating whether the workflow has rules defined
        /// </summary>
        bool HasRules { get; }

        /// <summary>
        /// Create an XmlReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>XmlReader instance</returns>
        XmlReader CreateXmlReaderForXoml();

        /// <summary>
        /// Create a TextReader from which to read Workflow's XOML
        /// </summary>
        /// <returns>TextReader instance</returns>
        TextReader CreateTextReaderForXoml();

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        XmlWriter CreateXmlWriterForXoml();

        /// <summary>
        /// Create a TextWriter to which to write Workflow's XOML
        /// </summary>
        /// <returns>TextWriter instance</returns>
        TextWriter CreateTextWriterForXoml();

        /// <summary>
        /// Create a XmlReader from which to read the workflow layout
        /// </summary>
        /// <returns>XmlReader instance</returns>
        XmlReader CreateXmlReaderForLayout();

        /// <summary>
        /// Create a TextReader from which to read the workflow layout
        /// </summary>
        /// <returns>TextReader instance</returns>
        TextReader CreateTextReaderForLayout();

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        XmlWriter CreateXmlWriterForLayout();

        /// <summary>
        /// Create a TextWriter to which to write Workflow's layout
        /// </summary>
        /// <returns>TextWriter instance</returns>
        TextWriter CreateTextWriterForLayout();

        /// <summary>
        /// Create a XmlReader from which to read the workflow rules
        /// </summary>
        /// <returns>XmlReader instance</returns>
        XmlReader CreateXmlReaderForRules();

        /// <summary>
        /// Create a TextReader from which to read the workflow's rules
        /// </summary>
        /// <returns>TextReader instance</returns>
        TextReader CreateTextReaderForRules();

        /// <summary>
        /// Create an XmlWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>XmlWriter instance</returns>
        XmlWriter CreateXmlWriterForRules();

        /// <summary>
        /// Create an TextWriter to which to write Workflow's rules
        /// </summary>
        /// <returns>TextWriter instance</returns>
        TextWriter CreateTextWriterForRules();

        /// <summary>
        /// Create a TextReader from which to read generated code
        /// </summary>
        /// <returns>TextReader instance</returns>
        TextReader CreateTextReaderForCode();

        /// <summary>
        /// Create a TextWriter to which to write generated code
        /// </summary>
        /// <returns>TextWriter instance</returns>
        TextWriter CreateTextWriterForCode();
	}
}