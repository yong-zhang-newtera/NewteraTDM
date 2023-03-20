/*
* @(#)ScriptNodeBase.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary> 
	/// The base class for all node in Scripts package
	/// </summary>
	/// <version> 1.0.0 23 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public abstract class ScriptNodeBase : IScriptNode
	{	
		/// <summary>
		/// Initiate an instance of ScriptNodeBase class
		/// </summary>
		public ScriptNodeBase()
		{
		}

		#region IScriptNode interface implementation
		
		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract NodeType NodeType {get;}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
		}

		#endregion
	}
}