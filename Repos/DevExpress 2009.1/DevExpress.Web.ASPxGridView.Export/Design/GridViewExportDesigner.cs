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
using System.Web.UI.Design;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design;
using System.Web.UI;
using DevExpress.Web.ASPxGridView.Design;
namespace DevExpress.Web.ASPxGridView.Export.Design {
	public class ASPxGridViewExportDesigner : ControlDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host != null){
				DevExpress.Utils.Design.ReferencesHelper.EnsureReferences(host,
					"DevExpress.Data" + AssemblyInfo.VSuffix,
					"DevExpress.Web" + AssemblyInfo.VSuffix,
					"DevExpress.Web.ASPxEditors" + AssemblyInfo.VSuffix,
					"DevExpress.Web.ASPxGridView" + AssemblyInfo.VSuffix
				);
			}
		}
		public override string GetDesignTimeHtml() {
			return base.CreatePlaceHolderDesignTimeHtml();
		}
	}
	public class GridViewIDConverter : GridViewBaseStringListConverter {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			IDesignerHost service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
			if(service == null || service.Container == null) return;
			foreach(IComponent component in service.Container.Components) {
				ASPxGridView grid = component as ASPxGridView;
				if(((grid != null) && !string.IsNullOrEmpty(grid.ID))) {
					list.Add(grid.ID);
				}
			}
		}
	}
}
