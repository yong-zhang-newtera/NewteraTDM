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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid.Frames {
	[ToolboxItem(false)]
	public class GroupsDesigner : DevExpress.Utils.Frames.XtraFrame {
		Panel pnlButtonTop;
		Panel pnlGroup;
		Panel pnlFields;
		SimpleButton btnAddGroup;
		SimpleButton btnRemoveGroup;
		SplitterControl splitter;
		ListBoxControl lboxGroups;
		TextEdit edGroupCaption;
		GroupControl pnlGroupedFields;
		GroupControl pnlUngroupedFields;
		Panel pnlGroupedFieldsButtons;
		ListBoxControl lboxGroupedFields;
		ListBoxControl lboxUngroupedFields;
		SimpleButton btnAddField;
		SimpleButton btnRemoveField;
		SimpleButton btnFieldUp;
		SimpleButton btnFieldDown;
		public GroupsDesigner()	{
			CreateButtonTopPanel();
			CreateGroupPanel();
			CreateFieldsPanel();
			pnlGroup.Dock = DockStyle.Fill;
			pnlGroup.BringToFront();
		}
		public PivotGridControl PivotGrid { get { return EditingObject as PivotGridControl; } }
		public PivotGridFieldCollection Fields { get { return PivotGrid.Fields; } }
		public PivotGridGroupCollection Groups { get { return PivotGrid.Groups; } }
		protected PivotGridGroup ActiveGroup { get {	return lboxGroups.SelectedItem != null ? lboxGroups.SelectedItem as PivotGridGroup : null; } }
		protected override void Dispose(bool disposing){
			if(disposing){
			}
			base.Dispose( disposing );
		}
		public override void  DoInitFrame() {
 			base.DoInitFrame();
			FillGroupList();
			UpdateGroupControls();
			FillUngroupedFields();
			UpdateGroupedControls();
		}
		protected override string DescriptionText { 
			get { return "You can add and delete PivotGrid groups and modify their settings."; }
		}
		protected void CreateButtonTopPanel() {
			pnlButtonTop = new Panel();
			pnlButtonTop.Dock = DockStyle.Top;
			pnlButtonTop.Height = 30;
			pnlButtonTop.Parent = pnlMain;
			pnlButtonTop.BringToFront();
			btnAddGroup = CreateTopButton("Add Group", 4, 6);
			btnAddGroup.Click += new EventHandler(OnAddGroup);
			btnRemoveGroup = CreateTopButton("Remove Group", 130, 7);
			btnRemoveGroup.Click += new EventHandler(OnRemoveGroup);
		}
		protected SimpleButton CreateTopButton(string caption, int x, int imageIndex) {
			SimpleButton button = new SimpleButton();
			button.Parent = pnlButtonTop;
			button.Bounds = new Rectangle(x, 4, 120, 24);
			button.Text = caption;
			button.Image = DesignerImages16.Images[imageIndex];
			return button;
		}
		protected void CreateGroupPanel() {
			pnlGroup = new Panel();
			pnlGroup.Width = 150;
			pnlGroup.Dock = DockStyle.Left;
			pnlGroup.Parent = pnlMain;
			pnlGroup.BringToFront();
			Label lblGroupCaption = new Label();
			lblGroupCaption.Text = "Caption: ";
			lblGroupCaption.Location = new Point(2, 4);
			lblGroupCaption.Parent = pnlGroup;
			lblGroupCaption.AutoSize = true;
			edGroupCaption = new TextEdit();
			edGroupCaption.Parent = pnlGroup;
			edGroupCaption.Bounds = new Rectangle(lblGroupCaption.Right + 2, 2, pnlGroup.Width - lblGroupCaption.Right - 4, 20);
			edGroupCaption.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			edGroupCaption.EditValueChanged += new EventHandler(OnGroupCaptionChanged);
			lboxGroups = new ListBoxControl();
			lboxGroups.Parent = pnlGroup;
			lboxGroups.Bounds = new Rectangle(2, 4 + edGroupCaption.Bottom, pnlGroup.Width - 4, pnlGroup.Height - edGroupCaption.Bottom - 4);
			lboxGroups.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
		}
		protected void CreateFieldsPanel() {
			const int panelFieldsWidth = 120;
			const int panelButtonFieldsWidth = 30;
			pnlFields = new Panel();
			pnlFields.Width = panelFieldsWidth * 2 + panelButtonFieldsWidth;
			pnlFields.Dock = DockStyle.Right;
			pnlFields.Parent = pnlMain;
			pnlFields.BringToFront();
			splitter = new SplitterControl();
			splitter.Parent = pnlMain;
			splitter.Dock = DockStyle.Right;
			splitter.BringToFront();
			pnlGroupedFields = new GroupControl();
			pnlGroupedFields.Width = panelFieldsWidth;
			pnlGroupedFields.Dock = DockStyle.Left;
			pnlGroupedFields.DockPadding.All = 2;
			pnlGroupedFields.Text = "Grouped fields";
			pnlGroupedFields.Parent = pnlFields;
			pnlGroupedFields.BringToFront();
			pnlGroupedFieldsButtons = new Panel();
			pnlGroupedFieldsButtons.Width = panelButtonFieldsWidth;
			pnlGroupedFieldsButtons.Dock = DockStyle.Left;
			pnlGroupedFieldsButtons.Parent = pnlFields;
			pnlGroupedFieldsButtons.BringToFront();
			pnlUngroupedFields = new GroupControl();
			pnlUngroupedFields.Dock = DockStyle.Fill;
			pnlUngroupedFields.DockPadding.All = 2;
			pnlUngroupedFields.Parent = pnlFields;
			pnlUngroupedFields.Text = "Ungrouped fields";
			pnlUngroupedFields.BringToFront();
			lboxGroupedFields = CreateGroupedFieldsListBox(pnlGroupedFields, new EventHandler(OnRemoveField));
			lboxUngroupedFields = CreateGroupedFieldsListBox(pnlUngroupedFields, new EventHandler(OnAddField));
			btnAddField = CreateFieldButton(25, 10, "Add field", new EventHandler(OnAddField));
			btnRemoveField = CreateFieldButton(55, 11, "Remove field", new EventHandler(OnRemoveField));
			btnFieldUp = CreateFieldButton(100, 12, "Up", new EventHandler(OnFieldUp));
			btnFieldDown = CreateFieldButton(130, 13, "Down", new EventHandler(OnFieldDown));
   		}
		protected ListBoxControl CreateGroupedFieldsListBox(Control parent, EventHandler clickHandler) {
			ListBoxControl listBoxControl = new ListBoxControl();
			listBoxControl.Dock = DockStyle.Fill;
			listBoxControl.Parent = parent;
			listBoxControl.DoubleClick += clickHandler;
			return listBoxControl;
		}
		protected SimpleButton CreateFieldButton(int y, int imageIndex, string toolTip, EventHandler clickHandler) {
			const int size = 24;
			SimpleButton button = new SimpleButton();
			button.Parent = pnlGroupedFieldsButtons;
			int offsetX = (pnlGroupedFieldsButtons.Width - size) / 2;
			button.Bounds = new Rectangle(offsetX, y, size, size);
			button.Image = DesignerImages16.Images[imageIndex];
			button.ToolTip = toolTip;
			button.Click += clickHandler;
			return button;
		}
		protected void SetSelectedIndex(ListBoxControl listBox, int oldSelectedIndex) {
			if(oldSelectedIndex >= listBox.Items.Count)
				oldSelectedIndex = listBox.Items.Count - 1;
			if(oldSelectedIndex > -1) {
				listBox.SelectedIndex = oldSelectedIndex;
			} else listBox.SelectedIndex = 0;
		}
		protected void FillGroupList() {
			lboxGroups.SelectedIndexChanged -= new EventHandler(GroupsSelectedIndexChanged);
			int oldSelectedIndex = lboxGroups.SelectedIndex;
			lboxGroups.Items.Clear();
			for(int i = 0; i < Groups.Count; i ++) {
				lboxGroups.Items.Add(Groups[i]);
			}
			SetSelectedIndex(lboxGroups, oldSelectedIndex);
			UpdateGroupControls();
			lboxGroups.SelectedIndexChanged += new EventHandler(GroupsSelectedIndexChanged);
		}
		protected void UpdateGroupControls() {
			edGroupCaption.Enabled = ActiveGroup != null;
			btnRemoveGroup.Enabled = ActiveGroup != null;
			edGroupCaption.Text = ActiveGroup != null ? ActiveGroup.Caption : string.Empty;
			FillGroupedFields();
		}
		protected void FillUngroupedFields() {
			int oldSelectedIndex = lboxUngroupedFields.SelectedIndex;
			lboxUngroupedFields.SelectedIndexChanged -= new EventHandler(OnUngroupedFieldsSelectedIndexChanged);
			lboxUngroupedFields.Items.Clear();
			for(int i = 0; i < Fields.Count; i ++) {
				if(Fields[i].Group == null) {
					lboxUngroupedFields.Items.Add(Fields[i]);
				}
			}
			SetSelectedIndex(lboxUngroupedFields, oldSelectedIndex);
			UpdateUngroupedControls();
			lboxUngroupedFields.SelectedIndexChanged += new EventHandler(OnUngroupedFieldsSelectedIndexChanged);
		}
		protected void FillGroupedFields() {
			lboxGroupedFields.SelectedIndexChanged -= new EventHandler(OnGroupedFieldsSelectedIndexChanged);
			int oldSelectedIndex = lboxGroupedFields.SelectedIndex;
			lboxGroupedFields.Items.Clear();
			if(ActiveGroup == null) return;
			for(int i = 0; i < ActiveGroup.Count; i ++) {
				lboxGroupedFields.Items.Add(ActiveGroup[i]);
			}
			SetSelectedIndex(lboxGroupedFields, oldSelectedIndex);
			UpdateGroupedControls();
			lboxGroupedFields.SelectedIndexChanged += new EventHandler(OnGroupedFieldsSelectedIndexChanged);
		}
		void GroupsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateGroupControls();
		}
		void OnGroupCaptionChanged(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			ActiveGroup.Caption = edGroupCaption.Text;
			lboxGroups.Refresh();
		}
		void UpdateUngroupedControls() {
			btnAddField.Enabled = lboxUngroupedFields.SelectedItem != null;
			if(lboxGroups.SelectedItem == null) btnAddField.Enabled = false;
		}
		void OnUngroupedFieldsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateUngroupedControls();
		}
		void UpdateGroupedControls() {
			btnRemoveField.Enabled = lboxGroupedFields.SelectedItem != null;
			btnFieldUp.Enabled = btnRemoveField.Enabled && lboxGroupedFields.SelectedIndex > 0;
			btnFieldDown.Enabled = btnRemoveField.Enabled && lboxGroupedFields.SelectedIndex < lboxGroupedFields.ItemCount - 1;
			if(lboxGroups.SelectedItem == null) 
				btnRemoveField.Enabled = btnFieldUp.Enabled = btnFieldDown.Enabled = false;
		}
		void OnGroupedFieldsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateGroupedControls();
		}
		void OnAddGroup(object sender, EventArgs e) {
			Groups.Add();
			FillGroupList();
			lboxGroups.SelectedIndex = Groups.Count - 1;
			UpdateUngroupedControls();
		}
		void OnRemoveGroup(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			Groups.Remove(ActiveGroup);
			FillGroupList();
			FillUngroupedFields();
			UpdateGroupedControls();
		}
		void OnAddField(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxUngroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			ActiveGroup.Add(field);
			FillUngroupedFields();
			FillGroupedFields();
			lboxGroups.Refresh();
		}
		void OnRemoveField(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxGroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			ActiveGroup.Remove(field);
			FillUngroupedFields();
			FillGroupedFields();
			lboxGroups.Refresh();
		}
		void OnFieldUp(object sender, EventArgs e) {
			ChangeFieldIndex(-1);
		}
		void OnFieldDown(object sender, EventArgs e) {
			ChangeFieldIndex(1);
		}
		void ChangeFieldIndex(int dx) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxGroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			int newIndex = lboxGroupedFields.SelectedIndex + dx;
			ActiveGroup.ChangeFieldIndex(field, newIndex);
			FillGroupedFields();
			lboxGroupedFields.SelectedIndex = newIndex;
		}
	}
}
