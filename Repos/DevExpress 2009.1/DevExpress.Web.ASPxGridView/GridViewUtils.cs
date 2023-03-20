#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxGridView                                 }
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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Web.ASPxClasses;
using System.Web.UI.WebControls;
using System.Collections;
using DevExpress.XtraGrid;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView.Helper {
	public class GridFilterData : BaseFilterData {
		ASPxGridView grid;
		public GridFilterData(ASPxGridView grid) {
			this.grid = grid;
		}
		public ASPxGridView Grid { get { return grid; } }
		protected override void OnFillColumns() {
			foreach(GridViewDataColumn column in Grid.DataColumns) {
				if(string.IsNullOrEmpty(column.FieldName)) continue;
				GridColumnInfo info = CreateColumnInfo(column);
				if(!info.Required) continue;
				Columns[column.FieldName] = info;
			}
		}
		protected virtual GridColumnInfo CreateColumnInfo(GridViewDataColumn column) {
			GridColumnInfo info = new GridColumnInfo(this, column);
			info.Required = Grid.FilterHelper.GetColumnFilterMode(column, Grid.RenderHelper.GetColumnAutoFilterEditKind(column) == GridViewColumnEditKind.CheckBox) == ColumnFilterMode.DisplayText;
			return info;
		}
		public override int SortCount { get { return Grid.SortCount; } }
		public override int GroupCount { get { return Grid.GroupCount; } }
		public override int GetSortIndex(object column) {
			GridViewDataColumn col = column as GridViewDataColumn;
			return Grid.SortedColumns.IndexOf(col);
		}
		public GridColumnInfo GetInfo(GridViewDataColumn column) {
			return GetInfoCore(column.FieldName) as GridColumnInfo;
		}
		protected override object GetKey(DataColumnInfo column) { return column.Name; }
	}
	public class GridSortData : GridFilterData {
		public GridSortData(ASPxGridView grid)
			: base(grid) {
		}
		protected override GridColumnInfo CreateColumnInfo(GridViewDataColumn column) {
			GridColumnSortInfo info = new GridColumnSortInfo(this, column);
			info.SortMode = GetSortMode(column);
			info.GroupInterval = GetColumnGroupInterval(column);
			return info;
		}
		protected virtual ColumnGroupInterval GetColumnGroupInterval(GridViewDataColumn column) {
			EditPropertiesBase edit = Grid.RenderHelper.GetColumnEdit(column);
			if(edit == null) return ColumnGroupInterval.Value;
			ColumnGroupInterval group = column.Settings.GroupInterval;
			if(group == ColumnGroupInterval.Default) {
				if(edit is DateEditProperties) {
					group = ColumnGroupInterval.Date;
				}
			}
			if(group == ColumnGroupInterval.Default) {
				group = ColumnGroupInterval.Value;
			}
			return group;
		}
		protected virtual ColumnSortMode GetSortMode(GridViewDataColumn column) {
			EditPropertiesBase edit = Grid.RenderHelper.GetColumnEdit(column);
			if(edit == null) return ColumnSortMode.Value;
			ColumnSortMode sort = column.Settings.SortMode;
			if(sort == ColumnSortMode.Default)
				sort = Grid.SettingsBehavior.SortMode;
			if(sort == ColumnSortMode.Default)
				sort = edit.RequireDisplayTextSorting ? ColumnSortMode.DisplayText : ColumnSortMode.Value;
			return sort;
		}
		protected override string[] GetOutlookLocalizedStrings() {
			return new string[] {
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Older),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LastMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_EarlierThisMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_ThreeWeeksAgo),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_TwoWeeksAgo),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LastWeek),
				"", "", "", "", "", "", "",
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Yesterday),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Today),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Tomorrow),
				"", "", "", "", "", "", "",
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_NextWeek),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_TwoWeeksAway),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_ThreeWeeksAway),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LaterThisMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_NextMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_BeyondNextMonth)
			};
		}
	}
	public class GridColumnSortInfo : GridColumnInfo {
		public GridColumnSortInfo(GridSortData columnData, GridViewDataColumn column)
			: base(columnData, column) {
		}
		public override bool Required {
			get {
				if(base.Required) return true;
				if(SortMode == ColumnSortMode.DisplayText || SortMode == ColumnSortMode.Custom) return true;
				if(GroupInterval != ColumnGroupInterval.Default && GroupInterval != ColumnGroupInterval.Value) return true;
				return false;
			}
			set { base.Required = value; }
		}
	}
	public class GridColumnInfo : BaseGridColumnInfo, IValueProvider {
		EditPropertiesBase properties;
		CustomColumnSortEventArgs sortArgs;
		public GridColumnInfo(GridFilterData data, GridViewDataColumn column)
			: base(data, column) {
			this.properties = RenderHelper.GetColumnEdit(Column);
			this.sortArgs = new CustomColumnSortEventArgs(column, null, null, ColumnSortOrder.None);
			this.sortArgs.grid = data.Grid;
		}
		public new GridFilterData Data { get { return base.Data as GridFilterData; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Data.Grid.RenderHelper; } }
		public new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		public EditPropertiesBase Properties { get { return properties; } }
		public override string GetDisplayText(int listSourceIndex, object val) {
			this.listSourceRow = listSourceIndex;
			string res = Properties.GetDisplayText(RenderHelper.TextBuilder.GetDisplayControlArgsCore(Column, listSourceIndex, this, Data.Grid, val), false); 
			this.listSourceRow = DataController.InvalidRow;
			return res;
		}
		protected override int RaiseCustomSort(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			Data.Grid.RaiseCustomColumnSort(this.sortArgs);
			return this.sortArgs.GetSortResult();
		}
		protected override int RaiseCustomGroup(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder columnSortOrder) {
			this.sortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, ColumnSortOrder.None);
			Data.Grid.RaiseCustomColumnGroup(this.sortArgs);
			return this.sortArgs.GetSortResult();
		}
		#region IValueProvider Members
		int listSourceRow = DataController.InvalidRow;
		object IValueProvider.GetValue(string fieldName) {
			return Data.Grid.DataProxy.GetListSourceRowValue(listSourceRow, fieldName);
		}
		#endregion
	}
	public class BaseFilterHelper {
		protected virtual string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, CriteriaOperator currentFilter) {
			CriteriaOperator op = currentFilter;
			BinaryOperator bop;
			if(object.ReferenceEquals(op, null))
				return string.Empty;
			bop = op as BinaryOperator;
			GroupOperator group = op as GroupOperator;
			if(!object.ReferenceEquals(group, null) && group.Operands.Count > 0) bop = group.Operands[0] as BinaryOperator;
			if(object.ReferenceEquals(bop, null))
				return string.Empty;
			OperandProperty p = bop.LeftOperand as OperandProperty;
			if(object.ReferenceEquals(p, null) || p.PropertyName != fieldName)
				return string.Empty;
			OperandValue v = bop.RightOperand as OperandValue;
			if(object.ReferenceEquals(v, null))
				return string.Empty;
			if(v.Value == null) return string.Empty;
			string value = v.Value.ToString();
			if(v.Value is DateTime) value = ((DateTime)v.Value).ToString("d", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			if(bop.OperatorType == BinaryOperatorType.Like)
				value = ExtractLikeString(condition, value);
			return value;
		}
		protected virtual CriteriaOperator CreateAutoFilter(AutoFilterCondition condition, string fieldName, Type dataType, string _value, bool roundDateTime) {
			if(string.IsNullOrEmpty(_value)) return null;
			CriteriaOperator op;
			if(condition == AutoFilterCondition.Equals) {
				op = DevExpress.Data.Helpers.FilterHelper.CalcColumnFilterCriteriaByValue(fieldName, dataType, _value, roundDateTime, System.Globalization.DateTimeFormatInfo.InvariantInfo);
			}
			else {
				op = GetOperatorForCondition(condition, fieldName, _value, dataType);
			}
			return op;
		}
		CriteriaOperator GetOperatorForCondition(AutoFilterCondition condition, string fieldName, string value, Type dataType) {
			switch(condition) {
				case AutoFilterCondition.NotEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.NotEqual);
				case AutoFilterCondition.Less:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.Less);
				case AutoFilterCondition.Greater:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.Greater);
				case AutoFilterCondition.LessOrEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.LessOrEqual);
				case AutoFilterCondition.GreaterOrEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.GreaterOrEqual);
				default:
					return new BinaryOperator(fieldName, CreateAutofilterLike(condition, value), BinaryOperatorType.Like);
			}			
		}
		object ChangeTypeSafe(object value, Type type) {
			if(value == null) return null;
			if(value.GetType() == type) return value;
			try {
				Type underlying = Nullable.GetUnderlyingType(type);
				if(underlying != null) type = underlying;
				value = FixFloatingPoint(value, type); 
				return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
			} catch {
				return null;
			}
		}
		object FixFloatingPoint(object value, Type type) {
			if(type != typeof(float) && type != typeof(double) && type != typeof(decimal))
				return value;
			if(value is string)
				return value.ToString().Replace(",", ".");
			return value;
		}
		protected internal virtual string ExtractLikeString(AutoFilterCondition condition, string input) {
			string text = input;
			if(condition == AutoFilterCondition.EndsWith || condition == AutoFilterCondition.Contains) {
				if(input.StartsWith("%")) text = text.Length < 2 ? string.Empty : text.Substring(1);
			}
			if(condition == AutoFilterCondition.BeginsWith || condition == AutoFilterCondition.Contains) {
				if(input.EndsWith("%")) text = text.Length < 2 ? string.Empty : text.Substring(0, text.Length - 1);
			}
			string res = UnEscapeString(text);
			return res;
		}
		protected virtual string UnEscapeString(string input) {
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < input.Length; n++) {
				char ch = input[n];
				if(ch == '[' && n + 2 < input.Length && input[n + 2] == ']') {
					ch = input[n + 1];
					n += 2;
				}
				sb.Append(ch);
			}
			return sb.ToString();
		}
		bool IsMask(char ch) {
			return ch == '?' || ch == '_' || ch == '*' || ch == '%';
		}
		string ReplaceMask(char ch) {
			if(ch == '?' || ch == '_') return "_";
			if(ch == '*' || ch == '%') return "%";
			return string.Empty;
		}
		protected internal virtual string CreateAutofilterLike(AutoFilterCondition condition, string input) {
			if(string.IsNullOrEmpty(input)) return "%";
			char start = input[0], end = input[input.Length - 1];
			if(input.Length == 1 && IsMask(start) && ReplaceMask(start) == "%") return "%";
			if(input.Length == 1) end = '\0';
			string text = input.Substring(1, input.Length == 1 ? 0 : input.Length - 2);
			if(!IsMask(start)) text = start + text;
			if(input.Length > 1 && !IsMask(end) && end != '\0') text = text + end;
			text = ReplaceMask(start) + DevExpress.Data.Filtering.Helpers.LikeData.Escape(text) + ReplaceMask(end);
			if(condition == AutoFilterCondition.EndsWith || condition == AutoFilterCondition.Contains) text = "%" + text;
			if(condition == AutoFilterCondition.BeginsWith || condition == AutoFilterCondition.Contains) text = text + "%";
			return text;
		}
		protected virtual AutoFilterCondition GetColumnAutoFilterCondition(AutoFilterCondition sourceCondition, ColumnFilterMode sourceMode, GridViewColumnEditKind editKind, Type dataType, bool serverMode) {
			AutoFilterCondition condition = sourceCondition;
			if(condition == AutoFilterCondition.Default) condition = GetDefaultAutoFilterCondition(dataType, serverMode);
			ColumnFilterMode mode = GetColumnFilterMode(sourceMode, editKind == GridViewColumnEditKind.CheckBox, serverMode);
			if(editKind == GridViewColumnEditKind.CheckBox) return AutoFilterCondition.Equals;
			if(condition == AutoFilterCondition.BeginsWith || condition == AutoFilterCondition.Contains || condition == AutoFilterCondition.EndsWith) { 
				if((editKind == GridViewColumnEditKind.DateEdit || editKind == GridViewColumnEditKind.ComboBox) && mode != ColumnFilterMode.DisplayText)
					condition = AutoFilterCondition.Equals;
			}
			return condition;
		}
		protected virtual ColumnFilterMode GetColumnFilterMode(ColumnFilterMode mode, bool isCheckBox, bool serverMode) {
			if(serverMode) return ColumnFilterMode.Value;
			if(isCheckBox) return ColumnFilterMode.Value;
			return mode;
		}		
		internal static AutoFilterCondition GetDefaultAutoFilterCondition(Type type, bool serverMode) {
			return type != typeof(string) && serverMode ? AutoFilterCondition.Equals : AutoFilterCondition.BeginsWith;
		}
	}
	public enum GridViewColumnEditKind { Text, DateEdit, ComboBox, CheckBox };
	public class GridViewFilterHelper : BaseFilterHelper {
		ASPxGridView grid;
		public GridViewFilterHelper(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		public virtual string GetColumnAutoFilterText(GridViewDataColumn column) {
			CriteriaOperator op = Grid.GetColumnFilter(column);
			if(object.ReferenceEquals(op, null)) return string.Empty;
			string res = GetColumnAutoFilterText(GetColumnAutoFilterCondition(column), column.FieldName, op);
			ASPxGridViewAutoFilterEventArgs e = new ASPxGridViewAutoFilterEventArgs(column, op, GridViewAutoFilterEventKind.ExtractDisplayText, res);
			Grid.RaiseProcessColumnAutoFilter(e);
			return e.Value;
		}
		public AutoFilterCondition GetColumnAutoFilterCondition(GridViewDataColumn column)  {
			return GetColumnAutoFilterCondition(column.Settings.AutoFilterCondition, column.Settings.FilterMode, GetEditKind(column), column.GetDataType(), Grid.GetServerMode());
		}
		GridViewColumnEditKind GetEditKind(GridViewDataColumn column) { return Grid.RenderHelper.GetColumnAutoFilterEditKind(column); }
		public virtual CriteriaOperator CreateAutoFilter(GridViewDataColumn column, string value) {
			CriteriaOperator res = CreateAutoFilter(GetColumnAutoFilterCondition(column), column.FieldName, column.GetDataType(), value, GetEditKind(column) == GridViewColumnEditKind.DateEdit);
			ASPxGridViewAutoFilterEventArgs e = new ASPxGridViewAutoFilterEventArgs(column, res, GridViewAutoFilterEventKind.CreateCriteria, value);
			Grid.RaiseProcessColumnAutoFilter(e);
			return e.Criteria;
		}
		public virtual ColumnFilterMode GetColumnFilterMode(GridViewDataColumn column, bool isCheckBox) {
			return GetColumnFilterMode(column.Settings.FilterMode, isCheckBox, Grid.GetServerMode());
		}
		public virtual CriteriaOperator CreateHeaderFilter(GridViewDataColumn column, string value) {
			Type type = column.GetDataType();
			if(column.Settings.FilterMode == ColumnFilterMode.DisplayText) {
				type = typeof(string);
			}
			if(type == typeof(DateTime))
				value = DateTime.Parse(value, System.Globalization.CultureInfo.CurrentCulture).ToString(System.Globalization.CultureInfo.InvariantCulture);
			return CreateAutoFilter(AutoFilterCondition.Equals, column.FieldName, type, value, GetEditKind(column) == GridViewColumnEditKind.DateEdit);
		}
	}
	public class GridViewEventsHelper {
		Dictionary<object, bool> pendingEvents;
		Dictionary<object, bool> PendingEvents {
			get {
				if(pendingEvents == null) pendingEvents = new Dictionary<object, bool>();
				return pendingEvents;
			}
		}
		public void SetPending(object evt) {
			PendingEvents[evt] = true;
		}
		public bool CheckClear(object evt) {
			if(this.pendingEvents == null) return false;
			if(!PendingEvents.ContainsKey(evt)) return false;
			PendingEvents.Remove(evt);
			return true;
		}
	}
	public enum GridViewStyleKinds { DataCell, DataRowEven, DataRowOdd, GroupRow, Preview, RowCommandColumn, RowCommandColumnItem }
	public class StylesCache {
		Hashtable table;
		Hashtable Table {
			get {
				if(table == null) table = new Hashtable();
				return table;
			}
		}
		public void Clear() {
			if(this.table != null) this.table.Clear();
			this.table = null;
		}
		protected object GetKindObject(object styleKind) {
			if(table == null) return null;
			return Table[styleKind];
		}
		Hashtable GetKindHash(object styleKind) {
			return GetKindObject(styleKind) as Hashtable; 
		}
		public void AddStyle(object styleKind, Style style) { AddStyle(styleKind, null, style); }
		public void AddStyle(object styleKind, object styleHash, Style style) {
			if(styleKind == null) throw new ArgumentNullException("styleKind");
			if(style == null) throw new ArgumentNullException("style");
			object hash = GetKindObject(styleKind);
			Hashtable hashTable = hash as Hashtable;
			if(hash == null || styleHash == null) {
				if(styleHash == null) {
					Table[styleKind] = style;
					return;
				}
				Table[styleKind] = hashTable = new Hashtable();
			}
			hashTable[styleHash] = style;
		}
		public Style GetStyle(object styleKind) { return GetStyle(styleKind, null); }
		public Style GetStyle(object styleKind, object styleHash) {
			object hash = GetKindObject(styleKind);
			if(hash == null) return null;
			Hashtable hashTable = hash as Hashtable;
			if(hashTable != null) return hashTable[styleHash] as Style;
			return hash as Style;
		}
	}
	public delegate Style StyleCreateMethod();
}
