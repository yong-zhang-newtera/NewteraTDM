/*
* @(#)ProjectModelElementBase.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Collections;
	using System.ComponentModel;

	//using Newtera.Common.Core;

	/// <summary>
	/// The base class for all other elements that are of project model.
	/// </summary>
	/// <version>  1.0.1 11 Nov 2005</version>
	/// <author>Yong Zhang</author>
	public abstract class ProjectModelElementBase : IProjectModelElement
	{
		private bool _isChanged; // run-time use

		private IProjectModelElement _parentElement;

		/// <summary>
		/// Value changed event handler
		/// </summary>
		public event EventHandler ValueChanged;

		private string _name;
		private string _caption;
		private string _description;
		private int _position;

		/// <summary>
		/// Initiating an instance of ProjectModelElementBase class
		/// </summary>
		/// <param name="name">Name of the element</param>
		public ProjectModelElementBase(string name)
		{
			_name = name;
			_caption = null;
			_description = null;
			_position = 0;
			_parentElement = null;
		}

		/// <summary>
		/// Initiating an instance of ProjectModelElementBase class
		/// </summary>
		internal ProjectModelElementBase()
		{
			_name = null;
			_caption = null;
			_description = null;
			_parentElement = null;
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public abstract ElementType ElementType {get;}

		/// <summary>
		/// Gets the name of element
		/// </summary>
		/// <value>The data view name</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The name of the item"),
		ReadOnlyAttribute(true),
		]		
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

				// Raise the event for caption change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the caption of element
		/// </summary>
		/// <value>The element caption.</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The display name of the item"),
		]		
		public virtual string Caption
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

				// Raise the event for value change
				FireValueChangedEvent(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the description of element
		/// </summary>
		/// <value>The element description.</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The description of the item")
		]		
		public string Description
		{
			get
			{
				if (_description != null)
				{
					return _description;
				}
				else
				{
					// Databindings does not like null
					return "";
				}
			}
			set
			{
				_description = value;

				// Raise the event for caption change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets position of this element among its sibling.
		/// </summary>
		/// <value>A zero-based integer representing the position.</value>
		[BrowsableAttribute(false)]		
		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;

				// Raise the event for value change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the parent element of the element
		/// </summary>
		[BrowsableAttribute(false)]	
		public IProjectModelElement ParentElement
		{
			get
			{
				return _parentElement;
			}
			set
			{
				_parentElement = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		[BrowsableAttribute(false)]			
		public virtual bool IsValueChanged
		{
			get
			{
				return _isChanged;
			}
			set
			{
				_isChanged = value;
				
				// propogate the change up when it is true
				if (this.ParentElement != null)
				{
					this.ParentElement.IsValueChanged = value;
				}
			}
		}

		/// <summary>
		/// Reset the value change status in the project model
		/// </summary>
		public virtual void Reset()
		{
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse the project model tree.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public abstract void Accept(IProjectModelElementVisitor visitor);


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

			// set value of the description
			text = parent.GetAttribute("Description");
			if (text != null && text.Length >0)
			{
				_description = text;
			}
		
			text = parent.GetAttribute("Position");
			if (text != null && text.Length > 0)
			{
				_position = int.Parse(text);
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

			// write the description
			if (_description != null && _description.Length > 0)
			{
				parent.SetAttribute("Description", _description);
			}

			parent.SetAttribute("Position", _position.ToString());
		}

		/// <summary>
		/// Fire an event for value change
		/// </summary>
		/// <param name="value">new value</param>
		internal void FireValueChangedEvent(object value)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ProjectModelElement", value));
			}
		}

		/// <summary>
		/// Handler for Value Changed event fired by members of a project data model
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected void ValueChangedHandler(object sender, EventArgs e)
		{
			IsValueChanged = true;
		}
	}
}