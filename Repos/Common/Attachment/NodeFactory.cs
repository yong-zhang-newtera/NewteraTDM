/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Attachment
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IAttachmentInfo based on a xml element
	/// </summary>
	/// <version>1.0.0 08 Jan 2004 </version>
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
		/// Creates an instance of IAttachmentInfo type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IAttachmentInfo instance</returns>
		public IAttachmentInfo Create(XmlElement xmlElement)
		{
			IAttachmentInfo obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.Attachment:
					obj = new AttachmentInfo(xmlElement);
					break;
				case NodeType.Collection:
					obj = new AttachmentInfoCollection(xmlElement);
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
				case NodeType.Attachment:
					str = "Attachment";
					break;
				case NodeType.Collection:
					str = "Attachments";
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
				case "Attachment":
					type = NodeType.Attachment;
					break;
				case "Attachments":
					type = NodeType.Collection;
					break;
			}

			return type;
		}
	}
}