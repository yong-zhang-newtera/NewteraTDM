/*
* @(#)WorkflowTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	/// <summary>
	/// Specify the types of workflow.
	/// </summary>
	public enum WorkflowType
	{
		/// <summary>
		/// Unknown type
		/// </summary>
		Unknown,

        /// <summary>
        /// Sequential
        /// </summary>
        Sequential,

		/// <summary>
        /// StateMachine
		/// </summary>
        StateMachine,

        /// <summary>
        /// Wizard
        /// </summary>
        Wizard
	}
}