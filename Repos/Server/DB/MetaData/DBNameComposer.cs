/*
* @(#) DBNameComposer.cs	1.0.1
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Composing various names for database manipulations
	/// </summary>
	/// <version> 	1.0.1 14 10 2003</version>
	/// <author> 	Yong Zhang </author>
	public class DBNameComposer
	{
		private static int TableNameMaxLength = 30;
		private static string TableNamePrefix = "T_";
		private static int ColumnNameMaxLength = 30;
		private static string ColumnNamePrefix = "C_";
		private static string FKNamePrefix = "FC_";
		private static int UniqueKeyNameMaxLength = 18;
		private static string UniqueKeyPrefix = "UC_";
		private static string SequenceNamePrefix = "S_";
		private static string TriggerNamePrefix = "G_";
		private static string RegularIndexNamePrefix = "IDX_";
		private static string ForeignKeyNamePrefix = "FK_";
		private static string PrimaryKeyNamePrefix = "PK_";
        private static string DefaultValuePrefix = "DEF_";
        private static int DefaultValueNameMaxLength = 18;

		/// <summary>
		/// Gets the database Table Name for an added class.
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>A created table name</returns>
		/// <remarks>
		/// Table name format is : T_<class name>_<class id>_<schema id>
		/// 
		/// Note that the length of a table name can not exceed the length
		/// specified by TableNameMaxLength. When necessary, we have to trim
		/// the class name to make it meet the max length requirement.
		/// </remarks>
		public static string GetTableName(ClassElement classElement)
		{
			String classId = classElement.ID;
			string schemaId = classElement.SchemaModel.SchemaInfo.ID;
			int classNameLength = TableNameMaxLength - classId.Length- schemaId.Length - 4;

			string className = classElement.Name;
			if (className.Length > classNameLength)
			{
				className = className.Substring(0, classNameLength);
			}

			return TableNamePrefix + className.ToUpper() + "_" + classId + "_" + schemaId;
		}

		/// <summary>
		/// Gets the database column Name for the added simple attribute.
		/// </summary>
		/// <param name="schemaModelElement">The attribute element</param>
		/// <returns>A created column name</returns>
		/// <remarks>
		/// Column name format is : C_<attribute name>_<attribute id>
		/// 
		/// Note that the length of a column name can not exceed the length
		/// specified by ColumnNameMaxLength. When necessary, we have to trim
		/// the attribute name to make it meet the max length requirement.
		/// </remarks>
		public static string GetColumnName(SchemaModelElement attribute)
		{
			String attributeId = attribute.ID;
			int attributeNameLength = ColumnNameMaxLength - attributeId.Length - 3;

			string attributeName = attribute.Name.ToUpper();
			if (attributeName.Length > attributeNameLength)
			{
				attributeName = attributeName.Substring(0, attributeNameLength);
			}

			return ColumnNamePrefix + attributeName.ToUpper() + "_" + attributeId;
		}

		/// <summary>
		/// Gets the unique key name .
		/// </summary>
		/// <param name="classElement">The owner class element</param>
		/// <param name="attribute">A simple attribute element</param>
		/// <return>A created unique key name</return>
		/// <remarks>
		/// Unique key name format is : UC_<class name>_<attribute id>_<class id>
		/// 
		/// Note that the length of a unique key name can not exceed the length
		/// specified by UniqueKeyNameMaxLength. When necessary, we have to trim
		/// the class name to make it meet the max length requirement.
		/// </remarks>
		public static string GetUniqueKeyName(ClassElement classElement, SimpleAttributeElement attribute)
		{
			string attributeId = "KEY";
			if (attribute != null)
			{
				attributeId = attribute.ID;
			}

			String classId = classElement.ID;
			int classNameLength = UniqueKeyNameMaxLength - UniqueKeyPrefix.Length - classId.Length - attributeId.Length - 5;

			string className = classElement.Name.ToUpper();
			if (className.Length > classNameLength)
			{
				className = className.Substring(0, classNameLength);
			}

			return UniqueKeyPrefix + className.ToUpper() + "_" + attributeId + "_" + classId;
		}

        /// <summary>
        /// Gets the default value constraint name .
        /// </summary>
        /// <param name="classElement">The owner class element</param>
        /// <param name="attribute">A simple attribute element</param>
        /// <return>A created unique default value constraint name</return>
        /// <remarks>
        /// Default value constraint name format is : DEF_<class name>_<attribute id>_<class id>
        /// 
        /// Note that the length of a Default value constraint name can not exceed the length
        /// specified by DefaultValueNameMaxLength. When necessary, we have to trim
        /// the class name to make it meet the max length requirement.
        /// </remarks>
        public static string GetDefaultValueConstraintName(ClassElement classElement, SimpleAttributeElement attribute)
        {
            string attributeId = "KEY";
            if (attribute != null)
            {
                attributeId = attribute.ID;
            }

            String classId = classElement.ID;
            int classNameLength = DefaultValueNameMaxLength - DefaultValuePrefix.Length - classId.Length - attributeId.Length - 5;

            string className = classElement.Name.ToUpper();
            if (className.Length > classNameLength)
            {
                className = className.Substring(0, classNameLength);
            }

            return DefaultValuePrefix + className.ToUpper() + "_" + attributeId + "_" + classId;
        }

		/// <summary>
		/// Gets the primary key name .
		/// </summary>
		/// <param name="classId">The owner class id</param>
		/// <return>A created primary key name</return>
		/// <remarks>
		/// Primary key name format is : PK_<class id>
		/// </remarks>
		public static string GetPrimaryKeyName(string classId)
		{
			return PrimaryKeyNamePrefix + classId;
		}

		/// <summary>
		/// Gets the index name.
		/// </summary>
		/// <param name="classElement">The owner class element</param>
		/// <param name="attribute">A simple attribute element</param>
		/// <return>A index name</return>
		/// <remarks>
		/// Regular index name format is : IDX_<attributeID>_<classID>
		/// 
		/// Full text index name format is : IDX1_<attributeID>_<classID>
		/// </remarks>
		public static string GetIndexName(ClassElement classElement, SimpleAttributeElement attribute)
		{
			string indexNamePrefix;

			indexNamePrefix = RegularIndexNamePrefix;

			return indexNamePrefix + attribute.ID + "_" + classElement.ID;
		}

        /// <summary>
        /// Gets the index name for relationship attribute.
        /// </summary>
        /// <param name="classElement">The owner class element</param>
        /// <param name="attribute">A relationship attribute element</param>
        /// <return>A index name</return>
        /// <remarks>
        /// Regular index name format is : IDX_<attributeID>_<classID>
        /// 
        /// Full text index name format is : IDX1_<attributeID>_<classID>
        /// </remarks>
        public static string GetIndexName(ClassElement classElement, RelationshipAttributeElement attribute)
        {
            string indexNamePrefix = RegularIndexNamePrefix;

            return indexNamePrefix + attribute.ID + "_" + classElement.ID;
        }

		/// <summary>
		/// Gets a constraint name for a foreign key.
		/// </summary>
		/// <param name="element">A schema model element</param>
		/// <param name="isParentChild">True if the foreign key is for a parent-child relationship, false otherwise.</param>
		/// <return>A constraint name</return>
		/// <remarks>
		/// Regular FK constraint name format is : FK_<attribute id>
		/// 
		/// Parent-child FK constraint name format is : FK_<class id>_OID
		/// </remarks>
		public static string GetFKConstraintName(SchemaModelElement element,
			bool isParentChild)
		{
			string constraintName;

			if (isParentChild)
			{
				constraintName = ForeignKeyNamePrefix + element.ID + "_OID";
			}
			else
			{
				constraintName = ForeignKeyNamePrefix + element.ID;
			}

			return constraintName;
		}

		/// <summary>
		/// Gets name of a join table
		/// </summary>
		/// <returns>the name on the joined table.</returns>
		/// <remarks>
		/// Table name format is : T_<attribute id of join manager>_<id of backward relationship of linked class>
		/// </remarks>
		public static string GetJoinedTableName(RelationshipAttributeElement relationship,
			RelationshipAttributeElement backwardRelationship)
		{
			string attributeId1, attributeId2;
			
			// the first id is join manager's
			if (relationship.IsJoinManager)
			{
				attributeId1 = relationship.ID;
				attributeId2 = backwardRelationship.ID;
			}
			else
			{
				attributeId1 = backwardRelationship.ID;
				attributeId2 = relationship.ID;
			}

			return TableNamePrefix + attributeId1 + "_" + attributeId2;
		}

		/// <summary>
		/// Gets the Foreign Key Name for the added relationship attribute.
		/// </summary>
		/// <returns>A created column name</returns>
		/// <remarks>
		/// For "MANY TO ONE" and "ONE TO ONE & JOIN MANAGER", column name is the foreign key column name;
		/// </remarks>
		public static string GetForeignKeyName(RelationshipAttributeElement relationship)
		{
		    return GetLocalFKName(relationship);
		}

		/// <summary>
		/// Get a local foreign key name
		/// </summary>
		/// <returns>The Foreign key name on local table</returns>
		/// <remarks>
		/// FK name format is : FC_<linked class name(5)>_<attribute id>
		/// 
		/// Note that the length of the linked class name is limited to 5.
		/// </remarks>
		private static string GetLocalFKName(RelationshipAttributeElement relationship)
		{
			String attributeId = relationship.ID;

			string linkedClassName = relationship.LinkedClass.Name.ToUpper();
			if (linkedClassName.Length > 5)
			{
				linkedClassName = linkedClassName.Substring(0, 5);
			}

			return FKNamePrefix + linkedClassName.ToUpper() + "_" + attributeId;
		}

		/// <summary>
		/// Gets a sequence name for an auto-increment column.
		/// </summary>
		/// <param name="classElement">The owner class element</param>
		/// <param name="attribute">A simple attribute element</param>
		/// <return>A created sequence name</return>
		/// <remarks>
		/// Sequence name format is : S_<attribute id>_<class id>
		/// </remarks>
		public static string GetSequenceName(ClassElement classElement, SimpleAttributeElement attribute)
		{
			return SequenceNamePrefix + attribute.ID + "_" + classElement.ID;
		}

		/// <summary>
		/// Gets a trigger name for an auto-increment column.
		/// </summary>
		/// <param name="classElement">The owner class element</param>
		/// <param name="attribute">A simple attribute element</param>
		/// <return>A created trigger name</return>
		/// <remarks>
		/// Sequence name format is : G_<attribute id>_<class id>
		/// </remarks>
		public static string GetTriggerName(ClassElement classElement, SimpleAttributeElement attribute)
		{
			return TriggerNamePrefix + attribute.ID + "_" + classElement.ID;
		}
	}
}