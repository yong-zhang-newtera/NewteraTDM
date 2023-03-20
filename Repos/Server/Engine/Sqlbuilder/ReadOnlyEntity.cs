/*
* @(#)ReadOnlyEntity.cs
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
	/// An ReadOnlyEntity object represents the read-only attribute of an instance
	/// which provides the information about attributes that are read-only
	/// </summary>
	/// <version>  	1.0.0 20 Feb 2009 </version>
	public class ReadOnlyEntity : DBEntity
	{
		/// <summary>
		/// Initializes a new instance of the ReadOnlyEntity class
		/// </summary>
		public ReadOnlyEntity():base()
		{
		}

		/// <summary>
		/// Gets the name of permission count attribute name.
		/// </summary>
		/// <value> the name string </returns>
		public override string Name
		{
			get
			{
                return SQLElement.READ_ONLY;
			}
		}

		/// <summary>
		/// Gets type of  the read only entity
		/// </summary>
		/// <returns> the permission type</returns>
		public override DataType Type
		{
			get
			{
				return DataType.String;
			}
		}

		/// <summary>
		/// The permission flags
		/// is generated dynamically, there is no DB column corresponding to it.
		/// </summary>
		/// <returns> the column name for the permission count</returns>
		public override string ColumnName
		{
			get
			{
				return "";
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor</param>
		public override void Accept(EntityVisitor visitor)
		{
			// do nothing
		}
	}
}