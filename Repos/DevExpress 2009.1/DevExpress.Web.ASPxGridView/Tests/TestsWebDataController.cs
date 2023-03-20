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

#if DEBUGTEST
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.Data;
using NUnit.Framework;
using DevExpress.Data;
using System.IO;
using DevExpress.Data.IO;
using DevExpress.Web.ASPxGridView.Tests;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using System.ComponentModel;
using System.Web.UI;
namespace DevExpress.Web.ASPxGridView.Tests {
	public class WebColumnInfoTester : IWebColumnInfo {
		string fieldName;
		ColumnSortOrder sortOrder;
		public WebColumnInfoTester(string fieldName, ColumnSortOrder sortOrder) {
			this.fieldName = fieldName;
			this.sortOrder = sortOrder;
		}
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public string FieldName { get { return fieldName; } }
		public UnboundColumnType UnboundType { get { return UnboundColumnType.Bound; } }
		public bool ReadOnly { get { return false; } }
	}
	public class WebDataProxyTester : WebDataProxy, IWebDataOwner, IWebDataEvents, IWebControlObject, IWebControlPageSettings {
		DataTable table;
		int pageSize = 10;
		int pageIndex = 0;
		GridViewPagerMode pagerMode = GridViewPagerMode.ShowPager;
		List<string> invisibleColumns = new List<string>();
		internal bool dataBoundRequested = false;
		public WebDataProxyTester(int rowCount) : this(rowCount, new Type[] { typeof(int), typeof(string), typeof(DateTime) }) { 
		}
		public WebDataProxyTester(int rowCount, Type[] columnTypes) : base(null, null, null) {
			SetEvents(this);
			SetupTable(rowCount, columnTypes);
			SetDataSource(table.DefaultView);
			SetKeyField();
		}
		public void LoadData(string data) {
			SetDataProvider(new WebDataCachedProvider(this, data), true);
		}
		public bool AllowOnlyOneMasterRowExpanded { get { return false; } }
		protected internal override IWebDataOwner Owner {  get {  return this; } }
		protected internal override IWebControlPageSettings PageSettings { get { return this; } }
		public DataTable Table { get { return table; } }
		public bool IsCachedDataProvider { get { return DataProvider is WebDataCachedProvider; } }
		public new WebDataProviderBase DataProvider { get { return base.DataProvider; } }
		protected virtual void SetupTable(int rowCount, Type[] columnTypes) {
			this.table = new DataTable();
			AddKeyField();
			foreach(Type type in columnTypes) {
				Table.Columns.Add(string.Format("column{0}", Table.Columns.Count), type);
			}
			for(int i = 0; i < rowCount; i++) {
				Table.Rows.Add(GetNewValues());
			}
		}
		protected virtual void AddKeyField() {
			Table.Columns.Add("id", typeof(int));
		}
		protected virtual void SetKeyField() {
			KeyFieldName = "id";
		}
		protected virtual object[] GetNewValues() {
			object[] values = new object[Table.Columns.Count];
			values[0] = Table.Rows.Count + 1;
			for(int i = 1; i < values.Length; i++) {
				values[i] = GetCellValue(i);
			}
			return values;
		}
		protected virtual object GetCellValue(int columnIndex) {
			int random = GetIntRandom(columnIndex);
			Type columnType = Table.Columns[columnIndex].DataType;
			if(columnType == typeof(string))
				return "str" + random.ToString();
			if(columnType == typeof(DateTime)) {
				DateTime dt = new DateTime(2007, 1, 15);
				return dt.AddDays(random);
			}
			return random;
		}
		protected virtual int GetIntRandom(int columnIndex) {
			int rowIndex = Table.Rows.Count;
			return rowIndex % (columnIndex + 2);
		}
		public int OwnerPageSize { get { return pageSize; } set { pageSize = value; } }
		public int OwnerPageIndex { 
			get { return pageIndex; } 
			set {
				if(PageIndex == value) return;
				pageIndex = value;
				if(pageIndexChanged != null) pageIndexChanged(this, EventArgs.Empty);
			} 
		}
		public GridViewPagerMode OwnerPagerMode { get { return pagerMode; } set { pagerMode = value; } }
		public void GroupByColumn(string fieldName) {
			GroupByColumns(fieldName);
		}
		public void GroupByColumns(params string[] fieldNames) {
			List<IWebColumnInfo> columnList = new List<IWebColumnInfo>();
			foreach(string fieldName in fieldNames) {
				WebColumnInfoTester columnInfo = new WebColumnInfoTester(fieldName, ColumnSortOrder.Ascending);
				if(columnInfo != null) {
					columnList.Add(columnInfo);
				}
			}
			SortGroupChanged(columnList, columnList.Count, FilterExpression);
		}
		public WebDataProxyTester CreateFromStream() {
			int savedPageIndex = PageIndex;
			WebDataProxyTester newTester = new WebDataProxyTester(Table.Rows.Count);
			MemoryStream stream = new MemoryStream();
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			SaveDataState(writer);
			string data = SaveData();
			stream.Position = 0;
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			newTester.LoadDataState(reader, string.Empty, false);
			newTester.LoadData(data);
			newTester.OwnerPageIndex = savedPageIndex;
			return newTester;
		}
		public List<string> SerializedColumns { get { return DataProvider.GetSerializedColumns(); } }
		List<IWebColumnInfo> IWebDataOwner.GetColumns() {
			List<IWebColumnInfo> list = new List<IWebColumnInfo>();
			foreach(DataColumn column in Table.Columns) {
				if(InvisibleColumns.IndexOf(column.ColumnName) < 0) {
					list.Add(new WebColumnInfoTester(column.ColumnName, ColumnSortOrder.None));
				}
			}
			return list;
		}
		public List<string> InvisibleColumns { get { return invisibleColumns; } }
		ASPxTestDataSource data;
		EventHandler pageIndexChanged;
		Dictionary<string, object> IWebDataOwner.GetEditTemplateValues() { return null; }
		bool IWebDataOwner.ValidateEditTemplates() { return true; }
		bool IWebDataOwner.IsForceDataSourcePaging { get { return false; } }
		DataSourceSelectArguments IWebDataOwner.SelectArguments {
			get {
				return new DataSourceSelectArguments();
			}
		}
		void IWebDataOwner.RequireDataBound() { this.dataBoundRequested = true; }
		bool IWebDataOwner.IsDesignTime { get { return false; } }
		IDataControllerSort IWebDataOwner.SortClient { get { return null; } }
		event EventHandler IWebDataOwner.PageIndexChanged { add { pageIndexChanged += value; } remove { pageIndexChanged -= value; } }
		int IWebControlPageSettings.PageSize { get { return OwnerPageSize; } }
		int IWebControlPageSettings.PageIndex { get { return OwnerPageIndex; } set { OwnerPageIndex = value; } }
		GridViewPagerMode IWebControlPageSettings.PagerMode { get { return OwnerPagerMode; } }
		IWebControlObject IWebDataOwner.WebControl { get { return this; } }
		System.Web.UI.DataSourceView IWebDataOwner.GetData() {
			if(this.table == null) return null;
			if(data == null) data = new ASPxTestDataSource(table);
			return data.GetView(null); 
		}
		public event EventHandler FocusedRowChanged;
		public event ASPxDataUpdatedEventHandler RowUpdated;
		public event ASPxDataUpdatingEventHandler RowUpdating;
		public event ASPxDataInitNewRowEventHandler InitNewRowEvent;
		public event ASPxDataDeletedEventHandler RowDeleted;
		public event ASPxDataDeletingEventHandler RowDeleting;
		public event ASPxDataInsertedEventHandler RowInserted;
		public event ASPxDataInsertingEventHandler RowInserting;
		public event ASPxStartRowEditingEventHandler StartRowEditing;
		public event ASPxStartRowEditingEventHandler CancelRowEditing;
		public event ASPxParseValueEventHandler ParseValue;
		void IWebDataEvents.OnParseValue(ASPxParseValueEventArgs e) {
			if(ParseValue != null) ParseValue(this, e);
		}
		void IWebDataEvents.OnStartRowEditing(ASPxStartRowEditingEventArgs e) {
			if(StartRowEditing != null) StartRowEditing(this, e);
		}
		void IWebDataEvents.OnCancelRowEditing(ASPxStartRowEditingEventArgs e) {
			if(CancelRowEditing != null) CancelRowEditing(this, e);
		}
		void IWebDataEvents.OnSummaryExists(CustomSummaryExistEventArgs e) {
		}
		void IWebDataEvents.OnFocusedRowChanged() {
			if(FocusedRowChanged != null) FocusedRowChanged(this, EventArgs.Empty);
		}
		void IWebDataEvents.OnCustomSummary(CustomSummaryEventArgs e) { }
		void IWebDataEvents.OnSelectionChanged() { }
		void IWebDataEvents.OnDetailRowsChanged() { }
		object IWebDataEvents.GetUnboundData(int listSourceRowIndex, string fieldName) { return null; }
		void IWebDataEvents.SetUnboundData(int listSourceRowIndex, string fieldName, object value) { }
		void IWebDataEvents.OnRowDeleting(ASPxDataDeletingEventArgs e) {
			if(RowDeleting != null) RowDeleting(this, e);
		}
		void IWebDataEvents.OnRowDeleted(ASPxDataDeletedEventArgs e) {
			if(RowDeleted != null) RowDeleted(this, e);
		}
		void IWebDataEvents.OnRowValidating(ASPxDataValidationEventArgs e) {
		}
		void IWebDataEvents.OnInitNewRow(ASPxDataInitNewRowEventArgs e) {
			if(InitNewRowEvent != null) InitNewRowEvent(this, e);
		}
		void IWebDataEvents.OnRowInserting(ASPxDataInsertingEventArgs e) {
			if(RowInserting != null) RowInserting(this, e);
		}
		void IWebDataEvents.OnRowInserted(ASPxDataInsertedEventArgs e) {
			if(RowInserted != null) RowInserted(this, e);
		}
		void IWebDataEvents.OnRowUpdating(ASPxDataUpdatingEventArgs e) {
			if(RowUpdating != null) RowUpdating(this, e);
		}
		void IWebDataEvents.OnRowUpdated(ASPxDataUpdatedEventArgs e) {
			if(RowUpdated != null) RowUpdated(this, e);
		}
		#region IWebControlObject Members
		bool IWebControlObject.IsDesignMode() { return false; }
		bool IWebControlObject.IsLoading() { return false;}
		bool IWebControlObject.IsRendering() { return false; }
		void IWebControlObject.LayoutChanged() { }
		void IWebControlObject.TemplatesChanged() { }
		#endregion
	}
	[TestFixture]
	public class WebDataControllerBase {
		[Test]
		public void HasFieldNameTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(true, tester.HasFieldName("column1"));
			Assert.AreEqual(false, tester.HasFieldName("xcolumn1"));
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.HasFieldName("column1"));
			Assert.AreEqual(false, tester.HasFieldName("xcolumn1"));
		}
	}
	[TestFixture]
	public class WebDataControllerSerialization {
		[Test]
		public void UseCachedDataProviderTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.IsFalse(tester.IsCachedDataProvider);
			string data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsTrue(tester.IsCachedDataProvider);
		}
		[Test]
		public void GetListSourceRowValueTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(tester.GetRowValue(1, "column1"), tester.GetListSourceRowValue(1, "column1"));
		}
		[Test]
		public void VisibleRowCountTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			int visibleCount = tester.VisibleRowCountOnPage;
			string data = tester.SaveData();
			tester.SetDataSource(null);
			tester.LoadData(data);
			Assert.AreEqual(visibleCount, tester.VisibleRowCountOnPage);
		}
		[Test]
		public void SaveAndRestoreRowValueTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			object val = tester.GetRowValue(4, "column1");
			string data = tester.SaveData();
			tester.SetDataSource(null);
			tester.LoadData(data);
			Assert.AreEqual(val, tester.GetRowValue(4, "column1"));
		}
		[Test]
		public void SaveAndRestoreRowValueShowAllRecordsTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.OwnerPagerMode = GridViewPagerMode.ShowAllRecords;
			object val = tester.GetRowValue(20, "column1");
			tester = tester.CreateFromStream();
			Assert.AreEqual(val, tester.GetRowValue(20, "column1"));
		}
		[Test]
		public void SaveAndRestoreRowValueTwoTimesTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			object val = tester.GetRowValue(4, "column1");
			string data = tester.SaveData();
			tester.SetDataSource(null);
			tester.LoadData(data);
			string newData = tester.SaveData();
			Assert.AreEqual(data, newData);
		}
		[Test]
		public void CreateWebDataControllerOnSort() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.IsFalse(tester.IsCachedDataProvider);
			string data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsTrue(tester.IsCachedDataProvider);
			tester.GroupByColumn("column1");
			Assert.IsFalse(tester.IsCachedDataProvider);
		}
		[Test]
		public void SaveAndRestoreGroupTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			Assert.AreEqual(17, tester.GetChildDataRowCount(0));
			Assert.AreEqual(1, tester.DataProvider.GroupCount);
			object val = tester.GetRowValue(0, string.Empty);
			object val1 = tester.GetRowValue(1, string.Empty);
			object val2 = tester.GetRowValue(2, string.Empty);
			string data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsTrue(tester.IsCachedDataProvider);
			Assert.AreEqual(1, tester.DataProvider.GroupCount);
			Assert.AreEqual(WebRowType.Group, tester.GetRowType(0));
			Assert.AreEqual(0, tester.GetRowLevel(0));
			Assert.AreEqual(17, tester.GetChildDataRowCount(0));
			Assert.AreEqual(val, tester.GetRowValue(0, string.Empty));
			Assert.AreEqual(val1, tester.GetRowValue(1, string.Empty));
			Assert.AreEqual(val2, tester.GetRowValue(2, string.Empty));
		}
		[Test]
		public void SaveAndRestoreGroupExpandTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			string data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsFalse(tester.IsRowExpanded(0));
			tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			tester.ExpandAll();
			data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsTrue(tester.IsRowExpanded(0));
		}
		[Test]
		public void SaveVisibleGroupOnlyTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			string data = tester.SaveData();
			tester.LoadData(data);
			WebDataCachedProvider cachedProvider = tester.DataProvider as WebDataCachedProvider;
			Assert.AreEqual(tester.VisibleRowCountOnPage, cachedProvider.Data.Count);
		}
		[Test]
		public void CreateWebDataControllerOnNextPage() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.IsFalse(tester.IsCachedDataProvider);
			string data = tester.SaveData();
			tester.LoadData(data);
			Assert.IsTrue(tester.IsCachedDataProvider);
		}
		[Test]
		public void GetCachedRowValue() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			object value = tester.GetRowValue(5, "column1");
			Assert.AreEqual(value, (tester.GetRow(5) as DataRowView)["column1"]);
			Assert.AreEqual(true, value != null);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsCachedDataProvider);
			WebCachedDataRow cachedRow = tester.GetRow(5) as WebCachedDataRow;
			Assert.AreEqual(true, cachedRow != null);
			Assert.AreEqual(value, TypeDescriptor.GetProperties(cachedRow)["column1"].GetValue(cachedRow));
		}
		[Test]
		public void SerializeColumnsByRequest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.InvisibleColumns.Add("column1");
			tester.InvisibleColumns.Add("column2");
			tester.InvisibleColumns.Add("column3");
			tester.SaveData();
			Assert.AreEqual(1, tester.SerializedColumns.Count);
			Assert.AreEqual("id", tester.SerializedColumns[0]);
			tester.GetRowValue(5, "column1");
			Assert.AreEqual(2, tester.SerializedColumns.Count);
			Assert.AreEqual("column1", tester.SerializedColumns[1]);
		}
	}
	[TestFixture]
	public class WebDataControllerEditing {
		[Test]
		public void GetKeyValue() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(tester.GetRowValue(5, "id"), tester.GetRowKeyValue(5));
		}
		[Test]
		public void GetKeyValueFromCached() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester = tester.CreateFromStream();
			Assert.AreEqual(tester.GetRowValue(5, "id"), tester.GetRowKeyValue(5));
		}
		[Test]
		public void StartEdit_IsEditing() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(false, tester.IsEditing);
			tester.StartEdit(2);
			Assert.AreEqual(true, tester.IsEditing);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsEditing);
		}
		[Test]
		public void StartEdit_IsEditingLoadAllRecords() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.OwnerPagerMode = GridViewPagerMode.ShowAllRecords;
			Assert.AreEqual(false, tester.IsEditing);
			tester.StartEdit(2);
			Assert.AreEqual(true, tester.IsEditing);
		}
		[Test]
		public void StartEdit_IsEditingFromCached() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.StartEdit(2);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsEditing);
			Assert.AreEqual(true, tester.IsRowEditing(2));
			Assert.AreEqual(false, tester.IsRowEditing(0));
		}
		[Test]
		public void StartEdit_IsEditingFromCachedOnNewInstance() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.StartEdit(2);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsEditing); 
		}
		[Test]
		public void StartEdit_StartEditFromCache() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester = tester.CreateFromStream();
			tester.StartEdit(1);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsEditing);
		}
		[Test]
		public void CancelEditFromCache() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester = tester.CreateFromStream();
			tester.StartEdit(1);
			tester = tester.CreateFromStream();
			tester.CancelEdit();
			Assert.AreEqual(false, tester.IsEditing);
		}
		[Test]
		public void UpdateEdit() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.StartEdit(0);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("column1", 100);
			tester.SetEditorValues(values, false);
			tester.UpdateRow(false);
			Assert.AreEqual(100, tester.Table.DefaultView[0]["column1"]);
		}
		[Test]
		public void GetEditingRowValue() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			object value = tester.GetRowValue(0, "column1");
			tester.StartEdit(0);
			Assert.AreEqual(value, tester.GetEditingRowValue(0, "column1"));
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("column1", 100);
			tester.SetEditorValues(values, false);
			Assert.AreEqual(100, tester.GetEditingRowValue(0, "column1"));
		}
		[Test]
		public void ParseValueEvent() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.ParseValue += new ASPxParseValueEventHandler(tester_ParseValue);
			bool passed = false;
			tester.RowUpdating += delegate(object sender, ASPxDataUpdatingEventArgs e) {
				if(e.NewValues["column1"].ToString() == "5") passed = true;
			};
			tester.StartEdit(0);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values["column1"] = "yyy";
			tester.SetEditorValues(values, false);
			tester.UpdateRow(false);
			Assert.AreEqual(true, passed);
		}
		void tester_ParseValue(object sender, ASPxParseValueEventArgs e) {
			if(e.FieldName == "column1" && e.Value.ToString() == "yyy") e.Value = 5;
		}
		[Test]
		public void UpdateEditEvents() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.RowUpdated += new ASPxDataUpdatedEventHandler(tester_RowUpdated);
			tester.RowUpdating += new ASPxDataUpdatingEventHandler(tester_RowUpdating);
			object key = tester.GetRowKeyValue(0);
			tester.StartEdit(0);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values["column1"] = 100;
			tester.SetEditorValues(values, false);
			tester.UpdateRow(false);
			Assert.AreEqual(key, updatingKey);
			Assert.AreEqual(key, updatedKey);
			Assert.AreEqual(100, tester.Table.DefaultView[0]["column1"]);
			values["column1"] = 200;
			tester.StartEdit(0);
			tester.SetEditorValues(values, false);
			this.cancelKey = key;
			tester.UpdateRow(false);
			Assert.AreEqual(100, tester.Table.DefaultView[0]["column1"]);
		}
		[Test]
		public void GroupRowCannotBeEditing() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			tester.ExpandAll();
			tester.StartEdit(1);
			Assert.AreEqual(true, tester.IsRowEditing(1));
			Assert.AreEqual(false, tester.IsRowEditing(0));
		}
		object updatingKey = null, updatedKey = null, cancelKey = null;
		void tester_RowUpdating(object sender, ASPxDataUpdatingEventArgs e) {
			this.updatingKey = e.Keys[0];
			if(object.Equals(this.updatingKey, this.cancelKey)) e.Cancel = true;
		}
		void tester_RowUpdated(object sender, ASPxDataUpdatedEventArgs e) {
			this.updatedKey = e.Keys[0];
		}
	}
	[TestFixture]
	public class WebDataControllerAddNewRow {
		[Test]
		public void AddNewRow() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			int visibleCount = tester.VisibleRowCount;
			tester.AddNewRow();
			Assert.AreEqual(true, tester.IsEditing);
			Assert.AreEqual(true, tester.IsNewRowEditing);
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsNewRowEditing);
			Assert.AreEqual(true, tester.IsEditing);
			tester.CancelEdit();
			Assert.AreEqual(false, tester.IsEditing);
			Assert.AreEqual(false, tester.IsNewRowEditing);
		}
		[Test]
		public void GetNewRowValue() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(null, tester.GetRowValue(WebDataProxy.NewItemRow, "id"));
		}
		[Test]
		public void NewRowAfterEdit() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.StartEdit(2);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("column1", 100);
			tester.SetEditorValues(values, false);
			tester.AddNewRow();
			Assert.AreEqual(null, tester.GetEditingRowValue(WebDataProxy.NewItemRow, "column1"));
		}
		[Test]
		public void StartEditingTest() {
			this.startEditingCount = 0;
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.StartRowEditing += new ASPxStartRowEditingEventHandler(tester_StartRowEditing);
			tester.StartEdit(0);
			Assert.AreEqual(true, tester.IsEditing);
			Assert.AreEqual(1, this.startEditingCount);
			tester.CancelEdit();
			tester.StartEdit(1);
			Assert.AreEqual(false, tester.IsEditing);
		}
		[Test]
		public void CancelEditingTest() {
			this.startEditingCount = 0;
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.CancelRowEditing += new ASPxStartRowEditingEventHandler(tester_StartRowEditing);
			tester.StartEdit(0);
			Assert.AreEqual(true, tester.IsEditing);
			Assert.AreEqual(0, this.startEditingCount);
			tester.CancelEdit();
			Assert.AreEqual(1, this.startEditingCount);
			Assert.AreEqual(false, tester.IsEditing);
			tester.StartEdit(1);
			tester.CancelEdit();
			Assert.AreEqual(true, tester.IsEditing);
			Assert.AreEqual(2, this.startEditingCount);
		}
		int startEditingCount = 0;
		void tester_StartRowEditing(object sender, ASPxStartRowEditingEventArgs e) {
			this.startEditingCount++;
			if(object.Equals(e.EditingKeyValue, 2)) e.Cancel = true;
		}
		[Test]
		public void AddNewRowEvents() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.InitNewRowEvent += new ASPxDataInitNewRowEventHandler(tester_InitNewRowEvent);
			tester.RowInserted += new ASPxDataInsertedEventHandler(tester_RowInserted);
			tester.RowInserting += new ASPxDataInsertingEventHandler(tester_RowInserting);
			int visibleCount = tester.VisibleRowCount;
			tester.AddNewRow();
			tester.EndEdit();
			Assert.AreEqual(visibleCount + 1, tester.VisibleRowCount);
			Assert.AreEqual(1000, insertingValue);
			Assert.AreEqual(1000, insertedValue);
			Assert.AreEqual(1000, tester.GetRowValue(visibleCount, "column1"));
			this.cancelAdd = true;
			tester.AddNewRow();
			tester.EndEdit();
			Assert.AreEqual(visibleCount + 1, tester.VisibleRowCount);
		}
		bool cancelAdd;
		object insertingValue, insertedValue;
		void tester_RowInserting(object sender, ASPxDataInsertingEventArgs e) {
			if(this.cancelAdd) e.Cancel = true;
			this.insertingValue = e.NewValues["column1"];
		}
		void tester_RowInserted(object sender, ASPxDataInsertedEventArgs e) {
			this.insertedValue = e.NewValues["column1"];
		}
		void tester_InitNewRowEvent(object sender, ASPxDataInitNewRowEventArgs e) {
			e.NewValues.Add("column1", 1000);
		}
	}
	[TestFixture]
	public class WebDataControllerDeleteTests {
		[Test]
		public void Delete() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			int visibleCount = tester.VisibleRowCount;
			tester.RowDeleted += new ASPxDataDeletedEventHandler(tester_RowDeleted);
			tester.RowDeleting += new ASPxDataDeletingEventHandler(tester_RowDeleting);
			object key = tester.GetRowKeyValue(1);
			tester.DeleteRow(1);
			Assert.IsNotNull(key);
			Assert.AreEqual(key, this.deleteKey);
			Assert.AreEqual(key, this.deletingKey);
			Assert.AreEqual(visibleCount - 1, tester.VisibleRowCount);
		}
		[Test]
		public void DeleteCancel() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			int visibleCount = tester.VisibleRowCount;
			tester.RowDeleting += new ASPxDataDeletingEventHandler(tester_RowDeleting);
			this.deleteCancelKey = tester.GetRowKeyValue(1);
			tester.DeleteRow(1);
			Assert.AreEqual(visibleCount, tester.VisibleRowCount);
		}
		object deleteKey = null, deletingKey = null, deleteCancelKey = null;
		void tester_RowDeleting(object sender, ASPxDataDeletingEventArgs e) {
			this.deletingKey = e.Keys[0];
			if(object.Equals(this.deletingKey, deleteCancelKey)) e.Cancel = true;
		}
		void tester_RowDeleted(object sender, ASPxDataDeletedEventArgs e) {
			this.deleteKey = e.Keys[0];
		}
		[Test]
		public void DeleteWhileEditing() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			int visibleCount = tester.VisibleRowCount;
			tester.StartEdit(0);
			tester.DeleteRow(1);
			Assert.AreEqual(false, tester.IsEditing);
			Assert.AreEqual(visibleCount - 1, tester.VisibleRowCount);
		}
	}
	[TestFixture]
	public class WebDataControllerSelectionTests {
		[Test]
		public void SelectOneRecord() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(false, tester.Selection.IsRowSelected(2));
			tester.Selection.SelectRow(2);
			Assert.AreEqual(1, tester.Selection.Count);
			Assert.AreEqual(true, tester.Selection.IsRowSelected(2));
		}
		[Test]
		public void RestoreFromCache() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Selection.SelectRow(2);
			tester = tester.CreateFromStream();
			Assert.AreEqual(1, tester.Selection.Count);
			Assert.AreEqual(true, tester.Selection.IsRowSelected(2));
		}
		[Test]
		public void SelectAllUnSelectAll() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Selection.SelectAll();
			Assert.AreEqual(50, tester.Selection.Count);
			Assert.AreEqual(true, tester.Selection.IsRowSelected(2));
			tester.Selection.UnselectRow(2);
			Assert.AreEqual(false, tester.Selection.IsRowSelected(2));
			Assert.AreEqual(49, tester.Selection.Count);
			tester.Selection.SelectRow(2);
			Assert.AreEqual(true, tester.Selection.IsRowSelected(2));
			Assert.AreEqual(50, tester.Selection.Count);
			tester.Selection.UnselectAll();
			Assert.AreEqual(false, tester.Selection.IsRowSelected(2));
			Assert.AreEqual(0, tester.Selection.Count);
			tester.Selection.SelectRow(2);
			Assert.AreEqual(true, tester.Selection.IsRowSelected(2));
			Assert.AreEqual(1, tester.Selection.Count);
		}
		[Test]
		public void SelectAllRestoreFromCache() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Selection.SelectAll();
			Assert.AreEqual(50, tester.Selection.Count);
			tester = tester.CreateFromStream();
			Assert.AreEqual(50, tester.Selection.Count);
		}
		[Test]
		public void DeleteSelectionBug_B19078() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Selection.SelectRow(2);
			Assert.AreEqual(1, tester.Selection.Count);
			tester.DeleteRow(2);
			Assert.AreEqual(0, tester.Selection.Count);
		}
		[Test]
		public void HasSelection() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			List<int> selected = tester.GetSelectedVisibleIndexesIfAllExpanded();
			Assert.AreEqual(0, selected.Count);
			tester.Selection.SelectRow(2);
			selected = tester.GetSelectedVisibleIndexesIfAllExpanded();
			Assert.AreEqual(3, selected.Count);
			Assert.AreEqual(0, selected[0]);
			Assert.AreEqual(1, selected[1]);
			Assert.AreEqual(2, selected[2]);
		}
		[Test]
		public void B36178() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.SettingsPager.PageSize = 3;
			tester.DataBind();
			tester.Selection.SelectAll();
			Assert.AreEqual(tester.VisibleRowCount - tester.SettingsPager.PageSize, tester.DataProxy.GetSelectedRowCountWithoutCurrentPage());
			tester.GroupBy(tester.Columns[tester.KeyFieldName]);
			Assert.AreEqual(0, tester.DataProxy.GetSelectedRowCountWithoutCurrentPage());
			tester.ExpandRow(0);
			tester.ExpandRow(2);
			Assert.AreEqual(1, tester.DataProxy.GetSelectedRowCountWithoutCurrentPage());
		}
	}
	[TestFixture]
	public class WebDataControllerParentRowsTests {
		[Test]
		public void LiveDataProviderTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			Assert.AreEqual(0, tester.GetParentRows().Count);
			tester.OwnerPageIndex = 1;
			Assert.AreEqual(2, tester.GetParentRows().Count);
			Assert.AreEqual("str1", tester.GetRowValue(tester.GetParentRows()[1], "column2"));
			Assert.AreEqual(0, tester.GetRowValue(tester.GetParentRows()[0], "column1"));
		}
		[Test]
		public void CachedDataProviderTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.GetRowValue(1, "column2");
			tester.GetRowValue(1, "column1");
			tester.ExpandAll();
			tester.OwnerPageIndex = 1;
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsCachedDataProvider);
			Assert.AreEqual(2, tester.GetParentRows().Count);
			Assert.AreEqual("str1", tester.GetRowValue(tester.GetParentRows()[1], "column2"));
			Assert.AreEqual(0, tester.GetRowValue(tester.GetParentRows()[0], "column1"));
		}
		[Test]
		public void CheckVisibleRows() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("id");
			tester.ExpandAll();
			tester.OwnerPageSize = 3;
			Assert.AreEqual(false, tester.IsGroupRowFitOnPage(2));
			tester.OwnerPageSize = 4;
			Assert.AreEqual(true, tester.IsGroupRowFitOnPage(2));
		}
		[Test]
		public void LoadAllRecords() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.OwnerPageSize = 2;
			tester.ExpandAll();
			Assert.AreEqual(false, tester.IsGroupRowFitOnPage(0));
			tester.OwnerPagerMode = GridViewPagerMode.ShowAllRecords;
			Assert.AreEqual(true, tester.IsGroupRowFitOnPage(0));
		}
	}
	[TestFixture]
	public class WebDataControllerGroupFooterTests {
		[Test]
		public void IsLastRowTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1");
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(0));
			tester.ExpandAll();
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(0));
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(1));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(tester.GetChildDataRowCount(0)));
		}
		[Test]
		public void IsLastRowTest2() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(21));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(17));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(16));
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(12));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(64));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(60));
		}
		[Test]
		public void RowIsLastInLevelTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			Assert.AreEqual(0, tester.RowIsLastInLevel(21));
			Assert.AreEqual(0, tester.RowIsLastInLevel(17));
			Assert.AreEqual(1, tester.RowIsLastInLevel(16));
			Assert.AreEqual(-1, tester.RowIsLastInLevel(12));
			Assert.AreEqual(0, tester.RowIsLastInLevel(64));
			Assert.AreEqual(0, tester.RowIsLastInLevel(60));
		}
		[Test]
		public void RowIsLastInLevelCachedTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.OwnerPageSize = 500;
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			tester = tester.CreateFromStream();
			Assert.AreEqual(0, tester.RowIsLastInLevel(21));
			Assert.AreEqual(0, tester.RowIsLastInLevel(17));
			Assert.AreEqual(1, tester.RowIsLastInLevel(16));
			Assert.AreEqual(-1, tester.RowIsLastInLevel(12));
			Assert.AreEqual(0, tester.RowIsLastInLevel(64));
			Assert.AreEqual(0, tester.RowIsLastInLevel(60));
		}
		[Test]
		public void IsLastRowTest3() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.ExpandAll();
			tester.CollapseRow(17, true);
			tester.CollapseRow(12, true);
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(13)); 
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(12));
		}
		[Test]
		public void IsLastRowCachedTest1() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.OwnerPageSize = 50;
			tester.GroupByColumns("column1");
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(0));
			tester.ExpandAll();
			tester = tester.CreateFromStream();
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(0));
			Assert.AreEqual(false, tester.IsLastRowInCurrentLevel(1));
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(tester.GetChildDataRowCount(0)));
		}
		[Test]
		public void GetParentGroupRows() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.OwnerPageSize = 50;
			tester.ExpandAll();
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(16));
			List<int> rows = tester.GetFooterParentGroupRows(16);
			Assert.AreEqual(1, rows.Count);
			Assert.AreEqual(12, rows[0]);
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(21));
			rows = tester.GetFooterParentGroupRows(21);
			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(17, rows[0]);
			Assert.AreEqual(0, rows[1]);
			rows = tester.GetFooterParentGroupRows(64);
			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(60, rows[0]);
			Assert.AreEqual(44, rows[1]);
		}
		[Test]
		public void GetParentGroupRowsCachedTest() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumns("column1", "column2");
			tester.OwnerPageSize = 100;
			tester.ExpandAll();
			tester = tester.CreateFromStream();
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(16));
			List<int> rows = tester.GetFooterParentGroupRows(16);
			Assert.AreEqual(1, rows.Count);
			Assert.AreEqual(12, rows[0]);
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(21));
			rows = tester.GetFooterParentGroupRows(21);
			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(17, rows[0]);
			Assert.AreEqual(0, rows[1]);
			rows = tester.GetFooterParentGroupRows(64);
			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(60, rows[0]);
			Assert.AreEqual(44, rows[1]);
		}
		[Test]
		public void BugFoundByRomanRodin3GroupedColumns() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Table.Rows.Add(101, 10, "st 1", new DateTime(2001, 1, 1));
			tester.Table.Rows.Add(102, 10, "st 2", new DateTime(2001, 1, 1));
			tester.Table.Rows.Add(103, 10, "st 3", new DateTime(2001, 1, 1));
			tester.GroupByColumns("column3", "column1", "column2");
			tester.OwnerPageSize = 100;
			tester.ExpandRow(0, false);
			tester.ExpandRow(1, false);
			Assert.AreEqual(true, tester.IsLastRowInCurrentLevel(4));
			Assert.AreEqual(2, tester.GetGroupFooterVisibleIndexes(4, true).Count);
			List<int> rows = tester.GetGroupFooterVisibleIndexes(4, false);
			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(4, rows[0]);
			Assert.AreEqual(1, rows[1]);
			Assert.AreEqual(0, rows[2]);
			tester.ExpandRow(4, false);
			rows = tester.GetGroupFooterVisibleIndexes(5, false);
			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(4, rows[0]);
			Assert.AreEqual(1, rows[1]);
			Assert.AreEqual(0, rows[2]);
		}
		[Test]
		public void BugFoundByRomanRodin3GroupedColumns2() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.Table.Rows.Add(101, 10, "st 1", new DateTime(2001, 1, 1));
			tester.Table.Rows.Add(102, 10, "st 2", new DateTime(2001, 1, 1));
			tester.Table.Rows.Add(103, 10, "st 3", new DateTime(2001, 1, 1));
			tester.GroupByColumns("column3", "column1", "column2");
			tester.OwnerPageSize = 100;
			tester.ExpandRow(0, false);
			tester.ExpandRow(1, false);
			tester.ExpandRow(3, false);
			tester.ExpandRow(5, false);
			for(int i = 0; i < tester.VisibleRowCount; i++) {
				System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}", i, tester.GetRowLevel(i)));
			}
			List<int> rows = tester.GetGroupFooterVisibleIndexes(6, false);
			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(5, rows[0]);
			Assert.AreEqual(1, rows[1]);
			Assert.AreEqual(0, rows[2]);
		}
	}
	[TestFixture]
	public class WebDataControllerRecreationTests {
		[Test]
		public void TestXAFSelectionBug_B30108() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			tester.ExpandRow(0, false);
			MemoryStream stream = new MemoryStream();
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			tester.SaveDataState(writer);
			tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column1");
			stream.Position = 0;
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			tester.LoadDataState(reader, "FT", false);
			tester.RestoreRowsState();
			Assert.AreEqual(1, tester.Selection.Count);
		}
		[Test]
		public void B131609() {
			WebDataProxyTester tester = new WebDataProxyTester(1);
			tester.GroupByColumn("column1");
			tester.ExpandRow(0, false);
			string data = tester.SaveData();
			tester.LoadData(data);
			WebDataCachedProvider cachedProvider = tester.DataProvider as WebDataCachedProvider;
			object value = cachedProvider.GetRowValue(1, "id", false);
			Assert.AreEqual(value, tester.GetRowValue(0, "id"));
			Assert.AreEqual(1, cachedProvider.FindRowByKey("id", value, false));
		}
	}
	[TestFixture]
	public class WebDataControllerRequireDataBoundTests {
		[Test]
		public void Test() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.InvisibleColumns.Add("column1");
			tester.InvisibleColumns.Add("column2");
			tester.InvisibleColumns.Add("column3");
			string data = tester.SaveData();
			tester.LoadData(data);
			object val = tester.GetRowValue(0, "column1");
			Assert.AreEqual(true, tester.HasFieldName("column1"));
			Assert.AreEqual(true, tester.dataBoundRequested);
		}
		[Test]
		public void Test2() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.InvisibleColumns.Add("column2");
			tester.InvisibleColumns.Add("column3");
			string data = tester.SaveData();
			tester.LoadData(data);
			object val = tester.GetRowValue(0, "column1");
			Assert.AreEqual(true, tester.HasFieldName("column1"));
			Assert.AreEqual(false, tester.dataBoundRequested);
		}
	}
	[TestFixture]
	public class WebDataControllerFocusRowTests {
		int focusedRowChangedCount;
		[SetUp]
		protected void Init() {
			this.focusedRowChangedCount = 0;
		}
		protected WebDataProxyTester CreateDataController() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.FocusedRowChanged += new EventHandler(tester_FocusedRowChanged);
			tester.AllowFocusedRow = true;
			this.focusedRowChangedCount = 0;
			return tester;
		}
		void tester_FocusedRowChanged(object sender, EventArgs e) {
			this.focusedRowChangedCount++;
		}
		[Test]
		public void AllowFocusedRow() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			Assert.AreEqual(false, tester.AllowFocusedRow);
			Assert.AreEqual(false, tester.IsRowFocused(0));
			Assert.AreEqual(-1, tester.FocusedRowVisibleIndex);
			tester.AllowFocusedRow = true;
			Assert.AreEqual(true, tester.IsRowFocused(0));
			Assert.AreEqual(0, tester.FocusedRowVisibleIndex);
		}
		[Test]
		public void SetFocusedRowVisibleIndex() {
			WebDataProxyTester tester = CreateDataController();
			tester.FocusedRowVisibleIndex = 5;
			Assert.AreEqual(5, tester.FocusedRowVisibleIndex);
			Assert.AreEqual(true, tester.IsRowFocused(5));
			Assert.AreEqual(false, tester.IsRowFocused(4));
			tester.FocusedRowVisibleIndex = -100;
			Assert.AreEqual(-1, tester.FocusedRowVisibleIndex);
			tester.FocusedRowVisibleIndex = 500;
			Assert.AreEqual(tester.VisibleRowCount - 1, tester.FocusedRowVisibleIndex);
			Assert.AreEqual(3, this.focusedRowChangedCount);
		}
		[Test]
		public void ChangePageIndex() {
			WebDataProxyTester tester = CreateDataController();
			tester.OwnerPageIndex = 2;
			Assert.AreEqual(tester.PageSize * 2, tester.FocusedRowVisibleIndex);
			Assert.AreEqual(1, this.focusedRowChangedCount);
		}
		[Test]
		public void ChangeFocusedIndex() {
			WebDataProxyTester tester = CreateDataController();
			tester.FocusedRowVisibleIndex = 44;
			Assert.AreEqual(1, this.focusedRowChangedCount);
			Assert.AreEqual(44 / tester.PageSize, tester.PageIndex);
		}
		[Test]
		public void FocusGroupRowTests() {
			WebDataProxyTester tester = CreateDataController();
			tester.FocusedRowVisibleIndex = 0;
			Assert.AreEqual(0, tester.FocusedRowVisibleIndex);
			tester.GroupByColumn("id");
			Assert.AreEqual(0, tester.FocusedRowVisibleIndex);
		}
		[Test]
		public void FilterToEmptyGrid() {
			WebDataProxyTester tester = CreateDataController();
			tester.FocusedRowVisibleIndex = 1;
			Assert.AreEqual(1, this.focusedRowChangedCount);
			tester.SortGroupChanged(new List<IWebColumnInfo>(), 0, "id = -1");
			tester.CheckFocusedRowChanged();
			Assert.AreEqual(-1, tester.FocusedRowVisibleIndex);
			Assert.AreEqual(2, this.focusedRowChangedCount);
		}
		[Test]
		public void MakeRowVisible_Ungroup() {
			WebDataProxyTester tester = CreateDataController();
			Assert.AreEqual(0, tester.PageIndex);
			Assert.IsTrue(tester.MakeRowVisible(15));
			Assert.AreEqual(1, tester.PageIndex);
			Assert.IsFalse(tester.MakeRowVisible(150));
			Assert.AreEqual(1, tester.PageIndex);
		}
		[Test]
		public void MakeRowVisible_Group() {
			WebDataProxyTester tester = CreateDataController();
			tester.GroupByColumn("column1");
			Assert.AreEqual(0, tester.PageIndex);
			int visibleRowCount = tester.VisibleRowCount;
			Assert.IsTrue(tester.MakeRowVisible(47));
			Assert.AreNotEqual(visibleRowCount, tester.VisibleRowCount);
			Assert.AreEqual(1, tester.PageIndex);
			Assert.IsFalse(tester.MakeRowVisible(150));
			Assert.AreEqual(1, tester.PageIndex);
		}
	}
	[TestFixture]
	public class WebDataControllerChildRows {
		[Test]
		public void GetChildRowCount() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column2");
			Assert.AreEqual(13, tester.GetChildDataRowCount(0));
			Assert.AreEqual(13, tester.GetChildDataRowCount(1));
			Assert.AreEqual(12, tester.GetChildDataRowCount(2));
			Assert.AreEqual(12, tester.GetChildDataRowCount(3));
		}
		[Test]
		public void GetChildRowValues() {
			WebDataProxyTester tester = new WebDataProxyTester(50);
			tester.GroupByColumn("column2");
			Assert.AreEqual("str0", tester.GetChildRowValues(0, 0, "column2"));
			Assert.AreEqual(0, tester.GetChildRowValues(0, 0, "column1"));
			Assert.AreEqual(1, tester.GetChildRowValues(0, 1, "column1"));
			Assert.AreEqual(new object[] { 1, "str0" }, tester.GetChildRowValues(0, 1, "column1", "column2"));
			Assert.AreEqual(new object[] { 2, "str1" }, tester.GetChildRowValues(1, 1, "column1", "column2"));
			Assert.AreEqual(2, ((DataRowView)tester.GetChildRow(1, 1))["column1"]);
		}
	}
}
#endif
