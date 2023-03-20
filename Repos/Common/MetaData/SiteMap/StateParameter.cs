/*
* @(#)Menu.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.IO;
	using System.Xml;
    using System.ComponentModel;
    using System.Drawing.Design;

	/// <summary> 
	/// Represent a state parameter in SiteMapNode
	/// </summary>
	/// <version> 1.0.0 14 Mar 2016</version>
	public class StateParameter : SiteMapNodeBase
	{
        private string _parameterValue;

		/// <summary>
		/// Initiate an instance of Menu class
		/// </summary>
		public StateParameter()
		{
			Init();
		}

		/// <summary>
		/// Initiating an instance of StateParameter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal StateParameter(XmlElement xmlElement) : base()
		{
			Init();
			Unmarshal(xmlElement);
		}


        /// <summary>
        /// Initialize the instance
        /// </summary>
        private void Init()
        {
            _parameterValue = null;
        }

        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [
          CategoryAttribute("Appearance"),
          DescriptionAttribute("The name of the parameter"),
          ReadOnlyAttribute(false)
        ]
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the node title
        /// </summary>
        [BrowsableAttribute(false)]
        public override string Title
        {
            get
            {
                return base.Title;
            }
            set
            {
                base.Title = value;
            }
        }

        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The value of the parameter")
        ]
        public string Value
        {
            get
            {
                return _parameterValue;
            }
            set
            {
                _parameterValue = value;
            }
        }

        /// <summary>
        /// Gets the type of Node
        /// </summary>
        /// <value>One of NodeType values</value>
        public override NodeType NodeType	
		{
			get
			{
                return NodeType.Parameter;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISiteMapNodeVisitor visitor)
        {
        }
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
            base.Unmarshal(parent);

            if (parent.GetAttribute("Value") != null)
            {
                _parameterValue = parent.GetAttribute("Value");
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
        public override void Marshal(XmlElement parent)
		{
            base.Marshal(parent);

            if (!string.IsNullOrEmpty(_parameterValue))
            {
                parent.SetAttribute("Value", _parameterValue);
            }
		}

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            return "";
        }

        /// <summary>
        /// Return a displayed title path representation of the ISiteMapNode
        /// </summary>
        /// <returns>a title path representation</returns>
        public override string ToDisplayPath()
        {
            return "";
        }
	}
}