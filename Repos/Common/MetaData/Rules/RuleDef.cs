/*
* @(#)RuleDef.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;
    using System.Text;
	using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// The class represents definition of a data validating rule for a certain class.
	/// </summary>
	/// <version>1.0.0 16 Jun 2004</version>
	/// <author> Yong Zhang </author>
	public class RuleDef : RuleNodeBase
	{
		private string _className;
        private ReferencedClassCollection _referencedClasses;
		private Filter _condition;
        private Filter _thenAction;
        private Filter _elseAction;
        private int _priority = 0; // o is a default priority
        private DataViewModel _dataView = null; // run-time variable
        private string _objId = null; // run-time variable
        private string _alias = null; // run-time variable
		
		/// <summary>
		/// Initiate an instance of RuleDef class.
		/// </summary>
		public RuleDef() : base()
		{
			_className = null;
            _condition = new Filter();
            _thenAction = new Filter();
            _elseAction = new Filter();
            _priority = 0;
            _referencedClasses = new ReferencedClassCollection();
		}

		/// <summary>
		/// Initiating an instance of RuleDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal RuleDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
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
        /// Gets or sets the DataViewModel object for the class to which the rule belongs to.
        /// </summary>
        /// <value>The DataViewModel object.</value>
        [BrowsableAttribute(false)]	
        public DataViewModel DataView
        {
            get
            {
                if (_dataView == null)
                {
                    IRuleNode owner = this.Owner;
                    while (owner != null)
                    {
                        if (owner is RuleManager)
                        {
                            _dataView = ((RuleManager)owner).GetDataView(ClassName);
                            break;
                        }
                        else
                        {
                            owner = owner.Owner;
                        }
                    }
                }

                return _dataView;
            }
            set
            {
                _dataView = value;
            }
        }

        /// <summary>
        /// Gets or sets the data instance id to the rule since some of the functions
        /// may depends on it.
        /// </summary>
        /// <value>The DataViewModel object.</value>
        [BrowsableAttribute(false)]
        public string ObjId
        {
            get
            {
                return _objId;
            }
            set
            {
                _objId = value;
            }
        }

        /// <summary>
        /// Gets alias of the class that owns the rules.
        /// </summary>
        /// <value>The class alias.</value>
        [BrowsableAttribute(false)]
        public string ClassAlias
        {
            get
            {
                if (_alias == null && ClassName != null)
                {
                    _alias = ClassName.ToLower();
                }

                return _alias;
            }
        }

		/// <summary>
		/// Gets or sets condition of the rule.
		/// </summary>
		/// <value> The rule expression.</value>
        [
            CategoryAttribute("Condition"),
            DescriptionAttribute("The condition of the rule"),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.RuleConditionEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.RuleConditionConverter, Studio")
        ]
        public IDataViewElement Condition
		{
			get
			{
                if (_condition.DataView == null)
                {
                    _condition.DataView = DataView;
                }
				return _condition.Expression;
			}
			set
			{
                _condition.Expression = value;
			}
		}

        /// <summary>
        /// Gets or sets then action of the rule.
        /// </summary>
        /// <value> The then action.</value>
        [
            CategoryAttribute("Action"),
            DescriptionAttribute("Then action of the rule"),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.RuleActionEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.RuleActionConverter, Studio")
        ]
        public IDataViewElement ThenAction
        {
            get
            {
                if (_thenAction.DataView == null)
                {
                    _thenAction.DataView = DataView;
                }
                return _thenAction.Expression;
            }
            set
            {
                _thenAction.Expression = value;
            }
        }

        /// <summary>
        /// Gets or sets else action of the rule. The else action is optional
        /// </summary>
        /// <value> The else action.</value>
        [
            CategoryAttribute("Action"),
            DescriptionAttribute("Else action of the rule"),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.RuleActionEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.RuleActionConverter, Studio")
        ]
        public IDataViewElement ElseAction
        {
            get
            {
                if (_elseAction.DataView == null)
                {
                    _elseAction.DataView = DataView;
                }
                return _elseAction.Expression;
            }
            set
            {
                _elseAction.Expression = value;
            }
        }

        /// <summary>
        /// Gets or sets priority of the rule.
        /// </summary>
        /// <value> The big the number is, the higher the priority.</value>
        [
            DescriptionAttribute("The priority of the rule"),
            DefaultValueAttribute(0)
        ]
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
        [BrowsableAttribute(false)]	
        public override NodeType NodeType
		{
			get
			{
				return NodeType.RuleDef;
			}
		}

        /// <summary>
        /// Clone a rule
        /// </summary>
        /// <returns>The cloned rule.</returns>
        public RuleDef Clone()
        {
            // use Marshal and Unmarshal to clone a RuleDef
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Rule");
            doc.AppendChild(root);
            XmlElement child = doc.CreateElement(NodeFactory.ConvertTypeToString(this.NodeType));
            this.Marshal(child);
            root.AppendChild(child);

            // create a new RuleDef and unmarshal from the xml element as source
            RuleDef ruleDef = new RuleDef(child);
            ruleDef.Owner = this.Owner;

            return ruleDef;
        }

        /// <summary>
        /// Get information indicating whether an attribute is referenced by the rule
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced by the rule, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;
            FindSearchAttributeVisitor visitor = new FindSearchAttributeVisitor(className, attributeName);

            // check in the Condition
            if (Condition != null)
            {
                visitor = new FindSearchAttributeVisitor(className, attributeName);
                Condition.Accept(visitor);
                if (visitor.IsFound)
                {
                    status = true;
                }
            }

            if (!status)
            {
                if (ThenAction != null)
                {
                    visitor = new FindSearchAttributeVisitor(className, attributeName);
                    ThenAction.Accept(visitor);
                    if (visitor.IsFound)
                    {
                        status = true;
                    }
                }
            }

            if (!status)
            {
                if (ElseAction != null)
                {
                    visitor = new FindSearchAttributeVisitor(className, attributeName);
                    ElseAction.Accept(visitor);
                    if (visitor.IsFound)
                    {
                        status = true;
                    }
                }
            }

            if (!status)
            {
                if (_referencedClasses != null)
                {
                    // checking the referenced class for relationships
                    foreach (DataClass refClass in _referencedClasses)
                    {
                        if (refClass.ReferringRelationshipName == attributeName)
                        {
                            DataClass referringClass = this.FindClass(refClass.ReferringClassAlias);
                            if (referringClass != null)
                            {
                                ClassElement schemaModelElement = referringClass.GetSchemaModelElement() as ClassElement;
                                if (schemaModelElement != null &&
                                    schemaModelElement.Name == className)
                                {
                                    status = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Propagate the parameters to the functions contained in the rule
        /// </summary>
        public void PropagateParameters()
        {
            DataViewModel dataView = DataView;
            PropagateFunctionParametersVisitor visitor = new PropagateFunctionParametersVisitor(dataView.SchemaInfo.Name,
                dataView.SchemaInfo.Version, dataView.BaseClass.Name, _objId);
            if (this.Condition != null)
            {
                this.Condition.Accept(visitor);
            }

            if (this.ThenAction != null)
            {
                this.ThenAction.Accept(visitor);
            }

            if (this.ElseAction != null)
            {
                this.ElseAction.Accept(visitor);
            }
        }

		/// <summary>
		/// create an RuleDef from a xml document.
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

            str = parent.GetAttribute("priority");
            if (!string.IsNullOrEmpty(str))
            {
                _priority = Int32.Parse(str);
            }
            else
            {
                _priority = 0;
            }

            // create rule condition
            _condition = (Filter)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);

            // create rule then action
            _thenAction = (Filter)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[1]);

            // create rule else action
            _elseAction = (Filter)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[2]);
		}

		/// <summary>
		/// write RuleDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("class", _className);
			}

            if (_priority > 0)
            {
                parent.SetAttribute("priority", _priority.ToString());
            }

            // write the _condition
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_condition.ElementType));
            _condition.Marshal(child);
            parent.AppendChild(child);

            // write the _thenAction
            child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_thenAction.ElementType));
            _thenAction.Marshal(child);
            parent.AppendChild(child);

            // write the _elseAction
            child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_elseAction.ElementType));
            _elseAction.Marshal(child);
            parent.AppendChild(child);
		}

        /// <summary>
        /// return the rule in text form
        /// </summary>
        /// <returns>Rule text</returns>
        public override string ToString()
        {
            FlattenSearchFiltersVisitor visitor;
            DataViewElementCollection flattenExprs;
            string conditionString = "";
            string thenActionString = "";
            string elseActionString = "";

            if (this.Condition != null)
            {
                visitor = new FlattenSearchFiltersVisitor();

                this.Condition.Accept(visitor);

                flattenExprs = visitor.FlattenedSearchFilters;

                foreach (IDataViewElement element in flattenExprs)
                {
                    conditionString += element.ToString();
                }
            }

            if (this.ThenAction != null)
            {
                visitor = new FlattenSearchFiltersVisitor();

                this.ThenAction.Accept(visitor);

                flattenExprs = visitor.FlattenedSearchFilters;

                foreach (IDataViewElement element in flattenExprs)
                {
                    thenActionString += element.ToString();
                }
            }

            if (this.ElseAction != null)
            {
                visitor = new FlattenSearchFiltersVisitor();

                this.ElseAction.Accept(visitor);

                flattenExprs = visitor.FlattenedSearchFilters;

                foreach (IDataViewElement element in flattenExprs)
                {
                    elseActionString += element.ToString();
                }
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("IF ").Append(conditionString).Append(" THEN ").Append(thenActionString);

            if (!string.IsNullOrEmpty(elseActionString))
            {
                builder.Append(" ELSE ").Append(elseActionString);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Find the class element in the data view given a class alias
        /// </summary>
        /// <param name="alias">The class alias</param>
        /// <returns>A DataClass element</returns>
        private DataClass FindClass(string alias)
        {
            DataClass found = null;

            if (!string.IsNullOrEmpty(alias))
            {
                foreach (DataClass dataClass in _referencedClasses)
                {
                    if (dataClass.Alias == alias)
                    {
                        found = dataClass;
                        break;
                    }
                }
            }

            return found;
        }
	}
}