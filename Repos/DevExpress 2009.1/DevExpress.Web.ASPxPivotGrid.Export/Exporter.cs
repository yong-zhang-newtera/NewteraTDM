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
using System.Web.UI;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPivotGrid;
using System.IO;
using System.Text;
using DevExpress.XtraPivotGrid.Data;
using System.Reflection;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.ViewInfo;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Web.UI.WebControls;
using DevExpress.XtraPivotGrid.Web;
using System.Web.UI.Design;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.XtraEditors;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxClasses;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraPivotGrid.Web {
	[
	Designer(typeof(ASPxPivotGridExporterDesigner)),
	PersistChildren(false), ParseChildren(true),
	System.Drawing.ToolboxBitmap(typeof(PivotGridControl), ControlConstants.BitmapPath + "ASPxPivotGridExporter.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class ASPxPivotGridExporter : ASPxWebControl, IPrintable, IPivotGridEventsImplementor, IPivotGridPrinterOwner {
		static readonly object customExportHeader = new object();
		static readonly object customExportFieldValue = new object();
		static readonly object customExportCell = new object();
		static readonly object exportStarted = new object();
		static readonly object exportFinished = new object();
		PivotGridPrinterBase printer;
		DynamicPrintHelper printHelper;
		DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid pivotGrid;
		PivotGridViewInfoData viewInfoData;
		WebPivotGridOptionsPrint optionsPrint;
		Dictionary<DevExpress.XtraPivotGrid.PivotGridField, DevExpress.Web.ASPxPivotGrid.PivotGridField> fieldsMap;
		PivotFieldValueItemsCreator columnCreator, rowCreator;
		PivotGridCellDataProvider cellDataProvider;
		protected DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected DevExpress.Web.ASPxPivotGrid.Data.PivotGridWebData ASPxGridData { get { return PivotGrid.Data; } }
		protected PivotFieldValueItemsCreator ColumnCreator {
			get {
				if(columnCreator == null && ASPxGridData != null) {
					columnCreator = new PivotFieldValueItemsCreator(ASPxGridData, true);
					columnCreator.CreateItems();
				}
				return columnCreator;
			}
		}
		protected PivotFieldValueItemsCreator RowCreator {
			get {
				if(rowCreator == null && ASPxGridData != null) {
					rowCreator = new PivotFieldValueItemsCreator(ASPxGridData, false);
					rowCreator.CreateItems();
				}
				return rowCreator;
			}
		}
		protected PivotGridCellDataProvider CellDataProvider {
			get {
				if(cellDataProvider == null && ASPxGridData != null)
					cellDataProvider = new PivotGridCellDataProvider(ASPxGridData);
				return cellDataProvider;
			}
		}
		PivotFieldValueItem LocalToASPxFieldValueItem(PivotFieldValueItem item) {
			return item.IsColumn ? ColumnCreator[item.Index] : RowCreator[item.Index];
		}
		DevExpress.Web.ASPxPivotGrid.PivotGridField LocalToASPxField(DevExpress.XtraPivotGrid.PivotGridField field) {
			if(field == null) return null;
			return FieldsMap[field];
		}
		PivotGridCellItem LocalToASPxCellItem(PivotGridCellItem item) {
			return new PivotGridCellItem(CellDataProvider, ColumnCreator.GetLastLevelItem(item.ColumnIndex),
				RowCreator.GetLastLevelItem(item.RowIndex), item.ColumnIndex, item.RowIndex);
		}
		protected PivotGridViewInfoData Data {
			get {
				if(viewInfoData != null) return viewInfoData;
				FieldsMap.Clear();
				if(PivotGrid == null) return null;
				if(!ASPxGridData.IsDataBound)
					PivotGrid.DataBind();
				viewInfoData = new PivotGridViewInfoData();
				viewInfoData.EventsImplementor = this;
				viewInfoData.OptionsView.Assign(ASPxGridData.OptionsView);
				viewInfoData.AppearancePrint.Combine(WebPrintAppearanceConverter.Convert(AppearancePrint), viewInfoData.AppearancePrint.GetAppearanceDefaultInfo());
				viewInfoData.OptionsPrint.Assign(OptionsPrint);
				viewInfoData.OptionsPrint.UsePrintAppearance = true;
				viewInfoData.OptionsDataField.Assign(ASPxGridData.OptionsDataField);
				viewInfoData.BeginUpdate();
				PivotGridFieldReadOnlyCollection fields = ASPxGridData.GetSortedFields();
				for(int i = 0; i < fields.Count; i++) {
					DevExpress.XtraPivotGrid.PivotGridField field = viewInfoData.Fields.Add(fields[i].FieldName, fields[i].Area);
					field.Assign(fields[i]);
					FieldsMap.Add(field, (DevExpress.Web.ASPxPivotGrid.PivotGridField)fields[i]);
				}
				if(ASPxGridData.IsOLAP)
					viewInfoData.OLAPConnectionString = ASPxGridData.OLAPConnectionString;
				else
					viewInfoData.ListSource = ASPxGridData.ListSource;
				viewInfoData.EndUpdate();
				using(MemoryStream stream = new MemoryStream()) {
					ASPxGridData.SaveCollapsedStateToStream(stream);
					stream.Position = 0;
					viewInfoData.LoadCollapsedStateFromStream(stream);
				}
				viewInfoData.BestFit();
				return viewInfoData;
			}
		}
		protected IASPxPivotGridDataOwner DataOwner { get { return PivotGrid as IASPxPivotGridDataOwner; } }
		protected PivotGridFieldCollection Fields {
			get {
				if(viewInfoData == null) return null;
				return viewInfoData.Fields;
			}
		}
		protected Dictionary<DevExpress.XtraPivotGrid.PivotGridField, DevExpress.Web.ASPxPivotGrid.PivotGridField> FieldsMap {
			get { return fieldsMap; }
		}
		protected WebPrintAppearance AppearancePrint { get { return PivotGrid.StylesPrint; } }
		protected PivotGridPrinterBase Printer { 
			get {
				if(printer == null) {
					printer = CreatePrinter();
					printer.Owner = this;
				}
				return printer; 
			} 
		}
		protected virtual PivotGridPrinterBase CreatePrinter() {
			return new PivotGridPrinterBase(Data);			
		}
		protected override bool HasContent() {
			return false;
		}
		public override bool HasControls() {
			return false;
		}
		protected override bool HasHoverScripts() {
			return false;
		}
		protected override bool HasLoadingDiv() {
			return false;
		}
		protected override bool HasLoadingPanel() {
			return false;
		}
		protected override bool HasFunctionalityScripts() {
			return false;
		}
		protected override bool HasPressedScripts() {
			return false;
		}
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerialize(); }
		void ResetOptionsPrint() { OptionsPrint.Reset(); }
		[Description("Provides access to the print options. "), Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true)]
		public WebPivotGridOptionsPrint OptionsPrint { get { return optionsPrint; } }
		protected DynamicPrintHelper PrintHelper {
			get {
				if(printHelper == null)
					printHelper = new DynamicPrintHelper();
				printHelper.PageSettings = OptionsPrint.PageSettings.ToPageSettings();
				printHelper.VerticalContentSplitting = OptionsPrint.VerticalContentSplitting;
				return printHelper;
			}
		}
		public ASPxPivotGridExporter()
			: base() {
			optionsPrint = new WebPivotGridOptionsPrint();
			fieldsMap = new Dictionary<DevExpress.XtraPivotGrid.PivotGridField, DevExpress.Web.ASPxPivotGrid.PivotGridField>();
		}
		public override void Dispose() {
			if(printer != null)
				printer.Dispose();
			base.Dispose();			
		}
		#region Events
		[Description("Enables you to render a different content for individual field headers."), Category("Export")]
		public event EventHandler<WebCustomExportHeaderEventArgs> CustomExportHeader {
			add { Events.AddHandler(customExportHeader, value); }
			remove { Events.RemoveHandler(customExportHeader, value); }
		}
		[Description("Enables you to render a different content for individual field values."), Category("Export")]
		public event EventHandler<WebCustomExportFieldValueEventArgs> CustomExportFieldValue {
			add { Events.AddHandler(customExportFieldValue, value); }
			remove { Events.RemoveHandler(customExportFieldValue, value); }
		}
		[Description("Enables you to render a different content for individual cells."), Category("Export")]
		public event EventHandler<WebCustomExportCellEventArgs> CustomExportCell {
			add { Events.AddHandler(customExportCell, value); }
			remove { Events.RemoveHandler(customExportCell, value); }
		}
		[Description("Occurs after the ASPxPivotGrid's export has been started."), Category("Export")]
		public event EventHandler ExportStarted {
			add { this.Events.AddHandler(exportStarted, value); }
			remove { this.Events.RemoveHandler(exportStarted, value); }
		}
		[Description("Occurs after the ASPxPivotGrid's export has been completed."), Category("Export")]
		public event EventHandler ExportFinished {
			add { this.Events.AddHandler(exportFinished, value); }
			remove { this.Events.RemoveHandler(exportFinished, value); }
		}
		#endregion
		#region Raise methods
		protected virtual void RaiseCustomExportHeader(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo) {
			EventHandler<WebCustomExportHeaderEventArgs> handler = (EventHandler<WebCustomExportHeaderEventArgs>)this.Events[customExportHeader];
			if(handler != null) {
				WebCustomExportHeaderEventArgs e = new WebCustomExportHeaderEventArgs(brick, headerViewInfo, 
					LocalToASPxField(headerViewInfo.Field));
				handler(this, e);
			}
		}
		protected virtual void RaiseCustomExportFieldValue(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase fieldViewInfo) {
			EventHandler<WebCustomExportFieldValueEventArgs> handler = (EventHandler<WebCustomExportFieldValueEventArgs>)this.Events[customExportFieldValue];
			if(handler != null) {
				WebCustomExportFieldValueEventArgs e = new WebCustomExportFieldValueEventArgs(brick, fieldViewInfo, LocalToASPxField(fieldViewInfo.Field));
				handler(this, e);
			}
		}
		protected virtual void RaiseCustomExportCell(IVisualBrick brick, PivotCellViewInfo cellViewInfo) {
			EventHandler<WebCustomExportCellEventArgs> handler = (EventHandler<WebCustomExportCellEventArgs>)this.Events[customExportCell];
			if(handler != null) {
				WebCustomExportCellEventArgs e = new WebCustomExportCellEventArgs(brick, cellViewInfo,
					LocalToASPxField(cellViewInfo.ColumnField), LocalToASPxField(cellViewInfo.RowField), LocalToASPxField(cellViewInfo.DataField));
				handler(this, e);
			}
		}
		protected virtual void RaiseExportStarted() {
			EventHandler handler = (EventHandler)this.Events[exportStarted];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseExportFinished() {
			EventHandler handler = (EventHandler)this.Events[exportFinished];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		#endregion
		#region ViewState
		IXtraPropertyCollection firstSnapshot;
		protected IXtraPropertyCollection FirstSnapshot { get { return firstSnapshot; } }
		protected void MakeFirstSnapshot() {
			firstSnapshot = GetSnapshot();
		}
		protected IXtraPropertyCollection GetSnapshot() {
			return new SnapshotSerializeHelper().SerializeObject(this, OptionsLayoutBase.FullLayout);
		}
		protected string SerializeSnapshot(IXtraPropertyCollection snapshot) {
			return new Base64XtraSerializer().Serialize(snapshot);
		}
		protected void DeserializeSnapshot(string base64Snapshot) {
			new Base64XtraSerializer().Deserialize(this, base64Snapshot);
		}
		protected void SaveState() {
			IXtraPropertyCollection toSerialize = SerializationDiffCalculator.CalculateDiffCore(firstSnapshot, GetSnapshot());
			CreateHiddenField(this.ID + "STATE", SerializeSnapshot(toSerialize));
		}
		protected void CreateHiddenField(string name, string value) {
			HiddenField field = new HiddenField();
			field.ID = name;
			field.Value = value;
			Controls.Add(field);
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			MakeFirstSnapshot();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			MakeFirstSnapshot();
		}
		protected override object SaveViewState() {
			SaveState();
			return base.SaveViewState();
		}
		#endregion
		[DefaultValue(""), Themeable(false), Category("Data"),
		TypeConverter(typeof(PivotGridIDConverter))]
		public string ASPxPivotGridID {
			get {
				object value = ViewState["ASPxPivotGridID"];
				return value != null ? value.ToString() : string.Empty;
			}
			set {
				if(value == null) value = string.Empty;
				ViewState["ASPxPivotGridID"] = value;
				Reset();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		void Reset() {
			pivotGrid = null;
			viewInfoData = null;
			printer = null;
			columnCreator = null;
			rowCreator = null;
			cellDataProvider = null;
		}
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
		public void ExportToPdf(Stream stream) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, stream);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToPdf(this, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, filePath, options);
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
		public void ExportToText(Stream stream, string separator) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, options);
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored.")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators) {
			RaiseExportStarted();
			PrintHelper.ExportToText(this, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		[Obsolete("The quoteStringsWithSeparators parameter is ignored.")]
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
		#endregion
		#region IPrintable Members
		bool IPrintable.CreatesIntersectedBricks {
			get { return false; }
		}
		void IPrintable.AcceptChanges() {
			Printer.AcceptChanges();
		}
		bool IPrintable.HasPropertyEditor() { return false; }
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() {
			Printer.RejectChanges();
		}
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			Printer.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(Printer != null) Printer.Release();
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			PivotGridControl.SetCommandsVisibility(ps);
			BindToPivotGrid();
			Printer.Initialize(ps, link, Data);
		}
		#endregion
		DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid FindControl(Control root, string id, Control excludeControl) {
			if(root == null) return null;
			foreach(Control control in root.Controls) {
				if(control == excludeControl) continue;
				if(control.ID == id) return control as DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid;
			}
			foreach(Control control in root.Controls) {
				if(control == excludeControl) continue;
				Control result = FindControl(control, id, control.Parent);
				if(result != null) return result as DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid;
			}
			if(root.Parent != excludeControl)
				return FindControl(root.Parent, id, root);
			return null;
		}
		protected void BindToPivotGrid() {
			Reset();
			pivotGrid = FindControl(this, ASPxPivotGridID, this);
			if(PivotGrid == null) throw new Exception("The control specified by the ASPxPivotGridID property couldn't be found.");			
		}
		#region IPivotGridEventsImplementor Members
		void IPivotGridEventsImplementor.DataSourceChanged() { }
		void IPivotGridEventsImplementor.BeginRefresh() { }
		void IPivotGridEventsImplementor.EndRefresh() { }
		void IPivotGridEventsImplementor.LayoutChanged() { }
		bool IPivotGridEventsImplementor.FieldAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex) { return true; }
		object IPivotGridEventsImplementor.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {
			return DataOwner.GetUnboundValue(LocalToASPxField((DevExpress.XtraPivotGrid.PivotGridField)field), listSourceRowIndex);
		}
		void IPivotGridEventsImplementor.CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			DataOwner.CalcCustomSummary(LocalToASPxField((DevExpress.XtraPivotGrid.PivotGridField)field), customSummaryInfo);
		}
		bool IPivotGridEventsImplementor.ShowingCustomizationForm(System.Windows.Forms.Form customizationForm, ref System.Windows.Forms.Control parentControl) { return false; }
		void IPivotGridEventsImplementor.ShowCustomizationForm() { }
		void IPivotGridEventsImplementor.HideCustomizationForm() { }
		void IPivotGridEventsImplementor.OnPopupShowMenu(PivotGridMenuEventArgs e) { }
		void IPivotGridEventsImplementor.OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e) { }
		void IPivotGridEventsImplementor.BeforeLayoutLoad(LayoutAllowEventArgs e) { }
		void IPivotGridEventsImplementor.LayoutUpgrade(LayoutUpgadeEventArgs e) { }
		void IPivotGridEventsImplementor.FieldFilterChanged(PivotGridField field) { }
		void IPivotGridEventsImplementor.FieldAreaChanged(PivotGridField field) { }
		void IPivotGridEventsImplementor.FieldWidthChanged(PivotGridField field) { }
		void IPivotGridEventsImplementor.FieldExpandedInFieldsGroupChanged(PivotGridField field) { }
		string IPivotGridEventsImplementor.FieldValueDisplayText(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			return DataOwner.GetFieldValueDisplayText(LocalToASPxFieldValueItem(fieldCellViewInfo.Item));
		}
		string IPivotGridEventsImplementor.FieldValueDisplayText(PivotGridField field, object value) {
			return DataOwner.GetFieldValueDisplayText(LocalToASPxField(field), value);
		}
		object IPivotGridEventsImplementor.CustomGroupInterval(PivotGridField field, object value) {
			return DataOwner.GetCustomGroupInterval(LocalToASPxField(field), value);
		}
		bool IPivotGridEventsImplementor.BeforeFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) { return false; }
		void IPivotGridEventsImplementor.AfterFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) { }
		int IPivotGridEventsImplementor.FieldValueImageIndex(PivotFieldsAreaCellViewInfo fieldCellViewInfo) { return -1; }
		int IPivotGridEventsImplementor.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return DataOwner.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, 
				LocalToASPxField((DevExpress.XtraPivotGrid.PivotGridField)field), sortOrder);
		}
		void IPivotGridEventsImplementor.CellDoubleClick(DevExpress.XtraPivotGrid.ViewInfo.PivotCellViewInfo cellViewInfo) { }
		void IPivotGridEventsImplementor.CellClick(DevExpress.XtraPivotGrid.ViewInfo.PivotCellViewInfo cellViewInfo) { }
		string IPivotGridEventsImplementor.CustomCellDisplayText(PivotCellViewInfo cellViewInfo) {
			return DataOwner.CustomCellDisplayText(LocalToASPxCellItem(cellViewInfo));
		}
		bool IPivotGridEventsImplementor.CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, System.Drawing.Rectangle bounds) { return false; }
		bool IPivotGridEventsImplementor.CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds) { return false; }
		bool IPivotGridEventsImplementor.CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter) { return false; }
		bool IPivotGridEventsImplementor.CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, HeaderObjectPainter painter) { return false; }
		bool IPivotGridEventsImplementor.CustomDrawCell(ViewInfoPaintArgs paintArgs, ref DevExpress.Utils.AppearanceObject appearance, PivotCellViewInfo cellViewInfo) { return false; }
		void IPivotGridEventsImplementor.CustomAppearance(ref DevExpress.Utils.AppearanceObject appearance, PivotCellViewInfo cellViewInfo) { }
		void IPivotGridEventsImplementor.FocusedCellChanged() { }
		void IPivotGridEventsImplementor.CellSelectionChanged() { }
		void IPivotGridEventsImplementor.AfterFieldValueChangeNotExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) { }
		void IPivotGridEventsImplementor.PrefilterCriteriaChanged() { }
		void IPivotGridEventsImplementor.OLAPQueryTimeout() { }
		object IPivotGridEventsImplementor.CustomEditValue(object value, PivotCellViewInfo cellViewInfo) { return value; }
		int IPivotGridEventsImplementor.GetCustomRowHeight(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int height) {
			return height;
		}
		int IPivotGridEventsImplementor.GetCustomColumnWidth(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int width) {
			return width;
		}
		RepositoryItem IPivotGridEventsImplementor.GetCellEdit(PivotCellViewInfo cellViewInfo) {
			return null;
		}
		#endregion		
		#region IPivotGridPrinterOwner Members
		void IPivotGridPrinterOwner.CustomDrawCell(IVisualBrick brick, PivotCellViewInfo cellViewInfo) {
			RaiseCustomExportCell(brick, cellViewInfo);
		}
		void IPivotGridPrinterOwner.CustomDrawFieldValue(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase fieldViewInfo) {
			RaiseCustomExportFieldValue(brick, fieldViewInfo);
		}
		void IPivotGridPrinterOwner.CustomDrawHeader(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo) {
			RaiseCustomExportHeader(brick, headerViewInfo);
		}
		#endregion
	}
	public class ASPxPivotGridExporterDesigner : ASPxWebControlDesigner {
		protected override string GetDesignTimeHtmlInternal() {
			return base.CreatePlaceHolderDesignTimeHtml();
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences("DevExpress.Data" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.Utils" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.Web.ASPxPivotGrid" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.Web.ASPxPivotGrid" + AssemblyInfo.VSuffix + ".Export");
			EnsureReferences("DevExpress.XtraPivotGrid" + AssemblyInfo.VSuffix + ".Core");
			EnsureReferences("DevExpress.XtraPivotGrid" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.XtraEditors" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.XtraPrinting" + AssemblyInfo.VSuffix);			
		}
		public override bool HasAutoFormats() {
			return false;
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override bool HasAbout() {
			return false;
		}
	}
	public class PivotGridIDConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return (sourceType == typeof(string));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) {
				return string.Empty;
			}
			if(value.GetType() != typeof(string)) {
				throw base.GetConvertFromException(value);
			}
			return (string)value;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			string[] values = null;
			if(context != null) {
				IDesignerHost service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
				if(service != null) {
					IComponent rootComponent = service.RootComponent;
					if(rootComponent != null) {
						List<string> gridList = new List<string>();
						foreach(IComponent component in rootComponent.Site.Container.Components) {
							System.Web.UI.Control grid = component as System.Web.UI.Control;
							if(((grid != null) && !string.IsNullOrEmpty(grid.ID)) && grid.GetType().Name == "ASPxPivotGrid") {
								gridList.Add(grid.ID);
							}
						}
						gridList.Sort(StringComparer.OrdinalIgnoreCase);
						gridList.Insert(0, string.Empty);
						values = gridList.ToArray();
					}
				}
			}
			return new TypeConverter.StandardValuesCollection(values);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class WebPrintAppearanceConverter {
		public static AppearanceObject Convert(WebPrintAppearanceObject obj) {
			AppearanceObject result = new AppearanceObject();
			result.BackColor = obj.BackColor;
			result.BackColor2 = obj.BackColor2;
			result.BorderColor = obj.BorderColor;
			result.Font = obj.Font;
			result.ForeColor = obj.ForeColor;
			result.GradientMode = obj.GradientMode;
			result.Image = obj.Image;
			return result;
		}
		public static PivotGridAppearancesPrint Convert(WebPrintAppearance app) {
			PivotGridAppearancesPrint result = new PivotGridAppearancesPrint();
			Type resultType = result.GetType();
			PropertyInfo[] props = app.GetType().GetProperties();
			for(int i = 0; i < props.Length; i++) {
				PropertyInfo prop = resultType.GetProperty(props[i].Name);
				if(prop == null) continue;
				((AppearanceObject)prop.GetValue(result, null)).Assign(Convert((WebPrintAppearanceObject)props[i].GetValue(app, null)));
			}
			return result;
		}
	}
	public class WebPivotGridOptionsPrint : PivotGridOptionsPrint {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool UsePrintAppearance { get { return base.UsePrintAppearance; } set { base.UsePrintAppearance = value; } }
		bool ShouldSerializePageSettings() { return !PageSettings.IsEmpty; }
		void ResetPageSettings() { PageSettings.Reset(); }
		[Description(""), XtraSerializableProperty(XtraSerializationVisibility.Content),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public override PivotGridPageSettings PageSettings { get { return base.PageSettings; } }
	}
}
