/*
* @(#) ClassIdGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// The generator that generates unique class id for a class
	/// </summary>
	/// <version> 	1.0.1	20 Oct 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class ClassIdGenerator : KeyGenerator
	{
		/// <summary>
		/// Initiating an instance of ClassIdGenerator class.
		/// </summary>
		/// <param name="dataProvider">The data provider for the schema</param>
		public ClassIdGenerator(IDataProvider dataProvider) : base(dataProvider)
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
				return "CLASS_ID";
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