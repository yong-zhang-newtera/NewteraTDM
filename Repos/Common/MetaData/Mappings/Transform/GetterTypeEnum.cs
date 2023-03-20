/*
* @(#)GetterTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	/// <summary>
	/// Specify the types of attribute setters.
	/// </summary>
	public enum GetterType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// SimpleAttributeGetter
		/// </summary>
		SimpleAttributeGetter,
		/// <summary>
		/// ArrayDataCellGetter
		/// </summary>
		ArrayDataCellGetter,
		/// <summary>
		/// PrimaryKeyGetter
		/// </summary>
		PrimaryKeyGetter
	}
}