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
using System.Data;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.Data;
using System.Drawing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Localization;
using System.IO;
namespace DevExpress.Data.PivotGrid {
	public class PivotSummaryPropertyDescriptor : PropertyDescriptor {
		readonly PivotGridData data;
		readonly PivotGridFieldBase field;
		protected PivotGridData Data { get { return data; } }
		public PivotGridFieldBase Field { get { return field; } }
		internal PivotSummaryPropertyDescriptor(PivotGridData data, PivotGridFieldBase field)
			: base(data.GetFieldName(field), null) {
			this.data = data;
			this.field = field;
		}
		public override bool IsBrowsable { get { return Field.Visible; } }
		public override bool IsReadOnly { get { return true; } }
		public override string Category { get { return string.Empty; } }
		public override string Name {
			get {
				if(Field.Area == PivotArea.DataArea) return Data.GetFieldName(Field) + '_' + Field.SummaryType.ToString();
				return Data.GetFieldName(Field);
			}
		}
		public override string DisplayName {
			get { return string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption; }
		}
		public override Type PropertyType { get { return Data.GetFieldType(Field, false); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			PivotSummaryDataRow row = component as PivotSummaryDataRow;
			if(row == null) return null;
			if(Field.IsColumnOrRow) {
				if(Data.GetIsOthersValue(Field, row.ColumnIndex, row.RowIndex))
					return Field.DataType == typeof(string) ? PivotGridLocalizer.GetString(PivotGridStringId.TopValueOthersRow) : null;
				return Data.GetFieldValue(Field, row.ColumnIndex, row.RowIndex);
			} else
				return Data.GetCellValue(row.ColumnIndex, row.RowIndex, Field);
		}
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return DisplayName;
		}
	}
	public class PivotSummaryPropertyDescriptorCollection : PropertyDescriptorCollection {
		public PivotSummaryPropertyDescriptorCollection(PropertyDescriptor[] properties) : base(properties) { }
		public PivotSummaryPropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly) : base(properties, readOnly) { }
		public new PivotSummaryPropertyDescriptor this[int index] { get { return (PivotSummaryPropertyDescriptor)base[index]; } }
		public new PivotSummaryPropertyDescriptor this[string name] { get { return (PivotSummaryPropertyDescriptor)base[name]; } }
		public PivotSummaryPropertyDescriptor this[PivotGridFieldBase field] {
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].Field == field) return this[i];
				return null;
			}
		}
	}
	public class PivotSummaryDataSource : IPivotDataSource {
		readonly PivotGridData data;
		readonly int columnIndex, rowIndex;
		List<Point> children;
		PivotSummaryDataRow[] rows;
		PivotSummaryPropertyDescriptorCollection propertyDescriptors;
		protected PivotGridData Data { get { return data; } }
		protected int ColumnIndex { get { return columnIndex; } }
		protected int RowIndex { get { return rowIndex; } }
		protected List<Point> Children { get { return children; } }
		protected PivotSummaryDataRow[] Rows { get { return rows; } }
		protected PivotSummaryPropertyDescriptorCollection PropertyDescriptors {
			get {
				if(propertyDescriptors == null) {
					propertyDescriptors = CreatePropertyDescriptors();
				}
				return propertyDescriptors;
			}
		}
		PivotSummaryPropertyDescriptorCollection CreatePropertyDescriptors() {
			PivotGridFieldReadOnlyCollection sortedFields = Data.GetSortedFields();
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PivotGridFieldBase field in sortedFields) {
				if(field.Area != PivotArea.FilterArea)
					props.Add(new PivotSummaryPropertyDescriptor(Data, field));
			}
			return new PivotSummaryPropertyDescriptorCollection(props.ToArray());
		}
		public PivotSummaryDataSource(PivotGridData data, int columnIndex, int rowIndex) {
			this.data = data;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			SetupDataSource(data, columnIndex, rowIndex);
		}
		~PivotSummaryDataSource() {
			if(IsDisposed) return;
			Dispose(false);
		}
		void SetupDataSource(PivotGridData data, int columnIndex, int rowIndex) {
			this.children = data.GetCellChildren(columnIndex, rowIndex);
			this.rows = new PivotSummaryDataRow[RowCount];
			this.propertyDescriptors = null;
		}
		void ResetDataSource() {
			this.children = new List<Point>();
			this.rows = new PivotSummaryDataRow[0];
			this.propertyDescriptors = new PivotSummaryPropertyDescriptorCollection(new PropertyDescriptor[0]);
		}
		void DataSourceChanged() {			
		}
		#region IDisposable implementation
		bool isDisposed;
		protected bool IsDisposed { get { return isDisposed; } }
		public void Dispose() {
			if(IsDisposed) return;			
			GC.SuppressFinalize(this);
			Dispose(true);			
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				ResetDataSource();
			isDisposed = true;
		}		
		#endregion
		public int RowCount { get { return Children.Count; } }
		public object GetValue(int rowIndex, int columnIndex) {
			if(rowIndex < 0 || rowIndex >= RowCount || columnIndex < 0 || columnIndex >= PropertyDescriptors.Count) return null;
			return PropertyDescriptors[columnIndex].GetValue(GetRow(rowIndex));
		}
		public object GetValue(int rowIndex, string columnName) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[columnName] == null) return null;
			return PropertyDescriptors[columnName].GetValue(GetRow(rowIndex));
		}
		public object GetValue(int rowIndex, PivotGridFieldBase field) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[field] == null) return null;
			return PropertyDescriptors[field].GetValue(GetRow(rowIndex));
		}
		public bool GetIsOthersValue(int rowIndex, int columnIndex) {
			if(rowIndex < 0 || rowIndex >= RowCount || columnIndex < 0 || columnIndex >= PropertyDescriptors.Count) return false;
			return Data.GetIsOthersValue(PropertyDescriptors[columnIndex].Field, Children[rowIndex].X, Children[rowIndex].Y);
		}
		public bool GetIsOthersValue(int rowIndex, string columnName) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[columnName] == null) return false;
			return Data.GetIsOthersValue(PropertyDescriptors[columnName].Field, Children[rowIndex].X, Children[rowIndex].Y);
		}
		protected PivotSummaryDataRow GetRow(int index) {
			if(index < 0 || index >= RowCount) return null;
			if(Rows[index] == null)
				Rows[index] = new PivotSummaryDataRow(Children[index].X, Children[index].Y);
			return Rows[index];
		}
		public void ExportToXml(string fileName, bool writeSchema) {
			GetDataTable().WriteXml(fileName, writeSchema ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema);
		}
		public void ExportToXml(Stream stream, bool writeSchema) {
			GetDataTable().WriteXml(stream, writeSchema ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema);
		}
		DataTable GetDataTable() {
			DataTable table = new DataTable("SummaryDataSource");
			for(int i = 0; i < PropertyDescriptors.Count; i++)
				table.Columns.Add(PropertyDescriptors[i].Name, PropertyDescriptors[i].PropertyType);
			object[] values = new object[PropertyDescriptors.Count];
			for(int i = 0; i < RowCount; i++) {
				for(int j = 0; j < values.Length; j++)
					values[j] = GetValue(i, j);
				table.Rows.Add(values);
			}
			return table;
		}
		#region IPivotDataSource Members
		public bool IsLive { get { return rowIndex == -1 && columnIndex == -1 && !IsDisposed; } set { } }
		void IPivotDataSource.Refresh() {
			SetupDataSource(Data, ColumnIndex, RowIndex);
			if(this.ListChanged != null) {
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}
		void IPivotDataSource.ResetData() {
			ResetDataSource();
			if(this.ListChanged != null)
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}
		void IPivotDataSource.DataSourceChanged() {
			DataSourceChanged();
		}
		#endregion
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) { return PropertyDescriptors; }
		public string GetListName(PropertyDescriptor[] listAccessors) { return string.Empty; }
		#endregion
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { }
		object IBindingList.AddNew() { return null; }
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) { }
		int IBindingList.Find(PropertyDescriptor property, object key) { return -1; }
		bool IBindingList.IsSorted { get { return false; } }
		public event ListChangedEventHandler ListChanged;
		void IBindingList.RemoveIndex(PropertyDescriptor property) { }
		void IBindingList.RemoveSort() { }
		ListSortDirection IBindingList.SortDirection { get { return new ListSortDirection(); } }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		#endregion
		#region IList Members
		int IList.Add(object value) { return -1; }
		void IList.Clear() { }
		bool IList.Contains(object value) { return IndexOf(value) > -1; }
		public int IndexOf(object value) {
			PivotSummaryDataRow row = value as PivotSummaryDataRow;
			if(row == null) return -1;
			return Children.IndexOf(row.Location);
		}
		void IList.Insert(int index, object value) { }
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void IList.Remove(object value) { }
		void IList.RemoveAt(int index) { }
		public object this[int index] {
			get { return GetRow(index); }
			set { }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { }
		int ICollection.Count { get { return RowCount; } }
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() { return this; }
		#endregion
		#region IEnumerator Members
		int currentPosition;
		object IEnumerator.Current {
			get { return this[currentPosition]; }
		}
		bool IEnumerator.MoveNext() {
			if(currentPosition < RowCount - 1) {
				currentPosition++;
				return true;
			}
			return false;
		}
		void IEnumerator.Reset() { currentPosition = 0; }
		#endregion		
	}
}
