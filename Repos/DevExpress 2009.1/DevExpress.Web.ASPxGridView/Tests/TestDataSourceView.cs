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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Data;
using System.ComponentModel;
namespace DevExpress.Web.ASPxGridView.Tests {
	public class ASPxTestDataSource : IDataSource {
		DataSourceView testView;
		public ASPxTestDataSource(DataTable table) {
			this.testView = new ASPxTestDataSourceView(this, table);
		}
		public event EventHandler DataSourceChanged = null;
		public DataSourceView GetView(string viewName) {
			return testView;
		}
		public ASPxTestDataSourceView View { get { return testView as ASPxTestDataSourceView; } }
		public ICollection GetViewNames() {
			return null;
		}
		void Raise() {
			if(DataSourceChanged != null) DataSourceChanged(this, EventArgs.Empty);
		}
	}
	public class ASPxTestDataSourceView : DataSourceView {
		DataTable table;
		public ASPxTestDataSourceView(ASPxTestDataSource owner, DataTable table)
			: base(owner, "test") {
			this.table = table;
		}
		protected DataTable Table { get { return table; } }
		internal List<DataSourceSelectArguments> selectHistory = new List<DataSourceSelectArguments>();
		internal List<IEnumerable> selectResults = new List<IEnumerable>();
		internal void ResetSelectResults() {
			this.selectHistory.Clear();
			this.selectResults.Clear();
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			this.selectHistory.Add(arguments);
			IEnumerable res = ExecuteCore(arguments);
			this.selectResults.Add(res);
			return res;
		}
		IEnumerable ExecuteCore(DataSourceSelectArguments arguments) {
			if(arguments.RetrieveTotalRowCount) {
				arguments.TotalRowCount = Table.DefaultView.Count;
			}
			table.DefaultView.Sort = arguments.SortExpression;
			if(arguments.TotalRowCount > -1) {
				TypedList rows = new TypedList((table.DefaultView as ITypedList).GetItemProperties(null));
				for(int n = arguments.StartRowIndex; n < table.DefaultView.Count; n++) {
					if(n - arguments.StartRowIndex == arguments.MaximumRows) break;
					rows.Add(table.DefaultView[n]);
				}
				return rows;
			}
			return Table.DefaultView;
		}
		class TypedList : List<object>, ITypedList {
			PropertyDescriptorCollection properties;
			public TypedList(PropertyDescriptorCollection properties) {
				this.properties = properties;
			}
			PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
				return properties;
			}
			string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return string.Empty; }
		}
		protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues) {
			DataRowView dw = FindRowView(keys);
			if(dw == null) return 0;
			foreach(DictionaryEntry entry in values) {
				dw[entry.Key.ToString()] = entry.Value;
			}
			return 1;
		}
		protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues) {
			DataRowView dw = FindRowView(keys);
			if(dw == null) return 0;
			dw.Delete();
			return 1;
		}
		protected override int ExecuteInsert(IDictionary values) {
			DataRow row = Table.NewRow();
			foreach(DictionaryEntry entry in values) {
				row[entry.Key.ToString()] = entry.Value;
			}
			Table.Rows.Add(row);
			return 1;
		}
		DataRowView FindRowView(IDictionary keys) {
			int index = FindRow(keys);
			if(index < 0) return null;
			return Table.DefaultView[index];
		}
		int FindRow(IDictionary keys) {
			for(int n = 0; n < Table.DefaultView.Count; n++) {
				object val = Table.DefaultView[n]["id"];
				if(object.Equals(val, keys["id"])) return n;
			}
			return -1;
		}
	}
}
#endif
