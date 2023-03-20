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
using DevExpress.Web.ASPxGridView.Export;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Utils;
using DevExpress.Web.ASPxClasses;
using System.Web.UI.WebControls;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxClasses.Internal;
using System.Web.UI;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.ASPxGridView.Export.Helper {
	public class ASPxGridViewPrintTextBuilder : ASPxGridViewTextBuilder {
		public ASPxGridViewPrintTextBuilder(ASPxGridView grid) : base(grid) { }
		protected override string GetEditorDisplayTextCore(EditPropertiesBase editor, GridViewDataColumn column, int visibleIndex, object value) {
			return editor.GetExportDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		public CreateDisplayControlArgs GetDisplayConrolArgs(GridViewDataColumn column, int visibleIndex) {
			return GetDisplayControlArgsCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
	}
	public class GridViewLinkWebStylePrintStyle {
		GridViewExportAppearance webStyle;
		BrickStyle brickStyle;
		GridViewColumn column;
		public GridViewLinkWebStylePrintStyle(GridViewExportAppearance webStyle, BrickStyle brickStyle, GridViewColumn column) {
			this.webStyle = webStyle;
			this.brickStyle = brickStyle;
			this.column = column;
		}
		public bool IsFit(GridViewExportAppearance webStyle, GridViewColumn column) {
			return WebStyle == webStyle && Column == column;
		}
		public GridViewExportAppearance WebStyle { get { return webStyle; } }
		public BrickStyle BrickStyle { get { return brickStyle; } }
		public GridViewColumn Column { get { return column; } }
	}
	public delegate void GridViewPrinterDrawDetailGrid(ASPxGridView detailGrid);
	public class GridViewPrinter : IWebControlPageSettings, IDisposable {
		ASPxGridViewExporter exporter;
		ASPxGridView grid;
		WebDataProxy dataProxy;
		List<GridViewColumn> columns;
		Dictionary<GridViewColumn, int> columnWidths;
		int graphBrickTop;
		GridViewExportStyles styles;
		ASPxGridViewPrintTextBuilder textBuilder;
		Graphics graphics;
		BrickGraphics psGraph;
		GridViewPrinterDrawDetailGrid onDrawDetailGrid;
		int level;
		int dataRowIndex;
		string savedFilterExpression = "";
		public GridViewPrinter(ASPxGridViewExporter exporter, ASPxGridView grid, GridViewExportStyles styles, Graphics graphics, BrickGraphics psGraph, GridViewPrinterDrawDetailGrid onDrawDetailGrid, int level) {
			this.exporter = exporter;
			this.grid = grid;
			this.styles = styles;
			this.onDrawDetailGrid = onDrawDetailGrid;
			this.textBuilder = new ASPxGridViewPrintTextBuilder(Grid);
			this.dataProxy = Grid.DataBoundProxy;
			this.columnWidths = new Dictionary<GridViewColumn, int>();
			this.graphics = graphics;
			this.psGraph = psGraph;
			this.level = level;
			this.dataRowIndex = 0;
			BeforeCreate();
		}
		protected ASPxGridViewExporter Exporter { get { return exporter; } }
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewPrintTextBuilder TextBuilder { get { return textBuilder; } }
		protected GridViewExportStyles Styles { get { return styles; } }
		protected Graphics Graphics { get { return graphics; } }
		protected BrickGraphics PSGraph { get { return psGraph; } }
		protected WebDataProxy DataProxy { get { return dataProxy; } }
		protected GridViewPrinterDrawDetailGrid OnDrawDetailGrid { get { return onDrawDetailGrid; } }
		protected int Level { get { return level; } }
		protected bool PrintSelectCheckBox { get { return Exporter.PrintSelectCheckBox; } } 
		protected bool ShowColumnHeaders { get { return Grid.Settings.ShowColumnHeaders; } }
		protected bool ShowTitle { get { return Grid.Settings.ShowTitlePanel && !string.IsNullOrEmpty(TitleText); } }
		protected bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		protected string TitleText { get { return Grid.SettingsText.Title; } }
		protected string GetFooterText(GridViewColumn  column) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if (dataColumn == null) return string.Empty;
			return TextBuilder.GetFooterCaption(dataColumn, "\xd\xa");
		}
		protected string GetGroupFooterText(GridViewColumn column, int rowIndex) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if (dataColumn == null) return string.Empty;
			return TextBuilder.GetGroupRowFooterText(dataColumn, rowIndex, "\xd\xa");
		}
		protected string GetGroupRowText(int rowIndex) {
			return TextBuilder.GetGroupRowText(Grid.GetSortedColumns()[DataProxy.GetRowLevel(rowIndex)], rowIndex);
		}
		protected List<int> GetGroupFooterVisibleIndexes(int visibleIndex) {
			if(Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.Hidden) return null;
			return DataProxy.GetGroupFooterVisibleIndexes(visibleIndex, Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.VisibleIfExpanded);
		}
		protected List<GridViewColumn> Columns {
			get {
				if(this.columns == null) {
					this.columns = CreateColumns();
				}
				return columns;
			}
		}
		protected void BeforeCreate() {
			DataProxy.PrinterPageSettings = this;
			this.savedFilterExpression = Grid.FilterExpression;
			if(!Exporter.PreserveGroupRowStates || Exporter.ExportedRowType == GridViewExportedRowType.Selected) {
				DataProxy.ExpandAll();
			}
			FilterOutNonSelectedRows();
			CalcBrickSizes();
		}
		protected int GetColumnWidth(GridViewColumn column) {
			return this.columnWidths[column];
		}
		protected int GetHeaderFooterWidth(GridViewColumn column) {
			int width = GetColumnWidth(column);
			if (Columns.IndexOf(column) == 0) {
				width += GetGroupLevelOffSet(Grid.GroupCount);
			}
			return width;
		}
		int maxHeaderHeight = 0;
		int maxFooterHeight = 0;
		Dictionary<int, int> rowsHeight;
		Dictionary<int, int> groupFooterRowsHeight;
		protected void FilterOutNonSelectedRows() {			
			if(Exporter.ExportedRowType != GridViewExportedRowType.Selected) return;
			if(Level > 0 && Grid.Selection.Count < 1) return; 
			if(Grid.Selection.Count < 1) {
				Grid.FilterExpression = "false";
			} else {
				CriteriaOperator
					gridCriteria = CriteriaOperator.Parse(this.savedFilterExpression),
					selectionCriteria = new InOperator(Grid.KeyFieldName, Grid.GetSelectedFieldValues(Grid.KeyFieldName));
				Grid.FilterExpression = (object.ReferenceEquals(gridCriteria, null) || !Grid.FilterEnabled
					? selectionCriteria
					: GroupOperator.Combine(GroupOperatorType.And, gridCriteria, selectionCriteria)).ToString();
			}
		}
		protected void CalcBrickSizes() {
			this.maxHeaderHeight = GetColumnSize("Wg", Styles.Header).Height;
			rowsHeight = new Dictionary<int, int>();
			groupFooterRowsHeight = new Dictionary<int, int>();
			foreach(GridViewColumn column in Columns) {
			   this.columnWidths[column] = CalcColumnWidth(column);
			}
		}
		protected virtual WebRowType GetRowType(int rowIndex) {
			return DataProxy.GetRowType(rowIndex);
		}
		protected virtual int GetRowLevel(int rowIndex) {
			return DataProxy.GetRowLevel(rowIndex);
		}
		protected virtual int CalcColumnWidth(GridViewColumn column) {
			int maxWidth = CalcHeaderWidth(column);
			if(maxWidth > Exporter.MaxColumnWidth) return Exporter.MaxColumnWidth;
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {				
				maxWidth = Math.Max(maxWidth, CalcGroupFootersWidth(column, i));
				if(GetRowType(i) != WebRowType.Data) continue;
				maxWidth = Math.Max(maxWidth, CalcCellWidth(column, i));
			}
			return Math.Max(maxWidth, CalcFooterWidth(column));
		}
		protected virtual int CalcHeaderWidth(GridViewColumn column) {
			if (!ShowColumnHeaders) return ASPxGridViewExporter.MinColumnWidth;
			Size size = GetColumnSize(TextBuilder.GetHeaderCaption(column), Styles.Header);
			this.maxHeaderHeight = Math.Max(this.maxHeaderHeight, size.Height);
			return Math.Max(ASPxGridViewExporter.MinColumnWidth, size.Width);
		}
		protected virtual int CalcCellWidth(GridViewColumn column, int rowIndex) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if (dataColumn == null) {
				return 12; 
			}
			Size size = GetColumnSize(TextBuilder.GetRowDisplayText(dataColumn, rowIndex), Styles.Cell);
			UpdateRowsHeight(this.rowsHeight, rowIndex, size);
			return size.Width;
		}
		protected virtual int CalcGroupFootersWidth(GridViewColumn column, int rowIndex) {
			List<int> indexes = GetGroupFooterVisibleIndexes(rowIndex);
			if(indexes == null) return 0;
			int maxWidth = 0;
			for(int i = 0; i < indexes.Count; i++) {
				maxWidth = Math.Max(maxWidth, CalcGroupFooterWidth(column, indexes[i]));
			}
			return maxWidth;
		}
		protected virtual int CalcGroupFooterWidth(GridViewColumn column, int parentGroupVisibleIndex) {
			Size size = GetColumnSize(GetGroupFooterText(column, parentGroupVisibleIndex), Styles.GroupFooter);
			UpdateRowsHeight(this.groupFooterRowsHeight, parentGroupVisibleIndex, size);
			return size.Width;
		}
		void UpdateRowsHeight(Dictionary<int, int> heights, int index, Size size) {
			if(!heights.ContainsKey(index)) heights[index] = 0;
			heights[index] = Math.Max(heights[index], size.Height);
		}
		protected virtual int CalcFooterWidth(GridViewColumn column) {
			if (!ShowFooter) return ASPxGridViewExporter.MinColumnWidth;
			string text = GetFooterText(column);
			if (string.IsNullOrEmpty(text)) return ASPxGridViewExporter.MinColumnWidth;
			Size size = GetColumnSize(text, Styles.Header);
			this.maxFooterHeight = Math.Max(this.maxFooterHeight, size.Height);
			return Math.Max(ASPxGridViewExporter.MinColumnWidth, size.Width);
		}
		Size GetColumnSize(string text, GridViewExportAppearance style) {
			return GetTextSize(text, style, Exporter.MaxColumnWidth);
		}
		Size GetTextSize(string text, GridViewExportAppearance style, int maxWidth) {
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(PSGraph, style);
			int horPaddings = brickStyle.Padding.Left + brickStyle.Padding.Right;
			maxWidth -= horPaddings;
			SizeF sizeF = DevExpress.XtraPrinting.Native.Measurement.MeasureString(text, brickStyle.Font, maxWidth, brickStyle.StringFormat.Value, PSGraph.PageUnit);
			RectangleF rect = new RectangleF(PointF.Empty, sizeF);
			rect = brickStyle.InflateBorderWidth(rect, GraphicsDpi.Pixel);
			Size size = Size.Ceiling(rect.Size);
			size.Width += horPaddings;
			size.Height += brickStyle.Padding.Top + brickStyle.Padding.Bottom;
			return size;
		}
		protected int GetHeaderHeight() {
			return maxHeaderHeight; 
		}
		protected int GetFooterHeight() {
			return maxFooterHeight;
		}
		protected int GetGroupRowWidth(int rowIndex) {
			return GridWidth - GetGroupLevelOffSetByRowIndex(rowIndex);
		}
		protected int GetGroupHeight(int rowIndex) {
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(PSGraph, Styles.GroupRow);
			int offset = brickStyle.Padding.Left + brickStyle.Padding.Right; 
			return GetTextSize(GetGroupRowText(rowIndex), Styles.GroupRow, GetGroupRowWidth(rowIndex) - offset).Height;
		}
		protected int GetRowHeight(int rowIndex) {
			if(GetRowType(rowIndex) == WebRowType.Group) return GetGroupHeight(rowIndex);
			if(!this.rowsHeight.ContainsKey(rowIndex)) return 0; 
			return this.rowsHeight[rowIndex];
		}
		protected int GetGroupFooterHeight(int rowIndex) {
			return this.groupFooterRowsHeight[rowIndex];
		}
		protected int GetGroupLevelOffSet(int groupLevel) {
			return groupLevel * 10; 
		}
		protected int GetGroupLevelOffSetByRowIndex(int rowIndex) {
			return GetGroupLevelOffSet(GetRowLevel(rowIndex));
		}
		int gridWidth = -1;
		protected int GridWidth {
			get {
				if(this.gridWidth < 0) {
					this.gridWidth = GetGroupLevelOffSet(Grid.GroupCount);
					foreach(GridViewColumn column in Columns) {
						this.gridWidth += GetColumnWidth(column);
					}
				}
				return this.gridWidth;
			}
		}
		protected List<GridViewColumn> CreateColumns() {
			List<GridViewColumn> list = new List<GridViewColumn>();
			foreach (GridViewColumn column in Grid.GetColumnsShownInHeaders()) {
				GridViewDataColumn dataColumn = column as GridViewDataColumn;
				if (dataColumn != null) {
					list.Add(dataColumn);
				}
				if (PrintSelectCheckBox) {
					GridViewCommandColumn commandColumn = column as GridViewCommandColumn;
					if (commandColumn != null && commandColumn.ShowSelectCheckbox) {
						list.Add(commandColumn);
					}
				}
			}
			return list;
		}
		public void CreateDetailHeader(BrickGraphics graph) {
			SetupGraphBrickTop();
			DrawTitle(graph);
			DrawHeaders(graph);
		}
		public void CreateDetail(BrickGraphics graph) {
			SetupGraphBrickTop();
			DrawRows(graph);
			DrawFooter(graph);
		}
		void SetupGraphBrickTop() {
			this.graphBrickTop = this.graphBrickTop == 0 && !string.IsNullOrEmpty(Exporter.ReportHeader) ? 1 : 0;
		}
		protected virtual void DrawTitle(BrickGraphics graph) {
			if(!ShowTitle) return;
			BrickStyle defBrick = graph.DefaultBrickStyle;
			string text = TitleText;
			GridViewExportAppearance style = new GridViewExportAppearance();
			style.CopyFrom(Styles.Title);
			style.HorizontalAlign = GetHorizontalTitleAlignment();
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(PSGraph, Styles.Title);
			int offset = brickStyle.Padding.Left + brickStyle.Padding.Right + 2; 
			int height = GetTextSize(text, Styles.Preview, GridWidth - offset).Height;
			DrawTextBrick(graph, text, style, 0, GridWidth, height, null, -1, GridViewRowType.Title);
			this.graphBrickTop += height;
			graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawHeaders(BrickGraphics graph) {
			if (!ShowColumnHeaders) return;
			BrickStyle defBrick = graph.DefaultBrickStyle;
			int left = 0;
			foreach(GridViewColumn column in Columns) {
				DrawHeader(graph, column, ref left);
			}
			this.graphBrickTop += GetHeaderHeight();
			graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawHeader(BrickGraphics graph, GridViewColumn column, ref int left) {
			int width = GetHeaderFooterWidth(column);
			DrawTextBrick(graph, TextBuilder.GetHeaderCaption(column),
				Styles.Header, left, width, GetHeaderHeight(), column, -1, GridViewRowType.Header);
			left += width;
		}
		protected virtual void DrawRows(BrickGraphics graph) {
			BrickStyle defBrick = graph.DefaultBrickStyle;
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {				
				if(GetRowType(i) == WebRowType.Group)
					DrawGroupRow(graph, i);
				else DrawDataRow(graph, i);
				this.graphBrickTop += GetRowHeight(i);
				DrawDetailRow(graph, i);
				DrawPreview(graph, i);
				DrawGroupFooters(graph, i);
			}
			graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawGroupRow(BrickGraphics graph, int rowIndex) {
			int left = GetGroupLevelOffSetByRowIndex(rowIndex);
			DrawTextBrick(graph, GetGroupRowText(rowIndex),
				Styles.GroupRow, left, GetGroupRowWidth(rowIndex), GetRowHeight(rowIndex), null, rowIndex, GridViewRowType.Group);
		}
		protected virtual void DrawDataRow(BrickGraphics graph, int rowIndex) {
			int left = GetGroupLevelOffSet(GetRowLevel(rowIndex));
			foreach(GridViewColumn column in Columns) {
				DrawCell(graph, rowIndex, column, ref left);
			}
			this.dataRowIndex++;
		}
		protected virtual void DrawGroupFooters(BrickGraphics graph, int rowIndex) {
			List<int> indexes = GetGroupFooterVisibleIndexes(rowIndex);
			if(indexes == null) return;
			for(int i = 0; i < indexes.Count; i++) {
				DrawGroupFooter(graph, indexes[i]);
			}
		}
		protected virtual void DrawGroupFooter(BrickGraphics graph, int parentGroupVisibleIndex) {
			int left = GetGroupLevelOffSet(GetRowLevel(parentGroupVisibleIndex) + 1);
			foreach(GridViewColumn column in Columns) {
				DrawGroupFooterCell(graph, parentGroupVisibleIndex, column, ref left);
			}
			this.graphBrickTop += GetGroupFooterHeight(parentGroupVisibleIndex);
		}
		protected virtual void DrawFooter(BrickGraphics graph) {
			if (!ShowFooter || GetFooterHeight() <= 0) return;
			BrickStyle defBrick = graph.DefaultBrickStyle;
			int left = 0;
			foreach (GridViewColumn column in Columns) {
				DrawFooterCell(graph, column, ref left);
			}
			this.graphBrickTop += GetFooterHeight();
			graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawFooterCell(BrickGraphics graph, GridViewColumn column, ref int left) {
			int width = GetHeaderFooterWidth(column);
			DrawTextBrick(graph, GetFooterText(column), Styles.Footer, left, width, GetFooterHeight(), column, -1, GridViewRowType.Footer);
			left += width;
		}
		protected virtual void DrawCell(BrickGraphics graph, int rowIndex, GridViewColumn column, ref int left) {
			int width = GetColumnWidth(column);
			GridViewExportAppearance style = new GridViewExportAppearance();
			style.CopyFrom(Styles.Cell);
			if (this.dataRowIndex % 2 == 1 && IsAltRowStyleEnabled())
				style.CopyFrom(Styles.AlternatingRowCell);
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if (dataColumn == null) {
				DrawSelectedCheckBox(graph, rowIndex, style, width, left);
			} else {
				DrawDataCell(graph, rowIndex, dataColumn, style, width, left);
			}
			left += width;
		}
		protected virtual void DrawSelectedCheckBox(BrickGraphics graph, int rowIndex, GridViewExportAppearance style, int width, int left) {
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(graph, style);
			graph.DefaultBrickStyle = brickStyle;
			CheckBoxBrick checkBoxBrick = new CheckBoxBrick();
			checkBoxBrick.Checked = DataProxy.Selection.IsRowSelected(rowIndex);
			DrawBrickCore(graph, checkBoxBrick, left, width, GetRowHeight(rowIndex));
		}
		protected virtual void DrawDataCell(BrickGraphics graph, int rowIndex, GridViewDataColumn dataColumn, 
			GridViewExportAppearance style, int width, int left) {
			CreateDisplayControlArgs args = TextBuilder.GetDisplayConrolArgs(dataColumn, rowIndex);
			object textValue = null;
			string textValueFormatString = string.Empty;
			string url = string.Empty;
			if (dataColumn.PropertiesEdit != null) {
				textValue = dataColumn.PropertiesEdit.GetExportValue(args);
				textValueFormatString = dataColumn.PropertiesEdit.DisplayFormatString;
				url = dataColumn.PropertiesEdit.GetExportNavigateUrl(args);
			}
			if (!string.IsNullOrEmpty(url))
				style.CopyFrom(Styles.HyperLink);
			DrawTextBrick(graph, TextBuilder.GetRowDisplayText(dataColumn, rowIndex), textValue, textValueFormatString, url,
				style, left, width, GetRowHeight(rowIndex), dataColumn, rowIndex, GridViewRowType.Data);
		}
		bool IsAltRowStyleEnabled() {
			switch(Styles.AlternatingRowCell.Enabled) {
				case DevExpress.Web.ASPxClasses.DefaultBoolean.False:
					return false;
				case DevExpress.Web.ASPxClasses.DefaultBoolean.True:
					return true;
				default:
					return Grid.Styles.AlternatingRow.Enabled == DevExpress.Web.ASPxClasses.DefaultBoolean.True
						|| !Grid.Styles.AlternatingRow.IsEmpty;
			}
		}
		protected virtual void DrawGroupFooterCell(BrickGraphics graph, int rowIndex, GridViewColumn column, ref int left) {
			int width = GetColumnWidth(column);
			DrawTextBrick(graph, GetGroupFooterText(column, rowIndex),
				Styles.GroupFooter, left, width, GetGroupFooterHeight(rowIndex), column, rowIndex, GridViewRowType.GroupFooter);
			left += width;
		}
		protected virtual void DrawPreview(BrickGraphics graph, int rowIndex) {
			if(!Grid.Settings.ShowPreview) return;
			if(DataProxy.GetRowType(rowIndex) != WebRowType.Data) return;
			string text = TextBuilder.GetPreviewText(rowIndex);
			if(string.IsNullOrEmpty(text)) return;
			int left = GetGroupLevelOffSetByRowIndex(rowIndex);
			int width = GridWidth - left;
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(PSGraph, Styles.Preview);
			int offset = brickStyle.Padding.Left + brickStyle.Padding.Right + 2; 
			int height = GetTextSize(text, Styles.Preview, width - offset).Height;
			DrawTextBrick(graph, text, Styles.Preview, left, width, height, null, rowIndex, GridViewRowType.Preview);
			this.graphBrickTop += height;
		}
		protected virtual void DrawDetailRow(BrickGraphics graph, int rowIndex) {
			if(Grid.SettingsDetail.ExportMode == GridViewDetailExportMode.None
				|| Grid.Templates.DetailRow == null
				|| Grid.Page == null
				|| DataProxy.GetRowType(rowIndex) != WebRowType.Data) return;
			if(Grid.DetailRows.IsVisible(rowIndex)) {
				Control container = Grid.FindControl(ASPxGridViewRenderHelper.DXDetailRowString + rowIndex.ToString());
				DrawDetailRowCore(container);
			} else if(Grid.SettingsDetail.ExportMode == GridViewDetailExportMode.All) {
				Control parent = Grid.Parent;
			using(GridViewDetailRowTemplateContainer container = new GridViewDetailRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(rowIndex), rowIndex)) {
				container.ID = "dxPrinterLink" + Grid.ID;
				Grid.Templates.DetailRow.InstantiateIn(container);
				parent.Controls.Add(container);
					DrawDetailRowCore(container);					
					parent.Controls.Remove(container);
				}
				}
			}
		void DrawDetailRowCore(Control container) {
			List<ASPxGridView> grids = new List<ASPxGridView>();
			FindDetailGrids(container, grids);
			grids.Sort(DoDetailGridCompare);
			foreach(ASPxGridView grid in grids) {
				grid.DataBind();
				DrawDetailGrid(grid);
		}
		}
		void FindDetailGrids(Control parent, List<ASPxGridView> list) {
			if(parent is ASPxGridView) {
				ASPxGridView grid = parent as ASPxGridView;
				if(grid.SettingsDetail.ExportIndex > -1) {
					list.Add(parent as ASPxGridView);
				}
				return;
			}
			for(int i = 0; i < parent.Controls.Count; i++) {
				FindDetailGrids(parent.Controls[i], list);
			}
		}
		int DoDetailGridCompare(ASPxGridView g1, ASPxGridView g2) {
			return Comparer<int>.Default.Compare(g1.SettingsDetail.ExportIndex, g2.SettingsDetail.ExportIndex);
		}
		protected virtual void DrawDetailGrid(ASPxGridView detailGrid) {
			if(OnDrawDetailGrid != null) {
				this.graphBrickTop = Exporter.DetailVerticalOffset;
				OnDrawDetailGrid(detailGrid);
			}
		}
		protected void DrawTextBrick(BrickGraphics graph, string text, GridViewExportAppearance style,
			int left, int width, int height, GridViewColumn column,
			int visibleIndex, GridViewRowType rowType) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			DrawTextBrick(graph, text, null, string.Empty, string.Empty, style, left, width, height, dataColumn, visibleIndex, rowType);
		}
		protected void DrawTextBrick(BrickGraphics graph, string text, object textValue, string textValueFormatString,
			string url, GridViewExportAppearance style,
			int left, int width, int height, GridViewDataColumn column,
			int visibleIndex, GridViewRowType rowType) {
			if(object.Equals(textValue, DateTime.MinValue)) textValue = string.Empty; 
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(graph, style, column);
			ASPxGridViewExportRenderingEventArgs e = Exporter.RaiseRenderBrick(visibleIndex, column, rowType, DataProxy, brickStyle, text, textValue, textValueFormatString, url);
			if(e != null) {
				brickStyle = e.BrickStyle;
				text = e.Text;
				textValue = e.TextValue;
				textValueFormatString = e.TextValueFormatString;
				url = e.Url;
			}
			graph.DefaultBrickStyle = brickStyle;
			TextBrick brick = new TextBrick();
			brick.Url = url;
			brick.Text = text;
			brick.TextValue = textValue;
			brick.TextValueFormatString = textValueFormatString;
			DrawBrickCore(graph, brick, left, width, height);
		}
		protected void DrawBrickCore(BrickGraphics graph, Brick brick, int left, int width, int height) {
			RectangleF bounds = new RectangleF(left + Level * Exporter.DetailHorizontalOffset , this.graphBrickTop, width, height);
			PSGraph.DrawBrick(brick, bounds);
		}
		HorizontalAlign GetHorizontalTitleAlignment() {
			if(Styles.Title.HorizontalAlign != HorizontalAlign.NotSet) return Styles.Title.HorizontalAlign;
			if(Grid.Styles.TitlePanel.HorizontalAlign != HorizontalAlign.NotSet) return Grid.Styles.TitlePanel.HorizontalAlign;
			return HorizontalAlign.Center;
		}
		HorizontalAlign GetHorizontalAlignment(GridViewExportAppearance style, GridViewColumn column) {
			if(style.HorizontalAlign == HorizontalAlign.NotSet && column != null) {
				HorizontalAlign horzAligment = TextBuilder.GetColumnDisplayControlDefaultAlignment(column);
				if (horzAligment != HorizontalAlign.NotSet)
					return horzAligment;
			}
			return style.HorizontalAlign;
		}
		BrickStyle CreateBrickStyleByExportStyle(BrickGraphics graph, GridViewExportAppearance style, GridViewColumn column) {
			BrickStyle brickStyle = CreateBrickStyleByExportStyle(graph, style);
			brickStyle.SetAlignment(GetBrickHorzAlignment(GetHorizontalAlignment(style, column)), GetBrickVertAlignment(style.VerticalAlign));
			return brickStyle;
		}
		BrickStyle CreateBrickStyleByExportStyle(BrickGraphics graph, GridViewExportAppearance style) {
			BrickStyle brickStyle = new BrickStyle();
			brickStyle.Font = CreateFontByFontInfo(graph.DefaultFont, style.Font);
			brickStyle.BorderColor = style.BorderColor;
			brickStyle.BorderWidth = style.BorderWidth;
			brickStyle.BorderStyle = BrickBorderStyle.Center;
			brickStyle.Sides = style.BorderSides;
			brickStyle.BackColor = style.BackColor.IsEmpty ? graph.BackColor : style.BackColor;
			brickStyle.ForeColor = style.ForeColor.IsEmpty ? graph.ForeColor : style.ForeColor;
			if(!style.Paddings.IsEmpty) {
				brickStyle.Padding = CreatePaddingByStylePadding(style.Paddings);
			}
			return brickStyle;
		}
		PaddingInfo CreatePaddingByStylePadding(Paddings paddings) {
			int left = GetPaddingValue(paddings.PaddingLeft, paddings.Padding);
			int right = GetPaddingValue(paddings.PaddingRight, paddings.Padding);
			int top = GetPaddingValue(paddings.PaddingTop, paddings.Padding);
			int bottom = GetPaddingValue(paddings.PaddingBottom, paddings.Padding);
			return new PaddingInfo(left, right, top, bottom);
		}
		int GetPaddingValue(Unit unit, Unit commonUnit) {
			if(unit.IsEmpty) unit = commonUnit;
			if(unit.IsEmpty || unit.Type != UnitType.Pixel) return 0;
			return (int)unit.Value;
		}
		float GetBorderWidth(Unit unit, float defaultWidth) {
			if(unit.IsEmpty || unit.Type != UnitType.Pixel) return defaultWidth;
			return (float)unit.Value;
		}
		public static Font CreateFontByFontInfo(Font defaultFont, FontInfo fontInfo) {
			FontStyle fontStyle = FontStyle.Regular;
			if(fontInfo.Bold) fontStyle |= FontStyle.Bold;
			if(fontInfo.Italic) fontStyle |= FontStyle.Italic;
			if(fontInfo.Strikeout) fontStyle |= FontStyle.Strikeout;
			if(fontInfo.Underline) fontStyle |= FontStyle.Underline;
			float emSize = defaultFont.Size;
			string familyName = string.IsNullOrEmpty(fontInfo.Name) ? defaultFont.Name : fontInfo.Name;
			if(!fontInfo.Size.IsEmpty)
				emSize = GetFontSize(fontInfo.Size, emSize);
			return new Font(familyName, emSize, fontStyle);
		}
		static float GetFontSize(FontUnit size, float defaultSize) {
			if(size.Type == FontSize.NotSet || size.Type == FontSize.Medium) return defaultSize;
			if(size.Type == FontSize.AsUnit && !size.Unit.IsEmpty)
				return (float)size.Unit.Value;
			int rank = 10, defaultRank = 10;
			switch(size.Type) {
				case FontSize.Large: rank = 14; break;
				case FontSize.Larger: rank = 16; break;
				case FontSize.XLarge: rank = 20; break;
				case FontSize.XXLarge: rank = 24; break;
				case FontSize.Small: rank = 8; break;
				case FontSize.Smaller: rank = 6; break;
				case FontSize.XSmall: rank = 5; break;
				case FontSize.XXSmall: rank = 4; break;
			}
			return defaultSize * rank / defaultRank;
		}
		HorzAlignment GetBrickHorzAlignment(HorizontalAlign align) {
			switch (align) {
				case HorizontalAlign.Right: return HorzAlignment.Far;
				case HorizontalAlign.Center: return HorzAlignment.Center;
			}
			return HorzAlignment.Near;
		}
		VertAlignment GetBrickVertAlignment(VerticalAlign align) {
			switch (align) {
				case VerticalAlign.Bottom: return VertAlignment.Bottom;
				case VerticalAlign.Top: return VertAlignment.Top;
			}
			return VertAlignment.Center;
		}
		int IWebControlPageSettings.PageSize { get { return int.MaxValue; } }
		int IWebControlPageSettings.PageIndex { get { return 0; } set {} }
		GridViewPagerMode IWebControlPageSettings.PagerMode { get { return GridViewPagerMode.ShowAllRecords; }}
		#region IDisposable Members
		public void Dispose() {
			DataProxy.PrinterPageSettings = null;
			Grid.FilterExpression = this.savedFilterExpression;
		}
		#endregion
	}
}
