/*
* @(#)ApiNodeBase.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.ComponentModel;

	/// <summary> 
	/// The base class for all node in Apis package
	/// </summary>
	/// <version> 1.0.0 16 Oct 2015</version>
    public abstract class ApiNodeBase : IApiNode, ICustomTypeDescriptor
	{
        private string _name;

        private PropertyDescriptorCollection _globalizedProperties = null;

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;
	
		/// <summary>
		/// Initiate an instance of ApiNodeBase class
		/// </summary>
		public ApiNodeBase()
		{
            _name = null;
		}

        /// <summary>
        /// Initiate an instance of ApiNodeBase class
        /// </summary>
        public ApiNodeBase(string name)
        {
            _name = name;
        }

		#region IApiNode interface implementation

        /// <summary>
        /// Gets or sets the name of the node
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The name of the item"),
        ]
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
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public abstract ApiNodeType NodeType {get;}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public abstract void Accept(IApiNodeVisitor visitor);

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
            string str = parent.GetAttribute("Name");
            if (str != null && str.Length > 0)
            {
                _name = str;
            }
            else
            {
                _name = null;
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
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
				ValueChanged(this, new ValueChangedEventArgs("IApiNode", value));
			}
		}

        #region ICustomTypeDescriptor

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
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
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to be globalized
                foreach (PropertyDescriptor property in baseProps)
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
        /// Refer to ICustomTypeDescriptor specification
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