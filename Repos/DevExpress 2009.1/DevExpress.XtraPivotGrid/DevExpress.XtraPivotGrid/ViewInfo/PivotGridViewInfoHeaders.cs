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
using DevExpress.XtraPivotGrid.FilterDropDown;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Printing;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotHeadersViewInfo : PivotHeadersViewInfoBase {
		public PivotHeadersViewInfo(PivotGridViewInfo viewInfo, PivotArea area)
			: base(viewInfo, area) { }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override IDXMenuManager GetMenuManager() {
			return Data.MenuManager;
		}
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
			return new PivotGridMenuEventArgs((PivotGridViewInfo)ViewInfo, MenuType, menu, (PivotGridField)MenuField, MenuArea, menuLocation);
		}
		protected override PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return new PivotGridMenuItemClickEventArgs((PivotGridViewInfo)ViewInfo, MenuType, this.menu, (PivotGridField)MenuField, MenuArea, menuLocation, menuItem);
		}
		public new PivotHeaderViewInfo this[int index] { get { return (PivotHeaderViewInfo)base[index]; } }
		protected override PivotHeaderViewInfoBase CreateHeaderViewInfo(PivotGridField field) {
			return new PivotHeaderViewInfo(ViewInfo, field);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(!Data.OptionsHint.ShowHeaderHints) return null;
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].ControlBounds.Contains(pt)) {
					return this[i].GetToolTipObjectInfo(pt);
				}
			}
			return null;
		}
		public override void InvalidateHighLight() {
			if(Data.ControlOwner == null) return;
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			try {
				this.Paint(Data.ControlOwner, new PaintEventArgs(graphics, ControlBounds));
			} finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableHeaderAreaMenu) {
				ShowPopupMenu(e);
			}
			return null;
		}
		protected override PivotGridMenuType MenuType { get { return PivotGridMenuType.HeaderArea; } }
		protected override PivotArea MenuArea { get { return Area; } }
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			AddPopupMenuRefresh();
			AddPopupMenuFieldCustomization();
			AddPopupMenuPrefilter();
		}
		protected override Rectangle GetDrawRectangle() {
			Rectangle drawBounds = PaintBounds;
			if(drawBounds.Right > ViewInfo.ScrollableRectangle.Right)
				drawBounds.Width = ViewInfo.ScrollableRectangle.Right - drawBounds.X;
			return drawBounds;
		}
	}
	public class PivotHeadersViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner {
		PivotArea area;
		AppearanceObject appearance;
		public PivotHeadersViewInfoBase(PivotGridViewInfoBase viewInfo, PivotArea area)
			: base(viewInfo) {
			this.area = area;
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] {DefaultAppearance, Data.PaintAppearance.HeaderArea});
		}
		public PivotArea Area { get { return area; } }
		public new PivotHeaderViewInfoBase this[int index] { get { return (PivotHeaderViewInfoBase)base[index]; } }
		public override PivotGridFieldBase GetFieldAt(Point pt) {
			if(!ControlBounds.Contains(pt)) return null;
			for(int i = 0; i < ChildCount; i ++) {
				if(this[i].ControlBounds.Contains(pt))
					return this[i].Field;
			}
			return null;
		}
		public override bool AcceptDragDrop { get { return true; } }
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotHeaderViewInfoBase viewInfo = GetViewInfoAtPoint(hitPoint, false) as PivotHeaderViewInfoBase;
			return viewInfo != null ? viewInfo.CalcHitInfo(hitPoint) : new PivotGridHitInfo(this, null, PivotGridHeaderHitTest.None,  hitPoint);
		}
		public override Rectangle GetDragDrawRectangle(PivotGridFieldBase field, Point pt) { 
			if(!ControlBounds.Contains(pt)) return Rectangle.Empty;
			bool isBefore;
			PivotHeaderViewInfoBase header = GetDragHeaderInfo(pt, out isBefore);
			if(header != null) {
				if(header.Field == field)
					return Rectangle.Empty;
				return GetDragDrawRectangle(header, isBefore);
			}
			Rectangle bounds = ControlBounds;
			bounds.Inflate(0, -HeaderHeightOffset);
			bounds.X += HeaderWidthOffset;
			bounds.Width = Data.DefaultFieldWidth / 2;
			return bounds;
		}
		public override int GetNewFieldPosition(PivotGridFieldBase field, Point pt, out PivotArea area) {
			area = Area;
			if(!ControlBounds.Contains(pt)) return -1;
			bool isBefore;
			PivotHeaderViewInfoBase header = GetDragHeaderInfo(pt, out isBefore);
			if(header == null) return 0;
			if(field == header.Field) return -1;
			int oldAreaIndexWithData = 0, targetAreaIndexWithData = 0;
			GetFieldIndexes(field, header.Field, out oldAreaIndexWithData, out targetAreaIndexWithData);
			targetAreaIndexWithData = isBefore ? targetAreaIndexWithData : targetAreaIndexWithData + 1;
			if(oldAreaIndexWithData == targetAreaIndexWithData && field.Area == Area)
				return -1;
			return targetAreaIndexWithData;
		}
		void GetFieldIndexes(PivotGridFieldBase field, PivotGridFieldBase tagetField, out int oldFullIndex, out int newFullIndex) {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(area, true);
			oldFullIndex = fields.IndexOf(field);
			fields.Remove(field);
			newFullIndex = fields.IndexOf(tagetField);
		}
		public void CorrectHeadersHeight() {
			if(Area != PivotArea.FilterArea) return;
			int headersWidth = Bounds.Width < ViewInfo.Bounds.Width ? Bounds.Width : ViewInfo.Bounds.Width;
			if(ChildCount < 2 || this[ChildCount - 1].Bounds.Right < headersWidth) return;
			int right = HeaderWidthOffset;
			int top = HeaderHeightOffset;
			int headerCount = 0;
			int index = 0;
			while(index < ChildCount) {
				int width = GetFieldGroupWidth(index);
				if(headerCount > 0 && (right + width) >= headersWidth) {
					Bounds.Height += ViewInfo.DefaultHeaderHeight + HeaderHeightOffset;
					top += ViewInfo.DefaultHeaderHeight + HeaderHeightOffset;
					right = HeaderWidthOffset;
					headerCount = 0;
				}
				SetFieldGroupLocation(index, new Point(right, top));
				right += GetFieldGroupWidth(index);
				index += GetFieldGroupCount(index);
				headerCount += GetFieldGroupCount(index);
			}
		}
		int GetFieldGroupCount(int index) {
			if(index >= ChildCount) return 0;
			return this[index].Field.Group != null ? this[index].Field.Group.VisibleCount : 1;
		}
		int GetFieldGroupWidth(int index) {
			int visibleCount = GetFieldGroupCount(index);
			int width = 0;
			for(int i = 0; i < visibleCount; i ++)
				width += this[index + i].Bounds.Width + HeaderWidthOffset;
			return width;
		}
		void SetFieldGroupLocation(int index, Point location) {
			int visibleCount = GetFieldGroupCount(index);
			for(int i = 0; i < visibleCount; i ++) {
				this[index + i].Bounds.Location = location;
				location.X += this[index + i].Bounds.Width + HeaderWidthOffset;
			}
		}
		public void CorrectHeadersWidth() {
			if(Area != PivotArea.DataArea) return;
			if(ChildCount == 0 || this[ChildCount - 1].Bounds.Right < Bounds.Width) return;
			int averageWidth = (Bounds.Width - (ChildCount + 1) * HeaderWidthOffset) / ChildCount;
			int noNeedCorrectionWidth = 0;
			int noNeedCorrectionCount = 0;
			for(int i = 0; i < ChildCount; i ++) {
				if(this[i].Bounds.Width < averageWidth) {
					noNeedCorrectionWidth += this[i].Bounds.Width;
					noNeedCorrectionCount ++;
				}
			}
			averageWidth = (Bounds.Width - (ChildCount + 1) * HeaderWidthOffset - noNeedCorrectionWidth) / (ChildCount - noNeedCorrectionCount);
			for(int i = 0; i < ChildCount; i ++) {
				if(this[i].Bounds.Width > averageWidth)
					this[i].Bounds.Width = averageWidth;
			}
			UpdateChildXLocation();
		}
		public int GetBestWidth(PivotGridFieldBase field) {
			return field.AreaIndex < ChildCount ? this[field.AreaIndex].BestWidth : 0;
		}
		public string CustomizeText {
			get { return PivotGridLocalizer.GetHeadersAreaText((int)Area); }
		}		
		protected int HeaderWidthOffset { get { return ViewInfo.HeaderWidthOffset; } }
		protected int HeaderHeightOffset { get { return ViewInfo.HeaderHeightOffset; } }
		protected int FirstLastHeaderWidthOffset { get { return ViewInfo.FirstLastHeaderWidthOffset; } }
		PivotHeaderViewInfoBase GetDragHeaderInfo(Point pt, out bool isBefore) {
			isBefore = false;
			if(ChildCount == 0) return null;
			PivotHeaderViewInfoBase viewInfo = null;
			for(int i = 0; i < ChildCount; i ++) {
				Rectangle rect = this[i].ControlBounds;
				if(pt.Y >= rect.Top && pt.Y <= rect.Bottom) {
					viewInfo = this[i];
					if(pt.X < rect.Right) {
						if(pt.X - rect.Left < rect.Right - pt.X) 
							isBefore = true;
						return this[i];
					} 
				}
			}
			return viewInfo != null ? viewInfo : this[ChildCount - 1];
		}
		Rectangle GetDragDrawRectangle(PivotHeaderViewInfoBase headerViewInfo, bool isBefore) {
			Rectangle bounds = headerViewInfo.ControlBounds;
			if(isBefore)
				return new Rectangle(bounds.Left - HeaderWidthOffset, bounds.Y, bounds.Width / 2, bounds.Height);
			else return new Rectangle(bounds.Left + bounds.Width / 2 + HeaderWidthOffset, bounds.Y, bounds.Width / 2, bounds.Height);
		}
		protected virtual PivotGridField[] GetPivotFields() {
			List<PivotGridFieldBase> baseFields = Data.GetFieldsByArea(Area, true);
			List<PivotGridField> fields = new List<PivotGridField>();
			for(int i = 0; i < baseFields.Count; i++) {
				if(!ViewInfo.Data.OptionsPrint.PrintUnusedFilterFields && Area == PivotArea.FilterArea && baseFields[i].FilterValues.Count == 0 && ViewInfo is PivotGridPrintViewInfo) continue;
				fields.Add(baseFields[i] as PivotGridField);
			}
			return fields.ToArray();
		}
		protected override void OnBeforeCalculating() {
			PivotGridField[] fields = GetPivotFields();
			for(int i = 0; i < fields.Length; i ++) {
				PivotHeaderViewInfoBase header = CreateHeaderViewInfo(fields[i]);
				header.Bounds.Y = HeaderHeightOffset; 
				int width = fields[i].Width - HeaderWidthOffset;
				if(i == 0 || i == fields.Length - 1)
					width -= FirstLastHeaderWidthOffset;
				header.Bounds.Size = new Size(width, ViewInfo.DefaultHeaderHeight);
				AddChild(header);
				header.UpdateCaption();
			}
		}
		protected virtual PivotHeaderViewInfoBase CreateHeaderViewInfo(PivotGridField field) {
			return new PivotHeaderViewInfoBase(ViewInfo, field);
		}
		protected override void OnAfterCalculated() {
			UpdateChildXLocation();
		}
		void UpdateChildXLocation() {
			int x = Area == PivotArea.ColumnArea ? 0 : HeaderWidthOffset; 
			for(int i = 0; i < ChildCount; i ++) {
				this[i].Bounds.X = x;
				x += this[i].Bounds.Width + HeaderWidthOffset;
			}
		}
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		protected AppearanceObject DefaultAppearance {
			get {
				if(Area == PivotArea.ColumnArea)
					return Data.PaintAppearance.ColumnHeaderArea;
				if(Area == PivotArea.RowArea)
					return Data.PaintAppearance.RowHeaderArea;
				if(Area == PivotArea.DataArea)
					return Data.PaintAppearance.DataHeaderArea;
				return Data.PaintAppearance.FilterHeaderArea;
			}
		}
		protected bool IsNeedFillBackground {
			get {
				AppearanceObject areaAppearance = Data.Appearance.HeaderArea;
				AppearanceObject defaultAppearance = DefaultAppearance;
				return areaAppearance.BackColor != defaultAppearance.BackColor || 
					areaAppearance.BackColor2 != defaultAppearance.BackColor2 ||
					areaAppearance.GradientMode != defaultAppearance.GradientMode;
			}
		}		
		protected virtual Rectangle GetDrawRectangle() {
			Rectangle drawBounds = PaintBounds;
			return drawBounds;
		}
		protected virtual Rectangle GetTextRectangle() {
			Rectangle bounds = ControlBounds;
			bounds.Inflate(-HeaderWidthOffset - 1, 0);
			return bounds;
		}
		protected virtual void DrawSeparator(ViewInfoPaintArgs e) {
			if(Area != PivotArea.FilterArea || !Data.OptionsView.ShowFilterSeparatorBar) return;
			Rectangle lineBounds = GetDrawRectangle();
			lineBounds.Y = lineBounds.Bottom - 1;		
			lineBounds.Height = 1;
			lineBounds.Width = lineBounds.Right - lineBounds.X - 2;
			Data.PaintAppearance.FilterSeparator.FillRectangle(e.GraphicsCache, lineBounds);
		}
		protected virtual void DrawGroupLines(ViewInfoPaintArgs e) {
			for(int i = 0; i < ChildCount - 1; i ++) {
				if(this[i].Field.IsNextVisibleFieldInSameGroup) {
					DrawGroupLine(e, this[i]);
				}
			}
		}
		protected virtual void DrawGroupLine(ViewInfoPaintArgs e, PivotHeaderViewInfoBase headerViewInfo) {
			int x = headerViewInfo.ControlBounds.Right;
			int y = headerViewInfo.ControlBounds.Top + headerViewInfo.Bounds.Height / 2;
			Data.PaintAppearance.HeaderGroupLine.FillRectangle(e.GraphicsCache, new Rectangle(x, y, HeaderWidthOffset, 1));
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			if(!ViewInfo.DrawHeaderArea(this, e, GetDrawRectangle())) {
				if(IsNeedFillBackground)
					Appearance.FillRectangle(e.GraphicsCache, GetDrawRectangle());
				if(ChildCount == 0) 
					DrawCustomizeText(e);
				DrawSeparator(e);
				DrawGroupLines(e);
			}
		}
		protected override void AfterPaint(ViewInfoPaintArgs e) {
			if(ViewInfo.HighLightedArea == this) {
				Color highLightColor = Color.FromArgb(75, SystemColors.Highlight);
				Brush brush = new SolidBrush(highLightColor);
				e.Graphics.FillRectangle(brush, GetDrawRectangle());
				brush.Dispose();
			}
		}
		protected virtual void DrawCustomizeText(ViewInfoPaintArgs e) {
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near; 
			format.LineAlignment = StringAlignment.Center;
			Appearance.DrawString(e.GraphicsCache, CustomizeText, GetTextRectangle(), format);
		}
	}
	public class PivotHeaderViewInfo : PivotHeaderViewInfoBase, IPivotGridDropDownFilterEditOwner {
		const int MoveToBeginningMenuID = 100;
		const int MoveToEndMenuID = 101;
		const int MoveRightMenuID = 102;
		const int MoveLeftMenuID = 103;
		const int HideFieldMenuID = 104;
		bool hotTrackFilterButton;
		bool isFilterDown;
		Point mouseDownLocation;
		protected override bool IsFilterDown { get { return isFilterDown; } }
		public PivotHeaderViewInfo(PivotGridViewInfo viewInfo, PivotGridField field)
			: base(viewInfo, field) {
			this.hotTrackFilterButton = false;
			this.isFilterDown = false;
			this.mouseDownLocation = Point.Empty;
		}
		public new PivotHeadersViewInfo HeadersViewInfo { get { return (PivotHeadersViewInfo)Parent; } }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }		
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override IDXMenuManager GetMenuManager() {
			return Data.MenuManager;
		}
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
			return new PivotGridMenuEventArgs((PivotGridViewInfo)ViewInfo, MenuType, menu, (PivotGridField)MenuField, MenuArea, menuLocation);
		}
		protected virtual PivotGridMenuEventArgs CreateSummariesMenuEventArgs() {
			return new PivotGridMenuEventArgs((PivotGridViewInfo)ViewInfo, PivotGridMenuType.HeaderSummaries,
							this.menu, (PivotGridField)MenuField, MenuArea, this.menuLocation);
		}
		protected override PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return new PivotGridMenuItemClickEventArgs((PivotGridViewInfo)ViewInfo, MenuType, this.menu, (PivotGridField)MenuField, MenuArea, menuLocation, menuItem);
		}
		public bool IsDragging { get { return ViewInfo.IsDragging; } }
		protected override bool HotTrackFilterButton {
			get { return hotTrackFilterButton; }
			set {
				if(HotTrackFilterButton == value || FilterInfo == null) return;
				this.hotTrackFilterButton = value;
				Data.Invalidate(ControlBounds);
			}
		}
		protected PivotHeaderViewInfo DraggingHeader {
			get {
				if(Field.Group == null || HeadersViewInfo == null) return this;
				if(Field.Group[0] == Field) return this;
				return HeadersViewInfo[Field.Group.AreaIndex];
			}
		}
		protected override void MouseUpCore(MouseEventArgs e) {
			this.mouseDownLocation = Point.Empty;
			if(e.Button != MouseButtons.Left || Field.IsDesignTime) return;
			if(isFilterDown) {
				if(IsMouseOnFilterButton(new Point(e.X, e.Y))) {
					Rectangle bounds = ControlBounds;
					if(!FilterInfo.Bounds.IsEmpty && IsFilterSmartTagStyle) bounds = FilterInfo.Bounds;
					PivotGridDropDownFilterEdit dropDown = new PivotGridDropDownFilterEdit(this, Data, Field, bounds);
					dropDown.Show();
				} else isFilterDown = false;
			}			
			if(CanSort && !isFilterDown && !IsDragging)
				Field.SortOrder = Field.SortOrder == PivotSortOrder.Ascending ? PivotSortOrder.Descending : PivotSortOrder.Ascending;
			if(AllowRunTimeSummaryChange && !isFilterDown && !IsDragging) {
				ShowSummariesMenu();
			}
			Invalidate();
		}
		protected void ShowSummariesMenu() {
			this.menu = new DXPopupMenu();
			this.menuLocation = new Point(Parent.Bounds.Left + Bounds.Left, Parent.Bounds.Top + Bounds.Bottom + 1);
			CreateSummaryTypesMenuItems(this.menu);
			PivotGridMenuEventArgs menuEvent = CreateSummariesMenuEventArgs();
			if(!RaiseShowingMenu(menuEvent) && (menuEvent.Menu != null) && (menuEvent.Menu.Items.Count > 0)) {
				ShowMenuCore(menuEvent);
			}
			this.menu = null;
		}
		bool IsMouseOnFilterButton(Point pt) {
			return FilterInfo != null && FilterInfo.Bounds.Contains(pt);
		}
		protected override void MouseMoveCore(MouseEventArgs e) {
			if(IsDragging) return;
			if(!this.mouseDownLocation.IsEmpty && CanDrag && !isFilterDown) {
				Size dragSize = System.Windows.Forms.SystemInformation.DragSize;
				if(Math.Abs(this.mouseDownLocation.X - e.X) >= dragSize.Width
					|| Math.Abs(this.mouseDownLocation.Y - e.Y) >= dragSize.Height) {
					ViewInfo.StartDragging(DraggingHeader);
				}
			}
			HotTrackFilterButton = FilterInfo != null ? FilterInfo.Bounds.Contains(new Point(e.X, e.Y)) : false;
		}
		protected override void MouseEnterCore() {
			Invalidate();
		}
		protected override void MouseLeaveCore() {
			this.hotTrackFilterButton = false;
			Invalidate();
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableHeaderMenu) {
				ShowPopupMenu(e);
			}
			if(e.Button != MouseButtons.Left) return null;
			if(OpenCloseButtonInfo != null && OpenCloseButtonInfo.Bounds.Contains(e.X, e.Y)) {
				Field.ExpandedInFieldsGroup = !Field.ExpandedInFieldsGroup;
				return null;
			}
			this.mouseDownLocation = new Point(e.X, e.Y);
			if(Field.IsDesignTime) {
				Field.SelectedAtDesignTime = true;
				Data.Invalidate();
				return this;
			}
			if(IsMouseOnFilterButton(new Point(e.X, e.Y))) {
				isFilterDown = true;
			}
			Invalidate();
			return this;
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotGridHeaderHitTest hitTest = PivotGridHeaderHitTest.None;
			if(IsMouseOnFilterButton(hitPoint))
				hitTest = PivotGridHeaderHitTest.Filter;
			return new PivotGridHitInfo(HeadersViewInfo, Field, hitTest, hitPoint);
		}	
		void IPivotGridDropDownFilterEditOwner.CloseFilter() {
			this.isFilterDown = false;
			Invalidate();
		}
		public override bool IsFilterSmartTagStyle { get { return Data.OptionsView.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag; } }
		protected override void OnMenuItemClick(DXMenuItem menuItem) {
			base.OnMenuItemClick(menuItem);
			switch((int)menuItem.Tag) {
				case HideFieldMenuID:
					Field.Visible = false;
					break;
				case MoveToBeginningMenuID:
					Data.SetFieldAreaPosition(Field, Field.Area, 0);
					break;
				case MoveToEndMenuID:
					if(Field.AreaIndex > 0) {
						Data.SetFieldAreaPosition(Field, Field.Area, Field.AreaIndex - 1);
					}
					break;
				case MoveRightMenuID:
					Data.SetFieldAreaPosition(Field, Field.Area, Field.AreaIndex + 1);
					break;
				case MoveLeftMenuID:
					Data.SetFieldAreaPosition(Field, Field.Area, int.MaxValue);
					break;
			}
		}
		protected override void OnAfterCalculated() {
			if(Field.Area != PivotArea.RowArea) {
				Bounds.Width = BestWidth;
			}
		}
		protected override ViewInfoPaintArgs CreateViewInfoPaintArgs(GraphicsCache cache) {
			return new ViewInfoPaintArgs(Data.ControlOwner, new PaintEventArgs(cache.Graphics, Rectangle.Empty));
		}
		protected override void Invalidate(Rectangle bounds) {
			Data.Invalidate(bounds);
		}
		protected void CreateSummaryTypesMenuItems(DXPopupMenu menu) {
			foreach(PivotSummaryType type in Enum.GetValues(typeof(PivotSummaryType))) {
				DXMenuCheckItem item = new DXMenuCheckItem(type.ToString(), type == Field.SummaryType);
				item.Tag = type;
				item.CheckedChanged += new EventHandler(OnSummaryTypeMenu_Click);
				menu.Items.Add(item);
			}
		}
		void OnSummaryTypeMenu_Click(object sender, EventArgs e) {
			if(RaiseMenuClick((DXMenuItem)sender)) return;
			Field.SummaryType = (PivotSummaryType)((DXMenuItem)sender).Tag;
		}
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			AddPopupMenuRefresh();
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuHideField), HideFieldMenuID, CanHide));
			menu.Items[menu.Items.Count - 1].BeginGroup = true;
			if(Field.Group == null) {
				int fieldCount = Data.GetFieldCountByArea(Field.Area);
				if(Data.DataField.Visible && Data.DataField.Area == Field.Area)
					fieldCount++;
				DXSubMenuItem subMenuItem = new DXSubMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuFieldOrder));
				subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoBeginning), MoveToBeginningMenuID, Field.AreaIndex > 0));
				subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoLeft), MoveToEndMenuID, Field.AreaIndex > 0));
				subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoRight), MoveRightMenuID, Field.AreaIndex + 1 < fieldCount));
				subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoEnd), MoveLeftMenuID, Field.AreaIndex + 1 < fieldCount));
				menu.Items.Add(subMenuItem);
			}
			AddPopupMenuFieldCustomization();
			AddPopupMenuPrefilter();
		}
	}
	public class PivotHeaderViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner {
		const int CollapsedButtonOffset = 2;
		PivotGridField field;
		HeaderObjectInfoArgs info;
		SortedShapeObjectInfoArgs sortedInfo;
		GridFilterButtonInfoArgs filterInfo;
		protected GridFilterButtonInfoArgs FilterInfo { get { return filterInfo; } }
		AppearanceObject appearance;		
		OpenCloseButtonInfoArgs openCloseButtonInfo;
		protected OpenCloseButtonInfoArgs OpenCloseButtonInfo { get { return openCloseButtonInfo; } }
		protected virtual bool IsFilterDown { get { return false; } }
		public PivotHeaderViewInfoBase(PivotGridViewInfoBase viewInfo, PivotGridField field)
			: base(viewInfo) {
			this.field = field;
			this.filterInfo = null;
			this.sortedInfo = null;
			this.info = new HeaderObjectInfoArgs();
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] { Field.Appearance.Header, ViewInfo.PrintAndPaintAppearance.FieldHeader });
			InfoArgs.SetAppearance(Appearance);
			CreateSort();
			CreateFilter();
			InfoArgs.Caption = Caption;
			InfoArgs.IsTopMost = true;
			InfoArgs.HeaderPosition = HeaderPositionKind.Special;
			CreateHeaderImage();
			CreateOpenCloseButton();
		}
		protected void CreateSort() {
			if(!CanSort || !AddSort) return;
			this.sortedInfo = new SortedShapeObjectInfoArgs();
			this.sortedInfo.Ascending = Field.SortOrder == PivotSortOrder.Ascending;
			InfoArgs.InnerElements.Add(Data.ActiveLookAndFeel.Painter.SortedShape, this.sortedInfo);
		}
		protected void CreateFilter() {
			if(!CanFilter || !AddFilter) return;
			this.filterInfo = new GridFilterButtonInfoArgs();
			this.filterInfo.Filtered = Field.FilterValues.HasFilter;
			if(IsFilterSmartTagStyle) {
				DrawElementInfo di = new DrawElementInfo(FilterButtonHelper.GetSmartPainter(Data.ActiveLookAndFeel), this.filterInfo, StringAlignment.Far);
				di.ElementInterval = 0;
				di.RequireTotalBounds = true;
				InfoArgs.InnerElements.Add(di);
			} else {
				InfoArgs.InnerElements.Add(FilterButtonHelper.GetPainter(Data.ActiveLookAndFeel), this.filterInfo);
			}
		}
		protected void CreateHeaderImage() {
			if(AddImage && Field.ImageIndex > -1 && Data.HeaderImages != null) {
				InfoArgs.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(Data.HeaderImages, Field.ImageIndex, null), StringAlignment.Near));
			}
		}
		protected void CreateOpenCloseButton() {
			if(!AddCollapseButton) return;
			this.openCloseButtonInfo = ShowCollapsedButton ? new OpenCloseButtonInfoArgs(null) : null;
			if(this.openCloseButtonInfo != null) {
				this.openCloseButtonInfo.Opened = Field.ExpandedInFieldsGroup;
				InfoArgs.InnerElements.Add(new DrawElementInfo(Data.ActiveLookAndFeel.Painter.OpenCloseButton, this.openCloseButtonInfo, StringAlignment.Near));
			}
		}
		public PivotGridField Field { get { return field; } }
		public HeaderObjectInfoArgs InfoArgs { get { return info; } }
		public bool CanSort { 
			get { 
				if(!ViewInfo.CanHeaderSort) return false;
				return Field.CanSort;
			} 
		}
		public bool CanFilter { 
			get { 
				if(!ViewInfo.CanHeaderFilter) return false;
				return Field.CanFilter;
			} 
		}
		public bool CanDrag { get { return Field.CanDrag; } }
		public bool CanHide { get { return Field.CanHide; } }
		public bool ShowCollapsedButton {
			get {
				return Field.Group != null && Field.Group.CanExpandField(field);
			}
		}
		protected bool AllowRunTimeSummaryChange {
			get {
				if(Field.Area != PivotArea.DataArea) return false;
				return Field.Options.AllowRunTimeSummaryChange;
			}
		}
		protected bool ShowSummaryTypeName { get { return Field.Options.ShowSummaryTypeName; } }
		protected virtual bool HotTrackFilterButton { get { return false; } set { ; } }
		public virtual bool IsFilterSmartTagStyle { get { return false; } }
		public int BestWidth {
			get {
				int bestWidth = 0;
				Graphics graphics =	GraphicsInfo.Default.AddGraphics(null);
				GraphicsCache graphicsCache = new GraphicsCache(graphics);
				try {
					bestWidth = GetMinBounds(graphicsCache).Width;
				}
				finally {
					graphicsCache.Dispose();
					GraphicsInfo.Default.ReleaseGraphics();
				}
				return bestWidth + ViewInfo.HeaderWidthOffset;
			}
		}
		protected Rectangle GetMinBounds(GraphicsCache graphicsCache) {
			InfoArgs.Cache = graphicsCache;
			InfoArgs.Bounds = new Rectangle(0, 0, Bounds.Height, int.MaxValue);
			InfoArgs.CaptionRect = InfoArgs.Bounds;
			return GetHeaderPainter().CalcObjectMinBounds(InfoArgs);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(Field.ToolTips.HeaderText != string.Empty)
				return new ToolTipControlInfo(this, Field.ToolTips.HeaderText);
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			bool isFit = GetHeaderPainter().IsCaptionFit(ginfo.Cache, InfoArgs);
			ginfo.ReleaseGraphics();
			if(isFit) return null;
			return new ToolTipControlInfo(this, InfoArgs.Caption);
		}		
		public void UpdateCaption() {
			InfoArgs.Caption = Caption;
		}
		public PivotHeadersViewInfoBase HeadersViewInfo { get { return (PivotHeadersViewInfoBase)Parent; } }
		public string Caption {
			get {
				return (Field.Group != null && HeadersViewInfo == null) ? Field.Group.ToString() : Field.HeaderDisplayText;
			}
		}
		public bool IsSelected {
			get {
				return IsActive && !IsFilterDown;
			}		
		}
		protected virtual bool AddSort { get { return true; } }
		protected virtual bool AddFilter { get { return true; } }
		protected virtual bool AddImage { get { return true; } }
		protected virtual bool AddCollapseButton { get { return true; } }
		protected override PivotGridFieldBase MenuField { get { return Field; } }				
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				appearance = value;
			}
		}		
		public void Paint(GraphicsCache cache, bool selected) {
			InternalPaint(CreateViewInfoPaintArgs(cache), Bounds, selected);
		}
		protected virtual ViewInfoPaintArgs CreateViewInfoPaintArgs(GraphicsCache cache) {
			return new ViewInfoPaintArgs(null, new PaintEventArgs(cache.Graphics, Rectangle.Empty));
		}
		public void PaintDragHeader(ViewInfoPaintArgs e) {
			PaintDragHeader(e, 0);
		}
		public void PaintDragHeader(ViewInfoPaintArgs e, int locationX) {
			Rectangle drawBounds = new Rectangle(new Point(locationX, 0), Bounds.Size);
			if(HeadersViewInfo != null) {
				HeadersViewInfo.Appearance.FillRectangle(e.GraphicsCache, drawBounds);
			}
			InternalPaint(e, drawBounds, IsSelected);
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			if(PaintBounds.Width <  ViewInfo.HeaderWidthOffset * 2) return;
			InternalPaint(e, PaintBounds, IsSelected);
		}
		protected void InternalPaint(ViewInfoPaintArgs e, Rectangle paintBounds, bool selected) {
			InitInfoArgs(e.GraphicsCache, paintBounds, selected);
			GetHeaderPainter().CalcObjectBounds(InfoArgs);
			if(this.filterInfo != null) {
				this.filterInfo.SetAppearance(this.filterInfo.Filtered ? Data.PaintAppearance.HeaderFilterButtonActive : Data.PaintAppearance.HeaderFilterButton);
				this.filterInfo.State = IsFilterDown ? ObjectState.Pressed : HotTrackFilterButton ? ObjectState.Hot : ObjectState.Normal;
			}
			if(!ViewInfo.DrawFieldHeader(this, e, GetHeaderPainter()))
				GetHeaderPainter().DrawObject(InfoArgs);
		}
		protected void InitInfoArgs(GraphicsCache graphicsCache, Rectangle paintBounds, bool selected) {
			InfoArgs.Bounds = paintBounds;
			InfoArgs.CaptionRect = paintBounds;
			InfoArgs.Cache = graphicsCache;
			InfoArgs.DesignTimeSelected = Field.SelectedAtDesignTime;			
			if(IsEnabled)
				InfoArgs.State = selected ? ObjectState.Pressed : IsHotTrack ? ObjectState.Hot : ObjectState.Normal;
			else
				InfoArgs.State = ObjectState.Disabled;
		}
		protected HeaderObjectPainter GetHeaderPainter() {
			return Data.ActiveLookAndFeel.Painter.Header;
		}
		public virtual Padding GetPaddings() {
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			try {
				Rectangle minBounds = GetMinBounds(graphicsCache);
				InitInfoArgs(graphicsCache, minBounds, IsSelected);
				GetHeaderPainter().CalcObjectBounds(InfoArgs);
				Rectangle bounds = InfoArgs.Bounds,
					captionRect = InfoArgs.CaptionRect;
				return new Padding(captionRect.Left - bounds.Left, captionRect.Top - bounds.Top, 
					bounds.Right - captionRect.Right, bounds.Bottom - captionRect.Bottom);
			} finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
			}			
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotGridHeaderHitTest hitTest = PivotGridHeaderHitTest.None;
			return new PivotGridHitInfo(HeadersViewInfo, Field, hitTest, hitPoint);
		}
	}
}
