/*
* @(#)IndexBuilder.cs
*
* Copyright (c) 2003-2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.Xml;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Security.Principal;
	using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.Wrapper;
    using Newtera.Server.DB;
    using Newtera.Server.Engine.Cache;
    using Newtera.Server.UsrMgr;
 

	/// <summary> 
	/// Responsible for travelling the related data instances to collect values of an index-contributed attribute, saving
    /// the collected values to the indexing attribute of a class, and build full-text index for the indexing attribute.
	/// </summary>
	/// <version>  	1.0.0 24 Apr 2014</version>
	public class IndexBuilder
	{
        private MetaDataModel _metaData;
        private ClassElement _baseClassElement;
        private SimpleAttributeElement _indexingAttribute;
        private DateTime _lastIndexedTime;
        private int _depth;
        private bool _isInProcess;

		/// <summary>
        /// Initiating an IndexBuilder instance
		/// </summary>
        /// <param name="metaData">The MetaDataModel object</param>
        /// <param name="baseClassElement">The classs that the index attribute is defined</param>
        /// <param name="indexingAttribute">The full-text index attribute</param>
        /// <param name="lastIndexedTime">The last index time</param>
        public IndexBuilder(MetaDataModel metaData, ClassElement baseClassElement, SimpleAttributeElement indexingAttribute, DateTime lastIndexedTime, int depth)
		{
            _metaData = metaData;
            _baseClassElement = baseClassElement;
            _indexingAttribute = indexingAttribute;
            _lastIndexedTime = lastIndexedTime;
            _depth = depth;
            _isInProcess = false;

            // sync the last indexed time to th eindexing attribute
            _indexingAttribute.LastIndexedTime = _lastIndexedTime.ToString("s");
		}

        /// <summary>
        /// Gets or sets the information indicating whether the index builder is in process
        /// </summary>
        public bool IsInProcess
        {
            get
            {
                return _isInProcess;
            }
            set
            {
                _isInProcess = value;
            }
        }

        /// <summary>
        /// Gets or sets the last time the index was built
        /// </summary>
        public DateTime LastIndexTime
        {
            get
            {
                return _lastIndexedTime;
            }
            set
            {
                _lastIndexedTime = value;

                // sync the last indexed time to th eindexing attribute
                _indexingAttribute.LastIndexedTime = _lastIndexedTime.ToString("s");
            }
        }

        /// <summary>
        /// Gets or sets the last time the index was built
        /// </summary>
        public SimpleAttributeElement IndexingAttribute
        {
            get
            {
                return _indexingAttribute;
            }
        }

        /// <summary>
        /// run the event raiser
        /// </summary>
        public void Run(Object threadContext)
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            _isInProcess = true;
            Spider spider;
            DBIndexCreator indexCreator;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                IDataProvider dataProvider = DataProviderFactory.Instance.Create();

                // first collect values from the instances in the base class and related classes that will be used
                // to create index

                spider = new Spider(_depth);
                // Crawl the database and build keywords in an incremental mode
                bool needRebuild = spider.StartCrawling(_metaData, _baseClassElement, _indexingAttribute.Name, _lastIndexedTime, true);

                if (needRebuild)
                {
                    indexCreator = new DBIndexCreator();

                    // build a DB specific full-text index
                    indexCreator.CreateFullTextIndex(dataProvider, _indexingAttribute);

                }
            }
            catch (Exception ex)
            {
                // write the error message to the log
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // notify the index mananger that its completion
                IndexingManager.Instance.IndexBuildingCompleted(this);

                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;

                _isInProcess = false;
            }
        }

	}
}