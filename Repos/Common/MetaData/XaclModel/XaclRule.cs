/*
* @(#)XaclRule.cs
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
	/// The class represents an xacl rule in an XaclDef that includes
	/// an XaclSubject, A set of XaclAction, and XaclCondition.
	/// </summary>
	/// <version>1.0.0 11 Dec 2003</version>
	/// <author> Yong Zhang </author>
	public class XaclRule : XaclNodeBase
	{
		private XaclSubject _subject;
		
		private XaclActionCollection _actions;
		
		private XaclCondition _condition;

		private bool _allowPropagation;

		private bool _isOverrided; // whether to override inherited rule

		private string _href; // Run-time use only, do not write to xml
		
		/// <summary>
		/// Initiate an instance of XaclRule class.
		/// </summary>
		/// <param name="subject"> XaclSubject object </param>
		public XaclRule(XaclSubject subject) : this(subject, null)
		{
		}

		/// <summary>
		/// Initiate an instance of XaclRule class.
		/// </summary>
		/// <param name="subject"> XaclSubject object </param>
		/// <param name="href">The href of the rule</param>
		public XaclRule(XaclSubject subject, string href) : base()
		{
			_subject = subject;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subject.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_actions = CreateDefaultActions();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _actions.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_condition = new XaclCondition();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _condition.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_allowPropagation = true;
			_isOverrided = false;
			_href = href;
		}

		/// <summary>
		/// Initiating an instance of XaclRule class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclRule(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>An XaclActionCollection instance.</value>
		public XaclActionCollection Actions
		{
			get
			{
				return _actions;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a rule has a condition
		/// </summary>
		/// <value>true if it has condition, false otherwise</value>
		public bool HasCondition
		{
			get
			{
				if (this.Condition.Condition != null && this.Condition.Condition.Length > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a read permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the read permission is granted, false if denied</value>
		public bool IsReadGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Read) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
				}
				else
				{
					// Denying read permission will also denying write, create,
					// delete, upload, and download permission
					SetPermission(XaclActionType.Read, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Download, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a read permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the read permission is denied, false if granted</value>
		public bool IsReadDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Read) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// Denying read permission will also denying write, create,
					// delete, upload, and download permission
					SetPermission(XaclActionType.Read, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Download, XaclPermissionType.Deny);
				}
				else
				{
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a write permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the write permission is granted, false if denied</value>
		
		public bool IsWriteGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Write) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// grant a write permission will grant a read permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
				}
				else
				{
					// Denying write permission will also denying create,
					// delete and upload permission automatically
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a write permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the write permission is denied, false if granted</value>
		public bool IsWriteDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Write) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// Denying write permission will also denying create, 
					// delete, and upload permission
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
				}
				else
				{
					// grant a write permission will grant a read, upload, download permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a create permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the create permission is granted, false if denied</value>
		public bool IsCreateGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Create) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// grant a create permission will grant a read, write, upload, and download permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Create, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
				else
				{
					// Denying create permission will also denying delete permission automatically
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a create permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the create permission is denied, false if granted</value>
		public bool IsCreateDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Create) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// Denying create permission will also denying delete permission
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
				else
				{
					// grant a create permission will grant a read, write, upload, and download permissions automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Create, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a delete permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the delete permission is granted, false if denied</value>
		public bool IsDeleteGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Delete) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// grant a delete permission will grant a read, write, upload, download, and create permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Create, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
				else
				{
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a delete permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the delete permission is denied, false if granted</value>
		public bool IsDeleteDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Delete) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
				else
				{
					// grant a delete permission will grant a read, download, upload, write and create permissions automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Create, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
			}
		}


		/// <summary>
		/// Gets or sets the information indicating whether a upload permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the upload permission is granted, false if denied</value>
		public bool IsUploadGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Upload) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// grant a upload permission will also grant a read, write, and download permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
				else
				{
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a upload permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the upload permission is denied, false if granted</value>
		public bool IsUploadDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Upload) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
				}
				else
				{
					// grant a upload permission will also grant a read, download, upload, and write permissions automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Write, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a download permission of a rule
		/// is granted.
		/// </summary>
		/// <value>true if the download permission is granted, false if denied</value>
		public bool IsDownloadGranted
		{
			get
			{
				if (GetPermission(XaclActionType.Download) == XaclPermissionType.Grant)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					// grant a download permission will also grant a read permission automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
				else
				{
					// Denying download permission will also denying read, write, create,
					// delete, upload, and download permission
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Download, XaclPermissionType.Deny);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether a download permission of a rule
		/// is denied.
		/// </summary>
		/// <value>true if the download permission is denied, false if granted</value>
		public bool IsDownloadDenied
		{
			get
			{
				if (GetPermission(XaclActionType.Download) == XaclPermissionType.Deny)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (value)
				{
					SetPermission(XaclActionType.Download, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
					SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
				}
				else
				{
					// grant a download permission will also grant a read permissions automatically
					SetPermission(XaclActionType.Read, XaclPermissionType.Grant);
					SetPermission(XaclActionType.Download, XaclPermissionType.Grant);
				}
			}
		}

		/// <summary>
		/// Gets or sets the condition of an XaclRule.
		/// </summary>
		/// <value> the XaclCondition object.</value>
		public XaclCondition Condition
		{
			get
			{
				return _condition;
			}
		}

		/// <summary>
		/// Gets the XaclSubject of The XaclRule. 
		/// </summary>
		public XaclSubject Subject
		{
			get
			{
				return _subject;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the rule is allowed to
		/// propagate
		/// </summary>
		/// <value>true if it is allowed to propagate, false otherwise</value>
		public bool AllowPropagation
		{
			get
			{
				return _allowPropagation;
			}
			set
			{
				_allowPropagation = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the rule overrides the
		/// inherited rule with the same subject
		/// </summary>
		/// <value>true if it overrides the inherited rule, false otherwise. Default is false.</value>
		public bool IsOverrided
		{
			get
			{
				return _isOverrided;
			}
			set
			{
				_isOverrided = value;
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
				return NodeType.Rule;
			}
		}

		/// <summary>
		/// Gets the permission of the rule for an action type
		/// </summary>
		/// <param name="actionType">One of XaclActionType values</param>
		/// <returns>One of XaclPermissionType values</returns>
		public XaclPermissionType GetPermission(XaclActionType actionType)
		{
			XaclPermissionType permission = XaclPermissionType.Unknown;

			foreach (XaclAction action in _actions)
			{
				if (action.ActionType == actionType)
				{
					permission = action.Permission;
					break;
				}
			}

			return permission;
		}

		/// <summary>
		/// Sets the permission of the rule for an action type
		/// </summary>
		/// <param name="actionType">One of XaclActionType values</param>
		/// <param name="permission">One of XaclPermissionType</param>
		public void SetPermission(XaclActionType actionType, XaclPermissionType permission)
		{
			foreach (XaclAction action in _actions)
			{
				if (action.ActionType == actionType)
				{
					action.Permission = permission;
					break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the href of the XaclObject to which the rule is
		/// associated with.
		/// </summary>
		/// <value>A href string</value>
		public string ObjectHref
		{
			get
			{
				return _href;
			}
			set
			{
				_href = value;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            if (visitor.VisitXaclRule(this))
            {
                _subject.Accept(visitor);
                _actions.Accept(visitor);
                _condition.Accept(visitor);
            }
        }

		/// <summary>
		/// create an XaclRule from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("propagate");
			if (str != null && str == "false")
			{
				_allowPropagation = false;
			}
			else
			{
				_allowPropagation = true;
			}

			str = parent.GetAttribute("override");
			if (str != null && str == "true")
			{
				_isOverrided = true;
			}
			else
			{
				_isOverrided = false; // default
			}


			// the first child is XaclSubject
			_subject = (XaclSubject) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subject.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of XaclAction instances
			_actions = (XaclActionCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _actions.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// new actions added later may not be saved, therefore,
			// we have to handle this case for sake of backward-compatibility
			bool found = false;
			XaclAction action;

			foreach (XaclAction act in _actions)
			{
				if (act.ActionType == XaclActionType.Upload)
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				// default permission for upload is same as Write
				action = new XaclAction(XaclActionType.Upload, GetPermission(XaclActionType.Write));
				_actions.Add(action);
			}

			found = false;
			foreach (XaclAction act in _actions)
			{
				if (act.ActionType == XaclActionType.Download)
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				// default permission for download is the same as Read
				action = new XaclAction(XaclActionType.Download, GetPermission(XaclActionType.Read));
				_actions.Add(action);
			}

			// then a XaclCondition
			_condition = (XaclCondition) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _condition.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write XaclRule to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (!_allowPropagation)
			{
				parent.SetAttribute("propagate", "false");
			}

			if (_isOverrided)
			{
				parent.SetAttribute("override", "true");
			}

			// write the _subject
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_subject.NodeType));
			_subject.Marshal(child);
			parent.AppendChild(child);

			// write the actions
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_actions.NodeType));
			_actions.Marshal(child);
			parent.AppendChild(child);

			// write the _condition
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_condition.NodeType));
			_condition.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Create default actions for read, write, create, delete, upload, and download.
		/// </summary>
		/// <returns>A collection of XaclAction</returns>
		private XaclActionCollection CreateDefaultActions()
		{
			XaclActionCollection actions = new XaclActionCollection();
			XaclAction action;

			action = new XaclAction(XaclActionType.Read, XaclPermissionType.Grant);
			actions.Add(action);

			action = new XaclAction(XaclActionType.Write, XaclPermissionType.Grant);
			actions.Add(action);

			action = new XaclAction(XaclActionType.Create, XaclPermissionType.Grant);
			actions.Add(action);

			action = new XaclAction(XaclActionType.Delete, XaclPermissionType.Grant);
			actions.Add(action);

			action = new XaclAction(XaclActionType.Upload, XaclPermissionType.Grant);
			actions.Add(action);

			action = new XaclAction(XaclActionType.Download, XaclPermissionType.Grant);
			actions.Add(action);

			return actions;
		}
	}
}