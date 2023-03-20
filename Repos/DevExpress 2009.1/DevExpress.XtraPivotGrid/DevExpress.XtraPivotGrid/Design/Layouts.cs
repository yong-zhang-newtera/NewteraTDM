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
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Frames;
using DevExpress.Utils;
using DevExpress.XtraTab;
namespace DevExpress.XtraPivotGrid.Frames {
	[ToolboxItem(false)]
	public class Layouts : LayoutsBase{
		DevExpress.XtraPivotGrid.PivotGridControl grid = null;
		public Layouts() : base(0)  {
		}
		protected override string DescriptionText { get { return "Modify the PivotGrid's layout (sorting settings, field arrangement) and click the Apply button to apply the modifications to the current PivotGrid. You can also save the layout to an XML file (this can be loaded and applied to other views at design time and runtime)."; } }
		protected PivotGridControl EditingGrid { get { return EditingObject as PivotGridControl; } }
		protected PivotGridControl Grid { get { return grid; } }
		protected override Control CreatePreviewControl() {
			grid = CreatePivotGridControl();
			AssignLayouts(EditingGrid, Grid);
			Grid.OLAPConnectionString = EditingGrid.OLAPConnectionString;
			if(dataSet != null) 
				Grid.DataSource = dataSet.Tables[tableName];
			else Grid.DataSource = EditingGrid.DataSource;
			Grid.ShowCustomizationForm += new System.EventHandler(this.view_ShowCustomizationForm);
			Grid.HideCustomizationForm += new System.EventHandler(this.view_HideCustomizationForm);
			return Grid;
		}
		protected virtual PivotGridControl CreatePivotGridControl() {
			return new PivotGridLayoutEditorControl();
		}
		protected override void ShowColumnsCustomization() {
			Grid.FieldsCustomization();
		}
		protected override void HideColumnsCustomization() {
			Grid.DestroyCustomization();
		}
		protected override void SetColumnSelectorText(bool showing) {
			SetColumnSelectorCaption(showing ? "Hide fields &selector" : "Show fields &selector");	
		}
		protected override void ApplyLayouts() {
			AssignLayouts(Grid, EditingGrid);
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			Grid.RestoreLayoutFromXml(fileName, OptionsLayoutBase.FullLayout);
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
			Grid.SaveLayoutToXml(fileName, OptionsLayoutBase.FullLayout);
		}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected override void SetControlDataSource(DataView dataView) {
			Grid.DataSource = dataView;
			SetLayoutChanged(false);
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingGrid.DataSource == null)  return null;
			try {
				DataView dv = EditingGrid.ListSource as DataView;
				if(dv != null) return dv.Table;
			}
			catch {
			}
			return null;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			Grid.GridLayout += new EventHandler(Grid_Layout);
		}
		void AssignLayouts(PivotGridControl source, PivotGridControl destination) {
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			source.SaveLayoutToStream(str, OptionsLayoutBase.FullLayout);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			destination.RestoreLayoutFromStream(str, OptionsLayoutBase.FullLayout);
			str.Close();
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			foreach(object comp in EditingGrid.Container.Components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingGrid.DataSource, EditingGrid.DataMember);
		}
		void view_ShowCustomizationForm(object sender, System.EventArgs e) {
			OnShowCustomizationForm();
		}
		void view_HideCustomizationForm(object sender, System.EventArgs e) {
			OnHideCustomizationForm();
		}
		void Grid_Layout(object sender, EventArgs e) {
			if(Grid.IsLoading) return;
			SetLayoutChanged(true);
		}
	}
	[ToolboxItem(false)]
	public class PivotGridLayoutEditorControl : DevExpress.XtraPivotGrid.PivotGridControl {
		protected override bool IsDesignModeCore { get { return true; } }
	}
}
