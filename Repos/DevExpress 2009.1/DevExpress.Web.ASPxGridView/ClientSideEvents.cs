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
using System.IO;
using DevExpress.Data.IO;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxGridView.Helper;
namespace DevExpress.Web.ASPxGridView {
	public class GridViewClientSideEvents : CallbackClientSideEvents {
		public GridViewClientSideEvents() : base() {
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.SelectionChanged event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectionChanged {
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.FocusedRowChanged event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FocusedRowChanged {
			get { return GetEventHandler("FocusedRowChanged"); }
			set { SetEventHandler("FocusedRowChanged", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.RowClick event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string RowClick {
			get { return GetEventHandler("RowClick"); }
			set { SetEventHandler("RowClick", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.RowDblClick event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string RowDblClick {
			get { return GetEventHandler("RowDblClick"); }
			set { SetEventHandler("RowDblClick", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ContextMenu event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ContextMenu {
			get { return GetEventHandler("ContextMenu"); }
			set { SetEventHandler("ContextMenu", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ColumnSorting event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnSorting {
			get { return GetEventHandler("ColumnSorting"); }
			set { SetEventHandler("ColumnSorting", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ColumnGrouping event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnGrouping {
			get { return GetEventHandler("ColumnGrouping"); }
			set { SetEventHandler("ColumnGrouping", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ColumnMoving event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnMoving {
			get { return GetEventHandler("ColumnMoving"); }
			set { SetEventHandler("ColumnMoving", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ColumnStartDragging event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnStartDragging {
			get { return GetEventHandler("ColumnStartDragging"); }
			set { SetEventHandler("ColumnStartDragging", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.ColumnResizing event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnResizing {
			get { return GetEventHandler("ColumnResizing"); }
			set { SetEventHandler("ColumnResizing", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.RowExpanding event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string RowExpanding {
			get { return GetEventHandler("RowExpanding"); }
			set { SetEventHandler("RowExpanding", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.RowCollapsing event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string RowCollapsing {
			get { return GetEventHandler("RowCollapsing"); }
			set { SetEventHandler("RowCollapsing", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.DetailRowExpanding event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string DetailRowExpanding {
			get { return GetEventHandler("DetailRowExpanding"); }
			set { SetEventHandler("DetailRowExpanding", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.DetailRowCollapsing event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string DetailRowCollapsing {
			get { return GetEventHandler("DetailRowCollapsing"); }
			set { SetEventHandler("DetailRowCollapsing", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.CustomizationWindowCloseUp event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomizationWindowCloseUp {
			get { return GetEventHandler("CustomizationWindowCloseUp"); }
			set { SetEventHandler("CustomizationWindowCloseUp", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client ASPxGridView's ASPxClientGridView.CustomButtonClick event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomButtonClick {
			get { return GetEventHandler("CustomButtonClick"); }
			set { SetEventHandler("CustomButtonClick", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.AddRange(new string[] {
				"SelectionChanged",
				"FocusedRowChanged",
				"ColumnGrouping",
				"ColumnResizing",
				"ColumnSorting",
				"ColumnMoving",
				"ColumnStartDragging", 
				"RowExpanding",
				"RowCollapsing",
				"DetailRowExpanding",
				"DetailRowCollapsing",
				"RowClick",
				"RowDblClick",
				"ContextMenu",
				"CustomizationWindowCloseUp",
				"CustomButtonClick",
			});
		}
   }
}
