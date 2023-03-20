/*
* @(#)ScriptBase.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents a base class for all scripts written in C#, VB script, or
	/// Java#
	/// </summary>
	/// <version>1.0.0 15 Jan 2014</version>
    public abstract class ScriptBase : MappingNodeBase
	{
		private bool _enabled;
		private ScriptLanguage _language;
		private string _classType;
		private string _script;
		
		/// <summary>
		/// Initiate an instance of ScriptBase class.
		/// </summary>
		public ScriptBase() : base()
		{
			_language = ScriptLanguage.CSharp;
			_script = null;
			_classType = null;
			_enabled = false;
		}

		/// <summary>
		/// Initiating an instance of ScriptBase class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ScriptBase(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the information indicating whether the script is enabled
		/// for transformation.
		/// </summary>
		/// <value>true if it is enabled, false otherwise.</value>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		/// <summary>
		/// Gets or sets type of the script language
		/// </summary>
		/// <value>One of ScriptLanguage enum values.</value>
		public virtual ScriptLanguage ScriptLanguage
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets class type of the script
		/// </summary>
		/// <value>Class type string</value>
		public virtual string ClassType
		{
			get
			{
				return _classType;
			}
			set
			{
				_classType = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the script string
		/// </summary>
		public virtual string Script
		{
			get
			{
				return _script;
			}
			set
			{
				_script = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// create an ScriptBase from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string enabled = parent.GetAttribute("Enabled");
			if (enabled != null && enabled == "true")
			{
				_enabled = true;
			}
			else
			{
				_enabled = false;
			}

			string classType = parent.GetAttribute("Class");
			if (classType != null && classType.Length > 0)
			{
				this._classType = classType;
			}
			else
			{
				this._classType = null;
			}

			string languageStr = parent.GetAttribute("Language");

			if (languageStr != null && languageStr.Length > 0)
			{
				_language = (ScriptLanguage) Enum.Parse(typeof(ScriptLanguage), languageStr);
			}
			else
			{
				_language = ScriptLanguage.CSharp;
			}

			string str = parent.InnerText;

			if (str != null && str.Length > 0)
			{
				// hack, replace \n with \r\n. For some reason \r\n in the script
				// was changed to \n when saved to database
				if (str.IndexOf("\r\n") < 0)
				{
					// do it once
					_script = str.Replace("\n", "\r\n");
				}
				else
				{
					_script = str;
				}
			}
			else
			{
				_script = null;
			}
		}

		/// <summary>
		/// write ScriptBase to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_enabled)
			{
				parent.SetAttribute("Enabled", "true");
			}

			if (this._classType != null && this._classType.Length > 0)
			{
				parent.SetAttribute("Class", _classType);
			}

			parent.SetAttribute("Language", Enum.GetName(typeof(ScriptLanguage), _language));

			// write the _script
			if (_script != null && _script.Length > 0)
			{
				parent.InnerText = _script;
			}
		}
	}
}