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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.LookAndFeel;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridEditCellDataProvider : PivotGridCellDataProvider {
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public PivotGridEditCellDataProvider(PivotGridViewInfoData data) : base(data) { }
		public override object GetCellValue(PivotGridCellItem cellItem) {
			object value = base.GetCellValue(cellItem);
			PivotCellViewInfo cellViewInfo = cellItem as PivotCellViewInfo;
			if(cellViewInfo == null || cellViewInfo.Edit == null)
				return value;
			return Data.CustomEditValue(value, cellViewInfo);
		}
	}
	public class PivotCellViewInfo : PivotCellViewInfoBase {
		public static Point EmptyCoord = new Point(-1000, -1000);
		public PivotCellViewInfo(PivotGridCellDataProviderBase dataProvider, PivotFieldsAreaCellViewInfo columnViewInfoValue, PivotFieldsAreaCellViewInfo rowViewInfoValue, int columnIndex, int rowIndex)
			: base(dataProvider, columnViewInfoValue, rowViewInfoValue, columnIndex, rowIndex) {
		}
		protected PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
		public BaseEditViewInfo EditViewInfo { 
			get {
				RepositoryItem edit = Edit;
				return edit != null ? ViewInfo.GetEditViewInfo(edit) : null;
			} 
		}
		public DetailLevel DetailLevel {
			get { return ViewInfo.GetEditDetailLevel(this); }		
		}
		public RepositoryItem Edit { get { return Data.GetCellEdit(this); } }
		protected new PivotFieldsAreaCellViewInfo ColumnViewInfoValue { get { return (PivotFieldsAreaCellViewInfo)base.ColumnViewInfoValue; } }
		protected new PivotFieldsAreaCellViewInfo RowViewInfoValue { get { return (PivotFieldsAreaCellViewInfo)base.RowViewInfoValue; } }
		public new PivotGridField ColumnField { get { return (PivotGridField)ColumnViewInfoValue.ColumnField; } }
		public new PivotGridField RowField { get { return (PivotGridField)RowViewInfoValue.RowField; } }
		public new PivotGridField DataField { get { return (PivotGridField)base.DataField; } }
		public Rectangle PaintBounds {
			get {
				return new Rectangle(ColumnViewInfoValue.PaintBounds.X, RowViewInfoValue.PaintBounds.Y, 
					ColumnViewInfoValue.PaintBounds.Width, RowViewInfoValue.PaintBounds.Height);
			}
		}		
		public void Draw(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance, bool drawFocusedCellRect) {
			if(Edit != null)
				DrawEditViewInfo(e, cellBounds, cellAppearance);
			else {				
				if(ShowKPIGraphic) {
					cellBounds = DrawKPIGraphic(e, cellBounds, cellAppearance);
				} else
					DrawText(e, cellBounds, cellAppearance);
			}
			if(Focused && drawFocusedCellRect) {
				e.GraphicsCache.Paint.DrawFocusRectangle(e.Graphics, cellBounds, cellAppearance.GetForeColor(), cellAppearance.GetBackColor());
			}
		}
		protected Rectangle DrawKPIGraphic(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			Bitmap bitmap = Data.GetKPIBitmap(KPIGraphic, KPIValue);
			e.Graphics.DrawImage(bitmap,
					cellBounds.Left + (cellBounds.Width - bitmap.Width) / 2,
					cellBounds.Top + (cellBounds.Height - bitmap.Height) / 2);
			return cellBounds;
		}
		protected void DrawText(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			Padding paddings = GetPaddings();
			TextBounds = new Rectangle(cellBounds.X + paddings.Left, cellBounds.Y, 
				cellBounds.Width - paddings.Left - paddings.Right, cellBounds.Height);
			cellAppearance.DrawString(e.GraphicsCache, Text, TextBounds);
		}
		protected void DrawEditViewInfo(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			BaseEditViewInfo viewInfo = EditViewInfo;
			viewInfo.Item.BeginUpdate();
			viewInfo.PaintAppearance.Assign(cellAppearance);
			viewInfo.Item.CancelUpdate();
			Rectangle bounds = new Rectangle(cellBounds.X + 1, cellBounds.Y + 1, cellBounds.Width - 2, cellBounds.Height - 2);
			if(!viewInfo.IsReady)
				viewInfo.CalcViewInfo(e.Graphics);
			UpdateEditViewInfoCellFormat(viewInfo);
			viewInfo.DetailLevel = DetailLevel;
			viewInfo.EditValue = Value;
			viewInfo.SetDisplayText(Text);
			viewInfo.ReCalcViewInfo(e.Graphics, MouseButtons.None, Point.Empty, bounds);
			viewInfo.FillBackground = false;
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			viewInfo.Bounds = bounds;
			BaseControlPainter painter = viewInfo.Painter;
			painter.Draw(new ControlGraphicsInfoArgs(viewInfo, e.GraphicsCache, cellBounds));
		}
		void UpdateEditViewInfoCellFormat(BaseEditViewInfo editViewInfo) {
			FormatInfo cellFormat = GetCellFormatInfo();
			if(cellFormat != null) {
				editViewInfo.Item.BeginUpdate();
				editViewInfo.Format.Assign(cellFormat);
				editViewInfo.Item.CancelUpdate();
			}
		}
		protected override string GetDisplayText() {
			string text = base.GetDisplayText();
			if(EditViewInfo == null)
				return text;
			UpdateEditViewInfoCellFormat(EditViewInfo);
			EditViewInfo.EditValue = Value;
			return EditViewInfo.DisplayText;
		}
	}
	public class PivotCellViewInfoBase : PivotGridCellItem {
		const int TextOffset = 2;
		Rectangle bounds;
		Rectangle textBounds;
		AppearanceObject appearance;
		bool focused;
		bool selected;
		FormatType formatType;
		PivotFieldsAreaCellViewInfoBase columnViewInfoValue;
		PivotFieldsAreaCellViewInfoBase rowViewInfoValue;
		int isTextFit;
		public PivotCellViewInfoBase(PivotGridCellDataProviderBase dataProvider, PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int columnIndex, int rowIndex) 
			: base(dataProvider, columnViewInfoValue.Item, rowViewInfoValue.Item, columnIndex, rowIndex) {
			this.columnViewInfoValue = columnViewInfoValue;
			this.rowViewInfoValue = rowViewInfoValue;
			this.appearance = new AppearanceObject();
			this.bounds = Rectangle.Empty;
			this.textBounds = Rectangle.Empty;
			this.focused = false;
			this.selected = false;
			this.formatType = GetFormatType();
			this.isTextFit = -1;
		}
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				this.appearance = value;
			}
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle TextBounds {
			get { return textBounds.IsEmpty ? Bounds : textBounds; }	
			set { textBounds = value; }
		}
		public bool Focused { get { return focused; } set { focused = value; } }
		public bool Selected { get { return selected; } set { selected = value; } }		
		public FormatType FormatType { get { return formatType; } set { formatType = value; } }
		public bool ShowTooltip {
			get { return !IsTextFit || ShowKPIGraphic; }
		}
		public string TooltipText { 
			get { 
				if(!ShowKPIGraphic) return Text;
				return Data.GetKPITooltipText(DataField.KPIType, KPIValue);
			} 
		}
		public bool IsTextFit {
			get {
				if(isTextFit < 0) {
					GraphicsInfo ginfo = new GraphicsInfo();
					ginfo.AddGraphics(null);
					Size size = Appearance.CalcTextSize(ginfo.Graphics, Text, 0).ToSize();
					ginfo.ReleaseGraphics();
					isTextFit = TextBounds.Contains(new Rectangle(TextBounds.Location, size)) ? 1 : 0;
				}
				return isTextFit > 0 ? true : false;
			}
		}
		protected PivotFieldsAreaCellViewInfoBase ColumnViewInfoValue { get { return columnViewInfoValue; } }
		protected PivotFieldsAreaCellViewInfoBase RowViewInfoValue { get { return rowViewInfoValue; } }
		public static Padding GetPaddings() {
			return new Padding(TextOffset, 0, TextOffset, 0);
		}
	}
	public class PivotCellsViewInfo : PivotCellsViewInfoBase {
		const int MinScrollInterval = 50;
		const int MaxScrollInterval = 200;
		const int StartScrollInterval = 50;
		const int ScrollIntervalChangeStep = 20;
		Timer scrollTimer;
		Point scrollPoint;
		bool firstTick;
		int tickNum;
		bool isMouseDown;
		Point leftTopCoordOffset;		
		public PivotCellsViewInfo(PivotGridViewInfo viewInfo)
			:base(viewInfo)	{
			this.scrollTimer = null;
			this.scrollPoint = Point.Empty;
			this.leftTopCoordOffset = Point.Empty;
		}
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public bool IsMultiSelect { get { return Data.OptionsSelection.MultiSelect; } }
		public bool IsControlDown { get { return KeysState == Keys.Control; } }
		public bool IsShiftDown { get { return KeysState == Keys.Shift; } }
		protected virtual Keys KeysState { get { return Control.ModifierKeys; } }
		public override void InvalidatedCell(Point cell) {
			if(!IsCellValid(cell)) return;
			Rectangle bounds = GetCellBounds(cell);
			Data.Invalidate(bounds);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(!Data.OptionsHint.ShowCellHints) return null;
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(pt);
			if(cellViewInfo != null && cellViewInfo.ShowTooltip) {
				return new ToolTipControlInfo(cellViewInfo, cellViewInfo.TooltipText);
			}
			return null;
		}
		public void DoScrollAndSelection(Point pt, Point offset) {
			if(offset.IsEmpty) return;
			tickNum++;
			Point newLeftTopCoord = LeftTopCoord;
			if(tickNum % 3 == 0)
				newLeftTopCoord.X += offset.X;
			newLeftTopCoord.Y += offset.Y;
			LeftTopCoord = newLeftTopCoord;
			DoCellsSelection(pt);
		}
		Point fPreviousCell = PivotCellViewInfo.EmptyCoord;
		protected virtual void DoCellsSelection(Point pt) {
			int x = GetColumnIndexAt(pt.X);
			int y = GetRowIndexAt(pt.Y);
			if(x < 0)
				x = pt.X < Bounds.X ? Math.Max(LeftTopCoord.X - 1, 0) : LeftTopCoord.X + this.GetVisibleColumnCount(ViewInfo.ScrollableSize.Width);
			if(y < 0)
				y = pt.Y < Bounds.Y ? Math.Max(LeftTopCoord.Y - 1, 0) : LeftTopCoord.Y + this.GetVisibleRowCount(ViewInfo.ScrollableSize.Height);
			Point currentCell = new Point(x, y);
			if(currentCell != fPreviousCell || (fPreviousCell == PivotCellViewInfo.EmptyCoord && currentCell != ViewInfo.FocusedCell))
				ViewInfo.MoveSelectionTo(currentCell);
			fPreviousCell = currentCell;
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return null;
			Point cell = Point.Empty;
			cell.X = GetColumnIndexAt(e.X);
			cell.Y = GetRowIndexAt(e.Y);
			if(!(ViewInfo.IsLastSelectionIsShiftDown && IsShiftDown)) ViewInfo.StartSelection(IsMultiSelect);
			Point oldFocusedCell = ViewInfo.FocusedCell;
			if(!IsShiftDown) ViewInfo.FocusedCell = cell;
			if(IsShiftDown && oldFocusedCell != cell) {
				int x1 = Math.Min(oldFocusedCell.X, cell.X);
				int y1 = Math.Min(oldFocusedCell.Y, cell.Y);
				int x2 = Math.Max(oldFocusedCell.X, cell.X);
				int y2 = Math.Max(oldFocusedCell.Y, cell.Y);
				ViewInfo.AddSelection(new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1));
			}
			if(IsMultiSelect && IsControlDown && oldFocusedCell != cell && !ViewInfo.SelectedCells.Contains(oldFocusedCell)) {
				ViewInfo.AddSelection(new Rectangle(oldFocusedCell.X, oldFocusedCell.Y, 1, 1));
				ViewInfo.StartSelection(IsMultiSelect);
			}
			if(!IsMultiSelect && !IsShiftDown) ViewInfo.Selection = Rectangle.Empty;
			isMouseDown = true;
			return this;
		}
		protected override void MouseUpCore(MouseEventArgs e) {
			isMouseDown = false;
			StopScrollTimer();
			if(ViewInfo.Selection == Rectangle.Empty) {
				PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(e.Location);
				if(cellViewInfo != null)
					Data.CellClick(cellViewInfo);
			}
			base.MouseUpCore(e);
		}
		protected override void MouseMoveCore(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left || !isMouseDown) return;
			StartScrollTimer(new Point(e.X, e.Y));
			DoCellsSelection(new Point(e.X, e.Y));
		}
		public override void DoubleClick() {
			if(Data.ControlOwner == null) return;
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(Data.ControlOwner.PointToClient(Cursor.Position));
			if(cellViewInfo != null)
				Data.CellDoubleClick(cellViewInfo);
		}
		void StartScrollTimer(Point pt) {
			this.scrollPoint = pt;
			this.leftTopCoordOffset = GetLeftTopCoordOffset(pt);
			if(!this.leftTopCoordOffset.IsEmpty) {
				if(scrollTimer != null) return;
				this.firstTick = true;
				this.tickNum = 0;
				this.scrollTimer = new Timer();
				this.scrollTimer.Interval = StartScrollInterval;
				this.scrollTimer.Tick += new EventHandler(OnScrollTimerElapsed);
				this.scrollTimer.Enabled = true;
			} else
				StopScrollTimer();
		}
		void StopScrollTimer() {
			if(this.scrollTimer != null) {
				this.scrollTimer.Enabled = false;
				this.scrollTimer.Tick -= new EventHandler(OnScrollTimerElapsed);
				this.scrollTimer.Dispose();
				this.scrollTimer = null;
			}
		}
		void OnScrollTimerElapsed(object sender, EventArgs e) {
			if(this.firstTick) {
				firstTick = false;
				this.scrollTimer.Interval = MaxScrollInterval;
			}
			if(this.scrollTimer != null && this.scrollTimer.Interval > MinScrollInterval) {
				this.scrollTimer.Interval -= ScrollIntervalChangeStep;
			}
			DoScrollAndSelection(this.scrollPoint, this.leftTopCoordOffset);
		}
		protected override string GetPivotCellText(PivotCellViewInfoBase cell) {
			return Data.GetPivotCellText((PivotCellViewInfo)cell);
		}
		protected override void SetFocusAndSelectionCell(List<PivotCellViewInfoBase> cells) {
			for(int i = 0; i < cells.Count; i++) {
				PivotCellViewInfo cell = (PivotCellViewInfo)cells[i];
				cell.Focused = cell.ColumnIndex == ViewInfo.FocusedCell.X && cell.RowIndex == ViewInfo.FocusedCell.Y;
				cell.Selected = ViewInfo.SelectedCells.Contains(new Point(cell.ColumnIndex, cell.RowIndex));
			}
		}
		internal protected override void CombinWithSelectAppearance(PivotCellViewInfo cellViewInfo, AppearanceObject cellAppearance) {
			if(cellViewInfo.Focused && Data.OptionsSelection.EnableAppearanceFocusedCell) {
				AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] { Data.PaintAppearance.FocusedCell, cellViewInfo.Appearance });
			} else {
				if(cellViewInfo.Selected) {
					bool blendBackColor = cellViewInfo.Appearance.Options.UseBackColor && Data.PaintAppearance.SelectedCell.Options.UseBackColor;
					bool blendForeColor = cellViewInfo.Appearance.Options.UseForeColor && Data.PaintAppearance.SelectedCell.Options.UseForeColor;
					Color backColor = blendColor(cellViewInfo.Appearance.BackColor, Data.PaintAppearance.SelectedCell.BackColor);
					Color foreColor = blendColor(cellViewInfo.Appearance.ForeColor, Data.PaintAppearance.SelectedCell.ForeColor);
					AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] { Data.PaintAppearance.SelectedCell, cellViewInfo.Appearance });
					if(blendForeColor)
						cellAppearance.ForeColor = foreColor;
					if(blendBackColor)
						cellAppearance.BackColor = backColor;
				}
			}
		}
		Color blendColor(Color c0, Color c1) {
			if(c1.A == 255) return c1;
			float alpha = (float)c1.A / byte.MaxValue;
			return Color.FromArgb(blendComponent(c0.R, c1.R, alpha),
				blendComponent(c0.G, c1.G, alpha), blendComponent(c0.B, c1.B, alpha));
		}
		int blendComponent(int c0, int c1, float alpha) {
			return (int)(c0 + (-c0 + c1) * alpha);
		}
		internal protected override bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo) {
			return Data.CustomDrawCell(paintArgs, ref cellAppearance, cellViewInfo);
		}
		internal protected override void CustomAppearance(ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo) {
			Data.CustomAppearance(ref cellAppearance, cellViewInfo);
		}
		protected override AppearanceObject GetCellAppearance(PivotCellViewInfoBase cellViewInfo) {
			AppearanceObject appearance = base.GetCellAppearance(cellViewInfo);
			PivotGridStyleFormatCondition formatAppearance = Data.FormatConditions.GetStyleFormatByValue(cellViewInfo.DataField, cellViewInfo.Value, GetCellType(cellViewInfo));
			if(formatAppearance != null) {
				AppearanceObject oldAppearance = appearance;
				appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { formatAppearance.Appearance, oldAppearance });
			}
			return appearance;
		}
		protected override PivotCellViewInfoBase CreateCellViewInfoCore(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row) {
			return new PivotCellViewInfo(VisualItems.CellDataProvider, (PivotFieldsAreaCellViewInfo)columnViewInfoValue, (PivotFieldsAreaCellViewInfo)rowViewInfoValue, col, row);
		}
	}
	public class PivotCellsViewInfoBase : PivotViewInfo {		
		AppearanceObject cellAppearance, totalAppearance, grandTotalAppearance;						
		ArrayList rowLines;
		ArrayList columnLines;
		List<PivotCellViewInfoBase> cells;
		int cellsBottom;
		Rectangle cellBounds;
		public PivotCellsViewInfoBase(PivotGridViewInfoBase viewInfo)
			: base(viewInfo) {
			this.cellAppearance = ViewInfo.PrintAndPaintAppearance.Cell;
			this.totalAppearance = new AppearanceObject();
			this.grandTotalAppearance = new AppearanceObject();
			AppearanceHelper.Combine(this.totalAppearance, new AppearanceObject[] {ViewInfo.PrintAndPaintAppearance.TotalCell, CellAppearance});
			AppearanceHelper.Combine(this.grandTotalAppearance, new AppearanceObject[] {ViewInfo.PrintAndPaintAppearance.GrandTotalCell, ViewInfo.PrintAndPaintAppearance.TotalCell, CellAppearance});			
			this.rowLines = new ArrayList();
			this.columnLines = new ArrayList();
			this.cells = new List<PivotCellViewInfoBase>();
			this.cellsBottom = 0;
			this.cellBounds = Rectangle.Empty;
		}
		protected PivotVisualItemsBase VisualItems { get { return Data.VisualItems; } }
		public Point GetMaximumLeftTopCoord(Size size) {
			return new Point(GetMaximumLeftTopCoord(true, size), GetMaximumLeftTopCoord(false, size));
		}
		public int ColumnCount { get { return GetFieldsArea(true).LastLevelItemCount; } }
		public int RowCount { get { return GetFieldsArea(false).LastLevelItemCount; } }
		public int TotalHeight { get { return GetTotalSize(false, int.MaxValue); }	}
		public int TotalWidth { get { return GetTotalSize(true, int.MaxValue); }	}
		public Point LeftTopCoord {
			get { return new Point(ViewInfo.CellsLeftCoord, ViewInfo.LeftTopCoord.Y); }
			set {
				if(ViewInfo.IsHorzScrollControl) 
					value.X += ViewInfo.RowAreaFields.ScrollColumnCount;
				ViewInfo.LeftTopCoord = value;
			}
		}
		public int OffsetByLeftTopCoord(bool isColumn) {
			return GetTotalSize(isColumn, isColumn ? LeftTopCoord.X : LeftTopCoord.Y);
		}
		public bool IsCellValid(Point cell) {
			return cell.X >= 0 && cell.X < ColumnCount && cell.Y >= 0 && cell.Y < RowCount;
		}
		public int GetVisibleRowCount(int height) {
			int count = 0;
			int topCoord = LeftTopCoord.Y > RowCount ? RowCount : LeftTopCoord.Y;
			for(int row = topCoord; row < RowCount; row ++) {
				height -= GetCellHeight(GetRowValue(row));
				if(height <= 0) break;
				count ++;
			}			
			for(int row = topCoord - 1; row >= 0; row --) {
				height -= GetCellHeight(GetRowValue(row));
				if(height <= 0) break;
				count ++;
			}			
			return count;
		}
		public int GetVisibleColumnCount(int width) {
			int count = 0;
			int leftCoord = ViewInfo.CellsLeftCoord > ColumnCount ? ColumnCount : ViewInfo.CellsLeftCoord;
			for(int col = leftCoord; col < ColumnCount; col ++) {
				width -= GetCellWidth(GetColumnValue(col));
				if(width <= 0) break;
				count ++;
			}
			for(int col = leftCoord - 1; col >= 0; col --) {
				width -= GetCellWidth(GetColumnValue(col));
				if(width <= 0) break;
				count ++;
			}
			return count;
		}
		public virtual void InvalidatedCell(Point cell) {  }
		public override bool AcceptDragDrop { get { return true; } }
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(hitPoint);
			return cellViewInfo != null ?  new PivotGridHitInfo(cellViewInfo, hitPoint) : new PivotGridHitInfo(hitPoint);
		}
		public override Rectangle GetDragDrawRectangle(PivotGridFieldBase field, Point pt) { 
			return ViewInfo.ScrollableBounds; 
		}
		public override int GetNewFieldPosition(PivotGridFieldBase field, Point pt, out PivotArea area) { 
			area = PivotArea.DataArea;
			return Data.DataFieldCount; 
		}
		public PivotCellViewInfo GetCellViewInfoAt(Point pt) {
			if(this.cells.Count == 0)
				CalculateCellsViewInfo();
			return (PivotCellViewInfo)this.cells.Find(new Predicate<PivotCellViewInfoBase>(
				delegate(PivotCellViewInfoBase cell) {
					return cell.Bounds.Contains(pt);
				}
			));
		}
		public Point GetCellCoordAt(Point pt) {
			PivotCellViewInfo viewInfo = GetCellViewInfoAt(pt);
			return viewInfo == null ? PivotCellViewInfo.EmptyCoord : new Point(viewInfo.ColumnIndex, viewInfo.RowIndex);
		}
		public int GetBestWidth(PivotGridFieldBase field) {
			if(field.Area != PivotArea.DataArea) return 0;
			int width = 0;
			string maxText = GetBiggestText(field);
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			try {
				width = GetStringWidth(graphics, maxText, CellAppearance.Font, CellAppearance.GetStringFormat());
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
			Padding paddings = PivotCellViewInfoBase.GetPaddings();
			return width + paddings.Left + paddings.Right + (ShowVertLines ? LineWidth : 0) + 1;
		}
		public string GetBiggestText(PivotGridFieldBase field) {
			string text = string.Empty;
			for(int i = 0; i < RowCount; i ++) {
				for(int j = 0; j < ColumnCount; j ++) {
					PivotCellViewInfoBase cellViewInfo = CreateCellViewInfo(GetColumnValue(j), GetRowValue(i), j, i);
					if(cellViewInfo.DataField != field) continue;
					if(!string.IsNullOrEmpty(cellViewInfo.Text) && cellViewInfo.Text.Length > text.Length)
						text = cellViewInfo.Text;
				}
			}
			return text;
		}
		int GetStringWidth(Graphics graphics, string drawText, Font font, StringFormat format) {
			if(drawText.Length == 0) return 0;
			return XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, drawText, font, format, 0).Width);
		}
		public Rectangle GetCellBounds(Point cell) {
			return new Rectangle(FirstCellOffSet.X + GetTotalSize(true, cell.X) - GetTotalSize(true, ViewInfo.CellsLeftCoord), 
				Bounds.Y + GetTotalSize(false, cell.Y)  - GetTotalSize(false, LeftTopCoord.Y),
				GetCellWidth(GetColumnValue(cell.X)), GetCellHeight(GetRowValue(cell.Y)));
		}
		protected int GetCellWidth(PivotFieldsAreaCellViewInfoBase columnViewInfoValue) {
			return columnViewInfoValue.Bounds.Width;
		}
		protected int GetCellHeight(PivotFieldsAreaCellViewInfoBase rowViewInfoValue) {
			return rowViewInfoValue.Bounds.Height;
		}
		protected internal PivotFieldsAreaCellViewInfoBase GetRowValue(int index) {
			return GetValue(false, index);
		}
		protected internal PivotFieldsAreaCellViewInfoBase GetColumnValue(int index) {
			return GetValue(true, index);
		}
		protected PivotFieldsAreaCellViewInfoBase GetValue(bool isColumn, int index) {
			return GetFieldsArea(isColumn).GetLastLevelViewInfo(index);
		}
		protected PivotFieldsAreaViewInfoBase GetFieldsArea(bool isColumn) {
			return ViewInfo.GetFieldsArea(isColumn);
		}
		protected int LineWidth { get { return 1; } }
		public int GetTotalSize(bool isColumn, int maxCount) {
			int count = GetFieldsArea(isColumn).LastLevelItemCount;
			int size = 0;
			for(int i = 0; i < count; i ++) {
				if(i == maxCount) break;
				size += isColumn ? GetCellWidth(GetColumnValue(i)) : GetCellHeight(GetRowValue(i));
			}
			return size;
		}
		protected int GetMaximumLeftTopCoord(bool isColumn, Size clientSize) {
			int totalSize = GetTotalSize(isColumn, int.MaxValue);
			int size = isColumn ? clientSize.Width : clientSize.Height;
			if(size >= totalSize) return 0;
			return isColumn ? 
				ColumnCount - GetVisibleColumnCount(clientSize.Width) : 
				RowCount - GetVisibleRowCount(clientSize.Height);
		}
		protected AppearanceObject CellAppearance { get { return cellAppearance; } }
		protected AppearanceObject TotalAppearance { get { return totalAppearance; } }
		protected AppearanceObject GrandTotalAppearance { get { return grandTotalAppearance; } }
		protected PivotGridCellType GetCellType(PivotCellViewInfoBase cellViewInfo) {
			PivotGridCellType cellType = PivotGridCellType.Cell;
			if(cellViewInfo.IsCustomTotalAppearance)
				cellType = PivotGridCellType.CustomTotal;
			else {
				if(cellViewInfo.IsGrandTotalAppearance) {
					cellType = PivotGridCellType.GrandTotal;
				}
				if(cellViewInfo.IsTotalAppearance) {
					cellType = PivotGridCellType.Total;
				}
			}
			return cellType;
		}
		protected virtual AppearanceObject GetCellAppearance(PivotCellViewInfoBase cellViewInfo) {
			AppearanceObject defaultAppearance = CellAppearance;
			AppearanceObject appearance = null;
			if(cellViewInfo.IsCustomTotalAppearance) {
				defaultAppearance = ViewInfo.PrintAndPaintAppearance.CustomTotalCell;
				appearance = new AppearanceObject();
				PivotGridCustomTotal customTotal = (PivotGridCustomTotal)cellViewInfo.CustomTotal;
				AppearanceHelper.Combine(appearance, new AppearanceObject[] {customTotal.Appearance, defaultAppearance});
			}
			else {
				if(cellViewInfo.IsGrandTotalAppearance) 
					defaultAppearance = GrandTotalAppearance;
				if(cellViewInfo.IsTotalAppearance) 
					defaultAppearance = TotalAppearance;
			}
			if(appearance == null)
				appearance = defaultAppearance;			
			return appearance;
		}
		protected int GetColumnIndexAt(int x) {
			int width = FirstCellOffSet.X;
			if(x < width) return -1;
			for(int i = ViewInfo.CellsLeftCoord; i < ColumnCount; i ++) {
				width += GetCellWidth(GetColumnValue(i));
				if(x < width)
					return i;
			}
			return -1;
		}
		protected int GetRowIndexAt(int y) {
			y -= Bounds.Y;
			if(y < 0) return -1;
			int height = 0;
			for(int i = LeftTopCoord.Y; i < RowCount; i ++) {
				height +=  Data.DefaultFieldHeight;
				if(y < height)
					return i;
			}
			return -1;
		}		
		public Point GetLeftTopCoordOffset(Point pt) {
			Point offset = Point.Empty;
			if(ViewInfo.IsHScrollBarVisible) {
				if(pt.X < Bounds.X) 
					offset.X = -1;
				if(pt.X > ViewInfo.Bounds.Right)
					offset.X = 1;
			}
			if(ViewInfo.IsVScrollBarVisible) {
				if(pt.Y < Bounds.Y) 
					offset.Y = -1;
				if(pt.Y > ViewInfo.Bounds.Bottom)
					offset.Y = 1;
			}
			return offset;
		}		
		public virtual bool ShowVertLines { get { return Data.OptionsView.ShowVertLines; } }
		public virtual bool ShowHorzLines { get { return Data.OptionsView.ShowHorzLines; } }
		protected virtual bool DrawFocusedCellRect { get { return Data.OptionsView.DrawFocusedCellRect; } }
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			if(Data.DataFieldCount == 0)
				DrawEmptyCells(e);
			else DrawDataCells(e);
		}
		public void CalculateCellsViewInfo() {
			if(cellBounds.Location != LeftTopCoord || cellBounds.Size != ViewInfo.ControlBounds.Size) {
				cellBounds.Location = LeftTopCoord;
				cellBounds.Size = ViewInfo.ControlBounds.Size;
				this.cells.Clear();
				this.rowLines.Clear();
				this.columnLines.Clear();
				this.cellsBottom = 0;
			}
			if(this.cells.Count == 0) {
				CreateCellsViewInfo(LeftTopCoord, FirstCellOffSet, new Point(ViewInfo.Bounds.Right, ViewInfo.Bounds.Bottom), this.cells, this.columnLines, this.rowLines, out cellsBottom);
			}
		}
		public Point FirstCellOffSet {
			get {
				Point pt = Bounds.Location;
				if(ViewInfo.IsHorzScrollControl)
					pt.X = Bounds.X + ViewInfo.BoundsOffset.Width + OffsetByLeftTopCoord(true);
				return pt;
			}
		}
		protected virtual void DrawDataCells(ViewInfoPaintArgs e) {
			CalculateCellsViewInfo();
			SetFocusAndSelectionCell(this.cells);
			DrawCells(e, this.cells);
			DrawLines(e, this.rowLines, -1);
			DrawLines(e, this.columnLines, this.cellsBottom - Bounds.Y);
		}
		protected virtual void DrawEmptyCells(ViewInfoPaintArgs e) {
			Rectangle dataBounds = new Rectangle(FirstCellOffSet.X, FirstCellOffSet.Y, ViewInfo.Bounds.Right - FirstCellOffSet.X, ViewInfo.Bounds.Bottom - FirstCellOffSet.Y);
			if(dataBounds.Right > ViewInfo.ColumnAreaFields.Bounds.Right)
				dataBounds.Width = ViewInfo.ColumnAreaFields.Bounds.Right - dataBounds.X;
			if(dataBounds.Bottom > ViewInfo.RowAreaFields.Bounds.Bottom)
				dataBounds.Height = ViewInfo.RowAreaFields.Bounds.Bottom - dataBounds.Y;
			CellAppearance.FillRectangle(e.GraphicsCache, dataBounds);
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			string text = PivotGridLocalizer.GetString(PivotGridStringId.DataHeadersCustomization);
			Font font = new Font(CellAppearance.Font.FontFamily, GetFontHeight(e.Graphics, CellAppearance.Font, text, format, dataBounds.Size), CellAppearance.Font.Style);
			CellAppearance.DrawString(e.GraphicsCache, text, dataBounds,  font, format);
			font.Dispose();
			DrawEmptyLines(e);
		}
		protected virtual void DrawEmptyLines(ViewInfoPaintArgs e) {
			int right = Math.Min(ViewInfo.ColumnAreaFields.ControlBounds.Right, ViewInfo.Bounds.Right);
			int bottom = Math.Min(ViewInfo.RowAreaFields.ControlBounds.Bottom, ViewInfo.Bounds.Bottom);
			if(ViewInfo.RowAreaFields.ControlBounds.Bottom < ViewInfo.Bounds.Bottom) 
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, new Rectangle(Bounds.X, bottom - LineWidth, right - Bounds.X, LineWidth));
			if(ViewInfo.RowAreaFields.ControlBounds.Right < ViewInfo.Bounds.Right) 
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, new Rectangle(right - LineWidth, Bounds.Y, LineWidth, bottom - Bounds.Y));
		}
		static float GetFontHeight(Graphics graphics, Font origionalFont, string text, StringFormat format, Size size) {
			const float MaxFontSize = 16, MinFontSize = 5;
			Padding padding = PivotCellViewInfoBase.GetPaddings();
			float fontSize = origionalFont.Size;
			size.Width -= padding.Left + padding.Right;
			size.Height -= padding.Top + padding.Bottom;
			Size testSize = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, text, origionalFont, format, 0));
			float dx = testSize.Width > size.Width && testSize.Height > size.Height ? -1 : 1;
			while(dx > 0 ? fontSize < MaxFontSize : fontSize > MinFontSize) {
				Font testFont = new Font(origionalFont.FontFamily, fontSize + dx, origionalFont.Style);
				testSize = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, text, testFont, format, 0));
				if(dx > 0) {
					if(testSize.Width > size.Width  || testSize.Height > size.Height)
						break;
				} else {
					if(testSize.Width <= size.Width && testSize.Height <= size.Height)
						break;
				}
				fontSize += dx;
			}
			return fontSize;
		}
		public PivotCellViewInfoBase CreateCellViewInfo(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= ColumnCount) return null;
			if(rowIndex < 0 || rowIndex >= RowCount) return null;
			PivotFieldsAreaCellViewInfoBase columnViewInfoValue = GetColumnValue(columnIndex);
			PivotFieldsAreaCellViewInfoBase rowViewInfoValue = GetRowValue(rowIndex);
			return CreateCellViewInfo(columnViewInfoValue, rowViewInfoValue, columnIndex, rowIndex);
		}
		public void CreateCellsViewInfo(Point leftTopCoord, Point leftTop, Point rightBottom, List<PivotCellViewInfoBase> cells, ArrayList colLines, ArrayList rowLines, out int cellsBottom) {
			CreateCellsViewInfo(leftTopCoord, leftTop, rightBottom, cells, colLines, rowLines, out cellsBottom, null);
		}
		public void CreateCellsViewInfo(Point leftTopCoord, Point leftTop, Point rightBottom, List<PivotCellViewInfoBase> cells, ArrayList colLines, ArrayList rowLines, out int cellsBottom, EventHandler onCell) {
			int y = leftTop.Y;
			int right = rightBottom.X;
			int bottom = rightBottom.Y;			
			for(int row = leftTopCoord.Y; row < RowCount; row ++) {
				PivotFieldsAreaCellViewInfoBase rowViewInfoValue = GetRowValue(row);
				y += rowViewInfoValue.Separator;
				if(y >= bottom) break;
				int x = leftTop.X;
				int maxX = x;
				int cellHeight = GetCellHeight(rowViewInfoValue);
				for(int col = leftTopCoord.X; col < ColumnCount; col ++) {
					PivotFieldsAreaCellViewInfoBase columnViewInfoValue = GetColumnValue(col);
					x += columnViewInfoValue.Separator;
					if(x >= right) break;
					PivotCellViewInfoBase cell = CreateCellViewInfo(columnViewInfoValue, rowViewInfoValue, col, row);
					int cellWidth = GetCellWidth(columnViewInfoValue);
					cell.Bounds = new Rectangle(x, y, cellWidth, cellHeight);
					if(cells != null)
						cells.Add(cell);
					if(onCell != null)
						onCell(cell, null);
					x += cellWidth;
					if(colLines != null && ShowVertLines && row == ViewInfo.LeftTopCoord.Y)
						colLines.Add(new Rectangle(x - LineWidth, ControlBounds.Y, LineWidth, 0));
				}
				y += cellHeight;
				if(rowLines != null && ShowHorzLines)
					rowLines.Add(new Rectangle(ControlBounds.X, y - LineWidth, x - ControlBounds.X, LineWidth));
			}
			cellsBottom = y;
		}
		protected virtual PivotCellViewInfoBase CreateCellViewInfoCore(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row) {
			return new PivotCellViewInfoBase(VisualItems.CellDataProvider, columnViewInfoValue, rowViewInfoValue, col, row);
		}
		protected PivotCellViewInfoBase CreateCellViewInfo(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row) {
			PivotCellViewInfoBase cell = CreateCellViewInfoCore(columnViewInfoValue, rowViewInfoValue, col, row);
			cell.Text = GetPivotCellText(cell);
			AppearanceHelper.Combine(cell.Appearance, new AppearanceObject[] {GetCellAppearance(cell)});
			return cell;
		}
		protected virtual string GetPivotCellText(PivotCellViewInfoBase cell) {
			return cell.Text;
		}
		protected virtual void SetFocusAndSelectionCell(List<PivotCellViewInfoBase> cells) {
			for(int i = 0; i < cells.Count; i ++) {
				PivotCellViewInfo cell = (PivotCellViewInfo)cells[i];
				cell.Focused = false;
				cell.Selected = false;
			}
		}
		public void UpdateCellAppearanceHAlignment(AppearanceObject cellAppearance) {
			if(cellAppearance.HAlignment == HorzAlignment.Default)
				cellAppearance.TextOptions.HAlignment = HorzAlignment.Far;
		}
		void DrawCells(ViewInfoPaintArgs e, List<PivotCellViewInfoBase> cells) {
			for(int i = 0; i < cells.Count; i ++)
				DrawCell(e, (PivotCellViewInfo)cells[i]);
		}
		void DrawCell(ViewInfoPaintArgs e, PivotCellViewInfo cellViewInfo) {
			Rectangle cellBounds = cellViewInfo.Bounds;
			if(!cellBounds.IntersectsWith(e.ClipRectangle)) return;
			if(ShowVertLines)
				cellBounds.Width -= LineWidth;
			if(ShowHorzLines)
				cellBounds.Height -= LineWidth;
			AppearanceObject cellAppearance = new AppearanceObject();
			AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] {cellViewInfo.Appearance});
			CombinWithSelectAppearance(cellViewInfo, cellAppearance);
			UpdateCellAppearanceHAlignment(cellAppearance);
			CustomAppearance(ref cellAppearance, cellViewInfo);
			if(!CustomDrawCell(e, ref cellAppearance, cellViewInfo)) {
				cellViewInfo.Draw(e, cellBounds, cellAppearance, DrawFocusedCellRect);
			}
		}
		internal protected virtual void CombinWithSelectAppearance(PivotCellViewInfo cellViewInfo, AppearanceObject cellAppearance) {
		}
		internal protected virtual bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo) {
			return false;
		}
		internal protected virtual void CustomAppearance(ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo) {
		}
		void DrawLines(ViewInfoPaintArgs e, ArrayList lines, int height) {
			for(int i = 0; i < lines.Count; i ++) {
				Rectangle bounds = (Rectangle)lines[i];
				if(height > 0)
					bounds.Height = height;
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, bounds);
			}
		}
	}
}
