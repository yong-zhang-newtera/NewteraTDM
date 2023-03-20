/*
* @(#)Api.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;
    using System.Text;
    using System.Collections.Specialized;
	using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;

	/// <summary>
	/// The class represents definition of a api for a class.
	/// </summary>
	/// <version>1.0.0 16 Oct 2015</version>
	public partial class Api : ApiNodeBase
	{
		private string _className;
        private MethodType _methodType;
        private HttpMethod _httpMethod;
        private bool _isAuthorized;
        private string _description;
        private string _inlineHandler;
        private string _externalHandler;

        private MetaDataModel _metaData = null; // run-time value
		
		/// <summary>
		/// Initiate an instance of Api class.
		/// </summary>
		public Api() : base()
		{
			_className = null;
            _description = null;
            _inlineHandler = null;
            _externalHandler = null;
            _methodType = MethodType.GetMany;
            _httpMethod = HttpMethod.GET;
            _isAuthorized = false;

            // set default name
            this.Name = "API1";
		}

		/// <summary>
		/// Initiating an instance of Api class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Api(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        [CategoryAttribute("API")]
        [DescriptionAttribute("The MethodType property specifies what type of operation that API performs.")]
        public MethodType MethodType
        {
            get
            {
                return _methodType;
            }
            set
            {

                _methodType = value;
            }
        }

        [CategoryAttribute("API")]
        [ReadOnly(true)]
        [DescriptionAttribute("The HttpMethod property is used to specify the HTTP method of the API.")]
        public HttpMethod HttpMethod
        {
            get
            {
                _httpMethod = HttpMethod.GET;

                switch (_methodType)
                {
                    case MethodType.Create:
                    case MethodType.Update:

                        _httpMethod = HttpMethod.POST;
                        break;
                }

                return _httpMethod;
            }
            set
            {

                _httpMethod = value;
            }
        }

        [CategoryAttribute("API")]
        [DescriptionAttribute("The IsAuthorized property is used to specify whether it is an authorized api.")]
        public bool IsAuthorized
        {
            get
            {
                return _isAuthorized;
            }
            set
            {
                _isAuthorized = value;
            }
        }

        [CategoryAttribute("API")]
        [DescriptionAttribute("The URI property displays the uri of the api.")]
        [ReadOnly(true)]
        public string URI
        {
            get
            {
                string uri = "";
                string schemaName = "schema";
                if (_metaData != null)
                {
                    schemaName = _metaData.SchemaInfo.Name;
                }

                switch (_methodType)
                {
                    case MethodType.GetMany:
                        uri = "api/data/" + schemaName + "/" + _className + "/custom/" + this.Name;

                        break;

                    case MethodType.GetOne:
                        uri = "api/data/" + schemaName + "/" + _className + "/oid/custom/" + this.Name;

                        break;

                    case MethodType.Create:

                        uri = "api/data/" + schemaName + "/" + _className + "/custom/" + this.Name;

                        break;

                    case MethodType.Update:

                        uri = "api/data/" + schemaName + "/" + _className + "/oid/custom/" + this.Name;

                        break;
                }

                return uri;
            }
        }

        [CategoryAttribute("API")]
        [DescriptionAttribute("The description property is used to specify the description of the API.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Description
        {

            get
            {
                return _description;
            }
            set
            {

                _description = value;
            }
        }

        /// <summary>
        /// Gets or sets the inline code of event handler.
        /// </summary>
        /// <value>The inline code in C#</value>
        /*
        [
            CategoryAttribute("Implementation"),
            DescriptionAttribute("The inline handler that is called when the event arises"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        */
        [BrowsableAttribute(false)]
        public string InlineHandler
        {
            get
            {
                return _inlineHandler;
            }
            set
            {
                if (value != null)
                {
                    _inlineHandler = value;
                }
                else
                {
                    _inlineHandler = "";
                }
            }
        }

        /// <summary>
        /// Gets or sets the external event handler in form of "NameSpace.ClassName,LibName"
        /// </summary>
        [
            CategoryAttribute("Implementation"),
            DescriptionAttribute("The external event handler for the event"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string ExternalHanlder
        {
            get
            {
                return _externalHandler;
            }
            set
            {
                _externalHandler = value;
            }
        }

		/// <summary>
		/// Gets or sets name of the class that this rule is associated with.
		/// </summary>
		/// <value> The unique class name.</value>
        [BrowsableAttribute(false)]
        public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

        /// <summary>
        /// Gets or sets MetaDataModel instance.
        /// </summary>
        [BrowsableAttribute(false)]
        public MetaDataModel MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                _metaData = value;
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of ApiNodeType values</value>
        [BrowsableAttribute(false)]		
        public override ApiNodeType NodeType
		{
			get
			{
				return ApiNodeType.Api;
			}
		}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IApiNodeVisitor visitor)
        {
            visitor.VisitApi(this);
        }

		/// <summary>
		/// create an Api from a xml document.
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

            str = parent.GetAttribute("desc");
            if (!string.IsNullOrEmpty(str))
            {
                _description = str;
            }
            else
            {
                _description = null;
            }

            str = parent.GetAttribute("optype");
            if (!string.IsNullOrEmpty(str))
            {
                _methodType = (MethodType)Enum.Parse(typeof(MethodType), str);
            }
            else
            {
                _methodType = MethodType.GetMany;
            }

            str = parent.GetAttribute("method");
            if (!string.IsNullOrEmpty(str))
            {
                _httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), str);
            }
            else
            {
                _httpMethod = HttpMethod.GET;
            }

            str = parent.GetAttribute("authorized");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isAuthorized = true;
            }
            else
            {
                _isAuthorized = false;
            }

            str = parent.InnerText;

            if (str != null && str.Length > 0)
            {
                // hack, replace \n with \r\n. For some reason \r\n in the script
                // was changed to \n when saved to database
                if (str.IndexOf("\r\n") < 0)
                {
                    // do it once
                    _inlineHandler = str.Replace("\n", "\r\n");
                }
                else
                {
                    _inlineHandler = str;
                }
            }
            else
            {
                _inlineHandler = null;
            }

            str = parent.GetAttribute("externalHandler");
            if (!string.IsNullOrEmpty(str))
            {
                _externalHandler = str;
            }
            else
            {
                _externalHandler = null;
            }
		}

		/// <summary>
		/// write Api to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}

            if (!string.IsNullOrEmpty(_description))
            {
                parent.SetAttribute("desc", _description);
            }

            if (_methodType != MethodType.GetMany)
            {
                // default is GetMany
                parent.SetAttribute("optype", Enum.GetName(typeof(MethodType), _methodType));
            }
            
            if (_httpMethod != HttpMethod.GET)
            {
                // default to GET method
                parent.SetAttribute("method", Enum.GetName(typeof(HttpMethod), _httpMethod));
            }

            if (_isAuthorized)
            {
                // default to false
                parent.SetAttribute("authorized", "true");
            }

            if (!string.IsNullOrEmpty(_inlineHandler))
            {
                parent.InnerText = _inlineHandler;
            }

            if (!string.IsNullOrEmpty(_externalHandler))
            {
                parent.SetAttribute("externalHandler", _externalHandler);
            }
		}
	}

    public enum MethodType
    {
        GetMany,
        GetOne,
        Create,
        Update
    }

    public enum HttpMethod
    {
        GET,
        POST
    }
}