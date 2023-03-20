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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.WebUtils;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPopupControl;
using System.Globalization;
using System.IO.Compression;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.ASPxPivotGrid.Data {
	public interface IASPxPivotGridDataOwnerExtended : IASPxPivotGridDataOwner {
		void CustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle);
		void PageIndexChanged();
		void OLAPQueryTimeout();
		void LayoutChanged();
		PivotKPIImages KPIImages { get; }
	}
	internal enum HeaderType { Header, Group, Area };
	public class PivotGridWebData : PivotGridData, IViewBagOwner {
		public new const string WebResourcePath = "DevExpress.Web.ASPxPivotGrid.";
		public new const string PivotGridImagesResourcePath = WebResourcePath + "Images.";
		public const string PivotGridScriptsResourcePath = WebResourcePath + "Scripts.";
		public const string PivotGridScriptResourceName = PivotGridScriptsResourcePath + "PivotGrid.js";
		public const string PivotGridCssResourceName = WebResourcePath + "Css.Default.css";
		public static string[] FieldHeaderIdPostfixes = new string[] { "T", "F", "S", "GB" };
		public const string ElementName_FilterPopupContent = "FPC";
		public const string ElementName_CustomizationFieldsId = "DXCustFields";
		public const string ElementName_CustomizationFieldsHiddenInputId = "CFP";
		public const string ElementName_HeaderMenu = "HM";
		public const string ElementName_FieldValueMenu = "FVM";
		public const string ElementName_FilterPopupWindow = "FPW";
		public const string ElementName_FilterPopupWindowOKButton = "FPWOK";
		public const string ElementName_FilterPopupWindowResizer = "FPWR";
		public const string ElementName_FilterLoadingPanel = "FLP";
		public const string ElementName_MainTable = "MT";
		public const string ElementName_MainTD = "MTD";
		public const string ElementName_AreaCellContainer = "ACC";
		public const string ElementName_DataHeadersPopup = "DHP";
		public const string ElementName_DataHeadersPopupCell = "DHPC";
		public const char ArgumentsSeparator = '|';
		public const char ExpandChangedChar = 'E';
		public const string ExpandColumnChanged = "EC";
		public const string ExpandRowChanged = "ER";
		public const string ExpandFieldChanged = "EA";
		public const string CollapseFieldChanged = "CA";
		public const string FilterChanged = "F";
		public const string FilterShowWindow = "FS";
		public const string HeaderDrag = "D";
		public const string HeaderSort = "S";
		public const string HeaderSortBySummaryChanged = "SS";
		public const string HideField = "H";
		public const string Pager = "P";
		public const string CustomCallback = "C";
		public const string GroupExpanded = "G";
		public const string ShowPrefilter = "PREFILTER";
		public const string ReloadDataCallback = "RELOAD";
		internal const string CancelBubbleJs = "event.cancelBubble=true";
		WebControl owner;
		IDataSourceViewSchemaAccessor dataSourceViewSchemaAccessor = null;
		int selectedFieldIndex = -1;
		string webFieldValuesStateCache;
		bool fieldsWereGrouped;
		ITemplate headerTemplate = null;
		ITemplate emptyAreaTemplate = null;
		ITemplate fieldValueTemplate = null;
		ITemplate cellTemplate = null;
		public PivotGridWebData(WebControl owner)  {
			this.owner = owner;
			OptionsView.ShowAllTotals();
			this.optionsPager = CreateOptionsPager();
			this.optionsLoadingPanel = new PivotGridWebOptionsLoadingPanel(PivotGrid);
		}
		public WebControl Owner { get { return owner; } }
		internal ASPxPivotGrid PivotGrid { get { return Owner as ASPxPivotGrid; } }
		public new PivotWebVisualItems VisualItems { get { return (PivotWebVisualItems)base.VisualItems; } }
		internal PivotGridStyles Styles { get { return PivotGrid.Styles; } }
		public PivotKPIImages KPIImages { get { return DataOwner != null ? DataOwner.KPIImages : null; } }
		public IASPxPivotGridDataOwnerExtended DataOwner { get { return Owner as IASPxPivotGridDataOwnerExtended; } }
		public override bool IsDesignMode { get { return Owner as ASPxDataWebControlBase != null ? (Owner as ASPxDataWebControlBase).DesignMode : false; } }
		public bool IsDataBound { get { return ListSource != null || !string.IsNullOrEmpty(OLAPConnectionString); } }
		public override bool IsLoading { get { return IsDeserializing || (PivotGrid != null && !PivotGrid.Initialized); } }
		public string ClientID { get { return Owner.ClientID; } }
		public string GetHeaderMouseDown() {
			return string.Format("pivotGrid_HeaderMouseDown('{0}', this, event);", ClientID);
		}
		public string GetHeaderClick() {
			return string.Format("pivotGrid_HeaderClick('{0}', this, event);", ClientID);
		}
		public string GetCellClick(PivotGridCellItem cellItem) {
			return GetClickArgs("pivotGrid_CellClick", cellItem);
		}
		public string GetCellDblClick(PivotGridCellItem cellItem) {
			return GetClickArgs("pivotGrid_CellDoubleClick", cellItem);
		}
		public string GetAccessibleSortUrl(PivotGridField field) {
			return string.Format("javascript:pivotGrid_Sort508('{0}','{1}')", ClientID, GetHeaderID(field));
		}
		string GetClickArgs(string functionName, PivotGridCellItem cellItem) {
			StringBuilder str = new StringBuilder(functionName);
			str.Append("(\'").Append(ClientID)
				.Append("\',\'").Append(Convert.ToString(cellItem.Value, CultureInfo.InvariantCulture))
				.Append("\',").Append(cellItem.ColumnIndex)
				.Append(",").Append(GetGlobalRowIndex(cellItem))
				.Append(",\'").Append(EscapeString(Convert.ToString(cellItem.ColumnFieldValueItem.Value, CultureInfo.InvariantCulture)))
				.Append("\',\'").Append(EscapeString(Convert.ToString(cellItem.RowFieldValueItem.Value, CultureInfo.InvariantCulture)))
				.Append("\',\'").Append(cellItem.ColumnField != null ? FieldToString(cellItem.ColumnField) : "")
				.Append("\',\'").Append(cellItem.RowField != null ? FieldToString(cellItem.RowField) : "")
				.Append("\',\'").Append(cellItem.ColumnValueType.ToString())
				.Append("\',\'").Append(cellItem.RowValueType.ToString())
				.Append("\',").Append(cellItem.DataIndex).Append(");");
			return str.ToString();
		}
		int GetGlobalRowIndex(PivotGridCellItem cellItem) {
			if(OptionsView.TotalsLocation == PivotTotalsLocation.Near && cellItem.RowValueType == PivotGridValueType.GrandTotal)
				return cellItem.RowIndex;
			if(cellItem.RowIndex > OptionsPager.RowsPerPage - 1)
				return this.RowCellCount + cellItem.RowIndex - OptionsPager.RowsPerPage;
			return OptionsPager.RowsPerPage * OptionsPager.PageIndex + cellItem.RowIndex;
		}
		string FieldToString(PivotGridFieldBase field) {
			return EscapeString(string.IsNullOrEmpty(field.Name) ? field.FieldName : field.Name);
		}
		string EscapeString(string s) {
			return HtmlConvertor.EscapeString(s);
		}
		internal bool IsDataAreaCollapsed {
			get {
				return OptionsView.DataHeadersDisplayMode == PivotDataHeadersDisplayMode.Popup && GetFieldCountByArea(PivotArea.DataArea) >= OptionsView.DataHeadersPopupMinCount;
			}
		}
		protected Page Page { get { return Owner.Page; } }
		public new PivotGridFieldCollection Fields { get { return base.Fields as PivotGridFieldCollection; } }
		public new PivotGridWebGroupCollection Groups { get { return (PivotGridWebGroupCollection)base.Groups; } }
		public new WebPrefilter Prefilter { get { return (WebPrefilter)base.Prefilter; } }
		public override string OLAPConnectionString {
			get { return base.OLAPConnectionString; }
			set {
				BeginUpdate();
				base.OLAPConnectionString = value;
				CancelUpdate();
			}
		}
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new PivotWebVisualItems(this);
		}
		protected override PrefilterBase CreatePrefilter() {
			return new WebPrefilter(this);
		}
		protected override PivotGridFieldBase CreateDataField() {
			return new PivotGridField(this);
		}
		protected override PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollection(this);
		}
		protected override PivotGridGroupCollection CreateGroupCollection() {
			return new PivotGridWebGroupCollection(this);
		}
		public string WebFieldValuesStateCache { get { return webFieldValuesStateCache; } set { webFieldValuesStateCache = value; } }
		public override void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(DataOwner != null) DataOwner.EnsureRefreshData();
			base.ChangeFieldExpanded(field, expanded);
		}
		public override void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(DataOwner != null) DataOwner.EnsureRefreshData();
			base.ChangeFieldExpanded(field, expanded, value);
		}
		public override void ChangeExpandedAll(bool expanded) {
			if(DataOwner != null) DataOwner.EnsureRefreshData();
			base.ChangeExpandedAll(expanded);
		}
		public override object[] GetUniqueFieldValues(PivotGridFieldBase field) {
			if(DataOwner != null) DataOwner.EnsureRefreshData();
			return base.GetUniqueFieldValues(field);
		}
		public new PivotGridWebOptionsView OptionsView { get { return (PivotGridWebOptionsView)base.OptionsView; } }
		public new PivotGridWebOptionsCustomization OptionsCustomization { get { return (PivotGridWebOptionsCustomization)base.OptionsCustomization; } }
		protected override PivotGridOptionsViewBase CreateOptionsView() {
			return new PivotGridWebOptionsView(new EventHandler(OnOptionsViewChanged), this, "OptionsView"); 
		}
		protected override PivotGridOptionsCustomization CreateOptionsCustomization() {
			return new PivotGridWebOptionsCustomization(new EventHandler(OnOptionsChanged), this, "OptionsCustomization"); 
		}
		protected override PivotGridOptionsDataField CreateOptionsDataField() {
			return new PivotGridWebOptionsDataField(this, this, "OptionsDataField"); 
		}
		protected PivotGridWebOptionsPager optionsPager;
		public PivotGridWebOptionsPager OptionsPager { get { return optionsPager; } }
		protected PivotGridWebOptionsPager CreateOptionsPager() {
			return new PivotGridWebOptionsPager(new PivotGridWebOptionsPager.OptionsPagerChangedEventHandler(OnOptionsPagerChanged), this, "OptionsPager");
		}
		public new PivotGridWebOptionsChartDataSource OptionsChartDataSource { get { return (PivotGridWebOptionsChartDataSource)base.OptionsChartDataSource; } }
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridWebOptionsChartDataSource();
		}
		PivotGridWebOptionsLoadingPanel optionsLoadingPanel;
		public PivotGridWebOptionsLoadingPanel OptionsLoadingPanel { get { return optionsLoadingPanel; } }
		protected void OnOptionsPagerChanged(PivotGridWebOptionsPager sender, PivotGridWebOptionsPager.OptionsPagerChangedEventArgs e) {
			if(e.Reason == PivotGridWebOptionsPager.OptionsPagerChangedReason.PageIndex)
				DataOwner.PageIndexChanged();
			LayoutChanged();
		}
		public PivotGridFilterItems CreatePivotGridFilterItems(PivotGridField field) {
			return new PivotGridFilterItems(this, Fields[field.Index]);
		}
		public AppearanceStyle GetTableStyle() {
			AppearanceStyle style = Styles.GetMainTableStyle();
			MergeBorderWithControlBorder(style);
			return style;
		}
		void MergeBorderWithControlBorder(AppearanceStyleBase style) {
			AppearanceStyle controlStyle = PivotGrid.InternalControlStyle;
			if(style.Border.BorderColor.IsEmpty) {
				style.Border.BorderColor = controlStyle.Border.BorderColor;
			}
			if(style.Border.BorderStyle == System.Web.UI.WebControls.BorderStyle.NotSet) {
				style.Border.BorderStyle = controlStyle.Border.BorderStyle;
			}
			if(style.Border.BorderWidth.IsEmpty) {
				style.Border.BorderWidth = controlStyle.Border.BorderWidth;
			}
		}
		public PivotHeaderStyle GetHeaderStyle(PivotGridField field) {
			PivotHeaderStyle style = new PivotHeaderStyle();
			Styles.ApplyDefaultHeaderStyle(style);
			style.CopyFrom(Styles.HeaderStyle);
			style.CopyFrom(field.HeaderStyle);		
			return style;
		}
		public PivotHeaderStyle GetGroupButtonStyle(PivotGridField field){
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderGroupButtonStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderTextStyle(PivotGridField field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderTextStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderTableStyle(PivotGridField field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderTableStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderSortStyle(PivotGridField field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderSortStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderFilterStyle(PivotGridField field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderFilterStyle(style);
			return style;
		}
		public AppearanceSelectedStyle GetHeaderHoverStyle(PivotGridField field) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(Styles.GetDefaultHeaderHoverStyle());
			style.CopyFrom(Styles.HeaderStyle.HoverStyle);
			style.CopyFrom(field.HeaderStyle.HoverStyle);
			return style;
		}
		public PivotAreaStyle GetAreaStyle(PivotArea area) {
			PivotAreaStyle style = Styles.GetAreaStyle(area);
			MergeBorderWithControlBorder(style);
			if(!OptionsView.ShowDataHeaders && !OptionsView.ShowColumnHeaders && area == PivotArea.RowArea)
				style.BorderTop.BorderStyle = BorderStyle.None;
			return style;
		}
		public PivotAreaStyle GetEmptyAreaStyle(PivotArea area) {
			PivotAreaStyle style = GetAreaStyle(area);
			Styles.ApplyEmptyAreaStyle(style);
			return style;
		}
		public PivotFieldValueStyle GetFieldValueStyle(PivotFieldValueItem item, PivotGridField field) {
			PivotFieldValueStyle style = new PivotFieldValueStyle();
			Styles.ApplyFieldValueStyle(style, item.Area == PivotArea.ColumnArea, field);
			if(item.IsTotal)
				Styles.ApplyTotalFieldValueStyle(style, item.Area == PivotArea.ColumnArea, field);
			if(item.ValueType == PivotGridValueType.GrandTotal)
				Styles.ApplyGrandTotalFieldValueStyle(style, item.Area == PivotArea.ColumnArea);
			if(item.IsColumn && !item.ShowCollapsedButton && style.HorizontalAlign == HorizontalAlign.NotSet) 
				style.HorizontalAlign = HorizontalAlign.Center;
			MergeBorderWithControlBorder(style);
			return style;
		}
		public AppearanceStyle GetCollapsedButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			Styles.ApplyCollapsedButtonStyle(style);
			style.Cursor = RenderUtils.GetPointerCursor();
			return style;
		}
		public AppearanceStyle GetSortByColumnImageStyle() {
			AppearanceStyle style = new AppearanceStyle();
			Styles.ApplySortByColumnImageStyle(style);
			return style;
		}
		public void ApplyCellStyle(PivotGridCellItem cellItem, PivotCellStyle style) {
			Styles.ApplyCellStyle(cellItem.ColumnIndex == 0, cellItem.RowIndex == 0, style, cellItem.ShowKPIGraphic);
			if(cellItem.IsGrandTotalAppearance) 
				Styles.ApplyGrandTotalCellStyle(style);	
			else {
				if(cellItem.IsTotalAppearance)
					Styles.ApplyTotalCellStyle(style);	
				else {
					if(cellItem.IsCustomTotalAppearance)
						Styles.ApplyCustomTotalCellStyle(style);
				}
			}
			DataOwner.CustomCellStyle(cellItem, style);
			PivotGridField dataField = GetFieldByBaseField(cellItem.DataField);
			if(dataField != null) {
				style.CopyFrom(dataField.CellStyle);
			}
		}
		public Paddings GetAreaPaddings(PivotArea area, bool isFirst, bool isLast) {
			PivotAreaStyle style = GetAreaStyle(area);
			Paddings result = new Paddings();
			result.CopyFrom(Styles.GetAreaPaddings(area, isFirst, isLast));
			result.CopyFrom(style.Paddings);			
			return result;
		}
		public Paddings GetFilterButtonPanelPaddings() {
			PivotFilterButtonPanelStyle style = GetFilterButtonPanelStyle();
			Paddings result = new Paddings();
			result.CopyFrom(Styles.GetFilterButtonPanelPaddings(style.Font));
			result.CopyFrom(style.Paddings);
			return result;
		}
		public Unit GetFilterButtonPanelSpacing() {
			PivotFilterButtonPanelStyle style = GetFilterButtonPanelStyle();
			return !style.Spacing.IsEmpty ? style.Spacing : Styles.GetFilterButtonPanelSpacing(style.Font);
		}
		public PivotFilterStyle GetFilterWindowStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(Styles.GetFilterWindowStyle());
			style.CopyFrom(Styles.FilterWindowStyle);			
			return style;
		}
		public PivotFilterStyle GetFilterItemsAreaStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(Styles.GetFilterItemsAreaStyle());
			style.CopyFrom(Styles.FilterItemsAreaStyle);			
			return style;
		}
		public PivotFilterItemStyle GetFilterItemStyle() {
			PivotFilterItemStyle style = new PivotFilterItemStyle();
			style.CopyFrom(Styles.GetFilterItemStyle());
			style.CopyFrom(Styles.FilterItemStyle);
			return style;
		}
		public PivotFilterButtonStyle GetFilterButtonStyle() {
			PivotFilterButtonStyle style = new PivotFilterButtonStyle();
			style.CopyFrom(Styles.GetFilterButtonStyle());
			style.CopyFrom(Styles.FilterButtonStyle);
			return style;
		}
		public PivotFilterButtonPanelStyle GetFilterButtonPanelStyle() {
			PivotFilterButtonPanelStyle style = new PivotFilterButtonPanelStyle();
			style.CopyFrom(Styles.GetFilterButtonPanelStyle());
			style.CopyFrom(Styles.FilterButtonPanelStyle);
			return style;
		}
		public AppearanceStyle GetPagerStyle(bool isTopPager) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetPagerStyle(isTopPager));
			MergeBorderWithControlBorder(style);
			switch(OptionsPager.PagerAlign) {
				case PagerAlign.Left:
					style.HorizontalAlign = HorizontalAlign.Left;
					break;
				case PagerAlign.Right:
					style.HorizontalAlign = HorizontalAlign.Right;
					break;
				case PagerAlign.Center:
					style.HorizontalAlign = HorizontalAlign.Center;
					break;
				case PagerAlign.Justify:
					style.HorizontalAlign = HorizontalAlign.Justify;
					break;
			}
			return style;
		}
		public ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {headerTemplate = value; }
		}
		public ITemplate EmptyAreaTemplate {
			get { return emptyAreaTemplate; }
			set { emptyAreaTemplate = value; }
		}
		public ITemplate FieldValueTemplate {
			get { return fieldValueTemplate; }
			set { fieldValueTemplate = value; }
		}
		public ITemplate CellTemplate {
			get { return cellTemplate; }
			set { cellTemplate = value; }
		}
		public void TemplatesChanged() {
			if (DataOwner != null)
				DataOwner.ElementTemplatesChanged();
		}
		public Control SetupTemplateContainer(Control templateContainer, ITemplate template) {
			template.InstantiateIn(templateContainer);
			templateContainer.DataBind();
			return templateContainer;
		}
		public string GetCollapsedFieldValueChangeState(PivotFieldValueItem item) {
			if(!item.ShowCollapsedButton) return string.Empty;
			return (item.IsColumn ? PivotGridWebData.ExpandColumnChanged : PivotGridWebData.ExpandRowChanged) +
				PivotGridWebData.ArgumentsSeparator + item.UniqueIndex.ToString();
		}
		public const string CallbackHandler = "pivotGrid_OnCallback";
		public string GetCollapsedImageOnClick(PivotFieldValueItem item) {
			if (Owner == null || Page == null) return string.Empty;
			return string.Format("pivotGrid_PerformCallback('{0}', this, '{1}');",  ClientID, GetCollapsedFieldValueChangeState(item));
		}
		public void RestoreFieldsInGroups() {
			if(Groups == null || Fields == null)
				return;
			SetIsDeserializing(true);
			BeginUpdate();
			foreach(PivotGridField field in Fields) {
				if(field.GroupIndexCore == -1)
					continue;
				Groups[field.GroupIndexCore].Add(field);
			}
			foreach(PivotGridWebGroup group in Groups)
				group.SortFields();
			CancelUpdate();
			SetIsDeserializing(false);
		}
		public int SelectedFieldIndex {
			get {
				if (selectedFieldIndex > -1 && WebFields != null && selectedFieldIndex < WebFields.Count)
					return selectedFieldIndex;
				return -1;
			}
			set { selectedFieldIndex = value; }
		}
		public bool IsFieldSelected(PivotGridField field) {
			return field.Index == SelectedFieldIndex;
		}
		protected PivotGridFieldCollection WebFields {
			get {
				return DataOwner != null ? DataOwner.Fields as PivotGridFieldCollection : null;
			}
		}
		const string AreaId = "pgArea";
		const string HeaderId = "pgHeader";
		const string SortedHeaderId = "sorted";
		const string GroupID = "pgGroupHeader";
		public string GetHeaderTableID(PivotArea area) {
			return ElementName_AreaCellContainer + area.ToString();
		}
		public string GetID(PivotGridField field) {
			if(field.InnerGroupIndex == -1)
				return GetHeaderID(field);
			else
				return GetGroupHeaderID(field.Group);
		}
		public string GetHeaderID(PivotGridField field) {
			string sorted = field.ShowSortImage && field.Visible ? SortedHeaderId : string.Empty;
			return sorted + HeaderId + field.Index.ToString();
		}
		public string GetGroupHeaderID(PivotGridGroup group) {
			return GroupID + group.Index.ToString();
		}
		public string GetGroupButtonID(PivotGridField field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[3];
		}
		public string GetHeaderTextCellID(PivotGridField field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[0];
		}
		public string GetHeaderFilterCellID(PivotGridField field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[1];
		}
		public string GetHeaderSortCellID(PivotGridField field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[2];
		}
		public IDataSourceViewSchemaAccessor DataSourceViewSchemaAccessor {
			get { return dataSourceViewSchemaAccessor; }
			set { dataSourceViewSchemaAccessor = value; }
		}
		public string GetFullHeaderID(PivotGridField field) {
			return ClientID + '_' + GetHeaderID(field);
		}		
		public string GetAreaID(PivotArea area) {
			return AreaId + ((int)area).ToString();
		}
		public bool GetAreaByID(string areaClientId, out PivotArea area) {
			area = PivotArea.ColumnArea;
			int areaIndex = GetStringIndex(areaClientId, HeaderType.Area);
			if (areaIndex < 0 || areaIndex >= Enum.GetValues(typeof(PivotArea)).Length)
				return false;
			area = (PivotArea)areaIndex;
			return true;
		}
		public bool IsCustomizationFields(string clientId) {
			return clientId.Contains(ElementName_CustomizationFieldsId);
		}
		internal int GetStringIndex(string stringId, HeaderType headerType) {
			if(string.IsNullOrEmpty(stringId)) 
				return -1;
			stringId = GetLastPart(stringId);
			string idConst = GetIDConst(headerType);
			int index = stringId.IndexOf(idConst);
			int result = -1;
			if(index == -1)
				return result;
			int start = index + idConst.Length;
			int end = GetLastNumberIndex(stringId, start);
			if(int.TryParse(stringId.Substring(start, end + 1 - start), out result))
				return result;
			return -1;
		}
		protected string GetLastPart(string stringId) {
			int separatorIndex = stringId.LastIndexOf('_');
			return separatorIndex >= 0 ? stringId.Substring(separatorIndex + 1) : stringId;
		}
		int GetLastNumberIndex(string str, int startPosition) {
			string numbers = "-0123456789";
			int lastNumber = -1;
			for(int i = startPosition; i < str.Length; i++) {
				if(numbers.Contains(str[i].ToString()))
					lastNumber = i;
				else
					break;
			}
			return lastNumber;
		}
		string GetIDConst(HeaderType headerType) {
			switch(headerType) {				
				case HeaderType.Group:
					return GroupID;
				case HeaderType.Area:
					return AreaId;					
				case HeaderType.Header:
				default:
					return HeaderId;
			}
		}
		public string GetFilterButtonOnClick(PivotGridField field) {
			return string.Format("pivotGrid_ShowFilterPopup(\"{0}\", \"{1}\", {2});", ClientID, GetFullHeaderID(field), field.Index);
		}
		public string GetPagerOnClick(string id) {
			return string.Format("pivotGrid_PagerClick('{0}', this, '{1}');", ClientID, id);
		}
		public string GetGroupButtonOnClick(string id) {
			return string.Format("pivotGrid_PerformCallback('{0}', this, '{1}');", ClientID, 
				GroupExpanded.ToString() + ArgumentsSeparator + id);
		}
		public string GetWebFieldValuesState() {
			using(MemoryStream stream = new MemoryStream()) {
				using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true))
				using(BufferedStream buffered = new BufferedStream(compressor))
					WebSaveCollapsedStateToStream(buffered);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public void SetWebFieldValuesState(string value) {
			if(string.IsNullOrEmpty(value)) return;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				using(DeflateStream decompressor = new DeflateStream(stream, CompressionMode.Decompress))
					WebLoadCollapsedStateFromStream(decompressor);
			}
		}
		public string GetFieldValuesState() {
			MemoryStream stream = new MemoryStream();
			SaveCollapsedStateToStream(stream);
			return Convert.ToBase64String(stream.ToArray());
		}		
		public void SetFieldValuesState(string value) {
			if(string.IsNullOrEmpty(value)) return;
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
			LoadCollapsedStateFromStream(stream);
			stream.Close();
		}
		public string GetFilterValuesState() {
			if (ListSource == null && String.IsNullOrEmpty((DataOwner as ASPxPivotGrid).OLAPConnectionString)) return string.Empty; 
			MemoryStream stream = new MemoryStream();
			SaveFilterValuesToStream(stream);
			return Convert.ToBase64String(stream.ToArray());
		}		
		public bool SetFilterValueState(string value) {
			if(string.IsNullOrEmpty(value)) return false;
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
			LoadFilterValuesFromStream(stream);
			stream.Close();
			return true;
		}
		bool isDataBinding = false;
		protected override bool LockRefresh {
			get {
				return DataOwner != null ? !DataOwner.IsPivotDataCanDoRefresh : base.LockRefresh;
			}
		}
		protected override void DoRefreshCore() {
			if(isDataBinding) return;
			this.isDataBinding = true;
			if(IsOLAP && DelayFieldsGroupingByHierarchies && !fieldsWereGrouped) {
				Fields.GroupFieldsByHierarchies();
				fieldsWereGrouped = true;
			}
			if(DataOwner != null && !DataOwner.IsPivotDataCanDoRefresh)
				DataOwner.RequireUpdateData();
			else
				base.DoRefreshCore();
			this.isDataBinding = false;			
		}
		public bool IsRendering { get { return PivotGrid.IsRendering; } }
		protected override bool DelayFieldsGroupingByHierarchies { get { return true; } }
		protected override void LayoutChangedCore() {
			base.LayoutChangedCore();
			if(DataOwner != null) DataOwner.LayoutChanged();
			if(DataOwner != null && !DataOwner.IsPivotDataCanDoRefresh && !IsLoading) {
				DataOwner.RequireUpdateData();
			}
		}
		public PivotGridField GetFieldByBaseField(PivotGridFieldBase baseField) {
			return baseField as PivotGridField;
		}
		public PivotGridField[] GetFieldsByArea(PivotArea area) {
			List<PivotGridFieldBase> baseFields = GetFieldsByArea(area, true);
			PivotGridField[] fields = new PivotGridField[baseFields.Count];
			for(int i = 0; i < fields.Length; i++) {
				fields[i] = GetFieldByBaseField(baseFields[i]);
			}
			return fields;
		}
		public PivotGridField GetFieldByClientID(string name) {
			int index = GetStringIndex(name, HeaderType.Header);
			if(index >= 0) 
				return Fields[index];
			index = GetStringIndex(name, HeaderType.Group);
			if(index >= 0)
				return (PivotGridField)Groups[index].Fields[0];
			if(name.Contains(GetHeaderID(GetFieldByBaseField(DataField))))
				return GetFieldByBaseField(DataField);
			return null;
		}
		#region Events
		protected override object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {
			if(DataOwner != null)
				return DataOwner.GetUnboundValue(GetFieldByBaseField(field), listSourceRowIndex);
			else return null;
		}
		protected override void OnCalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			if(DataOwner != null)
				DataOwner.CalcCustomSummary(GetFieldByBaseField(field), customSummaryInfo);
		}
		protected override int GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			if(DataOwner != null)
				return DataOwner.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, GetFieldByBaseField(field), sortOrder);
			else return base.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		public override object GetCustomGroupInterval(PivotGridFieldBase field, object value) {
			if(DataOwner != null)
				return DataOwner.GetCustomGroupInterval(field, value);
			return base.GetCustomGroupInterval(field, value);
		}
		internal void OnFieldValueStateChanged(bool isColumn, object[] values) {
			PivotGridFieldBase baseField = GetFieldByLevel(isColumn, values.Length - 1);
			if(baseField == null) return;
			PivotGridField field = GetFieldByBaseField(baseField);
			if(field == null) return;
			bool isCollapsed = IsObjectCollapsed(isColumn, values);
			if(!BeforeFieldValueChangeExpanded(field, values, isCollapsed)) return;
			bool success = ChangeExpanded(isColumn, values);
			AfterFieldValueChangeExpanded(field, values, isCollapsed, success);
		}
		public override void OnFieldAreaChanged(PivotGridFieldBase field) {
			base.OnFieldAreaChanged(field);
			DataOwner.OnFieldAreaChanged(field);
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			DataOwner.OnDataSourceChanged();
		}
		public override void OnFieldAreaIndexChanged(PivotGridFieldBase field, bool doRefresh) {
			base.OnFieldAreaIndexChanged(field, doRefresh);
			DataOwner.OnFieldAreaIndexChanged(field);
		}
		public override void OnFieldVisibleChanged(PivotGridFieldBase field) {
			base.OnFieldVisibleChanged(field);
			DataOwner.OnFieldVisibleChanged(field);
		}
		public override void OnFieldFilteringChanged(PivotGridFieldBase field) {
			base.OnFieldFilteringChanged(field);
			DataOwner.OnFieldFilterChanged(field);
		}
		protected override void OLAPQueryTimeout() {
			base.OLAPQueryTimeout();
			DataOwner.OLAPQueryTimeout();
		}
		internal bool BeforeFieldValueChangeExpanded(PivotGridField field, object[] values, bool isCollapsed) {
			return DataOwner != null ? DataOwner.OnFieldValueStateChanging(field, values, isCollapsed) : true;
		}
		internal void AfterFieldValueChangeExpanded(PivotGridField field, object[] values, bool isCollapsed, bool success) {
			if(DataOwner != null) DataOwner.OnFieldValueStateChanged(field, values, isCollapsed, success);
		}
		internal string GetPivotFieldValueText(PivotFieldValueItem item) {
			return DataOwner != null ? DataOwner.GetFieldValueDisplayText(item) : item.Text;
		}
		internal string GetCellDisplayText(PivotGridCellItem cellItem) {
			return DataOwner != null ? DataOwner.CustomCellDisplayText(cellItem) : cellItem.Text;
		}
		public override string GetPivotFieldValueText(PivotGridFieldBase field, object value) {
			if(DataOwner != null && !Disposing)
				return DataOwner.GetFieldValueDisplayText(field, value);
			return base.GetPivotFieldValueText(field, value);
		}
		#endregion
		T IViewBagOwner.GetViewBagProperty<T>(string objectPath, string propertyName, T value) {
			IViewBagOwner viewBagOwner = Owner as IViewBagOwner;
			return viewBagOwner != null ? viewBagOwner.GetViewBagProperty(objectPath, propertyName, value): value;
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value) {
			IViewBagOwner viewBagOwner = Owner as IViewBagOwner;
			if(viewBagOwner != null) {
				viewBagOwner.SetViewBagProperty(objectPath, propertyName, defaultValue, value);
			}
		}
		public new void SetIsDeserializing(bool value) {
			base.SetIsDeserializing(value);
		}
	}
	public class PivotWebVisualItems : PivotVisualItemsBase {
		PivotFieldValueItemsCreator unpagedRowItemsCreator;
		public PivotWebVisualItems(PivotGridWebData data)
			: base(data) {
		}		
		protected new PivotGridWebData Data { get { return (PivotGridWebData)base.Data; } }
		protected PivotFieldValueItemsCreator UnpagedRowItemsCreator {
			get {
				if(unpagedRowItemsCreator == null)
					unpagedRowItemsCreator = CreateRowItemsCreator();
				return unpagedRowItemsCreator; 
			}
		}
		protected override PivotFieldValueItemsCreator RowItemsCreator {
			get {
				if(!HasPager)
					return UnpagedRowItemsCreator;
				return base.RowItemsCreator;
			}
		}
		protected bool HasPager { get { return Data.OptionsPager.HasPager; } }
		public int PageIndex { get { return Data.OptionsPager.PageIndex; } }
		public int PageStartRow {
			get {
				return PageIndex >= 0 ? PageIndex * Data.OptionsPager.RowsPerPage : 0;
			}
		}
		public int PageRowsCount {
			get { return PageIndex >= 0 ? Data.OptionsPager.RowsPerPage : 0; }
		}
		public int UnpagedRowCount {
			get { return UnpagedRowItemsCreator.LastLevelItemCount; }
		}
		protected PivotFieldValueItemsCreator GetItemsCreator(bool isColumn, bool paged) {
			if(isColumn) return ColumnItemsCreator;
			return paged ? RowItemsCreator : UnpagedRowItemsCreator;
		}
		public PivotFieldValueItem GetRowItem(int index, bool paged) {
			return GetItemsCreator(false, paged).GetLastLevelItem(index);
		}
		protected override void Calculate() {
			ColumnItemsCreator.CreateItems();			
			UnpagedRowItemsCreator.CreateItems();
			if(HasPager)
				RowItemsCreator.CreateItems(PageStartRow, PageRowsCount);
		}
		public override void Clear() {
			if(unpagedRowItemsCreator != null)
				ClearItemsCreator(unpagedRowItemsCreator);
			base.Clear();			
		}
		public object[] GetItemValues(bool isColumn, int uniqueIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return itemsCreator.GetItemValues(uniqueIndex);
		}
		public PivotGridFieldBase GetItemField(bool isColumn, int uniqueIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return itemsCreator.GetUnpagedItem(uniqueIndex).Field;
		}
		public PivotGridCellItem CreateCellItem(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, int columnIndex, int rowIndex) {
			return new PivotGridCellItem(CellDataProvider, columnItem, rowItem, columnIndex, rowIndex);
		}
		public PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex, bool paged) {
			return new PivotGridCellItem(CellDataProvider, GetColumnItem(columnIndex),
				GetRowItem(rowIndex, paged), columnIndex, rowIndex);
		}
		public int GetVisibleIndex(bool isColumn, int lastLevelIndex, bool paged) {
			PivotFieldValueItem item = GetLastLevelItem(isColumn, lastLevelIndex, paged);
			return item != null ? item.VisibleIndex : -1;
		}
		public PivotFieldValueItem GetUnpagedItem(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn, false);
			return GetItemCore(lastLevelIndex, level, itemsCreator);
		}
		public PivotFieldValueItem GetUnpagedItem(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(field.IsColumn, false);
			return GetItemCore(field, lastLevelIndex, itemsCreator);
		}
		public PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex, bool paged) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn, paged);
			return GetLastLevelItemCore(lastLevelIndex, itemsCreator);
		}
		public object GetUnpagedFieldValue(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return item != null ? item.Value : null;
		}
		public IOLAPMember GetUnpagedOLAPMember(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return GetOLAPMemberCore(field, item);
		}
		public bool IsUnpagedObjectCollapsed(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return item != null ? Data.IsObjectCollapsed(field.IsColumn, item.VisibleIndex) : false;
		}
		public object GetUnpagedCellValue(int columnIndex, int rowIndex) {
			return CellDataProvider.GetCellValue(GetColumnItem(columnIndex), GetRowItem(rowIndex, false));
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, bool paged) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex, paged);
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, dataIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, bool paged) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex, paged);
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex);
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex, bool paged) {
			return Data.CreateSummaryDataSource(GetVisibleIndex(true, columnIndex, paged), GetVisibleIndex(true, rowIndex, paged));
		}
		protected override void SavedFieldValueItemsStateCore(MemoryStream stream) {
			ColumnItemsCreator.SaveToStream(stream);
			UnpagedRowItemsCreator.SaveToStream(stream);
			if(HasPager)
				RowItemsCreator.SaveToStream(stream);
		}
		protected override void SavedDataCellsStateCore(MemoryStream stream) {
			CellDataProvider.SaveToStream(stream, ColumnItemsCreator, UnpagedRowItemsCreator);
		}
		protected override void LoadFieldValueItemsStateCore(MemoryStream stream) {
			ColumnItemsCreator.LoadFromStream(stream);
			UnpagedRowItemsCreator.LoadFromStream(stream);
			if(HasPager)
				RowItemsCreator.LoadFromStream(stream);
		}
		protected override void LoadDataCellsStateCore(MemoryStream stream) {
			StreamDataProvider.LoadFromStream(stream, ColumnItemsCreator, UnpagedRowItemsCreator);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebPrefilter : PrefilterBase {
		public WebPrefilter(IPrefilterOwnerBase owner)
			: base(owner) { }
		[
		Description(""), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(null),
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
		]
		public new CriteriaOperator Criteria {
			get { return base.Criteria; }
			set { base.Criteria = value; }
		}
		[
		Description(""),
		DefaultValue(""), NotifyParentProperty(true),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
		]
		public new string CriteriaString {
			get { return base.CriteriaString; }
			set { base.CriteriaString = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			Enabled = true;
			Criteria = null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			return Enabled != true || !object.ReferenceEquals(Criteria, null);
		}
	}
}
