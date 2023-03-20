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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Runtime.Serialization;
using System.IO;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
using DevExpress.WebUtils;
using System.Collections.Generic;
using DevExpress.Utils.DateHelpers;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridNativeDataSource : IPivotGridDataSource, IDataControllerData2, IDataControllerSort, IPivotClient {
		PivotDataController dataController;
		PivotGridData pivotGridData;
		UnboundColumnInfo nullableColumn;
		UnboundColumnInfoCollection unboundColumns;
		DateTime refreshDate;
		Hashtable fieldsByColumns;
		Dictionary<DataColumnInfo, List<PivotGridFieldBase>> customSummaryColumnInfos;
		bool needPopulateColumns;
		#region PivotGridData properties
		protected PivotGridData GridData { get { return pivotGridData; } }
		protected PivotGridFieldCollectionBase Fields { get { return GridData.Fields; } }
		protected PivotGridOptionsDataField OptionsDataField { get { return GridData.OptionsDataField; } }
		protected int DataFieldCount { get { return GridData.DataFieldCount; } }
		protected PivotGridFieldBase GetFieldByArea(PivotArea area, int index) { return GridData.GetFieldByArea(area, index); }
		protected bool IsDesignMode { get { return GridData != null ? GridData.IsDesignMode : false; } }
		#endregion
		public PivotGridNativeDataSource(PivotGridData pivotGridData) {
			this.pivotGridData = pivotGridData;
			this.dataController = new PivotDataController();
			DataController.DataClient = this;
			DataController.SortClient = this;
			DataController.PivotClient = this;
			this.nullableColumn = new UnboundColumnInfo("PivotGridNullableColumn", UnboundColumnType.Integer, true);
			this.unboundColumns = new UnboundColumnInfoCollection();
			this.refreshDate = DateTime.Today;
			this.fieldsByColumns = new Hashtable();
			this.customSummaryColumnInfos = new Dictionary<DataColumnInfo, List<PivotGridFieldBase>>();
			this.needPopulateColumns = false;
		}
		public PivotDataController DataController { get { return dataController; } }
		protected DataColumnInfo GetColumnInfo(PivotGridFieldBase field) {
			if(field == null || field.ColumnHandle < 0) return null;
			return DataController.Columns[field.ColumnHandle];
		}
		PivotGridFieldBase GetFieldByPivotColumnInfo(DataColumnInfo columnInfo) {
			return columnInfo != null ? columnInfo.Tag as PivotGridFieldBase : null;
		}
		DateTime RefreshDate { get { return refreshDate; } set { refreshDate = value; } }
		void AddFieldIntoDataController(PivotGridFieldBase field, DataColumnInfo columnInfo) {
			this.fieldsByColumns[columnInfo] = field;
			if(!field.Visible) return;
			if(field.IsColumnOrRow) {
				DataColumnInfo sortedByColumn;
				PivotSummaryType sortBySummaryType;
				GetSortedByInfo(field.SortBySummaryInfo, out sortedByColumn, out sortBySummaryType);
				if(sortedByColumn != null && sortBySummaryType == PivotSummaryType.Custom) 
					AddCustomSummaryInfo(field.SortBySummaryInfo.Field, sortedByColumn);				
				ColumnSortOrder sortOrder = field.SortOrder == PivotSortOrder.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
				PivotColumnInfo pivotColumnInfo = DataController.AddColumnToArea(field.Area == PivotArea.ColumnArea, columnInfo, field.SummaryType, 
														sortOrder, sortedByColumn, sortBySummaryType,
														GetSortbyConditions(field),
														field.TopValueCount, field.TopValueType == PivotTopValueType.Absolute, field.TopValueShowOthers);
				if(pivotColumnInfo != null) 
					pivotColumnInfo.Tag = field;
				if(field.TotalsVisibility == PivotTotalsVisibility.CustomTotals && field.CustomTotals.Contains(PivotSummaryType.Custom)) {
					foreach(PivotGridFieldBase dataField in GridData.GetFieldsByArea(PivotArea.DataArea, false)) {
						DataColumnInfo dataColumn = DataController.Columns[GetFieldName(dataField)];
						if(dataColumn != null)
							AddCustomSummaryInfo(dataField, dataColumn);
					}
				}
			}
			if(field.Area == PivotArea.DataArea) {
				DataController.AddSummary(new PivotSummaryItem(columnInfo, field.SummaryType));
				if(field.SummaryType == PivotSummaryType.Custom) 
					AddCustomSummaryInfo(field, columnInfo);
			}			
		}
		List<PivotSortByCondition> GetSortbyConditions(PivotGridFieldBase field) {
			PivotGridFieldSortConditionCollection list = field.SortBySummaryInfo.Conditions;
			if(list == null || list.Count == 0) return null;
			List<PivotSortByCondition> res = new List<PivotSortByCondition>(list.Count);
			for(int i = 0; i < list.Count; i++) {
				PivotGridFieldBase conditionField = list[i].Field;
				if(conditionField == null || !conditionField.IsColumnOrRow || conditionField.Area == field.Area || conditionField.AreaIndex < 0) 
					continue;
				DataColumnInfo columnInfo = DataController.Columns[GetFieldName(conditionField)];
				if(columnInfo == null)
					continue;
				res.Add(new PivotSortByCondition(columnInfo, list[i].Value, conditionField.AreaIndex));
			}
			res.Sort(GetSortbyConditionsComp);
			return res;
		}
		int GetSortbyConditionsComp(PivotSortByCondition x, PivotSortByCondition y) {
			return Comparer<int>.Default.Compare(x.Level, y.Level);
		}
		void GetSortedByInfo(PivotGridFieldSortBySummaryInfo sortBySummaryInfo, out DataColumnInfo sortedByColumn, out PivotSummaryType sortBySummaryType) {
			sortedByColumn = null;
			sortBySummaryType = sortBySummaryInfo.SummaryType;
			if(sortBySummaryInfo.Field != null) {
				sortedByColumn = DataController.Columns[GetFieldName(sortBySummaryInfo.Field)];
				sortBySummaryType = sortBySummaryInfo.Field.SummaryType;
			}
			if(sortedByColumn == null)
				sortedByColumn = DataController.Columns[sortBySummaryInfo.FieldName];
		}
		void AddCustomSummaryInfo(PivotGridFieldBase field, DataColumnInfo columnInfo) {
			if(!CustomSummaryColumnInfos.ContainsKey(columnInfo))
				CustomSummaryColumnInfos[columnInfo] = new List<PivotGridFieldBase>();
			if(!CustomSummaryColumnInfos[columnInfo].Contains(field))
				CustomSummaryColumnInfos[columnInfo].Add(field);			
		}
		Hashtable FieldsByColumns { get { return fieldsByColumns; } }
		Dictionary<DataColumnInfo, List<PivotGridFieldBase>> CustomSummaryColumnInfos { get { return customSummaryColumnInfos; } }
		void ClearFieldsAndSummaries() {
			this.fieldsByColumns.Clear();
			CustomSummaryColumnInfos.Clear();
			DataController.ClearAreaColumnsAndSummaries();
		}
		protected UnboundColumnInfo NullableColumn { get { return nullableColumn; } }
		protected UnboundColumnInfoCollection UnboundColumns {
			get {
				unboundColumns.Clear();
				unboundColumns.Add(NullableColumn);
				if(DataController.SupportsUnboundColumns) {
					for(int i = 0; i < Fields.Count; i++) {
						if(Fields[i].IsUnbound) {
							unboundColumns.Add(new UnboundColumnInfo(GetFieldName(Fields[i]), GetUnboundFieldType(Fields[i]), false));
						}
					}
				}
				return unboundColumns;
			}
		}
		UnboundColumnType GetUnboundFieldType(PivotGridFieldBase field) {
			if(field.UnboundType != UnboundColumnType.Bound && field.GroupInterval == PivotGroupInterval.Default)
				return field.UnboundType;
			if(field.GroupInterval == PivotGroupInterval.Alphabetical)
				return UnboundColumnType.String;
			if(field.GroupInterval == PivotGroupInterval.Date)
				return UnboundColumnType.DateTime;
			if(field.GroupInterval == PivotGroupInterval.Custom)
				return UnboundColumnType.Object;
			return UnboundColumnType.Integer;
		}
		int unboundId = 0;
		protected string GetFieldName(PivotGridFieldBase field) {
			if(field.FieldName != string.Empty && field.GroupInterval == PivotGroupInterval.Default) return field.FieldName;
			if(field.IsUnbound) {
				if(string.IsNullOrEmpty(field.UnboundFieldName)) 
					field.UnboundFieldName = !string.IsNullOrEmpty(field.Name) ? field.Name : "UnboundColumn" + unboundId.ToString();
				unboundId++;
				return field.UnboundFieldName;
			}
			return string.Empty;
		}
		protected object GetGroupInvervalValue(PivotGridFieldBase field, object value) {
			if(value == null) return value;
			if(field.GroupInterval == PivotGroupInterval.Default) return value;
			if(field.GroupInterval == PivotGroupInterval.Custom) 
				return GridData.GetCustomGroupInterval(field, value);
			if(field.GroupInterval == PivotGroupInterval.Alphabetical) {
				string text = value.ToString();
				return text.Length > 1 ? text.Substring(0, 1) : text;
			}
			if(field.IsGroupIntervalNumeric) {
				int intValue = 0;
				if(field.GroupInterval == PivotGroupInterval.Numeric) {
					intValue = (int)Convert.ToDecimal(value);
				} else {
					DateTime dateTime = (DateTime)value;
					if(field.GroupInterval == PivotGroupInterval.YearAge)
						intValue = DateHelper.GetFullYears(dateTime, RefreshDate);
					if(field.GroupInterval == PivotGroupInterval.MonthAge)
						intValue = DateHelper.GetFullMonths(dateTime, RefreshDate);
					if(field.GroupInterval == PivotGroupInterval.WeekAge)
						intValue = DateHelper.GetFullWeeks(dateTime, RefreshDate);
					if(field.GroupInterval == PivotGroupInterval.DayAge)
						intValue = DateHelper.GetFullDays(dateTime, RefreshDate);
				}
				if(intValue < 0) {
					if(Math.Abs(intValue) % field.GroupIntervalNumericRange != 0) {
						intValue = -((Math.Abs(intValue) / field.GroupIntervalNumericRange + 1) * field.GroupIntervalNumericRange);
					}
				}
				return (int)(intValue / field.GroupIntervalNumericRange);
			}
			try {
				DateTime dateTime = (DateTime)value;
				if(field.GroupInterval == PivotGroupInterval.Date)
					return dateTime.Date;
				if(field.GroupInterval == PivotGroupInterval.DateYear)
					return dateTime.Year;
				if(field.GroupInterval == PivotGroupInterval.DateMonth)
					return dateTime.Month;
				if(field.GroupInterval == PivotGroupInterval.DateQuarter)
					return (dateTime.Month - 1) / 3 + 1;
				if(field.GroupInterval == PivotGroupInterval.DateDay)
					return dateTime.Day;
				if(field.GroupInterval == PivotGroupInterval.DateDayOfWeek)
					return dateTime.DayOfWeek;
				if(field.GroupInterval == PivotGroupInterval.DateDayOfYear)
					return dateTime.DayOfYear;
				if(field.GroupInterval == PivotGroupInterval.DateWeekOfMonth)
					return DateHelper.GetWeekOfMonth(dateTime);
				if(field.GroupInterval == PivotGroupInterval.DateWeekOfYear)
					return DateHelper.GetWeekOfYear(dateTime);
				if(field.GroupInterval == PivotGroupInterval.Hour)
					return dateTime.Hour;
			} catch {
			}
			return null;
		}
		#region IDataControllerSort implementation
		bool IDataControllerSort.RequireSortCell(DataColumnInfo column) {
			PivotGridFieldBase field = GetFieldByPivotColumnInfo(column);
			if(field == null) return false;
			return field.SortMode == PivotSortMode.DisplayText || field.SortMode == PivotSortMode.Custom ||
				((field.SortMode == PivotSortMode.Value || field.SortMode == PivotSortMode.Default)
					&& (field.GroupInterval == PivotGroupInterval.DateDayOfWeek));
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) { return false; }
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value) { return string.Empty; }
		int IDataControllerSort.SortRow(int listSourceRow1, int listSourceRow2) { return 3; }
		int IDataControllerSort.SortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			return DataControllerSortCell(listSourceRow1, listSourceRow2, value1, value2, sortColumn, sortOrder);
		}
		int IDataControllerSort.SortGroupCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) {
			return DataControllerSortCell(listSourceRow1, listSourceRow2, value1, value2, sortColumn, ColumnSortOrder.Ascending);
		}
		void IDataControllerSort.BeforeSorting() { }
		void IDataControllerSort.AfterSorting() { }
		void IDataControllerSort.BeforeGrouping() { }
		void IDataControllerSort.AfterGrouping() { }
		int DataControllerSortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			PivotGridFieldBase field = GetFieldByPivotColumnInfo(sortColumn);
			if(field == null) return 3;
			switch(field.SortMode) {
				case PivotSortMode.DisplayText:
					return DataControllerSortCellByDisplayText(value1, value2, sortOrder, field);
				case PivotSortMode.Custom:
					return DataControllerSortCellCustom(listSourceRow1, listSourceRow2, value1, value2, sortOrder, field);
				case PivotSortMode.Default:
				case PivotSortMode.Value:
					if(field.GroupInterval == PivotGroupInterval.DateDayOfWeek) 
						return DateHelper.CompareDayOfWeek((DayOfWeek)value1, (DayOfWeek)value2);
					return 3;
				default:
					return 3;
			}
		}
		int DataControllerSortCellCustom(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			PivotSortOrder pivotSortOrder = sortOrder == ColumnSortOrder.Ascending ? PivotSortOrder.Ascending : PivotSortOrder.Descending;
			return GridData.GetCustomSortRowsAccess(listSourceRow1, listSourceRow2, value1, value2, field, pivotSortOrder);
		}
		int DataControllerSortCellByDisplayText(object value1, object value2, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			string st1 = GridData.GetPivotFieldValueText(field, value1);
			string st2 = GridData.GetPivotFieldValueText(field, value2);
			int result = DataController.ValueComparer.Compare(st1, st2);
			if(sortOrder != ColumnSortOrder.Ascending) {
				result *= -1;
			}
			return result;
		}
		#endregion
		#region IDataControllerData implementation
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column) {
			if(column == null) return null;
			PivotGridFieldBase field = column.Tag as PivotGridFieldBase;
			if(field == null) return null;
			if(field.UnboundType != UnboundColumnType.Bound) {
				object value = GridData.GetUnboundValueAccess(field, listSourceRow1);
				if(field.GroupInterval != PivotGroupInterval.Default) {
					value = GetGroupInvervalValue(field, value);
				}
				return value;
			}
			if(field.GroupIntervalColumnHandle > -1) {
				return GetGroupInvervalValue(field, DataController.GetRowValue(listSourceRow1, field.GroupIntervalColumnHandle));
			}
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return UnboundColumns;
		}
		#endregion
		#region IPivotClient implementation
		bool IPivotClient.HasFilters {
			get {
				for(int i = 0; i < Fields.Count; i++) {
					if(Fields[i].ColumnHandle > -1 && Fields[i].FilterValues.HasFilter)
						return true;
				}
				return false;
			}
		}
		bool IPivotClient.IsDesignMode { get { return IsDesignMode; } }
		bool IPivotClient.IsUpdateLocked { get { return GridData.IsLockUpdate; } }
		void IPivotClient.SetFilteredValues(PivotFilteredValues[] filteredValues) {
			for(int i = 0; i < Fields.Count; i++) {
				if(Fields[i].ColumnHandle > -1 && Fields[i].FilterValues.HasFilter) {
					filteredValues[Fields[i].ColumnHandle] = new PivotFilteredValues(Fields[i].FilterValues.GetHashtable(), Fields[i].FilterValues.FilterType, Fields[i].FilterValues.ShowBlanks);
				}
			}
		}
		bool IPivotClient.NeedCalcCustomSummary(DataColumnInfo columnInfo) {
			return CustomSummaryColumnInfos.ContainsKey(columnInfo);
		}
		void IPivotClient.CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo) {
			object columnInfo = CustomSummaryColumnInfos[customSummaryInfo.DataColumn];
			List<PivotGridFieldBase> fieldList = columnInfo as List<PivotGridFieldBase>;
			CalcCustomSummaryForMultipleFields(customSummaryInfo, fieldList);
		}
		void CalcCustomSummaryForMultipleFields(PivotCustomSummaryInfo customSummaryInfo, List<PivotGridFieldBase> fieldList) {
			PivotGridCustomValues customValues = new PivotGridCustomValues();
			foreach(PivotGridFieldBase field in fieldList) {
				customSummaryInfo.SummaryValue.CustomValue = null;
				GridData.OnCalcCustomSummaryAccess(field, customSummaryInfo);
				customValues[field] = customSummaryInfo.SummaryValue.CustomValue;
			}
			customSummaryInfo.SummaryValue.CustomValue = customValues;
		}
		bool IPivotClient.HasColumnVariation(DataColumnInfo columnInfo) {
			if(OptionsDataField.Area == PivotDataArea.RowArea) return false;
			return HasVariation(columnInfo);
		}
		bool IPivotClient.HasRowVariation(DataColumnInfo columnInfo) {
			if(OptionsDataField.Area != PivotDataArea.RowArea) return false;
			return HasVariation(columnInfo);
		}
		bool HasVariation(DataColumnInfo columnInfo) {
			for(int i = 0; i < DataFieldCount; i++) {
				PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
				if(field.ColumnHandle != columnInfo.Index) continue;
				if(field.SummaryDisplayType == PivotSummaryDisplayType.AbsoluteVariation ||
					field.SummaryDisplayType == PivotSummaryDisplayType.PercentVariation)
					return true;
			}
			return false;
		}
		void IPivotClient.PopulateColumns() {
			if(!GridData.CanDoRefresh) {
				this.needPopulateColumns = true;
			}
			GridData.BindColumns();
			GridData.DoRefresh();
		}
		void IPivotClient.UpdateLayout() {
			GridData.LayoutChanged();
		}
		string IPivotClient.GetFieldCaption(DataColumnInfo columnInfo) {
			PivotGridFieldBase field = fieldsByColumns[columnInfo] as PivotGridFieldBase;
			return field != null ? field.Caption : string.Empty;
		}
		CriteriaOperator IPivotClient.PrefilterCritera {
			get {
				CriteriaOperator res = GridData.Prefilter.Enabled ? GridData.Prefilter.Criteria : null;
				return new PrefilterPatcher(GridData.Fields).Patch(res);
			}
		}
		#endregion
		#region IDisposable implementation
		public virtual void Dispose() {
			this.dataController.Dispose();
			this.dataController = null;
		}
		#endregion
		#region IPivotGridDataSource implementation
		public virtual IList ListSource {
			get { return DataController.ListSource; }
			set { DataController.ListSource = value; }
		}
		bool IPivotGridDataSource.CaseSensitive {
			get { return DataController.CaseSensitive; }
			set { DataController.CaseSensitive = value; }
		}
		bool IPivotGridDataSource.SupportsUnboundColumns { get { return DataController.SupportsUnboundColumns; } }
		event EventHandler IPivotGridDataSource.ListSourceChanged {
			add { DataController.ListSourceChanged += value; }
			remove { DataController.ListSourceChanged -= value; }
		}
		void IPivotGridDataSource.ReloadData() {
			this.needPopulateColumns = true;
			GridData.DoRefresh();
		}
		void IPivotGridDataSource.RetrieveFields() { RetrieveFieldsCore(); }	
		void IPivotGridDataSource.DoRefresh(PivotGridFieldReadOnlyCollection sortedFields) {
			DataController.BeginUpdate();
			RefreshDate = DateTime.Today;
			if(DataController.Columns.Count == 0 || this.needPopulateColumns) {
				this.needPopulateColumns = false;
				DataController.PopulateColumns();
			}
			try {
				BindColumnsCore(sortedFields);
			} finally {
				DataController.CancelUpdate();
			}
			if(!GridData.LockRefresh)
				DataController.DoRefresh();
		}
		protected virtual void BindColumnsCore(PivotGridFieldReadOnlyCollection sortedFields) {
			DataController.BeginUpdate();
			if(DataController.Columns[NullableColumn.Name] != null) {
				DataController.Columns[NullableColumn.Name].Visible = false;
			}
			try {
				ClearFieldsAndSummaries();
				for(int i = 0; i < sortedFields.Count; i++) {
					DataColumnInfo columnInfo = DataController.Columns[GetFieldName(sortedFields[i])];
					sortedFields[i].SetColumnHandle(columnInfo != null ? columnInfo.Index : -1);
					if(sortedFields[i].GroupInterval != PivotGroupInterval.Default)
						sortedFields[i].GroupIntervalColumnHandle = DataController.Columns[sortedFields[i].FieldName] != null ? DataController.Columns[sortedFields[i].FieldName].Index : -1;
					if(columnInfo == null) {
						columnInfo = DataController.Columns[NullableColumn.Name];
					}
					if(columnInfo == null) continue;
					columnInfo.Tag = sortedFields[i];
					AddFieldIntoDataController(sortedFields[i], columnInfo);
				}
			} finally {
				DataController.CancelUpdate();
			}
		}
		void IPivotGridDataSource.BindColumns(PivotGridFieldReadOnlyCollection sortedFields) {
			BindColumnsCore(sortedFields);
		}
		Type IPivotGridDataSource.GetFieldType(PivotGridFieldBase field, bool raw) {
			Type columnType = GetFieldTypeCore(field);
			if(raw || columnType == null || field.Area != PivotArea.DataArea) return columnType;
			switch(field.SummaryType) {
				case PivotSummaryType.Average:
				case PivotSummaryType.Sum:
					return typeof(decimal);
				case PivotSummaryType.Count:
					return typeof(int);
				case PivotSummaryType.Max:
				case PivotSummaryType.Min:
					return columnType;
				case PivotSummaryType.StdDev:
				case PivotSummaryType.StdDevp:
				case PivotSummaryType.Var:
				case PivotSummaryType.Varp:
					return typeof(double);
				case PivotSummaryType.Custom:
					return typeof(object);
				default:
					throw new Exception("Unknown summary type.");
			}
		}
		Type GetFieldTypeCore(PivotGridFieldBase field) {
			DataColumnInfo columnInfo = GetColumnInfo(field);
			return columnInfo != null ? columnInfo.Type : null;
		}
		int IPivotGridDataSource.CompareValues(object val1, object val2) {
			return DataController.ValueComparer.Compare(val1, val2);
		}
		bool IPivotGridDataSource.ChangeFieldSortOrder(PivotGridFieldBase field) {
			if(field.ColumnHandle > -1 && field.ColumnHandle < DataController.Columns.Count) {
				DataController.ChangeFieldSortOrder(field.Area == PivotArea.ColumnArea, field.AreaIndex);
				return true;
			}
			return false;
		}
		object IPivotGridDataSource.GetListSourceRowValue(int listSourceRow, string fieldName) {
			return DataController.GetRowValue(listSourceRow, fieldName);
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			return DataController.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			return DataController.CreateDrillDownDataSource(visibleListSourceRows, groupRow);
		}
		object IPivotGridDataSource.GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			return DataController.GetCellValue(columnIndex, rowIndex, dataIndex, summaryType);
		}
		PivotSummaryValue IPivotGridDataSource.GetCellSummaryValue(int columnIndex, int rowIndex, int dataIndex) {
			return DataController.GetCellSummaryValue(columnIndex, rowIndex, dataIndex);
		}
		int IPivotGridDataSource.GetVisibleIndexByValues(bool isColumn, object[] values) {
			return DataController.GetVisibleIndexByValues(isColumn, values);
		}
		int IPivotGridDataSource.GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return DataController.GetNextOrPrevVisibleIndex(isColumn, visibleIndex, isNext);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return DataController.IsObjectCollapsed(isColumn, visibleIndex);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, object[] values) {
			return DataController.IsObjectCollapsed(isColumn, DataController.GetVisibleIndexByValues(isColumn, values));
		}		
		bool IPivotGridDataSource.ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			DataController.ChangeExpanded(isColumn, visibleIndex, expanded, false);
			return true;
		}
		void IPivotGridDataSource.ChangeExpandedAll(bool isColumn, bool expanded) {
			DataController.ChangeExpanded(isColumn, expanded);
		}
		void IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(!field.IsColumnOrRow) throw new Exception("Cannot expand data and filter fields");
			DataController.ChangeFieldExpanded(field.Area == PivotArea.ColumnArea, field.AreaIndex, expanded);
		}
		void IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(!field.IsColumnOrRow) throw new Exception("Cannot expand data and filter fields");
			DataController.ChangeFieldExpanded(field.Area == PivotArea.ColumnArea, field.AreaIndex, expanded, value);
		}
		object IPivotGridDataSource.GetFieldValue(bool isColumn, int columnRowIndex, int areaIndex) {
			return DataController.GetFieldValue(isColumn, columnRowIndex, areaIndex);
		}
		bool IPivotGridDataSource.GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex) {
			return DataController.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		object[] IPivotGridDataSource.GetUniqueFieldValues(PivotGridFieldBase field) { return GetUniqueFieldValuesCore(field); }		
		bool IPivotGridDataSource.HasNullValues(PivotGridFieldBase field) { return DataController.HasNullValues(field.ColumnHandle); }
		int IPivotGridDataSource.GetCellCount(bool isColumn) { return DataController.GetCellCount(isColumn); }		
		int IPivotGridDataSource.GetObjectLevel(bool isColumn, int columnRowIndex) { return DataController.GetObjectLevel(isColumn, columnRowIndex); }
		void IPivotGridDataSource.SaveCollapsedStateToStream(Stream stream) { 
			DataController.SaveCollapsedStateToStream(stream); 
		}
		void IPivotGridDataSource.WebSaveCollapsedStateToStream(Stream stream) {
			DataController.WebSaveCollapsedStateToStream(stream);
		}
		void IPivotGridDataSource.SaveDataToStream(Stream stream, bool compressed) { 
			DataController.SaveDataToStream(stream, compressed); 
		}
		void IPivotGridDataSource.LoadCollapsedStateFromStream(Stream stream) { 
			DataController.LoadCollapsedStateFromStream(stream); 
		}
		void IPivotGridDataSource.WebLoadCollapsedStateFromStream(Stream stream) {
			DataController.WebLoadCollapsedStateFromStream(stream);
		}
		bool IPivotGridDataSource.IsAreaAllowed(PivotGridFieldBase field, PivotArea area) { return IsAreaAllowedCore(field, area); }
		string[] IPivotGridDataSource.GetFieldList() { return GetFieldListCore(); }
		string IPivotGridDataSource.GetFieldCaption(string fieldName) { return GetFieldCaptionCore(fieldName); }
		int IPivotGridDataSource.GetFieldHierarchyLevel(string fieldName) { return GetFieldHierarchyLevelCore(fieldName); }
		PivotKPIType IPivotGridDataSource.GetKPIType(PivotGridFieldBase field) { return PivotKPIType.None; }
		PivotKPIGraphic IPivotGridDataSource.GetKPIGraphic(PivotGridFieldBase field) { return PivotKPIGraphic.None; }
		#endregion
		protected virtual object[] GetUniqueFieldValuesCore(PivotGridFieldBase field) {
			return DataController.GetUniqueFieldValues(field.ColumnHandle);
		}
		protected virtual int GetFieldHierarchyLevelCore(string fieldName) {
			return 0;
		}
		protected virtual bool IsAreaAllowedCore(PivotGridFieldBase field, PivotArea area) {
			return true;
		}
		protected virtual void RetrieveFieldsCore() {
			DataController.PopulateColumns();
			for(int i = 0; i < DataController.Columns.Count; i++) {
				if(DataController.Columns[i].Name != NullableColumn.Name) {
					PivotGridFieldBase field = Fields.Add(DataController.Columns[i].Name, PivotArea.FilterArea);
					field.Caption = DataController.Columns[i].Caption;
				}
			}
		}
		protected virtual string[] GetFieldListCore() {
			if(DataController.Columns.Count == 0) return null;
			List<string> list = new List<string>();
			for(int n = 0; n < DataController.Columns.Count; n++) {
				if(!DataController.Columns[n].Unbound) list.Add(DataController.Columns[n].Name);
			}
			return list.ToArray();
		}
		protected virtual string GetFieldCaptionCore(string fieldName) {
			if(DataController.Columns.Count == 0) return null;
			DataColumnInfo column = DataController.Columns[fieldName];
			return column != null ? column.Caption : null;
		}
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties {
			get { return !IsDesignMode; }
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			for(int i = 0; i < this.missingComplexColumns.Count; i++) {
				res.Add(this.missingComplexColumns[i]);
			}
			return res;
		}
		bool IDataControllerData2.HasUserFilter {
			get {
				return false; 
			}
		}
		int IDataControllerData2.IsRowFit(int listSourceRow) {
			return 1;	
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(collection == null || GridData.Fields.Count == 0)
				return collection;
			this.missingComplexColumns.Clear();
			for(int i = 0; i < GridData.Fields.Count; i++) {
				PivotGridFieldBase field = GridData.Fields[i];
				if(field.UnboundType == UnboundColumnType.Bound && collection.Find(field.FieldName, false) == null) {
					collection.Find(field.FieldName, true);	
				}
				if(field.IsComplex && collection.Find(field.FieldName, false) == null) {
					this.missingComplexColumns.Add(field.FieldName);
				}
			}
			return collection;
		}
		List<string> missingComplexColumns = new List<string>();
		#endregion
	}
}
