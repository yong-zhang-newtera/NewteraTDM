/*
* @(#)ICustomFunction.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Wrapper
{
	using System;

	/// <summary>
	/// Represents an interface for a custom function used by InvokeCustomFunctionActivity
	/// </summary>
	/// <version> 1.0.0 2 Dec 2007 </version>
	public interface ICustomFunction
	{
		/// <summary>
		/// Execute an custom function
		/// </summary>
        /// <param name="instance">The instance of IInstanceWrapper interface that represents the data instance bound to a workflow instance.</param>
		/// <returns>A string return from custome function</returns>
		string Execute(IInstanceWrapper instance);
	}
}