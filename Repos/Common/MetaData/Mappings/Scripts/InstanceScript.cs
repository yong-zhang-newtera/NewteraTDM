/*
* @(#)InstanceScript.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;

	/// <summary>
	/// The class represents an insert, update and search query for an instance,
	/// it also stores script execution status.
	/// </summary>
	/// <version>  1.0.0 23 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class InstanceScript : ScriptNodeBase
	{
		/// <summary>
		/// Constant definition for OBJ_ID
		/// </summary>
		public const string OBJ_ID = "@obj_id";

		private string _search;
		private string _insert;
		private string _update;
		private string _msg;
		private bool _isSucceeded;
				
		/// <summary>
		/// Initiate an instance of a InstanceScript class.
		/// </summary>
		public InstanceScript() : base()
		{
			_search = null;
			_insert = null;
			_update = null;
			_msg = null;
			_isSucceeded = true;
		}
		
		/// <summary>
		/// Initiating an instance of InstanceScript class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal InstanceScript(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets or sets the search query that retrieve an instance
		/// </summary>
		public string SearchQuery
		{
			get
			{
				return _search;
			}
			set
			{
				_search = value;
			}
		}

		/// <summary>
		/// Gets or sets the insert query that inserts an instance
		/// </summary>
		public string InsertQuery
		{
			get
			{
				return _insert;
			}
			set
			{
				_insert = value;
			}
		}

		/// <summary>
		/// Gets or sets the update query that updates an instance
		/// </summary>
		public string UpdateQuery
		{
			get
			{
				return _update;
			}
			set
			{
				_update = value;
			}
		}

		/// <summary>
		/// Gets or sets the message of execution
		/// </summary>
		public string Message
		{
			get
			{
				return _msg;
			}
			set
			{
				_msg = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the execution of
		/// the script is succeeded?
		/// </summary>
		/// <value>true if succeeded, false otherwise.</value>
		public bool IsSucceeded
		{
			get
			{
				return _isSucceeded;
			}
			set
			{
				_isSucceeded = value;
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
				return NodeType.InstanceScript;
			}
		}

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Msg");
			if (str != null && str.Length > 0)
			{
				_msg = str;
			}
			else
			{
				_msg = null;
			}

			// Set isSucceeded member
			string status = parent.GetAttribute("Status");
			_isSucceeded = (status != null && status == "true" ? true : false);

			if (parent.ChildNodes[0].InnerText != null &&
				parent.ChildNodes[0].InnerText.Length > 0)
			{
				// first child node is search query
				_search = parent.ChildNodes[0].InnerText;
			}
			else
			{
				_search = null;
			}

			if (parent.ChildNodes[1].InnerText != null &&
				parent.ChildNodes[1].InnerText.Length > 0)
			{
				// second child node is insert query
				_insert = parent.ChildNodes[1].InnerText;
			}
			else
			{
				_insert = null;
			}

			if (parent.ChildNodes[2].InnerText != null &&
				parent.ChildNodes[2].InnerText.Length > 0)
			{
				// third child node is update query
				_update = parent.ChildNodes[2].InnerText;
			}
			else
			{
				_update = null;
			}
		}

		/// <summary>
		/// write InstanceScript instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _msg
			if (_msg != null && _msg.Length > 0)
			{
				parent.SetAttribute("Msg", _msg);
			}

			// Set IsSucceeded member
			if (_isSucceeded)
			{
				parent.SetAttribute("Status", "true");
			}

			// write search query
			XmlElement xmlElement = parent.OwnerDocument.CreateElement("Search");
			xmlElement.InnerText = (_search != null ? _search : "");
			parent.AppendChild(xmlElement);

			// write insert query
			xmlElement = parent.OwnerDocument.CreateElement("Insert");
			xmlElement.InnerText = (_insert != null ? _insert : "");
			parent.AppendChild(xmlElement);

			// write update query
			xmlElement = parent.OwnerDocument.CreateElement("Update");
			xmlElement.InnerText = (_update != null ? _update : "");
			parent.AppendChild(xmlElement);
		}
	}
}