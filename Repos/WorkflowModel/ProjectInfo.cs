/*
* @(#)ProjectInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// A class contains project information
	/// </summary>
	/// <version>  	1.0.0 8 Dec 2006 </version>
	public class ProjectInfo
	{
        private const string DefaultVersion = "1.0";
        private string _id;
        private string _name;
        private string _version;
        private string _description;
        private DateTime _modifiedTime;
        private string _nameAndVersion;

        public ProjectInfo()
        {
            _id = null;
            _name = null;
            _version = null;
            _description = null;
            _nameAndVersion = null;
        }

        /// <summary>
        /// Gets or sets the id of project
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of project
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                _nameAndVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the version of project
        /// </summary>
        public string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    return DefaultVersion;
                }
                else
                {
                    return _version;
                }
            }
            set
            {
                _version = value;
                _nameAndVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the description of project
        /// </summary>
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
        /// Gets or sets the modified time of project
        /// </summary>
        public DateTime ModifiedTime
        {
            get
            {
                return _modifiedTime;
            }
            set
            {
                _modifiedTime = value;
            }
        }

        /// <summary>
        /// Gets unique id of a project
        /// </summary>
        public string NameAndVersion
        {
            get
            {
                if (_nameAndVersion == null)
                {
                    _nameAndVersion = Name + Version;
                }

                return _nameAndVersion;
            }
        }
	}
}