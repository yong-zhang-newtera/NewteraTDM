/*
* @(#)XaclAction.cs
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
	/// The class represents an action in an XaclRule object.
	/// </summary>
	/// <version> 1.0.0 10 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class XaclAction : XaclNodeBase
	{
		private XaclActionType _actionType = XaclActionType.Unknown;
		
		private XaclPermissionType _permission = XaclPermissionType.Unknown;
		
		/// <summary>
		/// Constructor  with parameters.
		/// </summary>
		/// <param name="actionType">action type</param>
		/// <param name="permissionType"> XaclPermissionType </param>
		public XaclAction(XaclActionType actionType, XaclPermissionType permissionType): base()
		{
			_actionType = actionType;
			_permission = permissionType;
		}

		/// <summary>
		/// Initiating an instance of XaclAction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclAction(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the value of permission of the action element.
		/// </summary>
		public XaclPermissionType Permission
		{
			get
			{
				return _permission;
			}
			set
			{
				_permission = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the action type.
		/// </summary>
		/// <value>One of the XaclActionType values.</value>
		public virtual XaclActionType ActionType
		{
			get
			{
				return _actionType;
			}
			set
			{
				_actionType = value;
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
				return NodeType.Action;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            visitor.VisitXaclAction(this);
        }

		/// <summary>
		/// create an XaclAction from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_actionType = (XaclActionType) Enum.Parse(typeof(XaclActionType), parent.GetAttribute("type"));
			_permission = (XaclPermissionType) Enum.Parse(typeof(XaclPermissionType), parent.GetAttribute("permission"));
		}

		/// <summary>
		/// write XaclAction to xml document
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			parent.SetAttribute("type", Enum.GetName(typeof(XaclActionType), _actionType));
			parent.SetAttribute("permission", Enum.GetName(typeof(XaclPermissionType), _permission));
		}
	}
}