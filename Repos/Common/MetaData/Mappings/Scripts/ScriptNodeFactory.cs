/*
* @(#)ScriptNodeFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IScriptNode based on a xml element
	/// </summary>
	/// <version>1.0.0 23 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class ScriptNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ScriptNodeFactory theFactory;
		
		static ScriptNodeFactory()
		{
			theFactory = new ScriptNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ScriptNodeFactory()
		{
		}

		/// <summary>
		/// Gets the ScriptNodeFactory instance.
		/// </summary>
		/// <returns> The ScriptNodeFactory instance.</returns>
		static public ScriptNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IScriptNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IScriptNode instance</returns>
		public IScriptNode Create(XmlElement xmlElement)
		{
			IScriptNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.ClassScript:
					obj = new ClassScript(xmlElement);
					break;
				case NodeType.InstanceScript:
					obj = new InstanceScript(xmlElement);
					break;
				case NodeType.ClassScriptCollection:
					obj = new ClassScriptCollection(xmlElement);
					break;
				case NodeType.InstanceScriptCollection:
					obj = new InstanceScriptCollection(xmlElement);
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
				case NodeType.ScriptManager:
					str = "ScriptManager";
					break;
				case NodeType.ClassScript:
					str = "ClassScript";
					break;
				case NodeType.InstanceScript:
					str = "InstanceScript";
					break;
				case NodeType.ClassScriptCollection:
					str = "ClassScriptCollection";
					break;
				case NodeType.InstanceScriptCollection:
					str = "InstanceScriptCollection";
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
				case "ScriptManager":
					type = NodeType.ScriptManager;
					break;
				case "ClassScript":
					type = NodeType.ClassScript;
					break;
				case "InstanceScript":
					type = NodeType.InstanceScript;
					break;
				case "ClassScriptCollection":
					type = NodeType.ClassScriptCollection;
					break;
				case "InstanceScriptCollection":
					type = NodeType.InstanceScriptCollection;
					break;
			}

			return type;
		}
	}
}