/*
* @(#)ViewBooleanValue.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
    using System.Xml;

	/// <summary>
	/// Defintion of an ViewBooleanValue, including value, display text.
	/// </summary>
    public class ViewBooleanValue : IComboBoxItem
	{
		private string _value = null;
		private string _displayText = null;

        public ViewBooleanValue(string displayText, string val)
        {
            _displayText = displayText;
            _value = val;
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

        public override string ToString()
        {
            return DisplayText;
        }
	}
}