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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPager;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPopupControl;
namespace DevExpress.Web.ASPxGridView {
	public class GridViewStyleBase : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class GridViewTableStyle : GridViewStyleBase { }
	public class GridViewCustomizationStyle : GridViewStyleBase { }
	public class GridViewPopupEditFormStyle : GridViewStyleBase { }
	public class GridViewHeaderPanelStyle : GridViewStyleBase { }
	public class GridViewCellStyle : GridViewStyleBase { }
	public class GridViewFooterStyle : GridViewStyleBase { }
	public class GridViewGroupFooterStyle : GridViewStyleBase { }
	public class GridViewEditCellStyle : GridViewStyleBase { }
	public class GridViewFilterCellStyle : GridViewStyleBase { }
	public class GridViewInlineEditRowStyle : GridViewStyleBase { }
	public class GridViewEditFormStyle : GridViewStyleBase { }
	public class GridViewEditFormCaptionStyle : GridViewStyleBase { }
	public class GridViewTitleStyle : GridViewStyleBase { }
	public class GridViewStatusBarStyle : GridViewStyleBase { }
	public class GridViewFilterBarStyle : GridViewStyleBase { }
	public class GridViewEditFormTableStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class GridViewGroupPanelStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class GridViewCommandColumnStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class GridViewHeaderStyle : GridViewStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[Description("Gets or sets the distance between the filter image and the caption within the column header."),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit FilterImageSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
		[Description("Gets or sets the distance between the sorting image and the caption within the column header."),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit SortingImageSpacing {
			get { return ImageSpacing; }
			set { ImageSpacing = value; }
		}
		protected internal Unit GetFilterImageSpacing() {
			return FilterImageSpacing.IsEmpty ? 5 : FilterImageSpacing;
		}
		protected internal Unit GetSortingImageSpacing() {
			return SortingImageSpacing.IsEmpty ? 5 : SortingImageSpacing;
		}
	}
	public class GridViewAlternatingRowStyle : GridViewDataRowStyle {
		[Description("Gets or sets whether the style settings used to paint alternating rows are enabled."),
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatEnable]
		public DefaultBoolean Enabled {
			get { return (DefaultBoolean)ViewStateUtils.GetIntProperty(ViewState, "Enabled", (int)DefaultBoolean.Default); }
			set {
				ViewStateUtils.SetIntProperty(ViewState, "Enabled", (int)DefaultBoolean.Default, (int)value);
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			GridViewAlternatingRowStyle altStyle = style as GridViewAlternatingRowStyle;
			if(altStyle != null) Enabled = altStyle.Enabled;
		}
	}
	public class GridViewRowStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class GridViewGroupRowStyle : GridViewRowStyle {}
	public class GridViewDataRowStyle : GridViewRowStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class GridViewFilterStyle : GridViewStyleBase {}
	public class GridViewStyles : StylesBase {
		public const string CustomizationStyleName = "Customization";
		public const string PopupEditFormStyleName = "PopupEditForm";
		public const string TableStyleName = "Table";
		public const string HeaderStyleName = "Header";
		public const string GroupRowStyleName = "GroupRow";
		public const string FocusedGroupRowStyleName = "FocusedGroupRow";
		public const string DetailRowStyleName = "DetailRow";
		public const string DetailCellStyleName = "DetailCell";
		public const string PreviewRowStyleName = "PreviewRow";
		public const string EmptyDataRowStyleName = "EmptyDataRow";
		public const string RowStyleName = "Row";
		public const string AlternatingRowStyleName = "AlternatingRow";
		public const string SelectedRowStyleName = "SelectedRow";
		public const string FocusedRowStyleName = "FocusedRow";
		public const string FilterRowStyleName = "FilterRow";
		public const string CellStyleName = "Cell";
		public const string FooterStyleName = "Footer";
		public const string GroupFooterStyleName = "GroupFooter";
		public const string GroupPanelStyleName = "GroupPanel";
		public const string HeaderPanelStyleName = "HeaderPanel";
		public const string PagerTopPanelStyleName = "PagerTopPanel";
		public const string PagerBottomPanelStyleName = "PagerBottomPanel";
		public const string DetailButtonStyleName = "DetailButton";
		public const string CustomizationWindowStyleName = "CustomizationWindow";
		public const string CustomizationWindowCloseButtonStyleName = "CustomizationWindowCloseButton";
		public const string CustomizationWindowContentStyleName = "CustomizationWindowContent";
		public const string CustomizationWindowHeaderStyleName = "CustomizationWindowHeader";
		public const string PopupEditFormWindowStyleName = "PopupEditFormWindow";
		public const string PopupEditFormWindowCloseButtonStyleName = "PopupEditFormWindowCloseButton";
		public const string PopupEditFormWindowContentStyleName = "PopupEditFormWindowContent";
		public const string PopupEditFormWindowHeaderStyleName = "PopupEditFormWindowHeader";
		public const string FilterBuilderCloseButtonStyleName = "FilterBuilderCloseButton";
		public const string FilterBuilderHeaderStyleName = "FilterBuilderHeader";
		public const string FilterBuilderMainAreaStyleName = "FilterBuilderMainArea";
		public const string FilterBuilderButtonAreaStyleName = "FilterBuilderButtonArea";
		public const string CommandColumnStyleName = "CommandColumn";
		public const string CommandColumnItemStyleName = "CommandColumnItem";
		public const string FilterCellStyleName = "FilterCell";
		public const string InlineEditRowStyleName = "InlineEditRow";
		public const string EditFormStyleName = "EditForm";
		public const string EditFormDisplayRowStyleName = "EditFormDisplayRow";
		public const string EditingErrorRowStyleName = "EditingErrorRow";
		public const string EditFormTableStyleName = "EditFormTable";
		public const string EditFormColumnCaptionStyleName = "EditFormColumnCaption";
		public const string InlineEditCellStyleName = "InlineEditCell";
		public const string EditFormCellStyleName = "EditFormCell";
		public const string TitlePanelStyleName = "TitlePanel";
		public const string StatusBarStyleName = "StatusBar";
		public const string FilterBarStyleName = "FilterBar";
		public const string FilterBarLinkStyleName = "FilterBarLink";
		public const string FilterBarCheckBoxCellStyleName = "FilterBarCheckBoxCell";		
		public const string FilterBarImageCellStyleName = "FilterBarImageCell";		
		public const string FilterBarExpressionCellStyleName = "FilterBarExpressionCell";
		public const string FilterBarClearButtonCellStyleName = "FilterBarClearButtonCell";
		public const string FilterPopupWindowStyleName = "FilterPopupWindow";
		public const string FilterPopupItemsAreaStyleName = "FilterPopupItemsArea";
		public const string FilterPopupButtonPanelStyleName = "FilterPopupButtonPanel";
		public const string FilterPopupItemStyleName = "FilterPopupItem";
		public const string FilterPopupActiveItemStyleName = "FilterPopupActiveItem";
		public const string FilterPopupSelectedItemStyleName = "FilterPopupSelectedItem";
		public const string
			FilterRowMenuStyleName = "FilterRowMenu",
			FilterRowMenuItemStyleName = "FilterRowMenuItem";
		protected internal const string GridPrefix = "dxgv";
		protected internal const string GridHasTextCellPrefix = "HasTextCell";
		protected internal const string GridIndentCellPrefix = "IndentCell";
		protected internal const string GridLastIndentCellPrefix = "LastIndentCell";
		protected internal const string GridExpandedGroupButtonCellPrefix = "ExpandedGroupButtonCell";
		public GridViewStyles(ASPxGridView grid)
			: base(grid) {
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(CustomizationStyleName, delegate() { return new GridViewCustomizationStyle(); } ));
			list.Add(new StyleInfo(PopupEditFormStyleName, delegate() { return new GridViewPopupEditFormStyle(); } ));
			list.Add(new StyleInfo(TableStyleName, delegate() { return new GridViewTableStyle(); } ));
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new GridViewHeaderStyle(); } ));
			list.Add(new StyleInfo(GroupRowStyleName, delegate() { return new GridViewGroupRowStyle(); } ));
			list.Add(new StyleInfo(FocusedGroupRowStyleName, delegate() { return new GridViewGroupRowStyle(); } ));
			list.Add(new StyleInfo(RowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(DetailRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(DetailCellStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(PreviewRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(EmptyDataRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(AlternatingRowStyleName, delegate() { return new GridViewAlternatingRowStyle(); } ));
			list.Add(new StyleInfo(SelectedRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(FocusedRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(FilterRowStyleName, delegate() { return new GridViewRowStyle(); } ));
			list.Add(new StyleInfo(CellStyleName, delegate() { return new GridViewCellStyle(); } ));
			list.Add(new StyleInfo(FooterStyleName, delegate() { return new GridViewFooterStyle(); } ));
			list.Add(new StyleInfo(GroupFooterStyleName, delegate() { return new GridViewGroupFooterStyle(); } ));
			list.Add(new StyleInfo(GroupPanelStyleName, delegate() { return new GridViewGroupPanelStyle(); } ));
			list.Add(new StyleInfo(HeaderPanelStyleName, delegate() { return new GridViewHeaderPanelStyle(); } ));
			list.Add(new StyleInfo(PagerTopPanelStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(PagerBottomPanelStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(DetailButtonStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(CustomizationWindowStyleName, delegate() { return new AppearanceStyle(); } ));
			list.Add(new StyleInfo(CustomizationWindowCloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); } ));
			list.Add(new StyleInfo(CustomizationWindowContentStyleName, delegate() { return new PopupWindowContentStyle(); } ));
			list.Add(new StyleInfo(CustomizationWindowHeaderStyleName, delegate() { return new PopupWindowStyle(); } ));
			list.Add(new StyleInfo(PopupEditFormWindowStyleName, delegate() { return new AppearanceStyle(); } ));
			list.Add(new StyleInfo(PopupEditFormWindowCloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); } ));
			list.Add(new StyleInfo(PopupEditFormWindowContentStyleName, delegate() { return new PopupWindowContentStyle(); } ));
			list.Add(new StyleInfo(PopupEditFormWindowHeaderStyleName, delegate() { return new PopupWindowStyle(); } ));
			list.Add(new StyleInfo(FilterBuilderCloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));			
			list.Add(new StyleInfo(FilterBuilderHeaderStyleName, delegate() { return new PopupWindowStyle(); }));
			list.Add(new StyleInfo(FilterBuilderMainAreaStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(FilterBuilderButtonAreaStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(CommandColumnStyleName, delegate() { return new GridViewCommandColumnStyle(); } ));
			list.Add(new StyleInfo(CommandColumnItemStyleName, delegate() { return new GridViewCommandColumnStyle(); } ));
			list.Add(new StyleInfo(InlineEditCellStyleName, delegate() { return new GridViewEditCellStyle(); } ));
			list.Add(new StyleInfo(FilterCellStyleName, delegate() { return new GridViewFilterCellStyle(); } ));
			list.Add(new StyleInfo(InlineEditRowStyleName, delegate() { return new GridViewInlineEditRowStyle(); } ));
			list.Add(new StyleInfo(EditFormDisplayRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(EditingErrorRowStyleName, delegate() { return new GridViewRowStyle(); } ));
			list.Add(new StyleInfo(EditFormStyleName, delegate() { return new GridViewEditFormStyle(); } ));
			list.Add(new StyleInfo(EditFormCellStyleName, delegate() { return new GridViewEditCellStyle(); } ));
			list.Add(new StyleInfo(EditFormTableStyleName, delegate() { return new GridViewEditFormTableStyle(); } ));
			list.Add(new StyleInfo(EditFormColumnCaptionStyleName, delegate() { return new GridViewEditFormCaptionStyle(); } ));
			list.Add(new StyleInfo(TitlePanelStyleName, delegate() { return new GridViewTitleStyle(); } ));
			list.Add(new StyleInfo(StatusBarStyleName, delegate() { return new GridViewStatusBarStyle(); } ));
			list.Add(new StyleInfo(FilterBarStyleName, delegate() { return new GridViewFilterBarStyle(); }));
			list.Add(new StyleInfo(FilterBarLinkStyleName, delegate() { return new GridViewFilterBarStyle(); }));
			list.Add(new StyleInfo(FilterBarCheckBoxCellStyleName, delegate() { return new GridViewFilterBarStyle(); }));
			list.Add(new StyleInfo(FilterBarImageCellStyleName, delegate() { return new GridViewFilterBarStyle(); }));			
			list.Add(new StyleInfo(FilterBarExpressionCellStyleName, delegate() { return new GridViewFilterBarStyle(); }));
			list.Add(new StyleInfo(FilterBarClearButtonCellStyleName, delegate() { return new GridViewFilterBarStyle(); }));
			list.Add(new StyleInfo(FilterPopupWindowStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterPopupItemsAreaStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterPopupButtonPanelStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterPopupItemStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterPopupActiveItemStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterPopupSelectedItemStyleName, delegate() { return new GridViewFilterStyle(); }));
			list.Add(new StyleInfo(FilterRowMenuStyleName, delegate() { return new DevExpress.Web.ASPxMenu.MenuStyle(); }));
			list.Add(new StyleInfo(FilterRowMenuItemStyleName, delegate() { return new DevExpress.Web.ASPxMenu.MenuItemStyle(); }));
		}
		protected override string GetCssClassNamePrefix() {
			return GridPrefix;
		}
		protected override bool IsEmptyCssClassName(string cssName) {
			string[] emptyClassNames = new string[] { "FooterCell", "GroupFooterCell", "HeaderHover", "Cell", "FilterCell" };
			foreach(string st in emptyClassNames) {
				if(st == cssName) return true;
			}
			return false;
		}
		[Description("Gets or sets the group button's width."),
		DefaultValue(15), NotifyParentProperty(true), AutoFormatEnable]
		public int GroupButtonWidth {
			get { return GetIntProperty("GroupButtonWidth", 15); }
			set { SetIntProperty("GroupButtonWidth", 15, value); }
		}
		[Description("Gets the style settings used to paint the ASPxGridView when it is disabled."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DisabledStyle Disabled {
			get { return base.Disabled; }
		}
		[Description("Gets the style settings used to paint the customization window."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCustomizationStyle Customization {
			get { return (GridViewCustomizationStyle)GetStyle(CustomizationStyleName); }
		}
		[Description("Gets the style settings used to paint the Popup Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewPopupEditFormStyle PopupEditForm {
			get { return (GridViewPopupEditFormStyle)GetStyle(PopupEditFormStyleName); }
		}
		[Description("Gets the style settings used to paint the table that represents the ASPxGridView."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewTableStyle Table {
			get { return (GridViewTableStyle)GetStyle(TableStyleName); }
		}
		[Description("Gets the style settings used to paint column headers."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewHeaderStyle Header {
			get { return (GridViewHeaderStyle)GetStyle(HeaderStyleName); }
		}
		[Description("Gets the style settings used to paint group rows."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupRowStyle GroupRow {
			get { return (GridViewGroupRowStyle)GetStyle(GroupRowStyleName); }
		}
		[Description("Gets the style settings used to paint the currently focused group row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupRowStyle FocusedGroupRow {
			get { return (GridViewGroupRowStyle)GetStyle(FocusedGroupRowStyleName); }
		}
		[Description("Gets the style settings used to paint data rows."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle Row {
			get { return (GridViewDataRowStyle)GetStyle(RowStyleName); }
		}
		[Description("Gets the style settings used to paint detail rows."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle DetailRow {
			get { return (GridViewDataRowStyle)GetStyle(DetailRowStyleName); }
		}
		[Description("Gets the style settings used to paint the detail row cell."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle DetailCell {
			get { return (GridViewCellStyle)GetStyle(DetailCellStyleName); }
		}
		[Description("Gets the style settings used to paint the preview rows."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle PreviewRow {
			get { return (GridViewDataRowStyle)GetStyle(PreviewRowStyleName); }
		}
		[Description("Gets the style settings used to paint the Empty Data Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle EmptyDataRow {
			get { return (GridViewDataRowStyle)GetStyle(EmptyDataRowStyleName); }
		}
		[Description("Gets the style settings used to paint Alternating Data Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewAlternatingRowStyle AlternatingRow {
			get { return (GridViewAlternatingRowStyle)GetStyle(AlternatingRowStyleName); }
		}
		[Description("Gets the style settings used to paint selected data rows."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle SelectedRow {
			get { return (GridViewDataRowStyle)GetStyle(SelectedRowStyleName); }
		}
		[Description("Gets the style settings used to paint the currently focused data row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle FocusedRow {
			get { return (GridViewDataRowStyle)GetStyle(FocusedRowStyleName); }
		}
		[Description("Gets the style settings used to paint the Auto Filter Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle FilterRow {
			get { return (GridViewRowStyle)GetStyle(FilterRowStyleName); }
		}
		[Description("Gets the style settings used to paint data cells."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle Cell {
			get { return (GridViewCellStyle)GetStyle(CellStyleName); }
		}
		[Description("Gets the style settings used to paint footer cells."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFooterStyle Footer {
			get { return (GridViewFooterStyle)GetStyle(FooterStyleName); }
		}
		[Description("Gets the style settings used to paint group footers."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupFooterStyle GroupFooter {
			get { return (GridViewGroupFooterStyle)GetStyle(GroupFooterStyleName); }
		}
		[Description("Gets the style settings used to paint the Group Panel."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupPanelStyle GroupPanel {
			get { return (GridViewGroupPanelStyle)GetStyle(GroupPanelStyleName); }
		}
		[Description("Gets the style settings used to paint the Header Panel."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewHeaderPanelStyle HeaderPanel {
			get { return (GridViewHeaderPanelStyle)GetStyle(HeaderPanelStyleName); }
		}
		[Description("Gets the style settings used to paint the Pager top panel."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle PagerTopPanel {
			get { return (GridViewCellStyle)GetStyle(PagerTopPanelStyleName); }
		}
		[Description("Gets the style settings used to paint the Pager bottom panel."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle PagerBottomPanel {
			get { return (GridViewCellStyle)GetStyle(PagerBottomPanelStyleName); }
		}
		[Description("Gets the style settings used to paint detail buttons."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle DetailButton {
			get { return (GridViewCellStyle)GetStyle(DetailButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Window."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle CustomizationWindow {
			get { return (AppearanceStyle)GetStyle(CustomizationWindowStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Window's Close button."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle CustomizationWindowCloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CustomizationWindowCloseButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Window's content."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle CustomizationWindowContent {
			get { return (PopupWindowContentStyle)GetStyle(CustomizationWindowContentStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Window's header."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle CustomizationWindowHeader {
			get { return (PopupWindowStyle)GetStyle(CustomizationWindowHeaderStyleName); }
		}
		[Description("Gets the style settings used to paint the Popup Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle PopupEditFormWindow {
			get { return (AppearanceStyle)GetStyle(PopupEditFormWindowStyleName); }
		}
		[Description("Gets the style settings used to paint the Popup Edit Form's Close button."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle PopupEditFormWindowCloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(PopupEditFormWindowCloseButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the Popup Edit Form's content."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle PopupEditFormWindowContent {
			get { return (PopupWindowContentStyle)GetStyle(PopupEditFormWindowContentStyleName); }
		}
		[Description("Gets the style settings used to paint the Popup Edit Form's header."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle PopupEditFormWindowHeader {
			get { return (PopupWindowStyle)GetStyle(PopupEditFormWindowHeaderStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Control's Close button."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle FilterBuilderCloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(FilterBuilderCloseButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Control's header."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle FilterBuilderHeader {
			get { return (PopupWindowStyle)GetStyle(FilterBuilderHeaderStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Control's content area."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle FilterBuilderMainArea {
			get { return (AppearanceStyle)GetStyle(FilterBuilderMainAreaStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Control's footer which displays buttons."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle FilterBuilderButtonArea {
			get { return (AppearanceStyle)GetStyle(FilterBuilderButtonAreaStyleName); }
		}
		[Description("Provides style settings for a loading panel that can be displayed while waiting for a callback response."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanel; }
		}
		[Description("Gets the style settings used to paint a rectangle displayed above the ASPxGridView while waiting for a callback response."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingDivStyle LoadingDiv {
			get { return base.LoadingDiv; }
		}
		[Description("Gets the style settings used to paint the Command Column's cells."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandColumnStyle CommandColumn {
			get { return (GridViewCommandColumnStyle)GetStyle(CommandColumnStyleName); }
		}
		[Description("Gets the style settings used to paint command column items."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandColumnStyle CommandColumnItem {
			get { return (GridViewCommandColumnStyle)GetStyle(CommandColumnItemStyleName); }
		}
		[Description("Gets the style settings used to paint in-line edit row cells."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditCellStyle InlineEditCell {
			get { return (GridViewEditCellStyle)GetStyle(InlineEditCellStyleName); }
		}
		[Description("Gets the style settings used to paint cells within the Auto Filter Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterCellStyle FilterCell {
			get { return (GridViewFilterCellStyle)GetStyle(FilterCellStyleName); }
		}
		[Description("Gets the style settings used to paint the In-Line Edit Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewInlineEditRowStyle InlineEditRow {
			get { return (GridViewInlineEditRowStyle)GetStyle(InlineEditRowStyleName); }
		}
		[Description("Get the style settings used to paint the data row currently being edited."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle EditFormDisplayRow {
			get { return (GridViewDataRowStyle)GetStyle(EditFormDisplayRowStyleName); }
		}
		[Description("Gets the style settings used to paint the Error Row."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle EditingErrorRow {
			get { return (GridViewRowStyle)GetStyle(EditingErrorRowStyleName); }
		}
		[Description("Gets the style settings used to paint the Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormStyle EditForm {
			get { return (GridViewEditFormStyle)GetStyle(EditFormStyleName); }
		}
		[Description("Gets the style settings used to paint edit cells displayed within the Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditCellStyle EditFormCell {
			get { return (GridViewEditCellStyle)GetStyle(EditFormCellStyleName); }
		}
		[Description("Gets the style settings used to paint the table that represents the Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormTableStyle EditFormTable {
			get { return (GridViewEditFormTableStyle)GetStyle(EditFormTableStyleName); }
		}
		[Description("Gets the style settings used to paint the edit cell captions within the Edit Form."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormCaptionStyle EditFormColumnCaption {
			get { return (GridViewEditFormCaptionStyle)GetStyle(EditFormColumnCaptionStyleName); }
		}
		[Description("Gets the style settings used to paint the Title Panel."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewTitleStyle TitlePanel {
			get { return (GridViewTitleStyle)GetStyle(TitlePanelStyleName); }
		}
		[Description("Gets the style settings used to paint the Status Bar."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStatusBarStyle StatusBar {
			get { return (GridViewStatusBarStyle)GetStyle(StatusBarStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Bar."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBar {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarStyleName); }
		}
		[Description(""),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBarLink {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarLinkStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Bar's cell which displays the check box."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBarCheckBoxCell {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarCheckBoxCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Bar's cell which displays the filter image."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBarImageCell {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarImageCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Bar's cell which displays the current filter expression."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBarExpressionCell {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarExpressionCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Filter Bar's cell which displays the Clear button."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterBarStyle FilterBarClearButtonCell {
			get { return (GridViewFilterBarStyle)GetStyle(FilterBarClearButtonCellStyleName); }
		}
		[Description("Gets the style settings used to paint the filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupWindow {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupWindowStyleName); }
		}
		[Description("Gets the style settings used to paint the items area within a filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupItemsArea {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupItemsAreaStyleName); }
		}
		[Description("Gets the style settings used to paint the button panel within a filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupButtonPanel {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupButtonPanelStyleName); }
		}
		[Description("Gets the style settings used to paint the items displayed within a filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupItem {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupItemStyleName); }
		}
		[Description("Gets the style settings used to paint the active item within a filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupActiveItem {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupActiveItemStyleName); }
		}
		[Description("Gets the style settings used to paint the item currently being selected within a filter popup."),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterStyle FilterPopupSelectedItem {
			get { return (GridViewFilterStyle)GetStyle(FilterPopupSelectedItemStyleName); }
		}
		[Description("Gets the style settings used to paint the filter row menu."), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Web.ASPxMenu.MenuStyle FilterRowMenu {
			get { return (DevExpress.Web.ASPxMenu.MenuStyle)GetStyle(FilterRowMenuStyleName); }
		}
		[Description("Gets the style settings used to paint the filter row menu items."), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Web.ASPxMenu.MenuItemStyle FilterRowMenuItem {
			get { return (DevExpress.Web.ASPxMenu.MenuItemStyle)GetStyle(FilterRowMenuItemStyleName); }
		}
	}
	public class GridViewPagerStyles : PagerStyles {
		public GridViewPagerStyles(ASPxGridView grid)
			: base(grid) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class GridViewEditorStyles : EditorStyles {
		public GridViewEditorStyles(ASPxGridView grid)
			: base(grid) {
		}
		public override string ToString() { return string.Empty; }
	}	
}
