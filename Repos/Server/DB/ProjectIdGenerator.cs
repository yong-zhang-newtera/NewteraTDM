/*
* @(#) ProjectIdGenerator.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// The generator that generates unique id for a workflow project.
	/// </summary>
	/// <version>1.0.1 15 Dec 2006 </version>
	public class ProjectIdGenerator : KeyGenerator
	{
		/// <summary>
		/// Initiating an instance of ProjectIdGenerator class.
		/// </summary>
		/// <param name="dataProvider">The data provider for the schema</param>
		public ProjectIdGenerator(IDataProvider dataProvider) : base(dataProvider)
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
				return "PROJECT_ID";
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