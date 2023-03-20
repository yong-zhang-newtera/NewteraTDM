/*
* @(#)IBindingInstanceService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Wrapper
{
    using System;
    using System.ComponentModel;

    /// <summary> 
    /// The interface for implementing the service for getting a binding instance from
    /// the database.
    /// </summary>
    public interface IBindingInstanceService
    {
        /// <summary>
        /// The wrapped binding data instance.
        /// </summary>
        /// <returns>The IInstanceWrapper object.</returns>
        IInstanceWrapper GetBindingInstance(Guid workflowInstanceId);

        /// <summary>
        /// Save the channges of the wrapped binding data instance to database.
        /// </summary>
        void SaveBindingInstance(Guid workflowInstanceId);

        /// <summary>
        /// Clear the cached binding instance
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        void ClearBindingInstance(Guid workflowInstanceId);
    }
}
