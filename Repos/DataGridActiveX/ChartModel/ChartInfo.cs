/*
* @(#)ChartInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;

	/// <summary> 
	/// A class represents the basic information of a chart
	/// </summary>
	/// <version> 1.0.0 30 Apr 2006</version>
	public class ChartInfo : ChartNodeBase
	{
		private string _id;
		private string _schema;
		private string _version;
		private string _user;
        private string _class;
		private string _name;
		private string _desc;
		private string _type;
		private DateTime _createdTime;

		/// <summary>
		/// Initiate an instance of ChartInfo class
		/// </summary>
		public ChartInfo()
		{
			_id = null;
			_schema = null;
			_version = null;
			_user = null;
            _class = null;
			_name = null;
			_desc = null;
			_type = null;
			_createdTime = DateTime.Today;
		}

		/// <summary>
		/// Initiating an instance of DataPoint class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ChartInfo(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets an unique chart id.
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
		/// Gets or sets the schema that the chart is associated with.
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
		/// Gets or sets the schema version that the chart is associated with.
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
		/// Gets or sets the user name whose owns the chart.
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
        /// Gets or sets the class name whose owns the chart.
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
		/// Gets or sets the name of the chart.
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
		/// Gets or sets the type of the chart, such as line, contiur, and etc.
		/// </summary>
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the description of the chart, can be null.
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
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ChartInfo;
			}
		}

		/// <summary>
		/// create an DataPoint from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

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

			str = parent.GetAttribute("type");
			if (str != null && str.Length > 0)
			{
				_type = str;
			}
			else
			{
				_type = null;
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
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

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

			if (_type != null && _type.Length > 0)
			{
				parent.SetAttribute("type", _type);
			}

			if (_desc != null && _desc.Length > 0)
			{
				parent.SetAttribute("desc", _desc);
			}

			parent.SetAttribute("ctime", _createdTime.ToShortDateString());
		}
	}
}