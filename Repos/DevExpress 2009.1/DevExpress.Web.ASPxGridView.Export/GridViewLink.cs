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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.ASPxGridView.Export;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Utils;
using DevExpress.Web.ASPxClasses;
using System.Web.UI.WebControls;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxClasses.Internal;
using System.Web.UI;
namespace DevExpress.Web.ASPxGridView.Export.Helper {
	public class GridViewLink : Link {
		ASPxGridViewExporter exporter;
		ASPxGridView grid;
		GridViewExportRenderStyles styles;
		Graphics graphics;
		Stack<GridViewPrinter> printers;
		public GridViewLink(ASPxGridViewExporter exporter) {
			this.exporter = exporter;
			this.grid = Exporter.GridView;
			this.styles = new GridViewExportRenderStyles();
			Styles.Init(Exporter.Styles);
			this.graphics = CreateGraphics();
			this.printers = new Stack<GridViewPrinter>();
		}
		protected ASPxGridViewExporter Exporter { get { return exporter; } }
		protected ASPxGridView Grid { get { return grid; } }
		protected GridViewExportRenderStyles Styles { get { return styles; } }
		protected Graphics Graphics { get { return graphics; } }
		protected Stack<GridViewPrinter> Printers { get { return printers; } }
		protected GridViewPrinter ActivePrinter { 
			get {
				if(Printers.Count < 1)
					AddPrinter(Grid);
				return Printers.Peek(); 
			} 
		}
		public PrintingSystem CreatePS() {
			PrintingSystem ps = new PrintingSystem();
			XtraPrinting.InternalAccess.PrintingSystemAccessor.AssignDefaultPageSettings(ps);
			this.PrintingSystem = ps;
			CreateDocument();
			return ps;
		}
		protected void AddPrinter(ASPxGridView grid) {
			Printers.Push(new GridViewPrinter(Exporter, grid, Styles, Graphics, PrintingSystem.Graph, OnDrawDetailGrid, Printers.Count));
		}
		protected void RemovePrinter() {
			ActivePrinter.Dispose();
			Printers.Pop();
		}
		protected override void OnBeforeCreate(EventArgs e) {
			base.OnBeforeCreate(e);
			ps.Graph.PageUnit = System.Drawing.GraphicsUnit.Pixel;
		}
		protected override void OnAfterCreate(EventArgs e) {
			base.OnAfterCreate(e);
			RemovePrinter();
		}
		protected override void Dispose(bool disposing) {
			if(Graphics != null) {
				this.graphics.Dispose();
				this.graphics = null;
			}
			base.Dispose(disposing);
		}
		Graphics CreateGraphics() {
			Bitmap bmp = new Bitmap(10, 10);
			Graphics g = Graphics.FromImage(bmp);
			return g;
		}
		protected override void CreateDetailHeader(BrickGraphics graph) {
			ActivePrinter.CreateDetailHeader(graph);
		}
		protected override void CreateDetail(BrickGraphics graph) {
			ActivePrinter.CreateDetail(graph);
		}
		protected void OnDrawDetailGrid(ASPxGridView detailGrid) {
			AddPrinter(detailGrid);
			AddSubreport(new PointF(0, Exporter.DetailVerticalOffset));
			RemovePrinter();
		}
	}
	public class GridViewExportRenderStyles : GridViewExportStyles {
		public GridViewExportRenderStyles()
			: base(null) {
		}
		public void Init(GridViewExportStyles userStyles) {
			SetupBuiltInDefault();
			Import(Default);
			SetupBuiltInStyles();
			Import(userStyles.Default);
			Import(userStyles);
		}
		void SetupBuiltInDefault() {
			Default.BackColor = Color.White;
			Default.ForeColor = Color.Black;
			Default.BorderColor = Color.DarkGray;
			Default.Paddings.PaddingLeft = Default.Paddings.PaddingRight = Unit.Pixel(2);
			Default.Paddings.PaddingTop = Default.Paddings.PaddingBottom = Unit.Pixel(1);
		}
		void SetupBuiltInStyles() {
			Header.BackColor = Color.Gray;
			Header.ForeColor = Color.White;
			Header.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
			GroupRow.BackColor = Color.LightGray;
			Header.ForeColor = Color.White;
			Preview.ForeColor = Color.Blue;
			Footer.BackColor = Color.LightYellow;
			GroupFooter.BackColor = Color.LightYellow;
			Title.HorizontalAlign = HorizontalAlign.Center;
			Title.Font.Bold = true;
			HyperLink.ForeColor = Color.Blue;
			HyperLink.Font.Underline = true;
			AlternatingRowCell.BackColor = Color.FromArgb(0xededeb);
		}
		void Import(Style commonStyle) {
			Cell.CopyFrom(commonStyle);
			Header.CopyFrom(commonStyle);
			GroupRow.CopyFrom(commonStyle);
			Preview.CopyFrom(commonStyle);
			Footer.CopyFrom(commonStyle);
			GroupFooter.CopyFrom(commonStyle);
			Title.CopyFrom(commonStyle);
			HyperLink.CopyFrom(commonStyle);
			AlternatingRowCell.CopyFrom(commonStyle);
		}
		void Import(GridViewExportStyles styles) {
			Cell.CopyFrom(styles.Cell);
			Header.CopyFrom(styles.Header);
			GroupRow.CopyFrom(styles.GroupRow);
			Preview.CopyFrom(styles.Preview);
			Footer.CopyFrom(styles.Footer);
			GroupFooter.CopyFrom(styles.GroupFooter);
			Title.CopyFrom(styles.Title);
			HyperLink.CopyFrom(styles.HyperLink);
			AlternatingRowCell.CopyFrom(styles.AlternatingRowCell);
		}
	}
}
