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
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.Internal;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView.Rendering {
	[ViewStateModeById]
	public class GridViewHtmlHeaderContent : InternalTable {
		GridViewColumn column;
		TableCell textCell;
		TableCell filterCell;
		Image filterImage;
		Image sortImage;
		TableCell sortCell;
		GridViewHeaderLocation location;
		public GridViewHtmlHeaderContent(GridViewColumn column, GridViewHeaderLocation location) {
			this.column = column;
			this.location = location;
		}
		public GridViewColumn Column { get { return column; } }
		public ASPxGridView Grid { get { return Column.Grid; } }
		public ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public GridViewHeaderLocation Location { get { return location; } }
		protected bool ShowFilterButton { get { return Column.GetHasFilterButton(); } }
		protected bool IsFilterActive { get { return Column.GetIsFiltered(); } }
		protected bool ShowSortImage { get { return Column.GetSortOrder() != DevExpress.Data.ColumnSortOrder.None; } }
		protected override void CreateControlHierarchy() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			textCell = RenderUtils.CreateTableCell();
			row.Cells.Add(textCell);
			if(!RenderHelper.AddHeaderCaptionTemplateControl(Column, textCell, Location)) {
				CreateCaptionText();
			} else {
				ID = RenderHelper.GetColumnHeaderContentId(Column, Location);
			}
			AddSortButton(row);
			AddFilterButton(row);
		}
		protected override void PrepareControlHierarchy() {
			CellPadding = 0;
			CellSpacing = 0;
			Width = Unit.Percentage(100);
			if(RenderHelper.AllowColumnResizing) {
				if(RenderUtils.IsIE)
					Style[HtmlTextWriterStyle.TextOverflow] = "ellipsis";
			}
			GridViewHeaderStyle headerStyle = RenderHelper.GetHeaderStyle(Column);
			headerStyle.AssignToControl(this,  AttributesRange.Font);
			if(textCell != null) {
				headerStyle.AssignToControl(textCell, AttributesRange.Cell);
				RenderUtils.SetPaddings(textCell, headerStyle.Paddings);
				if(Location == GridViewHeaderLocation.Group && headerStyle.Paddings.GetPaddingLeft().IsEmpty) {
					RenderUtils.SetStyleUnitAttribute(textCell, "padding-left", Unit.Pixel(1));
				}
				if(Location == GridViewHeaderLocation.Customization) {
					HorizontalAlign popupContentAlign = RenderHelper.Styles.CustomizationWindowContent.HorizontalAlign;
					if(popupContentAlign != HorizontalAlign.NotSet)
						RenderUtils.SetHorizontalAlign(textCell, popupContentAlign);
				}
			}
			if(sortCell != null || filterCell != null) {
				RenderUtils.SetStyleUnitAttribute(textCell, "border-right-width", 0);
				textCell.Width = Unit.Percentage(100);
				if(sortCell != null) {
					RenderUtils.SetStyleUnitAttribute(textCell, "padding-right", headerStyle.GetSortingImageSpacing());
				} else {
					RenderUtils.SetStyleUnitAttribute(textCell, "padding-right", headerStyle.GetFilterImageSpacing());
				}
			}
			if(sortCell != null) {
				Paddings sortPaddings = headerStyle.Paddings;
				RenderUtils.SetPaddings(sortCell, sortPaddings);
				RenderUtils.SetStyleUnitAttribute(sortCell, "border-left-width", 0);
				RenderUtils.SetStyleUnitAttribute(sortCell, "padding-left", headerStyle.GetSortingImageSpacing());
				if(filterCell != null) {
					RenderUtils.SetStyleUnitAttribute(sortCell, "border-right-width", 0);
					RenderUtils.SetStyleUnitAttribute(sortCell, "padding-right", headerStyle.GetFilterImageSpacing());
				}
				RenderHelper.GetHeaderSortImage(Column.GetSortOrder()).AssignToControl(sortImage, Grid.DesignMode);
			}
			if(filterCell != null) {
				RenderUtils.SetPaddings(filterCell, headerStyle.Paddings);
				RenderUtils.SetStyleUnitAttribute(filterCell, "border-left-width", 0);
				RenderUtils.SetStyleUnitAttribute(filterCell, "padding-left", 0);
				RenderUtils.SetCursor(filterCell, RenderUtils.GetDefaultCursor());
			}
		}
		protected void AddFilterButton(TableRow row) {
			if(!ShowFilterButton) return;
			filterCell = RenderUtils.CreateTableCell();
			row.Cells.Add(filterCell);
			filterImage = RenderUtils.CreateImage();
			filterCell.Controls.Add(filterImage);
			string imageName = IsFilterActive ?
				GridViewImages.HeaderFilterActiveName : GridViewImages.HeaderFilterName;
			RenderHelper.AssignImageToControl(imageName, filterImage);
			if(IsEnabled)
				filterImage.Attributes.Add("onclick", RenderHelper.Scripts.GetShowFilterPopup(RenderHelper.GetColumnId(Column, Location), Column));
		}
		protected void AddSortButton(TableRow row) {
			if(!ShowSortImage) return;
			sortCell = RenderUtils.CreateTableCell();
			row.Cells.Add(sortCell);
			this.sortImage = RenderUtils.CreateImage();
			sortCell.Controls.Add(this.sortImage);
		}
		void CreateCaptionText() {
			string text = RenderHelper.TextBuilder.GetHeaderCaption(Column);
			if(string.IsNullOrEmpty(text)) {
				textCell.Text = "&nbsp;";
				return;
			}
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(!Grid.IsAccessibilityCompliantRender(true) || dataColumn == null || !dataColumn.GetAllowSort()) {
				textCell.Text = text;
			} else {
				HyperLink link = RenderUtils.CreateHyperLink();
				link.Text = text;
				link.NavigateUrl = string.Format("javascript:{0}", RenderHelper.Scripts.GetAccessibleSortClick(Column.Index));
				RenderUtils.SetStringAttribute(link, "onmousedown", ASPxGridViewRenderHelper.CancelBubbleJs);
				textCell.Controls.Add(link);
			}
		}
	}
	[ViewStateModeById]
	public class GridViewTableCell : InternalTableCell {
		ASPxGridViewRenderHelper renderHelper;
		public GridViewTableCell(ASPxGridViewRenderHelper renderHelper) {
			this.renderHelper = renderHelper;
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected string GridClientID { get { return RenderHelper.Grid.ClientID; } }
	}
	public class GridViewTableCellEx : GridViewTableCell {
		bool removeRightBorder;
		public GridViewTableCellEx(ASPxGridViewRenderHelper renderHelper) : this(renderHelper, false) { }
		public GridViewTableCellEx(ASPxGridViewRenderHelper renderHelper, bool removeRightBorder)
			: base(renderHelper) {
			this.removeRightBorder = removeRightBorder;
		}
		protected bool RemoveRightBorder { get { return removeRightBorder && RenderHelper.RemoveCellRightBorder; } }
		protected GridViewLastRowBottomBorder LastRowBottomBorder {
			get {
				GridViewTableRow row = Parent as GridViewTableRow;
				return row != null ? row.LastRowBottomBorder : GridViewLastRowBottomBorder.RegularRow;
			}
		}
		protected virtual bool GetDefaultRemoveBottomBorder() { return false; }
		protected virtual bool GetRemoveBottomBorder() {
			if(LastRowBottomBorder == GridViewLastRowBottomBorder.RequireBorder) return false;
			if(LastRowBottomBorder == GridViewLastRowBottomBorder.LastRowRemoveBorder) return true;
			return !RenderHelper.ShowHorizontalGridLine ? true : GetDefaultRemoveBottomBorder();
		}
		protected virtual bool GetRemoveRightBorder() { return RemoveRightBorder; }
		protected override void PrepareControlHierarchy() {
			if(GetRemoveRightBorder()) {
				RenderUtils.SetStyleUnitAttribute(this, "border-right-width", 0);
			}
			if(GetRemoveBottomBorder()) RenderUtils.SetStyleUnitAttribute(this, "border-bottom-width", 0);
		}
		protected WebControl GetVerticalScrollingParentControl() {
			WebControl parentControl = this;
			if(RequireDivControlForScrollingColumnResizing) {
				parentControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parentControl.Style[HtmlTextWriterStyle.Overflow] = "hidden";
				parentControl.Width = Unit.Percentage(100);
				Controls.Add(parentControl);
	}
			return parentControl;
			}
		protected virtual bool RequireDivControlForScrollingColumnResizing {
			get { return RenderHelper.ShowVerticalScrolling &&  !RenderUtils.IsIE; }
		}
	}
	[ViewStateModeById]
	public abstract class GridViewTableRow : InternalTableRow {
		ASPxGridViewRenderHelper renderHelper;
		int visibleIndex;
		GridViewLastRowBottomBorder lastRowBottomBorder;
		public GridViewTableRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) {
			this.renderHelper = renderHelper;
			this.visibleIndex = visibleIndex;
			this.lastRowBottomBorder = lastRowBottomBorder;
		}
		public virtual GridViewLastRowBottomBorder LastRowBottomBorder { get { return lastRowBottomBorder; } }
		public ASPxGridView Grid { get { return RenderHelper.Grid; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public abstract GridViewRowType RowType { get; }
		protected virtual bool RequireRenderParentRows { get { return false; } }
		protected bool HasParentRows { get { return RequireRenderParentRows && VisibleIndex > -1 && VisibleIndex == DataProxy.VisibleStartIndex && DataProxy.HasParentRows; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected List<GridViewColumn> Columns { get { return RenderHelper.VisibleColumns; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected int GroupSpanCount { get { return RenderHelper.GroupCount; } }
		protected virtual int ColumIndentCount { get { return GroupSpanCount; } }
		protected virtual int RowLastInLevel { get { return VisibleIndex > -1 ? DataProxy.RowIsLastInLevel(VisibleIndex) : -1; } }
		protected virtual bool RenderDetailIndent { get { return RenderHelper.HasDetailButton; } }
		protected virtual void CreateIndentCells() {
			if(ColumIndentCount > 0) {
				int startIndex = 0;
				if(HasParentRows) {
					Cells.Add(new GridViewTableInvisibleParentsRowDataCell(RenderHelper));
					startIndex++;
				}
				CreateIndentCellsCore(startIndex);
			}
			if(RenderDetailIndent) {
				Cells.Add(CreateDetailButtonCell());
			}
		}
		protected virtual void CreateIndentCellsCore(int startIndex) {
			int rowLastInLevel = RowLastInLevel;
			if(rowLastInLevel < 0) rowLastInLevel = ColumIndentCount;
			List<int> groupFooters = RenderHelper.GetGroupFooterVisibleIndexes(VisibleIndex);
			if(groupFooters != null) {
				rowLastInLevel += groupFooters.Count;
			}
			for(int i = startIndex; i < ColumIndentCount; i++) {
				Cells.Add(CreateIndentTableCell(rowLastInLevel <= i));
			}
		}
		protected virtual TableCell CreateDetailButtonCell() {
			return new GridViewTableEmptyIndentCell(RenderHelper);
		}
		protected virtual TableCell CreateIndentTableCell(bool isLastInLevel) {
			return new GridViewTableIndentDataCell(RenderHelper, isLastInLevel);
		}
	}
	[ViewStateModeById]
	public class GridViewTableHeaderRow : InternalTableRow {
		ASPxGridViewRenderHelper renderHelper;
		public GridViewTableHeaderRow(ASPxGridViewRenderHelper renderHelper)  {
			this.renderHelper = renderHelper; 
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected List<GridViewColumn> Columns { get { return RenderHelper.VisibleColumns; } }
		protected int GroupSpanCount { get { return RenderHelper.GroupCount; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = RenderHelper.GetHeadersRowId();
			if(Columns.Count == 0) {
				Cells.Add(new GridViewTableEmptyHeaderCell(RenderHelper));
			} else {
				for(int i = 0; i < GroupSpanCount; i++) {
					CreateEmptyHeaderCell();
				}
				if(RenderHelper.HasDetailButton) {
					CreateEmptyHeaderCell();
				}
				for(int i = 0; i < Columns.Count; i++) {
					CreateHeaderCell(Columns[i], i == Columns.Count - 1);
				}
			}
		}
		protected virtual void CreateHeaderCell(GridViewColumn column, bool removeRightBorder) {
			Cells.Add(new GridViewTableHeaderCell(RenderHelper, column, GridViewHeaderLocation.Row, removeRightBorder));
		}
		protected virtual void CreateEmptyHeaderCell() {
			Cells.Add(new GridViewTableHeaderIndentCell(RenderHelper));
		}
	}
	public class GridViewTableEmptyHeaderCell : GridViewTableCellEx {
		public GridViewTableEmptyHeaderCell(ASPxGridViewRenderHelper renderHelper) : base(renderHelper, false) { }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetEmptyHeaderId();
			Text = RenderHelper.Grid.SettingsText.GetEmptyHeaders();
			ColumnSpan = RenderHelper.ColumnSpanCount;
		}
		protected override bool GetRemoveBottomBorder() { return false; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderPanelStyle().AssignToControl(this, true);
			Width = Unit.Percentage(.1d);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableHeaderCellBase : GridViewTableCellEx {
		GridViewHeaderLocation location;
		public GridViewTableHeaderCellBase(ASPxGridViewRenderHelper renderHelper, GridViewHeaderLocation location, bool removeRightBorder)
			: base(renderHelper, removeRightBorder) {
			this.location = location;
		}		
		public GridViewHeaderLocation Location { get { return location; } }
		protected virtual bool IsClickable { get { return false; } }
		protected override void PrepareControlHierarchy() {
			if(Location == GridViewHeaderLocation.Row) {
				RenderUtils.SetStyleUnitAttribute(this, "border-left-width", 0);
				if(!RenderHelper.RequireHeaderTopBorder) {
					RenderUtils.SetStyleUnitAttribute(this, "border-top-width", 0);
				}
			}
			if(!IsClickable || !RenderHelper.IsGridEnabled) {
				Style[HtmlTextWriterStyle.Cursor] = "default";
			}
			base.PrepareControlHierarchy();
		}
	}
	public enum GridViewHeaderLocation { Row, Group, Customization }
	public class GridViewTableHeaderCell : GridViewTableHeaderCellBase {
		GridViewColumn column;
		WebControl parentControl;
		public GridViewTableHeaderCell(ASPxGridViewRenderHelper renderHelper, GridViewColumn column, GridViewHeaderLocation location, bool removeRightBorder)
			: base(renderHelper, location, removeRightBorder) {
			this.column = column;
		}		
		public GridViewColumn Column { get { return column; } }
		protected WebControl ParentControl { get { return parentControl; } }
		protected override bool GetRemoveBottomBorder() { return false; }
		protected override bool RequireDivControlForScrollingColumnResizing {
			get {
				return IsResizableColumn  && base.RequireDivControlForScrollingColumnResizing;
			}
		}
		protected bool IsResizableColumn { get { return RenderHelper.AllowColumnResizing && Location == GridViewHeaderLocation.Row; } }
		protected bool HasToSetWidth {
			get {
				if(Location == GridViewHeaderLocation.Customization) return false;
				if(IsResizableColumn) return false;
				return true;
			}
		}
		protected override HtmlTextWriterTag TagKey { 
			get { return RenderHelper.Grid.IsAccessibilityCompliantRender() ? HtmlTextWriterTag.Th : base.TagKey; } 
		}
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetColumnId(column, Location);
			this.parentControl = GetVerticalScrollingParentControl();
			if(!RenderHelper.AddHeaderTemplateControl(Column, parentControl, Location)) {
				parentControl.Controls.Add(new GridViewHtmlHeaderContent(Column, Location));
				if(!string.IsNullOrEmpty(Column.ToolTip)) {
					ToolTip = Column.ToolTip;
				}
			}
		}
		protected override bool IsClickable { get { return Column.IsClickable(); } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderStyle(Column).AssignToControl(this, true);
			Attributes.Add("onmousedown", Scripts.GetHeaderColumnClick());
			string onContextMenuScript = RenderHelper.Scripts.GetContextMenu("header", Column.Index);
			if(!string.IsNullOrEmpty(onContextMenuScript)) {
				Attributes["oncontextmenu"] = onContextMenuScript;
			}
			if(RenderHelper.Grid.DesignMode && Location == GridViewHeaderLocation.Row) {
				Attributes[System.Web.UI.Design.DesignerRegion.DesignerRegionAttributeName] = Column.VisibleIndex.ToString();
			}
			if(HasToSetWidth) {
				Width = Column.Width;
			}
			if(RenderHelper.Grid.IsAccessibilityCompliantRender()) {
				RenderUtils.SetStringAttribute(this, "scope", "col");
				RenderUtils.SetStringAttribute(this, "abbr", RenderHelper.TextBuilder.GetHeaderCaption(Column));
			}
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableHeaderIndentCell : GridViewTableHeaderCellBase {
		public GridViewTableHeaderIndentCell(ASPxGridViewRenderHelper renderHeler)
			: base(renderHeler, GridViewHeaderLocation.Row,  false) {
		}
		protected override void CreateControlHierarchy() {
			Image image = RenderUtils.CreateImage();
			Controls.Add(image);	
			image.AlternateText = "img";
			image.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			RenderHelper.AssignImageToControl(GridViewImages.ExpandedButtonName, image);
			image.AlternateText = string.Empty;
		}
		protected override bool GetRemoveBottomBorder() {  return false; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderStyle(null).AssignToControl(this);
			Width = GetBrowserDependentWidth();
			Style[HtmlTextWriterStyle.Cursor] = "default";
			base.PrepareControlHierarchy();
		}
		Unit GetBrowserDependentWidth() {
			if(RenderHelper.RequireFixedTableLayout && !RenderHelper.RequireRenderColGroups)
				return RenderHelper.Styles.GroupButtonWidth;
			if(RenderUtils.IsOpera)
				return Unit.Pixel(1);
			return Unit.Percentage(.1d);
		}
	}
	public abstract class GridViewTableGroupAndDataRow : GridViewTableRow {
		bool isStyledRow = false;
		public GridViewTableGroupAndDataRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) { 
		}
		public bool IsStyledRow { get { return isStyledRow; } set { isStyledRow = value; } }
		protected abstract string ContextMenuType { get; }
		protected override void PrepareControlHierarchy() {
			if(!IsStyledRow) {
				string onContextMenuScript = RenderHelper.Scripts.GetContextMenu(ContextMenuType, VisibleIndex);
				if(!string.IsNullOrEmpty(onContextMenuScript)) {
					Attributes["oncontextmenu"] = onContextMenuScript;
				}
			}
		}
	}
	public class GridViewTableGroupRow : GridViewTableGroupAndDataRow {
		int groupLevel;
		bool isGroupButtonLive;
		bool hasGroupFooter;
		public GridViewTableGroupRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter, GridViewLastRowBottomBorder lastRowBottomBorder)
			: this(renderHelper, visibleIndex, hasGroupFooter, true, lastRowBottomBorder) { }
		public GridViewTableGroupRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter, bool isGroupButtonLive, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) {
			this.groupLevel = DataProxy.GetRowLevel(VisibleIndex);
			this.isGroupButtonLive = isGroupButtonLive;
			this.hasGroupFooter = hasGroupFooter;
		}
		protected override bool RequireRenderParentRows { get { return true; } }
		public override GridViewRowType RowType { get { return GridViewRowType.Group; } }
		protected override string ContextMenuType { get { return "grouprow"; } }
		protected GridViewDataColumn Column { get { return Grid.SortedColumns[GroupLevel]; } }
		protected int ColumnSpanCount { get { return RenderHelper.ColumnSpanCount; } }
		public int GroupLevel { get { return groupLevel; } }
		protected override int ColumIndentCount { get { return GroupLevel; } }
		protected bool IsGroupButtonLive { get { return isGroupButtonLive; } }
		protected bool IsExpanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
		protected bool HasGroupFooter { get { return hasGroupFooter; } }
		protected override int RowLastInLevel { get { return IsExpanded ? -1 : base.RowLastInLevel; } }
		protected override void CreateControlHierarchy() {
			if(IsStyledRow) return;
			ID = RenderHelper.GetGroupRowId(VisibleIndex);
			CreateIndentCells();
			if(RenderHelper.AddGroupRowTemplateControl(VisibleIndex, Column, this, ColumnSpanCount - GroupLevel)) return;
			if(Grid.Settings.ShowGroupButtons) CreateButtonCell();
			CreateContentCell();
		}
		protected override bool RenderDetailIndent { get { return false; } }
		protected virtual void CreateButtonCell() {
			Cells.Add(new GridViewTableGroupButtonCell(RenderHelper, VisibleIndex, HasGroupFooter, IsGroupButtonLive));
		}
		protected virtual int GetColSpanCount() {
			return ColumnSpanCount - GroupLevel - (Grid.Settings.ShowGroupButtons ? 1 : 0);
		}
		protected virtual void CreateContentCell() {
			TableCell res = new GridViewTableCellEx(RenderHelper, true);
			if(RenderUtils.IsOpera)
				res.Width = Unit.Pixel(9999);
			res.ColumnSpan = GetColSpanCount();
			RenderHelper.AppendDefaultDXClassName(res);
			Cells.Add(res);
			if(!RenderHelper.AddGroupRowContentTemplateControl(VisibleIndex, Column, res)) {
				res.Text = GetDisplayText();
				if(!DataProxy.IsGroupRowFitOnPage(VisibleIndex)) {
					res.Text += " " + Grid.SettingsText.GetGroupContinuedOnNextPage();
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			if(!IsStyledRow && DataProxy.IsRowFocused(VisibleIndex))
				RenderHelper.GetFocusedGroupRowStyle().AssignToControl(this, true);
			else RenderHelper.GetGroupRowStyle().AssignToControl(this, true);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected virtual string GetDisplayText() {
			return RenderHelper.TextBuilder.GetGroupRowText(Column, VisibleIndex);
		}
	}
	public class GridViewTableGroupFooterRow : GridViewTableRow {
		int groupLevel;
		public GridViewTableGroupFooterRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) {
			this.groupLevel = DataProxy.GetRowLevel(VisibleIndex);
		}
		protected int GroupLevel { get { return groupLevel; } }
		public override GridViewRowType RowType { get { return GridViewRowType.GroupFooter; } }
		protected override void CreateControlHierarchy() {
			CreateIndentCells();
			for (int i = 0; i < Columns.Count; i++) {
				TableCell cell = CreateFooterCell(Columns[i], i == Columns.Count - 1);
				Cells.Add(cell);
			}
			if(Columns.Count == 0) {
				Cells.Add(new GridViewTableEmptyIndentCell(RenderHelper));
			}
		}
		protected override void CreateIndentCellsCore(int startIndex) {
			if(ColumIndentCount == 0) return;
			int footerIndentCount = RenderHelper.GroupCount - GroupLevel - 1;
			int dataIndentCount = ColumIndentCount - footerIndentCount;
			for(int i = 0; i < dataIndentCount - 1; i++) {
				Cells.Add(CreateIndentTableCell(false));
			}
			Cells.Add(CreateGroupFooterIndentDataTableCell());
			if(footerIndentCount > 0) {
				Cells.Add(CreateGroupFooterIndentTableCell(footerIndentCount));
			}
		}
		protected virtual TableCell CreateFooterCell(GridViewColumn column, bool removeRightBorder) {
			return new GridViewTableGroupFooterCell(RenderHelper, column, VisibleIndex, removeRightBorder);
		}
		protected virtual TableCell CreateGroupFooterIndentTableCell(int columIndentCount) {
			return new GridViewTableGroupFooterIndentCell(RenderHelper, columIndentCount);
		}
		protected virtual TableCell CreateGroupFooterIndentDataTableCell() {
			return new GridViewTableGroupFooterIndentDataCell(RenderHelper);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetGroupFooterStyle().AssignToControl(this, true);
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTableGroupButtonCell : GridViewTableCellEx {
		int visibleIndex;
		bool isGroupButtonLive;
		bool hasGroupFooter;
		public GridViewTableGroupButtonCell(ASPxGridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter, bool isGroupButtonLive)
			: base(renderHelper, true) {
			this.visibleIndex = visibleIndex;
			this.isGroupButtonLive = isGroupButtonLive;
			this.hasGroupFooter = hasGroupFooter;
		}
		public int VisibleIndex { get { return visibleIndex; } }
		protected bool IsGroupButtonLive { get { return isGroupButtonLive; } }
		protected bool HasGroupFooter { get { return hasGroupFooter; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected bool IsRowExpanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
		protected override bool GetDefaultRemoveBottomBorder() {
			return IsRowExpanded || HasGroupFooter; 
		}
		protected bool Accessible { get { return RenderHelper.Grid.IsAccessibilityCompliantRender(true); } }
		protected override void CreateControlHierarchy() {
			Image image = RenderUtils.CreateImage();
			string imageName = IsRowExpanded ? GridViewImages.ExpandedButtonName : GridViewImages.CollapsedButtonName;
			RenderHelper.AssignImageToControl(imageName, image);
			HyperLink link = null;
			if(Accessible) {
				link = RenderUtils.CreateHyperLink();
				Controls.Add(link);
				link.Controls.Add(image);
			} else {
				Controls.Add(image);
			}
			if(IsGroupButtonLive) {
				string js = IsRowExpanded ? Scripts.GetCollapseRowFunction(VisibleIndex) : Scripts.GetExpandRowFunction(VisibleIndex);
				if(link != null) {
					link.NavigateUrl = string.Format("javascript:{0}", js);
					RenderUtils.SetStringAttribute(link, "onclick", ASPxGridViewRenderHelper.CancelBubbleJs);
				} else {
					RenderUtils.SetStringAttribute(image, "onclick", js + ";" + ASPxGridViewRenderHelper.CancelBubbleJs);
					if(RenderHelper.IsGridEnabled)
						RenderUtils.SetCursor(image, RenderUtils.GetPointerCursor());
				}
			}						
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(.1d);
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	[ViewStateModeById]
	public class GridTemplateTableCell : InternalTableCell {
	}
	public class GridViewTablePreviewRow : GridViewTableRow {
		public GridViewTablePreviewRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Preview; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetPreviewRowId(VisibleIndex);
			CreateIndentCells();
			if(RenderHelper.AddPreviewRowTemplateControl(VisibleIndex, this, Columns.Count)) return;
			Cells.Add(CreatePreviewCell());
		}
		protected virtual TableCell CreatePreviewCell() {
			TableCell cell = RenderUtils.CreateTableCell();
			cell.ColumnSpan = Columns.Count;
			string text = RenderHelper.TextBuilder.GetPreviewText(VisibleIndex);
			if(string.IsNullOrEmpty(text)) text = "&nbsp;";
			cell.Text = text;
			RenderHelper.AppendDefaultDXClassName(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPreviewRowStyle().AssignToControl(this, true);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected override TableCell CreateIndentTableCell(bool isLastInLevel) {
			return new GridViewTableEmptyIndentCell(RenderHelper);
		}
	}
	public class GridViewTableEmptyDataRow : GridViewTableRow {
		GridViewCommandColumnButtonControl newButtonControl;
		WebControl textContainer;
		public GridViewTableEmptyDataRow(ASPxGridViewRenderHelper renderHelper, GridViewLastRowBottomBorder lastRowBottomBorder) : base(renderHelper, -1, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.EmptyDataRow; } }
		protected GridViewCommandColumnButtonControl NewButtonControl { get { return newButtonControl; } }
		protected WebControl TextContainer { get { return textContainer; } }
		protected override void CreateControlHierarchy() {
			CreateIndentCells();
			if(RenderHelper.AddEmptyDataRowTemplateControl(this, Columns.Count)) return;
			Cells.Add(CreateEmptyDataCell());
		}
		protected virtual TableCell CreateEmptyDataCell() {
			TableCell cell = new GridViewTableCellEx(RenderHelper, false);
			cell.ColumnSpan = Columns.Count;
			GridViewCommandColumnButton newButton = RenderHelper.GetVisibleNewRowButton();
			if(newButton != null) {
				this.newButtonControl = new GridViewCommandColumnButtonControl(newButton, RenderHelper.Scripts.GetAddNewRowFunction, Grid.IsEnabled(), -1);
				cell.Controls.Add(NewButtonControl);
			}
			this.textContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.TextContainer.Controls.Add(RenderUtils.CreateLiteralControl(Grid.SettingsText.GetEmptyDataRow()));
			cell.Controls.Add(TextContainer);
			RenderHelper.AppendDefaultDXClassName(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			if(NewButtonControl != null)
				RenderHelper.GetCommandColumnItemStyle(RenderHelper.GetVisibleNewRowButton().Column).AssignToControl(NewButtonControl);
			GridViewDataRowStyle emptyDataRowStyle = RenderHelper.GetEmptyDataRowStyle();
			emptyDataRowStyle.AssignToControl(this, true);
			if(TextContainer != null) {
				GridViewDataRowStyle textContainerStyle = new GridViewDataRowStyle();
				textContainerStyle.ForeColor = emptyDataRowStyle.ForeColor;
				textContainerStyle.HorizontalAlign = emptyDataRowStyle.HorizontalAlign;
				textContainerStyle.AssignToControl(this.textContainer);
			}
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTablePagerEmptyCell : GridViewTableBaseCell {
		public GridViewTablePagerEmptyCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewColumn column, int visibleIndex, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, false, removeRightBorder) {
		}
		protected override void CreateControlHierarchy() {
			Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetCellStyle(Column, VisibleIndex).AssignToControl(this, true);
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTablePagerEmptyRow : GridViewTableRow {
		public GridViewTablePagerEmptyRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) : base(renderHelper, visibleIndex, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.PagerEmptyRow; } }
		protected override void CreateControlHierarchy() {
			CreateIndentCells();
			for(int i = 0; i < Columns.Count; i ++) {
				Cells.Add(CreateEmptyDataCell(Columns[i], i == Columns.Count - 1));
			}
		}
		protected virtual TableCell CreateEmptyDataCell(GridViewColumn column, bool removeRightBorder) {
			return new GridViewTablePagerEmptyCell(RenderHelper, RenderHelper.DataProxy, column, VisibleIndex, removeRightBorder);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDataRowStyle(VisibleIndex, VisibleIndex).AssignToControl(this, true);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTableDetailRow : GridViewTableRow {
		public GridViewTableDetailRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Detail; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetDetailRowId(VisibleIndex);
			CreateIndentCells();
            // try-catch is changed by newtera to prevent a bug
            try
            {
                RenderHelper.AddDetailRowTemplateControl(VisibleIndex, this, Columns.Count);
            }
            catch (Exception)
            {
            }
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDetailRowStyle().AssignToControl(this, false);
			TableCell cell = Cells[Cells.Count - 1];
			RenderHelper.GetDetailCellStyle().AssignToControl(cell, true);
			if(LastRowBottomBorder == GridViewLastRowBottomBorder.LastRowRemoveBorder)
				cell.Style["border-bottom"] = "0"; 
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected override TableCell CreateIndentTableCell(bool isLastInLevel) {
			return new GridViewTableEmptyIndentCell(RenderHelper);
		}
	}
	public class GridViewTableDataRow : GridViewTableGroupAndDataRow, IValueProvider {
		int dataRowIndex;
		public GridViewTableDataRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, int dataRowIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: this(renderHelper, visibleIndex, dataRowIndex, lastRowBottomBorder, false) { }
		public GridViewTableDataRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, int dataRowIndex, GridViewLastRowBottomBorder lastRowBottomBorder, bool isStyledRow)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) {
			this.dataRowIndex = dataRowIndex;
			IsStyledRow = isStyledRow;
		}
		public override GridViewRowType RowType { get { return GridViewRowType.Data; } }
		protected override string ContextMenuType { get { return "row"; }  }
		protected override bool RequireRenderParentRows { get { return true; } }
		protected int DataRowIndex { get { return dataRowIndex; } }
		protected bool IsSelected { get { return DataProxy.Selection.IsRowSelected(VisibleIndex); } }
		protected bool IsFocused { get { return DataProxy.IsRowFocused(VisibleIndex); } }
		protected bool HasDetailButton { get { return RenderHelper.HasDetailButton; } }
		protected bool IsDetailButtonExpanded { get { return RenderHelper.HasDetailRow(VisibleIndex); } } 
		protected override void CreateControlHierarchy() {
			if(IsStyledRow) return;
			CreateIndentCells();
			ID = RenderHelper.GetDataRowId(VisibleIndex);
			if(RenderHelper.AddDataRowTemplateControl(VisibleIndex, this, Columns.Count)) return;
			for(int i = 0; i < Columns.Count; i++) {
				Cells.Add(CreateContentCell(Columns[i], i));
			}
			if(Columns.Count == 0) {
				Cells.Add(new GridViewTableEmptyIndentCell(RenderHelper));
			}
		}
		protected override TableCell CreateDetailButtonCell() {
			return new GridViewTableDetailButtonCell(RenderHelper, VisibleIndex, IsDetailButtonExpanded);
		}
		protected override void PrepareControlHierarchy() {
			if((IsSelected || IsFocused) && !IsStyledRow) {
				if(IsFocused)
					RenderHelper.GetFocusedRowStyle().AssignToControl(this, true);
				else RenderHelper.GetSelectedRowStyle().AssignToControl(this, true);
			} else {
				RenderHelper.GetDataRowStyle(VisibleIndex, DataRowIndex).AssignToControl(this, true);
			}
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		TableCell CreateContentCell(GridViewColumn column, int index) {
			return RenderHelper.CreateContentCell(this, column, index, VisibleIndex, Columns.Count);
		}
		object IValueProvider.GetValue(string fieldName) { return DataProxy.GetRowValue(VisibleIndex, fieldName); }
	}
	public class GridViewTableVerticalScrollingEmptyRow : GridViewTableRow {
		public GridViewTableVerticalScrollingEmptyRow(ASPxGridViewRenderHelper renderHelper, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, -1, lastRowBottomBorder) {
		}
		public override GridViewRowType RowType { get { return GridViewRowType.EmptyDataRow; } } 
		protected override void CreateControlHierarchy() {
			CreateIndentCells();
			for(int i = 0; i < Columns.Count; i++) {
				Cells.Add(new GridViewTableVerticalScrollingEmptyCell(RenderHelper, Columns[i]));
			}
		}
		protected override TableCell CreateDetailButtonCell() {
			return new GridViewTableVerticalScrollingEmptyCell(RenderHelper, null);
		}
		protected override TableCell CreateIndentTableCell(bool isLastInLevel) {
			return new GridViewTableVerticalScrollingEmptyCell(RenderHelper, null);
		}
		protected override void PrepareControlHierarchy() {
		}
	}
	public class GridViewTableVerticalScrollingEmptyCell : GridViewTableCellEx {
		GridViewColumn column;
		WebControl div;
		public GridViewTableVerticalScrollingEmptyCell(ASPxGridViewRenderHelper renderHelper, GridViewColumn column)
			: base(renderHelper) {
			this.column = column;
		}
		public GridViewColumn Column { get { return column; } }
		protected WebControl Div { get { return div; } }
		protected override void CreateChildControls() {
			this.div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(Div);
		}
		protected override void PrepareControlHierarchy() {
			if(Div == null) return;
			if(!RenderUtils.IsIE && Column != null && RenderHelper.AllowColumnResizing) {
				Div.Width = Column.Width;
			}
		}
	}
	public class GridViewTableIndentDataCellBase : GridViewTableCellEx {
		public GridViewTableIndentDataCellBase(ASPxGridViewRenderHelper renderHelper, int columnSpan)
			: this(renderHelper, columnSpan, true) {
		}
		public GridViewTableIndentDataCellBase(ASPxGridViewRenderHelper renderHelper, int columnSpan, bool removeRigthBorder)
			: base(renderHelper, removeRigthBorder) {
			if(columnSpan > 1)
				ColumnSpan = columnSpan;
		}
		protected override void CreateControlHierarchy() {
			Text = "&nbsp;";
		}
		protected virtual void AddIndentCellClassName() {
			RenderHelper.AppendIndentCellClassName(this);
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Pixel(0);
			AddIndentCellClassName();
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableIndentDataCell : GridViewTableIndentDataCellBase {
		bool isLastInLevel;
		public GridViewTableIndentDataCell(ASPxGridViewRenderHelper renderHelper, bool isLastInLevel)
			: base(renderHelper, 1) {
			this.isLastInLevel = isLastInLevel;
		}
		protected bool IsLastInLevel { get { return isLastInLevel; } }
		protected override bool GetRemoveRightBorder() { return false; }
		protected override bool GetDefaultRemoveBottomBorder() { return !IsLastInLevel; }
	}
	public class GridViewTableEmptyIndentCell : GridViewTableIndentDataCellBase {
		public GridViewTableEmptyIndentCell(ASPxGridViewRenderHelper renderHelper)
			: base(renderHelper, 1) { }
		protected override bool GetDefaultRemoveBottomBorder() { return false; }
	}
	public class GridViewTableFooterIndentCell : GridViewTableIndentDataCellBase {
		public GridViewTableFooterIndentCell(ASPxGridViewRenderHelper renderHelper, int columnSpan) : base(renderHelper, columnSpan) { }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFooterCellStyle(null).AssignToControl(this, true); 
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableGroupFooterIndentDataCell : GridViewTableIndentDataCellBase {
		public GridViewTableGroupFooterIndentDataCell(ASPxGridViewRenderHelper renderHelper) : base(renderHelper, 1, false) {  }
		protected override bool GetDefaultRemoveBottomBorder() { return false; }
	}
	public class GridViewTableGroupFooterIndentCell : GridViewTableIndentDataCellBase {
		public GridViewTableGroupFooterIndentCell(ASPxGridViewRenderHelper renderHelper, int columnSpan) : base(renderHelper, columnSpan) { }
		protected override void AddIndentCellClassName() {
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetGroupFooterCellStyle(null).AssignToControl(this, true);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableInvisibleParentsRowDataCell : GridViewTableIndentDataCellBase {
		Image image;
		public GridViewTableInvisibleParentsRowDataCell(ASPxGridViewRenderHelper renderHelper) : base(renderHelper, 1) { }
		protected Image Image { get { return image; } }
		protected override bool GetDefaultRemoveBottomBorder() { return true; }
		protected override bool GetRemoveRightBorder() { return false; }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetParentRowsId();
			this.image = RenderUtils.CreateImage();
			Controls.Add(image);
		}
		protected override void PrepareControlHierarchy() {
			Attributes["onmouseover"] = Scripts.GetShowParentRowsWindowFunction();
			Attributes["onmouseout"] = Scripts.GetHideParentRowsWindowFunction(false);
			RenderHelper.AssignImageToControl(GridViewImages.ParentGroupRowsName, Image);
			Style[HtmlTextWriterStyle.Padding] = Unit.Pixel(0).ToString();
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableDetailButtonCell : GridViewTableIndentDataCellBase {
		int visibleIndex = -1;
		bool isDetailButtonExpanded = false;
		public GridViewTableDetailButtonCell(ASPxGridViewRenderHelper renderHelper, int visibleIndex, 
			bool isDetailButtonExpanded)
			: base(renderHelper, 1) {
			this.visibleIndex = visibleIndex;
			this.isDetailButtonExpanded = isDetailButtonExpanded;
		}
		protected int VisibleIndex { get { return visibleIndex; } }
		protected bool IsDetailButtonExpanded { get { return isDetailButtonExpanded; } }
		protected object KeyValue { get { return RenderHelper.DataProxy.GetRowKeyValue(VisibleIndex); } }
		protected bool Accessible { get { return RenderHelper.Grid.IsAccessibilityCompliantRender(true); } }
		protected override void AddIndentCellClassName() { }
		protected override void CreateControlHierarchy() {
			Image image = RenderUtils.CreateImage();
			string imageName = IsDetailButtonExpanded ? GridViewImages.DetailExpandedButtonName : GridViewImages.DetailCollapsedButtonName;
			RenderHelper.AssignImageToControl(imageName, image);
			HyperLink link = null;
			if(Accessible) {
				link = RenderUtils.CreateHyperLink();
				Controls.Add(link);
				link.Controls.Add(image);
			} else {
				Controls.Add(image);
			}
			GridViewDetailRowButtonState buttonState = GetButtonState();
			if(buttonState == GridViewDetailRowButtonState.Visible) {
				string js = IsDetailButtonExpanded ? Scripts.GetHideDetailRowFunction(VisibleIndex) : Scripts.GetShowDetailRowFunction(VisibleIndex);
				if(link != null) {
					link.NavigateUrl = string.Format("javascript:{0}", js);
					RenderUtils.SetStringAttribute(link, "onclick", ASPxGridViewRenderHelper.CancelBubbleJs);
				} else {
					RenderUtils.SetStringAttribute(image, "onclick", js + ";" + ASPxGridViewRenderHelper.CancelBubbleJs);
					if(RenderHelper.IsGridEnabled)
						RenderUtils.SetCursor(image, RenderUtils.GetPointerCursor());
				}
			} else {
				WebControl control = link != null ? (WebControl)link : image;
				control.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			}			
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDetailButtonStyle().AssignToControl(this, true);
			base.PrepareControlHierarchy();			
			if(RenderHelper.RequireFixedTableLayout) {
				Style[HtmlTextWriterStyle.Overflow] = "visible";
				Width = Unit.Empty;
			}
		}
		protected GridViewDetailRowButtonState GetButtonState() {
			ASPxGridViewDetailRowButtonEventArgs args = new ASPxGridViewDetailRowButtonEventArgs(VisibleIndex, KeyValue, IsDetailButtonExpanded);
			RenderHelper.Grid.RaiseDetailRowGetButtonVisibility(args);
			return args.ButtonState;
		}
	}
	public class GridViewTableBaseCell : GridViewTableCellEx {
		GridViewColumn column;
		int visibleIndex;
		WebDataProxy dataProxy;
		bool removeLeftBorder;
		public GridViewTableBaseCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, removeRightBorder) {
			this.dataProxy = dataProxy;
			this.column = column;
			this.visibleIndex = visibleIndex;
			this.removeLeftBorder = removeLeftBorder;
		}
		protected WebDataProxy DataProxy { get { return dataProxy; } }
		public GridViewColumn Column { get { return column; } }
		public int VisibleIndex { get { return visibleIndex; } }
		protected bool RemoveLeftBorder { get { return removeLeftBorder; } }
		protected virtual bool GetRemoveLeftBorder() { return RemoveLeftBorder || !RenderHelper.ShowVerticalGridLine; }
		protected override bool GetRemoveRightBorder() { return RemoveRightBorder || !RenderHelper.ShowVerticalGridLine; }
		protected override void PrepareControlHierarchy() {
			if(GetRemoveLeftBorder()) RenderUtils.SetStyleUnitAttribute(this, "border-left-width", 0);
			base.PrepareControlHierarchy();
			if(HasToSetWidth())
				Width = Column.Width;
		}
		bool HasToSetWidth() {
			return !RenderHelper.AllowColumnResizing
				&& VisibleIndex == DataProxy.VisibleStartIndex
				&& !RenderHelper.Grid.Settings.ShowColumnHeaders;
		}
	}
	public class GridViewTableUpdateCancelCell : GridViewTableCell {
		public class GridViewUpdateCancelCellCommandColumn : GridViewCommandColumn {
			ASPxGridView grid;
			public GridViewUpdateCancelCellCommandColumn(ASPxGridView grid) {
				this.grid = grid;
			}
			public override ASPxGridView Grid { get { return grid; } }
		}
		public GridViewTableUpdateCancelCell(ASPxGridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(CreateUpdateButton());
			Unit spacing = RenderHelper.Styles.CommandColumn.Spacing;
			if(!spacing.IsEmpty)
				Controls.Add(new GridViewCommandColumnSpacer(spacing));			
			Controls.Add(CreateCancelButton());
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected bool IsGridEnabled { get { return Grid.IsEnabled(); } }
		public Control CreateUpdateButton() {
			GridViewCommandColumn commandColumn = GetCommandColumn();
			return new GridViewCommandColumnButtonControl(commandColumn.UpdateButton, Scripts.GetUpdateEditFunction, IsGridEnabled, -1);
		}
		public Control CreateCancelButton() {
			GridViewCommandColumn commandColumn = GetCommandColumn();
			return new GridViewCommandColumnButtonControl(commandColumn.CancelButton, Scripts.GetCancelEditFunction, IsGridEnabled, -1);
		}
		protected GridViewCommandColumn GetCommandColumn() {
			GridViewCommandColumn result = null;
			foreach(GridViewColumn column in Grid.Columns) {
				GridViewCommandColumn commandColumn = column as GridViewCommandColumn;
				if(commandColumn != null) {
					if(commandColumn.UpdateButton.Visible)
						return commandColumn;
					result = commandColumn;
				}
			}
			if(result == null) {
				result = new GridViewUpdateCancelCellCommandColumn(Grid);
			}
			return result;
		}
		protected override void PrepareControlHierarchy() {
			GridViewCommandColumnStyle style = RenderHelper.GetUpdateCancelButtonsStyle();
			style.AssignToControl(this, true);
			HorizontalAlign = HorizontalAlign.Right;
			base.PrepareControlHierarchy();
		}
	}
	public delegate string GridViewCommandColumnButtonControlFunc(string id, int visibleIndex);
	[ToolboxItem(false)]
	public class GridViewCommandColumnButtonControl : ASPxWebControlBase {
		GridViewCommandColumn column;
		string text;
		string buttonID;
		ImageProperties buttonImage;
		GridViewCommandColumnButtonControlFunc func;
		bool isGridEnabled;
		int visibleIndex;
		WebControl control;
		public GridViewCommandColumnButtonControl(GridViewCommandColumnButton button, GridViewCommandColumnButtonControlFunc func, bool isGridEnabled, int visibleIndex)
			: this(button.Column, string.Empty, button.GetText(), button.Image, func, isGridEnabled, visibleIndex) { }
		public GridViewCommandColumnButtonControl(GridViewCommandColumnCustomButton button, GridViewCommandColumnButtonControlFunc func, bool isGridEnabled, int visibleIndex)
			: this(button.Column, button.GetID(), button.GetText(), button.Image, func, isGridEnabled, visibleIndex) { }
		public GridViewCommandColumnButtonControl(GridViewCommandColumn column,  string buttonID, string text, ImageProperties buttonImage, GridViewCommandColumnButtonControlFunc func, bool isGridEnabled, int visibleIndex){
			this.column = column;
			this.buttonID = buttonID;
			this.text = text;
			this.buttonImage = buttonImage;
			this.func = func;
			this.isGridEnabled = isGridEnabled;
			this.visibleIndex = visibleIndex;
			this.EnableViewState = false;
		}
		protected WebControl Control { get { return control; } }
		protected HyperLink HyperLink { get { return Control as HyperLink; } }
		protected override object SaveViewState() { return null; }
		public GridViewCommandColumnButtonControlFunc Func { get { return func; } }
		protected bool IsGridEnabled { get { return isGridEnabled; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected GridViewCommandColumn Column { get { return column; } }
		protected ImageProperties ButtonImage { get { return buttonImage; } }
		protected bool IsGridDesignMode() { return Column.Grid != null ? Column.Grid.DesignMode : false; }
		protected ButtonType ButtonType { get { return Column.ButtonType; } }
		protected string ButtonID { get { return buttonID; } }
		protected string NavigateUrl { get { return "javascript:" + Func(ButtonID, VisibleIndex); } }
		protected string OnClick {
			get {
				return ASPxGridViewRenderHelper.CancelBubbleJs + ";" + Func(ButtonID, VisibleIndex) + "return false;"; 
			}
		}
		public string Text { 
			get { 
				string res = text;
				if(!IsGridEnabled && ButtonType == ButtonType.Link) {
					res += "&nbsp;";
				}
				return res;
			} 
		}
		protected override void CreateControlHierarchy() {
			this.control = CreateCommandControl();
			Control.Enabled = IsGridEnabled;
			Controls.Add(Control);
		}
		protected virtual WebControl CreateCommandControl() {
			if(ButtonType == ButtonType.Button)
				return CreateButtonControl();
			if(ButtonType == ButtonType.Image)
				return CreateImageControl();
			return CreateLinkControl();
		}
		protected virtual WebControl CreateButtonControl() {
			WebControl control = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			control.Attributes["type"] = "button";
			control.Attributes["value"] = Text;
			return control;
		}
		protected virtual WebControl CreateImageControl() {
			Image image = RenderUtils.CreateImage();
			ButtonImage.AssignToControl(image, IsGridDesignMode());
			if(string.IsNullOrEmpty(image.ToolTip)) {
				image.ToolTip = Text;
			}
			return image;
		}
		protected virtual WebControl CreateLinkControl() {
			HyperLink link = RenderUtils.CreateHyperLink();
			link.Text = Text; 
			return link;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(HyperLink != null) {
				HyperLink.NavigateUrl = NavigateUrl;
			} else {
				Control.Attributes["onclick"] = OnClick;
			}
		}
		protected internal void AssignInnerControlStyle(AppearanceStyleBase style) {
			style.AssignToControl(Control, true);
		}
	}
	[ToolboxItem(false)]
	public class GridViewCommandColumnSpacer : WebControl {		
		public GridViewCommandColumnSpacer(Unit width) 
			: base(HtmlTextWriterTag.Span) {
			RenderUtils.SetHorizontalMargins(this, Unit.Empty, width);
			Font.Size = FontUnit.Parse("1px");
			Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
	}
	public abstract class GridViewTableBaseCommandCell : GridViewTableBaseCell {
		public GridViewTableBaseCommandCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public new GridViewCommandColumn Column { get { return base.Column as GridViewCommandColumn; } }
		public abstract GridViewTableCommandCellType CellType { get; }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected bool IsGridEnabled { get { return Grid.IsEnabled(); } }
		protected bool IsRowEditing { get { return RenderHelper.DataProxy.IsRowEditing(VisibleIndex); } }
		protected override void PrepareControlHierarchy() {
			GridViewCommandColumnStyle style = RenderHelper.GetCommandColumnStyle(Column);
			style.AssignToControl(this, true);
			GridViewCommandColumnStyle itemStyle = RenderHelper.GetCommandColumnItemStyle(Column);
			foreach(WebControl control in Controls) {
				if(control is GridViewCommandColumnSpacer) continue;
				GridViewCommandColumnButtonControl button = control as GridViewCommandColumnButtonControl;
				if(button != null)
					button.AssignInnerControlStyle(itemStyle);
				else
				itemStyle.AssignToControl(control, true);
			}
			RenderHelper.AppendDefaultDXClassName(this);
			Grid.RaiseHtmlCommandCellPrepared(this);
			base.PrepareControlHierarchy();
		}
		protected void CreateCustomCommands() {
			foreach(GridViewCommandColumnCustomButton button in Column.CustomButtons) {
				CreateCommand(button, Scripts.GetCustomButtonFunction);
			}
		}
		protected void CreateCommand(GridViewCommandColumnButton button, GridViewCommandColumnButtonControlFunc func) {
			CreateCommand(button, func, false);
		}
		protected void CreateCommand(GridViewCommandColumnCustomButton button, GridViewCommandColumnButtonControlFunc func) {
			ASPxGridViewCustomButtonEventArgs e = new ASPxGridViewCustomButtonEventArgs(button, VisibleIndex, CellType, IsRowEditing);
			Grid.RaiseCustomButtonInitialize(e);
			switch(e.IsVisible) {
				case DefaultBoolean.False:
					return;
				case DefaultBoolean.Default:
					if (!button.IsVisible(CellType, IsRowEditing)) return;
					break;
			}			
			CreateSpacerIfNeeded();
			Controls.Add(new GridViewCommandColumnButtonControl(button, func, IsGridEnabled, VisibleIndex));
		}
		protected virtual void CreateCommand(GridViewCommandColumnButton button, GridViewCommandColumnButtonControlFunc func, bool createInAnyWay) {
			if(!createInAnyWay && !button.Visible) return;
			ASPxGridViewCommandButtonEventArgs e = new ASPxGridViewCommandButtonEventArgs(button, VisibleIndex, IsRowEditing);
			Grid.RaiseCommandButtonInitialize(e);
			if (!e.Visible) return;
			CreateSpacerIfNeeded();
			Controls.Add(new GridViewCommandColumnButtonControl(button, func, IsGridEnabled && e.Enabled, VisibleIndex));
		}
		void CreateSpacerIfNeeded() {
			if(Controls.Count < 1) return;
			Unit spacing = RenderHelper.GetCommandColumnStyle(Column).Spacing;
			if(!spacing.IsEmpty)
				Controls.Add(new GridViewCommandColumnSpacer(spacing));
		}
	}
	public class GridViewTableFilterRowCommandCell : GridViewTableBaseCommandCell {
		public GridViewTableFilterRowCommandCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public override GridViewTableCommandCellType CellType { get { return GridViewTableCommandCellType.Filter; } }
		protected override void CreateControlHierarchy() {
			if(!string.IsNullOrEmpty(Grid.FilterExpression)) {
				CreateCommand(Column.ClearFilterButton, Scripts.GetClearFilterFunction);
			}
			CreateCustomCommands();
			if(Controls.Count == 0) Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			HorizontalAlign = HorizontalAlign.Center;
		}
	}
	public class GridViewTableCommandCell : GridViewTableBaseCommandCell {
		public GridViewTableCommandCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public override GridViewTableCommandCellType CellType { get { return GridViewTableCommandCellType.Data; } }
		protected bool IsEditorButton {
			get { return Column.EditButton.Visible || Column.NewButton.Visible || RenderHelper.CommandColumnsCount == 1; }
		}
		protected override void CreateControlHierarchy() {
			bool isRowEditing = DataProxy.IsRowEditing(VisibleIndex);
			if(isRowEditing) {
				if(IsEditorButton && RenderHelper.Grid.SettingsEditing.Mode == GridViewEditingMode.Inline) {
					CreateUpdateCancel();
				}
				CreateCustomCommands(); 
				if(Controls.Count == 0) Text = "&nbsp;";
				return;
			}
			CreateCommand(Column.EditButton, Scripts.GetStartEditFunction);
			CreateCommand(Column.NewButton, Scripts.GetAddNewRowFunction);
			CreateCommand(Column.DeleteButton, Scripts.GetDeleteRowFunction);
			CreateCommand(Column.SelectButton, Scripts.GetSelectRowFunction);
			CreateCustomCommands();
			if(Column.ShowSelectCheckbox) CreateSelectCheckbox();
			if(Controls.Count == 0) Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			HorizontalAlign = HorizontalAlign.Center;
		}
		protected virtual void CreateUpdateCancel() {
			CreateCommand(Column.UpdateButton, Scripts.GetUpdateEditFunction, true);
			CreateCommand(Column.CancelButton, Scripts.GetCancelEditFunction, true);
		}
		protected virtual void CreateSelectCheckbox() {
			WebControl check = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			check.ID = RenderHelper.GetSelectButtonId(VisibleIndex);
			check.Attributes["type"] = "checkbox";
			check.Attributes["onclick"] = Scripts.GetSelectRowFunction(VisibleIndex);
			check.Enabled = IsGridEnabled;
			if(DataProxy.Selection.IsRowSelected(VisibleIndex)) {
				check.Attributes["checked"] = "T";
			}
			Controls.Add(check);
		}
	}
	public class GridViewTableDataCell : GridViewTableBaseCell {
		GridViewTableDataRow row;
		WebControl fitting;
		public GridViewTableDataCell(ASPxGridViewRenderHelper renderHelper, GridViewTableDataRow row, WebDataProxy dataProxy, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) {
			this.row = row;
		}
		public ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected GridViewTableDataRow Row { get { return row; } }
		protected override void CreateControlHierarchy() {			
			if(IsFittingRequired()) {
				this.fitting = new WebControl(HtmlTextWriterTag.Div);
				Controls.Add(this.fitting);			
			}
			if(!RenderHelper.AddDataItemTemplateControl(VisibleIndex, DataColumn, this)) {
				RenderHelper.AddDisplayControlToDataCell(this, DataColumn, VisibleIndex, Row);
			}
		}
		public GridViewDataColumn DataColumn { get { return base.Column as GridViewDataColumn; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetCellStyle(DataColumn, VisibleIndex).AssignToControl(this, true);
			PrepareFitting();
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlDataCellPrepared(this);
		}
		void PrepareFitting() {
			if(this.fitting == null) return;
			this.fitting.Width = DataColumn.Width;
			this.fitting.Height = 0;			
			this.fitting.Style[HtmlTextWriterStyle.FontSize] = "0";
		}
		bool IsFittingRequired() {			
			if(VisibleIndex != Grid.VisibleStartIndex)
				return false;
			return RenderHelper.ShowHorizontalScrolling;
		}
	}
	[ViewStateModeById]
	public class GridViewGroupPanel : ASPxInternalWebControl {
		List<GridViewDataColumn> columns;
		List<TableCell> groupPanelColumnIndents;
		ASPxGridViewRenderHelper renderHelper;
		public GridViewGroupPanel(ASPxGridViewRenderHelper renderHelper, List<GridViewDataColumn> columns){
			this.renderHelper = renderHelper;
			this.columns = columns;
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected ASPxGridViewTextSettings SettingsText { get { return Grid.SettingsText; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() { return true; }
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected int GroupCount { get { return RenderHelper.GroupCount; } }
		protected List<GridViewDataColumn> Columns { get { return columns; } }
		protected override void CreateControlHierarchy() {
			if(GroupCount > 0) {
				CreateGroupHeaders();
			} else {
				ID = RenderHelper.GetGroupPanelId();
				LiteralControl textControl = new LiteralControl();
				Controls.Add(textControl);
				textControl.Text = GroupPanelText;
			}
		}
		protected string GroupPanelText { get {   return SettingsText.GetGroupPanel(); } }
		protected virtual void CreateGroupHeaders() {
			this.groupPanelColumnIndents = new List<TableCell>();
			Table table = RenderUtils.CreateTable();
			if(RenderUtils.IsSafariFamily)
				table.Width = Unit.Percentage(100); 
			this.Controls.Add(table);
			table.GridLines = GridLines.None;
			table.BorderStyle = BorderStyle.None;
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			for(int i = 0; i < GroupCount; i++) {
				row.Cells.Add(new GridViewTableHeaderCell(RenderHelper, Columns[i], GridViewHeaderLocation.Group, false));
				if(i < GroupCount - 1) {
					TableCell cell = RenderUtils.CreateTableCell();
					cell.Text = "&nbsp;";
					row.Cells.Add(cell);
					this.groupPanelColumnIndents.Add(cell);
				}
			}
			TableCell groupCell = RenderUtils.CreateTableCell();
			row.Cells.Add(groupCell);
			groupCell.ID = RenderHelper.GetGroupPanelId();
			groupCell.Width = Unit.Percentage(100);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyle groupStyle = RenderHelper.GetGroupPanelStyle();
			groupStyle.AssignToControl(this, true);
			if(this.groupPanelColumnIndents != null) {
				foreach(TableCell cell in this.groupPanelColumnIndents) {
					cell.Width = groupStyle.Spacing;
				}
			}
		}
	}
	public class GridViewTableFilterRow : GridViewTableRow {
		public GridViewTableFilterRow(ASPxGridViewRenderHelper renderHelper) : base(renderHelper, -1, GridViewLastRowBottomBorder.RequireBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Filter; } }
		protected override void CreateControlHierarchy() {
			CreateIndentCells();
			for(int i = 0; i < Columns.Count; i++) {
				TableCell cell = CreateFilterCell(Columns[i], i == Columns.Count - 1);
				this.Cells.Add(cell);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFilterRowStyle().AssignToControl(this, true);
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected virtual TableCell CreateFilterCell(GridViewColumn column, bool removeRightBorder) {
			if(column is GridViewCommandColumn) {
				return new GridViewTableFilterRowCommandCell(RenderHelper, DataProxy, column as GridViewCommandColumn, 0, false, removeRightBorder);
			}
			return new GridViewTableFilterEditorCell(RenderHelper, removeRightBorder, column);
		}
	}
	#region OldFilterCell
	#endregion OldFilterCell
	public class GridViewTableFilterEditorCell : GridViewTableCellEx {
		ASPxEditBase editor;
		GridViewColumn column;
		TableCell editorCell;
		Image menuImage;
		public GridViewTableFilterEditorCell(ASPxGridViewRenderHelper renderHelper, bool removeRightBorder, GridViewColumn column)
			: base(renderHelper, removeRightBorder) {
			this.column = column;
		}
		protected GridViewColumn Column { get { return column; } }
		protected GridViewDataColumn DataColumn { get { return Column as GridViewDataColumn; } }
		protected ASPxGridView Grid { get { return Column.Grid; } }
		protected ASPxEditBase Editor { get { return editor; } }
		protected TableCell EditorCell { get { return editorCell; } }
		protected Image MenuImage { get { return menuImage; } }
		protected override void CreateControlHierarchy() {
			if(!Column.GetAllowAutoFilter()) {
				Text = "&nbsp;";
				return;
			}
			TableCell editContainer = this;
			if(RenderHelper.IsFilterRowMenuIconVisible(Column)) {
				Table table = RenderUtils.CreateTable();
				TableRow row = RenderUtils.CreateTableRow();
				this.editorCell = RenderUtils.CreateTableCell();
				TableCell imageCell = RenderUtils.CreateTableCell();
				Controls.Add(table);
				table.Rows.Add(row);				
				row.Cells.Add(EditorCell);
				row.Cells.Add(imageCell);
				this.menuImage = RenderUtils.CreateImage();
				imageCell.Controls.Add(MenuImage);
				editContainer = this.editorCell;
			}		
			this.editor = RenderHelper.CreateAutoFilterEditor(editContainer, DataColumn, Grid.FilterHelper.GetColumnAutoFilterText(DataColumn), EditorInplaceMode.Inplace);
		}
		protected override void PrepareControlHierarchy() {
			if(DataColumn != null) {
				GridViewFilterCellStyle style = RenderHelper.GetFilterCellStyle(DataColumn);
				style.AssignToControl(this, true);
				if(Editor != null) {
					bool allowFilterTextTimer = DataColumn.Settings.AllowAutoFilterTextInputTimer != DefaultBoolean.False;
					Editor.Width = Unit.Percentage(100);
					EditClientSideEvents events = Editor.GetClientSideEvents() as EditClientSideEvents;
					TextEditClientSideEvents textEvents = Editor.GetClientSideEvents() as TextEditClientSideEvents;
					TextBoxClientSideEvents textBoxEvents = Editor.GetClientSideEvents() as TextBoxClientSideEvents;
					if(events != null) events.ValueChanged = string.Format("function(s, event) {{ {0}; }}", Scripts.GetFilterOnChangedFunction());
					if(!allowFilterTextTimer) textBoxEvents = null;
					string keyEvent = string.Format("function(s, event) {{ {0}; }}", (textBoxEvents != null ? Scripts.GetFilterOnKeyPressFunction() : Scripts.GetFilterOnSpecKeyPressFunction()));
					if(textEvents != null) {
						if(RenderUtils.IsOpera) {
							textEvents.KeyPress = keyEvent;
						} else {
							textEvents.KeyDown = keyEvent;
						}
					}
				}
				if(EditorCell != null) {
					EditorCell.Width = Unit.Percentage(100);
					Unit spacing = style.Spacing;
					if(spacing.IsEmpty) spacing = Unit.Pixel(2);
					RenderUtils.SetHorizontalPaddings(EditorCell, Unit.Empty, spacing);
				}
			}
			if(MenuImage != null) {
				RenderHelper.GetImage(GridViewImages.FilterRowButtonName).AssignToControl(MenuImage, DesignMode);
				RenderUtils.SetCursor(MenuImage, RenderUtils.GetPointerCursor());
				RenderUtils.SetStringAttribute(MenuImage, "onclick", Scripts.GetFilterRowMenuImageClick(Column.Index));
			}
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableFooterRow : GridViewTableRow {
		public GridViewTableFooterRow(ASPxGridViewRenderHelper renderHelper, GridViewLastRowBottomBorder lastRowBottomBorder) : base(renderHelper, -1, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Footer; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetFooterRowId();
			CreateIndentCells();
			if(RenderHelper.AddFooterRowTemplateControl(this, Columns.Count)) return;
			for(int i = 0; i < Columns.Count; i++) {
				TableCell cell = CreateFooterCell(Columns[i], i == Columns.Count - 1);
				this.Cells.Add(cell);
			}
		}
		protected virtual TableCell CreateFooterCell(GridViewColumn column, bool removeRightBorder) {
			return new GridViewTableFooterCell(RenderHelper, column, removeRightBorder);
		}
		protected override void CreateIndentCellsCore(int startIndex) {
			Cells.Add(CreateIndentTableCell(true));
		}
		protected override TableCell CreateIndentTableCell(bool isLastInLevel) {
			return new GridViewTableFooterIndentCell(RenderHelper, ColumIndentCount);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFooterStyle().AssignToControl(this, true);
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTableFooterCell : GridViewTableCellEx {
		GridViewColumn column;
		public GridViewTableFooterCell(ASPxGridViewRenderHelper renderHelper, GridViewColumn column, bool removeRightBorder)
			: base(renderHelper, removeRightBorder) {
			this.column = column;
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected GridViewColumn Column { get { return column; } }
		protected GridViewDataColumn DataColumn { get { return Column as GridViewDataColumn; } }
		protected override void CreateControlHierarchy() {
			WebControl parentControl = GetVerticalScrollingParentControl();
			if(!RenderHelper.AddFooterCellTemplateControl(Column, parentControl)) {
				string text = new ASPxGridViewTextBuilder(Grid).GetFooterCaption(DataColumn, "<br/>");
				LiteralControl literal = RenderUtils.CreateLiteralControl(string.IsNullOrEmpty(text) ? "&nbsp;" : text);
				parentControl.Controls.Add(literal);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFooterCellStyle(Column).AssignToControl(this, true);
			Grid.RaiseHtmlFooterCellPrepared(Column, -1, this);
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableGroupFooterCell : GridViewTableCellEx {
		GridViewColumn column;
		int visibleIndex;
		public GridViewTableGroupFooterCell(ASPxGridViewRenderHelper renderHelper, GridViewColumn column, int visibleIndex, bool removeRightBorder)
			: base(renderHelper, removeRightBorder) {
			this.column = column;
			this.visibleIndex = visibleIndex;
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected GridViewColumn Column { get { return column; } }
		protected GridViewDataColumn DataColumn { get { return Column as GridViewDataColumn; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected override void CreateControlHierarchy() {
			string text = new ASPxGridViewTextBuilder(Grid).GetGroupRowFooterText(DataColumn, VisibleIndex, "<br/>");
			Text = string.IsNullOrEmpty(text) ? "&nbsp;" : text;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetGroupFooterCellStyle(Column).AssignToControl(this, true);
			Grid.RaiseHtmlFooterCellPrepared(Column, VisibleIndex, this);
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableStatusCell : GridViewTableCell {
		Table mainTable;
		TableRow mainRow;
		TableCell cell;
		public GridViewTableStatusCell(ASPxGridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected Table MainTable { get { return mainTable; } }
		protected TableRow MainRow { get { return mainRow; } }
		protected TableCell Cell { get { return cell; } }
		protected override void CreateControlHierarchy() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			this.mainRow = RenderUtils.CreateTableRow();
			this.mainTable.Rows.Add(MainRow);
			this.cell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(Cell);
		}
		protected override void PrepareControlHierarchy() {
		}
	}
}
