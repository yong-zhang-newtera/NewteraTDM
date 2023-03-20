/*
* @(#)PivotLayoutManager.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/

namespace SmartWord
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Text;
    using System.IO;

    using Newtera.Common.MetaData;
    using Newtera.DataGridActiveX.Pivot;
    using Newtera.WindowsControl;
    using SmartWord.OLAPWebService;

	/// <summary>
    /// Implementation of IPivotLayoutManager interface
	/// </summary>
	/// <version>  	1.0.0 21 OCT 2008 </version>
    public class PivotLayoutManager : IPivotLayoutManager
	{
        MetaDataModel _metaData;
        OLAPService _webService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PivotLayoutManager(MetaDataModel metaData)
		{
            _webService = new OLAPService();
            _metaData = metaData;
        }

        /// <summary>
        /// Save a pivot layout data to database with the given name.
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="name">The template name</param>
        /// <param name="desc">The chart description</param>
        /// <param name="viewName">The data view's name</param>
        /// <param name="xml">Pivot Layout in xml</param>
        /// <returns>An unique id of the pivot layout</returns>
        public string SaveNamedPivotLayout(string className, string name, string desc, string viewName, string xml)
        {
            return _webService.SaveNamedPivotLayout(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), 
                className, name, desc, viewName, xml);
        }

        /// <summary>
        /// Check if a pivot layout with the given name has already existed for a class.
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="name">The given template name</param>
        public bool IsPivotLayoutNameUnique(string className, string name)
        {
            return _webService.IsPivotLayoutNameUnique(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), 
                className, name);
        }

        /// <summary>
        /// Gets xml that is text represention of a collection of PivotLayout objects of a class
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="className">The owner class name</param>
        /// <returns>A collection of PivotLayout objects</returns>
        public PivotLayoutCollection GetPivotLayouts(string className)
        {
            string xml = _webService.GetPivotLayouts(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                className);

            TextReader reader = new StringReader(xml);

            PivotLayoutCollection pivotLayouts = new PivotLayoutCollection();
            pivotLayouts.Read(reader);

            return pivotLayouts;
        }

        /// <summary>
        /// Gets a pivot layout's xml of a given a id.
        /// </summary>
        /// <param name="pivotLayoutId">The given pivot layout id</param>
        /// <returns>A xml string representing a pivot layout</returns>
        public string GetPivotLayoutXmlById(string pivotLayoutId)
        {
            return _webService.GetPivotLayoutXmlById(pivotLayoutId);
        }

        /// <summary>
        /// Delete a pivot layout of a given id.
        /// </summary>
        /// <param name="pivotLayoutId">The given pivot layout id</param>
        public void DeletePivotLayoutById(string pivotLayoutId)
        {
            _webService.DeletePivotLayoutById(pivotLayoutId);
        }
	}
}