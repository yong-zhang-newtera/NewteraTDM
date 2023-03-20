/*
* @(#)MappingNodeBase.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Mappings.Transform;

	/// <summary> 
	/// The base class for all node in mapping package
	/// </summary>
	/// <version> 1.0.0 03 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public abstract class MappingNodeBase : IMappingNode, ICustomTypeDescriptor
	{
		private string _name;
		private string _caption;
		private string _desc;
		private int _position;
		private ITransformer _transformer = null; // run-time use only

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption Changed event handler
		/// </summary>
		public event EventHandler CaptionChanged;

		private PropertyDescriptorCollection _globalizedProperties = null;
	
		/// <summary>
		/// Initiate an instance of MappingNodeBase class
		/// </summary>
		public MappingNodeBase()
		{
			_name = null;
			_caption = null;
			_desc = null;
			_position = 0;
		}

		/// <summary>
		/// Gets or sets the transformer associated with the mapping node
		/// </summary>
		internal ITransformer Transformer
		{
			get
			{
				return _transformer;
			}
			set
			{
				_transformer = value;
			}
		}

		#region IMappingNode interface implementation
		
		/// <summary>
		/// Gets the name of item
		/// </summary>
		/// <value>The mapping item name</value>
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
		/// Gets or sets the caption of item
		/// </summary>
		/// <value>The item caption.</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The display name of the item"),
		]		
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

				// Raise the event for Caption change
				if (CaptionChanged != null)
				{
					CaptionChanged(this, new ValueChangedEventArgs("Caption", value));
				}

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
				if (_desc != null)
				{
					return _desc;
				}
				else
				{
					// Databindings does not like null
					return "";
				}
			}
			set
			{
				_desc = value;

				// Raise the event for desc change
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

				// Raise the event for position change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract NodeType NodeType {get;}

		/// <summary>
		/// Make a copy of the IMappingNode instance
		/// </summary>
		/// <returns>A copy of IMappingNode instance</returns>
		public virtual IMappingNode Copy()
		{
			// convert this AttributeMapping instance to xml
			XmlDocument doc = new XmlDocument();
			XmlElement xmlElement = doc.CreateElement(MappingNodeFactory.ConvertTypeToString(this.NodeType));
			this.Marshal(xmlElement);

			// Create a copy of IMappingNode from the xml
			IMappingNode copy = MappingNodeFactory.Instance.Create(xmlElement);

			return copy;
		}

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
			if (text != null && text.Length > 0)
			{
				_desc = text;
			}
		
			text = parent.GetAttribute(NewteraNameSpace.POSITION);
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
			if (_desc != null && _desc.Length > 0)
			{
				parent.SetAttribute("Description", _desc);
			}

			parent.SetAttribute(NewteraNameSpace.POSITION, _position.ToString());
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
				ValueChanged(this, new ValueChangedEventArgs("IMappingNode", value));
			}
		}

		#region ICustomTypeDescriptor

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(this,true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this,true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public EventDescriptor GetDefaultEvent() 
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetDefaultProperty() 
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="editorBaseType"></param>
		/// <returns></returns>
		public object GetEditor(Type editorBaseType) 
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents(Attribute[] attributes) 
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <returns></returns>
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		/// <summary>
		/// Called to get the properties of a type.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if ( _globalizedProperties == null) 
			{
				// Get the collection of properties
				PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

				_globalizedProperties = new PropertyDescriptorCollection(null);

				// For each property use a property descriptor of our own that is able to be globalized
				foreach(PropertyDescriptor property in baseProps )
				{
					_globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
				}
			}

			return _globalizedProperties;
		}

		/// <summary>
		/// Our implementation overrides GetProperties() only and creates a
		/// collection of custom property descriptors of type GlobalizedPropertyDescriptor
		/// and returns them to the caller instead of the default ones.
		/// </summary>
		/// <returns>A collection of Property Descriptors.</returns>
		public PropertyDescriptorCollection GetProperties()
		{
			// Only do once
			if (_globalizedProperties == null) 
			{
				// Get the collection of properties
				PropertyDescriptorCollection baseProperties = TypeDescriptor.GetProperties(this, true);

				_globalizedProperties = new PropertyDescriptorCollection(null);

				// For each property use a property descriptor of our own that is able to 
				// be globalized
				foreach (PropertyDescriptor property in baseProperties)
				{
					// create our custom property descriptor and add it to the collection
					_globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
				}
			}

			return _globalizedProperties;
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public object GetPropertyOwner(PropertyDescriptor pd) 
		{
			return this;
		}

		#endregion
	}
}