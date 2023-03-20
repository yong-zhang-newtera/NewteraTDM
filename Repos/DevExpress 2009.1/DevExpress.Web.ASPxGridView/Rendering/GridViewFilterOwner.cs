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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPopupControl;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView.Localization;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class GridViewFilterPopupOwner : IHtmlFilterPopupOwner {
		ASPxGridView grid;
		public GridViewFilterPopupOwner(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		#region IHtmlFilterPopupOwner Members
		ASPxWebControl IHtmlFilterPopupOwner.MainControl { get { return Grid; } }
		string IHtmlFilterPopupOwner.FilterPopupId {  get { return RenderHelper.GetFilterPopupId(); } }
		AppearanceStyle IHtmlFilterPopupOwner.FilterWindowStyle { get { return RenderHelper.GetFilterPopupWindowStyle(); } }
		AppearanceStyle IHtmlFilterPopupOwner.FilterItemsAreaStyle { get { return RenderHelper.GetFilterPopupItemsAreaStyle(); } }
		AppearanceStyle IHtmlFilterPopupOwner.FilterButtonPanelStyle { get { return RenderHelper.GetFilterPopupButtonPanelStyle(); } }
		ImageProperties IHtmlFilterPopupOwner.GetWindowResizer(Page page) { return Grid.Images.GetImageProperties(page, ImagesBase.WindowResizerImageName); }
		Unit IHtmlFilterPopupOwner.DefaultHeight { get { return Grid.SettingsBehavior.HeaderFilterDefaultHeight; } }
		#endregion
	}
	public class GridViewFilterPopupContainerOwner : GridViewFilterPopupOwner, IHtmlFilterPopupContainerOwner {
		GridViewDataColumn column;
		List<FilterValue> filterValues = null;
		object[] uniqueValues = null;
		Type dataType;
		object activeFilterValue;
		string activeFilterQuery;
		public GridViewFilterPopupContainerOwner(GridViewDataColumn column, bool forceAllValues): base(column.Grid) {
			this.column = column;
			this.dataType = column.GetDataType();
			Grid.DataProxy.RequireDataBound();
			this.activeFilterValue = GetActiveFilterValue();
			this.activeFilterQuery = Grid.GetColumnFilterString(column).ToLower();
			FillUniqueValues(forceAllValues);
		}
		protected virtual void FillUniqueValues(bool forceAll) {
			this.uniqueValues = Grid.DataProxy.GetUniqueColumnValues(Column.FieldName, Grid.SettingsBehavior.HeaderFilterMaxRowCount, forceAll || !string.IsNullOrEmpty(Column.FilterExpression));
		}
		protected virtual FilterValue GetShowAllItem() {
			return FilterValue.CreateShowAllValue(Grid.SettingsText.GetHeaderFilterShowAll());
		}
		protected virtual List<FilterValue> PopulateFilterValues() {
			List<FilterValue> res = new List<FilterValue>();
			res.Add(GetShowAllItem());
			if(this.uniqueValues != null) {
				foreach(object val in this.uniqueValues) {
					res.Add(new FilterValue(GetValueDisplayText(val), GetValueText(val)));
				}
			}
			Grid.RaiseHeaderFilterFillItems(new ASPxGridViewHeaderFilterEventArgs(Column, res));
			return res;
		}
		protected virtual string GetValueText(object val) {
			if(val == null || val == DBNull.Value) return string.Empty;
			string res = string.Empty;
			try {
				res = val.ToString();
			} catch {
			}
			return res;
		}
		protected virtual string GetValueDisplayText(object val) {
			return RenderHelper.TextBuilder.GetFilterPopupItemText(Column, val);
		}
		protected GridViewDataColumn Column { get { return column; } }
		public List<FilterValue> FilterValues {
			get {
				if(filterValues == null) filterValues = PopulateFilterValues();
				return filterValues;
			}
		}
		object GetActiveFilterValue() {
			CriteriaOperator op = Grid.GetColumnFilter(Column);
			BinaryOperator binOp = op as BinaryOperator;
			if(object.Equals(binOp, null)) {
				GroupOperator grOp = op as GroupOperator;
				if(!object.ReferenceEquals(grOp, null) && grOp.Operands.Count > 0)
					binOp = grOp.Operands[0] as BinaryOperator;
			}
			if(object.ReferenceEquals(binOp, null))
				return null;
			OperandValue value = binOp.RightOperand as OperandValue;
			if(object.ReferenceEquals(value, null))
				return null;
			return value.Value;
		}
		protected object ActiveFilterValue { get { return activeFilterValue; } }
		public bool IsFilterValueActive(FilterValue filterValue) {
			if(filterValue.IsEmpty) {
				return ActiveFilterValue == null;
			}
			if(filterValue.IsFilterByQuery)
				return filterValue.Query.ToLower() == this.activeFilterQuery;
			if(ActiveFilterValue == null) return false;
			try {
				object value = filterValue.Value;
				if (value.GetType() != ActiveFilterValue.GetType()) {
					value = Convert.ChangeType(filterValue.Value, this.dataType);
				}
				return Object.Equals(value, ActiveFilterValue);			
			} catch {
				return false;
			}
		}
		#region IHtmlFilterPopupContainerOwner Members
		int IHtmlFilterPopupContainerOwner.ItemCount {
			get {
				return FilterValues.Count;
			} 
		}
		FilterValue IHtmlFilterPopupContainerOwner.GetFilterValue(int index) {
			if(index < 0 || index >= ((IHtmlFilterPopupContainerOwner)this).ItemCount) return null;
			return FilterValues[index];
		}
		string IHtmlFilterPopupContainerOwner.GetMouseDownScript(int index) {
			return RenderHelper.Scripts.GetColumnFilterApply(Column);
		}
		string IHtmlFilterPopupContainerOwner.GetMouseOutScript(int index) {
			return RenderHelper.Scripts.GetColumnFilterItemMouseOut();
		}
		string IHtmlFilterPopupContainerOwner.GetMouseOverScript(int index) {
			return RenderHelper.Scripts.GetColumnFilterItemMouseOver();
		}
		AppearanceStyle IHtmlFilterPopupContainerOwner.GetItemStyle(int index) {
			if(IsFilterValueActive(FilterValues[index]))
				return RenderHelper.GetFilterPopupSelectedItemStyle();
			return RenderHelper.GetFilterPopupItemStyle();
		}
		AppearanceStyle IHtmlFilterPopupContainerOwner.ActiveItemStyle { get { return RenderHelper.GetFilterPopupActiveItemStyle(); } }
		void IHtmlFilterPopupContainerOwner.AppendDefaultDXClassName(WebControl control) {
			RenderHelper.AppendDefaultDXClassName(control);
		}
		#endregion
	}
}
