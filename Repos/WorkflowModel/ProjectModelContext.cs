/*
* @(#)ProjectModelContext.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	using Newtera.Common.Core;
	
	/// <summary>
	/// A static class that provides current project model to activity property editors.
	/// </summary>
	/// <version>  1.0.1 21 Mar 2007</version>
	public class ProjectModelContext
	{
		private ProjectModel _currentProject = null;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ProjectModelContext theInstance;
		
		static ProjectModelContext()
		{
			theInstance = new ProjectModelContext();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ProjectModelContext()
		{
		}

		/// <summary>
		/// Gets the ProjectModelContext instance.
		/// </summary>
		/// <returns> The ProjectModelContext instance.</returns>
		static public ProjectModelContext Instance
		{
			get
			{
				return theInstance;
			}
		}

		/// <summary>
		/// Gets the Workflow Project that is currently active
		/// </summary>
		/// <returns>A ProjectModel</returns>
		public ProjectModel Project
		{
            get
            {
                return _currentProject;
            }
            set
            {
                _currentProject = value;
            }
		}
	}
}