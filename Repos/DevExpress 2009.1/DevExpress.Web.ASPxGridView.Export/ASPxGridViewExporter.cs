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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxClasses;
namespace DevExpress.Web.ASPxGridView.Export {
	public enum GridViewExportedRowType { All, Selected }
	[Designer(typeof(DevExpress.Web.ASPxGridView.Export.Design.ASPxGridViewExportDesigner)),
	PersistChildren(false), ParseChildren(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class ASPxGridViewExporter : Control {
		GridViewExportStyles styles;
		private static readonly object renderBrick = new object();
		GridViewExporterHeaderFooter pageHeader, pageFooter;
		public ASPxGridViewExporter() {
			this.styles = new GridViewExportStyles(null);
			this.pageHeader = new GridViewExporterHeaderFooter();
			this.pageFooter = new GridViewExporterHeaderFooter();
		}
		[Category("Events")]
		public event ASPxGridViewExportRenderingEventHandler RenderBrick {
			add { Events.AddHandler(renderBrick, value); }
			remove { Events.RemoveHandler(renderBrick, value); }
		}
		protected internal virtual ASPxGridViewExportRenderingEventArgs RaiseRenderBrick(int visibleIndex, GridViewDataColumn column, GridViewRowType rowType, WebDataProxy data, DevExpress.XtraPrinting.BrickStyle brickStyle, string text, object textValue, string textValueFormat, string url) {
			ASPxGridViewExportRenderingEventHandler handler = (ASPxGridViewExportRenderingEventHandler)Events[renderBrick];
			if(handler == null) return null;
			ASPxGridViewExportRenderingEventArgs e = new ASPxGridViewExportRenderingEventArgs(visibleIndex, column, rowType, data, brickStyle, text, textValue, textValueFormat, url);
			handler(this, e);
			return e;
		}
		object GetObjectFromViewState(string name, object defaultValue) {
			object value = ViewState[name];
			return value != null ? value : defaultValue;
		}
		void SetObjectToViewState(string name, object defaultValue, object value) {
			if(value == defaultValue) {
				ViewState[name] = null;
			} else {
				ViewState[name] = value;
			}
		}
		[Description("Gets or sets the programmatic identifier of the associated ASPxGridView control."),
		DefaultValue(""), Themeable(false), IDReferenceProperty(typeof(ASPxGridView)),
		TypeConverter(typeof(DevExpress.Web.ASPxGridView.Export.Design.GridViewIDConverter))]
		public virtual string GridViewID {
			get { return (string)GetObjectFromViewState("GridViewID", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetObjectToViewState("GridViewID", string.Empty, value);
			}
		}
		[Description("Gets or sets the file name to which the grid's data is exported."),
		DefaultValue(""), Themeable(false)]
		public virtual string FileName {
			get { return (string)GetObjectFromViewState("FileName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetObjectToViewState("FileName", string.Empty, value);
			}
		}
		const int DefaultMaxColumnWidth = 300;
		protected internal const int MinColumnWidth = 20;
		[Description("Gets or sets the column's maximum width."),
		Category("Behavior"), DefaultValue(DefaultMaxColumnWidth), Themeable(false)]
		public virtual int MaxColumnWidth {
			get { return (int)GetObjectFromViewState("MaxColumnWidth", DefaultMaxColumnWidth); }
			set {
				if(value <= MinColumnWidth) value = MinColumnWidth;
				SetObjectToViewState("MaxColumnWidth", DefaultMaxColumnWidth, value);
			}
		}
		[Description("Gets or sets whether check boxes used to select/deselect data rows, are printed."),
		Category("Behavior"), DefaultValue(false)]
		public virtual bool PrintSelectCheckBox {
			get { return (bool)GetObjectFromViewState("PrintSelectCheckBox", false); }
			set { SetObjectToViewState("PrintSelectCheckBox", false, value); }
		}
		[Description("Gets or sets whether the expanded state of group rows is preserved when the ASPxGridView's data is exported."),
		Category("Behavior"), DefaultValue(false)]
		public virtual bool PreserveGroupRowStates {
			get { return (bool)GetObjectFromViewState("PreserveGroupRowStates", false); }
			set { SetObjectToViewState("PreserveGroupRowStates", false, value); }
		}
		[Description("Gets or sets which rows should be exported."),
		Category("Behavior"), DefaultValue(GridViewExportedRowType.All)]
		public virtual GridViewExportedRowType ExportedRowType {
			get { return (GridViewExportedRowType)GetObjectFromViewState("ExportedRowType", GridViewExportedRowType.All); }
			set { SetObjectToViewState("ExportedRowType", GridViewExportedRowType.All, value); }
		}
		[Description("Gets or sets the target document's bottom margin."),
		Category("PageSettings"), DefaultValue(-1)]
		public virtual int BottomMargin {
			get { return (int)GetObjectFromViewState("BottomMargin", -1); }
			set {
				if(value < -1) value = -1;
				SetObjectToViewState("BottomMargin", -1, value);
			}
		}
		[Description("Gets or sets the target document's top margin."),
		Category("PageSettings"), DefaultValue(-1)]
		public virtual int TopMargin {
			get { return (int)GetObjectFromViewState("TopMargin", -1); }
			set {
				if(value < -1) value = -1;
				SetObjectToViewState("TopMargin", -1, value);
			}
		}
		[Description("Gets or sets the target document's left margin."),
		Category("PageSettings"), DefaultValue(-1)]
		public virtual int LeftMargin {
			get { return (int)GetObjectFromViewState("LeftMargin", -1); }
			set {
				if(value < -1) value = -1;
				SetObjectToViewState("LeftMargin", -1, value);
			}
		}
		[Description("Gets or sets the target document's right margin."),
		Category("PageSettings"), DefaultValue(-1)]
		public virtual int RightMargin {
			get { return (int)GetObjectFromViewState("RightMargin", -1); }
			set {
				if(value < -1) value = -1;
				SetObjectToViewState("RightMargin", -1, value);
			}
		}
		[Description("Gets or sets whether data is exported to PDF in Landscape."),
		Category("PageSettings"), DefaultValue(false)]
		public bool Landscape {
			get { return (bool)GetObjectFromViewState("Landscape", false); }
			set { SetObjectToViewState("Landscape", false, value); }
		}
		[Description("Provides access to the properties that specify the appearance of grid elements when the grid is exported."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportStyles Styles { get { return styles; } }
		[Description("Gets the page header's settings."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor(typeof(DevExpress.Web.ASPxGridView.Export.Design.GridViewExporterPageHeaderFooterEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public GridViewExporterHeaderFooter PageHeader { get { return pageHeader; } }
		[Description("Gets the page footer's settings."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor(typeof(DevExpress.Web.ASPxGridView.Export.Design.GridViewExporterPageHeaderFooterEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public GridViewExporterHeaderFooter PageFooter { get { return pageFooter; } }
		[Description("Gets or sets the text displayed within a report's header."),
		NotifyParentProperty(true), DefaultValue(""),
		Editor(typeof(DevExpress.Web.ASPxGridView.Export.Design.GridViewExporterReportHeaderFooterEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ReportHeader {
			get { return (string)GetObjectFromViewState("ReportHeader", string.Empty); }
			set { SetObjectToViewState("ReportHeader", string.Empty, value); }
		}
		[Description("Gets or sets the text displayed within a report's footer."),
		NotifyParentProperty(true), DefaultValue(""),
		Editor(typeof(DevExpress.Web.ASPxGridView.Export.Design.GridViewExporterReportHeaderFooterEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ReportFooter {
			get { return (string)GetObjectFromViewState("ReportFooter", string.Empty); }
			set { SetObjectToViewState("ReportFooter", string.Empty, value); }
		}
		[Description("Gets or sets the detail row's vertical offset."),
		NotifyParentProperty(true), DefaultValue(5)]
		public int DetailVerticalOffset {
			get { return (int)GetObjectFromViewState("DetailVerticalOffset", 5); }
			set {
				if(value < 0) value = 0;
				SetObjectToViewState("DetailVerticalOffset", 5, value);
			}
		}
		[Description("Gets or sets the detail row's horizontal offset."),
		NotifyParentProperty(true), DefaultValue(5)]
		public int DetailHorizontalOffset {
			get { return (int)GetObjectFromViewState("DetailHorizontalOffset", 5); }
			set {
				if(value < 0) value = 0;
				SetObjectToViewState("DetailHorizontalOffset", 5, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxGridView GridView {
			get {
				if(Page == null) return null;
				if(string.IsNullOrEmpty(GridViewID))
					return FindAnyGrid(Page);
				return FindControlHelper.LookupControl(this, GridViewID) as ASPxGridView;
			}
		}
		ASPxGridView FindAnyGrid(Control control) {
			if(control is ASPxGridView) return control as ASPxGridView;
			foreach(Control child in control.Controls) {
				ASPxGridView grid = FindAnyGrid(child);
				if (grid != null) return grid;
			}
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void RenderControl(HtmlTextWriter writer) { base.RenderControl(writer); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ApplyStyleSheetSkin(Page page) { base.ApplyStyleSheetSkin(page); }
		protected override ControlCollection CreateControlCollection() { return new EmptyControlCollection(this); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Control FindControl(string id) { return base.FindControl(id); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Focus() {  }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool HasControls() { return base.HasControls(); }
		[Description("This property is not in effect."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string ClientID { get { return base.ClientID; } }
		[Description("This property is not in effect."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override ControlCollection Controls { get { return base.Controls; } }
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public override bool EnableTheming { get { return false; } set { } }
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string SkinID { get { return base.SkinID; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible { get { return false; } set { } }
		#region Pdf Export
		protected virtual void WritePdfCore(Stream stream, DevExpress.XtraPrinting.ExportOptionsBase exportOptions) {
			DevExpress.XtraPrinting.PdfExportOptions options = exportOptions as DevExpress.XtraPrinting.PdfExportOptions;
			using(DevExpress.XtraPrinting.PrintingSystem ps = CreatePS()) {
				if(options != null) {
					ps.ExportToPdf(stream, options);
				} else {
					ps.ExportToPdf(stream);
				}
			}
		}
		public void WritePdf(Stream stream) {
			WritePdfCore(stream, null);
		}
		public void WritePdfToResponse() {
			WritePdfToResponse(GetFileName());
		}
		public void WritePdfToResponse(string fileName) {
			WritePdfToResponse(fileName, true);
		}
		public void WritePdfToResponse(bool saveAsFile) {
			WritePdfToResponse(GetFileName(), saveAsFile);
		}
		public void WritePdfToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "pdf", new ExportToStream(WritePdfCore), null);
		}
		public void WritePdf(Stream stream, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			WritePdfCore(stream, exportOptions);
		}
		public void WritePdfToResponse(DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			WritePdfToResponse(GetFileName(), exportOptions);
		}
		public void WritePdfToResponse(string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			WritePdfToResponse(fileName, true, exportOptions);
		}
		public void WritePdfToResponse(bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			WritePdfToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WritePdfToResponse(string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "pdf", new ExportToStream(WritePdfCore), exportOptions);
		}
		#endregion
		#region Xls Export
		protected virtual void WriteXlsCore(Stream stream, DevExpress.XtraPrinting.ExportOptionsBase exportOptions) {
			DevExpress.XtraPrinting.XlsExportOptions options = exportOptions as DevExpress.XtraPrinting.XlsExportOptions;
			using(DevExpress.XtraPrinting.PrintingSystem ps = CreatePS()) {
				if(options != null) {
					ps.ExportToXls(stream, options);
				} else {
					ps.ExportToXls(stream);
				}
			}
		}
		public void WriteXls(Stream stream) {
			WriteXlsCore(stream, null);
		}
		public void WriteXlsToResponse() {
			WriteXlsToResponse(GetFileName());
		}
		public void WriteXlsToResponse(string fileName) {
			WriteXlsToResponse(fileName, true);
		}
		public void WriteXlsToResponse(bool saveAsFile) {
			WriteXlsToResponse(GetFileName(), saveAsFile);
		}
		public void WriteXlsToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "xls", new ExportToStream(WriteXlsCore), null);
		}
		public void WriteXls(Stream stream, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			WriteXlsCore(stream, exportOptions);
		}
		public void WriteXlsToResponse(DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			WriteXlsToResponse(GetFileName(), exportOptions);
		}
		public void WriteXlsToResponse(string fileName, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			WriteXlsToResponse(fileName, true, exportOptions);
		}
		public void WriteXlsToResponse(bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			WriteXlsToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteXlsToResponse(string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "xls", new ExportToStream(WriteXlsCore), exportOptions);
		}
		#endregion
		#region Rtf Export
		protected virtual void WriteRtfCore(Stream stream, DevExpress.XtraPrinting.ExportOptionsBase exportOptions) {
			DevExpress.XtraPrinting.RtfExportOptions options = exportOptions as DevExpress.XtraPrinting.RtfExportOptions;
			using(DevExpress.XtraPrinting.PrintingSystem ps = CreatePS()) {
				if(options != null) {
					ps.ExportToRtf(stream, options);
				} else {
					ps.ExportToRtf(stream);
				}
			}
		}
		public void WriteRtf(Stream stream) {
			WriteRtfCore(stream, null);
		}
		public void WriteRtfToResponse() {
			WriteRtfToResponse(GetFileName());
		}
		public void WriteRtfToResponse(string fileName) {
			WriteRtfToResponse(fileName, true);
		}
		public void WriteRtfToResponse(bool saveAsFile) {
			WriteRtfToResponse(GetFileName(), saveAsFile);
		}
		public void WriteRtfToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "rtf", new ExportToStream(WriteRtfCore), null);
		}
		public void WriteRtf(Stream stream, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			WriteRtfCore(stream, exportOptions);
		}
		public void WriteRtfToResponse(DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			WriteRtfToResponse(GetFileName(), exportOptions);
		}
		public void WriteRtfToResponse(string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			WriteRtfToResponse(fileName, true, exportOptions);
		}
		public void WriteRtfToResponse(bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			WriteRtfToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteRtfToResponse(string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "rtf", new ExportToStream(WriteRtfCore), exportOptions);
		}
		#endregion
		#region Csv Export
		protected virtual void WriteCsvCore(Stream stream, DevExpress.XtraPrinting.ExportOptionsBase exportOptions) {
			DevExpress.XtraPrinting.CsvExportOptions options = exportOptions as DevExpress.XtraPrinting.CsvExportOptions;
			using(DevExpress.XtraPrinting.PrintingSystem ps = CreatePS()) {
				if(options != null) {
					ps.ExportToCsv(stream, options);
				} else {
					ps.ExportToCsv(stream);
				}
			}
		}
		public void WriteCsv(Stream stream) {
			WriteCsv(stream, null);
		}
		public void WriteCsvToResponse() {
			WriteCsvToResponse(GetFileName());
		}
		public void WriteCsvToResponse(string fileName) {
			WriteCsvToResponse(fileName, true);
		}
		public void WriteCsvToResponse(bool saveAsFile) {
			WriteCsvToResponse(GetFileName(), saveAsFile);
		}
		public void WriteCsvToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "csv", new ExportToStream(WriteCsvCore), null);
		}
		public void WriteCsv(Stream stream, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			WriteCsvCore(stream, exportOptions);
		}
		public void WriteCsvToResponse(DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			WriteCsvToResponse(GetFileName(), exportOptions);
		}
		public void WriteCsvToResponse(string fileName, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			WriteCsvToResponse(fileName, true, exportOptions);
		}
		public void WriteCsvToResponse(bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			WriteCsvToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteCsvToResponse(string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "csv", new ExportToStream(WriteCsvCore), exportOptions);
		}
		#endregion
		protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, ExportToStream getStream, DevExpress.XtraPrinting.ExportOptionsBase options) {
			if(Page == null || Page.Response == null) return;
			MemoryStream stream = new MemoryStream();
			getStream(stream, options);
			string disposition = saveAsFile ? "attachment" : "inline";
			Page.Response.Clear();
			Page.Response.Buffer = false;
			Page.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat));
			Page.Response.AppendHeader("Content-Transfer-Encoding", "binary");
			Page.Response.AppendHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", disposition, HttpUtility.UrlEncode(fileName).Replace("+", "%20"),  fileFormat));
			Page.Response.BinaryWrite(stream.ToArray());
			Page.Response.End();
		}
		protected delegate void ExportToStream(Stream stream, DevExpress.XtraPrinting.ExportOptionsBase options);
		protected string GetFileName() {
			if(!string.IsNullOrEmpty(FileName)) return FileName;
			if(GridView != null) return GridView.ID;
			return "GridView";
		}
		protected virtual DevExpress.XtraPrinting.PrintingSystem CreatePS() {
			if(GridView == null) throw new Exception("GridView is null"); 
			DevExpress.Web.ASPxGridView.Export.Helper.GridViewLink link = new DevExpress.Web.ASPxGridView.Export.Helper.GridViewLink(this);
			link.Landscape = Landscape;
			link.PageHeaderFooter = new DevExpress.XtraPrinting.PageHeaderFooter(
				new DevExpress.XtraPrinting.PageHeaderArea(PageHeader.Text, PageHeader.GetFont(), PageHeader.VerticalAlignment),
				new DevExpress.XtraPrinting.PageFooterArea(PageFooter.Text, PageFooter.GetFont(), PageFooter.VerticalAlignment));
			link.RtfReportHeader = ReportHeader;
			link.RtfReportFooter = ReportFooter;
			DevExpress.XtraPrinting.PrintingSystem ps = link.CreatePS();
			if(ps == null) return null;
			if(BottomMargin > -1) {
				ps.PageSettings.BottomMargin = BottomMargin;
			}
			if(TopMargin > -1) {
				ps.PageSettings.TopMargin = TopMargin;
			}
			if(LeftMargin > -1) {
				ps.PageSettings.LeftMargin = LeftMargin;
			}
			if(RightMargin > -1) {
				ps.PageSettings.RightMargin = RightMargin;
			}
			return ps;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridViewExporterHeaderFooter : StateManager {
		Style style;
		public GridViewExporterHeaderFooter() {
			this.style = new Style();
		}
		[Description(""), DefaultValue(""), NotifyParentProperty(true)]
		public string Left {
			get { return GetStringProperty("Left", string.Empty); }
			set { SetStringProperty("Left", string.Empty, value); }
		}
		[Description(""), DefaultValue(""), NotifyParentProperty(true)]
		public string Center {
			get { return GetStringProperty("Center", string.Empty); }
			set { SetStringProperty("Center", string.Empty, value); }
		}
		[Description(""), DefaultValue(""), NotifyParentProperty(true)]
		public string Right {
			get { return GetStringProperty("Right", string.Empty); }
			set { SetStringProperty("Right", string.Empty, value); }
		}
		protected internal string[] Text { get { return new string[] { Left, Center, Right }; } }
		protected internal System.Drawing.Font GetFont() {
			return DevExpress.Web.ASPxGridView.Export.Helper.GridViewPrinter.CreateFontByFontInfo(DevExpress.XtraPrinting.BrickStyle.DefaultFont, Font);
		}
		protected Style Style { get { return style; } }
		[Description(""),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public FontInfo Font { get { return style.Font; } }
		[Description(""), 
		DefaultValue(DevExpress.XtraPrinting.BrickAlignment.Center), NotifyParentProperty(true)]
		public DevExpress.XtraPrinting.BrickAlignment VerticalAlignment {
			get { return (DevExpress.XtraPrinting.BrickAlignment)GetEnumProperty("VerticalAlignment", DevExpress.XtraPrinting.BrickAlignment.Center); }
			set { SetEnumProperty("VerticalAlignment", DevExpress.XtraPrinting.BrickAlignment.Center, value); }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Style };
		}
	}
	public class ASPxGridViewExportRenderingEventArgs : EventArgs {
		int visibleIndex;
		GridViewDataColumn column;
		GridViewRowType rowType;
		WebDataProxy data;
		DevExpress.XtraPrinting.BrickStyle brickStyle;
		string text;
		object textValue;
		string textValueFormatString;
		string url;
		protected internal ASPxGridViewExportRenderingEventArgs(int visibleIndex, GridViewDataColumn column, GridViewRowType rowType, WebDataProxy data, DevExpress.XtraPrinting.BrickStyle brickStyle,
			string text, object textValue, string textValueFormatString, string url) {
			this.visibleIndex = visibleIndex;
			this.column = column;
			this.rowType = rowType;
			this.data = data;
			this.brickStyle = new DevExpress.XtraPrinting.BrickStyle(brickStyle);
			this.text = text;
			this.textValue = textValue;
			this.textValueFormatString = textValueFormatString;
			this.url = url;
		}
		protected WebDataProxy Data { get { return data; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public GridViewDataColumn Column { get { return column; } }
		public GridViewRowType RowType { get { return rowType; } }
		public string Text { get { return text; } set { text = value; } }
		public object TextValue { get { return textValue; } set { textValue = value; } }
		public string TextValueFormatString { get { return textValueFormatString; } set { textValueFormatString = value; } }
		public string Url { get { return url; } set { url = value; } }
		public DevExpress.XtraPrinting.BrickStyle BrickStyle { get { return brickStyle; } }
		public object KeyValue { get { return VisibleIndex > -1 ? Data.GetRowKeyValue(VisibleIndex) : null; } }
		public object GetValue(string fieldName) {  return Data.GetRowValue(visibleIndex, fieldName); } 
		public object Value { get { return Data.GetRowValue(VisibleIndex, Column != null ? Column.FieldName : string.Empty); } }
	}
	public delegate void ASPxGridViewExportRenderingEventHandler(object sender, ASPxGridViewExportRenderingEventArgs e);
}
