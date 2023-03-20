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
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections.Generic;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridFilterItem : IPivotGridFilterItem {
		object value;
		string text;
		bool isChecked;
		public PivotGridFilterItem(object value, string text, bool isChecked) {
			this.value = value;
			this.text = text;
			this.isChecked = isChecked;
		}
		public object Value { get { return value; } }
		public bool IsChecked { get { return isChecked; } set { isChecked = value; } }
		public bool IsBlank { get { return Value == null; } }
		public override string ToString() { return text; } 
	}
	public class PivotGridFilterItems : CollectionBase, IFilterItems {
		public const char CheckedChar = 'T';
		public const char UncheckedChar = 'F';
		PivotGridData data;
		PivotGridFieldBase field;
		bool[] savedChecks;
		public PivotGridFilterItems(PivotGridData data, PivotGridFieldBase field) {
			this.data = data;
			this.field = field;
			this.savedChecks = null;
		}		
		public PivotGridFilterItem this[int index] { get { return InnerList[index] as PivotGridFilterItem; } }
		IPivotGridFilterItem IFilterItems.this[int index] { get { return this[index]; } }
		public PivotGridData Data { get { return data; } }
		public PivotGridFieldBase Field { get { return field; } }
		protected PivotGridFieldFilterValues FilterValues { get { return Field.FilterValues; } }
		public int CheckCount { 
			get { 
				int checkCount = 0;
				for(int i = 0; i < Count; i ++) {
					if(this[i].IsChecked) checkCount ++;
				}
				return checkCount;
			}
		}
		public void CheckAllItems(bool isChecked) {
			for(int i = 0; i < Count; i ++) {
				this[i].IsChecked = isChecked;
			}
		}
		public bool CanAccept { get { return CheckCount > 0; }	}
		public bool HasChanges {
			get { return !IsChecksEquals(this.savedChecks, GetChecks()); }
		}
		public void CreateItems() {
			Clear();
			AddBlankValue();
			AddValues();
			this.savedChecks = GetChecks();
		}
		public void ApplyFilter() {
			if(!HasChanges || !CanAccept) return;
			FilterValues.SetValues(GetFilteredValues(), FilterValues.FilterType, FilterShowBlank);
		}
		public string StatesString {
			get {
				StringBuilder sb = new StringBuilder(Count);
				for(int i = 0; i < Count; i ++) {
					sb.Append(this[i].IsChecked ? CheckedChar : UncheckedChar);
				}
				return sb.ToString();
			}
			set {
				if(value == null || value == string.Empty) return;
				int count = Math.Min(Count, value.Length);
				for(int i = 0; i < count; i ++) {
					this[i].IsChecked = value[i] == CheckedChar;
				}
			}
		}
		public string PersistentString {
			get {
				MemoryStream stream = new MemoryStream();
				TypedBinaryWriter writer = new TypedBinaryWriter(stream);
				Type columnType = Data.GetFieldType(Field);
				writer.WriteType(columnType);
				writer.Write(Count);
				for(int i = 0; i < Count; i ++) {
					if(columnType == typeof(object))
						writer.WriteTypedObject(this[i].Value);
					else
						writer.WriteObject(this[i].Value);
					writer.Write(this[i].IsChecked);
				}
				writer.Flush();
				writer.Close();
				return Convert.ToBase64String(stream.ToArray());
			}
			set {
				if(value == null || value == string.Empty) return;
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
				TypedBinaryReader reader = new TypedBinaryReader(stream);
				Type columnType = reader.ReadType();
				int count = reader.ReadInt32();
				for(int i = 0; i < count; i ++) {
					object val = columnType == typeof(object) ? reader.ReadTypedObject() : reader.ReadObject(columnType);
					Add(val, string.Empty, reader.ReadBoolean());
				}
				reader.Close();
				stream.Close();
			}
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			this.savedChecks = null;
		}
		int ValueStartIndex { get { return HasBlankItem ? 1 : 0; } }
		bool HasBlankItem {	get { return Count > 0 && this[0].IsBlank; } }
		bool FilterShowBlank {
			get { return HasBlankItem ? this[0].IsChecked : FilterValues.ShowBlanks; }
		}
		object[] GetFilteredValues() {
			int startIndex = ValueStartIndex;
			ArrayList values = new ArrayList();
			bool includeState = FilterValues.FilterType == PivotFilterType.Included;
			for(int i = ValueStartIndex; i < Count; i ++) {
				if(this[i].IsChecked == includeState)
					values.Add(this[i].Value);
			}
			return (object[]) values.ToArray(typeof(object));
		}
		void Add(object value, string text, bool isChecked) {
			InnerList.Add(new PivotGridFilterItem(value, text, isChecked));
		}
		bool[] GetChecks() {
			bool[] checks = new bool[Count];
			for(int i = 0; i < Count; i ++)
				checks[i] = this[i].IsChecked;
			return checks;
		}
		bool IsChecksEquals(bool[] checks1, bool[] checks2) {
			if(checks1 == null || checks2 == null) return false;
			if(checks1.Length != checks2.Length) return false;
			for(int i = 0; i < checks1.Length; i ++) {
				if(checks1[i] != checks2[i]) return false;
			}
			return true;
		}
		void AddBlankValue() {
			if(!Data.HasNullValues(Field) || Data.IsOLAP) 
				return;
			string showBlanksText = Data.GetPivotFieldValueText(Field, null);
			if(showBlanksText == string.Empty)
				showBlanksText = PivotGridLocalizer.GetString(PivotGridStringId.FilterShowBlanks);
			Add(null, showBlanksText, FilterValues.ShowBlanks);
		}		
		void AddValues() {
			object[] values = Field.GetUniqueValues();
			if(values == null) return;
			if(!Data.IsOLAP) {
				if(Field.SortOrder != PivotSortOrder.Ascending) Array.Sort(values, new ReverseComparer());
				else Array.Sort(values);
			}
			for(int i = 0; i < values.Length; i ++) {
				Add(values[i], Data.GetPivotFieldValueText(Field, values[i]), FilterValues.Contains(values[i]));
			}
		}
	}
	class ReverseComparer : IComparer {
		int IComparer.Compare(object a, object b) {
			return Comparer.Default.Compare(b, a);
		}
	}
	class ReverseComparer<T> : IComparer<T> {
		#region IComparer<T> Members
		public int Compare(T x, T y) {
			return Comparer.Default.Compare(y, x);
		}
		#endregion
	}
}
