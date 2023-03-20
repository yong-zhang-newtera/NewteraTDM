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
using System.Collections.Generic;
using DevExpress.Utils.About;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;
namespace DevExpress.Web.ASPxGridView.Design {
	public class GridViewAboutDialogHelper : AboutDialogHelperBase {
		private static bool fTrialAboutShown = false;
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxGridView), ProductKind.ASPxGridView,
				GetProductInfoStage(typeof(ASPxGridView)), "DevExpress.Web.ASPxGridView.About.png", 298, 171);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(IsTrial(typeof(ASPxGridView)) && !fTrialAboutShown) {
				ShowAbout(provider);
				fTrialAboutShown = true;
			}
		}
		public static void DisplayDataSourceWarningMessage() {
			MessageBox.Show("Activating 'DataSourceForceStandardPaging' automatically disables built-in unbound columns, grouping, summaries and filtering support.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}
	public class EditorContextHelper : IWindowsFormsEditorService, ITypeDescriptorContext, IServiceProvider {
		public EditorContextHelper(ComponentDesigner designer, PropertyDescriptor prop) {
			this.designer = designer;
			this.targetProperty = prop;
			if(prop == null) {
				prop = TypeDescriptor.GetDefaultProperty(designer.Component);
				if((prop != null) && typeof(ICollection).IsAssignableFrom(prop.PropertyType)) {
					this.targetProperty = prop;
				}
			}
		}
		public static void RefreshSmartPanel(IComponent component) {
			if(component == null || component.Site == null) return;
			DesignerActionUIService uiService = component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(uiService != null) uiService.Refresh(component);
		}
		public static void SetPropertyValue(ComponentDesigner designer, object component, string property, object value) {
			SetPropertyValue(designer, component, property, value, null);
		}
		public static void SetPropertyValue(IServiceProvider provider, object component, string property, object value) {
			SetPropertyValue(provider, component, property, value, null);
		}
		public static void SetPropertyValue(ComponentDesigner designer, object component, string property, object value, object notifyComponent) {
			if(designer == null || designer.Component == null || designer.Component.Site == null) return;
			SetPropertyValue(designer.Component.Site, component, property, value, notifyComponent);
		}
		public static void SetPropertyValue(IServiceProvider provider, object component, string property, object value, object notifyComponent) {
			if(provider == null) return;
			IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			IComponentChangeService service = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			PropertyDescriptor desc = TypeDescriptor.GetProperties(component)[property];
			if(host != null && service != null && desc != null) {
				using(DesignerTransaction transaction1 = host.CreateTransaction(string.Format("Set property '{0}'", property))) {
					service.OnComponentChanging(component, desc);
					desc.SetValue(component, value);
					service.OnComponentChanged(component, desc, null, null);
					transaction1.Commit();
					if(notifyComponent != null) service.OnComponentChanged(notifyComponent, desc, null, null);
				}
			}
		}
		public static void FireChanged(ComponentDesigner designer, object component) { FireChanged(designer, component, null); }
		public static void FireChanged(ComponentDesigner designer, object component, string property) {
			if(designer == null || designer.Component == null || designer.Component.Site == null) return;
			FireChanged(designer.Component.Site, component, property);
		}
		public static void FireChanged(IServiceProvider provider, object component) { FireChanged(provider, component, null); }
		public static void FireChanged(IServiceProvider provider, object component, string property) {
			if(provider == null) return;
			IComponentChangeService service = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			PropertyDescriptor descriptor = null;
			if(property != null) descriptor = TypeDescriptor.GetProperties(component)[property];
			if(service != null) service.OnComponentChanged(component, descriptor, null, null);
		}
		public static object EditValue(ComponentDesigner designer, object objectToChange, string propName) {
			PropertyDescriptor pd = TypeDescriptor.GetProperties(objectToChange)[propName];
			EditorContextHelper context = new EditorContextHelper(designer, pd);
			UITypeEditor editor = pd.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			object obj1 = pd.GetValue(objectToChange);
			object obj2 = editor.EditValue(context, context, obj1);
			if(obj2 != obj1) {
				try {
					pd.SetValue(objectToChange, obj2);
				} catch(CheckoutException) { }
			}
			return obj2;
		}
		IComponentChangeService changeService;
		ComponentDesigner designer;
		PropertyDescriptor targetProperty;
		void ITypeDescriptorContext.OnComponentChanged() {
			ChangeService.OnComponentChanged(this.designer.Component, this.targetProperty, null, null);
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			try {
				this.ChangeService.OnComponentChanging(this.designer.Component, this.targetProperty);
			} catch(CheckoutException e) {
				if(e != CheckoutException.Canceled) throw;
				return false;
			}
			return true;
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService)) {
				return this;
			}
			if(this.designer.Component.Site != null) {
				return this.designer.Component.Site.GetService(serviceType);
			}
			return null;
		}
		IComponentChangeService ChangeService {
			get {
				if(changeService == null)
					changeService = (IComponentChangeService)((IServiceProvider)this).GetService(typeof(IComponentChangeService));
				return changeService;
			}
		}
		IContainer ITypeDescriptorContext.Container {
			get {
				if(designer.Component.Site != null) {
					return designer.Component.Site.Container;
				}
				return null;
			}
		}
		object ITypeDescriptorContext.Instance { get { return designer.Component; } }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return targetProperty; } }
		DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
			IUIService service1 = (IUIService)((IServiceProvider)this).GetService(typeof(IUIService));
			if(service1 != null) {
				return service1.ShowDialog(dialog);
			}
			return dialog.ShowDialog(this.designer.Component as IWin32Window);
		}
		void IWindowsFormsEditorService.DropDownControl(System.Windows.Forms.Control control) { }
		void IWindowsFormsEditorService.CloseDropDown() { }
	}
}
