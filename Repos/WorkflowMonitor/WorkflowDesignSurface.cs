/*
* @(#)WorkflowDesignSurface.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;


namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// This type is used to show workflow design surface.
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    internal sealed class WorkflowDesignSurface : DesignSurface
    {
        internal WorkflowDesignSurface(IMemberCreationService memberCreationService)
        {
            this.ServiceContainer.AddService(typeof(ITypeProvider), new TypeProvider(this.ServiceContainer), true);
            this.ServiceContainer.AddService(typeof(IMemberCreationService), memberCreationService);
            this.ServiceContainer.AddService(typeof(IMenuCommandService), new MenuCommandService(this.ServiceContainer));
        }
    }
}
