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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	[ToolboxItem(false)]
	public class BaseControl : Control, ISupportInitialize {
		protected int fInitializing;
		bool _firstInit;
		bool loadFired = false;
		bool firstPaint = true;
		public BaseControl() {
			this.fInitializing = 0;
			this._firstInit = true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RemoveHookOnLoaded();
			}
			base.Dispose(disposing);
		}
		public virtual void BeginInit() {
			if(this._firstInit) {
				this._firstInit = false;
				OnFirstInit();
			}
			this.loadFired = false;
			this.fInitializing ++;
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return this.fInitializing != 0; } }
		public virtual void EndInit() {
			if(-- this.fInitializing == 0) OnEndInit();
		}
		protected virtual void OnFirstInit() { 
		}
		protected virtual void OnEndInit() {
			HookOnLoaded();
			if(IsHandleCreated && !DesignMode) {
				if(FireOnLoadOnPaint && !IsPainted && !Visible) return; 
				OnLoaded();
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!DesignMode && Visible && IsHandleCreated && FireOnLoadOnPaint) OnLoaded();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			HookOnLoaded();
			if(!IsLoading && !DesignMode) {
				if(FireOnLoadOnPaint && !IsPainted) return; 
				OnLoaded();
			}
		}
		bool IsPainted { get { return !firstPaint; } }
		protected virtual bool FireOnLoadOnPaint { get { return false; } }
		protected override void OnPaint(PaintEventArgs e) {
			bool first = this.firstPaint;
			this.firstPaint = false;
			if(first && FireOnLoadOnPaint) {
				OnLoaded();
			}
		}
		IDesignerHost designerHost = null;
		protected virtual void RemoveHookOnLoaded() {
			if(designerHost != null) {
				designerHost.LoadComplete -= new EventHandler(OnDesignerHost_LoadComplete);
				designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
			}
			this.designerHost = null;
		}
		protected virtual void HookOnLoaded() {
			RemoveHookOnLoaded();
			this.designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null) {
				this.designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
				designerHost.LoadComplete += new EventHandler(OnDesignerHost_LoadComplete);
			}
		}
		protected void OnDesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if(this.designerHost != null && !this.designerHost.Loading 
				&& (!this.designerHost.InTransaction || this.designerHost.TransactionDescription == null)) 
				OnLoaded();
		}
		protected void OnDesignerHost_LoadComplete(object sender, EventArgs e) {
			if(IsDisposed) return;
			OnLoaded();
		}
		protected virtual bool IsInitialized { get { return loadFired; } }
		protected virtual void OnLoaded() {
			if(IsLoading || IsInitialized) return;
			this.loadFired = true;
		}
	}
	public abstract class BaseViewInfoControl : BaseControl, IViewInfoControl {
		DevExpress.XtraEditors.HScrollBar hScrollBar;
		DevExpress.XtraEditors.VScrollBar vScrollBar;
		int scrollBarUpdateCount;
		int controlUpdateCount;
		bool isScrollBarsReady;
		public BaseViewInfoControl() {
			this.scrollBarUpdateCount = 0;
			this.controlUpdateCount = 0;
			this.hScrollBar = new DevExpress.XtraEditors.HScrollBar();
			HScrollBar.Parent = this;
			HScrollBar.ValueChanged += new EventHandler(OnScrollBarsValueChanged);
			this.vScrollBar = new DevExpress.XtraEditors.VScrollBar();
			VScrollBar.Parent = this;
			VScrollBar.ValueChanged += new EventHandler(OnScrollBarsValueChanged);
		}
		public override void Refresh() {
			InternalRefresh();
			base.Refresh();
		}
		public virtual void BeginUpdate() { 
			this.controlUpdateCount ++;
			BeginScrollBarsUpdate();
		}
		protected virtual void CancelUpdate() {
			this.controlUpdateCount --;
			if(this.controlUpdateCount < 0) 
				this.controlUpdateCount = 0;
			EndScrollBarsUpdate();
		}
		public void EndUpdate() {
			CancelUpdate();
			if(! IsUpdating) 
				Refresh();
		}
		protected virtual BaseViewInfo RootViewInfo { get { return null; } }
		protected virtual Rectangle ScrollableRectangle { get { return ClientRectangle; } }
		protected override void CreateHandle() {
			base.CreateHandle();
			InvalidateScrollBars();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible && !Disposing) {
				InvalidateScrollBars();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(RootViewInfo == null) {
				base.OnPaint(e);
				return;
			}
			RootViewInfo.EnsureIsCalculated();
			UpdateScrollBars();
			RootViewInfo.Paint(this, e);
			DrawScrollBarsGlyph(e);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			InvalidateScrollBars();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			if(RootViewInfo != null)
				RootViewInfo.KeyDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseMove(e);
			if(e.Handled) return;
			if(RootViewInfo != null)
				RootViewInfo.MouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseDown(e);
			if(e.Handled) return;
			Focus();
			if(RootViewInfo != null)
				RootViewInfo.MouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseUp(e);
			if(e.Handled) return;
			if(RootViewInfo != null)
				RootViewInfo.MouseUp(e);
		}
		protected override void OnDoubleClick(EventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(this, ee);
			base.OnDoubleClick(e);
			if(e.Handled) return;
			if(RootViewInfo != null)
				RootViewInfo.DoubleClick();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(RootViewInfo != null)
				RootViewInfo.MouseEnter();
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(RootViewInfo != null)
				RootViewInfo.MouseLeave();
		}
		protected override void OnMouseWheel(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseWheel(e);
			if(e.Handled) return;
			if(VScrollBar != null && VScrollBar.Visible)
				VScrollBar.Value += e.Delta > 0 ? -1 : 1;
		}
		protected DevExpress.XtraEditors.HScrollBar HScrollBar { get { return hScrollBar; } }
		protected DevExpress.XtraEditors.VScrollBar VScrollBar { get { return vScrollBar; } }
		protected virtual void InvalidateScrollBars() {
			isScrollBarsReady = false;
		}
		protected virtual void UpdateScrollBars() {
			if(!IsHandleCreated || isScrollBarsReady) return;
			BeginScrollBarsUpdate();
			try {
				UpdateHScrollBar();
				UpdateVScrollBar();
			} finally {
				EndScrollBarsUpdate();
				isScrollBarsReady = true;
			}
		}
		protected virtual void InternalRefresh() {
			InvalidateScrollBars();
		}
		protected abstract bool IsHScrollBarVisible { get; }
		protected abstract bool IsVScrollBarVisible { get; }
		protected abstract ScrollArgs HScrollBarInfo { get; }
		protected abstract ScrollArgs VScrollBarInfo { get; }
		protected abstract void ScrollBarsValueChanged(Point newValue);
		protected bool IsUpdating { get { return this.controlUpdateCount > 0; } }
		protected Size ScrollBarsSize { get { return new Size(HScrollBar.Height, VScrollBar.Width); } }
		Rectangle IViewInfoControl.ClientRectangle { get { return ClientRectangle; } }
		void IViewInfoControl.Invalidate(Rectangle bounds) {
			Invalidate(bounds);
		}
		Control IViewInfoControl.ControlOwner { get { return this; } }
		void IViewInfoControl.UpdateScrollBars() { this.UpdateScrollBars(); }
		void IViewInfoControl.InvalidateScrollBars() { this.InvalidateScrollBars(); }
		bool IViewInfoControl.IsDesignMode { get { return IsDesignModeCore; } }
		protected virtual bool IsDesignModeCore { get { return Site != null ? Site.DesignMode : false; } }
		protected virtual Color ScrollBarsGlyphColor { get { return SystemColors.Control; } }
		void DrawScrollBarsGlyph(PaintEventArgs e) {
			if(!IsHScrollBarVisible || !IsVScrollBarVisible) return;
			Rectangle bounds = new Rectangle(HScrollBar.Right, VScrollBar.Bottom, VScrollBar.Width, HScrollBar.Height);
			if(bounds.IsEmpty) return;
			Brush brush = new SolidBrush(ScrollBarsGlyphColor);
			e.Graphics.FillRectangle(brush, bounds);
			brush.Dispose();
		}
		void UpdateHScrollBar() {
			HScrollBar.Visible = IsHScrollBarVisible;
			if(!HScrollBar.Visible) return;
			int vScrollBarHeight = IsVScrollBarVisible ? VScrollBar.Width : 0;
			Rectangle bounds = ScrollableRectangle;
			HScrollBar.Bounds = new Rectangle(bounds.Left, bounds.Height + bounds.Top - HScrollBar.Height, 
				bounds.Width - vScrollBarHeight, HScrollBar.Height);
			HScrollBarInfo.AssignTo(HScrollBar);
		}
		void UpdateVScrollBar() {
			VScrollBar.Visible = IsVScrollBarVisible;
			if(!VScrollBar.Visible) return;
			int hScrollBarHeight = IsHScrollBarVisible ? HScrollBar.Height : 0;
			Rectangle bounds = ScrollableRectangle;
			VScrollBar.Bounds = new Rectangle(bounds.Width + bounds.Left - VScrollBar.Width, bounds.Top,
				VScrollBar.Width, bounds.Height - hScrollBarHeight);
			VScrollBarInfo.AssignTo(VScrollBar);
		}
		void BeginScrollBarsUpdate() { this.scrollBarUpdateCount ++; }
		void EndScrollBarsUpdate() {
			this.scrollBarUpdateCount --;
			if(this.scrollBarUpdateCount < 0) 
				this.scrollBarUpdateCount = 0;
		}
		bool IsScrollBarsUpdating { get { return this.scrollBarUpdateCount > 0; } }
		void OnScrollBarsValueChanged(object sender, EventArgs e) {
			if(IsScrollBarsUpdating) return;
			ScrollBarsValueChanged(new Point(HScrollBar.Value, VScrollBar.Value));
		}
	}
}
