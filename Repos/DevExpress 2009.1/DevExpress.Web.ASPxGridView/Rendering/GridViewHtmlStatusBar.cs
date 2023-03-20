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
namespace DevExpress.Web.ASPxGridView.Rendering {
	class GridViewHtmlStatusBar : InternalTable {
		ASPxGridView grid;
		public GridViewHtmlStatusBar(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected TableRow MainRow { get { return Rows[0]; } }
		protected TableCell TemplateContainer { get { return MainRow.Cells[0]; } }
		protected TableCell LoadingContainer { get { return MainRow.Cells.Count > 1 ? MainRow.Cells[1] : null; } }
		protected override void CreateControlHierarchy() {
			Rows.Add(RenderUtils.CreateTableRow());
			MainRow.Cells.Add(RenderUtils.CreateTableCell());
			if(!RenderHelper.AddStatusBarTemplateControl(TemplateContainer)) {
				TemplateContainer.Text = "&nbsp;";
			}
			if(Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar) {
				MainRow.Cells.Add(RenderUtils.CreateTableCell());
				LoadingContainer.ID = RenderHelper.GetLoadingContainerID();
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetStatusBarStyle().AssignToControl(this, true);
			Width = Unit.Percentage(100);
			TemplateContainer.Width = Unit.Percentage(100);
			RenderHelper.AppendDefaultDXClassName(MainRow);
			base.PrepareControlHierarchy();
		}
	}
}
