/*
* @(#)ElementTypeEnum.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	/// <summary>
	/// Specify the types of elements in project model.
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Unknown element
		/// </summary>
		Unknown,
		/// <summary>
		/// Project element
		/// </summary>
		Project,
		/// <summary>
		/// Grammar element
		/// </summary>
		Grammar,
		/// <summary>
		/// Parser element
		/// </summary>
		Parser,
		/// <summary>
		/// Sample element
		/// </summary>
		Sample,
		/// <summary>
		/// Collection element
		/// </summary>
		Collection,
		/// <summary>
		/// ParserCollection element
		/// </summary>
		ParserCollection,
		/// <summary>
		/// SampleCollection element
		/// </summary>
		SampleCollection
	}
}