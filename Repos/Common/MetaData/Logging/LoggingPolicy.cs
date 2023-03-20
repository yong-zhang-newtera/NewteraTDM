/*
* @(#)LoggingPolicy.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
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
    using Newtera.Common.MetaData.Logging.Validate;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The class provides methods to allow easy accesses and 
	/// modifications of an underlying logging policy.
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	public class LoggingPolicy : LoggingNodeBase
	{
		private bool _isAltered;

		private LoggingSetting _setting = null;
		
		private LoggingDefCollection _loggingDefs;

		private Hashtable _table;

		/// <summary>
		/// Initiate an instance of LoggingPolicy class
		/// </summary>
		public LoggingPolicy(): base()
		{
			_isAltered = false;
			_setting = new LoggingSetting();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _setting.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_loggingDefs = new LoggingDefCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _loggingDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_table = new Hashtable();
		}

		/// <summary>
		/// Gets or sets the information indicating whether the logging policy has been altered
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
				if (this._loggingDefs.Count == 0)
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
		/// <value>LoggingSetting object.</value>
		public LoggingSetting Setting
		{
			get
			{
				return _setting;
			}
		}

        /// <summary>
        /// Validate the logging policy to see if rules it contains are valid or not.
        /// </summary>
        /// <param name="metaData">The meta data model to locate the schema model element associated with offending logging rules</param>
        /// <param name="userManager">The User Manager to get users or roles data</param>
        /// <param name="result">The validate result to which to append logging validating errors.</param>
        /// <returns>The result in ValidateResult object</returns>
        /// <remarks>LoggingModelValidateVisitor will also try to find obsolete rules, the obsolete rules
        /// will be deleted as the result of method call</remarks>
        public ValidateResult Validate(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
        {
            LoggingModelValidateVisitor visitor = new LoggingModelValidateVisitor(metaData, userManager, result);

            Accept(visitor); // start validating

            LoggingDefCollection obsoleteDefs = new LoggingDefCollection();
            foreach (LoggingDef loggingDef in this.LoggingDef)
            {
                if (loggingDef.IsObsolete)
                {
                    obsoleteDefs.Add(loggingDef);
                }
            }

            foreach (LoggingDef loggingDef in obsoleteDefs)
            {
                this.LoggingDef.Remove(loggingDef);
                this.IsAltered = true;
            }

            return visitor.ValidateResult;
        }

		/// <summary>
		/// Gets all the logging definitions of the policy.
		/// </summary>
		/// <value> All the logging definitions of a policy.</value>
		public LoggingDefCollection LoggingDef
		{
			get
			{
				return _loggingDefs;
			}
		}
		
		/// <summary>
		/// Gets the rules, including the inherited ones, for an LoggingObject
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <returns>A collection of LoggingRule instances</returns>
		public LoggingRuleCollection GetRules(LoggingObject obj)
		{
			LoggingRuleCollection rules = new LoggingRuleCollection();
            LoggingRuleCollection overridingRules = new LoggingRuleCollection();
			LoggingDef def;
			string href = obj.Href;

            int level = 0;
			while (href != null)
			{
				// get the LoggingDef object based on href
				def = (LoggingDef) _table[href];
				
				if (def != null)
				{
					foreach (LoggingRule rule in def.Rules)
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

				// get a href of the parent LoggingObject
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
		/// Gets the rules (not including the inherited ones) for an LoggingObject
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <returns>A collection of LoggingRule instances</returns>
		public LoggingRuleCollection GetLocalRules(LoggingObject obj)
		{
			string href = obj.Href;

			LoggingDef def = (LoggingDef) _table[href];

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
            LoggingObject obj = new LoggingObject(schemaModelElement.ToXPath());

			LoggingRuleCollection rules = this.GetLocalRules(obj);
            if (rules != null && rules.Count > 0)
            {
                status = true;
            }

            return status;
        }

		/// <summary>
		/// Gets the information indicating whether a rule for an LoggingObject with the
		/// same subject has already existed.
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <param name="rule">The rule</param>
		/// <returns>true if it exists, false otherwise.</returns>
		/// <remarks>If a rule with the same subject exists in the local LoggingDef
		/// instance, then this method will return true. If an inherited rule from
		/// the parent LoggingDef instances with the same subject exists,
		/// this method will return true as well
		/// </remarks>
		public bool IsRuleExist(LoggingObject obj, LoggingRule rule)
		{
			bool isExist = false;

			LoggingRuleCollection rules = GetRules(obj);

			foreach (LoggingRule tmpRule in rules)
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
		/// Gets the information indicating whether a local rule for an LoggingObject with the
		/// same subject has already existed.
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <param name="rule">The rule</param>
		/// <returns>true if it exists, false otherwise.</returns>
		/// <remarks>If a rule with the same subject exists in the local LoggingDef
		/// instance, then this method will return true. It won't search up for
		/// the propagated rules
		/// </remarks>
		public bool IsLocalRuleExist(LoggingObject obj, LoggingRule rule)
		{
			bool isExist = false;

			LoggingRuleCollection rules = GetLocalRules(obj);

			if (rules != null)
			{
				foreach (LoggingRule tmpRule in rules)
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
		/// <param name="obj">The LoggingObject</param>
		/// <param name="rule">The local rule</param>
		/// <returns>A LoggingRule object, null if there is none</returns>
		/// <remarks>A propagated rule is the one in the path of the given
		/// LoggingObject and has AllowPropagate set to true.
		/// </remarks>
		public LoggingRule GetPropagatedRule(LoggingObject obj, LoggingRule rule)
		{
			LoggingRule propagatedRule = null;

			LoggingRuleCollection rules = GetRules(obj);

			foreach (LoggingRule tmpRule in rules)
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
		/// Add a rule for an LoggingObject in a ploicy
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <param name="rule">The LoggingRule</param>
		public void AddRule(LoggingObject obj, LoggingRule rule)
		{
			// Create an LoggingDef instance for the LoggingObject if it doesn't exist.
			LoggingDef loggingDef = (LoggingDef) _table[obj.Href];

			if (loggingDef == null)
			{
				loggingDef = new LoggingDef(obj);
				_loggingDefs.Add(loggingDef);
				_table[obj.Href] = loggingDef;
			}

			if (!IsLocalRuleExist(obj, rule))
			{
				rule.ObjectHref = obj.Href;
				loggingDef.Rules.Add(rule);
			}
		}

		/// <summary>
		/// Remove a rule for an LoggingObject from a ploicy
		/// </summary>
		/// <param name="obj">The LoggingObject</param>
		/// <param name="rule">The LoggingRule to be removed</param>
		public void RemoveRule(LoggingObject obj, LoggingRule rule)
		{
			LoggingDef loggingDef = (LoggingDef) _table[obj.Href];

			if (loggingDef != null)
			{
				loggingDef.Rules.Remove(rule);

				if (loggingDef.Rules.Count == 0)
				{
					_loggingDefs.Remove(loggingDef);
					_table.Remove(loggingDef.Object.Href);
				}
			}
		}

		/// <summary>
		/// Constrauct a logging policy from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					//Open the stream and read xml from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new LoggingException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a logging policy from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
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
					throw new LoggingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a logging policy from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
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
					throw new LoggingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write a logging policy to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
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
				throw new LoggingException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write a logging policy as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
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
				throw new LoggingException("Failed to write the logging policy", ex);
			}
		}

		/// <summary>
		/// Write a logging policy as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="LoggingException">LoggingException is thrown when it fails to
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
				throw new LoggingException("Failed to write the logging policy", ex);
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
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            if (visitor.VisitLoggingPolicy(this))
            {
                _loggingDefs.Accept(visitor);
            }
        }

		/// <summary>
		/// create an policy from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// the first child is LoggingSetting
			_setting = (LoggingSetting) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _setting.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of LoggingDef instances
			_loggingDefs = (LoggingDefCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _loggingDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			foreach (LoggingDef logging in _loggingDefs)
			{
				_table[logging.Object.Href] = logging;
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
			

			// write the logging defs
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_loggingDefs.NodeType));
			_loggingDefs.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an logging policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("LoggingPolicy");

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
		private bool IsRuleOverrided(LoggingRule rule, LoggingRuleCollection rules, LoggingRuleCollection overridingRules)
		{
			bool isExist = false;

			foreach (LoggingRule tmpRule in rules)
			{
				if (rule.Subject.Equals(tmpRule.Subject) && tmpRule.IsOverrided)
				{
					isExist = true;
					break;
				}
			}

            foreach (LoggingRule tmpRule in overridingRules)
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
		/// A handler to call when a value of the logging policy changed
		/// </summary>
		/// <param name="sender">the IXaclNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}
	}
}