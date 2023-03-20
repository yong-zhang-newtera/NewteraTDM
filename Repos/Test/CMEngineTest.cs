/*
* @(#)CMEngineTest.cs
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
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.Attachment;

	/// <summary>
	/// Test program of CM.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class CMEngineTest
	{
		public const string ConnectionString = "";

		// set to 1 to disable most tests
		static int RUN_CODE = 3;

		static string[][] queries = new string[47][]
		{
			new string[] {"0", "290", "for $c in document(\"db://Service.xml\")//Customer return <Customer> {$c/Name} {$c/Contact} {$c/Grade}</Customer>"},
			new string[] {"1", "216", "document(\"db://Service.xml\")//AC[ID != \"A001\" and Power > 2]"},
			new string[] {"2", "1446", "for $s in document(\"db://Service.xml\")//ServiceRecord where $s/@customer=>Customer/Name=\"广州电信\" return $s"},
			new string[] {"3", "1446", "for $c in document(\"db://Service.xml\")//Customer let $s := $c/@services=>ServiceRecord where $c/Name=\"广州电信\" return $s"},
			new string[] {"4", "775", "for $c in document(\"db://Service.xml\")//ServiceRecord return <ServiceRecord><ID>{text($c/ID)}</ID><Who>{text($c/@customer=>Customer/Name)}</Who></ServiceRecord>"},
			new string[] {"5", "549", "for $c in document(\"db://Service.xml\")//Customer where $c/Name in (\"广州电信\", \"深圳电信局\") return $c"},			
			new string[] {"6", "1446", "document(\"db://Service.xml\")//ServiceRecord[@customer=>Customer/Name = \"广州电信\"]"},
			new string[] {"7", "1090", "document(\"db://Service.xml\")//ServiceRecord[Cost > 3000 and @customer=>Customer/Name = \"广州电信\"]"},
			new string[] {"8", "1090", "for $i in document(\"db://Service.xml\")//ServiceRecord where $i/Cost > 3000 and $i/@customer=>Customer/Name = \"广州电信\" return $i"},
			new string[] {"9", "2869", "for $s in document(\"db://Service.xml\")//ServiceRecord where $s/ID != \"0100001\" or (($s/Status = \"完成\" or $s/Status = \"进行中\") and $s/Cost > 2000) return $s"},			
			new string[] {"10", "1090", "document(\"db://Service.xml\")//Customer[Name = \"广州电信\"]/@services=>ServiceRecord[Cost > 3000]"},
			new string[] {"11", "1807", "for $s in document(\"db://Service.xml\")//ServiceRecord[1 to 5] return $s sortby ($s/ID, $s/Cost descending)"},
			new string[] {"12", "1713", "document(\"db://Service.xml\")//ServiceRecord[1 to 5] sortby (@customer=>Customer/Name)"},
			new string[] {"13", "379", "for $p in document(\"db://Service.xml\")//Product where $p/Name, like \"%空调%\" return $p"},
			new string[] {"14", "379", "for $p in document(\"db://Service.xml\")//Product where contains($p/Name, \"空调\") and $p/ID != \"1001\" return $p"},
			new string[] {"15", "654", "document(\"db://Service.xml\")//Product[contains(Description, 'office', 1) AND Price > 0] sortby (score1)"},
			new string[] {"16", "255", "for $c in document(\"db://Service.xml\")//Customer let $a := avg($c/@services=>ServiceRecord/Cost) return <Customer><Name>{text($c/Name)}</Name><Avg>{$a}</Avg></Customer>"},
			new string[] {"17", "290", "document(\"db://Service.xml\")//Customer[@obj_id = \"8589934592\"]"},
			new string[] {"18", "362", "let $a := sum(document(\"db://Service.xml\")//Customer/@services=>ServiceRecord/Cost) for $c in document(\"db://Service.xml\")//Customer return <GoodCustomer><Sum>{$a}</Sum><Name>{text($c/Name)}</Name></GoodCustomer>"},
			new string[] {"19", "636", "<A>{for $c in document(\"db://Service.xml\")//Customer where count($c/@services=>ServiceRecord) > 2 return $c}</A>"},
			new string[] {"20", "87", "avg(document(\"db://Service.xml\")//Customer/@services=>ServiceRecord/Cost)"},
			new string[] {"21", "2869", "let $a := min(document(\"db://Service.xml\")//ServiceRecord/Cost) for $o in document(\"db://Service.xml\")//ServiceRecord where $o/Cost > $a return $o"},
			new string[] {"22", "197", "for $name in (\"广州电信\", \"深圳电信局\") return <CustomerInfo><Name>{$name}</Name><Cost>{max(document(\"db://Service.xml\")//Customer[Name = $name]/@services=>ServiceRecord/Cost)}</Cost></CustomerInfo>"},
			new string[] {"23", "3098", "<TroubleCustomers>{for $c in document(\"db://Service.xml\")//Customer let $services := $c/@services=>ServiceRecord return <Customer><Name>{text($c/Name)}</Name><Services>{for $service in $services where $service/Cost > 2000 return $service}</Services></Customer>}</TroubleCustomers>"},
			new string[] {"24", "1", "count(document(\"db://Service.xml\")//Customer[contains(Name, \"电信\")]/@services=>ServiceRecord[Cost > 2000]/@serviceman=>Employee)"},
			new string[] {"25", "932", "document(\"db://Service.xml\")//Customer"}, // test of class with self reference
			new string[] {"26", "247", "document(\"db://Service.xml\")//Employee"},  // test of one-to-one relationship
			new string[] {"27", "2940", "for $c in document(\"db://Service.xml\")//Customer return <CustInfo>{$c/Name}<Services>{for $s in $c/@services=>ServiceRecord where $s/Cost < 8000 return $s}</Services></CustInfo>"},				
			new string[] {"28", "681", "for $s in document(\"db://Service.xml\")//ServiceRecord return <Service><ID>{text($s/ID)}</ID><Cost>{if ($s/Cost > 2000) then ($s/Cost - 100) + 20 else $s/Cost + 10}</Cost></Service>"},
			new string[] {"29", "377", "for $s in document(\"db://Service.xml\")//ServiceRecord where $s/Status = null and $s/Cost != null return $s"},
			new string[] {"30", "370", "for $c in document(\"db://Service.xml\")//Customer return <Customer>{$c/Name, $c/Address}</Customer>"},
			new string[] {"31", "370", "for $c in document(\"db://Service.xml\")//Customer return <Customer>{$c/Name}{$c/Address}</Customer>"},
			new string[] {"32", "2189", "let $doc := document(\"db://Service.xml\") for $c in $doc//Customer[Contact != null] for $s in $doc//ServiceRecord[Status != $c/Contact] return <Item>{$c/Name, $s/ID}</Item>"},
			new string[] {"33", "375", "for $c in document(\"db://Service.xml\")//Customer return <Customer>{$c/Name, if ($c/Name = \"广州电信\") then $c/Telephone else $c/Address}</Customer>"},
			new string[] {"34", "349", "for $c in document(\"db://Service.xml\")//Customer return <Customer>{$c/Name, $c/Address} <Max>{max($c/@services=>ServiceRecord/Cost)}</Max></Customer>"},
			new string[] {"35", "549", "for $s in (\"广州电信\", \"深圳电信局\") for $c in document(\"db://Service.xml\")//Customer[Name = $s] return $c"},		
			new string[] {"36", "15", "for $p in document(\"db://Service.xml\")//Refrigirator where $p/Size > 100 return (setText($p/Size, \"120\"), updateInstance(document(\"db://Service.xml\"), $p))"},			
			new string[] {"37", "15", "let $p := [[<Product xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Switch\"><ID>C0004</ID><Name>250 门交换机</Name><Lines>250</Lines></Product>]] return addInstance(document(\"db://Service.xml\"), $p)"},				
			new string[] {"38", "15", "let $p := [[<Product obj_id=\"34359738369\" xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Switch\"><Lines xsi:nil=\"true\"/></Product>]] return updateInstance(document(\"db://Service.xml\"), $p)"},
			new string[] {"39", "15", "let $p := [[<Product obj_id=\"34359738369\" xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Switch\" customer=\"34359738434\"><TotalPrice>25000</TotalPrice><customer><Name>IBM</Name></customer></Product>]] return updateInstance(document(\"db://Service.xml\"), $p)"},
			new string[] {"40", "15", "let $p := [[<Product obj_id=\"34359738369\" xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Switch\"><Lines>50</Lines></Product>]] return updateInstance(document(\"db://Service.xml\"), $p)"},
			new string[] {"41", "15", "for $p in document(\"db://Service.xml\")//Switch where $p/Lines = 250 return deleteInstance(document(\"db://Service.xml\"), $p)"},
			new string[] {"42", "123", "for $s in document(\"db://Service.xml\")//ServiceRecord let $p := $s/@product=>Product return <ServiceRecord>{$s/ID}{$s/Description}<serviceman><obj_id>{text($s/@serviceman)}</obj_id></serviceman></ServiceRecord>"},
			new string[] {"43", "686", "for $c in document(\"db://Service.xml\")//Customer return <Customer {$c/@obj_id, $c/@xsi:type, $c/@attachments}>{$c/Name}</Customer>"},
			new string[] {"44", "322", "for $s in document(\"db://Service.xml\")//ServiceRecord let $sv := $s/@serviceman=>employee let $c := $s/@customer=>Customer let $d := $s/@department=>department let $p := $s/@product=>Product where $s/@obj_id=\"12884901909\" return <ServiceRecord>{$s/ID}{$s/Description} <serviceman>{$sv/ID}</serviceman><customer>{$c/Name}</customer><department>{$d/ID}</department><product>{$p/ID}</product></ServiceRecord>"},
			new string[] {"45", "15", "let $p := [[<ServiceRecord xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"ServiceRecord\"><ID>88888</ID><Description>Just test</Description><customer><Name>IBM</Name></customer></ServiceRecord>]] return addInstance(document(\"db://Service.xml\"), $p)"},				
			new string[] {"46", "15", "for $p in document(\"db://Service.xml\")//ServiceRecord where $p/ID = \"88888\" return deleteInstance(document(\"db://Service.xml\"), $p)"},
			//new string[] {"46", "15", "let $e := [[<Employee obj_id=\"446676598784\" xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Employee\"><Name>Test1</Name></Employee>]] return addInstance(document(\"db://NewTest.xml\"), $e)"},
		};

		public static void Main()
		{
			// test of XQueries
			TestCase0();
			TestCase1();
			TestCase2();
			TestCase3();
			TestCase4();
			TestCase5();
			TestCase6();
			TestCase7();
			TestCase8();
			TestCase9();
			TestCase10();
			TestCase11();
			TestCase12();
			TestCase13();
			TestCase14();
			TestCase15();
			TestCase16();
			TestCase17();
			TestCase18();
			TestCase19();
			TestCase20();
			TestCase21();
			TestCase22();
			TestCase23();
			TestCase24();
			TestCase25();
			TestCase26();
			TestCase27();
			TestCase28();
			TestCase29();
			TestCase30();
			TestCase31();
			TestCase32();
			TestCase33();
			TestCase34();
			TestCase35();
			TestCase36();
			TestCase37();
			TestCase38();
			TestCase39();
			TestCase40();
			TestCase41();
			TestCase42();
			TestCase43();
			TestCase44();
			TestCase45();
			TestCase46();

			return;
		}

		static void RunTest(int i, int code) {
			int length = int.Parse(queries[i][1]);
			if (code != RUN_CODE) {
				return;
			}

			try
			{
				using (CMConnection con = new CMConnection("SCHEMA_NAME=Service;SCHEMA_VERSION=1.0"))
				{
					con.Open();
					CMCommand cmd = con.CreateCommand();
					cmd.CommandText = queries[i][2];

					string str;
					if (i == 24) 
					{
						str = cmd.ExecuteScalar().ToString();
					} 
					else 
					{
						XmlReader reader = cmd.ExecuteXMLReader();
						DataSet ds = new DataSet();
						ds.ReadXml(reader);
						str = ds.GetXml();
					}

					if ((code & 1) != 0) 
					{
						System.Console.WriteLine("Case " + i + " results: " + str);
						System.Console.WriteLine("With length " + str.Length);
					}

					if (str.Length != length)
					{
						System.Console.WriteLine("Case " + i + " failed.");
					}
					else
					{
						System.Console.WriteLine("Case " + i + " succeeded.");
					}
				}
			}
			catch (Exception e) 
			{
				System.Console.WriteLine(e.Message);

				if (e.GetBaseException() != null)
				{
					System.Console.WriteLine(e.GetBaseException().StackTrace);
				}
				else
				{
					System.Console.WriteLine(e.StackTrace);
				}
			}
		}

		static public void TestCase0()
		{
			RunTest(0, 3);
		}

		static public void TestCase1()
		{
			RunTest(1, 1);
		}

		static public void TestCase2()
		{
			RunTest(2, 1);
		}

		static public void TestCase3()
		{
			RunTest(3, 1);
		}

		static public void TestCase4()
		{
			RunTest(4, 1);
		}

		static public void TestCase5()
		{
			RunTest(5, 1);
		}

		static public void TestCase6()
		{
			RunTest(6, 1);
		}

		static public void TestCase7()
		{
			RunTest(7, 1);
		}

		static public void TestCase8()
		{
			RunTest(8, 1);
		}

		static public void TestCase9()
		{
			RunTest(9, 1);
		}

		static public void TestCase10()
		{
			RunTest(10, 1);
		}

		static public void TestCase11()
		{
			RunTest(11, 1);
		}

		static public void TestCase12()
		{
			RunTest(12, 2);
		}

		static public void TestCase13()
		{
			RunTest(13, 1);
		}

		static public void TestCase14()
		{
			RunTest(14, 1);
		}

		static public void TestCase15()
		{
			RunTest(15, 2);
		}

		static public void TestCase16()
		{
			RunTest(16, 1);
		}

		static public void TestCase17()
		{
			RunTest(17, 1);
		}

		static public void TestCase18()
		{
			RunTest(18, 1);
		}

		static public void TestCase19()
		{
			RunTest(19, 1);
		}

		static public void TestCase20()
		{
			RunTest(20, 2);
		}

		static public void TestCase21()
		{
			RunTest(21, 1);
		}

		static public void TestCase22()
		{
			RunTest(22, 1);
		}

		static public void TestCase23()
		{
			RunTest(23, 1);
		}

		static public void TestCase24()
		{
			RunTest(24, 1);
		}

		static public void TestCase25()
		{
			RunTest(25, 1);
		}

		static public void TestCase26()
		{
			RunTest(26, 1);
		}

		static public void TestCase27()
		{
			RunTest(27, 1);
		}

		static public void TestCase28()
		{
			RunTest(28, 1);
		}

		static public void TestCase29()
		{
			RunTest(29, 1);
		}

		static public void TestCase30()
		{
			RunTest(30, 1);
		}

		static public void TestCase31()
		{
			RunTest(31, 1);
		}

		static public void TestCase32()
		{
			RunTest(32, 1);
		}

		static public void TestCase33()
		{
			RunTest(33, 1);
		}

		static public void TestCase34()
		{
			RunTest(34, 1);
		}

		static public void TestCase35()
		{
			RunTest(35, 1);
		}

		static public void TestCase36()
		{
			RunTest(36, 1);
		}

		static public void TestCase37()
		{
			RunTest(37, 1);
		}

		static public void TestCase38()
		{
			RunTest(38, 1);
		}

		static public void TestCase39()
		{
			RunTest(39, 1); // update relationship
		}

		static public void TestCase40()
		{
			RunTest(40, 1);
		}

		static public void TestCase41()
		{
			RunTest(41, 1);
		}

		static public void TestCase42()
		{
			// test of empty sub element
			RunTest(42, 1);
		}

		static public void TestCase43()
		{
			// test element attributes
			RunTest(43, 1);
		}

		static public void TestCase44()
		{
			// test of null foreign key value
			RunTest(44, 1);
		}

		static public void TestCase45()
		{
			// test of add an instance with a relationship
			RunTest(45, 1);
		}

		static public void TestCase46()
		{
			// test of delete an instance with a relationship
			RunTest(46, 1);
		}

		static private void TestXQuery()
		{
			CMConnection con = new CMConnection();

			try
			{
				con.Open();
				CMCommand cmd = con.CreateCommand();
				cmd.CommandText = "document(\"db://SERVICE.xml\")//Customer";

				XmlReader reader = cmd.ExecuteXMLReader();
				DataSet ds = new DataSet();
				ds.ReadXml(reader);
				System.Console.WriteLine(ds.GetXml());				
			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
		}
	}
}