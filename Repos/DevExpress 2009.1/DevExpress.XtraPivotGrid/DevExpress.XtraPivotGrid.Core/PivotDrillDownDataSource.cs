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
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Data.PivotGrid;
using System.Data.OleDb;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid {
	public class PivotDrillDownPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor propertyDescriptor;
		bool isBrowsable;
		int columnIndex;
		internal PivotDrillDownPropertyDescriptor(PropertyDescriptor propertyDescriptor, int columnIndex, bool isBrowsable)
			: base(propertyDescriptor.Name, null) {
			this.propertyDescriptor = propertyDescriptor;
			this.columnIndex = columnIndex;
			this.isBrowsable = isBrowsable;
		}
		protected PropertyDescriptor PropertyDescriptor { get { return propertyDescriptor; } }
		protected virtual int ColumnIndex { get { return columnIndex; } }
		public override bool IsBrowsable { get { return isBrowsable; } }
		public override bool IsReadOnly { get { return PropertyDescriptor.IsReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override string Name { get { return PropertyDescriptor.Name; } }
		public override Type PropertyType { get { return PropertyDescriptor.PropertyType; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			PivotDrillDownDataRow row = component as PivotDrillDownDataRow;
			if(row == null) return null;
			return row[ColumnIndex];
		}
		public override void SetValue(object component, object value) {
			PivotDrillDownDataRow row = component as PivotDrillDownDataRow;
			if(row == null) return;
			row[ColumnIndex] = value;
		}
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return Name;
		}
	}
	public class PivotSummaryDataRow {
		readonly int columnIndex, rowIndex;
		public PivotSummaryDataRow(int columnIndex, int rowIndex) {
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
		}
		public int ColumnIndex { get { return columnIndex; } }
		public int RowIndex { get { return rowIndex; } }
		public Point Location { get { return new Point(ColumnIndex, RowIndex); } }
	}
	public interface IPivotDataSource : IEnumerable, IEnumerator, ITypedList, IBindingList, IDisposable {
		bool IsLive { get; set; }
		void Refresh();
		void ResetData();
		void DataSourceChanged();
	}
	public class PivotDrillDownDataRow : ICustomTypeDescriptor {
		PivotDrillDownDataSource dataSource;
		int index;
		public PivotDrillDownDataRow(PivotDrillDownDataSource dataSource, int index) {
			this.dataSource = dataSource;
			this.index = index;
		}
		[Description("Gets the row's index in the pivot grid's data source.")]
		public int ListSourceRowIndex { get { return DataSource.GetListSourceRowIndex(Index); } }
		[Description("Gets the row's value for the specified field.")]
		public object this[int index] {
			get { return DataSource.GetValue(Index, index); }
			set { DataSource.SetValue(Index, index, value); }
		}
		[Description("Gets the row's value for the specified field.")]
		public object this[string fieldName] {
			get { return DataSource.GetValue(Index, fieldName); }
			set { DataSource.SetValue(Index, fieldName, value); }
		}
		[Description("Gets the row's value for the specified field.")]
		public object this[PivotGridFieldBase field] {
			get { return DataSource.GetValue(Index, field); }
			set { DataSource.SetValue(Index, field, value); }
		}
		[Description("Gets the data source which contains the current row.")]
		public PivotDrillDownDataSource DataSource { get { return dataSource; } }
		[Description("Gets the row's index within PivotDrillDownDataRow.DataSource.")]
		public int Index { get { return index; } }
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return DataSource.GetDescriptorCollection();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return DataSource.GetDescriptorCollection();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
	}
	public abstract class PivotDrillDownDataSource : IPivotDataSource {
		public const int AllRows = -1;
		int currentIndex;
		PivotDrillDownDataRow[] rows;
		public PivotDrillDownDataSource() {
		}
		~PivotDrillDownDataSource() {
			if(IsDisposed) return;
			Dispose(false);
		}
		#region IDisposable implementation
		bool isDisposed;
		protected bool IsDisposed { get { return isDisposed; } }
		public void Dispose() {
			if(IsDisposed) return;
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			isDisposed = true;
		}
		#endregion
		protected PivotDrillDownDataRow[] Rows {
			get {
				if(rows == null) rows = new PivotDrillDownDataRow[RowCountInternal];
				return rows;
			}
		}
		protected abstract int RowCountInternal { get; }
		internal abstract int GetListSourceRowIndex(int rowIndex);
		protected internal abstract PropertyDescriptorCollection GetDescriptorCollection();
		protected virtual void OnResetData() { }
		protected virtual void OnRefreshData() { }
		protected virtual void OnDataSourceChanged() { IsLive = false; }
		#region IPivotDataSource Members
		[Description("This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.")]
		public virtual bool IsLive { get { return false; } set { } }
		void IPivotDataSource.Refresh() {
			rows = null;
			OnRefreshData();
			if(this.ListChanged != null) {
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}
		void IPivotDataSource.ResetData() {
			rows = new PivotDrillDownDataRow[0];
			OnResetData();
			if(this.ListChanged != null)
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}
		void IPivotDataSource.DataSourceChanged() {
			OnDataSourceChanged();
		}
		#endregion
		#region IList
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		int IList.Add(object value) { return -1; }
		void IList.Clear() { }
		void IList.Insert(int index, object value) { }
		void IList.Remove(object value) { }
		void IList.RemoveAt(int index) { }
		bool IList.Contains(object value) { return IndexOf(value) > -1; }
		int IList.IndexOf(object value) { return IndexOf(value); }
		object IList.this[int index] { get { return this[index]; } set { } }
		#endregion
		#region ICollection
		void ICollection.CopyTo(Array array, int count) { }
		int ICollection.Count { get { return RowCount; } }
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		#region IEnumerator
		IEnumerator IEnumerable.GetEnumerator() {
			((IEnumerator)this).Reset();
			return this; 
		}
		object IEnumerator.Current { get { return this[currentIndex]; } }
		bool IEnumerator.MoveNext() {
			return ++currentIndex < RowCount;
		}
		void IEnumerator.Reset() {
			currentIndex = -1;
		}
		#endregion		
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return GetDescriptorCollection();
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "PivotDrillDownDataSource"; }
		#region IBindingList
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.AddIndex(PropertyDescriptor property) { }
		void IBindingList.ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction) { }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		int IBindingList.Find(PropertyDescriptor property, object key) { return -1; }
		bool IBindingList.SupportsSorting { get { return false; } }
		bool IBindingList.IsSorted { get { return false; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		System.ComponentModel.ListSortDirection IBindingList.SortDirection { get { return new System.ComponentModel.ListSortDirection(); } }
		public event System.ComponentModel.ListChangedEventHandler ListChanged;
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		void IBindingList.RemoveSort() { }
		object IBindingList.AddNew() { return null; }
		void IBindingList.RemoveIndex(PropertyDescriptor property) { }
		#endregion
		[Description("Gets the number of records in the data source.")]
		public int RowCount { get { return Rows.Length; } }
		[Description("Provides indexed access to the rows in the current data source.")]
		public PivotDrillDownDataRow this[int index] {
			get { return GetRow(index); }
			set { }
		}
		protected PivotDrillDownDataRow GetRow(int index) {
			if(index < 0 || index >= RowCount) return null;
			if(Rows[index] == null) Rows[index] = new PivotDrillDownDataRow(this, index);
			return Rows[index];
		}
		public int IndexOf(object value) {
			for(int i = 0; i < RowCount; i++)
				if(Rows[i] == value) return i;
			return -1;
		}
		public abstract object GetValue(int rowIndex, PivotGridFieldBase field);
		public abstract object GetValue(int rowIndex, int columnIndex);
		public abstract object GetValue(int rowIndex, string fieldName);
		public abstract void SetValue(int rowIndex, PivotGridFieldBase field, object value);
		public abstract void SetValue(int rowIndex, int columnIndex, object value);
		public abstract void SetValue(int rowIndex, string fieldName, object value);
		protected bool IsRowIndexValid(int rowIndex) {
			return rowIndex >= 0 && rowIndex < RowCount;
		}
	}
	public class OLAPPivotDrillDownDataSource : PivotDrillDownDataSource {
		public delegate bool IsRowFitDelegate(object[] row);
		DataTable innerTable;
		protected DataTable InnerTable { get { return innerTable; } }
		public OLAPPivotDrillDownDataSource(OleDbDataReader reader, IsRowFitDelegate filter) {
			innerTable = new DataTable();
			for(int i = 0; i < reader.FieldCount; i++) {
				innerTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
			}
			object[] row = new object[reader.FieldCount];
			while(reader.Read()) {
				reader.GetValues(row);
				if(filter == null || filter(row))
					innerTable.Rows.Add(row);
			}
		}
		protected override int RowCountInternal { get { return innerTable.Rows.Count; } }
		internal override int GetListSourceRowIndex(int rowIndex) { return rowIndex; }
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			PropertyDescriptorCollection props = ((ITypedList)innerTable.DefaultView).GetItemProperties(null);
			PivotDrillDownPropertyDescriptor[] properties = new PivotDrillDownPropertyDescriptor[props.Count];
			for(int i = 0; i < props.Count; i++) 
				properties[i] = new PivotDrillDownPropertyDescriptor(props[i], i, props[i].IsBrowsable);
			return new PropertyDescriptorCollection(properties);
		}
		public override object GetValue(int rowIndex, PivotGridFieldBase field) {
			return GetValue(rowIndex, field.OLAPDrillDownColumnName);
		}
		public override object GetValue(int rowIndex, int columnIndex) {
			return innerTable.Rows[rowIndex][columnIndex];
		}
		public override object GetValue(int rowIndex, string fieldName) {
			return innerTable.Rows[rowIndex][fieldName];
		}
		public override void SetValue(int rowIndex, PivotGridFieldBase field, object value) {
			throw new Exception("The operation is not supported.");
		}
		public override void SetValue(int rowIndex, int columnIndex, object value) {
			throw new Exception("The operation is not supported.");
		}
		public override void SetValue(int rowIndex, string fieldName, object value) {
			throw new Exception("The operation is not supported.");
		}
	}
	public abstract class NativePivotDrillDownDataSource : PivotDrillDownDataSource {
		PivotDataController controller;
		protected PivotDataController Controller { get { return controller; } }
		public NativePivotDrillDownDataSource(PivotDataController controller) {
			this.controller = controller;
			(this as IEnumerator).Reset();
		}
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			PivotDrillDownPropertyDescriptor[] properties = new PivotDrillDownPropertyDescriptor[Controller.Columns.Count];
			for(int i = 0; i < properties.Length; i++) {
				DataColumnInfo columnInfo = Controller.Columns[i];
				properties[i] = new PivotDrillDownPropertyDescriptor(columnInfo.PropertyDescriptor, columnInfo.Index, columnInfo.Visible);
			}
			return new PropertyDescriptorCollection(properties);
		}		
		public override object GetValue(int rowIndex, PivotGridFieldBase field) {
			return GetValue(rowIndex, field.ColumnHandle);
		}
		public override object GetValue(int rowIndex, int columnIndex) {
			if(!IsRowIndexValid(rowIndex)) return null;
			return Controller.GetRowValue(GetListSourceRowIndex(rowIndex), columnIndex);
		}
		public override object GetValue(int rowIndex, string fieldName) {
			if(!IsRowIndexValid(rowIndex)) return null;
			return Controller.GetRowValue(GetListSourceRowIndex(rowIndex), fieldName);
		}
		public override void SetValue(int rowIndex, PivotGridFieldBase field, object value) {
			if(!IsRowIndexValid(rowIndex))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), field.ColumnHandle, value);
		}
		public override void SetValue(int rowIndex, int columnIndex, object value) {
			if(!IsRowIndexValid(rowIndex))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), columnIndex, value);
		}
		public override void SetValue(int rowIndex, string fieldName, object value) {
			if(!IsRowIndexValid(rowIndex))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), fieldName, value);
		}
	}
	public class ClientPivotDrillDownDataSource : NativePivotDrillDownDataSource {
		VisibleListSourceRowCollection visibleListSourceRows;
		int startVisibleIndex;
		GroupRowInfo groupRow;
		int fMaxRowCount;
		int lifeCount;
		public ClientPivotDrillDownDataSource(PivotDataController controller, VisibleListSourceRowCollection visibleListSourceRows, GroupRowInfo groupRow, int maxRowCount)
			: base(controller) {
			this.visibleListSourceRows = visibleListSourceRows;
			this.startVisibleIndex = groupRow != null ? groupRow.ChildControllerRow : 0;
			this.groupRow = groupRow;
			this.fMaxRowCount = maxRowCount;
			this.lifeCount = 0;
		}
		public override bool IsLive {
			get { return lifeCount == 0 && !IsDisposed; }
			set {
				if(IsLive == value)
					return;
				lifeCount ++;
			}
		}
		protected new PivotDataController Controller { get { return (PivotDataController)base.Controller; } }
		protected override int RowCountInternal {
			get {
				int actualRowCount = groupRow != null ? groupRow.ChildControllerRowCount : visibleListSourceRows != null ? visibleListSourceRows.Count : 0;
				if(fMaxRowCount == PivotDrillDownDataSource.AllRows)
					return actualRowCount;
				else
					return Math.Min(fMaxRowCount, actualRowCount);
			}
		}
		internal override int GetListSourceRowIndex(int rowIndex) {
			return Controller.GetListSourceRowByControllerRow(visibleListSourceRows, startVisibleIndex + rowIndex);
		}
	}
	#region ServerPivotDrillDownDataRow
	#endregion
	#region ServerPivotDrillDownDataSource
	#endregion
}
