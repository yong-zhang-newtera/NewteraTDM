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
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxGridView.Helper;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView {
	public class ASPxGridViewRowEventArgs : EventArgs {
		int visibleIndex;
		object keyValue;
		public ASPxGridViewRowEventArgs(int visibleIndex, object keyValue) {
			this.visibleIndex = visibleIndex;
			this.keyValue = keyValue;
		}
		public object KeyValue { get { return keyValue; } }
		public int VisibleIndex { get { return visibleIndex; } }
	}
	public enum GridViewRowType { Data, Group, Preview, Detail, InlineEdit, EditForm, EditingErrorRow, Footer, GroupFooter, Filter, EmptyDataRow, PagerEmptyRow, Title, Header }
	public class ASPxGridViewTableRowEventArgs : ASPxGridViewRowEventArgs {
		GridViewTableRow row;
		public ASPxGridViewTableRowEventArgs(GridViewTableRow row, object keyValue)
			: base(row.VisibleIndex, keyValue) {
			this.row = row;
		}
		public TableRow Row { get { return row; } }
		public GridViewRowType RowType { get { return this.row.RowType; } }
		public object GetValue(string fieldName) { return row.Grid.GetRowValues(VisibleIndex, fieldName); }
	}
	public class ASPxGridViewTableDataCellEventArgs : ASPxGridViewRowEventArgs {
		GridViewTableDataCell cell;
		public ASPxGridViewTableDataCellEventArgs(GridViewTableDataCell cell, object keyValue)
			: base(cell.VisibleIndex, keyValue) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
		public GridViewDataColumn DataColumn { get { return cell.DataColumn; } }
		public object CellValue { get { return GetValue(DataColumn.FieldName); } }
		public object GetValue(string fieldName) { return cell.Grid.GetRowValues(VisibleIndex, fieldName); }
	}
	public enum GridViewTableCommandCellType { Data, Filter }
	public class ASPxGridViewTableCommandCellEventArgs : ASPxGridViewRowEventArgs {
		GridViewTableBaseCommandCell cell;
		public ASPxGridViewTableCommandCellEventArgs(GridViewTableBaseCommandCell cell, object keyValue)
			: base(cell.VisibleIndex, keyValue) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
		public GridViewCommandColumn CommandColumn { get { return cell.Column ; } }
		public GridViewTableCommandCellType CommandCellType { get { return cell.CellType; } }
	}
	public class ASPxGridViewRowCommandEventArgs : ASPxGridViewRowEventArgs {
		CommandEventArgs commandArgs;
		object commandSource;
		public ASPxGridViewRowCommandEventArgs(int visibleIndex, object keyValue, CommandEventArgs commandArgs, object commandSource)
			: base(visibleIndex, keyValue) {
			this.commandArgs = commandArgs;
			this.commandSource = commandSource;
		}
		public CommandEventArgs CommandArgs { get { return commandArgs; } }
		public object CommandSource { get { return commandSource; } }
	}
	public class ASPxGridViewEditorCreateEventArgs : ASPxGridViewRowEventArgs {
		GridViewDataColumn column;
		EditPropertiesBase editorProperties;
		object value;
		public ASPxGridViewEditorCreateEventArgs(GridViewDataColumn column, int visibleIndex, EditPropertiesBase editorProperties, object keyValue, object value)
			: base(visibleIndex, keyValue) {
			this.column = column;
			this.value = value;
			this.editorProperties = editorProperties;
		}
		public GridViewDataColumn Column { get { return column; } }
		public EditPropertiesBase EditorProperties { 
			get { return editorProperties; }
			set {
				if(value == null) throw new ArgumentNullException("value");
				editorProperties = value;
			}
		}
		public object Value { get { return value; } set { this.value = value; }  }
	}
	public class ASPxGridViewEditorEventArgs : ASPxGridViewRowEventArgs {
		GridViewDataColumn column;
		ASPxEditBase editor;
		object value;
		public ASPxGridViewEditorEventArgs(GridViewDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value)
			: base(visibleIndex, keyValue) {
			this.column = column;
			this.value = value;
			this.editor = editor;
		}
		public GridViewDataColumn Column { get { return column; } }
		public ASPxEditBase Editor { get { return editor; } }
		public object Value { get { return value; } }
	}
	public class FilterValue {
		string displayText, query, value;
		public const string FilterAllQuery = "(ShowAll)";
		public static FilterValue CreateShowAllValue(string text) {
			return new FilterValue(text, "", FilterValue.FilterAllQuery);
		}
		public FilterValue(string displayText, string value, string query) {
			this.displayText = displayText;
			this.query = query;
			this.value = value;
		}
		public FilterValue(string displayText, string value) : this(displayText, value, "") { }
		public FilterValue() : this("", "", "") { }
		public string DisplayText {
			get { return displayText; }
			set {
				if(value == null) value = string.Empty;
				displayText = value;
			}
		}
		public string Query {
			get { return query; }
			set {
				if(value == null) value = string.Empty;
				query = value;
			}
		}
		public string Value {
			get { return value; }
			set {
				if(value == null) value = string.Empty;
				this.value = value;
			}
		}
		public bool IsFilterByValue { get { return string.IsNullOrEmpty(Query); } }
		public bool IsFilterByQuery { get { return !string.IsNullOrEmpty(Query); } }
		public bool IsShowAllFilter { get { return Query == FilterAllQuery; } }
		public bool IsEmpty { get { return IsShowAllFilter; } }
		public override string ToString() { return DisplayText; }
		internal string HtmlValue {
			get {
				if(IsFilterByQuery) return "#" + Query;
				return "!" + Value;
			}
		}
		internal static FilterValue FromHtmlValue(string value) {
			if(string.IsNullOrEmpty(value)) return new FilterValue("", "", FilterAllQuery);
			if(value[0] == '#') return new FilterValue(value, "", value.Substring(1));
			return new FilterValue(value.Substring(1), value.Substring(1), "");
		}
	}
	public class ASPxGridViewHeaderFilterEventArgs : EventArgs {
		GridViewDataColumn column;
		List<FilterValue> values;
		public ASPxGridViewHeaderFilterEventArgs(GridViewDataColumn column, List<FilterValue> values) {
			this.column = column;
			this.values = values;
		}
		protected string HeaderFilterShowAllText {
			get {
				if(Column == null || Column.Grid == null) return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowAllItem);
				return Column.Grid.SettingsText.GetHeaderFilterShowAll();
			}
		}
		public GridViewDataColumn Column { get { return column; } }
		public List<FilterValue> Values { get { return values; } }
		public void AddValue(string displayText, string value) {
			Values.Add(new FilterValue(displayText, value));
		}
		public void AddValue(string displayText, string value, string query) {
			Values.Add(new FilterValue(displayText, value, query));
		}
		public void AddShowAll() {
			Values.Add(FilterValue.CreateShowAllValue(HeaderFilterShowAllText));
		}
	}
	public class ASPxGridViewBeforeColumnGroupingSortingEventArgs : EventArgs {
		GridViewDataColumn column;
		ColumnSortOrder oldSortOrder;
		int oldSortIndex;
		int oldGroupIndex;
		public ASPxGridViewBeforeColumnGroupingSortingEventArgs(GridViewDataColumn column,
			ColumnSortOrder oldSortOrder, int oldSortIndex, int oldGroupIndex) {
			this.column = column;
			this.oldSortOrder = oldSortOrder;
			this.oldSortIndex = oldSortIndex;
			this.oldGroupIndex = oldGroupIndex;
		}
		public GridViewDataColumn Column { get { return column; } }
		public ColumnSortOrder OldSortOrder { get { return oldSortOrder; } }
		public int OldSortIndex { get { return oldSortIndex; } }
		public int OldGroupIndex { get { return oldGroupIndex; } }
	}
	public class ASPxGridViewCustomCallbackEventArgs : EventArgs {
		string parameters;
		public ASPxGridViewCustomCallbackEventArgs(string parameters) {
			this.parameters = parameters;
		}
		public string Parameters { get { return parameters; } }
	}
	public class ASPxGridViewCustomButtonCallbackEventArgs : EventArgs {
		string buttonID;
		int visibleIndex;
		public ASPxGridViewCustomButtonCallbackEventArgs(string buttonID, int visibleIndex) {
			this.buttonID = buttonID;
			this.visibleIndex = visibleIndex;
		}
		public string ButtonID { get { return buttonID; } }
		public int VisibleIndex { get { return visibleIndex; } }
	}
	public class ASPxGridViewCustomDataCallbackEventArgs : ASPxGridViewCustomCallbackEventArgs {
		object result;
		public ASPxGridViewCustomDataCallbackEventArgs(string parameters) : base(parameters) {}
		public object Result { get { return result; } set { result = value; } }
	}
	public class ASPxGridViewAfterPerformCallbackEventArgs : EventArgs {
		string callbackName;
		string[] args;
		public ASPxGridViewAfterPerformCallbackEventArgs(string callbackName, string[] args) {
			this.callbackName = callbackName;
			this.args = args;
		}
		public string CallbackName { get { return callbackName; } }
		public string[] Args { get { return args; } }
	}
	public class ASPxGridViewColumnDataEventArgs : EventArgs {
		GridViewDataColumn column;
		object _value = null;
		int listSourceRow;
		bool isGetAction = true;
		public ASPxGridViewColumnDataEventArgs(GridViewDataColumn column, int listSourceRow, object _value, bool isGetAction) {
			this.isGetAction = isGetAction;
			this.column = column;
			this.listSourceRow = listSourceRow;
			this._value = _value;
		}
		protected WebDataProxy Data { get { return Column.Grid.DataProxy; } }
		public GridViewDataColumn Column { get { return column; } }
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public bool IsGetData { get { return isGetAction; } }
		public bool IsSetData { get { return !IsGetData; } }
		public object Value { get { return _value; } set { _value = value; } }
		public object GetListSourceFieldValue(string fieldName) {
			return GetListSourceFieldValue(ListSourceRowIndex, fieldName);
		}
		public object GetListSourceFieldValue(int listSourceRowIndex, string fieldName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
	}
	public class ASPxGridViewSummaryDisplayTextEventArgs : EventArgs {
		ASPxSummaryItem item;
		string text;
		object value;
		int visibleIndex;
		bool isGroupRow;
		public ASPxGridViewSummaryDisplayTextEventArgs(ASPxSummaryItem item, object value, string text, int visibleIndex, bool isGroupRow) {
			this.isGroupRow = isGroupRow;
			this.visibleIndex = visibleIndex;
			this.item = item;
			this.text = text;
			this.value = value;
		}
		public string Text {
			get { return text; }
			set {
				if(value == null) value = string.Empty;
				text = value;
			}
		}
		public int VisibleIndex { get { return visibleIndex; } }
		public object Value { get { return value; } }
		public bool IsGroupSummary { get { return isGroupRow; } }
		public bool IsTotalSummary { get { return !isGroupRow; } }
		public ASPxSummaryItem Item { get { return item; } }
	}
	public class ASPxGridViewColumnDisplayTextEventArgs : EventArgs {
		GridViewDataColumn column;
		object _value = null;
		string displayText = null;
		int visibleRowIndex;
		public ASPxGridViewColumnDisplayTextEventArgs(GridViewDataColumn column, int visibleRowIndex, object _value) {
			this._value = _value;
			this.column = column;
			this.visibleRowIndex = visibleRowIndex;
		}
		protected WebDataProxy Data { get { return Column.Grid.DataProxy; } }
		public GridViewDataColumn Column { get { return column; } }
		public int VisibleRowIndex { get { return visibleRowIndex; } }
		public string DisplayText {
			get { return displayText; }
			set { displayText = value; }
		}
		public object Value { get { return _value; } set { _value = value; } }
		public object GetFieldValue(string fieldName) {
			return GetFieldValue(VisibleRowIndex, fieldName);
		}
		public object GetFieldValue(int visibleRowIndex, string fieldName) {
			return Data.GetRowValue(visibleRowIndex, fieldName);
		}
	}
	public enum GridViewDetailRowButtonState { Visible, Hidden }
	public class ASPxGridViewDetailRowButtonEventArgs : ASPxGridViewRowEventArgs {
		GridViewDetailRowButtonState buttonState;
		bool isExpanded;
		public ASPxGridViewDetailRowButtonEventArgs(int visibleIndex, object keyValue, bool isExpanded): base(visibleIndex, keyValue) {
			this.buttonState = GridViewDetailRowButtonState.Visible;
			this.isExpanded = isExpanded;
		}
		public bool IsExpanded { get { return isExpanded; } }
		public GridViewDetailRowButtonState ButtonState { get { return buttonState; } set { buttonState = value; } }
	}
	public enum GridViewErrorTextKind { General, RowValidate };
	public class ASPxGridViewCustomErrorTextEventArgs : EventArgs {
		string errorText;
		Exception exception;
		GridViewErrorTextKind errorTextKind;
		public ASPxGridViewCustomErrorTextEventArgs(GridViewErrorTextKind errorTextKind, string errorText) : this(null, errorTextKind, errorText) { }
		public ASPxGridViewCustomErrorTextEventArgs(Exception exception, GridViewErrorTextKind errorTextKind, string errorText) {
			this.errorTextKind = errorTextKind;
			this.errorText = errorText;
			this.exception = exception;
		}
		public GridViewErrorTextKind ErrorTextKind { get { return errorTextKind; } }
		public string ErrorText {
			get { return errorText; }
			set {
				if(value == null) value = string.Empty;
				errorText = value;
			}
		}
		public Exception Exception { get { return exception; } }
	}
	public enum GridViewAutoFilterEventKind { CreateCriteria, ExtractDisplayText };
	public class ASPxGridViewAutoFilterEventArgs : EventArgs {
		GridViewAutoFilterEventKind kind;
		GridViewDataColumn column;
		CriteriaOperator criteria;
		string value;
		public ASPxGridViewAutoFilterEventArgs(GridViewDataColumn column, CriteriaOperator criteria, GridViewAutoFilterEventKind kind, string value) {
			this.column = column;
			this.criteria = criteria;
			this.kind = kind;
			this.value = value;
		}
		public GridViewAutoFilterEventKind Kind { get { return kind; } }
		public GridViewDataColumn Column { get { return column; } }
		public CriteriaOperator Criteria { 
			get { return criteria; }
			set { criteria = value; }
		}
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
	}
	public class CustomColumnSortEventArgs : EventArgs {
		GridViewDataColumn column;
		ColumnSortOrder sortOrder;
		bool handled = false;
		internal object value1, value2;
		int result = 0;
		int listSourceRow1, listSourceRow2;
		internal ASPxGridView grid;
		public CustomColumnSortEventArgs(GridViewDataColumn column, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.listSourceRow1 = this.listSourceRow2 = DataController.InvalidRow;
			this.column = column;
			this.value1 = value1;
			this.value2 = value2;
		}
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public GridViewDataColumn Column { get { return column; } set { column = value; } }
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
		public object GetRow1Value(string fieldName) { return GetRowValueCore(this.listSourceRow1, fieldName);  }
		public object GetRow2Value(string fieldName) { return GetRowValueCore(this.listSourceRow2, fieldName); }
		object GetRowValueCore(int listSourceRow, string fieldName) {
			if(grid == null) return null;
			return grid.DataProxy.GetListSourceRowValue(listSourceRow, fieldName);
		}
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.value1 = value1;
			this.value2 = value2;
			this.result = 0;
			this.handled = false;
			this.listSourceRow1 = listSourceRow1;
			this.listSourceRow2 = listSourceRow2;
		}
	}
	public class ASPxGridViewClientJSPropertiesEventArgs : CustomJSPropertiesEventArgs {
		public ASPxGridViewClientJSPropertiesEventArgs()
			: base() {
		}
		public ASPxGridViewClientJSPropertiesEventArgs(Dictionary<string, object> properties)
			: base(properties) {
		}
	}
	public class ASPxGridViewEditFormEventArgs : EventArgs {
		WebControl editForm;
		public ASPxGridViewEditFormEventArgs(WebControl editForm) {
			this.editForm = editForm;
		}
		public WebControl EditForm { get { return editForm; } }
	}
	public class ASPxGridViewTableFooterCellEventArgs : EventArgs {
		ASPxGridView grid;
		GridViewColumn column;
		int visibleIndex;
		TableCell cell;
		internal ASPxGridViewTableFooterCellEventArgs(ASPxGridView grid, GridViewColumn column, int visibleIndex, TableCell cell) {
			this.grid = grid;
			this.column = column;
			this.visibleIndex = visibleIndex;
			this.cell = cell;
		}
		public GridViewColumn Column { get { return column; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public bool IsTotalFooter { get { return VisibleIndex < 0; } }
		public TableCell Cell { get { return cell; } }
		protected ASPxGridView Grid { get { return grid; } }
		public object GetSummaryValue(ASPxSummaryItem item) {
			if(IsTotalFooter)
				return Grid.GetTotalSummaryValue(item);
			return Grid.GetGroupSummaryValue(VisibleIndex, item);
		}
	}
	public class ASPxGridViewDetailRowEventArgs : EventArgs {
		int visibleIndex;
		bool expanded;
		public ASPxGridViewDetailRowEventArgs(int visibleIndex, bool expanded) {
			this.visibleIndex = visibleIndex;
			this.expanded = expanded;
		}
		public int VisibleIndex { get { return visibleIndex; } }
		public bool Expanded { get { return expanded; } }
	}
	public class ASPxGridViewCustomButtonEventArgs : EventArgs {
		GridViewCommandColumnCustomButton button;
		int visibleIndex;
		GridViewTableCommandCellType cellType;
		bool isEditingRow;
		DefaultBoolean isVisible;
		public ASPxGridViewCustomButtonEventArgs(GridViewCommandColumnCustomButton button, int visibleIndex, GridViewTableCommandCellType cellType, bool isEditingRow) {
			this.button = button;
			this.visibleIndex = visibleIndex;
			this.cellType = cellType;
			this.isEditingRow = isEditingRow;
			this.isVisible = DefaultBoolean.Default;						
		}
		public GridViewCommandColumnCustomButton Button { get { return button; } }
		public GridViewCommandColumn Column { get { return Button.Column; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public GridViewTableCommandCellType CellType { get { return cellType; } }
		public bool IsEditingRow { get { return isEditingRow; } }
		public DefaultBoolean IsVisible { get { return isVisible; } set { isVisible = value; } }
	}
	public class ASPxGridViewCommandButtonEventArgs : EventArgs {
		GridViewCommandColumnButton button;
		int visibleIndex;
		bool isEditingRow;
		bool visible;
		bool enabled;
		public ASPxGridViewCommandButtonEventArgs(GridViewCommandColumnButton button, int visibleIndex, bool isEditingRow) {
			this.button = button;
			this.visibleIndex = visibleIndex;
			this.isEditingRow = isEditingRow;
			this.visible = true;
			this.enabled = true;
		}
		public GridViewCommandColumnButton Button { get { return button; } }
		public GridViewCommandColumn Column { get { return Button.Column; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public bool IsEditingRow { get { return isEditingRow; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public bool Enabled { get { return enabled; } set { enabled = value; } }
	}
	public delegate void ASPxGridViewCustomColumnSortEventHandler(object sender, CustomColumnSortEventArgs e);
	public delegate void ASPxGridViewBeforeColumnGroupingSortingEventHandler(object sender, ASPxGridViewBeforeColumnGroupingSortingEventArgs e);
	public delegate void ASPxGridViewAutoFilterEventHandler(object sender, ASPxGridViewAutoFilterEventArgs e);
	public delegate void ASPxGridViewHeaderFilterEventHandler(object sender, ASPxGridViewHeaderFilterEventArgs e);
	public delegate void ASPxGridViewEditorCreateEventHandler(object sender, ASPxGridViewEditorCreateEventArgs e);
	public delegate void ASPxGridViewRowCommandEventHandler(object sender, ASPxGridViewRowCommandEventArgs e);
	public delegate void ASPxGridViewTableRowEventHandler(object sender, ASPxGridViewTableRowEventArgs e);
	public delegate void ASPxGridViewTableDataCellEventHandler(object sender, ASPxGridViewTableDataCellEventArgs e);
	public delegate void ASPxGridViewTableCommandCellEventHandler(object sender, ASPxGridViewTableCommandCellEventArgs e);
	public delegate void ASPxGridViewEditorEventHandler(object sender, ASPxGridViewEditorEventArgs e);
	public delegate void ASPxGridViewCustomCallbackEventHandler(object sender, ASPxGridViewCustomCallbackEventArgs e);
	public delegate void ASPxGridViewCustomDataCallbackEventHandler(object sender, ASPxGridViewCustomDataCallbackEventArgs e);
	public delegate void ASPxGridViewAfterPerformCallbackEventHandler(object sender, ASPxGridViewAfterPerformCallbackEventArgs e);
	public delegate void ASPxGridViewCustomButtonCallbackEventHandler(object sender, ASPxGridViewCustomButtonCallbackEventArgs e);
	public delegate void ASPxGridViewColumnDisplayTextEventHandler(object sender, ASPxGridViewColumnDisplayTextEventArgs e);
	public delegate void ASPxGridViewColumnDataEventHandler(object sender, ASPxGridViewColumnDataEventArgs e);
	public delegate void ASPxGridViewSummaryDisplayTextEventHandler(object sender, ASPxGridViewSummaryDisplayTextEventArgs e);
	public delegate void ASPxGridViewDetailRowButtonEventHandler(object sender, ASPxGridViewDetailRowButtonEventArgs e);
	public delegate void ASPxGridViewCustomErrorTextEventHandler(object sender, ASPxGridViewCustomErrorTextEventArgs e);
	public delegate void ASPxGridViewClientJSPropertiesEventHandler(object sender, ASPxGridViewClientJSPropertiesEventArgs e);
	public delegate void ASPxGridViewEditFormEventHandler(object sender, ASPxGridViewEditFormEventArgs e);
	public delegate void ASPxGridViewTableFooterCellEventHandler(object sender, ASPxGridViewTableFooterCellEventArgs e);
	public delegate void ASPxGridViewDetailRowEventHandler(object sender, ASPxGridViewDetailRowEventArgs e);
	public delegate void ASPxGridViewCustomButtonEventHandler(object sender, ASPxGridViewCustomButtonEventArgs e);
	public delegate void ASPxGridViewCommandButtonEventHandler(object sender, ASPxGridViewCommandButtonEventArgs e);
}
