/*
* @(#)IdentifyRowScript.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents an identify row script written in C#, VB script, or
	/// Java#
	/// </summary>
	/// <version>1.0.0 5 Jan 2014</version>
    public class IdentifyRowScript : ScriptBase
	{	
		/// <summary>
		/// Initiate an instance of IdentifyRowScript class.
		/// </summary>
		public IdentifyRowScript() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of IdentifyRowScript class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal IdentifyRowScript(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.IdentifyRowScript;
			}
		}

		/// <summary>
		/// create an IdentifyRowScript from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write IdentifyRowScript to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}
	}
}