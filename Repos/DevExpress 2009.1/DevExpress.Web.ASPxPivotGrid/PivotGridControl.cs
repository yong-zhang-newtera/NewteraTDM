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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxMenu;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections;
using System.Threading;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils;
using DevExpress.Web.ASPxPager;
using System.IO.Compression;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPivotGrid.Web;
using DevExpress.Web.ASPxPopupControl;
using System.Collections.Specialized;
using DevExpress.Data.PivotGrid;
using DevExpress.Web.ASPxClasses.Localization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.ASPxEditors.FilterControl;
using DevExpress.Web.ASPxEditors;
namespace DevExpress.Web.ASPxPivotGrid {
	[
	DevExpress.Utils.Design.DXClientDocumentationProvider("DevExpress.ASPxPivotGrid/DevExpressWebASPxPivotGridScripts.htm"),
	Designer(typeof(DevExpress.Web.ASPxPivotGrid.Design.PivotGridControlDesigner)),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class ASPxPivotGrid : ASPxDataWebControl, IASPxPivotGridDataOwnerExtended, IViewBagOwner,
		IXtraSerializable, IRequiresLoadPostDataControl, IDataSource, IPopupFilterControlOwner,
		IPopupFilterControlStyleOwner {
		private string[] ChildControlNames = new string[] { "Web", "Editors" };
		static Hashtable olapColumnsCache;
		protected static Hashtable OlapColumnsCache {
			get {
				if(olapColumnsCache == null)
					olapColumnsCache = new Hashtable();
				return olapColumnsCache;
			}
		}
		PivotGridWebData data;
		bool enableCallBacks;
		bool isPrefilterPopupVisible;
		TableCell mainTD;
		HiddenField callBackStateField;
		Table tableContainer;
		PivotGridHtmlTable mainTable;
		PivotGridHtmlCustomizationFields customizationFields;
		PivotGridHtmlFilterPopup filterPopup;
		WebFilterControlPopup prefilterPopup;
		ASPxPivotGridPopupMenu headerMenu;
		ASPxPivotGridPopupMenu fieldValueMenu;
		Image arrowUpImage;
		Image arrowDownImage;
		Image dragHideFieldImage;
		Image groupSeparatorImage;
		PivotGridImages images;
		PivotGridPagerStyles pagerStyles;
		WebPrintAppearance printStyles;
		EditorStyles stylesEditors;
		FilterControlStyles stylesFilterControl;
		PivotKPIImages kpiImages;
		FilterControlImages imagesPrefilterControl;
		EditorImages imagesEditors;
		ASPxPivotGridRenderHelper renderHelper;
		PivotGridHtmlFilterPopupContent filterPopupContentControl;
		PivotGridPostBackActionBase postbackAction;
		bool isPivotDataCanDoRefresh = false;
		bool requireDataUpdate = false;
		protected bool RequireDataUpdate {
			get { return requireDataUpdate; }
			set {
				if(Data.IsDeserializing || requireDataUpdate == value) return;
				requireDataUpdate = value;
				if(value)
					ResetControlHierarchy();
			}
		}
		private static readonly object customUnboundFieldData = new object();
		private static readonly object customSummary = new object();
		private static readonly object customFieldSort = new object();
		private static readonly object customGroupInterval = new object();
		private static readonly object fieldAreaChanged = new object();
		private static readonly object fieldAreaIndexChanged = new object();
		private static readonly object fieldVisibleChanged = new object();
		private static readonly object fieldFilterChanged = new object();
		private static readonly object pageIndexChanged = new object();
		private static readonly object fieldValueCollapsed = new object();
		private static readonly object fieldValueExpanded = new object();
		private static readonly object fieldValueNotExpanded = new object();
		private static readonly object fieldValueCollapsing = new object();
		private static readonly object fieldValueExpanding = new object();
		private static readonly object fieldValueDisplayText = new object();
		private static readonly object controlHierarchyCreated = new object();
		private static readonly object customCellDisplayText = new object();
		private static readonly object customCellStyle = new object();
		private static readonly object addPopupMenuItem = new object();
		private static readonly object popupMenuCreated = new object();
		private static readonly object dataAreaPopupCreated = new object();
		private static readonly object olapQueryTimeout = new object();
		private static readonly object customCallback = new object();
		private static readonly object afterPerformCallback = new object();
		private static readonly object layoutChanged = new object();
		private static readonly object beforePerformDataSelect = new object();
		private static readonly object customFilterExpressionDisplayText = new object();		
		protected virtual bool IsPostBack {
			get {
				return Page != null && Page.IsPostBack;
			}
		}
		internal new bool IsTrackingViewState { get { return base.IsTrackingViewState; } }
		public ASPxPivotGrid() {
			this.EnableCallBacksInternal = true;
			this.enableCallBacks = true;
			this.customizationFields = new PivotGridHtmlCustomizationFields(this);
			this.images = new PivotGridImages(this);
			this.imagesPrefilterControl = new FilterControlImages(this);
			this.imagesEditors = new EditorImages(this);
			this.pagerStyles = new PivotGridPagerStyles(this);
			this.printStyles = new WebPrintAppearance();
			this.stylesEditors = new EditorStyles(this);
			this.stylesFilterControl = new FilterControlStyles(this);			
			OptionsChartDataSource.Changed += OnOptionsChartDataSourceChanged;
		}
		public override void Dispose() {
			OptionsChartDataSource.Changed -= OnOptionsChartDataSourceChanged;
			data.DisconnectOLAP();
			base.Dispose();
		}
		static ASPxPivotGrid() {
			SetLocalizer();
		}
		static void SetLocalizer() {
			DefaultActiveLocalizerProvider<PivotGridStringId> defaultProvider = PivotGridLocalizer.GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<PivotGridStringId>;
			if(defaultProvider == null || defaultProvider.GetActiveLocalizer().GetType() == typeof(PivotGridResLocalizer)) {
				ASPxActiveLocalizerProvider<PivotGridStringId> provider = new ASPxActiveLocalizerProvider<PivotGridStringId>(CreateResLocalizerInstance);
				PivotGridLocalizer.SetActiveLocalizerProvider(provider);
			}
		}
		static XtraLocalizer<PivotGridStringId> CreateResLocalizerInstance() {
			return new ASPxPivotGridResLocalizer();
		}
		[Description("Provides access to a pivot grid's field collection."),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), DefaultValue(null), AutoFormatDisable,
		Editor("DevExpress.Web.ASPxPivotGrid.Design.FieldsEditor, " + AssemblyInfo.SRAssemblyASPxPivotGrid, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue)]
		public PivotGridFieldCollection Fields { get { return Data != null ? Data.Fields : null; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void XtraClearFields(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				Fields.ClearAndDispose();
				return;
			}
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = XtraFindFieldsItem(new XtraItemEventArgs(this, Fields, xp));
				if(col != null) list.Add(col);
			}
			Data.BeginUpdate();
			for(int n = Fields.Count - 1; n >= 0; n--) {
				PivotGridField field = Fields[n];
				if(!list.Contains(field)) {
					field.Dispose();
				}
			}
			Data.EndUpdate();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateFieldsItem(XtraItemEventArgs e) {
			return Fields.Add();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraFindFieldsItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["ID"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(name == null || name == string.Empty) return null;
			PivotGridField field = Fields[name];
			return field;
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex) {
			EnsureRefreshData();
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, dataIndex, false);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, false);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex,
			List<string> customColumns) {			
			return CreateOLAPDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount,
			List<string> customColumns) {
			EnsureRefreshData();
			return VisualItems.CreateOLAPDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.CreateSummaryDataSource(columnIndex, rowIndex, false);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			EnsureRefreshData();
			return Data.CreateSummaryDataSource();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridWebData Data { 
			get {
				if(data == null) 
					data = CreateData();
				return data; 
			} 
		}
		protected PivotWebVisualItems VisualItems {
			get { return Data.VisualItems; }
		}
		protected PivotGridPostBackActionBase PostBackAction {
			get { return postbackAction; }
			set {
				if(postbackAction == value) return;
				postbackAction = value;
				if(postbackAction != null && postbackAction.RequireDataUpdate) {
					RequireDataUpdate = true;
					UpdateDataBoundAndChildControls(false);
				}
			}
		}
		protected internal ASPxPivotGridRenderHelper RenderHelper {
			get {
				if(renderHelper == null)
					renderHelper = CreateRenderHelper();
				return renderHelper;
			}
		}
		[Description("Gets a value indicating whether the grid is data bound."), Category("Data"), Browsable(false)]
		public bool IsDataBound { get { return Data.IsDataBound; } }
		protected internal PivotGridHtmlTable MainTable { get { return mainTable; } }
		protected WebControl MainTD { get { return mainTD; } }
		protected HiddenField CallBackStateField { get { return callBackStateField; } }
		internal Table TableContainer { get { return tableContainer; } }
		internal WebControl CallbackContainer { get { return MainTD; } }
		protected PivotGridHtmlCustomizationFields CustomizationFields { get { return customizationFields; } }
		protected internal PivotGridHtmlFilterPopupContent FilterPopupContentControl { get { return filterPopupContentControl; } }
		protected Image ArrowUpImage { get { return arrowUpImage; } }
		protected Image ArrowDownImage { get { return arrowDownImage; } }
		protected Image DragHideFieldImage { get { return dragHideFieldImage; } }
		protected Image GroupSeparatorImage { get { return groupSeparatorImage; } }
		protected WebFilterControlPopup PrefilterPopup { get { return prefilterPopup; } }
		[Description("Specifies a connection string to a cube in an MS Analysis Services database."),
		DefaultValue(""), Themeable(false), Category("Data"), Browsable(true), AutoFormatDisable(), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxPivotGrid.Design.OLAPConnectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string OLAPConnectionString {
			get { return GetStringProperty("OLAPConnectionString", ""); }
			set {
				SetStringProperty("OLAPConnectionString", "", value);
				OnDataPropertyChanged();
			}
		}
		protected bool IsOLAP { get { return !String.IsNullOrEmpty(OLAPConnectionString); } }
		[
		Description("Gets or sets whether the Filter Editor (Prefilter) is visible."),
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable,
		XtraSerializableProperty
		]
		public bool IsPrefilterPopupVisible {
			get { return isPrefilterPopupVisible; }
			set {
				if(IsPrefilterPopupVisible == value) return;
				isPrefilterPopupVisible = value;
				ResetControlHierarchy();
			}
		}
		[Description("Enables support for Section 508."),
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[Description("Gets or sets the text to render in an HTML caption element in an ASPxPivotGrid."),
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[Description("Gets or sets a value that describes the ASPxPivotGrid's contents."),
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable,
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SummaryText {
			get { return GetStringProperty("SummaryText", ""); }
			set {
				if(value == SummaryText) return;
				if(value != null)
					value = value.Replace('\n', ' ').Replace("\r", "");
				SetStringProperty("SummaryText", "", value);
			}
		}
		[Description("Gets or sets the pivot grid's client programmatic identifier."), AutoFormatDisable,
		Category("Client-Side"), DefaultValue(""), Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[Description("Gets or sets a value that specifies whether the grid view can be manipulated on the client side via code."), AutoFormatDisable,
		DefaultValue(false), Category("Client-Side"),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The client-side API is always available for this control.")]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[Description("Gets or sets a value that specifies whether the callback or postback technology is used to manage round trips to the server."),
		DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableCallBacks {
			get { return enableCallBacks; }
			set { enableCallBacks = value; }
		}
		[Description("Gets or sets whether callback result compression is enabled."),
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[Description("Gets an object that lists the client-side events specific to the ASPxPivotGrid."),
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotGridClientSideEvents ClientSideEvents {
			get { return (PivotGridClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PivotGridClientSideEvents();
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new ASPxPivotGridDataHelper(this, name);
		}
		protected virtual ASPxPivotGridRenderHelper CreateRenderHelper() {
			return new ASPxPivotGridRenderHelper(this);
		}
		protected virtual PivotGridWebData CreateData() {
			return new PivotGridWebData(this);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainTable = null;
			this.mainTD = null;
			this.filterPopup = null;
			this.prefilterPopup = null;
			this.headerMenu = null;
			this.fieldValueMenu = null;
			this.arrowUpImage = null;
			this.arrowDownImage = null;
			this.dragHideFieldImage = null;
			this.callBackStateField = null;
			ClearLoadingPanel();
			ClearLoadingDiv();
		}
		protected override void CreateControlHierarchy() {
			RefreshData();
			if(IsFilterPopupCallBack) {
				base.CreateControlHierarchy();
				return;
			}
			this.mainTable = new PivotGridHtmlTable(this);
			MainTable.ID = PivotGridWebData.ElementName_MainTable;
			this.mainTD = RenderUtils.CreateTableCell();
			MainTD.ID = PivotGridWebData.ElementName_MainTD;
			MainTD.Controls.Add(MainTable);
			this.callBackStateField = RenderUtils.CreateHiddenField(GetCallbackStateName());
			MainTD.Controls.Add(CallBackStateField);
			this.tableContainer = RenderUtils.CreateTable();
			TableContainer.Rows.Add(RenderUtils.CreateTableRow());
			TableContainer.Rows[0].Cells.Add(this.mainTD);
			Controls.Add(tableContainer);
			base.CreateControlHierarchy();
			this.filterPopup = new PivotGridHtmlFilterPopup(Data);
			Controls.Add(filterPopup);
			CreatePrefilterPopup();
			if(!DesignMode && OptionsView.ShowContextMenus && IsEnabled()) {
				this.headerMenu = CreateHeaderMenu();
				this.fieldValueMenu = CreateFieldValueMenu();
				MainTD.Controls.Add(this.headerMenu);
				MainTD.Controls.Add(this.fieldValueMenu);
			}
			if(!DesignMode) MainTD.Controls.Add(CustomizationFields);
			this.arrowUpImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragDownImage, PivotGridImages.DragArrowDownName);
			this.arrowDownImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragUpImage, PivotGridImages.DragArrowUpName);
			this.dragHideFieldImage = CreateHiddenImage(PivotGridImages.ElementName_DragHideFieldImage, PivotGridImages.DragHideFieldName);
			this.groupSeparatorImage = CreateHiddenImage(PivotGridImages.ElementName_GroupSeparatorImage, PivotGridImages.GroupSeparatorName);
			Controls.Add(ArrowUpImage);
			Controls.Add(ArrowDownImage);
			Controls.Add(DragHideFieldImage);
			Controls.Add(GroupSeparatorImage);
			CreateLoadingPanel();
			CreateLoadingDiv();
		}
		protected virtual void CreatePrefilterPopup() {
			if(!IsPrefilterPopupVisible || DesignMode) return;
			this.prefilterPopup = new WebFilterControlPopup(this);
			MainTD.Controls.Add(PrefilterPopup);
			PrefilterPopup.EnableViewState = false;
		}
		protected override void OnDataPropertyChanged() {
			RequireDataUpdate = true;
			base.OnDataPropertyChanged();			
		}
		bool isDataBinding;
		protected void DataBindIfNecessary() {
			if(!Data.IsDataBound && !isDataBinding) {
				isDataBinding = true;
				DataBind();
				isDataBinding = false;
			}
		}
		protected void EnsureRefreshData() {
			DataBindIfNecessary();
			RefreshData();
		}
		protected bool IsDataSourceEmpty {
			get {
				return DataSource == null && string.IsNullOrEmpty(DataSourceID)
					  && string.IsNullOrEmpty(OLAPConnectionString);
			}
		}
		protected internal void BindFakeDataIfNecessary() {
			if(!Data.IsDataBound && IsDataSourceEmpty) {
				Data.ListSource = new ArrayList();
			}
		}
		protected override void OnPreRender(EventArgs e) {
			if(!IsPostBack && !IsCallback) {
				BindFakeDataIfNecessary();
				EnsureRefreshData();
			} else {
				if(!IsOurCallbackPostback) {
					if(!IsDataSourceEmpty) {
						if(RequireDataUpdate)
							EnsureRefreshData();
						HasContentInternal = true;
					} else {
						BindFakeDataIfNecessary();
						ResetControlHierarchy();
					}
				}
			}
			base.OnPreRender(e);
		}
		protected override void EnsureChildControlsRecursive(Control control, bool skipContentContainers) {
			base.EnsureChildControlsRecursive(control, skipContentContainers);
			if(control == this && mainTD != null && IsEnabled())
				mainTD.Controls.Add(RenderUtils.CreateLiteralControl(GetStartupScript()));
			RaiseControlHierarchyCreated(new EventArgs());
		}
		protected override void PrepareControlHierarchy() {
			if(HasContentInternal) {
				SaveState();
				PrepareTableContainer();
				PrepareLoadingPanel();
				PrepareLoadingDiv();
				if(headerMenu != null) {
					headerMenu.ItemStyle.Assign(Styles.GetMenuItemStyle());
					headerMenu.ControlStyle.CopyFrom(Styles.MenuStyle);
					(headerMenu as IWebControlObject).LayoutChanged();
				}
				if(fieldValueMenu != null) {
					fieldValueMenu.ItemStyle.Assign(Styles.GetMenuItemStyle());
					fieldValueMenu.ControlStyle.CopyFrom(Styles.MenuStyle);
					(fieldValueMenu as IWebControlObject).LayoutChanged();
				}
				RenderHelper.GetDragArrowUpImage().AssignToControl(ArrowDownImage, DesignMode);
				RenderHelper.GetDragArrowDownImage().AssignToControl(ArrowUpImage, DesignMode);
				RenderHelper.GetDragHideFieldImage().AssignToControl(DragHideFieldImage, DesignMode);
				RenderHelper.GetGroupSeparatorImage().AssignToControl(GroupSeparatorImage, DesignMode);
			}
			base.PrepareControlHierarchy();
		}
		protected virtual void PrepareTableContainer() {
			RenderUtils.AssignAttributes(this, TableContainer);
			TableContainer.CellPadding = 0;
			TableContainer.CellSpacing = 0;
			TableContainer.BorderWidth = 0;
			InternalControlStyle.AssignToControl(TableContainer);
			Styles.ApplyContainerCellStyle(MainTD);
		}
		protected override void PrepareLoadingDiv(WebControl loadingDiv) {
			base.PrepareLoadingDiv(loadingDiv);
			if(LoadingDiv != null && IsEnabled()) {
				RenderUtils.SetAttribute(LoadingDiv, "onmousemove", "pivotGrid_ClearSelection();", "");
			}
		}
		protected override void ResetControlHierarchy() {
			RenderHelper.ResetMenus();
			base.ResetControlHierarchy();
		}
		protected virtual void SetCallbackStateFieldValue(string value) {
			if(CallBackStateField == null) return;
			CallBackStateField.Value = value;
		}
		protected virtual string GetCallbackStateFieldValue() {
			return (Request != null) ? Request.Params[UniqueID + "$" + GetCallbackStateName()] : null;
		}
		StringBuilder stateStringBuilder = new StringBuilder();
		Hashtable stateValues = new Hashtable();
		protected internal string SaveCallbackState(bool saveDifferenceOnly, bool saveCachedState) {
			ClearCallbackState();
			IXtraPropertyCollection toSerialize = GetPropertiesToSerialize(saveDifferenceOnly);
			SetStateValue(GetFieldsStateName(), SerializeSnapshot(toSerialize));
			SetStateValue(GetFilterValuesName(), Data.GetFilterValuesState());
			SetStateValue(GetFieldValueStateName(), Data.GetWebFieldValuesState());
			if(saveCachedState) {
				SetStateValue(GetFieldValueItemsStateName(), VisualItems.SavedFieldValueItemsState());
				SetStateValue(GetDataCellsStateName(), VisualItems.SavedDataCellsState());
			}
			if(stateStringBuilder[stateStringBuilder.Length - 1] == '|')
				stateStringBuilder.Remove(stateStringBuilder.Length - 2, 1);
			return stateStringBuilder.ToString();
		}
		protected IXtraPropertyCollection GetPropertiesToSerialize(bool saveDifferenceOnly) {
			if(!saveDifferenceOnly || FirstSnapshot == null)
				return GetSnapshot();
			return SerializationDiffCalculator.CalculateDiffCore(FirstSnapshot, GetSnapshot());
		}
		protected void ClearCallbackState() {
			stateStringBuilder.Length = 0;
		}
		protected void SetStateValue(string name, string value) {
			stateStringBuilder.Append(name).Append("_CBDEL_")
				.Append(value.Replace(PivotGridWebData.ArgumentsSeparator.ToString(), "_DEL_")).Append("_CBDEL_");
		}
		protected string GetStateValue(string name) {
			if(stateValues.Contains(name))
				return ((string)stateValues[name]).Replace("_DEL_", PivotGridWebData.ArgumentsSeparator.ToString());
			return String.Empty;
		}
		void SetCallbackState(string callback) {
			stateValues.Clear();
			if(string.IsNullOrEmpty(callback)) return;
			string[] values = callback.Split(new string[] { "_CBDEL_" }, StringSplitOptions.None);
			for(int i = 0; i < values.Length; i += 2) {
				if(i == values.Length - 1) break;
				stateValues.Add(values[i], values[i + 1]);
			}
		}
		protected void SaveState() {
			SetCallbackStateFieldValue(SaveCallbackState(true, true));
			ClearCallbackState();
		}
		protected internal void LoadCallbackState(string state, bool resetProperties) {
			if(string.IsNullOrEmpty(state)) 
				return;
			SetCallbackState(state);
			LoadLayoutFromCallbackState(resetProperties);
		}
		void LoadLayoutFromCallbackState(bool resetProperties) {
			DeserializeSnapshot(GetStateValue(GetFieldsStateName()), resetProperties);
			Data.SetFilterValueState(GetStateValue(GetFilterValuesName()));
			Data.WebFieldValuesStateCache = GetStateValue(GetFieldValueStateName());
			Data.LoadVisualItemsState(GetStateValue(GetFieldValueItemsStateName()), GetStateValue(GetDataCellsStateName()));
		}
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		protected override bool CanLoadPostDataOnCreateControls() {
			return true;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			LoadCallbackState(GetCallbackStateFieldValue(), false);
			return false;
		}
		internal string SerializeSnapshot(IXtraPropertyCollection snapshot) {
			return new Base64XtraSerializer().Serialize(snapshot);
		}
		protected void DeserializeSnapshot(string base64Snapshot, bool resetProperties) {
			new Base64XtraSerializer().Deserialize(this, base64Snapshot, resetProperties);
		}
		internal void DeserializeSnapshot(string base64Snapshot) {
			DeserializeSnapshot(base64Snapshot, true);
		}
		internal new StateBag GetViewState() {
			return ViewState;
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			MakeFirstSnapshot();
		}
		IXtraPropertyCollection firstSnapshot;
		protected IXtraPropertyCollection FirstSnapshot { get { return firstSnapshot; } }
		protected internal void MakeFirstSnapshot() {
			firstSnapshot = GetSnapshot();
		}
		internal protected IXtraPropertyCollection GetSnapshot() {
			return new SnapshotSerializeHelper().SerializeObject(this, OptionsLayoutBase.FullLayout);
		}
		public string SaveLayout() {
			return SaveCallbackState(false, false);
		}
		public void LoadLayout(string layoutState) {
			LoadCallbackState(layoutState, true);
		}
		protected ASPxPivotGridPopupMenu HeaderMenu {
			get { return headerMenu; }
		}
		protected string GetFilterValuesName() {
			return "FTR";
		}
		protected string GetFieldValueStateName() {
			return "FVS";
		}
		protected string GetFieldsStateName() {
			return "FLDS";
		}
		protected string GetFieldValueItemsStateName() {
			return "FITEMS";
		}
		protected string GetDataCellsStateName() {
			return "DSS";
		}
		protected string GetCallbackStateName() {
			return "CallbackState";
		}
		protected internal virtual PivotGridDataAreaPopup CreateDataAreaPopup() {
			PivotGridDataAreaPopup dataAreaPopup = new PivotGridDataAreaPopup(Data.PivotGrid);
			RaiseDataAreaPopupCreated(dataAreaPopup);
			return dataAreaPopup;
		}
		protected virtual ASPxPivotGridPopupMenu CreateHeaderMenu() {
			ASPxPivotGridPopupMenu menu = new ASPxPivotGridPopupMenu(this);
			menu.ID = PivotGridWebData.ElementName_HeaderMenu;
			menu.ClientSideEvents.ItemClick = "pivotGrid_OnHeaderMenuClick";
			menu.EnableViewState = false;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderRefresh))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuRefreshData), "Refresh");
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderHide))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuHideField), "Hide").BeginGroup = true;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderShowList)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuHideFieldList), "HideList");
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuShowFieldList), "ShowList");
			}
			if(OptionsCustomization.AllowPrefilter && RaiseAddPopupMenuItem(MenuItemEnum.HeaderShowPrefilter)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuShowPrefilter), "ShowPrefilter");
			}
			RaisePopupMenuCreated(PivotGridPopupMenuType.HeaderMenu, menu);
			return menu;
		}
		protected virtual ASPxPivotGridPopupMenu CreateFieldValueMenu() {
			ASPxPivotGridPopupMenu menu = new ASPxPivotGridPopupMenu(this);
			menu.EnableViewState = false;
			menu.ID = PivotGridWebData.ElementName_FieldValueMenu;
			menu.ClientSideEvents.ItemClick = "pivotGrid_OnFieldValueMenuClick";
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueExpand)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuExpand), "Expand");
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuCollapse), "Collapse");
			}
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueExpandAll)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuExpandAll), "ExpandAll").BeginGroup = true;
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuCollapseAll), "CollapseAll");
			}
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueSortBySummaryFields)) {
				CreateCrossAreaMenuItems(menu, PivotArea.ColumnArea);
				CreateCrossAreaMenuItems(menu, PivotArea.RowArea);
			}
			RaisePopupMenuCreated(PivotGridPopupMenuType.FieldValueMenu, menu);
			return menu;
		}
		protected internal virtual void CreateCrossAreaMenuItems(ASPxPivotGridPopupMenu menu, PivotArea area) {
			if(area != PivotArea.ColumnArea && area != PivotArea.RowArea)
				throw new ArgumentException("area");
			if(Data.DataFieldCount == 0) 
				return;
			PivotArea crossArea = area == PivotArea.ColumnArea ? PivotArea.RowArea : PivotArea.ColumnArea;
			List<PivotGridFieldBase> crossAreaFields = Data.GetFieldsByArea(crossArea, false),
									 dataFields = Data.GetFieldsByArea(PivotArea.DataArea, false);
			for(int i = 0; i < crossAreaFields.Count; i++) {
				PivotGridFieldBase field = crossAreaFields[i];
				if(!field.CanSortBySummary) continue;
				CreateSortByWithDataMenuItems(area, menu, field, dataFields, i == 0);
			}
			menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuRemoveAllSortByColumn), "SortBy_" + area.ToString() + "_RemoveAll").BeginGroup = true;
		}		
		protected void CreateSortByWithDataMenuItems(PivotArea area, ASPxPivotGridPopupMenu menu, PivotGridFieldBase field, List<PivotGridFieldBase> dataFields, bool beginGroup) {			
			for(int i = 0; i < dataFields.Count; i++) {
				CreateSortByMenuItem(area, menu, field, dataFields[i], beginGroup && i == 0);
			}
		}
		protected void CreateSortByMenuItem(PivotArea area, ASPxPivotGridPopupMenu menu, PivotGridFieldBase field,
									PivotGridFieldBase dataField, bool beginGroup) {
			bool showDataCaption = Data.DataField.Area != area && Data.DataFieldCount > 1;
			string caption = GetCaption(field, dataField, area, showDataCaption),
				menuId = "SortBy_" + area.ToString() + "_" + field.Index.ToString();
			if(dataField != null)
				menuId += "_" + dataField.Index.ToString();
			DevExpress.Web.ASPxMenu.MenuItem menuItem = menu.Items.Add(caption, menuId);
			menuItem.GroupName = menuId;
			menuItem.BeginGroup = beginGroup;
		}
		protected internal string GetCaption(PivotGridFieldBase field, PivotGridFieldBase dataField, PivotArea area, bool showDataFieldCaption) {
			PivotGridStringId stringId = area == PivotArea.ColumnArea ? PivotGridStringId.PopupMenuSortFieldByColumn : PivotGridStringId.PopupMenuSortFieldByRow;
			string captionTemplate = GetLocalizedString(stringId),
				fieldCaption = field.HeaderDisplayText;
			if(showDataFieldCaption)
				fieldCaption += " - " + dataField.HeaderDisplayText;
			return string.Format(captionTemplate, fieldCaption);
		}
		protected string GetLocalizedString(PivotGridStringId id) {
			return PivotGridLocalizer.GetString(id);
		}
		protected virtual Image CreateHiddenImage(string id, string src) {
			Image image = RenderUtils.CreateImage();
			image.ID = id;
			image.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			image.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
			image.ImageUrl = src;
			return image;
		}
		protected override bool HasLoadingPanel() {
			return true;
		}
		protected override bool HasLoadingDiv() {
			return IsCallBacksEnabled();
		}
		int fLockRefreshDataCount = 0;
		void LockRefreshData() {
			fLockRefreshDataCount++;
		}
		void UnlockRefreshData() {
			fLockRefreshDataCount--;
		}
		bool IsLockRefreshData { get { return fLockRefreshDataCount != 0; } }
		protected void RefreshData() {
			if((!this.RequireDataUpdate && !DesignMode) || IsLockRefreshData) return;
			LockRefreshData();
			this.RequireDataUpdate = false;
			if(PostBackAction != null) {
				PostBackAction.ApplyBefore();
			}
			RefreshDataCore();
			if(!string.IsNullOrEmpty(Data.WebFieldValuesStateCache) && (Data.ListSource != null || !string.IsNullOrEmpty(Data.OLAPConnectionString))) {
				Data.SetWebFieldValuesState(Data.WebFieldValuesStateCache);
				Data.WebFieldValuesStateCache = null;
			}
			if(PostBackAction != null) {
				PostBackAction.ApplyAfter();
				if(PostBackAction.ApplyOnlyOnce)
					PostBackAction = null;
			}
			if(CustomizationFields != null) {
				CustomizationFields.Refresh();
			}
			CorrectPageIndex();
			InvalidateChartData();
			UnlockRefreshData();
			if((IsPostBack || IsCallback) && PostBackAction != null)
				RaiseAfterPerformCallback();
		}
		void CorrectPageIndex() {
			if(!OptionsPager.HasPager || (Data.ListSource == null && string.IsNullOrEmpty(Data.OLAPConnectionString))) return;
			int unpagedRowCount = VisualItems.UnpagedRowCount;
			if(OptionsPager.PageIndex * OptionsPager.RowsPerPage >= unpagedRowCount) {				
				OptionsPager.PageIndex = unpagedRowCount % OptionsPager.RowsPerPage == 0 ? 
					unpagedRowCount / OptionsPager.RowsPerPage - 1 : 
					unpagedRowCount / OptionsPager.RowsPerPage;
			}
		}
		protected internal virtual void RefreshDataCore() {
			this.isPivotDataCanDoRefresh = true;
			try {
				Data.DoRefresh();
			}
			finally {
				this.isPivotDataCanDoRefresh = false;
			}
		}
		bool hasContentInternal;
		protected bool HasContentInternal {
			get { return hasContentInternal; }
			set {
				if(hasContentInternal == value) return;
				hasContentInternal = value;
				ResetControlHierarchy();
			}
		}
		protected override bool HasContent() {
			return HasContentInternal || IsPrefilterPopupVisible;
		}
		protected override void PerformDataBinding(string dataHelperName, IEnumerable dataSource) {
			if(IsOLAP) {				
				Data.OLAPConnectionString = OLAPConnectionString;
				string cachedColumns = OlapColumnsCache[OLAPConnectionString] as string;
				if(string.IsNullOrEmpty(cachedColumns)) {
					lock(OlapColumnsCache.SyncRoot) {
						OlapColumnsCache[OLAPConnectionString] = Data.SaveOLAPDataSourceState();
					}
				} else {
					Data.RestoreOLAPDataSourceState(cachedColumns);
				}
				RequireDataUpdate = true;
			} else {
				IList list = dataSource as System.Collections.IList;
				if(list == null && dataSource != null) {
					list = new ArrayList();
					foreach(object o in dataSource)
						list.Add(o);
				}
				Data.ListSource = list;
			}
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxPivotGrid), PivotGridWebData.PivotGridScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPivotGrid";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return base.GetStateManagedObjects();
		}
		protected virtual void UpdateDataBoundAndChildControls(bool boundImmediate) {
			if(Page == null && IsLoading()) return;
			if(!Data.IsDataBound) {
				ResetControlHierarchy();
				if(boundImmediate)
					DataBind();
				else {
					PerformSelect();
					DataBindChildren();
					ResetControlHierarchy();
				}
			}
			else {
				RequireDataUpdate = true;
			}
		}
		bool IASPxPivotGridDataOwner.IsPivotDataCanDoRefresh { get { return isPivotDataCanDoRefresh; } }
		void IASPxPivotGridDataOwner.RequireUpdateData() {
			RequireDataUpdate = true;
		}
		void IASPxPivotGridDataOwner.EnsureRefreshData() {
			EnsureRefreshData();
		}
		void IASPxPivotGridDataOwner.ResetControlHierarchy() {
			ResetControlHierarchy();
		}
		void IASPxPivotGridDataOwner.ElementTemplatesChanged() {
			TemplatesChanged();
		}
		PivotGridFieldCollectionBase IASPxPivotGridDataOwner.Fields { get { return Fields; } }
		protected virtual PivotGridPostBackActionBase GetPostbackAction(string postbackId, string eventArgument) {
			switch(postbackId) {
				case PivotGridWebData.ExpandColumnChanged:
				case PivotGridWebData.ExpandRowChanged:
					return new PivotGridPostbackExpandedItemAction(Data, eventArgument);
				case PivotGridWebData.FilterChanged:
					return new PivotGridPostBackFilterFieldAction(Data, Fields, eventArgument);
				case PivotGridWebData.HeaderDrag:
					return new PivotGridPostBackDragFieldAction(Data, Fields, eventArgument);
				case PivotGridWebData.HeaderSort:
					return new PivotGridPostBackFieldSortAction(Data, Fields, eventArgument);
				case PivotGridWebData.HeaderSortBySummaryChanged:
					return new PivotGridPostBackSortByColumnAction(Data, Fields, eventArgument);
				case PivotGridWebData.HideField:
					return new PivotGridPostBackHideFieldAction(Data, Fields, eventArgument);
				case PivotGridWebData.GroupExpanded:
					return new PivotGridPostBackChangeGroupExpandedAction(Data, Fields, eventArgument);
				case PivotGridWebData.CustomCallback:
					return new PivotGridCustomPostbackAction(this, eventArgument.Substring(2));
				case PivotGridWebData.Pager:
					return new PivotGridPostBackPagerAction(Data, eventArgument);
				case PivotGridWebData.ShowPrefilter:
					return new PivotGridPostBackPrefilterAction(this, eventArgument);
				case PivotGridWebData.ReloadDataCallback:
					return null; 
				default:
					return null;
			}
		}
		bool isOurCallbackPostback;
		protected bool IsOurCallbackPostback {
			get { return isOurCallbackPostback; }
		}
		protected virtual void PerformCallbackOrPostBack(string eventArgument) {
			if(string.IsNullOrEmpty(eventArgument)) 
				return;
			this.isOurCallbackPostback = true;
			BindFakeDataIfNecessary();
			EnsureRefreshData();
			string postbackId = GetPostBackId(eventArgument);			
			if(postbackId == PivotGridWebData.FilterShowWindow) {
				PrepareFilterShowCallBack(eventArgument);
				return;
			}
			PostBackAction = GetPostbackAction(postbackId, eventArgument);			
		}
		protected string GetPostBackId(string eventArgument) {
			int separatorIndex = eventArgument.IndexOf(PivotGridWebData.ArgumentsSeparator);
			return separatorIndex >= 0 ? eventArgument.Substring(0, separatorIndex) : eventArgument;
		}
		bool isFilterPopupCallBack = false;
		void PrepareFilterShowCallBack(string eventArgument) {
			string[] parts = eventArgument.Split(PivotGridWebData.ArgumentsSeparator);
			int index = 0;
			isFilterPopupCallBack = true;
			if(!int.TryParse(parts[1], out index)) 
				return;
			if(IsOLAP) Data.DoRefresh();
			UpdateDataBoundAndChildControls(true);
			PivotGridField filterField = Fields[index];
			if(filterField != null) {
				filterPopupContentControl = new PivotGridHtmlFilterPopupContent(Data, filterField);
			}
		}
		bool IsFilterPopupCallBack { get { return filterPopupContentControl != null || isFilterPopupCallBack; } }
		protected override void RaisePostBackEvent(string eventArgument) {
			if(string.IsNullOrEmpty(eventArgument)) return;
			PerformCallbackOrPostBack(eventArgument);
		}
		protected override object GetCallbackResult() {
			if(filterPopupContentControl != null)
				return filterPopupContentControl.GetCallBackString();
			EnsureChildControls();
			SaveState();
			StringBuilder sb = new StringBuilder("G|");
			foreach(Control control in mainTD.Controls) {
				sb.Append(RenderUtils.GetRenderResult(control));
			}
			return sb.ToString();
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			int callbackStart = eventArgument.IndexOf("|CB|");
			if(callbackStart > -1) {
				EnsureChildControls();
				PerformCallbackOrPostBack(eventArgument.Substring(0, callbackStart));
			}
			else
				PerformCallbackOrPostBack(eventArgument);
		}			  
		new string GetStartupScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendLine(RenderHelper.GetHoverScript());
			stb.AppendLine(RenderHelper.GetContextMenuScript());
			stb.AppendLine(RenderHelper.GetAllowedAreaIdsScript());			
			stb.AppendLine(RenderHelper.GetGroupsScript());
			stb.AppendLine(RenderHelper.GetAfterCallBackInitializeScript());			
			return RenderUtils.GetScriptHtml(stb.ToString());
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName).Append(".callBacksEnabled = ").Append(EnableCallBacks ? "true" : "false").AppendLine(";");
		}		
		[Description("Gets or sets a value that specifies whether the pivot grid's Customization form is displayed within the page."),
		Category("Behavior"), DefaultValue(false), AutoFormatDisable()]
		public bool CustomizationFieldsVisible {
			get { return CustomizationFields.ShowOnPageLoad; }
			set {
				CustomizationFields.ShowOnPageLoad = value;
			}
		}
		[Description("Gets or sets the X-coordinate of the Customization form's top-left corner."),
		Category("Behavior"), DefaultValue(100), AutoFormatDisable()]
		public int CustomizationFieldsLeft {
			get { return CustomizationFields.Left; }
			set {
				CustomizationFields.Left = value;
			}
		}
		[Description("Gets or sets the Y-coordinate of the Customization form's top-left corner."),
		Category("Behavior"), DefaultValue(100), AutoFormatDisable()]
		public int CustomizationFieldsTop {
			get { return CustomizationFields.Top; }
			set {
				CustomizationFields.Top = value;
			}
		}
		void ResetPrefilter() { Prefilter.Reset(); }
		bool ShouldSerializePrefilter() { return Prefilter.ShouldSerialize(); }
		[
		Description("Gets the Prefilter's settings."),
		Category("Data"), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public WebPrefilter Prefilter {
			get { return Data.Prefilter; }
		}
		void ResetOptionsView() { OptionsView.Reset(); }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerialize(); }
		[Description("Provides access to the ASPxPivotGrid control's view options."),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable]
		public PivotGridWebOptionsView OptionsView { get { return Data.OptionsView; } }
		void ResetOptionsCustomization() { OptionsCustomization.Reset(); }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerialize(); }
		[Description("Provides access to the pivot grid's customization options."),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable]
		public PivotGridWebOptionsCustomization OptionsCustomization { get { return Data.OptionsCustomization as PivotGridWebOptionsCustomization; } }
		void ResetOptionsChartDataSource() { OptionsChartDataSource.Reset(); }
		bool ShouldSerializeOptionsChartDataSource() { return OptionsChartDataSource.ShouldSerialize(); }
		[Description("Provides access to the chart options."),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable]
		public PivotGridWebOptionsChartDataSource OptionsChartDataSource { get { return Data.OptionsChartDataSource; } }
		void ResetOptionsDataField() { OptionsDataField.Reset(); }
		bool ShouldSerializeOptionsDataField() { return OptionsDataField.ShouldSerialize(); }
		[Description("Provides access to options, which control the presentation of data fields in the ASPxPivotGrid."),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable()]
		public PivotGridWebOptionsDataField OptionsDataField { get { return Data.OptionsDataField as PivotGridWebOptionsDataField; } }
		[Description("Provides access to the pivot grid's pager options"),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true), AutoFormatEnable()]
		public PivotGridWebOptionsPager OptionsPager { get { return Data.OptionsPager; } }
		[Description("Provides access to the loading panel's settings."),
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Localizable(true), AutoFormatEnable()]
		public PivotGridWebOptionsLoadingPanel OptionsLoadingPanel { get { return Data.OptionsLoadingPanel; } }
		void ResetOptionsData() { OptionsData.Reset(); }
		bool ShouldSerializeOptionsData() { return OptionsData.ShouldSerialize(); }
		[
		Description("Provides access to the pivot grid's data specific options."),
		Category("Options"),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Localizable(true),
		AutoFormatEnable()
		]
		public PivotGridOptionsData OptionsData { get { return Data.OptionsData; } }
		protected override StylesBase CreateStyles() {
			return new PivotGridStyles(this);
		}
		protected override string GetDefaultCssFilePath() {
			return RenderUtils.GetWebResourceUrl(Page, typeof(ASPxPivotGrid), PivotGridWebData.PivotGridCssResourceName, true);
		}
		AppearanceStyle internalControlStyle;
		internal AppearanceStyle InternalControlStyle {
			get {
				if(internalControlStyle == null) {
					internalControlStyle = new AppearanceStyle();
					Styles.ApplyControlStyle(internalControlStyle);
					internalControlStyle.CopyFrom(ControlStyle);
				}
				return internalControlStyle;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableDefaultAppearance {
			get { return base.EnableDefaultAppearance; }
			set { base.EnableDefaultAppearance = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Description("Gets or sets the name of the cascading style sheet (CSS) class that specifies the ASpxPivotGrid's style."),
		DefaultValue(""), Localizable(false), AutoFormatEnable()]
		public override string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
		[Description("Provides access to the settings that define images displayed within the pivot grid's elements."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImages Images { get { return (PivotGridImages)ImagesInternal; } }
		protected override ImagesBase CreateImages() {
			return new PivotGridImages(this);
		}
		PivotKPIImages IASPxPivotGridDataOwnerExtended.KPIImages {
			get {
				if(kpiImages == null)
					kpiImages = new PivotKPIImages(this);
				return kpiImages;
			}
		}
		[Description("Provides access to the settings that define images displayed in the editors, used to edit filter values within the Prefilter Control (Filter Editor)."),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorImages ImagesEditors { get { return imagesEditors; } }
		[Description("Provides access to the settings that define images displayed within the Prefilter (Filter Editor)."),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImages ImagesPrefilterControl { get { return imagesPrefilterControl; } }
		#region Images (obsolete)
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.FieldValueCollapsed property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties FieldValueCollapsedImage { get { return Images.FieldValueCollapsed; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.FieldValueExpanded property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties FieldValueExpandedImage { get { return Images.FieldValueExpanded; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.HeaderFilter property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties HeaderFilterImage { get { return Images.HeaderFilter; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.HeaderActiveFilter property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties HeaderActiveFilterImage { get { return Images.HeaderActiveFilter; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.HeaderSortDown property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties HeaderSortDownImage { get { return Images.HeaderSortDown; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.HeaderSortUp property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties HeaderSortUpImage { get { return Images.HeaderSortUp; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.FilterWindowSizeGrip property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties FilterWindowSizeGripImage { get { return Images.FilterWindowSizeGrip; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[Obsolete("Use the Images.CustomizationFieldsClose property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties CustomizationFieldsCloseImage { get { return Images.CustomizationFieldsClose; } }
		#endregion	  
		#region Styles (obsolete)
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The MenuItemStyle property is obsolete. Use Styles.MenuItemStyle property instead.")]
		public DevExpress.Web.ASPxMenu.MenuItemStyle MenuItemStyle { get { return Styles.MenuItemStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The MenuStyle property is obsolete. Use Styles.MenuStyle property instead.")]
		public DevExpress.Web.ASPxMenu.MenuStyle MenuStyle { get { return Styles.MenuStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CustomizationFieldsStyle property is obsolete. Use Styles.CustomizationFieldsStyle property instead.")]
		public AppearanceStyle CustomizationFieldsStyle { get { return Styles.CustomizationFieldsStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CustomizationFieldsCloseButtonStyle property is obsolete. Use Styles.CustomizationFieldsCloseButtonStyle property instead.")]
		public PopupWindowButtonStyle CustomizationFieldsCloseButtonStyle { get { return Styles.CustomizationFieldsCloseButtonStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CustomizationFieldsContentStyle property is obsolete. Use Styles.CustomizationFieldsContentStyle property instead.")]
		public PopupWindowContentStyle CustomizationFieldsContentStyle { get { return Styles.CustomizationFieldsContentStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CustomizationFieldsHeaderStyle property is obsolete. Use Styles.CustomizationFieldsHeaderStyle property instead.")]
		public PopupWindowStyle CustomizationFieldsHeaderStyle { get { return Styles.CustomizationFieldsHeaderStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The HeaderStyle property is obsolete. Use Styles.HeaderStyle property instead.")]
		public PivotHeaderStyle HeaderStyle { get { return Styles.HeaderStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The AreaStyle property is obsolete. Use Styles.AreaStyle property instead.")]
		public PivotAreaStyle AreaStyle { get { return Styles.AreaStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterAreaStyle property is obsolete. Use Styles.FilterAreaStyle property instead.")]
		public PivotAreaStyle FilterAreaStyle { get { return Styles.FilterAreaStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FieldValueStyle property is obsolete. Use Styles.FieldValueStyle property instead.")]
		public PivotFieldValueStyle FieldValueStyle { get { return Styles.FieldValueStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FieldValueTotalStyle property is obsolete. Use Styles.FieldValueTotalStyle property instead.")]
		public PivotFieldValueStyle FieldValueTotalStyle { get { return Styles.FieldValueTotalStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FieldValueGrandTotalStyle property is obsolete. Use Styles.FieldValueGrandTotalStyle property instead.")]
		public PivotFieldValueStyle FieldValueGrandTotalStyle { get { return Styles.FieldValueGrandTotalStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CellStyle property is obsolete. Use Styles.CellStyle property instead.")]
		public PivotCellStyle CellStyle { get { return Styles.CellStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("TThe otalCellStyle property is obsolete. Use Styles.TotalCellStyle property instead.")]
		public PivotCellStyle TotalCellStyle { get { return Styles.TotalCellStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The GrandTotalCellStyle property is obsolete. Use Styles.GrandTotalCellStyle property instead.")]
		public PivotCellStyle GrandTotalCellStyle { get { return Styles.GrandTotalCellStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The CustomTotalCellStyle property is obsolete. Use Styles.CustomTotalCellStyle property instead.")]
		public PivotCellStyle CustomTotalCellStyle { get { return Styles.CustomTotalCellStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterWindowStyle property is obsolete. Use Styles.FilterWindowStyle property instead.")]
		public PivotFilterStyle FilterWindowStyle { get { return Styles.FilterWindowStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterItemsAreaStyle property is obsolete. Use Styles.FilterItemsAreaStyle property instead.")]
		public PivotFilterStyle FilterItemsAreaStyle { get { return Styles.FilterItemsAreaStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterItemStyle property is obsolete. Use Styles.FilterItemStyle property instead.")]
		public PivotFilterItemStyle FilterItemStyle { get { return Styles.FilterItemStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterButtonStyle property is obsolete. Use Styles.FilterButtonStyle property instead.")]
		public PivotFilterButtonStyle FilterButtonStyle { get { return Styles.FilterButtonStyle; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("The FilterButtonPanelStyle property is obsolete. Use Styles.FilterButtonPanelStyle property instead.")]
		public PivotFilterButtonPanelStyle FilterButtonPanelStyle { get { return Styles.FilterButtonPanelStyle; } }
		#endregion
		internal AppearanceStyle[] GetStyles() {
			return new AppearanceStyle[] { 
				LoadingPanelStyle
			};
		}
		[Description("Provides access to the style settings that control the appearance of the pivot grid's elements."),
		Category("Styles"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()]
		public PivotGridStyles Styles { get { return StylesInternal as PivotGridStyles; } }
		[Description("Provides access to the style settings that control the appearance of the pager displayed within the pivot grid."),
		Category("Styles"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()]
		public PivotGridPagerStyles StylesPager { get { return pagerStyles; } }
		[Description("Gets the style settings defining the pivot grid's print appearance."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatEnable()]
		public WebPrintAppearance StylesPrint { get { return printStyles; } }
		void ResetStylesPrint() { StylesPrint.Reset(); }
		bool ShouldSerializeStylesPrint() { return StylesPrint.ShouldSerialize(); }
		[Description(""),
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlStyles StylesFilterControl { get { return stylesFilterControl; } }
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridHeaderTemplateContainer)), AutoFormatDisable()]
		public virtual ITemplate HeaderTemplate {
			get { return Data.HeaderTemplate; }
			set {
				Data.HeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridEmptyAreaTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate EmptyAreaTemplate {
			get { return Data.EmptyAreaTemplate; }
			set {
				Data.EmptyAreaTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridFieldValueTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate FieldValueTemplate {
			get { return Data.FieldValueTemplate; }
			set {
				Data.FieldValueTemplate = value;
				TemplatesChanged();
			}
		}
		[Description("This member supports the .NET Framework infrastructure and cannot be used directly from your code."),
		Localizable(false), AutoFormatDisable()]
		public override string ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridCellTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate CellTemplate {
			get { return Data.CellTemplate; }
			set {
				Data.CellTemplate = value;
				TemplatesChanged();
			}
		}
		[
		Description("Enables you to supply any server data that can then be parsed on the client. "),
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[Category("Data")]
		public event CustomFieldDataEventHandler CustomUnboundFieldData {
			add { 
				this.Events.AddHandler(customUnboundFieldData, value);
				RequireDataUpdate = true;
			}
			remove { 
				this.Events.RemoveHandler(customUnboundFieldData, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomSummaryEventHandler CustomSummary {
			add { 
				this.Events.AddHandler(customSummary, value);
				RequireDataUpdate = true;
			}
			remove { 
				this.Events.RemoveHandler(customSummary, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomFieldSortEventHandler CustomFieldSort {
			add { 
				this.Events.AddHandler(customFieldSort, value);
				RequireDataUpdate = true;
			}
			remove { 
				this.Events.RemoveHandler(customFieldSort, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomGroupIntervalEventHandler CustomGroupInterval {
			add { 
				this.Events.AddHandler(customGroupInterval, value);
				RequireDataUpdate = true;
			}
			remove { 
				this.Events.RemoveHandler(customGroupInterval, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Layout")] 
		public event PivotFieldEventHandler FieldAreaChanged {
			add { this.Events.AddHandler(fieldAreaChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldAreaIndexChanged {
			add { this.Events.AddHandler(fieldAreaIndexChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaIndexChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldVisibleChanged {
			add { this.Events.AddHandler(fieldVisibleChanged, value); }
			remove { this.Events.RemoveHandler(fieldVisibleChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldFilterChanged {
			add { this.Events.AddHandler(fieldFilterChanged, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanged, value); }
		}
		[Category("Pager")]
		public event EventHandler PageIndexChanged {
			add { Events.AddHandler(pageIndexChanged, value); }
			remove { Events.RemoveHandler(pageIndexChanged, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedEventHandler FieldValueCollapsed {
			add { this.Events.AddHandler(fieldValueCollapsed, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsed, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedEventHandler FieldValueExpanded {
			add { this.Events.AddHandler(fieldValueExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanded, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedCancelEventHandler FieldValueCollapsing {
			add { this.Events.AddHandler(fieldValueCollapsing, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsing, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedCancelEventHandler FieldValueExpanding {
			add { this.Events.AddHandler(fieldValueExpanding, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanding, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedCancelEventHandler FieldValueNotExpanded {
			add { this.Events.AddHandler(fieldValueNotExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueNotExpanded, value); }
		}
		[Category("Behavior")]
		public event EventHandler ControlHierarchyCreated {
			add { this.Events.AddHandler(controlHierarchyCreated, value); }
			remove { this.Events.RemoveHandler(controlHierarchyCreated, value); }
		}
		[Category("Data")]
		public event PivotFieldDisplayTextEventHandler FieldValueDisplayText {
			add { this.Events.AddHandler(fieldValueDisplayText, value); }
			remove { this.Events.RemoveHandler(fieldValueDisplayText, value); }
		}
		[Category("Data")]
		public event PivotCellDisplayTextEventHandler CustomCellDisplayText {
			add { this.Events.AddHandler(customCellDisplayText, value); }
			remove { this.Events.RemoveHandler(customCellDisplayText, value); }
		}
		[Category("Style")]
		public event PivotCustomCellStyleEventHandler CustomCellStyle {
			add { this.Events.AddHandler(customCellStyle, value); }
			remove { this.Events.RemoveHandler(customCellStyle, value); }
		}
		[Category("Menu")]
		public event PivotAddPopupMenuItemEventHandler AddPopupMenuItem {
			add { this.Events.AddHandler(addPopupMenuItem, value); }
			remove { this.Events.RemoveHandler(addPopupMenuItem, value); }
		}
		[Category("Menu")]
		public event PivotPopupMenuCreatedEventHandler PopupMenuCreated {
			add { this.Events.AddHandler(popupMenuCreated, value); }
			remove { this.Events.RemoveHandler(popupMenuCreated, value); }
		}
		[Category("Behavior")]
		public event EventHandler<PivotDataAreaPopupCreatedEventArgs> DataAreaPopupCreated {
			add { this.Events.AddHandler(dataAreaPopupCreated, value); }
			remove { this.Events.RemoveHandler(dataAreaPopupCreated, value); }
		}
		[Category("Behavior")]
		public event EventHandler OLAPQueryTimeout {
			add { this.Events.AddHandler(olapQueryTimeout, value); }
			remove { this.Events.RemoveHandler(olapQueryTimeout, value); }
		}
		[Category("Behavior")]
		public event PivotCustomCallbackEventHandler CustomCallback {
			add { this.Events.AddHandler(customCallback, value); }
			remove { this.Events.RemoveHandler(customCallback, value); }
		}
		[Category("Behavior")]
		public event EventHandler AfterPerformCallback {
			add { this.Events.AddHandler(afterPerformCallback, value); }
			remove { this.Events.RemoveHandler(afterPerformCallback, value); }
		}
		[Category("Layout")]
		public event EventHandler GridLayout {
			add { this.Events.AddHandler(layoutChanged, value); }
			remove { this.Events.RemoveHandler(layoutChanged, value); }
		}
		[Category("Data")]
		public event EventHandler BeforePerformDataSelect {
			add { this.Events.AddHandler(beforePerformDataSelect, value); }
			remove { this.Events.RemoveHandler(beforePerformDataSelect, value); }
		}
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJsProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}		
		public event CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText {
			add { Events.AddHandler(customFilterExpressionDisplayText, value); }
			remove { Events.RemoveHandler(customFilterExpressionDisplayText, value); }
		}
		public void BeginUpdate() {
			Data.BeginUpdate();
		}
		public void EndUpdate() {
			Data.EndUpdate();
		}
		public void RetrieveFields() {
			Data.RetrieveFields();
		}
		public string[] GetFieldList() {
			return Data.GetFieldList();
		}
		public List<PivotGridField> GetFieldsByArea(PivotArea area) {
			List<PivotGridFieldBase> baseList = Data.GetFieldsByArea(area, false);
			List<PivotGridField> res = new List<PivotGridField>(baseList.Count);
			for(int i = 0; i < baseList.Count; i++)
				res.Add((PivotGridField)baseList[i]);
			return res;
		}
		public PivotGridField GetFieldByArea(PivotArea area, int areaIndex) {
			return (PivotGridField)Data.GetFieldByArea(area, areaIndex);
		}
		public List<string> GetOLAPKPIList() {
			if(!Data.IsDataBound) DataBind();
			return Data.GetOLAPKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			if(!Data.IsDataBound) DataBind();
			return Data.GetOLAPKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			if(!Data.IsDataBound) DataBind();
			return Data.GetOLAPKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			if(!Data.IsDataBound) DataBind();
			return Data.GetOLAPKPIServerGraphic(kpiName, kpiType);
		}
		public ImageProperties GetKPIImage(PivotKPIGraphic graphic, PivotKPIType kpiType, int value) {
			return Data.KPIImages.GetImage(graphic, kpiType, value);
		}
		public void ReloadData() {
			Data.ReloadData();
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			Data.RestoreFieldsInGroups();
		}
		object IASPxPivotGridDataOwner.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {			
			return RaiseCustomUnboundColumnData(field, listSourceRowIndex);			
		}
		void IASPxPivotGridDataOwner.CalcCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {			
			RaiseCustomSummary(field, customSummaryInfo);
		}		
		int IASPxPivotGridDataOwner.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {			
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);			
		}
		object IASPxPivotGridDataOwner.GetCustomGroupInterval(PivotGridFieldBase field, object value) {			
			return RaiseCustomGroupInterval(field, value);
		}		
		bool IASPxPivotGridDataOwner.OnFieldValueStateChanging(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			if(IsLoading()) return true;			
			if(isCollapsed)
				return RaiseFieldValueExpanding(field, values, isCollapsed);
			else 
				return RaiseFieldValueCollapsing(field, values, isCollapsed);			
		}
		void IASPxPivotGridDataOwner.OnFieldAreaChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldAreaChanged(field);
		}
		void IASPxPivotGridDataOwner.OnFieldAreaIndexChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldAreaIndexChanged(field);
		}
		void IASPxPivotGridDataOwner.OnFieldVisibleChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldVisibleChanged(field);
		}
		void IASPxPivotGridDataOwner.OnFieldFilterChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldFilterChanged(field);
		}
		void IASPxPivotGridDataOwner.OnFieldValueStateChanged(PivotGridFieldBase field, object[] values, bool isCollapsed, bool success) {
			if(IsLoading()) return; 
			if(isCollapsed) {
				if(success) RaiseFieldValueExpanded(field, values, isCollapsed);
				else RaiseFieldValueNotExpanded(field, values, isCollapsed);
			} else RaiseFieldValueCollapsed(field, values, isCollapsed);
		}
		void IASPxPivotGridDataOwner.OnDataSourceChanged() {
			HasContentInternal = Data.IsDataBound;
		}
		string IASPxPivotGridDataOwner.GetFieldValueDisplayText(PivotFieldValueItem item) {
			return RaiseFieldValueDisplayText(item);
		}
		string IASPxPivotGridDataOwner.GetFieldValueDisplayText(PivotGridFieldBase field, object value) {			
			return RaiseFieldValueDisplayText(field, value);
		}
		string IASPxPivotGridDataOwner.CustomCellDisplayText(PivotGridCellItem cellItem) {			
			return RaiseCellDisplayText(cellItem);			
		}
		void IASPxPivotGridDataOwnerExtended.CustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle) {			
			RaiseCustomCellStyle(cellItem, cellStyle);
		}
		void IASPxPivotGridDataOwnerExtended.PageIndexChanged() {
			if(!IsLoading()) RaisePageIndexChanged();
		}
		void IASPxPivotGridDataOwnerExtended.OLAPQueryTimeout() {
			if(!IsLoading()) RaiseOLAPQueryTimeout();
		}
		void IASPxPivotGridDataOwnerExtended.LayoutChanged() {
			if(!IsLoading()) RaiseLayoutChanged();
		}
		T IViewBagOwner.GetViewBagProperty<T>(string objectPath, string propertyName, T value) {
			return (T)GetObjectProperty(GetBagKeyName(objectPath, propertyName), value);
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value) {
			SetObjectProperty(GetBagKeyName(objectPath, propertyName), defaultValue, value);
		}
		Dictionary<string, Dictionary<string, string>> names = new Dictionary<string, Dictionary<string, string>>();
		string GetBagKeyName(string objectName, string propertyName) {
			Dictionary<string, string> list;
			if(!names.TryGetValue(objectName, out list)) {
				list = new Dictionary<string, string>();
				names.Add(objectName, list);
			}
			string name;
			if(!list.TryGetValue(propertyName, out name)) {
				name = objectName + "." + propertyName;
				list.Add(propertyName, name);
			}
			return name;
		}
		[Description("Gets the collection of field groups."),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 100),
		Editor("DevExpress.Web.ASPxPivotGrid.Design.GroupsEditor, " + AssemblyInfo.SRAssemblyASPxPivotGrid, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PivotGridWebGroupCollection Groups {
			get { return Data.Groups; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateGroupsItem(XtraItemEventArgs e) {
			return Data.XtraCreateGroupsItem(e);
		}
		protected virtual object RaiseCustomUnboundColumnData(PivotGridFieldBase field, int listSourceRowIndex) {
			CustomFieldDataEventHandler handler = (CustomFieldDataEventHandler)this.Events[customUnboundFieldData];
			if(handler != null) {
				CustomFieldDataEventArgs e = new CustomFieldDataEventArgs(Data, field as PivotGridField, listSourceRowIndex, null);
				handler(this, e);
				return e.Value;
			} else
				return null;
		}
		protected virtual void RaiseCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			PivotGridCustomSummaryEventHandler handler = (PivotGridCustomSummaryEventHandler)this.Events[customSummary];
			if(handler != null) {
				PivotGridCustomSummaryEventArgs e = new PivotGridCustomSummaryEventArgs(Data, field as PivotGridField, customSummaryInfo);
				handler(this, e);
			}
		}
		PivotGridCustomFieldSortEventArgs fieldSortEventArgs = null;
		protected virtual int RaiseCustomFieldSort(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			PivotGridCustomFieldSortEventHandler handler = (PivotGridCustomFieldSortEventHandler)this.Events[customFieldSort];
			if(handler != null) {
				if(fieldSortEventArgs != null && (fieldSortEventArgs.Field != field || fieldSortEventArgs.Data != Data)) {
					fieldSortEventArgs = null;
				}
				if(fieldSortEventArgs == null) {
					fieldSortEventArgs = new PivotGridCustomFieldSortEventArgs(Data, field as PivotGridField);
				}
				fieldSortEventArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
				handler(this, fieldSortEventArgs);
				return fieldSortEventArgs.GetSortResult();
			} else
				return 3;
		}
		protected virtual object RaiseCustomGroupInterval(PivotGridFieldBase field, object value) {
			PivotGridCustomGroupIntervalEventHandler handler = (PivotGridCustomGroupIntervalEventHandler)this.Events[customGroupInterval];
			if(handler != null) {
				PivotCustomGroupIntervalEventArgs e = new PivotCustomGroupIntervalEventArgs((PivotGridField)field, value);
				handler(this, e);
				return e.GroupValue;
			} else
				return value;
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs((PivotGridField)field));
		}
		protected virtual void RaiseFieldAreaIndexChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaIndexChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs((PivotGridField)field));
		}
		protected virtual void RaiseFieldVisibleChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldVisibleChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs((PivotGridField)field));
		}
		protected virtual void RaiseFieldFilterChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldFilterChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs((PivotGridField)field));
		}
		protected virtual void RaisePageIndexChanged() {
			EventHandler handler = (EventHandler)Events[pageIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseFieldValueCollapsed(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueCollapsed];
			RaiseFieldValueCollapsedExpandedCore(field, values, isCollapsed, handler);
		}
		protected virtual void RaiseFieldValueExpanded(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueExpanded];
			RaiseFieldValueCollapsedExpandedCore(field, values, isCollapsed, handler);
		}
		protected virtual void RaiseFieldValueNotExpanded(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueNotExpanded];
			RaiseFieldValueCollapsedExpandedCore(field, values, isCollapsed, handler);
		}
		protected void RaiseFieldValueCollapsedExpandedCore(PivotGridFieldBase field, object[] values, bool isCollapsed, PivotFieldStateChangedEventHandler handler) {
			if(handler != null) {
				PivotFieldStateChangedEventArgs e = new PivotFieldStateChangedEventArgs(field as PivotGridField, values, isCollapsed);
				handler(this, e);
			}
		}
		protected virtual void RaiseControlHierarchyCreated(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[controlHierarchyCreated];
			if(handler != null) handler(this, e);
		}
		protected virtual bool RaiseFieldValueCollapsing(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			PivotFieldStateChangedCancelEventHandler handler = (PivotFieldStateChangedCancelEventHandler)this.Events[fieldValueCollapsing];
			return RaiseFieldValueExpandingCollapsingCore(field, values, isCollapsed, handler);
		}
		protected virtual bool RaiseFieldValueExpanding(PivotGridFieldBase field, object[] values, bool isCollapsed) {
			PivotFieldStateChangedCancelEventHandler handler = (PivotFieldStateChangedCancelEventHandler)this.Events[fieldValueExpanding];
			return RaiseFieldValueExpandingCollapsingCore(field, values, isCollapsed, handler);
		}
		private bool RaiseFieldValueExpandingCollapsingCore(PivotGridFieldBase field, object[] values, bool isCollapsed, PivotFieldStateChangedCancelEventHandler handler) {
			if(handler != null) {
				PivotFieldStateChangedCancelEventArgs e = new PivotFieldStateChangedCancelEventArgs(field as PivotGridField, values, isCollapsed);
				handler(this, e);
				return !e.Cancel;
			} else
				return true;
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridFieldBase field, object value) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(field as PivotGridField, value);
				handler(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(value);			
		}
		protected virtual string RaiseFieldValueDisplayText(PivotFieldValueItem item) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(item);
				handler(this, e);
				return e.DisplayText;
			} else
				return item.Text;
		}
		protected virtual string RaiseCellDisplayText(PivotGridCellItem cellItem) {
			PivotCellDisplayTextEventHandler handler = (PivotCellDisplayTextEventHandler)this.Events[customCellDisplayText];
			if(handler != null) {
				PivotCellDisplayTextEventArgs e = new PivotCellDisplayTextEventArgs(cellItem);
				handler(this, e);
				return e.DisplayText;
			} else
				return cellItem.Text;
		}
		PivotCellDisplayTextEventArgs CreateCellDisplayTextEventArgs(PivotGridCellItem cellItem) {
			return new PivotCellDisplayTextEventArgs(cellItem);
		}
		protected virtual void RaiseCustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle) {
			PivotCustomCellStyleEventHandler handler = (PivotCustomCellStyleEventHandler)this.Events[customCellStyle];
			if(handler != null) 
				handler(this, new PivotCustomCellStyleEventArgs(cellItem, cellStyle));
		}
		protected bool RaiseAddPopupMenuItem(MenuItemEnum item) {
			PivotAddPopupMenuItemEventHandler handler = (PivotAddPopupMenuItemEventHandler)this.Events[addPopupMenuItem];
			if(handler != null) {
				PivotAddPopupMenuItemEventArgs args = new PivotAddPopupMenuItemEventArgs(item);
				handler(this, args);
				return args.Add;
			}
			return true;
		}
		protected void RaisePopupMenuCreated(PivotGridPopupMenuType menuType, ASPxPivotGridPopupMenu menu) {
			PivotPopupMenuCreatedEventHandler handler = (PivotPopupMenuCreatedEventHandler)this.Events[popupMenuCreated];
			if(handler != null) {
				PivotPopupMenuCreatedEventArgs args = new PivotPopupMenuCreatedEventArgs(menuType, menu);
				handler(this, args);
			}
		}
		protected void RaiseDataAreaPopupCreated(PivotGridDataAreaPopup popup) {
			EventHandler<PivotDataAreaPopupCreatedEventArgs> handler = (EventHandler<PivotDataAreaPopupCreatedEventArgs>)this.Events[dataAreaPopupCreated];
			if(handler != null) {
				PivotDataAreaPopupCreatedEventArgs args = new PivotDataAreaPopupCreatedEventArgs(popup);
				handler(this, args);
			}
		}
		protected void RaiseOLAPQueryTimeout() {
			EventHandler handler = (EventHandler)this.Events[olapQueryTimeout];
			if(handler != null) handler(this, new EventArgs());
		}
		protected void RaiseLayoutChanged() {
			EventHandler handler = (EventHandler)this.Events[layoutChanged];
			if(handler != null) handler(this, new EventArgs());
		}
		protected internal void RaiseCustomCallback(string parameters) {
			PivotCustomCallbackEventHandler handler = (PivotCustomCallbackEventHandler)this.Events[customCallback];
			if(handler != null) handler(this, new PivotGridCustomCallbackEventArgs(parameters));
		}
		bool raiseAfterPerformCallbackCalled;
		protected internal void RaiseAfterPerformCallback() {
			if(this.raiseAfterPerformCallbackCalled) return;
			RaiseAfterPerformCallbackCore();
			this.raiseAfterPerformCallbackCalled = true;
		}
		protected internal void RaiseAfterPerformCallbackCore() {
			EventHandler handler = (EventHandler)this.Events[afterPerformCallback];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseBeforePerformDataSelect() {
			EventHandler handler = (EventHandler)this.Events[beforePerformDataSelect];
			if(handler != null) handler(this, new EventArgs());
		}
		public void CollapseAll() {
			Data.ChangeExpandedAll(false);
		}
		public void CollapseAllRows() {
			Data.ChangeExpandedAll(false, false);
		}
		public void CollapseAllColumns() {
			Data.ChangeExpandedAll(true, false);
		}
		public void ExpandAll() {
			Data.ChangeExpandedAll(true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedCellValue(columnIndex, rowIndex);
		}
		public int ColumnCount {
			get { return VisualItems.ColumnCount; }
		}
		public int RowCount {
			get { return VisualItems.UnpagedRowCount; }
		}
		public int VisibleRowsOnPage {
			get { return VisualItems.RowCount; }
		}
		public bool IsFieldValueCollapsed(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.IsUnpagedObjectCollapsed(field, cellIndex);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedFieldValue(field, cellIndex);
		}
		public IOLAPMember GetFieldValueOLAPMember(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedOLAPMember(field, cellIndex);
		}
		public object GetFieldValueByIndex(PivotGridField field, int fieldValueIndex) {
			EnsureRefreshData();
			return Data.GetFieldValue(field, fieldValueIndex, fieldValueIndex);
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			Data.SetIsDeserializing(true);
			Data.BeginUpdate();			
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			Data.OnDeserializationComplete();
			Data.EndUpdate();
			Data.SetIsDeserializing(false);
			ResetControlHierarchy();
		}
		void IXtraSerializable.OnStartSerializing() { }
		void IXtraSerializable.OnEndSerializing() { }
		#endregion
		public override bool IsLoading() {
			return base.IsLoading() || Data.IsDeserializing;
		}
		#region Skins
		protected override string GetSkinControlName() {
			return "PivotGrid";
		}
		protected override string[] GetChildControlNames() {
			return ChildControlNames;
		}
		#endregion
		#region Loading Panel Members
		internal new ImageProperties LoadingPanelImage { get { return base.LoadingPanelImage; } }
		internal ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		internal new LoadingPanelStyle LoadingPanelStyle { get { return base.LoadingPanelStyle; } }
		internal string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		#endregion
		#region ChartDataSource Members
		PivotChartDataSourceView chartDataSourceView;
		protected virtual PivotChartDataSourceView ChartDataSourceView {
			get {
				if(!Data.IsDataBound) DataBind();
				if(chartDataSourceView == null) 					
					chartDataSourceView = CreateChartDataSourceView();
				return chartDataSourceView;
			}
		}		
		protected EventHandler ChartDataSourceChanged;
		protected virtual void RaiseDataSourceChanged() {
			if(ChartDataSourceChanged != null)
				ChartDataSourceChanged(this, EventArgs.Empty);
		}
		protected virtual void InvalidateChartData() {
			if(chartDataSourceView != null)
				chartDataSourceView.InvalidateChartData();
			RaiseDataSourceChanged();
		}		
		protected virtual PivotChartDataSourceView CreateChartDataSourceView() {
			return new PivotChartDataSourceView(this);
		}
		event EventHandler IDataSource.DataSourceChanged { add { ChartDataSourceChanged += value; } remove { ChartDataSourceChanged -= value; } }
		DataSourceView IDataSource.GetView(string viewName) {
			if(viewName != string.Empty) return null;
			return ChartDataSourceView;
		}
		ICollection IDataSource.GetViewNames() {
			return new string[] { string.Empty };
		}
		protected virtual void OnOptionsChartDataSourceChanged(object sender, EventArgs e) {
			InvalidateChartData();
		}
		#endregion		
		#region IPopupFilterControlOwner Members
		void IPopupFilterControlOwner.CloseFilterControl() {
			IsPrefilterPopupVisible = false;
		}
		string IPopupFilterControlOwner.FilterPopupHeaderText {
			get { return PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.PrefilterFormCaption); }
		}
		object IPopupFilterControlOwner.GetControlCallbackResult() {
			return GetCallbackResult();
		}
		FilterControlImages IPopupFilterControlOwner.GetImages() {
			return ImagesPrefilterControl;
		}
		EditorImages IPopupFilterControlOwner.GetImagesEditors() {
			return ImagesEditors;
		}
		string IPopupFilterControlOwner.GetJavaScriptForApplyFilterControl() {
			return String.Format("pivotGrid_ApplyPrefilter('{0}');", ClientID);
		}
		string IPopupFilterControlOwner.GetJavaScriptForCloseFilterControl() {
			return String.Format("pivotGrid_HidePrefilter('{0}');", ClientID);
		}
		FilterControlStyles IPopupFilterControlOwner.GetStyles() {
			return StylesFilterControl;
		}
		EditorStyles IPopupFilterControlOwner.GetStylesEditors() {
			return StylesEditors;
		}
		string IPopupFilterControlOwner.MainElementID {
			get { return PivotGridWebData.ElementName_MainTD; }
		}
		ASPxWebControl IPopupFilterControlOwner.OwnerControl {
			get { return this; }
		}
		#endregion
		#region IFilterControlRowOwner Members
		void IFilterControlRowOwner.AppendDefaultDXClassName(WebControl control) {
			Styles.AppendDefaultDXClassName(control);
		}
		void IFilterControlRowOwner.AssignCheckBoxCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelCheckBoxCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignClearButtonCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelClearButtonCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignExpressionCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelExpressionCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignFilterStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignImageCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelImageCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignLinkStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelLinkStyle().AssignToControl(control);
		}
		string IFilterControlRowOwner.ClearButtonText {
			get { return string.Empty; }   
		}
		ImageProperties IFilterControlRowOwner.CreateFilterImage {
			get { return RenderHelper.GetPrefilterImage(); }
		}
		string IFilterControlRowOwner.GetJavaScriptForClearFilter() {
			return String.Format("pivotGrid_ClearPrefilter('{0}');", ClientID);
		}
		string IFilterControlRowOwner.GetJavaScriptForSetFilterEnabledForCheckbox() {
			return String.Format("pivotGrid_ChangePrefilterEnabled('{0}');", ClientID);
		}
		string IFilterControlRowOwner.GetJavaScriptForShowFilterControl() {
			return String.Format("pivotGrid_ShowPrefilter('{0}');", ClientID);
		}
		bool IFilterControlRowOwner.IsFilterEnabled {
			get { return Prefilter.Enabled; } 
		}
		bool IFilterControlRowOwner.IsFilterEnabledSupported {
			get { return true; }
		}
		string IFilterControlRowOwner.ShowFilterBuilderText {
			get { return PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.PopupMenuShowPrefilter); }  
		}
		void IFilterControlRowOwner.RaiseCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			CustomFilterExpressionDisplayTextEventHandler handler = Events[customFilterExpressionDisplayText] as CustomFilterExpressionDisplayTextEventHandler;
			if(handler == null) return;
			handler(this, e);
		}
		#endregion
		#region IFilterControlOwner Members
		int IFilterControlOwner.ColumnCount {
			get {
				int res = 0;
				for(int i = 0; i < Fields.Count; i++) {
					if(Fields[i].CanShowInPrefilter)
						res++;
				}
				return res;
			}
		}
		string IFilterControlOwner.FilterExpression {
			get {
				return Data.Prefilter.CriteriaString;
			}
			set {
				PostBackAction = new PivotGridPostBackPrefilterAction(this, PivotGridPrefilterCommand.Set, value);
			}
		}
		IFilterColumn IFilterControlOwner.GetFilterColumn(int index) {
			int res = index + 1;
			for(int i = 0; i < Fields.Count; i++) {
				if(Fields[i].CanShowInPrefilter) {
					res--;
					if(res == 0)
						return Fields[i];
				}
			}
			return null;
		}
		bool IFilterControlOwner.GetValueDisplayText(IFilterColumn column, object value, out string displayText) {
			displayText = Data.GetPivotFieldValueText((PivotGridField)column, value);
			return true;
		}
		#endregion
		#region IPopupFilterControlStyleOwner Members
		AppearanceStyle IPopupFilterControlStyleOwner.ButtonAreaStyle {
			get { return Styles.GetPrefilterBuilderButtonAreaStyle(); }
		}
		ImageProperties IPopupFilterControlStyleOwner.CloseButtonImage {
			get { return RenderHelper.GetCustomizationFieldsCloseImage(); }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.CloseButtonStyle {
			get { return Styles.PrefilterBuilderCloseButtonStyle; }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.HeaderStyle {
			get { return Styles.PrefilterBuilderHeaderStyle; }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.MainAreaStyle {
			get { return Styles.GetPrefilterBuilderMainAreaStyle(); }
		}
		#endregion
	}
	public class ASPxPivotGridDataHelper : DataHelper {
		public ASPxPivotGridDataHelper(ASPxPivotGrid pivot, string name) : base(pivot, name) { }
		public ASPxPivotGrid Pivot { get { return (ASPxPivotGrid)Control; } }
		public override void PerformSelect() {
			if(Pivot.DesignMode && !string.IsNullOrEmpty(Pivot.DataSourceID)) return;
			Pivot.RaiseBeforePerformDataSelect();
			ResetSelectArguments();
			base.PerformSelect();
		}
	}
}
