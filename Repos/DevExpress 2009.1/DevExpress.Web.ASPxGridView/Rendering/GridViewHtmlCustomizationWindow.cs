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
using System.Collections;
namespace DevExpress.Web.ASPxGridView.Rendering {
	[ToolboxItem(false)]
	public class GridViewHtmlCustomizationWindow : DevExpress.Web.ASPxPopupControl.ASPxPopupControl {
		ASPxGridView grid;
		Table maintable;
		WebControl contentDiv;
		public GridViewHtmlCustomizationWindow(ASPxGridView grid)
			: base(grid) {
			this.grid = grid;
			ShowOnPageLoad = false;
			ParentSkinOwner = Grid;
		}
		public string CloseUp {
			get { return ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp; }
			set { ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp = value; }
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected GridViewColumnCollection Columns { get { return Grid.Columns; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected Table MainTable { get { return maintable; } }
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected Unit GetWidth() {
			if(Grid.SettingsCustomizationWindow.Width.IsEmpty)
				return ASPxGridViewCustomizationWindowSettings.DefaultWidth;
			return Grid.SettingsCustomizationWindow.Width;
		}
		protected Unit GetHeight() {
			if(Grid.SettingsCustomizationWindow.Height.IsEmpty)
				return ASPxGridViewCustomizationWindowSettings.DefaultHeight;
			return Grid.SettingsCustomizationWindow.Height;
		}
		protected override Paddings GetContentPaddings(PopupWindow window) {
			Paddings paddings = new Paddings();
			paddings.Padding = Unit.Pixel(0);
			return paddings;
		}
		protected override void ClearControlFields() {
			this.maintable = null;
			this.contentDiv = null;
		}
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetCustomizationWindowId();
			Font.CopyFrom(Grid.Font);
			HeaderText = Grid.SettingsText.GetCustomizationWindowCaption();
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			this.contentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.maintable = RenderUtils.CreateTable();
			CreateHeaders();
			ContentDiv.Controls.Add(MainTable);
			Controls.Clear();
			Controls.Add(ContentDiv);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			CloseButtonImage.CopyFrom(Grid.Images.CustomizationWindowClose);
			ControlStyle.CopyFrom(Grid.Styles.CustomizationWindow);
			CloseButtonStyle.CopyFrom(Grid.Styles.CustomizationWindowCloseButton);
			HeaderStyle.CopyFrom(Grid.Styles.CustomizationWindowHeader);			
			ContentStyle.CopyFrom(Grid.Styles.CustomizationWindowContent);
			base.PrepareControlHierarchy();
			RenderHelper.GetCustomizationStyle().AssignToControl(ContentDiv);
			Width = GetWidth();
			MainTable.Width = Unit.Percentage(100);
			MainTable.GridLines = GridLines.None;
			MainTable.CellSpacing = 3;
			MainTable.CellPadding = 0;
			ContentDiv.Height = GetHeight();
			ContentDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
			ContentDiv.Style[HtmlTextWriterStyle.OverflowY] = "auto";
		}
		void CreateHeaders() {
			List<GridViewColumn> headers = new List<GridViewColumn>();
			for(int i = 0; i < Columns.Count; i++) {
				GridViewColumn column = Columns[i];
				if(column.Visible || !column.ShowInCustomizationForm) continue;
				GridViewDataColumn dc = column as GridViewDataColumn;
				if(dc != null && dc.GroupIndex > -1) continue;
				headers.Add(Columns[i]);
			}
			headers.Sort(new Comparison<GridViewColumn>(SortHeaderByName));
			foreach(GridViewColumn col in headers) {
				CreateHeader(col);
			}
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			row.Cells.Add(RenderUtils.CreateTableCell());
		}
		int SortHeaderByName(GridViewColumn col1, GridViewColumn col2) {
			if(col1 == col2) return 0;
			int res = Comparer.Default.Compare(col1.ToString(), col2.ToString());
			if(res == 0) res = Comparer.Default.Compare(col1.Index, col2.Index);
			return res;
		}
		void CreateHeader(GridViewColumn column) {
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			TableCell headerCell = new GridViewTableHeaderCell(RenderHelper, column, GridViewHeaderLocation.Customization, false);
			row.Cells.Add(headerCell);
		}
	}
}
