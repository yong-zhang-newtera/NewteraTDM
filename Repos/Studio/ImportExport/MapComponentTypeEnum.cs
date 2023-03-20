/*
* @(#)MapComponentTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Describes the types of components used in map panel.
	/// </summary>
	public enum MapComponentType
	{
		/// <summary>
		/// Pointer
		/// </summary>
		Pointer = 1,
		/// <summary>
		/// One to one mapping
		/// </summary>
		OneToOne = 2,
		/// <summary>
		/// One to many mapping
		/// </summary>
		OneToMany = 3,
		/// <summary>
		/// Many to one mapping
		/// </summary>
		ManyToOne = 4,
		/// <summary>
		/// Many to many mapping
		/// </summary>
		ManyToMany = 5,
		/// <summary>
		/// Source end
		/// </summary>
		SourceEnd = 6,
		/// <summary>
		/// Destination end
		/// </summary>
		DestinationEnd = 7,
		/// <summary>
		/// Composite
		/// </summary>
		Composite,
		/// <summary>
		/// Mapping input
		/// </summary>
		InputEnd,
		/// <summary>
		/// Mapping output
		/// </summary>
		OutputEnd,
		/// <summary>
		/// Input line
		/// </summary>
		InputLine,
		/// <summary>
		/// Output line
		/// </summary>
		OutputLine
	}
}