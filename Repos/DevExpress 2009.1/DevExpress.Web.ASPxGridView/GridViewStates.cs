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
using System.Text;
using DevExpress.Web.ASPxClasses;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Data;
using System.Collections;
using System.IO;
using DevExpress.Data.IO;
using DevExpress.Web.ASPxEditors;
using System.Collections.Specialized;
namespace DevExpress.Web.ASPxGridView.Cookies {
	public class GridViewColumnStateBase {
		GridViewColumn column;
		public GridViewColumnStateBase(GridViewColumn column) {
			this.column = column;
		}
		public GridViewColumn Column { get { return column; } }
	}
	public class GridViewColumnSortState : GridViewColumnStateBase {
		ColumnSortOrder sortOrder;
		public GridViewColumnSortState(GridViewDataColumn column, ColumnSortOrder sortOrder) : base(column) {
			this.sortOrder = sortOrder;
		}
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
	}
	public class GridViewColumnAutoFilterConditionState {
		GridViewDataColumn column;
		AutoFilterCondition condition;
		public GridViewColumnAutoFilterConditionState(GridViewDataColumn column, AutoFilterCondition condition) {
			this.column = column;
			this.condition = condition;
		}
		public GridViewDataColumn Column { get { return column; } }
		public AutoFilterCondition Condition { get { return condition; } }
		public void Apply() {
			Column.Settings.AutoFilterCondition = Condition;
		}
	}
	public class GridViewColumnState : GridViewColumnStateBase {
		Unit width;
		bool visible;
		int visibleIndex;
		public GridViewColumnState(GridViewColumn column) : base(column) {
			ResetWidth();
			ResetVisibilty();
		}
		public Unit Width { get { return width; } set { width = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public bool IsEmpty { get { return Width == Column.Width && Visible == Column.Visible && VisibleIndex == Column.VisibleIndex; } }
		public virtual void Apply() {
			if(IsEmpty) return;
			Column.Width = Width;
			Column.SetColVisible(Visible);
			Column.SetColVisibleIndex(VisibleIndex);
		}
		public void ResetWidth() {
			Width = Column.Width;
		}
		public void ResetVisibilty() {
			Visible = Column.Visible;
			VisibleIndex = Column.VisibleIndex;
		}
	}
	public class GridViewColumnsState {
		ASPxGridView grid;
		int groupCount;
		List<GridViewColumnSortState> sortList;
		List<GridViewColumnState> columnStates;
		List<GridViewColumnAutoFilterConditionState> filterConditionList;
		public GridViewColumnsState(ASPxGridView grid) {
			this.grid = grid;
			this.groupCount = 0;
			this.sortList = new List<GridViewColumnSortState>();
			this.columnStates = new List<GridViewColumnState>();
			foreach(GridViewColumn column in Grid.Columns) {
				ColumnStates.Add( new GridViewColumnState(column));
			}
			this.filterConditionList = new List<GridViewColumnAutoFilterConditionState>();
		}
		public ASPxGridView Grid { get { return grid; } }
		public int GroupCount { get { return Math.Min(this.groupCount, SortList.Count); } set { groupCount = value; } }
		public List<GridViewColumnSortState> SortList { get { return sortList; } }
		public List<GridViewColumnState> ColumnStates { get { return columnStates; } }
		public List<GridViewColumnAutoFilterConditionState> FilterConditionList { get { return filterConditionList; } }
		public virtual void Apply() {
			foreach(GridViewColumnState state in ColumnStates) {
				state.Apply();
			}
			for(int n = 0; n < SortList.Count; n++) {
				GridViewDataColumn column = SortList[n].Column;
				column.SetSortOrder(SortList[n].SortOrder);
				column.SetSortIndex(n);
				if(n < GroupCount) column.SetGroupIndex(n);
			}
			foreach(GridViewColumnAutoFilterConditionState state in FilterConditionList)
				state.Apply();
		}
		public virtual void ResetColumnsWidth() {
			foreach(GridViewColumnState state in ColumnStates) {
				state.ResetWidth();
			}
		}
		public virtual void ResetColumnsVisibility() {
			foreach(GridViewColumnState state in ColumnStates) {
				state.ResetVisibilty();
			}
		}
		public bool IsEmpty {
			get {
				if(SortList.Count > 0) return false;
				if(FilterConditionList.Count > 0) return false;
				foreach(GridViewColumnState state in ColumnStates) {
					if(!state.IsEmpty) return false;
				}
				return true;
			}
		}
		public void Read(TypedBinaryReader reader) {
			LoadColumnsVisibleState(reader);
			LoadColumnsWidths(reader);
			LoadColumnAutoFilterConditions(reader);
			LoadGroupingAndSorting(reader);
		}
		void LoadColumnsVisibleState(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int visibleIndex = reader.ReadObject<int>();
				bool visible = reader.ReadObject<bool>();
				if(n >= ColumnStates.Count) continue;
				ColumnStates[n].Visible = visible;
				ColumnStates[n].VisibleIndex = visibleIndex;
			}
		}
		void LoadColumnsWidths(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int index = reader.ReadObject<int>();
				UnitType type = (UnitType)reader.ReadObject<int>();
				double value = reader.ReadObject<double>();
				if(index >= 0 && index < ColumnStates.Count) {
					ColumnStates[index].Width = new Unit(value, type);
				}
			}
		}
		void LoadColumnAutoFilterConditions(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				int index = reader.ReadObject<int>();				
				AutoFilterCondition condition = (AutoFilterCondition)reader.ReadObject<int>();
				GridViewDataColumn column = index < Grid.Columns.Count ? Grid.Columns[index] as GridViewDataColumn : null;
				if(column != null)
					FilterConditionList.Add(new GridViewColumnAutoFilterConditionState(column, condition));
			}
		}
		void LoadGroupingAndSorting(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			this.groupCount = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int index = reader.ReadObject<int>();
				ColumnSortOrder sortOrder = (ColumnSortOrder)reader.ReadObject(typeof(int));
				ColumnSortOrder ungroupedSortOrder = (ColumnSortOrder)reader.ReadObject(typeof(int));
				GridViewDataColumn column = index < Grid.Columns.Count ? Grid.Columns[index] as GridViewDataColumn : null;
				if(column == null) continue;
				column.UngroupedSortOrder = ungroupedSortOrder;
				SortList.Add(new GridViewColumnSortState(column, sortOrder));
			}
		}
	}
	public class GridViewCookiesBase {
		ASPxGridView grid;
		const char divider = '|';
		protected const string PagePrefix = "page";
		protected const string GroupPrefix = "group";
		protected const string SortPrefix = "sort";
		protected const string FilterPrefix = "filter";
		protected const string FilterEnabledPrefix = "fltenabled";
		protected const string WidthPrefix = "width";
		protected const string VisiblePrefix = "visible";
		protected const string VersionPrefix = "version";
		protected const string FilterConditionsPrefix = "conditions";
		string filterExpression;
		bool filterEnabled = true;
		GridViewColumnsState columnsState;
		int pageIndex; 
		public GridViewCookiesBase(ASPxGridView grid) {
			this.grid = grid;
			this.columnsState = new GridViewColumnsState(grid);
			Clear();
		}
		public ASPxGridView Grid { get { return grid; } }
		public GridViewColumnsState ColumnsState { get { return columnsState; } }
		public int PageIndex { get { return pageIndex; } }
		public string FilterExpression { get { return filterExpression; } }
		public bool FilterEnabled { get { return filterEnabled; } }
		protected internal void SetPageIndex() {
			if(!StorePaging) return;
			Grid.PageIndex = PageIndex;
		}
		protected virtual bool StorePaging { get { return true; } }
		protected virtual bool StoreGroupingAndSorting { get { return true; } }
		protected virtual bool StoreFiltering { get { return true; } }
		protected virtual bool StoreWidth { get { return false; } }
		protected virtual bool StoreVisibility { get { return false; } }
		protected virtual string Version { get { return string.Empty; } }
		public string SaveState(int pageIndex) {
			StringBuilder sb = new StringBuilder();
			SaveState(sb, pageIndex);
			return sb.ToString();
		}
		protected virtual void SaveState(StringBuilder sb, int pageIndex) {
			if(!string.IsNullOrEmpty(Version)) {
				AppendFormat(sb, "{0}{1}", VersionPrefix, Version);
			}
			if(StorePaging) {
				AppendFormat(sb, "{0}{1}", PagePrefix, pageIndex + 1);
			}
			if(Grid.GroupCount > 0 && StoreGroupingAndSorting) {
				AppendFormat(sb, "{0}{1}", GroupPrefix, Grid.GroupCount);
			}
			if(Grid.SortCount > 0 && StoreGroupingAndSorting) {
				AppendFormat(sb, "{0}{1}", SortPrefix, Grid.SortCount);
				foreach(GridViewDataColumn column in grid.GetSortedColumns()) {
					sb.Append(string.Format("|{0}{1}", (column.SortOrder == ColumnSortOrder.Ascending) ? 'a' : 'd', column.Index));
				}
			}
			if(StoreFiltering) {
				if(!string.IsNullOrEmpty(Grid.FilterExpression)) {
					AppendFormat(sb, "{0}{1}", FilterPrefix, Grid.FilterExpression);
				}
				if(!Grid.FilterEnabled) {
					AppendFormat(sb, "{0}{1}", FilterEnabledPrefix, "false");
				}
				List<GridViewDataColumn> list = new List<GridViewDataColumn>();
				foreach(GridViewDataColumn column in Grid.DataColumns) {
					if(column.Settings.AutoFilterCondition != AutoFilterCondition.Default)
						list.Add(column);
				}
				if(list.Count > 0) {
					AppendFormat(sb, "{0}{1}", FilterConditionsPrefix, list.Count);
					foreach(GridViewDataColumn column in list) {
						sb.AppendFormat("|{0}|{1}", column.Index, (int)column.Settings.AutoFilterCondition);
					}
				}
			}
			if(StoreVisibility) {
				AppendFormat(sb, "{0}{1}", VisiblePrefix, Grid.Columns.Count);
				foreach(GridViewColumn column in Grid.Columns) {
					sb.Append(string.Format("|{0}{1}", column.Visible ? 't' : 'f', column.VisibleIndex));
				}
			}
			if(StoreWidth) {
				AppendFormat(sb, "{0}{1}", WidthPrefix, Grid.Columns.Count);
				foreach(GridViewColumn column in Grid.Columns) {
					sb.Append(string.Format("|{0}", column.Width.IsEmpty ? "e" : column.Width.ToString()));
				}
			}
		}
		void AppendFormat(StringBuilder sb, string format, params object[] args) {
			if(sb.Length > 0) sb.Append('|');
			sb.Append(string.Format(format, args));
		}
		public bool LoadState(string state) {
			Clear();
			if(string.IsNullOrEmpty(state)) return !IsEmpty;
			LoadStateCore(state);
			ClearUnsavedFields();
			return !IsEmpty;
		}
		protected virtual void LoadStateCore(string state) {
			this.pageIndex = ReadIndex(PagePrefix, ref state) - 1;
			ColumnsState.GroupCount = ReadIndex(GroupPrefix, ref state);
			ReadSortsInfo(ref state);
			this.filterExpression = ReadString(FilterPrefix, ref state);
			string filterEnabledSt = ReadString(FilterEnabledPrefix, ref state);
			if(!string.IsNullOrEmpty(filterEnabledSt)) {
				bool.TryParse(filterEnabledSt, out this.filterEnabled);
			}
			ReadFilterConditions(ref state);
			ReadVisibilitiesInfo(ref state);
			ReadColumnsWidth(ref state);
		}
		protected virtual void ClearUnsavedFields() {
			if(!StorePaging) {
				this.pageIndex = -2;
			}
			if(!StoreGroupingAndSorting) {
				this.ColumnsState.GroupCount = 0;
				ColumnsState.SortList.Clear();
			}
			if(!StoreFiltering) {
				this.filterExpression = string.Empty;
				ColumnsState.FilterConditionList.Clear();
			}
			if(!StoreVisibility) {
				ColumnsState.ResetColumnsVisibility();
			}
			if(!StoreWidth) {
				ColumnsState.ResetColumnsWidth();
			}
		}
		public virtual bool IsEmpty {
			get {
				return PageIndex < -1 && ColumnsState.IsEmpty && string.IsNullOrEmpty(FilterExpression);
			}
		}
		protected virtual void Clear() {
			this.columnsState = new GridViewColumnsState(Grid);
			this.pageIndex = -2;
			this.filterExpression = string.Empty;
		}
		protected void ReadSortsInfo(ref string state) {
			int sortCount = ReadIndex(SortPrefix, ref state);
			if(sortCount == 0) return;
			for(int i = 0; i < sortCount; i++) {
				GridViewColumnSortState sort = ReadSortInfo(ref state);
				if(sort == null) break;
				ColumnsState.SortList.Add(sort);
			}
		}
		GridViewColumnSortState ReadSortInfo(ref string state) {
			if(string.IsNullOrEmpty(state)) return null;
			ColumnSortOrder sortOrder = state[0] == 'a' ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
			state = state.Remove(0, 1);
			int columnIndex = ReadIndex(ref state);
			if(columnIndex < 0) return null;
			if(columnIndex >= Grid.Columns.Count) return null;
			GridViewDataColumn column = Grid.Columns[columnIndex] as GridViewDataColumn;
			if(column == null) return null;
			return new GridViewColumnSortState(column, sortOrder);
		}
		protected void ReadVisibilitiesInfo(ref string state) {
			int count = ReadIndex(VisiblePrefix, ref state);
			for(int i = 0; i < count; i++) {
				if(i >= ColumnsState.ColumnStates.Count) break;
				ReadColumnVisibility(ColumnsState.ColumnStates[i], ref state);
			}
		}
		void ReadColumnVisibility(GridViewColumnState columnState, ref string state) {
			if(string.IsNullOrEmpty(state)) return;
			columnState.Visible = state[0] == 't';
			state = state.Remove(0, 1);
			columnState.VisibleIndex = ReadIndex(ref state);
		}
		void ReadColumnsWidth(ref string state) {
			int count = ReadIndex(WidthPrefix, ref state);
			for(int i = 0; i < count; i++) {
				if(i >= ColumnsState.ColumnStates.Count) break;
				ReadColumnWidth(ColumnsState.ColumnStates[i], ref state);
			}
		}
		void ReadColumnWidth(GridViewColumnState columnState, ref string state) {
			string width = ReadString(ref state);
			if(!string.IsNullOrEmpty(width)) {
				columnState.Width = width == "e" ? Unit.Empty : Unit.Parse(width);
			}
		}
		void ReadFilterConditions(ref string state) {
			int count = ReadIndex(FilterConditionsPrefix, ref state);						
			for(int i = 0; i < count; i++) {
				GridViewColumnAutoFilterConditionState condState = ReadFilterCondition(ref state);
				if(condState != null)
					ColumnsState.FilterConditionList.Add(condState);
			}
		}
		GridViewColumnAutoFilterConditionState ReadFilterCondition(ref string state) {
			int columnIndex = ReadIndex(ref state);
			int value = ReadIndex(ref state);
			if(columnIndex >= Grid.Columns.Count) return null;
			GridViewDataColumn column = Grid.Columns[columnIndex] as GridViewDataColumn;
			if(column == null) return null;
			return new GridViewColumnAutoFilterConditionState(column, (AutoFilterCondition)value);
		}
		protected int ReadIndex(string prefixName, ref string state) {
			if(!state.StartsWith(prefixName)) return 0;
			state = state.Remove(0, prefixName.Length);
			return ReadIndex(ref state);
		}
		protected string ReadString(string prefixName, ref string state) {
			if(!state.StartsWith(prefixName)) return string.Empty;
			state = state.Remove(0, prefixName.Length);
			return ReadString(ref state);
		}
		int ReadIndex(ref string state) {
			string res = ReadString(ref state);
			int index = -1;
			if(!int.TryParse(res, out index)) return -1;
			return index;
		}
		string ReadString(ref string state) {
			int pos = state.IndexOf(divider);
			if(pos < 0) pos = state.Length;
			string result = state.Substring(0, pos);
			state = state.Remove(0, pos);
			if(!string.IsNullOrEmpty(state) && state[0] == divider) {
				state = state.Remove(0, 1);
			}
			return result;
		}
	}
	public class GridViewSEOProcessing : GridViewCookiesBase {
		public GridViewSEOProcessing(ASPxGridView grid) : base(grid) { }
	}
	public class GridViewCookies : GridViewCookiesBase {
		public GridViewCookies(ASPxGridView grid) : base(grid) { }
		protected ASPxGridViewCookiesSettings Settings { get { return Grid.SettingsCookies; } }
		protected override bool StorePaging { get { return Settings.StorePaging; } }
		protected override bool StoreGroupingAndSorting { get { return Settings.StoreGroupingAndSorting; } }
		protected override bool StoreFiltering { get { return Settings.StoreFiltering; } }
		protected override bool StoreWidth { get { return Settings.StoreColumnsWidth; } }
		protected override bool StoreVisibility { get { return Settings.StoreColumnsVisiblePosition; } }
		protected override string Version { get { return Settings.Version; } }
		protected override void LoadStateCore(string state) {
			string oldVersion = ReadString(VersionPrefix, ref state);
			if(oldVersion != Version) return;
			base.LoadStateCore(state);
		}
	}
}
