/*
* @(#)ParameterBindingInfo.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;

	/// <summary>
	/// A class represents a data binding to a parameter.
	/// </summary>
	/// <version>1.0.0 16 Nov 2007 </version>
    [Serializable]
    public class ParameterBindingInfo : CustomTypeDescriptorBase
	{
        public const char SEPARATOR = '/';

        private string _path;
        private SourceType _sourceType;

        public static string GetActivityName(string path)
        {
            string name = null;

            if (path != null)
            {
                string[] substrings = path.Split(ParameterBindingInfo.SEPARATOR);
                if (substrings.Length > 0)
                {
                    name = substrings[0];
                }
            }

            return name;
        }

        public static string GetPropertyName(string path)
        {
            string name = null;

            if (path != null)
            {
                string[] substrings = path.Split(ParameterBindingInfo.SEPARATOR);
                if (substrings.Length > 1)
                {
                    name = substrings[1];
                }
            }

            return name;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterBindingInfo()
        {
            _sourceType = SourceType.Unknown;
            _path = null;
        }

        /// <summary>
        /// Gets or sets the path of value in the binding source object
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The path of value in the binding source object."),
            ReadOnlyAttribute(true)
        ]
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the binding source object
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The type of the binding source object"),
            ReadOnlyAttribute(true)
        ]
        public SourceType SourceType
        {
            get
            {
                return _sourceType;
            }
            set
            {
                _sourceType = value;
            }
        }

        /// <summary>
        /// Clone the ParameterBindingInfo object
        /// </summary>
        /// <returns>The cloned ParameterBindingInfo object</returns>
        public ParameterBindingInfo Clone()
        {
            ParameterBindingInfo clone = new ParameterBindingInfo();
            clone.Path = Path;
            clone.SourceType = SourceType;

            return clone;
        }
	}

    /// <summary>
    /// Definition of source type enum
    /// </summary>
    public enum SourceType
    {
        Unknown,
        DataInstance,
        Activity,
        Parameter
    }
}