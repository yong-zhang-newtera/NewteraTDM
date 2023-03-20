/*
* @(#)SchemaEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A SchemaEntity object represents a schema in which a query is sent to. A SchemaEntity
	/// may contain mutile ClassEntity objects as roots.
	/// </summary>
	/// <version>  	1.0.0 28 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class SchemaEntity : DBEntity
	{		
		/* private instance variables */
		private string _name; // The name of the schema
		private DBEntityCollection _roots; // The root entities
		
		/// <summary>
		/// Initiating an instance of SchemaEntity class
		/// </summary>
		public SchemaEntity() : base()
		{
			_name = "";
			_roots = new DBEntityCollection();
		}
		
		/// <summary>
		/// Initiating an instance of SchemaEntity class
		/// </summary>
		/// <param name="name">name of the schema.</param>
		public SchemaEntity(string name) : base()
		{
			_name = name;
			_roots = new DBEntityCollection();
		}
		
		/// <summary>
		/// Gets the name of schema.
		/// </summary>
		/// <value> the schema name.</value>
		public override string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the data type of schema
		/// </summary>
		/// <value> one of DataType enum.</value>
		public override DataType Type
		{
			get
			{
				return DataType.Unknown;
			}
		}

		/// <summary>
		/// Gets the Database name for the schema.
		/// </summary>
		/// <value>the same as schema name.</value>
		public override string ColumnName
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the root entities.
		/// </summary>
		/// <value> the root entities.</value>
		public DBEntityCollection RootEntities
		{
			get
			{
				return _roots;
			}
		}
	
		/// <summary>
		/// Accept a SchemaEntity to visit its root entities.
		/// </summary>
		/// <param name="visitor">the visiting visitor.</param>
		public override void Accept(EntityVisitor visitor)
		{
			// Visit itself
			if (visitor.VisitSchema(this))
			{
				
				foreach (DBEntity root in _roots)
				{
					root.Accept(visitor);
				}
			}
		}
		
		/// <summary>
		/// Adds a root entity to the schema entity.
		/// </summary>
		/// <param name="rootEntity">the root entity.</param>
		public void AddRootEntity(DBEntity rootEntity)
		{
			_roots.Add(rootEntity);
		}
	}
}