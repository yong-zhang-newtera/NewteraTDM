#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraPivotGrid                                 }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid {
	public enum PivotGridHitTest {None, HeadersArea, Cell, Value}
	public enum PivotGridHeaderHitTest { None, Filter }
	public class PivotGridHeadersAreaHitInfo {
		PivotHeadersViewInfoBase headersViewInfo;
		PivotGridField field;
		PivotGridHeaderHitTest headerHitTest;
		public PivotGridHeadersAreaHitInfo(PivotHeadersViewInfoBase headersViewInfo, PivotGridField field, PivotGridHeaderHitTest headerHitTest) {
			this.headersViewInfo = headersViewInfo;
			this.field = field;
			this.headerHitTest = headerHitTest;
		}
		protected PivotHeadersViewInfoBase HeadersViewInfo { get { return headersViewInfo; } }
		public PivotGridField Field { get { return field; } }
		public PivotArea Area { get { return HeadersViewInfo.Area; } }
		public PivotGridHeaderHitTest HeaderHitTest { get { return headerHitTest; } }
	}
	public class PivotGridHitInfo {
		PivotGridHitTest hitTest;
		Point hitPoint;
		PivotCellEventArgs cellInfo;
		PivotFieldValueEventArgs valueInfo;
		PivotGridHeadersAreaHitInfo headersAreaInfo;
		public PivotGridHitInfo(Point hitPoint) {
			this.hitPoint = hitPoint;
			this.hitTest = PivotGridHitTest.None;
			this.cellInfo = null;
			this.valueInfo = null;
			this.headersAreaInfo = null;
		}
		public PivotGridHitInfo(PivotCellViewInfo cellViewInfo, Point hitPoint) : this(hitPoint) {
			this.hitTest = PivotGridHitTest.Cell;
			this.cellInfo = new PivotCellEventArgs(cellViewInfo);
		}
		public PivotGridHitInfo(PivotFieldsAreaCellViewInfo fieldCellViewInfo, Point hitPoint) : this(hitPoint) {
			this.hitTest = PivotGridHitTest.Value;
			this.valueInfo = new PivotFieldValueEventArgs(fieldCellViewInfo);
		}
		public PivotGridHitInfo(PivotHeadersViewInfoBase headersViewInfo, PivotGridField field, PivotGridHeaderHitTest headerHitTest, Point hitPoint) : this(hitPoint) {
			this.hitTest = PivotGridHitTest.HeadersArea;
			this.headersAreaInfo = new PivotGridHeadersAreaHitInfo(headersViewInfo, field, headerHitTest);
		}
		public PivotGridHitTest HitTest { get { return hitTest; } }
		public Point HitPoint { get { return hitPoint; } }
		public PivotCellEventArgs CellInfo { get { return cellInfo; } }
		public PivotFieldValueEventArgs ValueInfo { get { return valueInfo; } }
		public PivotGridHeadersAreaHitInfo HeadersAreaInfo { get { return headersAreaInfo; } }
		public PivotGridField HeaderField { get { return HeadersAreaInfo != null ? HeadersAreaInfo.Field : null; } }
	}
}
