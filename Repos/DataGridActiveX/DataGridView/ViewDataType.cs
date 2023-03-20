/*
* @(#)DataTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;

	/// <summary>
	/// Describes the types of data for attributes in a class
	/// </summary>
	public enum ViewDataType
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
		/// Byte
		/// </summary>
		Byte,
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
		/// Integer
		/// </summary>
		Integer,
		/// <summary>
		/// BigInteger
		/// </summary>
		BigInteger,
		/// <summary>
		/// String
		/// </summary>
		String,
		/// <summary>
		/// Text for string more than 2000 characters
		/// </summary>
		Text
	}
}