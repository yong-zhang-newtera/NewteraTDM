#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraPivotGrid                                 }
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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid {
	public class PivotFieldEventArgs : EventArgs {
		PivotGridField field;
		public PivotFieldEventArgs(PivotGridField field) {
			this.field = field;
		}
		public PivotGridField Field { get { return field; } }
	}
	public class PivotCustomGroupIntervalEventArgs : PivotFieldEventArgs {
		object value, groupValue;
		public PivotCustomGroupIntervalEventArgs(PivotGridField field, object value) : base(field) {
			this.groupValue = this.value = value;
		}
		public object Value { get { return value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
	}
	public class PivotFieldValueEventArgs : PivotFieldEventArgs {
		PivotFieldsAreaCellViewInfo fieldCellViewInfo = null;
		public PivotFieldValueEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo)
			: base(fieldCellViewInfo.Field) {
			this.fieldCellViewInfo = fieldCellViewInfo;
		}
		public PivotFieldValueEventArgs(PivotGridField field) : base(field) {}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldsAreaCellViewInfo FieldCellViewInfo { get { return fieldCellViewInfo; } }
		protected PivotCellsViewInfoBase CellsArea { get { return Data.ViewInfo.CellsArea; } }
		public bool IsColumn { get { return FieldCellViewInfo != null ? FieldCellViewInfo.IsColumn : true; } }
		public int MinIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.MinLastLevelIndex : -1; } }
		public int MaxIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.MaxLastLevelIndex : -1; } }
		public int FieldIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.VisibleIndex : -1; } }
		public virtual object Value { get { return FieldCellViewInfo != null ? FieldCellViewInfo.Value : null; }  }
		public bool IsOthersValue { get { return FieldCellViewInfo != null ? FieldCellViewInfo.IsOthersValue : false; } }
		public PivotGridValueType ValueType { get { return FieldCellViewInfo != null ? FieldCellViewInfo.ValueType : PivotGridValueType.Value; } }
		public PivotGridCustomTotal CustomTotal { get { return FieldCellViewInfo != null ? FieldCellViewInfo.CustomTotal : null; } }
		public bool IsCollapsed { get { return FieldCellViewInfo.IsCollapsed; } }
		public void ChangeExpandedState() { FieldCellViewInfo.ChangeExpanded(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridViewInfoData Data { get { return FieldCellViewInfo.Data; } }
		public PivotGridField[] GetHigherLevelFields() {
			if(Field == null) return new PivotGridField[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotGridField[] fields = new PivotGridField[Data.GetFieldCountByArea(Data.OptionsDataField.DataFieldArea)];
				for(int i = 0; i < fields.Length; i++)
					fields[i] = (PivotGridField)Data.GetFieldByArea(Data.OptionsDataField.DataFieldArea, i);
				return fields;
			} else {
				PivotGridField[] fields = new PivotGridField[Field.AreaIndex];
				for(int i = Field.AreaIndex - 1; i >= 0; i--) {
					fields[i] = Data.GetFieldByLevel(FieldCellViewInfo.IsColumn, i) as PivotGridField;
				}
				return fields;
			}
		}
		public object GetHigherLevelFieldValue(PivotGridField field) {
			if(field.Area != Field.Area || field.AreaIndex > Field.AreaIndex || !field.Visible) return null;
			return Data.GetFieldValue(field, FieldCellViewInfo.VisibleIndex, FieldCellViewInfo.VisibleIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return FieldCellViewInfo.Item.CreateDrillDownDataSource();
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return FieldCellViewInfo.Item.CreateOLAPDrillDownDataSource(maxRowCount, customColumns);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= Data.VisualItems.ColumnCount)
				throw new ArgumentOutOfRangeException("columnIndex");
			if(rowIndex < 0 || rowIndex >= Data.VisualItems.RowCount)
				throw new ArgumentOutOfRangeException("rowIndex");
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldValueEventArgs {
		string displayText;
		object value;
		public PivotFieldDisplayTextEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo) : base(fieldCellViewInfo) {
			this.value = FieldCellViewInfo.Value;
			this.displayText = FieldCellViewInfo.Text;
		}
		public PivotFieldDisplayTextEventArgs(PivotGridField field, object value) : base(field) {
			this.value = value;
			this.displayText = field.GetValueText(value);
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return FieldCellViewInfo == null; } }
	}
	public class PivotCustomRowHeightEventArgs : PivotFieldValueEventArgs {
		int height;
		public PivotCustomRowHeightEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int height)
			: base(fieldCellViewInfo) {
			this.height = height;
		}
		public int RowHeight {
			get { return height; }
			set { height = value; }
		}
		public int RowLineCount {
			get { return FieldCellViewInfo.GetRowLineCount(); }
		}
		public int RowIndex {
			get { return MinIndex; }
		}
		public int ColumnCount { get { return CellsArea.ColumnCount; } }
		public object GetRowCellValue(int columnIndex) {
			return GetCellValue(columnIndex, RowIndex);
		}
	}
	public class PivotCustomColumnWidthEventArgs : PivotFieldValueEventArgs {
		int width;
		public PivotCustomColumnWidthEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int width)
			: base(fieldCellViewInfo) {
			this.width = width;
		}
		public int ColumnWidth {
			get { return width; }
			set { width = value; }
		}
		public int ColumnLineCount {
			get { return FieldCellViewInfo.GetColumnLineCount(); }
		}
		public int ColumnIndex {
			get { return MinIndex; }
		}
		public int RowCount { get { return CellsArea.RowCount; } }		
		public object GetColumnCellValue(int rowIndex) {
			return GetCellValue(ColumnIndex, rowIndex);
		}
	}
	public class PivotFieldImageIndexEventArgs : PivotFieldValueEventArgs {
		int imageIndex;
		public PivotFieldImageIndexEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo) : base(fieldCellViewInfo) {
			this.imageIndex = -1;
		}
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
	}
	public class PivotFieldValueCancelEventArgs : PivotFieldValueEventArgs {
		bool cancel = false;
		public PivotFieldValueCancelEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo) : base(fieldCellViewInfo) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PivotCellBaseEventArgs : EventArgs {
		PivotCellViewInfo cellViewInfo;
		public PivotCellBaseEventArgs(PivotCellViewInfo cellViewInfo) {
			this.cellViewInfo = cellViewInfo;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotCellViewInfo CellViewInfo { get { return cellViewInfo; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridViewInfoData Data { get { return CellViewInfo.Data; } }
		public PivotGridField DataField { get { return CellViewInfo.DataField; } }
		public int ColumnIndex { get { return CellViewInfo.ColumnIndex; } }
		public int RowIndex { get { return CellViewInfo.RowIndex; } }
		public int ColumnFieldIndex { get {	return CellViewInfo.ColumnFieldIndex; } }
		public int RowFieldIndex { get {	return CellViewInfo.RowFieldIndex; } }
		public PivotGridField ColumnField { get { return CellViewInfo.ColumnField; } }
		public PivotGridField RowField { get { return CellViewInfo.RowField; } }
		public object Value { get { return CellViewInfo.Value; } }
		public PivotSummaryValue SummaryValue { get { return Data.GetCellSummaryValue(ColumnFieldIndex, RowFieldIndex, DataField); } }
		public PivotGridValueType ColumnValueType { get { return CellViewInfo.ColumnValueType; } }
		public PivotGridValueType RowValueType { get { return CellViewInfo.RowValueType; } }
		public PivotGridCustomTotal ColumnCustomTotal { get { return CellViewInfo.ColumnCustomTotal as PivotGridCustomTotal; } }
		public PivotGridCustomTotal RowCustomTotal { get { return CellViewInfo.RowCustomTotal as PivotGridCustomTotal; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, CellViewInfo.DataIndex);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetOLAPDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, CellViewInfo.DataIndex, maxRowCount,
				customColumns);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(List<string> customColumns) {
			return CreateOLAPDrillDownDataSource(-1, customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource(ColumnFieldIndex, RowFieldIndex);
		}
		public object GetFieldValue(PivotGridField field) {
			return CellViewInfo.GetFieldValue(field);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
		public bool IsOthersFieldValue(PivotGridField field) {
			return CellViewInfo.IsOthersFieldValue(field);
		}
		public bool IsFieldValueExpanded(PivotGridField field) {
			return CellViewInfo.IsFieldValueExpanded(field);
		}
		public bool IsFieldValueRetrievable(PivotGridField field) {
			return CellViewInfo.IsFieldValueRetrievable(field);
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
			for(int i = 0; i < fields.Length; i ++)
				fields[i] = Data.GetFieldByArea(area, i) as PivotGridField;
			return fields;
		}
		public object GetCellValue(PivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
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
	public class EditValueChangedEventArgs : PivotCellBaseEventArgs {
		BaseEdit editor;
		public BaseEdit Editor { get { return editor; } }
		public EditValueChangedEventArgs(PivotCellViewInfo cellInfo, BaseEdit editor)
			: base(cellInfo) {
			if(cellInfo == null || editor == null) throw new ArgumentException("cellInfo or editor is null");
			this.editor = editor;			
		}
	}
	public class PivotCellEventArgs : PivotCellBaseEventArgs {
		public PivotCellEventArgs(PivotCellViewInfo cellViewInfo) : base(cellViewInfo) {
		}
		public bool Focused { get { return CellViewInfo.Focused; } }
		public bool Selected { get { return CellViewInfo.Selected; } }
		public string DisplayText { get { return CellViewInfo.Text; } }
		public Rectangle Bounds { get { return CellViewInfo.PaintBounds; } }
	}
	public class CancelPivotCellEditEventArgs : PivotCustomCellEditEventArgs {
		public CancelPivotCellEditEventArgs(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem)
			: base(cellViewInfo, repositoryItem) {
		}
		public new RepositoryItem RepositoryItem {
			get { return base.RepositoryItem; }
		}
		bool cancel;
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		public PivotCellDisplayTextEventArgs(PivotCellViewInfo cellViewInfo) : base(cellViewInfo) {
			this.displayText = CellViewInfo.Text;
		}
		public string DisplayText { get { return displayText; }  set { displayText = value; } }
	}
	public class PivotCustomDrawCellEventArgs : PivotCellEventArgs {
		AppearanceObject appearance;
		ViewInfoPaintArgs paintArgs;
		bool handle;
		public PivotCustomDrawCellEventArgs(PivotCellViewInfo cellViewInfo,	AppearanceObject appearance, ViewInfoPaintArgs paintArgs) : base(cellViewInfo) {
			this.appearance = appearance;
			this.paintArgs = paintArgs;
			this.handle = false;
		}
		public bool Handled { get { return handle; } set { handle = value; } }
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		public Graphics Graphics { get { return paintArgs.Graphics; } }
		public GraphicsCache GraphicsCache { get { return paintArgs.GraphicsCache; } }
	}
	public class PivotCustomAppearanceEventArgs : PivotCellEventArgs {
		AppearanceObject appearance;
		public PivotCustomAppearanceEventArgs(PivotCellViewInfo cellViewInfo, AppearanceObject appearance)
			: base(cellViewInfo) {
			this.appearance = appearance;
		}
		public AppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
	}
	public interface IPivotCustomDrawAppearanceOwner {
		AppearanceObject Appearance { get; set; }
	}
	public class PivotCustomDrawEventArgs : EventArgs {
		IPivotCustomDrawAppearanceOwner appearanceOwner;
		ViewInfoPaintArgs paintArgs;
		Rectangle bounds;
		bool handled;
		public PivotCustomDrawEventArgs(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			this.appearanceOwner = appearanceOwner;
			this.paintArgs = paintArgs;
			this.bounds = bounds;
			this.handled = false;
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public AppearanceObject Appearance { 
			get { return appearanceOwner.Appearance; } 			
			set {
				if(value == null) return;
				appearanceOwner.Appearance = value;
			}
		}
		public Graphics Graphics { get { return paintArgs.Graphics; } }
		public GraphicsCache GraphicsCache { get { return paintArgs.GraphicsCache; } }
		public Rectangle Bounds { get { return bounds; } }
	}
	public class PivotCustomDrawHeaderAreaEventArgs : PivotCustomDrawEventArgs {
		PivotHeadersViewInfoBase headersViewInfo;
		public PivotCustomDrawHeaderAreaEventArgs(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds) 
			: base(headersViewInfo, paintArgs, bounds) {
			this.headersViewInfo = headersViewInfo;
		}
		public PivotArea Area { get { return headersViewInfo.Area; } }
	}
	public class PivotCustomDrawFieldHeaderEventArgs : PivotCustomDrawEventArgs {
		PivotHeaderViewInfoBase headerViewInfo;
		HeaderObjectPainter painter;
		public PivotCustomDrawFieldHeaderEventArgs(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter)
			: base(headerViewInfo, paintArgs, headerViewInfo.InfoArgs.Bounds) {
			this.headerViewInfo = headerViewInfo;
			this.painter = painter;
		}
		public PivotGridField Field { get { return headerViewInfo.Field; } }
		public HeaderObjectInfoArgs Info { get { return headerViewInfo.InfoArgs; } }
		public HeaderObjectPainter Painter { get { return painter; } }
	}
	public class PivotCustomDrawFieldValueEventArgs : PivotCustomDrawEventArgs {
		PivotFieldsAreaCellViewInfo fieldCellViewInfo;
		HeaderObjectInfoArgs info;
		HeaderObjectPainter painter;
		public PivotCustomDrawFieldValueEventArgs(PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, 
			ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter)
			: base(fieldCellViewInfo, paintArgs, info.Bounds) {
			this.fieldCellViewInfo = fieldCellViewInfo;
			this.info = info;
			this.painter = painter;
		}
		protected PivotFieldsAreaCellViewInfo FieldCellViewInfo { get { return fieldCellViewInfo; } }
		public object Value { get { return FieldCellViewInfo.Value; } }
		public int MinIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.MinLastLevelIndex : -1; } }
		public int MaxIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.MaxLastLevelIndex : -1; } }
		public int FieldIndex { get { return FieldCellViewInfo != null ? FieldCellViewInfo.VisibleIndex : -1; } }
		public string DisplayText { get { return Info.Caption; } }
		public PivotGridValueType ValueType { get { return FieldCellViewInfo.ValueType; } }
		public PivotGridCustomTotal CustomTotal { get { return FieldCellViewInfo.CustomTotal; } }
		public bool IsOthersValue { get { return FieldCellViewInfo != null ? FieldCellViewInfo.IsOthersValue : false; } }
		public PivotGridField Field { get { return FieldCellViewInfo.Field; } }
		public PivotArea Area { get { return FieldCellViewInfo.Item.Area; } }
		public HeaderObjectInfoArgs Info { get { return info; } }
		public HeaderObjectPainter Painter { get { return painter; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridViewInfoData Data { get { return FieldCellViewInfo.Data; } }
		public PivotGridField[] GetHigherLevelFields() {
			if(Field == null) return new PivotGridField[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotGridField[] fields = new PivotGridField[Data.GetFieldCountByArea(Data.OptionsDataField.DataFieldArea)];
				for(int i = 0; i < fields.Length; i++)
					fields[i] = (PivotGridField)Data.GetFieldByArea(Data.OptionsDataField.DataFieldArea, i);
				return fields;
			} else {
				PivotGridField[] fields = new PivotGridField[Field.AreaIndex];
				for(int i = Field.AreaIndex - 1; i >= 0; i--) {
					fields[i] = Data.GetFieldByLevel(FieldCellViewInfo.IsColumn, i) as PivotGridField;
				}
				return fields;
			}
		}
		public object GetHigherLevelFieldValue(PivotGridField field) {
			if(field.Area != Field.Area || field.AreaIndex > Field.AreaIndex || !field.Visible) return null;
			return Data.GetFieldValue(field, FieldCellViewInfo.VisibleIndex, FieldCellViewInfo.VisibleIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= Data.VisualItems.ColumnCount)
				throw new ArgumentOutOfRangeException("columnIndex");
			if(rowIndex < 0 || rowIndex >= Data.VisualItems.RowCount)
				throw new ArgumentOutOfRangeException("rowIndex");
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
	}
	public class PivotAreaChangingEventArgs : PivotFieldEventArgs {
		int newAreaIndex;
		PivotArea newArea;
		bool allow;
		public PivotAreaChangingEventArgs(PivotGridField field, PivotArea newArea, int newAreaIndex) : base(field) {
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
			this.allow = true;
		}
		public int NewAreaIndex { get { return newAreaIndex; } }
		public PivotArea NewArea { get { return newArea; } }
		public bool Allow { get { return allow; } set { allow = value; } }
	}
	public class CustomFieldDataEventArgs : EventArgs {
		PivotGridField field;
		object _value = null;
		int listSourceRow;
		PivotGridData data;
		public CustomFieldDataEventArgs(PivotGridData data, PivotGridField field, int listSourceRow, object _value) {
			this.field = field;
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.data = data;
		}
		public PivotGridField Field { get { return field; } }
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
		int  result = 0;
		int listSourceRow1, listSourceRow2;
		public PivotGridCustomFieldSortEventArgs(PivotGridData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			SetArgs(-1, -1, null, null, PivotSortOrder.Ascending);
		}
		public PivotSortOrder SortOrder { get { return sortOrder; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridData Data { get { return data; } }
		public PivotGridField Field { get { return field; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		public int  Result {
			get { return result; }
			set { result = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetSortResult() {
			if(!Handled) return PivotGridData.UnhandledCustomFieldSort;
			return Result;
		}
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotSortOrder sortOrder) {
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
	public enum PivotGridMenuType { Header, HeaderArea, FieldValue, HeaderSummaries };
	public class PivotGridMenuEventArgs : PivotGridMenuEventArgsBase {
		public PivotGridMenuEventArgs(PivotGridViewInfo viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridField field, PivotArea area, Point point)
			: base(viewInfo, menuType, menu, field, area, point) { }
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
		public PivotGridHitInfo HitInfo { get { return ViewInfo.CalcHitInfo(Point); } }
	}
	public class PivotGridMenuEventArgsBase : EventArgs {
		PivotGridViewInfoBase viewInfo;
		PivotGridMenuType menuType;
		DXPopupMenu menu;
		PivotGridFieldBase field;
		PivotArea area;
		Point point;
		bool allow;
		public PivotGridMenuEventArgsBase(PivotGridViewInfoBase viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridFieldBase field, PivotArea area, Point point) {
			this.viewInfo = viewInfo;
			this.menu = menu;
			this.menuType = menuType;
			this.field = field;
			this.area = area;
			this.point = point;
			this.allow = true;
		}
		protected PivotGridViewInfoBase ViewInfo { get { return viewInfo; } }
		[Description("Gets the field whose header has been right-clicked by an end-user.")]
		public PivotGridFieldBase Field { get { return field; } }
		[Description("Gets the type of the invoked menu.")]
		public PivotGridMenuType MenuType { get { return menuType; } }
		[Description("Gets the area of the field whose header or value has been right-clicked by a user to invoke the context menu.")]
		public PivotArea Area { get { return area; } }
		[Description("Gets or sets whether the context menu is displayed.")]
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
		[Description("Gets or sets the pivot grid's context menu.")]
		public DXPopupMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
		[Description("Gets or sets the location at which the context menu has been invoked.")]
		public Point Point {
			get { return point; }
			set { point = value; }
		}		
	}
	public class PivotGridMenuItemClickEventArgs : PivotGridMenuItemClickEventArgsBase {
		public PivotGridMenuItemClickEventArgs(PivotGridViewInfo viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridField field, PivotArea area, Point point, DXMenuItem item)
			: base(viewInfo, menuType, menu, field, area, point, item) { }
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
		public PivotGridHitInfo HitInfo { get { return ViewInfo.CalcHitInfo(Point); } }
	}
	public class PivotGridMenuItemClickEventArgsBase : PivotGridMenuEventArgsBase {
		DXMenuItem item;
		public PivotGridMenuItemClickEventArgsBase(PivotGridViewInfoBase viewInfo, PivotGridMenuType menuType, DXPopupMenu menu, PivotGridFieldBase field, PivotArea area, Point point, DXMenuItem item)
			: base(viewInfo, menuType, menu, field, area, point) {
			this.item = item;
		}
		[Description("Gets the menu item that has been clicked.")]
		public DXMenuItem Item { get { return item; } }
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
		public PivotGridField ColumnField {
			get {return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.ColColumn) as PivotGridField; }
		}
		public PivotGridField RowField {
			get {return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.RowColumn) as PivotGridField; }
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
	public class CustomizationFormShowingEventArgs : EventArgs {
		Form customizationForm;
		bool cancel;
		Control parentControl;
		public CustomizationFormShowingEventArgs(Form customizationForm, Control parentControl) {
			this.customizationForm = customizationForm;
			this.parentControl = parentControl;
			this.cancel = false;
		}
		public Form CustomizationForm { get { return customizationForm; } }
		public Control ParentControl { get { return parentControl; } set { parentControl = value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Please use Cancel property")]
		public bool Handled { get { return Cancel; } set { Cancel = value; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class CustomEditValueEventArgs : PivotCellBaseEventArgs {
		object value;
		public new object Value { get { return this.value; } set { this.value = value; } }
		public CustomEditValueEventArgs(object value, PivotCellViewInfo cellViewInfo)
			: base(cellViewInfo){
			this.value = value;
		}
	}
	public class PivotCellEditEventArgs : PivotCellBaseEventArgs {
		BaseEdit edit;
		public PivotCellEditEventArgs(PivotCellViewInfo cellViewInfo, BaseEdit edit)
			: base(cellViewInfo) {
			this.edit = edit;
		}
		public BaseEdit Edit { get { return edit; } }
	}
	public class PivotCustomCellEditEventArgs : PivotCellBaseEventArgs {
		RepositoryItem repositoryItem;
		public PivotCustomCellEditEventArgs(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem)
			: base(cellViewInfo) {
			this.repositoryItem = repositoryItem;
		}
		public RepositoryItem RepositoryItem { get { return repositoryItem; } set { repositoryItem = value; } }
	}
	public class CustomPrintEventArgs : EventArgs {
		IVisualBrick brick;
		public CustomPrintEventArgs(IVisualBrick brick) {
			this.brick = brick;
		}
		public IVisualBrick Brick { get { return brick; } }
	}
	public class CustomExportHeaderEventArgs : CustomPrintEventArgs {
		PivotHeaderViewInfoBase headerViewInfo;
		public CustomExportHeaderEventArgs(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo)
			: base(brick) {
			this.headerViewInfo = headerViewInfo;
		}
		protected PivotHeaderViewInfoBase HeaderViewInfo { get { return headerViewInfo; } }
		public AppearanceObject Appearance { get { return HeaderViewInfo.Appearance; } }
		public string Caption { get { return HeaderViewInfo.Caption; } }
		public PivotGridField Field { get { return HeaderViewInfo.Field; } }
	}
	public class CustomExportFieldValueEventArgs : CustomPrintEventArgs {
		PivotFieldsAreaCellViewInfoBase viewInfo;
		public CustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase viewInfo) : base(brick) {
			this.viewInfo = viewInfo;
		}
		protected PivotFieldsAreaCellViewInfoBase ViewInfo { get { return viewInfo; } }
		public string Text { get { return ViewInfo.Text; } }
		public object Value { get { return ViewInfo.Value; } }
		public PivotGridField Field { get { return ViewInfo.Field; } }
		public int MinIndex { get { return ViewInfo.MinLastLevelIndex; } }
		public int MaxIndex { get { return ViewInfo.MaxLastLevelIndex; } }
		public int StartLevel { get { return ViewInfo.StartLevel; } }
		public int EndLevel { get { return ViewInfo.EndLevel; } }
		public bool ContainsLevel(int level) { return ViewInfo.ContainsLevel(level); }
		public PivotGridField DataField { get { return (PivotGridField)ViewInfo.Item.DataField; } }
		public PivotGridCustomTotal CustomTotal { get { return ViewInfo.CustomTotal; } }				
		public PivotGridValueType ValueType { get { return ViewInfo.ValueType; } }		
		public bool IsTopMost { get { return ViewInfo.IsTopMost; } }		
		public bool IsCollapsed { get { return ViewInfo.IsCollapsed; } }
		public bool IsColumn { get { return ViewInfo.IsColumn; } }
		public bool IsOthersValue { get { return ViewInfo.IsOthersValue; } }
	}
	public class CustomExportCellEventArgs : CustomPrintEventArgs {
		PivotCellViewInfo cellViewInfo;
		public CustomExportCellEventArgs(IVisualBrick brick, PivotCellViewInfo cellViewInfo)
			: base(brick) {
			this.cellViewInfo = cellViewInfo;			
		}
		protected PivotCellViewInfo CellViewInfo { get { return cellViewInfo; } }
		public AppearanceObject Appearance { get { return CellViewInfo.Appearance; } }
		public string Text { get { return CellViewInfo.Text; } }
		public object Value { get { return CellViewInfo.Value; } }
		public PivotFieldValueItem ColumnValue { get { return CellViewInfo.ColumnFieldValueItem; } }
		public PivotFieldValueItem RowValue { get { return CellViewInfo.RowFieldValueItem; } }
		public PivotGridField ColumnField { get { return CellViewInfo.ColumnField; } }
		public PivotGridField RowField { get { return CellViewInfo.RowField; } }
		public PivotGridField DataField { get { return CellViewInfo.DataField; } }
		public bool Selected { get { return CellViewInfo.Selected; } }
		public bool Focused { get { return CellViewInfo.Focused; } }
		public FormatType FormatType { get { return CellViewInfo.FormatType; } }
		public bool IsTextFit { get { return CellViewInfo.IsTextFit; } }		
	}
	public delegate void CustomFieldDataEventHandler(object sender, CustomFieldDataEventArgs e);
	public delegate void PivotGridCustomFieldSortEventHandler(object sender, PivotGridCustomFieldSortEventArgs e);
	public delegate void PivotGridCustomSummaryEventHandler(object sender, PivotGridCustomSummaryEventArgs e);
	public delegate void PivotFieldEventHandler(object sender, PivotFieldEventArgs e);
	public delegate void PivotFieldValueEventHandler(object sender, PivotFieldValueEventArgs e);
	public delegate void PivotFieldValueCancelEventHandler(object sender, PivotFieldValueCancelEventArgs e);
	public delegate void PivotFieldDisplayTextEventHandler(object sender, PivotFieldDisplayTextEventArgs e);
	public delegate void PivotCustomGroupIntervalEventHandler(object sender, PivotCustomGroupIntervalEventArgs e);
	public delegate void PivotFieldImageIndexEventHandler(object sender, PivotFieldImageIndexEventArgs e);
	public delegate void PivotCellEventHandler(object sender, PivotCellEventArgs e);
	public delegate void PivotCellDisplayTextEventHandler(object sender, PivotCellDisplayTextEventArgs e);
	public delegate void PivotCustomDrawCellEventHandler(object sender, PivotCustomDrawCellEventArgs e);
	public delegate void PivotCustomAppearanceEventHandler(object sender, PivotCustomAppearanceEventArgs e);
	public delegate void PivotCustomDrawEventHandler(object sender, PivotCustomDrawEventArgs e);
	public delegate void PivotCustomDrawFieldHeaderEventHandler(object sender, PivotCustomDrawFieldHeaderEventArgs e);
	public delegate void PivotCustomDrawHeaderAreaEventHandler(object sender, PivotCustomDrawHeaderAreaEventArgs e);
	public delegate void PivotCustomDrawFieldValueEventHandler(object sender, PivotCustomDrawFieldValueEventArgs e);
	public delegate void PivotAreaChangingEventHandler(object sender, PivotAreaChangingEventArgs e);
	public delegate void PivotGridMenuEventHandler(object sender, PivotGridMenuEventArgs e);
	public delegate void PivotGridMenuItemClickEventHandler(object sender, PivotGridMenuItemClickEventArgs e);
	public delegate void CustomizationFormShowingEventHandler(object sender, CustomizationFormShowingEventArgs e);
	public delegate void EditValueChangedEventHandler(object sender, EditValueChangedEventArgs e);
	public delegate void CustomEditValueEventHandler(object sender, CustomEditValueEventArgs e);
}
