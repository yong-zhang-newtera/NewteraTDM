/*
* @(#)EventTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	/// <summary>
	/// Specify the types of operation on the database
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// Insert operation
		/// </summary>
		Insert,

		/// <summary>
		/// Update operation
		/// </summary>
		Update,

		/// <summary>
		/// Delete operation
		/// </summary>
		Delete,

        /// <summary>
        /// Timer event
        /// </summary>
        Timer
	}
}