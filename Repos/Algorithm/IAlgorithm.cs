/*
* @(#)IAlgorithm.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Algorithm
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Represents a common interface for all algorithm implementations.
	/// </summary>
	/// <version> 1.0.0 20 Aug 2007</version>
    /// <remarks>Deprecated in 4.0.0</remarks>
	public interface IAlgorithm
	{
		/// <summary>
		/// Execute the algorithm on the provided data in the TableTable.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for algorithm.</param>
        /// <returns>The result of executing the algorithm</returns>
		string Execute(DataTable dataTable);
	}
}