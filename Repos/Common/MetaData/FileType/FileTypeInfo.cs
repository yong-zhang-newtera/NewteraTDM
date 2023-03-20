/*
* @(#)FileTypeInfo.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	using System;
	using System.IO;
	using System.Xml;

	/// <summary> 
	/// The base class for all xacl node classes
	/// </summary>
	/// <version> 1.0.0 14 Jan 2003</version>
	/// <author> Yong Zhang </author>
	public class FileTypeInfo : IFileTypeNode
	{
		private string _type;
		private string _desc;
		private string _smallIconPath;
		private string _largeIconPath;
		private string _application;
		private FileTypeNodeCollection _suffixes;
		private Stream _smallIconStream; // run-time use only, do not marshal to xml
		private Stream _largeIconStream; // run-time use only, do not marshal to xml
		private int _imageIndex; // run-time use only, do not marshal to xml

		/// <summary>
		/// Initiate an instance of FileTypeInfo class
		/// </summary>
		public FileTypeInfo()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of FileTypeInfo class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal FileTypeInfo(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the type string.
		/// </summary>
		/// <value>The type string</value>
		/// <example>image/gif, image/jpeg, etc.</example>
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the type description.
		/// </summary>
		/// <value>The type description</value>
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}

		/// <summary>
		/// Gets or sets the file suffixes for the type.
		/// </summary>
		/// <value>A FileTypeNodeCollection</value>
		public FileTypeNodeCollection Suffixes
		{
			get
			{
				return _suffixes;
			}
			set
			{
				_suffixes = value;
			}
		}

		/// <summary>
		/// Gets or sets the application that consumes the type.
		/// </summary>
		/// <value>The application name</value>
		public string Application
		{
			get
			{
				return _application;
			}
			set
			{
				_application = value;
			}
		}

		/// <summary>
		/// Gets or sets a path of small icon image file.
		/// </summary>
		/// <value>A String object</value>
		public string SmallIconPath
		{
			get
			{
				return _smallIconPath;
			}
			set
			{
				_smallIconPath = value;
			}
		}

		/// <summary>
		/// Gets or sets a path of large icon image file.
		/// </summary>
		/// <value>A String object</value>
		public string LargeIconPath
		{
			get
			{
				return _largeIconPath;
			}
			set
			{
				_largeIconPath = value;
			}
		}

		/// <summary>
		/// Gets or sets a stream to read an image of the small icon.
		/// </summary>
		/// <value>A Stream object</value>
		public Stream SmallIconStream
		{
			get
			{
				return _smallIconStream;
			}
			set
			{
				_smallIconStream = value;
			}
		}

		/// <summary>
		/// Gets or sets a stream to read an image of the large icon.
		/// </summary>
		/// <value>A Stream object</value>
		public Stream LargeIconStream
		{
			get
			{
				return _largeIconStream;
			}
			set
			{
				_largeIconStream = value;
			}
		}

		/// <summary>
		/// Gets or set an index that indicates the position of the icon in a
		/// image list.
		/// </summary>
		/// <value>An Integer representing an index</value>
		public int ImageIndex
		{
			get
			{
				return _imageIndex;
			}
			set
			{
				_imageIndex = value;
			}
		}

		/// <summary>
		/// Initialize the instance
		/// </summary>
		private void Init()
		{
			_type = null;
			_desc = null;
			_suffixes = new FileTypeNodeCollection();
			_smallIconPath = null;
			_largeIconPath = null;
			_application = null;
			_smallIconStream = null;
			_largeIconStream = null;
			_imageIndex = 0;
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
				return NodeType.FileType;
			}
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			_type = parent.GetAttribute("type");
			_desc = parent.GetAttribute("desc");
			_application = parent.GetAttribute("application");
			_smallIconPath = parent.GetAttribute("smallIcon");
			_largeIconPath = parent.GetAttribute("largeIcon");

			foreach (XmlElement child in parent.ChildNodes)
			{
				IFileTypeNode node = NodeFactory.Instance.Create(child);

				_suffixes.Add(node); 
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			parent.SetAttribute("type", _type);
			parent.SetAttribute("desc", _desc);
			parent.SetAttribute("application", _application);
			parent.SetAttribute("smallIcon", _smallIconPath);
			parent.SetAttribute("largeIcon", _largeIconPath);

			XmlElement child;

			foreach (IFileTypeNode node in _suffixes)
			{
				child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#endregion
	}
}