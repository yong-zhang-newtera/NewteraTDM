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
using NUnit.Framework;
using System.Web.UI;
using DevExpress.Web.ASPxGridView;
using DevExpress.Data;
using DevExpress.Web.Data;
using System.Reflection;
using DevExpress.Web.ASPxGridView.Rendering;
namespace DevExpress.Web.ASPxGridView.Tests {
	[System.ComponentModel.ToolboxItem(false)]
	public class ASPxGridViewEditFormLayoutTester : DevExpress.Web.ASPxGridView.ASPxGridView {
		public ASPxGridViewEditFormLayoutTester(int gridColumnCount, int editFormColumnCount) {
			CreateColumns(gridColumnCount);
			ColumnCount = editFormColumnCount;
		}
		public int ColumnCount { get { return SettingsEditing.EditFormColumnCount; } set { SettingsEditing.EditFormColumnCount = value; } }
		public void CreateColumns(int gridColumnCount) {
			Columns.Clear();
			for(int i = 0; i < gridColumnCount; i++) {
				GridViewDataColumn column = new GridViewDataColumn();
				Columns.Add(column);
				column.Caption = string.Format("col {0}", i);
				column.EditFormSettings.CaptionLocation = ASPxColumnCaptionLocation.None;
			}
		}
		public GridViewEditFormLayout GetLayout() {
			return new GridViewEditFormLayout(this);
		}
		public void SetColumnDefaultCaptionLocation() {
			SetColumnCaptionLocation(ASPxColumnCaptionLocation.Default);
		}
		public void SetColumnCaptionLocation(ASPxColumnCaptionLocation location) {
			foreach(GridViewDataColumn column in DataColumns) {
				column.EditFormSettings.CaptionLocation = location;
			}
		}
	}
	[TestFixture]
	public class ASPxGridViewEditFormLayoutTest {
		[Test]
		public void SimpleOneColumnTest() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(5, 1);
			Assert.AreEqual(5, tester.GetLayout().RowCount);
		}
		[Test]
		public void SimpleTwoColumnTest() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 2);
			Assert.AreEqual(3, tester.GetLayout().RowCount);
		}
		[Test]
		public void VisibleColumnTest() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 1);
			Assert.AreEqual(6, tester.GetLayout().RowCount);
			tester.DataColumns[0].EditFormSettings.Visible = DevExpress.Web.ASPxClasses.DefaultBoolean.False;
			Assert.AreEqual(5, tester.GetLayout().RowCount);
			tester.DataColumns[1].Visible = false;
			Assert.AreEqual(4, tester.GetLayout().RowCount);
		}
		[Test]
		public void SortColumnTest() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 2);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			tester.DataColumns[1].EditFormSettings.VisibleIndex = 0;
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(0, 0));
		}
		[Test]
		public void RowSpanOneColumnTest() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 1);
			tester.DataColumns[1].EditFormSettings.RowSpan = 5;
			Assert.AreEqual(10, tester.GetLayout().RowCount);
		}
		[Test]
		public void RowSpanTwoColumnsTest1() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 2);
			tester.DataColumns[0].EditFormSettings.RowSpan = 5;
			Assert.AreEqual(5, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(1, 1));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(1, 2));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(1, 3));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(1, 4));
		}
		[Test]
		public void RowSpanAnd3ColumnsTest1() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 3);
			tester.DataColumns[0].EditFormSettings.RowSpan = 4;
			tester.DataColumns[1].EditFormSettings.RowSpan = 3;
			Assert.AreEqual(4, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(2, 0));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(2, 1));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(2, 2));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(1, 3));
		}
		[Test]
		public void RowSpanAnd3ColumnsTest2() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(7, 3);
			tester.DataColumns[1].EditFormSettings.RowSpan = 3;
			Assert.AreEqual(3, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(2, 0));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(0, 1));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(2, 1));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(0, 2));
			Assert.AreEqual(tester.DataColumns[6], tester.GetLayout().GetEditColumn(2, 2));
		}
		[Test]
		public void ColSpanAnd2ColumnsTest1() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(5, 2);
			tester.DataColumns[0].EditFormSettings.ColumnSpan = 2;
			Assert.AreEqual(3, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(0, 1));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(1, 1));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(0, 2));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(1, 2));
		}
		[Test]
		public void ComplexLayoutTest1() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(4, 3);
			tester.DataColumns[0].EditFormSettings.RowSpan = 2;
			tester.DataColumns[1].EditFormSettings.ColumnSpan = 2;
			tester.DataColumns[2].EditFormSettings.ColumnSpan = 2;
			tester.DataColumns[2].EditFormSettings.ColumnSpan = 2;
			Assert.AreEqual(3, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(1, 1));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(0, 2));
		}
		[Test]
		public void ComplexLayoutTest2() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 3);
			tester.DataColumns[2].EditFormSettings.RowSpan = 4;
			Assert.AreEqual(4, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(2, 0));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(0, 1));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(1, 1));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(0, 2));
		}
		[Test]
		public void ComplexLayoutTest3() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 3);
			tester.DataColumns[2].EditFormSettings.RowSpan = 3;
			tester.DataColumns[3].EditFormSettings.ColumnSpan = 2;
			tester.DataColumns[4].EditFormSettings.ColumnSpan = 3;
			Assert.AreEqual(5, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(2, 0));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(0, 1));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(0, 3));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(0, 4));
		}
		[Test]
		public void ComplexLayoutTest4() {
			ASPxGridViewEditFormLayoutTester tester = new ASPxGridViewEditFormLayoutTester(6, 3);
			tester.DataColumns[2].EditFormSettings.ColumnSpan = 3;
			tester.DataColumns[2].EditFormSettings.VisibleIndex = 100;
			Assert.AreEqual(3, tester.GetLayout().RowCount);
			Assert.AreEqual(tester.DataColumns[0], tester.GetLayout().GetEditColumn(0, 0));
			Assert.AreEqual(tester.DataColumns[1], tester.GetLayout().GetEditColumn(1, 0));
			Assert.AreEqual(tester.DataColumns[3], tester.GetLayout().GetEditColumn(2, 0));
			Assert.AreEqual(tester.DataColumns[4], tester.GetLayout().GetEditColumn(0, 1));
			Assert.AreEqual(tester.DataColumns[5], tester.GetLayout().GetEditColumn(1, 1));
			Assert.AreEqual(tester.DataColumns[2], tester.GetLayout().GetEditColumn(0, 2));
		}
	}
}
#endif
