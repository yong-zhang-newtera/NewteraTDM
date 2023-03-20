/*
* @(#) WrapperException.cs	1.1.0
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Wrapper
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// An exception for Newtera.Common.Wrapper name space.
	/// </summary>
	// <version> 	1.1.0	13 May 2007</version>
    [Serializable]
    public class WrapperException : ApplicationException
	{
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WrapperException()
        {
        }

		/// <summary>
		/// A constructor.
		/// </summary>
		/// <param name="desc">a description of the exception.</param>
		public WrapperException(string desc) : base(desc)
		{
		}
	}
}