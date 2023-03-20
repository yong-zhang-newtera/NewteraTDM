#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxPivotGrid                                 }
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
using System.Data;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotFieldEventArgs : EventArgs {
		PivotGridField field;
		public PivotFieldEventArgs(PivotGridField field) {
			this.field = field;
		}	   
		public PivotGridField Field { get { return field; } }
	}
	public class PivotCustomGroupIntervalEventArgs : PivotFieldEventArgs {
		object value, groupValue;
		public PivotCustomGroupIntervalEventArgs(PivotGridField field, object value)
			: base(field) {
			this.groupValue = this.value = value;
		}
		public object Value { get { return value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
	}
	public class PivotFieldValueEventArgs : PivotFieldEventArgs {
		PivotFieldValueItem fieldValueItem;
		public PivotFieldValueEventArgs(PivotFieldValueItem fieldValueItem)
			: base(fieldValueItem.Field as PivotGridField) {
			this.fieldValueItem = fieldValueItem;
		}
		protected PivotFieldValueEventArgs(PivotGridField field) : base(field) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldValueItem FieldValueItem { get { return fieldValueItem; } }
		public virtual object Value { get { return FieldValueItem != null ? FieldValueItem.Value : null; } }
		public bool IsOthersValue { get { return FieldValueItem != null ? FieldValueItem.IsOthersRow : false; } }
		public PivotGridValueType ValueType { get { return FieldValueItem != null ? FieldValueItem.ValueType : PivotGridValueType.Value; } }
		public bool IsCollapsed { get { return FieldValueItem != null ? FieldValueItem.IsCollapsed : false; } }
		public bool IsColumn { get { return FieldValueItem != null ? FieldValueItem.IsColumn : false; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return FieldValueItem != null ? FieldValueItem.CreateDrillDownDataSource() : null;
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return FieldValueItem != null ? FieldValueItem.CreateOLAPDrillDownDataSource(maxRowCount, customColumns) : null;
		}
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldValueEventArgs {
		string displayText;
		object value;
		public PivotFieldDisplayTextEventArgs(PivotFieldValueItem fieldValueItem) : base(fieldValueItem) {
			this.value = fieldValueItem.Value;
			this.displayText = fieldValueItem.Text;
		}
		public PivotFieldDisplayTextEventArgs(PivotGridField field, object value)
			: base(field) {
			this.value = value;
			this.displayText = Field.GetValueText(value);
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return FieldValueItem == null; } }
	}
	public class PivotFieldStateChangedEventArgs : PivotFieldEventArgs {
		object[] values;
		bool isCollapsed;
		public PivotFieldStateChangedEventArgs(PivotGridField field, object[] values, bool isCollapsed)
			: base(field) {
			this.values = values;
			this.isCollapsed = isCollapsed;
		}
		public object[] Values { get { return values; } }
		public object FieldValue { get { return values.Length > 0 ? values[values.Length - 1] : null; } }
		public bool IsCollapsed { get { return isCollapsed; } }
	}
	public class PivotFieldStateChangedCancelEventArgs : PivotFieldStateChangedEventArgs {
		bool cancel = false;
		public PivotFieldStateChangedCancelEventArgs(PivotGridField field, object[] values, bool isCollapsed) : base(field, values, isCollapsed) { }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PivotCellBaseEventArgs : EventArgs {
		PivotGridCellItem cellItem;
		public PivotCellBaseEventArgs(PivotGridCellItem cellItem) {
			this.cellItem = cellItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridCellItem CellItem { get { return cellItem; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridData Data { get { return cellItem.Data as PivotGridData; } }
		public PivotGridField DataField { get { return CellItem.DataField as PivotGridField; } }
		public int ColumnIndex { get { return CellItem.ColumnIndex; } }
		public int RowIndex { get { return CellItem.RowIndex; } }
		public int ColumnFieldIndex { get { return CellItem.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return CellItem.RowFieldIndex; } }
		public PivotGridField ColumnField { get { return CellItem.ColumnField as PivotGridField; } }
		public PivotGridField RowField { get { return CellItem.RowField as PivotGridField; } }
		public object Value { get { return CellItem.Value; } }
		public PivotSummaryValue SummaryValue { get { return Data.GetCellSummaryValue(ColumnFieldIndex, RowFieldIndex, DataField); } }
		public PivotGridValueType ColumnValueType { get { return CellItem.ColumnValueType; } }
		public PivotGridValueType RowValueType { get { return CellItem.RowValueType; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, CellItem.DataIndex);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource(ColumnFieldIndex, RowFieldIndex);
		}
		public object GetFieldValue(PivotGridField field) {
			return CellItem.GetFieldValue(field);
		}
		public bool IsOthersFieldValue(PivotGridField field) {
			return CellItem.IsOthersFieldValue(field);
		}
		public bool IsFieldValueExpanded(PivotGridField field) {
			return CellItem.IsFieldValueExpanded(field);
		}
		public bool IsFieldValueRetrievable(PivotGridField field) {
			return CellItem.IsFieldValueRetrievable(field);
		}
		public PivotGridField[] GetColumnFields() {
			return GetFields(PivotArea.ColumnArea, Data.GetColumnLevel(ColumnFieldIndex) + 1);
		}
		public PivotGridField[] GetRowFields() {
			return GetFields(PivotArea.RowArea, Data.GetRowLevel(RowFieldIndex) + 1);
		}
		PivotGridField[] GetFields(PivotArea area, int fieldCount) {
			if(fieldCount <= 0 || fieldCount > Data.GetFieldCountByArea(area)) return new PivotGridField[0];
			PivotGridField[] fields = new PivotGridField[fieldCount];
			for(int i = 0; i < fields.Length; i++)
				fields[i] = Data.GetFieldByArea(area, i) as PivotGridField;
			return fields;
		}
		public object GetCellValue(PivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetPrevRowCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextRowCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetPrevColumnCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextColumnCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetColumnGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(-1, RowFieldIndex, dataField);
		}
		public object GetColumnGrandTotal(object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(null, rowValues, dataField);
		}
		public object GetRowGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, -1, dataField);
		}
		public object GetRowGrandTotal(object[] columnValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, null, dataField);
		}
		public object GetGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(-1, -1, dataField);
		}
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		public PivotCellDisplayTextEventArgs(PivotGridCellItem cellItem) : base(cellItem) {
			this.displayText = cellItem.Text;
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	public class PivotCustomCellStyleEventArgs : PivotCellBaseEventArgs {
		PivotCellStyle cellStyle;
		public PivotCustomCellStyleEventArgs(PivotGridCellItem cellItem, PivotCellStyle cellStyle)
			: base(cellItem) {
			this.cellStyle = cellStyle;
		}
		public PivotCellStyle CellStyle { get { return cellStyle; } }
	}
	public enum MenuItemEnum { HeaderRefresh, HeaderHide, HeaderShowList, 
		HeaderShowPrefilter, FieldValueExpand, FieldValueExpandAll, FieldValueSortBySummaryFields }
	public class PivotAddPopupMenuItemEventArgs : EventArgs {
		readonly MenuItemEnum menuItem;
		bool add;
		public MenuItemEnum MenuItem { get { return menuItem; } }
		public bool Add { get { return add; } set { add = value; } }
		public PivotAddPopupMenuItemEventArgs(MenuItemEnum menuItem) {
			this.menuItem = menuItem;
			this.add = true;
		}
	}
	public enum PivotGridPopupMenuType { HeaderMenu, FieldValueMenu };
	public class PivotPopupMenuCreatedEventArgs : EventArgs {
		readonly ASPxPivotGridPopupMenu menu;
		readonly PivotGridPopupMenuType menuType;
		public ASPxPivotGridPopupMenu Menu { get { return menu; } }
		public PivotGridPopupMenuType MenuType { get { return menuType; } }
		public PivotPopupMenuCreatedEventArgs(PivotGridPopupMenuType menuType, ASPxPivotGridPopupMenu menu) {
			this.menuType = menuType;
			this.menu = menu;
		}
	}
	public class PivotDataAreaPopupCreatedEventArgs : EventArgs {
		readonly PivotGridDataAreaPopup popup;
		public PivotGridDataAreaPopup Popup { get { return popup; } }
		public PivotDataAreaPopupCreatedEventArgs(PivotGridDataAreaPopup popup) {
			this.popup = popup;
		}
	}
	public class CustomFieldDataEventArgs : PivotFieldEventArgs {
		object _value = null;
		int listSourceRow;
		PivotGridData data;
		public CustomFieldDataEventArgs(PivotGridData data, PivotGridField field, int listSourceRow, object _value)
			: base(field) {
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.data = data;
		}
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public object Value { get { return _value; } set { _value = value; } }
		public object GetListSourceColumnValue(string columnName) {
			return GetListSourceColumnValue(ListSourceRowIndex, columnName);
		}
		public object GetListSourceColumnValue(int listSourceRowIndex, string columnName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, columnName);
		}
		protected PivotGridData Data { get { return data; } }
	}
	public class PivotGridCustomFieldSortEventArgs : EventArgs {
		PivotGridData data;
		PivotGridField field;
		PivotSortOrder sortOrder;
		bool handled = false;
		internal object value1, value2;
		int result = 0;
		int listSourceRow1, listSourceRow2;
		public PivotGridCustomFieldSortEventArgs(PivotGridData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			SetArgs(-1, -1, null, null, PivotSortOrder.Ascending);
		}
		public PivotSortOrder SortOrder { get { return sortOrder; } }
		protected internal PivotGridData Data { get { return data; } }
		public PivotGridField Field { get { return field; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		public int Result {
			get { return result; }
			set { result = value; }
		}
		internal int GetSortResult() {
			if(!Handled) return 3;
			return Result;
		}
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.value1 = value1;
			this.value2 = value2;
			this.result = 0;
			this.handled = false;
			this.listSourceRow1 = listSourceRow1;
			this.listSourceRow2 = listSourceRow2;
		}
		public object GetListSourceColumnValue(int listSourceRowIndex, string columnName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, columnName);
		}
	}
	public class PivotGridCustomSummaryEventArgs : EventArgs {
		PivotGridField field;
		PivotCustomSummaryInfo customSummaryInfo;
		PivotGridData data;
		PivotDrillDownDataSource dataSource;
		public PivotGridCustomSummaryEventArgs(PivotGridData data, PivotGridField field, PivotCustomSummaryInfo customSummaryInfo) {
			this.data = data;
			this.field = field;
			this.customSummaryInfo = customSummaryInfo;
			this.dataSource = null;
		}
		protected PivotCustomSummaryInfo CustomSummaryInfo { get { return customSummaryInfo; } }
		public object CustomValue { get { return SummaryValue.CustomValue; } set { SummaryValue.CustomValue = value; } }
		public PivotSummaryValue SummaryValue { get { return CustomSummaryInfo.SummaryValue; } }
		public PivotGridField DataField { get { return field; } }
		public string FieldName { get { return CustomSummaryInfo.DataColumn.Name; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return DataSource;
		}
		public PivotGridFieldBase ColumnField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.ColColumn); }
		}
		public PivotGridFieldBase RowField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.RowColumn); }
		}
		public object ColumnFieldValue { get { return GetFieldValue(CustomSummaryInfo.ColColumn); } }
		public object RowFieldValue { get { return GetFieldValue(CustomSummaryInfo.RowColumn); } }
		[Obsolete(), EditorBrowsable(EditorBrowsableState.Never)]
		public int ChildCount { get { return 0; } }
		[Obsolete(), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridCustomSummaryEventArgs GetChild(int index) { return null; }
		protected PivotDrillDownDataSource DataSource {
			get {
				if(this.dataSource == null)
					this.dataSource = data.GetDrillDownDataSource(CustomSummaryInfo.GroupRow, CustomSummaryInfo.VisibleListSourceRows);
				return this.dataSource;
			}
		}
		protected object GetFieldValue(PivotColumnInfo columnInfo) {
			if(columnInfo == null) return null;
			return DataSource[0][columnInfo.ColumnInfo.Name];
		}
	}
	public class PivotGridCustomCallbackEventArgs : EventArgs {
		string parameters;
		public string Parameters { get { return parameters; } }
		public PivotGridCustomCallbackEventArgs(string parameters) {
			this.parameters = parameters;
		}
	}
	public delegate void CustomFieldDataEventHandler(object sender, CustomFieldDataEventArgs e);
	public delegate void PivotGridCustomFieldSortEventHandler(object sender, PivotGridCustomFieldSortEventArgs e);
	public delegate void PivotGridCustomGroupIntervalEventHandler(object sender, PivotCustomGroupIntervalEventArgs e);
	public delegate void PivotGridCustomSummaryEventHandler(object sender, PivotGridCustomSummaryEventArgs e);
	public delegate void PivotFieldEventHandler(object sender, PivotFieldEventArgs e);
	public delegate void PivotFieldStateChangedEventHandler(object sender, PivotFieldStateChangedEventArgs e);
	public delegate void PivotFieldStateChangedCancelEventHandler(object sender, PivotFieldStateChangedCancelEventArgs e);
	public delegate void PivotFieldDisplayTextEventHandler(object sender, PivotFieldDisplayTextEventArgs e);
	public delegate void PivotCellDisplayTextEventHandler(object sender, PivotCellDisplayTextEventArgs e);
	public delegate void PivotCustomCellStyleEventHandler(object sender, PivotCustomCellStyleEventArgs e);
	public delegate void PivotAddPopupMenuItemEventHandler(object sender, PivotAddPopupMenuItemEventArgs e);
	public delegate void PivotPopupMenuCreatedEventHandler(object sender, PivotPopupMenuCreatedEventArgs e);
	public delegate void PivotFieldValueEventHandler(object sender, PivotFieldValueEventArgs e);
	public delegate void PivotCustomCallbackEventHandler(object sender, PivotGridCustomCallbackEventArgs e);
}
