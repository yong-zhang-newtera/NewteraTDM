/*
* @(#)IAggregateFunction.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Function
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Represents a common interface for all aggegate function implementations.
	/// </summary>
	/// <version> 1.0.0 12 Dec 2010</version>
	public interface IAggregateFunction
	{
        /// <summary>
        /// To initialize the algorithm
        /// </summary>
        void BeginCalculate();

		/// <summary>
		/// Calculate method is called for each row in a data grid
		/// </summary>
        /// <param name="fieldName">A field name of the currently processed row</param>
        /// <param name="dataRow">the currently processed data row</param>
		void Calculate(string fieldName, DataRow dataRow);

        /// <summary>
        /// Get finalized result of the algorithm
        /// </summary>
        /// <returns>The calculate result</returns>
        string EndCalculate();
	}
}