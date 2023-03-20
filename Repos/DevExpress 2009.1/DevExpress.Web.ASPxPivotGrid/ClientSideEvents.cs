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
using System.Web.UI;
using DevExpress.Web.ASPxClasses;
using DevExpress.Utils.Serializing;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridClientSideEvents : ClientSideEvents {
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client pivot grid's ASPxClientPivotGrid.CustomizationFieldsVisibleChanged event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CustomizationFieldsVisibleChanged {
			get { return GetEventHandler("CustomizationFieldsVisibleChanged"); }
			set { SetEventHandler("CustomizationFieldsVisibleChanged", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle the ASPxClientPivotGrid.CellClick client event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CellClick {
			get { return GetEventHandler("CellClick"); }
			set { SetEventHandler("CellClick", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle the ASPxClientPivotGrid.CellDblClick client event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CellDblClick {
			get { return GetEventHandler("CellDblClick"); }
			set { SetEventHandler("CellDblClick", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle the client ASPxClientPivotGrid.BeforeCallback event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string BeforeCallback {
			get { return GetEventHandler("BeforeCallback"); }
			set { SetEventHandler("BeforeCallback", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle the client ASPxClientPivotGrid.AfterCallback event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string AfterCallback {
			get { return GetEventHandler("AfterCallback"); }
			set { SetEventHandler("AfterCallback", value); }
		}
		[Description("Gets or sets the name of the JavaScript function or the entire code which will handle a client pivot grid's ASPxClientPivotGrid.PopupMenuItemClick event."),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string PopupMenuItemClick {
			get { return GetEventHandler("PopupMenuItemClick"); }
			set { SetEventHandler("PopupMenuItemClick", value); }
		}
		[Description(""),
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(DevExpress.Web.ASPxClasses.Design.ClientSideEventsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public new string Init {
			get { return GetEventHandler("Init"); }
			set { SetEventHandler("Init", value); }
		}
		public PivotGridClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("CustomizationFieldsVisibleChanged");
			names.Add("CellClick");
			names.Add("CellDblClick");
			names.Add("BeforeCallback");
			names.Add("AfterCallback");
			names.Add("PopupMenuItemClick");
		}
	}
}
