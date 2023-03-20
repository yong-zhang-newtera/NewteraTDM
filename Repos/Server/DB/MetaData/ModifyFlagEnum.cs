/*
* @(#)ModifyFlagEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	/// <summary>
	/// Describes the flags indicating which aspect of a column is modified
	/// </summary>
	/// <remarks>
	/// Note: remember to modify the value of All option when you add new enums
	/// </remarks>
	public enum ModifyFlag
	{
		None = 0,				// 0x0000000
		DataType = 1,			// 0x0000001
		MaxLength = 2,			// 0x0000010
		MinLength = 4,			// 0x0000100
		IsNullable = 8,			// ox0001000
		IsRequired = 16,		// 0x0010000
		DefaultValue = 32,		// 0x0100000
		All = 63				// 0x0111111  remember to modify value of All option when you add new enums
	}
}