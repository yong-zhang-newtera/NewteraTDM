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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridAppearancesBase : BaseAppearanceCollection {
		protected const string FieldHeaderAppearanceName = "FieldHeader";
		protected const string CellAppearanceName = "Cell";
		protected const string TotalCellAppearanceName = "TotalCell";
		protected const string GrandTotalCellAppearanceName = "GrandTotalCell";
		protected const string CustomTotalCellAppearanceName = "CustomTotalCell";
		protected const string LinesAppearanceName = "Lines";
		protected const string FilterSeparatorAppearanceName = "FilterSeparator";
		protected const string FieldValueAppearanceName = "FieldValue";
		protected const string FieldValueTotalAppearanceName = "FieldValueTotal";
		protected const string FieldValueGrandTotalAppearanceName = "FieldValueGrandTotal";
		protected const string HeaderGroupLineAppearanceName = "HeaderGroupLine";
		AppearanceObject fieldHeader, cell, totalCell, grandTotalCell, customTotalCell,
			fieldValue, fieldValueTotal, fieldValueGrandTotal, lines, filterSeparator,
			headerGroupLine;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.fieldHeader = CreateAppearance(FieldHeaderAppearanceName);
			this.cell = CreateAppearance(CellAppearanceName);
			this.totalCell = CreateAppearance(TotalCellAppearanceName);
			this.grandTotalCell = CreateAppearance(GrandTotalCellAppearanceName);
			this.customTotalCell = CreateAppearance(CustomTotalCellAppearanceName);
			this.fieldValue = CreateAppearance(FieldValueAppearanceName);
			this.fieldValueTotal = CreateAppearance(FieldValueTotalAppearanceName);
			this.fieldValueGrandTotal = CreateAppearance(FieldValueGrandTotalAppearanceName);
			this.lines = CreateAppearance(LinesAppearanceName); 
			this.filterSeparator = CreateAppearance(FilterSeparatorAppearanceName);
			this.headerGroupLine = CreateAppearance(HeaderGroupLineAppearanceName);
		}
		bool ShouldSerializeCell() { return Cell.ShouldSerialize(); }
		void ResetCell() { Cell.Reset(); }
		[
		Description("Gets the appearance settings used to paint data cells."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.Cell")
		]
		public AppearanceObject Cell { get { return cell; } }
		bool ShouldSerializeFieldHeader() { return FieldHeader.ShouldSerialize(); }
		void ResetFieldHeader() { FieldHeader.Reset(); }
		[
		Description("Gets the appearance settings used to paint field headers."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldHeader")
		]
		public AppearanceObject FieldHeader { get { return fieldHeader; } }
		bool ShouldSerializeTotalCell() { return TotalCell.ShouldSerialize(); }
		void ResetTotalCell() { TotalCell.Reset(); }
		[
		Description("Gets the appearance settings used to paint automatic total cells."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.TotalCell")
		]
		public AppearanceObject TotalCell { get { return totalCell; } }
		bool ShouldSerializeGrandTotalCell() { return GrandTotalCell.ShouldSerialize(); }
		void ResetGrandTotalCell() { GrandTotalCell.Reset(); }
		[
		Description("Gets the appearance settings used to paint Grand Total cells."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.GrandTotalCell")
		]
		public AppearanceObject GrandTotalCell { get { return grandTotalCell; } }
		bool ShouldSerializeCustomTotalCell() { return CustomTotalCell.ShouldSerialize(); }
		void ResetCustomTotalCell() { CustomTotalCell.Reset(); }
		[
		Description("Gets the appearance settings used to paint custom total cells."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.CustomTotalCell")
		]
		public AppearanceObject CustomTotalCell { get { return customTotalCell; } }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		void ResetFieldValue() { FieldValue.Reset(); }
		[
		Description("Gets the appearance settings used to paint the values of fields and the default appearance settings used to paint grand totals and totals."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValue")
		]
		public AppearanceObject FieldValue { get { return fieldValue; } }
		bool ShouldSerializeFieldValueTotal() { return FieldValueTotal.ShouldSerialize(); }
		void ResetFieldValueTotal() { FieldValueTotal.Reset(); }
		[
		Description("Gets the appearance settings used to paint the headers of Totals."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValueTotal")
		]
		public AppearanceObject FieldValueTotal { get { return fieldValueTotal; } }
		bool ShouldSerializeFieldValueGrandTotal() { return FieldValueGrandTotal.ShouldSerialize(); }
		void ResetFieldValueGrandTotal() { FieldValueGrandTotal.Reset(); }
		[
		Description("Gets the appearance settings used to paint grand total headers."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValueGrandTotal")
		]
		public AppearanceObject FieldValueGrandTotal { get { return fieldValueGrandTotal; } }
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		void ResetLines() { Lines.Reset(); }
		[
		Description("Gets the appearance settings used to paint the horizontal and vertical lines."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.Lines")
		]
		public AppearanceObject Lines { get { return lines; } }
		bool ShouldSerializeFilterSeparator() { return FilterSeparator.ShouldSerialize(); }
		void ResetFilterSeparator() { FilterSeparator.Reset(); }
		[
		Description("Gets the appearance settings used to paint the filter header area separator."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FilterSeparator")
		]
		public AppearanceObject FilterSeparator { get { return filterSeparator; } }
		bool ShouldSerializeHeaderGroupLine() { return HeaderGroupLine.ShouldSerialize(); }
		void ResetHeaderGroupLine() { HeaderGroupLine.Reset(); }
		[
		Description("Gets the appearance settings used to paint connector lines between field headers combined in a field group."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.HeaderGroupLine")
		]
		public AppearanceObject HeaderGroupLine { get { return headerGroupLine; } }
	}
	public class PivotGridAppearances : PivotGridAppearancesBase {
		protected const string FocusedCellAppearanceName = "FocusedCell";
		protected const string SelectedCellAppearanceName = "SelectedCell";
		protected const string HeaderAreaAppearanceName = "HeaderArea";
		protected const string ColumnHeaderAreaAppearanceName = "ColumnHeaderArea";
		protected const string RowHeaderAreaAppearanceName = "RowHeaderArea";
		protected const string FilterHeaderAreaAppearanceName = "FilterHeaderArea";
		protected const string DataHeaderAreaAppearanceName = "DataHeaderArea";
		protected const string EmptyAppearanceName = "Empty";
		protected const string ExpandButtonAppearanceName = "ExpandButton";
		protected const string HeaderFilterButtonAppearanceName = "HeaderFilterButton";
		protected const string HeaderFilterButtonActiveAppearanceName = "HeaderFilterButtonActive";
		protected const string PrefilterPanelName = "FilterPanel";
		AppearanceObject focusedCell, selectedCell, headerArea, columnHeaderArea, 
			rowHeaderArea, filterHeaderArea, dataHeaderArea, empty, drillDownButton, 
			headerFilterButton, headerFilterButtonActive, prefilterPanel;
		Image sortByColumnIndicatorImage;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.focusedCell = CreateAppearance(FocusedCellAppearanceName);
			this.selectedCell = CreateAppearance(SelectedCellAppearanceName);
			this.headerArea = CreateAppearance(HeaderAreaAppearanceName);
			this.columnHeaderArea = CreateAppearance(ColumnHeaderAreaAppearanceName);
			this.rowHeaderArea = CreateAppearance(RowHeaderAreaAppearanceName);
			this.filterHeaderArea = CreateAppearance(FilterHeaderAreaAppearanceName);
			this.dataHeaderArea = CreateAppearance(DataHeaderAreaAppearanceName);
			this.empty = CreateAppearance(EmptyAppearanceName);
			this.drillDownButton = CreateAppearance(ExpandButtonAppearanceName);
			this.headerFilterButton = CreateAppearance(HeaderFilterButtonAppearanceName);
			this.headerFilterButtonActive = CreateAppearance(HeaderFilterButtonActiveAppearanceName);
			this.prefilterPanel = CreateAppearance(PrefilterPanelName);
			this.sortByColumnIndicatorImage = null;
		}
		bool ShouldSerializeFocusedCell() { return FocusedCell.ShouldSerialize(); }
		void ResetFocusedCell() { FocusedCell.Reset(); }
		[
		Description("Gets the appearance settings used to paint the currently focused cell."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.FocusedCell")
		]
		public AppearanceObject FocusedCell { get { return focusedCell; } }
		bool ShouldSerializeSelectedCell() { return SelectedCell.ShouldSerialize(); }
		void ResetSelectedCell() { SelectedCell.Reset(); }
		[
		Description("Gets the appearance settings used to paint the selected cells."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.SelectedCell")
		]
		public AppearanceObject SelectedCell { get { return selectedCell; } }
		bool ShouldSerializeHeaderArea() { return HeaderArea.ShouldSerialize(); }
		void ResetHeaderArea() { HeaderArea.Reset(); }
		[
		Description("Gets the appearance settings used to paint the header area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderArea")
		]
		public AppearanceObject HeaderArea { get { return headerArea; } }
		bool ShouldSerializeColumnHeaderArea() { return ColumnHeaderArea.ShouldSerialize(); }
		void ResetColumnHeaderArea() { ColumnHeaderArea.Reset(); }
		[
		Description("Gets the appearance settings used to paint the column header area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.ColumnHeaderArea")
		]
		public AppearanceObject ColumnHeaderArea { get { return columnHeaderArea; } }
		bool ShouldSerializeRowHeaderArea() { return RowHeaderArea.ShouldSerialize(); }
		void ResetRowHeaderArea() { RowHeaderArea.Reset(); }
		[
		Description("Gets the appearance settings used to paint the row header area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.RowHeaderArea")
		]
		public AppearanceObject RowHeaderArea { get { return rowHeaderArea; } }
		bool ShouldSerializeFilterHeaderArea() { return FilterHeaderArea.ShouldSerialize(); }
		void ResetFilterHeaderArea() { FilterHeaderArea.Reset(); }
		[
		Description("Gets the appearance settings used to paint the filter header area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.FilterHeaderArea")
		]
		public AppearanceObject FilterHeaderArea { get { return filterHeaderArea; } }
		bool ShouldSerializeDataHeaderArea() { return DataHeaderArea.ShouldSerialize(); }
		void ResetDataHeaderArea() { DataHeaderArea.Reset(); }
		[
		Description("Gets the appearance settings used to paint the data header area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.DataHeaderArea")
		]
		public AppearanceObject DataHeaderArea { get { return dataHeaderArea; } }
		bool ShouldSerializeEmpty() { return Empty.ShouldSerialize(); }
		void ResetEmpty() { Empty.Reset(); }
		[
		Description("Gets the appearance settings used to paint the pivot grid's empty area."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.Empty")
		]
		public AppearanceObject Empty { get { return empty; } }
		bool ShouldSerializeExpandButton() { return ExpandButton.ShouldSerialize(); }
		void ResetExpandButton() { ExpandButton.Reset(); }
		[
		Description("Gets the appearance settings used to paint expand buttons."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.ExpandButton")
		]
		public AppearanceObject ExpandButton { get { return drillDownButton; } }
		bool ShouldSerializeHeaderFilterButton() { return HeaderFilterButton.ShouldSerialize(); }
		void ResetHeaderFilterButton() { HeaderFilterButton.Reset(); }
		[
		Description("Gets the appearance settings used to paint filter buttons."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButton")
		]
		public AppearanceObject HeaderFilterButton { get { return headerFilterButton; } }
		bool ShouldSerializeHeaderFilterButtonActive() { return HeaderFilterButtonActive.ShouldSerialize(); }
		void ResetHeaderFilterButtonActive() { HeaderFilterButtonActive.Reset(); }
		[
		Description("Gets the appearance settings used to paint the filter buttons displayed within the field headers that are involved in filtering."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButtonActive")
		]
		public AppearanceObject HeaderFilterButtonActive { get { return headerFilterButtonActive; } }
		bool ShouldSerializePrefilterPanel() { return PrefilterPanel.ShouldSerialize(); }
		void ResetPrefilterPanel() { PrefilterPanel.Reset(); }
		[
		Description("Gets the appearance settings used to paint the Prefilter Panel."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.PrefilterPanel")
		]
		public AppearanceObject PrefilterPanel { get { return prefilterPanel; } }
		[
		Description("Gets or sets a glyph that is used to indicate that values of column/row fields are sorted by a specific row/column."), DefaultValue(null),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.SortByColumnIndicatorImage"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public Image SortByColumnIndicatorImage {
			get { return sortByColumnIndicatorImage; }
			set { sortByColumnIndicatorImage = value; }
		}
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo(UserLookAndFeel lf) {
			ArrayList list = new ArrayList();
			list.AddRange(GetEmptyAppearanceDefaultInfo());
			switch(lf.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin:
					list.AddRange(GetSkinAppearanceDefaultInfo(lf));
					break;
				case ActiveLookAndFeelStyle.Office2003:
					list.AddRange(GetOffice2003AppearanceDefaultInfo());
					break;
				case ActiveLookAndFeelStyle.WindowsXP:
					list.AddRange(GetWindowsXPAppearanceDefaultInfo());
					break;
				default:
					list.AddRange(GetFlatAppearanceDefaultInfo());
					break;
			}			
			return (AppearanceDefaultInfo[]) list.ToArray(typeof(AppearanceDefaultInfo));
		}
		public virtual AppearanceDefaultInfo[] GetEmptyAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo(GrandTotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueGrandTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(ColumnHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(RowHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FilterHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(DataHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(PrefilterPanelName, new AppearanceDefault(Color.Empty, Color.Empty)),
			};
		}
		public virtual AppearanceDefaultInfo[] GetFlatAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(CellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(FocusedCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
				new AppearanceDefaultInfo(SelectedCellAppearanceName, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight)),
				new AppearanceDefaultInfo(TotalCellAppearanceName, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info)),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, Color.LightGray)),
				new AppearanceDefaultInfo(FieldValueAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(HeaderAreaAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(LinesAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(HeaderGroupLineAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDarkDark)),
				new AppearanceDefaultInfo(EmptyAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(ExpandButtonAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(HeaderFilterButtonAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(HeaderFilterButtonActiveAppearanceName, new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(PrefilterPanelName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
			};
		}
		public virtual AppearanceDefaultInfo[] GetWindowsXPAppearanceDefaultInfo() {
			return GetFlatAppearanceDefaultInfo();
		}
		public virtual AppearanceDefaultInfo[] GetOffice2003AppearanceDefaultInfo() {
			AppearanceDefaultInfo[] appearances = GetFlatAppearanceDefaultInfo();
			for(int i = 0; i < appearances.Length; i ++) {
				if(appearances[i].Name == HeaderAreaAppearanceName) {
					appearances[i].DefaultAppearance = Office2003Colors.Default[Office2003GridAppearance.GroupPanel].Clone() as AppearanceDefault;
					break;
				}
				if(appearances[i].Name == PrefilterPanelName) {
					appearances[i].DefaultAppearance = Office2003Colors.Default[Office2003GridAppearance.FooterPanel].Clone() as AppearanceDefault;
					break;
				}
			}
			return appearances;
		}
		public virtual AppearanceDefaultInfo[] GetSkinAppearanceDefaultInfo(UserLookAndFeel lf) {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(CellAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
 				new AppearanceDefaultInfo(FocusedCellAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
				new AppearanceDefaultInfo(SelectedCellAppearanceName, UpdateSystemColors(lf, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight))),
				new AppearanceDefaultInfo(TotalCellAppearanceName, UpdateSystemColors(lf, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info))),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, UpdateSystemColors(lf, new AppearanceDefault(SystemColors.WindowText, Color.LightGray))),
				new AppearanceDefaultInfo(FieldValueAppearanceName, UpdateAppearance(lf, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, UpdateAppearance(lf, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
				new AppearanceDefaultInfo(HeaderAreaAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(LinesAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, UpdateFilterSeparatorAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo(HeaderGroupLineAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDarkDark))),
				new AppearanceDefaultInfo(FilterHeaderAreaAppearanceName, UpdateHeaderAreaAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(Color.Empty, Color.Empty))),
				new AppearanceDefaultInfo(EmptyAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
				new AppearanceDefaultInfo(ExpandButtonAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(HeaderFilterButtonAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(HeaderFilterButtonActiveAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(Color.Blue, SystemColors.Control, SystemColors.ControlLightLight))),
				new AppearanceDefaultInfo(PrefilterPanelName, UpdateAppearance(lf, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark))),
			};
		}
		protected AppearanceDefault UpdateAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			if(element.Color.GetBackColor() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateHeaderAreaAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			if(element.Color.GetBackColor2() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor2();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateFilterSeparatorAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			if(element.Border.All != Color.Empty) {
				info.BackColor = element.Border.All;
				info.BackColor2 = element.Border.All;
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateSystemColors(UserLookAndFeel lf, AppearanceDefault info) {
			info.ForeColor = CommonSkins.GetSkin(lf).TranslateColor(info.ForeColor);
			info.BackColor = CommonSkins.GetSkin(lf).TranslateColor(info.BackColor);
			return info;
		}
	}
	public class PivotGridAppearancesPrint : PivotGridAppearancesBase {
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(CellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(TotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, Color.LightGray)),
				new AppearanceDefaultInfo(GrandTotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(FieldValueTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueGrandTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(LinesAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))
			};
		}	
	}
	public class PivotGridFieldAppearances : BaseAppearanceCollection {
		protected const string ValueAppearanceName = "Value";
		protected const string ValueTotalAppearanceName = "ValueTotal";
		protected const string HeaderAppearanceName = "Header";
		AppearanceObject value, valueTotal, header;
		protected override void CreateAppearances() {
			this.value = CreateAppearance(ValueAppearanceName);
			this.header = CreateAppearance(HeaderAppearanceName);
			this.valueTotal = CreateAppearance(ValueTotalAppearanceName);
		}
		bool ShouldSerializeValue() { return Value.ShouldSerialize(); }
		void ResetValue() { Value.Reset(); }
		[Description("Gets the appearance settings used to paint the values of fields."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Value { get { return value; } }
		bool ShouldSerializeHeader() { return Header.ShouldSerialize(); }
		void ResetHeader() { Header.Reset(); }
		[Description("Gets the appearance settings used to paint the field header."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Header { get { return header; } }
		bool ShouldSerializeValueTotal() { return ValueTotal.ShouldSerialize(); }
		void ResetValueTotal() { ValueTotal.Reset(); }
		[Description("Gets the appearance settings used to paint the field's totals."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ValueTotal { get { return valueTotal; } }
	}
}
