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
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.Data {	
	public abstract class PivotGridFieldValueItemsDataProviderBase {
		bool isColumn;
		PivotGridData data;
		public PivotGridFieldValueItemsDataProviderBase(PivotGridData data, bool isColumn) {
			this.data = data;
			this.isColumn = isColumn;
		}
		public PivotGridData Data { get { return data; } }
		public bool IsColumn { get { return isColumn; } }
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public abstract int LevelCount { get; }
		public abstract int CellCount { get; }
		public abstract int GetObjectLevel(int visibleIndex);
		public abstract object GetObjectValue(int visibleIndex);
		public abstract bool IsObjectCollapsed(int visibleIndex);
		public abstract bool IsOthersValue(int visibleIndex);
		public virtual void Reset() { }
		protected abstract Type GetFieldColumnType(int level);
		public bool IsObjectVisible(int visibleIndex, int level, bool isObjectCollapsed) {
			if(level + 1 == LevelCount)
				return true;
			if(isObjectCollapsed) return true;
			bool singleValue = GetLevelValueCount(level + 1, visibleIndex + 1) == 1;
			Data.EnsureFieldCollections();
			return Data.GetFieldByArea(Area, level).GetTotalSummaryCount(singleValue) > 0;
		}
		public int GetLevelValueCount(int level, int visibleIndex) {
			int count = CellCount;
			int cellCount = 0;
			for(int i = visibleIndex; i < count; i ++) {
				int objectLevel = GetObjectLevel(i);
				if(objectLevel == level) cellCount ++;
				if(objectLevel < level)
					break;
			}
			return cellCount;
		}
		public virtual void SaveToStream(Stream stream) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			writer.Write(LevelCount);
			for(int i = 0; i < LevelCount; i ++) {
				writer.WriteType(GetFieldColumnType(i));
			}
			int count = CellCount;
			writer.Write(count);
			for(int i = 0; i < count; i ++) {
				byte byteValue = (byte)GetObjectLevel(i);
				byteValue = (byte)(byteValue << 2);
				if (IsObjectCollapsed(i))
					byteValue += 1;
				if (IsOthersValue(i))
					byteValue += 2;
				writer.Write(byteValue);
				writer.WriteObject(GetObjectValue(i));
			}
		}
	}
	public class PivotGridFieldValueItemsDataProvider : PivotGridFieldValueItemsDataProviderBase {
		int levelCount;
		int cellCount;
		public PivotGridFieldValueItemsDataProvider(PivotGridData data, bool isColumn) : base(data, isColumn) {
			CalcCellAndLevelCount();
		}
		public override int LevelCount { get { return levelCount; } }
		public override int CellCount { get { return cellCount; } }
		public override object GetObjectValue(int visibleIndex) {
			return Data.GetObjectValue(IsColumn, visibleIndex);
		}
		public override int GetObjectLevel(int visibleIndex) { 
			return Data.GetObjectLevel(IsColumn, visibleIndex); 
		}
		public override bool IsObjectCollapsed(int visibleIndex) {
			return Data.IsObjectCollapsed(IsColumn, visibleIndex);
		}
		public override bool IsOthersValue(int visibleIndex) {
			return Data.GetIsOthersValue(IsColumn, visibleIndex);
		}
		protected override Type GetFieldColumnType(int level) {
			Type res = Data.GetFieldTypeByArea(Area, level);
			if(res == typeof(object) && Data.GetCellCount(IsColumn) > 0) {
				object value = GetObjectValueByLevel(level);
				if(value != null)
					res = value.GetType();
			}
			return res;
		}
		protected object GetObjectValueByLevel(int level) {
			int count = Data.GetCellCount(IsColumn);
			for(int i = 0; i < count; i++) {
				if(GetObjectLevel(i) == level)
					return GetObjectValue(i);
			}
			return null;
		}
		public override void Reset() {
			CalcCellAndLevelCount();
		}
		protected void CalcCellAndLevelCount() {
			this.levelCount = Data.GetLevelCount(IsColumn);
			this.cellCount = Data.GetCellCount(IsColumn);
		}
	}
	public class PivotGridFieldValueItemsStreamDataProviderItem {
		public byte Level;
		public object Value;
		public bool Collapsed;
		public bool IsOthersValue;
	}
	public class PivotGridFieldValueItemsStreamDataProvider : PivotGridFieldValueItemsDataProviderBase {
		ArrayList list;
		int levelCount;
		Type[] fieldTypes;
 		public PivotGridFieldValueItemsStreamDataProvider(PivotGridData data, bool isColumn) : base(data, isColumn) {
			this.list = new ArrayList();
			this.levelCount = 0;
			this.fieldTypes = new Type[0];
		}
		protected PivotGridFieldValueItemsStreamDataProviderItem GetItem(int index) {
			if (index < 0 || index >= list.Count) return null;
			return list[index] as PivotGridFieldValueItemsStreamDataProviderItem;  
		}
		public override int LevelCount { get { return levelCount; } }
		public override int CellCount { get { return list.Count; } }
		public override object GetObjectValue(int visibleIndex) {
			return GetItem(visibleIndex) != null ? GetItem(visibleIndex).Value : null;
		}
		public override int GetObjectLevel(int visibleIndex) { 
			return GetItem(visibleIndex) != null ?  (int)GetItem(visibleIndex).Level : -1;
		}
		public override bool IsObjectCollapsed(int visibleIndex) {
			return GetItem(visibleIndex) != null ? GetItem(visibleIndex).Collapsed : false;
		}
		public override bool IsOthersValue(int visibleIndex) {
			return GetItem(visibleIndex) != null ? GetItem(visibleIndex).IsOthersValue : false;
		}
		protected override Type GetFieldColumnType(int level) {
			return fieldTypes[level];
		}
		public void LoadFromStream(Stream stream) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			levelCount = reader.ReadInt32();
			fieldTypes = new Type[levelCount];
			for(int i = 0; i < levelCount; i ++) {
				fieldTypes[i] = reader.ReadType();
			}
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i ++) {
				PivotGridFieldValueItemsStreamDataProviderItem item = new PivotGridFieldValueItemsStreamDataProviderItem();
				byte byteValue = reader.ReadByte();
				item.Collapsed = (byteValue & 1) == 1;
				item.IsOthersValue = (byteValue & 2) == 2;
				item.Level = (byte)(byteValue >> 2);
				item.Value = reader.ReadObject(fieldTypes[item.Level]);
				list.Add(item);
			}
		}
	}
	public class PivotFieldValueItemBase {
		PivotGridFieldValueItemsDataProviderBase dataProvider;
		public PivotFieldValueItemBase(PivotGridFieldValueItemsDataProviderBase dataProvider) {
			this.dataProvider = dataProvider;
		}
		protected PivotGridFieldValueItemsDataProviderBase DataProvider { get { return dataProvider; } set { dataProvider = value; } }
		public PivotGridData Data { get { return dataProvider.Data; } }
		public bool IsColumn { get { return dataProvider.IsColumn; } }
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public PivotArea CrossArea { get { return IsColumn ? PivotArea.RowArea : PivotArea.ColumnArea; } }
		protected PivotGridOptionsDataField OptionsDataField { get { return Data.OptionsDataField; } }
		public int LevelCount { 
			get {
				int levelCount = DataProvider.LevelCount;
				if(levelCount == 0) levelCount ++;
				if(IsDataFieldsVisible)
					levelCount ++;
				return levelCount;
			}
		}
		protected int LastFieldLevel {
			get { 
				return GetCorrectedFieldValueLevel(DataProvider.LevelCount - 1); 
			}
		}
		protected int GetCorrectedFieldValueLevel(int level) {
			if (IsLevelAfterDataField(level))
				level++;
			return level;
		}
		protected bool IsLevelBeforeDataField(int level) {
			return IsDataFieldsVisible && OptionsDataField.DataFieldAreaIndex > level;
		}
		public bool IsLevelAfterDataField(int level) {
			return IsDataFieldsVisible && OptionsDataField.DataFieldAreaIndex <= level;
		}
		public bool IsDataFieldsVisible { 
			get {
				return Data.GetIsDataFieldsVisible(IsColumn);
			} 
		}
		public bool IsDataLocatedInThisArea { get { return OptionsDataField.DataFieldArea == Area; } } 
		public int DataLevel { 
			get {
				int index = OptionsDataField.DataFieldAreaIndex; 
				if(index == 0 && DataProvider.LevelCount == 0)
					index ++;
				return index;
			} 
		}
		protected string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
	}
	public class PivotFieldValueItemsCreator : PivotFieldValueItemBase {
		List<PivotFieldValueItem> items;
		List<PivotFieldValueItem> lastLevelItems;
		List<PivotFieldValueItem> unpagedItems;
		public PivotFieldValueItemsCreator(PivotGridData data, bool isColumn) : base(new PivotGridFieldValueItemsDataProvider(data, isColumn))  {
			this.items = new List<PivotFieldValueItem>();
			this.lastLevelItems = new List<PivotFieldValueItem>();
			this.unpagedItems = new List<PivotFieldValueItem>();
		}
		public int Count { get { return items.Count; } }
		public PivotFieldValueItem this[int index] { get { return items[index]; } }
		public PivotFieldValueItem GetUnpagedItem(int uniqueIndex) { return unpagedItems[uniqueIndex];  }
		public int UnpagedItemsCount { get { return unpagedItems.Count; } }
		public int LastLevelItemCount { get { return lastLevelItems.Count; } }
		public PivotFieldValueItem GetLastLevelItem(int index) { return lastLevelItems[index] as PivotFieldValueItem; }
		public void Clear() {
			this.items.Clear();
			this.lastLevelItems.Clear();
			this.unpagedItems.Clear();
		}
		public void CreateItems() {
			CreateItems(0, 0);
		}
		public void CreateItems(int startValueItem, int maxFieldValueItemCount) {
			Clear();
			DataProvider.Reset();
			int fieldCount = DataProvider.CellCount;
			int lastLevel = DataProvider.LevelCount - 1;
			AddGrandTotals(PivotTotalsLocation.Near);
			int valuesDataIndex = GetDataIndex(PivotGridValueType.Value);
			if(fieldCount > 0 && lastLevel >= 0) {
				int[] summaries = CreateSummariesArray(lastLevel);
				int fieldIndex = 0;
				AddSummaries(summaries, 0, PivotTotalsLocation.Near);
				while(fieldIndex < fieldCount) {
					int level = DataProvider.GetObjectLevel(fieldIndex);
					if(IsDataFieldsVisible && level == DataLevel) {
						fieldIndex = AddDataFieldsAndFieldsBelow(level, fieldIndex, lastLevel, fieldCount, summaries);
					} else {
						PivotFieldCellValueItem cellValueItem = AddFieldValue(fieldIndex, level, lastLevel, summaries, valuesDataIndex);
						if(level == lastLevel || cellValueItem.IsCollapsed) {
							if(IsDataLocatedInThisArea && Data.DataFieldCount >= 2 && level < Data.OptionsDataField.DataFieldAreaIndex &&
								!ShouldAddDataCells(PivotGridValueType.Value)) {
								cellValueItem.EndLevel = Data.OptionsDataField.DataFieldAreaIndex;
							}
							AddDataCells(level, fieldIndex, PivotGridValueType.Value);
						}
						fieldIndex++;
					}
				}
				AddSummaries(summaries, 0, PivotTotalsLocation.Far);
			}
			AddGrandTotals(PivotTotalsLocation.Far);
			SetIndexes();
			for(int i = 0; i < items.Count; i++) 
				items[i].UniqueIndex = i;
			SetLastLevelIndexes();
			unpagedItems.AddRange(items);
			ApplyPaging(startValueItem, maxFieldValueItemCount);
			SetIndexes();
			SetChildren();
			CreateLastLevelItems();
		}
		int GetDataIndex(PivotGridValueType valueType) {
			for(int i = 0; i < Data.DataFieldCount; i++) {
				if(CanAddDataCell(i, valueType))
					return i;
			}
			return 0;
		}
		void ApplyPaging(int startValueItem, int maxFieldValueItemCount) {
			if(maxFieldValueItemCount <= 0) return;
			int lastLevelItemsCount = GetLastLevelItemsCount();
			if(maxFieldValueItemCount > lastLevelItemsCount) return;
			if(startValueItem < lastLevelItemsCount &&
				maxFieldValueItemCount > lastLevelItemsCount - startValueItem) {
				maxFieldValueItemCount = lastLevelItemsCount - startValueItem;
			}
			bool leftCutted = startValueItem > 0,
				rightCutted = startValueItem + maxFieldValueItemCount < lastLevelItemsCount;
			List<PivotFieldValueItem> newItems = new List<PivotFieldValueItem>(maxFieldValueItemCount),
				grandTotals = new List<PivotFieldValueItem>(),
				parents = new List<PivotFieldValueItem>(),
				lastLevelItems = new List<PivotFieldValueItem>(lastLevelItemsCount);
			SplitItems(lastLevelItems, grandTotals, parents);
			if(startValueItem < lastLevelItems.Count) {
				lastLevelItems = lastLevelItems.GetRange(startValueItem, 
					Math.Min(lastLevelItems.Count - startValueItem, maxFieldValueItemCount));
				AddWithParents(newItems, lastLevelItems, parents);
			}
			if(this[0].ValueType == PivotGridValueType.GrandTotal)
				newItems.InsertRange(0, grandTotals);
			else
				newItems.AddRange(grandTotals);
			items.Clear();			
			items.AddRange(newItems);
		}
		private void SplitItems(List<PivotFieldValueItem> lastLevelItems, List<PivotFieldValueItem> grandTotals, List<PivotFieldValueItem> parents) {
			for(int i = 0; i < Count; i++) {
				switch(this[i].ValueType) {
					case PivotGridValueType.GrandTotal:
						grandTotals.Add(this[i]);
						break;
					case PivotGridValueType.Total:
					case PivotGridValueType.CustomTotal:
					case PivotGridValueType.Value:
						if(this[i].IsLastFieldLevel)
							lastLevelItems.Add(this[i]);
						else
							parents.Add(this[i]);
						break;
				}
			}
		}
		private void AddWithParents(List<PivotFieldValueItem> newItems, List<PivotFieldValueItem> lastLevelItems, List<PivotFieldValueItem> parents) {
			for(int i = 0; i < lastLevelItems.Count; i++) {
				int index = newItems.Count;
				PivotFieldValueItem parent = GetParentItem(lastLevelItems[i]);
				while(parent != null && newItems.IndexOf(parent) == -1) {
					newItems.Insert(index, parent);
					parent = GetParentItem(parent);
				}
				newItems.Add(lastLevelItems[i]);
			}
		}
		private int GetLastLevelItemsCount() {
			int counter = 0;
			for(int i = 0; i < Count; i++) {
				counter += this[i].IsLastFieldLevel && this[i].ValueType != PivotGridValueType.GrandTotal ? 1 : 0;
			}
			return counter;
		}
		protected void SetIndexes() {
			for(int i = 0; i < Count; i++) {
				this[i].SetIndex(i);
			}
		}
		protected void SetLastLevelIndexes() {
			int lastLevelIndex;
			for(int curLevel = LevelCount - 1; curLevel >= 0; curLevel--) {
				PivotFieldValueItem lastItem = null;
				lastLevelIndex = -1;
				for(int i = 0; i < Count; i++) {
					if(this[i].ContainsLevel(curLevel) && curLevel != LevelCount - 1) {
						if(lastItem != null)
							lastItem.MaxLastLevelIndex = lastLevelIndex;
						if(this[i].IsLastFieldLevel)
							lastItem = null;
						else {
							this[i].MinLastLevelIndex = lastLevelIndex + 1;
							lastItem = this[i];
						}
					}
					if(this[i].IsLastFieldLevel) {
						lastLevelIndex++;
						if(curLevel == LevelCount - 1)
							this[i].MaxLastLevelIndex = this[i].MinLastLevelIndex = lastLevelIndex;
					}
				}
				if(lastItem != null)
					lastItem.MaxLastLevelIndex = lastLevelIndex;
			}
		}
		void SetChildren() {
			for(int i = 0; i < Count; i++) {
				AddToParent(this[i]);
			}
		}
		void AddToParent(PivotFieldValueItem item) {
			PivotFieldValueItem parent = GetParentItem(item);
			if(parent != null) {
				parent.AddCell(item);
			}
		}
		public int GetItemChildAndTotalsCount(PivotFieldValueItem item) {
			int startIndex = this.items.IndexOf(item);
			if(startIndex < 0) return 0;
			int count = 0;
			for(int i = startIndex; i < Count; i ++) {
				if(this[i].StartLevel <= item.StartLevel && this[i].VisibleIndex != item.VisibleIndex)
					break;
				count++;
			}
			return count;
		}
		public object[] GetItemValues(int uniqueIndex) {
			if(uniqueIndex < 0 || uniqueIndex >= UnpagedItemsCount) return null;
			return GetItemValues(GetUnpagedItem(uniqueIndex));
		}
		public object[] GetItemValues(PivotFieldValueItem item) {
			if(item == null) return null;
			PivotFieldValueItem[] items = new PivotFieldValueItem[item.LevelCount];
			while(item != null) {
				items[item.StartLevel] = item;
				item = GetParentItem(item);
			}
			object[] values = new object[GetNotNullObjects(items)];
			int index = 0;
			for(int i = 0; i < items.Length; i ++) {
				if(items[i] == null) continue;
				values[index++] = items[i].Value;
			}
			return values;
		}
		public PivotFieldValueItem GetRootParentItem(PivotFieldValueItem item) {
			return GetParentItemCore(item, 0);
		}
		public PivotFieldValueItem GetParentItem(PivotFieldValueItem item) {
			return GetParentItemCore(item, item.StartLevel - 1);
		}
		PivotFieldValueItem GetParentItemCore(PivotFieldValueItem item, int prevLevel) {
			if(item == null || item.StartLevel == 0) return null;
			for(int i = item.Index; i >= 0; i--) {
				if(this[i].ContainsLevel(prevLevel)) return this[i];
			}
			return null;
		}
		int GetNotNullObjects(PivotFieldValueItem[] items) {
			int count = 0;
			for(int i = 0; i < items.Length; i ++) {
				if(items[i] != null) count ++;
			}
			return count;
		}
		public void SaveToStream(Stream stream) {
			DataProvider.SaveToStream(stream);
		}
		public void LoadFromStream(Stream stream) {
			Clear();
			DataProvider = new PivotGridFieldValueItemsStreamDataProvider(Data, IsColumn);
			(DataProvider as PivotGridFieldValueItemsStreamDataProvider).LoadFromStream(stream);
		}
		public void ResetDataProvider() {
			DataProvider = new PivotGridFieldValueItemsDataProvider(Data, IsColumn);
		}
		public PivotGridFieldBase GetFieldByLevel(int level) {
			if(level < 0 || level >= LevelCount) return null;
			return Data.GetFieldByArea(Area, level);
		}
		protected bool ShowGrandTotals { 
			get { 
				bool show = IsColumn ? Data.OptionsView.ShowColumnGrandTotals : Data.OptionsView.ShowRowGrandTotals; 
				show = show && (Data.OptionsView.ShowGrandTotalsForSingleValues || DataProvider.GetLevelValueCount(0, 0) > 1);
				show = show && Data.HasNonVariationSummary;
				return show;
			} 
		}
		protected virtual void AddChildCell(PivotFieldValueItem valueItem) {
			this.items.Add(valueItem);
		}
		int AddDataFieldsAndFieldsBelow(int level, int fieldIndex, int lastLevel, int fieldCount, int[] summaries) {
			int dataFieldIndex = fieldIndex;
			for(int dataIndex = 0; dataIndex < Data.DataFieldCount; dataIndex ++) {
				AddChildCell(new PivotFieldTopDataCellValueItem(DataProvider, fieldIndex, dataIndex));
				dataFieldIndex = fieldIndex;
				int dataLevel = level;
				AddSummaries(summaries, level, dataIndex, PivotTotalsLocation.Near); 
				while(dataFieldIndex < fieldCount && dataLevel >= level) {
					AddFieldValue(dataFieldIndex, dataLevel, lastLevel, summaries, dataIndex);
					dataFieldIndex ++;
					dataLevel = DataProvider.GetObjectLevel(dataFieldIndex);
				}
				AddSummaries(summaries, level, dataIndex, PivotTotalsLocation.Far); 
			}
			return dataFieldIndex;
		}
		PivotFieldCellValueItem AddFieldValue(int fieldIndex, int level, int lastLevel, int[] summaries, int dataIndex) {
			bool isObjectCollapsed = DataProvider.IsObjectCollapsed(fieldIndex);
			SetSummariesLevel(fieldIndex, level, lastLevel, summaries, isObjectCollapsed, PivotTotalsLocation.Near);
			if(level < lastLevel) {
				AddSummaries(summaries, level, Data.OptionsView.TotalsLocation);
			}
			PivotFieldCellValueItem item = new PivotFieldCellValueItem(DataProvider, fieldIndex, isObjectCollapsed, dataIndex);
			if(!IsValuesEmpty() || !IsDataLocatedInThisArea || Data.DataFieldCount == 0) AddChildCell(item);
			SetSummariesLevel(fieldIndex, level, lastLevel, summaries, isObjectCollapsed, PivotTotalsLocation.Far);
			return item;
		}
		void SetSummariesLevel(int fieldIndex, int level, int lastLevel, int[] summaries, bool isObjectCollapsed, PivotTotalsLocation totalsLocation) {
			if(Data.OptionsView.TotalsLocation != totalsLocation) return;
			if(level < lastLevel && DataProvider.IsObjectVisible(fieldIndex, level, isObjectCollapsed) && !isObjectCollapsed)
				summaries[level] = fieldIndex;
		}
		void AddSummaries(int[] summaries, int level, PivotTotalsLocation totalsLocation) {
			AddSummaries(summaries, level, GetDataIndex(PivotGridValueType.Total), totalsLocation);
		}
		void AddSummaries(int[] summaries, int level, int dataIndex, PivotTotalsLocation totalsLocation) {
			if(Data.OptionsView.TotalsLocation != totalsLocation) return;
			for(int i = summaries.Length - 1; i >= level; i --) {
				if(summaries[i] > -1) {
					bool isSingleValue = DataProvider.GetLevelValueCount(i + 1, summaries[i] + 1) <= 1;
					if(Data.GetFieldByArea(Area, i).GetTotalSummaryCount(isSingleValue) > 0) {
						if(Data.GetFieldByArea(Area, i).TotalsVisibility == PivotTotalsVisibility.CustomTotals) {
							AddCustomSummaries(Data.GetFieldByArea(Area, i), summaries[i], level, dataIndex);
						}
						else {
							if(CanAddDataCell(PivotGridValueType.Total) || Data.DataFieldCount == 0) {
								PivotFieldTotalCellValueItem totalCell = new PivotFieldTotalCellValueItem(DataProvider, summaries[i], dataIndex);
								AddChildCell(totalCell);
								if(Data.GetIsDataFieldsVisible(IsColumn) && !ShouldAddDataCells(PivotGridValueType.Total)
									&& level < Data.OptionsDataField.DataFieldAreaIndex) {
									totalCell.EndLevel++;
								}
								AddDataCells(level, summaries[i], PivotGridValueType.Total);
							}
						}
					}
					summaries[i] = -1;
				}
			}
		}
		void AddCustomSummaries(PivotGridFieldBase field, int visibleIndex, int level, int dataIndex) {
			if(!field.Options.ShowCustomTotals || !CanAddDataCell(PivotGridValueType.CustomTotal)) return;
			for(int i = 0; i < field.CustomTotals.Count; i ++) {
				AddChildCell(new PivotFieldCustomTotalCellValueItem(DataProvider, visibleIndex, field.CustomTotals[i], dataIndex, i == 0));
				AddCustomTotalsDataCells(level, visibleIndex, field.CustomTotals[i]);
			}
		}
		void AddCustomTotalsDataCells(int level, int index, PivotGridCustomTotalBase customTotal) {
			if(!Data.GetIsDataFieldsVisible(IsColumn) || level >= Data.OptionsDataField.DataFieldAreaIndex) return;
			for(int i = 0; i < Data.DataFieldCount; i++) {
				if(CanAddDataCell(i, PivotGridValueType.CustomTotal)) {
					AddChildCell(new PivotFieldCustomTotalDataCellValueItem(DataProvider, index, customTotal, i));
				}
			}
		}
		void AddDataCells(int level, int index, PivotGridValueType valueType) {
			if(!Data.GetIsDataFieldsVisible(IsColumn) || level >= Data.OptionsDataField.DataFieldAreaIndex 
				|| !ShouldAddDataCells(valueType)) return;
			bool forcedAdd = DataProvider.CellCount == 0 && IsGrandTotalEmpty();
			for(int i = 0; i < Data.DataFieldCount; i++) {
				if(CanAddDataCell(i, valueType) || forcedAdd) {
					AddChildCell(new PivotFieldDataCellValueItem(DataProvider, index, i, valueType));
				}
			}
		}
		bool ShouldAddDataCells(PivotGridValueType valueType) {
			bool forcedAdd = DataProvider.CellCount == 0 && IsGrandTotalEmpty();
			int dataCellsCount = 0;
			for(int i = 0; i < Data.DataFieldCount; i++)
				if(CanAddDataCell(i, valueType))
					dataCellsCount++;
			if(dataCellsCount < 2 && !forcedAdd) return false;
			return true;
		}
		bool CanAddDataCell(PivotGridValueType valueType) {
			for(int i = 0; i < Data.DataFieldCount; i++) 
				if(CanAddDataCell(i, valueType)) return true;
			return false;
		}
		bool CanAddDataCell(int dataFieldIndex, PivotGridValueType valueType) {
			PivotGridFieldBase dataField = Data.GetFieldByArea(PivotArea.DataArea, dataFieldIndex);
			if(dataField == null) return true;
			return dataField.CanShowValueType(valueType);
		}
		void AddGrandTotals(PivotTotalsLocation totalsLocation) {
			if(Data.OptionsView.TotalsLocation != totalsLocation) return;
			if((ShowGrandTotals && !IsGrandTotalEmpty()) || DataProvider.CellCount == 0) {
				PivotFieldGrandTotalCellValueItem grandTotalCellItem = new PivotFieldGrandTotalCellValueItem(DataProvider, GetDataIndex(PivotGridValueType.GrandTotal));
				if(Data.GetIsDataFieldsVisible(IsColumn) && !ShouldAddDataCells(PivotGridValueType.GrandTotal))
					grandTotalCellItem.EndLevel++;
				AddChildCell(grandTotalCellItem);
				AddDataCells(-1, -1, PivotGridValueType.GrandTotal);
			}
		}
		bool IsGrandTotalEmpty() {
			for(int i = 0; i < Data.DataFieldCount; i++)
				if(CanAddDataCell(i, PivotGridValueType.GrandTotal)) return false;
			return true;
		}
		bool IsValuesEmpty() {
			for(int i = 0; i < Data.DataFieldCount; i++)
				if(CanAddDataCell(i, PivotGridValueType.Value)) return false;
			return true;
		}
		int[] CreateSummariesArray(int lastLevel) {
			int[] summaries = new int[lastLevel];
			for(int i = 0; i < lastLevel; i ++) {
				summaries[i] = -1;
			}
			return summaries;
		}
		void CreateLastLevelItems() {
			for (int i = 0; i < Count; i++) {
				if (this[i].IsLastFieldLevel)
					this.lastLevelItems.Add(this[i]);
			}
		}		
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(int itemIndex) {
			PivotFieldValueItem item = this[itemIndex];
			return Data.GetFieldSortConditions(item.IsColumn, item.VisibleIndex);
		}
		public bool IsFieldSortedBySummary(PivotGridFieldBase field, PivotGridFieldBase dataField, int itemIndex) {
			return IsFieldSortedBySummaryCore(field, dataField, GetFieldSortConditions(itemIndex));
		}
		bool IsFieldSortedBySummaryCore(PivotGridFieldBase field, PivotGridFieldBase dataField, List<PivotGridFieldSortCondition> itemConditions) {
			return Data.IsFieldSortedBySummary(field, dataField, itemConditions);
		}
		public bool GetIsAnyFieldSortedBySummary(int itemIndex) {
			return GetSortedBySummaryFields(itemIndex) != null;
		}
		public List<PivotGridFieldPair> GetSortedBySummaryFields(int itemIndex) {
			PivotFieldValueItem item = this[itemIndex];			
			if(!item.IsLastFieldLevel) return null;
			List<PivotGridFieldPair> res = null;
			PivotGridFieldBase dataField = item.DataField;
			List<PivotGridFieldSortCondition> itemConditions = GetFieldSortConditions(itemIndex);
			int fieldCount = Data.Fields.Count;
			for(int i = 0; i < fieldCount; i++) {
				PivotGridFieldBase field = Data.Fields[i];
				if(field.Area == item.CrossArea && field.Visible && IsFieldSortedBySummaryCore(field, dataField, itemConditions)) {
					if(res == null)
						res = new List<PivotGridFieldPair>();
					res.Add(new PivotGridFieldPair(field, dataField != null ? dataField : field.SortBySummaryInfo.Field));
				}
			}
			return res;
		}
	}
	public class PivotFieldValueItem : PivotFieldValueItemBase {
		int visibleIndex;
		int index;
		int dataIndex;
		int uniqueIndex;
		int minLastLevelIndex;
		int maxLastLevelIndex;
		int startLevel;
		int endLevel;
		int fieldLevel;
		ArrayList cells;
		object tag;
		List<PivotGridFieldSortCondition> cachedFieldSortConditions;
		public PivotFieldValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex) 
			: base(dataProvider) {
			this.visibleIndex = visibleIndex;
			this.dataIndex = dataIndex;
			if(OptionsDataField.DataFieldVisible) {
				this.fieldLevel = GetCorrectedFieldValueLevel(DataProvider.GetObjectLevel(visibleIndex));
				this.startLevel = this.endLevel = FieldLevel;
			} else {
				this.fieldLevel = DataProvider.GetObjectLevel(visibleIndex);
				this.startLevel = this.endLevel = GetCorrectedFieldValueLevel(DataProvider.GetObjectLevel(visibleIndex));
			}
			this.cells = null;
		}
		public object Tag { get { return tag; } set { tag = value; } }
		public int CellCount { get { return cells != null ? cells.Count : 0; } }
		public int TotalsCount {
			get {
				if(IsCollapsed) return 0;
				switch(Field.TotalsVisibility) {
					case PivotTotalsVisibility.AutomaticTotals:
						if(IsDataFieldsVisible && IsDataLocatedInThisArea && Data.OptionsDataField.AreaIndex > Field.AreaIndex)
							return Data.DataFieldCount;
						else
							return 1;
					case PivotTotalsVisibility.CustomTotals:
						return Field.CustomTotals.Count;
				}
				return 0;
			}
		}
		public PivotFieldValueItem GetCell(int index) {
			return cells[index] as PivotFieldValueItem;
		}
		public PivotFieldValueItem[] GetLastLevelCells() {
			ArrayList list = new ArrayList();
			CopyLastLevelCells(this, list);
			return (PivotFieldValueItem[])list.ToArray(typeof(PivotFieldValueItem));
		}
		protected void CopyLastLevelCells(PivotFieldValueItem root, ArrayList list) {
			if(CellCount == 0) {
				if(root != this) {
					list.Add(this);
				}
				return;
			}
			for(int i = 0; i < CellCount; i ++) {
				GetCell(i).CopyLastLevelCells(root, list);
			}
		}
		public void AddCell(PivotFieldValueItem cell) {
			if (this.cells == null) {
				this.cells = new ArrayList();
			}
			cells.Add(cell);
		}
		public int VisibleIndex { get { return visibleIndex; } }
		public int UniqueIndex { get { return uniqueIndex; } set { uniqueIndex = value; } }
		public int MinLastLevelIndex { get { return minLastLevelIndex; } set { minLastLevelIndex = value; } }
		public int MaxLastLevelIndex { get { return maxLastLevelIndex; } set { maxLastLevelIndex = value; } }
		public int Level {	get { return StartLevel; } }
		public int StartLevel { get { return startLevel; } set { startLevel = value; } }
		public int EndLevel { get { return endLevel; } set { endLevel = value; } }
		public bool IsLastFieldLevel { get { return EndLevel == LevelCount - 1; } }
		public int SpanCount {
			get {
				if(IsLastFieldLevel) return 1;
				int count = 0;
				for(int i = 0; i < CellCount; i ++) {
					if(GetCell(i).IsLastFieldLevel) 
						count ++;
					else count += GetCell(i).SpanCount;
				}
				return count;
			}
		}
		protected int FieldLevel { get { return fieldLevel; } }
		public virtual bool ContainsLevel(int level) { return level >= StartLevel && level <= EndLevel; }
		public virtual int DataIndex { get { return dataIndex; } }
		public virtual PivotSummaryType SummaryType { 
			get { 
				PivotGridFieldBase field = DataField;
				if(field != null)
					return field.SummaryType;
				else return PivotSummaryType.Sum;
			}
		}
		public virtual PivotGridCustomTotalBase CustomTotal { get { return null; } }
		public virtual string Text { get { return string.Empty; } }
		public virtual bool IsOthersRow { get { return DataProvider.IsOthersValue(VisibleIndex); } }
		public virtual PivotGridValueType ValueType { get { return PivotGridValueType.Value; } }
		public virtual PivotGridFieldBase Field { get { return GetFieldByLevel(); } }
		public virtual PivotGridFieldBase ResizingField { get { return Field; } }
		public PivotGridFieldBase ColumnField { get {  return IsColumn ? GetFieldByLevel() : null;  } }
		public PivotGridFieldBase RowField { get { return !IsColumn ? GetFieldByLevel() : null; } }
		public PivotGridFieldBase DataField {
			get {
				Data.EnsureFieldCollections();
				return Data.GetFieldByArea(PivotArea.DataArea, DataIndex); 
			}
		}
		PivotGridFieldBase GetFieldByLevel() {
			Data.EnsureFieldCollections();
			return Data.GetFieldByLevel(IsColumn, FieldLevel); 
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(IsColumn ? VisibleIndex : -1, !IsColumn ? VisibleIndex : -1, DataIndex);
		}
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetOLAPDrillDownDataSource(IsColumn ? VisibleIndex : -1, !IsColumn ? VisibleIndex : -1, DataIndex, 
				maxRowCount, customColumns);
		}
		public virtual object Value { get { return VisibleIndex > -1 ? DataProvider.GetObjectValue(VisibleIndex) : null; } }
		protected virtual int GetRowFieldWidth(int level) {
			if(IsDataFieldsVisible && DataLevel == level) return OptionsDataField.RowHeaderWidth;
			if(IsLevelAfterDataField(level)) level --;
			return Data.GetFieldWidth(PivotArea.RowArea, level);
		}
		protected virtual int RowHeaderWidth {
			get {
				if(IsColumn || StartLevel < 0) return Data.DefaultFieldWidth;
				int width = 0;
				for(int i = StartLevel; i <= EndLevel; i ++)
					width += GetRowFieldWidth(i);
				return width;
			}
		}
		public int CellLevelCount { get { return EndLevel - StartLevel + 1; } }
		public virtual bool ShowCollapsedButton { get { return false; } }
		public virtual bool IsCollapsed { get { return false; } }
		public bool IsTotal { get { return ValueType != PivotGridValueType.Value; } }
		protected bool AllowExpand {
			get {
				if(Field == null || !Data.OptionsCustomization.AllowExpand) return false;
				if(Field.Options.AllowExpand == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowExpand;
				return Field.Options.AllowExpand == DefaultBoolean.True ? true : false;
			}
		}
		protected bool CanShowCollapsedButton { get { return StartLevel != LastFieldLevel && AllowExpand; } }
		protected bool IsTotalsLocationFarOrClosed { 
			get {
				return Data.OptionsView.TotalsLocation == PivotTotalsLocation.Far || IsCollapsed; 
			} 
		}
		protected bool IsTotalsVisible {
			get {				
				return Field.GetTotalSummaryCount(CellCount <= 1) > 0;
			}
		}
		public int Index { get { return index; } }
		internal void SetIndex(int value) { index = value; }
		protected internal List<PivotGridFieldSortCondition> CachedFieldSortConditions { get { return cachedFieldSortConditions; } set { cachedFieldSortConditions = value; } }
		public List<PivotGridFieldBase> GetCrossAreaFields() {
			return Data.GetFieldsByArea(CrossArea, false);
		}
	}
	public class PivotFieldCellValueItem : PivotFieldValueItem {
		bool isCollapsed;
		public PivotFieldCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, bool isObjectCollapsed, int dataIndex) : 
			base(dataProvider, visibleIndex, dataIndex) {
			this.isCollapsed = isObjectCollapsed;
			if(IsCollapsed) {
				if(IsDataLocatedInThisArea && Data.DataFieldCount > 1 && Level < OptionsDataField.DataFieldAreaIndex)
					EndLevel = DataProvider.LevelCount - 1;
				else EndLevel = LastFieldLevel;
			}
		}
		public override bool IsCollapsed { get { return isCollapsed; } }
		public override bool ShowCollapsedButton { get { return CanShowCollapsedButton && (IsTotalsLocationFarOrClosed || !IsTotalsVisible); } }
		public override string Text { 
			get { 
				if(IsOthersRow)
					return GetLocalizedString(PivotGridStringId.TopValueOthersRow);
				return Field.GetValueText(Value); 
			} 
		}
	}
	public class PivotFieldTotalCellValueItem : PivotFieldValueItem {
		bool isBeforeData;
		public PivotFieldTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex) : 
			base(dataProvider, visibleIndex, dataIndex) {
			EndLevel = LastFieldLevel;
			isBeforeData = IsLevelBeforeDataField(Level) && OptionsDataField.DataFieldAreaIndex < DataProvider.LevelCount;
			if(isBeforeData)
				EndLevel -= 1;
		}
		public override bool ShowCollapsedButton { get { return CanShowCollapsedButton && !IsTotalsLocationFarOrClosed; } }
		public override PivotGridValueType ValueType { get { return PivotGridValueType.Total; } }
		public override string Text { 
			get { 
				if(IsOthersRow)					
					return Field.GetTotalOthersText();
				return Field.GetTotalValueText(Value); 
			} 
		}
	}
	public class PivotFieldCustomTotalCellValueItem : PivotFieldTotalCellValueItem {
		PivotGridCustomTotalBase customTotal;
		int dataIndex;
		bool isFirst;
		public PivotFieldCustomTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, 
			PivotGridCustomTotalBase customTotal, int dataIndex, bool isFirst) 
			: base(dataProvider, visibleIndex, dataIndex) {
			this.customTotal = customTotal;
			this.dataIndex = dataIndex;
			this.isFirst = isFirst;
			EndLevel = LastFieldLevel;
			if(IsLevelBeforeDataField(Level) && OptionsDataField.DataFieldAreaIndex < DataProvider.LevelCount)
				EndLevel -= 1;
		}
		public override bool ShowCollapsedButton { get { return CanShowCollapsedButton && !IsTotalsLocationFarOrClosed && IsFirst; } }
		public override PivotGridCustomTotalBase CustomTotal { get { return customTotal; } }
		public override PivotGridValueType ValueType {	get {	return PivotGridValueType.CustomTotal; } }
		public override int DataIndex { get { return dataIndex; } }
		public override PivotSummaryType SummaryType { get { return customTotal.SummaryType; } }
		public override string Text { get { return CustomTotal.GetValueText(Value); } }
		public bool IsFirst { get { return isFirst; } }
	}
	public class PivotFieldGrandTotalCellValueItem : PivotFieldValueItem {
		PivotGridFieldBase childDataField;
		public PivotFieldGrandTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int dataIndex) : base(dataProvider, -1, dataIndex) {
			StartLevel = 0;
			EndLevel = DataProvider.LevelCount - 1;
			if(EndLevel < 0)
				EndLevel = 0;
		}
		public override PivotGridFieldBase Field { 
			get {
				if(Data.GetFieldCountByArea(Area) == 0 && Data.DataFieldCount == 1) 
					return Data.GetFieldByArea(PivotArea.DataArea, 0);
				return null;
			}
		}
		protected PivotGridFieldBase ChildDataField {
			get {
				if(childDataField == null)
					childDataField = GetChildDataField();
				return childDataField;
			}
		}
		protected PivotGridFieldBase GetChildDataField() {
			if(!IsDataLocatedInThisArea)
				return null;
			PivotGridFieldBase res = null;
			for(int i = 0; i < Data.Fields.Count; i++) {
				PivotGridFieldBase field = Data.Fields[i];
				if(field.Area == PivotArea.DataArea && field.Options.ShowGrandTotal) {
					if(res == null)
						res = field;
					else
						return null;	
				}
			}
			return res;
		}
		public override PivotGridFieldBase ResizingField { 
			get {
				if(Field != null) return Field;
				if(!IsColumn) return null;
				if(ChildDataField != null && Data.DataFieldCount > 1)
					return ChildDataField;
				if(Data.ColumnFieldCount == 0 && Data.DataFieldCount < 1)
					return Data.GetFieldByArea(PivotArea.DataArea, 0) as PivotGridFieldBase;
				if(Data.DataFieldCount < 2 && Data.ColumnFieldCount > 0)
					return Data.GetFieldByArea(PivotArea.ColumnArea, Data.ColumnFieldCount - 1) as PivotGridFieldBase;
				if(Data.DataField.Area == PivotArea.RowArea)
					return Data.DataField;
				return null;
			} 
		}
		public override PivotGridValueType ValueType {	get {	return PivotGridValueType.GrandTotal; } }
		protected override int RowHeaderWidth { 
			get { return ResizingField != null ? ResizingField.Width : base.RowHeaderWidth;	}
		}
		public override string Text { 
			get { 
				if(Field != null) {
					return Field.GetGrandTotalText();
				}
				return GetLocalizedString(PivotGridStringId.GrandTotal); 
			} 
		}
		public override bool IsOthersRow { get { return false; } }
	}
	public class PivotFieldDataCellValueItem : PivotFieldValueItem {
		string text;
		PivotGridValueType valueType;
		public PivotFieldDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex) : this(dataProvider, visibleIndex, dataIndex, PivotGridValueType.Value) {
		}
		public PivotFieldDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex, PivotGridValueType valueType) : base(dataProvider, visibleIndex, dataIndex) {
			StartLevel = EndLevel = DataProvider.LevelCount; 
			if(StartLevel == 0) 
				StartLevel = EndLevel = 1;
			this.text = DataField.ToString();
			this.valueType = valueType;
		}
		public override PivotGridFieldBase Field  { get { return DataField; } }
		public override PivotGridFieldBase ResizingField { 
			get { 
				if(IsColumn) return base.ResizingField;
				if(Level == DataLevel)
					return Data.DataField;
				if(Data.GetFieldCountByArea(Area) > 0)
					return Data.GetFieldByArea(Area, Data.GetFieldCountByArea(Area) - 1); 
				return base.ResizingField;
			} 
		}
		public override PivotGridValueType ValueType {	get {	return this.valueType; } }
		public override string Text { get { return text; } }
	}
	public class PivotFieldCustomTotalDataCellValueItem : PivotFieldCustomTotalCellValueItem {
		string text;
		public PivotFieldCustomTotalDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex,
			PivotGridCustomTotalBase customTotal, int dataIndex)
			: base(dataProvider, visibleIndex, customTotal, dataIndex, false) {
			StartLevel = EndLevel = DataProvider.LevelCount;
			if(StartLevel == 0)
				StartLevel = EndLevel = 1;
			this.text = DataField.ToString();
		}
		public override bool ShowCollapsedButton { get { return false; } }
		public override PivotGridFieldBase Field { get { return DataField; } }
		public override string Text { get { return text; } }
	}
	public class PivotFieldTopDataCellValueItem : PivotFieldValueItem {
		string text;
		public PivotFieldTopDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex) : base(dataProvider, visibleIndex, dataIndex) {
			StartLevel = EndLevel = DataLevel; 
			this.text = DataField.ToString();
		}
		public override PivotGridFieldBase Field  { get { return DataField; } }
		public override PivotGridFieldBase ResizingField { get { return IsColumn ? base.ResizingField : Data.DataField; } }
		public override string Text { get { return text; } }
	}
}
