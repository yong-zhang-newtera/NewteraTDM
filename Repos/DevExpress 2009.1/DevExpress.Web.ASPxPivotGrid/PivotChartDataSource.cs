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

using System.Web.UI;
using System.Collections;
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotChartDataSourceView : DataSourceView {
		ASPxPivotGrid pivot;
		PivotChartDataSource chartDataSource;
		public PivotChartDataSource ChartDataSource { get { return chartDataSource; } }
		public PivotChartDataSourceView(ASPxPivotGrid pivot)
			: base(pivot, string.Empty) {
			this.pivot = pivot;
			this.chartDataSource = CreateChartDataSource(pivot);
		}
		protected virtual PivotChartDataSource CreateChartDataSource(ASPxPivotGrid pivot) {
			return new PivotChartDataSource(pivot);
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			return chartDataSource;
		}
		public virtual void InvalidateChartData() {
			chartDataSource.InvalidateChartData();
		}
	}
	public class PivotChartDataSource : IList, ITypedList {
		readonly ASPxPivotGrid pivot;
		public PivotChartDataSource(ASPxPivotGrid pivot) {
			this.pivot = pivot;
		}
		public string GetChartText(PivotFieldValueItem item) {
			PivotFieldValueItemsCreator itemsCreator = item.IsColumn ? columnItemsCreator : rowItemsCreator;
			string result = ((IASPxPivotGridDataOwner)pivot).GetFieldValueDisplayText(item);
			if(item.StartLevel == 0) return result;
			while((item = itemsCreator.GetParentItem(item)) != null)
				result = ((IASPxPivotGridDataOwner)pivot).GetFieldValueDisplayText(item) + " | " + result;
			return result;
		}
		public PivotGridOptionsChartDataSourceBase Options { get { return pivot.OptionsChartDataSource; } }
		Type cellValueType;
		public Type CellValueType {
			get {
				if(cellValueType == null)
					cellValueType = GetCellValueTypeCore();
				return cellValueType;
			}
		}
		protected Type GetCellValueTypeCore() {
			List<PivotGridFieldBase> fields = pivot.Data.GetFieldsByArea(PivotArea.DataArea, false);
			Type res = null;
			for(int i = 0; i < fields.Count; i++) {
				Type fieldType = pivot.Data.GetFieldType(fields[i], false);
				if((res != null && res != fieldType) || fieldType == typeof(object) || fieldType == null)
					return typeof(decimal);
				res = fieldType;
			}
			return res != null ? res : typeof(decimal);
		}
		#region ChartDataSource
		List<PivotGridCellItem> datasourceCells;
		PropertyDescriptorCollection chartProps;
		bool IsChartDataValid { get { return datasourceCells != null; } }
		PivotFieldValueItemsCreator columnItemsCreator;
		PivotFieldValueItemsCreator rowItemsCreator;
		PivotGridCellDataProvider cellDataProvider;
		public virtual void InvalidateChartData() {
			datasourceCells = null;
			chartProps = null;
			cellValueType = null;
		}
		public virtual void EnsureChartData() {
			if(!IsChartDataValid) BuildChartData();
		}
		protected virtual void BuildChartData() {
			CreateFieldValueItems();
			CreateChartDataSourceCells();
			CreatePropertyDescriptors();
		}
		protected virtual void CreateFieldValueItems() {
			columnItemsCreator = new PivotFieldValueItemsCreator(pivot.Data, true);
			columnItemsCreator.CreateItems();
			rowItemsCreator = new PivotFieldValueItemsCreator(pivot.Data, false);
			rowItemsCreator.CreateItems();
			cellDataProvider = new PivotGridCellDataProvider(pivot.Data);
		}
		protected virtual void CreatePropertyDescriptors() {
			if(chartProps == null) chartProps = new PropertyDescriptorCollection(null);
			else chartProps.Clear();
			chartProps.Add(new PivotChartDescriptor(this, PivotChartDescriptorType.ColumnField));
			chartProps.Add(new PivotChartDescriptor(this, PivotChartDescriptorType.RowField));
			chartProps.Add(new PivotChartDescriptor(this, PivotChartDescriptorType.CellValue));
		}
		protected virtual void CreateChartDataSourceCells() {
			datasourceCells = AllCells;
			StripTotalsFromChartDataSource();
		}
		protected virtual void StripTotalsFromChartDataSource() {
			if(!Options.ShouldRemoveTotals) return;
			for(int i = datasourceCells.Count - 1; i >= 0; i--) {
				if(Options.ShouldRemoveItem(true, datasourceCells[i].ColumnValueType) ||
					Options.ShouldRemoveItem(false, datasourceCells[i].RowValueType)) {
					datasourceCells.RemoveAt(i);
				}
			}
		}
		protected virtual List<PivotGridCellItem> AllCells {
			get {
				List<PivotGridCellItem> result = new List<PivotGridCellItem>();
				for(int j = 0; j < rowItemsCreator.LastLevelItemCount; j++)
					for(int i = 0; i < columnItemsCreator.LastLevelItemCount; i++)
						result.Add(new PivotGridCellItem(cellDataProvider, columnItemsCreator.GetLastLevelItem(i), rowItemsCreator.GetLastLevelItem(j), i, j));
				return result;
			}
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			for(int i = 0; i < ((ICollection)this).Count; i++) {
				if(index + i >= array.Length) break;
				array.SetValue(((IList)this)[i], index + i);
			}
		}
		int ICollection.Count {
			get {
				EnsureChartData();
				return datasourceCells.Count;
			}
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.Clear() {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IList.Contains(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		int IList.IndexOf(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.Insert(int index, object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void IList.Remove(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.RemoveAt(int index) {
			throw new Exception("The method or operation is not implemented.");
		}
		object IList.this[int index] {
			get {
				EnsureChartData();
				if(index < 0 || index >= ((ICollection)this).Count)
					return null;
				return datasourceCells[index];
			}
			set {
				throw new Exception("The method or operation is not implemented.");
			}
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			EnsureChartData();
			return listAccessors != null && listAccessors.Length > 0 ? new PropertyDescriptorCollection(null) : chartProps;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return pivot.ID;
		}
		#endregion
		#endregion
	}
	public enum PivotChartDescriptorType { ColumnField, RowField, CellValue };
	public class PivotChartDescriptor : PropertyDescriptor {
		PivotChartDescriptorType type;
		PivotChartDataSource dataSource;
		protected PivotChartDescriptorType Type { get { return type; } }
		protected PivotChartDataSource DataSource { get { return dataSource; } }
		public PivotChartDescriptor(PivotChartDataSource dataSource, PivotChartDescriptorType type)
			: base(GetName(dataSource.Options.ChartDataVertical, type), new Attribute[0] { }) {
			this.dataSource = dataSource;
			this.type = type;
		}
		static string GetName(bool vertical, PivotChartDescriptorType type) {
			switch(type) {
				case PivotChartDescriptorType.CellValue:
					return "Values";
				case PivotChartDescriptorType.ColumnField:
					return vertical ? "Series" : "Arguments";
				case PivotChartDescriptorType.RowField:
					return vertical ? "Arguments" : "Series";
			}
			throw new ArgumentException("Unknown type!");
		}
		public override object GetValue(object component) {
			PivotGridCellItem item = component as PivotGridCellItem;
			if(item == null) return null;
			switch(type) {
				case PivotChartDescriptorType.CellValue:
					return item.Value;
				case PivotChartDescriptorType.ColumnField:
					return dataSource.GetChartText(item.ColumnFieldValueItem);
				case PivotChartDescriptorType.RowField:
					return dataSource.GetChartText(item.RowFieldValueItem);
			}
			return null;
		}
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(object); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {
			get {
				switch(type) {
					case PivotChartDescriptorType.ColumnField:
						return typeof(string);
					case PivotChartDescriptorType.RowField:
						return typeof(string);
					case PivotChartDescriptorType.CellValue:
						return dataSource.CellValueType;
				}
				return null;
			}
		}
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
}
