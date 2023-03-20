/*
* @(#)RelationshipAttributeElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Collections;
	using System.Xml;
	using System.Xml.Schema;
	using System.ComponentModel;

	using Newtera.Common.Core;
	using System.Drawing.Design;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// RelationshipAttributeElement represents a relationship attribute in class.
	/// </summary>
	/// 
	/// <remarks>
	/// A Relationship attribute is saved in a xml schema as an attribute of the complexType
	/// with type either IDREF or IDREFS, depending on the type of relationship.
	/// For one-to-one and one-to-many relationship, an element is also created on one
	/// side of classes.
	/// </remarks>
	/// 
	/// <version>  	1.0.1 26 Jun 2003
	/// </version>
	/// <author>Yong Zhang</author>
	public class RelationshipAttributeElement : AttributeElementBase
	{
		private ClassElement _linkedClass = null;
		private string _backwardRelationshipName = null;
		private bool _isRequired = false;
		private RelationshipOwnership _ownership = RelationshipOwnership.LooselyReferenced;
		private bool _isJoinManager = false;
		private RelationshipType _type = RelationshipType.OneToOne;
		private RelationshipAttributeElement _backwardRelationship = null;
        private DefaultViewUsage _usage = DefaultViewUsage.Excluded;
        private bool _isIndexed = false;
        private bool _isOwnedRelationship = false;
        private bool _isUsedForFullTextIndex = false;
        private string _cascadedAttributeName = null;
        private string _filterExpression = null;
		
		/// <summary>
		/// Initializing a RelationshipAttributeElement object
		/// </summary>
		/// <param name="name">The name of relationship attribute</param>
		public RelationshipAttributeElement(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing a RelationshipAttributeElement object
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal RelationshipAttributeElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets the name of class referenced by the relationship
		/// </summary>
		/// <value>
		/// The name of reference class
		/// </value>
		[BrowsableAttribute(false)]		
		public string LinkedClassName
		{
			get
			{
				return _linkedClass.Name;
			}
		}

        /// <summary>
        /// Gets or sets an unique alias of class referenced by the relationship
        /// </summary>
        /// <value>
        /// The alias of reference class
        /// </value>
        [BrowsableAttribute(false)]
        public string LinkedClassAlias
        {
            get
            {
                // linked class name + relatiosnhisp name
                // TODO, this name may be duplicated when a data view with circular relationships
                return _linkedClass.Name + "_"+ this.Name;
            }
        }

		/// <summary>
		/// Gets the class element linked by the relationship
		/// </summary>
		/// <value>
		/// The linked class element.
		/// </value>
		[
			CategoryAttribute("Relationship"),
			DescriptionAttribute("The class that this relationship links to"),
			ReadOnlyAttribute(true),
			TypeConverterAttribute("Newtera.Studio.LinkedClassPropertyConverter, Studio")
		]
		public ClassElement LinkedClass
		{
			get
			{
				return _linkedClass;
			}
			set
			{
				_linkedClass = value;
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether the attribute is required
		/// </summary>
		/// <value>
		/// true if it is required, false otherwise. Default is false
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Is the value required?"),
			DefaultValueAttribute(false)
		]		
		public bool IsRequired
		{
			get
			{
		        return _isRequired;
			}
			set
			{
				_isRequired = value;

			}
		}

        /// <summary>
        /// Gets or sets information whether the attribute is indexed.
        /// </summary>
        /// <value> true if it is indexed, false, otherwise. Default is false. 
        /// </value>
        /// <remarks>Only works for the relationship when its IsForeignKeyRequired is true.</remarks>
        [
            CategoryAttribute("Index"),
            DescriptionAttribute("Is the attribute value indexed by database?"),
            DefaultValueAttribute(false)
        ]
        public bool IsIndexed
        {
            get
            {
                // can only index on the foreign key
                return _isIndexed;
            }
            set
            {
                _isIndexed = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether this relationship is used by
        /// full-text index spider when travelling to the related instances to collect the values for
        /// full-text search.
        /// </summary>
        /// <value>
        /// true if it is used for full-text index building, false otherwise. Default is false.
        /// </value>
        [
        CategoryAttribute("Index"),
        DescriptionAttribute("Is the relatiosnhip used for building full-text index?"),
        DefaultValueAttribute(false)
        ]
        public bool IsUsedForFullTextIndex
        {
            get
            {
                return _isUsedForFullTextIndex;
            }
            set
            {
                _isUsedForFullTextIndex = value;
            }

        }

        /// <summary>
        /// Gets or sets the information indicating whether the related class through this relationship is owned by this class.
        /// </summary>
        /// <value>
        /// true if it is a owned relationship, false, otherwise.
        /// </value>
        [
            CategoryAttribute("Relationship"),
            DescriptionAttribute("The related class is owned by this class"),
            DefaultValueAttribute(false)
        ]
        public bool IsOwnedRelationship
        {
            get
            {
                return _isOwnedRelationship;
            }
            set
            {
                _isOwnedRelationship = value;
                if (value)
                {
                    // only one side of the relationship can be owner
                    this.BackwardRelationship.IsOwnedRelationship = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets usage of the attribute.
        /// </summary>
        /// <value>
        /// A value of DefaultViewUsage enum values.
        /// </value>
        [
        CategoryAttribute("DefaultView"),
        DescriptionAttribute("Describe whether the relationship is used for result, search, both or none."),
        DefaultValueAttribute(DefaultViewUsage.Excluded)
        ]
        public DefaultViewUsage Usage
        {
            get
            {
                return _usage;
            }
            set
            {
                _usage = value;
            }
        }

		/// <summary>
		/// Gets or sets the ownership.
		/// </summary>
		/// <value>
		/// One of the RelationshipOwnership value. Default is owned
		/// </value>
		[
			CategoryAttribute("Relationship"),
			DescriptionAttribute("Relationship ownership, this value is applicable only if the relationship is a join manager."),
			DefaultValueAttribute(RelationshipOwnership.LooselyReferenced)
		]		
		public RelationshipOwnership Ownership
		{
			get
			{
				return _ownership;
			}
			set
			{
				_ownership = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the owner class of
		/// relationship attribute is a join manager.
		/// </summary>
		/// <value>
		/// true if it is a join manager, false, otherwise.
		/// </value>
		[
			CategoryAttribute("Relationship"),
			DescriptionAttribute("The join manager is responsible for maintaining the relationship"),
			ReadOnlyAttribute(true),
			DefaultValueAttribute(false)
		]	
		public bool IsJoinManager
		{
			get
			{
				return _isJoinManager;
			}
			set
			{
				_isJoinManager = value;
			}
		}

		/// <summary>
		/// Gets or sets the relationship type
		/// </summary>
		/// <value>
		/// One of RelationshipType Enumeration values, default is one-to-one
		/// </value>
		[
			CategoryAttribute("Relationship"),
			DescriptionAttribute("The type of relationship"),
			ReadOnlyAttribute(true),
			DefaultValueAttribute(RelationshipType.OneToOne)
		]		
		public RelationshipType Type 
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

        /// <summary>
        /// Gets or sets a expression that is used to filter the data instances in the related class via many-to-one relationship
        /// </summary>
        /// <value>
        /// A XQuery expression string, or null
        /// </value>
        [
            CategoryAttribute("Relationship"),
            DescriptionAttribute("An expression that is used to filter the data instances in the related class via many-to-one relationship"),
            DefaultValueAttribute(null)
        ]
        public string FilterExpression
        {
            get
            {
                return _filterExpression;
            }
            set
            {
                _filterExpression = value;
            }
        }

		/// <summary>
		/// Gets or sets the name of backward relationship in the linked class.
		/// </summary>
		/// <value>
		/// backward relationship
		/// </value>
		[BrowsableAttribute(false)]		
		public string BackwardRelationshipName
		{
			get
			{
				if (_backwardRelationship != null)
				{
					return _backwardRelationship.Name;
				}
				else
				{
					return _backwardRelationshipName;
				}
			}
			set
			{
				_backwardRelationshipName = value;
			}
		}

		/// <summary>
		/// The backward relationship attribute defined in the referenced class.
		/// </summary>
		/// <value>a relationship attribute</value>
		[
			CategoryAttribute("Relationship"),
			DescriptionAttribute("The backward relationship attribute at side of the linked class"),
			ReadOnlyAttribute(true),
			TypeConverterAttribute("Newtera.Studio.BackwardRelationshipPropertyConverter, Studio")
		]		
		public RelationshipAttributeElement BackwardRelationship 
		{
			get
			{
				if (_backwardRelationship == null) 
				{
					// Get the corresponded relationship in the linked class
					String backwardRelationshipName = BackwardRelationshipName;
					if (backwardRelationshipName != null) 
					{						
						// Get all attributes from the refClass.
						SchemaModelElementCollection relationships = this.LinkedClass.RelationshipAttributes;
						foreach (RelationshipAttributeElement relationship in relationships) 
						{
							if (relationship.Name == backwardRelationshipName) 
							{
								// Found
								_backwardRelationship = relationship;
								break;
							}
						}
					}
				}
        
				return _backwardRelationship;
			}
			set
			{
				_backwardRelationship = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a foreign key is required for the
		/// relationship
		/// </summary>
		/// <value>
		/// True if a foreign key is required, false otherwise.
		/// </value>
		[BrowsableAttribute(false)]		
		public bool IsForeignKeyRequired
		{
			get
			{
				if (Type == RelationshipType.ManyToOne ||
					(Type == RelationshipType.OneToOne && !IsJoinManager))
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
        /// Gets or sets names of cascaded attributes. The value of this attribute determines the
        /// list of values available for the cascade attribute.
        /// </summary>
        [
            CategoryAttribute("System"),
            BrowsableAttribute(false),
            DescriptionAttribute("The values of a cascaded attribute is determined by the value of this attribute."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.CascadedAttributePropertyEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.CascadedAttributePropertyConverter, Studio")
        ]
        public string CascadedAttributes
        {
            get
            {
                return _cascadedAttributeName;
            }
            set
            {
                _cascadedAttributeName = value;
            }
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
				// the parent is the owner class element
				return this.OwnerClass;
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

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitRelationshipAttributeElement(this);
		}
		
		/// <summary>
		/// Return the name of the RelationshipAttributeElement
		/// </summary>
		[BrowsableAttribute(false)]
		protected override string ElementName
		{
			get
			{
				return ((XmlSchemaAttribute) XmlSchemaElement).Name;
			}
		}
		
		/// <summary>
		/// Gets or sets data type of the attribute.
		/// </summary>
		/// <value>DataType.String</value>
		[BrowsableAttribute(false)]	
		public override DataType DataType
		{
			get
			{
				return DataType.String;
			}
			set
			{
			}
		}

		/// <summary>
		/// Create xml schema element as an internal representation
		/// of Schema Model element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = name;

			return xmlSchemaAttribute;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			// set the linked class member
			string linkedClassName = GetNewteraAttributeValue(NewteraNameSpace.REF_CLASS);
			this._linkedClass = SchemaModel.FindClass(linkedClassName);

			// set the backward relationship name
			_backwardRelationshipName = GetNewteraAttributeValue(NewteraNameSpace.REF_ATTR);

			// set isRequired member
			_isRequired = (xmlSchemaAttribute.Use == XmlSchemaUse.Required);

            // Set _usage member
            string usgeString = this.GetNewteraAttributeValue(NewteraNameSpace.USAGE);
            if (usgeString != null && usgeString.Length > 0)
            {
                AttributeUsage val = (AttributeUsage)Enum.Parse(typeof(AttributeUsage), usgeString);
                if (val == AttributeUsage.Both || val == AttributeUsage.Result)
                {
                    _usage = DefaultViewUsage.Included;
                }
                else
                {
                    _usage = DefaultViewUsage.Excluded;
                }
            }
            else
            {
                _usage = DefaultViewUsage.Excluded; // default value is excluded
            }

            // set the ownership member
            RelationshipOwnership ownership = RelationshipOwnership.Owned;
			string attrValue = GetNewteraAttributeValue(NewteraNameSpace.OWNERSHIP);
			if (attrValue != null)
			{
				ownership = ConvertToRelationshipOwnershipEnum(attrValue);
			}
			_ownership = ownership;

			// set isJoinManager member
			bool status = false;
			attrValue = this.GetNewteraAttributeValue(NewteraNameSpace.JOIN_MANAGER);
			if (attrValue != null)
			{
				status = (attrValue == "true" ? true : false);
			}
			_isJoinManager = status;

            // set isOwnedRelationship member
            status = false;
            attrValue = this.GetNewteraAttributeValue(NewteraNameSpace.OWNED_RELATIONSHIP);
            if (attrValue != null)
            {
                status = (attrValue == "true" ? true : false);
            }
            _isOwnedRelationship = status;

			// Set relationshipType member
			RelationshipType relationshipType = RelationshipType.OneToOne;
			attrValue = GetNewteraAttributeValue(NewteraNameSpace.REF_TYPE);
			if (attrValue != null)
			{
				relationshipType = ConvertToRelationshipTypeEnum(attrValue);
			}
			_type = relationshipType;

            // Set _isIndexed member
            attrValue = this.GetNewteraAttributeValue(NewteraNameSpace.INDEX);
            _isIndexed = (attrValue != null && attrValue == "true" ? true : false);

            // Set _isUsedForFullTextIndex member
            attrValue = this.GetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT);
            _isUsedForFullTextIndex = (attrValue != null && attrValue == "true" ? true : false);

            // Set _cascadeAttribute member
            _cascadedAttributeName = this.GetNewteraAttributeValue(NewteraNameSpace.CASCADE_ATTRIBUTE);

            // set the _filterExpression
            _filterExpression = GetNewteraAttributeValue(NewteraNameSpace.FILTER);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <!--
		/// The example of relationship between "Orders" and "Customer" in xml schema
		///	<xsd:complexType name="Order" psd:displayName="Order" psd:order="2" psd:id="1458">
		///	<xsd:sequence>
		///		<xsd:element name="Number" type="OrderNumber" minOccurs="1" maxOccurs="1" psd:displayName="Number" psd:order="0" psd:key="true" psd:id="8474" />
		///		<xsd:element name="OrderDate" type="xsd:date" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="OrderDate" psd:order="1" psd:id="8475" />
		///		<xsd:element name="TotalPrice" type="xsd:double" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="TotalPrice" psd:order="2" psd:id="8476" />
		///		<xsd:element name="customer" minOccurs="0" psd:keyref="true" maxOccurs="1">
		///			<xsd:complexType>
		///			<xsd:sequence>
		///				<xsd:element name="FirstName" psd:type="string" type="xsd:string" />
		///				<xsd:element name="LastName" psd:type="string" type="xsd:string" />
		///			</xsd:sequence>
		///			</xsd:complexType>
		///		</xsd:element>
		///	</xsd:sequence>
		///	<xsd:attribute name="customer" psd:refType="manyToOne" type="xsd:IDREF" psd:refClass="Customer" psd:displayName="customer" psd:order="4" psd:refAttr="orders" psd:ownership="looseReferenced" psd:id="8478" />
		///	<xsd:attribute name="obj_id" type="xsd:ID" />
		///	</xsd:complexType>
		///	
		///	<xsd:complexType name="Customer" psd:displayName="Customer" psd:order="3" psd:id="1459">
		///	<xsd:sequence>
		///		<xsd:element name="FisrtName" type="xsd:string" minOccurs="1" maxOccurs="1" psd:displayName="Name" psd:order="0" psd:key="true" psd:id="8479" />
		///		<xsd:element name="LastName" type="xsd:string" minOccurs="1" maxOccurs="1" psd:displayName="Name" psd:order="0" psd:key="true" psd:id="8479" />
		///		<xsd:element name="Address" type="xsd:string" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="Address" psd:order="1" psd:id="8480" />
		///		<xsd:element name="City" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="City" psd:order="2" psd:id="8481"/>
		///		<xsd:element name="State" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="State" psd:order="3" psd:id="8482"/>
		///	</xsd:sequence>
		///	<xsd:attribute name="orders" psd:refType="oneToMany" type="xsd:IDREFS" psd:refClass="Order" psd:displayName="orders" psd:order="4" psd:refAttr="customer" psd:joinManager="true" psd:ownership="looseReferenced" minOccurs="0" maxOccurs="1" psd:id="8483" />
		///	<xsd:attribute name="obj_id" type="xsd:ID" />
		///	</xsd:complexType>
		///	
		///	<xsd:element name="Demo" psd:version="1.0" psd:mappingMethod="vertical" psd:id="261">
		///	<xsd:complexType>
		///		<xsd:all>
		///			<xsd:element name="OrderList" minOccurs="0">
		///				<xsd:complexType>
		///				<xsd:sequence>
		///					<xsd:element name="Order" type="Order" minOccurs="0" maxOccurs="unbounded" />
		///				</xsd:sequence>
		///				</xsd:complexType>
		///			</xsd:element>
		///			<xsd:element name="CustomerList" minOccurs="0">
		///				<xsd:complexType>
		///				<xsd:sequence>
		///					<xsd:element name="Customer" type="Customer" minOccurs="0" maxOccurs="unbounded" />
		///				</xsd:sequence>
		///				</xsd:complexType>
		///			</xsd:element>
		///		</xsd:all>
		///	</xsd:complexType>
		///	<xsd:key name="CustomerPK" psd:className="Customer" psd:allClasses="Customer,Corporate,Personal">
		///		<xsd:selector xpath="CustomerList/Customer" />
		///		<xsd:field xpath="FirstName" psd:type="string" />
		///		<xsd:field xpath="LastName" psd:type="string" />			
		///	</xsd:key>
		///	<xsd:keyref name="OrdercustomerCustomerFK" refer="CustomerPK" psd:className="Order" psd:refClass="Customer" psd:refRootClass="Customer" psd:allClasses="Order">
		///		<xsd:selector xpath="OrderList/Order" />
		///		<xsd:field xpath="customer/FirstName" psd:type="string" psd:refRootClass="Customer" psd:refAttr="Name" />
		///		<xsd:field xpath="customer/LastName" psd:type="string" psd:refRootClass="Customer" psd:refAttr="Name" />
		///	</xsd:keyref>
		/// </xsd:element>
		/// -->
		internal override void Marshal()
		{
			XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute) XmlSchemaElement;

			// Write _referencedClassName member
			SetNewteraAttributeValue(NewteraNameSpace.REF_CLASS, LinkedClassName);

			// Write _backwardRelationshipName member
			SetNewteraAttributeValue(NewteraNameSpace.REF_ATTR, BackwardRelationshipName);

			// Write isRequired member
			if (_isRequired)
			{
				xmlSchemaAttribute.Use = XmlSchemaUse.Required;
			}

            // Write _usage member
            if (_usage == DefaultViewUsage.Included)
            {
                SetNewteraAttributeValue(NewteraNameSpace.USAGE, Enum.GetName(typeof(AttributeUsage), AttributeUsage.Result));
            }

            // write ownership member
            string attrValue = ConvertToRelationshipOwnershipString(_ownership);
			
			SetNewteraAttributeValue(NewteraNameSpace.OWNERSHIP, attrValue);

			// write isJoinManager member
			if (_isJoinManager)
			{
				SetNewteraAttributeValue(NewteraNameSpace.JOIN_MANAGER, "true");
			}
			else
			{
				SetNewteraAttributeValue(NewteraNameSpace.JOIN_MANAGER, "false");
			}

            // write isOwnedRelationship member
            if (_isOwnedRelationship)
            {
                SetNewteraAttributeValue(NewteraNameSpace.OWNED_RELATIONSHIP, "true");
            }
            else
            {
                SetNewteraAttributeValue(NewteraNameSpace.OWNED_RELATIONSHIP, "false");
            }

			// Write Type member
			switch (_type)
			{
				case RelationshipType.OneToOne:
					attrValue = NewteraNameSpace.ONE_TO_ONE;
					xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName("IDREF", "http://www.w3.org/2003/XMLSchema");
					break;
				case RelationshipType.OneToMany:
					attrValue = NewteraNameSpace.ONE_TO_MANY;
					xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName("IDREFS", "http://www.w3.org/2003/XMLSchema");
					break;
				case RelationshipType.ManyToOne:
					attrValue = NewteraNameSpace.MANY_TO_ONE;
					xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName("IDREF", "http://www.w3.org/2003/XMLSchema");
					break;
			}
			SetNewteraAttributeValue(NewteraNameSpace.REF_TYPE, attrValue);

            // Write IsIndexed member
            if (_isIndexed)
            {
                SetNewteraAttributeValue(NewteraNameSpace.INDEX, "true");
            }

            // Write IsUsedForFullTextIndex member
            if (_isUsedForFullTextIndex)
            {
                SetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT, "true");
            }

            // Write _cascadeAttribute member
            if (!string.IsNullOrEmpty(_cascadedAttributeName))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CASCADE_ATTRIBUTE, _cascadedAttributeName);
            }

            // Write _filterExpression member
            if (!string.IsNullOrEmpty(_filterExpression))
            {
                SetNewteraAttributeValue(NewteraNameSpace.FILTER, _filterExpression);
            }

			/*
			 * If relationshipType is ManyToOne, or relationshipType is OneToOne and
			 * isJoinManager is true, create an element of complexType as a foreign key
			 * and a keyref in the schema body (see example of customer relationship in
			 * Order class)
			 */
			if (IsForeignKeyRequired)
			{
				WriteForeignKey();
			}

			base.Marshal();
		}

		/// <summary>
		/// Convert a string to one of RelationshipType values.
		/// </summary>
		/// <param name="typeStr">The string</param>
		/// <returns>One of RelationshipType values</returns>
		private RelationshipType ConvertToRelationshipTypeEnum(string typeStr)
		{
			RelationshipType type;

			switch (typeStr)
			{
				case NewteraNameSpace.ONE_TO_ONE:
					type = RelationshipType.OneToOne;
					break;
				case NewteraNameSpace.ONE_TO_MANY:
					type = RelationshipType.OneToMany;
					break;
				case NewteraNameSpace.MANY_TO_ONE:
					type = RelationshipType.ManyToOne;
					break;
				default:
					type = RelationshipType.OneToOne;
					break;
			}

			return type;
		}

		/// <summary>
		/// Convert one of RelationshipType values to its string representation.
		/// </summary>
		/// <param name="type">one of RelationshipType values</param>
		/// <returns>String representation</returns>
		private string ConvertToRelationshipTypeString(RelationshipType type)
		{
			string typeStr = NewteraNameSpace.ONE_TO_ONE;

			switch (type)
			{
				case RelationshipType.OneToOne:
					typeStr = NewteraNameSpace.ONE_TO_ONE;
					break;
				case RelationshipType.OneToMany:
					typeStr = NewteraNameSpace.ONE_TO_MANY;
					break;
				case RelationshipType.ManyToOne:
					typeStr = NewteraNameSpace.MANY_TO_ONE;
					break;
			}

			return typeStr;
		}

		/// <summary>
		/// Convert a string to one of RelationshipOwnership values.
		/// </summary>
		/// <param name="str">The string</param>
		/// <returns>One of RelationshipOwnership values</returns>
		private RelationshipOwnership ConvertToRelationshipOwnershipEnum(string str)
		{
			RelationshipOwnership ownership;

			switch (str)
			{
				case NewteraNameSpace.LOOSE_REFERENCED:
					ownership = RelationshipOwnership.LooselyReferenced;
					break;
				case NewteraNameSpace.OWNED:
					ownership = RelationshipOwnership.Owned;
					break;
				case NewteraNameSpace.TIGHT_REFERENCED:
					ownership = RelationshipOwnership.TightlyReferenced;
					break;
				default:
					ownership = RelationshipOwnership.LooselyReferenced;
					break;
			}

			return ownership;
		}

		/// <summary>
		/// Convert one of RelationshipOwnership values to its string representation.
		/// </summary>
		/// <param name="ownership">one of RelationshipOwnership values</param>
		/// <returns>String representation</returns>
		private string ConvertToRelationshipOwnershipString(RelationshipOwnership ownership)
		{
			string str = NewteraNameSpace.LOOSE_REFERENCED;

			switch (ownership)
			{
				case RelationshipOwnership.LooselyReferenced:
					str = NewteraNameSpace.LOOSE_REFERENCED;
					break;
				case RelationshipOwnership.Owned:
					str = NewteraNameSpace.OWNED;
					break;
				case RelationshipOwnership.TightlyReferenced:
					str = NewteraNameSpace.TIGHT_REFERENCED;
					break;
			}

			return str;
		}

		/// <summary>
		/// Create an element of complexType as a foreign key
		///	and a keyref in the schema body.
		///	(see example of customer relationship in Order class)
		/// </summary>
		private void WriteForeignKey()
		{
			// Create an element of complexType as a foreign key in the class
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = Name;
			if (_isRequired)
			{
				xmlSchemaElement.MinOccursString = "1";
			}
			else
			{
				xmlSchemaElement.MinOccursString = "0";
			}

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			XmlSchemaSequence sequence = new XmlSchemaSequence();
			complexType.Particle = sequence;
			xmlSchemaElement.SchemaType = complexType;

			// write primary keys of referenced class as nested elements
			ClassElement linkedClass = SchemaModel.FindClass(LinkedClassName);
			if (linkedClass == null)
			{
				throw new InvalidClassNameException("Unable to find the linked class with name " + LinkedClassName);
			}

			foreach (SimpleAttributeElement pk in linkedClass.PrimaryKeys)
			{
				XmlSchemaElement pkElement = new XmlSchemaElement();
				pkElement.Name = pk.Name;
				pkElement.SchemaTypeName = new XmlQualifiedName(DataTypeConverter.ConvertToTypeString(pk.DataType), "http://www.w3.org/2003/XMLSchema");
				sequence.Items.Add(pkElement);
			}

			// Add a keyref constraint to the schema body for the foreign key
			SchemaModel.SchemaBody.AddForeignKeys(this, linkedClass.PrimaryKeys);
		}
	}

	/// <summary>
	/// Describes the options for relationship ownership
	/// </summary>
	public enum RelationshipOwnership
	{
		/// <summary>
		/// LooselyReferenced, does not cause cascade deletion
		/// </summary>
		LooselyReferenced,
		/// <summary>
		/// Owned, cause cascade deletion
		/// </summary>
		Owned,
		/// <summary>
		/// TightlyReferenced, prevent from being deleted
		/// </summary>
		TightlyReferenced
	}

	/// <summary>
	/// Describes the types for relationship
	/// </summary>
	public enum RelationshipType
	{
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,
		/// <summary>
		/// OneToOne
		/// </summary>
		OneToOne,
		/// <summary>
		/// OneToMany
		/// </summary>
		OneToMany,
		/// <summary>
		/// ManyToOne
		/// </summary>
		ManyToOne,
	}
}