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
using DevExpress.Web.ASPxClasses.Internal;
using System.Collections;
using System.IO;
using DevExpress.Utils;
using DevExpress.Data.IO;
using System.Collections.ObjectModel;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxEditors;
namespace DevExpress.Web.ASPxGridView {
	public class GridViewBaseTemplateContainer : TemplateContainerBase {
		ASPxGridView grid;
		public GridViewBaseTemplateContainer(ASPxGridView grid) : this(grid, 0, grid) { }
		public GridViewBaseTemplateContainer(ASPxGridView grid, int index, object dataItem)
			: base(index, dataItem) {
			this.grid = grid;
		}
		[Description("Gets the ASPxGridView that owns the current object.")]
		public ASPxGridView Grid { get { return grid; } }
		public override void DataBind() {
			base.DataBind();
			if (NeedLoadPostData)
				LoadPostData(this);
		}
		protected virtual bool NeedLoadPostData { get { return false; } }
		protected delegate Control CreateControlForReplace();
		protected virtual void ReplaceControl(GridViewTemplateReplacementType replacementType, CreateControlForReplace createControl) {
			ASPxGridViewTemplateReplacement control = FindReplacementControl(this, replacementType);
			if (control != null) {
				ReplaceControl(control, createControl());
				control.EnsureControls();
			}
		}
		protected ASPxGridViewTemplateReplacement FindReplacementControl(Control parent, GridViewTemplateReplacementType replacementType) {
			foreach (Control control in parent.Controls) {
				ASPxGridViewTemplateReplacement rc = control as ASPxGridViewTemplateReplacement;
				if (rc != null && rc.ReplacementType == replacementType) return rc;
				rc = FindReplacementControl(control, replacementType);
				if (rc != null) return rc;
			}
			return null;
		}
		protected void ReplaceControl(ASPxGridViewTemplateReplacement source, Control control) {
			if (source == null) return;
			source.Controls.Clear();
			source.Controls.Add(control);
		}
		void LoadPostData(Control parent) {
			if (Grid.postCollection == null || !Grid.InternalIsCallBacksEnabled()) return;
			if (Page != null && Page.IsCallback)
				RenderUtils.LoadPostDataRecursive(parent, Grid.postCollection, true);
		}
	}
	public class GridViewHeaderTemplateContainer : GridViewBaseTemplateContainer {
		GridViewHeaderLocation headerLocation;
		public GridViewHeaderTemplateContainer(GridViewColumn column, GridViewHeaderLocation headerLocation)
			: base(column.Grid, column.Index, column) {
			this.headerLocation = headerLocation;
			string location = "";
			if (HeaderLocation == GridViewHeaderLocation.Group) {
				location = "G";
			}
			if (HeaderLocation == GridViewHeaderLocation.Customization) {
				location = "C";
			}
			ID = string.Format("header{0}{1}", location, column.Index);
		}
		[Description("Gets the column that contains the template container.")]
		public GridViewColumn Column { get { return DataItem as GridViewColumn; } }
		[Description("Gets the column header's current location.")]
		public GridViewHeaderLocation HeaderLocation { get { return headerLocation; } }
	}
	public class GridViewBaseRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewBaseRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, visibleIndex, row) {
			if (IdPrefix != string.Empty)
				ID = IdPrefix + VisibleIndexPrefix;
		}
		[Description("Gets an object that uniquely identifies the row that contains the template container.")]
		public virtual object KeyValue { get { return Grid.DataProxy.GetRowKeyValue(VisibleIndex); } }
		protected virtual string IdPrefix { get { return string.Empty; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		[Description("Gets the index of the rendered item object.")]
		public int VisibleIndex { get { return ItemIndex; } }
		protected virtual string VisibleIndexPrefix { get { return VisibleIndex < 0 ? "new" : VisibleIndex.ToString(); } }
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			CommandEventArgs cmdArgs = args as CommandEventArgs;
			if (cmdArgs != null) {
				RaiseBubbleEvent(this, new ASPxGridViewRowCommandEventArgs(VisibleIndex, KeyValue, cmdArgs, source));
				return true;
			}
			return base.OnBubbleEvent(source, args);
		}
	}
	public class GridViewPreviewRowTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewPreviewRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "pr"; } }
		[Description("Gets the text displayed within the preview section.")]
		public string Text { get { return Grid.RenderHelper.TextBuilder.GetPreviewText(VisibleIndex); } }
	}
	public class GridViewDetailRowTemplateContainer : GridViewBaseRowTemplateContainer {
		bool bound = false;
		public GridViewDetailRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "dxdt"; } }
		public override void DataBind() {
			if (this.bound) return; 
			base.DataBind();
			this.bound = true;
		}
	}
	public class GridViewDataRowTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewDataRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "row"; } }
	}
	public class GridViewDataItemTemplateContainer : GridViewDataRowTemplateContainer {
		GridViewDataColumn column;
		public GridViewDataItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column)
			: base(grid, row, visibleIndex) {
			this.column = column;
			GenerateCellId();
		}
		protected virtual void GenerateCellId() {
			ID = string.Format("cell{0}_{1}", VisibleIndexPrefix, Column.Index);
		}
		protected override string IdPrefix { get { return string.Empty; } }
		[Description("Gets a data column where the template is rendered.")]
		public GridViewDataColumn Column { get { return column; } }
		[Description("Gets the data item's text.")]
		public string Text {
			get {
				string text = RenderHelper.TextBuilder.GetRowDisplayText(Column, VisibleIndex);
				return string.IsNullOrEmpty(text) ? "&nbsp;" : text;
			}
		}
	}
	public class GridViewEditItemTemplateContainer : GridViewDataItemTemplateContainer {
		public GridViewEditItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column)
			: base(grid, row, visibleIndex, column) {
		}
		[Description("Gets a value that specifies the group of template controls for which the current container causes validation.")]
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
		protected override void GenerateCellId() {
			ID = string.Format("edit{0}_{1}", VisibleIndexPrefix, Column.Index);
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public class GridViewGroupRowTemplateContainer : GridViewBaseRowTemplateContainer {
		GridViewDataColumn column;
		public GridViewGroupRowTemplateContainer(ASPxGridView grid, GridViewDataColumn column, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
			this.column = column;
			ID = string.Format("gr{0}_{1}", VisibleIndexPrefix, Column.Index);
		}
		protected override string IdPrefix { get { return string.Empty; } }
		[Description("Gets an object that uniquely identifies the group row that contains the template container.")]
		public override object KeyValue { get { return Grid.DataProxy.GetRowValue(VisibleIndex, Column.FieldName); } }
		[Description("Gets whether the group row is expanded.")]
		public bool Expanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
		[Description("Gets the grouped column.")]
		public GridViewDataColumn Column { get { return column; } }
		[Description("Gets the text displayed within the group row.")]
		public string GroupText { get { return RenderHelper.TextBuilder.GetGroupRowDisplayText(Column, VisibleIndex); } }
		[Description("Gets the summary text displayed within the group row.")]
		public string SummaryText { get { return Grid.GetGroupRowSummaryText(VisibleIndex); } }
	}
	public class GridViewTitleTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewTitleTemplateContainer(ASPxGridView grid)
			: base(grid) {
			ID = "Title";
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public class GridViewStatusBarTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewStatusBarTemplateContainer(ASPxGridView grid)
			: base(grid) {
			ID = "StatusBar";
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public enum GridViewPagerBarPosition { Top, Bottom }
	public class GridViewPagerBarTemplateContainer : GridViewBaseTemplateContainer {
		GridViewPagerBarPosition position;
		string pagerId;
		public GridViewPagerBarTemplateContainer(ASPxGridView grid, GridViewPagerBarPosition position, string pagerId)
			: base(grid) {
			this.position = position;
			this.pagerId = pagerId;
			ID = "PagerBar" + GetIDSuffix();
		}
		public GridViewPagerBarPosition Position { get { return position; } }
		protected string PagerId { get { return pagerId; } }
		protected override bool NeedLoadPostData { get { return true; } }
		protected string GetIDSuffix() { return Position == GridViewPagerBarPosition.Top ? "T" : "B"; }
		public override void DataBind() {
			ReplaceControl(GridViewTemplateReplacementType.Pager, delegate { return GridViewHtmlPagerPanelBase.CreatePager(Grid, PagerId); });
			base.DataBind();
		}
	}
	public class GridViewEmptyDataRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewEmptyDataRowTemplateContainer(ASPxGridView grid)
			: base(grid) {
			ID = "EmptyRow";
		}
	}
	public class GridViewFooterRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFooterRowTemplateContainer(ASPxGridView grid)
			: base(grid) {
			ID = "FooterRow";
		}
	}
	public class GridViewFooterCellTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFooterCellTemplateContainer(GridViewColumn column)
			: base(column.Grid, column.Index, column) {
			ID = string.Format("footer{0}", column.Index);
		}
		[Description("Gets a data column where the template is rendered.")]
		public GridViewColumn Column { get { return DataItem as GridViewColumn; } }
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public enum GridViewTemplateReplacementType { EditFormContent, EditFormCancelButton, EditFormUpdateButton, EditFormEditors, EditFormCellEditor, Pager }
	[ToolboxItem(true), HiddenToolboxItem]
	public class ASPxGridViewTemplateReplacement : Control, IStopLoadPostDataOnCallbackMarker {
		GridViewTemplateReplacementType replacementType = GridViewTemplateReplacementType.EditFormContent;
		string columnKey = string.Empty;
		public ASPxGridViewTemplateReplacement() {
		}
		[Description("Gets or sets which edit form controls are displayed by the ASPxGridViewTemplateReplacement.")]
		public GridViewTemplateReplacementType ReplacementType {
			get { return replacementType; }
			set { replacementType = value; }
		}
		[Description("Gets or sets a value that identifies the edit cell.")]
		public string ColumnID {
			get { return columnKey; }
			set { columnKey = value; }
		}
		internal void EnsureControls() {
			EnsureChildControls();
			EnsureChildControlsRecursive(this);
		}
		protected virtual void EnsureChildControlsRecursive(Control control) {
			for (int i = 0; i < control.Controls.Count; i++) {
				Control childControl = control.Controls[i];
				if (childControl is IContentContainer)
					continue;
				if (childControl is IASPxWebControl)
					(childControl as IASPxWebControl).EnsureChildControls();
				EnsureChildControlsRecursive(childControl);
			}
		}
	}
	public class GridViewEditFormTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewEditFormTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		[Description("Gets a value that specifies the group of template controls for which the current container causes validation.")]
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
		protected override string IdPrefix { get { return "ef"; } }
		protected ASPxGridViewScripts Scripts { get { return Grid.RenderHelper.Scripts; } }
		[Description("Gets the script that implements the cancel action.")]
		public string CancelAction { get { return Scripts.GetCancelEditFunction(); } }
		[Description("Gets the script that implements the update action.")]
		public string UpdateAction { get { return Scripts.GetUpdateEditFunction(); } }
		protected GridViewTableEditFormCell EditForm { get { return Parent as GridViewTableEditFormCell; } }
		protected Control CreateEditorsTable(bool renderUpdateCancelButtons) {
			return new GridViewEditFormTable(RenderHelper, VisibleIndex, renderUpdateCancelButtons);
		}
		public override void DataBind() {
			ReplaceControl(GridViewTemplateReplacementType.EditFormContent, delegate { return CreateEditorsTable(true); });
			ReplaceControl(GridViewTemplateReplacementType.EditFormCancelButton, delegate { return new GridViewTableUpdateCancelCell(RenderHelper).CreateCancelButton(); });
			ReplaceControl(GridViewTemplateReplacementType.EditFormUpdateButton, delegate { return new GridViewTableUpdateCancelCell(RenderHelper).CreateUpdateButton(); });
			ReplaceControl(GridViewTemplateReplacementType.EditFormEditors, delegate { return CreateEditorsTable(false); });
			ReplaceCellEditors();
			base.DataBind();
		}
		protected override bool NeedLoadPostData { get { return true; } }
		void ReplaceCellEditors() {
			List<ASPxGridViewTemplateReplacement> list = new List<ASPxGridViewTemplateReplacement>();
			FindEditorReplacements(this, list);
			foreach (ASPxGridViewTemplateReplacement replacement in list) {
				if(!TryReplaceCellEditor(replacement)) {
					string msg = String.Format("[Replacement failed for column '{0}']", replacement.ColumnID);
					ReplaceControl(replacement, RenderUtils.CreateLiteralControl(msg));
				}
			}
		}
		void FindEditorReplacements(Control parent, List<ASPxGridViewTemplateReplacement> list) {
			ASPxGridViewTemplateReplacement replacement = parent as ASPxGridViewTemplateReplacement;
			if (replacement != null) {
				if (replacement.ReplacementType == GridViewTemplateReplacementType.EditFormCellEditor)
					list.Add(replacement);
				return;
			}
			foreach (Control child in parent.Controls)
				FindEditorReplacements(child, list);
		}
		bool TryReplaceCellEditor(ASPxGridViewTemplateReplacement replacement) {
			GridViewDataColumn column = FindCellEditorColumn(replacement.ColumnID);
			if(column == null) 
				return false;
			object editorValue = DataProxy.GetEditingRowValue(VisibleIndex, column.FieldName);
			ASPxEditBase editor = RenderHelper.CreateGridEditor(column, editorValue, DevExpress.Web.ASPxEditors.EditorInplaceMode.EditForm, false);
			if(editor == null)
				return false;
			ReplaceControl(replacement, editor);
			Grid.RaiseEditorInitialize(new ASPxGridViewEditorEventArgs(column, VisibleIndex, editor, DataProxy.EditingKeyValue, editorValue));
			return true;
		}
		GridViewDataColumn FindCellEditorColumn(string id) {			
			int columnIndex;
			if(int.TryParse(id, out columnIndex)) {
				if(columnIndex > -1 && columnIndex < Grid.Columns.Count - 1)
					return Grid.Columns[columnIndex] as GridViewDataColumn;
			} else {
				return Grid.Columns[id] as GridViewDataColumn;
			}
			return null;
		}		
	}
	public class GridViewTemplates : PropertiesBase {
		ITemplate header, headerCaption, dataItem, groupRowContent,
			groupRow, dataRow, detailRow, previewRow, titlePanel, statusBar, pagerBar, editForm, emptyDataRow, footerRow, footerCell;
		public GridViewTemplates(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewHeaderTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate Header {
			get { return header; }
			set {
				if (Header == value) return;
				header = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewHeaderTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderCaption {
			get { return headerCaption; }
			set {
				if (HeaderCaption == value) return;
				headerCaption = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDataItemTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataItem {
			get { return dataItem; }
			set {
				if (DataItem == value) return;
				dataItem = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupRowContent {
			get { return groupRowContent; }
			set {
				if (GroupRowContent == value) return;
				groupRowContent = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupRow {
			get { return groupRow; }
			set {
				if (GroupRow == value) return;
				groupRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDataRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataRow {
			get { return dataRow; }
			set {
				if (DataRow == value) return;
				dataRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDetailRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DetailRow {
			get { return detailRow; }
			set {
				if (DetailRow == value) return;
				detailRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewPreviewRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PreviewRow {
			get { return previewRow; }
			set {
				if (PreviewRow == value) return;
				previewRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewEmptyDataRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EmptyDataRow {
			get { return emptyDataRow; }
			set {
				if (EmptyDataRow == value) return;
				emptyDataRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFooterRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterRow {
			get { return footerRow; }
			set {
				if (FooterRow == value) return;
				footerRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFooterCellTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterCell {
			get { return footerCell; }
			set {
				if (FooterCell == value) return;
				footerCell = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewTitleTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TitlePanel {
			get { return titlePanel; }
			set {
				if (TitlePanel == value) return;
				titlePanel = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewStatusBarTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate StatusBar {
			get { return statusBar; }
			set {
				if (StatusBar == value) return;
				statusBar = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewPagerBarTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PagerBar {
			get { return pagerBar; }
			set {
				if (PagerBar == value) return;
				pagerBar = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewEditFormTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EditForm {
			get { return editForm; }
			set {
				if (EditForm == value) return;
				editForm = value;
				TemplatesChanged();
			}
		}
		protected virtual void TemplatesChanged() {
			Changed();
		}
	}
}
