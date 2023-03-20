/*
* @(#)DataRelationshipAttribute.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
    using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A DataRelationshipAttribute instance represents a relationship attribute in a data view.
	/// It can appears in the result attribute collection.
	/// </summary>
	/// <version>1.0.1 28 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class DataRelationshipAttribute : DataViewElementBase, IQueryElement
	{
		private string _ownerClassAlias;
		private string _linkedClassName;
        private bool _showPrimaryKeys;
		private string _function;
		private int _pkCount;
		private DataViewElementCollection _primaryKeys; // run-time use
        private DataClass _referencedClass; // run-time use
		private string _value; // run-time use only

		/// <summary>
		/// Initiating an instance of DataRelationshipAttribute class
		/// </summary>
		/// <param name="name">Name of the relationship attribute</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns this attribute</param> 
		/// <param name="linkedClassName">The name of the linked class</param>
		public DataRelationshipAttribute(string name, string ownerClassAlias, string linkedClassName) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_linkedClassName = linkedClassName;
			_function = null;
			_pkCount = 0;
			_primaryKeys = null;
            _referencedClass = null;
			_value = null; // run-time used only
            _showPrimaryKeys = false;
		}

		/// <summary>
		/// Initiating an instance of DataRelationshipAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataRelationshipAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_value = null; // run-time used only
		}

        /// <summary>
        /// Get an unique name for the DataTable in a DataSet that represents the primary key data of a class referenced by a relationship
        /// </summary>
        /// <param name="baseClassName">The name of a base class where the relationship is part of</param>
        /// <param name="relationshipAttributeName">The name of relationship attribute</param>
        /// <returns>An unique name for the DataTable in a DataSet</returns>
        /// <remarks>When two base class inherits a relationship or happens to have a relationship of the same name,
        /// it requires to use the base class name as part of the DataTable name to distinguish the DataTable for different
        /// relatiosnhip attributes when a DataSet contains data for both base classes.</remarks>
        static public string GetRelationshipDataTableName(string baseClassName, string relationshipAttributeName)
        {
            return baseClassName + "_" + relationshipAttributeName;
        }

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.RelationshipAttribute;
			}
		}

        /// <summary>
        /// Gets or sets an alias that is used to identifies the simple attribute when used
        /// in the search expression of data view.
        /// </summary>
        /// <value>A string, can be null.</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                {
                    // default alias
                    string alias = _ownerClassAlias + "_" + Name;
                    base.Alias = alias;
                }

                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

		/// <summary>
		/// Gets the information indicating whether the attribute has a function associated.
		/// </summary>
		/// <value>true if it has, false otherwise</value>
		public bool HasFunction
		{
			get
			{
				if (_function != null && _function.Length > 0)
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
		/// Gets or sets the aggregate function for the relationship attribute.
		/// This allows to set function such count functions
		/// </summary>
		/// <value>The function name</value>
		public string Function
		{
			get
			{
				if (_function != null && _function.Length > 0)
				{
					return _function;
				}
				else
				{
					return "";
				}
			}
			set
			{
				_function = value;
			}
		}

        /// <summary>
        /// Gets the information indicating whether to display the primary keys of the referenced class
        /// referred by the relationship as the result.
        /// </summary>
        /// <value>true if it has, false otherwise</value>
        public bool ShowPrimaryKeys
        {
            get
            {
                return _showPrimaryKeys;
            }
            set
            {
                _showPrimaryKeys = value;
            }
        }

		/// <summary>
		/// Gets alias of class that owns this attribute
		/// </summary>
		/// <value>Owner class alias</value>
		public string OwnerClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}

		/// <summary>
		/// Gets unique alias of the linked class.
		/// </summary>
		/// <value>Linked class alias</value>
		public string LinkedClassAlias
		{
			get
			{
				return ReferencedDataClass.Alias;
			}
		}

		/// <summary>
		/// Gets name of the linked class.
		/// </summary>
		/// <value>Linked class name</value>
		public string LinkedClassName
		{
			get
			{
				return _linkedClassName;
			}
		}

        /// <summary>
        /// Gets the DataClass object for the class referenced by the relationship
        /// </summary>
        public DataClass ReferencedDataClass
        {
            get
            {
                if (_referencedClass == null)
                {
                    RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)GetSchemaModelElement();

                    _referencedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                        relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                    _referencedClass.ReferringClassAlias = DataView.BaseClass.Alias; // base class is the parent
                    _referencedClass.ReferringRelationshipName = relationshipAttribute.Name;
                    _referencedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                    _referencedClass.ReferringRelationship = relationshipAttribute;
                }

                return _referencedClass;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a foreign key is required for the
		/// relationship
        /// </summary>
        public bool IsForeignKeyRequired
        {
            get
            {
                RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)GetSchemaModelElement();

                return relationshipAttribute.IsForeignKeyRequired;
            }
        }

		/// <summary>
		/// Gets primary keys of the linked class of a relationship attribute
		/// </summary>
		/// <value>A DataViewElementCollection, null if there is no primary keys</value>
		public DataViewElementCollection PrimaryKeys
		{
			get
			{
                if (_primaryKeys == null)
				{
                    _primaryKeys = GetPrimaryKeys();
				}

				return _primaryKeys;
			}
		}

		/// <summary>
		/// Get the count of primary keys of the linked class
		/// </summary>
		/// <value>A number of pk, default is 0</value>
		public int PrimaryKeyCount
		{
			get
			{
				if (PrimaryKeys != null)
				{
					return PrimaryKeys.Count;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the primary keys of relationship
		/// attribute has an value(s)
		/// </summary>
		/// <value>true if it has, false, otherwise</value>
		public bool HasValue
		{
			get
			{
				bool status = true;

				if (PrimaryKeys != null)
				{
					foreach (DataSimpleAttribute pk in PrimaryKeys)
					{
						if (!pk.HasValue)
						{
							status = false;
							break;
						}
					}
				}

				return status;
			}
		}

		/// <summary>
		/// Gets or sets the value of a relationship attribute.
		/// </summary>
		/// <value>The attribute value</value>
		/// <remarks>The value of RelationshipAttribute is the obj_id of an instance
		/// that this relationship linked to. </remarks>
		public string AttributeValue
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the given obj_id is different from
        /// the obj_id kept by the attribute
        /// </summary>
        /// <param name="val">The given value</param>
        /// <returns>true if they are different, false otherwise.</returns>
        /// <remarks>If the give value is null or empty string, and the attribute
        /// value is null or empty string, they are considered to be same</remarks>
        public bool IsValueDifferent(string val)
        {
            string attributeValue = this.AttributeValue;
            if (attributeValue != null && attributeValue.Length == 0)
            {
                attributeValue = null;
            }

            if (val != null && val.Length == 0)
            {
                val = null;
            }

            if (attributeValue != val)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		public override DataViewModel DataView
		{
			get
			{
				return base.DataView;
			}
			set
			{
				base.DataView = value;
			}
		}

		/// <summary>
		/// Gets an unique name for primary key.
		/// </summary>
		/// <param name="pkName">The name of primary key</param>
		/// <returns>A unique name for primary name</returns>
		public string GetUniquePKName(string pkName)
		{
			return this.Name + "_" + pkName;
		}

		/// <summary>
		/// Split values of the primary keys in a value string
		/// </summary>
		/// <param name="primaryKeyValues">Connected primary key values</param>
		/// <returns>The split values in a string array, could be null</returns>
		public string[] SplitPKValues(string primaryKeyValues)
		{
            return Parameter.SplitValues(primaryKeyValues);
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitRelationshipBegin(this);

            /*
			if (visitor.VisitRelationshipAttribute(this))
			{
                if (primaryKeyExpr != null)
				{
                    primaryKeyExpr.Accept(visitor);
				}
			}
             */

            visitor.VisitRelationshipAttribute(this);
   
			visitor.VisitRelationshipEnd(this);
		}

        /// <summary>
        /// Gets an AND expression consisting of multiple relational expressions, each
        /// relational expression consisting of a primary key of referenced class as left side
        /// and a parameter(s) as right side of the provided operator type.
        /// For example, FirstName = "David" AND LastName="Li", where FirstName and LastName
        /// are the primary keys of a referenced class.
        /// </summary>
        /// <value>The constructed element</value>
        public IDataViewElement GetPrimaryKeyFilter(BinaryExpr expectedExpr, IParameter parameter)
        {
            RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)GetSchemaModelElement();
            IDataViewElement pkFilter = null;
            IDataViewElement expr = null;
            IDataViewElement left = null;
            IDataViewElement right = null;
            ClassElement linkedClass = relationshipAttribute.LinkedClass;
            SchemaModelElementCollection pkElements = null;

            if (relationshipAttribute.IsForeignKeyRequired)
            {
                // get the primary keys from itself or parent class
                while (linkedClass != null)
                {
                    pkElements = linkedClass.PrimaryKeys;
                    if (pkElements.Count > 0)
                    {
                        break;
                    }

                    linkedClass = linkedClass.ParentClass;
                }

                if (pkElements != null)
                {
                    int index = 0;
                    foreach (SimpleAttributeElement pk in pkElements)
                    {
                        left = new DataSimpleAttribute(pk.Name, ReferencedDataClass.Alias);
                        left.Caption = pk.Caption;
                        left.Description = pk.Description;

                        // get an IParameter for the primary key
                        right = GetPKParameter(pk, ReferencedDataClass.Alias, parameter, index);

                        if (expectedExpr is RelationalExpr)
                        {
                            expr = new RelationalExpr(expectedExpr.ElementType, left, right);
                        }
                        else if (expectedExpr is InExpr)
                        {
                            expr = new InExpr(expectedExpr.ElementType, left, right);
                        }

                        if (expr != null)
                        {
                            if (pkFilter == null)
                            {
                                pkFilter = expr;
                            }
                            else
                            {
                                // combined with And expression
                                pkFilter = new LogicalExpr(ElementType.And, pkFilter, expr);
                            }
                        }

                        index++;
                    }
                }
            }

            pkFilter.DataView = this.DataView;

            return pkFilter;
        }
		
		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public override SchemaModelElement GetSchemaModelElement()
		{
			if (_schemaModelElement == null && DataView != null)
			{
				DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
				    ClassElement classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
				    _schemaModelElement = classElement.FindInheritedRelationshipAttribute(Name);
                }
			}

			return _schemaModelElement;
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of _ownerClassAlias member
			_ownerClassAlias = parent.GetAttribute("OwnerAlias");

			_linkedClassName = parent.GetAttribute("LinkedClass");

			_function = parent.GetAttribute("Function");

			string str = parent.GetAttribute("PKCount");
			if (str != null && str.Length > 0)
			{
				_pkCount = System.Convert.ToInt32(str);
			}

            str = parent.GetAttribute("ShowPK");
            if (!string.IsNullOrEmpty(str))
            {
                _showPrimaryKeys = System.Convert.ToBoolean(str);
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _ownerName member
			parent.SetAttribute("OwnerAlias", _ownerClassAlias);

			parent.SetAttribute("LinkedClass", _linkedClassName);

			if (_function != null && _function.Length > 0)
			{
				// Set the expression member
				parent.SetAttribute("Function", _function);
			}

			if (_pkCount > 0)
			{
				parent.SetAttribute("PKCount", _pkCount + "");
			}

            if (_showPrimaryKeys)
            {
                parent.SetAttribute("ShowPK", _showPrimaryKeys.ToString());
            }
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

        /// <summary>
        /// Get a collection of IDataViewElement objects that represents the primary keys of
        /// the referenced class.
        /// </summary>
        /// <returns>A DataViewElementCollection representing primary keys</returns>
        private DataViewElementCollection GetPrimaryKeys()
        {
            DataViewElementCollection primaryKeys = new DataViewElementCollection();
            primaryKeys.DataView = this.DataView;
            RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)GetSchemaModelElement();
            SchemaModelElementCollection pkElements = null;
            ClassElement linkedClass = relationshipAttribute.LinkedClass;
            IDataViewElement pkElement;

            if (relationshipAttribute.IsForeignKeyRequired)
            {
                // get the primary keys from itself or parent class
                while (linkedClass != null)
                {
                    pkElements = linkedClass.PrimaryKeys;
                    if (pkElements.Count > 0)
                    {
                        break;
                    }

                    linkedClass = linkedClass.ParentClass;
                }

                if (pkElements != null)
                {
                    foreach (SimpleAttributeElement pk in pkElements)
                    {
                        pkElement = new DataSimpleAttribute(pk.Name, ReferencedDataClass.Alias);
                        pkElement.Caption = pk.Caption;
                        pkElement.Description = pk.Description;

                        primaryKeys.Add(pkElement);
                    }
                }
            }

            return primaryKeys;
        }

        /// <summary>
        /// Get an IDataViewElement representing a parameter for a primary key
        /// </summary>
        /// <param name="pk">The primary key schema model element</param>
        /// <param name="classAlias">The alias of primary key owner class</param>
        /// <param name="rParameter">The IParameter for the relationship attribute</param>
        /// <returns>An IDataViewElement for the primary key</returns>
        private IDataViewElement GetPKParameter(SimpleAttributeElement pk, string classAlias,
            IParameter rParameter, int index)
        {
            IDataViewElement pkParameter = null;

            if (rParameter != null)
            {
                // The provided IParameter's value may consists of multiple primary key values
                // separated with & symbol, this method is to get an IParameter that contains
                // a single value for a primary key indicated by the index
                if (pk.IsMultipleChoice)
                {
                    pkParameter = (IDataViewElement)rParameter.GetParameterByIndex(index, pk.Name, DataType.String); 
                }
                else
                {
                    pkParameter = (IDataViewElement)rParameter.GetParameterByIndex(index, pk.Name, pk.DataType);
                }
            }

            return pkParameter;
        }

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Parent.ToXPath() + "/@" + this.Name + NewteraNameSpace.ATTRIBUTE_SUFFIX;
            }

            return _xpath;
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        public override IXaclObject Parent
        {
            get
            {
                ClassElement classElement = null;
                DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
                    classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                    if (classElement == null)
                    {
                        throw new Exception("Unable to find the class whose caption is " + ownerClass.Caption + " in the schema " + DataView.SchemaModel.SchemaInfo.Name);
                    }
                }
                else
                {
                    throw new Exception("Unable to find the class whose alias is " + _ownerClassAlias + " in a data view for class " + DataView.BaseClass.Caption);
                }

                // the parent is the owner class element
                return classElement;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			string xquery = null;

			if (_function != null  && _function.Length > 0)
			{
				// it is an aggregate function, construct xquery function
				StringBuilder query = new StringBuilder();
				query.Append(_function).Append("(");
				query.Append("$").Append(_ownerClassAlias).Append("/@").Append(Name);
				query.Append("=>").Append(_linkedClassName);
				query.Append(")");

				xquery = query.ToString();
			}
            /*
            else if (PrimaryKeyFilter != null)
            {
                // construct xquery for primary key filter
                xquery = ((IQueryElement)PrimaryKeyFilter).ToXQuery();
            }
            */

			return xquery;
		}

		#endregion
	}
}