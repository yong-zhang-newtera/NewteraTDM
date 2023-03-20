/*
* @(#)RuleSet.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represent a set of rules that are associated with a particular class.
	/// </summary>
	/// <version>  1.0.0 16 Jun 2004</version>
	public class RuleSet : RuleNodeBase
	{
		private string _className;
		
		private RuleCollection _rules;
		
		/// <summary>
		/// Initiate an instance of a RuleSet class.
		/// </summary>
		public RuleSet(string className) : base()
		{
			_className = className;
			_rules = new RuleCollection();
            _rules.Owner = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _rules.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of RuleSet class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal RuleSet(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets name of the class associated with a RuleSet
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// Gets the rules contained in a RuleSet
		/// </summary>
		public RuleCollection Rules
		{
			get
			{
				return _rules;
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
				return NodeType.RuleSet;
			}
		}

        /// <summary>
        /// Get information indicating whether an attribute is referenced by expressions in any of
        /// the rules.
        /// </summary>
        /// <param name="className">The name of attribute owner class</param>
        /// <param name="attributeName">The name of the attribute to be found</param>
        /// <returns>true if the attribute is referenced, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;

            foreach (RuleDef ruleDef in _rules)
            {
                if (ruleDef.IsAttributeReferenced(className, attributeName))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("class");
			if (str != null && str.Length > 0)
			{
				_className = str;
			}
			else
			{
				_className = null;
			}

			// then a collection of  acl rules
			_rules = (RuleCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            _rules.Owner = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _rules.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write RuleSet instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _className
			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}			

			// write the rules
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_rules.NodeType));
			_rules.Marshal(child);
			parent.AppendChild(child);
		}
	}
}