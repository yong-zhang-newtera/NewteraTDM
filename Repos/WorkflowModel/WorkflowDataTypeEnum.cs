/*
* @(#)WorkflowDataTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	/// <summary>
	/// Specify the types of workflow data.
	/// </summary>
	public enum WorkflowDataType
	{
		/// <summary>
		/// Unknown type
		/// </summary>
		Unknown,

        /// <summary>
        /// Xoml
        /// </summary>
        Xoml,

		/// <summary>
        /// Rules
		/// </summary>
		Rules,

        /// <summary>
        /// Layout
        /// </summary>
        Layout,

        /// <summary>
        /// Code
        /// </summary>
        Code
	}
}