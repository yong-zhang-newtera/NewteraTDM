/*
* @(#)DataSourceTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	/// <summary>
	/// Specify the types of data sources supported by Mappings package.
	/// </summary>
	public enum DataSourceType
	{
		/// <summary>
		/// Unknown data source
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Text format
		/// </summary>
		Text,
		/// <summary>
		/// Excel format
		/// </summary>
		Excel,
		/// <summary>
		/// Other format
		/// </summary>
		Other
	}
}