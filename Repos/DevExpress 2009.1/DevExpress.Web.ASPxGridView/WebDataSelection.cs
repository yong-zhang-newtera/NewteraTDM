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
using DevExpress.Data;
using System.Collections;
using System.IO;
using DevExpress.Data.IO;
using System.ComponentModel;
using System.Collections.Specialized;
namespace DevExpress.Web.Data {
	public class WebDataSelectionBase {
		Dictionary<object, bool> selected;
		WebDataProxy webData;
		bool isStoreSelected, selectionChangedOnLock = false;
		int lockSelection = 0;
		protected Dictionary<object, bool> Selected { get { return selected; } }
		protected WebDataProxy WebData { get { return webData; } }
		protected WebRowType GetRowType(int visibleIndex) { return WebData.GetRowType(visibleIndex); }
		protected object GetRowKeyValue(int visibleIndex) { return WebData.GetRowKeyValue(visibleIndex); }
		protected object GetListSourceRowKeyValue(int listSourceRowIndex) { return WebData.GetListSourceRowKeyValue(listSourceRowIndex); }
		protected bool IsStoreSelected { get { return isStoreSelected; } set { isStoreSelected = value; } }
		protected bool SelectionChangedOnLock { get { return selectionChangedOnLock; } }
		protected internal bool IsLockSelection { get { return lockSelection != 0; } }
		protected internal void BeginSelection() {
			this.selectionChangedOnLock = false;
			this.lockSelection++;
		}
		protected internal void CancelSelection() {
			this.lockSelection--;
		}
		protected internal void EndSelection() {
			if(--this.lockSelection == 0) {
				if(this.selectionChangedOnLock) {
					this.selectionChangedOnLock = false;
					OnSelectionChanged();
				}
			}
		}
		public WebDataSelectionBase(WebDataProxy webData) {
			this.webData = webData;
			this.selected = new Dictionary<object, bool>();
			this.isStoreSelected = true;
		}
		protected bool CanSelectRow(int visibleIndex) {
			return GetRowType(visibleIndex) == WebRowType.Data;
		}
		protected void AddRow(object keyValue) {
			if(keyValue == null) return;
			Selected[keyValue] = true;
			OnSelectionChanged();
		}
		protected void RemoveRow(object keyValue) {
			if(!Selected.ContainsKey(keyValue)) return;
			Selected.Remove(keyValue);
			OnSelectionChanged();
		}
		protected virtual void OnSelectionChanged() {
			this.selectionChangedOnLock = true;
			if(IsLockSelection) return;
			FireSelectionChanged();   
		}
		protected virtual void FireSelectionChanged() { }
		protected bool IsRowSelectedByKeyCore(object keyValue) {
			if(keyValue == null) return false;
			return IsStoreSelected ? Selected.ContainsKey(keyValue) : !Selected.ContainsKey(keyValue);
		}
		protected bool IsListSourceRowSelected(int listSourceRowIndex) {
			if(CountCore == 0) return false;
			return IsRowSelectedByKeyCore(GetListSourceRowKeyValue(listSourceRowIndex));
		}
		protected virtual void SetSelectionCore(object keyValue, bool selected) {
			if(IsRowSelectedByKeyCore(keyValue) == selected) return;
			if(!IsStoreSelected)
				selected = !selected;
			if(selected)
				AddRow(keyValue);
			else
				RemoveRow(keyValue);
		}
		protected internal virtual List<object> GetSelectedValues(string[] fieldNames, int visibleStartIndex, int visibleRowCountOnPage) {
			List<object> list = new List<object>();
			int selectedCount = CountCore;
			if(selectedCount == 0 || fieldNames.Length == 0) return list;
			if(fieldNames.Length == 1 && fieldNames[0] == WebData.KeyFieldName && IsStoreSelected) {
				list.AddRange(Selected.Keys);
				return list;
			}
			for(int n = 0; n < visibleRowCountOnPage; n++) {
				if(IsRowSelectedCore(n + visibleStartIndex)) list.Add(WebData.GetRowValues(n + visibleStartIndex, fieldNames));
			}
			if(list.Count == selectedCount) return list;
			list.Clear();
			WebData.DoOwnerDataBinding();
			for(int n = 0; n < WebData.ListSourceRowCount; n++) {
				if(IsListSourceRowSelected(n)) {
					list.Add(WebData.GetListSourceRowValues(n, fieldNames));
					if(list.Count == selectedCount) break;
				}
			}
			return list;
		}
		protected int CountCore {
			get {
				if(IsStoreSelected)
					return Selected.Count;
				return WebData.VisibleRowCount - Selected.Count;
			}
		}
		protected bool IsRowSelectedCore(int visibleIndex) {
			if(CountCore == 0) return false;
			if(!CanSelectRow(visibleIndex)) return false;
			return IsRowSelectedByKeyCore(GetRowKeyValue(visibleIndex));
		}
		protected void SetSelectionCore(int visibleIndex, bool selected) {
			if(!CanSelectRow(visibleIndex)) return;
			SetSelectionCore(GetRowKeyValue(visibleIndex), selected);
		}
		protected virtual void SelectAllCore() {
			if(WebData.VisibleRowCount == CountCore) return;
			Selected.Clear();
			if(WebData.IsFiltered) {
				WebData.DoOwnerDataBinding();
				AddAllRowsIntoSelectedList();
			} else {
				IsStoreSelected = false;
			}
			OnSelectionChanged();
		}
		void AddAllRowsIntoSelectedList() {
			BeginSelection();
			for(int i = 0; i < WebData.VisibleRowCount; i++) {
				SetSelectionCore(i, true);
			}
			CancelSelection();
		}
		protected void UnselectAllCore() {
			if(CountCore == 0) return;
			IsStoreSelected = true;
			Selected.Clear();
			OnSelectionChanged();
		}
		protected internal void SaveState(TypedBinaryWriter writer) {
			writer.WriteObject(IsStoreSelected);
			writer.WriteObject(Selected.Count);
			foreach(KeyValuePair<object, bool> entry in Selected) {
				writer.WriteTypedObject(entry.Key);
			}
		}
		protected void LoadStateCore(TypedBinaryReader reader) {
			BeginSelection();
			try {
				LoadStateFromStream(reader);
			} finally {
				CancelSelection();
			}
		}
		void LoadStateFromStream(TypedBinaryReader reader) {
			Selected.Clear();
			IsStoreSelected = reader.ReadObject<bool>();
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				AddRow(reader.ReadTypedObject());
			}
		}
	}
	public class WebDataSelection : WebDataSelectionBase {
		public WebDataSelection(WebDataProxy webData) : base(webData) { }
		protected override void FireSelectionChanged() {
			WebData.OnSelectionChanged();
		}
		[Description("Gets the number of selected rows within the ASPxGridView.")]
public int Count { get { return CountCore; } }
		public bool IsRowSelected(int visibleIndex) {  return IsRowSelectedCore(visibleIndex); }
		public bool IsRowSelectedByKey(object keyValue) { return IsRowSelectedByKeyCore(keyValue); }
		public void SelectRowByKey(object keyValue) { SetSelectionByKey(keyValue, true); }
		public void UnselectRowByKey(object keyValue) {  SetSelectionByKey(keyValue, false);  }
		public void SelectRow(int visibleIndex) { SetSelection(visibleIndex, true); }
		public void UnselectRow(int visibleIndex) { SetSelection(visibleIndex, false); }
		public void SetSelection(int visibleIndex, bool selected) { SetSelectionCore(visibleIndex, selected); }
		public void SetSelectionByKey(object keyValue, bool selected) { SetSelectionCore(keyValue, selected); }
		public void SelectAll() { SelectAllCore(); }
		public void UnselectAll() { UnselectAllCore(); }
		protected internal void LoadState(TypedBinaryReader reader, string pageSelectionResult) {
			if(reader != null) LoadStateCore(reader);
			WebData.BeginUseCachedProvider();
			try {
				BeginSelection();
				try {
					LoadStateFromPage(pageSelectionResult);
				}
				finally {
					EndSelection();
				}
			}
			finally {
				WebData.EndUseCachedProvider();
			}
		}
		void LoadStateFromPage(string pageSelection) {
			if(string.IsNullOrEmpty(pageSelection)) return;
			if(pageSelection == "U") {
				pageSelection = new string('F', WebData.VisibleRowCountOnPage);
			}
			if(pageSelection.Length < WebData.VisibleRowCountOnPage)
				pageSelection += new string('F', WebData.VisibleRowCountOnPage - pageSelection.Length);
			for(int i = 0; i < pageSelection.Length;  i ++) {
				SetSelection(WebData.VisibleStartIndex + i, pageSelection[i] == 'T');
			}
		}
	}
	public class WebDataDetailRows : WebDataSelectionBase {
		public WebDataDetailRows(WebDataProxy webData) : base(webData) { }
		protected override void FireSelectionChanged() {
			WebData.OnDetailRowsChanged();
		}
		[Description("Gets the number of expanded master rows displayed within all pages.")]
public int VisibleCount { get { return CountCore; } }
		public bool IsVisible(int visibleIndex) { return IsRowSelectedCore(visibleIndex); }
		public void ExpandRowByKey(object keyValue) { SetSelectionCore(keyValue, true); }
		public void CollapseRowByKey(object keyValue) { SetSelectionCore(keyValue, false); }
		public void ExpandRow(int visibleIndex) { SetSelectionCore(visibleIndex, true); }
		public void CollapseRow(int visibleIndex) { SetSelectionCore(visibleIndex, false); }
		public void ExpandAllRows() { SelectAllCore(); }
		public void CollapseAllRows() { UnselectAllCore(); }
		protected internal void LoadState(TypedBinaryReader reader) { LoadStateCore(reader); }
		bool AllowOnlyOneMasterRowExpanded { get { return WebData.Owner != null && WebData.Owner.AllowOnlyOneMasterRowExpanded; } }
		protected override void SetSelectionCore(object keyValue, bool selected) {
			if(selected && AllowOnlyOneMasterRowExpanded) {
				if(!IsRowSelectedByKeyCore(keyValue)) CollapseAllRows();
			}
			base.SetSelectionCore(keyValue, selected);
		}
		protected override void SelectAllCore() {
			if(AllowOnlyOneMasterRowExpanded) {
				SetSelectionCore(0, true);
				return;
			}
			base.SelectAllCore();
		}
	}
}
