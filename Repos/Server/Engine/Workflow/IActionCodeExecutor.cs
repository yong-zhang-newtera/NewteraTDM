/*
* @(#)IActionCodeExecutor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Engine.Workflow
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Text;

    using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.DataView;

    /// <summary>
    /// interface to be implemented by the classes that execute custom action code defined for the tasks.
    /// </summary>
    public interface IActionCodeExecutor
    {
        /// <summary>
        /// Gets or sets the data instance used as execution context
        /// </summary>
        IInstanceWrapper Instance { get; set;}

        /// <summary>
        /// Gets or sets the client name of the executor
        /// </summary>
        string Client { get; set;}

        /// <summary>
        /// Gets or sets the name of the propety that invokes the callback function
        /// </summary>
        string Property { get; set; }

        /// <summary>
        /// Execute the custom code
        /// </summary>
        void Execute();
    }
}