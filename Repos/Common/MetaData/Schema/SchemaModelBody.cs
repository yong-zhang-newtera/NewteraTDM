/*
* @(#)SchemaModelBody.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Xml.Schema;
	using Newtera.Common.Core;

	/// <summary>
	/// The SchemaModelBody represents the body of the xml schema that contains
	/// xml schema element definitions for root classes.
	/// </summary>
	/// <version>  1.0.0 26 Jun 2003</version>
	/// <author>  Yong Zhang</author>
	/// <!--
	/// Here is an example of structure of a typical xml schema
	///	ComplexType definitions for classes go here
	///
	/// <xsd:element name="Demo" psd:version="1.0">
	///	<xsd:complexType>
	///		<xsd:all>
	///			<xsd:element name="ProductList" minOccurs="0">
	///			<xsd:complexType>
	///			<xsd:sequence>
	///				<xsd:element name="Product" type="Product" minOccurs="0" maxOccurs="unbounded" />
	///			</xsd:sequence>
	///			</xsd:complexType>
	///			</xsd:element>
	///			<xsd:element name="LineItemList" minOccurs="0">
	///			<xsd:complexType>
	///			<xsd:sequence>
	///				<xsd:element name="LineItem" type="LineItem" minOccurs="0" maxOccurs="unbounded" />
	///			</xsd:sequence>
	///			</xsd:complexType>
	///			</xsd:element>
	///			<xsd:element name="OrderList" minOccurs="0">
	///			<xsd:complexType>
	///			<xsd:sequence>
	///				<xsd:element name="Order" type="Order" minOccurs="0" maxOccurs="unbounded" />
	///			</xsd:sequence>
	///			</xsd:complexType>
	///			</xsd:element>
	///			<xsd:element name="CustomerList" minOccurs="0">
	///			<xsd:complexType>
	///			<xsd:sequence>
	///				<xsd:element name="Customer" type="Customer" minOccurs="0" maxOccurs="unbounded" />
	///			</xsd:sequence>
	///			</xsd:complexType>
	///			</xsd:element>
	///		</xsd:all>
	///	</xsd:complexType>
	///	<xsd:key name="ProductPK" psd:className="Product" psd:allClasses="Product,Furniture,car">
	///		<xsd:selector xpath="ProductList/Product" />
	///		<xsd:field xpath="Name" psd:type="string" />
	///		<xsd:field xpath="Supplier" psd:type="string" />
	///	</xsd:key>
	///	<xsd:key name="OrderPK" psd:className="Order" psd:allClasses="Order">
	///		<xsd:selector xpath="OrderList/Order" />
	///		<xsd:field xpath="Number" psd:type="string" />
	///	</xsd:key>
	///	<xsd:key name="CustomerPK" psd:className="Customer" psd:allClasses="Customer,Corporate,Personal">
	///		<xsd:selector xpath="CustomerList/Customer" />
	///		<xsd:field xpath="Name" psd:type="string" />
	///	</xsd:key>
	///	<xsd:unique name="PersonalCardNumberUQ" psd:className="Personal" psd:allClasses="Personal">
	///		<xsd:selector xpath="CustomerList/Customer" />
	///		<xsd:field xpath="CardNumber" psd:type="string" />
	///	</xsd:unique>
	///	<xsd:keyref name="customerOrderCustomerFK" refer="CustomerPK" psd:className="Order" psd:refClass="Customer" psd:refRootClass="Customer" psd:allClasses="Order">
	///		<xsd:selector xpath="OrderList/Order" />
	///		<xsd:field xpath="customer/Name" psd:type="string" psd:refRootClass="Customer" psd:refAttr="Name" />
	///	</xsd:keyref>
	///	<xsd:keyref name="ordersCustomerOrderFK" refer="OrderPK" psd:className="Customer" psd:refClass="Order" psd:refRootClass="Order" psd:allClasses="Customer,Corporate,Personal">
	///		<xsd:selector xpath="CustomerList/Customer" />
	///		<xsd:field xpath="orders/Number" psd:type="string" psd:refRootClass="Order" psd:refAttr="Number" />
	///	</xsd:keyref>
	///	<xsd:keyref name="productLineItemProductFK" refer="ProductPK" psd:className="LineItem" psd:refClass="Product" psd:refRootClass="Product" psd:allClasses="LineItem">
	///		<xsd:selector xpath="LineItemList/LineItem" />
	///		<xsd:field xpath="product/Name" psd:type="string" psd:refRootClass="Product" psd:refAttr="Name" />
	///		<xsd:field xpath="product/Supplier" psd:type="string" psd:refRootClass="Product" psd:refAttr="Supplier" />
	///	</xsd:keyref>
	///	<xsd:keyref name="orderLineItemOrderFK" refer="OrderPK" psd:className="LineItem" psd:refClass="Order" psd:refRootClass="Order" psd:allClasses="LineItem">
	///		<xsd:selector xpath="LineItemList/LineItem" />
	///		<xsd:field xpath="order/Number" psd:type="string" psd:refRootClass="Order" psd:refAttr="Number" />
	///	</xsd:keyref>
	///</xsd:element>
	/// -->
	internal class SchemaModelBody
	{
		private XmlSchemaElement _xmlSchemaElement;

		/// <summary>
		/// Initializing a SchemaInfoElement
		/// </summary>
		/// <param name="xmlSchemaElement">xml schema element</param>
		internal SchemaModelBody(XmlSchemaAnnotated xmlSchemaElement)
		{
			_xmlSchemaElement = (XmlSchemaElement) xmlSchemaElement;	
		}

        /// <summary>
        /// Gets the XmlSchemaElement
        /// </summary>
        /// <value>The XmlSchemaElement object</value> 
        public XmlSchemaElement XmlSchemaElement
        {
            get
            {
                return _xmlSchemaElement;
            }
        }

		/// <summary>
		/// Add the class to the body of xml schema.
		/// </summary>
		/// <param name="classElement">The class element to be added</param>
		internal void AddClassElement(ClassElement classElement)
		{
			// Create a xml schema element to represent the class
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = classElement.Name + NewteraNameSpace.LIST_SUFFIX;
			xmlSchemaElement.MinOccurs = 0;
			xmlSchemaElement.MaxOccurs = 1;
			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			xmlSchemaElement.SchemaType = complexType;
			XmlSchemaSequence sequence = new XmlSchemaSequence();
			complexType.Particle = sequence;
			XmlSchemaElement nestedXmlSchemaElement = new XmlSchemaElement();
			nestedXmlSchemaElement.Name = classElement.Name;
			nestedXmlSchemaElement.SchemaTypeName = new XmlQualifiedName(classElement.Name);
			nestedXmlSchemaElement.MaxOccursString = "unbounded";
			sequence.Items.Add(nestedXmlSchemaElement); 

			XmlSchemaAll allElement = (XmlSchemaAll) ((XmlSchemaComplexType) _xmlSchemaElement.SchemaType).Particle;			
			allElement.Items.Add(xmlSchemaElement);
		}

		/// <summary>
		/// Write the primary keys of a class as <!--<xsd:key></xsd:key>--> to schema body
		/// </summary>
		/// <param name="ownerClass">The class that owns primary keys</param>
		/// <param name="primaryKeys">The primary keys</param>
		internal void AddPrimaryKeys(ClassElement ownerClass, SchemaModelElementCollection primaryKeys)
		{
			// <xsd:key name="CustomerPK">
			XmlSchemaKey key = new XmlSchemaKey();
			key.Name = ownerClass.Name + NewteraNameSpace.PRIMARY_KEY_SUFFIX;

			// <xs:selector xpath="CustomerList/Customer"/>
			key.Selector = new XmlSchemaXPath();
			key.Selector.XPath = ownerClass.Name + NewteraNameSpace.LIST_SUFFIX + "/" + ownerClass.Name;

			foreach (SimpleAttributeElement pk in primaryKeys)
			{
				// <xs:field xpath="Name"/>
				XmlSchemaXPath field = new XmlSchemaXPath();
				field.XPath = pk.Name;
				key.Fields.Add(field);
			}

			_xmlSchemaElement.Constraints.Add(key);
		}

		/// <summary>
		/// Write the foreign keys of a class as <!--<xsd:keyref></xsd:keyref>--> to schema body
		/// </summary>
		/// <param name="attribute">The relationship attribute</param>
		/// <param name="primaryKeys">The primary keys of referenced class</param>
		internal void AddForeignKeys(RelationshipAttributeElement attribute, SchemaModelElementCollection primaryKeys)
		{
			ClassElement ownerClass = attribute.OwnerClass;

			// <xsd:keyref name="customerOrderCustomerPK" refer="CustomerPK">
			XmlSchemaKeyref keyref = new XmlSchemaKeyref();
			keyref.Name = attribute.Name + ownerClass.Name + attribute.LinkedClassName + NewteraNameSpace.KEY_REF_SUFFIX;
			keyref.Refer = new XmlQualifiedName(attribute.LinkedClassName + NewteraNameSpace.PRIMARY_KEY_SUFFIX);

			// <xs:selector xpath="OrderList/Order"/>
			keyref.Selector = new XmlSchemaXPath();
			keyref.Selector.XPath = ownerClass.Name + NewteraNameSpace.LIST_SUFFIX + "/" + ownerClass.Name;

			foreach (SimpleAttributeElement pk in primaryKeys)
			{
				// <xs:field xpath="customer/Name"/>
				XmlSchemaXPath field = new XmlSchemaXPath();
				field.XPath = attribute.Name + "/" + pk.Name;
				keyref.Fields.Add(field);
			}

			_xmlSchemaElement.Constraints.Add(keyref);
		}

		/// <summary>
		/// Write an unique constraint of a class that consists of only-one attribute
        /// as <!--<xsd:unique></xsd:unique>--> to schema body
		/// </summary>
		/// <param name="attribute">The attribute</param>
		internal void AddUniqueConstraint(SimpleAttributeElement attribute)
		{
			ClassElement ownerClass = attribute.OwnerClass;

			// <xsd:unique name="CustomerNumberUQ">
			XmlSchemaUnique uniqueElement = new XmlSchemaUnique();
			uniqueElement.Name = attribute.OwnerClass.Name + attribute.Name + NewteraNameSpace.UNIQUE_SUFFIX;

			// <xs:selector xpath="CustomerList/Customer"/>
			uniqueElement.Selector = new XmlSchemaXPath();
			uniqueElement.Selector.XPath = ownerClass.Name + NewteraNameSpace.LIST_SUFFIX + "/" + ownerClass.Name;

			// <xs:field xpath="Number"/>
			XmlSchemaXPath field = new XmlSchemaXPath();
			field.XPath = attribute.Name;
			uniqueElement.Fields.Add(field);

			_xmlSchemaElement.Constraints.Add(uniqueElement);
		}

        /// <summary>
        /// Write an unique constraint of a class that consists of multiple attributes
        /// as <!--<xsd:unique></xsd:unique>--> to schema body
        /// </summary>
        /// <param name="ownerClass">The class that are uniquely constrainted</param>
        /// <param name="uniqueKeys">The attributes that form a combined unique constraint</param>
        internal void AddUniqueConstraint(ClassElement ownerClass, SchemaModelElementCollection uniqueKeys)
        {
            // <xsd:unique name="CustomerUQ"> where Customer being the class name
            XmlSchemaUnique uniqueElement = new XmlSchemaUnique();
            uniqueElement.Name = ownerClass.Name + NewteraNameSpace.UNIQUE_SUFFIX;

            // <xs:selector xpath="CustomerList/Customer"/>
            uniqueElement.Selector = new XmlSchemaXPath();
            uniqueElement.Selector.XPath = ownerClass.Name + NewteraNameSpace.LIST_SUFFIX + "/" + ownerClass.Name;

            foreach (AttributeElementBase key in uniqueKeys)
            {
                // <xs:field xpath="Name"/>
                XmlSchemaXPath field = new XmlSchemaXPath();
                field.XPath = key.Name;
                uniqueElement.Fields.Add(field);
            }

            _xmlSchemaElement.Constraints.Add(uniqueElement);
        }

        /// <summary>
        /// Get an unique constraint element for the given class.
        /// </summary>
        /// <param name="ownerClass">The owner class</param>
        /// <returns>The xml element representing the unique constraint, or null if it desn't exist.</returns>
        internal XmlSchemaUnique GetUniqueConstraint(ClassElement ownerClass)
        {
            XmlSchemaUnique uniqueElement = null;
            foreach (XmlSchemaObject element in _xmlSchemaElement.Constraints)
            {
                if (element is XmlSchemaUnique &&
                    ((XmlSchemaUnique)element).Name == (ownerClass.Name + NewteraNameSpace.UNIQUE_SUFFIX))
                {
                    uniqueElement = (XmlSchemaUnique)element;
                    break;
                }
            }

            return uniqueElement;
        }
	}
}