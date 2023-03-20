/*
* @(#)ClsIdEntity.cs
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
	/// An ClsIdEntity object represents the class id attribute of a class. The class id
	/// attribute is a default attribute of a class.
	/// </summary>
	/// <version>  	1.0.1 23 Jul 2003 </version>
	/// <author> Yong Zhang</author>
	public class ClsIdEntity:DBEntity
	{	
		/// <summary>
		/// Initiating an instance of ClsIdEntity class.
		/// </summary>
		public ClsIdEntity() : base()
		{
		}

		/// <summary>
		/// Get the name of class id attribute name.
		/// </summary>
		/// <returns> the name </returns>
		public override string Name
		{
			get
			{
				return SQLElement.CLS_ID;
			}
		}

		/// <summary>
		/// Gets the class id type
		/// </summary>
		/// <returns> one of the DataType enum</returns>
		public override DataType Type
		{
			get
			{
				return SQLElement.CLS_ID_TYPE;
			}
		}

		/// <summary>
		/// Gets the DB column name for the class id.
		/// </summary>
		/// <returns> the column name for the class id</returns>
		public override string ColumnName
		{
			get
			{
				return SQLElement.COLUMN_CLS_ID;
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor.</param>
		public override void Accept(EntityVisitor visitor)
		{
			// nothing
		}
	}
}