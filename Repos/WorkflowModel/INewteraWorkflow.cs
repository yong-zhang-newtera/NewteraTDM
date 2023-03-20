/*
* @(#)INewteraWorkflow.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Wrapper;

	/// <summary>
	/// Represents a interface for the workflow root activity defined using Newtera WorkflowStudio,
    /// including NewteraSequetialWorkflowActivity and NewteraStateMachineWorkflowActivity.
    /// object.
	/// </summary>
	/// <version>  	1.0.0 16 May 2007</version>
	public interface INewteraWorkflow
	{
        event EventHandler StartEventChanged;

        /// <summary>
        /// Gets or sets the schema id for the binding instance
        /// </summary>
        string SchemaId { get; set;}

        /// <summary>
        /// Gets or sets the class name for the binding instance
        /// </summary>
        string ClassName { get; set;}

        /// <summary>
        /// Gets or sets the class caption for the binding instance
        /// </summary>
        string ClassCaption { get; set;}

        /// <summary>
        /// Gets or sets the starting event name
        /// </summary>
        string EventName { get; set;}

        /// <summary>
        /// Gets or sets the input parameters of the workflow
        /// </summary>
        IList InputParameters { get; set;}

        /// <summary>
        /// Gets the binding instance
        /// </summary>
        /// <remarks>The property is used at execution only, not available at design time</remarks>
        IInstanceWrapper Instance {get;}
	}
}