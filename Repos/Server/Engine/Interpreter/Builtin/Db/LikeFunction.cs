/*
* @(#)LikeFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// Class LikeFunction is a builtin function that compares
	/// an attribute value with an substring, similar to SQL LIKE operator.
	/// It return boolean value.
	/// </summary>
	/// <version>  1.0 Dec. 04, 2003</version>
	/// <author>  Yong Zhang </author>
	public class LikeFunction : FunctionImpBase, IDBFunction
	{
		private TreeManager _treeManager = null;
		private SQLElement _likeFuncElement = null;
		private DBEntity _functionEntity = null;
		private IDataProvider _dataProvider = null;
		
		/// <summary> Constructor: Functions are instanciated by Class.forName().
		/// This means its real constructor cannot have any arguments. This
		/// constructor is fairly useless, the real work is done in the 
		/// method Constructor()
		/// </summary>
		public LikeFunction() : base()
		{
		}

		/// <summary>
		/// Set the tree manager for retrieving an entity using a path
		/// </summary>
		/// <value>the tree manager </value>
		public TreeManager TreeManager
		{
			set
			{
				_treeManager = value;
			}
		}

		/// <summary>
		/// Sets the data provider.
		/// </summary>
		/// <value>the database connection.</value>
		public IDataProvider DataProvider
		{
			set
			{
				_dataProvider = value;
			}
		}

		/// <summary>
		/// Gets an SQL equivalent element created for the function
		/// </summary>
		/// <value> an SQLElement </value>
		public SQLElement SQLElement
		{
			get
			{
				return _likeFuncElement;
			}
		}

		/// <summary>
		/// Get the DBEntity for the function.
		/// </summary>
		/// <value> a DBEntity</value>
		public DBEntity FunctionEntity
		{
			get
			{
				return _functionEntity;
			}
		}
		
		/// <summary>
		/// Get the function type, defined in SQLElement.
		/// </summary>
		/// <value> one of SQLFunction enum values.</value>
		public SQLFunction Type
		{
			get
			{
				return SQLFunction.Like;
			}
		}

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			// There must be two argument
			if (!(arguments.Count == 2))
			{
				throw new InterpreterException("Usage: like(path, string)");
			}
			
			if (!(arguments[0] is Path))
			{
				throw new InterpreterException("The first argument of like function must be a path.");
			}
		}

		/// <summary>
		/// prepare the function.
		/// </summary>
		public override void Prepare()
		{
			// nothing needs to be prepared
		}
		
		/// <summary>
		/// This is called during visiting qualifiers in a query and allows to build
		/// a SQL equivalent function to contains function.
		/// </summary>
		public override void Restrict()
		{
			DBEntity entity = _treeManager.FindEntity(((Path) _arguments[0]).GetAbsolutePathEnumerator());
			if (entity is AttributeEntity)
			{
				AttributeEntity attrEntity = (AttributeEntity) entity;
				SQLElement field = new SearchFieldName(attrEntity.ColumnName, attrEntity.OwnerClass.Alias, attrEntity.CaseStyle, _dataProvider);
				field.ClassEntity = attrEntity.OwnerClass; // reference to its owner class
				
				_likeFuncElement = new LikeFunc(field, ((IExpr) _arguments[1]).Eval().ToString(), _dataProvider);
			}
			else
			{
				throw new InterpreterException("The first argument is an invalid path");
			}
		}
		
		/// <summary>
		/// like function is executed by a database, therefore, this function does not need to
        /// perform anything except just returning a true, since all the records have been selected
        /// by the data base according to the search criteria.
		/// </summary>
		public override Value Eval()
		{
            return new XBoolean(true);
		}
	}
}