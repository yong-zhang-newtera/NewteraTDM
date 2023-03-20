/*
* @(#)LocaleInfo.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
	using System.Resources;


	/// <summary>
	/// A singleton class that provide localized version of key word and date/time
	/// formats
	/// </summary>
	/// <version>  1.0.0 14 Nov 2003 </version>
	/// <author> Yong Zhang </author>
	public class LocaleInfo
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static LocaleInfo theFactory;
		
		private ResourceManager _resources;

		static LocaleInfo()
		{
			theFactory = new LocaleInfo();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private LocaleInfo()
		{
			_resources = new ResourceManager(typeof(LocaleInfo));
		}

		/// <summary>
		/// Gets the LocaleInfo instance.
		/// </summary>
		/// <returns> The LocaleInfo instance.</returns>
		static public LocaleInfo Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Gets the Localized None word
		/// </summary>
		public string None
		{
			get
			{				
				return _resources.GetString("None");
			}
		}

		/// <summary>
		/// Gets the localized true word
		/// </summary>
		public string True
		{
			get
			{				
				return _resources.GetString("True");
			}
		}

		/// <summary>
		/// Gets localized false word
		/// </summary>
		public string False
		{
			get
			{				
				return _resources.GetString("False");
			}
		}

		/// <summary>
		/// Gets localized Date Format.
		/// </summary>
		public string DateFormat
		{
			get
			{				
				return _resources.GetString("DateFormat");
			}
		}

        /// <summary>
        /// Gets localized C# Date Format.
        /// </summary>
        public string CSharpDateFormat
        {
            get
            {
                return _resources.GetString("CSharpDateFormat");
            }
        }

		/// <summary>
		/// Gets localized Time format.
		/// </summary>
		public string TimeFormat
		{
			get
			{				
				return _resources.GetString("TimeFormat");
			}
		}

		/// <summary>
		/// Gets localized Date and Time format
		/// </summary>
		public string DateTimeFormat
		{
			get
			{				
				return _resources.GetString("DateTimeFormat");
			}
		}

        /// <summary>
        /// Gets localized C# Date and Time format
        /// </summary>
        public string CSharpDateTimeFormat
        {
            get
            {
                return _resources.GetString("CSharpDateTimeFormat");
            }
        }

        /// <summary>
        /// Gets localized person name display format
        /// </summary>
        public string PersonNameFormat
        {
            get
            {
                return _resources.GetString("NameFormat");
            }
        }

        /// <summary>
        /// Gets the information indicating whether a given string represents a boolean true.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>true if it represents a boolean true value, false otherwise</returns>
        public bool IsTrue(string val)
        {
            if (val == True || val == "1" || val == "true" || val == "True" || val == "ÊÇ" || val == "Yes" || val == "YES")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a given string represents a boolean false.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>true if it represents a boolean false value, false otherwise</returns>
        public bool IsFalse(string val)
        {
            if (val == False || val == "0" || val == "false" || val == "False" || val == "·ñ" || val == "No" || val == "NO")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}
}