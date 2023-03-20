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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
namespace DevExpress.XtraPivotGrid.Data { 
	public abstract class PivotGridCellDataProviderBase {
		PivotGridData data;
		public PivotGridCellDataProviderBase(PivotGridData data) {
			this.data = data;
		}
		public PivotGridData Data { get { return data; } }
		public abstract object GetCellValue(PivotGridCellItem cellItem);
		public abstract object GetCellValue(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem);
		public static int GetDataIndex(PivotArea dataFieldArea, PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			return dataFieldArea == PivotArea.ColumnArea ? columnItem.DataIndex : rowItem.DataIndex;
		}
		public static PivotGridCustomTotalBase GetCustomTotal(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			if(rowItem.CustomTotal != null)
				return rowItem.CustomTotal;
			if(columnItem.CustomTotal != null)
				return columnItem.CustomTotal;
			return null;
		}
		public static PivotSummaryType GetSummaryType(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, PivotGridFieldBase dataField) {
			PivotSummaryType summaryType = dataField != null ? dataField.SummaryType : PivotSummaryType.Sum;
			PivotGridCustomTotalBase customTotal = GetCustomTotal(columnItem, rowItem);
			if(customTotal != null)
				summaryType = customTotal.SummaryType;
			return summaryType;
		}
		public void SaveToStream(Stream stream, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItemsCreator rowItemCreator) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			for(int i = 0; i < rowItemCreator.LastLevelItemCount; i ++) {
				SaveRowToStream(writer, columnItemCreator, rowItemCreator.GetLastLevelItem(i), i);
			}
		}
		void SaveRowToStream(TypedBinaryWriter writer, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItem rowItem, int rowIndex) {
			for(int i = 0; i < columnItemCreator.LastLevelItemCount; i ++)
				SaveCellToStream(writer, columnItemCreator.GetLastLevelItem(i), rowItem, i, rowIndex);
		}
		void SaveCellToStream(TypedBinaryWriter writer, PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, int colIndex, int rowIndex) {
			PivotGridCellItem cellItem = new PivotGridCellItem(this, columnItem, rowItem, colIndex, rowIndex);
			Type cellType = cellItem.Value != null ? cellItem.Value.GetType() : null;
			writer.WriteType(cellType);
			if(cellType != null) {
				writer.WriteObject(cellItem.Value);
			}
		}
	}
	public class PivotGridCellDataProvider : PivotGridCellDataProviderBase {
		public PivotGridCellDataProvider(PivotGridData data) : base(data) {
		}
		public override object GetCellValue(PivotGridCellItem cellItem) {
			return Data.GetCellValue(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex, cellItem.SummaryType);
		}		
		public override object GetCellValue(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			int dataIndex = GetDataIndex(Data.OptionsDataField.DataFieldArea, columnItem, rowItem);
			PivotGridFieldBase dataField = dataIndex > -1 ? Data.GetFieldByArea(PivotArea.DataArea, dataIndex) : null;
			PivotSummaryType summaryType = GetSummaryType(columnItem, rowItem, dataField);
			return Data.GetCellValue(columnItem.VisibleIndex, rowItem.VisibleIndex, dataIndex, summaryType);
		}
	}
	public class PivotGridCellStreamDataProvider : PivotGridCellDataProviderBase {
		int columnCount, rowCount;
		object[,] values;
		public PivotGridCellStreamDataProvider(PivotGridData data) : base(data) {
			this.columnCount = 0;
			this.rowCount = 0;
			this.values = null;
		}
		public int ColumnCount { get { return columnCount; } }
		public int RowCount { get { return rowCount; } }
		public override object GetCellValue(PivotGridCellItem cellItem) {
			return GetCellValueCore(cellItem.ColumnIndex, cellItem.RowIndex);
		}
		public override object GetCellValue(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			return GetCellValueCore(columnItem.MinLastLevelIndex, rowItem.MinLastLevelIndex);
		}
		protected object GetCellValueCore(int columnIndex, int rowIndex) {
			if(this.values == null) return null;
			if(columnIndex >= this.columnCount) return null;
			if(rowIndex >= this.rowCount) return null;
			return values[rowIndex, columnIndex];
		}
		public void LoadFromStream(Stream stream, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItemsCreator rowItemCreator) {
			this.columnCount = columnItemCreator.LastLevelItemCount;
			this.rowCount = rowItemCreator.LastLevelItemCount;
			this.values = new object[RowCount, ColumnCount];
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			for(int i = 0; i < RowCount; i ++) {
				for(int j = 0; j < ColumnCount; j ++) {
					PivotGridCellItem cellItem = new PivotGridCellItem(this, columnItemCreator.GetLastLevelItem(j), 
						rowItemCreator.GetLastLevelItem(i), j, i);
					Type cellType = reader.ReadType();
					if(cellType != null) {
						this.values[i, j] = reader.ReadObject(cellType);
					}
				}
			}
		}
	}
	public class PivotGridCellItem	{		
		PivotGridCellDataProviderBase dataProvider;
		PivotFieldValueItem columnFieldValueItem;
		PivotFieldValueItem rowFieldValueItem;
		object _value;
		string text;
		PivotGridFieldBase dataField;
		int columnIndex, rowIndex;
		public PivotGridCellItem(PivotGridCellDataProviderBase dataProvider, PivotFieldValueItem columnFieldValueItem, PivotFieldValueItem rowFieldValueItem, int columnIndex, int rowIndex){
			this.dataProvider = dataProvider;
			this.columnFieldValueItem = columnFieldValueItem;
			this.rowFieldValueItem = rowFieldValueItem;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			this.dataField = DataIndex > -1 ? Data.GetFieldByArea(PivotArea.DataArea, DataIndex) : null;
			if(IsValueValid) {
				this._value = DataProvider.GetCellValue(this);
				this.text = GetDisplayText();
			} else {
				this._value = null;
				this.text = string.Empty;
			}
		}
		protected PivotGridCellDataProviderBase DataProvider { get { return dataProvider; } }
		public PivotGridData Data { get { return DataProvider.Data; } }
		public PivotFieldValueItem ColumnFieldValueItem { get { return columnFieldValueItem; } }
		public PivotFieldValueItem RowFieldValueItem { get { return rowFieldValueItem; } }
		public int ColumnIndex { get { return columnIndex; }}
		public int RowIndex { get { return rowIndex; } }
		public object Value { get { return _value; } set { _value = value; } }
		public string Text { get { return text; } set { text = value; } }
		public bool IsEmpty { get { return text == null || text == string.Empty; } }
		public int ColumnFieldIndex { get { return ColumnFieldValueItem.VisibleIndex; }}
		public int RowFieldIndex { get { return RowFieldValueItem.VisibleIndex; } }
		public PivotGridFieldBase ColumnField { get { return ColumnFieldValueItem.ColumnField; } }
		public PivotGridFieldBase RowField { get { return RowFieldValueItem.RowField; } }
		public int DataIndex { 
			get { 
				return Data.OptionsDataField.DataFieldArea == PivotArea.ColumnArea ? 
					ColumnFieldValueItem.DataIndex : 
					RowFieldValueItem.DataIndex;
			} 
		}
		public PivotGridFieldBase DataField { get { return dataField; } }
		public Type CellObjectType {
			get {
				if(DataField == null || !IsValueValid) return null;
				if(SummaryType == PivotSummaryType.Count) return typeof(int);
				if(IsObjectTypeChangedBySummaryType) return typeof(decimal);
				return Data.GetFieldType(DataField);
			}
		}
		bool IsObjectTypeChangedBySummaryType {
			get {
				return SummaryType != PivotSummaryType.Average && SummaryType != PivotSummaryType.Max && SummaryType != PivotSummaryType.Min && SummaryType != PivotSummaryType.Custom;
			}
		}
		public PivotGridValueType ColumnValueType { get { return ColumnFieldValueItem.ValueType; } }
		public PivotGridValueType RowValueType { get { return RowFieldValueItem.ValueType; } }
		public PivotSummaryType SummaryType {
			get {
				PivotSummaryType summaryType = DataField != null ? DataField.SummaryType : PivotSummaryType.Sum;
				if(CustomTotal != null)
					summaryType = CustomTotal.SummaryType;
				return summaryType;
			}
		}
		public PivotGridCustomTotalBase ColumnCustomTotal { get { return ColumnFieldValueItem.CustomTotal; } }
		public PivotGridCustomTotalBase RowCustomTotal { get { return RowFieldValueItem.CustomTotal; } }
		public PivotKPIGraphic KPIGraphic {
			get {
				if(Data == null || DataField == null) return PivotKPIGraphic.None;
				return Data.GetKPIGraphic(DataField);
			}
		}
		public int KPIValue {
			get {
				try {
					if(Value != null) {
						int state = Convert.ToInt32(Value);
						if(Data.IsValidKPIState(state)) return state;
					}
				} catch { }
				return PivotGridData.InvalidKPIValue;
			}
		}
		public bool ShowKPIGraphic {
			get {
				return Data != null && Value != null && KPIGraphic != PivotKPIGraphic.None && KPIValue != PivotGridData.InvalidKPIValue;
			}
		}
		public bool IsValueValid { get { return ColumnCustomTotal == null || RowCustomTotal == null; } }
		public bool IsColumnCustomTotal { get { return ColumnCustomTotal != null; } }
		public bool IsRowCustomTotal { get { return RowCustomTotal != null; } }
		public PivotGridCustomTotalBase CustomTotal { 
			get { 
				if(RowCustomTotal != null)
					return RowCustomTotal;
				if(ColumnCustomTotal != null)
					return ColumnCustomTotal;
				return null;
			} 
		}
		public bool IsTotalAppearance { get { return IsValueType(PivotGridValueType.Total); } }
		public bool IsGrandTotalAppearance { get { return IsValueType(PivotGridValueType.GrandTotal); } }
		public bool IsCustomTotalAppearance { get { return CustomTotal != null; } }
		public object GetCellValue(PivotGridFieldBase dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetFieldValue(PivotGridFieldBase field) {
			if(field.Area == PivotArea.DataArea) return GetCellValue(field);
			return IsFieldValueRetrievable(field) ? Data.GetFieldValue(field, ColumnFieldIndex, RowFieldIndex) : null;
		}
		public bool IsOthersFieldValue(PivotGridFieldBase field) {
			if(field.Area == PivotArea.DataArea || !IsFieldValueRetrievable(field)) return false;
			return Data.GetIsOthersValue(field, ColumnFieldIndex, RowFieldIndex);
		}
		public bool IsFieldValueExpanded(PivotGridFieldBase field) {
			if(!IsFieldValueRetrievable(field)) return false;
			return Data.IsFieldValueExpanded(field, ColumnFieldIndex, RowFieldIndex);
		}
		int GetFieldAreaIndex(PivotGridFieldBase field) {
			return field.Area != PivotArea.DataArea ? field.AreaIndex : Data.DataField.AreaIndex;
		}
		public bool IsFieldValueRetrievable(PivotGridFieldBase field) {
			if(!field.Visible) return false;
			if(field.Area == PivotArea.ColumnArea)
				return ColumnField != null ? field.AreaIndex <= GetFieldAreaIndex(ColumnField) : false;
			if(field.Area == PivotArea.RowArea)
				return RowField != null ? field.AreaIndex <= GetFieldAreaIndex(RowField) : false;
			return false;
		}
		protected bool IsValueType(PivotGridValueType valueType) {
			if(RowValueType == valueType) return true;
			if(ColumnValueType == valueType) return true;
			return false;
		}
		protected virtual string GetDisplayText() {
			if(Value == null) {
				return DataField != null ? DataField.EmptyCellText : string.Empty;
			}
			if(Value == PivotSummaryValue.ErrorValue)
				return PivotGridLocalizer.GetString(PivotGridStringId.CellError);
			FormatInfo formatInfo = GetCellFormatInfo();
			return formatInfo != null ? formatInfo.GetDisplayText(Value) : Value.ToString();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FormatInfo GetCellFormatInfo() {
			if(DataField == null) return null;
			FormatInfo cellFormat = DataField.CellFormat.IsEmpty ? null : DataField.CellFormat;
			FormatInfo totalCellFormat = DataField.TotalCellFormat.IsEmpty ? cellFormat : DataField.TotalCellFormat;
			if(CustomTotal != null)
				cellFormat = CustomTotal.GetCellFormat().IsEmpty ? totalCellFormat : CustomTotal.GetCellFormat();
			else {
				if(IsGrandTotalAppearance) {
					cellFormat = DataField.GrandTotalCellFormat.IsEmpty ? totalCellFormat : DataField.GrandTotalCellFormat;
				}
				if(cellFormat == null && IsTotalAppearance) {
					cellFormat = totalCellFormat;
				}
			}
			if(cellFormat == null || cellFormat.IsEmpty) {
				if(DataField.SummaryDisplayType == PivotSummaryDisplayType.PercentVariation ||
					DataField.SummaryDisplayType == PivotSummaryDisplayType.PercentOfColumn ||
					DataField.SummaryDisplayType == PivotSummaryDisplayType.PercentOfRow)
					cellFormat = PivotGridFieldBase.DefaultPercentFormat;
				else {
					if(DataField.SummaryType != PivotSummaryType.Count) {
						Type fieldType = Data.GetFieldType(DataField);
						if(fieldType == null && Value != null) fieldType = Value.GetType();
						if(fieldType == typeof(decimal))
							cellFormat = PivotGridFieldBase.DefaultDecimalFormat;
					}
				}
			}
			return cellFormat;
		}
		protected FormatType GetFormatType() {
			FormatInfo formatInfo = GetCellFormatInfo();
			return formatInfo != null ? formatInfo.FormatType : FormatType.None;
		}
	}
}
