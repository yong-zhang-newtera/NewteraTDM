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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridDragManager : DragManager {
		PivotHeaderViewInfo headerViewInfo;
		Rectangle drawRectangle;
		Point lastMovePt;
		DragState lastDragState;
		bool accept;
		public PivotGridDragManager(PivotHeaderViewInfo headerViewInfo) {
			this.headerViewInfo = headerViewInfo;
			this.drawRectangle = Rectangle.Empty;
			this.lastMovePt = Point.Empty;
			this.lastDragState = DragState.None;
			this.accept = false;
		}
		public PivotHeaderViewInfo HeaderViewInfo { get { return headerViewInfo; } }
		public PivotHeadersViewInfo HeadersViewInfo { get { return HeaderViewInfo.HeadersViewInfo; } }
		public PivotGridViewInfo ViewInfo { get { return HeaderViewInfo.ViewInfo; } }
		public PivotGridViewInfoData Data { get { return HeaderViewInfo.Data; } }
		protected Control Control { get { return Data.PivotGrid; } }
		public PivotGridField Field { get { return HeaderViewInfo.Field; } }
		public void DoDragDrop() {
			base.DoDragDrop(HeaderSize, HeaderViewInfo.ControlBounds.Location);
			DrawReversibleFrame();
			ViewInfo.MouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
			if(LastDragState == DragState.Move) {
				if(IsMouseOverCustomizationForm) {
					HideField();
				} else {
					PivotArea area;
					int areaIndex = ViewInfo.GetNewFieldPosition(Field, this.lastMovePt, out area);
					if(areaIndex > -1) {
						Data.SetFieldAreaPosition(Field, area, areaIndex);
						if(Data.OptionsBehavior.ApplyBestFitOnFieldDragging)
							Field.BestFit();
					}
				}
			} else {
				if(LastDragState == DragState.Remove) HideField();
			}
			ViewInfo.Invalidate();
		}		
		protected override DragState GetDragState(Point pt) {
			if(lastMovePt.Equals(pt)) return this.lastDragState;
			lastMovePt = pt;
			this.lastDragState = DragState.Remove;
			if(IsMouseOverCustomizationForm) {
				this.lastDragState = Data.AllowHideFields ? DragState.Move : DragState.None;
				ShowDragFrame(new Point(-1, -1), Rectangle.Empty);
				return this.lastDragState;
			}
			Rectangle newDrawRectangle = ViewInfo.GetDragDrawRectangle(Field, pt);
			this.accept = ViewInfo.AcceptDragDrop(pt);
			if(this.accept) {
				PivotArea area;
				int areaIndex = ViewInfo.GetNewFieldPosition(Field, this.lastMovePt, out area);
				this.accept = Data.FieldAreaChanging(Field, area, areaIndex);
			}
			this.lastDragState = DragState.Move;
			if(!this.accept) {
				this.lastDragState = Data.AllowHideFields ? DragState.Remove : DragState.None;
				newDrawRectangle = Rectangle.Empty;
			}
			ShowDragFrame(pt, newDrawRectangle);
			return this.lastDragState;
		}
		bool IsMouseOverCustomizationForm {
			get {
				if(Data.CustomizationForm == null) return false;
				Rectangle bounds = Data.CustomizationForm.Bounds;
				if(Data.CustomizationForm.Parent != null)
					bounds.Location = Data.CustomizationForm.PointToScreen(new Point(0, 0));
				return bounds.Contains(this.lastMovePt);
			}
		}
		void HideField() {
			if(headerViewInfo.CanHide)
				Field.Visible = false;
		}
		protected override void RaisePaint (PaintEventArgs e) {
			ViewInfoPaintArgs pe = new ViewInfoPaintArgs(Data.ControlOwner, e);
			PivotHeaderViewInfoBase[] headers = GetDragHeaders();
			int x = 0;
			for(int i = 0; i < headers.Length; i ++) {
				headers[i].PaintDragHeader(pe, x);
				x += headers[i].Bounds.Width;
			}
		}
		protected Size HeaderSize {
			get {	
				PivotHeaderViewInfoBase[] headers = GetDragHeaders();
				int width = 0;
				for(int i = 0; i < headers.Length; i ++) {
					width += headers[i].Bounds.Width;
				}
				return new Size(width, HeaderViewInfo.Bounds.Height);
			}
		}
		protected PivotHeaderViewInfo[] GetDragHeaders() {
			if(Field.Group == null || HeadersViewInfo == null) {
				return new PivotHeaderViewInfo[1] {HeaderViewInfo};
			} else {
				PivotHeaderViewInfo[] headers = new PivotHeaderViewInfo[Field.Group.VisibleCount];
				for(int i = 0; i < Field.Group.VisibleCount; i ++) {
					headers[i] = HeadersViewInfo[Field.Group.AreaIndex + i];
				}
				return headers;
			}
		}
		void ShowDragFrame(Point pt, Rectangle newDrawRectangle) {
			if(!newDrawRectangle.Equals(this.drawRectangle)) {
				DrawReversibleFrame();
				this.drawRectangle = newDrawRectangle;
				DrawReversibleFrame();
			}
		}
		void DrawReversibleFrame() {
			if (!this.drawRectangle.IsEmpty) {
				DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleFrame(Control.Handle, this.drawRectangle);
			}
		}
	}
}
