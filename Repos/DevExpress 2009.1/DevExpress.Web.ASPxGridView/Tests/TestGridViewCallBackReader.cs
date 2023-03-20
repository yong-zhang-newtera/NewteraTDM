#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxGridView                                 }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2009 Developer Express Inc.

#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DevExpress.Web.ASPxGridView.Helper;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxGridView.Tests {
	[TestFixture]
	public class TestCallbackArgumentsReader {
		[Test]
		public void OnArgumentTest() {
			CallbackArgumentsReader reader = new CallbackArgumentsReader("CB|5;12345;", new string[] { "CB" });
			Assert.AreEqual("12345", reader["CB"]);
			Assert.AreEqual(null, reader["GB"]);
		}
		[Test]
		public void TwoArgumentTest() {
			CallbackArgumentsReader reader = new CallbackArgumentsReader("CB|5;12345;SR|4;TTFF;", new string[] { "CB", "SR" });
			Assert.AreEqual("12345", reader["CB"]);
			Assert.AreEqual("TTFF", reader["SR"]);
			Assert.AreEqual(null, reader["GB"]);
		}
		[Test]
		public void TwoArgumentReverceTest() {
			CallbackArgumentsReader reader = new CallbackArgumentsReader("CB|5;12345;SR|4;TTFF;", new string[] { "SR", "CB" });
			Assert.AreEqual("12345", reader["CB"]);
			Assert.AreEqual("TTFF", reader["SR"]);
			Assert.AreEqual(null, reader["GB"]);
		}
		[Test]
		public void IntTest() {
			CallbackArgumentsReader reader = new CallbackArgumentsReader("CB|5;12345;", new string[] { "CB" });
			Assert.AreEqual(12345, reader.GetIndexValue("CB"));
			Assert.AreEqual(-1, reader.GetIndexValue("GB"));
		}
		[Test]
		public void UnitArrayReadTest1() {
			List<Unit> list = CallbackArgumentsReader.ReadUnitArray("|50");
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(Unit.Empty, list[0]);
			Assert.AreEqual(Unit.Pixel(50), list[1]);
		}
		[Test]
		public void UnitArrayReadPercentTest() {
			List<Unit> list = CallbackArgumentsReader.ReadUnitArray("we|25%|345px");
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(Unit.Empty, list[0]);
			Assert.AreEqual(Unit.Percentage(25), list[1]);
			Assert.AreEqual(Unit.Pixel(345), list[2]);
		}
	}
	public class GridViewCallBackEditorValuesReaderTester : GridViewCallBackEditorValuesReader {
		public GridViewCallBackEditorValuesReaderTester(string text) : base(text) { }
		public int GetNumberFromText(int pos, char ch) {
			Pos = pos;
			return GetNumberFromText(ch);
		}
		public int GetNumberFromText(char ch) {
			return GetNumber(ch);
		}
	}
	[TestFixture]
	public class TestGridViewCallBackEditorValuesReader {
		public static GridViewCallBackEditorValuesReaderTester CreateTester(string text) {
			return new GridViewCallBackEditorValuesReaderTester(text);
		}
		[Test]
		public void GetNumberTest() {
			GridViewCallBackEditorValuesReaderTester tester = CreateTester("1;5,6,123456;ddfdf");
			Assert.AreEqual(1, tester.GetNumberFromText(0, ';'));
			Assert.AreEqual(5, tester.GetNumberFromText(','));
			Assert.AreEqual(6, tester.GetNumberFromText(','));
		}
		[Test]
		public void OneEditorValue() {
			GridViewCallBackEditorValuesReaderTester tester = CreateTester("1;2,5,12345;CB|Test");
			tester.Proccess();
			Assert.AreEqual(1, tester.Values.Count);
			Assert.AreEqual(2, tester.Values[0].ColumnIndex);
			Assert.AreEqual("12345", tester.Values[0].Value);
		}
		[Test]
		public void TwoEditorValue() {
			GridViewCallBackEditorValuesReaderTester tester = CreateTester("2;2,3,123;4,6,abcdef;CB|Test");
			tester.Proccess();
			Assert.AreEqual(2, tester.Values.Count);
			Assert.AreEqual(2, tester.Values[0].ColumnIndex);
			Assert.AreEqual("123", tester.Values[0].Value);
			Assert.AreEqual(4, tester.Values[1].ColumnIndex);
			Assert.AreEqual("abcdef", tester.Values[1].Value);
		}
		[Test]
		public void NullEditorValue() {
			GridViewCallBackEditorValuesReaderTester tester = CreateTester("2;2,3,123;4,-1,;CB|Test");
			tester.Proccess();
			Assert.AreEqual(2, tester.Values.Count);
			Assert.AreEqual(2, tester.Values[0].ColumnIndex);
			Assert.AreEqual("123", tester.Values[0].Value);
			Assert.AreEqual(4, tester.Values[1].ColumnIndex);
			Assert.AreEqual(null, tester.Values[1].Value);
		}
		[Test]
		public void FirstNullEditorValue() {
			GridViewCallBackEditorValuesReaderTester tester = CreateTester("2;4,-1,;2,3,123;CB|Test");
			tester.Proccess();
			Assert.AreEqual(2, tester.Values.Count);
			Assert.AreEqual(4, tester.Values[0].ColumnIndex);
			Assert.AreEqual(null, tester.Values[0].Value);
			Assert.AreEqual(2, tester.Values[1].ColumnIndex);
			Assert.AreEqual("123", tester.Values[1].Value);
		}
	}
}
#endif
