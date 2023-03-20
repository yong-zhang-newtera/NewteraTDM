/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IRuleNode based on a xml element
	/// </summary>
	/// <version>1.0.0 17 Jun 2004 </version>
	/// <author> Yong Zhang </author>
	public class NodeFactory
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
		/// Creates an instance of IRuleNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IRuleNode instance</returns>
		public IRuleNode Create(XmlElement xmlElement)
		{
			IRuleNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.RuleSet:
					obj = new RuleSet(xmlElement);
					break;
				case NodeType.RuleDef:
					obj = new RuleDef(xmlElement);
					break;
				case NodeType.RuleSetCollection:
					obj = new RuleSetCollection(xmlElement);
					break;
				case NodeType.RuleCollection:
					obj = new RuleCollection(xmlElement);
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
				case NodeType.RuleManager:
					str = "RuleManager";
					break;
				case NodeType.RuleSet:
					str = "RuleSet";
					break;
				case NodeType.RuleDef:
					str = "RuleDef";
					break;
				case NodeType.RuleSetCollection:
					str = "RuleSets";
					break;
				case NodeType.RuleCollection:
					str = "Rules";
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
				case "RuleManager":
					type = NodeType.RuleManager;
					break;
				case "RuleSet":
					type = NodeType.RuleSet;
					break;
				case "RuleDef":
					type = NodeType.RuleDef;
					break;
				case "RuleSets":
					type = NodeType.RuleSetCollection;
					break;
				case "Rules":
					type = NodeType.RuleCollection;
					break;
			}

			return type;
		}
	}
}