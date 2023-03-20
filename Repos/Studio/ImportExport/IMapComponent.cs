/*
* @(#)IMapComponent.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// Represents a common interface for the UI components used in map panel.
	/// </summary>
	/// <version>1.0.0 29 Oct 2004</version>
	/// <author>Yong Zhang </author>
	public interface IMapComponent
	{
		/// <summary>
		/// Gets or sets the IMappingNode associated with the mapping UI componnet
		/// </summary>
		/// <value>The associated IMappingNode instance, it could be null</value>
		IMappingNode MappingNode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the component is
		/// selected.
		/// </summary>
		/// <value>True if it selected, false otherwise.</value>
		bool IsSelected
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the component is
		/// visible or not on the panel
		/// </summary>
		/// <value>True if it visible, false otherwise.</value>
		bool IsVisible
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the x where the component is hit by mouse
		/// </summary>
		int HitX
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the y where the component is hit by mouse
		/// </summary>
		int HitY
		{
			get; set;
		}

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>One of MapComponentTypeEnum</value>
		MapComponentType ComponentType
		{
			get;
		}

		/// <summary>
		/// Gets the principal component when it represents a group of
		/// components.
		/// </summary>
		IMapComponent Principal
		{
			get;
		}

		/// <summary>
		/// Gets the information indicate whether the mapping represented by
		/// the IMapComponent is valid or not.
		/// </summary>
		/// <value>True if it is valid, false otherwise.</value>
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Indicate whether the component is tightly coupled in a selection group.
		/// If it is tightly coupled, it is deleted when the group is deleted.
		/// </summary>
		/// <value>True if it is tightly coupled, false otherwise.By default, true</value>
		bool IsTightlyCoupled
		{
			get; set;
		}

		/// <summary>
		///  Gets the information indicating whether the mapping represented by
		///  this component has existed.
		/// </summary>
		/// <returns>True if it existed, false otherwise.</returns>
		bool IsMappingExist();

		/// <summary>
		/// Get the group of components that are supposed to be selected together
		/// </summary>
		/// <returns>IMapComponent</returns>
		IMapComponent GetSelectionGroup();

		/// <summary>
		/// Create a IMappingNode instance represent this UI map component
		/// </summary>
		void CreateMapping();

		/// <summary>
		/// Show the mapping(s) on the map panel
		/// </summary>
		void ShowMapping();

		/// <summary>
		/// Adjust the position of the components in the map panel
		/// </summary>
		void AdjustPosition();

		/// <summary>
		/// Select the component.
		/// </summary>
		void Select();

		/// <summary>
		/// Deselect the component.
		/// </summary>
		void Deselect();

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		void Delete();

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// component
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the component, false, otherwise.</returns>
		bool Hits(int x, int y);

		/// <summary>
		/// Draw a the component on the panel
		/// </summary>
		void Draw();

		/// <summary>
		/// Erase the component on the panel
		/// </summary>
		void Erase();
	}
}