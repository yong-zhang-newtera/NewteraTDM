/*
* @(#)XaclSetting.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// This class defines some setting at the policy level 
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class XaclSetting : XaclNodeBase
	{
		private XaclPropagationType _propagationType;
		
		private XaclConflictResolutionType _conflictResolutionType;
		
		private XaclPermissionType _defaultReadPermission;
		
		private XaclPermissionType _defaultWritePermission;
		
		private XaclPermissionType _defaultCreatePermission;
	
		private XaclPermissionType _defaultDeletePermission;

		private XaclPermissionType _defaultUploadPermission;

		private XaclPermissionType _defaultDownloadPermission;
		
		/// <summary>
		/// Initiate an instance of XaclSetting class
		/// </summary>
		public XaclSetting() : base()
		{
			_propagationType = XaclPropagationType.Downward;
			_conflictResolutionType = XaclConflictResolutionType.Dtp;
			_defaultReadPermission = XaclPermissionType.Deny;
			_defaultWritePermission = XaclPermissionType.Deny;
			_defaultCreatePermission = XaclPermissionType.Deny;
			_defaultDeletePermission = XaclPermissionType.Deny;
			_defaultUploadPermission = XaclPermissionType.Deny;
			_defaultDownloadPermission = XaclPermissionType.Deny;
		}

		/// <summary>
		/// Initiating an instance of XaclSetting class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclSetting(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the propagation type
		/// </summary>
		/// <value> One of XaclPropagationType class.</value>
		public XaclPropagationType PropagationType
		{
			get
			{
				return _propagationType;
			}
			set
			{
				_propagationType = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the propagation type
		/// </summary>
		/// <value> One of XaclPropagationType class.</value>
		public XaclConflictResolutionType ConflictResolutionType
		{
			get
			{
				return _conflictResolutionType;
			}
			set
			{
				_conflictResolutionType = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default read permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultReadPermission
		{
			get
			{
				return _defaultReadPermission;
			}
			set
			{
				_defaultReadPermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default write permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultWritePermission
		{
			get
			{
				return _defaultWritePermission;
			}
			set
			{
				_defaultWritePermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default create permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultCreatePermission
		{
			get
			{
				return _defaultCreatePermission;
			}
			set
			{
				_defaultCreatePermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default delete permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultDeletePermission
		{
			get
			{
				return _defaultDeletePermission;
			}
			set
			{
				_defaultDeletePermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default upload permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultUploadPermission
		{
			get
			{
				return _defaultUploadPermission;
			}
			set
			{
				_defaultUploadPermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default download permission.
		/// </summary>
		/// <value> One of XaclPermissionType enum values</value>
		public XaclPermissionType DefaultDownloadPermission
		{
			get
			{
				return _defaultDownloadPermission;
			}
			set
			{
				_defaultDownloadPermission = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Setting;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
        }

		/// <summary>
		/// create an XaclSetting from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_propagationType = (XaclPropagationType) Enum.Parse(typeof(XaclPropagationType), parent.GetAttribute("propagation"));
			_conflictResolutionType = (XaclConflictResolutionType) Enum.Parse(typeof(XaclConflictResolutionType), parent.GetAttribute("conflict"));
			_defaultReadPermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), parent.GetAttribute("read"));
			_defaultWritePermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), parent.GetAttribute("write"));
			_defaultCreatePermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), parent.GetAttribute("create"));
			_defaultDeletePermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), parent.GetAttribute("delete"));
			// since upload and download are added later, we have to handle it specially for
			// sake of backward-compatibility
			string enumName = parent.GetAttribute("upload");
			if (enumName != null && enumName.Length > 0)
			{
				_defaultUploadPermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), enumName);
			}
			else
			{
				_defaultUploadPermission = XaclPermissionType.Deny; // for older version
			}
			enumName = parent.GetAttribute("download");
			if (enumName != null && enumName.Length > 0)
			{
				_defaultDownloadPermission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), enumName);
			}
			else
			{
				_defaultDownloadPermission = XaclPermissionType.Deny; // for older version
			}
		}

		/// <summary>
		/// write XaclSetting to xml document
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			parent.SetAttribute("propagation", Enum.GetName(typeof(XaclPropagationType), _propagationType));
			parent.SetAttribute("conflict", Enum.GetName(typeof(XaclConflictResolutionType), _conflictResolutionType));
			parent.SetAttribute("read", Enum.GetName(typeof(XaclPermissionType), _defaultReadPermission));
			parent.SetAttribute("write", Enum.GetName(typeof(XaclPermissionType), _defaultWritePermission));
			parent.SetAttribute("create", Enum.GetName(typeof(XaclPermissionType), _defaultCreatePermission));
			parent.SetAttribute("delete", Enum.GetName(typeof(XaclPermissionType), _defaultDeletePermission));
			parent.SetAttribute("upload", Enum.GetName(typeof(XaclPermissionType), _defaultUploadPermission));
			parent.SetAttribute("download", Enum.GetName(typeof(XaclPermissionType), _defaultDownloadPermission));
		}
	}
}