/*
* @(#)AddClassAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Add a new class to the database. It is responsible to perform individual task, such as add simple or relationship attributes, etc.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddClassAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddClassAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		/// <param name="element">The class element</param>
		/// <param name="dataProvider">The data provider</param>
		public AddClassAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider) : base(metaDataModel, element, dataProvider)
		{
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.AddClass;
			}
		}

		/// <summary>
		/// Prepare the action for adding a class to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans)
		{
			ClassElement classElement = (ClassElement) SchemaModelElement;

			// assign an unique id and table name to the class
			KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ClassId, MetaDataModel.SchemaInfo);
			classElement.ID = generator.NextKey().ToString();
			classElement.TableName = DBNameComposer.GetTableName(classElement);

			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = trans;
			
			// add a record in mm_class for an added class
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddClassDML");
			DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
			sql = dmlGenerator.GetAddClassDML(sql, classElement.ID,
				classElement.Name.ToUpper(), classElement.Caption,
				classElement.TableName, classElement.SchemaModel.SchemaInfo.ID,
				DBNameComposer.GetUniqueKeyName(classElement, null));
			
			cmd.CommandText = sql;

			if (_log != null)
			{
				_log.Append(sql, LogType.DML);
			}

			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of adding a class to database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		/// <remarks>
		/// It creates DDLs for:
		/// 1. an OID column as the PK of each table.
		/// 2. A CID column for storing bottom class id
		/// 3. A ANUM column for storing number of attachments to an object
		/// 4. Columns for simple attributes.
		/// 5. FK column for certain relationship attributes.
		/// NOTES:  FK constraint is not created here, it must be created after
		/// the creation of all the tables. They are taken care by AddFKConstraintAction object.
		/// </remarks>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			ClassElement classElement = (ClassElement) SchemaModelElement;

			// Create a DDL for creating a table for this class
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			StringBuilder builder = new StringBuilder();

			builder.Append(generator.GetAddTableHeaderDDL(classElement.TableName)).Append("\n");

			builder.Append(generator.GetAddColumnDDL(classElement.ID, "OID", DataType.BigInteger, true)).Append(",\n");
			builder.Append(generator.GetAddColumnDDL(classElement.ID, "CID", DataType.BigInteger)).Append(",\n");
			builder.Append(generator.GetAddColumnDDL(classElement.ID, "ANUM", DataType.Integer, false, "0")).Append(",\n");

			// Add columns for simple attributes
			foreach (SimpleAttributeElement attribute in classElement.SimpleAttributes)
			{
				builder.Append(generator.GetAddColumnDDL(attribute)).Append(",\n");
			}

			// Add columns for array attributes
			foreach (ArrayAttributeElement attribute in classElement.ArrayAttributes)
			{
				builder.Append(generator.GetAddColumnDDL(attribute)).Append(",\n");
			}

            // Virtual attributes do not have columns in database

            // Add columns for image attributes
            foreach (ImageAttributeElement attribute in classElement.ImageAttributes)
            {
                builder.Append(generator.GetAddColumnDDL(attribute)).Append(",\n");
            }

			// Add foreign key column when relationship is many-to-one or
			// when the relationship is one-to-one and is not join manager of this
			// relationship.
			SchemaModelElementCollection foreignKeys = new SchemaModelElementCollection();

			foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
			{
				if (relationship.IsForeignKeyRequired)
				{
					foreignKeys.Add(relationship);
				}
			}

			foreach (RelationshipAttributeElement fk in foreignKeys)
			{
				builder.Append(generator.GetAddFKColumnDDL(fk)).Append(",\n");
			}

			// get ride of the extra ",\n" at the end
			builder.Remove(builder.Length - 2, 2);

			builder.Append(generator.GetAddTableFooterDDL()).Append("\n");

			string ddl = builder.ToString();
			cmd.CommandText = ddl;

			if (_log != null)
			{
				_log.Append(ddl, LogType.DDL);
			}

			cmd.ExecuteNonQuery();

			// Create primary key constraint for all primary keys
			if (classElement.PrimaryKeys.Count > 0)
			{
				builder = new StringBuilder();

				string constraintName = DBNameComposer.GetUniqueKeyName(classElement, null);
				builder.Append(generator.GetAddUniqueConstraintHeaderDDL(classElement.TableName, constraintName));

				int index = 0;
				foreach (SimpleAttributeElement pk in classElement.PrimaryKeys)
				{
					builder.Append(pk.ColumnName);

					if (index < classElement.PrimaryKeys.Count - 1)
					{
						builder.Append(", ");
					}

					index ++;
				}

				builder.Append(generator.GetAddUniqueConstraintFooterDDL());

				ddl = builder.ToString();
				cmd.CommandText = ddl;

				if (_log != null)
				{
					_log.Append(ddl, LogType.DDL);
				}

				cmd.ExecuteNonQuery();
			}
		}
	}
}