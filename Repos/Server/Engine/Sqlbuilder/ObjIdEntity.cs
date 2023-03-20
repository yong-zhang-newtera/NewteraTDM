/*
* @(#)ObjIdEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// An ObjIdEntity object represents the obj_id attribute of a class. The obj_id attribute
	/// is a default attribute of a class.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class ObjIdEntity : DBEntity
	{
		/* private instance variables */
		
		/// <summary>
		/// Initiating an instance of ObjIdEntity class
		/// </summary>
		public ObjIdEntity() : base()
		{
		}

		/// <summary>
		/// Gets the obj id name.
		/// </summary>
		/// <value> the obj id name</value>
		public override string Name
		{
			get
			{
				return SQLElement.OBJ_ID;
			}
		}

		/// <summary>
		/// Gets the obj id type
		/// </summary>
		/// <value> the obj id type </value>
		public override DataType Type
		{
			get
			{
				return SQLElement.OBJ_ID_TYPE;
			}
		}

		/// <summary>
		/// Gets the DB column name for the obj id. 
		/// </summary>
		/// <value> the column name for the object id</value>
		public override string ColumnName
		{
			get
			{
				return SQLElement.COLUMN_OBJ_ID;
			}
		}
				
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor</param>
		public override void Accept(EntityVisitor visitor)
		{
		}
	}
}