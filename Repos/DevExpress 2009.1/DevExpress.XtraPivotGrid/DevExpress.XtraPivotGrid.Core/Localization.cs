#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
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
using System.Resources;
using System.Globalization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraPivotGrid.Localization {
	[ToolboxItem(false)]
	public class PivotGridLocalizer : XtraLocalizer<PivotGridStringId> {
		static PivotGridLocalizer() {
			if(GetActiveLocalizerProvider() == null) {
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PivotGridStringId>(new PivotGridResLocalizer()));
			}
		}
		public static new XtraLocalizer<PivotGridStringId> Active {
			get { return XtraLocalizer<PivotGridStringId>.Active; }
			set { XtraLocalizer<PivotGridStringId>.Active = value; }
		}
		public static string GetHeadersAreaText(int areaIndex) {
			PivotGridStringId[] ids = new PivotGridStringId[] { PivotGridStringId.RowHeadersCustomization, PivotGridStringId.ColumnHeadersCustomization, PivotGridStringId.FilterHeadersCustomization, PivotGridStringId.DataHeadersCustomization };
			return Active.GetLocalizedString(ids[(int)areaIndex]);
		}
		public static string GetString(PivotGridStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PivotGridStringId> CreateResXLocalizer() {
			return new PivotGridResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddString(PivotGridStringId.RowHeadersCustomization, "Drop Row Fields Here");
			AddString(PivotGridStringId.ColumnHeadersCustomization, "Drop Column Fields Here");
			AddString(PivotGridStringId.FilterHeadersCustomization, "Drop Filter Fields Here");
			AddString(PivotGridStringId.DataHeadersCustomization, "Drop Data Items Here");
			AddString(PivotGridStringId.RowArea, "Row Area");
			AddString(PivotGridStringId.ColumnArea, "Column Area");
			AddString(PivotGridStringId.FilterArea, "Filter Area");
			AddString(PivotGridStringId.DataArea, "Data Area");
			AddString(PivotGridStringId.FilterShowAll, "(Show All)");
			AddString(PivotGridStringId.FilterOk, "OK");
			AddString(PivotGridStringId.FilterCancel, "Cancel");
			AddString(PivotGridStringId.FilterShowBlanks, "(Show Blanks)");
			AddString(PivotGridStringId.CustomizationFormCaption, "PivotGrid Field List");
			AddString(PivotGridStringId.CustomizationFormText, "Drag Items to the PivotGrid");
			AddString(PivotGridStringId.CustomizationFormAddTo, "Add To");
			AddString(PivotGridStringId.Total, "Total");
			AddString(PivotGridStringId.GrandTotal, "Grand Total");
			AddString(PivotGridStringId.TotalFormat, "{0} Total");
			AddString(PivotGridStringId.TotalFormatCount, "{0} Count");
			AddString(PivotGridStringId.TotalFormatSum, "{0} Sum");
			AddString(PivotGridStringId.TotalFormatMin, "{0} Min");
			AddString(PivotGridStringId.TotalFormatMax, "{0} Max");
			AddString(PivotGridStringId.TotalFormatAverage, "{0} Average");
			AddString(PivotGridStringId.TotalFormatStdDev, "{0} StdDev");
			AddString(PivotGridStringId.TotalFormatStdDevp, "{0} StdDevp");
			AddString(PivotGridStringId.TotalFormatVar, "{0} Var");
			AddString(PivotGridStringId.TotalFormatVarp, "{0} Varp");
			AddString(PivotGridStringId.TotalFormatCustom, "{0} Custom");
			AddString(PivotGridStringId.PrintDesigner, "Print Designer");
			AddString(PivotGridStringId.PrintDesignerPageOptions, "Options");
			AddString(PivotGridStringId.PrintDesignerPageBehavior, "Behavior");
			AddString(PivotGridStringId.PrintDesignerCategoryDefault, "Default");
			AddString(PivotGridStringId.PrintDesignerCategoryLines, "Lines");
			AddString(PivotGridStringId.PrintDesignerCategoryHeaders, "Headers");
			AddString(PivotGridStringId.PrintDesignerCategoryFieldValues, "Field Values");
			AddString(PivotGridStringId.PrintDesignerHorizontalLines, "Horizontal Lines");
			AddString(PivotGridStringId.PrintDesignerVerticalLines, "Vertical Lines");
			AddString(PivotGridStringId.PrintDesignerFilterHeaders, "Filter Headers");
			AddString(PivotGridStringId.PrintDesignerDataHeaders, "Data Headers");
			AddString(PivotGridStringId.PrintDesignerColumnHeaders, "Column Headers");
			AddString(PivotGridStringId.PrintDesignerRowHeaders, "Row Headers");
			AddString(PivotGridStringId.PrintDesignerHeadersOnEveryPage, "Headers On Every Page");
			AddString(PivotGridStringId.PrintDesignerUnusedFilterFields, "Unused Filter Fields");
			AddString(PivotGridStringId.PrintDesignerMergeColumnFieldValues, "Merge Column Field Values");
			AddString(PivotGridStringId.PrintDesignerMergeRowFieldValues, "Merge Row Field Values");
			AddString(PivotGridStringId.PrintDesignerUsePrintAppearance, "Use Print Appearance");
			AddString(PivotGridStringId.PopupMenuRefreshData, "Refresh Data");
			AddString(PivotGridStringId.PopupMenuHideField, "Hide");
			AddString(PivotGridStringId.PopupMenuShowFieldList, "Show Field List");
			AddString(PivotGridStringId.PopupMenuHideFieldList, "Hide Field List");
			AddString(PivotGridStringId.PopupMenuFieldOrder, "Order");
			AddString(PivotGridStringId.PopupMenuMovetoBeginning, "Move to Beginning");
			AddString(PivotGridStringId.PopupMenuMovetoLeft, "Move to Left");
			AddString(PivotGridStringId.PopupMenuMovetoRight, "Move to Right");
			AddString(PivotGridStringId.PopupMenuMovetoEnd, "Move to End");
			AddString(PivotGridStringId.PopupMenuExpand, "Expand");
			AddString(PivotGridStringId.PopupMenuCollapse, "Collapse");
			AddString(PivotGridStringId.PopupMenuExpandAll, "Expand All");
			AddString(PivotGridStringId.PopupMenuCollapseAll, "Collapse All");
			AddString(PivotGridStringId.PopupMenuSortFieldByColumn, "Sort \"{0}\" by This Column");
			AddString(PivotGridStringId.PopupMenuSortFieldByRow, "Sort \"{0}\" by This Row");
			AddString(PivotGridStringId.PopupMenuRemoveAllSortByColumn, "Remove All Sorting");
			AddString(PivotGridStringId.DataFieldCaption, "Data");
			AddString(PivotGridStringId.TopValueOthersRow, "Others");
			AddString(PivotGridStringId.CellError, "Error");
			AddString(PivotGridStringId.CannotCopyMultipleSelections, "This command cannot be used on multiple selections.");   
			AddString(PivotGridStringId.PopupMenuShowPrefilter, "Show Prefilter");
			AddString(PivotGridStringId.PopupMenuHidePrefilter, "Hide Prefilter");
			AddString(PivotGridStringId.PrefilterFormCaption, "PivotGrid Prefilter");
			AddString(PivotGridStringId.EditPrefilter, "Edit Prefilter");
			AddString(PivotGridStringId.OLAPMeasuresCaption, "Measures");
			AddString(PivotGridStringId.OLAPDrillDownFilterException, "Show Details command cannot be executed when multiple items are selected in a report filter field. Select a single item for each field in the report filter area before performing a drillthrough.");
			AddString(PivotGridStringId.TrendGoingDown, "Going Down");
			AddString(PivotGridStringId.TrendGoingUp, "Going Up");
			AddString(PivotGridStringId.TrendNoChange, "No Change");
			AddString(PivotGridStringId.StatusBad, "Bad");
			AddString(PivotGridStringId.StatusGood, "Good");
			AddString(PivotGridStringId.StatusNeutral, "Neutral");
			AddString(PivotGridStringId.Alt_Expand, "[Expand]");
			AddString(PivotGridStringId.Alt_Collapse, "[Collapse]");
			AddString(PivotGridStringId.Alt_SortedAscending, "(Ascending)");
			AddString(PivotGridStringId.Alt_SortedDescending, "(Descending)");
			AddString(PivotGridStringId.Alt_FilterWindowSizeGrip, "[Resize]");
			AddString(PivotGridStringId.Alt_FilterButton, "[Filter]");
			AddString(PivotGridStringId.Alt_FilterButtonActive, "[Filtered]");
			AddString(PivotGridStringId.Alt_DragHideField, "Hide");
		}
	}
	public class PivotGridResLocalizer : PivotGridLocalizer {
		ResourceManager manager = null;
		public PivotGridResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraPivotGrid.LocalizationRes", typeof(PivotGridResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(PivotGridStringId id) {
			string resStr = "PivotGridStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum PivotGridStringId
	public enum PivotGridStringId {
		RowHeadersCustomization,
		ColumnHeadersCustomization,
		FilterHeadersCustomization,
		DataHeadersCustomization,
		RowArea,
		ColumnArea,
		FilterArea,
		DataArea,
		FilterShowAll,
		FilterOk,
		FilterCancel,
		FilterShowBlanks,
		CustomizationFormCaption,
		CustomizationFormText,
		CustomizationFormAddTo,
		Total,
		GrandTotal,
		TotalFormat,
		TotalFormatCount,
		TotalFormatSum,
		TotalFormatMin,
		TotalFormatMax,
		TotalFormatAverage,
		TotalFormatStdDev,
		TotalFormatStdDevp,
		TotalFormatVar,
		TotalFormatVarp,
		TotalFormatCustom,
		PrintDesigner,
		PrintDesignerPageOptions,
		PrintDesignerPageBehavior,
		PrintDesignerCategoryDefault,
		PrintDesignerCategoryLines,
		PrintDesignerCategoryHeaders,
		PrintDesignerCategoryFieldValues,
		PrintDesignerHorizontalLines,
		PrintDesignerVerticalLines,
		PrintDesignerFilterHeaders,
		PrintDesignerDataHeaders,
		PrintDesignerColumnHeaders,
		PrintDesignerRowHeaders,
		PrintDesignerHeadersOnEveryPage,
		PrintDesignerUnusedFilterFields,
		PrintDesignerMergeColumnFieldValues,
		PrintDesignerMergeRowFieldValues,
		PrintDesignerUsePrintAppearance,
		PopupMenuRefreshData,
		PopupMenuHideField,
		PopupMenuShowFieldList,
		PopupMenuHideFieldList,
		PopupMenuFieldOrder,
		PopupMenuMovetoBeginning,
		PopupMenuMovetoLeft,
		PopupMenuMovetoRight,
		PopupMenuMovetoEnd,
		PopupMenuCollapse,
		PopupMenuExpand,
		PopupMenuCollapseAll,
		PopupMenuExpandAll,
		PopupMenuShowPrefilter,
		PopupMenuHidePrefilter,
		PopupMenuSortFieldByColumn,
		PopupMenuSortFieldByRow,
		PopupMenuRemoveAllSortByColumn,
		DataFieldCaption,
		TopValueOthersRow,
		CellError,
		CannotCopyMultipleSelections,	
		PrefilterFormCaption,
		EditPrefilter,
		OLAPMeasuresCaption,
		OLAPDrillDownFilterException,
		TrendGoingUp,
		TrendGoingDown,
		TrendNoChange,
		StatusBad, 
		StatusNeutral,
		StatusGood,
		Alt_Expand,
		Alt_Collapse,
		Alt_SortedAscending,
		Alt_SortedDescending,
		Alt_FilterWindowSizeGrip,
		Alt_FilterButton, 
		Alt_FilterButtonActive,
		Alt_DragHideField
	}
	#endregion
}
