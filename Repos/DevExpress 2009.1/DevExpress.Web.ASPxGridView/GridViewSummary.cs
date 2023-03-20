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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Data;
using DevExpress.Data;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxGridView.Design;
namespace DevExpress.Web.ASPxGridView {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxSummaryItem : CollectionItem, DevExpress.Utils.Design.ICaptionSupport {
		[Description("Gets or sets the aggregate function type."),
		DefaultValue(SummaryItemType.None), NotifyParentProperty(true)]
		public SummaryItemType SummaryType {
			get { return (SummaryItemType)GetIntProperty("SummaryType", (int)SummaryItemType.None); }
			set {
				if(SummaryType == value) return;
				SetIntProperty("SummaryType", (int)SummaryItemType.None, (int)value);
				OnSummaryChanged();
			}
		}
		[Description("Gets or sets the name of a data source field whose values are used for summary calculation."),
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(GridViewFieldConverter)), NotifyParentProperty(true)]
		public virtual string FieldName {
			get { return GetStringProperty("FieldName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("FieldName", string.Empty, value);
				OnSummaryChanged();
			}
		}
		[Description("Gets or sets the pattern used to format the summary value."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public virtual string DisplayFormat {
			get { return GetStringProperty("DisplayFormat", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("DisplayFormat", string.Empty, value);
				OnSummaryChanged();
			}
		}
		[Description("Gets or sets a value that specifies a column in whose footer or group rows the summary will be displayed."),
		DefaultValue(""), Localizable(false), TypeConverter(typeof(GridViewColumnsConverter)), NotifyParentProperty(true)]
		public virtual string ShowInColumn {
			get { return GetStringProperty("ShowInColumn", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("ShowInColumn", string.Empty, value);
				OnSummaryChanged();
			}
		}
		[Description("Gets or sets the column whose group footer cells should display summary values."),
		DefaultValue(""), Localizable(false), TypeConverter(typeof(GridViewColumnsConverter)), NotifyParentProperty(true)]
		public virtual string ShowInGroupFooterColumn {
			get { return GetStringProperty("ShowInGroupFooterColumn", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("ShowInGroupFooterColumn", string.Empty, value);
				OnSummaryChanged();
			}
		}
		protected internal bool IsShowInGroupRow { get { return string.IsNullOrEmpty(ShowInColumn) && string.IsNullOrEmpty(ShowInGroupFooterColumn); } }
		[Description("Gets or sets data associated with the summary item."),
		DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public string Tag {
			get { return GetStringProperty("Tag", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("Tag", string.Empty, value);
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ASPxSummaryItem item = source as ASPxSummaryItem;
			Tag = item.Tag;
			ShowInColumn = item.ShowInColumn;
			DisplayFormat = item.DisplayFormat;
			FieldName = item.FieldName;
			SummaryType = item.SummaryType;
		}
		protected string GetDisplayFormat() {
			if(string.IsNullOrEmpty(DisplayFormat)) return string.Empty;
			string res = DisplayFormat;
			if(res.Contains("{")) return res;
			return string.Format("{{0:{0}}}", res); 
		}
		protected virtual string GetGroupDisplayFormat() {
			string res = GetDisplayFormat();
			if(res != string.Empty) return res;
			switch(SummaryType) {
				case SummaryItemType.Count: return "Count={0}";
				case SummaryItemType.Min: return "Min={0}";
				case SummaryItemType.Max: return "Max={0}";
				case SummaryItemType.Average: return "Avg={0:0.##}";
				case SummaryItemType.Sum: return "Sum={0:0.##}";
			}
			return "{0}";
		}
		protected virtual string GetFooterDisplayFormat() {
			string res = GetDisplayFormat();
			if(res != string.Empty) return res;
			switch(SummaryType) {
				case SummaryItemType.Count: return "Count={0}";
				case SummaryItemType.Min: return "Min of {1} is {0}";
				case SummaryItemType.Max: return "Max of {1} is {0}";
				case SummaryItemType.Average: return "Avg of {1} is {0:0.##}";
				case SummaryItemType.Sum: return "Sum of {1} is {0:0.##}";
			}
			return "{0}";
		}
		public string GetGroupDisplayText(string columnCaption, object value) {
			return string.Format(GetGroupDisplayFormat(), value, columnCaption);
		}
		public string GetFooterDisplayText(string columnCaption, object value) {
			if(string.IsNullOrEmpty(ShowInColumn) || FieldName == ShowInColumn) return GetGroupDisplayText(columnCaption, value);
			return string.Format(GetFooterDisplayFormat(), value, columnCaption);
		}
		protected void OnSummaryChanged() {
			if(IsLoading() || Collection == null) return;
			((ASPxSummaryItemCollection)Collection).OnSummaryChanged(this);
		}
		string DevExpress.Utils.Design.ICaptionSupport.Caption {
			get {
				if(SummaryType == SummaryItemType.None) return string.Empty;
				string res = SummaryType.ToString();
				if(!string.IsNullOrEmpty(FieldName))
					res = FieldName + " (" + res + ")";
				return res;
			}
		}
	}
	public class ASPxSummaryItemCollection : Collection {
		public event CollectionChangeEventHandler SummaryChanged;
		public ASPxSummaryItemCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public override string ToString() {
			return string.Empty;
		}
		[Browsable(false)]
		public ASPxSummaryItem this[int index] {
			get { return List[index] as ASPxSummaryItem; }
		}
		[Browsable(false)]
		public ASPxSummaryItem this[string fieldName] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].FieldName == fieldName) return this[i];
				}
				return null;
			}
		}
		[Browsable(false)]
		public ASPxSummaryItem this[string fieldName, SummaryItemType summaryType] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].FieldName == fieldName && this[i].SummaryType == summaryType) return this[i];
				}
				return null;
			}
		}
		protected internal void OnSummaryChanged(ASPxSummaryItem item) {
			if(IsLoading || Owner == null) return;
			if(SummaryChanged != null) SummaryChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnSummaryChanged(null);
		}
		protected internal List<ASPxSummaryItem> GetActiveItems() {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem item in this) {
				if(item.SummaryType != SummaryItemType.None) res.Add(item);
			}
			return res;
		}
		protected internal List<ASPxSummaryItem> GetGroupRowItems() {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem item in GetActiveItems()) {
				if(item.IsShowInGroupRow) res.Add(item);
			}
			return res;
		}
		protected IList List { get { return this as IList; } }
		protected override Type GetKnownType() {
			return typeof(ASPxSummaryItem);
		}
		public ASPxSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			ASPxSummaryItem res = new ASPxSummaryItem();
			res.SummaryType = summaryType;
			res.FieldName = fieldName;
			List.Add(res);
			return res;
		}
		public void Add(ASPxSummaryItem item) {
			List.Add(item);
		}
		public void Insert(int index, ASPxSummaryItem item) {
			List.Insert(index, item);
		}
		public void Remove(ASPxSummaryItem item) {
			List.Remove(item);
		}
		public int IndexOf(ASPxSummaryItem item) {
			return List.IndexOf(item);
		}
	}
	public class ASPxGroupSummarySortInfo : CollectionItem {
		ASPxSummaryItem summaryItem;
		ColumnSortOrder sortOrder = ColumnSortOrder.None;
		string groupColumn = string.Empty;
		public ASPxGroupSummarySortInfo() {}		
		public ASPxGroupSummarySortInfo(string groupColumn, ASPxSummaryItem groupSummary) : this(groupColumn, groupSummary, ColumnSortOrder.Ascending) {}
		public ASPxGroupSummarySortInfo(string groupColumn, ASPxSummaryItem groupSummary, ColumnSortOrder sortOrder) {
			this.groupColumn = groupColumn;
			this.summaryItem = groupSummary;
			this.sortOrder = sortOrder;
		}
		[DefaultValue("")]
		public string GroupColumn { 
			get { return groupColumn; }
			set {
				if(value == null) value = string.Empty;
				if(value == GroupColumn) return;
				groupColumn = value;
				OnSummaryChanged();
			}
		}
		[DefaultValue(null)]
		public ASPxSummaryItem SummaryItem { 
			get { return summaryItem; }
			set {
				if(SummaryItem == value) return;
				summaryItem = value;
				OnSummaryChanged();
			}
		}
		[DefaultValue(ColumnSortOrder.None)]
		public ColumnSortOrder SortOrder { 
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				OnSummaryChanged();
			}
		}
		public void Remove() {
			if(Collection == null) return;
			((ASPxGroupSummarySortInfoCollection)Collection).Remove(this);
		}
		protected void OnSummaryChanged() {
			if(IsLoading() || Collection == null) return;
			((ASPxGroupSummarySortInfoCollection)Collection).OnSummaryChanged(this);
		}
	}
	public class ASPxGroupSummarySortInfoCollection : Collection {
		public event CollectionChangeEventHandler SummaryChanged;
		public ASPxGroupSummarySortInfoCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public override string ToString() {
			return string.Empty;
		}
		[Browsable(false)]
		public ASPxGroupSummarySortInfo this[int index] {
			get { return this[index] as ASPxGroupSummarySortInfo; }
		}
		protected internal void OnSummaryChanged(ASPxGroupSummarySortInfo item) {
			if(IsLoading || Owner == null) return;
			if(SummaryChanged != null) SummaryChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnSummaryChanged(null);
		}
		public void Remove(ASPxGroupSummarySortInfo info) {
			int index = IndexOf(info);
			if(index >= 0) RemoveAt(index);
		}
		public void ClearAndAddRange(params ASPxGroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos);
			}
			finally {
				EndUpdate();
			}
		}
		public void Remove(ASPxSummaryItem summary) {
			for(int n = Count - 1; n >= 0; n--) {
				ASPxGroupSummarySortInfo info = this[n];
				if(info.SummaryItem == summary) RemoveAt(n);
			}
		}
		public void AddRange(params ASPxGroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				foreach(ASPxGroupSummarySortInfo info in sortInfos) { Add(info); }
			}
			finally {
				EndUpdate();
			}
		}
	}
}
