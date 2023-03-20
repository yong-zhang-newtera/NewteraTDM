/*
* @(#)FileSuffix.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	using System;
	using System.Xml;

	/// <summary> 
	/// The base class for all xacl node classes
	/// </summary>
	/// <version> 1.0.0 14 Jan 2003</version>
	/// <author> Yong Zhang </author>
	public class FileSuffix : IFileTypeNode
	{
		private string _name;
		
		/// <summary>
		/// Initiate an instance of FileSuffix class
		/// </summary>
		public FileSuffix()
		{
			_name = null;
		}

		/// <summary>
		/// Initiating an instance of FileSuffix class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal FileSuffix(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the suffix string.
		/// </summary>
		/// <value>The suffix string</value>
		/// <example>doc, gif, and xml, etc.</example>
		public string Suffix
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		#region IFileTypeNode interface implementation

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType	
		{
			get
			{
				return NodeType.Suffix;
			}
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			_name = parent.GetAttribute("name");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			parent.SetAttribute("name", _name);
		}

		#endregion
	}
}