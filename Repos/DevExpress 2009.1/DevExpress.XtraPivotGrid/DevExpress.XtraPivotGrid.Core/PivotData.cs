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
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Data;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using DevExpress.Utils.Design;
namespace DevExpress.Data.PivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSummaryType {Count = 0, Sum = 1, Min = 2, Max = 3, Average = 4, StdDev = 5, StdDevp = 6, Var = 7, Varp = 8, Custom = 9 };
	public enum PivotSummaryVariation { None, Absolute, Percent };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSummaryDisplayType { Default, AbsoluteVariation, PercentVariation, PercentOfColumn, PercentOfRow };
	public class PivotSummaryDisplayTypeConverter {
		static public PivotSummaryVariation DisplayTypeToVariation(PivotSummaryDisplayType displayType) {
			switch(displayType) {
				case PivotSummaryDisplayType.AbsoluteVariation:
					return PivotSummaryVariation.Absolute;
				case PivotSummaryDisplayType.PercentVariation:
					return PivotSummaryVariation.Percent;
				default:
					return PivotSummaryVariation.None;
			}
		}
		static public PivotSummaryDisplayType VariationToDisplayType(PivotSummaryVariation variation) {
			switch(variation) {
				case PivotSummaryVariation.Absolute:
					return PivotSummaryDisplayType.AbsoluteVariation;
				case PivotSummaryVariation.Percent:
					return PivotSummaryDisplayType.PercentVariation;
				default:
					return PivotSummaryDisplayType.Default;
			}
		}
		static public bool IsVariation(PivotSummaryDisplayType displayType) {
			return DisplayTypeToVariation(displayType) != PivotSummaryVariation.None;
		}
	}
	public class PivotSummaryValue {
		object tag;
		object min;
		object max;
		object customValue;
		decimal summary;
		double squareSummary;
		int count;
		bool isSummaryNull;
		ValueComparer valueComparer;
		bool compareError,
			summaryError;
		static object errorValue;
		public static object ErrorValue {
			get {
				if(errorValue == null)
					errorValue = new object();
				return errorValue;
			}
		}
		public PivotSummaryValue(ValueComparer valueComparer) {
			this.valueComparer = valueComparer;
			Clear();
		}
		public void Clear() {
			this.customValue = null;
			this.min = null;
			this.max = null;
			this.count = 0;
			this.summary = 0;
			this.squareSummary = 0;
			this.isSummaryNull = true;
			this.compareError = false;
			this.summaryError = false;
		}
		public void AddValue(object value, decimal numericValue) {
			if(value == null || value is System.DBNull) return;
			if(count == 0) {
				min = max = value;
			} else {
				try {
					if(!compareError) {
						if(valueComparer.Compare(max, value) < 0)
							max = value;
						if(valueComparer.Compare(min, value) > 0)
							min = value;
					}
				} catch(Exception) {
					compareError = true;
				}
			}
			count++;
			if(isSummaryNull)
				isSummaryNull = false;
			try {
				if(!summaryError)
					summary += numericValue;
			} catch(System.OverflowException) {
				summaryError = true;
			}
			squareSummary += ((double)numericValue * (double)numericValue);
		}
		public void AddValue(PivotSummaryValue summaryValue) {
			if(summaryValue == null || summaryValue.Count == 0) return;
			if(count == 0) {
				min = summaryValue.Min;
				max = summaryValue.Max;
			} else {
				try {
					if(!compareError) {
						if(valueComparer.Compare(max, summaryValue.Max) < 0)
							max = summaryValue.Max;
						if(valueComparer.Compare(min, summaryValue.Min) > 0)
							min = summaryValue.Min;
					}
				} catch(Exception) {
					compareError = true;
				}
			}
			count += summaryValue.Count;
			if(!summaryValue.IsSummaryNull) {
				isSummaryNull = false;
				summary += summaryValue.SummaryCore;
				squareSummary += summaryValue.SquareSummary;
			}
		}
		public object GetValue(PivotSummaryType summaryType) {
			switch(summaryType) {
				case PivotSummaryType.Average: return Average;
				case PivotSummaryType.Max: return Max;
				case PivotSummaryType.Min: return Min;
				case PivotSummaryType.Sum: return Summary;
				case PivotSummaryType.StdDev: return StdDev;
				case PivotSummaryType.StdDevp: return StdDevp;
				case PivotSummaryType.Var: return Var;
				case PivotSummaryType.Varp: return Varp;
				case PivotSummaryType.Custom: return CustomValue;
				case PivotSummaryType.Count: return Count;
			}
			return null;
		}
		public object GetCustomValue(PivotGridFieldBase field) {
			PivotGridCustomValues dic = CustomValue as PivotGridCustomValues;
			if(dic == null) return CustomValue;
			if(!dic.Contains(field)) return dic.Contains(null) ? dic[null] : null;
			return dic[field];
		}
		public int Count { get { return count; } }
		public object Min { get { return compareError ? ErrorValue : min; } }
		public object Max { get { return compareError ? ErrorValue : max; } }
		public object CustomValue { get { return customValue; } set { customValue = value; } }
		public object Tag { get { return tag; } set { tag = value; } }
		public object Summary { get { return summaryError ? ErrorValue : summary; } }
		public object Average {
			get {
				if(summaryError)
					return ErrorValue;
				if(IsSummaryNull)
					return null;
				else return summary / count;
			}
		}
		public object StdDev { 
			get { 
				if(IsStdDevIncorrect) 
					return null; 
				else return Math.Sqrt(VarCore); 
			} 
		}
		public object StdDevp { 
			get { 
				if(IsStdDevpIncorrect) 
					return null; 
				else return Math.Sqrt(VarpCore); 
			} 
		}
		public object Var { 
			get { 
				if(IsStdDevIncorrect) 
					return null; 
				else return VarCore; 
			} 
		}
		public object Varp { 
			get { 
				if(IsStdDevpIncorrect) 
					return null; 
				else return VarpCore; 
			} 
		}
		protected bool IsSummaryNull { get { return count == 0 || isSummaryNull || summaryError; } }
		protected bool IsStdDevIncorrect { get { return count < 2 || isSummaryNull || summaryError; } }
		protected bool IsStdDevpIncorrect { get { return count < 1 || isSummaryNull || summaryError; } }
		protected decimal SummaryCore { get { return summary; } }
		protected double SquareSummary { get { return squareSummary; } }
		protected double VarCore { get { return (squareSummary - ((double)summary * (double)summary)/ count) / (count - 1); }	}
		protected double VarpCore { get { return (squareSummary - ((double)summary * (double)summary)/ count) / count; } }
	}
	class PivotGridCustomValues {
		Dictionary<PivotGridFieldBase, object> customValues;
		object nullFieldValue;
		bool isNullAsigned;
		public PivotGridCustomValues() {
			customValues = new Dictionary<PivotGridFieldBase, object>();
			isNullAsigned = false;
		}
		public bool Contains(PivotGridFieldBase field) {
			return (field == null && isNullAsigned) || (field != null && customValues.ContainsKey(field));
		}
		public object this[PivotGridFieldBase field] {
			get {
				if(field == null) return nullFieldValue;
				return customValues[field];
			}
			set {
				if(field == null) {
					nullFieldValue = value;
					isNullAsigned = true;
				} else {
					if(customValues.ContainsKey(field))
						customValues[field] = value;
					else
						customValues.Add(field, value);
				}
			}
		}
	}
	public class PivotSortByCondition {
		DataColumnInfo column;
		object value;
		int level;
		public PivotSortByCondition(DataColumnInfo column, object value, int level) {
			if(column == null)
				throw new ArgumentNullException("column");
			if(level < 0)
				throw new ArgumentException("level must be greater that zero");
			this.column = column;
			this.value = value;
			this.level = level;
		}
		public DataColumnInfo Column { get { return column; } }
		public object Value { get { return value; } }
		public int Level { get { return level; } }
	}
	public class PivotSummaryItem : SummaryItemBase {
		readonly PivotSummaryType summaryType;
		public PivotSummaryItem(DataColumnInfo columnInfo, PivotSummaryType summaryType) : base(columnInfo) {
			this.summaryType = summaryType;
		}
		public override bool RequireValueConvert { 
			get { 
				if(ColumnInfo == null) return false;
				if(ColumnInfo.Type == typeof(DateTime)) 
					return false;
				return base.RequireValueConvert;
			}
		}
		public PivotSummaryType SummaryType { get { return summaryType; } }
	}
	public class PivotSummaryItemCollection : ColumnInfoNotificationCollection {
		public PivotSummaryItemCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public PivotSummaryItem this[int index] { get { return (PivotSummaryItem)List[index]; } }
		public bool Contains(PivotSummaryItem item) { return List.Contains(item); }
		public bool Contains(DataColumnInfo columnInfo) {
			for(int i = 0; i < Count; i ++)
				if(this[i].ColumnInfo == columnInfo)
					return true;
			return false;
		}
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
		public PivotSummaryItem GetItem(DataColumnInfo columnInfo) {
			for(int i = 0; i < Count; i ++)
				if(this[i].ColumnInfo == columnInfo)
					return this[i];
			return null;
		}
		public virtual PivotSummaryItem Add(PivotSummaryItem item) { 
			List.Add(item);
			return item;
		}
		public void ClearAndAddRange(PivotSummaryItemCollection collection) {
			ClearAndAddRange((PivotSummaryItem[])collection.InnerList.ToArray(typeof(PivotSummaryItem)));
		}
		public void ClearAndAddRange(PivotSummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				Clear();
				AddRange(summaryItems);
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(PivotSummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				foreach(PivotSummaryItem summaryItem in summaryItems) { List.Add(summaryItem); }
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnSummaryItemChanged(PivotSummaryItem item) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh,  item));
		}
	}
	public class PivotGroupRowKeeper {
		object[] values;
		public PivotGroupRowKeeper(object[] values) {
			this.values = values;
		}
		public object[] Values { get { return values; } }
		public int Level { get { return Values.Length - 1; } }
		public string RowHashCode { get { return GetRowHashCode(Level); }	}
		public string GetRowHashCode(int level) {
			StringBuilder sb = new StringBuilder();
			sb.Append(level);
			for(int i = 0; i <= level; i ++) {
				sb.Append('-');
				if(values[i] != null) {
					sb.Append(values[i]);
				}
			}
			return sb.ToString();
		}
	}
	public class PivotGroupRowKeeperCollection : CollectionBase {
		public PivotGroupRowKeeper this[int index] { get { return (PivotGroupRowKeeper)InnerList[index]; } }
		public void Add(PivotGroupRowKeeper rowKeeper) {
			InnerList.Add(rowKeeper);
		}
		public void Add(object[] values) {
			PivotGroupRowKeeper rowKeeper = new PivotGroupRowKeeper(values);
			InnerList.Add(rowKeeper);
		}
	}
	public class PivotGroupRowsKeeperColumnInfo {
		Type columnType,
			actualColumnType = null;
		string columnName;
		bool isRowsExpanded;
		public PivotGroupRowsKeeperColumnInfo() {
			Init(null, null, string.Empty);
		}
		public PivotGroupRowsKeeperColumnInfo(DataColumnInfo columnInfo) {
			Type actualColumnType = columnInfo.Type == typeof(object) ? IdentifyActualType(columnInfo) : columnInfo.Type;
			Init(columnInfo.Type, actualColumnType, columnInfo.Name);
		}
		protected void Init(Type columnType, Type actualColumnType, string columnName) {
			this.columnType = columnType;
			this.columnName = columnName;
			this.actualColumnType = actualColumnType;
			this.isRowsExpanded = true;
		}
		public Type ColumnType { get { return columnType; } }
		public Type ActualColumnType { get { return actualColumnType; } set { actualColumnType = value; } }
		public string ColumnName { get { return columnName; } }
		public bool IsRowsExpanded { get { return isRowsExpanded; }  set { isRowsExpanded = true; } }
		bool IsObjectType { get { return ColumnType == typeof(object); } }
		protected Type IdentifyActualType(DataColumnInfo columnInfo) {
			if(!columnInfo.StorageComparer.IsStorageEmpty) {
				for(int i = 0; i < columnInfo.StorageComparer.RecordCount; i++) {
					object val = columnInfo.StorageComparer.GetNullableRecordValue(i);
					if(object.ReferenceEquals(val, null))
						continue;
					return val.GetType();
				}
			}
			return null;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(ColumnName);
			writer.Write(ColumnType.AssemblyQualifiedName);
			if(IsObjectType)
				writer.Write(ActualColumnType.AssemblyQualifiedName);
			writer.Write(IsRowsExpanded);
		}
		public void Read(BinaryReader reader) {
			this.columnName = reader.ReadString();
			this.columnType = Type.GetType(reader.ReadString());
			if(IsObjectType)
				ActualColumnType = Type.GetType(reader.ReadString());
			this.isRowsExpanded = reader.ReadBoolean();
		}
	}
	public class PivotGroupRowsKeeper  {
		PivotDataControllerArea area;
		PivotGroupRowsKeeperColumnInfo[] savedColumns;
		PivotGroupRowKeeperCollection rows;
		Hashtable foundRows;
		public PivotGroupRowsKeeper(PivotDataControllerArea area) {
			this.area = area;
			this.rows = new PivotGroupRowKeeperCollection();
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[] {};
			this.foundRows = new Hashtable();
		}
		public PivotDataControllerArea Area { get { return area; } }
		protected GroupRowInfoCollection GroupInfo { get { return Area.GroupInfo; } }
		public PivotGroupRowKeeperCollection Rows { get { return rows; } }
		public void Clear() {
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[] {};
			Rows.Clear();
			foundRows.Clear();
		}
		public void Restore() {
			int restoredLevel = GetRestoredLevel();
			RestoreRows(restoredLevel);
		}
		public void WebWriteToStream(Stream stream) {
			List<bool> expandValues = WebSaveExpandValues();
			WebWriteExpandValues(stream, expandValues);
		}
		List<bool> WebSaveExpandValues() {
			int lastLevel = GetRestoredLevel();
			List<bool> expandValues = new List<bool>(GroupInfo.Count);
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level >= lastLevel) continue;
				expandValues.Add(groupRow.Expanded);
			}
			return expandValues;
		}
		void WebWriteExpandValues(Stream stream, List<bool> expandValues) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(expandValues.Count);
			byte c = 0;
			for(int i = 0; i < expandValues.Count; i++) {
				c |= (byte)((expandValues[i] ? 1 : 0) << (i % 8));
				if(i % 8 == 7) {
					writer.Write(c);
					c = 0;
				}
			}
			if(expandValues.Count % 8 != 0) writer.Write(c);
		}
		public void WebReadFromStream(Stream stream) {
			List<bool> expandValues = WebReadExpandValues(stream);
			RestoreExpandValues(expandValues);
		}
		List<bool> WebReadExpandValues(Stream stream) {
			BinaryReader reader = new BinaryReader(stream);
			int count = reader.ReadInt32();
			List<bool> expandValues = new List<bool>(count);
			byte c = 0;
			for(int i = 0; i < count; i++) {
				if(i % 8 == 0) c = reader.ReadByte();
				expandValues.Add((c & 1) == 1);
				c >>= 1;
			}
			return expandValues;
		}
		void RestoreExpandValues(List<bool> expandValues) {
			int lastLevel = GetRestoredLevel();
			for(int i = 0, j = 0; i < GroupInfo.Count && j < expandValues.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level >= lastLevel) continue;
				groupRow.Expanded = expandValues[j++];
			}
		}
		public void WriteToStream(Stream stream) {
			SaveColumns();
			SaveRows();
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			WriteColumns(writer);
			WriteRows(writer);
			writer.Flush();
		}
		public void ReadFromStream(Stream stream) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			ReadColumns(reader);
			ReadRows(reader);
		}
		protected void WriteColumns(TypedBinaryWriter writer) {
			writer.Write(SavedColumns.Length);
			for(int i = 0; i < SavedColumns.Length; i ++) {
				SavedColumns[i].Write(writer);
			}
		}
		protected void ReadColumns(TypedBinaryReader reader) {
			int count = reader.ReadInt32();
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[count];
			for(int i = 0; i < this.savedColumns.Length; i ++) {
				SavedColumns[i] =  new PivotGroupRowsKeeperColumnInfo();
				SavedColumns[i].Read(reader);
			}
		}
		protected void WriteRows(TypedBinaryWriter writer) {
			writer.Write(Rows.Count);
			for(int i = 0; i < Rows.Count; i ++) {
				WriteRow(writer, Rows[i]);
			}
		}
		protected void ReadRows(TypedBinaryReader reader) {
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i ++) {
				ReadRow(reader);
			}
		}
		protected void WriteRow(TypedBinaryWriter writer, PivotGroupRowKeeper rowKeeper) {
			writer.Write(rowKeeper.Values.Length);
			for(int i = 0; i < rowKeeper.Values.Length; i++) {
				writer.WriteObject(rowKeeper.Values[i]);
			}
		}
		protected void ReadRow(TypedBinaryReader reader) {
			int count = reader.ReadInt32();
			object[] values = new object[count];
			for(int i = 0; i < values.Length; i ++) {
				PivotGroupRowsKeeperColumnInfo column = SavedColumns[i];
				values[i] = reader.ReadObject(column.ColumnType == typeof(object) ? column.ActualColumnType : column.ColumnType);
			}
			Rows.Add(values);
		}
		public int GetRestoredLevel() {
			int count = Math.Min(Area.Columns.Count - 1, savedColumns.Length);
			for(int i = 0; i < count; i ++)
				if(!IsColumnEquals(SavedColumns[i], Area.Columns[i].ColumnInfo))
					return i;
			return count;
		}
		protected bool IsColumnEquals(PivotGroupRowsKeeperColumnInfo keeperColumnInfo, DataColumnInfo columnInfo) {
			return keeperColumnInfo.ColumnType == columnInfo.Type && keeperColumnInfo.ColumnName == columnInfo.Name;
		}
		protected PivotGroupRowsKeeperColumnInfo[] SavedColumns { get { return savedColumns; } }
		protected object GetValue(GroupRowInfo groupRow) {
			return Area.Controller.GetRowValue(Area.GetListSourceRowByControllerRow(groupRow.ChildControllerRow), Area.Columns[groupRow.Level].ColumnInfo);
		}
		public void SaveColumns() {
			Clear();
			int count = Area.Columns.Count - 1;
			if(count > 1) {
				if(GroupInfo.LastExpandableLevel >= 0 && GroupInfo.LastExpandableLevel < count)
					count = GroupInfo.LastExpandableLevel;
			}
			if(count < 0) 
				count = 0;
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[count];
			for(int i = 0; i < SavedColumns.Length; i ++) {
				SavedColumns[i] = new PivotGroupRowsKeeperColumnInfo(Area.Columns[i].ColumnInfo);
			}
		}
		protected void SaveDefaultExpandedCollasped() {
			int[] expandedCount = new int[SavedColumns.Length];
			for(int i = 0; i < expandedCount.Length; i ++)
				expandedCount[i] = 0;
			for(int i = 0; i < GroupInfo.Count; i ++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level >= expandedCount.Length) continue;
				if(groupRow.Expanded)
					expandedCount[groupRow.Level] ++;
				else expandedCount[groupRow.Level] --;
			}
			for(int i = 0; i < expandedCount.Length; i ++)
				this.savedColumns[i].IsRowsExpanded = expandedCount[i] >= 0;
		}
		public void SaveRows() {
			Rows.Clear();
			SaveDefaultExpandedCollasped();
			int lastLevel = GetRestoredLevel();
			for(int i = 0; i < GroupInfo.Count; i ++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level >= lastLevel) continue;
				if(groupRow.Expanded != this.savedColumns[groupRow.Level].IsRowsExpanded) {
					SaveRow(groupRow);
				}
			}
		}
		protected void SaveRow(GroupRowInfo groupRow) {
			object[] values = new object[groupRow.Level + 1];
			while(groupRow != null) {
				values[groupRow.Level] = GetValue(groupRow);
				groupRow = groupRow.ParentGroup;
			}
			Rows.Add(values);
		}
		protected void RestoreRows(int restoredLevel) {
			if(restoredLevel == 0) return;
			RestoreDefaultExpandedCollapsed(restoredLevel);
			for(int i = 0; i < Rows.Count; i ++) {
				if(Rows[i].Level < restoredLevel) {
					GroupRowInfo groupRow = FindRestoredRow(Rows[i]);
					if(groupRow != null) {
						groupRow.Expanded = !SavedColumns[groupRow.Level].IsRowsExpanded;
					}
				}
			}
		}
		protected void RestoreDefaultExpandedCollapsed(int restoredLevel) {
			for(int i = 0; i < GroupInfo.Count; i ++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level < restoredLevel) {
					groupRow.Expanded = this.savedColumns[groupRow.Level].IsRowsExpanded;
				}
			}
		}
		protected GroupRowInfo FindRestoredRow(PivotGroupRowKeeper rowKeeper) {
			GroupRowInfo groupRow = FindRestoredRow(rowKeeper, rowKeeper.Level);
			if(groupRow == null) return null;
			while(groupRow != null && groupRow.Level < rowKeeper.Level) {
				groupRow = FindRestoredRow(groupRow, rowKeeper);
			}
			return groupRow;
		}
		protected GroupRowInfo FindRestoredRow(PivotGroupRowKeeper rowKeeper, int level) {
			GroupRowInfo groupRow = foundRows[rowKeeper.GetRowHashCode(level)] as GroupRowInfo;
			if(groupRow != null) return groupRow;
			if(level > 0) 
				return FindRestoredRow(rowKeeper, level - 1);
			for(int i = 0; i < GroupInfo.Count; i ++) {
				if(GroupInfo[i].Level != 0) continue;
				if(IsEqual(GroupInfo[i], rowKeeper,  0)) {
					return GroupInfo[i];
				}
			}
			return null;
		}
		protected GroupRowInfo FindRestoredRow(GroupRowInfo parentGroupRow, PivotGroupRowKeeper rowKeeper) {
			for(int i = parentGroupRow.Index + 1; i < GroupInfo.Count; i ++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level <= parentGroupRow.Level) 
					break;
				if(groupRow.Level > parentGroupRow.Level + 1) continue;
				if(IsEqual(groupRow, rowKeeper, parentGroupRow.Level + 1))
					return groupRow;
			}
			return null;
		}
		protected bool IsEqual(GroupRowInfo groupRow, PivotGroupRowKeeper rowKeeper, int level) {
			if(IsEqual(groupRow, rowKeeper.Values[level])) {
				foundRows[rowKeeper.GetRowHashCode(level)] = groupRow;
				return true;
			}
			return false;
		}
		protected bool IsEqual(GroupRowInfo groupRow, object value) {
			return Comparer.Default.Compare(GetValue(groupRow), value) == 0;
		}
	}
	public class NullableHashtable {
		static object NullKey = new object();
		Hashtable innerHashtable;		
		public NullableHashtable() : this(0, null) {
		}
		public NullableHashtable(int capacity) : this(capacity, null) {
		}
		public NullableHashtable(int capacity, IEqualityComparer comparer) {
			this.innerHashtable = new Hashtable(capacity, comparer);
		}
		protected Hashtable InnerHashtable {
			get { return innerHashtable; }
		}
		public object this[object key] {
			get {
				if(key == null)
					key = NullKey;
				return InnerHashtable[key];
			}
			set {
				if(key == null)
					key = NullKey;
				InnerHashtable[key] = value;
			}
		}
		public int Count {
			get { return InnerHashtable.Count; }
		}
		public bool ContainsKey(object key) {
			if(key == null)
				key = NullKey;
			return InnerHashtable.ContainsKey(key);
		}
		public bool Contains(object key) {			
			return ContainsKey(key);
		}
		public void Add(object key, object value) {
			if(key == null)
				key = NullKey;
			InnerHashtable.Add(key, value);
		}
		public void Remove(object key) {
			if(key == null)
				key = NullKey;
			InnerHashtable.Remove(key);
		}
		public void Clear() {
			InnerHashtable.Clear();
		}
		public void CopyKeysTo(Array array, int index) {			
			object[] tempArray = new object[Count];
			InnerHashtable.Keys.CopyTo(tempArray, 0);
			int tempIndex = tempArray.Length,
				arrayIndex = index;
			while(--tempIndex >= 0) {
				object key = tempArray[tempIndex];
				if(key == NullKey)
					key = null;
				array.SetValue(key, arrayIndex++);
			}
		}
	}
}
