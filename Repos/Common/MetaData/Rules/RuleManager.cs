/*
* @(#)RuleManager.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// This is the top level class that manages data validating rules associated with
	/// all classes in a schema and provides methods to allow easy accesses, addition, and 
	/// deletion of the rules.
	/// </summary>
	/// <version> 1.0.0 12 June 2004 </version>
	/// <author> Yong Zhang </author>
	public class RuleManager : RuleNodeBase
	{
		private bool _isAltered;
		
		private RuleSetCollection _ruleSets;

		private Hashtable _table;

        private MetaDataModel _metaData;

		public RuleManager(): base()
		{
			_isAltered = false;
			_ruleSets = new RuleSetCollection();
            _ruleSets.Owner = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _ruleSets.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_table = new Hashtable();
            _metaData = null;
		}

        /// <summary>
        /// Gets or sets the meta-data model in which the rules are defined
        /// </summary>
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
		/// Gets or sets the information indicating whether rule information has been
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
                    // Raise the event if the rules is altered
                    this.FireValueChangedEvent(value);
                }
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty rule set
		/// </summary>
		/// <value>true if it is an empty rule set, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._ruleSets.Count == 0)
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
        /// Gets the detailed data view model of a class
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>The detailed DataViewModel object</returns>
        public DataViewModel GetDataView(string className)
        {
            DataViewModel dataView = null;

            if (_metaData != null)
            {
                dataView = _metaData.GetDetailedDataView(className);
            }

            return dataView;
        }
		
		/// <summary>
		/// Gets the rules, including the inherited ones, for a class
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A collection of Rules</returns>
		public RuleCollection GetRules(ClassElement classElement)
		{
			RuleCollection rules = new RuleCollection();
            rules.Owner = this;
			RuleSet ruleSet;
			ClassElement currentClass = classElement;

			while (currentClass != null)
			{
				// get the RuleSet object based on class name
				ruleSet = (RuleSet) _table[currentClass.Name];
				
				if (ruleSet != null)
				{
					foreach (RuleDef rule in ruleSet.Rules)
					{
						rules.Add(rule);
					}
				}

				// get the rules inherited from the parent class
				currentClass = currentClass.ParentClass;
			}

			return rules;
		}

        /// <summary>
        /// Gets the rules, including the inherited ones, for a class, in an order based
        /// on their priorities.
        /// </summary>
        /// <param name="classElement">The class element</param>
        /// <returns>A prioritized rules</returns>
        public PrioritizedRuleList GetPrioritizedRules(ClassElement classElement)
        {
            PrioritizedRuleList prioritizedList = new PrioritizedRuleList(this);

            RuleCollection rules = GetRules(classElement);
            foreach (RuleDef ruleDef in rules)
            {
                prioritizedList.Add(ruleDef.Priority, ruleDef);
            }

            return prioritizedList;
        }

		/// <summary>
		/// Gets the rules (not including the inherited ones) for a class.
		/// </summary>
        /// <param name="ownerClassName">The owner class name</param>
		/// <returns>A collection of Rules owned by the class</returns>
		public RuleCollection GetLocalRules(string ownerClassName)
		{
            RuleSet ruleSet = (RuleSet)_table[ownerClassName];

			if (ruleSet != null)
			{
				return ruleSet.Rules;
			}
			else
			{
				RuleCollection rules = new RuleCollection(); // return an empty rule collection
                rules.Owner = this;
                return rules;
			}
		}

		/// <summary>
		/// Add a rule for a class
		/// </summary>
		/// <param name="classElement">The class Element</param>
		/// <param name="rule">The RuleDef</param>
		public void AddRule(ClassElement classElement, RuleDef rule)
		{
			// Create a RuleSet instance for the class element if it doesn't exist.
			RuleSet ruleSet = (RuleSet) _table[classElement.Name];

			if (ruleSet == null)
			{
				// class name is unique among the classes in a schema
				ruleSet = new RuleSet(classElement.Name);
				_ruleSets.Add(ruleSet);
				_table[classElement.Name] = ruleSet;
			}
			
			ruleSet.Rules.Add(rule);
		}

		/// <summary>
		/// Remove a rule from a class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <param name="rule">The RuleDef to be removed</param>
		public void RemoveRule(ClassElement classElement, RuleDef rule)
		{
			RuleSet ruleSet = (RuleSet) _table[classElement.Name];

			if (ruleSet != null)
			{
				ruleSet.Rules.Remove(rule);

				if (ruleSet.Rules.Count == 0)
				{
					_ruleSets.Remove(ruleSet);
					_table.Remove(classElement.Name);
				}
			}
		}

        /// <summary>
        /// Get information indicating whether an attribute is referenced by expressions in any of
        /// the rules.
        /// </summary>
        /// <param name="className">The name of attribute owner class</param>
        /// <param name="attributeName">The name of the attribute to be found</param>
        /// <returns>true if the attribute is referenced, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName, out string ruleOwnerCaption)
        {
            bool status = false;
            ruleOwnerCaption = null;

            foreach (RuleSet ruleSet in _ruleSets)
            {
                if (ruleSet.IsAttributeReferenced(className, attributeName))
                {
                    if (_metaData != null)
                    {
                        ClassElement ownerClassElement = _metaData.SchemaModel.FindClass(ruleSet.ClassName);
                        if (ownerClassElement != null)
                        {
                            ruleOwnerCaption = ownerClassElement.Caption;
                        }
                    }

                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Read rules from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
                throw new RuleException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read rules from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
					throw new RuleException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read rules from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
					throw new RuleException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write rules to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
				throw new RuleException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write rules as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
				throw new RuleException("Failed to write the rules", ex);
			}
		}

		/// <summary>
		/// Write rules as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="RuleException">RuleException is thrown when it fails to
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
				throw new RuleException("Failed to write the rules", ex);
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
				return NodeType.RuleManager;
			}
		}

		/// <summary>
		/// create rules from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of RuleSet instances
			_ruleSets = (RuleSetCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            _ruleSets.Owner = this;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _ruleSets.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			foreach (RuleSet ruleSet in _ruleSets)
			{
				_table[ruleSet.ClassName] = ruleSet;
			}
		}

		/// <summary>
		/// write rules to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the rule defs
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_ruleSets.NodeType));
			_ruleSets.Marshal(child);
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

			XmlElement element = doc.CreateElement("RuleManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
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