/*
* @(#)IAttachmentInfo.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Attachment
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the attachment info.
	/// </summary>
	/// <version>  	1.0.0 08 Jan 2004</version>
	/// <author>  Yong Zhang </author>
	public interface IAttachmentInfo
	{
		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		NodeType NodeType {get;}

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		void Unmarshal(XmlElement parent);

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		void Marshal(XmlElement parent);
	}
}