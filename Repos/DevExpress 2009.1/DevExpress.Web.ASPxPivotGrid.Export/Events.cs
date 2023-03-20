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

using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid.Web {
	public class WebCustomExportHeaderEventArgs : CustomExportHeaderEventArgs {
		DevExpress.Web.ASPxPivotGrid.PivotGridField field;
		public WebCustomExportHeaderEventArgs(IVisualBrick brick, PivotHeaderViewInfoBase headerViewInfo, 
					DevExpress.Web.ASPxPivotGrid.PivotGridField field)
			: base(brick, headerViewInfo) {
			this.field = field;
		}
		public new DevExpress.Web.ASPxPivotGrid.PivotGridField Field { get { return field; } }
	}
	public class WebCustomExportFieldValueEventArgs : CustomExportFieldValueEventArgs {
		DevExpress.Web.ASPxPivotGrid.PivotGridField field;
		public WebCustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldsAreaCellViewInfoBase viewInfo, 
					DevExpress.Web.ASPxPivotGrid.PivotGridField field)
			: base(brick, viewInfo) {
			this.field = field;
		}
		public new DevExpress.Web.ASPxPivotGrid.PivotGridField Field { get { return field; } }
	}
	public class WebCustomExportCellEventArgs : CustomExportCellEventArgs {
		DevExpress.Web.ASPxPivotGrid.PivotGridField columnField, rowField, dataField;
		public WebCustomExportCellEventArgs(IVisualBrick brick, PivotCellViewInfo cellViewInfo,
			DevExpress.Web.ASPxPivotGrid.PivotGridField columnField, DevExpress.Web.ASPxPivotGrid.PivotGridField rowField,
			DevExpress.Web.ASPxPivotGrid.PivotGridField dataField)
			: base(brick, cellViewInfo) {
			this.columnField = columnField;
			this.rowField = rowField;
			this.dataField = dataField;
		}
		public new DevExpress.Web.ASPxPivotGrid.PivotGridField ColumnField { get { return columnField; } }
		public new DevExpress.Web.ASPxPivotGrid.PivotGridField RowField { get { return rowField; } }
		public new DevExpress.Web.ASPxPivotGrid.PivotGridField DataField { get { return dataField; } }
	}
}
