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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.Data;
using System.Drawing;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class GridViewEditFormLayout {
		ASPxGridView grid;
		List<GridViewDataColumn> columns;
		Dictionary<Point, GridViewDataColumn> table;
		public GridViewEditFormLayout(ASPxGridView grid) {
			this.grid = grid;
			this.columns = new List<GridViewDataColumn>();
			this.table = new Dictionary<Point, GridViewDataColumn>();
			BuildLayout();
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected List<GridViewDataColumn> Columns { get { return columns; } }
		protected Dictionary<Point, GridViewDataColumn> Table { get { return table; } }
		public int RowCount { get { return GetRowCount(); } }
		public int ColumnCount { get { return Grid.SettingsEditing.EditFormColumnCount; } }
		public GridViewDataColumn GetEditColumn(int columnIndex, int rowIndex) {
			return GetColumnByPoint(new Point(columnIndex, rowIndex));
		}
		public ASPxColumnCaptionLocation GetCaptionLocation(GridViewDataColumn editColumn) {
			ASPxColumnCaptionLocation location = editColumn.EditFormSettings.CaptionLocation;
			if(location == ASPxColumnCaptionLocation.Default) {
				if(GetRowSpan(editColumn) > 1) return ASPxColumnCaptionLocation.Top;
				return ASPxColumnCaptionLocation.Near;
			}
			return location;
		}
		public int GetRowSpan(GridViewDataColumn column) {
			int res = column.EditFormSettings.RowSpan;
			return res > 0 ? res : 1;
		}
		public int GetColSpan(GridViewDataColumn column) {
			int res = Math.Min(column.EditFormSettings.ColumnSpan, ColumnCount);
			return res > 0 ? res : 1;
		}
		public bool HasEditColumn(int rowIndex) {
			for(int i = 0; i < ColumnCount; i++) {
				if(GetEditColumn(i, rowIndex) != null) return true;
			}
			return false;
		}
		protected GridViewDataColumn GetColumnByPoint(Point pt) {
			if(!Table.ContainsKey(pt)) return null;
			return Table[pt];
		}
		protected int GetRowCount() {
			int y = 0;
			foreach(KeyValuePair<Point, GridViewDataColumn> pair in Table) {
				y = Math.Max(y, pair.Key.Y + GetRowSpan(pair.Value));
			}
			return y;
		}
		protected virtual void BuildLayout() {
			Clear();
			BuildColumns();
			BuildTable();
		}
		protected virtual void Clear() {
			Columns.Clear();
			Table.Clear();
		}
		protected virtual void BuildColumns() {
			foreach(GridViewDataColumn column in grid.DataColumns) {
				if(ColumnIsEditable(column)) Columns.Add(column);
			}
			columns.Sort(new Comparison<GridViewDataColumn>(CompareColumnVisibleIndexes));
		}
		int CompareColumnVisibleIndexes(GridViewDataColumn col1, GridViewDataColumn col2) {
			if(col1 == col2) return 0;
			int v1 = col1.EditFormSettings.VisibleIndex < 0 ? col1.VisibleIndex : col1.EditFormSettings.VisibleIndex;
			int v2 = col2.EditFormSettings.VisibleIndex < 0 ? col2.VisibleIndex : col2.EditFormSettings.VisibleIndex;
			if(v1 == v2) {
				if(col1.EditFormSettings.VisibleIndex == col2.EditFormSettings.VisibleIndex)
					return System.Collections.Comparer.Default.Compare(col1.Index, col2.Index);
				if(col1.EditFormSettings.VisibleIndex < 0) return 1;
				if(col2.EditFormSettings.VisibleIndex < 0) return -1;
				return System.Collections.Comparer.Default.Compare(col1.EditFormSettings.VisibleIndex, col2.EditFormSettings.VisibleIndex);
			}
			return System.Collections.Comparer.Default.Compare(v1, v2);
		}
		protected virtual void BuildTable() {
			int x = 0, y = 0;
			foreach(GridViewDataColumn column in Columns) {
				FindNearestEmptyPoint(column, ref x, ref y);
				Table.Add(new Point(x, y), column);
				x += GetColSpan(column);
				x = GetColumnX(x, y);
			}
		}
		int GetColumnX(int x, int y) {
			GridViewDataColumn column = GetColumnByContainingPoint(new Point(x, y));
			while(column != null) {
				x += GetColSpan(column);
				column = GetColumnByContainingPoint(new Point(x, y));
			}
			return x;
		}
		GridViewDataColumn GetColumnByContainingPoint(Point pt) {
			foreach(KeyValuePair<Point, GridViewDataColumn> pair in Table) {
				System.Drawing.Rectangle r = new System.Drawing.Rectangle(pair.Key,
							new System.Drawing.Size(GetColSpan(pair.Value), GetRowSpan(pair.Value)));
				if(pt.X == r.Right || pt.Y == r.Bottom) continue;
				if(r.Contains(pt)) return pair.Value;
			}
			return null;
		}
		void FindNearestEmptyPoint(GridViewDataColumn column, ref int x, ref int y) {
			while(true) {
				while(x > 0 && x >= ColumnCount) {
					y ++;
					x = GetColumnX(0, y);
				}
				if(GetColumnByContainingPoint(new Point(x, y)) != null)
					x++;
				else {
					if(IsColumnFit(ref x, y, GetColSpan(column))) 
						return;
				}
			}
		}
		bool IsColumnFit(ref int x, int y, int columnSpan) {
			if(columnSpan == 1) return true;
			if(columnSpan > ColumnCount - x) {
				x++;
				return false;
			}
			for(int i = 1; i < columnSpan; i++) {
				if(GetColumnByContainingPoint(new Point(x + i, y)) != null) {
					x += i;
					return false;
				}
			}
			return true;
		}
		protected virtual bool ColumnIsEditable(GridViewDataColumn column) {
			if(column.EditFormSettings.Visible == DefaultBoolean.False) return false;
			if(column.EditFormSettings.Visible == DefaultBoolean.True) return true;
			return column.Visible;
		}
	}
}
