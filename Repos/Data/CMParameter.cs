/*
* @(#)CMParameter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;

	/// <summary>
	/// Represents a parameter to a Command object, and optionally, its mapping to DataSet
	/// columns.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003</version>
	/// <author>  		Yong Zhang </author>
	public class CMParameter : IDataParameter
	{
		private DbType _dbType;
		private ParameterDirection _direction;
		private bool _isNullable = false;
		private string _paramName = null;
		private string _sourceColumn = null;
		private object _value = null;
		private DataRowVersion _sourceVersion = DataRowVersion.Current;
		
		/// <summary> 
		/// Initiate a new instance of CMParameter class
		/// </summary>
		public CMParameter()
		{
			_dbType = DbType.String;
			_direction = ParameterDirection.Input;
		}
		
		/// <summary> 
		/// Initiate a new instance of CMParameter class
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="type">One of the DbType values.</param>
		public CMParameter(string parameterName, DbType type)
		{
			_direction = ParameterDirection.Input;
			_paramName = parameterName;
			_dbType = type;
		}
		
		/// <summary>
		/// Initiate a new instance of CMParameter class
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="val">An Object that is the value of the CMParameter.</param>
		public CMParameter(string parameterName, object val)
		{
			_direction = ParameterDirection.Input;
			_paramName = parameterName;
			_value = val;
			_dbType = InferType(val);
		}
		
		/// <summary>
		/// Initiate a new instance of CMParameter class
		/// </summary>
		/// <param name="parameterName">The name of the parameter to map.</param>
		/// <param name="dbType">One of the DbType values.</param>
		/// <param name="sourceColumn">The name of the source column.</param>
		public CMParameter(string parameterName, DbType dbType, string sourceColumn)
		{
			_direction = ParameterDirection.Input;
			_paramName = parameterName;
			_dbType = dbType;
			_sourceColumn = sourceColumn;
		}
		
		/// <summary> 
		/// Gets or sets the DbType of the parameter.
		/// </summary>
		/// <value> One of the DbType values. The default is String.</value>
		public DbType DbType
		{
			get
			{
				return _dbType;
			}
			set
			{
				_dbType = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the parameter is input-only, output-only, bidirectional,
		/// or a stored procedure return value parameter.
		/// </summary>
		/// <value> One of the ParameterDirection values. The default is Input.</value>
		public ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the parameter accepts null values.
		/// </summary>
		/// <value>true if null values are accepted; otherwise, false. The default is false.</value>
		public bool IsNullable
		{
			get
			{
				return _isNullable;
			}
		}

		/// <summary>
		/// Gets or sets the name of the CMDataParameter.
		/// </summary>
		/// <value> name of the CMDataParameter. Default is empty string</value>
		/// <remarks>
		/// You must set ParameterName before executing a command that relies on parameters.
		/// </remarks>
		public string ParameterName
		{
			get
			{
				return _paramName;
			}
			set
			{
				_paramName = value;
			}
		}

		/// <summary>
		/// Gets the name of the source column that is mapped to the DataSet and used for
		/// loading or returning the Value.
		/// </summary>
		/// <value> The name of the source column that is mapped to the DataSet. The default is an empty string.
		/// </value>
		public string SourceColumn
		{
			get
			{
				return _sourceColumn;
			}
			set
			{
				_sourceColumn = value;
			}
		}

		/// <summary>
		/// Gets or sets the DataRowVersion to use when loading Value.
		/// </summary>
		/// <value>One of the DataRowVersion values. The default is Current.</value>
		/// <remarks>
		/// This property is used by the UpdateCommand during the Update to determine whether
		/// the original or current value is used for a parameter value. This allows primary keys
		/// to be updated. This property is ignored by the InsertCommand and DeleteCommand.
		/// This property is set to the version of the DataRow used by the Item property,
		/// or the GetChildRows method of the DataRow object.
		/// </remarks>
		public DataRowVersion SourceVersion 
		{
			get
			{
				return _sourceVersion;
			}
			set
			{
				_sourceVersion = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		/// <value> An Object that is the value of the parameter. The default value is null.
		/// </value>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_dbType = InferType(value);
			}
		}
		
		/// <summary>
		/// Convert value type from .Net type to DBType
		/// </summary>
		/// <returns>One of DbType, the default is Object</returns>
		private DbType InferType(object val)
		{
			if (val is System.Boolean)
			{
				return DbType.Boolean;
			}
			else if (val is System.Byte)
			{
				return DbType.Byte;
			}
			else if (val is System.DateTime)
			{
				return DbType.Date;
			}
			else if (val is System.Double)
			{
				return DbType.Double;
			}
			else if (val is System.Single)
			{
				return DbType.Single;
			}
			else if (val is System.Int32)
			{
				return DbType.Int32;
			}
			else if (val is System.Int64)
			{
				return DbType.Int64;
			}
			else if (val is string)
			{
				return DbType.String;
			}
			else
			{
				return DbType.Object;
			}
		}
	}
}