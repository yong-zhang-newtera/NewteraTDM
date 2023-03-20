/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of ILoggingNode based on a xml element
	/// </summary>
	/// <version>1.0.0 04 Jan 2009 </version>
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
		/// Creates an instance of ILoggingNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A ILoggingNode instance</returns>
		public ILoggingNode Create(XmlElement xmlElement)
		{
			ILoggingNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.Definition:
					obj = new LoggingDef(xmlElement);
					break;
				case NodeType.Object:
                    obj = new LoggingObject(xmlElement);
					break;
				case NodeType.Subject:
                    obj = new LoggingSubject(xmlElement);
					break;
				case NodeType.Rule:
                    obj = new LoggingRule(xmlElement);
					break;
				case NodeType.Action:
					obj = new LoggingAction(xmlElement);
					break;
				case NodeType.Rules:
					obj = new LoggingRuleCollection(xmlElement);
					break;
				case NodeType.Actions:
					obj = new LoggingActionCollection(xmlElement);
					break;
				case NodeType.Definitions:
					obj = new LoggingDefCollection(xmlElement);
					break;
				case NodeType.Setting:
					obj = new LoggingSetting(xmlElement);
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
				case NodeType.Policy:
					str = "Policy";
					break;
				case NodeType.Definition:
					str = "definition";
					break;
				case NodeType.Object:
					str = "object";
					break;
				case NodeType.Subject:
					str = "subject";
					break;
				case NodeType.Rule:
					str = "rule";
					break;
				case NodeType.Action:
					str = "action";
					break;
				case NodeType.Rules:
					str = "rules";
					break;
				case NodeType.Actions:
					str = "actions";
					break;
				case NodeType.Definitions:
					str = "definitions";
					break;
				case NodeType.Setting:
					str = "setting";
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
				case "Policy":
					type = NodeType.Policy;
					break;
				case "definition":
					type = NodeType.Definition;
					break;
				case "object":
					type = NodeType.Object;
					break;
				case "subject":
					type = NodeType.Subject;
					break;
				case "rule":
					type = NodeType.Rule;
					break;
				case "action":
					type = NodeType.Action;
					break;
				case "rules":
					type = NodeType.Rules;
					break;
				case "actions":
					type = NodeType.Actions;
					break;
				case "definitions":
					type = NodeType.Definitions;
					break;
				case "setting":
					type = NodeType.Setting;
					break;
			}

			return type;
		}
	}
}