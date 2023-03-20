/*
* @(#) IndexEventManager.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.IO;
    using System.Xml;
    using System.Data;
	using System.Collections;
    using System.Collections.Generic;
	using System.Threading;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// This is the single object that runs an external full-text search index update (such as Elasticsearch)
	/// </summary>
	/// <version> 1.0.0 22 Nov 2017 </version>
	public class IndexEventManager
	{		
		// Static cache object, all invokers will use this cache object.
		private static IndexEventManager theManager;

        private IIndexingRunner _indexingRunner;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private IndexEventManager()
		{
            _indexingRunner = null;
        }

		/// <summary>
		/// Gets the IndexEventManager instance.
		/// </summary>
		/// <returns> The IndexEventManager instance.</returns>
		static public IndexEventManager Instance
		{
			get
			{
				return theManager;
			}
		}

        public IIndexingRunner IndexingRunner
        {
            get
            {
                return _indexingRunner;
            }
            set
            {
                _indexingRunner = value;
            }
        }

        /// <summary>
        /// Post an event 
        /// </summary>
        public void PostEvent(IndexingContext indexContext)
        {
            IndexingWorker indexingWorker = new IndexingWorker(indexContext);

            try
            {
                // run the index update asynchronousely using the worker thread from a threadpool;
                if (_indexingRunner != null)
                    indexingWorker.Run(indexContext, _indexingRunner);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("Failed in running an indexing event due to " + ex.Message + "\n" + ex.StackTrace);
            }
        }

		static IndexEventManager()
		{
			// Initializing the manager.
			{
				theManager = new IndexEventManager();
			}
		}
	}
}