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
using DevExpress.Web.ASPxEditors.Internal;
using System.Collections.Specialized;
using DevExpress.Web.ASPxGridView.Cookies;
using DevExpress.Web.ASPxGridView.Helper;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class ASPxGridViewTextBuilder {
		ASPxGridView grid;
		public ASPxGridViewTextBuilder(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		public WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public HorizontalAlign GetColumnDisplayControlDefaultAlignment(GridViewColumn column) {
			return RenderHelper.GetColumnDisplayControlDefaultAlignment(column);
		}
		public virtual string GetHeaderCaption(GridViewColumn column) {
			return column.ToString();
		}
		public virtual string GetFooterCaption(GridViewDataColumn column, string lineDivider) {
			if(column == null) return string.Empty;
			List<ASPxSummaryItem> list = Grid.GetTotalSummaryItems(column);
			if(list.Count == 0) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < list.Count; n++) {
				if(n > 0) sb.Append(lineDivider);
				ASPxSummaryItem item = list[n];
				object value = Grid.GetTotalSummaryValue(item);
				string text = item.GetFooterDisplayText(column.ToString(), value);
				sb.Append(Grid.RaiseSummaryDisplayText(new ASPxGridViewSummaryDisplayTextEventArgs(item, value, text, -1, false)));
			}
			return sb.ToString();
		}
		public virtual string GetGroupRowDisplayText(GridViewDataColumn column, int visibleIndex) {
			object value = DataProxy.GetRowValue(visibleIndex, column.FieldName);
			GridColumnInfo info = Grid.SortData.GetInfo(column);
			string res = string.Empty;
			DevExpress.Utils.FormatInfo format = null;
			if(info != null) {
				value = info.UpdateGroupDisplayValue(value);
				format = info.GetColumnGroupFormat();
			}
			if(format != null)
				res = format.GetDisplayText(value);
			else
				res = GetRowDisplayTextCore(column, visibleIndex, value);
			if(info != null) res = info.GetGroupDisplayText(value, res);
			ASPxGridViewColumnDisplayTextEventArgs e = new ASPxGridViewColumnDisplayTextEventArgs(column, visibleIndex, value);
			e.DisplayText = res;
			Grid.RaiseCustomGroupDisplayText(e);
			return e.DisplayText;
		}
		public virtual string GetGroupRowText(GridViewDataColumn column, int visibleIndex) {
			string value = GetGroupRowDisplayText(column, visibleIndex);
			string summary = grid.GetGroupRowSummaryText(visibleIndex);
			return string.Format(Grid.Settings.GroupFormat, column.ToString(), value, summary);
		}
		public virtual string GetGroupRowFooterText(GridViewDataColumn column, int visibleIndex, string lineDivider) {
			if(column == null) return string.Empty;
			List<ASPxSummaryItem> list = Grid.GetGroupFooterSummaryItems(column);
			if(list.Count == 0) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < list.Count; n++) {
				ASPxSummaryItem item = list[n];
				if(n > 0) sb.Append(lineDivider);
				object value = Grid.GetGroupSummaryValue(visibleIndex, item);
				string text = item.GetFooterDisplayText(column.ToString(), value);
				sb.Append(Grid.RaiseSummaryDisplayText(new ASPxGridViewSummaryDisplayTextEventArgs(item, value, text, visibleIndex, true)));
			}
			return sb.ToString();
		}
		public virtual string GetPreviewText(int visibleIndex) {
			return Grid.GetPreviewText(visibleIndex);
		}
		public string GetRowDisplayText(GridViewDataColumn column, int visibleIndex) {
			return GetRowDisplayTextCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected delegate string GetEditorDisplayTextCoreFunc(EditPropertiesBase editor, GridViewDataColumn column, int visibleIndex, object value);
		protected virtual string GetDisplayTextCore(GridViewDataColumn column, int visibleIndex, object value, GetEditorDisplayTextCoreFunc func) {
			EditPropertiesBase editor = Grid.RenderHelper.GetColumnEdit(column);
			string strValue = string.Empty;
			if(editor != null)
				strValue = func(editor, column, visibleIndex, value);
			else {
				if(value == null || value == DBNull.Value)
					strValue = string.Empty;
				else
					strValue = value.ToString();
			}
			return strValue;
		}
		public virtual string GetFilterPopupItemText(GridViewDataColumn column, object value) {
			return GetDisplayTextCore(column, -1, value, GetEditorFilterItemTextCore);
		}
		public virtual string GetFilterControlItemText(GridViewDataColumn column, object value) {
			return GetDisplayTextCore(column, -1, value, GetEditorFilterItemTextCore);
		}
		protected virtual string GetRowDisplayTextCore(GridViewDataColumn column, int visibleIndex, object value) {
			return GetDisplayTextCore(column, visibleIndex, value, GetEditorDisplayTextCore);
		}
		protected virtual string GetEditorDisplayTextCore(EditPropertiesBase editor, GridViewDataColumn column, int visibleIndex, object value) {
			return editor.GetDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		public CreateDisplayControlArgs GetDisplayControlArgs(GridViewDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridView grid) {
			return GetDisplayControlArgsCore(column, visibleIndex, provider, grid, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected CreateDisplayControlArgs GetDisplayControlArgsCore(GridViewDataColumn column, int visibleIndex, object value) {
			return GetDisplayControlArgsCore(column, visibleIndex, new SimpleValueProvider(DataProxy, visibleIndex), Grid, value);
		}
		protected internal virtual CreateDisplayControlArgs GetDisplayControlArgsCore(GridViewDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridView grid, object value) {
			ASPxGridViewColumnDisplayTextEventArgs args = new ASPxGridViewColumnDisplayTextEventArgs(column, visibleIndex, value);
			Grid.RaiseCustomColumnDisplayText(args);
			CreateDisplayControlArgs res = new CreateDisplayControlArgs(value, column.GetDataType(),
				args.DisplayText, provider, grid.ImagesEditors, grid.StylesEditors, grid,
				grid.DummyNamingContainer, grid.DesignMode);
			return res;
		}
		protected virtual string GetEditorFilterItemTextCore(EditPropertiesBase editor, GridViewDataColumn column, int visibleIndex, object value) {
			CreateDisplayControlArgs args = new CreateDisplayControlArgs(value, column.GetDataType(),
				null, null, grid.ImagesEditors, grid.StylesEditors,
				grid.DummyNamingContainer, grid.DesignMode);
			return editor.GetDisplayText(args);
		}
	}
	public class ASPxGridViewRenderHelper {
		internal const string CancelBubbleJs = "event.cancelBubble = true";
		public const string DXString = "DX",
							DXMainTableString = DXString + "MainTable",
							DXHeaderTableString = DXString + "HeaderTable",
							DXFooterTableString = DXString + "FooterTable",
							DXScrollDivString = DXString + "ScrollDiv",
							DXHorzScrollDivString = DXString + "HorzScrollDiv",
							DXFixedColumnsDivString = DXString + "FixedColumnsDiv",
							DXFixedColumnsContentDivString = DXString + "FixedColumnsContentDiv",
							DragTargetString = DXString + "TD",
							DXTargetString = DXString + "T",
							DXEditorString = DXString + "Editor",
							DXFilterRowEditorString = DXString + "FREditorcol",
							DXSelectButtonString = DXString + "SelBtn",
							DXSelectedInputString = DXString + "SelInput",
							DXColumnResizingInputString = DXString + "ColResizedInput",
							DXFocusedRowInputString = DXString + "FocusedRowInput",
							DXFooterRowString = DXString + "FooterRow",
							DXDataRowString = DXString + "DataRow",
							DXPreviewRowString = DXString + "PRow",
							DXDetailRowString = DXString + "DRow",
							DXGroupRowString = DXString + "GroupRow",
							DXGroupRowStringExpandedSuffix = "Exp",
							DXDataStyleTableString = DXString + "StyleTable",
							DXCustomizationWindowString = DragTargetString + "custwindow",
							DXFilterPopupString = DragTargetString + "filterpopup",
							CallbackHiddenFieldName = "CallbackState",
							DXHeaderRow = "DXHeadersRow",
							DXEditingRow = "DXEditingRow",
							DXEditingErrorRow = "DXEditingErrorRow",
							DXLoadingPanelContainer = "DXLPContainer",
							DXPopupEditForm = "DXPEForm",
							DXEditFormTable = DXString + "EFT",
							DXFilterRowMenu = DXString + "FilterRowMenu";
		StylesCache stylesCache;
		ASPxGridView grid;
		TemplateContainerCollection headerTemplates, rowCellTemplates, editRowCellTemplates, groupRowTemplates,
				dataRowTemplates, detailRowTemplates, previewRowTemplates, titleTemplates, statusBarTemplates,
				emptyDataRowTemplates, footerRowTemplates, footerCellTemplates, editFormTemplates, pagerBarTemplates;
		List<GridViewColumn> visibleColumns;
		int commandColumnsCount;
		List<ASPxEditBase> editorList;
		List<ASPxEditBase> dummyEditorList;
		List<ASPxEditBase> editingRowEditorList;
		Dictionary<GridViewColumn, EditPropertiesBase> columnEditors;
		Dictionary<GridViewColumn, string> validateErrors;
		string editingErrorText;
		ASPxGridViewTextBuilder textBuilder;
		ASPxGridViewScripts scripts;
		GridViewSEOProcessing seo;
		public ASPxGridViewRenderHelper(ASPxGridView grid) {
			this.grid = grid;
			this.stylesCache = new StylesCache();
			this.textBuilder = CreateTextBuilder();
			this.editorList = new List<ASPxEditBase>();
			this.dummyEditorList = new List<ASPxEditBase>();
			this.editingRowEditorList = new List<ASPxEditBase>();
			this.validateErrors = new Dictionary<GridViewColumn, string>();
			this.columnEditors = new Dictionary<GridViewColumn, EditPropertiesBase>();
			this.headerTemplates = new TemplateContainerCollection(Grid);
			this.rowCellTemplates = new TemplateContainerCollection(Grid);
			this.editRowCellTemplates = new TemplateContainerCollection(Grid);
			this.groupRowTemplates = new TemplateContainerCollection(Grid);
			this.detailRowTemplates = new TemplateContainerCollection(Grid);
			this.previewRowTemplates = new TemplateContainerCollection(Grid);
			this.dataRowTemplates = new TemplateContainerCollection(Grid);
			this.emptyDataRowTemplates = new TemplateContainerCollection(Grid);
			this.footerRowTemplates = new TemplateContainerCollection(Grid);
			this.footerCellTemplates = new TemplateContainerCollection(Grid);
			this.titleTemplates = new TemplateContainerCollection(Grid);
			this.statusBarTemplates = new TemplateContainerCollection(Grid);
			this.pagerBarTemplates = new TemplateContainerCollection(Grid);
			this.editFormTemplates = new TemplateContainerCollection(Grid);
			this.visibleColumns = new List<GridViewColumn>();
			this.commandColumnsCount = -1;
			this.scripts = new ASPxGridViewScripts(Grid);
			this.seo = new GridViewSEOProcessing(Grid);
		}
		public TemplateContainerCollection HeaderTemplates { get { return headerTemplates; } }
		public TemplateContainerCollection RowCellTemplates { get { return rowCellTemplates; } }
		public TemplateContainerCollection EditRowCellTemplates { get { return editRowCellTemplates; } }
		public TemplateContainerCollection GroupRowTemplates { get { return groupRowTemplates; } }
		public TemplateContainerCollection DetailRowTemplates { get { return detailRowTemplates; } }
		public TemplateContainerCollection PreviewRowTemplates { get { return previewRowTemplates; } }
		public TemplateContainerCollection DataRowTemplates { get { return dataRowTemplates; } }
		public TemplateContainerCollection EmptyDataRowTemplates { get { return emptyDataRowTemplates; } }
		public TemplateContainerCollection FooterRowTemplates { get { return footerRowTemplates; } }
		public TemplateContainerCollection FooterCellTemplates { get { return footerCellTemplates; } }
		public TemplateContainerCollection TitleTemplates { get { return titleTemplates; } }
		public TemplateContainerCollection StatusBarTemplates { get { return statusBarTemplates; } }
		public TemplateContainerCollection PagerBarTemplates { get { return pagerBarTemplates; } }
		public TemplateContainerCollection EditFormTemplates { get { return editFormTemplates; } }
		public ASPxGridView Grid { get { return grid; } }
		public ASPxGridViewScripts Scripts { get { return scripts; } }
		public GridViewImages Images { get { return Grid.Images; } }
		public GridViewStyles Styles { get { return Grid.Styles; } }
		public WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		public ASPxGridViewTextBuilder TextBuilder { get { return textBuilder; } }
		public GridViewSEOProcessing SEO { get { return seo; } }
		public bool IsGridEnabled { get { return Grid.IsEnabled(); } }
		public string ClientID { get { return grid.ClientID; } }
		public string MainTableID { get { return DXMainTableString; } }
		public string HeaderTableID { get { return DXHeaderTableString; } }
		public string FooterTableID { get { return DXFooterTableString; } }
		public string ScrollDivID { get { return DXScrollDivString; } }
		public string HorzScrollDivID { get { return DXHorzScrollDivString; } }
		public string FixedColumnsDivID { get { return DXFixedColumnsDivString; } }
		public string FixedColumnsContentDivID { get { return DXFixedColumnsContentDivString; } }
		public string GetSEOID() { return "seo" + Grid.ClientID; }
		public string GetColumnId(GridViewColumn column, GridViewHeaderLocation location) {
			string options = string.Empty;
			GridViewDataColumn dc = column as GridViewDataColumn;
			if(column.GetAllowGroup()) options += "G";
			if(column.GetAllowSort() || (dc != null && dc.GroupIndex > -1 && column.GetAllowGroup()) && location != GridViewHeaderLocation.Customization) options += "S";
			return GetHeadersTargetString(column.GetAllowDragDrop()) + (location == GridViewHeaderLocation.Group ? "group" : "") + options + "col" + column.Index.ToString();
		}
		public string GetColumnHeaderContentId(GridViewColumn column, GridViewHeaderLocation location) {
			return "content" + GetColumnId(column, location);
		}
		public string GetVisibleIndexString(int visibleIndex) {
			if(visibleIndex >= 0) return visibleIndex.ToString();
			if(visibleIndex == ListSourceDataController.FilterRow) return "frow";
			return "new";
		}
		public string GetColumnFiltedEditId(GridViewColumn column) {
			return DXFilterRowEditorString + column.Index.ToString();
		}
		public string GetSelectButtonId(int visibleIndex) {
			return DXSelectButtonString + GetVisibleIndexString(visibleIndex);
		}
		public string GetGroupPanelId() {
			return GetHeadersTargetString(true) + "grouppanel";
		}
		public string GetCustomizationWindowId() {
			return DXCustomizationWindowString;
		}
		public string GetFilterPopupId() {
			return DXFilterPopupString;
		}
		public string GetFooterRowId() {
			return DXFooterRowString;
		}
		public string GetPopupEditFormId() {
			return DXPopupEditForm;
		}
		public string GetParentRowsId() {
			return DXString + "parentrow";
		}
		public string GetParentRowsWindowId() {
			return DXString + "parentrowswindow";
		}
		public string GetEmptyHeaderId() {
			return GetHeadersContainerTargetString() + "emptyheader";
		}
		public string GetTopPagerId() {
			return DXString + "PagerTop";
		}
		public string GetBottomPagerId() {
			return DXString + "PagerBottom";
		}
		public string GetDetailRowId(int visibleIndex) {
			return DXDetailRowString + GetVisibleIndexString(visibleIndex);
		}
		public string GetRowId(int visibleIndex) {
			return DataProxy.GetRowType(visibleIndex) == WebRowType.Data ? GetDataRowId(visibleIndex) : GetGroupRowId(visibleIndex);
		}
		public string GetDataRowId(int visibleIndex) {
			return DXDataRowString + GetVisibleIndexString(visibleIndex);
		}
		public string GetPreviewRowId(int visibleIndex) {
			return DXPreviewRowString + GetVisibleIndexString(visibleIndex);
		}
		public string GetGroupRowId(int visibleIndex) {
			string prefix = DataProxy.IsRowExpanded(visibleIndex) 
				? DXGroupRowString + DXGroupRowStringExpandedSuffix
				: DXGroupRowString;
			return prefix + GetVisibleIndexString(visibleIndex);
		}
		public string GetEditorId(GridViewDataColumn column) {
			return DXEditorString + column.Index.ToString();
		}
		public string GetStyleTableId() {
			return DXDataStyleTableString;
		}
		public string GetLoadingContainerID() {
			return DXLoadingPanelContainer;
		}
		public string GetHeadersRowId() {
			return DXHeaderRow;
		}
		public string GetEditingRowId() {
			return DXEditingRow;
		}
		public string GetEditingErrorRowId() {
			return DXEditingErrorRow;
		}
		protected virtual ASPxGridViewTextBuilder CreateTextBuilder() {
			return new ASPxGridViewTextBuilder(Grid);
		}
		protected StylesCache StylesCache { get { return stylesCache; } }
		string GetHeadersContainerTargetString() { return GetHeadersTargetString(Grid.SettingsBehavior.AllowDragDrop); }
		string GetHeadersTargetString(bool allowDragDrop) { return allowDragDrop ? DragTargetString : DXTargetString; }
		public Control GenerateSelectInput() {
			HiddenField input = RenderUtils.CreateHiddenField(DXSelectedInputString);
			if(DataProxy.Selection.Count == 0) return input;
			input.Value = GetSelectInputValue();
			return input;
		}
		protected internal string GetSelectInputValue() {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				sb.Append(DataProxy.Selection.IsRowSelected(DataProxy.VisibleStartIndex + i) ? 'T' : 'F');
			}
			string result = sb.ToString();
			int n = result.LastIndexOf("T");
			result = n < 0 ? string.Empty : result.Substring(0, n + 1);
			return result;
		}
		public Control GenereateColumnResizingInput() {
			return RenderUtils.CreateHiddenField(DXColumnResizingInputString);
		}
		public Control GenerateFocusedRowInput() {
			return RenderUtils.CreateHiddenField(DXFocusedRowInputString);
		}
		public void ClearControlHierarchy() {
			this.groupCount = -1;
			this.visibleColumns.Clear();
			this.commandColumnsCount = -1;
			ClearStylesCache();
			ColumnEditors.Clear();
			EditorList.Clear();
			EditingRowEditorList.Clear();
			DetailRowTemplates.Clear();
			PreviewRowTemplates.Clear();
			HeaderTemplates.Clear();
			EditRowCellTemplates.Clear();
			RowCellTemplates.Clear();
			GroupRowTemplates.Clear();
			DataRowTemplates.Clear();
			EmptyDataRowTemplates.Clear();
			FooterRowTemplates.Clear();
			FooterCellTemplates.Clear();
			TitleTemplates.Clear();
			StatusBarTemplates.Clear();
			PagerBarTemplates.Clear();
			EditFormTemplates.Clear();
		}
		public List<GridViewColumn> VisibleColumns {
			get {
				if(visibleColumns.Count == 0) CreateVisibleColumns();
				return visibleColumns;
			}
		}
		public bool HasFixedColumns { 
			get { 
				return Grid.FixedColumnCount > 0 && ShowHorizontalScrolling
					&& (GroupCount == 0) && !Grid.Settings.ShowPreview 
					&& !HasDetailRows && (Grid.Templates.DataRow == null); 
			} 
		}
		public bool Is100PercentageMainTableWidthRequired(GridViewHtmlTableRenderPart renderPart) {
			if (!RenderUtils.IsIE) {
				return renderPart != GridViewHtmlTableRenderPart.All || !HasFixedColumns;
			}
			return renderPart != GridViewHtmlTableRenderPart.Content  && !HasFixedColumns;
		}
		public int CommandColumnsCount {
			get {
				if(commandColumnsCount < 0) {
					commandColumnsCount = 0;
					foreach(GridViewColumn column in VisibleColumns) {
						if(column is GridViewCommandColumn) {
							commandColumnsCount++;
						}
					}
				}
				return commandColumnsCount;
			}
		}
		void CreateVisibleColumns() {
			this.visibleColumns.AddRange(Grid.GetColumnsShownInHeaders());
		}
		protected Dictionary<GridViewColumn, EditPropertiesBase> ColumnEditors { get { return columnEditors; } }
		public List<ASPxEditBase> EditorList { get { return editorList; } }
		public List<ASPxEditBase> DummyEditorList { get { return dummyEditorList; } }
		public List<ASPxEditBase> EditingRowEditorList { get { return editingRowEditorList; } }
		public Dictionary<GridViewColumn, string> ValidationError { get { return validateErrors; } }
		public bool HasEditingError { get { return !string.IsNullOrEmpty(EditingErrorText); } }
		public string EditingErrorText {
			get { return editingErrorText; }
			set { editingErrorText = value; }
		}
		public void ResetEditingErrorText() {
			this.editingErrorText = string.Empty;
		}
		public int VisibleColumnCount {
			get { return VisibleColumns.Count > 0 ? VisibleColumns.Count : 1; }
		}
		public int IndentColumnCount { get { return GroupCount + (HasDetailButton ? 1 : 0); } }
		public int ColumnSpanCount { get { return VisibleColumnCount + IndentColumnCount; } }
		int groupCount = -1;
		public int GroupCount {
			get {
				if(groupCount == -1) {
					groupCount = Grid.GroupCount;
				}
				return groupCount;
			}
		}
		public GridViewCommandColumnButton GetVisibleNewRowButton() {
			foreach(GridViewColumn column in Grid.Columns) {
				GridViewCommandColumn colColumn = column as GridViewCommandColumn;
				if(colColumn == null) continue;
				if(colColumn.NewButton.Visible) return colColumn.NewButton;
			}
			return null;
		}
		public bool RequireRenderFilterPopupWindow {
			get {
				if(Grid.IsCallback) return false; 
				if(Grid.Settings.ShowHeaderFilterButton) return true; 
				for(int i = 0; i < VisibleColumns.Count; i++) {
					if(VisibleColumns[i].GetHasFilterButton()) return true;
				}
				return false;
			}
		}
		public bool RemoveCellRightBorder { get { return !ShowVerticalScrolling && !HasFixedColumns; } }
		public bool ShowVerticalScrolling { get { return Grid.Settings.ShowVerticalScrollBar; } }
		public bool IsVirtualScrolling { get { return ShowVerticalScrolling && Grid.Settings.VerticalScrollBarStyle == GridViewVerticalScrollBarStyle.Virtual; } }
		public int VirtualScrollRowHeight { get { return 20; } }
		public int VirtualScrollMarginTop { get { return DataProxy.VisibleStartIndex * VirtualScrollRowHeight; } }
		public int VirtualScrollMarginBottom { get { return (DataProxy.VisibleRowCount -  DataProxy.VisibleStartIndex - DataProxy.VisibleRowCountOnPage) * VirtualScrollRowHeight; } }
		public bool ShowHorizontalScrolling { get { return Grid.Settings.ShowHorizontalScrollBar; } }
		public bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		public Unit GetMainTableWidth(bool isScrollableTable) {
			if(ShowVerticalScrolling && !isScrollableTable) return Unit.Percentage(100);
			return !ShowHorizontalScrolling ? Unit.Percentage(100) : Unit.Empty;  
		}
		protected bool RequireRenderPagerControl { get { return Grid.SettingsPager.Mode == GridViewPagerMode.ShowPager && Grid.SettingsPager.Visible && (Grid.DesignMode || Grid.PageCount > 1 || Grid.SettingsPager.AlwaysShowPager); } }
		public bool RequireRenderTopPagerControl { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Bottom; } }
		public bool RequireRenderBottomPagerControl { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Top; } }
		public bool RequireHeaderTopBorder { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Bottom; } }
		public bool RequireFixedTableLayout { get { return Grid.Settings.UseFixedTableLayout || RequireRenderColGroups; } }
		public bool RequireRenderColGroups { get { return ShowVerticalScrolling || AllowColumnResizing || HasFixedColumns; } }
		public bool AllowColumnResizing { get { return Grid.SettingsBehavior.ColumnResizeMode != ColumnResizeMode.Disabled; } }
		public bool RequireTablesHelperScripts { get { return AllowColumnResizing || ShowVerticalScrolling || ShowHorizontalScrolling; } }
		public bool RequireRenderCustomizationWindow { get { return Grid.SettingsCustomizationWindow.Enabled; } }
		public bool RequireRenderFilterRowMenu { 
			get {
				if(!Grid.Settings.ShowFilterRow) return false;
				foreach(GridViewDataColumn column in Grid.DataColumns) {
					if(IsFilterRowMenuIconVisible(column))
						return true;
				}
				return false;
			} 
		}
		public bool IsRemoveBorderFromMainTableLastRow {
			get { return IsRemoveBorderFromMainTableLastNewItemRow && !RequireRenderNewItemRowAtBottom; }
		}
		public bool IsRemoveBorderFromMainTableLastNewItemRow {
			get { return !RequireRenderBottomPagerControl && !ShowVerticalScrolling && !ShowFooter; }
		}
		protected bool RequireRenderNewItemRowAtBottom {
			get { return DataProxy.IsNewRowEditing && Grid.SettingsEditing.NewItemRowPosition == GridViewNewItemRowPosition.Bottom; } 
		}
		public bool HasEmptyDataRow { 
			get { 
				return DataProxy.VisibleRowCountOnPage == 0 && !DataProxy.IsEditing && 
					(DataProxy.IsReady || Grid.DataSource == null); 
			} 
		}
		public int EmptyPagerDataRowCount {
			get {
				if (!Grid.SettingsPager.ShowEmptyDataRows || Grid.SettingsPager.Mode == GridViewPagerMode.ShowAllRecords) return 0;
				int result = Grid.SettingsPager.PageSize - DataProxy.VisibleRowCountOnPage;
				if (HasEmptyDataRow) result--;
				if (Grid.IsNewRowEditing) result--;
				return result; 
			}
		}
		public bool HasDetailRows { get { return Grid.SettingsDetail.ShowDetailRow; } }
		public bool HasDetailButton { get { return HasDetailRows && Grid.SettingsDetail.ShowDetailButtons; } }
		public bool HasDetailRow(int visibleIndex) {
			return HasDetailRows && DataProxy.DetailRows.IsVisible(visibleIndex);
		}
		public bool HasGroupRowFooter(int visibleIndex) { return GetGroupFooterVisibleIndexes(visibleIndex) != null; } 
		public List<int> GetGroupFooterVisibleIndexes(int visibleIndex) {
			if(Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.Hidden) return null;
			return DataProxy.GetGroupFooterVisibleIndexes(visibleIndex, Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.VisibleIfExpanded);
		}
		public void CreateGridDummyEditors(Control parent) {
			for(int i = 0; i < Grid.Columns.Count; i++) {
				if(!(Grid.Columns[i] is GridViewDataColumn)) continue;
				ASPxEditBase editor = CreateGridEditor((GridViewDataColumn)Grid.Columns[i], null, EditorInplaceMode.Inplace, true);
				parent.Controls.Add(editor);
				DummyEditorList.Add(editor);
			}
		}
		private bool GridWidthSpecifiedInPixels {
			get { return !Grid.Width.IsEmpty && Grid.Width.Type != UnitType.Percentage; }
		}
		private bool InplaceAllowEditorSizeRecalc {
			get { return GridWidthSpecifiedInPixels || AllowColumnResizing; }
		}
		public ASPxEditBase CreateGridEditor(GridViewDataColumn column, object value, EditorInplaceMode mode, bool isInternal) {
			CreateEditControlArgs args = new CreateEditControlArgs(value, column.GetDataType(),
				Grid.ImagesEditors, Grid.StylesEditors, Grid, mode, InplaceAllowEditorSizeRecalc);
			ASPxEditBase baseEditor = GetColumnEdit(column).CreateEdit(args);
			baseEditor.ID = GetEditorId(column);
			baseEditor.EnableClientSideAPI = true;
			baseEditor.EnableViewState = false;
			if(!isInternal) {
				EditorList.Add(baseEditor);
				EditingRowEditorList.Add(baseEditor);
			}
			return baseEditor;
		}
		public void ApplyEditorSettings(ASPxEditBase baseEditor, GridViewDataColumn column) {
			ASPxEdit editor = baseEditor as ASPxEdit;
			if(editor != null) {
				bool hasError = ValidationError.ContainsKey(column);
				editor.IsValid = !hasError;
				editor.ValidationSettings.ErrorText = hasError ? ValidationError[column] : string.Empty;
			}
			baseEditor.ReadOnly = Grid.IsReadOnly(column);
		}
		public ASPxEditBase CreateAutoFilterEditor(TableCell cell, GridViewDataColumn column, object value, EditorInplaceMode mode) {
			EditPropertiesBase properties = GetColumnAutoFilterEdit(column);
			AutoFilterCondition condition = Grid.FilterHelper.GetColumnAutoFilterCondition(column);
			CreateEditControlArgs args = new CreateEditControlArgs(value, column.GetDataType(),
				Grid.ImagesEditors, Grid.StylesEditors, Grid, mode, InplaceAllowEditorSizeRecalc);
			if(condition == AutoFilterCondition.Equals && value != null && !string.IsNullOrEmpty(value.ToString())) {
				args.EditValue = ConvertFilterEditorValue(column, value); 
			}
			ASPxGridViewEditorCreateEventArgs ei = new ASPxGridViewEditorCreateEventArgs(column, -1, properties, null, args.EditValue);
			Grid.RaiseAutoFilterEditorCreate(ei);
			args.EditValue = ei.Value;
			ASPxEditBase editor = ei.EditorProperties.CreateEdit(args);
			ASPxEditorValidationDisabler.DisableValidation(editor);
			SetupFilterEditor(editor);
			editor.ID = GetColumnFiltedEditId(column);
			editor.EnableClientSideAPI = true;
			editor.EnableViewState = false;
			if(Grid.DesignMode) {
				editor.DataSource = null;
				editor.DataSourceID = "";
			}
			cell.Controls.Add(editor);
			editor.DataBind();	
			ASPxGridViewEditorEventArgs e = new ASPxGridViewEditorEventArgs(column, -1, editor, null, editor.Value);
			Grid.RaiseAutoFilterEditorInitialize(e);
			EditorList.Add(e.Editor);
			return e.Editor;
		}
		object ConvertFilterEditorValue(GridViewDataColumn column, object value) {
			GridViewDataComboBoxColumn comboColumn = column as GridViewDataComboBoxColumn;
			if(comboColumn != null) {
				PropertyDescriptor descriptor = new WebPropertyDescriptor(column.FieldName, "", comboColumn.PropertiesComboBox.ValueType, false, false);
				return new DataColumnInfo(descriptor).ConvertValue(value);
			}
			return DataProxy.ConvertValue(column.FieldName, value);
		}
		public bool IsFilterRowMenuIconVisible(GridViewColumn column) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(dataColumn == null) return false;
			if(dataColumn.Settings.ShowFilterRowMenu == DefaultBoolean.True) return true;
			if(dataColumn.Settings.ShowFilterRowMenu == DefaultBoolean.False) return false;
			return Grid.Settings.ShowFilterRowMenu;
		}
		protected virtual void SetupFilterEditor(ASPxEditBase editor) {
			ASPxDateEdit date = editor as ASPxDateEdit;
			if(date != null) date.CalendarProperties.ShowClearButton = true;
		}
		public GridViewColumnEditKind GetColumnAutoFilterEditKind(GridViewDataColumn column) {
			EditPropertiesBase res = GetColumnEdit(column);
			if(res is CheckBoxProperties) return GridViewColumnEditKind.CheckBox;
			if(Grid.FilterHelper.GetColumnFilterMode(column, false) != ColumnFilterMode.DisplayText) {
				if(res is DateEditProperties) return GridViewColumnEditKind.DateEdit;
				if(res is ComboBoxProperties) return GridViewColumnEditKind.ComboBox;
			} else {
				return GridViewColumnEditKind.Text;
			}
			return GridViewColumnEditKind.Text;
		}
		public EditPropertiesBase GetColumnAutoFilterEdit(GridViewDataColumn column) {
			EditPropertiesBase res = GetColumnEdit(column);
			if(res is CheckBoxProperties) {
				CheckBoxProperties check = res as CheckBoxProperties;
				ComboBoxProperties combo = new ComboBoxProperties();
				combo.ValueType = check.ValueType;
				combo.Items.Add(string.Empty, null);
				combo.Items.Add(ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Checked), check.ValueChecked);
				combo.Items.Add(ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Unchecked), check.ValueUnchecked);
				return combo;
			}
			if(Grid.FilterHelper.GetColumnFilterMode(column, false) != ColumnFilterMode.DisplayText) {
				if(res is DateEditProperties) return res;
				if(res is ComboBoxProperties) return res;
			} else {
				return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
			}
			return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
		}
		public virtual void AddDisplayControlToDataCell(Control cell, GridViewDataColumn column, int visibleIndex, IValueProvider row) {
			cell.Controls.Add(CreateDataCellDisplayControl(column, visibleIndex, row));
		}
		protected virtual Control CreateDataCellDisplayControl(GridViewDataColumn column, int visibleIndex, IValueProvider row) {
			return GetColumnEdit(column).CreateDisplayControl(TextBuilder.GetDisplayControlArgs(column, visibleIndex, row, Grid));			
		}
		public EditPropertiesBase GetColumnEdit(GridViewDataColumn column) {
			if(column.PropertiesEdit != null)
				return column.PropertiesEdit;
			if(!ColumnEditors.ContainsKey(column)) ColumnEditors[column] = EditRegistrationInfo.CreatePropertiesByDataType(DataProxy.GetFieldType(column.FieldName));
			return ColumnEditors[column];
		}
		public AppearanceStyle GetControlFontStyle() {
			AppearanceStyle controlStyle = new AppearanceStyle();
			controlStyle.CopyFontFrom(Grid.ControlStyle);
			return controlStyle;
		}
		public AppearanceStyle GetControlStyle() {
			AppearanceStyle controlStyle = new AppearanceStyle();
			controlStyle.CopyFrom(Grid.ControlStyle);
			return controlStyle;
		}
		public AppearanceStyle GetRootTableStyle() {
			return (AppearanceStyle)CreateStyle(new AppearanceStyle(),
				null, null, Styles.CreateStyleByName("Control"), GetControlStyle());
		}
		public AppearanceStyle GetDisabledRootTableStyle() {
			return (AppearanceStyle)CreateStyle(new AppearanceStyle(),
				null, null, Styles.CreateStyleByName("Disabled"), GetControlStyle(), Styles.Disabled);
		}
		public ImageProperties GetImage(string imageName) {
			return Images.GetImageProperties(Grid.Page, imageName);
		}
		public void AssignImageToControl(string imageName, Image image) {
			Images.GetImageProperties(Grid.Page, imageName).AssignToControl(image, Grid.DesignMode);
		}
		public ImageProperties GetHeaderSortImage(ColumnSortOrder sortOrder) {
			return sortOrder == ColumnSortOrder.Ascending ?
				GetImage(GridViewImages.HeaderSortUpName) : GetImage(GridViewImages.HeaderSortDownName);
		}
		public void PrepareLoadingPanel(LoadingPanelControl loadingPanel) {
			LoadingPanelStyle style = GetLoadingPanelStyle();
			loadingPanel.Style = style;
			loadingPanel.Paddings = style.Paddings;
			loadingPanel.ImageSpacing = style.ImageSpacing;
		}
		public void PrepareLoadingDiv(WebControl loadingDiv) {
			RenderUtils.SetVisibility(loadingDiv, false, true);
			LoadingDivStyle style = GetLoadingDivStyle();
			style.AssignToControl(loadingDiv);
			loadingDiv.Style.Add("z-index", RenderUtils.LoadingDivZIndex.ToString());
			loadingDiv.Style.Add("position", "absolute");
		}
		public virtual bool ShowVerticalGridLine { get { return Grid.Settings.GridLines == GridLines.Both || Grid.Settings.GridLines == GridLines.Vertical; } }
		public virtual bool ShowHorizontalGridLine { get { return Grid.Settings.GridLines == GridLines.Both || Grid.Settings.GridLines == GridLines.Horizontal; } }
		public void AppendDefaultDXClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, GridViewStyles.GridPrefix);
		}
		public void AppendIndentCellClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, GridViewStyles.GridPrefix + GridViewStyles.GridIndentCellPrefix);
		}
		public virtual AppearanceStyle GetGroupPanelStyle() {
			return (AppearanceStyle)CreateStyle(new GridViewGroupPanelStyle(), Styles.CreateStyleByName("GroupPanel"), Styles.GroupPanel);
		}
		public GridViewTableStyle GetTableStyle() {
			return (GridViewTableStyle)CreateStyle(new GridViewTableStyle(),
				null, null, Styles.CreateStyleByName("Table"), GetControlFontStyle(), Styles.Table);
		}
		public GridViewHeaderStyle GetHeaderStyle(GridViewColumn column) {
			return (GridViewHeaderStyle)CreateStyle(new GridViewHeaderStyle(), Styles.CreateStyleByName("Header"),
				GetControlFontStyle(), Styles.Header, column == null ? null : column.HeaderStyle);
		}
		public GridViewCustomizationStyle GetCustomizationStyle() {
			return (GridViewCustomizationStyle)CreateStyle(new GridViewCustomizationStyle(), Styles.CreateStyleByName("Customization"),
				Styles.Customization);
		}
		public GridViewPopupEditFormStyle GetPopupEditFormStyle() {
			return (GridViewPopupEditFormStyle)CreateStyle(new GridViewPopupEditFormStyle(), Styles.CreateStyleByName("PopupEditForm"),
				Styles.PopupEditForm);
		}
		public GridViewHeaderPanelStyle GetHeaderPanelStyle() {
			return (GridViewHeaderPanelStyle)CreateStyle(new GridViewHeaderPanelStyle(),
				Styles.CreateStyleByName("HeaderPanel"), GetControlFontStyle(), Styles.HeaderPanel);
		}
		public GridViewCellStyle GetPagerTopPanelStyle() {
			return (GridViewCellStyle)CreateStyle(new GridViewCellStyle(),
				Styles.CreateStyleByName("PagerTopPanel"), GetControlFontStyle(), Styles.PagerTopPanel);
		}
		public GridViewCellStyle GetPagerBottomPanelStyle() {
			return (GridViewCellStyle)CreateStyle(new GridViewCellStyle(),
				Styles.CreateStyleByName("PagerBottomPanel"), GetControlFontStyle(), Styles.PagerBottomPanel);
		}
		public GridViewGroupRowStyle GetGroupRowStyle() {
			GridViewGroupRowStyle res = StylesCache.GetStyle(GridViewStyleKinds.GroupRow) as GridViewGroupRowStyle;
			if(res == null) {
				res = (GridViewGroupRowStyle)CreateStyle(new GridViewGroupRowStyle(),
				Styles.CreateStyleByName("GroupRow"), GetControlFontStyle(), Styles.GroupRow);
				StylesCache.AddStyle(GridViewStyleKinds.GroupRow, res);
			}
			return res;
		}
		public GridViewGroupRowStyle GetFocusedGroupRowStyle() {
			return (GridViewGroupRowStyle)CreateStyle(new GridViewGroupRowStyle(),
				Styles.CreateStyleByName("FocusedGroupRow"), GetControlFontStyle(), Styles.GroupRow, Styles.FocusedGroupRow);
		}
		public GridViewDataRowStyle GetDataRowStyle(int visibleIndex, int dataRowIndex) {
			if(Grid.EditingRowVisibleIndex == visibleIndex) {
				return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(), GetControlFontStyle(),
					Styles.CreateStyleByName("EditFormDisplayRow"), Styles.Row, Styles.EditFormDisplayRow);
			}
			bool odd = dataRowIndex % 2 == 1;
			GridViewDataRowStyle dataRow = StylesCache.GetStyle(odd ? GridViewStyleKinds.DataRowOdd : GridViewStyleKinds.DataRowEven) as GridViewDataRowStyle;
			if(dataRow == null) {
				dataRow = GetDataRowStyleCore(odd);
				StylesCache.AddStyle(odd ? GridViewStyleKinds.DataRowOdd : GridViewStyleKinds.DataRowEven, dataRow);
			}
			return dataRow;
		}
		public GridViewCellStyle GetDetailButtonStyle() {
			return (GridViewCellStyle)CreateStyle(new GridViewCellStyle(),
				Styles.CreateStyleByName("DetailButton"), GetControlFontStyle(), Styles.DetailButton);
		}
		protected GridViewDataRowStyle GetDataRowStyleCore(bool odd) {
			GridViewDataRowStyle alternatingRowStyle = null;
			if(odd) {
				if(Styles.AlternatingRow.Enabled != DefaultBoolean.False && (
					!Styles.AlternatingRow.IsEmpty || Styles.AlternatingRow.Enabled == DefaultBoolean.True))
					alternatingRowStyle = (GridViewDataRowStyle)CreateStyle(new GridViewAlternatingRowStyle(), Styles.CreateStyleByName("DataRowAlt"), Styles.AlternatingRow);
			}
			return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(), GetControlFontStyle(),
				Styles.CreateStyleByName("DataRow"), Styles.Row, alternatingRowStyle);
		}
		public GridViewDataRowStyle GetDetailRowStyle() {
			return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(),
				Styles.CreateStyleByName("DetailRow"), GetControlFontStyle(), Styles.DetailRow);
		}
		public GridViewCellStyle GetDetailCellStyle() {
			return (GridViewCellStyle)CreateStyle(new GridViewCellStyle(),
				Styles.CreateStyleByName("DetailCell"), GetControlFontStyle(), Styles.DetailCell);
		}
		public GridViewDataRowStyle GetPreviewRowStyle() {
			GridViewDataRowStyle res = StylesCache.GetStyle(GridViewStyleKinds.Preview) as GridViewDataRowStyle;
			if(res == null) {
				res = (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(),
				Styles.CreateStyleByName("PreviewRow"), GetControlFontStyle(), Styles.PreviewRow);
				StylesCache.AddStyle(GridViewStyleKinds.Preview, res);
			}
			return res;
		}
		public GridViewDataRowStyle GetEmptyDataRowStyle() {
			return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(),
				Styles.CreateStyleByName("EmptyDataRow"), GetControlFontStyle(), Styles.EmptyDataRow);
		}
		public GridViewDataRowStyle GetSelectedRowStyle() {
			return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(),
				Styles.CreateStyleByName("SelectedRow"), GetControlFontStyle(), Styles.Row, Styles.SelectedRow);
		}
		public GridViewDataRowStyle GetFocusedRowStyle() {
			return (GridViewDataRowStyle)CreateStyle(new GridViewDataRowStyle(),
				Styles.CreateStyleByName("FocusedRow"), GetControlFontStyle(), Styles.Row, Styles.FocusedRow);
		}
		public GridViewRowStyle GetFilterRowStyle() {
			return (GridViewRowStyle)CreateStyle(new GridViewRowStyle(),
				Styles.CreateStyleByName("FilterRow"), GetControlFontStyle(), Styles.FilterRow);
		}
		public GridViewCellStyle GetCellStyle(GridViewColumn column, int visibleIndex) {
			GridViewCellStyle res = StylesCache.GetStyle(GridViewStyleKinds.DataCell, column) as GridViewCellStyle;
			if(res == null) {
				res = (GridViewCellStyle)ApplyDisplayControlAlignment(column, CreateStyle(new GridViewCellStyle(),
				Styles.CreateStyleByName("Cell"), Styles.Cell, column.CellStyle));
				StylesCache.AddStyle(GridViewStyleKinds.DataCell, column, res);
			}
			return res;
		}
		public GridViewFooterStyle GetFooterStyle() {
			return (GridViewFooterStyle)CreateStyle(new GridViewFooterStyle(),
				Styles.CreateStyleByName("Footer"), GetControlFontStyle(), Styles.Footer);
		}
		public GridViewFooterStyle GetFooterCellStyle(GridViewColumn column) {
			GridViewFooterStyle columnStyle = column != null ? column.FooterCellStyle : null;
			return (GridViewFooterStyle)ApplyDisplayControlAlignment(column, CreateStyle(new GridViewFooterStyle(),
				Styles.CreateStyleByName("FooterCell"), Styles.Footer, columnStyle));
		}
		public GridViewGroupFooterStyle GetGroupFooterStyle() {
			return (GridViewGroupFooterStyle)CreateStyle(new GridViewGroupFooterStyle(),
				Styles.CreateStyleByName("GroupFooter"), GetControlFontStyle(), Styles.GroupFooter);
		}
		public GridViewGroupFooterStyle GetGroupFooterCellStyle(GridViewColumn column) {
			GridViewGroupFooterStyle columnStyle = column != null ? column.GroupFooterCellStyle : null;
			return (GridViewGroupFooterStyle)ApplyDisplayControlAlignment(column, CreateStyle(new GridViewGroupFooterStyle(),
				Styles.CreateStyleByName("GroupFooterCell"), Styles.GroupFooter, columnStyle));
		}
		public AppearanceSelectedStyle GetHeaderHoverStyle(GridViewColumn column) {
			return (AppearanceSelectedStyle)CreateStyle(new AppearanceSelectedStyle(),
				GetHeaderStyle(column),
				Styles.CreateStyleByName("HeaderHover"), Styles.Header.HoverStyle,
				column.HeaderStyle.HoverStyle);
		}
		public LoadingDivStyle GetLoadingDivStyle() {
			return (LoadingDivStyle)CreateStyle(new LoadingDivStyle(),
					Styles.CreateStyleByName("LoadingDiv"), Styles.LoadingDiv);
		}
		public GridViewCommandColumnStyle GetCommandColumnStyle(GridViewColumn column) {
			GridViewCommandColumnStyle res = StylesCache.GetStyle(GridViewStyleKinds.RowCommandColumn, column) as GridViewCommandColumnStyle;
			if(res == null) {
				res = (GridViewCommandColumnStyle)CreateStyle(new GridViewCommandColumnStyle(),
					Styles.CreateStyleByName("CommandColumn"), Styles.CommandColumn, column.CellStyle);
				StylesCache.AddStyle(GridViewStyleKinds.RowCommandColumn, column, res);
			}
			return res;
		}
		public GridViewCommandColumnStyle GetCommandColumnItemStyle(GridViewColumn column) {
			GridViewCommandColumnStyle res = StylesCache.GetStyle(GridViewStyleKinds.RowCommandColumnItem, column) as GridViewCommandColumnStyle;
			if(res == null) {
				res = (GridViewCommandColumnStyle)CreateStyle(new GridViewCommandColumnStyle(),
					Styles.CreateStyleByName("CommandColumnItem"), Styles.CommandColumnItem);
				StylesCache.AddStyle(GridViewStyleKinds.RowCommandColumnItem, column, res);
			}
			return res;
		}
		public GridViewCommandColumnStyle GetUpdateCancelButtonsStyle() {
			return (GridViewCommandColumnStyle)CreateStyle(new GridViewCommandColumnStyle(),
					Styles.CreateStyleByName("CommandColumn"), Styles.CommandColumn);
		}
		public GridViewEditCellStyle GetEditFormCellStyle(GridViewDataColumn column) {
			return (GridViewEditCellStyle)CreateStyle(new GridViewEditCellStyle(),
					Styles.CreateStyleByName("EditFormCell"), Styles.EditFormCell, column.EditCellStyle);
		}
		public GridViewEditCellStyle GetInlineEditCellStyle(GridViewDataColumn column) {
			return (GridViewEditCellStyle)ApplyDisplayControlAlignment(column, CreateStyle(new GridViewEditCellStyle(),
					Styles.CreateStyleByName("InlineEditCell"), Styles.InlineEditCell, column.EditCellStyle));
		}
		public GridViewRowStyle GetEditingErrorRowStyle() {
			return (GridViewRowStyle)CreateStyle(new GridViewRowStyle(),
					Styles.CreateStyleByName("EditingErrorRow"), Styles.EditingErrorRow);
		}
		public GridViewFilterCellStyle GetFilterCellStyle(GridViewDataColumn column) {
			return (GridViewFilterCellStyle)ApplyDisplayControlAlignment(column, CreateStyle(new GridViewFilterCellStyle(),
					Styles.CreateStyleByName("FilterCell"), Styles.FilterCell, column.FilterCellStyle));
		}
		public GridViewInlineEditRowStyle GetInlineEditRowStyle() {
			return (GridViewInlineEditRowStyle)CreateStyle(new GridViewInlineEditRowStyle(),
					Styles.CreateStyleByName("InlineEditRow"), Styles.InlineEditRow);
		}
		public GridViewEditFormStyle GetEditFormRowStyle() {
			return (GridViewEditFormStyle)CreateStyle(new GridViewEditFormStyle(),
					Styles.CreateStyleByName("EditForm"), GetControlFontStyle(), Styles.Row, Styles.EditForm);
		}
		public GridViewEditFormTableStyle GetEditFormStyle() {
			return (GridViewEditFormTableStyle)CreateStyle(new GridViewEditFormTableStyle(),
					Styles.CreateStyleByName("EditFormTable"), GetControlFontStyle(), Styles.EditFormTable);
		}
		public GridViewEditFormCaptionStyle GetEditFormCaptionStyle(GridViewDataColumn column) {
			return (GridViewEditFormCaptionStyle)CreateStyle(new GridViewEditFormCaptionStyle(),
					Styles.CreateStyleByName("EditFormCaption"), Styles.EditFormColumnCaption, column.EditFormCaptionStyle);
		}
		public GridViewTitleStyle GetTitleStyle() {
			return (GridViewTitleStyle)CreateStyle(new GridViewTitleStyle(),
					Styles.CreateStyleByName("TitlePanel"), GetControlFontStyle(), Styles.TitlePanel);
		}
		public GridViewTitleStyle GetStatusBarStyle() {
			return (GridViewTitleStyle)CreateStyle(new GridViewTitleStyle(),
					Styles.CreateStyleByName("StatusBar"), GetControlFontStyle(), Styles.StatusBar);
		}
		public GridViewFilterBarStyle GetFilterBarStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(),
					Styles.CreateStyleByName(GridViewStyles.FilterBarStyleName), GetControlFontStyle(), Styles.FilterBar);
		}
		public GridViewFilterBarStyle GetFilterBarLinkStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(), GetControlFontStyle(), Styles.FilterBarLink); 
		}
		public GridViewFilterBarStyle GetFilterBarCheckBoxCellStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(),
					Styles.CreateStyleByName(GridViewStyles.FilterBarCheckBoxCellStyleName), GetControlFontStyle(), Styles.FilterBarCheckBoxCell);
		}
		public GridViewFilterBarStyle GetFilterBarImageCellStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(),
					Styles.CreateStyleByName(GridViewStyles.FilterBarImageCellStyleName), GetControlFontStyle(), Styles.FilterBarImageCell);
		}
		public GridViewFilterBarStyle GetFilterBarExpressionCellStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(),
					Styles.CreateStyleByName(GridViewStyles.FilterBarExpressionCellStyleName), GetControlFontStyle(), Styles.FilterBarExpressionCell);
		}
		public GridViewFilterBarStyle GetFilterBarClearButtonCellStyle() {
			return (GridViewFilterBarStyle)CreateStyle(new GridViewFilterBarStyle(),
					Styles.CreateStyleByName(GridViewStyles.FilterBarClearButtonCellStyleName), GetControlFontStyle(), Styles.FilterBarClearButtonCell);
		}
		public AppearanceStyle GetFilterBuilderMainAreaStyle() {
			return (AppearanceStyle)CreateStyle(new AppearanceStyle(),
				Styles.CreateStyleByName(GridViewStyles.FilterBuilderMainAreaStyleName), Styles.FilterBuilderMainArea);
		}
		public AppearanceStyle GetFilterBuilderButtonAreaStyle() {
			return (AppearanceStyle)CreateStyle(new AppearanceStyle(),
				Styles.CreateStyleByName(GridViewStyles.FilterBuilderButtonAreaStyleName), Styles.FilterBuilderButtonArea);
		}
		public GridViewFilterStyle GetFilterPopupWindowStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupWindow"), GetControlFontStyle(), Styles.FilterPopupWindow);
		}
		public GridViewFilterStyle GetFilterPopupItemsAreaStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupItemsArea"), GetControlFontStyle(), Styles.FilterPopupItemsArea);
		}
		public GridViewFilterStyle GetFilterPopupButtonPanelStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupButtonPanel"), GetControlFontStyle(), Styles.FilterPopupButtonPanel);
		}
		public GridViewFilterStyle GetFilterPopupItemStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupItem"), GetControlFontStyle(), Styles.FilterPopupItem);
		}
		public GridViewFilterStyle GetFilterPopupActiveItemStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupActiveItem"), GetControlFontStyle(), Styles.FilterPopupActiveItem);
		}
		public GridViewFilterStyle GetFilterPopupSelectedItemStyle() {
			return (GridViewFilterStyle)CreateStyle(new GridViewFilterStyle(),
				Styles.CreateStyleByName("FilterPopupSelectedItem"), GetControlFontStyle(), Styles.FilterPopupSelectedItem);
		}
		protected virtual LoadingPanelStyle GetLoadingPanelStyle() {
			string styleName = Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar ?
				"LoadingPanelStatusBar" : "LoadingPanel";
			return (LoadingPanelStyle)CreateStyle(new LoadingPanelStyle(),
					Styles.CreateStyleByName(styleName), Styles.LoadingPanel);
		}
		protected virtual AppearanceStyleBase ApplyDisplayControlAlignment(GridViewColumn column, AppearanceStyleBase style) {
			if(style.HorizontalAlign != HorizontalAlign.NotSet) return style;
			style.HorizontalAlign = GetColumnDisplayControlDefaultAlignment(column);
			return style;
		}
		public virtual HorizontalAlign GetColumnDisplayControlDefaultAlignment(GridViewColumn column) {
			GridViewDataColumn dc = column as GridViewDataColumn;
			if(dc == null) return HorizontalAlign.NotSet;
			EditPropertiesBase edit = GetColumnEdit(dc);
			HorizontalAlign align = edit == null ? HorizontalAlign.NotSet : edit.GetDisplayControlDefaultAlign();
			if(align == HorizontalAlign.NotSet) align = dc.GetDisplayControlDefaultAlign();
			return align;
		}
		protected internal AppearanceStyleBase CreateStyle(AppearanceStyleBase style, params AppearanceStyleBase[] styles) {
			return RenderUtils.CreateStyle(style, styles);
		}
		protected internal virtual TableCell CreateContentCell(GridViewTableDataRow row, GridViewColumn column, int index, int visibleRowIndex, int columnCount) {
			if(column is GridViewDataColumn) return new GridViewTableDataCell(this, row, Grid.DataProxy, column as GridViewDataColumn, visibleRowIndex, index == 0 && Grid.GroupCount > 0, index == columnCount - 1);
			if(column is GridViewCommandColumn) return new GridViewTableCommandCell(this, Grid.DataProxy, column as GridViewCommandColumn, visibleRowIndex, index == 0 && Grid.GroupCount > 0, index == columnCount - 1);
			return RenderUtils.CreateTableCell();
		}
		protected internal virtual TableCell CreateInlineEditorCell(GridViewColumn column, int index, int visibleRowIndex, int columnCount) {
			if(column is GridViewCommandColumn) return new GridViewTableCommandCell(this, Grid.DataProxy, column as GridViewCommandColumn, visibleRowIndex, index == 0 && Grid.GroupCount > 0, index == columnCount - 1);
			if(column is GridViewDataColumn) {
				return new GridViewTableInlineEditorCell(this, Grid.DataProxy, column as GridViewDataColumn, visibleRowIndex, index == 0 && Grid.GroupCount > 0, index == columnCount - 1);
			}
			return RenderUtils.CreateTableCell();
		}
		public bool AddHeaderTemplateControl(GridViewColumn column, Control templateContainer, GridViewHeaderLocation headerLocation) {
			ITemplate template = GetTemplate(Grid.Templates.Header, column.HeaderTemplate);
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewHeaderTemplateContainer(column, headerLocation), HeaderTemplates);
			return true;
		}
		public bool AddHeaderCaptionTemplateControl(GridViewColumn column, Control templateContainer, GridViewHeaderLocation headerLocation) {
			ITemplate template = GetTemplate(Grid.Templates.HeaderCaption, column.HeaderCaptionTemplate);
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewHeaderTemplateContainer(column, headerLocation), HeaderTemplates);
			return true;
		}
		public bool AddDataItemTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.DataItem, column.DataItemTemplate);
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewDataItemTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex, column), RowCellTemplates);
			return true;
		}
		public bool AddEditItemTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = column.EditItemTemplate;
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewEditItemTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex, column), EditRowCellTemplates);
			return true;
		}
		public bool AddGroupRowContentTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = Grid.Templates.GroupRowContent;
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewGroupRowTemplateContainer(Grid, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupRowTemplates);
			return true;
		}
		public bool AddGroupRowTemplateControl(int visibleIndex, GridViewDataColumn column, TableRow row, int spanCount) {
			ITemplate template = GetTemplate(Grid.Templates.GroupRow, column.GroupRowTemplate);
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewGroupRowTemplateContainer(Grid, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupRowTemplates);
			return true;
		}
		public bool AddPreviewRowTemplateControl(int visibleIndex, GridViewTablePreviewRow row, int spanCount) {
			ITemplate template = Grid.Templates.PreviewRow;
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewPreviewRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), PreviewRowTemplates);
			return true;
		}
		public bool AddDetailRowTemplateControl(int visibleIndex, GridViewTableDetailRow row, int spanCount) {
			ITemplate template = Grid.Templates.DetailRow;
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewDetailRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), DetailRowTemplates);
			return true;
		}
		protected TableCell CreateTemplateCell(TableRow row, int spanCount) {
			TableCell cell = new GridTemplateTableCell();
			cell.ColumnSpan = spanCount;
			row.Cells.Add(cell);
			AppendDefaultDXClassName(cell);
			return cell;
		}
		public bool AddDataRowTemplateControl(int visibleIndex, TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.DataRow;
			if(template == null) return false;
			GridViewTableCellEx templateContainer = new GridViewTableCellEx(this, true);
			templateContainer.ColumnSpan = spanCount;
			row.Cells.Add(templateContainer);
			GridViewDataRowTemplateContainer cont = new GridViewDataRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex);
			AddTemplateToControl(templateContainer, template, cont, DataRowTemplates);
			return true;
		}
		public bool AddFooterRowTemplateControl(TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.FooterRow;
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewFooterRowTemplateContainer(Grid), FooterRowTemplates);
			return true;
		}
		public bool AddFooterCellTemplateControl(GridViewColumn column, Control templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.FooterCell, column.FooterTemplate);
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewFooterCellTemplateContainer(column), FooterCellTemplates);
			return true;
		}
		public bool AddEmptyDataRowTemplateControl(TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.EmptyDataRow;
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewEmptyDataRowTemplateContainer(Grid), EmptyDataRowTemplates);
			return true;
		}
		public bool AddTitleTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.TitlePanel;
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewTitleTemplateContainer(Grid), TitleTemplates);
			return true;
		}
		public bool AddStatusBarTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.StatusBar;
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewStatusBarTemplateContainer(Grid), StatusBarTemplates);
			return true;
		}
		public bool AddPagerBarTemplateControl(WebControl templateContainer, GridViewPagerBarPosition position, string pagerId) {
			ITemplate template = Grid.Templates.PagerBar;
			if (template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewPagerBarTemplateContainer(Grid, position, pagerId), PagerBarTemplates);
			return true;
		}
		public bool AddEditFormTemplateControl(WebControl templateContainer, int visibleIndex) {
			return AddEditFormTemplateControl(templateContainer, visibleIndex, true);
		}
		public bool AddEditFormTemplateControl(WebControl templateContainer, int visibleIndex, bool doDataBinding) {
			ITemplate template = Grid.Templates.EditForm;
			if(template == null) return false;
			AddTemplateToControl(templateContainer, template, new GridViewEditFormTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), EditFormTemplates, doDataBinding);
			return true;
		}
		ITemplate GetTemplate(params ITemplate[] templates) {
			for(int n = templates.Length - 1; n >= 0; n--) {
				if(templates[n] != null) return templates[n];
			}
			return null;
		}
		void AddTemplateToControl(Control destination, ITemplate template, Control templateContainer, TemplateContainerCollection collection) {
			AddTemplateToControl(destination, template, templateContainer, collection, true);
		}
		void AddTemplateToControl(Control destination, ITemplate template, Control templateContainer, TemplateContainerCollection collection, bool doDataBinding) {
			template.InstantiateIn(templateContainer);
			destination.Controls.Add(templateContainer);
			if(string.IsNullOrEmpty(destination.ID) && !string.IsNullOrEmpty(templateContainer.ID)) {
				destination.ID = "tc" + templateContainer.ID;
			}
			if(!Grid.DesignMode && doDataBinding) {
				templateContainer.DataBind();
			}
			if(collection != null) collection.Add(templateContainer);
		}
		protected internal void ParseEditorValues() {
			if(DataProxy.IsEditorValuesExists) return;
			Dictionary<string, object> values = new Dictionary<string, object>();
			foreach(ASPxEditBase editor in EditorList) {
				int index = GetEditColumnIndex(editor);
				if(index < 0) continue;
				GridViewDataColumn col = Grid.Columns[index] as GridViewDataColumn;
				if(col != null) {
					values[col.FieldName] = editor.Value;
				}
			}
			DataProxy.SetEditorValues(values, false);
		}
		int GetEditColumnIndex(ASPxEditBase editor) {
			if(editor.ID.StartsWith(DXEditorString)) {
				int res;
				if(int.TryParse(editor.ID.Substring(DXEditorString.Length), out res)) return res;
			}
			return -1;
		}
		public bool HasPreviewRow(int visibleIndex) {
			if(!Grid.Settings.ShowPreview) return false;
			string text = TextBuilder.GetPreviewText(visibleIndex);
			if(text != null && text.Trim().Length > 0) return true;			
			return Grid.Templates.PreviewRow != null;
		}
		internal void ClearStylesCache() {
			StylesCache.Clear();
		}
		internal static void EnsureHierarchy(Control container) {			
			container.FindControl("any");
			foreach(Control child in container.Controls)
				EnsureHierarchy(child);			
		}
		internal int GetGroupButtonWidth() {
			return Grid.CalcBrowserDependentGroupButtonWidth();
		}
	}
	public delegate Control TemplateContainerFinder(Control container, object parameters, string id);
	public class TemplateContainerCollection : List<Control> {
		ASPxGridView grid;
		public TemplateContainerCollection(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		public Control FindChild(TemplateContainerFinder finder, object parameters, string id) {
			Grid.EnsureChildControlsCore();
			foreach(Control control in this) {
				Control res = finder(control, parameters, id);
				if(res != null) return res;
			}
			return null;
		}
		internal Dictionary<string, object> FindTwoWayBindings(ITemplate template) {
			IBindableTemplate bindable = template as IBindableTemplate;
			if(bindable == null) return null;
			Grid.EnsureChildControlsCore();
			if(Count == 0) return null;
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(Control control in this) {
				IOrderedDictionary values = bindable.ExtractValues(control);
				if(values == null || values.Count == 0) continue;
				foreach(DictionaryEntry entry in values) {
					res[ExtractTwoWayName(entry.Key)] = entry.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
		string ExtractTwoWayName(object key) {
			string result = key.ToString();
			if(result.StartsWith("[") && result.EndsWith("]"))
				result = result.Substring(1, result.Length - 2);
			return result;
		}
		internal Dictionary<string, object> FindTwoWayBindings(Dictionary<object, ITemplate> templates, TemplateContainerFinder finder) {
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(KeyValuePair<object, ITemplate> pair in templates) {
				IBindableTemplate bindable = pair.Value as IBindableTemplate;
				if(bindable == null) continue;
				Control control = FindChild(finder, pair.Key, null);
				if(control == null) continue;
				IOrderedDictionary values = bindable.ExtractValues(control);
				if(values == null || values.Count == 0) continue;
				foreach(DictionaryEntry entry in values) {
					res[entry.Key.ToString()] = entry.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
	}
	internal class SimpleValueProvider : IValueProvider {
		int visibleIndex;
		WebDataProxy data;
		public SimpleValueProvider(WebDataProxy data, int visibleIndex) {
			this.data = data;
			this.visibleIndex = visibleIndex;
		}
		object IValueProvider.GetValue(string fieldName) {
			return data.GetRowValue(visibleIndex, fieldName);
		}
	}
}
