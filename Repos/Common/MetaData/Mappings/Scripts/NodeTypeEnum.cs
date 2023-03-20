/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	/// <summary>
	/// Specify the types of nodes in Scripts package.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown node type
		/// </summary>
		Unknown,
		/// <summary>
		/// Script manager
		/// </summary>
		ScriptManager,
		/// <summary>
		/// Class Script
		/// </summary>
		ClassScript,
		/// <summary>
		/// Class Script Collection
		/// </summary>
		ClassScriptCollection,
		/// <summary>
		/// InstanceScript
		/// </summary>
		InstanceScript,
		/// <summary>
		/// Instance Script Collection
		/// </summary>
		InstanceScriptCollection,
		/// <summary>
		/// Collection
		/// </summary>
		Collection
	}
}