/*
* @(#)XaclPolicy.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
    using Newtera.Common.MetaData.XaclModel.Validate;

	/// <summary>
	/// The class provides methods to allow easy accesses and 
	/// modifications of an underlying access control policy expressed in 
	/// an XACL document. The policy is independent of any view format.
	/// </summary>
	/// <version> 1.0.0 08 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class XaclPolicy : XaclNodeBase
	{
		private bool _isAltered;

		private XaclSetting _setting = null;
		
		private XaclDefCollection _xaclDefs;

		private Hashtable _table;

		/// <summary>
		/// Initiate an instance of XaclPolicy class
		/// </summary>
		public XaclPolicy(): base()
		{
			_isAltered = false;
			_setting = new XaclSetting();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _setting.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_xaclDefs = new XaclDefCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _xaclDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_table = new Hashtable();
		}

		/// <summary>
		/// Gets or sets the information indicating whether the xacl policy has been altered
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
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty policy
		/// </summary>
		/// <value>true if it is an empty policy, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._xaclDefs.Count == 0)
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
		/// Gets the setting of the policy.
		/// </summary>
		/// <value>XaclSetting object.</value>
		public XaclSetting Setting
		{
			get
			{
				return _setting;
			}
		}

        /// <summary>
        /// Validate the xacl policy to see if rules it contains are valid or not.
        /// </summary>
        /// <param name="metaData">The meta data model to locate the schema model element associated with offending xacl rules</param>
        /// <param name="userManager">The User Manager to get users or roles data</param>
        /// <param name="result">The validate result to which to append xacl validating errors.</param>
        /// <returns>The result in ValidateResult object</returns>
        /// <remarks>XaclModelValidateVisitor will also try to find obsolete rules, the obsolete rules
        /// will be deleted as the result of method call</remarks>
        public ValidateResult Validate(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
        {
            XaclModelValidateVisitor visitor = new XaclModelValidateVisitor(metaData, userManager, result);

            Accept(visitor); // start validating

            XaclDefCollection obsoleteDefs = new XaclDefCollection();
            foreach (XaclDef xaclDef in this.XaclDefs)
            {
                if (xaclDef.IsObsolete)
                {
                    obsoleteDefs.Add(xaclDef);
                }
            }

            foreach (XaclDef xaclDef in obsoleteDefs)
            {
                this.XaclDefs.Remove(xaclDef);
                this.IsAltered = true;
            }

            return visitor.ValidateResult;
        }

		/// <summary>
		/// Gets all the xacl definitions of the policy.
		/// </summary>
		/// <value> All the xacl definitions of a policy.</value>
		public XaclDefCollection XaclDefs
		{
			get
			{
				return _xaclDefs;
			}
		}

        /// <summary>
        /// Gets the resolution permission of an action when there are conflicts between matched rules.
        /// </summary>
        /// <returns>A XaclPermissionType object</returns>
        public XaclPermissionType GetResolutionPermission(XaclActionType actionType)
        {
            XaclPermissionType resolutionPermission = XaclPermissionType.Unknown;

            XaclConflictResolutionType resolutionType = Setting.ConflictResolutionType;

            switch (resolutionType)
            {
                case XaclConflictResolutionType.Dtp:
                    resolutionPermission = XaclPermissionType.Deny;
                    break;
                case XaclConflictResolutionType.Gtp:
                    resolutionPermission = XaclPermissionType.Grant;
                    break;
                case XaclConflictResolutionType.Ntp:
                    // take the default permission
                    switch (actionType)
                    {
                        case XaclActionType.Read:
                            resolutionPermission = Setting.DefaultReadPermission;
                            break;
                        case XaclActionType.Write:
                            resolutionPermission = Setting.DefaultWritePermission;
                            break;
                        case XaclActionType.Create:
                            resolutionPermission = Setting.DefaultCreatePermission;
                            break;
                        case XaclActionType.Delete:
                            resolutionPermission = Setting.DefaultDeletePermission;
                            break;
                        case XaclActionType.Upload:
                            resolutionPermission = Setting.DefaultUploadPermission;
                            break;
                        case XaclActionType.Download:
                            resolutionPermission = Setting.DefaultDownloadPermission;
                            break;
                    }
                    break;
            }

            return resolutionPermission;
        }
		
		/// <summary>
		/// Gets the rules, including the inherited ones, for an XaclObject
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <returns>A collection of XaclRule instances</returns>
		public XaclRuleCollection GetRules(XaclObject obj)
		{
			XaclRuleCollection rules = new XaclRuleCollection();
            XaclRuleCollection overridingRules = new XaclRuleCollection();
			XaclDef def;
			string href = obj.Href;

            int level = 0;
			while (href != null)
			{
				// get the XaclDef object based on href
				def = (XaclDef) _table[href];
				
				if (def != null)
				{
					foreach (XaclRule rule in def.Rules)
					{
						rule.ObjectHref = href;
						if (href == obj.Href)
						{
							// it's a local rule, simply add it
							rules.Add(rule);
						}
						else if (rule.AllowPropagation)
						{
                            if (!IsRuleOverrided(rule, rules, overridingRules))
                            {
							    // it's an inherited rule through propagation
							    rules.Add(rule);
                            }
						}
                        else 
                        {
                            if (rule.IsOverrided)
                            {
                                overridingRules.Add(rule);
                            }

                            // if the obj represent a class attribute, even though the rule
                            // in its owner class with AllowPropagation setting to false, we
                            // still let the attribute inherites the rule
                            if (obj.IsClassAttribute && level == 1)
                            {
                                rules.Add(rule);
                            }
                        }
					}
				}

				// get a href of the parent XaclObject
				// href is represented as a xpath with / as separator
				int pos = href.LastIndexOf('/');
				if (pos > 0)
				{
					href = href.Substring(0, pos);
				}
				else
				{
					href = null;
				}

                level++;
			}

			return rules;
		}

		/// <summary>
		/// Gets the rules (not including the inherited ones) for an XaclObject
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <returns>A collection of XaclRule instances</returns>
		public XaclRuleCollection GetLocalRules(XaclObject obj)
		{
			string href = obj.Href;

			XaclDef def = (XaclDef) _table[href];

			if (def != null)
			{
				return def.Rules;
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Gets information indicating whether a SchemaModelElement has rules defined localy
        /// </summary>
        /// <param name="schemaModelElement"></param>
        /// <returns>true if it has local rules, false otherwise</returns>
        public bool HasLocalRules(IXaclObject schemaModelElement)
        {
            bool status = false;
            XaclObject obj = new XaclObject(schemaModelElement.ToXPath());

			XaclRuleCollection rules = this.GetLocalRules(obj);
            if (rules != null && rules.Count > 0)
            {
                status = true;
            }

            return status;
        }

		/// <summary>
		/// Gets the information indicating whether a rule for an XaclObject with the
		/// same subject has already existed.
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <param name="rule">The rule</param>
		/// <returns>true if it exists, false otherwise.</returns>
		/// <remarks>If a rule with the same subject exists in the local XaclDef
		/// instance, then this method will return true. If an inherited rule from
		/// the parent XaclDef instances with the same subject exists,
		/// this method will return true as well
		/// </remarks>
		public bool IsRuleExist(XaclObject obj, XaclRule rule)
		{
			bool isExist = false;

			XaclRuleCollection rules = GetRules(obj);

			foreach (XaclRule tmpRule in rules)
			{
				if (rule.Subject.Equals(tmpRule.Subject))
				{
					isExist = true;
					break;
				}
			}

			return isExist;
		}

		/// <summary>
		/// Gets the information indicating whether a local rule for an XaclObject with the
		/// same subject has already existed.
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <param name="rule">The rule</param>
		/// <returns>true if it exists, false otherwise.</returns>
		/// <remarks>If a rule with the same subject exists in the local XaclDef
		/// instance, then this method will return true. It won't search up for
		/// the propagated rules
		/// </remarks>
		public bool IsLocalRuleExist(XaclObject obj, XaclRule rule)
		{
			bool isExist = false;

			XaclRuleCollection rules = GetLocalRules(obj);

			if (rules != null)
			{
				foreach (XaclRule tmpRule in rules)
				{
					if (rule.Subject.Equals(tmpRule.Subject) &&
						rule.ObjectHref == tmpRule.ObjectHref)
					{
						isExist = true;
						break;
					}
				}
			}

			return isExist;
		}

		/// <summary>
		/// Gets the most immediate propagated rule of a given local rule.
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <param name="rule">The local rule</param>
		/// <returns>A XaclRule object, null if there is none</returns>
		/// <remarks>A propagated rule is the one in the path of the given
		/// XaclObject and has AllowPropagate set to true.
		/// </remarks>
		public XaclRule GetPropagatedRule(XaclObject obj, XaclRule rule)
		{
			XaclRule propagatedRule = null;

			XaclRuleCollection rules = GetRules(obj);

			foreach (XaclRule tmpRule in rules)
			{
				if (rule.Subject.Equals(tmpRule.Subject))
				{
					propagatedRule = tmpRule;
					break;
				}
			}

			return propagatedRule;
		}

		/// <summary>
		/// Add a rule for an XaclObject in a ploicy
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <param name="rule">The XaclRule</param>
		public void AddRule(XaclObject obj, XaclRule rule)
		{
			// Create an XaclDef instance for the XaclObject if it doesn't exist.
			XaclDef xaclDef = (XaclDef) _table[obj.Href];

			if (xaclDef == null)
			{
				xaclDef = new XaclDef(obj);
				_xaclDefs.Add(xaclDef);
				_table[obj.Href] = xaclDef;
			}

			if (!IsLocalRuleExist(obj, rule))
			{
				rule.ObjectHref = obj.Href;
				xaclDef.Rules.Add(rule);
			}
		}

		/// <summary>
		/// Remove a rule for an XaclObject from a ploicy
		/// </summary>
		/// <param name="obj">The XaclObject</param>
		/// <param name="rule">The XaclRule to be removed</param>
		public void RemoveRule(XaclObject obj, XaclRule rule)
		{
			XaclDef xaclDef = (XaclDef) _table[obj.Href];

			if (xaclDef != null)
			{
				xaclDef.Rules.Remove(rule);

				if (xaclDef.Rules.Count == 0)
				{
					_xaclDefs.Remove(xaclDef);
					_table.Remove(xaclDef.Object.Href);
				}
			}
		}

		/// <summary>
		/// Constrauct a xacl policy from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
                throw new XaclException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a xacl policy from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
					throw new XaclException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a xacl policy from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
					throw new XaclException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write a xacl policy to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
				throw new XaclException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write a xacl policy as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
				throw new XaclException("Failed to write the xacl policy", ex);
			}
		}

		/// <summary>
		/// Write a xacl policy as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="XaclException">XaclException is thrown when it fails to
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
				throw new XaclException("Failed to write the xacl policy", ex);
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
				return NodeType.Policy;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            if (visitor.VisitXaclPolicy(this))
            {
                _xaclDefs.Accept(visitor);
            }
        }

		/// <summary>
		/// create an policy from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// the first child is XaclSetting
			_setting = (XaclSetting) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _setting.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of XaclDef instances
			_xaclDefs = (XaclDefCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _xaclDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			foreach (XaclDef xacl in _xaclDefs)
			{
				_table[xacl.Object.Href] = xacl;
			}
		}

		/// <summary>
		/// write policy to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _setting
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_setting.NodeType));
			_setting.Marshal(child);
			parent.AppendChild(child);
			

			// write the xacl defs
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_xaclDefs.NodeType));
			_xaclDefs.Marshal(child);
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

			XmlElement element = doc.CreateElement("Policy");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// Gets the information indicating whether a rule with the same subject has
		/// already existed in the collection.
		/// </summary>
		/// <param name="rule">The rule</param>
		/// <param name="rules">The rules that have been matched</param>
        /// <param name="overridingRules">The rules that overrides the inherited ones but not propagated</param>
		/// <returns>true if it exists, false otherwise.</returns>
		private bool IsRuleOverrided(XaclRule rule, XaclRuleCollection rules, XaclRuleCollection overridingRules)
		{
			bool isExist = false;

			foreach (XaclRule tmpRule in rules)
			{
				if (rule.Subject.Equals(tmpRule.Subject) && tmpRule.IsOverrided)
				{
					isExist = true;
					break;
				}
			}

            foreach (XaclRule tmpRule in overridingRules)
            {
                if (rule.Subject.Equals(tmpRule.Subject))
                {
                    isExist = true;
                    break;
                }
            }

			return isExist;
		}

		/// <summary>
		/// A handler to call when a value of the xacl policy changed
		/// </summary>
		/// <param name="sender">the IXaclNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}
	}
}