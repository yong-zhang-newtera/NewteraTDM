/*
* @(#)LoggingSubject.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Collections;

	/// <summary>
	/// The class represents the a subject in a LoggingRule
	/// </summary>
	/// <version>  	1.0.0 04 Jan 2009 </version>
	public class LoggingSubject : LoggingNodeBase
	{	
		// the special uid for processor
		/// <summary>
		/// Constant definition for EveryOne role
		/// </summary>
		public static string EveryOne = "Everyone";

		/// <summary>
		/// Constant definition for Anonymous user
		/// </summary>
		public static string AnonymousUser = "Anonymous";

		private string _uid = null; // the user id 
		
		private string[] _roles; // an user roles
		
		private string[] _groups; // an user groups

		/// <summary>
		/// Initiating an instance of LoggingSubject class
		/// </summary>
		public LoggingSubject() : base()
		{
			_uid = null;
			_roles = new string[0];
			_groups = new string[0];
		}

		/// <summary>
		/// Initiating an instance of LoggingSubject class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingSubject(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the user id of the subject.
		/// </summary>
		/// <value> the user id of the subject.</value>
		public string Uid
		{
			get
			{
				return _uid;
			}
			set
			{
				_uid = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the user roles of the subject.
		/// </summary>
		/// <value> an array of user roles</value>
		public string[] Roles
		{
			get
			{
				return _roles;
			}
			set
			{
				_roles = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the user groups of the subject.
		/// </summary>
		/// <value> an array of user groups.</value>
		public string[] Groups
		{
			get
			{
				return _groups;
			}
			set
			{
				_groups = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Add a role to the subject
		/// </summary>
		/// <param name="role">A role to add</param>
		public void AddRole(string role)
		{
			int size = (_roles == null ? 1 : _roles.Length + 1);
			string[] roles = new string[size];
			if (_roles != null)
			{
				for (int i = 0; i < _roles.Length; i++)
				{
					roles[i] = _roles[i];
				}
			}
			roles[size - 1] = role; // append the role
			_roles = roles;

			FireValueChangedEvent(role);
		}

		/// <summary>
		/// Add a group to the subject
		/// </summary>
		/// <param name="group">A group to add</param>
		public void AddGroup(string group)
		{
			int size = (_groups == null ? 1 : _groups.Length + 1);
			string[] groups = new string[size];
			if (_groups != null)
			{
				for (int i = 0; i < _groups.Length; i++)
				{
					groups[i] = _groups[i];
				}
			}
			groups[size - 1] = group; // append the group
			_groups = groups;

			FireValueChangedEvent(group);
		}

		/// <summary>
		/// Gets the information indicating whether this subject equals to another
		/// subject
		/// </summary>
		/// <param name="obj">Another subject</param>
		/// <returns>true if it equals to another subject, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			bool isEqual = true;
			LoggingSubject another = (LoggingSubject) obj;

			if (!IsStringEqual(this._uid, another._uid))
			{
				isEqual = false;
			}
			else if (!IsArrayEqual(this._roles, another._roles))
			{
				isEqual = false;
			}
			else if (!IsArrayEqual(this._groups, another._groups))
			{
				isEqual = false;
			}

			return isEqual;
		}

		/// <summary>
		/// Override GetHashCode from the base class
		/// </summary>
		/// <returns>A hashcode</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Subject;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            visitor.VisitLoggingSubject(this);
        }

		/// <summary>
		/// create an LoggingSubject from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_uid = parent.GetAttribute("uid");
			if (_uid != null && _uid.Length == 0)
			{
				_uid = null;
			}
			string attributeValue = parent.GetAttribute("roles");
			_roles = ConvertToStringArray(attributeValue);
			attributeValue = parent.GetAttribute("groups");
			_groups = ConvertToStringArray(attributeValue);
		}

		/// <summary>
		/// write policy to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _uid member
			if (_uid != null && _uid.Length > 0)
			{
				parent.SetAttribute("uid", _uid);
			}

			if (_roles != null && _roles.Length > 0)
			{
				parent.SetAttribute("roles", ConvertToString(_roles));
			}

			if (_groups != null && _groups.Length > 0)
			{
				parent.SetAttribute("groups", ConvertToString(_groups));
			}
		}

		/// <summary>
		/// Convert a string of values separated by comma to a string array
		/// </summary>
		/// <param name="str">The string of values</param>
		/// <returns>a string array</returns>
		private string[] ConvertToStringArray(string str)
		{
			string[] values = new string[0];

			if (str != null && str.Length > 0)
			{
				// TODO: Compile a regular expression to find objId
				Regex exp = new Regex(",");

				values = exp.Split(str);
			}

			return values;
		}

		/// <summary>
		/// Convert an array of strings to a single string separated by comma
		/// </summary>
		/// <param name="values">An array of strings</param>
		/// <returns>a string</returns>
		private string ConvertToString(string[] values)
		{
			StringBuilder builder = new StringBuilder();

			int index = 0;
			foreach (string val in values)
			{
				builder.Append(val);
				if (index < values.Length - 1)
				{
					builder.Append(",");
				}

				index++;
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets the information indicating whether two string are equal.
		/// </summary>
		/// <param name="str1">One string</param>
		/// <param name="str2">Another string</param>
		/// <returns>true if they are equal, false otherwise</returns>
		private bool IsStringEqual(string str1, string str2)
		{
			bool isEqual = true;

			if ((str1 == null && str2 != null) ||
				(str1 != null && str2 == null) ||
				(str1 != null && str2 != null && str1 != str2))
			{
				isEqual = false;
			}

			return isEqual;
		}

		/// <summary>
		/// Gets the information indicating whether two string arrays are equal.
		/// </summary>
		/// <param name="strs1">One string array </param>
		/// <param name="strs2">Another string array</param>
		/// <returns>true if they are equal, false otherwise</returns>
		private bool IsArrayEqual(string[] strs1, string[] strs2)
		{
			bool isEqual = true;

			if ((strs1 == null && strs2 != null) ||
				(strs1 != null && strs2 == null))
			{
				isEqual = false;
			}
			else if (strs1 != null && strs2 != null)
			{
				if (strs1.Length != strs2.Length)
				{
					isEqual = false;
				}
				else
				{
					for (int i = 0; i < strs1.Length; i++)
					{
						if (strs1[i] != strs2[i])
						{
							isEqual = false;
							break;
						}
					}
				}
			}

			return isEqual;
		}
	}
}