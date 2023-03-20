/*
* @(#)EnumElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.ComponentModel;
	using System.Drawing.Design;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml.Schema;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	
	/// <summary>
	/// The EnumElement represents an enumeration constraint. 
	/// </summary>
	/// <version>  	1.0.1 26 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// </author>
    public class EnumElement : ConstraintElementBase, IEnumConstraint
	{
		/// <summary>
		/// Constant definition for enum value SEPARATOR
		/// </summary>
		public const string SEPARATOR = ";";

		// The list of enumeration's values
		private EnumValueCollection _values;
		private bool _isMultipleSelection = false;
        private EnumDisplayMode _displayMode = EnumDisplayMode.Text;
		
		/// <summary>
		/// Get info if the xmlSchemaElemet represents an enumeration constraint
		/// </summary>
		/// <param name="xmlSchemaElement">the XmlSchemaAnnotated object
		/// </param>
		/// <returns>
		/// return true if the element represents an enumeration constraint, 
		/// otherwise, false.
		/// </returns>
		static public bool isEnum(XmlSchemaAnnotated xmlSchemaElement)
		{
			bool status = false;
			
			if (xmlSchemaElement is XmlSchemaSimpleType)
			{
				XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) xmlSchemaElement).Content;
				
				if (restriction != null)
				{
					if (restriction.Facets.Count > 0 &&
						restriction.Facets[0] is XmlSchemaEnumerationFacet)
					{
						status = true;
					}
				}
			}
			
			return status;
		}

        /// <summary>
        /// Compare the values of two enum
        /// </summary>
        /// <param name="firstEnum"></param>
        /// <param name="secondEnum"></param>
        /// <returns>return 1 if the firstEnum includes all values of the second enum,
        /// -1 if the first enum doesn't include all values of the second enum</returns>
        static public int Compare(EnumElement firstEnum, EnumElement secondEnum)
        {
            int result = 1;
            foreach (EnumValue oldValue in secondEnum.Values)
            {
                bool found = false;
                foreach (EnumValue newValue in firstEnum.Values)
                {
                    if (newValue.Value == oldValue.Value &&
                        newValue.DisplayText == oldValue.DisplayText)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    result = -1;
                    break;
                }
            }

            return result;
        }

		/// <summary>
		/// Initializing an EnumElement object
		/// </summary>
		/// <param name="name">Name of element</param>
		public EnumElement(string name): base(name)
		{
			_values = new EnumValueCollection();
			DataType = DataType.String; // default type for Enum
		}

		/// <summary>
		/// Initializing an EnumElement object
		/// </summary>
		/// <param name="xmlSchemaElement">The XmlSchemaAnnotated object</param>
		internal EnumElement(XmlSchemaAnnotated xmlSchemaElement): base(xmlSchemaElement)
		{
			_values = new EnumValueCollection();
		}

		/// <summary>
		/// Gets or sets the enumeration values.
		/// </summary>
		/// <value>
		/// An collection of strings of enumeration values
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("A collection of enumeration values"),
			EditorAttribute("Newtera.Studio.EnumValueCollectionEditor, Studio", typeof(UITypeEditor)),
		]		
		public EnumValueCollection Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;

				FireValueChangedEvent(value); // fire a Value changed event

				// The enum values have been changed, change the enum type name so that
				// EnumTypeFactory will create a new enum type next time it is called
				ChangeEnumTypeName();
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether it allows
		/// multiple selection of enum values.
		/// </summary>
		/// <value>
		/// True if it is multiple selection, false for single selection. Default is false.
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Allow multiple selection?"),
			DefaultValueAttribute(false)
		]		
		public bool IsMultipleSelection
		{
			get
			{
				return _isMultipleSelection;
			}
			set
			{
                bool oldValue = _isMultipleSelection;
				if (value)
				{
					this.DataType = DataType.Integer;
				}

				_isMultipleSelection = value;

                // Raise the event for selection changing
                if (oldValue != value)
                {
                    FireValueChangedEvent(this, "IsMultipleSelection", value, oldValue);
                }
			}
		}

        /// <summary>
        /// Gets or sets the display method of enum values
        /// </summary>
        /// <value>
        /// One of the EnumDisplayMode enum values
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Display method of enum values"),
            DefaultValueAttribute(EnumDisplayMode.Text)
        ]
        public EnumDisplayMode DisplayMode
        {
            get
            {
                return _displayMode;
            }
            set
            {
                _displayMode = value;
            }
        }

        /// <summary>
        /// Gets the information indicates whether the enum values of the constraint are conditional
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsConditionBased
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the information indicates whether the values of the constraint are user-based
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsUserBased
        {
            get
            {
                return false;
            }
        }
		
		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitEnumElement(this);
		}

		/// <summary>
		/// Add an enumeration value.
		/// </summary>
		/// <param name="value">a value in EnumValue
		/// </param>
		public void AddValue(EnumValue enumValue)
		{
			if (_values.Contains(enumValue)) 
			{
				throw new DuplicateValueException("The value " + enumValue.Value + " is already exists.");
			}

			_values.Add(enumValue);		
		}
		
		/// <summary>
		/// Remove an enumeration value.
		/// </summary>
		/// <param name="value">The enumeration value to be removed
		/// </param>
		public void RemoveValue(EnumValue value)
		{
			_values.Remove(value);
		}

		/// <summary>
		/// Gets the information indicating whether the given value is valid
		/// based on the constraint.
		/// </summary>
		/// <param name="value">The given value</param>
		/// <returns>true if it is valid, false otherwise</returns>
		public override bool IsValueValid(string value)
		{
			if (this.IsMultipleSelection)
			{
				bool status = true;

				string[] values = GetStringArray(value);

				for (int i = 0; i < values.Length; i++)
				{
					if (!Values.Contains(values[i]))
					{
						status = false;
						break;
					}
				}

				return status;
			}
			else
			{
				return Values.Contains(value);
			}
        }

        #region IEnumConstraint

        /// <summary>
        /// Convert an enum display text to its value.
        /// </summary>
        /// <param name="text">An enum display text.</param>
        /// <returns>The corresponding enum value</returns>
        public string GetValue(string text)
        {
            string val = "";
            EnumValueCollection enumValues = Values;
            if (enumValues != null)
            {
                foreach (EnumValue enumValue in enumValues)
                {
                    if (enumValue.DisplayText == text)
                    {
                        val = enumValue.Value;
                        break;
                    }
                }
            }

            return val;
        }

        /// <summary>
        /// Gets the corresponsing image name of an enum value
        /// </summary>
        /// <param name="enumValue">enum value</param>
        /// <returns>The image name, could be null</returns>
        public string GetImageName(string val)
        {
            string imageName = null;
            EnumValueCollection enumValues = Values;
            if (enumValues != null)
            {
                foreach (EnumValue enumValue in enumValues)
                {
                    if (enumValue.Value == val)
                    {
                        imageName = enumValue.ImageName;
                        break;
                    }
                }
            }

            return imageName;
        }

        /// <summary>
        /// Gets the corresponsing image name of an enum text
        /// </summary>
        /// <param name="text">enum text</param>
        /// <returns>The image name, can be null</returns>
        public string GetImageNameByText(string text)
        {
            string imageName = null;
            EnumValueCollection enumValues = Values;
            if (enumValues != null)
            {
                foreach (EnumValue enumValue in enumValues)
                {
                    if (enumValue.DisplayText == text)
                    {
                        imageName = enumValue.ImageName;
                        break;
                    }
                }
            }

            return imageName;
        }

        #endregion

        /// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			_values.Clear();

			EnumValue enumValue;
			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
			foreach (XmlSchemaEnumerationFacet facet in restriction.Facets)
			{
				enumValue = new EnumValue();
				enumValue.Value = facet.Value;
				enumValue.DisplayText = facet.Id;
                if (facet.Annotation != null &&
                    facet.Annotation.Items.Count > 0)
                {
                    XmlSchemaAppInfo appInfo = (XmlSchemaAppInfo)(XmlSchemaAppInfo)facet.Annotation.Items[0];
                    foreach (XmlNode node in appInfo.Markup)
                    {
                        enumValue.ImageName = node.InnerText;
                    }
                }
                  
				AddValue(enumValue);
			}

			// Set isMultipleSelection member
			string status = GetNewteraAttributeValue(NewteraNameSpace.MULTI_SELECTION);
			_isMultipleSelection = (status != null && status == "true" ? true : false);

            //Set display mode
            string mode = GetNewteraAttributeValue(NewteraNameSpace.DISPLAY_MODE);
            if (mode != null)
            {
                _displayMode = (EnumDisplayMode) Enum.Parse(typeof(EnumDisplayMode), mode);
            }
            else
            {
                _displayMode = EnumDisplayMode.Text;
            }
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <!--
		/// Enum in xml schema:
		/// 
		/// <xsd:simpleType name="countryType">
		/// <xsd:restriction base="xsd:string">
		/// <xsd:maxLength value="2"/>
		/// <xsd:enumeration value="CN"/>
		/// <xsd:enumeration value="UK"/>
		/// <xsd:enumeration value="US"/>
		/// <xsd:enumeration value="FR"/>
		/// </xsd:restriction>
		/// </xsd:simpleType>
		/// -->
		internal override void Marshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) XmlSchemaElement).Content;
			foreach (EnumValue enumValue in _values)
			{
				XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
				enumeration.Value = enumValue.Value;
				enumeration.Id = enumValue.DisplayText;
                if (!string.IsNullOrEmpty(enumValue.ImageName))
                {
                    // save the image name as annotation
                    XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();
                    enumeration.Annotation = annotation;
                    XmlSchemaAppInfo appInfo = new XmlSchemaAppInfo();
                    annotation.Items.Add(appInfo);
                    appInfo.Markup = TextToNodeArray(enumValue.ImageName);
                }

				restriction.Facets.Add(enumeration);
			}

			// Write IsMultipleSelection member
			if (_isMultipleSelection)
			{
				SetNewteraAttributeValue(NewteraNameSpace.MULTI_SELECTION, "true");
			}

            // Set _displayMode
            SetNewteraAttributeValue(NewteraNameSpace.DISPLAY_MODE, Enum.GetName(typeof(EnumDisplayMode), _displayMode));       

			base.Marshal();
		}
	}

    public enum EnumDisplayMode
    {
        Text,
        Image
    }
}