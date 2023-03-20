/*
* @(#)SchemaModelElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Collections;
	using System.ComponentModel;
    using System.Security;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Provides the base functionality for creating Schema Model Element
	/// </summary>
	/// <version>  1.0.1 26 Jun 2003</version>
	/// <author>  Yong Zhang</author>
	/// <remarks>
	/// SchemaModelElement implements ICustomTypeDescriptor so that it can
	/// return the property descriptors with localized display name 
	/// </remarks>
	public abstract class SchemaModelElement : IMetaDataElement, IXaclObject, ICustomTypeDescriptor
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Caption changed handler
		/// </summary>
		public event EventHandler CaptionChanged;

		private XmlSchemaAnnotated _xmlSchemaElement;
		private SchemaModel _schemaModel = null;
		private string _id = null;
		private string _name = null;
		private string _caption = null;
		private string _description = null;
		private int _position = 0;
		private string _attributesString = null;
		internal string _xpath = null; // run-time member
        private bool _eventDisabled = false; // run-time use

		private PropertyDescriptorCollection _globalizedProperties = null;

		/// <summary>
		/// Return the value of an attribute of Newtera Namespace. If the attribute
		/// does not exist, return null.
		/// </summary>
		/// <param name="xmlSchemaNode">schema node</param>
		/// <param name="attributeName">the name of attribute</param>
		/// <returns> Value of the attribute</returns>
		internal static string GetNewteraAttributeValue(XmlSchemaAnnotated xmlSchemaNode, string attributeName)
		{
			string value = null;

			XmlAttribute[] attributes = xmlSchemaNode.UnhandledAttributes;

			foreach (XmlAttribute attribute in attributes) 
			{
				if (attribute.Prefix == NewteraNameSpace.PREFIX &&
					attribute.LocalName == attributeName)
				{
					value = attribute.Value;
					break;
				}
			}
			return value;
		}

		/// <summary> Initializes a new instance of the SchemaModelElement class.
		/// </summary>
		/// <param name="name">The name of Schema model element</param>
		public SchemaModelElement(string name)
		{
			//_xmlSchemaElement = CreateXmlSchemaElement(name);
			_name = name;
		}

		/// <summary> Initializes a new instance of the SchemaModelElement class.
		/// </summary>
		/// <param name="element">The XmlSchemaAnnotated element</param>
		public SchemaModelElement(XmlSchemaAnnotated element)
		{
			_xmlSchemaElement = element;
		}

		/// <summary> Gets or sets name of the element.
		/// </summary>
		/// <value> The name of the element</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The name of the item"),
		ReadOnlyAttribute(true),
		]
		public string Name
		{
			get
			{
				if (_name == null)
				{
					_name = ElementName;
				}

				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary> Gets or sets id of the element.
		/// </summary>
		/// <value> The id of the element</value>
		/// 
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

		/// <summary> Gets or sets description of a Schema Model Element.
		/// 
		/// </summary>
		/// <value> The description of the element</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The description of the item"),
		]
		public string Description
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
		/// Gets or sets the caption for the element
		/// </summary>
		/// <value> The caption of the element. If not set, returns the Name value.</value>
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
					return this.Name;
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

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the schema model that the class belongs to.
		/// </summary>
		/// <value> The SchemaModel object</value>
		[BrowsableAttribute(false)]
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
			set
			{
				_schemaModel = value;
			}
		}

		/// <summary>
		/// Gets the XmlSchemaElement
		/// </summary>
		/// <value>The XmlSchemaAnnotated object</value> 
		[BrowsableAttribute(false)]
		public XmlSchemaAnnotated XmlSchemaElement
		{
			get
			{
				if (_xmlSchemaElement == null)
				{
					_xmlSchemaElement = CreateXmlSchemaElement(_name);
				}

				return _xmlSchemaElement;
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether firing event is disabled.
        /// </summary>
        /// <value>true if it is disabled, false, otherwise.</value>
        [BrowsableAttribute(false)]
        public bool IsEventDisabled
        {
            get
            {
                return _eventDisabled;
            }
            set
            {
                _eventDisabled = value;
            }
        }
		
		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns> String representation of element</returns>
		public override string ToString()
		{
            if (_xmlSchemaElement != null)
            {
                return _xmlSchemaElement.ToString();
            }
            else
            {
                return "";
            }
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
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Return the name of the element to be overrided by derived classes
		/// </summary>
		[BrowsableAttribute(false)]
		protected abstract string ElementName
		{
			get;
		}

		/// <summary>
		/// Create a Xml Schema Element that represents the Schema Model element.
		/// The method must be override by the subclass.
		/// </summary>
		/// <param name="name">The name of element</param>
		/// <returns>The created XmlSchemaAnnotated object</returns>
		protected abstract XmlSchemaAnnotated CreateXmlSchemaElement(string name);
		
		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself and
		/// let its children to accept the visitor next.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public abstract void Accept(ISchemaModelElementVisitor visitor);

		/// <summary>
		/// Return the value of an attribute of Newtera Namespace. If the attribute
		/// does not exist, return null.
		/// </summary>
		/// <param name="attributeName">the name of attribute</param>
		/// <returns> Value of the attribute</returns>
		protected string GetNewteraAttributeValue(string attributeName)
		{
			string value = null;

			XmlAttribute[] attributes = _xmlSchemaElement.UnhandledAttributes;

			foreach (XmlAttribute attribute in attributes) 
			{
				if (attribute.Prefix == NewteraNameSpace.PREFIX &&
					attribute.LocalName == attributeName)
				{
					value = attribute.Value;
					break;
				}
			}

			return value;
		}

        /// <summary>
        /// Gets the appinfo content
        /// </summary>
        /// <param name="appInfoName">the name of AppInfo</param>
        /// <returns> content of the appInfo</returns>
        protected string GetNewteraAppInfoContent(string appInfoName)
        {
            string value = null;
            StringBuilder builder = new StringBuilder();

            XmlSchemaAnnotation annotation = _xmlSchemaElement.Annotation;

            if (annotation != null)
            {
                foreach (XmlSchemaObject item in annotation.Items)
                {
                    if (item is XmlSchemaAppInfo)
                    {
                        XmlSchemaAppInfo appInfo = (XmlSchemaAppInfo)item;
                        foreach (XmlNode node in appInfo.Markup)
                        {
                            builder.Append(node.InnerText);
                        }
                    }
                }

                value = builder.ToString();
            }

            return value;
        }

        /// <summary>
        /// Set the appinfo content
        /// </summary>
        /// <param name="appInfoName">the name of AppInfo</param>
        /// <param name="content">The appInfo content</param>
        protected void SetNewteraAppInfoContent(string appInfoName, string content)
        {
            XmlSchemaAnnotation annotation = _xmlSchemaElement.Annotation;
            if (annotation == null)
            {
                annotation = new XmlSchemaAnnotation();
                _xmlSchemaElement.Annotation = annotation;
            }

            if (annotation.Items.Count > 0)
            {
                // currently there is only one item dedicated for virtual attribute code,
                // thus, we simply clear all content before set a new content
                annotation.Items.Clear(); 
            }

            XmlSchemaAppInfo appInfo = new XmlSchemaAppInfo();
            annotation.Items.Add(appInfo);
            appInfo.Markup = TextToNodeArray(content);
        }

        /// <summary>
        /// Convert a text to XmlNode Array
        /// </summary>
        /// <param name="text"></param>
        /// <returns>XmlNode array</returns>
        internal XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] { doc.CreateTextNode(text) };
        }

		/// <summary>
		/// Sets the value of an attribute of Newtera Namespace. If attribute does not
		/// exist, create an attribute for the given value.
		/// </summary>
		/// <param name="attributeName">the name of attribute</param>
		/// <param name="attributeValue">the value of attribute</param>
		protected void SetNewteraAttributeValue(string attributeName, string attributeValue)
		{
			if (attributeValue != null)
			{
				string attributeExp = NewteraNameSpace.PREFIX + ":" + attributeName + "=\"" + attributeValue + "\" ";
				if (_attributesString == null)
				{
					_attributesString = attributeExp;
				}
				else
				{
					_attributesString += attributeExp;
				}
			}
		}

		/// <summary>
		/// Create an array of XmlAttributes for Newtera domain attribute and set it
		/// as unhandled attribute to the xml schema element.
		/// </summary>
		protected void SetUnhandledAttributes()
		{
			if (_attributesString != null)
			{
				StringBuilder buf = new StringBuilder();

				buf.Append("<doc xmlns:").Append(NewteraNameSpace.PREFIX).Append("='http://www.newtera.com' ");
				buf.Append(_attributesString);
				buf.Append("/>");

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(buf.ToString());

				XmlAttributeCollection attributes = doc.DocumentElement.Attributes;
				
				XmlAttribute[] attributeArray = new XmlAttribute[attributes.Count];
				attributes.CopyTo(attributeArray, 0);
				_xmlSchemaElement.UnhandledAttributes = attributeArray;
			}		
		}

		/// <summary>
		/// Clear the element for a fresh start.
		/// </summary>
		internal void Clear()
		{
			_xmlSchemaElement = null;
			_attributesString = null;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal virtual void Unmarshal()
		{
			_name = ElementName;
			_id = GetNewteraAttributeValue(NewteraNameSpace.ID);
			_caption = GetNewteraAttributeValue(NewteraNameSpace.CAPTION);
			_description = GetNewteraAttributeValue(NewteraNameSpace.DESCRIPTION);
			string posStr = GetNewteraAttributeValue(NewteraNameSpace.POSITION);
			if (posStr != null)
			{
				_position = int.Parse(posStr);
			}
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal virtual void Marshal()
		{
			// The name is set as part of element
			SetNewteraAttributeValue(NewteraNameSpace.ID, _id);
            SetNewteraAttributeValue(NewteraNameSpace.CAPTION, SecurityElement.Escape(_caption));
			SetNewteraAttributeValue(NewteraNameSpace.DESCRIPTION, (_description != null? SecurityElement.Escape(_description) : null));
			SetNewteraAttributeValue(NewteraNameSpace.POSITION, _position.ToString());

			// Always call this last
			SetUnhandledAttributes();
		}

		/// <summary>
		/// Handler for Value Changed event fired by members of a schema model element
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected void ValueChangedHandler(object sender, EventArgs e)
		{
			// propagate the event
            if (ValueChanged != null && !_eventDisabled)
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
            if (ValueChanged != null && !_eventDisabled)
			{
				ValueChanged(this, new ValueChangedEventArgs("SchemaModelElement", value));
			}
		}

        /// <summary>
        /// Fire an event for value change
        /// </summary>
        /// <param name="property">The name of property whose value is changed</param>
        /// <param name="newValue">The new property value</param>
        /// <param name="oldValue">The old property value</param>
        protected void FireValueChangedEvent(object sender, string property, object newValue, object oldValue)
        {
            if (ValueChanged != null && !_eventDisabled)
            {
                ValueChanged(sender, new ValueChangedEventArgs(property, newValue, oldValue));
            }
        }

		#region ICustomTypeDescriptor

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(this,true);
		}

		/// <summary>
		/// Refer to ICustomTypeDescriptor specification
		/// </summary>
		/// <returns></returns>
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this,true);
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