/*
* @(#)ClassScriptCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of ClassScript instances.
	/// </summary>
	/// <version>1.0.0 23 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class ClassScriptCollection : ScriptNodeCollection
	{
		/// <summary>
		/// Initiating an instance of ClassScriptCollection class
		/// </summary>
		public ClassScriptCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ClassScriptCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ClassScriptCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.ClassScriptCollection;
			}
		}
	}
}