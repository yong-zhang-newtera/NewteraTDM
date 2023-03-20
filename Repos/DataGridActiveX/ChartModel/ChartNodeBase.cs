/*
* @(#)ChartNodeBase.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary> 
	/// The base class for all node in ChartModel package
	/// </summary>
	/// <version> 1.0.0 24 Apr 2006</version>
	public abstract class ChartNodeBase : IChartNode
	{
		private bool _isAltered; // run-time use

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;
	
		/// <summary>
		/// Initiate an instance of ChartNodeBase class
		/// </summary>
		public ChartNodeBase()
		{
		}

		#region IChartNode interface implementation
		
		/// <summary>
		/// Gets or sets the information indicating whether the content of the Node
		/// has been altered or not
		/// </summary>
		/// <value>True when it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract NodeType NodeType {get;}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
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

			this._isAltered = true;
		}

		/// <summary>
		/// Fire an event for value change
		/// </summary>
		/// <param name="value"></param>
		protected void FireValueChangedEvent(object value)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("IChartNode", value));
			}

			this._isAltered = true;
		}
	}
}