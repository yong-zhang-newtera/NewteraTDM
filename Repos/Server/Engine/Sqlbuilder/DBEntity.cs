/*
* @(#)DBEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
    using System.Collections;

	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The DBEntity is an abstract class for the entity class that represent logical entities
	/// of Newtera's object-relational data model. Examples of the entities include ClassEntity,
	/// AttributeEntity, RelationshipEntity, and others. SQLBuilder relies on information
	/// in these entities and relationships of the entities to build a SQL statement that
	/// retrieve data from a relational database.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003 </version>
	/// <author> Yong Zhang</author>
    abstract public class DBEntity : IXaclObject
	{
		
		/* instance variables */
		private ClassEntity _ownerClass; // The owner class
		private int _colIndex; // the column index in a query result
        internal string _xpath = null; // run-time member
		
		/// <summary>
		/// Initiating a DBEntity object.
		/// </summary>
		public DBEntity()
		{
		}

		/// <summary>
		/// Gets the db entity type. this method is override by subclasses
		/// </summary>
		/// <value> One of the DataType enum</value>
		public abstract DataType Type
		{
			get;
		}

		/// <summary>
		/// Gets the entity name
		/// </summary>
		/// <value> the entity name</returns>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Gets the DB column name for the entity.
		/// </summary>
		/// <value>column name for the entity</value>
		public abstract string ColumnName{
			get;
		}

		/// <summary>
		/// Gets or sets the owner class of this attribute.
		/// </summary>
		/// <value> The owner class</value>
		public ClassEntity OwnerClass
		{
			get
			{
				return _ownerClass;
			}
			set
			{
				_ownerClass = value;
			}
		}

		/// <summary>
		/// Gets or sets the column index.
		/// </summary>
		/// <value> The column index</value>
		public int ColumnIndex
		{
			get
			{
				return _colIndex;
			}
			set
			{
				_colIndex = value;
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit this entity and its related entities. The
		/// concrete entity class is responsible for implement this method to traverse
		/// the visitor with its related entities.
		/// </summary>
		/// <param name="visitor">a visitor that implements EntityVisitor interface</param>
		public abstract void Accept(EntityVisitor visitor);

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public virtual string ToXPath()
        {
            return "";
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        public virtual IXaclObject Parent
        {
            get
            {
                // TODO
                return null;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public virtual IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion
    }
}