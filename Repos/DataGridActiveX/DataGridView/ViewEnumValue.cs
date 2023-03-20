/*
* @(#)ViewEnumValue.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
    using System.Xml;

	/// <summary>
	/// Defintion of an ViewEnumValue, including value, display text, and image name.
	/// </summary>
    public class ViewEnumValue : DataGridViewElementBase, IComboBoxItem
	{
		private string _value = null;
		private string _displayText = null;
        private string _imageName = null;

        public ViewEnumValue()
        {
        }

        /// <summary>
		/// Initiating an instance of ViewFilter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewEnumValue(XmlElement xmlElement)
            : base()
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
                return ViewElementType.EnumValue;
            }
        }

        /// <summary>
        /// Accept a visitor of IDataGridViewElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IDataGridViewElementVisitor visitor)
        {
            visitor.VisitEnumValue(this);
        }

        #region IComboBoxItem

        /// <summary>
		/// Gets or sets the enum value
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets or sets the display text of the enum value
		/// </summary>
		public string DisplayText
		{
			get
			{
				if (string.IsNullOrEmpty(_displayText))
				{
					return _value;
				}
				else
				{
					return _displayText;
				}
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_displayText = value;
				}
				else
				{
					_displayText = null;
				}
			}
        }

        #endregion

        /// <summary>
        /// Gets or sets the image name of the enum value
        /// </summary>
        public string ImageName
        {
            get
            {
                return _imageName;
            }
            set
            {
                _imageName = value;
            }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            // Set _value member
            string str = parent.GetAttribute("evalue");
            _value = str;

            // Set _displayText member
            str = parent.GetAttribute("etext");
            this._displayText = str;

            // Set _imageName member
            str = parent.GetAttribute("eimage");
            this._imageName = str;
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

            // Write _value member
            parent.SetAttribute("evalue", _value);

            // Set _displayMode
            if (_displayText != null)
            {
                parent.SetAttribute("etext", _displayText);
            }

            // Set _imageName
            if (_imageName != null)
            {
                parent.SetAttribute("eimage", _imageName);
            }
        }

        public override string ToString()
        {
            return DisplayText;
        }
	}
}