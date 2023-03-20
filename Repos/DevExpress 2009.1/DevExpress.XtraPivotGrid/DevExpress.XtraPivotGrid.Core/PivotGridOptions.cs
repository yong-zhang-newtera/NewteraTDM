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
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsBase : BaseOptions, IXtraSerializableLayoutEx {
		public PivotGridOptionsBase() : this(null) { }
		public PivotGridOptionsBase(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsBase(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(viewBagOwner, objectPath) {
			this.OptionsChanged = optionsChanged;
		}
		public override void Assign(BaseOptions options) {
			PropertyDescriptorCollection localProperties = TypeDescriptor.GetProperties(this, AssigningAttributes),
				targetProperties = TypeDescriptor.GetProperties(options);
			foreach(PropertyDescriptor prop in targetProperties) {
				if(!localProperties.Contains(prop)) continue;
				localProperties[prop.Name].SetValue(this, prop.GetValue(options));
			}
		}
		Attribute[] AssigningAttributes { get { return new Attribute[] { new XtraSerializableProperty() }; } }
		protected event EventHandler OptionsChanged;
		protected void OnOptionsChanged() {
			if(OptionsChanged != null) OptionsChanged(this, new EventArgs());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShouldSerialize() { return base.ShouldSerialize(); }
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			Reset();
		}
	}
	public class PivotGridFieldOptions : PivotGridOptionsBase {
		DefaultBoolean allowSort;
		DefaultBoolean allowFilter;
		DefaultBoolean allowDrag;
		DefaultBoolean allowExpand;
		DefaultBoolean allowSortBySummary;
		bool allowRunTimeSummaryChange;
		bool showInCustomizationForm;
		bool showGrandTotal;
		bool showTotals;
		bool showValues;
		bool showCustomTotals;
		bool showSummaryTypeName;
		public PivotGridFieldOptions(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.allowSort = DefaultBoolean.Default;
			this.allowFilter = DefaultBoolean.Default;
			this.allowDrag = DefaultBoolean.Default;
			this.allowExpand = DefaultBoolean.Default;
			this.allowSortBySummary = DefaultBoolean.Default;
			this.allowRunTimeSummaryChange = false;
			this.showInCustomizationForm = true;
			this.showGrandTotal = true;
			this.showTotals = true;
			this.showValues = true;
			this.showCustomTotals = true;
			this.showSummaryTypeName = false;
		}
		[Description("Gets or sets whether an end-user can modify the field's current sort order."),
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSort")]
		public DefaultBoolean AllowSort {
			get { return allowSort; }
			set {
				if(value == AllowSort) return;
				allowSort = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether an end-user can apply a filter to the current field."),
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowFilter")]
		public DefaultBoolean AllowFilter {
			get { return allowFilter; }
			set {
				if(value == AllowFilter) return;
				allowFilter = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether an end-user is allowed to drag the Field Header."),
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowDrag")]
		public DefaultBoolean AllowDrag {
			get { return allowDrag; }
			set {
				if(value == AllowDrag) return;
				allowDrag = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether an end-user is allowed to expand/collapse the rows or columns which correspond to the current field."),
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowExpand")]
		public DefaultBoolean AllowExpand {
			get { return allowExpand; }
			set {
				if(value == AllowExpand) return;
				allowExpand = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether end-users can sort the current row/column field values by other column/row summary values."),
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSortBySummary")]
		public DefaultBoolean AllowSortBySummary {
			get { return allowSortBySummary; }
			set {
				if(value == AllowSortBySummary) return;
				allowSortBySummary = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether an end-user can change the data field's summary type."),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowRunTimeSummaryChange"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool AllowRunTimeSummaryChange {
			get { return allowRunTimeSummaryChange; }
			set {
				if(value == AllowRunTimeSummaryChange) return;
				allowRunTimeSummaryChange = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether the field's header is displayed within the customization form when the field is hidden."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInCustomizationForm"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(value == ShowInCustomizationForm) return;
				showInCustomizationForm = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether grand totals that correspond to the current data field are visible."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowGrandTotal"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowGrandTotal {
			get { return showGrandTotal; }
			set {
				if(value == showGrandTotal) return;
				showGrandTotal = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether Totals that correspond to the current data field are visible."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowTotals {
			get { return showTotals; }
			set {
				if(value == showTotals) return;
				showTotals = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether data cells that correspond to the current data field are visible."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowValues {
			get { return showValues; }
			set {
				if(value == showValues) return;
				showValues = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether Custom Totals that correspond to the current Column Field or Row Field are visible."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowCustomTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowCustomTotals {
			get { return showCustomTotals; }
			set {
				if(value == showCustomTotals) return;
				showCustomTotals = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether the summary type is displayed within the data field's header."),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowSummaryTypeName"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowSummaryTypeName {
			get { return showSummaryTypeName; }
			set {
				if(value == showSummaryTypeName) return;
				showSummaryTypeName = value;
				OnOptionsChanged();
			}
		}
	}
	public class PivotGridOptionsCustomization : PivotGridOptionsBase {
		bool allowSort;
		bool allowFilter;
		bool allowDrag;
		bool allowExpand;
		bool allowSortBySummary;
		bool allowPrefilter;
		AllowHideFieldsType allowHideFields;
		public PivotGridOptionsCustomization(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsCustomization(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.allowSort = true;
			this.allowFilter = true;
			this.allowDrag = true;
			this.allowExpand = true;
			this.allowSortBySummary = true;
			this.allowPrefilter = true;
			this.allowHideFields = AllowHideFieldsType.WhenCustomizationFormVisible;
		}
		[
		Description("Gets or sets whether end-users can change the sort order of fields."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowSort {
			get { return allowSort; }
			set {
				if(value == AllowSort) return;
				allowSort = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether filter buttons are displayed within field headers."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowFilter {
			get { return allowFilter; }
			set {
				if(value == AllowFilter) return;
				allowFilter = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether end-users are allowed to drag field headers."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowDrag {
			get { return allowDrag; }
			set {
				if(value == AllowDrag) return;
				allowDrag = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether expand buttons are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowExpand {
			get { return allowExpand; }
			set {
				if(value == AllowExpand) return;
				allowExpand = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether end-users can sort row field values by column values, and column field values by row values."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowSortBySummary {
			get { return allowSortBySummary; }
			set {
				if(value == AllowSortBySummary) return;
				allowSortBySummary = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets a value which specifies when the fields can be hidden by end-users."),
		DefaultValue(AllowHideFieldsType.WhenCustomizationFormVisible), XtraSerializableProperty(),
		NotifyParentProperty(true)
		]
		public AllowHideFieldsType AllowHideFields {
			get { return allowHideFields; }
			set {
				if(value == AllowHideFields) return;
				allowHideFields = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether end-users are allowed to invoke the Prefilter."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowPrefilter {
			get { return allowPrefilter; }
			set {
				if(value == AllowPrefilter) return;
				allowPrefilter = value;
				OnOptionsChanged();
			}
		}
	}
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum DataFieldNaming { FieldName, Name };
	public class PivotGridOptionsDataField : PivotGridOptionsBase {
		PivotGridData data;
		PivotDataArea area;
		int areaIndex;
		int rowHeaderWidth;
		string caption;
		DataFieldNaming fieldNaming;
		bool enableFilteringByData;
		public PivotGridOptionsDataField(PivotGridData data) : this(data, null, string.Empty) { }
		public PivotGridOptionsDataField(PivotGridData data, IViewBagOwner viewBagOwner, string objectPath)
			: base(null, viewBagOwner, objectPath) {
			this.data = data;
			this.area = PivotDataArea.None;
			this.areaIndex = -1;
			this.rowHeaderWidth = Data.DefaultFieldWidth;
			this.caption = string.Empty;
			this.fieldNaming = DataFieldNaming.FieldName;
		}
		[
		Description("Gets or sets the area in which the data field headers are displayed. "),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.Area"),
		DefaultValue(PivotDataArea.None), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotDataArea Area {
			get { return area; }
			set {
				if(value == Area) return;
				this.area = value;
				OnChanged(true);
			}
		}
		[
		Description("Gets or sets the position of the data field headers. "),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.AreaIndex"),
		DefaultValue(-1), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public int AreaIndex {
			get { return areaIndex; }
			set {
				if(value < -1) value = -1;
				if(value == AreaIndex) return;
				AreaIndexOldCore = AreaIndex;
				AreaIndexCore = value;
				OnChanged(true);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotArea DataFieldArea {
			get {
				return Area == PivotDataArea.RowArea ? PivotArea.RowArea : PivotArea.ColumnArea;
			}
			set {
				if(value == PivotArea.ColumnArea || value == PivotArea.RowArea) {
					Area = value == PivotArea.ColumnArea ? PivotDataArea.ColumnArea : PivotDataArea.RowArea;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotDataArea DataFieldsLocationArea {
			get {
				if(Data == null || Data.DataFieldCount < 2) return PivotDataArea.None;
				return Area == PivotDataArea.RowArea ? Area : PivotDataArea.ColumnArea;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DataFieldAreaIndex {
			get {
				if(Data == null || !Data.GetIsDataFieldsVisible(DataFieldArea == PivotArea.ColumnArea)) 
					return -1;
				int index = AreaIndex;
				if(index > Data.GetFieldCountByArea(DataFieldArea) || index < 0)
					index = Data.GetFieldCountByArea(DataFieldArea);
				return index;
			}
			set {
				if(Data == null) return;
				if(value > Data.GetFieldCountByArea(DataFieldArea)) value = -1;
				AreaIndex = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DataFieldVisible {
			get {
				if(Data == null || Data.Fields == null) return false;
				if(!Data.GetIsDataFieldsVisible(DataFieldArea == PivotArea.ColumnArea)) return false;
				return Area != PivotDataArea.None;
			}
			set {
				if(DataFieldVisible == value) return;
				if(value) {
					if(Area == PivotDataArea.None) {
						Area = PivotDataArea.ColumnArea;
					}
				} else Area = PivotDataArea.None;
			}
		}
		[
		Description("Gets or sets the width of the data field headers when they are displayed as row headers."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.RowHeaderWidth"),
		DefaultValue(100), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public int RowHeaderWidth {
			get { return rowHeaderWidth; }
			set {
				if(value < PivotGridData.DefaultFieldMinWidth)
					value = PivotGridData.DefaultFieldMinWidth;
				if(value == RowHeaderWidth) return;
				rowHeaderWidth = value;
				OnChanged(false);
			}
		}
		[
		Description("Gets or sets the text displayed within the data header."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.Caption"),
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), Localizable(true)
		]
		public string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(value == Caption) return;
				caption = value;
				OnChanged(false);
			}
		}
		[
		Description("Gets or sets the rule for naming columns, that correspond to data fields, when creating a data source via the CreateSummaryDataSource methods."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.FieldNaming"),
		DefaultValue(DataFieldNaming.FieldName), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public DataFieldNaming FieldNaming {
			get { return fieldNaming; }
			set {
				if(value == fieldNaming) return;
				fieldNaming = value;
				OnChanged(false);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EnableFilteringByData {
			get { return enableFilteringByData; }
			set { enableFilteringByData = value; }
		}
		internal int AreaIndexCore { get { return areaIndex; } set { areaIndex = value; } }
		protected int AreaIndexOldCore { get { return Data.DataField.AreaIndexOldCore; } set { Data.DataField.AreaIndexOldCore = value; } }
		protected PivotGridData Data { get { return data; } }
		protected virtual void OnChanged(bool dataChanged) {
			if(Data.DataFieldCount > 1) {
				if(dataChanged)
					Data.DoRefresh();
				else Data.LayoutChanged();
			}
		}
	}
	public class PivotGridOptionsData : PivotGridOptionsBase {
		PivotGridData data;
		bool allowCrossGroupVariation;
		int fDrillDownMaxRowCount;
		public PivotGridOptionsData(PivotGridData data, EventHandler optionsChanged)
			: base(optionsChanged) {
			this.data = data;
			this.allowCrossGroupVariation = true;
			this.fDrillDownMaxRowCount = PivotDrillDownDataSource.AllRows;
		}
		PivotGridData Data { get { return data; } set { data = value; } }
		[
		Description("Gets or sets whether data is grouped case-sensitive."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsData.CaseSensitive"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true),
		XtraSerializableProperty(),
		AutoFormatEnable(),
		NotifyParentProperty(true)
		]
		public bool CaseSensitive {
			get { return Data.CaseSensitive; }
			set { Data.CaseSensitive = value; }
		}
		[
		Description("Gets or sets whether summary variations are calculated independently within individual groups or throughout the PivotGridControl."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsData.AllowCrossGroupVariation"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public bool AllowCrossGroupVariation {
			get { return allowCrossGroupVariation; }
			set {
				if(AllowCrossGroupVariation == value) return;
				allowCrossGroupVariation = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets the maximum number of rows returned when calling the CreateDrillDownDataSource method."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
			DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridOptionsData.DrillDownMaxRowCount"),
		DefaultValue(PivotDrillDownDataSource.AllRows), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public int DrillDownMaxRowCount {
			get { return fDrillDownMaxRowCount; }
			set {
				if(fDrillDownMaxRowCount == value) return;
				if(value != PivotDrillDownDataSource.AllRows && value <= 0)
					value = PivotDrillDownDataSource.AllRows;
				fDrillDownMaxRowCount = value;
				OnOptionsChanged();
			}
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {	
	public class PivotGridOptionsChartDataSourceBase : PivotGridOptionsBase {
		bool chartDataVertical,
			showColumnGrandTotals, showRowGrandTotals,
			showColumnTotals, showRowTotals,
			showColumnCustomTotals, showRowCustomTotals;
		public PivotGridOptionsChartDataSourceBase() {
			chartDataVertical = true;
			showColumnGrandTotals = true;
			showRowGrandTotals = true;
			showColumnTotals = true;
			showRowTotals = true;
			showColumnCustomTotals = true;
			showRowCustomTotals = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler Changed {
			add { base.OptionsChanged += value; }
			remove { base.OptionsChanged -= value; }
		}
		[Description("Gets or sets whether series in a chart control are created based on PivotGrid columns or rows."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ChartDataVertical {
			get { return chartDataVertical; }
			set {
				if(chartDataVertical == value) return;
				chartDataVertical = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether data of column grand totals is displayed in a chart control."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowColumnGrandTotals {
			get { return showColumnGrandTotals; }
			set {
				if(showColumnGrandTotals == value) return;
				showColumnGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether data of row grand totals is displayed in a chart control."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowRowGrandTotals {
			get { return showRowGrandTotals; }
			set {
				if(showRowGrandTotals == value) return;
				showRowGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[Description(""), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowColumnTotals {
			get { return showColumnTotals; }
			set {
				if(showColumnTotals == value) return;
				showColumnTotals = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether data of row totals is displayed in a chart control."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowRowTotals {
			get { return showRowTotals; }
			set {
				if(showRowTotals == value) return;
				showRowTotals = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether data of column custom totals should be displayed in a chart control."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowColumnCustomTotals {
			get { return showColumnCustomTotals; }
			set {
				if(showColumnCustomTotals == value) return;
				showColumnCustomTotals = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether data of row custom totals should be displayed in a chart control."), DefaultValue(true),
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowRowCustomTotals {
			get { return showRowCustomTotals; }
			set {
				if(showRowCustomTotals == value) return;
				showRowCustomTotals = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false)]
		public bool ShouldRemoveTotals {
			get {
				return !ShowRowTotals || !ShowColumnTotals || !ShowRowCustomTotals || !ShowColumnCustomTotals ||
				  !ShowRowGrandTotals || !ShowColumnGrandTotals;
			}
		}
		public bool ShouldRemoveItem(bool isColumn, PivotGridValueType valueType) {
			switch(valueType) {
				case PivotGridValueType.Total:
					return (isColumn && !ShowColumnTotals) || (!isColumn && !ShowRowTotals);
				case PivotGridValueType.CustomTotal:
					return (isColumn && !ShowColumnCustomTotals) || (!isColumn && !ShowRowCustomTotals);
				case PivotGridValueType.GrandTotal:
					return (isColumn && !ShowColumnGrandTotals) || (!isColumn && !ShowRowGrandTotals);
			}
			return false;
		}
	}
	public class PivotGridOptionsPrint : PivotGridOptionsBase {
		DefaultBoolean printHorzLines;
		DefaultBoolean printVertLines;
		bool printHeadersOnEveryPage;
		DefaultBoolean[] showHeaders;
		int columnFieldValueSeparator;
		int rowFieldValueSeparator;
		bool usePrintAppearance;
		bool mergeColumnFieldValues;
		bool mergeRowFieldValues;
		bool printUnusedFilterFields;
		PivotGridPageSettings pageSettings;
		VerticalContentSplitting verticalContentSplitting;
		public PivotGridOptionsPrint() {
			this.printHorzLines = DefaultBoolean.Default;
			this.printVertLines = DefaultBoolean.Default;
			this.usePrintAppearance = false;
			this.showHeaders = new DefaultBoolean[Enum.GetValues(typeof(PivotArea)).Length];
			for(int i = 0; i < showHeaders.Length; i++)
				this.showHeaders[i] = DefaultBoolean.Default;
			this.columnFieldValueSeparator = 0;
			this.rowFieldValueSeparator = 0;
			this.printHeadersOnEveryPage = false;
			this.mergeColumnFieldValues = true;
			this.mergeRowFieldValues = true;
			this.printUnusedFilterFields = true;
			this.pageSettings = new PivotGridPageSettings();
			this.verticalContentSplitting = VerticalContentSplitting.Smart;
		}
		[Description("Gets or sets whether a cell is allowed to be split across pages.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.VerticalContentSplitting")]
		[DefaultValue(VerticalContentSplitting.Smart), XtraSerializableProperty()]
		public virtual VerticalContentSplitting VerticalContentSplitting { get { return verticalContentSplitting; } set { verticalContentSplitting = value; } }
		[Description("Gets or sets whether horizontal grid lines are printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHorzLines")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintHorzLines { get { return printHorzLines; } set { printHorzLines = value; } }
		[Description("Gets or sets whether vertical grid lines are printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintVertLines")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintVertLines { get { return printVertLines; } set { printVertLines = value; } }
		public DefaultBoolean GetPrintHeaders(PivotArea area) { return this.showHeaders[(int)area]; }
		protected void SetPrintHeaders(PivotArea area, DefaultBoolean value) { this.showHeaders[(int)area] = value; }
		[Description("Gets or sets whether data field headers are printed. ")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintDataHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintDataHeaders {
			get { return GetPrintHeaders(PivotArea.DataArea); }
			set { SetPrintHeaders(PivotArea.DataArea, value); }
		}
		[Description("Gets or sets whether filter field headers are printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintFilterHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintFilterHeaders {
			get { return GetPrintHeaders(PivotArea.FilterArea); }
			set { SetPrintHeaders(PivotArea.FilterArea, value); }
		}
		[Description("Gets or sets whether column field headers are printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintColumnHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintColumnHeaders {
			get { return GetPrintHeaders(PivotArea.ColumnArea); }
			set { SetPrintHeaders(PivotArea.ColumnArea, value); }
		}
		[Description("Gets or sets whether row field headers are printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintRowHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean PrintRowHeaders {
			get { return GetPrintHeaders(PivotArea.RowArea); }
			set { SetPrintHeaders(PivotArea.RowArea, value); }
		}
		[
		Description("Gets or sets whether to print column headers on every page."),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHeadersOnEveryPage"),
		DefaultValue(false), XtraSerializableProperty(),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool PrintHeadersOnEveryPage {
			get { return printHeadersOnEveryPage; }
			set { printHeadersOnEveryPage = value; }
		}
		[
		Description("Gets or sets whether print appearances are used when the pivot grid is printed."),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.UsePrintAppearance"),
		DefaultValue(false), XtraSerializableProperty(),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool UsePrintAppearance { get { return usePrintAppearance; } set { usePrintAppearance = value; } }
		[Description("Gets or sets the distance between the values of column fields when the pivot grid is printed. ")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.ColumnFieldValueSeparator")]
		[DefaultValue(0), XtraSerializableProperty()]
		public int ColumnFieldValueSeparator {
			get { return columnFieldValueSeparator; }
			set {
				if(value < 0) value = 0;
				columnFieldValueSeparator = value;
			}
		}
		[Description("Gets or sets the distance between row field values when the pivot grid is printed.")]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.RowFieldValueSeparator")]
		[DefaultValue(0), XtraSerializableProperty()]
		public int RowFieldValueSeparator {
			get { return rowFieldValueSeparator; }
			set {
				if(value < 0) value = 0;
				rowFieldValueSeparator = value;
			}
		}
		[
		Description("Gets or sets whether the values of outer column fields are merged when a pivot grid is printed."),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeColumnFieldValues"),
		DefaultValue(true), XtraSerializableProperty(),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool MergeColumnFieldValues {
			get { return mergeColumnFieldValues; }
			set { mergeColumnFieldValues = value; }
		}
		[
		Description("Gets or sets whether the values of outer row fields are merged when a pivot grid is printed."),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeRowFieldValues"),
		DefaultValue(true), XtraSerializableProperty(),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool MergeRowFieldValues {
			get { return mergeRowFieldValues; }
			set { mergeRowFieldValues = value; }
		}
		public bool IsMergeFieldValues(bool isColumn) {
			return isColumn ? MergeColumnFieldValues : MergeRowFieldValues;
		}
		[
		Description("Gets or sets whether unused filter fields are printed/exported. "),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintUnusedFilterFields"),
		DefaultValue(true), XtraSerializableProperty(),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool PrintUnusedFilterFields { get { return printUnusedFilterFields; } set { printUnusedFilterFields = value; } }
		bool ShouldSerializePageSettings() { return !PageSettings.IsEmpty; }
		void ResetPageSettings() { PageSettings.Reset(); }
		[Description("Provides print page settings."), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PageSettings")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PivotGridPageSettings PageSettings { get { return pageSettings; } }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			PivotGridOptionsPrint optionsPrint = options as PivotGridOptionsPrint;
			if(optionsPrint == null) return;
			this.ColumnFieldValueSeparator = optionsPrint.ColumnFieldValueSeparator;
			this.MergeColumnFieldValues = optionsPrint.MergeColumnFieldValues;
			this.MergeRowFieldValues = optionsPrint.MergeRowFieldValues;
			this.PrintColumnHeaders = optionsPrint.PrintColumnHeaders;
			this.PrintDataHeaders = optionsPrint.PrintDataHeaders;
			this.PrintFilterHeaders = optionsPrint.PrintFilterHeaders;
			this.PrintHeadersOnEveryPage = optionsPrint.PrintHeadersOnEveryPage;
			this.PrintHorzLines = optionsPrint.PrintHorzLines;
			this.PrintRowHeaders = optionsPrint.PrintRowHeaders;
			this.PrintUnusedFilterFields = optionsPrint.PrintUnusedFilterFields;
			this.PrintVertLines = optionsPrint.PrintVertLines;
			this.RowFieldValueSeparator = optionsPrint.RowFieldValueSeparator;
			this.UsePrintAppearance = optionsPrint.UsePrintAppearance;
			this.PageSettings.Assign(optionsPrint.PageSettings);
			this.VerticalContentSplitting = optionsPrint.VerticalContentSplitting;
		}
	}
	public class PivotGridOptionsViewBase : PivotGridOptionsBase {
		const int DefaultHeaderOffset = 3;
		int headerWidthOffset, headerHeightOffset;
		bool showColumnTotals;
		bool showRowTotals;
		bool showColumnGrandTotals;
		bool showRowGrandTotals;
		PivotTotalsLocation totalsLocation;
		bool showTotalsForSingleValues;
		bool showCustomTotalsForSingleValues;
		bool showGrandTotalsForSingleValues;
		bool showVertLines;
		bool showHorzLines;
		bool[] showHeaders;
		bool drawFocusedCellRect;
		bool showFilterSeparatorBar;
		public PivotGridOptionsViewBase(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsViewBase(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.showColumnTotals = false;
			this.showRowTotals = false;
			this.showColumnGrandTotals = false;
			this.showRowGrandTotals = false;
			this.totalsLocation = PivotTotalsLocation.Far;
			this.showTotalsForSingleValues = false;
			this.showCustomTotalsForSingleValues = false;
			this.showGrandTotalsForSingleValues = false;
			this.showHeaders = new bool[Enum.GetValues(typeof(PivotArea)).Length];
			for(int i = 0; i < this.showHeaders.Length; i++)
				this.showHeaders[i] = true;
			this.showHorzLines = true;
			this.showVertLines = true;
			this.headerWidthOffset = headerHeightOffset = DefaultHeaderOffset;
			this.drawFocusedCellRect = true;
			this.showFilterSeparatorBar = true;
		}
		[Description("Gets or sets the distance (horizontal) between field headers. "),
		DefaultValue(DefaultHeaderOffset), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderWidthOffset")]
		public int HeaderWidthOffset {
			get { return headerWidthOffset; }
			set {
				if(value == HeaderWidthOffset) return;
				headerWidthOffset = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets the distance (vertical) between field headers. "),
		DefaultValue(DefaultHeaderOffset), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderHeightOffset")]
		public int HeaderHeightOffset {
			get { return headerHeightOffset; }
			set {
				if(value == HeaderHeightOffset) return;
				headerHeightOffset = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether column automatic Totals are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnTotals {
			get { return showColumnTotals; }
			set {
				if(ShowColumnTotals == value) return;
				showColumnTotals = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether row automatic Totals are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowTotals {
			get { return showRowTotals; }
			set {
				if(ShowRowTotals == value) return;
				showRowTotals = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether column Grand Totals are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnGrandTotals {
			get { return showColumnGrandTotals; }
			set {
				if(ShowColumnGrandTotals == value) return;
				showColumnGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether row Grand Totals are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowGrandTotals {
			get { return showRowGrandTotals; }
			set {
				if(ShowRowGrandTotals == value) return;
				showRowGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets the location of the totals and grand totals."),
		DefaultValue(PivotTotalsLocation.Far), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.TotalsLocation")]
		public PivotTotalsLocation TotalsLocation {
			get { return totalsLocation; }
			set {
				if(TotalsLocation == value) return;
				totalsLocation = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether automatic totals are displayed for the field values which contain a single nesting field value. "),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowTotalsForSingleValues {
			get { return showTotalsForSingleValues; }
			set {
				if(value == ShowTotalsForSingleValues) return;
				showTotalsForSingleValues = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether custom totals are displayed for the field values which contain a single nesting field value. "),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowCustomTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowCustomTotalsForSingleValues {
			get { return showCustomTotalsForSingleValues; }
			set {
				if(value == ShowCustomTotalsForSingleValues) return;
				showCustomTotalsForSingleValues = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether grand totals are displayed when the control lists a single value of an outer column field or row field along its left or top edge."),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowGrandTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowGrandTotalsForSingleValues {
			get { return showGrandTotalsForSingleValues; }
			set {
				if(value == ShowGrandTotalsForSingleValues) return;
				showGrandTotalsForSingleValues = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether horizontal grid lines are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowHorzLines"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowHorzLines {
			get { return showHorzLines; }
			set {
				if(value == ShowHorzLines) return;
				showHorzLines = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether vertical grid lines are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowVertLines"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowVertLines {
			get { return showVertLines; }
			set {
				if(value == ShowVertLines) return;
				showVertLines = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether data headers are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowDataHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowDataHeaders {
			get { return GetShowHeaders(PivotArea.DataArea); }
			set { SetShowHeaders(PivotArea.DataArea, value); }
		}
		[
		Description("Gets or sets whether filter headers are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowFilterHeaders {
			get { return GetShowHeaders(PivotArea.FilterArea); }
			set { SetShowHeaders(PivotArea.FilterArea, value); }
		}
		[
		Description("Gets or sets whether column headers are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnHeaders {
			get { return GetShowHeaders(PivotArea.ColumnArea); }
			set { SetShowHeaders(PivotArea.ColumnArea, value); }
		}
		[
		Description("Gets or sets whether row headers are displayed."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowHeaders {
			get { return GetShowHeaders(PivotArea.RowArea); }
			set { SetShowHeaders(PivotArea.RowArea, value); }
		}
		[
		Description("Gets or sets whether a focus rectangle is painted around the currently focused cell."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.DrawFocusedCellRect"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool DrawFocusedCellRect {
			get { return drawFocusedCellRect; }
			set {
				if(value == DrawFocusedCellRect) return;
				drawFocusedCellRect = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets whether the horizontal line that separates the Filter Header Area from the Data Area and Column Header Area is displayed. "),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterSeparatorBar"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowFilterSeparatorBar {
			get { return showFilterSeparatorBar; }
			set {
				if(value == ShowFilterSeparatorBar) return;
				showFilterSeparatorBar = value;
				OnOptionsChanged();
			}
		}
		public bool GetShowHeaders(PivotArea area) {
			return this.showHeaders[(int)area];
		}
		protected void SetShowHeaders(PivotArea area, bool value) {
			if(value == GetShowHeaders(area)) return;
			this.showHeaders[(int)area] = value;
			OnOptionsChanged();
		}
		string GetShowHeadersViewBagName(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea: return "ShowColumnHeaders";
				case PivotArea.RowArea: return "ShowRowHeaders";
				case PivotArea.DataArea: return "ShowDataHeaders";
				case PivotArea.FilterArea: return "ShowFilterHeaders";
			}
			return string.Empty;
		}
		public void ShowAllTotals() {
			ShowAllTotals(true);
		}
		public void HideAllTotals() {
			ShowAllTotals(false);
		}
		protected void ShowAllTotals(bool show) {
			BeginUpdate();
			ShowColumnTotals = show;
			ShowColumnGrandTotals = show;
			ShowRowTotals = show;
			ShowRowGrandTotals = show;
			EndUpdate();
		}
		public override void Reset() {
			BeginUpdate();
			ShowAllTotals(true);
			TotalsLocation = PivotTotalsLocation.Far;
			ShowTotalsForSingleValues = false;
			ShowCustomTotalsForSingleValues = false;
			ShowGrandTotalsForSingleValues = false;
			for(int i = 0; i < this.showHeaders.Length; i++)
				showHeaders[i] = true;
			ShowHorzLines = true;
			ShowVertLines = true;
			EndUpdate();
		}
	}
}
