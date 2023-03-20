/*
* @(#)DataGridViewElementBase.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;

	/// <summary>
	/// A base class that implements IDataGridViewElement interface and serves as
	/// the base class of other class view elements.
	/// </summary>
	/// 
	/// <version>1.0.1 28 May 2006</version>
	public abstract class DataGridViewElementBase : IDataGridViewElement
	{
		private DataGridViewModel _dataGridView;
		private string _name;
		private string _caption;

		/// <summary>
		/// Initiating an instance of DataGridViewElementBase class
		/// </summary>
		/// <param name="name">Name of the element</param>
		public DataGridViewElementBase(string name)
		{
			_dataGridView = null;
			_name = name;
			_caption = null;
		}

		/// <summary>
		/// Initiating an instance of DataGridViewElementBase class
		/// </summary>
		internal DataGridViewElementBase()
		{
			_name = null;
			_caption = null;
		}

		/// <summary>
		/// Gets or sets the DataGridViewModel that owns this element
		/// </summary>
		/// <value>DataGridViewModel object</value>
		public virtual DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public abstract ViewElementType ElementType {get;}

		/// <summary>
		/// Gets the name of element
		/// </summary>
		/// <value>The class view name</value>	
		public string Name
		{
			get
			{
				if (_name != null)
				{
					return _name;
				}
				else
				{
					return "";
				}
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the caption of element
		/// </summary>
		/// <value>The element caption.</value>	
		public string Caption
		{
			get
			{
				if (_caption == null || _caption.Length == 0)
				{
					return _name;
				}
				else
				{
					return _caption;
				}
			}
			set
			{
				_caption = value;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public abstract void Accept(IDataGridViewElementVisitor visitor);


		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			// set value of  the name member
			string text;
			text = parent.GetAttribute("Name");
			if (text != null && text.Length >0)
			{
				_name = text;
			}

			// set value of the caption
			text = parent.GetAttribute("Caption");
			if (text != null && text.Length >0)
			{
				_caption = text;
			}
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

			// write the caption
			if (_caption != null && _caption.Length > 0)
			{
				parent.SetAttribute("Caption", _caption);
			}
		}
	}
}