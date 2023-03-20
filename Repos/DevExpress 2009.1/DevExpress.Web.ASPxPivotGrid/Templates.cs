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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Data.PivotGrid;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridHeaderTemplateContainer : TemplateContainerBase {
		[Description("Gets a field for which the template's content is being instantiated.")]
		public PivotGridField Field {
			get { return DataItem as PivotGridField; }
		}
		public PivotGridHeaderTemplateContainer(PivotGridField field)
			: base(field.Index, field) {
		}
	}
	public class PivotGridEmptyAreaTemplateContainer : TemplateContainerBase {
		PivotArea area;
		public PivotGridEmptyAreaTemplateContainer(PivotArea area)
			: base((int)area, null) {
			this.area = area;
		}
		[Description("Gets the area for which the template's content is being instantiated.")]
		public PivotArea Area { get { return area; } }
	}
	public class PivotGridFieldValueTemplateItem {
		PivotGridField field;
		PivotFieldValueItem item;
		public PivotGridFieldValueTemplateItem(PivotGridField field, PivotFieldValueItem item) {
			this.field = field;
			this.item = item;
		}
		[Description("Gets the field being processed.")]
		public PivotGridField Field { get { return field; } }
		[Description("Gets an object that contains the necessary information on the processed field value.")]
		public PivotFieldValueItem ValueItem { get { return item; } }
		[Description("Gets the settings, which define the image displayed within the corresponding field value in the collapsed state.")]
		public ImageProperties CollaspedImage { get { return PivotGrid.RenderHelper.GetFieldValueCollapsedImage(ValueItem.IsCollapsed); } }
		[Description("Gets the java script code that realizes a field value's expanding/collapsing functionality.")]
		public string ImageOnClick { get { return Data.GetCollapsedImageOnClick(ValueItem); } }
		protected PivotGridWebData Data { get { return (PivotGridWebData)ValueItem.Data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
	}
	public class PivotGridFieldValueTemplateContainer : TemplateContainerBase {
		public PivotGridFieldValueTemplateContainer(PivotGridFieldValueTemplateItem item)
			: base(item.ValueItem.Index, item) {
		}
		[Description("Gets an object that contains the necessary information on the rendered field value.")]
		public PivotGridFieldValueTemplateItem Item {
			get { return DataItem as PivotGridFieldValueTemplateItem; }
		}
		[Description("Gets a field for which the template's content is being instantiated.")]
		public PivotGridField Field { get { return Item.Field; } }
		[Description("Gets an object that contains the necessary information on the rendered field value.")]
		public PivotFieldValueItem ValueItem { get { return Item.ValueItem; } }
		[Description("Gets the data value of the field value currently being rendered.")]
		public object Value { get { return ValueItem.Value; } }
		[Description("Gets the display text of the field value currently being rendered.")]
		public object Text { get { return ValueItem.Text; } }
		[Description("Gets the minimum row index (for row fields) or column index (for column fields) that corresponds to the rendered field value.")]
		public int MinIndex { get { return ValueItem.MinLastLevelIndex; } }
		[Description("Gets the maximum row index (for row fields) or column index (for column fields) that corresponds to the rendered field value.")]
		public int MaxIndex { get { return ValueItem.MaxLastLevelIndex; } }
	}
	public class PivotGridCellTemplateItem {
		PivotCellBaseEventArgs args;
		string text;
		public PivotGridCellTemplateItem(PivotGridCellItem cellItem, string text) {
			this.args = new PivotCellBaseEventArgs(cellItem);
			this.text = text;
		}
		[Description("Gets the display text of the cell currently being rendered.")]
		public string Text { get { return text; } }
		[Description("Gets the data field that identifies the column to which the processed cell belongs.")]
		public PivotGridField DataField { get { return args.DataField; } }
		[Description("Gets the visual index of the column that contains the processed cell.")]
		public int ColumnIndex { get { return args.ColumnIndex; } }
		[Description("Gets the visual index of the row that contains the processed cell.")]
		public int RowIndex { get { return args.RowIndex; } }
		[Description("Gets the index of the field to which the templated cell belongs.")]
		public int ColumnFieldIndex { get { return args.ColumnFieldIndex; } }
		[Description("This member supports the .NET Framework infrastructure and cannot be used directly from your code.")]
		public int RowFieldIndex { get { return args.RowFieldIndex; } }
		[Description("Gets the innermost column field to which the processed cell corresponds.")]
		public PivotGridField ColumnField { get { return args.ColumnField; } }
		[Description("Gets the innermost row field which corresponds to the processed cell.")]
		public PivotGridField RowField { get { return args.RowField; } }
		[Description("Gets the value of the cell currently being rendered.")]
		public object Value { get { return args.Value; } }
		[Description("Gets the summary value currently being processed.")]
		public PivotSummaryValue SummaryValue { get { return args.SummaryValue; } }
		[Description("Gets the type of the column which contains the processed cell.")]
		public PivotGridValueType ColumnValueType { get { return args.ColumnValueType; } }
		[Description("Gets the type of the row which contains the processed cell.")]
		public PivotGridValueType RowValueType { get { return args.RowValueType; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() { return args.CreateDrillDownDataSource(); }		
		public object GetFieldValue(PivotGridField field) { return args.GetFieldValue(field); }
		public bool IsOthersFieldValue(PivotGridField field) { return args.IsOthersFieldValue(field); }
		public bool IsFieldValueExpanded(PivotGridField field) { return args.IsFieldValueExpanded(field); }
		public bool IsFieldValueRetrievable(PivotGridField field) { return args.IsFieldValueRetrievable(field); }
		public PivotGridField[] GetColumnFields() { return args.GetColumnFields(); }
		public PivotGridField[] GetRowFields() { return args.GetRowFields(); }
		public object GetCellValue(PivotGridField dataField) { return args.GetCellValue(dataField); }
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return args.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetColumnGrandTotal(PivotGridField dataField) { return args.GetColumnGrandTotal(dataField); }
		public object GetColumnGrandTotal(object[] rowValues, PivotGridField dataField) { return args.GetColumnGrandTotal(rowValues, dataField); }
		public object GetRowGrandTotal(PivotGridField dataField) { return args.GetRowGrandTotal(dataField); }
		public object GetRowGrandTotal(object[] columnValues, PivotGridField dataField) { return args.GetRowGrandTotal(columnValues, dataField); }
		public object GetGrandTotal(PivotGridField dataField) { return args.GetGrandTotal(dataField); }
	}
	public class PivotGridCellTemplateContainer : TemplateContainerBase {
		public PivotGridCellTemplateContainer(PivotGridCellTemplateItem item) : base(-1, item) { 
		}
		[Description("Gets an object that contains the necessary information on the rendered cell.")]
		public PivotGridCellTemplateItem Item {
			get { return DataItem as PivotGridCellTemplateItem; }
		}
		[Description("Gets the data field that identifies the column to which the rendered cell belongs.")]
		public PivotGridField DataField { get { return Item.DataField; } }
		[Description("Gets the data value calculated for the cell currently being rendered.")]
		public object Value { get { return Item.Value; } }
		[Description("Gets the display text of the cell currently being rendered.")]
		public string Text { get { return Item.Text; } }
		[Description("Gets the innermost column field to which the rendered cell corresponds.")]
		public PivotGridField ColumnField { get { return Item.ColumnField; } }
		[Description("Gets the innermost row field which corresponds to the rendered cell.")]
		public PivotGridField RowField { get { return Item.RowField; } }
		public object GetFieldValue(PivotGridField field) { return Item.GetFieldValue(field); }
	}
}
