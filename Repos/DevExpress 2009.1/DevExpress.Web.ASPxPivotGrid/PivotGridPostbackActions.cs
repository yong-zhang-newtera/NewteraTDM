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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Web.ASPxPivotGrid.Data {
	public class PivotGridPostBackActionBase {
		PivotGridWebData data;
		string[] values;
		public PivotGridPostBackActionBase(PivotGridWebData data, string arguments) {
			this.data = data;
			this.values = arguments.Split(new char[] { '|' });
		}
		protected PivotGridWebData Data { get { return data; } }
		protected string[] Values { get { return values; } }
		public virtual bool RequireDataUpdate { get { return true; } }
		public virtual bool ApplyOnlyOnce { get { return false; } }
		public virtual void ApplyBefore() {
		}
		public virtual void ApplyAfter() {
		}
	}
	public class PivotGridPostbackExpandedItemAction : PivotGridPostBackActionBase {
		int uniqueIndex;
		bool isColumn;
		public PivotGridPostbackExpandedItemAction(PivotGridWebData data, string arguments)
			: base(data, arguments) {
			if(!int.TryParse(Values[1], out uniqueIndex))
				uniqueIndex = -1;
			isColumn = Values[0] == PivotGridWebData.ExpandColumnChanged;
		}
		protected int UniqueIndex { get { return uniqueIndex; } }
		protected bool IsColumn { get { return isColumn; } }
		protected object[] ItemValues {
			get { return Data.VisualItems.GetItemValues(IsColumn, UniqueIndex); }
		}
		protected PivotGridFieldBase Field {
			get { return Data.VisualItems.GetItemField(IsColumn, UniqueIndex); }
		}
		public override void ApplyAfter() {
			if(Values.Length == 2)
				ExpandItem();
			if(Values.Length == 3)
				ExpandField();
		}
		protected virtual void ExpandField() {
			Data.ChangeFieldExpanded(Field, Values[2] == PivotGridWebData.ExpandFieldChanged);
		}
		protected virtual void ExpandItem() {
			if(ItemValues != null)
				Data.OnFieldValueStateChanged(IsColumn, ItemValues);
		}
	}
	public class PivotGridFieldsPostbackActionBase : PivotGridPostBackActionBase {
		PivotGridFieldCollection fields;
		public PivotGridFieldsPostbackActionBase(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			:
			base(data, arguments) {
			this.fields = fields;
		}
		protected PivotGridFieldCollection Fields { get { return fields; } }
		protected PivotGridField GetDragField() {
			if(Values.Length < 2) throw new ArgumentException("Drag field was not specified.");
			return Data.GetFieldByClientID(Values[1]);
		}
	}
	public class PivotGridPostBackChangeGroupExpandedAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackChangeGroupExpandedAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			:
			base(data, fields, arguments) { }
		public override void ApplyBefore() {
			int groupIndex = int.Parse(Values[1]);
			Fields[groupIndex].ExpandedInFieldsGroup = !Fields[groupIndex].ExpandedInFieldsGroup;
		}
	}
	public class PivotGridPostBackDragFieldAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackDragFieldAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			: base(data, fields, arguments) {
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			PivotGridField dragField = GetDragField(), targetField = GetTargetField();
			dragField.Visible = GetIsVisible();
			if(!dragField.Visible)
				return;
			if(dragField.Group != null && dragField.Group.Contains(targetField))
				return;
			PivotArea area = GetPivotArea(dragField, targetField);
			int newIndex = GetPivotAreaIndex(dragField, targetField);
			dragField.SetAreaPosition(area, newIndex);
		}
		protected PivotGridField GetTargetField() {
			if(Values.Length < 3) return null;
			return Data.GetFieldByClientID(Values[2]);
		}
		protected PivotArea GetPivotArea(PivotGridField dragField, PivotGridField targetField) {
			if(Values.Length < 3) return dragField.Area;
			if(targetField != null) return targetField.Area;
			PivotArea area;
			if(Data.GetAreaByID(Values[2], out area))
				return area;
			return dragField.Area;
		}
		protected int GetPivotAreaIndex(PivotGridField dragField, PivotGridField targetField) {
			if(targetField == null) return 0;
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(targetField.Area, true);
			fields.Remove(dragField);
			fields.Insert(fields.IndexOf(targetField) + (IsLeft() ? 0 : 1), dragField);
			return fields.IndexOf(dragField);
		}
		protected bool IsLeft() {
			if(Values.Length < 4) return false;
			return Values[3].ToUpper() == bool.TrueString.ToUpper();
		}
		protected bool GetIsVisible() {
			if(Values.Length < 3) return true;
			return !Data.IsCustomizationFields(Values[2]);
		}
	}
	public class PivotGridPostBackFieldSortAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackFieldSortAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			: base(data, fields, arguments) {
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			PivotGridField field = GetDragField();
			if(field == null) return;
			field.SortOrder = field.SortOrder == PivotSortOrder.Ascending ? PivotSortOrder.Descending : PivotSortOrder.Ascending;
		}
	}
	public class PivotGridPostBackSortByColumnAction : PivotGridFieldsPostbackActionBase {
		int visibleIndex;
		bool isColumn;
		PivotGridField field, dataField;
		bool isRemoveAll;
		List<PivotGridFieldSortCondition> fieldSortConditions;
		public PivotGridPostBackSortByColumnAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			: base(data, fields, arguments) {
			ParseArguments(arguments);
		}
		void ParseArguments(string arguments) {
			this.isRemoveAll = Values[3] == "RemoveAll";
			this.visibleIndex = Int32.Parse(Values[2]);
			PivotArea itemArea = Data.DataField.Area;
			if(VisibleIndex >= 0) {
				PivotGridField itemField = (PivotGridField)Fields.GetFieldByClientID(Values[1]);
				itemArea = itemField.Area != PivotArea.DataArea ? itemField.Area : Data.DataField.Area;
			}
			if(itemArea != PivotArea.ColumnArea && itemArea != PivotArea.RowArea)
				throw new ArgumentException("itemArea");
			if(!IsRemoveAll)
				this.field = Fields[Int32.Parse(Values[3])];
			this.isColumn = itemArea == PivotArea.ColumnArea;
			int dataIndex = Int32.Parse(Values[4]);
			this.dataField = dataIndex >= 0 ? Fields[dataIndex] : null;
			this.fieldSortConditions = Data.GetFieldSortConditions(IsColumn, VisibleIndex);
		}
		public bool IsColumn { get { return isColumn; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public List<PivotGridFieldSortCondition> FieldSortConditions { get { return fieldSortConditions; } }
		public bool IsRemoveAll { get { return isRemoveAll; } }
		public PivotArea FieldArea { get { return IsColumn ? PivotArea.RowArea : PivotArea.ColumnArea; } }
		public PivotGridField Field { get { return field; } }
		public PivotGridField DataField { get { return dataField; } }
		public override void ApplyBefore() {
			base.ApplyBefore();
			if(IsRemoveAll)
				ApplyRemoveAll();
			else
				ApplySorting();
		}
		void ApplySorting() {
			if(IsSortedByThisSummary(Field)) {
				Field.SortBySummaryInfo.Reset();
			} else {
				Field.SortBySummaryInfo.Reset();
				Field.SortBySummaryInfo.Field = DataField;
				Field.SortBySummaryInfo.Conditions.AddRange(FieldSortConditions);
			}
		}
		bool IsSortedByThisSummary(PivotGridFieldBase field) {
			return Data.IsFieldSortedBySummary(field, DataField, FieldSortConditions);
		}
		void ApplyRemoveAll() {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(FieldArea, false);
			for(int i = 0; i < fields.Count; i++) {
				if(IsSortedByThisSummary(fields[i]))
					fields[i].SortBySummaryInfo.Reset();
			}
		}
	}
	public class PivotGridPostBackHideFieldAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackHideFieldAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			: base(data, fields, arguments) {
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			int index = -1;
			if(!int.TryParse(Values[1], out index)) return;
			if(index < -1 || index >= Fields.Count) return;
			Fields[index].Visible = false;
		}
	}
	public class PivotGridPostBackFilterFieldAction : PivotGridPostBackActionBase {
		PivotGridFilterItems filterItems;
		public PivotGridPostBackFilterFieldAction(PivotGridWebData data, PivotGridFieldCollection fields, string arguments)
			: base(data, arguments) {
			PivotGridField field = fields[int.Parse(Values[3])];
			if(field != null) {
				this.filterItems = Data.CreatePivotGridFilterItems(field);
				FilterItems.PersistentString = Values[2];
				FilterItems.StatesString = Values[1];
			}
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			if(FilterItems == null) return;
			FilterItems.ApplyFilter();
		}
		protected PivotGridFilterItems FilterItems { get { return filterItems; } }
	}
	public class PivotGridCustomPostbackAction : PivotGridPostBackActionBase {
		ASPxPivotGrid pivot;
		public PivotGridCustomPostbackAction(ASPxPivotGrid pivot, string arguments)
			: base(pivot.Data, arguments) {
			this.pivot = pivot;
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			pivot.RaiseCustomCallback(string.Join("|", Values));
		}
	}
	public class PivotGridPostBackPagerAction : PivotGridPostBackActionBase {
		public PivotGridPostBackPagerAction(PivotGridWebData data, string arguments)
			: base(data, arguments) { }
		public override bool ApplyOnlyOnce { get { return true; } }
		public override void ApplyBefore() {
			if(Values[1].StartsWith("PN")) {
				string newPageIndexStr = Values[1].Substring(2);
				int newPageIndex = 0;
				if(Int32.TryParse(newPageIndexStr, out newPageIndex))
					Data.OptionsPager.PageIndex = newPageIndex;
			}
			if(Values[1] == "PBN")
				Data.OptionsPager.PageIndex++;
			if(Values[1] == "PBP")
				Data.OptionsPager.PageIndex--;
			if(Values[1] == "PBA")
				Data.OptionsPager.PageIndex = -1;
		}
	}
	public enum PivotGridPrefilterCommand { Show, Hide, Reset, ChangeEnabled, Set };
	public class PivotGridPostBackPrefilterAction : PivotGridPostBackActionBase {
		bool requireDataUpdate;
		PivotGridPrefilterCommand command;
		ASPxPivotGrid pivot;
		public PivotGridPostBackPrefilterAction(ASPxPivotGrid pivot, string arguments)
			: base(pivot.Data, arguments) {
			this.requireDataUpdate = false;
			this.command = (PivotGridPrefilterCommand)Enum.Parse(typeof(PivotGridPrefilterCommand), Values[1]);
			this.pivot = pivot;
			Process();
		}
		public PivotGridPostBackPrefilterAction(ASPxPivotGrid pivot, PivotGridPrefilterCommand command, string argument)
			: base(pivot.Data, "|" + command.ToString() + "|" + argument) {
			this.requireDataUpdate = false;
			this.command = command;
			this.pivot = pivot;
			Process();
		}
		public override bool RequireDataUpdate { get { return requireDataUpdate; } }
		protected PivotGridPrefilterCommand Command { get { return command; } }
		protected ASPxPivotGrid PivotGrid { get { return pivot; } }
		protected string Argument { get { return Values[2]; } }
		protected void Process() {
			switch(Command) {
				case PivotGridPrefilterCommand.Show:
					PivotGrid.IsPrefilterPopupVisible = true;
					break;
				case PivotGridPrefilterCommand.Hide:
					PivotGrid.IsPrefilterPopupVisible = false;
					break;
				case PivotGridPrefilterCommand.Reset:
				case PivotGridPrefilterCommand.ChangeEnabled:
				case PivotGridPrefilterCommand.Set:
					this.requireDataUpdate = true;
					break;
			}
		}
		public override void ApplyBefore() {
			switch(Command) {
				case PivotGridPrefilterCommand.Reset:
					PivotGrid.Prefilter.Criteria = null;
					break;
				case PivotGridPrefilterCommand.ChangeEnabled:
					PivotGrid.Prefilter.Enabled = !pivot.Prefilter.Enabled;
					break;
				case PivotGridPrefilterCommand.Set:
					PivotGrid.Prefilter.CriteriaString = Argument;
					break;
			}
		}
	}
}
