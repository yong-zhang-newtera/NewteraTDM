#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
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
using System.Globalization;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Data.PivotGrid {
	public class PivotColumnInfo : DataColumnSortInfo {
		DataColumnInfo sortbyColumn;
		PivotSummaryType summaryType;
		PivotSummaryType sortbySummaryType;
		List<PivotSortByCondition> sortbyCondition;
		int showTopRows;
		bool showTopRowsAbsolute = true;
		bool showOthersValue;
		object tag;
		public PivotColumnInfo(DataColumnInfo columnInfo) : this(columnInfo, ColumnSortOrder.Ascending) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) : this(columnInfo, sortOrder, null, PivotSummaryType.Sum) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, int showTopRows) : this(columnInfo, sortOrder, null, PivotSummaryType.Sum, showTopRows) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType) : this(columnInfo, sortOrder, sortbyColumn, sortbySummaryType, 0) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows) : this(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, sortbySummaryType, showTopRows, true, false) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn,
			PivotSummaryType sortbySummaryType, int showTopRows, bool showTopRowsAbsolute, bool showOthersValue)
			: this(columnInfo, summaryType, sortOrder, sortbyColumn, sortbySummaryType, null, showTopRows, showTopRowsAbsolute, showOthersValue) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, 
			PivotSummaryType sortbySummaryType, List<PivotSortByCondition> sortbyCondition,
			int showTopRows, bool showTopRowsAbsolute, bool showOthersValue) : base(columnInfo, sortOrder) { 
			this.sortbyColumn = sortbyColumn;
			this.sortbySummaryType = sortbySummaryType;
			this.sortbyCondition = sortbyCondition;
			this.showTopRows = showTopRows;
			this.showTopRowsAbsolute = showTopRowsAbsolute;
			this.showOthersValue = showOthersValue;
			this.summaryType = summaryType;
		}
		public bool ContainsOthersValue { get { return ShowTopRows > 0; } }
		public bool ContainsSortSummaryOrOthersValue { get { return ContainsSortSummary || ContainsOthersValue; } }
		public bool ContainsSortSummary { get { return SortbyColumn != null; } }
		public bool ContainsSortSummaryConditions { get { return SortbyConditions != null && SortbyConditions.Count > 0; } }
		public PivotSummaryType SummaryType { get { return summaryType; } }
		public DataColumnInfo SortbyColumn { get { return sortbyColumn; } }
		public PivotSummaryType SortbySummaryType { get { return sortbySummaryType; } }
		public List<PivotSortByCondition> SortbyConditions { get { return sortbyCondition; } }
		public int ShowTopRows { get { return showTopRows; } }
		public bool ShowTopRowsAbsolute { get { return showTopRowsAbsolute; } }
		public bool ShowOthersValue { get { return showOthersValue; } }
		public int GetTopRowsCount(int rowCount) {
			if(ShowTopRows <= 0) return 0;
			int count = ShowTopRows;
			if(!ShowTopRowsAbsolute) {
				if(count > 100)
					count = 100;
				int prevCount = count;
				count = (int)(prevCount * rowCount / 100);
				if((int)(count * 100 / prevCount) != rowCount) {
					count ++;
				}
			}
			return rowCount > count ? count : rowCount;
		}
		public object Tag { get { return tag; } set { tag = value; } }
		public PivotColumnInfo Clone(ColumnSortOrder sortOrder) {
			return Clone(ColumnInfo, sortOrder);
		}
		public PivotColumnInfo Clone(DataColumnInfo columnInfo) {
			return Clone(columnInfo, SortOrder);
		}
		PivotColumnInfo Clone(DataColumnInfo dataColumnInfo, ColumnSortOrder sortOrder) {
			PivotColumnInfo columnInfo = new PivotColumnInfo(dataColumnInfo, summaryType, sortOrder, SortbyColumn,
				SortbySummaryType, SortbyConditions, ShowTopRows, ShowTopRowsAbsolute, ShowOthersValue);
			columnInfo.Tag = tag;
			return columnInfo;
		}
	}
	public class PivotColumnInfoCollection : ColumnInfoNotificationCollection {
		public PivotColumnInfoCollection(DataControllerBase controller) : this(controller, null) { }
		public PivotColumnInfoCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public PivotColumnInfo this[int index] { get { return (PivotColumnInfo)List[index]; } }
		public void ClearAndAddRange(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(columnInfos);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal void ClearAndAddRangeSilent(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(columnInfos);
			}
			finally {
				CancelUpdate();
			}
		}
		public void AddRange(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				foreach(PivotColumnInfo columnInfo in columnInfos) { 
					if(columnInfo == null) continue;
					List.Add(columnInfo); 
				}
			}
			finally {
				EndUpdate();
			}
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo) {
			return Add(columnInfo, ColumnSortOrder.Ascending);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) {
			return Add(columnInfo, sortOrder, null, PivotSummaryType.Sum);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, int showTopRows) {
			return Add(columnInfo, sortOrder, null, PivotSummaryType.Sum, showTopRows);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType) {
			return Add(columnInfo, ColumnSortOrder.Ascending, sortbyColumn, sortbySummaryType, 0);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows) {
			return Add(columnInfo, ColumnSortOrder.Ascending, sortbyColumn, sortbySummaryType, showTopRows);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType) {
			return Add(columnInfo, sortOrder, sortbyColumn, sortbySummaryType, 0);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows) {
			return Add(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, sortbySummaryType, showTopRows, true, false);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows, bool showTopRowsAbsolute, bool showOthersValue) {
			return Add(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, sortbySummaryType, showTopRows, showTopRowsAbsolute, showOthersValue);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder, 
			DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows, bool showTopRowsAbsolute, 
			bool showOthersValue) {
			return Add(columnInfo, summaryType, sortOrder, sortbyColumn, sortbySummaryType, null, showTopRows, showTopRowsAbsolute, showOthersValue);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder, 
			DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, List<PivotSortByCondition> sortbyConditions,
			int showTopRows, bool showTopRowsAbsolute, bool showOthersValue) {
			PivotColumnInfo pivotColumnInfo = new PivotColumnInfo(columnInfo, summaryType, sortOrder, sortbyColumn,
				sortbySummaryType, sortbyConditions, showTopRows, showTopRowsAbsolute, showOthersValue);
			List.Add(pivotColumnInfo);
			return pivotColumnInfo;
		}
		public bool IsEquals(PivotColumnInfoCollection collection) {
			if(collection == null || collection.Count != Count) return false;
			for(int i = 0; i < Count; i ++) {
				if(!this[i].IsEquals(collection[i]))
					return false;
			}
			return true;
		}
		public void ChangeSortOrder(int index) {
			if(index < 0 || index >= Count) return;
			InnerList[index] = this[index].Clone(this[index].SortOrder == ColumnSortOrder.Ascending ? ColumnSortOrder.Descending : ColumnSortOrder.Ascending);
		}
		public PivotColumnInfo[] ToArray() {
			return (PivotColumnInfo[])InnerList.ToArray(typeof(PivotColumnInfo));
		}
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
	}
	public class PivotCustomSummaryInfo {
		PivotSummaryItem summaryItem;
		PivotColumnInfo colColumn;
		PivotColumnInfo rowColumn;
		GroupRowInfo groupRow;
		PivotSummaryValue summaryValue;
		DataControllerGroupHelperBase helper;
		VisibleListSourceRowCollection visibleListSourceRows;
		public PivotCustomSummaryInfo(DataControllerGroupHelperBase helper, VisibleListSourceRowCollection visibleListSourceRows,
			PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, GroupRowInfo groupRow) {
			this.helper = helper;
			this.visibleListSourceRows = visibleListSourceRows;
			this.groupRow = groupRow;
			this.summaryItem = summaryItem;
			this.summaryValue = summaryValue;
			this.colColumn = null;
			this.rowColumn = null;
		}
		public PivotSummaryItem SummaryItem { get { return summaryItem; } }
		public DataColumnInfo DataColumn { get { return SummaryItem.ColumnInfo; } }
		public PivotColumnInfo ColColumn { get { return colColumn; } set { colColumn = value; } }
		public PivotColumnInfo RowColumn { get { return rowColumn; } set {rowColumn = value; } }
		public GroupRowInfo GroupRow { get { return groupRow; } }
		public PivotSummaryValue SummaryValue { get { return summaryValue; } }
		public VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		public int ChildCount {
			get {
				if(helper == null || GroupRow == null || GroupRow.Level >= helper.GroupInfo.Count - 1) return 0;
				return helper.GroupInfo.GetTotalChildrenGroupCount(groupRow);
			}
		}
		public PivotCustomSummaryInfo GetChildSummaryInfo(int index) {
			if(index >= ChildCount) return null;
			GroupRowInfo childGroupRow = helper.GroupInfo[GroupRow.Index + index + 1];
			return helper.CreateCustomSummaryInfo(summaryItem, childGroupRow);
		}
	}
	class ChildGroupsCountCache : IDisposable {
		SortedList<int, int>[] childGroupsCount;
		int level0GroupsCount;
		public ChildGroupsCountCache(int columnCount, GroupRowInfoCollection groups) {
			this.childGroupsCount = new SortedList<int, int>[columnCount];
			for(int i = 0; i < columnCount; i++)
				this.childGroupsCount[i] = new SortedList<int, int>();
			this.level0GroupsCount = groups.GetTotalGroupsCountByLevel(0);
			for(int i = 0; i < groups.Count; i++) {
				GroupRowInfo row = groups[i];
				int listIndex = groups.VisibleListSourceRows.GetRow(row.ChildControllerRow),
					childCount = groups.GetChildrenGroupCount(row);
				this.childGroupsCount[row.Level].Add(listIndex, childCount);
			}
		}
		public int this[int level, int listIndex] {
			get {
				if(level == -1)
					return this.level0GroupsCount;
				else
					return this.childGroupsCount[level][listIndex];
			}
		}
		#region IDisposable Members
		protected bool IsDisposed { get { return this.childGroupsCount == null; } }
		public void Dispose() {
			if(IsDisposed) return;
			for(int i = 0; i < childGroupsCount.Length; i++)
				childGroupsCount[i] = null;
			childGroupsCount = null;
			GC.SuppressFinalize(this);
		}
		~ChildGroupsCountCache() {
			if(!IsDisposed) Dispose();
		}
		#endregion
	}
	public abstract class DataControllerGroupHelperBase {
		PivotDataController controller;
		GroupRowInfoCollection groupInfo;
		VisibleListSourceRowCollection visibleListSourceRows;
		Hashtable othersRows;
		protected internal readonly static object OthersValue = new object();
		protected internal readonly static object NullValue = new object();
		public DataControllerGroupHelperBase(PivotDataController controller) {
			this.controller = controller;
			this.visibleListSourceRows = new VisibleListSourceRowCollection(Controller);
			this.groupInfo = CreateGroupRowInfoCollection();
			this.othersRows = null;
		}
		public PivotDataController Controller { get { return controller; } }
		public IDataControllerSort SortClient { get { return Controller.SortClient; } }
		public int GetListSourceRowByControllerRow(int row) { return Controller.GetListSourceRowByControllerRow(VisibleListSourceRows, row); } 
		protected internal abstract DataColumnSortInfoCollection SortInfo { get; }
		protected abstract PivotColumnInfo[] CreateSummaryColumns();
		protected internal abstract PivotSummaryItemCollection Summaries { get; }
		protected virtual GroupRowInfoCollection CreateGroupRowInfoCollection() {
			return new GroupRowInfoCollection(Controller, SortInfo, VisibleListSourceRows);
		}
		public GroupRowInfoCollection GroupInfo { 
			get { return groupInfo; }
			set {
				this.groupInfo = value;
				OnGroupInfoRecreated();
			}
		}
		public VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		public object GetValue(GroupRowInfo groupRow) {
			if(groupRow == null) return null;
			if(GetIsOthersValue(groupRow)) return OthersValue;
			DataColumnInfo columnInfo = SortInfo[groupRow.Level].ColumnInfo;
			if(columnInfo == null) return null;
			return Controller.GetRowValueFromHelper(GroupInfo, groupRow.ChildControllerRow, columnInfo.Index);
		}
		public bool GetIsOthersValue(GroupRowInfo groupRow) {
			if(groupRow == null || this.othersRows == null) return false;
			return this.othersRows.Contains(groupRow);
		}
		public bool IsSorted { get { return SortInfo.Count > 0; } }
		public bool IsGrouped { get { return SortInfo.GroupCount > 0; } }
		public virtual void Reset() {
			Controller.ResetSortInfoCollection(SortInfo);
			this.GroupInfo.ClearSummary();
		}
		public virtual void DoRefresh() {
			ClearVisialIndexesAndGroupInfo();
			DoSortRows();
			DoGroupRows();
			CalcSummary(); 
			BuildVisibleIndexes();
		}
		protected virtual void ClearVisialIndexesAndGroupInfo() {
			this.othersRows = null;
			this.rootIndex = null;
			this.rowsIndex = null;
			VisibleListSourceRows.Clear();
			GroupInfo.Clear();
		}
		protected virtual void DoSortRows() {
			if(!IsSorted) return;
			VisibleListSourceRows.CloneRecords(Controller.VisibleListSourceRows);
			DoSortRowsCore();
		}
		protected void DoSortRowsCore() {
			if(!Controller.CacheData) {
				Controller.CreateColumnStorages(SortInfo, VisibleListSourceRows);
			}
			DoSortRowsCore(SortInfo, 0, VisibleListSourceRows.Count - 1);
			if(!Controller.CacheData) {
				SortInfo.ClearColumnStorages();
			}
		}
		protected virtual void DoSortRowsCore(DataColumnSortInfoCollection sortInfo, int left, int right) {
			if(left >= right) return;
			if(SortClient != null) SortClient.BeforeSorting();
			try {
				Controller.VisibleListSourceCollectionQuickSort(VisibleListSourceRows, sortInfo, left, right);
			} finally {
				if(SortClient != null) SortClient.AfterSorting();
			}
		}
		protected virtual void DoGroupRows() {
			if(!IsGrouped) return;
			DoGroupRowsCore(0, VisibleListSourceRowCount, null);
			DoSetHelperCompareCache();
		}
		protected virtual void DoGroupRowsCore(int controllerRow, int rowCount, GroupRowInfo parentGroupRow) {
			if(SortClient != null) SortClient.BeforeGrouping();
			try {
				Controller.DoGroupColumn(SortInfo, GroupInfo, controllerRow, rowCount, parentGroupRow);
				GroupInfo.UpdateIndexes();
				BuildRowsIndex();
				BuildVisibleIndexes();
			} finally {
				if(SortClient != null) SortClient.AfterGrouping();
			}
		}
		Hashtable rootIndex;
		Hashtable[] rowsIndex;
		protected void BuildRowsIndex() {
			this.rootIndex = new Hashtable();
			rowsIndex = new Hashtable[GroupInfo.Count];
			int start = 0;
			BuildRowsIndex(ref start, 0, rootIndex);
		}
		void BuildRowsIndex(ref int start, int level, Hashtable list) {
			int columnInfoIndex = SortInfo[level].ColumnInfo.Index;
			int count = GroupInfo.Count;
			for( ; start < count; start++) {
				GroupRowInfo gri = GroupInfo[start];
				if(gri.Level == level) {
					object value = GetValue(gri);
					if(value == null) value = NullValue;
					if(!list.Contains(value)) list.Add(value, start);
				}
				if(gri.Level > level) {
					Hashtable child = new Hashtable();
					rowsIndex[start - 1] = child;
					BuildRowsIndex(ref start, level + 1, child);
					start--;
				}
				if(gri.Level < level)
					break;
			}
		}
		protected void DoSetHelperCompareCache() {
			if(!IsGrouped || (ListSourceRowCount !=  VisibleListSourceRows.Count)) return;
			int columnIndex = SortInfo[0].ColumnInfo.Index;
			if(!Controller.SupportComparerCache(columnIndex) || Controller.HasComparerCache(columnIndex)) return;
			int[] cache = new int[ListSourceRowCount];
			ArrayList rootGroups = new ArrayList();
			GroupInfo.GetChildrenGroups(null, rootGroups);
			for(int i = 0; i < rootGroups.Count; i ++) {
				DoSetHelperComparerArrayCacheByGroup((GroupRowInfo)rootGroups[i], cache);
			}
			Controller.SetComparerCache(columnIndex, cache, SortInfo[0].SortOrder == ColumnSortOrder.Ascending);
		}
		void DoSetHelperComparerArrayCacheByGroup(GroupRowInfo groupRow, int[] cache) {
			for(int i = 0; i < groupRow.ChildControllerRowCount; i ++) {
				int listRowIndex = Controller.GetListSourceRowIndex(GroupInfo, groupRow.ChildControllerRow + i);
				cache[listRowIndex] = groupRow.Index;
			}
		}
		protected virtual void BuildVisibleIndexes() {
		}
		protected internal GroupRowInfo GetSummaryGroupRow(GroupRowInfo groupRow, object[] values) {
			if(groupRow == null) return null;
			int count = GroupInfo.GetTotalChildrenGroupCount(groupRow);
			int equaledLevel = -1;
			int i = 0;
			while(i < count) {		
				GroupRowInfo summaryGroupRow = GroupInfo[groupRow.Index + 1 + i];
				if(Controller.IsEqualGroupValues(
					values[summaryGroupRow.Level - groupRow.Level - 1],	GetValue(summaryGroupRow))) {
					if(values.Length == summaryGroupRow.Level - groupRow.Level)
						return summaryGroupRow;
					equaledLevel = summaryGroupRow.Level;
					i ++;
				} else {
					if(equaledLevel >= summaryGroupRow.Level) return null;
					i += GroupInfo.GetTotalChildrenGroupCount(summaryGroupRow) + 1;
				}
			}
			return null;
		}
		protected virtual void UpdateGroupSummaryCore() {
			GroupInfo.ClearSummary();
			PrepareSummaryItems();
			CalcGroupSummariesAndVariations(0, GroupInfo.Count - 1);
		}
		protected virtual void PrepareSummaryItems() {}
		protected PivotSummaryItemCollection CreateSummaryItems(PivotColumnInfo[] columns) {
			PivotSummaryItemCollection summaries = new PivotSummaryItemCollection(Controller, null);
			summaries.ClearAndAddRange(Controller.Summaries);
			AddSummariesItems(summaries, columns);
			return summaries;
		}
		protected void AddSummariesItems(PivotSummaryItemCollection summaries, PivotColumnInfo[] columns) {
			for(int i = 0; i < columns.Length; i ++) {
				if(columns[i].SortbyColumn != null && !summaries.Contains(columns[i].SortbyColumn)) {
					summaries.Add(new PivotSummaryItem(columns[i].SortbyColumn, columns[i].SummaryType));
				}
			}
		}
		protected void CalcGroupSummariesAndVariations(int startGroupIndex, int endGroupIndex) {
			for(int i = 0; i < Summaries.Count; i ++) {
				CalcGroupSummary(Summaries[i], startGroupIndex, endGroupIndex);
			}
		}
		protected void CalcGroupSummary(PivotSummaryItem summaryItem) {
			CalcGroupSummary(summaryItem, 0, GroupInfo.Count - 1);
		}
		protected void CalcGroupSummary(PivotSummaryItem summaryItem, int startGroupIndex, int endGroupIndex) {
			bool needCalcCustomSummary = Controller.NeedCalcCustomSummary(summaryItem.ColumnInfo);
			for(int i = endGroupIndex; i >= startGroupIndex; i --) {
				CalcGroupRowSummary(GroupInfo[i], summaryItem, needCalcCustomSummary);
			}
		}		
		PivotSummaryValue GetSummaryValue(GroupRowInfo groupRow, PivotSummaryItem summaryItem) {
			return groupRow != null ? groupRow.GetSummaryValue(summaryItem) as PivotSummaryValue : null;
		}
		protected virtual GroupRowInfo GetPrevColumnGroupRow(GroupRowInfo groupRow) { 
			if(groupRow.Level < GetRowGroupCount()) return null;
			return GetPrevGroupRow(groupRow, GetRowGroupCount()); 
		}
		protected virtual GroupRowInfo GetPrevRowGroupRow(GroupRowInfo groupRow) { 
			if(GetRowGroupCount() == 0) return null;
			if(groupRow.Level < GetRowGroupCount()) return GetPrevGroupRow(groupRow);
			object[] values = new object[groupRow.Level + 1 - GetRowGroupCount()];
			for(int i = values.Length - 1; i >= 0; i --) {
				values[i] = GetValue(groupRow); 
				groupRow = groupRow.ParentGroup;
			}
			GroupRowInfo summaryGroupRow = null;
			while(summaryGroupRow == null && groupRow != null) {
				groupRow = GetPrevGroupRow(groupRow);
				summaryGroupRow = GetSummaryGroupRow(groupRow, values);
			}
			return summaryGroupRow;
		}
		protected virtual int GetRowGroupCount() { return 0;  }
		protected GroupRowInfo GetPrevGroupRow(GroupRowInfo groupRow) {
			return GetPrevGroupRow(groupRow, 0);
		}
		protected GroupRowInfo GetNextGroupRow(GroupRowInfo groupRow) {
			return GetNextGroupRow(groupRow, 0);
		}
		protected GroupRowInfo GetPrevGroupRow(GroupRowInfo groupRow, int groupRowCount) {
			for(int i = groupRow.Index - 1; i >= 0; i --) {
				if(GroupInfo[i].Level < groupRowCount) return null;
				if(GroupInfo[i].Level == groupRow.Level)
					return GroupInfo[i];
			}
			return null;
		}
		protected GroupRowInfo GetNextGroupRow(GroupRowInfo groupRow, int groupRowCount) {
			for(int i = groupRow.Index + 1; i < GroupInfo.Count; i ++) {
				if(GroupInfo[i].Level < groupRowCount) return null;
				if(GroupInfo[i].Level == groupRow.Level)
					return GroupInfo[i];
			}
			return null;
		}
		protected void CalcGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, bool needCalcCustomSummary) {
			if(!RequireCalcGroupRowSummary(groupRow)) return;
			PivotSummaryValue summaryValue = new PivotSummaryValue(Controller.ValueComparer);
			groupRow.SetSummaryValue(summaryItem, summaryValue);
			if(GroupInfo.IsLastLevel(groupRow)) 
				CalcLastLevelGroupRowSummary(groupRow, summaryItem, summaryValue);
			else CalcParentGroupRowSummary(groupRow, summaryItem, summaryValue);
			if(needCalcCustomSummary)
				Controller.CalcCustomSummary(CreateCustomSummaryInfo(summaryItem, groupRow));
		}
		protected virtual bool RequireCalcGroupRowSummary(GroupRowInfo groupRow) { return true; } 
		public PivotCustomSummaryInfo CreateCustomSummaryInfo(PivotSummaryItem summaryItem, GroupRowInfo groupRow) {
			PivotCustomSummaryInfo info = new PivotCustomSummaryInfo(this, VisibleListSourceRows, summaryItem, (PivotSummaryValue)groupRow.GetSummaryValue(summaryItem), groupRow);
			info.ColColumn = GetColColumnByGroupRow(groupRow);
			info.RowColumn = GetRowColumnByGroupRow(groupRow);
			return info;
		}
		public virtual PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return null;
		}
		public virtual PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			return null;
		}
		public object[] GetGroupRowValues(GroupRowInfo groupRow) {
			if(groupRow == null) return new object[0];
			object[] values = new object[groupRow.Level + 1];
			while(groupRow != null) {
				values[groupRow.Level] = GetValue(groupRow);
				groupRow = groupRow.ParentGroup;
			}
			return values;
		}
		protected void CalcLastLevelGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			Controller.CalcLastLevelGroupRowSummary(VisibleListSourceRows, summaryItem, summaryValue, groupRow.ChildControllerRow, groupRow.ChildControllerRowCount);
		}
		public void CalcParentGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			int startIndex = groupRow != null ? groupRow.Index + 1 : 0;
			int level = groupRow != null ? groupRow.Level + 1 : 0;
			for(int i = startIndex; i < GroupInfo.Count; i ++) {
				GroupRowInfo childGroupRow = GroupInfo[i];
				if(childGroupRow.Level < level && groupRow != null) break;
				if(childGroupRow.Level == level) {
					summaryValue.AddValue((PivotSummaryValue)childGroupRow.GetSummaryValue(summaryItem));
				}
			}
		}
		protected virtual void CalcSummary() {
			UpdateGroupSummaryCore();
		}
		ChildGroupsCountCache childGroupsCountCache;
		public bool DoConditionalSortSummaryAndAddOthers(IComparer[] comparers, bool firstPass) {
			PivotColumnInfo[] summaryColumns = CreateSummaryColumns();
			GroupRowInfoCollection groups = CreateGroupRowInfoCollection();
			groups.AutoExpandAllGroups = GroupInfo.AutoExpandAllGroups;
			bool refreshRequired = DoConditionalSortSummary(summaryColumns, groups, null, comparers, !firstPass && childGroupsCountCache != null);
			groups.UpdateIndexes();
			this.childGroupsCountCache = firstPass && refreshRequired ? new ChildGroupsCountCache(summaryColumns.Length, groups) : null;
			GroupInfo = groups;
			BuildRowsIndex();
			BuildVisibleIndexes();
			return refreshRequired;
		}
		protected bool DoConditionalSortSummary(PivotColumnInfo[] summaryColumns, GroupRowInfoCollection groups, GroupRowInfo parentGroup, IComparer[] comparers, bool useCountCache) {
			if(parentGroup != null && GroupInfo.IsLastLevel(parentGroup)) return false;
			int level = parentGroup == null ? 0 : parentGroup.Level + 1;
			PivotColumnInfo pivotColumnInfo = summaryColumns[level];
			ArrayList list = new ArrayList();
			GroupInfo.GetChildrenGroups(parentGroup, list);
			IComparer comparer = comparers != null ? comparers[level] : null;
			DoConditionSortListCore(parentGroup, pivotColumnInfo, list, comparer);
			bool doRefresh = false;
			int groupedCount = GetGroupedCount(groups, parentGroup, useCountCache, level, pivotColumnInfo, list);
			doRefresh = groupedCount < list.Count && !pivotColumnInfo.ShowOthersValue;
			for(int n = 0; n < groupedCount; n++) {
				GroupRowInfo row = (GroupRowInfo)list[n];
				groups.Add(row);
				doRefresh |= DoConditionalSortSummary(summaryColumns, groups, row, comparers, useCountCache);
				GroupRowInfo lastChild = groups[groups.Count - 1];
				if(row != lastChild) 
					row.ChildControllerRowCount = lastChild.ChildControllerRow + lastChild.ChildControllerRowCount - row.ChildControllerRow;
			}
			if(pivotColumnInfo.ShowOthersValue)
				doRefresh |= AddOthersGroup(summaryColumns, groups, list, groupedCount, comparers, useCountCache);
			if(doRefresh && !useCountCache && level == 0) {				
				int[] newRows;
				int rowCount;
				GetNewVisibleRows(groups, out newRows, out rowCount);
				if(rowCount < groups.VisibleListSourceRows.Count) {
					groups.VisibleListSourceRows.SetListCore(newRows, rowCount, groups.VisibleListSourceRows.AppliedFilterExpression, groups.VisibleListSourceRows.HasUserFilter);
					return true;
				} else
					throw new Exception("Unnecessary refresh");
			}
			return doRefresh;
		}
		void DoConditionSortListCore(GroupRowInfo parentGroup, PivotColumnInfo pivotColumnInfo, ArrayList list, IComparer comparer) {
			if(pivotColumnInfo.SortbyColumn != null && comparer != null) {
				list.Sort(comparer);
				MoveVisualListBlocks(list, parentGroup);
				ApplySummarySortForList(list, parentGroup);
			}
		}
		protected virtual int GetGroupedCount(GroupRowInfoCollection groups, GroupRowInfo parentGroup, bool useCountCache, int level, PivotColumnInfo pivotColumnInfo, ArrayList list) {
			int topRowCount = pivotColumnInfo.GetTopRowsCount(list.Count),
				groupedCount = topRowCount > 0 && topRowCount < list.Count ? topRowCount : list.Count;
			if(useCountCache) {
				int groupedCountFromCache = GetGroupCountFromCache(groups, parentGroup);
				if(groupedCountFromCache < groupedCount)
					throw new Exception("Corrupted groupCount cache");
				groupedCount = Math.Min(list.Count, groupedCountFromCache);
			}
			return groupedCount;
		}
		int GetGroupCountFromCache(GroupRowInfoCollection groups, GroupRowInfo parentGroup) {			
			if(parentGroup == null)
				return childGroupsCountCache[-1, 0];
			int listIndex = groups.VisibleListSourceRows.GetRow(parentGroup.ChildControllerRow);
			return childGroupsCountCache[parentGroup.Level, listIndex];
		}
		void GetNewVisibleRows(GroupRowInfoCollection groups, out int[] newRows, out int rowCount) {
			newRows = new int[groups.VisibleListSourceRows.Count];
			rowCount = 0;
			int offset = 0;
			for(int i = 0; i < groups.Count; i++) {
				GroupRowInfo row = groups[i];
				if(row.Level == 0) {
					offset = row.ChildControllerRow - rowCount;
					int firstInex = row.ChildControllerRow, lastIndex = row.ChildControllerRow + row.ChildControllerRowCount;
					for(int j = firstInex; j < lastIndex; j++)
						newRows[j - offset] = VisibleListSourceRows.GetRow(j);
					rowCount += row.ChildControllerRowCount;
				}
				row.ChildControllerRow -= offset;
			}
		}
		protected virtual void OnGroupInfoRecreated() { }	
		public void MoveVisualListBlocks(ArrayList groupList, GroupRowInfo parentGroup) {
			int[] records = new int[parentGroup != null ? parentGroup.ChildControllerRowCount : VisibleListSourceRows.Count];
			int currentControllerRowPos = 0;
			for(int i = 0; i < groupList.Count; i++) {
				GroupRowInfo row = (GroupRowInfo)groupList[i];
				MoveVisualListForGroup(row, records, currentControllerRowPos);
				currentControllerRowPos += row.ChildControllerRowCount;
			}
			VisibleListSourceRows.SetRange(parentGroup != null ? parentGroup.ChildControllerRow : 0, records);
		}
		void MoveVisualListForGroup(GroupRowInfo groupRow, int[] records, int newPosition) {
			for(int i = 0; i < groupRow.ChildControllerRowCount; i++) {
				records[newPosition + i] = VisibleListSourceRows.GetRow(groupRow.ChildControllerRow + i);
			}
		}
		public void ApplySummarySortForList(ArrayList groupList, GroupRowInfo parentGroup) {
			int childControllerRow = parentGroup != null ? parentGroup.ChildControllerRow : 0;
			for(int i = 0; i < groupList.Count; i++) {
				GroupRowInfo row = (GroupRowInfo)groupList[i];
				row.ChildControllerRow = childControllerRow;
				childControllerRow += row.ChildControllerRowCount;
				if(!GroupInfo.IsLastLevel(row)) {
					ArrayList childList = new ArrayList();
					GroupInfo.GetChildrenGroups(row, childList);
					ApplySummarySortForList(childList, row);
				}
			}
		}
		protected virtual bool AddOthersGroup(PivotColumnInfo[] summaryColumns, GroupRowInfoCollection groups, ArrayList groupRowList, int groupedCount, IComparer[] comparers, bool useCountCache) {
			if(groupedCount >= groupRowList.Count) return false;
			OthersGroupMoveVisibleListSourceItems(groupRowList, groupedCount);
			GroupRowInfo groupRow = GetOthersGroupRowInfo(groupRowList, groupedCount);
			if(this.othersRows == null)
				this.othersRows = new Hashtable();
			this.othersRows.Add(groupRow, groupRow);
			OthersGroupChangeVisibleListSource(groupRow);
			OthersGroupChangeGroupInfo(groupRow, summaryColumns);
			groups.Add(groupRow);
			return DoConditionalSortSummary(summaryColumns, groups, groupRow, comparers, useCountCache);
		}
		void OthersGroupMoveVisibleListSourceItems(ArrayList groupRowList, int groupedCount) {
			GroupRowInfo parentRow = ((GroupRowInfo)groupRowList[0]).ParentGroup;
			int controllerRowIndex = parentRow != null ? parentRow.ChildControllerRow : 0;
			for(int i = 0; i < groupedCount; i ++) {
				GroupRowInfo row = (GroupRowInfo)groupRowList[i];
				OthersGroupMoveVisibleListSourceItems(i, controllerRowIndex, groupRowList);
				controllerRowIndex += row.ChildControllerRowCount;
			}
		}
		void OthersGroupMoveVisibleListSourceItems(int rowIndex, int controllerRowIndex, ArrayList groupRowList) {
			GroupRowInfo row = (GroupRowInfo)groupRowList[rowIndex];
			if(row.ChildControllerRow == controllerRowIndex) return;
			for(int i = rowIndex + 1; i < groupRowList.Count; i ++) {
				GroupRowInfo changedRow = (GroupRowInfo)groupRowList[i];
				if(changedRow.ChildControllerRow < row.ChildControllerRow) {
					OthersGroupChangeGroupInfoChildControllerRows(changedRow, row.ChildControllerRowCount);
				}
			}
			OthersGroupChangeGroupInfoChildControllerRows(row, controllerRowIndex - row.ChildControllerRow);
		}
		void OthersGroupChangeGroupInfoChildControllerRows(GroupRowInfo groupRow, int delta) {
			groupRow.ChildControllerRow += delta;
			ArrayList list = new ArrayList();
			GroupInfo.GetChildrenGroups(groupRow, list);
			for(int i = 0; i < list.Count; i ++) {
				OthersGroupChangeGroupInfoChildControllerRows((GroupRowInfo)list[i], delta);
			}
		}
		GroupRowInfo GetOthersGroupRowInfo(ArrayList groupRowList, int groupedCount) {
			GroupRowInfo groupRow = (GroupRowInfo)groupRowList[groupedCount];
			int childRowCount = groupRow.ChildControllerRowCount;
			for(int i = groupedCount + 1; i < groupRowList.Count; i ++) {
				GroupRowInfo testRow = (GroupRowInfo)groupRowList[i];
				childRowCount += testRow.ChildControllerRowCount;
				if(testRow.ChildControllerRow < groupRow.ChildControllerRow)
					groupRow = testRow;
			}
			groupRow.ChildControllerRowCount = childRowCount;
			return groupRow;
		}
		void OthersGroupChangeVisibleListSource(GroupRowInfo groupRow) {
			DataColumnSortInfoCollection sortInfo = new DataColumnSortInfoCollection(Controller);
			for(int i = groupRow.Level + 1; i < SortInfo.Count; i ++)
				sortInfo.Add(SortInfo[i].ColumnInfo, SortInfo[i].SortOrder);
			if(sortInfo.Count > 0) {
				VisibleListSourceRows.UseStorageOnSorting = false;
				DoSortRowsCore(sortInfo, groupRow.ChildControllerRow, groupRow.ChildControllerRow + groupRow.ChildControllerRowCount - 1);
				VisibleListSourceRows.UseStorageOnSorting = true;
			}
		}
		void OthersGroupChangeGroupInfo(GroupRowInfo groupRow, PivotColumnInfo[] summaryColumns) {
			int insertedCount = 0;
			if(!GroupInfo.IsLastLevel(groupRow)) {
				int deletedIndex = groupRow.Index + 1;
				while(deletedIndex < GroupInfo.Count) {
					if(GroupInfo[deletedIndex].Level < groupRow.Level) break;
					GroupInfo.RemoveAt(deletedIndex);
				}
				int prevCount = GroupInfo.Count;
				DoGroupRowsCore(groupRow.ChildControllerRow, groupRow.ChildControllerRowCount, groupRow);
				insertedCount = GroupInfo.Count - prevCount;
				GroupInfo.MoveFromEndToMiddle(prevCount, insertedCount, deletedIndex);
			}
			CalcGroupSummariesAndVariations(groupRow.Index, groupRow.Index + insertedCount);
		}
		public virtual void UpdateGroupSummary() {
			UpdateGroupSummaryCore();
		}
		protected internal void AddColumnsToSortInfo(PivotColumnInfoCollection columns) {
			for(int i = 0; i < columns.Count; i ++) 
				SortInfo.Add(columns[i].ColumnInfo, columns[i].SortOrder);
			SortInfo.GroupCount = SortInfo.Count;
		}
		protected bool IsReady { get { return Controller.IsReady; } }
		protected int ListSourceRowCount { get { return Controller.ListSourceRowCount; } }
		protected int VisibleListSourceRowCount { get { return VisibleListSourceRows.IsEmpty ? ListSourceRowCount : VisibleListSourceRows.Count; } }
		public GroupRowInfo GetGroupRowByValues(object[] values) {
			if(values == null || values.Length < 1 || values.Length > GroupInfo.LevelCount) return null;
			return GetGroupRowByValues(0, rootIndex, values);
		}
		public GroupRowInfo GetGroupRowByValues(int level, Hashtable list, object[] values) {
			object value = values[level];
			object start = list[value == null ? NullValue : value];
			if(start == null)
				return null;
			if(level == values.Length - 1)
				return GroupInfo[(int)start];
			return GetGroupRowByValues(level + 1, rowsIndex[((int)start)], values);
		}
	}
	public class PivotVisibleIndexCollection : VisibleIndexCollection {
		public PivotVisibleIndexCollection(DataControllerBase controller, GroupRowInfoCollection groupInfo) : base(controller, groupInfo) {	}
		internal void SetGroupInfo(GroupRowInfoCollection newGroupInfo) { GroupInfo = newGroupInfo; }
	}
	public class PivotDataControllerArea : DataControllerGroupHelperBase {
		PivotColumnInfoCollection columns;
		PivotGroupRowsKeeper rowsKeeper;
		PivotVisibleIndexCollection visibleIndexes;
		DataColumnSortInfoCollection sortInfo;
		PivotSummaryItemCollection summaries;
		public PivotDataControllerArea(PivotDataController controller) : base(controller) {
			this.summaries = null;
			this.visibleIndexes = new PivotVisibleIndexCollection(Controller, GroupInfo);
			this.columns = new PivotColumnInfoCollection(Controller, new CollectionChangeEventHandler(OnColumnInfoCollectionChanged));
			GroupInfo.AutoExpandAllGroups = true;
			this.rowsKeeper = new PivotGroupRowsKeeper(this);
		}
		public int VisibleCount { get { return VisibleIndexes.Count; } }
		public PivotColumnInfoCollection Columns { get { return columns; } }
		protected internal override DataColumnSortInfoCollection SortInfo { 
			get { 
				if(this.sortInfo == null) { 
					this.sortInfo = new DataColumnSortInfoCollection(Controller, null);
				}
				return sortInfo;
			}
		}
		protected internal override PivotSummaryItemCollection Summaries { get { return summaries; } }
		protected override void PrepareSummaryItems() {
			this.summaries = CreateSummaryItems(CreateSummaryColumns());
		}
		public override void DoRefresh() {
			UpdateColumns();
			base.DoRefresh();
		}
		void UpdateColumns() {
			if(Columns.Count == 0) return;
			PivotColumnInfo[] newColumns = new PivotColumnInfo[Columns.Count];
			for(int i = 0; i < Columns.Count; i ++) {
				newColumns[i] = Columns[i].Clone(Controller.Columns[Columns[i].ColumnInfo.Name]);
			}
			Columns.ClearAndAddRangeSilent(newColumns);
		}
		public int GetVisibleIndexByValues(object[] values) {
			GroupRowInfo groupInfo = GetGroupRowByValues(values);
			return groupInfo != null ? VisibleIndexes.IndexOf(groupInfo.Handle) : -1;
		}
		public int GetControllerRowHandle(int visibleIndex) {
			if(!IsReady || visibleIndex < 0 || visibleIndex >= VisibleCountCore) return DataController.InvalidRow;
			return VisibleIndexes.GetHandle(visibleIndex);
		}
		public int GetVisibleIndexByGroupInfo(GroupRowInfo groupInfo) {
			return groupInfo != null ? VisibleIndexes.IndexOf(groupInfo.Handle) : -1;
		}
		public GroupRowInfo GetGroupRowInfo(int visibleIndex) {
			int controllerRowHandle = GetControllerRowHandle(visibleIndex);
			return GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRowHandle);
		}
		object GetVal(GroupRowInfo groupRow) {
			int listSourceRow = GetListSourceRowByControllerRow(groupRow.ChildControllerRow);
			return Controller.GetRowValue(listSourceRow, Controller.Columns["Sales Person"]);
		}
		public ArrayList GetGroupRows(int startIndex, int endIndex) {
			ArrayList result = new ArrayList();
			if(startIndex == -1) {
				GroupInfo.GetChildrenGroups(null, result);
			} else {
				int maxLevel = this.Columns.Count - 1,
					expandedStartIndex = GetControllerRowHandle(startIndex),
					expandedEndIndex = GetControllerRowHandle(endIndex);
				for(int i = expandedStartIndex; i > expandedEndIndex; i--) {
					GroupRowInfo groupRow = GroupInfo.GetGroupRowInfoByControllerRowHandle(i);
					if(groupRow == null || groupRow.Level != maxLevel) continue;
					result.Add(groupRow);
				}
			}
			return result;
		}
		public int GetPrevVisibleIndex(int visibleIndex) {
			return GetNextOrPrevVisibleIndex(visibleIndex, false);
		}
		public int GetNextVisibleIndex(int visibleIndex) {
			return GetNextOrPrevVisibleIndex(visibleIndex, true);
		}
		public int GetNextOrPrevVisibleIndex(int visibleIndex, bool isNext) {
			GroupRowInfo groupRow = GetGroupRowInfo(visibleIndex);
			if(groupRow != null) {
				groupRow = isNext ? GetNextGroupRow(groupRow) : GetPrevGroupRow(groupRow);
			}
			return groupRow != null ? VisibleIndexes.IndexOf(groupRow.Handle) : -1;
		}
		public object GetValue(int visibleIndex) {
			return GetValue(GetGroupRowInfo(visibleIndex));
		}
		public object GetValue(int visibleIndex, int columnIndex) {
			GroupRowInfo groupRow = GetGroupRowByColumnStrictly(visibleIndex, columnIndex);
			return groupRow != null ? GetValue(groupRow) : null;
		}
		public bool IsColumnValueExpanded(int visibleIndex, int columnIndex) {
			GroupRowInfo groupRow = GetGroupRowByColumn(visibleIndex, columnIndex);
			return groupRow != null ? groupRow.Expanded : false;
		}
		protected GroupRowInfo GetGroupRowByColumn(int visibleIndex, int groupLevel) {
			if(groupLevel < 0 || groupLevel >= SortInfo.Count) return null;
			GroupRowInfo groupRow = GetGroupRowInfo(visibleIndex);
			if(groupRow == null) return null;
			while(groupLevel < groupRow.Level)
				groupRow = groupRow.ParentGroup;
			return groupRow;
		}
		protected GroupRowInfo GetGroupRowByColumnStrictly(int visibleIndex, int groupLevel) {
			GroupRowInfo groupRow = GetGroupRowByColumn(visibleIndex, groupLevel);
			if(groupRow == null || groupLevel != groupRow.Level) return null;
			return groupRow;
		}
		public bool GetIsOthersValue(int visibleIndex) {
			return GetIsOthersValue(GetGroupRowInfo(visibleIndex));
		}
		public bool GetIsOthersValue(int visibleIndex, int levelIndex) {
			return GetIsOthersValue(GetGroupRowByColumnStrictly(visibleIndex, levelIndex));
		}
		protected VisibleIndexCollection VisibleIndexes { get { return visibleIndexes; } }
		protected PivotGroupRowsKeeper RowsKeeper { get { return rowsKeeper; } }
		protected int VisibleCountCore { get { return VisibleIndexes.IsEmpty ? VisibleListSourceRowCount : VisibleIndexes.Count; } }
		protected override void OnGroupInfoRecreated() {
			base.OnGroupInfoRecreated();
			if(VisibleIndexes != null) visibleIndexes.SetGroupInfo(GroupInfo);
		}
		public int AlwaysVisibleLevelIndex {
			get { return GroupInfo.AlwaysVisibleLevelIndex; }
			set { 
				if(value < -1) value = -1;
				if(AlwaysVisibleLevelIndex == value) return;
				GroupInfo.AlwaysVisibleLevelIndex = value; 
				BuildVisibleIndexes();
			}
		}
		public void SaveGroupRowsColumns() {
			RowsKeeper.SaveColumns();
		}
		public void SaveGroupRowsState() { 
			RowsKeeper.SaveRows();
		}
		public void SaveFieldsStateToStream(Stream stream) {
			RowsKeeper.WriteToStream(stream);
		}
		public void RestoreGroupRowsState() { 
			RowsKeeper.Restore(); 
			BuildVisibleIndexes();
		}
		public void LoadFieldsStateFromStream(Stream stream) {
			RowsKeeper.ReadFromStream(stream);
			RestoreGroupRowsState();
		}
		public void WebSaveFieldsStateToStream(Stream stream) {
			RowsKeeper.WebWriteToStream(stream);
		}
		public void WebLoadFieldsStateFromStream(Stream stream) {
			RowsKeeper.WebReadFromStream(stream);
			BuildVisibleIndexes();
		}
		public void ChangeFieldSortOrder(int index) {
			if(index < 0 || index > Columns.Count) return;
			SortInfo.ChangeGroupSorting(index);
			Columns.ChangeSortOrder(index);
			if(Columns[index].ShowTopRows > 0) {
				Controller.DoRefresh();
			} else {
				GroupInfo.ReverseLevel(index);
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
				BuildRowsIndex();
			}
		}
		public void ClearGroupRowsState() {
			RowsKeeper.Clear();
			GroupInfo.Clear();
		}
		public void ExpandAll() {
			ChangeAllExpanded(true);
		}
		public void CollapseAll() {
			ChangeAllExpanded(false);
		}
		public void ChangeAllExpanded(bool expanded) {
			bool hasChanged = false;
			for(int i = 0; i < Columns.Count - 1; i ++) {
				hasChanged |= GroupInfo.ChangeLevelExpanded(i, expanded);
			}
			if(hasChanged) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public bool IsRowExpanded(int visibleIndex) {
			GroupRowInfo groupRow = this.GetGroupRowInfo(visibleIndex);
			return groupRow == null ? false : groupRow.Expanded;
		}
		public void ExpandRow(int visibleIndex) {
			ExpandRow(visibleIndex, false);
		}
		public void ExpandRow(int visibleIndex, bool recursive) {
			ChangeExpanded(visibleIndex, true, recursive);
		}
		public void CollapseRow(int visibleIndex) {
			CollapseRow(visibleIndex, false);
		}
		public void CollapseRow(int visibleIndex, bool recursive) {
			ChangeExpanded(visibleIndex, false, recursive);
		}
		public object[] GetRowValues(int visibleIndex) {
			return GetGroupRowValues(GetGroupRowInfo(visibleIndex));
		}
		public void CollapseColumn(int columnIndex) {
			ChangeColumnExpanded(columnIndex, false);
		}
		public void ExpandColumn(int columnIndex) {
			ChangeColumnExpanded(columnIndex, true);
		}
		public void CollapseColumn(int columnIndex, object value) {
			ChangeColumnExpanded(columnIndex, false, value);
		}
		public void ExpandColumn(int columnIndex, object value) {
			ChangeColumnExpanded(columnIndex, true, value);
		}
		public virtual void ChangeColumnExpanded(int columnIndex, bool expanded) {
			if(columnIndex >= Columns.Count) return;
			if(GroupInfo.ChangeLevelExpanded(columnIndex, expanded)) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public void ChangeColumnExpanded(object[] values) {
			GroupRowInfo groupRow = GetGroupRowByValues(values);
			if(groupRow != null) ChangeColumnExpanded(groupRow, !groupRow.Expanded);
		}
		public void ChangeColumnExpanded(object[] values, bool expanded) {
			GroupRowInfo groupRow = GetGroupRowByValues(values);
			if(groupRow != null) ChangeColumnExpanded(groupRow, expanded);
		}
		protected void ChangeColumnExpanded(GroupRowInfo groupRow, bool expanded) {
			if(groupRow == null || groupRow.Expanded == expanded) return;
			groupRow.Expanded = expanded;
			BuildVisibleIndexes();
			VisualClientUpdateLayout();
		}
		public void ChangeColumnExpanded(int columnIndex, bool expanded, object value) {
			if(columnIndex >= Columns.Count - 1) return;
			ArrayList list = GetGroupRowHandlesByGroupRowValue(columnIndex, value);
			bool hasChanged = false;
			for(int i = 0; i < list.Count; i ++)
				hasChanged |= GroupInfo.ChangeExpanded((int)list[i], expanded, false);
			if(hasChanged) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public override void Reset() {
			Columns.Clear();
			RowsKeeper.Clear();
			base.Reset();
		}
		public void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, GroupRowInfo groupRow, GroupRowInfo filterGroup) {
			ArrayList groupList = GetSummaryRows(groupRow);
			CreateSummaryGroups(groups, dataHelpers, groupList, filterGroup);
		}
		public void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, int startIndex, int endIndex, int filterStartIndex, int filterEndIndex) {
			ArrayList groupList = GetGroupRows(startIndex, endIndex);
			CreateSummaryGroups(groups, dataHelpers, groupList, filterStartIndex, filterEndIndex);
		}
		protected virtual void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, ArrayList groupList, int filterStartIndex, int filterEndIndex) {
			CreateSummaryGroups(groups, dataHelpers, groupList, null);	
		}
		protected virtual void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, ArrayList groupList, GroupRowInfo filterGroup) {
			for(int i = 0; i < groupList.Count; i ++) {
				groups.Add(groupList[i]);
				dataHelpers.Add(this);
			}
		}
		protected ArrayList GetSummaryRows(GroupRowInfo groupRow) {
			ArrayList rowGroups = new ArrayList();
			int rowLevel = GroupInfo.LevelCount - 1;
			if(groupRow != null && groupRow.Level == rowLevel) {
				rowGroups.Add(groupRow);
			} else {
				GroupInfo.GetChildrenGroups(groupRow, rowGroups, rowLevel);
			}
			return rowGroups;
		}
		protected override void BuildVisibleIndexes() {
			if(GroupInfo.Count > 0)
				VisibleIndexes.BuildVisibleIndexes(VisibleCountCore, false, false);
			else 
				VisibleIndexes.Clear();
		}
		protected override PivotColumnInfo[] CreateSummaryColumns() { return Columns.ToArray(); }
		protected virtual ArrayList GetGroupRowHandlesByGroupRowValue(int columnIndex, object value) {
			ArrayList list = new ArrayList();
			int columnInfoIndex = Columns[columnIndex].ColumnInfo.Index;
			for(int i = 0; i < GroupInfo.Count; i ++) {
				if(GroupInfo[i].Level != columnIndex) continue;
				if(Controller.IsObjectEqualsFromHelper(GroupInfo,  GroupInfo[i].ChildControllerRow, columnInfoIndex, value))
					list.Add(GroupInfo[i].Handle);
			}
			return list;
		}
		protected override void ClearVisialIndexesAndGroupInfo() {
			base.ClearVisialIndexesAndGroupInfo();
			VisibleIndexes.Clear();
			PrepareSortInfo();
		}
		protected virtual void PrepareSortInfo() {
			SortInfo.Clear();
			AddColumnsToSortInfo(Columns);
			GroupInfo.LastExpandableLevel = Columns.Count - 1;
		}
		public virtual void ChangeExpanded(int visibleIndex, bool expanded, bool recursive) {
			int groupRowHandle = GetControllerRowHandle(visibleIndex);
			if(GroupInfo.ChangeExpanded(groupRowHandle, expanded, recursive)) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		protected virtual void VisualClientUpdateLayout() {
			Controller.VisualClientUpdateLayout();
		}
		protected void OnColumnInfoCollectionChanged(object sender, CollectionChangeEventArgs e) {
			Controller.DoRefresh();
		}
	}
	public class ColumnPivotDataControllerArea : PivotDataControllerArea {
		public ColumnPivotDataControllerArea(PivotDataController controller) : base(controller) {}
		protected override GroupRowInfo GetPrevColumnGroupRow(GroupRowInfo groupRow) { 
			return GetPrevGroupRow(groupRow, 0); 
		}
		protected override GroupRowInfo GetPrevRowGroupRow(GroupRowInfo groupRow) { 
			return null; 
		}
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return Columns[groupRow.Level];
		}
	}
	public class DataControllerRowGroupInfo : DataControllerGroupHelperBase {
		GroupRowInfo parentGroupRow;
		PivotColumnInfo[] summaryColumns;
		public DataControllerRowGroupInfo(PivotDataController controller, GroupRowInfo parentGroupRow) : base(controller) {
			this.parentGroupRow = parentGroupRow;
		}
		public GroupRowInfo ParentGroupRow { get { return parentGroupRow; } }
		protected internal override PivotSummaryItemCollection Summaries { get { return Controller.RowArea.RowGroupsSummaries; } }
		protected override PivotColumnInfo[] CreateSummaryColumns() {
			if(summaryColumns != null && summaryColumns.Length > 0)
				return summaryColumns;
			ArrayList list = new ArrayList();
			for(int i = 0; i < Controller.ColumnArea.Columns.Count; i++)
				list.Add(Controller.ColumnArea.Columns[i]);
			summaryColumns = (PivotColumnInfo[])list.ToArray(typeof(PivotColumnInfo));
			return summaryColumns;
		}
		public override void DoRefresh() {
			base.DoRefresh();
			DoAddOthers();
		}
		public void DoAddOthers() {
			bool createOthers;
			IComparer[] comparers = Controller.GetComparers(Controller.ColumnArea, Controller.RowArea, out createOthers);
			if(comparers != null || createOthers) {
				bool refreshRequired = DoConditionalSortSummaryAndAddOthers(comparers, true);
				if(refreshRequired)
					throw new Exception("Unexpected refresh required");
			}			
		}
		protected override void DoSortRows() {
			if(!IsSorted) return;
			int[] list = new int[ParentGroupRow.ChildControllerRowCount];
			for(int i = 0; i < list.Length; i ++) {
				list[i] = Controller.RowArea.GetListSourceRowByControllerRow(ParentGroupRow.ChildControllerRow + i);
			}
			Controller.SetVisibleListSourceCollection(VisibleListSourceRows, list, list.Length);
			DoSortRowsCore();
		}
		protected override int GetGroupedCount(GroupRowInfoCollection groups, GroupRowInfo parentGroup, bool useCountCache, int level, PivotColumnInfo pivotColumnInfo, ArrayList list) {
			int groupedCount = base.GetGroupedCount(groups, parentGroup, useCountCache, level, pivotColumnInfo, list);
			if(!summaryColumns[level].ShowOthersValue)
				return groupedCount;
			object[] parentValues = Controller.RowArea.GetGroupRowValues(parentGroup);
			object[] values = new object[parentValues.Length + 1];
			Array.Copy(parentValues, values, parentValues.Length);
			for(int i = 0; i < groupedCount; i++) {
				GroupRowInfo group = (GroupRowInfo)list[i];
				values[values.Length - 1] = GetValue(group);
				GroupRowInfo columnGroup = Controller.ColumnArea.GetGroupRowByValues(values);
				if(columnGroup == null || Controller.ColumnArea.GetIsOthersValue(columnGroup))
					return i;
			}
			return groupedCount;
		}
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return Controller.ColumnArea.Columns[groupRow.Level];
		}
		public override PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			return Controller.RowArea.GetRowColumnByGroupRow(ParentGroupRow);
		}
		protected internal override DataColumnSortInfoCollection SortInfo { get { return Controller.ColumnArea.SortInfo; } }
		protected override GroupRowInfo GetPrevRowGroupRow(GroupRowInfo groupRow) { 
			GroupRowInfo prevGroupRow = base.GetPrevRowGroupRow(groupRow);
			if(prevGroupRow != null) return prevGroupRow;
			DataControllerRowGroupInfo  controllerRowGroupInfo = Controller.RowArea.GetPrevControllerRowGroup(ParentGroupRow);
			return controllerRowGroupInfo != null ? controllerRowGroupInfo.GetLastGroupRow(groupRow.Level) : null;
		}
		GroupRowInfo GetLastGroupRow(int level) {
			for(int i = GroupInfo.Count - 1; i >= 0; i --) {
				if(GroupInfo[i].Level == level) 
					return GroupInfo[i];
			}
			return null;
		}
		public void CreateSummaryGroupRows(ArrayList groups, ArrayList dataHelpers, int filterStartIndex, int filterEndIndex) {
			if(filterStartIndex == -1) {
				groups.Add(null);
				dataHelpers.Add(this);
				return;
			}
			ArrayList list = new ArrayList();
			GroupInfo.GetChildrenGroups(null, list, GroupInfo.LevelCount - 1);			
			for(int i = 0; i < list.Count; i++) {
				GroupRowInfo groupInfo = (GroupRowInfo)list[i];
				int visibleIndex = Controller.ColumnArea.GetVisibleIndexByValues(GetGroupRowValues(groupInfo));
				if(visibleIndex >= filterStartIndex && visibleIndex < filterEndIndex) {
					groups.Add(list[i]);
					dataHelpers.Add(this);
				}
			}
		}
		public void CreateSummaryGroupRows(ArrayList groups, ArrayList dataHelpers, GroupRowInfo filterGroup) {
			ArrayList list = new ArrayList();
			GroupInfo.GetChildrenGroups(null, list, GroupInfo.LevelCount - 1);
			object[] filterValues = filterGroup != null ? Controller.ColumnArea.GetGroupRowValues(filterGroup) : null;
			for(int i = 0; i < list.Count; i++) {
				if(IsGroupFit(list[i] as GroupRowInfo, filterValues)) {
					groups.Add(list[i]);
					dataHelpers.Add(this);
				}
			}
		}
		bool IsGroupFit(GroupRowInfo groupRow, object[] filterValues) {
			if(filterValues == null) return true;
			object[] values = GetGroupRowValues(groupRow);
			for(int i = 0; i < filterValues.Length; i ++) {
				if(!Controller.IsEqualGroupValues(values[i], filterValues[i])) return false;
			}
			return true;
		}		
	}
	public class PivotGroupRowInfo : GroupRowInfo {
		DataControllerRowGroupInfo controllerRowGroupInfo = null;
		public PivotGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroup) : base(level, childControllerRow, parentGroup) { 
		}
		public DataControllerRowGroupInfo ControllerRowGroupInfo {
			get { return controllerRowGroupInfo; }
			set { controllerRowGroupInfo = value; }
		}
		public bool HasSummary { get { return Summary != null; } }
	}
	public class PivotGroupRowInfoCollection : GroupRowInfoCollection {
		public PivotGroupRowInfoCollection(DataControllerBase controller, DataColumnSortInfoCollection sortInfo, 
			VisibleListSourceRowCollection visibleListSourceRows) : base(controller, sortInfo, visibleListSourceRows) {
		}
		protected override GroupRowInfo CreateGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroupRow) {
			return new PivotGroupRowInfo(level, childControllerRow, parentGroupRow);
		}
	}
	public class RowPivotDataControllerArea : PivotDataControllerArea {
		PivotDataControllerArea columnArea;
		PivotSummaryItemCollection rowGroupsSummaries;
		PivotColumnInfo[] summaryColumns;
		public RowPivotDataControllerArea(PivotDataController controller, PivotDataControllerArea columnArea) : base(controller) {
			this.columnArea = columnArea;
			this.rowGroupsSummaries = null;
			this.summaryColumns = new PivotColumnInfo[0];
		}
		protected override GroupRowInfoCollection CreateGroupRowInfoCollection() {
			return new PivotGroupRowInfoCollection(Controller, SortInfo, VisibleListSourceRows);
		}
		public PivotSummaryItemCollection RowGroupsSummaries { get { return rowGroupsSummaries; } }
		public PivotColumnInfo[] SummaryColumns { get { return summaryColumns; } }
		public DataControllerRowGroupInfo GetControllerRowGroup(GroupRowInfo groupRow) {
			DataControllerRowGroupInfo controllerGroupInfo = (groupRow as PivotGroupRowInfo).ControllerRowGroupInfo;
			return controllerGroupInfo != null ? controllerGroupInfo : CreateControllerGroupRow(groupRow);
		}
		protected DataControllerRowGroupInfo CreateControllerGroupRow(GroupRowInfo groupRow) {
			DataControllerRowGroupInfo controllerGroupInfo = new DataControllerRowGroupInfo(Controller, groupRow);
			SetControllerRowGroup(groupRow, controllerGroupInfo);
			controllerGroupInfo.DoRefresh();
			return controllerGroupInfo;
		}
		protected void SetControllerRowGroup(GroupRowInfo groupRow, DataControllerRowGroupInfo value) {
			(groupRow as PivotGroupRowInfo).ControllerRowGroupInfo = value;
		}
		public void ClearControllerRowGroups() {
			for(int i = 0; i < GroupInfo.Count; i++) {
				SetControllerRowGroup(GroupInfo[i], null);
			}
		}
		public DataControllerRowGroupInfo GetPrevControllerRowGroup(GroupRowInfo groupRow) {
			groupRow = GetPrevGroupRow(groupRow, 0);
			return groupRow != null ? GetControllerRowGroup(groupRow) : null;
		}
		protected PivotDataControllerArea ColumnArea { get { return columnArea; } }
		protected override PivotColumnInfo[] CreateSummaryColumns() { 
			if(summaryColumns.Length > 0)
				return summaryColumns;
			ArrayList list = new ArrayList();
			for(int i = 0; i < Columns.Count; i ++)
				list.Add(Columns[i]);
			for(int i = 0; i < Controller.ColumnArea.Columns.Count; i ++)
				list.Add(Controller.ColumnArea.Columns[i]);
			summaryColumns = (PivotColumnInfo[])list.ToArray(typeof(PivotColumnInfo));
			return summaryColumns;
		}
		protected override void UpdateGroupSummaryCore() {
			summaryColumns = new PivotColumnInfo[0];
			base.UpdateGroupSummaryCore();
		}
		protected override int GetRowGroupCount() { return Columns.Count;  }
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return groupRow.Level >= Columns.Count ? Controller.ColumnArea.Columns[groupRow.Level - Columns.Count] : null;
		}
		public override PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			if(groupRow.Level < Columns.Count) return Columns[groupRow.Level]; 
			return Columns.Count > 0 ? Columns[Columns.Count - 1] : null;
		}
		public override void DoRefresh() {
			base.DoRefresh();
			CreateGroupInfos();
		}
		protected void CreateGroupInfos() {
			if(Controller.ColumnArea.Columns.Count == 0) {
				this.rowGroupsSummaries = null;
			} else {
				this.rowGroupsSummaries = CreateSummaryItems(Controller.ColumnArea.Columns.ToArray());
			}
		}
		public override void UpdateGroupSummary() {
			base.UpdateGroupSummary();
			if(Controller.ColumnArea.Columns.Count > 0) {
				this.rowGroupsSummaries = CreateSummaryItems(Controller.ColumnArea.Columns.ToArray());
				for(int i = 0; i < GroupInfo.Count; i ++) {
					SetControllerRowGroup(GroupInfo[i], null);
				}
			}
		}
		public GroupRowInfo GetSummaryGroupRow(GroupRowInfo groupRow, object[] values, out VisibleListSourceRowCollection cellVisibleListSourceRows) {
			DataControllerRowGroupInfo controllerGroupInfo = GetControllerRowGroup(groupRow);
			GroupRowInfo prevGroupRow = GetPrevGroupRow(groupRow, 0);
			if(prevGroupRow != null) {
				if(GetControllerRowGroup(prevGroupRow) == null) {
					controllerGroupInfo.UpdateGroupSummary();
				}
			}
			cellVisibleListSourceRows = controllerGroupInfo.VisibleListSourceRows; 
			return controllerGroupInfo.GetGroupRowByValues(values);
		}
		protected override void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, ArrayList groupList, GroupRowInfo filterGroup) {
			if(ColumnArea.Columns.Count > 0) {
				for(int i = 0; i < groupList.Count; i ++)
					CreateSummaryGroups(groups, dataHelpers, groupList[i] as PivotGroupRowInfo, filterGroup);
			} else
				base.CreateSummaryGroups(groups, dataHelpers, groupList, filterGroup);
		}
		protected override void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, ArrayList groupList, int filterStartIndex, int filterEndIndex) {
			if(ColumnArea.Columns.Count > 0) {
				for(int i = 0; i < groupList.Count; i++)
					CreateSummaryGroups(groups, dataHelpers, groupList[i] as PivotGroupRowInfo, filterStartIndex, filterEndIndex);
			} else
				base.CreateSummaryGroups(groups, dataHelpers, groupList, filterStartIndex, filterEndIndex);		
		}
		void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, PivotGroupRowInfo pivotGroupRow, GroupRowInfo filterGroup) {
			GetControllerRowGroup(pivotGroupRow).CreateSummaryGroupRows(groups, dataHelpers, filterGroup);
		}
		void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, PivotGroupRowInfo pivotGroupRow, int filterStartIndex, int filterEndIndex) {
			if(pivotGroupRow == null) return;
			if(filterStartIndex == -1) {
				groups.Add(pivotGroupRow);
				dataHelpers.Add(this);
				return;
			}
			GetControllerRowGroup(pivotGroupRow).CreateSummaryGroupRows(groups, dataHelpers, filterStartIndex, filterEndIndex);
		}
	}
	public enum PivotFilterType {Excluded, Included};
	public class PivotFilteredValues {
		NullableHashtable values;
		PivotFilterType filteredType;
		bool showBlanks;
		public PivotFilteredValues(NullableHashtable values, PivotFilterType filteredType, bool showBlanks) {
			if(values == null)
				values = new NullableHashtable();
			this.values = values;
			this.filteredType = filteredType;
			this.showBlanks = showBlanks;
		}
		public PivotFilteredValues() : this(null, PivotFilterType.Excluded, false) {
		}
		public NullableHashtable Values { get { return values; } }
		public PivotFilterType FilteredType { get { return filteredType; } }
		public bool ShowBlanks { get { return showBlanks; } }
		public bool IsValueFit(object value) {
			if(value == null) 
				return ShowBlanks;
			else return FilteredType == PivotFilterType.Included ? Values.ContainsKey(value) : !Values.ContainsKey(value);
		}
	}
	public interface IPivotClient {
		bool HasFilters { get; }
		bool IsDesignMode { get; }
		bool IsUpdateLocked { get; }
		void SetFilteredValues(PivotFilteredValues[] values);
		bool NeedCalcCustomSummary(DataColumnInfo columnInfo);
		void CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo);
		bool HasColumnVariation(DataColumnInfo columnInfo);
		bool HasRowVariation(DataColumnInfo columnInfo);
		void PopulateColumns();
		void UpdateLayout();
		string GetFieldCaption(DataColumnInfo columnInfo);
		CriteriaOperator PrefilterCritera { get; }
	}
	#region IPivotDataController
	#endregion
	public class PivotDataController : DataControllerBase, IEvaluatorDataAccess{
		FilterHelper filterHelper;
		ColumnPivotDataControllerArea columnArea;
		RowPivotDataControllerArea rowArea;
		Hashtable totalValues;
		VisibleListSourceRowCollection visibleListSourceRows;
		IPivotClient pivotClient;
		PivotSummaryItemCollection summaries;
		bool cacheData = true; 
		bool caseSensitive = true;
		public const string DataStreamSign = "PGDHLPER";
		public const string StreamSign = "PIVOTDC";	
		public PivotDataController() {
			this.columnArea = new ColumnPivotDataControllerArea(this);
			this.summaries = new PivotSummaryItemCollection(this, new CollectionChangeEventHandler(OnSummaryCollectionChanged));
			this.rowArea = new RowPivotDataControllerArea(this, ColumnArea);
			this.visibleListSourceRows = new VisibleListSourceRowCollection(this);
			this.filterHelper = new FilterHelper(this, VisibleListSourceRows);
			this.totalValues = new Hashtable();
			this.pivotClient = null;
		}
		public PivotDataControllerArea ColumnArea { get { return columnArea; } }
		public RowPivotDataControllerArea RowArea { get { return rowArea; } }
		public new IList ListSource {
			get { return base.ListSource; }
			set {
				BeginUpdate();
				try {
					SetListSource(value);
				} finally {
					EndUpdate();
				}
			}
		}
		public IPivotClient PivotClient { get { return pivotClient; } set { pivotClient = value; } }
		public FilterHelper FilterHelper { get { return filterHelper; } }
		public PivotSummaryItemCollection Summaries { get { return summaries; } }
		public bool CaseSensitive { 
			get { return caseSensitive; } 
			set {
				if(caseSensitive == value) return;
				caseSensitive = value;
				OnListSourceChanged();
			} 
		}
		public bool SupportsUnboundColumns { get { return true; } }
		public bool IsDesignMode { get { return PivotClient != null ? PivotClient.IsDesignMode : false; } }
		public override bool IsUpdateLocked { get { return (PivotClient != null && PivotClient.IsUpdateLocked) || base.IsUpdateLocked; } }
		#region DataController protected methods
		public int GetListSourceRowByControllerRow(VisibleListSourceRowCollection visibleListRowCollection, int controllerRow) {
			return GetListSourceFromVisibleListSourceRowCollection(visibleListRowCollection, controllerRow);
		}
		public void SetVisibleListSourceCollection(VisibleListSourceRowCollection visibleListSourceRowCollection, int[] list, int count) {
			SetVisibleListSourceCollectionCore(visibleListSourceRowCollection, list, count);
		}
		public object GetRowValueFromHelper(GroupRowInfoCollection groupInfo, int childControllerRow, int columnIndex) {
			int listSource = GetListSourceRowIndex(groupInfo, childControllerRow);
			return Helper.GetRowValue(listSource, columnIndex);
		}
		public bool IsObjectEqualsFromHelper(GroupRowInfoCollection groupInfo, int childControllerRow, int columnIndex, object valueToCompare) {
			return ValueComparer.ObjectEquals(GetRowValueFromHelper(groupInfo, childControllerRow, columnIndex), valueToCompare);
		}
		public void CreateColumnStorages(DataColumnSortInfoCollection SortInfo, VisibleListSourceRowCollection VisibleListSourceRows) {
			SortInfo.CreateColumnStorages(VisibleListSourceRows, Helper);
		}
		public void ResetSortInfoCollection(DataColumnSortInfoCollection sortInfo) {
			ResetSortInfoCollectionCore(sortInfo);
		}
		public void VisibleListSourceCollectionQuickSort(VisibleListSourceRowCollection visibleListSourceRowCollection,
			DataColumnSortInfoCollection sortInfo, int left, int right) {
			VisibleListSourceCollectionQuickSortCore(visibleListSourceRowCollection, sortInfo, left, right);
		}
		public new void VisualClientUpdateLayout() {
			base.VisualClientUpdateLayout();
		}
		public new void DoGroupColumn(DataColumnSortInfoCollection sortInfo, GroupRowInfoCollection groupInfo, int controllerRow, int rowCount, GroupRowInfo parentGroup) {
			base.DoGroupColumn(sortInfo, groupInfo, controllerRow, rowCount, parentGroup);
		}
		#endregion
		protected PivotGridDataHelper CachedHelper { get { return base.Helper as PivotGridDataHelper; } }
		public bool CacheData { 
			get { return cacheData; }
			set {
				if(cacheData == value) return;
				cacheData = value;
				OnListSourceChanged();
			}
		}
		protected override BaseDataControllerHelper CreateHelper() {
			if(CacheData) {
				return new PivotGridDataHelper(base.CreateHelper(), this);
			}
			return base.CreateHelper();
		}
		protected override ValueComparer CreateValueComparer() {
			return new PivotValueComparer(this);
		}
		PivotDataControllerArea GetPivotArea(bool isColumn) {
			return isColumn ? ColumnArea : RowArea;
		}
		public PivotColumnInfo AddColumnToArea(bool isColumn, DataColumnInfo columnInfo, PivotSummaryType summaryType, 
				ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, 
				List<PivotSortByCondition> sortbyConditions,
				int topValueCount, bool showTopRowsAbsolute, bool showOthersValue) {
			return GetPivotArea(isColumn).Columns.Add(columnInfo, summaryType, sortOrder, sortbyColumn,
				sortbySummaryType, sortbyConditions, topValueCount, showTopRowsAbsolute, showOthersValue);
		}
		public void AddSummary(PivotSummaryItem summaryItem) {
			Summaries.Add(summaryItem);
		}
		public void ChangeFieldExpanded(bool isColumn, int areaIndex, bool expanded) {
			GetPivotArea(isColumn).ChangeColumnExpanded(areaIndex, expanded);
		}
		public void ChangeFieldExpanded(bool isColumn, int areaIndex, bool expanded, object value) {
			GetPivotArea(isColumn).ChangeColumnExpanded(areaIndex, expanded, value);
		}
		public void ChangeExpanded(bool isColumn, int visibleIndex, bool expanded, bool recursive) {
			GetPivotArea(isColumn).ChangeExpanded(visibleIndex, expanded, recursive);
		}
		public void ChangeExpanded(bool isColumn, bool expanded) {
			GetPivotArea(isColumn).ChangeAllExpanded(expanded);
		}
		public void ClearAreaColumnsAndSummaries() {
			ColumnArea.Columns.Clear();
			RowArea.Columns.Clear();
			Summaries.Clear();
		}
		public bool IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return IsObjectCollapsed(isColumn, GetPivotArea(isColumn).GetGroupRowInfo(visibleIndex));
		}
		bool IsObjectCollapsed(bool isColumn, GroupRowInfo groupRow) {
			if(groupRow == null) return false;
			return (groupRow.Level < GetPivotArea(isColumn).Columns.Count - 1) && !groupRow.Expanded;
		}
		public object GetFieldValue(bool isColumn, int columnRowIndex, int areaIndex) {
			return GetPivotArea(isColumn).GetValue(columnRowIndex, areaIndex);
		}
		public int GetVisibleIndexByValues(bool isColumn, object[] values) {
			return GetPivotArea(isColumn).GetVisibleIndexByValues(values);
		}
		public bool GetIsOthersFieldValue(bool isColumn, int visibleIndex, int areaIndex) {
			return GetPivotArea(isColumn).GetIsOthersValue(visibleIndex, areaIndex);
		}
		public int GetObjectLevel(bool isColumn, int visibleIndex) {
			GroupRowInfo groupRow = GetPivotArea(isColumn).GetGroupRowInfo(visibleIndex);
			return groupRow != null ? groupRow.Level : -1;
		}
		public int GetLevelCount(bool isColumn) {
			return GetPivotArea(isColumn).Columns.Count;
		}
		public int GetCellCount(bool isColumn) {
			return GetPivotArea(isColumn).VisibleCount;
		}
		public object[] GetUniqueFieldValues(int columnIndex) {
			object[] result = FilterHelper.GetUniqueColumnValues(columnIndex, -1, true, false);
			return result == null ? new object[0] : result;
		}
		public object GetRowValue(int listSourceRow, string fieldName) {
			return GetRowValue(listSourceRow, base.Helper.Columns[fieldName]);
		}
		public object GetRowValue(int listSourceRow, DataColumnInfo column) {
			return column != null ? base.Helper.GetRowValue(listSourceRow, column.Index) : null;
		}
		public object GetRowValue(int listSourceRow, int columnIndex) {
			if(!IsColumnIndexValid(columnIndex)) return null;
			return base.Helper.GetRowValue(listSourceRow, columnIndex);
		}
		public void SetRowValue(int listSourceRow, string fieldName, object value) {
			SetRowValue(listSourceRow, base.Helper.Columns[fieldName], value);
		}
		public void SetRowValue(int listSourceRow, DataColumnInfo column, object value) {
			if(column == null) return;
			SetRowValue(listSourceRow, column.Index, value);
		}
		public void SetRowValue(int listSourceRow, int columnIndex, object value) {
			if(!IsColumnIndexValid(columnIndex)) return;
			base.Helper.SetRowValue(listSourceRow, columnIndex, value);
		}
		bool IsColumnIndexValid(int columnIndex) {
			return 0 <= columnIndex && columnIndex < base.Helper.Columns.Count;
		}
		public object GetNextOrPrevRowCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType, bool isNext) { 
			rowVisibleIndex = RowArea.GetNextOrPrevVisibleIndex(rowVisibleIndex, isNext);
			if(rowVisibleIndex < 0) return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, summaryType);
		}
		public object GetNextOrPrevColumnCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType, bool isNext) { 
			columnVisibleIndex = ColumnArea.GetNextOrPrevVisibleIndex(columnVisibleIndex, isNext);
			if(columnVisibleIndex < 0) return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, summaryType);
		}
		public object GetCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType) {
			PivotSummaryValue summaryValue = GetCellSummaryValue(columnVisibleIndex, rowVisibleIndex, dataIndex);
			return summaryValue != null ? summaryValue.GetValue(summaryType) : null;
		}
		public object GetCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex) {
			if(Summaries.Count == 0) return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, Summaries[dataIndex].SummaryType);
		}
		public PivotSummaryValue GetCellSummaryValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex) {
			return GetCellSummaryValue(ColumnArea.GetGroupRowInfo(columnVisibleIndex),
				RowArea.GetGroupRowInfo(rowVisibleIndex), dataIndex);
		}
		protected PivotSummaryValue GetCellSummaryValue(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, int dataIndex) {
			if(dataIndex < 0 || dataIndex >= Summaries.Count) return null;
			if(columnGroupRow == null && rowGroupRow == null)
				return GetTotalCellValue(dataIndex);
			VisibleListSourceRowCollection dummy;
			GroupRowInfo summaryRow = GetSummaryGroupRow(columnGroupRow, rowGroupRow, out dummy);
			return summaryRow != null ? summaryRow.GetSummaryValue(Summaries[dataIndex]) as PivotSummaryValue : null;
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(int columnVisibleIndex, int rowVisibleIndex) {
			return CreateDrillDownDataSource(columnVisibleIndex, rowVisibleIndex, PivotDrillDownDataSource.AllRows);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(int columnVisibleIndex, int rowVisibleIndex, int maxRowCount) {
			VisibleListSourceRowCollection cellVisibleListSourceRows = null;
			GroupRowInfo columnGroupRow = ColumnArea.GetGroupRowInfo(columnVisibleIndex);
			GroupRowInfo rowGroupRow = RowArea.GetGroupRowInfo(rowVisibleIndex);
			if(columnGroupRow == null && rowGroupRow == null)
				return CreateDrillDownDataSource(VisibleListSourceRows, null, maxRowCount);
			GroupRowInfo summaryRow = GetSummaryGroupRow(columnGroupRow, rowGroupRow, out cellVisibleListSourceRows);
			if(summaryRow == null)
				cellVisibleListSourceRows = null;
			return CreateDrillDownDataSource(cellVisibleListSourceRows, summaryRow, maxRowCount);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow) {
			return CreateDrillDownDataSourceCore(cellVisibleListSourceRows, groupRow, PivotDrillDownDataSource.AllRows);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow, int maxRowCount) {
			return CreateDrillDownDataSourceCore(cellVisibleListSourceRows, groupRow, maxRowCount);
		}
		protected NativePivotDrillDownDataSource CreateDrillDownDataSourceCore(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow, int maxRowCount) {
			return new ClientPivotDrillDownDataSource(this, cellVisibleListSourceRows, groupRow, maxRowCount);
		}
		public int GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return GetPivotArea(isColumn).GetNextOrPrevVisibleIndex(visibleIndex, isNext);
		}
		public void ChangeFieldSortOrder(bool isColumn, int index) {
			if(isColumn) 
				ColumnArea.ChangeFieldSortOrder(index);
			else RowArea.ChangeFieldSortOrder(index);
		}
		public bool NeedCalcCustomSummary(DataColumnInfo columnInfo) {
			return PivotClient != null && PivotClient.NeedCalcCustomSummary(columnInfo);
		}
		public void CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo) {
			if(PivotClient != null) {
				PivotClient.CalcCustomSummary(customSummaryInfo);
			}
		}
		public bool HasColumnVariation(DataColumnInfo columnInfo) {
			return PivotClient != null ? PivotClient.HasColumnVariation(columnInfo) : false;
		}
		public bool HasRowVariation(DataColumnInfo columnInfo) {
			return PivotClient != null ? PivotClient.HasRowVariation(columnInfo) : false;
		}
		public bool SupportComparerCache(int column) {
			if(CachedHelper == null) return false;
			return CachedHelper.SupportComparerCache(column);
		}
		public bool HasComparerCache(int column) {
			if(CachedHelper == null) return false;
			return CachedHelper.HasComparerCache(column);
		}
		public void SetComparerCache(int column, int[] cache, bool isAscending) {
			if(CachedHelper == null) return;
			CachedHelper.SetComparerCache(column, cache, isAscending);
		}
		protected void ClearComparerCache(int column) {
			if(CachedHelper == null) return;
			CachedHelper.SetComparerCache(column, null, true);
		}
		public bool HasNullValues(int columnIndex) {
			if(CachedHelper != null) {
				return CachedHelper.HasNullValue(columnIndex);
			}
			for(int i = 0; i < VisibleListSourceRows.Count; i ++) {
				object value = Helper.GetRowValue(GetListSourceRowByControllerRow(VisibleListSourceRows, i), columnIndex);
				if(value == null || value == DBNull.Value) return true;
			}
			return false;
		}
		public void SaveDataToStream(Stream stream, bool compress) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(StreamSign);
			writer.Write(1);
			long startPosition = stream.Position;
			writer.Write(0L);
			writer.Write(0);
			int columnsCount = 0;
			for(int i = 0; i < Columns.Count; i++) {
				if(Columns[i].Unbound) continue;
				writer.Write(Columns[i].Name);
				writer.Write(Columns[i].Caption);
				writer.Write(Columns[i].Type.AssemblyQualifiedName);
				columnsCount++;
			}
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			writer.Write(columnsCount);
			stream.Position = endPosition;
			if(!CacheData) CacheData = true;
			CachedHelper.SaveToStream(stream, compress);			
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			ColumnArea.SaveFieldsStateToStream(stream);
			RowArea.SaveFieldsStateToStream(stream);
		}		
		public void LoadCollapsedStateFromStream(Stream stream) {
			ColumnArea.LoadFieldsStateFromStream(stream);
			RowArea.LoadFieldsStateFromStream(stream);
		}
		public void WebLoadCollapsedStateFromStream(Stream stream) {
			ColumnArea.WebLoadFieldsStateFromStream(stream);
			RowArea.WebLoadFieldsStateFromStream(stream);
		}
		public void WebSaveCollapsedStateToStream(Stream stream) {
			ColumnArea.WebSaveFieldsStateToStream(stream);
			RowArea.WebSaveFieldsStateToStream(stream);
		}
		public GroupRowInfo GetSummaryGroupRow(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow) {
			VisibleListSourceRowCollection dummy;
			return GetSummaryGroupRow(columnGroupRow, rowGroupRow, out dummy);
		}
		GroupRowInfo GetSummaryGroupRow(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, out VisibleListSourceRowCollection cellVisibleListSourceRows) {
			cellVisibleListSourceRows = null;
			if(rowGroupRow == null && columnGroupRow == null) return null;
			if(rowGroupRow == null) {
				cellVisibleListSourceRows = ColumnArea.VisibleListSourceRows;
				return columnGroupRow;
			}
			if(columnGroupRow == null) {
				cellVisibleListSourceRows = RowArea.VisibleListSourceRows;
				return rowGroupRow;
			}
			return RowArea.GetSummaryGroupRow(rowGroupRow, 
								ColumnArea.GetGroupRowValues(columnGroupRow), out cellVisibleListSourceRows);
		}
		PivotSummaryValue GetTotalCellValue(int dataIndex) {
			if(dataIndex < 0 || dataIndex >= Summaries.Count) return null;
			PivotSummaryItem summary = Summaries[dataIndex];
			PivotSummaryValue summaryValue = totalValues[summary] as PivotSummaryValue;
			if(summaryValue == null) 
				summaryValue = CalcTotalSummaryValue(summary);
			return summaryValue;
		}
		PivotSummaryValue CalcTotalSummaryValue(PivotSummaryItem summary) {
			PivotSummaryValue summaryValue = new PivotSummaryValue(ValueComparer);
			if(ColumnArea.GroupInfo.Count > 0)
				ColumnArea.CalcParentGroupRowSummary(null, summary, summaryValue);
			else {
				if(RowArea.GroupInfo.Count > 0)
					RowArea.CalcParentGroupRowSummary(null, summary, summaryValue);
				else {
					CalcLastLevelGroupRowSummary(VisibleListSourceRows, summary, summaryValue, 0, VisibleListSourceRows.Count);
				}
			}
			if(NeedCalcCustomSummary(summary.ColumnInfo))
				CalcCustomSummary(new PivotCustomSummaryInfo(null, VisibleListSourceRows, summary, summaryValue, null));
			totalValues[summary] = summaryValue;
			return summaryValue;
		}
		public void CalcLastLevelGroupRowSummary(VisibleListSourceRowCollection visibleListRows, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, int startIndex, int count) {
			object value = null;
			decimal numericValue = 0;
			int colIndex = GetColumnIndex(summaryItem.ColumnInfo);
			for(int n = startIndex; n < startIndex + count; n++) {
				value = Helper.GetRowValue(GetListSourceRowByControllerRow(visibleListRows, n), colIndex);
				summaryItem.ConvertValue(value, out numericValue);
				summaryValue.AddValue(value, numericValue);
			}
		}
		protected internal VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		protected override void EndUpdateCore(bool sortUpdate) {
			base.EndUpdateCore(sortUpdate);
			if(!IsUpdateLocked)
				DoRefresh(false);
		}
		protected override void Reset() {
			RowArea.Reset();
			ColumnArea.Reset();
			this.totalValues.Clear();
			this.summaries.Clear();
		}
		protected bool IsRefreshInProgress { get { return refreshInProgress != 0; } }
		int refreshInProgress = 0;
		public void ClientPopulateColumns() {
			DoSaveGroupRowsState();
			if(CachedHelper != null) {
				CachedHelper.ClearStorage();
			}
			if(PivotClient != null)
				PivotClient.PopulateColumns();
			if(CachedHelper != null) {
				CachedHelper.RefreshData();
			}
		}
		protected virtual void DoRefreshAreas() {
			ColumnArea.DoRefresh();
			RowArea.DoRefresh();
		}
		bool isGroupRowsStateSaved = false;
		protected void DoSaveGroupRowsState() {
			if(!isGroupRowsStateSaved && ListSourceRowCount > 0) {
				isGroupRowsStateSaved = true;
				ColumnArea.SaveGroupRowsState();
				RowArea.SaveGroupRowsState();
			}
		}
		protected override void DoRefresh(bool useRowsKeeper) {
			if(IsUpdateLocked) return;
			this.refreshInProgress ++;			
			DoSaveGroupRowsState();
			try {
				this.totalValues.Clear();
				DoFilterRows();
				DoRefreshAreas();
				DoCrossAreaRefresh();
				ColumnArea.RestoreGroupRowsState();
				RowArea.RestoreGroupRowsState();
				if(this.ListSource != null) {
					ColumnArea.SaveGroupRowsColumns();
					RowArea.SaveGroupRowsColumns();
				}
			} finally {
				this.isGroupRowsStateSaved = false;
				this.refreshInProgress --;
				if(PivotClient != null)
					PivotClient.UpdateLayout();
			} 
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			base.OnBindingListChanged(e);
			switch(e.ListChangedType) {
				case ListChangedType.Reset : {
					PopulateColumns();
					break;
				}
			}
			OnBindingListChangedCore(e);
		}
		protected virtual void OnBindingListChangedCore(ListChangedEventArgs e) {
			if(IsRefreshInProgress || CacheData) return;
			DoRefresh();
		}
		protected virtual void DoFilterRows() {
			ExpressionEvaluator eval = FilterExpressionEvaluator;
			if(PivotClient == null || (ListSourceRowCount == 0) || (!PivotClient.HasFilters && eval == null)) {
				VisibleListSourceRows.CreateList(ListSourceRowCount);
				return;
			}
			bool hasFilters = PivotClient.HasFilters,
				hasPrefilter = eval != null;
			int[] filteredRows = new int[ListSourceRowCount];
			int filteredRowsCount = 0;
			PivotFilteredValues[] filteredValues = hasFilters ? GetFilteredValues() : null;
			try {
				for(int i = 0; i < ListSourceRowCount; i++) {
					if((hasFilters ? IsRowFit(i, filteredValues) : true) &&
						(hasPrefilter ? eval.Fit(i) : true)) {
						filteredRows[filteredRowsCount++] = i;
					}
				}
			} catch { }
			SetVisibleListSourceCollection(VisibleListSourceRows, filteredRows, filteredRowsCount);
		}
		PivotFilteredValues[] GetFilteredValues() {
			PivotFilteredValues[] filteredValues = new PivotFilteredValues[Columns.Count];
			for(int i = 0; i < filteredValues.Length; i++) {
				filteredValues[i] = null;
			}
			PivotClient.SetFilteredValues(filteredValues);
			return filteredValues;
		}		
		protected virtual bool IsRowFit(int listSourceRow, PivotFilteredValues[] filteredValues) {
			for(int i = 0; i < Columns.Count; i ++) {
				if(filteredValues[i] == null) continue;
				if(!filteredValues[i].IsValueFit(Helper.GetRowValue(listSourceRow, i)))
					return false;
			}
			return true;
		}
		protected ExpressionEvaluator FilterExpressionEvaluator {
			get {
				ExpressionEvaluator filterExpressionEvaluator = null;
				if(PivotClient != null && !ReferenceEquals(PivotClient.PrefilterCritera, null))
					filterExpressionEvaluator = CreateExpressionEvaluator(PivotClient.PrefilterCritera);
				return filterExpressionEvaluator;
			}
		}
		ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator expression) {
			if(!IsReady) return null;
			if(ReferenceEquals(expression, null)) return null;
			try {
				ExpressionEvaluator ev = new ExpressionEvaluator(GetFilterDescriptorCollection(), expression, CaseSensitive);
				ev.DataAccess = this;
				return ev;
			} catch {
				return null;
			}
		}
		PropertyDescriptorCollection GetFilterDescriptorCollection() {
			PropertyDescriptor[] pds = new PropertyDescriptor[Columns.Count];
			for(int n = 0; n < Columns.Count; n++) 
				pds[n] = Columns[n].PropertyDescriptor;		
			return new PropertyDescriptorCollection(pds);
		}
		protected virtual void DoCrossAreaRefresh() {
			ClearComparerCache(RowArea);
			ClearComparerCache(ColumnArea);
			bool rowDoRefresh = DoCrossAreaRefresh(RowArea, ColumnArea, true),
				columnDoRefresh = DoCrossAreaRefresh(ColumnArea, RowArea, true);
			RowArea.ClearControllerRowGroups();
			if(rowDoRefresh || columnDoRefresh) {		
				int[] rows = null;
				if(rowDoRefresh && !columnDoRefresh)
					rows = RowArea.GroupInfo.VisibleListSourceRows.ToArray();
				if(!rowDoRefresh && columnDoRefresh)
					rows = ColumnArea.GroupInfo.VisibleListSourceRows.ToArray();
				if(rowDoRefresh && columnDoRefresh) {
					int[] rowAreaRows = RowArea.GroupInfo.VisibleListSourceRows.ToArray(),
						columnAreaRows = ColumnArea.GroupInfo.VisibleListSourceRows.ToArray();
					rows = IntersectArrays(rowAreaRows, columnAreaRows);				
				}
				SetVisibleListSourceCollection(VisibleListSourceRows, rows, rows.Length);
				DoRefreshAreas();
				rowDoRefresh = DoCrossAreaRefresh(RowArea, ColumnArea, false);
				columnDoRefresh = DoCrossAreaRefresh(ColumnArea, RowArea, false);
				if(rowDoRefresh || columnDoRefresh)
					throw new Exception("Double entrance");
				RowArea.ClearControllerRowGroups();
			}
		}
		internal static int[] IntersectArrays(int[] array1, int[] array2) {
			Array.Sort<int>(array1);
			Array.Sort<int>(array2);
			List<int> res = new List<int>();
			int i1 = 0, i2 = 0;
			while(i1 < array1.Length && i2 < array2.Length) {
				int cmp = Comparer<int>.Default.Compare(array1[i1], array2[i2]);
				switch(cmp) {
					case 0:
						res.Add(array1[i1]);
						i1++;
						i2++;
						break;
					case 1:
						i2++;
						break;
					case -1:
						i1++;
						break;
				}
			}
			return res.ToArray();
		}
		void ClearComparerCache(PivotDataControllerArea area) {
			for(int i = 0; i < area.Columns.Count; i++) {
				ClearPivotColumnComparerCache(area.Columns[i]);
			}
		}
		void ClearPivotColumnComparerCache(PivotColumnInfo column) {
			if(column.ShowTopRows > 0) {
				ClearComparerCache(column.ColumnInfo.Index);
			}
		}
		protected bool DoCrossAreaRefresh(PivotDataControllerArea area, PivotDataControllerArea secondArea, bool firstPass) {
			return DoCrossAreaSort(area, secondArea, firstPass);
		}
		protected bool DoCrossAreaSort(PivotDataControllerArea area, PivotDataControllerArea secondArea, bool firstPass) {
			IComparer[] comparers = null;
			bool createOthers;
			comparers = GetComparers(area, secondArea, out createOthers);
			if(comparers != null || createOthers) {
				return area.DoConditionalSortSummaryAndAddOthers(comparers, firstPass);
			}
			return false;
		}
		public IComparer[] GetComparers(PivotDataControllerArea area, PivotDataControllerArea secondArea, out bool createOthers) {
			IComparer[] comparers = null;
			createOthers = false;
			for(int i = 0; i < area.Columns.Count; i++) {
				createOthers |= area.Columns[i].ContainsOthersValue;
				IComparer comparer = GetSortComparer(area, secondArea, area.Columns[i]);
				if(comparer != null) {
					if(comparers == null)
						comparers = new IComparer[area.Columns.Count];
					comparers[i] = comparer;
				}
			}
			return comparers;
		}
		IComparer GetSortComparer(PivotDataControllerArea area, PivotDataControllerArea secondArea, PivotColumnInfo columnInfo) {
			IComparer comparer = null;
			if(columnInfo.ContainsSortSummaryConditions)
				comparer = GetConditionalComparer(columnInfo, area, secondArea);
			if(comparer == null && columnInfo.ContainsSortSummary) {
				comparer = new PivotGroupSummaryComparer(columnInfo.SortbyColumn.Tag as PivotGridFieldBase,
					this, area.Summaries.GetItem(columnInfo.SortbyColumn), columnInfo.SortOrder, columnInfo.SortbySummaryType);
			}
			return comparer;
		}
		IComparer GetConditionalComparer(PivotColumnInfo columnInfo, PivotDataControllerArea area, PivotDataControllerArea secondArea) {
			if(columnInfo.ContainsSortSummaryConditions) {
				GroupRowInfo sortbyGroup = GetSortByGroup(secondArea, columnInfo.SortbyConditions);
				PivotSummaryItem summaryItem = RowArea.RowGroupsSummaries.GetItem(columnInfo.SortbyColumn);
				if(sortbyGroup != null && summaryItem != null) {
					return new PivotConditionalGroupSummaryComparer(this, columnInfo.SortbyColumn.Tag as PivotGridFieldBase,
						sortbyGroup, columnInfo.SortOrder == ColumnSortOrder.Ascending, area == ColumnArea,
						summaryItem, columnInfo.SortbySummaryType);
				}
			}
			return null;
		}
		GroupRowInfo GetSortByGroup(PivotDataControllerArea secondArea, List<PivotSortByCondition> sortedList) {
			int condIndex = 0,
				level = sortedList[condIndex].Level;
			object value = sortedList[condIndex].Value;
			for(int i = 0; i < secondArea.GroupInfo.Count; i++) {
				GroupRowInfo groupRow = secondArea.GroupInfo[i];
				if(groupRow.Level < level)
					break;
				if(groupRow.Level == level && ValueComparer.Compare(secondArea.GetValue(groupRow), value) == 0) {
					if(condIndex == sortedList.Count - 1)
						return groupRow;
					else {
						condIndex++;
						level = sortedList[condIndex].Level;
						value = sortedList[condIndex].Value;
					}
				}
			}
			return null;
		}
		protected void DoCrossAreaFiltering(PivotDataControllerArea area, PivotDataControllerArea secondArea) {
			bool visibleListSourceRowsChanged = false;
			for(int i = 0; i < area.Columns.Count; i++) {
				bool changed = DoFilterColumnTopRows(area, i);
				if(changed)
					area.DoRefresh();
				visibleListSourceRowsChanged |= changed;
			}
			if(visibleListSourceRowsChanged) {
				secondArea.DoRefresh();
			}
		}		
		protected virtual bool DoFilterColumnTopRows(PivotDataControllerArea area, int columnIndex) {
			PivotColumnInfo columnInfo = area.Columns[columnIndex];
			if(columnInfo.ShowTopRows <= 0 || columnInfo.ShowOthersValue) return false;
			int listRowCount = 0;
			int[] listRows = new int[VisibleListSourceRows.Count];
			ArrayList topGroupRows = GetTopGroupRows(area.GroupInfo, columnIndex, columnInfo);
			for(int i = 0; i < topGroupRows.Count; i ++) {
				GroupRowInfo groupRow = topGroupRows[i] as GroupRowInfo;
				for(int j = 0; j < groupRow.ChildControllerRowCount; j ++)
					listRows[listRowCount ++] = groupRow.ChildControllerRow + j;
			}
			if(listRowCount < VisibleListSourceRows.Count) {
				for(int i = 0; i < listRowCount; i ++)
					listRows[i] = area.GetListSourceRowByControllerRow(listRows[i]);
				SetVisibleListSourceCollection(VisibleListSourceRows, listRows, listRowCount);
				return true;
			} else return false;
		}
		protected ArrayList GetTopGroupRows(GroupRowInfoCollection groupRows, int level, PivotColumnInfo columnInfo) {
			ArrayList list = new ArrayList();
			ArrayList levelList = new ArrayList();
			for(int i = 0; i < groupRows.Count; i ++) {
				if(groupRows[i].Level == level) {
					levelList.Add(groupRows[i]);
				}
				if(groupRows[i].Level < level) {
					CopyTopGroupRows(levelList, list, columnInfo);
				}
			}
			CopyTopGroupRows(levelList, list, columnInfo);
			return list;
		}
		void CopyTopGroupRows(ArrayList source, ArrayList destination, PivotColumnInfo columnInfo) {
			int count = columnInfo.GetTopRowsCount(source.Count);
			for(int i = 0; i < count; i ++) {
				destination.Add(source[i]);
			}
			source.Clear();
		}
		protected override bool IsEqualNonNullValues(object val1, object val2) {
			string x = val1 as string,
				y = val2 as string;
			if(x != null && y != null)
				return String.Compare(x, y, !CaseSensitive, CultureInfo.CurrentCulture) == 0;
			return val1.Equals(val2);
		}
		public virtual void UpdateGroupSummary() {
			if(IsUpdateLocked) return;
			this.totalValues.Clear();
			ColumnArea.UpdateGroupSummary();
			RowArea.UpdateGroupSummary();
		}
		void OnSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			UpdateGroupSummary();
		}
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			return GetRowValue((int)theObject, Columns[descriptor.Name].Index);
		}
		#endregion
	}
	public class PivotValueComparer : ValueComparer {
		PivotDataController controller;
		public PivotValueComparer(PivotDataController controller) {
			this.controller = controller;
		}
		protected bool CaseSensitive { get { return controller.CaseSensitive; } }
		protected StringComparison StringComparision {
			get {
				return CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			}
		}
		protected override int CompareCore(object x, object y) {
			string val1 = x as string,
				val2 = y as string;
			if(val1 != null && val2 != null)
				return String.Compare(val1, val2, !CaseSensitive, CultureInfo.CurrentCulture);
			else
				return base.CompareCore(x, y);
		}
		protected override bool ObjectEqualsCore(object x, object y) {
			string val1 = x as string,
				val2 = y as string;
			if(val1 != null && val2 != null)
				return String.Equals(val1, val2, StringComparision);
			else
				return base.ObjectEqualsCore(x, y);
		}
	}
	#region PivotGroupSummaryComparer
	public class PivotGroupSummaryComparer : IComparer {
		DataControllerBase controller;
		PivotSummaryItem summaryItem;
		ColumnSortOrder sortOrder;
		PivotSummaryType summaryType;
		PivotGridFieldBase field;
		public PivotGroupSummaryComparer(PivotGridFieldBase field, DataControllerBase controller, PivotSummaryItem summaryItem, ColumnSortOrder sortOrder, PivotSummaryType summaryType) {
			this.controller = controller;
			this.summaryItem = summaryItem;
			this.sortOrder = sortOrder;
			this.summaryType = summaryType;
			this.field = field;
		}
		protected DataControllerBase Controller { get { return controller; } }
		protected ValueComparer ValueComparer { get { return Controller.ValueComparer; } }
		protected PivotGridFieldBase Field { get { return field; } }
		public int Compare(object x, object y) {
			GroupRowInfo groupRow1 = (GroupRowInfo)x, groupRow2 = (GroupRowInfo)y;
			int res = 0;
			if(this.summaryItem != null) {
				res = Compare(groupRow1, groupRow2);
			}
			if(res == 0) res = Comparer.Default.Compare(groupRow1.Index, groupRow2.Index);
			if(this.sortOrder == ColumnSortOrder.Ascending) return res;
			res = (res > 0 ? -1 : 1);
			return res;
		}
		public int Compare(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			object val1 = GetValueByGroupRow(groupRow1);
			object val2 = GetValueByGroupRow(groupRow2);
			return ValueComparer.Compare(val1, val2);
		}
		object GetValueByGroupRow(GroupRowInfo groupRow) {
			PivotSummaryValue sumValue = groupRow.GetSummaryValue(summaryItem) as PivotSummaryValue;
			if(sumValue != null && summaryType == PivotSummaryType.Custom)
				return sumValue.GetCustomValue(Field);
			return sumValue != null ? sumValue.GetValue(summaryType) : null;
		}
	}
	#endregion
	#region PivotConditionalGroupSummaryComparer
	public class PivotConditionalGroupSummaryComparer : IComparer {
		readonly PivotDataController controller;
		readonly GroupRowInfo sortbyGroup;
		readonly PivotSummaryItem summaryItem;
		readonly PivotSummaryType summaryType;
		readonly ValueComparer valueComparer;
		readonly bool isColumn, isAscending;
		readonly PivotGridFieldBase field;
		public PivotConditionalGroupSummaryComparer(PivotDataController controller, PivotGridFieldBase field,
				GroupRowInfo sortbyGroup, bool isAscending,
				bool isColumn, PivotSummaryItem summaryItem, PivotSummaryType summaryType) {
			this.controller = controller;
			this.valueComparer = controller.ValueComparer;
			this.field = field;
			this.sortbyGroup = sortbyGroup;
			this.summaryItem = summaryItem;
			this.summaryType = summaryType;
			this.isColumn = isColumn;
			this.isAscending = isAscending;
		}
		#region IComparer Members
		public int Compare(object x, object y) {
			GroupRowInfo group1 = (GroupRowInfo)x, group2 = (GroupRowInfo)y;
			return CompareCore(group1, group2);
		}
		#endregion
		int CompareCore(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			object val1 = GetValueByGroupRow(groupRow1);
			object val2 = GetValueByGroupRow(groupRow2);
			int res = valueComparer.Compare(val1, val2);
			return isAscending ? res : -res;
		}
		object GetValueByGroupRow(GroupRowInfo groupRow) {
			GroupRowInfo summaryRow = controller.GetSummaryGroupRow(isColumn ? groupRow : sortbyGroup, isColumn ? sortbyGroup : groupRow);
			if(summaryRow == null) return null;
			PivotSummaryValue sumValue = (PivotSummaryValue)summaryRow.GetSummaryValue(summaryItem);
			if(sumValue == null) return null;
			if(summaryType == PivotSummaryType.Custom)
				return sumValue.GetCustomValue(field);
			else
				return sumValue.GetValue(summaryType);
		}
	}
	#endregion
	#region PivotServerModeDataControllerArea
	#endregion
	#region IPivotListServer
	#endregion
	#region PivotListSortDescription
	#endregion
	#region PivotServerModeDataController
	#endregion
}
