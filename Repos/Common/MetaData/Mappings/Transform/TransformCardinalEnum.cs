/*
* @(#)TransformCardinalEnum.cs
*
* Copyright (c) 2003-2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	/// <summary>
	/// Specify the types of transform cardinal types.
	/// </summary>
	public enum TransformCardinal
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Transform one source row to one destination row
		/// </summary>
		OneToOne,
		/// <summary>
		/// Transform many source row to one destination row
		/// </summary>
		ManyToOne,
		/// <summary>
		/// Transform all source row to one destination row
		/// </summary>
		AllToOne
	}
}