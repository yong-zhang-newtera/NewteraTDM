/*
* @(#)TransformScript.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents a transform script written in C#, VB script, or
	/// Java#
	/// </summary>
	/// <version>1.0.0 15 Nov 2004</version>
	/// <author> Yong Zhang </author>
    public class TransformScript : ScriptBase
	{	
		/// <summary>
		/// Initiate an instance of TransformScript class.
		/// </summary>
		public TransformScript() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of TransformScript class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal TransformScript(XmlElement xmlElement) : base()
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
				return NodeType.TransformScript;
			}
		}

		/// <summary>
		/// create an TransformScript from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write TransformScript to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}
	}
}