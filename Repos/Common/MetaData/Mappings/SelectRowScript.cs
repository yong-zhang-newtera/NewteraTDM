/*
* @(#)SelectRowScript.cs
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

	/// <summary>lect row
	/// The class represents a select row script written in C#, VB script, or
	/// Java#
	/// </summary>
	/// <version>1.0.0 5 Jan 2014</version>
    public class SelectRowScript : ScriptBase
	{	
		/// <summary>
		/// Initiate an instance of SelectRowScript class.
		/// </summary>
		public SelectRowScript() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of SelectRowScript class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal SelectRowScript(XmlElement xmlElement) : base()
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
				return NodeType.SelectRowScript;
			}
		}

		/// <summary>
		/// create an SelectRowScript from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write SelectRowScript to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}
	}
}