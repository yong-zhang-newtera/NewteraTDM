/*
* @(#)EnumValue.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;

	/// <summary>
	/// Defintion of an EnumValue, including value and display text.
	/// </summary>
    public class EnumValue : IComboBoxItem
	{
		private string _value = null;
		private string _displayText = null;
        private string _imageName = null;

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
				if (_displayText == null)
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
				if (value != null && value.Length > 0)
				{
					_displayText = value;
				}
				else
				{
					_displayText = null;
				}
			}
		}

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
	}

    /// <summary>
    /// Represents a common interface for the items used in InlineComboBox control.
    /// </summary>
    /// <version>  	1.0.0 25 Jul 2008</version>
    public interface IComboBoxItem
    {
        /// <summary>
        /// Gets or sets value of a combo box item
        /// </summary>
        string Value { get; set;}

        /// <summary>
        /// Gets or sets the display text of a combo box item
        /// </summary>
        string DisplayText { get; set;}
    }
}