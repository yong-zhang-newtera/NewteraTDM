/*
* @(#) EnumPropertyRadioButtonControl.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Xml;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WebForm
{
    /// <summary>
    /// An IPropertyControl control that displays enum values of a property in radio button group
    /// </summary>
    public class EnumPropertyRadioButtonControl : PropertyControlBase
    {
        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        public override bool HasValue(JProperty jProperty)
        {
            return base.HasValue(jProperty);
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool HasValue(XmlElement element)
        {
            return base.HasValue(element);
        }

        /// <summary>
        /// Update property's model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            string val = jProperty.Value.ToString();

            try
            {
                Enum vEnum = GetEnumValue(val, PropertyInfo.PropertyType);
                if (vEnum != null)
                {
                    PropertyInfo.SetValue(null, vEnum);
                }
            }
            catch (Exception)
            {
                // ignore the error due to the enum value definitions may have been changed
            }
        }

        /// <summary>
        /// Update property's model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null)
            {
                string val = element.InnerText;

                try
                {
                    Enum vEnum = GetEnumValue(val, PropertyInfo.PropertyType);
                    if (vEnum != null)
                    {
                        PropertyInfo.SetValue(null, vEnum);
                    }
                }
                catch (Exception)
                {
                    // ignore the error due to the enum value definitions may have been changed
                }
            }
        }

        /// <summary>
        /// Convert the property's model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            string jValue = null;
            object val = PropertyInfo.GetValue();
            if (val != null && val is Enum)
            {
                Enum vEnum = (Enum)val;

                jValue = Convert.ChangeType(vEnum, vEnum.GetTypeCode()).ToString();

                if (jValue == "0")
                    jValue = ""; // first option is unknown, change the value from "0" to ''
            }

            return jValue;
        }

        /// <summary>
        /// Create the property UI
        /// </summary>
        /// <returns></returns>
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            CreateInlineRadioGroup(container, this.InstanceEditor.Document, PropertyInfo);

            return container;
        }  // CreatePropertyNode()

        private Enum GetEnumValue(string enumValue, Type pEnumType)
        {
            if (!string.IsNullOrEmpty(enumValue))
            {
                int vValue = Convert.ToInt32(enumValue);
                Enum vEnum = (Enum)(Enum.ToObject(pEnumType, vValue));

                return vEnum;
            }
            else
            {
                return null;
            }
        }  // GetEnumValue()

        private void CreateInlineRadioGroup(HtmlNode container, HtmlDocument doc, InstanceAttributePropertyDescriptor fPropertyInfo)
        {
            Array vEnumValues = Enum.GetValues(PropertyInfo.PropertyType);

            HtmlNode label, input, span;
            // skip the first item which is "unknown"
            for (int vI = 1; vI < vEnumValues.Length; vI++)
            {
                int vValue = Convert.ToInt32(vEnumValues.GetValue(vI));
                string vName = HtmlDocument.HtmlEncode(Enum.GetName(PropertyInfo.PropertyType, vValue));

                label = this.InstanceEditor.Document.CreateElement("label");
   
                container.AppendChild(label);

                input = this.InstanceEditor.Document.CreateElement("input");
                input.SetAttributeValue("type", "radio");
                input.SetAttributeValue("class", "radiobox style-0");
                input.SetAttributeValue("name", fPropertyInfo.Name);
                input.SetAttributeValue("ng-model", this.InstanceEditor.ViewModelPath + fPropertyInfo.Name);
                input.SetAttributeValue("value", vValue.ToString());
                if (IsReadOnly)
                {
                    input.SetAttributeValue("disabled", "disabled");
                }

                label.AppendChild(input);

                span = this.InstanceEditor.Document.CreateElement("span");
                span.InnerHtml= vName;
                label.AppendChild(span);
            }  // for
        }  // CreateRadioButtonList()
    }
}