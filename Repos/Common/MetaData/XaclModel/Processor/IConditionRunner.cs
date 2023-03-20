/*
* @(#)IConditionRunner.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Processor
{
	using System;

	/// <summary>
	/// Represents an interface for executing a condition expressed in xquery and return
	/// a boolean value indicating the result of a condition.
	/// </summary>
	/// <version>  	1.0.0 18 Dec. 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IConditionRunner
	{
		/// <summary>
		/// Gets an information indicating if a condition expressed in xquery is met or
		/// or not.
		/// </summary>
		/// <param name="condition">The condition expressed in xquery</param>
		/// <returns>true if the condition is met, false otherwise</returns>
		bool IsConditionMet(string condition);
	}
}