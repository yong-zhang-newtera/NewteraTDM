/*
* @(#)XmlDataSourceListHandler.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Util
{
	using System;
    using System.Xml;
    using System.Threading;
	using System.Collections.Specialized;
    using System.Security.Principal;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// Represents the server-side xml data source service
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
            Newtera.Server.UsrMgr.CMUserManager userMgr = new Newtera.Server.UsrMgr.CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            QueryReader reader = null;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            XmlDocument doc = new XmlDocument();

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                XmlNode root = doc.CreateElement("QueryResult");
                doc.AppendChild(root);

                Interpreter interpreter = new Interpreter();

                // get result in pages
                interpreter.IsPaging = true;
                interpreter.PageSize = 50;
                interpreter.OmitArrayData = true;
                interpreter.DelayVirtualAttributeCalculation = false; // calculate the virtual attribute values at conversion
                reader = interpreter.GetQueryReader(query);

                XmlNode imported;
                XmlDocument partialDoc = reader.GetNextPage();
                while (partialDoc != null && partialDoc.DocumentElement.ChildNodes.Count > 0)
                {
                    foreach (XmlElement node in partialDoc.DocumentElement.ChildNodes)
                    {
                        imported = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(imported);
                    }

                    partialDoc = reader.GetNextPage();
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;

                if (reader != null)
                {
                    reader.Close();
                }
            }

			return doc;
		}
	}
}