/*
* @(#)PivotLayout.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Pivot
{
	using System;
	using System.Xml;

	/// <summary> 
	/// A class represents the basic information of a pivot layout data
	/// </summary>
	/// <version> 1.0.0 16 Oct 2008</version>
	public class PivotLayout
	{
		private string _id;
		private string _schema;
		private string _version;
		private string _user;
        private string _class;
        private string _viewName;
		private string _name;
		private string _desc;
		private DateTime _createdTime;

		/// <summary>
		/// Initiate an instance of PivotLayout class
		/// </summary>
		public PivotLayout()
		{
			_id = null;
			_schema = null;
			_version = null;
			_user = null;
            _class = null;
            _viewName = null;
			_name = null;
			_desc = null;
			_createdTime = DateTime.Today;
		}

		/// <summary>
        /// Initiating an instance of PivotLayout class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal PivotLayout(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets an unique pivot layout id.
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
		/// Gets or sets the schema that the pivot layout is associated with.
		/// </summary>
		public string Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				_schema = value;
			}
		}

		/// <summary>
		/// Gets or sets the schema version that the pivot layout is associated with.
		/// </summary>
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets the user name whose owns the pivot layout.
		/// </summary>
		public string UserName
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

        /// <summary>
        /// Gets or sets the class name whose owns the pivot layout.
        /// </summary>
        public string ClassName
        {
            get
            {
                return _class;
            }
            set
            {
                _class = value;
            }
        }

		/// <summary>
		/// Gets or sets the name of the pivot layout.
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
			}
		}

		/// <summary>
		/// Gets or sets the name of data view
		/// </summary>
		public string ViewName
		{
			get
			{
				return _viewName;
			}
			set
			{
				_viewName = value;
			}
		}

		/// <summary>
		/// Gets or sets the description of the pivot layout, can be null.
		/// </summary>
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
		/// Gets or sets the created time of an attachment.
		/// </summary>
		/// <value>A DateTime object</value>
		public DateTime CreateTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				_createdTime = value;
			}
		}

		/// <summary>
		/// create an DataPoint from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			string str = parent.GetAttribute("id");
			if (str != null && str.Length > 0)
			{
				_id = str;
			}
			else
			{
				_id = null;
			}

			str = parent.GetAttribute("schema");
			if (str != null && str.Length > 0)
			{
				_schema = str;
			}
			else
			{
				_schema = null;
			}

			str = parent.GetAttribute("version");
			if (str != null && str.Length > 0)
			{
				_version = str;
			}
			else
			{
				_version = null;
			}

			str = parent.GetAttribute("user");
			if (str != null && str.Length > 0)
			{
				_user = str;
			}
			else
			{
				_user = null;
			}

            str = parent.GetAttribute("cls");
            if (str != null && str.Length > 0)
            {
                _class = str;
            }
            else
            {
                _class = null;
            }

			str = parent.GetAttribute("name");
			if (str != null && str.Length > 0)
			{
				_name = str;
			}
			else
			{
				_name = null;
			}

			str = parent.GetAttribute("view");
			if (str != null && str.Length > 0)
			{
				_viewName = str;
			}
			else
			{
				_viewName = null;
			}

			str = parent.GetAttribute("desc");
			if (str != null && str.Length > 0)
			{
				_desc = str;
			}
			else
			{
				_desc = null;
			}

			str = parent.GetAttribute("ctime");
			if (str != null && str.Length > 0)
			{
				_createdTime = DateTime.Parse(str);
			}
			else
			{
				_createdTime = DateTime.Today;
			}
		}

		/// <summary>
		/// write DataPoint to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			if (_id != null && _id.Length > 0)
			{
				parent.SetAttribute("id", _id);
			}

			if (_schema != null && _schema.Length > 0)
			{
				parent.SetAttribute("schema", _schema);
			}

			if (_version != null && _version.Length > 0)
			{
				parent.SetAttribute("version", _version);
			}

			if (_user != null && _user.Length > 0)
			{
				parent.SetAttribute("user", _user);
			}

            if (_class != null && _class.Length > 0)
            {
                parent.SetAttribute("cls", _class);
            }

			if (_name != null && _name.Length > 0)
			{
				parent.SetAttribute("name", _name);
			}

			if (_viewName != null && _viewName.Length > 0)
			{
				parent.SetAttribute("view", _viewName);
			}

			if (_desc != null && _desc.Length > 0)
			{
				parent.SetAttribute("desc", _desc);
			}

			parent.SetAttribute("ctime", _createdTime.ToShortDateString());
		}
	}
}