/*
* @(#)ScoreEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// A ScoreEntity object represents the score attribute. The score attribute is required when
	/// the score of a full-text search is used in display or in sorting the result.
	/// </summary>
	/// <version>  	1.0.1 22 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class ScoreEntity : DBEntity
	{
		/* private instance variables */
		private string _name; // The name that identifies the score entity
		private string _label; // the unique label
		private DatabaseType _dbType; // The database type
		
		/// <summary>
		/// Initiating an instance of ScoreEntity class.
		/// </summary>
		/// <param name="label">an unique lable for the score</param>
		/// <param name="dataProvider">the database provider</param>
		public ScoreEntity(string label, IDataProvider dataProvider) : base()
		{
			_name = SQLElement.SCORE + label;
			_label = label;
			_dbType = dataProvider.DatabaseType;
		}

		/// <summary>
		/// Get the name of score function name.
		/// </summary>
		/// <value> the function name.</value>
		public override string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the data type of score function.
		/// </summary>
		/// <value> the function data type.</value>
		public override DataType Type
		{
			get
			{
				return SQLElement.SCORE_TYPE;
			}
		}

		/// <summary>
		/// Gets the Database name for the score.
		/// </summary>
		/// <value> the column name for the score.</value>
		/// <remarks>This name varies on type of database.
		/// For example, the name is score(label) for Oracle database.
		/// </remarks>
		public override string ColumnName
		{
			get
			{
				return SymbolLookupFactory.Instance.Create(_dbType).GetScoreFunc(_label);
			}
		}
		
		/// <summary>
		/// Gets the label of score function name.
		/// </summary>
		/// <value> the label </value>
		public string Label
		{
			get
			{
				return _label;
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor.</param>
		public override void Accept(EntityVisitor visitor)
		{
			visitor.VisitScore(this);
		}
	}
}