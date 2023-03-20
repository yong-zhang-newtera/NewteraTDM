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
using System.Text;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using System.Drawing;
using DevExpress.Web.ASPxClasses.Internal;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class GroupsEditorForm : EditorFormBase {
		GroupsEditorHelper fHelper;
		PivotGridWebGroup ActiveGroup { get { return fGroupsListBox.SelectedItem != null ? (PivotGridWebGroup)fGroupsListBox.SelectedItem : null; } }
		public GroupsEditorForm(object component,
			ITypeDescriptorContext context,
			IServiceProvider provider,
			object propertyValue)
			: base(component, context, provider, propertyValue) {
			this.MaximizeBox = false;
			fHelper = new GroupsEditorHelper((ASPxPivotGrid)component);
			this.ShowInTaskbar = false;
		}
		ListBox fGroupsListBox;
		ToolStrip fToolStrip;
		protected override void AddControlsToMainPanel(Panel mainPanel) {
			fGroupsListBox = fHelper.CreateListBox(mainPanel, DockStyle.Fill, Font, OnEditGroup);
			fToolStrip = fHelper.CreateToolStrip(mainPanel, GetToolStripButtons(), DesignUtils.GetToolStripRenderer(ServiceProvider));
		}
		protected override void AssignControls() {
			base.AssignControls();
			FillGroups();
			fHelper.SelectHead(fGroupsListBox);
		}
		ToolStripItem[] GetToolStripButtons() {
			return new ToolStripItem[] {
				fHelper.CreateToolStripButton(StringResources.ItemsEditor_AddItemButtonText, CreateBitmapFromResources(ItemsEditorFormBase.AddItemImageResource), OnAddItemButtonClick),
				fHelper.CreateToolStripButton(StringResources.ItemsEditor_RemoveItemButtonText, CreateBitmapFromResources(ItemsEditorFormBase.RemoveItemImageResource), OnRemoveItemButtonClick),
				fHelper.CreateToolStripSeparator(),
				fHelper.CreateToolStripButton(StringResources.ItemsEditor_RemoveAllItemsButtonText, CreateBitmapFromResources(ItemsEditorFormBase.RemoveAllItemImageResource), OnRemoveAllItemsButtonClick),
				fHelper.CreateToolStripSeparator(),
				fHelper.CreateToolStripButton(StringResources.DataEditing_CommandEdit, EditGroup)
			};
		}
		void OnAddItemButtonClick(object sender, EventArgs e) {
			fGroupsListBox.Items.Add(fHelper.Groups.Add());
			fHelper.SelectTail(fGroupsListBox);
			ComponentChanged(false);
		}
		void OnRemoveItemButtonClick(object sender, EventArgs e) {
			if(ActiveGroup == null)
				return;
			SelectionStore selection = new SelectionStore(fGroupsListBox);
			RemoveItem(fGroupsListBox, ActiveGroup);
			selection.RestoreSelection();
			ComponentChanged(false);
		}
		void OnRemoveAllItemsButtonClick(object sender, EventArgs e) {
			RemoveAllItems(fGroupsListBox);
			ComponentChanged(false);
		}
		void OnEditGroup(object sender, MouseEventArgs e) {
			if(fHelper.ValidMove(fGroupsListBox, e.Location))
				EditGroupCore();
		}
		void EditGroup(object sender, EventArgs e) {
			if(ActiveGroup == null)
				return;
			EditGroupCore();
		}
		void EditGroupCore() {
			GroupsManagerForm form = new GroupsManagerForm(Component, Context, ServiceProvider, PropertyValue, ActiveGroup, Location);
			form.ShowDialog();
			if(form.DialogResult == DialogResult.OK)
				ActiveGroup.Caption = form.NewCaption;
			SelectionStore selection = new SelectionStore(fGroupsListBox);
			FillGroups();
			selection.RestoreSelection();
		}
		void RemoveItem(ListBox listBox, PivotGridWebGroup group) {
			fHelper.Groups.Remove(group);
			listBox.Items.Remove(group);
		}
		void RemoveAllItems(ListBox listBox) {
			listBox.Items.Clear();
			fHelper.Groups.Clear();
		}
		protected override string GetPropertyStorePathPrefix() {
			return "GroupsEditorForm";
		}
		protected override void SaveUndoData() {
			fHelper.SaveUndoData();
		}
		protected override void Undo() {
			fHelper.Undo();
		}
		void FillGroups() {
			fGroupsListBox.BeginUpdate();
			fGroupsListBox.Items.Clear();
			foreach(PivotGridWebGroup group in fHelper.Groups)
				fGroupsListBox.Items.Add(group);
			fGroupsListBox.EndUpdate();
		}
		protected override Size FormDefaultSize { get { return new Size(210, 500); } }
		protected override Size FormMinimumSize { get { return new Size(200, 500); } }
		protected override string Caption { get { return GroupsEditorHelper.GroupsEditorFormCaption; } }
		class GroupsManagerForm : EditorFormBase {
			GroupsEditorHelper fHelper;
			const int FormWidth = 500;
			public string NewCaption { get { return fTextBox.Text; } }
			PivotGridWebGroup group;
			PivotGridWebGroup Group { get { return group; } }
			PivotGridField ActiveGroupedField { get { return fGroupedFieldsListBox.SelectedItem != null ? (PivotGridField)fGroupedFieldsListBox.SelectedItem : null; } }
			PivotGridField ActiveUngroupedField { get { return fUngroupedFieldsListBox.SelectedItem != null ? (PivotGridField)fUngroupedFieldsListBox.SelectedItem : null; } }
			public GroupsManagerForm(object component,
				ITypeDescriptorContext context,
				IServiceProvider provider,
				object propertyValue,
				PivotGridWebGroup group,
				Point location) : base(component, context, provider, propertyValue) {
				this.MaximumSize = new Size(FormWidth, 2 * FormWidth);
				Location = location;
				MaximizeBox = false;
				this.group = group;
				fHelper = new GroupsEditorHelper((ASPxPivotGrid)component);
			}
			protected override Size FormDefaultSize { get { return new Size(FormWidth, FormWidth); } }
			protected override Size FormMinimumSize { get { return new Size(FormWidth, FormWidth); } }
			protected override string GetPropertyStorePathPrefix() {
				return "GroupsManagerForm";
			}
			protected override string Caption { get { return GroupsEditorHelper.GroupsManagerCaption; } }
			ListBox fGroupedFieldsListBox;
			ListBox fUngroupedFieldsListBox;
			Button fButtonUp;
			Button fButtonDown;
			Button fButtonAdd;
			Button fButtonRemove;
			TextBox fTextBox;
			protected override void AddControlsToMainPanel(Panel mainPanel) {
				int captionY = NonRelatedControlsSpacing;
				int side = 24;
				int LabelHeight = 20;
				int ButtonPanelWidth = NonRelatedControlsSpacing + side + NonRelatedControlsSpacing;
				int CaptionPanelHeigth = captionY + side + NonRelatedControlsSpacing + LabelHeight + RelatedControlsSpacing;
				int ListBoxCaptionsY = CaptionPanelHeigth - LabelHeight - RelatedControlsSpacing;
				int PanelWidth = (mainPanel.Width - ButtonPanelWidth) / 2;
				Panel captionPanel = fHelper.CreatePanel(mainPanel, CaptionPanelHeigth, DockStyle.Top, true);
				Panel groupedPanel = fHelper.CreatePanel(mainPanel, PanelWidth, DockStyle.Left);
				Panel buttonPanel = fHelper.CreatePanel(mainPanel, ButtonPanelWidth, DockStyle.Left);
				Panel ungroupedPanel = fHelper.CreatePanel(mainPanel, PanelWidth, DockStyle.Fill);
				Label groupedLabel = fHelper.CreateLabel(captionPanel, "Grouped Fields", 2, ListBoxCaptionsY, LabelHeight);
				Label ungroupedLabel = fHelper.CreateLabel(captionPanel, "Ungrouped Fields", 2 + groupedPanel.Width + buttonPanel.Width, ListBoxCaptionsY, LabelHeight);
				Label label = fHelper.CreateLabel(captionPanel, "Caption:", 0, captionY, LabelHeight);
				fTextBox = fHelper.CreateTextBox(captionPanel, label.Width, captionY, mainPanel.Width - label.Width, side, Group.Caption);
				fGroupedFieldsListBox = fHelper.CreateListBox(groupedPanel, DockStyle.Fill, Font, OnMoveFieldToUngrouped);
				fUngroupedFieldsListBox = fHelper.CreateListBox(ungroupedPanel, DockStyle.Fill, Font, OnMoveFieldToGrouped);
				fButtonAdd = fHelper.CreateButton(buttonPanel, CreateBitmapFromResources(GroupsEditorHelper.MoveLeftImageResource), OnAddButtonClick, 25, side);
				fButtonRemove = fHelper.CreateButton(buttonPanel, CreateBitmapFromResources(GroupsEditorHelper.MoveRightImageResource), OnRemoveButtonClick, 55, side);
				fButtonUp = fHelper.CreateButton(buttonPanel, CreateBitmapFromResources(GroupsEditorHelper.MoveUpImageResource), OnMoveUpButtonClick, 100, side);
				fButtonDown = fHelper.CreateButton(buttonPanel, CreateBitmapFromResources(GroupsEditorHelper.MoveDownImageResource), OnMoveDownButtonClick, 130, side);
			}
			protected override void AssignControls() {
				base.AssignControls();
				FillGroupedFields(fGroupedFieldsListBox);
				fHelper.SelectHead(fGroupedFieldsListBox);
				FillUngroupedFields(fUngroupedFieldsListBox);
				fHelper.SelectHead(fUngroupedFieldsListBox);
				fGroupedFieldsListBox.SelectedIndexChanged += OnSelectedIndexChanged;
				UpdateButtons();
			}
			public override Bitmap CreateBitmapFromResources(string resourceName) {
				return CreateBitmapFromResources(resourceName, typeof(GroupsManagerForm).Assembly);
			}
			void OnSelectedIndexChanged(object sender, EventArgs e) {
				UpdateButtons();
			}
			void UpdateButtons() {
				fButtonUp.Enabled = true;
				fButtonDown.Enabled = true;
				fButtonAdd.Enabled = true;
				fButtonRemove.Enabled = true;
				if(fUngroupedFieldsListBox.SelectedIndex == -1)
					fButtonAdd.Enabled = false;
				if(fGroupedFieldsListBox.SelectedIndex == -1) {
					fButtonUp.Enabled = false;
					fButtonDown.Enabled = false;
					fButtonRemove.Enabled = false;
					return;
				}
				if(fGroupedFieldsListBox.SelectedIndex == 0)
					fButtonUp.Enabled = false;
				if(fGroupedFieldsListBox.SelectedIndex == fGroupedFieldsListBox.Items.Count - 1)
					fButtonDown.Enabled = false;
			}
			void OnMoveUpButtonClick(object sender, EventArgs e) {
				ChangeFieldOrder(-1);
			}
			void OnMoveDownButtonClick(object sender, EventArgs e) {
				ChangeFieldOrder(1);
			}
			void ChangeFieldOrder(int d) {
				if(ActiveGroupedField == null)
					return;
				int newIndex = fGroupedFieldsListBox.SelectedIndex + d;
				Group.ChangeFieldIndex(ActiveGroupedField, newIndex);
				Swap(fGroupedFieldsListBox, fGroupedFieldsListBox.SelectedIndex, newIndex);
				fGroupedFieldsListBox.SelectedIndex = newIndex;
				ComponentChanged(false);
			}
			void Swap(ListBox listBox, int first, int second) {
				object temp = listBox.Items[first];
				listBox.Items[first] = listBox.Items[second];
				listBox.Items[second] = temp;
			}
			void OnAddButtonClick(object sender, EventArgs e) {
				if(ActiveUngroupedField == null)
					return;
				MoveFieldToGroupedFields(ActiveUngroupedField);
			}
			void OnMoveFieldToUngrouped(object sender, MouseEventArgs e) {
				if(fHelper.ValidMove(fGroupedFieldsListBox, e.Location))
					MoveFieldToUngroupedFields(ActiveGroupedField);
			}
			void OnMoveFieldToGrouped(object sender, MouseEventArgs e) {
				if(fHelper.ValidMove(fUngroupedFieldsListBox, e.Location))
					MoveFieldToGroupedFields(ActiveUngroupedField);
			}
			void MoveFieldToGroupedFields(PivotGridField field) {
				SelectionStore selection = new SelectionStore(fUngroupedFieldsListBox);
				AddFieldToGroup(field);
				fHelper.SelectTail(fGroupedFieldsListBox);
				selection.RestoreSelection();
				UpdateButtons();
				ComponentChanged(false);
			}
			void AddFieldToGroup(PivotGridField field) {
				Group.Add(field);
				fGroupedFieldsListBox.Items.Add(field);
				fUngroupedFieldsListBox.Items.Remove(field);
			}
			void OnRemoveButtonClick(object sender, EventArgs e) {
				if(ActiveGroupedField == null)
					return;
				MoveFieldToUngroupedFields(ActiveGroupedField);
			}
			void MoveFieldToUngroupedFields(PivotGridField field) {
				SelectionStore selection = new SelectionStore(fGroupedFieldsListBox);
				RemoveField(field);
				fHelper.SelectTail(fUngroupedFieldsListBox);
				selection.RestoreSelection();
				UpdateButtons();
				ComponentChanged(false);
			}
			void RemoveField(PivotGridField field) {
				Group.Remove(field);
				fGroupedFieldsListBox.Items.Remove(field);
				fUngroupedFieldsListBox.Items.Add(field);
			}
			void FillGroupedFields(ListBox listBox) {
				listBox.BeginUpdate();
				listBox.Items.Clear();
				foreach(PivotGridField field in Group)
					listBox.Items.Add(field);
				listBox.EndUpdate();
				fHelper.SelectHead(listBox);
			}
			void FillUngroupedFields(ListBox listBox) {
				listBox.BeginUpdate();
				listBox.Items.Clear();
				foreach(PivotGridField field in fHelper.Data.Fields) {
					if(field.Group == null)
						listBox.Items.Add(field);
				}
				listBox.EndUpdate();
				fHelper.SelectHead(listBox);
			}
			protected override void SaveUndoData() {
				fHelper.SaveUndoData();
			}
			protected override void Undo() {
				fHelper.Undo();
			}
		}
		class SelectionStore {
			ListBox fListBox;
			int fSelected;
			public SelectionStore(ListBox listBox) {
				fListBox = listBox;
				SaveSelection();
			}
			public void SaveSelection() {
				fSelected = fListBox.SelectedIndex;
			}
			public void RestoreSelection() {
				fListBox.SelectedIndex = Math.Min(fSelected, fListBox.Items.Count - 1);
			}
		}
	}
	public class GroupsEditorHelper {
		public const string MoveLeftImageResource = "DevExpress.Web.ASPxPivotGrid.Design.Images.MoveLeft.png";
		public const string MoveRightImageResource = "DevExpress.Web.ASPxPivotGrid.Design.Images.MoveRight.png";
		public const string MoveUpImageResource = "DevExpress.Web.ASPxPivotGrid.Design.Images.MoveUp.png";
		public const string MoveDownImageResource = "DevExpress.Web.ASPxPivotGrid.Design.Images.MoveDown.png";
		public const string GroupsEditorFormCaption = "Groups Editor";
		public const string GroupsManagerCaption = "Group's Fields Editor";
		ASPxPivotGrid pivotGrid;
		public ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		public PivotGridWebData Data { get { return PivotGrid.Data; } }
		public PivotGridGroupCollection Groups { get { return Data.Groups; } }
		public GroupsEditorHelper(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		public ToolStripButton CreateToolStripButton(string toolTipText, Bitmap image, EventHandler onClick) {
			image.MakeTransparent(Color.Magenta);
			ToolStripButton button = new ToolStripButton(toolTipText, image, onClick);
			button.DisplayStyle = ToolStripItemDisplayStyle.Image;
			button.AutoToolTip = true;
			button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			return button;
		}
		internal ToolStripItem CreateToolStripButton(string text, EventHandler onClick) {
			ToolStripButton button = new ToolStripButton(text, null, onClick);
			button.DisplayStyle = ToolStripItemDisplayStyle.Text;
			button.AutoToolTip = true;
			return button;
		}
		public Button CreateButton(Control parent, Bitmap image, EventHandler onClick, int y, int side) {
			image.MakeTransparent(Color.Magenta);
			Button button = new Button();
			button.Parent = parent;
			button.Image = image;
			button.Click += onClick;
			int offset = (parent.Width - side) / 2;
			button.Bounds = new Rectangle(offset, y, side, side);
			button.Size = new Size(side, side);
			return button;
		}
		public ToolStrip CreateToolStrip(Control parent, ToolStripItem[] items, ToolStripRenderer renderer) {
			ToolStrip toolStrip = new ToolStrip();
			toolStrip.Parent = parent;
			toolStrip.Renderer = renderer;
			toolStrip.CanOverflow = false;
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			toolStrip.AutoSize = false;
			toolStrip.Dock = DockStyle.Top;
			toolStrip.Items.AddRange(items);
			return toolStrip;
		}
		public ToolStripItem CreateToolStripSeparator() {
			return new ToolStripSeparator();
		}
		public ListBox CreateListBox(Control parent, DockStyle dockStyle, Font font, MouseEventHandler onDoubleClick) {
			ListBox listBox = new ListBoxItemPainter(true).CreateListBox(font);
			listBox.Dock = dockStyle;
			listBox.Parent = parent;
			listBox.MouseDoubleClick += onDoubleClick;
			return listBox;
		}
		public Panel CreatePanel(Control parent, int width, DockStyle dockStyle) {
			return CreatePanel(parent, width, dockStyle, false);
		}
		public Panel CreatePanel(Control parent, int side, DockStyle dockStyle, bool setHeight) {
			Panel panel = new Panel();
			panel.Dock = dockStyle;
			panel.Parent = parent;
			if(setHeight)
				panel.Height = side;
			else
				panel.Width = side;
			panel.BringToFront();
			return panel;
		}
		string fUndoString;
		public void SaveUndoData() {
			fUndoString = PivotGrid.SerializeSnapshot(PivotGrid.GetSnapshot());
		}
		public void Undo() {
			if(!String.IsNullOrEmpty(fUndoString))
				PivotGrid.DeserializeSnapshot(fUndoString);
		}
		public Label CreateLabel(Control parent, string text, int x, int y, int height) {
			Label label = new Label();
			label.Parent = parent;
			label.Text = text;
			label.TextAlign = ContentAlignment.MiddleLeft;
			int width = FitWidth(label);
			label.Bounds = new Rectangle(x, y, width, height);
			return label;
		}
		int FitWidth(Label label) {
			label.AutoSize = true;
			int width = label.Width;
			label.AutoSize = false;
			return width;
		}
		public TextBox CreateTextBox(Control parent, int x, int y, int width, int height, string text) {
			TextBox textBox = new TextBox();
			textBox.Parent = parent;
			textBox.Text = text;
			textBox.Bounds = new Rectangle(x, y, width, height);
			return textBox;
		}
		public bool ValidMove(ListBox listBox, Point location) {
			if(listBox.SelectedIndex == -1)
				return false;
			Rectangle itemRectangle = listBox.GetItemRectangle(listBox.SelectedIndex);
			if(!itemRectangle.Contains(location))
				return false;
			return true;
		}
		public void SelectTail(ListBox listBox) {
			listBox.SelectedIndex = listBox.Items.Count - 1;
		}
		public void SelectHead(ListBox listBox) {
			if(listBox.Items.Count > 0)
				listBox.SelectedIndex = 0;
		}
	}
	public class GroupsEditor : TypeEditorBase {
		public override EditorFormBase CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new GroupsEditorForm(component, context, provider, propertyValue);
		}
	}
}
