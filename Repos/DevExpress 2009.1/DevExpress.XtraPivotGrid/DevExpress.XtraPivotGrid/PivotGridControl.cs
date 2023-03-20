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
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Helpers;
using DevExpress.XtraPivotGrid.Customization;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraPivotGrid {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.XtraPivotGrid.Design.PivotGridControlDesigner, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(IDesigner)),
	ToolboxBitmap(typeof(DevExpress.XtraPivotGrid.PivotGridControl), DevExpress.Utils.ControlConstants.BitmapPath + "PivotGridControl.bmp"),
	Description("Represents data from an underlying data source in a cross-tabulated form; allows data from an OLAP server to be displayed. Supports summaries to be calculated manually and automatically using one of multiple summary functions.")
]
#if DXWhidbey
	[Docking(DockingBehavior.Ask)]
#endif
	public class PivotGridControl : DevExpress.XtraPivotGrid.ViewInfo.BaseViewInfoControl, IComponentLoading,
		ISupportLookAndFeel, IPrintable, IToolTipControlClient, IPivotGridEventsImplementor,
		IPivotGridDataOwner, DevExpress.Utils.Menu.IDXManagerPopupMenu, IBindingList, ITypedList, IPivotGridViewInfoDataOwner,
		IPivotGridPrinterOwner, IDataContainerBase {
		class ChartDataEnumerator : IEnumerator {
			PivotGridControl pivotGrid;
			PivotGridControl PivotGrid { get { return pivotGrid; } }
			ICollection Collection { get { return (ICollection)PivotGrid; } }
			IList List { get { return (IList)PivotGrid; } }
			int index;
			int Index { get { return index; } set { index = value; } }
			public ChartDataEnumerator(PivotGridControl pivotGrid) {
				this.pivotGrid = pivotGrid;
			}
			#region IEnumerator Members
			object IEnumerator.Current { get { return List[Index]; } }
			bool IEnumerator.MoveNext() {
				if(!PivotGrid.IsChartDataValid) throw new Exception("Invalid enumerator");
				if(Collection.Count == 0 || Index == Collection.Count - 1) return false;
				Index++;
				return true;
			}
			void IEnumerator.Reset() { Index = 0; }
			#endregion
		}
		protected PivotGridViewInfoData fData;
		object dataSource;
		string dataMember;
		string olapConnectionString;
		UserLookAndFeel lookAndFeel;
		PivotGridPrinter printer;
		ToolTipController toolTipController;
		DynamicPrintHelper printHelper;
		static readonly object dataSourceChanged = new object();
		static readonly object beginRefresh = new object();
		static readonly object endRefresh = new object();
		static readonly object gridLayout = new object();
		static readonly object fieldAreaChanging = new object();
		static readonly object customUnboundFieldData = new object();
		static readonly object customSummary = new object();
		static readonly object customFieldSort = new object();
		static readonly object showingCustomizationForm = new object();
		static readonly object showCustomizationForm = new object();
		static readonly object hideCustomizationForm = new object();
		static readonly object layoutUpgrade = new object();
		static readonly object beforeLoadLayout = new object();
		static readonly object fieldFilterChanged = new object();
		static readonly object fieldAreaChanged = new object();
		static readonly object fieldExpandedInFieldGroupChanged = new object();
		static readonly object fieldWidthChanged = new object();
		static readonly object fieldValueDisplayText = new object();
		static readonly object customGroupInterval = new object();
		static readonly object fieldValueImageIndex = new object();
		static readonly object customCellDisplayText = new object();
		static readonly object cellDoubleClick = new object();
		static readonly object cellClick = new object();
		static readonly object cellSelectionChanged = new object();
		static readonly object focusedCellChanged = new object();
		static readonly object customDrawCell = new object();
		static readonly object customAppearance = new object();
		static readonly object customDrawFieldValue = new object();
		static readonly object customDrawFieldHeader = new object();
		static readonly object customDrawFieldHeaderArea = new object();
		static readonly object customDrawEmptyArea = new object();
		static readonly object customRowHeight = new object();
		static readonly object customColumnWidth = new object();
		static readonly object menuItemClick = new object();
		static readonly object showMenu = new object();
		static readonly object fieldValueCollapsed = new object();
		static readonly object fieldValueExpanded = new object();
		static readonly object fieldValueCollapsing = new object();
		static readonly object fieldValueExpanding = new object();
		static readonly object fieldValueNotExpanded = new object();
		static readonly object prefilterCriteriaChanged = new object();
		static readonly object olapQueryTimeout = new object();
		static readonly object editValueChanged = new object();
		static readonly object validatingEditor = new object();
		static readonly object invalidValueException = new object();
		static readonly object customEditValue = new object();
		static readonly object showingEditor = new object();
		static readonly object shownEditor = new object();
		static readonly object hiddenEditor = new object();
		static readonly object customCellEdit = new object();
		static readonly object customCellEditForEditing = new object();
		static readonly object customExportHeader = new object();
		static readonly object customExportFieldValue = new object();
		static readonly object customExportCell = new object();
		static readonly object exportStarted = new object();
		static readonly object exportFinished = new object();
		static ImageCollection customizationTreeNodeImages;
		[Description("Specifies the collection of images identifying nodes in the Customization Form used when the control is in OLAP mode.")]
		public static ImageCollection CustomizationTreeNodeImages {
			get {
				if(customizationTreeNodeImages == null)
					customizationTreeNodeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Images.FieldsTreeHierarchy.png", 
						typeof(PivotGridControl).Assembly, new Size(16, 16));
				return customizationTreeNodeImages;
			}
			set {
				customizationTreeNodeImages = value;
			}
		}
		public static void About() {
				DevExpress.Utils.About.AboutForm.Show(typeof(PivotGridControl), DevExpress.Utils.About.ProductKind.XtraPivotGrid); 
		}
		public PivotGridControl()
			: base() {
			InitializeCore(null);
		}
		protected PivotGridControl(PivotGridViewInfoData viewInfoData)
			: base() {
			InitializeCore(viewInfoData);
		}
		void InitializeCore(PivotGridViewInfoData viewInfoData) {
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.Selectable, true);
			if(viewInfoData == null) {
				fData = CreateData();
			OptionsView.ShowAllTotals();
			} else
				fData = viewInfoData;
			fData.OptionsChartDataSource.Changed += OnOptionsChartDataSourceChanged;
			this.dataSource = null;
			this.dataMember = string.Empty;
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeel_StyleChanged);
			fData.ControlLookAndFeel = lookAndFeel;
			HScrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			VScrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			this.printer = CreatePrinter();
			ToolTipController.DefaultController.AddClientControl(this);
			this.toolTipController = null;
			this.editorContainer = new PivotGridEditorContainer();
			this.editorHelper = EditorContainer.EditorHelper;
			EditorHelper.Pivot = this;
		}
		protected override void Dispose(bool disposing) {
			if(IsDisposed) return;
			if(disposing) {
				OptionsChartDataSource.Changed -= OnOptionsChartDataSourceChanged;
				if(this.lookAndFeel != null) {
					this.lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeel_StyleChanged);
					this.lookAndFeel.Dispose();
					this.lookAndFeel = null;
				}
				if(printer != null) {
					this.printer.Dispose();
					this.printer = null;
				}
				DisposeEdit();
				DisposeData();						
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
			}
			base.Dispose(disposing);
		}
		protected void DisposeEdit() {
			HideEditor();
			if(this.editorContainer != null) {
				this.editorContainer.Dispose();
				this.editorContainer = null;
			}
		}
		protected virtual void DisposeData() {
			if(this.fData != null) {
				this.fData.Dispose();
				this.fData = null;
			}
		}
		protected virtual PivotGridViewInfoData CreateData() {
			return new PivotGridViewInfoData(this);
		}
		public override void BeginUpdate() {
			base.BeginUpdate();
			Data.BeginUpdate();
			InvalidateChartData(true);
		}
		public new void EndUpdate() { 
			base.EndUpdate(); 
		}
		protected override void CancelUpdate() {
			base.CancelUpdate();
			Data.EndUpdate();
		}
		protected override Rectangle ScrollableRectangle { get { return ViewInfo.ScrollableRectangle; } }
		public void LayoutChanged() {
			Data.LayoutChanged();
		}
		[Description("Gets or sets the object used as the data source for the current control. "), Category("Data"),
#if DXWhidbey
 AttributeProvider(typeof(IListSource)),
#else
		   TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
#endif
 DefaultValue(null), Localizable(true)]
		public object DataSource {
			get { return dataSource; }
			set {
				if(value == this) return;
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(IsValidDataSource(value)) {
					dataSource = value;
					isDataSourceActive = false;
					if(value != null) OLAPConnectionString = null;
					ActivateDataSource();
				}
			}
		}
		[Description("Gets or sets the data source member which supplies data to the control."), Category("Data"), Localizable(true), DefaultValue(""),
		Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				ActivateDataSource();
			}
		}
		[Description("Specifies a connection string to a cube in an MS Analysis Services database."), Category("Data"), Localizable(true), DefaultValue(null),
		Editor("DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))]
		public string OLAPConnectionString {
			get { return Data.OLAPConnectionString; }
			set {
				if(olapConnectionString != value) {
					olapConnectionString = value;
					isDataSourceActive = false;
					if(!String.IsNullOrEmpty(value)) DataSource = null;
					ActivateDataSource();
				}
			}
		}
		[Browsable(false)]
		public bool IsOLAPDataSource { get { return !String.IsNullOrEmpty(OLAPConnectionString); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList ListSource { get { return Data.ListSource; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		[Category("Data"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridCells Cells {
			get {
				EnsureViewInfoIsCalculated();
				return Data.Cells;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the OptionsChartDataSource.ChartDataVertical property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ChartDataVertical {
			get { return OptionsChartDataSource.ChartDataVertical; }
			set { OptionsChartDataSource.ChartDataVertical = value; }
		}
		[Description("Gets or sets the border style for the pivot grid."), Category("Appearance"), DefaultValue(BorderStyles.Default)]
		public BorderStyles BorderStyle { get { return Data.BorderStyle; } set { Data.BorderStyle = value; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		void ResetLookAndFeel() { LookAndFeel.Reset(); }
		[Description("Provides access to the settings that control the pivot grid's look and feel."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[Description("Provides access to the properties that control the appearance of the pivot grid's elements."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridAppearances Appearance { get { return Data.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridAppearances PaintAppearance { get { return Data.PaintAppearance; } }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		bool ShouldSerializeAppearancePrint() { return AppearancePrint.ShouldSerialize(); }
		[Description("Provides access to the properties that specify the appearances of the pivot grid's elements when the pivot grid is printed."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public virtual PivotGridAppearancesPrint AppearancePrint { get { return Data.AppearancePrint; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PivotGridAppearancesPrint PaintAppearancePrint { get { return Data.PaintAppearancePrint; } }
		[Description("Provides access to a pivot grid's field collection."), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PivotGridFieldCollection Fields { get { return Data.Fields; } }
		public PivotGridField GetFieldAt(Point pt) {
			return ViewInfo.GetFieldAt(pt) as PivotGridField;
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
		[Description("Gets the collection of style format conditions for a pivot grid."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PivotGridFormatConditionCollection FormatConditions { get { return Data.FormatConditions; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PivotGridGroupCollection Groups { get { return Data.Groups; } }
		bool ShouldSerializePrefilter() { return this.Prefilter.ShouldSerialize(); }
		void ResetPrefilter() { this.Prefilter.Reset(); }
		[Description("Gets the Prefilter's settings."), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public Prefilter Prefilter { get { return Data.Prefilter; } }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerialize(); }
		void ResetOptionsBehavior() { OptionsBehavior.Reset(); }
		[Description("Provides access to the control's behavior options. "), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsBehavior OptionsBehavior { get { return Data.OptionsBehavior; } }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerialize(); }
		void ResetOptionsCustomization() { OptionsCustomization.Reset(); }
		[Description("Provides access to the pivot grid's customization options. "), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsCustomizationEx OptionsCustomization { get { return Data.OptionsCustomization; } }
		bool ShouldSerializeOptionsDataField() { return OptionsDataField.ShouldSerialize(); }
		void ResetOptionsDataField() { OptionsDataField.Reset(); }
		[Description("Provides access to the options which control the presentation of the data fields in the PivotGrid."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsDataFieldEx OptionsDataField { get { return Data.OptionsDataField; } }
		bool ShouldSerializeOptionsData() { return OptionsData.ShouldSerialize(); }
		void ResetOptionsData() { OptionsData.Reset(); }
		[Description("Provides access to the pivot grid's data specific options."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsData OptionsData { get { return Data.OptionsData; } }
		bool ShouldSerializeOptionsChartDataSource() { return OptionsChartDataSource.ShouldSerialize(); }
		void ResetOptionsChartDataSource() { OptionsChartDataSource.Reset(); }
		[Description("Provides access to the options controlling the display of the PivotGrid control's data in a chart control."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsChartDataSource OptionsChartDataSource { get { return Data.OptionsChartDataSource; } }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerialize(); }
		void ResetOptionsLayout() { OptionsLayout.Reset(); }
		[Description("Provides options which control how the pivot grid's layout is stored to/restored from storage (a stream, xml file or system registry)."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayoutPivotGrid OptionsLayout { get { return Data.OptionsLayout; } }
		bool ShouldSerializeOptionsHint() { return OptionsHint.ShouldSerialize(); }
		void ResetOptionsHint() { OptionsHint.Reset(); }
		[Description("Provides access to the pivot grid's hint options."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsHint OptionsHint { get { return Data.OptionsHint; } }
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerialize(); }
		void ResetOptionsMenu() { OptionsMenu.Reset(); }
		[Description("Provides access to the pivot grid's menu options."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsMenu OptionsMenu { get { return Data.OptionsMenu; } }
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerialize(); }
		void ResetOptionsSelection() { OptionsSelection.Reset(); }
		[Description("Provides access to the pivot grid's selection options."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsSelection OptionsSelection { get { return Data.OptionsSelection; } }
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerialize(); }
		void ResetOptionsPrint() { OptionsPrint.Reset(); }
		[Description("Provides access to the pivot grid's print options. "), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsPrint OptionsPrint { get { return Data.OptionsPrint; } }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerialize(); }
		void ResetOptionsView() { OptionsView.Reset(); }
		[Description("Provides access to the pivot grid's display options."), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public virtual PivotGridOptionsView OptionsView { get { return Data.OptionsView; } }
		[Description("Gets or sets the menu manager which controls the look and feel of the context menus."), DefaultValue(null)]
		public IDXMenuManager MenuManager { get { return Data.MenuManager; } set { Data.MenuManager = value; } }
		[Description("Fires when the pivot grid's data source changes."), Category("Data")]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[Description("Occurs before the control's data recalculation starts."), Category("Data")]
		public event EventHandler BeginRefresh {
			add { this.Events.AddHandler(beginRefresh, value); }
			remove { this.Events.RemoveHandler(beginRefresh, value); }
		}
		[Description("Occurs after the control's data recalculation has been completed."), Category("Data")]
		public event EventHandler EndRefresh {
			add { this.Events.AddHandler(endRefresh, value); }
			remove { this.Events.RemoveHandler(endRefresh, value); }
		}
		[Description("Fires after the control's layout has been changed."), Category("Layout")]
		public event EventHandler GridLayout {
			add { this.Events.AddHandler(gridLayout, value); }
			remove { this.Events.RemoveHandler(gridLayout, value); }
		}
		[Description("Enables you to control whether the dragged field header can be dropped at the area it's currently located over. "), Category("Layout")]
		public event PivotAreaChangingEventHandler FieldAreaChanging {
			add { this.Events.AddHandler(fieldAreaChanging, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanging, value); }
		}
		[Description("Enables data to be provided for unbound fields. "), Category("Data")]
		public event CustomFieldDataEventHandler CustomUnboundFieldData {
			add { this.Events.AddHandler(customUnboundFieldData, value); }
			remove { this.Events.RemoveHandler(customUnboundFieldData, value); }
		}
		[Description("Enables summary values to be calculated manually."), Category("Data")]
		public event PivotGridCustomSummaryEventHandler CustomSummary {
			add { this.Events.AddHandler(customSummary, value); }
			remove { this.Events.RemoveHandler(customSummary, value); }
		}
		[Description("Provides the ability to sort data using custom rules."), Category("Data")]
		public event PivotGridCustomFieldSortEventHandler CustomFieldSort {
			add { this.Events.AddHandler(customFieldSort, value); }
			remove { this.Events.RemoveHandler(customFieldSort, value); }
		}
		[Description("Occurs before the customization form is displayed."), Category("Customization")]
		public event CustomizationFormShowingEventHandler ShowingCustomizationForm {
			add { this.Events.AddHandler(showingCustomizationForm, value); }
			remove { this.Events.RemoveHandler(showingCustomizationForm, value); }
		}
		[Description("Fires immediately after the customization form has been invoked. "), Category("Customization")]
		public event EventHandler ShowCustomizationForm {
			add { this.Events.AddHandler(showCustomizationForm, value); }
			remove { this.Events.RemoveHandler(showCustomizationForm, value); }
		}
		[Description("Fires before the customization form is closed. "), Category("Customization")]
		public event EventHandler HideCustomizationForm {
			add { this.Events.AddHandler(hideCustomizationForm, value); }
			remove { this.Events.RemoveHandler(hideCustomizationForm, value); }
		}
		[Description("Occurs when a layout is restored from a data store (a stream, xml file or the system registry) and its version is different than that of the control's current layout version."), Category("Customization")]
		public event LayoutUpgadeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[Description("Occurs before a layout is restored from storage (a stream, xml file or the system registry)."), Category("Customization")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[Description("Occurs after the filter criteria have been changed for a specific field."), Category("Data")]
		public event PivotFieldEventHandler FieldFilterChanged {
			add { this.Events.AddHandler(fieldFilterChanged, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanged, value); }
		}
		[Description("Occurs after the field's location or visibility has been changed. "), Category("Layout")]
		public event PivotFieldEventHandler FieldAreaChanged {
			add { this.Events.AddHandler(fieldAreaChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanged, value); }
		}
		[Description("Fires when the expansion status of fields combined into a field group is changed."), Category("Layout")]
		public event PivotFieldEventHandler FieldExpandedInFieldGroupChanged {
			add { this.Events.AddHandler(fieldExpandedInFieldGroupChanged, value); }
			remove { this.Events.RemoveHandler(fieldExpandedInFieldGroupChanged, value); }
		}
		[Description("Provides a notification that the field's width has been changed."), Category("Layout")]
		public event PivotFieldEventHandler FieldWidthChanged {
			add { this.Events.AddHandler(fieldWidthChanged, value); }
			remove { this.Events.RemoveHandler(fieldWidthChanged, value); }
		}
		[Description("Enables the display text of individual column and row headers and filter dropdown items to be customized."), Category("Data")]
		public event PivotFieldDisplayTextEventHandler FieldValueDisplayText {
			add { this.Events.AddHandler(fieldValueDisplayText, value); }
			remove { this.Events.RemoveHandler(fieldValueDisplayText, value); }
		}
		[Description("Allows you to custom group values of column and row fields."), Category("Data")]
		public event PivotCustomGroupIntervalEventHandler CustomGroupInterval {
			add { this.Events.AddHandler(customGroupInterval, value); }
			remove { this.Events.RemoveHandler(customGroupInterval, value); }
		}
		[Description("Enables images to be shown within column and row headers."), Category("Appearance")]
		public event PivotFieldImageIndexEventHandler FieldValueImageIndex {
			add { this.Events.AddHandler(fieldValueImageIndex, value); }
			remove { this.Events.RemoveHandler(fieldValueImageIndex, value); }
		}
		[Description("Enables custom display text to be provided for the cells displayed within the Data Area."), Category("Data")]
		public event PivotCellDisplayTextEventHandler CustomCellDisplayText {
			add { this.Events.AddHandler(customCellDisplayText, value); }
			remove { this.Events.RemoveHandler(customCellDisplayText, value); }
		}
		[Description("Occurs when a cell is double-clicked."), Category("Behavior")]
		public event PivotCellEventHandler CellDoubleClick {
			add { this.Events.AddHandler(cellDoubleClick, value); }
			remove { this.Events.RemoveHandler(cellDoubleClick, value); }
		}
		[Description("Occurs when a cell is clicked."), Category("Behavior")]
		public event PivotCellEventHandler CellClick {
			add { this.Events.AddHandler(cellClick, value); }
			remove { this.Events.RemoveHandler(cellClick, value); }
		}
		[Description("Fires in response to cell focus changing. "), Category("Behavior")]
		public event EventHandler FocusedCellChanged {
			add { this.Events.AddHandler(focusedCellChanged, value); }
			remove { this.Events.RemoveHandler(focusedCellChanged, value); }
		}
		[Description("Occurs when the cell selection is modified."), Category("Behavior")]
		public event EventHandler CellSelectionChanged {
			add { this.Events.AddHandler(cellSelectionChanged, value); }
			remove { this.Events.RemoveHandler(cellSelectionChanged, value); }
		}
		[Description("Enables you to customize the context menu or prevent it from being shown."), Category("Behavior")]
		public event PivotGridMenuEventHandler ShowMenu {
			add { this.Events.AddHandler(showMenu, value); }
			remove { this.Events.RemoveHandler(showMenu, value); }
		}
		[Description("Allows custom responses to be provided for clicks on context menu items."), Category("Behavior")]
		public event PivotGridMenuItemClickEventHandler MenuItemClick {
			add { this.Events.AddHandler(menuItemClick, value); }
			remove { this.Events.RemoveHandler(menuItemClick, value); }
		}
		[Description("Enables header areas to be painted manually."), Category("Appearance")]
		public event PivotCustomDrawHeaderAreaEventHandler CustomDrawFieldHeaderArea {
			add { this.Events.AddHandler(customDrawFieldHeaderArea, value); }
			remove { this.Events.RemoveHandler(customDrawFieldHeaderArea, value); }
		}
		[Description("Enables a pivot grid's background to be custom painted."), Category("Appearance")]
		public event PivotCustomDrawEventHandler CustomDrawEmptyArea {
			add { this.Events.AddHandler(customDrawEmptyArea, value); }
			remove { this.Events.RemoveHandler(customDrawEmptyArea, value); }
		}
		[Description("Enables data cells to be painted manually."), Category("Appearance")]
		public event PivotCustomDrawCellEventHandler CustomDrawCell {
			add { this.Events.AddHandler(customDrawCell, value); }
			remove { this.Events.RemoveHandler(customDrawCell, value); }
		}
		[Description("Allows the appearances of cells to be dynamically customized when the control is displayed onscreen and in the print output."), Category("Appearance")]
		public event PivotCustomAppearanceEventHandler CustomAppearance {
			add { this.Events.AddHandler(customAppearance, value); }
			remove { this.Events.RemoveHandler(customAppearance, value); }
		}
		[Description("Enables field headers to be painted manually."), Category("Appearance")]
		public event PivotCustomDrawFieldHeaderEventHandler CustomDrawFieldHeader {
			add { this.Events.AddHandler(customDrawFieldHeader, value); }
			remove { this.Events.RemoveHandler(customDrawFieldHeader, value); }
		}
		[Description("Enables column and row headers to be painted manually."), Category("Appearance")]
		public event PivotCustomDrawFieldValueEventHandler CustomDrawFieldValue {
			add { this.Events.AddHandler(customDrawFieldValue, value); }
			remove { this.Events.RemoveHandler(customDrawFieldValue, value); }
		}
		[Description("This member supports the internal infrastructure and is not intended to be used directly from your code."), Category("Appearance")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public event EventHandler<PivotCustomRowHeightEventArgs> CustomRowHeight {
			add { this.Events.AddHandler(customRowHeight, value); }
			remove { this.Events.RemoveHandler(customRowHeight, value); }
		}
		[Description("This member supports the internal infrastructure and is not intended to be used directly from your code."), Category("Appearance")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public event EventHandler<PivotCustomColumnWidthEventArgs> CustomColumnWidth {
			add { this.Events.AddHandler(customColumnWidth, value); }
			remove { this.Events.RemoveHandler(customColumnWidth, value); }
		}
		[Description("Fires immediately after a field value has been collapsed."), Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueCollapsed {
			add { this.Events.AddHandler(fieldValueCollapsed, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsed, value); }
		}
		[Description("Fires immediately after a field value has been expanded."), Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueExpanded {
			add { this.Events.AddHandler(fieldValueExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanded, value); }
		}
		[Description(""), Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueNotExpanded {
			add { this.Events.AddHandler(fieldValueNotExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueNotExpanded, value); }
		}
		[Description("Enables you to control whether field values can be collapsed. "), Category("Behavior")]
		public event PivotFieldValueCancelEventHandler FieldValueCollapsing {
			add { this.Events.AddHandler(fieldValueCollapsing, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsing, value); }
		}
		[Description("Enables you to control whether field values can be expanded. "), Category("Behavior")]
		public event PivotFieldValueCancelEventHandler FieldValueExpanding {
			add { this.Events.AddHandler(fieldValueExpanding, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanding, value); }
		}
		[Description("Fires when the Prefilter's criteria are changed."), Category("Data")]
		public event EventHandler PrefilterCriteriaChanged {
			add { this.Events.AddHandler(prefilterCriteriaChanged, value); }
			remove { this.Events.RemoveHandler(prefilterCriteriaChanged, value); }
		}
		[Description("Fires after a specific interval of time has elapsed when sending a query to an OLAP server."), Category("Data")]
		public event EventHandler OLAPQueryTimeout {
			add { this.Events.AddHandler(olapQueryTimeout, value); }
			remove { this.Events.RemoveHandler(olapQueryTimeout, value); }
		}
		[Description("Fires immediately after a cell's value has been changed."), Category("Editor")]
		public event EditValueChangedEventHandler EditValueChanged {
			add { this.Events.AddHandler(editValueChanged, value); }
			remove { this.Events.RemoveHandler(editValueChanged, value); }
		}
		[Description("Enables you to manually validate cell values."), Category("Editor")]
		public event BaseContainerValidateEditorEventHandler ValidatingEditor {
			add { this.Events.AddHandler(validatingEditor, value); }
			remove { this.Events.RemoveHandler(validatingEditor, value); }
		}
		[Description("Enables you to provide a proper response to entering an invalid cell value."), Category("Editor")]
		public event InvalidValueExceptionEventHandler InvalidValueException {
			add { this.Events.AddHandler(invalidValueException, value); }
			remove { this.Events.RemoveHandler(invalidValueException, value); }
		}
		[Description("Enables you to change cell values."), Category("Editor")]
		public event CustomEditValueEventHandler CustomEditValue {
			add { this.Events.AddHandler(customEditValue, value); }
			remove { this.Events.RemoveHandler(customEditValue, value); }
		}
		[Description("Allows you to cancel editor activation."), Category("Editor")]
		public event EventHandler<CancelPivotCellEditEventArgs> ShowingEditor {
			add { this.Events.AddHandler(showingEditor, value); }
			remove { this.Events.RemoveHandler(showingEditor, value); }
		}
		[Description("Fires immediately after a cell editor has been invoked. "), Category("Editor")]
		public event EventHandler<PivotCellEditEventArgs> ShownEditor {
			add { this.Events.AddHandler(shownEditor, value); }
			remove { this.Events.RemoveHandler(shownEditor, value); }
		}
		[Description("Fires immediately after an inplace editor has been closed. "), Category("Editor")]
		public event EventHandler<PivotCellEditEventArgs> HiddenEditor {
			add { this.Events.AddHandler(hiddenEditor, value); }
			remove { this.Events.RemoveHandler(hiddenEditor, value); }
		}
		[Description("Allows you to assign editors for particular cells."), Category("Editor")]
		public event EventHandler<PivotCustomCellEditEventArgs> CustomCellEdit {
			add { this.Events.AddHandler(customCellEdit, value); }
			remove { this.Events.RemoveHandler(customCellEdit, value); }
		}
		[Description("Allows you to assign a custom editor to a cell for in-place editing, and override the default cell editor, which is used in both display and edit modes, by default."), Category("Editor")]
		public event EventHandler<PivotCustomCellEditEventArgs> CustomCellEditForEditing {
			add { this.Events.AddHandler(customCellEditForEditing, value); }
			remove { this.Events.RemoveHandler(customCellEditForEditing, value); }
		}
		[Description("Enables you to render a different content for individual field headers."), Category("Export & Printing")]
		public event EventHandler<CustomExportHeaderEventArgs> CustomExportHeader {
			add { this.Events.AddHandler(customExportHeader, value); }
			remove { this.Events.RemoveHandler(customExportHeader, value); }
		}
		[Description("Enables you to render a different content for individual field values."), Category("Export & Printing")]
		public event EventHandler<CustomExportFieldValueEventArgs> CustomExportFieldValue {
			add { this.Events.AddHandler(customExportFieldValue, value); }
			remove { this.Events.RemoveHandler(customExportFieldValue, value); }
		}
		[Description("Enables you to render a different content for individual cells."), Category("Export & Printing")]
		public event EventHandler<CustomExportCellEventArgs> CustomExportCell {
			add { this.Events.AddHandler(customExportCell, value); }
			remove { this.Events.RemoveHandler(customExportCell, value); }
		}
		[Description("Occurs after the PivotGrid's export has been started."), Category("Export & Printing")]
		public event EventHandler ExportStarted {
			add { this.Events.AddHandler(exportStarted, value); }
			remove { this.Events.RemoveHandler(exportStarted, value); }
		}
		[Description("Occurs after the PivotGrid's export has been completed."), Category("Export & Printing")]
		public event EventHandler ExportFinished {
			add { this.Events.AddHandler(exportFinished, value); }
			remove { this.Events.RemoveHandler(exportFinished, value); }
		}
		[Description("Gets or sets the source of the images available for display within field headers."), Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object HeaderImages { get { return Data.HeaderImages; } set { Data.HeaderImages = value; } }
		[Description("Gets or sets the source of the images that are available for display within field values."), Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ValueImages { get { return Data.ValueImages; } set { Data.ValueImages = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		public void BestFit(PivotGridField field) { Data.BestFit(field); }
		public void BestFit() { Data.BestFit(); }
		public PivotGridHitInfo CalcHitInfo(Point hitPoint) { return ViewInfo.CalcHitInfo(hitPoint); }
		public void RefreshData() {
			Data.ReloadData();
		}
		public void RetrieveFields() {
			Data.RetrieveFields();
		}
		public void FieldsCustomization(Point showPoint) {
			Data.FieldsCustomization(showPoint);
		}
		public void FieldsCustomization() {
			Data.FieldsCustomization();
		}
		public void FieldsCustomization(Control parentControl) {
			Data.FieldsCustomization(parentControl);
		}
		public void DestroyCustomization() {
			Data.DestroyCustomization();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle CustomizationFormBounds { get { return Data.CustomizationFormBounds; } set { Data.CustomizationFormBounds = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CustomizationForm CustomizationForm { get { return Data.CustomizationForm; } }
		[Browsable(false)]
		public bool CanResizeField(Point pt) {
			if(!ViewInfo.IsReady) return false;
			return ViewInfo.GetSizingField(pt.X, pt.Y) != null;
		}
		public void SaveLayoutToXml(string xmlFile) { SaveLayoutToXml(xmlFile, OptionsLayout); }
		public void RestoreLayoutFromXml(string xmlFile) { RestoreLayoutFromXml(xmlFile, OptionsLayout); }
		public void SaveLayoutToRegistry(string path) { SaveLayoutToRegistry(path, OptionsLayout); }
		public void RestoreLayoutFromRegistry(string path) { RestoreLayoutFromRegistry(path, OptionsLayout); }
		public void SaveLayoutToStream(Stream stream) { SaveLayoutToStream(stream, OptionsLayout); }
		public void RestoreLayoutFromStream(Stream stream) { RestoreLayoutFromStream(stream, OptionsLayout); }
		public void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		public void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Data.SaveLayoutCore(serializer, path, options);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Data.RestoreLayoutCore(serializer, path, options);
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			Data.SaveCollapsedStateToStream(stream);
		}
		public void SaveCollapsedStateToFile(string path) {
			using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				SaveCollapsedStateToStream(stream);
			}
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			Data.LoadCollapsedStateFromStream(stream);
			LayoutChanged();
		}
		public void LoadCollapsedStateFromFile(string path) {
			FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			LoadCollapsedStateFromStream(stream);
			stream.Close();
		}
		public void SavePivotGridToFile(string path) {
			SavePivotGridToFile(path, false);
		}
		public void SavePivotGridToFile(string path, bool compress) {
			Data.SavePivotGridToFile(path, compress);
		}
		public void SavePivotGridToStream(Stream stream) {
			SavePivotGridToStream(stream, false);
		}
		public void SavePivotGridToStream(Stream stream, bool compress) {
			Data.SavePivotGridToStream(stream, compress);
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
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		public bool IsObjectCollapsed(PivotGridField field, int lastLevelIndex) {
			return VisualItems.IsObjectCollapsed(field, lastLevelIndex);
		}
		public object GetFieldValue(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValue(field, lastLevelIndex);
		}
		public PivotGridValueType GetFieldValueType(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValueType(field, lastLevelIndex);
		}
		public PivotGridValueType GetFieldValueType(bool isColumn, int lastLevelIndex) {
			return VisualItems.GetLastLevelItem(isColumn, lastLevelIndex).ValueType;
		}
		public IOLAPMember GetFieldValueOLAPMember(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetOLAPMember(field, lastLevelIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {			
			return VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource();
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, 
			List<string> customColumns) {
			return CreateOLAPDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, 
			List<string> customColumns) {
			return VisualItems.CreateOLAPDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		internal Rectangle GetRealSelection(Rectangle selection) {
			int x1 = selection.Left, y1 = selection.Top,
				x2 = selection.Right, y2 = selection.Bottom;
			if(x1 < 0) x1 = 0;
			if(y1 < 0) y1 = 0;
			if(x2 >= ViewInfo.CellsArea.ColumnCount) x2 = ViewInfo.CellsArea.ColumnCount - 1;
			if(y2 >= ViewInfo.CellsArea.RowCount) y2 = ViewInfo.CellsArea.RowCount - 1;
			int lastColLevel = Data.GetLevelCount(true) - 1,
				lastRowLevel = Data.GetLevelCount(false) - 1;
			PivotCellViewInfoBase cell1 = ViewInfo.CellsArea.CreateCellViewInfo(x1, y1),
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, y2);
			if(cell2.ColumnField != null && cell2.ColumnField.AreaIndex != lastColLevel &&
				cell2.ColumnFieldIndex < ViewInfo.CellsArea.CreateCellViewInfo(x2 - 1, y2).ColumnFieldIndex) {
				int startAreaIndex = cell2.ColumnField.AreaIndex;
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(++x2, y2);
				while(cell2.ColumnField != null && cell2.ColumnField.AreaIndex < startAreaIndex &&
					x2 < ViewInfo.CellsArea.ColumnCount && cell2.RowFieldIndex >= 0) {
					cell2 = ViewInfo.CellsArea.CreateCellViewInfo(++x2, y2);
				}
			}
			if(cell2.RowField != null && cell2.RowField.AreaIndex != lastRowLevel &&
				cell2.RowFieldIndex < ViewInfo.CellsArea.CreateCellViewInfo(x2, y2 - 1).RowFieldIndex) {
				int startAreaIndex = cell2.RowField.AreaIndex;
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, ++y2);
				while(cell2.RowField != null && cell2.RowField.AreaIndex < startAreaIndex &&
					y2 < ViewInfo.CellsArea.RowCount && cell2.RowFieldIndex >= 0) {
					cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, ++y2);
				}
			}
			int realX1 = cell1.ColumnFieldIndex >= 0 ? cell1.ColumnFieldIndex : 0,
				realY1 = cell1.RowFieldIndex >= 0 ? cell1.RowFieldIndex : 0,
				realX2 = cell2.ColumnFieldIndex >= 0 ? cell2.ColumnFieldIndex : Data.GetCellCount(true),
				realY2 = cell2.RowFieldIndex >= 0 ? cell2.RowFieldIndex : Data.GetCellCount(false);
			return new Rectangle(realX1, realY1, realX2 - realX1, realY2 - realY1);
		}
		protected virtual void RaiseDataSourceChanged() {
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseBeginRefresh() {
			EventHandler handler = (EventHandler)this.Events[beginRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEndRefresh() {
			EventHandler handler = (EventHandler)this.Events[endRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseGridLayout() {
			EventHandler handler = (EventHandler)this.Events[gridLayout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			PivotAreaChangingEventHandler handler = (PivotAreaChangingEventHandler)this.Events[fieldAreaChanging];
			if(handler != null) {
				PivotAreaChangingEventArgs e = new PivotAreaChangingEventArgs(field, newArea, newAreaIndex);
				handler(this, e);
				return e.Allow;
			} else
				return true;
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
		protected virtual void RaiseCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
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
				return PivotGridData.UnhandledCustomFieldSort;
		}
		protected virtual void RaiseHideCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[hideCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseShowingCustomizationForm(Form customizationForm, ref Control parentControl) {
			CustomizationFormShowingEventHandler handler = (CustomizationFormShowingEventHandler)this.Events[showingCustomizationForm];
			if(handler != null) {
				CustomizationFormShowingEventArgs e = new CustomizationFormShowingEventArgs(customizationForm, parentControl);
				handler(this, e);
				parentControl = e.ParentControl;
				return e.Cancel;
			} else
				return false;
		}
		protected virtual void RaiseShowCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[showCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgadeEventArgs e) {
			LayoutUpgadeEventHandler handler = (LayoutUpgadeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseFieldFilterChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldFilterChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field));
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field));
		}
		protected virtual void RaiseFieldExpandedInFieldsGroupChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldExpandedInFieldGroupChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field));
		}
		protected virtual void RaiseFieldWidthChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldWidthChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field));
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridField field, object value) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(field, value);
				handler(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(value);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(fieldCellViewInfo);
				handler(this, e);
				return e.DisplayText;
			} else
				return fieldCellViewInfo.Text;
		}
		protected virtual object RaiseCustomGroupInterval(PivotGridField field, object value) {
			PivotCustomGroupIntervalEventHandler handler = (PivotCustomGroupIntervalEventHandler)this.Events[customGroupInterval];
			if(handler != null) {
				PivotCustomGroupIntervalEventArgs e = new PivotCustomGroupIntervalEventArgs(field, value);
				handler(this, e);
				return e.GroupValue;
			} else
				return value;
		}
		protected virtual int RaiseFieldValueImageIndex(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldImageIndexEventHandler handler = (PivotFieldImageIndexEventHandler)this.Events[fieldValueImageIndex];
			if(handler != null) {
				PivotFieldImageIndexEventArgs e = new PivotFieldImageIndexEventArgs(fieldCellViewInfo);
				handler(this, e);
				return e.ImageIndex;
			} else
				return -1;
		}
		protected virtual string RaiseCellDisplayText(PivotCellViewInfo cellViewInfo) {
			PivotCellDisplayTextEventHandler handler = (PivotCellDisplayTextEventHandler)this.Events[customCellDisplayText];
			if(handler != null) {
				PivotCellDisplayTextEventArgs e = new PivotCellDisplayTextEventArgs(cellViewInfo);
				handler(this, e);
				return e.DisplayText;
			} else
				return cellViewInfo.Text;
		}
		protected virtual void RaiseCellDoubleClick(PivotCellViewInfo cellViewInfo) {
			PivotCellEventHandler handler = (PivotCellEventHandler)this.Events[cellDoubleClick];
			if(handler != null) handler(this, new PivotCellEventArgs(cellViewInfo));
		}
		protected virtual void RaiseCellClick(PivotCellViewInfo cellViewInfo) {
			PivotCellEventHandler handler = (PivotCellEventHandler)this.Events[cellClick];
			if(handler != null) handler(this, new PivotCellEventArgs(cellViewInfo));
		}
		protected virtual void RaiseCellSelectionChanged() {
			EventHandler handler = (EventHandler)this.Events[cellSelectionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseFocusedCellChanged() {
			EventHandler handler = (EventHandler)this.Events[focusedCellChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseCustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {
			PivotCustomDrawCellEventHandler handler = (PivotCustomDrawCellEventHandler)this.Events[customDrawCell];
			if(handler != null) {
				PivotCustomDrawCellEventArgs e = new PivotCustomDrawCellEventArgs(cellViewInfo, appearance, paintArgs);
				handler(this, e);
				appearance = e.Appearance;
				return e.Handled;
			} else
				return false;
		}
		protected virtual void RaiseCustomAppearance(ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {
			PivotCustomAppearanceEventHandler handler = (PivotCustomAppearanceEventHandler)this.Events[customAppearance];
			if(handler != null) {
				PivotCustomAppearanceEventArgs e = new PivotCustomAppearanceEventArgs(cellViewInfo, appearance);
				handler(this, e);
				appearance = e.Appearance;
			}
		}
		protected virtual bool RaiseCustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter) {
			PivotCustomDrawFieldHeaderEventHandler handler = (PivotCustomDrawFieldHeaderEventHandler)this.Events[customDrawFieldHeader];
			if(handler != null) {
				PivotCustomDrawFieldHeaderEventArgs e = new PivotCustomDrawFieldHeaderEventArgs(headerViewInfo, paintArgs, painter);
				handler(this, e);
				return e.Handled;
			} else
				return false;
		}
		protected virtual bool RaiseCustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, HeaderObjectPainter painter) {
			PivotCustomDrawFieldValueEventHandler handler = (PivotCustomDrawFieldValueEventHandler)this.Events[customDrawFieldValue];
			if(handler != null) {
				PivotCustomDrawFieldValueEventArgs e = new PivotCustomDrawFieldValueEventArgs(fieldCellViewInfo, info, paintArgs, painter);
				handler(this, e);
				return e.Handled;
			} else 
				return false;
		}
		protected virtual int RaiseCustomRowHeight(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int height) {
			EventHandler<PivotCustomRowHeightEventArgs> handler = (EventHandler<PivotCustomRowHeightEventArgs>)this.Events[customRowHeight];
			if(handler != null) {
				PivotCustomRowHeightEventArgs e = new PivotCustomRowHeightEventArgs(fieldCellViewInfo, height);
				handler(this, e);
				return e.RowHeight;
			} else
				return height;
		}
		protected virtual int RaiseCustomColumnWidth(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int width) {
			EventHandler<PivotCustomColumnWidthEventArgs> handler = (EventHandler<PivotCustomColumnWidthEventArgs>)this.Events[customColumnWidth];
			if(handler != null) {
				PivotCustomColumnWidthEventArgs e = new PivotCustomColumnWidthEventArgs(fieldCellViewInfo, width);
				handler(this, e);
				return e.ColumnWidth;
			} else
				return width;
		}
		protected virtual bool RaiseCustomDrawFieldHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			PivotCustomDrawHeaderAreaEventHandler handler = (PivotCustomDrawHeaderAreaEventHandler)this.Events[customDrawFieldHeaderArea];
			if(handler != null) {
				PivotCustomDrawHeaderAreaEventArgs e = new PivotCustomDrawHeaderAreaEventArgs(headersViewInfo, paintArgs, bounds);
				handler(this, e);
				return e.Handled;
			} else
				return false;
		}
		protected virtual bool RaiseCustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			PivotCustomDrawEventHandler handler = (PivotCustomDrawEventHandler)this.Events[customDrawEmptyArea];
			if(handler != null) {
				PivotCustomDrawEventArgs e = new PivotCustomDrawEventArgs(appearanceOwner, paintArgs, bounds);
				handler(this, e);
				return e.Handled;
			} else
				return false;
		}
		protected virtual void RaiseShowMenu(PivotGridMenuEventArgs e) {
			PivotGridMenuEventHandler handler = (PivotGridMenuEventHandler)this.Events[showMenu];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMenuItemClick(PivotGridMenuItemClickEventArgs e) {
			PivotGridMenuItemClickEventHandler handler = (PivotGridMenuItemClickEventHandler)this.Events[menuItemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseFieldValueCollapsed(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueCollapsed];
			if(handler != null) 
				handler(this, new PivotFieldValueEventArgs(fieldCellViewInfo));
		}
		protected virtual void RaiseFieldValueExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueExpanded];
			if(handler != null) 
				handler(this, new PivotFieldValueEventArgs(fieldCellViewInfo));
		}
		protected virtual void RaiseFieldValueNotExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueNotExpanded];
			if(handler != null) {
				PivotFieldValueEventArgs e = new PivotFieldValueEventArgs(fieldCellViewInfo);
				handler(this, e);
			}
		}
		protected virtual bool RaiseFieldValueCollapsing(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldValueCancelEventHandler handler = (PivotFieldValueCancelEventHandler)this.Events[fieldValueCollapsing];
			return RaiseFieldValueCollapsingExpandingCore(fieldCellViewInfo, handler);
		}
		protected virtual bool RaiseFieldValueExpanding(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			PivotFieldValueCancelEventHandler handler = (PivotFieldValueCancelEventHandler)this.Events[fieldValueExpanding];
			return RaiseFieldValueCollapsingExpandingCore(fieldCellViewInfo, handler);
		}
		protected bool RaiseFieldValueCollapsingExpandingCore(PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotFieldValueCancelEventHandler handler) {
			if(handler != null) {
				PivotFieldValueCancelEventArgs e = new PivotFieldValueCancelEventArgs(fieldCellViewInfo);
				handler(this, e);
				return !e.Cancel;
			} else
				return true;
		}
		protected virtual void RaisePrefilterCriteriaChanged() {
			EventHandler handler = (EventHandler)this.Events[prefilterCriteriaChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseOLAPQueryTimeout() {
			EventHandler handler = (EventHandler)this.Events[olapQueryTimeout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEditValueChanged(PivotCellViewInfo cellInfo, BaseEdit edit) {
			EditValueChangedEventHandler handler = (EditValueChangedEventHandler)this.Events[editValueChanged];
			if(handler != null) {
				EditValueChangedEventArgs e = new EditValueChangedEventArgs(cellInfo, edit);
				handler(this, e);
			}
		}
		protected virtual void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BaseContainerValidateEditorEventHandler handler = (BaseContainerValidateEditorEventHandler)this.Events[validatingEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[invalidValueException];
			if(handler != null) handler(this, e);
		}
		protected virtual object RaiseCustomEditValue(object value, PivotCellViewInfo cellViewInfo) {
			CustomEditValueEventHandler handler = (CustomEditValueEventHandler)this.Events[customEditValue];
			if(handler != null) {
				CustomEditValueEventArgs e = new CustomEditValueEventArgs(value, cellViewInfo);
				handler(this, e);
				return e.Value;
			} else
				return value;
		}
		protected virtual void RaiseShowingEditor(CancelPivotCellEditEventArgs e) {
			EventHandler<CancelPivotCellEditEventArgs> handler = (EventHandler<CancelPivotCellEditEventArgs>)this.Events[showingEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseShownEditor(PivotCellEditEventArgs e) {
			EventHandler<PivotCellEditEventArgs> handler = (EventHandler<PivotCellEditEventArgs>)this.Events[shownEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseHiddenEditor(PivotCellEditEventArgs e) {
			EventHandler<PivotCellEditEventArgs> handler = (EventHandler<PivotCellEditEventArgs>)this.Events[hiddenEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual RepositoryItem RaiseCustomCellEdit(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem) {
			EventHandler<PivotCustomCellEditEventArgs> handler = (EventHandler<PivotCustomCellEditEventArgs>)this.Events[customCellEdit];
			if(handler != null) {
				PivotCustomCellEditEventArgs e = new PivotCustomCellEditEventArgs(cellViewInfo, repositoryItem);
				handler(this, e);
				return e.RepositoryItem;
			}
			return repositoryItem;
		}
		protected virtual RepositoryItem RaiseCustomCellEditForEditing(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem) {
			EventHandler<PivotCustomCellEditEventArgs> handler = (EventHandler<PivotCustomCellEditEventArgs>)this.Events[customCellEditForEditing];
			if(handler != null) {
				PivotCustomCellEditEventArgs e = new PivotCustomCellEditEventArgs(cellViewInfo, repositoryItem);
				handler(this, e);
				return e.RepositoryItem;
			}
			return repositoryItem;
		}
		protected virtual void RaiseCustomExportHeader(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo) {
			EventHandler<CustomExportHeaderEventArgs> handler = (EventHandler<CustomExportHeaderEventArgs>)this.Events[customExportHeader];
			if(handler != null) handler(this, new CustomExportHeaderEventArgs(brick, headerViewInfo));
		}
		protected virtual void RaiseCustomExportFieldValue(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase fieldViewInfo) {
			EventHandler<CustomExportFieldValueEventArgs> handler = (EventHandler<CustomExportFieldValueEventArgs>)this.Events[customExportFieldValue];
			if(handler != null) handler(this, new CustomExportFieldValueEventArgs(brick, fieldViewInfo));
		}
		protected virtual void RaiseCustomExportCell(IVisualBrick brick, PivotCellViewInfo cellViewInfo) {
			EventHandler<CustomExportCellEventArgs> handler = (EventHandler<CustomExportCellEventArgs>)this.Events[customExportCell];
			if(handler != null) handler(this, new CustomExportCellEventArgs(brick, cellViewInfo));
		}
		protected virtual void RaiseExportStarted() {
			EventHandler handler = (EventHandler)this.Events[exportStarted];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseExportFinished() {
			EventHandler handler = (EventHandler)this.Events[exportFinished];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			if(dataSource is DataSet) return true;
			if(dataSource is System.Data.DataView) {
				System.Data.DataView dv = dataSource as System.Data.DataView;
				if(dv.Table == null) return false;
				return true;
			}
			if(dataSource is System.Data.DataTable) return true;
			return false;
		}
		bool isDataSourceActive;
		protected bool IsDataSourceActive { get { return isDataSourceActive; } }
		void ActivateDataSource() {
			if(IsLoading || fData == null) return;
			if(string.IsNullOrEmpty(olapConnectionString)) {
				if(DataSource is DataView) {
					if((DataSource as DataView).Table == null) return;
				}
				fData.ListSource = MasterDetailHelper.GetDataSource(BindingContext, DataSource, DataMember);	
			}
			fData.OLAPConnectionString = olapConnectionString;
			isDataSourceActive = true;
		}
		void EsureDataSourceIsActive() {
			if(!IsDataSourceActive) ActivateDataSource();
		}
		public string[] GetFieldList() {
			return Data.GetFieldList();
		}
		public List<string> GetOLAPKPIList() {
			return Data.GetOLAPKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			return Data.GetOLAPKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			return Data.GetOLAPKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			return Data.GetOLAPKPIServerGraphic(kpiName, kpiType);
		}
		public Bitmap GetKPIBitmap(PivotKPIGraphic graphic, int state) {
			return Data.GetKPIBitmap(graphic, state);
		}
		bool IComponentLoading.IsLoading { get { return IsLoading; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		protected virtual void OnLookAndFeel_StyleChanged(object sender, EventArgs e) {
			fData.LookAndFeelChanged();
		}
		protected internal PivotGridViewInfoData Data {
			get {
				EsureDataSourceIsActive();
				return fData;
			}
		}
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return this.Data; }
		}
		protected PivotGridViewInfo ViewInfo { get { return Data != null ? (PivotGridViewInfo)Data.ViewInfo : null; } }
		protected PivotVisualItemsBase VisualItems { get { return Data.VisualItems; } }
		public void EnsureViewInfoIsCalculated() {
			if(ViewInfo != null && !ViewInfo.IsReady) 
				ViewInfo.EnsureIsCalculated();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point LeftTopCoord {
			get { return ViewInfo.LeftTopCoord; }
			set {
				InvalidateScrollBars();
				CloseEditor();
				ViewInfo.LeftTopCoord = value;
			}
		}
		protected override Color ScrollBarsGlyphColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control); } }
		protected override BaseViewInfo RootViewInfo { get { return Data != null ? Data.ViewInfo : null; } }
		public virtual void ForceInitialize() {
			OnLoaded();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			EditorContainer.OnLoaded();
			ActivateDataSource();
			if(ListSource == null)
				RefreshData();
		}
		protected override void OnValidating(CancelEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				CloseEditor();
			} catch(HideException) {
				e.Cancel = true;
			} finally {
				EditorHelper.EndAllowHideException();
			}
			base.OnValidating(e);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Down || key == Keys.Up || key == Keys.Left || key == Keys.Right) return false;
			return base.ProcessDialogKey(keyData);
		}
		Point clickedCellCoord = PivotCellViewInfo.EmptyCoord;
		protected Point ClickedCellCoord { get { return clickedCellCoord; } set { clickedCellCoord = value; } }		
		protected override void OnMouseDown(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(ActiveEditor != null && !ActiveEditor.Bounds.Contains(e.Location))
					CloseEditor();
				Point clickedCell = ViewInfo.CellsArea.GetCellCoordAt(e.Location),
					focusedCell = FocusedCell;
				ClickedCellCoord = clickedCell;
				base.OnMouseDown(e);
				if(e.Button == MouseButtons.Left) {					
					EditorShowMode showMode = Data.OptionsBehavior.GetEditorShowMode();
					if(showMode == EditorShowMode.MouseDown ||
						(showMode == EditorShowMode.MouseDownFocused && clickedCell == focusedCell)) {
						ShowEditor(clickedCell);
						if(ActiveEditor != null) {
							ViewInfo.MouseUp(e);	
							ActiveEditor.SendMouse(ActiveEditor.PointToClient(Control.MousePosition), Control.MouseButtons);
						}
					}
				}				
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseUp(e);
				if(e.Button == MouseButtons.Left && ActiveEditor == null
						&& ClickedCellCoord == ViewInfo.CellsArea.GetCellCoordAt(e.Location)
						&& Data.OptionsBehavior.GetEditorShowMode() == EditorShowMode.MouseUp) {
					ShowEditor(ClickedCellCoord);
				}
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(Data.OptionsBehavior.GetEditorShowMode() == EditorShowMode.Click
					&& ViewInfo.CellsArea.GetCellCoordAt(e.Location) == FocusedCell) {
					ShowEditor(FocusedCell);
				}
				base.OnMouseClick(e);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseDoubleClick(e);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseMove(e);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyDown(e);
				ProcessKeyDown(e);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyPress(e);
				ProcessKeyPress(e);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected virtual void ProcessKeyDown(KeyEventArgs e) {
			if(ActiveEditor != null || !ViewInfo.IsReady) return;
			RepositoryItem repositoryItem = GetCellEditForEditing(GetCellInfo(FocusedCell));
			if(repositoryItem == null) return;
			if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2) {
				ShowEditor();
				return;
			}
			if(repositoryItem.IsActivateKey(e.KeyCode)) {
				ShowEditorByKey(e);
				return;
			}
		}
		static char[] keyPress = new char[] { (char)27, (char)9, (char)8, (char)3 };
		protected virtual void ProcessKeyPress(KeyPressEventArgs e) {
			if(ActiveEditor != null || !ViewInfo.IsReady) return;
			if(Array.IndexOf(keyPress, e.KeyChar) == -1) {
				if(e.KeyChar == ' ' && Control.ModifierKeys == Keys.Control) return;
				if(e.KeyChar == 1 && Control.ModifierKeys == Keys.Control) return;
				ShowEditorByKeyPress(e);
			}
		}
		protected virtual void ShowEditorByKey(KeyEventArgs e) {
			ShowEditor();
			if(ActiveEditor != null && e.KeyCode != Keys.Enter) {
				ActiveEditor.SendKey(e);
			}
		}
		protected virtual void ShowEditorByKeyPress(KeyPressEventArgs e) {
			ShowEditor();
			if(ActiveEditor != null && e.KeyChar != 13 && e.KeyChar != 9) {
				ActiveEditor.SendKey(lastKeyMessage, e);
			}
		}
		internal object lastKeyMessage = null;
		protected override void WndProc(ref Message m) {
			lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			if(DevExpress.XtraEditors.Senders.BaseSender.RequireShowEditor(ref m)) ShowEditor();
			base.WndProc(ref m);
		}
		protected override void OnMouseWheel(MouseEventArgs ee) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseWheel(ee);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected virtual void UpdateCursor() {
			if(ViewInfo.State == PivotGridViewInfoState.FieldResizing)
				base.Cursor = Cursors.SizeWE;
			else base.Cursor = Cursors.Default;
		}
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		protected override void OnSizeChanged(EventArgs e) {
			if(IsHandleCreated && ViewInfo != null)
				ViewInfo.ClientSizeChanged();
			base.OnSizeChanged(e);
		}
		[Category("Appearance"), Description("Indicates whether a field header is being dragged by a user."), Browsable(false)]
		public bool IsDragging { get { return ViewInfo.IsDragging; } }
		protected override bool IsHScrollBarVisible { get { return ViewInfo.IsHScrollBarVisible; } }
		protected override bool IsVScrollBarVisible { get { return ViewInfo.IsVScrollBarVisible; } }
		protected override ScrollArgs HScrollBarInfo { get { return ViewInfo.HScrollBarInfo; } }
		protected override ScrollArgs VScrollBarInfo { get { return ViewInfo.VScrollBarInfo; } }
		protected override void ScrollBarsValueChanged(Point newValue) {
			try {
				EditorHelper.BeginAllowHideException();
				LeftTopCoord = newValue;
			} catch(HideException) {
				UpdateScrollBars();
			} finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void InternalRefresh() {
			Data.InvalidateViewInfo();
			base.InternalRefresh();
		}
		protected virtual PivotGridPrinter CreatePrinter() {
			PivotGridPrinter printer = new PivotGridPrinter(this);
			printer.Owner = this;
			return printer;
		}
		[Category("Appearance"), Description("Gets or sets the tooltip controller component that controls the appearance, position and the content of the hints displayed by the XtraPivotGrid control."), DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				} else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		#region IToolTipControlClient
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ViewInfo.GetToolTipObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		#endregion
		#region IPrintable
		bool IPrintable.CreatesIntersectedBricks {
			get { return false; }
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(printer != null) {
				printer.Release();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetCommandsVisibility(IPrintingSystem ps) {
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			SetCommandsVisibility(ps);
			EsureDataSourceIsActive();
			printer.Initialize(ps, link, ViewInfo.Data);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			printer.CreateArea(areaName, graph);
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return PrintHelper.IsPrintingAvailable; } }
		public void ShowPrintPreview() { PrintHelper.ShowPreview(this); }
		public void Print() { PrintHelper.Print(this); }
		protected DynamicPrintHelper PrintHelper {
			get {
				if(printHelper == null)
					printHelper = new DynamicPrintHelper();
				printHelper.PageSettings = OptionsPrint.PageSettings.ToPageSettings();
				printHelper.VerticalContentSplitting = OptionsPrint.VerticalContentSplitting;
				return printHelper;
			}
		}
		#endregion
		#region Export
		public void ExportToXls(string filePath) {
			ExportToXls(filePath, true);
		}
		public void ExportToXls(string filePath, bool useNativeFormat) {
			RaiseExportStarted();
			PrintHelper.ExportToXls(this, filePath, new XlsExportOptions(useNativeFormat));
			RaiseExportFinished();
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToXls(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToXls(Stream stream) {
			ExportToXls(stream, true);
		}
		public void ExportToXls(Stream stream, bool useNativeFormat) {
			RaiseExportStarted();
			PrintHelper.ExportToXls(this, stream, new XlsExportOptions(useNativeFormat));
			RaiseExportFinished();
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToXls(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToRtf(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToRtf(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToRtf(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToRtf(this, stream);
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, filePath, new HtmlExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, filePath, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, stream);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, stream, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToHtml(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, filePath, new MhtExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, filePath, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, stream, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToMht(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, stream);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToImage(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToImage(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToImage(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToImage(this, stream);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToImage(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored.")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored.")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator, Encoding encoding) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored. Use ExportToCsv method instead.")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored. Use ExportToCsv method instead.")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator, Encoding encoding) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToCsv(this, stream);
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToCsv(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToCsv(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToCsv(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToCsv(this, filePath, options);
			RaiseExportFinished();
		}
		#endregion
		#region IPivotGridEventsImplementor
		void IPivotGridEventsImplementor.DataSourceChanged() {
			RaiseDataSourceChanged();
		}
		void IPivotGridEventsImplementor.BeginRefresh() {
			RaiseBeginRefresh();
		}
		void IPivotGridEventsImplementor.EndRefresh() {
			RaiseEndRefresh();
		}
		void IPivotGridEventsImplementor.LayoutChanged() {
			InvalidateChartData();
			RaiseGridLayout();
		}
		bool IPivotGridEventsImplementor.FieldAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex) {			
			return RaiseAreaChanging(field, newArea, newAreaIndex);
		}
		object IPivotGridEventsImplementor.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {			
			return RaiseCustomUnboundColumnData(field, listSourceRowIndex);
		}
		void IPivotGridEventsImplementor.CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {			
			RaiseCustomSummary(field, customSummaryInfo);
		}
		bool IPivotGridEventsImplementor.ShowingCustomizationForm(Form customizationForm, ref Control parentControl) {			
			return RaiseShowingCustomizationForm(customizationForm, ref parentControl);			
		}
		void IPivotGridEventsImplementor.ShowCustomizationForm() {
			RaiseShowCustomizationForm();
		}
		void IPivotGridEventsImplementor.HideCustomizationForm() {
			RaiseHideCustomizationForm();
		}
		void IPivotGridEventsImplementor.OnPopupShowMenu(PivotGridMenuEventArgs e) {
			RaiseShowMenu(e);
		}
		void IPivotGridEventsImplementor.OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e) {
			RaiseMenuItemClick(e);
		}
		void IPivotGridEventsImplementor.BeforeLayoutLoad(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
		}
		void IPivotGridEventsImplementor.LayoutUpgrade(LayoutUpgadeEventArgs e) {
			RaiseLayoutUpgrade(e);
		}
		void IPivotGridEventsImplementor.FieldFilterChanged(PivotGridField field) {
			RaiseFieldFilterChanged(field);
		}
		void IPivotGridEventsImplementor.FieldAreaChanged(PivotGridField field) {
			RaiseFieldAreaChanged(field);
		}
		void IPivotGridEventsImplementor.FieldExpandedInFieldsGroupChanged(PivotGridField field) {
			RaiseFieldExpandedInFieldsGroupChanged(field);
		}
		void IPivotGridEventsImplementor.FieldWidthChanged(PivotGridField field) {
			RaiseFieldWidthChanged(field);
		}
		string IPivotGridEventsImplementor.FieldValueDisplayText(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			return RaiseFieldValueDisplayText(fieldCellViewInfo);
		}
		bool IPivotGridEventsImplementor.BeforeFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(fieldCellViewInfo.IsCollapsed)
				return RaiseFieldValueExpanding(fieldCellViewInfo);
			else 
				return RaiseFieldValueCollapsing(fieldCellViewInfo);
		}
		void IPivotGridEventsImplementor.AfterFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(fieldCellViewInfo.IsCollapsed)
				RaiseFieldValueExpanded(fieldCellViewInfo);
			else
				RaiseFieldValueCollapsed(fieldCellViewInfo);
		}
		void IPivotGridEventsImplementor.AfterFieldValueChangeNotExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			RaiseFieldValueNotExpanded(fieldCellViewInfo);
		}
		string IPivotGridEventsImplementor.FieldValueDisplayText(PivotGridField field, object value) {
			return RaiseFieldValueDisplayText(field, value);
		}
		int IPivotGridEventsImplementor.GetCustomRowHeight(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int height) {
			return RaiseCustomRowHeight(fieldCellViewInfo, height);
		}
		int IPivotGridEventsImplementor.GetCustomColumnWidth(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int width) {
			return RaiseCustomColumnWidth(fieldCellViewInfo, width);
		}
		object IPivotGridEventsImplementor.CustomGroupInterval(PivotGridField field, object value) {
			return RaiseCustomGroupInterval(field, value);
		}
		int IPivotGridEventsImplementor.FieldValueImageIndex(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			return RaiseFieldValueImageIndex(fieldCellViewInfo);
		}
		string IPivotGridEventsImplementor.CustomCellDisplayText(PivotCellViewInfo cellViewInfo) {			
			return RaiseCellDisplayText(cellViewInfo);
		}
		int IPivotGridEventsImplementor.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {			
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);			
		}
		void IPivotGridEventsImplementor.CellDoubleClick(PivotCellViewInfo cellViewInfo) {
			RaiseCellDoubleClick(cellViewInfo);
		}
		void IPivotGridEventsImplementor.CellClick(PivotCellViewInfo cellViewInfo) {
			RaiseCellClick(cellViewInfo);
		}
		void IPivotGridEventsImplementor.CellSelectionChanged() {
			if(OptionsChartDataSource.SelectionOnly) InvalidateChartData();
			RaiseCellSelectionChanged();
		}
		void IPivotGridEventsImplementor.FocusedCellChanged() {
			if(OptionsChartDataSource.SelectionOnly) InvalidateChartData();
			RaiseFocusedCellChanged();
		}
		bool IPivotGridEventsImplementor.CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds) {			
			return RaiseCustomDrawFieldHeaderArea(headersViewInfo, paintArgs, bounds);
		}
		bool IPivotGridEventsImplementor.CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			return RaiseCustomDrawEmptyArea(appearanceOwner, paintArgs, bounds);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter) {
			return RaiseCustomDrawFieldHeader(headerViewInfo, paintArgs, painter);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, HeaderObjectPainter painter) {
			return RaiseCustomDrawFieldValue(paintArgs, fieldCellViewInfo, info, painter);
		}
		bool IPivotGridEventsImplementor.CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {
			return RaiseCustomDrawCell(paintArgs, ref appearance, cellViewInfo);
		}
		void IPivotGridEventsImplementor.CustomAppearance(ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {			
			RaiseCustomAppearance(ref appearance, cellViewInfo);
		}
		void IPivotGridEventsImplementor.PrefilterCriteriaChanged() {
			RaisePrefilterCriteriaChanged();
		}
		void IPivotGridEventsImplementor.OLAPQueryTimeout() {
			RaiseOLAPQueryTimeout();
		}
		object IPivotGridEventsImplementor.CustomEditValue(object value, PivotCellViewInfo cellViewInfo) {
			return RaiseCustomEditValue(value, cellViewInfo);
		}
		RepositoryItem IPivotGridEventsImplementor.GetCellEdit(PivotCellViewInfo cellViewInfo) {
			return GetCellEdit(cellViewInfo);
		}
		#endregion
		#region IPivotGridDataOwner
		void IPivotGridDataOwner.FireChanged(object[] changedObjects) {
			if(!DesignMode || IsLoading) return;
			if(changedObjects == null) {
				changedObjects = new object[] { this };
			}
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				foreach(object obj in changedObjects) {
					Component component = obj as Component;
					if(component == null) continue;
					srv.OnComponentChanged(component, null, null, null);
				}
				srv.OnComponentChanged(this, null, null, null);
			}
			InvalidateChartData();
		}
		#endregion
		#region ChartDataSource
		List<Point> datasourceCells;
		bool isBuildingDataSource;
		PropertyDescriptorCollection chartProps;
		internal bool IsChartDataValid { get { return datasourceCells != null && chartProps != null; } }
		protected virtual void OnOptionsChartDataSourceChanged(object sender, EventArgs e) {
			InvalidateChartData();
		}
		internal void InvalidateChartData() { InvalidateChartData(false); }
		internal void InvalidateChartData(bool force) {
			if(!force && (isBuildingDataSource || !IsChartDataValid || IsUpdating)) return;
			datasourceCells = null;
			chartProps = null;
			RaiseListChanged();
		}
		internal virtual void EnsureChartData() {
			if(!IsChartDataValid) BuildChartData();
		}
		internal virtual void BuildChartData() {
			isBuildingDataSource = true;
			ViewInfo.EnsureIsCalculated();
			CreateChartDataSourceCells();
			CreatePropertyDescriptors();
			isBuildingDataSource = false;
		}
		void CreatePropertyDescriptors() {
			if(chartProps == null) chartProps = new PropertyDescriptorCollection(null);
			else chartProps.Clear();
			chartProps.Add(new PivotChartDescriptor(Data, PivotChartDescriptorType.ColumnField));
			chartProps.Add(new PivotChartDescriptor(Data, PivotChartDescriptorType.RowField));
			chartProps.Add(new PivotChartDescriptor(Data, PivotChartDescriptorType.CellValue));
		}
		void CreateChartDataSourceCells() {
			if(IsUpdating || Data.IsLoading)
				datasourceCells = new List<Point>();
			else {
				datasourceCells = OptionsChartDataSource.SelectionOnly ? SelectedCells : AllCells;
				StripTotalsFromChartDataSource();
			}
		}		
		void StripTotalsFromChartDataSource() {
			if(!OptionsChartDataSource.ShouldRemoveTotals) return;
			for(int i = datasourceCells.Count - 1; i >= 0; i--) {
				if(OptionsChartDataSource.ShouldRemoveItem(true, GetFieldValueType(true, datasourceCells[i].X)) ||
					OptionsChartDataSource.ShouldRemoveItem(false, GetFieldValueType(false, datasourceCells[i].Y))) {
					datasourceCells.RemoveAt(i);
				}
			}
		}
		internal virtual void RaiseListChanged() {
			if(listChanged != null) {
				listChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0));
				listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		List<Point> SelectedCells {
			get {
				List<Point> result = new List<Point>(ViewInfo.SelectedCells.Count + 1);
				ViewInfo.SelectedCells.CopyTo(result);
				if(!result.Contains(ViewInfo.FocusedCell))
					result.Add(ViewInfo.FocusedCell);
				result.Sort(new PointsComparer());
				return result;
			}
		}
		List<Point> AllCells {
			get {
				List<Point> result = new List<Point>();
				for(int j = 0; j < Cells.RowCount; j++)
					for(int i = 0; i < Cells.ColumnCount; i++)
						result.Add(new Point(i, j));
				return result;
			}
		}
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			EnsureChartData();
			return listAccessors != null && listAccessors.Length > 0 ? new PropertyDescriptorCollection(null) : chartProps;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return this.Name;
		}
		#endregion
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new Exception("AddIndex : The method or operation is not implemented.");
		}
		object IBindingList.AddNew() {
			throw new Exception("AddNew : The method or operation is not implemented.");
		}
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new Exception("ApplySort : The method or operation is not implemented.");
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new Exception("Find : The method or operation is not implemented.");
		}
		bool IBindingList.IsSorted { get { return false; } }
		ListChangedEventHandler listChanged;
		event ListChangedEventHandler IBindingList.ListChanged {
			add { this.listChanged += value; }
			remove { this.listChanged -= value; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new Exception("RemoveIndex : The method or operation is not implemented.");
		}
		void IBindingList.RemoveSort() {
			throw new Exception("RemoveSort : The method or operation is not implemented.");
		}
		ListSortDirection IBindingList.SortDirection { get { return ListSortDirection.Ascending; } }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new Exception("Add : The method or operation is not implemented.");
		}
		void IList.Clear() {
			throw new Exception("Clear : The method or operation is not implemented.");
		}
		bool IList.Contains(object value) {
			throw new Exception("Contains : The method or operation is not implemented.");
		}
		int IList.IndexOf(object value) {
			throw new Exception("IndexOf : The method or operation is not implemented.");
		}
		void IList.Insert(int index, object value) {
			throw new Exception("Insert : The method or operation is not implemented.");
		}
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void IList.Remove(object value) {
			throw new Exception("Remove : The method or operation is not implemented.");
		}
		void IList.RemoveAt(int index) {
			throw new Exception("RemoveAt : The method or operation is not implemented.");
		}
		object IList.this[int index] {
			get {
				EnsureChartData();
				if(index < 0 || index >= (this as ICollection).Count)
					return null;
				int[] row = new int[2] { datasourceCells[index].X, datasourceCells[index].Y };
				return row;
			}
			set {
				throw new Exception("this[].set : The method or operation is not implemented.");
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			for(int i = 0; i < (this as ICollection).Count; i++) {
				if(index + i >= array.Length) break;
				array.SetValue((this as IList)[i], index + i);
			}
		}
		int ICollection.Count {
			get {
				EnsureChartData();
				return datasourceCells.Count;
			}
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			EnsureChartData();
			return new ChartDataEnumerator(this); 
		}
		#endregion
		#endregion
		#region Editors
		PivotGridEditorContainer editorContainer;
		PivotGridEditorContainerHelper editorHelper;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridEditorContainer EditorContainer { get { return editorContainer; } }
		PivotGridEditorContainerHelper EditorHelper { get { return editorHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RepositoryItemCollection RepositoryItems { get { return EditorHelper.RepositoryItems; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseEdit ActiveEditor { get { return EditorHelper == null ? null : EditorHelper.ActiveEditor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object EditValue {
			get { return ActiveEditor == null ? null : ActiveEditor.EditValue; }
			set { if(ActiveEditor != null) ActiveEditor.EditValue = value; }
		}
		public void ShowEditor() {
			ShowEditor(FocusedCell);
		}
		public void ShowEditor(Point location) {
			CloseEditor();
			BaseEdit edit = ActivateEditor(location);
			if(edit != null) {
				RaiseShownEditor(new PivotCellEditEventArgs(GetCellInfo(location), edit));
			}
		}
		bool CanShowEditor(PivotCellViewInfo cellInfo, RepositoryItem repositoryItem) {
			if(IsDesignModeCore) 
				return false;
			CancelPivotCellEditEventArgs e = new CancelPivotCellEditEventArgs(cellInfo, repositoryItem);
			RaiseShowingEditor(e);
			if(e.Cancel) return false;
			return true;
		}
		protected internal Point FocusedCell { get { return ViewInfo != null ? ViewInfo.FocusedCell : Point.Empty; } }
		protected internal BaseEdit ActivateEditor(Point cellCoord) {
			PivotCellViewInfo cellInfo = GetCellInfo(cellCoord);
			if(cellInfo != null)
				return ActivateEditor(cellInfo);
			return null;
		}
		protected BaseEdit ActivateEditor(PivotCellViewInfo cellInfo) {
			RepositoryItem rItem = GetCellEditForEditing(cellInfo);
			if(rItem == null || !GetAllowEdit(cellInfo) || !CanShowEditor(cellInfo, rItem))
				return null;
			return UpdateEditor(rItem, CreateUpdateEditorInfoArgs(cellInfo));			
		}
		protected virtual bool GetAllowEdit(PivotCellViewInfo cellInfo) {
			return cellInfo.DataField != null ? cellInfo.DataField.CanEdit : false;
		}
		protected virtual RepositoryItem GetCellEditForEditing(PivotCellViewInfo cellInfo) {
			return RaiseCustomCellEditForEditing(cellInfo, GetCellEdit(cellInfo));
		}
		protected virtual RepositoryItem GetCellEdit(PivotCellViewInfo cellInfo) {
			RepositoryItem repositoryItem = cellInfo.DataField != null ? cellInfo.DataField.FieldEdit : null;
			return RaiseCustomCellEdit(cellInfo, repositoryItem);
		}
		protected UpdateEditorInfoArgs CreateUpdateEditorInfoArgs(PivotCellViewInfo cellInfo) {
			Rectangle bounds = new Rectangle(cellInfo.PaintBounds.X + 1, cellInfo.PaintBounds.Y + 1, cellInfo.PaintBounds.Width - 3, cellInfo.PaintBounds.Height - 3);
			return new UpdateEditorInfoArgs(Data.GetReadOnly(cellInfo.DataField), bounds, cellInfo.Appearance, cellInfo.Value, LookAndFeel);
		}
		protected BaseEdit UpdateEditor(RepositoryItem rItem, UpdateEditorInfoArgs updateArgs) {
			BaseEdit edit = EditorHelper.UpdateEditor(rItem, updateArgs);
			EditorHelper.ShowEditor(edit, this);
#if	DEBUGTEST
			edit.Properties.Appearance.BackColor = Color.Pink;
#endif
			return edit;
		}
		public void CloseEditor() {
			if(ActiveEditor == null) return;
			PostEditor();
			HideEditor();
		}
		public void HideEditor() {
			if(ActiveEditor == null) return;
			BaseEdit edit = ActiveEditor;
			EditorHelper.HideEditorCore(this, true);			
			RaiseHiddenEditor(new PivotCellEditEventArgs(GetCellInfo(FocusedCell), edit));
		}
		public bool PostEditor() {
			if(ActiveEditor == null) return false;
			if(!ValidateEditor()) return false;
			PivotCellViewInfo cellInfo = GetCellInfo(FocusedCell);
			if(ActiveEditor.IsModified || !object.Equals(cellInfo.Value, ActiveEditor.EditValue)) {				
				OnEditValueChanged(cellInfo, ActiveEditor);
				Data.DoRefresh();
			}
			return true;
		}
		public bool ValidateEditor() {
			if(ActiveEditor == null) return true;
			return EditorHelper.ValidateEditor(this);
		}
		void OnValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			RaiseValidatingEditor(e);
		}
		protected virtual void OnEditValueChanged(PivotCellViewInfo cellInfo, BaseEdit edit) {			
			RaiseEditValueChanged(cellInfo, edit);
		}
		protected internal PivotCellViewInfo GetCellInfo(Point coord) {
			return ViewInfo != null ? (PivotCellViewInfo)ViewInfo.CellsArea.CreateCellViewInfo(coord.X, coord.Y) : null;
		}
		public class PivotGridEditorContainer : EditorContainer {
			protected override EditorContainerHelper CreateHelper() {
				return new PivotGridEditorContainerHelper(this);
			}
			new internal protected PivotGridEditorContainerHelper EditorHelper { get { return (PivotGridEditorContainerHelper)base.EditorHelper; } }
			protected override void RaiseEditorKeyDown(KeyEventArgs e) {
				base.RaiseEditorKeyDown(e);
				if(e.Handled) return;
				if(EditorHelper.ActiveEditor != null) {
					BaseEdit be = (BaseEdit)EditorHelper.ActiveEditor;
					if(be.IsNeededKey(e))
						return;
				}
				try {
					EditorHelper.BeginAllowHideException();
					ProcessKeyDown(e);
				} catch(HideException) {
					e.Handled = true;
				} finally {
					EditorHelper.EndAllowHideException();
				}
			}
			void ProcessKeyDown(KeyEventArgs e) {
				switch(e.KeyCode) {
					case Keys.Enter:
						EditorHelper.Pivot.CloseEditor();
						break;
					case Keys.Escape:
						EditorHelper.Pivot.HideEditor();
						break;
				}
			}
			internal new void OnLoaded() {
				base.OnLoaded();
			}
		}
		public class PivotGridEditorContainerHelper : EditorContainerHelper {
			PivotGridControl pivot;
			public PivotGridEditorContainerHelper(EditorContainer owner)
				: base(owner) {
			}
			public override void Dispose() {
				base.Dispose();
				Pivot = null;
			}
			protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
				if(Pivot != null)
					Pivot.RaiseInvalidValueException(e);
			}
			protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs va) {
				if(Pivot != null)
					Pivot.RaiseValidatingEditor(va);
			}
			internal PivotGridControl Pivot {
				get { return pivot; }
				set {
					pivot = value;
					((PersistentRepository)InternalRepository).SetParentComponent(Pivot);
				}
			}
			public override ToolTipController RealToolTipController {
				get {
					return (Pivot.ToolTipController == null) ? ToolTipController.DefaultController : Pivot.ToolTipController;
				}
			}
			protected override IDXMenuManager MenuManager {
				get { return Pivot.MenuManager; }
			}
			protected override void OnRepositoryItemChanged(RepositoryItem item) {
				base.OnRepositoryItemChanged(item);
				Pivot.Data.InvalidateViewInfo();
			}
		}
		#endregion Editors
		#region IPivotGridPrinterOwner Members
		void IPivotGridPrinterOwner.CustomDrawHeader(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo) {
			RaiseCustomExportHeader(brick, headerViewInfo);
		}
		void IPivotGridPrinterOwner.CustomDrawFieldValue(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase fieldViewInfo) {
			RaiseCustomExportFieldValue(brick, fieldViewInfo);
		}
		void IPivotGridPrinterOwner.CustomDrawCell(IVisualBrick brick, PivotCellViewInfo cellViewInfo) {
			RaiseCustomExportCell(brick, cellViewInfo);
		}
		#endregion
	}
	public enum PivotChartDescriptorType { ColumnField, RowField, CellValue };
	class PivotChartDescriptor : PropertyDescriptor {
		PivotGridViewInfoData data;
		PivotChartDescriptorType type;
		protected PivotGridViewInfoData Data { get { return data; } }
		protected PivotChartDescriptorType Type { get { return type; } }
		protected PivotGridControl PivotGrid { get { return data.PivotGrid; } }
		public PivotChartDescriptor(PivotGridViewInfoData data, PivotChartDescriptorType type)
			: base(GetName(data.OptionsChartDataSource.ChartDataVertical, type), new Attribute[0] { }) {
			this.data = data;
			this.type = type;
		}
		static string GetName(bool vertical, PivotChartDescriptorType type) {
			switch(type) {
				case PivotChartDescriptorType.CellValue:
					return "Values";
				case PivotChartDescriptorType.ColumnField:
					return vertical ? "Series" : "Arguments";
				case PivotChartDescriptorType.RowField:
					return vertical ? "Arguments" : "Series";
			}
			throw new ArgumentException("Unknown type!");
		}
		public override object GetValue(object component) {
			if(!PivotGrid.IsChartDataValid) throw new Exception("PropertyDescriptor is invalid");
			int[] indexes = (int[])component;
			PivotFieldValueItem columnItem = Data.GetAreaViewInfo(true).GetLastLevelViewInfo(indexes[0]).Item,
				rowItem = Data.GetAreaViewInfo(false).GetLastLevelViewInfo(indexes[1]).Item;
			switch(Type) {
				case PivotChartDescriptorType.ColumnField:
					return Data.GetAreaViewInfo(true).GetChartText(indexes[0]);
				case PivotChartDescriptorType.RowField:
					return Data.GetAreaViewInfo(false).GetChartText(indexes[1]);
				case PivotChartDescriptorType.CellValue:
					object res = Data.GetCellValue(columnItem.VisibleIndex, rowItem.VisibleIndex, Data.OptionsDataField.Area == PivotDataArea.RowArea ? rowItem.DataIndex : columnItem.DataIndex);
					if(res == null)
						return DBNull.Value;
					else {
						try {
							return Convert.ChangeType(res, CellValueType);
						} catch {
							return DBNull.Value;
						}
					}
			}
			return null;
		}
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(int[]); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {
			get {
				switch(type) {
					case PivotChartDescriptorType.ColumnField:
						return typeof(string);
					case PivotChartDescriptorType.RowField:
						return typeof(string);
					case PivotChartDescriptorType.CellValue:
						return CellValueType;
				}
				return null;
			}
		}
		Type cellValueType;
		protected Type CellValueType {
			get {
				if(cellValueType == null)
					cellValueType = GetCellValueTypeCore();
				return cellValueType;
			}
		}
		Type GetCellValueTypeCore() {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(PivotArea.DataArea, false);
			Type res = null;
			for(int i = 0; i < fields.Count; i++) {
				Type fieldType = Data.GetFieldType(fields[i], false);
				if((res != null && res != fieldType) || fieldType == typeof(object) || fieldType == null)
					return typeof(decimal);
				res = fieldType;
			}
			return res != null ? res : typeof(decimal);
		}
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public interface IPivotGridViewInfoDataOwner {
		PivotGridViewInfoData DataViewInfo { get; }
	}
}
