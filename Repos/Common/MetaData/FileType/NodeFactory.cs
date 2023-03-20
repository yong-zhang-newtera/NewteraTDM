/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IFileTypeNode based on a xml element
	/// </summary>
	/// <version>1.0.0 14 Jan 2004 </version>
	/// <author> Yong Zhang </author>
	internal class NodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static NodeFactory theFactory;
		
		static NodeFactory()
		{
			theFactory = new NodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private NodeFactory()
		{
		}

		/// <summary>
		/// Gets the NodeFactory instance.
		/// </summary>
		/// <returns> The NodeFactory instance.</returns>
		static public NodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IFileTypeNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IFileTypeNode instance</returns>
		public IFileTypeNode Create(XmlElement xmlElement)
		{
			IFileTypeNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.FileType:
					obj = new FileTypeInfo(xmlElement);
					break;
				case NodeType.Collection:
					obj = new FileTypeInfoCollection(xmlElement);
					break;
				case NodeType.Suffix:
					obj = new FileSuffix(xmlElement);
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
				case NodeType.FileType:
					str = "FileType";
					break;
				case NodeType.TypeCollection:
					str = "FileTypes";
					break;
				case NodeType.Suffix:
					str = "Suffix";
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
				case "FileType":
					type = NodeType.FileType;
					break;
				case "FileTypes":
					type = NodeType.TypeCollection;
					break;
				case "Suffix":
					type = NodeType.Suffix;
					break;
			}

			return type;
		}
	}
}