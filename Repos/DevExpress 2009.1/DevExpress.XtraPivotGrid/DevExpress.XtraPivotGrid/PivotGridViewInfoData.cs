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
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using DevExpress.Data.Helpers;
using System.Collections.Generic;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Design;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class PivotGridCustomTotal : PivotGridCustomTotalBase {
		AppearanceObject appearance;
		public PivotGridCustomTotal()
			: this(PivotSummaryType.Sum) {
		}
		public PivotGridCustomTotal(PivotSummaryType summaryType)
			: base(summaryType) {
			this.appearance = new AppearanceObject();
			this.appearance.Changed += new EventHandler(OnApperanceChanged);
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
		Description("Gets the appearance settings used to paint the custom total's cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotal.Appearance"),
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance), Localizable(true)
		]
		public AppearanceObject Appearance { get { return appearance; } }
		void OnApperanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override void CloneTo(PivotGridCustomTotalBase clone) {
			base.CloneTo(clone);
			if(clone is PivotGridCustomTotal)
				((PivotGridCustomTotal)clone).Appearance.Assign(Appearance);
		}
		public override bool IsEqual(PivotGridCustomTotalBase total) {
			if(!(total is PivotGridCustomTotal)) return false;
			return base.IsEqual(total) && Appearance.IsEqual(((PivotGridCustomTotal)total).Appearance);
		}
	}
	public class PivotGridCustomTotalCollection : PivotGridCustomTotalCollectionBase {
		public PivotGridCustomTotalCollection() { }
		public PivotGridCustomTotalCollection(PivotGridFieldBase field) : base(field) { }
		public PivotGridCustomTotalCollection(PivotGridCustomTotalBase[] totals)
			: base(totals) { }
		[Description("Provides indexed access to the elements in the collection.")]
		public new PivotGridCustomTotal this[int index] { get { return (PivotGridCustomTotal)InnerList[index]; } }
		[Browsable(false)]
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
		public new PivotGridCustomTotal Add(PivotSummaryType summaryType) {
			return (PivotGridCustomTotal)base.Add(summaryType);
		}
		public virtual void AddRange(PivotGridCustomTotal[] customSummaries) {
			foreach(PivotGridCustomTotal customTotal in customSummaries) {
				AddCore(customTotal);
			}
		}
		protected override PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotal();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridFieldToolTips {
		PivotGridField field;
		string headerText;
		FormatInfo valueFormat;
		string valueText;
		public PivotGridFieldToolTips(PivotGridField field) {
			this.field = field;
			this.headerText = string.Empty;
			this.valueText = string.Empty;
			this.valueFormat = new FormatInfo(Field);
		}
		protected PivotGridField Field { get { return field; } }
		[Description(""),
		DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.HeaderText")]
		public string HeaderText {
			get { return headerText; }
			set { headerText = value; }
		}
		void ResetValueFormat() { ValueFormat.Reset(); }
		bool XtraShouldSerializeValueFormat() { return ShouldSerializeValueFormat(); }
		bool ShouldSerializeValueFormat() { return !ValueFormat.IsEmpty; }
		[Description(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueFormat")]
		public FormatInfo ValueFormat { get { return valueFormat; } }
		[Description(""),
		DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueText")]
		public string ValueText {
			get { return valueText; }
			set { valueText = value; }
		}
		public string GetValueText(object value) {
			if(ValueFormat.IsEmpty) return ValueText;
			return ValueFormat.GetDisplayText(value);
		}
		public bool ShouldSerialize() {
			return HeaderText != string.Empty || ValueText != string.Empty || !ValueFormat.IsEmpty;
		}
		public void Reset() {
			HeaderText = string.Empty;
			ValueText = string.Empty;
			ValueFormat.Reset();
		}
	}
	public class PivotGridField : PivotGridFieldBase, IPivotGridViewInfoDataOwner {
		public const int MinimumValueLineCount = 1,
			MaximumColumnValueLineCount = 5,
			MaximumRowValueLineCount = 5;
		PivotGridFieldAppearances appearance;
		int imageIndex;
		Size dropDownFilterListSize;
		PivotGridFieldToolTips toolTips;
		int columnValueLineCount = 1, rowValueLineCount = 1;
		RepositoryItem fieldEdit;
		EventHandler summaryTypeChanged;
		public PivotGridField() : this(string.Empty, PivotArea.FilterArea) { }
		public PivotGridField(PivotGridData data)
			: base(data) {
			Init();
		}
		public PivotGridField(string fieldName, PivotArea area)
			: base(fieldName, area) {
			Init();
		}
		void Init() {
			this.appearance = new PivotGridFieldAppearances();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.imageIndex = -1;
			this.dropDownFilterListSize = Size.Empty;
			this.toolTips = new PivotGridFieldToolTips(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && Appearance != null) {
				Appearance.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearance = null;
			}
			base.Dispose(disposing);
		}
		#region events
		[Description("Occurs after the field's summary type has been changed."), Category("Layout")]
		public event EventHandler SummaryTypeChanged {
			add { summaryTypeChanged += value; }
			remove { summaryTypeChanged -= value; }
		}
		protected void RaiseSummaryTypeChanged() {
			if(summaryTypeChanged != null) summaryTypeChanged(this, new EventArgs());
		}
		#endregion
		void ResetCustomTotals() { CustomTotals.Clear(); }
		bool ShouldSerializeCustomTotals() { return CustomTotals.Count > 0; }
		[
		Description("Gets the collection of custom totals for the current field."), Browsable(true),
		Category("Behaviour"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		Editor("DevExpress.XtraPivotGrid.Design.CustomTotalsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridCore,
			"System.Drawing.Design.UITypeEditor, System.Drawing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.CustomTotals"),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public new PivotGridCustomTotalCollection CustomTotals {
			get { return (PivotGridCustomTotalCollection)base.CustomTotals; }
		}
		[
		Description("Contains the field's options."), Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Options")
		]
		public new PivotGridFieldOptionsEx Options { get { return (PivotGridFieldOptionsEx)base.Options; } }
		protected override PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollection(this);
		}
		protected override PivotGridFieldOptions CreateOptions(EventHandler eventHandler, string name) {
			return new PivotGridFieldOptionsEx(eventHandler, this, "Options");
		}
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		protected PivotGridField DataField { get { return Data != null ? (PivotGridField)Data.DataField : null; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[Description("Provides access to the appearance settings used to paint the field's header, values and value totals."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Appearance")]
		public virtual PivotGridFieldAppearances Appearance { get { return appearance; } }
		[Description("Gets or sets the index of the image which is displayed within the field's header."), DefaultValue(-1), Category("Appearance"),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.ImageList("HeaderImages"), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ImageIndex")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnImageChanged();
			}
		}
		void ResetToolTips() { ToolTips.Reset(); }
		bool ShouldSerializeToolTips() { return ToolTips.ShouldSerialize(); }
		[Description("Gets the field's hint settings."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ToolTips")]
		public PivotGridFieldToolTips ToolTips { get { return toolTips; } }
		[Description("If the current field is displayed in the Column Header Area, this property gets or sets the height of the field's values, in text lines. "), DefaultValue(1), Category("Appearance"),
		XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ColumnValueLineCount")]
		public int ColumnValueLineCount {
			get {
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					return Data.OptionsDataField.ColumnValueLineCount;
				else
					return columnValueLineCount;
			}
			set {
				if(value < MinimumValueLineCount) value = MinimumValueLineCount;
				if(value > MaximumColumnValueLineCount) value = MaximumColumnValueLineCount;
				if(value == ColumnValueLineCount) return;
				columnValueLineCount = value;
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					Data.OptionsDataField.ColumnValueLineCount = value;
				OnColumnRowValueCountChanged();
			}
		}
		[Description("If the current field is displayed in the Row Header Area, this property gets or sets the height of the field's values, in text lines. "), DefaultValue(1), Category("Appearance"),
		XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.RowValueLineCount")]
		public int RowValueLineCount {
			get {
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					return Data.OptionsDataField.RowValueLineCount;
				else
					return rowValueLineCount;
			}
			set {
				if(value < MinimumValueLineCount) value = MinimumValueLineCount;
				if(value > MaximumRowValueLineCount) value = MaximumRowValueLineCount;
				if(value == RowValueLineCount) return;
				rowValueLineCount = value;
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					Data.OptionsDataField.RowValueLineCount = value;
				OnColumnRowValueCountChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object HeaderImages {
			get { return DataViewInfo == null ? null : DataViewInfo.HeaderImages; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size DropDownFilterListSize {
			get { return dropDownFilterListSize; }
			set { dropDownFilterListSize = value; }
		}
		[Browsable(false)]
		public PivotGridControl PivotGrid { get { return DataViewInfo != null ? DataViewInfo.PivotGrid : null; } }
		void SetDefaultParameters(RepositoryItem edit) {
			if(edit == null)
				return;
			if(edit.BorderStyle == BorderStyles.Default)
				edit.BorderStyle = BorderStyles.NoBorder;
			edit.DisplayFormat.Assign(CellFormat);
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual bool CanEdit {
			get {
				if(IsDesignTime) return false;
				return Options.AllowEdit && Data.OptionsCustomization.AllowEdit;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Options.ReadOnly property instead")
		]
		public virtual bool ReadOnly {
			get { return Options.ReadOnly; }
			set { Options.ReadOnly = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Options.AllowEdit property instead")
		]
		public virtual bool AllowEdit {
			get { return Options.AllowEdit; }
			set { Options.AllowEdit = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null),
		TypeConverter(typeof(DevExpress.XtraPivotGrid.TypeConverters.FieldEditConverter)),
		Editor("DevExpress.XtraPivotGrid.Design.FieldEditEditor, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[Description("Gets or sets the editor used to edit cells corresponding to the current data field."), Category("Editing")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.FieldEdit")]
		public virtual RepositoryItem FieldEdit {
			get { return fieldEdit; }
			set {
				if(fieldEdit == value)
					return;
				RepositoryItem old = fieldEdit;
				fieldEdit = value;
				if(old != null) old.Disconnect(this);
				if(FieldEdit != null) {
					FieldEdit.Connect(this);
				}
				SetDefaultParameters(FieldEdit);
				OnEditOptionsChanged();
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Options.ShowButtonMode property instead")
		]
		public virtual PivotShowButtonModeEnum ShowButtonMode {
			get { return Options.ShowButtonMode; }
			set { Options.ShowButtonMode = value; }
		}		
		public void BestFit() {
			if(DataViewInfo != null)
				DataViewInfo.BestFit(this);
		}
		void OnImageChanged() {
			if(Visible) DoLayoutChanged();
		}
		void OnEditOptionsChanged() {
			if(!Visible || Area != PivotArea.DataArea) return;
			DoLayoutChanged();
		}
		void OnColumnRowValueCountChanged() {
			if(!Visible || Area == PivotArea.FilterArea) return;
			DoLayoutChanged();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			if(Visible) DoLayoutChanged();
		}
		protected override void OnSummaryChanged() {
			base.OnSummaryChanged();
			RaiseSummaryTypeChanged();
		}
		protected PivotGridViewInfoData DataViewInfo { get { return (PivotGridViewInfoData)base.Data; } }
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return DataViewInfo; }
		}  
		public override string ToString() {
			string displayText = base.ToString();
			return displayText != string.Empty ? displayText : Name;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void XtraClearCustomTotals(XtraItemEventArgs e) {
			CustomTotals.Clear();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateCustomTotalsItem(XtraItemEventArgs e) {
			return CustomTotals.Add(PivotSummaryType.Sum);
		}
		protected override bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			OptionsLayoutPivotGrid opts = options as OptionsLayoutPivotGrid;
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIdAppearance:
					return opts.StoreAppearance || opts.Columns.StoreAppearance;
				case LayoutIdLayout:
					return opts.Columns.StoreLayout;
				case LayoutIdData:
					return opts.StoreDataSettings;
			}
			return base.OnAllowSerializationProperty(options, propertyName, id);
		}
		protected override void OnResetSerializationProperties(OptionsLayoutBase options) {
			base.OnResetSerializationProperties(options);
			Appearance.Reset();
		}
		internal string GetDisplayText(object value) {
			return Data.GetPivotFieldValueText(this, value);
		}
	}
	public class PivotGridFieldOptionsEx : PivotGridFieldOptions {
		bool readOnly;
		bool allowEdit;
		PivotShowButtonModeEnum showButtonMode;
		public PivotGridFieldOptionsEx(EventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.readOnly = false;
			this.allowEdit = true;
			this.showButtonMode = PivotShowButtonModeEnum.Default;
		}
		[
		XtraSerializableProperty(), DefaultValue(false),
		Description("Gets or sets whether end-users can modify cell values."), Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Options.ReadOnly"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				if(readOnly == value) return;
				readOnly = value;
				OnOptionsChanged();
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(true),
		Description("Gets or sets whether data editing is allowed."), Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Options.AllowEdit"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool AllowEdit {
			get { return allowEdit; }
			set {
				if(allowEdit == value) return;
				allowEdit = value;
				OnOptionsChanged();
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(PivotShowButtonModeEnum.Default),
		Description("Gets or sets which cells corresponding to the current field  display editor buttons."), Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Options.ShowButtonMode")
		]
		public virtual PivotShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(showButtonMode == value) return;
				showButtonMode = value;
				OnOptionsChanged();
			}
		}
	}
	public class PivotGridFieldCollection : PivotGridFieldCollectionBase {
		public PivotGridFieldCollection(PivotGridData data) : base(data) { }
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		[Description("Gets the PivotGridField object specified by the bound field name."), EditorBrowsable(EditorBrowsableState.Always)]
		public new PivotGridField this[int index] { get { return (PivotGridField)base[index]; } }
		[Description("Gets the PivotGridField object specified by the bound field name."), EditorBrowsable(EditorBrowsableState.Always)]
		public new PivotGridField this[string fieldName] { get { return (PivotGridField)base[fieldName]; } }
		public PivotGridField FieldByName(string name) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Name == name)
					return this[i];
			}
			return null;
		}
		public new PivotGridField Add() {
			return (PivotGridField)base.Add();
		}
		public new PivotGridField Add(string fieldName, PivotArea area) {
			return (PivotGridField)base.Add(fieldName, area);
		}
		public void Add(PivotGridField field) {
			base.Add(field);
		}
		public virtual void AddRange(PivotGridField[] fields) {
			foreach(PivotGridField field in fields) {
				AddCore(field);
			}
		}
		public void Remove(PivotGridField field) {
			List.Remove(field);
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			PivotGridField field = CreateFieldCore(fieldName, area);
			IContainer componentContainer = Data.GetComponentContainer();
			if(componentContainer != null) {
				if(!Data.IsLoading && field.Name == string.Empty && field.FieldName != string.Empty)
					field.SetNameCore(GenerateName(componentContainer, field.FieldName));
				try {
					componentContainer.Add(field, field.Name);
				} catch {
					componentContainer.Add(field);
				}
			}
			return field;
		}
		protected virtual PivotGridField CreateFieldCore(string fieldName, PivotArea area) {
			return new PivotGridField(fieldName, area);
		}
		protected string GenerateName(IContainer container, string fieldName) {
			string res = GenerateName(fieldName),
				final = res;
			int n = 1;
			while(true) {
				if(container == null) break;
				if(container.Components[final] == null) break;
				final = res + n++.ToString();
			}
			return final;
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			Data.PopupateCustomizationFormFields();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			DisposeField((PivotGridField)obj);
		}
		protected override void DisposeField(PivotGridFieldBase field) {
			IContainer componentContainer = Data.GetComponentContainer();
			if(componentContainer != null) {
				componentContainer.Remove(field);
			}
			base.DisposeField(field);
			Data.PopupateCustomizationFormFields();
		}
	}
	public enum PivotGridScrolling { CellsArea, Control };	
	public class PivotGridOptionsDataFieldEx : PivotGridOptionsDataField {
		int columnValueLineCount, rowValueLineCount;
		public PivotGridOptionsDataFieldEx(PivotGridViewInfoData data)
			: base(data) {
			Init();
		}
		public PivotGridOptionsDataFieldEx(PivotGridViewInfoData data, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(data, viewBagOwner, objectPath) {
			Init();
		}
		protected virtual void Init() {
			this.columnValueLineCount = 1;
			this.rowValueLineCount = 1;
		}
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		[Description("Gets or sets the height of data field headers, in text lines. This property is in effect when there are two or more data fields, and data field headers are displayed in the Column Header Area.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataFieldEx.ColumnValueLineCount")]
		[DefaultValue(1), XtraSerializableProperty()]
		public int ColumnValueLineCount {
			get { return columnValueLineCount; }
			set { 
				columnValueLineCount = value;
				if(Data != null && Data.Fields != null && Data.Fields.GetVisibleFieldCount(PivotArea.DataArea) >= 2)
					Data.LayoutChanged();
			}
		}
		[Description("Gets or sets the height of data field headers, in text lines. This property is in effect when there are two or more data fields, and data field headers are displayed in the Row Header Area.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataFieldEx.RowValueLineCount")]
		[DefaultValue(1), XtraSerializableProperty()]
		public int RowValueLineCount {
			get { return rowValueLineCount; }
			set {
				rowValueLineCount = value;
				if(Data != null && Data.Fields != null && Data.Fields.GetVisibleFieldCount(PivotArea.DataArea) >= 2)
					Data.LayoutChanged();
			}
		}
	}
	public class PivotGridOptionsChartDataSource : PivotGridOptionsChartDataSourceBase {
		bool selectionOnly;
		public PivotGridOptionsChartDataSource() {
			selectionOnly = true;			
		}
		[Description("Gets or sets whether a chart control must display selected cells or all the data of the PivotGrid control."), DefaultValue(true), XtraSerializableProperty()]
		public bool SelectionOnly {
			get { return selectionOnly; }
			set {
				if(selectionOnly == value) return;
				selectionOnly = value;
				OnOptionsChanged();
			}
		}
	}
	public class PivotGridOptionsCustomizationEx : PivotGridOptionsCustomization {
		bool allowEdit;
		public PivotGridOptionsCustomizationEx(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsCustomizationEx(EventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.allowEdit = true;
		}
		[
		Description("Gets or sets whether data editing is enabled."),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowEdit {
			get { return allowEdit; }
			set {
				if(value == allowEdit) return;
				allowEdit = value;
				OnOptionsChanged();
			}
		}
	}
	public class PivotGridOptionsBehavior : PivotGridOptionsBase {
		bool applyBestFitOnFieldDragging;
		PivotGridScrolling horizontalScrolling;
		bool copyToClipboardWithFieldValues;
		EditorShowMode editorShowMode;
		public PivotGridOptionsBehavior(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.applyBestFitOnFieldDragging = false;
			this.horizontalScrolling = PivotGridScrolling.CellsArea;
			this.editorShowMode = EditorShowMode.Default;
		}
		[Description("Gets or sets whether the Best-Fit feature is automatically applied to a field after it has been dragged and dropped at another location."), DefaultValue(false), XtraSerializableProperty()]
		public bool ApplyBestFitOnFieldDragging {
			get { return applyBestFitOnFieldDragging; }
			set { applyBestFitOnFieldDragging = value; }
		}
		[Description("Gets or sets whether corresponding field values are copied to the clipboard when data cells are copied to the clipboard."), DefaultValue(false), XtraSerializableProperty()]
		public bool CopyToClipboardWithFieldValues {
			get { return copyToClipboardWithFieldValues; }
			set { copyToClipboardWithFieldValues = value; }
		}
		[
		Description("Gets or sets a value which specifies the pivot grid's behavior when it is scrolled horizontally."),
		DefaultValue(PivotGridScrolling.CellsArea), XtraSerializableProperty()
		]
		public PivotGridScrolling HorizontalScrolling {
			get { return horizontalScrolling; }
			set {
				if(HorizontalScrolling == value) return;
				horizontalScrolling = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets how in-place editors are activated."),
		DefaultValue(EditorShowMode.Default), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public EditorShowMode EditorShowMode {
			get { return editorShowMode; }
			set {
				if(value == editorShowMode) return;
				editorShowMode = value;
				OnOptionsChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EditorShowMode GetEditorShowMode() {
			EditorShowMode res = EditorShowMode;
			if(res == EditorShowMode.Default)
				res = EditorShowMode.MouseDownFocused;
			return res;
		}
	}
	internal class PivotGridOptionsSeparator : PivotGridOptionsBase {
		int width;
		int autoLevelCount;
		public PivotGridOptionsSeparator(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.width = 0;
			this.autoLevelCount = 0;
		}
		[Description(""), DefaultValue(0), XtraSerializableProperty()]
		public int Width {
			get { return width; }
			set {
				if(value == Width) return;
				width = value;
				OnOptionsChanged();
			}
		}
		[Description(""), DefaultValue(0), XtraSerializableProperty()]
		public int AutoLevelCount {
			get { return autoLevelCount; }
			set {
				if(value == AutoLevelCount) return;
				autoLevelCount = value;
				OnOptionsChanged();
			}
		}
	}	   
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotShowButtonModeEnum {
		Default,
		ShowAlways,
		ShowForFocusedCell,
		ShowOnlyInEditor
	}
	public class PivotGridOptionsView : PivotGridOptionsViewBase {
		int filterSeparatorBarPadding;
		PivotGridOptionsSeparator columnValueSeparator;
		PivotGridOptionsSeparator rowValueSeparator;
		FilterButtonShowMode headerFilterButtonShowMode;
		bool groupFieldsInCustomizationWindow;
		PivotShowButtonModeEnum showButtonMode;
		public PivotGridOptionsView(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.headerFilterButtonShowMode = FilterButtonShowMode.Default;
			this.filterSeparatorBarPadding = 1;
			this.columnValueSeparator = new PivotGridOptionsSeparator(optionsChanged);
			this.rowValueSeparator = new PivotGridOptionsSeparator(optionsChanged);
			this.groupFieldsInCustomizationWindow = true;
			this.showButtonMode = PivotShowButtonModeEnum.Default;
		}
		[Description("Gets or sets how and when filter buttons are displayed within field headers.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.HeaderFilterButtonShowMode")]
		[DefaultValue(FilterButtonShowMode.Default), XtraSerializableProperty()]
		public virtual FilterButtonShowMode HeaderFilterButtonShowMode {
			get { return headerFilterButtonShowMode; }
			set {
				if(HeaderFilterButtonShowMode == value) return;
				FilterButtonShowMode prevValue = HeaderFilterButtonShowMode;
				headerFilterButtonShowMode = value;
				OnOptionsChanged();
			}
		}
		[
		Description("Gets or sets a value specifying when buttons of in-place editors are shown in cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.ShowButtonMode"),
		DefaultValue(PivotShowButtonModeEnum.Default), XtraSerializableProperty()
		]
		public virtual PivotShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(showButtonMode == value) return;
				showButtonMode = value;
				OnOptionsChanged();
			}
		}
		[Description("Gets or sets whether hidden fields are grouped within the Customization Form (OLAP).")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.GroupFieldsInCustomizationWindow")]
		[DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool GroupFieldsInCustomizationWindow {
			get { return groupFieldsInCustomizationWindow; }
			set {
				if(value == GroupFieldsInCustomizationWindow) return;
				groupFieldsInCustomizationWindow = value;
				OnOptionsChanged();
			}
		}
		protected internal virtual FilterButtonShowMode GetHeaderFilterButtonShowMode() {
			return HeaderFilterButtonShowMode == FilterButtonShowMode.Default ? FilterButtonShowMode.SmartTag : HeaderFilterButtonShowMode;
		}
		[Description("Gets or sets a value which specifies the distance from the separator line to the adjacent areas.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.FilterSeparatorBarPadding")]
		[DefaultValue(1), XtraSerializableProperty()]
		public int FilterSeparatorBarPadding {
			get { return filterSeparatorBarPadding; }
			set {
				if(value == FilterSeparatorBarPadding) return;
				filterSeparatorBarPadding = value;
				OnOptionsChanged();
			}
		}
		internal PivotGridOptionsSeparator ColumnValueSeparator {
			get { return columnValueSeparator; }
		}
		internal PivotGridOptionsSeparator RowValueSeparator {
			get { return rowValueSeparator; }
		}
	}
	public class PivotGridOptionsHint : PivotGridOptionsBase {
		bool showCellHints;
		bool showValueHints;
		bool showHeaderHints;
		public PivotGridOptionsHint() {
			this.showCellHints = true;
			this.showValueHints = true;
			this.showHeaderHints = true;
		}
		[Description("Gets or sets whether hints are displayed for cells with truncated content."), DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCellHints { get { return showCellHints; } set { showCellHints = value; } }
		[Description("Gets or sets whether hints are displayed for field values with truncated content."), DefaultValue(true), XtraSerializableProperty()]
		public bool ShowValueHints { get { return showValueHints; } set { showValueHints = value; } }
		[Description("Gets or sets whether hints are displayed for field headers that have truncated captions."), DefaultValue(true), XtraSerializableProperty()]
		public bool ShowHeaderHints { get { return showHeaderHints; } set { showHeaderHints = value; } }
	}
	public class PivotGridOptionsMenu : PivotGridOptionsBase {
		bool enableHeaderMenu;
		bool enableHeaderAreaMenu;
		bool enableFieldValueMenu;
		public PivotGridOptionsMenu() {
			this.enableHeaderMenu = true;
			this.enableHeaderAreaMenu = true;
			this.enableFieldValueMenu = true;
		}
		[Description("Gets or sets whether end-users can invoke the field header context menu."), DefaultValue(true), XtraSerializableProperty()]
		public bool EnableHeaderMenu { get { return enableHeaderMenu; } set { enableHeaderMenu = value; } }
		[Description("Gets or sets whether end-users can invoke the header area context menu."), DefaultValue(true), XtraSerializableProperty()]
		public bool EnableHeaderAreaMenu { get { return enableHeaderAreaMenu; } set { enableHeaderAreaMenu = value; } }
		[Description("Gets or sets whether end-users can invoke the field value context menu."), DefaultValue(true), XtraSerializableProperty()]
		public bool EnableFieldValueMenu { get { return enableFieldValueMenu; } set { enableFieldValueMenu = value; } }
	}
	public class PivotGridOptionsSelection : PivotGridOptionsBase {
		bool cellSelection;
		bool enableAppearanceFocusedCell;
		int maxWidth, maxHeight;
		bool multiSelect;
		public PivotGridOptionsSelection() {
			this.cellSelection = true;
			this.enableAppearanceFocusedCell = false;
			this.maxWidth = -1;
			this.maxHeight = -1;
			this.multiSelect = true;
		}
		[Description("Gets or sets whether multiple cells can be selected."), DefaultValue(true), XtraSerializableProperty()]
		public bool CellSelection { get { return cellSelection; } set { cellSelection = value; } }
		[Description("Gets or sets whether the appearance settings used to paint the focused cell are enabled."), DefaultValue(false), XtraSerializableProperty()]
		public bool EnableAppearanceFocusedCell { get { return enableAppearanceFocusedCell; } set { enableAppearanceFocusedCell = value; } }
		[Description("Gets the maximum number of columns that can be selected at the same time."), DefaultValue(-1), XtraSerializableProperty()]
		public int MaxWidth { get { return maxWidth; } set { maxWidth = value; } }
		[Description("Gets the maximum number of rows that can be selected at the same time."), DefaultValue(-1), XtraSerializableProperty()]
		public int MaxHeight { get { return maxHeight; } set { maxHeight = value; } }
		[Description("Gets or sets whether multiple range selection is enabled."), DefaultValue(true), XtraSerializableProperty()]
		public bool MultiSelect { get { return multiSelect; } set { multiSelect = value; } }
	}
	public class OptionsLayoutPivotGrid : OptionsLayoutGrid {
		bool addNewGroups;
		public OptionsLayoutPivotGrid() { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShouldSerialize() { return base.ShouldSerialize(); }
		[
		Description("Gets or sets whether the field groups that exist in the current control but do not exist in a layout when it's restored should be retained."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.AddNewGroups"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		DefaultValue(false)
		]
		public bool AddNewGroups { get { return addNewGroups; } set { addNewGroups = value; } }
	}
	public class PivotGridCells {
		PivotGridViewInfoData data;
		public PivotGridCells(PivotGridViewInfoData data) {
			this.data = data;
		}
		protected PivotGridViewInfoData Data { get { return data; } }
		protected PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
		[Description("Gets or sets the coordinates of the focused cell.")]
		public Point FocusedCell { get { return ViewInfo.FocusedCell; } set { ViewInfo.FocusedCell = value; } }
		[Description("Gets or sets the coordinates of the selected cells.")]
		public Rectangle Selection { get { return ViewInfo.Selection; } set { ViewInfo.Selection = value; } }
		[Description("Gets the pivot grid's selection.")]
		public IMultipleSelection MultiSelection { get { return ViewInfo; } }
		[Description("")]
		public void CopySelectionToClipboard() { ViewInfo.CopySelectionToClipboard(); }
		[Description("Gets the number of columns in the XtraPivotGrid control.")]
		public int ColumnCount { get { return ViewInfo.CellsArea.ColumnCount; } }
		[Description("Gets the number of rows in the XtraPivotGrid control.")]
		public int RowCount { get { return ViewInfo.CellsArea.RowCount; } }
		public PivotCellEventArgs GetCellInfo(int columnIndex, int rowIndex) {
			if(ViewInfo.CellsArea == null) return null;
			PivotCellViewInfo cellViewInfo = (PivotCellViewInfo)ViewInfo.CellsArea.CreateCellViewInfo(columnIndex, rowIndex);
			return cellViewInfo != null ? new PivotCellEventArgs(cellViewInfo) : null;
		}
		public PivotCellEventArgs GetFocusedCellInfo() {
			return GetCellInfo(FocusedCell.X, FocusedCell.Y);
		}
		public void InvalidateCell(int x, int y) {
			InvalidateCell(GetCellInfo(x, y));
		}
		public void InvalidateCell(PivotCellEventArgs cellInfo) {
			if(cellInfo == null || Data == null) return;
			Data.InvalidateCell(new Point(cellInfo.ColumnIndex, cellInfo.RowIndex));
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	public interface IPivotGridEventsImplementor {
		void DataSourceChanged();
		void BeginRefresh();
		void EndRefresh();
		void LayoutChanged();
		bool FieldAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex);
		object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex);
		void CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo);
		bool ShowingCustomizationForm(Form customizationForm, ref Control parentControl);
		void ShowCustomizationForm();
		void HideCustomizationForm();
		void OnPopupShowMenu(PivotGridMenuEventArgs e);
		void OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e);
		void BeforeLayoutLoad(LayoutAllowEventArgs e);
		void LayoutUpgrade(LayoutUpgadeEventArgs e);
		void FieldFilterChanged(PivotGridField field);
		void FieldAreaChanged(PivotGridField field);
		void FieldExpandedInFieldsGroupChanged(PivotGridField field);
		void FieldWidthChanged(PivotGridField field);
		string FieldValueDisplayText(PivotFieldsAreaCellViewInfo fieldCellViewInfo);
		string FieldValueDisplayText(PivotGridField field, object value);
		int GetCustomRowHeight(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int height);
		int GetCustomColumnWidth(PivotFieldsAreaCellViewInfo fieldCellViewInfo, int width);
		object CustomGroupInterval(PivotGridField field, object value);
		bool BeforeFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo);
		void AfterFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo);
		void AfterFieldValueChangeNotExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo);
		int FieldValueImageIndex(PivotFieldsAreaCellViewInfo fieldCellViewInfo);
		int GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder);
		void CellDoubleClick(PivotCellViewInfo cellViewInfo);
		void CellClick(PivotCellViewInfo cellViewInfo);
		string CustomCellDisplayText(PivotCellViewInfo cellViewInfo);
		bool CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds);
		bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds);
		bool CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter);
		bool CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, HeaderObjectPainter painter);
		bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo);
		void CustomAppearance(ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo);
		void FocusedCellChanged();
		void CellSelectionChanged();
		void PrefilterCriteriaChanged();
		void OLAPQueryTimeout();
		object CustomEditValue(object value, PivotCellViewInfo cellViewInfo);
		RepositoryItem GetCellEdit(PivotCellViewInfo cellViewInfo);
	}
	public interface IPivotGridDataOwner {
		void FireChanged(object[] changedObjects);
	}
	public class PivotGridViewInfoData : PivotGridData, IViewInfoControl, IXtraSerializable,
					IXtraSerializableLayout, IPrefilterOwner, IFilteredComponent, IXtraSerializableLayoutEx {
		internal PivotGridViewInfoBase viewInfo;
		UserLookAndFeel controlLookAndFeel;
		Rectangle customizationFormBounds;
		CustomizationForm customizationForm;
		PivotGridOptionsBehavior optionsBehavior;			
		OptionsLayoutPivotGrid optionsLayout;
		PivotGridOptionsHint optionsHint;
		PivotGridOptionsMenu optionsMenu;
		PivotGridOptionsSelection optionsSelection;
		IDXMenuManager menuManager;
		IViewInfoControl control;
		PivotGridCells cells;		
		public PivotGridViewInfoData() : this(null) { }
		public PivotGridViewInfoData(IViewInfoControl control) {
			this.control = control;
			this.eventsImplementor = control as IPivotGridEventsImplementor;
			this.viewInfo = CreateViewInfo();
			this.controlLookAndFeel = null;
			this.customizationFormBounds = Rectangle.Empty;
			this.customizationForm = null;
			this.optionsBehavior = new PivotGridOptionsBehavior(new EventHandler(OnOptionsBehaviorChanged));			
			this.optionsLayout = new OptionsLayoutPivotGrid();
			this.optionsHint = new PivotGridOptionsHint();
			this.optionsMenu = new PivotGridOptionsMenu();
			this.optionsSelection = new PivotGridOptionsSelection();
			this.menuManager = null;
			this.cells = new PivotGridCells(this);
			this.formatConditions = new PivotGridFormatConditionCollection();
			this.formatConditions.Data = this;
			this.formatConditions.CollectionChanged += new CollectionChangeEventHandler(OnFormatConditionChanged);
			this.appearance = new PivotGridAppearances();
			Appearance.Changed += new EventHandler(OnApperanceChanged);
			this.appearancePrint = CreatePivotGridAppearancesPrint();
			AppearancePrint.Changed += new EventHandler(OnApperancePrintChanged);
			this.paintAppearance = new PivotGridAppearances();
			this.paintAppearancePrint = new PivotGridAppearancesPrint();
			this.paintAppearanceDirty = true;
			this.paintAppearancePrintDirty = true;
			this.defaultAppearance = null;
			this.borderStyle = BorderStyles.Default;
		}
		protected virtual PivotGridAppearancesPrint CreatePivotGridAppearancesPrint() {
			return new PivotGridAppearancesPrint();
		}
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new PivotVisualItems(this);
		}
		protected override void Dispose(bool disposing) {
			if(this.customizationForm != null) {
				this.customizationForm.Dispose();
				this.customizationForm = null;
			}
			if(disposing) {
				this.viewInfo.Dispose();
				this.viewInfo = null;
			}
			if(disposing) {
				Appearance.Changed -= new EventHandler(OnApperanceChanged);
				this.appearance.Dispose();
				this.appearance = null;
				AppearancePrint.Changed -= new EventHandler(OnApperancePrintChanged);
				this.appearancePrint.Dispose();
				this.appearancePrint = null;
			}
			base.Dispose(disposing);
		}
		protected virtual PivotGridViewInfoBase CreateViewInfo() {
			return new PivotGridViewInfo(this);
		}
		protected override PrefilterBase CreatePrefilter() {
			return new Prefilter(this);
		}
		public bool IsEnabled {
			get { return Control != null ? ControlOwner.Enabled : true; }
		}
		public override bool IsDesignMode {
			get { return Control != null ? Control.IsDesignMode : false; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		public new PivotGridFieldCollection Fields { get { return (PivotGridFieldCollection)base.Fields; } }
		public PivotGridControl PivotGrid { get { return Control as PivotGridControl; } }
		protected override PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollection(this);
		}
		public void SetListSource(BindingContext context, object dataSource, string dataMember) {
			ListSource = MasterDetailHelper.GetDataSource(context, dataSource, dataMember);
		}
		public PivotGridViewInfoBase ViewInfo { 
			get {
				if(viewInfo != null && !Disposing)
					viewInfo.EnsureIsCalculated();
				return viewInfo; 
			} 
		}
		public UserLookAndFeel ControlLookAndFeel {
			get { return controlLookAndFeel; }
			set { controlLookAndFeel = value; }
		}
		public UserLookAndFeel ActiveLookAndFeel { get { return ControlLookAndFeel != null ? ControlLookAndFeel : UserLookAndFeel.Default; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new Prefilter Prefilter { get { return (Prefilter)base.Prefilter; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }		
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridOptionsChartDataSource();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new PivotGridOptionsChartDataSource OptionsChartDataSource { get { return (PivotGridOptionsChartDataSource)base.OptionsChartDataSource; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsLayoutPivotGrid OptionsLayout { get { return optionsLayout; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsHint OptionsHint { get { return optionsHint; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsMenu OptionsMenu { get { return optionsMenu; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsSelection OptionsSelection { get { return optionsSelection; } }
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		public bool ShowCustomizationTree {
			get {
				return IsOLAP && OptionsView.GroupFieldsInCustomizationWindow;
			}
		}
		public PivotGridCells Cells { get { return cells; } }
		protected override PivotGridFieldBase CreateDataField() {
			return new PivotGridField(this);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new PivotGridField DataField { get { return (PivotGridField)base.DataField; } }
		protected override PivotGridOptionsViewBase CreateOptionsView() { return new PivotGridOptionsView(new EventHandler(OnOptionsViewChanged)); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new PivotGridOptionsView OptionsView { get { return (PivotGridOptionsView)base.OptionsView; } }
		protected override PivotGridOptionsCustomization CreateOptionsCustomization() { return new PivotGridOptionsCustomizationEx(new EventHandler(OnOptionsChanged)); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new PivotGridOptionsCustomizationEx OptionsCustomization { get { return (PivotGridOptionsCustomizationEx)base.OptionsCustomization; } }
		protected override PivotGridOptionsDataField CreateOptionsDataField() {
			return new PivotGridOptionsDataFieldEx(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, 1)]
		public new PivotGridOptionsDataFieldEx OptionsDataField { get { return (PivotGridOptionsDataFieldEx)base.OptionsDataField; } }
		public CustomizationForm CustomizationForm { get { return customizationForm; } }
		public Rectangle CustomizationFormBounds { get { return customizationFormBounds; } set { customizationFormBounds = value; } }
		public override bool IsLoading {
			get {
				if(IsDeserializing) return true;
				if(ControlOwner is IComponentLoading)
					return ((IComponentLoading)ControlOwner).IsLoading;
				if(Control is IComponentLoading)
					return ((IComponentLoading)Control).IsLoading;
				return false;
			}
		}
		public void BestFit(PivotGridField field) {
			if(ViewInfo != null)
				ViewInfo.BestFit(field);
		}
		public void BestFit() {
			BestFitCore(GetFieldsByArea(PivotArea.DataArea, true));
			BestFitCore(GetFieldsByArea(PivotArea.ColumnArea, true));
			BestFitCore(GetFieldsByArea(PivotArea.RowArea, true));
		}
		protected void BestFitCore(List<PivotGridFieldBase> fields) {
			for(int i = fields.Count - 1; i >= 0; i--) {
				BestFit((PivotGridField)fields[i]);
			}
		}
		public override bool AllowHideFields {
			get {
				if(OptionsCustomization.AllowHideFields == AllowHideFieldsType.WhenCustomizationFormVisible)
					return CustomizationForm != null;
				return OptionsCustomization.AllowHideFields == AllowHideFieldsType.Always ? true : false;
			}
		}
		public override void OnPopupShowMenu(object e) {
			if(EventsImplementor != null)
				EventsImplementor.OnPopupShowMenu((PivotGridMenuEventArgs)e);
		}
		public override void OnPopupMenuItemClick(object e) {
			if(EventsImplementor != null)
				EventsImplementor.OnPopupMenuItemClick((PivotGridMenuItemClickEventArgs)e);
		}
		public void FocusedCellChanged() {
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.FocusedCellChanged();
		}
		public void CellSelectionChanged() {
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.CellSelectionChanged();
		}
		protected override void PrefilterCriteriaChanged() {
			base.PrefilterCriteriaChanged();
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.PrefilterCriteriaChanged();
		}
		protected override void OLAPQueryTimeout() {
			base.OLAPQueryTimeout();
			if(EventsImplementor != null)
				EventsImplementor.OLAPQueryTimeout();
		}
		public int GetFieldValueSeparator(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			PivotGridOptionsSeparator optionsSeparator = cellViewInfo.IsColumn ? OptionsView.ColumnValueSeparator : OptionsView.RowValueSeparator;
			if(optionsSeparator.AutoLevelCount == 0 || cellViewInfo.Level < optionsSeparator.AutoLevelCount)
				return optionsSeparator.Width;
			return 0;
		}
		public bool FieldAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			if(!field.CanChangeLocationTo(newArea)) return false;
			if(EventsImplementor != null && !Disposing)
				return EventsImplementor.FieldAreaChanging(field, newArea, newAreaIndex);
			else return true;
		}
		public void SetFieldAreaPosition(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			if(!DataField.Visible || field == DataField) {
				field.SetAreaPosition(newArea, newAreaIndex);
				return;
			}
			int correctedNewAreaIndex = CorrectNewAreaIndex(field, newArea, newAreaIndex);
			int newDataFieldIndex = GetNewDataFieldIndex(field, newAreaIndex, newArea);
			field.SetAreaPosition(newArea, correctedNewAreaIndex);
			DataField.AreaIndex = newDataFieldIndex;
		}
		int CorrectNewAreaIndex(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(newArea, true);
			fields.Remove(field);
			if(newAreaIndex < 0) newAreaIndex = 0;
			if(newAreaIndex > fields.Count) newAreaIndex = fields.Count;
			fields.Insert(newAreaIndex, field);
			fields.Remove(DataField);
			return fields.IndexOf(field);
		}
		int GetNewDataFieldIndex(PivotGridField field, int newAreaIndex, PivotArea newArea) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(DataField.Area, true);
			if(field.Area == DataField.Area)
				fields.Remove(field);
			if(newArea == DataField.Area)
				fields.Insert((newAreaIndex < fields.Count) ? newAreaIndex : fields.Count, field);
			return fields.IndexOf(DataField);
		}
		public string GetPivotFieldValueText(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(EventsImplementor != null && !Disposing)
				return EventsImplementor.FieldValueDisplayText(fieldCellViewInfo);
			else return fieldCellViewInfo.Text;
		}
		public override string GetPivotFieldValueText(PivotGridFieldBase field, object value) {
			if(EventsImplementor != null && !Disposing)
				return EventsImplementor.FieldValueDisplayText((PivotGridField)field, value);
			return base.GetPivotFieldValueText(field, value);
		}
		public override object GetCustomGroupInterval(PivotGridFieldBase field, object value) {
			if(EventsImplementor != null && !Disposing)
				return EventsImplementor.CustomGroupInterval((PivotGridField)field, value);
			return base.GetCustomGroupInterval(field, value);
		}
		public void ChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(!BeforeFieldValueChangeExpanded(fieldCellViewInfo)) return;
			if(ChangeExpanded(fieldCellViewInfo.IsColumn, fieldCellViewInfo.VisibleIndex, fieldCellViewInfo.IsCollapsed))
				AfterFieldValueChangeExpanded(fieldCellViewInfo);
			else
				AfterFieldValueChangeNotExpanded(fieldCellViewInfo);
		}
		public bool BeforeFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			return EventsImplementor != null ? EventsImplementor.BeforeFieldValueChangeExpanded(fieldCellViewInfo) : true;
		}
		public void AfterFieldValueChangeExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(EventsImplementor != null) EventsImplementor.AfterFieldValueChangeExpanded(fieldCellViewInfo);
		}
		public void AfterFieldValueChangeNotExpanded(PivotFieldsAreaCellViewInfo fieldCellViewInfo) {
			if(EventsImplementor != null) EventsImplementor.AfterFieldValueChangeNotExpanded(fieldCellViewInfo);
		}
		protected override void DoRefreshCore() {
			if(EventsImplementor != null && !Disposing) EventsImplementor.BeginRefresh();
			base.DoRefreshCore();
			if(EventsImplementor != null && !Disposing) EventsImplementor.EndRefresh();
		}
		public PivotFieldsAreaViewInfoBase GetAreaViewInfo(bool isColumn) {
			return isColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
		}
		public int GetVisibleIndex(PivotGridField field, int lastLevelIndex) {
			PivotFieldsAreaViewInfoBase viewInfo = field.IsColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
			if(lastLevelIndex < 0 || lastLevelIndex >= viewInfo.LastLevelItemCount) return -1;
			for(int i = viewInfo.GetLastLevelViewInfo(lastLevelIndex).VisibleIndex; i >= 0; i--)
				if(GetObjectLevel(field.IsColumn, i) == field.AreaIndex) return i;
			return -1;
		}
		public override int GetPivotFieldImageIndex(object fieldCellViewInfo) {
			if(EventsImplementor != null)
				return EventsImplementor.FieldValueImageIndex((PivotFieldsAreaCellViewInfo)fieldCellViewInfo);
			else return -1;
		}
		public string GetPivotCellText(PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomCellDisplayText(cellViewInfo);
			else return cellViewInfo.Text;
		}
		public int GetCustomRowHeight(PivotFieldsAreaCellViewInfo viewInfo, int height) {
			if(EventsImplementor != null)
				return EventsImplementor.GetCustomRowHeight(viewInfo, height);
			else
				return height;
		}
		public int GetCustomColumnWidth(PivotFieldsAreaCellViewInfo viewInfo, int width) {
			if(EventsImplementor != null)
				return EventsImplementor.GetCustomColumnWidth(viewInfo, width);
			else
				return width;
		}
		public RepositoryItem GetCellEdit(PivotCellViewInfo cellViewInfo) {
			return EventsImplementor != null ? EventsImplementor.GetCellEdit(cellViewInfo) : null;
		}
		protected override int GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			if(EventsImplementor != null)
				return EventsImplementor.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
			else return base.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		public void CellDoubleClick(PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CellDoubleClick(cellViewInfo);
		}
		public void CellClick(PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CellClick(cellViewInfo);
		}
		public bool CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawHeaderArea(headersViewInfo, paintArgs, bounds);
			else return false;
		}
		public bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawEmptyArea(appearanceOwner, paintArgs, bounds);
			else return false;
		}
		public bool CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawFieldHeader(headerViewInfo, paintArgs, painter);
			else return false;
		}
		public bool CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, HeaderObjectInfoArgs info, HeaderObjectPainter painter) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawFieldValue(paintArgs, fieldCellViewInfo, info, painter);
			else return false;
		}
		public bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawCell(paintArgs, ref appearance, cellViewInfo);
			else return false;
		}
		public void CustomAppearance(ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CustomAppearance(ref appearance, cellViewInfo);
		}
		public Point CustomizationFormDefaultLocation {
			get { return new Point(CustomizationFormBase.DefaultLocation, CustomizationFormBase.DefaultLocation); }
		}
		public override void ChangeFieldsCustomizationVisible() {
			if(IsFieldCustomizationShowing) DestroyCustomization();
			else FieldsCustomization();
		}
		public virtual void FieldsCustomization(Control parentControl, Point showPoint) {
			DestroyCustomization();
			if(ControlOwner == null) return;
			customizationForm = CreateCustomizationForm();
			CustomizationForm.LookAndFeel.ParentLookAndFeel = ActiveLookAndFeel;
			if(!OnShowingCustomizationForm(ref parentControl)) {
				CustomizationForm.ShowCustomization(parentControl, showPoint);
				OnShowCustomizationForm();
			} else {
				DestroyCustomizationCore(false);
			}
		}
		public void FieldsCustomization(Control parentControl) {
			FieldsCustomization(parentControl, CustomizationFormDefaultLocation);
		}
		public void FieldsCustomization(Point showPoint) {
			FieldsCustomization(null, showPoint);
		}
		public void FieldsCustomization() {
			FieldsCustomization(CustomizationFormDefaultLocation);
		}
		public void DestroyCustomization() {
			DestroyCustomizationCore(true);
		}
		void DestroyCustomizationCore(bool fireEvent) {
			if(customizationForm != null) {
				this.customizationFormBounds = CustomizationForm.Bounds;
				customizationForm.Dispose();
				customizationForm = null;
				if(fireEvent)
					OnHideCustomizationForm();
			}
		}
		public override bool IsFieldCustomizationShowing { get { return this.customizationForm != null; } }
		protected virtual CustomizationForm CreateCustomizationForm() {
			return new CustomizationForm(this);
		}
		public void ChangePrefilterVisible() {
			Prefilter.ChangePrefilterVisible();
		}
		public bool IsPrefilterFormShowing { get { return Prefilter.IsPrefilterFormShowing; } }
		public void XtraClearFields(XtraItemEventArgs e) {
			OptionsLayoutPivotGrid gridOptions = e.Options as OptionsLayoutPivotGrid;
			bool addNewColumns = (gridOptions != null && gridOptions.Columns.AddNewColumns);
			List<PivotGridField> commonFields = GetCommonFields(e.Item.ChildProperties);
			List<PivotGridField> newFields = GetNewFields(commonFields);
			if(!addNewColumns) {
				RemoveNewFields(newFields);
			} else {
				SetNew(newFields);
			}
		}
		List<PivotGridField> GetNewFields(List<PivotGridField> savingFields) {
			List<PivotGridField> newFields = new List<PivotGridField>();
			for(int i = Fields.Count - 1; i >= 0; i--) {
				PivotGridField field = Fields[i];
				if(!savingFields.Contains(field))
					newFields.Add(field);
			}
			return newFields;
		}
		void SetNew(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				field.IsNew = true;
		}
		List<PivotGridField> GetCommonFields(IXtraPropertyCollection layoutItems) {
			List<PivotGridField> commonFields = new List<PivotGridField>();
			if(layoutItems == null)
				return commonFields;
			foreach(XtraPropertyInfo pInfo in layoutItems) {
				PivotGridField commonField = XtraFindFieldsItem(new XtraItemEventArgs(this, Fields, pInfo)) as PivotGridField;
				if(commonField != null)
					commonFields.Add(commonField);
			}
			return commonFields;
		}
		void RemoveNewFields(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				Fields.Remove(field);
		}
		public object XtraCreateFieldsItem(XtraItemEventArgs e) {
			OptionsLayoutPivotGrid gridOptions = e.Options as OptionsLayoutPivotGrid;
			if(gridOptions != null) {
				XtraPropertyInfo propertyInfo = e.Item.ChildProperties["Name"];
				string name = propertyInfo != null ? propertyInfo.ValueToObject(typeof(string)) as string : null;
				if(gridOptions.Columns.RemoveOldColumns) return null;
			}
			return Fields.Add();
		}
		public object XtraFindFieldsItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(string.IsNullOrEmpty(name)) return null;
			PivotGridField field = Fields.FieldByName(name);
			return field;
		}
		public void XtraClearGroups(XtraItemEventArgs e) {
			OptionsLayoutPivotGrid gridOptions = e.Options as OptionsLayoutPivotGrid;
			bool clearGroups = gridOptions == null ? true : !gridOptions.AddNewGroups;
			if(clearGroups)
				Groups.Clear();
		}
		public override void FireChanged(object[] objs) {
			if(IsLockUpdate || IsLoading || Disposing) return;
			IPivotGridDataOwner dataOwner = (IPivotGridDataOwner)ControlOwner;
			if(dataOwner != null)
				dataOwner.FireChanged(objs);
		}
		protected override void LayoutChangedCore() {
			base.LayoutChangedCore();
			Invalidate();
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.LayoutChanged();
		}
		public void InvalidateViewInfo() {
			viewInfo.Clear();
		}
		public virtual void Invalidate() {
			Invalidate(ClientRectangle);
		}
		public virtual void Invalidate(Rectangle bounds) {
			if(!IsLockUpdate)
				InvalidateControl(bounds);
		}
		public void InvalidateCell(Point cell) {
			if(ViewInfo == null) return;
			ViewInfo.CellsArea.InvalidatedCell(cell);
		}
		public override void OnFieldVisibleChanged(PivotGridFieldBase field) {
			base.OnFieldVisibleChanged(field);
			PopupateCustomizationFormFields();
			FireChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldAreaChanged((PivotGridField)field);
		}
		protected internal void PopupateCustomizationFormFields() {
			if(CustomizationForm != null)
				CustomizationForm.Populate();
		}
		public override void OnGroupsChanged() {
			base.OnGroupsChanged();
			PopupateCustomizationFormFields();
		}
		public override void OnFieldFilteringChanged(PivotGridFieldBase field) {
			base.OnFieldFilteringChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldFilterChanged((PivotGridField)field);
		}
		public override void OnFieldWidthChanged(PivotGridFieldBase field) {
			base.OnFieldWidthChanged(field);
			FireChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldWidthChanged((PivotGridField)field);
		}
		public override void OnFieldAreaIndexChanged(PivotGridFieldBase field, bool doRefresh) {
			base.OnFieldAreaIndexChanged(field, doRefresh);
			FireChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldAreaChanged((PivotGridField)field);
		}
		public override void OnFieldAreaChanged(PivotGridFieldBase field) {
			base.OnFieldAreaChanged(field);
			FireChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldAreaChanged((PivotGridField)field);
		}
		public override void OnFieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
			base.OnFieldExpandedInFieldsGroupChanged(field);
			FireChanged(field);
			if(!IsLoading && EventsImplementor != null)
				EventsImplementor.FieldExpandedInFieldsGroupChanged((PivotGridField)field);
		}
		public override void OnColumnInsert(PivotGridFieldBase field) {
			base.OnColumnInsert(field);
			FireChanged(field);
		}
		public override void EndUpdate() {
			base.EndUpdate();
			FireChanged();
		}
		public override int DefaultFieldHeight { get { return ViewInfo.GetFieldHeight(1); } }
		public int GetFieldHeight(int lineCount) { return ViewInfo.GetFieldHeight(lineCount); }
		protected internal IViewInfoControl Control { get { return control; } }
		protected virtual void InvalidateControl(Rectangle bounds) {
			if(Control != null) Control.Invalidate(bounds);
		}
		IPivotGridEventsImplementor eventsImplementor;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IPivotGridEventsImplementor EventsImplementor { 
			get { return eventsImplementor; } 
			set { eventsImplementor = value; } 
		}
		protected override object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {
			if(EventsImplementor != null)
				return EventsImplementor.GetUnboundValue(field, listSourceRowIndex);
			else return null;
		}
		protected override void OnCalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CalcCustomSummary(field, customSummaryInfo);
		}
		protected virtual bool OnShowingCustomizationForm(ref Control parentControl) {
			if(EventsImplementor != null)
				return EventsImplementor.ShowingCustomizationForm(CustomizationForm, ref parentControl);
			return false;
		}
		protected virtual void OnShowCustomizationForm() {
			if(EventsImplementor != null)
				EventsImplementor.ShowCustomizationForm();
		}
		protected virtual void OnHideCustomizationForm() {
			if(EventsImplementor != null)
				EventsImplementor.HideCustomizationForm();
		}
		protected virtual void OnBeforeLayoutLoad(LayoutAllowEventArgs e) {
			if(EventsImplementor != null)
				EventsImplementor.BeforeLayoutLoad(e);
		}
		protected virtual void OnLayoutUpgrade(LayoutUpgadeEventArgs e) {
			if(EventsImplementor != null)
				EventsImplementor.LayoutUpgrade(e);
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			if(EventsImplementor != null)
				EventsImplementor.DataSourceChanged();
		}
		public Control ControlOwner { get { return Control != null ? Control.ControlOwner : null; } }
		internal IContainer GetComponentContainer() {
			if(ControlOwner is Component)
				return ((Component)ControlOwner).Container;
			if(Control is Component)
				return ((Component)Control).Container;
			return null;
		}
		public virtual Rectangle ClientRectangle {
			get {
				return Control != null ? Control.ClientRectangle : Rectangle.Empty;
			}
		}
		public virtual Rectangle ScrollableRectangle {
			get {
				Rectangle bounds = ClientRectangle;
				if(bounds.IsEmpty) return bounds;
				BorderObjectInfoArgs infoArgs = new BorderObjectInfoArgs();
				infoArgs.Bounds = bounds;
				BorderPainter painter = BorderHelper.GetGridPainter(BorderStyle, ActiveLookAndFeel);
				bounds = painter.GetObjectClientRectangle(infoArgs);
				return bounds;
			}
		}
		protected virtual void OnStartDeserializing(LayoutAllowEventArgs e) {
			OnBeforeLayoutLoad(e);
			if(!e.Allow) return;
			SetIsDeserializing(true);
			BeginUpdate();
		}
		protected virtual void OnEndDeserializing(string restoredVersion) {
			OnDeserializationComplete();			
			SetIsDeserializing(false);
			try {
				if(restoredVersion != OptionsLayout.LayoutVersion) OnLayoutUpgrade(new LayoutUpgadeEventArgs(restoredVersion));
			} finally {
				EndUpdate();
			}
		}
		void OnOptionsBehaviorChanged(object sender, EventArgs e) {
			ViewInfo.BoundsOffset = Size.Empty;
			ViewInfo.LeftTopCoord = Point.Empty;
			LayoutChanged();
		}
		Rectangle IViewInfoControl.ClientRectangle { get { return ClientRectangle; } }
		void IViewInfoControl.Invalidate(Rectangle bounds) {
			Invalidate(bounds);
		}
		Control IViewInfoControl.ControlOwner { get { return ControlOwner; } }
		void IViewInfoControl.UpdateScrollBars() {
			if(Control != null) Control.UpdateScrollBars();
		}
		void IViewInfoControl.InvalidateScrollBars() {
			if(Control != null) Control.InvalidateScrollBars();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {			
			OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		#region FormatConditions
		PivotGridFormatConditionCollection formatConditions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue)]
		public PivotGridFormatConditionCollection FormatConditions { get { return formatConditions; } }
		public void XtraClearFormatConditions(XtraItemEventArgs e) {
			FormatConditions.Clear();
		}
		public object XtraCreateFormatConditionsItem(XtraItemEventArgs e) {
			PivotGridStyleFormatCondition formatCondition = new PivotGridStyleFormatCondition();
			FormatConditions.Add(formatCondition);
			return formatCondition;
		}
		void OnFormatConditionChanged(object sender, CollectionChangeEventArgs e) {
			LayoutChanged();
			FireChanged();
		}
		#endregion
		#region Appearance
		PivotGridAppearances appearance;
		PivotGridAppearances paintAppearance;
		AppearanceDefaultInfo[] defaultAppearance;
		PivotGridAppearancesPrint appearancePrint;
		PivotGridAppearancesPrint paintAppearancePrint;
		bool paintAppearanceDirty;
		bool paintAppearancePrintDirty;
		BorderStyles borderStyle;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)]
		public PivotGridAppearances Appearance { get { return appearance; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)]
		public PivotGridAppearancesPrint AppearancePrint { get { return appearancePrint; } }
		public PivotGridAppearances PaintAppearance {
			get {
				if(this.paintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		public PivotGridAppearancesPrint PaintAppearancePrint {
			get {
				if(this.paintAppearancePrintDirty) UpdatePaintAppearancePrint();
				return paintAppearancePrint;
			}
		}
		void UpdatePaintAppearance() {
			this.paintAppearanceDirty = false;
			paintAppearance.Combine(Appearance, DefaultAppearance);
		}
		void UpdatePaintAppearancePrint() {
			this.paintAppearancePrintDirty = false;
			paintAppearancePrint.Combine(AppearancePrint, AppearancePrint.GetAppearanceDefaultInfo());
		}
		AppearanceDefaultInfo[] DefaultAppearance {
			get {
				if(this.defaultAppearance == null)
					this.defaultAppearance = Appearance.GetAppearanceDefaultInfo(ActiveLookAndFeel);
				return this.defaultAppearance;
			}
		}
		void OnApperanceChanged(object sender, EventArgs e) {
			paintAppearanceDirty = true;
			LayoutChanged();
			FireChanged();
		}
		void OnApperancePrintChanged(object sender, EventArgs e) {
			paintAppearancePrintDirty = true;
			FireChanged();
		}
		[DefaultValue(BorderStyles.Default), XtraSerializableProperty()]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		public void LookAndFeelChanged() {
			this.paintAppearanceDirty = true;
			this.defaultAppearance = null;
			LayoutChanged();
		}
		#endregion
		#region IXtraSerializableLayout Members
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		#endregion
		#region IPrefilterOwner Members
		Control IPrefilterOwner.ControlOwner { get { return ControlOwner; } }
		UserLookAndFeel IPrefilterOwner.ActiveLookAndFeel { get { return ActiveLookAndFeel; } }
		IFilteredComponent IPrefilterOwner.FilteredComponent { get { return this; } }
		#endregion
		#region IFilteredComponent Members
		FilterColumnCollection IFilteredComponent.CreateFilterColumnCollection() {
			FilterColumnCollection collection = new FilterColumnCollection();
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].Visible && (OptionsDataField.EnableFilteringByData || Fields[i].Area != PivotArea.DataArea))
					collection.Add(new FieldFilterColumn(Fields[i], MenuManager));
			return collection;
		}
		DevExpress.Data.Filtering.CriteriaOperator IFilteredComponent.RowCriteria {
			get { return Prefilter.Criteria; }
			set { Prefilter.Criteria = value; }
		}
		event EventHandler IFilteredComponent.PropertiesChanged { add { ; } remove { ; } }
		event EventHandler IFilteredComponent.RowFilterChanged { add { ; } remove { ; } }
		#endregion
		public object CustomEditValue(object value, PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor == null)
				return value;
			return EventsImplementor.CustomEditValue(value, cellViewInfo);
		}
		internal bool GetReadOnly(PivotGridField field) {
			if(field == null)
				return true;
			bool readOnly = field.Options.ReadOnly;
			string fieldName = GetFieldName(field);
			try {
				readOnly = readOnly || DataController.Columns[fieldName].ReadOnly;
			} catch {
			}
			return readOnly;
		}
		public Bitmap GetKPIBitmap(PivotKPIGraphic graphic, int state) {
			if(state != 0 && state != -1 && state != 1) throw new ArgumentException("state");
			if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined) throw new ArgumentException("graphic");
			return DevExpress.Utils.Controls.ImageHelper.CreateBitmapFromResources(
							PivotGridData.PivotGridImagesResourcePath + graphic.ToString() + "." + state.ToString() + ".png",
							typeof(PivotGridData).Assembly);
		}
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		#endregion
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			Appearance.Reset();
			AppearancePrint.Reset();
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {			
			OptionsLayoutPivotGrid opts = options as OptionsLayoutPivotGrid;
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIdAppearance:
					return opts.StoreAppearance || opts.Columns.StoreAppearance;
				case LayoutIdData:
					return opts.StoreDataSettings;
			}
			return true;
		}
	}
	public class PivotVisualItems : PivotVisualItemsBase {
		public PivotVisualItems(PivotGridViewInfoData data)
			: base(data) {
		}
		protected new PivotGridViewInfoData Data {
			get { return (PivotGridViewInfoData)base.Data; }
		}
		protected override PivotGridCellDataProviderBase CreateCellDataProvider() {
			return new PivotGridEditCellDataProvider(Data);
		}
	}
	public class FieldFilterColumn : FilterColumn {
		readonly PivotGridField field;
		readonly IDXMenuManager menuManager;
		protected PivotGridField Field { get { return field; } }
		protected IDXMenuManager MenuManager { get { return menuManager; } }
		PivotFieldValueRepositoryItemComboBox repositoryItem;
		protected RepositoryItemComboBox RepositoryItem {
			get {
				if(repositoryItem == null) {
					repositoryItem = new PivotFieldValueRepositoryItemComboBox();
					repositoryItem.Field = Field;
					repositoryItem.MenuManager = MenuManager;
					repositoryItem.Items.AddRange(CreateItems(field.GetUniqueValues()));
					repositoryItem.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
				}
				return repositoryItem;
			}
		}
		public FieldFilterColumn(PivotGridField field, IDXMenuManager menuManager) {
			this.field = field;
			this.menuManager = menuManager;
		}
		public override void Dispose() {
			if(repositoryItem != null) repositoryItem.Dispose();
			base.Dispose();
		}
		public override FilterColumnClauseClass ClauseClass { get { return ColumnType == typeof(string) ? FilterColumnClauseClass.String : FilterColumnClauseClass.Generic; } }
		public override string ColumnCaption { get { return Field.ToString(); } }
		public override RepositoryItem ColumnEditor { get { return RepositoryItem; } }
		public override Type ColumnType { get { return Field.DataType; } }
		public override string FieldName { get { return !string.IsNullOrEmpty(field.Name) ? field.Name : field.DataControllerColumnName; } }
		public override Image Image { get { return null; } }
		FilterItem[] CreateItems(object[] values) {
			FilterItem[] result = new FilterItem[values.Length];
			for(int i = 0; i < values.Length; i++)
				result[i] = new FilterItem(values[i], Field.GetDisplayText(values[i]));
			return result;
		}
	}
	internal class PivotFieldValueRepositoryItemComboBox : RepositoryItemComboBox {
		const string EditorName = "PivotFieldValueComboBoxEdit";
		PivotGridField field;
		IDXMenuManager menuManager;
		internal IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		internal PivotGridField Field { get { return field; } set { field = value; } }
		public PivotFieldValueRepositoryItemComboBox()
			: base() {
		}
		public override string EditorTypeName { get { return PivotFieldValueComboBoxEdit.EditorName; } }
		public override BaseEditPainter CreatePainter() {
			return new ButtonEditPainter();
		}
		public override BaseEdit CreateEditor() {
			BaseEdit editor = new PivotFieldValueComboBoxEdit(field);
			editor.MenuManager = MenuManager;
			return editor;
		}
		internal void SetOwnerEditInternal(BaseEdit edit) {
			SetOwnerEdit(edit);
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new ComboBoxViewInfo(this);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(Field != null && !(editValue is string)) {
				FilterItem item = editValue as FilterItem;
				if(item != null) return item.DisplayText;
				return Field.GetDisplayText(editValue);
			}
			return base.GetDisplayText(format, editValue);
		}
	}
	internal class PivotFieldValueComboBoxEdit : ComboBoxEdit {
		readonly PivotGridField field;
		public PivotFieldValueComboBoxEdit(PivotGridField field)
			: base() {
			this.field = field;
			((PivotFieldValueRepositoryItemComboBox)fProperties).Field = field;
		}
		internal const string EditorName = "PivotFieldValueComboBoxEdit";
		public new PivotFieldValueRepositoryItemComboBox Properties {
			get { return (PivotFieldValueRepositoryItemComboBox)base.Properties; }
		}
		public override string EditorTypeName { get { return EditorName; } }
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				foreach(FilterItem item in Properties.Items) {
					if(object.Equals(item.Value, value)) {
						base.EditValue = item;
						return;
					}
				}
				base.EditValue = value;
			}
		}
		protected override void CreateRepositoryItem() {
			fProperties = new PivotFieldValueRepositoryItemComboBox();
			((PivotFieldValueRepositoryItemComboBox)fProperties).SetOwnerEditInternal(this);
		}
	}
	internal class FilterItem : IConvertible {
		object value;
		string displayText;
		public object Value { get { return value; } set { this.value = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public FilterItem(object value, string displayText) {
			this.value = value;
			this.displayText = displayText;
		}
		public override string ToString() {
			return displayText;
		}
		#region IConvertible Members
		TypeCode IConvertible.GetTypeCode() {
			if(Value == null) return TypeCode.Object;
			return Type.GetTypeCode(Value.GetType());
		}
		bool IConvertible.ToBoolean(IFormatProvider provider) {
			return Convert.ToBoolean(Value);
		}
		byte IConvertible.ToByte(IFormatProvider provider) {
			return Convert.ToByte(Value);
		}
		char IConvertible.ToChar(IFormatProvider provider) {
			return Convert.ToChar(Value);
		}
		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			return Convert.ToDateTime(Value);
		}
		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			return Convert.ToDecimal(Value);
		}
		double IConvertible.ToDouble(IFormatProvider provider) {
			return Convert.ToDouble(Value);
		}
		short IConvertible.ToInt16(IFormatProvider provider) {
			return Convert.ToInt16(Value);
		}
		int IConvertible.ToInt32(IFormatProvider provider) {
			return Convert.ToInt32(Value);
		}
		long IConvertible.ToInt64(IFormatProvider provider) {
			return Convert.ToInt64(Value);
		}
		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			return Convert.ToSByte(Value);
		}
		float IConvertible.ToSingle(IFormatProvider provider) {
			return Convert.ToSingle(Value);
		}
		string IConvertible.ToString(IFormatProvider provider) {
			return Convert.ToString(Value);
		}
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			IConvertible convertibleValue = Value as IConvertible;
			if(convertibleValue != null)
				return convertibleValue.ToType(conversionType, provider);
			return Value;
		}
		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			return Convert.ToUInt16(Value);
		}
		uint IConvertible.ToUInt32(IFormatProvider provider) {
			return Convert.ToUInt32(Value);
		}
		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			return Convert.ToUInt64(Value);
		}
		#endregion
	}
}
