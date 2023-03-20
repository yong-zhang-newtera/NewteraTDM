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
using DevExpress.Web.ASPxClasses;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Data;
using System.Collections;
using System.IO;
using DevExpress.Data.IO;
using System.Collections.ObjectModel;
using DevExpress.Web.ASPxPager;
using DevExpress.Web.ASPxPager.Internal;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxEditors;
namespace DevExpress.Web.ASPxGridView {
	[ToolboxItem(false)]
	public class ASPxGridViewPager : DevExpress.Web.ASPxPager.ASPxPagerBase {
		ASPxGridView grid;
		public ASPxGridViewPager(ASPxGridView grid)
			: base(grid) {
			this.grid = grid;
			EnableViewState = false;
			PagerSettings.Assign(Grid.SettingsPager);
			Styles.Assign(Grid.StylesPager);
			ParentSkinOwner = Grid;
		}
		[Description("For internal use.")]
		public override int PageCount { get { return Grid.DataProxy.PageCount; } }
		[Description("For internal use.")]
		public override int PageIndex { get { return Grid.DataProxy.PageIndex; } }
		[Description("For internal use.")]
		public override int ItemCount { get { return Grid.DataProxy.VisibleRowCount; } }
		[Description("For internal use.")]
		public override int ItemsPerPage { get { return Grid.DataProxy.PageSize; } }
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected ASPxGridViewScripts Scripts { get { return Grid.RenderHelper.Scripts; } }
		protected override string GetItemElementOnClick(string id) {
			return Scripts.GetPagerOnClickFunction(id);
		}
		protected override SEOTarget GetSEOTarget(params object[] args) {
			return new SEOTarget(RenderHelper.GetSEOID(), RenderHelper.SEO.SaveState((int)args[0]));
		}
		protected override bool HasContent() { return true; }
		protected override void PrepareControlHierarchy() {
			Font.CopyFrom(Grid.Font);
			if(Page != null) {
				ApplyStyleSheetSkin(Page);
			}
			base.PrepareControlHierarchy();
		}
	}
}
