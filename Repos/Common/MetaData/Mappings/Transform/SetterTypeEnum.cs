/*
* @(#)SetterTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	/// <summary>
	/// Specify the types of attribute setters.
	/// </summary>
	public enum SetterType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// SimpleAttributeSetter
		/// </summary>
		SimpleAttributeSetter,
		/// <summary>
		/// ArrayDataCellSetter
		/// </summary>
		ArrayDataCellSetter,
		/// <summary>
		/// PrimaryKeySetter
		/// </summary>
		PrimaryKeySetter
	}
}