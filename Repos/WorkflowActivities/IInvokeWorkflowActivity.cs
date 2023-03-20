/*
* @(#)IInvokeWorkflowActivity.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Activities
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the activities that can invoke Newtera Workflow
	/// </summary>
	/// <version> 1.0.0 10 Dec 2010</version>
	public interface IInvokeWorkflowActivity
	{
        /// <summary>
        /// Gets or sets name of the project that defines the workflow to be invoked.
        /// </summary>
        string ProjectName { get; set;}

        /// <summary>
        /// Gets or sets name of the workflow to be invoked
        /// </summary>
        string WorkflowName { get; set;}

        /// <summary>
        /// Gets or sets the parameter bindings of the invoked workflow
        /// </summary>
        IList ParameterBindings { get; set;}
	}
}