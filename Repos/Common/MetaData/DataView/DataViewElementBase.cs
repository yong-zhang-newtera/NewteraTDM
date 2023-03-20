/*
* @(#)DataViewElementBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A base class that implements IDataViewElement interface and serves as
	/// the base class of other data view elements.
	/// </summary>
	/// 
	/// <version>1.0.1 28 Oct 2003</version>
	/// <author>Yong Zhang</author>
    public abstract class DataViewElementBase : IDataViewElement, ICustomTypeDescriptor
	{
		private bool _isChanged; // run-time use

		/// <summary>
		/// Value changed event handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption Changed event handler
		/// </summary>
		public event EventHandler CaptionChanged;

		private DataViewModel _dataView;
		private string _name;
		private string _caption;
		private string _description;
		private int _position;
		private bool _isReadOnly;
		internal SchemaModelElement _schemaModelElement;
        protected string _xpath = null; // run-time use
        private IDataViewElement _parentElement; // run-time use
        private string _elementAlias = null; // an alias that identifies the element. run-time use

		private PropertyDescriptorCollection _globalizedProperties = null;

		/// <summary>
		/// Initiating an instance of DataViewElementBase class
		/// </summary>
		/// <param name="name">Name of the element</param>
		public DataViewElementBase(string name)
		{
			_dataView = null;
			_name = name;
			_caption = null;
			_description = null;
			_position = 0;
			_isReadOnly = false; // run-time use only, do not save it in xml
			_schemaModelElement = null; // run-time use only, do not save it in xml
            _parentElement = null;
		}

		/// <summary>
		/// Initiating an instance of DataViewElementBase class
		/// </summary>
		internal DataViewElementBase()
		{
			_name = null;
			_caption = null;
			_description = null;
            _parentElement = null;
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		[BrowsableAttribute(false)]		
		public virtual DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
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
		public virtual string Name
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
        public virtual string Description
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
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		[BrowsableAttribute(false)]			
		public bool IsValueChanged
		{
			get
			{
				return _isChanged;
			}
			set
			{
				_isChanged = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the DataViewElement is
		/// readonly
		/// </summary>
		/// <value>true if it is read-only, false otherwise</value>
		[BrowsableAttribute(false)]		
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				_isReadOnly = value;
			}
		}

        /// <summary>
        /// Gets or sets an alias that is used to identifies the element at some situation.
        /// The alias is set by some programs
        /// </summary>
        /// <value>A string, can be null.</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        [BrowsableAttribute(false)]
        public virtual string Alias
        {
            get
            {
                if (!string.IsNullOrEmpty(_elementAlias))
                {
                    return _elementAlias;
                }
                else
                {
                    return Name;
                }
            }
            set
            {
                _elementAlias = value;
            }
        }

        /// <summary>
        /// Gets or sets the data view element that is the parent element in expression tree
        /// </summary>
        /// <value>The data view element</value>
        [BrowsableAttribute(false)]
        public IDataViewElement ParentElement
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public abstract void Accept(IDataViewElementVisitor visitor);

		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public virtual SchemaModelElement GetSchemaModelElement()
		{
			return _schemaModelElement;
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
			if (text != null && text.Length >0)
			{
				_description = text;
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
			if (_description != null && _description.Length > 0)
			{
				parent.SetAttribute("Description", _description);
			}

			parent.SetAttribute(NewteraNameSpace.POSITION, _position.ToString());
		}

		/// <summary>
		/// Fire an event for value change
		/// </summary>
		/// <param name="value">new value</param>
		protected void FireValueChangedEvent(object value)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("DataViewElement", value));
			}
		}

		/// <summary>
		/// Handler for Value Changed event fired by members of a data view model
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected void ValueChangedHandler(object sender, EventArgs e)
		{
			// propagate the event
			if (ValueChanged != null)
			{
				ValueChanged(sender, e);
			}
		}

        /// <summary>
        /// Default behavious, can be overrided by the subclass
        /// </summary>
        /// <returns>true if the property is hidden, false otherwise.</returns>
        protected virtual bool IsHiddenProperty(PropertyDescriptor property)
        {
            return false;
        }

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public virtual string ToXPath()
        {
            return "";
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        [BrowsableAttribute(false)]
        public virtual IXaclObject Parent
        {
            get
            {
                // TODO
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public virtual IEnumerator GetChildren()
        {
            // TODO
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion

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
                    if (!IsHiddenProperty(property))
                    {
                        _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                    }
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