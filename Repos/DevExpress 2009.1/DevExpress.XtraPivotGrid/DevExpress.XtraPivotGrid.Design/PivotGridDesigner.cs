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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPivotGrid.Design  {
	public class PivotGridDataSourceActionList : DesignerActionList {
		PivotGridControl pivotGrid;
		public PivotGridDataSourceActionList(PivotGridControl pivotGrid) : base(pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		protected PivotGridControl PivotGrid { get { return pivotGrid; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source"));
			res.Add(new DesignerActionPropertyItem("OLAPConnectionString", "Choose OLAP Data Source"));
			return res;
		}
#if DXWhidbey
		[AttributeProvider(typeof(IListSource))]
#endif
		public object DataSource {
			get { return PivotGrid.DataSource; }
			set {
				EditorContextHelper.SetPropertyValue(PivotGrid.Site, PivotGrid, "DataSource", value);
				EditorContextHelper.RefreshSmartPanel(PivotGrid);
			}
		}
		[Editor("DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))]
		public string OLAPConnectionString {
			get { return PivotGrid.OLAPConnectionString; }
			set {
				EditorContextHelper.SetPropertyValue(PivotGrid.Site, PivotGrid, "OLAPConnectionString", value);
				EditorContextHelper.RefreshSmartPanel(PivotGrid);
			}
		}
	}
	public class PivotGridAddFieldsActionList : DesignerActionList {
		PivotGridControlDesigner designer;
		PivotArea area;
		public PivotGridAddFieldsActionList(PivotGridControlDesigner designer) : base(designer.Component) {
			this.designer = designer;
			this.area = PivotArea.RowArea;
		}
		protected PivotGridControlDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("Area", "Field area for a new field"));
			res.Add(new DesignerActionMethodItem(this, "AddField", "Add Field to " + Area.ToString()));
			res.Add(new DesignerActionMethodItem(this, "EditFields", "Run Designer..."));
			return res;
		}
		public PivotArea Area { get { return area; } 
			set {
				if (Area == value) return;
				area = value;
				EditorContextHelper.RefreshSmartPanel(Component);
			} 
		}
		public void AddField() {
			designer.AddField(Area);
		}
		public void EditFields() {
			designer.RunDesigner();
		}
	}
	public class PivotGridControlDesigner : BaseControlDesigner {
		DesignerVerbCollection verbs;
		PivotGridEditorForm editor;
		ISelectionService selectionService;
		bool isFieldSelected;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited { get { return false; } }
		public PivotGridControlDesigner()	{
			verbs =	new DesignerVerbCollection(	new DesignerVerb[] {
			   new DesignerVerb("About", new EventHandler(OnAboutClick)),
			   new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick)),
			   new DesignerVerb("&Add field", new EventHandler(OnAddFieldClick)),
			});
			this.selectionService = null;
			this.isFieldSelected  = false;
			this.editor = null;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(SelectionService != null) {
				SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			}
		}
		protected override void Dispose(bool disposing) {
			if(this.selectionService != null) {
				this.selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
				this.selectionService = null;
			}
			if(disposing) {
				Editor = null;
			}
			base.Dispose(disposing);
		}
		public override ICollection AssociatedComponents {
			get {
				if(PivotGrid == null) return base.AssociatedComponents;
				ArrayList fields = new ArrayList();
				foreach(PivotGridField field in PivotGrid.Fields) {
					fields.Add(field);
				}
				return fields;
			}
		}
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		public void AddField(PivotArea area) {
			if (PivotGrid == null) return;
			PivotGridField field = PivotGrid.Fields.Add(string.Empty, area);
			if (this.selectionService != null)
				this.selectionService.SetSelectedComponents(new object[] { field });
		}
		public void RunDesigner() {
			if (PivotGrid == null) return;
			Editor = new PivotGridEditorForm();
			editor.InitEditingObject(PivotGrid);
			Editor.ShowDialog();
			Editor = null;
		}
		protected ISelectionService SelectionService {
			get {
				if(selectionService == null)
					selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
				return selectionService;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new PivotGridDataSourceActionList(PivotGrid));
			list.Add(new PivotGridAddFieldsActionList(this));
			base.RegisterActionLists(list);
		}
		protected PivotGridControl PivotGrid { get { return Control as PivotGridControl; } }
		protected PivotGridEditorForm Editor {  
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		protected override bool EnableDragRect { get { return false; } }
		protected override void OnDragEnter(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override void OnDragOver(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected virtual bool GetHitTestCore(Point client) {
			if(DebuggingState) return false;
			if(!Rectangle.Inflate(Control.ClientRectangle, -3, -3).Contains(client)) return false;
			if(PivotGrid == null) return false;
			return PivotGrid.GetFieldAt(client) != null || PivotGrid.CanResizeField(client);
		}
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(DebuggingState) return res;
			if(PivotGrid == null || res) return res;
			Point client = Control.PointToClient(point);
			return GetHitTestCore(client);
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if(SelectionService == null || PivotGrid == null) return;
			bool oldIsFieldSelected = isFieldSelected;
			PivotGridField field = SelectionService.PrimarySelection as PivotGridField;
			isFieldSelected = field != null ? PivotGrid.Fields.Contains(field) : false;
			if(oldIsFieldSelected && !isFieldSelected)
				PivotGrid.Invalidate();
		}
		void OnAboutClick(object sender, EventArgs e) {
			PivotGridControl.About();			
		}
		void OnDesignerClick(object sender, EventArgs e) {
			RunDesigner();
		}
		void OnAddFieldClick(object sender, EventArgs e) {
			AddField(PivotArea.RowArea);
		}
	}
}
