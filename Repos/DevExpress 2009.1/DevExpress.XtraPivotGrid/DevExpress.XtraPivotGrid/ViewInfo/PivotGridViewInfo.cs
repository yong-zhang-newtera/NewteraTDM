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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	internal class PivotCustomDrawAppearanceOwner : IPivotCustomDrawAppearanceOwner {
		AppearanceObject appearance;
		public PivotCustomDrawAppearanceOwner(AppearanceObject originalAppearance) {
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] { originalAppearance });
		}
		public AppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
	}
	public class PivotGridViewInfo : PivotGridViewInfoBase, IMultipleSelection {
		IViewInfoControl control;
		PivotGridFieldBase resizingField;
		int initResizingX;
		int drawResizingLineX;
		PivotGridViewInfoState state;
		PivotGridViewScroller scroller;
		PivotGridDragManager dragManager;
		readonly PivotGridSelection fSelection;
		Point focusedCell;
		Dictionary<RepositoryItem, BaseEditViewInfo> editViewInfoCache;
		public PivotGridViewInfo(PivotGridViewInfoData data)
			: base(data) {
			this.control = data;
			this.resizingField = null;
			this.initResizingX = 0;
			this.drawResizingLineX = -1;
			this.scroller = new PivotGridViewScroller(this);
			this.dragManager = null;
			this.fSelection = new PivotGridSelection();
			this.focusedCell = Point.Empty;			
		}
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public new PivotViewInfo this[int index] { get { return (PivotViewInfo)base[index]; } }
		public new PivotCellsViewInfo CellsArea { get { return (PivotCellsViewInfo)base.CellsArea; } }
		public new PivotFieldsAreaViewInfo ColumnAreaFields { get { return (PivotFieldsAreaViewInfo)base.ColumnAreaFields; } }
		public new PivotFieldsAreaViewInfo RowAreaFields { get { return (PivotFieldsAreaViewInfo)base.RowAreaFields; } }
		public bool IsMultiSelect { get { return CellsArea.IsMultiSelect; } }
		public bool IsControlDown { get { return CellsArea.IsControlDown; } }
		public bool IsShiftDown { get { return CellsArea.IsShiftDown; } }
		public Dictionary<RepositoryItem, BaseEditViewInfo> EditViewInfoCache {
			get {
				if(editViewInfoCache == null)
					editViewInfoCache = new Dictionary<RepositoryItem, BaseEditViewInfo>();
				return editViewInfoCache;
			}
		}
		#region Scrollbars
		public override bool IsHScrollBarVisible { get { return MaximumLeftTopCoord.X > 0; } }
		public override bool IsVScrollBarVisible { get { return MaximumLeftTopCoord.Y > 0; } }
		public override bool IsPrefilterPanelVisible { get { return Data.OptionsCustomization.AllowPrefilter && !ReferenceEquals(Data.Prefilter.Criteria, null); } }
		public override ScrollArgs HScrollBarInfo {
			get {
				EnsureIsCalculated();
				return GetScrollBarInfo(LeftTopCoord.X, MaximumLeftTopCoord.X, HorizontalScrollBarCount);
			}
		}
		public override ScrollArgs VScrollBarInfo {
			get {
				EnsureIsCalculated();
				return GetScrollBarInfo(LeftTopCoord.Y, MaximumLeftTopCoord.Y, VerticalScrollBarCount);
			}
		}
		public override bool IsHorzScrollControl { get { return Data.OptionsBehavior.HorizontalScrolling == PivotGridScrolling.Control; } }
		int HorizontalScrollBarCount {
			get {
				int count = CellsArea.ColumnCount;
				if(IsHorzScrollControl)
					count += RowAreaFields.ScrollColumnCount;
				return count;
			}
		}
		int VerticalScrollBarCount { get { return CellsArea.RowCount; } }
		public override int CellsLeftCoord {
			get {
				int x = LeftTopCoord.X;
				if(IsHorzScrollControl) {
					return x < RowAreaFields.ScrollColumnCount ? 0 : x - RowAreaFields.ScrollColumnCount;
				}
				return x;
			}
		}
		int HorizontalBoundsOffset {
			get {
				int offset = CellsArea.OffsetByLeftTopCoord(true);
				if(IsHorzScrollControl)
					offset += RowAreaFields.ScrollOffset;
				return offset;
			}
		}
		ScrollArgs GetScrollBarInfo(int lefttop, int maximum, int count) {
			ScrollArgs e = new ScrollArgs();
			if(maximum > 0) {
				e.Value = lefttop;
				e.Maximum = 0;
				e.Maximum = count - 1;
				e.SmallChange = 1;
				e.LargeChange = count - maximum;
			}
			return e;
		}
		#endregion
		#region Selection
		int selectionSetting;
		bool IsSelectionLocked { get { return selectionSetting != 0; } }
		public Point FocusedCell {
			get { return focusedCell; }
			set { SetFocusedCell(value, true); }
		}
		protected void SetFocusedCell(Point value, bool makeVisible) {
			if(FocusedCell == value) return;
			if(!CellsArea.IsCellValid(value)) return;
			this.focusedCell = value;
			if(!(IsMultiSelect && IsControlDown))
				Selection = Rectangle.Empty;
			if(makeVisible)
				MakeCellVisible(FocusedCell);
			Data.FocusedCellChanged();
		}
		public Rectangle Selection {
			get { return Data.OptionsSelection.CellSelection ? fSelection.Rectangle : Rectangle.Empty; }
			set {
				if(IsSelectionLocked || !Data.OptionsSelection.CellSelection || Selection.Equals(value))
					return;
				this.selectionSetting++;
				Rectangle oldSelection = fSelection.Rectangle;
				if(IsSelectionEqualsFocusedCell(value) || (value.Width == 1 && value.Height == 1)) {
					fSelection.Clear();
					FocusedCell = value.Location;
				} else
					fSelection.Rectangle = value;
				CorrectSelection(oldSelection);
				if(!Selection.IsEmpty && !IsFocusedCellInSelectionCorners) {
					SetFocusedCell(LastSelection.Location, false);
				}
				SelectionChanged();
				this.selectionSetting--;
			}
		}
		Rectangle FullSelection {
			get {
				Rectangle bounds = SelectedCells.Rectangle;
				if(!bounds.Contains(FocusedCell))
					bounds = Rectangle.Union(bounds, new Rectangle(FocusedCell, new Size(1, 1)));
				return bounds;
			}
		}
		public ReadOnlyCells SelectedCells {
			get { return Data.OptionsSelection.CellSelection ? fSelection.Cells : ReadOnlyCells.Empty; }
		}
		public void SetSelection(Point[] points) {
			if(!Data.OptionsSelection.CellSelection) return;
			fSelection.SetSelection(points);
			SelectionChanged();
		}
		public void AddSelection(Rectangle selection) {
			if(!Data.OptionsSelection.CellSelection) return;
			Rectangle oldSelection = Selection;
			if(IsSelectionEqualsFocusedCell(selection))
				fSelection.LoadLastState();
			else
				fSelection.Add(selection);
			CorrectSelection(oldSelection);
			SelectionChanged();
		}
		public void MoveSelectionTo(Point selectionTo) {
			Rectangle selectionRectangle = new Rectangle(Math.Min(selectionTo.X, FocusedCell.X), Math.Min(selectionTo.Y, FocusedCell.Y),
				Math.Abs(selectionTo.X - FocusedCell.X) + 1, Math.Abs(selectionTo.Y - FocusedCell.Y) + 1);
			AddSelection(selectionRectangle);
		}
		public void SubtractSelection(Rectangle selection) {
			fSelection.Subtract(selection);
			SelectionChanged();
		}
		public void StartSelection(bool isMultiSelect) {
			fSelection.StartSelection(isMultiSelect, IsControlDown, IsShiftDown);
			SelectionChanged();
		}
		void SelectionChanged() {
			Invalidate(Bounds);
			if(fSelection.IsChanged) {
				Data.CellSelectionChanged();
				fSelection.IsChanged = false;
			}
		}
		void CorrectFocusedCell() {
			if(this.focusedCell.X >= CellsArea.ColumnCount) this.focusedCell.X = CellsArea.ColumnCount - 1;
			if(this.focusedCell.Y >= CellsArea.RowCount) this.focusedCell.Y = CellsArea.RowCount - 1;
			if(this.focusedCell.X < 0) this.focusedCell.X = 0;
			if(this.focusedCell.Y < 0) this.focusedCell.Y = 0;
		}
		void CorrectSelection(Rectangle lastSelection) {
			fSelection.CorrectSelection(CellsArea.ColumnCount, CellsArea.RowCount,
				Data.OptionsSelection.MaxWidth, Data.OptionsSelection.MaxHeight, lastSelection);
		}
		Rectangle SelectionBounds {
			get {
				Rectangle selection = Selection;
				if(selection.IsEmpty) return Rectangle.Empty;
				Point leftTop = CellsArea.GetCellBounds(selection.Location).Location;
				Rectangle bounds = CellsArea.GetCellBounds(new Point(selection.Right - 1, selection.Bottom - 1));
				return new Rectangle(leftTop.X, leftTop.Y, bounds.Right - leftTop.X, bounds.Bottom - leftTop.Y);
			}
		}
		Rectangle LastSelection { get { return fSelection.LastSelection.Rectangle; } }
		bool IsMultipleSelected { get { return SelectedCells.Count != Selection.Width * Selection.Height; } }
		bool IsSelectionEqualsFocusedCell(Rectangle selection) { return selection.Location == FocusedCell && selection.Width == 1 && selection.Height == 1; }
		public bool IsLastSelectionIsMultiSelect { get { return fSelection.IsLastSelectionIsMultiSelect; } }
		public bool IsLastSelectionIsShiftDown { get { return fSelection.IsLastSelectionIsShiftDown; } }
		bool IsFocusedCellInSelectionCorners {
			get {
				Rectangle lastSelection = LastSelection;
				return FocusedCell.Equals(lastSelection.Location)
					|| FocusedCell.Equals(new Point(lastSelection.Right - 1, lastSelection.Y))
					|| FocusedCell.Equals(new Point(lastSelection.X, lastSelection.Bottom - 1))
					|| FocusedCell.Equals(new Point(lastSelection.Right - 1, lastSelection.Bottom - 1));
			}
		}
		#endregion
		public override bool CustomDrawFieldValue(ViewInfoPaintArgs e, PivotFieldsAreaCellViewInfoBase fieldCellViewInfo, HeaderObjectInfoArgs info, 
			HeaderObjectPainter painter) {
			return Data.CustomDrawFieldValue(e, (PivotFieldsAreaCellViewInfo)fieldCellViewInfo, info, painter);
		}
		public override bool DrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs e, HeaderObjectPainter painter) {
			return Data.CustomDrawFieldHeader(headerViewInfo, e, painter);
		}
		public override bool DrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs e, Rectangle bounds) {
			return Data.CustomDrawHeaderArea(headersViewInfo, e, bounds);
		}
		public override Rectangle ScrollableRectangle {
			get {
				Rectangle bounds = Data.ScrollableRectangle;
				if(IsPrefilterPanelVisible) bounds.Height -= PivotPrefilterPanelViewInfoBase.CalcHeight(Data);
				return bounds;
			}
		}
		protected override Rectangle ClientRectangle { get { return Control != null ? Control.ClientRectangle : Rectangle.Empty; } }
		public override void ClientSizeChanged() {
			base.ClientSizeChanged();
			Point maxLeftTopCoord = MaximumLeftTopCoordCore;
			if(maxLeftTopCoord.X < LeftTopCoord.X || maxLeftTopCoord.Y < LeftTopCoord.Y) {
				SetLeftTopCoordCore(maxLeftTopCoord);
			}
		}
		protected override void Invalidate(Rectangle bounds) {
			if(Control != null)
				Control.Invalidate(bounds);
		}
		protected IViewInfoControl Control { get { return control; } }
		public override Point LeftTopCoord {
			get {
				return base.LeftTopCoord;
			}
			set {
				base.LeftTopCoord = value;
				if(Control != null)
					Control.InvalidateScrollBars();
			}
		}
		protected override void SetLeftTopCoordOffset() {
			int horizontalOffset = -HorizontalBoundsOffset;
			if(IsHorzScrollControl)
				BoundsOffset = new Size(horizontalOffset, 0);
			else
				ColumnAreaFields.BoundsOffset = new Size(horizontalOffset, 0);
			RowAreaFields.BoundsOffset = new Size(0, -CellsArea.OffsetByLeftTopCoord(false));
		}
		public override PivotGridViewInfoState State { get { return state; } }
		protected override int FilterHeadersHeight {
			get {
				int height = HeadersHeight;
				if(Data.OptionsView.ShowFilterSeparatorBar) {
					height += 2 * Data.OptionsView.FilterSeparatorBarPadding + 1;
				}
				return height;
			}
		}
		protected override void OnAfterChildrenCalculated() {
			base.OnAfterChildrenCalculated();
			CorrectFocusedCell();
			CorrectSelection(Selection);
		}
		protected override int CalcFieldHeight(bool isHeader, int lineCount) {
			if(Control == null || Control.ControlOwner == null)
				return lineCount * 20; 
			HeaderObjectInfoArgs headerInfo = new HeaderObjectInfoArgs();
			headerInfo.SetAppearance(isHeader ? Data.PaintAppearance.FieldHeader : Data.PaintAppearance.FieldValue);
			if(isHeader) {
				headerInfo.InnerElements.Add(Data.ActiveLookAndFeel.Painter.SortedShape, new SortedShapeObjectInfoArgs());
				headerInfo.InnerElements.Add(FilterButtonHelper.GetPainter(Data.ActiveLookAndFeel), new GridFilterButtonInfoArgs());
				if(Data.HeaderImages != null) {
					headerInfo.InnerElements.Add(new GlyphElementPainter(), new GlyphElementInfoArgs(Data.HeaderImages, 0, null));
				}
			} else {
				if(Data.ValueImages != null) {
					headerInfo.InnerElements.Add(new GlyphElementPainter(), new GlyphElementInfoArgs(Data.ValueImages, 0, null));
				}
			}
			headerInfo.Caption = GetMeasureCaption(lineCount);
			headerInfo.Bounds = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			int height = 0;
			try {
				headerInfo.Cache = graphicsCache;
				height = Data.ActiveLookAndFeel.Painter.Header.CalcObjectMinBounds(headerInfo).Height;
				if(!isHeader) {
					float cellHeight = Data.PaintAppearance.Cell.CalcTextSize(graphics, headerInfo.Caption, Int32.MaxValue).Height;
					if(cellHeight > height)
						height = (int)(cellHeight + 0.5);
				}
			} finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return height;
		}
		protected string GetMeasureCaption(int lineCount) {
			string str = "Qq";
			StringBuilder res = new StringBuilder();
			for(int i = 0; i < lineCount; i++) {
				res.Append(str);
				if(i != lineCount - 1)
					res.AppendLine();
			}
			return res.ToString();
		}
		public PivotGridFieldBase GetFieldAt(Point pt) {
			EnsureIsCalculated();
			for(int i = 0; i < ChildCount; i++) {
				PivotGridFieldBase field = this[i].GetFieldAt(pt);
				if(field != null) return field;
			}
			return null;
		}
		public void CopySelectionToClipboard() {
			EnsureIsCalculated();
			Clipboard.SetDataObject(GetSelectionString(), true, 3, 100);
		}
		public string GetSelectionString() {
			if(SelectedCells.IsEmpty) return GetCellString(FocusedCell) + Environment.NewLine;
			Rectangle bounds = FullSelection;
			StringBuilder result = new StringBuilder();
			List<List<Point>> sortedSelection = GetSortedSelection(true);
			if(Data.OptionsBehavior.CopyToClipboardWithFieldValues) {
				for(int i = 0; i < ColumnAreaFields.LevelCount; i++) {
					for(int j = 0; j < RowAreaFields.LevelCount; j++) result.Append('\t');
					for(int colIndex = bounds.X; colIndex < bounds.Right; colIndex++) {						
						PivotFieldsAreaCellViewInfoBase cellViewInfo = ColumnAreaFields.GetItem(colIndex, i);
						if(cellViewInfo != null)
							result.Append(cellViewInfo.DisplayText);
						result.Append('\t');
					}
					result.Append(Environment.NewLine);
				}
			}
			for(int rowIndex = 0; rowIndex < bounds.Height; rowIndex++) {
				if(Data.OptionsBehavior.CopyToClipboardWithFieldValues) {
					for(int i = 0; i < RowAreaFields.LevelCount; i++) {
						PivotFieldsAreaCellViewInfoBase cellViewInfo = RowAreaFields.GetItem(rowIndex + bounds.Top, i);
						if(cellViewInfo != null)
							result.Append(cellViewInfo.DisplayText);
						result.Append('\t');
					}
				}
				List<Point> row = sortedSelection[rowIndex];
				if(row.Count > 0) {
					for(int colIndex = bounds.X, i = 0; colIndex < bounds.Right; colIndex++) {
						if(i == row.Count || row[i].X != colIndex) result.Append('\t');
						else {
							result.Append(GetCellString(row[i]));
							if(colIndex != bounds.Right - 1) result.Append('\t');
							i++;
						}
					}
				}
				result.Append(Environment.NewLine);
			}
			return result.ToString();
		}
		string GetCellString(Point point) {
			PivotCellViewInfoBase viewInfo = CellsArea.CreateCellViewInfo(point.X, point.Y);
			return viewInfo.Text;
		}
		string GetFieldValueString(PivotGridField field, int index) {
			if(!field.IsColumnOrRow) return null;
			int visibleIndex = Data.GetVisibleIndex(field, index);
			return field.GetDisplayText(Data.GetFieldValue(field, visibleIndex, visibleIndex));
		}
		#region SortedSelectionCells				
		public List<List<Point>> GetSortedSelection(bool includeFocusedCell) {
			Rectangle bounds = includeFocusedCell ? FullSelection : Selection;
			List<List<Point>> result = new List<List<Point>>(bounds.Height);
			for(int i = 0; i < bounds.Height; i++)
				result.Add(new List<Point>());
			foreach(Point point in SelectedCells)
				result[point.Y - bounds.Y].Add(point);
			if(includeFocusedCell) {
				List<Point> rowFocused = result[FocusedCell.Y - bounds.Y];
				if(!rowFocused.Contains(FocusedCell))
					rowFocused.Add(FocusedCell);
			}
			PointsComparer comparer = new PointsComparer();
			foreach(List<Point> row in result)
				row.Sort(comparer);
			return result;
		}
		#endregion
		public bool IsDragging { get { return this.dragManager != null; } }
		public void StartDragging(PivotHeaderViewInfo headerViewInfo) {
			dragManager = new PivotGridDragManager(headerViewInfo);
			dragManager.DoDragDrop();
			dragManager.Dispose();
			dragManager = null;
		}
		public void StopDragging() {
			dragManager.Dispose();
			dragManager = null;
		}
		public override PivotGridFieldBase GetSizingField(int x, int y) {
			if(!IsReady) EnsureIsCalculated();
			if(IsPrefilterPanelVisible && PrefilterPanel.Bounds.Contains(x, y)) return null;
			if(CellsArea.Bounds.Contains(new Point(x, y))) {
				if(RowAreaFields.Bounds.Contains(new Point(x - FrameBorderWidth, y)))
					x -= FrameBorderWidth;
			}
			if(RowAreaFields.Bounds.Contains(new Point(x, y)))
				return RowAreaFields.GetSizingField(new Point(x, y));
			if(ColumnAreaFields.Bounds.Contains(new Point(x, y)))
				return ColumnAreaFields.GetSizingField(new Point(x, y));
			return null;
		}
		bool CanResizeField(int newResizingX) {
			return this.resizingField.Width + newResizingX - this.initResizingX >= this.resizingField.MinWidth;
		}
		protected override PivotHeadersViewInfoBase CreateHeadersViewInfo(int i) {
			return new PivotHeadersViewInfo(this, (PivotArea)i);
		}
		public override void KeyDown(KeyEventArgs e) {
			base.KeyDown(e);
			KeyDown(e.KeyCode, e.Control, e.Shift);
		}
		public void KeyDown(Keys keyCode, bool control, bool shift) {
			Point newFocusedCell = GetKeyDownFocusedCell(shift);
			switch(keyCode) {
				case Keys.Escape:
					if(this.resizingField != null)
						StopResizing();
					break;
				case Keys.Left:
					newFocusedCell = new Point(newFocusedCell.X - 1, newFocusedCell.Y);
					break;
				case Keys.Down:
					newFocusedCell = new Point(newFocusedCell.X, newFocusedCell.Y + 1);
					break;
				case Keys.Right:
					newFocusedCell = new Point(newFocusedCell.X + 1, newFocusedCell.Y);
					break;
				case Keys.Up:
					newFocusedCell = new Point(newFocusedCell.X, newFocusedCell.Y - 1);
					break;
				case Keys.Home:
					if(!control)
						newFocusedCell = new Point(0, newFocusedCell.Y);
					else newFocusedCell = new Point(newFocusedCell.X, 0);
					break;
				case Keys.End:
					if(!control)
						newFocusedCell = new Point(CellsArea.ColumnCount - 1, newFocusedCell.Y);
					else newFocusedCell = new Point(newFocusedCell.X, CellsArea.RowCount - 1);
					break;
				case Keys.PageDown: {
						int visibleRowCount = CellsArea.GetVisibleRowCount(ScrollableSize.Height);
						int newY = newFocusedCell.Y < LeftTopCoord.Y + visibleRowCount - 1 ? LeftTopCoord.Y + visibleRowCount - 1 : newFocusedCell.Y + visibleRowCount;
						newFocusedCell = new Point(newFocusedCell.X, newY < CellsArea.RowCount ? newY : CellsArea.RowCount - 1);
						break;
					}
				case Keys.PageUp: {
						int newY = newFocusedCell.Y > LeftTopCoord.Y ? LeftTopCoord.Y : newFocusedCell.Y - CellsArea.GetVisibleRowCount(ScrollableSize.Height);
						newFocusedCell = new Point(newFocusedCell.X, newY >= 0 ? newY : 0);
						break;
					}
				case Keys.A: {
						if(control) {
							FocusedCell = Point.Empty;
							Selection = new Rectangle(0, 0, CellsArea.ColumnCount, CellsArea.RowCount);
							return;
						}
						break;
					}
				case Keys.Insert:
				case Keys.C:
					if(control)
						CopySelectionToClipboard();
					break;
			}
			if(!newFocusedCell.Equals(FocusedCell) || !Selection.IsEmpty) {
				if(shift) {
					if(CellsArea.IsCellValid(newFocusedCell) && !(keyCode == Keys.Shift || keyCode == Keys.ShiftKey)) {
						MakeCellVisible(newFocusedCell);
						MoveSelectionTo(newFocusedCell);
					}
				} else {
					if(newFocusedCell != FocusedCell) {
						FocusedCell = newFocusedCell;
						StartSelection(false);
						Selection = Rectangle.Empty;
					}
				}
			}
		}
		Point GetKeyDownFocusedCell(bool shift) {
			if(!shift || Selection.IsEmpty) return FocusedCell;
			Rectangle lastSelection = fSelection.LastSelection.Rectangle;
			return new Point(lastSelection.X == FocusedCell.X ? lastSelection.Right - 1 : lastSelection.X,
				lastSelection.Y == FocusedCell.Y ? lastSelection.Bottom - 1 : lastSelection.Y);
		}
		bool fMouseDown;
		PivotArea fMouseArea;
		int fMouseStartIndexMin, fMouseStartIndexMax;
		Point clickedCellCoord;
		public override void MouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				clickedCellCoord = CellsArea.GetCellCoordAt(e.Location);
				if(clickedCellCoord != FocusedCell && !SelectedCells.Contains(clickedCellCoord))
					clickedCellCoord = PivotCellViewInfo.EmptyCoord;
				this.resizingField = GetSizingField(e.X, e.Y);
				if(this.resizingField != null) {
					this.initResizingX = e.X;
					this.state = PivotGridViewInfoState.FieldResizing;
					DrawSizingLine(e.X, true);
					return;
				}
			}
			if(Data.ControlOwner != null && e.Button == MouseButtons.Middle && e.Clicks == 1) {
				if(this.CellsArea.ControlBounds.Contains(e.X, e.Y)) {
					Data.ControlOwner.Focus();
					this.scroller.Start(Data.ControlOwner);
				}
			}
			StartColumnRowSelection(e);
			base.MouseDown(e);
		}
		PivotCellViewInfo GetCellInfo(Point point) {
			return CellsArea.GetCellViewInfoAt(point);
		}
		public override void MouseMove(MouseEventArgs e) {
			if(!IsReady) return;
			if(this.resizingField != null) {
				if(CanResizeField(e.X))
					DrawSizingLine(e.X, true);
				return;
			}
			if(GetSizingField(e.X, e.Y) != null)
				this.state = PivotGridViewInfoState.FieldResizing;
			else this.state = PivotGridViewInfoState.Normal;
			PerformColumnRowSelection(e);
			base.MouseMove(e);
		}
		public override void MouseUp(MouseEventArgs e) {
			if(this.resizingField != null) {
				if(CanResizeField(e.X))
					this.resizingField.Width += e.X - this.initResizingX;
				else this.resizingField.Width = this.resizingField.MinWidth;
				StopResizing();
				return;
			}
			StopColumnRowSelection();
			base.MouseUp(e);
		}
		void StartColumnRowSelection(MouseEventArgs e) {
			if(!Data.OptionsSelection.CellSelection) return;
			BaseViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y);
			if(e.Button == MouseButtons.Left && viewInfo is PivotFieldsAreaCellViewInfo && (!IsPrefilterPanelVisible || !PrefilterPanel.Bounds.Contains(e.Location))) {
				PivotFieldsAreaCellViewInfo activeViewInfo = (PivotFieldsAreaCellViewInfo)viewInfo;
				fMouseDown = true;
				fMouseArea = activeViewInfo.Item.Area;
				fMouseStartIndexMin = activeViewInfo.MinLastLevelIndex;
				fMouseStartIndexMax = activeViewInfo.MaxLastLevelIndex;
				StartSelection(IsMultiSelect);
				if(activeViewInfo.Item.Area == PivotArea.RowArea)
					SelectAllColumnRow(new Rectangle(0, activeViewInfo.MinLastLevelIndex, CellsArea.ColumnCount, activeViewInfo.MaxLastLevelIndex - activeViewInfo.MinLastLevelIndex + 1));
				else
					SelectAllColumnRow(new Rectangle(activeViewInfo.MinLastLevelIndex, 0, activeViewInfo.MaxLastLevelIndex - activeViewInfo.MinLastLevelIndex + 1, CellsArea.RowCount));
			}
		}
		void PerformColumnRowSelection(MouseEventArgs e) {
			if(!Data.OptionsSelection.CellSelection) return;
			if(fMouseDown) {
				Point p = e.Location;
				if(fMouseArea == PivotArea.RowArea) {
					p.X = Math.Max(p.X, RowAreaFields.Bounds.Left + 1);
					p.X = Math.Min(p.X, RowAreaFields.Bounds.Right + 1);
					p.Y = Math.Max(p.Y, RowAreaFields.Bounds.Top + 1);
					p.Y = Math.Min(p.Y, RowAreaFields.Bounds.Bottom - 1);
				}
				if(fMouseArea == PivotArea.ColumnArea) {
					p.X = Math.Max(p.X, ColumnAreaFields.Bounds.Left + 1);
					p.X = Math.Min(p.X, ColumnAreaFields.Bounds.Right - 1);
					p.Y = Math.Max(p.Y, ColumnAreaFields.Bounds.Top + 1);
					p.Y = Math.Min(p.Y, ColumnAreaFields.Bounds.Bottom - 1);
				}
				PivotFieldsAreaCellViewInfo activeViewInfo = GetViewInfoAtPoint(p) as PivotFieldsAreaCellViewInfo;
				if(activeViewInfo != null) {
					int minIndex = Math.Min(fMouseStartIndexMin, activeViewInfo.MinLastLevelIndex),
						maxIndex = Math.Max(fMouseStartIndexMax, activeViewInfo.MaxLastLevelIndex);
					if(activeViewInfo.Item.Area == PivotArea.RowArea)
						SelectAllColumnRow(new Rectangle(0, minIndex, CellsArea.ColumnCount, maxIndex - minIndex + 1));
					else
						SelectAllColumnRow(new Rectangle(minIndex, 0, maxIndex - minIndex + 1, CellsArea.RowCount));
				}
			}
		}
		void SelectAllColumnRow(Rectangle columnRow) {
			if(columnRow.Width == 1 && columnRow.Height == 1) {
				FocusedCell = columnRow.Location;
				if(!(IsMultiSelect && IsControlDown))
					Selection = Rectangle.Empty;
			} else {
				if(IsMultiSelect && IsControlDown) {
					AddSelection(columnRow);
					FocusedCell = columnRow.Location;
				} else {
					if(IsMultiSelect && IsShiftDown) {
						if(columnRow.Height == 1) {
							Rectangle selection = new Rectangle(
								columnRow.X, Math.Min(columnRow.Y, FocusedCell.Y),
								columnRow.Width, Math.Max(columnRow.Bottom, FocusedCell.Y) - Math.Min(columnRow.Y, FocusedCell.Y));
							AddSelection(selection);
						}
						if(columnRow.Width == 1) {
							Rectangle selection = new Rectangle(
								Math.Min(columnRow.X, FocusedCell.X), FocusedCell.Y,
								Math.Max(columnRow.Right, FocusedCell.X) - Math.Min(columnRow.X, FocusedCell.X), columnRow.Height);
							AddSelection(selection);
						}
					} else
						Selection = columnRow;
				}				
			}
		}
		void StopColumnRowSelection() {
			if(fMouseDown) fMouseDown = false;
		}
		public override void DoubleClick() {
			if(this.resizingField != null) {
				BestFit((PivotGridField)this.resizingField);
			} else base.DoubleClick();
		}
		public bool AcceptDragDrop(Point pt) {
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].ControlBounds.Contains(pt))
					return this[i].AcceptDragDrop;
			}
			return false;
		}
		public Rectangle GetDragDrawRectangle(PivotGridField field, Point pt) {
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(!this[i].ControlBounds.Contains(pt)) continue;
				return this[i].GetDragDrawRectangle(field, pt);
			}
			return Rectangle.Empty;
		}
		public void HighLightArea(Point pt) {
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].ControlBounds.Contains(pt)) {
					HighLightedArea = this[i];
					return;
				}
			}
			HighLightedArea = null;
		}
		public int GetNewFieldPosition(PivotGridField field, Point pt, out PivotArea area) {
			area = PivotArea.FilterArea;
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(!this[i].ControlBounds.Contains(pt)) continue;
				int newAreaIndex = this[i].GetNewFieldPosition(field, pt, out area);
				if(newAreaIndex > -1) {
					return newAreaIndex;
				}
			}
			return -1;
		}
		void DrawSizingLine(int x, bool show) {
			EnsureIsCalculated();
			int top = RowAreaFields.Bounds.Top;
			if(this.resizingField.Area != PivotArea.RowArea)
				top -= Data.DefaultFieldHeight;
			int bottom = ScrollableRectangle.Bottom - ScrollBarSize.Height;
			DrawSizingLine(this.drawResizingLineX, top, bottom);
			this.drawResizingLineX = show ? x : -1;
			DrawSizingLine(this.drawResizingLineX, top, bottom);
		}
		void DrawSizingLine(int x, int top, int bottom) {
			if(x >= 0 && Control.ControlOwner != null) {
				Point start = new Point(x, top);
				Point end = new Point(x, bottom);
				SplitterLineHelper.Default.DrawReversibleLine(Control.ControlOwner.Handle, start, end);
			}
		}
		void StopResizing() {
			DrawSizingLine(-1, false);
			this.initResizingX = 0;
			this.drawResizingLineX = -1;
			this.resizingField = null;
			this.state = PivotGridViewInfoState.Normal;
			if(Data.CustomizationForm != null)
				Data.CustomizationForm.Refresh();
		}
		protected override void InternalClear() {
			base.InternalClear();
			this.state = PivotGridViewInfoState.Normal;
			if(this.editViewInfoCache != null)
				this.editViewInfoCache.Clear();
		}
		protected override bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs e, Rectangle bounds) {
			return Data.CustomDrawEmptyArea(appearanceOwner, e, bounds);
		}
		protected override PivotCellsViewInfoBase CreateCellsViewInfo() {
			return new PivotCellsViewInfo(this);
		}
		protected override PivotFieldsAreaViewInfoBase CreateFieldsAreawViewInfo(bool isColumn) {
			return new PivotFieldsAreaViewInfo(this, isColumn);
		}
		protected override PivotPrefilterPanelViewInfoBase CreatePrefilterPanelViewInfo() {
			return new PivotPrefilterPanelViewInfo(this);
		}
		public virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			for(int i = 0; i < ChildCount; i++)
				if(this[i].ControlBounds.Contains(pt))
					return this[i].GetToolTipObjectInfo(pt);
			return null;
		}
		public PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			EnsureIsCalculated();
			if(!Bounds.Contains(hitPoint))
				return new PivotGridHitInfo(hitPoint);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].ControlBounds.Contains(hitPoint))
					return this[i].CalcHitInfo(hitPoint);
			}
			return new PivotGridHitInfo(hitPoint);
		}
		protected override void OnAfterChildCalculated(BaseViewInfo viewInfo) {
			base.OnAfterChildCalculated(viewInfo);
			if(Control != null) Control.InvalidateScrollBars();
		}
		public BaseEditViewInfo GetEditViewInfo(RepositoryItem repositoryItem) {
			if(!EditViewInfoCache.ContainsKey(repositoryItem)) {
				EditViewInfoCache.Add(repositoryItem, CreateEditViewInfo(repositoryItem));
			}
			return EditViewInfoCache[repositoryItem];
		}
		public DetailLevel GetEditDetailLevel(PivotCellViewInfo cellViewInfo) {
			PivotGridField dataField = cellViewInfo.DataField;
			bool focusedCell = cellViewInfo.ColumnIndex == FocusedCell.X && cellViewInfo.RowIndex == FocusedCell.Y;
			PivotShowButtonModeEnum mode = dataField.Options.ShowButtonMode;
			if(mode == PivotShowButtonModeEnum.Default)
				mode = Data.OptionsView.ShowButtonMode;
			if(mode == PivotShowButtonModeEnum.Default)
				mode = PivotShowButtonModeEnum.ShowForFocusedCell;
			bool showButtons = (mode == PivotShowButtonModeEnum.ShowAlways) ||
				(focusedCell && mode == PivotShowButtonModeEnum.ShowForFocusedCell);
			return showButtons ? DetailLevel.Full : DetailLevel.Minimum;
		}
		protected BaseEditViewInfo CreateEditViewInfo(RepositoryItem repositoryItem) {
			BaseEditViewInfo res = repositoryItem.CreateViewInfo();
			res.InplaceType = InplaceType.Grid;
			return res;
		}
	}
	public class PivotGridViewInfoBase : BaseViewInfo {
		public const int FieldResizingOffset = 2;
		const int firstLastHeaderWidthOffset = 1;
		PivotGridViewInfoData data;
		PivotFieldsAreaViewInfoBase columnAreaFields;
		PivotFieldsAreaViewInfoBase rowAreaFields;
		PivotCellsViewInfoBase cellsArea;
		PivotPrefilterPanelViewInfoBase prefilterPanel;
		protected Size fScrollableSize;
		PivotHeadersViewInfoBase[] headers;
		Point leftTopCoord;
		int defaultHeaderHeight;
		SortedList<int, int> fieldHeights;
		PivotViewInfo highLightedArea;
		public PivotGridViewInfoBase(PivotGridViewInfoData data)
			: base() {
			this.leftTopCoord = Point.Empty;
			this.data = data;
			this.rowAreaFields = null;
			this.columnAreaFields = null;
			this.cellsArea = null;
			this.highLightedArea = null;
			this.headers = new PivotHeadersViewInfoBase[Enum.GetValues(typeof(PivotArea)).Length];
			this.fieldHeights = new SortedList<int, int>();
			InternalClear();
			VisualItems.Cleared += OnVisualItemsCleared;
		}
		public override void Dispose() {
			base.Dispose();
			if(VisualItems != null)
				VisualItems.Cleared -= OnVisualItemsCleared;
		}
		public PivotGridViewInfoData Data { get { return data; } }
		public PivotVisualItemsBase VisualItems { get { return Data != null ? Data.VisualItems : null; } }
		public new PivotViewInfo this[int index] { get { return (PivotViewInfo)base[index]; } }
		public PivotFieldsAreaViewInfoBase ColumnAreaFields { get { return columnAreaFields; } }
		public PivotFieldsAreaViewInfoBase RowAreaFields { get { return rowAreaFields; } }
		public PivotCellsViewInfoBase CellsArea { get { return cellsArea; } }
		public PivotPrefilterPanelViewInfoBase PrefilterPanel { get { return prefilterPanel; } }
		public PivotHeadersViewInfoBase GetHeader(PivotArea area) { return headers[(int)area]; }
		public int HeaderCount { get { return this.headers.Length; } }
		public virtual bool CanShowHeader(PivotArea area) {
			return Data.OptionsView.GetShowHeaders(area);
		}
		public virtual bool IsPrefilterPanelVisible { get { return false; } }
		public virtual bool IsEnabled { get { return Data.IsEnabled; } }
		protected void CreateHeaders() {
			for(int i = 0; i < HeaderCount; i++)
				headers[i] = CreateHeadersViewInfo(i);
		}
		protected virtual PivotHeadersViewInfoBase CreateHeadersViewInfo(int i) {
			return new PivotHeadersViewInfoBase(this, (PivotArea)i);
		}
		public PivotHeadersViewInfoBase ColumnHeaders { get { return GetHeader(PivotArea.ColumnArea); } }
		public PivotHeadersViewInfoBase RowHeaders { get { return GetHeader(PivotArea.RowArea); } }
		public PivotHeadersViewInfoBase FilterHeaders { get { return GetHeader(PivotArea.FilterArea); } }
		public PivotHeadersViewInfoBase DataHeaders { get { return GetHeader(PivotArea.DataArea); } }
		public virtual int HeaderWidthOffset { get { return Data.OptionsView.HeaderWidthOffset; } }
		public virtual int HeaderHeightOffset { get { return Data.OptionsView.HeaderHeightOffset; } }
		public virtual int FirstLastHeaderWidthOffset { get { return firstLastHeaderWidthOffset; } }
		public virtual Point LeftTopCoord {
			get { return leftTopCoord; }
			set {
				if(LeftTopCoord == value) return;
				EnsureIsCalculated();
				SetLeftTopCoordCore(value);
			}
		}
		public Point MaximumLeftTopCoord {
			get {
				EnsureIsCalculated();
				return MaximumLeftTopCoordCore;
			}
		}
		protected Point MaximumLeftTopCoordCore {
			get {
				this.fScrollableSize = ScrollableRectangle.Size;
				if(!IsHorzScrollControl) {
					this.fScrollableSize.Width -= RowAreaFields.Bounds.Width;
				}
				this.fScrollableSize.Height -= ColumnAreaFields.ControlBounds.Bottom;
				int x = GetMaximumLeftTopCoordX(this.fScrollableSize);
				if(x > 0) {
					this.fScrollableSize.Height -= ScrollBarSize.Height;
				}
				int y = CellsArea.GetMaximumLeftTopCoord(this.fScrollableSize).Y;
				if(y > 0) {
					this.fScrollableSize.Width -= ScrollBarSize.Width;
					x = GetMaximumLeftTopCoordX(this.fScrollableSize);
					if(x > 0 && this.fScrollableSize.Height == ScrollableRectangle.Height) {
						this.fScrollableSize.Height -= ScrollBarSize.Height;
						y = CellsArea.GetMaximumLeftTopCoord(this.fScrollableSize).Y;
					}
				}
				if(CellsArea.Bounds.Width < this.fScrollableSize.Width)
					this.fScrollableSize.Width = CellsArea.Bounds.Width;
				if(CellsArea.Bounds.Height < this.fScrollableSize.Height)
					this.fScrollableSize.Height = CellsArea.Bounds.Height;
				return new Point(x, y);
			}
		}
		int GetMaximumLeftTopCoordX(Size size) {
			if(!IsHorzScrollControl) return CellsArea.GetMaximumLeftTopCoord(size).X;
			int totalSize = CellsArea.GetTotalSize(true, int.MaxValue) + RowAreaFields.Bounds.Width;
			if(size.Width > totalSize) return 0;
			int maxLeftTopCoordX = CellsArea.ColumnCount + RowAreaFields.LevelCount;
			maxLeftTopCoordX -= RowAreaFields.GetVisibleColumnCount(size.Width);
			if(size.Width - RowAreaFields.Bounds.Width > 0)
				maxLeftTopCoordX -= CellsArea.GetVisibleColumnCount(size.Width - RowAreaFields.Bounds.Width);
			return maxLeftTopCoordX;
		}
		public virtual int CellsLeftCoord {
			get {
				return LeftTopCoord.X;
			}
		}
		protected virtual int ScrollRowFieldsLeftCoordCount { get { return IsHorzScrollControl ? RowAreaFields.ScrollColumnCount : 0; } }
		public virtual bool IsHorzScrollControl { get { return false; } }
		public virtual Rectangle ScrollableRectangle { get { return ClientRectangle; } }
		protected virtual Rectangle ClientRectangle { get { return Rectangle.Empty; } }
		public Rectangle ScrollableBounds {
			get {
				return new Rectangle(new Point(ScrollableBoundsLeft, ColumnAreaFields.Bounds.Bottom), fScrollableSize);
			}
		}
		int ScrollableBoundsLeft {
			get { return RowAreaFields.Bounds.Right; }
		}
		public Size ScrollableSize {
			get {
				if(this.fScrollableSize.IsEmpty) {
					this.fScrollableSize = ScrollableRectangle.Size;
				}
				return this.fScrollableSize;
			}
		}
		public virtual bool IsHScrollBarVisible { get { return false; } }
		public virtual bool IsVScrollBarVisible { get { return false; } }
		public virtual void ClientSizeChanged() {
			Bounds = ClientRectangle;
			ClearOnClientSizeChanged();
			EnsureIsCalculated();
			SetFilterAndColumnHeadersWidth();
		}
		protected void ClearOnClientSizeChanged() {
			if(!IsReady || !CanShowHeader(PivotArea.FilterArea)) return;
			int oldHeight = FilterHeaders.Bounds.Height;
			FilterHeaders.Bounds.Height = FilterHeadersHeight;
			SetFilterAndColumnHeadersWidth();
			FilterHeaders.CorrectHeadersHeight();
			if(FilterHeaders.Bounds.Height != oldHeight)
				Clear();
		}
		public Size ScrollBarSize { 
			get { 
				return new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight); 
			} 
		}
		public int DefaultFieldHeight {
			get {
				return GetFieldHeight(1);
			}
		}
		public int DefaultHeaderHeight {
			get {
				if(defaultHeaderHeight <= 0)
					defaultHeaderHeight = CalcFieldHeight(true, 1);
				return defaultHeaderHeight;
			}
		}
		public int GetFieldHeight(int lineCount) {
			if(!fieldHeights.ContainsKey(lineCount))
				fieldHeights.Add(lineCount, CalcFieldHeight(false, lineCount));
			return fieldHeights[lineCount];
		}
		public void BestFit(PivotGridFieldBase field) {
			if(field == null || !field.Visible) return;
			EnsureIsCalculated();
			if(field.Area == PivotArea.RowArea)
				BestFitRowField(field);
			if(field.Area == PivotArea.DataArea)
				BestFitDataField(field);
			if(field.Area == PivotArea.ColumnArea)
				BestFitColumnField(field);
		}
		protected virtual void BestFitRowField(PivotGridFieldBase field) {
			field.Width = Math.Max(RowHeaders.GetBestWidth(field), RowAreaFields.GetBestWidth(field));
		}
		protected virtual void BestFitDataField(PivotGridFieldBase field) {
			if(Data.DataFieldCount == 1 && Data.RowFieldCount == 0) {
				field.Width = Math.Max(RowAreaFields.GetBestWidth(field), CellsArea.GetBestWidth(field));
			} else {
				field.Width = Math.Max(ColumnAreaFields.GetBestWidth(field), CellsArea.GetBestWidth(field));
			}
		}
		protected virtual void BestFitColumnField(PivotGridFieldBase field) {
			bool isDataFieldLastInArea = Data.OptionsDataField.Area == PivotDataArea.ColumnArea && Data.OptionsDataField.AreaIndex == Data.GetFieldCountByArea(PivotArea.ColumnArea);
			List<PivotGridFieldBase> fields = GetFieldsWithValues(Data.GetFieldsByArea(PivotArea.DataArea, false));
			if(fields.Count > 1 && isDataFieldLastInArea)
				return;
			int width = ColumnAreaFields.GetBestWidth(field);
			foreach(PivotGridFieldBase dataField in fields) {
				width = Math.Max(width, CellsArea.GetBestWidth(dataField));
			}
			field.Width = width;
		}
		List<PivotGridFieldBase> GetFieldsWithValues(List<PivotGridFieldBase> fields) {
			List<PivotGridFieldBase> fieldsWithValues = new List<PivotGridFieldBase>();
			foreach(PivotGridFieldBase field in fields)
				if(field.Options.ShowValues == true)
					fieldsWithValues.Add(field);
			return fieldsWithValues;
		}
		protected override void InternalClear() {
			base.InternalClear();
			this.fScrollableSize = Size.Empty;
			this.defaultHeaderHeight = 0;
			this.fieldHeights.Clear();
		}
		public PivotViewInfo HighLightedArea {
			get { return highLightedArea; }
			set {
				if(HighLightedArea == value) return;
				PivotViewInfo oldHighLightedArea = HighLightedArea;
				this.highLightedArea = value;
				if(oldHighLightedArea != null)
					oldHighLightedArea.InvalidateHighLight();
				if(HighLightedArea != null)
					HighLightedArea.InvalidateHighLight();
			}
		}
		public virtual bool AllowExpand { get { return true; } }
		public virtual bool CanHeaderSort { get { return true; } }
		public virtual bool CanHeaderFilter { get { return true; } }
		public virtual bool CustomDrawFieldValue(ViewInfoPaintArgs e, PivotFieldsAreaCellViewInfoBase fieldCellViewInfo,
			HeaderObjectInfoArgs info, HeaderObjectPainter painter) {
			return true;
		}
		public virtual bool DrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs e, HeaderObjectPainter painter) {
			return true;
		}
		public virtual bool DrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs e, Rectangle bounds) {
			return true;
		}
		protected override bool CheckControlBounds { get { return false; } }
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			AdjustPrefilterBounds();
			DrawHeadersAndFields(e);
			DrawHorzEmptySpace(e);
			DrawVertEmptySpace(e);
		}
		protected override void AfterPaint(ViewInfoPaintArgs e) {
			base.AfterPaint(e);			
			PaintBorder(e);
			PaintDisabled(e);
		}
		protected void PaintBorder(ViewInfoPaintArgs e) {
			BorderObjectInfoArgs borderArgs = new BorderObjectInfoArgs(e.GraphicsCache, Data.PaintAppearance.Empty, ClientRectangle); 
			BorderPainter borderPainter = BorderHelper.GetGridPainter(Data.BorderStyle, Data.ActiveLookAndFeel); 
			borderPainter.CalcObjectBounds(borderArgs);
			borderPainter.DrawObject(borderArgs);
		}
		private void PaintDisabled(ViewInfoPaintArgs e) {
			if(!IsEnabled)
				BackgroundPaintHelper.PaintDisabledControl(Data.ActiveLookAndFeel, e.GraphicsCache, e.ClientRectangle);
		}
		public int FrameBorderWidth { get { return System.Windows.Forms.SystemInformation.FrameBorderSize.Width / 2; } }
		void DrawHorzEmptySpace(ViewInfoPaintArgs e) {
			if(RowAreaFields.ControlBounds.Bottom >= Bounds.Bottom) return;
			Rectangle bounds = new Rectangle(Bounds.X, RowAreaFields.ControlBounds.Bottom, Bounds.Width, Bounds.Bottom - RowAreaFields.ControlBounds.Bottom);
			DrawEmptySpace(e, bounds);
		}
		void DrawVertEmptySpace(ViewInfoPaintArgs e) {
			if(ColumnAreaFields.ControlBounds.Right >= Bounds.Right) return;
			Rectangle bounds = new Rectangle(ColumnAreaFields.ControlBounds.Right, ColumnAreaFields.ControlBounds.Y, Bounds.Right - ColumnAreaFields.ControlBounds.Right, RowAreaFields.ControlBounds.Bottom);
			DrawEmptySpace(e, bounds);
		}
		protected void DrawEmptySpace(ViewInfoPaintArgs e, Rectangle bounds) {
			PivotCustomDrawAppearanceOwner appearanceOnwer = new PivotCustomDrawAppearanceOwner(Data.PaintAppearance.Empty);
			if(!CustomDrawEmptyArea(appearanceOnwer, e, bounds))
				appearanceOnwer.Appearance.FillRectangle(e.GraphicsCache, bounds);
		}
		protected virtual bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs e, Rectangle bounds) {
			return false;
		}
		void DrawHeadersAndFields(ViewInfoPaintArgs e) {
			Rectangle bounds = new Rectangle(FilterHeaders.Bounds.X, FilterHeaders.Bounds.Y, FilterHeaders.Bounds.Width, ColumnAreaFields.Bounds.Bottom - FilterHeaders.Bounds.Y);
			if(bounds.Width > ScrollableRectangle.Width)
				bounds.Width = ScrollableRectangle.Width;
			FillHeadersAndFields(e, bounds);
		}
		protected virtual void FillHeadersAndFields(ViewInfoPaintArgs e, Rectangle bounds) {
			ObjectPainter.DrawObject(e.GraphicsCache, Data.ActiveLookAndFeel.Painter.GroupPanel, new StyleObjectInfoArgs(e.GraphicsCache, bounds, Data.PaintAppearance.HeaderArea));
		}
		protected int HeadersHeight { get { return DefaultHeaderHeight + HeaderHeightOffset * 2; } }
		protected virtual int FilterHeadersHeight {
			get {
				return HeadersHeight;
			}
		}
		protected override void OnBeforeCalculating() {
			Bounds = ClientRectangle;
			Rectangle scrollableRectangle = ScrollableRectangle;
			CreateHeaders();
			RowHeaders.Bounds.Height = ColumnHeaders.Bounds.Height = DataHeaders.Bounds.Height = HeadersHeight;
			FilterHeaders.Bounds.Height = FilterHeadersHeight;
			for(int i = 0; i < HeaderCount; i++) {
				if(!CanShowHeader((PivotArea)i))
					GetHeader((PivotArea)i).Bounds.Height = 0;
			}
			this.rowAreaFields = CreateFieldsAreawViewInfo(false);
			this.columnAreaFields = CreateFieldsAreawViewInfo(true);
			this.cellsArea = CreateCellsViewInfo();
			if(IsPrefilterPanelVisible)
				this.prefilterPanel = CreatePrefilterPanelViewInfo();
			FilterHeaders.Bounds.Y = scrollableRectangle.Top;
			RowAreaFields.Bounds.X = FilterHeaders.Bounds.X = DataHeaders.Bounds.X = RowHeaders.Bounds.X = scrollableRectangle.Left;
			AdjustPrefilterBounds();
			AddChild(ColumnAreaFields);
			AddChild(RowAreaFields);
			AddChild(CellsArea);
			if(CanShowHeader(PivotArea.RowArea)) {
				AddChild(RowHeaders);
			}
			if(CanShowHeader(PivotArea.ColumnArea)) {
				AddChild(ColumnHeaders);
			}
			if(CanShowHeader(PivotArea.FilterArea)) {
				AddChild(FilterHeaders);
			}
			if(CanShowHeader(PivotArea.DataArea)) {
				AddChild(DataHeaders);
			}
			if(IsPrefilterPanelVisible)
				AddChild(PrefilterPanel);
			base.OnBeforeCalculating();
		}
		protected virtual void AdjustPrefilterBounds() {
			if(!IsPrefilterPanelVisible) return;
			PrefilterPanel.Bounds = PrefilterPanel.CalculateBounds(ScrollableRectangle);
		}
		protected virtual PivotCellsViewInfoBase CreateCellsViewInfo() {
			return new PivotCellsViewInfoBase(this);
		}
		protected virtual PivotFieldsAreaViewInfoBase CreateFieldsAreawViewInfo(bool isColumn) {
			return new PivotFieldsAreaViewInfoBase(this, isColumn);
		}
		protected virtual PivotPrefilterPanelViewInfoBase CreatePrefilterPanelViewInfo() {
			return new PivotPrefilterPanelViewInfoBase(this);
		}
		protected override void OnAfterChildrenCalculated() {
			CellsArea.Bounds.X = ColumnAreaFields.Bounds.X = ColumnHeaders.Bounds.X = RowAreaFields.Bounds.Right;
			SetFilterAndColumnHeadersWidth();
			RowHeaders.Bounds.Width = DataHeaders.Bounds.Width = RowAreaFields.Bounds.Width;
			FilterHeaders.CorrectHeadersHeight();
			StretchRowHeadersHeight();
			DataHeaders.Bounds.Y = ColumnHeaders.Bounds.Y = FilterHeaders.ControlBounds.Bottom;
			if(DataHeaders.Bounds.Height + RowHeaders.Bounds.Height > ColumnHeaders.Bounds.Height + ColumnAreaFields.Bounds.Height)
				ColumnHeaders.Bounds.Height = DataHeaders.Bounds.Height + RowHeaders.Bounds.Height - ColumnAreaFields.Bounds.Height;
			ColumnAreaFields.Bounds.Y = ColumnHeaders.ControlBounds.Bottom;
			CellsArea.Bounds.Y = RowAreaFields.Bounds.Y = RowAreaFieldsAndCellAreaTop;
			CellsArea.Bounds.Width = ColumnAreaFields.Bounds.Width;
			CellsArea.Bounds.Height = RowAreaFields.Bounds.Height;
			RowHeaders.Bounds.Y = ColumnAreaFields.Bounds.Bottom - RowHeaders.Bounds.Height;
			DataHeaders.CorrectHeadersWidth();
			SetLeftTopCoordCore(LeftTopCoord, false);
		}
		protected virtual void OnVisualItemsCleared(PivotVisualItemsBase items) {
			Clear();
		}
		void StretchRowHeadersHeight() {
			if(RowHeaders.Bounds.Height >= ColumnAreaFields.Bounds.Height)
				return;
			int prevRowHeadersHeight = RowHeaders.Bounds.Height;
			RowHeaders.Bounds.Height = ColumnAreaFields.Bounds.Height;
			int offset = RowHeaders.Bounds.Height - prevRowHeadersHeight;
			for(int i = 0; i < RowHeaders.ChildCount; i++)
				RowHeaders[i].Bounds.Y += offset;
		}
		protected virtual int RowAreaFieldsAndCellAreaTop { get { return ColumnAreaFields.ControlBounds.Bottom; } }
		void SetFilterAndColumnHeadersWidth() {
			ColumnHeaders.Bounds.Width = Math.Max(ColumnAreaFields.Bounds.Width, Bounds.Right - RowAreaFields.Bounds.Right);
			FilterHeaders.Bounds.Width = ColumnHeaders.Bounds.Right;
		}
		protected void SetLeftTopCoordCore(Point value) {
			SetLeftTopCoordCore(value, true);
		}
		protected void SetLeftTopCoordCore(Point value, bool invalidate) {
			Point oldLeftTopCoord = LeftTopCoord;
			this.leftTopCoord = CorrectLeftTopCoord(value);
			SetLeftTopCoordOffset();
			if(invalidate && !oldLeftTopCoord.Equals(LeftTopCoord)) {
				Rectangle bounds = Bounds;
				bounds.Y -= ColumnAreaFields.Bounds.Height;
				bounds.Height += ColumnAreaFields.Bounds.Height;
				bounds.X -= RowAreaFields.Bounds.Width;
				bounds.Width += RowAreaFields.Bounds.Width;
				Invalidate(bounds);
			}
		}
		protected virtual void SetLeftTopCoordOffset() {  }
		Point CorrectLeftTopCoord(Point value) {
			Point coord = value;
			Point maximumCoord = MaximumLeftTopCoordCore;
			if(coord.X > maximumCoord.X)
				coord.X = maximumCoord.X;
			if(coord.X < 0)
				coord.X = 0;
			if(coord.Y > maximumCoord.Y)
				coord.Y = maximumCoord.Y;
			if(coord.Y < 0)
				coord.Y = 0;
			return coord;
		}
		public virtual PivotGridAppearancesBase PrintAndPaintAppearance { get { return Data.PaintAppearance; } }
		protected virtual int CalcFieldHeight(bool isHeader, int lineCount) {
			return Data.DefaultFieldHeight;
		}		
		protected void MakeCellVisible(Point cell) {
			if(!CellsArea.IsCellValid(cell)) return;
			cell.X += ScrollRowFieldsLeftCoordCount;
			Point newLeftTopCoord = new Point(
				CalcCellVisibleIndex(cell.X, leftTopCoord.X, CellsArea.GetVisibleColumnCount(ScrollableSize.Width)),
				CalcCellVisibleIndex(cell.Y, leftTopCoord.Y, CellsArea.GetVisibleRowCount(ScrollableSize.Height)));
			if(LeftTopCoord != newLeftTopCoord) {
				LeftTopCoord = newLeftTopCoord;
			}
		}
		int CalcCellVisibleIndex(int newLeftTop, int curLeftTop, int visibleCount) {
			int result = curLeftTop;
			if(newLeftTop < curLeftTop)
				result = newLeftTop;
			if(visibleCount == 0) {
				visibleCount = 1;
			}
			if(newLeftTop >= curLeftTop + visibleCount)
				result = newLeftTop - visibleCount + 1;
			return result;
		}
		public PivotFieldsAreaViewInfoBase GetFieldsArea(bool isColumn) {
			return isColumn ? ColumnAreaFields : RowAreaFields;
		}
		#region dumps
		public virtual PivotGridFieldBase GetSizingField(int x, int y) { return null; }
		public virtual PivotGridViewInfoState State { get { return PivotGridViewInfoState.Normal; } }
		public virtual ScrollArgs HScrollBarInfo { get { return null; } }
		public virtual ScrollArgs VScrollBarInfo { get { return null; } }
		#endregion
	}
	public class PivotViewInfo : BaseViewInfo {
		const int ReloadDataMenuID = -1000;
		const int ShowHideCustomizationFieldsMenuID = -1001;
		const int ShowHidePrefilterMenuID = -1002;
		PivotGridViewInfoBase viewInfo;
		public PivotViewInfo(PivotGridViewInfoBase viewInfo) {
			this.viewInfo = viewInfo;
		}
		public PivotGridViewInfoBase ViewInfo { get { return viewInfo; } }
		public PivotGridViewInfoData Data { get { return viewInfo.Data; } }
		protected virtual Control GetControlOwner() { return null; }
		protected virtual IDXMenuManager GetMenuManager() { return null; }
		protected bool IsEnabled { get { return ViewInfo.IsEnabled; } }
		public virtual PivotGridFieldBase GetFieldAt(Point pt) { return null; }
		public virtual bool AcceptDragDrop { get { return false; } }
		public virtual Rectangle GetDragDrawRectangle(PivotGridFieldBase field, Point pt) { return Rectangle.Empty; }
		public virtual int GetNewFieldPosition(PivotGridFieldBase field, Point pt, out PivotArea area) {
			area = PivotArea.FilterArea;
			return -1;
		}
		public virtual void InvalidateHighLight() { }
		protected string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		#region Menu
		[ThreadStatic]
		static ImageList menuImages = null;
		protected static ImageList MenuImages {
			get {
				if(menuImages == null) menuImages = DevExpress.Utils.Controls.ImageHelper.CreateImageListFromResources("DevExpress.XtraPivotGrid.Images.popupmenuicons.bmp", typeof(PivotViewInfo).Assembly, new Size(16, 16));
				return menuImages;
			}
		}
		protected Point menuLocation = Point.Empty;
		protected DXPopupMenu menu = null;
		protected void ShowPopupMenu(MouseEventArgs e) {
			this.menu = new DXPopupMenu();
			menuLocation = new Point(e.X, e.Y);
			CreatePopupMenuItems(this.menu);
			PivotGridMenuEventArgsBase menuEvent = CreateMenuEventArgs();
			if(!RaiseShowingMenu(menuEvent) && (menuEvent.Menu != null) && (menuEvent.Menu.Items.Count > 0)) {
				ShowMenuCore(menuEvent);
			}
			this.menu = null;
		}
		protected void ShowMenuCore(PivotGridMenuEventArgsBase menuEvent) {
			MenuManagerHelper.ShowMenu(menuEvent.Menu, ((PivotGridViewInfoData)Data).ActiveLookAndFeel, GetMenuManager(), GetControlOwner(), menuLocation);
		}
		protected virtual PivotGridMenuType MenuType { get { return PivotGridMenuType.Header; } }
		protected virtual PivotGridFieldBase MenuField { get { return null; } }
		protected virtual PivotArea MenuArea { get { return MenuField != null ? MenuField.Area : PivotArea.DataArea; } }
		protected virtual void CreatePopupMenuItems(DXPopupMenu menu) { }
		protected virtual void OnMenuItemClick(DXMenuItem menuItem) {
			switch((int)menuItem.Tag) {
				case ReloadDataMenuID:
					Data.ReloadData();
					break;
				case ShowHideCustomizationFieldsMenuID:
					Data.ChangeFieldsCustomizationVisible();
					break;
				case ShowHidePrefilterMenuID:
					Data.ChangePrefilterVisible();
					break;
			}
		}
		protected void AddPopupMenuRefresh() {
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuRefreshData), ReloadDataMenuID, 0));
		}
		protected void AddPopupMenuFieldCustomization() {
			menu.Items.Add(CreateMenuItem(GetLocalizedString(Data.IsFieldCustomizationShowing ? 
												PivotGridStringId.PopupMenuHideFieldList : 
												PivotGridStringId.PopupMenuShowFieldList), 
											ShowHideCustomizationFieldsMenuID, 1));
			SetBeginGrouptoLastMenuItem();
		}
		protected void AddPopupMenuPrefilter() {
			if(!Data.OptionsCustomization.AllowPrefilter) return;
			menu.Items.Add(CreateMenuItem(GetLocalizedString(Data.IsPrefilterFormShowing ? 
											   PivotGridStringId.PopupMenuHidePrefilter : 
											   PivotGridStringId.PopupMenuShowPrefilter), 
										  ShowHidePrefilterMenuID, 2));
		}
		protected void SetBeginGrouptoLastMenuItem() {
			if(menu.Items.Count == 1) return;
			menu.Items[menu.Items.Count - 1].BeginGroup = true;
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag) {
			return CreateMenuItem(caption, tag, true);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, bool enabled) {
			return CreateMenuItem(caption, tag, enabled, -1, false);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, int imageIndex) {
			return CreateMenuItem(caption, tag, true, imageIndex, false);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, bool enabled, int imageIndex, bool beginGroup) {
			DXMenuItem item = new DXMenuItem(caption, new EventHandler(OnMenu_Click));
			item.Tag = tag;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			if(imageIndex >= 0) {
				item.Image = MenuImages.Images[imageIndex];
			}
			return item;
		}
		protected DXMenuCheckItem CreateMenuCheckItem(string caption, bool check, object tag, bool beginGroup) {
			DXMenuCheckItem item = new DXMenuCheckItem(caption, check);
			item.CheckedChanged += new EventHandler(OnMenu_Click);
			item.Tag = tag;
			item.BeginGroup = beginGroup;
			return item;
		}
		protected bool RaiseShowingMenu(PivotGridMenuEventArgsBase e) {
			Data.OnPopupShowMenu(e);
			menuLocation = e.Point;
			return !e.Allow;
		}
		protected bool RaiseMenuClick(DXMenuItem menuItem) {
			PivotGridMenuItemClickEventArgsBase e = CreateMenuItemClickEventArgs(menuItem);
			Data.OnPopupMenuItemClick(e);
			return !e.Allow;
		}
		void OnMenu_Click(object sender, EventArgs e) {
			if(RaiseMenuClick((DXMenuItem)sender)) return;
			OnMenuItemClick((DXMenuItem)sender);
		}
		protected virtual PivotGridMenuEventArgsBase CreateMenuEventArgs() {
			return new PivotGridMenuEventArgsBase(ViewInfo, MenuType, menu, MenuField, MenuArea, menuLocation);
		}
		protected virtual PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return new PivotGridMenuItemClickEventArgsBase(ViewInfo, MenuType, this.menu, MenuField, MenuArea, menuLocation, menuItem);
		}
		#endregion
		public virtual PivotGridHitInfo CalcHitInfo(Point hitPoint) { return new PivotGridHitInfo(hitPoint); }
		public virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) { return null; }
	}
	class PointsComparer : IComparer<Point> {
		#region IComparer<Point> Members
		public int Compare(Point a, Point b) {
			if(a.Y < b.Y) return -1;
			if(a.Y > b.Y) return 1;
			if(a.X == b.X) return 0;
			if(a.X < b.X) return -1;
			return 1;
		}
		#endregion
	}		
}
