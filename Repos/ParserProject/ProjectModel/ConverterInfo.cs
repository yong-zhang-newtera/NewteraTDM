/*
* @(#)ConverterInfo.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;

	/// <summary>
	/// Holder of a converter information.
	/// </summary>
	/// <version>1.0.1 05 Dec. 2005</version>
	/// <author>Yong Zhang</author>
	public class ConverterInfo
	{
		private string _name;
		private string _assembly;
		private string _class;
		private string _dir;
		private bool _isAdd;
		private bool _isUpdate;

		/// <summary>
		/// Initiating an instance of ConverterInfo class
		/// </summary>
		/// <param name="name">Parser name</param>
		public ConverterInfo()
		{
			_name = null;
			_assembly = null;
			_class = null;
			_dir = null;
			_isAdd = true;
			_isUpdate = false;
		}

		/// <summary>
		/// Gets or sets the name of the converter
		/// </summary>
		public string ConverterName
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
		/// Gets or sets the name of the converter assembly
		/// </summary>
		public string AssemlyName
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the converter class
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
		/// Gets or sets the directory of the converter assembly
		/// </summary>
		public string AssemblyDir
		{
			get
			{
				return _dir;
			}
			set
			{
				if (value != null)
				{
					_dir = value.Trim();
				}
				else
				{
					_dir = null;
				}
			}
		}

		/// <summary>
		/// Gets a name of the assembly dll
		/// </summary>
		public string AssemblyDLLName
		{
			get
			{
				return _assembly + ".dll";
			}
		}

		/// <summary>
		/// Gets the name of the xml setting file
		/// </summary>
		public string SettingXMLFileName
		{
			get
			{
				return _assembly + ".xml";
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to add this converter to
		/// the existing list.
		/// </summary>
		/// <value>true to add, false otherwise</value>
		public bool IsAdd
		{
			get
			{
				return _isAdd;
			}
			set
			{
				_isAdd = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to update this converter to
		/// the existing list.
		/// </summary>
		/// <value>true to update, false otherwise</value>
		public bool IsUpdate
		{
			get
			{
				return _isUpdate;
			}
			set
			{
				_isUpdate = value;
			}
		}

		/// <summary>
		/// Gets a full path of the assembly dll
		/// </summary>
		public string AssemblyDLLFullPath
		{
			get
			{
				if (!_dir.EndsWith(@"\"))
				{
					return _dir + @"\" + _assembly + ".dll";
				}
				else
				{
					return _dir + _assembly + ".dll";
				}
			}
		}

		/// <summary>
		/// Gets a full path of the setting file
		/// </summary>
		public string SettingXMLFileFullPath
		{
			get
			{
				if (!_dir.EndsWith(@"\"))
				{
					return _dir + @"\" + _assembly + ".xml";
				}
				else
				{
					return _dir + _assembly + ".xml";
				}
			}
		}
	}
}