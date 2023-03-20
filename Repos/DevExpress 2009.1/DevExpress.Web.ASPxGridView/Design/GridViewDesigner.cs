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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.Design;
using System.Reflection;
namespace DevExpress.Web.ASPxGridView.Design {
	public class GridViewDesigner : ASPxDataWebControlDesigner {
		public override string CssTemplatePath {
			get { return ASPxGridView.WebResourceCssDefaultPath; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
			RegisterTagPrefix(typeof(ASPxEditBase));
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				"DevExpress.Web" + AssemblyInfo.VSuffix,
				"DevExpress.Web.ASPxEditors" + AssemblyInfo.VSuffix
			);
		}
		protected internal ASPxGridView Grid { get { return Component as ASPxGridView; } }
		static string[] controlTemplateNames = new string[] { "Header", "HeaderCaption",
					"DataRow", "DataItem", "GroupRow", "GroupRowContent",
					"EditForm", "DetailRow", "PreviewRow", "EmptyDataRow", "FooterRow", "FooterCell", "StatusBar", "TitlePanel", "PagerBar" };
		static string columnTemplateName = "Columns[{0}].{1}";
		static string[] columnTemplateNames = new string[] { 
			"HeaderTemplate", "HeaderCaptionTemplate", "FooterTemplate", "$DataItemTemplate", "$EditItemTemplate", "$GroupRowTemplate"};
		protected override void PreFilterProperties(System.Collections.IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor autog = (PropertyDescriptor)properties["AutoGenerateColumns"];
			properties["AutoGenerateColumns"] = TypeDescriptor.CreateProperty(typeof(GridViewDesigner), autog);
			if(Grid.AutoGenerateColumns) {
				PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor)properties["Columns"];
				if(oldPropertyDescriptor != null) {
					Attribute[] attributes = new Attribute[] { new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden), new BrowsableAttribute(false) };
					properties["Columns"] = TypeDescriptor.CreateProperty(typeof(GridViewDesigner), oldPropertyDescriptor, attributes);
				}
			}
		}
		[DefaultValue(true)]
		public bool AutoGenerateColumns {
			get { return Grid.AutoGenerateColumns; }
			set {
				if(AutoGenerateColumns == value) return;
				Grid.AutoGenerateColumns = value;
				OnAutoGenerateColumnsChanged();
			}
		}
		void OnAutoGenerateColumnsChanged() {
			if(AutoGenerateColumns) {
				foreach(GridViewColumn column in Grid.Columns) {
					GridViewDataColumn dc = column as GridViewDataColumn;
					if(dc == null) continue;
					SetAutoGenerated(dc);
				}
				TypeDescriptor.Refresh(Grid);
				ComponentChanged();
			}
		}
		void SetAutoGenerated(GridViewDataColumn dc) {
			PropertyInfo pi = dc.GetType().GetProperty("AutoGenereated", BindingFlags.Instance | BindingFlags.NonPublic);
			if(pi != null) pi.SetValue(dc, true, null);
		}
		public GridViewColumnCollection Columns { get { return Grid == null ? null : Grid.Columns; } }
		public override TemplateGroupCollection TemplateGroups {
			get {
				TemplateGroupCollection templateGroups = base.TemplateGroups;
				foreach(GridViewColumn column in Grid.Columns) {
					TemplateGroup templateGroup = new TemplateGroup(string.Format(columnTemplateName, column.Index, column.ToString()));
					for(int j = 0; j < columnTemplateNames.Length; j++) {
						string template = columnTemplateNames[j];
						if(template.StartsWith("$")) {
							template = template.Substring(1);
							if(!(column is GridViewDataColumn)) continue;
						}
						TemplateDefinition templateDefinition = new TemplateDefinition(this, template, column, template, this.GetTemplateStyle());
						templateDefinition.SupportsDataBinding = true;
						templateGroup.AddTemplateDefinition(templateDefinition);
					}
					templateGroups.Add(templateGroup);
				}
				for(int i = 0; i < controlTemplateNames.Length; i++) {
					TemplateGroup templateGroup = new TemplateGroup(controlTemplateNames[i]);
					TemplateDefinition templateDefinition = new TemplateDefinition(this, controlTemplateNames[i], Grid.Templates, controlTemplateNames[i], this.GetTemplateStyle());
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
					templateGroups.Add(templateGroup);
				}
				return templateGroups;
			}
		}
		Style GetTemplateStyle() {
			return Grid.ControlStyle;
		}
		protected override void OnSchemaRefreshed() {
			base.OnSchemaRefreshed();
			Cursor current = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(OnSchemaRefreshedCallback), null, "SchemaRefreshed");
				UpdateDesignTimeHtml();
			}
			finally {
				Cursor.Current = current;
			}
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxGridView viewControl = (ASPxGridView)dataControl;
			IDataSourceViewSchema dataSourceSchema = GetDataSourceSchema();
			if(dataSourceSchema != null) {
				IDataSourceFieldSchema[] fields = dataSourceSchema.GetFields();
				if(fields == null || fields.Length == 0)
					viewControl.KeyFieldName = string.Empty;
			}
			if(viewControl.Columns.Count == 0)
				viewControl.AutoGenerateColumns = true;
			TypeDescriptor.Refresh(Component);
			base.DataBind(dataControl);
		}
		protected override bool UsePreviewControl {
			get { return !IsRootDesignerDummy(); }
		}
		public IDataSourceViewSchema GetDataSourceSchema() {
			DesignerDataSourceView designerView = DesignerView;
			if(designerView != null) {
				try {
					return designerView.Schema;
				} catch {
				}
			}
			return null;
		}
		bool OnSchemaRefreshedCallback(object context) {
			IDataSourceViewSchema dataSourceSchema = GetDataSourceSchema();
			bool isDataSettingExits = Grid.Columns.Count > 0 || !string.IsNullOrEmpty(Grid.KeyFieldName);
			if(DataSourceID.Length > 0 && dataSourceSchema != null) {
				if(isDataSettingExits) {
					if(ShowMessage(Grid.Site, "Would you like to regenerate the GridView column fields and data keys using the selected data source schema?  Warning: this will delete all existing column fields.", 
						string.Format("Refresh Fields and Keys for '{0}'", Grid.ID), MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Grid.KeyFieldName = string.Empty;
						Grid.Columns.Clear();
						AddKeysAndBoundFields(dataSourceSchema);
					}
				} else {
					this.AddKeysAndBoundFields(dataSourceSchema);
				}
				return true;
			} 
			if(isDataSettingExits && ShowMessage(Component.Site, "Would you like to clear the GridView column fields and data keys?  Warning: this will delete all existing column fields.", 
				string.Format("Refresh Fields and Keys for '{0}'", Grid.ID), MessageBoxButtons.YesNo) == DialogResult.Yes) {
				Grid.KeyFieldName = string.Empty;
				Grid.Columns.Clear();
			}
			return true;
		}
		protected internal DialogResult ShowMessage(IServiceProvider serviceProvider, string message, string caption, MessageBoxButtons buttons) {
			if(serviceProvider != null) {
				IUIService service = (IUIService)serviceProvider.GetService(typeof(IUIService));
				if(service != null) return service.ShowMessage(message, caption, buttons);
			}
			return MessageBox.Show(message, caption, buttons);
		}
		void AddKeysAndBoundFields(IDataSourceViewSchema schema) { AddKeysAndBoundFields(schema, true); }
		protected internal void AddKeysAndBoundFields(IDataSourceViewSchema schema, bool addKeys) {
			if(schema == null) return;
			IDataSourceFieldSchema[] fields = schema.GetFields();
			if(fields == null || fields.Length == 0) return;
			List<string> keys = new List<string>();
			foreach(IDataSourceFieldSchema field in fields) {
				if(!IsBindableType(field.DataType)) continue;
				GridViewDataColumn column = GridViewEditDataColumn.CreateColumn(field.DataType);
				string name = field.Name;
				if(field.PrimaryKey) {
					keys.Add(name);
				}
				column.FieldName = name;
				column.ReadOnly = field.PrimaryKey || field.IsReadOnly;
				if(field.Identity) {
					column.EditFormSettings.Visible = DevExpress.Web.ASPxClasses.DefaultBoolean.False;
				}
				Grid.Columns.Add(column);
			}
			Grid.AutoGenerateColumns = false;
			if(keys.Count > 0 && addKeys) {
				Grid.KeyFieldName = keys[0];
			}
		}
		bool IsBindableType(Type type) {
			if((!type.IsPrimitive && type != typeof(string)) && type != typeof(DateTime) && type != typeof(decimal)) {
				return (type == typeof(Guid));
			}
			return true;
		}
		#region Selection
		#endregion Selection
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new GridViewDesignerActionList(this);
		}
		public override bool HasHttpHandler() {
			return true;
		}
		public override void ShowAbout() {
			GridViewAboutDialogHelper.ShowAbout(Component.Site);
		}
		public void EditColumns() {
			ShowDialog(CreateEditorForm("Columns", Grid.Columns));
		}
	}
	public class GridViewDesignerActionList : ASPxWebControlDesignerActionList {
		GridViewDesigner designer;
		public GridViewDesignerActionList(GridViewDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		protected new GridViewDesigner Designer {
			get { return designer; }
		}
		protected ASPxGridView Grid {
			get { return Designer.Grid; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			if(Grid.SettingsPager.Mode == GridViewPagerMode.ShowPager) {
				collection.Add(new DesignerActionPropertyItem("ShowPager",
					StringResources.GridViewActionList_ShowPager,
					StringResources.GridViewActionList_ChecksCategory,
					StringResources.GridViewActionList_ShowPagerDescription));
			}
			collection.Add(new DesignerActionPropertyItem("ShowGroupPanel",
				StringResources.GridViewActionList_ShowGroupPanel,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_ShowGroupPanelDescription));
			collection.Add(new DesignerActionPropertyItem("EnableEditing",
				StringResources.DataEditingActionList_EnableEditing,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_EnableEditingDescription));
			collection.Add(new DesignerActionPropertyItem("EnableInserting",
				StringResources.DataEditingActionList_EnableInserting,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_EnableInsertingDescription));
			collection.Add(new DesignerActionPropertyItem("EnableSelection",
				StringResources.GridViewActionList_EnableSelection,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_EnableSelectionDescription));
			collection.Add(new DesignerActionPropertyItem("EnableDeleting",
				StringResources.DataEditingActionList_EnableDeleting,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_EnableDeletingDescription));
			collection.Add(new DesignerActionPropertyItem("EnableFiltering",
				StringResources.GridViewActionList_EnableFiltering,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.GridViewActionList_EnableFilteringDescription));
			collection.Insert(0, new DesignerActionMethodItem(this, "EditColumns",
				StringResources.GridViewActionList_EditColumns,
				StringResources.ActionList_MiscCategory,
				StringResources.GridViewActionList_EditColumnsDescription, true));
			return collection;
		}
		public bool ShowPager {
			get { return Grid.SettingsPager.Visible; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, Grid.SettingsPager, "Visible", value, null);
				EditorContextHelper.FireChanged(Designer, Grid, "SettingsPager");
				EditorContextHelper.RefreshSmartPanel(Component);
			}
		}
		public bool EnableFiltering {
			get { return Grid.Settings.ShowFilterRow && GetCommandColumn() != null && GetCommandColumn().ClearFilterButton.Visible; }
			set {
				GetOrCreateCommandColumn().ClearFilterButton.Visible = value;
				EditorContextHelper.SetPropertyValue(Designer, Grid.Settings, "ShowFilterRow", value, null);
				EditorContextHelper.FireChanged(Designer, Grid, "Settings");
				EditorContextHelper.RefreshSmartPanel(Component);
				FireColumnsChanged();
			}
		}
		public bool ShowGroupPanel {
			get { return Grid.Settings.ShowGroupPanel; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, Grid.Settings, "ShowGroupPanel", value, null);
				EditorContextHelper.FireChanged(Designer, Grid, "Settings");
				EditorContextHelper.RefreshSmartPanel(Component);
			}
		}
		public bool EnableSelection {
			get {
				return GetCommandColumn() != null && GetCommandColumn().ShowSelectCheckbox;
			}
			set {
				GetOrCreateCommandColumn().ShowSelectCheckbox = value;
				FireColumnsChanged();
			}
		}
		public bool EnableDeleting {
			get {
				return GetCommandColumn() != null && GetCommandColumn().DeleteButton.Visible;
			}
			set {
				GetOrCreateCommandColumn().DeleteButton.Visible = value;
				FireColumnsChanged();
			}
		}
		public bool EnableEditing {
			get {
				return GetCommandColumn() != null && GetCommandColumn().EditButton.Visible;
			}
			set {
				GetOrCreateCommandColumn().EditButton.Visible = value;
				FireColumnsChanged();
			}
		}
		public bool EnableInserting {
			get {
				return GetCommandColumn() != null && GetCommandColumn().NewButton.Visible;
			}
			set {
				GetOrCreateCommandColumn().NewButton.Visible = value;
				FireColumnsChanged();
			}
		}
		void FireColumnsChanged() {
			EditorContextHelper.FireChanged(Designer, Grid, "Columns");
			EditorContextHelper.RefreshSmartPanel(Component);
		}
		protected GridViewCommandColumn GetCommandColumn() {
			foreach(GridViewColumn col in Grid.Columns) {
				GridViewCommandColumn cmd = col as GridViewCommandColumn;
				if(cmd != null) return cmd;
			}
			return null;
		}
		protected GridViewCommandColumn GetOrCreateCommandColumn() {
			GridViewCommandColumn res = GetCommandColumn();
			if(res == null) res = CreateCommandColumn();
			return res;
		}
		protected GridViewCommandColumn CreateCommandColumn() {
			GridViewCommandColumn col = new GridViewCommandColumn();
			col.VisibleIndex = 0;
			Grid.Columns.Insert(0, col);
			return col;
		}
		protected void EditColumns() {
			Designer.EditColumns();
		}
		void FireChanged() {
			EditorContextHelper.FireChanged(Designer, Grid);
		}
	}
	public class GridViewColumnEditPropertiesConverter : ExpandableObjectConverter {
		internal const string DefaultName = "Default";
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.Equals(typeof(string))) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) {
				if(value.ToString() == DefaultName) return null;
				return EditRegistrationInfo.CreateProperties(value.ToString());
			}
			return value;
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value is string) return value.ToString();
				if(value == null) return DefaultName;
				return EditRegistrationInfo.GetEditName(value as EditPropertiesBase);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class GridViewColumnEditPropertiesEditor : ObjectSelectorEditor {
		public GridViewColumnEditPropertiesEditor() : base(true) { }
		List<string> GetEditors() {
			List<string> res = new List<string>(EditRegistrationInfo.Editors.Keys);
			res.Sort();
			res.Insert(0, GridViewColumnEditPropertiesConverter.DefaultName);
			return res;
		}
		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector, ITypeDescriptorContext context, IServiceProvider provider) {
			selector.ShowLines = false;
			selector.ShowRootLines = false;
			selector.ShowPlusMinus = false;
			selector.Width = 100;
			selector.Height = 80;
			selector.FullRowSelect = true;
			selector.Clear();
			EditPropertiesBase value = GetCurrentValue(context);
			List<string> list = GetEditors();
			foreach(string s in list) {
				selector.AddNode(s, s, null);
			}
			string valueText = value == null ? GridViewColumnEditPropertiesConverter.DefaultName : EditRegistrationInfo.GetEditName(value);
			foreach(System.Windows.Forms.TreeNode node in selector.Nodes) {
				if(node.Text == valueText) {
					selector.SelectedNode = node;
					break;
				}
			}
			selector.Select();
		}
		EditPropertiesBase GetCurrentValue(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			return context.PropertyDescriptor.GetValue(context.Instance) as EditPropertiesBase;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null) return null;
			value = base.EditValue(context, provider, value);
			if(value is string) {
				value = EditRegistrationInfo.CreateProperties(value.ToString());
			}
			return value;
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}
	}
	public class GridViewDataColumnBuilder : ControlBuilder, IEditorPropertiesContainer {
		string editPropertiesType = string.Empty;
		public string EditPropertiesType { get { return editPropertiesType; } }
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			this.editPropertiesType = (string)attribs["PropertiesEditType"];
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
		#region IEditorPropertiesContainer Members
		Type IEditorPropertiesContainer.GetEditorType() {
			return EditRegistrationInfo.GetEditType(EditPropertiesType);
		}
		#endregion
	}
	public abstract class GridViewBaseStringListConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> list = new List<string>();
			FillList(context, list);
			list.Sort();
			list.Insert(0, string.Empty);
			return new StandardValuesCollection(list);
		}
		protected abstract void FillList(ITypeDescriptorContext context, List<string> list);
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string && value.ToString() == "(None)") return string.Empty;
			if(value is string) return value.ToString();
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(value.ToString() == string.Empty) return "(None)";
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected ASPxGridView GetGrid(object instance) {
			ASPxSummaryItem summary = instance as ASPxSummaryItem;
			ASPxGridView grid = instance as ASPxGridView;
			if(grid != null) return grid;
			if(summary != null && summary.Collection != null) {
				return summary.Collection.Owner as ASPxGridView;
			}
			GridViewColumn column = instance as GridViewColumn;
			if(column != null) return column.Grid;
			return null;
		}
	}
	public class GridViewColumnsConverter : GridViewBaseStringListConverter {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			ASPxGridView grid =  GetGrid(context == null ? null : context.Instance);
			if(grid == null) return;
			foreach(GridViewColumn column in grid.Columns) {
				if(column is GridViewDataColumn) list.Add(column.ToString());
			}
		}
	}
	public class GridViewFieldConverter : GridViewBaseStringListConverter {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			IDataSourceViewSchema schema = GetDataSourceSchema(GetGrid(context == null ? null : context.Instance));
			if(schema != null) {
				IDataSourceFieldSchema[] fields = schema.GetFields();
				foreach(IDataSourceFieldSchema field in fields) {
					list.Add(field.Name);
				}
			}
		}
		IDataSourceViewSchema GetDataSourceSchema(ASPxGridView grid) {
			if(grid == null || grid.Site == null) return null;
			IDesignerHost host = grid.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			GridViewDesigner designer = null;
			if(host != null) designer = host.GetDesigner(grid) as GridViewDesigner;
			if(designer != null) return designer.GetDataSourceSchema();
			return null;
		}
	}
	public class FakeWebSite : ISite {
		IComponent component;
		IContainer container;
		IComponent ISite.Component { get { return component; } }
		IContainer ISite.Container { get { return container; } }
		bool ISite.DesignMode { get { return true; } }
		string ISite.Name { get { return component.GetType().FullName; } set { } }
		public FakeWebSite(IComponent component, IContainer container) {
			this.component = component;
			this.container = container;
		}
		object IServiceProvider.GetService(Type type) {
			return null;
		}
	}
	[ToolboxItem(false)]
	public class FakeWebComponent : ICustomTypeDescriptor {
		object obj;
		EventHandler onChangedMethod;
		public object Object { get { return obj; } }
		public FakeWebComponent(object selectedObject, EventHandler onChangedMethod)
			: base() {
			this.onChangedMethod = onChangedMethod;
			this.obj = selectedObject;
		}
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(obj.GetType());
		}
		string ICustomTypeDescriptor.GetClassName() {
			return obj.GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return obj.GetType().FullName;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(obj);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(obj);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(obj);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(typeof(Object), editorBaseType);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return EventDescriptorCollection.Empty;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attribute) {
			return EventDescriptorCollection.Empty;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			PropertyDescriptorCollection list = TypeDescriptor.GetProperties(obj);
			PropertyDescriptor[] res = new PropertyDescriptor[list.Count];
			for(int n = 0; n < list.Count; n++) {
				PropertyDescriptor pd = list[n];
				if(!(pd is FakeWebComponentProperty)) pd = new FakeWebComponentProperty(pd, onChangedMethod);
				res[n] = pd;
			}
			return new PropertyDescriptorCollection(res);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return ((ICustomTypeDescriptor)this).GetProperties();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return obj;
		}
	}
	public class FakeWebComponentProperty : PropertyDescriptor {
		PropertyDescriptor source;
		EventHandler onChangedMethod;
		public FakeWebComponentProperty(PropertyDescriptor source, EventHandler onChangedMethod)
			: base(source) {
			this.source = source;
			this.onChangedMethod = onChangedMethod;
		}
		public override Type ComponentType { get { return source.ComponentType; } }
		public override Type PropertyType { get { return source.PropertyType; } }
		public override string Name { get { return source.Name; } }
		public override bool CanResetValue(object component) { return source.CanResetValue(component); }
		public override object GetValue(object component) { return source.GetValue(component); }
		public override void SetValue(object component, object value) {
			source.SetValue(component, value);
		}
		public override bool IsReadOnly { get { return source.IsReadOnly; } }
		public override bool ShouldSerializeValue(object component) { return source.ShouldSerializeValue(component); }
		public override void ResetValue(object component) { source.ResetValue(component); }
		internal void NotifyChanged(object component) {
			if(onChangedMethod != null) onChangedMethod(component, EventArgs.Empty);
		}
	}
}
