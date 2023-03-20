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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlFilterPopup : PopupFilterWindow {
		PivotGridWebData data;
		TableCell okCell;
		TableCell cancelCell;
		Button okButton;
		Button cancelButton;
		public PivotGridHtmlFilterPopup(PivotGridWebData data) : base(data.Owner) {
			this.data = data;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.okButton = null;
			this.cancelButton = null;
			this.okCell = null;
			this.cancelCell = null;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetFilterButtonStyle().AssignToControl(okButton, true);
			Data.GetFilterButtonStyle().AssignToControl(cancelButton, true);
			Paddings buttonPanelPaddings = Data.GetFilterButtonPanelPaddings();
			RenderUtils.SetPaddings(okCell, buttonPanelPaddings);
			RenderUtils.SetPaddings(cancelCell, buttonPanelPaddings);
			buttonPanelPaddings.PaddingLeft = Data.GetFilterButtonPanelSpacing();
		}
		protected override void CreateButtonPanelCells(TableRow row) {
			this.okButton = AddButton(row, PivotGridLocalizer.GetString(PivotGridStringId.FilterOk), "pivotGrid_ApplyFilter('" + Data.ClientID + "');  return false;", out this.okCell);
			this.okButton.ID = PivotGridWebData.ElementName_FilterPopupWindowOKButton;
			this.cancelButton = AddButton(row, PivotGridLocalizer.GetString(PivotGridStringId.FilterCancel), "pivotGrid_HideFilter('" + Data.ClientID + "'); return false;", out this.cancelCell);
		}
		Button AddButton(TableRow row, string caption, string onclick, out TableCell cell) {
			cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			Button button = CreateButton(caption, onclick);
			cell.Controls.Add(button);
			return button;
		}
		Button CreateButton(string caption, string onclick) {
			Button button = new Button();
			button.Text = caption;
			button.OnClientClick = onclick;
			return button;
		}
		protected override AppearanceStyle GetFilterWindowStyle() { return Data.GetFilterWindowStyle(); }
		protected override AppearanceStyle GetFilterItemsAreaStyle() { return Data.GetFilterItemsAreaStyle(); }
		protected override AppearanceStyle GetFilterButtonPanelStyle() { return Data.GetFilterButtonPanelStyle(); }
		protected override ImageProperties GetFilterWindowSizeGripImage() { return PivotGrid.RenderHelper.GetFilterWindowSizeGripImage(); }
	}
	public class PivotGridHtmlFilterPopupContent : ASPxInternalWebControl {
		PivotGridField field;
		Table table;
		PivotGridFilterItems filterItems;
		PivotGridWebData data;
		public PivotGridHtmlFilterPopupContent(PivotGridWebData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			this.filterItems = Data.CreatePivotGridFilterItems(field);
		}
		protected PivotGridWebData Data { get { return data; } }
		public string GetCallBackString() {
			EnsureChildControls();
			return "F|" + FilterItems.StatesString + '|' + FilterItems.PersistentString + '|' + Field.Index.ToString() + '|' + RenderUtils.GetRenderResult(this);
		}
		protected PivotGridField Field { get { return field; } }
		protected internal PivotGridFilterItems FilterItems { get { return filterItems; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.Owner as ASPxPivotGrid; } }
		protected override void ClearControlFields() {
			this.table = null;
		}
		protected bool IsShowAllChecked() {
			for(int i = 0; i < FilterItems.Count; i++)
				if(!FilterItems[i].IsChecked) return false;
			return true;
		}
		protected override void CreateControlHierarchy() {
			table = RenderUtils.CreateTable();
			Controls.Add(table);
			FilterItems.CreateItems();
			for(int i = -1; i < FilterItems.Count; i++) {
				TableRow row = RenderUtils.CreateTableRow();
				table.Rows.Add(row);
				TableCell cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);				
				CheckBox checkBox = new CheckBox();
				checkBox.InputAttributes["value"] = i.ToString();
				checkBox.Checked = i < 0 ? IsShowAllChecked() : FilterItems[i].IsChecked;
				checkBox.Attributes.Add("onclick", string.Format("pivotGrid_FilterValueChanged('{0}', {1});", PivotGrid.ClientID, i));
				checkBox.ID = PivotGrid.ClientID + "FTRI" + (i < 0 ? "All" : i.ToString());
				cell.Controls.Add(checkBox);
				cell = RenderUtils.CreateTableCell();
				Data.GetFilterItemStyle().AssignToControl(cell);
				row.Cells.Add(cell);
				cell.Controls.Add(RenderUtils.CreateLiteralControl("<label for=\"" + checkBox.ID + "\">" + (i < 0 ? PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll) : Data.GetPivotFieldValueText(FilterItems.Field, FilterItems[i].Value)) + "</label>"));
			}
		}
   }
}
