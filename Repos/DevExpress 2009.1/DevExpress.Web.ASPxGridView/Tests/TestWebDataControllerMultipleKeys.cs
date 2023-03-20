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
using System.Data;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.Data;
using NUnit.Framework;
using DevExpress.Data;
using System.IO;
using DevExpress.Data.IO;
using DevExpress.Web.ASPxGridView.Tests;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using System.ComponentModel;
using System.Web.UI;
namespace DevExpress.Web.ASPxGridView.Tests {
	public class WebDataProxyMultipleKeysTester : WebDataProxyTester {
		public WebDataProxyMultipleKeysTester() : this(20) { }
		public WebDataProxyMultipleKeysTester(int rowCount) : base(rowCount, new Type[] { typeof(string), typeof(string), typeof(int), typeof(DateTime) }) { }
		protected override object[] GetNewValues() {
			object[] values = new object[Table.Columns.Count];
			for (int i = 0; i < values.Length; i++) {
				values[i] = GetCellValue(i);
			}
			return values;
		}
		protected override void AddKeyField() {
		}
		protected override void SetKeyField() {
			KeyFieldName = "column0;column2";
		}
	}
	[TestFixture]
	public class MultipleKeysTests {
		[Test]
		public void HasCorrectKeyFieldName() {
			WebDataProxyMultipleKeysTester data = new WebDataProxyMultipleKeysTester();
			Assert.IsTrue(data.HasCorrectKeyFieldName);
			data.KeyFieldName = ";";
			Assert.IsFalse(data.HasCorrectKeyFieldName);
			data.KeyFieldName = "column0;";
			Assert.IsTrue(data.HasCorrectKeyFieldName);
		}
		[Test]
		public void KeyFieldNameProperty() {
			WebDataProxyMultipleKeysTester data = new WebDataProxyMultipleKeysTester();
			data.KeyFieldName = ";";
			Assert.AreEqual(string.Empty, data.KeyFieldName);
			Assert.IsFalse(data.IsMultipleKeyFields);
			data.KeyFieldName = "column0;";
			Assert.AreEqual("column0", data.KeyFieldName);
			Assert.IsFalse(data.IsMultipleKeyFields);
			data.KeyFieldName = "column0;column2;";
			Assert.AreEqual("column0;column2", data.KeyFieldName);
			Assert.IsTrue(data.IsMultipleKeyFields);
		}
		[Test]
		public void GetKeyValue() {
			WebDataProxyMultipleKeysTester data = new WebDataProxyMultipleKeysTester();
			Assert.AreEqual(data.GetRowValue(0, "column0").ToString() + "|" + data.GetRowValue(0, "column2").ToString(), data.GetRowKeyValue(0));
		}
		[Test]
		public void FindVisibleIndexByKey() {
			WebDataProxyMultipleKeysTester data = new WebDataProxyMultipleKeysTester();
			object value = data.GetRowKeyValue(3);
			Assert.AreEqual(3, data.FindVisibleIndexByKey(value, false));
		}
		[Test]
		public void FindVisibleIndexByKeyInCached() {
			WebDataProxyMultipleKeysTester data = new WebDataProxyMultipleKeysTester();
			object value = data.GetRowKeyValue(3);
			WebDataProxyTester tester = data.CreateFromStream();
			Assert.IsFalse(tester.IsBound);
			Assert.AreEqual(3, tester.FindVisibleIndexByKey(value, false));
			Assert.IsFalse(tester.IsBound);
		}
		int rowUpdatingCount = 0;
		int rowDeletingCount = 0;
		[Test]
		public void UpdateEdit() {
			WebDataProxyMultipleKeysTester tester = new WebDataProxyMultipleKeysTester();
			tester.RowUpdating += new ASPxDataUpdatingEventHandler(tester_RowUpdating);
			tester.StartEdit(3);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("column1", 100);
			tester.SetEditorValues(values, false);
			Assert.AreEqual(0, this.rowUpdatingCount);
			tester.UpdateRow(false);
			Assert.AreEqual(1, this.rowUpdatingCount);
		}
		[Test]
		public void DeleteRow() {
			WebDataProxyMultipleKeysTester tester = new WebDataProxyMultipleKeysTester();
			tester.RowDeleting += new ASPxDataDeletingEventHandler(tester_RowDeleting);
			tester.DeleteRow(2);
			Assert.AreEqual(1, this.rowDeletingCount);
		}
		void tester_RowUpdating(object sender, ASPxDataUpdatingEventArgs e) {
			Assert.AreEqual(2, e.Keys.Count);
			Assert.AreEqual("str1", e.Keys["column0"]);
			Assert.AreEqual(3, e.Keys["column2"]);
			this.rowUpdatingCount++;
			e.Cancel = true;
		}
		void tester_RowDeleting(object sender, ASPxDataDeletingEventArgs e) {
			Assert.AreEqual(2, e.Keys.Count);
			Assert.AreEqual("str0", e.Keys["column0"]);
			Assert.AreEqual(2, e.Keys["column2"]);
			this.rowDeletingCount++;
			e.Cancel = true;
		}
	}
}
#endif
