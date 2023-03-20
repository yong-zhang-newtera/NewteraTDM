/*
* @(#)XaclNodeBase.cs
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
	/// The base class for all xacl node classes
	/// </summary>
	/// <version> 1.0.0 07 Dec 2003</version>
	/// <author> Yong Zhang </author>
	public abstract class XaclNodeBase : IXaclNode
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;
	
		private string _name;

		/// <summary>
		/// Initiate an instance of XaclNodeBase class
		/// </summary>
		public XaclNodeBase()
		{
			_name = null;
		}

		/// <summary>
		/// Initiate an instance of XaclNodeBase class
		/// </summary>
		/// <param name="name">The name of node</param>
		public XaclNodeBase(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Gets or sets the name of a node.
		/// </summary>
		/// <value>The name of a node</value>
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

		#region IXaclNode interface implementation

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract NodeType NodeType {get;}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public abstract void Accept(IXaclNodeVisitor visitor);
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			// set value of  the name member
			_name = parent.GetAttribute("Name");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			// write the name member
			if (_name != null && _name.Length > 0)
			{
				parent.SetAttribute("Name", _name);
			}
		}

		#endregion

		/// <summary>
		/// Handler for Value Changed event fired by members of a xacl model
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected virtual void ValueChangedHandler(object sender, EventArgs e)
		{
			// propagate the event
			if (ValueChanged != null)
			{
				ValueChanged(sender, e);
			}
		}

		/// <summary>
		/// Fire an event for value change
		/// </summary>
		/// <param name="value"></param>
		protected void FireValueChangedEvent(object value)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("IXaclNode", value));
			}
		}
	}
}