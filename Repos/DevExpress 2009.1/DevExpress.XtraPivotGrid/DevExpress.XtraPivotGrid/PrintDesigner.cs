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
using System.IO;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraTab;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using System.Drawing.Imaging;
namespace DevExpress.XtraPivotGrid.Frames {
	[ToolboxItem(false)]
	public class PivotGridPrinting : ControlPrintingBase {
		public override Size UserControlSize {
			get { return new Size(620, 345); }
		}
		protected override ImageCollection CreateImageCollection() {
			return DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.PivotGridPrintOptions.png", GetType().Assembly, new Size(16, 16));
		}
		protected new PivotGridControl EditingObject { get { return (PivotGridControl)base.EditingObject; } }
		protected new PivotGridControl SampleControl { get { return (PivotGridControl)base.SampleControl; } }
		protected override int TabControlWidth { get { return 190; } }
		protected override Control CreateSampleControl() {
			return new PivotGridPrintControl();
		}
		protected virtual string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		protected override void CreateOptionItems() {
			OptionItems.Add("OptionsPrint.PrintHorzLines", "OptionsView.ShowHorzLines", GetLocalizedString(PivotGridStringId.PrintDesignerHorizontalLines), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryLines), 0);
			OptionItems.Add("OptionsPrint.PrintVertLines", "OptionsView.ShowVertLines", GetLocalizedString(PivotGridStringId.PrintDesignerVerticalLines), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryLines), 1);
			OptionItems.Add("OptionsPrint.PrintFilterHeaders", "OptionsView.ShowFilterHeaders", GetLocalizedString(PivotGridStringId.PrintDesignerFilterHeaders), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 2);
			OptionItems.Add("OptionsPrint.PrintDataHeaders", "OptionsView.ShowDataHeaders", GetLocalizedString(PivotGridStringId.PrintDesignerDataHeaders), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 3);
			OptionItems.Add("OptionsPrint.PrintColumnHeaders", "OptionsView.ShowColumnHeaders", GetLocalizedString(PivotGridStringId.PrintDesignerColumnHeaders), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 4);
			OptionItems.Add("OptionsPrint.PrintRowHeaders", "OptionsView.ShowRowHeaders", GetLocalizedString(PivotGridStringId.PrintDesignerRowHeaders), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 5);
			OptionItems.Add("OptionsPrint.PrintHeadersOnEveryPage", GetLocalizedString(PivotGridStringId.PrintDesignerHeadersOnEveryPage), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 7);
			OptionItems.Add("OptionsPrint.PrintUnusedFilterFields", GetLocalizedString(PivotGridStringId.PrintDesignerUnusedFilterFields), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryHeaders), 8);
			OptionItems.Add("OptionsPrint.UsePrintAppearance", GetLocalizedString(PivotGridStringId.PrintDesignerUsePrintAppearance), GetLocalizedString(PivotGridStringId.PrintDesignerPageBehavior), GetLocalizedString(PivotGridStringId.PrintDesignerPageOptions), 6);
			OptionItems.Add("OptionsPrint.MergeColumnFieldValues", GetLocalizedString(PivotGridStringId.PrintDesignerMergeColumnFieldValues), GetLocalizedString(PivotGridStringId.PrintDesignerPageBehavior), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryFieldValues), 9);
			OptionItems.Add("OptionsPrint.MergeRowFieldValues", GetLocalizedString(PivotGridStringId.PrintDesignerMergeRowFieldValues), GetLocalizedString(PivotGridStringId.PrintDesignerPageBehavior), GetLocalizedString(PivotGridStringId.PrintDesignerCategoryFieldValues), 10);
		}
		protected override void ApplyOptionsCore() {
			EditingObject.OptionsPrint.Assign(SampleControl.OptionsPrint);
		}
	}
	[ToolboxItem(false)]
	public class PivotGridPrintControl : PivotGridControl {
		DynamicPrintHelper printHelper;
		public PivotGridPrintControl() {
			Dock = DockStyle.Fill;
			Enabled = false;
			HScrollBar.Visible = false;
			VScrollBar.Visible = false;
			CreateFields();
			DataSource = CreateDataSource().DefaultView;
			this.printHelper = new DynamicPrintHelper();
		}
		protected void CreateFields() {
			Fields.Add("Category Name", PivotArea.RowArea);
			Fields.Add("Product Name", PivotArea.RowArea);
			Fields.Add("Year", PivotArea.ColumnArea);
			Fields.Add("Sale", PivotArea.DataArea);
			Fields.Add("Employee Name", PivotArea.FilterArea);
			Fields["Year"].Width = 70;
			Fields["Sale"].CellFormat.FormatType = FormatType.Numeric;
			Fields["Sale"].CellFormat.FormatString = "c";
		}
		protected DataTable CreateDataSource() {
			DataTable table = new DataTable();
			table.Columns.Add("Category Name", typeof(string));
			table.Columns.Add("Product Name", typeof(string));
			table.Columns.Add("Year", typeof(int));
			table.Columns.Add("Sale", typeof(float));
			table.Columns.Add("Quantity", typeof(int));
			table.Columns.Add("Employee Name", typeof(string));
			table.Rows.Add(new object[] { "Beverages", "Chai", 1995, 5070.60, 319, null });
			table.Rows.Add(new object[] { "Beverages", "Chai", 1996, 6295.50, 399, null });
			table.Rows.Add(new object[] { "Beverages", "Ipoh Coffee", 1995, 10034.90, 228, null });
			table.Rows.Add(new object[] { "Beverages", "Ipoh Coffee", 1996, 8560.60, 216, null });
			table.Rows.Add(new object[] { "Confections", "Chocolade", 1995, 1282.01, 130, null });
			table.Rows.Add(new object[] { "Confections", "Chocolade", 1996, 86.70, 8, null });
			table.Rows.Add(new object[] { "Confections", "Scottish Breads", 1995, 3909.00, 380, null });
			table.Rows.Add(new object[] { "Confections", "Scottish Breads", 1996, 4175.00, 354, null });
			return table;
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			e.Graphics.Clear(Color.White);
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(MemoryStream stream = new MemoryStream()) {
				printHelper.ExportToImage(this, stream, new ImageExportOptions(ImageFormat.Bmp));
				stream.Position = 0;
				Image image = Image.FromStream(stream);
				e.Graphics.DrawImageUnscaled(image, 0, 0);
			}
		}
		protected override bool IsHScrollBarVisible { get { return false; } }
		protected override bool IsVScrollBarVisible { get { return false; } }
	}
}
