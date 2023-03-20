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
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxPopupControl;
using DevExpress.Web.ASPxGridView.Rendering;
namespace DevExpress.Web.ASPxGridView.Rendering {
	[ToolboxItem(false)]
	public class GridViewHtmlParentRowsWindow : DevExpress.Web.ASPxPopupControl.ASPxPopupControl {
		ASPxGridViewRenderHelper renderHelper;
		WebDataProxy dataProxy;
		Table table;
		public GridViewHtmlParentRowsWindow(ASPxGridViewRenderHelper renderHelper, WebDataProxy dataProxy) {
			this.EnableViewState = false;
			this.ShowOnPageLoad = false;
			this.renderHelper = renderHelper;
			this.dataProxy = dataProxy;
			this.ShowHeader = false;
			this.PopupVerticalAlign = PopupVerticalAlign.Above;
			this.PopupHorizontalOffset = -1;
			ContentStyle.Paddings.Padding = Unit.Pixel(0);
			ParentSkinOwner = Grid;
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected WebDataProxy DataProxy { get { return dataProxy; } }
		protected Table Table { get { return table; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetParentRowsWindowId();
			Controls.Clear();
			this.table = CreateTable();
			Controls.Add(Table);
			base.CreateControlHierarchy();
		}
		protected virtual Table CreateTable() {
			Table table = RenderUtils.CreateTable();
			List<int> list = DataProxy.GetParentRows();
			for(int i = 0; i < list.Count; i++) {
				table.Rows.Add(new GridViewTableParentGroupRow(RenderHelper, list[i], GridViewLastRowBottomBorder.RequireBorder));
			}
			return table;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Attributes["onmouseout"] = Scripts.GetHideParentRowsWindowFunction(true);
			this.Width = Unit.Pixel(10);
		}
	}
	public class GridViewTableParentGroupRow : GridViewTableGroupRow {
		public GridViewTableParentGroupRow(ASPxGridViewRenderHelper renderHelper, int visibleIndex, GridViewLastRowBottomBorder lastRowBottomBorder)
			: base(renderHelper, visibleIndex, true, lastRowBottomBorder) { }
		protected override int GetColSpanCount() {
			return RenderHelper.GroupCount - GroupLevel;
		}
		public override GridViewLastRowBottomBorder LastRowBottomBorder { get { return GridViewLastRowBottomBorder.RegularRow; } }
	}
}
