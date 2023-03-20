/*
* @(#)IFormula.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Generator
{
	using System;
	using System.Xml;
	using System.Collections.Specialized;

    using Newtera.Common.Wrapper;

	/// <summary>
	/// Represents a interface for a formula that generates value of a virtual attribute.
	/// </summary>
	/// <version> 1.0.0 26 May 2007</version>
	public interface IFormula
	{
		/// <summary>
		/// Execute the formula to generate a value of the virtual attribute.
		/// </summary>
		/// <param name="instance">A wrapped instance of the current row.</param>
        /// <param name="context">The execution context.</param>
		string Execute(IInstanceWrapper instance, ExecutionContext context);
	}
}