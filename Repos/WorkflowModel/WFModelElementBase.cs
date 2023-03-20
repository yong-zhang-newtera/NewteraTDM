/*
* @(#)WFModelElementBase.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;

	/// <summary>
	/// Base class for all other IWFModelElement implementations
	/// </summary>
	/// <version>  1.0.0 26 Dec 2006</version>
    public abstract class WFModelElementBase : IWFModelElement, ICustomTypeDescriptor
	{
        internal const string POSITION = "order";

        private PropertyDescriptorCollection _globalizedProperties = null;

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

        private string _id = null;
		private string _name = null;
		private string _description = null;
		private int _position = 0;
        private bool _isAltered = true; // run-time value

		/// <summary> Initializes a new instance of the WFModelElementBase class.
		/// </summary>
		/// <param name="name">The name of Schema model element</param>
		public WFModelElementBase(string name)
		{
			//_xmlSchemaElement = CreateXmlSchemaElement(name);
			_name = name;
		}

        /// <summary>
		/// Initiating an instance of WFModelElementBase class
		/// </summary>
        internal WFModelElementBase()
		{
		}

        /// <summary> Gets or sets id of the element.
        /// </summary>
        /// <value> The id of the element</value>
        [BrowsableAttribute(false)]	
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

		/// <summary> Gets or sets name of the element.
		/// </summary>
		/// <value> The name of the element</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The name of the item"),
		ReadOnlyAttribute(true),
		]
		public virtual string Name
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
        /// Gets or sets description of an element.
		/// </summary>
		/// <value> The description of an element</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The description of the item"),
		]
		public virtual string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Gets or sets position of this element among its siblings.
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

				FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the project has been altered or not.
        /// </summary>
        [BrowsableAttribute(false)]	
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
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public abstract ElementType ElementType { get;}

		/// <summary>
		/// Accept a visitor of IWFModelElementVisitor type to visit itself and
		/// let its children to accept the visitor next.
		/// </summary>
		/// <param name="visitor">The visitor</param>
        public virtual void Accept(IWFModelElementVisitor visitor)
        {
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public virtual void Unmarshal(XmlElement parent)
        {
            // set value of  the name member
            string text;

            text = parent.GetAttribute("ID");
            if (text != null)
            {
                _id = text;
            }
            else
            {
                _id = null;
            }

            text = parent.GetAttribute("Name");
            if (text != null && text.Length > 0)
            {
                _name = text;
            }

            // set value of the description
            text = parent.GetAttribute("Description");
            if (text != null && text.Length > 0)
            {
                _description = text;
            }

            text = parent.GetAttribute(WFModelElementBase.POSITION);
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
            if (_id != null && _id.Length > 0)
            {
                parent.SetAttribute("ID", _id);
            }

            if (_name != null && _name.Length > 0)
            {
                parent.SetAttribute("Name", _name);
            }

            // write the description
            if (_description != null && _description.Length > 0)
            {
                parent.SetAttribute("Description", _description);
            }

            parent.SetAttribute(WFModelElementBase.POSITION, _position.ToString());
		}

		/// <summary>
		/// Handler for Value Changed event fired by members of a schema model element
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
				ValueChanged(this, null);
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