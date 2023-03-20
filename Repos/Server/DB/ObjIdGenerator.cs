/*
* @(#) ObjIdGenerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// The generator that generates unique obj id for a data instance
	/// </summary>
	/// <version> 	1.0.1	20 Oct 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class ObjIdGenerator : KeyGenerator
	{
		/// <summary>
		/// Initiating an instance of ObjIdGenerator class.
		/// </summary>
		/// <param name="dataProvider">The data provider for the schema</param>
		public ObjIdGenerator(IDataProvider dataProvider) : base(dataProvider)
		{
		}
	}
}