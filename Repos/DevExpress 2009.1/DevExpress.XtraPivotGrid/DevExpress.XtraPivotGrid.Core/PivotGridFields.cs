#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
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
using System.ComponentModel.Design;
using System.Data;
using System.Runtime.Serialization;
using System.IO;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
using DevExpress.WebUtils;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.ObjectModel;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotArea { RowArea = 0, ColumnArea = 1, FilterArea = 2, DataArea = 3 };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotDataArea { None, ColumnArea, RowArea };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotTotalsVisibility { AutomaticTotals, CustomTotals, None }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotTotalsLocation { Near, Far }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSortOrder { Ascending, Descending };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSortMode { Default, Value, DisplayText, Custom, Key, None }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotTopValueType { Absolute, Percent };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotGroupInterval {
		Default, Date, DateDay, DateDayOfWeek,
		DateDayOfYear, DateWeekOfMonth, DateWeekOfYear,
		DateMonth, DateQuarter, DateYear,
		YearAge, MonthAge, WeekAge, DayAge,
		Alphabetical, Numeric, Hour, Custom
	}
	public enum PivotGridValueType { Value, Total, GrandTotal, CustomTotal };
	[Flags]
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotGridAllowedAreas { All = 15, RowArea = 1, ColumnArea = 2, FilterArea = 4, DataArea = 8 }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum AllowHideFieldsType { Never, Always, WhenCustomizationFormVisible };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotKPIType { None, Value, Goal, Status, Trend, Weight }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotKPIGraphic {
		None, ServerDefined, Shapes, TrafficLights, RoadSigns, Gauge, ReversedGauge, Thermometer, ReversedThermometer,
		Cylinder, ReversedCylinder, Faces, VarianceArrow, StandardArrow, StatusArrow, ReversedStatusArrow
	};
	internal class CurrentCultureEqualityComparer : IEqualityComparer {
		readonly ValueComparer valueComparer = new ValueComparer();
		readonly StringComparer stringComparer = StringComparer.CurrentCulture;
		bool IEqualityComparer.Equals(object x, object y) {
			return valueComparer.Compare(x, y) == 0;
		}
		int IEqualityComparer.GetHashCode(object obj) {
			return stringComparer.GetHashCode(obj);
		}
	}
	[Serializable]
	public class PivotGridFieldFilterValues : ISerializable {
		NullableHashtable values;
		PivotGridFieldBase field;
		PivotFilterType filterType;
		bool showBlanks;
		public PivotGridFieldFilterValues(PivotGridFieldBase field) {
			this.field = field;
			this.values = new NullableHashtable(0, new CurrentCultureEqualityComparer());
			this.filterType = PivotFilterType.Excluded;
			this.showBlanks = true;
		}
		[Description("Gets the field which owns the current collection.")]
		public PivotGridFieldBase Field { get { return field; } }
		[Description("Gets the number of elements in the PivotGridFieldFilterValues.Values array.")]
		public int Count { get { return values.Count; } }
		[Description("Gets whether the current collection contains values used to filter against the current field.")]
		public bool IsEmpty {
			get {
				if(FilterType == PivotFilterType.Excluded) return Count == 0;
				object[] uniqueValues = Field.GetUniqueValues();
				foreach(object value in uniqueValues)
					if(!Contains(value)) return false;
				return true;
			}
		}
		[Description("Gets whether the current PivotGridFieldFilterValues object specifies non-empty filter criteria.")]
		public bool HasFilter { get { return !IsEmpty || !ShowBlanks; } }
		[Description("Gets or sets an array of filter values."),
		XtraSerializableProperty(0)]
		public object[] Values {
			get {
				object[] fValues = new object[Count];
				values.CopyKeysTo(fValues, 0);
				return fValues;
			}
			set {
				if(value == null || value.Length == 0) {
					Clear();
					return;
				}
				values.Clear();
				for(int i = 0; i < value.Length; i++)
					values.Add(value[i], true);
				OnChanged();
			}
		}
		[Description("Gets or sets the values that are displayed in the current field.")]
		public object[] ValuesIncluded {
			get { return GetValues(PivotFilterType.Included); }
			set {
				this.filterType = PivotFilterType.Included;
				Values = value;
			}
		}
		[Description("Gets or sets which values are excluded from the current field.")]
		public object[] ValuesExcluded {
			get { return GetValues(PivotFilterType.Excluded); }
			set {
				this.filterType = PivotFilterType.Excluded;
				Values = value;
			}
		}
		object[] GetValues(PivotFilterType fType) {
			if(fType == FilterType)
				return Values;
			if(Field == null) return new object[0];
			ArrayList list = new ArrayList(Field.GetUniqueValues());
			object[] values = Values;
			for(int i = 0; i < values.Length; i++)
				list.Remove(values[i]);
			return (object[])list.ToArray(typeof(object));
		}
		[Description("Gets or sets whether the records which contain NULL values in the current field should be processed by the control.")]
		[XtraSerializableProperty(1)]
		public bool ShowBlanks {
			get { return showBlanks; }
			set {
				if(value == ShowBlanks) return;
				showBlanks = value;
				OnChanged();
			}
		}
		[Description("Gets or sets the field's filter type.")]
		[XtraSerializableProperty(2)]
		public PivotFilterType FilterType {
			get { return filterType; }
			set {
				if(FilterType == value) return;
				filterType = value;
				OnChanged();
			}
		}
		public void Add(object value) {
			if(values.ContainsKey(value)) return;
			values.Add(value, true);
			OnChanged();
		}
		public void Remove(object value) {
			if(!values.ContainsKey(value)) return;
			values.Remove(value);
			OnChanged();
		}
		public void Clear() {
			if(!HasFilter) return;
			values.Clear();
			this.showBlanks = true;
			OnChanged();
		}
		public bool Contains(object value) {
			return FilterType == PivotFilterType.Excluded ? !values.ContainsKey(value) : values.ContainsKey(value);
		}
		protected internal bool ContainsKey(object value) {
			return values.ContainsKey(value);
		}
		public void Assign(PivotGridFieldFilterValues filteredValues) {
			if(filteredValues == null) return;
			SetValues(filteredValues.Values, filteredValues.FilterType, filteredValues.ShowBlanks);
		}
		public bool IsEquals(object[] values) {
			if(this.values.Count != values.Length) return false;
			for(int i = 0; i < values.Length; i++) {
				if(!this.values.ContainsKey(values[i])) return false;
				if((bool)this.values[values[i]] == false) return false;
			}
			return true;
		}
		public void SetValues(object[] values, PivotFilterType filterType, bool showBlanks) {
			bool notEquals = !IsEquals(values),
				changed = notEquals || this.filterType != filterType || this.showBlanks != showBlanks;
			if(notEquals) {
				this.values.Clear();
				for(int i = 0; i < values.Length; i++)
					this.values.Add(values[i], true);
			}
			this.filterType = filterType;
			this.showBlanks = showBlanks;
			if(changed) OnChanged();
		}
		protected internal NullableHashtable GetHashtable() { return values; }
		protected virtual void OnChanged() {
			if(Field != null)
				Field.OnFilteredValueChanged();
		}
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			si.AddValue("FilterType", FilterType == PivotFilterType.Excluded ? 0 : 1);
			si.AddValue("ShowBlanks", ShowBlanks);
			object[] values = Values;
			for(int i = 0; i < values.Length; i++)
				si.AddValue("Item" + i.ToString(), values[i]);
		}
		protected internal void SaveToStream(TypedBinaryWriter writer, Type fieldType) {
			if(fieldType == null) {
				writer.WriteType(fieldType);
				return;
			}
			object[] values = Values;
			if(fieldType == typeof(object) && Count > 0) {
				foreach(object val in values) {
					if(val == null) continue;
					if(fieldType == typeof(object)) {
						fieldType = val.GetType();
					} else {
						if(fieldType != val.GetType()) throw new ArgumentException("All filter items must be of the same type");
					}
				}
			}
			writer.WriteType(fieldType);
			writer.Write((byte)FilterType);
			writer.Write(ShowBlanks);
			writer.Write(Count);
			for(int i = 0; i < Count; i++) {
				writer.WriteObject(values[i]);
			}
		}
		protected internal void LoadFromStream(TypedBinaryReader reader) {
			Type fieldType = reader.ReadType();
			if(fieldType == null) return;
			PivotFilterType filterType = (PivotFilterType)reader.ReadByte();
			bool showBlanks = reader.ReadBoolean();
			int count = reader.ReadInt32();
			object[] values = new object[count];
			for(int i = 0; i < count; i++) {
				values[i] = reader.ReadObject(fieldType);
			}
			SetValues(values, filterType, showBlanks);
		}
	}
	public class PivotGridCustomTotalBase {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3;
		PivotSummaryType summaryType;
		[NonSerialized]
		PivotGridCustomTotalCollectionBase collection;
		FormatInfo format;
		FormatInfo cellFormat;
		object tag;
		static FormatInfo countCellFormat;
		static FormatInfo[] defaultFormats;
		static PivotGridCustomTotalBase() {
			countCellFormat = new FormatInfo();
			countCellFormat.FormatType = FormatType.Numeric;
			countCellFormat.FormatString = "{0}";
			PivotGridStringId[] ids = new PivotGridStringId[] { 
			  PivotGridStringId.TotalFormatCount, PivotGridStringId.TotalFormatSum,
			  PivotGridStringId.TotalFormatMin, PivotGridStringId.TotalFormatMax, PivotGridStringId.TotalFormatAverage,
			  PivotGridStringId.TotalFormatStdDev, PivotGridStringId.TotalFormatStdDevp,
			  PivotGridStringId.TotalFormatVar, PivotGridStringId.TotalFormatVarp, PivotGridStringId.TotalFormatCustom};
			defaultFormats = new FormatInfo[ids.Length];
			for(int i = 0; i < defaultFormats.Length; i++) {
				defaultFormats[i] = new FormatInfo();
				defaultFormats[i].FormatType = FormatType.Numeric;
				defaultFormats[i].FormatString = PivotGridLocalizer.GetString(ids[i]);
			}
		}
		public PivotGridCustomTotalBase()
			: this(PivotSummaryType.Sum) {
		}
		public PivotGridCustomTotalBase(PivotSummaryType summaryType) {
			this.summaryType = summaryType;
			this.collection = null;
			this.format = new FormatInfo();
			this.format.Changed += new EventHandler(OnFormatChanged);
			this.cellFormat = new FormatInfo();
			this.cellFormat.Changed += new EventHandler(OnFormatChanged);
		}
		[
		Description("Gets or sets the type of the summary function used to calculate the current custom total."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.SummaryType"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(SummaryType == value) return;
				summaryType = value;
				OnChanged();
			}
		}
		void ResetFormat() { Format.Reset(); }		
		bool ShouldSerializeFormat() { return !Format.IsEmpty; }
		[
		Description("Provides the settings used to format the text within the custom total's headers."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.Format"),
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true), NotifyParentProperty(true)
		]
		public FormatInfo Format { get { return format; } }
		bool XtraShouldSerializeFormat() { return ShouldSerializeFormat(); }
		void ResetCellFormat() { CellFormat.Reset(); }		
		bool ShouldSerializeCellFormat() { return !CellFormat.IsEmpty; }
		[
		Description("Provides the settings used to format the custom total's values."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.CellFormat"),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true), NotifyParentProperty(true)
		]
		public FormatInfo CellFormat { get { return cellFormat; } }
		bool XtraShouldSerializeCellFormat() { return ShouldSerializeCellFormat(); }
		[
		Description("Gets or sets the data associated with the custom total."), XtraSerializableProperty(), Category("Data"), DefaultValue(null),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.Tag"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.Utils.Editors.UIObjectEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter)), NotifyParentProperty(true)
		]
		public object Tag { get { return tag; } set { tag = value; } }
		public string GetValueText(object value) {
			if(!Format.IsEmpty)
				return Format.GetDisplayText(value);
			if(Collection.Field != null)
				value = Collection.Field.GetValueText(value);
			return defaultFormats[(int)SummaryType].GetDisplayText(value);
		}
		public FormatInfo GetCellFormat() {
			if(SummaryType == PivotSummaryType.Count && CellFormat.IsEmpty)
				return countCellFormat;
			return CellFormat;
		}
		protected PivotGridCustomTotalCollectionBase Collection { get { return collection; } }
		protected virtual void OnChanged() {
			if(Collection != null)
				Collection.OnFieldChanged();
		}
		protected internal void SetCollection(PivotGridCustomTotalCollectionBase collection) {
			this.collection = collection;
		}
		void OnFormatChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override string ToString() {
			return SummaryType.ToString();
		}
		public virtual void CloneTo(PivotGridCustomTotalBase clone) {
			clone.SummaryType = SummaryType;
			clone.Format.Assign(Format);
			clone.CellFormat.Assign(CellFormat);
			clone.Tag = Tag;
		}
		public virtual bool IsEqual(PivotGridCustomTotalBase total) {
			return total.SummaryType == SummaryType &&
				total.Format.IsEquals(Format) &&
				total.CellFormat.IsEquals(CellFormat) &&
				total.SummaryType == SummaryType;
		} 
	}
	[ListBindable(false)]
	public class PivotGridCustomTotalCollectionBase : CollectionBase {
		PivotGridFieldBase field;
		public PivotGridCustomTotalCollectionBase() { }
		public PivotGridCustomTotalCollectionBase(PivotGridFieldBase field) : this(){
			this.field = field;
		}
		public PivotGridCustomTotalCollectionBase(PivotGridCustomTotalBase[] totals) : this() {
			AssignArray(totals);
		}
		public void AssignArray(PivotGridCustomTotalBase[] totals) {
			Clear();
			for(int i = 0; i < totals.Length; i++)
				Add(totals[i]);
		}
		public PivotGridCustomTotalBase[] CloneToArray() {
			PivotGridCustomTotalBase[] result = new PivotGridCustomTotalBase[Count];
			for(int i = 0; i < Count; i++) {
				result[i] = CreateCustomTotal();
				this[i].CloneTo(result[i]);
			}
			return result;
		}
		[Description("Provides indexed access to the elements in the collection."), NotifyParentProperty(true)]
		public PivotGridCustomTotalBase this[int index] { get { return InnerList[index] as PivotGridCustomTotalBase; } }
		[Description("Gets the field which owns the current collection."), NotifyParentProperty(true)]
		public PivotGridFieldBase Field {
			get { return field; }
			set { field = value; }
		}		
		public PivotGridCustomTotalBase Add(PivotSummaryType summaryType) {
			PivotGridCustomTotalBase customTotal = CreateCustomTotal();
			customTotal.SummaryType = summaryType;
			AddCore(customTotal);
			return customTotal;
		}
		protected virtual PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotalBase();
		}
		public void Add(PivotGridCustomTotalBase customTotal) {
			AddCore(customTotal);
		}
		public void Add(PivotGridCustomTotalCollectionBase customTotals) {
			for(int i = 0; i < customTotals.Count; i++){
				PivotGridCustomTotalBase total = this.Add(customTotals[i].SummaryType);
				total.CellFormat.Assign(customTotals[i].CellFormat);
				total.Format.Assign(customTotals[i].Format);
			}
		}
		public bool Contains(PivotSummaryType summaryType) {
			foreach(PivotGridCustomTotalBase customTotal in List) {
				if(customTotal.SummaryType == summaryType) 
					return true;
			}
			return false;
		}
		public void Remove(PivotGridCustomTotalBase customTotal) {
			List.Remove(customTotal);
		}
		protected virtual void AddCore(PivotGridCustomTotalBase customTotal) {
			customTotal.SetCollection(this);
			List.Add(customTotal);
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			NotifyField();
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			NotifyField();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			NotifyField();
		}
		protected virtual void NotifyField() {
			if(Field != null)
				Field.OnCustomTotalChanged();
		}
		protected internal void OnFieldChanged() {
			NotifyField();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridFieldSortBySummaryInfo : ViewStatePersisterCore {
		PivotGridFieldBase owner;
		PivotGridFieldBase field;
		string fieldName;
		string fieldComponentName;
		PivotSummaryType summaryType;
		PivotGridFieldSortConditionCollection conditions;
		public PivotGridFieldSortBySummaryInfo(PivotGridFieldBase owner, string objectPath)
			: base(owner, objectPath) {
			this.owner = owner;
			this.summaryType = PivotSummaryType.Sum;
			this.field = null;
			this.fieldName = string.Empty;
			this.fieldComponentName = string.Empty;
			this.conditions = new PivotGridFieldSortConditionCollection(Owner);
			Conditions.Changed += ConditionsChanged;
		}		
		public void Assign(PivotGridFieldSortBySummaryInfo sortInfo) {
			this.owner = sortInfo.owner;
			this.summaryType = sortInfo.summaryType;
			this.field = sortInfo.field;
			this.fieldName = sortInfo.fieldName;
			this.fieldComponentName = sortInfo.fieldComponentName;
			Conditions.AddRange(sortInfo.Conditions);
		}
		[
		Description("Gets or sets the field whose summary values define the order in which the values of the current column field or row field are arranged."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.Field"),
		DefaultValue(null), NotifyParentProperty(true)
		]
		public PivotGridFieldBase Field {
			get { return field; }
			set {
				if(value == Owner) value = null;
				if(Field == value) return;
				field = value;
				OnChanged();
			}
		}
		[
		Description("Contains conditions that identify the column or row whose values are sorted."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.Conditions"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotGridFieldSortConditionCollection Conditions {
			get { return conditions; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConditions() {
			return Conditions.Count > 0;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetConditions() {
			Conditions.Clear();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string FieldComponentName {
			get { return Field != null ? Field.ComponentName : string.Empty; }
			set { fieldComponentName = value; }
		}
		protected internal void OnGridDeserialized() {
			if(Owner == null || Owner.Collection == null || fieldComponentName == string.Empty) return;
			field = Owner.GetFieldFromComponentName(fieldComponentName);
			fieldComponentName = string.Empty;
			Conditions.Changed -= ConditionsChanged;
			for(int i = 0; i < Conditions.Count; i++) {
				if(string.IsNullOrEmpty(Conditions[i].FieldComponentName) || Conditions[i].Field != null) 
					continue;
				Conditions[i].Field = Owner.GetFieldFromComponentName(Conditions[i].FieldComponentName);
				Conditions[i].FieldComponentName = null;
			}
			Conditions.Changed += ConditionsChanged;
		}
		[
		Description("Gets or sets the field name of the field whose summary values define the order in which the values of the current column field or row field are arranged."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.FieldName"),
		Localizable(true),
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true),
		Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotColumnNameEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value) return;
				fieldName = value;
				OnChanged();
			}
		}
		[
		Description("Gets or sets the summary function type used to calculate the summary values which define the order in which the current column field's (row field's) values are arranged."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.SummaryType"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(SummaryType == value) return;
				summaryType = value;
				OnChanged();
			}
		}
		public bool ShouldSerialize() {
			return !string.IsNullOrEmpty(FieldName) || Field != null || SummaryType != PivotSummaryType.Sum || Conditions.Count > 0;
		}
		public void Reset() {
			SummaryType = PivotSummaryType.Sum;
			Field = null;
			FieldName = string.Empty;
			Conditions.Clear();
		}
		[Browsable(false)]
		public PivotGridFieldBase Owner { get { return owner; } }
		protected virtual void OnChanged() {
			if(Owner != null) Owner.OnSortBySummaryInfoChanged();
		}
		protected void ConditionsChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if(!string.IsNullOrEmpty(FieldName))
				sb.Append(FieldName).Append(" ");
			if(Field != null)
				sb.Append(Field).Append(" ");
			if(sb.Length == 0)
				sb.Append("(None)");
			else
				sb.Append(SummaryType);
			return sb.ToString();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateConditionsItem(XtraItemEventArgs e) {
			PivotGridFieldSortCondition res = new PivotGridFieldSortCondition();
			Conditions.Add(res);
			return res;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void XtraClearConditions(XtraItemEventArgs e) {
			Conditions.Clear();
		}
	}
	public class PivotGridFieldSortConditionCollection : Collection<PivotGridFieldSortCondition> {
		PivotGridFieldBase owner;
		protected PivotGridFieldBase Owner { get { return owner; } }
		protected bool IsDeserializing { get { return Owner.IsDataDeserializing; } }
		public PivotGridFieldSortConditionCollection(PivotGridFieldBase owner) {
			this.owner = owner;
		}
		public int IndexOf(PivotGridFieldBase field) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Field == field) 
					return i;
			}
			return -1;
		}
		public bool Contains(PivotGridFieldBase field) {
			return IndexOf(field) >= 0;
		}
		public PivotGridFieldSortCondition this[PivotGridFieldBase field] {
			get {
				int index = IndexOf(field);
				if(index < 0) return null;
				return this[index];
			}
		}
		public void AddRange(IList<PivotGridFieldSortCondition> conditions) {
			for(int i = 0; i < conditions.Count; i++) 
				Add(conditions[i]);
		}
		#region Collection overrides
		protected override void InsertItem(int index, PivotGridFieldSortCondition item) {
			CheckNewItem(item);
			base.InsertItem(index, item);
			SubscribeEvents(item);
			RaiseChanged();
		}		
		protected override void RemoveItem(int index) {
			UnsubscribeEvents(this[index]);
			base.RemoveItem(index);
			RaiseChanged();
		}
		protected override void SetItem(int index, PivotGridFieldSortCondition item) {
			base.SetItem(index, item);
			SubscribeEvents(item);
			RaiseChanged();
		}
		protected override void ClearItems() {
			for(int i = 0; i < Count; i++)
				UnsubscribeEvents(this[i]);
			base.ClearItems();
			RaiseChanged();
		}
		protected virtual void ItemChanged(PivotGridFieldSortCondition item) {
			RaiseChanged();
		}
		protected void CheckNewItem(PivotGridFieldSortCondition item) {
			if(!IsDeserializing && Contains(item.Field))
				throw new ArgumentException("Duplicate entry: collection already contains this field's condition.");
		}
		#endregion
		protected void SubscribeEvents(PivotGridFieldSortCondition item) {
			if(item == null) return;
			item.Changed += item_Changed;
		}		
		protected void UnsubscribeEvents(PivotGridFieldSortCondition item) {
			if(item == null) return;
			item.Changed -= item_Changed;
		}
		void item_Changed(object sender, EventArgs e) {
			ItemChanged((PivotGridFieldSortCondition)sender);
		}
		public event EventHandler Changed;
		protected void RaiseChanged() {
			if(Changed != null) 
				Changed(this, EventArgs.Empty);
		}
	}
	public class PivotGridFieldSortCondition {
		PivotGridFieldBase field;
		object value;
		string olapUniqueName;
		string fieldComponentName;
		public PivotGridFieldSortCondition() : this(null, null) { }
		public PivotGridFieldSortCondition(PivotGridFieldBase field, object value)
			: this(field, value, null) { }
		public PivotGridFieldSortCondition(PivotFieldValueItem item)
			: this(item.Field, item.Value, null) {
			IOLAPMember member = item.Data.GetOLAPMember(item.IsColumn, item.VisibleIndex);
			if(member != null)
				this.olapUniqueName = member.UniqueName;
		}
		public PivotGridFieldSortCondition(PivotGridFieldBase field, object value, string olapUniqueName) {
			this.field = field;
			this.value = value;
			this.olapUniqueName = olapUniqueName;
		}
		[
		Description(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.Field"),
		DefaultValue(null),
		]
		public PivotGridFieldBase Field { 
			get { return field; } 
			set {
				if(Field == value) return;
				field = value;
				RaiseChanged();
			} 
		}
		[
		DefaultValue(null), XtraSerializableProperty(),
		Description(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.Value"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Value { 
			get { return value; } 
			set {
				if(Value == value) return;
				this.value = value;
				RaiseChanged();
			} 
		}
		[
		DefaultValue(null), XtraSerializableProperty(),
		Description(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.OLAPUniqueMemberName"),
		]
		public string OLAPUniqueMemberName { 
			get { return olapUniqueName; } 
			set {
				if(OLAPUniqueMemberName == value) return;
				olapUniqueName = value;
				RaiseChanged();
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string FieldComponentName {
			get { return Field != null ? Field.ComponentName : fieldComponentName; }
			set { fieldComponentName = value; }
		}
		public event EventHandler Changed;
		protected void RaiseChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
	}	
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class PivotGridFieldBase : Component, IComponentLoading, IXtraSerializableLayoutEx, IViewBagOwner,
						IXtraSerializable {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3;
		protected const string FieldNameEqualsNameExceptionString = "The values of FieldName and Name(ID) properties can not be the same.";
		static string namePrefix = "field";
		[Browsable(false)]
		public static string NamePrefix { get { return namePrefix; } set { namePrefix = value; } }
		string name;
		string fieldName;
		UnboundColumnType unboundType;
		bool visible;
		PivotArea area;
		int areaIndex;
		int areaIndexOld;
		PivotGridFieldCollectionBase collection;
		PivotSummaryType summaryType;
		PivotSummaryDisplayType summaryDisplayType;
		int minWidth;
		int width;
		string caption;
		PivotGridFieldOptions options;
		string emptyCellText;
		string emptyValueText;
		string grandTotalText;
		PivotTotalsVisibility totalsVisibility;
		PivotGridFieldFilterValues filterValues;
		int columnHandle;
		PivotSortOrder sortOrder;
		PivotSortMode sortMode;
		PivotGridCustomTotalCollectionBase customTotals;
		PivotGridFieldSortBySummaryInfo sortBySummaryInfo;
		int topValueCount;
		PivotTopValueType topValueType;
		PivotGridData dataFieldData;
		PivotKPIGraphic kpiGraphic;
		bool topValueShowOthers;
		string unboundFieldName;
		int groupIntervalColumnHandle;
		PivotGroupInterval groupInterval;
		int groupIntervalNumericRange;
		bool expandedInFieldsGroup;
		PivotGridAllowedAreas allowedAreas;
		object tag;
		int indexInternal;
		bool isNew;
		bool runningTotal;
		bool isDisposed;
		FormatInfo cellFormat, totalCellFormat, grandTotalCellFormat, valueFormat, totalValueFormat;
		DefaultBoolean useNativeFormat;
		static FormatInfo defaultDateFormat = new FormatInfo();
		static FormatInfo defaultTotalFormat = new FormatInfo();
		static FormatInfo defaultDecimalFormat = new FormatInfo();
		static FormatInfo defaultPercentFormat = new FormatInfo();
		static PivotGridFieldBase() {
			defaultDateFormat.FormatType = FormatType.DateTime;
			defaultDateFormat.FormatString = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			defaultTotalFormat.FormatType = FormatType.Numeric;
			defaultTotalFormat.FormatString = PivotGridLocalizer.GetString(PivotGridStringId.TotalFormat);
			defaultDecimalFormat.FormatType = FormatType.Numeric;
			defaultDecimalFormat.FormatString = "c";
			defaultPercentFormat.FormatType = FormatType.Numeric;
			defaultPercentFormat.FormatString = "p";
		}
		[Description("Provides access to global formatting settings for decimal values.")]
		public static FormatInfo DefaultDecimalFormat { get { return defaultDecimalFormat; } }
		[Description("Provides access to global formatting settings for percent values.")]
		public static FormatInfo DefaultPercentFormat { get { return defaultPercentFormat; } }
		public PivotGridFieldBase() : this(string.Empty, PivotArea.FilterArea) { }
		public PivotGridFieldBase(PivotGridData dataFieldData)
			: this(string.Empty, PivotArea.ColumnArea) {
			this.dataFieldData = dataFieldData;
			if(IsDataField) {
				UnboundFieldName = "pivot$dataField$";
				UnboundType = UnboundColumnType.Integer;
				Options.AllowSort = DefaultBoolean.False; 
				Options.AllowFilter = DefaultBoolean.False;
				Options.AllowExpand = DefaultBoolean.False;
			}
		}
		public PivotGridFieldBase(string fieldName, PivotArea area) {
			this.name = string.Empty;
			this.fieldName = fieldName;
			this.unboundType = UnboundColumnType.Bound;
			this.visible = true;
			this.area = area;
			this.areaIndex = -1;
			this.collection = null;
			this.summaryType = PivotSummaryType.Sum;
			this.summaryDisplayType = PivotSummaryDisplayType.Default;
			this.width = -1;
			this.minWidth = -1;
			this.caption = string.Empty;
			this.emptyCellText = string.Empty;
			this.emptyValueText = string.Empty;
			this.grandTotalText = string.Empty;
			this.sortOrder = PivotSortOrder.Ascending;
			this.sortMode = PivotSortMode.Default;
			this.filterValues = new PivotGridFieldFilterValues(this);
			this.customTotals = CreateCustomTotals();
			this.sortBySummaryInfo = new PivotGridFieldSortBySummaryInfo(this, "SortBySummaryInfo");
			this.options = CreateOptions(new EventHandler(OnOptionsChanged), "Options");
			this.columnHandle = -1;
			this.unboundFieldName = string.Empty;
			this.groupInterval = PivotGroupInterval.Default;
			this.groupIntervalNumericRange = 10;
			this.groupIntervalColumnHandle = -1;
			this.topValueCount = 0;
			this.topValueType = PivotTopValueType.Absolute;
			this.topValueShowOthers = false;
			this.expandedInFieldsGroup = true;
			this.allowedAreas = PivotGridAllowedAreas.All;
			this.tag = null;
			this.indexInternal = -1;
			this.isNew = false;
			this.runningTotal = false;
			this.kpiGraphic = PivotKPIGraphic.ServerDefined;
			this.cellFormat = new FormatInfo(this, this, "CellFormat");
			CellFormat.Changed += new EventHandler(OnFormatChanged);
			this.totalCellFormat = new FormatInfo(this, this, "TotalCellFormat");
			TotalCellFormat.Changed += new EventHandler(OnFormatChanged);
			this.grandTotalCellFormat = new FormatInfo(this, this, "GrandTotalCellFormat");
			GrandTotalCellFormat.Changed += new EventHandler(OnFormatChanged);
			this.valueFormat = new FormatInfo(this, this, "ValueFormat");
			ValueFormat.Changed += new EventHandler(OnFormatChanged);
			this.totalValueFormat = new FormatInfo(this, this, "TotalValueFormat");
			TotalValueFormat.Changed += new EventHandler(OnFormatChanged);
			this.useNativeFormat = DefaultBoolean.Default;
		}		
		protected override void Dispose(bool disposing) {
			if(disposing) {
				isDisposed = true;
				if(Group != null) {
					Group.Remove(this);
				}
				if(Collection != null) {
					PivotGridFieldCollectionBase cols = Collection;
					this.collection = null;
					cols.Remove(this);
				}
				if(CellFormat != null) {
					CellFormat.Changed -= new EventHandler(OnFormatChanged);
					TotalCellFormat.Changed -= new EventHandler(OnFormatChanged);
					GrandTotalCellFormat.Changed -= new EventHandler(OnFormatChanged);
					ValueFormat.Changed -= new EventHandler(OnFormatChanged);
					TotalValueFormat.Changed -= new EventHandler(OnFormatChanged);
					this.cellFormat = null;
					this.totalCellFormat = null;
					this.grandTotalCellFormat = null;
					this.valueFormat = null;
					this.totalValueFormat = null;
				}				
			}
			base.Dispose(disposing);
		}
		public void Assign(PivotGridFieldBase field) {
			this.Area = field.Area;
			this.AreaIndex = field.AreaIndex;
			this.Caption = field.Caption;
			this.CellFormat.Assign(field.CellFormat);
			this.CustomTotals.Add(field.CustomTotals);
			this.EmptyCellText = field.EmptyCellText;
			this.EmptyValueText = field.EmptyValueText;
			this.FieldName = field.FieldName;
			this.FilterValues.Assign(field.FilterValues);
			this.GrandTotalCellFormat.Assign(field.GrandTotalCellFormat);
			this.GrandTotalText = field.GrandTotalText;
			this.GroupInterval = field.GroupInterval;
			this.Options.Assign(field.Options);
			this.SortBySummaryInfo.Assign(field.SortBySummaryInfo);
			this.SortMode = field.SortMode;
			this.SortOrder = field.SortOrder;
			this.SummaryDisplayType = field.SummaryDisplayType;
			this.SummaryType = field.SummaryType;
			this.TopValueCount = field.TopValueCount;
			this.TopValueShowOthers = field.TopValueShowOthers;
			this.TopValueType = field.TopValueType;
			this.TotalCellFormat.Assign(field.TotalCellFormat);
			this.TotalsVisibility = field.TotalsVisibility;
			this.TotalValueFormat.Assign(field.TotalValueFormat);
			this.UnboundType = field.UnboundType;
			this.UnboundFieldName = field.UnboundFieldName;
			this.UseNativeFormat = field.UseNativeFormat;
			this.ValueFormat.Assign(field.ValueFormat);
			this.Visible = field.Visible;
			this.RunningTotal = field.RunningTotal;
		}
		protected virtual PivotGridFieldOptions CreateOptions(EventHandler eventHandler, string name) {
			return new PivotGridFieldOptions(eventHandler, this, "Options");
		}
		internal bool IsDisposed { get { return isDisposed; } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(-1), 
		Localizable(false), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Name")]
		public virtual string Name {
			get {
				if(IsDataDeserializing) return name;
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value && value != string.Empty) {
					if(!IsDeserializing) throw new Exception(FieldNameEqualsNameExceptionString);
					else value = NamePrefix + value;
				}
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		protected internal virtual string ComponentName { get { return Name; } }
		protected internal virtual PivotGridFieldBase GetFieldFromComponentName(string componentName) {
			return Collection.GetFieldByName(componentName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetNameCore(string name) { this.name = name; }
		[Description("Gets or sets the area of the XtraPivotGrid in which the field is displayed. "), Category("Behaviour"),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 0), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Area")]
		public PivotArea Area {
			get { return GetArea(); }
			set {
				if(Area == value || !IsAreaAllowed(value)) return;
				SetArea(value);
			}
		}
		protected virtual PivotArea GetArea() {
			if(IsDataField) {
				return Data.OptionsDataField.DataFieldArea;
			}
			if(Group != null)
				area = Group.Area;
			return area;
		}
		protected virtual void SetArea(PivotArea value) {
			if(IsDataField) {
				Data.OptionsDataField.DataFieldArea = value;
				return;
			}
			if(!CanChangeArea) return;
			area = value;
			if(!IsLoading)
				this.areaIndex = -1;
			UpdateAreaIndex();
			OnAreaChanged(true);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanChangeLocationTo(PivotArea newArea) {
			if(IsDataField) {
				return newArea == PivotArea.ColumnArea || newArea == PivotArea.RowArea;
			}
			return IsAreaAllowed(newArea);
		}
		[Description("Gets or sets the field's index from among the other fields displayed within the same area. "), Category("Behaviour"),
		DefaultValue(-1), XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 1), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.AreaIndex")]
		public virtual int AreaIndex {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.DataFieldAreaIndex;
				}
				return areaIndex;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.DataFieldAreaIndex = value;
					return;
				}
				if(!CanChangeArea) return;
				if(value < 0) {
					this.areaIndex = -1;
					Visible = false;
					return;
				}
				if(AreaIndex == value && !IsDataDeserializing) return;
				if(Data != null && !IsDataDeserializing) this.visible = true;
				AreaIndexCore = value;
				UpdateAreaIndex();
				OnAreaIndexChanged(true);
			}
		}
		[Description("Gets or sets whether the field is visible."), Category("Behaviour"),
		DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 1000), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Visible")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public virtual bool Visible {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.DataFieldVisible;
				}
				if(Group != null)
					visible = Group.IsFieldVisible(this);
				return visible;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.DataFieldVisible = value;
					return;
				}
				if(Visible == value) return;
				if(!CanChangeArea) return;
				if(value && !IsAreaAllowed(Area)) return;
				visible = value;
				UpdateAreaIndex();
				OnVisibleChanged();
			}
		}
		[Description("Gets or sets the areas within which the field can be positioned."), Category("Behaviour"),
		DefaultValue(PivotGridAllowedAreas.All), XtraSerializableProperty(), NotifyParentProperty(true),
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor))]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.AllowedAreas")]
		public PivotGridAllowedAreas AllowedAreas {
			get { return allowedAreas; }
			set {
				if(value == AllowedAreas) return;
				allowedAreas = value;
				if(!IsLoading && !IsAreaAllowed(Area))
					Visible = false;
			}
		}
		public bool IsAreaAllowed(PivotArea area) {
			if(IsDeserializing) return true;
			if(Data != null && !Data.IsAreaAllowed(this, area)) return false;
			if(AllowedAreas == PivotGridAllowedAreas.All) return true;
			int test = (int)Math.Pow(2, (int)area);		   
			return (test & (int)AllowedAreas) != 0;
		}
		[Description("Gets or sets whether Running totals are calculated for the current data field.")]
		[Category("Data"), XtraSerializableProperty(), DefaultValue(false), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.RunningTotal")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool RunningTotal {
			get { return runningTotal; }
			set {
				if(runningTotal == value)
					return;
				runningTotal = value;
				OnRunningTotalChanged();
			}
		}
		void OnRunningTotalChanged() {
			if(IsColumnOrRow)
				DoReloadData();
		}
		[Browsable(false)]
		public bool IsLoading { get { return Data == null || Data.IsLoading; } }
		protected internal virtual bool IsDataDeserializing { get { return Data != null && Data.IsDeserializing; } }
		internal PivotArea AreaCore { get { return area; } }
		internal bool VisibleCore { get { return visible; } }
		internal int AreaIndexCore {
			get { return areaIndex; }
			set {
				areaIndexOld = areaIndex;
				areaIndex = value;
				OnAreaIndexCoreChagned();
			}
		}
		protected virtual void OnAreaIndexCoreChagned() {
		}
		internal int AreaIndexOldCore { get { return areaIndexOld; } set { areaIndexOld = value; } }
		internal protected void FixOldAreaIndex() {
			if(AreaIndexCore >= 0 && AreaIndexOldCore == -1) 
				AreaIndexOldCore = Int32.MaxValue;
		}
		protected bool CanChangeArea { get { return (Group == null || Group.CanChangeArea(this)); } }
		protected void UpdateAreaIndex() {
			if(Group != null)
				Group.UpdateAreaIndexes();
			if(Collection != null)
				Collection.UpdateAreaIndexes();
		}
		[Browsable(false)]
		public bool IsColumnOrRow { get { return Visible && (Area == PivotArea.RowArea || Area == PivotArea.ColumnArea); } }
		[Browsable(false)]
		public bool IsColumn { get { return Visible && Area == PivotArea.ColumnArea; } }
		protected void BeginUpdate() {
			if(Data != null) Data.BeginUpdate();
		}
		protected void EndUpdate() {
			if(Data != null) Data.EndUpdate();
		}
		protected bool IsLockUpdate { get { return Data != null ? Data.IsLockUpdate : false; } }
		public bool SetAreaPosition(PivotArea area, int areaIndex) {
			if(!IsAreaAllowed(area)) return false;
			if(area == Area && AreaIndex == areaIndex && Visible) return false;
			bool oldVisible = Visible;
			PivotArea oldArea = Area;
			int oldAreaIndex = AreaIndex;
			BeginUpdate();
			try {
				this.visible = true;
				Area = area;
				AreaIndex = areaIndex;
			} finally {
				EndUpdate();
			}
			if(oldArea != Area || oldVisible != Visible)
				OnAreaChanged(false);
			else {
				if(oldAreaIndex != AreaIndex)
					OnAreaIndexChanged(false);
			}
			return true;
		}
		[Description("Gets or sets how the values of the current column or row field are combined into groups."), Category("Behaviour"),
		DefaultValue(PivotGroupInterval.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupInterval")]
		public PivotGroupInterval GroupInterval {
			get { return groupInterval; }
			set {
				if(GroupInterval == value) return;
				this.groupInterval = value;
				OnGroupIntervalChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsGroupIntervalNumeric {
			get {
				return GroupInterval == PivotGroupInterval.Numeric ||
					GroupInterval == PivotGroupInterval.YearAge || GroupInterval == PivotGroupInterval.MonthAge ||
					GroupInterval == PivotGroupInterval.WeekAge || GroupInterval == PivotGroupInterval.DayAge;
			}
		}
		[Description("Gets or sets the length of the intervals when values are grouped together. "), Category("Behaviour"),
		DefaultValue(10), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupIntervalNumericRange")]
		public int GroupIntervalNumericRange {
			get { return groupIntervalNumericRange; }
			set {
				if(value < 1) return;
				if(GroupIntervalNumericRange == value) return;
				this.groupIntervalNumericRange = value;
				OnGroupIntervalNumericRangeChanged();
			}
		}
		protected internal int GroupIntervalColumnHandle { get { return groupIntervalColumnHandle; } set { groupIntervalColumnHandle = value; } }
		[Category("Behaviour"), Browsable(false)]
		public virtual bool CanShowInCustomizationForm {
			get {
				if(!Options.ShowInCustomizationForm) return false;
				return !Visible && (Group == null || Group.IndexOf(this) == 0);
			}
		}
		[Browsable(false)]
		public virtual bool CanShowInPrefilter {
			get {
				return Area != PivotArea.DataArea;
			}
		}
		void ResetOptions() { Options.Reset(); }
		bool ShouldSerializeOptions() { return Options.ShouldSerialize(); }
		[Description("Contains the field's options."), Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Options")]
		public PivotGridFieldOptions Options { get { return options; } }
		protected internal void MakeVisible() {
			visible = true;
		}
		[
		Description("Gets or sets the name of the database field that is assigned to the current PivotGridFieldBase object."), Category("Data"),
		Localizable(true), DefaultValue(""), XtraSerializableProperty(), MergableProperty(false), NotifyParentProperty(true),
		Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotColumnNameEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor)),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.FieldName")
		]
		public virtual string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value) return;
				if(Name == value && value != string.Empty) {
					if(!IsDeserializing) throw new Exception(FieldNameEqualsNameExceptionString);
					else Name = NamePrefix + Name;
				}
				fieldName = value;
				OnFieldNameChanged();
			}
		}
		[Description("Gets or sets the name of a column in a summary data source that corresponds to the current unbound field."), Category("Data"),
		XtraSerializableProperty(), MergableProperty(false), DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundFieldName")]
		public string UnboundFieldName {
			get { return unboundFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(unboundFieldName == value) return;
				unboundFieldName = value;
				OnUnboundFieldNameChanged();
			}
		}
		public static bool IsOLAPFieldName(string fieldName) {
			return Regex.IsMatch(fieldName, @"\[([^\[\]]+)\]\.\[([^\[\]]+)\]\.\[([^\[\]]+)\]") ||
						Regex.IsMatch(fieldName, @"\[([^\[\]]+)\]\.\[([^\[\]]+)\]");
		}
		internal bool IsOLAPField {
			get { return IsOLAPFieldName(FieldName); }
		}
		internal bool IsOLAPMeasure {
			get { return FieldName.StartsWith("[Measures]."); }
		}
		internal string Hierarchy { 
			get {
				if(IsOLAPField && !IsOLAPMeasure)
					return FieldName.Substring(0, FieldName.LastIndexOf('.'));
				else
					return string.Empty;
			} 
		}
		internal virtual int Level { get { return Data != null ? Data.GetFieldHierarchyLevel(this) : 0; } }
		[Description("Gets the field's data type."), Category("Data"), Browsable(false)]
		public Type DataType { get { return Data != null ? Data.GetFieldType(this) : typeof(object); } }
		[
		Description("Gets or sets the data type and binding mode of the field."), Category("Data"),
		DefaultValue(UnboundColumnType.Bound), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundType")
		]
		public UnboundColumnType UnboundType {
			get { return unboundType; }
			set {
				if(UnboundType == value) return;
				unboundType = value;
				OnUnboundChanged();
			}
		}
		[Description("Gets or sets the field's display caption."), MergableProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Caption")]
		public string Caption {
			get { return GetCaptionCore(); }
			set {
				if(value == null) value = string.Empty;
				caption = value;
				OnCaptionChanged();
			}
		}
		[Browsable(false)]
		public string HeaderDisplayText {
			get {
				if(Options.ShowSummaryTypeName)
					return ToString() + " (" + SummaryType.ToString() + ")";
				else
					return ToString();
			}
		}
		protected virtual string GetCaptionCore() {
			if(!IsDataField) {
				string value;
				if(Data != null && Data.IsOLAP && string.IsNullOrEmpty(caption)) {
					value = Data.GetHierarchyCaption(FieldName);
					if(string.IsNullOrEmpty(value))
						value = caption;
				} else
					value = caption;
				return value == null ? "" : value;
			}
			if(Data.OptionsDataField.Caption != string.Empty) return Data.OptionsDataField.Caption;
			return PivotGridLocalizer.GetString(PivotGridStringId.DataFieldCaption);
		}
		[Description("Gets the text displayed by an empty cell."), NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyCellText")]
		public string EmptyCellText {
			get { return emptyCellText; }
			set {
				if(value == null) value = string.Empty;
				emptyCellText = value;
				OnEmptyCellTextChanged();
			}
		}
		[Description("Gets the text used for an empty field value header."), NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyValueText")]
		public string EmptyValueText {
			get { return emptyValueText; }
			set {
				if(value == null) value = string.Empty;
				emptyValueText = value;
				OnEmptyValueTextChanged();
			}
		}
		[Description("Gets or sets the text displayed within the Grand Total's header that corresponds to the current field."), NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalText")]
		public string GrandTotalText {
			get { return grandTotalText; }
			set {
				if(value == null) value = string.Empty;
				grandTotalText = value;
			}
		}
		internal int IndexInternal { get { return indexInternal; } set { indexInternal = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsNew { get { return isNew; } set { isNew = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public int Index {
			get { return Collection != null ? Collection.IndexOf(this) : -1; }
			set {
				if(IsDeserializing)
					IndexInternal = value;
				else {
					if(Collection != null)
						Collection.SetFieldIndex(this, value);
				}
			}
		}
		[Description("Gets or sets the type of the summary function which is calculated against the current data field."), Category("Data"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SummaryType")]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(SummaryType == value) return;
				summaryType = value;
				OnSummaryChanged();
			}
		}
		[Obsolete("Please use the SummaryDisplayType property instead", false)]
		[Description("Gets or sets how a summary value calculated against the current data field is represented in a cell."), Category("Data"), 
		DefaultValue(PivotSummaryVariation.None), XtraSerializableProperty(10), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotSummaryVariation SummaryVariation {
			get { return PivotSummaryDisplayTypeConverter.DisplayTypeToVariation(SummaryDisplayType); }
			set { SummaryDisplayType = PivotSummaryDisplayTypeConverter.VariationToDisplayType(value); }
		}
		[Description("Gets or sets how a summary value calculated against the current data field is represented in a cell."), Category("Data"),
		DefaultValue(PivotSummaryDisplayType.Default), XtraSerializableProperty(20), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SummaryDisplayType")]
		public PivotSummaryDisplayType SummaryDisplayType {
			get { return summaryDisplayType; }
			set {
				if(SummaryDisplayType == value) return;
				summaryDisplayType = value;
				OnSummaryChanged();
			}
		}
		void ResetSortBySummaryInfo() { SortBySummaryInfo.Reset(); }
		bool ShouldSerializeSortBySummaryInfo() { return SortBySummaryInfo.ShouldSerialize(); }
		[Description("Contains the settings used to sort the values of the current column field or row field by summary values in rows/columns."), Category("Behaviour"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortBySummaryInfo")]
		public PivotGridFieldSortBySummaryInfo SortBySummaryInfo { get { return sortBySummaryInfo; } }
		[Description("Gets or sets the absolute or relative number of field values that are to be displayed for the current column field or row field."), Category("Data"),
		DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueCount")]
		public int TopValueCount {
			get { return topValueCount; }
			set {
				if(value < 0) value = 0;
				if(TopValueCount == value) return;
				topValueCount = value;
				OnTopValuesChanged();
			}
		}
		[Description("Gets or sets how the number of Top Values is determined."), Category("Data"),
		DefaultValue(PivotTopValueType.Absolute), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueType")]
		public PivotTopValueType TopValueType {
			get { return topValueType; }
			set {
				if(TopValueType == value) return;
				topValueType = value;
				OnTopValuesChanged();
			}
		}
		[
		Description("Gets or sets whether the \"Others\" item is displayed within the XtraPivotGrid when the Top X Value feature is enabled."), Category("Data"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueShowOthers"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool TopValueShowOthers {
			get { return topValueShowOthers; }
			set {
				if(TopValueShowOthers == value) return;
				topValueShowOthers = value;
				OnTopValuesChanged();
			}
		}
		[Description("Gets or sets whether totals are displayed for the current field when it is positioned within the Column Header Area or Row Header Area and if so, whether they are automatic or custom."),
		Category("Behaviour"), DefaultValue(PivotTotalsVisibility.AutomaticTotals), XtraSerializableProperty(),
		Localizable(true), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalsVisibility")]
		public PivotTotalsVisibility TotalsVisibility {
			get { return totalsVisibility; }
			set {
				if(TotalsVisibility == value) return;
				totalsVisibility = value;
				OnTotalSummaryChanged();
			}
		}
		[Browsable(false)]
		public int GetTotalSummaryCount(bool singleValue) {
			if(Data == null) return 0;
			if(TotalsVisibility == PivotTotalsVisibility.CustomTotals) {
				return !singleValue || Data.OptionsView.ShowCustomTotalsForSingleValues ? CustomTotals.Count : 0;
			}
			if(TotalsVisibility == PivotTotalsVisibility.None)
				return 0;
			if(!IsColumnOrRow) return 0;
			bool show = (Area == PivotArea.ColumnArea ? Data.OptionsView.ShowColumnTotals : Data.OptionsView.ShowRowTotals);
			if(show && singleValue) {
				show = Data.OptionsView.ShowTotalsForSingleValues;
			}
			return show ? 1 : 0;
		}
		[Description("Gets whether the field has null values."), Browsable(false)]
		public bool HasNullValues { get { return Data != null ? Data.HasNullValues(this) : false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true)]
		public PivotGridFieldFilterValues FilterValues {
			get { return filterValues; }
			set {
				filterValues.Assign(value);
			}
		}
		protected internal void XtraAssignFilteredValues(PivotGridFieldFilterValues filteredValues) {
			FilterValues.Assign(filteredValues);
		}
		protected internal void OnGridDeserialized() {
			SortBySummaryInfo.OnGridDeserialized();
		}
		[Description(""),
		Editor("DevExpress.XtraPivotGrid.Design.CustomTotalsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridCore, 
		"System.Drawing.Design.UITypeEditor, System.Drawing")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.CustomTotals")]
		protected internal PivotGridCustomTotalCollectionBase CustomTotals { 
			get { return customTotals; }
		}
		[Description("Gets or sets the field's sort order. "), Category("Data"),
		DefaultValue(PivotSortOrder.Ascending), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortOrder")]
		public PivotSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				OnSortOrderChanged();
			}
		}
		[Description("Gets or sets how the field's data is sorted when sorting is applied to it."), XtraSerializableProperty(),
		Category("Data"), DefaultValue(PivotSortMode.Default), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortMode")]
		public PivotSortMode SortMode {
			get { return sortMode; }
			set {
				if(SortMode == value) return;
				sortMode = value;
				OnSortModeChanged();
			}
		}
		[Description("Gets or sets the width of each column which corresponds to the current field."), Category("Layout"),
		DefaultValue(100), XtraSerializableProperty(100), Localizable(true), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Width")]
		public virtual int Width {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.RowHeaderWidth;
				}
				if(width < 0)
					return Data != null ? Data.DefaultFieldWidth : 100;
				else return width;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.RowHeaderWidth = value;
					return;
				}
				if(value < -1) value = -1;
				if(value == Width) return;
				if(value >= 0 && value < minWidth) return;
				width = value;
				OnWidthChanged();
			}
		}
		[Description("Gets or sets the minimum width of each column which corresponds to the current field."),
		Category("Layout"), DefaultValue(PivotGridData.DefaultFieldMinWidth),
		XtraSerializableProperty(10), Localizable(true), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.MinWidth")]
		public int MinWidth {
			get {
				if(minWidth <= 0)
					return PivotGridData.DefaultFieldMinWidth;
				else return minWidth;
			}
			set {
				if(value < -1) value = -1;
				if(value > Width) value = Width;
				minWidth = value;
			}
		}
		void ResetCellFormat() { CellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeCellFormat() { return ShouldSerializeCellFormat(); }
		bool ShouldSerializeCellFormat() { return !CellFormat.IsEmpty; }
		[Description("Provides access to the formatting settings applied to cells."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.CellFormat")]
		public FormatInfo CellFormat { get { return cellFormat; } }
		void ResetTotalCellFormat() { TotalCellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeTotalCellFormat() { return ShouldSerializeTotalCellFormat(); }
		bool ShouldSerializeTotalCellFormat() { return !TotalCellFormat.IsEmpty; }
		[Description("Provides access to the formatting settings applied to total cells."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalCellFormat")]
		public FormatInfo TotalCellFormat { get { return totalCellFormat; } }
		void ResetGrandTotalCellFormat() { GrandTotalCellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeGrandTotalCellFormat() { return ShouldSerializeGrandTotalCellFormat(); }
		bool ShouldSerializeGrandTotalCellFormat() { return !GrandTotalCellFormat.IsEmpty; }
		[Description("Provides access to the formatting settings applied to grand total values."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalCellFormat")]
		public FormatInfo GrandTotalCellFormat { get { return grandTotalCellFormat; } }
		void ResetValueFormat() { ValueFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeValueFormat() { return ShouldSerializeValueFormat(); }
		bool ShouldSerializeValueFormat() { return !ValueFormat.IsEmpty; }
		[Description("Provides access to the formatting settings applied to field values."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.ValueFormat")]
		public FormatInfo ValueFormat { get { return valueFormat; } }
		void ResetTotalValueFormat() { TotalValueFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeTotalValueFormat() { return ShouldSerializeTotalValueFormat(); }
		bool ShouldSerializeTotalValueFormat() { return !TotalValueFormat.IsEmpty; }
		[Description("Provides access to the formatting settings applied to the total header."), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalValueFormat")]
		public FormatInfo TotalValueFormat { get { return totalValueFormat; } }
		[Description("Gets or sets the data associated with the field. "), XtraSerializableProperty(), Category("Data"), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true),
		Editor("DevExpress.Utils.Editors.UIObjectEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Tag")]
		public object Tag { get { return tag; } set { tag = value; } }
		[Description("Gets or sets whether to use the current field's data format when the pivot grid control is exported in XLS format."), NotifyParentProperty(true),
		Category("Data"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UseNativeFormat")]
		public DefaultBoolean UseNativeFormat { get { return useNativeFormat; } set { useNativeFormat = value; } }
		[Browsable(false)]
		public int ColumnHandle { get { return columnHandle; } }
		public void CollapseAll() {
			if(Data != null)
				Data.ChangeFieldExpanded(this, false);
		}
		public void ExpandAll() {
			if(Data != null)
				Data.ChangeFieldExpanded(this, true);
		}
		public void CollapseValue(object value) {
			if(Data != null)
				Data.ChangeFieldExpanded(this, false, value);
		}
		public void ExpandValue(object value) {
			if(Data != null)
				Data.ChangeFieldExpanded(this, true, value);
		}
		public virtual string GetValueText(object value) {
			if(value == null) return EmptyValueText;
			FormatInfo format = ValueFormat;
			if(GroupInterval == PivotGroupInterval.DateDayOfWeek && format.IsEmpty) {
				return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)value];
			}
			if(IsGroupIntervalNumeric) {
				return string.Format("{0} - {1}", format.GetDisplayText((int)value * GroupIntervalNumericRange), format.GetDisplayText(((int)value + 1) * GroupIntervalNumericRange - 1));
			}
			if(GroupInterval == PivotGroupInterval.DateMonth && value is int && (int)value >= 1 && (int)value <= 12) {
				if(format.IsEmpty)
					return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)value);
				else
					value = new DateTime(2000, (int)value, 1);
			}
			if(GroupInterval == PivotGroupInterval.Date && format.IsEmpty) {
				format = defaultDateFormat;
			}
			return format.GetDisplayText(value);
		}
		public virtual string GetTotalValueText(object value) {
			if(!TotalValueFormat.IsEmpty)
				return TotalValueFormat.GetDisplayText(value);
			else return defaultTotalFormat.GetDisplayText(GetValueText(value));
		}
		public virtual string GetTotalOthersText() {
			return defaultTotalFormat.GetDisplayText(PivotGridLocalizer.GetString(PivotGridStringId.TopValueOthersRow));
		}
		public virtual string GetGrandTotalText() {
			if(GrandTotalText != string.Empty)
				return GrandTotalText;
			else return ToString() + " " + PivotGridLocalizer.GetString(PivotGridStringId.Total);
		}		
		protected internal void SetColumnHandle(int columnHandle) {
			this.columnHandle = columnHandle;
		}
		protected internal PivotGridFieldCollectionBase Collection { get { return collection; } }
		protected virtual PivotGridData Data {
			get {
				if(IsDataField) {
					return dataFieldData;
				}
				return Collection != null ? Collection.Data : null;
			}
		}
		protected PivotGridGroupCollection Groups { get { return Data != null ? Data.Groups : null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridGroup Group { get { return Groups != null ? Groups.GetGroupByField(this) : null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int InnerGroupIndex { get { return Group != null ? Group.IndexOf(this) : -1; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int GroupIndex { get { return Group != null ? Groups.IndexOf(Group) : -1; } }
		[Obsolete("Please use ExpandedInFieldsGroup property"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsExpandedInFieldsGroup {
			get { return ExpandedInFieldsGroup; }
			set { ExpandedInFieldsGroup = value; }
		}
		[
		Description("Gets or sets the expansion status of the current field if it belongs to a field group."),
		Category("Data"),
		DefaultValue(true),
		XtraSerializableProperty(),
		NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.ExpandedInFieldsGroup"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ExpandedInFieldsGroup {
			get { return expandedInFieldsGroup; }
			set {
				if(expandedInFieldsGroup == value) return;
				expandedInFieldsGroup = value;
				OnExpandedInFieldsGroupChanged();
			}
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			AreaIndex = -1;
			Visible = true;
			AllowedAreas = PivotGridAllowedAreas.All;
			GroupInterval = PivotGroupInterval.Default;
			GroupIntervalNumericRange = 10;
			Options.Reset();
			UnboundType = UnboundColumnType.Bound;
			Caption = "";
			EmptyCellText = "";
			EmptyValueText = "";
			GrandTotalText = "";
			Width = 100;
			MinWidth = PivotGridData.DefaultFieldMinWidth;
			SummaryType = PivotSummaryType.Sum;
			SummaryDisplayType = PivotSummaryDisplayType.Default;
			SortOrder = PivotSortOrder.Ascending;
			SortMode = PivotSortMode.Default;
			ExpandedInFieldsGroup = true;
			SortBySummaryInfo.Reset();
			TopValueCount = 0;
			TopValueType = PivotTopValueType.Absolute;
			TopValueShowOthers = false;
			TotalsVisibility = PivotTotalsVisibility.AutomaticTotals;
			CellFormat.Reset();
			TotalCellFormat.Reset();
			GrandTotalCellFormat.Reset();
			ValueFormat.Reset();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsNextVisibleFieldInSameGroup {
			get {
				if(Group == null || !ExpandedInFieldsGroup) return false;
				int index = Group.IndexOf(this);
				return index < Group.Count - 1;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsUnbound {
			get { return GroupInterval != PivotGroupInterval.Default || UnboundType != UnboundColumnType.Bound; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsComplex {
			get {
				if(IsUnbound || IsOLAPField) return false;
				return !string.IsNullOrEmpty(FieldName) && (FieldName.Contains(".") || FieldName.Contains("!"));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string DataControllerColumnName {
			get {
				return !IsUnbound || IsOLAPField ? FieldName : UnboundFieldName;
			}
		}
		[
		Description("In OLAP mode, this method gets the name of the corresponding column in the underlying data source, on an OLAP server. "), Category("Data"),
		MergableProperty(false),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.OLAPDrillDownColumnName")
		]
		public string OLAPDrillDownColumnName {
			get {
				return Data != null ? Data.GetOLAPDrillDownColumnName(FieldName) : null;
			}
		}
		[Description("Gets the KPI type."), Category("KPI")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.KPIType")]
		public PivotKPIType KPIType {
			get {
				if(Data == null) return PivotKPIType.None;
				return Data.GetKPIType(this);
			}
		}
		[Description("Gets or sets a graphic set used to indicate KPI values."), Category("KPI"),
		DefaultValue(PivotKPIGraphic.ServerDefined), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.KPIGraphic")]
		public PivotKPIGraphic KPIGraphic {
			get { return kpiGraphic; }
			set {
				if(kpiGraphic == value) return;
				kpiGraphic = value;
				OnKPIGraphicChanged();
			}
		}
		public object[] GetUniqueValues() {
			if(Data == null) return new object[0];
			return Data.GetUniqueFieldValues(this);
		}
		public IOLAPMember[] GetOLAPMembers() {
			if(Data == null) return new IOLAPMember[0];
			return Data.GetOLAPColumnMembers(FieldName);
		}
		[Browsable(false)]
		public bool IsDesignTime { get { return DesignMode; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SelectedAtDesignTime {
			get {
				ISelectionService ss = GetSelectionService();
				return ss != null ? ss.GetComponentSelected(this) : false;
			}
			set {
				ISelectionService ss = GetSelectionService();
				if(ss != null)
					ss.SetSelectedComponents(new object[] { this });
			}
		}
		public override string ToString() {
			if(Caption != string.Empty) return Caption;
			if(FieldName != string.Empty) return FieldName;
			if(Site != null) return Site.Name;
			return string.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanFilter {
			get {
				if((Area == PivotArea.DataArea && Visible) || Data == null) return false;
				if(Options.AllowFilter == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowFilter;
				return Options.AllowFilter == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDrag {
			get {
				if(IsDesignTime) return true;
				if(Options.AllowDrag == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowDrag;
				return Options.AllowDrag == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanHide {
			get {
				if(IsDataField) return false;
				return Data.AllowHideFields;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanSort {
			get {
				if(Area == PivotArea.FilterArea || Area == PivotArea.DataArea || !Visible) return false;
				if(Options.AllowSort == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowSort;
				return Options.AllowSort == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanSortBySummary {
			get {
				if(IsDesignTime || !IsColumnOrRow) return false;
				if(Options.AllowSortBySummary == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowSortBySummary;
				return Options.AllowSortBySummary == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDataField { get { return dataFieldData != null; } }
		public bool CanShowValueType(PivotGridValueType valueType) {
			if(Area != PivotArea.DataArea) return true;
			return (valueType == PivotGridValueType.GrandTotal && Options.ShowGrandTotal
						&& !PivotSummaryDisplayTypeConverter.IsVariation(SummaryDisplayType))
				|| (valueType == PivotGridValueType.Total && Options.ShowTotals)
				|| (valueType == PivotGridValueType.CustomTotal && Options.ShowCustomTotals)
				|| (valueType == PivotGridValueType.Value && Options.ShowValues);
		}
		protected virtual ISelectionService GetSelectionService() {
			if(!DesignMode) return null;
			return GetService(typeof(ISelectionService)) as ISelectionService;
		}
		protected virtual PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollectionBase(this);
		}
		protected internal virtual void DoLayoutChanged() {
			if(Data != null) Data.LayoutChanged();
		}
		protected virtual void DoRefresh() {
			if(Data != null) Data.DoRefresh();
		}
		protected virtual void DoReloadData() {
			if(Data != null) Data.ReloadData();
		}
		protected void GroupFieldsByHierarchies() {
			if(Collection != null) Collection.GroupFieldsByHierarchies();
		}
		protected virtual void OnAreaChanged(bool doRefresh) {
			if(IsLockUpdate) return;
			if(Visible && doRefresh)
				DoRefresh();
			if(Data != null) Data.OnFieldAreaChanged(this);
		}
		protected virtual void OnAreaIndexChanged(bool doRefresh) {
			if(IsLockUpdate) return;
			if(Data != null) Data.OnFieldAreaIndexChanged(this, doRefresh);
		}
		protected virtual void OnVisibleChanged() {
			DoRefresh();
			if(Data != null) Data.OnFieldVisibleChanged(this);
		}
		protected virtual void OnFieldNameChanged() {
			if(Visible) {
				if(IsComplex)
					DoReloadData();
				else
					DoRefresh();
			}
			PivotGridGroup group = Group;
			if(group != null && group.IsOLAP && !group.IsFieldValid(this)) {
				group.Remove(this);
				if(group.Count == 0 && Data != null) Data.Groups.Remove(group); 
			}
			GroupFieldsByHierarchies();
		}
		protected void OnUnboundFieldNameChanged() {
			if(!Visible) return;
			if(IsUnbound) DoReloadData();
			else DoRefresh();
		}
		protected virtual void OnSummaryChanged() {
			if(Visible && Area == PivotArea.DataArea)
				DoRefresh();
		}
		protected virtual void OnTopValuesChanged() {
			if(Visible && IsColumnOrRow)
				DoRefresh();
		}
		protected virtual void OnCaptionChanged() {
			if(Visible) DoLayoutChanged();
		}
		protected virtual void OnKPIGraphicChanged() {
			if(Visible) DoLayoutChanged();
		}
		protected virtual void OnEmptyCellTextChanged() {
			if(Visible && Area == PivotArea.DataArea)
				DoLayoutChanged();
		}
		protected virtual void OnEmptyValueTextChanged() {
			if(Visible && IsColumnOrRow)
				DoLayoutChanged();
		}
		protected virtual void OnWidthChanged() {
			if(Visible && Data != null) {
				Data.OnFieldWidthChanged(this);
			}
		}
		protected internal virtual void OnFilteredValueChanged() {
			DoRefresh();
			if(Data != null) Data.OnFieldFilteringChanged(this);
		}
		protected virtual void OnSortOrderChanged() {
			if(Visible && IsColumnOrRow && Data != null) Data.OnSortOrderChanged(this);
		}
		protected virtual void OnSortModeChanged() {
			if(Visible && IsColumnOrRow && Data != null) Data.OnSortModeChanged(this);
		}
		protected virtual void OnTotalSummaryChanged() {
			if(Visible && IsColumnOrRow) DoLayoutChanged();
		}
		protected virtual void OnUnboundChanged() {
			if(!Visible) return;
			if(UnboundType == UnboundColumnType.Bound)
				DoRefresh();
			else DoReloadData();
		}
		protected virtual void OnGroupIntervalChanged() {
			if(!Visible) return;
			if(GroupInterval == PivotGroupInterval.Default)
				DoRefresh();
			else DoReloadData();
		}
		protected virtual void OnGroupIntervalNumericRangeChanged() {
			if(IsGroupIntervalNumeric)
				DoReloadData();
		}
		protected internal virtual void OnCustomTotalChanged() {
			if(IsColumnOrRow && Visible && TotalsVisibility == PivotTotalsVisibility.CustomTotals)
				DoLayoutChanged();
		}
		protected internal virtual void OnSortBySummaryInfoChanged() {
			if(IsColumnOrRow && Visible)
				DoRefresh();
		}
		protected internal virtual void OnExpandedInFieldsGroupChanged() {
			if(Group != null && Visible) {
				DoRefresh();
				Data.OnFieldExpandedInFieldsGroupChanged(this);
			}
		}
		void OnOptionsChanged(object sender, EventArgs e) {
			if(Visible) DoLayoutChanged();
		}
		protected internal virtual void SetCollection(PivotGridFieldCollectionBase collection) {
			this.collection = collection;
		}
		void OnFormatChanged(object sender, EventArgs e) {
			if(Visible && Area == PivotArea.DataArea)
				DoLayoutChanged();
		}
		T IViewBagOwner.GetViewBagProperty<T>(string objectName, string propertyName, T value) {
			return GetViewBagPropertyCore(objectName, propertyName, value);
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectName, string propertyName, T defaultValue, T value) {
			SetViewBagPropertyCore(objectName, propertyName, defaultValue, value);
		}
		protected virtual T GetViewBagPropertyCore<T>(string objectName, string propertyName, T value) {
			return value;
		}
		protected virtual void SetViewBagPropertyCore<T>(string objectName, string propertyName, T defaultValue, T value) {
		}
		#region IXtraSerializable Members
		bool deserializing;
		bool serializing;
		protected bool IsDeserializing { get { return deserializing; } }
		protected bool IsSerializing { get { return serializing; } }
		public void OnEndDeserializing(string restoredVersion) {
			deserializing = false;
		}
		public void OnEndSerializing() {
			serializing = false;
		}
		public void OnStartDeserializing(LayoutAllowEventArgs e) {
			e.Allow = true;
			deserializing = true;
		}
		public void OnStartSerializing() {
			serializing = true;
		}
		#endregion
	}
	[ListBindable(false)]
	public class PivotGridFieldCollectionBase : CollectionBase {
		const string DefaultNamePrefix = "field";
		PivotGridData data;
		public PivotGridFieldCollectionBase(PivotGridData data) {
			this.data = data;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridFieldBase this[int index] { get { return InnerList[index] as PivotGridFieldBase; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridFieldBase this[string fieldName] {
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].FieldName == fieldName)
						return this[i];
				return null;
			}
		}
		public PivotGridFieldBase GetFieldByName(string name) {
			for(int i = 0; i < Count; i++)
				if(this[i].Name == name)
					return this[i];
			return null;
		}
		public PivotGridFieldBase Add() {
			return Add(string.Empty, PivotArea.RowArea);
		}
		public void Add(PivotGridFieldBase field) {
			AddCore(field);
		}
		public PivotGridFieldBase Add(string fieldName, PivotArea area) {
			PivotGridFieldBase field = CreateField(fieldName, area);
			AddCore(field);
			return field;
		}
		public void Remove(PivotGridFieldBase field) {
			if(InnerList.Contains(field))
				List.Remove(field);
		}
		protected virtual PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			return new PivotGridFieldBase(fieldName, area);
		}
		protected void AddCore(PivotGridFieldBase field) {
			Data.CheckBound(field);
			field.FixOldAreaIndex();
			List.Add(field);			
		}		
		protected void InsertCore(int index, PivotGridFieldBase field) {
			Data.CheckBound(field);
			List.Insert(index, field);
		}
		public bool Contains(PivotGridFieldBase field) { return InnerList.Contains(field); }
		public int IndexOf(PivotGridFieldBase field) { return InnerList.IndexOf(field); }
		protected List<PivotGridFieldBase> GetFieldsByHierarchy(string hierarchy) {
			if(string.IsNullOrEmpty(hierarchy)) return new List<PivotGridFieldBase>();
			List<PivotGridFieldBase> result = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++)
				if(this[i].Hierarchy == hierarchy)
					result.Add(this[i]);
			return result;
		}
		public int GetVisibleFieldCount(PivotArea area) {
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].Visible && this[i].Area == area)
					count++;
			}
			return count;
		}
		public void UpdateAreaIndexes() {
			if(Data == null || Data.IsLoading || Data.IsDeserializing || Data.Disposing) return;
			int areaCount = Enum.GetValues(typeof(PivotArea)).Length;
			for(int i = 0; i < areaCount; i++)
				UpdateAreaIndexes((PivotArea)i);
		}
		public void SetFieldIndex(PivotGridFieldBase field, int newIndex) {
			if(field == null || IndexOf(field) < 0) return;
			if(newIndex < 0 || newIndex > Count || newIndex == field.Index) return;
			int deletedIndex = field.Index;
			if(newIndex < deletedIndex)
				deletedIndex++;
			if(field.Index < newIndex)
				newIndex++;
			InternalMove(field, deletedIndex, newIndex);
		}
		void InternalMove(PivotGridFieldBase field, int oldIndex, int newIndex) {
			int oldAreaIndex = field.AreaIndex;
			if(newIndex < 0) newIndex = 0;
			if(newIndex < Count)
				InnerList.Insert(newIndex, field);
			else InnerList.Add(field);
			InnerList.RemoveAt(oldIndex);
		}
		protected void UpdateAreaIndexes(PivotArea area) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(area, true, false);
			bool dataFieldPresent = fields.Contains(Data.DataField);
			fields.Sort(new PivotGridFieldAreaIndexCompare());
			if(dataFieldPresent) {
				Data.OptionsDataField.AreaIndexCore = fields.IndexOf(Data.DataField);				
			}
			fields.Remove(Data.DataField);
			for(int i = 0; i < fields.Count; i++) {
				PivotGridFieldBase field = fields[i];
				field.AreaIndexCore = i;
				field.AreaIndexOldCore = i;
			}
		}
		internal List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField) {
			return GetFieldsByArea(area, includeDataField, true);
		}
		protected List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField, bool sort) {
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++)
				if(this[i].Area == area && this[i].Visible)
					fields.Add(this[i]);
			PivotGridFieldBase dataField = Data.DataField;
			if(includeDataField && dataField.Visible && area == dataField.Area && 0 <= dataField.AreaIndex && dataField.AreaIndex <= fields.Count)
				fields.Insert(dataField.AreaIndex, Data.DataField);
			if(sort) fields.Sort(new PivotGridFieldAreaIndexCompare());
			return fields;
		}
		protected internal PivotGridData Data { get { return data; } }
		[Description("Gets the default width of Field Value boxes.")]
		public int DefaultFieldWidth { get { return Data != null ? Data.DefaultFieldWidth : -1; } }
		[Description("Gets the default height of Field Value boxes.")]
		public int DefaultFieldHeight { get { return Data != null ? Data.DefaultFieldHeight : -1; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnGridDeserialized() {
			for(int i = 0; i < Count; i++) {
				this[i].OnGridDeserialized();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearAndDispose() {
			PivotGridFieldBase[] fields = new PivotGridFieldBase[List.Count];
			List.CopyTo(fields, 0);
			Clear();
			for(int i = 0; i < fields.Length; i++) {
				DisposeField(fields[i]);
			}
		}
		protected virtual void DisposeField(PivotGridFieldBase field) {
			field.Dispose();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(Data != null) Data.OnColumnsClear();
		}
		protected override void OnInsertComplete(int index, object obj) {
			PivotGridFieldBase field = (PivotGridFieldBase)obj;
			field.SetCollection(this);
			base.OnInsertComplete(index, obj);
			if(Data != null && index == Count - 1 && field.AreaIndex == -1) {
				int areaIndex = Data.GetFieldCountByArea(field.Area) - 1;
				field.AreaIndexCore = areaIndex;
				field.AreaIndexOldCore = areaIndex;
			} else {
				UpdateAreaIndexes();
			}
			if(Data != null) Data.OnColumnInsert(obj as PivotGridFieldBase);
			if(Data != null && !Data.IsLockUpdate)
				GroupFieldsByHierarchies();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			UpdateAreaIndexes();
			if(Data != null) Data.OnColumnRemove(obj as PivotGridFieldBase);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void GroupFieldsByHierarchies() {
			if(Data == null || !Data.IsOLAP || Data.IsDeserializing) return;
			Dictionary<string, List<PivotGridFieldBase>> groups = GetFieldGroups();
			foreach(List<PivotGridFieldBase> fieldGroup in groups.Values){
				if(fieldGroup.Count == 1) continue;
				PivotGridGroup group = GetGroup(fieldGroup);
				foreach(PivotGridFieldBase field in fieldGroup)
					if(field.Group != group)
						group.Add(field);
				if(!Data.Groups.Contains(group))
					Data.Groups.Add(group);
			}
			for(int i = Data.Groups.Count - 1; i >= 0; i--) {
				PivotGridGroup group = Data.Groups[i];
				if(!group.IsOLAP) continue;
				group.RemoveInvalidFields();
				if(group.Fields.Count == 0)
					Data.Groups.RemoveAt(i);
			}
		}
		PivotGridGroup GetGroup(List<PivotGridFieldBase> fields) {
			foreach(PivotGridFieldBase field in fields)
				if(field.Group != null) return field.Group;
			return Data.Groups.CreateGroup(fields[0].Hierarchy);
		}
		Dictionary<string, List<PivotGridFieldBase>> GetFieldGroups() {
			Dictionary<string, List<PivotGridFieldBase>> groups = new Dictionary<string, List<PivotGridFieldBase>>();
			for(int i = 0; i < Count; i++) {
				if(!string.IsNullOrEmpty(this[i].Hierarchy)) {
					if(!groups.ContainsKey(this[i].Hierarchy))
						groups.Add(this[i].Hierarchy, new List<PivotGridFieldBase>());
					groups[this[i].Hierarchy].Add(this[i]);
				}
			}
			return groups;
		}
		protected internal virtual string GenerateName(string fieldName) {
			if(!String.IsNullOrEmpty(fieldName)) {
				if(PivotGridFieldBase.IsOLAPFieldName(fieldName)) {
					string[] parts = fieldName.Split('.');
					fieldName = parts[parts.Length - 1];
				}
				char[] buf = new char[fieldName.Length + 1];
				int c = 0;
				for(int i = 0; i < fieldName.Length; i++)
					if((fieldName[i] >= 'A' && fieldName[i] <= 'Z') ||
						(fieldName[i] >= 'a' && fieldName[i] <= 'z') ||
						(fieldName[i] >= '0' && fieldName[i] <= '9')) {
						buf[c] = fieldName[i];
						c++;
					}
				return DefaultNamePrefix + new string(buf, 0, c);
			} else return DefaultNamePrefix;
		}
	}
	[TypeConverter(typeof(UniversalTypeConverterEx))]
	public class PivotGridGroup : IEnumerable {		
		List<PivotGridFieldBase> innerList;
		string caption;
		PivotGridGroupCollection collection;
		string hierarchy;
		public PivotGridGroup() : this(string.Empty) { }
		public PivotGridGroup(string caption) {
			this.collection = null;
			this.caption = caption;
			this.innerList = new List<PivotGridFieldBase>();
		}
		protected internal PivotGridGroupCollection Collection { get { return collection; } set { collection = value; } }
		protected PivotGridData Data { get { return Collection != null ? Collection.Data : null; } }
		protected bool IsLoading { get { return Data != null ? Data.IsLoading : false; } }
		[Description("Provides indexed access to the fields in the group.")]
		public PivotGridFieldBase this[int index] { get { return innerList[index] as PivotGridFieldBase; } }
		[Browsable(false)]
		public int Index { get { return Collection != null ? Collection.IndexOf(this) : -1; } }
		[
		Description("Gets the number of fields within the group."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.Count")
		]
		public int Count { get { return innerList.Count; } }
		public void RemoveAt(int index) {
			if(index < 0 && index >= Count) return;
			RemoveCore(index);
		}
		protected void RemoveCore(int index) {
			if(IsOLAP && IsFieldValid(this[index]) && !this[index].IsDisposed)
				throw new Exception("Cannot remove the field from the hierarchy.");
			object obj = innerList[index];
			innerList.RemoveAt(index);
			OnRemoveComplete(index, obj);
		}
		public void Remove(PivotGridFieldBase field) {
			RemoveAt(IndexOf(field));
		}
		public void Clear() {
			innerList.Clear();
			OnChanged();
		}
		protected void Sort(IComparer<PivotGridFieldBase> comparer) {
			innerList.Sort(comparer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, false, true)]
		public IList Fields { get { return innerList; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateFieldsItem(XtraItemEventArgs e) {
			string name = e.Item.Value.ToString();
			if(name != string.Empty && Data != null) {
				PivotGridFieldBase field = Data.Fields.GetFieldByName(name);
				if(field != null) {
					return field;
				}
			}
			return null;
		}
		[
		Description("Gets or sets the group's caption."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.Caption"),
		DefaultValue(""), XtraSerializableProperty()
		]
		public string Caption {
			get { return GetCaptionCore(); }
			set {
				if(value == null)
					value = string.Empty;
				if(Caption == value) return;
				caption = value;
				FireChanged(null);
			}
		}
		string GetCaptionCore() {
			if(Data != null && Data.IsOLAP && string.IsNullOrEmpty(caption))
				return Data.GetHierarchyCaption(Hierarchy);
			return caption;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotArea Area { get { return Count > 0 ? this[0].AreaCore : PivotArea.FilterArea; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AreaIndex { get { return Count > 0 ? this[0].AreaIndex : -1; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible { get { return Count > 0 ? this[0].VisibleCore : false; } }
		[
		Description("Gets the number of visible fields within the current group."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.VisibleCount")
		]
		public int VisibleCount {
			get {
				for(int i = 0; i < Count; i++) {
					if(!this[i].ExpandedInFieldsGroup)
						return i + 1;
				}
				return Count;
			}
		}
		public bool IsFieldVisible(PivotGridFieldBase field) {
			if(field.Group != this) return field.Visible;
			int fieldIndex = IndexOf(field);
			for(int i = 0; i < fieldIndex; i++) {
				if(!this[i].ExpandedInFieldsGroup)
					return false;
			}
			return Visible;
		}
		public List<PivotGridFieldBase> GetVisibleFields() {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++) {
				if(!this[i].ExpandedInFieldsGroup || !this[i].Visible)
					break;
				res.Add(this[i]);
			}
			return res;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanExpandField(PivotGridFieldBase field) {
			if(field.Group != this) return false;
			return IndexOf(field) < Count - 1;
		}
		public bool CanAdd(PivotGridFieldBase field) {
			return Collection == null || !Collection.Contains(field);
		}
		public void Add(PivotGridFieldBase field) {
			if(IsOLAP && !IsFieldValid(field) && !IsLoading)
				throw new Exception("Cannot move the field to another hierarchy.");
			if(field.Group != null) return;
			innerList.Add(field);
			OnInsertComplete(Count, field);
		}
		public void AddRange(PivotGridFieldBase[] fields) {
			foreach(PivotGridFieldBase field in fields) {
				Add(field);
			}
		}
		public int IndexOf(PivotGridFieldBase field) {
			return innerList.IndexOf(field);
		}
		public bool Contains(PivotGridFieldBase field) {
			return innerList.Contains(field);
		}
		public bool CanChangeArea(PivotGridFieldBase field) {
			return field.Group == this && IndexOf(field) < 1;
		}
		public bool CanChangeAreaTo(PivotArea newArea, int newAreaIndex) {
			if(Count < 2 || Area != newArea) return true;
			return newAreaIndex <= AreaIndex || newAreaIndex >= AreaIndex + VisibleCount;
		}
		public void ChangeFieldIndex(PivotGridFieldBase field, int newIndex) {
			if(newIndex < 0 || newIndex >= Count || field.Group != this || IndexOf(field) == newIndex) return;
			innerList.Remove(field);
			innerList.Insert(newIndex, field);
			OnChanged();
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption)) return Caption;
			if(Count > 0) {
				string st = this[0].ToString();
				for(int i = 1; i < Count; i++) {
					st += " - " + this[i].ToString();
				}
				return st;
			}
			if(Collection != null) {
				return "Group " + Collection.IndexOf(this).ToString();
			}
			return string.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public string Hierarchy { get { return hierarchy; } set { hierarchy = value; } }
		protected bool CollectionContainsField(PivotGridFieldBase field) {
			return Collection != null ? Collection.Contains(field) : Contains(field);
		}		
		protected void OnInsertComplete(int index, object obj) {
			if(IsOLAP && Count > 1 )
				Sort(new ByLevelComarer());
			FireChanged(obj);
			if(Count > 1)
				OnChanged();			
		}
		protected void OnRemoveComplete(int index, object obj) {
			FireChanged(obj);
			if(index > 0 && index < Count)
				OnChanged();
		}
		protected virtual void OnChanged() {
			UpdateAreaIndexes();
			if(Collection != null)
				Collection.GroupsChanged();
		}
		protected internal virtual void UpdateAreaIndexes() {
			if(Data == null) return;
			Data.Fields.UpdateAreaIndexes();
			Data.DoRefresh();
		}
		protected void FireChanged(object obj) {
			if(Data != null)
				Data.FireChanged(obj);
		}
		System.Collections.IEnumerator IEnumerable.GetEnumerator() {
			return (innerList as IEnumerable).GetEnumerator();
		}
		internal void RemoveInvalidFields() {
			if(!IsOLAP) return;
			for(int i = Count - 1; i >= 0; i--)
				if(!IsFieldValid(this[i]))
					RemoveCore(i);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsOLAP { get { return !string.IsNullOrEmpty(Hierarchy); } }
		internal bool IsFieldValid(PivotGridFieldBase field) {
			return field.Hierarchy == Hierarchy;
		}
		class ByLevelComarer : IComparer<PivotGridFieldBase> {
			bool connectException = false;
			public int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
				if(connectException) return 0;
				try {
					return Comparer<int>.Default.Compare(x.Level, y.Level);
				} catch(OLAPConnectionException) {
					connectException = true;
					return 0;
				}
			}
		}
	}
	[ListBindable(false)]
	public class PivotGridGroupCollection : CollectionBase {
		PivotGridData data;
		public PivotGridGroupCollection(PivotGridData data) {
			this.data = data;
		}
		protected internal PivotGridData Data { get { return data; } }
		[Description("Provides indexed access to the groups in the collection.")]
		public PivotGridGroup this[int index] { get { return InnerList[index] as PivotGridGroup; } }
		public PivotGridGroup Add() {
			PivotGridGroup fieldGroup = CreateGroup();
			Add(fieldGroup);
			return fieldGroup;
		}
		public void Add(PivotGridGroup fieldGroup) {
			List.Add(fieldGroup);
		}
		public void Add(string caption) {
			Add(new PivotGridFieldBase[0] { }, caption);
		}
		public void Add(PivotGridFieldBase[] fields) {
			Add(fields, string.Empty);
		}
		public void Add(PivotGridFieldBase[] fields, string caption) {
			PivotGridGroup group = CreateGroup();
			group.AddRange(fields);
			group.Caption = caption;
			Add(group);
		}
		public void Insert(int index, PivotGridGroup fieldGroup) {
			List.Insert(index, fieldGroup);
		}
		public void Remove(PivotGridGroup fieldGroup) {
			List.Remove(fieldGroup);
		}
		public int IndexOf(PivotGridGroup fieldGroup) {
			return InnerList.IndexOf(fieldGroup);
		}
		public bool Contains(PivotGridGroup fieldGroup) {
			return InnerList.Contains(fieldGroup);
		}
		public bool Contains(PivotGridFieldBase field) {
			return GetGroupByField(field) != null;
		}
		public void AddRange(PivotGridGroup[] groups) {
			foreach(PivotGridGroup fieldGroup in groups) {
				Add(fieldGroup);
			}
		}
		public PivotGridGroup GetGroupByField(PivotGridFieldBase field) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Contains(field))
					return this[i];
			}
			return null;
		}
		public bool CanChangeAreaTo(PivotArea newArea, int newAreaIndex) {
			for(int i = 0; i < Count; i++) {
				if(!this[i].CanChangeAreaTo(newArea, newAreaIndex))
					return false;
			}
			return true;
		}
		protected internal void GroupsChanged() {
			FireChanged();
			if(Data != null)
				Data.OnGroupsChanged();
		}
		protected override void OnInsert(int index, object obj) {
			base.OnInsert(index, obj);
			PivotGridGroup fieldGroup = obj as PivotGridGroup;
			fieldGroup.Collection = this;
			if(fieldGroup.Count > 0) {
				for(int i = fieldGroup.Count - 1; i >= 0; i--) {
					if(fieldGroup[i].Group != null)
						fieldGroup.RemoveAt(i);
				}
			}
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			PivotGridGroup fieldGroup = obj as PivotGridGroup;
			if(fieldGroup.Count > 1) {
				fieldGroup.UpdateAreaIndexes();
			}
			GroupsChanged();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			GroupsChanged();
		}
		protected void FireChanged() {
			if(Data != null)
				Data.FireChanged();
		}
		protected virtual PivotGridGroup CreateGroup() {
			return new PivotGridGroup();
		}
		internal PivotGridGroup CreateGroup(string hierarhy) {
			PivotGridGroup group = CreateGroup();
			group.Hierarchy = hierarhy;
			return group;
		}
	}
	public class PivotGridFieldPair {
		PivotGridFieldBase field, dataField;
		public PivotGridFieldPair(PivotGridFieldBase field, PivotGridFieldBase dataField) {
			this.field = field;
			this.dataField = dataField;
		}
		public PivotGridFieldBase Field { get { return field; } }
		public PivotGridFieldBase DataField { get { return dataField; } }
	}
}
