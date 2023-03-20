/*
* @(#)ApiManager.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema.Validate;

	/// <summary>
	/// This is the top level class that manages apis associated with
	/// all classes in a schema and provides methods to allow easy accesses, addition, and 
	/// deletion of the apis.
	/// </summary>
	/// <version> 1.0.0 16 Oct 2015 </version>
	public class ApiManager : ApiNodeBase
	{
		private bool _isAltered;
		private ApiGroupCollection _apiGroups;
        private Hashtable _hasApisTable;

		/// <summary>
		/// Initiate an instance of ApiManager class
		/// </summary>
		public ApiManager(): base()
		{
			_isAltered = false;
			_apiGroups = new ApiGroupCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _apiGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
            _hasApisTable = new Hashtable();
		}

		/// <summary>
		/// Gets or sets the information indicating whether api information has been
		/// altered.
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;

                if (value)
                {
                    this.FireValueChangedEvent(value);
                }
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty api set
		/// </summary>
		/// <value>true if it is an empty api set, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._apiGroups.Count == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether a class has the apis defined
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>true if it has apis defined, false otherwise</returns>
        public bool HasApis(string className)
        {
            bool status = false;

            if (this._hasApisTable[className] == null)
            {
                ApiCollection apis = this.GetClassApis(className);
               
                if (apis != null && apis.Count > 0)
                {
                    status = true;
                }

                this._hasApisTable[className] = status;
            }
            else
            {
                status = (bool)this._hasApisTable[className];
            }

            return status;
        }

        /// <summary>
        /// Gets the apis for a class.
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>A collection of Apis</returns>
        public ApiCollection GetClassApis(string className)
		{
            ApiGroup apiGroup = (ApiGroup)this._apiGroups[className];

            if (apiGroup != null)
            {
                return apiGroup.Apis;
            }
            else
            {
                return null;
            }
		}

        /// <summary>
        /// Gets the apis for a class.
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="apiName">The api name</param>
        /// <returns>A Api object, null if not matched</returns>
        public Api GetClassApi(string className, string apiName)
        {
            Api found = null;

            ApiCollection apis = GetClassApis(className);
            if (apis != null)
            {
                foreach (Api tmpApi in apis)
                {
                    if (apiName == tmpApi.Name)
                    {
                        found = tmpApi;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Gets the information indicating whether a api has already existed for a
        /// class or for its parent classes.
        /// </summary>
        /// <param name="classElement">The class element</param>
        /// <param name="api">The api</param>
        /// <returns>true if it exists, false otherwise.</returns>
        public bool IsApiExist(string className, Api api)
		{
			bool isExist = false;

			ApiCollection apis = GetClassApis(className);

            if (apis != null)
            {
                foreach (Api tmpApi in apis)
                {
                    if (api.Name == tmpApi.Name)
                    {
                        isExist = true;
                        break;
                    }
                }
            }

			return isExist;
		}

		/// <summary>
		/// Add a api for a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="api">The Api</param>
		public void AddApi(string className, Api api)
		{
			ApiGroup apiGroup = (ApiGroup) this._apiGroups[className];

            // Create a ApiGroup instance for the class element if it doesn't exist.
			if (apiGroup == null)
			{
				// class name is unique among the classes in a schema
				apiGroup = new ApiGroup(className);
				_apiGroups.Add(apiGroup);
			}
			
			if (!IsApiExist(className, api))
			{
				apiGroup.Apis.Add(api);

                _hasApisTable.Remove(className);
			}
		}

		/// <summary>
		/// Remove a api from a class.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="api">The Api to be removed</param>
		public void RemoveApi(string className, Api api)
		{
            ApiGroup apiGroup = (ApiGroup)this._apiGroups[className];

			if (apiGroup != null)
			{
				apiGroup.Apis.Remove(api);

                _hasApisTable.Remove(className);
			}
		}

        /// <summary>
        /// Remove apis belongs to a class.
        /// </summary>
        /// <param name="className">The class name</param>
        public void RemoveApis(string className)
        {
            ApiGroup apiGroup = (ApiGroup)this._apiGroups[className];

            if (apiGroup != null)
            {
                this._apiGroups.Remove(apiGroup);

                _hasApisTable.Remove(className);
            }
        }

        /// <summary>
        /// Validate the event definitions to see if they are valid or not.
        /// </summary>
        /// <param name="metaData">The meta data model to locate the schema model element associated with offending logging rules</param>
        /// <param name="userManager">The User Manager to get users or roles data</param>
        /// <param name="result">The validate result to which to append logging validating errors.</param>
        /// <returns>The result in ValidateResult object</returns>
        public ValidateResult Validate(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
        {
            ApiModelValidateVisitor visitor = new ApiModelValidateVisitor(metaData, userManager, result);

            Accept(visitor); // start validating

            return visitor.ValidateResult;
        }

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IApiNodeVisitor visitor)
        {
            if (visitor.VisitApiManager(this))
            {
                this._apiGroups.Accept(visitor);
            }
        }

		/// <summary>
		/// Read apis from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					//Open the stream and read XSD from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new ApiException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read apis from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// read the stream.</exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(stream);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new ApiException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read apis from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// read the text reader</exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(reader);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new ApiException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write apis to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// write to the file.</exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new ApiException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write apis as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new ApiException("Failed to write the apis", ex);
			}
		}

		/// <summary>
		/// Write apis as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="ApiException">ApiException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new ApiException("Failed to write the apis", ex);
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
				return ApiNodeType.ApiManager;
			}
		}

		/// <summary>
		/// create apis from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of ApiGroup instances
            _apiGroups = (ApiGroupCollection)ApiNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _apiGroups.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write apis to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the api defs
            XmlElement child = parent.OwnerDocument.CreateElement(ApiNodeFactory.ConvertTypeToString(_apiGroups.NodeType));
			_apiGroups.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("ApiManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the ApiManager changed
		/// </summary>
		/// <param name="sender">the IEventNode that cause the api</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}
	}
}