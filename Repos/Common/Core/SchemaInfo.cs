/*
* @(#) SchemaInfo.cs	1.0.0		2003-01-05
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;

	/// <summary>
	/// The parameters of a schema .
	/// </summary>
	/// <version> 	1.0.0	15 Jul 2003 </version>
	[Serializable]
	public class SchemaInfo
	{
		private string _id;
		private string _name;
		private string _version;
		private string _desc;
        private DateTime _modifiedTime;

		/// <summary>
		/// Gets or sets id of the schema
		/// </summary>
		/// <value> The id of the schema.</value>
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
		/// Gets or sets name of the schema
		/// </summary>
		/// <value> The name of the schema.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets version of the schema
		/// </summary>
		/// <value> The version of the schema.</value>
		public string Version
		{
			get
			{
                if (!string.IsNullOrEmpty(_version))
                {
                    return _version;
                }
                else
                {
                    return "1.0"; // default
                }
			}
			set
			{
				_version = value;
			}
		}

        /// <summary>
        /// Gets or sets modified time of the schema
        /// </summary>
        /// <value> The modified time.</value>
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
		/// Gets Name and Version of the schema
		/// </summary>
		/// <value> The name and version of the schema.</value>
		public string NameAndVersion
		{
			get
			{
				return Name.ToUpper() + " " + Version.ToUpper();
			}
		}

		/// <summary>
		/// Gets or sets description of the schema
		/// </summary>
		/// <value> The id of the schema.</value>
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}

        /// <summary>
        /// Clone a SchemaInfo instance
        /// </summary>
        /// <returns>The cloned SchemaInfo instance.</returns>
        public SchemaInfo Clone()
        {
            SchemaInfo schemaInfo = new SchemaInfo();

            schemaInfo.ID = this.ID;
            schemaInfo.Name = this.Name;
            schemaInfo.Version = this.Version;
            schemaInfo.ModifiedTime = this.ModifiedTime;
            schemaInfo.Description = this.Description;

            return schemaInfo;
        }
	}
}