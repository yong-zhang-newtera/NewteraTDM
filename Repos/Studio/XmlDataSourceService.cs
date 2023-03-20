/*
* @(#)XmlDataSourceListHandler.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.Xml;
    using System.Threading;
	using System.Collections.Specialized;
    using System.Security.Principal;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.WinClientCommon;

	/// <summary>
	/// Represents the windows client side xml data source service
	/// </summary>
	/// <version> 1.0.0 13 Nov 2009 </version>
    public class XmlDataSourceService : IXMLDataSourceService
	{
		/// <summary>
		/// Get a result from xml database
		/// </summary>
		/// <param name="query">The xquery</param>
        /// <returns>A XmlDocument object</returns>
		public XmlDocument Execute(string query)
		{
            MetaDataServiceStub webService = new MetaDataServiceStub();

            XmlDocument xmlDoc = (XmlDocument)webService.SearchXmlDataSource(query);

            // create an XML document
            //XmlDocument doc = new XmlDocument();
            //doc.AppendChild(doc.ImportNode(xmlDoc.DocumentElement, true));
 
            return xmlDoc;
		}
	}
}