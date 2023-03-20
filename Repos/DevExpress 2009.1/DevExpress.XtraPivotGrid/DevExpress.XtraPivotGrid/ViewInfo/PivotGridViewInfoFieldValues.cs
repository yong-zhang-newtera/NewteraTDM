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
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotFieldsAreaViewInfo : PivotFieldsAreaViewInfoBase {
		public PivotFieldsAreaViewInfo(PivotGridViewInfo viewInfo, bool isColumn)
			: base(viewInfo, isColumn) { }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }
		public new PivotFieldsAreaCellViewInfo this[int index] { get { return (PivotFieldsAreaCellViewInfo)base[index]; } }
		public override int GetFieldValueSeparator(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return Data.GetFieldValueSeparator(cellViewInfo);
		}
		protected override PivotFieldsAreaCellViewInfoBase CreateFieldsAreaCellViewInfo(PivotFieldValueItem item) {
			return new PivotFieldsAreaCellViewInfo(ViewInfo, item);
		}
		public string[] GetValues(PivotFieldsAreaCellViewInfo CellViewInfo) {
			string[] result = new string[CellViewInfo.StartLevel + 1];
			PivotFieldsAreaCellViewInfo curViewInfo = CellViewInfo;
			result[curViewInfo.StartLevel] = curViewInfo.Text;
			if(result.Length == 1) return result;
			for(int i = CellViewInfo.Item.Index - 1; i >= 0; i--)
				if(this[i].StartLevel < curViewInfo.StartLevel) {
					curViewInfo = this[i];
					result[curViewInfo.StartLevel] = curViewInfo.Text;
					if(curViewInfo.StartLevel == 0)
						return result;
				}
			return null;
		}
		public PivotFieldsAreaCellViewInfo GetCellByValues(string[] values) {
			return GetCellByValues(values, 0, 0);
		}
		public PivotFieldsAreaCellViewInfo GetCellByValues(string[] values, int startIndex, int level) {
			for(int i = startIndex; i < ChildCount; i++) {
				if(this[i].StartLevel == level && this[i].Text == values[level]) {
					if(this[i].StartLevel == values.Length - 1) return this[i];
					else return GetCellByValues(values, i + 1, level + 1);
				}
			}
			return null;
		}
		public PivotFieldsAreaCellViewInfo GetValueFromTotal(PivotFieldsAreaCellViewInfo cellViewInfo) {
			if(cellViewInfo.ValueType == PivotGridValueType.Value)
				return cellViewInfo;
			if(cellViewInfo.ValueType != PivotGridValueType.Total && cellViewInfo.ValueType != PivotGridValueType.CustomTotal)
				return null;
			PivotFieldsAreaCellViewInfo curCellViewInfo = cellViewInfo;
			do {
				int newIndex = curCellViewInfo.Item.Index + (Data.OptionsView.TotalsLocation == PivotTotalsLocation.Far ? -1 : 1);
				if(newIndex < 0 || newIndex == ChildCount) 
					return null;
				curCellViewInfo = this[newIndex];
			}
			while(curCellViewInfo.ValueType != PivotGridValueType.Value && curCellViewInfo.StartLevel == cellViewInfo.StartLevel);
			return curCellViewInfo;
		}
		public int GetDataFieldsBeforeCount(PivotFieldsAreaCellViewInfo cellViewInfo) {
			int result = 0;
			for(int i = cellViewInfo.Item.Index; i >= 0; i--)
				if(this[i].ResizingField != null && this[i].ResizingField.IsDataField)
					result++;
			return result;
		}
	}
	public class PivotFieldsAreaViewInfoBase : PivotViewInfo {
		List<int> firstLevelIndexes;
		bool isColumn;
		public PivotFieldsAreaViewInfoBase(PivotGridViewInfoBase viewInfo, bool isColumn)
			: base(viewInfo) {
			this.isColumn = isColumn;
			this.firstLevelIndexes = new List<int>();
		}
		protected PivotVisualItemsBase VisualItems { get { return Data.VisualItems; } }
		public bool IsColumn { get { return isColumn; } }
		public int LevelCount { get { return VisualItems.GetLevelCount(IsColumn); } }
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public new PivotFieldsAreaCellViewInfoBase this[int index] { get { return (PivotFieldsAreaCellViewInfoBase)base[index]; } }
		public int LastLevelItemCount { get { return VisualItems.GetLastLevelItemCount(IsColumn); } }
		protected PivotFieldValueItem GetLastLevelItem(int index) {
			return VisualItems.GetLastLevelItem(IsColumn, index);
		}
		protected PivotFieldValueItem GetItem(int index) {
			return VisualItems.GetItem(IsColumn, index);
		}
		protected PivotFieldValueItem GetParentItem(PivotFieldValueItem item) {
			return VisualItems.GetParentItem(IsColumn, item);
		}
		protected int ItemCount { get { return VisualItems.GetItemCount(IsColumn); } }		
		public PivotFieldsAreaCellViewInfoBase GetLastLevelViewInfo(int index) {
			PivotFieldValueItem item = GetLastLevelItem(index);
			return item != null ? (PivotFieldsAreaCellViewInfoBase)item.Tag : null;
		}
		public PivotFieldsAreaCellViewInfoBase GetItem(int lastLevelIndex, int level) {
			PivotFieldValueItem item = GetLastLevelItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = GetParentItem(item);
			return item != null ? (PivotFieldsAreaCellViewInfoBase)item.Tag : null;
		}
		public string GetChartText(int index) {
			PivotFieldValueItem item = GetLastLevelItem(index);
			string result = GetItemDisplayText(item);
			if(item.StartLevel == 0) return result;
			while((item = GetParentItem(item)) != null)
				result = GetItemDisplayText(item) + " | " + result;
			return result;
		}
		protected string GetItemDisplayText(PivotFieldValueItem item) {
			return ((PivotFieldsAreaCellViewInfoBase)item.Tag).DisplayText;
		}
		public int GetBestWidth(PivotGridFieldBase field) {
			int bestWidth = 0;
			Graphics graphics =	GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			try {
				if(IsColumn) {
					for(int i = 0; i < ChildCount; i ++) {
						if(this[i].ResizingField == field || ChildCount == 1) {
							bestWidth = Math.Max(bestWidth, this[i].GetBestWidth(graphicsCache));
						}
					}
				} 
				else {					
					for(int i = 0; i < ChildCount; i ++) {
						if(this[i].ResizingField != field && LevelCount != 1) continue;
						int cellBestWidth = this[i].GetBestWidth(graphicsCache);
						if(this[i].IsCollapsed) 
							cellBestWidth -= GetLowerFieldsWidth(field.AreaIndex);						
						bestWidth = Math.Max(bestWidth, cellBestWidth);
					}
				}
			}
			finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return bestWidth > 0 ? bestWidth : 0;
		}
		protected int GetLowerFieldsWidth(int startAreaIndex) {
			int res = 0;
			for(int i = startAreaIndex + 1; i < Data.GetFieldCountByArea(Area); i++) {
				res += Data.GetFieldByArea(Area, i).Width;
			}
			return res;
		}
		public int GetVisibleColumnCount(int width) {
			if(IsColumn) throw new Exception("Method is not implemented");
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(Area, true);
			int count = 0, widthLeft = width;
			for(int i = 0; i < fields.Count; i++) {
				widthLeft -= fields[i].Width;
				if(widthLeft < 0) break;
				count++;
			}
			return count;
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			for(int i = 0; i < ChildCount; i ++)
				if(this[i].ControlBounds.Contains(pt))
					return this[i].GetToolTipObjectInfo(pt);
			return null;
		}
		public PivotGridFieldBase GetSizingField(Point pt) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(pt, false);
			if(viewInfo == null) return null;
			if(pt.X - viewInfo.ControlBounds.X <= ViewInfo.FrameBorderWidth) {
				pt.X -= ViewInfo.FrameBorderWidth;
				viewInfo = GetViewInfoAtPoint(pt, false);
			}
			PivotFieldsAreaCellViewInfoBase cellViewInfo = viewInfo as PivotFieldsAreaCellViewInfoBase;
			if(cellViewInfo == null) return null;
			if(Math.Abs(pt.X - cellViewInfo.ControlBounds.Right) <= ViewInfo.FrameBorderWidth 
				&& (!IsColumn || (cellViewInfo.EndLevel == LevelCount - 1))) 
				return cellViewInfo.ResizingField;
			return null;
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(hitPoint, false);
			PivotFieldsAreaCellViewInfo cellViewInfo = viewInfo as PivotFieldsAreaCellViewInfo;
			return cellViewInfo != null ? new PivotGridHitInfo(cellViewInfo, hitPoint) : new PivotGridHitInfo(hitPoint);
		}
		public int ScrollColumnCount { 
			get { 
				int count = Data.GetFieldCountByArea(Area);
				if(count == 0) count ++;
				return count;
			}
		}
		public int ScrollOffset {
			get {
				int leftTop = IsColumn ? ViewInfo.LeftTopCoord.Y : ViewInfo.LeftTopCoord.X;
				if(leftTop >= ScrollColumnCount)
					return IsColumn ? Bounds.Height : Bounds.Width;
				int offset = 0;
				for(int i = 0; i < leftTop; i ++)
					offset += IsColumn ? Data.DefaultFieldHeight : Data.GetFieldByArea(Area, i).Width;
				return offset;
			}
		}
		protected override void OnBeforeCalculating() {
			this.firstLevelIndexes.Clear();
			int itemCount = ItemCount;
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = GetItem(i);
				PivotFieldsAreaCellViewInfoBase valueViewInfo = CreateFieldsAreaCellViewInfo(item);
				item.Tag = valueViewInfo;
				AddChild(valueViewInfo);
				if(valueViewInfo.StartLevel == 0)
					this.firstLevelIndexes.Add(i);
			}
		}
		protected virtual PivotFieldsAreaCellViewInfoBase CreateFieldsAreaCellViewInfo(PivotFieldValueItem item) {
			return new PivotFieldsAreaCellViewInfoBase(ViewInfo, item);
		}
		protected override void OnAfterCalculated() {
			int levelCount = LevelCount;
			int cellTop = 0;
			int cellLeft = 0;
			int nextCellTop = 0;
			PivotFieldsAreaCellViewInfoBase lastTopCell = null;
			for(int curLevel = 0; curLevel < levelCount; curLevel ++) {
				cellLeft = 0;
				for(int i = 0; i < ChildCount; i ++) {
					if(this[i].Level == curLevel) {
						if(IsColumn) {
							this[i].Bounds.Y = cellTop;
						}
						else {
							this[i].Bounds.X = cellTop;
						}
						this[i].IsTopMost = (IsColumn && curLevel == 0) || (!IsColumn && cellLeft == 0);
						if((IsColumn && curLevel == 0) || (!IsColumn && curLevel == levelCount - 1 && cellLeft == 0))
							lastTopCell = this[i];
					}
					if(this[i].ContainsLevel(curLevel)) {
						cellLeft += GetFieldValueSeparator(this[i]);
						if(IsColumn) {
							this[i].Bounds.X = cellLeft;
							cellLeft += this[i].Bounds.Width;
							if(this[i].EndLevel == curLevel)
								nextCellTop = this[i].Bounds.Bottom;
						}
						else { 
							this[i].Bounds.Y = cellLeft;
							cellLeft += this[i].Bounds.Height;
							if(this[i].EndLevel == curLevel)
								nextCellTop = this[i].Bounds.Right;
						}
						if(this[i].Bounds.X == 0) 
							this[i].HeaderPosition = HeaderPositionKind.Left;
					}
				}
				cellTop = nextCellTop;
			}
			if(lastTopCell != null && lastTopCell.Bounds.X > 0)
				lastTopCell.HeaderPosition = HeaderPositionKind.Right;
			if(ChildCount == 1) {
				Bounds.Size = this[0].Bounds.Size;
			} else {
				Bounds.Size = IsColumn ? new Size(cellLeft, cellTop) : new Size(cellTop, cellLeft);
			}
		}
		protected override int GetStartLocationAtPoint(Point pt) {
			if(this.firstLevelIndexes.Count <= 1) return 0;
			int startIndex = 0;
			bool startExceed = IsChildViewInfoLocationExceedPoint(pt, startIndex);
			if(startExceed) return startIndex;
			int endIndex = this.firstLevelIndexes.Count - 1;
			bool endExceed = IsChildViewInfoLocationExceedPoint(pt, endIndex);
			if(!endExceed) return endIndex;
			while(endIndex - startIndex > 1) {
				int mediumIndex = startIndex + (endIndex - startIndex) / 2;
				bool exceed = IsChildViewInfoLocationExceedPoint(pt, mediumIndex);
				if(exceed)
					endIndex = mediumIndex;
				else startIndex = mediumIndex;
			}
			return this.firstLevelIndexes[startIndex];
		}
		bool IsChildViewInfoLocationExceedPoint(Point pt, int firstLevelIndex) {
			Point controlPoint = this[this.firstLevelIndexes[firstLevelIndex]].ControlBounds.Location;
			return IsColumn ? controlPoint.X > pt.X : controlPoint.Y > pt.Y;
		}
		public virtual int GetFieldValueSeparator(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return 0;
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.GetFieldSortConditions(IsColumn, cellViewInfo.Item.Index);			
		}
		public bool IsFieldSortedBySummary(PivotGridFieldBase field, PivotGridFieldBase dataField, PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.IsFieldSortedBySummary(IsColumn, field, dataField, cellViewInfo.Item.Index);
		}
		public bool GetIsAnyFieldSortedBySummary(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.GetIsAnyFieldSortedBySummary(IsColumn, cellViewInfo.Item.Index);
		}
	}
	public class PivotFieldsAreaCellViewInfo : PivotFieldsAreaCellViewInfoBase {
		const int ChangeExpandedMenuID = 201, ExpandAllMenuID = 202, CollapseAllMenuID = 203,
			RemoveSortBySummaryMenuID = 204;
		int isCaptionFit;
		public PivotFieldsAreaCellViewInfo(PivotGridViewInfo viewInfo, PivotFieldValueItem item)
			: base(viewInfo, item) {
			this.isCaptionFit = -1;
		}
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }
		public new PivotGridField Field { get { return (PivotGridField)Item.Field; } }
		public new PivotGridField ColumnField { get { return (PivotGridField)Item.ColumnField; } }
		public new PivotGridField RowField { get { return (PivotGridField)Item.RowField; } }
		public new PivotGridField ResizingField { get { return (PivotGridField)Item.ResizingField; } }
		protected override string DefaultValueToolTip {
			get {
				if(Field != null && (ValueType == PivotGridValueType.Value || Field.Area == PivotArea.DataArea))
					return Field.ToolTips.GetValueText(Value);
				return string.Empty;
			}
		}
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
		public override string DisplayText { get { return Data.GetPivotFieldValueText(this) + (Field != null && Field.Area == PivotArea.DataArea && Field.Options.ShowSummaryTypeName ? " (" + Field.SummaryType.ToString() + ")" : ""); } }
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			string defaultToolTip = DefaultValueToolTip;
			if(defaultToolTip != string.Empty) {
				return new ToolTipControlInfo(this, defaultToolTip);
			}
			if(!Data.OptionsHint.ShowValueHints) return null;
			if(isCaptionFit < 0) {
				GraphicsInfo ginfo = new GraphicsInfo();
				ginfo.AddGraphics(null);
				HeaderObjectInfoArgs info = CreateHeaderInfoArgs(ginfo.Cache);
				info.CaptionRect = GetHeaderTextPaintBounds(GetHeaderPainter().GetObjectClientRectangle(info));
				isCaptionFit = GetHeaderPainter().IsCaptionFit(ginfo.Cache, info) ? 1 : 0;
				ginfo.ReleaseGraphics();
			}
			return isCaptionFit == 0 ? new ToolTipControlInfo(this, DisplayText) : null;
		}		
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableFieldValueMenu) {
				ShowPopupMenu(e);
			}			
			return null;
		}
		public override void ChangeExpanded() {
			Data.ChangeExpanded(this);
		}
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			if(Item.ShowCollapsedButton) {
				menu.Items.Add(CreateMenuItem(Item.IsCollapsed ? GetLocalizedString(PivotGridStringId.PopupMenuExpand) : GetLocalizedString(PivotGridStringId.PopupMenuCollapse), ChangeExpandedMenuID));
				menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuExpandAll), ExpandAllMenuID));
				SetBeginGrouptoLastMenuItem();
				menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuCollapseAll), CollapseAllMenuID));
			}
			if(IsLastFieldLevel) {
				CreateSortBySummaryMenuItems(menu);
			}
		}
		protected virtual void CreateSortBySummaryMenuItems(DXPopupMenu menu) {
			List<PivotGridFieldBase> crossAreaFields = Item.GetCrossAreaFields();
			bool showRemoveAllSortingItem = false;
			string captionTemplate = GetLocalizedString(Item.Area == PivotArea.ColumnArea ? PivotGridStringId.PopupMenuSortFieldByColumn : PivotGridStringId.PopupMenuSortFieldByRow);
			List<PivotGridFieldBase> dataFields = IsDataLocatedInThisArea || Data.DataFieldCount == 1 ? null : Data.GetFieldsByArea(PivotArea.DataArea, false);
			for(int i = 0; i < crossAreaFields.Count; i++) {
				PivotGridFieldBase field = crossAreaFields[i];
				if(!field.CanSortBySummary) continue;
				if(IsDataLocatedInThisArea || Data.DataFieldCount == 1) {
					showRemoveAllSortingItem |= CreateSortByMenuItem(menu, captionTemplate, field, Item.DataField, i == 0);
				} else {
					showRemoveAllSortingItem |= CreateSortByWithDataMenuItems(menu, captionTemplate, field, dataFields, i == 0);
				}
			}
			if(showRemoveAllSortingItem) {
				menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuRemoveAllSortByColumn), 
					RemoveSortBySummaryMenuID, true, -1, true));
			}
		}
		bool CreateSortByMenuItem(DXPopupMenu menu, string captionTemplate, PivotGridFieldBase field, PivotGridFieldBase dataField, bool beginGroup) {
			string caption = string.Format(captionTemplate, field.HeaderDisplayText);
			return CreateSortByMenuItemCore(menu, field, dataField, beginGroup, caption);
		}
		bool CreateSortByMenuItemCore(DXPopupMenu menu, PivotGridFieldBase field, PivotGridFieldBase dataField, bool beginGroup, string caption) {
			bool isChecked = Parent.IsFieldSortedBySummary(field, dataField, this);
			menu.Items.Add(CreateMenuCheckItem(caption, isChecked, new PivotGridFieldPair(field, dataField), beginGroup && menu.Items.Count > 0));
			return isChecked;
		}
		bool CreateSortByWithDataMenuItems(DXPopupMenu menu, string captionTemplate, PivotGridFieldBase field, List<PivotGridFieldBase> dataFields, bool beginGroup) {
			bool result = false;
			for(int i = 0; i < dataFields.Count; i++) {
				PivotGridFieldBase dataField = dataFields[i];
				string caption = string.Format(captionTemplate, field.HeaderDisplayText + " - " + dataField.HeaderDisplayText);
				result |= CreateSortByMenuItemCore(menu, field, dataField, beginGroup && i == 0, caption);
			}
			return result;
		}
