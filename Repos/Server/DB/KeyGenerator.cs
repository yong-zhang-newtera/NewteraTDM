/*
* @(#) BigIntegerKeyGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// The purpose of this class is to support generating an
	/// unique numeric value for a appliaction in a distributed environment.
	/// It generates an unique integer and return it an UInt64 which is 64-bit
	/// unsigned integer.  The 32-bit high number is retrieved from a
	/// provider-indenpent relational database. The 32-bit low number is generated
	/// at the runtime.
	/// 
	/// The table in database is named KEY_GENERATE, the column of the table is
	/// named KEY_VALUE and is a numeric type by default.  In Oracle 8.1.6,
	/// the type of the column may be number(38). In MS SQL and Server2000,
	/// the type of the column may be decimal.
	/// 
	/// To support that key is generated from difference tables or/and from
	/// difference columns, you must implement this class and override the method
	/// GetTableName or/and GetColumnName. And initialize each class that implemented
	/// this class to generate an unique value for each kind of key.
	/// 
	/// To support defference range bit-length number, you should implement this
	/// class and override the method GetLowBitLength or/and GetHightBitLength.
	/// You should initialize this class used a KeyGeneratorFactory object.	
	/// 
	/// NOTE: You should assign an individual database connection to each object
	/// of IKeyGenerator.  Because the operation of generating number value need
	/// separate transcation control of the database.If the database connection
	/// assigned to this class is also used in other place of the application,
	/// the appliaction may loss transcation.																																																																																																																																						 * control when it accesses database.	/// 
	/// </summary>
	/// <version> 	1.0.1	20 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class KeyGenerator
	{
        private static object _globalLock = new object();
		private IDataProvider _dataProvider;
		private BigInteger _highValue = null;
		private BigInteger _lowValue = BigInteger.Zero;

		/// <summary>
		/// Instantiate an instance of KeyGenerator class.
		/// </summary>
		/// <param name="dataProvider">Data Provider</param>
		public KeyGenerator(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
			_lowValue = new BigInteger(0);
		}

		/// <summary>
		/// Generates an unique integer. The value is 64-bit long.
		/// </summary>
		/// <returns>A big integer instance</returns>
		public BigInteger NextKey()
		{
            // Must synchronize access to NextKey of multiple generators since
            // they'll all update the single record in the database
            lock (_globalLock)
			{				
				// High value has not been initialized.
				if(_highValue == null) 
				{
					_highValue = GetHighValue();
				}
				
				// Check if the low integer has been reached maximum.
				if(_lowValue.BitLength > LowBitLength) 
				{
					_highValue = GetHighValue();
					_lowValue = _lowValue.Reset(BigInteger.Zero);
				}

				BigInteger keyValue = _highValue.Add(_lowValue);
				
				// Low value increases by 1 for next time.
				_lowValue.Add(BigInteger.One);

				return keyValue;
			}
		}
		
		/// <summary>
		/// Get High 32-bit integer from database.
		/// </summary>
		/// <returns>A big integer with high value loaded from database</returns>
		private BigInteger GetHighValue()
		{
			string updateSQL = "UPDATE " + TableName + " SET " + ColumnName + "=" + ColumnName + "+1";
			string selectSQL = "SELECT " + ColumnName + " FROM " + TableName;
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDataReader dataReader = null;

			try
			{
				IDbCommand cmd = con.CreateCommand();
				cmd.Transaction = tran;

				// increase the value by one
				cmd.CommandText = updateSQL;
				cmd.ExecuteNonQuery();

				// Retrieve the high value from DB
				cmd.CommandText = selectSQL;
				dataReader = cmd.ExecuteReader();
				dataReader.Read();
				ulong val = System.Convert.ToUInt64(dataReader.GetValue(0));
				
				BigInteger highValue = new BigInteger(val);

				highValue.ShiftLeft(LowBitLength);

				dataReader.Close();
				dataReader = null;

				tran.Commit();

				return highValue;
			}
			catch (Exception ex)
			{
				tran.Rollback();
				throw new GeneratIDFailException(ex.Message, ex);
			}
			finally
			{
				if (dataReader != null && !dataReader.IsClosed)
				{
					dataReader.Close();
				}

				con.Close();
			}
		}
		
		/// <summary>
		/// Gets the name of table that stores high values of keys
		/// </summary>
		/// <value>The default value is KEY_GENERATE</value>
		protected virtual String TableName
		{
			get
			{
				return "KEY_GENERATE";
			}
		}

		/// <summary>
		/// Gets the name of column that stores high value of a key
		/// </summary>
		/// <value>The default value is KEY_VALUE</value>
		protected virtual String ColumnName
		{
			get
			{
				return "KEY_VALUE";
			}
		}
		
		/// <summary>
		/// Gets the bit length of high value.
		/// </summary>
		/// <value>The default value is 32</value>
		protected virtual int HighBitLength
		{
			get
			{
				return 32;
			}
		}

		/// <summary>
		/// Gets the bit length of low value.
		/// </summary>
		/// <value>The default value is 32</value>
		protected virtual int LowBitLength
		{
			get
			{
				return 32;
			}
		}
	}
}
