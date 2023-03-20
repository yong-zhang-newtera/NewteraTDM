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
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPopupControl;
using DevExpress.Web.ASPxEditors;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public interface IHtmlFilterPopupOwner {
		ASPxWebControl MainControl { get; }
		string FilterPopupId { get; }
		AppearanceStyle FilterWindowStyle { get; }
		AppearanceStyle FilterItemsAreaStyle { get; }
		AppearanceStyle FilterButtonPanelStyle { get;  }
		ImageProperties GetWindowResizer(Page page);
		Unit DefaultHeight { get; }
	}
	public interface IHtmlFilterPopupContainerOwner : IHtmlFilterPopupOwner {
		int ItemCount { get; }
		FilterValue GetFilterValue(int index);
		string GetMouseDownScript(int index);
		string GetMouseOverScript(int index);
		string GetMouseOutScript(int index);
		AppearanceStyle GetItemStyle(int index);
		AppearanceStyle ActiveItemStyle { get; }
		void AppendDefaultDXClassName(WebControl control);
	}
	[ToolboxItem(false)]
	public class GridViewHtmlFilterPopup : PopupFilterWindow {
		IHtmlFilterPopupOwner filterOwner;
		public GridViewHtmlFilterPopup(IHtmlFilterPopupOwner filterOwner) : base(filterOwner.MainControl) {
			this.filterOwner = filterOwner;
			EnableViewState = false;
		}
		protected IHtmlFilterPopupOwner FilterOwner { get { return filterOwner; } }
		protected override AppearanceStyle GetFilterWindowStyle() { return FilterOwner.FilterWindowStyle; }
		protected override AppearanceStyle GetFilterItemsAreaStyle() { return FilterOwner.FilterItemsAreaStyle; }
		protected override AppearanceStyle GetFilterButtonPanelStyle() { return FilterOwner.FilterButtonPanelStyle; }
		protected override ImageProperties GetFilterWindowSizeGripImage() { return FilterOwner.GetWindowResizer(Page); }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetStyleStringAttribute(Content, "overflow-x", "hidden", true);
			MainTable.Width = 150;
			Content.Width = Unit.Percentage(100); 
			if (!FilterOwner.DefaultHeight.IsEmpty) {
				RenderUtils.SetStyleUnitAttribute(Content, "Height", FilterOwner.DefaultHeight, false);
			}
		}
	}
	[ToolboxItem(false)]
	public class GridViewHtmlFilterContainer : DevExpress.Web.ASPxClasses.ASPxWebControl {
		IHtmlFilterPopupContainerOwner owner;
		Table table;
		TableRow activeStyleRow;
		public GridViewHtmlFilterContainer(IHtmlFilterPopupContainerOwner owner) : base(owner.MainControl) {
			this.owner = owner;
		}
		protected IHtmlFilterPopupContainerOwner Owner { get { return owner; } }
		protected Table Table { get { return table; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.table = RenderUtils.CreateTable();
			Controls.Add(Table);
			for(int i = 0; i < Owner.ItemCount; i++) {
				AddItem(i);
			}
			this.activeStyleRow = AddActiveStyleRow();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Table.Width = Unit.Percentage(100);
			Owner.ActiveItemStyle.AssignToControl(this.activeStyleRow);
			this.activeStyleRow.Height = Unit.Pixel(0);
		}
		protected void AddItem(int index) {
			Table.Rows.Add(new GridViewHtmlFilterContainerTableRow(Owner, index));
		}
		protected TableRow AddActiveStyleRow() {
			TableRow row = RenderUtils.CreateTableRow();
			Table.Rows.Add(row);
			return row;
		}
		public new void EnsureChildControls() {
			base.EnsureChildControls();
		}
	}
	public class GridViewHtmlFilterContainerTableRow : InternalTableRow {
		IHtmlFilterPopupContainerOwner owner;
		int index;
		public GridViewHtmlFilterContainerTableRow(IHtmlFilterPopupContainerOwner owner, int index) {
			this.owner = owner;
			this.index = index;
		}
		public IHtmlFilterPopupContainerOwner Owner { get { return owner; } }
		public int Index { get { return index; } }
		protected FilterValue Value { get { return Owner.GetFilterValue(Index); } }
		protected override void CreateChildControls() {
			Cells.Add(RenderUtils.CreateTableCell());
			Cells[0].Text = Value.DisplayText;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Owner.GetItemStyle(Index).AssignToControl(this, true);
			Owner.AppendDefaultDXClassName(Cells[0]);
			Attributes["filterValue"] = Value.HtmlValue;
			Attributes["onMouseDown"] = Owner.GetMouseDownScript(Index);
			Attributes["onMouseOver"] = Owner.GetMouseOverScript(Index);
			Attributes["onMouseOut"] = Owner.GetMouseOutScript(Index);
		}
	}
}
