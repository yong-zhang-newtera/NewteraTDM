#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraPivotGrid                                 }
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Data;
using System.Data.OleDb;
using Microsoft.Win32;
namespace DevExpress.XtraPivotGrid.Design {
	public partial class OLAPConnectionEditorForm : XtraForm {
		OLAPConnectionStringBuilder options;
		internal OLAPConnectionStringBuilder Options { get { return options; } }
		public OLAPConnectionEditorForm(OLAPConnectionStringBuilder options) {
			InitializeComponent();
			ShowInTaskbar = false;
			this.options = options;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.propertyGrid.SelectedObject = Options;
			this.propertyGrid.AddTabs();
			this.propertyGrid.SelectedTab = typeof(BasicTab);
		}
		private void ceAdvanced_CheckedChanged(object sender, EventArgs e) {
			if(ceAdvanced.Checked)
				this.propertyGrid.SelectedTab = typeof(AdvancedTab);
			else
				this.propertyGrid.SelectedTab = typeof(BasicTab);
		}
	}
}
