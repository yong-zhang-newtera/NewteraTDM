/*
* @(#)RuleCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of RuleDef instances.
	/// </summary>
	/// <version>1.0.0 16 Jun 2004</version>
	/// <author>Yong Zhang</author>
	public class RuleCollection : RuleNodeCollection
	{
		/// <summary>
		/// Initiating an instance of RuleCollection class
		/// </summary>
		public RuleCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of RuleCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal RuleCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.RuleCollection;
			}
		}
	}
}