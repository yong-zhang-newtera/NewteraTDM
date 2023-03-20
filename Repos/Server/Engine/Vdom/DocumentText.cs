/*
* @(#)DocumentText.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
    using System.IO;
	using System.Collections;
	using System.Xml;

    using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
    using Newtera.Server.Engine.Sqlbuilder;

    /// <summary>
    /// Implementation of text XML document.
    /// </summary>
    /// <version>  	1.0.0 11 Jul 2003</version>
    /// <author>  Yong Zhang </author>
    public class DocumentText : VDocument
	{		
		/// <summary>
		/// Initializing a DocumentText object
		/// </summary>
		public DocumentText() : base()
		{
		}

		/// <summary>
		/// Return a data provider associated with the document.
		/// </summary>
		/// <value>null object</value>
		public override IDataProvider DataProvider
		{
			get
			{
				return null;
			}
		}
		
		/// <summary>
		/// Gets the information indicating whether the document is a db virtual document
		/// </summary>
		/// <returns>true if it is, false otherwise</returns>
		public override bool IsDB
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets information indicating whether a xml node represents a virtual attribute of a class
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>false always</returns>
        public override bool IsVirtualAttribute(XmlNode xmlNode)
        {
            return false;
        }

        /// <summary>
        /// Obtains the value of a virtual attribute represented by a xml node
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>null</returns>
        public override string ObtainVirualAttributeValue(XmlNode xmlNode)
        {
            return null;
        }

        /// <summary>
        /// Keep a VirtualAttributeValueGeneratorContext in the document with an unique id
        /// </summary>
        /// <param name="id">unique id</param>
        /// <param name="vContext">Virtual Attribute Value GeneratorContext</param>
        public override void SetVirtualValueGeneratorContext(string id, VirtualAttributeValueGeneratorContext vContext)
        {
            // do nothing
        }


        /// <summary>
        /// This method does nothing for this document
        /// </summary>
        public override void Initialize()
		{
		}
		
		/// <summary>
		/// Loads the XML document from the specified path.
		/// </summary>
		public override void Load(string url)
		{
            string path = NewteraNameSpace.GetAppHomeDir() + url;
			this.URL = path;
			this.IsLoaded = true;

            
			base.Load(path);
		}

		/// <summary>
		/// For text document, there is no need to prepare the nodes.
		/// </summary>
		/// <param name="pathEnumerator">the path that specifies the nodes to be prepared</param>
		public override void PrepareNodes(PathEnumerator pathEnumerator)
		{
		}

		/// <summary>
		/// This method don't do anything for text document
		/// </summary>
		/// <param name="qualifier">the interpreter's IExpr qualifier.</param>
		public override void PrepareQualifier(IExpr qualifier)
		{
		}

		/// <summary>
		/// This method don't do anything for text document
		/// </summary>
		/// <param name="func">the function</param>
		/// <param name="params">the list of parameters.</param>
		public override void PrepareFunction(IDBFunction func, ExprCollection parameters)
		{
		}

		/// <summary>
		/// This method don't do anything for text document
		/// </summary>
		/// <param name="sortBy">the sort by spec</param>
		public override void PrepareSortBy(SortbySpec sortBy)
		{
		}
	}
}