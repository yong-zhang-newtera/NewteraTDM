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
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Tests;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.IO;
using DevExpress.Web.ASPxGridView.Cookies;
namespace DevExpress.Web.ASPxGridView.Tests {
	[TestFixture]
	public class CookiesTest {
		protected GridViewSEOProcessing CreateSEO() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			return new GridViewSEOProcessing(grid);
		}
		protected GridViewCookies CreateCookies() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			return new GridViewCookies(grid);
		}
		[Test]
		public void CheckEmpty() {
			GridViewSEOProcessing seo = CreateSEO();
			Assert.AreEqual(true, seo.IsEmpty);
			seo.LoadState("page3|");
			Assert.AreEqual(false, seo.IsEmpty);
		}
		[Test]
		public void ChangePaging() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("page3|");
			Assert.AreEqual(2, seo.PageIndex);
			seo.LoadState("page4");
			Assert.AreEqual(3, seo.PageIndex);
		}
		[Test]
		public void CheckShowAllPagers() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("page0|");
			Assert.AreEqual(-1, seo.PageIndex);
			Assert.AreEqual(false, seo.IsEmpty);
			seo.LoadState("page");
			Assert.AreEqual(-2, seo.PageIndex);
			Assert.AreEqual(true, seo.IsEmpty);
		}
		[Test]
		public void ReadSorting1() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("page2|sort2|a0|d2");
			Assert.AreEqual(2, seo.ColumnsState.SortList.Count);
			Assert.AreEqual(0, seo.ColumnsState.SortList[0].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Ascending, seo.ColumnsState.SortList[0].SortOrder);
			Assert.AreEqual(2, seo.ColumnsState.SortList[1].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Descending, seo.ColumnsState.SortList[1].SortOrder);
		}
		[Test]
		public void ReadSorting() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("sort2|a0|d2|");
			Assert.AreEqual(2, seo.ColumnsState.SortList.Count);
			Assert.AreEqual(0, seo.ColumnsState.SortList[0].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Ascending, seo.ColumnsState.SortList[0].SortOrder);
			Assert.AreEqual(2, seo.ColumnsState.SortList[1].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Descending, seo.ColumnsState.SortList[1].SortOrder);
		}
		[Test]
		public void ReadGrouping() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("group1|sort2|a0|d2|");
			Assert.AreEqual(1, seo.ColumnsState.GroupCount);
			Assert.AreEqual(2, seo.ColumnsState.SortList.Count);
			Assert.AreEqual(0, seo.ColumnsState.SortList[0].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Ascending, seo.ColumnsState.SortList[0].SortOrder);
			Assert.AreEqual(2, seo.ColumnsState.SortList[1].Column.Index);
			Assert.AreEqual(ColumnSortOrder.Descending, seo.ColumnsState.SortList[1].SortOrder);
		}
		[Test]
		public void ReadFilterExpression() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.LoadState("filtercolumn1='a'");
			Assert.AreEqual("column1='a'", seo.FilterExpression);
		}
		[Test]
		public void ReadFilterEnabled() {
			GridViewSEOProcessing seo = CreateSEO();
			Assert.AreEqual(true, seo.FilterEnabled);
			seo.LoadState("fltenabledfalse");
			Assert.AreEqual(false, seo.FilterEnabled);
		}
		[Test]
		public void WritePaging() {
			GridViewSEOProcessing seo = CreateSEO();
			Assert.AreEqual("page5", seo.SaveState(4));
		}
		[Test]
		public void WriteSorting() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.Grid.DataColumns[0].SortOrder = ColumnSortOrder.Descending;
			seo.Grid.DataColumns[3].SortOrder = ColumnSortOrder.Ascending;
			Assert.AreEqual("page5|sort2|d0|a3", seo.SaveState(4));
		}
		[Test]
		public void WriteGrouping() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.Grid.DataColumns[3].SortOrder = ColumnSortOrder.Descending;
			seo.Grid.GroupBy(seo.Grid.DataColumns[0]);
			Assert.AreEqual("page5|group1|sort2|a0|d3", seo.SaveState(4));
		}
		[Test]
		public void WriteReadWidths() {
			GridViewCookies cookies = CreateCookies();
			ASPxGridView grid = cookies.Grid;
			grid.Columns[1].Width = Unit.Percentage(50);
			grid.Columns[2].Width = Unit.Pixel(25);
			string state = cookies.SaveState(1);
			foreach(GridViewColumn column in grid.Columns) {
				column.Width = Unit.Pixel(100);
			}
			cookies.LoadState(state);
			cookies.ColumnsState.Apply();
			Assert.AreEqual(Unit.Empty, grid.Columns[0].Width);
			Assert.AreEqual(Unit.Percentage(50), grid.Columns[1].Width);
			Assert.AreEqual(Unit.Pixel(25), grid.Columns[2].Width);
			Assert.AreEqual(Unit.Empty, grid.Columns[3].Width);
		}
		[Test]
		public void WriteReadVisible() {
			GridViewCookies cookies = CreateCookies();
			ASPxGridView grid = cookies.Grid;
			grid.Columns[2].Visible = false;
			grid.Columns[1].VisibleIndex = 0;
			string state = cookies.SaveState(1);
			foreach(GridViewColumn column in grid.Columns) {
				column.Visible = false;
				column.VisibleIndex = -1;
			}
			cookies.LoadState(state);
			cookies.ColumnsState.Apply();
			Assert.AreEqual(true, grid.Columns[0].Visible);
			Assert.AreEqual(true, grid.Columns[1].Visible);
			Assert.AreEqual(false, grid.Columns[2].Visible);
			Assert.AreEqual(true, grid.Columns[3].Visible);
			Assert.AreEqual(1, grid.Columns[0].VisibleIndex);
			Assert.AreEqual(0, grid.Columns[1].VisibleIndex);
			Assert.AreEqual(2, grid.Columns[3].VisibleIndex);
		}
		[Test]
		public void DontStoreWidth() {
			GridViewCookies cookies = CreateCookies();
			ASPxGridView grid = cookies.Grid;
			grid.Columns[1].Width = Unit.Percentage(50);
			grid.Columns[2].Width = Unit.Pixel(25);
			string state = cookies.SaveState(1);
			foreach(GridViewColumn column in grid.Columns) {
				column.Width = Unit.Pixel(100);
			}
			grid.SettingsCookies.StoreColumnsWidth = false;
			cookies.LoadState(state);
			cookies.ColumnsState.Apply();
			foreach(GridViewColumn column in grid.Columns) {
				Assert.AreEqual(Unit.Pixel(100), column.Width);
			}
		}
		[Test]
		public void NewVersion() {
			GridViewCookies cookies = CreateCookies();
			ASPxGridView grid = cookies.Grid;
			string state = cookies.SaveState(4);
			grid.SettingsCookies.Version = "1.0";
			cookies.LoadState(state);
			Assert.AreEqual(-2, cookies.PageIndex);
			state = cookies.SaveState(4);
			cookies.LoadState(state);
			Assert.AreEqual(4, cookies.PageIndex);
			state = cookies.SaveState(2);
			grid.SettingsCookies.Version = "1.1";
			cookies.LoadState(state);
			Assert.AreEqual(-2, cookies.PageIndex);
		}
		[Test]
		public void DontStorePageIndex() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsCookies.StorePaging = false;
			grid.Columns[1].Width = Unit.Percentage(50);
			grid.PageIndex = 2;
			string state = grid.SaveClientLayout();
			grid.Columns[1].Width = Unit.Empty;
			grid.PageIndex = 3;
			grid.LoadClientLayout(state);
			Assert.AreEqual(3, grid.PageIndex);
		}
		[Test]
		public void WriteFilterConditions() {
			GridViewSEOProcessing seo = CreateSEO();
			seo.Grid.DataColumns[0].Settings.AutoFilterCondition = AutoFilterCondition.Default;
			seo.Grid.DataColumns[2].Settings.AutoFilterCondition = AutoFilterCondition.NotEqual;
			Assert.AreEqual("page1|conditions1|2|" + ((int)AutoFilterCondition.NotEqual).ToString(), seo.SaveState(0));
		}
		[Test]
		public void ReadFilterConditions() {
			GridViewSEOProcessing seo = CreateSEO();
			string state = string.Format("conditions2|0|{0}|3|{1}", (int)AutoFilterCondition.Greater, (int)AutoFilterCondition.Less);
			seo.LoadState(state);
			seo.ColumnsState.Apply();
			Assert.AreEqual(AutoFilterCondition.Greater, seo.Grid.DataColumns[0].Settings.AutoFilterCondition);
			Assert.AreEqual(AutoFilterCondition.Less, seo.Grid.DataColumns[3].Settings.AutoFilterCondition);
		}
		[Test]
		public void B97070() {
			const string expr = "[Test] = 1";
			ASPxGridView grid = new ASPxGridView();
			grid.SettingsCookies.StoreFiltering = false;
			grid.FilterExpression = expr;
			grid.LoadClientLayout("page2");
			Assert.AreEqual(expr, grid.FilterExpression);
		}
	}
}
#endif
