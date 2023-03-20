/*
* @(#)ApiGroup.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represent a group of apis that are associated with a certain class.
	/// </summary>
	/// <version>  1.0.0 16 Oct 2015</version>
	public class ApiGroup : ApiNodeBase
	{
		private string _className;
		
		private ApiCollection _apis;
		
		/// <summary>
		/// Initiate an instance of a ApiGroup class.
		/// </summary>
        public ApiGroup(string className) : base(className)
		{
			_className = className;
			_apis = new ApiCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _apis.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of ApiGroup class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ApiGroup(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}
		
		/// <summary>
		/// Gets name of the class associated with a ApiGroup
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		/// <summary>
		/// Gets the apis contained in a ApiGroup
		/// </summary>
		public ApiCollection Apis
		{
			get
			{
				return _apis;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of ApiNodeType values</value>
        public override ApiNodeType NodeType 
		{
			get
			{
				return ApiNodeType.ApiGroup;
			}
		}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IApiNodeVisitor visitor)
        {
            if (visitor.VisitApiGroup(this))
            {
                this._apis.Accept(visitor);
            }
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
            _apis = (ApiCollection)ApiNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _apis.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write ApiGroup instance to xml document
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
            XmlElement child = parent.OwnerDocument.CreateElement(ApiNodeFactory.ConvertTypeToString(_apis.NodeType));
			_apis.Marshal(child);
			parent.AppendChild(child);
		}
	}
}