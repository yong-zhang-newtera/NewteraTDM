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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPivotGrid.Design;
using System.Drawing.Design;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.FilterControl;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridField : PivotGridFieldBase, IWebControlObject, IDataSourceViewSchemaAccessor,
			IDataDictionary, IFilterColumn {
		ITemplate headerTemplate;
		ITemplate valueTemplate;
		PivotHeaderStyle headerStyle;
		PivotFieldValueStyle valueStyle;
		PivotFieldValueStyle valueTotalStyle;
		PivotCellStyle cellStyle;
		string id = string.Empty;
		public PivotGridField()
			: base() {
			Initialize();
		}
		public PivotGridField(PivotGridData data)
			: base(data) {
			Initialize();
		}
		public PivotGridField(string fieldName, PivotArea area)
			: base(fieldName, area) {
			Initialize();
		}
		void Initialize() {
			this.headerTemplate = null;
			this.valueTemplate = null;
			this.headerStyle = new PivotHeaderStyle();
			this.valueStyle = new PivotFieldValueStyle();
			this.valueTotalStyle = new PivotFieldValueStyle();
			this.cellStyle = new PivotCellStyle();
			TrackViewState();
		}
		protected override PivotGridFieldOptions CreateOptions(EventHandler eventHandler, string name) {
			return new PivotGridWebFieldOptions(eventHandler, this, name);
		}
		PivotGridWebData WebData { get { return Data != null ? (PivotGridWebData)Data : null; } }
		internal ASPxPivotGrid PivotGrid { get { return WebData != null ? WebData.PivotGrid : null; } }
		protected new PivotGridFieldCollection Collection { get { return (PivotGridFieldCollection)base.Collection; } }
		internal bool ShowFilterButton { get { return CanFilter; } }
		internal bool ShowSortImage { get { return CanSort; } }
		internal string ClientID {
			get {
				if(!string.IsNullOrEmpty(ID)) return ID;
				return Index.ToString();
			}
		}
		[
		Description("Gets or sets the name of the database field that is assigned to the current PivotGridField object."), Category("Data"),
		Localizable(true), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true),
		Editor(typeof(EditorLoader), typeof(UITypeEditor)),
		EditorLoader("DevExpress.Data.Browsing.Design.WebColumnNameEditor", AssemblyInfo.SRAssemblyUtils)
		]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("Use the ID property instead.")]
		public override string Name {
			get { return ID; }
			set { ID = value; }
		}
		protected override string ComponentName {
			get { return ClientID; }
		}
		protected override PivotGridFieldBase GetFieldFromComponentName(string componentName) {
			return Collection.GetFieldByClientID(componentName);
		}
		[Description("Gets or sets the field's unique identifier name. "),
		Category("Data"), DefaultValue(""), XtraSerializableProperty(), Localizable(false), NotifyParentProperty(true)]
		public string ID {
			get { return id; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value && value != string.Empty) {
					if(!IsDeserializing) throw new Exception(FieldNameEqualsNameExceptionString);
					else value = NamePrefix + value;
				}
				id = value;
			}
		}
		void ResetOptions() { Options.Reset(); }
		bool ShouldSerializeOptions() { return Options.ShouldSerialize(); }
		[Description("Contains the field's options."), Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new PivotGridWebFieldOptions Options { get { return (PivotGridWebFieldOptions)base.Options; } }
		[Description("Gets style settings for cells."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotCellStyle CellStyle { get { return cellStyle; } }
		int groupIndex = -1;
		int innerGroupIndex = -1;
		[Description("This member supports the .NET Framework infrastructure and cannot be used directly from your code."),
		PersistenceMode(PersistenceMode.Attribute),
		DefaultValue(-1),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int GroupIndex {
			get { return base.GroupIndex; }
			set { groupIndex = value; }
		}
		internal int GroupIndexCore { get { return groupIndex; } }
		[Description("This member supports the .NET Framework infrastructure and cannot be used directly from your code."),
		PersistenceMode(PersistenceMode.Attribute),
		DefaultValue(-1),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int InnerGroupIndex {
			get { return base.InnerGroupIndex; }
			set { innerGroupIndex = value; }
		}
		internal int InnerGroupIndexCore { get { return innerGroupIndex; } }
		[Description("Gets the field's style settings."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotHeaderStyle HeaderStyle { get { return headerStyle; } }
		[Description("Gets style settings used to paint field values."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotFieldValueStyle ValueStyle { get { return valueStyle; } }
		[Description("Gets style settings used to paint totals."),
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotFieldValueStyle ValueTotalStyle { get { return valueTotalStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsTextOnly { get { return !ShowFilterButton && !ShowSortImage; } }
		protected override bool IsDataDeserializing {
			get {
				return base.IsDataDeserializing || (Data != null && Data.IsLoading);
			}
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridHeaderTemplateContainer))]
		public ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {
				if(HeaderTemplate == value) return;
				headerTemplate = value;
				RaiseTemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridFieldValueTemplateContainer))]
		public ITemplate ValueTemplate {
			get { return valueTemplate; }
			set {
				if(ValueTemplate == value) return;
				valueTemplate = value;
				RaiseTemplatesChanged();
			}
		}
		void RaiseTemplatesChanged() {
			if(WebData != null) WebData.TemplatesChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties FilterButtonImage {
			get {
				if(WebData == null || !ShowFilterButton) return null;
				return FilterValues.IsEmpty ? PivotGrid.RenderHelper.GetHeaderFilterImage() : PivotGrid.RenderHelper.GetHeaderActiveFilterImage();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties SortImage {
			get {
				if(!ShowSortImage || Data == null) return null;
				return PivotGrid.RenderHelper.GetHeaderSortImage(SortOrder);
			}
		}
		void ResetCustomTotals() { CustomTotals.Clear(); }
		bool ShouldSerializeCustomTotals() { return CustomTotals.Count > 0; }
		[Description("Gets the collection of custom totals for the current field."), AutoFormatDisable,
		Category("Behaviour"), MergableProperty(false), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.DefaultValue)]
		[Editor("DevExpress.XtraPivotGrid.Design.CustomTotalsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridCore,
			"System.Drawing.Design.UITypeEditor, System.Drawing")]
		public new PivotGridCustomTotalCollection CustomTotals {
			get { return (PivotGridCustomTotalCollection)base.CustomTotals; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void XtraClearCustomTotals(XtraItemEventArgs e) {
			CustomTotals.Clear();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateCustomTotalsItem(XtraItemEventArgs e) {
			return CustomTotals.Add(PivotSummaryType.Sum);
		}
		protected override PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollection(this);
		}
		#region IWebControlObject Members
		bool IWebControlObject.IsDesignMode() {
			return WebData.IsDesignMode;
		}
		bool IWebControlObject.IsLoading() {
			return WebData.IsLoading;
		}
		bool IWebControlObject.IsRendering() {
			return WebData.IsRendering;
		}
		void IWebControlObject.LayoutChanged() {
			WebData.LayoutChanged();
		}
		void IWebControlObject.TemplatesChanged() {
			WebData.TemplatesChanged();
		}
		#endregion
		#region IDataSourceViewSchemaAccessor Members
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				if(PivotGrid != null && !String.IsNullOrEmpty(PivotGrid.OLAPConnectionString)) {
					WebData.OLAPConnectionString = PivotGrid.OLAPConnectionString;
					return WebData.GetDesignOLAPDataSourceObject();
				} else {
					IDataSourceViewSchemaAccessor acessor = WebData.DataSourceViewSchemaAccessor;
					return acessor != null ? acessor.DataSourceViewSchema : null;
				}
			}
			set { ; }
		}
		#endregion
		internal void TrackViewState() {
			((IStateManager)CellStyle).TrackViewState();
			((IStateManager)HeaderStyle).TrackViewState();
			((IStateManager)ValueStyle).TrackViewState();
			((IStateManager)ValueTotalStyle).TrackViewState();
		}
		#region IDataDictionary Members
		string IDataDictionary.GetDataSourceDisplayName() {
			if(WebData != null && WebData.PivotGrid != null)
				return WebData.PivotGrid.DataSourceID;
			else
				return string.Empty;
		}
		string IDataDictionary.GetObjectDisplayName(string dataMember) {
			return dataMember;
		}
		#endregion
		#region IFilterColumn Members
		FilterColumnClauseClass IFilterColumn.ClauseClass {
			get {
				if(DataType == typeof(string))
					return FilterColumnClauseClass.String;
				if(DataType == typeof(DateTime))
					return FilterColumnClauseClass.DateTime;
				return FilterColumnClauseClass.Generic;
			}
		}
		string IFilterColumn.DisplayName {
			get { return ToString(); }
		}
		ComboBoxProperties propertiesEdit;
		EditPropertiesBase IFilterColumn.PropertiesEdit {
			get {
				if(propertiesEdit == null) {
					propertiesEdit = new ComboBoxProperties();
					propertiesEdit.Items.AddRange(GetUniqueValues());
					propertiesEdit.DropDownStyle = DropDownStyle.DropDown;
					propertiesEdit.EnableIncrementalFiltering = true;
				}
				return propertiesEdit;
			}
		}
		string IFilterColumn.PropertyName {
			get { return !string.IsNullOrEmpty(Name) ? Name : DataControllerColumnName; }
		}
		Type IFilterColumn.PropertyType {
			get { return DataType; }
		}
		#endregion
	}
	public class PivotGridFieldCollection : PivotGridFieldCollectionBase {
		public PivotGridFieldCollection(PivotGridData data)
			: base(data) {
		}
		public new PivotGridField this[int index] { get { return base[index] as PivotGridField; } }
		[Browsable(false)]
		public new PivotGridField this[string id_fieldName_Caption] {
			get {
				foreach(PivotGridField field in this) {
					if(field.ID == id_fieldName_Caption) return field;
				}
				foreach(PivotGridField field in this) {
					if(field.FieldName == id_fieldName_Caption) return field;
				}
				foreach(PivotGridField field in this) {
					if(field.Caption == id_fieldName_Caption) return field;
				}
				return null;
			}
		}
		public void AddField(PivotGridField field) {
			AddCore(field);
		}
		public void Add(PivotGridField field) {
			AddCore(field);
		}
		public new PivotGridField Add(string fieldName, PivotArea area) {
			return base.Add(fieldName, area) as PivotGridField;
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			PivotGridField field = new PivotGridField(fieldName, area);
			if(!Data.IsLoading && string.IsNullOrEmpty(field.Name))
				field.ID = GenerateName(field.FieldName);
			return field;
		}
		protected override string GenerateName(string fieldName) {
			string res = base.GenerateName(fieldName),
				final = res;
			int n = 1;
			while(true) {
				if(this[final] == null) break;
				final = res + n++.ToString();
			}
			return final;
		}
		public PivotGridFieldBase GetFieldByClientID(string clientID) {
			for(int i = 0; i < Count; i++) {
				if(this[i].ClientID == clientID)
					return this[i];
			}
			return null;
		}
	}
	public class PivotGridCustomTotal : PivotGridCustomTotalBase {
		public PivotGridCustomTotal() : base() { }
		public PivotGridCustomTotal(PivotSummaryType summaryType) : base(summaryType) { }
	}
	public class PivotGridCustomTotalCollection : PivotGridCustomTotalCollectionBase {
		public PivotGridCustomTotalCollection() { }
		public PivotGridCustomTotalCollection(PivotGridFieldBase field) : base(field) { }
		public PivotGridCustomTotalCollection(PivotGridCustomTotalBase[] totals)
			: base(totals) {}
		[Description("Provides indexed access to individual items within the PivotGridCustomTotalCollection.")]
		public new PivotGridCustomTotal this[int index] {
			get { return (PivotGridCustomTotal)base[index]; }
		}
		protected override PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotal();
		}
	}
	public class PivotGridWebGroup : PivotGridGroup {
		[Description("Provides access to the group's field collection."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, false, true)]
		public new IList Fields { get { return base.Fields; } }
		internal void SortFields() {
			Sort(new FieldGroupComparer());
		}
		class FieldGroupComparer : IComparer<PivotGridFieldBase> {
			public int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
				return Comparer.Default.Compare(((PivotGridField)x).InnerGroupIndexCore, ((PivotGridField)y).InnerGroupIndexCore);
			}
		}
	}
	public class PivotGridWebGroupCollection : PivotGridGroupCollection {
		public PivotGridWebGroupCollection(PivotGridData data) : base(data) { }
		public new PivotGridWebGroup this[int index] { get { return (PivotGridWebGroup)InnerList[index]; } }
		protected override PivotGridGroup CreateGroup() {
			return new PivotGridWebGroup();
		}
	}
}
