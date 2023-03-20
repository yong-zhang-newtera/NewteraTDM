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
using DevExpress.Web.ASPxEditors.Internal;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public abstract class GridViewHtmlPagerPanelBase : ASPxInternalWebControl {
		public static ASPxGridViewPager CreatePager(ASPxGridView grid, string id) {
			ASPxGridViewPager pager = new ASPxGridViewPager(grid);
			pager.ID = id;
			return pager;
		}
		ASPxGridViewRenderHelper renderHelper;
		WebControl pagerPanel;
		public GridViewHtmlPagerPanelBase(ASPxGridViewRenderHelper renderHelper) {
			this.renderHelper = renderHelper;
		}
		protected ASPxGridViewPager CreatePager() {
			return CreatePager(Grid, PagerId);
		}
		protected ASPxGridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected WebControl PagerPanel { get { return pagerPanel; } }
		protected abstract string PagerId { get; }
		protected abstract GridViewPagerBarPosition Position { get; }
		protected override void CreateChildControls() {
			this.pagerPanel = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(PagerPanel);
			if (!RenderHelper.AddPagerBarTemplateControl(PagerPanel, Position, PagerId)) {
				PagerPanel.Controls.Add(CreatePager());
			}
		}
	}
	public class GridViewHtmlTopPagerPanel : GridViewHtmlPagerPanelBase {
		public GridViewHtmlTopPagerPanel(ASPxGridViewRenderHelper renderHelper) : base(renderHelper) { }
		protected override string PagerId { get { return RenderHelper.GetTopPagerId(); } }
		protected override GridViewPagerBarPosition Position { get { return GridViewPagerBarPosition.Top; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPagerTopPanelStyle().AssignToControl(PagerPanel, true);
		}
	}
	public class GridViewHtmlBottomPagerPanel : GridViewHtmlPagerPanelBase {
		public GridViewHtmlBottomPagerPanel(ASPxGridViewRenderHelper renderHelper) : base(renderHelper) { }
		protected override string PagerId { get { return RenderHelper.GetBottomPagerId(); } }
		protected override GridViewPagerBarPosition Position { get { return GridViewPagerBarPosition.Bottom; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPagerBottomPanelStyle().AssignToControl(PagerPanel, true);
		}
	}
}
