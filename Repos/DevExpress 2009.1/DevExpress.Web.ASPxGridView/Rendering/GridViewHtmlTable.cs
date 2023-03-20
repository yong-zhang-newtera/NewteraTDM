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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxEditors.Internal;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class GridViewHtmlScrollableControlBase : ASPxInternalWebControl {
		ASPxGridViewRenderHelper renderHelper;
		WebControl scrollableDiv;
		public GridViewHtmlScrollableControlBase(ASPxGridViewRenderHelper renderHelper) {
			this.renderHelper = renderHelper;
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected WebControl ScrollableDiv { get { return scrollableDiv; } set { scrollableDiv = value; } }
		protected override void CreateControlHierarchy() {
			this.scrollableDiv = CreateScrollableDiv();
			Controls.Add(ScrollableDiv);
		}
		protected virtual WebControl CreateScrollableDiv() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
		}
		protected override void PrepareControlHierarchy() {
			ScrollableDiv.Style[HtmlTextWriterStyle.Overflow] = "auto";
			if(renderHelper.ShowHorizontalScrolling) {
				ScrollableDiv.Width = DesignMode ? Grid.Width : 1; 
				ScrollableDiv.Style[HtmlTextWriterStyle.OverflowX] = "scroll";
			}
		}
	}
	public class GridViewHtmlScrollableControl : GridViewHtmlScrollableControlBase {
		WebControl headerDiv;
		WebControl footerDiv;
		public GridViewHtmlScrollableControl(ASPxGridViewRenderHelper renderHelper) : base(renderHelper) {
		}
		protected ASPxGridViewSettings Settings { get { return Grid.Settings; } }
		protected WebControl HeaderDiv { get { return headerDiv; } }
		protected WebControl FooterDiv { get { return footerDiv; } }
		protected override void CreateControlHierarchy() {
			CreateHeader();
			CreateContent();
			CreateFooter();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareHeaderOrFooter(HeaderDiv, true);
			PrepareHeaderOrFooter(FooterDiv, false);
			ScrollableDiv.Height = Unit.Pixel(Settings.VerticalScrollableHeight); 
			ScrollableDiv.Style[HtmlTextWriterStyle.OverflowY] = "scroll";
			if(RenderHelper.HasFixedColumns)
				ScrollableDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
		}
		protected void PrepareHeaderOrFooter(WebControl control, bool isHeader) {
			if (control == null) return;
			if (!isHeader || RenderHelper.ShowHorizontalScrolling || !RenderUtils.IsIE) {
				control.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			}
			control.Style[HtmlTextWriterStyle.MarginRight] = "17px";
			if (RenderHelper.ShowHorizontalScrolling) {
				control.Width = DesignMode ? Grid.Width : 0;
			} 
		}
		protected virtual void CreateHeader() {
			if(!Settings.ShowColumnHeaders && !Settings.ShowFilterRow) return;
			this.headerDiv = AddNewPartTable(GridViewHtmlTableRenderPart.Header, RenderHelper.HeaderTableID, RenderUtils.CreateDiv());
		}
		protected virtual void CreateContent() {
			ScrollableDiv = RenderUtils.CreateDiv();
			AddTopBottomVirtualScrollMargin(RenderHelper.VirtualScrollMarginTop, false);
			AddNewPartTable(GridViewHtmlTableRenderPart.Content, RenderHelper.MainTableID, ScrollableDiv);
			AddTopBottomVirtualScrollMargin(RenderHelper.VirtualScrollMarginBottom, true);
		}
		protected virtual void AddTopBottomVirtualScrollMargin(int height, bool createWithEmptyHeight) {
			if (!RenderHelper.IsVirtualScrolling) return;
			if (height == 0 && !createWithEmptyHeight) return;
			WebControl marginDiv = RenderUtils.CreateDiv();
			marginDiv.Width = Unit.Pixel(1);
			marginDiv.Style[HtmlTextWriterStyle.Height] = height.ToString() + "px";
			ScrollableDiv.Controls.Add(marginDiv);
		}
		protected virtual void CreateFooter() {
			if(!Settings.ShowFooter) return;
			this.footerDiv = AddNewPartTable(GridViewHtmlTableRenderPart.Footer, RenderHelper.FooterTableID, RenderUtils.CreateDiv());
		}
		protected virtual WebControl AddNewPartTable(GridViewHtmlTableRenderPart renderPart, string id, WebControl contentDiv) {
			Table table = new GridViewHtmlTable(RenderHelper, renderPart);
			if(!string.IsNullOrEmpty(id)) {
				table.ID = id;
			}
			contentDiv.Controls.Add(table);
			Controls.Add(contentDiv);
			return contentDiv;
		}
	}
	public class GridViewHtmlHorizontalScrollableControl : GridViewHtmlScrollableControlBase {
		WebControl mainContent;
		public GridViewHtmlHorizontalScrollableControl(ASPxGridViewRenderHelper renderHelper, WebControl mainContent) : base(renderHelper) {
			this.mainContent = mainContent;
		}
		protected WebControl MainContent { get { return mainContent; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ScrollableDiv.Controls.Add(MainContent);
			ScrollableDiv.ID = RenderHelper.HorzScrollDivID;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RenderHelper.HasFixedColumns)
				ScrollableDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
		}
	}
	public class GridViewHtmlFixedColumnsScrollableControl : GridViewHtmlScrollableControlBase {
		WebControl fixedContent;
		public GridViewHtmlFixedColumnsScrollableControl(ASPxGridViewRenderHelper renderHelper) : base(renderHelper) {
		}
		protected WebControl FixedContent { get { return fixedContent; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ScrollableDiv.ID = RenderHelper.FixedColumnsDivID;
			this.fixedContent = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			FixedContent.ID = RenderHelper.FixedColumnsContentDivID;
			ScrollableDiv.Controls.Add(FixedContent);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			FixedContent.Height = Unit.Pixel(17);
			FixedContent.Width = Unit.Pixel(5000);
			if(!RenderHelper.ShowVerticalScrolling)
				ScrollableDiv.Style[HtmlTextWriterStyle.OverflowY] = "hidden";
		}
	}
	public enum GridViewLastRowBottomBorder { RegularRow, LastRowRemoveBorder, RequireBorder }
	public enum GridViewHtmlTableRenderPart { All, Header, Content, Footer }
	[ViewStateModeById]
	public class GridViewHtmlTable : InternalTable {		
		ASPxGridViewRenderHelper renderHelper;
		GridViewHtmlTableRenderPart renderPart;
		int dataRowIndex;
		public GridViewHtmlTable(ASPxGridViewRenderHelper renderHelper) : this(renderHelper, GridViewHtmlTableRenderPart.All) { }
		public GridViewHtmlTable(ASPxGridViewRenderHelper renderHelper, GridViewHtmlTableRenderPart renderPart) {
			this.renderHelper = renderHelper;
			this.renderPart = renderPart;
			this.dataRowIndex = 0;
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return renderHelper.Grid; } }
		protected GridViewHtmlTableRenderPart RenderPart { get { return renderPart; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected List<GridViewColumn> Columns { get { return RenderHelper.VisibleColumns; } }
		protected bool CanRenderPart(GridViewHtmlTableRenderPart part) { return part == RenderPart || RenderPart == GridViewHtmlTableRenderPart.All; }
		protected bool RequireTableOnClick { get { return DataProxy.AllowFocusedRow || !string.IsNullOrEmpty(Grid.ClientSideEvents.RowClick) || Grid.SettingsBehavior.AllowMultiSelection; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(CanRenderPart(GridViewHtmlTableRenderPart.Header)) {
			CreateHeaders();
			CreateFilterRow();
			}
			if(CanRenderPart(GridViewHtmlTableRenderPart.Content)) {
				CreateNewRow(GridViewNewItemRowPosition.Top);
				CreateRows();
				CreateNewRow(GridViewNewItemRowPosition.Bottom);
			}
			if(CanRenderPart(GridViewHtmlTableRenderPart.Footer)) {
				CreateFooter();
			}
			if(Rows.Count > 0)
				CreateColGroups();
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.ClearStylesCache();
			base.PrepareControlHierarchy();
			CellPadding = 0;
			CellSpacing = 0;
			AppearanceStyleBase style = RenderHelper.GetTableStyle();
			style.AssignToControl(this);
			if(RenderHelper.Is100PercentageMainTableWidthRequired(RenderPart)) {
				Width = Unit.Percentage(100);
			}
			Style["empty-cells"] = "show";
			if(RequireTableOnClick) {
				Attributes["onclick"] = Scripts.GetMainTableClickFunction();
			}
			if(!string.IsNullOrEmpty(Grid.ClientSideEvents.RowDblClick)) {
				Attributes["ondblclick"] = Scripts.GetMainTableDblClickFunction();
			}
			if(RenderHelper.RequireFixedTableLayout) {
				Style["TABLE-LAYOUT"] = "fixed";
				Style[HtmlTextWriterStyle.Overflow] = "hidden";
				if(RenderHelper.AllowColumnResizing)
					Style[HtmlTextWriterStyle.TextOverflow] = "ellipsis";
			}
			if(RenderPart == GridViewHtmlTableRenderPart.All || RenderPart == GridViewHtmlTableRenderPart.Header) {
				Caption = Grid.Caption;
				RenderUtils.SetStringAttribute(this, "summary", Grid.SummaryText);
			}
		}
		protected virtual bool RequireVerticalScrollEmtyRow {
			get {
				if(!RenderHelper.ShowVerticalScrolling || !RenderUtils.IsIE) return false;
				if(renderHelper.GroupCount > 0) return true;
				return Grid.SettingsEditing.IsInline && DataProxy.IsEditing;
			}
		}
		protected virtual void CreateVerticalScrollEmtyRow() {
			if(!RequireVerticalScrollEmtyRow) return;
			TableRow row = RenderUtils.CreateTableRow();
			row.Height = Unit.Pixel(0);
			Rows.Add(row);
			for(int i = 0; i < RenderHelper.ColumnSpanCount; i++) {
				row.Cells.Add(RenderUtils.CreateTableCell());
			}
		}
		protected virtual void CreateHeaders() {
			if(!Grid.Settings.ShowColumnHeaders) return;
			CreateVerticalScrollEmtyRow();
			Rows.Add(new GridViewTableHeaderRow(RenderHelper));
		}
		protected virtual void CreateFilterRow() {
			if(!Grid.Settings.ShowFilterRow || Columns.Count == 0) return;
			AddRowAndRaiseRowCreated(new GridViewTableFilterRow(RenderHelper));
		}
		protected GridViewLastRowBottomBorder LastDataRowBottomBorder {
			get {
				bool isRemoveBorderFromLastRow = RenderHelper.IsRemoveBorderFromMainTableLastRow && !ShowFooter;
				return isRemoveBorderFromLastRow ? GridViewLastRowBottomBorder.LastRowRemoveBorder : GridViewLastRowBottomBorder.RequireBorder;
			}
		}
		protected virtual void CreateRows() {
			CreateVerticalScrollEmtyRow();
			int si = DataProxy.VisibleStartIndex;
			int emptyDataRowCount = RenderHelper.EmptyPagerDataRowCount;
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				GridViewLastRowBottomBorder rowBottomBorder = GridViewLastRowBottomBorder.RegularRow;
				if (i == DataProxy.VisibleRowCountOnPage - 1 + emptyDataRowCount) {
					rowBottomBorder = LastDataRowBottomBorder;
				}
				CreateRow(i + si, rowBottomBorder);
			}
			if(RenderHelper.HasEmptyDataRow) {
				CreateEmptyRow(emptyDataRowCount > 0 ? GridViewLastRowBottomBorder.RegularRow : LastDataRowBottomBorder);
			}
			for (int i = 0; i < emptyDataRowCount; i++) {
				CreatePagerEmptyRow(DataProxy.VisibleRowCountOnPage + i, i < emptyDataRowCount - 1 ? GridViewLastRowBottomBorder.RegularRow : LastDataRowBottomBorder);
			}
		}
		protected virtual void CreateNewRow(GridViewNewItemRowPosition rowPosition) {
			if(!DataProxy.IsNewRowEditing || rowPosition != Grid.SettingsEditing.NewItemRowPosition) return;
			GridViewLastRowBottomBorder lastRowBorder = rowPosition == GridViewNewItemRowPosition.Bottom && RenderHelper.IsRemoveBorderFromMainTableLastNewItemRow ?
				GridViewLastRowBottomBorder.LastRowRemoveBorder : GridViewLastRowBottomBorder.RequireBorder;
			CreateEditRow(WebDataProxy.NewItemRow, lastRowBorder);
		}
		protected virtual void CreateEmptyRow(GridViewLastRowBottomBorder lastRowBottomBorder) {
			AddRowAndRaiseRowCreated(new GridViewTableEmptyDataRow(RenderHelper, lastRowBottomBorder));
		}
		protected virtual void CreatePagerEmptyRow(int rowVisibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			AddRowAndRaiseRowCreated(new GridViewTablePagerEmptyRow(RenderHelper, rowVisibleIndex, lastRowBottomBorder));
		}
		protected virtual void CreateRow(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			List<int> groupFooterVisibleIndexes = RenderHelper.GetGroupFooterVisibleIndexes(visibleIndex);
			CreateRowCore(visibleIndex, groupFooterVisibleIndexes, lastRowBottomBorder);
			if(groupFooterVisibleIndexes != null) {
				CreateGroupFooterRow(groupFooterVisibleIndexes, lastRowBottomBorder);
			}
		}
		protected virtual void CreateRowCore(int visibleIndex, List<int> groupFooterVisibleIndexes, GridViewLastRowBottomBorder lastRowBottomBorder) {
			if(DataProxy.GetRowType(visibleIndex) == WebRowType.Group) {
				CreateGroupRow(visibleIndex, groupFooterVisibleIndexes, lastRowBottomBorder);
				return;
			}
			if(DataProxy.IsRowEditing(visibleIndex)) {
				CreateEditRow(visibleIndex, lastRowBottomBorder);
			}
			CreateDataPreviewDetailRows(visibleIndex, lastRowBottomBorder);
		}
		protected virtual void AddRowAndRaiseRowCreated(GridViewTableRow row) {
			Rows.Add(row);
			Grid.RaiseHtmlRowCreated(row);
		}
		protected virtual void CreateDataPreviewDetailRows(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			bool hasDetailRow = RenderHelper.HasDetailRow(visibleIndex);
			if(DataProxy.EditingRowVisibleIndex == visibleIndex && Grid.SettingsEditing.Mode != GridViewEditingMode.Inline) {
				lastRowBottomBorder = GridViewLastRowBottomBorder.RegularRow;
			}
			if(!DataProxy.IsRowEditing(visibleIndex)) {
				bool hasPreviewRow = RenderHelper.HasPreviewRow(visibleIndex);
				AddRowAndRaiseRowCreated(CreateDataRow(visibleIndex, (hasPreviewRow || hasDetailRow) ? GridViewLastRowBottomBorder.RegularRow : lastRowBottomBorder));
				if(hasPreviewRow) {
					AddRowAndRaiseRowCreated(CreatePreviewRow(visibleIndex, hasDetailRow ? GridViewLastRowBottomBorder.RegularRow : lastRowBottomBorder));
				}
			}
			if(hasDetailRow) {
				AddRowAndRaiseRowCreated(CreateDetailRow(visibleIndex, lastRowBottomBorder));
			}
		}
		protected virtual GridViewTableRow CreateDetailRow(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			return new GridViewTableDetailRow(RenderHelper, visibleIndex, lastRowBottomBorder);
		}
		protected virtual GridViewTableRow CreatePreviewRow(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			return new GridViewTablePreviewRow(RenderHelper, visibleIndex, lastRowBottomBorder);
		}
		protected virtual void CreateGroupRow(int visibleIndex, List<int> groupFooters, GridViewLastRowBottomBorder lastRowBottomBorder) {
			bool hasGroupFooter = groupFooters != null && groupFooters.Count == DataProxy.GetRowLevel(visibleIndex) + 1;
			AddRowAndRaiseRowCreated(new GridViewTableGroupRow(RenderHelper, visibleIndex, hasGroupFooter, lastRowBottomBorder));
		}
		protected virtual void CreateGroupFooterRow(List<int> visibleIndexes, GridViewLastRowBottomBorder lastRowBottomBorder) {
			for(int i = 0; i < visibleIndexes.Count; i ++) {
				AddRowAndRaiseRowCreated(new GridViewTableGroupFooterRow(RenderHelper, visibleIndexes[i], lastRowBottomBorder));
			}
		}
		protected virtual GridViewTableRow CreateDataRow(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			return new GridViewTableDataRow(RenderHelper, visibleIndex, dataRowIndex++, lastRowBottomBorder);
		}
		protected virtual void CreateEditRow(int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			if(Grid.SettingsEditing.DisplayEditingRow && !DataProxy.IsNewRowEditing) {
				GridViewTableDataRow dataRow = (GridViewTableDataRow)CreateDataRow(visibleIndex, GridViewLastRowBottomBorder.RegularRow);
				AddRowAndRaiseRowCreated(dataRow);
			}
			if(Grid.SettingsEditing.IsInline) {
				AddRowAndRaiseRowCreated(new GridViewTableInlineEditRow(RenderHelper, visibleIndex, RenderHelper.HasEditingError ? GridViewLastRowBottomBorder.RegularRow : lastRowBottomBorder));
			} 
			if(Grid.SettingsEditing.IsEditForm) {
				AddRowAndRaiseRowCreated(new GridViewTableEditFormRow(RenderHelper, visibleIndex, RenderHelper.HasEditingError ? GridViewLastRowBottomBorder.RegularRow : lastRowBottomBorder));
			}
			if(RenderHelper.HasEditingError) {
				AddRowAndRaiseRowCreated(new GridViewTableEditingErrorRow(RenderHelper, false, lastRowBottomBorder));
			}
		}
		protected bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		protected virtual void CreateFooter() {
			if(!ShowFooter) return;
			CreateVerticalScrollEmtyRow(); 
			GridViewLastRowBottomBorder rowBottomBorder = RenderHelper.RequireRenderBottomPagerControl ? 
				GridViewLastRowBottomBorder.RequireBorder : GridViewLastRowBottomBorder.LastRowRemoveBorder;
			AddRowAndRaiseRowCreated(new GridViewTableFooterRow(RenderHelper, rowBottomBorder));
		}
		protected virtual bool RequireColGroups {  get {  return RenderHelper.RequireRenderColGroups; } }
		protected virtual void CreateColGroups() {
			if(!RequireColGroups) return;
			InternalTableColGroup colGroup = new InternalTableColGroup();
			ColGroups.Add(colGroup);
			int groupButtonWidth = RenderHelper.GetGroupButtonWidth();
			for(int i = 0; i < RenderHelper.IndentColumnCount; i++)
				colGroup.AddCol(groupButtonWidth);
			for(int i = 0; i < Columns.Count; i++) {
				colGroup.AddCol(Columns[i].Width); 
			}
		}
	}
	public class GridViewHtmlStyleTable : InternalTable {
		ASPxGridView grid;
		TableRow selectedRow, focusedRow, focusedGroupRow;
		public GridViewHtmlStyleTable(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected TableRow SelectedRow { get { return selectedRow; } }
		protected TableRow FocusedRow { get { return focusedRow; } }
		protected TableRow FocusedGroupRow { get { return focusedGroupRow; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetStyleTableId();
			CreateHeaderRow();
			CreateEditingErrorRow();
			CreateSelectedRow();
			CreateFocusedRow();
			CreateFocusedGroupRow();
			CreateRows();
		}
		protected override void PrepareControlHierarchy() {
			CreateEmptyTDForXHTMLCompatibility();
			Style.Add(HtmlTextWriterStyle.Display, "none");
			RenderHelper.GetSelectedRowStyle().AssignToControl(SelectedRow, true);
			RenderHelper.GetFocusedRowStyle().AssignToControl(FocusedRow, true);
			RenderHelper.GetFocusedGroupRowStyle().AssignToControl(FocusedGroupRow, true);
		}
		protected virtual void CreateHeaderRow() {
			Rows.Add(RenderUtils.CreateTableRow());
		}
		protected virtual void CreateEditingErrorRow() {
			Rows.Add(new GridViewTableEditingErrorRow(RenderHelper, true, GridViewLastRowBottomBorder.RegularRow)); 
		}
		protected virtual void CreateSelectedRow() {
			this.selectedRow = RenderUtils.CreateTableRow();
			Rows.Add(SelectedRow);
		}
		protected virtual void CreateFocusedRow() {
			this.focusedRow = RenderUtils.CreateTableRow();
			Rows.Add(FocusedRow);
		}
		protected virtual void CreateFocusedGroupRow() {
			this.focusedGroupRow = RenderUtils.CreateTableRow();
			Rows.Add(FocusedGroupRow);
		}
		protected virtual void CreateRows() {
			int dataRowIndex = 0;
			int si = DataProxy.VisibleStartIndex;
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				int visibleIndex = si + i;
				if(DataProxy.GetRowType(si + i) == WebRowType.Data) {
					Rows.Add(new GridViewTableDataRow(RenderHelper, visibleIndex, dataRowIndex++, GridViewLastRowBottomBorder.RequireBorder, true));
				} else {
					GridViewTableGroupRow groupRow = new GridViewTableGroupRow(RenderHelper, visibleIndex, false, GridViewLastRowBottomBorder.RequireBorder);
					groupRow.IsStyledRow = true;
					Rows.Add(groupRow);
				}
			}
		}
		protected virtual void CreateEmptyTDForXHTMLCompatibility() {
			foreach(TableRow row in Rows) {
				if(row.Cells.Count == 0) {
					row.Cells.Add(RenderUtils.CreateTableCell());
				}
			}
		}
	}
}
