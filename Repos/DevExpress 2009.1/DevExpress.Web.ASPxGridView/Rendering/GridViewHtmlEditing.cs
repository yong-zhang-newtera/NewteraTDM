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
using DevExpress.Web.ASPxPopupControl;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class GridViewTableInlineEditRow : GridViewTableRow {
		public GridViewTableInlineEditRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder) : base(renderHelper, visibleIndex, lastRowBottomBorder) { }
		public override GridViewRowType RowType { get { return GridViewRowType.InlineEdit; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetEditingRowId();
			CreateIndentCells();
			for(int i = 0; i < Columns.Count; i++) {
				TableCell cell = RenderHelper.CreateInlineEditorCell(Columns[i], i, VisibleIndex, Columns.Count);
				Cells.Add(cell);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetInlineEditRowStyle().AssignToControl(this, true);
			Grid.RaiseHtmlRowPrepared(this); 
		}
	}
	[ViewStateModeById]
	public class GridViewTableEditFormRow : GridViewTableRow {
		public GridViewTableEditFormRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, lastRowBottomBorder) {
		}
		public override GridViewRowType RowType { get { return GridViewRowType.EditForm; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetEditingRowId();
			CreateIndentCells();
			Cells.Add(new GridViewTableEditFormCell(RenderHelper, VisibleIndex));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetEditFormRowStyle().AssignToControl(this, true);
		}
	}
	public class GridViewTableEditFormCell : GridViewTableCellEx {
		int visibleIndex;
		public GridViewTableEditFormCell(ASPxGridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper) {
			this.visibleIndex = visibleIndex;
		}
		protected int VisibleIndex { get { return visibleIndex; } }
		protected override void CreateControlHierarchy() {
			ColumnSpan = RenderHelper.VisibleColumnCount;
			if(!RenderHelper.AddEditFormTemplateControl(this, VisibleIndex)) {
				Controls.Add(CreateMainTable());
			}
			RenderHelper.Grid.RaiseHtmlEditFormCreated(this);
		}
		protected Table CreateMainTable() {
			return new GridViewEditFormTable(RenderHelper, VisibleIndex);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewHtmlEditFormPopupContainer : InternalWebControl {
		public GridViewHtmlEditFormPopupContainer() : base(HtmlTextWriterTag.Div) { }
	}
	[ToolboxItem(false), ViewStateModeById]
	public class GridViewHtmlEditFormPopup : DevExpress.Web.ASPxPopupControl.ASPxPopupControl {
		ASPxGridView grid;
		int visibleIndex;
		Table maintable, errorTable;
		WebControl contentDiv;
		public GridViewHtmlEditFormPopup(ASPxGridView grid, int visibleIndex) : base(grid) {
			this.grid = grid;
			this.visibleIndex = visibleIndex;
			ShowOnPageLoad = false;
			EnableAnimation = false;
			ParentSkinOwner = Grid;
			Modal = Settings.PopupEditFormModal;
		}
		public string CloseUp {
			get { return ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp; }
			set { ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp = value; }
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewEditingSettings Settings { get { return grid.SettingsEditing; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected Table MainTable { get { return maintable; } }
		protected Table ErrorTable { get { return errorTable; } }
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected Unit WindowWidth { get { return Settings.PopupEditFormWidth; } }
		protected Unit WindowHeight { get { return Settings.PopupEditFormHeight; } }
		protected override Paddings GetContentPaddings(PopupWindow window) {
			Paddings paddings = new Paddings();
			paddings.Padding = Unit.Pixel(0);
			return paddings;
		}
		protected override void ClearControlFields() {
			this.errorTable = null;
			this.maintable = null;
			this.contentDiv = null;
		}
		protected override void EnsureChildControls() {
			base.EnsureChildControls();
			if(MainTable != null) {
				(MainTable as IASPxWebControl).EnsureChildControls();
			}
		}
		protected override void CreateControlHierarchy() {
			ShowHeader = Settings.PopupEditFormShowHeader;
			ID = RenderHelper.GetPopupEditFormId();
			Font.CopyFrom(Grid.Font);
			HeaderText = Grid.SettingsText.GetPopupEditFormCaption();
			AllowDragging = true;
			AllowResize = Settings.PopupEditFormAllowResize;
			CloseAction = CloseAction.CloseButton;
			base.CreateControlHierarchy();
			this.contentDiv = new GridViewHtmlEditFormPopupContainer();
			Controls.Clear();
			Controls.Add(ContentDiv);
			if(!RenderHelper.AddEditFormTemplateControl(ContentDiv, VisibleIndex, false)) {
				this.maintable = new GridViewEditFormTable(RenderHelper, VisibleIndex);
				ContentDiv.Controls.Add(MainTable);
			}
			CreateErrorTable();
			EnsureChildControlsRecursive(this, false);
			if(MainTable == null) { 
				ContentDiv.DataBind();
			}
			Grid.RaiseHtmlEditFormCreated(ContentDiv); 
		}
		protected void CreateErrorTable() {
			this.errorTable = new InternalTable();
			ContentDiv.Controls.Add(ErrorTable);
			ErrorTable.Width = Unit.Percentage(100);
			if(RenderHelper.HasEditingError) {
				GridViewTableRow row = new GridViewTableEditingErrorRow(RenderHelper, false, GridViewLastRowBottomBorder.LastRowRemoveBorder);
				ErrorTable.Rows.Add(row);
				Grid.RaiseHtmlRowCreated(row);
			} else {
				TableRow row = new InternalTableRow();
				row.ID = RenderHelper.GetEditingRowId();
				row.Cells.Add(new InternalTableCell());
				ErrorTable.Rows.Add(row);
			}
		}
		protected override void PrepareControlHierarchy() {
			CloseButtonImage.CopyFrom(Grid.Images.PopupEditFormWindowClose);
			ControlStyle.CopyFrom(Grid.Styles.PopupEditFormWindow);
			CloseButtonStyle.CopyFrom(Grid.Styles.PopupEditFormWindowCloseButton);
			HeaderStyle.CopyFrom(Grid.Styles.PopupEditFormWindowHeader);
			ContentStyle.CopyFrom(Grid.Styles.PopupEditFormWindowContent);
			base.PrepareControlHierarchy();
			if(!WindowWidth.IsEmpty) {
				Width = WindowWidth;
			}
			if(!WindowHeight.IsEmpty) {
				ContentDiv.Height = WindowHeight;
			}
			ClientSideEvents.Init = "function (s, e) { s.Show(); }";
			PopupElementID = GetPopupElementID();
			if(Grid.IsNewRowEditing && RenderHelper.DataProxy.VisibleRowCountOnPage < 1) {
				PopupHorizontalAlign = PopupHorizontalAlign.Center;
				PopupVerticalAlign = PopupVerticalAlign.Middle;
				PopupHorizontalOffset = 0;
				PopupVerticalOffset = 0;
			} else {
				PopupHorizontalAlign = Settings.PopupEditFormHorizontalAlign;
				PopupVerticalAlign = Settings.PopupEditFormVerticalAlign;
				PopupHorizontalOffset = Settings.PopupEditFormHorizontalOffset;
				PopupVerticalOffset = Settings.PopupEditFormVerticalOffset;
			}
			RenderHelper.GetPopupEditFormStyle().AssignToControl(ContentDiv);
			ContentDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
			if(MainTable != null) {
				MainTable.GridLines = GridLines.None;
				MainTable.CellSpacing = 3;
				MainTable.CellPadding = 0;
			}
		}
		protected virtual string GetPopupElementID() {
			if(RenderHelper.DataProxy.VisibleRowCountOnPage > 0) {
				return RenderHelper.GetRowId(GetShowingVisibleIndex());
			}
			return Grid.UniqueID;
		}
		protected virtual int GetShowingVisibleIndex() {
			if(VisibleIndex >= 0) return VisibleIndex;
			return RenderHelper.DataProxy.VisibleStartIndex;
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return false;
		}
	}
	public class GridViewEditFormTable : InternalTable {
		int visibleIndex;
		ASPxGridViewRenderHelper renderHelper;
		bool renderUpdateCancelButtons;
		public GridViewEditFormTable(ASPxGridViewRenderHelper renderHelper, int visibleIndex) : this(renderHelper, visibleIndex, true) { }
		public GridViewEditFormTable(ASPxGridViewRenderHelper renderHelper, int visibleIndex, bool renderUpdateCancelButtons) {
			this.renderHelper = renderHelper;
			this.visibleIndex = visibleIndex;
			this.renderUpdateCancelButtons = renderUpdateCancelButtons;
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected bool RenderUpdateCancelButtons { get { return renderUpdateCancelButtons; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected override void CreateControlHierarchy() {
			ID = ASPxGridViewRenderHelper.DXEditFormTable;
			GridViewEditFormLayout layout = new GridViewEditFormLayout(Grid);
			for(int i = 0; i < layout.RowCount; i++) {
				Rows.Add(CreateRow(layout, i));
			}
			if(RenderUpdateCancelButtons) {
				CreateUpdateCancelRow(layout.ColumnCount * 2);
			}
		}
		protected void CreateUpdateCancelRow(int columnSpan) {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			TableCell cell = new GridViewTableUpdateCancelCell(RenderHelper);
			row.Cells.Add(cell);
			cell.ColumnSpan = columnSpan;
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			GridViewEditFormTableStyle style = RenderHelper.GetEditFormStyle();
			style.AssignToControl(this, true);
			CellSpacing = (int)style.Spacing.Value;
		}
		protected virtual TableRow CreateRow(GridViewEditFormLayout layout, int rowIndex) {
			TableRow row = RenderUtils.CreateTableRow();
			int colIndex = 0;
			while(colIndex < layout.ColumnCount) {
				GridViewDataColumn editColumn = layout.GetEditColumn(colIndex, rowIndex);
				if(editColumn != null) {
					int extraCaptionSpanCount = 1;
					ASPxColumnCaptionLocation location = layout.GetCaptionLocation(editColumn);
					TableCell cell = new GridViewTableEditFormEditorCell(RenderHelper, DataProxy, editColumn, VisibleIndex, location == ASPxColumnCaptionLocation.Top);
					if(location == ASPxColumnCaptionLocation.Near) {
						extraCaptionSpanCount = 0;
						row.Cells.Add(new GridViewTableEditFormEditorCaptionCell(RenderHelper, editColumn));
					}
					row.Cells.Add(cell);
					cell.RowSpan = layout.GetRowSpan(editColumn);
					cell.Width = Unit.Percentage(100.0 * layout.GetColSpan(editColumn) / layout.ColumnCount);
					cell.ColumnSpan = layout.GetColSpan(editColumn) * 2 - 1 + extraCaptionSpanCount;
				} else {
					TableCell cell = RenderUtils.CreateTableCell();
					cell.ColumnSpan = 2;
					row.Cells.Add(cell);
				}
				colIndex += editColumn != null ? layout.GetColSpan(editColumn) : 1;
			}
			return row;
		}
	}
	public class GridViewTableEditFormEditorCaptionCell : GridViewTableCell {
		GridViewDataColumn column;
		public GridViewTableEditFormEditorCaptionCell(ASPxGridViewRenderHelper helper, GridViewDataColumn column)
			: base(helper) {
			this.column = column;
		}
		protected GridViewDataColumn Column { get { return column; } }
		protected override void CreateControlHierarchy() {
			WebControl label = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
			Controls.Add(label);
			label.Controls.Add(RenderUtils.CreateLiteralControl(Column.EditFormCaption));
		}
		protected override void PrepareControlHierarchy() {
			IAssociatedControlID assocControl = FindControl(RenderHelper.GetEditorId(Column)) as IAssociatedControlID;
			if(assocControl != null)
				(Controls[0] as WebControl).Attributes["for"] = assocControl.ClientID();
			RenderHelper.GetEditFormCaptionStyle(Column).AssignToControl(this, true);
		}
	}
	public abstract class GridViewTableEditorCellBase : GridViewTableBaseCell {
		LiteralControl caption;
		ASPxEditBase editor;
		bool hasTopCaption;
		public GridViewTableEditorCellBase(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: this(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder, false) { }
		public GridViewTableEditorCellBase(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder, bool hasTopCaption)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) {
			this.hasTopCaption = hasTopCaption;
		}
		protected abstract EditorInplaceMode GetEditMode();
		protected bool HasTopCaption { get { return hasTopCaption; } }
		protected LiteralControl Caption { get { return caption; } }
		protected override void CreateControlHierarchy() {
			if(HasTopCaption) {
				this.caption = RenderUtils.CreateLiteralControl(Column.EditFormCaption);
				Controls.Add(Caption);
				Controls.Add(RenderUtils.CreateBr());
			}
			if(CreateTemplate()) return;
			object value = DataProxy.GetEditingRowValue(VisibleIndex, Column.FieldName);
			this.editor = CreateEditor(value);
			Controls.Add(editor);
			RenderHelper.ApplyEditorSettings(editor, Column);
			if(this.editor.Width.IsEmpty)
				this.editor.Width = Unit.Percentage(100);
			if(ColumnSpan > 1 && this.editor.Height.IsEmpty)
				this.editor.Height = Unit.Percentage(100);
			RenderHelper.Grid.RaiseEditorInitialize(new ASPxGridViewEditorEventArgs(Column, VisibleIndex, this.editor, DataProxy.EditingKeyValue, value));
		}
		protected virtual bool CreateTemplate() {
			return RenderHelper.AddEditItemTemplateControl(VisibleIndex, Column, this);
		}
		protected virtual ASPxEditBase CreateEditor(object value) {
			return RenderHelper.CreateGridEditor(Column, value, GetEditMode(), false);
		}
		protected new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
	}
	public class GridViewTableEditFormEditorCell : GridViewTableEditorCellBase {
		public GridViewTableEditFormEditorCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewDataColumn column, int visibleIndex, bool hasTopCaption)
			: base(renderHelper, dataProxy, column, visibleIndex, false, false, hasTopCaption) {
		}
		protected override EditorInplaceMode GetEditMode() { return EditorInplaceMode.EditForm; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetEditFormCellStyle(Column).AssignToControl(this, true);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableInlineEditorCell : GridViewTableEditorCellBase {
		public GridViewTableInlineEditorCell(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, dataProxy, column, visibleIndex, removeLeftBorder, removeRightBorder) { }
		protected override EditorInplaceMode GetEditMode() { return EditorInplaceMode.Inplace; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetInlineEditCellStyle(Column).AssignToControl(this, true);
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableEditingErrorRow : GridViewTableRow {
		bool isStyledRow;
		public GridViewTableEditingErrorRow(ASPxGridViewRenderHelper renderHelper, bool isStyledRow, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, -1, lastRowBottomBorder) {
			this.isStyledRow = isStyledRow;
		}
		protected bool IsStyledRow { get { return isStyledRow; } }
		public override GridViewRowType RowType { get { return GridViewRowType.EditingErrorRow; } }
		protected override void CreateControlHierarchy() {
			if(!IsStyledRow && RenderHelper.HasEditingError) {
				ID = RenderHelper.GetEditingErrorRowId();
			}
			CreateIndentCells();
			Cells.Add(new GridViewTableEditingErrorCell(RenderHelper, IsStyledRow));
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetEditingErrorRowStyle().AssignToControl(this, true);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableEditingErrorCell : GridViewTableCellEx {
		bool isStyledCell;
		public GridViewTableEditingErrorCell(ASPxGridViewRenderHelper renderHelper, bool isStyledCell)
			: base(renderHelper, true) {
			this.isStyledCell = isStyledCell;
		}
		protected bool IsStyledCell { get { return isStyledCell; } }
		protected override void CreateControlHierarchy() {
			ColumnSpan = RenderHelper.VisibleColumnCount;
			if(!IsStyledCell) {
				Text = RenderHelper.EditingErrorText;
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.AppendDefaultDXClassName(this);
			base.PrepareControlHierarchy();
		}
	}
}
