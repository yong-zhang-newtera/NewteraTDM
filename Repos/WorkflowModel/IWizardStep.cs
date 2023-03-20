/*
* @(#)IWizardStep.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a interface for a wizard step.
	/// </summary>
	/// <version> 1.0.0 13 Jun. 2009</version>
    public interface IWizardStep
	{
        /// <summary>
        /// Gets an unique name of the step
        /// </summary>
        /// <returns>An unique Name</returns>
        string Name { get; set;}

		/// <summary>
		/// Gets or sets title of the step
		/// </summary>
        string Title { get; set;}

		/// <summary>
		/// Gets or sets description of the step
		/// </summary>
		/// <returns>The element description</returns>
		string Description { get; set;}	
	
		/// <summary>
		/// Gets or sets indez of the element
		/// </summary>
		/// <returns>The display position</returns>
		int StepIndex { get; set;}

        /// <summary>
        /// Gets or sets url of an user control which serves as user interface of the step
        /// </summary>
        string DisplayUrl { get; set;}
	}
}