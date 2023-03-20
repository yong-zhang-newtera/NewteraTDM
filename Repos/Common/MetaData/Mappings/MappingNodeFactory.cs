/*
* @(#)MappingNodeFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IMappingNode based on a xml element
	/// </summary>
	/// <version>1.0.0 02 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class MappingNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static MappingNodeFactory theFactory;
		
		static MappingNodeFactory()
		{
			theFactory = new MappingNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private MappingNodeFactory()
		{
		}

		/// <summary>
		/// Gets the MappingNodeFactory instance.
		/// </summary>
		/// <returns> The MappingNodeFactory instance.</returns>
		static public MappingNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IMappingNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IMappingNode instance</returns>
		public IMappingNode Create(XmlElement xmlElement)
		{
			IMappingNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.MappingPackage:
					obj = new MappingPackage(xmlElement);
					break;
				case NodeType.ClassMapping:
					obj = new ClassMapping(xmlElement);
					break;
				case NodeType.AttributeMapping:
					obj = new AttributeMapping(xmlElement);
					break;
				case NodeType.MappingPackageCollection:
					obj = new MappingPackageCollection(xmlElement);
					break;
				case NodeType.ClassMappingCollection:
					obj = new ClassMappingCollection(xmlElement);
					break;
				case NodeType.AttributeMappingCollection:
					obj = new AttributeMappingCollection(xmlElement);
					break;
				case NodeType.TextFormat:
					obj = new TextFormat(xmlElement);
					break;
				case NodeType.DefaultValue:
					obj = new DefaultValue(xmlElement);
					break;
				case NodeType.DefaultValueCollection:
					obj = new DefaultValueCollection(xmlElement);
					break;
				case NodeType.ArrayDataCellMapping:
					obj = new ArrayDataCellMapping(xmlElement);
					break;
				case NodeType.PrimaryKeyMapping:
					obj = new PrimaryKeyMapping(xmlElement);
					break;
				case NodeType.InputOutputAttribute:
					obj = new InputOutputAttribute(xmlElement);
					break;
				case NodeType.TransformScript:
					obj = new TransformScript(xmlElement);
					break;
                case NodeType.SelectRowScript:
                    obj = new SelectRowScript(xmlElement);
                    break;
                case NodeType.IdentifyRowScript:
                    obj = new IdentifyRowScript(xmlElement);
                    break;
				case NodeType.OneToManyMapping:
					obj = new OneToManyMapping(xmlElement);
					break;
				case NodeType.ManyToOneMapping:
					obj = new ManyToOneMapping(xmlElement);
					break;
				case NodeType.ManyToManyMapping:
					obj = new ManyToManyMapping(xmlElement);
					break;
				case NodeType.Selector:
					obj = new Selector(xmlElement);
					break;
				case NodeType.SelectorCollection:
					obj = new SelectorCollection(xmlElement);
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Convert a NodeType value to a string
		/// </summary>
		/// <param name="type">One of NodeType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(NodeType type)
		{
			string str = "Unknown";

			switch (type)
			{
				case NodeType.MappingManager:
					str = "MappingManager";
					break;
				case NodeType.ClassMapping:
					str = "ClassMapping";
					break;
				case NodeType.AttributeMapping:
					str = "AttributeMapping";
					break;
				case NodeType.MappingPackage:
					str = "MappingPackage";
					break;
				case NodeType.MappingPackageCollection:
					str = "MappingPackageCollection";
					break;
				case NodeType.ClassMappingCollection:
					str = "ClassMappingCollection";
					break;
				case NodeType.AttributeMappingCollection:
					str = "AttributeMappingCollection";
					break;
				case NodeType.TextFormat:
					str = "TextFormat";
					break;
				case NodeType.DefaultValue:
					str = "DefaultValue";
					break;
				case NodeType.DefaultValueCollection:
					str = "DefaultValueCollection";
					break;
				case NodeType.ArrayDataCellMapping:
					str = "ArrayDataCellMapping";
					break;
				case NodeType.PrimaryKeyMapping:
					str = "PrimaryKeyMapping";
					break;
				case NodeType.InputOutputAttribute:
					str = "InputOutputAttribute";
					break;
				case NodeType.TransformScript:
					str = "TransformScript";
					break;
                case NodeType.SelectRowScript:
                    str = "SelectRowScript";
                    break;
                case NodeType.IdentifyRowScript:
                    str = "IdentifyRowScript";
                    break;
				case NodeType.OneToManyMapping:
					str = "OneToManyMapping";
					break;
				case NodeType.ManyToOneMapping:
					str = "ManyToOneMapping";
					break;
				case NodeType.ManyToManyMapping:
					str = "ManyToManyMapping";
					break;
				case NodeType.Selector:
					str = "Selector";
					break;
				case NodeType.SelectorCollection:
					str = "SelectorCollection";
					break;
				case NodeType.SelectorManager:
					str = "SelectorManager";
					break;
			}

			return str;
		}

		/// <summary>
		/// Convert a type string to a NodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of NodeType values</returns>
		internal static NodeType ConvertStringToType(string str)
		{
			NodeType type = NodeType.Unknown;

			switch (str)
			{
				case "MappingManager":
					type = NodeType.MappingManager;
					break;
				case "MappingPackage":
					type = NodeType.MappingPackage;
					break;
				case "ClassMapping":
					type = NodeType.ClassMapping;
					break;
				case "AttributeMapping":
					type = NodeType.AttributeMapping;
					break;
				case "MappingPackageCollection":
					type = NodeType.MappingPackageCollection;
					break;
				case "AttributeMappingCollection":
					type = NodeType.AttributeMappingCollection;
					break;
				case "ClassMappingCollection":
					type = NodeType.ClassMappingCollection;
					break;
				case "TextFormat":
					type = NodeType.TextFormat;
					break;
				case "DefaultValue":
					type = NodeType.DefaultValue;
					break;
				case "DefaultValueCollection":
					type = NodeType.DefaultValueCollection;
					break;
				case "ArrayDataCellMapping":
					type = NodeType.ArrayDataCellMapping;
					break;
				case "PrimaryKeyMapping":
					type = NodeType.PrimaryKeyMapping;
					break;
				case "InputOutputAttribute":
					type = NodeType.InputOutputAttribute;
					break;
				case "TransformScript":
					type = NodeType.TransformScript;
					break;
                case "SelectRowScript":
                    type = NodeType.SelectRowScript;
                    break;
                case "IdentifyRowScript":
                    type = NodeType.IdentifyRowScript;
                    break;
				case "OneToManyMapping":
					type = NodeType.OneToManyMapping;
					break;
				case "ManyToOneMapping":
					type = NodeType.ManyToOneMapping;
					break;
				case "ManyToManyMapping":
					type = NodeType.ManyToManyMapping;
					break;
				case "Selector":
					type = NodeType.Selector;
					break;
				case "SelectorCollection":
					type = NodeType.SelectorCollection;
					break;
				case "SelectorManager":
					type = NodeType.SelectorManager;
					break;
			}

			return type;
		}
	}
}