#if DEBUGTEST
		internal DXPopupMenu CreatePopupMenuItemsTest() {
			DXPopupMenu menu = new DXPopupMenu();
			CreatePopupMenuItems(menu);
			return menu;
		}
#endif
		protected override void OnMenuItemClick(DXMenuItem menuItem) {
			if(menuItem.Tag is int) {
				switch((int)menuItem.Tag) {
					case ChangeExpandedMenuID:
						ChangeExpanded();
						break;
					case ExpandAllMenuID:
						Field.ExpandAll();
						break;
					case CollapseAllMenuID:
						Field.CollapseAll();
						break;
					case RemoveSortBySummaryMenuID:
						RemoveSortBySummary();
						break;
				}
			}
			PivotGridFieldPair pair = menuItem.Tag as PivotGridFieldPair;
			DXMenuCheckItem checkMenuItem = menuItem as DXMenuCheckItem;
			if(checkMenuItem != null && pair != null) {
				Data.BeginUpdate();
				SetFieldSortBySummary(pair.Field, pair.DataField, checkMenuItem.Checked);
				Data.EndUpdate();
			}
		}		
		protected void RemoveSortBySummary() {
			List<PivotGridFieldBase> crossAreaFields = Item.GetCrossAreaFields();
			Data.BeginUpdate();
			for(int i = 0; i < crossAreaFields.Count; i++) {
				if(crossAreaFields[i].CanSortBySummary && Parent.IsFieldSortedBySummary(crossAreaFields[i], null, this))
					SetFieldSortBySummary(crossAreaFields[i], null, false);
			}
			Data.EndUpdate();
		}		
		protected override PivotFieldCollapsedButtonViewInfoBase CreateCollapsedButtonViewInfo() {
			return new PivotFieldValueCollapsedButtonViewInfo(this);
		}
		protected override Size CalculateCellSize() {
			Size res = base.CalculateCellSize();
			if(!IsLastFieldLevel)
				return res;
			if(IsColumn) 
				res.Width = Data.GetCustomColumnWidth(this, res.Width);
			else
				res.Height = Data.GetCustomRowHeight(this, res.Height);
			return res;
		}
	}	
	public class PivotFieldsAreaCellViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner {
		public const int HeaderTextOffset = 2;
		PivotFieldValueItem item;
		HeaderPositionKind headerPosition;
		bool isTopMost;
		Nullable<bool> isAnyFieldSortedBySummary;
		AppearanceObject appearance;
		GlyphElementInfoArgs glyphInfo;
		Image indicatorImage;
		PivotFieldCollapsedButtonViewInfoBase collapsedButton;
		public PivotFieldsAreaCellViewInfoBase(PivotGridViewInfoBase viewInfo, PivotFieldValueItem item)
			: base(viewInfo) {
			this.item = item;
			this.headerPosition = HeaderPositionKind.Center;
			this.isTopMost = false;
		}
		protected new PivotFieldsAreaViewInfoBase Parent { get { return (PivotFieldsAreaViewInfoBase)base.Parent; } }
		public PivotFieldValueItem Item { get { return item; } }
		public bool IsColumn { get { return Item.IsColumn; } }
		public int CellCount { get { return item.CellCount; } }
		public PivotFieldsAreaCellViewInfoBase GetCell(int index) {
			return (PivotFieldsAreaCellViewInfoBase)item.GetCell(index).Tag;
		}
		public PivotFieldsAreaCellViewInfo[] GetLastLevelChildren()  {
			PivotFieldValueItem[] items = Item.GetLastLevelCells();
			PivotFieldsAreaCellViewInfo[] children = new PivotFieldsAreaCellViewInfo[items.Length];
			for(int i = 0; i < items.Length; i ++) {
				children[i] = (PivotFieldsAreaCellViewInfo)items[i].Tag;
			}
			return children;
		}
		public int VisibleIndex { get { return Item.VisibleIndex; } }
		public int Level {	get { return Item.Level; } }
		public int StartLevel { get { return Item.StartLevel; }  }
		public int EndLevel { get { return Item.EndLevel; }  }
		public bool IsLastFieldLevel { get { return Item.IsLastFieldLevel; } }
		public bool ContainsLevel(int level) { return Item.ContainsLevel(level); }
		public int DataIndex { get { return Item.DataIndex; } }
		public PivotSummaryType SummaryType { get { return Item.SummaryType; } }
		public PivotGridCustomTotal CustomTotal { get { return (PivotGridCustomTotal)Item.CustomTotal; } }
		public string Text { get { return Item.Text; } }
		public bool IsOthersValue { get { return Item.IsOthersRow; } }
		public PivotGridValueType ValueType { get {	return Item.ValueType; } }
		public object Value { get { return Item.Value; } }
		public PivotGridField Field { get { return (PivotGridField)Item.Field; } }
		public PivotGridField ColumnField { get { return (PivotGridField)Item.ColumnField; } }
		public PivotGridField RowField { get { return (PivotGridField)Item.RowField; } }
		public PivotGridField ResizingField { get { return (PivotGridField)Item.ResizingField; } }
		public virtual string DisplayText { get { return Text; } }
		public int MinLastLevelIndex { get { return Item.MinLastLevelIndex; } }
		public int MaxLastLevelIndex { get { return Item.MaxLastLevelIndex; } }
		protected bool IsDataLocatedInThisArea { get { return Item.IsDataFieldsVisible; } }
		public HeaderPositionKind HeaderPosition { get { return headerPosition; } set { headerPosition = value; } }
		public bool IsTopMost { get { return isTopMost; } set { isTopMost = value; } }
		public virtual int GetBestWidth(GraphicsCache graphicsCache) {
			HeaderObjectInfoArgs info = CreateHeaderInfoArgs(graphicsCache);
			return GetBestWidthCore(info);
		}
		protected int GetBestWidthCore(HeaderObjectInfoArgs info) {
			Padding paddings = GetPaddings();
			return GetHeaderPainter().CalcObjectMinBounds(info).Width + paddings.Left + paddings.Right;
		}
		public Padding GetPaddings() {
			return new Padding(GetLeftHeaderTextOffset(), 0, GetRightHeaderTextOffset(), 0);
		}
		protected virtual string DefaultValueToolTip {
			get { return string.Empty; }
		}
		public int Separator {
			get {
				PivotFieldsAreaViewInfoBase fieldsArea = Parent;
				return fieldsArea != null ? fieldsArea.GetFieldValueSeparator(this) : 0;
			}
		}
		protected PivotFieldCollapsedButtonViewInfoBase CollapsedButton { get { return collapsedButton; } }
		protected GlyphElementInfoArgs GlyphInfo { get { return glyphInfo; } }
		protected Image IndicatorImage { get { return indicatorImage; } }		
		public AppearanceObject Appearance { 
			get { 
				if(appearance == null) {
					appearance = new AppearanceObject();
					AppearanceHelper.Combine(appearance, GetCombinedAppearanceObjects());
					if(appearance.TextOptions.VAlignment == VertAlignment.Default)
						appearance.TextOptions.VAlignment = VertAlignment.Top;
				}
				return appearance; 
			} 
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		protected virtual AppearanceObject[] GetCombinedAppearanceObjects() {
			switch(ValueType) {
				case PivotGridValueType.GrandTotal:
					return new AppearanceObject[] { ViewInfo.PrintAndPaintAppearance.FieldValueGrandTotal, ViewInfo.PrintAndPaintAppearance.FieldValueTotal, ViewInfo.PrintAndPaintAppearance.FieldValue };
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
					if(Field == null) 
						return new AppearanceObject[] { ViewInfo.PrintAndPaintAppearance.FieldValueTotal, Field.Appearance.Value, ViewInfo.PrintAndPaintAppearance.FieldValue };
					else return new AppearanceObject[] { Field.Appearance.ValueTotal, ViewInfo.PrintAndPaintAppearance.FieldValueTotal, Field.Appearance.Value, ViewInfo.PrintAndPaintAppearance.FieldValue };
				default:
					if(Field == null)
						return new AppearanceObject[] { ViewInfo.PrintAndPaintAppearance.FieldValue };
					else return new AppearanceObject[] { Field.Appearance.Value, ViewInfo.PrintAndPaintAppearance.FieldValue };
			}
		}
		protected override void OnBeforeCalculating() {
			base.OnBeforeCalculating();
			for(int i = 0; i < CellCount; i ++)
				GetCell(i).EnsureIsCalculated();
			if(Item.ShowCollapsedButton) {
				this.collapsedButton = CreateCollapsedButtonViewInfo();
				AddChild(CollapsedButton);
			}
		}
		protected virtual PivotFieldCollapsedButtonViewInfoBase CreateCollapsedButtonViewInfo() {
			return new PivotFieldCollapsedButtonViewInfoBase(this);
		}
		protected override void OnAfterCalculated() {
			Bounds.Size = CalculateCellSize();
			CreateCollapsedButton();
		}
		protected virtual Size CalculateCellSize() {
			if(IsColumn) {
				int width = 0;
				if(CellCount > 0) 
					width = GetChildCellsWidth();
				else 
					width = ResizingField != null ? ResizingField.Width : Data.DefaultFieldWidth;
				return new Size(width, GetColumnCellHeight());
			} else {				
				if(CellCount > 0) 
					return new Size(GetRowHeaderWidth(), GetChildCellsHeight());
				else 
					return new Size(GetRowHeaderWidth(), ViewInfo.GetFieldHeight(GetRowLineCount()));
			}
		}
		private int GetColumnCellHeight() {
			if(Data.GetFieldCountByArea(PivotArea.ColumnArea) == 0) 
				return ViewInfo.GetFieldHeight(1);
			int height = 0;
			for(int i = Item.StartLevel; i <= Item.EndLevel; i++) {
				PivotGridField field = (PivotGridField)Data.GetFieldByLevel(true, i);
				height += ViewInfo.GetFieldHeight(field.ColumnValueLineCount);
			}
			return height;
		}
		public int GetRowFieldWidth(int level) {
			if(Item.IsDataFieldsVisible && Item.DataLevel == level) return Data.DataField.Width;
			if(Item.IsLevelAfterDataField(level)) level--;
			return Data.GetFieldWidth(PivotArea.RowArea, level);
		}
		public int GetRowHeaderWidth() {
			if(ValueType == PivotGridValueType.GrandTotal && ResizingField != null) {
				return ResizingField.Width;
			}
			if(IsColumn || StartLevel < 0) return Data.DefaultFieldWidth;
			int width = 0;
			for(int i = StartLevel; i <= EndLevel; i++)
				width += GetRowFieldWidth(i);
			return width;
		}
		public int GetChildCellsWidth() {
			int width = 0;
			for(int i = 0; i < CellCount; i++)
				width += GetCell(i).Bounds.Width;
			return width;
		}
		public int GetChildCellsHeight() {
			int height = 0;
			for(int i = 0; i < CellCount; i++)
				height += GetCell(i).Bounds.Height;
			return height;
		}
		public int GetColumnLineCount() {
			int columnLineCount = Item.CellLevelCount;
			if(Data.GetFieldCountByArea(PivotArea.ColumnArea) == 0) return columnLineCount;
			for(int i = Item.StartLevel; i <= Item.EndLevel; i++) {
				PivotGridField field = (PivotGridField)Data.GetFieldByLevel(true, i);
				columnLineCount += field.ColumnValueLineCount - 1;
			}
			return columnLineCount;
		}
		public int GetRowLineCount() {
			int rowLineCount = 1;
			if(Data.GetFieldCountByArea(PivotArea.RowArea) == 0) return rowLineCount;
			for(int i = Item.StartLevel; i <= Item.EndLevel; i++) {
				PivotGridField field = (PivotGridField)Data.GetFieldByLevel(false, i);
				rowLineCount += field.RowValueLineCount - 1;
			}
			return rowLineCount;
		}
		void CreateCollapsedButton() {
			if(CollapsedButton == null) return;
			HeaderObjectInfoArgs info = new HeaderObjectInfoArgs();
			info.Bounds = Bounds;
			Rectangle clientBounds = GetHeaderPainter().GetObjectClientRectangle(info);
			Rectangle buttonBounds = CollapsedButton.Bounds;
			buttonBounds.Location = clientBounds.Location;
			int verticalOffset = (ViewInfo.GetFieldHeight(1) - buttonBounds.Height) / 2 - buttonBounds.Location.Y;
			buttonBounds.Offset(0, verticalOffset);
			CollapsedButton.Bounds = buttonBounds;
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			HeaderObjectInfoArgs info = CreateHeaderInfoArgs(e.GraphicsCache);
			UpdateHeaderCaptionRect(info);
			if(!ViewInfo.CustomDrawFieldValue(e, this, info, GetHeaderPainter())) {
				UpdateHeaderCaptionRect(info);
				GetHeaderPainter().DrawObject(info);
				DrawAdditionElements(e, info);
			}
		}
		void UpdateHeaderCaptionRect(HeaderObjectInfoArgs info) {
			Rectangle contentRect = GetHeaderPainter().GetObjectClientRectangle(info),
				captionRect = GetHeaderTextPaintBounds(contentRect);
			if(!IsColumn && CollapsedButton != null && !Parent.ControlBounds.Contains(captionRect)) { 
				captionRect = RectangleHelper.Shrink(captionRect, -(CollapsedButton.Bounds.Width + HeaderTextOffset), 0, 0, 0);
			}
			Rectangle visibleRect = CalcVisibleRect(contentRect);
			info.CaptionRect = Rectangle.Intersect(visibleRect, captionRect);
			if(visibleRect.X > captionRect.X) 
				info.CaptionRect = RectangleHelper.Shrink(info.CaptionRect, HeaderTextOffset, 0, 0, 0);
			GetHeaderPainter().CalcBoundsByClientRectangle(info, info.Bounds);
		}
		Rectangle CalcVisibleRect(Rectangle contentRect) {
			int glyphWidth = (IsColumn && GlyphInfo != null) ?  GlyphInfo.Bounds.Width : 0;
			return new Rectangle(contentRect.X + glyphWidth,
				contentRect.Y,
				(Root.Bounds.Width - HeaderTextOffset) - contentRect.X - glyphWidth,
				(Root.Bounds.Height - HeaderTextOffset) - contentRect.Y);
		}
		protected virtual void DrawAdditionElements(ViewInfoPaintArgs e, HeaderObjectInfoArgs info) {
			if(GlyphInfo != null) {
				int x = info.CaptionRect.X - HeaderTextOffset - GlyphInfo.Bounds.Width;
				int y = info.CaptionRect.Y;
				GlyphInfo.Bounds = new Rectangle(new Point(x, y), GlyphInfo.Bounds.Size);
				(new GlyphElementPainter()).DrawObject(GlyphInfo);
			}
			if(IndicatorImage != null) {
				int x = info.CaptionRect.Right + HeaderTextOffset;
				int y = info.CaptionRect.Y + (int)Math.Round((double)(info.CaptionRect.Height - IndicatorImage.Height) / 2);
				e.GraphicsCache.Paint.DrawImage(e.Graphics, IndicatorImage, new Point(x, y));
			}
		}
		protected virtual HeaderObjectInfoArgs CreateHeaderInfoArgs(GraphicsCache graphicsCache) {
			Rectangle bounds = HeaderPaintBounds;
			HeaderObjectInfoArgs info = new HeaderObjectInfoArgs();
			info.Cache = graphicsCache;
			info.Caption = DisplayText;
			info.Bounds = bounds;
			info.SetAppearance(Appearance);
			if(HeaderPosition == HeaderPositionKind.Left && IsColumn && ControlBounds.Width != Bounds.Width)
				info.HeaderPosition = HeaderPositionKind.Center;
			else info.HeaderPosition = HeaderPosition;
			info.IsTopMost = IsTopMost && (IsColumn || ControlBounds.Height == Bounds.Height);
			if(Data.ValueImages != null) {
				int imageIndex = Data.GetPivotFieldImageIndex(this);
				if(imageIndex > -1) {
					this.glyphInfo = new GlyphElementInfoArgs(Data.ValueImages, imageIndex, null);
					GlyphInfo.Cache = graphicsCache;
					GlyphInfo.Bounds = new Rectangle(Point.Empty, GlyphInfo.GlyphSize);
				}
			}
			if(IsAnyFieldSortedByThisSummary && IsLastFieldLevel) {
				if(Data.PaintAppearance.SortByColumnIndicatorImage != null)
					this.indicatorImage = Data.PaintAppearance.SortByColumnIndicatorImage;
				else
					this.indicatorImage = GetIndicatorPainter().ImageList.Images[9];
			}
			GetHeaderPainter().CalcObjectBounds(info);
			return info;
		}
		protected HeaderObjectPainter GetHeaderPainter() {
			return Data.ActiveLookAndFeel.Painter.Header;
		}
		protected IndicatorObjectPainter GetIndicatorPainter() {
			return Data.ActiveLookAndFeel.Painter.Indicator;
		}
		protected Rectangle HeaderPaintBounds { get {return PaintBounds; } }
		protected virtual int GetLeftHeaderTextOffset() {
			int offset = HeaderTextOffset;
			if(GlyphInfo != null)
				offset += GlyphInfo.Bounds.Width + HeaderTextOffset;
			if(CollapsedButton != null) 
				offset += CollapsedButton.Bounds.Width + HeaderTextOffset;
			return offset;
		}
		protected virtual int GetRightHeaderTextOffset() {
			int res = 0;
			if(IndicatorImage != null)
				res += IndicatorImage.Width + HeaderTextOffset;
			return res;
		}
		protected Rectangle GetHeaderTextPaintBounds(Rectangle bounds) {
			if(HeaderPaintBounds.Width != Bounds.Width) {
				bounds.X -= Bounds.Width - bounds.Width;
				bounds.Width = Bounds.Width;
			}
			if(HeaderPaintBounds.Height != Bounds.Height) {
				bounds.Y -= Bounds.Height - bounds.Height;
				bounds.Height = Bounds.Height;
			}
			bounds.X += GetLeftHeaderTextOffset();
			bounds.Width -= GetLeftHeaderTextOffset() + GetRightHeaderTextOffset();
			return bounds;
		}		
		protected override PivotGridMenuType MenuType { get { return PivotGridMenuType.FieldValue; } }
		protected override PivotArea MenuArea { get { return Item.Area; } }		
		public bool IsCollapsed { get { return Item.IsCollapsed; } }
		public virtual void ChangeExpanded() {}
		protected internal bool IsAnyFieldSortedByThisSummary { 
			get {
				if(isAnyFieldSortedBySummary == null)
					isAnyFieldSortedBySummary = Parent.GetIsAnyFieldSortedBySummary(this);
				return isAnyFieldSortedBySummary.Value; 
			} 
		}
		protected void SetFieldSortBySummary(PivotGridFieldBase field, PivotGridFieldBase dataField, bool sort) {
			if(!field.CanSortBySummary) return;
			if(sort) {
				field.SortBySummaryInfo.Field = dataField;
				field.SortBySummaryInfo.Conditions.Clear();
				field.SortBySummaryInfo.Conditions.AddRange(GetFieldSortConditions());
			} else {
				field.SortBySummaryInfo.Reset();
			}
		}
		protected internal List<PivotGridFieldSortCondition> GetFieldSortConditions() {			
			return Parent.GetFieldSortConditions(this);
		}
	}
	public class PivotFieldValueCollapsedButtonViewInfo : PivotFieldCollapsedButtonViewInfoBase {
		public PivotFieldValueCollapsedButtonViewInfo(PivotFieldsAreaCellViewInfo cellViewInfo)
			: base(cellViewInfo) {}
		protected new PivotFieldsAreaCellViewInfo CellViewInfo { get { return (PivotFieldsAreaCellViewInfo)base.CellViewInfo; } }
		protected override void DoChangeState() {
			PivotFieldsAreaCellViewInfo oldCellViewInfo = GetParent().GetValueFromTotal(CellViewInfo);
			int oldCellCount = oldCellViewInfo.Item.CellCount + oldCellViewInfo.Item.TotalsCount,
				dataFieldsBefore = GetParent().GetDataFieldsBeforeCount(oldCellViewInfo);
			if(dataFieldsBefore > 0)
				dataFieldsBefore--;
			string[] values = GetParent().GetValues(oldCellViewInfo);
			Point oldLeftTopCoord = CellViewInfo.ViewInfo.LeftTopCoord;
			CellViewInfo.ChangeExpanded();
			CellViewInfo.ViewInfo.EnsureIsCalculated();
			if(CellViewInfo.Data.OptionsDataField.DataFieldVisible &&
				(CellViewInfo.Data.OptionsDataField.Area == PivotDataArea.ColumnArea && CellViewInfo.IsColumn ||
					CellViewInfo.Data.OptionsDataField.Area == PivotDataArea.RowArea && !CellViewInfo.IsColumn) &&
				CellViewInfo.Data.OptionsDataField.AreaIndex < CellViewInfo.Item.StartLevel) {
				PivotFieldsAreaCellViewInfo newCellViewInfo = GetParent().GetCellByValues(values);
				int newCellCount = newCellViewInfo.Item.CellCount + newCellViewInfo.Item.TotalsCount,
					deltaIndex = (newCellCount - oldCellCount + (newCellViewInfo.IsCollapsed ? 1 : -1)) * dataFieldsBefore;
				if(CellViewInfo.IsColumn)
					CellViewInfo.ViewInfo.LeftTopCoord = new Point(oldLeftTopCoord.X + deltaIndex, oldLeftTopCoord.Y);
				else
					CellViewInfo.ViewInfo.LeftTopCoord = new Point(oldLeftTopCoord.X, oldLeftTopCoord.Y + deltaIndex);
			}
		}
		PivotFieldsAreaViewInfo GetParent() {
			return CellViewInfo.IsColumn ? CellViewInfo.ViewInfo.ColumnAreaFields : CellViewInfo.ViewInfo.RowAreaFields;
		}
	}
	public class PivotFieldCollapsedButtonViewInfoBase : BaseViewInfo {
		PivotFieldsAreaCellViewInfoBase cellViewInfo;
		PivotGridViewInfoData data;
		public PivotFieldCollapsedButtonViewInfoBase(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			this.cellViewInfo = cellViewInfo;
			this.data = cellViewInfo.Data;
		}
		public PivotGridViewInfoData Data { get { return data; } }
		protected PivotFieldsAreaCellViewInfoBase CellViewInfo { get { return cellViewInfo; } }
		protected virtual bool IsCollapsed { get { return CellViewInfo.IsCollapsed; } }
		protected virtual void DoChangeState() {  }
		protected override void OnAfterCalculated() {
			OpenCloseButtonInfoArgs info = new OpenCloseButtonInfoArgs(null);
			Bounds.Size = GetObjectPainter().CalcObjectMinBounds(info).Size;
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				DoChangeState();
			return null;
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			OpenCloseButtonInfoArgs info = new OpenCloseButtonInfoArgs(e.GraphicsCache);
			info.SetAppearance(Data.PaintAppearance.ExpandButton);
			info.Opened = !IsCollapsed;
			info.Bounds = PaintBounds;
			info.State = IsActive ? ObjectState.Pressed : ObjectState.Normal;
			GetObjectPainter().DrawObject(info);
		}
		protected ObjectPainter GetObjectPainter() {
			return Data.ActiveLookAndFeel.Painter.OpenCloseButton;
		}
	}	
}
