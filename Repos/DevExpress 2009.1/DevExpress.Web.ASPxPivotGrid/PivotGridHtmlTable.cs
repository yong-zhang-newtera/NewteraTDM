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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Web.ASPxEditors.FilterControl;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlTable : InternalTable {
		ASPxPivotGrid pivotGrid;
		PivotGridPagerContainer topPagerContainer;
		WebFilterControlPopupRow prefilterPanel;
		InternalTableCell prefilterPanelContainer;
		public PivotGridHtmlTable(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotWebVisualItems VisualItems { get { return Data.VisualItems; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		public ASPxPivotGridPager TopPager { get { return topPagerContainer != null ? topPagerContainer.Pager : null; } }
		protected WebFilterControlPopupRow PrefilterPanel { get { return prefilterPanel; } }
		protected InternalTableCell PrefilterPanelContainer { get { return prefilterPanelContainer; } }
		protected int ColumnCount { 
			get { return VisualItems.GetLevelCount(false) + VisualItems.GetLastLevelItemCount(true); } 
		}
		protected int ColumnAreaLevelCount { 
			get { return VisualItems.GetLevelCount(true); }
		}
		protected int RowAreaLevelCount {
			get { return VisualItems.GetLevelCount(false); }
		}
		protected bool HasTopPager {
			get {
				return (PivotGrid.OptionsPager.Position == PagerPosition.Top ||
						PivotGrid.OptionsPager.Position == PagerPosition.TopAndBottom) &&
					PivotGrid.OptionsPager.RowsPerPage > 0;
			}
		}
		protected bool HasBottomPager {
			get {
				return (PivotGrid.OptionsPager.Position == PagerPosition.Bottom ||
						PivotGrid.OptionsPager.Position == PagerPosition.TopAndBottom) &&
					PivotGrid.OptionsPager.RowsPerPage > 0;
			}
		}
		protected override void CreateControlHierarchy() {
			Data.EnsureFieldCollections();
			CreateTopPager();
			CreateFilterHeaders();
			if(Data.OptionsView.ShowDataHeaders || Data.OptionsView.ShowColumnHeaders)
				CreateDataAndColumnHeaders();
			CreateColumnFieldsAndRowHeaders();
			CreateRowFieldsAndDataCells();
			CreateBottomPager();
			CreatePrefilterPanel();
		}		
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			this.CellPadding = 0;
			this.CellSpacing = 0;
			Data.GetTableStyle().AssignToControl(this);
			RenderUtils.SetStyleStringAttribute(this, "border-collapse", "separate");
			this.GridLines = GridLines.Both;
			Caption = PivotGrid.Caption;
			RenderUtils.SetStringAttribute(this, "summary", PivotGrid.SummaryText);
			PreparePrefilterPanel();
		}
		void PreparePrefilterPanel() {
			if(PrefilterPanelContainer == null) return;
			Data.Styles.GetPrefilterPanelContainerStyle().AssignToControl(PrefilterPanelContainer);
		}
		void CreateTopPager() {
			if(HasTopPager)
				CreatePagerCore(true);
		}
		void CreateBottomPager() {
			if(HasBottomPager)
				CreatePagerCore(false);
		}		
		void CreatePagerCore(bool isTopPager) {
			if(!PivotGrid.OptionsPager.Visible || PivotGrid.OptionsPager.RowsPerPage >= VisualItems.UnpagedRowCount) 
				return;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			PivotGridPagerContainer cell = new PivotGridPagerContainer(this, PivotGrid, isTopPager);
			cell.ColumnSpan = ColumnCount;
			row.Controls.Add(cell);
			if(isTopPager)
				topPagerContainer = cell;
		}
		void CreatePrefilterPanel() {
			if(Data.Prefilter.IsEmpty || !Data.OptionsCustomization.AllowPrefilter) return;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			this.prefilterPanelContainer = new InternalTableCell();
			PrefilterPanelContainer.ColumnSpan = ColumnCount;
			row.Cells.Add(PrefilterPanelContainer);
			this.prefilterPanel = new WebFilterControlPopupRow(PivotGrid);
			PrefilterPanelContainer.Controls.Add(PrefilterPanel);
		}
		void CreateFilterHeaders() {
			if(!Data.OptionsView.ShowFilterHeaders) return;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			AddHeader(row, PivotArea.FilterArea, ColumnCount);
		}	
		void CreateDataAndColumnHeaders() {
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			AddHeader(row, PivotArea.DataArea, RowAreaLevelCount);
			AddHeader(row, PivotArea.ColumnArea, VisualItems.ColumnCount);
		}
		void AddHeader(InternalTableRow row, PivotArea area, int colSpan) {
			PivotGridHtmlAreaCellContainerBase cell = PivotGridHtmlAreaCellContainerBase.Create(Data, area);
			cell.ColumnSpan = colSpan;
			row.Controls.Add(cell);
		}
		InternalTableRow[] CreateRows(int count) {
			InternalTableRow[] rows = new InternalTableRow[count];
			for(int i = 0; i < rows.Length; i++) {
				rows[i] = new InternalTableRow();
				Rows.Add(rows[i]);
			}
			return rows;
		}
		void CreateColumnFieldsAndRowHeaders() {
			InternalTableRow[] rows = CreateRows(ColumnAreaLevelCount);
			CreateRowHeaders();
			int itemCount = VisualItems.GetItemCount(true);
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(true, i);
				rows[item.Level].Controls.Add(
					new PivotGridHtmlColumnFieldCell(Data, item, VisualItems.GetSortedBySummaryFields(true, i))
				);
			}
		}
		void CreateRowHeaders() {
			InternalTableRow row = (InternalTableRow)Rows[Rows.Count - ColumnAreaLevelCount];
			row.ID = Data.GetHeaderTableID(PivotArea.RowArea);
			int rowSpan = ColumnAreaLevelCount > 1 ? ColumnAreaLevelCount : 1;
			if(Data.GetFieldCountByArea(PivotArea.RowArea) == 0) {
				PivotGridHtmlEmptyAreaCellContainer rowAreaContainer = new PivotGridHtmlEmptyAreaCellContainer(Data, PivotArea.RowArea);
				row.Controls.Add(rowAreaContainer);
				rowAreaContainer.RowSpan = rowSpan;
				if(RowAreaLevelCount > 1)
					rowAreaContainer.ColumnSpan = RowAreaLevelCount;
			} else {
				PivotGridField[] fields = Data.GetFieldsByArea(PivotArea.RowArea);
				for(int i = 0; i < fields.Length; i++) {
					PivotGridHtmlRowHeaderContainer headerContainer = new PivotGridHtmlRowHeaderContainer(Data, fields[i], rowSpan);
					row.Controls.Add(headerContainer);
				}
				for(int i = fields.Length; i < RowAreaLevelCount; i++) {
					EmptyPivotGridHtmlRowHeaderContainer emptyCell = new EmptyPivotGridHtmlRowHeaderContainer(Data, rowSpan);
					row.Controls.Add(emptyCell);
				}
			}
		}
		void CreateRowFieldsAndDataCells() {
			InternalTableRow[] rows = CreateRows(VisualItems.GetLastLevelItemCount(false));
			CreateRowFieldsAndDataCells(rows);
		}
		void CreateRowFieldsAndDataCells(InternalTableRow[] rows) {
			int rowIndex = 0, itemCount = VisualItems.GetItemCount(false);
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(false, i);
				rows[rowIndex].Controls.Add(
					new PivotGridHtmlRowFieldCell(Data, item, VisualItems.GetSortedBySummaryFields(false, i))
				);
				if(item.IsLastFieldLevel) {
					CreateDataCells(rows[rowIndex], item, rowIndex);
					rowIndex++;
				}
			}
		}
		void CreateDataCells(InternalTableRow row, PivotFieldValueItem rowItem, int rowIndex) {
			int lastLevelItemCount = VisualItems.GetLastLevelItemCount(true);
			for(int i = 0; i < lastLevelItemCount; i++) {
				row.Controls.Add(
					new PivotGridHtmlDataCell(Data, VisualItems.CreateCellItem(VisualItems.GetLastLevelItem(true, i), 
						rowItem, i, rowIndex)
				));
			}
		}
	}
	internal class PivotGridHtmlGroupHeader : InternalTable {
		List<Image> fSeparators = new List<Image>();
		PivotGridWebData data;
		PivotGridGroup group;
		InternalTableRow Row;
		public PivotGridHtmlGroupHeader(PivotGridWebData data, PivotGridGroup group){
			this.data = data;
			this.group = group;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected PivotGridGroup Group { get { return group; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = Data.GetGroupHeaderID(Group);
			Row = new InternalTableRow();
			Controls.Add(Row);
			PopulateHeaders();
			Attributes.Add("onmousedown", Data.GetHeaderMouseDown());
		}
		void PopulateHeaders() {
			foreach(PivotGridField field in Group.Fields) {
				if(!Group.IsFieldVisible(field))
					continue;
				Row.Controls.Add(CreateHeaderCell(Data, field));
				if(field.IsNextVisibleFieldInSameGroup)
					Row.Controls.Add(GetHorizontalLine());
			}
		}
		InternalTableCell CreateHeaderCell(PivotGridWebData data, PivotGridField field) {
			InternalTableCell cell = new InternalTableCell();
			cell.Controls.Add(new PivotGridHtmlHeaderContent(Data, field));
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach(Image image in fSeparators)
				Data.PivotGrid.RenderHelper.GetGroupSeparatorImage().AssignToControl(image, Data.IsDesignMode);
			CellPadding = 0;
			CellSpacing = 0;
			RenderUtils.SetStyleStringAttribute(this, "width", "100%");
		}
		InternalTableCell GetHorizontalLine() {
			InternalTableCell cell = new InternalTableCell();
			Image image = RenderUtils.CreateImage();
			fSeparators.Add(image);
			cell.Controls.Add(image);
			return cell;
		}
	}
	internal class PivotGridHtmlSolidGroupHeader : PivotGridHtmlGroupHeader {
		InternalTableCell fCell;
		public PivotGridHtmlSolidGroupHeader(PivotGridWebData data, DevExpress.XtraPivotGrid.PivotGridGroup group) : base(data, group){}
		protected override void CreateControlHierarchy() {
			ID = Data.GetGroupHeaderID(Group);
			InternalTableRow row = new InternalTableRow();
			fCell = new InternalTableCell();
			fCell.Text = Group.ToString();
			if(string.IsNullOrEmpty(fCell.Text))
				fCell.Text = "&nbsp;";
			row.Controls.Add(fCell);
			Rows.Add(row);
			Attributes.Add("onmousedown", Data.GetHeaderMouseDown());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderTextStyle((PivotGridField)Group.Fields[0]).AssignToControl(fCell);
			Data.GetHeaderStyle((PivotGridField)Group.Fields[0]).AssignToControl(this);
			BorderWidth = 1;
		}
	}
	internal class PivotGridHtmlHeaderContent : InternalTable {
		PivotGridWebData data;
		PivotGridField field;
		PivotGridHtmlHeaderFilter filterButton;
		DefaultBoolean canDrag;
		public PivotGridHtmlHeaderContent(PivotGridWebData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			this.canDrag = DefaultBoolean.Default;
		}
		PivotGridWebData Data { get { return data; } }
		PivotGridField Field { get { return field; } }
		bool ShowGroupButton { get { return Field.Group != null && Field.Group.CanExpandField(field); } }
		bool ShowFilterButton { get { return Field.ShowFilterButton && AddFilter; } }
		bool AddFilter { get { return (!Field.Visible && Field.FilterValues.HasFilter) || Field.Visible; } }
		bool ShowSortButton { get { return Field.ShowSortImage; } }
		public DefaultBoolean CanDragInGroup {
			get { return canDrag; }
			set {
				if(canDrag == value) return;
				canDrag = value;
				ResetControlHierarchy();
			}
		}
		bool AddDragScript {
			get {
				return Field.CanDrag && (Field.Group == null || CanDragInGroup == DefaultBoolean.True) &&
					Data.PivotGrid.IsEnabled();
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = Data.GetHeaderID(field);
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			if(HeaderTemplate != null) {
				InternalTableCell cell = new InternalTableCell();
				row.Cells.Add(cell);
				cell.Controls.Add(Data.SetupTemplateContainer(new PivotGridHeaderTemplateContainer(Field), HeaderTemplate));
			} else {
				if(AddDragScript)
					Attributes.Add("onmousedown", Data.GetHeaderMouseDown());
				if(ShowGroupButton)
					row.Controls.Add(new PivotGridHtmlGroupButton(data, field));
				PivotGridHtmlHeaderText headerText = new PivotGridHtmlHeaderText(data, field);
				row.Controls.Add(headerText);
				if(Data.PivotGrid.IsEnabled() && Field.ShowSortImage && Field.Visible) {
					if(!Data.PivotGrid.IsAccessibilityCompliantRender())
						headerText.Attributes.Add("onclick", Data.GetHeaderClick());
				}
				if(ShowSortButton) {
					InternalTableCell sortButton = new PivotGridHtmlHeaderSort(data, field);
					if(!Data.PivotGrid.IsAccessibilityCompliantRender())
						sortButton.Attributes.Add("onclick", Data.GetHeaderClick());
					row.Controls.Add(sortButton);
				}
				if(ShowFilterButton) {
					filterButton = new PivotGridHtmlHeaderFilter(data, field);
					row.Controls.Add(filterButton);
				}
				if(Field.Visible) {
					Data.PivotGrid.RenderHelper.AddHeaderContextMenu(ID, Field);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CellPadding = 0;
			CellSpacing = 0;
			Data.GetHeaderTableStyle(Field).AssignToControl(this);
			if(filterButton != null && !Field.Visible)
				filterButton.HorizontalAlign = HorizontalAlign.Right;			
		}
		protected ITemplate HeaderTemplate {
			get { return Field.HeaderTemplate != null ? Field.HeaderTemplate : Data.HeaderTemplate; }
		}
	}
	internal class PivotGridHtmlRowHeaderContainer : EmptyPivotGridHtmlRowHeaderContainer {
		PivotGridField field;
		PivotGridHtmlHeaderContent header;
		Image leftSeparator, rightSeparator;
		InternalTable containerTable;
		public PivotGridHtmlRowHeaderContainer(PivotGridWebData data, PivotGridField field, int rowSpan)
			: base(data, rowSpan) {
			this.field = field;			
		}
		public PivotGridField Field {
			get { return field; }
		}
		public PivotGridHtmlHeaderContent Header {
			get { return header; }
		}
		public InternalTable ContainerTable {
			get {
				return containerTable;
			}
		}
		protected ASPxPivotGridRenderHelper RenderHelper {
			get { return Data.PivotGrid.RenderHelper; }
		}
		public Image LeftSeparator {
			get { return leftSeparator; }
		}
		public Image RightSeparator {
			get { return rightSeparator; }
		}
		public bool IsFirst {
			get {
				return Data.GetFieldByLevel(false, 0) == Field;
			}
		}
		public bool IsLast {
			get {
				return Data.GetFieldByLevel(false, Data.GetFieldCountByArea(PivotArea.RowArea) - 1) == Field;
			}
		}
		public bool RequiresRightGroupSeparator {
			get {
				PivotGridGroup group = Field.Group;
				return group != null &&
					Field.InnerGroupIndex != group.Count - 1 &&
					group.IsFieldVisible(group[Field.InnerGroupIndex + 1]);
			}
		}
		public bool RequiresLeftGroupSeparator {
			get { return Field.Group != null && Field.InnerGroupIndex != 0; }
		}
		protected override void CreateControlHierarchy() {
			CreateContainerTable();
			CreateLeftGroupSeparator();
			CreateHeader();
			CreateRightGroupSeparator();
		}
		protected void CreateContainerTable() {
			this.containerTable = new InternalTable();
			Controls.Add(ContainerTable);
			ContainerTable.Rows.Add(new InternalTableRow());
		}
		protected void CreateHeader() {
			this.header = new PivotGridHtmlHeaderContent(Data, Field);
			Header.CanDragInGroup = DefaultBoolean.True;
			AddControl(Header);
		}
		protected void CreateLeftGroupSeparator() {
			if(RequiresLeftGroupSeparator) {
				this.leftSeparator = new Image();
				AddControl(LeftSeparator);
			}
		}
		protected void CreateRightGroupSeparator() {
			if(RequiresRightGroupSeparator) {
				this.rightSeparator = new Image();
				AddControl(RightSeparator);
			}
		}		
		protected void AddControl(WebControl control) {
			InternalTableCell cell = new InternalTableCell();
			cell.Controls.Add(control);
			ContainerTable.Rows[0].Cells.Add(cell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareContainerControl();
			if(LeftSeparator != null)
				PrepareSeparator(LeftSeparator);
			if(RightSeparator != null)
				PrepareSeparator(RightSeparator);			
			SetPaddings();
		}
		private void PrepareContainerControl() {
			ContainerTable.CellPadding = 0;
			ContainerTable.CellSpacing = 0;
			ContainerTable.BorderStyle = BorderStyle.None;
			ContainerTable.Width = new Unit(100, UnitType.Percentage);
		}
		protected void PrepareSeparator(Image separator) {
			RenderHelper.GetGroupSeparatorImage().AssignToControl(separator, Data.IsDesignMode);
			Data.Styles.ApplyGroupSeparatorStyle(separator);
		}
		protected void SetPaddings() {
			Paddings paddings = Data.GetAreaPaddings(PivotArea.RowArea, IsFirst, IsLast);
			if(RequiresLeftGroupSeparator) {
				LeftSeparator.Width = paddings.PaddingLeft;
				((InternalTableCell)LeftSeparator.Parent).Width = LeftSeparator.Width;  
				LeftSeparator.Height = new Unit(1, UnitType.Pixel); 
				paddings.PaddingLeft = 0;
			}
			if(RequiresRightGroupSeparator) {
				RightSeparator.Width = paddings.PaddingRight;
				RightSeparator.Height = new Unit(1, UnitType.Pixel); 
				paddings.PaddingRight = 0;
			}
			RenderUtils.SetPaddings(this, paddings);
		}
	}
	internal class EmptyPivotGridHtmlRowHeaderContainer : InternalTableCell {
		PivotGridWebData data;
		public EmptyPivotGridHtmlRowHeaderContainer(PivotGridWebData data, int rowSpan) {
			this.data = data;
			RowSpan = rowSpan;
		}
		public PivotGridWebData Data {
			get { return data; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetAreaStyle(PivotArea.RowArea).AssignToControl(this);
		}
	}
	internal class PivotGridHtmlHeaderCell : InternalTableCell {
		PivotGridField field;
		PivotGridWebData data;
		public PivotGridField Field { get { return field; } }
		public PivotGridWebData Data { get { return data; } }
		public PivotGridHtmlHeaderCell(PivotGridWebData data, PivotGridField field) {
			this.field = field;
			this.data = data;
		}
	}
	internal class PivotGridHtmlGroupButton : PivotGridHtmlHeaderCell {
		Image GroupButtonImage;
		HyperLink link;
		public PivotGridHtmlGroupButton(PivotGridWebData data, PivotGridField field) : base(data, field) {
			ID = Data.GetGroupButtonID(field);
		}
		protected override void CreateControlHierarchy() {			
			GroupButtonImage = RenderUtils.CreateImage();
			if(Data.PivotGrid.IsAccessibilityCompliantRender() && Data.PivotGrid.IsEnabled()) {
				this.link = RenderUtils.CreateHyperLink();
				Controls.Add(this.link);
				this.link.Controls.Add(GroupButtonImage);
			} else {
				Controls.Add(GroupButtonImage);
			}
		}
		protected override void PrepareControlHierarchy() {			
			Data.GetGroupButtonStyle(Field).AssignToControl(this);			
			Data.PivotGrid.RenderHelper.GetGroupButtonImage(Field.ExpandedInFieldsGroup).AssignToControl(GroupButtonImage, Data.IsDesignMode);
			if(Data.PivotGrid.IsEnabled()) {
				string js = Data.GetGroupButtonOnClick(Field.Index.ToString());
				if(this.link != null) {
					this.link.NavigateUrl = string.Format("javascript:{0}", js);
				} else {
					GroupButtonImage.Attributes.Add("onclick", js);
				}
			}
		}
	}
	internal class PivotGridHtmlHeaderText : PivotGridHtmlHeaderCell {
		HyperLink link;
		public PivotGridHtmlHeaderText(PivotGridWebData data, PivotGridField field)
			: base(data, field) {
			ID = Data.GetHeaderTextCellID(Field);
		}
		protected override void CreateControlHierarchy() {
			if(Field.ShowSortImage && Data.PivotGrid.IsAccessibilityCompliantRender() && Data.PivotGrid.IsEnabled()) {
				string text = Field.ToString();
				if(!string.IsNullOrEmpty(text)) {
					this.link = RenderUtils.CreateHyperLink();
					this.link.Text = text;
					Controls.Add(link);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderTextStyle(Field).AssignToControl(this);
			if(Field.Area == PivotArea.RowArea && Field.Visible && !RenderUtils.IsOpera)
				Width = Unit.Percentage(100);
			if(this.link != null) {
				this.link.NavigateUrl = Data.GetAccessibleSortUrl(Field);
				RenderUtils.SetStringAttribute(this.link, "onmousedown", PivotGridWebData.CancelBubbleJs);
			} else {
				Text = Field.ToString();
				if(string.IsNullOrEmpty(Text)) Text = "&nbsp;";
			}
		}
	}
	internal class PivotGridHtmlHeaderSort : PivotGridHtmlHeaderCell {
		Image sortImage;
		public PivotGridHtmlHeaderSort(PivotGridWebData data, PivotGridField field)
			: base(data, field) {
			ID = Data.GetHeaderSortCellID(Field);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();			
			sortImage = RenderUtils.CreateImage();
			Controls.Add(sortImage);
			Field.SortImage.AssignToControl(sortImage, Data.IsDesignMode);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderSortStyle(Field).AssignToControl(this);			
		}
	}
	internal class PivotGridHtmlHeaderFilter : PivotGridHtmlHeaderCell {
		Image filterImage;
		HyperLink link;
		public PivotGridHtmlHeaderFilter(PivotGridWebData data, PivotGridField field)
			: base(data, field) {
			ID = Data.GetHeaderFilterCellID(Field);
		}
		protected override void CreateControlHierarchy() {
			filterImage = RenderUtils.CreateImage();
			if(Data.PivotGrid.IsAccessibilityCompliantRender() && Data.PivotGrid.IsEnabled()) {
				this.link = RenderUtils.CreateHyperLink();
				Controls.Add(this.link);
				this.link.Controls.Add(filterImage);
			} else {
				Controls.Add(filterImage);
			}
			Field.FilterButtonImage.AssignToControl(filterImage, Data.IsDesignMode);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderFilterStyle(Field).AssignToControl(this);
			if(!Field.Visible) Width = Unit.Percentage(100);
			if(Data.PivotGrid.IsEnabled()) {
				string js = Data.GetFilterButtonOnClick(Field);
				if(this.link != null) {
					this.link.NavigateUrl = string.Format("javascript:{0}", js);
				} else {
					if(Field.Visible) 
						filterImage.Attributes.Add("onclick", PivotGridWebData.CancelBubbleJs + ";" + js);
					RenderUtils.SetCursor(filterImage, RenderUtils.GetDefaultCursor());
				}
			}
		}
	}
	internal class PivotGridHtmlAreaHeaders : InternalTableRow {
		PivotGridWebData data;
		PivotArea area;		
		public PivotGridHtmlAreaHeaders(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		public PivotGridWebData Data { get { return data; } }
		public PivotArea Area { get { return area; } }
		protected override void CreateControlHierarchy() {
			PivotGridField[] fields = Data.GetFieldsByArea(Area);
			foreach(PivotGridField field in fields) {
				WebControl header = HeaderHelper.CreateHeader(field, Data, false);
				if(header == null)
					continue;
				InternalTableCell cell = new InternalTableCell();
				cell.Controls.Add(header);
				Controls.Add(cell);
			}
		}
		protected override void PrepareControlHierarchy() {
			for(int i = 0; i < Controls.Count; i++) {
				if(Controls[i] is TableCell) {
					TableCell cell = (TableCell)Controls[i];
					RenderUtils.SetPaddings(cell, Data.GetAreaPaddings(Area, i == 0, i == Controls.Count - 1));
				}
			}
		}
	}
	internal class PivotGridHtmlArea : InternalTable {
		PivotGridWebData data;
		PivotArea area;
		public PivotGridHtmlArea(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected PivotArea Area { get { return area; } }
		protected override void CreateControlHierarchy() {
			ID = Area.ToString();
			PivotGridHtmlAreaHeaders headers = new PivotGridHtmlAreaHeaders(Data, Area);
			Controls.Add(headers);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Attributes["cellpadding"] = "0";
			Attributes["cellspacing"] = "0";
		}
	}
	internal abstract class PivotGridHtmlAreaCellContainerBase : InternalTableCell {
		PivotGridWebData data;
		PivotArea area;
		public PivotGridHtmlAreaCellContainerBase(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		public static PivotGridHtmlAreaCellContainerBase Create(PivotGridWebData data, PivotArea area) {
			if(data.GetFieldCountByArea(area) == 0)
				return new PivotGridHtmlEmptyAreaCellContainer(data, area);
			if(area == PivotArea.DataArea && data.IsDataAreaCollapsed)
				return new PivotGridHtmlCollapsedDataAreaCellContainer(data, area);
			return new PivotGridHtmlAreaCellContainer(data, area);
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected PivotArea Area { get { return area; } }
		protected virtual string GetID() {
			return Data.GetHeaderTableID(Area);
		}
		protected override void CreateControlHierarchy() {
			ID = GetID();
			PivotGrid.RenderHelper.AddHeaderContextMenu(ID, null);
		}
		protected override void PrepareControlHierarchy() {
		}
	}
	internal class PivotGridHtmlEmptyAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		public PivotGridHtmlEmptyAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area){ }
		protected override string GetID() {
			return Data.GetAreaID(Area);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(Data.EmptyAreaTemplate != null) 
				Controls.Add(Data.SetupTemplateContainer(new PivotGridEmptyAreaTemplateContainer(Area), Data.EmptyAreaTemplate));
			else
				Text = PivotGridLocalizer.GetHeadersAreaText((int)Area);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetEmptyAreaStyle(Area).AssignToControl(this);
		}
	}
	internal class PivotGridHtmlAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		public PivotGridHtmlAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area) { }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if((Area == PivotArea.RowArea && Data.OptionsView.ShowRowHeaders) ||
				(Area == PivotArea.ColumnArea && Data.OptionsView.ShowColumnHeaders) ||
				(Area == PivotArea.DataArea && Data.OptionsView.ShowDataHeaders) ||
				(Area == PivotArea.FilterArea && Data.OptionsView.ShowFilterHeaders)) {
				Controls.Add(new PivotGridHtmlArea(Data, Area));
			}
			if(Area == PivotArea.ColumnArea && !Data.OptionsView.ShowColumnHeaders && Data.OptionsView.ShowDataHeaders) {
				Controls.Add(new LiteralControl("&nbsp;"));	
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetAreaStyle(Area).AssignToControl(this);
		}
	}
	internal class PivotGridHtmlCollapsedDataAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		PivotGridDataAreaPopup dataAreaPopup;
		Image image;
		LiteralControl text;
		protected PivotGridDataAreaPopup DataAreaPopup { get { return dataAreaPopup; } }
		protected Image Image { get { return image; } }
		protected new LiteralControl Text { get { return text; } }
		public PivotGridHtmlCollapsedDataAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area) { }
		protected override string GetID() {
			return PivotGridWebData.ElementName_DataHeadersPopupCell;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.image = RenderUtils.CreateImage();
			this.text = RenderUtils.CreateLiteralControl(PivotGridLocalizer.GetString(PivotGridStringId.PrintDesignerDataHeaders));
			Controls.Add(Image);
			Controls.Add(Text);
			if(!Data.PivotGrid.DesignMode) {
				this.dataAreaPopup = Data.PivotGrid.CreateDataAreaPopup();
				Controls.Add(dataAreaPopup);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetEmptyAreaStyle(Area).AssignToControl(this);
			if(DataAreaPopup != null)
				DataAreaPopup.PopupElementID = ClientID;
			if(Image != null) {
				PivotGrid.RenderHelper.GetDataHeadersImage().AssignToControl(Image, Data.IsDesignMode);
				PivotGrid.Styles.ApplyDataHeadersImageStyle(Image);
			}
		}
	}
	internal class PivotGridPagerContainer : InternalTableCell {
		PivotGridHtmlTable htmlTable;
		ASPxPivotGrid pivotGrid;
		ASPxPivotGridPager pager;
		bool isTopPager;
		public PivotGridPagerContainer(PivotGridHtmlTable htmlTable, ASPxPivotGrid pivotGrid, bool isTopPager) {
			this.htmlTable = htmlTable;
			this.pivotGrid = pivotGrid;
			this.isTopPager = isTopPager;
		}
		protected PivotGridHtmlTable HtmlTable { get { return htmlTable; } }
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		public ASPxPivotGridPager Pager { get { return pager; } }
		protected bool IsTopPager { get { return isTopPager; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			pager = new ASPxPivotGridPager(HtmlTable, PivotGrid);
			pager.ID = IsTopPager ? "TopPager" : "BottomPager";
			Controls.Add(Pager);
		}
		protected override void PrepareControlHierarchy() {		
			Pager.PagerSettings.Assign(PivotGrid.OptionsPager);
			Data.GetPagerStyle(IsTopPager).AssignToControl(this);
			base.PrepareControlHierarchy();
		}
	}
	internal class PivotGridTableCell : InternalTableCell {
		PivotGridWebData data;
		public PivotGridTableCell(PivotGridWebData data) {
			this.data = data;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
	}
	internal class PivotGridHtmlFieldCellBase : PivotGridTableCell {
		PivotFieldValueItem item;
		Image fCollapsedButtonImage, fSortByColumnImage;
		LiteralControl text;
		HyperLink fLink;
		List<PivotGridFieldPair> sortedFields;
		public PivotGridHtmlFieldCellBase(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data) {
			this.item = item;
			this.sortedFields = sortedFields;
		}
		protected override HtmlTextWriterTag TagKey { 
			get { return PivotGrid.IsAccessibilityCompliantRender() ? HtmlTextWriterTag.Th : base.TagKey; } 
		}
		protected List<PivotGridFieldPair> SortedFields { get { return sortedFields; } }
		protected bool IsAnyFieldSortedByThisSummary { get { return SortedFields != null && SortedFields.Count > 0; } }
		protected PivotFieldValueItem Item { get { return item; } }
		protected PivotGridField Field {
			get {
				PivotGridField webField = Item.Field as PivotGridField;
				return webField;
			}
		}
		protected override void CreateControlHierarchy() {
			if(ValueTemplate != null) {
				Controls.Add(Data.SetupTemplateContainer(
					new PivotGridFieldValueTemplateContainer(new PivotGridFieldValueTemplateItem(Field, Item)), ValueTemplate));
			} else {
				if(Item.ShowCollapsedButton) {
					this.fCollapsedButtonImage = RenderUtils.CreateImage();
					if(PivotGrid.IsAccessibilityCompliantRender() && PivotGrid.IsEnabled()) {
						this.fLink = RenderUtils.CreateHyperLink();
						Controls.Add(this.fLink);
						this.fLink.Controls.Add(this.fCollapsedButtonImage);
					} else {
						Controls.Add(fCollapsedButtonImage);
					}
				};
				ID = (Item.IsColumn ? "C" : "R") + Item.UniqueIndex.ToString();
				PivotGrid.RenderHelper.AddFieldValueContextMenu(ID, Item, SortedFields);
				this.text = RenderUtils.CreateLiteralControl();
				Controls.Add(text);
				string displayText = Data.GetPivotFieldValueText(Item);
				if(!string.IsNullOrEmpty(displayText)) {
					text.Text = displayText;
				} else text.Text = "&nbsp;";
				if(IsAnyFieldSortedByThisSummary) {
					this.fSortByColumnImage = RenderUtils.CreateImage();
					Controls.Add(fSortByColumnImage);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			PivotFieldValueStyle fieldValueStyle = Data.GetFieldValueStyle(Item, Field);
			fieldValueStyle.AssignToControl(this);
			if(!fieldValueStyle.Paddings.IsEmpty)
				RenderUtils.SetPaddings(this, fieldValueStyle.Paddings);
			if(this.fCollapsedButtonImage != null) {			
				PivotGrid.RenderHelper.GetFieldValueCollapsedImage(Item.IsCollapsed).AssignToControl(fCollapsedButtonImage, Data.IsDesignMode);
				Data.GetCollapsedButtonStyle().AssignToControl(fCollapsedButtonImage, true);
				if(!fieldValueStyle.ImageSpacing.IsEmpty)
					RenderUtils.SetMargins(fCollapsedButtonImage, new Paddings(0, 0, fieldValueStyle.ImageSpacing, 0));
			}
			if(this.fSortByColumnImage != null) {
				PivotGrid.RenderHelper.GetSortByColumnImage().AssignToControl(fSortByColumnImage, Data.IsDesignMode);
				Data.GetSortByColumnImageStyle().AssignToControl(fSortByColumnImage, true);
				if(!fieldValueStyle.ImageSpacing.IsEmpty)
					RenderUtils.SetMargins(fSortByColumnImage, new Paddings(fieldValueStyle.ImageSpacing, 0, 0, 0));
			}
			if(PivotGrid.IsEnabled()) {
				string js = Data.GetCollapsedImageOnClick(Item);
				if(this.fLink != null) {
					this.fLink.NavigateUrl = string.Format("javascript:{0}", js);
				} else {
					if(fCollapsedButtonImage != null)
						fCollapsedButtonImage.Attributes.Add("onclick", js);
				}
			}
		}
		ITemplate ValueTemplate {
			get {
				if(Field == null) return Data.FieldValueTemplate;
				return Field.ValueTemplate != null ? Field.ValueTemplate : Data.FieldValueTemplate;
			}
		}
	}
	internal class PivotGridHtmlColumnFieldCell : PivotGridHtmlFieldCellBase {
		public PivotGridHtmlColumnFieldCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) { }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Item.SpanCount > 1) ColumnSpan = Item.SpanCount;
			if(Item.CellLevelCount > 1) RowSpan = Item.CellLevelCount;
			if(PivotGrid.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "scope", "col");			
		}
		protected bool IsLastGrandTotal(PivotFieldValueItem Item) {
			if(Item == null || Item.Data == null || Item.Data.Fields == null) return false;
			PivotGridFieldCollection Fields = Item.Data.Fields as PivotGridFieldCollection;
			if(Fields == null) return false;
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].Area == PivotArea.DataArea && Fields[i].AreaIndex > Item.DataIndex)
					return false;
			return true;
		}
	}
	internal class PivotGridHtmlRowFieldCell : PivotGridHtmlFieldCellBase {
		public PivotGridHtmlRowFieldCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) { }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Item.SpanCount > 1) RowSpan = Item.SpanCount;
			if(Item.CellLevelCount > 1) ColumnSpan = Item.CellLevelCount;
			if(Data.Styles.FieldValueStyle.TopAlignedRowValues && Item.SpanCount > 1)
				VerticalAlign = VerticalAlign.Top;
			if(PivotGrid.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "scope", "row");
		}
	}
	internal class PivotGridHtmlDataCell : PivotGridTableCell {
		PivotGridCellItem cellItem;
		public PivotGridHtmlDataCell(PivotGridWebData data, PivotGridCellItem cellItem)
			: base(data) {
			this.cellItem = cellItem;
		}
		protected PivotGridCellItem CellItem { get { return cellItem; } }
		protected string GetText() { return Data.GetCellDisplayText(CellItem); }
		protected string GetPreparedText() {
			string cellText = GetText();
			if(cellText != null)
				cellText = cellText.Trim();
			return string.IsNullOrEmpty(cellText) ? "&nbsp;" : cellText;
		}
		protected override void CreateChildControls() {
			if(Data.CellTemplate != null) {
				Controls.Add(Data.SetupTemplateContainer(
					new PivotGridCellTemplateContainer(new PivotGridCellTemplateItem(CellItem, GetText())), Data.CellTemplate));
				return;
			}
			if(CellItem.ShowKPIGraphic) {
				Image kpi = new Image();
				Data.KPIImages.GetImage(CellItem.KPIGraphic, CellItem.DataField.KPIType, CellItem.KPIValue).AssignToControl(kpi, false);
				Controls.Add(kpi);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Data.CellTemplate == null && !CellItem.ShowKPIGraphic) {
				Text = GetPreparedText();
			}
			if(!string.IsNullOrEmpty(Data.PivotGrid.ClientSideEvents.CellClick)) {
				if(PivotGrid.IsEnabled()) Attributes.Add("onclick", Data.GetCellClick(CellItem));
			}
			if(!string.IsNullOrEmpty(Data.PivotGrid.ClientSideEvents.CellDblClick)) {
				if(PivotGrid.IsEnabled()) Attributes.Add("ondblclick", Data.GetCellDblClick(CellItem));
			}
			PivotCellStyle cellStyle = new PivotCellStyle();
			Data.ApplyCellStyle(CellItem, cellStyle);
			if(CellItem.ColumnIndex == 0)
				cellStyle.BorderLeft.BorderColor = Data.GetTableStyle().Border.BorderColor;
			cellStyle.AssignToControl(this, true);
			base.PrepareControlHierarchy();
		}
	}
	internal static class HeaderHelper {
		public static WebControl CreateHeader(PivotGridField field, PivotGridWebData data, bool customizationForm) {
			if(field.Group == null) {
				return new PivotGridHtmlHeaderContent(data, field);
			} else {
				if(field.InnerGroupIndex == 0)
					return customizationForm ? new PivotGridHtmlSolidGroupHeader(data, field.Group) : new PivotGridHtmlGroupHeader(data, field.Group);
			}
			return null;
		}
	}
}
