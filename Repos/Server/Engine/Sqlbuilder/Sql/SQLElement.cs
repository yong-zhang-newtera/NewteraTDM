/*
* @(#)SQLElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A SQLElement abstract class defines an uniform interface for all objects
	/// that can appear in a SQL statement structure. Its subclasses define both
	/// primitive elements (like keyword or fields) and structural elements
	/// (like SELECT clause and WHERE clause).
	/// </summary>
	/// <version>  	1.0.1 14 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	abstract public class SQLElement : IComparable
	{		
		public const string OPT_EQUALS = "=";
		public const string OPT_GT = ">";
		public const string OPT_GEQ = ">=";
		public const string OPT_LT = "<";
		public const string OPT_LEQ = "<=";
		public const string OPT_NEQ = "<>";
		public const string OPT_LIKE = "LIKE";
		public const string OPT_IN = "IN";
		public const string OPT_NOT_IN = "NOT IN";
				
		// Function Name definitions
		public const string FUNCTION_AVG_STR = "avg";
		public const string FUNCTION_COUNT_STR = "count";
        public const string FUNCTION_DISTINCT_STR = "distinct";
		public const string FUNCTION_MIN_STR = "min";
		public const string FUNCTION_MAX_STR = "max";
		public const string FUNCTION_SUM_STR = "sum";
		
		public const string SORT_ASCEND = "ASC";
		public const string SORT_DESCEND = "DESC";
		
		// Represents a SQL null
		public const string VALUE_NULL = "@";
		public const string NULL_STRING = "";
		
		// Internal column definitions
		public const string TABLE_NAME = "tableName";
		public const string COLUMN_NAME = "columnName";
		
		// DB column definition
		public const string OBJ_ID = "obj_id";
		public const string CLS_ID = "type";
		public const string ATTACHMENT_COUNT = "attachments";
		public const string PERMISSION = "permission";
        public const string READ_ONLY = "read_only";
		public const string SCORE = "score";
		public const string COLUMN_OBJ_ID = "OID";
		public const string COLUMN_CLS_ID = "CID";
		public const string COLUMN_ATTACHMENT_COUNT = "ANUM";

		public const DataType OBJ_ID_TYPE = DataType.BigInteger;
		public const DataType CLS_ID_TYPE = DataType.BigInteger;
		public const DataType ATTACHMENT_COUNT_TYPE = DataType.Integer;
		public const DataType SCORE_TYPE = DataType.Integer;
				
		// Element name definitions
		public const string ELEMENT_CLASS_NAME_SUFFIX = "List";
		
		// Encrypted value definition
		public const string ENCRYPTED_VALUE = "";
		
		// private instance members
		private SQLElement _parent; // reference to the parent element
		private int _position;
		private ClassEntity _entity;
		
		/// <summary>
		/// Initiating SQLElement object.
		/// </summary>
		public SQLElement()
		{
			_parent = null;
			_position = 0;
			_entity = null;
		}

		/// <summary>
		/// Gets children of the SQLElement.
		/// </summary>
		/// <value>
		/// an empty collection, subclasses can override it
		/// </value>
		public virtual SQLElementCollection Children
		{
			get
			{
				return new SQLElementCollection();
			}
		}

		/// <summary>
		/// Gets the information indicating whether the element is a composit element.
		/// </summary>
		/// <value>
		/// return false by default.
		/// </value>
		public virtual bool IsComposite
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets or sets parent of the element object.
		/// </summary>
		/// <value>
		/// The SQLElement that is the parent of the element, null if the element is root
		/// </value>
		public SQLElement Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Gets or sets position of the element.
		/// </summary>
		/// <value>
		/// The integer represents the position of the element in a list
		/// </value>
		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		/// <summary>
		/// Gets or sets class entity of the element.
		/// </summary>
		/// <Value>
		/// the ClassEntity object.
		/// </Value>
		public ClassEntity ClassEntity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}
				
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public abstract string ToSQL();
		
		/// <summary> 
		/// Add an element as child
		/// </summary>
		/// <param name="element">the element to be added.</param>
		/// <remarks>Throw an exception by default for non-composite object (leaf)</remarks>
		public virtual void Add(SQLElement element)
		{
			// throw an exception by default
			throw new SQLBuilderException("Try to add an element to a leaf node");
		}
		
		/// <summary>
		/// Remove a child
		/// </summary>
		/// <param name="element">the element to be removed.</param>
		/// <remarks>Throw an exception by default for non-composite object (leaf)</remarks>
		public virtual void Remove(SQLElement element)
		{
			// throw an exception by default
			throw new SQLBuilderException("Try to remove child from a leaf node");
		}
		
		/// <summary>
		/// Sort the children based on their positions.
		/// </summary>
		/// <remarks>Throw an exception by default for non-composite object (leaf)</remarks>
		public virtual void SortChildren()
		{
			// throw an exception by default
			throw new SQLBuilderException("Try to sort children for a leaf node");
		}

		#region IComparable Members

		/// <summary>
		/// IComparable.CompareTo implementation.
		/// </summary>
		/// <param name="obj">An object to compare with this instance. </param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the comparands. The return value has these meanings:</returns>
		public int CompareTo(object obj)
		{
			SQLElement other = (SQLElement) obj;
			if (this.Position == other.Position)
			{
				return 0;
			}
			else if (this.Position > other.Position)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}

		#endregion
		
		/// <summary>
		/// Get information indicating whether the given type is one of the
		/// numeric type. The numeric type includes double, float, and number.
		/// </summary>
		/// <returns>
		/// true if the type is a numeric type, such as float, double and number
		/// </returns>
		public static bool IsNumericType(DataType type)
		{
			if (type == DataType.Double ||
				type == DataType.Float)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// Describes the options for SQL functions
	/// </summary>
	public enum SQLFunction
	{
		Unknown,
		Avg,
		Count,
        Distinct,
		Min,
		Max,
		Sum,
		Contains,
		Like,
		Score
	}

	/// <summary>
	/// Describes the options for relationship direction
	/// </summary>
	public enum RelationshipDirection
	{
		Forward,
		Backward
	}
}