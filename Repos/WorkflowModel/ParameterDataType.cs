/*
* @(#)ParameterDataType.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// Describes the types of data for attribute values
	/// </summary>
	public enum ParameterDataType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Boolean
		/// </summary>
		Boolean,
		/// <summary>
		/// Date
		/// </summary>
		Date,
		/// <summary>
		/// DateTime
		/// </summary>
		DateTime,
		/// <summary>
		/// Decimal
		/// </summary>
		Decimal,
		/// <summary>
		/// Double
		/// </summary>
		Double,
		/// <summary>
		/// Float
		/// </summary>
		Float,
		/// <summary>
		/// Integer (int32)
		/// </summary>
		Integer,
		/// <summary>
		/// BigInteger (int64)
		/// </summary>
		BigInteger,
		/// <summary>
		/// String
		/// </summary>
		String,
        /// <summary>
        /// Array
        /// </summary>
        Array
	}
}