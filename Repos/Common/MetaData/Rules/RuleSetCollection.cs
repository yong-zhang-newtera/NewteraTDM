/*
* @(#)RuleSetCollection.cs
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
	/// Represents a collection of RuleSet instances.
	/// </summary>
	/// <version>1.0.0 16 Jun 2004</version>
	/// <author>Yong Zhang</author>
	public class RuleSetCollection : RuleNodeCollection
	{
		/// <summary>
		/// Initiating an instance of RuleSetCollection class
		/// </summary>
		public RuleSetCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of RuleSetCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal RuleSetCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.RuleSetCollection;
			}
		}
	}
}