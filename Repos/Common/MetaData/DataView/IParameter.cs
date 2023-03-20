/*
* @(#)IParameter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Collections;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents an interface for all parameters used in binary expressions in XQuery
	/// </summary>
	/// <version> 1.0.0 24 Oct 2007</version>
	public interface IParameter
	{
		/// <summary>
		/// Gets information indicating whether the parameter has value.
		/// </summary>
		/// <returns>true if the parameter has value, false otherwise.</returns>
		bool HasValue {get;}

        /// <summary>
        /// If the paramter's value is composed of multiple values separated by "&",
        /// this method return the same type of IParameter that contains a single value
        /// whose position is indicated by the index.
        /// </summary>
        /// <param name="index">The value index</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="dataType">The data type of parameter</param>
        /// <returns>An IParameter whose value is standalone, null if there isn't a value at the given index.</returns>
        IParameter GetParameterByIndex(int index, string name, DataType dataType);
	}
}