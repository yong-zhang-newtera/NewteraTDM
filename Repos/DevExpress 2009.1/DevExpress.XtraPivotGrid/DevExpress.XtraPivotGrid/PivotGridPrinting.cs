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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid.Frames;
namespace DevExpress.XtraPivotGrid.Printing {
	public class PivotGridPrintViewInfo : PivotGridViewInfo {
		bool isPSOwner;
		public PivotGridPrintViewInfo(PivotGridViewInfoData data, bool isPSOwner) : base(data)	{
			this.isPSOwner = isPSOwner;
		}
		public override void MouseMove(MouseEventArgs e) {  }
		public override void MouseDown(MouseEventArgs e) {  }
		public override void KeyDown(KeyEventArgs e) {  }
		public override bool IsEnabled { get { return true; } }
		public override bool AllowExpand { get { return false; } }		
		public override bool CanHeaderSort { get { return false; } }
		public override bool CanHeaderFilter { get { return false; } }
		private bool UsePrintAppearance {
			get {
				return Data.OptionsPrint.UsePrintAppearance;
			}
		}
		public override PivotGridAppearancesBase PrintAndPaintAppearance { 
			get {
				return UsePrintAppearance ? Data.PaintAppearancePrint : base.PrintAndPaintAppearance;
			} 
		}
		public Color LinesColor {
			get {
				if(UsePrintAppearance || Data.Appearance.Lines.Options.UseForeColor)
					return PrintAndPaintAppearance.Lines.ForeColor;
				else
					return Color.Empty;	 
			}
		}
		public override bool CustomDrawFieldValue(ViewInfoPaintArgs e, PivotFieldsAreaCellViewInfoBase fieldCellViewInfo,
			HeaderObjectInfoArgs info, HeaderObjectPainter painter) {
			Rectangle bounds = info.Bounds;
			info.Appearance.DrawBackground(info.Cache, bounds);
			Rectangle headerBounds = bounds;
			if(info.HeaderPosition != HeaderPositionKind.Left) {
				headerBounds.X --;
				headerBounds.Width ++;
			}
			if(!info.IsTopMost) {
				headerBounds.Y --;
				headerBounds.Height ++;
			}
			info.Cache.DrawRectangle(Pens.Black, headerBounds);
			bounds.Inflate(-2, -1);
			info.Appearance.DrawString(info.Cache, info.Caption, bounds);
			return true;
		}
		public override bool DrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs e, HeaderObjectPainter painter) {
			HeaderObjectInfoArgs info = headerViewInfo.InfoArgs;
			Rectangle bounds = info.Bounds;
			info.Appearance.DrawBackground(info.Cache, bounds);
			bounds.Inflate(-2, -1);
			info.Appearance.DrawString(info.Cache, info.Caption, bounds);
			return true;
		}
		public override bool DrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			return true;
		}
		protected override void FillHeadersAndFields(ViewInfoPaintArgs e, Rectangle bounds) {
			Data.PaintAppearance.Empty.FillRectangle(e.GraphicsCache, bounds);
		}
		public override bool CanShowHeader(PivotArea area) {
			if(Data.GetFieldCountByArea(area) == 0) return false;
			if(Data.OptionsPrint.GetPrintHeaders(area) == DefaultBoolean.Default)
				return Data.OptionsView.GetShowHeaders(area);
			return Data.OptionsPrint.GetPrintHeaders(area) == DefaultBoolean.True ? true : false;
		}
		public override int HeaderWidthOffset { get { return 0; } }
		public override int HeaderHeightOffset { get { return 0; } }
		public override int FirstLastHeaderWidthOffset { get { return 0; } }
		protected override int CalcFieldHeight(bool isHeader, int lineCount) {
			return base.CalcFieldHeight(false, lineCount);
		}
		protected override PivotCellsViewInfoBase CreateCellsViewInfo() {
			return new PivotPrintCellsViewInfo(this);
		}
		public new PivotPrintFieldsAreaViewInfo ColumnAreaFields { get { return (PivotPrintFieldsAreaViewInfo)base.ColumnAreaFields; } }
		public new PivotPrintFieldsAreaViewInfo RowAreaFields { get { return (PivotPrintFieldsAreaViewInfo)base.RowAreaFields; } }
		protected override PivotFieldsAreaViewInfoBase CreateFieldsAreawViewInfo(bool isColumn) {
			return new PivotPrintFieldsAreaViewInfo(this, isColumn);
		}
		protected override PivotHeadersViewInfoBase CreateHeadersViewInfo(int i) {
			return new PivotPrintHeadersViewInfo(this, (PivotArea)i);
		}
		public override Rectangle ScrollableRectangle { get { return ClientRectangle; } }
		protected bool IsPSOwner { get { return isPSOwner; } }
		protected override Rectangle ClientRectangle { 
			get { 
				return !IsPSOwner ? base.ClientRectangle : new Rectangle(0, 0, int.MaxValue, int.MaxValue); 
			} 
		}
		protected override int RowAreaFieldsAndCellAreaTop { 
			get { 
				if(Data.OptionsPrint.PrintHeadersOnEveryPage) return 0;
				return base.RowAreaFieldsAndCellAreaTop; 
			} 
		}
	}
	public class PivotPrintFieldsAreaViewInfo : PivotFieldsAreaViewInfo {
		public PivotPrintFieldsAreaViewInfo(PivotGridPrintViewInfo viewInfo, bool isColumn) : base(viewInfo, isColumn) {
		}
		public override int GetFieldValueSeparator(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			if(cellViewInfo.ValueType != PivotGridValueType.Value && cellViewInfo.ValueType != PivotGridValueType.GrandTotal) 
				return 0;
			if(!IsTheSameLevelWithRoot(cellViewInfo.VisibleIndex, cellViewInfo.MinLastLevelIndex)) 
				return 0;
			return IsColumn ? Data.OptionsPrint.ColumnFieldValueSeparator : Data.OptionsPrint.RowFieldValueSeparator;
		}
		protected bool IsTheSameLevelWithRoot(int visibleIndex, int minLastLevelIndex) {
			if(minLastLevelIndex == 0) return false;
			int level = Data.GetObjectLevel(IsColumn, visibleIndex);
			if(level <= 0) return true;
			for(int i = visibleIndex - 1; i > 0; i--) {
				int parentLevel = Data.GetObjectLevel(IsColumn, i);
				if(parentLevel == 0) return true;
				if(level == parentLevel) return false;
			}
			return false;
		}
		protected override PivotFieldsAreaCellViewInfoBase CreateFieldsAreaCellViewInfo(PivotFieldValueItem item) {
			return new PivotPrintFieldsAreaCellViewInfo(ViewInfo, item);
		}
	}
	public class PivotPrintFieldsAreaCellViewInfo : PivotFieldsAreaCellViewInfo {
		public PivotPrintFieldsAreaCellViewInfo(PivotGridViewInfo viewInfo, PivotFieldValueItem item)
			: base(viewInfo, item) { }
		public override int GetBestWidth(GraphicsCache graphicsCache) {
			HeaderObjectInfoArgs info = CreateHeaderInfoArgs(graphicsCache);
			int res = GetBestWidthCore(info);
			HeaderObjectInfoArgs ee = (HeaderObjectInfoArgs)info;
			string caption = info.Caption;
			int gdiWidth = ee.Appearance.CalcTextSize(ee.Graphics, caption, 0).ToSize().Width,
				gdipWidth = ee.Graphics.MeasureString(caption, ee.Appearance.Font).ToSize().Width;
			res += gdipWidth - gdiWidth;
			return res;
		}
		protected override int GetLeftHeaderTextOffset() {
			return HeaderTextOffset;
		}
		protected override int GetRightHeaderTextOffset() {
			return 0;
		}
	}
	public class PivotPrintHeadersViewInfo : PivotHeadersViewInfo {
		public PivotPrintHeadersViewInfo(PivotGridPrintViewInfo viewInfo, PivotArea area) 
			: base(viewInfo, area) {
		}
		protected override PivotHeaderViewInfoBase CreateHeaderViewInfo(PivotGridField field) {
			return new PivotPrintHeaderViewInfo(ViewInfo, field);
		}
	}
	public class PivotPrintHeaderViewInfo : PivotHeaderViewInfo {
		public PivotPrintHeaderViewInfo(PivotGridViewInfo viewInfo, PivotGridField field)
			: base(viewInfo, field) { }
		protected override bool AddImage { get { return false; } }
		protected override bool AddCollapseButton { get { return false; } }
		public override Padding GetPaddings() {
			return new Padding(PivotFieldsAreaCellViewInfoBase.HeaderTextOffset, 0, 0, 0);
		}
	}
	public class PivotPrintCellsViewInfo : PivotCellsViewInfo {
		public PivotPrintCellsViewInfo(PivotGridPrintViewInfo viewInfo) : base(viewInfo) {
		}
		protected override bool DrawFocusedCellRect { get { return false; } }
		public override bool ShowVertLines { 
			get { 
				if(Data.OptionsPrint.PrintVertLines == DefaultBoolean.Default)
					return Data.OptionsView.ShowVertLines; 
				return Data.OptionsPrint.PrintVertLines == DefaultBoolean.True ? true : false;
			} 
		}
		public override bool ShowHorzLines { 
			get { 
				if(Data.OptionsPrint.PrintHorzLines == DefaultBoolean.Default)
					return Data.OptionsView.ShowHorzLines; 
				return Data.OptionsPrint.PrintHorzLines == DefaultBoolean.True ? true : false;
			} 
		}
	}
	public class PivotGridPrinter : PivotGridPrinterBase {
		PivotGridControl pivotGridControl;
		PivotGridPrinting printControl;
		public PivotGridPrinter(PivotGridControl pivotGridControl)
			: base(pivotGridControl.Data) {
			this.pivotGridControl = pivotGridControl;
			this.printControl = null;
		}
		public PivotGridControl PivotGridControl { get { return pivotGridControl; } }
		public override void AcceptChanges() {
			printControl.ApplyOptions(true);
		}
		protected internal override UserControl PropertyEditorControl {
			get {
				if(printControl == null) {
					printControl = CreatePropertyEditorControl();
				}
				return printControl;
			}
		}
		PivotGridPrinting CreatePropertyEditorControl() {
			PivotGridPrinting ctrl = new PivotGridPrinting();
			ctrl.InitFrame(PivotGridControl, PivotGridLocalizer.GetString(PivotGridStringId.PrintDesigner), null);
			ctrl.lbCaption.Visible = false;
			ctrl.Size = ctrl.UserControlSize;
			ctrl.AutoApply = false;
			return ctrl;
		}
	}
	public interface IPivotGridPrinterOwner {
		void CustomDrawHeader(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo);
		void CustomDrawFieldValue(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase fieldViewInfo);
		void CustomDrawCell(IVisualBrick brick, PivotCellViewInfo cellViewInfo);
	}
	public class PivotGridPrinterBase : IDisposable {
		IPrintingSystem ps;
		ILink link;
		PivotGridPrintViewInfo viewInfo;
		IBrickGraphics graph;
		IPivotGridPrinterOwner owner;
		PivotGridViewInfoData data;
		public PivotGridPrinterBase(PivotGridViewInfoData data) {
			this.data = data;
		} 
		public void Dispose() {
			Release();
		}
		public void Release() {
			if(this.ps != null) {
				this.ps.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(OnAfterChange);
				this.ps = null;
			}
			this.link = null;
			this.viewInfo = null;
		}
		public void Initialize(IPrintingSystem ps, ILink link, PivotGridViewInfoData data) {
			if(this.ps != ps) {
				this.ps = ps;
				this.ps.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(OnAfterChange);
			}
			this.link = link;
			ViewInfo.EnsureIsCalculated();
		}
		protected virtual PivotGridPrintViewInfo CreateViewInfo(PivotGridViewInfoData data) {
			return new PivotGridPrintViewInfo(data, true);
		}
		protected IPrintingSystem PS { get { return ps; } }
		protected virtual IBrickGraphics Graph { get { return graph; } }
		public IPivotGridPrinterOwner Owner { get { return owner; } set { owner = value; } }
		protected internal PivotGridPrintViewInfo ViewInfo { 
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo(Data);
				return viewInfo; 
			} 
		}
		protected internal PivotPrintCellsViewInfo CellsArea { get { return (PivotPrintCellsViewInfo)ViewInfo.CellsArea; } }
		protected internal PivotGridViewInfoData Data { get { return data; } }
		protected bool PrintHeadersOnEveryPage { get { return Data.OptionsPrint.PrintHeadersOnEveryPage; } }
		public virtual void CreateArea(string areaName, IBrickGraphics graph) {
			this.graph = graph;
			switch(areaName) {
				case DevExpress.XtraPrinting.SR.MarginalHeader: 
					CreateHeader();
					break;
				case DevExpress.XtraPrinting.SR.Detail:
					CreateDetails();
					break;
				case DevExpress.XtraPrinting.SR.DetailHeader:
					CreateDetailHeader();
					break;
			}
		}
		public virtual void AcceptChanges() { }
		public void RejectChanges() {}
		public void ShowHelp() {}
		public bool SupportsHelp() { return false; }
		public bool HasPropertyEditor() { return true; } 
		protected internal virtual UserControl PropertyEditorControl { get { return null; } }
		void OnAfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
		}
		protected void CreateHeader() {
		}
		protected void CreateDetailHeader() {
			if(PrintHeadersOnEveryPage) {
				CreateHeaderBricks();
			}
		}
		protected void CreateDetails() {
			if(!PrintHeadersOnEveryPage) {
				CreateHeaderBricks();
			}
			CreateDetailBricks();
		}
		void CreateHeaderBricks() {
			DrawHeaders();
			DrawColumns();
		}
		void CreateDetailBricks() {
			DrawRows();
			DrawCells();
		}
		protected void DrawHeaders() {
			for(int i = 0; i < ViewInfo.HeaderCount; i ++)
				DrawHeader(ViewInfo.GetHeader((PivotArea)i));
		}
		protected void DrawHeader(PivotHeadersViewInfoBase headersViewInfo) {
			if(headersViewInfo.Bounds.Width == 0 || headersViewInfo.Bounds.Height == 0) return;
			BrickStyle defBrick = this.graph.DefaultBrickStyle;
			for(int i = 0; i < headersViewInfo.ChildCount; i ++) {
				ITextBrick brick = DrawHeaderBrick(headersViewInfo[i]);
			}
			this.graph.DefaultBrickStyle = defBrick;
		}
		protected virtual ITextBrick DrawHeaderBrick(PivotHeaderViewInfoBase headerViewInfo) {
			AppearanceObject headerAppearance = CalculateAppearance(headerViewInfo);
			SetDefaultBrickStyle(headerAppearance);
			ITextBrick brick = DrawTextBrick(headerViewInfo.Caption, headerViewInfo.ControlBounds, 
				headerViewInfo.GetPaddings());
			brick.Separable = false;
			if(Owner != null) Owner.CustomDrawHeader(brick, headerViewInfo);
			return brick;
		}
		protected void DrawColumns() {
			DrawFieldValues(ViewInfo.ColumnAreaFields);
		}
		protected void DrawRows() {
			DrawFieldValues(ViewInfo.RowAreaFields);
		}
		protected void DrawFieldValues(PivotPrintFieldsAreaViewInfo areaViewInfo) {
			Point offset = areaViewInfo.ControlBounds.Location;
			BrickStyle defBrick = this.graph.DefaultBrickStyle;
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			try {
				for(int i = 0; i < areaViewInfo.ChildCount; i++) {
					PivotFieldsAreaCellViewInfo[] children = new PivotFieldsAreaCellViewInfo[0];
					if(!Data.OptionsPrint.IsMergeFieldValues(areaViewInfo.IsColumn)) {
						children = areaViewInfo[i].GetLastLevelChildren();
					}
					if(children.Length == 0) {
						DrawFieldValue(graphicsCache, areaViewInfo[i], areaViewInfo[i].Bounds, offset);
					}
					else {
						DrawUnmergeFieldValue(graphicsCache, areaViewInfo[i], children, offset);
					}
				}
			}
			finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
				this.graph.DefaultBrickStyle = defBrick;
			}
		}
		protected void DrawUnmergeFieldValue(GraphicsCache graphicsCache, PivotFieldsAreaCellViewInfoBase fieldViewInfo, PivotFieldsAreaCellViewInfo[] children, Point offset) {
			for(int i = 0; i < children.Length; i ++) {
				Rectangle bounds = fieldViewInfo.Bounds;
				if(fieldViewInfo.IsColumn) {
					bounds.X = children[i].Bounds.X;
					bounds.Width = children[i].Bounds.Width;
				} else {
					bounds.Y = children[i].Bounds.Y;
					bounds.Height = children[i].Bounds.Height;
				}
				DrawFieldValue(graphicsCache, fieldViewInfo, bounds, offset);
			}
		}
		protected void DrawFieldValue(GraphicsCache graphicsCache, PivotFieldsAreaCellViewInfoBase fieldViewInfo, Rectangle bounds, Point offset) {
			bounds.X += offset.X;
			bounds.Y += offset.Y;
			AppearanceObject fieldValueAppearance = CalculateAppearance(fieldViewInfo);
			SetDefaultBrickStyle(fieldValueAppearance);
			ITextBrick brick = DrawTextBrick(fieldViewInfo.DisplayText, bounds, fieldViewInfo.GetPaddings());
			if(fieldViewInfo.IsLastFieldLevel)
			brick.Separable = false;
			else {
				brick.SeparableHorz = fieldViewInfo.IsColumn;
				brick.SeparableVert = !fieldViewInfo.IsColumn;
			}
			brick.VertAlignment = fieldViewInfo.Appearance.TextOptions.VAlignment;
			if(Owner != null) Owner.CustomDrawFieldValue(brick, fieldViewInfo);
		}		
		protected void DrawCells() {
			int dummy = 0;
			BrickStyle defBrick = this.graph.DefaultBrickStyle;
			CellsArea.CreateCellsViewInfo(Point.Empty, Point.Empty, new Point(int.MaxValue, int.MaxValue), null, null, null, out dummy, new EventHandler(OnCell));
			this.graph.DefaultBrickStyle = defBrick;
		}
		void OnCell(object cell, EventArgs a) {
			DrawCell((PivotCellViewInfo)cell);
		}
		protected void DrawCell(PivotCellViewInfo cellViewInfo) {
			AppearanceObject appearance = CalculateAppearance(cellViewInfo);
			Rectangle bounds = cellViewInfo.Bounds;
			bounds.X += ViewInfo.CellsArea.Bounds.X;
			bounds.Y += ViewInfo.CellsArea.Bounds.Y;
			IVisualBrick brick = DrawCellBrick(appearance, bounds, cellViewInfo);
			ApplyXlsExportNativeFormat(brick, cellViewInfo.DataField);
			SetBorderSides(brick, cellViewInfo);
			if(Owner != null)
				Owner.CustomDrawCell(brick, cellViewInfo);
		}
		void ApplyXlsExportNativeFormat(IVisualBrick brick, PivotGridFieldBase dataField) {
			ITextBrick textBrick = brick as ITextBrick;
			if(textBrick != null)
				textBrick.XlsExportNativeFormat = dataField != null ? dataField.UseNativeFormat : DefaultBoolean.Default;
		}
		IVisualBrick DrawCellBrick(AppearanceObject appearance, Rectangle bounds, PivotCellViewInfo cellViewInfo) {
			FormatInfo cellFormat = cellViewInfo.GetCellFormatInfo();
			string formatString = cellFormat == null ? "" : cellFormat.FormatString;
			IVisualBrick brick = null;			
			RepositoryItem rItem = cellViewInfo.Edit;
			if(rItem != null) {
				SetDefaultBrickStyle(appearance);
				PrintCellHelperInfo info = new PrintCellHelperInfo(
					Graph.DefaultBrickStyle.BorderColor,
					PS,
					cellViewInfo.Value,
					appearance,
					cellViewInfo.Text,
					bounds,
					Graph,
					appearance.HAlignment,
					CellsArea.ShowHorzLines,
					CellsArea.ShowVertLines,
					formatString);
				brick = DrawCellBrick(info, rItem, PivotCellViewInfoBase.GetPaddings());
			} else {
				if(cellViewInfo.ShowKPIGraphic) {
					SetDefaultBrickStyle(appearance);
					brick = DrawImageBrick(cellViewInfo, bounds, ((PivotGridViewInfoData)Data).GetKPIBitmap(cellViewInfo.KPIGraphic, cellViewInfo.KPIValue));
				} else {
					SetDefaultBrickStyle(appearance);
					brick = DrawTextBrick(cellViewInfo.Text, cellViewInfo.Value, bounds, formatString, PivotCellViewInfoBase.GetPaddings());
				}
			}
			return brick;
		}		
		void SetBorderSides(IVisualBrick brick, PivotCellViewInfo cellViewInfo) {
			if(!CellsArea.ShowHorzLines || !CellsArea.ShowVertLines) {
				BorderSide sides = BorderSide.None;
				if(cellViewInfo.RowIndex == 0) sides |= BorderSide.Top;
				if(cellViewInfo.RowIndex == CellsArea.RowCount - 1) sides |= BorderSide.Bottom;
				if(cellViewInfo.ColumnIndex == 0) sides |= BorderSide.Left;
				if(cellViewInfo.ColumnIndex == CellsArea.ColumnCount - 1) sides |= BorderSide.Right;
				if(CellsArea.ShowHorzLines) sides |= BorderSide.Top | BorderSide.Bottom;
				if(CellsArea.ShowVertLines) sides |= BorderSide.Left | BorderSide.Right;
				brick.Sides = sides;
			}
		}
		protected IImageBrick DrawImageBrick(PivotCellViewInfo cellViewInfo, Rectangle bounds, Bitmap bitmap) {
			IImageBrick brick = CreateImageBrick();
			brick.Image = bitmap;
			brick.SizeMode = PictureBoxSizeMode.CenterImage;
			Graph.DrawBrick(brick, bounds);
			ApplyPadding(brick, PivotCellViewInfoBase.GetPaddings());
			return brick;
		}
		protected ITextBrick DrawTextBrick(string text, Rectangle bounds, Padding paddings) {
			return DrawTextBrick(text, text, bounds, "", paddings);
		}
		protected virtual ITextBrick DrawTextBrick(string text, object textValue, Rectangle bounds, 
				string textValueFormatString, Padding paddings) {
			ITextBrick brick = CreateTextBrick();
			Graph.DrawBrick(brick, bounds);
			brick.Text = text;
			if(textValue != null)
				brick.TextValue = textValue;
			brick.TextValueFormatString = textValueFormatString;
			ApplyPadding(brick, paddings);
			return brick;
		}
		protected IVisualBrick DrawCellBrick(PrintCellHelperInfo info, RepositoryItem rItem, Padding paddings) {
			IVisualBrick brick = rItem.GetBrick(info);
			Graph.DrawBrick(brick, info.Rectangle);
			ApplyPadding(brick, paddings);
			return brick;
		}
		void SetDefaultBrickStyle(AppearanceObject appearance) {
			if(appearance == null)
				return;
			BrickStyle brickStyle = AppearanceHelper.CreateBrick(appearance, BorderSide.All, appearance.BorderColor, 1);
			brickStyle.TextAlignment = TextAlignmentConverter.ToTextAlignment(appearance.TextOptions.HAlignment, appearance.TextOptions.VAlignment);
			Graph.DefaultBrickStyle = brickStyle;
		}
		AppearanceObject CalculateAppearance(PivotHeaderViewInfoBase headerViewInfo) {
			AppearanceObject headerAppearance = (AppearanceObject)headerViewInfo.Appearance.Clone();
			headerAppearance.BorderColor = ViewInfo.LinesColor;
			return headerAppearance;
		}		
		AppearanceObject CalculateAppearance(PivotCellViewInfo cellViewInfo) {
			AppearanceObject cellAppearance = (AppearanceObject)cellViewInfo.Appearance.Clone();
			cellAppearance.BorderColor = ViewInfo.LinesColor;
			CellsArea.CombinWithSelectAppearance(cellViewInfo, cellAppearance);
			CellsArea.UpdateCellAppearanceHAlignment(cellAppearance);
			CellsArea.CustomAppearance(ref cellAppearance, cellViewInfo);
			return cellAppearance;
		}
		AppearanceObject CalculateAppearance(PivotFieldsAreaCellViewInfoBase fieldViewInfo) {
			AppearanceObject fieldValueAppearance = (AppearanceObject)fieldViewInfo.Appearance.Clone();
			fieldValueAppearance.BorderColor = ViewInfo.LinesColor;
			return fieldValueAppearance;
		}		
		protected void ApplyPadding(IVisualBrick brick, Padding paddings) {
			brick.Padding = new DevExpress.XtraPrinting.PaddingInfo(paddings.Left, paddings.Right, 
				paddings.Top, paddings.Bottom, GraphicsUnit.Pixel);
		}
		protected virtual ITextBrick CreateTextBrick() {
			ITextBrick brick = PS.CreateTextBrick();
			brick.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			return brick;
		}
		protected virtual IImageBrick CreateImageBrick() {
			return PS.CreateImageBrick();
		}
	}
}
