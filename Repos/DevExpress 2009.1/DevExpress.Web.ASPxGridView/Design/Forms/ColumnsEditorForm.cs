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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.Design;
using System.Collections;
using System.Drawing.Design;
using System.Drawing;
namespace DevExpress.Web.ASPxGridView.Design {
	public class GridViewColumnsCollectionEditor : DevExpress.Web.ASPxClasses.Design.CollectionEditor {
		public override EditorFormBase CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new GridViewColumnsCollectionEditorForm(component, context, provider, propertyValue);
		}
	}
	public class GridViewColumnsCollectionEditorForm : CollectionEditorForm {
		public GridViewColumnsCollectionEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override Size FormDefaultSize { get { return new Size(710, 500); } }
		protected override Size FormMinimumSize { get { return new Size(700, 500); } }
		protected override string GetPropertyStorePathPrefix() {
			return "ColumnsCollectionEditorForm";
		}
		ToolStripButton retrieveButton;
		ToolStripDropDownButton convertDropDown;
		protected override void AddToolStripButtons(List<ToolStripItem> buttons) {
			base.AddToolStripButtons(buttons);
			buttons.Add(CreateToolStripSeparator());
			this.convertDropDown = new ToolStripDropDownButton("Change To");
			this.convertDropDown.DropDownItems.AddRange(GetConvertDropDownItems());
			buttons.Add(this.convertDropDown);
			buttons.Add(CreateToolStripSeparator());
			this.retrieveButton = new ToolStripButton("Retrieve Columns", null, new EventHandler(OnRetrieveColumns));
			buttons.Add(retrieveButton);
		}
		ToolStripItem[] GetConvertDropDownItems() {
			List<ToolStripItem> items = new List<ToolStripItem>();
			foreach(CollectionItemType item in GetCollectionItemTypes()) {
				ToolStripMenuItem subItem = new ToolStripMenuItem(item.Text, null, new EventHandler(OnChangeColumnType));
				subItem.Tag = item;
				items.Add(subItem);
			}
			return items.ToArray();
		}
		protected ASPxGridView Grid { get { return Component as ASPxGridView; } }
		protected new GridViewDesigner Designer {
			get {
				return base.Designer as GridViewDesigner;
			}
		}
		protected override void SaveChanges() {
			Grid.AutoGenerateColumns = false;
			TypeDescriptor.Refresh(Grid);
			base.SaveChanges();
		}
		protected override bool NotifyCollectionChanges { get { return false; } }
		protected virtual void OnRetrieveColumns(object sender, EventArgs e) {
			if(GetDataSourceSchema() == null) return;
			if(Designer.ShowMessage(Grid.Site, "Would you like to regenerate the GridView column fields?  Warning: this will delete all existing column fields.", 
				string.Format("Refresh Fields and Keys for '{0}'", Grid.ID), MessageBoxButtons.YesNo) != DialogResult.Yes)  return;
			Grid.Columns.Clear();
			Designer.AddKeysAndBoundFields(GetDataSourceSchema(), false);
			AssignControls();
			ComponentChanged(false);
		}
		protected override void UpdateToolStrip() {
			base.UpdateToolStrip();
			this.retrieveButton.Enabled = GetDataSourceSchema() != null;
			this.convertDropDown.Enabled = GetSelectedItems().Length > 0;
		}
		private IDataSourceViewSchema GetDataSourceSchema() {
			return Designer != null ? Designer.GetDataSourceSchema() : null;
		}
		protected override List<CollectionItemType> GetCollectionItemTypes() {
			return new List<CollectionItemType>(
				new CollectionItemType[] { 
					  new CollectionItemType(typeof(GridViewDataTextColumn), "Text Column"),
					  new CollectionItemType(typeof(GridViewDataCheckColumn), "Check Column"),
					  new CollectionItemType(typeof(GridViewDataDateColumn), "Date Column"),
					  new CollectionItemType(typeof(GridViewDataSpinEditColumn), "Spin Edit Column"),
					  new CollectionItemType(typeof(GridViewDataComboBoxColumn), "Combobox Column"),
					  new CollectionItemType(typeof(GridViewDataMemoColumn), "Memo Column"),
					  new CollectionItemType(typeof(GridViewDataButtonEditColumn), "Button Edit Column"),
					  new CollectionItemType(typeof(GridViewDataHyperLinkColumn), "Hyperlink Column"),
					  new CollectionItemType(typeof(GridViewDataImageColumn), "Image Column"),
					  new CollectionItemType(typeof(GridViewDataBinaryImageColumn), "Binary Image Column"),
					  new CollectionItemType(typeof(GridViewDataProgressBarColumn), "Progress Bar Column"),
					  new CollectionItemType(typeof(GridViewCommandColumn), "Command Column"), 
				});
		}
		protected override void OnItemCreated(object item) {
			GridViewCommandColumn command = item as GridViewCommandColumn;
			if(command != null) {
				command.ShowSelectCheckbox = true;
				command.ClearFilterButton.Visible = true;
			}
			if(item.GetType() == typeof(GridViewDataBinaryImageColumn)) {
				Designer.AddHttpHandler(false);
			}
		}
		protected override void OnInsertItem(object item) {
			GridViewColumn column = item as GridViewColumn;
			if(ItemsListBox.SelectedIndex > -1) column.VisibleIndex = ItemsListBox.SelectedIndex;
			base.OnInsertItem(item);
		}
		protected override void MoveViewerItem(int oldIndex, int newIndex) {
			IList collection = GetParentViewerItemCollection();
			object item = collection[oldIndex];
			GridViewColumn column = item as GridViewColumn;
			if(column != null) column.VisibleIndex = newIndex;
			base.MoveViewerItem(oldIndex, newIndex);
		}
		protected override object CreateNewItem() {
			return new GridViewDataTextColumn();
		}
		ToolStripMenuItem convertMenuItem;
		protected override void CreateCustomMenuItems(List<ToolStripItem> buttons) {
			base.CreateCustomMenuItems(buttons);
			this.convertMenuItem = new ToolStripMenuItem("Change Column Type To");
			this.convertMenuItem.DropDownItems.AddRange(GetConvertDropDownItems());
			buttons.Add(this.convertMenuItem);
		}
		protected override void UpdateMenuStrip() {
			base.UpdateMenuStrip();
			this.convertMenuItem.Enabled = GetSelectedItems().Length > 0;
		}
		void OnChangeColumnType(object sender, EventArgs e) {
			object[] items = GetSelectedItems();
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			CollectionItemType itemType = (CollectionItemType)menuItem.Tag;
			ArrayList newItems = new ArrayList();
			foreach(GridViewColumn column in items) {
				GridViewColumn newColumn = (GridViewColumn)Activator.CreateInstance(itemType.Type);
				newColumn.Assign(column);
				OnItemCreated(newColumn);
				((IList)Collection).Insert(column.Index, newColumn);
				((IList)Collection).Remove(column);
				newItems.Add(newColumn);
			}
			AssignControls();
			if(newItems.Count == 0) return;
			for(int n = 0; n < ItemsListBox.Items.Count; n++) {
				if(ItemsListBox.Items[n] == newItems[0]) {
					ItemsListBox.ClearSelected();
					ItemsListBox.SelectedIndex = n;
					break;
				}
			}
			ComponentChanged(false);
		}
	}
}
