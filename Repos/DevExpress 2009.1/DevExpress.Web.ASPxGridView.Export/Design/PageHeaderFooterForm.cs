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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.Web.ASPxGridView.Export.Design {
	public class GridViewExporterPageHeaderFooterEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));
			ASPxGridViewExporter exporter = selectionService.PrimarySelection as ASPxGridViewExporter;
			if(exporter != null) {
				Form form = new GridViewExporterPageHeaderFooterForm(exporter);
				form.ShowDialog();
			}
			return value;
		}
	} 
	public class GridViewExporterPageHeaderFooterForm : DevExpress.XtraPrinting.Native.WinControls.HeaderFooterFormBase {
		ASPxGridViewExporter exporter;
		public GridViewExporterPageHeaderFooterForm(ASPxGridViewExporter exporter) {
			this.exporter = exporter;
			SetFormElements(HeaderSettings, HeadControls);
			HeadAligment = HeaderSettings.VerticalAlignment;
			SetFormElements(FooterSettings, FootControls);
			FootAligment = FooterSettings.VerticalAlignment;
		}
		public GridViewExporterHeaderFooter HeaderSettings { get { return exporter.PageHeader; } }
		public GridViewExporterHeaderFooter FooterSettings { get { return exporter.PageFooter; } }
		protected override bool IsFontBarItemVisible { get { return false; } }
		protected override bool IsImageBarItemVisible { get { return false; } }
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			FocusTabHeader();
		}
		protected override void Apply() {
			Apply(HeaderSettings, HeadControls, HeadAligment);
			Apply(FooterSettings, FootControls, FootAligment);
		}
		void Apply(GridViewExporterHeaderFooter settings, DevExpress.XtraEditors.MemoEdit[] controls, DevExpress.XtraPrinting.BrickAlignment align) {			
			settings.Left = controls[0].Text;
			settings.Center= controls[1].Text;
			settings.Right = controls[2].Text;
			settings.VerticalAlignment = align;
			PropertyChanged(exporter, "PageHeader");
			PropertyChanged(exporter, "PageFooter");
		}
		void SetFormElements(GridViewExporterHeaderFooter settings, DevExpress.XtraEditors.MemoEdit[] controls) {
			controls[0].Text = settings.Left;
			controls[1].Text = settings.Center;
			controls[2].Text = settings.Right;	
		}
		void PropertyChanged(IComponent component, string name) {
			IComponentChangeService changeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			changeService.OnComponentChanged(component, TypeDescriptor.GetProperties(component)[name], null, null);
		}
	}
}
