/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of ISiteMapNode based on a xml element
	/// </summary>
	/// <version>1.0.0 14 Jun 2009 </version>
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
		/// Creates an instance of ISiteMapNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A ISiteMapNode instance</returns>
		public ISiteMapNode Create(XmlElement xmlElement)
		{
			ISiteMapNode obj = null;

			string elemntName = xmlElement.Name;

            NodeType type = (NodeType)Enum.Parse(typeof(NodeType), elemntName);

			switch (type)
			{
				case NodeType.siteMapNode:
					obj = new SiteMapNode(xmlElement);
					break;
				case NodeType.SiteMapNodeCollection:
					obj = new SiteMapNodeCollection(xmlElement);
					break;
                case NodeType.SideMenuGroup:
                    obj = new SideMenuGroup(xmlElement);
                    break;
                case NodeType.Menu:
                    obj = new Menu(xmlElement);
                    break;
                case NodeType.MenuItem:
                    obj = new MenuItem(xmlElement);
                    break;
                case NodeType.CustomCommandGroup:
                    obj = new CustomCommandGroup(xmlElement);
                    break;
                case NodeType.CustomCommand:
                    obj = new CustomCommand(xmlElement);
                    break;
                case NodeType.Model:
                    obj = new SiteMapModel(xmlElement);
                    break;
                case NodeType.Parameter:
                    obj = new StateParameter(xmlElement);
                    break;
            }
			
			return obj;
		}

        public string ConvertTypeToString(NodeType nodeType)
        {
            return Enum.GetName(typeof(NodeType), nodeType);
        }
	}
}