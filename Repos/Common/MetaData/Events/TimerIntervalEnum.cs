/*
* @(#)EventTypeEnum.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	/// <summary>
	/// Specify the types of operation on the database
	/// </summary>
	public enum TimerInterval
	{
		/// <summary>
		/// Every hour
		/// </summary>
		EveryHour,

		/// <summary>
		/// Every day
		/// </summary>
		EveryDay,

        /// <summary>
        /// Every week
        /// </summary>
        EveryWeek,

        /// <summary>
        /// Every month
        /// </summary>
        EveryMonth,

        /// <summary>
        /// Every year
        /// </summary>
        EveryYear
	}
}