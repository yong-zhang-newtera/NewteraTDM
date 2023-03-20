/*
* @(#) ValueIdGenerator.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// The generator that generates unique id for an auto-icremental value.
	/// </summary>
	/// <version> 	1.0.1	14 Nov 2007 </version>
	public class ValueIdGenerator : KeyGenerator
	{
		/// <summary>
		/// Initiating an instance of ValueIdGenerator class.
		/// </summary>
		/// <param name="dataProvider">The data provider for the schema</param>
		public ValueIdGenerator(IDataProvider dataProvider) : base(dataProvider)
		{
		}

		/// <summary>
		/// Gets the name of column that stores high value of a key
		/// </summary>
		/// <value>The default value is KEY_VALUE</value>
		protected override String ColumnName
		{
			get
			{
				return "VALUE_ID";
			}
		}

		/// <summary>
		/// Gets the bit length of low value.
		/// </summary>
		/// <value>return o bit length</value>
		protected override int LowBitLength
		{
			get
			{
				return 0;
			}
		}
	}
}