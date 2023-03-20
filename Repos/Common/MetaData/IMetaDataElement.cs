/*
* @(#)IMetaDataElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a common interface for the elements in MetaData and its subfolder name spaces.
	/// </summary>
	/// <version> 1.0.0 16 Nov. 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IMetaDataElement
	{
		/// <summary>
		/// Value changed event handler
		/// </summary>
		event EventHandler ValueChanged;

		/// <summary>
		/// Caption changed event handler
		/// </summary>
		event EventHandler CaptionChanged;

		/// <summary>
		/// Gets name of the element
		/// </summary>
		/// <returns>The element name</returns>
		string Name {get;}

		/// <summary>
		/// Gets or sets Caption of the element
		/// </summary>
		/// <returns>The element caption</returns>
		string Caption { get; set;}

		/// <summary>
		/// Gets or sets description of the element
		/// </summary>
		/// <returns>The element description</returns>
		string Description { get; set;}	
	
		/// <summary>
		/// Gets or sets display position of the element
		/// </summary>
		/// <returns>The display position</returns>
		int Position { get; set;}	
	}
}