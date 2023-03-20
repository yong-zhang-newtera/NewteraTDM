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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Utils.About;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Web.ASPxClasses.Design.Forms;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class FieldsEditorForm : TwoSidesFormBase {
		ASPxPivotGrid pivotGrid;
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected PivotGridFieldCollection Fields { get { return Data.Fields; } }
		public FieldsEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
			this.pivotGrid = component as ASPxPivotGrid;
			if(this.pivotGrid == null) throw new Exception("FieldsEditorForm should be used with ASPxPivotGrid.");
			this.ShowInTaskbar = false;
		}		
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				fListBox.SelectedIndexChanged -= new EventHandler(fListBox_SelectedIndexChanged);
				fListBox.KeyDown -= new KeyEventHandler(fListBox_KeyDown);
			}
		}
		ListBox fListBox;
		PropertyGrid fPropertyGrid;
		ToolStrip fToolStrip;
		protected override void AddControlsToLeftPanel(Panel leftPanel) {
			fListBox = new ListBoxItemPainter(true).CreateListBox(Font);
			fListBox.Dock = DockStyle.Fill;
			fListBox.Margin = new Padding(0);
			fListBox.SelectedIndexChanged += new EventHandler(fListBox_SelectedIndexChanged);
			fListBox.KeyDown += new KeyEventHandler(fListBox_KeyDown);
			fListBox.SelectionMode = SelectionMode.MultiExtended;
			leftPanel.Controls.Add(fListBox);
			FillListBox();
			fToolStrip = new ToolStrip();
			fToolStrip.Renderer = DesignUtils.GetToolStripRenderer(ServiceProvider);
			fToolStrip.CanOverflow = false;
			fToolStrip.GripStyle = ToolStripGripStyle.Hidden;
			fToolStrip.AutoSize = false;
			fToolStrip.Dock = DockStyle.Top;
			fToolStrip.Margin = new Padding(0);
			fToolStrip.Items.AddRange(GetToolStripButtons());
			leftPanel.Controls.Add(fToolStrip);
		}
		void fListBox_KeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Delete:
					OnRemoveItemButtonClick(null, new EventArgs());
					break;
				case Keys.A:
					if(e.Control)
						SelectAll();
					break;
			}
		}
		void FillListBox() {
			int oldSelectedIndex = fListBox.SelectedIndex;
			fListBox.Items.Clear();
			for(int i = 0; i < Fields.Count; i++)
				fListBox.Items.Add(Fields[i]);
			fListBox.SelectedIndex = oldSelectedIndex;
		}
		void fListBox_SelectedIndexChanged(object sender, EventArgs e) {
			object[] selectedFields = new object[fListBox.SelectedItems.Count];
			for(int i = 0; i < fListBox.SelectedItems.Count; i++)
				selectedFields[i] = fListBox.SelectedItems[i];
			fPropertyGrid.SelectedObjects = selectedFields;
			UpdateTools();
		}
		protected override void AddControlsToRightPanel(Panel rightPanel) {
			fPropertyGrid = new FormPropertyGrid(ServiceProvider);
			fPropertyGrid.TabStop = true;
			fPropertyGrid.Dock = DockStyle.Fill;
			fPropertyGrid.Site = new CollectionPropertyGridSite(new CollectionServiceProvider(ServiceProvider, this), fPropertyGrid);
			rightPanel.Controls.Add(fPropertyGrid);			
		}
		protected override void ComponentChanged(bool checkChanged, bool isCollectionChangeNotification) {
			base.ComponentChanged(checkChanged, isCollectionChangeNotification);
			fListBox.Invalidate();
		}
		ToolStripItem[] GetToolStripButtons() {
			return new ToolStripItem[] { CreatePushButton(StringResources.ItemsEditor_AddItemButtonText, ItemsEditorFormBase.AddItemImageResource, OnAddItemButtonClick), 
				CreatePushButton(StringResources.ItemsEditor_RemoveItemButtonText, ItemsEditorFormBase.RemoveItemImageResource, OnRemoveItemButtonClick),
				CreateToolStripSeparator(),
				CreatePushButton(StringResources.ItemsEditor_RemoveAllItemsButtonText, ItemsEditorFormBase.RemoveAllItemImageResource, OnRemoveAllItemsButtonClick),
				CreateToolStripSeparator(),
				CreatePushButton("Retrieve fields", "", OnRetrieveFields)};
		}
		ToolStripItem CreateToolStripSeparator() {
			return new ToolStripSeparator();
		}
		ToolStripItem CreatePushButton(string toolTipText, string imageResource, EventHandler onClick) {
			Bitmap image = null;
			if(imageResource != "") {
				image = CreateBitmapFromResources(imageResource);
				image.MakeTransparent(Color.Magenta);
			}
			return CreatePushButton(toolTipText, image, onClick);
		}
		ToolStripButton CreatePushButton(string toolTipText, Image image, EventHandler onClick) {
			ToolStripButton button = null;
			if(image != null) {
				button = new ToolStripButton(toolTipText, image, onClick);
				button.DisplayStyle = ToolStripItemDisplayStyle.Image;
			} else {
				button = new ToolStripButton(toolTipText);
				button.Click += onClick;
				button.Text = toolTipText;
				button.DisplayStyle = ToolStripItemDisplayStyle.Text;
			}
			button.AutoToolTip = true;
			button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			return button;
		}
		void OnRetrieveFields(object sender, EventArgs e) {
			if(string.IsNullOrEmpty(PivotGrid.OLAPConnectionString)) {
				IDataSourceViewSchemaAccessor accessor = PivotGrid as IDataSourceViewSchemaAccessor;
				if(accessor == null) return;
				IDataSourceViewSchema schema = accessor.DataSourceViewSchema as IDataSourceViewSchema;
				if(schema == null) return;
				IDataSourceFieldSchema[] fields = schema.GetFields();
				Data.BeginUpdate();
				Data.Fields.ClearAndDispose(); ;
				for(int i = 0; i < fields.Length; i++)
					Data.Fields.Add(fields[i].Name, DevExpress.XtraPivotGrid.PivotArea.FilterArea);
				Data.EndUpdate();
			}
			else
				Data.RetrieveFields();
			FillListBox();
			ComponentChanged(false);
		}
		void OnAddItemButtonClick(object sender, EventArgs e) {
			AddNewItem();
			UpdateTools();
			ComponentChanged(false);
			fListBox.SelectedItems.Clear();
			fListBox.SelectedIndex = fListBox.Items.Count - 1;
		}
		void OnRemoveItemButtonClick(object sender, EventArgs e) {
			RemoveItem();
			UpdateTools();
			ComponentChanged(false);
		}		
		void OnRemoveAllItemsButtonClick(object sender, EventArgs e) {
			if(MessageBoxEx.Show(this, StringResources.ItemsEditor_RemoveAllConfirmDialogText,
				StringResources.ItemsEditor_ConfirmDialogCaption, MessageBoxButtonsEx.OKCancel) == DialogResultEx.OK) {
				fPropertyGrid.SelectedObject = null;
				RemoveAllItems();
				UpdateTools();
				ComponentChanged(false);
			}
		}	
		void AddNewItem() {
			fListBox.Items.Add(Fields.Add());
		}
		void RemoveItem() {
			if(fListBox.SelectedItems.Count == 0) return;
			PivotGridField[] selectedFields = GetSelectedFields();
			foreach(PivotGridField field in selectedFields) {
				Fields.Remove(field);
				fListBox.Items.Remove(field);
			}
		}
		private PivotGridField[] GetSelectedFields() {
			PivotGridField[] selectedFields = new PivotGridField[fListBox.SelectedItems.Count];
			for(int i = 0; i < fListBox.SelectedItems.Count; i++)
				selectedFields[i] = fListBox.SelectedItems[i] as PivotGridField;
			return selectedFields;
		}
		void SelectAll() {
			for(int i = 0; i < fListBox.Items.Count; i++)
				fListBox.SetSelected(i, true);
		}
		void RemoveAllItems() {
			foreach(PivotGridField field in fListBox.Items)
				Fields.Remove(field);
			fListBox.Items.Clear();
		}
		void UpdateTools() {
			ToolStripItem item = FindToolItemByText(StringResources.ItemsEditor_RemoveItemButtonText);
			item.Enabled = fListBox.SelectedIndex != -1;
			item = FindToolItemByText(StringResources.ItemsEditor_RemoveAllItemsButtonText);
			item.Enabled = fListBox.Items.Count > 0;
		}
		protected ToolStripItem FindToolItemByText(string text) {
			foreach(ToolStripItem item in fToolStrip.Items) {
				if(item.ToolTipText == text)
					return item;
			}
			return null;
		}
		string undoString;
		protected string UndoString { get { return undoString; } }
		protected override void SaveUndoData() {
			undoString = PivotGrid.SerializeSnapshot(PivotGrid.GetSnapshot());
		}
		protected override void Undo() {
			if(!String.IsNullOrEmpty(UndoString))
				PivotGrid.DeserializeSnapshot(UndoString);
		}
		protected override string GetPropertyStorePathPrefix() {
			return "FieldsEditorForm";
		}
		public override Bitmap CreateBitmapFromResources(string resourceName) {
			return CreateBitmapFromResources(resourceName, typeof(ItemsEditorFormBase).Assembly);
		}
		public class CollectionPropertyGridSite : ISite {
			IServiceProvider sp;
			IComponent comp;
			public CollectionPropertyGridSite(IServiceProvider sp, IComponent comp) {
				this.sp = sp;
				this.comp = comp;
			}
			IComponent ISite.Component {
				get { return comp; }
			}
			IContainer ISite.Container {
				get { return null; }
			}
			bool ISite.DesignMode {
				get { return false; }
			}
			string ISite.Name {
				get { return string.Empty; }
				set { }
			}
			object IServiceProvider.GetService(Type t) {
				return sp.GetService(t);
			}
		}
		public class CollectionServiceProvider : IServiceProvider {
			readonly IServiceProvider ParentProvider;
			readonly FieldsEditorForm Form;
			public CollectionServiceProvider(IServiceProvider parentProvider, FieldsEditorForm form) {
				ParentProvider = parentProvider;
				Form = form;
			}
			#region IServiceProvider Members
			public object GetService(Type serviceType) {
				if(serviceType == typeof(IComponentChangeService))
					return new CollectionComponentChangeService(Form);
				return null;
			}
			#endregion
		}
		public class CollectionComponentChangeService : IComponentChangeService {
			readonly FieldsEditorForm Form;
			public CollectionComponentChangeService(FieldsEditorForm form) {
				Form = form;
			}
			#region IComponentChangeService Members
			event ComponentEventHandler componentAdded;
			event ComponentEventHandler componentAdding;
			event ComponentChangedEventHandler componentChanged;
			event ComponentChangingEventHandler componentChanging;
			event ComponentEventHandler componentRemoved;
			event ComponentEventHandler componentRemoving;
			event ComponentRenameEventHandler componentRename;
			public event ComponentEventHandler ComponentAdded {
				add { componentAdded += value; }
				remove { componentAdded -= value; }
			}
			public event ComponentEventHandler ComponentAdding {
				add { componentAdding += value; }
				remove { componentAdding -= value; }
			}
			public event ComponentChangedEventHandler ComponentChanged {
				add { componentChanged += value; }
				remove { componentChanged -= value; }
			}
			public event ComponentChangingEventHandler ComponentChanging {
				add { componentChanging += value; }
				remove { componentChanging -= value; }
			}
			public event ComponentEventHandler ComponentRemoved {
				add { componentRemoved += value; }
				remove { componentRemoved -= value; }
			}
			public event ComponentEventHandler ComponentRemoving {
				add { componentRemoving += value; }
				remove { componentRemoving -= value; }
			}
			public event ComponentRenameEventHandler ComponentRename {
				add { componentRename += value; }
				remove { componentRename -= value; }
			}
			public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue) {
				if(component is FormPropertyGrid) return;
				Form.ComponentChanged(false);
			}
			public void OnComponentChanging(object component, MemberDescriptor member) {
				if(component is FormPropertyGrid) return;
				Form.ComponentChanging();
			}
			#endregion
		}
	}
	public class FieldsEditor : TypeEditorBase {
		public override EditorFormBase CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new FieldsEditorForm(component, context, provider, propertyValue);
		}
	}	
}
