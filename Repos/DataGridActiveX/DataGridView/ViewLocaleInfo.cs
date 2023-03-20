/*
* @(#)ViewLocaleInfo.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
    using System.Xml;
    using System.Collections;

	/// <summary>
	/// Defintion of locale info.
	/// </summary>
    public class ViewLocaleInfo : DataGridViewElementBase
	{
		private string _none = "None";
		private string _true = "True";
        private string _false = "False";

        public ViewLocaleInfo()
        {
        }

        /// <summary>
		/// Initiating an instance of ViewFilter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewLocaleInfo(XmlElement xmlElement)
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
                return ViewElementType.LocaleInfo;
            }
        }

        /// <summary>
        /// Gets or sets the Localized None word
        /// </summary>
        public string None
        {
            get
            {
                return _none;
            }
            set
            {
                _none = value;
            }
        }

        /// <summary>
        /// Gets or sets the localized true word
        /// </summary>
        public string True
        {
            get
            {
                return _true;
            }
            set
            {
                _true = value;
            }
        }

        /// <summary>
        /// Gets or sets localized false word
        /// </summary>
        public string False
        {
            get
            {
                return _false;
            }
            set
            {
                _false = value;
            }
        }

        /// <summary>
        /// Gets a list of IComboBoxItem for boolean values
        /// </summary>
        /// <returns>A IList of IComboBoxItem object</returns>
        public IList GetBooleanValues()
        {
            ArrayList values = new ArrayList();
            ViewBooleanValue val = new ViewBooleanValue(_none, _none);
            values.Add(val);
            val = new ViewBooleanValue(_true, _true);
            values.Add(val);
            val = new ViewBooleanValue(_false, _false);
            values.Add(val);

            return values;
        }

        /// <summary>
        /// Accept a visitor of IDataGridViewElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IDataGridViewElementVisitor visitor)
        {
            visitor.VisitLocaleInfo(this);
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public override void Unmarshal(XmlElement parent)
        {
            base.Unmarshal(parent);

            // Set _none member
            string str = parent.GetAttribute("noneword");
            _none = str;

            // Set _true member
            str = parent.GetAttribute("trueword");
            this._true = str;

            // Set _false member
            str = parent.GetAttribute("falseword");
            this._false = str;
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
        {
            base.Marshal(parent);

            // Write _none member
            parent.SetAttribute("noneword", _none);

            // Set _true
            parent.SetAttribute("trueword", _true);

            // Set _false
            parent.SetAttribute("falseword", _false);
        }
	}
}