/*
* @(#)ViewEnumElement.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;
	using System.ComponentModel;
	using System.Drawing.Design;
	using System.Collections;
    using System.Collections.Specialized;
	
	/// <summary>
	/// The ViewEnumElement represents an enumeration constraint. 
	/// </summary>
	/// <version>  	1.0.1 31 May 2007
	/// </version>
    public class ViewEnumElement : DataGridViewElementBase
	{
		/// <summary>
		/// Constant definition for enum value SEPARATOR
		/// </summary>
		public const string SEPARATOR = ";";

		// The list of enumeration's values
		private ViewEnumValueCollection _values;
        private ViewEnumDisplayMode _displayMode = ViewEnumDisplayMode.Text;
        private bool _isMultipleChoice = false;
		
		/// <summary>
		/// Initializing an ViewEnumElement object
		/// </summary>
		/// <param name="name">Name of element</param>
		public ViewEnumElement(string name): base(name)
		{
			_values = new ViewEnumValueCollection();
		}

		/// <summary>
		/// Initializing an ViewEnumElement object
		/// </summary>
        /// <param name="xmlElement">The XmlElement object</param>
        internal ViewEnumElement(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ViewElementType values</value>
        public override ViewElementType ElementType
        {
            get
            {
                return ViewElementType.EnumElement;
            }
        }

		/// <summary>
		/// Gets or sets the enumeration values.
		/// </summary>
		/// <value>
		/// An collection of strings of enumeration values
		/// </value>
		public ViewEnumValueCollection Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
			}
		}

        /// <summary>
        /// Gets or sets the display method of enum values
        /// </summary>
        /// <value>
        /// One of the ViewEnumDisplayMode enum values
        /// </value>
        public ViewEnumDisplayMode DisplayMode
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
        /// Gets or sets the information indicating whether it is a multiple choice
        /// </summary>
        public bool IsMultipleChoice
        {
            get
            {
                return _isMultipleChoice;
            }
            set
            {
                _isMultipleChoice = value;
            }
        }
		
        /// <summary>
        /// Accept a visitor of IDataGridViewElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IDataGridViewElementVisitor visitor)
        {
            visitor.VisitEnumElement(this);
        }

        /// <summary>
        /// Gets the display texts of enum values
        /// </summary>
        public StringCollection DisplayTexts
        {
            get
            {
                StringCollection displayTexts = new StringCollection();
                foreach (ViewEnumValue enumValue in this.Values)
                {
                    displayTexts.Add(enumValue.DisplayText);
                }

                return displayTexts;
            }
        }

		/// <summary>
		/// Gets the corresponsing display text of an enum value
		/// </summary>
		/// <param name="enumValue">enum value</param>
		/// <returns>The display text</returns>
		public string GetDisplayText(string val)
		{
			string text = "";
			if (Values != null)
			{
				foreach (ViewEnumValue enumValue in Values)
				{
					if (enumValue.Value == val)
					{
						text = enumValue.DisplayText;
						break;
					}
				}
			}

			return text;
		}

        /// <summary>
        /// Gets the corresponsing image name of an enum value
        /// </summary>
        /// <param name="enumValue">enum value</param>
        /// <returns>The display text, could be null</returns>
        public string GetImageName(string val)
        {
            string imageName = null;
            if (Values != null)
            {
                foreach (ViewEnumValue enumValue in Values)
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
		/// Gets the corresponsing enum value for an display text
		/// </summary>
		/// <param name="text">display text</param>
		/// <returns>The enum value</returns>
		public string GetValue(string text)
		{
			string val = "";
			if (Values != null)
			{
				foreach (ViewEnumValue enumValue in Values)
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
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            // Set isMultipleSelection member
            string str = parent.GetAttribute("multichoice");
            if (str != null && str == "true")
            {
                this._isMultipleChoice = true;
            }
            else
            {
                this._isMultipleChoice = false;
            }

            //Set display mode
            str = parent.GetAttribute("displaymode");
            if (!string.IsNullOrEmpty(str))
            {
                _displayMode = (ViewEnumDisplayMode) Enum.Parse(typeof(ViewEnumDisplayMode), str);
            }
            else
            {
                _displayMode = ViewEnumDisplayMode.Text;
            }

            // then comes a collection of enum values
            _values = (ViewEnumValueCollection)ViewElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
		}

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

			// Write IsMultipleChoice member
            if (this._isMultipleChoice)
            {
                parent.SetAttribute("multichoice", "true");
            }

            // Set _displayMode
            parent.SetAttribute("displaymode", Enum.GetName(typeof(ViewEnumDisplayMode), _displayMode));

            // write the _values
            XmlElement child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_values.ElementType));
            _values.Marshal(child);
            parent.AppendChild(child);
		}
	}

    public enum ViewEnumDisplayMode
    {
        Text,
        Image
    }
}