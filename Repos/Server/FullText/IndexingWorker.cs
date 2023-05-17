/*
* @(#)IndexingWorker.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.Xml;
    using System.Data;
    using System.IO;
	using System.Collections;
    using System.Threading;
    using System.Collections.Specialized;
    using System.Workflow.Runtime;
    using System.Security.Principal;
    using System.Runtime.Remoting.Messaging;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;

    /// <summary> 
    /// Performs an external full-text search index update using a worker thread from thread pool.
    /// </summary>
    /// <version>  	1.0.0 22 Nov 2017</version>
    public class IndexingWorker
	{
        private IndexingContext _context;
        private IIndexingRunner _runner;
 
		/// <summary>
        /// Initiating an IndexRunner instance
		/// </summary>
        /// <param name="context">The event context</param>
        public IndexingWorker(IndexingContext context)
		{
            _context = context;
		}

        /// <summary>
        /// run the indexer async
        /// </summary>
        public void Run(IndexingContext context, IIndexingRunner runner)
        {
            _context = context;
            _runner = runner;

            // run the event raiser using a worker thread from the thread pool
            RunIndexAsyncDelegate runEventDelegate = new RunIndexAsyncDelegate(RunEventAsync);

            AsyncCallback callback = new AsyncCallback(RunEventCallback);

            runEventDelegate.BeginInvoke(callback, CancellationToken.None);
        }

        private delegate void RunIndexAsyncDelegate();

        /// <summary>
        /// run the event raiser
        /// </summary>
        private void RunEventAsync()
        {
            if (_runner != null)
                _runner.Execute(_context, CancellationToken.None);
        }

        private void RunEventCallback(IAsyncResult ar)
        {
            // Retrieve the delegate.
            AsyncResult result = (AsyncResult)ar;
            RunIndexAsyncDelegate caller = (RunIndexAsyncDelegate)result.AsyncDelegate;

            // Call EndInvoke to avoid memory leak
            caller.EndInvoke(ar);
        }
	}
}