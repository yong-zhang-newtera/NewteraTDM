/*
* @(#) IIndexingRunner.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A common interface for an external full-text search (such as Elasticsearch) index runner.
    /// </summary>
    /// <remarks>The implementation of IIndexingRunner must be thread-safe</remarks>
    /// <version> 	1.0.0 22 Nov 2017 </version>
    public interface IIndexingRunner
	{
        /// <summary>
        /// Create or update an external full-text search index
        /// </summary>
        Task Execute(IndexingContext context, CancellationToken cancellationToken);	
	}
}