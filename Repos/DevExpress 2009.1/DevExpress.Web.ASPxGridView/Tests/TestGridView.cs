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
using NUnit.Framework;
using System.Web.UI;
using DevExpress.Web.ASPxGridView;
using DevExpress.Data;
using DevExpress.Web.Data;
using System.Reflection;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Tests;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.Web.ASPxEditors.FilterControl;
namespace DevExpress.Web.ASPxGridView.Tests {
	[ToolboxItem(false)]
	public class MyTestTemplateButton : Button, ITemplate {
		void ITemplate.InstantiateIn(Control container) {
			Button btn = new Button();
			btn.ID = ID;
			container.Controls.Add(btn);
		}
	}
	public class GridWebProxyTester : WebDataProxy {
		public GridWebProxyTester(IWebDataOwner owner, IWebControlPageSettings pageSettings, IWebDataEvents events)
			: base(owner, pageSettings, events) {
		}
		protected override WebDataSelection CreateDataSelection() { return new WebTestDataSelection(this);  }
		public DataController DataController { get { return (DataProvider as WebDataControllerProvider).DataController; } }
		public bool IsCachedDataProvider { get { return DataProvider is WebDataCachedProvider; } }
		public List<string> SerializedColumns { get { return DataProvider.GetSerializedColumns(); } }
	}
	public class GridViewContainerControlTester : GridViewContainerControl {
		public GridViewContainerControlTester(ASPxGridView gridView) : base(gridView) { }
		protected override bool CanRenderPopupControls { get { return true; } }
	}
	public class WebTestDataSelection : WebDataSelection {
		public WebTestDataSelection(WebDataProxy webData) : base(webData) { }
		public bool TestIsStoreSelectected { get { return IsStoreSelected; } }
	}
	[ToolboxItem(false)]
	public class ASPxGridViewTester : DevExpress.Web.ASPxGridView.ASPxGridView, IWebControlObject {
		DataTable table;
		GridViewCommandColumn commandColumn;
		bool isTestLoading = false;
		int selectionChangedCounter = 0, focusedRowChangedCounter, performSelectCounter = 0;
		bool allowTestGridSaveViewState = true;		
		string customCallbackArgs;
		public ASPxGridViewTester(bool createCommandColumn) {
			typeof(ASPxWebControlBase).GetField("isLoaded", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, true);			
			this.KeyFieldName = "id";
			ID = "grid";
			SelectionChanged += new EventHandler(ASPxGridViewTester_SelectionChanged);
			FocusedRowChanged += new EventHandler(ASPxGridViewTester_FocusedRowChanged);
			DetailRowExpandedChanged += delegate(object s, ASPxGridViewDetailRowEventArgs e) {
				DetailRowExpandedChangedLog += string.Format("{0},{1} ", e.VisibleIndex, e.Expanded);
			};
			SettingsPager.Mode = GridViewPagerMode.ShowPager;
			table = new WebDataProxyTester(50).Table;
			DataSource = new ASPxTestDataSource(table);
			PopulateColumns();
			if(createCommandColumn) {
				this.commandColumn = new GridViewCommandColumn();
				Columns.Add(this.commandColumn);
			}
			DoControlInit();
			TrackViewState();			
		}
		public ASPxGridViewTester() : this(false) {
		}
		internal ASPxTestDataSource Data { get { return DataSource as ASPxTestDataSource; } }
		internal ASPxTestDataSourceView View { get { return Data == null ? null : Data.View; } }
		public void MakeDataSourceNullable() {
			this.table = null;
		}
		bool IWebControlObject.IsLoading() { return IsTestLoading; }
		public bool IsTestLoading { get { return isTestLoading; } set { isTestLoading = value; } } 
		protected override GridViewContainerControl CreateContainerControl() { return new GridViewContainerControlTester(this); }
		protected override WebDataProxy  CreateDataProxy() {
 			return new GridWebProxyTester(this, this, this);
		}
		public GridViewCommandColumn CommandColumn { get { return commandColumn; } }
		public DataController DataController { get { return (DataProxy as GridWebProxyTester).DataController; } }
		public DataTable DataTable { get { return table; } }
		public ASPxGridViewTester CreateGridAndPerformPostBack(string args) {
			string reformattedCallbackArgs = ReformatCallbackArgs(args, "GB");
			return CreateGridAndPerformCallBack(string.Empty, reformattedCallbackArgs, false);
		}
		public ASPxGridViewTester CreateGridAndPerformCallBack(string args) {
			string reformattedCallbackArgs = ReformatCallbackArgs(args, "GB");
			return CreateGridAndPerformCallBack(string.Empty, string.Format("GB|{0};{1};", reformattedCallbackArgs.Length, reformattedCallbackArgs), true);
		}
		public ASPxGridViewTester CreateGridAndApplyAddonParams(string prefix, string cbAddonParams) {
			string reformattedCallbackArgs = ReformatCallbackArgs(cbAddonParams, prefix);
			return CreateGridAndPerformCallBack(string.Format("{0}|{1};{2};", prefix, reformattedCallbackArgs.Length, reformattedCallbackArgs), string.Empty, true);
		}
		public ASPxGridViewTester CreateGridAndPerformCallBackRaw(params object[] callbackParams) {
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < callbackParams.Length; i += 2) {
				string prefix = callbackParams[i].ToString();
				string data = callbackParams[1 + i].ToString();
				string reformattedCallbackArgs = ReformatCallbackArgs(data, prefix);
				builder.AppendFormat("{0}|{1};{2};", prefix, reformattedCallbackArgs.Length, reformattedCallbackArgs);
			}
			return CreateGridAndPerformCallBack(builder.ToString(), string.Empty, true);
		}
		public GridViewHtmlTable CreateMainTable() {
			return CreateMainTable(GridViewHtmlTableRenderPart.All);
		}
		public GridViewHtmlTable CreateMainTable(GridViewHtmlTableRenderPart part) {
			GridViewHtmlTable table = new GridViewHtmlTable(RenderHelper, part);
			RenderUtils.GetRenderResult(table);
			return table;
		}
		public static string ReformatCallbackArgs(string args, string prefix) {
			return prefix != "GB" ? args : CommonUtils.SerializeStringArray(args.Split('|'));
		}
		public bool AllowTestGridSaveViewState { get { return allowTestGridSaveViewState; } set { allowTestGridSaveViewState = value; }}
		protected override object SaveViewState() {
			return AllowTestGridSaveViewState ? base.SaveViewState() : null;
		}
		ASPxGridViewTester CreateGridAndPerformCallBack(string cbAddonParams, string args, bool doCallBack) {
			ASPxGridViewTester grid = new ASPxGridViewTester();			
			grid.DataSourceForceStandardPaging = this.DataSourceForceStandardPaging;
			grid.FindControl("any");
			grid.Events.AddHandlers(Events);
			SetCallbackPropertyValue("Data", string.Empty, DataProxy.SaveData());
			SetCallbackPropertyValue("State", string.Empty, SaveGridControlState());
			object viewState = SaveViewState();
			if(viewState != null) {
				grid.LoadViewState(viewState);
			}
			if(doCallBack) {
				string cbString = cbAddonParams + string.Format("CB|{0};{1};", GetCallbackStateString().Length, GetCallbackStateString());
				if(!string.IsNullOrEmpty(args))
					cbString += args;
				ICallbackEventHandler callback = (ICallbackEventHandler)grid;
				callback.RaiseCallbackEvent(cbString);
				grid.CALLBackResultText = callback.GetCallbackResult();
			} else grid.RaisePostBackEvent(args);
			return grid;
		}
		public string CALLBackResultText = string.Empty;
		public string GetFuncCallBackResult(string func, string args) {
			string funcArgs = func + '|' + args;
			string reformattedCallbackArgs = ReformatCallbackArgs(funcArgs, "GB");
			ASPxGridViewTester grid = CreateGridAndPerformCallBack("FB|1;0;", string.Format("GB|{0};{1};", reformattedCallbackArgs.Length, reformattedCallbackArgs), true);
			return (grid as ICallbackEventHandler).GetCallbackResult();
		}
		public string BuildHtml() {
			ClearControlFields();
			EnsureChildControls();
			using(MemoryStream ms = new MemoryStream()) {
				using(StreamWriter sw = new StreamWriter(ms)) {
					HtmlTextWriter writer = new HtmlTextWriter(sw);
					RenderControl(writer);
					ms.Position = 0;
					return GetString(ms.GetBuffer());
				}
			}
		}
		string GetString(byte[] bytes) {
			StringBuilder sb = new StringBuilder();
			foreach(byte b in bytes) { sb.Append((char)b); }
			return sb.ToString();
		}
		public ASPxGridViewTester CreateGridFromStream() {
			ASPxGridViewTester grid = new ASPxGridViewTester();			
			grid.FindControl("any");
			SetCallbackPropertyValue("Data", string.Empty, DataProxy.SaveData());
			SetCallbackPropertyValue("State", string.Empty, SaveGridControlState());
			object viewState = SaveViewState();
			if(viewState != null) {
				grid.LoadViewState(viewState);
			}
			grid.SetCallbackStateString(GetCallbackStateString());
			grid.LoadGridControlState(string.Empty, string.Empty, true);
			return grid;
		}
		protected override void OnDataBinding(EventArgs e) {
			base.OnDataBinding(e);
			this.performSelectCounter++;
		}
		public string CustomCallbackArgs { get { return customCallbackArgs; } }
		public int PerformSelectCounter { get { return performSelectCounter; } set { performSelectCounter = value; } }
		public int SelectionChangedCounter { get { return selectionChangedCounter; } }
		public int FocusedRowChangedCounter { get { return focusedRowChangedCounter; } set { focusedRowChangedCounter = value; } }		
		public bool IsCachedDataProvider { get { return ((GridWebProxyTester)DataProxy).IsCachedDataProvider; } }
		public List<string> SerializedColumns { get { return ((GridWebProxyTester)DataProxy).SerializedColumns; } }
		public string DetailRowExpandedChangedLog {
			get { return GetStringProperty("__DetailRowExpandedChangedLog", string.Empty); }
			set { SetStringProperty("__DetailRowExpandedChangedLog", string.Empty, value); }
		}
		protected override void RaiseCustomCallback(ASPxGridViewCustomCallbackEventArgs e) {
			this.customCallbackArgs = e.Parameters;
		}
		protected void DoControlInit() {
			MethodInfo mi = this.GetType().GetMethod("InitRecursive", BindingFlags.Instance | BindingFlags.NonPublic);
			mi.Invoke(this, new object[] { null });
		}
		void PopulateColumns() {
			foreach(DataColumn column in table.Columns) {
				GridViewDataColumn col = new GridViewDataColumn();
				col.Caption = column.Caption;
				col.FieldName = column.ColumnName;
				Columns.Add(col);
			}
		}
		void ASPxGridViewTester_SelectionChanged(object sender, EventArgs e) {
			selectionChangedCounter++;
		}
		void ASPxGridViewTester_FocusedRowChanged(object sender, EventArgs e) {
			focusedRowChangedCounter++;
		}
	}
	[TestFixture(Description = "This test harness contains tests that check that the edit form editors perform size correction in appropriate scenarios (B134130).")]
	public class GridViewEditFormEditorsSizeRecalculationTests : TestClassBase {
		[Test]
		public void NoWidthCorrectionByDefaultTest() {
			GridViewEditingMode[] editModes = new GridViewEditingMode[] { GridViewEditingMode.EditForm, GridViewEditingMode.Inline };
			foreach(GridViewEditingMode editMode in editModes) {
				ASPxGridView grid = CreateGrid();
				grid.SettingsEditing.Mode = editMode;
				string renderResult = StartEditAndGetRenderResult(grid);
				Assert.IsFalse(IsWidthCorrectionEnabled(renderResult), "Width correction is enabled. Edit mode = '" + editMode.ToString() + "'.");
			}
		}
		[Test]
		public void InlineModeColumnResizingEnablesSizeCorrectionTest() {
			ASPxGridView grid = CreateGrid();
			grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Control;
			string renderResult = StartEditAndGetRenderResult(grid);
			Assert.IsTrue(IsWidthCorrectionEnabled(renderResult), "Width correction is disabled.");
		}
		[Test]
		public void InlineModeGridWidthSpecifiedInPixelsEnablesSizeCorrectionTest() {
			ASPxGridView grid = CreateGrid();
			grid.Width = 640;
			grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
			string renderResult = StartEditAndGetRenderResult(grid);
			Assert.IsTrue(IsWidthCorrectionEnabled(renderResult), "Width correction is disabled.");
		}
		[Test]
		public void EditorWidthSpecifiedInPixelsEnablesSizeCorrectionTest() {
			GridViewEditingMode[] editModes = new GridViewEditingMode[] { GridViewEditingMode.EditForm, GridViewEditingMode.Inline };
			foreach(GridViewEditingMode editMode in editModes) {
				ASPxGridView grid = CreateGrid();
				grid.SettingsEditing.Mode = editMode;
				grid.CellEditorInitialize += new ASPxGridViewEditorEventHandler(delegate(object sender, ASPxGridViewEditorEventArgs args) {
					args.Editor.Width = 50;
				});
				string renderResult = StartEditAndGetRenderResult(grid);
				Assert.IsTrue(IsWidthCorrectionEnabled(renderResult), "Width correction is disabled. Edit mode = '" + editMode.ToString() + "'.");
			}
		}
		private static bool IsWidthCorrectionEnabled(string renderResult) {
			if(renderResult.IndexOf("new ASPxClientTextBox") < 0)
				throw new InvalidOperationException("There are no editors have been rendered, so it makes no sense to check whether width correction is enabled or not.");
			return renderResult.IndexOf("widthCorrectionRequired = true") >= 0;
		}
		private string StartEditAndGetRenderResult(ASPxGridView gridView) {
			DoControlInit(gridView);
			gridView.AddNewRow();
			DoControlPreRender(gridView);
			return base.GetRenderResult(gridView);
		}
		private ASPxGridView CreateGrid() {
			ASPxGridView grid = new ASPxGridView();
			grid.Columns.Add(new GridViewDataColumn());
			grid.DataSource = new object[] { };
			grid.DataBind();
			return grid;
		}
	}
	[TestFixture]
	public class ASPxGridViewBaseEventsTests {
		int rowCreated = 0;
		[Test]
		public void RowCreated() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			this.rowCreated = 0;
			grid.HtmlRowCreated += new ASPxGridViewTableRowEventHandler(grid_RowCreated);
			grid.BuildHtml();
			Assert.AreEqual(this.rowCreated, grid.DataProxy.VisibleRowCount); 
		}
		void grid_RowCreated(object sender, ASPxGridViewTableRowEventArgs e) {
			rowCreated++;
		}
		[Test]
		public void CustomErrorText() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			string sourceText = string.Empty;
			grid.CustomErrorText += delegate(object sender, ASPxGridViewCustomErrorTextEventArgs e) {
				sourceText = e.ErrorText;
				e.ErrorText = "****PASSED****";
			};
			grid.RowInserting += delegate(object sender, ASPxDataInsertingEventArgs e) {
				throw new Exception("TEST");
			};
			grid.AddNewRow();
			grid = grid.CreateGridAndPerformCallBack("UPDATEEDIT");
			Assert.AreEqual("TEST", sourceText);
			Assert.AreEqual(true, grid.CALLBackResultText.Contains("****PASSED****"));
			grid.RowValidating += delegate(object sender, ASPxDataValidationEventArgs e) {
				e.RowError = "VALIDATE";
			};
			grid = grid.CreateGridAndPerformCallBack("UPDATEEDIT");
			Assert.AreEqual("VALIDATE", sourceText);
		}
		[Test]
		public void AutoFilterTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.ProcessColumnAutoFilter += delegate(object sender, ASPxGridViewAutoFilterEventArgs e) {
				if(e.Kind == GridViewAutoFilterEventKind.CreateCriteria) {
					e.Criteria = new BinaryOperator("id", 1);
				}
				if(e.Kind == GridViewAutoFilterEventKind.ExtractDisplayText) {
					e.Value = "10000";
				}
			};
			grid.AutoFilterByColumn(grid.Columns["id"], "100");
			Assert.AreEqual(grid.GetColumnFilter(grid.Columns["id"] as GridViewDataColumn), new BinaryOperator("id", 1));
			Assert.AreEqual("10000", grid.FilterHelper.GetColumnAutoFilterText(grid.Columns["id"] as GridViewDataColumn));
		}
		[Test]
		public void CustomDisplayText() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.CustomColumnDisplayText += delegate(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
				e.DisplayText = e.Value.ToString() + " " + e.VisibleRowIndex.ToString();
			};
			Assert.AreEqual(grid.GetRowValues(1, ((GridViewDataColumn)grid.Columns[0]).FieldName).ToString() + " " + 1, grid.RenderHelper.TextBuilder.GetRowDisplayText(grid.Columns[0] as GridViewDataColumn, 1));
		}
		[Test]
		public void CustomGroupDisplayText() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.CustomGroupDisplayText += delegate(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
				e.DisplayText = e.Value.ToString() + " group";
			};
			grid.GroupBy(grid.Columns["id"]);
			Assert.AreEqual(grid.GetRowValues(0, "id").ToString() + " group", grid.RenderHelper.TextBuilder.GetGroupRowDisplayText(grid.Columns["id"] as GridViewDataColumn, 0));
		}
		[Test]
		public void CustomSortTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SortBy(grid.Columns[0], ColumnSortOrder.Ascending);
			bool raised = false;
			grid.CustomColumnSort += delegate(object sender, CustomColumnSortEventArgs e) {
				raised = true;
			};
			Assert.AreEqual(false, raised);
			((GridViewDataColumn)grid.Columns[0]).Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
			grid.SortBy(grid.Columns[0], ColumnSortOrder.Descending);
			Assert.AreEqual(true, raised);
		}
		[Test]
		public void CustomGroupTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.GroupBy(grid.Columns[0], 0);
			bool raised = false;
			grid.CustomColumnGroup += delegate(object sender, CustomColumnSortEventArgs e) {
				raised = true;
			};
			Assert.AreEqual(false, raised);
			((GridViewDataColumn)grid.Columns[0]).Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
			grid.SortBy(grid.Columns[0], ColumnSortOrder.Descending);
			Assert.AreEqual(true, raised);
		}
		[Test]
		public void NewItemRowTests() {
			NewItemRowTestCore(true);
			NewItemRowTestCore(false);
		}
		void NewItemRowTestCore(bool dataBind) {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			if(!dataBind) {
				grid = grid.CreateGridFromStream();
			}
			grid.InitNewRow += delegate(object sender, ASPxDataInitNewRowEventArgs e) {
				e.NewValues["column1"] = 999;
			};
			grid.AddNewRow();
			Assert.AreEqual(999, grid.DataProxy.GetRowValueForTemplate(ListSourceDataController.NewItemRow, "column1"));
			object row = grid.DataProxy.GetRow(ListSourceDataController.NewItemRow);
			Assert.IsNotNull(row);
			Assert.AreEqual(999, TypeDescriptor.GetProperties(row)["column1"].GetValue(row));
		}
	}
	[TestFixture]
	public class ASPxGridViewAssignTests {
		public void CheckAssignProperties(object obj, string[] names) { CheckAssignProperties(obj, names, false); }
		public void CheckAssignProperties(object obj, string[] names, bool checkCoreTypes) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			int count = 0, errorCount = 0;
			List<string> propNames = new List<string>(names);
			foreach(PropertyDescriptor pd in props) {
				if(!checkCoreTypes && !pd.ComponentType.Equals(obj.GetType())) continue;
				if(pd.SerializationVisibility == DesignerSerializationVisibility.Hidden) continue;
				if(pd.IsReadOnly && pd.PropertyType.IsEnum) continue;
				count++;
				int nameIndex = propNames.IndexOf(pd.Name);
				if(nameIndex == -1) {
					errorCount++;
					System.Console.Error.WriteLine("Expect {0} member", pd.Name);
				}
				else
					propNames.RemoveAt(nameIndex);
			}
			if(propNames.Count != 0) {
				errorCount += propNames.Count;
				for(int i = 0; i < propNames.Count; i++)
					System.Console.Error.WriteLine("Extra {0} member", propNames[i]);
			}
			Assert.IsTrue(errorCount == 0, "Invalid members list. (for more details see Console.Error tab).");
		}
		[Test]
		public void ColumnSettings() {
			GridViewDataColumnSettings settings = new GridViewDataColumnSettings(null);
			CheckAssignProperties(settings, new string[] { 
				"AllowDragDrop",
				"AllowGroup",
				"AllowSort",
				"FilterMode",
				"AllowAutoFilter",
				"AllowHeaderFilter",
				"AutoFilterCondition",
				"AllowAutoFilterTextInputTimer",
				"SortMode",
				"GroupInterval",
				"ShowFilterRowMenu",
				"ShowInFilterControl"
			});
		}
		[Test]
		public void DataColumn() {
			GridViewDataColumn dataColumn = new GridViewDataColumn();
			CheckAssignProperties(dataColumn, new string[] { 
#region Text
				"Grid",
				"Name",
				"Caption",
				"ToolTip",
				"Width",
				"ShowInCustomizationForm",
				"PropertiesEdit", "PropertiesEditType",
				"Visible",
				"VisibleIndex",
				"FixedStyle",
				"HeaderCaptionTemplate",
				"HeaderTemplate",
				"FooterTemplate",
				"HeaderStyle",
				"CellStyle",
				"FooterCellStyle",
				"GroupFooterCellStyle",
				"EditCellStyle",
				"FilterCellStyle",
				"EditFormCaptionStyle",
				"EditFormSettings",
				"EditItemTemplate",
				"DataItemTemplate",
				"ReadOnly",
				"FieldName",
				"UnboundType",
				"GroupIndex",
				"SortIndex",
				"SortOrder",
				"Settings",
				"GroupRowTemplate"
#endregion
			}, true);
		}
		[Test]
		public void CommandColumn() {
			GridViewCommandColumn column = new GridViewCommandColumn();
			CheckAssignProperties(column, new string[] { 
				"AllowDragDrop",
				"ShowSelectCheckbox",
				"ButtonType",
				"EditButton",
				"NewButton",
				"DeleteButton",
				"SelectButton",
				"CancelButton",
				"UpdateButton",
				"ClearFilterButton",
				"CustomButtons"
			});
		}
		[Test]
		public void SummaryItem() {
			ASPxSummaryItem summaryitem = new ASPxSummaryItem();
			CheckAssignProperties(summaryitem, new string[] { 
				"ShowInColumn",
				"ShowInGroupFooterColumn",
				"DisplayFormat",
				"FieldName",
				"Tag",
				"SummaryType"});
		}
		[Test]
		public void CommandColumnButton() {
			GridViewCommandColumnButton button = new GridViewCommandColumnButton(null, ColumnCommandButtonType.New);
			CheckAssignProperties(button, new string[] { 
				"Visible",
				"Image",
				"Text"
			});
		}
		[Test]
		public void CommandColumnCustomButton() {
			GridViewCommandColumnCustomButton button = new GridViewCommandColumnCustomButton();
			CheckAssignProperties(button, new string[] { 
				"Visibility",
				"Image",
				"Text",
				"ID"
			});
		}
	}
	[TestFixture]
	public class ASPxGridViewVisibleColumnsTest {
		[Test]
		public void SimpleVisible() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(grid.Columns.Count, grid.VisibleColumns.Count);
			grid.Columns[0].Visible = false;
			Assert.AreEqual(grid.Columns.Count - 1, grid.VisibleColumns.Count);
			grid.Columns[0].Visible = true;
			Assert.AreEqual(grid.Columns.Count, grid.VisibleColumns.Count);
			Assert.AreEqual(0, grid.Columns[0].VisibleIndex);
			grid.Columns[0].VisibleIndex = -1;
			Assert.AreEqual(grid.Columns.Count - 1, grid.VisibleColumns.Count);
		}
		[Test]
		public void MoveVisible() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Columns[3].VisibleIndex = 0;
			Assert.AreEqual(0, grid.Columns[3].VisibleIndex);
			Assert.AreEqual(0, grid.VisibleColumns.IndexOf(grid.Columns[3]));
			Assert.AreEqual(1, grid.Columns[0].VisibleIndex);
		}
		[Test]
		public void AddColumn() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			GridViewColumn col = new GridViewDataColumn();
			int vcount = grid.VisibleColumns.Count;
			grid.Columns.Add(col);
			Assert.AreEqual(vcount + 1, grid.VisibleColumns.Count);
			Assert.AreEqual(vcount, col.VisibleIndex);
		}
		[Test]
		public void UseState() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Columns[3].Visible = false;
			grid.Columns[2].VisibleIndex = 0;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(false, grid.Columns[3].Visible);
			Assert.AreEqual(0, grid.Columns[2].VisibleIndex);
		}
		[Test]
		public void MoveThroughCallBackFromStart() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|2|true|");
			Assert.AreEqual(1, grid.Columns[0].VisibleIndex);
		}
		[Test]
		public void MoveThroughCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|2|0||");
			Assert.AreEqual(0, grid.Columns[2].VisibleIndex);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|2|-1||");
			Assert.AreEqual(-1, grid.Columns[2].VisibleIndex);
			Assert.AreEqual(false, grid.Columns[2].Visible);
		}
		[Test]
		public void MoveAllInVisibleThroughCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			for(int n = 0; n < grid.Columns.Count; n++) {
				grid.Columns[n].Visible = false;
				grid.Columns[n].VisibleIndex = -1;
			}
			Assert.AreEqual(0, grid.VisibleColumns.Count);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|0||");
			Assert.AreEqual(1, grid.VisibleColumns.Count);
		}
		[Test]
		public void VisibleInVisibleThroughCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|-1||");
			Assert.AreEqual(false, grid.Columns[0].Visible);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|1||");
			Assert.AreEqual(true, grid.Columns[0].Visible);
		}
		[Test]
		public void DataBindOnShowingInvisibleColumn() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.DataProxy.GetRowValue(5, "column1");
			int index = grid.Columns["column1"].Index;
			grid = grid.CreateGridAndPerformCallBack(string.Format("COLUMNMOVE|{0}|-1||", index));
			grid = grid.CreateGridFromStream();
			grid.DataBind();
			grid = grid.CreateGridAndPerformCallBack(string.Format("COLUMNMOVE|{0}|0||", index));
			Assert.AreEqual(value, grid.DataProxy.GetRowValue(5, "column1"));
		}
		[Test]
		public void MoveFromGroup() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|0|false|true|");
			Assert.AreEqual(1, grid.GroupCount);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|1|false|false|");
			Assert.AreEqual(1, grid.GroupCount);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|1|false|false|true|");
			Assert.AreEqual(0, grid.GroupCount);
		}
		[Test]
		public void FixedColumnsTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(grid.Columns[0], grid.VisibleColumns[0]);
			grid.Columns[2].FixedStyle = GridViewColumnFixedStyle.Left;
			Assert.AreEqual(grid.Columns[2], grid.VisibleColumns[0]);
			grid.Columns[2].FixedStyle = GridViewColumnFixedStyle.None;
			grid.Columns[2].VisibleIndex = 2;
			Assert.AreEqual(grid.Columns[0], grid.VisibleColumns[0]);
		}
		[Test]
		public void FixedColumnCount() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(0, grid.FixedColumnCount);
			grid.Columns[2].FixedStyle = GridViewColumnFixedStyle.Left;
			Assert.AreEqual(1, grid.FixedColumnCount);
			grid.Columns[1].FixedStyle = GridViewColumnFixedStyle.Left;
			Assert.AreEqual(2, grid.FixedColumnCount);
			grid.Columns[1].FixedStyle = GridViewColumnFixedStyle.None;
			grid.Columns[2].FixedStyle = GridViewColumnFixedStyle.None;
			Assert.AreEqual(0, grid.FixedColumnCount);
		}
		[Test]
		public void RenderHelperHasFixedColumns() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.Columns[2].FixedStyle = GridViewColumnFixedStyle.Left;
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.Settings.ShowHorizontalScrollBar = true;
			Assert.IsTrue(grid.RenderHelper.HasFixedColumns);
			grid.GroupBy(grid.Columns[1]);
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.ClearSort();
			Assert.IsTrue(grid.RenderHelper.HasFixedColumns);
			grid.Settings.ShowPreview = true;
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.Settings.ShowPreview = false;
			Assert.IsTrue(grid.RenderHelper.HasFixedColumns);
			grid.SettingsDetail.ShowDetailRow = true;
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.SettingsDetail.ShowDetailRow = false;
			Assert.IsTrue(grid.RenderHelper.HasFixedColumns);
			grid.Templates.DataRow = new MyTestTemplateButton();
			Assert.IsFalse(grid.RenderHelper.HasFixedColumns);
			grid.Templates.DataRow = null;
			Assert.IsTrue(grid.RenderHelper.HasFixedColumns);
		}
		[Test]
		public void MoveFixedColumn() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(grid.VisibleColumns[1], grid.Columns[1]);
			grid.Columns[1].FixedStyle = GridViewColumnFixedStyle.Left;
			Assert.AreEqual(grid.VisibleColumns[0], grid.Columns[1]);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|3|true|");
			Assert.AreEqual(grid.VisibleColumns[0], grid.Columns[1]);
		}
	}
	[TestFixture]
	public class ASPxGridViewDataTest {
		[Test]
		public void SimpleSorting() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].SortAscending();
			Assert.AreEqual(1, grid.SortCount);
			Assert.AreEqual(0, grid.DataColumns[0].SortIndex);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[0].SortOrder);
			grid.DataColumns[0].SortAscending();
			grid.DataColumns[0].SortDescending();
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[0].SortOrder);
			grid.DataColumns[0].UnSort();
			Assert.AreEqual(ColumnSortOrder.None, grid.DataColumns[0].SortOrder);
			Assert.AreEqual(0, grid.SortCount);
		}
		[Test]
		public void SimpleFiltering() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			int count = grid.DataProxy.VisibleRowCount;
			grid.FilterExpression = "id <> 1";
			Assert.AreNotEqual(count, grid.DataProxy.VisibleRowCount);
			grid.FilterExpression = "";
			Assert.AreEqual(count, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void SimpleColumnFiltering() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			int count = grid.DataProxy.VisibleRowCount;
			grid.DataColumns[0].AutoFilterBy("1");
			Assert.AreNotEqual(count, grid.DataProxy.VisibleRowCount);
			grid.DataColumns[0].AutoFilterBy("");
			Assert.AreEqual(count, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void SimpleGrouping2() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(1, grid.GetGroupedColumns().Count);
			Assert.AreEqual(grid.DataColumns[0], grid.GetGroupedColumns()[0]);
			grid.DataColumns[0].SortIndex = -1;
			Assert.AreEqual(0, grid.GroupCount);
			Assert.AreEqual(0, grid.GetGroupedColumns().Count);
			Assert.AreEqual(-1, grid.DataColumns[0].GroupIndex);
		}
		[Test]
		public void SetGroupIndex() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			GridViewDataColumn column = grid.DataColumns[0];
			grid.Columns.Remove(column);
			column.GroupIndex = 0;
			grid.Columns.Add(column);
			grid.DataBind();
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(ColumnSortOrder.Ascending, column.SortOrder);
		}
		[Test]
		public void SimpleGroupingAndSorting() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			Assert.AreEqual(1, grid.GroupCount);
			grid.DataColumns[1].SortOrder = ColumnSortOrder.Ascending;
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(-1, grid.DataColumns[1].GroupIndex);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[1].SortOrder);
		}
		[Test]
		public void SimpleExGrouping() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid.DataColumns[1].SortIndex = 0;
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(-1, grid.DataColumns[1].GroupIndex);
		}
		[Test]
		public void SimpleGrouping() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			grid.DataColumns[0].UnGroup();
			Assert.AreEqual(0, grid.GroupCount);
			Assert.AreEqual(-1, grid.DataColumns[0].GroupIndex);
		}
		[Test]
		public void SortIndexTest() {
			GridViewDataTextColumn column = new GridViewDataTextColumn();
			column.SortIndex = -1;
			column.GroupIndex = -1;
			Assert.AreEqual(-1, column.SortIndex); 
			Assert.AreEqual(-1, column.GroupIndex);
			column.GroupIndex = 0;
			column.SortIndex = -1;
			Assert.AreEqual(-1, column.SortIndex);
		}
		[Test]
		public void SimpleTwoColumnGrouping() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid.DataColumns[1].GroupBy();
			Assert.AreEqual(2, grid.GroupCount);
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(1, grid.DataColumns[1].GroupIndex);
			grid.DataColumns[0].UnGroup();
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(-1, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(0, grid.DataColumns[1].GroupIndex);
		}
		[Test]
		public void DataControllerSynhronization() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid.DataColumns[1].GroupBy();
			grid.DataBind();
			Assert.AreEqual(2, grid.DataController.GroupedColumnCount);
		}
		[Test]
		public void ExpandCollapseTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			int visibleCount1 = grid.DataController.VisibleCount;
			grid.DataColumns[0].GroupBy();
			int visibleCount2 = grid.DataController.VisibleCount;
			grid.ExpandAll();
			Assert.AreEqual(visibleCount1 + visibleCount2, grid.DataController.VisibleCount);
			grid.CollapseAll();
			Assert.AreEqual(visibleCount2, grid.DataController.VisibleCount);
		}
		[Test]
		public void ExpandCollapseSingleRowTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			int visibleCount1 = grid.DataController.VisibleCount;
			grid.DataColumns[0].GroupBy();
			int visibleCount2 = grid.DataController.VisibleCount;
			grid.ExpandRow(0, true);
			Assert.AreNotEqual(visibleCount1, grid.DataController.VisibleCount);
			grid.CollapseRow(0, true);
			Assert.AreEqual(visibleCount2, grid.DataController.VisibleCount);
		}
		[Test]
		public void SimpleSortingSortBy() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			GridViewDataColumn column = grid.DataColumns[0];
			grid.SortBy(column, ColumnSortOrder.Descending);
			Assert.AreEqual(1, grid.SortedColumns.Count);
			Assert.AreEqual(ColumnSortOrder.Descending, grid.SortedColumns[0].SortOrder);
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataController.SortInfo[0].SortOrder);
			grid.SortBy(column, ColumnSortOrder.Ascending);
			Assert.AreEqual(1, grid.SortedColumns.Count);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.SortedColumns[0].SortOrder);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataController.SortInfo[0].SortOrder);
		}
		[Test]
		public void PageIndexOutOfBoundsAfterGrouping() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.PageIndex = 1;
			grid = grid.CreateGridAndPerformCallBack("GROUP|1||");
			Assert.AreEqual(grid.PageCount - 1, grid.PageIndex);
		}
		[Test]
		public void GetDataRowTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Assert.AreEqual(grid.DataTable.Rows[1], grid.GetDataRow(1));
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(grid.DataTable.Rows[1], grid.GetDataRow(1));
		}
		[Test]
		public void AutoExpandAllGroupsTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsBehavior.AutoExpandAllGroups = true;
			grid.DataBind();
			grid.DataColumns[0].GroupIndex = 0;
			Assert.AreEqual(true, grid.IsRowExpanded(0));
		}
		[Test]
		public void Q109068() {
			ASPxGridView grid = new ASPxGridView();
			grid.SettingsBehavior.AutoExpandAllGroups = true;
			grid.GroupSummary.Add(SummaryItemType.Count, "A");
			int count = 0;
			grid.DataBinding += delegate(object s, EventArgs e) {
				if(++count > 10) Assert.Fail();
			};
			typeof(Control).GetMethod("InitRecursive", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(grid, new object[] { null });
			typeof(Control).GetMethod("LoadRecursive", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(grid, new object[0]);
			grid.GroupSummary[0].SummaryType = SummaryItemType.None;			
		}
		[Test]
		public void B96508() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.SettingsBehavior.AllowFocusedRow = true;
			bool flag = false;
			tester.FocusedRowChanged += delegate(object s, EventArgs e) {
				flag = true;
			};
			tester.DataBind();			
			int index = tester.VisibleRowCount - 1;
			tester.FocusedRowIndex = index;
			tester.DeleteRow(index);
			Assert.IsTrue(flag);
			Assert.AreEqual(index - 1, tester.FocusedRowIndex);
		}
		[Test]
		public void B97135() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.GroupBy(tester.DataColumns[0]);
			tester.ExpandRow(0);
			tester.ExpandRow(2);
			Assert.IsTrue(tester.IsRowExpanded(0) && tester.IsRowExpanded(2));
			tester.StartEdit(1);
			tester.UpdateEdit();
			Assert.IsTrue(tester.IsRowExpanded(0) && tester.IsRowExpanded(2));
		}
		[Test]
		public void B32947() {
			ASPxGridView gridView = new ASPxGridView();
			GridViewDataTextColumn column = new GridViewDataTextColumn();
			column.FieldName = "DataField";
			gridView.Columns.Add(column);
			column.SortIndex = 0;
			column.SortOrder = ColumnSortOrder.Descending;
			column.SortIndex = 0;
			Assert.AreEqual(ColumnSortOrder.Descending, column.SortOrder);
		}
		[Test]
		public void B133585() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.GroupBy(tester.Columns["column1"]);			
			Assert.AreEqual(-1, tester.FindVisibleIndexByKeyValue(1));
		}
	}
	[TestFixture]
	public class ASPxGridViewCustomCallBackTests {
		[Test]
		public void AllowFilterTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid = grid.CreateGridAndPerformCallBack("CUSTOMCALLBACK|customerback:100");
			Assert.AreEqual("customerback:100", grid.CustomCallbackArgs);
		}
	}
	[TestFixture]
	public class ASPxGridViewFilterTests {
		[Test]
		public void CallBackFilterTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			int rowCount = grid.DataProxy.VisibleRowCount;
			grid = grid.CreateGridAndPerformCallBack("APPLYCOLUMNFILTER|2|str1");
			Assert.AreNotEqual(0, grid.DataProxy.VisibleRowCount);
			Assert.AreNotEqual(rowCount, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void NeedToRenderFilterPopup() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			Assert.AreEqual(false, grid.RenderHelper.RequireRenderFilterPopupWindow);
			grid.Settings.ShowHeaderFilterButton = true;
			Assert.AreEqual(true, grid.RenderHelper.RequireRenderFilterPopupWindow);
			grid.Settings.ShowHeaderFilterButton = false;
			grid.DataColumns[0].Settings.AllowHeaderFilter = DefaultBoolean.True;
			Assert.AreEqual(true, grid.RenderHelper.RequireRenderFilterPopupWindow);
			grid.DataColumns[0].Settings.AllowHeaderFilter = DefaultBoolean.Default;
			Assert.AreEqual(false, grid.RenderHelper.RequireRenderFilterPopupWindow);
		}
		[Test]
		public void IsHeaderFilterItemActive() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			GridViewFilterPopupContainerOwner popupOwner = new GridViewFilterPopupContainerOwner(grid.DataColumns[0], true);
			Assert.AreEqual(true, popupOwner.IsFilterValueActive(popupOwner.FilterValues[0]));
			Assert.AreEqual(false, popupOwner.IsFilterValueActive(popupOwner.FilterValues[10]));
			grid.FilterByHeaderPopup(grid.DataColumns[0], popupOwner.FilterValues[10].HtmlValue);
			popupOwner = new GridViewFilterPopupContainerOwner(grid.DataColumns[0], true);
			Assert.AreEqual(false, popupOwner.IsFilterValueActive(popupOwner.FilterValues[0]));
			Assert.AreEqual(true, popupOwner.IsFilterValueActive(popupOwner.FilterValues[10]));
		}
		[Test]
		public void FilterEnabledTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			int count = grid.DataProxy.VisibleRowCount;
			grid.FilterExpression = "id <> 1";
			Assert.AreEqual(count - 1, grid.DataProxy.VisibleRowCount);
			string oldFilterExpression = grid.FilterExpression;
			grid.FilterEnabled = false;
			Assert.AreEqual(count, grid.DataProxy.VisibleRowCount);
			Assert.AreEqual(oldFilterExpression, grid.FilterExpression);
			grid.FilterEnabled = true;
			Assert.AreEqual(count - 1, grid.DataProxy.VisibleRowCount);
			grid.FilterEnabled = false;
			Assert.AreEqual(count, grid.DataProxy.VisibleRowCount);
			grid.FilterExpression = "id <> 2";
			Assert.AreEqual(true, grid.FilterEnabled);
			Assert.AreEqual(count - 1, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void ShowColumnsInFilterControl() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			Assert.AreEqual(grid.DataTable.Columns.Count, (grid as IFilterControlOwner).ColumnCount);
			grid.DataColumns[0].Settings.ShowInFilterControl = DefaultBoolean.False;
			Assert.AreEqual(grid.DataTable.Columns.Count - 1, (grid as IFilterControlOwner).ColumnCount);
			grid.DataColumns[0].Settings.ShowInFilterControl = DefaultBoolean.True;
			Assert.AreEqual(grid.DataTable.Columns.Count, (grid as IFilterControlOwner).ColumnCount);
			grid.DataColumns[0].Visible = false;
			grid.DataColumns[0].Settings.ShowInFilterControl = DefaultBoolean.Default;
			Assert.AreEqual(grid.DataTable.Columns.Count - 1, (grid as IFilterControlOwner).ColumnCount);
			grid.DataColumns[0].Settings.ShowInFilterControl = DefaultBoolean.True;
			Assert.AreEqual(grid.DataTable.Columns.Count, (grid as IFilterControlOwner).ColumnCount);
		}
	}
	[TestFixture]
	public class ASPxGridViewLockUpdateTests {
		[Test]
		public void SimpleTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(false);
			Assert.AreEqual(false, grid.IsLockUpdate);
			grid.BeginUpdate();
			Assert.AreEqual(true, grid.IsLockUpdate);
			grid.BeginUpdate();
			Assert.AreEqual(true, grid.IsLockUpdate);
			grid.EndUpdate();
			Assert.AreEqual(true, grid.IsLockUpdate);
			grid.EndUpdate();
			Assert.AreEqual(false, grid.IsLockUpdate);
		}
		[Test]
		public void GroupByTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(false);
			grid.BeginUpdate();
			grid.GroupBy(grid.DataColumns[0]);
			Assert.AreEqual(0, grid.DataController.GroupedColumnCount);
			grid.EndUpdate();
			Assert.AreEqual(1, grid.DataController.GroupedColumnCount);
		}
		[Test]
		public void GroupByTests2() {
			ASPxGridViewTester grid = new ASPxGridViewTester(false);
			grid.GroupBy(grid.DataColumns[0]);
			grid.BeginUpdate();
			grid.ClearSort();
			Assert.AreEqual(1, grid.DataController.GroupedColumnCount);
			grid.EndUpdate();
			Assert.AreEqual(0, grid.DataController.GroupedColumnCount);
		}
		[Test]
		public void FilterTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(false);
			grid.BeginUpdate();
			grid.AutoFilterByColumn(grid.DataColumns[0], "%");
			Assert.AreEqual("", grid.DataController.FilterExpression);
			grid.EndUpdate();
			Assert.AreNotEqual("", grid.DataController.FilterExpression);
		}
	}
	[TestFixture]
	public class ASPxGridViewCallbacksTest {
		[Test]
		public void Paging() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsPager.PageSize = 1;
			grid = grid.CreateGridAndPerformCallBack("NEXTPAGE");
			Assert.AreEqual(1, grid.PageIndex);
			grid = grid.CreateGridAndPerformCallBack("PREVPAGE");
			Assert.AreEqual(0, grid.PageIndex);
			grid = grid.CreateGridAndPerformCallBack("GOTOPAGE|3");
			Assert.AreEqual(3, grid.PageIndex);
			grid = grid.CreateGridAndPerformCallBack("GOTOPAGE|-1");
			Assert.AreEqual(-1, grid.PageIndex);
		}
		[Test]
		public void Selecting() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SELECTROWS|true|1");
			Assert.AreEqual(true, grid.Selection.IsRowSelected(1));
			Assert.AreEqual(1, grid.Selection.Count);
			grid = grid.CreateGridAndPerformCallBack("SELECTROWS|false|1");
			Assert.AreEqual(false, grid.Selection.IsRowSelected(1));
			grid = grid.CreateGridAndPerformCallBack("SELECTROWS|all|1");
			Assert.AreEqual(true, grid.Selection.IsRowSelected(0));
			Assert.AreEqual(grid.VisibleRowCount, grid.Selection.Count);
			grid.Selection.UnselectAll();
			grid = grid.CreateGridAndPerformCallBack("SELECTROWS|true|1|3");
			Assert.AreEqual(2, grid.Selection.Count);
			Assert.AreEqual(true, grid.Selection.IsRowSelected(1));
			Assert.AreEqual(true, grid.Selection.IsRowSelected(3));
		}
		[Test]
		public void SelectingByKey() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid = grid.CreateGridAndPerformCallBack("SELECTROWSKEY|true|" + grid.DataProxy.GetRowKeyValue(1).ToString());
			Assert.AreEqual(true, grid.Selection.IsRowSelected(1));
			Assert.AreEqual(1, grid.Selection.Count);
			grid = grid.CreateGridAndPerformCallBack("SELECTROWSKEY|false|" + grid.DataProxy.GetRowKeyValue(1).ToString());
			Assert.AreEqual(false, grid.Selection.IsRowSelected(1));
			grid.Selection.UnselectAll();
			grid = grid.CreateGridAndPerformCallBack("SELECTROWSKEY|true|" + grid.DataProxy.GetRowKeyValue(1).ToString() + "|" +grid.DataProxy.GetRowKeyValue(3).ToString());
			Assert.AreEqual(2, grid.Selection.Count);
			Assert.AreEqual(true, grid.Selection.IsRowSelected(1));
			Assert.AreEqual(true, grid.Selection.IsRowSelected(3));
		}
		[Test]
		public void B95795() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.DataBind();
			tester.CustomErrorText += delegate(object s, ASPxGridViewCustomErrorTextEventArgs e) {
				Assert.Fail();
			};
			tester = tester.CreateGridAndPerformCallBackRaw("GB", "SELECTROWS|all|0");
			tester = tester.CreateGridAndPerformCallBackRaw("SR", "".PadLeft(tester.SettingsPager.PageSize, 'T'), "GB", "PAGERONCLICK|PN1");
		}
	}
	[TestFixture]
	public class ASPxGridViewControlStateTest {
		[Test]
		public void ChangeGroupAndSorting() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid.DataColumns[1].SortOrder = ColumnSortOrder.Descending;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(-1, grid.DataColumns[2].GroupIndex);
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[1].SortOrder);
		}
		[Test]
		public void CallBackSortTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SORT|1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformCallBack("SORT|1||");
			Assert.AreEqual(2, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformCallBack("SORT|1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
		}
		[Test]
		public void CallBackSortByIdFieldTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SORT|column1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformCallBack("SORT|column1||");
			Assert.AreEqual(2, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformCallBack("SORT|column1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
		}
		[Test]
		public void PostBackSortTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformPostBack("SORT|1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformPostBack("SORT|1||");
			Assert.AreEqual(2, grid.DataProxy.GetRowValue(0, "column1"));
			grid = grid.CreateGridAndPerformPostBack("SORT|1||");
			Assert.AreEqual(0, grid.DataProxy.GetRowValue(0, "column1"));
		}
		[Test]
		public void UseCachedDataProvider() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(1, grid.GroupCount);
		}
		[Test]
		public void UseCachedDataProviderAndExpandAll() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid.DataBind();
			grid.ExpandAll();
			int visibleCount = grid.DataController.VisibleCount;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(true, grid.DataProxy.IsRowExpanded(0));
			grid = grid.CreateGridAndPerformCallBack("NEXTPAGE");
			Assert.AreEqual(visibleCount, grid.DataController.VisibleCount);
		}
		[Test]
		public void UseCachedDataSourceOnFilter() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			string filterExperssion = "[id] = 1";
			grid.FilterExpression = filterExperssion;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(filterExperssion, grid.FilterExpression);
			grid = grid.CreateGridAndPerformCallBack("NEXTPAGE");
			Assert.AreEqual(filterExperssion, grid.FilterExpression);
		}
		[Test]
		public void TestPerformSelectCount() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SORT|0||ASC|");
			Assert.AreEqual(1, grid.PerformSelectCounter);
		}
		[Test]
		public void TestSortCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SORT|0||ASC|");
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[0].SortOrder);
			grid = grid.CreateGridAndPerformCallBack("SORT|0||DSC|");
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[0].SortOrder);
			grid = grid.CreateGridAndPerformCallBack("SORT|1|||");
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[1].SortOrder);
			grid = grid.CreateGridAndPerformCallBack("SORT|1|||");
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[1].SortOrder);
		}
		[Test]
		public void SortingAndGroupingCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[0].GroupBy();
			grid = grid.CreateGridAndPerformCallBack("SORT|1|||");
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[1].SortOrder);
			grid = grid.CreateGridAndPerformCallBack("SORT|1|||");
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[1].SortOrder);
		}
		[Test]
		public void MultiplyGroupingCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("GROUP|0||");
			Assert.AreEqual(1, grid.GroupCount);
			grid = grid.CreateGridAndPerformCallBack("GROUP|1||");
			Assert.AreEqual(2, grid.GroupCount);
		}
		[Test]
		public void TestMoveToGroupCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|0|true|true");
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|1|0|true|true");
			Assert.AreEqual(2, grid.GroupCount);
			Assert.AreEqual(1, grid.DataColumns[0].GroupIndex);
			Assert.AreEqual(0, grid.DataColumns[1].GroupIndex);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|0|true|false|true");
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(-1, grid.DataColumns[0].GroupIndex);
		}
		[Test]
		public void SubmitOnSecondPage() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.PageIndex ++;
			grid.BuildHtml();
			Assert.AreEqual(1, grid.PageIndex);
			grid = grid.CreateGridFromStream();
			grid.BuildHtml();
			Assert.AreEqual(1, grid.PageIndex);
		}
		[Test]
		public void CheckDataInCachedDataOnGroup() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataColumns[2].GroupBy();
			grid.DataColumns[2].SortDescending();
			grid.BuildHtml();
			object value1 = grid.DataProxy.GetRowValue(5, "column1");
			object value2 = grid.DataProxy.GetRowValue(5, "column2");
			object value3 = grid.DataProxy.GetRowValue(5, "column3");
			grid = grid.CreateGridFromStream();
			grid.BuildHtml();
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(ColumnSortOrder.Descending, grid.DataColumns[2].SortOrder);
			Assert.AreEqual(value1, grid.DataProxy.GetRowValue(5, "column1"));
			Assert.AreEqual(value2, grid.DataProxy.GetRowValue(5, "column2"));
			Assert.AreEqual(value3, grid.DataProxy.GetRowValue(5, "column3"));
		}
		[Test]
		public void SaveColumnFieldsOnRequest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.BuildHtml();
			object value1 = grid.DataProxy.GetRowValue(5, "column1");
			Assert.AreEqual(grid.VisibleColumns.Count, grid.SerializedColumns.Count);
			grid = grid.CreateGridFromStream();
			grid.BuildHtml();
			Assert.AreEqual(value1, grid.DataProxy.GetRowValue(5, "column1"));
		}
		[Test]
		public void DisableRowCacheOnCallbackState() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.EnableRowsCache = false;
			object id = grid.GetRowValues(0, "id");
			Assert.AreEqual(true, grid.DataProxy.IsBound);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(id, grid.GetRowValues(0, "id"));
			Assert.AreEqual(true, grid.DataProxy.IsBound);
		}
		[Test]
		public void DisableRowCacheOnCallbackStateAndSorting() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.EnableRowsCache = false;
			grid.SortBy(grid.DataColumns[0], ColumnSortOrder.Descending);
			int rowCount = grid.DataProxy.VisibleRowCount;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(rowCount, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void DisableRowCacheSelectionChangedTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.EnableRowsCache = false;
			grid = grid.CreateGridAndApplyAddonParams("SR", "FFTT");
			Assert.AreEqual(true, grid.DataProxy.IsBound);
		}
		[Test]
		public void DisableRowCacheExpandCollapseTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.EnableRowsCache = false;
			grid.GroupBy(grid.DataColumns[0]);
			grid.ExpandRow(0, false);
			grid.CreateGridFromStream();
			Assert.AreEqual(true, grid.IsRowExpanded(0));
		}
	}
	[TestFixture]
	public class ASPxGridViewColumnsTest {
		private const bool IsDataColumn = true;
		private const bool IsVisibleColumn = true;
		[Test]
		public void VisibleDataColumnsRetrievalTest() {
			ASPxGridView gridView = new ASPxGridView();
			gridView.Columns.Add(Column("Column1", !IsDataColumn, IsVisibleColumn));
			gridView.Columns.Add(Column("Column2", !IsDataColumn, !IsVisibleColumn));
			gridView.Columns.Add(Column("Column3", IsDataColumn, IsVisibleColumn));
			gridView.Columns.Add(Column("Column4", IsDataColumn, !IsVisibleColumn));
			IList<GridViewDataColumn> visibleDataColumns =
				(IList<GridViewDataColumn>)ReflectionUtils.GetNonPublicInstancePropertyValue(gridView, "VisibleDataColumns");
			Assert.IsTrue(visibleDataColumns.IsReadOnly, "Visible data column collection must be readonly.");
			Assert.AreEqual(1, visibleDataColumns.Count, "Invalid visible data columns count.");
			Assert.AreEqual("Column3", visibleDataColumns[0].Name, "Invalid single visible data column name.");
		}
		[Test]
		public void ColumnToString() {
			GridViewDataColumn column = new GridViewDataColumn();
			Assert.AreEqual(string.Empty, column.ToString());
			column.FieldName = "field";
			Assert.AreEqual("field", column.ToString());
			column.FieldName = "CustomerName";
			Assert.AreEqual("Customer Name", column.ToString());
			column.FieldName = "MyCustomerName";
			Assert.AreEqual("My Customer Name", column.ToString());
			column.FieldName = "TEST";
			Assert.AreEqual("TEST", column.ToString());
			column.FieldName = "ASPTest";
			Assert.AreEqual("ASP Test", column.ToString());
			column.Caption = "Name";
			Assert.AreEqual("Name", column.ToString());
		}
		[Test]
		public void FindColumnByIdFieldNameCaptionTest1() {
			ASPxGridView grid = new ASPxGridView();
			GridViewDataColumn column = new GridViewDataColumn();
			grid.Columns.Add(column);
			column.Name = "id1";
			Assert.AreEqual(null, grid.Columns["id2"]);
			Assert.AreEqual(null, grid.Columns["id3"]);
			Assert.AreEqual(grid.Columns[0], grid.Columns["id1"]);
			column.FieldName = "id2";
			Assert.AreEqual(grid.Columns[0], grid.Columns["id2"]);
			column.Caption = "id3";
			Assert.AreEqual(grid.Columns[0], grid.Columns["id3"]);
		}
		[Test]
		public void AllowTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			Assert.AreEqual(true, grid.Columns[0].GetAllowAutoFilter());
			Assert.AreEqual(false, grid.CommandColumn.GetAllowAutoFilter());
			Assert.AreEqual(true, grid.Columns[0].GetAllowGroup());
			Assert.AreEqual(true, grid.Columns[0].GetAllowSort());
			Assert.AreEqual(false, grid.CommandColumn.GetAllowSort());
			grid.SettingsBehavior.AllowSort = false;
			Assert.AreEqual(false, grid.Columns[0].GetAllowSort());
			((GridViewDataColumn)grid.Columns[0]).Settings.AllowSort = DefaultBoolean.True;
			Assert.AreEqual(true, grid.Columns[0].GetAllowSort());
			grid.SettingsBehavior.AllowSort = true;
			((GridViewDataColumn)grid.Columns[0]).Settings.AllowSort = DefaultBoolean.False;
			Assert.AreEqual(false, grid.Columns[0].GetAllowSort());
			((GridViewDataColumn)grid.Columns[0]).Settings.AllowSort = DefaultBoolean.Default;
			grid.SettingsBehavior.AllowGroup = false;
			Assert.AreEqual(false, grid.Columns[0].GetAllowGroup());
			((GridViewDataColumn)grid.Columns[0]).Settings.AllowGroup = DefaultBoolean.True;
			Assert.AreEqual(true, grid.Columns[0].GetAllowGroup());
			grid.SettingsBehavior.AllowGroup = true;
			((GridViewDataColumn)grid.Columns[0]).Settings.AllowGroup = DefaultBoolean.False;
			Assert.AreEqual(false, grid.Columns[0].GetAllowGroup());
		}
		private GridViewColumn Column(string name, bool isDataColumn, bool isVisible) {
			GridViewColumn column = isDataColumn ?
				(GridViewColumn)(new GridViewDataColumn()) : (GridViewColumn)(new GridViewCommandColumn());
			column.Name = name;
			column.Visible = isVisible;
			return column;
		}
	}
	[TestFixture]
	public class ASPxGridViewContainterControlTest {
		[Test]		
		public void TableColumnCountTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Assert.AreEqual(grid.DataTable.Columns.Count, grid.RenderHelper.ColumnSpanCount);
		}
		[Test]
		public void TableNoVisibleColumnsColumnCountTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			foreach(GridViewColumn column in grid.Columns) {
				column.Visible = false;
			}
			grid.RenderHelper.ClearControlHierarchy();
			Assert.AreEqual(1, grid.RenderHelper.ColumnSpanCount);
			grid.BuildHtml();
			TableRow row = grid.FindControl("DXDataRow0") as TableRow;
			Assert.IsNotNull(row);
			Assert.AreEqual(1, row.Cells.Count);
		}
		[Test]
		public void TableGroupAndNoVisibleColumnsColumnCountTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			foreach(GridViewColumn column in grid.Columns) {
				column.Visible = false;
			}
			grid.GroupBy(grid.Columns[0] as GridViewDataColumn);
			grid.RenderHelper.ClearControlHierarchy();
			Assert.AreEqual(1 + 1, grid.RenderHelper.ColumnSpanCount);
		}
		[Test]
		public void RenderHelperCommandColumnsCountTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(0, grid.RenderHelper.CommandColumnsCount);
			grid.Columns.Add(new GridViewCommandColumn());
			grid.RenderHelper.ClearControlHierarchy();
			Assert.AreEqual(1, grid.RenderHelper.CommandColumnsCount);
			grid.Columns.Add(new GridViewCommandColumn());
			grid.RenderHelper.ClearControlHierarchy();
			Assert.AreEqual(2, grid.RenderHelper.CommandColumnsCount);
			grid.Columns[grid.Columns.Count - 1].Visible = false;
			grid.RenderHelper.ClearControlHierarchy();
			Assert.AreEqual(1, grid.RenderHelper.CommandColumnsCount);
		}
		[Test]
		public void TableColumnCountAfterGroupingTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Settings.ShowGroupedColumns = true;
			grid.DataColumns[0].GroupBy();
			grid.DataBind();
			Assert.AreEqual(grid.DataTable.Columns.Count + 1, grid.RenderHelper.ColumnSpanCount);
		}
		[Test]
		public void GenerateWithoutKeyFieldName() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.KeyFieldName = string.Empty;
			grid.DataBind();
			grid.BuildHtml();
		}
		[Test]
		public void RenderStatusBar() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderStatusBar);
			grid.Settings.ShowStatusBar = GridViewStatusBarMode.Visible;
			Assert.AreEqual(true, new GridViewContainerControl(grid).RequireRenderStatusBar);
			grid.Settings.ShowStatusBar = GridViewStatusBarMode.Auto;
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderStatusBar);
			grid.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.ShowOnStatusBar;
			Assert.AreEqual(true, new GridViewContainerControl(grid).RequireRenderStatusBar);
			grid.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderStatusBar);
		}
		[Test]
		public void RenderFilterBar() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderFilterBar);
			grid.Settings.ShowFilterBar = GridViewStatusBarMode.Visible;
			Assert.AreEqual(true, new GridViewContainerControl(grid).RequireRenderFilterBar);
			grid.Settings.ShowFilterBar = GridViewStatusBarMode.Auto;
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderFilterBar);
			grid.FilterExpression = "id is not null";
			Assert.AreEqual(true, new GridViewContainerControl(grid).RequireRenderFilterBar);
			grid.Settings.ShowFilterBar = GridViewStatusBarMode.Hidden;
			Assert.AreEqual(false, new GridViewContainerControl(grid).RequireRenderFilterBar);
		}
	}
	[TestFixture]
	public class ASPxGridViewCallBackMethodsTest {
		[Test]
		public void GroupByColumn1() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("GROUP|0||");
			Assert.AreEqual(1, grid.GroupCount);
			Assert.AreEqual(0, grid.DataColumns[0].GroupIndex);
		}
		[Test]
		public void SortByColumn1() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("SORT|0|||");
			Assert.AreEqual(0, grid.DataColumns[0].SortIndex);
			Assert.AreEqual(ColumnSortOrder.Ascending, grid.DataColumns[0].SortOrder);
		}
		[Test]
		public void MultipleColumnFilterApply() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("APPLYCOLUMNFILTER|1|str");
			grid = grid.CreateGridFromStream();
			Assert.AreEqual("str", grid.FilterHelper.GetColumnAutoFilterText(grid.DataColumns[1]));
			grid = grid.CreateGridAndPerformCallBack("APPLYCOLUMNFILTER|0|1");
			grid = grid.CreateGridFromStream();
			Assert.AreEqual("str", grid.FilterHelper.GetColumnAutoFilterText(grid.DataColumns[1]));
			Assert.AreEqual("1", grid.FilterHelper.GetColumnAutoFilterText(grid.DataColumns[0]));
		}
	}
	[TestFixture]
	public class ASPxGridViewEditTests {
		[Test]
		public void IsGridEditingCallBackTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Assert.IsFalse(grid.IsEditing);
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			Assert.IsTrue(grid.IsEditing);
		}
		[Test]
		public void IsGridEditingPostBackTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Assert.IsFalse(grid.IsEditing);
			grid = grid.CreateGridAndPerformPostBack("STARTEDIT|1");
			Assert.IsTrue(grid.IsEditing);
		}
		[Test]
		public void StoreKeyFieldTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Columns.RemoveAt(0);
			grid.DataBind();
			grid = grid.CreateGridFromStream();
			Assert.IsFalse(grid.IsEditing);
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			Assert.IsTrue(grid.IsEditing);
		}
		[Test]
		public void IsGridEditingCallBackTestFromCache() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid = grid.CreateGridFromStream();
			Assert.IsFalse(grid.IsEditing);
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			Assert.IsTrue(grid.IsEditing);
		}
		[Test]
		public void IsGridCancelEditingCallBackTestFromCache() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid = grid.CreateGridFromStream();
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			grid = grid.CreateGridFromStream();
			grid = grid.CreateGridAndPerformCallBack("CANCELEDIT");
			Assert.AreEqual(false, grid.IsEditing);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(false, grid.IsEditing);
		}
		[Test]
		public void ChangeEditMode() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(GridViewEditingMode.Inline, grid.SettingsEditing.Mode);
		}
		[Test]
		public void ValidationTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			grid.RowValidating += new ASPxDataValidationEventHandler(grid_RowValidating);
			grid.UpdateEdit();
			Assert.AreEqual(true, grid.IsEditing);
			Assert.AreEqual(1, grid.RenderHelper.ValidationError.Count);
		}
		[Test]
		public void EditViaPostback() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.StartEdit(0);
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|2|");
			grid.FindControl("xxx");
			grid.RenderHelper.EditorList[0].Value = 100;
			grid.UpdateEdit();
			Assert.AreEqual(100, grid.GetRowValues(0, "column1"));
		}
		void grid_RowValidating(object sender, ASPxDataValidationEventArgs e) {
			e.Errors[(sender as ASPxGridView).Columns[1]] = "Test";
		}
		[Test]
		public void UpdateOnFilterGridWithFilterModeDispayByText_Bug_Q100520() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			(grid.Columns[2] as GridViewDataColumn).Settings.FilterMode = ColumnFilterMode.DisplayText;
			grid.DataTable.Rows[0]["column2"] = "testedfilterstr";
			grid.FilterExpression = "column2='testedfilterstr'";
			grid.Settings.ShowFooter = false;
			grid.Settings.ShowColumnHeaders = false;
			grid.DataBind();
			Table table =  grid.FindControl(grid.RenderHelper.MainTableID) as Table;
			Assert.AreEqual(1, table.Rows.Count);
		}
		[Test]
		public void MultipleKeys() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.KeyFieldName = "column1;column2";
			grid.DataBind();
			grid = grid.CreateGridAndPerformCallBack("STARTEDIT|1");
			Assert.IsTrue(grid.IsEditing);
		}
	}
	[TestFixture]
	public class ASPxGridViewCommandColumnTests {
		[Test]
		public void CallBackRestoreTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind(); 
			int recordCount = grid.DataProxy.VisibleRowCount;
			grid = grid.CreateGridAndPerformCallBack("SORT|4|||");
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(recordCount, grid.DataProxy.VisibleRowCount);
		}
		[Test]
		public void CustomButtonVisibility() {
			GridViewCommandColumnCustomButton button = new GridViewCommandColumnCustomButton();
			Hashtable values = new Hashtable();
			values[GridViewCustomButtonVisibility.AllDataRows] = new bool[] { true, true, false, false };
			values[GridViewCustomButtonVisibility.BrowsableRow] = new bool[] { false, true, false, false };
			values[GridViewCustomButtonVisibility.EditableRow] = new bool[] { true, false, false, false };
			values[GridViewCustomButtonVisibility.FilterRow] = new bool[] { false, false, true, true };
			values[GridViewCustomButtonVisibility.Invisible] = new bool[] { false, false, false, false };
			foreach(GridViewCustomButtonVisibility visibility in Enum.GetValues(typeof(GridViewCustomButtonVisibility))) {
				button.Visibility = visibility;
				bool[] res = (bool[])values[visibility];
				Assert.AreEqual(res[0], button.IsVisible(GridViewTableCommandCellType.Data, true));
				Assert.AreEqual(res[1], button.IsVisible(GridViewTableCommandCellType.Data, false));
				Assert.AreEqual(res[2], button.IsVisible(GridViewTableCommandCellType.Filter, true));
				Assert.AreEqual(res[3], button.IsVisible(GridViewTableCommandCellType.Filter, false));
			}
		}
	}
	[TestFixture]
	public class ASPxGridViewSelectionTests {
		[Test]
		public void RestoreSelectionFromCache() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(1);
			grid.Selection.SelectRow(3);
			Assert.AreEqual(2, grid.Selection.Count);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(2, grid.Selection.Count);
		}
		[Test]
		public void SelectFromCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			Assert.AreEqual(0, grid.Selection.Count);
			grid = grid.CreateGridAndApplyAddonParams("SR", "FFTT");
			Assert.AreEqual(2, grid.Selection.Count);
		}
		[Test]
		public void UnSelectAllFromCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(1);
			grid.Selection.SelectRow(3);
			Assert.AreEqual(2, grid.Selection.Count);
			grid = grid.CreateGridAndApplyAddonParams("SR", "U");
			Assert.AreEqual(0, grid.Selection.Count);
		}
		[Test]
		public void SelectionChangedTests() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			Assert.AreEqual(0, grid.SelectionChangedCounter);
			grid.Selection.SelectRow(1);
			Assert.AreEqual(1, grid.SelectionChangedCounter);
			grid.Selection.SelectRow(1);
			Assert.AreEqual(1, grid.SelectionChangedCounter);
			grid.Selection.UnselectRow(0);
			Assert.AreEqual(1, grid.SelectionChangedCounter);
			grid.Selection.UnselectRow(1);
			Assert.AreEqual(2, grid.SelectionChangedCounter);
			grid.Selection.UnselectAll();
			Assert.AreEqual(2, grid.SelectionChangedCounter);
			grid.Selection.SelectAll();
			Assert.AreEqual(3, grid.SelectionChangedCounter);
			grid.Selection.SelectAll();
			Assert.AreEqual(3, grid.SelectionChangedCounter);
			grid.Selection.UnselectAll();
			Assert.AreEqual(4, grid.SelectionChangedCounter);
		}
		[Test]
		public void SelectionChangedFromCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			Assert.AreEqual(0, grid.SelectionChangedCounter);
			grid = grid.CreateGridAndApplyAddonParams("SR", "FFTT");
			Assert.AreEqual(1, grid.SelectionChangedCounter);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(0, grid.SelectionChangedCounter);
		}
		[Test]
		public void SelectionChangedFromNonSelectionCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(0);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(1, grid.Selection.Count);
			Assert.AreEqual(0, grid.SelectionChangedCounter);
		}
		[Test]
		public void GetSelectedFieldValuesSimpleTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(0);
			grid.Selection.SelectRow(10);
			List<object> sel = grid.GetSelectedFieldValues("column1");
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column1"), sel[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column1"), sel[1]);
		}
		[Test]
		public void GetSelectedFieldValuesUseCachedDataProvider() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(1);
			grid.Selection.SelectRow(9);
			List<object> sel1 = grid.GetSelectedFieldValues("column1");
			grid = grid.CreateGridFromStream();
			List<object> sel2 = grid.GetSelectedFieldValues("column1");
			Assert.AreEqual(sel1.Count, sel2.Count);
			Assert.AreEqual(sel1[0], sel2[0]);
			Assert.AreEqual(sel1[1], sel2[1]);
			Assert.AreEqual(2, grid.Selection.Count);
			Assert.AreEqual(0, grid.DataProxy.GetSelectedRowCountWithoutCurrentPage());
			Assert.AreEqual(2, grid.DataProxy.GetSelectedRowCountOnPage());
			Assert.AreEqual(true, grid.IsCachedDataProvider);
		}
		[Test]
		public void GetSelectedKeyFieldValuesUseCachedDataProvider() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(1);
			grid.Selection.SelectRow(20);
			List<object> sel1 = grid.GetSelectedFieldValues(grid.KeyFieldName);
			grid = grid.CreateGridFromStream();
			List<object> sel2 = grid.GetSelectedFieldValues(grid.KeyFieldName);
			Assert.AreEqual(sel1.Count, sel2.Count);
			Assert.AreEqual(sel1[0], sel2[0]);
			Assert.AreEqual(sel1[1], sel2[1]);
			Assert.AreEqual(true, grid.IsCachedDataProvider);
		}
		[Test]
		public void GetSelectedAllKeyFieldValuesUseCachedDataProvider() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectAll();
			List<object> sel1 = grid.GetSelectedFieldValues(grid.KeyFieldName);
			Assert.AreEqual(grid.DataProxy.VisibleRowCount, sel1.Count);
			grid = grid.CreateGridFromStream();
			List<object> sel2 = grid.GetSelectedFieldValues(grid.KeyFieldName);
			Assert.AreEqual(sel1.Count, sel2.Count);
			for(int i = 0; i < sel1.Count; i++) {
				Assert.AreEqual(sel1[i], sel2[i]);
			}
			Assert.AreEqual(false, grid.IsCachedDataProvider);
		}
		[Test]
		public void GetSelectedFieldsValuesSimpleTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(0);
			grid.Selection.SelectRow(10);
			List<object> sel = grid.GetSelectedFieldValues("column1", "column2");
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column1"), ((object[])sel[0])[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column1"), ((object[])sel[1])[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column2"), ((object[])sel[0])[1]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column2"), ((object[])sel[1])[1]);
		}
		[Test]
		public void GetFBCallBackSelectedFieldsValuesSimpleTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(0);
			grid.Selection.SelectRow(10);
			List<object> sel = (List<object>)grid.InternalCallBacks["SELFIELDVALUES"](new string[] { "column1" });
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column1"), sel[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column1"), sel[1]);
			sel = (List<object>)grid.InternalCallBacks["SELFIELDVALUES"](new string[] { "column1;column2" });
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column1"), ((object[])sel[0])[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column1"), ((object[])sel[1])[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(0, "column2"), ((object[])sel[0])[1]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(10, "column2"), ((object[])sel[1])[1]);
		}
		[Test]
		public void GetSelectedFieldValuesTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.Selection.SelectRow(0);
			grid.Selection.SelectRow(10);
			List<object> sel = grid.GetSelectedFieldValues("id");
			Assert.AreEqual(2, sel.Count);
			Assert.AreEqual(1, sel[0]);
			Assert.AreEqual(11, sel[1]);
		}
		ASPxGridViewTester CreateFilteredGrid(bool applyFilter) {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			if(applyFilter) {
				grid.FilterExpression = "column2='str2'";
			}
			return grid;
		}
		[Test]
		public void SelectAllInFilteredGridShouldSelectAllByRecords() {
			ASPxGridViewTester grid = CreateFilteredGrid(true);
			grid.Selection.SelectAll();
			bool isStoreSelected = ((grid.DataProxy as GridWebProxyTester).Selection as WebTestDataSelection).TestIsStoreSelectected;
			Assert.AreEqual(true, isStoreSelected);
		}
		[Test]
		public void SelectionAfterFilter_Bug_B93448_1() {
			ASPxGridViewTester grid = CreateFilteredGrid(true);
			Assert.AreEqual(12, grid.DataProxy.VisibleRowCount);
			object firstKey = grid.GetRowValues(0, "id");
			grid.Selection.SelectAll();
			grid = grid.CreateGridFromStream();
			List<object> sel = grid.GetSelectedFieldValues("id");
			Assert.AreEqual(firstKey, sel[0]);
		}
		[Test]
		public void SelectionAfterFilter_Bug_B93448_2() {
			ASPxGridViewTester grid = CreateFilteredGrid(false);
			grid.Selection.SelectAll();
			object firstKey = grid.GetRowValues(0, "id");
			grid.FilterExpression = "column2='str2'";
			grid = grid.CreateGridFromStream();
			List<object> sel = grid.GetSelectedFieldValues("id");
			Assert.AreEqual(firstKey, sel[0]);
		}
		[Test]
		public void SelectionAfterFilter_Bug_B93448_3() {
			ASPxGridViewTester grid = CreateFilteredGrid(true);
			Assert.AreEqual(12, grid.DataProxy.VisibleRowCount);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(12, grid.DataProxy.VisibleRowCount);
			object firstKey = grid.GetRowValues(0, "id");
			Assert.AreEqual(true, grid.DataProxy.IsFiltered);
			grid.Selection.SelectAll();
			Assert.AreEqual(12, grid.Selection.Count);
			grid = grid.CreateGridFromStream();
			List<object> sel = grid.GetSelectedFieldValues("id");
			Assert.AreEqual(firstKey, sel[0]);
		}
		[Test]
		public void SelectedAfterFilter_Bug_B30720() {
			ASPxGridViewTester grid = CreateFilteredGrid(true);
			int visibleCount = grid.DataProxy.VisibleRowCount;
			grid = grid.CreateGridFromStream();
			grid.Selection.SelectAll();
			Assert.AreEqual(true, grid.DataProxy.IsBound);
		}
	}
	[TestFixture]
	public class ASPxGridViewTotalSummaryTests {
		[Test]
		public void SimpleSummary() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.TotalSummary.Add(SummaryItemType.Count, "");
			Assert.AreEqual(grid.DataTable.Rows.Count, grid.GetTotalSummaryValue(grid.TotalSummary[0]));
		}
		[Test]
		public void SimpleSummaryAndCachedGrid() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.TotalSummary.Add(SummaryItemType.Count, "");
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(1, grid.TotalSummary.Count);
			Assert.AreEqual(grid.DataTable.Rows.Count, grid.GetTotalSummaryValue(grid.TotalSummary[0]));
		}
		[Test]
		public void SimpleCustomSummary() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.CustomSummaryCalculate += new CustomSummaryEventHandler(grid_CustomSummaryCalculate);
			grid.TotalSummary.Add(SummaryItemType.Custom, "");
			Assert.AreEqual(1000, grid.GetTotalSummaryValue(grid.TotalSummary[0]));
		}
		void grid_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e) {
			e.TotalValue = 1000;
		}
	}
	[TestFixture]
	public class ASPxGridViewDetailTests {
		[Test]
		public void AllowOnlyOneRow() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsDetail.ShowDetailRow = true;
			grid.DetailRows.ExpandRow(0);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			grid.DetailRows.ExpandRow(1);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(1));
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			grid.DetailRows.ExpandAllRows();
			Assert.AreEqual(grid.VisibleRowCount, grid.DetailRows.VisibleCount);
			grid.SettingsDetail.AllowOnlyOneMasterRowExpanded = true;
			grid.DetailRows.CollapseAllRows();
			grid.DetailRows.ExpandRow(0);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			grid.DetailRows.ExpandRow(1);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(1));
			Assert.AreEqual(false, grid.DetailRows.IsVisible(0));
			Assert.AreEqual(1, grid.DetailRows.VisibleCount);
			grid.DetailRows.ExpandAllRows();
			Assert.AreEqual(1, grid.DetailRows.VisibleCount);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
		}
		[Test]
		public void ShowDetailAndEditRows_Bug_B30824() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Table table =  grid.FindControl(grid.RenderHelper.MainTableID) as Table;
			int rowCount = table.Rows.Count;
			grid.SettingsDetail.ShowDetailRow = true;
			grid.DetailRows.ExpandRow(0);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			grid.StartEdit(0);
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			grid.BuildHtml();
			Assert.AreEqual(true, grid.DetailRows.IsVisible(0));
			table = grid.FindControl(grid.RenderHelper.MainTableID) as Table;
			Assert.AreEqual(rowCount + 2, table.Rows.Count);
		}
		[Test]
		public void DetailRowExpandedChanged() {
			ASPxGridViewTester grid = new ASPxGridViewTester();			
			grid.DataBind();
			grid.SettingsDetail.ShowDetailRow = true;
			for(int i = 0; i < 2; i++)
				grid = grid.CreateGridAndPerformCallBack("SHOWDETAILROW|0");
			for(int i = 0; i < 2; i++)
				grid = grid.CreateGridAndPerformCallBack("HIDEDETAILROW|0");
			Assert.AreEqual("0,True 0,False ", grid.DetailRowExpandedChangedLog);
		}
	}
	[TestFixture]
	public class ASPxGridViewGroupSummaryTests {
		public static ASPxGridViewTester CreateGroupFooterGrid() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Settings.ShowGroupFooter = GridViewGroupFooterMode.VisibleAlways;
			grid.GroupBy((GridViewDataColumn)grid.Columns["column1"]);
			return grid;
		}
		[Test]
		public void SimpleSummary() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.GroupSummary.Add(SummaryItemType.Count, "");
			grid.GroupBy((GridViewDataColumn)grid.Columns["column1"]);
			int n = grid.DataProxy.GetChildDataRowCount(0);
			Assert.AreEqual(n, grid.GetGroupSummaryValue(0, grid.GroupSummary[0]));
		}
		[Test]
		public void SimpleSummaryAndCachedGrid() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.GroupSummary.Add(SummaryItemType.Count, "");
			grid.GroupBy((GridViewDataColumn)grid.Columns["column1"]);
			grid = grid.CreateGridFromStream();
			int n = grid.DataProxy.GetChildDataRowCount(0);
			Assert.AreEqual(n, grid.GetGroupSummaryValue(0, grid.GroupSummary[0]));
		}
		[Test]
		public void GroupFooterShowingGroupRowDifferentModeTest() {
			ASPxGridViewTester grid = CreateGroupFooterGrid();
			Assert.AreEqual(0, grid.RenderHelper.GetGroupFooterVisibleIndexes(0)[0]);
			Assert.AreEqual(1, grid.RenderHelper.GetGroupFooterVisibleIndexes(1)[0]);
			grid.Settings.ShowGroupFooter = GridViewGroupFooterMode.VisibleIfExpanded;
			Assert.AreEqual(null, grid.RenderHelper.GetGroupFooterVisibleIndexes(1));
			grid.Settings.ShowGroupFooter = GridViewGroupFooterMode.VisibleAlways;
			Assert.AreEqual(1, grid.RenderHelper.GetGroupFooterVisibleIndexes(1)[0]);
			grid.DataProxy.ExpandRow(1, false);
			Assert.AreEqual(null, grid.RenderHelper.GetGroupFooterVisibleIndexes(1));
		}
		[Test]
		public void GroupFooterShowingForDataRow() {
			ASPxGridViewTester grid = CreateGroupFooterGrid();
			grid.DataProxy.ExpandRow(0, false);
			int childCount = grid.DataProxy.GetChildDataRowCount(0);
			Assert.AreEqual(null, grid.RenderHelper.GetGroupFooterVisibleIndexes(0));
			Assert.AreEqual(null, grid.RenderHelper.GetGroupFooterVisibleIndexes(1));
			Assert.AreEqual(0, grid.RenderHelper.GetGroupFooterVisibleIndexes(childCount)[0]);
		}
		[Test]
		public void ShowInFooterColumnTest() {
			ASPxGridViewTester grid = CreateGroupFooterGrid();
			grid.GroupSummary.Add(SummaryItemType.Count, "");
			Assert.AreEqual(1, grid.GroupSummary.GetGroupRowItems().Count);
			grid.GroupSummary[0].ShowInGroupFooterColumn = (grid.Columns[0] as GridViewDataColumn).FieldName;
			Assert.AreEqual(0, grid.GroupSummary.GetGroupRowItems().Count);
		}
		[Test]
		public void AssignCaptionToShowInGroupFooterColumn() {
			ASPxGridViewTester grid = CreateGroupFooterGrid();
			grid.Columns[0].Caption = "My Caption";
			grid.GroupSummary.Add(SummaryItemType.Count, "");
			grid.GroupSummary[0].ShowInGroupFooterColumn = (grid.Columns[0] as GridViewDataColumn).FieldName;
			ASPxGridViewTextBuilder text = new ASPxGridViewTextBuilder(grid);
			string res = text.GetGroupRowFooterText(grid.Columns[0] as GridViewDataColumn, 0, "");
			grid.GroupSummary[0].ShowInGroupFooterColumn = grid.Columns[0].Caption;
			Assert.AreEqual(res, text.GetGroupRowFooterText(grid.Columns[0] as GridViewDataColumn, 0, ""));
		}
		[Test]
		public void AssignCaptionToShowInGroupFooterColumnTest2() {
			ASPxGridViewTester grid = CreateGroupFooterGrid();
			grid.DataTable.Columns[1].ColumnName = "TestColumn";
			(grid.Columns[1] as GridViewDataColumn).FieldName = "TestColumn";
			(grid.Columns[1] as GridViewDataColumn).Caption = string.Empty;
			grid.DataBind();
			grid.GroupSummary.Add(SummaryItemType.Count, "");
			grid.GroupSummary[0].ShowInGroupFooterColumn = (grid.Columns[1] as GridViewDataColumn).FieldName;
			ASPxGridViewTextBuilder text = new ASPxGridViewTextBuilder(grid);
			string res = text.GetGroupRowFooterText(grid.Columns[1] as GridViewDataColumn, 0, "");
			grid.GroupSummary[0].ShowInGroupFooterColumn = grid.Columns[1].ToString();
			Assert.AreEqual(res, text.GetGroupRowFooterText(grid.Columns[1] as GridViewDataColumn, 0, ""));
		}
		[Test]
		public void MultiLevelsBugB90606() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Columns.Clear();
			DataTable table = new DataTable();
			table.Columns.Add("Level1");
			table.Columns.Add("Level2");
			table.Columns.Add("Level3");
			table.Columns.Add("NumberColumn");
			table.Rows.Add(new object[] { 1, 1, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 1, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 1, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 2, 5.0 });
			table.Rows.Add(new object[] { 1, 2, 2, 5.0 });
			table.Rows.Add(new object[] { 2, 2, 2, 5.0 });
			grid.DataSource = table;
			grid.Columns.Add(new GridViewDataColumn("Level1"));
			grid.Columns.Add(new GridViewDataColumn("Level2"));
			grid.Columns.Add(new GridViewDataColumn("Level3"));
			grid.Columns.Add(new GridViewDataColumn("NumberColumn"));
			for(int i = 0; i < 3; i++) {
				grid.DataColumns[i].GroupBy();
			}
			grid.Settings.ShowGroupFooter = GridViewGroupFooterMode.VisibleAlways;
			grid.DataBind();
			Assert.AreEqual(1, grid.RenderHelper.GetGroupFooterVisibleIndexes(0).Count);
			Assert.AreEqual(0, grid.RenderHelper.GetGroupFooterVisibleIndexes(0)[0]);
			grid.ExpandRow(0);
			Assert.AreEqual(true, grid.DataProxy.IsLastRowInCurrentLevel(2));
			Assert.AreEqual(2, grid.RenderHelper.GetGroupFooterVisibleIndexes(2).Count);
			Assert.AreEqual(2, grid.RenderHelper.GetGroupFooterVisibleIndexes(2)[0]);
			Assert.AreEqual(0, grid.RenderHelper.GetGroupFooterVisibleIndexes(2)[1]);
			grid.ExpandRow(2);
			Assert.AreEqual(true, grid.DataProxy.IsLastRowInCurrentLevel(4));
			Assert.AreEqual(true, grid.DataProxy.IsLastRowInCurrentLevel(2));
			Assert.AreEqual(3, grid.RenderHelper.GetGroupFooterVisibleIndexes(4).Count);
			Assert.AreEqual(4, grid.RenderHelper.GetGroupFooterVisibleIndexes(4)[0]);
			Assert.AreEqual(2, grid.RenderHelper.GetGroupFooterVisibleIndexes(4)[1]);
			Assert.AreEqual(0, grid.RenderHelper.GetGroupFooterVisibleIndexes(4)[2]);
		}
	}
	[TestFixture]
	public class ASPxGridViewUnboundColumnsTests {
		[Test]
		public void SimpleTest1() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.CustomUnboundColumnData += new ASPxGridViewColumnDataEventHandler(grid_CustomUnboundColumnData);
			GridViewDataColumn col1 = new GridViewDataColumn();
			col1.UnboundType = UnboundColumnType.Integer;
			col1.FieldName = "UInt";
			grid.Columns.Add(col1);
			Assert.AreEqual(1, grid.DataProxy.GetRowValue(1, "UInt"));
		}
		[Test]
		public void SimpleTest2() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.CustomUnboundColumnData += new ASPxGridViewColumnDataEventHandler(grid_CustomUnboundColumnData);
			GridViewDataColumn col1 = new GridViewDataColumn();
			grid.Columns.Add(col1);
			col1.UnboundType = UnboundColumnType.Integer;
			col1.FieldName = "UInt";
			Assert.AreEqual(1, grid.DataProxy.GetRowValue(1, "UInt"));
		}
		void grid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e) {
			e.Value = e.ListSourceRowIndex;
		}
	}
	[TestFixture]
	public class ASPxGridViewGroupPagingTests {
		[Test]
		public void GroupRowFitOnPage() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsPager.PageSize = 1;
			grid.GroupBy(grid.DataColumns[1]);
			grid.ExpandAll();
			Assert.AreEqual(false, grid.DataProxy.IsGroupRowFitOnPage(0));
			grid.CollapseAll();
			Assert.AreEqual(true, grid.DataProxy.IsGroupRowFitOnPage(0));
			grid.SettingsPager.PageSize = 100;
			grid.ExpandAll();
			Assert.AreEqual(true, grid.DataProxy.IsGroupRowFitOnPage(0));
		}
		[Test]
		public void ContinuedFromPrevPage() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsPager.PageSize = 1;
			grid.GroupBy(grid.DataColumns[1]);
			grid.ExpandAll();
			grid = grid.CreateGridAndPerformCallBack("NEXTPAGE");
			Assert.AreEqual(true, grid.DataProxy.IsContinuedFromPrevPage(1));
			grid.CollapseAll();
			Assert.AreEqual(false, grid.DataProxy.IsContinuedFromPrevPage(1));
		}
	}
	[TestFixture]
	public class ASPxGridViewHtmlTests {
		[Test]
		public void CssMergingTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			AppearanceStyle style1 = new AppearanceStyle(), style2 = new AppearanceStyle(), style3 = new AppearanceStyle();
			style1.CssClass = "class1";
			style2.CssClass = "class2";
			grid.RenderHelper.CreateStyle(style3, null, null, style1, style2);
			Assert.AreEqual("class1 class2", style3.CssClass);
		}
		[Test]
		public void KeyFieldNameShouldNotRequired() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.KeyFieldName = string.Empty;
			grid.BuildHtml();
		}
		[Test]
		public void ShowHasParentRows() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsPager.PageSize = 1;
			grid.SettingsPager.Mode = GridViewPagerMode.ShowPager;
			grid.GroupBy(grid.DataColumns[2]);
			grid.ExpandAll();
			grid.PageIndex = 1;
			Assert.AreEqual(true, grid.DataProxy.HasParentRows);
		}
		[Test]
		public void CalcVisibleGroupLevelCount() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(0, grid.RenderHelper.GroupCount);
			grid.GroupBy(grid.DataColumns[2]);
			Assert.AreEqual(1, grid.RenderHelper.GroupCount);
			grid.ExpandAll();
			Assert.AreEqual(1, grid.RenderHelper.GroupCount);
		}
		[Test]
		public void ShowGroupedColumns() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(1, grid.RenderHelper.VisibleColumns.IndexOf(grid.Columns[1]));
			grid.GroupBy(grid.Columns[1] as GridViewDataColumn);
			Assert.AreEqual(-1, grid.RenderHelper.VisibleColumns.IndexOf(grid.Columns[1]));
			grid.Settings.ShowGroupedColumns = true;
			Assert.AreEqual(1, grid.RenderHelper.VisibleColumns.IndexOf(grid.Columns[1]));
		}
		[Test]
		public void ShowDetailRowColumns() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.BuildHtml();
			Assert.AreEqual(grid.Columns.Count, grid.RenderHelper.ColumnSpanCount);
			grid.SettingsDetail.ShowDetailRow = true;
			grid.BuildHtml();
			Assert.AreEqual(grid.Columns.Count + 1, grid.RenderHelper.ColumnSpanCount);
			grid.Settings.ShowGroupedColumns = true;
			grid.GroupBy(grid.Columns[0]);
			grid.BuildHtml();
			Assert.AreEqual(grid.Columns.Count + 2, grid.RenderHelper.ColumnSpanCount);
		}
		[Test]
		public void ShowDetailButtons() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.BuildHtml();
			Assert.AreEqual(grid.Columns.Count, grid.RenderHelper.ColumnSpanCount);
			grid.SettingsDetail.ShowDetailRow = true;
			grid.BuildHtml();
			Assert.AreEqual(true, grid.RenderHelper.HasDetailButton);
			Assert.AreEqual(grid.Columns.Count + 1, grid.RenderHelper.ColumnSpanCount);
			grid.SettingsDetail.ShowDetailButtons = false;
			grid.BuildHtml();
			Assert.AreEqual(false, grid.RenderHelper.HasDetailButton);
			Assert.AreEqual(grid.Columns.Count, grid.RenderHelper.ColumnSpanCount);
		}
		[Test]
		public void FindAddNewRowButton() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(null, grid.RenderHelper.GetVisibleNewRowButton());
			GridViewCommandColumn column = new GridViewCommandColumn();
			grid.Columns.Add(column);
			Assert.AreEqual(null, grid.RenderHelper.GetVisibleNewRowButton());
			column.NewButton.Visible = true;
			Assert.AreEqual(column.NewButton, grid.RenderHelper.GetVisibleNewRowButton());
		}
		[Test]
		public void EmptyDataRow() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			Assert.AreEqual(false, grid.RenderHelper.HasEmptyDataRow);
			grid.FilterExpression = "column1=-1";
			grid.DataBind();
			Assert.AreEqual(true, grid.RenderHelper.HasEmptyDataRow);
			grid.AddNewRow();
			grid.BuildHtml();
			Assert.AreEqual(false, grid.RenderHelper.HasEmptyDataRow);
			grid.CancelEdit();
			grid.DataBind();
			Assert.AreEqual(true, grid.RenderHelper.HasEmptyDataRow);
			grid.FilterExpression = "";
			Assert.AreEqual(false, grid.RenderHelper.HasEmptyDataRow);
		}
		[Test]
		public void EmptyDataRowForNullDataSource() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			object dataSource = grid.DataSource;
			grid.DataSource = null;
			grid.DataBind();
			Assert.AreEqual(true, grid.RenderHelper.HasEmptyDataRow);
			grid.DataSource = dataSource;
			grid.DataBind();
			Assert.AreEqual(false, grid.RenderHelper.HasEmptyDataRow);
		}
		[Test]
		public void IsRemoveBorderFromMainTableLastRowTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsPager.Mode = GridViewPagerMode.ShowPager;
			grid.SettingsPager.Position = PagerPosition.Top;
			Assert.AreEqual(true, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.SettingsPager.Position = PagerPosition.Bottom;
			Assert.AreEqual(false, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.SettingsPager.Position = PagerPosition.Top;
			grid.Settings.ShowVerticalScrollBar = true;
			Assert.AreEqual(false, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.Settings.ShowVerticalScrollBar = false;
			Assert.AreEqual(true, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.Settings.ShowFooter = true;
			Assert.AreEqual(false, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.Settings.ShowFooter = false;
			Assert.AreEqual(true, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.AddNewRow();
			grid.SettingsPager.Position = PagerPosition.Top;
			Assert.AreEqual(true, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			grid.SettingsEditing.NewItemRowPosition = GridViewNewItemRowPosition.Bottom;
			Assert.AreEqual(false, grid.RenderHelper.IsRemoveBorderFromMainTableLastRow);
			Assert.AreEqual(true, grid.RenderHelper.IsRemoveBorderFromMainTableLastNewItemRow);
		}
		[Test]
		public void HtmlEditFormCreated() {
			const string marker = "__DX_EDIT_FORM__";
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.HtmlEditFormCreated += delegate(object s, ASPxGridViewEditFormEventArgs e) {
				e.EditForm.Controls.Add(new LiteralControl(marker));
			};
			grid.DataBind();
			grid.StartEdit(0);
			GridViewEditingMode[] modes = new GridViewEditingMode[] { GridViewEditingMode.EditForm, GridViewEditingMode.EditFormAndDisplayRow, GridViewEditingMode.PopupEditForm };
			foreach(GridViewEditingMode mode in modes) {
				grid.SettingsEditing.Mode = mode;
				Assert.IsTrue(RenderUtils.GetRenderResult(grid).Contains(marker));
			}
		}
		[Test]
		public void EmptyPagerDataRowCount() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			const int pageSize = 13;
			grid.SettingsPager.PageSize = pageSize;
			grid.DataBind();
			Assert.AreEqual(0, grid.RenderHelper.EmptyPagerDataRowCount);
			int rowsCount = grid.DataTable.Rows.Count;
			grid.PageIndex = rowsCount / pageSize;
			Assert.AreEqual(0, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.SettingsPager.ShowEmptyDataRows = true;
			Assert.AreEqual(pageSize - grid.DataProxy.VisibleRowCountOnPage, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.AddNewRow();
			Assert.AreEqual(pageSize - grid.DataProxy.VisibleRowCountOnPage - 1, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.CancelEdit();
			Assert.AreEqual(pageSize - grid.DataProxy.VisibleRowCountOnPage, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.StartEdit(grid.DataProxy.VisibleStartIndex);
			Assert.AreEqual(pageSize - grid.DataProxy.VisibleRowCountOnPage, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.CancelEdit();
			grid.DataSource = null;
			grid.DataBind();
			grid.SettingsPager.ShowEmptyDataRows = false;
			Assert.AreEqual(0, grid.RenderHelper.EmptyPagerDataRowCount);
			grid.SettingsPager.ShowEmptyDataRows = true;
			Assert.AreEqual(pageSize - 1, grid.RenderHelper.EmptyPagerDataRowCount);
		}
		[Test]
		public void ExactRowCountOnPage() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsPager.PageSize = 13;
			grid.DataBind();
			GridViewHtmlTable table = grid.CreateMainTable(GridViewHtmlTableRenderPart.Content);
			Assert.AreEqual(13, table.Rows.Count);
			int rowsCount = grid.DataTable.Rows.Count;
			grid.PageIndex = rowsCount / 13;
			table = grid.CreateMainTable(GridViewHtmlTableRenderPart.Content);
			Assert.AreEqual(rowsCount - (rowsCount / 13) * 13, table.Rows.Count);
			grid.SettingsPager.ShowEmptyDataRows = true;
			table = grid.CreateMainTable(GridViewHtmlTableRenderPart.Content);
			Assert.AreEqual(13, table.Rows.Count);
		}
	}
	[TestFixture]
	public class ASPxGridViewImagePathTests {
		[Test]
		public void ImagePathTest() {
			GridViewImages images = new GridViewImages(null);
			images.CollapsedButton.Width = Unit.Pixel(20);
			Assert.AreEqual(Unit.Pixel(20), images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Width);
			Assert.AreEqual(string.Empty, images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Url);
			images.ImageFolder = "~/myFolder/";
			Assert.AreEqual(Unit.Pixel(20), images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Width);
			Assert.AreEqual("~/myFolder/gvCollapsedButton.png", images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Url);
			images.ImageFolder = "myFolder";
			Assert.AreEqual("myFolder/", images.ImageFolder);
			images.CollapsedButton.Url = "myUrl";
			Assert.AreEqual(Unit.Pixel(20), images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Width);
			Assert.AreEqual("myUrl", images.GetImageProperties(null, GridViewImages.CollapsedButtonName).Url);
		}
	}
	[TestFixture]
	public class ASPxGridViewViewStateManagedTests : TestClassBase {
		[Test]
		public void Test() {
			CheckSettings(new ASPxGridView());
			CheckStyles(new ASPxGridView());
		}
		static void CheckStyles(object instance) { CheckObject(instance, @"Styles\S+"); }
		static void CheckSettings(object instance) { CheckObject(instance, @"Settings\S*"); }
		static void CheckObject(object instance, string mask) {
			PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(instance);
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			Regex reg = new Regex(mask);
			MethodInfo miGetStateManagedObjects = instance.GetType().GetMethod("GetStateManagedObjects", BindingFlags.NonPublic | BindingFlags.Instance);
			IStateManager[] objects = miGetStateManagedObjects.Invoke(instance, null) as IStateManager[];
			foreach(PropertyDescriptor pd in pds) {
				if(!reg.IsMatch(pd.Name)) continue;
				object value = pd.GetValue(instance);
				if(Array.IndexOf(objects, value) != -1) continue;
				Assert.AreEqual(string.Format("{0}.{1} doesnt added to StateManagedObjects", pd.ComponentType.Name, pd.Name), false);
			}
		}
	}
	[TestFixture]
	public class GridViewAttributesTests : TestClassBase {
		[Test]
		public void NotifyParentTestMethod() {
			CheckControlPropertiesNotifyParent(typeof(ASPxGridView));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewCommandColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataTextColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataMemoColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataHyperLinkColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataCheckColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataDateColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataSpinEditColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataComboBoxColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataImageColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataBinaryImageColumn));
			CheckObjectNotifyParentForRootProperties(typeof(GridViewDataProgressBarColumn));
			CheckObjectNotifyParentForRootProperties(typeof(ASPxSummaryItem));
		}
		[Test]
		public void DesignerSerializationVisibilityContentTestMethod() {
			CheckDesignerSerializationVisibilityContent(typeof(ASPxGridView));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewCommandColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataTextColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataMemoColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataHyperLinkColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataCheckColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataDateColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataSpinEditColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataComboBoxColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataImageColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataBinaryImageColumn));
			CheckDesignerSerializationVisibilityContent(typeof(GridViewDataProgressBarColumn));
			CheckDesignerSerializationVisibilityContent(typeof(ASPxSummaryItem));
		}
		[Test]
		public void LocalizableTestMethod() {
			CheckLocalizableAttribute(typeof(ASPxGridView));
			CheckLocalizableAttribute(typeof(GridViewColumn));
			CheckLocalizableAttribute(typeof(GridViewDataColumn));
			CheckLocalizableAttribute(typeof(GridViewCommandColumn));
			CheckLocalizableAttribute(typeof(GridViewDataTextColumn));
			CheckLocalizableAttribute(typeof(GridViewDataMemoColumn));
			CheckLocalizableAttribute(typeof(GridViewDataHyperLinkColumn));
			CheckLocalizableAttribute(typeof(GridViewDataCheckColumn));
			CheckLocalizableAttribute(typeof(GridViewDataDateColumn));
			CheckLocalizableAttribute(typeof(GridViewDataSpinEditColumn));
			CheckLocalizableAttribute(typeof(GridViewDataComboBoxColumn));
			CheckLocalizableAttribute(typeof(GridViewDataImageColumn));
			CheckLocalizableAttribute(typeof(GridViewDataBinaryImageColumn));
			CheckLocalizableAttribute(typeof(GridViewDataProgressBarColumn));
			CheckLocalizableAttribute(typeof(ASPxSummaryItem));
		}
		[Test]
		public void AutoFormatAttributesTestMethod() {
			CheckAutoFormatAttributes(typeof(ASPxGridView));
		}
	}
	[TestFixture]
	public class GridViewFuncCallBackTests {
		[Test]
		public void DoFuncCallbackTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.IsTrue(grid.GetFuncCallBackResult("SELFIELDVALUE", "col1").IndexOf("FB|0|") > -1);
		}
		[Test]
		public void GetRowValuesSimpleTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.GetRowValues(1, "column1");
			Assert.AreEqual(grid.DataProxy.GetRowValue(1, "column1"), value);
		}
		[Test]
		public void GetRowValuesOnGroupRow() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.GetRowValues(0, "column1");
			grid.GroupBy(grid.Columns["column1"]);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(value, grid.DataProxy.GetRowValue(0, "column1"));
		}
		[Test]
		public void GetRowValuesArrayTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object[] values = (object[])grid.GetRowValues(1, new string[] { "column1", "column2" });
			Assert.AreEqual(grid.DataProxy.GetRowValue(1, "column1"), values[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(1, "column2"), values[1]);
		}
		[Test]
		public void GetRowValuesSimpleCachedTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.GetRowValues(5, "column1");
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(value, grid.GetRowValues(5, "column1"));
			Assert.AreEqual(true, grid.IsCachedDataProvider);
			value = grid.GetRowValues(15, "column1");
			Assert.AreEqual(false, grid.IsCachedDataProvider);
		}
		[Test]
		public void GetRowValuesSimpleTestFromCallback() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.InternalCallBacks["ROWVALUES"](new string[] { "5", "column1" });
			Assert.AreEqual(grid.DataProxy.GetRowValue(5, "column1"), value);
			object[] values = (object[])grid.InternalCallBacks["ROWVALUES"](new string[] { "6", "column1;column2" });
			Assert.AreEqual(grid.DataProxy.GetRowValue(6, "column1"), values[0]);
			Assert.AreEqual(grid.DataProxy.GetRowValue(6, "column2"), values[1]);
		}
		[Test]
		public void GetRowValuesByKeyValueSimpleTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			object value = grid.GetRowValuesByKeyValue(2, "column1");
			Assert.AreEqual(value, grid.DataProxy.GetRowValue(1, "column1"));
			value = grid.GetRowValuesByKeyValue(21, "column1");
			Assert.AreEqual(value, grid.DataProxy.GetRowValue(20, "column1"));
		}
		[Test]
		public void GetRowValuesByKeyFromCallbackTest() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|2|true|");
			object value = grid.GetRowValuesByKeyValue(21, "column1");
			Assert.AreEqual(value, grid.DataProxy.GetRowValue(20, "column1"));
		}
	}
	[TestFixture]
	public class GridViewTemplates {
		[Test]
		public void TestGlobalHeaderTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.Header = button;
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[0], "btn").ID);
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[1], "btn").ID);
		}
		[Test]
		public void TestGlobalHeaderCaptionTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.HeaderCaption = button;
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[0], "btn").ID);
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[1], "btn").ID);
		}
		[Test]
		public void TestColumnHeaderTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Columns[0].HeaderTemplate = button;
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindHeaderTemplateControl(grid.Columns[1], "btn"));
		}
		[Test]
		public void TestColumnHeaderCaptionTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Columns[0].HeaderCaptionTemplate = button;
			Assert.AreEqual("btn", grid.FindHeaderTemplateControl(grid.Columns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindHeaderTemplateControl(grid.Columns[1], "btn"));
		}
		[Test]
		public void TestColumnRowDataItemTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.DataColumns[0].DataItemTemplate = button;
			Assert.AreEqual("btn", grid.FindRowCellTemplateControl(0, grid.DataColumns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindRowCellTemplateControl(0, grid.DataColumns[1], "btn"));
			Assert.AreEqual("btn", grid.FindRowCellTemplateControlByKey(grid.DataProxy.GetRowKeyValue(0), grid.DataColumns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindRowCellTemplateControlByKey(grid.DataProxy.GetRowKeyValue(0), grid.DataColumns[1], "btn"));
		}
		[Test]
		public void TestColumnRowEditItemTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.DataColumns[0].EditItemTemplate = button;
			grid.StartEdit(0);
			Assert.AreEqual("btn", grid.FindEditRowCellTemplateControl(grid.DataColumns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindEditRowCellTemplateControl(grid.DataColumns[1], "btn"));
		}
		[Test]
		public void TestColumnRowEditItemTemplateForPupupFormB19277() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm;
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.DataColumns[0].EditItemTemplate = button;
			grid.StartEdit(0);
			Assert.AreEqual("btn", grid.FindEditRowCellTemplateControl(grid.DataColumns[0], "btn").ID);
			Assert.AreEqual(null, grid.FindEditRowCellTemplateControl(grid.DataColumns[1], "btn"));
		}
		[Test]
		public void TestRowDataItemTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.DataItem = button;
			Assert.AreEqual("btn", grid.FindRowCellTemplateControl(0, grid.DataColumns[0], "btn").ID);
			Assert.AreEqual("btn", grid.FindRowCellTemplateControl(1, grid.DataColumns[1], "btn").ID);
			Assert.AreEqual("btn", grid.FindRowCellTemplateControlByKey(grid.DataProxy.GetRowKeyValue(0), grid.DataColumns[0], "btn").ID);
			Assert.AreEqual("btn", grid.FindRowCellTemplateControlByKey(grid.DataProxy.GetRowKeyValue(1), grid.DataColumns[1], "btn").ID);
			Assert.AreEqual("btn", grid.FindRowCellTemplateControl(0, null, "btn").ID);
		}
		[Test]
		public void TestGroupRowContentTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.GroupBy(grid.DataColumns[0]);
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.GroupRowContent = button;
			Assert.AreEqual(null, grid.FindRowCellTemplateControl(0, grid.DataColumns[0], "btn"));
			Assert.AreEqual("btn", grid.FindGroupRowTemplateControl(0, "btn").ID);
		}
		[Test]
		public void TestDetailRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.SettingsDetail.ShowDetailRow = true;
			grid.DetailRows.ExpandRow(0);
			grid.Templates.DetailRow = button;
			Assert.AreEqual("btn", grid.FindDetailRowTemplateControl(0, "btn").ID);
			grid.DetailRows.CollapseRow(0);
			grid.BuildHtml();
			Assert.AreEqual(null, grid.FindDetailRowTemplateControl(0, "btn"));
		}
		[Test]
		public void TestPreviewRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Settings.ShowPreview = true;
			grid.Templates.PreviewRow = button;
			Assert.AreEqual("btn", grid.FindPreviewRowTemplateControl(0, "btn").ID);
			grid.Settings.ShowPreview = false;
			grid.BuildHtml();
			Assert.AreEqual(null, grid.FindPreviewRowTemplateControl(0, "btn"));
		}
		[Test]
		public void TestGroupRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.GroupBy(grid.DataColumns[0]);
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.GroupRow = button;
			Assert.AreEqual(null, grid.FindRowCellTemplateControl(0, grid.DataColumns[0], "btn"));
			Assert.AreEqual("btn", grid.FindGroupRowTemplateControl(0, "btn").ID);
		}
		[Test]
		public void TestDataRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.DataRow = button;
			Assert.AreEqual("btn", grid.FindRowTemplateControl(0, "btn").ID);
		}
		[Test]
		public void TestTitleTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.Settings.ShowTitlePanel = true;
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.TitlePanel = button;
			Assert.AreEqual("btn", grid.FindTitleTemplateControl("btn").ID);
		}
		[Test]
		public void TestEmptyDataRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.FilterExpression = "column1=-1";
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.EmptyDataRow = button;
			grid.DataBind();
			Assert.AreEqual(0, grid.DataProxy.VisibleRowCount);
			Assert.AreEqual("btn", grid.FindEmptyDataRowTemplateControl("btn").ID);
		}
		[Test]
		public void TestFooterRowTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Settings.ShowFooter = true;
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.FooterRow = button;
			Assert.AreEqual("btn", grid.FindFooterRowTemplateControl("btn").ID);
		}
		[Test]
		public void TestFooterCellTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.Settings.ShowFooter = true;
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Columns[2].FooterTemplate = button;
			Assert.AreEqual("btn", grid.FindFooterCellTemplateControl(grid.Columns[2], "btn").ID);
		}
		[Test]
		public void TestEditFormTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			grid.Settings.ShowStatusBar = GridViewStatusBarMode.Visible;
			button.ID = "btn";
			grid.Templates.EditForm = button;
			grid.SettingsEditing.Mode = GridViewEditingMode.EditForm;
			grid.StartEdit(0);
			Assert.AreEqual("btn", grid.FindEditFormTemplateControl("btn").ID);
		}
		[Test]
		public void TestPagerBarTemplate() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			MyTestTemplateButton button = new MyTestTemplateButton();
			button.ID = "btn";
			grid.Templates.PagerBar = button;
			Assert.AreEqual("btn", grid.FindPagerBarTemplateControl("btn", GridViewPagerBarPosition.Bottom).ID);
		}
	}
	[TestFixture]
	public class GridViewFocuseRowTests {
		[Test]
		public void SynhronizeWithDataProxy() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			Assert.AreEqual(false, grid.SettingsBehavior.AllowFocusedRow);
			Assert.AreEqual(false, grid.DataProxy.AllowFocusedRow);
			grid.SettingsBehavior.AllowFocusedRow = true;
			Assert.AreEqual(true, grid.SettingsBehavior.AllowFocusedRow);
			Assert.AreEqual(true, grid.DataProxy.AllowFocusedRow);
		}
		[Test]
		public void LoadFromViewState() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsBehavior.AllowFocusedRow = true;
			Assert.AreEqual(1, grid.FocusedRowChangedCounter);
			grid = grid.CreateGridFromStream();
			Assert.AreEqual(true, grid.SettingsBehavior.AllowFocusedRow);
			Assert.AreEqual(true, grid.DataProxy.AllowFocusedRow);
			Assert.AreEqual(0, grid.FocusedRowChangedCounter);
		}
		[Test]
		public void FocusedRowFromCallBack() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.SettingsBehavior.AllowFocusedRow = true;
			grid = grid.CreateGridAndApplyAddonParams("FR", "5");
			Assert.AreEqual(5, grid.DataProxy.FocusedRowVisibleIndex);
			grid.PageIndex = 1;
			grid = grid.CreateGridAndApplyAddonParams("FR", "15");
			Assert.AreEqual(15, grid.DataProxy.FocusedRowVisibleIndex);
		}
		[Test]
		public void GoToPageByChangingFocusingRow() {
			ASPxGridViewTester grid = new ASPxGridViewTester(true);
			grid.DataBind();
			grid.SettingsBehavior.AllowFocusedRow = true;
			object keyValue = grid.GetRowValues(44, grid.KeyFieldName);
			grid = grid.CreateGridFromStream();
			grid.FocusedRowIndex = grid.FindVisibleIndexByKeyValue(keyValue);
			Assert.AreEqual(4, grid.PageIndex);
		}
		[Test]
		public void SortGroupSynchronization() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsBehavior.AllowFocusedRow = true;
			Assert.AreEqual(1, grid.FocusedRowChangedCounter);
			grid.SortBy(grid.DataColumns[0], ColumnSortOrder.Descending);
			Assert.AreEqual(2, grid.FocusedRowChangedCounter);
		}
		[Test]
		public void FocusedRowSynchronization() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsBehavior.AllowFocusedRow = true;
			grid.GroupBy(grid.DataColumns[0]);
			grid.FocusedRowIndex = 1;
			grid.FocusedRowChangedCounter = 0;
			Assert.AreEqual(1, grid.FocusedRowIndex);
			grid.ExpandAll();
			Assert.AreEqual(1, grid.FocusedRowChangedCounter);
		}
		[Test]
		public void FocusedRowOutOfBoundsSynchronization() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataBind();
			grid.SettingsBehavior.AllowFocusedRow = true;
			grid.GroupBy(grid.DataColumns[0]);
			grid.ExpandAll();
			grid.FocusedRowIndex = grid.DataProxy.VisibleRowCount - 1;
			grid.CollapseAll();
			Assert.AreEqual(grid.DataProxy.VisibleRowCount - 1, grid.FocusedRowIndex);
		}
	}
	[TestFixture]
	public class GridViewColumnResizingTests {
		[Test]
		public void ColumnResizeFromCallbackOneColumn() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndApplyAddonParams("CR", "|50|");
			Assert.AreEqual(true, grid.Columns[0].Width.IsEmpty);
			Assert.AreEqual(false, grid.Columns[1].Width.IsEmpty);
			Assert.AreEqual(50, grid.Columns[1].Width.Value);
			Assert.AreEqual(UnitType.Pixel, grid.Columns[1].Width.Type);
		}
		[Test]
		public void ColumnResizeFromCallbackAndThenPersist() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid = grid.CreateGridAndApplyAddonParams("CR", "20%|100|300");
			Assert.AreEqual(Unit.Percentage(20), grid.Columns[0].Width);
			Assert.AreEqual(Unit.Pixel(100), grid.Columns[1].Width);
			Assert.AreEqual(Unit.Pixel(300), grid.Columns[2].Width);
			grid.AllowTestGridSaveViewState = false;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Control;
			grid = grid.CreateGridAndPerformCallBack("SORT|1||");
			Assert.AreEqual(Unit.Percentage(20), grid.Columns[0].Width);
			Assert.AreEqual(Unit.Pixel(100), grid.Columns[1].Width);
			Assert.AreEqual(Unit.Pixel(300), grid.Columns[2].Width);
		}
		[Test]
		public void ColumnResizeWithGrouping() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.GroupBy(grid.Columns[1]);
			grid = grid.CreateGridAndApplyAddonParams("CR", "15|20%|100|300");
			Assert.AreEqual(Unit.Percentage(20), grid.Columns[0].Width);
			Assert.AreEqual(Unit.Empty, grid.Columns[1].Width);
			Assert.AreEqual(Unit.Pixel(100), grid.Columns[2].Width);
			Assert.AreEqual(Unit.Pixel(300), grid.Columns[3].Width);
		}
		[Test]
		public void ColumnResizeWithDetailRow() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.SettingsDetail.ShowDetailRow = true;
			grid = grid.CreateGridAndApplyAddonParams("CR", "15|20%|100|300");
			Assert.AreEqual(Unit.Percentage(20), grid.Columns[0].Width);
			Assert.AreEqual(Unit.Pixel(100), grid.Columns[1].Width);
			Assert.AreEqual(Unit.Pixel(300), grid.Columns[2].Width);
		}
	}
	[TestFixture]
	public class TestStandardPaging {
		[Test]
		public void TestPaging() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataSourceForceStandardPaging = true;
			grid.SettingsPager.PageSize = 2;
			grid.PageIndex = 3;
			Assert.AreEqual(1, grid.View.selectHistory.Count);
			Assert.AreEqual(6, grid.View.selectHistory[0].StartRowIndex);
			Assert.AreEqual(2, grid.View.selectHistory[0].MaximumRows);
		}
		[Test]
		public void TestDataRowCache() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataSourceForceStandardPaging = true;
			grid.DataBind();
			int visibleCount = grid.VisibleRowCount;
			grid = grid.CreateGridAndPerformCallBack("COLUMNMOVE|0|2|true|");
			Assert.AreEqual(visibleCount, grid.VisibleRowCount);
		}
		[Test]
		public void TestSelectCountOnPaging() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataSourceForceStandardPaging = true;
			grid.SettingsPager.PageSize = 2;
			grid = grid.CreateGridAndPerformCallBack("NEXTPAGE");
			Assert.AreEqual(1, grid.PageIndex);
			Assert.AreEqual(2, grid.View.selectHistory.Count);
			Assert.AreEqual(true, grid.View.selectHistory[0].RetrieveTotalRowCount);
			Assert.AreEqual(0, grid.View.selectHistory[0].MaximumRows);
			grid = grid.CreateGridAndPerformCallBack("GOTOPAGE|1000");
			Assert.AreEqual(24, grid.PageIndex);
		}
		[Test]
		public void TestSortingOnPaging() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			grid.DataSourceForceStandardPaging = true;
			grid.SettingsPager.PageSize = 5;
			grid.SortBy(grid.DataColumns[0], ColumnSortOrder.Ascending);
			Assert.AreEqual(1, grid.View.selectHistory.Count);
			Assert.AreEqual("id ASC", grid.View.selectHistory[0].SortExpression);
		}
	}
	[TestFixture]
	public class B31640 {
		[Test]
		public void Run() {
			ASPxGridView grid;
			grid = CreateGrid(null, null, "GB|19;4|SORT1|00|0|4|true;");
			grid = CreateGrid(grid.GetCallbackStateString(), null, "GB|19;4|SORT1|00|0|4|true;");
			grid = CreateGrid(grid.GetCallbackStateString(), "T", null);
			Assert.AreEqual(1, grid.Selection.Count);
			Assert.IsTrue(grid.Selection.IsRowSelectedByKey(4));
		}
		ASPxGridView CreateGrid(string callbackState, string selection, string command) {
			ASPxGridView grid = new ASPxGridView();
			grid.ID = "g";
			grid.KeyFieldName = "id";
			grid.EnableRowsCache = false;
			grid.Columns.Add(new GridViewDataColumn("id"));
			grid.SettingsPager.PageSize = 2;
			grid.DataSource = new WebDataProxyTester(4).Table;
			grid.DataBind();
			if(callbackState != null) {
				NameValueCollection post = new NameValueCollection();
				post[grid.UniqueID + "$" + ASPxGridViewRenderHelper.CallbackHiddenFieldName] = callbackState;
				post[grid.UniqueID + "$" + ASPxGridViewRenderHelper.DXSelectedInputString] = selection;
				(grid as IPostBackDataHandler).LoadPostData("", post);
			}
			if(command != null) {
				ICallbackEventHandler handler = grid as ICallbackEventHandler;
				handler.RaiseCallbackEvent(command);
			}
			grid.OnAfterCreateControlHierarchy();
			return grid;
		}
	}
	[TestFixture]
	public class Q133197 {
		[Test]
		public void Run() {
			ASPxGridView grid;
			grid = DoAction(null, "EXPANDROW", 0);
			grid = DoAction(grid, "EXPANDROW", 1);
			grid = DoAction(grid, "EXPANDROW", 2);
			grid = DoAction(grid, "EXPANDROW", 4);
			grid = DoAction(grid, "COLLAPSEROW", 4);
			grid = CreateGrid(grid.GetCallbackStateString(), "GB|15;CUSTOMCALLBACK|;");
			Assert.IsFalse(grid.IsRowExpanded(4));
		}
		ASPxGridView DoAction(ASPxGridView grid, string name, int index) {
			string state = grid == null ? null : grid.GetCallbackStateString();
			string command = string.Format("{0}|{1}|false", name, index);
			return CreateGrid(state, string.Format("GB|{0};{1};", command.Length, command));
		}
		ASPxGridView CreateGrid(string callbackState, string command) {
			ASPxGridView grid = new ASPxGridView();
			grid.ID = "g";
			string[] names = new string[] { "A", "B", "C" };
			foreach(string name in names)
				grid.Columns.Add(new GridViewDataColumn(name));
			grid.DataSource = CreateData();
			grid.DataBind();
			for(int i = 0; i < 3; i++)
				grid.GroupBy(grid.Columns[i], i);
			if(callbackState != null) {
				NameValueCollection post = new NameValueCollection();
				post[grid.UniqueID + "$" + ASPxGridViewRenderHelper.CallbackHiddenFieldName] = callbackState;
				(grid as IPostBackDataHandler).LoadPostData("", post);
			}
			if(command != null)
				(grid as ICallbackEventHandler).RaiseCallbackEvent(command);
			grid.OnAfterCreateControlHierarchy();
			return grid;
		}
		object CreateData() {
			DataTable table = new DataTable();
			table.Columns.Add("A");
			table.Columns.Add("B");
			table.Columns.Add("C");
			table.Rows.Add(1, 1, 2);
			table.Rows.Add(1, 1, 1);
			table.Rows.Add(2, 1, 2);
			return table;
		}
	}
	[TestFixture]
	public class B132255_B34710 {
		[Test]
		public void Run() {
			ASPxGridViewTester grid = new ASPxGridViewTester();
			TestGroupUngroupSortOrder(grid, ColumnSortOrder.None, ColumnSortOrder.Ascending);
			TestGroupUngroupSortOrder(grid, ColumnSortOrder.Ascending, ColumnSortOrder.Ascending);
			TestGroupUngroupSortOrder(grid, ColumnSortOrder.Descending, ColumnSortOrder.Descending); 
		}
		void TestGroup(ASPxGridViewTester grid, ColumnSortOrder order, ColumnSortOrder newOrder, bool isGroupByGrid) {
			if(isGroupByGrid)
				grid.GroupBy(grid.DataColumns[0]);
			else
				grid.DataColumns[0].GroupBy();
			Assert.AreEqual(newOrder, grid.DataColumns[0].SortOrder);
		}
		void TestUngroup(ASPxGridViewTester grid, ColumnSortOrder order, ColumnSortOrder newOrder, bool isGroupByGrid) {
			if(isGroupByGrid)
				grid.UnGroup(grid.DataColumns[0]);
			else
				grid.DataColumns[0].UnGroup();
			Assert.AreEqual(newOrder, grid.DataColumns[0].SortOrder);
		}
		void TestGroupUngroupSortOrder(ASPxGridViewTester grid, ColumnSortOrder order, ColumnSortOrder newOrder) {
			grid.SortBy(grid.DataColumns[0], order);
			for(int i = 0; i < 4; i++) {
				TestGroup(grid, order, newOrder, (i & 2) > 0);
				TestUngroup(grid, newOrder, order, (i & 1) > 0);
			}
		}
	}
	[TestFixture]
	public class B35049 {
		ASPxGridView grid;
		[Test]
		public void Run() {
			Init();
			grid.GroupBy(grid.Columns["id"]);
			grid.ExpandRow(0);
			grid.Selection.SelectAll();
			grid.OnAfterCreateControlHierarchy(); 
			string state = grid.GetCallbackStateString();
			string selection = grid.RenderHelper.GetSelectInputValue();
			Init();
			NameValueCollection post = new NameValueCollection();
			post[grid.UniqueID + "$" + ASPxGridViewRenderHelper.CallbackHiddenFieldName] = state;
			post[grid.UniqueID + "$" + ASPxGridViewRenderHelper.DXSelectedInputString] = selection;
			(grid as IPostBackDataHandler).LoadPostData("", post);			
		}
		void Init() {
			grid = new ASPxGridViewTester();
			grid.ID = "g";
			grid.EnableRowsCache = false;
			grid.DataSource = new WebDataProxyTester(1).Table;
			grid.DataBind();
		}
	}
	[TestFixture]
	public class B134745_B134906 {
		class MyTemplate : ITemplate {
			void ITemplate.InstantiateIn(Control container) {
				TextBox box = new TextBox();
				box.ID = "a";
				container.Controls.Add(box);
			}
		}
		[Test]
		public void B134906() {
			object o = null;
			ASPxGridView grid = CreateGrid();
			grid.InitNewRow += delegate(object s, ASPxDataInitNewRowEventArgs e) {
				o = grid.FindEditRowCellTemplateControl(grid.DataColumns[0], "a");
			};
			grid.AddNewRow();
			Assert.IsInstanceOfType(typeof(TextBox), o);
		}
		[Test]
		public void B134745() {
			object o = null;
			ASPxGridView grid = CreateGrid();
			grid.StartRowEditing += delegate(object s, ASPxStartRowEditingEventArgs e) {
				o = grid.FindEditRowCellTemplateControl(grid.DataColumns[0], "a");
			};
			grid.StartEdit(0);
			Assert.IsInstanceOfType(typeof(TextBox), o);
		}
		ASPxGridView CreateGrid() {
			ASPxGridViewTester tester = new ASPxGridViewTester();
			tester.DataColumns[0].EditItemTemplate = new MyTemplate();
			tester.DataBind();
			return tester;
		}
	}
}
#endif
