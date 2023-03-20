/*
* @(#)ClassScript.cs
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
	/// The class represents script definitions for a specific class.
	/// A ClassScript object contains a collection of InstanceScript instances.
	/// </summary>
	/// <version>  1.0.0 23 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class ClassScript : ScriptNodeBase
	{	
		private string _className;
		private InstanceScriptCollection _instanceScripts;
		
		/// <summary>
		/// Initiate an instance of a ClassScript class.
		/// </summary>
		public ClassScript(string className) : base()
		{
			_className = className;

			_instanceScripts = new InstanceScriptCollection();
		}

		/// <summary>
		/// Gets or sets the information indicating whether the execution of
		/// importing instances in this class is succeeded?
		/// </summary>
		/// <value>true if succeeded, false otherwise.</value>
		public bool IsSucceeded
		{
			get
			{
				return IsAllInstancesSucceeded();
			}
		}
		
		/// <summary>
		/// Initiating an instance of ClassScript class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ClassScript(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets name of the class
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// Gets or sets the collection of the attribute mappings contained in a ClassScript
		/// </summary>
		public InstanceScriptCollection InstanceScripts
		{
			get
			{
				return _instanceScripts;
			}
			set
			{
				_instanceScripts = value;
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
				return NodeType.ClassScript;
			}
		}

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Name");
			if (str != null && str.Length > 0)
			{
				_className = str;
			}
			else
			{
				_className = null;
			}

			// then a collection of  acl rules
			_instanceScripts = (InstanceScriptCollection) ScriptNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
		}

		/// <summary>
		/// write ClassScript instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _className
			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("Name", _className);
			}

			// write the rules
			XmlElement child = parent.OwnerDocument.CreateElement(ScriptNodeFactory.ConvertTypeToString(_instanceScripts.NodeType));
			_instanceScripts.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the information indicating whether all the instances have been
		/// imported successfully.
		/// </summary>
		/// <returns>true if they all succeeded, false, if any of them didn't</returns>
		private bool IsAllInstancesSucceeded()
		{
			bool status = true;

			foreach (InstanceScript instanceScript in _instanceScripts)
			{
				if (!instanceScript.IsSucceeded)
				{
					status = false;
					break;
				}
			}

			return status;
		}
	}
}