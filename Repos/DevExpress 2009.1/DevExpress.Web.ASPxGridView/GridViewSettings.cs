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
using DevExpress.Web.ASPxPager;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Localization;
using DevExpress.Web.ASPxGridView.Localization;
using DevExpress.XtraGrid;
namespace DevExpress.Web.ASPxGridView {
	public enum GridViewPagerMode { ShowAllRecords, ShowPager }
	public enum ColumnResizeMode { Disabled, Control, NextColumn }
	public enum GridViewVerticalScrollBarStyle { Standard, Virtual }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxGridViewBaseSettings : StateManager {
		ASPxGridView grid;
		public ASPxGridViewBaseSettings(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected virtual void OnChanged() {
			Grid.OnSettingsChanged(this);
		}
	}
	public class ASPxGridViewPagerSettings : PagerSettingsEx {
		ASPxGridView grid;
		public ASPxGridViewPagerSettings(ASPxGridView grid) : base(grid) { 
			this.grid = grid;
			Position = PagerPosition.Bottom;
		}
		protected ASPxGridView Grid { get { return grid; } }
		[Description("Gets or sets whether page-mode navigation is enabled."),
		DefaultValue(GridViewPagerMode.ShowPager), NotifyParentProperty(true), AutoFormatDisable]
		public GridViewPagerMode Mode {
			get { return (GridViewPagerMode)GetIntProperty("Mode", (int)GridViewPagerMode.ShowPager); }
			set {
				if(value == Mode) return;
				SetIntProperty("Mode", (int)GridViewPagerMode.ShowPager, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets the maximum number of rows that can be displayed on a page."),
		DefaultValue(10), NotifyParentProperty(true), AutoFormatDisable]
		public int PageSize {
			get { return GetIntProperty("PageSize", 10); }
			set {
				if(value < 0) return;
				if(value == PageSize) return;
				SetIntProperty("PageSize", 10, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the pager's position within an ASPxGridView control."),
		NotifyParentProperty(true), DefaultValue(PagerPosition.Bottom)]
		public override PagerPosition Position {
			get { return base.Position; }
			set { base.Position = value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable,
		Description("Gets or sets whether Search-Engine Optimization (SEO) mode is enabled.")]
		public override SEOFriendlyMode SEOFriendly {
			get { return base.SEOFriendly; }
			set {
				if(base.SEOFriendly == value)
					return;
				base.SEOFriendly = value;
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Pager is displayed within the ASPxGridView when there is only one page."),
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool AlwaysShowPager {
			get { return GetBoolProperty("AlwaysShowPager", false); }
			set {
				if(value == AlwaysShowPager) return;
				SetBoolProperty("AlwaysShowPager", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether to show empty data rows if the number of data rows displayed within the last page fits entirely on the page."),
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowEmptyDataRows {
			get { return GetBoolProperty("ShowEmptyDataRows", false); }
			set {
				if (value == ShowEmptyDataRows) return;
				SetBoolProperty("ShowEmptyDataRows", false, value);
				OnChanged();
			}
		}
		protected virtual void OnChanged() {
			Grid.OnSettingsChanged(this);
		}
	}
	public class ASPxGridViewBehaviorSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewBehaviorSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets whether end-users can drag column headers."), DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowDragDrop {
			get { return GetBoolProperty("AllowDragDrop", true); }
			set {
				if(value == AllowDragDrop) return;
				SetBoolProperty("AllowDragDrop", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether end-users can sort data."), DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowSort {
			get { return GetBoolProperty("AllowSort", true); }
			set {
				if(value == AllowSort) return;
				SetBoolProperty("AllowSort", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether end-users can group data."), DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowGroup {
			get { return GetBoolProperty("AllowGroup", true); }
			set {
				if(value == AllowGroup) return;
				SetBoolProperty("AllowGroup", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the focused row is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowFocusedRow {
			get { return GetBoolProperty("AllowFocusedRow", false); }
			set {
				if(value == AllowFocusedRow) return;
				SetBoolProperty("AllowFocusedRow", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether end-users can select multiple data rows by clicking them."), DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowMultiSelection {
			get { return GetBoolProperty("AllowMultiSelection", false); }
			set {
				if(value == AllowMultiSelection) return;
				SetBoolProperty("AllowMultiSelection", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets a value that specifies how columns are resized when an end-user changes a column's width."), DefaultValue(ColumnResizeMode.Disabled), NotifyParentProperty(true)]
		public ColumnResizeMode ColumnResizeMode {
			get { return (ColumnResizeMode)GetIntProperty("ColumnResizeMode", (int)ColumnResizeMode.Disabled); }
			set {
				if(value == ColumnResizeMode) return;
				SetIntProperty("ColumnResizeMode", (int)ColumnResizeMode.Disabled, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Confirm Delete window is displayed when deleting a row."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ConfirmDelete {
			get { return GetBoolProperty("ConfirmDelete", false); }
			set {
				if(value == ConfirmDelete) return;
				SetBoolProperty("ConfirmDelete", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the ASPxGridView keeps its row errors that are HTML as HTML, or instead, strips out the HTML markers."), DefaultValue(false), NotifyParentProperty(true)]
		public bool EncodeErrorHtml {
			get { return GetBoolProperty("EncodeErrorHtml", false); }
			set {
				if(value == EncodeErrorHtml) return;
				SetBoolProperty("EncodeErrorHtml", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets a value that specifies whether the ASPxClientGridView.SelectionChanged event should be finally processed on the server side."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ProcessSelectionChangedOnServer {
			get { return GetBoolProperty("ProcessSelectionChangedOnServer", false); }
			set {
				if(value == ProcessSelectionChangedOnServer) return;
				SetBoolProperty("ProcessSelectionChangedOnServer", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets a value that specifies whether the ASPxClientGridView.FocusedRowChanged event should be finally processed on the server side."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ProcessFocusedRowChangedOnServer {
			get { return GetBoolProperty("ProcessFocusedRowChangedOnServer", false); }
			set {
				if(value == ProcessFocusedRowChangedOnServer) return;
				SetBoolProperty("ProcessFocusedRowChangedOnServer", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the time interval between the time a user starts typing within the Auto Filter Row, and filtering is applied."), DefaultValue(1200), NotifyParentProperty(true)]
		public int AutoFilterRowInputDelay {
			get { return GetIntProperty("AutoFilterRowInputDelay", 1200); }
			set {
				if(value == AutoFilterRowInputDelay) return;
				SetIntProperty("AutoFilterRowInputDelay", 1200, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether all group rows displayed within the ASPxGridView are automatically expanded."), DefaultValue(false), NotifyParentProperty(true)]
		public bool AutoExpandAllGroups {
			get { return GetBoolProperty("AutoExpandAllGroups", false); }
			set {
				if(value == AutoExpandAllGroups) return;
				SetBoolProperty("AutoExpandAllGroups", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Filter Dropdown's default height."), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public Unit HeaderFilterDefaultHeight {
			get { return (Unit)GetObjectProperty("HeaderFilterDefaultHeight", Unit.Empty); }
			set {
				if (value.Equals(HeaderFilterDefaultHeight)) return;
				SetObjectProperty("HeaderFilterDefaultHeight", Unit.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets the maximum allowed number of items that can be displayed within a column's filter dropdown."), DefaultValue(-1), NotifyParentProperty(true)]
		public int HeaderFilterMaxRowCount {
			get { return GetIntProperty("HeaderFilterMaxRowCount", -1); }
			set {
				if (value < 0) value = -1;
				if (value > 100) value = 100;
				if (value == HeaderFilterMaxRowCount) return;
				SetIntProperty("HeaderFilterMaxRowCount", -1, value);
				OnChanged();
			}
		}
		[Description("Gets or sets how data is sorted."), DefaultValue(ColumnSortMode.Default), NotifyParentProperty(true)]
		public ColumnSortMode SortMode {
			get { return (ColumnSortMode)GetEnumProperty("SortMode", ColumnSortMode.Default); }
			set {
				if(value == SortMode) return;
				SetEnumProperty("SortMode", ColumnSortMode.Default, value);
				OnChanged();
			}
		}
	}
	public enum GridViewStatusBarMode { Auto, Hidden, Visible }
	public enum GridViewGroupFooterMode { Hidden, VisibleIfExpanded, VisibleAlways };
	public enum GridViewNewItemRowPosition { Top, Bottom }
	public class ASPxGridViewSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets whether the ASPxGridView's Title Panel is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowTitlePanel {
			get { return GetBoolProperty("ShowTitlePanel", false); }
			set {
				if(value == ShowTitlePanel) return;
				SetBoolProperty("ShowTitlePanel", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the filter row is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFilterRow {
			get { return GetBoolProperty("ShowFilterRow", false); }
			set {
				if(value == ShowFilterRow) return;
				SetBoolProperty("ShowFilterRow", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether filter row buttons are displayed within the auto filter row."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFilterRowMenu {
			get { return GetBoolProperty("ShowFilterRowMenu", false); }
			set {
				if(value == ShowFilterRowMenu) return;
				SetBoolProperty("ShowFilterRowMenu", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether column headers display filter buttons."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowHeaderFilterButton {
			get { return GetBoolProperty("ShowHeaderFilterButton", false); }
			set {
				if(value == ShowHeaderFilterButton) return;
				SetBoolProperty("ShowHeaderFilterButton", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Group Panel is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowGroupPanel {
			get { return GetBoolProperty("ShowGroupPanel", false); }
			set {
				if(value == ShowGroupPanel) return;
				SetBoolProperty("ShowGroupPanel", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether group expand buttons are displayed within group rows."), DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowGroupButtons {
			get { return GetBoolProperty("ShowGroupButtons", true); }
			set {
				if(value == ShowGroupButtons) return;
				SetBoolProperty("ShowGroupButtons", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the view's footer is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFooter {
			get { return GetBoolProperty("ShowFooter", false); }
			set {
				if(value == ShowFooter) return;
				SetBoolProperty("ShowFooter", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets a value that specifies when the ASPxGridView displays group footers."), DefaultValue(GridViewGroupFooterMode.Hidden), NotifyParentProperty(true)]
		public GridViewGroupFooterMode ShowGroupFooter {
			get { return (GridViewGroupFooterMode)GetEnumProperty("ShowGroupFooter", GridViewGroupFooterMode.Hidden); }
			set {
				if(value == ShowGroupFooter) return;
				SetEnumProperty("ShowGroupFooter", GridViewGroupFooterMode.Hidden, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether preview rows are displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowPreview {
			get { return GetBoolProperty("ShowPreview", false); }
			set {
				if(value == ShowPreview) return;
				SetBoolProperty("ShowPreview", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether column headers are displayed."), DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowColumnHeaders {
			get { return GetBoolProperty("ShowColumnHeaders", true); }
			set {
				if(value == ShowColumnHeaders) return;
				SetBoolProperty("ShowColumnHeaders", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the vertical scrollbar is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowVerticalScrollBar {
			get { return GetBoolProperty("ShowVerticalScrollBar", false); }
			set {
				if(value == ShowVerticalScrollBar) return;
				SetBoolProperty("ShowVerticalScrollBar", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the horizontal scrollbar is displayed."), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowHorizontalScrollBar {
			get { return GetBoolProperty("ShowHorizontalScrollBar", false); }
			set {
				if(value == ShowHorizontalScrollBar) return;
				SetBoolProperty("ShowHorizontalScrollBar", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the scrollable area's height."), DefaultValue(200), NotifyParentProperty(true)]
		public int VerticalScrollableHeight {
			get { return GetIntProperty("VerticalScrollableHeight", 200); }
			set {
				value = Math.Max(50, value);
				if(value == VerticalScrollableHeight) return;
				SetIntProperty("VerticalScrollableHeight", 200, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the operating mode of the vertical scrollbar."), DefaultValue(GridViewVerticalScrollBarStyle.Standard), NotifyParentProperty(true)]
		public GridViewVerticalScrollBarStyle VerticalScrollBarStyle {
			get { return (GridViewVerticalScrollBarStyle)GetIntProperty("VerticalScrollBarStyle", (int)GridViewVerticalScrollBarStyle.Standard); }
			set {
				if (value == VerticalScrollBarStyle) return;
				SetIntProperty("VerticalScrollBarStyle", (int)GridViewVerticalScrollBarStyle.Standard, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Status Bar is displayed."), DefaultValue(GridViewStatusBarMode.Auto), NotifyParentProperty(true)]
		public GridViewStatusBarMode ShowStatusBar {
			get { return (GridViewStatusBarMode)GetIntProperty("ShowStatusBar", (int)GridViewStatusBarMode.Auto); }
			set {
				if(value == ShowStatusBar) return;
				SetIntProperty("ShowStatusBar", (int)GridViewStatusBarMode.Auto, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets a value which specifies when the Filter Bar is displayed. "), DefaultValue(GridViewStatusBarMode.Hidden), NotifyParentProperty(true)]
		public GridViewStatusBarMode ShowFilterBar {
			get { return (GridViewStatusBarMode)GetIntProperty("ShowFilterBar", (int)GridViewStatusBarMode.Hidden); }
			set {
				if(value == ShowFilterBar) return;
				SetIntProperty("ShowFilterBar", (int)GridViewStatusBarMode.Hidden, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the grouped columns are displayed within the ASPxGridView. "), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowGroupedColumns {
			get { return GetBoolProperty("ShowGroupedColumns", false); }
			set {
				if(value == ShowGroupedColumns) return;
				SetBoolProperty("ShowGroupedColumns", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text pattern for group rows."), DefaultValue("{0}: {1} {2}"), NotifyParentProperty(true), Localizable(true)]
		public string GroupFormat {
			get { return GetStringProperty("GroupFormat", "{0}: {1} {2}"); }
			set {
				if(value == null) value = string.Empty;
				if(value == GroupFormat) return;
				SetStringProperty("GroupFormat", "{0}: {1} {2}", value);
				OnChanged();
			}
		}
		[Description("Gets or sets the gridline style for the ASPxGridView."), DefaultValue(GridLines.Both), NotifyParentProperty(true)]
		public GridLines GridLines {
			get { return (GridLines)GetIntProperty("GridLines", (int)GridLines.Both); }
			set {
				if(value == GridLines) return;
				SetIntProperty("GridLines", (int)GridLines.Both, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the ASPxGridView's width can be changed by a browser to display the entire content."), DefaultValue(false), NotifyParentProperty(true)]
		public bool UseFixedTableLayout {
			get { return GetBoolProperty("UseFixedTableLayout", false); }
			set {
				if(value == UseFixedTableLayout) return;
				SetBoolProperty("UseFixedTableLayout", false, value);
				OnChanged();
			}
		}
	}
	public enum GridViewEditingMode { Inline, EditForm, EditFormAndDisplayRow, PopupEditForm }
	public class ASPxGridViewEditingSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewEditingSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets a value that specifies the ASPxGridView's editing mode."), DefaultValue(GridViewEditingMode.EditFormAndDisplayRow), NotifyParentProperty(true)]
		public GridViewEditingMode Mode {
			get { return (GridViewEditingMode)GetIntProperty("Mode", (int)GridViewEditingMode.EditFormAndDisplayRow); }
			set {
				if(value == Mode) return;
				SetIntProperty("Mode", (int)GridViewEditingMode.EditFormAndDisplayRow, (int)value);
				OnChanged();
			}
		}
		[Description("Gets or sets the position of the new item row within the ASPxGridView."), DefaultValue(GridViewNewItemRowPosition.Top), NotifyParentProperty(true)]
		public GridViewNewItemRowPosition NewItemRowPosition {
			get { return (GridViewNewItemRowPosition)GetEnumProperty("NewItemRowPosition", GridViewNewItemRowPosition.Top); }
			set {
				if(value == NewItemRowPosition) return;
				SetEnumProperty("NewItemRowPosition", GridViewNewItemRowPosition.Top, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Popup Edit Form's width."), NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupEditFormWidth {
			get { return (Unit)GetObjectProperty("PopupEditFormWidth", Unit.Empty); }
			set {
				if(value.Equals(PopupEditFormWidth)) return;
				SetObjectProperty("PopupEditFormWidth", Unit.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Popup Edit Form's height."), NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupEditFormHeight {
			get { return (Unit)GetObjectProperty("PopupEditFormHeight", Unit.Empty); }
			set {
				if(value.Equals(PopupEditFormHeight)) return;
				SetObjectProperty("PopupEditFormHeight", Unit.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Popup Edit Form's header is displayed."), NotifyParentProperty(true), DefaultValue(true)]
		public bool PopupEditFormShowHeader {
			get { return GetBoolProperty("PopupEditFormShowHeader", true); }
			set {
				if(value == PopupEditFormShowHeader) return;
				SetBoolProperty("PopupEditFormShowHeader", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether an end-user can resize the Popup Edit Form."), NotifyParentProperty(true), DefaultValue(false)]
		public bool PopupEditFormAllowResize {
			get { return GetBoolProperty("PopupEditFormAllowResize", false); }
			set {
				if (value == PopupEditFormAllowResize) return;
				SetBoolProperty("PopupEditFormAllowResize", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the popup edit form's horizaontal alignment."), NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public PopupHorizontalAlign PopupEditFormHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupEditFormHorizontalAlign", PopupHorizontalAlign.RightSides); }
			set {
				if(value == PopupEditFormHorizontalAlign) return;
				SetEnumProperty("PopupEditFormHorizontalAlign", PopupHorizontalAlign.RightSides, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the popup edit form's vertical alignment."), NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.Below)]
		public PopupVerticalAlign PopupEditFormVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupEditFormVerticalAlign", PopupVerticalAlign.Below); }
			set {
				if(value == PopupEditFormVerticalAlign) return;
				SetEnumProperty("PopupEditFormVerticalAlign", PopupVerticalAlign.Below, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the offset from the left or right border of the popup edit form's container."), NotifyParentProperty(true), DefaultValue(0)]
		public int PopupEditFormHorizontalOffset {
			get { return GetIntProperty("PopupEditFormHorizontalOffset", 0); }
			set {
				if(value == PopupEditFormHorizontalOffset) return;
				SetIntProperty("PopupEditFormHorizontalOffset", 0, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the offset from the top or bottom border of the data row currently being edited."), NotifyParentProperty(true), DefaultValue(-1)]
		public int PopupEditFormVerticalOffset {
			get { return GetIntProperty("PopupEditFormVerticalOffset", -1); }
			set {
				if(value == PopupEditFormVerticalOffset) return;
				SetIntProperty("PopupEditFormVerticalOffset", -1, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the Popup Edit Form is displayed as a modal dialog."), NotifyParentProperty(true), DefaultValue(false)]
		public bool PopupEditFormModal {
			get { return GetBoolProperty("PopupEditFormModal", false); }
			set {
				if(value == PopupEditFormModal) return;
				SetBoolProperty("PopupEditFormModal", false, value);
				OnChanged();
			}
		}
		protected internal bool DisplayEditingRow { get { return Mode == GridViewEditingMode.EditFormAndDisplayRow || Mode == GridViewEditingMode.PopupEditForm; } }
		protected internal bool IsEditForm { get { return Mode == GridViewEditingMode.EditForm || Mode == GridViewEditingMode.EditFormAndDisplayRow; } }
		protected internal bool IsInline { get { return Mode == GridViewEditingMode.Inline; } }
		protected internal bool IsPopupEditForm { get { return Mode == GridViewEditingMode.PopupEditForm; } }
		[Description("Gets or sets the maximum number of columns allowed in the Edit Form."), DefaultValue(2), NotifyParentProperty(true)]
		public int EditFormColumnCount {
			get { return GetIntProperty("EditFormColumnCount", 2); }
			set {
				if(value == EditFormColumnCount) return;
				SetIntProperty("EditFormColumnCount", 2, value);
				OnChanged();
			}
		}
	}
	public class ASPxGridViewTextSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewTextSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets the text displayed within the ASPxGridView's Title Panel."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Title {
			get { return GetStringProperty("Title", string.Empty); }
			set {
				if(value == Title) return;
				SetStringProperty("Title", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the ASPxGridView's Group Panel."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string GroupPanel {
			get { return GetStringProperty("GroupPanel", string.Empty); }
			set {
				if(value == GroupPanel) return;
				SetStringProperty("GroupPanel", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Confirm Delete window."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ConfirmDelete {
			get { return GetStringProperty("ConfirmDelete", string.Empty); }
			set {
				if(value == ConfirmDelete) return;
				SetStringProperty("ConfirmDelete", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's caption."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CustomizationWindowCaption {
			get { return GetStringProperty("CustomizationWindowCaption", string.Empty); }
			set {
				if(value == CustomizationWindowCaption) return;
				SetStringProperty("CustomizationWindowCaption", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Popup Edit Form's caption."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string PopupEditFormCaption {
			get { return GetStringProperty("PopupEditFormCaption", string.Empty); }
			set {
				if(value == PopupEditFormCaption) return;
				SetStringProperty("PopupEditFormCaption", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the column header panel when there are no visible columns."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string EmptyHeaders {
			get { return GetStringProperty("EmptyHeaders", string.Empty); }
			set {
				if(value == EmptyHeaders) return;
				SetStringProperty("EmptyHeaders", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within a group row when its child rows are displayed on another page(s)."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string GroupContinuedOnNextPage {
			get { return GetStringProperty("GroupContinuedOnNextPage", string.Empty); }
			set {
				if(value == GroupContinuedOnNextPage) return;
				SetStringProperty("GroupContinuedOnNextPage", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Empty Data Row."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string EmptyDataRow {
			get { return GetStringProperty("DataEmptyRow", string.Empty); }
			set {
				if(value == EmptyDataRow) return;
				SetStringProperty("DataEmptyRow", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Edit command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandEdit {
			get { return GetStringProperty("CommandEdit", string.Empty); }
			set {
				if(value == CommandEdit) return;
				SetStringProperty("CommandEdit", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the New command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandNew {
			get { return GetStringProperty("CommandNew", string.Empty); }
			set {
				if(value == CommandNew) return;
				SetStringProperty("CommandNew", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Delete command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandDelete {
			get { return GetStringProperty("CommandDelete", string.Empty); }
			set {
				if(value == CommandDelete) return;
				SetStringProperty("CommandDelete", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Select command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandSelect {
			get { return GetStringProperty("CommandSelect", string.Empty); }
			set {
				if(value == CommandSelect) return;
				SetStringProperty("CommandSelect", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Cancel command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandCancel {
			get { return GetStringProperty("CommandCancel", string.Empty); }
			set {
				if(value == CommandCancel) return;
				SetStringProperty("CommandCancel", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Update command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandUpdate {
			get { return GetStringProperty("CommandUpdate", string.Empty); }
			set {
				if(value == CommandUpdate) return;
				SetStringProperty("CommandUpdate", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Clear Filter command item."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandClearFilter {
			get { return GetStringProperty("CommandClearFilter", string.Empty); }
			set {
				if(value == CommandClearFilter) return;
				SetStringProperty("CommandClearFilter", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the 'Show All' filter item's caption."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string HeaderFilterShowAll {
			get { return GetStringProperty("HeaderFilterShowAll", string.Empty); }
			set {
				if(value == HeaderFilterShowAll) return;
				SetStringProperty("HeaderFilterShowAll", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Filter Control's caption."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string FilterControlPopupCaption {
			get { return GetStringProperty("FilterControlPopupCaption", string.Empty); }
			set {
				if(value == FilterControlPopupCaption) return;
				SetStringProperty("FilterControlPopupCaption", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the text of the Clear command displayed within the Filter Bar."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string FilterBarClear {
			get { return GetStringProperty("FilterBarClear", string.Empty); }
			set {
				if(value == FilterBarClear) return;
				SetStringProperty("FilterBarClear", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Create Filter command's text displayed within the Filter Bar."), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string FilterBarCreateFilter {
			get { return GetStringProperty("FilterBarCreateFilter", string.Empty); }
			set {
				if(value == FilterBarCreateFilter) return;
				SetStringProperty("FilterBarCreateFilter", string.Empty, value);
				OnChanged();
			}
		}
		protected internal string GetGroupPanel() {
			if(!string.IsNullOrEmpty(GroupPanel)) return GroupPanel;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.GroupPanel);			
		}
		protected internal string GetCustomizationWindowCaption() {
			if(!string.IsNullOrEmpty(CustomizationWindowCaption)) return CustomizationWindowCaption;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CustomizationWindowCaption);			
		}
		protected internal string GetPopupEditFormCaption() {
			if(!string.IsNullOrEmpty(PopupEditFormCaption)) return PopupEditFormCaption;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.PopupEditFormCaption);			
		}
		protected internal string GetConfirmDelete() {
			if(!string.IsNullOrEmpty(ConfirmDelete)) return ConfirmDelete;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ConfirmDelete);			
		}
		protected internal string GetEmptyHeaders() {
			if(!string.IsNullOrEmpty(EmptyHeaders)) return EmptyHeaders;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.EmptyHeaders);			
		}
		protected internal string GetGroupContinuedOnNextPage() {
			if(!string.IsNullOrEmpty(GroupContinuedOnNextPage)) return GroupContinuedOnNextPage;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.GroupContinuedOnNextPage);			
		}
		protected internal string GetEmptyDataRow() {
			if(!string.IsNullOrEmpty(EmptyDataRow)) return EmptyDataRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.EmptyDataRow);			
		}
		protected internal string GetHeaderFilterShowAll() {
			if(!string.IsNullOrEmpty(HeaderFilterShowAll)) return HeaderFilterShowAll;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowAllItem);
		}
		protected internal string GetCommandButtonText(ColumnCommandButtonType buttonType) {
			Dictionary<ColumnCommandButtonType, string> values = new Dictionary<ColumnCommandButtonType, string>();
			values[ColumnCommandButtonType.Edit] = CommandEdit;
			values[ColumnCommandButtonType.New] = CommandNew;
			values[ColumnCommandButtonType.Delete] = CommandDelete;
			values[ColumnCommandButtonType.Select] = CommandSelect;
			values[ColumnCommandButtonType.Cancel] = CommandCancel;
			values[ColumnCommandButtonType.Update] = CommandUpdate;
			values[ColumnCommandButtonType.ClearFilter] = CommandClearFilter;
			if(values.ContainsKey(buttonType) && !string.IsNullOrEmpty(values[buttonType]))
				return values[buttonType];
			return ASPxGridViewTextSettings.GetCommandButtonDefaultText(buttonType);
		}
		protected static internal string GetCommandButtonDefaultText(ColumnCommandButtonType buttonType) {
			switch(buttonType) {
				case ColumnCommandButtonType.Cancel:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandCancel);
				case ColumnCommandButtonType.ClearFilter:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandClearFilter);
				case ColumnCommandButtonType.Delete:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandDelete);
				case ColumnCommandButtonType.Edit:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandEdit);
				case ColumnCommandButtonType.New:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandNew);
				case ColumnCommandButtonType.Select:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandSelect);
				case ColumnCommandButtonType.Update:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandUpdate);
			}
			return string.Empty;
		}
	}
	public class ASPxGridViewCustomizationWindowSettings : ASPxGridViewBaseSettings {
		internal readonly static Unit
			DefaultWidth = 150, DefaultHeight = 170;
		public ASPxGridViewCustomizationWindowSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets whether the Customization Window is enabled."), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				SetBoolProperty("Enabled", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's horizontal position relative to the ASPxGridView's borders."), DefaultValue(PopupHorizontalAlign.RightSides), AutoFormatDisable, NotifyParentProperty(true)]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.RightSides); }
			set {
				SetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.RightSides, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's horizontal offset. "), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int PopupHorizontalOffset {
			get { return (int)GetIntProperty("PopupHorizontalOffset", 0); }
			set {
				SetIntProperty("PopupHorizontalOffset", 0, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's vertical position relative to the ASPxGridView's borders."), DefaultValue(PopupVerticalAlign.BottomSides), AutoFormatDisable, NotifyParentProperty(true)]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.BottomSides); }
			set {
				SetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.BottomSides, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's vertical offset. "), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int PopupVerticalOffset {
			get { return (int)GetIntProperty("PopupVerticalOffset", 0); }
			set {
				SetEnumProperty("PopupVerticalOffset", 0, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the Customization Window's width."), 
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[Description("Gets or sets the Customization Window's height."), 
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
	}
	public enum GridViewLoadingPanelMode { Disabled, ShowAsPopup, ShowOnStatusBar }
	public class ASPxGridViewLoadingPanelSettings : SettingsLoadingPanel {
		private ASPxGridView grid;
		public ASPxGridViewLoadingPanelSettings(ASPxGridView grid)
			: base(grid) {
			this.grid = grid;
		}
		[Description("Gets or sets a value that specifies how a Loading Panel is displayed within the ASPxGridView."),
		DefaultValue(GridViewLoadingPanelMode.ShowAsPopup), AutoFormatDisable, NotifyParentProperty(true)]
		public GridViewLoadingPanelMode Mode {
			get { return (GridViewLoadingPanelMode)GetEnumProperty("Mode", GridViewLoadingPanelMode.ShowAsPopup); }
			set {
				SetEnumProperty("Mode", GridViewLoadingPanelMode.ShowAsPopup, value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ASPxGridViewLoadingPanelSettings src = source as ASPxGridViewLoadingPanelSettings;
			if(src != null) {
				Mode = src.Mode;
			}
		}
		protected override void Changed() {
			this.grid.OnSettingsChanged(this);
		}
	}
	public class ASPxGridViewCookiesSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewCookiesSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets whether cookies are enabled."),
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				SetBoolProperty("Enabled", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the cookie's name (identifier)."),
		DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string CookiesID {
			get { return GetStringProperty("CookiesID", string.Empty); }
			set {
				SetStringProperty("CookiesID", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the version."),
		DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string Version {
			get { return GetStringProperty("Version", string.Empty); }
			set {
				SetStringProperty("Version", string.Empty, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether a cookie contains information on the active page."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StorePaging {
			get { return GetBoolProperty("StorePaging", true); }
			set {
				SetBoolProperty("StorePaging", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether a cookie contains information on sorting and grouping applied to the ASPxGridView."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreGroupingAndSorting {
			get { return GetBoolProperty("StoreGroupingAndSorting", true); }
			set {
				SetBoolProperty("StoreGroupingAndSorting", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether a cookie contains information on the filter criteria applied to the ASPxGridView."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreFiltering {
			get { return GetBoolProperty("StoreFiltering", true); }
			set {
				SetBoolProperty("StoreFiltering", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether a cookie contains information on columns' width."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreColumnsWidth {
			get { return GetBoolProperty("StoreColumnsWidth", true); }
			set {
				SetBoolProperty("StoreColumnsWidth", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether a cookie contains information on columns' visible position."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreColumnsVisiblePosition {
			get { return GetBoolProperty("StoreColumnsVisiblePosition", true); }
			set {
				SetBoolProperty("StoreColumnsVisiblePosition", true, value);
				OnChanged();
			}
		}
	}
	public enum GridViewDetailExportMode { None, Expanded, All }
	public class ASPxGridViewDetailSettings : ASPxGridViewBaseSettings {
		public ASPxGridViewDetailSettings(ASPxGridView grid) : base(grid) { }
		[Description("Gets or sets whether the ASPxGridView can display detail rows."),
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowDetailRow {
			get { return GetBoolProperty("ShowDetailRow", false); }
			set {
				SetBoolProperty("ShowDetailRow", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether the current ASPxGridView is a detail grid."),
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool IsDetailGrid {
			get { return GetBoolProperty("IsDetailGrid", false); }
			set {
				SetBoolProperty("IsDetailGrid", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether detail buttons are displayed."),
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowDetailButtons {
			get { return GetBoolProperty("ShowDetailButtons", true); }
			set {
				SetBoolProperty("ShowDetailButtons", true, value);
				OnChanged();
			}
		}
		[Description("Gets or sets whether several master rows can be expanded simultaneously."),
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool AllowOnlyOneMasterRowExpanded {
			get { return GetBoolProperty("AllowOnlyOneMasterRowExpanded", false); }
			set {
				SetBoolProperty("AllowOnlyOneMasterRowExpanded", false, value);
				OnChanged();
			}
		}
		[Description("Gets or sets which detail rows can be exported."),
		DefaultValue(GridViewDetailExportMode.None), AutoFormatDisable, NotifyParentProperty(true)]
		public GridViewDetailExportMode ExportMode {
			get { return (GridViewDetailExportMode)GetEnumProperty("ExportMode", GridViewDetailExportMode.None); }
			set {
				SetEnumProperty("ExportMode", GridViewDetailExportMode.None, value);
				OnChanged();
			}
		}
		[Description("Gets or sets the detail view's position among other details in a printed document."),
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int ExportIndex {
			get { return GetIntProperty("ExportIndex", 0); }
			set {
				SetIntProperty("ExportIndex", 0, value);
				OnChanged();
			}
		}
	}
}
