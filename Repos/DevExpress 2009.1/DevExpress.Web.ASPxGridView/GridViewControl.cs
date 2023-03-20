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
using DevExpress.Web.Data;
using DevExpress.WebUtils;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Data;
using System.Collections;
using System.IO;
using DevExpress.Data.IO;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxGridView.Helper;
using DevExpress.Web.ASPxGridView.Design;
using DevExpress.Web.ASPxGridView.Cookies;
using DevExpress.Web.ASPxEditors;
using System.Collections.Specialized;
using System.Data;
using DevExpress.Data.Helpers;
using DevExpress.Web.ASPxEditors.Internal;
using DevExpress.Web.ASPxEditors.FilterControl;
namespace DevExpress.Web.ASPxGridView {
	[DevExpress.Utils.Design.DXClientDocumentationProvider("DevExpress.ASPxGridView/DevExpressWebASPxGridViewScripts.htm"),
	Designer(typeof(DevExpress.Web.ASPxGridView.Design.GridViewDesigner)),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)
]
	public class ASPxGridView : ASPxDataWebControl, IWebDataOwner, IWebControlPageSettings, IWebDataEvents,
		IDataControllerSort, IWebColumnsOwner, IRequiresLoadPostDataControl, IPopupFilterControlOwner, IPopupFilterControlStyleOwner {
		private string[] ChildControlNames = new string[] { "Web", "Editors" };
		public const string GridViewResourcePath = "DevExpress.Web.ASPxGridView.";
		public const string GridViewScriptResourcePath = GridViewResourcePath + "Scripts.";
		public const string GridViewScriptResourceName = GridViewScriptResourcePath + "GridView.js";
		public const string GridViewTableColumnResizingResourceName = GridViewScriptResourcePath + "TableColumnResizing.js";
		public const string WebResourceImagePath = GridViewResourcePath + "Images.";
		public const string WebResourceCssDefaultPath = GridViewResourcePath + "Css.default.css";
		GridViewEventsHelper pendingEvents;
		bool fireFocusedRowChangedOnClient = false, fireSelectionChangedOnClient = false;
		CallbackInfo internalCallbackInfo;
		ASPxGridViewRenderHelper renderHelper;
		Table rootTable;
		GridViewContainerControl containerControl;
		WebDataProxy dataProxy;
		GridViewFilterHelper filterHelper;
		GridViewEditorImages imagesEditors;
		FilterControlImages imagesFilterControl;
		GridViewEditorStyles stylesEditors;
		FilterControlStyles stylesFilterControl;
		GridViewPagerStyles stylesPager;
		ASPxGridViewPagerSettings settingsPager;
		ASPxGridViewEditingSettings settingsEditing;
		ASPxGridViewBehaviorSettings settingsBehavior;
		ASPxGridViewSettings settings;
		ASPxGridViewTextSettings settingsText;
		ASPxGridViewCustomizationWindowSettings settingsCustomizationWindow;
		ASPxGridViewCookiesSettings settingsCookies;
		ASPxGridViewDetailSettings settingsDetail;
		GridViewColumnCollection columns;
		List<GridViewDataColumn> sortedColums;
		Dictionary<string, ASPxGridCallBackMethod> callBacks;
		Dictionary<string, ASPxGridCallBackFunction> internalCallBacks;
		GridViewTemplates templates;
		WebColumnsOwnerGridViewImplementation webColumnsOwnerImpl;
		int lockUpdate;
		private static readonly object processColumnAutoFilter = new object();
		private static readonly object beforePerformDataSelect = new object();
		private static readonly object cellEditorInitialize = new object();
		private static readonly object autoFilterCellEditorInitialize = new object();
		private static readonly object autoFilterCellEditorCreate = new object();
		private static readonly object headerFilterFillItems = new object();
		private static readonly object pageIndexChanged = new object();
		private static readonly object rowCommand = new object();
		private static readonly object htmlRowCreated = new object();
		private static readonly object htmlRowPrepared = new object();
		private static readonly object htmlDataCellPrepared = new object();
		private static readonly object htmlCommandCellPrepared = new object();
		private static readonly object focusedRowChanged = new object();
		private static readonly object startRowEditing = new object();
		private static readonly object cancelRowEditing = new object();
		private static readonly object rowUpdating = new object();
		private static readonly object rowUpdated = new object();
		private static readonly object rowDeleting = new object();
		private static readonly object rowDeleted = new object();
		private static readonly object parseValue = new object();
		private static readonly object rowValidating = new object();
		private static readonly object initNewRow = new object();
		private static readonly object rowInserted = new object();
		private static readonly object rowInserting = new object();
		private static readonly object customErrorText = new object();
		private static readonly object customCallback = new object();
		private static readonly object customButtonCallback = new object();
		private static readonly object customDataCallback = new object();
		private static readonly object afterPerformCallback = new object();
		private static readonly object customUnboundColumnData = new object();
		private static readonly object customSummaryCalculate = new object();
		private static readonly object summaryDisplayText = new object();
		private static readonly object customColumnDisplayText = new object();
		private static readonly object customGroupDisplayText = new object();
		private static readonly object selectionChanged = new object();
		private static readonly object detailsChanged = new object();
		private static readonly object detailRowGetButtonVisibility = new object();
		private static readonly object clientLayout = new object();
		private static readonly object customColumnSort = new object();
		private static readonly object customColumnGroup = new object();
		private static readonly object beforeColumnSortingGrouping = new object();
		private static readonly object beforeGetCallbackResult = new object();
		private static readonly object htmlEditFormCreated = new object();
		private static readonly object htmlFooterCellPrepared = new object();
		private static readonly object detailRowExpandedChanged = new object();
		private static readonly object customButtonInitialize = new object();
		private static readonly object commandButtonInitialize = new object();
		private static readonly object customFilterExpressionDisplayText = new object();
		public static object GetDetailRowKeyValue(Control control) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			return container != null ? container.KeyValue : null;
		}
		public static object GetDetailRowValues(Control control, params string[] fieldNames) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			if (container == null) return null;
			return container.Grid.GetRowValues(container.VisibleIndex, fieldNames);
		}
		protected static ASPxGridView FindParentGrid(Control control) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			return container != null ? container.Grid : null;
		}
		protected static GridViewBaseRowTemplateContainer FindParentGridTemplateContainer(Control control) {
			if (control == null) return null;
			GridViewBaseRowTemplateContainer container = null;
			while (control.Parent != null) {
				container = control.Parent as GridViewBaseRowTemplateContainer;
				if (container != null) break;
				control = control.Parent;
			}
			return container;
		}
		public ASPxGridView() {
			this.pendingEvents = new GridViewEventsHelper();
			this.filterHelper = new GridViewFilterHelper(this);
			this.dataProxy = CreateDataProxy();
			DataProxy.OwnerDataBinding = new WebDataProxyOwnerInvoker(DataBindNoControls);
			TotalSummary.SummaryChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			GroupSummary.SummaryChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			GroupSummarySortInfo.SummaryChanged += new CollectionChangeEventHandler(OnGroupSummaryChanged);
			this.templates = new GridViewTemplates(this);
			this.imagesEditors = new GridViewEditorImages(this);
			this.stylesEditors = new GridViewEditorStyles(this);
			this.stylesPager = new GridViewPagerStyles(this);
			this.settingsPager = new ASPxGridViewPagerSettings(this);
			this.settingsEditing = new ASPxGridViewEditingSettings(this);
			this.settings = new ASPxGridViewSettings(this);
			this.settingsBehavior = new ASPxGridViewBehaviorSettings(this);
			this.settingsCustomizationWindow = new ASPxGridViewCustomizationWindowSettings(this);
			this.settingsCookies = new ASPxGridViewCookiesSettings(this);
			this.settingsDetail = new ASPxGridViewDetailSettings(this);
			this.imagesFilterControl = new FilterControlImages(this);
			this.stylesFilterControl = new FilterControlStyles(this);
			EnableCallBacks = true;
			EnableCallBacksInternal = true;
			this.columns = CreateColumnCollection();
			this.sortedColums = new List<GridViewDataColumn>();
			EnableClientSideAPIInternal = true; 
			this.webColumnsOwnerImpl = new WebColumnsOwnerGridViewImplementation(this, Columns);
		}
		public override void Dispose() {
			DataProxy.Dispose();
			base.Dispose();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceDataRowType(Type type) {
			DataProxy.ForceDataRowType(type);
		}
		protected override string GetSkinControlName() { return "GridView"; }
		protected override string[] GetChildControlNames() {
			return ChildControlNames;
		}
		[Description("Gets an object that lists the client-side events specific to the ASPxGridView."),
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public GridViewClientSideEvents ClientSideEvents {
			get { return (GridViewClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Description("Enables you to supply any server data that can then be parsed on the client. "),
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new GridViewClientSideEvents();
		}
		protected internal GridViewFilterHelper FilterHelper { get { return filterHelper; } }
		protected internal ASPxGridViewRenderHelper RenderHelper {
			get {
				if (renderHelper == null) renderHelper = CreateRenderHelper();
				return renderHelper;
			}
		}
		protected GridViewEventsHelper PendingEvents { get { return pendingEvents; } }
		protected virtual ASPxGridViewRenderHelper CreateRenderHelper() {
			return new ASPxGridViewRenderHelper(this);
		}
		protected virtual WebDataProxy CreateDataProxy() { return new WebDataProxy(this, this, this); }
		protected internal WebDataProxy DataProxy { get { return dataProxy; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public WebDataProxy DataBoundProxy {
			get {
				DataBindNoControls();
				return DataProxy;
			}
		}
		public ReadOnlyCollection<GridViewDataColumn> GetSortedColumns() { return SortedColumns.AsReadOnly(); }
		public ReadOnlyCollection<GridViewDataColumn> GetGroupedColumns() {
			List<GridViewDataColumn> result = new List<GridViewDataColumn>();
			int count = Math.Min(GroupCount, SortedColumns.Count);
			for (int i = 0; i < count; i++) {
				result.Add(SortedColumns[i]);
			}
			return result.AsReadOnly();
		}
		protected internal List<GridViewDataColumn> SortedColumns { get { return sortedColums; } }
		protected Table RootTable { get { return rootTable; } }
		protected TableCell RootCell {
			get {
				if (RootTable == null) return null;
				return RootTable.Rows[0].Cells[0];
			}
		}
		protected internal bool GetServerMode() {
			return DataProxy.IsServerMode;
		}
		protected GridViewContainerControl ContainerControl { get { return containerControl; } }
		protected virtual void ResetVisibleColumns() {
			this.webColumnsOwnerImpl.ResetVisibleColumns();
		}
		protected internal List<GridViewColumn> GetVisibleColumns() {
			List<GridViewColumn> res = new List<GridViewColumn>();
			List<WebColumnBase> columns = this.webColumnsOwnerImpl.GetVisibleColumns();
			for (int i = 0; i < columns.Count; i++) {
				res.Add(columns[i] as GridViewColumn);
			}
			return res;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<GridViewColumn> GetColumnsShownInHeaders() {
			List<GridViewColumn> result;
			if (Settings.ShowGroupedColumns || GroupCount == 0) {
				result = GetVisibleColumns();
			} else {
				result = new List<GridViewColumn>();
				foreach (GridViewColumn col in GetVisibleColumns()) {
					GridViewDataColumn dc = col as GridViewDataColumn;
					if (dc != null && dc.GroupIndex > -1) continue;
					result.Add(col);
				}
			}
			if (SettingsBehavior.ColumnResizeMode == ColumnResizeMode.NextColumn) {
				int count = result.Count;
				if (count > 0)
					result[count - 1].ResetWidth();
			}
			return result;
		}		
		[Browsable(false)]
		public int FixedColumnCount { get { return this.webColumnsOwnerImpl.FixedColumnCount; } }
		[Browsable(false)]
		public int VisibleRowCount { get { return DataProxy.VisibleRowCount; } }
		[Browsable(false)]
		public int VisibleStartIndex { get { return DataProxy.VisibleStartIndex; } }
		[Browsable(false)]
		public WebDataDetailRows DetailRows { get { return DataProxy.DetailRows; } }
		[Browsable(false)]
		public WebDataSelection Selection { get { return DataProxy.Selection; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FocusedRowIndex { get { return DataProxy.FocusedRowVisibleIndex; } set { DataProxy.FocusedRowVisibleIndex = value; } }
		public object GetRow(int visibleIndex) {
			DataBindNoControls();
			return DataProxy.GetRow(visibleIndex);
		}
		public DataRow GetDataRow(int visibleIndex) {
			DataRowView rowView = GetRow(visibleIndex) as DataRowView;
			return rowView != null ? rowView.Row : null;
		}
		public int GetChildRowCount(int groupRowVisibleIndex) {
			return DataProxy.GetChildRowCount(groupRowVisibleIndex);
		}
		public object GetChildRow(int groupRowVisibleIndex, int childIndex) {
			return DataProxy.GetChildRow(groupRowVisibleIndex, childIndex);
		}
		public DataRow GetChildDataRow(int groupRowVisibleIndex, int childIndex) {
			DataRowView rowView = DataProxy.GetChildRow(groupRowVisibleIndex, childIndex) as DataRowView;
			return rowView != null ? rowView.Row : null;
		}
		public object GetChildRowValues(int groupRowVisibleIndex, int childIndex, params string[] fieldNames) {
			return DataProxy.GetChildRowValues(groupRowVisibleIndex, childIndex, fieldNames);
		}
		public bool IsGroupRow(int visibleIndex) {
			return DataProxy.GetRowType(visibleIndex) == WebRowType.Group;
		}
		public List<object> GetSelectedFieldValues(params string[] fieldNames) {
			return DataProxy.GetSelectedValues(fieldNames);
		}
		public int GetRowLevel(int visibleIndex) { return DataProxy.GetRowLevel(visibleIndex); }
		public object GetRowValues(int visibleIndex, params string[] fieldNames) {
			return DataProxy.GetValues(visibleIndex, fieldNames);
		}
		public object GetRowValuesByKeyValue(object keyValue, params string[] fieldNames) {
			return DataProxy.GetValuesByKeyValue(keyValue, fieldNames);
		}
		public int FindVisibleIndexByKeyValue(object keyValue) {
			return DataProxy.FindVisibleIndexByKey(keyValue, false);
		}
		public List<object> GetCurrentPageRowValues(params string[] fieldNames) {
			return DataProxy.GetCurrentPageRowValues(fieldNames);
		}
		public bool MakeRowVisible(object keyValue) {
			return DataProxy.MakeRowVisible(keyValue);
		}
		[Description("Gets or sets the ASPxGridView's client programmatic identifier."),
		Category("Client-Side"), AutoFormatDisable, DefaultValue(""), Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[Description("Gets or sets a value that specifies the initial visibility state of a web control on the client."),
		Category("Client-Side"), AutoFormatDisable, DefaultValue(true), Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[Browsable(false)]
		public override bool EnableDefaultAppearance {
			get { return base.EnableDefaultAppearance; }
			set { base.EnableDefaultAppearance = value; }
		}
		[Browsable(false)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Description("Gets or sets a value that specifies whether the callback or postback technology is used to manage round trips to the server."),
		DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableCallBacks {
			get { return GetBoolProperty("EnableCallBacks", false); }
			set {
				SetBoolProperty("EnableCallBacks", false, value);
				LayoutChanged();
			}
		}
		[Description("Gets or sets whether callback compression is enabled."),
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get {
				return base.Height;
			}
			set { }
		}
		[Browsable(false)]
		public ReadOnlyCollection<GridViewColumn> VisibleColumns { get { return GetVisibleColumns().AsReadOnly(); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[Browsable(false)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		[Browsable(false), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PageIndex {
			get { return CheckPageIndex((int)GetCallbackPropertyValue("PageIndex", 0)); }
			set {
				value = CheckPageIndex(value);
				if (PageIndex == value) return;
				SetCallbackPropertyValue("PageIndex", 0, value);
				OnPageIndexChanged();
			}
		}
		[Browsable(false), AutoFormatDisable]
		public int PageCount { get { return DataProxy.PageCount; } }
		[Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string FilterExpression {
			get { return (string)GetCallbackPropertyValue("FilterExpression", string.Empty); }
			set {
				value = CheckFilterExpression(value);
				if (FilterExpression == value) return;
				OnFilterExpressionChanging(value, true);
			}
		}
		[Browsable(false), DefaultValue(true), AutoFormatDisable]
		public bool FilterEnabled {
			get { return (bool)GetCallbackPropertyValue("FilterEnabled", true); }
			set {
				if (FilterEnabled == value) return;
				OnFilterExpressionChanging(FilterExpression, value);
			}
		}
		protected string GetEnabledFilterExpression() { return FilterEnabled ? FilterExpression : string.Empty; }
		[Description("Gets or sets the name of the data source key field."),
		DefaultValue(""), Category("Data"), Localizable(false),
		TypeConverter(typeof(GridViewFieldConverter)), AutoFormatDisable]
		public string KeyFieldName {
			get { return GetStringProperty("KeyFieldName", string.Empty); }
			set {
				if (value == null) value = string.Empty;
				SetStringProperty("KeyFieldName", string.Empty, value);
				DataProxy.KeyFieldName = value;
			}
		}
		[Description("Gets or sets the name of the data source field whose values are displayed within preview rows."),
		DefaultValue(""), Category("Data"), Localizable(false),
		TypeConverter(typeof(GridViewFieldConverter)), AutoFormatDisable]
		public string PreviewFieldName {
			get { return GetStringProperty("PreviewFieldName", string.Empty); }
			set {
				if (value == null) value = string.Empty;
				SetStringProperty("PreviewFieldName", string.Empty, value);
			}
		}
		[Description("Gets or sets whether columns are automatically created for all fields in the underlying data source."),
		DefaultValue(true), Category("Data"), AutoFormatDisable]
		public bool AutoGenerateColumns {
			get { return GetBoolProperty("AutoGenerateColumns", true); }
			set { SetBoolProperty("AutoGenerateColumns", true, value); }
		}
		[Description("Gets or sets whether data caching is enabled."),
		DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableRowsCache {
			get { return GetBoolProperty("EnableRowsCache", true); }
			set {
				SetBoolProperty("EnableRowsCache", true, value);
			}
		}
		void OnFilterExpressionChanging(string value, bool isFilterEnabled) {
			LayoutChanged();
			this.columnFilterInfo = null;
			SetCallbackPropertyValue("FilterExpression", string.Empty, value);
			SetCallbackPropertyValue("FilterEnabled", true, isFilterEnabled);
			BindAndSynchronizeDataProxy();
		}
		string CheckFilterExpression(string value) {
			if (value == null) value = string.Empty;
			return value.Trim();
		}
		[Description("Provides access to total summary items."),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable,
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public ASPxSummaryItemCollection TotalSummary { get { return DataProxy.TotalSummary; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxGroupSummarySortInfoCollection GroupSummarySortInfo { get { return DataProxy.GroupSummarySortInfo; } }
		[Description("Provides access to group summary items."),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable,
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public ASPxSummaryItemCollection GroupSummary { get { return DataProxy.GroupSummary; } }
		bool ShouldSerializeColumns() {
			if (Columns.Count == 0 || AutoGenerateColumns) return false;
			return true;
		}
		[Description("Provides access to an ASPxGridView's column collection."),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable,
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public GridViewColumnCollection Columns { get { return this.columns; } }
		internal List<GridViewDataColumn> DataColumns {
			get { return GetDataColumns(false); }
		}
		private IList<GridViewDataColumn> VisibleDataColumns {
			get { return GetDataColumns(true).AsReadOnly(); }
		}
		private List<GridViewDataColumn> GetDataColumns(bool onlyVisible) {
			List<GridViewDataColumn> list = new List<GridViewDataColumn>();
			foreach (GridViewColumn column in Columns) {
				GridViewDataColumn dataColumn = column as GridViewDataColumn;
				if (dataColumn != null && (!onlyVisible || column.Visible))
					list.Add(dataColumn);
			}
			return list;
		}
		[Description("Provides access to the control's behavior settings."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewBehaviorSettings SettingsBehavior { get { return settingsBehavior; } }
		[Description("Provides access to the Pager's settings."),
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewPagerSettings SettingsPager { get { return settingsPager; } }
		[Description("Provides access to the ASPxGridView's editing settings."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewEditingSettings SettingsEditing { get { return settingsEditing; } }
		[Description("Provides access to the ASPxGridView's display options."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewSettings Settings { get { return settings; } }
		[Description("Provides access to the control's text settings."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewTextSettings SettingsText {
			get {
				if (settingsText == null)
					settingsText = CreateSettingsText();
				return settingsText;
			}
		}
		[Description("Provides access to the Customization Window's settings."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow { get { return settingsCustomizationWindow; } }
		[Description("Provides access to the Loading Panel's settings."),
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return (ASPxGridViewLoadingPanelSettings)base.SettingsLoadingPanel; } }
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new ASPxGridViewLoadingPanelSettings(this);
		}
		[Description("Provides access to the control's cookie settings."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewCookiesSettings SettingsCookies { get { return settingsCookies; } }
		[Description("Provides access to the ASPxGridView's master-detail options."),
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewDetailSettings SettingsDetail { get { return settingsDetail; } }
		[Description("Gets or sets whether the default data source paging is used."),
		DefaultValue(false), AutoFormatDisable]
		public bool DataSourceForceStandardPaging {
			get { return GetBoolProperty("DataSourceForceStandardPaging", false); }
			set {
				if (DataSourceForceStandardPaging == value) return;
				SetBoolProperty("DataSourceForceStandardPaging", false, value);
				if (DesignMode && !IsLoading() && value) {
					GridViewAboutDialogHelper.DisplayDataSourceWarningMessage();
				}
			}
		}
		[Description("Gets the padding settings of an ASpxGridView control."),
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings { get { return (ControlStyle as AppearanceStyle).Paddings; } }
		protected internal string EditTemplateValidationGroup { get { return ClientID; } }
		[Description("Enables support for Section 508."),
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[Description("Gets or sets the text to render in an HTML caption element in an ASPxGridView."),
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[Description("Gets or sets a value that describes the ASPxGridView's contents."),
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable,
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SummaryText {
			get { return GetStringProperty("SummaryText", ""); }
			set {
				if (value == SummaryText) return;
				if (value != null)
					value = value.Replace('\n', ' ').Replace("\r", "");
				SetStringProperty("SummaryText", "", value);
			}
		}
		protected virtual ASPxGridViewTextSettings CreateSettingsText() {
			return new ASPxGridViewTextSettings(this);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		public object GetMasterRowKeyValue() {
			return ASPxGridView.GetDetailRowKeyValue(this);
		}
		public object GetMasterRowFieldValues(params string[] fieldNames) {
			return ASPxGridView.GetDetailRowValues(this, fieldNames);
		}
		[Browsable(false)]
		public int SortCount { get { return SortedColumns.Count; } }
		[Browsable(false)]
		public int GroupCount {
			get {
				int res = 0;
				for (int i = 0; i < SortedColumns.Count; i++) {
					if (SortedColumns[i].GroupIndex > -1) res++;
				}
				return res;
			}
		}
		public Control FindDetailRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.DetailRowTemplates.FindChild(new TemplateContainerFinder(FindDetailRowTemplate), visibleIndex, id);
		}
		public Control FindPreviewRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.PreviewRowTemplates.FindChild(new TemplateContainerFinder(FindPreviewRowTemplate), visibleIndex, id);
		}
		public Control FindGroupRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.GroupRowTemplates.FindChild(new TemplateContainerFinder(FindGroupRowTemplate), visibleIndex, id);
		}
		public Control FindRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.DataRowTemplates.FindChild(new TemplateContainerFinder(FindDataRowTemplateByVIndex), visibleIndex, id);
		}
		public Control FindRowTemplateControlByKey(object rowKey, string id) {
			return RenderHelper.DataRowTemplates.FindChild(new TemplateContainerFinder(FindDataRowTemplateByKey), rowKey, id);
		}
		public Control FindRowCellTemplateControl(int visibleIndex, GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.RowCellTemplates.FindChild(new TemplateContainerFinder(FindRowTemplateByVIndex), new RowFindParams(gridViewDataColumn, visibleIndex), id);
		}
		public Control FindRowCellTemplateControlByKey(object rowKey, GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.RowCellTemplates.FindChild(new TemplateContainerFinder(FindRowTemplateByKey), new RowFindParams(gridViewDataColumn, rowKey), id);
		}
		public Control FindEmptyDataRowTemplateControl(string id) {
			return RenderHelper.EmptyDataRowTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindFooterRowTemplateControl(string id) {
			return RenderHelper.FooterRowTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindFooterCellTemplateControl(GridViewColumn column, string id) {
			return RenderHelper.FooterCellTemplates.FindChild(new TemplateContainerFinder(FindFooterCellTemplate), column, id);
		}
		public Control FindEditRowCellTemplateControl(GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.EditRowCellTemplates.FindChild(new TemplateContainerFinder(FindEditRowCellTemplate), gridViewDataColumn, id);
		}
		public Control FindHeaderTemplateControl(GridViewColumn column, string id) {
			return RenderHelper.HeaderTemplates.FindChild(new TemplateContainerFinder(FindHeaderTemplate), column, id);
		}
		public Control FindTitleTemplateControl(string id) {
			return RenderHelper.TitleTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindStatusBarTemplateControl(string id) {
			return RenderHelper.StatusBarTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindPagerBarTemplateControl(string id, GridViewPagerBarPosition position) {
			return RenderHelper.PagerBarTemplates.FindChild(new TemplateContainerFinder(FindPagerBarTemplate), position, id);
		}
		public Control FindEditFormTemplateControl(string id) {
			return RenderHelper.EditFormTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		Control FindDataRowTemplateByVIndex(Control container, object parameters, string id) {
			GridViewDataRowTemplateContainer row = (GridViewDataRowTemplateContainer)container;
			if ((row.VisibleIndex != (int)parameters)) return null;
			return row.FindControl(id);
		}
		Control FindDataRowTemplateByKey(Control container, object parameters, string id) {
			GridViewDataRowTemplateContainer row = (GridViewDataRowTemplateContainer)container;
			if (!object.Equals(row.KeyValue, parameters)) return null;
			return row.FindControl(id);
		}
		Control FindDetailRowTemplate(Control container, object parameters, string id) {
			GridViewDetailRowTemplateContainer row = (GridViewDetailRowTemplateContainer)container;
			if (row.VisibleIndex != (int)parameters) return null;
			return row.FindControl(id);
		}
		Control FindPreviewRowTemplate(Control container, object parameters, string id) {
			GridViewPreviewRowTemplateContainer row = (GridViewPreviewRowTemplateContainer)container;
			if (row.VisibleIndex != (int)parameters) return null;
			return row.FindControl(id);
		}
		Control FindGroupRowTemplate(Control container, object parameters, string id) {
			GridViewGroupRowTemplateContainer group = (GridViewGroupRowTemplateContainer)container;
			if (group.VisibleIndex != (int)parameters) return null;
			return group.FindControl(id);
		}
		class RowFindParams {
			public object Key;
			public GridViewColumn Column;
			public RowFindParams(GridViewColumn column, object key) {
				this.Column = column;
				this.Key = key;
			}
		}
		Control FindRowTemplateByVIndex(Control container, object parameters, string id) {
			RowFindParams findParams = (RowFindParams)parameters;
			GridViewDataItemTemplateContainer item = (GridViewDataItemTemplateContainer)container;
			if ((findParams.Column == null || findParams.Column == item.Column) && item.VisibleIndex == (int)findParams.Key) return item.FindControl(id);
			return null;
		}
		Control FindRowTemplateByKey(Control container, object parameters, string id) {
			RowFindParams findParams = (RowFindParams)parameters;
			GridViewDataItemTemplateContainer item = (GridViewDataItemTemplateContainer)container;
			if (findParams.Column == item.Column && object.Equals(item.KeyValue, findParams.Key)) return item.FindControl(id);
			return null;
		}
		Control FindEditRowCellTemplate(Control container, object parameters, string id) {
			GridViewEditItemTemplateContainer item = (GridViewEditItemTemplateContainer)container;
			if (item.Column != parameters) return null;
			if (id == null) return item;
			return item.FindControl(id);
		}
		Control FindHeaderTemplate(Control container, object parameters, string id) {
			GridViewHeaderTemplateContainer header = (GridViewHeaderTemplateContainer)container;
			if (header.Column != parameters) return null;
			return header.FindControl(id);
		}
		Control FindFooterCellTemplate(Control container, object parameters, string id) {
			GridViewFooterCellTemplateContainer footerCell = (GridViewFooterCellTemplateContainer)container;
			if (footerCell.Column != parameters) return null;
			return footerCell.FindControl(id);
		}
		Control FindPagerBarTemplate(Control container, object parameters, string id) {
			GridViewPagerBarTemplateContainer pagerBar = (GridViewPagerBarTemplateContainer)container;
			if (pagerBar.Position != (GridViewPagerBarPosition)parameters) return null;
			return pagerBar.FindControl(id);
		}
		Control FindSingleControlTemplate(Control container, object parameters, string id) {
			return container.FindControl(id);
		}
		public void UnGroup(GridViewColumn column) {
			GroupBy(column, -1);
		}
		public int GroupBy(GridViewColumn column) {
			return GroupBy(column, GroupCount);
		}
		public int GroupBy(GridViewColumn column, int value) {
			GridViewDataColumn dcColumn = column as GridViewDataColumn;
			if (column != null && dcColumn == null) new ArgumentException("Column should be GridViewDataColumn", "column");
			if (dcColumn == null) throw new ArgumentNullException("column");
			ColumnSortOrder order;
			if (value == -1)
				order = dcColumn.UngroupedSortOrder;
			else {
				order = dcColumn.SortOrder;
				if (order == ColumnSortOrder.None)
					order = ColumnSortOrder.Ascending;
			}
			return SortedColumnsChanged(dcColumn, value, true, order);
		}
		public void DoRowValidation() {
			if (IsEditing) {
				DataProxy.ValidateRow(true);
				LayoutChanged();
			}
		}
		public void StartEdit(int visibleIndex) {
			LoadDataIfNotBinded(true);
			LayoutChanged(); 
			DataProxy.StartEdit(visibleIndex);
		}
		public void UpdateEdit() {
			if (!DataProxy.IsEditing) return;
			LoadDataIfNotBinded(true);
			if (SettingsDetail.IsDetailGrid) EnsureChildControls(); 
			RenderHelper.ParseEditorValues();
			if (DataProxy.EndEdit()) {
				RequireDataBinding();
				DataBind();
			} else {
				LayoutChanged();
			}
		}
		public void CancelEdit() {
			DataProxy.CancelEdit();
			LayoutChanged();
		}
		public void AddNewRow() {
			LoadDataIfNotBinded(true);
			LayoutChanged(); 
			DataProxy.AddNewRow();
		}
		public void DeleteRow(int visibleIndex) {
			LoadDataIfNotBinded(true);
			DataProxy.DeleteRow(visibleIndex);
			RequireDataBinding();
			DataBind();
			DataProxy.CheckFocusedRowChanged(); 
		}
		public void ShowFilterControl() { ShowFilterControl(true); }
		public void HideFilterControl() { ShowFilterControl(false); }
		protected void ShowFilterControl(bool show) {
			if (IsFilterControlVisible == show) return;
			SetCallbackPropertyValue("IsFilterControlShowing", false, show);
			LayoutChanged();
		}
		[Browsable(false)]
		public bool IsFilterControlVisible {
			get { return (bool)GetCallbackPropertyValue("IsFilterControlShowing", false); }
		}
		protected virtual Dictionary<string, object> GetEditTemplateValuesCore() {
			if (!IsEditing) return null;
			Dictionary<object, ITemplate> editItems = new Dictionary<object, ITemplate>();
			foreach (GridViewDataColumn column in DataColumns) {
				if (column.EditItemTemplate == null) continue;
				editItems[column] = column.EditItemTemplate;
			}
			Dictionary<string, object> cellValues = RenderHelper.EditRowCellTemplates.FindTwoWayBindings(editItems, new TemplateContainerFinder(FindEditRowCellTemplate));
			Dictionary<string, object> formValues = RenderHelper.EditFormTemplates.FindTwoWayBindings(Templates.EditForm);
			return MergeDictionaries(cellValues, formValues);
		}
		Dictionary<string, object> MergeDictionaries(params Dictionary<string, object>[] dicts) {
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach (Dictionary<string, object> dict in dicts) {
				if (dict == null) continue;
				foreach (KeyValuePair<string, object> pair in dict) {
					res[pair.Key] = pair.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
		[Browsable(false)]
		public bool IsEditing { get { return DataProxy.IsEditing; } }
		[Browsable(false)]
		public bool IsNewRowEditing { get { return DataProxy.IsNewRowEditing; } }
		[Browsable(false)]
		public int EditingRowVisibleIndex { get { return DataProxy.EditingRowVisibleIndex; } }
		public virtual void BeginUpdate() {
			this.lockUpdate++;
		}
		public virtual void EndUpdate() {
			if (--this.lockUpdate == 0) OnEndUpdate();
		}
		[Browsable(false)]
		public virtual bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected virtual void OnEndUpdate() {
			CheckBindAndSynchronizeDataProxy();
		}
		public virtual string GetPreviewText(int visibleIndex) {
			return DataProxy.GetRowDisplayText(visibleIndex, PreviewFieldName);
		}
		public string GetGroupRowSummaryText(int visibleIndex) {
			StringBuilder sb = new StringBuilder();
			GetGroupRowSummaryTextCore(visibleIndex, sb);
			return sb.ToString();
		}
		protected internal virtual void GetGroupRowSummaryTextCore(int visibleIndex, StringBuilder sb) {
			int ac = 0;
			foreach (ASPxSummaryItem item in DataProxy.GroupSummary.GetGroupRowItems()) {
				if (!DataProxy.IsGroupSummaryExists(visibleIndex, item)) continue;
				sb.Append(ac++ == 0 ? " (" : ", ");
				object value = DataProxy.GetGroupSummaryValue(visibleIndex, item);
				string caption = Columns[item.FieldName] != null ? Columns[item.FieldName].ToString() : string.Empty;
				string text = item.GetGroupDisplayText(caption, value);
				text = RaiseSummaryDisplayText(new ASPxGridViewSummaryDisplayTextEventArgs(item, value, text, visibleIndex, true));
				sb.Append(text);
			}
			if (ac > 0) sb.Append(")");
		}
		public object GetTotalSummaryValue(ASPxSummaryItem item) {
			if (item == null) throw new ArgumentNullException("item");
			return DataProxy.GetTotalSummaryValue(item);
		}
		public object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			if (item == null) throw new ArgumentNullException("item");
			return DataProxy.GetGroupSummaryValue(visibleIndex, item);
		}
		protected internal virtual string GetColumnFilterString(GridViewDataColumn column) {
			if (column == null) return string.Empty;
			return CriteriaOperator.ToString(GetColumnFilter(column));
		}
		protected internal virtual CriteriaOperator GetColumnFilter(GridViewDataColumn column) {
			if (column == null) return null;
			CriteriaOperator rv;
			ColumnFilterInfo.TryGetValue(new OperandProperty(column.FieldName), out rv);
			return rv;
		}
		public void AutoFilterByColumn(GridViewColumn column, string value) {
			GridViewDataColumn dcColumn = column as GridViewDataColumn;
			if (column != null && dcColumn == null) new ArgumentException("Column should be GridViewDataColumn", "column");
			if (dcColumn == null) throw new ArgumentNullException("column");
			CriteriaOperator filter = FilterHelper.CreateAutoFilter(dcColumn, value);
			ApplyFilterByColumn(dcColumn, filter);
		}
		protected internal void FilterByHeaderPopup(GridViewColumn column, string value) {
			GridViewDataColumn dcColumn = column as GridViewDataColumn;
			if (column != null && dcColumn == null) new ArgumentException("Column should be GridViewDataColumn", "column");
			if (dcColumn == null) throw new ArgumentNullException("column");
			FilterValue filterValue = FilterValue.FromHtmlValue(value);
			ApplyFilterByColumn(dcColumn, GetFilterExpression(dcColumn, filterValue));
		}
		protected virtual void ApplyFilterByColumn(GridViewDataColumn column, CriteriaOperator criteria) {
			if (column == null) throw new ArgumentNullException("column");
			ColumnFilterInfo[new OperandProperty(column.FieldName)] = criteria;
			FilterExpression = CriteriaOperator.ToString(GroupOperator.And(ColumnFilterInfo.Values));
		}
		CriteriaOperator GetFilterExpression(GridViewDataColumn column, FilterValue filterValue) {
			if (filterValue.IsEmpty) return null;
			if (filterValue.IsFilterByQuery) return CriteriaOperator.TryParse(filterValue.Query);
			return FilterHelper.CreateHeaderFilter(column, filterValue.Value);
		}
		internal bool IsAllowDataSourcePaging() {
			if (DesignMode) return false;
			if (!DataSourceForceStandardPaging) return false;
			return true;
		}
		public void ClearSort() {
			if (SortCount == 0) return;
			ResetSortGroup();
			BindAndSynchronizeDataProxy();
		}
		public bool IsAllowSort(GridViewColumn column) {
			return column.GetAllowSort();
		}
		public bool IsAllowGroup(GridViewColumn column) {
			if (IsAllowDataSourcePaging()) return false;
			return column.GetAllowGroup();
		}
		public virtual bool IsReadOnly(GridViewDataColumn column) {
			if (column.ReadOnly) return true;
			return DataProxy.IsReadOnly(column.FieldName);
		}
		public ColumnSortOrder SortBy(GridViewColumn column, ColumnSortOrder value) {
			GridViewDataColumn dcColumn = column as GridViewDataColumn;
			if (column != null && dcColumn == null) new ArgumentException("Column should be GridViewDataColumn", "column");
			if (dcColumn == null) throw new ArgumentNullException("column");
			if (dcColumn.GroupIndex < 0) {
				if (!IsAllowSort(dcColumn)) return ColumnSortOrder.None;
			} else {
				if (!IsAllowGroup(dcColumn)) return ColumnSortOrder.None;
			}
			if (dcColumn.SortOrder == value) return value;
			if (value == ColumnSortOrder.None) {
				SortedColumnsChanged(dcColumn, -1, true, ColumnSortOrder.None);
				return ColumnSortOrder.None;
			}
			if (SortedColumnsChanged(dcColumn, SortCount, false, value) == -1) return ColumnSortOrder.None;
			return value;
		}
		public int SortBy(GridViewColumn column, int value) {
			GridViewDataColumn dcColumn = column as GridViewDataColumn;
			if (column != null && dcColumn == null) new ArgumentException("Column should be GridViewDataColumn", "column");
			if (dcColumn == null) throw new ArgumentNullException("column");
			if (!IsAllowSort(dcColumn)) return -1;
			ColumnSortOrder order = dcColumn.SortOrder;
			if (order == ColumnSortOrder.None)
				order = ColumnSortOrder.Ascending;
			value = SortedColumnsChanged(dcColumn, value, false, order);
			return value;
		}
		public void CollapseRow(int visibleIndex) { CollapseRow(visibleIndex, false); }
		public void CollapseRow(int visibleIndex, bool recursive) {
			DataBoundProxy.CollapseRow(visibleIndex, recursive);
			LayoutChanged();
		}
		public void ExpandRow(int visibleIndex) { ExpandRow(visibleIndex, false); }
		public void ExpandRow(int visibleIndex, bool recursive) {
			DataBoundProxy.ExpandRow(visibleIndex, recursive);
			LayoutChanged();
		}
		public bool IsRowExpanded(int visibleIndex) {
			return DataProxy.IsRowExpanded(visibleIndex);
		}
		public void ExpandAll() {
			DataBoundProxy.ExpandAll();
			LayoutChanged();
		}
		public void CollapseAll() {
			DataBoundProxy.CollapseAll();
			LayoutChanged();
		}
		[Description("Provides access to the settings that define images displayed within the ASPxGridView's elements."),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewImages Images { get { return (GridViewImages)ImagesInternal; } }
		[Description("Provides access to the settings that define images displayed within the ASPxGridView's editors."),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditorImages ImagesEditors { get { return imagesEditors; } }
		[Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImages ImagesFilterControl { get { return imagesFilterControl; } }
		[Description("Provides access to the style settings that control the appearance of the ASPxGridView elements."),
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStyles Styles { get { return StylesInternal as GridViewStyles; } }
		[Description("Provides access to the style settings that control the appearance of the Pager displayed within the ASPxGridView."),
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewPagerStyles StylesPager { get { return stylesPager; } }
		[Description("Provides access to style settings used to paint ASPxGridView's editors."),
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditorStyles StylesEditors { get { return stylesEditors; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlStyles StylesFilterControl { get { return stylesFilterControl; } }
		[Browsable(false), AutoFormatEnable, Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewTemplates Templates { get { return templates; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		protected override ImagesBase CreateImages() {
			return new GridViewImages(this);
		}
		protected override StylesBase CreateStyles() {
			return new GridViewStyles(this);
		}
		public virtual string SaveClientLayout() {
			return new GridViewCookies(this).SaveState(PageIndex);
		}
		public virtual void LoadClientLayout(string layoutData) {
			ProcessCookies(new GridViewCookies(this), layoutData);
		}
		protected override bool IsStateSavedToCookies {
			get { return !DesignMode && SettingsCookies.Enabled; }
		}
		protected override string GetStateCookieName() {
			return base.GetStateCookieName(SettingsCookies.CookiesID);
		}
		protected override void SaveClientState() {
			if (IsClientLayoutExists)
				RaiseClientLayout(new ASPxClientLayoutArgs(ClientLayoutMode.Saving, GetClientStringLayout()));
			if (IsStateSavedToCookies)
				SetCookie(GetStateCookieName(), GetClientStringLayout());
		}
		protected override bool LoadClientState() {
			string state = string.Empty;
			if (IsClientLayoutExists) {
				ASPxClientLayoutArgs args = new ASPxClientLayoutArgs(ClientLayoutMode.Loading);
				RaiseClientLayout(args);
				state = args.LayoutData;
			}
			if (string.IsNullOrEmpty(state) && IsStateSavedToCookies)
				state = GetCookie(GetStateCookieName());
			if (!string.IsNullOrEmpty(state)) {
				GridViewCookies cookies = new GridViewCookies(this);
				ProcessCookies(cookies, state);
			}
			return true;
		}
		protected virtual string GetClientStringLayout() {
			return new GridViewCookies(this).SaveState(PageIndex);
		}
		protected internal bool SetColumnVisible(GridViewColumn column, bool value) {
			return this.webColumnsOwnerImpl.SetColumnVisible(column, value);
		}
		protected internal int SetColumnVisibleIndex(GridViewColumn column, int value) {
			return this.webColumnsOwnerImpl.SetColumnVisibleIndex(column, value);
		}
		protected int SortedColumnsChanged(GridViewDataColumn column, int value, bool changeGrouping, ColumnSortOrder sortOrder) {
			if (!IsAllowGroup(column)) changeGrouping = false;
			if (!changeGrouping && column.GroupIndex < 0 && !IsAllowSort(column)) return -1;
			if ((changeGrouping || column.GroupIndex > -1) && !IsAllowGroup(column)) return -1;
			ASPxGridViewBeforeColumnGroupingSortingEventArgs eventArgs =
				new ASPxGridViewBeforeColumnGroupingSortingEventArgs(
				column, column.SortOrder, column.SortIndex, column.GroupIndex);
			if (value < 0) value = -1;
			if (changeGrouping && value != -1)
				column.UngroupedSortOrder = column.SortOrder;
			if (changeGrouping || value == -1) column.SetGroupIndex(-1);
			if ((changeGrouping && value == -1) && (sortOrder != ColumnSortOrder.None))
				column.SetSortOrder(sortOrder);
			else {
				SortedColumns.Remove(column);
				column.SetSortIndex(-1);
				column.SetSortOrder(ColumnSortOrder.None);
			}
			if (value > -1) {
				if (value >= SortedColumns.Count)
					SortedColumns.Add(column);
				else SortedColumns.Insert(value, column);
				column.SetSortOrder(sortOrder);
			}
			if (value > -1)
				value = SortedColumns.IndexOf(column);
			if (changeGrouping) column.SetGroupIndex(value);
			UpdateGroupSortIndexes();
			RaiseBeforeColumnSortingGrouping(eventArgs);
			BindAndSynchronizeDataProxy();
			if (changeGrouping && SettingsBehavior.AutoExpandAllGroups) {
				ExpandAll();
			}
			return value;
		}
		int CompareColumnsByGroupIndex(GridViewDataColumn col1, GridViewDataColumn col2) {
			if (col1.GroupIndex == col2.GroupIndex) return Comparer.Default.Compare(col1.SortIndex, col2.SortIndex);
			if (col1.GroupIndex < 0) return 1;
			if (col2.GroupIndex < 0) return -1;
			return Comparer.Default.Compare(col1.GroupIndex, col2.GroupIndex);
		}
		void UpdateGroupSortIndexes() {
			for (int i = 0; i < SortedColumns.Count; i++) {
				SortedColumns[i].SetSortIndex(i);
			}
			SortedColumns.Sort(new Comparison<GridViewDataColumn>(CompareColumnsByGroupIndex));
			int gc = GroupCount;
			for (int i = 0; i < SortedColumns.Count; i++) {
				if (i < gc) SortedColumns[i].SetGroupIndex(i);
				SortedColumns[i].SetSortIndex(i);
			}
		}
		void BuildSortedColumns() {
			SortedColumns.Clear();
			foreach (GridViewDataColumn dataColumn in DataColumns) {
				if (dataColumn.SortIndex > -1) SortedColumns.Add(dataColumn);
			}
			SortedColumns.Sort(new Comparison<GridViewDataColumn>(CompareColumnsByGroupIndex));
			UpdateGroupSortIndexes();
		}
		void BuildVisibleColumns() {
			this.webColumnsOwnerImpl.BuildVisibleColumns();
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.dummyNamingContainer = null;
			this.rootTable = null;
			this.containerControl = null;
			RenderHelper.ClearControlHierarchy();
			RenderHelper.DummyEditorList.Clear();
		}
		protected override void ResetControlHierarchy() {
			base.ResetControlHierarchy();
			ClearControlFields();
		}
		protected internal virtual void OnBeforeCreateControlHierarchy() {
			LoadDataIfNotBinded();
		}
		protected internal virtual void OnAfterCreateControlHierarchy() {
			if (EnableRowsCache) {
				SetCallbackPropertyValue("Data", string.Empty, DataProxy.SaveData());
			}
			SetCallbackPropertyValue("State", string.Empty, SaveGridControlState());
		}
		class InternalNamingContainer : Control, INamingContainer {
			protected override void Render(HtmlTextWriter writer) {
			}
		}
		Control dummyNamingContainer;
		protected internal Control DummyNamingContainer {
			get {
				if (dummyNamingContainer == null) {
					if (Visible) { 
						EnsureChildControls();
					}
				}
				return dummyNamingContainer;
			}
		}
		protected internal string GetLoadingPanelIDInternal() { return GetLoadingPanelID(); }
		protected internal string GetLoadingDivIDInternal() { return GetLoadingDivID(); }
		protected virtual GridViewContainerControl CreateContainerControl() { return new GridViewContainerControl(this); }
		protected virtual void CreateRootControls() {
			Controls.Clear();
			this.dummyNamingContainer = new InternalNamingContainer();
			Controls.Add(DummyNamingContainer);
			RenderHelper.CreateGridDummyEditors(DummyNamingContainer);
			this.rootTable = RenderUtils.CreateTable(true);
			Controls.Add(RootTable);
			RootTable.Rows.Add(RenderUtils.CreateTableRow());
			RootTable.Rows[0].Cells.Add(RenderUtils.CreateTableCell());
			this.containerControl = CreateContainerControl();
			RootCell.Controls.Add(ContainerControl);
			if (RenderHelper.RequireRenderFilterPopupWindow)
				Controls.Add(new GridViewHtmlFilterPopup(new GridViewFilterPopupOwner(this)));
			this.hierarchyChanged = true;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateRootControls();
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if (DesignMode) BuildSortedColumns();
		}
		protected override void PrepareControlHierarchy() {
			if (RootTable != null) {
				RenderUtils.AssignAttributes(this, RootTable);
				RenderUtils.SetVisibility(RootTable, IsClientVisible(), true);
				AppearanceStyle style = IsEnabled() ? RenderHelper.GetRootTableStyle() : RenderHelper.GetDisabledRootTableStyle();
				style.AssignToControl(RootTable, false);
				style.Paddings.AssignToControl(RootCell);
				RootTable.CellPadding = 0;
				RootTable.CellSpacing = 0;
			}
		}
		protected bool IsFirstLoad {
			get { return Page == null || !Page.IsPostBack || this.postCollection != null && !this.isGridStateLoaded; }
		}
		protected override void OnLoad(EventArgs e) {
			BuildSortedColumns();
			ResetVisibleColumns();
			base.OnLoad(e);
			ProcessSEOPaging();
			CheckPendingEvents();
			if (Visible && !IsFirstLoad) return;
			SetDataProxyAllowFocusedRow();
		}
		IDictionary<OperandProperty, CriteriaOperator> columnFilterInfo;
		protected internal IDictionary<OperandProperty, CriteriaOperator> ColumnFilterInfo {
			get {
				if (columnFilterInfo == null) {
					columnFilterInfo = CriteriaColumnAffinityResolver.SplitByColumns(CriteriaOperator.Parse(FilterExpression));
				}
				return columnFilterInfo;
			}
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			this.isGridStateLoaded = false;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			list.Add(GroupSummary);
			list.Add(TotalSummary);
			list.Add(Settings);
			list.Add(SettingsPager);
			list.Add(SettingsEditing);
			list.Add(SettingsBehavior);
			list.Add(SettingsCustomizationWindow);
			list.Add(SettingsCookies);
			list.Add(SettingsDetail);
			list.Add(SettingsText);
			list.Add(StylesEditors);
			list.Add(StylesFilterControl);
			list.Add(StylesPager);
			list.Add(ImagesEditors);
			list.Add(ImagesFilterControl);
			return list.ToArray();
		}
		protected override object SaveViewState() {
			SetStringProperty(ASPxGridViewRenderHelper.CallbackHiddenFieldName, null, !Visible ? GetCallbackStateString() : null);
			return base.SaveViewState();
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return RenderHelper.ShowHorizontalScrolling || base.HasClientInitialization();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			if (RenderHelper.RequireTablesHelperScripts) {
				RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridViewTableColumnResizingResourceName);
			}
			RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridViewScriptResourceName);
		}
		protected override string GetDefaultCssFilePath() {
			return RenderUtils.GetCssWebResourceUrl(Page, typeof(ASPxGridView), WebResourceCssDefaultPath);
		}
		protected override void GetStyleSheetsLinks(List<string> list) {
			base.GetStyleSheetsLinks(list);
			for (int i = 0; i < RenderHelper.DummyEditorList.Count; i++)
				list.Add(GetStyleSheetLinkHtml(RenderHelper.DummyEditorList[i].GetRenderCssFilePath()));
		}
		protected override void GetStyleSheetsImports(List<string> list) {
			base.GetStyleSheetsImports(list);
			for (int i = 0; i < RenderHelper.DummyEditorList.Count; i++)
				list.Add(GetStyleSheetImportHtml(RenderHelper.DummyEditorList[i].GetRenderCssFilePath()));
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientGridView";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			InitializeClientObjectScript(stb, localVarName, clientName);
		}
		int scrollToVisibleIndexOnClient = -1;
		[AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int ScrollToVisibleIndexOnClient {
			get { return scrollToVisibleIndexOnClient; }
			set { scrollToVisibleIndexOnClient = value; }
		}
		protected void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			stb.AppendFormat("{0}.callBacksEnabled={1};\n", localVarName, EnableCallBacks ? "true" : "false");
			stb.AppendFormat("{0}.pageRowCount={1};\n", localVarName, DataProxy.VisibleRowCountOnPage);
			stb.AppendFormat("{0}.pageRowSize={1};\n", localVarName, DataProxy.PageSize);
			stb.AppendFormat("{0}.selectedWithoutPageRowCount={1};\n", localVarName, DataProxy.GetSelectedRowCountWithoutCurrentPage());
			stb.AppendFormat("{0}.visibleStartIndex={1};\n", localVarName, DataProxy.VisibleStartIndex);
			stb.AppendFormat("{0}.focusedRowIndex={1};\n", localVarName, DataProxy.FocusedRowVisibleIndex);
			stb.AppendFormat("{0}.allowFocusedRow={1};\n", localVarName, DataProxy.AllowFocusedRow ? "true" : "false");
			stb.AppendFormat("{0}.allowMultiSelection={1};\n", localVarName, SettingsBehavior.AllowMultiSelection ? "true" : "false");
			stb.AppendFormat("{0}.isColumnsResizable={1};\n", localVarName, SettingsBehavior.ColumnResizeMode != ColumnResizeMode.Disabled ? "true" : "false");
			stb.AppendFormat("{0}.isVerticalScrolling={1};\n", localVarName, Settings.ShowVerticalScrollBar ? "true" : "false");
			if (ScrollToVisibleIndexOnClient > -1) {
				stb.AppendFormat("{0}.scrollToRowIndex={1};\n", localVarName, ScrollToVisibleIndexOnClient);
			}
			stb.AppendFormat("{0}.isHorizontalScrolling={1};\n", localVarName, Settings.ShowHorizontalScrollBar ? "true" : "false");
			if (RenderHelper.HasFixedColumns) {
				stb.AppendFormat("{0}.fixedColumnCount={1};\n", localVarName, FixedColumnCount);
			}
			if (Settings.ShowVerticalScrollBar && Width.Type == UnitType.Percentage) {
				stb.AppendFormat("{0}.isWidthTypePercent={1};\n", localVarName, "true");
			}
			if (RenderHelper.IsVirtualScrolling) {
				stb.AppendFormat("{0}.virtualScrollRowHeight={1};\n", localVarName, RenderHelper.VirtualScrollRowHeight);
			}
			stb.AppendFormat("{0}.isMainControlResizable={1};\n", localVarName, SettingsBehavior.ColumnResizeMode == ColumnResizeMode.Control ? "true" : "false");
			if (SettingsBehavior.ColumnResizeMode != ColumnResizeMode.Disabled) {
				stb.AppendFormat("{0}.indentColumnCount={1};\n", localVarName, RenderHelper.IndentColumnCount);
			}
			stb.AppendFormat("{0}.callbackOnFocusedRowChanged={1};\n", localVarName, SettingsBehavior.ProcessFocusedRowChangedOnServer ? "true" : "false");
			stb.AppendFormat("{0}.callbackOnSelectionChanged={1};\n", localVarName, SettingsBehavior.ProcessSelectionChangedOnServer ? "true" : "false");
			stb.AppendFormat("{0}.autoFilterDelay='{1}';\n", localVarName, SettingsBehavior.AutoFilterRowInputDelay);
			if (SettingsBehavior.ConfirmDelete) {
				stb.AppendFormat("{0}.confirmDelete={1};\n", localVarName, HtmlConvertor.ToJSON(SettingsText.GetConfirmDelete()));
			}
			GenerateClientColumns(stb, localVarName);
			GenerateFiringClientEvents(stb, clientID);
			GenerateEditorIDList(stb, localVarName);
			if (RenderHelper.RequireRenderFilterRowMenu) {
				Hashtable filterRowConditions = new Hashtable();
				foreach (GridViewDataColumn column in DataColumns) {
					AutoFilterCondition condition = column.Settings.AutoFilterCondition;
					if (condition == AutoFilterCondition.Default)
						condition = BaseFilterHelper.GetDefaultAutoFilterCondition(column.GetDataType(), DataProxy.IsServerMode);
					filterRowConditions.Add(column.Index, (int)condition);
				}
				stb.AppendFormat("{0}.filterRowConditions={1};\n", localVarName, HtmlConvertor.ToJSON(filterRowConditions));
			}
			GenerateKeyValuesScript(stb, localVarName);
		}
		void GenerateEditorIDList(StringBuilder stb, string localVarName) {
			if (RenderHelper.EditingRowEditorList.Count < 1) return;
			stb.AppendFormat("{0}.editorIDList=[", localVarName);
			bool first = true;
			foreach (ASPxEditBase editor in RenderHelper.EditingRowEditorList) {
				if (!first) stb.Append(",");
				stb.AppendFormat(HtmlConvertor.ToScript(editor.ClientID));
				first = false;
			}
			stb.Append("];\n");
		}
		protected string GetCallbackClientObjectScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("var {0} = {1};\r\n", ShortClientLocalVariableName, GetClientInstanceName());
			InitializeClientObjectScript(stb, ShortClientLocalVariableName, ClientID);
			return RenderUtils.GetScriptHtml(stb.ToString());
		}
		protected void GenerateClientColumns(StringBuilder stb, string localVarName) {
			stb.AppendFormat("{0}.ClearColumns();\n", localVarName);
			foreach (GridViewColumn col in Columns) {
				GridViewDataColumn dataCol = col as GridViewDataColumn;
				stb.AppendFormat("{0}.CreateColumn({1},{2},{3},{4});\n",
					localVarName, HtmlConvertor.ToScript(col.Name), col.Index,
					HtmlConvertor.ToScript(dataCol == null ? null : dataCol.FieldName), col.Visible ? 1 : 0);
			}
		}
		protected void GenerateFiringClientEvents(StringBuilder stb, string clientID) {
			if (!EnableCallBacks)
				return;
			if (this.fireFocusedRowChangedOnClient)
				stb.AppendFormat("window.setTimeout(\"aspxGVFocusedRowChanged(\\\"{0}\\\")\", 0);", clientID);
			if (this.fireSelectionChangedOnClient)
				stb.AppendFormat("window.setTimeout(\"aspxGVSelectionChanged(\\\"{0}\\\")\", 0);", clientID);
		}
		protected void GenerateKeyValuesScript(StringBuilder stb, string localVarName) {
			if (!DataProxy.HasCorrectKeyFieldName) return;
			List<string> list = new List<string>(DataProxy.VisibleRowCountOnPage);
			for (int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				int index = DataProxy.VisibleStartIndex + i;
				string value = DataProxy.GetKeyValueForScript(index);
				list.Add(value);
			}
			stb.AppendFormat("{0}.keys={1};\n", localVarName, HtmlConvertor.ToJSON(list));
		}
		protected Dictionary<string, ASPxGridCallBackMethod> CallBacks {
			get {
				if (callBacks == null) {
					callBacks = new Dictionary<string, ASPxGridCallBackMethod>();
					RegisterCallBacks(callBacks);
				}
				return callBacks;
			}
		}
		protected internal Dictionary<string, ASPxGridCallBackFunction> InternalCallBacks {
			get {
				if (internalCallBacks == null) {
					internalCallBacks = new Dictionary<string, ASPxGridCallBackFunction>();
					RegisterInternalCallBacks(internalCallBacks);
				}
				return internalCallBacks;
			}
		}
		internal bool IsErrorOnCallbackCore { get { return IsErrorOnCallback; } }
		protected override void RaiseCallbackEvent(string eventArgument) {
			if (IsErrorOnCallback) return;
			this.internalCallbackInfo = null;
			GridCallbackArgumentsReader argumentsReader = new GridCallbackArgumentsReader(eventArgument);
			if (!string.IsNullOrEmpty(argumentsReader.CallbackState)) {
				SetCallbackStateString(argumentsReader.CallbackState);
			}
			if (!string.IsNullOrEmpty(argumentsReader.PageSelectionResult)) {
				LoadDataIfNotBinded();
			}
			LoadGridControlState(argumentsReader.PageSelectionResult, argumentsReader.ColumnResizingResult, true);
			RequireDataBinding();
			SetEditorValues(argumentsReader.EditValues, CanIgnoreInvalidEditorValues(argumentsReader.CallbackArguments) || argumentsReader.InternalCallbackIndex != -1);
			if (argumentsReader.InternalCallbackIndex == -1) {
				CheckPendingEvents();
				if (argumentsReader.FocusedRowIndex != -1) {
					DataProxy.FocusedRowVisibleIndex = argumentsReader.FocusedRowIndex;
					this.fireFocusedRowChangedOnClient = false;
				}
				DoCallBackPostBack(argumentsReader.CallbackArguments);
			} else {
				this.internalCallbackInfo = new CallbackInfo(argumentsReader.InternalCallbackIndex, argumentsReader.CallbackArguments);
			}
		}
		bool CanIgnoreInvalidEditorValues(string callBack) {
			if (!IsEditing)
				return false;
			return callBack == "10|CANCELEDIT" || callBack.StartsWith("9|STARTEDIT") || callBack.StartsWith("9|ADDNEWROW");
		}
		protected virtual void SetEditorValues(string editorValues, bool canIgnoreInvalidValues) {
			if (string.IsNullOrEmpty(editorValues)) return;
			GridViewCallBackEditorValuesReader reader = new GridViewCallBackEditorValuesReader(editorValues);
			reader.Proccess();
			if (reader.Values.Count > 0) {
				Dictionary<string, object> values = new Dictionary<string, object>();
				foreach (EditorValueInfo info in reader.Values) {
					GridViewDataColumn column = Columns[info.ColumnIndex] as GridViewDataColumn;
					if (column == null || string.IsNullOrEmpty(column.FieldName)) continue;
					values[column.FieldName] = info.Value;
				}
				DataProxy.SetEditorValues(values, canIgnoreInvalidValues);
			}
		}
		protected virtual string[] GetCallBackPostBackArgs(ref string eventArgument) {
			List<string> deserializedArgs = CommonUtils.DeserializeStringArray(eventArgument);
			string command = deserializedArgs[0];
			deserializedArgs.RemoveAt(0);
			eventArgument = command;
			return deserializedArgs.ToArray();
		}
		protected virtual void DoCallBackPostBack(string eventArgument) {
			if (string.IsNullOrEmpty(eventArgument))
				return;
			CheckPendingEvents();
			string[] args = GetCallBackPostBackArgs(ref eventArgument);
			if (CallBacks.ContainsKey(eventArgument))
				CallBacks[eventArgument](args);
			RaiseAfterPerformCallback(new ASPxGridViewAfterPerformCallbackEventArgs(eventArgument, args));
		}
		bool CheckRequireDataBound() {
			if (this.requireDataBound && !DataProxy.IsBound) {
				this.requireDataBound = false;
				ResetControlHierarchy();
				RequireDataBinding();
				EnsureDataBound();
				return true;
			}
			return false;
		}
		protected override object GetCallbackResult() {
			RaiseBeforeGetCallbackResult(EventArgs.Empty);
			Dictionary<string, bool> globalScripts = new Dictionary<string, bool>(GlobalScriptBlocksRendered);
			Dictionary<string, string> globalStyles = new Dictionary<string, string>(GlobalStyleSheetReferences);
			string res = GetCallbackResultCore();
			if (CheckRequireDataBound()) {
				GlobalScriptBlocksRendered.Clear();
				foreach (KeyValuePair<string, bool> pair in globalScripts) { GlobalScriptBlocksRendered[pair.Key] = pair.Value; }
				GlobalStyleSheetReferences.Clear();
				foreach (KeyValuePair<string, string> pair in globalStyles) { GlobalStyleSheetReferences[pair.Key] = pair.Value; }
				res = GetCallbackResultCore();
			}
			return res.Replace("\0", string.Empty); 
		}
		string GetCallbackResultCore() {
			if (this.internalCallbackInfo == null) {
				if (ContainerControl == null) {
					EnsureChildControls();
				}
				BeginRendering();
				try {
					return RenderUtils.GetRenderResult(ContainerControl, GetCallbackClientObjectScript());
				} finally {
					EndRendering();
				}
			}
			string res = "FB|" + this.internalCallbackInfo.CallbackId.ToString() + "|";
			string eventArgument = this.internalCallbackInfo.EventArgument;
			string[] args = GetCallBackPostBackArgs(ref eventArgument);
			if (InternalCallBacks.ContainsKey(eventArgument)) {
				object value = InternalCallBacks[eventArgument](args);
				res += HtmlConvertor.ToJSON(value);
			}
			return res;
		}
		protected virtual void OnPageIndexChanged() {
			DataProxy.ClearStoredPageSelectionResult(); 
			if (IsAllowDataSourcePaging() && DataProxy.IsBound) {
				PerformSelect();
			} else
				DataBindNoControls();
			LayoutChanged();
			RaisePageIndexChanged();
		}
		protected string SaveGridControlState() {
			using (MemoryStream stream = new MemoryStream()) {
				TypedBinaryWriter writer = new TypedBinaryWriter(stream);
				SaveColumnsVisibleState(writer);
				SaveColumnsWidths(writer);
				SaveColumnAutoFilterConditions(writer);
				writer.WriteObject(SortedColumns.Count);
				writer.WriteObject(GroupCount);
				foreach (GridViewDataColumn column in SortedColumns) {
					writer.WriteObject(column.Index);
					writer.WriteObject((int)column.SortOrder);
					writer.WriteObject((int)column.UngroupedSortOrder);
				}
				DataProxy.SaveDataState(writer);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		void SaveColumnsVisibleState(TypedBinaryWriter writer) {
			writer.WriteObject(Columns.Count);
			for (int n = 0; n < Columns.Count; n++) {
				writer.WriteObject(Columns[n].VisibleIndex);
				writer.WriteObject(Columns[n].Visible);
			}
		}
		void SaveColumnsWidths(TypedBinaryWriter writer) {
			int count = 0;
			if (RenderHelper.AllowColumnResizing) {
				for (int n = 0; n < Columns.Count; n++) {
					if (!Columns[n].Width.IsEmpty) count++;
				}
			}
			writer.WriteObject(count);
			if (count == 0) return;
			for (int n = 0; n < Columns.Count; n++) {
				if (Columns[n].Width.IsEmpty) continue;
				writer.WriteObject(n);
				writer.WriteObject((int)Columns[n].Width.Type);
				writer.WriteObject(Columns[n].Width.Value);
			}
		}
		void SaveColumnAutoFilterConditions(TypedBinaryWriter writer) {
			if (!RenderHelper.RequireRenderFilterRowMenu) {
				writer.WriteObject(0);
				return;
			}
			List<GridViewDataColumn> columns = DataColumns;
			writer.WriteObject(columns.Count);
			foreach (GridViewDataColumn column in columns) {
				writer.WriteObject(column.Index);
				writer.WriteObject((int)column.Settings.AutoFilterCondition);
			}
		}
		bool isGridStateLoaded = false;
		protected void LoadGridControlState(string pageSelectionResult, string columnResizingResult, bool restoreRowsState) {
			this.columnFilterInfo = null; 
			string webData = (string)GetCallbackPropertyValue("State", string.Empty);
			if (string.IsNullOrEmpty(webData)) return;
			if (this.isGridStateLoaded) return;
			this.isGridStateLoaded = true;
			LoadDataIfNotBinded();
			using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(webData))) {
				TypedBinaryReader reader = new TypedBinaryReader(stream);
				LoadGridColumnsState(reader, columnResizingResult);
				if (!EnableRowsCache) {
					LoadDataIfNotBinded(true);
					SynchronizeDataProxy(); 
				}
				DataProxy.LoadDataState(reader, pageSelectionResult, restoreRowsState);
				this.fireSelectionChangedOnClient = false;
			}
		}
		void ResetSortGroup() {
			SortedColumns.Clear();
			foreach (GridViewDataColumn dataColumn in DataColumns) {
				dataColumn.SetSortIndex(-1);
				dataColumn.SetGroupIndex(-1);
				dataColumn.SetSortOrder(ColumnSortOrder.None);
			}
		}
		void ResetSortOnly(GridViewColumn excludeColumn) {
			SortedColumns.Clear();
			foreach (GridViewDataColumn dataColumn in DataColumns) {
				if (excludeColumn != dataColumn && dataColumn.GroupIndex < 0) {
					dataColumn.SetSortIndex(-1);
					dataColumn.SetSortOrder(ColumnSortOrder.None);
				}
			}
			BuildSortedColumns();
		}
		void LoadGridColumnsState(TypedBinaryReader reader, string columnsResizingResult) {
			GridViewColumnsState columnStates = new GridViewColumnsState(this);
			columnStates.Read(reader);
			LoadGridColumnsState(columnStates);
			ApplyColumnResizingResult(columnsResizingResult);
		}
		void LoadGridColumnsState(GridViewColumnsState columnsState) {
			ResetSortGroup();
			columnsState.Apply();
			BuildSortedColumns();
			BuildVisibleColumns();
		}
		void ApplyColumnResizingResult(string columnsResizingResult) {
			int indentCount = GroupCount + (RenderHelper.HasDetailButton ? 1 : 0);
			List<Unit> widths = CallbackArgumentsReader.ReadUnitArray(columnsResizingResult);
			for (int i = 0; i < indentCount && i < widths.Count; i++) {
				widths.RemoveAt(0);
			}
			List<GridViewColumn> columns = GetColumnsShownInHeaders();
			for (int i = 0; i < widths.Count && i < columns.Count; i++) {
				if (!widths[i].IsEmpty) {
					columns[i].Width = widths[i];
				}
			}
		}
		int CheckPageIndex(int pageIndex) {
			if (pageIndex < -1) pageIndex = -1;
			if (IsAllowDataSourcePaging() && !DataProxy.IsBound) return pageIndex;
			if (pageIndex >= DataProxy.PageCount) pageIndex = Math.Max(0, DataProxy.PageCount - 1);
			return pageIndex;
		}
		protected override void Render(HtmlTextWriter writer) {
			if (!PreRendered) {
				EnsureChildControls();
				PrepareControlHierarchy();
			}
			CheckRequireDataBound();
			base.Render(writer);
		}
		GridFilterData filterData;
		protected GridFilterData FilterData {
			get {
				if (filterData == null) {
					filterData = new GridFilterData(this);
					filterData.OnStart();
				}
				return filterData;
			}
		}
		GridSortData sortData;
		protected internal GridSortData SortData {
			get {
				if (sortData == null) {
					sortData = new GridSortData(this);
				}
				return sortData;
			}
		}
		protected void DestroyFilterData() {
			this.filterData = null;
		}
		#region IWebDataOwner Members
		bool requireDataBound = false;
		void IWebDataOwner.RequireDataBound() { this.requireDataBound = true; }
		DataSourceView IWebDataOwner.GetData() { return GetData(); }
		bool IWebDataOwner.IsDesignTime { get { return DesignMode; } }
		bool IWebDataOwner.IsForceDataSourcePaging {
			get { return IsAllowDataSourcePaging(); }
		}
		DataSourceSelectArguments IWebDataOwner.SelectArguments {
			get {
				if (DataHelper != null) return DataHelper.LastArguments;
				return new DataSourceSelectArguments();
			}
		}
		GridViewDataHelper DataHelper { get { return DataContainer[DefaultDataHelperName] as GridViewDataHelper; } }
		int IWebControlPageSettings.PageSize { get { return SettingsPager.PageSize; } }
		int IWebControlPageSettings.PageIndex { get { return PageIndex; } set { PageIndex = value; } }
		GridViewPagerMode IWebControlPageSettings.PagerMode { get { return SettingsPager.Mode; } }
		List<IWebColumnInfo> IWebDataOwner.GetColumns() {
			List<IWebColumnInfo> list = new List<IWebColumnInfo>();
			foreach (GridViewDataColumn column in DataColumns) {
				list.Add(column);
			}
			return list;
		}
		IDataControllerSort IWebDataOwner.SortClient { get { return this; } }
		IWebControlObject IWebDataOwner.WebControl { get { return this; } }
		Dictionary<string, object> IWebDataOwner.GetEditTemplateValues() { return GetEditTemplateValuesCore(); }
		bool IWebDataOwner.AllowOnlyOneMasterRowExpanded { get { return SettingsDetail.AllowOnlyOneMasterRowExpanded; } }
		bool IWebDataOwner.ValidateEditTemplates() {
			return ASPxEdit.ValidateEditorsInContainer(this, EditTemplateValidationGroup);
		}
		#endregion
		#region CallBackState
		StateBag callbackState;
		protected virtual void ProcessCookies(GridViewCookiesBase cookies, string state) {
			if (!cookies.LoadState(state)) return;
			LoadGridColumnsState(cookies.ColumnsState);
			if (SettingsCookies.StoreFiltering) {
				FilterExpression = cookies.FilterExpression;
				FilterEnabled = cookies.FilterEnabled;
			}
			BindAndSynchronizeDataProxy();
			cookies.SetPageIndex();
		}
		void ProcessSEOPaging() {
			if (Page == null || Request == null || DesignMode) return;
			if (Page.IsCallback || Page.IsPostBack) return;
			ProcessCookies(RenderHelper.SEO, Request.Params[RenderHelper.GetSEOID()]);
		}
		protected internal NameValueCollection postCollection = null;
		protected override bool CanLoadPostDataOnCreateControls() {
			return true;
		}
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {			
			this.postCollection = postCollection;
			SetCallbackStateString(GetCallbackValue(ASPxGridViewRenderHelper.CallbackHiddenFieldName));
			LoadGridControlState(postCollection[UniqueID + IdSeparator + ASPxGridViewRenderHelper.DXSelectedInputString],
				postCollection[UniqueID + IdSeparator + ASPxGridViewRenderHelper.DXColumnResizingInputString], true);
			if(DataProxy.IsBound) SynchronizeDataProxy();
			ProcessSEOPaging();
			string focusedRowString = postCollection[UniqueID + IdSeparator + ASPxGridViewRenderHelper.DXFocusedRowInputString];
			if (!string.IsNullOrEmpty(focusedRowString)) {
				int newFocusedRow;
				if (int.TryParse(focusedRowString, out newFocusedRow)) {
					DataProxy.FocusedRowVisibleIndex = newFocusedRow;
				}
			}
			return true;
		}
		string GetCallbackValue(string valueId) {
			string value = this.postCollection[UniqueID + IdSeparator + valueId];
			if (!string.IsNullOrEmpty(value)) return value;
			return GetStringProperty(valueId, null);
		}
		protected override void RaisePostDataChangedEvent() {
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			DoCallBackPostBack(eventArgument);
			LayoutChanged();
		}
		protected object GetCallbackPropertyValue(string propertyName, object defaultValue) {
			if (!IsCallbackStateInitialize) return defaultValue;
			return ViewStateUtils.GetObjectProperty(CallbackState, propertyName, defaultValue);
		}
		protected void SetCallbackPropertyValue(string propertyName, object defaultValue, object value) {
			ViewStateUtils.SetObjectProperty(CallbackState, propertyName, defaultValue, value);
		}
		protected internal string GetCallbackStateString() {
			if (!IsCallbackStateInitialize) return string.Empty;
			object obj = (CallbackState as IStateManager).SaveViewState();
			return obj != null ? (new ObjectStateFormatter()).Serialize(obj) : string.Empty;
		}
		protected void SetCallbackStateString(string callbackText) {
			if (string.IsNullOrEmpty(callbackText)) {
				SetDataProxyAllowFocusedRow();
				CallbackState.Clear();
			} else {
				ObjectStateFormatter formatter = new ObjectStateFormatter();
				(CallbackState as IStateManager).LoadViewState(formatter.Deserialize(callbackText));
			}
		}
		bool IsCallbackStateInitialize { get { return callbackState != null && callbackState.Count > 0; } }
		StateBag CallbackState {
			get {
				if (callbackState == null) {
					callbackState = new StateBag();
					(callbackState as IStateManager).TrackViewState();
				}
				return callbackState;
			}
		}
		#endregion
		protected virtual void RegisterInternalCallBacks(Dictionary<string, ASPxGridCallBackFunction> callBacks) {
			callBacks["SELFIELDVALUES"] = new ASPxGridCallBackFunction(FBSelectFieldValues);
			callBacks["ROWVALUES"] = new ASPxGridCallBackFunction(FBGetRowValues);
			callBacks["PAGEROWVALUES"] = new ASPxGridCallBackFunction(FBPageRowValues);
			callBacks["FILTERPOPUP"] = new ASPxGridCallBackFunction(FBFilterPopup);
			callBacks["CUSTOMVALUES"] = new ASPxGridCallBackFunction(FBCustomValues);
		}
		protected virtual void RegisterCallBacks(Dictionary<string, ASPxGridCallBackMethod> callBacks) {
			callBacks["NEXTPAGE"] = new ASPxGridCallBackMethod(CBNextPage);
			callBacks["PREVPAGE"] = new ASPxGridCallBackMethod(CBPrevPage);
			callBacks["GOTOPAGE"] = new ASPxGridCallBackMethod(CBGotoPage);
			callBacks["SELECTROWS"] = new ASPxGridCallBackMethod(CBSelectRows);
			callBacks["SELECTROWSKEY"] = new ASPxGridCallBackMethod(CBSelectRowsKey);
			callBacks["GROUP"] = new ASPxGridCallBackMethod(CBGroup);
			callBacks["SORT"] = new ASPxGridCallBackMethod(CBSort);
			callBacks["COLUMNMOVE"] = new ASPxGridCallBackMethod(CBMove);
			callBacks["COLLAPSEALL"] = new ASPxGridCallBackMethod(CBCollapseAll);
			callBacks["EXPANDALL"] = new ASPxGridCallBackMethod(CBExpandAll);
			callBacks["EXPANDROW"] = new ASPxGridCallBackMethod(CBExpandRow);
			callBacks["COLLAPSEROW"] = new ASPxGridCallBackMethod(CBCollapseRow);
			callBacks["HIDEALLDETAIL"] = new ASPxGridCallBackMethod(CBHideAllDetailRows);
			callBacks["SHOWALLDETAIL"] = new ASPxGridCallBackMethod(CBShowAllDetailRows);
			callBacks["SHOWDETAILROW"] = new ASPxGridCallBackMethod(CBShowDetailRow);
			callBacks["HIDEDETAILROW"] = new ASPxGridCallBackMethod(CBHideDetailRow);
			callBacks["PAGERONCLICK"] = new ASPxGridCallBackMethod(CBPagerOnClick);
			callBacks["APPLYFILTER"] = new ASPxGridCallBackMethod(CBApplyFilter);
			callBacks["APPLYCOLUMNFILTER"] = new ASPxGridCallBackMethod(CBApplyColumnFilter);
			callBacks["APPLYHEADERCOLUMNFILTER"] = new ASPxGridCallBackMethod(CBApplyHeaderColumnFilter);
			callBacks["FILTERROWMENU"] = new ASPxGridCallBackMethod(CBFilterRowMenu);
			callBacks["STARTEDIT"] = new ASPxGridCallBackMethod(CBStartEdit);
			callBacks["CANCELEDIT"] = new ASPxGridCallBackMethod(CBCancelEdit);
			callBacks["UPDATEEDIT"] = new ASPxGridCallBackMethod(CBUpdateEdit);
			callBacks["ADDNEWROW"] = new ASPxGridCallBackMethod(CBAddNewRow);
			callBacks["DELETEROW"] = new ASPxGridCallBackMethod(CBDeleteRow);
			callBacks["CUSTOMBUTTON"] = new ASPxGridCallBackMethod(CBCustomButton);
			callBacks["CUSTOMCALLBACK"] = new ASPxGridCallBackMethod(CBCustomCallBack);
			callBacks["SHOWFILTERCONTROL"] = new ASPxGridCallBackMethod(CBShowFilterControl);
			callBacks["CLOSEFILTERCONTROL"] = new ASPxGridCallBackMethod(CBCloseFilterControl);
			callBacks["SETFILTERENABLED"] = new ASPxGridCallBackMethod(CBSetFilterEnabled);
			callBacks["REFRESH"] = new ASPxGridCallBackMethod(CBRefresh);
		}
		protected object FBSelectFieldValues(string[] args) {
			if (args.Length == 0) return new List<object>();
			string[] fieldNames = args[0].Split(';');
			return GetSelectedFieldValues(fieldNames);
		}
		protected object FBGetRowValues(string[] args) {
			if (args.Length < 2) return null;
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return null;
			string[] fieldNames = args[1].Split(';');
			return GetRowValues(visibleIndex, fieldNames);
		}
		protected object FBPageRowValues(string[] args) {
			if (args.Length == 0) return new List<object>();
			string[] fieldNames = args[0].Split(';');
			return GetCurrentPageRowValues(fieldNames);
		}
		protected object FBFilterPopup(string[] args) {
			if (args.Length != 3) return null;
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[1]);
			if (column == null) return null;
			string[] result = new string[2];
			result[0] = args[0];
			DataBindNoControls();
			GridViewHtmlFilterContainer container = new GridViewHtmlFilterContainer(new GridViewFilterPopupContainerOwner(column, args[2] == "T"));
			container.EnsureChildControls();
			result[1] = RenderUtils.GetRenderResult(container);
			return result;
		}
		protected object FBCustomValues(string[] args) {
			ASPxGridViewCustomDataCallbackEventArgs e = new ASPxGridViewCustomDataCallbackEventArgs(string.Join("|", args));
			RaiseCustomDataCallback(e);
			return e.Result;
		}
		protected void CBSelectRows(string[] args) {
			if (args.Length < 2) return;
			string selText = args[0];
			if (selText == "all") {
				Selection.SelectAll();
				return;
			}
			if (selText == "unall") {
				Selection.UnselectAll();
				return;
			}
			DataBindNoControls();
			bool select;
			if (!bool.TryParse(selText, out select)) select = true;
			List<int> selection = new List<int>();
			for (int n = 1; n < args.Length; n++) {
				int i;
				if (!Int32.TryParse(args[n], out i)) continue;
				selection.Add(i);
			}
			for (int n = 0; n < selection.Count; n++) {
				Selection.SetSelection(selection[n], select);
			}
		}
		protected void CBSelectRowsKey(string[] args) {
			if (args.Length < 2) return;
			bool select;
			if (!bool.TryParse(args[0], out select)) select = true;
			List<object> selection = new List<object>();
			for (int n = 1; n < args.Length; n++) {
				object val = null;
				try {
					val = DataProxy.ConvertValue(KeyFieldName, args[n]);
				} catch {
				}
				if (val == null) continue;
				selection.Add(val);
			}
			for (int n = 0; n < selection.Count; n++) {
				Selection.SetSelectionByKey(selection[n], select);
			}
		}
		protected void CBGroup(string[] args) {
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[0]);
			if (column == null) return;
			int groupIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			string order = args[2];
			if (groupIndex == -1) {
				column.UnGroup();
			} else {
				if (groupIndex == -2) groupIndex = column.GroupIndex < 0 ? GroupCount : column.GroupIndex;
				GroupBy(column, groupIndex);
				column.SortOrder = GetSortOrder(column, order);
			}
		}
		protected void CBSort(string[] args) {
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[0]);
			if (column == null) return;
			int sortIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			string order = args[2];
			bool reset = GetBoolArg(GetArg(args, 3), true);
			if (SortCount == 1 && column.SortOrder != ColumnSortOrder.None) reset = false;
			if (reset) {
				ResetSortOnly(column);
			}
			if (sortIndex == -1) {
				column.SortIndex = -1;
			} else {
				if (sortIndex != -2)
					SortBy(column, sortIndex);
				column.SortOrder = GetSortOrder(column, order);
			}
			if (DataSourceForceStandardPaging || !DataProxy.IsBound) DataBind();
		}
		bool GetBoolArg(string argument, bool defaultValue) {
			bool res = false;
			if (!bool.TryParse(argument, out res)) res = defaultValue;
			return res;
		}
		string GetArg(string[] args, int pos) {
			if (pos >= args.Length) return string.Empty;
			return args[pos];
		}
		protected void CBMove(string[] args) {
			GridViewColumn column = Columns.GetColumnByStringArg(args[0]);
			GridViewColumn moveToColumn = Columns.GetColumnByStringArg(args[1]);
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			GridViewDataColumn moveToDataColumn = moveToColumn as GridViewDataColumn;
			if (column == null) return;
			bool moveBefore = GetBoolArg(GetArg(args, 2), true),
				 moveToGroup = GetBoolArg(GetArg(args, 3), false),
				 moveFromGroup = GetBoolArg(GetArg(args, 4), false);
			int visibleIndex = -1;
			if (moveToGroup && dataColumn != null) {
				ColumnMoveToGroupBy(dataColumn, moveToDataColumn, moveBefore);
			} else {
				ColumnMoveTo(column, moveToColumn, dataColumn, moveBefore, visibleIndex, moveFromGroup);
			}
			ResetVisibleColumns();
		}
		void ColumnMoveTo(GridViewColumn column, GridViewColumn moveToColumn, GridViewDataColumn dataColumn, bool moveBefore, int visibleIndex, bool moveFromGroup) {
			if (moveToColumn != null) {
				visibleIndex = moveToColumn.VisibleIndex;
				if (column.Visible && column.VisibleIndex < visibleIndex && column.VisibleIndex > -1)
					visibleIndex--;
				if (!moveBefore) visibleIndex++;
				if (visibleIndex < 0) visibleIndex = 0;
			}
			if (dataColumn != null && dataColumn.GroupIndex > -1 && moveFromGroup) {
				dataColumn.UnGroup();
			}
			column.VisibleIndex = visibleIndex;
		}
		void ColumnMoveToGroupBy(GridViewDataColumn dataColumn, GridViewDataColumn moveToDataColumn, bool moveBefore) {
			int groupIndex = GroupCount;
			if (moveToDataColumn != null && moveToDataColumn.GroupIndex > -1) {
				groupIndex = moveToDataColumn.GroupIndex;
				if (!moveBefore) groupIndex++;
			}
			dataColumn.GroupIndex = groupIndex;
		}
		protected void CBExpandAll(string[] args) {
			ExpandAll();
		}
		protected void CBCollapseAll(string[] args) {
			CollapseAll();
		}
		protected void CBExpandRow(string[] args) {
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return;
			bool recursive = GetBoolArg(GetArg(args, 1), false);
			ExpandRow(visibleIndex, recursive);
		}
		protected void CBCollapseRow(string[] args) {
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return;
			bool recursive = GetBoolArg(GetArg(args, 1), false);
			CollapseRow(visibleIndex, recursive);
		}
		protected void CBShowAllDetailRows(string[] args) {
			DetailRows.ExpandAllRows();
		}
		protected void CBHideAllDetailRows(string[] args) {
			DetailRows.CollapseAllRows();
		}
		protected void CBShowDetailRow(string[] args) {
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return;
			ChangeDetailRowExpandedState(visibleIndex, true);
		}
		protected void CBHideDetailRow(string[] args) {
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return;
			ChangeDetailRowExpandedState(visibleIndex, false);
		}
		void ChangeDetailRowExpandedState(int visibleIndex, bool expanded) {
			bool oldExpanded = DetailRows.IsVisible(visibleIndex);
			if (expanded)
				DetailRows.ExpandRow(visibleIndex);
			else
				DetailRows.CollapseRow(visibleIndex);
			bool newState = DetailRows.IsVisible(visibleIndex);
			if (oldExpanded != newState) {
				ASPxGridViewDetailRowEventArgs e = new ASPxGridViewDetailRowEventArgs(visibleIndex, newState);
				RaiseDetailRowExpandedChanged(e);
			}
		}
		protected void CBNextPage(string[] args) {
			MovePageOnCallback(PageIndex + 1, false);
		}
		protected void CBPrevPage(string[] args) {
			MovePageOnCallback(PageIndex - 1, false);
		}
		protected void CBGotoPage(string[] args) {
			if (args.Length < 1) return;
			int pageIndex;
			if (!Int32.TryParse(args[0], out pageIndex)) return;
			MovePageOnCallback(pageIndex, true);
		}
		protected void CBPagerOnClick(string[] args) {
			PageIndex = DevExpress.Web.ASPxPager.ASPxPagerBase.GetNewPageIndex(args[0], PageIndex, GetPageCountOnCallback, false);
		}
		protected void MovePageOnCallback(int newIndex, bool allowNegative) {
			int pageCount = GetPageCountOnCallback();
			if (newIndex >= pageCount) newIndex = Math.Max(0, pageCount - 1);
			if (newIndex < -1) newIndex = -1;
			if (!allowNegative && newIndex < 0) newIndex = 0;
			PageIndex = newIndex;
		}
		protected int GetPageCountOnCallback() {
			int pageCount = 0;
			if (IsAllowDataSourcePaging() && DataHelper != null) {
				pageCount = DataProxy.GetPageCount(DataHelper.RetrieveTotalCount());
			} else {
				DataBindNoControls();
				pageCount = DataProxy.PageCount;
			}
			return pageCount;
		}
		protected void CBApplyFilter(string[] args) {
			FilterExpression = string.Join("|", args);
		}
		protected void CBApplyColumnFilter(string[] args) {
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[0]);
			if (column == null) return;
			column.AutoFilterBy(string.Join("|", args, 1, args.Length - 1));
		}
		protected void CBApplyHeaderColumnFilter(string[] args) {
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[0]);
			if (column == null) return;
			column.FilterByHeaderPopup(string.Join("|", args, 1, args.Length - 1));
		}
		protected void CBFilterRowMenu(string[] args) {
			GridViewDataColumn column = Columns.GetDataColumnByStringArg(args[0]);
			if (column == null) return;
			string filterValue = FilterHelper.GetColumnAutoFilterText(column);
			column.Settings.AutoFilterCondition = (AutoFilterCondition)Int32.Parse(args[1]);
			column.AutoFilterBy(filterValue);
		}
		protected void CBStartEdit(string[] args) {
			LoadDataIfNotBinded(true);
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if (key == null) return;
			StartEdit(FindVisibleIndexByKeyValue(key));
		}
		protected void CBUpdateEdit(string[] args) {
			UpdateEdit();
		}
		protected void CBAddNewRow(string[] args) {
			AddNewRow();
		}
		protected void CBCancelEdit(string[] args) {
			CancelEdit();
		}
		protected void CBDeleteRow(string[] args) {
			LoadDataIfNotBinded(true);
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if (key == null) return;
			DeleteRow(FindVisibleIndexByKeyValue(key));
		}
		protected void CBCustomButton(string[] args) {
			if (args.Length != 2) return;
			string id = args[0];
			int visibleIndex;
			if (!Int32.TryParse(args[1], out visibleIndex)) return;
			RaiseCustomButton(id, visibleIndex);
		}
		protected void CBCustomCallBack(string[] args) {
			RaiseCustomCallback(new ASPxGridViewCustomCallbackEventArgs(string.Join("|", args)));
		}
		protected void CBShowFilterControl(string[] args) {
			ShowFilterControl();
		}
		protected void CBCloseFilterControl(string[] args) {
			HideFilterControl();
		}
		protected void CBSetFilterEnabled(string[] args) {
			if (args.Length < 1) return;
			bool isFilterEnabled;
			if (!bool.TryParse(args[0], out isFilterEnabled)) return;
			FilterEnabled = isFilterEnabled;
		}
		protected void CBRefresh(string[] args) {
			DataBind();
		}
		ColumnSortOrder GetSortOrder(GridViewDataColumn column, string order) {
			switch (order) {
				case "NONE": return ColumnSortOrder.None;
				case "DSC": return ColumnSortOrder.Descending;
				case "ASC": return ColumnSortOrder.Ascending;
			}
			if (column.SortOrder == ColumnSortOrder.Ascending)
				return ColumnSortOrder.Descending;
			return ColumnSortOrder.Ascending;
		}
		protected delegate void ASPxGridCallBackMethod(string[] args);
		protected internal delegate object ASPxGridCallBackFunction(string[] args);
		internal void OnColumnCollectionChanged() {
			this.webColumnsOwnerImpl.OnColumnCollectionChanged();
		}
		internal void EnsureVisibleColumns() {
			this.webColumnsOwnerImpl.EnsureVisibleColumns();
		}
		protected internal virtual void OnSettingsChanged(object settings) {
			if (IsLoading()) return;
			if (settings == SettingsBehavior) SetDataProxyAllowFocusedRow();
			LayoutChanged();
		}
		void SetDataProxyAllowFocusedRow() {
			DataProxy.AllowFocusedRow = SettingsBehavior.AllowFocusedRow;
		}
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			ASPxGridViewRowCommandEventArgs rowCommand = args as ASPxGridViewRowCommandEventArgs;
			if (rowCommand != null) {
				RaiseRowCommand(rowCommand);
				return true;
			}
			return base.OnBubbleEvent(source, args);
		}
		#region EVENTS
		[Category("Rendering")]
		public event ASPxGridViewEditorEventHandler CellEditorInitialize {
			add { Events.AddHandler(cellEditorInitialize, value); }
			remove { Events.RemoveHandler(cellEditorInitialize, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewEditorEventHandler AutoFilterCellEditorInitialize {
			add { Events.AddHandler(autoFilterCellEditorInitialize, value); }
			remove { Events.RemoveHandler(autoFilterCellEditorInitialize, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewEditorCreateEventHandler AutoFilterCellEditorCreate {
			add { Events.AddHandler(autoFilterCellEditorCreate, value); }
			remove { Events.RemoveHandler(autoFilterCellEditorCreate, value); }
		}
		[Category("Data")]
		public event ASPxGridViewHeaderFilterEventHandler HeaderFilterFillItems {
			add { Events.AddHandler(headerFilterFillItems, value); }
			remove { Events.RemoveHandler(headerFilterFillItems, value); }
		}
		[Category("Action")]
		public event EventHandler PageIndexChanged {
			add { Events.AddHandler(pageIndexChanged, value); }
			remove { Events.RemoveHandler(pageIndexChanged, value); }
		}
		[Category("Data")]
		public event EventHandler BeforePerformDataSelect {
			add { Events.AddHandler(beforePerformDataSelect, value); }
			remove { Events.RemoveHandler(beforePerformDataSelect, value); }
		}
		[Category("Data")]
		public event ASPxGridViewAutoFilterEventHandler ProcessColumnAutoFilter {
			add { Events.AddHandler(processColumnAutoFilter, value); }
			remove { Events.RemoveHandler(processColumnAutoFilter, value); }
		}
		[Category("Action")]
		public event ASPxGridViewRowCommandEventHandler RowCommand {
			add { Events.AddHandler(rowCommand, value); }
			remove { Events.RemoveHandler(rowCommand, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewTableRowEventHandler HtmlRowCreated {
			add { Events.AddHandler(htmlRowCreated, value); }
			remove { Events.RemoveHandler(htmlRowCreated, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewTableRowEventHandler HtmlRowPrepared {
			add { Events.AddHandler(htmlRowPrepared, value); }
			remove { Events.RemoveHandler(htmlRowPrepared, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewTableDataCellEventHandler HtmlDataCellPrepared {
			add { Events.AddHandler(htmlDataCellPrepared, value); }
			remove { Events.RemoveHandler(htmlDataCellPrepared, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewTableCommandCellEventHandler HtmlCommandCellPrepared {
			add { Events.AddHandler(htmlCommandCellPrepared, value); }
			remove { Events.RemoveHandler(htmlCommandCellPrepared, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewEditFormEventHandler HtmlEditFormCreated {
			add { Events.AddHandler(htmlEditFormCreated, value); }
			remove { Events.RemoveHandler(htmlEditFormCreated, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewTableFooterCellEventHandler HtmlFooterCellPrepared {
			add { Events.AddHandler(htmlFooterCellPrepared, value); }
			remove { Events.RemoveHandler(htmlFooterCellPrepared, value); }
		}
		[Category("Action")]
		public event EventHandler FocusedRowChanged {
			add { Events.AddHandler(focusedRowChanged, value); }
			remove { Events.RemoveHandler(focusedRowChanged, value); }
		}
		[Category("Action")]
		public event ASPxDataUpdatedEventHandler RowUpdated {
			add { Events.AddHandler(rowUpdated, value); }
			remove { Events.RemoveHandler(rowUpdated, value); }
		}
		[Category("Action")]
		public event ASPxDataUpdatingEventHandler RowUpdating {
			add { Events.AddHandler(rowUpdating, value); }
			remove { Events.RemoveHandler(rowUpdating, value); }
		}
		[Category("Action")]
		public event ASPxStartRowEditingEventHandler StartRowEditing {
			add { Events.AddHandler(startRowEditing, value); }
			remove { Events.RemoveHandler(startRowEditing, value); }
		}
		[Category("Action")]
		public event ASPxStartRowEditingEventHandler CancelRowEditing {
			add { Events.AddHandler(cancelRowEditing, value); }
			remove { Events.RemoveHandler(cancelRowEditing, value); }
		}
		[Category("Data")]
		public event ASPxDataInitNewRowEventHandler InitNewRow {
			add { Events.AddHandler(initNewRow, value); }
			remove { Events.RemoveHandler(initNewRow, value); }
		}
		[Category("Action")]
		public event ASPxDataDeletedEventHandler RowDeleted {
			add { Events.AddHandler(rowDeleted, value); }
			remove { Events.RemoveHandler(rowDeleted, value); }
		}
		[Category("Data")]
		public event ASPxParseValueEventHandler ParseValue {
			add { Events.AddHandler(parseValue, value); }
			remove { Events.RemoveHandler(parseValue, value); }
		}
		[Category("Action")]
		public event ASPxDataDeletingEventHandler RowDeleting {
			add { Events.AddHandler(rowDeleting, value); }
			remove { Events.RemoveHandler(rowDeleting, value); }
		}
		[Category("Action")]
		public event ASPxDataValidationEventHandler RowValidating {
			add { Events.AddHandler(rowValidating, value); }
			remove { Events.RemoveHandler(rowValidating, value); }
		}
		[Category("Action")]
		public event ASPxDataInsertedEventHandler RowInserted {
			add { Events.AddHandler(rowInserted, value); }
			remove { Events.RemoveHandler(rowInserted, value); }
		}
		[Category("Action")]
		public event ASPxDataInsertingEventHandler RowInserting {
			add { Events.AddHandler(rowInserting, value); }
			remove { Events.RemoveHandler(rowInserting, value); }
		}
		[Category("Action")]
		public event ASPxGridViewCustomCallbackEventHandler CustomCallback {
			add { Events.AddHandler(customCallback, value); }
			remove { Events.RemoveHandler(customCallback, value); }
		}
		[Category("Action")]
		public event ASPxGridViewCustomButtonCallbackEventHandler CustomButtonCallback {
			add { Events.AddHandler(customButtonCallback, value); }
			remove { Events.RemoveHandler(customButtonCallback, value); }
		}
		[Category("Data")]
		public event ASPxGridViewCustomDataCallbackEventHandler CustomDataCallback {
			add { Events.AddHandler(customDataCallback, value); }
			remove { Events.RemoveHandler(customDataCallback, value); }
		}
		[Category("Action")]
		public event ASPxGridViewAfterPerformCallbackEventHandler AfterPerformCallback {
			add { Events.AddHandler(afterPerformCallback, value); }
			remove { Events.RemoveHandler(afterPerformCallback, value); }
		}
		[Category("Data")]
		public event ASPxGridViewColumnDataEventHandler CustomUnboundColumnData {
			add { Events.AddHandler(customUnboundColumnData, value); }
			remove { Events.RemoveHandler(customUnboundColumnData, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewColumnDisplayTextEventHandler CustomColumnDisplayText {
			add { Events.AddHandler(customColumnDisplayText, value); }
			remove { Events.RemoveHandler(customColumnDisplayText, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewColumnDisplayTextEventHandler CustomGroupDisplayText {
			add { Events.AddHandler(customGroupDisplayText, value); }
			remove { Events.RemoveHandler(customGroupDisplayText, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewCustomErrorTextEventHandler CustomErrorText {
			add { Events.AddHandler(customErrorText, value); }
			remove { Events.RemoveHandler(customErrorText, value); }
		}
		[Category("Data")]
		public event CustomSummaryEventHandler CustomSummaryCalculate {
			add { Events.AddHandler(customSummaryCalculate, value); }
			remove { Events.RemoveHandler(customSummaryCalculate, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewSummaryDisplayTextEventHandler SummaryDisplayText {
			add { Events.AddHandler(summaryDisplayText, value); }
			remove { Events.RemoveHandler(summaryDisplayText, value); }
		}
		[Category("Action")]
		public event EventHandler SelectionChanged {
			add { Events.AddHandler(selectionChanged, value); }
			remove { Events.RemoveHandler(selectionChanged, value); }
		}
		[Category("Action")]
		public event EventHandler DetailsChanged {
			add { Events.AddHandler(detailsChanged, value); }
			remove { Events.RemoveHandler(detailsChanged, value); }
		}
		[Category("Action")]
		public event ASPxGridViewDetailRowEventHandler DetailRowExpandedChanged {
			add { Events.AddHandler(detailRowExpandedChanged, value); }
			remove { Events.RemoveHandler(detailRowExpandedChanged, value); }
		}
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(clientLayout, value); }
			remove { Events.RemoveHandler(clientLayout, value); }
		}
		[Category("Data")]
		public event ASPxGridViewCustomColumnSortEventHandler CustomColumnSort {
			add { Events.AddHandler(customColumnSort, value); }
			remove { Events.RemoveHandler(customColumnSort, value); }
		}
		[Category("Data")]
		public event ASPxGridViewCustomColumnSortEventHandler CustomColumnGroup {
			add { Events.AddHandler(customColumnGroup, value); }
			remove { Events.RemoveHandler(customColumnGroup, value); }
		}
		[Category("Data")]
		public event ASPxGridViewBeforeColumnGroupingSortingEventHandler BeforeColumnSortingGrouping {
			add { Events.AddHandler(beforeColumnSortingGrouping, value); }
			remove { Events.RemoveHandler(beforeColumnSortingGrouping, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewDetailRowButtonEventHandler DetailRowGetButtonVisibility {
			add { Events.AddHandler(detailRowGetButtonVisibility, value); }
			remove { Events.RemoveHandler(detailRowGetButtonVisibility, value); }
		}
		[Category("Client-Side")]
		public event ASPxGridViewClientJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(beforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(beforeGetCallbackResult, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewCustomButtonEventHandler CustomButtonInitialize {
			add { Events.AddHandler(customButtonInitialize, value); }
			remove { Events.RemoveHandler(customButtonInitialize, value); }
		}
		[Category("Rendering")]
		public event ASPxGridViewCommandButtonEventHandler CommandButtonInitialize {
			add { Events.AddHandler(commandButtonInitialize, value); }
			remove { Events.RemoveHandler(commandButtonInitialize, value); }
		}
		[Category("Rendering")]
		public event CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText {
			add { Events.AddHandler(customFilterExpressionDisplayText, value); }
			remove { Events.RemoveHandler(customFilterExpressionDisplayText, value); }
		}
		protected internal virtual void RaiseCustomColumnSort(CustomColumnSortEventArgs e) {
			ASPxGridViewCustomColumnSortEventHandler handler = (ASPxGridViewCustomColumnSortEventHandler)Events[customColumnSort];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomColumnGroup(CustomColumnSortEventArgs e) {
			ASPxGridViewCustomColumnSortEventHandler handler = (ASPxGridViewCustomColumnSortEventHandler)Events[customColumnGroup];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeColumnSortingGrouping(ASPxGridViewBeforeColumnGroupingSortingEventArgs e) {
			ASPxGridViewBeforeColumnGroupingSortingEventHandler handler = (ASPxGridViewBeforeColumnGroupingSortingEventHandler)Events[beforeColumnSortingGrouping];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseEditorInitialize(ASPxGridViewEditorEventArgs e) {
			ASPxGridViewEditorEventHandler handler = (ASPxGridViewEditorEventHandler)Events[cellEditorInitialize];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseAutoFilterEditorInitialize(ASPxGridViewEditorEventArgs e) {
			ASPxGridViewEditorEventHandler handler = (ASPxGridViewEditorEventHandler)Events[autoFilterCellEditorInitialize];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseAutoFilterEditorCreate(ASPxGridViewEditorCreateEventArgs e) {
			ASPxGridViewEditorCreateEventHandler handler = (ASPxGridViewEditorCreateEventHandler)Events[autoFilterCellEditorCreate];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHeaderFilterFillItems(ASPxGridViewHeaderFilterEventArgs e) {
			ASPxGridViewHeaderFilterEventHandler handler = (ASPxGridViewHeaderFilterEventHandler)Events[headerFilterFillItems];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaisePageIndexChanged() {
			EventHandler handler = (EventHandler)Events[pageIndexChanged];
			if (handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseBeforePerformDataSelect() {
			EventHandler handler = (EventHandler)Events[beforePerformDataSelect];
			if (handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseProcessColumnAutoFilter(ASPxGridViewAutoFilterEventArgs e) {
			ASPxGridViewAutoFilterEventHandler handler = (ASPxGridViewAutoFilterEventHandler)Events[processColumnAutoFilter];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectionChanged() {
			EventHandler handler = (EventHandler)Events[selectionChanged];
			if (handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseDetailRowsChanged() {
			EventHandler handler = (EventHandler)Events[detailsChanged];
			if (handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseDetailRowExpandedChanged(ASPxGridViewDetailRowEventArgs e) {
			ASPxGridViewDetailRowEventHandler handler = (ASPxGridViewDetailRowEventHandler)Events[detailRowExpandedChanged];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDetailRowGetButtonVisibility(ASPxGridViewDetailRowButtonEventArgs e) {
			ASPxGridViewDetailRowButtonEventHandler handler = (ASPxGridViewDetailRowButtonEventHandler)Events[detailRowGetButtonVisibility];
			if (handler != null) handler(this, e);
		}
		protected internal virtual string RaiseCustomErrorText(ASPxGridViewCustomErrorTextEventArgs e) {
			ASPxGridViewCustomErrorTextEventHandler handler = (ASPxGridViewCustomErrorTextEventHandler)Events[customErrorText];
			if (handler != null) handler(this, e);
			return e.ErrorText;
		}
		protected virtual void RaiseCustomSummaryCalculate(CustomSummaryEventArgs e) {
			CustomSummaryEventHandler handler = (CustomSummaryEventHandler)Events[customSummaryCalculate];
			if (handler != null) handler(this, e);
		}
		protected internal virtual string RaiseSummaryDisplayText(ASPxGridViewSummaryDisplayTextEventArgs e) {
			ASPxGridViewSummaryDisplayTextEventHandler handler = (ASPxGridViewSummaryDisplayTextEventHandler)Events[summaryDisplayText];
			if (handler != null) handler(this, e);
			return e.Text;
		}
		protected virtual void RaiseCustomUnboundColumnData(ASPxGridViewColumnDataEventArgs e) {
			ASPxGridViewColumnDataEventHandler handler = (ASPxGridViewColumnDataEventHandler)Events[customUnboundColumnData];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomColumnDisplayText(ASPxGridViewColumnDisplayTextEventArgs e) {
			ASPxGridViewColumnDisplayTextEventHandler handler = (ASPxGridViewColumnDisplayTextEventHandler)Events[customColumnDisplayText];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomGroupDisplayText(ASPxGridViewColumnDisplayTextEventArgs e) {
			ASPxGridViewColumnDisplayTextEventHandler handler = (ASPxGridViewColumnDisplayTextEventHandler)Events[customGroupDisplayText];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseCustomCallback(ASPxGridViewCustomCallbackEventArgs e) {
			ASPxGridViewCustomCallbackEventHandler handler = (ASPxGridViewCustomCallbackEventHandler)Events[customCallback];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseCustomButton(string buttonID, int visibleIndex) {
			ASPxGridViewCustomButtonCallbackEventHandler handler = (ASPxGridViewCustomButtonCallbackEventHandler)Events[customButtonCallback];
			if (handler != null) {
				handler(this, new ASPxGridViewCustomButtonCallbackEventArgs(buttonID, visibleIndex));
			}
		}
		protected internal virtual void RaiseCustomButtonInitialize(ASPxGridViewCustomButtonEventArgs e) {
			ASPxGridViewCustomButtonEventHandler handler = (ASPxGridViewCustomButtonEventHandler)Events[customButtonInitialize];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCommandButtonInitialize(ASPxGridViewCommandButtonEventArgs e) {
			ASPxGridViewCommandButtonEventHandler handler = (ASPxGridViewCommandButtonEventHandler)Events[commandButtonInitialize];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseCustomDataCallback(ASPxGridViewCustomDataCallbackEventArgs e) {
			ASPxGridViewCustomDataCallbackEventHandler handler = (ASPxGridViewCustomDataCallbackEventHandler)Events[customDataCallback];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseAfterPerformCallback(ASPxGridViewAfterPerformCallbackEventArgs e) {
			ASPxGridViewAfterPerformCallbackEventHandler handler = (ASPxGridViewAfterPerformCallbackEventHandler)Events[afterPerformCallback];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRowCommand(ASPxGridViewRowCommandEventArgs e) {
			ASPxGridViewRowCommandEventHandler handler = (ASPxGridViewRowCommandEventHandler)Events[rowCommand];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlRowCreated(GridViewTableRow row) {
			ASPxGridViewTableRowEventHandler handler = (ASPxGridViewTableRowEventHandler)Events[htmlRowCreated];
			if (handler == null) return;
			row.EnsureChildControlsRecursive();
			ASPxGridViewTableRowEventArgs e = new ASPxGridViewTableRowEventArgs(row, DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(row.VisibleIndex) : null);
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlRowPrepared(GridViewTableRow row) {
			ASPxGridViewTableRowEventHandler handler = (ASPxGridViewTableRowEventHandler)Events[htmlRowPrepared];
			if (handler == null) return;
			ASPxGridViewTableRowEventArgs e = new ASPxGridViewTableRowEventArgs(row, DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(row.VisibleIndex) : null);
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlDataCellPrepared(GridViewTableDataCell cell) {
			ASPxGridViewTableDataCellEventHandler handler = (ASPxGridViewTableDataCellEventHandler)Events[htmlDataCellPrepared];
			if (handler == null) return;
			ASPxGridViewTableDataCellEventArgs e = new ASPxGridViewTableDataCellEventArgs(cell, DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(cell.VisibleIndex) : null);
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlCommandCellPrepared(GridViewTableBaseCommandCell cell) {
			ASPxGridViewTableCommandCellEventHandler handler = (ASPxGridViewTableCommandCellEventHandler)Events[htmlCommandCellPrepared];
			if (handler == null) return;
			object keyValue = cell.VisibleIndex > -1 && DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(cell.VisibleIndex) : null;
			ASPxGridViewTableCommandCellEventArgs e = new ASPxGridViewTableCommandCellEventArgs(cell, keyValue);
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlEditFormCreated(WebControl container) {
			ASPxGridViewEditFormEventHandler handler = (ASPxGridViewEditFormEventHandler)Events[htmlEditFormCreated];
			if (handler == null) return;
			ASPxGridViewRenderHelper.EnsureHierarchy(container);
			handler(this, new ASPxGridViewEditFormEventArgs(container));
		}
		protected internal virtual void RaiseHtmlFooterCellPrepared(GridViewColumn column, int visibleIndex, TableCell cell) {
			ASPxGridViewTableFooterCellEventHandler handler = (ASPxGridViewTableFooterCellEventHandler)Events[htmlFooterCellPrepared];
			if (handler == null) return;
			ASPxGridViewTableFooterCellEventArgs e = new ASPxGridViewTableFooterCellEventArgs(this, column, visibleIndex, cell);
			handler(this, e);
		}
		protected virtual void RaiseStartEditingRow(ASPxStartRowEditingEventArgs e) {
			ASPxStartRowEditingEventHandler handler = (ASPxStartRowEditingEventHandler)Events[startRowEditing];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseCancelEditingRow(ASPxStartRowEditingEventArgs e) {
			ASPxStartRowEditingEventHandler handler = (ASPxStartRowEditingEventHandler)Events[cancelRowEditing];
			if (handler != null) handler(this, e);
		}
		protected virtual void CheckPendingEvents() {
			if (PendingEvents.CheckClear(focusedRowChanged)) RaiseFocusedRowChanged(EventArgs.Empty);
			if (PendingEvents.CheckClear(selectionChanged)) RaiseSelectionChanged();
		}
		protected virtual void RaiseFocusedRowChanged(EventArgs e) {
			this.fireFocusedRowChangedOnClient = true;
			EventHandler handler = (EventHandler)Events[focusedRowChanged];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowUpdated(ASPxDataUpdatedEventArgs e) {
			ASPxDataUpdatedEventHandler handler = (ASPxDataUpdatedEventHandler)Events[rowUpdated];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowUpdating(ASPxDataUpdatingEventArgs e) {
			ASPxDataUpdatingEventHandler handler = (ASPxDataUpdatingEventHandler)Events[rowUpdating];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowDeleted(ASPxDataDeletedEventArgs e) {
			ASPxDataDeletedEventHandler handler = (ASPxDataDeletedEventHandler)Events[rowDeleted];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseParseValue(ASPxParseValueEventArgs e) {
			ASPxParseValueEventHandler handler = (ASPxParseValueEventHandler)Events[parseValue];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowDeleting(ASPxDataDeletingEventArgs e) {
			ASPxDataDeletingEventHandler handler = (ASPxDataDeletingEventHandler)Events[rowDeleting];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowValidating(ASPxDataValidationEventArgs e) {
			ASPxDataValidationEventHandler handler = (ASPxDataValidationEventHandler)Events[rowValidating];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowInserted(ASPxDataInsertedEventArgs e) {
			ASPxDataInsertedEventHandler handler = (ASPxDataInsertedEventHandler)Events[rowInserted];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseRowInserting(ASPxDataInsertingEventArgs e) {
			ASPxDataInsertingEventHandler handler = (ASPxDataInsertingEventHandler)Events[rowInserting];
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseInitNewRow(ASPxDataInitNewRowEventArgs e) {
			ASPxDataInitNewRowEventHandler handler = (ASPxDataInitNewRowEventHandler)Events[initNewRow];
			if (handler != null) handler(this, e);
		}
		protected override IDictionary<string, object> GetCustomJSProperties() {
			ASPxGridViewClientJSPropertiesEventArgs e = new ASPxGridViewClientJSPropertiesEventArgs(JSPropertiesInternal);
			RaiseCustomJSProperties(e);
			if (e.Properties.Count > 0)
				return e.Properties;
			return null;
		}
		protected void RaiseCustomJSProperties(ASPxGridViewClientJSPropertiesEventArgs e) {
			ASPxGridViewClientJSPropertiesEventHandler handler = Events[EventCustomJsProperties] as ASPxGridViewClientJSPropertiesEventHandler;
			if (handler != null) handler(this, e);
		}
		protected virtual void RaiseBeforeGetCallbackResult(EventArgs e) {
			EventHandler handler = (EventHandler)Events[beforeGetCallbackResult];
			if (handler != null) handler(this, e);
		}
		protected virtual bool IsClientLayoutExists { get { return Events[clientLayout] != null; } }
		protected virtual void RaiseClientLayout(ASPxClientLayoutArgs e) {
			ASPxClientLayoutHandler handler = (ASPxClientLayoutHandler)Events[clientLayout];
			if (handler != null) handler(this, e);
		}
		void IWebDataEvents.OnStartRowEditing(ASPxStartRowEditingEventArgs e) { RaiseStartEditingRow(e); }
		void IWebDataEvents.OnCancelRowEditing(ASPxStartRowEditingEventArgs e) { RaiseCancelEditingRow(e); }
		void IWebDataEvents.OnFocusedRowChanged() {
			if (!Loaded)
				PendingEvents.SetPending(focusedRowChanged);
			else
				RaiseFocusedRowChanged(EventArgs.Empty);
		}
		void IWebDataEvents.OnParseValue(ASPxParseValueEventArgs e) { RaiseParseValue(e); }
		void IWebDataEvents.OnRowDeleting(ASPxDataDeletingEventArgs e) { RaiseRowDeleting(e); }
		void IWebDataEvents.OnRowDeleted(ASPxDataDeletedEventArgs e) { RaiseRowDeleted(e); }
		void IWebDataEvents.OnRowValidating(ASPxDataValidationEventArgs e) {
			OnRowValidatingCore(e);
		}
		void IWebDataEvents.OnInitNewRow(ASPxDataInitNewRowEventArgs e) { RaiseInitNewRow(e); }
		void IWebDataEvents.OnRowInserting(ASPxDataInsertingEventArgs e) { RaiseRowInserting(e); }
		void IWebDataEvents.OnRowInserted(ASPxDataInsertedEventArgs e) { RaiseRowInserted(e); }
		void IWebDataEvents.OnRowUpdating(ASPxDataUpdatingEventArgs e) { RaiseRowUpdating(e); }
		void IWebDataEvents.OnRowUpdated(ASPxDataUpdatedEventArgs e) { RaiseRowUpdated(e); }
		void IWebDataEvents.OnCustomSummary(CustomSummaryEventArgs e) { RaiseCustomSummaryCalculate(e); }
		object IWebDataEvents.GetUnboundData(int listSourceRowIndex, string fieldName) {
			ASPxGridViewColumnDataEventArgs e = new ASPxGridViewColumnDataEventArgs((GridViewDataColumn)Columns[fieldName], listSourceRowIndex, null, true);
			RaiseCustomUnboundColumnData(e);
			return e.Value;
		}
		void IWebDataEvents.OnSummaryExists(CustomSummaryExistEventArgs e) {
			OnSummaryExists(e);
		}
		void IWebDataEvents.SetUnboundData(int listSourceRowIndex, string fieldName, object value) {
			ASPxGridViewColumnDataEventArgs e = new ASPxGridViewColumnDataEventArgs((GridViewDataColumn)Columns[fieldName], listSourceRowIndex, value, false);
			RaiseCustomUnboundColumnData(e);
		}
		void IWebDataEvents.OnSelectionChanged() {
			this.fireSelectionChangedOnClient = true;
			LayoutChanged();
			if (!Loaded)
				PendingEvents.SetPending(selectionChanged);
			else
				RaiseSelectionChanged();
		}
		void IWebDataEvents.OnDetailRowsChanged() {
			LayoutChanged();
			RaiseDetailRowsChanged();
		}
		#endregion
		protected virtual void OnRowValidatingCore(ASPxDataValidationEventArgs e) {
			RenderHelper.ValidationError.Clear();
			RenderHelper.ResetEditingErrorText();
			RaiseRowValidating(e);
			if (!string.IsNullOrEmpty(e.RowError)) {
				RenderHelper.EditingErrorText = EncodeText(RaiseCustomErrorText(new ASPxGridViewCustomErrorTextEventArgs(GridViewErrorTextKind.RowValidate, e.RowError)));
			}
			foreach (KeyValuePair<GridViewColumn, string> pair in e.Errors) {
				RenderHelper.ValidationError.Add(pair.Key, EncodeText(pair.Value));
			}
		}
		protected override string OnCallbackException(Exception e) {
			return EncodeText(RaiseCustomErrorText(new ASPxGridViewCustomErrorTextEventArgs(e, GridViewErrorTextKind.General, base.OnCallbackException(e))));
		}
		string EncodeText(string text) {
			if (!SettingsBehavior.EncodeErrorHtml || string.IsNullOrEmpty(text)) return text;
			return System.Web.HttpUtility.HtmlEncode(text);
		}
		protected virtual void OnSummaryExists(CustomSummaryExistEventArgs e) {
			if (e.IsGroupSummary) {
				ASPxSummaryItem item = e.Item as ASPxSummaryItem;
				if (item != null && !string.IsNullOrEmpty(item.ShowInColumn)) {
					if (e.GroupLevel < SortedColumns.Count && !SortedColumns[e.GroupLevel].IsEquals(item.ShowInColumn)) {
						e.Exists = false;
					}
				}
			}
		}
		protected internal virtual void OnColumnChanged(GridViewColumn column) {
			this.webColumnsOwnerImpl.OnColumnChanged(column);
		}
		protected internal virtual void OnSummaryChanged(object sender, CollectionChangeEventArgs e) {
			if (IsLoading()) return;
			LayoutChanged();
			BindAndSynchronizeDataProxy();
		}
		protected internal virtual void OnGroupSummaryChanged(object sender, CollectionChangeEventArgs e) {
			if (IsLoading()) return;
			LayoutChanged();
			BindAndSynchronizeDataProxy();
		}
		protected internal void OnColumnBindingChanged(GridViewDataColumn column) {
			if (IsLoading()) return;
			LayoutChanged();
			DataProxy.UpdateColumnBindings();
		}
		protected void LoadDataIfNotBinded() {
			LoadDataIfNotBinded(false);
		}
		protected void LoadDataIfNotBinded(bool bindIfNotCached) {
			if (EnableRowsCache) {
				if (!DataProxy.HasCachedProvider) {
					string state = (string)GetCallbackPropertyValue("Data", string.Empty);
					if (!string.IsNullOrEmpty(state)) {
						DataProxy.LoadCachedData(state);
						if (DataProxy.IsReady) return;
						DataProxy.SetCachedDataProvider();
					}
				}
			}
			if (DataProxy.IsReady) return;
			if (bindIfNotCached) DataBindNoControls();
		}
		bool lockDataBindNoControls = false; 
		protected internal void DataBindNoControls() {
			if (!DataProxy.IsBound && !this.lockDataBindNoControls) {
				this.lockDataBindNoControls = true;
				try {
					PerformSelect();
				} finally {
					this.lockDataBindNoControls = false;
				}
			}
		}
		bool needSyncrhonizeDataProxy = false;
		protected void CheckBindAndSynchronizeDataProxy() {
			if (needSyncrhonizeDataProxy) BindAndSynchronizeDataProxy();
		}
		protected void BindAndSynchronizeDataProxy() {
			this.needSyncrhonizeDataProxy = true;
			if (IsLockUpdate) return;
			this.needSyncrhonizeDataProxy = false;
			if (!DataProxy.IsBound) {
				RequireDataBinding();
				DataBindNoControls();
			} else {
				SynchronizeDataProxy();
			}
			if (FilterEnabled) {
				SetCallbackPropertyValue("FilterExpression", string.Empty, DataProxy.FilterExpression);
			}
			DataProxy.CheckFocusedRowChanged();
			LayoutChanged();
		}
		bool hierarchyChanged = false;
		void SynchronizeDataProxy() {
			this.hierarchyChanged = false;
			List<IWebColumnInfo> sortList = new List<IWebColumnInfo>();
			foreach (GridViewDataColumn column in SortedColumns) {
				sortList.Add(column);
			}
			DataProxy.SortGroupChanged(sortList, GroupCount, GetEnabledFilterExpression());
			DataProxy.RestoreRowsState();
			if (IsFirstLoad && SettingsBehavior.AutoExpandAllGroups) {
				ExpandAll();
			}
			if (this.hierarchyChanged) LayoutChanged();
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new GridViewDataHelper(this, name);
		}
		public override void DataBind() {
			foreach (GridViewDataColumn column in DataColumns) {
				if (column.PropertiesEdit != null)
					column.PropertiesEdit.RequireDataBinding();
			}
			base.DataBind();
		}
		protected override void PerformDataBinding(string dataHelperName, System.Collections.IEnumerable data) {
			if (!PreRendered) PopulateAutoGeneratedColumns(data);
			DataProxy.SetDataSource(data);
			if (!PreRendered) ResetControlHierarchy();
		}
		protected override void OnDataBound(EventArgs e) {
			BuildSortedColumns();
			SynchronizeDataProxy();
			base.OnDataBound(e);
		}
		protected virtual void PopulateAutoGeneratedColumns(System.Collections.IEnumerable data) {
			if (!AutoGenerateColumns || Columns.Count > 0) return;
			if (data == null) return;
			IEnumerable<DataColumnInfo> columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(data);
			if (columns == null) {
				columns = GetDataColumnInfoSecondTry(data);
				if (columns == null) return;
			}
			foreach (DataColumnInfo column in columns) {
				if (!CanPopulateAutoGeneratedColumn(column)) continue;
				GridViewDataColumn dataCol = GridViewEditDataColumn.CreateColumn(column.Type);
				dataCol.AutoGenerated = true;
				dataCol.FieldName = column.Name;
				dataCol.ReadOnly = column.ReadOnly;
				Columns.Add(dataCol);
			}
		}
		IEnumerable<DataColumnInfo> GetDataColumnInfoSecondTry(IEnumerable data) {
			ICustomTypeDescriptor descriptor = null;
			IEnumerator enumerator = data.GetEnumerator();
			if (enumerator.MoveNext())
				descriptor = enumerator.Current as ICustomTypeDescriptor;
			if (descriptor == null) return null;
			List<DataColumnInfo> list = new List<DataColumnInfo>();
			foreach (PropertyDescriptor prop in descriptor.GetProperties())
				list.Add(new DataColumnInfo(prop));
			return list;
		}
		protected virtual bool CanPopulateAutoGeneratedColumn(DataColumnInfo column) {
			return true;
		}
		protected internal List<ASPxSummaryItem> GetTotalSummaryItems(GridViewDataColumn column) {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach (ASPxSummaryItem item in TotalSummary) {
				if (item.SummaryType == SummaryItemType.None) continue;
				if (string.IsNullOrEmpty(item.ShowInColumn)) {
					if (item.FieldName != column.FieldName) continue;
				} else {
					if (!column.IsNameOrFieldOrCaption(item.ShowInColumn)) continue;
				}
				res.Add(item);
			}
			return res;
		}
		protected internal List<ASPxSummaryItem> GetGroupFooterSummaryItems(GridViewDataColumn column) {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach (ASPxSummaryItem item in GroupSummary) {
				if (column.IsNameOrFieldOrCaption(item.ShowInGroupFooterColumn)) {
					res.Add(item);
				}
			}
			return res;
		}
		protected internal bool InternalIsCallBacksEnabled() {
			return EnableCallBacks;
		}
		internal void EnsureChildControlsCore() {
			EnsureChildControls();
		}
		protected internal virtual int CalcBrowserDependentGroupButtonWidth() {
			int width = Styles.GroupButtonWidth;
			width += 7;
			if (RenderHelper.IndentColumnCount < 2 && RenderHelper.GroupCount < 1) {
				if (width < 14) width = 14;
				if (RenderUtils.IsIE)
					width -= 13;
			}
			return width;
		}
		protected virtual GridViewColumnCollection CreateColumnCollection() {
			return new GridViewColumnCollection(this);
		}
		#region IDataControllerSort Members
		void IDataControllerSort.AfterGrouping() {
		}
		void IDataControllerSort.AfterSorting() {
		}
		void IDataControllerSort.BeforeGrouping() {
		}
		void IDataControllerSort.BeforeSorting() {
			SortData.OnStart();
		}
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value) {
			return FilterData.GetDisplayText(listSourceRow, info, value);
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) {
			return FilterData.IsRequired(column);
		}
		bool IDataControllerSort.RequireSortCell(DataColumnInfo column) {
			return SortData.IsRequired(column);
		}
		int IDataControllerSort.SortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			BaseGridColumnInfo info = SortData.GetInfo(sortColumn);
			if (info == null) return 3;
			return info.CompareSortValues(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
		}
		int IDataControllerSort.SortGroupCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) {
			BaseGridColumnInfo info = SortData.GetInfo(sortColumn);
			if (info == null) return 3;
			return info.CompareGroupValues(listSourceRow1, listSourceRow2, value1, value2);
		}
		int IDataControllerSort.SortRow(int listSourceRow1, int listSourceRow2) {
			return 3;
		}
		#endregion
		#region IWebColumnsOwner implementation
		WebColumnCollectionBase IWebColumnsOwner.Columns { get { return this.webColumnsOwnerImpl.Columns; } }
		void IWebColumnsOwner.BuildVisibleColumns() { this.webColumnsOwnerImpl.BuildVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleColumns() { ResetVisibleColumns(); }
		void IWebColumnsOwner.EnsureVisibleColumns() { this.webColumnsOwnerImpl.EnsureVisibleColumns(); }
		List<WebColumnBase> IWebColumnsOwner.GetVisibleColumns() { return this.webColumnsOwnerImpl.GetVisibleColumns(); }
		bool IWebColumnsOwner.SetColumnVisible(WebColumnBase column, bool value) { return this.webColumnsOwnerImpl.SetColumnVisible(column, value); }
		int IWebColumnsOwner.SetColumnVisibleIndex(WebColumnBase column, int value) { return this.webColumnsOwnerImpl.SetColumnVisibleIndex(column, value); }
		void IWebColumnsOwner.OnColumnChanged(WebColumnBase column) { this.webColumnsOwnerImpl.OnColumnChanged(column); }
		void IWebColumnsOwner.OnColumnCollectionChanged() { this.webColumnsOwnerImpl.OnColumnCollectionChanged(); }
		#endregion
		#region IPopupFilterControlOwner Members
		void IPopupFilterControlOwner.CloseFilterControl() { this.HideFilterControl(); }
		object IPopupFilterControlOwner.GetControlCallbackResult() { return GetCallbackResult(); }
		string IPopupFilterControlOwner.MainElementID { get { return RenderHelper.MainTableID; } }
		ASPxWebControl IPopupFilterControlOwner.OwnerControl { get { return this; } }
		string IPopupFilterControlOwner.GetJavaScriptForApplyFilterControl() { return RenderHelper.Scripts.GetApplyFilterControl(); }
		string IPopupFilterControlOwner.GetJavaScriptForCloseFilterControl() { return RenderHelper.Scripts.GetCloseFilterControl(); }
		FilterControlImages IPopupFilterControlOwner.GetImages() { return ImagesFilterControl; }
		EditorImages IPopupFilterControlOwner.GetImagesEditors() { return ImagesEditors; }
		FilterControlStyles IPopupFilterControlOwner.GetStyles() { return StylesFilterControl; }
		EditorStyles IPopupFilterControlOwner.GetStylesEditors() { return StylesEditors; }
		string IPopupFilterControlOwner.FilterPopupHeaderText { get { return SettingsText.FilterControlPopupCaption; } }
		#endregion
		#region IFilterControlOwner Members
		int IFilterControlOwner.ColumnCount {
			get { return this.webColumnsOwnerImpl.FilterControlColumns.Count; }
		}
		IFilterColumn IFilterControlOwner.GetFilterColumn(int index) {
			return this.webColumnsOwnerImpl.FilterControlColumns[index];
		}
		bool IFilterControlOwner.GetValueDisplayText(IFilterColumn column, object value, out string displayText) {
			displayText = RenderHelper.TextBuilder.GetFilterControlItemText(column as GridViewDataColumn, value);
			return true;
		}		
		#endregion
		#region IFilterControlRowOwner Members
		string IFilterControlRowOwner.GetJavaScriptForClearFilter() { return RenderHelper.Scripts.GetClearFilterFunction(); }
		string IFilterControlRowOwner.GetJavaScriptForShowFilterControl() { return RenderHelper.Scripts.GetShowFilterControl(); }
		string IFilterControlRowOwner.GetJavaScriptForSetFilterEnabledForCheckbox() { return RenderHelper.Scripts.GetSetFilterEnabledForCheckBox(); }
		bool IFilterControlRowOwner.IsFilterEnabledSupported { get { return true; } }
		bool IFilterControlRowOwner.IsFilterEnabled { get { return FilterEnabled; } }
		void IFilterControlRowOwner.AppendDefaultDXClassName(WebControl control) {
			RenderHelper.AppendDefaultDXClassName(control);
		}
		void IFilterControlRowOwner.AssignFilterStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignLinkStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarLinkStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignCheckBoxCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarCheckBoxCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignImageCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarImageCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignExpressionCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarExpressionCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignClearButtonCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarClearButtonCellStyle().AssignToControl(control, true);
		}
		ImageProperties IFilterControlRowOwner.CreateFilterImage {
			get { return RenderHelper.GetImage(GridViewImages.FilterRowButtonName); }
		}
		string IFilterControlRowOwner.ClearButtonText { get { return SettingsText.FilterBarClear; } }
		string IFilterControlRowOwner.ShowFilterBuilderText { get { return SettingsText.FilterBarCreateFilter; } }
		void IFilterControlRowOwner.RaiseCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			CustomFilterExpressionDisplayTextEventHandler handler = Events[customFilterExpressionDisplayText] as CustomFilterExpressionDisplayTextEventHandler;
			if(handler == null) return;
			handler(this, e);
		}
		#endregion
		#region IPopupFilterControlStyleOwner Members
		ImageProperties IPopupFilterControlStyleOwner.CloseButtonImage { get { return Images.FilterBuilderClose; } }
		AppearanceStyle IPopupFilterControlStyleOwner.CloseButtonStyle { get { return Styles.FilterBuilderCloseButton; } }
		AppearanceStyle IPopupFilterControlStyleOwner.HeaderStyle { get { return Styles.FilterBuilderHeader; } }
		AppearanceStyle IPopupFilterControlStyleOwner.MainAreaStyle { get { return RenderHelper.GetFilterBuilderMainAreaStyle(); } }
		AppearanceStyle IPopupFilterControlStyleOwner.ButtonAreaStyle { get { return RenderHelper.GetFilterBuilderButtonAreaStyle(); } }
		#endregion
	}
	public class WebColumnsOwnerGridViewImplementation : WebColumnsOwnerDefaultImplementation {
		int fixedColumnCount = 0;
		List<GridViewDataColumn> filterControlColumns = null;
		public WebColumnsOwnerGridViewImplementation(IWebControlObject control, WebColumnCollectionBase columns) : base(control, columns) { }
		public int FixedColumnCount {
			get {
				EnsureVisibleColumns();
				return fixedColumnCount;
			}
		}
		public List<GridViewDataColumn> FilterControlColumns { 
			get {
				if (filterControlColumns == null) {
					BuildFilterControlColumns();
				}
				return filterControlColumns; 
			} 
		}
		public override void ResetVisibleColumns() {
			base.ResetVisibleColumns();
			this.filterControlColumns = null;			
		}
		public override void BuildVisibleColumns() {
			base.BuildVisibleColumns();
			CalcFixedColumnCount();
		}
		protected override int CompareColumnsByVisibleIndex(WebColumnBase col1, WebColumnBase col2) {
			GridViewColumn column1 = (GridViewColumn)col1;
			GridViewColumn column2 = (GridViewColumn)col2;
			if (column1.FixedStyle != column2.FixedStyle) {
				return column1.FixedStyle == GridViewColumnFixedStyle.Left ? -1 : 1;
			}
			return base.CompareColumnsByVisibleIndex(col1, col2);
		}
		protected void CalcFixedColumnCount() {
			this.fixedColumnCount = 0;
			for (int i = 0; i < GetVisibleColumns().Count; i++) {
				if (((GridViewColumn)GetVisibleColumns()[i]).FixedStyle == GridViewColumnFixedStyle.None) break;
				this.fixedColumnCount++;
			}
		}
		protected void BuildFilterControlColumns() {
			if (this.filterControlColumns == null) {
				this.filterControlColumns = new List<GridViewDataColumn>();
			}
			foreach(WebColumnBase column in Columns) {
				GridViewDataColumn dataColumn = column as GridViewDataColumn;
				if (dataColumn != null && dataColumn.ShowInFilterControl) {
					this.filterControlColumns.Add(dataColumn);
				}
			}
		}
	}
	public class GridViewDataHelper : DataHelper {
		DataSourceSelectArguments lastArguments = new DataSourceSelectArguments();
		public GridViewDataHelper(ASPxGridView grid, string name)
			: base(grid, name) {
		}
		internal DataSourceSelectArguments LastArguments { get { return lastArguments; } }
		protected ASPxGridView Grid { get { return (ASPxGridView)Control; } }
		protected override void OnDataSourceViewChanged() {
			if (Grid.SettingsDetail.IsDetailGrid) return;
			base.OnDataSourceViewChanged();
		}
		public override void PerformSelect() {
			if (Grid.DesignMode && !string.IsNullOrEmpty(Grid.DataSourceID)) return;
			Grid.RaiseBeforePerformDataSelect();
			ResetSelectArguments();
			this.lastArguments = SelectArguments;
			base.PerformSelect();
		}
		public int RetrieveTotalCount() {
			if (Grid.DesignMode) return 0;
			DataSourceSelectArguments select = new DataSourceSelectArguments();
			select.RetrieveTotalRowCount = true;
			select.MaximumRows = 0;
			PerformSelectCore(select, null);
			return select.TotalRowCount;
		}
		string GetSortExpression() {
			string res = string.Empty;
			foreach (GridViewDataColumn dc in Grid.SortedColumns) {
				if (res.Length > 0) res += ",";
				string field = dc.FieldName;
				if (field.IndexOf(' ') > -1) field = "[" + field + "]";
				res += string.Format("{0} {1}", field, dc.SortOrder == ColumnSortOrder.Ascending ? "ASC" : "DESC");
			}
			return res;
		}
		protected override DataSourceSelectArguments CreateDataSourceSelectArguments() {
			if (!Grid.IsAllowDataSourcePaging())
				return base.CreateDataSourceSelectArguments();
			string sortExpression = GetSortExpression();
			int startIndex = 0;
			int count = -1;
			if (Grid.SettingsPager.PageSize > -1) {
				count = Grid.SettingsPager.PageSize;
				startIndex = Grid.PageIndex * Grid.SettingsPager.PageSize;
			}
			DataSourceSelectArguments args = new DataSourceSelectArguments(sortExpression, startIndex, count);
			args.RetrieveTotalRowCount = true;
			return args;
		}
	}
}
