/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	/// <summary>
	/// Specify the types of nodes in Mappings package.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// MappingManager
		/// </summary>
		MappingManager,
		/// <summary>
		/// MappingPackage
		/// </summary>
		MappingPackage,
		/// <summary>
		/// MappingPackageCollection
		/// </summary>
		MappingPackageCollection,
		/// <summary>
		/// ClassMapping
		/// </summary>
		ClassMapping,
		/// <summary>
		/// AttributeMapping
		/// </summary>
		AttributeMapping,
		/// <summary>
		/// Collection
		/// </summary>
		Collection,
		/// <summary>
		/// ClassMappingCollection
		/// </summary>
		ClassMappingCollection,
		/// <summary>
		/// AttributeMappingCollection
		/// </summary>
		AttributeMappingCollection,
		/// <summary>
		/// TextFormat
		/// </summary>
		TextFormat,
		/// <summary>
		/// DefaultValue
		/// </summary>
		DefaultValue,
		/// <summary>
		/// DefaultValueCollection
		/// </summary>
		DefaultValueCollection,
		/// <summary>
		/// ArrayDataCellMapping
		/// </summary>
		ArrayDataCellMapping,
		/// <summary>
		/// Primary key mapping
		/// </summary>
		PrimaryKeyMapping,
		/// <summary>
		/// Input or output attribute definition
		/// </summary>
		InputOutputAttribute,
		/// <summary>
		/// OneToManyMapping
		/// </summary>
		OneToManyMapping,
		/// <summary>
		/// ManyToOneMapping
		/// </summary>
		ManyToOneMapping,
		/// <summary>
		/// ManyToManyMapping
		/// </summary>
		ManyToManyMapping,
		/// <summary>
		/// Transform script
		/// </summary>
		TransformScript,
		/// <summary>
		/// Selector
		/// </summary>
		Selector,
		/// <summary>
		/// Selector Collection
		/// </summary>
		SelectorCollection,
		/// <summary>
		/// SelectorManager
		/// </summary>
		SelectorManager,
        /// <summary>
        /// SelectRowScript
        /// </summary>
        SelectRowScript,
        /// <summary>
        /// IdentifyRowScript
        /// </summary>
        IdentifyRowScript
	}
}