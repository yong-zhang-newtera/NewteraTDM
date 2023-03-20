/*
* @(#)AdminTest.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Test
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Data;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Data;
	using Newtera.Data.DB;
	using Newtera.WebService;

	/// <summary>
	/// Test program of AdminWebService
	/// </summary>
	/// <version>  	1.0.0 03 Feb 2004 </version>
	/// <author>  Yong Zhang </author>
	public class AdminTest
	{
		private const string DATABASE_TYPE = "ORACLE";
		private const string DATA_SOURCE = "NEWTERA";

		public const string ConnectionString = "";

		public static void Main()
		{

			if (TestDataSource())
			{
				if (TestNeedCreateTablespace())
				{
				}

				TestUpdateSchema();
			}

			return;
		}

		static private bool TestDataSource()
		{
			AdminService service = new AdminService();
			bool status = true;
			try
			{
				service.IsDataSourceValid(DATABASE_TYPE, DATA_SOURCE);

				Console.WriteLine("Test of IsDataSourceValid succeeded.");
			}
			catch(Exception)
			{
				Console.WriteLine("Test of IsDataSourceValid failed.");
				status = false;
			}

			return status;
		}

		static private bool TestNeedCreateTablespace()
		{
			AdminService service = new AdminService();
			bool status = true;
			try
			{
				status = service.NeedCreateTablespace(DATABASE_TYPE, DATA_SOURCE);

				Console.WriteLine("Test of NeedCreateTablespace succeeded.");
			}
			catch(Exception)
			{
				Console.WriteLine("Test of NeedCreateTablespace failed.");
			}

			return status;
		}

		static private void TestUpdateSchema()
		{
			AdminService service = new AdminService();
			try
			{
				service.UpdateSchema(DATABASE_TYPE, DATA_SOURCE);

				Console.WriteLine("Test of UpdateSchema succeeded.");
			}
			catch(Exception)
			{
				Console.WriteLine("Test of UpdateSchema failed.");
			}
		}
	}
}