/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	/// <summary>
	/// Specify the types of nodes in FileType package
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown node type
		/// </summary>
		Unknown,
		/// <summary>
		/// Collection type
		/// </summary>
		Collection,
		/// <summary>
		/// Type collection
		/// </summary>
		TypeCollection,
		/// <summary>
		/// File Type
		/// </summary>
		FileType,
		/// <summary>
		/// Suffix
		/// </summary>
		Suffix
	}
}