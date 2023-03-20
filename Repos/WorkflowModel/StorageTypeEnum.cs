/*
* @(#)StorageTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	/// <summary>
	/// Specify the types of storage that stores workflow data.
	/// </summary>
	public enum StorageType
	{
		/// <summary>
		/// Unknown type
		/// </summary>
		Unknown,

        /// <summary>
        /// Memory storage
        /// </summary>
        Memory,

        /// <summary>
        /// File storage
        /// </summary>
        File,

		/// <summary>
        /// Database storage
		/// </summary>
		Database
	}
}