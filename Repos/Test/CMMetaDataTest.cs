/*
* @(#)CMMetaDataTest.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Test
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Data;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Data;
	using Newtera.Data.DB;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Test program of CM.
	/// </summary>
	/// <version>  	1.0.0 19 Oct 2003 </version>
	/// <author>  Yong Zhang </author>
	public class CMMetaDataTest
	{
		public const string ConnectionString = "";

		public static void Main()
		{
			//TestGetAllSchemas();

			MetaDataModel metaData;

			//metaData = TestCreateMetaData();

			metaData = TestGetMetaDataModel("UnitTest", "1.0");

			TestAlterMetaData(metaData);

			//TestDeleteMetaData();

			//TestAuthenticate();

			//TestWriteClob();

			return;
		}

		static private void TestGetAllSchemas()
		{
			CMConnection con = new CMConnection();
			IList schemas = con.AllSchemas;

			foreach (SchemaInfo schemaInfo in schemas)
			{
				System.Console.WriteLine("Schema Name = " + schemaInfo.Name + "; Schema Version = " + schemaInfo.Version);
			}
		}

		static private MetaDataModel TestGetMetaDataModel(string name, string version)
		{
			CMConnection con = new CMConnection("SCHEMA_NAME=" + name + ";SCHEMA_VERSION=" +version);

			try
			{
				con.Open();

				MetaDataModel metaData = con.MetaDataModel;

				StringBuilder builder = new StringBuilder();
				TextWriter writer = new StringWriter(builder);
				metaData.SchemaModel.Write(writer);

				string xmlSchema = builder.ToString();
				System.Console.WriteLine("TestGetMetaDataModel has result :" + xmlSchema);

				MetaDataModel mmCopy = new MetaDataModel(metaData.SchemaInfo);

				StringReader reader = new StringReader(xmlSchema);

				mmCopy.SchemaModel.Read(reader);

				/*
				builder = new StringBuilder();
				writer = new StringWriter(builder);
				metaData.DataViews.Write(writer);
				xmlSchema = builder.ToString();

				builder = new StringBuilder();
				writer = new StringWriter(builder);
				metaData.XaclPolicy.Write(writer);
				xmlSchema = builder.ToString();
				*/				

				return mmCopy;
			}
			finally
			{
				con.Close();
			}
		}

		static private MetaDataModel TestCreateMetaData()
		{
			CMConnection con = null;
			MetaDataModel metaData = null;

			try
			{
				con = new CMConnection("SCHEMA_NAME=UnitTest;SCHEMA_VERSION=1.0");
				con.Open();

				metaData = new MetaDataModel(con.SchemaInfo);
				
				// add a enum constraint
				EnumElement sexEnum = new EnumElement("SexEnum");
				sexEnum.DataType = DataType.String;
				sexEnum.AddValue("Male");
				sexEnum.AddValue("Female");
				metaData.SchemaModel.AddEnumConstraint(sexEnum);

				// Add a "Customer" class
				ClassElement customerCls = metaData.SchemaModel.CreateClass("Customer");
				SimpleAttributeElement attribute = new SimpleAttributeElement("Name");
				attribute.DataType = DataType.String;
				attribute.IsUnique = true;
				customerCls.AddSimpleAttribute(attribute);
				customerCls.AddPrimaryKey(attribute);
				attribute = new SimpleAttributeElement("Address");
				attribute.DataType = DataType.String;
				attribute.MaxLength = 300;
				customerCls.AddSimpleAttribute(attribute);
				attribute = new SimpleAttributeElement("City");
				attribute.DataType = DataType.String;
				attribute.MaxLength = 50;
				customerCls.AddSimpleAttribute(attribute);

				metaData.SchemaModel.AddRootClass(customerCls);

				// Add a "Domain" class
				ClassElement domainCls = metaData.SchemaModel.CreateClass("Domain");
				attribute = new SimpleAttributeElement("Code");
				attribute.DataType = DataType.String;
				domainCls.AddSimpleAttribute(attribute);
				domainCls.AddPrimaryKey(attribute);

				metaData.SchemaModel.AddRootClass(domainCls);

				// Add a subclass
				ClassElement personalCls = metaData.SchemaModel.CreateClass("Personal");
				attribute = new SimpleAttributeElement("Credit");
				attribute.DataType = DataType.Decimal;
				personalCls.AddSimpleAttribute(attribute);
				attribute = new SimpleAttributeElement("Sex");
				attribute.Constraint = sexEnum;
				personalCls.AddSimpleAttribute(attribute);

				customerCls.AddSubclass(personalCls);

				ClassElement corporateCls = metaData.SchemaModel.CreateClass("Corporate");
				attribute = new SimpleAttributeElement("Revenue");
				attribute.DataType = DataType.String;
				corporateCls.AddSimpleAttribute(attribute);

				customerCls.AddSubclass(corporateCls);

				// Add a root class
				ClassElement orderCls = metaData.SchemaModel.CreateClass("Order");
				attribute = new SimpleAttributeElement("ID");
				attribute.DataType = DataType.Integer;
				attribute.IsIndexed = true;
				attribute.IsAutoIncrement = true;
				orderCls.AddSimpleAttribute(attribute);

				attribute = new SimpleAttributeElement("Number");
				attribute.DataType = DataType.String;
				attribute.IsRequired = true;
				attribute.IsUnique = true;
				attribute.IsIndexed = true;
				orderCls.AddSimpleAttribute(attribute);
				metaData.SchemaModel.AddRootClass(orderCls);

				// Add a root class Product with primary keys "Number" and "SKU"
				ClassElement productCls = metaData.SchemaModel.CreateClass("Product");
				attribute = new SimpleAttributeElement("Number");
				attribute.DataType = DataType.String;
				productCls.AddSimpleAttribute(attribute);
				productCls.AddPrimaryKey(attribute);

				attribute = new SimpleAttributeElement("SKU");
				attribute.DataType = DataType.String;
				productCls.AddSimpleAttribute(attribute);
				productCls.AddPrimaryKey(attribute);

				attribute = new SimpleAttributeElement("Description");
				attribute.DataType = DataType.String;
				attribute.MaxLength = 2000;
				attribute.IsFullTextSearchable = true;
				productCls.AddSimpleAttribute(attribute);

				metaData.SchemaModel.AddRootClass(productCls);

				// Add a subclass "Computer" to "Product"
				ClassElement computerCls = metaData.SchemaModel.CreateClass("Computer");
				attribute = new SimpleAttributeElement("CPU");
				attribute.DataType = DataType.Integer;
				computerCls.AddSimpleAttribute(attribute);

				productCls.AddSubclass(computerCls);

				// Add two subclasses "Desktop" and "Laptop" to "Computer" class
				ClassElement desktopCls = metaData.SchemaModel.CreateClass("Desktop");
				attribute = new SimpleAttributeElement("IsTower");
				attribute.DataType = DataType.Boolean;
				desktopCls.AddSimpleAttribute(attribute);
				computerCls.AddSubclass(desktopCls);

				ClassElement laptopCls = metaData.SchemaModel.CreateClass("Laptop");
				attribute = new SimpleAttributeElement("Weight");
				attribute.DataType = DataType.Float;
				laptopCls.AddSimpleAttribute(attribute);
				computerCls.AddSubclass(laptopCls);

				// Add a Many-to-one relationship to Order class
				RelationshipAttributeElement relationship = new RelationshipAttributeElement("customer");
				relationship.Type = RelationshipType.ManyToOne;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "orders";
				relationship.LinkedClass = customerCls;

				orderCls.AddRelationshipAttribute(relationship);

				// Add a One-To-Many relationship to Customer class
				relationship = new RelationshipAttributeElement("orders");
				relationship.Type = RelationshipType.OneToMany;
				relationship.BackwardRelationshipName = "customer";
				relationship.LinkedClass = orderCls;

				customerCls.AddRelationshipAttribute(relationship);

				// Add a Many-to-one relationship between "Customer" and "Domain"
				relationship = new RelationshipAttributeElement("domain");
				relationship.Type = RelationshipType.ManyToOne;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "customers";
				relationship.LinkedClass = domainCls;
				customerCls.AddRelationshipAttribute(relationship);

				relationship = new RelationshipAttributeElement("customers");
				relationship.Type = RelationshipType.OneToMany;
				relationship.BackwardRelationshipName = "domain";
				relationship.LinkedClass = customerCls;
				domainCls.AddRelationshipAttribute(relationship);

				// Add a Many-to-one relationship between "Product" and "Domain"
				relationship = new RelationshipAttributeElement("domain");
				relationship.Type = RelationshipType.ManyToOne;
				relationship.Ownership = RelationshipOwnership.Owned;
				relationship.BackwardRelationshipName = "products";
				relationship.LinkedClass = domainCls;
				productCls.AddRelationshipAttribute(relationship);

				relationship = new RelationshipAttributeElement("products");
				relationship.Type = RelationshipType.OneToMany;
				relationship.BackwardRelationshipName = "domain";
				relationship.LinkedClass = productCls;
				domainCls.AddRelationshipAttribute(relationship);

				// Add a Many-to-many relationship between Order or Product
				relationship = new RelationshipAttributeElement("products");
				relationship.Type = RelationshipType.ManyToMany;
				relationship.IsJoinManager = true;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "orders";
				relationship.LinkedClass = productCls;

				orderCls.AddRelationshipAttribute(relationship);

				relationship = new RelationshipAttributeElement("orders");
				relationship.Type = RelationshipType.ManyToMany;
				relationship.BackwardRelationshipName = "products";
				relationship.LinkedClass = orderCls;

				productCls.AddRelationshipAttribute(relationship);

				StringBuilder builder = new StringBuilder();
				StringWriter writer = new StringWriter(builder);
		
				ValidateResult result = metaData.SchemaModel.Validate();
				if (!result.HasError)
				{
					metaData.SchemaModel.Write(writer);
			
					string xmlSchema = builder.ToString();
					System.Console.WriteLine("The new schema is :");
					System.Console.WriteLine(xmlSchema);

					// create the schema in database
					con.UpdateMetaData(MetaDataType.Schema, xmlSchema);

					string log = con.MetaDataUpdateLog;

					System.Console.WriteLine(log);
					System.Console.WriteLine("");

					// get the new meta data from the database
					metaData = con.MetaDataModel;
				}
				else
				{
					System.Console.WriteLine("Invalid schema");
				}
			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				if (con != null)
				{
					con.Close();
				}
			}

			return metaData;
		}

		static private void TestAlterMetaData(MetaDataModel metaData)
		{
			CMConnection con = null;

			try
			{
				con = new CMConnection("SCHEMA_NAME=UnitTest;SCHEMA_VERSION=1.0");
				con.Open();
				SchemaModel schemaModel = metaData.SchemaModel;
				SimpleAttributeElement attribute;

				// Add a "Age" simple attribute to "Personal" class
				/* passed
				ClassElement personalCls = schemaModel.FindClass("Personal");
				attribute = new SimpleAttributeElement("Age");
				attribute.DataType = DataType.Integer;
				attribute.IsRequired = true;
				attribute.DefaultValue = "20";
				personalCls.AddSimpleAttribute(attribute);
				*/

				// delete the "City" attribute from "Customer" class
				/* passed
				ClassElement customerCls = schemaModel.FindClass("Customer");
				customerCls.RemoveSimpleAttribute("City");
				*/

				// add a class "Supplier"
				/* passed
				ClassElement supplierCls = schemaModel.CreateClass("Supplier");
				attribute = new SimpleAttributeElement("Name");
				attribute.DataType = DataType.String;
				attribute.MaxLength = 150;
				supplierCls.AddSimpleAttribute(attribute);
				supplierCls.AddPrimaryKey(attribute);
				schemaModel.AddRootClass(supplierCls);
				*/

				// delete the class "Laptop"
				/* passed
				ClassElement computerCls = schemaModel.FindClass("Computer");
				ClassElement laptopCls = schemaModel.FindClass("Laptop");
				computerCls.RemoveSubclass(laptopCls);
				*/

				// Change the "Corporate" class from subclass to a Root class
				/* passed
				ClassElement corporateCls = schemaModel.FindClass("Corporate");
				ClassElement customerCls = schemaModel.FindClass("Customer");
				customerCls.RemoveSubclass(corporateCls);
				corporateCls.ParentClass = null;
				schemaModel.AddRootClass(corporateCls);
				*/

				// Change the primary keys of "Product" class
				/* passed
				ClassElement productCls = schemaModel.FindClass("Product");
				attribute = productCls.FindSimpleAttribute("SKU");
				productCls.RemovePrimaryKey(attribute);
				*/

				// Change the data type of "ID" attribute of the "Order" class
				/* error: unable to change data type because of existance of an index
				ClassElement orderCls = schemaModel.FindClass("Order");
				attribute = orderCls.FindSimpleAttribute("ID");
				attribute.DataType = DataType.String; // from Integer to String
				attribute.IsAutoIncrement = false;
				attribute.IsUnique = true;
				attribute.IsIndexed = false;
				*/

				// increase the max length of "Number" attribute in "Product"
				/* passed
				ClassElement productCls = schemaModel.FindClass("Product");
				attribute = productCls.FindSimpleAttribute("Number");
				attribute.MaxLength = 250;
				*/

				// add a new many-to-one relationship between "Product" and "Supplier"
				/* passed
				ClassElement supplierCls = schemaModel.FindClass("Supplier");
				ClassElement productCls = schemaModel.FindClass("Product");
				RelationshipAttributeElement relationship = new RelationshipAttributeElement("products");
				relationship.Type = RelationshipType.OneToMany;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "supplier";
				relationship.LinkedClass = productCls;
				supplierCls.AddRelationshipAttribute(relationship);

				relationship = new RelationshipAttributeElement("supplier");
				relationship.Type = RelationshipType.ManyToOne;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "products";
				relationship.LinkedClass = supplierCls;
				productCls.AddRelationshipAttribute(relationship);
				*/

				// Change a Many-to-one relationship between "Order" to "Customer"
				// to many-to-one relationship between "Product" to "Customer"

				// step one, remove the obsolete relationship
				/* passed
				ClassElement orderCls = schemaModel.FindClass("Order");
				RelationshipAttributeElement relationship = orderCls.FindRelationshipAttribute("customer");
				orderCls.RemoveRelationshipAttribute(relationship);
				*/

				// step two, add a new relationship to "Product" class
				/* passed
				ClassElement productCls = schemaModel.FindClass("Product");
				ClassElement customerCls = schemaModel.FindClass("Customer");
				relationship = new RelationshipAttributeElement("orderBy");
				relationship.Type = RelationshipType.ManyToOne;
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				relationship.BackwardRelationshipName = "orders";
				relationship.LinkedClass = customerCls;
				productCls.AddRelationshipAttribute(relationship);
				*/

				// Step three: Modify the relationship on "Customer" class
				/* passed
				relationship = customerCls.FindRelationshipAttribute("orders");
				relationship.BackwardRelationshipName = "orderBy";
				relationship.LinkedClass = productCls;
				*/
				// end

				// Change type of relationship between "Order" and "Product"
				// from Many-to-many to one-to-one
				/* passed
				ClassElement orderCls = schemaModel.FindClass("Order");
				ClassElement productCls = schemaModel.FindClass("Product");
				RelationshipAttributeElement relationship = orderCls.FindRelationshipAttribute("products");
				relationship.Type = RelationshipType.OneToOne;
				relationship.IsJoinManager = true;

				relationship = productCls.FindRelationshipAttribute("orders");
				relationship.Type = RelationshipType.OneToOne;
				*/
				// end

				// change the type of relationship between "Customer" and "Domain"
				/* passed
				ClassElement customerCls = schemaModel.FindClass("Customer");
				RelationshipAttributeElement relationship = customerCls.FindRelationshipAttribute("domain");
				relationship.Type = RelationshipType.OneToMany;

				ClassElement domainCls = schemaModel.FindClass("Domain");
				relationship = domainCls.FindRelationshipAttribute("customers");
				relationship.Type = RelationshipType.ManyToOne;
				*/

				// Change the ownership of relationship between "Product" and "Domain"
				/* passed
				ClassElement productCls = schemaModel.FindClass("Product");
				RelationshipAttributeElement relationship = productCls.FindRelationshipAttribute("domain");
				relationship.Ownership = RelationshipOwnership.LooselyReferenced;
				*/

				StringBuilder builder = new StringBuilder();
				StringWriter writer = new StringWriter(builder);
		
				metaData.SchemaModel.Write(writer);
		
				string xmlSchema = builder.ToString();
				System.Console.WriteLine("The new schema is :");
				System.Console.WriteLine(xmlSchema);

				// alter the schema in database
				con.UpdateMetaData(MetaDataType.Schema, xmlSchema);
			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				if (con != null)
				{
					con.Close();
				}
			}
		}

		static private void TestDeleteMetaData()
		{
			CMConnection con = null;

			try
			{
				con = new CMConnection("SCHEMA_NAME=UnitTest;SCHEMA_VERSION=1.0");
				con.Open();

				// delete the schema in database
				con.DeleteMetaData();

			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				if (con != null)
				{
					con.Close();
				}
			}
		}

		static private void TestWriteClob()
		{
			CMConnection con = new CMConnection("SCHEMA_NAME=UnitTest;SCHEMA_VERSION=1.0");

			try
			{
				con.Open();
				IClobDAO clobDAO = ClobDAOFactory.Instance.Create(con.DataProvider);
				FileStream fs = File.OpenRead("test.schema");

				using (clobDAO)
				{
					clobDAO.WriteClob(fs, "MM_SCHEMA", "XML_SCHEMA", "UNITTEST", "1.0");
				}
			}
			finally
			{
				con.Close();
			}
		}

		static private void TestAuthenticate()
		{			
			IUserManager userManager = new CMUserManager();
			
			bool isAuthenticated = userManager.Authenticate("admin", "admin");
		}
	}
}