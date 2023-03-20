/*
* @(#)AttachmentEntity.cs
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
	/// An AttachmentEntity object represents the attachment count attribute of a class.
	/// The attachment count attribute is a default attribute of a class.
	/// </summary>
	/// <version>  	1.0.0 16 Jul 2003 </version>
	/// <author> Yong Zhang</author>
	public class AttachmentEntity : DBEntity
	{
		/// <summary>
		/// Initializes a new instance of the AttachmentEntity class
		/// </summary>
		public AttachmentEntity():base()
		{
		}

		/// <summary>
		/// Gets the name of attachment count attribute name.
		/// </summary>
		/// <value> the name string </returns>
		public override string Name
		{
			get
			{
				return SQLElement.ATTACHMENT_COUNT;
			}
		}

		/// <summary>
		/// Gets type of  the attachment entity
		/// </summary>
		/// <returns> the attachment type</returns>
		public override DataType Type
		{
			get
			{
				return SQLElement.ATTACHMENT_COUNT_TYPE;
			}
		}

		/// <summary>
		/// Gets the DB column name for the attachment count.
		/// </summary>
		/// <returns> the column name for the attachment count</returns>
		public override string ColumnName
		{
			get
			{
				return SQLElement.COLUMN_ATTACHMENT_COUNT;
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