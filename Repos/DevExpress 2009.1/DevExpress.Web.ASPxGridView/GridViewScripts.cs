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
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class ASPxGridViewScripts {
		ASPxGridView grid;
		public ASPxGridViewScripts(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected string Name { get { return Grid.ClientID; } }
		protected bool IsDisabled { get { return !grid.IsEnabled(); } }
		public string GetContextMenu(string objectType, int index) {
			if(IsDisabled || string.IsNullOrEmpty(Grid.ClientSideEvents.ContextMenu)) return string.Empty;
			return string.Format("aspxGVContextMenu('{0}','{1}',{2},event); return false;", Name, objectType, index);
		}
		public string GetExpandRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVExpandRow('{0}',{1})", Name, visibleIndex);
		}
		public string GetCollapseRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVCollapseRow('{0}',{1})", Name, visibleIndex);
		}
		public string GetShowDetailRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVShowDetailRow('{0}',{1})", Name, visibleIndex);
		}
		public string GetHideDetailRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVHideDetailRow('{0}',{1})", Name, visibleIndex);
		}
		public string GetPagerOnClickFunction(string id) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVPagerOnClick('{0}','{1}');", Name, id);
		}
		public string GetFilterOnKeyPressFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVFilterKeyPress('{0}',s,event);", Name);
		}
		public string GetFilterOnSpecKeyPressFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVFilterSpecKeyPress('{0}',s,event);", Name);
		}
		public string GetFilterOnChangedFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVFilterChanged('{0}',s);", Name);
		}
		public string GetShowFilterControl() {
			return string.Format("aspxGVShowFilterControl('{0}');", Name);
		}
		public string GetApplyFilterControl() {
			return string.Format("aspxGVApplyFilterControl('{0}');", Name);
		}
		public string GetCloseFilterControl() {
			return string.Format("aspxGVCloseFilterControl('{0}');", Name);
		}
		public string GetSetFilterEnabledForCheckBox() {
			return string.Format("aspxGVSetFilterEnabled('{0}', this.checked);", Name);
		}
		public string GetStartEditFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVStartEditRow('{0}',{1});", Name, visibleIndex);
		}
		public string GetUpdateEditFunction() { return GetUpdateEditFunction(string.Empty, -1); }
		public string GetUpdateEditFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVUpdateEdit('{0}');", Name);
		}
		public string GetCancelEditFunction() { return GetCancelEditFunction(string.Empty, -1); }
		public string GetCancelEditFunction(int visibleIndex) { return GetCancelEditFunction(string.Empty, visibleIndex); }
		public string GetCancelEditFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVCancelEdit('{0}');", Name);
		}
		public string GetAddNewRowFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVAddNewRow('{0}');", Name);
		}
		public string GetDeleteRowFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVDeleteRow('{0}',{1});", Name, visibleIndex);
		}
		public string GetClearFilterFunction() { return GetClearFilterFunction(string.Empty, -1); }
		public string GetClearFilterFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVClearFilter('{0}');", Name);
		}
		public string GetSelectRowFunction(int visibleIndex) {
			return GetSelectRowFunction(string.Empty, visibleIndex);
		}
		public string GetSelectRowFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVSelectRow('{0}',{1},this);", Name, visibleIndex);
		}
		public string GetCustomButtonFunction(string id, int visibleIndex) {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVCommandCustomButton('{0}','{1}',{2});", Name, id, visibleIndex);
		}
		public string GetShowParentRowsWindowFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVShowParentRows('{0}', event, this);", Name);
		}
		public string GetHideParentRowsWindowFunction(bool always) {
			if(IsDisabled) return string.Empty;
			string evt = always ? string.Empty : ", event";
			return string.Format("aspxGVShowParentRows('{0}'{1});", Name, evt);
		}
		public string GetMainTableClickFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVTableClick('{0}', event);", Name);
		}
		public string GetMainTableDblClickFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVTableDblClick('{0}', event);", Name);
		}
		public string GetHeaderColumnClick() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVHeaderMouseDown('{0}', this, event);", Name);
		}
		public string GetHeaderColumnResizing() {
			if(IsDisabled) return string.Empty;
			return string.Format("aspxGVHeaderColumnResizing('{0}', this, event);", Name);
		}
		public string GetCustomizationWindowCloseUpHandler() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, event) {{ aspxGVCustWindowCloseUp('{0}'); }}", Name);
		}
		public string GetShowFilterPopup(string headerId, GridViewColumn column) {
			return string.Format("aspxGVShowFilterPopup('{0}', '{1}', {2}, this, event);", Name, headerId, column.Index);
		}
		public string GetColumnFilterApply(GridViewColumn column) {
			return string.Format("aspxGVApplyFilterPopup('{0}', {1}, this);", Name, column.Index);
		}
		public string GetColumnFilterItemMouseOver() {
			return string.Format("aspxGVFilterPopupItemOver('{0}', this);", Name);
		}
		public string GetColumnFilterItemMouseOut() {
			return string.Format("aspxGVFilterPopupItemOut('{0}', this);", Name);
		}
		public string GetFilterRowMenuImageClick(int columnIndex) {
			return string.Format("aspxGVFilterRowMenu('{0}',{1},this)", Name, columnIndex);
		}
		public string GetFilterRowMenuItemClick() {
			return string.Format("function(s,e){{aspxGVFilterRowMenuClick('{0}',e)}}", Name);
		}
		public string GetAccessibleSortClick(int columnIndex) {
			return string.Format("aspxGVSort('{0}',{1})", Name, columnIndex);
		}
	}
}
