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
using System.IO.Compression;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid.Design;
namespace DevExpress.XtraPivotGrid {
	public class ResFinder {
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	[ListBindable(false)]
	public class PivotGridFieldReadOnlyCollection : CollectionBase {
		public PivotGridFieldReadOnlyCollection() {
		}
		public PivotGridFieldReadOnlyCollection(PivotGridFieldCollectionBase fields) {
			InnerList.AddRange(fields);
		}
		public PivotGridFieldBase this[int index] { get { return InnerList[index] as PivotGridFieldBase; } }
		public void Sort() {
			InnerList.Sort(new PivotGridFieldAreaIndexCompare());
		}
		public int IndexOf(PivotGridFieldBase field) {
			return InnerList.IndexOf(field);
		}
		internal void Add(PivotGridFieldBase field) {
			InnerList.Add(field);
		}
		internal void Insert(int index, PivotGridFieldBase field) {
			if(index < 0 || index >= Count) {
				Add(field);
			}
			else {
				InnerList.Insert(index, field);
			}
		}
	}
	public class PivotGridFieldAreaIndexCompare : IComparer, IComparer<PivotGridFieldBase> {
		public PivotGridFieldAreaIndexCompare() {}
		int IComparer.Compare(object obj1, object obj2) {
			PivotGridFieldBase field1 = (obj1 as PivotGridFieldBase);
			PivotGridFieldBase field2 = (obj2 as PivotGridFieldBase);
			return CompareCore(field1, field2);
		}
		public int Compare(PivotGridFieldBase field1, PivotGridFieldBase field2) {
			return CompareCore(field1, field2);
		}
		int CompareCore(PivotGridFieldBase field1, PivotGridFieldBase field2) {
			int res = Comparer.Default.Compare(field1.Visible ? 0 : 1, field2.Visible ? 0 : 1);
			if(res != 0) return res;
			res = Comparer.Default.Compare((int)field1.Area, (int)field2.Area);
			if(res != 0) return res;
			if(field1.Group != null && field1.Group == field2.Group) {
				return Comparer.Default.Compare(field1.Group.IndexOf(field1), field2.Group.IndexOf(field2));
			}
			if(field1.Group != null) field1 = field1.Group[0];
			if(field2.Group != null) field2 = field2.Group[0];
			res = Comparer.Default.Compare(GetCompareIndexByFieldIndex(field1), GetCompareIndexByFieldIndex(field2));
			if(res != 0) return res;
			res = Comparer.Default.Compare(field1.IsDataField ? 0 : 1, field2.IsDataField ? 0 : 1);
			if(res != 0) return res;
			return Comparer.Default.Compare(GetCompareIndexByFieldOldIndex(field1), GetCompareIndexByFieldOldIndex(field2));
		}
		protected int GetCompareIndexByFieldIndex(PivotGridFieldBase field) {
			return field.AreaIndex < 0 ? int.MaxValue : field.AreaIndex;
		}
		int GetCompareIndexByFieldOldIndex(PivotGridFieldBase field) {
			if(field.AreaIndex == field.AreaIndexOldCore)
				return field.Index;
			return field.AreaIndex > field.AreaIndexOldCore ? int.MaxValue : int.MinValue;
		}
	}	
	public delegate void LoadingPanelVisibleChanged(bool visible);
	public interface IPivotGridDataSource : IDisposable {
		event EventHandler ListSourceChanged;
		IList ListSource { get; set; }
		bool CaseSensitive { get; set; }
		bool SupportsUnboundColumns { get; }
		void RetrieveFields();
		void ReloadData();		
		void DoRefresh(PivotGridFieldReadOnlyCollection sortedFields);
		void BindColumns(PivotGridFieldReadOnlyCollection sortedFields);
		Type GetFieldType(PivotGridFieldBase field, bool raw);
		int CompareValues(object val1, object val2);
		bool ChangeFieldSortOrder(PivotGridFieldBase field);
		object GetListSourceRowValue(int listSourceRow, string fieldName);
		PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount);
		PivotDrillDownDataSource GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows);
		object GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType);
		PivotSummaryValue GetCellSummaryValue(int columnIndex, int rowIndex, int dataIndex);
		int GetVisibleIndexByValues(bool isColumn, object[] values);
		int GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext);
		bool IsObjectCollapsed(bool isColumn, int visibleIndex);
		bool IsObjectCollapsed(bool isColumn, object[] values);
		bool ChangeExpanded(bool isColumn, int visibleIndex, bool expanded);
		void ChangeExpandedAll(bool isColumn, bool expanded);
		void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded);
		void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value);
		object GetFieldValue(bool isColumn, int visibleIndex, int areaIndex);
		object[] GetUniqueFieldValues(PivotGridFieldBase field);
		bool HasNullValues(PivotGridFieldBase field);
		bool GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex);
		int GetCellCount(bool isColumn);
		int GetObjectLevel(bool isColumn, int visibleIndex);
		void SaveCollapsedStateToStream(Stream stream);
		void WebSaveCollapsedStateToStream(Stream stream);
		void SaveDataToStream(Stream stream, bool compressed);
		void LoadCollapsedStateFromStream(Stream stream);
		void WebLoadCollapsedStateFromStream(Stream stream);
		bool IsAreaAllowed(PivotGridFieldBase field, PivotArea area);
		string[] GetFieldList();
		string GetFieldCaption(string fieldName);
		int GetFieldHierarchyLevel(string fieldName);
		PivotKPIType GetKPIType(PivotGridFieldBase field);
		PivotKPIGraphic GetKPIGraphic(PivotGridFieldBase field);
	}
	public class PivotGridData : IDisposable, IPrefilterOwnerBase {
		protected struct PivotDataCoord {
			public int Col, Row, Data;
			public PivotSummaryType Summary;
			public PivotDataCoord(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
				this.Col = columnIndex;
				this.Row = rowIndex;
				this.Data = dataIndex;
				this.Summary = summaryType;
			}
			public override int GetHashCode() {
				return Data ^ (Col << 5 ) ^ (Row << 18);
			}
		}
		protected class PivotDataCoordIterator {
			PivotDataCoord child;
			bool iterateByColumn;
			public PivotDataCoordIterator(PivotDataCoord child, bool iterateByColumn) {
				this.child = child;
				this.iterateByColumn = iterateByColumn;
			}
			public PivotDataCoord Child { get { return child; } }
			public int Coord {
				get { return iterateByColumn ? child.Col : child.Row; }
				set { 
					if(iterateByColumn) child.Col = value;
					else child.Row = value;
				}
			}
		}
		protected const int LayoutIdAppearance = 1, LayoutIdColumns = 2, LayoutIdData = 3, LayoutIdOptionsView = 4;
		public const string WebResourcePath = "DevExpress.XtraPivotGrid.";
		public const string PivotGridImagesResourcePath = WebResourcePath + "Images.";
		public const int DefaultFieldMinWidth = 20, InvalidKPIValue = -2, UnhandledCustomFieldSort = 3;
		IPivotGridDataSource pivotDataSource;
		PivotGridFieldCollectionBase fields;
		PivotGridGroupCollection groups;
		int lockUpdateCount;
		PivotGridFieldReadOnlyCollection[] fieldCollections;
		PivotGridFieldReadOnlyCollection columnFieldLevelCollection;
		PivotGridFieldReadOnlyCollection rowFieldLevelCollection;
		PivotGridOptionsViewBase optionsView;
		PivotGridOptionsCustomization optionsCustomization;
		PivotGridOptionsDataField optionsDataField;
		PivotGridOptionsPrint optionsPrint;
		PivotGridOptionsChartDataSourceBase optionsChartDataSource;
		PivotGridOptionsData optionsData;
		bool disposing;
		bool isSerializing;
		PivotGridFieldBase dataField;
		PivotVisualItemsBase visualItems;
		readonly PrefilterBase prefilter;
		readonly List<IPivotDataSource> dataSourceList;
		readonly Dictionary<PivotDataCoord, object> cachedValues = new Dictionary<PivotDataCoord, object>();
		internal List<IPivotDataSource> DataSourceList { get { return dataSourceList; } }
		public virtual bool IsDesignMode { 
			get { return false; } 
		}
		string olapConnectionString;
		public virtual string OLAPConnectionString { 
			get { return olapConnectionString; }
			set {
				if(olapConnectionString != value) {
					if(value != null) this.ListSource = null;
					olapConnectionString = value;
					RecreateDataSource();
					if(PivotDataSource is PivotGridOLAPDataSource) {
						((PivotGridOLAPDataSource)PivotDataSource).FullConnectionString = olapConnectionString;
						BeginUpdate();
						for(int i = Groups.Count - 1; i >= 0; i--) {
							if(Groups[i].IsOLAP)
								Groups.Remove(Groups[i]);
						}
						if(!DelayFieldsGroupingByHierarchies && ((PivotGridOLAPDataSource)PivotDataSource).PopulateColumns())
							Fields.GroupFieldsByHierarchies();
						CancelUpdate();						
						DoRefresh();
					}
					OnDataSourceChanged();
				}
			}
		}
		public bool IsOLAP { get { return !String.IsNullOrEmpty(OLAPConnectionString); } }
		protected virtual bool DelayFieldsGroupingByHierarchies { get { return false; } }
		public PivotGridData()	{
			this.pivotDataSource = CreatePivotDataSource();
			PivotDataSource.ListSourceChanged += new EventHandler(DataSourceChanged);
			this.lockUpdateCount = 0;
			this.fields = CreateFieldCollection();			
			this.groups = CreateGroupCollection();			
			this.dataField = CreateDataField();
			this.visualItems = CreateVisualItems();
			int areaCount = Enum.GetValues(typeof(PivotArea)).Length;
			this.fieldCollections = new PivotGridFieldReadOnlyCollection[areaCount];
			for(int i = 0; i < fieldCollections.Length; i++)
				this.fieldCollections[i] = new PivotGridFieldReadOnlyCollection();
			this.columnFieldLevelCollection = new PivotGridFieldReadOnlyCollection();
			this.rowFieldLevelCollection = new PivotGridFieldReadOnlyCollection();
			this.disposing = false;
			this.isSerializing = false;
			this.prefilter = CreatePrefilter();
			this.headerImages = null;
			this.valueImages = null;
			this.dataSourceList = new List<IPivotDataSource>();
		}				
		public void Dispose() {
			bool firstDisposing = !this.disposing;
			this.disposing = true;
			Dispose(firstDisposing);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposing) return;
			this.pivotDataSource.ListSourceChanged -= new EventHandler(DataSourceChanged);
			this.pivotDataSource.Dispose();
			this.pivotDataSource = null;
			Groups.Clear();
			Fields.ClearAndDispose();			
		}		
		protected virtual IPivotGridDataSource CreatePivotDataSource() {
			if(string.IsNullOrEmpty(OLAPConnectionString)) 
				return new PivotGridNativeDataSource(this);
			else 
				return new PivotGridOLAPDataSource(this);		
		}
		void RecreateDataSource() {
			if((ListSource != null && !(PivotDataSource is PivotGridOLAPDataSource)) ||
				(OLAPConnectionString != null && PivotDataSource is PivotGridOLAPDataSource) ||
				disposing) return;
			this.pivotDataSource.ListSourceChanged -= new EventHandler(DataSourceChanged);
			this.pivotDataSource.Dispose();
			this.pivotDataSource = CreatePivotDataSource();
			PivotDataSource.ListSourceChanged += new EventHandler(DataSourceChanged);
			CheckFieldsBound();
		}
		public bool Disposing { get { return disposing; } }
		public virtual bool IsDeserializing { get { return isSerializing; } }
		protected void SetIsDeserializing(bool value) { 
			if(IsDeserializing == value) return;
			isSerializing = value;
		}
		public PivotGridFieldBase DataField { get { return dataField; } }
		protected virtual PivotGridFieldBase CreateDataField() {
			return new PivotGridFieldBase(this);
		}
		public PivotVisualItemsBase VisualItems {
			get {
				if(visualItems != null && !Disposing)
				   visualItems.EnsureIsCalculated();
				return visualItems;
			}
		}
		protected virtual PivotVisualItemsBase CreateVisualItems() {
			return new PivotVisualItemsBase(this);
		}
		IList listSource;
		public IList ListSource {
			get { return listSource; } 
			set {
				if(listSource != value) {
					if(value != null) OLAPConnectionString = null;
					listSource = value;
					NotifyPivotDataSourcesChanged();
					RecreateDataSource();
					if(PivotDataSource is PivotGridNativeDataSource) ((PivotGridNativeDataSource)PivotDataSource).ListSource = value;
					if(PivotDataSource.ListSource is PivotFileDataSource) {
						PivotFileDataSource fileDataSource = (PivotFileDataSource)PivotDataSource.ListSource;
						Stream stream = fileDataSource.CreatedFromStream ? fileDataSource.Stream : new FileStream(fileDataSource.FileName, FileMode.Open, FileAccess.Read);
						PivotFileDataSourceHelper.SeekToFields(stream);
						BinaryReader reader = new BinaryReader(stream);
						long endLayoutPosition = reader.ReadInt64();
						int layoutLength = (int)(endLayoutPosition - stream.Position);
						MemoryStream layoutStream = new MemoryStream(layoutLength);
						layoutStream.SetLength(layoutLength);
						stream.Read(layoutStream.GetBuffer(), 0, layoutLength);
						RestoreLayoutCore(new XmlXtraSerializer(), layoutStream, OptionsLayoutBase.FullLayout);
						layoutStream.Dispose();
						stream.Position = endLayoutPosition;
						LoadCollapsedStateFromStream(stream);
						LayoutChanged();
						if(!fileDataSource.CreatedFromStream)
							stream.Dispose();
					}
					OnDataSourceChanged();
				}
			} 
		}
		public PrefilterBase Prefilter {
			get { return prefilter; }
		}
		protected virtual PrefilterBase CreatePrefilter() {
			return new PrefilterBase(this);
		}
		public bool CaseSensitive {
			get { return PivotDataSource.CaseSensitive; }
			set { 
				PivotDataSource.CaseSensitive = value; 
			}
		}
		public PivotGridFieldCollectionBase Fields { get { return fields; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 100)]
		public PivotGridGroupCollection Groups { get { return groups; } }
		public object XtraCreateGroupsItem(XtraItemEventArgs e) {
			return Groups.Add();
		}
		public virtual bool IsLoading { get { return false; } }
		protected virtual PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollectionBase(this);
		}
		protected virtual PivotGridGroupCollection CreateGroupCollection() {
			return new PivotGridGroupCollection(this);
		}
		public virtual bool AllowHideFields { get { return OptionsCustomization.AllowHideFields != AllowHideFieldsType.Never; } }
		#region Access for native data source
		internal int GetCustomSortRowsAccess(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		internal object GetUnboundValueAccess(PivotGridFieldBase field, int listSourceRowIndex) {
			return GetUnboundValue(field, listSourceRowIndex);
		}
		internal void OnCalcCustomSummaryAccess(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			OnCalcCustomSummary(field, customSummaryInfo);
		}
		#endregion
		protected virtual int GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return 3;
		}		
		protected virtual object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex) {
			return null;
		}		
		public int GetFieldCountByArea(PivotArea area) {
			return this.fieldCollections != null ? this.fieldCollections[(int)area].Count : 0;
		}
		public int DataFieldCount { 
			get { return this.fieldCollections != null ? this.fieldCollections[(int)PivotArea.DataArea].Count : 0; } 
		}
		public bool HasNonVariationSummary {
			get {
				for(int i = DataFieldCount - 1; i >= 0; i --) {
					PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
					if(field.SummaryDisplayType != PivotSummaryDisplayType.AbsoluteVariation && 
						field.SummaryDisplayType != PivotSummaryDisplayType.PercentVariation)
						return true;
				}
				return false;
			}
		}
		public virtual string GetPivotFieldValueText(PivotGridFieldBase field, object value) {
			return field.GetValueText(value);
		}
		public int ColumnFieldCount { get { return this.fieldCollections[(int)PivotArea.ColumnArea].Count; } }
		public int RowFieldCount { get { return this.fieldCollections[(int)PivotArea.RowArea].Count; } }
		public List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField) {
			return Fields.GetFieldsByArea(area, includeDataField);
		}
		public PivotGridFieldBase GetFieldByArea(PivotArea area, int index) {
			if(index < 0 || index >= GetFieldCollection(area).Count) return null;
			return GetFieldCollection(area)[index];
		}
		public Type GetFieldTypeByArea(PivotArea area, int index) {
			return GetFieldType(GetFieldByArea(area, index));
		}
		public Type GetFieldType(PivotGridFieldBase field) {
			return GetFieldType(field, true);
		}
		public Type GetFieldType(PivotGridFieldBase field, bool raw) {
			return PivotDataSource.GetFieldType(field, raw);
		}
		public PivotGridFieldBase GetFieldByLevel(bool isColumn, int level) {
			if(level < 0 || level >= GetFieldLevelCollection(isColumn).Count) return null;
			return GetFieldLevelCollection(isColumn)[level];
		}
		public PivotGridFieldBase GetFieldByFieldNameOrUnboundFieldName(string name) {
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].UnboundFieldName == name) return Fields[i];
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].FieldName == name) return Fields[i];
			return null;
		}
		protected PivotGridFieldReadOnlyCollection GetFieldCollection(PivotArea area) {
			return this.fieldCollections[(int)area];
		}
		public bool IsFieldCollectionsEmpty {
			get {
				for(int i = 0; i < this.fieldCollections.Length; i++) {
					if(this.fieldCollections[i].Count > 0)
						return false;
				}
				return true; 
			}
		}
		protected PivotGridFieldReadOnlyCollection GetFieldCollection(bool isColumn) {
			return GetFieldCollection(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea);
		}
		protected PivotGridFieldReadOnlyCollection GetFieldLevelCollection(bool isColumn) {
			return isColumn ? columnFieldLevelCollection : rowFieldLevelCollection;
		}
		protected virtual PivotGridOptionsViewBase CreateOptionsView() { return new PivotGridOptionsViewBase(new EventHandler(OnOptionsViewChanged)); }
		protected virtual PivotGridOptionsCustomization CreateOptionsCustomization() { return new PivotGridOptionsCustomization(new EventHandler(OnOptionsChanged)); }
		protected virtual PivotGridOptionsDataField CreateOptionsDataField() { return new PivotGridOptionsDataField(this); }
		public PivotGridOptionsViewBase OptionsView { 
			get {
				if(optionsView == null) {
					optionsView = CreateOptionsView();
				}
				return optionsView; 
			} 
		}
		protected virtual void OnOptionsViewChanged(object sender, EventArgs e) {
			LayoutChanged();
		}  
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsCustomization OptionsCustomization { 
			get {
				if(optionsCustomization == null) {
					optionsCustomization = CreateOptionsCustomization();
				}
				return optionsCustomization; 
			} 
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotGridOptionsDataField OptionsDataField { 
			get {
				if(optionsDataField == null) {
					optionsDataField = CreateOptionsDataField();
				}
				return optionsDataField; 
			} 
		}		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual PivotGridOptionsPrint OptionsPrint { 
			get { 
				if(optionsPrint == null)
					optionsPrint = CreatePivotGridOptionsPrint();
				return optionsPrint; 
			} 
		}
		protected virtual PivotGridOptionsPrint CreatePivotGridOptionsPrint() {
			return new PivotGridOptionsPrint();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotGridOptionsChartDataSourceBase OptionsChartDataSource { 
			get {
				if(optionsChartDataSource == null)
					optionsChartDataSource = CreateOptionsChartDataSource();
				return optionsChartDataSource; 
			} 
		}
		protected virtual PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridOptionsChartDataSourceBase();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsData OptionsData { 
			get {
				if(optionsData == null)
					optionsData = CreateOptionsData();
				return optionsData; 
			} 
		}
		protected virtual PivotGridOptionsData CreateOptionsData() {
			return new PivotGridOptionsData(this, new EventHandler(OnOptionsDataChanged));
		}
		protected virtual void OnOptionsDataChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected internal IPivotGridDataSource PivotDataSource { get { return pivotDataSource; } }
		protected PivotGridOLAPDataSource OLAPDataSource { get { return (PivotGridOLAPDataSource)PivotDataSource; } }
		protected PivotDataController DataController {
			get {
				if(PivotDataSource is PivotGridNativeDataSource)
					return (PivotDataController)((PivotGridNativeDataSource)PivotDataSource).DataController;
				return null;
			}
		}
		public bool SupportsUnboundColumns { get { return PivotDataSource.SupportsUnboundColumns; } }
		public void CheckBound(PivotGridFieldBase field) {
			if(field.IsUnbound && !SupportsUnboundColumns) throw new Exception("Current data source doesn't support unbound fields");
		}
		void CheckFieldsBound() {
			for(int i = 0; i < Fields.Count; i++)
				CheckBound(Fields[i]);
		}
		public int CompareValues(object val1, object val2) {
			return PivotDataSource.CompareValues(val1, val2);
		}		
		public void BeginUpdate() {
			this.lockUpdateCount ++;
		}
		public void CancelUpdate() {
			this.lockUpdateCount--;
		}
		public virtual void EndUpdate() {
			if(this.lockUpdateCount == 1 && IsOLAP) Fields.GroupFieldsByHierarchies();
			if(--this.lockUpdateCount == 0) {
				DoRefresh();
			}
		}
		public bool IsLockUpdate { get { return this.lockUpdateCount > 0; } }
		public void FireChanged() {
			FireChanged(null);
		}
		public void FireChanged(object obj) {
			if (obj == null) {
				FireChanged(null);
			}
			else {
				FireChanged(new object[] { obj });
			}
		}
		public virtual void FireChanged(object[] objs) {}
		public virtual void RetrieveFields() {
			BeginUpdate();
			try {
				Fields.ClearAndDispose();
				PivotDataSource.RetrieveFields();
			}
			finally {
				EndUpdate();
			}
		}
		public virtual string[] GetFieldList() {
			string[] result = PivotDataSource.GetFieldList();
			if(result != null) Array.Sort(result);
			return result;
		}
		public string GetFieldName(PivotGridFieldBase field) {
			switch(OptionsDataField.FieldNaming) {
				case DataFieldNaming.FieldName:
					if(!string.IsNullOrEmpty(field.FieldName))
						return field.FieldName;
					if(field.IsUnbound && !string.IsNullOrEmpty(field.UnboundFieldName))
						return field.UnboundFieldName;
					break;
				case DataFieldNaming.Name:
					if(!string.IsNullOrEmpty(field.Name))
						return field.Name;
					if(!string.IsNullOrEmpty(field.FieldName))
						return field.FieldName;
					break;
			}
			return string.Empty;
		}
		public virtual void ReloadData() {
			PivotDataSource.ReloadData();
		}
		internal bool CanDoRefresh { get { return !IsLockUpdate && !IsLoading && !Disposing;	} }
		internal protected virtual bool LockRefresh { get { return false; } }
		public void UpdateDataSources() {
			ResetPivotDataSources();
			RefreshPivotDataSources();
		}
		protected void NotifyPivotDataSourcesChanged() {
			for(int i = 0; i < DataSourceList.Count; i++) {
				DataSourceList[i].DataSourceChanged();
			}
		}
		protected void ResetPivotDataSources() {
			List<IPivotDataSource> liveSources = new List<IPivotDataSource>();
			for(int i = 0; i < DataSourceList.Count; i++) {
				IPivotDataSource dataSource = DataSourceList[i];
				if(dataSource.IsLive) liveSources.Add(dataSource);
				dataSource.ResetData();
			}
			DataSourceList.Clear();
			DataSourceList.AddRange(liveSources);
		}
		protected void RefreshPivotDataSources() {
			for(int i = 0; i < DataSourceList.Count; i++) 
				DataSourceList[i].Refresh();
		}
		public virtual void OnSortOrderChanged(PivotGridFieldBase field) {
			if(!CanDoRefresh || !field.IsColumnOrRow || !field.Visible) return;
			if(field.SortMode == PivotSortMode.Custom) {
				DoRefresh();
			} else {
				if(PivotDataSource.ChangeFieldSortOrder(field)) LayoutChanged();
			}
		}
		public virtual void OnSortModeChanged(PivotGridFieldBase field) {
			if(!field.IsColumnOrRow || !field.Visible) return;
			DoRefresh();
		}
		public void DisconnectOLAP() {
			if(IsOLAP)
				((PivotGridOLAPDataSource)PivotDataSource).Disconnect();
		}
		public string SaveOLAPDataSourceState() {
			if(IsOLAP)
				return ((PivotGridOLAPDataSource)PivotDataSource).SaveStateToString();
			return string.Empty;
		}
		public void RestoreOLAPDataSourceState(string savedState) {
			if(IsOLAP)
				((PivotGridOLAPDataSource)PivotDataSource).RestoreStateFromString(savedState);
		}
		public void DoRefresh() {
			if(CanDoRefresh) {
				ResetPivotDataSources();
				DoRefreshCore();
				RefreshPivotDataSources();
			}
		}
		protected virtual void DoRefreshCore() {
			Fields.UpdateAreaIndexes();
			if(PivotDataSource != null) PivotDataSource.DoRefresh(GetSortedFields());
		}
		public PivotGridFieldReadOnlyCollection GetSortedFields() {
			PivotGridFieldReadOnlyCollection sortedFields = new PivotGridFieldReadOnlyCollection(Fields);
			sortedFields.Sort();
			AddFieldsIntoFieldCollections(sortedFields);
			return sortedFields;
		}
		public void EnsureFieldCollections() {
			if(IsFieldCollectionsEmpty)
				GetSortedFields();
		}
		public void BindColumns() {
			if(PivotDataSource != null) PivotDataSource.BindColumns(GetSortedFields());
		}
		void AddFieldsIntoFieldCollections(PivotGridFieldReadOnlyCollection sortedFields) {
			for(int i = 0; i < this.fieldCollections.Length; i++)
				this.fieldCollections[i].Clear();
			this.columnFieldLevelCollection.Clear();
			this.rowFieldLevelCollection.Clear();
			foreach(PivotGridFieldBase field in sortedFields) {
				if(!field.Visible) continue;
				this.fieldCollections[(int)field.Area].Add(field);
				if(field.IsColumnOrRow) {
					GetFieldLevelCollection(field.Area == PivotArea.ColumnArea).Add(field);
				}
			}
			AddDataFieldIntoFieldCollections();
		}
		public void AddDataFieldIntoFieldCollections() {
			if(OptionsDataField.DataFieldsLocationArea != PivotDataArea.None) {
				int index = DataField.Visible ? DataField.AreaIndex : -1;
				GetFieldLevelCollection(OptionsDataField.DataFieldsLocationArea == PivotDataArea.ColumnArea).Insert(index, DataField);
			} else {
				if(GetFieldLevelCollection(true).Count == 0) {
					GetFieldLevelCollection(true).Add(DataField);
				}
			}
		}
		public PivotGridFieldBase GetFieldByPivotColumnInfo(PivotColumnInfo columnInfo) {
			return columnInfo != null ? columnInfo.Tag as PivotGridFieldBase : null;
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			PivotDataSource.SaveCollapsedStateToStream(stream);
		}
		public void WebSaveCollapsedStateToStream(Stream stream) {
			PivotDataSource.WebSaveCollapsedStateToStream(stream);
		}
		public void SaveDataToStream(Stream stream, bool compress) {
			PivotDataSource.SaveDataToStream(stream, compress);			
		}
		public virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			serializer.SerializeObject(this, path, "PivotGrid", options);
		}
		public virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			serializer.DeserializeObject(this, path, "PivotGrid", options);
		}		
		public void SavePivotGridToFile(string path, bool compress) {
			using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
				SaveDataToStream(stream, compress);
				SaveFieldsToStream(stream);
				SaveCollapsedStateToStream(stream);
			}
		}
		public void SavePivotGridToStream(Stream stream, bool compress) {
			SaveDataToStream(stream, compress);
			SaveFieldsToStream(stream);
			SaveCollapsedStateToStream(stream);
		}
		private void SaveFieldsToStream(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			long startPosition = stream.Position;
			writer.Write(0L);
			SaveLayoutCore(new XmlXtraSerializer(), stream, OptionsLayoutBase.FullLayout);
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			stream.Position = endPosition;
		}
		public void SaveFilterValuesToStream(Stream stream) {
			if(ListSource == null && String.IsNullOrEmpty(OLAPConnectionString)) return;
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			for(int i = 0; i < Fields.Count; i ++) {
				Fields[i].FilterValues.SaveToStream(writer, GetFieldType(Fields[i]));
			}
		}
		public void LoadFilterValuesFromStream(Stream stream) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			for(int i = 0; i < Fields.Count; i ++) {
				Fields[i].FilterValues.LoadFromStream(reader);
			}
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			PivotDataSource.LoadCollapsedStateFromStream(stream);
		}
		public void WebLoadCollapsedStateFromStream(Stream stream) {
			PivotDataSource.WebLoadCollapsedStateFromStream(stream);
		}
		public void LayoutChanged() {
			if(!IsLoading && !IsLockUpdate) {
				LayoutChangedCore();
				UpdateDataSources();
			}
		}
		protected virtual void LayoutChangedCore() {
			ClearCaches();
		}
		void ClearCaches() {
			cachedValues.Clear();
			visualItems.Clear();
			isDataFieldsVisible = null;
		}
		public void LoadVisualItemsState(string fieldValueItems, string dataCells) {
			visualItems.LoadFieldValueItemsState(fieldValueItems);
			visualItems.LoadDataCellsState(dataCells);
		}
		public virtual object GetCustomGroupInterval(PivotGridFieldBase field, object value) {
			return value;
		}
		public object GetListSourceRowValue(int listSourceRow, string fieldName) {
			return PivotDataSource.GetListSourceRowValue(listSourceRow, fieldName);
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex) {
			PivotDrillDownDataSource ds = PivotDataSource.GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, OptionsData.DrillDownMaxRowCount);
			if(ds != null) DataSourceList.Add(ds);
			return ds;
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			if(maxRowCount < 0)
				maxRowCount = OptionsData.DrillDownMaxRowCount;
			PivotDrillDownDataSource ds = PivotDataSource.GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount);
			if(ds != null) DataSourceList.Add(ds);
			return ds;
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			PivotDrillDownDataSource ds = PivotDataSource.GetDrillDownDataSource(groupRow, visibleListSourceRows);
			if(ds != null) DataSourceList.Add(ds);
			return ds;
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex) {
			PivotSummaryDataSource ds = new PivotSummaryDataSource(this, columnIndex, rowIndex);
			if(ds != null) DataSourceList.Add(ds);
			return ds;
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return CreateSummaryDataSource(-1, -1);
		}		
		public Rectangle GetCellChildrenBounds(int columnIndex, int rowIndex) {
			if(columnIndex < -1 || columnIndex >= ColumnCellCount ||
				rowIndex < -1 || rowIndex >= RowCellCount ||
				(ColumnCellCount == 0 && RowCellCount == 0)) return Rectangle.Empty;
			Rectangle result = new Rectangle();
			if(columnIndex == -1) {
				result.X = ColumnCellCount == 0 ? -1 : 0;
				result.Width = ColumnCellCount == 0 ? 1 : ColumnCellCount;
			} else {
				int nextIndex = PivotDataSource.GetNextOrPrevVisibleIndex(true, columnIndex, true);
				if(nextIndex < 0) nextIndex = ColumnCellCount;
				result.X = columnIndex;
				result.Width = nextIndex - columnIndex;
			}
			if(rowIndex == -1) {
				result.Y = RowCellCount == 0 ? -1 : 0;
				result.Height = RowCellCount == 0 ? 1 : RowCellCount;
			} else {
				int nextIndex = PivotDataSource.GetNextOrPrevVisibleIndex(false, rowIndex, true);
				if(nextIndex < 0) nextIndex = RowCellCount;
				result.Y = rowIndex;
				result.Height = nextIndex - rowIndex;
			}
			return result;
		}
		public List<Point> GetCellChildren(int columnIndex, int rowIndex) {
			int maxColumnLevel = GetMaxObjectLevel(true),
				maxRowLevel = GetMaxObjectLevel(false);
			List<Point> result = new List<Point>();
			Rectangle bounds = GetCellChildrenBounds(columnIndex, rowIndex);
			for(int y = bounds.Top; y < bounds.Bottom; y++)
				for(int x = bounds.Left; x < bounds.Right; x++)
					if(IsCellFit(x, y, maxColumnLevel, maxRowLevel)) {
						result.Add(new Point(x, y));
					}
			return result;
		}
		protected bool IsCellFit(int x, int y, int maxColumnLevel, int maxRowLevel) {
			return IsIndexFit(true, x, maxColumnLevel) && IsIndexFit(false, y, maxRowLevel) 
					&& !IsSummaryEmpty(x, y);
		}
		bool IsIndexFit(bool isColumn, int index, int maxLevel) {
			int level = GetObjectLevel(isColumn, index);
			return level == maxLevel || GetObjectLevel(isColumn, index + 1) == level ||
				index == GetCellCount(isColumn) - 1;
		}
		bool IsSummaryEmpty(int columnIndex, int rowIndex) {
			for(int i = 0; i < GetFieldCountByArea(PivotArea.DataArea); i++)
				if(GetCellValue(columnIndex, rowIndex, i) != null) return false;
			return true;
		}
		bool IsDataField(PivotGridFieldBase field) {
			return field != null && field.Area == PivotArea.DataArea && field.Visible;
		}
		public PivotSummaryValue GetCellSummaryValue(int columnIndex, int rowIndex, PivotGridFieldBase field) {
			if(field == null || field.Area != PivotArea.DataArea || !field.Visible) return null;
			return PivotDataSource.GetCellSummaryValue(columnIndex, rowIndex, field.AreaIndex);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridFieldBase field) {
			if(!IsDataField(field)) return null;
			int columnIndex = PivotDataSource.GetVisibleIndexByValues(true, columnValues);
			if(columnIndex < 0 && columnValues != null && columnValues.Length > 0) return null;
			int rowIndex = PivotDataSource.GetVisibleIndexByValues(false, rowValues);
			if(rowIndex < 0 && rowValues != null && rowValues.Length > 0) return null;
			return GetCellValue(columnIndex, rowIndex, field.AreaIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field) {
			if(!IsDataField(field)) return null;
			return GetCellValue(columnIndex, rowIndex, field.AreaIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex, int dataIndex) {
			PivotGridFieldBase dataField = GetFieldByArea(PivotArea.DataArea, dataIndex);
			if(dataField == null) return null;
			return GetCellValue(columnIndex, rowIndex, dataIndex, dataField.SummaryType);
		}
		public object GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			PivotDataCoord coord = new PivotDataCoord(columnIndex, rowIndex, dataIndex, summaryType);
			object value;
			if(!cachedValues.TryGetValue(coord, out value)) {
				value = GetCellValue(coord);
				cachedValues.Add(coord, value);
			}
			return value;
		}
		object GetCellValue(PivotDataCoord coord) {
			PivotGridFieldBase dataField = GetFieldByArea(PivotArea.DataArea, coord.Data);
			if(dataField == null) return null;
			bool byColumn = OptionsDataField.Area != PivotDataArea.RowArea;
			object result = GetCellValueCore(coord);
			PivotGridFieldBase columnField = GetFieldByLevel(true, GetObjectLevel(true, coord.Col)),
				rowField = GetFieldByLevel(false, GetObjectLevel(false, coord.Row));
			bool isRunningColumn = IsRunning(columnField),
				isRunningRow = IsRunning(rowField);
			if(isRunningColumn || isRunningRow)
				result = GetRunningCellValue(coord, dataField, result, isRunningColumn, isRunningRow);
			switch(dataField.SummaryDisplayType) {
				case PivotSummaryDisplayType.AbsoluteVariation:
				case PivotSummaryDisplayType.PercentVariation:
					return GetCellValueVariation(coord, dataField, byColumn, result);
				case PivotSummaryDisplayType.PercentOfColumn:
				case PivotSummaryDisplayType.PercentOfRow:
					return GetCellValuePercentOf(coord, dataField, result);
				default:
					return result;
			}
		}
		object GetCellValueCore(PivotDataCoord coord) {
			object result = PivotDataSource.GetCellValue(coord.Col, coord.Row, coord.Data, coord.Summary);
			PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, coord.Data);
			if(coord.Summary == PivotSummaryType.Custom && result != null)
				result = ((PivotGridCustomValues)result)[field];
			return result;
		}
		object GetCellValuePercentOf(PivotDataCoord coord, PivotGridFieldBase field, object result) {
			PivotDataCoord totalCoord = coord;
			if(field.SummaryDisplayType == PivotSummaryDisplayType.PercentOfColumn)
				totalCoord.Row = GetTotalIndex(false, coord.Row);
			if(field.SummaryDisplayType == PivotSummaryDisplayType.PercentOfRow)
				totalCoord.Col = GetTotalIndex(true, coord.Col);
			if(totalCoord.Col == coord.Col && totalCoord.Row == coord.Row) return 1m;
			try {
				decimal value = Convert.ToDecimal(result),
					totalValue = Convert.ToDecimal(GetCellValueCore(totalCoord));
				if(totalValue == 0) return 0m;
#if DEBUGTEST
				NUnit.Framework.Assert.IsTrue(value / totalValue <= 1m);
#endif
				return value / totalValue;
			} catch {
				return PivotSummaryValue.ErrorValue;
			}
		}
		decimal GetRunningCellValue(PivotDataCoord coord, PivotGridFieldBase field, object result, bool runColumn, bool runRow) {
			return Convert.ToDecimal(result) +
				(runColumn ? GetRunningCellValueCore(coord, true) : 0m) +
				(runRow ? GetRunningCellValueCore(coord, false) : 0m);
		}
		decimal GetRunningCellValueCore(PivotDataCoord coord, bool isColumn) {
			PivotDataCoordIterator iterator = new PivotDataCoordIterator(coord, isColumn);
			int startIndex = iterator.Coord,
				startLevel = GetObjectLevel(isColumn, iterator.Coord);
			bool hasChilds = true;
			decimal res = 0;
			for(iterator.Coord--; iterator.Coord >= 0; iterator.Coord--) {
				int curLevel = GetObjectLevel(isColumn, iterator.Coord);
				if(curLevel == startLevel) {
					hasChilds = true;
					res += Convert.ToDecimal(GetCellValueCore(iterator.Child));
				} else {
					if(curLevel == startLevel - 1) {
						if(!hasChilds)
							res += Convert.ToDecimal(GetCellValueCore(iterator.Child));
						hasChilds = false;
					}
				}
			}
			return res;
		}
		bool IsRunning(PivotGridFieldBase field) {
			return field != null && field.RunningTotal;
		}
		object GetCellValueVariation(PivotDataCoord coord, PivotGridFieldBase field, bool byColumn, object value) {
			int prevIndex = GetPrevIndex(byColumn, byColumn ? coord.Col : coord.Row, !OptionsData.AllowCrossGroupVariation);
			if(prevIndex == -1) return null;
			PivotDataCoord prevCoord = new PivotDataCoord(byColumn ? prevIndex : coord.Col, byColumn ? coord.Row : prevIndex, coord.Data, coord.Summary);
			object prevValue = GetCellValueCore(prevCoord);
			if(value == null && prevValue == null) return null;
			return GetCellValueVariationCore(field, value, prevValue);
		}
		decimal GetCellValueVariationCore(PivotGridFieldBase field, object value, object prevValue) {
			decimal cur_val = Convert.ToDecimal(value);
			decimal prev_val = Convert.ToDecimal(prevValue);
			if(prev_val == 0m && field.SummaryDisplayType == PivotSummaryDisplayType.PercentVariation) return 1m;
			if(field.SummaryDisplayType == PivotSummaryDisplayType.AbsoluteVariation) return cur_val - prev_val;
			else return (cur_val - prev_val) / prev_val;
		}
		int GetParentIndex(bool byColumn, int visibleIndex) {
			int level = GetObjectLevel(byColumn, visibleIndex) - 1;
			if(level >= 0) {
				for(int i = visibleIndex - 1; i >= 0; i--) {
					if(GetObjectLevel(byColumn, i) == level)
						return i;
				}
			}
			return -1;
		}
		int GetPrevIndex(bool byColumn, int currentIndex) {
			return GetPrevIndex(byColumn, currentIndex, false);
		}
		internal int GetPrevIndex(bool byColumn, int currentIndex, bool stopOnParent) {
			int i = currentIndex - 1, curLevel = GetObjectLevel(byColumn, currentIndex);
			if(stopOnParent) {
				while(GetObjectLevel(byColumn, i) > curLevel && i >= 0) i--;
			} else {
				while(GetObjectLevel(byColumn, i) != curLevel && i >= 0) i--;
			}
			if(GetObjectLevel(byColumn, i) != curLevel) return -1;
			return i;
		}
		int GetTotalIndex(bool byColumn, int currentIndex) {
			if(currentIndex == -1) return -1;
			int totalIndex = currentIndex,
				curLevel = GetObjectLevel(byColumn, currentIndex);
			if(curLevel == 0) totalIndex = -1;
			else while(GetObjectLevel(byColumn, totalIndex) >= curLevel) totalIndex--;
			return totalIndex;
		}
		int GetFieldIndex(PivotGridFieldBase field, int columnRowIndex) {
			if(!field.IsColumnOrRow) throw new Exception("This method can be called for the Column and Row fields only");
			int level = field.AreaIndex;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			for(int i = columnRowIndex; i >= 0; i--) {
				int curLevel = GetObjectLevel(isColumn, i);
				if(curLevel == level) return i;
				if(curLevel < level) return -1;
			}
			return -1;
		}
		public object GetNextOrPrevRowCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field, bool isNext) {
			PivotDataCoord coord = new PivotDataCoord(columnIndex, rowIndex, field.AreaIndex, field.SummaryType);
			return GetNextOrPrevCellValue(coord, isNext, false);
		}
		public object GetNextOrPrevColumnCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field, bool isNext) {
			PivotDataCoord coord = new PivotDataCoord(columnIndex, rowIndex, field.AreaIndex, field.SummaryType);
			return GetNextOrPrevCellValue(coord, isNext, true);
		}
		object GetNextOrPrevCellValue(PivotDataCoord coord, bool isNext, bool isColumnIndex) {			
			if(isColumnIndex)
				coord.Col = PivotDataSource.GetNextOrPrevVisibleIndex(true, coord.Col, isNext);
			else
				coord.Row = PivotDataSource.GetNextOrPrevVisibleIndex(false, coord.Row, isNext);
			if((isColumnIndex && coord.Col < 0) || (!isColumnIndex && coord.Row < 0)) return null;
			return GetCellValue(coord);
		}
		public int ColumnCellCount { get { return GetCellCount(true); } }
		public int RowCellCount { get { return GetCellCount(false); } }
		public int GetCellCount(bool isColumn) { return PivotDataSource.GetCellCount(isColumn); }
		public int GetLevelCount(bool isColumn) { return GetFieldCountByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea); }
		public int GetColumnLevel(int columnIndex) {
			return GetObjectLevel(true, columnIndex);
		}
		public int GetRowLevel(int rowIndex) {
			return GetObjectLevel(false, rowIndex);
		}
		public int GetObjectLevel(bool byColumn, int visibleIndex) {
			return PivotDataSource.GetObjectLevel(byColumn, visibleIndex);
		}
		public int GetMaxObjectLevel(bool isColumn) {
			int count = GetCellCount(isColumn),
				maxLevel = -1;
			for(int i = 0; i < count; i++)
				maxLevel = Math.Max(GetObjectLevel(isColumn, i), maxLevel);
			return maxLevel;
		}
		public int GetFieldWidth(PivotArea area, int fieldIndex) {
			PivotGridFieldBase field = GetFieldByArea(area, fieldIndex);
			return field != null ? field.Width : DefaultFieldWidth;
		}
		public bool IsColumnCollapsed(int columnIndex) {
			return IsObjectCollapsed(true, columnIndex);
		}
		public bool IsRowCollapsed(int rowIndex) {
			return IsObjectCollapsed(false, rowIndex);
		}
		public bool IsObjectCollapsed(bool isColumn, int index) {
			return PivotDataSource.IsObjectCollapsed(isColumn, index);
		}
		public bool IsObjectCollapsed(bool isColumn, object[] values) {
			return PivotDataSource.IsObjectCollapsed(isColumn, values);
		}
		public bool ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {  
			bool result = PivotDataSource.ChangeExpanded(isColumn, visibleIndex, expanded);
			OnGroupRowCollapsed();
			return result;
		}
		public bool ChangeExpanded(bool isColumn, object[] values) {
			int visibleIndex = PivotDataSource.GetVisibleIndexByValues(isColumn, values);
			if(visibleIndex < 0) return false;
			bool collapsed = IsObjectCollapsed(isColumn, visibleIndex);
			return ChangeExpanded(isColumn, visibleIndex, collapsed);
		}
		public virtual void ChangeExpandedAll(bool expanded) {
			ChangeExpandedAllCore(true, expanded);
			ChangeExpandedAllCore(false, expanded);
			OnGroupRowCollapsed();
		}
		public void ChangeExpandedAll(bool isColumn, bool expanded) {
			ChangeExpandedAllCore(isColumn, expanded);
			OnGroupRowCollapsed();
		}
		protected void ChangeExpandedAllCore(bool isColumn, bool expanded) {
			PivotDataSource.ChangeExpandedAll(isColumn, expanded);
		}		
		public virtual void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(CanNotChangeExpanded(field)) return;
			PivotDataSource.ChangeFieldExpanded(field, expanded);
			OnGroupRowCollapsed();
		}
		public virtual void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(CanNotChangeExpanded(field)) return;
			PivotDataSource.ChangeFieldExpanded(field, expanded, value);
			OnGroupRowCollapsed();
		}
		bool CanNotChangeExpanded(PivotGridFieldBase field) {
			return field == null || (!IsOLAP && field.ColumnHandle < 0) || !field.IsColumnOrRow || (field.AreaIndex >= GetLevelCount(field.IsColumn) - 1);
		}
		public bool IsFieldValueExpanded(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow) return false;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			int columnRowIndex = GetFieldIndex(field, isColumn ? columnIndex : rowIndex);
			return !IsObjectCollapsed(isColumn, columnRowIndex);			
		}
		public bool IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			return PivotDataSource.IsAreaAllowed(field, area);
		}
		public PivotKPIType GetKPIType(PivotGridFieldBase field) {
			return PivotDataSource.GetKPIType(field);
		}
		public PivotKPIGraphic GetKPIGraphic(PivotGridFieldBase field) {
			if(field.KPIGraphic == PivotKPIGraphic.ServerDefined)
				return PivotDataSource.GetKPIGraphic(field);
			return field.KPIGraphic;
		}
		public bool IsValidKPIState(int state) {
			return state == 0 || state == -1 || state == 1;
		}
		public string GetKPITooltipText(PivotKPIType type, int value) {
			switch(type) {
				case PivotKPIType.Status:
					return GetStatusTooltipText(value);
				case PivotKPIType.Trend:
					return GetTrendTooltipText(value);
			}
			return string.Empty;
		}
		public List<string> GetOLAPKPIList() {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			if(!IsOLAP) return PivotKPIGraphic.None;
			return OLAPDataSource.GetKPIServerDefinedGraphic(kpiName, kpiType);
		}
		public IOLAPMember GetOLAPMember(PivotGridFieldBase field, int visibleIndex) {
			if(!IsOLAP || !field.IsColumnOrRow) return null;
			return OLAPDataSource.GetFieldValues(field.IsColumn)[visibleIndex].Member;
		}
		public IOLAPMember GetOLAPMember(bool isColumn, int visibleIndex) {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetFieldValues(isColumn)[visibleIndex].Member;
		}
		public IOLAPMember[] GetOLAPColumnMembers(string fieldName) {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetUniqueMembers(fieldName);
		}
		public string GetOLAPDrillDownColumnName(string fieldName) {
			if(!IsOLAP) return null;
			return OLAPDataSource.GetDrillDownColumnName(fieldName);
		}
		public PivotDrillDownDataSource GetOLAPDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex,
			int maxRowCount, List<string> customColumns) {
			if(!IsOLAP) return null;
			if(maxRowCount < 0)
				maxRowCount = OptionsData.DrillDownMaxRowCount;
			PivotDrillDownDataSource ds = OLAPDataSource.GetDrillDownDataSource(columnIndex, rowIndex, 
				dataIndex, maxRowCount, customColumns);			
			if(ds != null) DataSourceList.Add(ds);
			return ds;
		}
		protected string GetStatusTooltipText(int state) {
			switch(state) {
				case -1: return PivotGridLocalizer.GetString(PivotGridStringId.StatusBad);
				case 0: return PivotGridLocalizer.GetString(PivotGridStringId.StatusNeutral);
				case 1: return PivotGridLocalizer.GetString(PivotGridStringId.StatusGood);
			}
			throw new ArgumentException("Invalid state");
		}
		protected string GetTrendTooltipText(int state) {
			switch(state) {
				case -1: return PivotGridLocalizer.GetString(PivotGridStringId.TrendGoingDown);
				case 0: return PivotGridLocalizer.GetString(PivotGridStringId.TrendNoChange);
				case 1: return PivotGridLocalizer.GetString(PivotGridStringId.TrendGoingUp);
			}
			throw new ArgumentException("Invalid state");
		}
		public object GetColumnValue(int columnIndex) {
			return GetObjectValue(true, columnIndex);
		}
		public object GetRowValue(int rowIndex) {
			return GetObjectValue(false, rowIndex);
		}
		public object GetObjectValue(bool isColumn, int index) {
			return PivotDataSource.GetFieldValue(isColumn, index, GetObjectLevel(isColumn, index));
		}
		public bool GetIsOthersValue(bool isColumn, int visibleIndex) {
			int levelIndex = GetObjectLevel(isColumn, visibleIndex);
			return PivotDataSource.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		public bool GetIsOthersValue(bool isColumn, int visibleIndex, int levelIndex) {
			return PivotDataSource.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		public bool GetIsOthersValue(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow) return false;
			return field.Area == PivotArea.ColumnArea ? GetIsOthersValue(true, columnIndex, field.AreaIndex) : GetIsOthersValue(false, rowIndex, field.AreaIndex);
		}
		Nullable<bool> isDataFieldsVisible;
		public bool GetIsDataFieldsVisible(bool isCoumn) {
			if(!GetIsDataLocatedInThisArea(isCoumn) || DataFieldCount <= 1) return false;
			if(!isDataFieldsVisible.HasValue)
				isDataFieldsVisible = GetIsDataFieldsVisibleCore();
			return isDataFieldsVisible.Value;
		}
		protected bool GetIsDataFieldsVisibleCore() {
			int foundFields = 0;
			for(int i = 0; i < GetFieldCountByArea(PivotArea.DataArea); i++) {
				PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
				foreach(PivotGridValueType valueType in Enum.GetValues(typeof(PivotGridValueType))) {
					if(!field.CanShowValueType(valueType)) continue;
					bool alreadyFound = ((foundFields >> (int)valueType) & 1) == 1;
					if(alreadyFound) 
						return true;
					else
						foundFields |= 1 << (int)valueType;
				}
			}
			return false;
		}
		public bool GetIsDataLocatedInThisArea(bool isColumn) { 
			return (isColumn && OptionsDataField.DataFieldArea == PivotArea.ColumnArea) ||
				(!isColumn && OptionsDataField.DataFieldArea == PivotArea.RowArea); 
		}
		public object GetFieldValue(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow) return null;
			return PivotDataSource.GetFieldValue(field.Area == PivotArea.ColumnArea, field.Area == PivotArea.ColumnArea ? columnIndex : rowIndex, field.AreaIndex);
		}
		public string GetHierarchyCaption(string dimensionName) {
			if(IsOLAP && !string.IsNullOrEmpty(dimensionName))
				return ((PivotGridOLAPDataSource)PivotDataSource).GetHierarchyCaption(dimensionName);
			return null;
		}
		public virtual object[] GetUniqueFieldValues(PivotGridFieldBase field) {
			if(field == null) return new object[0];
			return PivotDataSource.GetUniqueFieldValues(field);
		}
		public int GetFieldHierarchyLevel(PivotGridFieldBase field) {
			return PivotDataSource.GetFieldHierarchyLevel(field.FieldName);
		}
		public bool HasNullValues(PivotGridFieldBase field) {
			if(field == null || (!IsOLAP && field.ColumnHandle < 0)) return false;
			return PivotDataSource.HasNullValues(field);
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(bool isColumn, int visibleIndex) {
			List<PivotGridFieldSortCondition> res = new List<PivotGridFieldSortCondition>();
			if(visibleIndex < 0 || visibleIndex >= GetCellCount(isColumn))
				return res;
			res.Add(GetFieldSortCondition(isColumn, visibleIndex));
			int parentIndex = GetParentIndex(isColumn, visibleIndex);
			while(parentIndex >= 0) {
				res.Add(GetFieldSortCondition(isColumn, parentIndex));
				parentIndex = GetParentIndex(isColumn, parentIndex);
			}
			return res;
		}
		protected PivotGridFieldSortCondition GetFieldSortCondition(bool isColumn, int visibleIndex) {
			int level = GetObjectLevel(isColumn, visibleIndex);
			PivotGridFieldBase field = GetFieldByLevel(isColumn, level);
			object value = IsOLAP ? null : GetObjectValue(isColumn, visibleIndex);
			string uniqueName = IsOLAP ? GetOLAPMember(isColumn, visibleIndex).UniqueName : null;
			return new PivotGridFieldSortCondition(field, value, uniqueName);
		}
		public bool IsFieldSortedBySummary(PivotGridFieldBase field, PivotGridFieldBase dataField, List<PivotGridFieldSortCondition> itemConditions) {
			if(!CheckDataField(field.SortBySummaryInfo.Field, field.SortBySummaryInfo.FieldName, dataField))
				return false;
			if(itemConditions.Count != field.SortBySummaryInfo.Conditions.Count)
				return false;
			for(int i = 0; i < itemConditions.Count; i++) {
				PivotGridFieldSortCondition condition = field.SortBySummaryInfo.Conditions[itemConditions[i].Field];
				if(condition == null || (!IsOLAP && CompareValues(condition.Value, itemConditions[i].Value) != 0) ||
						(IsOLAP && condition.OLAPUniqueMemberName != itemConditions[i].OLAPUniqueMemberName))
					return false;
			}
			return true;
		}
		bool CheckDataField(PivotGridFieldBase actual, string actualFieldName, PivotGridFieldBase expected) {
			return (expected != null && actual == expected) || 
				(expected != null && actual == null && actualFieldName == expected.FieldName) ||
				(expected == null && actual != null && actual.Visible && actual.Area == PivotArea.DataArea);
		}
		public virtual int DefaultFieldWidth { get { return 100; } }
		public virtual int DefaultFieldHeight { get { return 20; } }
		public virtual void OnFieldAreaChanged(PivotGridFieldBase field) { }
		public virtual void OnFieldVisibleChanged(PivotGridFieldBase field) { }
		public virtual void OnFieldFilteringChanged(PivotGridFieldBase field) { 	}
		public virtual void OnFieldWidthChanged(PivotGridFieldBase field) {
			LayoutChanged();
		}
		public virtual void OnFieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) { }
		public virtual void OnFieldAreaIndexChanged(PivotGridFieldBase field, bool doRefresh) {
			if(!field.Visible || !doRefresh) return;
			if(field.Area != PivotArea.FilterArea)
				DoRefresh();
			else LayoutChanged();
		}
		public virtual void OnGroupsChanged() {}
		protected virtual void OnGroupRowCollapsed() {
			LayoutChanged();
		}
		public virtual void OnColumnsClear() {
			DoRefresh();
		}
		public virtual void OnColumnRemove(PivotGridFieldBase field) {
			DoRefresh();
		}
		public virtual void OnColumnInsert(PivotGridFieldBase field) {
			if(field.IsUnbound || field.IsComplex)
				ReloadData();
			else 
				DoRefresh();
		}
		protected virtual void OnDataSourceChanged() {}
		internal protected virtual void OLAPQueryTimeout() {}
		protected virtual void OnCalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
		}
		public virtual void OnDeserializationComplete() {
			FixFieldsOrder();
			Fields.OnGridDeserialized();
			FixAreaIndexes();
		}		
		void FixFieldsOrder() {
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].IndexInternal < 0 || Fields[i].IndexInternal >= Fields.Count) return;
			BeginUpdate();
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>(Fields.Count);
			foreach(PivotGridFieldBase field in Fields)
				fields.Add(field);
			fields.Sort(new FieldsInternalIndexComparer());
			Fields.Clear();
			for(int i = 0; i < fields.Count; i++)
				Fields.Add(fields[i]);
			CancelUpdate();
		}
		void FixAreaIndexes() {
			int dataFieldAreaIndex = OptionsDataField.AreaIndex;
			Fields.UpdateAreaIndexes();
			if(dataFieldAreaIndex != -1)
				OptionsDataField.AreaIndex = dataFieldAreaIndex;
		}
		class FieldsInternalIndexComparer : Comparer<PivotGridFieldBase> {
			public override int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
				if(x.IsNew && !y.IsNew)
					return 1;
				if(!x.IsNew && y.IsNew)
					return -1;
				return Comparer.Default.Compare(x.IndexInternal, y.IndexInternal);
			}
		}
		void DataSourceChanged(object sender, EventArgs e) {
			OnDataSourceChanged();
		}
		protected void OnOptionsChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		object headerImages, valueImages;		
		public object HeaderImages {
			get { return headerImages; }
			set {
				if(HeaderImages == value) return;
				headerImages = value;
				LayoutChanged();
			}
		}
		public object ValueImages {
			get { return valueImages; }
			set {
				if(ValueImages == value) return;
				valueImages = value;
				LayoutChanged();
			}
		}
		public object GetDesignOLAPDataSourceObject() {
			if(!IsOLAP) 
				throw new Exception("This method can't be used in non OLAP mode");
			return new OLAPDataSourceObject(OLAPDataSource.CubeName, GetFieldList());
		}
		#region events
		public virtual int GetPivotFieldImageIndex(object fieldCellViewInfo) {
			return -1;
		}
		public virtual void OnPopupShowMenu(object e) { }
		public virtual void OnPopupMenuItemClick(object e) { }
		#endregion
		#region FieldsCustomization
		public virtual void ChangeFieldsCustomizationVisible() { }
		public virtual bool IsFieldCustomizationShowing { get { return false; } }
		#endregion
		#region IPrefilterOwnerBase Members
		void IPrefilterOwnerBase.CriteriaChanged() {
			PrefilterCriteriaChanged();
		}
		protected virtual void PrefilterCriteriaChanged() {
			DoRefresh();
		}
		#endregion			
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridPageSettings {
		PaperKind paperKind;
		int paperWidth, paperHeight;
		bool landscape;
		Margins margins;
		static Margins DefaultMargins = new Margins(100, 100, 100, 100);
		[Description("Gets or sets the type of paper for the document. ")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperKind")]
		[DefaultValue(PaperKind.Letter), XtraSerializableProperty(), NotifyParentProperty(true)]
		[TypeConverter(typeof(PaperKindConverter))]
		public PaperKind PaperKind {
			get { return paperKind; }
			set {
				if(paperKind == value) return;
				paperKind = value;
			}
		}
		[Description("Gets or sets a custom width of the paper, in hundredths of an inch. The property is in effect when the PivotGridPageSettings.PaperKind property is set to Custom.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperWidth")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int PaperWidth {
			get { return paperWidth; }
			set {
				if(paperWidth == value) return;
				paperWidth = value;
			}
		}
		[Description("Gets or sets a custom height of the paper, in hundredths of an inch. The property is in effect when the PivotGridPageSettings.PaperKind property is set to Custom.")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperHeight")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int PaperHeight {
			get { return paperHeight; }
			set {
				if(paperHeight == value) return;
				paperHeight = value;
			}
		}
		[
		Description("Gets or sets a value indicating whether the page orientation is landscape."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.Landscape"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool Landscape {
			get { return landscape; }
			set {
				if(landscape == value) return;
				landscape = value;
			}
		}
		[Description("Gets or sets the margins of a print page. ")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.Margins")]
		[XtraSerializableProperty(), NotifyParentProperty(true)]
		public Margins Margins {
			get { return margins; }
			set {
				if(margins == value) return;
				margins = value;
			}
		}
		bool ShouldSerializeMargins() { return !margins.Equals(DefaultMargins); }
		void ResetMargins() { margins = (Margins)DefaultMargins.Clone(); }
		public PivotGridPageSettings()
			: base() {
			Reset();
		}
		public PageSettings ToPageSettings() {
			PageSettings pageSettings = new PageSettings();
			PaperSize paperSize = new PaperSize();
			paperSize.RawKind = (int)PaperKind;
			if(PaperKind == PaperKind.Custom) {
				paperSize.Width = PaperWidth;
				paperSize.Height = PaperHeight;
			}
			pageSettings.PaperSize = paperSize;
			pageSettings.Landscape = Landscape;
			pageSettings.Margins = (Margins)Margins.Clone();
			return pageSettings;
		}
		public void Assign(PivotGridPageSettings obj) {
			this.PaperWidth = obj.PaperWidth;
			this.PaperHeight = obj.PaperHeight;
			this.PaperKind = obj.PaperKind;
			this.Landscape = obj.Landscape;
			this.Margins = obj.Margins;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder(PaperKind.ToString());
			if(PaperKind == PaperKind.Custom)
				result.Append(", ").Append(PaperWidth).Append("x").Append(PaperHeight);
			if(Landscape)
				result.Append(", Landscape");
			result.Append(", ").Append(Margins);
			return result.ToString();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsEmpty {
			get {
				return Landscape == false && PaperKind == PaperKind.Letter && Margins.Equals(DefaultMargins) && PaperWidth == 0 && PaperHeight == 0;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			this.paperKind = PaperKind.Letter;
			this.margins = (Margins)DefaultMargins.Clone();
			this.paperWidth = 0;
			this.paperHeight = 0;
			this.landscape = false;
		}
	}	
	public interface IASPxPivotGridDataOwner {
		object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex);
		void CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo);
		int GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder);
		object GetCustomGroupInterval(PivotGridFieldBase field, object value);
		bool IsPivotDataCanDoRefresh { get; }
		void ResetControlHierarchy();
		void RequireUpdateData();
		void EnsureRefreshData();
		PivotGridFieldCollectionBase Fields { get; }
		void ElementTemplatesChanged();
		void OnFieldAreaChanged(PivotGridFieldBase field);
		void OnFieldAreaIndexChanged(PivotGridFieldBase field);
		void OnFieldVisibleChanged(PivotGridFieldBase field);
		void OnFieldFilterChanged(PivotGridFieldBase field);
		bool OnFieldValueStateChanging(PivotGridFieldBase field, object[] values, bool isCollapsed);
		void OnFieldValueStateChanged(PivotGridFieldBase field, object[] values, bool isCollapsed, bool success);
		void OnDataSourceChanged();
		string GetFieldValueDisplayText(PivotFieldValueItem item);
		string GetFieldValueDisplayText(PivotGridFieldBase field, object value);
		string CustomCellDisplayText(PivotGridCellItem cellItem);
	}
	public class PivotOLAPKPIValue {
		object value, goal;
		int status, trend;
		double weight;
		public PivotOLAPKPIValue(object value, object goal, int status, int trend, double weight) {
			this.value = value;
			this.goal = goal;
			this.status = status;
			this.trend = trend;
			this.weight = weight;
		}
		public object Value { get { return value; } }
		public object Goal { get { return goal; } }
		public int Status { get { return status; } }
		public int Trend { get { return trend; } }
		public double Weight { get { return weight; } }
	}
	public class PivotOLAPKPIMeasures {
		string kpiName;
		string value, goal, status, trend, weight;
		public PivotOLAPKPIMeasures(string kpiName, string value, string goal, string status, string trend, string weight) {
			this.kpiName = kpiName;
			this.value = value;
			this.goal = goal;
			this.status = status;
			this.trend = trend;
			this.weight = weight;
		}
		public string KPIName { get { return kpiName; } }
		public string ValueMeasure { get { return value; } }
		public string GoalMeasure { get { return goal; } }
		public string StatusMeasure { get { return status; } }
		public string TrendMeasure { get { return trend; } }
		public string WeightMeasure { get { return weight; } }
		public override string ToString() {
			return KPIName;
		}
	}
	public interface IOLAPMember {
		string UniqueName { get; }
		object Value { get; }
	}
	public class PivotVisualItemsBase {
		public delegate void ClearedHandler(PivotVisualItemsBase items);
		PivotGridData data;
		PivotFieldValueItemsCreator columnItemsCreator;
		PivotFieldValueItemsCreator rowItemsCreator;
		PivotGridCellDataProviderBase cellDataProvider;
		PivotGridCellStreamDataProvider streamDataProvider;
		public PivotVisualItemsBase(PivotGridData data) {
			this.data = data;
		}
		protected PivotGridData Data {
			get { return data; }
		}
		protected PivotFieldValueItemsCreator ColumnItemsCreator {
			get {
				if(columnItemsCreator == null)
					columnItemsCreator = CreateColumnItemsCreator();
				return columnItemsCreator;
			}
		}
		protected virtual PivotFieldValueItemsCreator RowItemsCreator {
			get {
				if(rowItemsCreator == null)
					rowItemsCreator = CreateRowItemsCreator();
				return rowItemsCreator;
			}
		}
		public PivotGridCellDataProviderBase CellDataProvider {
			get {
				if(StreamDataProvider != null) 
					return StreamDataProvider;
				if(cellDataProvider == null)
					cellDataProvider = CreateCellDataProvider();
				return cellDataProvider;
			}
		}
		protected PivotGridCellStreamDataProvider StreamDataProvider {
			get { return streamDataProvider; }
			set { streamDataProvider = value; }
		}
		public bool IsReady {
			get { return ColumnCount != 0 && RowCount != 0; }
		}
		public int ColumnCount {
			get { return ColumnItemsCreator.LastLevelItemCount; }
		}
		public int RowCount {
			get { return RowItemsCreator.LastLevelItemCount; }
		}
		public event ClearedHandler Cleared;
		protected virtual PivotFieldValueItemsCreator CreateColumnItemsCreator() {
			return new PivotFieldValueItemsCreator(Data, true);
		}
		protected virtual PivotFieldValueItemsCreator CreateRowItemsCreator() {
			return new PivotFieldValueItemsCreator(Data, false);
		}
		protected virtual PivotGridCellDataProviderBase CreateCellDataProvider() {
			return new PivotGridCellDataProvider(Data);
		}
		public void EnsureIsCalculated() {
			if(!IsReady) 
				Calculate();
		}
		protected virtual void Calculate() {
			ColumnItemsCreator.CreateItems();
			RowItemsCreator.CreateItems();
		}
		public virtual void Clear() {
			if(columnItemsCreator != null) 
				ClearItemsCreator(columnItemsCreator);
			if(rowItemsCreator != null) 
				ClearItemsCreator(rowItemsCreator);
			StreamDataProvider = null;
			if(Cleared != null)
				Cleared(this);
		}
		protected void ClearItemsCreator(PivotFieldValueItemsCreator itemsCreator) {
			itemsCreator.Clear();
			itemsCreator.ResetDataProvider();
		}
		protected PivotFieldValueItemsCreator GetItemsCreator(bool isColumn) {
			return isColumn ? ColumnItemsCreator : RowItemsCreator;
		}
		public PivotFieldValueItem GetColumnItem(int index) {
			return ColumnItemsCreator.GetLastLevelItem(index);
		}
		public PivotFieldValueItem GetRowItem(int index) {
			return RowItemsCreator.GetLastLevelItem(index);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return CellDataProvider.GetCellValue(GetColumnItem(columnIndex), GetRowItem(rowIndex));
		}
		public object GetFieldValue(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return item != null ? item.Value : null;
		}
		public PivotGridValueType GetFieldValueType(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			if(item == null) throw new ArgumentException("no field value found");
			return item.ValueType;
		}
		public IOLAPMember GetOLAPMember(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return GetOLAPMemberCore(field, item);
		}
		protected IOLAPMember GetOLAPMemberCore(PivotGridFieldBase field, PivotFieldValueItem item) {
			return item != null ? Data.GetOLAPMember(field, item.VisibleIndex) : null;
		}
		public bool IsObjectCollapsed(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return item != null ? Data.IsObjectCollapsed(field.IsColumn, item.VisibleIndex) : false;
		}
		public int GetLevelCount(bool isColumn) {
			return GetItemsCreator(isColumn).LevelCount;
		}
		public int GetLastLevelItemCount(bool isColumn) {
			return GetItemsCreator(isColumn).LastLevelItemCount;
		}
		public int GetItemCount(bool isColumn) {
			return GetItemsCreator(isColumn).Count;
		}
		public PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);			
			return GetLastLevelItemCore(lastLevelIndex, itemsCreator);
		}
		protected PivotFieldValueItem GetLastLevelItemCore(int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount) return null;
			return itemsCreator.GetLastLevelItem(lastLevelIndex);
		}
		public PivotFieldValueItem GetItem(bool isColumn, int index) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			if(index < 0 || index >= itemsCreator.Count) return null;
			return itemsCreator[index];
		}
		public PivotFieldValueItem GetItem(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return GetItemCore(lastLevelIndex, level, itemsCreator);
		}
		protected PivotFieldValueItem GetItemCore(int lastLevelIndex, int level, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = itemsCreator.GetParentItem(item);
			return item;
		}
		public PivotFieldValueItem GetItem(PivotGridFieldBase field, int lastLevelIndex) {			
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(field.IsColumn);
			return GetItemCore(field, lastLevelIndex, itemsCreator);
		}
		protected PivotFieldValueItem GetItemCore(PivotGridFieldBase field, int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount || !field.IsColumnOrRow) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelItem(lastLevelIndex);
			while(item != null && item.Field != field)
				item = itemsCreator.GetParentItem(item);
			return item;
		}
		public PivotFieldValueItem GetParentItem(bool isColumn, PivotFieldValueItem item) {
			return GetItemsCreator(isColumn).GetParentItem(item);
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(bool isColumn, int index) {
			return GetItemsCreator(isColumn).GetFieldSortConditions(index);
		}
		public List<PivotGridFieldPair> GetSortedBySummaryFields(bool isColumn, int itemIndex) {
			return GetItemsCreator(isColumn).GetSortedBySummaryFields(itemIndex);
		}
		public bool IsFieldSortedBySummary(bool isColumn, PivotGridFieldBase field, PivotGridFieldBase dataField, int index) {
			return GetItemsCreator(isColumn).IsFieldSortedBySummary(field, dataField, index);
		}
		public bool GetIsAnyFieldSortedBySummary(bool isColumn, int index) {
			return GetItemsCreator(isColumn).GetIsAnyFieldSortedBySummary(index);
		}
		public PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex) {
			return new PivotGridCellItem(CellDataProvider, GetColumnItem(columnIndex),
				GetRowItem(rowIndex), columnIndex, rowIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex);
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex, maxRowCount);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, 
			int maxRowCount, List<string> customColumns) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex);
			return Data.GetOLAPDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex,
				maxRowCount, customColumns);
		}
		public string SavedFieldValueItemsState() {
			MemoryStream stream = new MemoryStream();
			SavedFieldValueItemsStateCore(stream);
			return Convert.ToBase64String(stream.ToArray());
		}
		protected virtual void SavedFieldValueItemsStateCore(MemoryStream stream) {
			ColumnItemsCreator.SaveToStream(stream);
			RowItemsCreator.SaveToStream(stream);
		}
		public string SavedDataCellsState() {
			MemoryStream stream = new MemoryStream();
			SavedDataCellsStateCore(stream);
			return Convert.ToBase64String(stream.ToArray());
		}
		protected virtual void SavedDataCellsStateCore(MemoryStream stream) {
			CellDataProvider.SaveToStream(stream, ColumnItemsCreator, RowItemsCreator);
		}
		internal void LoadFieldValueItemsState(string state) {
			if(string.IsNullOrEmpty(state)) return;
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			LoadFieldValueItemsStateCore(stream);
			Calculate();
		}
		protected virtual void LoadFieldValueItemsStateCore(MemoryStream stream) {
			ColumnItemsCreator.LoadFromStream(stream);
			RowItemsCreator.LoadFromStream(stream);
		}
		internal void LoadDataCellsState(string state) {
			if(string.IsNullOrEmpty(state)) return;			
			StreamDataProvider = new PivotGridCellStreamDataProvider(Data);
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			LoadDataCellsStateCore(stream);
		}
		protected virtual void LoadDataCellsStateCore(MemoryStream stream) {			
			StreamDataProvider.LoadFromStream(stream, ColumnItemsCreator, RowItemsCreator);
		}
	}
}
