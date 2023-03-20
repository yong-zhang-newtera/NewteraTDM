/*
* @(#)AttachmentInfo.cs
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
	/// The class represents an attachment info
	/// </summary>
	/// <version> 1.0.0 07 Jan 2003</version>
	/// <author> Yong Zhang </author>
	public class AttachmentInfo : IAttachmentInfo
	{
		private string _id;
		private string _itemId;
        private string _attributeName; // an image attribute name, valid when the attachment type is image
		private string _className;
		private string _name;
		private string _type;
		private long _size;
		private string _desc;
		private DateTime _createdTime;
		private bool _isPublic;
        private DateTime _modifiedTime; // run-time use only
		private string _suffix; // run-time use only

		/// <summary>
		/// Initiate an instance of AttachmentInfo class
		/// </summary>
		public AttachmentInfo()
		{
			_id = null;
			_itemId = null;
            _attributeName = null;
			_className = null;
			_name = null;
			_type = null;
			_desc = null;
			_size = 0;
			_isPublic = true;
			_suffix = null;
            _modifiedTime = DateTime.MinValue;
		}

		/// <summary>
		/// Initiating an instance of AttachmentInfo class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AttachmentInfo(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the unique id of an attachment.
		/// </summary>
		/// <value>The id of an attachment</value>
		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets or sets the id of an item to which the attachment belongs.
		/// The item can be either an instance or a class.
		/// </summary>
		/// <value>The id of an item</value>
		public string ItemId
		{
			get
			{
				return _itemId;
			}
			set
			{
				_itemId = value;
			}
		}

        /// <summary>
        /// Gets or sets the name of an image attribute.
        /// </summary>
        /// <value>The name of an image attribute</value>
        public string AttributeName
        {
            get
            {
                return _attributeName;
            }
            set
            {
                _attributeName = value;
            }
        }

		/// <summary>
		/// Gets or sets the name of instance's class.
		/// </summary>
		/// <value>The name of a class</value>
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of an attachment.
		/// </summary>
		/// <value>The name of an attachment</value>
		public string Name
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

		/// <summary>
		/// Gets or sets the size of an attachment.
		/// </summary>
		/// <value>The size of an attachment</value>
		public long Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of an attachment.
		/// </summary>
		/// <value>The type of an attachment</value>
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
		/// Gets or sets the description of an attachment.
		/// </summary>
		/// <value>The description of an attachment</value>
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
		/// Gets or sets the created time of an attachment.
		/// </summary>
		/// <value>A DateTime object</value>
		public DateTime CreateTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				_createdTime = value;
			}
		}

        /// <summary>
        /// Gets or sets the modified time of an attachment.
        /// </summary>
        /// <value>A DateTime object</value>
        public DateTime ModifiedTime
        {
            get
            {
                if (_modifiedTime == DateTime.MinValue)
                    return _createdTime;
                else
                    return _modifiedTime;
            }
            set
            {
                _modifiedTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the attachment is public or not.
        /// </summary>
        /// <value>True if the attachment is public, false is the private. The default is true.</value>
        public bool IsPublic
		{
			get
			{
				return _isPublic;
			}
			set
			{
				_isPublic = value;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the attachment is viewable online or not.
        /// </summary>
        /// <value>True if the attachment is online viewable, false otherwise.</value>
        public bool IsViewable
        {
            get
            {
                bool status = false;
                int pos = _name.LastIndexOf('.');
                if (pos > 0 && pos < _name.Length)
                {
                    string suffix = _name.Substring(pos + 1).ToUpper();
                    if (suffix == "DWF")
                    {
                        status = true;
                    }
                }

                return status;
            }
        }

		/// <summary>
		/// Gets or sets the suffix of an attachment.
		/// </summary>
		/// <value>The suffix of an attachment</value>
		public string Suffix
		{
			get
			{
				if (_suffix == null)
				{
					return "";
				}
				else
				{
					return _suffix;
				}
			}
			set
			{
				_suffix = value;
			}
		}

		#region IAttachmentInfo interface implementation

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType	
		{
			get
			{
				return NodeType.Attachment;
			}
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			_id = parent.GetAttribute("ID");
			_itemId = parent.GetAttribute("InstanceId");
			_className = parent.GetAttribute("ClassName");
            _attributeName = parent.GetAttribute("AttributeName");
			_name = parent.GetAttribute("Name");
			_type = parent.GetAttribute("Type");
			_desc = parent.GetAttribute("Desc");
			_size = System.Convert.ToInt64(parent.GetAttribute("Size"));
			_createdTime = DateTime.Parse(parent.GetAttribute("Time"));
			string status = parent.GetAttribute("IsPublic");
			_isPublic = ((status != null && status == "false") ? false : true);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			parent.SetAttribute("ID", _id);
			parent.SetAttribute("InstanceId", _itemId);
			parent.SetAttribute("ClassName", _className);
            parent.SetAttribute("AttributeName", _attributeName);
			parent.SetAttribute("Name", _name);
			parent.SetAttribute("Type", _type);
			parent.SetAttribute("Size", System.Convert.ToString(_size));
			parent.SetAttribute("Time", _createdTime.ToString());
			if (_desc != null && _desc.Length > 0)
			{
				parent.SetAttribute("Desc", _desc);
			}

			if (!_isPublic)
			{
				parent.SetAttribute("IsPublic", "false"); // default is true
			}
		}

		#endregion
	}
}