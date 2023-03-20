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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Customization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPivotGrid.Localization;
using System.Text;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.Customization {
	[ToolboxItem(false)]
	public class CustomizationForm : CustomizationFormBase {
		PivotGridViewInfoData data;
		Panel pnlTop;
		Panel pnlBottom;
		ComboBoxEdit cbArea;
		SimpleButton button;
		internal Panel TopPanel { get { return pnlTop; } }
		internal Panel BottomPanel { get { return pnlBottom; } }
		internal ComboBoxEdit AreaComboBox { get { return cbArea; } }
		internal FieldCustomizationTreeBox PivotListBox { get { return (FieldCustomizationTreeBox)ActiveListBox; } }
		public CustomizationForm(PivotGridViewInfoData data) : base() {
			this.data = data;
		}
		public virtual new FieldCustomizationTreeBox ActiveListBox { get { return (FieldCustomizationTreeBox)base.ActiveListBox; } }
		public override Control ControlOwner { get { return Data.ControlOwner; } }
		protected override void InitCustomizationForm() {
			CreateTopPanel();
			CreateBottomPanel();
			base.InitCustomizationForm();
			this.DockPadding.All = 5;
			UpdateBounds();
		}
		void CreateTopPanel() {
			this.pnlTop = CreatePanel(DockStyle.Top, 20);
			Label label = new Label();
			label.Parent = this.pnlTop;
			label.Dock = DockStyle.Fill;
			label.BackColor = Color.Transparent;
			label.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormText);
		}
		void CreateBottomPanel() {
			this.pnlBottom = CreatePanel(DockStyle.Bottom, 30);
			button = new SimpleButton();
			button.Parent = this.pnlBottom;
			button.Click += new EventHandler(OnButtonClick);
			button.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormAddTo);
			button.Size = button.CalcBestSize();
			button.Left = 0;
			pnlBottom.Height = button.Height + 6;
			button.Top = (pnlBottom.Height - button.Height) / 2;
			cbArea = new ComboBoxEdit();
			cbArea.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			cbArea.Parent = this.pnlBottom;
			cbArea.Top = (pnlBottom.Height - cbArea.Height) / 2;
			cbArea.Left = button.Right + 3;
			cbArea.Width = pnlBottom.ClientRectangle.Right - cbArea.Left;
			cbArea.Anchor = AnchorStyles.Left | AnchorStyles.Right;			
		}		
		Panel CreatePanel(DockStyle style, int height) {
			Panel panel = new Panel();
			panel.Parent = this;
			panel.Dock = style;
			panel.Height = height;
			panel.BackColor = Color.Transparent;
			panel.BorderStyle = BorderStyle.None;
			return panel;
		}
		public PivotGridViewInfoData Data { get { return data; } }
		public PivotGridField SelectedField {
			get {
				if(ActiveListBox == null || ActiveListBox.SelectedItem == null)
					return null;
				return ActiveListBox.SelectedItem.Field;
			}
		}
		public void Populate() {
			if(ActiveListBox == null)
				return;
			ActiveListBox.SelectedIndexChanged -= new EventHandler(OnListBoxSelectedChanged);
			PivotListBox.Populate();
			UpdateComboAndButton();
			ActiveListBox.SelectedIndexChanged += new EventHandler(OnListBoxSelectedChanged);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ActiveListBox.BringToFront();
			ActiveListBox.SelectedIndexChanged += new EventHandler(OnListBoxSelectedChanged);
			UpdateComboAndButton();
		}		
		protected override CustomCustomizationListBox CreateCustomizationListBox() {
			return new FieldCustomizationTreeBox(this);
		}
		protected override string FormCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormCaption); } } 
		protected override void OnFormClosed() {
			if(Data == null)  return;
			PivotGridViewInfoData oldData = Data;
			this.data = null;
			oldData.DestroyCustomization();
		}
		protected override Rectangle CustomizationFormBounds { get { return Data.CustomizationFormBounds; } }
		protected override UserLookAndFeel ControlOwnerLookAndFeel { get { return Data.ActiveLookAndFeel; } }
		protected virtual void OnListBoxSelectedChanged(object sender, EventArgs e) {
			UpdateComboAndButton();
		}
		protected virtual void UpdateComboAndButton() {
			if(ActiveListBox.SelectedItem == null && ActiveListBox.ItemCount > 0)
				ActiveListBox.SelectedIndex = 0;
			this.button.Enabled = this.cbArea.Enabled = SelectedField != null;
			PopulateCombo(SelectedField);
			if(SelectedField != null)
				this.cbArea.SelectedItem = AreaToStr(GetSelectedArea(SelectedField));			
		}
		protected virtual void PopulateCombo(PivotGridFieldBase field) {
			this.cbArea.Properties.Items.Clear();
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(field == null || field.IsAreaAllowed(area))
					this.cbArea.Properties.Items.Add(AreaToStr(area));
			}		
		}
		protected virtual PivotArea StrToArea(string localizedString) {
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(AreaToStr(area) == localizedString)
					return area;
			}
			throw new ArgumentException("localizedString");
		}
		protected virtual string AreaToStr(PivotArea area) {
			return GetLocalizedString(GetStringId(area));
		}
		protected virtual PivotGridStringId GetStringId(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea: return PivotGridStringId.ColumnArea;
				case PivotArea.RowArea: return PivotGridStringId.RowArea;
				case PivotArea.FilterArea: return PivotGridStringId.FilterArea;
				case PivotArea.DataArea: return PivotGridStringId.DataArea;
			}
			throw new ArgumentException("area");
		}
		protected internal string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		protected virtual PivotArea GetSelectedArea(PivotGridFieldBase field) {
			PivotArea area;
			if(this.cbArea.SelectedItem != null){
				area = StrToArea((string)this.cbArea.SelectedItem);
				if(field.IsAreaAllowed(area))
					return area;
			}
			if(field.IsAreaAllowed(field.Area))
				return field.Area;
			if(field.IsAreaAllowed(PivotArea.FilterArea))
				return PivotArea.FilterArea;
			if(field.IsAreaAllowed(PivotArea.ColumnArea))
				return PivotArea.ColumnArea;
			if(field.IsAreaAllowed(PivotArea.RowArea))
				return PivotArea.RowArea;
			return PivotArea.DataArea;
		}
		protected virtual void OnButtonClick(object sender, EventArgs e) {
			MoveFieldToPivotGrid(PivotListBox.SelectedItem.Field);
		}
		public void MoveFieldToPivotGrid(PivotGridField field) {
			if(field == null) return;
			int oldSelectedIndex = PivotListBox.SelectedIndex;
			PivotArea area = GetSelectedArea(field);
			field.SetAreaPosition(area, Data.Fields.GetVisibleFieldCount(area));
			PivotListBox.Populate();
		}
	}
	public enum PivotCustomizationTreeNodeType {
		Unknown = -1,
		Measure = 0,
		Dimension = 1,
		Attribute = 2,
		Hierarchy = 3,
	}
	public class PivotCustomizationTreeNode {
		PivotGridField field;
		string[] fullName;
		string name;
		string parentName;
		bool expanded;
		string caption;
		PivotCustomizationTreeNodeType type;
		int visibleIndex = -1;
		PivotCustomizationTreeNodeViewInfo nodeViewInfo;
		public string[] FullName { get { return fullName; } }
		public string Caption { get { return caption; } }
		public string Name { get { return name; } }
		public string ParentName { get { return parentName; } }
		public bool IsExpanded { get { return expanded; } set { expanded = value; } }
		public bool CanExpand { get { return Type == PivotCustomizationTreeNodeType.Dimension || (Type == PivotCustomizationTreeNodeType.Measure && Level == 0); } }
		public int Level { get { return fullName.Length - 1; } }
		PivotGridGroup Group { get { return (Field != null) ? Field.Group : null; } }
		bool HasGroup { get { return Field != null && Field.Group != null; } }
		public PivotCustomizationTreeNodeType Type { get { return type; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public bool IsVisible { get { return visibleIndex > -1; } }
		public PivotCustomizationTreeNodeViewInfo ViewInfo { get { return nodeViewInfo; } set { nodeViewInfo = value; } }
		public PivotGridField Field { get { return field; } }
		public PivotCustomizationTreeNode(PivotGridField field) {
			this.field = field;
			InitializeNames(field.FieldName);
			this.caption = HasGroup ? Group.Caption : Field.Caption;
		}
		public PivotCustomizationTreeNode(string name) {
			InitializeNames(name);
			this.caption = Clean(this.fullName[Level]);
		}
		public PivotCustomizationTreeNode(string name, string caption) {
			InitializeNames(name);
			if(string.IsNullOrEmpty(caption))
				this.caption = Clean(this.fullName[Level]);
			else
				this.caption = caption;
		}
		protected virtual void InitializeNames(string name) {
			this.fullName = name.Split('.');
			this.type = DefineNodeType(this.fullName);
			this.fullName = CorrectFullName(this.fullName);
			this.name = GetCompleteName(this.fullName, this.fullName.Length - 1);
			this.parentName = GetCompleteName(this.fullName, this.fullName.Length - 2);
		}
		protected virtual string Clean(string str) {
			if(str.IndexOf('[') == 0 && str.LastIndexOf(']') > 1)
				return str.Substring(1, str.Length - 2);
			return null;
		}
		protected virtual PivotCustomizationTreeNodeType DefineNodeType(string[] AllNames) {
			if(fullName[0] == "[Measures]")
				return PivotCustomizationTreeNodeType.Measure;
			else if(fullName.Length == 1)
				return PivotCustomizationTreeNodeType.Dimension;
			else if(HasGroup)
				return PivotCustomizationTreeNodeType.Hierarchy;
			else if(Field != null)
				return PivotCustomizationTreeNodeType.Attribute;
			return PivotCustomizationTreeNodeType.Unknown;
		}
		protected virtual string[] CorrectFullName(string[] FullName) {
			List<string> list = new List<string>(FullName);
			if(list.Count >= 2)
				list = list.GetRange(0, 2);
			return list.ToArray();
		}
		protected virtual string GetCompleteName(string[] fullNameParts, int includeIndex) {
			if(includeIndex < 0 || includeIndex >= fullNameParts.Length)
				return null;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < includeIndex; i++)
				sb.Append(fullNameParts[i]).Append('.');
			sb.Append(fullNameParts[includeIndex]);
			return sb.ToString();
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			if(Field != null)
				return Field.ToString();
			return "";
		}
	}
	public class PivotCustomizationFieldsTree : IEnumerable {
		List<PivotCustomizationTreeNode> oldTree;
		List<PivotCustomizationTreeNode> tree;
		PivotGridViewInfoData data;
		protected PivotGridViewInfoData Data { get { return data; } }
		protected List<PivotCustomizationTreeNode> Tree { get { return tree; } }
		public PivotCustomizationFieldsTree() { }
		public PivotCustomizationFieldsTree(PivotGridViewInfoData data) {
			this.data = data;
		}
		protected virtual bool IsVisible(List<PivotCustomizationTreeNode> tree, int index) {
			if(tree[index].Level == 0)
				return true;
			int parentIndex = SearchParentPlace(tree, index);
			if(parentIndex == -1)
				return true;
			return IsVisible(tree, parentIndex) && tree[parentIndex].IsExpanded;
		}
		public int Count { get { return Tree.Count; } }
		public PivotCustomizationTreeNode this[int i] { get { return Tree[i]; } }
		public void Update(PivotGridFieldCollection fields, bool isGrouping) {
			this.oldTree = Tree;
			this.tree = PopulateTree(fields, isGrouping);
			LoadExpandedState();
			SetVisibleIndices(Tree, isGrouping);
		}
		protected virtual void LoadExpandedState() {
			if(oldTree == null)
				return;
			foreach(PivotCustomizationTreeNode oldNode in oldTree) {
				PivotCustomizationTreeNode newNode = Search(tree, oldNode.Name);
				if(newNode != null)
					newNode.IsExpanded = oldNode.IsExpanded;
			}
		}
		protected virtual List<PivotCustomizationTreeNode> PopulateTree(PivotGridFieldCollection fields, bool isGrouping) {
			List<PivotCustomizationTreeNode> tree = PopulateFields(fields);
			tree.Sort(new TreeNodeComparer(isGrouping));
			if(isGrouping)
				CreateHierarchy(tree);
			return tree;
		}
		protected virtual void SetVisibleIndices(List<PivotCustomizationTreeNode> tree, bool isGrouping) {
			for(int i = 0, nextIndex = 0; i < tree.Count; i++)
				if(IsVisible(tree, i) || !isGrouping) {
					tree[i].VisibleIndex = nextIndex;
					nextIndex++;
				}
		}
		protected virtual void CreateHierarchy(List<PivotCustomizationTreeNode> tree) {
			for(int i = 0; i < tree.Count; i++) {
				if(tree[i].Level == 0) continue;
				int place = SearchParentPlace(tree, i);
				if(place == -1 || tree[place].Name != tree[i].ParentName) {
					tree.Insert(place + 1, new PivotCustomizationTreeNode(tree[i].ParentName, Data != null ? Data.GetHierarchyCaption(tree[i].ParentName) : null));
					i++;
				}
			}
		}
		protected virtual PivotCustomizationTreeNode Search(List<PivotCustomizationTreeNode> tree, string name) {
			foreach(PivotCustomizationTreeNode node in tree) {
				if(node.Name == name)
					return node;
			}
			return null;
		}
		protected virtual int GetIndex(PivotCustomizationTreeNode node) {
			for(int i = 0; i < Count; i++)
				if(Tree[i] == node)
					return i;
			return -1;
		}
		public virtual int SearchParentPlace(PivotCustomizationTreeNode node) {
			int index = GetIndex(node);
			int nodeIndex = SearchParentPlace(Tree, index);
			if(nodeIndex == -1)
				return nodeIndex;
			return Tree[nodeIndex].VisibleIndex;
		}
		protected virtual int SearchParentPlace(List<PivotCustomizationTreeNode> tree, int childIndex) {
			if(tree[childIndex].Level == 0)
				return -1;
			int prev = SearchPrev(tree, childIndex);
			return prev;
		}
		protected virtual int SearchPrev(List<PivotCustomizationTreeNode> tree, int index) {
			PivotCustomizationTreeNode node = tree[index];
			for(int i = index - 1; i >= 0; i--) {
				if(tree[i].Level < node.Level || tree[i].ParentName != node.ParentName)
					return i;
			}
			return -1;
		}
		protected virtual List<PivotCustomizationTreeNode> PopulateFields(PivotGridFieldCollection fields) {
			List<PivotCustomizationTreeNode> tree = new List<PivotCustomizationTreeNode>();
			foreach(PivotGridField field in fields)
				if(field.CanShowInCustomizationForm)
					tree.Add(new PivotCustomizationTreeNode(field));
			return tree;
		}
		public IEnumerator GetEnumerator() {
			return Tree.GetEnumerator();
		}
	}
	internal class TreeNodeComparer : IComparer<PivotCustomizationTreeNode> {
		bool isGrouping;
		public TreeNodeComparer(bool isGrouping) {
			this.isGrouping = isGrouping;
		}
		public int Compare(PivotCustomizationTreeNode a, PivotCustomizationTreeNode b) {
			if(isGrouping)
				return CompareOLAP(a, b);
			return Comparer.Default.Compare(a.Field.ToString(), b.Field.ToString());
		}
		public int CompareOLAP(PivotCustomizationTreeNode a, PivotCustomizationTreeNode b) {
			int res = Compare(a, b, PivotCustomizationTreeNodeType.Measure);
			if(res != 0)
				return res;
			int last = Math.Min(a.FullName.Length, b.FullName.Length) - 1;
			for(int i = 0; i < last; i++) {
				res = Comparer.Default.Compare(a.FullName[i], b.FullName[i]);
				if(res != 0)
					return res;
			}
			res = Compare(b, a, PivotCustomizationTreeNodeType.Hierarchy);
			if(res != 0 && a.FullName.Length == b.FullName.Length)
				return res;
			res = Comparer.Default.Compare(a.FullName[last], b.FullName[last]);
			if(res != 0)
				return res;
			return Comparer.Default.Compare(a.Level, b.Level);
		}
		int Compare(PivotCustomizationTreeNode a, PivotCustomizationTreeNode b, PivotCustomizationTreeNodeType type) {
			if(a.Type == type && b.Type != type)
				return -1;
			if(a.Type != type && b.Type == type)
				return 1;
			return 0;
		}
	}
	[ToolboxItem(false)]
	public class FieldCustomizationTreeBox : CustomCustomizationListBox, IToolTipControlClient {
		PivotCustomizationFieldsTree fullTree;
		protected PivotCustomizationFieldsTree FullTree {
			get {
				if(fullTree == null)
					fullTree = CreateFieldsTree();
				return fullTree;
			}
		}
		public PivotGridViewInfoData Data { get { return CustomizationForm.Data; } }
		protected PivotGridControl PivotGrid { get { return Data != null ? Data.PivotGrid : null; } }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
		public new CustomizationForm CustomizationForm { get { return base.CustomizationForm as CustomizationForm; } }
		public override int GetItemHeight() { return ViewInfo.DefaultHeaderHeight; }
		protected override bool IsDragging { get { return ViewInfo.IsDragging; } }
		public FieldCustomizationTreeBox(CustomizationForm form) : base(form) {
			SetupTooltips();
		}		
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				UnsetTooltips();
			}
		}
		protected virtual PivotCustomizationFieldsTree CreateFieldsTree() {
			return new PivotCustomizationFieldsTree(Data);
		}
		protected virtual PivotCustomizationTreeNodeViewInfo CreateItemViewInfo(PivotCustomizationTreeNode node) {
			if(Data.ShowCustomizationTree)
				return new PivotCustomizationTreeNodeViewInfoTreeItem(node,
					Data.ActiveLookAndFeel, Data.PaintAppearancePrint.FieldHeader);
			else
				return new PivotCustomizationTreeNodeViewInfoListItem(node, (PivotGridViewInfo)Data.ViewInfo);
		}
		protected virtual void SetupTooltips() {
			if(PivotGrid == null) return;
			ToolTipController toolTipController = PivotGrid.ToolTipController;
			if(toolTipController != null)
				toolTipController.AddClientControl(this);
			else
				ToolTipController.DefaultController.AddClientControl(this);
		}
		protected virtual void UnsetTooltips() {
			if(PivotGrid == null) return;
			ToolTipController toolTipController = PivotGrid.ToolTipController;
			if(toolTipController != null)
				toolTipController.RemoveClientControl(this);
			else
				ToolTipController.DefaultController.RemoveClientControl(this);
		}
		public override void Populate() {
			int oldSelectedIndex = SelectedIndex;
			BeginUpdate();
			try {
				Items.Clear();
				FullTree.Update(Data.Fields, Data.IsOLAP && Data.OptionsView.GroupFieldsInCustomizationWindow);
				for(int i = 0; i < FullTree.Count; i++) {
					PivotCustomizationTreeNode node = FullTree[i];
					if(node.IsVisible)
						Items.Add(node);
				}
			} finally {
				EndUpdate();
			}
			if(oldSelectedIndex >= ItemCount)
				oldSelectedIndex = ItemCount - 1;
			if(oldSelectedIndex > -1)
				SelectedIndex = oldSelectedIndex;
		}
		protected override void DoDragDrop(object dragItem) {
			PivotCustomizationTreeNode node = (PivotCustomizationTreeNode)dragItem;
			if(node.Field == null || !node.Field.CanDrag)
				return;
			int oldSelectedIndex = SelectedIndex;
			PivotHeaderViewInfo headerViewInfo = new PivotHeaderViewInfo(ViewInfo, node.Field);
			headerViewInfo.Bounds = new Rectangle(Point.Empty, new Size(node.Field.Width, Data.DefaultFieldHeight));
			ViewInfo.StartDragging(headerViewInfo);
			Populate();
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds) {
			PivotCustomizationTreeNodeViewInfo itemViewInfo = GetItemViewInfo(index);
			PivotCustomizationTreeNode node = itemViewInfo.Node;
			itemViewInfo.Paint(cache, bounds, node == CustomizationForm.PressedItem || node == SelectedItem, Focused);
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			if(e.Button != MouseButtons.Left) return;
			PivotCustomizationTreeNodeViewInfo itemViewInfo = GetItemViewInfo(e.Location);
			Rectangle bounds = GetItemRectangle(IndexFromPoint(e.Location));
			if(itemViewInfo != null && itemViewInfo.HitTestOpenCloseButton(bounds, e.Location)) {
				ChangeExpanded(itemViewInfo.Node);
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			if(e.Button != MouseButtons.Left) return;
			PivotCustomizationTreeNode node = GetItem(e.Location);
			if(node != null) {
				if(node.Field != null)
					CustomizationForm.MoveFieldToPivotGrid(node.Field);
				else
					ChangeExpanded(node);				
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Left:
					ProcessLeftArrowKey(SelectedItem);
					return;
				case Keys.Right:
					ProcessRightArrowKey(SelectedItem);
					return;
				case Keys.Enter:
					ProcessEnterKey();
					return;
			}
			base.OnKeyDown(e);
		}
		protected virtual void ProcessEnterKey() {
			CustomizationForm.MoveFieldToPivotGrid(SelectedItem.Field);
		}
		protected virtual void ProcessRightArrowKey(PivotCustomizationTreeNode node) {
			if(node.CanExpand && !node.IsExpanded) {
				ChangeExpanded(node, true);
				return;
			}
			if(node.CanExpand && node.IsExpanded)
				SelectedIndex++;
		}
		protected virtual void ProcessLeftArrowKey(PivotCustomizationTreeNode node) {
			if(node.IsExpanded && node.CanExpand) {
				ChangeExpanded(node, false);
				return;
			}
			SelectedIndex = FullTree.SearchParentPlace(node);
		}
		protected internal virtual void ChangeExpanded(PivotCustomizationTreeNode node) {
			ChangeExpanded(node, !node.IsExpanded);
		}
		protected internal virtual void ChangeExpanded(PivotCustomizationTreeNode node, bool expanded) {
			if(node == null)
				return;
			node.IsExpanded = expanded;
			Populate();
		}
		public new PivotCustomizationTreeNode SelectedItem {
			get { return (PivotCustomizationTreeNode)base.SelectedItem; }
		}
		public new PivotCustomizationTreeNode GetItem(int index) {
			return (PivotCustomizationTreeNode)base.GetItem(index);
		}
		public PivotCustomizationTreeNode GetItem(Point location) {
			int index = IndexFromPoint(location);
			return GetItem(index);
		}
		protected PivotCustomizationTreeNodeViewInfo GetItemViewInfo(Point location) {
			int index = IndexFromPoint(location);
			return GetItemViewInfo(index);
		}
		protected PivotCustomizationTreeNodeViewInfo GetItemViewInfo(int index) {
			PivotCustomizationTreeNode node = GetItem(index);
			if(node == null)
				return null;
			if(node.ViewInfo == null)
				node.ViewInfo = CreateItemViewInfo(node);
			return node.ViewInfo;
		}
		#region IToolTipControlClient Members
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			PivotCustomizationTreeNodeViewInfo viewInfo = GetItemViewInfo(point);
			if(viewInfo == null) return null;
			return viewInfo.GetToolTipObjectInfo();
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return true; }
		}
		#endregion
	}
	public class PivotCustomizationTreeNodeViewInfo {
		PivotCustomizationTreeNode node;		
		public PivotCustomizationTreeNodeViewInfo(PivotCustomizationTreeNode node) {
			this.node = node;
		}
		public PivotCustomizationTreeNode Node { get { return node; } }
		public virtual void Paint(GraphicsCache cache, Rectangle bounds, bool selected, bool focused) {			
		}
		public virtual bool HitTestOpenCloseButton(Rectangle bounds, Point point) {
			return false;
		}
		public virtual ToolTipControlInfo GetToolTipObjectInfo() {
			return null;
		}
	}
	public class PivotCustomizationTreeNodeViewInfoListItem : PivotCustomizationTreeNodeViewInfo {
		PivotHeaderCustomizationFormViewInfo headerViewInfo;
		public PivotCustomizationTreeNodeViewInfoListItem(PivotCustomizationTreeNode node,
				PivotGridViewInfo viewInfo)
			: base(node) {
			this.headerViewInfo = CreateHeaderViewInfo(node, viewInfo);
		}		
		protected PivotHeaderViewInfo HeaderViewInfo { get { return headerViewInfo; } }
		protected virtual PivotHeaderCustomizationFormViewInfo CreateHeaderViewInfo(PivotCustomizationTreeNode node,
																	PivotGridViewInfo viewInfo) {
			return new PivotHeaderCustomizationFormViewInfo(viewInfo, node.Field);
		}
		public override void Paint(GraphicsCache cache, Rectangle bounds, bool selected, bool focused) {
			HeaderViewInfo.Bounds = bounds;
			HeaderViewInfo.Paint(cache, selected);
		}
		public override bool HitTestOpenCloseButton(Rectangle bounds, Point point) {
			return false;
		}
		public override ToolTipControlInfo GetToolTipObjectInfo() {
			return HeaderViewInfo.GetToolTipObjectInfo(Point.Empty);
		}
	}
	public class PivotCustomizationTreeNodeViewInfoTreeItem : PivotCustomizationTreeNodeViewInfo {		
		OpenCloseButtonViewInfo button;
		PivotCustomizationTreeItemCaptionViewInfo caption;
		PivotCustomizationTreeItemIconViewInfo icon;
		public PivotCustomizationTreeNodeViewInfoTreeItem(PivotCustomizationTreeNode node, 
						UserLookAndFeel activeLookAndFeel, AppearanceObject appearance)
			: base(node) {
			this.caption = CreateCaption(node);
			this.button = CreateButton(node, activeLookAndFeel);
			this.icon = CreateIcon(node);
			Button.Initialize(node.IsExpanded, appearance);
		}		
		protected OpenCloseButtonViewInfo Button { get { return button; } }
		protected PivotCustomizationTreeItemCaptionViewInfo Caption { get { return caption; } }
		protected PivotCustomizationTreeItemIconViewInfo Icon { get { return icon; } }
		protected virtual PivotCustomizationTreeItemIconViewInfo CreateIcon(PivotCustomizationTreeNode node) {
			return new PivotCustomizationTreeItemIconViewInfo(PivotGridControl.CustomizationTreeNodeImages, (int)node.Type);
		}
		protected virtual OpenCloseButtonViewInfo CreateButton(PivotCustomizationTreeNode node, UserLookAndFeel activeLookAndFeel) {
			return node.Field != null ?
				new NullOpenCloseButtonViewInfo() :
				new OpenCloseButtonViewInfo(activeLookAndFeel.Painter.OpenCloseButton);			
		}
		protected virtual PivotCustomizationTreeItemCaptionViewInfo CreateCaption(PivotCustomizationTreeNode node) {
			return new PivotCustomizationTreeItemCaptionViewInfo(node.Caption);
		}
		public override void Paint(GraphicsCache cache, Rectangle bounds, bool selected, bool focused) {
			Calculate(bounds);
			Button.Paint(cache);
			Icon.Paint(cache);
			Caption.SetAppearance(selected, focused);
			Caption.Paint(cache, selected);
		}
		public override bool HitTestOpenCloseButton(Rectangle bounds, Point point) {
			Calculate(bounds);
			return Button.Bounds.Contains(point);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo() {
			return null;
		}
		protected virtual void Calculate(Rectangle bounds) {
			PivotCustomizationTreeRestBounds rest = new PivotCustomizationTreeRestBounds(bounds);
			rest.Indent(Node.Level);
			Button.Calculate(rest);
			Icon.Calculate(rest);
			Caption.Calculate(rest);
		}
	}
	public class PivotCustomizationTreeRestBounds {
		Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } }
		public PivotCustomizationTreeRestBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		public int GetIndent(Rectangle innerRectangle) {
			return (bounds.Height - innerRectangle.Height) / 2;
		}
		public void Indent(int factor) {
			Reduce(bounds.Height * factor);
		}
		public void Reduce(int x) {
			this.bounds.Offset(x / 2, 0);
			this.bounds.Inflate(-x / 2 , 0);
		}
	}
	public class PivotCustomizationTreeBaseViewInfo {
		ObjectInfoArgs info;
		ObjectPainter painter;
		protected PivotCustomizationTreeBaseViewInfo(ObjectPainter painter, ObjectInfoArgs info) {
			this.painter = painter;
			this.info = info;
		}
		public ObjectPainter Painter { get { return painter; } }
		public ObjectInfoArgs Info { get { return info; } }
		public virtual void Paint(GraphicsCache cache) {
			this.info.Cache = cache;
			this.painter.DrawObject(this.info);
		}
		public virtual void Calculate(PivotCustomizationTreeRestBounds calculator) {
			Rectangle bounds = Painter.CalcObjectMinBounds(Info);
			bounds.Offset(calculator.Bounds.X + calculator.GetIndent(bounds), calculator.Bounds.Y + calculator.GetIndent(bounds));
			calculator.Reduce(2 * calculator.GetIndent(bounds) + bounds.Width);
			Bounds = bounds;
		}
		public Rectangle Bounds { get { return info.Bounds; } set { info.Bounds = value; } }
	}
	public class PivotCustomizationTreeItemIconViewInfo : PivotCustomizationTreeBaseViewInfo {
		public PivotCustomizationTreeItemIconViewInfo(object imageList, int index) : base(new GlyphElementPainter(), new GlyphElementInfoArgs(imageList, index, null)) { }
	}
	public class PivotCustomizationTreeItemCaptionViewInfo {
		string caption;
		Rectangle bounds;
		AppearanceObject appearance;
		public PivotCustomizationTreeItemCaptionViewInfo(string caption) {
			this.caption = caption;
		}
		public AppearanceObject Appearance { get { return appearance; } set { appearance = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public void Paint(GraphicsCache cache, bool selected) {
			if(selected)
				Appearance.DrawBackground(cache, CalcSelectionBounds(cache, Bounds));
			Appearance.DrawString(cache, this.caption, Bounds);
		}
		public void SetAppearance(bool selected, bool focused) {
			AppearanceObject appearance = new AppearanceObject();
			if(selected && focused) {
				appearance.BackColor = SystemColors.Highlight;
				appearance.ForeColor = SystemColors.HighlightText;
			} else {
				appearance.BackColor = SystemColors.ButtonFace;
			}
			Appearance = appearance;
		}
		Rectangle CalcSelectionBounds(GraphicsCache cache, Rectangle bounds) {
			const int Offset = 2;
			SizeF size = Appearance.CalcTextSize(cache, this.caption, Bounds.Width);
			Rectangle rect = bounds;
			rect = new Rectangle(rect.X, rect.Y, (int)size.Width, rect.Height);
			rect.Inflate(Offset, -Offset);
			return rect;
		}
		public void Calculate(PivotCustomizationTreeRestBounds calculator) {
			Bounds = calculator.Bounds;
		}
	}
	public class OpenCloseButtonViewInfo : PivotCustomizationTreeBaseViewInfo {
		AppearanceObject appearance;
		public OpenCloseButtonViewInfo(ObjectPainter painter) : base(painter, new OpenCloseButtonInfoArgs(null)) { }
		public new OpenCloseButtonInfoArgs Info { get { return (OpenCloseButtonInfoArgs)base.Info; } }
		public void Initialize(bool expanded, AppearanceObject appearance) {
			Info.Opened = expanded;
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] { appearance });
			Info.SetAppearance(this.appearance);
		}
	}
	internal class NullOpenCloseButtonViewInfo : OpenCloseButtonViewInfo {
		public NullOpenCloseButtonViewInfo() : base(new ObjectPainter()) { }
		public override void Paint(GraphicsCache cache) { }
	}
	public class PivotHeaderCustomizationFormViewInfo : PivotHeaderViewInfo {
		public PivotHeaderCustomizationFormViewInfo(PivotGridViewInfo viewInfo, PivotGridField field) : base(viewInfo, field) {
		}
		protected override bool AddFilter { get { return Field.FilterValues.HasFilter; } }
	}
}
