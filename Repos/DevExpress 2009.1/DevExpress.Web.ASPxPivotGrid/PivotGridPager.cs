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
using System.ComponentModel;
using DevExpress.Web.ASPxPager;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
namespace DevExpress.Web.ASPxPivotGrid {
	[ToolboxItem(false)]
	public class ASPxPivotGridPager : ASPxPagerBase {
		ASPxPivotGrid pivotGrid;
		PivotGridHtmlTable htmlTable;
		protected PivotGridHtmlTable HtmlTable { get { return htmlTable; } }
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		public ASPxPivotGridPager(PivotGridHtmlTable htmlTable, ASPxPivotGrid pivotGrid)
			: base() {
			this.htmlTable = htmlTable;
			this.pivotGrid = pivotGrid;
			EnableViewState = false;
			ParentSkinOwner = PivotGrid;
		}
		public override int PageIndex { get { return PivotGrid.OptionsPager.PageIndex; } }
		public override int ItemCount { get { return PivotGrid.Data.VisualItems.UnpagedRowCount; } }
		public override int ItemsPerPage { get { return PivotGrid.OptionsPager.RowsPerPage; } }
		public override int PageCount {
			get {
				if(Data.OptionsView.ShowRowGrandTotals && ItemCount == ItemsPerPage + 1) return 1;
				return base.PageCount;
			}
		}
		protected override string GetItemElementOnClick(string id) {
			if(Data != null)
				return Data.GetPagerOnClick(id);
			return base.GetItemElementOnClick(id);
		}
		internal new PagerSettingsEx PagerSettings { get { return base.PagerSettings; } }
		protected override void PrepareControlHierarchy() {
			if(Page != null) 
				ApplyStyleSheetSkin(Page);
			Styles.Assign(PivotGrid.StylesPager);
			base.PrepareControlHierarchy();
		}
	}
}
