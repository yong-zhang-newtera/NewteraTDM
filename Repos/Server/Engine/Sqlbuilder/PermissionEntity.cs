/*
* @(#)PermissionEntity.cs
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
	/// An PermissionEntity object represents the permission attribute of an instance
	/// which provides the information about user's permissions to perform certain
	/// actions on the instance.
	/// </summary>
	/// <version>  	1.0.0 06 Dec 2004 </version>
	/// <author> Yong Zhang</author>
	public class PermissionEntity : DBEntity
	{
		/// <summary>
		/// Initializes a new instance of the PermissionEntity class
		/// </summary>
		public PermissionEntity():base()
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
				return SQLElement.PERMISSION;
			}
		}

		/// <summary>
		/// Gets type of  the permission entity
		/// </summary>
		/// <returns> the permission type</returns>
		public override DataType Type
		{
			get
			{
				return DataType.Byte;
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