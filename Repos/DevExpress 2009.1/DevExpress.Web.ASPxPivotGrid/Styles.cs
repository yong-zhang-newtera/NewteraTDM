#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxPivotGrid                                 }
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
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxPopupControl;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxMenu;
using DevExpress.Web.ASPxPager;
using System.Collections;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridStyles : StylesBase {
		ASPxPivotGrid pivotGrid;
		public const string HeaderStyleName = "Header";
		public const string AreaStyleName = "Area";
		public const string FilterAreaStyleName = "FilterArea";
		public const string FieldValueStyleName = "FieldValue";
		public const string FieldValueTotalStyleName = "FieldValueTotal";
		public const string FieldValueGrandTotalStyleName = "FieldValueGrandTotal";
		public const string CellStyleName = "Cell";
		public const string KPICellStyleName = "KPICell";
		public const string TotalCellStyleName = "TotalCell";
		public const string GrandTotalCellStyleName = "GrandTotalCell";
		public const string CustomTotalCellStyleName = "CustomTotalCell";
		public const string FilterWindowStyleName = "FilterWindow";
		public const string FilterItemsAreaStyleName = "FilterItemsArea";
		public const string FilterButtonStyleName = "FilterButton";
		public const string FilterButtonPanelStyleName = "FilterButtonPanel";
		public const string FilterItemStyleName = "FilterItem";
		public const string MenuItemStyleName = "MenuItem";
		public const string MenuStyleName = "Menu";
		public const string CustomizationFieldsStyleName = "CustomizationFields";
		public const string CustomizationFieldsCloseButtonStyleName = "CustomizationFieldsCloseButton";
		public const string CustomizationFieldsContentStyleName = "CustomizationFieldsContent";
		public const string CustomizationFieldsHeaderStyleName = "CustomizationFieldsHeader";
		public const string PrefilterPanelStyleName = "PrefilterPanel",
			PrefilterPanelLinkStyleName = "PrefilterPanelLinkStyle",
			PrefilterPanelContainerStyleName = "PrefilterPanelContainer",
			PrefilterPanelCheckBoxCellStyleName = "PrefilterPanelCheckBoxCell",
			PrefilterPanelImageCellStyleName = "PrefilterPanelImageCell",
			PrefilterPanelClearButtonCellStyleName = "PrefilterPanelClearButtonCell",
			PrefilterPanelExpressionCellStyleName = "PrefilterPanelExpressionCell";
		public const string PrefilterBuilderCloseButtonStyleName = "FilterBuilderCloseButton",
			PrefilterBuilderHeaderStyleName = "FilterBuilderHeader",
			PrefilterBuilderMainAreaStyleName = "FilterBuilderMainArea",
			PrefilterBuilderButtonAreaStyleName = "FilterBuilderButtonArea";
		protected internal const string PivotGridPrefix = "dxpg";
		protected const string ControlStyleName = "ControlStyle",
			ContainerCellStyleName = "ContainerCell",
			MainTableStyleName = "MainTableStyle",
			ColumnAreaStyleName = "ColumnAreaStyle",
			RowAreaStyleName = "RowAreaStyle",
			DataAreaStyleName = "DataAreaStyle",
			EmptyAreaStyleName = "EmptyAreaStyle",
			ColumnFieldValueStyleName = "ColumnFieldValueStyle",
			RowFieldValueStyleName = "RowFieldValueStyle",
			ColumnTotalFieldValueStyleName = "ColumnTotalFieldValueStyle",
			RowTotalFieldValueStyleName = "RowTotalFieldValueStyle",
			ColumnGrandTotalFieldValueStyleName = "ColumnGrandTotalFieldValueStyle",
			RowGrandTotalFieldValueStyleName = "RowGrandTotalFieldValueStyle",
			CollapsedButtonStyleName = "CollapsedButtonStyle",
			SortByColumnImageStyleName = "SortByColumnImageStyle",
			DataHeadersImageStyleName = "DataHeadersImageStyle",
			HeaderHoverStyleName = "HeaderHoverStyle",
			HeadersTextStyleName = "HeaderTextStyle",
			HeadersGroupButtonStyleName = "HeaderGroupButtonStyle",
			HeadersSortStyleName = "HeaderSortStyle",
			HeadersFilterStyleName = "HeaderFilterStyle",
			HeaderTableStyleName = "HeaderTable",
			TopPagerStyleName = "TopPagerStyle",
			BottomPagerStyleName = "BottomPagerStyle",
			GroupSeparatorName = "GroupSeparator";
		string[] styleNames;
		internal string[] StyleNames {
			get {
				if(styleNames == null) {
					styleNames = new string[] {
						ControlStyleName, MainTableStyleName, ColumnAreaStyleName, RowAreaStyleName, DataAreaStyleName,
						AreaStyleName, EmptyAreaStyleName, FilterAreaStyleName,
						ColumnFieldValueStyleName, RowFieldValueStyleName, ColumnTotalFieldValueStyleName, RowTotalFieldValueStyleName,
						ColumnGrandTotalFieldValueStyleName, RowGrandTotalFieldValueStyleName, CollapsedButtonStyleName,
						CellStyleName, TotalCellStyleName, GrandTotalCellStyleName, 
						DataHeadersImageStyleName, HeaderStyleName, HeaderHoverStyleName,
						HeadersTextStyleName, HeadersGroupButtonStyleName, HeadersSortStyleName, HeadersFilterStyleName,
						HeaderTableStyleName,
						FilterWindowStyleName, FilterItemsAreaStyleName, FilterItemStyleName, FilterButtonStyleName, FilterButtonPanelStyleName,
						TopPagerStyleName, BottomPagerStyleName, CustomizationFieldsHeaderStyleName, CustomizationFieldsContentStyleName,
						MenuItemStyleName, PrefilterPanelStyleName, PrefilterPanelContainerStyleName, 
						PrefilterPanelLinkStyleName, PrefilterBuilderCloseButtonStyleName,
						PrefilterBuilderHeaderStyleName, PrefilterBuilderMainAreaStyleName,
						PrefilterBuilderButtonAreaStyleName, GroupSeparatorName
					};
				}
				return styleNames;
			}
		}
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		[Description("Gets the style settings used to paint the grid's headers (column header area, row header area, data header area)."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotHeaderStyle HeaderStyle {
			get { return (PivotHeaderStyle)GetStyle(HeaderStyleName); }
		}
		[Description("Gets the style settings used to paint the grid's data area."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotAreaStyle AreaStyle {
			get { return (PivotAreaStyle)GetStyle(AreaStyleName); }
		}
		[Description("Gets the style settings used to paint the filter header area."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotAreaStyle FilterAreaStyle {
			get { return (PivotAreaStyle)GetStyle(FilterAreaStyleName); }
		}
		[Description("Gets the style settings used to paint field values."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFieldValueStyle FieldValueStyle {
			get { return (PivotFieldValueStyle)GetStyle(FieldValueStyleName); }
		}
		[Description("Gets the style settings used to paint field values corresponding to totals. "),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFieldValueStyle FieldValueTotalStyle {
			get { return (PivotFieldValueStyle)GetStyle(FieldValueTotalStyleName); }
		}
		[Description("Gets the style settings used to paint field values corresponding to grand totals . "),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFieldValueStyle FieldValueGrandTotalStyle {
			get { return (PivotFieldValueStyle)GetStyle(FieldValueGrandTotalStyleName); }
		}
		[Description("Gets the style settings used to paint cells."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCellStyle CellStyle {
			get { return (PivotCellStyle)GetStyle(CellStyleName); }
		}
		[Description("Gets the style settings used to paint total cells."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCellStyle TotalCellStyle {
			get { return (PivotCellStyle)GetStyle(TotalCellStyleName); }
		}
		[Description("Gets the style settings used to paint grand total cells."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCellStyle GrandTotalCellStyle {
			get { return (PivotCellStyle)GetStyle(GrandTotalCellStyleName); }
		}
		[Description("Gets the style settings used to paint cells within custom totals. "),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCellStyle CustomTotalCellStyle {
			get { return (PivotCellStyle)GetStyle(CustomTotalCellStyleName); }
		}
		[Description("Gets the style settings used to paint the filter window."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFilterStyle FilterWindowStyle {
			get { return (PivotFilterStyle)GetStyle(FilterWindowStyleName); }
		}
		[Description("Gets the style settings used to paint the items area within the filter window."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFilterStyle FilterItemsAreaStyle {
			get { return (PivotFilterStyle)GetStyle(FilterItemsAreaStyleName); }
		}
		[Description("Gets the style settings used to paint items within the filter window."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFilterItemStyle FilterItemStyle {
			get { return (PivotFilterItemStyle)GetStyle(FilterItemStyleName); }
		}
		[Description("Gets the style settings used to paint the filter buttons."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFilterButtonStyle FilterButtonStyle {
			get { return (PivotFilterButtonStyle)GetStyle(FilterButtonStyleName); }
		}
		[Description("Gets the style settings used to paint filter button panels."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotFilterButtonPanelStyle FilterButtonPanelStyle {
			get { return (PivotFilterButtonPanelStyle)GetStyle(FilterButtonPanelStyleName); }
		}
		[Description("Gets the style settings used to paint the context menu's items."),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridMenuItemStyle MenuItemStyle {
			get { return (PivotGridMenuItemStyle)GetStyle(MenuItemStyleName); }
		}
		[Description("Gets the style settings used to paint the context menu."),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridMenuStyle MenuStyle {
			get { return (PivotGridMenuStyle)GetStyle(MenuStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Fields window."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public SerializableStyle CustomizationFieldsStyle {
			get { return (SerializableStyle)GetStyle(CustomizationFieldsStyleName); }
		}
		[Description("Gets the style settings used to paint the close button within the Customization Fields window."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPopupWindowButtonStyle CustomizationFieldsCloseButtonStyle {
			get { return (PivotGridPopupWindowButtonStyle)GetStyle(CustomizationFieldsCloseButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Fields window's content."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPopupWindowContentStyle CustomizationFieldsContentStyle {
			get { return (PivotGridPopupWindowContentStyle)GetStyle(CustomizationFieldsContentStyleName); }
		}
		[Description("Gets the style settings used to paint the Customization Fields window's header."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPopupWindowStyle CustomizationFieldsHeaderStyle {
			get { return (PivotGridPopupWindowStyle)GetStyle(CustomizationFieldsHeaderStyleName); }
		}
		[Description("Gets the style settings used to paint the Prefilter panel."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelStyleName); }
		}
		[Description("Gets the style settings used to paint links (filter expression, clear filter command) displayed within the Prefilter panel."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelLinkStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelLinkStyleName); }
		}
		[Description("Gets the style settings used to paint the Prefilter panel's cell which displays the check box."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelCheckBoxCellStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelCheckBoxCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Prefilter panel's cell that displays the Clear button."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelClearButtonCellStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelClearButtonCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Prefilter panel's cell which displays the current filter expression."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelExpressionCellStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelExpressionCellStyleName); }
		}
		[Description("Gets the style settings used to paint the Prefilter panel's cell which displays the filter image."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridPrefilterPanelStyle PrefilterPanelImageCellStyle {
			get { return (PivotGridPrefilterPanelStyle)GetStyle(PrefilterPanelImageCellStyleName); }
		}
		[Description("Gets the style settings used to paint the prefilter's Close button."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PopupWindowButtonStyle PrefilterBuilderCloseButtonStyle {
			get { return (PopupWindowButtonStyle)GetStyle(PrefilterBuilderCloseButtonStyleName); }
		}
		[Description("Gets the style settings used to paint the prefilter's header."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PopupWindowStyle PrefilterBuilderHeaderStyle {
			get { return (PopupWindowStyle)GetStyle(PrefilterBuilderHeaderStyleName); }
		}
		[Description("Gets the style settings used to paint a prefilter's footer that displays buttons."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public SerializableStyle PrefilterBuilderButtonAreaStyle {
			get { return (SerializableStyle)GetStyle(PrefilterBuilderButtonAreaStyleName); }
		}
		[Description("Gets the style settings used to paint the prefilter's content area."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public SerializableStyle PrefilterBuilderMainAreaStyle {
			get { return (SerializableStyle)GetStyle(PrefilterBuilderMainAreaStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public new PivotGridLoadingPanelStyle LoadingPanel {
			get { return (PivotGridLoadingPanelStyle)base.LoadingPanel; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public new PivotGridLoadingDivStyle LoadingDiv {
			get { return (PivotGridLoadingDivStyle)base.LoadingDiv; }
		}
		public PivotGridStyles(ASPxPivotGrid pivotGrid)
			: base(pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			list.Add(new StyleInfo(LoadingPanelStyleName, typeof(PivotGridLoadingPanelStyle)));
			list.Add(new StyleInfo(LoadingDivStyleName, typeof(PivotGridLoadingDivStyle)));
			list.Add(new StyleInfo(HeaderStyleName, typeof(PivotHeaderStyle)));
			list.Add(new StyleInfo(AreaStyleName, typeof(PivotAreaStyle)));
			list.Add(new StyleInfo(FilterAreaStyleName, typeof(PivotAreaStyle)));
			list.Add(new StyleInfo(FieldValueStyleName, typeof(PivotFieldValueStyle)));
			list.Add(new StyleInfo(FieldValueTotalStyleName, typeof(PivotFieldValueStyle)));
			list.Add(new StyleInfo(FieldValueGrandTotalStyleName, typeof(PivotFieldValueStyle)));
			list.Add(new StyleInfo(CellStyleName, typeof(PivotCellStyle)));
			list.Add(new StyleInfo(TotalCellStyleName, typeof(PivotCellStyle)));
			list.Add(new StyleInfo(GrandTotalCellStyleName, typeof(PivotCellStyle)));
			list.Add(new StyleInfo(CustomTotalCellStyleName, typeof(PivotCellStyle)));
			list.Add(new StyleInfo(FilterWindowStyleName, typeof(PivotFilterStyle)));
			list.Add(new StyleInfo(FilterItemsAreaStyleName, typeof(PivotFilterStyle)));
			list.Add(new StyleInfo(FilterItemStyleName, typeof(PivotFilterItemStyle)));
			list.Add(new StyleInfo(FilterButtonStyleName, typeof(PivotFilterButtonStyle)));
			list.Add(new StyleInfo(FilterButtonPanelStyleName, typeof(PivotFilterButtonPanelStyle)));
			list.Add(new StyleInfo(MenuItemStyleName, typeof(PivotGridMenuItemStyle)));
			list.Add(new StyleInfo(MenuStyleName, typeof(PivotGridMenuStyle)));
			list.Add(new StyleInfo(CustomizationFieldsStyleName, typeof(SerializableStyle)));
			list.Add(new StyleInfo(CustomizationFieldsCloseButtonStyleName, typeof(PivotGridPopupWindowButtonStyle)));
			list.Add(new StyleInfo(CustomizationFieldsContentStyleName, typeof(PivotGridPopupWindowContentStyle)));
			list.Add(new StyleInfo(CustomizationFieldsHeaderStyleName, typeof(PivotGridPopupWindowStyle)));
			list.Add(new StyleInfo(PrefilterPanelStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterPanelCheckBoxCellStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterPanelClearButtonCellStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterPanelExpressionCellStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterPanelImageCellStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterPanelLinkStyleName, typeof(PivotGridPrefilterPanelStyle)));
			list.Add(new StyleInfo(PrefilterBuilderCloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(PrefilterBuilderHeaderStyleName, delegate() { return new PopupWindowStyle(); }));
			list.Add(new StyleInfo(PrefilterBuilderMainAreaStyleName, delegate() { return new SerializableStyle(); }));
			list.Add(new StyleInfo(PrefilterBuilderButtonAreaStyleName, delegate() { return new SerializableStyle(); }));
		}
		protected override string GetCssClassNamePrefix() {
			return "dxpg";
		}
		internal PivotHeaderStyle GetDefaultHeaderHoverStyle() {
			PivotHeaderStyle style = new PivotHeaderStyle();
			style.CopyFrom(CreateStyleByName(HeaderHoverStyleName));
			return style;
		}
		internal void ApplyDefaultHeaderStyle(PivotHeaderStyle style) {
			ApplyControlStyle(style);
			style.CopyFrom(CreateStyleByName(HeaderStyleName));
		}
		internal void ApplyHeaderTableStyle(PivotHeaderStyle style) {
			style.CopyFrom(CreateStyleByName(HeaderTableStyleName));
		}
		internal void ApplyHeaderTextStyle(PivotHeaderStyle style) {
			style.CopyFrom(CreateStyleByName(HeadersTextStyleName));
		}
		internal void ApplyHeaderGroupButtonStyle(PivotHeaderStyle style) {
			style.CopyFrom(CreateStyleByName(HeadersGroupButtonStyleName));
		}
		internal void ApplyHeaderSortStyle(PivotHeaderStyle style) {
			style.CopyFrom(CreateStyleByName(HeadersSortStyleName));
		}
		internal void ApplyHeaderFilterStyle(PivotHeaderStyle style) {
			style.CopyFrom(CreateStyleByName(HeadersFilterStyleName));
		}
		internal void ApplyControlStyle(AppearanceStyle style) {
			style.CopyFrom(CreateStyleByName(ControlStyleName));
		}
		internal AppearanceStyle GetMainTableStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(MainTableStyleName));
			return style;
		}
		internal PivotAreaStyle GetAreaStyle(PivotArea area) {
			PivotAreaStyle style = new PivotAreaStyle();
			style.CopyFrom(CreateStyleByName(AreaStyleName));
			if(area == PivotArea.FilterArea) style.CopyFrom(CreateStyleByName(FilterAreaStyleName));
			if(area == PivotArea.ColumnArea) style.CopyFrom(CreateStyleByName(ColumnAreaStyleName));
			if(area == PivotArea.RowArea) style.CopyFrom(CreateStyleByName(RowAreaStyleName));
			if(area == PivotArea.DataArea) style.CopyFrom(CreateStyleByName(DataAreaStyleName));
			style.CopyFrom(AreaStyle);
			if(area == PivotArea.FilterArea) style.CopyFrom(FilterAreaStyle);
			return style;		
		}
		internal void ApplyEmptyAreaStyle(PivotAreaStyle style) {
			style.CopyFrom(CreateStyleByName(EmptyAreaStyleName));
		}
		internal void ApplyFieldValueStyle(PivotFieldValueStyle style, bool isColumn, PivotGridField field) {
			style.CopyFrom(isColumn ? CreateStyleByName(ColumnFieldValueStyleName) : CreateStyleByName(RowFieldValueStyleName));
			style.CopyFrom(FieldValueStyle);
			if(field != null) style.CopyFrom(field.ValueStyle);
		}
		internal void ApplyTotalFieldValueStyle(PivotFieldValueStyle style, bool isColumn, PivotGridField field) {
			style.CopyFrom(isColumn ? CreateStyleByName(ColumnTotalFieldValueStyleName) : CreateStyleByName(RowTotalFieldValueStyleName));
			style.CopyFrom(FieldValueTotalStyle);
			if(field != null) style.CopyFrom(field.ValueTotalStyle);
		}
		internal void ApplyGrandTotalFieldValueStyle(PivotFieldValueStyle style, bool isColumn) {
			style.CopyFrom(isColumn ? CreateStyleByName(ColumnGrandTotalFieldValueStyleName) : CreateStyleByName(RowGrandTotalFieldValueStyleName));
			style.CopyFrom(FieldValueGrandTotalStyle);
		}
		internal void ApplyCollapsedButtonStyle(AppearanceStyle style) {
			style.CopyFrom(CreateStyleByName(CollapsedButtonStyleName));		
		}
		internal void ApplySortByColumnImageStyle(AppearanceStyle style) {
			style.CopyFrom(CreateStyleByName(SortByColumnImageStyleName));
		}
		internal void ApplyCellStyle(bool leftMost, bool topMost, PivotCellStyle style, bool showKPIGraphic) {
			style.CopyFrom(CreateStyleByName(CellStyleName));
			if(showKPIGraphic) 
				style.CopyFrom(CreateStyleByName(KPICellStyleName));
			style.CopyFrom(CellStyle);
			if(leftMost)
				style.BorderLeft.BorderStyle = BorderStyle.None;
			if(topMost)
				style.BorderTop.BorderStyle = BorderStyle.None;
		}
		internal void ApplyTotalCellStyle(PivotCellStyle style) {
			style.CopyFrom(CreateStyleByName(TotalCellStyleName));
			style.CopyFrom(TotalCellStyle);
		}
		internal void ApplyCustomTotalCellStyle(PivotCellStyle style) {
			style.CopyFrom(CreateStyleByName(TotalCellStyleName));
			style.CopyFrom(CustomTotalCellStyle);
		}
		internal void ApplyGrandTotalCellStyle(PivotCellStyle style) {
			style.CopyFrom(CreateStyleByName(GrandTotalCellStyleName));
			style.CopyFrom(GrandTotalCellStyle);
		}
		internal void ApplyDataHeadersImageStyle(System.Web.UI.WebControls.Image image) {
			CreateStyleByName(DataHeadersImageStyleName).AssignToControl(image);
		}
		internal void ApplyContainerCellStyle(WebControl control) {
			CreateStyleByName(ContainerCellStyleName).AssignToControl(control);
		}
		internal void ApplyGroupSeparatorStyle(System.Web.UI.WebControls.Image image) {
			CreateStyleByName(GroupSeparatorName).AssignToControl(image);
		}
		internal SerializableStyle GetPrefilterPanelStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelStyleName));
			style.CopyFrom(PrefilterPanelStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterPanelLinkStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelLinkStyleName));
			style.CopyFrom(PrefilterPanelLinkStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterPanelContainerStyle() {
			SerializableStyle style = new SerializableStyle();
			ApplyControlStyle(style);
			style.CopyFrom(CreateStyleByName(PrefilterPanelContainerStyleName));
			return style;
		}
		internal SerializableStyle GetPrefilterPanelCheckBoxCellStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelCheckBoxCellStyleName));
			style.CopyFrom(PrefilterPanelCheckBoxCellStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterPanelImageCellStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelImageCellStyleName));
			style.CopyFrom(PrefilterPanelImageCellStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterPanelClearButtonCellStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelClearButtonCellStyleName));
			style.CopyFrom(PrefilterPanelClearButtonCellStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterPanelExpressionCellStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterPanelExpressionCellStyleName));
			style.CopyFrom(PrefilterPanelExpressionCellStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterBuilderButtonAreaStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterBuilderButtonAreaStyleName));
			style.CopyFrom(PrefilterBuilderButtonAreaStyle);
			return style;
		}
		internal SerializableStyle GetPrefilterBuilderMainAreaStyle() {
			SerializableStyle style = new SerializableStyle();
			style.CopyFrom(CreateStyleByName(PrefilterBuilderMainAreaStyleName));
			style.CopyFrom(PrefilterBuilderMainAreaStyle);
			return style;
		}
		internal Paddings GetAreaPaddings(PivotArea area, bool isFirst, bool isLast) {			
			Unit padding = 6,
				halfPadding = 3;
			switch(area) {
				case PivotArea.ColumnArea:
					return new Paddings(0, padding, padding, padding);
				case PivotArea.RowArea:
					return new Paddings(isFirst ? padding : halfPadding, padding, isLast ? padding : halfPadding, padding);
				default:
					return new Paddings(isFirst ? padding : 0, padding, padding, padding);
			}
		}
		internal Paddings GetFilterButtonPanelPaddings(FontInfo font) {
			Unit padding = UnitUtils.GetFontDependentSize(7, font);
			return new Paddings(padding, padding, 0, padding);
		}
		internal Unit GetFilterButtonPanelSpacing(FontInfo font) {
			return UnitUtils.GetFontDependentSize(4, font);
		}
		internal PivotFilterStyle GetFilterWindowStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(CreateStyleByName(FilterWindowStyleName));
			return style;
		}
		internal PivotFilterStyle GetFilterItemsAreaStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(CreateStyleByName(FilterItemsAreaStyleName));
			return style;
		}
		internal PivotFilterItemStyle GetFilterItemStyle() {
			PivotFilterItemStyle style = new PivotFilterItemStyle();
			style.CopyFrom(CreateStyleByName(FilterItemStyleName));
			return style;
		}
		internal PivotFilterButtonStyle GetFilterButtonStyle() {
			PivotFilterButtonStyle style = new PivotFilterButtonStyle();
			style.CopyFrom(CreateStyleByName(FilterButtonStyleName));
			return style;
		}
		internal PivotFilterButtonPanelStyle GetFilterButtonPanelStyle() {
			PivotFilterButtonPanelStyle style = new PivotFilterButtonPanelStyle();
			style.CopyFrom(CreateStyleByName(FilterButtonPanelStyleName));
			return style;
		}
		internal Paddings GetCustomizationFieldsPaddings(FontInfo font) {
			Unit padding = UnitUtils.GetFontDependentSize(3, font);
			return new Paddings(padding, padding, padding, 0);
		}
		internal Paddings GetCustomizationFieldsCaptionPaddings(FontInfo font) {
			Unit padding = UnitUtils.GetFontDependentSize(2, font);
			Unit left = UnitUtils.GetFontDependentSize(6, font);
			return new Paddings(left, padding, padding, padding);
		}
		internal AppearanceStyle GetPagerStyle(bool isTopPager) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(isTopPager ? TopPagerStyleName : BottomPagerStyleName));
			return style;
		}
		internal PopupWindowStyle GetCustomizationFieldsHeaderStyle() {
			PopupWindowStyle style = new PopupWindowStyle();
			style.CopyFrom(CreateStyleByName(CustomizationFieldsHeaderStyleName));
			style.BackgroundImage.ImageUrl = PivotGrid.RenderHelper.GetCustomizationFieldsBackgroundImage().Url;
			style.CopyFrom(CustomizationFieldsHeaderStyle);
			return style;
		}
		internal PopupWindowContentStyle GetCustomizationFieldsContentStyle() {
			PopupWindowContentStyle style = new PopupWindowContentStyle();
			style.CopyFrom(CreateStyleByName(CustomizationFieldsContentStyleName));
			style.CopyFrom(CustomizationFieldsContentStyle);
			return style;
		}
		internal DevExpress.Web.ASPxMenu.MenuItemStyle GetMenuItemStyle() {
			DevExpress.Web.ASPxMenu.MenuItemStyle style = new DevExpress.Web.ASPxMenu.MenuItemStyle();
			style.CopyFrom(CreateStyleByName(MenuItemStyleName));
			style.CopyFrom(MenuItemStyle);
			return style;
		}
		internal void AppendDefaultDXClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, PivotGridPrefix);
		}
	}
	public class SerializableStyle : AppearanceStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotFieldValueStyle : SerializableStyle {
		[Category("Layout"), DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable,
		Description("Gets or sets whether row values are top-aligned.")]
		[XtraSerializableProperty()]
		public bool TopAlignedRowValues {
			get { return ViewStateUtils.GetBoolProperty(ReadOnlyViewState, "TopAlignedRowValues", false); }
			set { ViewStateUtils.SetBoolProperty(ViewState, "TopAlignedRowValues", false, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class PivotCellStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotAreaStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotHeaderStyle : SerializableStyle {
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
		[Description("This property is not in effect for the PivotHeaderStyle class."),
		NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable()]
		public Unit FilterImageSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
		[Description("This property is not in effect for the PivotHeaderStyle class."),
		NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable()]
		public Unit SortingImageSpacing {
			get { return ImageSpacing; }
			set { ImageSpacing = value; }
		}
	}
	public class PivotFilterStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotFilterItemStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotFilterButtonStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotFilterButtonPanelStyle : SerializableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable()]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
	}
	public class PivotGridMenuItemStyle : DevExpress.Web.ASPxMenu.MenuItemStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridMenuStyle : MenuStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridLoadingPanelStyle : LoadingPanelStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridLoadingDivStyle : LoadingDivStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridPopupWindowButtonStyle : PopupWindowButtonStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridPopupWindowContentStyle : PopupWindowContentStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotGridPopupWindowStyle : PopupWindowStyle, IXtraSerializable2 {
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class CssStyleCollectionHelper {
		public static bool ContainsKey(CssStyleCollection collection, string needle) {
			foreach(object key in collection.Keys)
				if(key as string == needle) return true;
			return false;
		}
	}
	public class StateManagerSerializeHelper {
		class IndexedStringConverter : DevExpress.Utils.Serializing.Helpers.IOneTypeObjectConverter {
			public Type Type { get { return typeof(IndexedString); } }
			public object FromString(string str) {
				return new IndexedString(str);
			}
			public string ToString(object obj) {
				return ((IndexedString)obj).Value;
			}
		}
		static StateManagerSerializeHelper() {
			DevExpress.Utils.Serializing.Helpers.ObjectConverter.Instance.RegisterConverter(new IndexedStringConverter());
		}
		public static void DeserializeObject(object obj, IXtraPropertyCollection list) {
			if(list.Count > 0) {
				for(int i = 0; i < list.Count; i++) {
					if(((XtraPropertyInfo)list[i]).Name == "ViewState") {
						XtraPropertyInfo viewStateInfo = (XtraPropertyInfo)list[i];
						IStateManager style = (IStateManager)obj;
						style.LoadViewState(viewStateInfo.ValueToObject(typeof(object[])));
					}
				}
			}
		}
		static void DeserializeProperty(object obj, PropertyDescriptor prop, XtraPropertyInfo info) {
			if(info.IsKey) {
				DeserializeObject(prop.GetValue(obj), info.ChildProperties);
			} else
				prop.SetValue(obj, info.ValueToObject(prop.PropertyType));
		}
		public static XtraPropertyInfo[] SerializeObject(object obj) {
			IStateManager stateManager = (IStateManager)obj;
			object state = stateManager.SaveViewState();
			return new XtraPropertyInfo[] { new XtraPropertyInfo("ViewState", state != null ? state.GetType() : typeof(object[]), state) };
		}
		static void SerializeProperty(object obj, PropertyDescriptor prop, List<XtraPropertyInfo> array) {
			if(prop.SerializationVisibility == DesignerSerializationVisibility.Content ||
				((PersistenceModeAttribute)prop.Attributes[typeof(PersistenceModeAttribute)]).Mode == PersistenceMode.InnerProperty) {
				SerializeContent(obj, prop, array);
				return;
			}
			if(prop.SerializationVisibility == DesignerSerializationVisibility.Visible) {
				array.Add(new XtraPropertyInfo(prop.Name, prop.PropertyType, prop.GetValue(obj)));
				return;
			}
		}
		private static void SerializeContent(object obj, PropertyDescriptor prop, List<XtraPropertyInfo> array) {
			XtraPropertyInfo info = new XtraPropertyInfo(prop.Name, prop.PropertyType, null, true);
			info.ChildProperties.AddRange(SerializeObject(prop.GetValue(obj)));
			array.Add(info);
		}
	}
	public class PivotGridPagerStyles : PagerStyles {
		public PivotGridPagerStyles(ASPxPivotGrid grid)
			: base(grid) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class PivotGridPrefilterPanelStyle : SerializableStyle { }
}
